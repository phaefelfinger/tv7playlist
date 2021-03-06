﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            var appConfig = ConfigureAppConfig(services);

            LogConfiguration(appConfig);
            ConfigureParser(services, appConfig);
            ConfigureDatabase(services, appConfig);

            services.AddMvc().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            InitializeDatabase(app);


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PlaylistContext>().Database.Migrate();
            }
        }

        private void ConfigureParser(IServiceCollection services, IAppConfig appConfig)
        {
            services.AddTransient<IPlaylistLoader, PlaylistLoader>();
            services.AddTransient<IPlaylistBuilder, PlaylistBuilder>();
            services.AddTransient<IPlaylistSynchronizer, PlaylistSynchronizer>();

            var type = appConfig.SourceType;
            switch (type)
            {
                case SourceTypeEnum.M3U:
                    services.AddTransient<IPlaylistParser, M3UParser>();
                    break;
                case SourceTypeEnum.Xspf:
                    services.AddTransient<IPlaylistParser, XspfParser>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IAppConfig ConfigureAppConfig(IServiceCollection services)
        {
            var appConfig = Configuration.Get<AppConfig>();
            services.AddSingleton<IAppConfig>(appConfig);
            return appConfig;
        }

        private static void ConfigureDatabase(IServiceCollection services, IAppConfig appConfig)
        {
            services.AddDbContext<PlaylistContext>(options => options.UseSqlite(appConfig.SqLiteConnectionString));
        }

        private void LogConfiguration(IAppConfig appConfig)
        {
            _logger.LogInformation(LoggingEvents.Startup, "Using TV7 URL: {TV7Url}", appConfig.TV7Url);
            _logger.LogInformation(LoggingEvents.Startup, "Using Udpxy URL: {UdpxyUrl}", appConfig.UdpxyUrl);
            _logger.LogInformation(LoggingEvents.Startup, "Using DownloadFileName: {DownloadFileName}",
                appConfig.DownloadFileName);
        }
    }
}