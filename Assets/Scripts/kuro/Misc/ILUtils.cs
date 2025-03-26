using System;
using System.Reflection;
using System.Reflection.Emit;

namespace kuro.Core
{
    public static class ILUtils
    {
        /// <summary>
        /// IL.Emit动态创建获取字段值的方法
        /// </summary>
        public static Func<object, T> ILGet<T>(this FieldInfo field)
        {
            if (field == null)
                return null;
            var type = field.DeclaringType;
            if (type == null)
                return null;
            DynamicMethod method = new DynamicMethod(string.Empty, typeof(T), new Type[] { typeof(object) }, type);
            ILGenerator il = method.GetILGenerator();
            if (!field.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                if (type.IsValueType) il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Ldfld, field);
            }
            else
            {
                il.Emit(OpCodes.Ldsfld, field);
            }

            il.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Func<object, T>)) as Func<object, T>;
        }

        /// <summary>
        /// IL.Emit动态创建设置字段方法
        /// </summary>
        public static Action<object, T> ILSet<T>(this FieldInfo field)
        {
            if (field == null)
                return null;
            var type = field.DeclaringType;
            if (type == null)
                return null;
            DynamicMethod method = new DynamicMethod(string.Empty, null, new Type[] { typeof(object), typeof(T) }, type);
            ILGenerator il = method.GetILGenerator();
            if (!field.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(type.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, type);
            }

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field);
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Action<object, T>)) as Action<object, T>;
        }

        /// <summary>
        /// IL.Emit动态创建获取属性值的方法
        /// </summary>
        public static Func<object, T> ILGet<T>(this PropertyInfo property)
        {
            if (!property.CanRead)
                return null;
            var type = property.DeclaringType;
            if (type == null)
                return null;

            var get = property.GetMethod;
            DynamicMethod method = new DynamicMethod(string.Empty, typeof(T), new Type[] { typeof(object) }, type);
            ILGenerator il = method.GetILGenerator();
            if (!get.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(type.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, type);
            }

            il.Emit(get.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, get);
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Func<object, T>)) as Func<object, T>;
        }

        /// <summary>
        /// IL.Emit动态创建设置属性值的方法
        /// </summary>
        public static Action<object, T> ILSet<T>(this PropertyInfo property)
        {
            if (!property.CanWrite)
                return null;
            var type = property.DeclaringType;
            if (type == null)
                return null;

            var set = property.SetMethod;
            DynamicMethod method = new DynamicMethod(string.Empty, null, new Type[] { typeof(object), typeof(T) }, type);
            ILGenerator il = method.GetILGenerator();
            if (!set.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(type.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, type);
            }


            il.Emit(OpCodes.Ldarg_1);
            il.Emit(set.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, set);
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Action<object, T>)) as Action<object, T>;
        }
    }
}