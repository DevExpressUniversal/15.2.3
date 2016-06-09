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
using DevExpress.Charts.Native;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Native {
	public enum AxisDirection {
		LeftToRight,
		RightToLeft,
		TopToBottom,
		BottomToTop
	}
	public class AxisViewData {
		const int axisTitleIndent = 3;
		readonly Axis2D axis;
		readonly List<AxisIntervalViewData> intervalsViewData = new List<AxisIntervalViewData>();
		readonly AxisTitleViewData titleViewData;		
		readonly int nextAxisOffset;
		readonly AxisDirection axisDirection;
		readonly Rectangle paneBounds;
		List<AxisLabelResolveOverlappingCache> axisLabelResolveOverlappingCache = null;
		public int NextAxisOffset { get { return nextAxisOffset; } }
		public Axis2D Axis { get { return axis; } }
		public List<AxisIntervalViewData> IntervalsViewData { get { return intervalsViewData; } }
		public AxisDirection AxisDirection { get { return axisDirection; } }
		public AxisViewData(Axis2D axis, TextMeasurer textMeasurer, AxisGridAndTextData gridAndTextData, AxisMapping axisMapping, List<AxisIntervalMapping> intervalMappings, bool visible, int axisOffset, int elementsOffset, bool resolveLabelsOverlapping, bool autoLayout) {
			this.axis = axis;
			this.paneBounds = gridAndTextData.MaxBounds;
			axisDirection = GetAxisDirection();
			int startElementsOffset = elementsOffset;
			Rectangle bounds = Rectangle.Empty;
			foreach (AxisIntervalMapping intervalMapping in intervalMappings) {
				AxisIntervalViewData intervalViewData = new AxisIntervalViewData(textMeasurer, axis, intervalMapping,
					gridAndTextData[intervalMapping.IntervalLayout.Interval], visible, axisOffset, startElementsOffset);
				intervalsViewData.Add(intervalViewData);
				int nextElementsOffset = intervalViewData.NextElementOffset;
				if(nextElementsOffset > elementsOffset)
					elementsOffset = nextElementsOffset;
			}
			if (resolveLabelsOverlapping)
				ProcessOverlappingAxisLabels();
			if (autoLayout) {
				foreach (AxisIntervalViewData intervalViewData in IntervalsViewData) {
					if (bounds.IsEmpty)
						bounds = intervalViewData.AxisIntervalBounds;
					else
						bounds = Rectangle.Union(bounds, intervalViewData.AxisIntervalBounds);
					if ((intervalViewData.LabelViewData != null) && (intervalViewData.LabelViewData.Items != null))
						foreach (AxisLabelItem item in intervalViewData.LabelViewData.Items)
							bounds = Rectangle.Union(bounds, item.RoundedBounds);
				}
				axis.Bounds = new GRealRect2D(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			}
			foreach (AxisIntervalViewData intervalViewData in intervalsViewData)
				if (intervalViewData.NextElementOffset > elementsOffset)
					elementsOffset = intervalViewData.NextElementOffset;
			if (visible && axis.Title.Visibility != DefaultBoolean.False) {
				if (axis.Title.ActualVisibility)
					elementsOffset += axisTitleIndent;
				titleViewData = new AxisTitleViewData(textMeasurer, axis, axisMapping, axisOffset, elementsOffset);
				axis.Title.Bounds = new GRealRect2D(titleViewData.Bounds.X, titleViewData.Bounds.Y, titleViewData.Bounds.Width, titleViewData.Bounds.Height);
				if (axis.Title.ActualVisibility)
					elementsOffset += titleViewData.Size;
			}
			CalculateStripsViewData();
			CalculateConstantLinesViewData(textMeasurer, true);
			CalculateConstantLinesViewData(textMeasurer, false);
			nextAxisOffset = axisOffset + elementsOffset;
		}
		AxisDirection GetAxisDirection() {
			if(axis.IsVertical)
				return axis.ActualReverse ? AxisDirection.TopToBottom : AxisDirection.BottomToTop;
			else
				return axis.ActualReverse ? AxisDirection.RightToLeft : AxisDirection.LeftToRight;
		}
		void CalculateStripsViewData() {
			foreach (Strip strip in axis.Strips)
				if (strip.ShouldBeDrawnOnDiagram)
					CalculateStripViewData(strip);
		}
		void CalculateStripViewData(Strip strip) {
			double minValue = strip.MinLimit.ActualValue;
			double maxValue = strip.MaxLimit.ActualValue;
			foreach(AxisIntervalViewData intervalViewData in intervalsViewData) {
				AxisIntervalMapping intervalMapping = intervalViewData.IntervalMapping;
				if(minValue >= intervalMapping.Limits.Max || maxValue <= intervalMapping.Limits.Min)
					continue;
				RectangleF rect = MathUtils.MakeRectangle((PointF)intervalMapping.GetNearScreenPoint(minValue, 0, 0), (PointF)intervalMapping.GetFarScreenPoint(maxValue, 0, 0));
				if(rect.AreWidthAndHeightPositive())
					intervalViewData.StripsViewData.Add(new StripViewData(strip, rect));
			}
		}
		void CalculateConstantLinesViewData(TextMeasurer textMeasurer, bool showBehind) {
			IList<ConstantLine> constantLines = axis.ConstantLines.GetSubsetByShowBehindProperty(showBehind);
			foreach (ConstantLine constantLine in constantLines)
				if (constantLine.ShouldBeDrawnOnDiagram)
					CalculateConstantLineViewData(constantLine, textMeasurer, showBehind);
		}
		void CalculateConstantLineViewData(ConstantLine constantLine, TextMeasurer textMeasurer, bool showBehind) {
			double value = ((IAxisValueContainer)constantLine).GetValue();
			foreach(AxisIntervalViewData intervalViewData in intervalsViewData)
				if(intervalViewData.IntervalMapping.IntervalLayout.ValueWithinRange(value)) {
					ConstantLineViewData constantLineViewData = new ConstantLineViewData(textMeasurer, constantLine, intervalViewData.IntervalMapping, value);
					if(showBehind)
						intervalViewData.BackConstantLinesViewData.Add(constantLineViewData);
					else
						intervalViewData.FrontConstantLinesViewData.Add(constantLineViewData);
					return;
				}
		}
		bool CanApplyOverlappingCache(List<IAxisLabelLayout> labels, XYDiagram2D diagram) {
			return (diagram != null) && diagram.IsScrollingEnabled && (Axis.OverlappingCache != null) && Axis.OverlappingCache.Count == labels.Count;
		}
		bool ShouldLimitLabels() {
			bool shouldLimitLabels = false;
			foreach (AxisIntervalViewData intervalViewData in IntervalsViewData) {
				AxisLabelViewData labelViewData = intervalViewData.LabelViewData;
				if (labelViewData != null && labelViewData.ShouldLimitLabels) {
					shouldLimitLabels = true;
					break;
				}
			}
			return shouldLimitLabels;
		}
		List<IAxisLabelLayout> GetAxisLabelsLayoutList() {
			List<IAxisLabelLayout> labels = new List<IAxisLabelLayout>();
			foreach (AxisIntervalViewData intervalViewData in IntervalsViewData) {
				AxisLabelViewData labelViewData = intervalViewData.LabelViewData;
				if (labelViewData == null)
					continue;
				foreach (AxisLabelItemBase item in labelViewData.Items) {
					IAxisLabelLayout itemLayout = item as IAxisLabelLayout;
					if (itemLayout != null)
						labels.Add(itemLayout);
				}
			}
			return labels;
		}
		public void ProcessOverlappingAxisLabels() {
			AxisLabel labelSettings = Axis.Label;
			if (labelSettings != null) {
				List<IAxisLabelLayout> labels = GetAxisLabelsLayoutList();
				XYDiagram2D diagram = Axis.Diagram as XYDiagram2D;
				AxisLabelOverlappingResolver resolver = AxisLabelsResolveOverlappingHelper.CreateOverlappingResolver(labels, Axis, Axis.Alignment);
				if (labelSettings.ResolveOverlappingOptions.AllowResolveOverlapping || labelSettings.Staggered) {
					if (CanApplyOverlappingCache(labels, diagram))
						Axis.OverlappingCache.Apply(labels);
					else {
						resolver.Process();
						if (axisLabelResolveOverlappingCache == null)
							axisLabelResolveOverlappingCache = new List<AxisLabelResolveOverlappingCache>();
						axisLabelResolveOverlappingCache.Add(new AxisLabelResolveOverlappingCache(Axis, labels));
					}
				}
				if (ShouldLimitLabels())
					resolver.ProcessLabelsLimits(paneBounds.ToGRealRect2D());
				resolver.ProcessCustomLabels();
			}
		}
		public void UpdateCache() {
			if (axisLabelResolveOverlappingCache != null)
				foreach (AxisLabelResolveOverlappingCache cache in axisLabelResolveOverlappingCache)
					cache.Store();
		}
		public Rectangle CaluclateLabelBounds() {
			Rectangle labelBounds = Rectangle.Empty;
			foreach (AxisIntervalViewData intervalViewData in intervalsViewData) {
				AxisLabelViewData labelViewData = intervalViewData.LabelViewData;
				if (labelViewData != null) {
					Rectangle intervalLabelBounds = labelViewData.CalculateBounds();
					if (!intervalLabelBounds.IsEmpty)
						labelBounds = labelBounds.IsEmpty ? intervalLabelBounds : Rectangle.Union(labelBounds, intervalLabelBounds);
				}
			}
			return labelBounds;
		}
		public void CalculateDiagramBoundsCorrection(RectangleCorrection correction) {
			foreach(AxisIntervalViewData intervalViewData in intervalsViewData)
				intervalViewData.CalculateDiagramBoundsCorrection(correction);
			if (titleViewData != null && axis.Title.ActualVisibility)
				titleViewData.CalculateDiagramBoundsCorrection(correction);
		}
		public void Render(IRenderer renderer) {
			AxisIntervalViewData previousIntervalViewData = null;
			foreach (AxisIntervalViewData intervalViewData in intervalsViewData) {
				intervalViewData.Render(renderer, previousIntervalViewData);
				previousIntervalViewData = intervalViewData;
			}
			if (titleViewData != null && axis.Title.ActualVisibility)
				titleViewData.Render(renderer);
		}
		public void RenderMiddle(IRenderer renderer) {
			AxisIntervalViewData previousIntervalViewData = null;
			foreach (AxisIntervalViewData intervalViewData in intervalsViewData) {
				intervalViewData.RenderMiddle(renderer, previousIntervalViewData);
				previousIntervalViewData = intervalViewData;
			}
		}
		public void RenderInterlacedGraphics(IRenderer renderer) {
			foreach (AxisIntervalViewData intervalViewData in intervalsViewData)
				intervalViewData.RenderInterlacedGraphics(renderer);
		}
		public void RenderGridLines(IRenderer renderer, DiagramAppearance appearance) {
			foreach (AxisIntervalViewData intervalViewData in intervalsViewData)
				intervalViewData.RenderGridLines(renderer, appearance);
		}
		public void RenderStrips(IRenderer renderer) {
			foreach (AxisIntervalViewData intervalViewData in intervalsViewData)
				intervalViewData.RenderStrips(renderer);
		}
		public void RenderBackConstantLines(IRenderer renderer, HitRegionContainer paneHitContainer) {
			foreach (AxisIntervalViewData intervalViewData in intervalsViewData)
				intervalViewData.RenderBackConstantLines(renderer, paneHitContainer);
		}
		public void RenderBackConstantLinesTitles(IRenderer renderer) {
			foreach (AxisIntervalViewData intervalViewData in intervalsViewData)
				intervalViewData.RenderBackConstantLinesTitles(renderer);
		}
		public void RenderFrontConstantLines(IRenderer renderer, HitRegionContainer paneHitContainer) {
			foreach (AxisIntervalViewData intervalViewData in intervalsViewData)
				intervalViewData.RenderFrontConstantLines(renderer, paneHitContainer);
		}
		public void RenderFrontConstantLinesTitles(IRenderer renderer) {
			foreach (AxisIntervalViewData intervalViewData in intervalsViewData)
				intervalViewData.RenderFrontConstantLinesTitles(renderer);
		}
	}
}
