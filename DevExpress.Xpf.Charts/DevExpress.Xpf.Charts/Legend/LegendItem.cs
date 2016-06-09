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
using System.Windows.Shapes;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class LegendItem : MarkerItem {
		public static readonly DependencyProperty LegendProperty = DependencyPropertyManager.Register("Legend", typeof(Legend), typeof(LegendItem));
#if !SL
	[DevExpressXpfChartsLocalizedDescription("LegendItemLegend")]
#endif
		public Legend Legend { 
			get { return (Legend)GetValue(LegendProperty); } 
			set { SetValue(LegendProperty, value); }
		}
		readonly Pen markerPen;
		readonly string text;
		readonly bool supportsHatchSelection;
		readonly bool supportsSizeSelection;
		readonly bool supportsBrightnessSelection;
		readonly object sourceElement;
		readonly HitTestableElement hitTestableElement;
		bool isSelected = false;
		bool isEnabled = true;
		internal HitTestableElement HitTestableElement { get { return hitTestableElement; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("LegendItemIsSelected")]
#endif
		public bool IsSelected {
			get { return isSelected; }
			internal set {
				if (isSelected != value) {
					isSelected = value;
					UpdateSelectedState();
					NotifyPropertyChanged("IsSelected");
				}
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("LegendItemText")]
#endif
		public string Text { get { return text; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("LegendItemChartElement")]
#endif
		public object ChartElement { get { return GetChartElement(); } }
		public bool CheckBoxVisible { get { return Legend.UseCheckBoxes && RepresentedElement.CheckableInLegend; } }
		public bool MarkerVisible { get { return !CheckBoxVisible; } }
		public bool IsEnabled { get { return isEnabled; } }
		public Brush CheckBoxBrush { get { return markerPen != null ? markerPen.Brush : MarkerBrush; } }
		internal LegendItem(ILegendVisible browsableObject, object sourceElement, string text, Brush markerBrush, Brush markerLineBrush, LineStyle markerLineStyle, VisualSelectionType selectionType, bool isEnabled)
			: base(browsableObject, markerBrush, markerLineBrush, markerLineStyle) {
				this.sourceElement = sourceElement;
			this.hitTestableElement = new HitTestableElement(sourceElement);
			this.text = text;
			this.isEnabled = isEnabled;
			this.supportsHatchSelection = VisualSelectionHelper.SupportsHatchSelection(selectionType);
			this.supportsSizeSelection = VisualSelectionHelper.SupportsSizeSelection(selectionType);
			this.supportsBrightnessSelection = VisualSelectionHelper.SupportsBrightnessSelection(selectionType);
			this.markerPen = markerLineBrush != null && markerLineStyle != null ? markerLineStyle.CreatePen(markerLineBrush) : null;
		}
		internal LegendItem(ILegendVisible browsableObject, object sourceElement, string text, Brush markerBrush)
			: this(browsableObject, sourceElement, text, markerBrush, null, null, VisualSelectionType.None, true) {
		}
		internal LegendItem(ILegendVisible browsableObject, object sourceElement, string text, Brush markerLineBrush, LineStyle markerLineStyle)
			: this(browsableObject, sourceElement, text, null, markerLineBrush, markerLineStyle, VisualSelectionType.None, true) {
		}
		internal LegendItem(ILegendVisible browsableObject, object sourceElement, string text, Brush markerBrush, Brush markerLineBrush, LineStyle markerLineStyle, bool isEnabled)
			: this(browsableObject, sourceElement, text, markerBrush, markerLineBrush, markerLineStyle, VisualSelectionType.None, isEnabled) {
		}
		object GetChartElement() {
			RefinedPoint refinedPoint = sourceElement as RefinedPoint;
			return refinedPoint != null ? SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint) : sourceElement;
		}
		protected override double GetActualLineThickness() {
			return supportsSizeSelection ?  VisualSelectionHelper.GetLegendLineThickness(MarkerLineStyle.Thickness, isSelected) : base.GetActualLineThickness();
		}
		protected override Brush GetOpacityMask() {
			return (isSelected && supportsHatchSelection) ? VisualSelectionHelper.LegendMarkerSelectionOpacityMask : base.GetOpacityMask();
		}
		protected override Brush GetActualMarkerBrush() {
			SolidColorBrush markerBrush = MarkerBrush as SolidColorBrush;
			if (markerBrush != null && isSelected && supportsBrightnessSelection)
				return new SolidColorBrush(VisualSelectionHelper.GetSelectedPointColor(markerBrush.Color));
			return base.GetActualMarkerBrush();
		}
		protected override Brush GetActualMarkerLineBrush() {
			SolidColorBrush markerBrush = MarkerLineBrush as SolidColorBrush;
			if (markerBrush != null && isSelected && supportsBrightnessSelection)
				return new SolidColorBrush(VisualSelectionHelper.GetSelectedPointColor(markerBrush.Color));
			return base.GetActualMarkerLineBrush();
		}
		protected override Thickness GetActualMarkerMargin() {
			return (isSelected && supportsSizeSelection) ? VisualSelectionHelper.SelectedLegendMarkerMargin : base.GetActualMarkerMargin();
		}
		void UpdateSelectedState() {
			if (supportsHatchSelection)
				NotifyPropertyChanged("OpacityMask");
			if (supportsBrightnessSelection) {
				NotifyPropertyChanged("ActualMarkerBrush");
				NotifyPropertyChanged("ActualMarkerLineBrush");
			}
			if (supportsSizeSelection) {
				OnLineThicknessChanged();
				NotifyPropertyChanged("ActualMarkerMargin");
			}
		}
	}
	[NonCategorized]
	public abstract class LegendItemPresentation : ChartElementBase {
		protected static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LegendItemPresentation legendItemPresentation = d as LegendItemPresentation;
			if (legendItemPresentation != null)
				legendItemPresentation.UpdateMarkerGeometry();
		}
		Path markerPath;
		protected Path MarkerPath { get { return markerPath; } }
		public LegendItemPresentation() {
			DefaultStyleKey = typeof(LegendItemPresentation);
			UpdateMarkerGeometry();
		}
		protected abstract Geometry CreateLegendMarkerGeometry();
		internal void UpdateMarkerGeometry() {
			if (markerPath != null)
				markerPath.Data = CreateLegendMarkerGeometry();
		}
		protected override Size MeasureOverride(Size availableSize) {
			UpdateMarkerGeometry();
			return base.MeasureOverride(availableSize);
		}
		public override void OnApplyTemplate() {
			markerPath = GetTemplateChild("PART_MarkerPath") as Path;
			if (markerPath != null)
				markerPath.Data = null;
			base.OnApplyTemplate();
		}
	}
	[NonCategorized]
	public class StockLegendItemPresentation : LegendItemPresentation {
		public static readonly DependencyProperty ShowOpenCloseProperty = DependencyPropertyManager.Register("ShowOpenClose",
			typeof(StockType), typeof(StockLegendItemPresentation), new PropertyMetadata(PropertyChanged));
		public StockType ShowOpenClose {
			get { return (StockType)GetValue(ShowOpenCloseProperty); }
			set { SetValue(ShowOpenCloseProperty, value); }
		}
		Geometry CreateLegendStockMarker(Point leftTopStockMarker) {
			GeometryGroup stockMarkerGeometry = new GeometryGroup();
			stockMarkerGeometry.Children.Add(new RectangleGeometry() { Rect = new Rect(leftTopStockMarker.X + 2, leftTopStockMarker.Y, 2, 10) });
			if (ShowOpenClose == StockType.Open || ShowOpenClose == StockType.Both)
				stockMarkerGeometry.Children.Add(new RectangleGeometry() { Rect = new Rect(leftTopStockMarker.X, leftTopStockMarker.Y + 2, 2, 2) });
			if (ShowOpenClose == StockType.Close || ShowOpenClose == StockType.Both)
				stockMarkerGeometry.Children.Add(new RectangleGeometry() { Rect = new Rect(leftTopStockMarker.X + 4, leftTopStockMarker.Y + 6, 2, 2) });
			return stockMarkerGeometry;
		}
		protected override Geometry CreateLegendMarkerGeometry() {
			GeometryGroup group = new GeometryGroup();
			group.Children.Add(CreateLegendStockMarker(new Point(0, 0)));
			group.Children.Add(CreateLegendStockMarker(new Point(6, 2)));
			return group;
		}
	}
	[NonCategorized]
	public class LineStepLegendItemPresentation : LegendItemPresentation {
		public static readonly DependencyProperty InvertedStepProperty = DependencyPropertyManager.Register("InvertedStep",
			typeof(bool), typeof(LineStepLegendItemPresentation), new PropertyMetadata(PropertyChanged));
		public bool InvertedStep {
			get { return (bool)GetValue(InvertedStepProperty); }
			set { SetValue(InvertedStepProperty, value); }
		}
		protected override Geometry CreateLegendMarkerGeometry() {
			PathGeometry geometry = new PathGeometry();
			PathFigure figure = new PathFigure();
			PolyLineSegment segment = new PolyLineSegment();
			segment.Points.Add(new Point(0, 10));
			if (InvertedStep) {
				segment.Points.Add(new Point(0, 0));
				segment.Points.Add(new Point(5, 0));
				segment.Points.Add(new Point(5, 5));
				segment.Points.Add(new Point(10, 5));
			}
			else {
				segment.Points.Add(new Point(0, 5));
				segment.Points.Add(new Point(5, 5));
				segment.Points.Add(new Point(5, 0));
				segment.Points.Add(new Point(10, 0));
			}
			segment.Points.Add(new Point(10, 10));
			figure.Segments.Add(segment);
			figure.StartPoint = ((PolyLineSegment)figure.Segments[0]).Points[0];
			figure.IsFilled = false;
			geometry.Figures.Add(figure);
			return geometry;
		}
	}
	[NonCategorized]
	public class AreaStepLegendItemPresentation : LineStepLegendItemPresentation {
		protected override Geometry CreateLegendMarkerGeometry() {
			PathGeometry geometry = new PathGeometry();
			PathFigure figure = new PathFigure();
			PolyLineSegment segment = new PolyLineSegment();
			segment.Points.Add(new Point(0, 11));
			if (InvertedStep) {
				segment.Points.Add(new Point(0, 0));
				segment.Points.Add(new Point(5, 0));
				segment.Points.Add(new Point(5, 5));
				segment.Points.Add(new Point(11, 5));
			}
			else {
				segment.Points.Add(new Point(0, 5));
				segment.Points.Add(new Point(5, 5));
				segment.Points.Add(new Point(5, 0));
				segment.Points.Add(new Point(11, 0));
			}
			segment.Points.Add(new Point(11, 11));
			figure.Segments.Add(segment);
			figure.StartPoint = ((PolyLineSegment)figure.Segments[0]).Points[0];
			geometry.Figures.Add(figure);
			return geometry;
		}
	}
}
