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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
namespace DevExpress.XtraGauges.Core.Drawing {
	[TypeConverter(typeof(BaseAppearanceObjectTypeConverter))]
	public abstract class BaseAppearance : BaseObjectEx, ISupportColorShading, ISupportLockUpdate, ISupportAssign<BaseAppearance> {
		public void Accept(IColorShader shader) {
			BeginUpdate();
			AcceptCore(shader);
			EndUpdate();
		}
		protected virtual void AcceptCore(IColorShader shader) { }
		public void Assign(BaseAppearance appearance) {
			BeginUpdate();
			try {
				AssignCore(appearance);
			}
			finally { EndUpdate(); }
		}
		public bool IsDifferFrom(BaseAppearance source) {
			return IsDifferFromCore(source);
		}
		protected internal void AssignInternal(BaseAppearance appearance) {
			BeginUpdate();
			try {
				AssignCore(appearance);
			}
			finally { CancelUpdate(); }
		}
		protected override void CopyToCore(BaseObject clone) {
			BaseAppearance appearance = clone as BaseAppearance;
			if(appearance != null) appearance.Assign(this);
		}
		protected abstract void AssignCore(BaseAppearance appearance);
		protected abstract bool IsDifferFromCore(BaseAppearance appearance);
		protected void ReplaceAppearanceBrushObject(ref BrushObject target, BrushObject value) {
			if(target == value) return;
			if(target != null && !target.IsEmpty)
				target.Changed -= OnBrushChanged;
			target = value;
			if(target != null && !target.IsEmpty)
				target.Changed += OnBrushChanged;
		}
		protected void AssignAppearanceBrushObject(ref BrushObject target, BrushObject value) {
			DestroyAppearanceBrushObject(ref target);
			SetAppearanceBrushObject(ref target, value);
		}
		protected virtual void SetAppearanceBrushObject(ref BrushObject target, BrushObject value) {
			target = value;
			if(target != null && !target.IsEmpty)
				target.Changed += OnBrushChanged;
		}
		protected virtual void DestroyAppearanceBrushObject(ref BrushObject target) {
			if(target != null && !target.IsEmpty) {
				target.Changed -= OnBrushChanged;
				Ref.Dispose(ref target);
			}
		}
		void OnBrushChanged(object sender, EventArgs ea) {
			OnObjectChanged(ea);
		}
		public bool ShouldSerialize() {
			return ShouldSerializeCore();
		}
		public void Reset() {
			BeginUpdate();
			ResetCore();
			EndUpdate();
		}
		protected abstract bool ShouldSerializeCore();
		protected abstract void ResetCore();
		static Color[] EmptyColors = new Color[0];
		protected virtual Color[] ToColors() {
			return EmptyColors;
		}
		protected virtual void FromColors(Color[] colors) { }
		protected bool ShouldSerializeBrushCore(BrushObject brush) {
			return brush.ShouldSerialize();
		}
		protected object XtraCreateBrushCore(XtraItemEventArgs e) {
			if(e.Item.HasChildren) {
				foreach(XtraPropertyInfo info in e.Item.ChildProperties) {
					if(info.Name == "Color") {
						return new SolidBrushObject();
					}
					if(info.Name == "StartPoint" || info.Name == "EndPoint") {
						return new LinearGradientBrushObject();
					}
					if(info.Name == "Center" || info.Name == "RadiusX" || info.Name == "RadiusY") {
						return new EllipticalGradientBrushObject();
					}
				}
			}
			return BrushObject.Empty;
		}
	}
	public class BaseTextAppearance : BaseAppearance {
		Font fontCore;
		TextSpacing textSpacingCore;
		StringFormatObject stringFormatCore;
		BrushObject textBrushCore;
		public BaseTextAppearance()
			: base() {
		}
		public BaseTextAppearance(BrushObject brush)
			: base() {
			this.textBrushCore = brush;
		}
		public BaseTextAppearance(BaseTextAppearance appearance)
			: base() {
			AssignInternal(appearance);
		}
		public static Font DefaultFont = new Font("Tahoma", 8.25f);
		protected override void OnCreate() {
			base.OnCreate();
			this.fontCore = DefaultFont;
			this.textSpacingCore = new TextSpacing();
			this.textBrushCore = BrushObject.Empty;
		}
		protected override void OnDispose() {
			DestroyAppearanceBrushObject(ref textBrushCore);
			DestroyFont();
			DestroyStringFormatObject(ref stringFormatCore);
			base.OnDispose();
		}
		void OnStringFormatChanged(object sender, EventArgs ea) {
			OnObjectChanged("StringFormat");
		}
		protected override void AcceptCore(IColorShader shader) {
			TextBrush.Accept(shader);
		}
		[XtraSerializableProperty]
		public Font Font {
			get { return fontCore; }
			set {
				if(Font.Equals(value)) return;
				DestroyFont();
				fontCore = value;
				OnObjectChanged("Font");
			}
		}
		[XtraSerializableProperty]
		public TextSpacing Spacing {
			get { return textSpacingCore; }
			set {
				if(Spacing == value) return;
				textSpacingCore = value;
				OnObjectChanged("Spacing");
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public StringFormatObject Format {
			get {
				if(stringFormatCore == null)
					SetStringFormatObject(ref stringFormatCore, new StringFormatObject());
				return stringFormatCore;
			}
			set {
				if(stringFormatCore == value) return;
				AssignStringFormatObject(ref stringFormatCore, value);
				OnObjectChanged("Format");
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public BrushObject TextBrush {
			get { return textBrushCore; }
			set {
				if(TextBrush == value) return;
				AssignAppearanceBrushObject(ref textBrushCore, value);
				OnObjectChanged("TextBrush");
			}
		}
		protected void DestroyFont() {
			fontCore = null;
		}
		protected override BaseObject CloneCore() {
			return new BaseTextAppearance();
		}
		protected override void AssignCore(BaseAppearance appearance) {
			BaseTextAppearance source = appearance as BaseTextAppearance;
			if(source != null) {
				TextBrush = (BrushObject)source.TextBrush.Clone();
				Font = source.Font;
				Format = (source.stringFormatCore == null) ? null : (StringFormatObject)source.Format.Clone();
				Spacing = new TextSpacing(source.Spacing);
			}
		}
		protected override bool IsDifferFromCore(BaseAppearance appearance) {
			BaseTextAppearance source = appearance as BaseTextAppearance;
			return (source == null) ? true :
						TextBrush.IsDifferFrom(source.TextBrush) ||
						(!this.Font.Equals(source.Font)) ||
						(Format.Alignment != source.Format.Alignment ||
						 Format.LineAlignment != source.Format.LineAlignment ||
						 Format.Trimming != source.Format.Trimming ||
						 Format.FormatFlags != source.Format.FormatFlags) ||
						(Spacing != source.Spacing);
		}
		protected override bool ShouldSerializeCore() {
			return ShouldSerializeSpacing() || ShouldSerializeTextBrush() || ShouldSerializeFont();
		}
		protected override void ResetCore() {
			ResetSpacing();
			ResetTextBrush();
			ResetFont();
		}
		internal void ResetFont() {
			Font = DefaultFont;
		}
		internal bool ShouldSerializeFont() {
			return !this.Font.Equals(DefaultFont);
		}
		internal void ResetTextBrush() {
			SolidBrushObject solidBrush = TextBrush as SolidBrushObject;
			TextBrush = BrushObject.Empty;
		}
		internal bool ShouldSerializeTextBrush() {
			SolidBrushObject solidBrush = TextBrush as SolidBrushObject;
			return !TextBrush.IsEmpty;
		}
		internal void ResetFormat() {
			Format.Alignment = StringAlignment.Center;
			Format.LineAlignment = StringAlignment.Center;
			Format.Trimming = StringTrimming.Character;
			Format.FormatFlags = StringFormatFlags.NoClip;
		}
		internal bool ShouldSerializeFormat() {
			return
				Format.Alignment != StringAlignment.Center ||
				Format.LineAlignment != StringAlignment.Center ||
				Format.Trimming != StringTrimming.Character ||
				Format.FormatFlags != StringFormatFlags.NoClip;
		}
		internal void ResetSpacing() {
			Spacing = new TextSpacing(0, 0, 0, 0);
		}
		internal bool ShouldSerializeSpacing() {
			return Spacing != new TextSpacing(0, 0, 0, 0);
		}
		protected  void AssignStringFormatObject(ref StringFormatObject target, StringFormatObject value) {
			DestroyStringFormatObject(ref target);
			SetStringFormatObject(ref target, value);
		}
		protected virtual void SetStringFormatObject(ref StringFormatObject target, StringFormatObject value) {
			target = value;
			if(target != null) target.Changed += OnStringFormatChanged;
		}
		protected virtual void DestroyStringFormatObject(ref StringFormatObject target) {
			if(target != null) {
				target.Changed -= OnStringFormatChanged;
				Ref.Dispose(ref target);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateTextBrush(XtraItemEventArgs e) {
			return XtraCreateBrushCore(e);
		}
	}
	public class FrozenTextAppearance : BaseTextAppearance {
		  public FrozenTextAppearance()
			: base() {
		}
		public FrozenTextAppearance(BrushObject brush)
			: base(brush) {
		}
		public FrozenTextAppearance(BaseTextAppearance appearance)
			: base(appearance) {
		}
		protected override void SetAppearanceBrushObject(ref BrushObject target, BrushObject value) {
			target = value;
		}
		protected override void SetStringFormatObject(ref StringFormatObject target, StringFormatObject value) {
			target = value;
		}
		protected override void DestroyAppearanceBrushObject(ref BrushObject target) {
			if(target != null && !target.IsEmpty) {
				Ref.Dispose(ref target);
			}
		}
		protected override void DestroyStringFormatObject(ref StringFormatObject target) {
			if(target != null) {
				Ref.Dispose(ref target);
			}
		}
	}
	public class BaseShapeAppearance : BaseAppearance {
		BrushObject borderBrushCore;
		BrushObject contentBrushCore;
		float widthCore;
		public BaseShapeAppearance() : base() { }
		public BaseShapeAppearance(BaseShapeAppearance appearance)
			: base() {
			AssignInternal(appearance);
		}
		public BaseShapeAppearance(BrushObject border, BrushObject content)
			: base() {
			BorderBrush = border;
			ContentBrush = content;
		}
		protected override void AcceptCore(IColorShader shader) {
			BorderBrush.Accept(shader);
			ContentBrush.Accept(shader);
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public BrushObject BorderBrush {
			get { return borderBrushCore; }
			set {
				if(BorderBrush == value) return;
				AssignAppearanceBrushObject(ref this.borderBrushCore, value);
				OnObjectChanged("BorderBrush");
			}
		}
		protected internal void ReplaceBorderBrush(BrushObject value) {
			ReplaceAppearanceBrushObject(ref borderBrushCore, value);
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public BrushObject ContentBrush {
			get { return contentBrushCore; }
			set {
				if(ContentBrush == value) return;
				AssignAppearanceBrushObject(ref this.contentBrushCore, value);
				OnObjectChanged("ContentBrush");
			}
		}
		[DefaultValue(0f)]
		[XtraSerializableProperty]
		public float BorderWidth {
			get { return widthCore; }
			set {
				if(BorderWidth == value) return;
				this.widthCore = value;
				OnObjectChanged("BorderWidth");
			}
		}
		protected override void OnCreate() {
			this.borderBrushCore = BrushObject.Empty;
			this.contentBrushCore = BrushObject.Empty;
		}
		protected override void OnDispose() {
			DestroyAppearanceBrushObject(ref borderBrushCore);
			DestroyAppearanceBrushObject(ref contentBrushCore);
			base.OnDispose();
		}
		protected override void AssignCore(BaseAppearance appearance) {
			BaseShapeAppearance source = appearance as BaseShapeAppearance;
			if(source != null) {
				BorderBrush = (BrushObject)source.BorderBrush.Clone();
				ContentBrush = (BrushObject)source.ContentBrush.Clone();
				BorderWidth = source.BorderWidth;
			}
		}
		protected override bool IsDifferFromCore(BaseAppearance appearance) {
			BaseShapeAppearance source = appearance as BaseShapeAppearance;
			return (source == null) ? true :
						BorderBrush.IsDifferFrom(source.BorderBrush) ||
						ContentBrush.IsDifferFrom(source.ContentBrush) ||
						(BorderWidth != source.BorderWidth);
		}
		protected override BaseObject CloneCore() {
			return new BaseShapeAppearance();
		}
		protected override bool ShouldSerializeCore() {
			return ShouldSerializeBorderBrush() || ShouldSerializeContentBrush() || BorderWidth != 0;
		}
		protected override void ResetCore() {
			BorderWidth = 0;
			ResetBorderBrush();
			ResetContentBrush();
		}
		internal void ResetBorderBrush() {
			SolidBrushObject solidBrush = BorderBrush as SolidBrushObject;
			if(solidBrush != null) solidBrush.Color = Color.White;
			else BorderBrush = BrushObject.Empty;
		}
		internal bool ShouldSerializeBorderBrush() {
			return ShouldSerializeBrushCore(BorderBrush);
		}
		internal void ResetContentBrush() {
			SolidBrushObject solidBrush = ContentBrush as SolidBrushObject;
			if(solidBrush != null) solidBrush.Color = Color.White;
			else ContentBrush = BrushObject.Empty;
		}
		internal bool ShouldSerializeContentBrush() {
			return ShouldSerializeBrushCore(ContentBrush);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateBorderBrush(XtraItemEventArgs e) {
			return XtraCreateBrushCore(e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateContentBrush(XtraItemEventArgs e) {
			return XtraCreateBrushCore(e);
		}
	}
	public class RangeBarAppearance : BaseShapeAppearance {
		BrushObject backgroundBrushCore;
		public RangeBarAppearance() : base() { }
		public RangeBarAppearance(BrushObject border, BrushObject content, BrushObject background)
			: base() {
			BorderBrush = border;
			ContentBrush = content;
			BackgroundBrush = background;
		}
		public RangeBarAppearance(RangeBarAppearance appearance)
			: base() {
			AssignInternal(appearance);
		}
		protected override void AcceptCore(IColorShader shader) {
			base.AcceptCore(shader);
			BackgroundBrush.Accept(shader);
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public BrushObject BackgroundBrush {
			get { return backgroundBrushCore; }
			set {
				if(BackgroundBrush == value) return;
				AssignAppearanceBrushObject(ref this.backgroundBrushCore, value);
				OnObjectChanged("BackgroundBrush");
			}
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.BackgroundBrush = BrushObject.Empty;
		}
		protected override void OnDispose() {
			DestroyAppearanceBrushObject(ref backgroundBrushCore);
			base.OnDispose();
		}
		protected override void AssignCore(BaseAppearance appearance) {
			RangeBarAppearance source = appearance as RangeBarAppearance;
			base.AssignCore(appearance);
			if(source != null)
				BackgroundBrush = (BrushObject)source.BackgroundBrush.Clone();
		}
		protected override bool IsDifferFromCore(BaseAppearance appearance) {
			RangeBarAppearance source = appearance as RangeBarAppearance;
			return (source == null) ? true :
						BackgroundBrush.IsDifferFrom(source.BackgroundBrush) ||
						BorderBrush.IsDifferFrom(source.BorderBrush) ||
						ContentBrush.IsDifferFrom(source.ContentBrush) ||
						(BorderWidth != source.BorderWidth);
		}
		protected override BaseObject CloneCore() {
			return new RangeBarAppearance();
		}
		protected override bool ShouldSerializeCore() {
			return ShouldSerializeBorderBrush() || ShouldSerializeContentBrush() || ShouldSerializeBackgroundBrush() || BorderWidth != 0;
		}
		protected override void ResetCore() {
			base.ResetCore();
			ResetBackgroundBrush();
		}
		internal void ResetBackgroundBrush() {
			SolidBrushObject solidBrush = BackgroundBrush as SolidBrushObject;
			if(solidBrush != null) solidBrush.Color = Color.White;
			else BackgroundBrush = BrushObject.Empty;
		}
		internal bool ShouldSerializeBackgroundBrush() {
			return ShouldSerializeBrushCore(BackgroundBrush);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateBackgroundBrush(XtraItemEventArgs e) {
			return XtraCreateBrushCore(e);
		}
	}
	public class FrozenRangeBarAppearance : RangeBarAppearance {
		public FrozenRangeBarAppearance() : base() { }
		public FrozenRangeBarAppearance(BrushObject border, BrushObject content, BrushObject background)
			: base() {
		}
		protected override void SetAppearanceBrushObject(ref BrushObject target, BrushObject value) {
			target = value;
		}
		protected override void DestroyAppearanceBrushObject(ref BrushObject target) {
			if(target != null && !target.IsEmpty) {
				Ref.Dispose(ref target);
			}
		}
	}
	public class BaseScaleAppearance : BaseAppearance {
		BrushObject brushCore;
		float widthCore;
		public BaseScaleAppearance() : base() { }
		public BaseScaleAppearance(BaseScaleAppearance appearance)
			: base() {
			AssignInternal(appearance);
		}
		public BaseScaleAppearance(BrushObject border, BrushObject content)
			: base() {
			BeginUpdate();
			Brush = border;
			CancelUpdate();
		}
		protected override void AcceptCore(IColorShader shader) {
			Brush.Accept(shader);
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public BrushObject Brush {
			get { return brushCore; }
			set {
				if(Brush == value) return;
				AssignAppearanceBrushObject(ref this.brushCore, value);
				OnObjectChanged("Brush");
			}
		}
		[DefaultValue(0f)]
		[XtraSerializableProperty]
		public float Width {
			get { return widthCore; }
			set {
				if(Width == value) return;
				this.widthCore = value;
				OnObjectChanged("Width");
			}
		}
		protected override void OnCreate() {
			this.brushCore = BrushObject.Empty;
		}
		protected override void OnDispose() {
			DestroyAppearanceBrushObject(ref brushCore);
			base.OnDispose();
		}
		protected override void AssignCore(BaseAppearance appearance) {
			BaseScaleAppearance source = appearance as BaseScaleAppearance;
			if(source != null) {
				Brush = (BrushObject)source.Brush.Clone();
				Width = source.Width;
			}
		}
		protected override bool IsDifferFromCore(BaseAppearance appearance) {
			BaseScaleAppearance source = appearance as BaseScaleAppearance;
			return appearance == null ? true :
							Brush.IsDifferFrom(source.Brush) ||
							Width != source.Width;
		}
		protected override BaseObject CloneCore() {
			return new BaseScaleAppearance();
		}
		protected override bool ShouldSerializeCore() {
			return ShouldSerializeBrush() || Width != 0;
		}
		protected override void ResetCore() {
			ResetBrush();
			Width = 0;
		}
		internal void ResetBrush() {
			if(Brush is SolidBrushObject) (Brush as SolidBrushObject).Color = Color.White;
			else Brush = BrushObject.Empty;
		}
		internal bool ShouldSerializeBrush() {
			return ShouldSerializeBrushCore(Brush);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateBrush(XtraItemEventArgs e) {
			return XtraCreateBrushCore(e);
		}
	}
	public class BaseAppearanceObjectTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return destType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			if(value is BaseScaleAppearance) return "<BaseScaleAppearance>";
			if(value is BaseShapeAppearance) return "<BaseShapeAppearance>";
			if(value is BaseTextAppearance) return "<BaseTextAppearance>";
			return "<BaseAppearance>";
		}
	}
	[TypeConverter(typeof(StringFormatObjectTypeConverter))]
	public class StringFormatObject : BaseObjectEx, ISupportLockUpdate, ISupportAssign<StringFormatObject> {
		StringFormat formatCore;
		public StringFormatObject() { }
		public StringFormatObject(StringAlignment alignment, StringAlignment lineAlignment, StringTrimming trimming, StringFormatFlags flags)
			: base() {
			Format.Alignment = alignment;
			Format.LineAlignment = lineAlignment;
			Format.Trimming = trimming;
			Format.FormatFlags = flags;
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.formatCore = new StringFormat(StringFormatFlags.NoClip);
			Format.Alignment = StringAlignment.Center;
			Format.LineAlignment = StringAlignment.Center;
			Format.Trimming = StringTrimming.Character;
		}
		protected override void OnDispose() {
			Ref.Dispose(ref formatCore);
			base.OnDispose();
		}
		protected StringFormat Format {
			get { return formatCore; }
		}
		[XtraSerializableProperty, DefaultValue(StringAlignment.Center)]
		public StringAlignment Alignment {
			get { return Format.Alignment; }
			set {
				if(Format.Alignment == value) return;
				Format.Alignment = value;
				OnObjectChanged("Alignment");
			}
		}
		[XtraSerializableProperty, DefaultValue(StringAlignment.Center)]
		public StringAlignment LineAlignment {
			get { return Format.LineAlignment; }
			set {
				if(Format.LineAlignment == value) return;
				Format.LineAlignment = value;
				OnObjectChanged("LineAlignment");
			}
		}
		[XtraSerializableProperty, DefaultValue(StringTrimming.Character)]
		public StringTrimming Trimming {
			get { return Format.Trimming; }
			set {
				if(Format.Trimming == value) return;
				Format.Trimming = value;
				OnObjectChanged("Trimming");
			}
		}
		[XtraSerializableProperty, DefaultValue(StringFormatFlags.NoClip)]
		public StringFormatFlags FormatFlags {
			get { return Format.FormatFlags; }
			set {
				if(Format.FormatFlags == value) return;
				Format.FormatFlags = value;
				OnObjectChanged("FormatFlags");
			}
		}
		public StringFormat NativeFormat {
			get { return formatCore; }
		}
		public void Assign(StringFormatObject appearance) {
			BeginUpdate();
			AssignCore(appearance);
			EndUpdate();
		}
		public bool IsDifferFrom(StringFormatObject source) {
			return IsDifferFromCore(source);
		}
		protected internal void AssignInternal(StringFormatObject appearance) {
			BeginUpdate();
			AssignCore(appearance);
			CancelUpdate();
		}
		protected override BaseObject CloneCore() {
			return new StringFormatObject();
		}
		protected override void CopyToCore(BaseObject clone) {
			StringFormatObject format = clone as StringFormatObject;
			if(format != null) format.Assign(this);
		}
		protected void AssignCore(StringFormatObject format) {
			StringFormatObject source = format as StringFormatObject;
			if(source != null) {
				Alignment = source.Alignment;
				LineAlignment = source.LineAlignment;
				Trimming = source.Trimming;
				FormatFlags = source.FormatFlags;
			}
		}
		protected bool IsDifferFromCore(StringFormatObject format) {
			StringFormatObject source = format as StringFormatObject;
			return (source == null) ? true :
						 Alignment != source.Alignment ||
						 LineAlignment != source.LineAlignment ||
						 Trimming != source.Trimming ||
						 FormatFlags != source.FormatFlags;
		}
	}
	public class StringFormatObjectTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return FilterPropertiesHelper.FilterProperties(base.GetProperties(context, value, attributes), GetExpectedProperties(value));
		}
		static string[] GetExpectedProperties(object value) {
			StringFormatObject sFormat = value as StringFormatObject;
			if(sFormat == null) return new string[0];
			return new string[] { "Alignment", "LineAlignment", "Trimming", "FormatFlags" };
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return (destType == typeof(string)) || (destType == typeof(InstanceDescriptor));
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			if(value is StringFormatObject) {
				StringFormatObject format = value as StringFormatObject;
				if(destType == typeof(string)) {
					string result = "<StringFormat";
					string[] values = new string[] { 
							(format.Alignment != StringAlignment.Center) ? String.Format("Alignment=\"{0}\"", format.Alignment) : null, 
							(format.LineAlignment != StringAlignment.Center) ? String.Format("LineAlignment=\"{0}\"", format.LineAlignment) : null, 
							(format.Trimming != StringTrimming.Character) ? String.Format("Trimming=\"{0}\"", format.Trimming) : null, 
							(format.FormatFlags != StringFormatFlags.NoClip) ? String.Format("FormatFlags=\"{0}\"", format.FormatFlags) : null
						};
					for(int i = 0; i < values.Length; i++) {
						if(values[i] != null) result += (" " + values[i]);
					}
					return result + "/>";
				}
				if(destType == typeof(InstanceDescriptor)) {
					ConstructorInfo constructor = typeof(StringFormatObject).GetConstructor(
							new Type[] { typeof(StringAlignment), typeof(StringAlignment), typeof(StringTrimming), typeof(StringFormatFlags) }
						);
					if(constructor != null) return new InstanceDescriptor(constructor,
							new object[] { format.Alignment, format.LineAlignment, format.Trimming, format.FormatFlags });
				}
			}
			return base.ConvertTo(context, culture, value, destType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string source = value as string;
			if(source != null) {
				StringFormatObject format = new StringFormatObject();
				MatchCollection alignMatches = Regex.Matches(source, "\\s+Alignment\\s*=\\s*\"(?<alignment>.*?)\"");
				MatchCollection lineAlignMatches = Regex.Matches(source, "\\s+LineAlignment\\s*=\\s*\"(?<lineAlignment>.*?)\"");
				MatchCollection trimmingMatches = Regex.Matches(source, "\\s+Trimming\\s*=\\s*\"(?<trimming>.*?)\"");
				MatchCollection fflagsMatches = Regex.Matches(source, "\\s+FormatFlags\\s*=\\s*\"(?<formatFlags>.*?)\"");
				format.BeginUpdate();
				if(alignMatches.Count == 1) format.Alignment = (StringAlignment)Enum.Parse(typeof(StringAlignment), alignMatches[0].Groups["alignment"].Value);
				if(lineAlignMatches.Count == 1) format.LineAlignment = (StringAlignment)Enum.Parse(typeof(StringAlignment), lineAlignMatches[0].Groups["lineAlignment"].Value);
				if(trimmingMatches.Count == 1) format.Trimming = (StringTrimming)Enum.Parse(typeof(StringTrimming), trimmingMatches[0].Groups["trimming"].Value);
				if(fflagsMatches.Count == 1) format.FormatFlags = (StringFormatFlags)Enum.Parse(typeof(StringFormatFlags), fflagsMatches[0].Groups["formatFlags"].Value);
				format.EndUpdate();
				return format;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
