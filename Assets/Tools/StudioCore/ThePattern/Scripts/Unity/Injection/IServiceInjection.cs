using ThePattern.Common.Injection;

namespace ThePattern.Unity.Injection
{
    public interface IServiceInjection : IInjection
    {
        void OnServiceInit();
    }
}