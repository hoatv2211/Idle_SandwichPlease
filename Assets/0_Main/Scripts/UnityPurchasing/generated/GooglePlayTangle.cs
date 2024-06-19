// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("hNRWTkq7ufbWy51MDVI0VKfY3sSXjMQfNd3GMGvXFLh6q8Ym0D1dlqYUl7Smm5CfvBDeEGGbl5eXk5aV7dyyuDKHWKCMr9F6UAM3oTgHaQ0CnY/z36pMl3uWnmLON6oCm1SUbN4W6AlT6uRF7fbewF+s6BgLA6xgyOreoXE+h9u+qHMVDihNsUezmXsUl5mWphSXnJQUl5eWBCLj3y0sAqw9CY+75LAXGjo8dlvcaHIegv+nVEK7CvUQDpKSaZ8v3P+ecm44ISP6hdLDNMCIMeQBFvEblk69Ia55BxoEB7HvObLGmAdarTlNt+XoQKes28KFLGC52lQGbMpa3pSf7yLyHzPuBfoCGfXh4FmDy3bZpo1RI9lhJU4uXHA1TmyM55SVl5aX");
        private static int[] order = new int[] { 3,8,3,5,13,6,6,8,12,10,12,13,13,13,14 };
        private static int key = 150;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
