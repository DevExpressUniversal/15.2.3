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
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum RangeBarLabelKind {
		OneLabel,
		TwoLabels,
		MaxValueLabel,
		MinValueLabel
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum RangeBarLabelPosition {
		Outside,
		Inside,
		Center
	}   
	[
	TypeConverter(typeof(RangeBarSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RangeBarSeriesLabel : SeriesLabelBase {
		const RangeBarLabelKind DefaultLabelKind = RangeBarLabelKind.TwoLabels;
		const RangeBarLabelPosition DefaultLabelPosition = RangeBarLabelPosition.Outside;
		const int DefaultLabelIndent = 0;
		RangeBarLabelKind labelKind = DefaultLabelKind;
		RangeBarLabelPosition labelPosition = DefaultLabelPosition;
		int labelIndent = DefaultLabelIndent;
		protected internal override bool ShadowSupported { get { return true; } }
		protected internal override bool ConnectorSupported { get { return false; } }
		protected internal override bool ConnectorEnabled { get { return false; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new int LineLength { get { return 0; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new bool LineVisible { get { return false; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Color LineColor { get { return Color.Empty; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new LineStyle LineStyle { get { return null; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeBarSeriesLabelKind"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeBarSeriesLabel.Kind"),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public RangeBarLabelKind Kind {
			get { return this.labelKind; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.labelKind = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeBarSeriesLabelPosition"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeBarSeriesLabel.Position"),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public RangeBarLabelPosition Position {
			get { return this.labelPosition; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.labelPosition = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeBarSeriesLabelIndent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeBarSeriesLabel.Indent"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int Indent {
			get { return this.labelIndent; }
			set {
				if (value != this.labelIndent) {
					if (value < 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectRangeBarSeriesLabelIndent));
					SendNotification(new ElementWillChangeNotification(this));
					this.labelIndent = value;
					RaiseControlChanged();
				}
			}
		}
		public RangeBarSeriesLabel() : base() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "Kind")
				return ShouldSerializeKind();
			if(propertyName == "Position")
				return ShouldSerializePosition();
			if(propertyName == "Indent")
				return ShouldSerializeIndent();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeKind() {
			return this.labelKind != DefaultLabelKind;
		}
		void ResetKind() {
			Kind = DefaultLabelKind;
		}
		bool ShouldSerializePosition() {
			return this.labelPosition != DefaultLabelPosition;
		}
		void ResetPosition() {
			Position = DefaultLabelPosition;
		}
		bool ShouldSerializeIndent() {
			return this.labelIndent != DefaultLabelIndent;
		}
		void ResetIndent() {
			Indent = DefaultLabelIndent;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeKind() ||
				ShouldSerializePosition() ||
				ShouldSerializeIndent();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new RangeBarSeriesLabel();
		}		
		protected override string[] ConstructTexts(RefinedPoint refinedPoint) {
			string labelText = string.Empty;
			if (Formatter != null)
				labelText = Formatter.GetDataLabelText(refinedPoint);
			else {
				PatternParser patternParser;
				switch (Kind) {
					case RangeBarLabelKind.TwoLabels:
						string minValuePlaceholder = PatternUtils.GetMinValuePlaceholder(refinedPoint, (Scale)SeriesBase.ValueScaleType);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualTextPattern, PatternUtils.ValuePlaceholder, minValuePlaceholder), Series.View);
						patternParser.SetContext(refinedPoint, Series);
						string minText = patternParser.GetText();
						string maxValuePlaceholder = PatternUtils.GetMaxValuePlaceholder(refinedPoint, (Scale)SeriesBase.ValueScaleType);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualTextPattern, PatternUtils.ValuePlaceholder, maxValuePlaceholder), Series.View);
						patternParser.SetContext(refinedPoint, Series);
						string maxText = patternParser.GetText();
						return new string[] { minText, maxText };
					case RangeBarLabelKind.OneLabel:
						patternParser = new PatternParser(
							PatternUtils.ReplacePlaceholder(ActualTextPattern, PatternUtils.ValuePlaceholder, PatternUtils.Value1Placeholder, PatternUtils.Value2Placeholder),
							Series.View);
						break;
					case RangeBarLabelKind.MinValueLabel:
						minValuePlaceholder = PatternUtils.GetMinValuePlaceholder(refinedPoint, (Scale)SeriesBase.ValueScaleType);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualTextPattern, PatternUtils.ValuePlaceholder, minValuePlaceholder), Series.View);
						break;
					case RangeBarLabelKind.MaxValueLabel:
						maxValuePlaceholder = PatternUtils.GetMaxValuePlaceholder(refinedPoint, (Scale)SeriesBase.ValueScaleType);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualTextPattern, PatternUtils.ValuePlaceholder, maxValuePlaceholder), Series.View);
						break;
					default:
						ChartDebug.Fail("Unexpected RangeArea label kind.");
						return new string[0];
				}
				patternParser.SetContext(refinedPoint, Series);
				labelText = patternParser.GetText();
			}
			return new string[] { labelText };
		}
		protected internal override void CalculateLayout(SeriesLabelLayoutList labelLayoutList, RefinedPointData pointData, TextMeasurer textMeasurer) {
			XYDiagramSeriesLabelLayoutList xyLabelLayoutList = labelLayoutList as XYDiagramSeriesLabelLayoutList;
			if(xyLabelLayoutList == null) {
				ChartDebug.Fail("XYDiagramSeriesLabelsLayout expected.");
				return;
			}
			RangeBarSeriesView view = Series.View as RangeBarSeriesView;
			if(view == null) {
				ChartDebug.Fail("RangeBarSeriesView expected.");
				return;
			}
			BarData barData = pointData.GetBarData(view);
			Color color = pointData.DrawOptions.Color;
			if(Kind == RangeBarLabelKind.TwoLabels) {
				new MinRangeBarSeresLabelView(this).CalculateLayout(xyLabelLayoutList, textMeasurer, barData, pointData, pointData.LabelViewData[0], color);
				new MaxRangeBarSeresLabelView(this).CalculateLayout(xyLabelLayoutList, textMeasurer, barData, pointData, pointData.LabelViewData[1], color);
			}
			else
				RangeBarSeresLabelView.CreateInstance(this).CalculateLayout(xyLabelLayoutList, textMeasurer, barData, pointData, pointData.LabelViewData[0], color);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RangeBarSeriesLabel label = obj as RangeBarSeriesLabel;
			if (label == null)
				return;
			labelKind = label.labelKind;
			labelPosition = label.labelPosition;
			labelIndent = label.labelIndent;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public abstract class RangeBarSeresLabelView {
		public static RangeBarSeresLabelView CreateInstance(RangeBarSeriesLabel label) {
			switch (label.Kind) {
				case RangeBarLabelKind.OneLabel:
					return new OneRangeBarSeresLabelView(label);
				case RangeBarLabelKind.MinValueLabel:
					return new MinRangeBarSeresLabelView(label);
				case RangeBarLabelKind.MaxValueLabel:
					return new MaxRangeBarSeresLabelView(label);
				default:
					throw new DefaultSwitchException();
			}
		}
		readonly RangeBarSeriesLabel label;
		protected RangeBarSeriesLabel Label { get { return label; } }
		public RangeBarSeresLabelView(RangeBarSeriesLabel label) {
			this.label = label;
		}
		protected abstract XYDiagramSeriesLabelLayout CalculateLayoutInternal(XYDiagramSeriesLabelLayoutList labelLayoutList, BarData barData, RefinedPointData pointData, SeriesLabelViewData labelViewData, Color color, TextMeasurer textMeasurer);
		public void CalculateLayout(XYDiagramSeriesLabelLayoutList labelLayoutList, TextMeasurer textMeasurer, BarData barData, RefinedPointData pointData, SeriesLabelViewData labelViewData, Color color) {
			if (String.IsNullOrEmpty(labelViewData.Text))
				return;
			XYDiagramSeriesLabelLayout layout = CalculateLayoutInternal(labelLayoutList, barData, pointData, labelViewData, color, textMeasurer);
			if(layout != null)
				labelLayoutList.Add(layout);
		}
	}
	public abstract class MinMaxRangeBarSeresLabelView : RangeBarSeresLabelView {
		public MinMaxRangeBarSeresLabelView(RangeBarSeriesLabel label) : base(label) { }
		protected abstract double GetLabelPositionValue(BarData barData);
		protected abstract int GetLabelOffset();
		protected abstract double GetLabelAngle();
		protected override XYDiagramSeriesLabelLayout CalculateLayoutInternal(XYDiagramSeriesLabelLayoutList labelLayoutList, BarData barData, RefinedPointData pointData, SeriesLabelViewData labelViewData, Color color, TextMeasurer textMeasurer) {
			XYDiagramMappingBase diagramMapping = labelLayoutList.View.IsScrollingEnabled ?
				labelLayoutList.MappingContainer.MappingForScrolling : 
				barData.GetMappingForExtremeLabelPosition(labelLayoutList.MappingContainer, GetLabelPositionValue(barData));
			if(diagramMapping == null)
				return null;
			double angle = XYDiagramMappingHelper.CorrectAngle(diagramMapping, GetLabelAngle());
			DiagramPoint anchorPoint = barData.GetScreenPoint(barData.Argument, GetLabelPositionValue(barData), diagramMapping);
			anchorPoint = XYDiagramMappingHelper.ApplyYOffsetToPoint(diagramMapping, anchorPoint, GetLabelOffset());
			TextPainter painter = Label.Position == RangeBarLabelPosition.Center ?
				labelViewData.CreateTextPainterForCenterDrawing(Label, textMeasurer, anchorPoint) :
				labelViewData.CreateTextPainterForFlankDrawing(Label, textMeasurer, anchorPoint, angle);
			RectangleF validRectangle = RectangleF.Empty;
			if(Label.Position == RangeBarLabelPosition.Inside)
				validRectangle = barData.GetTotalRect(labelLayoutList.MappingContainer);
			return XYDiagramSeriesLabelLayout.CreateWithValidRectangle(pointData, color, painter, null,
				Label.ResolveOverlappingMode, anchorPoint, validRectangle);
		}
	}
	public class MinRangeBarSeresLabelView : MinMaxRangeBarSeresLabelView {
		public MinRangeBarSeresLabelView(RangeBarSeriesLabel label) : base(label) {
		}
		protected override double GetLabelPositionValue(BarData barData) {
			if (barData.ZeroValue < barData.ActualValue)
				return barData.ZeroValue;
			else
				return barData.ActualValue;
		}
		protected override int GetLabelOffset() {
			switch (Label.Position) {
				case RangeBarLabelPosition.Center:
					return 0;
				case RangeBarLabelPosition.Inside:
					return Label.Indent;
				case RangeBarLabelPosition.Outside:
					return -Label.Indent;
				default:
					throw new DefaultSwitchException();
			}
		}
		protected override double GetLabelAngle() {
			switch (Label.Position) {
				case RangeBarLabelPosition.Center:
					return 0;
				case RangeBarLabelPosition.Inside:
					return Math.PI / 2.0;
				case RangeBarLabelPosition.Outside:
					return -Math.PI / 2.0;
				default:
					throw new DefaultSwitchException();
			}
		}
	}
	public class MaxRangeBarSeresLabelView : MinMaxRangeBarSeresLabelView {
		public MaxRangeBarSeresLabelView(RangeBarSeriesLabel label) : base(label) {
		}
		protected override double GetLabelPositionValue(BarData barData) {
			if (barData.ActualValue > barData.ZeroValue)
				return barData.ActualValue;
			else
				return barData.ZeroValue;
		}
		protected override int GetLabelOffset() {
			switch (Label.Position) {
				case RangeBarLabelPosition.Center:
					return 0;
				case RangeBarLabelPosition.Inside:
					return -Label.Indent;
				case RangeBarLabelPosition.Outside:
					return Label.Indent;
				default:
					throw new DefaultSwitchException();
			}
		}
		protected override double GetLabelAngle() {
			switch (Label.Position) {
				case RangeBarLabelPosition.Center:
					return 0;
				case RangeBarLabelPosition.Inside:
					return -Math.PI / 2.0;
				case RangeBarLabelPosition.Outside:
					return Math.PI / 2.0;
				default:
					throw new DefaultSwitchException();
			}
		}
	}
	public class OneRangeBarSeresLabelView : RangeBarSeresLabelView {
		public OneRangeBarSeresLabelView(RangeBarSeriesLabel label) : base(label) {
		}
		protected override XYDiagramSeriesLabelLayout CalculateLayoutInternal(XYDiagramSeriesLabelLayoutList labelLayoutList, BarData barData, RefinedPointData pointData, SeriesLabelViewData labelViewData, Color color, TextMeasurer textMeasurer) {
			return SeriesLabelHelper.CalculateLayoutForCenterBarPosition(labelLayoutList, textMeasurer, barData, pointData, labelViewData, color);
		}
	}
}
