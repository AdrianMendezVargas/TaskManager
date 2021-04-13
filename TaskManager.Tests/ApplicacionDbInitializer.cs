﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Data;

namespace TaskManager.Tests {
    public static class ApplicacionDbInitializer {
        public static ApplicationDbContext GetDbContext() {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            return new ApplicationDbContext(options);
        }
    }
}
