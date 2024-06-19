using ThePattern.Common.Injection;

namespace ThePattern.Common.Injection
{
    public class PatternInjection
    {
        private static IConfigInjection _config;

        public static IConfigInjection Configuration
        {
            get
            {
                if (_config == null)
                    _config = ServiceProvider.CreateInjection<IConfigInjection>();
                return _config;
            }
        }
    }
}
