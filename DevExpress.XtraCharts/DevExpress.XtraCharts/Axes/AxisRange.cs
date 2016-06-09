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
	public abstract class AxisRange : ChartElement {
		readonly AxisRangeData data;
		internal readonly ScrollingRange scrollingRange;
		RangeDataBase.Deserializer visualRangeDeserializer;
		RangeDataBase.Deserializer Deserializer {
			get {
				if (visualRangeDeserializer == null)
					visualRangeDeserializer = new VisualRangeData.VisualRangeDeserializer(Axis);
				return visualRangeDeserializer;
			}
		}
		internal AxisBase Axis { get { return (AxisBase)base.Owner; } }
		internal AxisRangeData Data { get { return data; } }
		internal double SideMargins { get { return data.SideMargins; } }
		protected internal virtual bool Fixed { get { return false; } }
		protected internal virtual bool SupportAuto { get { return true; } }
		[
		Obsolete("This property is obsolete now. Use AxisBase.VisualRange.Auto instead."),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisRangeAuto"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisRange.Auto"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		NonTestableProperty
		]
		public virtual bool Auto {
			get {
				return Axis.VisualRangeData.CorrectionMode == RangeCorrectionMode.Auto;
			}
			set {
				Axis.VisualRangeData.Reset(true);
				if (Loading)
					Deserializer.CorrectionMode = value ? RangeCorrectionMode.Auto : Axis.VisualRangeData.CorrectionMode;
				if (!IsScrollingEnable())
					((WholeRangeData)Axis.WholeRangeData).CorrectionMode = value ? RangeCorrectionMode.Auto : RangeCorrectionMode.Values;
				((VisualRangeData)Axis.VisualRangeData).CorrectionMode = value ? RangeCorrectionMode.Auto : RangeCorrectionMode.Values;
			}
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.VisualRange.MinValue instead."),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisRangeMinValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisRange.MinValue"),
		TypeConverter(typeof(AxisRangeMinValueTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		NonTestableProperty
		]
		public virtual object MinValue {
			get { return data.MinValue; }
			set {
				if (RangeHelper.RangeCalculationSwitch) {
					if (IsScrollingEnable()) {
						((VisualRangeData)Axis.VisualRangeData).MinValue = value;
					}
					else {
						((VisualRangeData)Axis.VisualRangeData).AutoMin = false;
						((VisualRangeData)Axis.VisualRangeData).CorrectionMode = RangeCorrectionMode.Values;
						((WholeRangeData)Axis.WholeRangeData).MinValue = value;
						((VisualRangeData)Axis.VisualRangeData).MinValue = value;
					}
				}
			}
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.VisualRange.MaxValue instead."),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisRangeMaxValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisRange.MaxValue"),
		TypeConverter(typeof(AxisRangeMaxValueTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		NonTestableProperty
		]
		public virtual object MaxValue {
			get { return data.MaxValue; }
			set {
				if (RangeHelper.RangeCalculationSwitch) {
					if (IsScrollingEnable()) {
						((VisualRangeData)Axis.VisualRangeData).MaxValue = value;
					}
					else {
						((VisualRangeData)Axis.VisualRangeData).AutoMax = false;
						((VisualRangeData)Axis.VisualRangeData).CorrectionMode = RangeCorrectionMode.Values;
						((WholeRangeData)Axis.WholeRangeData).MaxValue = value;
						((VisualRangeData)Axis.VisualRangeData).MaxValue = value;
					}
				}
			}
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.VisualRange.MinValueInternal instead."),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisRangeMinValueInternal"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisRange.MinValueInternal"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public virtual double MinValueInternal {
			get { return data.MinValueInternal; }
			set {
				double minValue = value;
				if (Axis.ScaleTypeMap.ScaleType == ActualScaleType.DateTime)
					minValue += ((AxisDateTimeMap)Axis.ScaleTypeMap).Min;
				if (IsScrollingEnable()) {
					((VisualRangeData)Axis.VisualRangeData).MinValueInternal = minValue;
				}
				else {
					((WholeRangeData)Axis.WholeRangeData).MinValueInternal = minValue;
					if (((VisualRangeData)Axis.VisualRangeData).CorrectionMode == RangeCorrectionMode.Auto)
						((VisualRangeData)Axis.VisualRangeData).CorrectionMode = RangeCorrectionMode.InternalValues;
					else
						((VisualRangeData)Axis.VisualRangeData).MinValueInternal = minValue;
				}
				if (Loading)
					Deserializer.MinInternal = value;
			}
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.VisualRange.MaxValueInternal instead."),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisRangeMaxValueInternal"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisRange.MaxValueInternal"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public virtual double MaxValueInternal {
			get { return data.MaxValueInternal; }
			set {
				double maxValue = value;
				if (Axis.ScaleTypeMap.ScaleType == ActualScaleType.DateTime)
					maxValue += ((AxisDateTimeMap)Axis.ScaleTypeMap).Min;
				if (IsScrollingEnable()) {
					((VisualRangeData)Axis.VisualRangeData).MaxValueInternal = maxValue;
				}
				else {
					((WholeRangeData)Axis.WholeRangeData).MaxValueInternal = maxValue;
					if (((VisualRangeData)Axis.VisualRangeData).CorrectionMode == RangeCorrectionMode.Auto)
						((VisualRangeData)Axis.VisualRangeData).CorrectionMode = RangeCorrectionMode.InternalValues;
					else
						((VisualRangeData)Axis.VisualRangeData).MaxValueInternal = maxValue;
				}
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
		public virtual string MinValueSerializable {
			get { return null; }
			set {
				Deserializer.MinValueSerializable = value;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty(),
		XtraSerializableProperty
		]
		public virtual string MaxValueSerializable {
			get { return null; }
			set {
				Deserializer.MaxValueSerializable = value;
			}
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.WholeRange.AlwaysShowZeroLevel instead."),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisRangeAlwaysShowZeroLevel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisRange.AlwaysShowZeroLevel"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public bool AlwaysShowZeroLevel {
			get { return Axis.VisualRangeData.AlwaysShowZeroLevel; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				Axis.VisualRangeData.AlwaysShowZeroLevel = value;
				Axis.WholeRangeData.AlwaysShowZeroLevel = value;
				RaiseControlChanged();
			}
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.VisualRange.AutoSideMargins instead. To calculate side margins automatically, enable the VisualRange.AutoSideMargins property. When the VisualRange.AutoSideMargins property is disabled it becomes possible to specify custom side margins values using the VisualRange.SideMarginsValue property."),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisRangeSideMarginsEnabled"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisRange.SideMarginsEnabled"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public bool SideMarginsEnabled {
			get { return Axis.VisualRangeData.AutoSideMargins == SideMarginMode.Enable || Axis.VisualRangeData.AutoSideMargins == SideMarginMode.UserEnable; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				if (!IsScrollingEnable()) {
					Axis.WholeRangeData.AutoSideMargins = value ? SideMarginMode.UserEnable : SideMarginMode.UserDisable;
					if (!value)
						Axis.WholeRangeData.SideMarginsValue = 0;
				}
				Axis.VisualRangeData.AutoSideMargins = value ? SideMarginMode.UserEnable : SideMarginMode.UserDisable;
				if (!value)
					Axis.VisualRangeData.SideMarginsValue = 0;
				if (Loading) {
					Deserializer.AutoSideMargins = value;
					if (!value)
						Deserializer.SideMarginsValue = 0;
				}
				RaiseControlChanged();
			}
		}
		[
		Obsolete("This property is obsolete now. To specify a custom range, use the AxisBase.VisualRange and AxisBase.WholeRange properties instead. For more information, see the corresponding topic in the documentation. "),
		Browsable(false),
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisRangeScrollingRange"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisRange.ScrollingRange"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		NonTestableProperty,
		]
		public ScrollingRange ScrollingRange {
			get { return scrollingRange; }
		}
		protected AxisRange(AxisBase axis)
			: base(axis) {
			data = new AxisRangeDataEx(this);
			scrollingRange = new ScrollingRange(this);
		}
		protected AxisRange(AxisBase axis, RangeDataBase wholeAxisRange, RangeDataBase visibleAxisRange)
			: base(axis) {
			data = new AxisRangeDataEx(this);
			scrollingRange = new ScrollingRange(this, wholeAxisRange);
		}
		protected AxisRange(AxisBase axis, AxisRangeData data) {
			this.data = data;
			scrollingRange = new ScrollingRange(this);
		}
		#region ShouldSerialize & Reset
		internal bool ShouldSerializeMinValueInternal() {
			return false;
		}
		internal bool ShouldSerializeMaxValueInternal() {
			return false;
		}
		bool ShouldSerializeAuto() {
			return false;
		}
		bool ShouldSerializeMinValueSerializable() {
			return false;
		}
		bool ShouldSerializeMaxValueSerializable() {
			return false;
		}
		bool ShouldSerializeAlwaysShowZeroLevel() {
			return false;
		}
		bool ShouldSerializeSideMarginsEnabled() {
			return false;
		}
		protected internal override bool ShouldSerialize() {
			return false;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return false;
		}
		#endregion
		bool NeedStretchWholeRange(object minValue, object maxValue) {
			double min = Axis.ScaleTypeMap.NativeToInternal(minValue);
			double max = Axis.ScaleTypeMap.NativeToInternal(maxValue);
			return !(Contains(Axis.WholeRangeData, min) && Contains(Axis.WholeRangeData, max));
		}
		bool Contains(IAxisRangeData rande, double value) {
			return rande.Min + rande.SideMarginsMin <= value && value <= rande.Max - rande.SideMarginsMax;
		}
		internal RangeDataBase.Deserializer GetDeserializer() { return visualRangeDeserializer; }
		internal void OnScaleTypeUpdated() {
			data.OnScaleTypeUpdated();
			scrollingRange.Data.OnScaleTypeUpdated();
		}
		bool IsScrollingEnable() {
			XYDiagram diagram = Axis.Diagram as XYDiagram;
			if (diagram != null) {
				if (Axis.IsValuesAxis)
					return diagram.EnableAxisYScrolling;
				else
					return diagram.EnableAxisXScrolling;
			}
			else {
				SwiftPlotDiagram swiftPlotDiagram = Axis.Diagram as SwiftPlotDiagram;
				if (swiftPlotDiagram != null) {
					if (Axis.IsValuesAxis)
						return swiftPlotDiagram.EnableAxisYScrolling;
					else
						return swiftPlotDiagram.EnableAxisXScrolling;
				}
			}
			return false;
		}
		public virtual void SetMinMaxValues(object minValue, object maxValue) {
			if (NeedStretchWholeRange(minValue, maxValue)) {
				((WholeRangeData)Axis.WholeRangeData).SetRange(minValue, maxValue, double.NaN, double.NaN, true);
			}
			((VisualRangeData)Axis.VisualRangeData).SetRange(minValue, maxValue, double.NaN, double.NaN, true);
			data.SetMinMaxValues(minValue, maxValue);
		}
		public virtual void SetInternalMinMaxValues(double min, double max) {
			if (RangeHelper.RangeCalculationSwitch) {
				IAxisData axis = Axis as IAxisData;
				if (axis != null) {
					double minValue = min;
					double maxValue = max;
					if (axis.AxisScaleTypeMap.ScaleType == ActualScaleType.DateTime) {
						minValue += ((AxisDateTimeMap)Axis.ScaleTypeMap).Min;
						maxValue += ((AxisDateTimeMap)Axis.ScaleTypeMap).Min;
					}
					if (IsScrollingEnable()) {
						((VisualRangeData)Axis.VisualRangeData).SetRange(null, null, minValue, maxValue, true);
					}
					else {
						((WholeRangeData)Axis.WholeRangeData).SetRange(null, null, minValue, maxValue, true);
						if (((VisualRangeData)Axis.VisualRangeData).CorrectionMode == RangeCorrectionMode.Auto) {
							((VisualRangeData)Axis.VisualRangeData).CorrectionMode = RangeCorrectionMode.InternalValues;
							((VisualRangeData)Axis.VisualRangeData).SetRange(null, null, minValue, maxValue, true);
						}
						else
							((VisualRangeData)Axis.VisualRangeData).SetRange(null, null, minValue, maxValue, true);
					}
				}
			}
			Axis.VisualRangeData.SideMarginsValue = 0;
			data.SetInternalMinMaxValues(min, max);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			AxisRange range = obj as AxisRange;
			if (range != null) {
				data.Assign(range.data);
				scrollingRange.Assign(range.scrollingRange);
				if (Axis != null) {
					((VisualRangeData)Axis.VisualRangeData).Assign(range.data);
					((WholeRangeData)Axis.WholeRangeData).Assign(range.scrollingRange.Data);
				}
			}
		}
	}
	public class AxisXRange : AxisRange {
		internal AxisXRange()
			: base(null) {
		}
		internal AxisXRange(AxisXBase axis)
			: base(axis) {
		}
		internal AxisXRange(AxisBase axis, RangeDataBase wholeAxisRange, RangeDataBase visibleAxisRange)
			: base(axis, wholeAxisRange, visibleAxisRange) {
		}
		internal AxisXRange(SwiftPlotDiagramAxisXBase axis)
			: base(axis) {
		}
		internal AxisXRange(AxisX3D axis)
			: base(axis) {
		}
		internal AxisXRange(RadarAxisX axis)
			: base(axis) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new AxisXRange();
		}
	}
	public class AxisYRange : AxisRange {
		internal AxisYRange()
			: base(null) {
		}
		internal AxisYRange(AxisYBase axis)
			: base(axis) {
		}
		internal AxisYRange(AxisBase axis, RangeDataBase wholeAxisRange, RangeDataBase visibleAxisRange)
			: base(axis, wholeAxisRange, visibleAxisRange) {
		}
		internal AxisYRange(SwiftPlotDiagramAxisYBase axis)
			: base(axis) {
		}
		internal AxisYRange(AxisY3D axis)
			: base(axis) {
		}
		internal AxisYRange(RadarAxisY axis)
			: base(axis) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new AxisYRange();
		}
	}
}
