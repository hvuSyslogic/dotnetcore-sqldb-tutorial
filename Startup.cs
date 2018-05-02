using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Models;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;

namespace DotNetCoreSqlDb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // add ADAL
            //services.AddAuthentication( sharedOptions => { sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
            //    .AddAzureAdBearer(options => Configuration.Bind("AzureAd", options));
            // Add framework services.
            services.AddMvc();
            services.AddSwaggerGen(c => 
                {
                    c.SwaggerDoc("v1", new Info { Title = "DotNetCoreSqlDbAPI}", Version = "v1",
                        Description = "A simple example ASP.NET Core Web API",
                        TermsOfService = "None",
                        Contact = new Contact
                        {
                            Name = "Hung Vu",
                            Email = string.Empty,
                            Url = "https://twitter.com/hqvu81"
                        },
                        License = new License
                        {
                            Name = "Use under LICX",
                            Url = "https://www.linkedin.com/profileofhungvu"
                        }
                    });

                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });

            // Use SQL Database if in Azure, otherwise, use SQLite
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                services.AddDbContext<MyDatabaseContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("MyDbConnection")));
            else
                services.AddDbContext<MyDatabaseContext>(options =>
                        options.UseSqlite("Data Source=localdatabase.db"));

            // Automatically perform database migration
            services.BuildServiceProvider().GetService<MyDatabaseContext>().Database.Migrate();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotNetCoreSqlDbAPI V1");
            });

            app.UseMvc(routes =>
            {
                    routes.MapRoute(
                    name: "default",
                    template: "{controller=Todos}/{action=Index}/{id?}");
                      routes.MapRoute("DefaultApi", "api/{controller}");

            });
        }
    }
}
