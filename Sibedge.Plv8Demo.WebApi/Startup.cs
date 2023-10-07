namespace Sibedge.Plv8Demo.WebApi
{
    using System.Data;
    using Filters;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Npgsql;
    using Plv8Server;
    using Services;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<IDbConnection>(s => new NpgsqlConnection(
                this.Configuration.GetConnectionString("Default")));

            services.AddTransient<GraphQlService>();
            services.AddTransient<V8FuncService>();
            services.AddTransient<ChangeService>();

            services.Configure<Plv8Settings>(options => this.Configuration.GetSection(nameof(Plv8Settings)).Bind(options));

            services.AddMemoryCache();

            services.AddMvc(options =>
                {
                    options.Filters.Add(new WebApiExceptionFilterAttribute());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
