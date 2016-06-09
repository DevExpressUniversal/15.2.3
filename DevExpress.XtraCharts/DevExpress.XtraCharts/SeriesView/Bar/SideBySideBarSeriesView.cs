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
	TypeConverter(typeof(SideBySideBarSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SideBySideBarSeriesView : BarSeriesView, ISideBySideBarSeriesView {
		protected override Type PointInterfaceType { get { return typeof(IValuePoint); } }
		protected override bool NeedSeriesGroupsInteraction { get { return true; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnSideBySideBar); } }
		protected internal new SideBySideBarSeriesLabel Label { get { return (SideBySideBarSeriesLabel)base.Label; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SideBySideBarSeriesViewBarDistance"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideBarSeriesView.BarDistance"),
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
	DevExpressXtraChartsLocalizedDescription("SideBySideBarSeriesViewBarDistanceFixed"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideBarSeriesView.BarDistanceFixed"),
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
	DevExpressXtraChartsLocalizedDescription("SideBySideBarSeriesViewEqualBarWidth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SideBySideBarSeriesView.EqualBarWidth"),
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
		public SideBySideBarSeriesView() : base() {
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
		protected override void CalculateAnnotationAnchorPointLayout(Annotation annotation, XYDiagramAnchorPointLayoutList anchorPointLayoutList, RefinedPointData pointData) {
			BarData barData = pointData.GetBarData(this);
			XYDiagramMappingBase mapping = (IsScrollingEnabled && annotation.ScrollingSupported) ? anchorPointLayoutList.MappingContainer.MappingForScrolling
				: barData.GetMappingForTopLabelPosition(anchorPointLayoutList.MappingContainer);
			if (mapping != null)
				anchorPointLayoutList.Add(new AnnotationLayout(annotation, barData.GetScreenPoint(barData.Argument, barData.ActualValue, mapping), pointData.RefinedPoint));
		}
		protected override ChartElement CreateObjectForClone() {
			return new SideBySideBarSeriesView();
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
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value;
		}
		protected internal override DiagramPoint? CalculateRelativeToolTipPosition(XYDiagramMappingBase mapping, RefinedPointData pointData) {
			BarData barData = pointData.GetBarData(this);
			return barData.CalculateAnchorPointForToolTipTopPosition(mapping.Container);
		}
		protected internal override BarData CreateBarData(RefinedPoint pointInfo) {
			ISideBySidePoint sideBySidePoint = (ISideBySidePoint)pointInfo;
			return new BarData(sideBySidePoint.Argument, 0.0, sideBySidePoint.Value, sideBySidePoint.BarWidth, sideBySidePoint.Offset, sideBySidePoint.FixedOffset);
		}		
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new SideBySideBarSeriesLabel();
		}
		public override string GetValueCaption(int index) {
			if (index > 0)
				throw new IndexOutOfRangeException();
			return ChartLocalizer.GetString(ChartStringId.ValueMember);
		}
	}
}
