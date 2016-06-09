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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Utils;
using ChartsModel = DevExpress.Charts.Model;
using DevExpress.XtraSpreadsheet.Model.ModelChart;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Office.Drawing;
using DevExpress.Spreadsheet;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region ChartPivotFormats
	public class ChartPivotFormatCollection : ChartUndoableCollectionSupportsCopyFrom<ChartPivotFormat> {
		public ChartPivotFormatCollection(IChart parent)
			: base(parent) {
		}
		protected override ChartPivotFormat CreateNewItem(ChartPivotFormat source) {
			ChartPivotFormat result = new ChartPivotFormat(Parent, source.Idx);
			result.CopyFrom(source);
			return result;
		}
	}
	public class ChartPivotFormat : ISupportsCopyFrom<ChartPivotFormat> {
		int idx;
		ShapeProperties shapeProperties;
		TextProperties textProperties;
		Marker marker;
		DataLabel dataLabel;
		IChart parent;
		public ChartPivotFormat(IChart parent, int idx) {
			Guard.ArgumentNonNegative(idx, "idx");
			this.parent = parent;
			this.idx = idx;
			this.shapeProperties = new ShapeProperties(parent.DocumentModel);
			this.textProperties = new TextProperties(parent.DocumentModel);
			this.marker = new Marker(parent);
			this.dataLabel = new DataLabel(parent, -1);
		}
		public IChart Parent { get { return parent; } }
		public DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		public int Idx { get { return idx; } }
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		public TextProperties TextProperties { get { return textProperties; } }
		public Marker Marker { get { return marker; } }
		public DataLabel DataLabel { get { return dataLabel; } }
		public void CopyFrom(ChartPivotFormat source) {
			idx = source.idx;
			parent = source.parent;
			shapeProperties.CopyFrom(source.shapeProperties);
			textProperties.CopyFrom(source.textProperties);
			marker.CopyFrom(source.marker);
			dataLabel.CopyFrom(source.dataLabel);
		}
		public void ResetToStyle(MarkerStyle markerSymbol) {
			DocumentModel.BeginUpdate();
			try {
				ShapeProperties.ResetToStyle();
				TextProperties.ResetToStyle();
				Marker.ResetToStyle(markerSymbol);
				DataLabel.ResetToStyle();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region ChartPivotSource
	public class ChartPivotSource : ISupportsCopyFrom<ChartPivotSource> {
		int fmtId;
		string name;
		public ChartPivotSource(int fmtId, string name) {
			this.fmtId = fmtId;
			this.name = name;
		}
		public int FormatId { get { return fmtId; } }
		public string Name { get { return name; } }
		public void CopyFrom(ChartPivotSource value) {
			this.fmtId = value.fmtId;
			this.name = value.name;
		}
	}
	#endregion
	#region DisplayBlanksAs
	public enum DisplayBlanksAs {
		Zero = 0,
		Span = 1,
		Gap = 2
	}
	#endregion
	#region ChartInfo
	public class ChartInfo : ICloneable<ChartInfo>, ISupportsCopyFrom<ChartInfo>, ISupportsSizeOf {
		#region Fields
		const int offsetDispBlankAs = 12;
		const uint maskProtection = 0x0000001f;
		const uint maskDate1904 = 0x00000020;
		const uint maskRoundedCorners = 0x00000040;
		const uint maskPlotVisibleOnly = 0x00000080;
		const uint maskShowDataLabelsOverMax = 0x00000100;
		const uint maskAutoTitleDeleted = 0x00000200;
		const uint maskDispBlanksAs = 0x00003000;
		uint packedValues = 0x00000080;
		CultureInfo culture = new CultureInfo("en-US");
		#endregion
		#region Properties
		public bool AutoTitleDeleted {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskAutoTitleDeleted); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskAutoTitleDeleted, value); }
		}
		public CultureInfo Culture {
			get { return culture; }
			set {
				Guard.ArgumentNotNull(value, "Culture");
				culture = value;
			}
		}
		public bool Date1904 {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskDate1904); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskDate1904, value); }
		}
		public DisplayBlanksAs DispBlanksAs {
			get { return (DisplayBlanksAs)PackedValues.GetIntBitValue(this.packedValues, maskDispBlanksAs, offsetDispBlankAs); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskDispBlanksAs, offsetDispBlankAs, (int)value); }
		}
		public bool PlotVisibleOnly {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskPlotVisibleOnly); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskPlotVisibleOnly, value); }
		}
		public ChartSpaceProtection Protection {
			get { return (ChartSpaceProtection)PackedValues.GetIntBitValue(this.packedValues, maskProtection); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskProtection, (int)value); }
		}
		public bool RoundedCorners {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskRoundedCorners); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskRoundedCorners, value); }
		}
		public bool ShowDataLabelsOverMax {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowDataLabelsOverMax); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowDataLabelsOverMax, value); }
		}
		#endregion
		#region ICloneable<ChartInfo> Members
		public ChartInfo Clone() {
			ChartInfo result = new ChartInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ChartInfo> Members
		public void CopyFrom(ChartInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.culture = (CultureInfo)value.culture.Clone();
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			ChartInfo info = obj as ChartInfo;
			if (info == null)
				return false;
			return (this.packedValues == info.packedValues) && this.culture.Equals(info.culture);
		}
		public override int GetHashCode() {
			CombinedHashCode hashCode = new CombinedHashCode();
			hashCode.AddInt((int)this.packedValues);
			hashCode.AddInt(this.culture.GetHashCode());
			return hashCode.CombinedHash32;
		}
	}
	#endregion
	#region ChartInfoCache
	public class ChartInfoCache : UniqueItemsCache<ChartInfo> {
		public ChartInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override ChartInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new ChartInfo();
		}
	}
	#endregion
	#region IChart
	public interface IChart : ISupportsInvalidate {
		DocumentModel DocumentModel { get; }
		ChartViewCollection Views { get; }
		ChartViewSeriesDirection SeriesDirection { get; set; }
		AxisGroup PrimaryAxes { get; }
		AxisGroup SecondaryAxes { get; }
	}
	#endregion
	#region ChartAntialiasing
	public enum ChartAntialiasing {
		DefinedByOptions,
		Disabled,
		Enabled
	}
	#endregion
	#region Chart
	public partial class Chart : SpreadsheetUndoableIndexBasedObject<ChartInfo>, IDrawingObject, IChart {
		#region Fields
		const int maxStyleValue = 48;
		DrawingObject drawingObject;
		ColorMapOverride colorMapOverride;
		PrintSettings printSettings;
		PlotArea plotArea;
		SurfaceOptions floor;
		SurfaceOptions sideWall;
		SurfaceOptions backWall;
		Legend legend;
		View3DOptions view3D;
		TitleOptions title;
		AxisGroup primaryAxes;
		AxisGroup secondaryAxes;
		ChartViewCollection views;
		ShapeProperties shapeProperties;
		DataTableOptions dataTable;
		TextProperties textProperties;
		ChartPivotFormatCollection pivotFormats;
		ChartPivotSource pivotSource;
		int id;
		int style = 2;
		ChartsModel.Controller controller;
		ChartsModel.Chart modelChart;
		List<ChartDataSource> dataSources;
		bool invalidated = false;
		OfficeImage cachedImage;
		ChartAntialiasing antialiasing;
		ChartViewSeriesDirection seriesDirection;
		bool subscribed = false;
		ChartLocks locks;
		#endregion
		public Chart(Worksheet worksheet)
			: base(worksheet) {
			this.drawingObject = new DrawingObject(worksheet);
			Initialize();
		}
		public Chart(DrawingObject drawingObject)
			: base(drawingObject.Worksheet) {
			this.drawingObject = drawingObject;
			Initialize();
		}
		public Chart(Chartsheet chartsheet)
			: base(chartsheet.DocumentModel) {
			this.drawingObject = new DrawingObject(chartsheet);
			Initialize();
		}
		void Initialize() {
			this.printSettings = new PrintSettings(DocumentModel);
			this.colorMapOverride = new ColorMapOverride(this);
			this.plotArea = new PlotArea(this);
			this.floor = new SurfaceOptions(this);
			this.sideWall = new SurfaceOptions(this);
			this.backWall = new SurfaceOptions(this);
			this.legend = new Legend(this);
			this.view3D = new View3DOptions(this);
			this.title = new ChartTitleOptions(this);
			this.primaryAxes = new AxisGroup(this);
			this.secondaryAxes = new AxisGroup(this);
			this.views = new ChartViewCollection(this);
			this.dataTable = new DataTableOptions(this);
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = this };
			this.textProperties = new TextProperties(DocumentModel) { Parent = this };
			this.pivotFormats = new ChartPivotFormatCollection(this);
			this.pivotSource = null;
			this.controller = null;
			this.dataSources = new List<ChartDataSource>();
			this.cachedImage = null;
			this.antialiasing = ChartAntialiasing.DefinedByOptions;
			this.locks = new ChartLocks(DrawingObject.Locks);
			LinkHandlers();
		}
		#region Properties
		#region PivotSource
		public ChartPivotSource PivotSource { get { return pivotSource; } }
		protected internal void SetPivotSourceCore(ChartPivotSource pivotSource) {
			this.pivotSource = pivotSource;
		}
		#endregion
		#region AutoTitleDeleted
		public bool AutoTitleDeleted {
			get { return Info.AutoTitleDeleted; }
			set {
				if (AutoTitleDeleted == value)
					return;
				SetPropertyValue(SetAutoTitleDeletedCore, value);
			}
		}
		DocumentModelChangeActions SetAutoTitleDeletedCore(ChartInfo info, bool value) {
			info.AutoTitleDeleted = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Culture
		public CultureInfo Culture {
			get { return Info.Culture; }
			set {
				Guard.ArgumentNotNull(value, "Culture");
				if (Info.Culture.Equals(value))
					return;
				SetPropertyValue(SetCultureCore, value);
			}
		}
		DocumentModelChangeActions SetCultureCore(ChartInfo info, CultureInfo value) {
			info.Culture = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Date1904
		public bool Date1904 {
			get { return Info.Date1904; }
			set {
				if (Date1904 == value)
					return;
				SetPropertyValue(SetDate1904Core, value);
			}
		}
		DocumentModelChangeActions SetDate1904Core(ChartInfo info, bool value) {
			info.Date1904 = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DispBlanksAs
		public DisplayBlanksAs DispBlanksAs {
			get { return Info.DispBlanksAs; }
			set {
				if (DispBlanksAs == value)
					return;
				SetPropertyValue(SetDispBlanksAsCore, value);
			}
		}
		DocumentModelChangeActions SetDispBlanksAsCore(ChartInfo info, DisplayBlanksAs value) {
			info.DispBlanksAs = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PlotVisibleOnly
		public bool PlotVisibleOnly {
			get { return Info.PlotVisibleOnly; }
			set {
				if (PlotVisibleOnly == value)
					return;
				SetPropertyValue(SetPlotVisibleOnlyCore, value);
			}
		}
		DocumentModelChangeActions SetPlotVisibleOnlyCore(ChartInfo info, bool value) {
			info.PlotVisibleOnly = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Protection
		public ChartSpaceProtection Protection {
			get { return Info.Protection; }
			set {
				if (Protection == value)
					return;
				SetPropertyValue(SetProtectionCore, value);
			}
		}
		DocumentModelChangeActions SetProtectionCore(ChartInfo info, ChartSpaceProtection value) {
			info.Protection = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region RoundedCorners
		public bool RoundedCorners {
			get { return Info.RoundedCorners; }
			set {
				if (RoundedCorners == value)
					return;
				SetPropertyValue(SetRoundedCornersCore, value);
			}
		}
		DocumentModelChangeActions SetRoundedCornersCore(ChartInfo info, bool value) {
			info.RoundedCorners = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowDataLabelsOverMax
		public bool ShowDataLabelsOverMax {
			get { return Info.ShowDataLabelsOverMax; }
			set {
				if (ShowDataLabelsOverMax == value)
					return;
				SetPropertyValue(SetShowDataLabelsOverMaxCore, value);
			}
		}
		DocumentModelChangeActions SetShowDataLabelsOverMaxCore(ChartInfo info, bool value) {
			info.ShowDataLabelsOverMax = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Style
		public int Style {
			get { return style; }
			set {
				ValueChecker.CheckValue(value, 1, maxStyleValue, "Style");
				if (style == value)
					return;
				SetStyle(value);
			}
		}
		void SetStyle(int value) {
			ChartStylePropertyChangedHistoryItem historyItem = new ChartStylePropertyChangedHistoryItem(DocumentModel, this, this.style, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetStyleCore(int value) {
			this.style = value;
			SetupChartStyle();
		}
		#endregion
		public ColorMapOverride ColorMapOverride { get { return colorMapOverride; } }
		public PrintSettings PrintSettings { get { return printSettings; } }
		public PlotArea PlotArea { get { return plotArea; } }
		public SurfaceOptions Floor { get { return floor; } }
		public SurfaceOptions SideWall { get { return sideWall; } }
		public SurfaceOptions BackWall { get { return backWall; } }
		public Legend Legend { get { return legend; } }
		public View3DOptions View3D { get { return view3D; } }
		public TitleOptions Title { get { return title; } }
		public AxisGroup PrimaryAxes { get { return primaryAxes; } }
		public AxisGroup SecondaryAxes { get { return secondaryAxes; } }
		public ChartViewCollection Views { get { return views; } }
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		public DataTableOptions DataTable { get { return dataTable; } }
		public DrawingObject DrawingObject { get { return drawingObject; } }
		public TextProperties TextProperties { get { return textProperties; } }
		public ChartPivotFormatCollection PivotFormats { get { return pivotFormats; } }
		public bool Is3DChart {
			get {
				if (this.views.Count == 1)
					return this.views[0].Is3DView;
				return false;
			}
		}
		public bool HasSecondaryAxisGroup {
			get {
				if (Is3DChart)
					return false;
				foreach (IChartView view in Views)
					if (object.ReferenceEquals(view.Axes, SecondaryAxes))
						return true;
				return false;
			}
		}
		public ChartsModel.Controller Controller {
			get {
				if (controller == null)
					InitController();
				if (modelChart == null)
					InitModelChart();
				return controller;
			}
		}
		protected ChartsModel.Chart ModelChart { get { return modelChart; } }
		protected OfficeThemeBase<DocumentFormat> Theme { get { return (OfficeThemeBase<DocumentFormat>)DocumentModel.OfficeTheme; } }
		public ChartAntialiasing Antialiasing {
			get { return antialiasing; }
			set {
				if (antialiasing == value)
					return;
				antialiasing = value;
				ResetController();
			}
		}
		public ChartViewSeriesDirection SeriesDirection { get { return seriesDirection; } set { seriesDirection = value; } }
		#endregion
		void LinkHandlers() {
			if (!subscribed) {
				DocumentModel.ContentVersionChanged += DocumentModel_ContentVersionChanged;
				subscribed = true;
			}
		}
		void UnLinkHandlers() {
			if (subscribed) {
				DocumentModel.ContentVersionChanged -= DocumentModel_ContentVersionChanged;
				subscribed = false;
			}
		}
		protected internal void Activate() {
			LinkHandlers();
			UpdateDataReferences();
		}
		protected internal void Deactivate() {
			UnLinkHandlers();
			ResetController();
			if (this.controller != null) {
				IDisposable disposableController = this.controller as IDisposable;
				if (disposableController != null)
					disposableController.Dispose();
				this.controller = null;
			}
		}
		#region ContentVersion changed
		void DocumentModel_ContentVersionChanged(object sender, EventArgs e) {
			UpdateData();
		}
		public void UpdateData() {
			if (DocumentModel.IsSetContentMode)
				return;
			UpdateDataReferences();
			UpdateDataSources();
		}
		public void UpdateDataReferences() {
			bool resetRequired = Title.Text.CheckUpdates();
			foreach (AxisBase axis in PrimaryAxes)
				resetRequired |= axis.Title.Text.CheckUpdates();
			foreach (AxisBase axis in SecondaryAxes)
				resetRequired |= axis.Title.Text.CheckUpdates();
			foreach (IChartView view in views) {
				resetRequired |= view.ViewType == ChartViewType.Bubble;
				bool suppressDateTime = view.ViewType == ChartViewType.Bubble || view.ViewType == ChartViewType.Scatter;
				ChartNumberFormat axisNumberFormat = null;
				CategoryAxis categoryAxis = GetCategoryAxis(view);
				if (categoryAxis != null) {
					suppressDateTime = !categoryAxis.Auto;
					axisNumberFormat = new ChartNumberFormat();
					axisNumberFormat.SourceLinked = categoryAxis.NumberFormat.SourceLinked;
					axisNumberFormat.FormatCode = categoryAxis.NumberFormat.NumberFormatCode;
				}
				resetRequired |= axisNumberFormat != null && axisNumberFormat.SourceLinked;
				foreach (SeriesBase series in view.Series) {
					DataReferenceValueType argumentsType = series.Arguments.ValueType;
					DataReferenceValueType valuesType = series.Values.ValueType;
					series.Arguments.NumberFormat = axisNumberFormat;
					if (axisNumberFormat != null && axisNumberFormat.SourceLinked)
						series.Arguments.ResetCachedValue();
					series.Arguments.DetectValueType(suppressDateTime);
					series.Values.DetectValueType(true);
					resetRequired |= series.Arguments.ValueType != argumentsType || series.Values.ValueType != valuesType || series.Text.CheckUpdates();
				}
			}
			resetRequired |= ChartAxisHelper.UpdateSourceLinkedFormats(this);
			if (resetRequired)
				ResetController();
		}
		CategoryAxis GetCategoryAxis(IChartView view) {
			if (view.Axes != null && view.Axes.Count > 0)
				return view.Axes[0] as CategoryAxis;
			return null;
		}
		void UpdateDataSources() {
			if (dataSources.Count > 0) {
				foreach (ChartDataSource dataSource in dataSources)
					dataSource.OnDataChanged();
			}
			else {
				foreach (IChartView view in views) {
					foreach (ISeries series in view.Series)
						series.OnDataChanged();
				}
			}
		}
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<ChartInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.ChartInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
			this.Invalidate();
		}
		protected override void OnLastEndUpdateCore() {
			base.OnLastEndUpdateCore();
			if (invalidated) {
				invalidated = false;
				ResetController();
				Redraw();
			}
		}
		#endregion
		#region IDrawingObject Members
		public Worksheet Worksheet { get { return drawingObject.Worksheet; } }
		public ChartLocks Locks { get { return locks; } }
		public bool NoChangeAspect { get { return Locks.NoChangeAspect; } set { Locks.NoChangeAspect = value; } }
		public IGraphicFrameInfo GraphicFrameInfo { get { return drawingObject.GraphicFrameInfo; } }
		public bool LocksWithSheet {
			get { return drawingObject.LocksWithSheet; }
			set { drawingObject.LocksWithSheet = value; }
		}
		public bool PrintsWithSheet {
			get { return drawingObject.PrintsWithSheet; }
			set { drawingObject.PrintsWithSheet = value; }
		}
		public AnchorType AnchorType {
			get { return drawingObject.AnchorType; }
			set { drawingObject.AnchorType = value; }
		}
		public AnchorType ResizingBehavior {
			get { return drawingObject.ResizingBehavior; }
			set { drawingObject.ResizingBehavior = value; }
		}
		public AnchorPoint From {
			get { return drawingObject.From; }
			set { drawingObject.From = value; }
		}
		public AnchorPoint To {
			get { return drawingObject.To; }
			set { drawingObject.To = value; }
		}
		public DrawingObjectType DrawingType { get { return DrawingObjectType.Chart; } }
		public bool CanRotate { get { return false; } }
		public float Height {
			get { return drawingObject.Height; }
			set { drawingObject.Height = value; }
		}
		public float Width {
			get { return drawingObject.Width; }
			set { drawingObject.Width = value; }
		}
		public float CoordinateX {
			get { return drawingObject.CoordinateX; }
			set { drawingObject.CoordinateX = value; }
		}
		public float CoordinateY {
			get { return drawingObject.CoordinateY; }
			set { drawingObject.CoordinateY = value; }
		}
		public int ZOrder {
			get { return Worksheet.DrawingObjectsByZOrderCollections.GetZOrder(this); }
			set {
				if (ZOrder == value)
					return;
				DocumentHistory history = DocumentModel.History;
				SetZOrderHistoryItem historyItem = new SetZOrderHistoryItem(this, value, ZOrder);
				history.Add(historyItem);
				historyItem.Execute();
			}
		}
		public int IndexInCollection { get { return id; } }
		public void SetIndexInCollection(int value) {
			id = value;
		}
		public void Move(float offsetY, float offsetX) {
			drawingObject.Move(offsetY, offsetX);
		}
		public void SetIndependentWidth(float width) {
			drawingObject.SetIndependentWidth(width);
		}
		public void SetIndependentHeight(float height) {
			drawingObject.SetIndependentHeight(height);
		}
		public void CoordinatesFromCellKey(CellKey cellKey) {
			drawingObject.CoordinatesFromCellKey(cellKey);
		}
		public void SizeFromCellKey(CellKey cellKey) {
			drawingObject.SizeFromCellKey(cellKey);
		}
		public void Rotate(int angle) {
		}
		public float GetRotationAngleInDegrees() {
			return 0.0f;
		}
		public void EnlargeWidth(float scale) {
			drawingObject.EnlargeWidth(scale);
		}
		public void EnlargeHeight(float scale) {
			drawingObject.EnlargeHeight(scale);
		}
		public void ReduceWidth(float scale) {
			drawingObject.ReduceWidth(scale);
		}
		public void ReduceHeight(float scale) {
			drawingObject.ReduceHeight(scale);
		}
		public void Visit(IDrawingObjectVisitor visitor) {
			visitor.Visit(this);
		}
		public AnchorData AnchorData { get { return drawingObject.AnchorData; } }
		#endregion
		#region ICloneable<Chart> Members
		public Chart Clone() {
			Chart result = new Chart(this.Worksheet);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		public override void CopyFrom(SpreadsheetUndoableIndexBasedObject<ChartInfo> sourceItem) {
			Chart fromChart = sourceItem as Chart;
			if (fromChart == null)
				throw new ArgumentException("CopyFrom argument is not SpreadsheetUndoableIndexBasedObject<ChartInfo>");
			this.CopyFrom(fromChart, CellPositionOffset.None);
		}
		public override void CopyFrom(UndoableIndexBasedObject<ChartInfo, DocumentModelChangeActions> sourceItem) {
			Chart fromChart = sourceItem as Chart;
			if (fromChart == null)
				throw new ArgumentException("CopyFrom argument is not UndoableIndexBasedObject<ChartInfo>");
			this.CopyFrom(fromChart, CellPositionOffset.None);
		}
		public void CopyFrom(Chart value, CellPositionOffset offset) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			this.Style = value.Style;
			this.pivotSource = value.pivotSource; 
			this.drawingObject.CopyFrom(value.drawingObject);
			this.shapeProperties.CopyFrom(value.shapeProperties);
			this.textProperties.CopyFrom(value.textProperties);
			this.pivotFormats.CopyFrom(value.pivotFormats);
			this.printSettings.CopyFrom(value.printSettings);
			this.plotArea.CopyFrom(value.plotArea);
			this.title.CopyFrom(value.title);
			this.legend.CopyFrom(value.legend);
			this.view3D.CopyFrom(value.view3D);
			this.floor.CopyFrom(value.floor);
			this.sideWall.CopyFrom(value.sideWall);
			this.backWall.CopyFrom(value.backWall);
			this.dataTable.CopyFrom(value.dataTable);
			this.colorMapOverride.CopyFrom(value.colorMapOverride);
			this.primaryAxes.CopyFrom(value.primaryAxes);
			this.secondaryAxes.CopyFrom(value.secondaryAxes);
			this.views.Clear();
			for (int i = 0; i < value.views.Count; i++) {
				IChartView sourceView = value.views[i];
				IChartView view = sourceView.CloneTo(this);
				if (sourceView.Axes == value.primaryAxes)
					view.Axes = this.primaryAxes;
				else if (sourceView.Axes == value.secondaryAxes)
					view.Axes = this.secondaryAxes;
				this.views.Add(view);
			}
		}
		#region IChart Members
		public void Invalidate() {
			if (IsUpdateLocked)
				invalidated = true;
			else if (this.modelChart != null) {
				ResetController();
				Redraw();
			}
		}
		void Redraw() {
			if (DocumentModel.IsUpdateLocked)
				DocumentModel.ApplyChanges(DocumentModelChangeActions.Redraw);
			else
				DocumentModel.InnerApplyChanges(DocumentModelChangeActions.Redraw);
		}
		#endregion
		#region Common Charts Model
		void InitController() {
			IChartControllerFactoryService service = DocumentModel.GetService<IChartControllerFactoryService>();
			if (service == null || service.Factory == null)
				return;
			this.controller = service.Factory.CreateController();
		}
		void InitModelChart() {
			if (this.controller == null)
				return;
			BuildModelChart();
			if (this.modelChart != null) {
				SetupChartStyle();
				this.controller.ChartModel = this.modelChart;
			}
			else
				ResetController();
		}
		protected void BuildModelChart() {
			ModelChartBuilder builder = new ModelChartBuilder(this);
			this.modelChart = builder.BuildModelChart();
		}
		void ResetController() {
			if (this.controller != null) {
				this.modelChart = null;
				this.controller.ChartModel = null;
			}
			foreach (ChartDataSource dataSource in this.dataSources) {
				dataSource.ListChanged -= dataSource_ListChanged;
				dataSource.Dispose();
			}
			this.dataSources.Clear();
			ResetCachedImage();
		}
		protected internal void RegisterChartDataSource(ChartDataSource dataSource) {
			dataSource.Parent = this;
			dataSource.ListChanged += dataSource_ListChanged;
			this.dataSources.Add(dataSource);
		}
		void dataSource_ListChanged(object sender, ListChangedEventArgs e) {
			ResetCachedImage();
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				Deactivate();
		}
		#endregion
		#region ReferencesRanged
		public ChartReferencedRanges GetReferencedRanges() {
			FormulaReferencedRanges seriesNameRanges = new FormulaReferencedRanges();
			FormulaReferencedRanges argumentRanges = new FormulaReferencedRanges();
			FormulaReferencedRanges valueRanges = new FormulaReferencedRanges();
			foreach (IChartView view in views) {
				foreach (ISeries series in view.Series) {
					SeriesBase seriesBase = series as SeriesBase;
					if (seriesBase != null) {
						ChartTextRef textRef = seriesBase.Text as ChartTextRef;
						if (textRef != null)
							textRef.ObtainReferencedRanges(seriesNameRanges);
						seriesBase.Arguments.ObtainReferencedRanges(argumentRanges);
						seriesBase.Values.ObtainReferencedRanges(valueRanges);
					}
					BubbleSeries seriesBubble = series as BubbleSeries;
					if (seriesBubble != null)
						seriesBubble.BubbleSize.ObtainReferencedRanges(valueRanges);
				}
			}
			valueRanges = valueRanges.Union();
			seriesNameRanges = seriesNameRanges.Union();
			argumentRanges = argumentRanges.Union();
			valueRanges.AssignColors(0);
			seriesNameRanges.AssignColors(1);
			argumentRanges.AssignColors(2);
			ChartReferencedRanges result = new ChartReferencedRanges();
			result.ValueRanges = valueRanges;
			result.SeriesNameRanges = seriesNameRanges;
			result.ArgumentRanges = argumentRanges;
			return result;
		}
		public FormulaReferencedRanges GetAllReferencedRanges() {
			ChartReferencedRanges ranges = GetReferencedRanges();
			FormulaReferencedRanges result = new FormulaReferencedRanges();
			result.AddRange(ranges.ValueRanges);
			result.AddRange(ranges.SeriesNameRanges);
			result.AddRange(ranges.ArgumentRanges);
			return result;
		}
		public FormulaReferencedRanges GetAllReferencedRanges(Worksheet sheet) {
			FormulaReferencedRanges ranges = GetAllReferencedRanges();
			FormulaReferencedRanges result = new FormulaReferencedRanges();
			foreach (FormulaReferencedRange range in ranges) {
				if (object.ReferenceEquals(range.CellRange.Worksheet, sheet))
					result.Add(range);
			}
			return result;
		}
		#endregion
		#region CachedImage
		void ResetCachedImage() {
			if (this.cachedImage != null) {
				this.cachedImage.Dispose();
				this.cachedImage = null;
			}
		}
		public OfficeImage GetCachedImage(Size size) {
			if (this.cachedImage != null && !this.cachedImage.SizeInPixels.Equals(size))
				ResetController();
#if !SL && !DXPORTABLE
			if (this.cachedImage == null) {
				IChartControllerFactoryService service = DocumentModel.GetService<IChartControllerFactoryService>();
				if (service == null || service.Factory == null || Controller == null)
					return null;
				Image nativeImage = new Bitmap(size.Width, size.Height);
				using (Graphics graphics = Graphics.FromImage(nativeImage)) {
					ChartsModel.ModelRect rect = new ChartsModel.ModelRect(0, 0, size.Width, size.Height);
					Controller.RenderChart(service.Factory.CreateRenderContext(rect, graphics, rect));
				}
				if (this.Is3DChart)
					nativeImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
				this.cachedImage = OfficeImage.CreateImage(nativeImage);
			}
#endif
			return this.cachedImage;
		}
		#endregion
		#region GetChartImage
		public OfficeImage GetChartImage(Size size) {
			IChartControllerFactoryService service = DocumentModel.GetService<IChartControllerFactoryService>();
			if (service == null || service.Factory == null || Controller == null || Controller.ChartModel == null)
				return null;
			if (Is3DChart)
				return GetCachedImage(size);
			else {
#if SL || DXPORTABLE
				return null;
#else
				Bitmap bitmap = new Bitmap(size.Width, size.Height);
				using (Graphics graphics = Graphics.FromImage(bitmap)) {
					ChartsModel.ModelRect rect = new ChartsModel.ModelRect(0, 0, size.Width, size.Height);
					Controller.RenderChart(service.Factory.CreateRenderContext(rect, graphics));
				}
				return OfficeImage.CreateImage(bitmap);
#endif
			}
		}
		#endregion
		public void ResetToStyle() {
			ShapeProperties.ResetToStyle();
			PlotArea.ShapeProperties.ResetToStyle();
			Title.ResetToStyle();
			Legend.ResetToStyle();
			DataTable.ResetToStyle();
			Floor.ResetToStyle();
			BackWall.ResetToStyle();
			SideWall.ResetToStyle();
			foreach (ChartPivotFormat format in PivotFormats)
				format.ResetToStyle(MarkerStyle.Auto); 
			foreach (AxisBase axis in PrimaryAxes)
				axis.ResetToStyle();
			foreach (AxisBase axis in SecondaryAxes)
				axis.ResetToStyle();
			foreach (IChartView view in Views)
				view.ResetToStyle();
		}
		#region Series by order and index
		protected internal List<ISeries> GetSeriesList() {
			List<ISeries> result = new List<ISeries>();
			foreach (IChartView view in Views)
				result.AddRange(view.Series.InnerList);
			result.Sort(CompareByOrderAndIndex);
			return result;
		}
		int CompareByOrderAndIndex(ISeries first, ISeries second) {
			if (first.Order < second.Order)
				return -1;
			if (first.Order > second.Order)
				return 1;
			if (first.Index < second.Index)
				return -1;
			if (first.Index > second.Index)
				return 1;
			return 0;
		}
		#endregion
		DefaultBoolean DocumentCapabilityToDefaultBoolean(DocumentCapability documentCapability) {
			switch (documentCapability) {
				case DocumentCapability.Default:
					return DefaultBoolean.Default;
				case DocumentCapability.Enabled:
					return DefaultBoolean.True;
				default:
					return DefaultBoolean.False;
			}
		}
		DocumentCapability GetTextAntialiasing() {
			if (Antialiasing == ChartAntialiasing.DefinedByOptions)
				return DocumentModel.ViewOptions.Charts.TextAntialiasing;
			return DocumentCapability.Default;
		}
		protected internal bool GetActualAntialiasing() {
			if (Antialiasing == ChartAntialiasing.DefinedByOptions)
				return DocumentModel.ViewOptions.Charts.AntialiasingAllowed;
			return Antialiasing != ChartAntialiasing.Disabled;
		}
		protected internal DefaultBoolean GetActualTextAntialiasing() {
			return DocumentCapabilityToDefaultBoolean(GetTextAntialiasing());
		}
		#region IDrawingObject Members
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			Views.OnRangeInserting(context);
			PrimaryAxes.OnRangeInserting(context);
			SecondaryAxes.OnRangeInserting(context);
			Title.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			Views.OnRangeRemoving(context);
			PrimaryAxes.OnRangeRemoving(context);
			SecondaryAxes.OnRangeRemoving(context);
			Title.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
	#region ChartReferencedRanges
	public class ChartReferencedRanges {
		public FormulaReferencedRanges ValueRanges { get; set; }
		public FormulaReferencedRanges SeriesNameRanges { get; set; }
		public FormulaReferencedRanges ArgumentRanges { get; set; }
		public CellRangeBase GetDataRange() {
			CellRangeBase unionValueRanges = ValueRanges.GetUnionRanges();
			if (unionValueRanges == null)
				return null;
			ICellTable worksheet = ValueRanges[0].CellRange.Worksheet;
			CellRangeBase unionSeriesNameRanges = SeriesNameRanges.GetUnionRanges();
			CellRangeBase unionArgumentRanges = ArgumentRanges.GetUnionRanges();
			CellRange cornerRange = GetTopLeftCornerRange(worksheet, unionSeriesNameRanges, unionArgumentRanges);
			if (unionArgumentRanges != null && cornerRange != null)
				unionArgumentRanges = unionArgumentRanges.MergeWithRange(cornerRange);
			if (unionSeriesNameRanges != null)
				unionValueRanges = unionValueRanges.MergeWithRange(unionSeriesNameRanges);
			CellRangeBase result = unionArgumentRanges != null ? unionArgumentRanges.MergeWithRange(unionValueRanges) : unionValueRanges;
			if (result.Worksheet == null)
				result.Worksheet = worksheet;
			return result;
		}
		CellRange GetTopLeftCornerRange(ICellTable worksheet, CellRangeBase seriesNameRanges, CellRangeBase argumentRanges) {
			if (seriesNameRanges == null || argumentRanges == null)
				return null;
			int seriesNameRangesTopLeftRow = seriesNameRanges.TopLeft.Row;
			int argumentRangesTopLeftRow = argumentRanges.TopLeft.Row;
			if (seriesNameRangesTopLeftRow == argumentRangesTopLeftRow)
				return null;
			if (seriesNameRangesTopLeftRow > argumentRangesTopLeftRow)
				return GetTopLeftCornerRangeForHorizontalDirection(worksheet, seriesNameRanges, argumentRanges);
			return GetTopLeftCornerRangeForVerticalDirection(worksheet, seriesNameRanges, argumentRanges);
		}
		CellRange GetTopLeftCornerRangeForHorizontalDirection(ICellTable worksheet, CellRangeBase seriesNameRanges, CellRangeBase argumentRanges) {
			CellPosition topLeft = new CellPosition(seriesNameRanges.TopLeft.Column, argumentRanges.TopLeft.Row);
			CellPosition bottomRight = new CellPosition(seriesNameRanges.BottomRight.Column, argumentRanges.BottomRight.Row);
			return new CellRange(worksheet, topLeft, bottomRight);
		}
		CellRange GetTopLeftCornerRangeForVerticalDirection(ICellTable worksheet, CellRangeBase seriesNameRanges, CellRangeBase argumentRanges) {
			CellPosition topLeft = new CellPosition(argumentRanges.TopLeft.Column, seriesNameRanges.TopLeft.Row);
			CellPosition bottomRight = new CellPosition(argumentRanges.BottomRight.Column, seriesNameRanges.BottomRight.Row);
			return new CellRange(worksheet, topLeft, bottomRight);
		}
	}
	#endregion
	#region ChartUndoableCollection
	public class ChartUndoableCollection<T> : UndoableCollection<T> {
		readonly IChart parent;
		public ChartUndoableCollection(IChart parent)
			: base(parent.DocumentModel) {
			this.parent = parent;
		}
		protected internal IChart Parent { get { return parent; } }
		public override int AddCore(T item) {
			int result = base.AddCore(item);
			Parent.Invalidate();
			return result;
		}
		protected  override void InsertCore(int index, T item) {
			base.InsertCore(index, item);
			Parent.Invalidate();
		}
		public override void RemoveAtCore(int index) {
			base.RemoveAtCore(index);
			Parent.Invalidate();
		}
		public override void ClearCore() {
			if (Count == 0)
				return;
			base.ClearCore();
			Parent.Invalidate();
		}
		public override bool Remove(T item) {
			int index = IndexOf(item);
			if (index != -1)
				RemoveAt(index);
			return true;
		}
	}
	#endregion
	#region ChartUndoableCollectionSupportsCopyFrom (absract class)
	public abstract class ChartUndoableCollectionSupportsCopyFrom<T> : ChartUndoableCollection<T>, ISupportsCopyFrom<ChartUndoableCollectionSupportsCopyFrom<T>> {
		protected ChartUndoableCollectionSupportsCopyFrom(IChart parent)
			: base(parent) {
		}
		public void CopyFrom(ChartUndoableCollectionSupportsCopyFrom<T> value) {
			Clear();
			int count = value.Count;
			for (int i = 0; i < count; i++) {
				T item = CreateNewItem(value[i]);
				Add(item);
			}
		}
		protected abstract T CreateNewItem(T source);
	}
	#endregion
	#region ChartViewOptionsNotifyHandler
	public partial class DocumentModel {
		void InvalidateCharts(bool charts3DOnly) {
			BeginUpdate();
			try {
				ChartsInvalidateWalker walker = new ChartsInvalidateWalker(charts3DOnly);
				foreach (Worksheet sheet in Sheets)
					InvalidateCharts(sheet, walker);
			}
			finally {
				EndUpdate();
			}
		}
		void InvalidateCharts(Worksheet sheet, ChartsInvalidateWalker walker) {
			foreach (IDrawingObject drawing in sheet.DrawingObjects)
				drawing.Visit(walker);
		}
	}
	#endregion
	#region ChartsInvalidateWalker
	class ChartsInvalidateWalker : IDrawingObjectVisitor {
		bool charts3DOnly;
		public ChartsInvalidateWalker(bool charts3DOnly) {
			this.charts3DOnly = charts3DOnly;
		}
		#region IDrawingObjectVisitor Members
		public void Visit(Picture picture) {
		}
		public void Visit(Chart chart) {
			if (chart.Is3DChart || !charts3DOnly)
				chart.Invalidate();
		}
		public void Visit(ModelShape shape) {
		}
		public void Visit(GroupShape groupShape) {
		}
		public void Visit(ConnectionShape connectionShape) {
		}
		#endregion
	}
	#endregion
	#region ChartsUpdateDataWalker
	class ChartsUpdateDataWalker : IDrawingObjectVisitor {
		#region IDrawingObjectVisitor Members
		public void Visit(Picture picture) {
		}
		public void Visit(Chart chart) {
			chart.UpdateData();
		}
		public void Visit(ModelShape shape) {
		}
		public void Visit(GroupShape groupShape) {
		}
		public void Visit(ConnectionShape connectionShape) {
		}
		#endregion
	}
	#endregion
	#region SeriesFormulaDataWalker
	public class SeriesFormulaDataWalker : ISeriesVisitor {
		Action<IDataReference> ProcessDataReference { get; set; }
		Action<IChartText> ProcessChartText { get; set; }
		public SeriesFormulaDataWalker(Action<IDataReference> processDataReference, Action<IChartText> processChartText) {
			ProcessDataReference = processDataReference;
			ProcessChartText = processChartText;
		}
		public void Walk(Chart targetChart) {
			ProcessChartText(targetChart.Title.Text);
			foreach (AxisBase axis in targetChart.PrimaryAxes) {
				ProcessChartText(axis.Title.Text);
				ProcessDisplayUnitLabelText(axis as ValueAxis);
			}
			foreach (AxisBase axis in targetChart.SecondaryAxes) {
				ProcessChartText(axis.Title.Text);
				ProcessDisplayUnitLabelText(axis as ValueAxis);
			}
			foreach (IChartView view in targetChart.Views) {
				foreach (ISeries series in view.Series) {
					series.Visit(this);
					ProcessChartText(series.Text);
				}
			}
		}
		void DefaultProcessDataReference(ISeries series) {
			ProcessDataReference(series.Arguments);
			ProcessDataReference(series.Values);
		}
		void ProcessDataLabelsText(SeriesWithDataLabelsAndPoints series) {
			foreach (DataLabel dataLabel in series.DataLabels.Labels)
				ProcessChartText(dataLabel.Text);
		}
		void ProcessDisplayUnitLabelText(ValueAxis axis) {
			if (axis != null)
				ProcessChartText(axis.DisplayUnit.Text);
		}
		void ProcessTrendlineLabelText(SeriesWithErrorBarsAndTrendlines series) {
			foreach (Trendline trendline in series.Trendlines) {
				ProcessChartText(trendline.Label.Text);
			}
		}
		#region ISeriesVisitor Members
		public void Visit(AreaSeries series) {
			DefaultProcessDataReference(series);
			ProcessDataLabelsText(series);
			ProcessTrendlineLabelText(series);
		}
		public void Visit(BarSeries series) {
			DefaultProcessDataReference(series);
			ProcessDataLabelsText(series);
			ProcessTrendlineLabelText(series);
		}
		public void Visit(BubbleSeries series) {
			DefaultProcessDataReference(series);
			ProcessDataReference(series.BubbleSize);
			ProcessDataLabelsText(series);
			ProcessTrendlineLabelText(series);
		}
		public void Visit(LineSeries series) {
			DefaultProcessDataReference(series);
			ProcessDataLabelsText(series);
			ProcessTrendlineLabelText(series);
		}
		public void Visit(PieSeries series) {
			DefaultProcessDataReference(series);
			ProcessDataLabelsText(series);
		}
		public void Visit(RadarSeries series) {
			DefaultProcessDataReference(series);
			ProcessDataLabelsText(series);
		}
		public void Visit(ScatterSeries series) {
			DefaultProcessDataReference(series);
			ProcessDataLabelsText(series);
			ProcessTrendlineLabelText(series);
		}
		public void Visit(SurfaceSeries series) {
			DefaultProcessDataReference(series);
		}
		#endregion
	}
	#endregion
	#region ChartAxisHelper
	static class ChartAxisHelper {
		public static void CheckArgumentAxis(Chart chart) {
			IChartView view = GetView(chart, chart.PrimaryAxes);
			CheckArgumentAxis(view, GetArguments(view));
			view = GetView(chart, chart.SecondaryAxes);
			CheckArgumentAxis(view, GetArguments(view));
		}
		public static void CheckArgumentAxis(IChartView view, IDataReference arguments) {
			if (view == null || view.Axes == null || view.Axes.Count == 0)
				return;
			ChartDataReference dataRef = arguments as ChartDataReference;
			if (dataRef == null)
				return;
			dataRef.DetectValueType();
			AxisBase argumentAxis = view.Axes[0];
			CategoryAxis categoryAxis = argumentAxis as CategoryAxis;
			DateAxis dateAxis = argumentAxis as DateAxis;
			if (categoryAxis != null && dataRef.ValueType == DataReferenceValueType.DateTime) {
				dateAxis = new DateAxis(view.Parent);
				dateAxis.CopyFrom(argumentAxis);
				dateAxis.NumberFormat.NumberFormatCode = dataRef.GetNumberFormatString();
				dateAxis.NumberFormat.SourceLinked = true;
				ReplaceAxis(view.Axes, dateAxis);
			}
			else if (dateAxis != null && dataRef.ValueType != DataReferenceValueType.DateTime) {
				categoryAxis = new CategoryAxis(view.Parent);
				categoryAxis.CopyFrom(argumentAxis);
				ReplaceAxis(view.Axes, categoryAxis);
			}
		}
		static void ReplaceAxis(AxisGroup axes, AxisBase newAxis) {
			axes.RemoveAt(0);
			axes.Insert(0, newAxis);
			axes[0].CrossesAxis = axes[1];
			axes[1].CrossesAxis = axes[0];
		}
		static IChartView GetView(Chart chart, AxisGroup axes) {
			foreach (IChartView view in chart.Views) {
				if (object.ReferenceEquals(view.Axes, axes))
					return view;
			}
			return null;
		}
		static IDataReference GetArguments(IChartView view) {
			if (view == null || view.Series.Count == 0)
				return null;
			return view.Series[0].Arguments;
		}
		static IDataReference GetValues(IChartView view) {
			if (view == null || view.Series.Count == 0)
				return null;
			return view.Series[0].Values;
		}
		public static bool UpdateSourceLinkedFormats(Chart chart) {
			IChartView view = GetView(chart, chart.PrimaryAxes);
			bool result = UpdateAxisNumberFormat(view, 0, GetArguments(view) as ChartDataReference);
			result |= UpdateAxisNumberFormat(view, 1, GetValues(view) as ChartDataReference);
			view = GetView(chart, chart.SecondaryAxes);
			result |= UpdateAxisNumberFormat(view, 0, GetArguments(view) as ChartDataReference);
			result |= UpdateAxisNumberFormat(view, 1, GetValues(view) as ChartDataReference);
			return result;
		}
		static bool UpdateAxisNumberFormat(IChartView view, int index, ChartDataReference dataRef) {
			if (view != null && view.Axes != null && (index < view.Axes.Count) && dataRef != null) {
				AxisBase axis = view.Axes[index];
				if (axis.NumberFormat.SourceLinked) {
					dataRef.ResetCachedValue();
					axis.NumberFormat.NumberFormatCode = dataRef.GetNumberFormatString();
					return true;
				}
			}
			return false;
		}
	}
	#endregion
}
