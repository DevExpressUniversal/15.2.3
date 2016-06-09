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
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[TypeConverter(typeof(SimpleDiagramTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")]
	public class SimpleDiagram : Diagram, ISimpleDiagram {
		#region Nested classes: TitlesViewDataByDomain, SeriesGroup, SeriesGroupLayout
		class TitlesViewDataByDomain {
			List<List<DockableTitleViewData>> listOfTitlesViewDataByDomain;
			public List<DockableTitleViewData> this[int index] {
				get { return listOfTitlesViewDataByDomain[index]; }
			}
			public TitlesViewDataByDomain(int domainCount) {
				listOfTitlesViewDataByDomain = new List<List<DockableTitleViewData>>(domainCount);
			}
			public void Add(List<DockableTitleViewData> element) {
				listOfTitlesViewDataByDomain.Add(element);
			}
		}
		abstract class SeriesGroupCalculator<T> {
			protected abstract ISupportSeriesGroups GetSeriesGroupInterface(T series);
			public List<List<T>> CalculateSeriesGroups(IList<T> seriesList) {
				object emptyGroup = new object();
				Dictionary<object, List<T>> result = new Dictionary<object, List<T>>();
				foreach (T series in seriesList) {
					ISupportSeriesGroups seriesGrop = GetSeriesGroupInterface(series);
					if (seriesGrop != null) {
						List<T> group;
						object seriesGroup = (seriesGrop.SeriesGroup != null) ? seriesGrop.SeriesGroup : emptyGroup;
						if (result.TryGetValue(seriesGroup, out group))
							group.Add(series);
						else
							result.Add(seriesGroup, new List<T>() { series });
					}
					else
						result.Add(new object(), new List<T>() { series });
				}
				List<List<T>> resultList = new List<List<T>>();
				foreach (List<T> seriesGroup in result.Values)
					resultList.Add(seriesGroup);
				return resultList;
			}
		}
		class RefinedSeriesDataGroupCalculator : SeriesGroupCalculator<RefinedSeriesData> {
			protected override ISupportSeriesGroups GetSeriesGroupInterface(RefinedSeriesData series) {
				return series.Series.View as ISupportSeriesGroups;
			}
		}
		class RefinedSeriesGroupCalculator : SeriesGroupCalculator<IRefinedSeries> {
			protected override ISupportSeriesGroups GetSeriesGroupInterface(IRefinedSeries series) {
				return series.SeriesView as ISupportSeriesGroups;
			}
		}
		class SeriesGroupLayout {
			readonly List<DockableTitleViewData> titlesViewDataByDomain;
			readonly Rectangle elementBounds;
			readonly Rectangle elementWithLabelsBounds;
			readonly Rectangle domainBounds;
			readonly Rectangle seriesArea;
			readonly List<RefinedSeriesData> seriesGroup;
			public List<DockableTitleViewData> TitleViewDataByDomain { get { return titlesViewDataByDomain; } }
			public Rectangle ElementBounds { get { return elementBounds; } }
			public Rectangle ElementWithLabelsBounds { get { return elementWithLabelsBounds; } }
			public Rectangle DomainBounds { get { return domainBounds; } }
			public Rectangle SeriesArea { get { return seriesArea; } }
			public List<RefinedSeriesData> SeriesGroup { get { return seriesGroup; } }
			public SeriesGroupLayout(List<DockableTitleViewData> titlesViewDataByDomain, Rectangle elementBounds, Rectangle elementWithLabelsBounds, Rectangle domainBounds, Rectangle seriesArea, List<RefinedSeriesData> seriesGroup) {
				this.titlesViewDataByDomain = titlesViewDataByDomain;
				this.elementBounds = elementBounds;
				this.elementWithLabelsBounds = elementWithLabelsBounds;
				this.domainBounds = domainBounds;
				this.seriesArea = seriesArea;
				this.seriesGroup = seriesGroup;
			}
		}
		#endregion
		const int MinPieWidthForPieSizeEqualing = 2;
		const LayoutDirection DefaultLayoutDirection = LayoutDirection.Horizontal;
		const int DefaultDimension = 3;
		const int DefaultMargins = 0;
		const bool DefaultEqualPieSize = false;
		internal static GRealSize2D MinimumSize { get { return new GRealSize2D(150, 150); } }
		readonly RectangleIndents margins;
		LayoutDirection layoutDirection = DefaultLayoutDirection;
		int dimension = DefaultDimension;
		ISimpleDiagramPanel customPanel;
		ISimpleDiagramPanel actualPanel;
		bool equalPieSize = DefaultEqualPieSize;
		protected internal override GRealSize2D MinimumLayoutSize { get { return MinimumSize; } }
		protected internal override bool HasAutoLayoutElements { get { return HasAutoLayoutTitles(); } }
		protected internal override bool SupportTooltips {
			get { return true; }
		}
		protected internal override bool DependsOnBounds { get { return Chart.AutoLayout; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SimpleDiagramEqualPieSize"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SimpleDiagram.EqualPieSize"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public bool EqualPieSize {
			get { return equalPieSize; }
			set {
				if (value != equalPieSize) {
					SendNotification(new ElementWillChangeNotification(this));
					equalPieSize = value;
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		DefaultValue(null)
		]
		public ISimpleDiagramPanel CustomPanel {
			get { return customPanel; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				customPanel = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SimpleDiagramMargins"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SimpleDiagram.Margins"),
		Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleIndents Margins {
			get { return margins; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SimpleDiagramLayoutDirection"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SimpleDiagram.LayoutDirection"),
		Category("Behavior"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public LayoutDirection LayoutDirection {
			get { return layoutDirection; }
			set {
				if (value != layoutDirection) {
					SendNotification(new ElementWillChangeNotification(this));
					layoutDirection = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SimpleDiagramDimension"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SimpleDiagram.Dimension"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Dimension {
			get { return dimension; }
			set {
				if (value != dimension) {
					if (value < SimpleDiagramAutoLayoutHelper.MinDimension || value > SimpleDiagramAutoLayoutHelper.MaxDimension)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSimpleDiagramDimension));
					SendNotification(new ElementWillChangeNotification(this));
					dimension = value;
					RaiseControlChanged();
				}
			}
		}
		public SimpleDiagram() : base() {
			margins = new RectangleIndents(this, DefaultMargins);
			this.actualPanel = new SimpleDiagramPanel(this);
		}
		#region ShouldSeriazlize
		bool ShouldSerializeEqualPieSize() {
			return this.equalPieSize != DefaultEqualPieSize;
		}
		void ResetEqualPieSize() {
			EqualPieSize = DefaultEqualPieSize;
		}
		bool ShouldSerializeMargins() {
			return margins.ShouldSerialize();
		}
		bool ShouldSerializeLayoutDirection() {
			return layoutDirection != DefaultLayoutDirection;
		}
		void ResetLayoutDirection() {
			LayoutDirection = DefaultLayoutDirection;
		}
		bool ShouldSerializeDimension() {
			return this.dimension != DefaultDimension;
		}
		void ResetDimension() {
			Dimension = DefaultDimension;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeMargins() || ShouldSerializeLayoutDirection() || ShouldSerializeDimension() || ShouldSerializeEqualPieSize();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "LayoutDirection":
					return ShouldSerializeLayoutDirection();
				case "Dimension":
					return ShouldSerializeDimension();
				case "Margins":
					return ShouldSerializeMargins();
				case "EqualPieSize":
					return ShouldSerializeEqualPieSize();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ISimpleDiagram
		int ISimpleDiagram.Dimension { get { return Dimension; } }
		SimpleDiagramLayoutDirection ISimpleDiagram.LayoutDirection { get { return (SimpleDiagramLayoutDirection)layoutDirection; } }
		#endregion
		bool HasAutoLayoutTitles() {
			List<IRefinedSeries> seriesList = GetFirstSeriesOfGroups();
			List<IRefinedSeries> autoLayoutSeries = new List<IRefinedSeries>();
			foreach (IRefinedSeries series in seriesList) {
				if (IsAutoLayoutSeries(series))
					return true;
			}
			return false;
		}
		bool IsAutoLayoutSeries(IRefinedSeries series) {
			SimpleDiagramSeriesViewBase view = series.SeriesView as SimpleDiagramSeriesViewBase;
			if (view != null && view.Titles.Count > 0) {
				foreach (SeriesTitle title in view.Titles) {
					if (title.Visibility == DefaultBoolean.Default)
						return true;
				}
			}
			return false;
		}
		bool CorrectElementAndElementWithLabelsBounds(Size minSize, ref Rectangle correctedElelementBounds, ref Rectangle correctedElelementWithLabelsBounds) {
			ChartDebug.Assert(minSize.Width <= correctedElelementBounds.Width && minSize.Height <= correctedElelementBounds.Height, "Min size greater than current size.");
			int diffWidth = correctedElelementBounds.Width - minSize.Width;
			int diffHeight = correctedElelementBounds.Height - minSize.Height;
			int xOffset = diffWidth / 2;
			int yOffset = diffHeight / 2;
			correctedElelementBounds.Location = new Point(correctedElelementBounds.Location.X + xOffset, correctedElelementBounds.Location.Y + yOffset);
			correctedElelementBounds.Width = minSize.Width;
			correctedElelementBounds.Height = minSize.Height;
			correctedElelementWithLabelsBounds.Location = new Point(correctedElelementWithLabelsBounds.Location.X + xOffset, correctedElelementWithLabelsBounds.Location.Y + yOffset);
			correctedElelementWithLabelsBounds.Width -= diffWidth;
			correctedElelementWithLabelsBounds.Height -= diffHeight;
			if (correctedElelementBounds.AreWidthAndHeightPositive())
				return true;
			else
				return false;
		}
		List<IRefinedSeries> GetFirstSeriesOfGroups() {
			IList<IRefinedSeries> seriesList = new List<IRefinedSeries>();
			foreach (IRefinedSeries refinedSeries in ViewController.RefinedSeriesForLegend) {
				SeriesBase series = (SeriesBase)refinedSeries.Series;
				if (refinedSeries.Series.ShouldBeDrawnOnDiagram && refinedSeries.Points.Count > 0)
					seriesList.Add(refinedSeries);
			}
			RefinedSeriesGroupCalculator seriesGroups = new RefinedSeriesGroupCalculator();
			List<List<IRefinedSeries>> groups = seriesGroups.CalculateSeriesGroups(seriesList);
			List<IRefinedSeries> result = new List<IRefinedSeries>();
			foreach (List<IRefinedSeries> group in groups) {
				if (group.Count > 0)
					result.Add(group[0]);
			}
			return result;
		}
		IList<IRefinedSeries> GetFirstSeriesOfGroups(IList<List<RefinedSeriesData>> seriesGroups) {
			List<IRefinedSeries> result = new List<IRefinedSeries>();
			foreach (List<RefinedSeriesData> seriesGroup in seriesGroups)
				result.Add(seriesGroup[0].RefinedSeries);
			return result;
		}
		IList<Series> GetSeriesList(IList<IRefinedSeries> refinedSeries) {
			List<Series> result = new List<Series>();
			foreach (IRefinedSeries series in refinedSeries)
				result.Add((Series)series.Series);
			return result;
		}
		IList<Rectangle> CalculateSeriesGroupAreaList(IList<IRefinedSeries> firstSeriesOfGroups, Rectangle diagramBounds) {
			ISimpleDiagramPanelInternal internalPanel = actualPanel as ISimpleDiagramPanelInternal;
			if (internalPanel == null)
				return actualPanel.Arrange(GetSeriesList(firstSeriesOfGroups), diagramBounds);
			else
				return internalPanel.Arrange(firstSeriesOfGroups, diagramBounds);
		}
		IList<Rectangle> CalculateSeriesGroupAreaList(IList<List<RefinedSeriesData>> seriesGroups, Rectangle diagramBounds) {
			IList<IRefinedSeries> firstSeriesOfGroups = GetFirstSeriesOfGroups(seriesGroups);
			return CalculateSeriesGroupAreaList(firstSeriesOfGroups, diagramBounds);
		}
		List<SeriesGroupLayout> CalculateSeriesGroupLayouts(IList<List<RefinedSeriesData>> seriesGroups, IList<Rectangle> seriesGroupAreaList, TextMeasurer textMeasurer) {
			IEnumerator<Rectangle> seriesGroupAreaEnumerator = seriesGroupAreaList.GetEnumerator();
			List<SeriesGroupLayout> seriesGroupLayoutList = new List<SeriesGroupLayout>();
			foreach (List<RefinedSeriesData> seriesGroup in seriesGroups) {
				seriesGroupAreaEnumerator.MoveNext();
				if (seriesGroup.Count > 0) {
					RefinedSeriesData refinedSeriesData = seriesGroup[0];
					SimpleDiagramSeriesViewBase seriesView = (SimpleDiagramSeriesViewBase)refinedSeriesData.Series.View;
					Rectangle seriesArea = seriesGroupAreaEnumerator.Current;
					Rectangle seriesAreaExcludingTitles = seriesArea;
					List<DockableTitleViewData> titlesViewDataForCurrentSeries = seriesView.Titles.CalculateViewDataAndBoundsWithoutTitle(textMeasurer, ref seriesAreaExcludingTitles);
					Rectangle elementBounds;
					Rectangle elementWithLabelsBounds;
					bool boundsCalculationSuccess = seriesView.CalculateBounds(refinedSeriesData, seriesAreaExcludingTitles, out elementBounds, out elementWithLabelsBounds);
					if (!boundsCalculationSuccess) {
						elementBounds = Rectangle.Empty;
						elementWithLabelsBounds = Rectangle.Empty;
					}
					seriesGroupLayoutList.Add(new SeriesGroupLayout(titlesViewDataForCurrentSeries, elementBounds, elementWithLabelsBounds, seriesAreaExcludingTitles, seriesArea, seriesGroup));
				}
			}
			return seriesGroupLayoutList;
		}
		Size CalculateMinPieOrDoughnutSize(IList<SeriesGroupLayout> seriesGroupLayoutList) {
			Size minPieOrDoughnutSize = new Size(int.MaxValue, int.MaxValue);
			foreach (SeriesGroupLayout seriesGroupLayout in seriesGroupLayoutList) {
				Rectangle elementBounds = seriesGroupLayout.ElementBounds;
				if (!elementBounds.IsEmpty) {
					if (elementBounds.Width < minPieOrDoughnutSize.Width)
						minPieOrDoughnutSize.Width = elementBounds.Width;
					if (elementBounds.Height < minPieOrDoughnutSize.Height)
						minPieOrDoughnutSize.Height = elementBounds.Height;
				}
			}
			return minPieOrDoughnutSize;
		}
		protected override ChartElement CreateObjectForClone() {
			return new SimpleDiagram();
		}
		protected override void UpdateDiagramAccordingInfo(SeriesBase seriesTemplate, IEnumerable<IRefinedSeries> activeSeries) {
			base.UpdateDiagramAccordingInfo(seriesTemplate, activeSeries);
			foreach (Series series in Chart.Series) {
				PieSeriesViewBase view = series.View as PieSeriesViewBase;
				if (view != null)
					view.ApplyExplodeFiltersToPoints();
			}
		}
		protected internal override void UpdateAutomaticLayout(Rectangle bounds) {
			List<IRefinedSeries> series = GetFirstSeriesOfGroups();
			int actualDimension = SimpleDiagramAutoLayoutHelper.CalculateDimension(bounds.Width, bounds.Height, series.Count);
			CustomizeSimpleDiagramLayoutEventArgs e = new CustomizeSimpleDiagramLayoutEventArgs(actualDimension, LayoutDirection.Horizontal);
			Chart.ContainerAdapter.OnCustomizeSimpleDiagramLayout(e);
			dimension = e.Dimension;
			layoutDirection = e.LayoutDirection;
		}
		protected internal override void FinishLoading() {
			base.FinishLoading();
			foreach (Series series in Chart.Series) {
				PieSeriesViewBase view = series.View as PieSeriesViewBase;
				if (view != null)
					view.FinishUpdateExplodedPoints();
			}
		}
		protected internal override INativeGraphics CreateNativeGraphics(Graphics gr, IntPtr hDC, Rectangle bounds, Rectangle windowsBounds) {
			return new GdiPlusGraphics(gr);
		}
		protected internal override List<VisibilityLayoutRegion> GetAutolayoutElements(Rectangle bounds) {
			List<VisibilityLayoutRegion> regions = new List<VisibilityLayoutRegion>();
			List<IRefinedSeries> seriesList = GetFirstSeriesOfGroups();
			IList<Rectangle> boundsList = CalculateSeriesGroupAreaList(seriesList, bounds);
			for (int i = 0; i < seriesList.Count; i++) {
				IRefinedSeries series = seriesList[i];
				if (IsAutoLayoutSeries(series)) {
					List<ISupportVisibilityControlElement> elements = new List<ISupportVisibilityControlElement>();
					SimpleDiagramSeriesViewBase view = series.SeriesView as SimpleDiagramSeriesViewBase;
					if (view != null) {
						foreach (SeriesTitle title in view.Titles) {
							if (title.Visibility == DefaultBoolean.Default)
								elements.Add(title);
						}
					}
					GRealSize2D size = new GRealSize2D(boundsList[i].Width, boundsList[i].Height);
					regions.Add(new VisibilityLayoutRegion(size, elements));
				}
			}
			return regions;
		}
		protected internal override DiagramViewData CalculateViewData(TextMeasurer textMeasurer, Rectangle diagramBounds, IList<RefinedSeriesData> seriesDataList, bool performRangeCorrection) {
			List<SeriesLayout> seriesLayoutList = new List<SeriesLayout>();
			List<SeriesLabelLayoutList> labelLayoutLists = new List<SeriesLabelLayoutList>();
			List<AnnotationLayout> annotationsAnchorPointsLayout = new List<AnnotationLayout>();
			diagramBounds = margins.DecreaseRectangle(diagramBounds);
			if (!diagramBounds.AreWidthAndHeightPositive())
				return null;
			if (this.customPanel != null)
				actualPanel = customPanel;
			RefinedSeriesDataGroupCalculator calculator = new RefinedSeriesDataGroupCalculator();
			IList<List<RefinedSeriesData>> seriesGroups = calculator.CalculateSeriesGroups(seriesDataList);
			IList<Rectangle> seriesGroupAreaList = CalculateSeriesGroupAreaList(seriesGroups, diagramBounds);
			if (seriesGroupAreaList != null && seriesGroupAreaList.Count > 0) {
				List<SeriesGroupLayout> seriesGroupLayoutList = CalculateSeriesGroupLayouts(seriesGroups, seriesGroupAreaList, textMeasurer);
				Size minPieOrDoughnutSize = CalculateMinPieOrDoughnutSize(seriesGroupLayoutList);
				foreach (SeriesGroupLayout seriesGroupLayout in seriesGroupLayoutList) {
					List<RefinedSeriesData> seriesGroup = seriesGroupLayout.SeriesGroup;
					int seriesGroupCount = seriesGroup.Count;
					if (seriesGroupCount > 0) {
						Rectangle elementBounds = seriesGroupLayout.ElementBounds;
						RefinedSeriesData refinedSeriesData = seriesGroup[0];
						Rectangle elementWithLabelsBounds = seriesGroupLayout.ElementWithLabelsBounds;
						if (elementBounds.AreWidthAndHeightPositive()) {
							if (this.equalPieSize && refinedSeriesData.Series.View is PieSeriesView)
								if (!CorrectElementAndElementWithLabelsBounds(minPieOrDoughnutSize, ref elementBounds, ref elementWithLabelsBounds))
									continue;
							SimpleDiagramSeriesLayout firstSeriesLayout = new SimpleDiagramSeriesLayout(refinedSeriesData, textMeasurer, seriesGroupLayout.TitleViewDataByDomain,
								diagramBounds, seriesGroupLayout.SeriesArea, seriesGroupLayout.DomainBounds, elementWithLabelsBounds, elementBounds);
							seriesLayoutList.Add(firstSeriesLayout);
							labelLayoutLists.Add(firstSeriesLayout.LabelLayoutList);
							annotationsAnchorPointsLayout.AddRange(firstSeriesLayout.AnnotationsAnchorPointsLayout);
							for (int i = 1; i < seriesGroupCount; i++) {
								SimpleDiagramSeriesLayout seriesLayout = new SimpleDiagramSeriesLayout(seriesGroup[i], textMeasurer, null, diagramBounds,
									seriesGroupLayout.SeriesArea, seriesGroupLayout.DomainBounds, elementWithLabelsBounds, firstSeriesLayout.Domain.ElementBounds);
								seriesLayoutList.Add(seriesLayout);
								labelLayoutLists.Add(seriesLayout.LabelLayoutList);
								annotationsAnchorPointsLayout.AddRange(seriesLayout.AnnotationsAnchorPointsLayout);
							}
						}
					}
				}
			}
			return new SimpleDiagramViewData(this, diagramBounds, seriesLayoutList, labelLayoutLists, annotationsAnchorPointsLayout);
		}
		protected internal override bool Contains(object obj) {
			return false;
		}
		protected internal override bool CanDrag(Point point, MouseButtons button) {
			ChartHitInfo hitInfo = Chart.CalcHitInfo(point);
			Series series = hitInfo.SeriesInsideDiagram as Series;
			if (series == null || hitInfo.SeriesPoint == null || series.UseRandomPoints)
				return false;
			SimpleDiagramSeriesViewBase view = series.View as SimpleDiagramSeriesViewBase;
			return view != null && view.CanDrag(hitInfo);
		}
		protected internal override bool PerformDragging(int x, int y, int dx, int dy, ChartScrollEventType scrollEventType, Object focusedElement) {
			ChartHitInfo hitInfo = Chart.CalcHitInfo(x, y);
			Series series = hitInfo.SeriesInsideDiagram as Series;
			RefinedPoint seriesPoint = hitInfo.RefinedPointInsideDiagram;
			if (series == null || seriesPoint == null)
				return false;
			SimpleDiagramSeriesViewBase view = series.View as SimpleDiagramSeriesViewBase;
			return view != null && view.PerformDragging(seriesPoint, dx, dy);
		}
		protected internal override string GetDesignerHint(Point p) {
			ChartHitInfo hitInfo = Chart.CalcHitInfo(p);
			Series series = hitInfo.SeriesInsideDiagram as Series;
			if (series != null && CanDrag(p, MouseButtons.None)) {
				SimpleDiagramSeriesViewBase view = series.View as SimpleDiagramSeriesViewBase;
				if (view != null)
					return view.GetDesignerHint();
			}
			return String.Empty;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SimpleDiagram diagram = obj as SimpleDiagram;
			if (diagram != null) {
				layoutDirection = diagram.layoutDirection;
				dimension = diagram.dimension;
				margins.Assign(diagram.margins);
				equalPieSize = diagram.equalPieSize;
			}
		}
	}
	public interface ISimpleDiagramPanel {
		IList<Rectangle> Arrange(IList<Series> seriesList, Rectangle diagramBounds);
	}
}
