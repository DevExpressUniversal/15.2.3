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
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SideBySideBar3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SideBySideBar3DSeriesView : Bar3DSeriesView, ISideBySideBarSeriesView {
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnSideBySideBar3D); } }
		protected override Type PointInterfaceType { get { return typeof(IXYPoint); } }
		protected override bool NeedSeriesGroupsInteraction { get { return true; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SideBySideBar3DSeriesViewBarDistance"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideBar3DSeriesView.BarDistance"),
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
	DevExpressXtraChartsLocalizedDescription("SideBySideBar3DSeriesViewBarDistanceFixed"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideBar3DSeriesView.BarDistanceFixed"),
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
	DevExpressXtraChartsLocalizedDescription("SideBySideBar3DSeriesViewEqualBarWidth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideBar3DSeriesView.EqualBarWidth"),
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
		protected override ChartElement CreateObjectForClone() {
			return new SideBySideBar3DSeriesView();
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			return ((IValuePoint)point).Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			return ((IValuePoint)point).Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IValuePoint)point).Value);
		}
		protected override double GetBarWidth(IXYPoint refinedPoint) {
			return ((ISideBySidePoint)refinedPoint).BarWidth;
		}
		protected override double GetDisplayOffset(IXYPoint refinedPoint) {
			return ((ISideBySidePoint)refinedPoint).Offset;
		}
		protected override int GetFixedOffset(IXYPoint refinedPoint) {
			return ((ISideBySidePoint)refinedPoint).FixedOffset;
		}
	}
}
