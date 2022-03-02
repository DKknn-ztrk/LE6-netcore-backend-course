using Castle.DynamicProxy;
using System;

namespace Core.Utilities.Interceptors
{
    public abstract class MethodInterception:MethodInterceptionBaseAttribute
    {
        protected virtual void OnBefore(IInvocation invocation) { } //OnBefore - Metodun önünde yani metod Çalışmadan Önce sen çalış
        protected virtual void OnAfter(IInvocation invocation) { } //OnAfter - Metodun sonunda yani metod çalıştıktan sonra sen çalış
        protected virtual void OnException(IInvocation invocation, System.Exception e) { } //OnException - Metod hata verdiğinde sen çalış
        protected virtual void OnSuccess(IInvocation invocation) { } //OnSuccess - Metod başarılı ise sen çalış

        public override void Intercept(IInvocation invocation)
        {
            var isSuccess = true;
            OnBefore(invocation);
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                isSuccess = false;
                OnException(invocation,e);
                throw;
            }
            finally
            {
                if (isSuccess)
                {
                    OnSuccess(invocation);
                }
            }
            OnAfter(invocation);
        }
    }
}
