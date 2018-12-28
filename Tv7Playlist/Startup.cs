using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tv7Playlist.Core;
using Tv7Playlist.Core.Parsers;
using Tv7Playlist.Core.Parsers.M3u;
using Tv7Playlist.Core.Parsers.Xspf;
using Tv7Playlist.Data;

namespace Tv7Playlist
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ConfigureParser(services);
            ConfigureDatabase(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            LogConfiguration();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureParser(IServiceCollection services)
        {
            var type = Configuration.Get<AppConfig>().SourceType;
            switch (type)
            {
                case AppConfig.SourceTypeEnum.M3U:
                    services.AddTransient<IPlaylistParser, M3UParser>();
                    break;
                case AppConfig.SourceTypeEnum.XSPF:
                    services.AddTransient<IPlaylistParser, XspfParser>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static void ConfigureDatabase(IServiceCollection services)
        {
            //TODO: Move to settings to make it configurable within docker.
            var connection = "Data Source=playlist.db";
            services.AddDbContext<PlaylistContext>(options => options.UseSqlite(connection));
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
