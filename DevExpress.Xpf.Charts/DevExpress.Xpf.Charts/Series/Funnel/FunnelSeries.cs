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
using System.Windows.Controls;
using System.Windows.Shapes;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum Funnel2DLabelPosition {
		LeftColumn = 0,
		Left = 1,
		Center = 2,
		Right = 3,
		RightColumn = 4,
	}
	[
	TemplatePart(Name = "PART_PointsContainer", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_FunnelPointPath", Type = typeof(Path))
	]
	public class FunnelSeries2D : Series, ISupportSeriesBorder {
		static readonly DependencyPropertyKey TitlesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Titles",
			typeof(TitleCollection), typeof(FunnelSeries2D), new PropertyMetadata());
		public static readonly DependencyProperty TitlesProperty = TitlesPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ModelProperty = DependencyPropertyManager.Register("Model",
			typeof(Funnel2DModel), typeof(FunnelSeries2D), new FrameworkPropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty BorderProperty = DependencyPropertyManager.Register("Border",
			typeof(SeriesBorder), typeof(FunnelSeries2D), new PropertyMetadata(null, BorderChanged));
		static readonly DependencyPropertyKey ActualBorderPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorder",
			typeof(SeriesBorder), typeof(FunnelSeries2D), new PropertyMetadata(null));
		public static readonly DependencyProperty ActualBorderProperty = ActualBorderPropertyKey.DependencyProperty;
		public static readonly DependencyProperty PointDistanceProperty = DependencyPropertyManager.Register("PointDistance",
			typeof(int), typeof(FunnelSeries2D), new PropertyMetadata(0, ChartElementHelper.Update));
		public static readonly DependencyProperty HeightToWidthRatioProperty = DependencyPropertyManager.Register("HeightToWidthRatio",
			typeof(double), typeof(FunnelSeries2D), new PropertyMetadata(1.0, ChartElementHelper.Update));
		public static readonly DependencyProperty HeightToWidthRatioAutoProperty = DependencyPropertyManager.Register("HeightToWidthRatioAuto",
			typeof(bool), typeof(FunnelSeries2D), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty AlignToCenterProperty = DependencyPropertyManager.Register("AlignToCenter",
			typeof(bool), typeof(FunnelSeries2D), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty PointAnimationProperty = DependencyPropertyManager.Register("PointAnimation",
			typeof(Funnel2DSeriesPointAnimationBase), typeof(FunnelSeries2D), new PropertyMetadata(null, PointAnimationPropertyChanged));
		public static readonly DependencyProperty LabelPositionProperty = DependencyPropertyManager.RegisterAttached("LabelPosition",
			typeof(Funnel2DLabelPosition), typeof(FunnelSeries2D), new FrameworkPropertyMetadata(Funnel2DLabelPosition.Right, ChartElementHelper.UpdateWithClearDiagramCache));
		[
			Category(Categories.Layout), 
			XtraSerializableProperty
		]
		public static Funnel2DLabelPosition GetLabelPosition(SeriesLabel label) {
			return (Funnel2DLabelPosition)label.GetValue(LabelPositionProperty);
		}
		public static void SetLabelPosition(SeriesLabel label, Funnel2DLabelPosition position) {
			label.SetValue(LabelPositionProperty, position);
		}
		static void BorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FunnelSeries2D series = d as FunnelSeries2D;
			if (series != null) {
				SeriesBorder newBorder = e.NewValue as SeriesBorder;
				series.SetValue(ActualBorderPropertyKey, newBorder == null ? SeriesWithMarkerHelper.CreateDefaultBorder(DefaultLineThickness) : newBorder);
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as SeriesBorder, e.NewValue as SeriesBorder, series);
			}
		}
		const int DefaultLineThickness = 1;
		Rect viewport;
		ItemsControl pointsContainer;
		protected override string DefaultLegendTextPattern { get { return "{" + PatternUtils.PercentValuePlaceholder + "}"; } }
		protected override string DefaultLabelTextPattern { get { return "{" + PatternUtils.PercentValuePlaceholder + "}"; } }
		protected internal override FadeInMode AutoFadeInMode {
			get { return GetActualSeriesAnimation() != null ? FadeInMode.Labels : FadeInMode.PointsAndLabels; }
		}
		protected override bool ShouldSortPoints { get { return false; } }
		protected internal override VisualSelectionType SupportedSelectionType {
			get { return VisualSelectionType.Hatch; }
		}
		protected internal override bool ArePointsVisible {
			get { return true; }
		}
		protected internal override bool ActualColorEach {
			get { return true; }
		}
		protected internal override Type SupportedDiagramType {
			get { return typeof(SimpleDiagram2D); }
		}
		protected internal override bool LabelsResolveOverlappingSupported {
			get { return true; }
		}
		protected internal override bool IsLabelConnectorItemVisible {
			get { return FunnelSeries2D.GetLabelPosition(ActualLabel) != Funnel2DLabelPosition.Center ? ActualLabel.ConnectorVisible : false; }
		}
		protected override Type PointInterfaceType {
			get { return typeof(IValuePoint); }
		}
		protected override bool Is3DView {
			get { return false; }
		}
		protected override CompatibleViewType CompatibleViewType {
			get { return CompatibleViewType.SimpleView; }
		}
		internal Rect Viewport {
			get { return viewport; }
			set { viewport = value; }
		}
		internal AdditionalLengthCalculator AdditionalLengthCalculator { get; private set; }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FunnelSeries2DTitles"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public TitleCollection Titles {
			get { return (TitleCollection)GetValue(TitlesProperty); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FunnelSeries2DModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Funnel2DModel Model {
			get { return (Funnel2DModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FunnelSeries2DBorder"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public SeriesBorder Border {
			get { return (SeriesBorder)GetValue(BorderProperty); }
			set { SetValue(BorderProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public SeriesBorder ActualBorder {
			get { return (SeriesBorder)GetValue(ActualBorderProperty); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FunnelSeries2DPointDistance"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int PointDistance {
			get { return (int)GetValue(PointDistanceProperty); }
			set { SetValue(PointDistanceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FunnelSeries2DHeightToWidthRatio"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double HeightToWidthRatio {
			get { return (double)GetValue(HeightToWidthRatioProperty); }
			set { SetValue(HeightToWidthRatioProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FunnelSeries2DHeightToWidthRatioAuto"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public bool HeightToWidthRatioAuto {
			get { return (bool)GetValue(HeightToWidthRatioAutoProperty); }
			set { SetValue(HeightToWidthRatioAutoProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FunnelSeries2DAlignToCenter"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public bool AlignToCenter {
			get { return (bool)GetValue(AlignToCenterProperty); }
			set { SetValue(AlignToCenterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FunnelSeries2DPointAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Funnel2DSeriesPointAnimationBase PointAnimation {
			get { return (Funnel2DSeriesPointAnimationBase)GetValue(PointAnimationProperty); }
			set { SetValue(PointAnimationProperty, value); }
		}
		public FunnelSeries2D() {
			DefaultStyleKey = typeof(FunnelSeries2D);
			this.SetValue(ActualBorderPropertyKey, SeriesWithMarkerHelper.CreateDefaultBorder(DefaultLineThickness));
			this.SetValue(TitlesPropertyKey, ChartElementHelper.CreateInstance<TitleCollection>(this));
			AdditionalLengthCalculator = new AdditionalLengthCalculator();
		}
		protected override PatternDataProvider GetDataProvider(PatternConstants patternConstant) {
			switch (patternConstant) {
				case PatternConstants.Argument:
				case PatternConstants.Value:
				case PatternConstants.PercentValue:
				case PatternConstants.PointHint:
					return new FunnelPointPatternDataProvider(patternConstant);
				case PatternConstants.Series:
				case PatternConstants.SeriesGroup:
					return new SeriesPatternDataProvider(patternConstant);
			}
			return null;
		}
		protected override Series CreateObjectForClone() {
			return new FunnelSeries2D();
		}
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
			animationKinds.Add(new AnimationKind(typeof(Funnel2DWidenAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Funnel2DGrowUpAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Funnel2DSlideFromLeftAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Funnel2DSlideFromRightAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Funnel2DSlideFromTopAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Funnel2DSlideFromBottomAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Funnel2DFadeInAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Funnel2DFlyInAnimation)));
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
		protected override bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager) && sender is SeriesBorder) {
				ChartElementHelper.Update(this);
				success = true;
			}
			return success || base.PerformWeakEvent(managerType, sender, e);
		}
		protected override SeriesContainer CreateContainer() {
			return new SimpleSeriesContainer(this);
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value;
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			FunnelSeries2D funnelSeries2D = series as FunnelSeries2D;
			if (funnelSeries2D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, funnelSeries2D, PointDistanceProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, funnelSeries2D, HeightToWidthRatioProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, funnelSeries2D, HeightToWidthRatioAutoProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, funnelSeries2D, AlignToCenterProperty);
				if (CopyPropertyValueHelper.IsValueSet(funnelSeries2D, ModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, funnelSeries2D, ModelProperty))
						Model = funnelSeries2D.Model.CloneModel();
				CopyPropertyValueHelper.CopyPropertyValue(this, funnelSeries2D, BorderProperty);
				if (Label != null && funnelSeries2D.Label != null)
					FunnelSeries2D.SetLabelPosition(Label, FunnelSeries2D.GetLabelPosition(funnelSeries2D.Label));
				if (CopyPropertyValueHelper.IsValueSet(funnelSeries2D, PointAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, funnelSeries2D, PointAnimationProperty))
						PointAnimation = funnelSeries2D.PointAnimation.CloneAnimation() as Funnel2DSeriesPointAnimationBase;
				if (funnelSeries2D.Titles != null && funnelSeries2D.Titles.Count > 0) {
					Titles.Clear();
					foreach (Title title in funnelSeries2D.Titles) {
						Title newTitle = new Title();
						newTitle.Assign(title);
						Titles.Add(newTitle);
					}
				}
			}
		}
		protected internal Point CalculateToolTipPoint(SeriesPointItem pointItem) {
			Point location = new Point();
			FunnelSeries2DPointLayout pointLayout = pointItem.Layout as FunnelSeries2DPointLayout;
			if (pointLayout != null) {
				Rect funnelRect = LayoutHelper.GetRelativeElementRect(pointItem.PointItemPresentation, pointItem.Series.Diagram.ChartControl);
				location = new Point(
					funnelRect.Left + funnelRect.X + funnelRect.Width / 2,
					funnelRect.Top + funnelRect.Y);
			}
			return location;
		}
		protected internal override void CompletePointLayout(SeriesPointItem pointItem) {
			FunnelSeries2DPointLayout layout = pointItem.Layout as FunnelSeries2DPointLayout;
			if (layout == null || pointItem.RefinedPoint == null)
				return;
			Funnel2DSeriesPointAnimationBase animation = GetActualPointAnimation() as Funnel2DSeriesPointAnimationBase;
			if (animation != null) {
				double progress = pointItem.PointProgress.ActualProgress;
				Rect result = animation.CalculateAnimatedBounds(layout.GeometryBounds, viewport, progress);
				layout.Complete(result);
			}
			else
				layout.Complete(layout.GeometryBounds);
		}
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			return Model;
		}
		protected internal override SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return new Funnel2DWidenAnimation();
		}
		internal List<FunnelSeries2DPointLayout> GetSeriesPointsLayout(Size availableSize) {
			IList<SeriesPointItem> points = Item.AllPointItems;
			if (Diagram == null) {
				ChartDebug.Fail("Can't create series points layout because the Diagram is null.");
				return null;
			}
			IRefinedSeries refinedSeries = Diagram.ViewController.GetRefinedSeries(this);
			if (refinedSeries == null || refinedSeries.Points.Count == 0 || points.Count == 0)
				return null;
			Size correctedBounds = CorrectBoundsBySizeAndRatio(availableSize);
			if (correctedBounds.Height <= 0 || correctedBounds.Width <= 0)
				return null;
			var listRefinedPoints = new List<RefinedPoint>();
			foreach (var point in points)
				listRefinedPoints.Add(point.RefinedPoint);
			Funnel2DLayoutCalculator layoutCalculator = new Funnel2DLayoutCalculator(new GRealSize2D(correctedBounds.Width, correctedBounds.Height), listRefinedPoints, PointDistance);
			List<FunnelPointInfo> pointInfos = layoutCalculator.Calculate();
			if (pointInfos != null)
			{
				List<FunnelSeries2DPointLayout> layouts = new List<FunnelSeries2DPointLayout>();
				foreach (var item in pointInfos) 
					layouts.Add(new FunnelSeries2DPointLayout(viewport, new Rect(0,0,correctedBounds.Width, correctedBounds.Height), item));
				return layouts;
			}
			return null;
		}
		internal FunnelSeriesLabel2DLayout CreateSeriesLabelLayout(Rect pointsBounds, Rect labelBounds, SeriesLabelItem labelItem, double maxLabelWidth) {
			FunnelSeries2DPointLayout layout = labelItem.PointItem.Layout as FunnelSeries2DPointLayout;
			if (labelItem.PointItem.Layout == null)
				return null;
			double labelWidth = labelItem.LabelSize.Width;
			double labelHeight = labelItem.LabelSize.Height;
			Point anhorPoint, finishPoint, labelCenterPoint;
			double labelCenterPointY = pointsBounds.Y + layout.InitTopLeftPoint.Y + 1;
			double leftTopPointX = layout.InitTopLeftPoint.X + pointsBounds.X;
			double rightTopPointX = layout.InitTopRightPoint.X + pointsBounds.X;
			FunnelSeries2D funnel = labelItem.Label.Series as FunnelSeries2D;
			int additionalLength = funnel.AdditionalLengthCalculator.GetLength(labelItem.RefinedPoint);
			int connectorLength = labelItem.Label.Indent + additionalLength;
			Funnel2DLabelPosition labelPosition = FunnelSeries2D.GetLabelPosition(ActualLabel);
			switch (labelPosition) {
				case Funnel2DLabelPosition.Center:
					anhorPoint = finishPoint = new Point((leftTopPointX + rightTopPointX) / 2,
						(2 * pointsBounds.Y + layout.InitTopRightPoint.Y + layout.InitBottomRightPoint.Y) / 2);
					return new FunnelSeriesLabel2DLayout(labelItem, anhorPoint, anhorPoint);
				case Funnel2DLabelPosition.RightColumn:
					anhorPoint = new Point(rightTopPointX - 1, labelCenterPointY);
					labelCenterPoint = new Point(labelBounds.Right - labelWidth / 2, labelCenterPointY);
					return new FunnelSeriesLabel2DLayout(labelItem, anhorPoint, labelCenterPoint);
				case Funnel2DLabelPosition.LeftColumn:
					anhorPoint = new Point(leftTopPointX + 1, labelCenterPointY);
					labelCenterPoint = new Point(labelBounds.Left + labelWidth / 2, labelCenterPointY);
					return new FunnelSeriesLabel2DLayout(labelItem, anhorPoint, labelCenterPoint);
				case Funnel2DLabelPosition.Left:
					anhorPoint = new Point(leftTopPointX + 1, labelCenterPointY);
					labelCenterPoint = new Point(leftTopPointX - labelWidth / 2 - connectorLength, labelCenterPointY);
					return new FunnelSeriesLabel2DLayout(labelItem, anhorPoint, labelCenterPoint);
				case Funnel2DLabelPosition.Right:
					anhorPoint = new Point(rightTopPointX - 1, labelCenterPointY);
					labelCenterPoint = new Point(rightTopPointX + labelWidth / 2 + connectorLength, labelCenterPointY);
					return new FunnelSeriesLabel2DLayout(labelItem, anhorPoint, labelCenterPoint);
				default:
					ChartDebug.Fail("Unknown funnel label position");
					anhorPoint = finishPoint = new Point();
					return new FunnelSeriesLabel2DLayout(labelItem, finishPoint, finishPoint);
			}
		}
		internal void CreateSeriesPointsLayout(Size availableSize) {
			List<FunnelSeries2DPointLayout> layouts = GetSeriesPointsLayout(availableSize);
			if (layouts != null && layouts.Count == Item.AllPointItems.Count)
				for (int i = 0; i < Item.AllPointItems.Count; i++)
					Item.AllPointItems[i].Layout = layouts[i];
		}
		internal Size CorrectBoundsBySizeAndRatio(Size bounds) {
			if (HeightToWidthRatioAuto)
				return bounds;
			double newHeight = bounds.Width * HeightToWidthRatio;
			double newWidth = bounds.Width;
			if (newHeight > bounds.Height) {
				newHeight = bounds.Height;
				newWidth = newHeight / HeightToWidthRatio;
			}
			return new Size(newWidth, newHeight);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			pointsContainer = (ItemsControl)GetTemplateChild("PART_PointsContainer");
		}
		public override SeriesPointAnimationBase GetPointAnimation() { return PointAnimation; }
		public override void SetPointAnimation(SeriesPointAnimationBase value) {
			if (value != null && !(value is Funnel2DSeriesPointAnimationBase))
				return;
			PointAnimation = value as Funnel2DSeriesPointAnimationBase;
		}
	}
}
