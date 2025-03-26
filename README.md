# ReflectionTool

ReflectionTool 是一个强大的Unity编辑器扩展工具，用于处理反射和方法钩子（Method Hooking）相关的操作。它提供了一套完整的工具集，可以帮助开发者更便捷地处理Unity中的反射操作和方法拦截。

## 主要功能

### 1. 反射包装器生成 (Reflection Wrapper Generation)
- 自动生成类型的反射包装器
- 支持字段、属性、方法的访问
- 支持委托的访问
- 支持集合成员的便捷操作
- 支持Flags枚举类型的位运算
- 优化的性能实现，包含缓存机制

### 2. 方法钩子 (Method Hooking)
- 支持方法拦截和替换
- 支持构造函数钩子
- 支持属性访问器的钩子
- 提供优雅的钩子API设计

## 使用方法

### 配置包装器

Assets/Scripts/kuro/ReflectionTool/ReflectionTool.Settings.cs

```csharp
/// <summary>
/// 配置反射类型
/// </summary>
private static GenWrapInfo[] WrapInfoList => new GenWrapInfo[]
{
    new("UnityEngine.IMGUIModule", "UnityEngine.GUISkin"),
    new("UnityEditor.CoreModule", "UnityEditor.Editor"),
    new("UnityEditor.CoreModule", "UnityEditor.HandleUtility"),
    new("UnityEditor.CoreModule", "UnityEditor.EditorGUI"),
    new("UnityEditor.CoreModule", "UnityEditor.EditorGUIUtility"),
}
```

### 配置钩子

Assets/Scripts/kuro/ReflectionTool/ReflectionTool.Settings.cs

```csharp
private static GenHookInfo[] HookInfoList => new GenHookInfo[]
{
    new("UnityEngine.CoreModule", "UnityEngine.GameObject", "SetActive", null),
    new("UnityEngine.CoreModule", "UnityEngine.Transform", "set.position", null),
}
```

### 生成代码

![image](https://github.com/user-attachments/assets/4d50f7a8-9009-4703-9316-36226af86d60)

### 使用示例

请参考 Assets/Scripts/Sample.cs


## 支持平台

* Editor (Windows)

## 技术细节

### 反射优化
- 包装器使用结构体
- 使用委托缓存
- IL生成优化
- 参数数组池

### 钩子机制
- 支持方法前后拦截
- 参数修改支持
- 返回值控制
- 完整的错误处理

## reference
* https://github.com/Misaka-Mikoto-Tech/MonoHook
