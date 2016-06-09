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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.ModelSupport;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(PieSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PieSeriesView : PieSeriesViewBase, ISimpleDiagram2DSeriesView {
		const int DefaultRotation = 0;
		const int MinWidthAtWhichPieStillRendered = 2;
		const int MinHeightAtWhichPieStillRendered = 2;
		const double DefaultHeightToWidthRatio = 1;
		const double DefaultMinAllowedSizeFraction = 0.5;
		const bool DefaultRuntimeExploding = false;
		double heightToWidthRatio = DefaultHeightToWidthRatio;
		int angleDegree;
		CustomBorder border;
		PolygonFillStyle fillStyle;
		bool runtimeExploding = DefaultRuntimeExploding;
		double minAllowedSizeFraction = DefaultMinAllowedSizeFraction;
		PieSeriesViewAppearance Appearance {
			get {
				IChartAppearance actualAppearance = CommonUtils.GetActualAppearance(this);
				return actualAppearance.PieSeriesViewAppearance;
			}
		}
		new PieSeriesLabel Label { get { return (PieSeriesLabel)base.Label; } }
		protected Color BorderColorFromAppearance {
			get {
				IChartAppearance actualAppearance = CommonUtils.GetActualAppearance(this);
				if (actualAppearance != null)
					return actualAppearance.PieSeriesViewAppearance.BorderColor;
				else
					return Color.Empty;
			}
		}
		protected override double ActualHeightToWidthRatio { get { return heightToWidthRatio; } }
		protected override SimpleDiagramSeriesLayoutBase CreateSimpleDiagramSeriesLayout() {
			SimpleDiagramSeriesLayout layout;
			using (TextMeasurer textMeasurer = new TextMeasurer()) {
				RefinedSeriesData seriesData = new RefinedSeriesData(Chart.ViewController.FindRefinedSeries(Series), textMeasurer, null);
				Rectangle displayBounds = Chart.ContainerAdapter.DisplayBounds;
				layout = new SimpleDiagramSeriesLayout(seriesData, textMeasurer, displayBounds, displayBounds);
			}
			return layout;
		}
		protected internal override int ActualRotation { get { return Rotation; } }
		protected internal override int ActualBorderThickness { get { return Border.ActualThickness; } }
		protected internal override bool ActualRuntimeExploding { get { return RuntimeExploding; } }
		protected internal override bool IsSupportedToolTips { get { return true; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnPie); } }
		protected internal override bool HitTestingSupportedForLegendMarker { get { return true; } }
		protected override bool Is3DView { get { return false; } }
		protected override Type PointInterfaceType {
			get {
				return typeof(IValuePoint);
			}
		}
		internal PolygonFillStyle ActualFillStyle {
			get {
				if (fillStyle.FillMode != FillMode.Empty)
					return fillStyle;
				else
					return Appearance.FillStyle;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesViewMinAllowedSizePercentage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesView.MinAllowedSizePercentage"),
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public double MinAllowedSizePercentage {
			get { return minAllowedSizeFraction * 100.0; }
			set {
				if (!(0.0 <= value && value <= 100.0))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPercentValue));
				SendNotification(new ElementWillChangeNotification(this));
				minAllowedSizeFraction = value / 100.0;
				RaiseControlChanged();
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Type DiagramType { get { return typeof(SimpleDiagram); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesViewColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesView.Color"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Color Color { get { return Color.Empty; } set { } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesViewRotation"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesView.Rotation"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Rotation {
			get { return this.angleDegree; }
			set {
				if (angleDegree == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				this.angleDegree = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesViewHeightToWidthRatio"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesView.HeightToWidthRatio"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public double HeightToWidthRatio {
			get { return this.heightToWidthRatio; }
			set {
				if (value < 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgValueMustBeGreaterThenZero));
				if (heightToWidthRatio == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				heightToWidthRatio = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesViewBorder"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesView.Border"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public CustomBorder Border { get { return border; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesViewFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesView.FillStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public PolygonFillStyle FillStyle { get { return this.fillStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesViewRuntimeExploding"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesView.RuntimeExploding"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool RuntimeExploding {
			get { return runtimeExploding; }
			set {
				if (value != runtimeExploding) {
					SendNotification(new ElementWillChangeNotification(this));
					runtimeExploding = value;
					RaiseControlChanged();
				}
			}
		}
		public PieSeriesView()
			: base() {
			Initialize();
		}
		public PieSeriesView(int[] explodedPointIds)
			: base(explodedPointIds) {
			Initialize();
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "MinAllowedSizePercentage")
				return ShouldSerializeMinAllowedSizePercentage();
			if (propertyName == "Rotation")
				return ShouldSerializeRotation();
			if (propertyName == "HeightToWidthRatio")
				return ShouldSerializeHeightToWidthRatio();
			if (propertyName == "Border")
				return ShouldSerializeBorder();
			if (propertyName == "FillStyle")
				return ShouldSerializeFillStyle();
			if (propertyName == "RuntimeExploding")
				return ShouldSerializeRuntimeExploding();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeMinAllowedSizePercentage() {
			return this.minAllowedSizeFraction != DefaultMinAllowedSizeFraction;
		}
		void ResetMinAllowedSizePercentage() {
			MinAllowedSizePercentage = DefaultMinAllowedSizeFraction * 100;
		}
		bool ShouldSerializeRotation() {
			return Rotation != DefaultRotation;
		}
		void ResetRotation() {
		   Rotation = DefaultRotation;
		}
		bool ShouldSerializeHeightToWidthRatio() {
			return HeightToWidthRatio != DefaultHeightToWidthRatio;
		}
		void ResetHeightToWidthRatio() {
			HeightToWidthRatio = DefaultHeightToWidthRatio;
		}
		bool ShouldSerializeBorder() {
			return Border.ShouldSerialize();
		}
		bool ShouldSerializeFillStyle() {
			return FillStyle.ShouldSerialize();
		}
		bool ShouldSerializeRuntimeExploding() {
			return runtimeExploding != DefaultRuntimeExploding;
		}
		void ResetRuntimeExploding() {
			RuntimeExploding = DefaultRuntimeExploding;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeRotation() ||
				ShouldSerializeHeightToWidthRatio() ||
				ShouldSerializeBorder() ||
				ShouldSerializeFillStyle() ||
				ShouldSerializeRuntimeExploding();
		}
		#endregion
		void Initialize() {
			this.border = new CustomBorder(this, true, Color.Empty);
			this.fillStyle = new PolygonFillStyle(this);
		}
		DiagramPoint? CalculateAchorPoint(SeriesPointLayout pointLayout) {
			PieSeriesPointLayout pieLayout = pointLayout as PieSeriesPointLayout;
			if (pieLayout == null)
				return null;
			RefinedPointData pointData = pieLayout.PointData;
			IPiePoint piePoint = pointData.RefinedPoint;
			if(piePoint == null || piePoint.NormalizedValue == 0)
				return null;
			RectangleF pieBounds = pieLayout.PieBounds;
			if (!pieBounds.AreWidthAndHeightPositive())
				return null;
			Ellipse actualEllipse = new Ellipse(pieLayout.Pie.CalculateCenter(pieLayout.BasePoint), pieBounds.Width / 2, pieBounds.Height / 2);
			return (DiagramPoint)actualEllipse.CalcEllipsePoint(pieLayout.Pie.HalfAngle);
		}
		Rectangle CalculateMaxPossiblePieBounds(Rectangle seriesAreaExcludingTitles, out DirectionOfContactPieWithBounds direction) {
			Rectangle pieBounds = new Rectangle();
			int possiblePieHeight = MathUtils.StrongRound(seriesAreaExcludingTitles.Width * this.heightToWidthRatio);
			if (possiblePieHeight < seriesAreaExcludingTitles.Height) {
				pieBounds.Width = seriesAreaExcludingTitles.Width;
				pieBounds.Height = possiblePieHeight;
				direction = DirectionOfContactPieWithBounds.Horizontal;
			}
			else {
				int possiblePieWidth = MathUtils.StrongRound(seriesAreaExcludingTitles.Height / this.heightToWidthRatio);
				pieBounds.Width = possiblePieWidth;
				pieBounds.Height = seriesAreaExcludingTitles.Height;
				direction = DirectionOfContactPieWithBounds.Vertical;
			}
			return pieBounds.GetTheSameRectPlacedInCenterOf(seriesAreaExcludingTitles);
		}
		Size CalculateMaxLabelOrAnnotationInLableModeSize(RefinedSeriesData seriesData, Rectangle pieBounds) {
			Size maxLabelSize = Label.CalculateMaximumSize(seriesData, pieBounds.Size);
			Size maxAnnotationSize = AnnotationHelper.CalculateAnnotationSize(seriesData);
			Size maxlabelOrAnnotationSize = new Size(Math.Max(maxLabelSize.Width, maxAnnotationSize.Width), Math.Max(maxLabelSize.Height, maxAnnotationSize.Height));
			return maxlabelOrAnnotationSize;
		}
		Rectangle GetCorrectedByLabelsInHorizontalDirectionPieBounds(Rectangle maxPossiblePieBounds, Rectangle seriesAreaExcludingTitles, Size maxlabelOrAnnotationSize) {
			Rectangle correctingMaxPossiblePieBounds = maxPossiblePieBounds;
			int minAllowedPieWidth = MathUtils.StrongRound(seriesAreaExcludingTitles.Width * this.minAllowedSizeFraction);
			minAllowedPieWidth = minAllowedPieWidth < MinWidthAtWhichPieStillRendered ? MinWidthAtWhichPieStillRendered : minAllowedPieWidth;
			int minAllowedPieHeight = GetHeightByWidthUsingHeightToWidthRatio(minAllowedPieWidth);
			int maxHorizontalCorrectionAmount = seriesAreaExcludingTitles.Width - MathUtils.StrongRound(minAllowedPieWidth);
			int desiredHorizontalCorrectionAmount = 2 * maxlabelOrAnnotationSize.Width;
			if (desiredHorizontalCorrectionAmount > maxHorizontalCorrectionAmount) {
				int actualHorizontalCorrectionAmount = maxHorizontalCorrectionAmount;
				correctingMaxPossiblePieBounds.Width -= actualHorizontalCorrectionAmount;
				correctingMaxPossiblePieBounds.Height = GetHeightByWidthUsingHeightToWidthRatio(correctingMaxPossiblePieBounds.Width);
			}
			else {
				int actualHorizontalCorrectionAmount = desiredHorizontalCorrectionAmount;
				correctingMaxPossiblePieBounds.Width -= actualHorizontalCorrectionAmount;
				correctingMaxPossiblePieBounds.Height = GetHeightByWidthUsingHeightToWidthRatio(correctingMaxPossiblePieBounds.Width);
				int maxVerticalCorrectionAmount = correctingMaxPossiblePieBounds.Height - MathUtils.StrongRound(minAllowedPieHeight);
				int desiredVerticalCorrectionAmount = 2 * maxlabelOrAnnotationSize.Height - (seriesAreaExcludingTitles.Height - correctingMaxPossiblePieBounds.Height);
				int actualVerticalCorrectionAmount;
				if (desiredVerticalCorrectionAmount > maxVerticalCorrectionAmount)
					actualVerticalCorrectionAmount = maxVerticalCorrectionAmount;
				else
					if (desiredVerticalCorrectionAmount > 0)
						actualVerticalCorrectionAmount = desiredVerticalCorrectionAmount;
					else
						actualVerticalCorrectionAmount = 0;
				correctingMaxPossiblePieBounds.Height -= actualVerticalCorrectionAmount;
				correctingMaxPossiblePieBounds.Width = GetWidthByHeightUsingHeightToWidthRatio(correctingMaxPossiblePieBounds.Height);
				if (correctingMaxPossiblePieBounds.Width > seriesAreaExcludingTitles.Width)
					correctingMaxPossiblePieBounds.Width = seriesAreaExcludingTitles.Width;
			}
			return correctingMaxPossiblePieBounds.GetTheSameRectPlacedInCenterOf(seriesAreaExcludingTitles);
		}
		Rectangle GetCorrectedByLabelsInVerticalDirectionPieBounds(Rectangle maxPossiblePieBounds, Rectangle seriesAreaExcludingTitles, Size maxlabelOrAnnotationSize) {
			Rectangle correctingMaxPossiblePieBounds = maxPossiblePieBounds;
			int minAllowedPieHeight = MathUtils.StrongRound(seriesAreaExcludingTitles.Height * this.minAllowedSizeFraction);
			minAllowedPieHeight = minAllowedPieHeight < MinHeightAtWhichPieStillRendered ? MinHeightAtWhichPieStillRendered : minAllowedPieHeight;
			int minAllowedPieWidth = GetWidthByHeightUsingHeightToWidthRatio(minAllowedPieHeight);
			int maxVerticalCorrectionAmount = seriesAreaExcludingTitles.Height - minAllowedPieHeight;
			int desiredVerticalCorrectionAmount = 2 * maxlabelOrAnnotationSize.Height;
			if (desiredVerticalCorrectionAmount > maxVerticalCorrectionAmount) {
				int actualVerticalCorrectionAmount = maxVerticalCorrectionAmount;
				correctingMaxPossiblePieBounds.Height -= actualVerticalCorrectionAmount;
				correctingMaxPossiblePieBounds.Width = GetWidthByHeightUsingHeightToWidthRatio(correctingMaxPossiblePieBounds.Height);
			}
			else {
				int actualVerticalCorrectionAmount = desiredVerticalCorrectionAmount;
				correctingMaxPossiblePieBounds.Height -= actualVerticalCorrectionAmount;
				correctingMaxPossiblePieBounds.Width = GetWidthByHeightUsingHeightToWidthRatio(correctingMaxPossiblePieBounds.Height);
				int maxHorizontalCorrectionAmount = correctingMaxPossiblePieBounds.Width - minAllowedPieWidth;
				int desiredHorisontalCorrectionAmount = 2 * maxlabelOrAnnotationSize.Width - (seriesAreaExcludingTitles.Width - correctingMaxPossiblePieBounds.Width);
				int actualHorizontalCorrectionAmount;
				if (desiredHorisontalCorrectionAmount > maxHorizontalCorrectionAmount)
					actualHorizontalCorrectionAmount = maxHorizontalCorrectionAmount;
				else
					if (desiredHorisontalCorrectionAmount > 0)
						actualHorizontalCorrectionAmount = desiredHorisontalCorrectionAmount;
					else
						actualHorizontalCorrectionAmount = 0;
				correctingMaxPossiblePieBounds.Width -= actualHorizontalCorrectionAmount;
				correctingMaxPossiblePieBounds.Height = GetHeightByWidthUsingHeightToWidthRatio(correctingMaxPossiblePieBounds.Width);
				if (correctingMaxPossiblePieBounds.Height > seriesAreaExcludingTitles.Height)
					correctingMaxPossiblePieBounds.Height = seriesAreaExcludingTitles.Height;
			}
			return correctingMaxPossiblePieBounds.GetTheSameRectPlacedInCenterOf(seriesAreaExcludingTitles);
		}
		int GetWidthByHeightUsingHeightToWidthRatio(int height) {
			int width = MathUtils.StrongRound(height / this.heightToWidthRatio);
			if (width < MinWidthAtWhichPieStillRendered)
				width = MinWidthAtWhichPieStillRendered;
			return width;
		}
		int GetHeightByWidthUsingHeightToWidthRatio(int width) {
			int height = MathUtils.StrongRound(width * this.heightToWidthRatio);
			if (height < MinHeightAtWhichPieStillRendered)
				height = MinWidthAtWhichPieStillRendered;
			return height;
		}
		protected override DiagramPoint? CalculateAnnotationAchorPoint(ISimpleDiagramDomain domain, SeriesPointLayout pointLayout) {
			return CalculateAchorPoint(pointLayout);
		}
		protected override ChartElement CreateObjectForClone() {
			return new PieSeriesView();
		}
		protected override GraphicsCommand CreateGraphicsCommand(Pie pie, PointF basePoint, bool negativeValuesPresents, SelectionState selectionState, int borderThickness, SimpleDiagramDrawOptionsBase drawOptions) {
			PieDrawOptions pieDrawOptions = drawOptions as PieDrawOptions;
			if (pieDrawOptions == null || !pie.ShouldCreateGraphicsCommand())
				return null;
			Color color = drawOptions.Color;
			if (negativeValuesPresents) {
				color = Color.FromArgb(40, drawOptions.Color);
				pieDrawOptions.FillStyle.FillMode = FillMode.Solid;
			}
			GraphicsCommand command = new SimpleAntialiasingGraphicsCommand();
			GraphicsCommand pieCommand = pie.HolePercent == 100 ? pie.CreateBorderGraphicsCommand(color, 1, basePoint) :
				pie.CreateGraphicsCommand(color, drawOptions.ActualColor2, pieDrawOptions.FillStyle, basePoint);
			command.AddChildCommand(pieCommand);
			SeriesHitTestState hitState = Series.HitState;
			if (selectionState != SelectionState.Normal)
				command.AddChildCommand(pie.CreateHatchGraphicsCommand(hitState.GetPointHatchColor(Chart.SeriesSelectionMode, selectionState), basePoint));
			if (pieDrawOptions.Border.ActualVisibility) {
				Color borderColor = BorderHelper.CalculateBorderColor(pieDrawOptions.Border, BorderColorFromAppearance);
				int actualBorderThickness = borderThickness > 0 ? borderThickness : pieDrawOptions.Border.ActualThickness;
				command.AddChildCommand(pie.CreateBorderGraphicsCommand(borderColor, actualBorderThickness, basePoint));
			}
			return command;
		}
		protected override void Render(IRenderer renderer, Pie pie, PointF basePoint, bool negativeValuesPresents, SelectionState selectionState, int borderThickness, SimpleDiagramDrawOptionsBase drawOptions) {
			PieDrawOptions pieDrawOptions = drawOptions as PieDrawOptions;
			if (pieDrawOptions == null || !pie.ShouldCreateGraphicsCommand())
				return;
			Color color = drawOptions.Color;
			if (negativeValuesPresents) {
				color = Color.FromArgb(40, drawOptions.Color);
				pieDrawOptions.FillStyle.FillMode = FillMode.Solid;
			}
			renderer.EnableAntialiasing(true);
			if (pie.HolePercent == 100)
				pie.RenderBorder(renderer, color, 1, basePoint);
			else
				pie.Render(renderer, color, drawOptions.ActualColor2, pieDrawOptions.FillStyle, basePoint);
			if (selectionState != SelectionState.Normal)
				pie.RenderHatch(renderer, HitTestState.GetPointHatchColor(Chart.SeriesSelectionMode, selectionState), basePoint);
			if (pieDrawOptions.Border.ActualVisibility) {
				Color borderColor = BorderHelper.CalculateBorderColor(pieDrawOptions.Border, BorderColorFromAppearance);
				int actualBorderThickness = borderThickness > 0 ? borderThickness : pieDrawOptions.Border.ActualThickness;
				pie.RenderBorder(renderer, borderColor, actualBorderThickness, basePoint);
			}
			renderer.RestoreAntialiasing();
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
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new PieDrawOptions(this);
		}
		protected internal override DiagramPoint? CalculateRelativeToolTipPosition(SeriesPointLayout pointLayout) {
			return CalculateAchorPoint(pointLayout);
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new PieSeriesLabel();
		}
		protected internal override bool CalculateBounds(RefinedSeriesData seriesData, Rectangle seriesAreaExcludingTitles, out Rectangle pieBounds, out Rectangle pieWithLabelsBounds) {
			pieBounds = Rectangle.Empty;
			pieWithLabelsBounds = Rectangle.Empty;
			if (seriesAreaExcludingTitles.Width < MinWidthAtWhichPieStillRendered || seriesAreaExcludingTitles.Height < MinHeightAtWhichPieStillRendered)
				return false;
			DirectionOfContactPieWithBounds directionOfContact;
			Rectangle maxPossiblePieBounds = CalculateMaxPossiblePieBounds(seriesAreaExcludingTitles, out directionOfContact);
			Size maxlabelOrAnnotationSize = CalculateMaxLabelOrAnnotationInLableModeSize(seriesData, maxPossiblePieBounds);
			switch (directionOfContact) {
				case DirectionOfContactPieWithBounds.Horizontal:
					pieBounds = GetCorrectedByLabelsInHorizontalDirectionPieBounds(maxPossiblePieBounds, seriesAreaExcludingTitles, maxlabelOrAnnotationSize);
					break;
				case DirectionOfContactPieWithBounds.Vertical:
					pieBounds = GetCorrectedByLabelsInVerticalDirectionPieBounds(maxPossiblePieBounds, seriesAreaExcludingTitles, maxlabelOrAnnotationSize);
					break;
				default:
					ChartDebug.Fail("Unknown DirectionOfContactPieWithBounds");
					break;
			}
			pieWithLabelsBounds = pieBounds;
			pieWithLabelsBounds.Inflate(maxlabelOrAnnotationSize.Width, maxlabelOrAnnotationSize.Height);
			pieWithLabelsBounds = Rectangle.Intersect(pieWithLabelsBounds, seriesAreaExcludingTitles);
			ChartDebug.Assert(pieBounds.IsInto(pieWithLabelsBounds));
			return true;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ISimpleDiagram2DSeriesView simpleDiagramView = obj as ISimpleDiagram2DSeriesView;
			if (simpleDiagramView != null) {
				this.border.Assign(simpleDiagramView.Border);
				this.fillStyle.Assign(simpleDiagramView.FillStyle);
			}
			PieSeriesView view = obj as PieSeriesView;
			if (view == null)
				return;
			this.angleDegree = view.angleDegree;
			this.heightToWidthRatio = view.heightToWidthRatio;
			this.runtimeExploding = view.runtimeExploding;
			this.minAllowedSizeFraction = view.minAllowedSizeFraction;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			PieSeriesView view = (PieSeriesView)obj;
			return
				minAllowedSizeFraction == view.minAllowedSizeFraction &&
				angleDegree == view.angleDegree &&
				heightToWidthRatio == view.heightToWidthRatio &&
				border.Equals(view.border) &&
				fillStyle.Equals(view.fillStyle);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	enum DirectionOfContactPieWithBounds {
		Horizontal,
		Vertical
	}
}
