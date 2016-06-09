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
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum RangeAreaLabelKind {
		OneLabel,
		TwoLabels,
		MaxValueLabel,
		MinValueLabel,
		Value1Label,
		Value2Label
	}
	[
	TypeConverter(typeof(RangeAreaSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RangeAreaSeriesLabel : SeriesLabelBase {
		const RangeAreaLabelKind DefaultLabelKind = RangeAreaLabelKind.TwoLabels;
		const int DefaultMinValueAngle = 270;
		const int DefaultMaxValueAngle = 90;
		RangeAreaLabelKind labelKind = DefaultLabelKind;
		int minValueAngle = DefaultMinValueAngle;
		int maxValueAngle = DefaultMaxValueAngle;
		protected internal override bool ShadowSupported { get { return true; } }
		protected internal override bool ConnectorSupported { get { return true; } }
		protected internal override bool ConnectorEnabled { get { return Kind != RangeAreaLabelKind.OneLabel; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeAreaSeriesLabelKind"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeAreaSeriesLabel.Kind"),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public RangeAreaLabelKind Kind {
			get { return this.labelKind; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.labelKind = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeAreaSeriesLabelMinValueAngle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeAreaSeriesLabel.MinValueAngle"),
		Category(Categories.Behavior),
		Localizable(true),
		XtraSerializableProperty
		]
		public int MinValueAngle {
			get { return minValueAngle; }
			set {
				if (value != minValueAngle) {
					CheckAngleValue(value);
					SendNotification(new ElementWillChangeNotification(this));
					minValueAngle = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeAreaSeriesLabelMaxValueAngle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeAreaSeriesLabel.MaxValueAngle"),
		Category(Categories.Behavior),
		Localizable(true),
		XtraSerializableProperty
		]
		public int MaxValueAngle {
			get { return maxValueAngle; }
			set {
				if (value != maxValueAngle) {
					CheckAngleValue(value);
					SendNotification(new ElementWillChangeNotification(this));
					maxValueAngle = value;
					RaiseControlChanged();
				}
			}
		}
		public RangeAreaSeriesLabel()
			: base() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Kind")
				return ShouldSerializeKind();
			if (propertyName == "MinValueAngle")
				return ShouldSerializeMinValueAngle();
			if (propertyName == "MaxValueAngle")
				return ShouldSerializeMaxValueAngle();
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
		bool ShouldSerializeMinValueAngle() {
			return minValueAngle != DefaultMinValueAngle;
		}
		void ResetMinValueAngle() {
			MinValueAngle = DefaultMinValueAngle;
		}
		bool ShouldSerializeMaxValueAngle() {
			return maxValueAngle != DefaultMaxValueAngle;
		}
		void ResetMaxValueAngle() {
			MaxValueAngle = DefaultMaxValueAngle;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeMinValueAngle() ||
				ShouldSerializeMaxValueAngle() ||
				ShouldSerializeKind();
		}
		#endregion
		void CheckAngleValue(int value) {
			if (value < -360 || value > 360)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLabelAngle));
		}
		protected override ChartElement CreateObjectForClone() {
			return new RangeAreaSeriesLabel();
		}
		protected internal override bool CheckResolveOverlappingMode(ResolveOverlappingMode mode) {
			if (Kind != RangeAreaLabelKind.OneLabel)
				return true;
			return mode != ResolveOverlappingMode.JustifyAllAroundPoint && mode != ResolveOverlappingMode.JustifyAroundPoint;
		}
		protected override string[] ConstructTexts(RefinedPoint refinedPoint) {
			string labelText = string.Empty;
			if (Formatter != null)
				labelText = Formatter.GetDataLabelText(refinedPoint);
			else {
				PatternParser patternParser;
				switch (Kind) {
					case RangeAreaLabelKind.TwoLabels:
						string minValuePlaceholder = PatternUtils.GetMinValuePlaceholder(refinedPoint, (Scale)SeriesBase.ValueScaleType);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualTextPattern, PatternUtils.ValuePlaceholder, minValuePlaceholder), Series.View);
						patternParser.SetContext(refinedPoint, Series);
						string minText = patternParser.GetText();
						string maxValuePlaceholder = PatternUtils.GetMaxValuePlaceholder(refinedPoint, (Scale)SeriesBase.ValueScaleType);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualTextPattern, PatternUtils.ValuePlaceholder, maxValuePlaceholder), Series.View);
						patternParser.SetContext(refinedPoint, Series);
						string maxText = patternParser.GetText();
						return new string[] { minText, maxText };
					case RangeAreaLabelKind.OneLabel:
						patternParser = new PatternParser(
							PatternUtils.ReplacePlaceholder(ActualTextPattern, PatternUtils.ValuePlaceholder, PatternUtils.Value1Placeholder, PatternUtils.Value2Placeholder),
							Series.View);
						break;
					case RangeAreaLabelKind.Value1Label:
						patternParser = new PatternParser(
							PatternUtils.ReplacePlaceholder(ActualTextPattern, PatternUtils.ValuePlaceholder, PatternUtils.Value1Placeholder),
							Series.View);
						break;
					case RangeAreaLabelKind.Value2Label:
						patternParser = new PatternParser(
							PatternUtils.ReplacePlaceholder(ActualTextPattern, PatternUtils.ValuePlaceholder, PatternUtils.Value2Placeholder),
							Series.View);
						break;
					case RangeAreaLabelKind.MinValueLabel:
						minValuePlaceholder = PatternUtils.GetMinValuePlaceholder(refinedPoint, (Scale)SeriesBase.ValueScaleType);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualTextPattern, PatternUtils.ValuePlaceholder, minValuePlaceholder), Series.View);
						break;
					case RangeAreaLabelKind.MaxValueLabel:
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
			if (xyLabelLayoutList == null) {
				ChartDebug.Fail("XYDiagramSeriesLabelsLayout expected.");
				return;
			}
			RangeAreaSeriesView view = Series.View as RangeAreaSeriesView;
			if (view == null) {
				ChartDebug.Fail("RangeAreaSeriesView expected.");
				return;
			}
			MinMaxValues values = view.GetSeriesPointValues(pointData.RefinedPoint);
			bool increasingValuesOrder = values.Max >= values.Min;
			if (Kind == RangeAreaLabelKind.TwoLabels) {
				double minValue = increasingValuesOrder ? values.Min : values.Max;
				XYDiagramSeriesLabelLayout minLabelLayout = SeriesLabelHelper.CalculateLayoutForPoint(xyLabelLayoutList, textMeasurer, pointData, pointData.LabelViewData[0], this, minValue, minValueAngle);
				if (minLabelLayout != null)
					labelLayoutList.Add(minLabelLayout);
				double maxValue = increasingValuesOrder ? values.Max : values.Min;
				XYDiagramSeriesLabelLayout maxLabelLayout = SeriesLabelHelper.CalculateLayoutForPoint(xyLabelLayoutList, textMeasurer, pointData, pointData.LabelViewData[1], this, maxValue, maxValueAngle);
				if (maxLabelLayout != null)
					labelLayoutList.Add(maxLabelLayout);
				return;
			}
			XYDiagramSeriesLabelLayout layout = null;
			switch (Kind) {
				case RangeAreaLabelKind.OneLabel:
					layout = SeriesLabelHelper.CalculateLayoutForCenterAreaPosition(xyLabelLayoutList, textMeasurer, pointData, this, values);
					break;
				case RangeAreaLabelKind.MaxValueLabel:
					double value = increasingValuesOrder ? values.Max : values.Min;
					layout = SeriesLabelHelper.CalculateLayoutForPoint(xyLabelLayoutList, textMeasurer, pointData, pointData.LabelViewData[0], this, value, maxValueAngle);
					break;
				case RangeAreaLabelKind.MinValueLabel:
					value = increasingValuesOrder ? values.Min : values.Max;
					layout = SeriesLabelHelper.CalculateLayoutForPoint(xyLabelLayoutList, textMeasurer, pointData, pointData.LabelViewData[0], this, value, minValueAngle);
					break;
				case RangeAreaLabelKind.Value1Label:
					int angle = increasingValuesOrder ? minValueAngle : maxValueAngle;
					layout = SeriesLabelHelper.CalculateLayoutForPoint(xyLabelLayoutList, textMeasurer, pointData, pointData.LabelViewData[0], this, values.Min, angle);
					break;
				case RangeAreaLabelKind.Value2Label:
					angle = increasingValuesOrder ? maxValueAngle : minValueAngle;
					layout = SeriesLabelHelper.CalculateLayoutForPoint(xyLabelLayoutList, textMeasurer, pointData, pointData.LabelViewData[0], this, values.Max, angle);
					break;
				default:
					ChartDebug.Fail("Unexpected RangeArea label kind.");
					return;
			}
			if (layout != null)
				labelLayoutList.Add(layout);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RangeAreaSeriesLabel label = obj as RangeAreaSeriesLabel;
			if (label == null)
				return;
			labelKind = label.labelKind;
			minValueAngle = label.minValueAngle;
			maxValueAngle = label.maxValueAngle;
		}
	}
	[
	TypeConverter(typeof(RangeArea3DSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RangeArea3DSeriesLabel : RangeAreaSeriesLabel {
		protected internal override bool ShadowSupported { get { return false; } }
		protected internal override bool ConnectorEnabled { get { return Kind == RangeAreaLabelKind.MinValueLabel || Kind == RangeAreaLabelKind.MaxValueLabel; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Shadow Shadow { get { return base.Shadow; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new int MinValueAngle { get { return base.MinValueAngle; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new int MaxValueAngle { get { return base.MaxValueAngle; } }
		public RangeArea3DSeriesLabel()
			: base() {
		}
		protected internal override bool CheckResolveOverlappingMode(ResolveOverlappingMode mode) {
			return mode != ResolveOverlappingMode.JustifyAllAroundPoint && mode != ResolveOverlappingMode.JustifyAroundPoint;
		}
		protected override ChartElement CreateObjectForClone() {
			return new RangeArea3DSeriesLabel();
		}
		protected internal override void CalculateLayout(SeriesLabelLayoutList labelLayoutList, RefinedPointData pointData, TextMeasurer textMeasurer) {
			XYDiagram3DSeriesLabelLayoutList xyLabelLayoutList = labelLayoutList as XYDiagram3DSeriesLabelLayoutList;
			if (xyLabelLayoutList == null) {
				ChartDebug.Fail("XYDiagram3DSeriesLabelsLayout expected.");
				return;
			}
			RangeArea3DSeriesView view = Series.View as RangeArea3DSeriesView;
			if (view == null) {
				ChartDebug.Fail("RangeArea3DSeriesView expected.");
				return;
			}
			MinMaxValues values = view.GetSeriesPointValues(pointData.RefinedPoint);
			bool increasingValuesOrder = values.Max >= values.Min;
			switch (Kind) {
				case RangeAreaLabelKind.TwoLabels:
					double minValue = increasingValuesOrder ? values.Min : values.Max;
					double maxValue = increasingValuesOrder ? values.Max : values.Min;
					SeriesLabelHelper.CalculateLayoutForStackedArea3DLabel(xyLabelLayoutList, textMeasurer, pointData, pointData.LabelViewData[0], values, minValue);
					SeriesLabelHelper.CalculateLayoutForStackedArea3DLabel(xyLabelLayoutList, textMeasurer, pointData, pointData.LabelViewData[1], values, maxValue);
					break;
				case RangeAreaLabelKind.OneLabel:
					double labelPosition = MinMaxValues.Intersection(xyLabelLayoutList.AxisRangeY, values).CalculateCenter();
					SeriesLabelHelper.CalculateLayoutForStackedArea3DLabel(xyLabelLayoutList, textMeasurer, pointData, pointData.LabelViewData[0], values, labelPosition);
					break;
				case RangeAreaLabelKind.MaxValueLabel:
					double angle = Math.PI / 2.0;
					DiagramVector direction = new DiagramVector(0, 1, 0);
					maxValue = increasingValuesOrder ? values.Max : values.Min;
					SeriesLabelHelper.CalculateLayoutForLine3DLabel(xyLabelLayoutList, textMeasurer, pointData, maxValue, angle, direction);
					break;
				case RangeAreaLabelKind.MinValueLabel:
					angle = 3 * Math.PI / 2.0;
					direction = new DiagramVector(0, -1, 0);
					minValue = increasingValuesOrder ? values.Min : values.Max;
					SeriesLabelHelper.CalculateLayoutForLine3DLabel(xyLabelLayoutList, textMeasurer, pointData, minValue, angle, direction);
					break;
				case RangeAreaLabelKind.Value1Label:
					SeriesLabelHelper.CalculateLayoutForStackedArea3DLabel(xyLabelLayoutList, textMeasurer, pointData, pointData.LabelViewData[0], values, values.Min);
					break;
				case RangeAreaLabelKind.Value2Label:
					SeriesLabelHelper.CalculateLayoutForStackedArea3DLabel(xyLabelLayoutList, textMeasurer, pointData, pointData.LabelViewData[0], values, values.Max);
					break;
				default:
					ChartDebug.Fail("Unexpected RangeArea label kind.");
					break;
			}
		}
	}
}
