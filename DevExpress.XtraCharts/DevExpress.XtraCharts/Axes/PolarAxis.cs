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
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PolarAxisXRange : RadarAxisXRange {
		const double minRange = 0.0;
		const double maxRange = 360.0;
		internal static double CalculateRandomArgument(int pointIndex, int pointCount) {
			if (pointCount == 0)
				throw new InternalException("Invalid count of random points.");
			if (pointIndex < 0 || pointIndex > pointCount - 1)
				throw new InternalException("Invalid index of random point.");
			return pointIndex * maxRange / pointCount;
		}
		protected internal override bool Fixed { get { return true; } }
		protected internal override bool SupportAuto { get { return false; } }
		[
		Obsolete("This property is obsolete now. Use AxisBase.VisualRange.MinValue instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override object MinValue {
			get { return minRange; }
			set { throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXRangeChanged)); }
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.VisualRange.MaxValue instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override object MaxValue {
			get { return maxRange; }
			set { throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXRangeChanged)); }
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.VisualRange.MinValueInternal instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override double MinValueInternal {
			get { return base.MinValueInternal; }
			set { throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXRangeChanged)); }
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.VisualRange.MaxValueInternal instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override double MaxValueInternal {
			get { return base.MaxValueInternal; }
			set { throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXRangeChanged)); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override string MinValueSerializable {
			get { return base.MinValueSerializable; }
			set { throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXRangeChanged)); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override string MaxValueSerializable {
			get { return base.MaxValueSerializable; }
			set { throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXRangeChanged)); }
		}
		[
		Obsolete("This property is obsolete now. Use AxisBase.VisualRange.Auto instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override bool Auto {
			get { return base.Auto; }
			set { throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXRangeChanged)); }
		}
		internal PolarAxisXRange(PolarAxisX axis)
			: base(axis) {
			((IAxisData)axis).WholeRange.UpdateRange(minRange, maxRange, minRange, maxRange);
			((IAxisData)axis).VisualRange.UpdateRange(minRange, maxRange, minRange, maxRange);
		}
		internal PolarAxisXRange(PolarAxisX axis, RangeDataBase wholeAxisRange, RangeDataBase visibleAxisRange)
			: base(axis, wholeAxisRange, visibleAxisRange) {
				((IAxisData)axis).WholeRange.UpdateRange(minRange, maxRange, minRange, maxRange);
				((IAxisData)axis).VisualRange.UpdateRange(minRange, maxRange, minRange, maxRange);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void SetInternalMinMaxValues(double min, double max) {
			throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXRangeChanged));
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void SetMinMaxValues(object minValue, object maxValue) {
			throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXRangeChanged));
		}
		public override void Assign(ChartElement obj) {
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PolarAxisX : RadarAxisX {
		protected override bool IsRadarAxis { get { return false; } }
		[
		Browsable(false),
		Obsolete("This property is obsolete now. Use the NumericScaleOptions.AutoGrid and DateTimeScaleOptions.AutoGrid properties instead for the  numeric and date-time scales."),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override bool GridSpacingAuto {
			get { return base.GridSpacingAuto; }
			set { base.GridSpacingAuto = value; }
		}
		[
		Browsable(false),
		Obsolete("This property is obsolete now. Use the NumericScaleOptions.GridSpacing and DateTimeScaleOptions.GridSpacing properties instead for the  numeric and date-time scales."),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override double GridSpacing {
			get { return base.GridSpacing; }
			set { base.GridSpacing = value; }
		}
		[
		Obsolete("This property is obsolete now. To specify a custom range, use the AxisBase.VisualRange and AxisBase.WholeRange properties instead. For more information, see the corresponding topic in the documentation."),
		Browsable(false),		
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new AxisRange Range { get { return base.Range; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override bool Logarithmic {
			get { return false; }
			set { throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXLogarithmic)); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override double LogarithmicBase {
			get { return 10.0; }
			set { throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgPolarAxisXLogarithmic)); }
		}
		internal PolarAxisX(PolarDiagram diagram)
			: base(diagram) {
		}
		protected override AxisRange CreateAxisRange(RangeDataBase wholeAxisRange, RangeDataBase visibleAxisRange) {
			return new PolarAxisXRange(this);
		}
		protected internal override bool IsGridSpacingSupported {
			get {
				return false;
			}
		}
		protected internal override double GetGridSpacingByUserValue(double userValue) {
			return (WholeRange.MaxValueInternal - WholeRange.MinValueInternal) / 12.0;
		}
		protected internal override bool GetGridSpacingAutoByUserValue(bool userValue) {
			return false;
		}
		public override string ToString() {
			return "(PolarAxisX)";
		}
	}
}
