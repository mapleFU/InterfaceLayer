using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;
using org.apache.zookeeper;
using OSApiInterface.Services;
using StackExchange.Redis;

namespace OSApiInterface
{
    /// <summary>
    /// The entry point of the Application
    /// Which will do DI in this class.
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// Like adding db context for using EF, Using Mvc.
        /// Mostly Add.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<EntityCoreContext>()
                .BuildServiceProvider();
            
//        private ZooKeeper zk = new ZooKeeper("localhost:2181", 1500, null, true);


            services.AddSingleton<ZooKeeper>(
                new ZooKeeper("maplewish.cn:2181", 1500, null, true)
            );
            // TODO add http client
            services.AddHttpClient();
            
            // TODO : make clear this: http://zguide.zeromq.org/py%3aall#Getting-the-Message-Out
            var pubSocket = new PublisherSocket();
            pubSocket.Bind("tcp://*:11234");
            services.AddSingleton<PublisherSocket>(pubSocket);
            pubSocket.SendFrame("Hello");

            var pullSocket = new PullSocket();
            pullSocket.Bind("tcp://*:5558");
            services.AddSingleton<PullSocket>(pullSocket);

            // add interface for service
            // notice that something should be Scoped
            services.AddScoped<IZookeeperService, ZookeeperService>();
            services.AddScoped<IFileMetaService, FileMetaService>();
            services.AddScoped<IUserService, UserService>();
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowMyOrigin",
                    builder => builder.WithOrigins("http://localhost:8080"));
            });
            // Redis Service
            var conn = ConnectionMultiplexer.Connect("maplewish.cn:6700");
            services.AddSingleton<ConnectionMultiplexer>(conn);
            
            
            // https://stackoverflow.com/questions/52896068/reactasp-net-core-no-access-control-allow-origin-header-is-present-on-the-r
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// Config the routes and some other things.
        /// Use will add Middleware to the pipeline. (Mostly Use)
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
//            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");
            
            app.UseMvc();
            
//            app.UseCors("AllowMyOrigin");
            

        }
    }
}