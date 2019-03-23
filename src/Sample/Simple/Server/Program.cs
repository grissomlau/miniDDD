using System;
using Autofac;
using Jimu;
using Jimu.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Jimu.Common.Logger;
using Jimu.Server.OAuth;
using MiniDDD.UnitOfWork;

namespace DDD.Simple.Server
{
    //    public class BloggingContext : DbContext
    //    {
    //        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options)
    //{

    //        }
    //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //        {
    //            optionsBuilder.UseMySql("Data Source=blog.db");
    //        }
    //    }

    class Program
    {

        static IConfiguration Configuration { get; set; }
        static void Main(string[] args)
        {
            //var op = new DbContextOptions<BloggingContext>();

            //using (var context = new BloggingContext(op))
            //{
            //    var name = context.Database.ProviderName;
            //    // do stuff
            //}
            Configuration = new ConfigurationBuilder()
                 .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
                 .Build();
            //Log4netOptions log4NetOptions = Configuration.GetSection("Log4netOptions").Get<Log4netOptions>();
            //DbOptions dbOptions = Configuration.GetSection("DbOptions").Get<DbOptions>();

            DbContextOptions dbContextOption = Configuration.GetSection("DbContextOption").Get<DbContextOptions>();


            var containerBuilder = new ContainerBuilder();
            var builder = new ServiceHostServerBuilder(containerBuilder)
                .UseLog4netLogger()
                .RegisterService(container =>
                {
                    // inject dboptions
                    //container.Register(x => dbOptions);
                    container.Register(x => dbContextOption);
                    container.Register(x => dbContextOption).SingleInstance();
                })
                //.LoadServices(new string[] { "DDD.Simple.IServices", "DDD.Simple.Services" }, "DDD.Simple.IServices.*.*Service|DDD.Simple.Services.*.*Service")
                //.LoadServices(new[] { "Simple.IServices", "Simple.Services" })
                .LoadServices(new[] { "Simple.IServices", "Simple.Services", "Simple.Repository.SqlSugar", "MiniDDD.UnitOfWork.SqlSugar" })
                 //.LoadServices(new string[] { "DDD.Simple.IServices", "DDD.Simple.Services", "DDD.Simple.Repository.EF", "DbWorker.UnitOfWork.EF" })
                 //.UseDotNettyServer("127.0.0.1", 8009, server => { })
                 .UseDotNettyForTransfer("127.0.0.1", 8009)
                //.UseNetCoreHttpServer("127.0.0.1", 8009, server => { })
                .UseConsulForDiscovery("127.0.0.1", 8500, "JimuService", "127.0.0.1:8009")
                 .UseJoseJwtForOAuth<DotNettyAddress>(new JwtAuthorizationOptions
                 {
                     CheckCredential = context =>
                      {
                          if (context.UserName == "admin" && context.Password == "admin")
                          {
                              context.AddClaim("username", "DbWorker");
                              context.AddClaim("dep", "IT");
                              context.AddClaim("role", "admin");
                          }
                          else
                          {
                              context.Rejected("username or password are incorrect.", "用户名或密码错误");
                          }
                      },
                     ValidateLifetime = true,
                     ExpireTimeSpan = TimeSpan.FromDays(30),
                     SecretKey = "12345678901234567890123456789012",
                     ServerIp = "127.0.0.1",
                     ServerPort = 8009,
                     TokenEndpointPath = "oauth/token?username=&password="
                 })
                ;
            using (var host = builder.Build())
            {
                // 要使用 migration 必须要调用对象
                // EF 专享
                //var context = host.Container.Resolve<DefaultDbContext>();
                //var name = context.Database.ProviderName;
                Console.WriteLine("Server start successful.");
                host.Run();
                Console.ReadLine();
            }
        }
    }
}
