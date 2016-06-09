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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(ExpandableObjectConverter))
	]
	public class AxisLabel : TitleBase, IAxisLabel, IHitTest, IPatternHolder {
		const StringAlignment DefaultTextAlignment = StringAlignment.Center;
		const AxisLabelResolveOverlappingMode DefaultResolveOverlappingMode = AxisLabelResolveOverlappingMode.None;
		const int DefaultAngle = 0;
		const int DefaultMaxWidth = 0;
		const int DefaultMaxLineCount = 0;
		const bool DefaultStaggered = false;
		internal const int DefaultResolveOverlappingMinIndent = -1;
		static readonly Color DefaultBackColor = Color.Empty;
		int angle = DefaultAngle;
		int maxWidth = DefaultMaxWidth;
		int maxLineCount = DefaultMaxLineCount;
		bool staggered = false;
		string beginText = String.Empty;
		string endText = String.Empty;
		StringAlignment textAlignment = DefaultTextAlignment;
		AxisLabelResolveOverlappingMode resolveOverlappingMode = DefaultResolveOverlappingMode;
		AxisLabelResolveOverlappingOptions resolveOverlappingOptions;
		NumericOptions numericOptions;
		DateTimeOptions dateTimeOptions;
		IAxisLabelFormatter formatter;
		string textPattern;
		bool textPatternSyncronized = false;
		Color backColor = DefaultBackColor;
		readonly RectangleFillStyle fillStyle;
		readonly RectangularBorder border;
		Diagram Diagram { get { return Axis != null ? Axis.Diagram : null; } }
		protected internal AxisBase Axis { get { return (AxisBase)base.Owner; } }		
		protected override bool DefaultVisible { get { return true; } }
		protected override Font DefaultFont { get { return DefaultFonts.Tahoma8; } }
		protected override bool DefaultAntialiasing { get { return Angle % 90 != 0; } }
		internal string ActualTextPattern { get { return !string.IsNullOrEmpty(textPattern) ? textPattern : PatternUtils.ConstructDefaultPattern(Axis); } }
		internal IAxisLabelFormatter Formatter {
			get { return formatter; } 
			set {
				if (value != formatter) {
					SendNotification(new ElementWillChangeNotification(this));
					formatter = value;
					RaiseControlChanged();
				}
			}
		}
		#region obsolete properties
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the ResolveOverlappingOptions property instead."),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public int ResolveOverlappingMinIndent { get; set; }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the ResolveOverlappingOptions property instead."),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public AxisLabelResolveOverlappingMode ResolveOverlappingMode { get; set; }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the TextPattern property instead."),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string BeginText {
			get { return beginText; }
			set {
				if (value != beginText) {
					SendNotification(new ElementWillChangeNotification(this));
					beginText = value;
					UpdateTextPattern();
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the TextPattern property instead."),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string EndText {
			get { return endText; }
			set {
				if (value != endText) {
					SendNotification(new ElementWillChangeNotification(this));
					endText = value;
					UpdateTextPattern();
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the TextPattern property instead."),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public NumericOptions NumericOptions { get { return numericOptions; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the TextPattern property instead."),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DateTimeOptions DateTimeOptions { get { return dateTimeOptions; } }
		#endregion
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelAngle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabel.Angle"),
		Localizable(true),		
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Angle {
			get { return angle; }
			set {
				if (value < -360 || value > 360)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLabelAngle));
				if(value != angle) {
					SendNotification(new ElementWillChangeNotification(this));
					angle = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelStaggered"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabel.Staggered"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool Staggered {
			get { return staggered; }
			set {
				if (value != staggered) {
					SendNotification(new ElementWillChangeNotification(this));
					staggered = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelMaxWidth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabel.MaxWidth"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int MaxWidth {
			get { return maxWidth; }
			set {
				if (value < 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLabelMaxWidth));
				if (value != maxWidth) {
					SendNotification(new ElementWillChangeNotification(this));
					maxWidth = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelMaxLineCount"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabel.MaxLineCount"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int MaxLineCount {
			get { return maxLineCount; }
			set {
				if (value < 0 || value > 20)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectMaxLineCount));
				if (value != maxLineCount) {
					SendNotification(new ElementWillChangeNotification(this));
					maxLineCount = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelTextAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabel.TextAlignment"),
		Category(Categories.Behavior),
		TypeConverter(typeof(StringAlignmentTypeConvertor)),
		Localizable(true),
		XtraSerializableProperty
		]
		public StringAlignment TextAlignment {
			get { return textAlignment; }
			set {
				if (value != textAlignment) {
					SendNotification(new ElementWillChangeNotification(this));
					textAlignment = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelResolveOverlappingOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabel.ResolveOverlappingOptions"),
		Category(Categories.Behavior),
		NestedTagProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public AxisLabelResolveOverlappingOptions ResolveOverlappingOptions {
			get {
				return resolveOverlappingOptions;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelTextPattern"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabel.TextPattern"),
		Category(Categories.Behavior),
		Editor("DevExpress.XtraCharts.Design.AxisLabelPatternEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty]
		public string TextPattern {
			get { return textPattern; }
			set {
				if (value != textPattern) {
					SendNotification(new ElementWillChangeNotification(this));
					textPattern = value;
					RaiseControlChanged();
					textPatternSyncronized = false;
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabel.BackColor"),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public Color BackColor {
			get { return backColor; }
			set {
				if (value != backColor) {
					SendNotification(new ElementWillChangeNotification(this));
					backColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabel.FillStyle"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleFillStyle FillStyle {
			get { return fillStyle; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelBorder"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabel.Border"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangularBorder Border {
			get { return border; }
		}
		internal AxisLabel(AxisBase axis) : base(axis) {
			numericOptions = new NumericOptions(this);
			dateTimeOptions = new DateTimeOptions(this);
			resolveOverlappingOptions = new AxisLabelResolveOverlappingOptions();
			resolveOverlappingOptions.Owner = this;
			fillStyle = new RectangleFillStyle(this, Color.Empty, FillMode.Solid);
			border = new InsideRectangularBorder(this, false, Color.Empty);
		}
		#region IAxisLabel implementation
		IAxisLabelResolveOverlappingOptions IAxisLabel.ResolveOverlappingOptions { get { return ResolveOverlappingOptions; } }
		IAxisLabelFormatterCore IAxisLabel.Formatter {
			get { return Formatter; }
			set { Formatter = (IAxisLabelFormatter)value; }
		}
		string IAxisLabel.TextPattern {
			get { return ActualTextPattern; }
		}
		#endregion
		#region IHitTest implementation
		object IHitTest.Object { get { return Axis; } }
		HitTestState IHitTest.State { get { return ((IHitTest)Axis).State; } }
		#endregion
		#region IPatternHolder implementation
		PatternDataProvider IPatternHolder.GetDataProvider(PatternConstants patternConstant) { return new AxisPatternDataProvider(); }
		string IPatternHolder.PointPattern { get { return ActualTextPattern; } }
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeAngle() {
			return angle != 0;
		}
		void ResetAngle() {
			Angle = DefaultAngle;
		}
		bool ShouldSerializeStaggered() {
			return staggered;
		}
		void ResetStaggered() {
			Staggered = DefaultStaggered;
		}
		bool ShouldSerializeMaxWidth() {
			return maxWidth != 0;
		}
		void ResetMaxWidth() {
			MaxWidth = DefaultMaxWidth;
		}
		bool ShouldSerializeMaxLineCount() {
			return maxLineCount > 0;
		}
		void ResetMaxLineCount() {
			MaxLineCount = DefaultMaxLineCount;
		}
		bool ShouldSerializeTextAlignment() {
			return textAlignment != DefaultTextAlignment;
		}
		void ResetTextAlignment() {
			TextAlignment = DefaultTextAlignment;
		}
		bool ShouldSerializeResolveOverlappingMode() {
			return resolveOverlappingMode != DefaultResolveOverlappingMode;
		}
		bool ShouldSerializeResolveOverlappingOptions() {
			return resolveOverlappingOptions.ShouldSerialize();
		}
		bool ShouldSerializeNumericOptions() {
			return false;
		}
		bool ShouldSerializeDateTimeOptions() {
			return false;
		}
		bool ShouldSerializeTextPattern() {
			return !String.IsNullOrEmpty(textPattern);
		}
		void ResetTextPattern() {
			TextPattern = string.Empty;
		}
		bool ShouldSerializeBackColor() {
			return backColor != DefaultBackColor;
		}
		void ResetBackColor() {
			BackColor = DefaultBackColor;
		}
		bool ShouldSerializeFillStyle() {
			return fillStyle.ShouldSerialize();
		}
		bool ShouldSerializeBorder() {
			return Border.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize()
				|| ShouldSerializeAngle()
				|| ShouldSerializeStaggered()
				|| ShouldSerializeMaxWidth()
				|| ShouldSerializeMaxLineCount()
				|| ShouldSerializeTextAlignment()
				|| ShouldSerializeResolveOverlappingMode()
				|| ShouldSerializeResolveOverlappingOptions()
				|| ShouldSerializeDateTimeOptions()
				|| ShouldSerializeNumericOptions()
				|| ShouldSerializeTextPattern()
				|| ShouldSerializeBackColor()
				|| ShouldSerializeBorder()
				|| ShouldSerializeFillStyle();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Angle":
					return ShouldSerializeAngle();
				case "Staggered":
					return ShouldSerializeStaggered();
				case "MaxWidth":
					return ShouldSerializeMaxWidth();
				case "MaxLineCount":
					return ShouldSerializeMaxLineCount();
				case "TextAlignment":
					return ShouldSerializeTextAlignment();
				case "ResolveOverlappingMode":
					return ShouldSerializeResolveOverlappingMode();
				case "ResolveOverlappingOptions":
					return ShouldSerializeResolveOverlappingOptions();
				case "NumericOptions":
					return ShouldSerializeNumericOptions();
				case "DateTimeOptions":
					return ShouldSerializeDateTimeOptions();
				case "TextPattern":
					return ShouldSerializeTextPattern();
				case "BackColor":
					return ShouldSerializeBackColor();
				case "FillStyle":
					return ShouldSerializeFillStyle();
				case "Border":
					return ShouldSerializeBorder();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		void UpdateTextPattern() {
			textPattern = beginText + "{" + (Axis.IsValuesAxis ? PatternUtils.ValuePlaceholder : PatternUtils.ArgumentPlaceholder);
			if (Axis.ScaleType == ActualScaleType.Numerical)
				textPattern += ":" + NumericOptionsHelper.GetFormatString(numericOptions);
			if (Axis.ScaleType == ActualScaleType.DateTime)
				textPattern += ":" + DateTimeOptionsHelper.GetFormatString(dateTimeOptions);
			textPattern = textPattern + "}" + endText;
			textPatternSyncronized = true;
		}
		internal void SyncronizeTextPatterWithScaleType() {
			if (textPatternSyncronized)
				UpdateTextPattern();
		}
		protected override ChartElement CreateObjectForClone() {
			return new AxisLabel(null);
		}
		protected override Color GetTextColor(IChartAppearance actualAppearance) {
			return actualAppearance.XYDiagramAppearance.LabelsColor;
		}
		protected override bool ProcessChanged(ChartElement sender, ChartUpdateInfoBase changeInfo) {
			if (sender == numericOptions || sender == dateTimeOptions)
				UpdateTextPattern();
			return base.ProcessChanged(sender, changeInfo);
		}
		public override string ToString() {
			return "(AxisLabel)";
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			AxisLabel label = obj as AxisLabel;
			if (label != null) {
				resolveOverlappingOptions.Assign(label.resolveOverlappingOptions);
				angle = label.angle;
				maxWidth = label.maxWidth;
				maxLineCount = label.maxLineCount;
				textAlignment = label.textAlignment;
				staggered = label.staggered;
				resolveOverlappingMode = label.resolveOverlappingMode;
				textPattern = label.textPattern;
				backColor = label.backColor;
				fillStyle.Assign(label.fillStyle);
				border.Assign(label.border);
			}
		}
	}
}
