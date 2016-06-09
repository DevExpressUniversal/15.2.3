#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Utils;
using DevExpress.Utils.Paint;
using DevExpress.XtraEditors;
namespace DevExpress.Utils {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TextOptions {
		static TextOptions defaultOptions, defaultOptionsNoWrap, defaultOptionsMultiLine, defaultOptionsNoWrapEx, defaultOptionsCenteredWithEllipsis;
#if !SL
	[DevExpressUtilsLocalizedDescription("TextOptionsDefaultOptions")]
#endif
		public static TextOptions DefaultOptions {
			get {
				if(defaultOptions == null) {
					defaultOptions = new TextOptions(null);
				}
				return defaultOptions;
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("TextOptionsDefaultOptionsNoWrap")]
#endif
		public static TextOptions DefaultOptionsNoWrap {
			get {
				if(defaultOptionsNoWrap == null) {
					defaultOptionsNoWrap = new TextOptions(HorzAlignment.Near, VertAlignment.Top, WordWrap.NoWrap, Trimming.EllipsisCharacter);
				}
				return defaultOptionsNoWrap;
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("TextOptionsDefaultOptionsNoWrapEx")]
#endif
		public static TextOptions DefaultOptionsNoWrapEx {
			get {
				if(defaultOptionsNoWrapEx == null) {
					defaultOptionsNoWrapEx = new TextOptions(HorzAlignment.Near, VertAlignment.Center, WordWrap.NoWrap, Trimming.EllipsisCharacter);
				}
				return defaultOptionsNoWrapEx;
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("TextOptionsDefaultOptionsMultiLine")]
#endif
		public static TextOptions DefaultOptionsMultiLine {
			get {
				if(defaultOptionsMultiLine == null) {
					defaultOptionsMultiLine = new TextOptions(HorzAlignment.Near, VertAlignment.Top, WordWrap.Wrap, Trimming.EllipsisCharacter);
				}
				return defaultOptionsMultiLine;
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("TextOptionsDefaultOptionsCenteredWithEllipsis")]
#endif
		public static TextOptions DefaultOptionsCenteredWithEllipsis {
			get {
				if(defaultOptionsCenteredWithEllipsis == null) {
					defaultOptionsCenteredWithEllipsis = new TextOptions(HorzAlignment.Center, VertAlignment.Center, WordWrap.NoWrap, Trimming.EllipsisCharacter);
				}
				return defaultOptionsCenteredWithEllipsis;
			}
		}
		HKeyPrefix hotKeyPrefix;
		HorzAlignment hAlignment;
		VertAlignment vAlignment;
		WordWrap wordWrap;
		Trimming trimming;
		AppearanceObject owner;
		bool rightToLeft;
		public TextOptions(AppearanceObject owner) {
			Reset();
			this.owner = owner;
		}
		public TextOptions(HorzAlignment hAlignment, VertAlignment vAlignment, WordWrap wordWrap, Trimming trimming) : 
			this(hAlignment, vAlignment, wordWrap, trimming, HKeyPrefix.Default) {
		}
		public TextOptions(HorzAlignment hAlignment, VertAlignment vAlignment, WordWrap wordWrap, Trimming trimming, HKeyPrefix hotKeyPrefix) {
			this.hotKeyPrefix = hotKeyPrefix;
			this.trimming = trimming;
			this.hAlignment = hAlignment;
			this.vAlignment = vAlignment;
			this.wordWrap = wordWrap;
			this.trimming = trimming;
			this.owner = null;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RightToLeft {
			get { return rightToLeft; }
			set { rightToLeft = value; }
		}
		static bool useGenericDefaultStringFormat = false;
		public static void ForceUseGenericDefaultStringFormat() { useGenericDefaultStringFormat = true; } 
		[ThreadStatic]
		static StringFormat defaultStringFormat;
#if !SL
	[DevExpressUtilsLocalizedDescription("TextOptionsDefaultStringFormat")]
#endif
		public static StringFormat DefaultStringFormat {
			get {
				if(defaultStringFormat == null) {
					defaultStringFormat = new StringFormat(useGenericDefaultStringFormat ? StringFormat.GenericDefault : StringFormat.GenericTypographic);
					defaultStringFormat.FormatFlags = 0;
					defaultStringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
					defaultStringFormat.Alignment = StringAlignment.Near;
					defaultStringFormat.LineAlignment = StringAlignment.Near;
					defaultStringFormat.Trimming = StringTrimming.None;
				}
				return defaultStringFormat;
			}
		}
		protected AppearanceObject Owner { get { return owner; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("TextOptionsWordWrap"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.TextOptions.WordWrap"),
		DefaultValue(WordWrap.Default), XtraSerializableProperty()
		]
		public virtual WordWrap WordWrap {
			get { return wordWrap; }
			set {
				if(WordWrap == value) return;
				wordWrap = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("TextOptionsHotkeyPrefix"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.TextOptions.HotkeyPrefix"),
		DefaultValue(HKeyPrefix.Default), XtraSerializableProperty()
		]
		public virtual HKeyPrefix HotkeyPrefix {
			get { return hotKeyPrefix; }
			set {
				if(HotkeyPrefix == value) return;
				hotKeyPrefix = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("TextOptionsHAlignment"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.TextOptions.HAlignment"),
		DefaultValue(HorzAlignment.Default), XtraSerializableProperty()
		]
		public virtual HorzAlignment HAlignment {
			get { return hAlignment; }
			set {
				if(HAlignment == value) return;
				hAlignment = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("TextOptionsVAlignment"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.TextOptions.VAlignment"),
		DefaultValue(VertAlignment.Default), XtraSerializableProperty()
		]
		public virtual VertAlignment VAlignment {
			get { return vAlignment; }
			set {
				if(VAlignment == value) return;
				vAlignment = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("TextOptionsTrimming"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.TextOptions.Trimming"),
		DefaultValue(Trimming.Default), XtraSerializableProperty()
		]
		public virtual Trimming Trimming {
			get { return trimming; }
			set {
				if(Trimming == value) return;
				trimming = value;
				OnChanged();
			}
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
		internal void Assign(AppearanceDefault appearanceDefault) {
			if(appearanceDefault == null) return;
			if(appearanceDefault.HAlignment != HorzAlignment.Default) 
				this.hAlignment = appearanceDefault.HAlignment;
			if(appearanceDefault.VAlignment != VertAlignment.Default) 
				this.vAlignment = appearanceDefault.VAlignment;
			OnChanged();
		}
		public virtual void Assign(TextOptions options) {
			if(options == null || options == this) return;
			this.hotKeyPrefix = options.HotkeyPrefix;
			this.trimming = options.Trimming;
			this.wordWrap = options.WordWrap;
			this.vAlignment = options.VAlignment;
			this.hAlignment = options.HAlignment;
			this.rightToLeft = options.RightToLeft;
			OnChanged();
		}
		public virtual void Reset() {
			this.trimming = Trimming.Default;
			this.wordWrap = WordWrap.Default;
			this.vAlignment = VertAlignment.Default;
			this.hAlignment = HorzAlignment.Default;
			this.hotKeyPrefix = HKeyPrefix.Default;
			this.rightToLeft = false;
			OnChanged();
		}
		public virtual bool IsEqual(TextOptions options) {
			return this.hotKeyPrefix == options.HotkeyPrefix &&
				this.trimming == options.Trimming &&
				this.wordWrap == options.WordWrap &&
				this.vAlignment == options.VAlignment &&
				this.hAlignment == options.HAlignment && this.RightToLeft == options.RightToLeft;
		}
		public void UpdateDefaultOptions(TextOptions defaultOptions) {
			if(defaultOptions == null) return;
			if(HotkeyPrefix == HKeyPrefix.Default) this.hotKeyPrefix = defaultOptions.HotkeyPrefix;
			if(Trimming == Trimming.Default) this.trimming = defaultOptions.Trimming;
			if(WordWrap == WordWrap.Default) this.wordWrap = defaultOptions.WordWrap;
			if(VAlignment == VertAlignment.Default) this.vAlignment = defaultOptions.VAlignment;
			if(HAlignment == HorzAlignment.Default) this.hAlignment = defaultOptions.HAlignment;
		}
		public StringFormat GetStringFormat(TextOptions defaultOptions) {
			if(defaultOptions == null) defaultOptions = DefaultOptions;
			int hash = GetHash(defaultOptions);
			StringFormat format;
			if(!StringFormats.TryGetValue(hash, out format)) {
				format = new StringFormat(DefaultStringFormat);
				format.FormatFlags = GetFormatFlags(defaultOptions.WordWrap);
				format.Alignment = GetHAlignment(defaultOptions.HAlignment);
				format.Trimming = GetTrimming(defaultOptions.Trimming);
				format.HotkeyPrefix = GetHotkeyPrefix(defaultOptions.HotkeyPrefix);
				format.LineAlignment = GetVAlignment(defaultOptions.VAlignment);
				StringFormats.Add(hash, format);
			}
			return format;
		}
		public StringFormat GetStringFormat() { return GetStringFormat(DefaultOptions); }
		protected StringFormatFlags GetFormatFlags(WordWrap defaultWrap) {
			StringFormatFlags flags = 0;
			if(RightToLeft) flags |= StringFormatFlags.DirectionRightToLeft;
			WordWrap wrap = WordWrap;
			if(wrap == WordWrap.Default) wrap = defaultWrap;
			if(wrap != WordWrap.Wrap) flags |= StringFormatFlags.NoWrap;
			return flags;
		}
		protected virtual int GetHash(TextOptions defaultOptions) {
			int code = (GetInt((int)GetHAlignment(defaultOptions.HAlignment))) |
					   ((GetInt((int)GetVAlignment(defaultOptions.VAlignment))) << 4) |
					   ((GetInt((int)(Trimming == Trimming.Default ? defaultOptions.Trimming : Trimming))) << 8) |
					   ((GetInt((int)(WordWrap == WordWrap.Default ? defaultOptions.WordWrap : WordWrap))) << 16) | 
					   ((GetInt((int)(HotkeyPrefix == HKeyPrefix.Default ? DefaultOptions.HotkeyPrefix : HotkeyPrefix))) << 24) |
					   ((GetInt((int)(RightToLeft ? 1 : 0))) << 26);
			return code;
		}
		int GetInt(int v) { return v + 1; }
		[ThreadStatic]
		static Dictionary<int, StringFormat> stringFormats;
		static protected Dictionary<int, StringFormat> StringFormats {
			get {
				if(stringFormats == null)
					stringFormats = new Dictionary<int, StringFormat>();
				return stringFormats;
			}
		}
		static StringTrimming[] trim = new StringTrimming[] { StringTrimming.None, StringTrimming.None, StringTrimming.Character, StringTrimming.Word, StringTrimming.EllipsisCharacter, StringTrimming.EllipsisWord, StringTrimming.EllipsisPath };
		static HotkeyPrefix[] hprefix = new HotkeyPrefix[] {  
			System.Drawing.Text.HotkeyPrefix.None, System.Drawing.Text.HotkeyPrefix.None, System.Drawing.Text.HotkeyPrefix.Show, System.Drawing.Text.HotkeyPrefix.Hide};
		[Obsolete("Use DevExpress.Data.Utils.AlignmentConverter class")]
		public static StringAlignment HorzAlignmentToStringAlignment(HorzAlignment align) {
			return AlignmentConverter.HorzAlignmentToStringAlignment(align);
		}
		[Obsolete("Use DevExpress.Data.Utils.AlignmentConverter class")]
		public static StringAlignment VertAlignmentToStringAlignment(VertAlignment align) {
			return AlignmentConverter.VertAlignmentToStringAlignment(align);
		}
		protected HotkeyPrefix GetHotkeyPrefix(HKeyPrefix defaultHotkeyPrefix) {
			HKeyPrefix prefix = HotkeyPrefix;
			if(prefix == HKeyPrefix.Default) prefix = defaultHotkeyPrefix;
			return hprefix[(int)prefix];
		}
		protected StringTrimming GetTrimming(Trimming defaultTrimming) { 
			Trimming trimming = Trimming;
			if(trimming == Trimming.Default) trimming = defaultTrimming;
			return trim[(int)trimming]; 
		}
		protected internal StringAlignment GetHAlignment(HorzAlignment defaultAlignment) { 
			HorzAlignment ha = HAlignment;
			if(ha == HorzAlignment.Default)
				ha = defaultAlignment;
			return AlignmentConverter.HorzAlignmentToStringAlignment(ha); 
		}
		protected internal StringAlignment GetVAlignment(VertAlignment defaultAlignment) { 
			VertAlignment va = VAlignment;
			if(va == VertAlignment.Default)
				va = defaultAlignment;
			return AlignmentConverter.VertAlignmentToStringAlignment(va); 
		}
		protected internal virtual bool IsDefault() {
			return HAlignment == HorzAlignment.Default && VAlignment == VertAlignment.Default &&
				WordWrap == DevExpress.Utils.WordWrap.Default && Trimming == DevExpress.Utils.Trimming.Default 
				&& HotkeyPrefix == HKeyPrefix.Default;
		}
		protected virtual void OnChanged() {
			if(Owner != null) Owner.OnTextOptionsChanged();
		}
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class SerializableAppearanceObject : AppearanceObject {
	}
	public class AppearanceObjectPrint : AppearanceObject {
		public AppearanceObjectPrint(IAppearanceOwner owner) : this(owner, null, string.Empty) { }
		public AppearanceObjectPrint() : this(null, null, string.Empty) { }
		public AppearanceObjectPrint(IAppearanceOwner owner, AppearanceObject parentAppearance, string name) : base(owner, parentAppearance, name) { }
		protected override Font InnerDefaultFont {
			get {
				if(FontProvider != null && FontProvider.DefaultFont != null) return FontProvider.DefaultFont;
				return DefaultPrintFont;
			}
		}
		static Font defaultPrintFont;
		public static Font DefaultPrintFont {
			get {
				if(defaultPrintFont == null) defaultPrintFont = CreateDefaultPrintFont();
				return defaultPrintFont;
			}
			set {
				defaultPrintFont = value;
			}
		}
		internal static Font CreateDefaultPrintFont() {
			try {
				return new Font(new FontFamily("Tahoma"), 8.25f);
			}
			catch { }
			return Control.DefaultFont;
		}
	}
	public class AppearanceObjectEx : AppearanceObject {
		public AppearanceObjectEx(IAppearanceOwner owner) : this(owner, null, string.Empty) { }
		public AppearanceObjectEx() : this(null, null, string.Empty) { }
		public AppearanceObjectEx(IAppearanceOwner owner, AppearanceObject parentAppearance, string name) : base(owner, parentAppearance, name) { }
		public override object Clone() {
			AppearanceObjectEx appearance = new AppearanceObjectEx(null, ParentAppearance, string.Empty);
			appearance.Assign(this);
			return appearance;
		}
		protected override AppearanceOptions CreateOptions() { return new AppearanceOptionsEx(); }
		void ResetOptions() { Options.Reset(); }
		bool ShouldSerializeOptions() { return Options.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceObjectExOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObjectEx.Options"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public new AppearanceOptionsEx Options { get { return base.Options as AppearanceOptionsEx; } }
		public override void Assign(AppearanceDefault appearanceDefault) {
			if(appearanceDefault == null) return;
			BeginUpdate();
			try {
				base.Assign(appearanceDefault);
				Options.HighPriority = appearanceDefault.HighPriority == DefaultBoolean.True;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class AppearanceOptionsEx : AppearanceOptions {
		bool highPriority;
		protected internal override void ResetOptions() {
			base.ResetOptions();
			this.highPriority = false;
		}
		protected internal override bool GetOptionValue(string name) {
			if(IsEqual(name, AppearanceObject.optHighPriority)) return HighPriority;
			return base.GetOptionValue(name);
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				AppearanceOptionsEx ex = options as AppearanceOptionsEx;
				if(ex != null) {
					this.highPriority = ex.HighPriority;
				}
			}
			finally {
				EndUpdate();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceOptionsExHighPriority"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceOptionsEx.HighPriority"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(false), XtraSerializableProperty()
		]
		public virtual bool HighPriority {
			get { return highPriority; }
			set {
				if(HighPriority == value) return;
				bool prevValue = HighPriority;
				highPriority = value;
				OnChanged(AppearanceObject.optHighPriority, prevValue, HighPriority);
			}
		}
	}
	[Browsable(false)]
	public class FrozenAppearance : AppearanceObject {
		public FrozenAppearance() { }
		public FrozenAppearance(AppearanceObject appearance) {
			AssignInternal(appearance);
		}
		public FrozenAppearance(AppearanceDefault defaultAppearance){
			BeginUpdate();
			Assign(defaultAppearance);
			CancelUpdate();
		}
		public FrozenAppearance(AppearanceObject appearance, AppearanceDefault defaultAppearance) {
			BeginUpdate();
			AppearanceHelper.Combine(this, appearance, defaultAppearance);
			CancelUpdate();
		}
		protected internal override void OnPaintChanged() { }
		protected internal override void OnChanged() { }
		protected internal override void OnSizeChanged() { }
		protected override AppearanceOptions CreateOptions() { 
			return new FrozenOptions(); 
		}
		class FrozenOptions : AppearanceOptions {
			protected override void RaiseOnChanged(DevExpress.Utils.Controls.BaseOptionChangedEventArgs e) { }
			protected override void RaisePropertyChanged(string propertyName) { }
			protected internal override void AddChangedCoreHandler(BaseOptionChangedEventHandler handler) { }
			protected internal override void RemoveChangedCoreHandler(BaseOptionChangedEventHandler handler) { }
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class AppearanceObject : IDisposable, ICloneable, IXtraSerializableLayoutEx, IXtraSerializable {
		string name = string.Empty;
		internal Color foreColor, backColor, backColor2, borderColor;
		TextureBrush textureBrush;
		internal Image image;
		internal LinearGradientMode gradientMode;
		Font _font;
		TextOptions textOptions;
		AppearanceObject parentAppearance;
		internal AppearanceOptions options;
		protected int lockUpdate;
		int fontSizeDelta;
		FontStyle fontStyleDelta = FontStyle.Regular;
		IAppearanceOwner owner;
		bool isDisposed;
		internal static string 
			optUseTextOptions = "UseTextOptions",
			optUseForeColor = "UseForeColor",
			optUseBorderColor = "UseBorderColor",
			optUseBackColor = "UseBackColor",
			optUseImage = "UseImage",
			optUseFont = "UseFont",
			optHighPriority = "HighPriority";
		[Browsable(false)]
		public event EventHandler Changed, PaintChanged, SizeChanged;
		[ThreadStatic]
		static AppearanceObject dummy;
		internal static AppearanceObject Dummy {
			get {
				if(dummy == null) dummy = new AppearanceObject();
				return dummy;
			}
		}
		static AppearanceObject() { DevExpress.Utils.Design.DXAssemblyResolverEx.Init(); }
		public AppearanceObject(IAppearanceOwner owner, AppearanceObject parentAppearance) : this(owner, false) {
			this.parentAppearance = parentAppearance;
		}
		public AppearanceObject(IAppearanceOwner owner, bool reserved) : this((AppearanceObject)null) {
			this.owner = owner;
		}
		protected IAppearanceOwner Owner { get { return owner; } }
		protected IAppearanceDefaultFontProvider FontProvider { get { return Owner as IAppearanceDefaultFontProvider ; } }
		public AppearanceObject(AppearanceDefault appearanceDefault) : this((AppearanceObject)null) {
			Assign(appearanceDefault);
		}
		public AppearanceObject(AppearanceObject main, AppearanceObject defaultAppearance) : this((AppearanceObject)null) {
			AppearanceHelper.Combine(this, main, defaultAppearance);
		}
		public AppearanceObject(AppearanceObject main, AppearanceDefault appearanceDefault) : this((AppearanceObject)null) {
			AppearanceHelper.Combine(this, main, appearanceDefault);
		}
		public AppearanceObject(string name) : this((AppearanceObject)null, name) {
		}
		public AppearanceObject(AppearanceObject parentAppearance) : this(parentAppearance, string.Empty) { }
		public AppearanceObject(AppearanceObject parentAppearance, string name) : this(null, parentAppearance, name) { }
		public AppearanceObject(IAppearanceOwner owner, AppearanceObject parentAppearance, string name) {
			this.name = name;
			this.parentAppearance = parentAppearance;
			Reset();
			this.owner = owner;
		}
		public AppearanceObject() : this((AppearanceObject)null) {
		}
		public virtual void Dispose() {
			DestroyBrush();
			if(options != null)
				this.options.RemoveChangedCoreHandler(new BaseOptionChangedEventHandler(OnOptionsChanged));
			this.Changed = null;
			this.SizeChanged = this.PaintChanged = null;
			isDisposed = true;
		}
		public virtual object Clone() {
			AppearanceObject appearance = new AppearanceObject(ParentAppearance);
			appearance.Assign(this);
			return appearance;
		}
		public override string ToString() { return "Appearance"; }
		static AppearanceObject controlAppearance, emptyAppearance;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDisposed { get { return isDisposed; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("AppearanceObjectControlAppearance")]
#endif
		public static AppearanceObject ControlAppearance {
			get {
				if(controlAppearance == null) {
					controlAppearance = new AppearanceObject();
					controlAppearance.BackColor = SystemColors.Control;
					controlAppearance.ForeColor = SystemColors.ControlText;
					controlAppearance.BorderColor = SystemColors.Control;
				}
				return controlAppearance;
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("AppearanceObjectEmptyAppearance")]
#endif
		public static AppearanceObject EmptyAppearance {
			get {
				if(emptyAppearance == null) {
					emptyAppearance = new AppearanceObject();
				}
				return emptyAppearance;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceObject ParentAppearance { get { return parentAppearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Name { get { return name; } set { name = value; } }
		void ResetOptions() { if(options != null) Options.Reset(); }
		bool ShouldSerializeOptions() { return options != null && Options.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceObjectOptions"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.Options"),
		]
		public virtual AppearanceOptions Options {
			get {
				if(options == null) {
					this.options = CreateOptions();
					this.options.AddChangedCoreHandler(new BaseOptionChangedEventHandler(OnOptionsChanged));
					for(int i = 0; i < lockUpdate; i++)
						options.BeginUpdate();
				}
				return options;
			}
		}
		void ResetForeColor() { ForeColor = Color.Empty; }
		protected bool ShouldSerializeForeColor() { return ForeColor != Color.Empty; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceObjectForeColor"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.ForeColor"),
		XtraSerializableProperty(), Localizable(true)
		]
		public Color ForeColor {
			get { return foreColor; }
			set {
				if(ForeColor == value) return;
				foreColor = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseForeColor = foreColor != Color.Empty; } finally { Options.CancelUpdate(); }
				}
				OnPaintChanged();
			}
		}
		public StringFormat GetStringFormat() { return GetTextOptions().GetStringFormat(); }
		public StringFormat GetStringFormat(TextOptions defaultOptions) { return GetTextOptions().GetStringFormat(defaultOptions); }
		[Browsable(false)]
		public HorzAlignment HAlignment { get { return GetTextOptions().HAlignment; } }
		protected bool IsLoading { 
			get { 
				if(Owner == null) return deserializing > 0;
				return Owner.IsLoading || deserializing > 0;
			}
		}
		void ResetBorderColor() { BorderColor = Color.Empty; }
		protected bool ShouldSerializeBorderColor() { return BorderColor != Color.Empty; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceObjectBorderColor"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.BorderColor"),
		XtraSerializableProperty(), RefreshProperties(RefreshProperties.All), Localizable(true)
		]
		public Color BorderColor {
			get { return borderColor; }
			set {
				if(BorderColor == value) return;
				borderColor = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseBorderColor = borderColor != Color.Empty; } finally { Options.CancelUpdate(); }
				}
				OnPaintChanged();
			}
		}
		void ResetBackColor() { BackColor = Color.Empty; }
		protected bool ShouldSerializeBackColor() { return BackColor != Color.Empty; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceObjectBackColor"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.BackColor"),
		XtraSerializableProperty(), RefreshProperties(RefreshProperties.All), Localizable(true)
		]
		public Color BackColor {
			get { return backColor; }
			set {
				if(BackColor == value) return;
				backColor = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseBackColor = backColor != Color.Empty; } finally { Options.CancelUpdate(); }
				}
				OnPaintChanged();
			}
		}
		void ResetBackColor2() { BackColor2 = Color.Empty; }
		bool ShouldSerializeBackColor2() { return BackColor2 != Color.Empty; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceObjectBackColor2"),
#endif
 XtraSerializableProperty(), RefreshProperties(RefreshProperties.All),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.BackColor2"), Localizable(true)
		]
		public virtual Color BackColor2 {
			get { return backColor2; }
			set {
				if(BackColor2 == value) return;
				backColor2 = value;
				OnPaintChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceObjectImage"),
#endif
 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor)),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.Image"), Localizable(true)
		]
		public virtual Image Image {
			get { return image; }
			set {
				if(Image == value) return;
				image = value;
				DestroyBrush();
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseImage = image != null; } finally { Options.CancelUpdate(); }
				}
				OnImageChanged();
			}
		}
		protected virtual void OnImageChanged() {
			OnPaintChanged();
		}
		void ResetTextOptions() { if(textOptions != null) TextOptions.Reset(); }
		bool ShouldSerializeTextOptions() { return textOptions != null && DevExpress.Utils.Design.UniversalTypeConverter.ShouldSerializeObject(TextOptions); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceObjectTextOptions"),
#endif
 Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.TextOptions"),
		]
		public virtual TextOptions TextOptions {
			get {
				if(textOptions == null)
					textOptions = CreateTextOptions();
				return textOptions;
			}
		}
		protected virtual TextOptions CreateTextOptions() {
			return new TextOptions(this);
		}
		protected virtual Font InnerDefaultFont { 
			get { 
				if(FontProvider != null && FontProvider.DefaultFont != null) return FontProvider.DefaultFont;
				return DefaultFont; 
			} 
		}
		void ResetFont() { Font = null; }
		protected virtual bool ShouldSerializeFont() { return _font != null; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceObjectFont"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.Font"),
		TypeConverter(typeof(DevExpress.Utils.Design.FontTypeConverter)),
		RefreshProperties(RefreshProperties.All), XtraSerializableProperty(), Localizable(true)
		]
		public virtual Font Font {
			get {
				if(serializing == 0 && deltaBasedFont != null) {
					OnFontSizeDeltaChanged(true);
					if(deltaBasedFont != null) return deltaBasedFont;
				}
				return GetFontCore();
			}
			set {
				value = CheckFont(value);
				if(_font == value) return;
				if(Font == value) return;
				_font = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseFont = !Font.Equals(InnerDefaultFont); } finally { Options.CancelUpdate(); }
				}
				OnFontSizeDeltaChanged(deltaSourceFont == null || deltaSourceFont.Equals(value));
				OnSizeChanged();
			}
		}
		Font GetFontCore() {
			if(_font == null) return InnerDefaultFont;
			return _font;
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceObjectFontSizeDelta"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.FontSizeDelta"),
		RefreshProperties(RefreshProperties.All), XtraSerializableProperty(99), Localizable(true), DefaultValue(0)
		]
		public virtual int FontSizeDelta {
			get {
				return fontSizeDelta;
			}
			set {
				if(FontSizeDelta == value) return;
				fontSizeDelta = value;
				OnFontSizeDeltaChanged(false);
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseFont = !Font.Equals(InnerDefaultFont); }
					finally { Options.CancelUpdate(); }
				}
				OnSizeChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceObjectFontStyleDelta"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.FontStyleDelta"),
		RefreshProperties(RefreshProperties.All), XtraSerializableProperty(99), Localizable(true), DefaultValue(FontStyle.Regular)
		]
		public virtual FontStyle FontStyleDelta {
			get {
				return fontStyleDelta;
			}
			set {
				if(FontStyleDelta == value) return;
				fontStyleDelta = value;
				OnFontSizeDeltaChanged(false);
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseFont = !Font.Equals(InnerDefaultFont); }
					finally { Options.CancelUpdate(); }
				}
				OnSizeChanged();
			}
		}
#if DEBUGTEST
		internal Font GetFontInternal() { return _font; }
#endif
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceObjectGradientMode"),
#endif
 DefaultValue(LinearGradientMode.Horizontal), XtraSerializableProperty(),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.GradientMode"),
		TypeConverter(typeof(DevExpress.Utils.Design.LinearGradientModeConverter)), Localizable(true)
		]
		public virtual LinearGradientMode GradientMode {
			get { return gradientMode; }
			set {
				if(GradientMode == value) return;
				gradientMode = value;
				OnPaintChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal TextureBrush TextureBrush { get { return textureBrush; } set { textureBrush = value; } }
		public void AssignInternal(AppearanceObject val) {
			BeginUpdate();
			try {
				Assign(val);
			} finally {
				CancelUpdate();
			}
		}
		public void Combine(AppearanceObject val) {
			AppearanceHelper.Apply(this, val);
		}
		Font deltaBasedFont, deltaSourceFont;
		int deltaBasedSize = 0;
		FontStyle deltaBasedStyle = FontStyle.Regular;
		void OnFontSizeDeltaChanged(bool checkOnly) {
			if(checkOnly) {
				if(deltaBasedFont == null) return;
				if(deltaBasedSize == FontSizeDelta && deltaBasedStyle == FontStyleDelta && object.ReferenceEquals(deltaSourceFont, InnerDefaultFont)) return;
			}
			if(FontSizeDelta == 0) {
				this.deltaBasedFont = null;
				this.deltaSourceFont = null;
				if(fontStyleDelta == FontStyle.Regular) return;
			}
			this.deltaBasedStyle = FontStyleDelta;
			this.deltaBasedSize = FontSizeDelta;
			Font f = GetFontCore();
			this.deltaSourceFont = f;
			this.deltaBasedFont = new Font(f.FontFamily, GetSize(f.Size), f.Style | FontStyleDelta, f.Unit, f.GdiCharSet, f.GdiVerticalFont);
		}
		float GetSize(float source) {
			if(FontSizeDelta > 100) return source * ((FontSizeDelta - 100) / 10f);
			if(FontSizeDelta < -100) return source * ((FontSizeDelta + 100) / 10f);
			return source + FontSizeDelta;
		}
		public virtual void Assign(AppearanceObject val) {
			if(val == null) return;
			BeginUpdate();
			try {
				DestroyBrush();
				if(val.options != null) {
					Options.Assign(val.Options);
				} else
					if(options != null)
						Options.Reset();
				this.foreColor = val.ForeColor;
				this.borderColor = val.BorderColor;
				this.backColor = val.BackColor;
				this.backColor2 = val.BackColor2;
				this.gradientMode = val.GradientMode;
				this.image = val.Image;
				this.fontSizeDelta = val.FontSizeDelta;
				this.fontStyleDelta = val.FontStyleDelta;
				this.deltaBasedFont = val.deltaBasedFont;
				this.deltaBasedSize = val.deltaBasedSize;
				this.deltaSourceFont = val.deltaSourceFont;
				this.deltaBasedStyle = val.deltaBasedStyle;
				SetFont(val._font);
				if(val.textOptions != null)
					TextOptions.Assign(val.TextOptions);
				else
					if(textOptions != null)
						TextOptions.Reset();
				OnFontSizeDeltaChanged(true);
			}
			finally {
				EndUpdate();
			}
		}
		internal Font CheckFont(Font font) {
			if(font != null) {
				if(object.ReferenceEquals(InnerDefaultFont, font)) font = null;
				else {
					if(font.Equals(InnerDefaultFont)) font = null;
				}
			}
			return font;
		}
		internal void SetFont(Font font) { SetFont(font, false); }
		internal void SetFont(Font font, bool setUseFont) {
			this._font = CheckFont(font); 
			if(setUseFont && _font != null) Options.UseFont = true;
		}
		public virtual bool IsEqual(AppearanceObject val) {
			return (val.options ?? AppearanceOptions.Empty).IsEqual(options ?? AppearanceOptions.Empty) &&
				this.foreColor == val.ForeColor &&
				this.borderColor == val.BorderColor &&
				this.backColor == val.BackColor &&
				this.backColor2 == val.BackColor2 &&
				this.gradientMode == val.GradientMode &&
				this.image == val.Image &&
				this.Font == val.Font &&
				(val.textOptions ?? TextOptions.DefaultOptions).IsEqual(textOptions ?? TextOptions.DefaultOptions);
		}
		public virtual void BeginUpdate() {
			this.lockUpdate ++;
			if(options != null)
				Options.BeginUpdate();
		}
		public virtual void CancelUpdate() {
			if(options != null)
				Options.CancelUpdate();
			--this.lockUpdate;
		}
		public virtual void EndUpdate() {
			if(options != null)
				Options.EndUpdate();
			if(--this.lockUpdate == 0) {
				OnSizeChanged();
			}
		}
		protected internal void RotateBackColors(int angle) {
			if(angle == 0 || BackColor2 == Color.Empty) return;
			if(angle == 180) {
				Color clr = BackColor2;
				this.backColor2 = BackColor;
				this.backColor = clr;
				return;
			}
			if(angle == 90 || angle == 270) {
				LinearGradientMode gmode = GradientMode;
				switch(gmode) {
					case LinearGradientMode.Vertical : gmode = LinearGradientMode.Horizontal; break;
					case LinearGradientMode.ForwardDiagonal : gmode = LinearGradientMode.BackwardDiagonal; break;
					case LinearGradientMode.Horizontal : gmode = LinearGradientMode.Vertical; break;
					case LinearGradientMode.BackwardDiagonal : gmode = LinearGradientMode.ForwardDiagonal; break;
				}
				this.gradientMode = gmode;
			}
		}
		protected virtual void OnOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			OnSizeChanged();
		}
		protected internal virtual void OnTextOptionsChanged() {
			if(!IsLoading && this.lockUpdate == 0) {
				try { Options.BeginUpdate(); Options.UseTextOptions = !TextOptions.IsDefault(); } finally { Options.CancelUpdate(); }
			}
			OnSizeChanged();
		}
		protected internal virtual void OnPaintChanged() {
			if(this.lockUpdate != 0) return;
			if(PaintChanged != null) PaintChanged(this, EventArgs.Empty);
			OnChanged();
		}
		protected internal virtual void OnSizeChanged() {
			if(this.lockUpdate != 0) return;
			if(SizeChanged != null) SizeChanged(this, EventArgs.Empty);
			OnChanged();
		}
		protected internal virtual void OnChanged() {
			if(this.lockUpdate != 0) return;
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppearanceObject GetAppearanceByFont() {
			return GetAppearanceByOption(optUseFont);
		}
		public TextOptions GetTextOptions() {
			return GetAppearanceByOption(optUseTextOptions).TextOptions;
		}
		public Color GetForeColor() {
			return GetAppearanceByOption(optUseForeColor).ForeColor;
		}
		public Color GetBorderColor() {
			return GetAppearanceByOption(optUseBorderColor).BorderColor;
		}
		public Color GetBackColor() {
			return GetAppearanceByOption(optUseBackColor).BackColor;
		}
		public Color GetBackColor2() {
			return GetAppearanceByOption(optUseBackColor).BackColor2;
		}
		public LinearGradientMode GetGradientMode() {
			return GetAppearanceByOption(optUseBackColor).GradientMode;
		}
		public Font GetFont() {
			return GetAppearanceByOption(optUseFont).Font;
		}
		public virtual Image GetImage() {
			return GetAppearanceByOption(optUseImage).Image;
		}
		public Brush GetForeBrush(GraphicsCache cache) {
			if(cache == null) return new SolidBrush(GetForeColor());
			return cache.GetSolidBrush(GetForeColor());
		}
		public Pen GetBorderPen(GraphicsCache cache) {
			if(cache == null) return new Pen(GetBorderColor());
			return cache.GetPen(GetBorderColor());
		}
		public Pen GetBackPen(GraphicsCache cache) {
			if(cache == null) return new Pen(GetBackColor());
			return cache.GetPen(GetBackColor());
		}
		public Brush GetBorderBrush(GraphicsCache cache) {
			if(cache == null) return new SolidBrush(GetBorderColor());
			return cache.GetSolidBrush(GetBorderColor());
		}
		public Brush GetBackBrush(GraphicsCache cache) {
			if(cache == null) return new SolidBrush(GetBackColor());
			return cache.GetSolidBrush(GetBackColor());
		}
		public Brush GetBackBrush(GraphicsCache cache, Rectangle rect) {
			Color c1 = GetBackColor(), c2 = GetBackColor2();
			if(c1 == c2 || c2 == Color.Empty) return GetBackBrush(cache);
			if(cache == null) {
				return new LinearGradientBrush(rect, c1, c2, GetGradientMode());
			}
			return cache.GetGradientBrush(rect, c1, c2, GetGradientMode());
		}
		public Pen GetForePen(GraphicsCache cache) {
			if(cache == null) return new Pen(GetForeColor());
			return cache.GetPen(GetForeColor());
		}
		public TextureBrush GetTextureBrush() {
			AppearanceObject app = GetAppearanceByOption(optUseImage);
			if(app.textureBrush != null) return app.textureBrush;
			if(app.Image == null) return null;
			app.textureBrush = new TextureBrush(app.Image);
			return app.textureBrush;
		}
		public bool ShouldSerialize() {
			PropertyDescriptorCollection pdColl = TypeDescriptor.GetProperties(this);
			foreach(PropertyDescriptor pd in pdColl) {
				if(pd.SerializationVisibility != DesignerSerializationVisibility.Hidden && pd.ShouldSerializeValue(this)) return true;
			}
			return false;
		}
		public void DrawString(GraphicsCache cache, string text, Rectangle bounds) {
			Brush brush = cache.GetSolidBrush(GetForeColor());
			DrawString(cache, text, bounds, brush);
		}
		public void DrawString(GraphicsCache cache, string text, Rectangle bounds, Brush foreBrush) {
			StringFormat format = GetTextOptions().GetStringFormat();
			DrawString(cache, text, bounds, foreBrush, format);
		}
		public void DrawString(GraphicsCache cache, string text, Rectangle bounds, StringFormat format) {
			DrawString(cache, text, bounds, cache.GetSolidBrush(GetForeColor()), format);
		}
		public void DrawString(GraphicsCache cache, string text, Rectangle bounds, Font font, StringFormat format) {
			cache.DrawString(text, font, cache.GetSolidBrush(GetForeColor()), bounds, format);
		}
		public virtual void DrawString(GraphicsCache cache, string text, Rectangle bounds, Brush foreBrush, StringFormat format) {
			cache.DrawString(text, GetFont(), foreBrush, bounds, format);
		}
		public void DrawString(GraphicsCache cache, string s, Rectangle bounds, Font font, Brush foreBrush, StringFormat strFormat) {
			cache.DrawString(s, font, foreBrush, bounds, strFormat);
		}
		public void DrawVString(GraphicsCache cache, string text, Font font, Brush foreBrush, Rectangle bounds, StringFormat strFormat, int angle) {
			cache.DrawVString(text, font, foreBrush, bounds, strFormat, angle);
		}
		public void FillRectangle(GraphicsCache cache, Rectangle bounds) {
			DrawBackground(cache, bounds);
		}
		public void DrawBackground(GraphicsCache cache, Rectangle bounds) {
			DrawBackground(cache.Graphics, cache, bounds);
		}
		public void FillRectangle(GraphicsCache cache, Rectangle bounds, bool useZeroOffset) {
			DrawBackground(cache, bounds, useZeroOffset);
		}
		public void DrawBackground(GraphicsCache cache, Rectangle bounds, bool useZeroOffset) {
			DrawBackground(cache.Graphics, cache, bounds, useZeroOffset);
		}
		public void DrawBackground(Graphics graphics, GraphicsCache cache, Rectangle bounds) {
			DrawBackground(graphics, cache, bounds, false);
		}
		public virtual void DrawBackground(Graphics graphics, GraphicsCache cache, Rectangle bounds, bool useZeroOffset) {
			Color clr = GetBackColor();
			bool isEmpty = clr.IsEmpty;
			if(SystemInformation.HighContrast) {
				if(!isEmpty) {
					if(cache == null)
						graphics.FillRectangle(GetBackBrush(cache, bounds), bounds);
					else
						cache.FillRectangle(GetBackBrush(cache, bounds), bounds);
				}
				return;
			}
			TextureBrush brush = GetTextureBrush();
			if(brush != null && useZeroOffset) brush.TranslateTransform(bounds.X, bounds.Y);
			if(cache == null) {
				if(clr.A != 255 || isEmpty) {
					if(brush != null) graphics.FillRectangle(brush, bounds);
					if(!isEmpty) graphics.FillRectangle(GetBackBrush(cache, bounds), bounds);
				} else {
					graphics.FillRectangle(GetBackBrush(cache, bounds), bounds);
					if(brush != null) graphics.FillRectangle(brush, bounds);
				}
			} else {
				if(clr.A != 255 || isEmpty) {
					if(brush != null) cache.FillRectangle(brush, bounds);
					if(!isEmpty) cache.FillRectangle(GetBackBrush(cache, bounds), bounds);
				} else {
					cache.FillRectangle(GetBackBrush(cache, bounds), bounds);
					if(brush != null) cache.FillRectangle(brush, bounds);
				}
			}
			if(brush != null && useZeroOffset) brush.ResetTransform();
		}
		public virtual void Assign(AppearanceDefault appearanceDefault) {
			if(appearanceDefault == null) return;
			BeginUpdate();
			try {
				Reset();
				this.backColor = appearanceDefault.BackColor;
				this.foreColor = appearanceDefault.ForeColor;
				this.borderColor = appearanceDefault.BorderColor;
				this.backColor2 = appearanceDefault.BackColor2;
				this.gradientMode = appearanceDefault.GradientMode;
				this.FontSizeDelta = appearanceDefault.FontSizeDelta;
				this.FontStyleDelta = appearanceDefault.FontStyleDelta;
				SetFont((appearanceDefault.Font == null ? this._font : appearanceDefault.Font));
				this.TextOptions.Assign(appearanceDefault);
				Options.UseBackColor = (this.backColor != Color.Empty);
				Options.UseBorderColor = (this.borderColor != Color.Empty);
				Options.UseForeColor = (this.foreColor != Color.Empty);
				Options.UseFont = _font != null;
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void Reset() {
			BeginUpdate();
			try {
				if(textOptions != null)
					TextOptions.Reset();
				if(options != null)
					Options.ResetOptions();
				this.backColor = Color.Empty;
				this.backColor2 = Color.Empty;
				this.foreColor = Color.Empty;
				this.borderColor = Color.Empty;
				this.fontSizeDelta = 0;
				this.fontStyleDelta = FontStyle.Regular;
				this.deltaSourceFont = null;
				this.deltaBasedFont = null;
				this.deltaBasedSize = 0;
				this.deltaBasedStyle = FontStyle.Regular;
				this._font = null;
				this.gradientMode = LinearGradientMode.Horizontal;
				this.image = null;
				DestroyBrush();
				OnFontSizeDeltaChanged(true);
			}
			finally {
				EndUpdate();
			}
		}
		internal static void ResetDefaultFont() {  defaultFont = null; }
		static Font defaultFont;
#if !SL
	[DevExpressUtilsLocalizedDescription("AppearanceObjectDefaultFont")]
#endif
		public static Font DefaultFont {
			get {
				if(defaultFont == null) {
					FontBehaviorHelper.CheckFont();
					defaultFont = CreateDefaultFont();
				}
				return defaultFont;
			}
			set {
				FontBehaviorHelper.CheckFont();
				if(value == defaultFont) return;
				if(value == null) value = CreateDefaultFont();
				defaultFont = value;
				DevExpress.LookAndFeel.UserLookAndFeel.Default.OnStyleChanged();
			}
		}
		static Font defaultMenuFont;
#if !SL
	[DevExpressUtilsLocalizedDescription("AppearanceObjectDefaultMenuFont")]
#endif
		public static Font DefaultMenuFont {
			get {
				if(defaultMenuFont == null) defaultMenuFont = SystemInformation.MenuFont;
				return defaultMenuFont;
			}
			set {
				if(value == null) value = SystemInformation.MenuFont;
				defaultMenuFont = value;
				DevExpress.LookAndFeel.UserLookAndFeel.Default.OnStyleChanged();
			}
		}
		internal static Font CreateDefaultFont() {
			try {
				return FontBehaviorHelper.GetDefaultFont();
			}
			catch { }
			return Control.DefaultFont;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppearanceObject GetAppearanceByOption(string option) {
			return GetAppearanceByOption(option, 0, null);
		}
		protected internal AppearanceObject GetAppearanceByOption(string option, AppearanceObject defaultAppearance) {
			return GetAppearanceByOption(option, 0, defaultAppearance);
		}
		protected AppearanceObject GetAppearanceByOption(string option, int level, AppearanceObject defaultAppearance) {
			bool optValue = options == null ? false : Options.GetOptionValue(option);
			if(optValue || level > 10) return this;
			if(ParentAppearance == null) {
				if(defaultAppearance != null) return defaultAppearance.GetAppearanceByOption(option, 0, null);
				return this;
			}
			return ParentAppearance.GetAppearanceByOption(option, level + 1, defaultAppearance);
		}
		protected internal bool GetOptionState(string option) { return GetOptionState(option, 0, null); }
		protected internal bool GetOptionState(string option, int level, AppearanceObject defaultAppearance) {
			bool optValue = options == null ? false : Options.GetOptionValue(option);
			if(optValue) return true;
			if(level > 10) return false;
			if(ParentAppearance == null) {
				if(defaultAppearance != null) return defaultAppearance.GetOptionState(option, 0, null);
				return false;
			}
			return ParentAppearance.GetOptionState(option, level + 1, defaultAppearance);
		}
		protected virtual AppearanceOptions CreateOptions() { return new AppearanceOptions(); }
		protected internal virtual void DestroyBrush() {
			if(this.textureBrush != null) {
				this.textureBrush.Dispose();
				this.textureBrush = null;
			}
		}
		#region Font Defaults
		[Browsable(false)]
		public int FontHeight {
			get {
				long hashCode = GetFontHashCode(Font);
				object height = FontHeights[hashCode];
				if(height == null) {
					height = Font.Height;
					FontHeights[hashCode] = height;
				}
				return (int)height;
			}
		}
		[ThreadStatic]
		static Hashtable fontSizes;
		protected static Hashtable FontSizes {
			get { 
				if(fontSizes == null) fontSizes = new Hashtable();
				return fontSizes; 
			}
		}
		[ThreadStatic]
		static Hashtable fontHeights;
		protected static Hashtable FontHeights {
			get { 
				if(fontHeights == null) fontHeights = new Hashtable();
				return fontHeights; 
			}
		}
		internal static long GetFontHashCode(Font font) {
			return AppearanceHelper.GetFontHashCode(font);
		}
		public Size CalcDefaultTextSize() {
			Graphics g = GraphicsInfo.Default.AddGraphics(null);
			try {
				return CalcDefaultTextSize(g);
			}
			finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
		}
		public virtual Size CalcDefaultTextSize(Graphics g) {
			long hashCode = GetFontHashCode(Font);
			object defSize = FontSizes[hashCode];
			bool pageUnitDisplay = g.PageUnit == GraphicsUnit.Display;
			if (defSize == null || !pageUnitDisplay) {
				SizeF sizef = CalcTextSize(g, TextOptions.DefaultStringFormat, "Wg", 0);
				Size size = XPaint.TextSizeRound(sizef);
				if (!(XPaint.Graphics is XPaintMixed)) size.Height = Math.Max(FontHeight, size.Height);
				defSize = size;
				if (pageUnitDisplay) FontSizes[hashCode] = defSize;
			}
			return (Size)defSize;
		}
		public Size CalcTextSizeInt(GraphicsCache cache, string s, int width) {
			return CalcTextSizeInt(cache, GetTextOptions().GetStringFormat(), s, width);
		}
		public Size CalcTextSizeInt(GraphicsCache cache, StringFormat sf, string s, int width) {
			return cache.Paint.CalcTextSizeInt(cache.Graphics, s, Font, sf, width);
		}
		public Size CalcTextSizeInt(Graphics g, string s, int width) {
			return CalcTextSizeInt(g, GetTextOptions().GetStringFormat(), s, width);
		}
		public Size CalcTextSizeInt(Graphics g, StringFormat sf, string s, int width) {
			return DevExpress.Utils.Paint.XPaint.Graphics.CalcTextSizeInt(g, s, Font, sf, width);
		}
		public SizeF CalcTextSize(GraphicsCache cache, string s, int width) {
			return CalcTextSize(cache, GetTextOptions().GetStringFormat(), s, width);
		}
		public SizeF CalcTextSize(GraphicsCache cache, StringFormat sf, string s, int width) {
			return cache.Paint.CalcTextSize(cache.Graphics, s, Font, sf, width);
		}
		public SizeF CalcTextSize(Graphics g, string s, int width) {
			return CalcTextSize(g, GetTextOptions().GetStringFormat(), s, width);
		}
		public SizeF CalcTextSize(Graphics g, StringFormat sf, string s, int width) {
			return DevExpress.Utils.Paint.XPaint.Graphics.CalcTextSize(g, s, Font, sf, width);
		}
		public SizeF CalcTextSize(Graphics g, StringFormat sf, string s, int width, int height) {
			return DevExpress.Utils.Paint.XPaint.Graphics.CalcTextSize(g, s, Font, sf, width, height);
		}
		public SizeF CalcTextSize(Graphics g, StringFormat sf, string s, int width, int height, out bool isCropped) {
			return DevExpress.Utils.Paint.XPaint.Graphics.CalcTextSize(g, s, Font, sf, width, height, out isCropped);
		}
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return true;
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			BeginUpdate();
			try {
				Reset();
			}
			catch { }
			CancelUpdate();
		}
		int serializing = 0, deserializing = 0;
		void IXtraSerializable.OnStartSerializing() { serializing++; }
		void IXtraSerializable.OnEndSerializing() { serializing--; }
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) { deserializing++; }
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			deserializing--;
		}
		#endregion
	}
	public class AppearanceHelper {
		public static Color RemoveTransparency(Color color, Color defaultColor) {
			if(color.A == 255 || color.IsEmpty) return color;
			if(color != Color.Transparent) {
				color = Color.FromArgb(255, color);
			}
			if(color == Color.Transparent) color = defaultColor;
			return color;
		}
		public static BrickStyle CreateBrick(AppearanceObject appearance, BorderSide borderSide, Color borderColor, int borderWidth) {
			if(borderColor == Color.Empty) borderColor = Color.Gray;
			BrickStyle brick = new BrickStyle(borderSide, borderWidth, borderColor, appearance.GetBackColor(), 
				appearance.GetForeColor(), appearance.GetFont(), new BrickStringFormat(appearance.GetStringFormat()));
			return brick;
		}
		public static BrickStyle CreateBrick(AppearanceObject appearance) {
			return CreateBrick(appearance, BorderSide.None, Color.Transparent, 1);
		}
		public static long GetFontHashCode(Font font) {
			return FontHelper.GetFontHashCode(font);
		}
		public static long GetTextOptionsHashCode(TextOptions options, TextOptions defaultOptions) {
			if(defaultOptions == null) defaultOptions = TextOptions.DefaultOptions;
			int code = (GetInt((int)options.GetHAlignment(defaultOptions.HAlignment))) | 
				((GetInt((int)options.GetVAlignment(defaultOptions.VAlignment))) << 4) | 
				((GetInt((int)(options.Trimming == Trimming.Default ? defaultOptions.Trimming : options.Trimming))) << 8) |  
				((GetInt((int)(options.WordWrap == WordWrap.Default ? defaultOptions.WordWrap : options.WordWrap))) << 16);
			return code;
		}
		static int GetInt(int v) { return v + 1; }
		public static void Combine(AppearanceObject target, AppearanceObject first, AppearanceObject second, AppearanceObject defaultAppearance) {
			AppearanceObject temp = new AppearanceObject(second, defaultAppearance);
			Combine(target, first, temp);
			temp.Dispose();
		}
		public static void Combine(AppearanceObject target, AppearanceObject main, AppearanceObject defaultAppearance) {
			Combine(target, main, defaultAppearance, true);
		}
		public static void Combine(AppearanceObject target, AppearanceObject[] sources, AppearanceDefault appearanceDefault) {
			Combine(target, sources, appearanceDefault, true);
		}
		public static void Combine(AppearanceObject target, AppearanceObject[] sources, AppearanceDefault appearanceDefault, bool setdefaultUseFlag) {
			target.BeginUpdate();
			try {
				Combine(target, sources);
				Combine(target, appearanceDefault, setdefaultUseFlag);
			} finally {
				target.EndUpdate();
			}
		}
		protected static void Combine(AppearanceObject target, AppearanceObject[] sources, bool useReset) {
			target.BeginUpdate();
			try {
				if(useReset) target.Reset();
				target.foreColor = GetAppearanceByOption(target, AppearanceObject.optUseForeColor, sources).ForeColor;
				target.backColor = GetAppearanceByOption(target, AppearanceObject.optUseBackColor, sources).BackColor;
				target.backColor2 = GetAppearanceByOption(target, AppearanceObject.optUseBackColor, sources).BackColor2;
				target.borderColor = GetAppearanceByOption(target, AppearanceObject.optUseBorderColor, sources).BorderColor;
				target.gradientMode = GetAppearanceByOption(target, AppearanceObject.optUseBackColor, sources).GradientMode;
				target.SetFont(GetAppearanceByOption(target, AppearanceObject.optUseFont, sources).Font);
				target.image = GetAppearanceByOption(target, AppearanceObject.optUseImage, sources).Image;
				target.TextOptions.Assign(GetAppearanceByOption(target, AppearanceObject.optUseTextOptions, sources).TextOptions);
				target.TextOptions.RightToLeft = target.TextOptions.RightToLeft | IsRightToLeft(sources);
				bool option, nonEmpty = target.options != null;
				option = GetOptionState(AppearanceObject.optUseTextOptions, sources);
				if(option || nonEmpty)
					target.Options.UseTextOptions = option;
				option = GetOptionState(AppearanceObject.optUseForeColor, sources);
				if(option || nonEmpty)
					target.Options.UseForeColor = option;
				option = GetOptionState(AppearanceObject.optUseBackColor, sources);
				if(option || nonEmpty)
					target.Options.UseBackColor = option;
				option = GetOptionState(AppearanceObject.optUseBorderColor, sources);
				if(option || nonEmpty)
					target.Options.UseBorderColor = option;
				option = GetOptionState(AppearanceObject.optUseImage, sources);
				if(option || nonEmpty)
					target.Options.UseImage = option;
				option = GetOptionState(AppearanceObject.optUseFont, sources);
				if(option || nonEmpty)
					target.Options.UseFont = option;
			} finally {
				target.EndUpdate();
			}
		}
		static bool IsRightToLeft(AppearanceObject[] sources) {
			if(sources == null) return false;
			for(int n = 0; n < sources.Length; n++) {
				var source = sources[n];
				if(source != null && source.TextOptions.RightToLeft) return true;
			}
			return false;
		}
		public static void Combine(AppearanceObject target, AppearanceObject[] sources) {
			Combine(target, sources, true);
		}
		public static void Apply(AppearanceObject target, AppearanceObject appearance) {
			Combine(target, new AppearanceObject[]  { appearance, target }, false);
		}
		public static void Apply(AppearanceObject target, AppearanceDefault appearanceDefault) {
			if(appearanceDefault == null) return;
			target.BeginUpdate();
			try {
				if(appearanceDefault.ForeColor != Color.Empty) {
					target.foreColor = appearanceDefault.ForeColor;
					target.Options.UseForeColor = true;
				}
				if(appearanceDefault.BackColor != Color.Empty) {
					target.backColor = appearanceDefault.BackColor;
					target.backColor2 = appearanceDefault.BackColor2;
					target.gradientMode = appearanceDefault.GradientMode;
					target.Options.UseBackColor = true;
				}
				if(appearanceDefault.BorderColor != Color.Empty) {
					target.borderColor = appearanceDefault.BorderColor;
					target.Options.UseBorderColor = true;
				}
				if(appearanceDefault.Font != null) {
					target.SetFont(appearanceDefault.Font, true);
				}
				if(appearanceDefault.HAlignment != HorzAlignment.Default)
					target.TextOptions.HAlignment = appearanceDefault.HAlignment;
				if(appearanceDefault.VAlignment != VertAlignment.Default)
					target.TextOptions.VAlignment = appearanceDefault.VAlignment;
				AppearanceObjectEx targetEx = target as AppearanceObjectEx;
				if(targetEx != null && appearanceDefault.HighPriority != DefaultBoolean.Default)
					targetEx.Options.HighPriority = appearanceDefault.HighPriority == DefaultBoolean.True;
			}
			finally {
				target.EndUpdate();
			}
		}
		protected static void Combine(AppearanceObject target, AppearanceDefault appearanceDefault, bool setUseFlag) {
			if(appearanceDefault == null) return;
			target.BeginUpdate();
			try {
				if(!target.GetOptionState(AppearanceObject.optUseForeColor) && appearanceDefault.ForeColor != Color.Empty) {
					target.foreColor = appearanceDefault.ForeColor;
					if(setUseFlag)
						target.Options.UseForeColor = true;
				}
				if(!target.GetOptionState(AppearanceObject.optUseBackColor) && appearanceDefault.BackColor != Color.Empty) {
					target.backColor = appearanceDefault.BackColor;
					target.backColor2 = appearanceDefault.BackColor2;
					target.gradientMode = appearanceDefault.GradientMode;
					if(setUseFlag)
						target.Options.UseBackColor = true;
				}
				if(!target.GetOptionState(AppearanceObject.optUseBorderColor) && appearanceDefault.BorderColor != Color.Empty) {
					target.borderColor = appearanceDefault.BorderColor;
					if(setUseFlag)
						target.Options.UseBorderColor = true;
				}
				if(!target.GetOptionState(AppearanceObject.optUseFont)) {
					if(appearanceDefault.Font != null) {
						target.SetFont(appearanceDefault.Font, true);
					}
					if(appearanceDefault.FontSizeDelta != 0) {
						target.FontSizeDelta = appearanceDefault.FontSizeDelta;
						target.Options.UseFont = true;
					}
					if(appearanceDefault.FontStyleDelta != FontStyle.Regular) {
						target.FontStyleDelta = appearanceDefault.FontStyleDelta;
						target.Options.UseFont = true;
					}
				}
				bool useTextOptions = target.GetOptionState(AppearanceObject.optUseTextOptions);
				if(!useTextOptions || target.TextOptions.HAlignment == HorzAlignment.Default)
					target.TextOptions.HAlignment = appearanceDefault.HAlignment;
				if(!useTextOptions || target.TextOptions.VAlignment == VertAlignment.Default)
					target.TextOptions.VAlignment = appearanceDefault.VAlignment;
			}
			finally {
				target.EndUpdate();
			}
		}
		protected static AppearanceObject GetAppearanceByOption(AppearanceObject defaultAppearance, string option, AppearanceObject[] sources) {
			int count = sources.Length;
			AppearanceObject res = null;
			for(int n = 0; n < count; n++) {
				AppearanceObject current = sources[n];
				if(current == null) continue;
				bool optValue = current.options == null ? false : current.Options.GetOptionValue(option);
				if(optValue) return current;
				if(current.ParentAppearance == null) {
					res = current;
					continue;
				} else {
					if(current.ParentAppearance.GetOptionState(option)) 
						return current.ParentAppearance.GetAppearanceByOption(option);
				}
			}
			return res == null ? defaultAppearance : res;
		}
		protected static bool GetOptionState(string option, AppearanceObject[] sources) {
			int count = sources.Length;
			for(int n = 0; n < count; n++) {
				AppearanceObject current = sources[n];
				if(current == null) continue;
				bool optValue = current.options == null ? false : current.Options.GetOptionValue(option);
				if(optValue) return true;
				if(current.ParentAppearance != null) {
					if(current.ParentAppearance.GetOptionState(option)) return true;
				}
			}
			return false;
		}
		protected static void Combine(AppearanceObject target, AppearanceObject main, AppearanceObject defaultAppearance, bool reset) {
			target.BeginUpdate();
			try {
				target.DestroyBrush();
				if(reset) target.Options.ResetOptions();
				target.foreColor = main.GetAppearanceByOption(AppearanceObject.optUseForeColor, defaultAppearance).ForeColor;
				target.backColor = main.GetAppearanceByOption(AppearanceObject.optUseBackColor, defaultAppearance).BackColor;
				target.backColor2 = main.GetAppearanceByOption(AppearanceObject.optUseBackColor, defaultAppearance).BackColor2;
				target.borderColor = main.GetAppearanceByOption(AppearanceObject.optUseBorderColor, defaultAppearance).BorderColor;
				target.gradientMode = main.GetAppearanceByOption(AppearanceObject.optUseBackColor, defaultAppearance).GradientMode;
				target.SetFont(main.GetAppearanceByOption(AppearanceObject.optUseFont, defaultAppearance).Font);
				target.image = main.GetAppearanceByOption(AppearanceObject.optUseImage, defaultAppearance).Image;
				target.TextOptions.Assign(main.GetAppearanceByOption(AppearanceObject.optUseTextOptions, defaultAppearance).TextOptions);
				bool option, nonEmpty = target.options != null;
				option = main.GetOptionState(AppearanceObject.optUseTextOptions, 0, defaultAppearance);
				if(option || nonEmpty)
					target.Options.UseTextOptions = option;
				option = main.GetOptionState(AppearanceObject.optUseForeColor, 0, defaultAppearance);
				if(option || nonEmpty)
					target.Options.UseForeColor = option;
				option = main.GetOptionState(AppearanceObject.optUseBackColor, 0, defaultAppearance);
				if(option || nonEmpty)
					target.Options.UseBackColor = option;
				option = main.GetOptionState(AppearanceObject.optUseBorderColor, 0, defaultAppearance);
				if(option || nonEmpty)
					target.Options.UseBorderColor = option;
				option = main.GetOptionState(AppearanceObject.optUseImage, 0, defaultAppearance);
				if(option || nonEmpty)
					target.Options.UseImage = option;
				option = main.GetOptionState(AppearanceObject.optUseFont, 0, defaultAppearance);
				if(option || nonEmpty)
					target.Options.UseFont = option;
			}
			finally {
				target.EndUpdate();
			}
		}
		protected internal static void Combine(AppearanceObject target, AppearanceObject main, AppearanceDefault appearanceDefault) {
			Combine(target, main, appearanceDefault, false);
		}
		protected static void Combine(AppearanceObject target, AppearanceObject main, AppearanceObject controller, AppearanceDefault appearanceDefault) {
			Combine(target, main, controller, appearanceDefault, true);
		}
		protected static void Combine(AppearanceObject target, AppearanceObject main, AppearanceObject controller, AppearanceDefault appearanceDefault, bool reset) {
			target.BeginUpdate();
			try {
				if(reset)
					target.Reset();
				Combine(target, main, controller);
				Combine(target, appearanceDefault, true);
			}
			finally {
				target.EndUpdate();
			}
		}
		protected static void Combine(AppearanceObject target, AppearanceObject main, AppearanceDefault appearanceDefault, bool reset) {
			if(appearanceDefault == null) return;
			target.BeginUpdate();
			try {
				if(main != target) {
					if(reset) 
						target.Reset();
					else
						target.Options.ResetOptions();
				}
				bool option = false;
				if((option = main.GetOptionState(AppearanceObject.optUseForeColor)) || appearanceDefault.ForeColor == Color.Empty) {
					target.foreColor = main.ForeColor;
					target.Options.UseForeColor = option;
				} else {
					target.foreColor = appearanceDefault.ForeColor;
					target.Options.UseForeColor = true;
				}
				if((option = main.GetOptionState(AppearanceObject.optUseBackColor)) || appearanceDefault.BackColor == Color.Empty) {
					target.backColor = main.BackColor;
					target.backColor2 = main.BackColor2;
					target.gradientMode = main.GradientMode;
					target.Options.UseBackColor = option;
				} else {
					target.backColor = appearanceDefault.BackColor;
					target.backColor2 = appearanceDefault.BackColor2;
					target.gradientMode = appearanceDefault.GradientMode;
					target.Options.UseBackColor = true;
				}
				if((option = main.GetOptionState(AppearanceObject.optUseBorderColor)) || appearanceDefault.BorderColor == Color.Empty) {
					target.borderColor = main.BorderColor;
					target.Options.UseBorderColor = option;
				} else {
					target.borderColor = appearanceDefault.BorderColor;
					target.Options.UseBorderColor = true;
				}
				if((option = main.GetOptionState(AppearanceObject.optUseFont)) || appearanceDefault.Font == null) {
					target.SetFont(main.Font, option);
				} else {
					target.SetFont(appearanceDefault.Font, true); ;
				}
				Image oldImage = target.image;
				target.image = main.GetAppearanceByOption(AppearanceObject.optUseImage, target).Image;
				target.Options.UseImage = main.GetOptionState(AppearanceObject.optUseImage);
				if(oldImage != target.image) target.DestroyBrush();
				bool useTextOptions = main.GetOptionState(AppearanceObject.optUseTextOptions);
				if(useTextOptions) {
					target.TextOptions.Assign(main.GetTextOptions());
					target.Options.UseTextOptions = true;
				}
				if(!useTextOptions || main.TextOptions.HAlignment == HorzAlignment.Default)
					target.TextOptions.HAlignment = appearanceDefault.HAlignment;
				if(!useTextOptions || main.TextOptions.VAlignment == VertAlignment.Default)
					target.TextOptions.VAlignment = appearanceDefault.VAlignment;
			}
			finally {
				target.EndUpdate();
			}
		}
	}
	public class AppearanceOptions : BaseOptions {
		bool useForeColor, useBackColor, useImage, useFont, useBorderColor, useTextOptions;
		public AppearanceOptions() {
		}
		protected internal virtual new void OnChanged(BaseOptionChangedEventArgs e) {
			base.OnChanged(e);
		}
		protected internal virtual void AddChangedCoreHandler(BaseOptionChangedEventHandler handler) {
			ChangedCore += handler;
		}
		protected internal virtual void RemoveChangedCoreHandler(BaseOptionChangedEventHandler handler) {
			ChangedCore -= handler;
		}
		protected internal virtual void ResetOptions() {
			this.useTextOptions = false;
			this.useBorderColor = false;
			this.useForeColor = false;
			this.useBackColor = false;
			this.useImage = false;
			this.useFont = false;
		}
		static AppearanceOptions empty = new AppearanceOptions();
#if !SL
	[DevExpressUtilsLocalizedDescription("AppearanceOptionsEmpty")]
#endif
		public static AppearanceOptions Empty {
			get {
				return empty;
			}
		}
		public virtual bool IsEqual(AppearanceOptions options) {
			return this.useTextOptions == options.useTextOptions &&
				this.useBorderColor == options.useBorderColor &&
				this.useForeColor == options.useForeColor &&
				this.useBackColor == options.useBackColor &&
				this.useImage == options.useImage &&
				this.useFont == options.useFont;
		}
		protected bool IsEqual(object x, object y) {
			return x == y;
		}
		protected internal virtual bool GetOptionValue(string name) {
			if(IsEqual(name, AppearanceObject.optUseBackColor)) return UseBackColor;
			if(IsEqual(name, AppearanceObject.optUseTextOptions)) return UseTextOptions;
			if(IsEqual(name, AppearanceObject.optUseForeColor)) return UseForeColor;
			if(IsEqual(name, AppearanceObject.optUseBorderColor)) return UseBorderColor;
			if(IsEqual(name, AppearanceObject.optUseImage)) return UseImage;
			if(IsEqual(name, AppearanceObject.optUseFont)) return UseFont;
			throw new ArgumentException("Wrong option name", name);
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceOptionsUseForeColor"),
#endif
 DefaultValue(false), XtraSerializableProperty(),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceOptions.UseForeColor"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseForeColor {
			get { return useForeColor; }
			set {
				if(UseForeColor == value) return;
				bool prevValue = UseForeColor;
				useForeColor = value;
				OnChanged(AppearanceObject.optUseForeColor, prevValue, UseForeColor);
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceOptionsUseTextOptions"),
#endif
 DefaultValue(false), XtraSerializableProperty(),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceOptions.UseTextOptions"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseTextOptions {
			get { return useTextOptions; }
			set {
				if(UseTextOptions == value) return;
				bool prevValue = UseTextOptions;
				useTextOptions = value;
				OnChanged(AppearanceObject.optUseTextOptions, prevValue, UseTextOptions);
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceOptionsUseBorderColor"),
#endif
 DefaultValue(false), XtraSerializableProperty(),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceOptions.UseBorderColor"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseBorderColor {
			get { return useBorderColor; }
			set {
				if(UseBorderColor == value) return;
				bool prevValue = UseBorderColor;
				useBorderColor = value;
				OnChanged(AppearanceObject.optUseBorderColor, prevValue, UseBorderColor);
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceOptionsUseBackColor"),
#endif
 DefaultValue(false), XtraSerializableProperty(),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceOptions.UseBackColor"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseBackColor {
			get { return useBackColor; }
			set {
				if(UseBackColor == value) return;
				bool prevValue = UseBackColor;
				useBackColor = value;
				OnChanged(AppearanceObject.optUseBackColor, prevValue, UseBackColor);
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceOptionsUseImage"),
#endif
 DefaultValue(false), XtraSerializableProperty(), RefreshProperties(RefreshProperties.All),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceOptions.UseImage"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseImage {
			get { return useImage; }
			set {
				if(UseImage == value) return;
				bool prevValue = UseImage;
				useImage = value;
				OnChanged(AppearanceObject.optUseImage, prevValue, UseImage);
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AppearanceOptionsUseFont"),
#endif
 DefaultValue(false), XtraSerializableProperty(),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceOptions.UseFont"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseFont {
			get { return useFont; }
			set {
				if(UseFont == value) return;
				bool prevValue = UseFont;
				useFont = value;
				OnChanged(AppearanceObject.optUseFont, prevValue, UseFont);
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				AppearanceOptions opt = options as AppearanceOptions;
				if(opt == null) return;
				this.useForeColor = opt.UseForeColor;
				this.useBackColor = opt.UseBackColor;
				this.useBorderColor = opt.UseBorderColor;
				this.useTextOptions = opt.UseTextOptions;
				this.useFont = opt.UseFont;
				this.useImage = opt.UseImage;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal new bool ShouldSerialize() { return base.ShouldSerialize(); }
	}
	public class AppearanceDefault : ICloneable {
		#region static
		static AppearanceDefault empty;
		public static AppearanceDefault Empty { 
			get { 
				if(empty == null) empty = new AppearanceDefault();
				return empty;
			}
		}
		static AppearanceDefault control;
		public static AppearanceDefault Control { 
			get { 
				if(control == null) control = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
				return control;
			}
		}
		static AppearanceDefault window;
		public static AppearanceDefault Window { 
			get { 
				if(window == null) window = new AppearanceDefault(SystemColors.WindowText, SystemColors.Window);
				return window;
			}
		}
		#endregion
		Color foreColor, backColor, backColor2, borderColor;
		Font font;
		int fontSizeDelta;
		FontStyle fontStyleDelta = FontStyle.Regular;
		HorzAlignment hAlignment;
		VertAlignment vAlignment;
		LinearGradientMode gradientMode;
		DefaultBoolean highPriority = DefaultBoolean.Default;
		public AppearanceDefault(Color backColor) : this() {
			this.backColor = backColor;
		}
		public object Clone() {
			AppearanceDefault res = new AppearanceDefault();
			res.Assign(this);
			return res;
		}
		public virtual void Assign(AppearanceDefault source) {
			this.foreColor = source.foreColor;
			this.backColor = source.backColor;
			this.backColor2 = source.backColor2;
			this.borderColor = source.borderColor;
			this.font = source.font;
			this.hAlignment = source.hAlignment;
			this.vAlignment = source.vAlignment;
			this.gradientMode = source.gradientMode;
			this.fontSizeDelta = source.fontSizeDelta;
			this.fontStyleDelta = source.fontStyleDelta;
			this.highPriority = source.highPriority;
		}
		public DefaultBoolean HighPriority { get { return highPriority; } set { highPriority = value; } }
		public AppearanceDefault(Color foreColor, Color backColor) : this(backColor) {
			this.foreColor = foreColor;
		}
		public AppearanceDefault(Color foreColor, Color backColor, Font font) : this(foreColor, backColor) {
			this.font = font;
		}
		public AppearanceDefault(Color foreColor, Color backColor, Color borderColor) : this(foreColor, backColor) {
			this.borderColor = borderColor;
		}
		public AppearanceDefault(Color foreColor, Color backColor, Color borderColor, Font font) : this(foreColor, backColor, borderColor) {
			this.font = font;
		}
		public AppearanceDefault(Color foreColor, Color backColor, Color borderColor, Color backColor2) : this(foreColor, backColor, borderColor) {
			this.backColor2 = backColor2;
		}
		public AppearanceDefault(Color foreColor, Color backColor, Color borderColor, Color backColor2, LinearGradientMode gradient) : this(foreColor, backColor, borderColor, backColor2) {
			this.gradientMode = gradient;
		}
		public AppearanceDefault(Color foreColor, Color backColor, Color borderColor, Color backColor2, Font font) : this(foreColor, backColor, borderColor, backColor2) {
			this.font = font;
		}
		public AppearanceDefault(Color foreColor, Color backColor, Color borderColor, Color backColor2, Font font, HorzAlignment hAlignment, VertAlignment vAlignment) : this(foreColor, backColor, borderColor, backColor2, font) {
			this.hAlignment = hAlignment;
			this.vAlignment = vAlignment;
		}
		public AppearanceDefault(Color foreColor, Color backColor, Color borderColor, Color backColor2, LinearGradientMode gradientMode, HorzAlignment hAlignment, VertAlignment vAlignment) : this(foreColor, backColor, borderColor, backColor2, null, hAlignment, vAlignment) {
			this.gradientMode = gradientMode;
		}
		public AppearanceDefault(Color foreColor, Color backColor, Color borderColor, Color backColor2, LinearGradientMode gradientMode, HorzAlignment hAlignment, VertAlignment vAlignment, Font font) : this(foreColor, backColor, borderColor, backColor2, font, hAlignment, vAlignment) {
			this.gradientMode = gradientMode;
		}
		public AppearanceDefault(Color foreColor, Color backColor, Color borderColor, Color backColor2, HorzAlignment hAlignment, VertAlignment vAlignment) : this(foreColor, backColor, borderColor, backColor2, null, hAlignment, vAlignment) {
		}
		public AppearanceDefault(Color foreColor, Color backColor, HorzAlignment hAlignment, VertAlignment vAlignment) : this(foreColor, backColor, Color.Empty, Color.Empty, null, hAlignment, vAlignment) {
		}
		public AppearanceDefault(Color foreColor, Color backColor, HorzAlignment hAlignment, VertAlignment vAlignment, Font font) : this(foreColor, backColor, Color.Empty, Color.Empty, font, hAlignment, vAlignment) {
		}
		public AppearanceDefault(Color foreColor, Color backColor, HorzAlignment hAlignment) : this(foreColor, backColor, Color.Empty, Color.Empty, null, hAlignment, VertAlignment.Default) {
		}
		public AppearanceDefault() {
			Clear();
		}
		public virtual void Clear() {
			this.hAlignment = HorzAlignment.Default;
			this.vAlignment = VertAlignment.Default;
			this.foreColor = this.backColor = this.backColor2 = this.borderColor = Color.Empty;
			this.font = null;
			this.gradientMode = LinearGradientMode.Horizontal;
			this.fontStyleDelta = FontStyle.Regular;
			this.fontSizeDelta = 0;
			this.highPriority = DefaultBoolean.Default;
		}
		public int FontSizeDelta { get { return fontSizeDelta; } set { fontSizeDelta = value; } }
		public FontStyle FontStyleDelta { get { return fontStyleDelta; } set { fontStyleDelta = value; } }
		public HorzAlignment HAlignment { get { return hAlignment; } set { hAlignment = value; } }
		public VertAlignment VAlignment { get { return vAlignment; } set { vAlignment = value; } }
		public Color ForeColor { get { return foreColor; } set { foreColor = value; } }
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		public Color BackColor2 { get { return backColor2; } set { backColor2 = value; } }
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		public Font Font { 
			get { return font; } 
			set { 
				font = value; 
			} 
		}
		public LinearGradientMode GradientMode { get { return gradientMode; } set { gradientMode = value; } }
	}
	public class AppearanceDefaultInfo {
		string name;
		AppearanceDefault defaultAppearance;
		public AppearanceDefaultInfo(string name, AppearanceDefault defaultAppearance) {
			this.name = name;
			this.defaultAppearance = defaultAppearance;
		}
		public string Name { get { return name; } }
		public AppearanceDefault DefaultAppearance { get { return defaultAppearance; } set { defaultAppearance = value; } }
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IAppearanceOwner {
		bool IsLoading { get; }
	}
	public interface IAppearanceDefaultFontProvider {
		Font DefaultFont { get; set;  }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BaseAppearanceCollection : IDisposable, IAppearanceOwner, IXtraSerializable2, IEnumerable, IAppearanceDefaultFontProvider {
		[Browsable(false)]
		public event EventHandler Changed, PaintChanged, SizeChanged; 
		Hashtable names;
		int lockUpdate = 0;
		bool isDisposed;
		public BaseAppearanceCollection() {
			this.names = new Hashtable();
			CreateAppearances();
		}
		public virtual void Dispose() {
			PaintChanged = SizeChanged = Changed = null;
			DestroyAppearances();
			isDisposed = true;
		}
		public virtual void BeginUpdate() {
			this.lockUpdate ++;
		}
		public virtual void EndUpdate() {
			if(-- this.lockUpdate == 0) OnSizeChanged();
		}
		public virtual void CancelUpdate() {
			-- this.lockUpdate;
		}
		public void AssignInternal(BaseAppearanceCollection source) {
			BeginUpdate();
			try {
				Assign(source);
			} finally {
				CancelUpdate();
			}
		}
		public virtual void Assign(BaseAppearanceCollection source) {
			if(source == null) return;
			BeginUpdate();
			try {
				foreach(DictionaryEntry entry in Names) {
					AppearanceObject currentApp = entry.Value as AppearanceObject,
						sourceApp = source.GetAppearance(entry.Key.ToString());
					if(sourceApp == null || currentApp == null) continue;
					currentApp.AssignInternal(sourceApp);
				}
			} 
			finally {
				EndUpdate();
			}
		}
		public void UpdateRightToLeft(bool rightToLeft) {
			foreach(DictionaryEntry entry in Names) {
				AppearanceObject currentApp = entry.Value as AppearanceObject;
				currentApp.TextOptions.RightToLeft = rightToLeft;
			}
		}
		bool deserializing = false;
		bool IAppearanceOwner.IsLoading { get { return deserializing || IsLoading; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsLoading { get { return false; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDisposed { get { return isDisposed; } }
		public void Combine(BaseAppearanceCollection appearances, AppearanceDefaultInfo[] defaults) {
			Combine(appearances, defaults, true);
		}
		public virtual void Combine(BaseAppearanceCollection appearances, AppearanceDefaultInfo[] defaults, bool setDefaultUseFlag) {
			if(defaults == null) return;
			for(int n = 0; n < defaults.Length; n++) {
				AppearanceDefaultInfo info = defaults[n];
				Combine(GetAppearance(info.Name), appearances.GetAppearance(info.Name), info.DefaultAppearance, setDefaultUseFlag);
			}
		}
		protected virtual void Combine(AppearanceObject paint, AppearanceObject appearance, AppearanceDefault appearanceDefault,  bool setdefaultUseFlag) {
			if(paint == null || appearance == null) return;
			AppearanceHelper.Combine(paint, new AppearanceObject[] { appearance }, appearanceDefault, setdefaultUseFlag);
		}
		protected bool IsLockUpdate { get { return this.lockUpdate != 0; } }
		protected internal AppearanceObject CreateAppearance(string name) { return CreateAppearance(name, null); }
		protected virtual AppearanceObject CreateAppearance(string name, AppearanceObject parent) {
			ValidateAppearanceName(name);
			AppearanceObject appearance =  CreateAppearanceInstance(parent, name);
			InitAppearance(appearance, name);
			return appearance;
		}
		protected virtual AppearanceObject CreateAppearanceInstance(AppearanceObject parent, string name) {
			return new AppearanceObject(this, parent, name);
		}
		protected void ValidateAppearanceName(string name) {
			AppearanceObject existingAppearance = Names[name] as AppearanceObject;
			if(existingAppearance != null)
				throw new ArgumentException(String.Format("Duplicate appearance name: {0}", name), "name");
		}
		protected virtual void InitAppearance(AppearanceObject appearance, string name) {
			Subscribe(appearance);
			Names[name] = appearance;
		}
		protected virtual void DestroyAppearance(AppearanceObject appearance) {
			if(appearance == null) return;
			Unsubscribe(appearance);
			appearance.Dispose();
		}
		protected virtual void Subscribe(AppearanceObject appearance) {
			appearance.Changed += new EventHandler(OnAppearanceChanged);
			appearance.SizeChanged += new EventHandler(OnAppearanceSizeChanged);
			appearance.PaintChanged += new EventHandler(OnAppearancePaintChanged);
		}
		protected virtual void Unsubscribe(AppearanceObject appearance) {
			appearance.Changed -= new EventHandler(OnAppearanceChanged);
			appearance.SizeChanged -= new EventHandler(OnAppearanceSizeChanged);
			appearance.PaintChanged -= new EventHandler(OnAppearancePaintChanged);
		}
		protected virtual void DestroyAppearances() {
			foreach (AppearanceObject appearance in Names.Values)
				DestroyAppearance(appearance);
			Names.Clear();
		}
		protected internal Hashtable Names { 
			get { 
				if(names == null) names = new Hashtable();
				return names;
			}
		}
		public virtual IEnumerator GetEnumerator() {
			return Names.Values.GetEnumerator();
		}
		protected virtual void CreateAppearances() { }
		public virtual AppearanceObject GetAppearance(string name) {
			AppearanceObject res = Names[name] as AppearanceObject;
			if(res == null) return CreateNullAppearance();
			return res;
		}
		protected virtual AppearanceObject CreateNullAppearance() { return new AppearanceObject(); }
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			OnChanged();
		}
		protected virtual void OnAppearanceSizeChanged(object sender, EventArgs e) {
			OnSizeChanged();
		}
		protected virtual void OnAppearancePaintChanged(object sender, EventArgs e) {
			OnPaintChanged();
		}
		protected virtual void OnPaintChanged() {
			if(IsLockUpdate) return;
			if(PaintChanged != null) PaintChanged(this, EventArgs.Empty);
			OnChanged();
		}
		protected virtual void OnSizeChanged() {
			if(IsLockUpdate) return;
			if(SizeChanged != null) SizeChanged(this, EventArgs.Empty);
			OnChanged();
		}
		protected virtual void OnChanged() {
			if(IsLockUpdate) return;
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
		public virtual void Reset() {
			BeginUpdate();
			try {
				foreach(DictionaryEntry entry in Names) {
					AppearanceObject app = entry.Value as AppearanceObject;
					app.Reset();
				}
			} finally {
				EndUpdate();
			}
		}
		public virtual bool ShouldSerialize() {
			foreach(DictionaryEntry entry in Names) {
				AppearanceObject app = entry.Value as AppearanceObject;
				if(app.ShouldSerialize())
					return true;
			}
			return false;
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			ArrayList list = new ArrayList();
			foreach(DictionaryEntry entry in Names) {
				XtraPropertyInfo pInfo = new XtraPropertyInfo(entry.Key.ToString(), typeof(string), entry.Key.ToString(), true);
				SerializeHelper helper = new SerializeHelper();
				pInfo.ChildProperties.AddRange(helper.SerializeObject(entry.Value, XtraSerializationFlags.DefaultValue, OptionsLayoutBase.FullLayout));
				if(pInfo.ChildProperties.Count > 0) list.Add(pInfo);
			}
			return list.ToArray(typeof(XtraPropertyInfo)) as XtraPropertyInfo[];
		}
		void IXtraSerializable2.Deserialize(IList list) {
			BeginUpdate();
			try {
				this.deserializing = true;
				Reset();
				foreach(XtraPropertyInfo info in list) {
					DeserializeCore(info);
				}
			} finally {
				this.deserializing = false;
				EndUpdate();
			}
		}
		protected virtual void DeserializeCore(XtraPropertyInfo info) {
			AppearanceObject app = GetAppearance(info.Name);
			if(app != null) {
				DeserializeHelper helper = new DeserializeHelper();
				helper.DeserializeObject(app, info.ChildProperties, OptionsLayoutBase.FullLayout);
			}
		}
		public virtual void SaveLayoutToXml(string xmlFile) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void SaveLayoutToRegistry(string path) {
			SaveLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void RestoreLayoutFromRegistry(string path) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void SaveLayoutToStream(Stream stream) {
			SaveLayoutCore(new XmlXtraSerializer(), stream);
		}
		public virtual void RestoreLayoutFromStream(Stream stream) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream);
		}
		protected virtual void SaveLayoutCore(XtraSerializer serializer, object path) {
			Stream stream = path as Stream;
			if(stream != null) 
				serializer.SerializeObject(this, stream, "Appearances");
			else
				serializer.SerializeObject(this, path.ToString(), "Appearances");
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			Stream stream = path as Stream;
			if(stream != null) 
				serializer.DeserializeObject(this, stream, "Appearances");
			else
				serializer.DeserializeObject(this, path.ToString(), "Appearances");
		}
		Font defaultFont = null;
		Font IAppearanceDefaultFontProvider.DefaultFont {
			get {
				return defaultFont;	
			}
			set { defaultFont = value; }
		}
	}
	public class BrickCache {
		Hashtable bricks;
		public BrickCache() {
			this.bricks = new Hashtable();
		}
		public BrickStyle this[string name] { get { return Bricks[name] as BrickStyle; } }
		protected Hashtable Bricks { get { return bricks; } }
		public void Add(string name, AppearanceObject appearance, BorderSide borderSide, Color borderColor, int borderWidth) {
			BrickStyle brick = Create(appearance, borderSide, borderColor, borderWidth);
			Bricks[name] = brick;
		}
		public void Add(string name, AppearanceObject appearance) {
			Add(name, appearance, BorderSide.None, Color.Transparent, 1);
		}
		public void Clear() {
			Bricks.Clear();
		}
		public BrickStyle Create(AppearanceObject appearance, BorderSide borderSide, Color borderColor, int borderWidth) {
			if(borderColor == Color.Empty) borderColor = Color.Gray;
			BrickStyle brick = new BrickStyle(borderSide, borderWidth, borderColor, appearance.GetBackColor(), 
				appearance.GetForeColor(), appearance.GetFont(), new BrickStringFormat(appearance.GetStringFormat()));
			return brick;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class BaseOwnerAppearance : IDisposable, IAppearanceOwner {
		int lockUpdate;
		public event EventHandler Changed;
		IAppearanceOwner owner;
		public BaseOwnerAppearance(IAppearanceOwner owner) {
			this.owner = owner;
		}
		bool IAppearanceOwner.IsLoading { get { return owner != null && owner.IsLoading; } }
		protected virtual AppearanceObject CreateAppearance() {
			AppearanceObject a = new AppearanceObject(this, true);
			a.Changed += new EventHandler(OnApperanceChanged);
			return a;
		}
		protected virtual void DestroyAppearance(AppearanceObject appearance) {
			if(appearance == null) return;
			appearance.Changed -= new EventHandler(OnApperanceChanged);
			appearance.Dispose();
		}
		public virtual void Dispose() { Changed = null; }
		public virtual void BeginUpdate() {
			this.lockUpdate++;
		}
		public virtual void EndUpdate() {
			if(--this.lockUpdate == 0)
				OnChanged();
		}
		public virtual void Reset() {
			BeginUpdate();
			try {
				OnResetCore();
			} finally {
				EndUpdate();
			}
		}
		protected abstract void OnResetCore();
		protected virtual void OnChanged() {
			if(this.lockUpdate != 0) return;
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
		protected void OnApperanceChanged(object sender, EventArgs e) {
			OnChanged();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerialize() {
			PropertyDescriptorCollection pdColl = TypeDescriptor.GetProperties(this);
			foreach(PropertyDescriptor pd in pdColl) {
				if(pd.SerializationVisibility != DesignerSerializationVisibility.Hidden && pd.ShouldSerializeValue(this)) return true;
			}
			return false;
		}
		protected IAppearanceOwner OwnerCore { get { return owner; } }
	}
	public class FontBehaviorHelper {
		internal static WindowsFormsFontBehavior GetFontBehavior() {
			var res = WindowsFormsSettings.FontBehavior;
			if(res == WindowsFormsFontBehavior.Default) res = WindowsFormsFontBehavior.UseTahoma;
			return res;
		}
		internal static Font GetDefaultFont() {
			if(ForcedFromSettingsFont != null) return ForcedFromSettingsFont;
			switch(GetFontBehavior()) {
				case WindowsFormsFontBehavior.ForceTahoma:
					return GetTahomaFont();
				case WindowsFormsFontBehavior.ForceWindowsFont:
					return SystemFonts.MessageBoxFont;
				case WindowsFormsFontBehavior.UseWindowsFont:
					return SystemFonts.MessageBoxFont;
				case WindowsFormsFontBehavior.UseTahoma:
					return GetTahomaFont();
				case WindowsFormsFontBehavior.UseControlFont:
					return SystemFonts.DefaultFont;
			}
			return SystemFonts.DefaultFont;
		}
		static Font tahomaFont = null;
		static Font GetTahomaFont() {
			if(tahomaFont == null) tahomaFont = CreateTahomaFont();
			return tahomaFont;
		}
		static Font CreateTahomaFont() {
			try {
				return new Font(new FontFamily("Tahoma"), SystemFonts.DefaultFont.Size);
			}
			catch { }
			return SystemFonts.DefaultFont;
		}
		static bool userPreferencesSubscribed = false;
		static bool fontPreferencesInitialized = false;
		public static void CheckFont() {
			if(fontPreferencesInitialized) return;
			Update();
		}
		public static void Update() {
			fontPreferencesInitialized = true;
			switch(GetFontBehavior()) {
				case WindowsFormsFontBehavior.ForceTahoma:
				case WindowsFormsFontBehavior.ForceWindowsFont:
					AppearanceObject.ResetDefaultFont();
					UpdateDefaultFont(GetDefaultFont());
					SubscribeUserPreferences();
					break;
				case WindowsFormsFontBehavior.UseTahoma:
					AppearanceObject.ResetDefaultFont();
					UpdateDefaultFont(null);
					UnsubscribeUserPreferences();
					break;
				case WindowsFormsFontBehavior.UseWindowsFont:
					AppearanceObject.ResetDefaultFont();
					UpdateDefaultFont(null);
					UnsubscribeUserPreferences();
					break;
				case WindowsFormsFontBehavior.UseControlFont:
					UpdateDefaultFont(SystemFonts.DefaultFont);
					AppearanceObject.ResetDefaultFont();
					UnsubscribeUserPreferences();
					break;
			}
		}
		static void UnsubscribeUserPreferences() {
			if(!userPreferencesSubscribed) return;
			userPreferencesSubscribed = false;
			Microsoft.Win32.SystemEvents.UserPreferenceChanged -= OnUserPreferencesChanged;
		}
		static void SubscribeUserPreferences() {
			if(userPreferencesSubscribed) return;
			userPreferencesSubscribed = true;
			Microsoft.Win32.SystemEvents.UserPreferenceChanged += OnUserPreferencesChanged;
		}
		static void OnUserPreferencesChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e) {
			tahomaFont = null;
			Update();
		}
		static List<object> defaultFonts;
		static void UpdateDefaultFont(Font font) {
			if(font == null && Control.DefaultFont.Equals(SystemFonts.DefaultFont)) return;
			var fiFont = typeof(Control).GetField("defaultFont", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			var fiFontHandle = typeof(Control).GetField("defaultFontHandleWrapper", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			fiFont.SetValue(null, font);
			if(defaultFonts == null) defaultFonts = new List<object>();
			defaultFonts.Add(fiFontHandle.GetValue(null)); 
			fiFontHandle.SetValue(null, null);
		}
		internal static Font ForcedFromSettingsFont { get; set; }
	}
}
