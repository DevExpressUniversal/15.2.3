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
using System.Reflection;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(TextAnnotationTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class TextAnnotation : Annotation, ITextPropertiesProvider, ITextAppearance {
		const StringAlignment DefaultTextAlignment = StringAlignment.Center;
		const bool DefaultAutoSize = true;
		DefaultBoolean enableAntialiasing = DefaultBoolean.Default;
		bool autoSize = DefaultAutoSize;
		string text;
		Color textColor;
		Font font;
		StringAlignment textAlignment = DefaultTextAlignment;
		string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.DefaultAnnotation); } }
		Color DefaultTextColor { get { return Color.Empty; } }
		Font DefaultFont { get { return DefaultFonts.Tahoma8; } }
		Color ActualTextColor {
			get {
				if (textColor != Color.Empty)
					return textColor;
				else {
					if (TextAnnotationAppearance != null) {
						return TextAnnotationAppearance.TextColor;
					}
					return Color.Empty;
				}
			}
		}
		bool Rotated { get { return Angle % 90 != 0; } }
		bool ActualAntialiasing { get { return DefaultBooleanUtils.ToBoolean(enableAntialiasing, Rotated); } }
		TextAnnotationAppearance TextAnnotationAppearance { get { return Appearance as TextAnnotationAppearance; } }
		protected internal override bool ActualAutoSize { get { return autoSize; } set { autoSize = value; } }
		protected internal override string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.TextAnnotationPrefix); } }
		protected override AnnotationAppearance Appearance {
			get {
				IChartAppearance rootAppearance = CommonUtils.GetActualAppearance(this);
				return rootAppearance != null ? rootAppearance.TextAnnotationAppearance : null;
			}
		}
		internal string ActualText { get { return string.IsNullOrEmpty(text) ? Name : text; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TextAnnotationAutoSize"),
#endif
		Category(Categories.Behavior),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TextAnnotation.AutoSize"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool AutoSize {
			get { return autoSize; }
			set {
				if (value == autoSize)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				autoSize = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TextAnnotationEnableAntialiasing"),
#endif
		Category(Categories.Appearance),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TextAnnotation.EnableAntialiasing"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		XtraSerializableProperty
		]
		public DefaultBoolean EnableAntialiasing {
			get { return enableAntialiasing; }
			set {
				if (enableAntialiasing == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				enableAntialiasing = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TextAnnotationTextAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TextAnnotation.TextAlignment"),
		TypeConverter(typeof(StringAlignmentTypeConvertor)),
		Category(Categories.Behavior),
		Localizable(true),
		XtraSerializableProperty
		]
		public StringAlignment TextAlignment {
			get { return textAlignment; }
			set {
				if (value == textAlignment)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				textAlignment = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TextAnnotationFont"),
#endif
		Category(Categories.Appearance),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TextAnnotation.Font"),
		TypeConverter(typeof(FontTypeConverter)),
		Localizable(true),
		XtraSerializableProperty
		]
		public Font Font {
			get { return font; }
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectFont));
				SendNotification(new ElementWillChangeNotification(this));
				font = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TextAnnotationLines"),
#endif
		Category(Categories.Behavior),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TextAnnotation.Lines"),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor("DevExpress.XtraCharts.Design.StringCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public string[] Lines {
			get { return StringUtils.StringToStringArray(Text); }
			set { Text = StringUtils.StringArrayToString(value); }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TextAnnotationText"),
#endif
		Category(Categories.Behavior),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TextAnnotation.Text"),
		Localizable(true),
		XtraSerializableProperty
		]
		public string Text {
			get { return text; }
			set {
				if (value == text)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				text = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TextAnnotationTextColor"),
#endif
		Category(Categories.Appearance),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TextAnnotation.TextColor"),
		XtraSerializableProperty
		]
		public Color TextColor {
			get { return textColor; }
			set {
				if (value == textColor)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				textColor = value;
				RaiseControlChanged();
			}
		}
		[
		Obsolete("This property is now obsolete. Use the EnableAntialiasing property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool Antialiasing {
			get { return ActualAntialiasing; }
			set { EnableAntialiasing = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		public TextAnnotation() : this(string.Empty) { }
		public TextAnnotation(string name) : this(name, ChartLocalizer.GetString(ChartStringId.DefaultAnnotation)) {
		}
		public TextAnnotation(string name, string text) : base(name) {
			this.text = text;
			textColor = DefaultTextColor;
			font = DefaultFont;
		}
		#region ITextPropertiesProvider implementation
		StringAlignment ITextPropertiesProvider.Alignment { get { return TextAlignment; } }
		RectangleFillStyle ITextPropertiesProvider.FillStyle { get { return RectangleFillStyle.Empty; } }
		RectangularBorder ITextPropertiesProvider.Border { get { return null; } }
		Shadow ITextPropertiesProvider.Shadow { get { return null; } }
		bool ITextPropertiesProvider.ChangeSelectedBorder { get { return false; } }
		bool ITextPropertiesProvider.CorrectBoundsByBorder { get { return false; } }
		Color ITextPropertiesProvider.BackColor { get { return Color.Empty; } }
		Color ITextPropertiesProvider.GetTextColor(Color color) { return ActualTextColor; }
		Color ITextPropertiesProvider.GetBorderColor(Color color) { return Color.Empty; }
		#endregion
		#region ISupportTextAntialiasing implementation
		bool ISupportTextAntialiasing.DefaultAntialiasing { get { return false; } }
		bool ISupportTextAntialiasing.Rotated { get { return Rotated; } }
		Color ISupportTextAntialiasing.TextBackColor { get { return ActualBackColor; } }
		RectangleFillStyle ISupportTextAntialiasing.TextBackFillStyle { get { return ActualFillStyle; } }
		ChartElement ISupportTextAntialiasing.BackElement { get { return null; } }
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Text")
				return ShouldSerializeText();
			if (propertyName == "TextColor")
				return ShouldSerializeTextColor();
			if (propertyName == "Font")
				return ShouldSerializeFont();
			if (propertyName == "EnableAntialiasing")
				return ShouldSerializeEnableAntialiasing();
			if (propertyName == "TextAlignment")
				return ShouldSerializeTextAlignment();
			if (propertyName == "AutoSize")
				return ShouldSerializeAutoSize();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeAntialiasing() {
			return false;
		}
		bool ShouldSerializeEnableAntialiasing() {
			return this.enableAntialiasing != DefaultBoolean.Default;
		}
		void ResetEnableAntialiasing() {
			EnableAntialiasing = DefaultBoolean.Default;
		}
		bool ShouldSerializeTextAlignment() {
			return textAlignment != DefaultTextAlignment;
		}
		void ResetTextAlignment() {
			TextAlignment = DefaultTextAlignment;
		}
		bool ShouldSerializeAutoSize() {
			return autoSize != DefaultAutoSize;
		}
		void ResetAutoSize() {
			AutoSize = DefaultAutoSize;
		}
		bool ShouldSerializeText() {
			return this.text != DefaultText;
		}
		void ResetText() {
			Text = DefaultText;
		}
		bool ShouldSerializeLines() {
			return this.text != DefaultText;
		}
		void ResetLines() {
			Text = DefaultText;
		}
		bool ShouldSerializeTextColor() {
			return this.textColor != DefaultTextColor;
		}
		void ResetTextColor() {
			TextColor = DefaultTextColor;
		}
		bool ShouldSerializeFont() {
			return this.font != null && !this.font.Equals(DefaultFont);
		}
		void ResetFont() {
			Font = DefaultFont;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeText() ||
				ShouldSerializeTextColor() ||
				ShouldSerializeFont() ||
				ShouldSerializeEnableAntialiasing() ||
				ShouldSerializeTextAlignment() ||
				ShouldSerializeAutoSize();
		}
		#endregion
		protected override Size CalculateInnerSize() {
			StringInfo parsedText;
			using (TextMeasurer textMeasurer = new TextMeasurer()) {
				SizeF size = textMeasurer.MeasureString(text, font);
				parsedText = new StringInfo(text, this, size, textMeasurer, 0, 0, false);
			}			
			return new Size(MathUtils.Ceiling(parsedText.Bounds.Width), MathUtils.Ceiling(parsedText.Bounds.Height));
		}
		protected override ChartElement CreateObjectForClone() {
			return new TextAnnotation();
		}
		protected internal override AnnotationViewData CalculateViewData(TextMeasurer textMeasurer, AnnotationLayout shapeLayout, AnnotationLayout anchorPointLayout) {
			SaveLayout(shapeLayout.Position, anchorPointLayout.Position);
			if (!Visible || ChartContainer == null || ChartContainer.Chart == null)
				return null;
			int indexInRepository = ChartContainer.Chart.AnnotationRepository.IndexOf(this);
			return new TextAnnotationViewData(this, Shape, anchorPointLayout.RefinedPoint, anchorPointLayout.Position, indexInRepository, shapeLayout.Position, textMeasurer);
		}
		protected internal override AnnotationViewData CalculateViewData(TextMeasurer textMeasurer, AnnotationLayout shapeLayout, 
																		AnnotationLayout anchorPointLayout, Rectangle allowedBoundsForAnnotationPlacing) {
			SaveLayout(shapeLayout.Position, anchorPointLayout.Position);
			if (!Visible || ChartContainer == null || ChartContainer.Chart == null)
				return null;
			int indexInRepository = ChartContainer.Chart.AnnotationRepository.IndexOf(this);
			return new TextAnnotationViewData(this, Shape, anchorPointLayout.RefinedPoint, anchorPointLayout.Position, indexInRepository, shapeLayout.Position, allowedBoundsForAnnotationPlacing, textMeasurer);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			TextAnnotation annotation = obj as TextAnnotation;
			if (annotation == null)
				return;
			textAlignment = annotation.textAlignment;
			enableAntialiasing = annotation.enableAntialiasing;
			autoSize = annotation.autoSize;
			text = annotation.text;
			textColor = annotation.textColor;
			font = annotation.font;
		}
	}
}
