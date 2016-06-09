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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.Data.Export;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Utils.Zip;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraExport.Xls;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	#region XlsExportDisplayBlankAs
	enum XlsExportDisplayBlankAs {
		None = 0,
		Gap = 103,
		Span = 105
	}
	#endregion
	#region XlsBlockWritten
	[Flags]
	public enum XlsBlockWritten {
		None = 0x0000,
		AxisGroup = 0x0001,
		AttachedLabel = 0x0002,
		Axis = 0x0004,
		ChartGroup = 0x0008,
		DataTable = 0x0010,
		Frame = 0x0020,
		Legend = 0x0040,
		LegendException = 0x0080,
		Series = 0x0100,
		Chart = 0x0200,
		DataFormat = 0x0400,
		DropBar = 0x0800
	}
	#endregion
	public partial class XlsChartExporter : XlsSubstreamExporter, IAxisVisitor, IChartViewVisitor {
		#region Fields
		const short dataIndexArguments = 2;
		const short dataIndexValues = 1;
		const short dataIndexBubbleSize = 3;
		Chart chart;
		XlsCommandChart commandChart = new XlsCommandChart();
		XlsCommandChartFrame commandChartSpaceFrame = null;
		XlsCommandChartPropertiesBegin commandBegin = new XlsCommandChartPropertiesBegin();
		XlsCommandChartPropertiesEnd commandEnd = new XlsCommandChartPropertiesEnd();
		XlsCommandCrtLink commandCrtLink = new XlsCommandCrtLink();
		int axisIndex;
		int viewIndex;
		int seriesIndex;
		List<ISeries> seriesList = new List<ISeries>();
		Dictionary<IChartView, int> viewIndexTable = new Dictionary<IChartView, int>();
		MsoCrc32Compute crc32 = new MsoCrc32Compute();
		bool isPrimaryAxisGroup;
		XlsBlockWritten blockWritten = XlsBlockWritten.None;
#if WRITE_BLOCKS
		bool writeBlocks = true;
#else
		bool writeBlocks = false;
#endif
		#endregion
		public XlsChartExporter(BinaryWriter writer, Chart chart, ExportXlsStyleSheet exportStyleSheet) 
			: base(writer, chart.DocumentModel, exportStyleSheet) {
			this.chart = chart;
			IsEmbedded = true;
		}
		#region Properties
		protected internal Chart Chart { get { return chart; } }
		public bool IsEmbedded { get; set; }
		protected internal MsoCrc32Compute Crc32 { get { return crc32; } }
		#endregion
		#region IDisposable Members
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			this.chart = null;
		}
		#endregion
		public override void WriteContent() {
			Prepare();
			try {
				WriteBOF(XlsSubstreamType.Chart);
				WritePageHeader();
				WritePageFooter();
				WritePageHCenter();
				WritePageVCenter();
				WritePageLeftMargin();
				WritePageRightMargin();
				WritePageTopMargin();
				WritePageBottomMargin();
				WritePageSetup();
				WritePrintSize();
				WriteHeaderFooter();
				WriteFbi();
				WriteChartProtection();
				WriteChartUnits();
				WriteChart();
				WriteBegin();
				WriteChartFrtInfo();
				WriteFontLists();
				WriteViewScale();
				WritePlotGrowth();
				WriteChartSpaceFrame();
				WriteSeriesFormats();
				WriteChartSpaceProps();
				WriteAxesAndViews();
				WritePlotAreaLayout();
				WriteChartAttachedLabels();
				WriteChartExtProperties();
				WriteChartEndBlock();
				WriteEnd();
				WriteSeriesData();
				WriteChartSheetExtProperties();
				WriteEOF();
			}
			finally {
				Unprepare();
			}
		}
		#region PAGESETUP
		protected void WritePageHeader() {
			XlsCommandPageHeader command = new XlsCommandPageHeader();
			command.Value = Chart.PrintSettings.HeaderFooter.OddHeader;
			command.Write(StreamWriter);
		}
		protected void WritePageFooter() {
			XlsCommandPageFooter command = new XlsCommandPageFooter();
			command.Value = Chart.PrintSettings.HeaderFooter.OddFooter;
			command.Write(StreamWriter);
		}
		protected void WritePageHCenter() {
			XlsCommandPageHCenter command = new XlsCommandPageHCenter();
			command.Value = Chart.PrintSettings.PageSetup.CenterHorizontally;
			command.Write(StreamWriter);
		}
		protected void WritePageVCenter() {
			XlsCommandPageVCenter command = new XlsCommandPageVCenter();
			command.Value = Chart.PrintSettings.PageSetup.CenterVertically;
			command.Write(StreamWriter);
		}
		protected void WritePageLeftMargin() {
			XlsCommandPageLeftMargin command = new XlsCommandPageLeftMargin();
			command.Value = DocumentModel.UnitConverter.ModelUnitsToInchesF(Chart.PrintSettings.PageMargins.Left);
			command.Write(StreamWriter);
		}
		protected void WritePageRightMargin() {
			XlsCommandPageRightMargin command = new XlsCommandPageRightMargin();
			command.Value = DocumentModel.UnitConverter.ModelUnitsToInchesF(Chart.PrintSettings.PageMargins.Right);
			command.Write(StreamWriter);
		}
		protected void WritePageTopMargin() {
			XlsCommandPageTopMargin command = new XlsCommandPageTopMargin();
			command.Value = DocumentModel.UnitConverter.ModelUnitsToInchesF(Chart.PrintSettings.PageMargins.Top);
			command.Write(StreamWriter);
		}
		protected void WritePageBottomMargin() {
			XlsCommandPageBottomMargin command = new XlsCommandPageBottomMargin();
			command.Value = DocumentModel.UnitConverter.ModelUnitsToInchesF(Chart.PrintSettings.PageMargins.Bottom);
			command.Write(StreamWriter);
		}
		protected void WritePageSetup() {
			XlsCommandPageSetup command = new XlsCommandPageSetup();
			PrintSettings settings = Chart.PrintSettings;
			command.Properties.CopyFrom(settings.PageSetup.Info);
			command.HeaderMargin = DocumentModel.UnitConverter.ModelUnitsToInchesF(settings.PageMargins.Header);
			command.FooterMargin = DocumentModel.UnitConverter.ModelUnitsToInchesF(settings.PageMargins.Footer);
			command.Write(StreamWriter);
		}
		protected void WritePrintSize() {
			XlsCommandChartPrintSize command = new XlsCommandChartPrintSize();
			command.PrintSize = XlsChartPrintSize.DefinedByChart;
			command.Write(StreamWriter);
		}
		protected void WriteHeaderFooter() {
			XlsCommandHeaderFooter command = new XlsCommandHeaderFooter();
			HeaderFooterOptions options = Chart.PrintSettings.HeaderFooter;
			command.AlignWithMargins = options.AlignWithMargins;
			command.DifferentFirst = options.DifferentFirst;
			command.DifferentOddEven = options.DifferentOddEven;
			command.ScaleWithDoc = options.ScaleWithDoc;
			if (options.DifferentOddEven) {
				command.EvenHeader = options.EvenHeader;
				command.EvenFooter = options.EvenFooter;
			}
			if (options.DifferentFirst) {
				command.FirstHeader = options.FirstHeader;
				command.FirstFooter = options.FirstFooter;
			}
			command.Write(StreamWriter);
		}
		void WriteFbi() {
			WriteFbi(ExportStyleSheet.YMultFontList);
			WriteFbi(ExportStyleSheet.DataLabExtFontList);
		}
		void WriteFbi(List<RunFontInfo> fontList) {
			int count = fontList.Count;
			for (int i = 0; i < count; i++) {
				XlsCommandChartFbi command = PrepareFbi(fontList[i]);
				command.Write(StreamWriter);
			}
		}
		#endregion
		#region PROTECTION
		protected void WriteChartProtection() {
			XlsCommandProtected command = new XlsCommandProtected();
			command.Value = Chart.Protection != ChartSpaceProtection.None;
			command.Write(StreamWriter);
		}
		#endregion
		#region Units
		protected void WriteChartUnits() {
			XlsCommandChartUnits command = new XlsCommandChartUnits();
			command.Write(StreamWriter);
		}
		#endregion
		#region Begin/End/StartObject/EndObject/FrtWrapper
		protected void WriteBegin() {
			commandBegin.Write(StreamWriter);
		}
		protected void WriteEnd() {
			commandEnd.Write(StreamWriter);
		}
		void WriteStartObject(XlsChartObjectKind objectKind) {
			XlsCommandChartStartObject command = new XlsCommandChartStartObject();
			command.ObjectKind = objectKind;
			command.Write(StreamWriter);
		}
		void WriteStartObject(int objectInstance) {
			XlsCommandChartStartObject command = new XlsCommandChartStartObject();
			command.ObjectKind = XlsChartObjectKind.FontCache;
			command.ObjectInstance = objectInstance;
			command.Write(StreamWriter);
		}
		void WriteEndObject(XlsChartObjectKind objectKind) {
			XlsCommandChartEndObject command = new XlsCommandChartEndObject();
			command.ObjectKind = objectKind;
			command.Write(StreamWriter);
		}
		void WriteFrtWrapper(XlsCommandBase innerCommand) {
			if (innerCommand == null)
				return;
			XlsCommandFrtWrapper command = new XlsCommandFrtWrapper();
			command.InnerCommand = innerCommand;
			command.Write(StreamWriter);
		}
		#endregion
		#region StartBlock/EndBlock
		bool IsBlockWritten(XlsBlockWritten kind) {
			return (blockWritten & kind) == kind;
		}
		void SetBlockWritten(XlsBlockWritten kind, bool flag) {
			if (flag)
				blockWritten |= kind;
			else
				blockWritten &= ~kind;
		}
		protected void WriteStartBlock(XlsChartBlockObjectKind objectKind, int objectContext, int objectInstance1, int objectInstance2) {
			if (!writeBlocks)
				return;
			XlsCommandChartStartBlock command = new XlsCommandChartStartBlock();
			command.ObjectKind = objectKind;
			command.ObjectContext = objectContext;
			command.ObjectInstance1 = objectInstance1;
			command.ObjectInstance2 = objectInstance2;
			command.Write(StreamWriter);
		}
		protected void WriteEndBlock(XlsChartBlockObjectKind objectKind) {
			if (!writeBlocks)
				return;
			XlsCommandChartEndBlock command = new XlsCommandChartEndBlock();
			command.ObjectKind = objectKind;
			command.Write(StreamWriter);
		}
		#region Chart
		protected void WriteChartStartBlock() {
			if (!IsBlockWritten(XlsBlockWritten.Chart)) {
				SetBlockWritten(XlsBlockWritten.Chart, true);
				WriteStartBlock(XlsChartBlockObjectKind.Chart, 0, 0, 0);
			}
		}
		protected void WriteChartEndBlock() {
			if (IsBlockWritten(XlsBlockWritten.Chart)) {
				SetBlockWritten(XlsBlockWritten.Chart, false);
				WriteEndBlock(XlsChartBlockObjectKind.Chart);
			}
		}
		#endregion
		#region AxisGroup
		protected void WriteAxisGroupStartBlock() {
			if (!IsBlockWritten(XlsBlockWritten.AxisGroup)) {
				SetBlockWritten(XlsBlockWritten.AxisGroup, true);
				WriteStartBlock(XlsChartBlockObjectKind.AxisGroup, 0, isPrimaryAxisGroup ? 0 : 1, 0);
			}
		}
		protected void WriteAxisGroupEndBlock() {
			if (IsBlockWritten(XlsBlockWritten.AxisGroup)) {
				SetBlockWritten(XlsBlockWritten.AxisGroup, false);
				WriteEndBlock(XlsChartBlockObjectKind.AxisGroup);
			}
		}
		#endregion
		#region Axis
		protected void WriteAxisStartBlock(AxisBase axis) {
			if (!IsBlockWritten(XlsBlockWritten.Axis)) {
				SetBlockWritten(XlsBlockWritten.Axis, true);
				WriteAxisGroupStartBlock();
				WriteStartBlock(XlsChartBlockObjectKind.Axis, 0, GetAxisObjectInstance(axis), 0);
			}
		}
		protected void WriteAxisEndBlock() {
			if (IsBlockWritten(XlsBlockWritten.Axis)) {
				SetBlockWritten(XlsBlockWritten.Axis, false);
				WriteEndBlock(XlsChartBlockObjectKind.Axis);
			}
		}
		int GetAxisObjectInstance(AxisBase axis) {
			if (axis is SeriesAxis)
				return XlsChartDefs.SeriesAxisInstance;
			if (axis is ValueAxis)
				return axisIndex == 0 ? XlsChartDefs.XAxisInstance : XlsChartDefs.ValueAxisInstance;
			return XlsChartDefs.CategoryAxisInstance;
		}
		#endregion
		#region Series
		protected void WriteSeriesStartBlock() {
			if (!IsBlockWritten(XlsBlockWritten.Series)) {
				SetBlockWritten(XlsBlockWritten.Series, true);
				WriteStartBlock(XlsChartBlockObjectKind.Series, 0, seriesIndex, 0);
			}
		}
		protected void WriteSeriesEndBlock() {
			if (IsBlockWritten(XlsBlockWritten.Series)) {
				SetBlockWritten(XlsBlockWritten.Series, false);
				WriteEndBlock(XlsChartBlockObjectKind.Series);
			}
		}
		#endregion
		#region DataFormat
		protected void WriteDataFormatStartBlock(int pointIndex) {
			if (!IsBlockWritten(XlsBlockWritten.DataFormat)) {
				SetBlockWritten(XlsBlockWritten.DataFormat, true);
				WriteSeriesStartBlock();
				WriteStartBlock(XlsChartBlockObjectKind.DataFormat, seriesIndex, pointIndex, 0);
			}
		}
		protected void WriteDataFormatEndBlock() {
			if (IsBlockWritten(XlsBlockWritten.DataFormat)) {
				SetBlockWritten(XlsBlockWritten.DataFormat, false);
				WriteEndBlock(XlsChartBlockObjectKind.DataFormat);
			}
		}
		#endregion
		#region ChartGroup
		protected void WriteChartGroupStartBlock() {
			if (!IsBlockWritten(XlsBlockWritten.ChartGroup)) {
				SetBlockWritten(XlsBlockWritten.ChartGroup, true);
				WriteStartBlock(XlsChartBlockObjectKind.ChartGroup, 0, isPrimaryAxisGroup ? 0 : 1, 0);
			}
		}
		protected void WriteChartGroupEndBlock() {
			if (IsBlockWritten(XlsBlockWritten.ChartGroup)) {
				SetBlockWritten(XlsBlockWritten.ChartGroup, false);
				WriteEndBlock(XlsChartBlockObjectKind.ChartGroup);
			}
		}
		#endregion
		#region Legend
		protected void WriteLegendStartBlock(int objectContext) {
			if (!IsBlockWritten(XlsBlockWritten.Legend)) {
				SetBlockWritten(XlsBlockWritten.Legend, true);
				WriteChartGroupStartBlock();
				WriteStartBlock(XlsChartBlockObjectKind.Legend, objectContext, 0, 0);
			}
		}
		protected void WriteLegendEndBlock() {
			if (IsBlockWritten(XlsBlockWritten.Legend)) {
				SetBlockWritten(XlsBlockWritten.Legend, false);
				WriteEndBlock(XlsChartBlockObjectKind.Legend);
			}
		}
		#endregion
		#endregion
		#region Chart
		protected void WriteChart() {
			commandChart.Left = 0.0;
			commandChart.Top = 0.0;
			commandChart.Width = DocumentModel.UnitConverter.ModelUnitsToPointsF(chart.Width);
			commandChart.Height = DocumentModel.UnitConverter.ModelUnitsToPointsF(chart.Height);
			commandChart.Write(StreamWriter);
		}
		void WriteChartFrtInfo() {
			XlsCommandChartFrtInfo command = new XlsCommandChartFrtInfo();
			command.Originator = XlsApplicationVersion.Excel2002OfficeExcel2003;
			command.Writer = XlsApplicationVersion.Excel2002OfficeExcel2003;
			command.Write(StreamWriter);
		}
		#region WriteFontLists
		void WriteFontLists() {
			WriteFrtFontList(ExportStyleSheet.YMultFontList, 9, XlsFrtFontListType.YMult);
			WriteFrtFontList(ExportStyleSheet.DataLabExtFontList, 10, XlsFrtFontListType.DataLabExt);
		}
		void WriteFrtFontList(List<RunFontInfo> fontList, int version, XlsFrtFontListType type) {
			int fontCount = fontList.Count;
			if (fontCount < 1)
				return;
			WriteFrtFontList(fontList, type);
			WriteStartObject(version);
			for (int i = 0; i < fontCount; i++) {
				RunFontInfo fontInfo = fontList[i];
				WriteFrtWrapper(PrepareFont(fontInfo));
				WriteFrtWrapper(PrepareFbi(fontInfo));
			}
			WriteEndObject(XlsChartObjectKind.FontCache);
		}
		void WriteFrtFontList(List<RunFontInfo> fontList, XlsFrtFontListType type) {
			XlsCommandChartFrtFontList command = new XlsCommandChartFrtFontList();
			command.FontListType = type;
			int fontCount = fontList.Count;
			for (int i = 0; i < fontCount; i++) {
				XlsChartFrtFontInfo frtFontInfo = new XlsChartFrtFontInfo();
				frtFontInfo.Scaled = true;
				frtFontInfo.FontIndex = GetFontIndex(fontList[i]);
				command.FrtFonts.Add(frtFontInfo);
			}
			command.Write(StreamWriter);
		}
		XlsCommandFont PrepareFont(RunFontInfo fontInfo) {
			XlsCommandFont command = new XlsCommandFont();
			command.SetFontInfo(fontInfo);
			return command;
		}
		XlsCommandChartFbi PrepareFbi(RunFontInfo fontInfo) {
			XlsCommandChartFbi command = new XlsCommandChartFbi();
			command.HeightBasis = (int)(fontInfo.Size * 20);
			command.Scaling = true;
			command.FontIndex = GetFontIndex(fontInfo);
			return command;
		}
		#endregion
		protected void WriteViewScale() {
			XlsCommandSheetViewScale command = new XlsCommandSheetViewScale();
			command.Numerator = 1;
			command.Denominator = 1;
			command.Write(StreamWriter);
		}
		protected void WritePlotGrowth() {
			XlsCommandChartPlotGrowth command = new XlsCommandChartPlotGrowth();
			command.Horizontal = 1.0;
			command.Vertical = 1.0;
			command.Write(StreamWriter);
		}
		protected void WriteChartSpaceFrame() {
			this.commandChartSpaceFrame = WriteFrame(false, true);
			WriteBegin();
			ResetCrc32();
			WriteLineFormat(Chart.ShapeProperties, ChartElement.ChartArea);
			WriteAreaFormat(Chart.ShapeProperties, ChartElement.ChartArea);
			WriteChartStartBlock();
			WriteStartBlock(XlsChartBlockObjectKind.Frame, XlsChartDefs.ChartAreaFrameContext, 0, 0);
			WriteShapeFormat(Chart.ShapeProperties, XlsChartDefs.DataContext);
			WriteEndBlock(XlsChartBlockObjectKind.Frame);
			WriteEnd();
		}
		protected void WriteChartSpaceProps() {
			XlsCommandChartSpaceProperties command = new XlsCommandChartSpaceProperties();
			command.ManualSeriesAllocation = true;
			command.ManualPlotArea = true;
			command.AlwaysAutoPlotArea = true;
			command.DispBlankAs = chart.DispBlanksAs;
			command.NotSizeWith = false;
			command.PlotVisibleOnly = chart.PlotVisibleOnly;
			command.Write(StreamWriter);
		}
		protected void WriteSeriesFormats() {
			for (seriesIndex = 0; seriesIndex < seriesList.Count; seriesIndex++) {
				ISeries series = seriesList[seriesIndex];
				viewIndex = viewIndexTable[series.View];
				WriteSeries(series);
				WriteBegin();
				WriteSeriesText(series);
				WriteSeriesDataRef(series.Values, XlsChartDataRefId.Values);
				WriteSeriesDataRef(series.Arguments, XlsChartDataRefId.Arguments);
				IDataReference bubbleSize = null;
				BubbleSeries bubbleSeries = series as BubbleSeries;
				if (bubbleSeries != null)
					bubbleSize = bubbleSeries.BubbleSize;
				WriteSeriesDataRef(bubbleSize, XlsChartDataRefId.BubbleSize);
				WriteSeriesDataFormat(series as SeriesBase);
				WriteDataPoints(series as SeriesWithDataLabelsAndPoints);
				WriteSeriesToView();
				WriteLegendException(series, seriesIndex);
				WriteSeriesEndBlock();
				WriteEnd();
			}
		}
		protected void WriteAxesAndViews() {
			bool hasSecondaryAxes = chart.HasSecondaryAxisGroup;
			WriteAxesUsed(hasSecondaryAxes);
			WriteAxisGroup(chart.PrimaryAxes);
			if (hasSecondaryAxes)
				WriteAxisGroup(chart.SecondaryAxes);
		}
		#endregion
		#region PlotArea
		protected void WritePlotArea() {
			XlsCommandChartPlotArea command = new XlsCommandChartPlotArea();
			command.Write(StreamWriter);
		}
		protected void WritePlotAreaFrame() {
			PlotArea plotArea = Chart.PlotArea;
			WriteFrame(plotArea.Layout);
			WriteBegin();
			ResetCrc32();
			ChartElement element = Chart.Is3DChart ? ChartElement.PlotArea3D : ChartElement.PlotArea2D;
			WriteLineFormat(plotArea.ShapeProperties, element);
			WriteAreaFormat(plotArea.ShapeProperties, element);
			WriteStartBlock(XlsChartBlockObjectKind.Frame, XlsChartDefs.PlotAreaFrameContext, 0, 0);
			WriteShapeFormat(plotArea.ShapeProperties, XlsChartDefs.DataContext);
			WriteEndBlock(XlsChartBlockObjectKind.Frame);
			WriteEnd();
		}
		protected void WritePlotAreaLayout() {
			LayoutOptions layout = Chart.PlotArea.Layout;
			XlsCommandChartLayout12A command = new XlsCommandChartLayout12A();
			command.CheckSum = 1;
			command.Target = layout.Target;
			command.Left = GetOffsetInSPRC(layout.Left.Mode, layout.Left.Value, 0.0);
			command.Top = GetOffsetInSPRC(layout.Top.Mode, layout.Top.Value, 0.0);
			command.Right = GetOffsetInSPRC(layout.Width.Mode, layout.Width.Value, 1.0);
			command.Bottom = GetOffsetInSPRC(layout.Height.Mode, layout.Height.Value, 1.0);
			command.X = layout.Left.Value;
			command.XMode = layout.Left.Mode;
			command.Y = layout.Top.Value;
			command.YMode = layout.Top.Mode;
			command.Width = layout.Width.Value;
			command.WidthMode = layout.Width.Mode;
			command.Height = layout.Height.Value;
			command.HeightMode = layout.Height.Mode;
			command.Write(StreamWriter);
		}
		int GetOffsetInSPRC(LayoutMode mode, double value, double defaultValue) {
			return (int)(((mode != LayoutMode.Auto) ? value : defaultValue) * 4000);
		}
		#endregion
		#region Prepare/unprepare
		void Prepare() {
			Unprepare();
			bool hasSecondaryAxes = chart.HasSecondaryAxisGroup;
			RegisterAxisGroup(chart.PrimaryAxes);
			if (hasSecondaryAxes)
				RegisterAxisGroup(chart.SecondaryAxes);
			seriesList = chart.GetSeriesList();
		}
		void Unprepare() {
			viewIndexTable.Clear();
			seriesList.Clear();
		}
		void RegisterAxisGroup(AxisGroup axisGroup) {
			for (int i = 0; i < chart.Views.Count; i++) {
				IChartView view = chart.Views[i];
				if (view.Axes == axisGroup)
					viewIndexTable.Add(view, viewIndexTable.Count);
			}
		}
		#endregion
		#region CrtMlFrt
		protected void WriteExtProperties(XlsChartExtProperties extProperties) {
			if (extProperties.Items.Count == 0)
				return;
			XlsCommandChartML firstCommand = new XlsCommandChartML();
			XlsCommandChartMLContinue continueCommand = new XlsCommandChartMLContinue();
			using (XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, firstCommand, continueCommand)) {
				FutureRecordHeader header = new FutureRecordHeader();
				header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(firstCommand.GetType());
				header.Write(writer);
				extProperties.Write(writer);
			}
		}
		void WriteAttachedLabelExtProperties(bool overlay) {
			XlsChartExtProperties extProperties = new XlsChartExtProperties();
			extProperties.Parent = XlsChartExtPropParent.AttachedLabel;
			extProperties.Items.Add(new XlsChartExtPropOverlay() { Value = overlay });
			WriteExtProperties(extProperties);
		}
		void WriteChartExtProperties() {
			XlsChartExtProperties extProperties = new XlsChartExtProperties();
			extProperties.Parent = XlsChartExtPropParent.ChartSpace;
			extProperties.Items.Add(new XlsChartExtPropShowDLblsOverMax() { Value = chart.ShowDataLabelsOverMax });
			XlsExportDisplayBlankAs dispBlankAsValue = CalculateDispBlankAsProperty(chart.Views, chart.DispBlanksAs);
			if (dispBlankAsValue != XlsExportDisplayBlankAs.None)
				extProperties.Items.Add(new XlsChartExtPropDispBlankAs() { Value = (int)dispBlankAsValue });
			WriteExtProperties(extProperties);
		}
		void WriteChartSheetExtProperties() {
			XlsChartExtProperties extProperties = new XlsChartExtProperties();
			extProperties.Parent = XlsChartExtPropParent.Chart;
			extProperties.Items.Add(new XlsChartExtPropCultureCode() { Value = chart.Culture.Name });
			WriteExtProperties(extProperties);
			extProperties.Items.Clear();
			int styleId = chart.Style;
			if (styleId != 2)
				extProperties.Items.Add(new XlsChartExtPropStyle() { Value = styleId });
			WriteExtProperties(extProperties);
		}
		XlsExportDisplayBlankAs CalculateDispBlankAsProperty(ChartViewCollection viewCollection, DisplayBlanksAs dispBlankAs) {
			foreach (IChartView view in viewCollection) {
				ChartViewType type = view.ViewType;
				bool isStackedLineOrArea = IsAreaChartGroup(type) || IsStackedLineChartView(view);
				if (isStackedLineOrArea && dispBlankAs == DisplayBlanksAs.Span)
					return XlsExportDisplayBlankAs.Span;
				if ((IsPieChartGroup(type) || isStackedLineOrArea) && dispBlankAs == DisplayBlanksAs.Gap)
					return XlsExportDisplayBlankAs.Gap;
			}
			return XlsExportDisplayBlankAs.None;
		}
		bool IsStackedLineChartView(IChartView view) {
			LineChartView lineChartView = view as LineChartView;
			if (lineChartView != null) {
				ChartGrouping grouping = lineChartView.Grouping;
				return grouping == ChartGrouping.Stacked || grouping == ChartGrouping.PercentStacked;
			}
			return false;
		}
		#endregion
		#region CrtLayout12
		void WriteCrtLayout12(LayoutOptions layout) {
			XlsCommandChartLayout12 command = new XlsCommandChartLayout12();
			command.CheckSum = layout.Auto ? 0 : 1;
			command.XMode = layout.Left.Mode;
			command.X = layout.Left.Value;
			command.YMode = layout.Top.Mode;
			command.Y = layout.Top.Value;
			command.WidthMode = layout.Width.Mode;
			command.Width = layout.Width.Value;
			command.HeightMode = layout.Height.Mode;
			command.Height = layout.Height.Value;
			command.Write(StreamWriter);
		}
		#endregion
		void ResetCrc32() {
			crc32.CrcValue = 0;
		}
	}
}
