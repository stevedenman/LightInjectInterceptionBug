using LightInject;
using LightInject.Interception;
using LightInject.ServiceLocation;
using Microsoft.Practices.ServiceLocation;
using System;

namespace LightInjectInterceptionBug
{
    class Program
    {
        static void Main()
        {
            var container = new ServiceContainer();

            container.Register<IProductRepository, ProductRepository>();

            container.Intercept(ss => ss.ServiceType == typeof(IProductRepository), sf => new RepositoryInterceptor());

            var serviceLocator = new LightInjectServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            // test
            var productRepository = ServiceLocator.Current.GetInstance<IProductRepository>();
            productRepository.GetFieldValue<string>(new Product {Name = "foo"}, "Name");
        
        }
    }

    public class RepositoryInterceptor : IInterceptor
    {
        public object Invoke(IInvocationInfo invocationInfo)
        {
            Console.WriteLine("Interceptor invoked.");
            var returnValue = invocationInfo.Proceed();
            return returnValue;
        }
    }

    public interface IGenericRepository<T> where T : BaseEntity
    {
        TA GetFieldValue<TA>(T entity, string fieldName);
    }

    public class GenericRepository<T> where T : BaseEntity
    {
        public TA GetFieldValue<TA>(T entity, string fieldName)
        {
            Console.WriteLine("Called GetFieldValue().");
            return default(TA);
        }
    }

    public interface IProductRepository : IGenericRepository<Product> { }
    public class ProductRepository : GenericRepository<Product>, IProductRepository { }
    public class BaseEntity { }
    public class Product : BaseEntity 
    {
        public string Name { get; set; }
    }
}
