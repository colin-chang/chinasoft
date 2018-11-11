using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;

namespace Com.ChinaSoft.Utility
{
    public static class EnumHelper
    {
        /// <summary>
        /// 获取Enum类型值的自定义Attribute
        /// </summary>
        /// <typeparam name="T">要返回的实际枚举类型</typeparam>
        /// <param name="value">枚举值</param>
        /// <returns>返回<see cref="T"/>类型的实例</returns>
        public static T GetEnumAttr<T>(Enum value) where T : Attribute
        {
            ObjectCache cache = MemoryCache.Default;

            Type type = value.GetType();
            string cacheKey = $"{type.FullName}_{value.ToString()}";

            T attr = (T)cache.Get(cacheKey);
            if (attr == null)
            {
                MemberInfo member = (MemberInfo)type.GetMember(value.ToString()).FirstOrDefault();

                attr = member.GetCustomAttribute<T>();

                CacheItemPolicy policy = new CacheItemPolicy();

                cache.Set(cacheKey, attr, policy);
            }

            return attr;
        }
    }
}
