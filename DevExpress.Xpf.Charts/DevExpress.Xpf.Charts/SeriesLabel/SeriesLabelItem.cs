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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class SeriesLabelConnectorItem : NotifyPropertyChangedObject {
		readonly WeakReference labelItem;
		SeriesLabel2DConnectorLayout layout;
		List<Point> points;
		bool IsBrokenLine {
			get {
				if (LabelItem.Layout is IPieLabelLayout)
					return PieSeries.GetLabelPosition(LabelItem.Label) == PieLabelPosition.TwoColumns;
				else
					return false;
			}
		}
		public SeriesLabel2DConnectorLayout Layout {
			get { return layout; }
			set {
				layout = value;
				OnPropertyChanged("Layout");
			}
		}
		public List<Point> Points {
			get { return points; }
			set {
				points = value;
				OnPropertyChanged("Points");
			}
		}
		public SeriesLabelItem LabelItem { get { return labelItem.Target as SeriesLabelItem; } }
		internal SeriesLabelConnectorItem(SeriesLabelItem labelItem) {
			this.labelItem = new WeakReference(labelItem);
		}
		GRealPoint2D CalculateConnectorFinishPoint(GRealPoint2D anchorPoint, GRect2D labelBounds, GRealPoint2D labelCenter, GRealPoint2D labelCorner1, GRealPoint2D labelCorner2) {
			IntersectionInfo intersection;
			switch (LabelItem.Label.RenderMode) {
				case LabelRenderMode.RectangleConnectedToCenter:
					return labelCenter;
				case LabelRenderMode.Rectangle:
					intersection = GeometricUtils.CalcLineSegmentWithRectIntersection(anchorPoint, labelCenter, labelCorner1, labelCorner2);
					return intersection.HasIntersection ? intersection.IntersectionPoint : labelCenter;
				case LabelRenderMode.CustomShape:
					ChartContentPresenter presenter = LayoutHelper.FindElement(LabelItem.Presentation, element => element is ChartContentPresenter) as ChartContentPresenter;
					if (presenter != null && VisualTreeHelper.GetChildrenCount(presenter) > 0) {
						Border border = VisualTreeHelper.GetChild(presenter, 0) as Border;
						if (border != null && GraphicsUtils.IsSimpleBorder(border)) {
							intersection = GeometricUtils.CalcLineSegmentWithRoundedRectIntersection(anchorPoint, labelCenter, labelCorner1, labelCorner2,
								border.CornerRadius.BottomLeft, border.CornerRadius.TopLeft, border.CornerRadius.TopRight, border.CornerRadius.BottomRight);
							return intersection.HasIntersection ? intersection.IntersectionPoint : labelCenter;
						}
					}
					return IntersectionUtils.CalcIntersectionPointWithCustomShape(LabelItem.Presentation, anchorPoint, labelCenter, labelBounds);
				default:
					ChartDebug.Fail("Unexpected ConnectorTargetKind.");
					goto case LabelRenderMode.RectangleConnectedToCenter;
			}
		}
		bool AssertPoints(List<Point> points) {
			if (Points == null || Points.Count != points.Count)
				return false;
			for (int i = 0; i < Points.Count; i++)
				if (Points[i] != points[i])
					return false;
			return true;
		}
		List<Point> CalculateConnectorPoints(GRealPoint2D anchorPoint, GRect2D labelBounds) {
			GRealPoint2D connectorFinishPoint = CalculateConnectorFinishPoint(anchorPoint, labelBounds, MathUtils.CalcCenter(labelBounds),
				new GRealPoint2D(labelBounds.Left, labelBounds.Top), new GRealPoint2D(labelBounds.Right, labelBounds.Bottom));
			GRealPoint2D p1 = anchorPoint;
			GRealPoint2D p2 = connectorFinishPoint;
			MathUtils.CorrectSmoothLine(LabelItem.Label.ConnectorThickness, ref p1, ref p2);
			return new List<Point>() { new Point(p1.X, p1.Y), new Point(p2.X, p2.Y) };
		}
		List<Point> CalculateBrokenConnectorPoints(GRealPoint2D anchorPoint, GRect2D labelBounds) {
			GRealPoint2D start = new GRealPoint2D(0.0, 0.0), finish = new GRealPoint2D(0.0, 0.0);
			GRealPoint2D center = MathUtils.CalcCenter(labelBounds);
			GRealPoint2D? medium = ResolveOverlappingHelper.CalculateSalientPoint(anchorPoint, labelBounds, ((IPieLabelLayout)LabelItem.Layout).Angle);
			GRealPoint2D connectorFinishPoint = CalculateConnectorFinishPoint(medium.HasValue ? medium.Value : anchorPoint, labelBounds, MathUtils.CalcCenter(labelBounds),
				new GRealPoint2D(labelBounds.Left, labelBounds.Top), new GRealPoint2D(labelBounds.Right, labelBounds.Bottom));
			double thickness = LabelItem.Label.ConnectorThickness;
			if (medium.HasValue) {
				GRealPoint2D p1 = anchorPoint;
				GRealPoint2D p2 = medium.Value;
				GRealPoint2D p3 = connectorFinishPoint;
				MathUtils.CorrectSmoothLine(thickness, ref p1, ref p2);
				MathUtils.CorrectSmoothLine(thickness, ref p2, ref p3);
				return new List<Point>() { new Point(p1.X, p1.Y), new Point(p2.X, p2.Y), new Point(p3.X, p3.Y) };
			}
			else {
				GRealPoint2D p1 = anchorPoint;
				GRealPoint2D p2 = connectorFinishPoint;
				MathUtils.CorrectSmoothLine(LabelItem.Label.ConnectorThickness, ref p1, ref p2);
				return new List<Point>() { new Point(p1.X, p1.Y), new Point(p2.X, p2.Y) };
			}
		}
		internal void UpdateCoordinates(GRealPoint2D anchorPoint, GRect2D labelBounds) {
			List<Point> connectorPoints = IsBrokenLine ? CalculateBrokenConnectorPoints(anchorPoint, labelBounds) : CalculateConnectorPoints(anchorPoint, labelBounds);
			if (!AssertPoints(connectorPoints))
				Points = connectorPoints;
		}
	}
	[NonCategorized]
	public class SeriesLabelItem : NotifyPropertyChangedObject, ILayoutElement, ISupportFlowDirection {
		string text;
		Color color;
		SeriesLabel2DLayout layout;
		double opacity;
		readonly WeakReference label;
		readonly SolidColorBrush pointBrush;
		SeriesLabelConnectorItem connectorItem;
		SeriesLabelPresentation labelPresentation;
		SeriesPointItem pointItem;
		bool showConnector = true;
		SeriesPoint seriesPoint;
		public string Text {
			get { return text; }
			set {
				text = value;
				OnPropertyChanged("Text");
			}
		}
		public Color Color {
			get { return color; }
			set {
				color = value;
				OnPropertyChanged("Color");
			}
		}
		public SeriesLabel2DLayout Layout {
			get { return layout; }
			set {
				layout = value;
				OnPropertyChanged("Layout");
			}
		}
		public double Opacity {
			get { return opacity; }
			set {
				double oldValue = opacity;
				opacity = value;
				OpacityPropertyChanged(oldValue, opacity);
				OnPropertyChanged("Opacity");
			}
		}
		public SeriesPoint SeriesPoint {
			get { return seriesPoint; }
			private set {
				seriesPoint = value;
				OnPropertyChanged("SeriesPoint");
			}
		}
		public SeriesLabel Label { get { return label.Target as SeriesLabel; } }
		public SolidColorBrush PointBrush { get { return pointBrush; } }
		public SeriesLabelConnectorItem ConnectorItem {
			get {
				if (showConnector)
					return connectorItem;
				return null;
			}
			set { connectorItem = value; }
		}
		internal bool ShowConnector {
			get { return showConnector; }
			set { showConnector = value; }
		}
		internal Size LabelSize { get { return labelPresentation != null ? labelPresentation.DesiredSize : new Size(0, 0); } }
		internal SeriesLabelPresentation Presentation {
			get { return labelPresentation; }
			set { labelPresentation = value; }
		}
		internal SeriesPointItem PointItem {
			get { return pointItem; }
			set {
				pointItem = value;
				Text = pointItem.PresentationData.LabelText;
				Color = pointItem.PresentationData.PointColor;
				SeriesPoint = SeriesPoint.GetSeriesPoint(pointItem.SeriesPointData.SeriesPoint);
			}
		}
		internal RefinedPoint RefinedPoint { get { return pointItem.RefinedPoint; } }
		ILayout ILayoutElement.Layout { get { return Layout; } }
		internal SeriesLabelItem(SeriesLabel label) {
			this.label = new WeakReference(label);
		}
		internal SeriesLabelItem(SeriesLabel label, string text, Color pointColor)
			: this(label) {
			Text = text;
			Color = pointColor;
			this.pointBrush = new SolidColorBrush(pointColor);
		}
		void OpacityPropertyChanged(double oldValue, double newValue) {
			bool wasVisible = oldValue != 0;
			bool isVisible = newValue != 0;
			if (wasVisible != isVisible)
				UpdateConnectorCoordinates();
		}
		internal void UpdateConnectorItemLayout() {
			if (ConnectorItem == null)
				return;
			SeriesLabel2DLayout labelItemLayout = Layout;
			if (labelItemLayout != null) {
				ConnectorItem.Layout = labelItemLayout.IsVisibleForResolveOverlapping ? new SeriesLabel2DConnectorLayout(Rect.Empty) : null;
				ConnectorItem.UpdateCoordinates(labelItemLayout.AnchorPoint, labelItemLayout.LabelBounds);
			}
			else
				ConnectorItem.Layout = null;
		}
		internal void UpdateConnectorCoordinates() {
			if (ConnectorItem != null && Layout != null && Layout.Visible)
				ConnectorItem.UpdateCoordinates(Layout.AnchorPoint, Layout.LabelBounds);
		}
		Transform ISupportFlowDirection.CreateDirectionTransform() {
			ISupportFlowDirection flowDirectionLabel = Label as ISupportFlowDirection;
			if (label != null)
				return flowDirectionLabel.CreateDirectionTransform();
			else {
				ChartDebug.Fail("Label must support ISupportFlowDirection interface.");
				return null;
			}
		}
	}
	[NonCategorized]
	public class SeriesLabelPresentation : ChartElementBase, ILayoutElement, IHitTestableElement {
		readonly SeriesLabelItem labelItem;
		public SeriesLabelItem LabelItem { get { return labelItem; } }
		public SeriesLabel Label { get { return LabelItem != null ? LabelItem.Label : null; } }
		ILayout ILayoutElement.Layout { get { return LabelItem != null ? LabelItem.Layout : null; } }
		#region IHitTestableElement implementation
		object IHitTestableElement.Element { get { return Label; } }
		object IHitTestableElement.AdditionalElement { get { return LabelItem != null ? LabelItem.RefinedPoint : null; } }
		#endregion
		internal SeriesLabelPresentation(SeriesLabelItem labelItem) {
			DefaultStyleKey = typeof(SeriesLabelPresentation);
			this.labelItem = labelItem;
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (LabelItem != null) {
				LabelItem.Presentation = this;
				LabelItem.Opacity = LabelItem.PointItem.SeriesPointData.GetLabelOpacity();
			}
			return base.MeasureOverride(availableSize);
		}
	}
	[NonCategorized]
	public class SeriesLabelConnectorPresentation : ChartElementBase, ILayoutElement, IHitTestableElement {
		readonly SeriesLabelConnectorItem connectorItem;
		public SeriesLabelConnectorItem ConnectorItem { get { return connectorItem; } }
		public SeriesLabel Label { get { return ConnectorItem != null ? ConnectorItem.LabelItem.Label : null; } }
		public SeriesLabelItem LabelItem { get { return ConnectorItem != null ? ConnectorItem.LabelItem : null; } }
		ILayout ILayoutElement.Layout { get { return ConnectorItem != null ? ConnectorItem.Layout : null; } }
		#region IHitTestableElement implementation
		object IHitTestableElement.Element { get { return Label; } }
		object IHitTestableElement.AdditionalElement { get { return LabelItem != null ? LabelItem.SeriesPoint : null; } }
		#endregion
		internal SeriesLabelConnectorPresentation(SeriesLabelConnectorItem connectorItem) {
			DefaultStyleKey = typeof(SeriesLabelConnectorPresentation);
			this.connectorItem = connectorItem;
		}
	}
}
