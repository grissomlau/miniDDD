using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using DDD.Simple.IServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

using Autofac.Builder;
using Autofac.Core;
using Autofac.Util;
using Autofac.Extensions.DependencyInjection;
//using DDD.Simple.Repository.EF;
using Microsoft.Extensions.Configuration.Json;
using MiniDDD.UnitOfWork;
//using MiniDDD.UnitOfWork.EF;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ApiGateway
{
    public class Startup
    {
        public IContainer ApplicationContainer { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc();

            // Add controllers as services so they'll be resolved.
            services.AddMvc().AddControllersAsServices();
            services.AddMvcCore()
    .AddApiExplorer();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            //});

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            }); ;

            var builder = new ContainerBuilder();

            // When you do service population, it will include your controller
            // types automatically.
            builder.Populate(services);

            var baseType = typeof(IAutoInject);
            var assembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "/Simple.Services.dll");
            //var assembly2 = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "/Simple.Repository.EF.dll");
            var assembly2 = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "/Simple.Repository.Dapper.dll");
            //var assembly2 = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "/Simple.Repository.SqlSugar.dll");

            builder.RegisterAssemblyTypes(assembly)
                            .Where(t => baseType.IsAssignableFrom(t) && t != baseType)
                            .AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterAssemblyModules(assembly);

            var repoType = typeof(MiniDDD.IRepository<,>);
            builder.RegisterAssemblyTypes(assembly2)
                     .Where(t => repoType.IsAssignableFrom(t) && t != repoType)
                     .AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterAssemblyModules(assembly2);

            // using dapper

            builder.RegisterType<MiniDDD.UnitOfWork.Dapper.UnitOfWork>().As<IUnitOfWork>().AsImplementedInterfaces().InstancePerLifetimeScope();


            // using ef

            //builder.RegisterType<MiniDDD.UnitOfWork.EF.UnitOfWork>().As<IUnitOfWork>().AsImplementedInterfaces().InstancePerLifetimeScope();
            //builder.RegisterType<MiniDDD.UnitOfWork.EF.DefaultDbContext>()
            //    .WithParameter("tableModelAssemblyName", null)
            //    .WithParameter("logAction", new Action<LogLevel, string>((logLevel, log) =>
            //    {
            //        Debug.WriteLine($"{logLevel} - {log}");
            //    }))
            //    .WithParameter("logLevel", LogLevel.Debug)
            //    .SingleInstance();

            // using sqlsugar
            /*builder.RegisterType<DbContextOptions>().SingleInstance();

            builder.RegisterType<MiniDDD.UnitOfWork.SqlSugar.UnitOfWork>().As<IUnitOfWork>()
                .WithParameter("logAction", new Action<string>((sql) =>
                {
                    Debug.WriteLine("Sugar: " + sql);
                }))
                .AsImplementedInterfaces().InstancePerLifetimeScope();
                */



            //       Configuration
            //.Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            //.Build();
            //Log4netOptions log4NetOptions = Configuration.GetSection("Log4netOptions").Get<Log4netOptions>();
            //DbOptions dbOptions = Configuration.GetSection("DbOptions").Get<DbOptions>();

            DbContextOptions dbContextOption = Configuration.GetSection("DbContextOptions").Get<DbContextOptions>();
            builder.Register(x => dbContextOption).SingleInstance();

            // If you want to set up a controller for, say, property injection
            // you can override the controller registration after populating services.
            //builder.RegisterType<MyController>().PropertiesAutowired();

            this.ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
