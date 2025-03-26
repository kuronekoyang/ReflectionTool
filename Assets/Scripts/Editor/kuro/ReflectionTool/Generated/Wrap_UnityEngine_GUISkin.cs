// This file was automatically generated by kuroneko.
// ReSharper disable InconsistentNaming
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using kuro.Core;

namespace kuro.ReflectionTool
{
    public partial struct UnityEngine_GUISkin
    {

        /// <summary>
        /// <see cref="UnityEngine.GUISkin"/>
        /// </summary>
        public static Type __type__ { get; } = ReflectionUtils.GetTypeByFullName("UnityEngine.IMGUIModule", "UnityEngine.GUISkin");

        public delegate void SkinChangedDelegate();

        public UnityEngine.GUIStyle horizontalSliderThumbExtent
        {
            get => __Get__horizontalSliderThumbExtent(__self__);
            set => __Set__horizontalSliderThumbExtent(__self__, value);
        }

        public UnityEngine.GUIStyle sliderMixed
        {
            get => __Get__sliderMixed(__self__);
            set => __Set__sliderMixed(__self__, value);
        }

        public UnityEngine.GUIStyle verticalSliderThumbExtent
        {
            get => __Get__verticalSliderThumbExtent(__self__);
            set => __Set__verticalSliderThumbExtent(__self__, value);
        }

        public static UnityEngine.GUIStyle error => __Get__error(null);

        public UnityEngine.Font m_Font
        {
            get => __Get__m_Font(__self__);
            set => __Set__m_Font(__self__, value);
        }

        public UnityEngine.GUIStyle m_box
        {
            get => __Get__m_box(__self__);
            set => __Set__m_box(__self__, value);
        }

        public UnityEngine.GUIStyle m_button
        {
            get => __Get__m_button(__self__);
            set => __Set__m_button(__self__, value);
        }

        public UnityEngine.GUIStyle m_toggle
        {
            get => __Get__m_toggle(__self__);
            set => __Set__m_toggle(__self__, value);
        }

        public UnityEngine.GUIStyle m_label
        {
            get => __Get__m_label(__self__);
            set => __Set__m_label(__self__, value);
        }

        public UnityEngine.GUIStyle m_textField
        {
            get => __Get__m_textField(__self__);
            set => __Set__m_textField(__self__, value);
        }

        public UnityEngine.GUIStyle m_textArea
        {
            get => __Get__m_textArea(__self__);
            set => __Set__m_textArea(__self__, value);
        }

        public UnityEngine.GUIStyle m_window
        {
            get => __Get__m_window(__self__);
            set => __Set__m_window(__self__, value);
        }

        public UnityEngine.GUIStyle m_horizontalSlider
        {
            get => __Get__m_horizontalSlider(__self__);
            set => __Set__m_horizontalSlider(__self__, value);
        }

        public UnityEngine.GUIStyle m_horizontalSliderThumb
        {
            get => __Get__m_horizontalSliderThumb(__self__);
            set => __Set__m_horizontalSliderThumb(__self__, value);
        }

        public UnityEngine.GUIStyle m_horizontalSliderThumbExtent
        {
            get => __Get__m_horizontalSliderThumbExtent(__self__);
            set => __Set__m_horizontalSliderThumbExtent(__self__, value);
        }

        public UnityEngine.GUIStyle m_verticalSlider
        {
            get => __Get__m_verticalSlider(__self__);
            set => __Set__m_verticalSlider(__self__, value);
        }

        public UnityEngine.GUIStyle m_verticalSliderThumb
        {
            get => __Get__m_verticalSliderThumb(__self__);
            set => __Set__m_verticalSliderThumb(__self__, value);
        }

        public UnityEngine.GUIStyle m_verticalSliderThumbExtent
        {
            get => __Get__m_verticalSliderThumbExtent(__self__);
            set => __Set__m_verticalSliderThumbExtent(__self__, value);
        }

        public UnityEngine.GUIStyle m_SliderMixed
        {
            get => __Get__m_SliderMixed(__self__);
            set => __Set__m_SliderMixed(__self__, value);
        }

        public UnityEngine.GUIStyle m_horizontalScrollbar
        {
            get => __Get__m_horizontalScrollbar(__self__);
            set => __Set__m_horizontalScrollbar(__self__, value);
        }

        public UnityEngine.GUIStyle m_horizontalScrollbarThumb
        {
            get => __Get__m_horizontalScrollbarThumb(__self__);
            set => __Set__m_horizontalScrollbarThumb(__self__, value);
        }

        public UnityEngine.GUIStyle m_horizontalScrollbarLeftButton
        {
            get => __Get__m_horizontalScrollbarLeftButton(__self__);
            set => __Set__m_horizontalScrollbarLeftButton(__self__, value);
        }

        public UnityEngine.GUIStyle m_horizontalScrollbarRightButton
        {
            get => __Get__m_horizontalScrollbarRightButton(__self__);
            set => __Set__m_horizontalScrollbarRightButton(__self__, value);
        }

        public UnityEngine.GUIStyle m_verticalScrollbar
        {
            get => __Get__m_verticalScrollbar(__self__);
            set => __Set__m_verticalScrollbar(__self__, value);
        }

        public UnityEngine.GUIStyle m_verticalScrollbarThumb
        {
            get => __Get__m_verticalScrollbarThumb(__self__);
            set => __Set__m_verticalScrollbarThumb(__self__, value);
        }

        public UnityEngine.GUIStyle m_verticalScrollbarUpButton
        {
            get => __Get__m_verticalScrollbarUpButton(__self__);
            set => __Set__m_verticalScrollbarUpButton(__self__, value);
        }

        public UnityEngine.GUIStyle m_verticalScrollbarDownButton
        {
            get => __Get__m_verticalScrollbarDownButton(__self__);
            set => __Set__m_verticalScrollbarDownButton(__self__, value);
        }

        public UnityEngine.GUIStyle m_ScrollView
        {
            get => __Get__m_ScrollView(__self__);
            set => __Set__m_ScrollView(__self__, value);
        }

        public UnityEngine.GUIStyle[] m_CustomStyles
        {
            get => __Get__m_CustomStyles(__self__);
            set => __Set__m_CustomStyles(__self__, value);
        }

        public UnityEngine.GUISettings m_Settings
        {
            get => __Get__m_Settings(__self__);
            set => __Set__m_Settings(__self__, value);
        }

        public static UnityEngine.GUIStyle ms_Error
        {
            get => __Get__ms_Error(null);
            set => __Set__ms_Error(null, value);
        }

        public System.Collections.Generic.Dictionary<string, UnityEngine.GUIStyle> m_Styles
        {
            get => __Get__m_Styles(__self__);
            set => __Set__m_Styles(__self__, value);
        }

        public static SkinChangedDelegate m_SkinChanged
        {
            get => (__m_SkinChanged.GetValue(null) as Delegate)?.Cast<SkinChangedDelegate>();
            set => __m_SkinChanged.SetValue(null, value?.Cast(__m_SkinChanged.FieldType));
        }

        public static UnityEngine.GUISkin current
        {
            get => __Get__current(null);
            set => __Set__current(null, value);
        }

        public void OnEnable()
        {
            __OnEnable?.Invoke(__self__, Array.Empty<object>());
        }

        public static void CleanupRoots() => __CleanupRoots();

        public void Apply()
        {
            __Apply?.Invoke(__self__, Array.Empty<object>());
        }

        public void BuildStyleCache()
        {
            __BuildStyleCache?.Invoke(__self__, Array.Empty<object>());
        }

        public void MakeCurrent()
        {
            __MakeCurrent?.Invoke(__self__, Array.Empty<object>());
        }

        public UnityEngine_GUISkin(object __self__) => this.__self__ = __self__ as UnityEngine.Object;
        public UnityEngine.Object __self__;
        public bool __valid__ => __self__ != null && __type__ != null;
        public UnityEngine.GUISkin __super__ => (UnityEngine.GUISkin)(__self__);
        public UnityEngine.GUISkin __base__ => (UnityEngine.GUISkin)(__self__);

        private static Func<object, UnityEngine.Font> ___Get__m_Font;
        private static Func<object, UnityEngine.Font> __Get__m_Font => ___Get__m_Font ??= (__type__.QF("m_Font")).ILGet<UnityEngine.Font>();
        private static Action<object, UnityEngine.Font> ___Set__m_Font;
        private static Action<object, UnityEngine.Font> __Set__m_Font => ___Set__m_Font ??= (__type__.QF("m_Font")).ILSet<UnityEngine.Font>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_box;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_box => ___Get__m_box ??= (__type__.QF("m_box")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_box;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_box => ___Set__m_box ??= (__type__.QF("m_box")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_button;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_button => ___Get__m_button ??= (__type__.QF("m_button")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_button;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_button => ___Set__m_button ??= (__type__.QF("m_button")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_toggle;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_toggle => ___Get__m_toggle ??= (__type__.QF("m_toggle")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_toggle;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_toggle => ___Set__m_toggle ??= (__type__.QF("m_toggle")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_label;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_label => ___Get__m_label ??= (__type__.QF("m_label")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_label;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_label => ___Set__m_label ??= (__type__.QF("m_label")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_textField;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_textField => ___Get__m_textField ??= (__type__.QF("m_textField")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_textField;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_textField => ___Set__m_textField ??= (__type__.QF("m_textField")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_textArea;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_textArea => ___Get__m_textArea ??= (__type__.QF("m_textArea")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_textArea;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_textArea => ___Set__m_textArea ??= (__type__.QF("m_textArea")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_window;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_window => ___Get__m_window ??= (__type__.QF("m_window")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_window;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_window => ___Set__m_window ??= (__type__.QF("m_window")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_horizontalSlider;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_horizontalSlider => ___Get__m_horizontalSlider ??= (__type__.QF("m_horizontalSlider")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_horizontalSlider;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_horizontalSlider => ___Set__m_horizontalSlider ??= (__type__.QF("m_horizontalSlider")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_horizontalSliderThumb;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_horizontalSliderThumb => ___Get__m_horizontalSliderThumb ??= (__type__.QF("m_horizontalSliderThumb")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_horizontalSliderThumb;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_horizontalSliderThumb => ___Set__m_horizontalSliderThumb ??= (__type__.QF("m_horizontalSliderThumb")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_horizontalSliderThumbExtent;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_horizontalSliderThumbExtent => ___Get__m_horizontalSliderThumbExtent ??= (__type__.QF("m_horizontalSliderThumbExtent")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_horizontalSliderThumbExtent;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_horizontalSliderThumbExtent => ___Set__m_horizontalSliderThumbExtent ??= (__type__.QF("m_horizontalSliderThumbExtent")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_verticalSlider;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_verticalSlider => ___Get__m_verticalSlider ??= (__type__.QF("m_verticalSlider")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_verticalSlider;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_verticalSlider => ___Set__m_verticalSlider ??= (__type__.QF("m_verticalSlider")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_verticalSliderThumb;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_verticalSliderThumb => ___Get__m_verticalSliderThumb ??= (__type__.QF("m_verticalSliderThumb")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_verticalSliderThumb;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_verticalSliderThumb => ___Set__m_verticalSliderThumb ??= (__type__.QF("m_verticalSliderThumb")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_verticalSliderThumbExtent;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_verticalSliderThumbExtent => ___Get__m_verticalSliderThumbExtent ??= (__type__.QF("m_verticalSliderThumbExtent")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_verticalSliderThumbExtent;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_verticalSliderThumbExtent => ___Set__m_verticalSliderThumbExtent ??= (__type__.QF("m_verticalSliderThumbExtent")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_SliderMixed;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_SliderMixed => ___Get__m_SliderMixed ??= (__type__.QF("m_SliderMixed")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_SliderMixed;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_SliderMixed => ___Set__m_SliderMixed ??= (__type__.QF("m_SliderMixed")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_horizontalScrollbar;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_horizontalScrollbar => ___Get__m_horizontalScrollbar ??= (__type__.QF("m_horizontalScrollbar")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_horizontalScrollbar;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_horizontalScrollbar => ___Set__m_horizontalScrollbar ??= (__type__.QF("m_horizontalScrollbar")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_horizontalScrollbarThumb;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_horizontalScrollbarThumb => ___Get__m_horizontalScrollbarThumb ??= (__type__.QF("m_horizontalScrollbarThumb")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_horizontalScrollbarThumb;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_horizontalScrollbarThumb => ___Set__m_horizontalScrollbarThumb ??= (__type__.QF("m_horizontalScrollbarThumb")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_horizontalScrollbarLeftButton;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_horizontalScrollbarLeftButton => ___Get__m_horizontalScrollbarLeftButton ??= (__type__.QF("m_horizontalScrollbarLeftButton")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_horizontalScrollbarLeftButton;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_horizontalScrollbarLeftButton => ___Set__m_horizontalScrollbarLeftButton ??= (__type__.QF("m_horizontalScrollbarLeftButton")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_horizontalScrollbarRightButton;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_horizontalScrollbarRightButton => ___Get__m_horizontalScrollbarRightButton ??= (__type__.QF("m_horizontalScrollbarRightButton")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_horizontalScrollbarRightButton;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_horizontalScrollbarRightButton => ___Set__m_horizontalScrollbarRightButton ??= (__type__.QF("m_horizontalScrollbarRightButton")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_verticalScrollbar;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_verticalScrollbar => ___Get__m_verticalScrollbar ??= (__type__.QF("m_verticalScrollbar")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_verticalScrollbar;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_verticalScrollbar => ___Set__m_verticalScrollbar ??= (__type__.QF("m_verticalScrollbar")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_verticalScrollbarThumb;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_verticalScrollbarThumb => ___Get__m_verticalScrollbarThumb ??= (__type__.QF("m_verticalScrollbarThumb")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_verticalScrollbarThumb;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_verticalScrollbarThumb => ___Set__m_verticalScrollbarThumb ??= (__type__.QF("m_verticalScrollbarThumb")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_verticalScrollbarUpButton;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_verticalScrollbarUpButton => ___Get__m_verticalScrollbarUpButton ??= (__type__.QF("m_verticalScrollbarUpButton")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_verticalScrollbarUpButton;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_verticalScrollbarUpButton => ___Set__m_verticalScrollbarUpButton ??= (__type__.QF("m_verticalScrollbarUpButton")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_verticalScrollbarDownButton;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_verticalScrollbarDownButton => ___Get__m_verticalScrollbarDownButton ??= (__type__.QF("m_verticalScrollbarDownButton")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_verticalScrollbarDownButton;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_verticalScrollbarDownButton => ___Set__m_verticalScrollbarDownButton ??= (__type__.QF("m_verticalScrollbarDownButton")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__m_ScrollView;
        private static Func<object, UnityEngine.GUIStyle> __Get__m_ScrollView => ___Get__m_ScrollView ??= (__type__.QF("m_ScrollView")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__m_ScrollView;
        private static Action<object, UnityEngine.GUIStyle> __Set__m_ScrollView => ___Set__m_ScrollView ??= (__type__.QF("m_ScrollView")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle[]> ___Get__m_CustomStyles;
        private static Func<object, UnityEngine.GUIStyle[]> __Get__m_CustomStyles => ___Get__m_CustomStyles ??= (__type__.QF("m_CustomStyles")).ILGet<UnityEngine.GUIStyle[]>();
        private static Action<object, UnityEngine.GUIStyle[]> ___Set__m_CustomStyles;
        private static Action<object, UnityEngine.GUIStyle[]> __Set__m_CustomStyles => ___Set__m_CustomStyles ??= (__type__.QF("m_CustomStyles")).ILSet<UnityEngine.GUIStyle[]>();

        private static Func<object, UnityEngine.GUISettings> ___Get__m_Settings;
        private static Func<object, UnityEngine.GUISettings> __Get__m_Settings => ___Get__m_Settings ??= (__type__.QF("m_Settings")).ILGet<UnityEngine.GUISettings>();
        private static Action<object, UnityEngine.GUISettings> ___Set__m_Settings;
        private static Action<object, UnityEngine.GUISettings> __Set__m_Settings => ___Set__m_Settings ??= (__type__.QF("m_Settings")).ILSet<UnityEngine.GUISettings>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__ms_Error;
        private static Func<object, UnityEngine.GUIStyle> __Get__ms_Error => ___Get__ms_Error ??= (__type__.QF("ms_Error")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__ms_Error;
        private static Action<object, UnityEngine.GUIStyle> __Set__ms_Error => ___Set__ms_Error ??= (__type__.QF("ms_Error")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, System.Collections.Generic.Dictionary<string, UnityEngine.GUIStyle>> ___Get__m_Styles;
        private static Func<object, System.Collections.Generic.Dictionary<string, UnityEngine.GUIStyle>> __Get__m_Styles => ___Get__m_Styles ??= (__type__.QF("m_Styles")).ILGet<System.Collections.Generic.Dictionary<string, UnityEngine.GUIStyle>>();
        private static Action<object, System.Collections.Generic.Dictionary<string, UnityEngine.GUIStyle>> ___Set__m_Styles;
        private static Action<object, System.Collections.Generic.Dictionary<string, UnityEngine.GUIStyle>> __Set__m_Styles => ___Set__m_Styles ??= (__type__.QF("m_Styles")).ILSet<System.Collections.Generic.Dictionary<string, UnityEngine.GUIStyle>>();

        private static FieldInfo ___m_SkinChanged;
        private static FieldInfo __m_SkinChanged => ___m_SkinChanged ??= __type__?.QF("m_SkinChanged");

        private static Func<object, UnityEngine.GUISkin> ___Get__current;
        private static Func<object, UnityEngine.GUISkin> __Get__current => ___Get__current ??= (__type__.QF("current")).ILGet<UnityEngine.GUISkin>();
        private static Action<object, UnityEngine.GUISkin> ___Set__current;
        private static Action<object, UnityEngine.GUISkin> __Set__current => ___Set__current ??= (__type__.QF("current")).ILSet<UnityEngine.GUISkin>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__horizontalSliderThumbExtent;
        private static Func<object, UnityEngine.GUIStyle> __Get__horizontalSliderThumbExtent => ___Get__horizontalSliderThumbExtent ??= (__type__.QP("horizontalSliderThumbExtent")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__horizontalSliderThumbExtent;
        private static Action<object, UnityEngine.GUIStyle> __Set__horizontalSliderThumbExtent => ___Set__horizontalSliderThumbExtent ??= (__type__.QP("horizontalSliderThumbExtent")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__sliderMixed;
        private static Func<object, UnityEngine.GUIStyle> __Get__sliderMixed => ___Get__sliderMixed ??= (__type__.QP("sliderMixed")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__sliderMixed;
        private static Action<object, UnityEngine.GUIStyle> __Set__sliderMixed => ___Set__sliderMixed ??= (__type__.QP("sliderMixed")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__verticalSliderThumbExtent;
        private static Func<object, UnityEngine.GUIStyle> __Get__verticalSliderThumbExtent => ___Get__verticalSliderThumbExtent ??= (__type__.QP("verticalSliderThumbExtent")).ILGet<UnityEngine.GUIStyle>();
        private static Action<object, UnityEngine.GUIStyle> ___Set__verticalSliderThumbExtent;
        private static Action<object, UnityEngine.GUIStyle> __Set__verticalSliderThumbExtent => ___Set__verticalSliderThumbExtent ??= (__type__.QP("verticalSliderThumbExtent")).ILSet<UnityEngine.GUIStyle>();

        private static Func<object, UnityEngine.GUIStyle> ___Get__error;
        private static Func<object, UnityEngine.GUIStyle> __Get__error => ___Get__error ??= (__type__.QP("error")).ILGet<UnityEngine.GUIStyle>();

        private static MethodInfo ___OnEnable;
        private static MethodInfo __OnEnable => ___OnEnable ??= __type__.QM("OnEnable");

        private delegate void __D__CleanupRoots();
        private static __D__CleanupRoots ___CleanupRoots;
        private static __D__CleanupRoots __CleanupRoots => ___CleanupRoots ??= __type__.QM("CleanupRoots")?.CreateDelegate<__D__CleanupRoots>();

        private static MethodInfo ___Apply;
        private static MethodInfo __Apply => ___Apply ??= __type__.QM("Apply");

        private static MethodInfo ___BuildStyleCache;
        private static MethodInfo __BuildStyleCache => ___BuildStyleCache ??= __type__.QM("BuildStyleCache");

        private static MethodInfo ___MakeCurrent;
        private static MethodInfo __MakeCurrent => ___MakeCurrent ??= __type__.QM("MakeCurrent");
    }
    public static class UnityEngine_GUISkin_Extension
    {
        public static UnityEngine_GUISkin ReflectionHelper(this UnityEngine.GUISkin self) => new(self);
    }
}
