using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TaskManager.Models.Data;
using TaskManager.Repository;
using TaskManager.Services;
using TaskManager.Shared.Services;

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
            services.AddScoped<IUserService , UserService>();
        }

        public static void AddMailServices(this IServiceCollection services, IConfiguration Configuration) {
            services.AddScoped(s => new SmtpClient {
                Host = Configuration["SmtpHost"] ,
                Port = Convert.ToInt32(Configuration["MailPort"]) ,
                EnableSsl = true ,
                Credentials = new NetworkCredential(Configuration["MailSender"] , Configuration["MailSenderPassword"])
            });
            services.AddScoped<IMailService , MailService>();
        }


    }
}
