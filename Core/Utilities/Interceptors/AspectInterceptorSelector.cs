using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Core.Aspects.Autofac.Exception;
using Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;

namespace Core.Utilities.Interceptors
{
    public class AspectInterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var classAttributes = type.GetCustomAttributes<MethodInterceptionBaseAttribute>(true).ToList();
            var methodAttributes = type.GetMethod(method.Name).GetCustomAttributes<MethodInterceptionBaseAttribute>(true);
            classAttributes.AddRange(methodAttributes);
            classAttributes.Add(new ExceptionLogAspect(typeof(FileLogger)));
            // Bu kod managerdaki validationları yani işlemlerden önce gelicek olan
            // kontrol satırını tüm işlemlerin başına ekleme işlemi
            // Örn. ProductManager.cs içersindeki GetListByCategory ve diğer işlemlerin
            // hepsine FileLogger LogAspect Uygulamak istiyoruz
            // Burada classAttributes vererek tüm işlemlere eklemiş oluyoruz
            // [LogAspect(typeof(DatabaseLogger))] -> bunu her işlemden önce yazmak yerine yani hepsine ayrı ayrı yazmak yerine yukarıdaki işlemi uyguluyoruz
            // public IDataResult<List<Product>> GetListByCategory(int categoryId)
            // birden fazla classAttributes ekliyebiliriz FileLogger ve DatabaseLogger i aynı anda tüm işlemler başlamadan önce kontrol etsin
            // classAttributes.Add(new ExceptionLogAspect(typeof(DatabaseLogger)));

            return classAttributes.OrderBy(x => x.Priority).ToArray();
        }
    }
}
