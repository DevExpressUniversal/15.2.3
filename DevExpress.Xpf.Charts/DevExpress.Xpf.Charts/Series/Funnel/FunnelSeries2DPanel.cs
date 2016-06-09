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

using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Charts {
	public class FunnelSeries2DPanel : Panel {
		#region Nested class: FunnelSeries2DElements
		class FunnelSeries2DElements {
			readonly UIElementCollection elementCollection;
			public PointsContainer PointsContainer {
				get { return FindElementByType(typeof(PointsContainer)) as PointsContainer; }
			}
			public SeriesLabel Label {
				get { return FindElementByType(typeof(SeriesLabel)) as SeriesLabel; }
			}
			public IEnumerable<UIElement> LayoutElements {
				get {
					foreach (UIElement element in elementCollection)
						if (element is LayoutElementPresentation)
							yield return element;
				}
			}
			UIElement FindElementByType(Type elementType) {
				foreach (UIElement element in elementCollection) {
					if (element.GetType() == elementType)
						return element;
				}
				return null;
			}
			public FunnelSeries2DElements(UIElementCollection elementCollection) {
				this.elementCollection = elementCollection;
			}
		}
		#endregion
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		FunnelSeries2DElements seriesElements;
		Rect pointsBounds;
		FunnelSeries2D GetSeries() {
			FunnelSeries2DItemsControl parentItemsControl = LayoutHelper.FindParentObject<FunnelSeries2DItemsControl>(this);
			return parentItemsControl != null ? (FunnelSeries2D)parentItemsControl.Series : null;
		}
		Rect CorrectPointsBounds(Rect initialBouds, Thickness padding) {
			double newWidth = initialBouds.Width - padding.Left - padding.Right;
			if (newWidth < 0)
				newWidth = 0;
			double newHeight = pointsBounds.Height - padding.Top - padding.Bottom;
			if (newHeight < 0)
				newHeight = 0;
			return new Rect(initialBouds.Left + padding.Left, initialBouds.Top + padding.Top, newWidth, newHeight);
		}
		void CalculateSeriesLabelsLayout(SeriesLabel seriesLabel, Rect viewport, Rect labelBounds) {
			FunnelSeries2D series = GetSeries();
			if (seriesLabel != null && series != null) {
				if (seriesLabel.Items != null) {
					double maxLabelWidth = 0;
					foreach (SeriesLabelItem labelItem in seriesLabel.Items) {
						if (maxLabelWidth < labelItem.LabelSize.Width)
							maxLabelWidth = labelItem.LabelSize.Width;
					}
					foreach (SeriesLabelItem labelItem in seriesLabel.Items) {
						labelItem.Layout = series.CreateSeriesLabelLayout(viewport, labelBounds, labelItem, maxLabelWidth);
						labelItem.UpdateConnectorItemLayout();
					}
				}
			}
		}
		void MeasureLayoutElements(Size constraint) {
			foreach (UIElement element in seriesElements.LayoutElements) {
				ILayoutElement layoutElement = element as ILayoutElement;
				if (layoutElement != null)
					element.Measure(constraint);
			}
		}
		void ArrangeLayoutElements(Size finalSize) {
			foreach (UIElement element in seriesElements.LayoutElements) {
				ILayoutElement layoutElement = element as ILayoutElement;
				if (layoutElement != null) {
					bool isLayoutElementVisible = (layoutElement.Layout != null && layoutElement.Layout.Visible && pointsBounds.Width > 0 && pointsBounds.Height > 0);
					Rect layoutElementRect;
					if (!isLayoutElementVisible)
						layoutElementRect = RectExtensions.Zero;
					else if (layoutElement.Layout.Bounds.IsEmpty)
						layoutElementRect = new Rect(0, 0, finalSize.Width, finalSize.Height);
					else
						layoutElementRect = layoutElement.Layout.Bounds;
					element.Arrange(layoutElementRect);
				}
			}
		}
		void UpdateSeriesLabelsConnectors() {
			SeriesLabel label = seriesElements.Label;
			if (label != null && label.Items != null) {
				foreach (SeriesLabelItem labelItem in label.Items)
					labelItem.UpdateConnectorCoordinates();
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			Rect labelBounds = new Rect(0, 0, constraint.Width, constraint.Height);
			pointsBounds = labelBounds;
			seriesElements = new FunnelSeries2DElements(Children);
			MeasureLayoutElements(constraint);
			FunnelSeries2D series = GetSeries();
			SeriesLabel label = series.ActualLabel;
			Thickness padding = CalculateLabelPadding(label, series, labelBounds);
			pointsBounds = CorrectPointsBounds(labelBounds, padding);
			Rect newPointsBounds = CorrectBoundsBySizeAndRatio(series, pointsBounds);
			labelBounds = CorrectLabelBounds(labelBounds, pointsBounds, newPointsBounds);
			pointsBounds = newPointsBounds;
			Rect viewport = new Rect(-pointsBounds.Left, -pointsBounds.Top, constraintWidth, constraintHeight);
			if (series != null)
				series.Viewport = viewport;
			ItemsControl pointsContainer = seriesElements.PointsContainer;
			if (pointsContainer != null) {
				pointsContainer.Clip = new RectangleGeometry() { Rect = viewport };
				pointsContainer.Measure(new Size(pointsBounds.Width, pointsBounds.Height));
			}
			CalculateSeriesLabelsLayout(label, pointsBounds, labelBounds);
			return constraint;
		}
		Thickness CalculateLabelPadding(SeriesLabel label, FunnelSeries2D series, Rect labelBounds) {
			Thickness padding;
			if (label != null && series != null) {
				Funnel2DLabelPosition labelPosition = FunnelSeries2D.GetLabelPosition(label);
				List<FunnelSeries2DPointLayout> layouts = series.GetSeriesPointsLayout(labelBounds.Size);
				if (label == null || layouts == null || label.Items == null || label.Items.Count != layouts.Count)
					return new Thickness(0);
				List<FunnelLableInfo> labelInfos = new List<FunnelLableInfo>();
				for (int i = 0; i < label.Items.Count; i++) {
					SeriesLabelItem item = label.Items[i];
					labelInfos.Add(new FunnelLableInfo(item.RefinedPoint, new GRealSize2D(item.LabelSize.Width, item.LabelSize.Height), layouts[i].PointInfo));
				}
				Thickness correctionByLabels = Funnel2DLabelCorrectionCalulator.CalculateMaxLabelPadding(labelBounds, series.AdditionalLengthCalculator, labelInfos, label.Indent, series.AlignToCenter, series.HeightToWidthRatioAuto, labelPosition);
				padding = new Thickness(correctionByLabels.Left, correctionByLabels.Top, correctionByLabels.Right, correctionByLabels.Bottom);
			}
			else
				padding = new Thickness(1);
			return padding;
		}
		Rect CorrectLabelBounds(Rect labelBounds, Rect pointsBounds, Rect newPointsBounds) {
			if (pointsBounds.Width > newPointsBounds.Width) {
				double deltaWidth = pointsBounds.Width - newPointsBounds.Width;
				return new Rect(labelBounds.X + deltaWidth / 2, labelBounds.Y, labelBounds.Width - deltaWidth, labelBounds.Height);
			}
			return labelBounds;
		}
		Rect CorrectBoundsBySizeAndRatio(FunnelSeries2D series, Rect pointsBounds) {
			Size newSize = series.CorrectBoundsBySizeAndRatio(pointsBounds.Size);
			if (pointsBounds.Width > newSize.Width) {
				double deltaWidth = pointsBounds.Width - newSize.Width;
				return new Rect(pointsBounds.X + deltaWidth / 2, pointsBounds.Y, pointsBounds.Width - deltaWidth, pointsBounds.Height);
			}
			else if (pointsBounds.Height > newSize.Height) {
				double deltaHeight = pointsBounds.Height - newSize.Height;
				return new Rect(pointsBounds.X, pointsBounds.Y + deltaHeight / 2, pointsBounds.Width, pointsBounds.Height - deltaHeight);
			}
			return pointsBounds;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			ArrangeLayoutElements(finalSize);
			UpdateSeriesLabelsConnectors();
			ItemsControl pointsContainer = seriesElements.PointsContainer;
			if (pointsContainer != null)
				pointsContainer.Arrange(pointsBounds);
			return finalSize;
		}
	}
}
