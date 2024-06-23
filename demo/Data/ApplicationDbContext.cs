using demo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace demo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobCandidate> JobCandidates { get; set; }
        public DbSet<Recuiter> Recuiters { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedUserRole(builder);
            SeedBrand(builder);
            SeedProduct(builder);
            SeedCandidate(builder);
            SeedJob(builder);
            SeedJobCandidate(builder);
            SeedRecuiter(builder);
        }

        private void SeedUserRole(ModelBuilder builder)
        {
            //A) Setup IdentityUser
            //1. Create accounts
            var adminAccount = new IdentityUser
            {
                Id = "admin",
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true
            };

            var candidateAccount = new IdentityUser
            {
                Id = "candidate",
                UserName = "candidate@gmail.com",
                Email = "candidater@gmail.com",
                NormalizedUserName = "CANDIDATE@GMAIL.COM",
                NormalizedEmail = "CANDIDATE@GMAIL.COM",
                EmailConfirmed = true
            };

            var recuiterAccount = new IdentityUser
            {
                Id = "recuiter",
                UserName = "recuiter@gmail.com",
                Email = "recuiter@gmail.com",
                NormalizedUserName = "RECUITER@GMAIL.COM",
                NormalizedEmail = "RECUITER@GMAIL.COM",
                EmailConfirmed = true
            };

            //2. Declare hasher for password encryption
            var hasher = new PasswordHasher<IdentityUser>();

            //3. Setup password for accounts
            adminAccount.PasswordHash = hasher.HashPassword(adminAccount, "123456");
            recuiterAccount.PasswordHash = hasher.HashPassword(recuiterAccount, "123456");
            candidateAccount.PasswordHash = hasher.HashPassword(candidateAccount, "123456");

            //4. Add accounts to database
            builder.Entity<IdentityUser>().HasData(adminAccount, recuiterAccount, candidateAccount);

            //B) Setup IdentityRole
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "role1",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                },
                 new IdentityRole
                 {
                     Id = "role2",
                     Name = "Recuiter",
                     NormalizedName = "RECUITER"
                 },
                 new IdentityRole
                 {
                     Id = "role3",
                     Name = "Candidate",
                     NormalizedName = "CANDIDATE"
                 });

            //C) Setup IdentityUserRole
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = "admin",
                    RoleId = "role1"
                },
                new IdentityUserRole<string>
                {
                    UserId = "candidate",
                    RoleId = "role3"
                },
                new IdentityUserRole<string>
                {
                    UserId = "recuiter",
                    RoleId = "role2"
                }
             );
        }

        private void SeedBrand(ModelBuilder builder)
        {
            builder.Entity<Brand>().HasData(
                new Brand
                {
                    Id = 1,
                    Name = "Apple"
                },
                new Brand
                {
                    Id = 2,
                    Name = "Dell"
                },
                new Brand
                {
                    Id = 3,
                    Name = "LG"
                }
             );
        }

        private void SeedProduct(ModelBuilder builder)
        {
            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Macbook Pro M2",
                    Price = 2345,
                    Image = "/images/macbook.png",
                    BrandId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "XPS 15",
                    Price = 1999,
                    Image = "/images/xps.png",
                    BrandId = 2
                },
                new Product
                {
                    Id = 3,
                    Name = "Gram 17",
                    Price = 2024,
                    Image = "/images/gram.jpg",
                    BrandId = 3
                }
             );
        }
        private void SeedCandidate(ModelBuilder builder)
        {
            builder.Entity<Candidate>();
        }
        private void SeedJob(ModelBuilder builder)
        {
            builder.Entity<Job>();
        }
        private void SeedJobCandidate(ModelBuilder builder)
        {
            builder.Entity<JobCandidate>();
        }
        private void SeedRecuiter(ModelBuilder builder)
        {
            builder.Entity<Recuiter>();
        }
    }
}
