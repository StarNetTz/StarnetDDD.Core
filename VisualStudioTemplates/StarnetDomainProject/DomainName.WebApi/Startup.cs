using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Funq;
using ServiceStack;
using $safeprojectname$.ServiceInterface;
using $ext_projectname$.ReadModel;
using $ext_projectname$.ReadModel.Queries;
using $safeprojectname$.Infrastructure;
using ServiceStack.Validation;
using ServiceStack.Auth;
using ServiceStack.Caching;

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

        public AppHost(IConfiguration configuration) : base("Betting.WebApi", typeof(MyServices).Assembly)
        {

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

            container.Adapter = new SimpleInjectorIocAdapter(SetupSimpleInjectorContainer());


            Plugins.Add(new ValidationFeature());
            Plugins.Add(new CorsFeature(allowCredentials: true, allowedHeaders: "Content-Type, Authorization", allowOriginWhitelist: GetOriginWhiteList()));
            Plugins.Add(new AuthFeature(() => new AuthUserSession(),
                new IAuthProvider[] {
                new JwtAuthProvider(AppSettings) {
                    AuthKeyBase64 = Configuration["Jwt:Key"],
                    RequireSecureConnection = false, //dev configuration
                    EncryptPayload = false, //dev configuration
                    HashAlgorithm = "HS256"
                }
            }));

        }

        SimpleInjector.Container SetupSimpleInjectorContainer()
        {
            var simpleContainer = new SimpleInjector.Container();
            simpleContainer.Register(() => CreateRavenDbDocumentStore(), SimpleInjector.Lifestyle.Singleton);
            simpleContainer.Register<ICompanySmartSearchQuery, CompanySmartSearchQuery>();
            simpleContainer.Register<ITimeProvider, TimeProvider>();
            simpleContainer.Register<IMessageBus, NSBus>();
            simpleContainer.Register<ICacheClient>(() => new MemoryCacheClient());
            simpleContainer.Register(typeof(IQueryById<>), typeof(QueryById<>));
            return simpleContainer;
        }

        Raven.Client.Documents.IDocumentStore CreateRavenDbDocumentStore()
        {
            var ravenConfig = new RavenConfig { Urls = Configuration["RavenDb:Urls"].Split(';'), CertificateFilePassword = Configuration["RavenDb:CertificatePassword"], CertificateFilePath = Configuration["RavenDb:CertificatePath"], DatabaseName = Configuration["RavenDb:DatabaseName"] };
            return new RavenDocumentStoreFactory().CreateDocumentStore(ravenConfig);
        }

        string[] GetOriginWhiteList()
        {
            return Configuration["CORS:Whitelist"].Split(';');
        }
    }
}
