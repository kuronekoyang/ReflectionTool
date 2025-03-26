using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kuro.ReflectionTool;

namespace kuro
{
    public class Shell
    {
        private int _value;

        class BaseClass
        {
            private string _name;
        }

        class SampleClass : BaseClass
        {
            [Flags]
            private enum SampleEnum
            {
                None = 0,
                A = 1 << 0,
                B = 1 << 1,
                C = 1 << 2,
            }

            private static SampleClass s_instance { get; set; }
            private int _value;
            private SampleEnum _enum;
            private List<SampleClass> _list = new();

            private delegate int SampleDelegate(int value);

            private SampleDelegate OnSampleEvent;

            private int Value
            {
                get => _value;
                set => _value = value;
            }

            private int InvokeDelegate(int value) => OnSampleEvent?.Invoke(value) ?? 0;

            public SampleClass()
            {
            }

            public SampleClass(int value) => _value = value;
        }
    }

    public class Sample : MonoBehaviour
    {
        void Start()
        {
            WrapSample();
            HookSample();
        }

        void WrapSample()
        {
            // 1. 创建实例
            var ins = kuro_Shell_SampleClass.__new__();

            // 2. 访问私有字段
            ins._value = 10;

            // 3. 访问私有属性
            kuro_Shell_SampleClass.s_instance = ins;

            // 4. 获取包装器的实例 __self__
            ins._list.Add(kuro_Shell_SampleClass.__new__().__self__);

            // 5. 访问集合成员
            var child0 = ins._list__GetItem(0);

            // 6. 将object转为包装器
            var child0v2 = new kuro_Shell_SampleClass(ins._list[0]);

            // 7. 访问委托
            child0.OnSampleEvent += (value) => value + 1;

            // 8. 访问枚举
            child0._enum |= kuro_Shell_SampleClass_SampleEnum.A;
            child0._enum >>= 1;

            // 9. 访问父类
            var super = ins.__super__;
            super._name = "test";

            // 10. 公有类型，可以通过 ReflectionHelper 快速转为包装器
            var shell = new Shell();
            var shellHelper = shell.ReflectionHelper();
            shellHelper._value = 10;
        }

        void HookSample()
        {
            {
                var oldIns = kuro_Shell_SampleClass.__new__(4);
                // 注册方法前钩子，覆盖原有构造函数
                kuro_Shell_SampleClass.__hook__._ctor.OnBegin = (ref kuro_Shell_SampleClass.__hook__._ctor.Param param) =>
                {
                    param.__this__._value = param.value + 1;
                    return true;
                };
                var newIns = kuro_Shell_SampleClass.__new__(4);

                Debug.Log($"HookSample 1: oldIns._value:{oldIns._value}, newIns._value:{newIns._value}");
            }

            {
                var oldIns = kuro_Shell_SampleClass.__new__();
                oldIns.Value = 4;

                // 注册方法前钩子，调用原始函数
                kuro_Shell_SampleClass.__hook__.set_Value.OnBegin = (ref kuro_Shell_SampleClass.__hook__.set_Value.Param param) =>
                {
                    param.__original__(param.value + 1);
                    return true;
                };

                var newIns = kuro_Shell_SampleClass.__new__();
                newIns.Value = 4;

                Debug.Log($"HookSample 2: oldIns.Value:{oldIns.Value}, newIns.Value:{newIns.Value}");
            }

            {
                var oldIns = kuro_Shell_SampleClass.__new__();
                oldIns.Value = 4;

                // 注册方法前钩子，修改参数
                kuro_Shell_SampleClass.__hook__.set_Value.OnBegin = (ref kuro_Shell_SampleClass.__hook__.set_Value.Param param) =>
                {
                    param.value += 1;
                    return false;
                };

                var newIns = kuro_Shell_SampleClass.__new__();
                newIns.Value = 4;

                Debug.Log($"HookSample 3: oldIns.Value:{oldIns.Value}, newIns.Value:{newIns.Value}");
            }

            {
                var oldIns = kuro_Shell_SampleClass.__new__();
                oldIns.OnSampleEvent = x=> x;
                var oldInvokeResult = oldIns.InvokeDelegate(4);

                // 注册方法后钩子，修改结果
                kuro_Shell_SampleClass.__hook__.InvokeDelegate.OnEnd = (ref kuro_Shell_SampleClass.__hook__.InvokeDelegate.Param param) =>
                {
                    param.__result__ += 1;
                };

                var newIns = kuro_Shell_SampleClass.__new__();
                newIns.OnSampleEvent = x=> x;
                var newInvokeResult = newIns.InvokeDelegate(4);

                Debug.Log($"HookSample 4: oldInvokeResult:{oldInvokeResult}, newInvokeResult:{newInvokeResult}");
            }
        }

    }
}