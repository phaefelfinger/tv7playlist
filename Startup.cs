using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace tv7playlist
{
    public class Startup
    {
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            LogConfiguration();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseMvc();
        }

        private void LogConfiguration()
        {
            var appConfig = Configuration.Get<AppConfig>();
            _logger.LogInformation(LoggingEvents.Startup, "Using TV7 URL: {TV7Url}", appConfig.TV7Url);
            _logger.LogInformation(LoggingEvents.Startup, "Using Udpxy URL: {UdpxyUrl}", appConfig.UdpxyUrl);
            _logger.LogInformation(LoggingEvents.Startup, "Using DownloadFileName: {DownloadFileName}",
                appConfig.DownloadFileName);
        }
    }
}