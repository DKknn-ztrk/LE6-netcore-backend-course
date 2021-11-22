using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Caching;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Aspects.Autofac.Caching
{
    public class CacheAspect:MethodInterception
    {
        private int _duration;
        private ICacheManager _cacheManager;

        public CacheAspect(int duration=60)
        {
            _duration = duration;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }

        //ProductManager.GetByCategory(1,ffdsfs)
        public override void Intercept(IInvocation invocation)
        {
            var methodName = string.Format($"{invocation.Method.ReflectedType.FullName}.{invocation.Method.Name}");
            var arguments = invocation.Arguments.ToList();
            var key = $"{methodName}({string.Join(",",arguments.Select(x=>x?.ToString()??"<Null>"))})";
            if (_cacheManager.IsAdd(key)) // cache de var mı -> cache aynı mı
            {
                invocation.ReturnValue = _cacheManager.Get(key); // cache aynı ise burası çalışıyor yani olan cache keyini dön
                return;
            }
            invocation.Proceed(); // cache de yoksa burası -> Cache de eklenme vs olduysa buraya düşer yeni cache keyi gönder durationla birlikte...! çalıştır
            _cacheManager.Add(key, invocation.ReturnValue, _duration);
        }
    }
}
