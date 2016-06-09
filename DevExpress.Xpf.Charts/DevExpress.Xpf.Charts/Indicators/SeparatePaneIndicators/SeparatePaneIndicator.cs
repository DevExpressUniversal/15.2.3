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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public abstract class SeparatePaneIndicator : Indicator, ISeparatePaneIndicator {
		int paneIndex = -1;
		int axisYIndex = -1;
		XYSeries2D Series {
			get { return ((IOwnedElement)this).Owner as XYSeries2D; }
		}
		XYDiagram2D XyDiagram2D {
			get {
				if (Series.Diagram == null)
					return null;
				return (XYDiagram2D)Series.Diagram;
			}
		}
		protected internal AxisBase ActualAxisY {
			get {
				XYSeries2D series = Series as XYSeries2D;
				if (series != null) {
					XYDiagram2D diagram = series.Diagram as XYDiagram2D;
					if (diagram != null)
						return diagram.SecondaryAxesYInternal.Contains(AxisYInternal) ? AxisYInternal : series.ActualAxisY;
				}
				return null;
			}
		}
		protected internal Pane ActualPane {
			get {
				XYSeries2D series = Series as XYSeries2D;
				if (series != null) {
					XYDiagram2D diagram = series.Diagram as XYDiagram2D;
					if (diagram != null)
						return diagram.Panes.Contains(PaneInternal) ? PaneInternal : series.ActualPane;
				}
				return null;
			}
		}
		protected internal SecondaryAxisY2D AxisYInternal {
			get; set;
		}
		protected internal Pane PaneInternal {
			get; set;
		}
		[Browsable(false),
		 EditorBrowsable(EditorBrowsableState.Never),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		 NonTestableProperty,
		 XtraSerializableProperty]
		public int PaneIndex {
			get {
				Pane pane = XYDiagram2D.GetIndicatorPane(this);
				if (pane != null && XyDiagram2D != null)
					return XyDiagram2D.Panes.IndexOf(pane);
				return -1;
			}
			set { paneIndex = value; }
		}
		[Browsable(false),
		 EditorBrowsable(EditorBrowsableState.Never),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		 NonTestableProperty,
		 XtraSerializableProperty]
		public int AxisYIndex {
			get {
				SecondaryAxisY2D axis = AxisYInternal;
				if (axis != null && XyDiagram2D != null)
					return XyDiagram2D.SecondaryAxesYInternal.IndexOf(axis);
				return -1;
			}
			set { axisYIndex = value; }
		}
		#region ISeparatePaneIndicator
		IAxisData IAffectsAxisRange.AxisYData { get { return ActualAxisY; } }
		IPane ISeparatePaneIndicator.Pane { get { return ActualPane; } }
		MinMaxValues IAffectsAxisRange.GetMinMaxValues(IMinMaxValues visualRangeOfOtherAxisForFiltering) { return GetMinMaxValues(visualRangeOfOtherAxisForFiltering); }
		#endregion
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			SeparatePaneIndicator separatePaneIndicator = indicator as SeparatePaneIndicator;
			if (separatePaneIndicator != null) {
				CopyPropertyValueHelper.CopyPropertyValueByRef(this, separatePaneIndicator, XYDiagram2D.IndicatorPaneProperty);
				CopyPropertyValueHelper.CopyPropertyValueByRef(this, separatePaneIndicator, XYDiagram2D.IndicatorAxisYProperty);
			}
		}
		protected abstract MinMaxValues GetMinMaxValues(IMinMaxValues visualRangeOfOtherAxisForFiltering);
		protected MinMaxValues GetFilteredMinMaxY(List<GRealPoint2D> indicatorPoints, IMinMaxValues visualRangeOfOtherAxisForFiltering, MinMaxValues minMaxYByWholeXRange) {
			bool isRangeEmpty = double.IsNaN(visualRangeOfOtherAxisForFiltering.Delta);
			if (isRangeEmpty)
				return minMaxYByWholeXRange;
			if (indicatorPoints == null || indicatorPoints.Count == 0)
				return MinMaxValues.Empty;
			int minIndex = -1;
			for (int i = 0; i < indicatorPoints.Count; i++) {
				if (indicatorPoints[i].X > visualRangeOfOtherAxisForFiltering.Min) {
					minIndex = i;
					break;
				}
			}
			int maxIndex = -1;
			for (int i = indicatorPoints.Count - 1; i > -1; i--) {
				if (indicatorPoints[i].X < visualRangeOfOtherAxisForFiltering.Max) {
					maxIndex = i;
					break;
				}
			}
			if (minIndex == -1 || maxIndex == -1)
				return MinMaxValues.Empty;
			double minValue = indicatorPoints[minIndex].Y;
			double maxValue = indicatorPoints[minIndex].Y;
			for (int i = minIndex + 1; i < maxIndex; i++) {
				if (indicatorPoints[i].Y < minValue)
					minValue = indicatorPoints[i].Y;
				if (indicatorPoints[i].Y > maxValue)
					maxValue = indicatorPoints[i].Y;
			}
			return new MinMaxValues(minValue, maxValue);
		}
		protected internal override void CompleteDeserializing() {
			if (XyDiagram2D != null) {
				if (paneIndex >= 0 && paneIndex < XyDiagram2D.Panes.Count)
					XYDiagram2D.SetIndicatorPane(this, XyDiagram2D.Panes[paneIndex]);
				if (axisYIndex >= 0 && axisYIndex < XyDiagram2D.SecondaryAxesYInternal.Count)
					XYDiagram2D.SetIndicatorAxisY(this, XyDiagram2D.SecondaryAxesYInternal[axisYIndex]);
			}
		}
	}
}
