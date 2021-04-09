using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Models.Data;

namespace TaskManager.Api.Extensions {
    public static class ServiceExtensions {

        public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlite(configuration.GetConnectionString("SqliteConnection"), sqlOption => {
                    sqlOption.MigrationsAssembly("TaskManager.Api");
                });
            });
        }

    }
}
