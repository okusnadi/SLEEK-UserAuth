[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WebHost.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(WebHost.App_Start.NinjectWebCommon), "Stop")]

namespace WebHost.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using BrockAllen.MembershipReboot;
    using BrockAllen.MembershipReboot.WebHost;
    using WebHost.MR;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var config = WebHost.MR.CustomConfig.Config; //.Create();
            kernel.Bind<MembershipRebootConfiguration<CustomUser>>().ToConstant(config);
            //kernel.Bind<WebHost.MR.CustomConfig>().ToConstant(config);
            kernel.Bind<UserAccountService>().ToSelf();
            //kernel.Bind<AuthenticationService>().To<SamAuthenticationService>();
            //kernel.Bind<IUserAccountQuery>().To<BrockAllen.MembershipReboot.Ef.DefaultUserAccountRepository>().InRequestScope();
            //kernel.Bind<IUserAccountRepository>().To<BrockAllen.MembershipReboot.Ef.DefaultUserAccountRepository>().InRequestScope();
            kernel.Bind<IUserAccountRepository<CustomUser>>().To<CustomRepository>().InRequestScope();
            kernel.Bind<CustomDatabase>().ToSelf();
            kernel.Bind<IUserAccountQuery>().To<CustomRepository>();
            kernel.Bind<AuthenticationService<CustomUser>>().To<SamAuthenticationService<CustomUser>>();
        }        
    }
}
