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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class NestedDonut2DGroupItemsControl : Diagram2DItemsControl {
		public static readonly DependencyProperty NestedDonutGroupProperty =
			DependencyPropertyManager.Register("NestedDonutGroup", typeof(NestedDonut2DGroup), typeof(NestedDonut2DGroupItemsControl), new PropertyMetadata(PropertyChanged));
		[Category(Categories.Data)]
		public NestedDonut2DGroup NestedDonutGroup {
			get { return (NestedDonut2DGroup)GetValue(NestedDonutGroupProperty); }
			set { SetValue(NestedDonutGroupProperty, value); }
		}
		void SetBinding(FrameworkElement target, DependencyProperty property, object source, string path) {
			Binding binding = new Binding(path);
			binding.Source = source;
			target.SetBinding(property, binding);
		}
		PointsContainerPresenter CreatePointsContainerPresenter(SeriesItem seriesItem) {
			PointsContainerPresenter pointsContainerPresenter = new PointsContainerPresenter();
			SetBinding(pointsContainerPresenter, PointsContainerPresenter.ItemsSourceProperty, seriesItem, "VisiblePointItems");
			SetBinding(pointsContainerPresenter, PointsContainerPresenter.DataContextProperty, seriesItem, "Series");
			return pointsContainerPresenter;
		}
		protected override ObservableCollection<object> CreateItems() {
			NestedDonut2DGroup group = NestedDonutGroup;
			ObservableCollection<object> items = new ObservableCollection<object>();
			for (int i = 0; i < group.Count; i++ )
				items.Add(group[i]);
			foreach (SeriesLabelConnectorItem item in group.LabelConnectorItems)
				items.Add(item);
			foreach (SeriesLabelItem item in group.LabelItems)
				items.Add(item);
			return items;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new ChartContentPresenter();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			var presenter = (ChartContentPresenter)element;
			Type itemType = item.GetType();
			if (itemType == typeof(SeriesItem))
				presenter.Content = CreatePointsContainerPresenter((SeriesItem)item);
			else if (itemType == typeof(SeriesLabel)) {
				ChartControl chart = ((SeriesLabel)item).Series.Diagram.ChartControl;
				SetBinding(presenter, FrameworkElement.DataContextProperty, chart, "DataContext");
				presenter.Content = item;
			}
			else if (itemType == typeof(SeriesLabelItem) || itemType == typeof(SeriesLabelConnectorItem)) {
				LayoutElementPresentation layoutElementPresentation = new LayoutElementPresentation();
				layoutElementPresentation.Content = GetItemPresentation(item);
				presenter.Content = layoutElementPresentation;
			}
			else {
				MethodBase currentMethod = MethodBase.GetCurrentMethod();
				string message = string.Format(@"The '{0}.{1}' method cannot operate with an instance of the '{2}' type passed as 'item' parameter.", currentMethod.DeclaringType.Name, currentMethod.Name, itemType.Name);
				throw new ArgumentException(message);
			}
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return false;
		}
	}
	public class NestedDonut2DGroupPanel : Panel {
		Rect pointsBounds = RectExtensions.Zero;
		List<NestedDonutSeries2D> seriesList;
		List<ChartContentPresenter> pointsContainerList;
		List<ChartContentPresenter> labelsAndConnectorsOfOuterDonut;
		List<ChartContentPresenter> labelsAndConnectorsOfInnerDonuts;
		Size GetActualAvailableSize(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? 300 : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? 300 : availableSize.Height;
			return new Size(constraintWidth, constraintHeight);
		}
		void SplitChildrenIntoGroups() {
			seriesList = new List<NestedDonutSeries2D>();
			pointsContainerList = new List<ChartContentPresenter>();
			labelsAndConnectorsOfOuterDonut = new List<ChartContentPresenter>();
			labelsAndConnectorsOfInnerDonuts = new List<ChartContentPresenter>();
			for (int i = 0; i < Children.Count; i++) {
				var child = (ChartContentPresenter)Children[i];
				object element = child.Content;
				if (element is PointsContainerPresenter) {
					pointsContainerList.Add(child);
					var container = (PointsContainerPresenter)element;
					seriesList.Add((NestedDonutSeries2D)container.DataContext);
				}
				else if (element is LayoutElementPresentation) {
					ProcessLayoutElementPresentation(labelsAndConnectorsOfOuterDonut, labelsAndConnectorsOfInnerDonuts, (LayoutElementPresentation)element, child);
				}
				else {
					MethodBase currentMethod = MethodBase.GetCurrentMethod();
					string message = string.Format(@"The '{0}.{1}' method cannot operate with an instance of the '{2}' type contained in the 'Children' collection.", currentMethod.DeclaringType.Name, currentMethod.Name, element.GetType().Name);
					throw new ArgumentException(message);
				}
			}
		}
		void ProcessLayoutElementPresentation(List<ChartContentPresenter> labelsAndConnectorsOfOuterDonut, List<ChartContentPresenter> labelsAndConnectorsOfInnerDonuts, LayoutElementPresentation layoutElementPresentation, ChartContentPresenter contentPresenter) {
			object content = layoutElementPresentation.Content;
			if (content is SeriesLabelConnectorPresentation) {
				var presentation = (SeriesLabelConnectorPresentation)content;
				var series = (NestedDonutSeries2D)presentation.Label.Series;
				if (!series.IsOuter.HasValue)
					throw new InvalidOperationException("Core calculations should be performed before. IsOuter status shoul be determinated.");
				if (series.IsOuter.Value == true)
					labelsAndConnectorsOfOuterDonut.Add(contentPresenter);
				else
					labelsAndConnectorsOfInnerDonuts.Add(contentPresenter);
			}
			else if (content is SeriesLabelPresentation) {
				var presentation = (SeriesLabelPresentation)content;
				var series = (NestedDonutSeries2D)presentation.Label.Series;
				if (!series.IsOuter.HasValue)
					throw new InvalidOperationException("Core calculations should be performed before. IsOuter status shoul be determinated.");
				if (series.IsOuter.Value == true)
					labelsAndConnectorsOfOuterDonut.Add(contentPresenter);
				else
					labelsAndConnectorsOfInnerDonuts.Add(contentPresenter);
			}
		}
		Rect CorrectPointsBounds(Rect initialBouds, Thickness padding) {
			double newWidth = initialBouds.Width - padding.Left - padding.Right;
			if (newWidth < 0)
				newWidth = 0;
			double newHeight = initialBouds.Height - padding.Top - padding.Bottom;
			if (newHeight < 0)
				newHeight = 0;
			return new Rect(initialBouds.Left + padding.Left, initialBouds.Top + padding.Top, newWidth, newHeight);
		}
		void CalculateSeriesLabelsLayout(List<NestedDonutSeries2D> seriesList, Rect pointBounds, Rect availableBounds) {
			Point donutCenter = new Point(0.5 * (pointBounds.Left + pointBounds.Right), 0.5 * (pointBounds.Top + pointBounds.Bottom));
			double maxRadius = 0.5 * Math.Min(pointBounds.Width, pointBounds.Height);
			NestedDonutSeries2D outerDonut = seriesList[0];
			ChartDebug.Assert(outerDonut.IsOuter.Value == true);
			SeriesLabel outerDonutLabel = outerDonut.ActualLabel;
			CalculateLabelLayoutForSeries(outerDonut, availableBounds, donutCenter, maxRadius);
			for (int i = 1; i < seriesList.Count; i++)
				CalculateLabelLayoutForSeries(seriesList[i], pointBounds, donutCenter, maxRadius);
		}
		void CalculateLabelLayoutForSeries(NestedDonutSeries2D series, Rect availableBounds, Point donutCenter, double maxRadius) {
			SeriesLabel seriesLabel = series.ActualLabel;
			List<IPieLabelLayout> layoutList = new List<IPieLabelLayout>();
			if (seriesLabel.Items != null) {
				foreach (SeriesLabelItem labelItem in seriesLabel.Items) {
					labelItem.Layout = series.CreateSeriesLabelLayout(donutCenter, maxRadius, availableBounds, labelItem);
					labelItem.UpdateConnectorItemLayout();
					if (labelItem.Layout != null)
						layoutList.Add((IPieLabelLayout)(labelItem.Layout));
				}
				if (series.LabelsResolveOverlapping) {
					ResolveOverlappingHelper.Process(PieSeries.GetLabelPosition(seriesLabel), layoutList, availableBounds,
						new GRealEllipse(donutCenter.ToGRealPoint2D(), maxRadius + seriesLabel.Indent, maxRadius + seriesLabel.Indent), series.LabelsResolveOverlappingMinIndent, series.SweepDirection);
					foreach (SeriesLabelItem labelItem in seriesLabel.Items)
						labelItem.UpdateConnectorItemLayout();
				}
			}
		}
		void SetViewPortToAllSeries(Rect viewport, List<NestedDonutSeries2D> seriesList) {
			for (int i = 0; i < seriesList.Count; i++)
				seriesList[i].Viewport = viewport;
		}
		void UpdateSeriesLabelsConnectors(List<NestedDonutSeries2D> series) {
			for (int i = 0; i < series.Count; i++) {
				SeriesLabel label = series[i].ActualLabel;
				var items = label.Items;
				if (label != null && items != null)
					for (int j = 0; j < items.Count; j++)
						items[j].UpdateConnectorCoordinates();
			}
		}
		void MeasureLayoutElements(List<ChartContentPresenter> elements, Size size) {
			for (int i = 0; i < elements.Count; i++)
				elements[i].Measure(size);
		}
		void MeasurePointContainersAndSetClip(List<ChartContentPresenter> pointsContainerList, Size size, Rect clip) {
			for (int i = 0; i < pointsContainerList.Count; i++) {
				var container = pointsContainerList[i];
				container.Clip = new RectangleGeometry() { Rect = clip };
				container.Measure(size);
			}
		}
		void ArrangeLayoutElements(List<ChartContentPresenter> elements, Rect availableBounds) {
			for (int i = 0; i < elements.Count; i++) {
				ChartContentPresenter contentPresenter = elements[i];
				ILayoutElement layoutElement = (ILayoutElement)contentPresenter.Content;
				bool isLayoutElementVisible = (layoutElement.Layout != null && 
											   layoutElement.Layout.Visible && 
											   pointsBounds.Width  > 0	  && 
											   pointsBounds.Height > 0);
				Rect layoutElementRect;
				if (!isLayoutElementVisible)
					layoutElementRect = RectExtensions.Zero;
				else if (layoutElement.Layout.Bounds.IsEmpty)
					layoutElementRect = availableBounds;
				else
					layoutElementRect = layoutElement.Layout.Bounds;
				contentPresenter.Arrange(layoutElementRect);
			}
		}
		void ArrangePointContainers(List<ChartContentPresenter> pointContainers, Rect pointsBounds) {
			for (int i = 0; i < pointContainers.Count; i++)
				pointContainers[i].Arrange(pointsBounds);
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size actualAvailableSize = GetActualAvailableSize(availableSize);
			SplitChildrenIntoGroups();
			ChartDebug.Assert(pointsContainerList.Count + labelsAndConnectorsOfOuterDonut.Count + labelsAndConnectorsOfInnerDonuts.Count == Children.Count);
			ChartDebug.Assert(seriesList.Count == pointsContainerList.Count);
			MeasureLayoutElements(labelsAndConnectorsOfOuterDonut, actualAvailableSize);
			Rect availableBounds = new Rect(0, 0, actualAvailableSize.Width, actualAvailableSize.Height);
			ChartDebug.Assert(seriesList[0].IsOuter.Value == true);
			Thickness correctionByLabels = Pie2DCalulator.CalculateMaxLabelPadding(availableBounds, seriesList[0], seriesList[0].ActualLabel);
			pointsBounds = CorrectPointsBounds(availableBounds, correctionByLabels);
			MeasureLayoutElements(labelsAndConnectorsOfInnerDonuts, pointsBounds.Size());
			CalculateSeriesLabelsLayout(seriesList, pointsBounds, availableBounds);
			Rect viewport = new Rect(-pointsBounds.Left, -pointsBounds.Top, actualAvailableSize.Width, actualAvailableSize.Height);
			SetViewPortToAllSeries(viewport, seriesList);
			MeasurePointContainersAndSetClip(pointsContainerList, pointsBounds.Size(), viewport);
			return actualAvailableSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			List<ChartContentPresenter> layoutElements = new List<ChartContentPresenter>();
			layoutElements.AddRange(labelsAndConnectorsOfOuterDonut);
			layoutElements.AddRange(labelsAndConnectorsOfInnerDonuts);
			Rect availableBounds = new Rect(new Point(0, 0), finalSize);
			ArrangeLayoutElements(layoutElements, availableBounds);
			UpdateSeriesLabelsConnectors(seriesList);
			ArrangePointContainers(pointsContainerList, pointsBounds);
			return finalSize;
		}
	}
}
