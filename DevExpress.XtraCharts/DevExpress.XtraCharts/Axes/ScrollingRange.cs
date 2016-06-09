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
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ScrollingRange : ChartElement {
		#region fields and properties
		readonly AxisRangeData data;
		RangeDataBase.Deserializer wholeRangeDeserializer;
		RangeDataBase.Deserializer Deserializer {
			get {
				if (wholeRangeDeserializer == null)
					wholeRangeDeserializer = new RangeDataBase.Deserializer(Range.Axis);
				return wholeRangeDeserializer;
			}
		}
		bool DesignMode { get { return Range != null && ChartContainer != null && ChartContainer.DesignMode; } }
		bool ShouldSerializeProperties { get { return ChartContainer == null || ChartContainer.ControlType == ChartContainerType.WinControl; } }
		internal AxisRange Range { get { return (AxisRange)Owner; } }
		internal AxisRangeData Data { get { return data; } }
		[
		Obsolete("This property is obsolete now. Use AxisBase.WholeRange.Auto instead."),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollingRangeAuto"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollingRange.Auto"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		NonTestableProperty
		]
		public bool Auto {
			get { return data.Auto; }
			set {
				if (Loading)
					Deserializer.CorrectionMode = value ? RangeCorrectionMode.Auto : Range.Axis.VisualRangeData.CorrectionMode;
				data.Auto = value;
			}
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.WholeRange.MinValue instead."),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollingRangeMinValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollingRange.MinValue"),
		TypeConverter(typeof(AxisRangeMinValueTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public object MinValue {
			get { return data.MinValue; }
			set { data.MinValue = value; }
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.WholeRange.MaxValue instead."),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollingRangeMaxValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollingRange.MaxValue"),
		TypeConverter(typeof(AxisRangeMaxValueTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public object MaxValue {
			get { return data.MaxValue; }
			set { data.MaxValue = value; }
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.WholeRange.MinValueInternal instead."),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollingRangeMinValueInternal"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollingRange.MinValueInternal"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public double MinValueInternal {
			get { return data.MinValueInternal; }
			set {
				data.MinValueInternal = value;
				if (Loading)
					Deserializer.MinInternal = value;
			}
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.WholeRange.MaxValueInternal instead."),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollingRangeMaxValueInternal"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollingRange.MaxValueInternal"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public double MaxValueInternal {
			get { return data.MaxValueInternal; }
			set {
				data.MaxValueInternal = value;
				if (Loading)
					Deserializer.MaxInternal = value;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty(),
		XtraSerializableProperty
		]
		public string MinValueSerializable {
			get { return SerializingUtils.ConvertToSerializable(data.MinValue); }
			set { Deserializer.MinValueSerializable = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty(),
		XtraSerializableProperty
		]
		public string MaxValueSerializable {
			get { return SerializingUtils.ConvertToSerializable(data.MaxValue); }
			set { Deserializer.MaxValueSerializable = value; }
		}
		[		
		Obsolete("This property is obsolete now. Instead, set the ScrollingRange.MinValue property to 0."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public bool AlwaysShowZeroLevel {
			get { return Range.Axis.WholeRangeData.AlwaysShowZeroLevel; }
			set { }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollingRangeSideMarginsEnabled"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollingRange.SideMarginsEnabled"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		NonTestableProperty(),
		]
		public bool SideMarginsEnabled {
			get { return Range.Axis.WholeRangeData.AutoSideMargins == SideMarginMode.Enable || Range.Axis.WholeRangeData.AutoSideMargins == SideMarginMode.UserEnable; }
			set {
				Range.Axis.WholeRangeData.AutoSideMargins = value ? SideMarginMode.UserEnable : SideMarginMode.UserDisable;
				if (Loading)
					Deserializer.AutoSideMargins = value;
			}
		}
		#endregion
		internal ScrollingRange(AxisRange range)
			: base(range) {
			data = new AxisRangeDataEx(this);
		}
		internal ScrollingRange(AxisRange range, RangeDataBase axisRangeBase)
			: base(range) {
			data = new AxisRangeDataEx(range);
		}
		#region ShouldSerialize & Reset
		internal bool ShouldSerializeMinValueInternal() {
			return Range.Axis.WholeRangeData.CorrectionMode == RangeCorrectionMode.InternalValues;
		}
		internal bool ShouldSerializeMaxValueInternal() {
			return Range.Axis.WholeRangeData.CorrectionMode == RangeCorrectionMode.InternalValues;
		}
		bool ShouldSerializeAuto() {
			return Range.Axis.WholeRangeData.CorrectionMode != RangeCorrectionMode.Auto;
		}
		bool ShouldSerializeMinValueSerializable() {
			if (Range.Axis.WholeRangeData.CorrectionMode == RangeCorrectionMode.Values) {
				return !Range.Axis.WholeRangeData.AutoCorrectMin;
			}
			return false;
		}
		bool ShouldSerializeMaxValueSerializable() {
			if (Range.Axis.WholeRangeData.CorrectionMode == RangeCorrectionMode.Values) {
				return !Range.Axis.WholeRangeData.AutoCorrectMax;
			}
			return false;
		}
		bool ShouldSerializeAlwaysShowZeroLevel() {
			return false;
		}
		bool ShouldSerializeSideMarginsEnabled() {
			return Range.Axis.WholeRangeData.AutoSideMargins != SideMarginMode.Enable && Range.Axis.WholeRangeData.AutoSideMargins != SideMarginMode.UserEnable;
		}
		protected internal override bool ShouldSerialize() {
			return ShouldSerializeProperties && data.ShouldSerialize();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Auto":
					return ShouldSerializeAuto();
				case "MinValueSerializable":
					return ShouldSerializeMaxValueSerializable();
				case "MaxValueSerializable":
					return ShouldSerializeMaxValueSerializable();
				case "MinValueInternal":
					return ShouldSerializeMinValueInternal();
				case "MaxValueInternal":
					return ShouldSerializeMaxValueInternal();
				case "AlwaysShowZeroLevel":
					return ShouldSerializeAlwaysShowZeroLevel();
				case "SideMarginsEnables":
					return ShouldSerializeSideMarginsEnabled();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		internal RangeDataBase.Deserializer GetDeserializer() { return wholeRangeDeserializer; }
		bool IsScrollingEnable() {
			XYDiagram diagram = Range.Axis.Diagram as XYDiagram;
			if (diagram != null) {
				if (Range.Axis.IsValuesAxis)
					return diagram.EnableAxisYScrolling;
				else
					return diagram.EnableAxisXScrolling;
			}
			else {
				SwiftPlotDiagram swiftPlotDiagram = Range.Axis.Diagram as SwiftPlotDiagram;
				if (swiftPlotDiagram != null) {
					if (Range.Axis.IsValuesAxis)
						return swiftPlotDiagram.EnableAxisYScrolling;
					else
						return swiftPlotDiagram.EnableAxisXScrolling;
				}
			}
			return false;
		}
		protected override ChartElement CreateObjectForClone() {
			return new ScrollingRange(null);
		}
		public void SetMinMaxValues(object minValue, object maxValue) {
			((WholeRangeData)Range.Axis.WholeRangeData).SetRange(minValue, maxValue, double.NaN, double.NaN, true);
			data.SetMinMaxValues(minValue, maxValue);
		}
		public void SetInternalMinMaxValues(double min, double max) {
			IAxisData axis = Range.Axis as IAxisData;
			if (axis != null) {
				double minValue = min;
				double maxValue = max;
				if (axis.AxisScaleTypeMap.ScaleType == ActualScaleType.DateTime) {
					minValue += ((AxisDateTimeMap)Range.Axis.ScaleTypeMap).Min;
					maxValue += ((AxisDateTimeMap)Range.Axis.ScaleTypeMap).Min;
				}
				if (IsScrollingEnable()) {
					((WholeRangeData)Range.Axis.WholeRangeData).SetRange(null, null, minValue, maxValue, true);
				}
				else {
					((WholeRangeData)Range.Axis.WholeRangeData).SetRange(null, null, minValue, maxValue, true);
					((VisualRangeData)Range.Axis.VisualRangeData).SetRange(null, null, minValue, maxValue, true);
				}
			}
			data.SetInternalMinMaxValues(min, max);
		}
		public override void Assign(ChartElement obj) {
			ScrollingRange range = obj as ScrollingRange;
			if (range != null)
				data.Assign(range.data);
		}
	}
}
