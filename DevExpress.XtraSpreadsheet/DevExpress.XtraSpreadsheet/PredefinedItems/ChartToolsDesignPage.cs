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
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Services;
using ChartsModel = DevExpress.Charts.Model;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetChartsDesignTypeItemBuilder
	public class SpreadsheetChartsDesignTypeItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ChartChangeType));
		}
	}
	#endregion
	#region SpreadsheetChartsDesignDataItemBuilder
	public class SpreadsheetChartsDesignDataItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ChartSwitchRowColumn));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ChartSelectData));
		}
	}
	#endregion
	#region SpreadsheetChartsDesignLayoutsItemBuilder
	public class SpreadsheetChartsDesignLayoutsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			if (creationContext.IsRibbon) {
				items.Add(new GalleryChartLayoutItem());
			}
		}
	}
	#endregion
	#region SpreadsheetChartsDesignStylesItemBuilder
	public class SpreadsheetChartsDesignStylesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			if (creationContext.IsRibbon) {
				items.Add(new GalleryChartStyleItem());
			}
		}
	}
	#endregion
	#region GalleryChartLayoutItem
	public class GalleryChartLayoutItem : SpreadsheetCommandGalleryBarItem {
		#region Fields
		ChartPresetCategory presetCategory;
		IList<ChartLayoutModifier> modifiers;
		const int minColumnCount = 1;
		const int maxColumnCount = 6;
		const int rowCount = 2;
		#endregion
		public GalleryChartLayoutItem() {
			ResetChartViewType();
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.ModifyChartLayout; } }
		protected override void Initialize() {
			base.Initialize();
			Gallery.MinimumColumnCount = minColumnCount;
			Gallery.ColumnCount = maxColumnCount;
			Gallery.RowCount = rowCount;
			Gallery.ShowItemText = false;
			Gallery.DrawImageBackground = false;
			Gallery.ImageSize = new Size(48, 48);
		}
		void ResetChartViewType() {
			this.presetCategory = ChartPresetCategory.None;
			this.modifiers = new List<ChartLayoutModifier>();
		}
		protected override void OnControlUpdateUI(object sender, EventArgs e) {
			if (DesignMode)
				return;
			base.OnControlUpdateUI(sender, e);
			Gallery.BeginUpdate();
			try {
				PopulateGalleryItems();
			}
			finally {
				Gallery.EndUpdate();
			}
		}
		protected internal virtual void PopulateGalleryItems() {
			ModifyChartLayoutCommand command = new ModifyChartLayoutCommand(Control);
			ChartPresetCategory chartPresetCategory = command.CalculateChartPresetCategory();
			if (chartPresetCategory == presetCategory)
				return;
			this.presetCategory = chartPresetCategory;
			this.modifiers = ChartLayoutModifier.GetModifiers(this.presetCategory);
			if (presetCategory == ChartPresetCategory.None) {
				Gallery.Groups.Clear();
				return;
			}
			GalleryItemGroup group = new GalleryItemGroup();
			for (int i = 0; i < modifiers.Count; i++) {
				group.Items.Add(CreateGalleryItem(command, modifiers[i], i));
			}
			Gallery.Groups.Clear();
			Gallery.Groups.Add(group);
		}
		GalleryItem CreateGalleryItem(ModifyChartLayoutCommand command, ChartLayoutModifier modifier, int index) {
			command.Modifier = modifier;
			GalleryItem item = new GalleryItem();
			item.Image = command.LargeImage;
			item.Hint = "Layout " + (index + 1).ToString();
			item.Tag = modifier;
			return item;
		}
		protected override void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreateCommandUIState(command);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected override Command CreateCommand() {
			Command command = base.CreateCommand();
			ModifyChartLayoutCommand layoutCommand = command as ModifyChartLayoutCommand;
			if (layoutCommand != null) {
				ChartPresetCategory chartPresetCategory = layoutCommand.CalculateChartPresetCategory();
				IList<ChartLayoutModifier> modifiers = ChartLayoutModifier.GetModifiers(chartPresetCategory);
				if (modifiers != null && modifiers.Count > 0)
					layoutCommand.Modifier = modifiers[0];
			}
			return command;
		}
		protected internal virtual ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<ChartLayoutModifier> state = (DefaultValueBasedCommandUIState<ChartLayoutModifier>)command.CreateDefaultCommandUIState();
			state.Value = SelectedItem.Tag as ChartLayoutModifier;
			return state;
		}
		protected override void OnControlChanged() {
			ResetChartViewType();
		}
		protected override ICommandUIState CreateGalleryItemUIState() {
			return new BarGalleryItemValueUIState<CellStyleBase>(this);
		}
	}
	#endregion
	#region GalleryChartStyleItem
	public class GalleryChartStyleItem : SpreadsheetCommandGalleryBarItem {
		#region Fields
		Size itemSize = new Size(93, 56);
		const int minColumnCount = 1;
		const int maxColumnCount = 5;
		const int maxDropDownColumnCount = 6;
		const int rowCount = 9;
		const int selectionWidth = 3;
		const int maxFontSize = 18;
		public const int presetsCount = 48;
		readonly Dictionary<Size, ChartStyleImageCache> imageCacheTable = new Dictionary<Size, ChartStyleImageCache>();
		DocumentModel thumbnailDocumentModel = new DocumentModel();
		#endregion
		public GalleryChartStyleItem() {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.ModifyChartStyle; } }
		protected override bool DropDownGalleryShowGroupCaption { get { return false; } }
		protected override bool CorrectItemSizeWhenUseHighDPI { get { return false; } }
		protected override void Dispose(bool disposing) {
			try {
				ClearCache();
				if (thumbnailDocumentModel != null) {
					thumbnailDocumentModel.Dispose();
					thumbnailDocumentModel = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		ChartStyleImageCache GetStyleImageCache(Size size) {
			if (Control == null)
				return null;
			ChartStyleImageCache result;
			if (!imageCacheTable.TryGetValue(size, out result)) {
				result = new ChartStyleImageCache(thumbnailDocumentModel);
				result.ImageSize = size;
				imageCacheTable[size] = result;
			}
			return result;
		}
		void ClearCache() {
			if (this.imageCacheTable == null)
				return;
			foreach (Size key in imageCacheTable.Keys)
				imageCacheTable[key].Dispose();
			this.imageCacheTable.Clear();
		}
		protected override void Initialize() {
			base.Initialize();
			Gallery.MinimumColumnCount = 4;
			Gallery.ColumnCount = 8;
			Gallery.RowCount = 6;
			Gallery.ItemSize = itemSize;
			Gallery.ItemAutoSizeMode = GalleryItemAutoSizeMode.None;
			Gallery.ShowItemText = false;
			Gallery.ShowGroupCaption = false;
			Gallery.DrawImageBackground = false;
		}
		protected override void InitDropDownGallery(object sender, InplaceGalleryEventArgs e) {
			base.InitDropDownGallery(sender, e);
			InDropDownGallery popupGallery = e.PopupGallery;
			popupGallery.ItemSize = itemSize;
			popupGallery.ItemAutoSizeMode = GalleryItemAutoSizeMode.None;
			popupGallery.ShowItemText = false;
			popupGallery.ShowGroupCaption = false;
			popupGallery.DrawImageBackground = false;
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			GalleryCustomDrawItemImage += OnCustomDrawItem;
		}
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			Control.DocumentLoaded += OnDocumentLoaded;
			Control.EmptyDocumentCreated += OnDocumentCreated;
			Control.DocumentModel.ThemeChanged += OnThemeChanged;
			Control.SelectionChanged += OnSelectionChanged;
		}
		protected override void UnsubscribeControlEvents() {
			base.UnsubscribeControlEvents();
			Control.DocumentLoaded -= OnDocumentLoaded;
			Control.EmptyDocumentCreated -= OnDocumentCreated;
			Control.DocumentModel.ThemeChanged -= OnThemeChanged;
			Control.SelectionChanged -= OnSelectionChanged;
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			ClearCache();
		}
		void OnDocumentCreated(object sender, EventArgs e) {
			ClearCache();
		}
		void OnDocumentLoaded(object sender, EventArgs e) {
			ClearCache();
		}
		void OnThemeChanged(object sender, EventArgs e) {
			ClearCache();
		}
		protected override void OnControlChanged() {
			base.OnControlChanged();
			ClearCache();
			PopulateGalleryItems();
		}
		protected internal virtual void PopulateGalleryItems() {
			if (DesignMode)
				return;
			GalleryItemGroup commonGroup = new GalleryItemGroup();
			for (int i = 0; i < presetsCount; i++)
				commonGroup.Items.Add(CreateGalleryItem(i));
			Gallery.Groups.Clear();
			Gallery.Groups.Add(commonGroup);
		}
		GalleryItem CreateGalleryItem(int index) {
			GalleryItem item = new GalleryItem();
			item.Hint = "Style " + (index + 1).ToString();
			item.Tag = index;
			return item;
		}
		void OnCustomDrawItem(object sender, GalleryItemCustomDrawEventArgs e) {
			if (Control == null || Control.IsDisposed || imageCacheTable == null)
				return;
			if (!(e.Item.Tag is int))
				return;
			int styleIndex = (int)e.Item.Tag;
			if (styleIndex < 0 || styleIndex >= presetsCount)
				return;
			GalleryItemViewInfo viewInfo = e.ItemInfo as GalleryItemViewInfo;
			if (viewInfo == null)
				return;
			Worksheet sheet = Control.InnerControl.DocumentModel.ActiveSheet;
			SheetViewSelection selection = sheet.Selection;
			if (!selection.IsChartSelected)
				return;
			Chart chart = sheet.DrawingObjects[selection.SelectedDrawingIndexes[0]] as Chart;
			if (chart == null)
				return;
			Rectangle bounds = viewInfo.Bounds;
			Size size = bounds.Size;
			ChartStyleImageCache styleImageCache = this.GetStyleImageCache(size);
			Image image = CalculateChartImage(styleImageCache, chart);
			if (image == null)
				return;
			Rectangle contentBounds = viewInfo.ContentBounds;
			e.Cache.Graphics.DrawImage(image, contentBounds, styleIndex * size.Width + contentBounds.X - bounds.X, contentBounds.Y - bounds.Y, contentBounds.Width, contentBounds.Height, GraphicsUnit.Pixel);
			e.Handled = true;
		}
		Image CalculateChartImage(ChartStyleImageCache styleImageCache, Chart chart) {
			if (thumbnailDocumentModel == null)
				return null;
			int key = ChartStyleImageCache.CalculateKey(chart);
			Image image = styleImageCache.TryGetChartImage(key);
			if (image != null)
				return image;
			IChartControllerFactoryService service = chart.DocumentModel.GetService<IChartControllerFactoryService>();
			if (service != null)
				this.thumbnailDocumentModel.AddService(typeof(IChartControllerFactoryService), service);
			try {
				Chart thumbnailChart = CreateThumbnailChart(chart);
				if (thumbnailChart == null)
					return null;
				return styleImageCache.CalculateChartImage(thumbnailChart, key);
			}
			finally {
				if (service != null)
					this.thumbnailDocumentModel.RemoveService(typeof(IChartControllerFactoryService));
			}
		}
		Chart CreateThumbnailChart(Chart originalChart) {
			this.thumbnailDocumentModel.BeginSetContent();
			try {
				this.thumbnailDocumentModel.OfficeTheme = Control.InnerControl.DocumentModel.OfficeTheme.Clone();
				this.thumbnailDocumentModel.Sheets.Add(this.thumbnailDocumentModel.CreateWorksheet());
				this.thumbnailDocumentModel.ActiveSheetIndex = 0;
				Chart result = new Chart(this.thumbnailDocumentModel.ActiveSheet);
				result.BeginUpdate();
				try {
					IChartView view = CreateThumbnailView(result, originalChart.Views[0]);
					if (view == null)
						return null;
					result.Views.Add(view);
				}
				finally {
					result.EndUpdate();
				}
				return result;
			}
			finally {
				this.thumbnailDocumentModel.EndSetContent(DocumentModelChangeType.None);
			}
		}
		IChartView CreateThumbnailView(Chart targetChart, IChartView originalView) {
			ChartViewThumbnailFactory factory = new ChartViewThumbnailFactory(targetChart);
			originalView.Visit(factory);
			factory.SetupChart();
			return factory.Result;
		}
		protected override void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreateCommandUIState(command);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected internal virtual ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<int> state = new DefaultValueBasedCommandUIState<int>();
			if (SelectedItem.Tag is int)
				state.Value = (int)SelectedItem.Tag + 1;
			else
				state.Value = 1;
			return state;
		}
		protected override ICommandUIState CreateGalleryItemUIState() {
			return new BarGalleryItemValueUIState<int>(this);
		}
	}
	#endregion
	#region ChartViewThumbnailFactory
	public class ChartViewThumbnailFactory : IChartViewVisitor {
		readonly Chart targetChart;
		readonly DocumentModel targetDocumentModel;
		public ChartViewThumbnailFactory(Chart targetChart) {
			Guard.ArgumentNotNull(targetChart, "targetChart");
			this.targetChart = targetChart;
			this.targetDocumentModel = targetChart.DocumentModel;
		}
		public IChartView Result { get; set; }
		void CreateTwoAxes() {
			InsertChartCommandBase.CreateTwoPrimaryAxes(targetChart, false, true, AxisPosition.Bottom, AxisPosition.Left, AxisCrossBetween.Between);
			HideAxes();
		}
		void HideAxes() {
			foreach (AxisBase axis in targetChart.PrimaryAxes)
				axis.Delete = true;
		}
		void AddSeries(SeriesBase series, int[] values) {
			ChartDataReference dataReference = new ChartDataReference(targetDocumentModel, Result.SeriesDirection, true);
			VariantArray array = VariantArray.Create(values.Length, 1);
			for (int i = 0; i < values.Length; i++)
				array[i] = values[i];
			dataReference.CachedValue = VariantValue.FromArray(array);
			series.Values = dataReference;
			int index = Result.Series.Count;
			series.Index = index;
			series.Order = index;
			Result.Series.Add(series);
		}
		void AddBarSeries(int[] values) {
			BarSeries series = new BarSeries(Result);
			AddSeries(series, values);
		}
		void AddLineSeries(int[] values, bool showMarkers) {
			LineSeries series = new LineSeries(Result);
			AddSeries(series, values);
			series.Marker.Symbol = showMarkers ? MarkerStyle.Auto : MarkerStyle.None;
		}
		void AddAreaSeries(int[] values) {
			AreaSeries series = new AreaSeries(Result);
			AddSeries(series, values);
		}
		void AddPieSeries(int[] values, int explosionPercent) {
			PieSeries series = new PieSeries(Result);
			AddSeries(series, values);
			series.Explosion = explosionPercent;
		}
		void AddScatterSeries(int[] values, bool showLines, bool showMarkers, bool smoothLines) {
			ScatterSeries series = new ScatterSeries(Result);
			AddSeries(series, values);
			series.Marker.Symbol = showMarkers ? MarkerStyle.Auto : MarkerStyle.None;
			series.ShapeProperties.Outline.Fill = showLines ? DrawingFill.Automatic : DrawingFill.None;
			series.Smooth = smoothLines;
		}
		void AddBubbleSeries(int[] values, bool bubble3D) {
			BubbleSeries series = new BubbleSeries(Result);
			AddSeries(series, values);
			series.Bubble3D = bubble3D;
		}
		void AddRadarSeries(int[] values, bool showMarkers) {
			RadarSeries series = new RadarSeries(Result);
			AddSeries(series, values);
			series.Marker.Symbol = showMarkers ? MarkerStyle.Auto : MarkerStyle.None;
		}
		void SetupView3D(int xRotation, int yRotation, int perspective, bool rightAngleAxes) {
			View3DOptions view3D = targetChart.View3D;
			view3D.BeginUpdate();
			try {
				view3D.XRotation = xRotation;
				view3D.YRotation = yRotation;
				view3D.Perspective = perspective;
				view3D.RightAngleAxes = rightAngleAxes;
			}
			finally {
				view3D.EndUpdate();
			}
		}
		#region IChartViewVisitor implementation
		public void Visit(SurfaceChartView view) {
		}
		public void Visit(Surface3DChartView view) {
		}
		public void Visit(StockChartView view) {
			StockChartView thumbnailView = new StockChartView(targetChart);
			Result = thumbnailView;
			CreateTwoAxes();
			if (view.Series.Count > 3)
				AddLineSeries(new int[] { 8, 6 }, false);
			AddLineSeries(new int[] { 12, 10 }, false);
			AddLineSeries(new int[] { 6, 4 }, false);
			AddLineSeries(new int[] { 10, 8 }, false);
		}
		#region Scatter
		public void Visit(ScatterChartView view) {
			ScatterChartView thumbnailView = new ScatterChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			InsertChartCommandBase.CreateTwoPrimaryAxes(targetChart, true, true, AxisPosition.Bottom, AxisPosition.Left, AxisCrossBetween.Between);
			HideAxes();
			bool showLines = CalculateShowLines(view);
			bool showMarkers = CalculateShowMarkers(view);
			bool smoothLines = CalculateSmoothLines(view);
			AddScatterSeries(new int[] { 4, 6, 8 }, showLines, showMarkers, smoothLines);
			if (view.Series.Count > 1)
				AddScatterSeries(new int[] { 8, 10, 5 }, showLines, showMarkers, smoothLines);
		}
		bool CalculateShowLines(ChartViewWithVaryColors view) {
			if (view.Series.Count > 0) {
				SeriesWithErrorBarsAndTrendlines series = view.Series[0] as SeriesWithErrorBarsAndTrendlines;
				if (series != null)
					return series.ShapeProperties.Outline.Fill != DrawingFill.None;
			}
			return false;
		}
		bool CalculateShowMarkers(ChartViewWithVaryColors view) {
			if (view.Series.Count > 0) {
				SeriesWithMarkerAndSmooth series = view.Series[0] as SeriesWithMarkerAndSmooth;
				if (series != null)
					return series.Marker.Symbol != MarkerStyle.None;
			}
			return false;
		}
		bool CalculateSmoothLines(ChartViewWithVaryColors view) {
			if (view.Series.Count > 0) {
				SeriesWithMarkerAndSmooth series = view.Series[0] as SeriesWithMarkerAndSmooth;
				if (series != null)
					return series.Smooth;
			}
			return false;
		}
		#endregion
		#region Radar
		public void Visit(RadarChartView view) {
			RadarChartView thumbnailView = new RadarChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			HideAxes();
			thumbnailView.RadarStyle = view.RadarStyle;
			bool showMarkers = CalculateShowMarkers(view);
			AddRadarSeries(new int[] { 8, 4, 6, 8 }, showMarkers);
			if (view.Series.Count > 1)
				AddRadarSeries(new int[] { 9, 10, 8, 10 }, showMarkers);
		}
		bool CalculateShowMarkers(RadarChartView view) {
			if (view.Series.Count > 0) {
				RadarSeries series = view.Series[0] as RadarSeries;
				if (series != null)
					return series.Marker.Symbol != MarkerStyle.None;
			}
			return false;
		}
		#endregion
		public void Visit(PieChartView view) {
			PieChartView thumbnailView = new PieChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			int explosionPercent = CalculateExplosionPercent(view);
			AddPieSeries(new int[] { 60, 20, 10, 10 }, explosionPercent);
		}
		int CalculateExplosionPercent(ChartViewBase view) {
			if (view.Series.Count > 0) {
				PieSeries pieSeries = view.Series[0] as PieSeries;
				if (pieSeries != null)
					return pieSeries.Explosion;
			}
			return 0;
		}
		public void Visit(Pie3DChartView view) {
			SetupView3D(30, 0, 30, false);
			Pie3DChartView thumbnailView = new Pie3DChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			int explosionPercent = CalculateExplosionPercent(view);
			AddPieSeries(new int[] { 60, 20, 10, 10 }, explosionPercent);
		}
		public void Visit(OfPieChartView view) {
		}
		public void Visit(LineChartView view) {
			LineChartView thumbnailView = new LineChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			thumbnailView.Grouping = view.Grouping;
			CreateTwoAxes();
			bool showMarkers = CalculateShowMarkers(view);
			AddLineSeries(new int[] { 8, 4, 6, 8 }, showMarkers);
			if (view.Series.Count > 1)
				AddLineSeries(new int[] { 9, 10, 8, 10 }, showMarkers);
		}
		public void Visit(Line3DChartView view) {
			SetupView3D(20, 15, 30, false);
			Line3DChartView thumbnailView = new Line3DChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			thumbnailView.Grouping = view.Grouping;
			CreateTwoAxes();
			AddLineSeries(new int[] { 8, 4, 6, 8 }, false);
			if (view.Series.Count > 1)
				AddLineSeries(new int[] { 9, 10, 8, 10 }, false);
		}
		public void Visit(DoughnutChartView view) {
			DoughnutChartView thumbnailView = new DoughnutChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			int explosionPercent = CalculateExplosionPercent(view);
			AddPieSeries(new int[] { 60, 20, 10, 10 }, explosionPercent);
			thumbnailView.HoleSize = view.HoleSize;
		}
		public void Visit(BubbleChartView view) {
			BubbleChartView thumbnailView = new BubbleChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			InsertChartCommandBase.CreateTwoPrimaryAxes(targetChart, true, true, AxisPosition.Bottom, AxisPosition.Left, AxisCrossBetween.Between);
			HideAxes();
			bool bubble3D = CalculateBubble3D(view);
			thumbnailView.Bubble3D = view.Bubble3D;
			AddBubbleSeries(new int[] { 2, 1, 2 }, bubble3D);
		}
		bool CalculateBubble3D(ChartViewWithVaryColors view) {
			if (view.Series.Count > 0) {
				BubbleSeries series = view.Series[0] as BubbleSeries;
				if (series != null)
					return series.Bubble3D;
			}
			return false;
		}
		public void Visit(BarChartView view) {
			BarChartView thumbnailView = new BarChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			thumbnailView.Grouping = view.Grouping;
			thumbnailView.BarDirection = view.BarDirection;
			CreateTwoAxes();
			AddBarSeries(new int[] { 10, 6 });
			if (view.Series.Count > 1) {
				AddBarSeries(new int[] { 6, 10 });
				AddBarSeries(new int[] { 5, 5 });
			}
		}
		public void Visit(Bar3DChartView view) {
			SetupView3D(20, 15, 30, view.Grouping != BarChartGrouping.Standard);
			Bar3DChartView thumbnailView = new Bar3DChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			thumbnailView.Grouping = view.Grouping;
			thumbnailView.BarDirection = view.BarDirection;
			thumbnailView.Shape = view.Shape;
			CreateTwoAxes();
			AddBarSeries(new int[] { 10, 6 });
			if (view.Series.Count > 1) {
				AddBarSeries(new int[] { 6, 10 });
				AddBarSeries(new int[] { 5, 5 });
			}
		}
		public void Visit(AreaChartView view) {
			AreaChartView thumbnailView = new AreaChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			thumbnailView.Grouping = view.Grouping;
			CreateTwoAxes();
			AddAreaSeries(new int[] { 10, 10, 6, 8, 10 });
			if (view.Series.Count > 1)
				AddAreaSeries(new int[] { 8, 8, 4, 2, 3 });
		}
		public void Visit(Area3DChartView view) {
			SetupView3D(20, 15, 30, false);
			Area3DChartView thumbnailView = new Area3DChartView(targetChart);
			thumbnailView.VaryColors = view.VaryColors;
			Result = thumbnailView;
			thumbnailView.Grouping = view.Grouping;
			CreateTwoAxes();
			AddAreaSeries(new int[] { 10, 10, 6, 8, 10 });
			if (view.Series.Count > 1)
				AddAreaSeries(new int[] { 8, 8, 4, 2, 3 });
		}
		#endregion
		public void SetupChart() {
			targetChart.ShapeProperties.Outline.Fill = DrawingFill.None;
		}
	}
	#endregion
	#region ChartStyleImageCache
	public class ChartStyleImageCache : IDisposable {
		readonly DocumentModel documentModel;
		readonly Dictionary<int, Image> images = new Dictionary<int, Image>();
		Size imageSize;
		public ChartStyleImageCache(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		public Size ImageSize {
			get { return imageSize; }
			set {
				if (imageSize == value)
					return;
				imageSize = value;
				ClearImages();
			}
		}
		public static int CalculateKey(Chart chart) {
			if (chart == null)
				return -1;
			if (chart.Views.Count <= 0)
				return -1;
			IChartView view = chart.Views[0];
			return (int)view.ViewType;
		}
		public Image TryGetChartImage(int key) {
			Image image;
			if (images.TryGetValue(key, out image))
				return image;
			return null;
		}
		public Image CalculateChartImage(Chart chart, int key) {
			Image image;
			if (images.TryGetValue(key, out image))
				return image;
			image = CreateChartImage(chart);
			if (image != null)
				images[key] = image;
			return image;
		}
		Image CreateChartImage(Chart chart) {
			if (chart == null)
				return null;
			IChartControllerFactoryService service = documentModel.GetService<IChartControllerFactoryService>();
			if (service == null || service.Factory == null || chart.Controller == null || chart.Controller.ChartModel == null)
				return null;
			chart.Antialiasing = ChartAntialiasing.Disabled;
			Bitmap bitmap = new Bitmap(ImageSize.Width * GalleryChartStyleItem.presetsCount, ImageSize.Height);
			using (Graphics graphics = Graphics.FromImage(bitmap)) {
				if (chart.Is3DChart) {
					using (Bitmap partBitmap = new Bitmap(ImageSize.Width, ImageSize.Height)) {
						using (Graphics partGraphics = Graphics.FromImage(partBitmap)) {
							for (int i = 0; i < GalleryChartStyleItem.presetsCount; i++) {
								chart.Style = i + 1;
								ChartsModel.ModelRect rect = new ChartsModel.ModelRect(0, 0, ImageSize.Width, ImageSize.Height);
								chart.Controller.RenderChart(service.Factory.CreateRenderContext(rect, partGraphics));
								partBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
								graphics.DrawImage(partBitmap, ImageSize.Width * i, 0);
							}
						}
					}
				}
				else {
					for (int i = 0; i < GalleryChartStyleItem.presetsCount; i++) {
						chart.Style = i + 1;
						graphics.ResetTransform();
						graphics.TranslateTransform(ImageSize.Width * i, 0);
						ChartsModel.ModelRect rect = new ChartsModel.ModelRect(0, 0, ImageSize.Width, ImageSize.Height);
						chart.Controller.RenderChart(service.Factory.CreateRenderContext(rect, graphics));
					}
				}
			}
			return bitmap;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ChartStyleImageCache() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				ClearImages();
		}
		void ClearImages() {
			foreach (int key in images.Keys) {
				Image image = images[key];
				if (image != null)
					image.Dispose();
			}
			images.Clear();
		}
		public void Reset() {
			ClearImages();
		}
	}
	#endregion
	#region SpreadsheetChartsDesignTypeBarCreator
	public class SpreadsheetChartsDesignTypeBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ChartsDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartsDesignTypeRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ChartsDesignTypeBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new ChartsDesignTypeBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetChartsDesignTypeItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ChartsDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartsDesignTypeRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetChartsDesignDataBarCreator
	public class SpreadsheetChartsDesignDataBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ChartsDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartsDesignDataRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ChartsDesignDataBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new ChartsDesignDataBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetChartsDesignDataItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ChartsDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartsDesignDataRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetChartsDesignLayoutsBarCreator
	public class SpreadsheetChartsDesignLayoutsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ChartsDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartsDesignLayoutsRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ChartsDesignLayoutsBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new ChartsDesignLayoutsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetChartsDesignLayoutsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ChartsDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartsDesignLayoutsRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetChartsDesignStylesBarCreator
	public class SpreadsheetChartsDesignStylesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ChartsDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartsDesignStylesRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ChartsDesignStylesBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new ChartsDesignStylesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetChartsDesignStylesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ChartsDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartsDesignStylesRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartToolsRibbonPageCategory();
		}
	}
	#endregion
	#region ChartsDesignTypeBar
	public class ChartsDesignTypeBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ChartsDesignTypeBar() {
		}
		public ChartsDesignTypeBar(BarManager manager)
			: base(manager) {
		}
		public ChartsDesignTypeBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsDesignType); } }
	}
	#endregion
	#region ChartsDesignDataBar
	public class ChartsDesignDataBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ChartsDesignDataBar() {
		}
		public ChartsDesignDataBar(BarManager manager)
			: base(manager) {
		}
		public ChartsDesignDataBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsDesignData); } }
	}
	#endregion
	#region ChartsDesignLayoutsBar
	public class ChartsDesignLayoutsBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ChartsDesignLayoutsBar() {
		}
		public ChartsDesignLayoutsBar(BarManager manager)
			: base(manager) {
		}
		public ChartsDesignLayoutsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsDesignLayouts); } }
	}
	#endregion
	#region ChartsDesignStylesBar
	public class ChartsDesignStylesBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ChartsDesignStylesBar() {
		}
		public ChartsDesignStylesBar(BarManager manager)
			: base(manager) {
		}
		public ChartsDesignStylesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsDesignStyles); } }
	}
	#endregion
	#region ChartsDesignRibbonPage
	public class ChartsDesignRibbonPage : ControlCommandBasedRibbonPage {
		public ChartsDesignRibbonPage() {
		}
		public ChartsDesignRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageChartsDesign); } }
	}
	#endregion
	#region ChartsDesignTypeRibbonPageGroup
	public class ChartsDesignTypeRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ChartsDesignTypeRibbonPageGroup() {
		}
		public ChartsDesignTypeRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsDesignType); } }
	}
	#endregion
	#region ChartsDesignDataRibbonPageGroup
	public class ChartsDesignDataRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ChartsDesignDataRibbonPageGroup() {
		}
		public ChartsDesignDataRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsDesignData); } }
	}
	#endregion
	#region ChartsDesignLayoutsRibbonPageGroup
	public class ChartsDesignLayoutsRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ChartsDesignLayoutsRibbonPageGroup() {
		}
		public ChartsDesignLayoutsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsDesignLayouts); } }
	}
	#endregion
	#region ChartsDesignStylesRibbonPageGroup
	public class ChartsDesignStylesRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ChartsDesignStylesRibbonPageGroup() {
		}
		public ChartsDesignStylesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsDesignStyles); } }
	}
	#endregion
}
