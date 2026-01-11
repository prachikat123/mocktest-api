using Auth_WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Auth_WebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Question>Questions { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<MockTest> MockTests { get; set; }



    }
}