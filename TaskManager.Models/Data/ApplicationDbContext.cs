﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.Models.Domain;

namespace TaskManager.Models.Data {
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> { //1. Using IdentityDbContext and specifying the user class
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) {
        
        }

        public DbSet<UserTask> Tasks { get; set; }

    }
}
