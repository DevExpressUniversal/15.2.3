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

using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class RangeBarSideBySideSeries2D : RangeBarSeries2D, ISideBySideBarSeriesView {
		protected override bool NeedSeriesGroupsInteraction {
			get {
				return true;
			}
		}
		public RangeBarSideBySideSeries2D() : base() {
			DefaultStyleKey = typeof(RangeBarSideBySideSeries2D);
		}
		#region ISideBySideBarSeriesView
		double ISideBySideBarSeriesView.BarDistance {
			get { return Diagram is XYDiagram2D ? ((XYDiagram2D)Diagram).BarDistance : 0; }
			set {
				if (Diagram is XYDiagram2D) {
					((XYDiagram2D)Diagram).BarDistance = value;
				}
			}
		}
		int ISideBySideBarSeriesView.BarDistanceFixed {
			get { return Diagram is XYDiagram2D ? ((XYDiagram2D)Diagram).BarDistanceFixed : 0; }
			set {
				if (Diagram is XYDiagram2D) {
					((XYDiagram2D)Diagram).BarDistanceFixed = value;
				}
			}
		}
		bool ISideBySideBarSeriesView.EqualBarWidth {
			get { return Diagram is XYDiagram2D ? ((XYDiagram2D)Diagram).EqualBarWidth : true; }
			set {
				if (Diagram is XYDiagram2D) {
					((XYDiagram2D)Diagram).EqualBarWidth = value;
				}
			}
		}
		#endregion
		SeriesPointLayout CreateMarkerLayout(PaneMapping mapping, SeriesPointItem pointItem, int markerSize, double pointValue) {
			RefinedPoint pointInfo = pointItem.RefinedPoint;
			double argument = pointInfo.Argument + GetDisplayOffset(pointInfo);
			int halfMarkerSize = markerSize / 2;
			double fixedOffset = GetFixedOffset(pointInfo);
			if (IsAxisXReversed)
				fixedOffset = -fixedOffset;
			Point center = mapping.GetRoundedDiagramPoint(argument, pointValue);
			Rect viewport = new Rect(0, 0, mapping.Viewport.Width, mapping.Viewport.Height);
			return new MarkerSeries2DPointLayout(viewport, new Rect(center.X - halfMarkerSize + fixedOffset, center.Y - halfMarkerSize, markerSize, markerSize));
		}
		protected override double GetBarWidth(RefinedPoint pointInfo) {
			return ((ISideBySidePoint)pointInfo).BarWidth;
		}
		protected override int GetFixedOffset(RefinedPoint pointInfo) {
			return ((ISideBySidePoint)pointInfo).FixedOffset;
		}
		protected override double GetDisplayOffset(RefinedPoint pointInfo) {
			return ((ISideBySidePoint)pointInfo).Offset;
		}
		protected override SeriesPointLayout CreatePointItemLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			if (pointItem.ValueLevel == RangeValueLevel.Value1) {
				return CreateMarkerLayout(mapping, pointItem, MinMarkerSize, ((IRangePoint)pointItem.RefinedPoint).Min);
			}
			else if (pointItem.ValueLevel == RangeValueLevel.Value2) {
				return CreateMarkerLayout(mapping, pointItem, MaxMarkerSize, ((IRangePoint)pointItem.RefinedPoint).Max);
			}
			else
				return base.CreatePointItemLayout(mapping, pointItem);
		}
		protected override Series CreateObjectForClone() {
			return new RangeBarSideBySideSeries2D();
		}
	}
}
