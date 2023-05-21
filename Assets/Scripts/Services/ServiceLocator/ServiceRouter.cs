namespace Services.ServiceLocator
{
    public class ServiceRouter
    {
        private static ServiceRouter _instance;
        public static ServiceRouter Container => 
            _instance ??= new ServiceRouter();

        public void RegisterSingle<TService>(TService implementation) where TService : IService => 
            Implementation<TService>.ServiceInstance = implementation;

        public TService Single<TService>() where TService : IService => 
            Implementation<TService>.ServiceInstance;

        private static class Implementation<TService> where TService : IService
        {
            public static TService ServiceInstance;
        }
    }
}