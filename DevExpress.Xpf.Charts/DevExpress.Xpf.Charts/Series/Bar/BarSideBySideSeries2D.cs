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
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum Bar2DLabelPosition {
		Outside,
		Center,
		Auto
	}
	public class BarSideBySideSeries2D : BarSeries2D, ISideBySideBarSeriesView {
		public static readonly DependencyProperty LabelPositionProperty;
		[
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public static Bar2DLabelPosition GetLabelPosition(SeriesLabel label) {
			return (Bar2DLabelPosition)label.GetValue(LabelPositionProperty);
		}
		public static void SetLabelPosition(SeriesLabel label, Bar2DLabelPosition value) {
			label.SetValue(LabelPositionProperty, value);
		}
		protected override Type PointInterfaceType { get { return typeof(IValuePoint); } }
		static BarSideBySideSeries2D() {
			LabelPositionProperty = DependencyPropertyManager.RegisterAttached("LabelPosition", typeof(Bar2DLabelPosition), 
				typeof(BarSideBySideSeries2D), new PropertyMetadata(Bar2DLabelPosition.Auto, ChartElementHelper.UpdateWithClearDiagramCache));
		}
		protected override bool NeedSeriesGroupsInteraction {
			get {
				return true;
			}
		}
		public BarSideBySideSeries2D() : base() {
			DefaultStyleKey = typeof(BarSideBySideSeries2D);
		}
		#region ISideBySideBarSeriesView
		double ISideBySideBarSeriesView.BarDistance {
			get { return Diagram is XYDiagram2D ? ((XYDiagram2D)Diagram).BarDistance : 0; }
			set {
				if (Diagram is XYDiagram2D)
					((XYDiagram2D)Diagram).BarDistance = value;
			}
		}
		int ISideBySideBarSeriesView.BarDistanceFixed {
			get { return Diagram is XYDiagram2D ? ((XYDiagram2D)Diagram).BarDistanceFixed : 0; }
			set {
				if (Diagram is XYDiagram2D)
					((XYDiagram2D)Diagram).BarDistanceFixed = value;
			}
		}
		bool ISideBySideBarSeriesView.EqualBarWidth {
			get { return Diagram is XYDiagram2D ? ((XYDiagram2D)Diagram).EqualBarWidth : true; }
			set {
				if (Diagram is XYDiagram2D)
					((XYDiagram2D)Diagram).EqualBarWidth = value;
			}
		}
		#endregion
		protected override double GetBarWidth(RefinedPoint refinedPoint) {
			return ((ISideBySidePoint)refinedPoint).BarWidth;
		}
		protected override int GetFixedOffset(RefinedPoint refinedPoint) {
			return ((ISideBySidePoint)refinedPoint).FixedOffset;
		}
		protected override double GetDisplayOffset(RefinedPoint refinedPoint) {
			return ((ISideBySidePoint)refinedPoint).Offset;
		}
		protected override Series CreateObjectForClone() {
			return new BarSideBySideSeries2D();
		}
		protected override Bar2DLabelPosition GetSeriesLabelPosition() {
			return BarSideBySideSeries2D.GetLabelPosition(ActualLabel);
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			BarSideBySideSeries2D barSideBySideSeries2D = series as BarSideBySideSeries2D;
			if (barSideBySideSeries2D != null)
				if (Label != null && barSideBySideSeries2D.Label != null) 
					BarSideBySideSeries2D.SetLabelPosition(Label, BarSideBySideSeries2D.GetLabelPosition(barSideBySideSeries2D.Label));
		}
		protected internal override Point CalculateToolTipPoint(SeriesPointItem pointItem, PaneMapping mapping, Transform transform, bool inLabel) {
			Bar2D? bar = CalculateBar(mapping, pointItem.RefinedPoint, false);
			return bar.HasValue ? GetLabelAnchorPoint(pointItem, mapping, transform, false, bar.Value) : new Point(0.0, 0.0);
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
		protected override SeriesPointLayout CreatePointItemLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			if (pointItem == null || pointItem.RefinedPoint == null)
				return null;
			Rect barBounds = CalculateSeriesPointBounds(mapping, pointItem.RefinedPoint);
			if (barBounds.Width > 0 && (barBounds.Height < 1.0))
				barBounds.Height = 1.0;
			Rect viewport = new Rect(0, 0, mapping.Viewport.Width, mapping.Viewport.Height);
			Rect? clipBounds = CalculateSeriesPointClipBounds(mapping, pointItem, viewport, barBounds);
			return new BarSeries2DPointLayout(viewport, barBounds, clipBounds);
		}
	}	
}
