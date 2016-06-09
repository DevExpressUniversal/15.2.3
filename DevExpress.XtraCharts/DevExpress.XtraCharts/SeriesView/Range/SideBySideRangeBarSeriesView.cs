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
	TypeConverter(typeof(SideBySideRangeBarSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SideBySideRangeBarSeriesView : RangeBarSeriesView, ISideBySideBarSeriesView {
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnSideBySideRangeBar); } }
		protected override bool NeedSeriesGroupsInteraction { get { return true; } }
		protected override Type PointInterfaceType {
			get {
				return typeof(IRangePoint);
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SideBySideRangeBarSeriesViewBarDistance"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideRangeBarSeriesView.BarDistance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public double BarDistance {
			get { return Chart != null ? Chart.SideBySideBarDistance : SideBySideBarDefaults.DefaultBarDistance; }
			set {
				if (Chart != null)
					Chart.SideBySideBarDistance = value;
				else
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectBarDistancePropertyUsing));
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SideBySideRangeBarSeriesViewBarDistanceFixed"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideRangeBarSeriesView.BarDistanceFixed"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public int BarDistanceFixed {
			get { return Chart != null ? Chart.SideBySideBarDistanceFixed : SideBySideBarDefaults.DefaultBarDistanceFixed; }
			set {
				if (Chart != null)
					Chart.SideBySideBarDistanceFixed = value;
				else
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectBarDistanceFixedPropertyUsing));
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SideBySideRangeBarSeriesViewEqualBarWidth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideRangeBarSeriesView.EqualBarWidth"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool EqualBarWidth {
			get { return Chart != null ? Chart.SideBySideEqualBarWidth : SideBySideBarDefaults.DefaultEqualBarWidth; }
			set {
				if (Chart != null)
					Chart.SideBySideEqualBarWidth = value;
				else
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectEqualBarWidthPropertyUsing));
			}
		}
		public SideBySideRangeBarSeriesView() : base() {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeBarDistance() {
			return BarDistance != SideBySideBarDefaults.DefaultBarDistance;
		}
		void ResetBarDistance() {
			BarDistance = SideBySideBarDefaults.DefaultBarDistance;
		}
		bool ShouldSerializeBarDistanceFixed() {
			return BarDistanceFixed != SideBySideBarDefaults.DefaultBarDistanceFixed;
		}
		void ResetBarDistanceFixed() {
			BarDistanceFixed = SideBySideBarDefaults.DefaultBarDistanceFixed;
		}
		bool ShouldSerializeEqualBarWidth() {
			return EqualBarWidth != SideBySideBarDefaults.DefaultEqualBarWidth;
		}
		void ResetEqualBarWidth() {
			EqualBarWidth = SideBySideBarDefaults.DefaultEqualBarWidth;
		}
		#endregion
		protected override double GetRefinedPointMax(RefinedPoint point) {
			IRangePoint rangePoint = (IRangePoint)point;
			return Math.Max(rangePoint.Value1, rangePoint.Value2);
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			IRangePoint rangePoint = (IRangePoint)point;
			return Math.Min(rangePoint.Value1, rangePoint.Value2);
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			IRangePoint rangePoint = (IRangePoint)point;
			return Math.Min(Math.Abs(rangePoint.Value1), Math.Abs(rangePoint.Value2));
		}
		protected override ChartElement CreateObjectForClone() {
			return new SideBySideRangeBarSeriesView();
		}
		protected internal override BarData CreateBarData(RefinedPoint pointInfo) {
			IRangePoint rangePoint = pointInfo;
			ISideBySidePoint rangeBarPoint = pointInfo;
			return new BarData(pointInfo.Argument, rangePoint.Min, rangePoint.Max, rangeBarPoint.BarWidth, rangeBarPoint.Offset, rangeBarPoint.FixedOffset);
		}
	}
}
