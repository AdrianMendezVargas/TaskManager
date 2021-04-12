using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Models.Data;
using TaskManager.Repository;
using TaskManager.Services;

namespace TaskManager.Api.Extensions {
    public static class ServiceExtensions {

        public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlite(configuration.GetConnectionString("SqliteConnection"), sqlOptions => {
                    sqlOptions.MigrationsAssembly("TaskManager.Api");
                });
            });
        }

        public static void AddUnitOfWork(this IServiceCollection services) {
            services.AddScoped<IUnitOfWork , EfUnitOfWork>();
        }

        public static void AddBussinessServices(this IServiceCollection services) {
            services.AddScoped<ITaskService , TaskService>();
        }


    }
}
