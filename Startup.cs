using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using org.apache.zookeeper;

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
                new ZooKeeper("localhost:2181", 1500, null, true)
            );
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
            else
            {
                app.UseHsts();
            }
            
//            app.UseHttpsRedirection();
            app.UseMvc();
           
        }
    }
}