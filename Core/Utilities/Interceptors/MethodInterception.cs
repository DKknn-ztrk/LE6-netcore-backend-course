using Castle.DynamicProxy;

namespace Core.Utilities.Interceptors
{
    public abstract class MethodInterception:MethodInterceptionBaseAttribute
    {
        protected virtual void OnBefore(IInvocation ınvocation) { } //OnBefore - Metodun önünde yani metod Çalışmadan Önce sen çalış
        protected virtual void OnAfter(IInvocation ınvocation) { } //OnAfter - Metodun sonunda yani metod çalıştıktan sonra sen çalış
        protected virtual void OnException(IInvocation ınvocation) { } //OnException - Metod hata verdiğinde sen çalış
        protected virtual void OnSuccess(IInvocation ınvocation) { } //OnSuccess - Metod başarılı ise sen çalış

        public override void Intercept(IInvocation invocation)
        {
            var isSuccess = true;
            OnBefore(invocation);
            try
            {
                invocation.Proceed();
            }
            catch (System.Exception)
            {
                isSuccess = false;
                OnException(invocation);
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
