
using System.Collections.Generic;

namespace UnityGameFramework
{
    public static class DotNetExtension
    {
        #region List

        public static T GetAndRemoveLast<T>(this List<T> _list)
        {
            if (_list.Count == 0)
                return default;
            
            T result = _list[^1];
            _list.RemoveAt(_list.Count - 1);
            return result;
        }
        public static bool RemoveLast<T>(this List<T> _list)
        {
            if (_list.Count == 0)
                return false;
            
            _list.RemoveAt(_list.Count - 1);
            return true;
        }

        #endregion
        
        #region Dictionary

        public static T_VALUE GetValueAndRemove<T_KEY, T_VALUE>(this Dictionary<T_KEY, T_VALUE> _dictionary, T_KEY _key)
        {
            T_VALUE value = _dictionary[_key];
            _dictionary.Remove(_key);
            return value;
        }
        
        #endregion
    }
}