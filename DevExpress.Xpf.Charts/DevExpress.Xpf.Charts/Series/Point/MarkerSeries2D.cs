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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class MarkerSeries2D : XYSeries2D {
		public static readonly DependencyProperty AngleProperty = DependencyPropertyManager.RegisterAttached("Angle", 
			typeof(double), typeof(MarkerSeries2D), new PropertyMetadata(45.0, ChartElementHelper.Update));
		[
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public static double GetAngle(SeriesLabel label) {
			return (double)label.GetValue(AngleProperty);
		}
		public static void SetAngle(SeriesLabel label, double value) {
			label.SetValue(AngleProperty, value);
		}
		internal static bool MarkerSizeValidation(object size) {
			return (int)size > 0;
		}
		protected override Type PointInterfaceType { get { return typeof(IXYPoint); } }
		protected internal override bool IsLabelConnectorItemVisible { get { return ActualLabel.ConnectorVisible; } }
		protected internal override bool LabelsResolveOverlappingSupported { get { return true; } }
		protected internal override Type SupportedDiagramType { get { return typeof(XYDiagram2D); } }
		protected internal override VisualSelectionType SupportedSelectionType { get { return VisualSelectionType.Size; } }
		protected virtual int CalculateMarkerSize(PaneMapping mapping, RefinedPoint refinedPoint) { 
			return 0; 
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			MarkerSeries2D markerSeries2D = series as MarkerSeries2D;
			if (markerSeries2D != null) {
				if (Label != null && markerSeries2D.Label != null)
					MarkerSeries2D.SetAngle(Label, MarkerSeries2D.GetAngle(markerSeries2D.Label));
			}
		}
		protected override XYSeriesLabel2DLayout CreateSeriesLabelLayout(SeriesLabelItem labelItem, PaneMapping mapping, Transform transform) {
			Point anchorPoint = transform.Transform(mapping.GetRoundedDiagramPoint(labelItem.RefinedPoint.Argument, GetMaxValue(labelItem.RefinedPoint)));
			SeriesLabel label = labelItem.Label;
			int indent = label.Indent;
			return new XYSeriesLabel2DLayout(labelItem, mapping, anchorPoint, indent,
				MathUtils.Degree2Radian(NormalizeLabelAngle(GetAngle(label))), GraphicsUtils.ConvertRect(SeriesWithMarkerHelper.CalcAnchorHole(anchorPoint, indent)));
		}
		protected override SeriesPointLayout CreateSeriesPointLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			return CreatePointItemLayout(mapping, pointItem);
		}
		protected internal override void CompletePointLayout(SeriesPointItem pointItem) {
			SeriesWithMarkerHelper.CompletePointLayout(pointItem, Diagram, GetActualPointAnimation(), IsAxisXReversed, IsAxisYReversed);
		}
		protected internal override Point CalculateToolTipPoint(SeriesPointItem pointItem, PaneMapping mapping, Transform transform, bool inLabel) {
			return transform.Transform(mapping.GetRoundedDiagramPoint(pointItem.RefinedPoint.Argument, ((IValuePoint)pointItem.RefinedPoint).Value));
		}
		protected override SeriesPointLayout CreatePointItemLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			RefinedPoint refinedPoint = pointItem.RefinedPoint;
			int markerSize = CalculateMarkerSize(mapping, refinedPoint);
			return SeriesWithMarkerHelper.CreateSeriesPointLayout(mapping, pointItem, markerSize, GetMaxValue(refinedPoint));
		}
		protected virtual double GetMaxValue(RefinedPoint refinedPoint) {
			return ((IXYPoint)refinedPoint).Value;
		}
		protected void FillMarkerAnimationKinds(List<AnimationKind> animationKinds) {
			SeriesWithMarkerHelper.FillMarkerAnimationKinds(animationKinds);
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public static class SeriesWithMarkerHelper {
		const int highlightedSizeIncrement = 4;
		const double selectedSizeMultiplier = 1.5;
		static Rect CalculateHighlightedBounds(Rect initialBounds) {
			return new Rect(initialBounds.X - highlightedSizeIncrement / 2, initialBounds.Y - highlightedSizeIncrement / 2,
				initialBounds.Width + highlightedSizeIncrement, initialBounds.Height + highlightedSizeIncrement);
		}
		public static Rect CalculateSelectedBounds(Rect initialBounds) {
			return new Rect(initialBounds.X - initialBounds.Width * ( selectedSizeMultiplier - 1 ) / 2, initialBounds.Y - initialBounds.Height * ( selectedSizeMultiplier - 1 ) / 2,
				initialBounds.Width * selectedSizeMultiplier, initialBounds.Height * selectedSizeMultiplier);
		}
		public static Rect CalcAnchorHole(Point anchor, double lineLength) {
			return new Rect(anchor.X - lineLength, anchor.Y - lineLength, lineLength * 2, lineLength * 2);
		}
		public static SeriesPointLayout CreateSeriesPointLayout(PaneMapping mapping, SeriesPointItem pointItem, int markerSize, double pointValue) {
			RefinedPoint pointInfo = pointItem.RefinedPoint;
			int halfMarkerSize = markerSize / 2;
			Point center = mapping.GetRoundedDiagramPoint(pointInfo.Argument, pointValue);
			Rect viewport = new Rect(0, 0, mapping.Viewport.Width, mapping.Viewport.Height);
			return new MarkerSeries2DPointLayout(viewport, new Rect(center.X - halfMarkerSize, center.Y - halfMarkerSize, markerSize, markerSize));
		}
		public static Transform CreatepointTransform(Rect markerBounds, XYDiagram2D diagramXY) {
			Transform pointTransform;
			if (diagramXY.Rotated) {
				TransformGroup transform = new TransformGroup();
				transform.Children.Add(new ScaleTransform() { CenterX = markerBounds.Width / 2, ScaleX = -1 });
				transform.Children.Add(new RotateTransform() { CenterX = markerBounds.Width / 2, CenterY = markerBounds.Height / 2, Angle = -90 });
				pointTransform = transform;
			}
			else
				pointTransform = new MatrixTransform() { Matrix = Matrix.Identity };
			return pointTransform;
		}
		public static void CompletePointLayout(SeriesPointItem pointItem, Diagram diagram, SeriesPointAnimationBase animation, bool isAxisXReversed, bool isAxisYReversed) {
			XYDiagram2D diagramXY = diagram as XYDiagram2D;
			MarkerSeries2DPointLayout layout = pointItem.Layout as MarkerSeries2DPointLayout;
			if (diagramXY == null || layout == null || pointItem.RefinedPoint == null)
				return;
			Rect markerBounds = layout.InitialBounds;
			bool selected = pointItem.IsSelected;
			if (selected && VisualSelectionHelper.SupportsSizeSelection(pointItem.Series.SupportedSelectionType))
				markerBounds = CalculateSelectedBounds(markerBounds);
			if (pointItem.IsHighlighted && !selected)
				markerBounds = CalculateHighlightedBounds(markerBounds);
			Marker2DAnimationBase markerAnimation = animation as Marker2DAnimationBase;
			if (markerAnimation != null) {
				double progress = pointItem.PointProgress.ActualProgress;
				markerBounds = markerAnimation.CreateAnimatedMarkerBounds(markerBounds, layout.Viewport, isAxisXReversed, isAxisYReversed, diagramXY.Rotated, progress);
			}
			Transform pointTransform = CreatepointTransform(markerBounds, diagramXY);
			layout.Complete(markerBounds, pointTransform);
		}
		public static void FillMarkerAnimationKinds(List<AnimationKind> animationKinds) {
			animationKinds.Add(new AnimationKind(typeof(Marker2DWidenAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DSlideFromLeftAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DSlideFromRightAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DSlideFromTopAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DSlideFromBottomAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DSlideFromLeftCenterAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DSlideFromRightCenterAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DSlideFromTopCenterAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DSlideFromBottomCenterAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DSlideFromLeftTopCornerAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DSlideFromRightTopCornerAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DSlideFromRightBottomCornerAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DSlideFromLeftBottomCornerAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Marker2DFadeInAnimation)));
		}
		public static void FillAreaAnimationKinds(List<AnimationKind> animationKinds) {
			animationKinds.Add(new AnimationKind(typeof(Area2DGrowUpAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Area2DStretchFromNearAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Area2DStretchFromFarAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Area2DStretchOutAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Area2DDropFromNearAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Area2DDropFromFarAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Area2DUnwindAnimation)));
		}
		public static SeriesBorder CreateDefaultBorder(int thickness) {
			SeriesBorder border = new SeriesBorder();
			LineStyle lineStyle = new LineStyle();
			border.LineStyle = lineStyle;
			lineStyle.Thickness = thickness;
			lineStyle.LineJoin = PenLineJoin.Round;
			return border;
		}
	}
}
