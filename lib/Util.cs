using System;

namespace CSharpLens {
    public static class Util {
        public static object DefaultOf(Type type) {
            if (type.IsValueType) {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}