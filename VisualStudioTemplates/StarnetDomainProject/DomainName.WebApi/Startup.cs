using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Funq;
using ServiceStack;
using ServiceStack.Configuration;
using $safeprojectname$.ServiceInterface;
using $ext_projectname$.ReadModel;
using $ext_projectname$.ReadModel.Queries;
using $safeprojectname$.Infrastructure;
using ServiceStack.Validation;

namespace $safeprojectname$
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceStack(new AppHost(Configuration)
            {
                AppSettings = new NetCoreAppSettings(Configuration)
            });
        }
    }

    public class AppHost : AppHostBase
    {
        public IConfiguration Configuration { get; }

        public AppHost(IConfiguration configuration) : base("$safeprojectname$", typeof(MyServices).Assembly) {

            Configuration = configuration;
        }

        // Configure your AppHost with the necessary configuration and dependencies your App needs
        public override void Configure(Container container)
        {
            ServiceStack.Text.JsConfig.TreatEnumAsInteger = true;
            ServiceStack.Text.JsConfig.AssumeUtc = true;
            ServiceStack.Text.JsConfig.AlwaysUseUtc = true;
            ServiceStack.Text.JsConfig.DateHandler = ServiceStack.Text.DateHandler.ISO8601;

            SetConfig(new HostConfig
            {
                DefaultRedirectPath = "/metadata",
                DebugMode = AppSettings.Get(nameof(HostConfig.DebugMode), false)
            });
            var ravenConfig = new RavenConfig { Urls = Configuration["RavenDb:Urls"].Split(';'), CertificateFilePassword = Configuration["RavenDb:CertificatePassword"], CertificateFilePath = Configuration["RavenDb:CertificatePath"], DatabaseName = Configuration["RavenDb:DatabaseName"] };
            var docStore = new RavenDocumentStoreFactory().CreateDocumentStore(ravenConfig);
            container.Register(docStore);
            container.RegisterAutoWiredAs<CompanySmartSearchQuery, ICompanySmartSearchQuery>();
            container.RegisterAutoWiredAs<TimeProvider, ITimeProvider>();
            container.RegisterAutoWiredAs<NSBus, IMessageBus>();

            Plugins.Add(new ValidationFeature());
        }
    }
}
