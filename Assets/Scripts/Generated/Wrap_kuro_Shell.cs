// This file was automatically generated by kuroneko.
// ReSharper disable InconsistentNaming
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using kuro.Core;

namespace kuro.ReflectionTool
{
    public partial struct kuro_Shell
    {

        /// <summary>
        /// <see cref="kuro.Shell"/>
        /// </summary>
        public static Type __type__ { get; } = ReflectionUtils.GetTypeByFullName("Assembly-CSharp", "kuro.Shell");


        public int _value
        {
            get => __Get___value(__self__);
            set => __Set___value(__self__, value);
        }

        public kuro_Shell(object __self__) => this.__self__ = __self__ as object;
        public object __self__;
        public bool __valid__ => __self__ != null && __type__ != null;
        public kuro.Shell __super__ => (kuro.Shell)(__self__);
        public kuro.Shell __base__ => (kuro.Shell)(__self__);

        private static Func<object, int> ___Get___value;
        private static Func<object, int> __Get___value => ___Get___value ??= (__type__.QF("_value")).ILGet<int>();
        private static Action<object, int> ___Set___value;
        private static Action<object, int> __Set___value => ___Set___value ??= (__type__.QF("_value")).ILSet<int>();
    }
    public static class kuro_Shell_Extension
    {
        public static kuro_Shell ReflectionHelper(this kuro.Shell self) => new(self);
    }
}
