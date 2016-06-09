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
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class PieSeriesViewBase : SimpleDiagramSeriesViewBase {
		internal static readonly FontFamily ExclamationFontFamily = FontFamily.GenericSansSerif;
		internal static readonly StringAlignment ExclamationAlignment = StringAlignment.Center;
		internal static readonly StringAlignment ExclamationLineAlignment = StringAlignment.Center;
		const PieSweepDirection DefaultSweepDirection = PieSweepDirection.Counterclockwise;
		const double DoublePi = Math.PI * 2.0;
		const double DefaultSize = 100.0;
		const double DefaultExplodedDistancePercentage = 10.0;
		const PieExplodeMode DefaultExplodeMode = PieExplodeMode.None;
		const char Separator = ';';
		double sizeAsPercentage = DefaultSize;
		double explodedDistancePercentage = DefaultExplodedDistancePercentage;
		PieExplodeMode explodeMode = DefaultExplodeMode;
		ExplodedSeriesPointCollection explodedPoints;
		ExplodeFilter explodeFilter;
		int[] explodedPointIds = new int[0];
		SeriesPointFilterCollection explodedPointsFilters;
		PieSweepDirection sweepDirection = DefaultSweepDirection;
		new PieSeriesLabel Label { get { return (PieSeriesLabel)base.Label; } }
		ExplodeFilter ExplodeFilter { get { return explodeFilter; } }
		internal bool ShouldSerializeExplodedPoints {
			get {
				if (explodeMode != PieExplodeMode.UsePoints)
					return false;
				Series series = Owner as Series;
				return series != null && series.UnboundMode && !series.UseRandomPoints;
			}
		}
		protected internal override bool NeedFilterVisiblePoints {
			get {
				return false;
			}
		}
		protected internal override bool ShouldSortPoints { get { return false; } }
		protected internal override bool SerializeSeriesPointID { get { return true; } }
		protected internal virtual int ActualRotation { get { return 0; } }
		protected internal virtual bool ActualRuntimeExploding { get { return false; } }
		protected internal override bool AnnotationLabelModeSupported { get { return true; } }
		protected internal virtual int DepthPercent { get { return 0; } }
		protected internal virtual int HolePercent { get { return 0; } }
		protected internal virtual int LegendHolePercent { get { return 0; } }
		protected virtual double ActualHeightToWidthRatio { get { return 1; } }
		protected override CompatibleViewType CompatibleViewType { get { return CompatibleViewType.SimpleView; } }
		protected internal override ValueLevel[] SupportedValueLevels { get { return new ValueLevel[0]; } }
		protected internal override string DefaultLabelTextPattern {
			get { return "{" + PatternUtils.PercentValuePlaceholder + ":P2}"; }
		}
		[XtraSerializableProperty]
		protected double SizeAsPercentage {  
			get { return sizeAsPercentage; }
			set {
				ValidatePercentValue(value);
				if (value != sizeAsPercentage) {
					SendNotification(new ElementWillChangeNotification(this));
					sizeAsPercentage = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesViewBaseExplodedDistancePercentage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesViewBase.ExplodedDistancePercentage"),
		XtraSerializableProperty
		]
		public double ExplodedDistancePercentage {
			get { return explodedDistancePercentage; }
			set {
				if (value <= 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectExplodedDistancePercentage));
				if (value != explodedDistancePercentage) {
					SendNotification(new ElementWillChangeNotification(this));
					explodedDistancePercentage = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesViewBaseExplodeMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesViewBase.ExplodeMode"),
		TypeConverter(typeof(ExplodeModeTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public PieExplodeMode ExplodeMode {
			get { return explodeMode; }
			set {
				if (value != explodeMode) {
					SendNotification(new ElementWillChangeNotification(this));
					explodeMode = value;
					if (!Loading)
						UpdateExplodeFilter();
					RaiseControlChanged(new SeriesGroupsInteractionUpdateInfo(this));
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesViewBaseExplodedPoints"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesViewBase.ExplodedPoints"),
		TypeConverter(typeof(CollectionTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor("DevExpress.XtraCharts.Design.ExplodedPointsEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public ExplodedSeriesPointCollection ExplodedPoints { get { return explodedPoints; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string ExplodedPointIdsSerializable {
			get {
				int[] ids = explodedPoints.GetPointIds();
				string[] strings = new string[ids.Length];
				for (int i = 0; i < ids.Length; i++)
					strings[i] = ids[i].ToString();
				return SerializingUtils.StringArrayToString(strings);
			}
			set {
				string[] array = SerializingUtils.StringToStringArray(value);
				int[] ids = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
					ids[i] = Convert.ToInt32(array[i]);
				explodedPointIds = ids;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesViewBaseExplodedPointsFilters"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesViewBase.ExplodedPointsFilters"),
		TypeConverter(typeof(CollectionTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.SeriesPointFilterCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public SeriesPointFilterCollection ExplodedPointsFilters { get { return explodedPointsFilters; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public ConjunctionTypes ExplodedPointsFiltersConjuctionMode {
			get { return explodedPointsFilters.ConjunctionMode; }
			set { explodedPointsFilters.ConjunctionMode = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesViewBaseSweepDirection"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesViewBase.SweepDirection"),
		TypeConverter(typeof(EnumTypeConverter)),
		XtraSerializableProperty
		]
		public PieSweepDirection SweepDirection {
			get { return sweepDirection; }
			set {
				if (value != sweepDirection) {
					SendNotification(new ElementWillChangeNotification(this));
					sweepDirection = value;
					RaiseControlChanged();
				}
			}
		}
		public PieSeriesViewBase()
			: base() {
			explodedPoints = new ExplodedSeriesPointCollection(this);
			explodedPointsFilters = new SeriesPointFilterCollection(this);
		}
		public PieSeriesViewBase(int[] explodedPointIds)
			: this() {
			this.explodedPointIds = explodedPointIds;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "SizeAsPercentage":
					return ShouldSerializeSizeAsPercentage();
				case "ExplodedDistancePercentage":
					return ShouldSerializeExplodedDistancePercentage();
				case "ExplodeMode":
					return ShouldSerializeExplodeMode();
				case "ExplodedPointIdsSerializable":
					return ShouldSerializeExplodedPointIdsSerializable();
				case "ExplodedPointsFiltersConjuctionMode":
					return ShouldSerializeExplodedPointsFiltersConjuctionMode();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		protected override void SetIndexCollectionItemOnXtraDeserializing(string propertyName, XtraSetItemIndexEventArgs e) {
			if (propertyName == "ExplodedPointsFilters")
				ExplodedPointsFilters.Add((SeriesPointFilter)e.Item.Value);
			else
				base.SetIndexCollectionItemOnXtraDeserializing(propertyName, e);
		}
		protected override object CreateCollectionItemOnXtraDeserializing(string propertyName, XtraItemEventArgs e) {
			return propertyName == "ExplodedPointsFilters" ? new SeriesPointFilter() :
				base.CreateCollectionItemOnXtraDeserializing(propertyName, e);
		}
		#endregion
		#region ShouldSerialize & Reset
		protected bool ShouldSerializeSizeAsPercentage() {
			return sizeAsPercentage != DefaultSize;
		}
		protected void ResetSizeAsPercentage() {
			SizeAsPercentage = DefaultSize;
		}
		bool ShouldSerializeExplodedDistancePercentage() {
			return explodedDistancePercentage != DefaultExplodedDistancePercentage;
		}
		void ResetExplodedDistancePercentage() {
			ExplodedDistancePercentage = DefaultExplodedDistancePercentage;
		}
		bool ShouldSerializeExplodeMode() {
			return explodeMode != DefaultExplodeMode;
		}
		void ResetExplodeMode() {
			ExplodeMode = DefaultExplodeMode;
		}
		bool ShouldSerializeExplodedPointIdsSerializable() {
			return ShouldSerializeExplodedPoints &&
				(XtraSerializing || (ChartContainer != null && ChartContainer.ControlType == ChartContainerType.WebControl));
		}
		bool ShouldSerializeExplodedPointsFiltersConjuctionMode() {
			return ExplodedPointsFiltersConjuctionMode != SeriesPointFilterCollection.DefaultConjunctionMode;
		}
		bool ShouldSerializeSweepDirection() {
			return sweepDirection != DefaultSweepDirection;
		}
		void ResetSweepDirection() {
			SweepDirection = DefaultSweepDirection;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() ||
				ShouldSerializeSizeAsPercentage() ||
				ShouldSerializeExplodedDistancePercentage() ||
				ShouldSerializeExplodeMode() ||
				ShouldSerializeExplodedPointIdsSerializable() ||
				ShouldSerializeSweepDirection() ||
				ExplodedPointsFilters.Count > 0 ||
				ShouldSerializeExplodedPointsFiltersConjuctionMode();
		}
		#endregion
		RectangleF CorrectBoundsByExploding(RectangleF bounds, RefinedSeriesData seriesData, float explodedFactor, bool canExplode) {
			bool isExploded = IsExploded(seriesData.RefinedSeries);
			if (isExploded || canExplode) {
				if (!isExploded) {
					bool canExplodePoints = false;
					PieWholeSeriesViewData pieWholeViewData = seriesData.WholeViewData as PieWholeSeriesViewData;
					if (pieWholeViewData != null && pieWholeViewData.PositiveValuesCount > 1)
						canExplodePoints = HasExplodedPoints(seriesData.RefinedSeries);
					bool canExplodeAtDesignMode = Series.Chart.Container.DesignMode && canExplodePoints;
					bool canExplodeAtRuntime = !Series.Chart.Container.DesignMode && (ActualRuntimeExploding || canExplodePoints);
					if (!canExplodeAtDesignMode && !canExplodeAtRuntime)
						return bounds;
				}
				float newWidth = bounds.Width / (1 + explodedFactor);
				float newHeight = bounds.Height / (1 + explodedFactor);
				float newX = bounds.X + bounds.Width / 2 - newWidth / 2;
				float newY = bounds.Y + bounds.Height / 2 - newHeight / 2;
				return new RectangleF(newX, newY, newWidth, newHeight);
			}
			return bounds;
		}
		void UpdateExplodeFilter() {
			explodeFilter = ExplodeFilter.CreateInstance(this);
		}
		Pie CreatePieForLegendItem(Rectangle bounds) {
			GRealPoint2D center = new GRealPoint2D(bounds.Left, bounds.Bottom);
			Ellipse ellipse = new Ellipse(center, bounds.Width, bounds.Height);
			return new Pie(0, Math.PI / 2, ellipse, 0, LegendHolePercent);
		}
		SeriesPointLayout FindSeriesPointLayout(SeriesLayout seriesLayout, RefinedPoint refinedPoint) {
			foreach (SeriesPointLayout pointLayout in seriesLayout)
				if (object.ReferenceEquals(pointLayout.PointData.RefinedPoint, refinedPoint))
					return pointLayout;
			return null;
		}
		SeriesPointLayout CalculatePointLayout(ref double startAngle, ISimpleDiagramDomain domain, RefinedSeriesData seriesData, RefinedPointData pointData) {
			IPiePoint pointInfo = pointData.RefinedPoint;
			IRefinedSeries refinedSeries = seriesData.RefinedSeries;
			PieWholeSeriesViewData pieWholeViewData = seriesData.WholeViewData as PieWholeSeriesViewData;
			bool canExplodePoint = CanExplodePoint(pointInfo);
			double explodeFactor = GetExplodedFactor(refinedSeries);
			RectangleF bounds = CorrectBoundsByExploding(domain.ElementBounds, seriesData, (float)explodeFactor, canExplodePoint);
			if (!bounds.AreWidthAndHeightPositive() || domain == null || pointInfo == null || pointInfo.IsEmpty || pieWholeViewData == null)
				return null;
			SizeF boundsSize = bounds.Size;
			PointF basePoint = CalculateBasePoint(bounds);
			SizeF pieSize = CalculatePieSize(refinedSeries, boundsSize);
			Ellipse ellipse = new Ellipse(new GRealPoint2D(), pieSize.Width / 2, pieSize.Height / 2);
			double pieArea = ellipse.Area * pointInfo.NormalizedValue;
			double finishAngle = ellipse.CalcEllipseSectorFinishAngle(pieArea, startAngle);
			double holePercent = CalculateActualHolePercent(refinedSeries, boundsSize);
			Pie pie;
			if (SweepDirection == PieSweepDirection.Counterclockwise)
				pie = new Pie(startAngle, finishAngle, ellipse, DepthPercent, holePercent);
			else {
				double start = DoublePi - finishAngle;
				double finish = DoublePi - startAngle;
				while (start < 0 || finish < 0) {
					start += DoublePi;
					finish += DoublePi;
				}
				pie = new Pie(start, finish, ellipse, DepthPercent, holePercent);
			}
			bool canExplode = pieWholeViewData.PositiveValuesCount > 0 && !pieWholeViewData.NegativeValuePresents && canExplodePoint;
			if (canExplode && IsExploded(pointData.RefinedPoint))
				pie = ExplodePie(pie, explodeFactor);
			GRealPoint2D center = pie.CalculateCenter(basePoint);
			RectangleF pieBounds = new RectangleF((float)(center.X - ellipse.MajorSemiaxis), (float)(center.Y - ellipse.MinorSemiaxis),
				(float)(ellipse.MajorSemiaxis * 2), (float)(ellipse.MinorSemiaxis * 2));
			startAngle = finishAngle;
			return new PieSeriesPointLayout(pointData, domain.Bounds, pie, basePoint, pieBounds,
				ActualBorderThickness, pieWholeViewData.NegativeValuePresents, pieWholeViewData.PositiveValuesCount);
		}
		Pie ExplodePie(Pie pie, double explodedFactor) {
			double angle = pie.HalfAngle;
			double radius = pie.Ellipse.CalcEllipseRadius(angle);
			double explodeX = pie.CenterPoint.X + explodedFactor * radius * Math.Cos(angle);
			double explodeY = pie.CenterPoint.Y - explodedFactor * radius * Math.Sin(angle);
			GRealPoint2D explodeLocation = new GRealPoint2D(explodeX, explodeY);
			Ellipse explodeEllipse = new Ellipse(explodeLocation, pie.MajorSemiaxis, pie.MinorSemiaxis);
			return new Pie(pie.StartAngle, pie.FinishAngle, explodeEllipse, pie.DepthPercent, pie.HolePercent);
		}
		protected abstract GraphicsCommand CreateGraphicsCommand(Pie pie, PointF basePoint, bool negativeValuesPresents, SelectionState selectionState, int borderThickness, SimpleDiagramDrawOptionsBase drawOptions);
		protected abstract void Render(IRenderer renderer, Pie pie, PointF basePoint, bool negativeValuesPresents, SelectionState selectionState, int borderThickness, SimpleDiagramDrawOptionsBase drawOptions);
		protected abstract SimpleDiagramSeriesLayoutBase CreateSimpleDiagramSeriesLayout();
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			Render(renderer, CreatePieForLegendItem(bounds), PointF.Empty, false, selectionState, 1, (SimpleDiagramDrawOptionsBase)seriesPointDrawOptions);
		}
		protected override NormalizedValuesCalculator CreateNormalizedCalculator(ISeries series) {
			return new PieNormalizedValuesCalculator(series);
		}
		protected void ValidatePercentValue(double val) {
			if (val < 0 || val > 100)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPercentValue));
		}
		protected Rectangle CorrectBoundsBySizeAsPercentageAndHeightToWidthRatio(Rectangle bounds) {
			int height, width;
			if ((double)bounds.Height / (double)bounds.Width >= ActualHeightToWidthRatio) {
				width = MathUtils.StrongRound(sizeAsPercentage * bounds.Width / 100.0);
				height = MathUtils.StrongRound(ActualHeightToWidthRatio * width);
			}
			else {
				height = MathUtils.StrongRound(sizeAsPercentage * bounds.Height / 100.0);
				width = MathUtils.StrongRound(height / ActualHeightToWidthRatio);
			}
			int dx = MathUtils.StrongRound((bounds.Width - width) / 2.0);
			int dy = MathUtils.StrongRound((bounds.Height - height) / 2.0);
			return new Rectangle(bounds.X + dx, bounds.Y + dy, width, height);
		}
		protected virtual bool CanExplodePoint(IPiePoint pointInfo) {
			return true;
		}
		protected internal bool HasExplodedPoints(IRefinedSeries refinedSeries) {
			foreach (RefinedPoint refinedPoint in refinedSeries.Points)
				if (!refinedPoint.IsEmpty && IsExploded(refinedPoint))
					return true;
			return false;
		}
		protected internal bool IsExploded(RefinedPoint point) {
			return ExplodeFilter != null ? ExplodeFilter.CheckPoint(point) : false;
		}
		protected internal virtual SizeF CalculatePieSize(IRefinedSeries refinedSeries, SizeF domainSize) {
			return new SizeF(domainSize.Width, domainSize.Height);
		}
		protected internal virtual double CalculateActualHolePercent(IRefinedSeries refinedSeries, SizeF domainSize) {
			return HolePercent;
		}
		protected internal virtual double GetExplodedFactor(IRefinedSeries refinedSeries) {
			return explodedDistancePercentage / 100.0;
		}
		protected internal virtual Ellipse CreateEllipse(double majorSemiaxis, double minorSemiaxis) {
			return new Ellipse(new GRealPoint2D(), majorSemiaxis, minorSemiaxis);
		}
		protected internal virtual bool IsExploded(IRefinedSeries refinedSeries) {
			return false;
		}
		protected internal virtual DiagramPoint? CalculateRelativeToolTipPosition(SeriesPointLayout pointLayout) {
			return null;
		}
		protected internal override PointOptions CreatePointOptions() {
			return new PiePointOptions();
		}
		protected internal override bool CalculateBounds(RefinedSeriesData seriesData, Rectangle domainBounds, out Rectangle pieBounds, out Rectangle pieWithLabelsBounds) {
			pieBounds = domainBounds;
			pieWithLabelsBounds = domainBounds;
			if (!domainBounds.AreWidthAndHeightPositive())
				return false;
			Size maximumLabelSize = Label.CalculateMaximumSize(seriesData, pieBounds.Size);
			Size maximumAnnotationSize = AnnotationHelper.CalculateAnnotationSize(seriesData);
			Size maximumSize = new Size(Math.Max(maximumLabelSize.Width, maximumAnnotationSize.Width), Math.Max(maximumLabelSize.Height, maximumAnnotationSize.Height));
			pieBounds.Inflate(-maximumSize.Width, -maximumSize.Height);
			if (!pieBounds.AreWidthAndHeightPositive())
				return false;
			pieBounds = CorrectBoundsBySizeAsPercentageAndHeightToWidthRatio(pieBounds);
			pieWithLabelsBounds = Rectangle.Inflate(pieBounds, maximumLabelSize.Width, maximumLabelSize.Height);
			return pieBounds.AreWidthAndHeightPositive();
		}
		protected internal override void CheckArgumentScaleType(ScaleType argumentScaleType) {
			base.CheckArgumentScaleType(argumentScaleType);
			if (argumentScaleType != ((SeriesBase)Owner).ArgumentScaleType)
				foreach (SeriesPointFilter filter in explodedPointsFilters)
					if (filter.Key == SeriesPointKey.Argument && !filter.CanConvert(argumentScaleType))
						throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPieArgumentScaleType),
							argumentScaleType.ToString()));
		}
		protected internal override void SetArgumentScaleType(ScaleType argumentScaleType) {
			base.SetArgumentScaleType(argumentScaleType);
			if (argumentScaleType != ((SeriesBase)Owner).ArgumentScaleType)
				foreach (SeriesPointFilter filter in explodedPointsFilters)
					if (filter.Key == SeriesPointKey.Argument)
						filter.ConvertBasedOnScaleType(argumentScaleType);
		}
		protected internal override GraphicsCommand CreateGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			PieSeriesPointLayout pieLayout = (PieSeriesPointLayout)pointLayout;
			return CreateGraphicsCommand(pieLayout.Pie, pieLayout.BasePoint, pieLayout.NegativeValuesPresents, pieLayout.PointData.SelectionState, 0, (SimpleDiagramDrawOptionsBase)drawOptions);
		}
		protected internal override void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			PieSeriesPointLayout pieLayout = (PieSeriesPointLayout)pointLayout;
			Render(renderer, pieLayout.Pie, pieLayout.BasePoint, pieLayout.NegativeValuesPresents, pieLayout.PointData.SelectionState, 0, (SimpleDiagramDrawOptionsBase)drawOptions);
		}
		protected internal override GraphicsCommand CreateShadowGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
		}
		protected internal override void OnEndLoading() {
			UpdateExplodeFilter();
			FinishUpdateExplodedPoints();
		}
		protected internal override void SynchronizePoints() {
			explodedPoints.SynchronizeById();
		}
		protected internal override void CopySettings(ChartElement element) {
			base.CopySettings(element);
			PieSeriesViewBase view = element as PieSeriesViewBase;
			if (view != null)
				ExplodedPoints.CopyPoints(view.ExplodedPoints);
		}
		protected internal override string GetDesignerHint() {
			return ChartLocalizer.GetString(ChartStringId.Msg2DPieExplodingToolTip);
		}
		protected internal override bool CanDrag(ChartHitInfo hitInfo) {
			return Chart.Container.DesignMode ? (Series.UnboundMode && !hitInfo.SeriesPoint.IsAuxiliary) :
				ActualRuntimeExploding;
		}
		protected internal override bool PerformDragging(RefinedPoint refinedPoint, int dx, int dy) {
			Diagram diagram = Chart.Diagram;
			SimpleDiagramSeriesLayoutBase seriesLayout = CreateSimpleDiagramSeriesLayout();
			SeriesPointLayout pointLayout = FindSeriesPointLayout(seriesLayout, refinedPoint);
			if (pointLayout == null)
				return false;
			PieSeriesPointLayout pieLayout = (PieSeriesPointLayout)pointLayout;
			Point centerPoint = new Point((int)Math.Round(pieLayout.Pie.CenterPoint.X), (int)Math.Round(pieLayout.Pie.CenterPoint.Y));
			GRealPoint2D halfPoint = pieLayout.Pie.Ellipse.CalcEllipsePoint(pieLayout.Pie.HalfAngle);
			Chart.LockChanges();
			try {
				if ((Math.Round(halfPoint.X) - centerPoint.X) * dx + (Math.Round(halfPoint.Y) - centerPoint.Y) * dy > 0) {
					if (!ExplodedPoints.Contains(SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint)))
						ExplodedPoints.AddWithEvent(SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint), true);
				}
				else {
					if (ExplodedPoints.Contains(SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint)))
						ExplodedPoints.RemoveWithEvent(SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint), true);
				}
			}
			finally {
				Chart.UnlockChanges();
			}
			return true;
		}
		protected internal override IEnumerable<SeriesPointLayout> CalculatePointsLayout(ISimpleDiagramDomain domain, RefinedSeriesData seriesData) {
			double circleStartAngle = GeometricUtils.NormalizeRadian(MathUtils.Degree2Radian(ActualRotation));
			Ellipse ellipse = new Ellipse(new GRealPoint2D(), domain.ElementBounds.Width / 2.0, domain.ElementBounds.Height / 2.0);
			double angle = ellipse.CalcEllipseAngleFromCircleAngle(circleStartAngle);
			List<SeriesPointLayout> pointsLayout = new List<SeriesPointLayout>();
			foreach (RefinedPointData pointData in seriesData) {
				SeriesPointLayout pointLayout = CalculatePointLayout(ref angle, domain, seriesData, pointData);
				if (pointLayout != null) {
					pointData.ToolTipRelativePosition = CalculateRelativeToolTipPosition(pointLayout);
					pointsLayout.Add(pointLayout);
				}
			}
			return pointsLayout;
		}
		protected internal override WholeSeriesViewData CalculateWholeSeriesViewData(RefinedSeriesData seriesData, GeometryCalculator geometryCalculator) {
			int negativeValuesCount = 0;
			int positiveValuesCount = 0;
			foreach (RefinedPointData pointData in seriesData) {
				RefinedPoint pointInfo = pointData.RefinedPoint;
				if (!pointInfo.IsEmpty) {
					if (((IValuePoint)pointInfo).Value < 0)
						negativeValuesCount++;
					else if (((IValuePoint)pointInfo).Value > 0)
						positiveValuesCount++;
				}
			}
			if (positiveValuesCount == 0) {
				positiveValuesCount = negativeValuesCount;
				negativeValuesCount = 0;
			}
			if (positiveValuesCount == 0 && negativeValuesCount == 0)
				return null;
			return new PieWholeSeriesViewData(positiveValuesCount, negativeValuesCount > 0);
		}
		protected internal override void ChildCollectionChanged(ChartCollectionBase collection, ChartUpdateInfoBase changeInfo) {
			if (collection == explodedPoints)
				RaiseControlChanged(new SeriesGroupsInteractionUpdateInfo(this));
			base.ChildCollectionChanged(collection, changeInfo);
		}
		internal List<ISeriesPoint> CollectActualPoints() {
			List<ISeriesPoint> actualPoints = new List<ISeriesPoint>();
			ViewController controller = GetOwner<ViewController>();
			if (controller != null) {
				IRefinedSeries refinedSeries = controller.FindRefinedSeries(Series);
				if (refinedSeries != null) {
					IList<RefinedPoint> points = refinedSeries.Points;
					for (int i = 0; i < points.Count; i++)
						actualPoints.Add(SeriesPoint.GetSeriesPoint(points[i].SeriesPoint));
				}
			}
			return actualPoints;
		}
		internal void ApplyExplodeFiltersToPoints() {
			if (Loading)
				return;
			if ((explodeMode == PieExplodeMode.UsePoints) && (explodedPoints != null)) {
				explodedPoints.Synchronize();
				return;
			}
			explodedPoints.BeginUpdate(false, false);
			try {
				explodedPoints.Clear();
				if ((Owner is Series) && (Series.Points.Count > 0)) {
					ExplodeFilter explodeFilter = ExplodeFilter.CreateInstance(this);
					if (explodeFilter != null && Owner is Series) {
						List<ISeriesPoint> filteredPoints = explodeFilter.FilterPoints(CollectActualPoints());
						foreach (SeriesPoint point in filteredPoints)
							explodedPoints.Add(point);
					}
				}
			}
			finally {
				explodedPoints.CancelUpdate();
			}
		}
		internal void FinishUpdateExplodedPoints() {
			if (explodeMode == PieExplodeMode.UsePoints) {
				if (explodedPoints.Fill(explodedPointIds))
					explodedPointIds = new int[0];
			}
			else
				ApplyExplodeFiltersToPoints();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			PieSeriesViewBase view = obj as PieSeriesViewBase;
			if (view != null) {
				sizeAsPercentage = view.sizeAsPercentage;
				explodedDistancePercentage = view.explodedDistancePercentage;
				explodeMode = view.explodeMode;
				UpdateExplodeFilter();
				explodedPointIds = view.ExplodedPoints.GetPointIds();
				explodedPointsFilters.Assign(view.explodedPointsFilters);
				sweepDirection = view.sweepDirection;
				Series series = Owner as Series;
				if (series != null && series.Points.Count > 0)
					FinishUpdateExplodedPoints();
			}
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			PieSeriesViewBase view = (PieSeriesViewBase)obj;
			return sizeAsPercentage == view.sizeAsPercentage &&
				explodedDistancePercentage == view.explodedDistancePercentage &&
				explodeMode == view.explodeMode &&
				explodedPoints.Equals(view.explodedPoints) &&
				explodedPointsFilters.Equals(view.explodedPointsFilters);
		}
	}
	[ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PieExplodeMode {
		None,
		All,
		MinValue,
		MaxValue,
		UsePoints,
		UseFilters,
		Others
	}
	[ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PieSweepDirection {
		Clockwise = PointsSweepDirection.Clockwise,
		Counterclockwise = PointsSweepDirection.Counterclockwise,
	}
	public class ExplodedSeriesPointCollection : ChartCollectionBase {
		List<SeriesPoint> pointsToClear = null;
		PieSeriesViewBase View { get { return (PieSeriesViewBase)Owner; } }
		internal Series Series { get { return View.Owner as Series; } }
		internal SeriesBase SeriesBase { get { return (SeriesBase)View.Owner; } }
		public SeriesPoint this[int index] { get { return (SeriesPoint)List[index]; } }
		public ExplodedSeriesPointCollection(PieSeriesViewBase view)
			: base(view) {
		}
		void RaisePieSeriesPointExplodedEvent(List<SeriesPoint> points, bool exploded) {
			foreach (SeriesPoint point in points)
				RaisePieSeriesPointExplodedEvent(point, exploded, false);
		}
		void RaisePieSeriesPointExplodedEvent(SeriesPoint point, bool exploded, bool dragged) {
			if (Series == null || Series.Chart == null || Series.Chart.Container == null)
				return;
			PieSeriesPointExplodedEventArgs e = new PieSeriesPointExplodedEventArgs(Series, point, exploded, dragged);
			Series.Chart.ContainerAdapter.OnPieSeriesPointExploded(e);
		}
		protected override void OnClear() {
			if (pointsToClear == null) {
				pointsToClear = new List<SeriesPoint>();
				if (Series == null)
					return;
				foreach (SeriesPoint point in this)
					pointsToClear.Add(point);
			}
			else
				ChartDebug.Fail("The pointsToClear should be null.");
			base.OnClear();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if (pointsToClear == null) {
				ChartDebug.Fail("The pointsToClear can't be null.");
				return;
			}
			try {
				RaisePieSeriesPointExplodedEvent(pointsToClear, false);
			}
			finally {
				pointsToClear = null;
			}
		}
		protected override void DisposeItem(ChartElement item) {
		}
		protected override void DisposeItemBeforeRemove(ChartElement item) {
		}
		protected override void ChangeOwnerForItem(ChartElement item) {
		}
		protected internal override void ProcessChanged(ChartUpdateInfoBase changeInfo) {
			View.LockChanges();
			try {
				View.ExplodeMode = PieExplodeMode.UsePoints;
			}
			finally {
				View.UnlockChanges();
			}
			base.ProcessChanged(changeInfo);
		}
		internal bool Contains(SeriesPoint point) {
			return IndexOf(point) >= 0;
		}
		internal int IndexOf(SeriesPoint point) {
			if (point.IsAuxiliary) {
				for (int i = 0; i < Count; i++) {
					SeriesPoint item = this[i];
					if (item.IsAuxiliary && point.Argument == item.Argument)
						return i;
					foreach (SeriesPoint sourcePoint in point.SourcePoints)
						if (object.ReferenceEquals(item, sourcePoint))
							return i;
				}
				return -1;
			}
			for (int i = 0; i < Count; i++) {
				SeriesPoint item = this[i];
				foreach (SeriesPoint sourcePoint in item.SourcePoints)
					if (Object.ReferenceEquals(point, sourcePoint))
						return i;
			}
			return base.IndexOf(point);
		}
		internal bool Fill(int[] seriesPointIds) {
			if (seriesPointIds.Length > 0)
				InnerList.Clear();
			foreach (int id in seriesPointIds) {
				SeriesPoint point = null;
				if (id < 0) {
					foreach (SeriesPoint item in View.CollectActualPoints())
						if (item.IsAuxiliary) {
							point = item;
							break;
						}
				}
				else
					point = Series.Points.GetByID(id);
				if (point == null) {
					ChartDebug.Fail("Unexpected series point id.");
					return false;
				}
				InnerList.Add(point);
				RaisePieSeriesPointExplodedEvent(point, true, false);
			}
			return true;
		}
		internal void Synchronize() {
			SeriesPointCollection pointCollection = Series.Points;
			int index = 0;
			while (index < Count) {
				SeriesPoint point = this[index];
				if (pointCollection.Contains(point))
					index++;
				else {
					InnerList.Remove(point);
					RaisePieSeriesPointExplodedEvent(point, false, false);
				}
			}
		}
		internal void SynchronizeById() {
			SeriesPointCollection pointCollection = Series.Points;
			List<SeriesPoint> newPointList = new List<SeriesPoint>();
			foreach (SeriesPoint point in this) {
				SeriesPoint newPoint = pointCollection.GetByID(point);
				if (newPoint != null)
					newPointList.Add(newPoint);
			}
			BeginUpdate(false, false);
			try {
				Clear();
				AddRange(newPointList);
			}
			finally {
				CancelUpdate();
			}
		}
		internal int[] GetPointIds() {
			List<int> ids = new List<int>();
			foreach (SeriesPoint seriesPoint in this)
				if (!seriesPoint.IsEmpty)
					ids.Add(seriesPoint.SeriesPointID);
			return ids.ToArray();
		}
		internal void AddRange(List<SeriesPoint> points) {
			base.AddRange(points);
			RaisePieSeriesPointExplodedEvent(points, true);
		}
		internal void CopyPoints(ExplodedSeriesPointCollection explodedPoints) {
			InnerList.AddRange(explodedPoints);
		}
		internal int AddWithEvent(SeriesPoint point, bool dragged) {
			if (Series == null)
				throw new ExplodedSeriesPointCollectionException(ChartLocalizer.GetString(ChartStringId.MsgInvalidExplodedModeAdd));
			if (point.IsAuxiliary) {
				if (SeriesBase.ChartContainer != null && SeriesBase.ChartContainer.DesignMode)
					return -1;
			}
			else if (!Series.Points.Contains(point))
				throw new ExplodedSeriesPointCollectionException(ChartLocalizer.GetString(ChartStringId.MsgInvalidExplodedSeriesPoint));
			if (Contains(point))
				return -1;
			int addIndex = base.Add(point);
			RaisePieSeriesPointExplodedEvent(point, true, dragged);
			return addIndex;
		}
		internal void RemoveWithEvent(SeriesPoint point, bool dragged) {
			if (Series == null)
				throw new ExplodedSeriesPointCollectionException(ChartLocalizer.GetString(ChartStringId.MsgInvalidExplodedModeRemove));
			int index = IndexOf(point);
			if (index >= 0) {
				RaisePieSeriesPointExplodedEvent(this[index], false, dragged);
				base.RemoveAt(index);
			}
		}
		public int Add(SeriesPoint point) {
			return AddWithEvent(point, false);
		}
		public void Remove(SeriesPoint point) {
			RemoveWithEvent(point, false);
		}
		public void ToggleExplodedState(SeriesPoint point) {
			if (Contains(point))
				Remove(point);
			else
				Add(point);
		}
		public override void Assign(ChartCollectionBase collection) {
			if (collection == null)
				throw new ArgumentNullException("collection");
			InnerList.Clear();
			foreach (SeriesPoint point in collection) {
				SeriesPoint existingPoint = Series.Points.GetByID(point.SeriesPointID);
				if (existingPoint != null)
					InnerList.Add(existingPoint);
			}
		}
		public override bool Equals(object obj) {
			ExplodedSeriesPointCollection collection = (ExplodedSeriesPointCollection)obj;
			if (Count != collection.Count)
				return false;
			for (int i = 0; i < Count; i++)
				if (!this[i].Equals(collection[i]))
					return false;
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
