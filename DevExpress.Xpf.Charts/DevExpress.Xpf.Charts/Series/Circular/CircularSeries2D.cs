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
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class CircularSeries2D : XYSeries, IXYSeriesView {
		public static readonly DependencyProperty MarkerModelProperty = DependencyPropertyManager.Register("MarkerModel",
		   typeof(Marker2DModel), typeof(CircularSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty MarkerSizeProperty = DependencyPropertyManager.Register("MarkerSize",
			typeof(int), typeof(CircularSeries2D), new PropertyMetadata(10, ChartElementHelper.Update), MarkerSeries2D.MarkerSizeValidation);
		public static readonly DependencyProperty PointAnimationProperty = DependencyPropertyManager.Register("PointAnimation",
			typeof(CircularMarkerAnimationBase), typeof(CircularSeries2D), new PropertyMetadata(null, PointAnimationPropertyChanged));
		public static readonly DependencyProperty AngleProperty = DependencyPropertyManager.RegisterAttached("Angle",
		   typeof(double), typeof(CircularSeries2D), new PropertyMetadata(45.0, ChartElementHelper.Update));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CircularSeries2DMarkerSize"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int MarkerSize {
			get { return (int)GetValue(MarkerSizeProperty); }
			set { SetValue(MarkerSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CircularSeries2DMarkerModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker2DModel MarkerModel {
			get { return (Marker2DModel)GetValue(MarkerModelProperty); }
			set { SetValue(MarkerModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CircularSeries2DPointAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public CircularMarkerAnimationBase PointAnimation {
			get { return (CircularMarkerAnimationBase)GetValue(PointAnimationProperty); }
			set { SetValue(PointAnimationProperty, value); }
		}
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
		protected internal override bool IsLabelConnectorItemVisible { get { return ActualLabel.ConnectorVisible; } }
		protected internal override bool LabelsResolveOverlappingSupported { get { return true; } }
		protected internal override VisualSelectionType SupportedSelectionType { get { return VisualSelectionType.Size; } }
		protected override bool Is3DView { get { return false; } }
		CircularDiagram2D CircularDiagram { get { return Diagram as CircularDiagram2D; } }
		#region IXYSeriesView
		IAxisData IXYSeriesView.AxisXData { get { return CircularDiagram != null && SupportedDiagramType == CircularDiagram.GetType() ? CircularDiagram.AxisXImpl : null; } }
		IAxisData IXYSeriesView.AxisYData { get { return CircularDiagram != null && SupportedDiagramType == CircularDiagram.GetType() ? CircularDiagram.AxisYImpl : null; } }
		ToolTipPointDataToStringConverter IXYSeriesView.CrosshairConverter { get { return null; } }
		bool IXYSeriesView.CrosshairEnabled { get { return false; } }
		string IXYSeriesView.CrosshairLabelPattern { get { return string.Empty; } }
		IEnumerable<double> IXYSeriesView.GetCrosshairValues(RefinedPoint refinedPoint) {
			return null;
		}
		IPane IXYSeriesView.Pane { get { return ActualPane; } }
		int IXYSeriesView.PixelsPerArgument {
			get { return 40; }
		}
		bool IXYSeriesView.SideMarginsEnabled { get { return true; } }
		List<ISeparatePaneIndicator> IXYSeriesView.GetSeparatePaneIndicators() {
			return new List<ISeparatePaneIndicator>();
		}
		List<IAffectsAxisRange> IXYSeriesView.GetIndicatorsAffectRange() {
			return new List<IAffectsAxisRange>();
		}
		#endregion
		SeriesPointLayout CreateSeriesPointLayout(CircularDiagramMapping mapping, SeriesPointItem pointItem) {
			RefinedPoint refinedPoint = pointItem.RefinedPoint;
			int markerSize = MarkerSize;
			int halfMarkerSize = markerSize / 2;
			Point center = mapping.GetDiagramPoint(refinedPoint.Argument, ((IXYPoint)refinedPoint).Value);
			Rect viewport = new Rect(0, 0, mapping.Viewport.Width, mapping.Viewport.Height);
			return new MarkerSeries2DPointLayout(viewport, new Rect(center.X - halfMarkerSize, center.Y - halfMarkerSize, markerSize, markerSize));
		}
		XYSeriesLabel2DLayout CreateSeriesLabelLayout(SeriesLabelItem labelItem, CircularDiagramMapping mapping, Transform transform) {
			Point anchorPoint = transform.Transform(mapping.GetDiagramPoint(labelItem.RefinedPoint.Argument, ((IXYPoint)labelItem.RefinedPoint).Value));
			SeriesLabel label = labelItem.Label;
			int indent = label.Indent;
			return new XYSeriesLabel2DLayout(labelItem, mapping, anchorPoint, indent,
				MathUtils.Degree2Radian(MathUtils.NormalizeDegree(-(GetAngle(label)))), GraphicsUtils.ConvertRect(SeriesWithMarkerHelper.CalcAnchorHole(anchorPoint, indent)));
		}
		void UpdateAdditionalGeometryClip() {
			AdditionalLineSeriesGeometry additionalGeometry = AdditionalGeometry;
			if (CircularDiagram == null || additionalGeometry == null)
				return;
			Rect viewport = CircularDiagram.ActualViewport;
			Rect clipBounds = new Rect(0, 0, viewport.Width, viewport.Height);
			IUnwindAnimation unwindAnimation = GetActualSeriesAnimation() as IUnwindAnimation;
			if (unwindAnimation != null)
				clipBounds = unwindAnimation.CreateAnimatedClipBounds(clipBounds, SeriesProgress.ActualProgress);
			additionalGeometry.Clip = new RectangleGeometry() { Rect = clipBounds };
		}
		internal void CreateSeriesPointsLayout() {
			CircularDiagramMapping mapping = new CircularDiagramMapping(CircularDiagram);
			foreach (SeriesPointItem pointItem in Item.AllPointItems)
				pointItem.Layout = CreateSeriesPointLayout(mapping, pointItem);
			UpdateAdditionalGeometry();
			UpdateAdditionalGeometryClip();
		}
		internal void CreateSeriesLabelsLayout(Rect viewport) {
			CircularDiagramMapping mapping = new CircularDiagramMapping(CircularDiagram, viewport);
			Transform labelTransform = CircularDiagram.ViewportRenderTransform;
			foreach (SeriesLabelItem labelItem in ActualLabel.Items)
				labelItem.Layout = CreateSeriesLabelLayout(labelItem, mapping, labelTransform);
		}
		internal void CreateSeriesLabelConnectorsLayout() {
			if (ActualLabel.Items != null) {
				foreach (SeriesLabelItem labelItem in ActualLabel.Items)
					labelItem.UpdateConnectorItemLayout();
			}
		}
		protected override void SeriesProgressChanged() {
			base.SeriesProgressChanged();
			SeriesAnimationBase animation = GetActualSeriesAnimation();
			if (animation == null || animation.ShouldAnimateAdditionalGeometry)
				UpdateAdditionalGeometry();
			if (animation == null || animation.ShouldAnimateClipBounds)
				UpdateAdditionalGeometryClip();
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value;
		}
		protected internal virtual AdditionalLineSeriesGeometry CreateAdditionalGeometry() {
			return null;
		}
		protected internal override void CompletePointLayout(SeriesPointItem pointItem) {
			MarkerSeries2DPointLayout layout = pointItem.Layout as MarkerSeries2DPointLayout;
			if (CircularDiagram == null || layout == null || pointItem.RefinedPoint == null)
				return;
			Rect markerBounds = layout.InitialBounds;
			if (pointItem.IsSelected && VisualSelectionHelper.SupportsSizeSelection(SupportedSelectionType))
				markerBounds = SeriesWithMarkerHelper.CalculateSelectedBounds(markerBounds);
			CircularMarkerAnimationBase markerAnimation = GetActualPointAnimation() as CircularMarkerAnimationBase;
			if (markerAnimation != null) {
				double progress = pointItem.PointProgress.ActualProgress;
				markerBounds = markerAnimation.CreateAnimatedMarkerBounds(markerBounds, layout.Viewport, progress);
			}
			Transform pointTransform = new ScaleTransform() { CenterY = markerBounds.Height / 2, ScaleY = -1 };
			layout.Complete(markerBounds, pointTransform);
		}
		protected internal override IMapping CreateDiagramMapping() {
			return CircularDiagram != null ? new CircularDiagramMapping(CircularDiagram) : null;
		}
		protected internal override Point CalculateToolTipPoint(SeriesPointItem pointItem, bool inLabel) {
			Transform transform = CircularDiagram.ViewportRenderTransform;
			CircularDiagramMapping mapping = new CircularDiagramMapping(CircularDiagram);
			Point toolTipPoint = transform.Transform(mapping.GetDiagramPoint(pointItem.RefinedPoint.Argument, ((IValuePoint)pointItem.RefinedPoint).Value));
			Rect diagramRect = LayoutHelper.GetRelativeElementRect(CircularDiagram, Diagram.ChartControl);
			return new Point(toolTipPoint.X + diagramRect.Left, toolTipPoint.Y + diagramRect.Top);
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			CircularSeries2D circularSeries2D = series as CircularSeries2D;
			if (circularSeries2D != null) {
				if (Label != null && circularSeries2D.Label != null)
					CircularSeries2D.SetAngle(Label, CircularSeries2D.GetAngle(circularSeries2D.Label));
				CopyPropertyValueHelper.CopyPropertyValue(this, circularSeries2D, MarkerSizeProperty);
				if (CopyPropertyValueHelper.IsValueSet(circularSeries2D, MarkerModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, circularSeries2D, MarkerModelProperty))
						MarkerModel = circularSeries2D.MarkerModel.CloneModel();
				if (CopyPropertyValueHelper.IsValueSet(circularSeries2D, PointAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, circularSeries2D, PointAnimationProperty))
						PointAnimation = circularSeries2D.PointAnimation.CloneAnimation() as CircularMarkerAnimationBase;
			}
		}
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
			animationKinds.Add(new AnimationKind(typeof(CircularMarkerWidenAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularMarkerFadeInAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularMarkerSlideFromLeftCenterAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularMarkerSlideFromRightCenterAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularMarkerSlideFromTopCenterAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularMarkerSlideFromBottomCenterAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularMarkerSlideFromCenterAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularMarkerSlideToCenterAnimation)));
		}
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			return MarkerModel;
		}
		protected internal override SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return new CircularMarkerSlideFromCenterAnimation();
		}
		protected override PatternDataProvider GetDataProvider(PatternConstants patternConstant) {
			return GetXYDataProvider(patternConstant);
		}
		public override SeriesPointAnimationBase GetPointAnimation() { return PointAnimation; }
		public override void SetPointAnimation(SeriesPointAnimationBase value) {
			if (value != null && !(value is CircularMarkerAnimationBase))
				return;
			PointAnimation = (CircularMarkerAnimationBase)value;
		}
	}
}
