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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class PieSeries2DPanel : Panel {
		#region Nested class: PieSeries2DElements
		class PieSeries2DElements {
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
			public PieSeries2DElements(UIElementCollection elementCollection) {
				this.elementCollection = elementCollection;
			}
		}
		#endregion
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		PieSeries2DElements seriesElements;
		Rect pointsBounds;
		PieSeries2D GetSeries() {
			PieSeries2DItemsControl parentItemsControl = LayoutHelper.FindParentObject<PieSeries2DItemsControl>(this);
			return parentItemsControl != null ? (PieSeries2D)parentItemsControl.Series : null;
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
			Point pieCenter = new Point(0.5 * (viewport.Left + viewport.Right), 0.5 * (viewport.Top + viewport.Bottom));
			double maxRadius = 0.5 * Math.Min(viewport.Width, viewport.Height);
			PieSeries2D series = GetSeries();
			List<IPieLabelLayout> labels = new List<IPieLabelLayout>();
			if (seriesLabel != null && series != null) {
				if (seriesLabel.Items != null) {
					foreach (SeriesLabelItem labelItem in seriesLabel.Items) {
						labelItem.Layout = series.CreateSeriesLabelLayout(pieCenter, maxRadius, labelBounds, labelItem);
						labelItem.UpdateConnectorItemLayout();
						if (labelItem.Layout != null)
							labels.Add((IPieLabelLayout)(labelItem.Layout));
					}
					if (series.LabelsResolveOverlapping) {
						ResolveOverlappingHelper.Process(PieSeries.GetLabelPosition(seriesLabel), labels, labelBounds,
							new GRealEllipse(new GRealPoint2D(pieCenter.X, pieCenter.Y), maxRadius + seriesLabel.Indent, maxRadius + seriesLabel.Indent), series.LabelsResolveOverlappingMinIndent, series.SweepDirection);
						foreach (SeriesLabelItem labelItem in seriesLabel.Items)
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
			SeriesLabel label = GetSeries().ActualLabel;
			if (label != null && label.Items != null) {
				foreach (SeriesLabelItem labelItem in label.Items)
					labelItem.UpdateConnectorCoordinates();
			}
		}
		bool ShouldCorrectByLabels(Series series, SeriesLabel label) {
			var nestedDonutSeries = series as INestedDoughnutSeriesView;
			if (nestedDonutSeries != null) {
				if (!nestedDonutSeries.IsOutside.HasValue)
					throw new InvalidOperationException("Core calculations shold be performed before. IsOutside status should be known.");
				if (nestedDonutSeries.IsOutside.Value == false)
					return false;
			}
			return label != null && series != null && label.Items.Count > 0;
		}
		protected override Size MeasureOverride(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			Rect labelBounds = new Rect(0, 0, constraint.Width, constraint.Height);
			pointsBounds = labelBounds;
			seriesElements = new PieSeries2DElements(Children);
			MeasureLayoutElements(constraint);
			PieSeries2D series = GetSeries();
			SeriesLabel label = series.ActualLabel;
			Thickness correctionByLabels = new Thickness(0);
			if (ShouldCorrectByLabels(series, label))
				correctionByLabels = Pie2DCalulator.CalculateMaxLabelPadding(labelBounds, series, label);
			pointsBounds = CorrectPointsBounds(labelBounds, correctionByLabels);
			Rect viewport = new Rect(-pointsBounds.Left, -pointsBounds.Top, constraintWidth, constraintHeight);
			if (series != null)
				series.Viewport = viewport;
			CalculateSeriesLabelsLayout(label, pointsBounds, labelBounds);
			ItemsControl pointsContainer = seriesElements.PointsContainer;
			if (pointsContainer != null) {
				pointsContainer.Clip = new RectangleGeometry() { Rect = viewport };
				pointsContainer.Measure(new Size(pointsBounds.Width, pointsBounds.Height));
			}
			return constraint;
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
	public class PieSeries2DItemsControl : Diagram2DItemsControl {
		#region Dependency properties
		public static readonly DependencyProperty SeriesProperty = DependencyPropertyManager.Register("Series",
			typeof(Series), typeof(PieSeries2DItemsControl));
		public static readonly DependencyProperty PointsContainerProperty = DependencyPropertyManager.Register("PointsContainer",
			typeof(ItemsControl), typeof(PieSeries2DItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty SeriesLabelProperty = DependencyPropertyManager.Register("SeriesLabel",
			typeof(SeriesLabel), typeof(PieSeries2DItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty SeriesLabelItemsProperty = DependencyPropertyManager.Register("SeriesLabelItems",
			typeof(ObservableCollection<SeriesLabelItem>), typeof(PieSeries2DItemsControl), new PropertyMetadata(PropertyChanged));
		#endregion
		[
		Category(Categories.Common)
		]
		public Series Series {
			get { return (PieSeries2D)GetValue(SeriesProperty); }
			set { SetValue(SeriesProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public ItemsControl PointsContainer {
			get { return (ItemsControl)GetValue(PointsContainerProperty); }
			set { SetValue(PointsContainerProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public SeriesLabel SeriesLabel {
			get { return (SeriesLabel)GetValue(SeriesLabelProperty); }
			set { SetValue(SeriesLabelProperty, value); }
		}
		[
		Category(Categories.Common),
		NonTestableProperty
		]
		public ObservableCollection<SeriesLabelItem> SeriesLabelItems {
			get { return (ObservableCollection<SeriesLabelItem>)GetValue(SeriesLabelItemsProperty); }
			set { SetValue(SeriesLabelItemsProperty, value); }
		}
		protected override ObservableCollection<object> CreateItems() {
			ObservableCollection<object> items = new ObservableCollection<object>();
			if (PointsContainer != null)
				items.Add(PointsContainer);
			if (SeriesLabelItems != null) {
				foreach (SeriesLabelItem labelItem in SeriesLabelItems)
					if (labelItem.ConnectorItem != null)
						items.Add(labelItem.ConnectorItem);
				foreach (SeriesLabelItem labelItem in SeriesLabelItems)
					items.Add(labelItem);
			}
			return items;
		}
	}
}
