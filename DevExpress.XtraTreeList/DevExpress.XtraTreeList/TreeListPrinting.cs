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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.Export;
using DevExpress.Export.Xl;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraPrinting;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Helpers;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.StyleFormatConditions;
using DevExpress.XtraTreeList.ViewInfo;
using System.Linq;
namespace DevExpress.XtraTreeList.Printing {
	public class PrintStyleId {
		public enum PrintStyle { HeaderPanel, BandPanel, Row, Lines, Preview, FooterPanel, GroupFooter, EvenRow, OddRow, Caption }
		public static int HeaderPanel { get { return (int)PrintStyle.HeaderPanel; } }
		public static int BandPanel { get { return (int)PrintStyle.BandPanel; } }
		public static int Row { get { return (int)PrintStyle.Row; } }
		public static int Lines { get { return (int)PrintStyle.Lines; } }
		public static int Preview { get { return (int)PrintStyle.Preview; } }
		public static int FooterPanel { get { return (int)PrintStyle.FooterPanel; } }
		public static int GroupFooter { get { return (int)PrintStyle.GroupFooter; } }
		public static int EvenRow { get { return (int)PrintStyle.EvenRow; } }
		public static int OddRow { get { return (int)PrintStyle.OddRow; } }
		public static int Caption { get { return (int)PrintStyle.Caption; } }
		public static string[] GetStyleNames() { return Enum.GetNames(typeof(PrintStyle)); }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class XtraTreeListPrinter : ComponentPrinterDynamic {
		public XtraTreeListPrinter(IPrintable component) : base(component) { }
		public XtraTreeListPrinter(IPrintable component, PrintingSystemBase printingSystem) : base(component, printingSystem) { }
		protected TreeList TreeList { get { return Component as TreeList; } }
		public override void Export(ExportTarget target, Stream stream, ExportOptionsBase options) {
			ExportCore(target, stream, options, (exporter, fp) => { exporter.Export((Stream)fp); }, (targ, fp, opt) => { base.Export(targ, (Stream)fp, opt); });
		}
		public override void Export(ExportTarget target, string filePath, ExportOptionsBase options) {
			ExportCore(target, filePath, options, (exporter, fp) => { exporter.Export((string)fp); }, (targ, fp, opt) => { base.Export(targ, (string)fp, opt); });
		}
		public override void Export(ExportTarget target, string filePath) {
			ExportCore(target, filePath, (exporter, fp) => { exporter.Export((string)fp); }, (targ, fp) => { base.Export(targ, (string)fp); });
		}
		public override void Export(ExportTarget target, Stream stream) {
			ExportCore(target, stream, (exporter, fp) => { exporter.Export((Stream)fp); }, (targ, fp) => { base.Export(targ, (Stream)fp); });
		}
		protected virtual IDataAwareExportOptions InitExporterOptions(ExportTarget target, ExportOptionsBase options) {
			IDataAwareExportOptions result = DataAwareExportOptionsFactory.Create(target, options as IDataAwareExportOptions);
			result.AllowGrouping = DefaultBoolean.True;
			result.AllowCellMerge = DefaultBoolean.False;
			TreeList.OptionsPrint.PrintReportFooter = true;
			result.ShowTotalSummaries = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.ShowTotalSummaries, TreeList.OptionsPrint.PrintReportFooter);
			result.ShowGroupSummaries = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.ShowGroupSummaries, TreeList.OptionsPrint.PrintRowFooterSummary);
			result.ShowColumnHeaders = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.ShowColumnHeaders, TreeList.OptionsPrint.PrintPageHeader);
			result.AllowHorzLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.AllowHorzLines, TreeList.OptionsPrint.PrintHorzLines);
			result.AllowVertLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.AllowVertLines, TreeList.OptionsPrint.PrintVertLines);
			XlsExportOptionsBase xlsEO = options as XlsExportOptionsBase;
			result.AllowHyperLinks = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.AllowHyperLinks, xlsEO != null ? xlsEO.ExportHyperlinks : true);
			result.SheetName = xlsEO != null ? xlsEO.SheetName : TreeList.Caption;
			CsvExportOptions csvEO = options as CsvExportOptions;
			result.CSVEncoding = csvEO == null ? null : csvEO.Encoding;
			result.CSVSeparator = csvEO == null ? null : csvEO.Separator;
			result.ExportTarget = target;
			result.InitDefaults();
			return result;
		}
		void ExportCore(ExportTarget target, object filePath, Action2<GridViewExcelExporter<TreeListColumnImplementer, TreeListNodeImplementer>, object> action, Action2<ExportTarget, object> baseAction) {
			if(!DoExportCore(target, filePath, null, action)) baseAction(target, filePath);
		}
		void ExportCore(ExportTarget target, object filePath, ExportOptionsBase options, Action2<GridViewExcelExporter<TreeListColumnImplementer, TreeListNodeImplementer>, object> action, Action3<ExportTarget, object, ExportOptionsBase> baseAction) {
			if(!DoExportCore(target, filePath, options, action)) baseAction(target, filePath, options);
		}
		protected virtual bool DoExportCore(ExportTarget target, object filePath, ExportOptionsBase options, Action2<GridViewExcelExporter<TreeListColumnImplementer, TreeListNodeImplementer>, object> action) {
			if(DevExpress.XtraExport.ExportUtils.AllowNewExcelExportEx(options as IDataAwareExportOptions, target) && TreeList != null) {
				IDataAwareExportOptions exporterOptions = null;
				try {
					((IPrintableEx)TreeList).OnStartActivity();
					exporterOptions = InitExporterOptions(target, options);
					exporterOptions.ExportProgress += OnExportProgress;
					var exporter = new TreeListExcelExporter<TreeListColumnImplementer, TreeListNodeImplementer>(new TreeListImplementer(TreeList, target), exporterOptions);
					action(exporter, filePath);
				}
				finally {
					((IPrintableEx)TreeList).OnEndActivity();
					exporterOptions.ExportProgress -= OnExportProgress;
				}
				return true;
			}
			return false;
		}
		void OnExportProgress(ProgressChangedEventArgs ea) {
			if(TreeList == null) return;
			TreeList.OnPrintProgress(ea.ProgressPercentage);
		}
	}
	public class TreeListPrinter : IDisposable {
		IPrintingSystem ps;
		ILink link;
		TreeListViewInfo viewInfo;
		TreeList treeList;
		TreeListPrintInfo printInfo;
		TreeListPrintAppearanceCollection appearance;
		UserControl printControl;
		internal IBrickGraphics graph;
		internal TreePainter treePainter;
		internal bool IsPrinting;
		public static readonly string[] DefaultStyleNames = PrintStyleId.GetStyleNames();
		static AppearanceDefaultInfo[] defaultPrintStyles = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo(DefaultStyleNames[PrintStyleId.HeaderPanel], new AppearanceDefault(Color.Black, Color.LightGray, Color.DarkGray, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo(DefaultStyleNames[PrintStyleId.BandPanel], new AppearanceDefault(Color.Black, Color.LightGray, Color.DarkGray, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo(DefaultStyleNames[PrintStyleId.Row], new AppearanceDefault(Color.Black, Color.White, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo(DefaultStyleNames[PrintStyleId.Lines], new AppearanceDefault(Color.DarkGray, Color.DarkGray, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo(DefaultStyleNames[PrintStyleId.Preview], new AppearanceDefault(Color.DimGray, Color.White, HorzAlignment.Near, VertAlignment.Top)),
			new AppearanceDefaultInfo(DefaultStyleNames[PrintStyleId.FooterPanel], new AppearanceDefault(Color.Black, Color.DarkGray, HorzAlignment.Far, VertAlignment.Center)),
			new AppearanceDefaultInfo(DefaultStyleNames[PrintStyleId.GroupFooter], new AppearanceDefault(Color.Black, Color.LightGray, HorzAlignment.Far, VertAlignment.Center)),
			new AppearanceDefaultInfo(DefaultStyleNames[PrintStyleId.EvenRow], new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default)),
			new AppearanceDefaultInfo(DefaultStyleNames[PrintStyleId.OddRow], new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default)),
			new AppearanceDefaultInfo(DefaultStyleNames[PrintStyleId.Caption], new AppearanceDefault(Color.Black, Color.FromArgb(230,230,230), Color.DarkGray, Color.Empty, HorzAlignment.Center, VertAlignment.Center))
	};
		public static AppearanceDefaultInfo[] GetPrintAppearanceDefaults() {
			return defaultPrintStyles;
		}
		public TreeListPrinter(TreeList treeList) {
			this.ps = null;
			this.link = null;
			this.graph = null;
			this.viewInfo = null;
			this.treePainter = new TreePainter();
			this.treeList = treeList;
			this.appearance = new TreeListPrintAppearanceCollection(treeList);
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.IsPrinting = false;
			this.printInfo = new TreeListPrintInfo(this);
			this.printControl = null;
		}
		public List<PrintBandInfo> BandPrintInfo { get; private set; }
		public List<PrintColumnInfo> ColumnPrintInfo { get; private set; }
		protected int BandsRowCount { get { return viewInfo.GetBandPanelRowCount(); } }
		protected int ColumnRowCount { get { return viewInfo.GetColumnPanelRowCount(); } }
		protected PrintingSystemBase PrintingSystemBase { get { return (PrintingSystemBase)ps; } }
		IDisposable printerCore;
		public ComponentPrinterBase PrinterCore {
			get {
				if(printerCore == null)
					printerCore = CreateComponentPrinter();
				return (ComponentPrinterBase)printerCore;
			}
		}
		public PrintingSystemActivity PrintingActivity { get { return PrinterCore == null ? PrintingSystemActivity.Idle : PrinterCore.Activity; } }
		protected internal bool IsDocumentCreating { get { return PrintingSystemBase != null && PrintingSystemBase.Document != null && PrintingSystemBase.Document.IsCreating; } }
		protected internal bool CancelPending { get { return (PrintingSystemBase != null && PrintingSystemBase.CancelPending) || (TreeList.ProgressWindow != null && TreeList.ProgressWindow.CancelPending); } }
		public void CancelPrint() {
			if(PrintingSystemBase != null) PrintingSystemBase.Cancel();
		}
		int lastProgress = -1;
		protected internal virtual void OnPrintingProgress(int progress) {
			if((PrintingActivity & (PrintingSystemActivity.Printing | PrintingSystemActivity.Exporting)) != 0)
				progress = (progress / 2);
			if(progress == lastProgress) return;
			this.lastProgress = progress;
			TreeList.OnPrintProgress(progress);
		}
		protected internal void OnPrintingSystemProgress(int progress) {
			if(IsDocumentCreating) return;
			int currentProgress = progress;
			if((PrintingActivity & PrintingSystemActivity.Preparing) != 0) {
				currentProgress = progress / 2;
				currentProgress += 50;
			}
			TreeList.OnPrintProgress(currentProgress);
		}
		protected virtual ComponentPrinterBase CreateComponentPrinter() {
			return new XtraTreeListPrinter(TreeList); 
		}
		protected internal IPrintingSystem PS { get { return this.ps; } }
		protected internal virtual void Initialize(IPrintingSystem ps, ILink link, TreeListViewInfo viewInfo) {
			if(this.ps != ps) {
				this.ps = ps;
				this.ps.AfterChange += new DevExpress.XtraPrinting.ChangeEventHandler(OnAfterChange);
			}
			this.link = link;
			this.viewInfo = viewInfo;
			this.viewInfo.CreateResources();
			this.treePainter.ButtonStyle = UsePrintStyles ? PrintInfo.PrintAppearance.Row : viewInfo.PaintAppearance.GroupButton;
			RecalcPrintInfo();
			CreateHeadersPrintInfo();
			ps.SetCommandVisibility(PrintingSystemCommand.ExportCsv, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportRtf, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportPdf, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportHtm, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportTxt, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportXls, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportMht, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportXlsx, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendCsv, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendRtf, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendPdf, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendTxt, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendXls, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendMht, true);
			ps.SetCommandVisibility(PrintingSystemCommand.SendXlsx, true);
		}
		protected internal virtual void Release() {
			if(this.ps != null) {
				this.ps.AfterChange -= new DevExpress.XtraPrinting.ChangeEventHandler(OnAfterChange);
				this.ps = null;
			}
			this.link = null;
		}
		public virtual void Dispose() {
			Release();
			if(this.printerCore != null) {
				this.printerCore.Dispose();
				this.printerCore = null;
			}
			appearance.Changed -= new EventHandler(OnAppearanceChanged);
		}
		protected internal virtual void CreateArea(string areaName, IBrickGraphics graph) {
			this.graph = graph;
			switch(areaName) {
				case DevExpress.XtraPrinting.SR.Detail:
					CreateDetails();
					break;
				case DevExpress.XtraPrinting.SR.DetailHeader:
					CreateDetailHeader();
					break;
				case DevExpress.XtraPrinting.SR.ReportHeader:
					CreateReportHeader();
					break;
				case DevExpress.XtraPrinting.SR.ReportFooter:
					CreateReportFooter();
					break;
			}
		}
		protected internal virtual void AcceptChanges() {
			if(printControl != null && printControl is DevExpress.XtraTreeList.Design.IPrintDesigner)
				((DevExpress.XtraTreeList.Design.IPrintDesigner)printControl).ApplyOptions(true);
		}
		protected internal virtual void RejectChanges() { }
		protected internal virtual void ShowHelp() { }
		protected internal virtual bool SupportsHelp() { return false; }
		protected internal virtual bool HasPropertyEditor() { return true; }
		protected internal virtual UserControl PropertyEditorControl {
			get {
				DevExpress.XtraEditors.Designer.Utils.XtraFrame ctrl = new DevExpress.XtraTreeList.Frames.TreeListPrinting();
				ctrl.InitFrame(TreeList, "Print Designer", null);
				if(ctrl is DevExpress.XtraTreeList.Design.IPrintDesigner) {
					ctrl.Size = ((DevExpress.XtraTreeList.Design.IPrintDesigner)ctrl).UserControlSize;
					((DevExpress.XtraTreeList.Design.IPrintDesigner)ctrl).AutoApply = false;
					((DevExpress.XtraTreeList.Design.IPrintDesigner)ctrl).HideCaption();
				}
				printControl = ctrl;
				return ctrl;
			}
		}
		void OnAfterChange(object sender, DevExpress.XtraPrinting.ChangeEventArgs e) {
			switch(e.EventName) {
				case DevExpress.XtraPrinting.SR.BrickClick:
					OnBrickClick(e);
					break;
				case DevExpress.XtraPrinting.SR.ProgressPositionChanged:
					OnPrintingSystemProgress(PrintingSystemBase.ProgressReflector.Position);
					break;
				case DevExpress.XtraPrinting.SR.AfterMarginsChange:
				case DevExpress.XtraPrinting.SR.PageSettingsChanged:
					OnPageSettingsChanged();
					break;
			}
		}
		void OnBrickClick(DevExpress.XtraPrinting.ChangeEventArgs e) {
			if(!TreeList.OptionsPrint.PrintTreeButtons || TreeList.OptionsPrint.PrintAllNodes) return;
			Brick brick = e.ValueOf("Brick") as Brick;
			if(brick == null) return;
			try {
				float x = (float)e.ValueOf("X");
				float y = (float)e.ValueOf("Y");
				object val = brick.Value;
				if(val == null || !(val is IndentButtonInfo)) return;
				IndentButtonInfo info = val as IndentButtonInfo;
				if(info.ButtonRect.Contains(x, y)) OnClickButtonBrick(info.Node);
			}
			catch { }
		}
		protected virtual void OnClickButtonBrick(TreeListNode node) {
			node.Expanded = !node.Expanded;
			link.CreateDocument();
		}
		protected virtual void OnPageSettingsChanged() {
			if(TreeList.OptionsPrint.AutoWidth)
				link.CreateDocument();
		}
		void CreateHeadersPrintInfo() {
			BandPrintInfo = new List<Printing.PrintBandInfo>();
			ColumnPrintInfo = new List<Printing.PrintColumnInfo>();
			CalcHeadersPrintInfo();
		}
		internal Func<IHeaderObject, ICollection> GetBandRowsDelegate() {
			if(TreeList.AllowBandColumnsMultiRow) {
				return (headerObject) =>
				{
					TreeListBandRowCollection rows = TreeList.GetBandRowsCore(((PrintBandWrapper)headerObject).Band, (column) => { return column.Visible && column.OptionsColumn.Printable != DefaultBoolean.False; });
					List<PrintBandRow> printRows = new List<PrintBandRow>();
					foreach(TreeListBandRow row in rows) {
						PrintBandRow printRow = new PrintBandRow();
						printRows.Add(printRow);
						foreach(TreeListColumn column in row.Columns) {
							printRow.Columns.Add(new PrintColumnWrapper(column));
						}
					}
					return printRows;
				};
			}
			return null;
		}
		protected virtual void CalcHeadersPrintInfo() {
			BrickGraphics bg = ((PrintingSystemBase)ps).Graph;
			int pageWidth = Convert.ToInt32(bg.ClientPageSize.Width) - 1;
			HeaderWidthCalculator calculator = CreateHeaderWidthCalculator();
			if(TreeList.OptionsPrint.PrintBandHeader && TreeList.Bands.VisibleCount > 0) {
				List<PrintBandWrapper> bandWrappers = new List<PrintBandWrapper>();
				foreach(TreeListBand band in TreeList.Bands)
					bandWrappers.Add(new PrintBandWrapper(band));
				calculator.Calculate(TreeList.OptionsPrint.AutoWidth, pageWidth, 0, bandWrappers, GetBandRowsDelegate());
				CalcBandsPrintInfo(calculator.HeaderWidths, 0, 0);
			}
			else {
				List<PrintColumnWrapper> columnWrappers = new List<PrintColumnWrapper>();
				foreach(TreeListColumn column in TreeList.VisibleColumns)
					columnWrappers.Add(new PrintColumnWrapper(column));
				calculator.Calculate(TreeList.OptionsPrint.AutoWidth, pageWidth, 0, columnWrappers);
				CalcColumnsPrintInfo(calculator.HeaderWidths);
			}
		}
		protected virtual HeaderWidthCalculator CreateHeaderWidthCalculator() {
			return new HeaderWidthCalculator();
		}
		protected virtual void CalcColumnsPrintInfo(HeaderWidthInfoCollection collection) {
			int lastRight = 0;
			foreach(HeaderWidthInfo info in collection) {
				TreeListColumn column = ((PrintColumnWrapper)info.HeaderObject).Column;
				if(column == null) continue;
				PrintColumnInfo colInfo = new PrintColumnInfo(column);
				colInfo.Bounds = new Rectangle(lastRight, 0, info.Width, printInfo.ColumnPanelHeight);
				ColumnPrintInfo.Add(colInfo);
				lastRight = colInfo.Bounds.Right;
			}
		}
		protected virtual void CalcBandsPrintInfo(HeaderWidthInfoCollection collection, int rowIndex, int lastRight) {
			foreach(HeaderWidthInfo info in collection) {
				TreeListBand band = ((PrintBandWrapper)info.HeaderObject).Band;
				if(band != null && band.Visible)
					lastRight = CalcBandPrintInfo(info, lastRight, rowIndex);
			}
		}
		protected virtual int CalcBandPrintInfo(HeaderWidthInfo info, int lastRight, int rowIndex) {
			TreeListBand band = ((PrintBandWrapper)info.HeaderObject).Band;
			PrintBandInfo bi = new PrintBandInfo(band);
			int rowCount = band.RowCount;
			bool hasVisibleChildren = band.HasChildren && band.Bands.VisibleCount > 0;
			if(!hasVisibleChildren && band.AutoFill)
				rowCount = BandsRowCount - rowIndex;
			bi.Bounds = new Rectangle(lastRight, rowIndex * PrintInfo.BandPanelRowHeight, info.Width, rowCount * PrintInfo.BandPanelRowHeight);
			int lastBottom = bi.Bounds.Bottom;
			BandPrintInfo.Add(bi);
			if(hasVisibleChildren) {
				CalcBandsPrintInfo(info.Children, rowIndex + rowCount, lastRight);
			}
			else {
				if(!band.AutoFill && rowIndex + band.RowCount < BandsRowCount) {
					PrintBandInfo emptyBand = new PrintBandInfo(null);
					emptyBand.Bounds = new Rectangle(lastRight, bi.Bounds.Bottom, info.Width, (BandsRowCount - rowIndex - band.RowCount) * PrintInfo.BandPanelRowHeight);
					lastBottom = emptyBand.Bounds.Bottom;
					BandPrintInfo.Add(emptyBand);
				}
			}
			CalcBandColumnPrintInfo(bi, info, lastBottom);
			return bi.Bounds.Right;
		}
		protected virtual void CalcBandColumnPrintInfo(PrintBandInfo bandInfo, HeaderWidthInfo info, int bottom) {
			if(bandInfo.Band.HasChildren) return;
			CalcBandCoumnsPrintInfoCore(bandInfo, info, bottom);
		}
		protected virtual void CalcBandCoumnsPrintInfoCore(PrintBandInfo bandInfo, HeaderWidthInfo info, int bottom) {
			if(info.Children.Count > 0) {
				int lastRight = bandInfo.Bounds.X;
				int lastBottom = bottom;
				foreach(HeaderWidthInfo hi in info.Children) {
					MultiRowHeaderWidthInfo mHi = hi as MultiRowHeaderWidthInfo;
					if(mHi != null) {
						for(int i = 0; i < mHi.Rows.Count; i++) {
							MultiRowHeaderWidthInfoRow row = mHi.Rows[i];
							bool isLastRow = i == mHi.Rows.Count - 1;
							int maxColumnRowCount = GetMaxColumnRowCount(row);
							foreach(HeaderWidthInfo header in row.Headers) {
								TreeListColumn column = ((PrintColumnWrapper)header.HeaderObject).Column;
								lastRight = CalcBandColumnPrintInfoMultiRow(((PrintColumnWrapper)header.HeaderObject), lastRight, lastBottom, bandInfo.Bounds.Right, bandInfo, i, isLastRow, maxColumnRowCount);
							}
							lastBottom += printInfo.ColumnPanelHeight * GetMaxColumnRowCount(row);
							lastRight = bandInfo.Bounds.X;
						}
					}
					else {
						TreeListColumn column = ((PrintColumnWrapper)hi.HeaderObject).Column;
						lastRight = CalcBandColumnPrintInfo(hi, lastRight, bandInfo.Bounds.Bottom, bandInfo.Bounds.Right);
					}
				}
			}
			else {
				PrintColumnInfo colInfo = new PrintColumnInfo(null);
				colInfo.Bounds = new Rectangle(bandInfo.Bounds.X, printInfo.BandPanelRowHeight * BandsRowCount, bandInfo.Bounds.Width, printInfo.ColumnPanelHeight * ColumnRowCount);
				colInfo.RowCount = ColumnRowCount;
				ColumnPrintInfo.Add(colInfo);
			}
		}
		protected int GetMaxColumnRowCount(MultiRowHeaderWidthInfoRow row) {
			int count = 0;
			foreach(HeaderWidthInfo header in row.Headers)
				count = Math.Max(count, ((PrintColumnWrapper)header.HeaderObject).Column.RowCount);
			return count;
		}
		protected virtual int CalcBandColumnPrintInfoMultiRow(PrintColumnWrapper wrapper, int lastRight, int lastBottom, int maxRight, PrintBandInfo bi, int rowIndex, bool isLastRow, int maxColumnRowCount) {
			TreeListColumn column = wrapper.Column;
			PrintColumnInfo colInfo = new PrintColumnInfo(column);
			int rowCount = column.RowCount;
			if(column.AutoFill)
				rowCount = isLastRow ? ColumnRowCount - rowIndex : rowCount = maxColumnRowCount;
			colInfo.RowIndex = rowIndex;
			colInfo.RowCount = rowCount;
			colInfo.Bounds = new Rectangle(lastRight, lastBottom, Math.Max(0, Math.Min(wrapper.Width, maxRight - lastRight)), printInfo.ColumnPanelHeight * rowCount);
			ColumnPrintInfo.Add(colInfo);
			if(!column.AutoFill && isLastRow && rowIndex + column.RowCount < ColumnRowCount) {
				PrintColumnInfo emptyInfo = new PrintColumnInfo(null);
				emptyInfo.RowIndex = rowIndex;
				emptyInfo.RowCount = ColumnRowCount - rowIndex - column.RowCount;
				emptyInfo.Bounds = new Rectangle(lastRight, colInfo.Bounds.Bottom, Math.Max(0, Math.Min(wrapper.Width, maxRight - lastRight)), printInfo.ColumnPanelHeight * (ColumnRowCount - rowIndex - column.RowCount));
				ColumnPrintInfo.Add(emptyInfo);
			}
			return colInfo.Bounds.Right;
		}
		protected virtual int CalcBandColumnPrintInfo(HeaderWidthInfo hi, int lastRight, int lastBottom, int maxRight) {
			TreeListColumn column = ((PrintColumnWrapper)hi.HeaderObject).Column;
			PrintColumnInfo colInfo = new PrintColumnInfo(column);
			colInfo.Bounds = new Rectangle(lastRight, lastBottom, Math.Max(0, Math.Min(hi.Width, maxRight - lastRight)), printInfo.ColumnPanelHeight);
			ColumnPrintInfo.Add(colInfo);
			return colInfo.Bounds.Right;
		}
		protected internal void SetDefaultBrickStyle(BrickStyle style) {
			graph.DefaultBrickStyle = style;
		}
		protected virtual void CreateDetailHeader() {
			CreateBandBricks();
			CreateColumnBricks();
		}
		protected virtual void CreateDetails() {
			CreateRowBricks();
		}
		protected virtual void CreateReportFooter() { }
		protected virtual void CreateReportHeader() {
			CreateCaptionBrick();
		}
		protected virtual int CaptionHeight { get { return 30; } }
		void CreateCaptionBrick() {
			if(!TreeList.OptionsView.ShowCaption) return;
			AppearanceObject appearance = new AppearanceObject();
			if(UsePrintStyles)
				AppearanceHelper.Combine(appearance, new AppearanceObject[] { PrintInfo.PrintAppearance.Caption });
			else
				AppearanceHelper.Combine(appearance, new AppearanceObject[] { viewInfo.PaintAppearance.Caption });
			SetDefaultBrickStyle(AppearanceHelper.CreateBrick(appearance, DevExpress.XtraPrinting.BorderSide.All, BorderColor, 1));
			ITextBrick brick = (ITextBrick)PS.CreateBrick("TextBrick");
			brick.Text = TreeList.Caption;
			Rectangle r = new Rectangle(0, 0, ColumnTotalWidth, CaptionHeight);
			brick.Rect = r;
			graph.DrawBrick(brick, r);
		}
		void CreateBandBricks() {
			if(!TreeList.OptionsPrint.PrintBandHeader) return;
			foreach(PrintBandInfo info in BandPrintInfo) {
				if(info.Band == null) continue;
				ITextBrick brick = CreateBandBrick(info.Band, info.Bounds);
				graph.DrawBrick(brick, info.Bounds);
			}
		}
		void CreateColumnBricks() {
			if(!TreeList.OptionsPrint.PrintPageHeader) return;
			foreach(PrintColumnInfo info in ColumnPrintInfo) {
				if(info.Column == null) continue;
				ITextBrick brick = CreateColumnBrick(info.Column, info.Bounds);
				graph.DrawBrick(brick, info.Bounds);
			}
		}
		protected virtual ITextBrick CreateBandBrick(TreeListBand band, Rectangle rect) {
			AppearanceObject appearance = null;
			if(UsePrintStyles)
				appearance = PrintInfo.PrintAppearance.BandPanel;
			else {
				appearance = new AppearanceObject();
				AppearanceHelper.Combine(appearance, new AppearanceObject[] { band.AppearanceHeader, viewInfo.PaintAppearance.BandPanel });
			}
			SetDefaultBrickStyle(AppearanceHelper.CreateBrick(appearance, DevExpress.XtraPrinting.BorderSide.All, BorderColor, 1));
			ITextBrick brick = (ITextBrick)PS.CreateBrick("TextBrick");
			brick.Style.SetAlignment(appearance.TextOptions.HAlignment, appearance.TextOptions.VAlignment);
			if(band != null)
				brick.Text = band.GetTextCaption();
			brick.Rect = rect;
			return brick;
		}
		protected virtual ITextBrick CreateColumnBrick(TreeListColumn col, Rectangle rect) {
			AppearanceObject appearance = null;
			if(UsePrintStyles)
				appearance = PrintInfo.PrintAppearance.HeaderPanel;
			else {
				appearance = new AppearanceObject();
				AppearanceHelper.Combine(appearance, new AppearanceObject[] { col.AppearanceHeader, viewInfo.PaintAppearance.HeaderPanel });
			}
			SetDefaultBrickStyle(AppearanceHelper.CreateBrick(appearance, DevExpress.XtraPrinting.BorderSide.All, BorderColor, 1));
			ITextBrick brick = (ITextBrick)PS.CreateBrick("TextBrick");
			brick.Style.SetAlignment(appearance.TextOptions.HAlignment, appearance.TextOptions.VAlignment);
			if(col != null)
				brick.Text = col.GetTextCaption();
			brick.Rect = rect;
			return brick;
		}
		void CreateRowBricks() {
			if(ColumnPrintInfo.Count == 0) return;
			TreeListOperationPrintEachNode operation = CreatePrintEachNodeOperation();
			if(operation != null)
				TreeList.NodesIterator.DoOperation(operation);
		}
		protected virtual TreeListOperationPrintEachNode CreatePrintEachNodeOperation() {
			if(TreeList.OptionsPrint.PrintAllNodes)
				return new TreeListOperationPrintEachNode(TreeList, this, viewInfo, TreeList.OptionsPrint.PrintTree, TreeList.OptionsPrint.PrintImages);
			return new TreeListOperationPrintVisibleNode(TreeList, this, viewInfo, TreeList.OptionsPrint.PrintTree, TreeList.OptionsPrint.PrintImages);
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			if(PrintInfo != null)
				PrintInfo.SetPrintAppearanceDirty();
			TreeList.OnAppearancePrintChanged();
		}
		protected internal virtual void PrintLayoutChanged() {
			PrintInfo.NeedsRecalc = true;
		}
		protected virtual void RecalcPrintInfo() {
			if(!PrintInfo.NeedsRecalc) return;
			if(UsePrintStyles) PrintInfo.InitFromPrintSettings();
			else PrintInfo.InitFromViewInfo();
			PrintInfo.NeedsRecalc = false;
		}
		protected internal int ColumnTotalWidth {
			get {
				int sum = 0;
				if(ColumnPrintInfo != null) {
					foreach(PrintColumnInfo wi in ColumnPrintInfo) {
						if(wi.RowIndex == 0)
							sum += wi.Bounds.Width;
					}
				}
				return sum;
			}
		}
		protected bool UsePrintStyles { get { return TreeList.OptionsPrint.UsePrintStyles; } }
		protected Color BorderColor { get { return (UsePrintStyles ? PrintInfo.PrintAppearance.Lines.BackColor : viewInfo.PaintAppearance.HorzLine.BackColor); } }
		public TreeListPrintAppearanceCollection Appearance { get { return appearance; } }
		public TreeListPrintInfo PrintInfo { get { return printInfo; } }
		public TreeList TreeList { get { return treeList; } }
	}
	public class TreePainter {
		private Graphics g;
		internal AppearanceObject ButtonStyle;
		public static readonly float One = 1f;
		public static readonly float Two = 2f;
		public TreePainter() {
			g = null;
			ButtonStyle = null;
		}
		internal void DrawIndents(Graphics g, ArrayList indents, RectangleF indentItem, Brush brush) {
			if(brush == null) return;
			this.g = g;
			foreach(RowIndentItem rii in indents) {
				switch(rii) {
					case RowIndentItem.FirstRoot:
						DrawFirstRootIndentItem(indentItem, brush);
						break;
					case RowIndentItem.Root:
						DrawRootIndentItem(indentItem, brush);
						break;
					case RowIndentItem.LastRoot:
						DrawLastRootIndentItem(indentItem, brush);
						break;
					case RowIndentItem.Parent:
						DrawParentIndentItem(indentItem, brush);
						break;
					case RowIndentItem.Single:
						DrawSingleIndentItem(indentItem, brush);
						break;
					case RowIndentItem.NextChild:
						DrawNextChildIndentItem(indentItem, brush);
						break;
					case RowIndentItem.LastChild:
						DrawLastChildIndentItem(indentItem, brush);
						break;
					case RowIndentItem.None:
						break;
				}
				indentItem.X += indentItem.Width;
			}
			this.g = null;
		}
		private void DrawFirstRootIndentItem(RectangleF indentItem, Brush foreBrush) {
			g.FillRectangle(foreBrush,
				new RectangleF(indentItem.Left + (indentItem.Width - One) / 2,
				indentItem.Top + (indentItem.Height - One) / 2,
				indentItem.Width / 2,
				One));
			g.FillRectangle(foreBrush,
				new RectangleF(indentItem.Left + (indentItem.Width - One) / 2,
				indentItem.Top + (indentItem.Height - One) / 2,
				One,
				indentItem.Height / 2 + One));
		}
		private void DrawRootIndentItem(RectangleF indentItem, Brush foreBrush) {
			g.FillRectangle(foreBrush,
				new RectangleF(indentItem.Left + (indentItem.Width - One) / 2,
				indentItem.Top,
				One,
				indentItem.Height));
		}
		private void DrawLastRootIndentItem(RectangleF indentItem, Brush foreBrush) {
			g.FillRectangle(foreBrush,
				new RectangleF(indentItem.Left + (indentItem.Width - One) / 2,
				indentItem.Top + (indentItem.Height - One) / 2,
				indentItem.Width / 2,
				One));
		}
		private void DrawParentIndentItem(RectangleF indentItem, Brush foreBrush) {
			DrawSingleIndentItem(indentItem, foreBrush);
			g.FillRectangle(foreBrush,
				new RectangleF(indentItem.Left + (indentItem.Width - One) / 2,
				indentItem.Top + (indentItem.Height - One) / 2,
				One,
				indentItem.Height / 2 + One));
		}
		private void DrawSingleIndentItem(RectangleF indentItem, Brush foreBrush) {
			g.FillRectangle(foreBrush,
				new RectangleF(indentItem.Left,
				indentItem.Top + (indentItem.Height - One) / 2,
				indentItem.Width,
				One));
		}
		private void DrawNextChildIndentItem(RectangleF indentItem, Brush foreBrush) {
			DrawRootIndentItem(indentItem, foreBrush);
			g.FillRectangle(foreBrush,
				new RectangleF(indentItem.Left + (indentItem.Width - One) / 2,
				indentItem.Top + (indentItem.Height - One) / 2,
				indentItem.Width / 2,
				One));
		}
		private void DrawLastChildIndentItem(RectangleF indentItem, Brush foreBrush) {
			g.FillRectangle(foreBrush,
				new RectangleF(indentItem.Left + (indentItem.Width - One) / 2,
				indentItem.Top,
				One,
				indentItem.Height / 2));
			g.FillRectangle(foreBrush,
				new RectangleF(indentItem.Left + (indentItem.Width - One) / 2,
				indentItem.Top + (indentItem.Height - One) / 2,
				indentItem.Width / 2,
				One));
		}
		internal void DrawButton(GraphicsCache cache, bool buttonExpanded, RectangleF rf) {
			Graphics gr = cache.Graphics;
			if(gr == null) return;
			gr.FillRectangle(ButtonStyle.GetBackBrush(cache), rf);
			gr.DrawRectangle(ButtonStyle.GetForePen(cache), rf.Left, rf.Top, rf.Width, rf.Height);
			gr.FillRectangle(ButtonStyle.GetForeBrush(cache),
				new RectangleF(rf.Left + Two,
				rf.Top + (rf.Height - One) / 2,
				rf.Width - 2.0f * Two + 1, One)); 
			if(!buttonExpanded)
				gr.FillRectangle(ButtonStyle.GetForeBrush(cache),
					new RectangleF(rf.Left + (rf.Width - One) / 2,
					rf.Top + Two, One, rf.Height - 2.0f * Two + 1)); 
		}
	}
	public class TreeListPrintInfo {
		public int ColumnPanelHeight;
		public int RowHeight;
		public int PreviewHeight;
		public int RowFooterHeight;
		public int FooterHeight;
		public int BandPanelRowHeight;
		TreeListPrinter printer;
		TreeListPrintAppearanceCollection printAppearance;
		bool printAppearanceDirty, needsRecalc;
		internal TreeListPrintInfo(TreeListPrinter printer) {
			this.printer = printer;
			this.printAppearanceDirty = this.needsRecalc = true;
			ColumnPanelHeight = RowHeight = PreviewHeight = RowFooterHeight =
				FooterHeight = BandPanelRowHeight = 0;
			NeedsRecalc = true;
			this.printAppearance = new TreeListPrintAppearanceCollection(TreeList);
		}
		internal void InitFromViewInfo() {
			TreeListViewInfo vi = TreeList.ViewInfo;
			ColumnPanelHeight = vi.GetColumnPanelHeight(); 
			BandPanelRowHeight = vi.GetBandPanelRowHeight();
			RowHeight = vi.RC.MinRowHeight;
			if(!TreeList.OptionsPrint.AutoRowHeight) {
				RowHeight = vi.RowHeight;
				ColumnPanelHeight = vi.ColumnPanelHeight;
				BandPanelRowHeight = vi.BandPanelRowHeight;
			}
			PreviewHeight = vi.RC.PreviewRowHeight;
			RowFooterHeight = vi.GetFooterPanelHeightCore(vi.RC.GroupFooterCellHeight, 1);
			FooterHeight = vi.GetFooterPanelHeightCore(vi.SummaryFooterInfo.CellHeight, 1);
		}
		internal void InitFromPrintSettings() {
			SetPrintAppearanceDirty();
			TreeListViewInfo vi = TreeList.ViewInfo;
			using(Graphics g = TreeList.CreateGraphics()) {
				ColumnPanelHeight = Math.Max(GetMinHeight(g, PrintAppearance.HeaderPanel) + 6, TreeList.ViewInfo.ColumnPanelHeight);
				BandPanelRowHeight = Math.Max(GetMinHeight(g, PrintAppearance.BandPanel) + 6, TreeList.ViewInfo.BandPanelRowHeight);
				RowHeight = GetMinHeight(g, PrintAppearance.Row) + 4 + CellInfo.CellTextIndent;
				if(!vi.RC.StateImageSize.IsEmpty)
					RowHeight = Math.Max(RowHeight, vi.RC.StateImageSize.Height + 2 * CellInfo.CellTextIndent);
				if(!vi.RC.SelectImageSize.IsEmpty)
					RowHeight = Math.Max(RowHeight, vi.RC.SelectImageSize.Height + 2 * CellInfo.CellTextIndent);
				if(!TreeList.OptionsPrint.AutoRowHeight)
					RowHeight = Math.Max(RowHeight, TreeList.ViewInfo.RowHeight);
				PreviewHeight = TreeList.PreviewLineCount * GetMinHeight(g, PrintAppearance.Preview) + 4;
				RowFooterHeight = GetMinHeight(g, PrintAppearance.GroupFooter) + 6;
				FooterHeight = GetMinHeight(g, PrintAppearance.FooterPanel) + 8;
			}
		}
		private int GetMinHeight(Graphics g, AppearanceObject appearance) {
			if(appearance == null) return 0;
			int height = Convert.ToInt32(appearance.CalcTextSize(g, "Wg", 0).Height);
			return height;
		}
		public void SetPrintAppearanceDirty() { this.printAppearanceDirty = true; }
		protected virtual void UpdatePrintAppearance() {
			if(!this.printAppearanceDirty) return;
			this.printAppearance.Combine(printer.Appearance, TreeListPrinter.GetPrintAppearanceDefaults());
			AppearanceHelper.Combine(this.printAppearance.EvenRow, printer.Appearance.EvenRow, printAppearance.Row);
			AppearanceHelper.Combine(this.printAppearance.OddRow, printer.Appearance.OddRow, printAppearance.Row);
			this.printAppearanceDirty = false;
		}
		TreeList TreeList { get { return printer.TreeList; } }
		public TreeListPrintAppearanceCollection PrintAppearance {
			get {
				UpdatePrintAppearance();
				return printAppearance;
			}
		}
		public bool NeedsRecalc {
			get { return needsRecalc; }
			set {
				this.needsRecalc = value;
				if(NeedsRecalc)
					SetPrintAppearanceDirty();
			}
		}
	}
	internal class IndentButtonInfo {
		RectangleF buttonRect;
		TreeListNode node;
		public IndentButtonInfo(RectangleF buttonRect, TreeListNode node) {
			this.buttonRect = buttonRect;
			this.node = node;
		}
		public RectangleF ButtonRect { get { return buttonRect; } }
		public TreeListNode Node { get { return node; } }
	}
	public class PrintBandInfo {
		public PrintBandInfo(TreeListBand band) {
			Band = band;
		}
		public TreeListBand Band { get; private set; }
		public Rectangle Bounds { get; set; }
	}
	public class PrintColumnInfo {
		public PrintColumnInfo(TreeListColumn column) {
			Column = column;
			RowCount = 1;
			RowIndex = 0;
		}
		public int RowCount { get; set; }
		public int RowIndex { get; set; }
		public TreeListColumn Column { get; private set; }
		public Rectangle Bounds { get; set; }
	}
	public class PrintBandWrapper : IHeaderObject {
		int width;
		List<PrintBandWrapper> bands;
		List<PrintColumnWrapper> columns;
		public PrintBandWrapper(TreeListBand band) {
			Band = band;
			bands = new List<PrintBandWrapper>();
			columns = new List<PrintColumnWrapper>();
			foreach(TreeListBand child in Band.Bands)
				bands.Add(new PrintBandWrapper(child));
			foreach(TreeListColumn column in Band.Columns)
				columns.Add(new PrintColumnWrapper(column));
			width = Band.VisibleWidth;
		}
		public TreeListBand Band { get; private set; }
		#region IHeaderObject Members
		public int Width {
			get { return width; }
		}
		public int MinWidth {
			get { return Band.MinWidth; }
		}
		public int VisibleWidth {
			get { return width; }
		}
		public bool Visible {
			get { return Band.Visible; }
		}
		public FixedStyle Fixed {
			get { return FixedStyle.None; }
		}
		public bool FixedWidth {
			get { return false; }
		}
		public IList Children {
			get { return bands; }
		}
		public IList Columns {
			get { return columns; }
		}
		public IHeaderObject Parent {
			get { return null; }
		}
		public void SetWidth(int width, bool onlyVisibleWidth) {
			this.width = width;
		}
		#endregion
	}
	public class PrintColumnWrapper : IHeaderObject {
		int width;
		public PrintColumnWrapper(TreeListColumn column) {
			Column = column;
			width = Column.VisibleWidth;
		}
		public TreeListColumn Column { get; private set; }
		#region IHeaderObject Members
		public int Width {
			get { return width; }
		}
		public int MinWidth {
			get { return Column.MinWidth; }
		}
		public int VisibleWidth {
			get { return width; }
		}
		public bool Visible {
			get { return Column.Visible; }
		}
		public FixedStyle Fixed {
			get { return FixedStyle.None; }
		}
		public bool FixedWidth {
			get { return false; }
		}
		public IList Children {
			get { return null; }
		}
		public IList Columns {
			get { return null; }
		}
		public IHeaderObject Parent {
			get { return null; }
		}
		public void SetWidth(int width, bool onlyVisibleWidth) {
			this.width = width;
		}
		#endregion
	}
	public class PrintBandRow : IBandRow {
		public PrintBandRow() {
			Columns = new List<PrintColumnWrapper>();
		}
		public List<PrintColumnWrapper> Columns { get; private set; }
		#region IBandRow Members
		ICollection IBandRow.Columns {
			get { return Columns; }
		}
		#endregion
	}
	class AdditionalSheetInfoWrapper : IAdditionalSheetInfo {
		public string Name { get { return "Additional Data"; } }
		public XlSheetVisibleState VisibleState { get { return XlSheetVisibleState.Hidden; } }
	}
	public class TreeListImplementer : IGridView<TreeListColumnImplementer, TreeListNodeImplementer> {
		protected TreeList treeListCore;
		ExportTarget exportTargetCore;
		public TreeListImplementer(TreeList treeList, ExportTarget exportTarget) {
			treeListCore = treeList;
			exportTargetCore = exportTarget;
		}
		public XlCellFormatting AppearanceGroupRow { get { return null; } }
		public XlCellFormatting AppearanceEvenRow { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceOddRow { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceGroupFooter { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceHeader { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceFooter { get { return new XlCellFormatting(); } }
		public XlCellFormatting AppearanceRow { get { return new XlCellFormatting(); } }
		public IGridViewAppearancePrint AppearancePrint { get { return null; } }
		public int FixedRowsCount { get { return 1; } }
		public IEnumerable<FormatConditionObject> FormatConditionsCollection {
			get {
				List<FormatConditionObject> result = new List<FormatConditionObject>();
				foreach(StyleFormatCondition item in treeListCore.FormatConditions) {
					FormatConditionObject fco = ExcelFormattingConverter.Convert(item);
					if(item.Column != null && item.Condition != DevExpress.XtraGrid.FormatConditionEnum.Expression) {
						fco.Value1 = fco.Value1; 
						fco.Value2 = fco.Value2; 
					}
					result.Add(fco);
				}
				return result;
			}
		} 
		public IEnumerable<IFormatRuleBase> FormatRulesCollection {
			get {
				List<IFormatRuleBase> resultLst = new List<IFormatRuleBase>();
				foreach(TreeListFormatRule item in treeListCore.FormatRules)
					resultLst.Add(new FormatRuleImplementer(item));
				foreach(StyleFormatCondition item in treeListCore.FormatConditions)
					resultLst.Add(new FormatRuleImplementer(ConvertFormatConditionToFormatRule(item)));
				return resultLst;
			}
		}
		TreeListFormatRule ConvertFormatConditionToFormatRule(StyleFormatCondition condition) {
			TreeListFormatRule format = new TreeListFormatRule();
			format.ApplyToRow = condition.ApplyToRow;
			format.Enabled = condition.Enabled;
			format.Tag = condition.Tag;
			format.Name = condition.Name;
			if(condition.Column != null) format.Column = condition.Column;
			else
				if(!string.IsNullOrEmpty(condition.ColumnName)) format.ColumnName = condition.ColumnName;
			if(condition.Condition ==  DevExpress.XtraGrid.FormatConditionEnum.Expression) {
				FormatConditionRuleExpression ruleExpression = new FormatConditionRuleExpression() { Expression = condition.Expression };
				ruleExpression.Appearance.Assign(condition.Appearance);
				format.Rule = ruleExpression;
			}
			else {
				FormatConditionRuleValue ruleValue = new FormatConditionRuleValue() { Condition = (FormatCondition)condition.Condition, Value1 = condition.Value1, Value2 = condition.Value2 };
				ruleValue.Appearance.Assign(condition.Appearance);
				format.Rule = ruleValue;
			}
			return format;
		}
		public IEnumerable<TreeListColumnImplementer> GetAllColumns() {
			var res = new List<TreeListColumnImplementer>();
			int gridColumnIndex = 0;
			foreach(TreeListColumn column in treeListCore.VisibleColumns) {
				if(CanAddColumn(column))
					res.Add(new TreeListColumnImplementer(column, gridColumnIndex++));
			}
			return res;
		}
		bool CanAddColumn(TreeListColumn column) {
			bool canExportColumn = treeListCore.OptionsView.ShowColumns || column.Visible;
			return column.OptionsColumn.Printable != DefaultBoolean.False && canExportColumn;
		}
		public IEnumerable<TreeListNodeImplementer> GetAllRows() {
			List<TreeListNodeImplementer> result = new List<TreeListNodeImplementer>();
			foreach(TreeListNode node in treeListCore.Nodes) {
				if(node.IsVisible)
					result.Add(new TreeListNodeImplementer(node));
			}
			return result;
		}
		public bool GetAllowMerge(TreeListColumnImplementer col) { return false; }
		public IEnumerable<TreeListColumnImplementer> GetGroupedColumns() { return null; }
		public FormatSettings GetRowCellFormatting(TreeListNodeImplementer row, TreeListColumnImplementer col) { return null; }
		public string GetRowCellHyperlink(TreeListNodeImplementer row, TreeListColumnImplementer col) { return null; }
		public object GetRowCellValue(TreeListNodeImplementer row, TreeListColumnImplementer col) {			
			if(col == null) return null;
			if(row == null) return null;
			return row.Node.GetValue(col.Column);
		}
		public string GetViewCaption {
			get {
				if(treeListCore.OptionsView.ShowCaption)
					return treeListCore.Caption;
				return string.Empty;
			}
		}
		public IAdditionalSheetInfo AdditionalSheetInfo { get { return new AdditionalSheetInfoWrapper(); } }
		public IEnumerable<ISummaryItemEx> GridGroupSummaryItemCollection {
			get { return new List<ISummaryItemEx>(); }
		} 
		public IEnumerable<ISummaryItemEx> GridTotalSummaryItemCollection {
			get { 
				List<ISummaryItemEx> totalSummary = new List<ISummaryItemEx>(); 
				if(treeListCore.Columns == null) return totalSummary;
				foreach(TreeListColumn column in treeListCore.Columns)
					totalSummary.Add(TreeListTotalSummaryImplementer.Create(treeListCore, column));
				return totalSummary;
			}
		}
		public IEnumerable<ISummaryItemEx> FixedSummaryItems { get { return new List<ISummaryItemEx>(); } }
		public IEnumerable<ISummaryItemEx> GroupHeaderSummaryItems { get { return new List<ISummaryItemEx>(); } }
		public bool IsCancelPending { get { return treeListCore.ProgressWindow != null && treeListCore.ProgressWindow.CancelPending; } }
		public int RaiseMergeEvent(int startRow, int rowLogicalPosition, TreeListColumnImplementer col) { return -1; }
		public bool ReadOnly { get { return treeListCore.OptionsBehavior.Editable; } }
		public long RowCount { get { return treeListCore.RowCount; } }
		public int RowHeight { get { return treeListCore.RowHeight; } }
		public bool ShowFooter { get { return treeListCore.OptionsView.ShowRowFooterSummary; } }
		public bool ShowGroupFooter { get { return treeListCore.OptionsView.ShowSummaryFooter; } }
		public bool ShowGroupedColumns { get { return false; } }
		public string FilterString { get { return treeListCore.FindFilterText; } }
	}
	public class ClipboardTreeListImplementer : TreeListImplementer, IClipboardGridView<TreeListColumnImplementer, TreeListNodeImplementer> {
		internal bool ShowProgress;
		public ClipboardTreeListImplementer(TreeList treeList) : base(treeList, ExportTarget.Xls) { }
		public bool CanCopyToClipboard() {
			return true;
		}
		public XlCellFormatting GetCellAppearance(TreeListNodeImplementer nodeImplementor, TreeListColumnImplementer col) {
			if(treeListCore.ViewInfo == null) {
				return ExcelFormattingConverter.Convert(AppearanceObject.EmptyAppearance);
			}
			if(nodeImplementor == null) {
				return ExcelFormattingConverter.Convert(treeListCore.ViewInfo.PaintAppearance.HeaderPanel);
			}
			try {
				TreeListNode node = nodeImplementor.Node;
				ColumnInfo ci = treeListCore.ViewInfo.ColumnsInfo[col.Column];
				if(ci == null)
					ci = treeListCore.ViewInfo.CreateColumnInfo(col.Column);
				RowInfo ri = treeListCore.ViewInfo.RowsInfo[node];
				if(ri == null) {
					ri = treeListCore.ViewInfo.CreateRowInfo(node);
					treeListCore.ViewInfo.CalcConditionsCore(ri);
					treeListCore.ViewInfo.CalcFormatRulesCore(ri);
				}
				CellInfo cell = treeListCore.ViewInfo.CreateCellInfo(ci, ri);
				cell.State = (treeListCore.GetVisibleIndexByNode(node) % 2 == 0) ? TreeNodeCellState.Odd : TreeNodeCellState.Even;
				treeListCore.ViewInfo.UpdateRowCellPaintAppearance(cell);
				return ExcelFormattingConverter.Convert(cell.PaintAppearance);
			} catch(Exception) {
				return ExcelFormattingConverter.Convert(AppearanceObject.EmptyAppearance);
			}
		}
		bool isRootNodeSelected(TreeListNode node) {
			if(node.ParentNode == null)
				return false;
			while(node.ParentNode != null) {
				node = node.ParentNode;
				if(node.Selected && !node.Expanded) return true;
			}
			return node.Selected && !node.Expanded;
		}
		public string GetRowCellDisplayText(TreeListNodeImplementer nodeImplementor, string columnName) {
			if(treeListCore.IsCellSelect && !treeListCore.IsCellSelected(nodeImplementor.Node, treeListCore.Columns[columnName])) {
				if(treeListCore.OptionsClipboard.CopyCollapsedData == DefaultBoolean.True && isRootNodeSelected(nodeImplementor.Node))
					return nodeImplementor.Node.GetDisplayText(treeListCore.Columns[columnName]);
				return string.Empty;
			}
			return nodeImplementor.Node.GetDisplayText(treeListCore.Columns[columnName]);
		}
		public new object GetRowCellValue(TreeListNodeImplementer row, TreeListColumnImplementer col) {
			if(treeListCore.IsCellSelect && !treeListCore.IsCellSelected(row.Node, col.Column)) {
				if(treeListCore.OptionsClipboard.CopyCollapsedData == DefaultBoolean.True && isRootNodeSelected(row.Node))
					return row.Node.GetDisplayText(col.Column);
				return string.Empty;
			}
			return row.Node.GetValue(col.FieldName);
		}
		public int GetSelectedCellsCount() {
			if(treeListCore.IsCellSelect)
				return treeListCore.GetSelectedCells().Count;
			else
				return treeListCore.VisibleColumns.Count * treeListCore.Selection.Count;
		}
		public IEnumerable<TreeListColumnImplementer> GetSelectedColumns() {
			if(treeListCore.Selection.Count < 1)
				return new List<TreeListColumnImplementer>();
			if(treeListCore.IsCellSelect && treeListCore.OptionsSelection.MultiSelect) {
				HashSet<TreeListColumnImplementer> columns = new HashSet<TreeListColumnImplementer>();
				IEnumerable<TreeListColumnImplementer> allColumns = GetAllColumns();
				foreach(TreeListNode node in treeListCore.Selection) {
					foreach(TreeListColumnImplementer col in allColumns) {
						if(treeListCore.IsCellSelected(node, col.Column))
							columns.Add(col);
					}
					if(columns.Count == allColumns.Count())
						return columns.OrderBy(e => e.VisibleIndex);
				}
				return columns.OrderBy(e => e.VisibleIndex);
			} else {
				return treeListCore.VisibleColumns.Select(e => new TreeListColumnImplementer(e, e.ColumnIndex)).OrderBy(e => e.VisibleIndex);
			}
		}
		public IEnumerable<TreeListNodeImplementer> GetSelectedRows() {
			List<TreeListClipboardNodeImplementer> result = new List<TreeListClipboardNodeImplementer>();
			foreach(TreeListNode node in treeListCore.Selection) {
				if(node.ParentNode == null || !node.ParentNode.Selected)
					result.Add(new TreeListClipboardNodeImplementer(node));
			}
			return result.OrderBy(e => treeListCore.GetVisibleIndexByNode(e.Node));
		}
		public void ProgressBarCallBack(int progress) {
			if(!ShowProgress) return;
			treeListCore.ProgressWindow.SetProgress(progress);
		}
		public bool UseHierarchyIndent(TreeListNodeImplementer row, TreeListColumnImplementer col) {
			if(col.VisibleIndex != 0) return false;
			if(row.Node.ParentNode == null || !row.Node.ParentNode.Selected) return false;
			return true;
		}
	}
	public class TreeListClipboardNodeImplementer : TreeListNodeImplementer, IClipboardGroupRow<TreeListNodeImplementer> {
		public TreeListClipboardNodeImplementer(TreeListNode node) : base(node) { }
		#region IClipboardGroupRow
		public IEnumerable<TreeListNodeImplementer> GetSelectedRows() {
			List<TreeListNodeImplementer> result = new List<TreeListNodeImplementer>();
			if(nodeCore.TreeList.OptionsClipboard.CopyCollapsedData == DefaultBoolean.True && !nodeCore.Selected) {
				foreach(TreeListNode node in nodeCore.Nodes) {
					result.Add(CreateChildImplementer(node));
				}
			} else {
				foreach(TreeListNode node in nodeCore.Nodes)
					if(node.Selected)
						result.Add(CreateChildImplementer(node));
			}
			return result;
		}
		public override IEnumerable<TreeListNodeImplementer> GetAllRows() {
			List<TreeListNodeImplementer> selected = new List<TreeListNodeImplementer>();
			foreach(TreeListNode node in nodeCore.Nodes)
				if(node.Selected)
					selected.Add(CreateChildImplementer(node));
			if(selected.Count > 0)
				return selected;
			return base.GetAllRows();
		}
		protected override TreeListNodeImplementer CreateChildImplementer(TreeListNode node) {
			return new TreeListClipboardNodeImplementer(node);
		}
		public bool IsTreeListGroupRow() { return true; }
		public override int GetRowLevel() {
			if(Node.TreeList.OptionsClipboard.CopyNodeHierarchy != DefaultBoolean.False)
				return base.GetRowLevel();
			return 0;
		}
		#endregion
	}
	public class TreeListNodeImplementer : IRowBase, IGroupRow<TreeListNodeImplementer> {
		public TreeListNodeImplementer(TreeListNode node) {
			nodeCore = node;
		}
		protected TreeListNode nodeCore;
		public TreeListNode Node { get { return nodeCore; } }
		#region IRowBase
		public bool IsDataAreaRow { get { return true; } }
		public int DataSourceRowIndex { get { return nodeCore.Id; } }
		public FormatSettings FormatSettings { get { return null; } }
		public virtual int GetRowLevel() { return nodeCore.Level; }
		public virtual bool IsGroupRow { get { return nodeCore.Nodes.Count > 0; } }
		public int LogicalPosition { get { return nodeCore.Id; } }
		#endregion
		#region IGroupRow
		public virtual IEnumerable<TreeListNodeImplementer> GetAllRows() {
			IEnumerable<TreeListNodeImplementer> result = nodeCore.Nodes.Select(e =>
			{
				return CreateChildImplementer(e);
			});
			return result;
		}
		protected virtual TreeListNodeImplementer CreateChildImplementer(TreeListNode node) {
			return new TreeListNodeImplementer(node);
		}
		public string GetGroupRowHeader() { return string.Empty; }
		public bool IsCollapsed { get { return !nodeCore.Expanded; } }
		#endregion
	}
	public class TreeListColumnImplementer : IColumn {
		TreeListColumn columnCore;
		int indexCore;
		public TreeListColumnImplementer(TreeListColumn column, int index) {
			columnCore = column;
			indexCore = index;
		}
		public TreeListColumn Column { get { return columnCore; } }
		public XlCellFormatting Appearance { get { return ExcelFormattingConverter.Convert(columnCore.AppearanceCell); } }
		public XlCellFormatting AppearanceHeader { get { return ExcelFormattingConverter.Convert(columnCore.AppearanceHeader); } }
		public ColumnEditTypes ColEditType {
			get {
				if(columnCore.ColumnEdit is RepositoryItemComboBox) return ColumnEditTypes.Lookup;
				if(columnCore.ColumnEdit is RepositoryItemLookUpEdit) return ColumnEditTypes.Lookup;
				if(columnCore.ColumnEdit is RepositoryItemProgressBar) return ColumnEditTypes.ProgressBar;
				if(columnCore.ColumnEdit is RepositoryItemPictureEdit
				|| columnCore.ColumnEdit is RepositoryItemImageEdit) return ColumnEditTypes.Image;
				if(columnCore.ColumnEdit is RepositoryItemHyperLinkEdit) return ColumnEditTypes.Hyperlink;
				return ColumnEditTypes.Text;
			}
		}
		public IEnumerable<object> DataValidationItems {
			get {
				List<object> result = new List<object>();
				var ce = columnCore.GetActualColumnEdit();
				if(ce == null) return null;
				if(ce is RepositoryItemImageComboBox) {
					var riic = ce as RepositoryItemImageComboBox;
					foreach(XtraEditors.Controls.ImageComboBoxItem item in riic.Items) {
						if(item.Description != string.Empty) result.Add(item.ToString());
					}
					return result.Count == 0 ? null : result;
				}
				if(ce is RepositoryItemComboBox && !(ce is RepositoryItemImageComboBox)) {
					RepositoryItemComboBox ric = ce as RepositoryItemComboBox;
					foreach(var item in ric.Items)
						result.Add(ric.GetDisplayText(ric.DisplayFormat, item));
					return result;
				}
				if(ce is RepositoryItemTreeListLookUpEdit) {
					RepositoryItemTreeListLookUpEdit rigle = ce as RepositoryItemTreeListLookUpEdit;
					for(int i = 0; rigle.GetDisplayText(i) != null; i++) {
						result.Add(rigle.GetDisplayText(i));
					}
					return result;
				}
				if(ce is RepositoryItemLookUpEdit) {
					RepositoryItemLookUpEdit rile = ce as RepositoryItemLookUpEdit;
					for(int i = 0; rile.GetDataSourceValue(rile.ValueMember, i) != null; i++)
						result.Add(rile.GetDataSourceValue(rile.DisplayMember, i).ToString());
					return result;
				}
				return null;
			}
		}
		public string FieldName { get { return columnCore.FieldName; } }
		string GetFormatString() {
			var ce = columnCore.ColumnEdit as RepositoryItemTextEdit;
			if(ce != null)
				if(ce.Mask.UseMaskAsDisplayFormat && ce.Mask.EditMask != "N00") return ce.Mask.EditMask;
			string format = columnCore.Format.FormatString;
			if(!string.IsNullOrEmpty(format)) return format;
			if(ce != null) {
				string formatRepository = ce.DisplayFormat.FormatString;
				if(!string.IsNullOrEmpty(formatRepository)) return formatRepository;
			}
			return format;
		}
		public FormatSettings FormatSettings {
			get {
				return new FormatSettings { FormatType = columnCore.Format.FormatType, FormatString = GetFormatString(), ActualDataType = columnCore.ColumnType };
			}
		}
		public IEnumerable<IColumn> GetAllColumns() { return null; }
		public int GetColumnGroupLevel() { return -1; }
		public string GetGroupColumnHeader() { return null; }
		public int GroupIndex { get { return -1; } }
		public string Header { get { return columnCore.GetTextCaption(); } }
		public string HyperlinkEditorCaption {
			get {
				if(columnCore.ColumnEdit is RepositoryItemHyperLinkEdit) {
					var ce = columnCore.ColumnEdit as RepositoryItemHyperLinkEdit;
					return ce.Caption;
				}
				return string.Empty;
			}
		}
		public string HyperlinkTextFormatString { get { return string.Empty; } }
		public bool IsCollapsed { get { return false; } }
		public bool IsFixedLeft {
			get {
				if(columnCore.Fixed == FixedStyle.Left) return true;
				TreeListBand band = columnCore.ParentBand;
				if(band != null && band.RootBand != null && band.RootBand.Fixed == FixedStyle.Left) return true;
				return false;
			}
		}
		public ISparklineInfo SparklineInfo { get { return null; } }
		public bool IsGroupColumn { get { return false; } }
		public bool IsVisible { get { return columnCore.Visible; } }
		public int LogicalPosition { get { return indexCore; } }
		public string Name { get { return columnCore.Name; } }
		public IUnboundInfo UnboundInfo { get { return new UnboundInfoWrapper(columnCore); } }
		public int VisibleIndex { get { return columnCore.VisibleIndex; } }
		public int Width { get { return columnCore.Width; } }
		public override int GetHashCode() { return columnCore.GetHashCode(); }
		public override bool Equals(object obj) {
			TreeListColumnImplementer castedObj = obj as TreeListColumnImplementer;
			return castedObj.columnCore == columnCore;
		}
		public DevExpress.Data.ColumnSortOrder SortOrder {
			get { return (DevExpress.Data.ColumnSortOrder)columnCore.SortOrder; }
		}
	}
	public class UnboundInfoWrapper : IUnboundInfo {
		TreeListColumn column;
		public UnboundInfoWrapper(TreeListColumn column) {
			this.column = column;
		}
		public string UnboundExpression {
			get { return column.UnboundExpression; }
		}
		public DevExpress.Data.UnboundColumnType UnboundType {
			get { return Convert(column.UnboundType); }
		}
		DevExpress.Data.UnboundColumnType Convert(DevExpress.XtraTreeList.Data.UnboundColumnType unboundType) {
			string value = unboundType.ToString();
			DevExpress.Data.UnboundColumnType result;
			Enum.TryParse(value, out result);
			return result;
		}
	}
	public class TreeListTotalSummaryImplementer : ISummaryItemEx {		
		protected TreeListTotalSummaryImplementer() { }
		public static TreeListTotalSummaryImplementer Create(TreeList treeList, TreeListColumn column) {
			TreeListTotalSummaryImplementer summory = new TreeListTotalSummaryImplementer();
			summory.ShowInColumnFooterName = column.Caption;
			summory.DisplayFormat = column.SummaryFooterStrFormat;
			summory.FieldName = string.Empty;
			summory.SummaryType = (DevExpress.Data.SummaryItemType)column.SummaryFooter;
			summory.SummaryValue = treeList.GetSummaryValue(column);
			return summory;
		}
		public string DisplayFormat { get; set; }
		public string FieldName { get; private set; }
		public DevExpress.Data.SummaryItemType SummaryType { get; private set; }
		public object SummaryValue { get; private set; }
		public string ShowInColumnFooterName { get; private set; }
		public object GetSummaryValueByGroupId(int groupId) { return null; }
	}
	public class FormatRuleImplementer : IFormatRuleBase {
		TreeListFormatRule rule;
		public FormatRuleImplementer(TreeListFormatRule rule) {
			this.rule = rule;
		}
		public bool StopIfTrue { get { return rule.StopIfTrue; } }
		public bool ApplyToRow { get { return rule.ApplyToRow; } }
		public IColumn ColumnApplyTo {
			get {
				if(rule.ColumnApplyTo != null)
					return new TreeListColumnImplementer(rule.ColumnApplyTo, rule.ColumnApplyTo.VisibleIndex);
				return null;
			}
		}
		public string ColumnName { get { return rule.ColumnName; } }
		public IColumn Column {
			get {
				if(rule.Column == null) return null;
				return new TreeListColumnImplementer(rule.Column, rule.Column.VisibleIndex);
			}
		}
		public bool Enabled { get { return rule.Enabled; } }
		public string Name { get { return rule.Name; } }
		public IFormatConditionRuleBase Rule { get { return rule.Rule as IFormatConditionRuleBase; } }
		public object Tag {
			get { return rule.Tag; }
			set { rule.Tag = value; }
		}
	}
	class ExcelFormattingConverter {
		const string DefaultFontName = "Tahoma";
		const float DefaultFontSize = 8.25f;
		const string DefaultXlFontName = "Calibri";
		const float DefaultXlFontSize = 11;
		public static XlDifferentialFormatting ConvertConditionAppearance(AppearanceObject appearance) {
			XlDifferentialFormatting result = new XlDifferentialFormatting();
			result.Fill = new XlFill()
			{
				BackColor = appearance.GetBackColor() == Color.Empty ? Color.White : appearance.GetBackColor(),
				ForeColor = appearance.GetForeColor(),
				PatternType = XlPatternType.Solid,
			};
			result.Font = Convert(appearance.GetFont());
			return result;
		}
		public static XlFont Convert(Font font) {
			XlFont result = new XlFont();
			result.Bold = font.Bold;
			result.StrikeThrough = font.Strikeout;
			result.Underline = font.Underline ? XlUnderlineType.Single : XlUnderlineType.None;
			result.Italic = font.Italic;
			result.FontFamily = XlFontFamily.Swiss;
			result.Name = GetFontName(font);
			result.Size = GetFontSize(font);
			GetShemeStyle(result);
			return result;
		}
		static void GetShemeStyle(XlFont font) {
			font.SchemeStyle = font.Name == DefaultXlFontName ? XlFontSchemeStyles.Minor : XlFontSchemeStyles.None;
		}
		static float GetFontSize(Font font) {
			return Math.Abs(font.SizeInPoints - DefaultFontSize) < 0.01 ? DefaultXlFontSize : font.SizeInPoints;
		}
		static string GetFontName(Font font) {
			return font.Name == DefaultFontName ? DefaultXlFontName : font.Name;
		}
		static XlVerticalAlignment Convert(VertAlignment valgm) {
			switch(valgm) {
				case VertAlignment.Top:
					return XlVerticalAlignment.Top;
				case VertAlignment.Center:
					return XlVerticalAlignment.Center;
				case VertAlignment.Bottom:
					return XlVerticalAlignment.Bottom;
			}
			return XlVerticalAlignment.Center;
		}
		static XlHorizontalAlignment Convert(HorzAlignment halgm) {
			switch(halgm) {
				case HorzAlignment.Center:
					return XlHorizontalAlignment.Center;
				case HorzAlignment.Near:
					return XlHorizontalAlignment.Left;
				case HorzAlignment.Far:
					return XlHorizontalAlignment.Right;
				default:
					return XlHorizontalAlignment.General;
			}
		}
		public static XlCellFormatting Convert(AppearanceObject appearance) {
			XlCellFormatting result = new XlCellFormatting();
			GetFill(appearance, result);
			result.Alignment = new XlCellAlignment
			{
				WrapText = appearance.TextOptions.WordWrap.HasFlag(WordWrap.Wrap),
				VerticalAlignment = Convert(appearance.TextOptions.VAlignment),
				HorizontalAlignment = Convert(appearance.TextOptions.HAlignment)
			};
			GetBorders(appearance, result);
			result.Font = Convert(appearance.Font);
			result.Font.Color = GetFontColor(appearance);
			return result;
		}
		static void GetBorders(AppearanceObject appearance, XlCellFormatting result) {
			Color borderColor = appearance.GetBorderColor();
			if(!Equals(borderColor, Color.Empty)) {
				result.Border = new XlBorder
				{
					BottomLineStyle = XlBorderLineStyle.Thin,
					BottomColor = borderColor,
					LeftLineStyle = XlBorderLineStyle.Thin,
					LeftColor = borderColor,
					RightLineStyle = XlBorderLineStyle.Thin,
					RightColor = borderColor,
				};
			}
		}
		static void GetFill(AppearanceObject appearance, XlCellFormatting result) {
			var backColor = appearance.GetBackColor();
			if(!Equals(backColor, Color.Empty)) {
				result.Fill = new XlFill
				{
					BackColor = appearance.GetBackColor(),
					ForeColor = appearance.GetBackColor(),
					PatternType = XlPatternType.Solid
				};
			}
		}
		static XlColor GetFontColor(AppearanceObject appearance) {
			XlColor color = appearance.GetForeColor();
			return Equals(color, XlColor.Empty) ? XlColor.FromTheme(XlThemeColor.Dark1, 0.0) : color;
		}
		static Color GetBorderColor(AppearanceObject appearance) {
			Color color = appearance.GetBorderColor();
			return color == Color.Empty
				? Color.LightGray
				: color;
		}
		public static FormatConditions ConvertFromCondition(DevExpress.XtraGrid.FormatConditionEnum item) { return (FormatConditions)(int)item; }
		public static FormatConditionObject Convert(StyleFormatCondition condition) {
			FormatConditionObject cond = new FormatConditionObject();
			cond.Appearance = ConvertConditionAppearance(condition.Appearance);
			cond.Condition = ConvertFromCondition(condition.Condition);
			cond.ApplyToRow = condition.ApplyToRow;
			cond.ColumnName = condition.ColumnName;
			cond.Value1 = condition.Value1;
			cond.Value2 = condition.Value2;
			cond.Expression = condition.Expression;
			return cond;
		}
	}
}
namespace DevExpress.XtraTreeList.Design {
	public interface IPrintDesigner {
		void ApplyOptions(bool setOptions);
		Size UserControlSize { get; }
		bool AutoApply { get; set; }
		void HideCaption();
	}
}
