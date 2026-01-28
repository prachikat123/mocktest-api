
using Auth_WebAPI.Data;
using Auth_WebAPI.Helpers;
using Auth_WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Auth_WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(8080); // HTTP
                options.ListenLocalhost(8081, listenOptions =>
                {
                    listenOptions.UseHttps();
                });
            });

            // CORS Config
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy
                        //.WithOrigins("http://localhost:4200") // Angular URL
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Bearar token Authentication Config
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var authHeader = context.Request.Headers["Authorization"].ToString();

                            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                            {
                                context.Fail("Invalid Authorization header");
                                return Task.CompletedTask;
                            }

                            var token = authHeader.Replace("Bearer ", "").Trim();

                            using SqlConnection con =
                                new SqlConnection(
                                    builder.Configuration.GetConnectionString("cs"));

                            string sql = @"SELECT COUNT(*) 
                                           FROM BlacklistedTokens 
                                           WHERE Token = @t";

                            SqlCommand cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@t", SqlDbType.NVarChar).Value = token;

                            con.Open();
                            int count = (int)cmd.ExecuteScalar();
                            con.Close();

                            if (count > 0)
                            {
                                context.Fail("Token has been logged out");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            builder.Services.AddAuthorization();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("cs")));

            builder.Services.AddScoped<JwtHelper>();

            builder.Services.AddScoped<IResultService, ResultService>();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "My API",
                        Version = "v1" 
                    });

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors("AllowAngular");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

       
    }
}
