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
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.ModelSupport;
using System.Collections.Generic;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(StackedBarSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class StackedBarSeriesView : BarSeriesView, IStackedView {
		protected override int PixelsPerArgument { get { return 40; } }
		protected override Type PointInterfaceType { get { return typeof(IStackedPoint); } }
		protected override bool NeedSeriesInteraction { get { return true; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnStackedBar); } }
		protected internal override bool DateTimeValuesSupported { get { return false; } }
		public StackedBarSeriesView() : base() {
		}
		protected override IEnumerable<double> GetCrosshairValues(RefinedPoint refinedPoint) {
			yield return ((IStackedPoint)refinedPoint).MaxValue;
		}
		protected override void CalculateAnnotationAnchorPointLayout(Annotation annotation, XYDiagramAnchorPointLayoutList anchorPointLayoutList, RefinedPointData pointData) {
			AnnotationHelper.CalculateAchorPointLayoutForCenterBarPoint(annotation, this, anchorPointLayoutList, pointData);
		}
		protected override ChartElement CreateObjectForClone() {
			return new StackedBarSeriesView();
		}
		protected override SeriesPointLayout CalculateSeriesPointLayoutInternal(XYDiagramMappingBase diagramMapping, RefinedPointData pointData, BarData barData) {
			return barData.ZeroValue == barData.ActualValue ? null : base.CalculateSeriesPointLayoutInternal(diagramMapping, pointData, barData);
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			return ((IStackedPoint)point).Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			return ((IStackedPoint)point).Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IStackedPoint)point).Value);
		}
		protected override SeriesContainer CreateContainer() {
			return new StackedInteractionContainer(this);
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value;
		}
		protected internal override DiagramPoint? CalculateRelativeToolTipPosition(XYDiagramMappingBase mapping, RefinedPointData pointData) {
			BarData barData = pointData.GetBarData(this);
			return barData.CalculateAnchorPointForCenterLabelPosition(mapping.Container);
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new StackedBarSeriesLabel();
		}
		protected internal override BarData CreateBarData(RefinedPoint refinedPoint) {
			return new BarData(refinedPoint.Argument, ((IStackedPoint)refinedPoint).MinValue, ((IStackedPoint)refinedPoint).MaxValue, BarWidth, 0.0, 0);
		}
		public override string GetValueCaption(int index) {
			return ChartLocalizer.GetString(ChartStringId.ValueMember);
		}
	}
}
