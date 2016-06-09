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
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid.Native;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.Data;
using DevExpress.Xpf.Printing;
using System.Reflection;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.Xpf.Grid.Printing {
	public abstract class PrintingDataTreeBuilderBase : DataTreeBuilder {
		protected static ColumnBase[] EmptyColumns = new ColumnBase[0];
		IList<ColumnBase> visibleColumns;
		public PrintingDataTreeBuilderBase(DataViewBase view, double totalHeaderWidth)
			: base(view, null, null) {
			PrintRowTemplate = view.GetPrintRowTemplate();
			PrintHeaderTemplate = view.PrintHeaderTemplate;
			TotalHeaderWidth = totalHeaderWidth;
			ReusingRowData = CreateReusingRowData();
			PrintFooterTemplate = view.PrintFooterTemplate;
			PrintFixedFooterTemplate = view.PrintFixedFooterTemplate;
		}
		public DataTemplate PrintRowTemplate { get; private set; }
		public DataTemplate PrintHeaderTemplate { get; private set; }
		public RowData ReusingRowData { get; private set; }
		public double TotalHeaderWidth { get; private set; }
		public override bool SupportsHorizontalVirtualization { get { return false; } }
		public DataTemplate PrintFooterTemplate { get; private set; }
		public DataTemplate PrintFixedFooterTemplate { get; private set; }
		protected IList<ColumnBase> VisibleColumns {
			get {
				if(visibleColumns == null)
					visibleColumns = GetPrintableColumns();
				return visibleColumns;
			}
		}
		protected virtual IList<ColumnBase> GetPrintableColumns() {
			return View.PrintableColumns.ToList();
		}
		protected virtual RowData CreateReusingRowData() {
			return new RowData(this, false, false);
		}
		public abstract void GenerateAllItems();
		public abstract IDataNode CreateGroupPrintingNode(NodeContainer container, RowNode groupNode, IDataNode parentNode, int index, Size pageSize);
		public abstract IDataNode CreateDetailPrintingNode(NodeContainer container, RowNode rowNode, IDataNode parentNode, int index);
		protected virtual double GetBorderThickness() { return 1; }
		protected IList<T> CopyList<T>(IList<T> source) {
			List<T> list = new List<T>(source.Count);
			foreach(T column in source) {
				list.Add(column);
			}
			return list;
		}
		internal override IList<ColumnBase> GetFixedNoneColumns() {
			return VisibleColumns;
		}
		internal override IList<ColumnBase> GetVisibleColumns() {
			return VisibleColumns;
		}
		internal override IList<ColumnBase> GetFixedLeftColumns() {
			return EmptyColumns;
		}
		internal override IList<ColumnBase> GetFixedRightColumns() {
			return EmptyColumns;
		}
		internal virtual void OnRootNodeDispose() { }
	}
	public abstract class ItemsGenerationStrategyBase {
		DataViewBase view;
		protected DataViewBase View { get { return view; } }
		protected DataProviderBase DataProvider { get { return view.DataProviderBase; } }
		bool autoExpandAllGroups = false;
		public abstract string GetTotalSummaryText(ColumnBase column);
		public abstract string GetFixedTotalSummaryLeftText();
		public abstract string GetFixedTotalSummaryRightText();
		public ItemsGenerationStrategyBase(DataViewBase view) {
			this.view = view;
		}
		public abstract bool RequireFullExpand { get; }
		public void PrepareDataControllerAndPerformPrintingAction(Action printingAction) {
			try {
				GenerateAll();
				printingAction();
			}
			finally {
				ClearAll();
			}
		}
		public void GenerateAll() {
			autoExpandAllGroups = DataProvider.AutoExpandAllGroups;
			if(RequireFullExpand)
				DataProvider.AutoExpandAllGroups = false;
			GenerateAllCore();
		}
		protected virtual void GenerateAllCore() { }
		public void ClearAll() {
			ClearAllCore();
			if(RequireFullExpand)
				DataProvider.AutoExpandAllGroups = autoExpandAllGroups;
		}
		protected virtual void ClearAllCore() { }
		public abstract object GetRowValue(RowData rowData);
		public abstract object GetCellValue(RowData rowData, string fieldName);
		public virtual void Clear() { }
	}
	public interface ISupportMasterDetailPrinting {
		IDescriptorAndDataControlBase GetDescriptorAndGridControl(DataControlDetailDescriptor descriptor);
		bool IsGeneratedControl(DataControlBase grid);
	}
	public interface IDescriptorAndDataControlBase {
		DataControlBase Grid { get; }
		DataControlBase GetDetailGridControl(PrintingDataTreeBuilderBase treeBuilder, out bool isGenerated);
		DetailDescriptorBase Descriptor { get; }
	}
	public static class PrintHelper {
		static readonly string ClipboardExportManagerTypeName = "DevExpress.Xpf.Printing.ClipboardExportManager`2";
		static readonly string PrintHelperTypeName = "DevExpress.Xpf.Printing.PrintHelper";
		static Type printHelperType;
		static Type clipboardExportManagerType;
		static PrintHelper() {
			System.Reflection.Assembly printingAssembly = DevExpress.Data.Utils.Helpers.LoadWithPartialName(AssemblyInfo.SRAssemblyXpfPrinting + ", Version=" + AssemblyInfo.Version);		   
			if(printingAssembly != null){
				printHelperType = printingAssembly.GetType(PrintHelperTypeName);
				clipboardExportManagerType = printingAssembly.GetType(ClipboardExportManagerTypeName);
			}	  
		}
		public static bool IsPrintingAvailable { get { return printHelperType != null; } }
		static void CheckIsPrintingAvailable() {
			if(!IsPrintingAvailable)
				throw new Exception(AssemblyInfo.SRAssemblyXpfPrinting + " dll is not available.");
		}
		static object InvokeMember(MethodBase prototypeMethod, params object[] paramValues) {
			CheckIsPrintingAvailable();
			Type[] paramTypes = prototypeMethod.GetParameters().Select((p) => { return p.ParameterType; }).ToArray();
			MethodInfo info = printHelperType.GetMethod(prototypeMethod.Name, paramTypes);
			return info.Invoke(null, paramValues);
		}
		public static object ClipboardExportManagerInstance(Type typeColumn, Type typeRow, object manager) {
			Type[] typeParams = new Type[] { typeColumn,  typeRow};
			Type constructedType = clipboardExportManagerType.MakeGenericType(typeParams);
			return Activator.CreateInstance(constructedType, manager);
		}
		public static void ShowPrintPreview(FrameworkElement owner, IPrintableControl printableControl) {
			InvokeMember(MethodBase.GetCurrentMethod(), owner, printableControl);
		}
		public static void ShowPrintPreview(FrameworkElement owner, IPrintableControl printableControl, string documentName) {
			InvokeMember(MethodBase.GetCurrentMethod(), owner, printableControl, documentName);
		}
		public static void ShowPrintPreview(FrameworkElement owner, IPrintableControl printableControl, string documentName, string title) {
			InvokeMember(MethodBase.GetCurrentMethod(), owner, printableControl, documentName, title);
		}
		public static void ShowPrintPreview(Window owner, IPrintableControl printableControl) {
			InvokeMember(MethodBase.GetCurrentMethod(), owner, printableControl);
		}
		public static void ShowPrintPreview(Window owner, IPrintableControl printableControl, string documentName) {
			InvokeMember(MethodBase.GetCurrentMethod(), owner, printableControl, documentName);
		}
		public static void ShowPrintPreview(Window owner, IPrintableControl printableControl, string documentName, string title) {
			InvokeMember(MethodBase.GetCurrentMethod(), owner, printableControl, documentName, title);
		}
		public static void ShowPrintPreviewDialog(Window owner, IPrintableControl printableControl) {
			InvokeMember(MethodBase.GetCurrentMethod(), owner, printableControl);
		}
		public static void ShowPrintPreviewDialog(Window owner, IPrintableControl printableControl, string documentName) {
			InvokeMember(MethodBase.GetCurrentMethod(), owner, printableControl, documentName);
		}
		public static void ShowPrintPreviewDialog(Window owner, IPrintableControl printableControl, string documentName, string title) {
			InvokeMember(MethodBase.GetCurrentMethod(), owner, printableControl, documentName, title);
		}
		public static void Print(IPrintableControl printableControl) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl);
		}
		public static void PrintDirect(IPrintableControl printableControl) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl);
		}
		public static void PrintDirect(IPrintableControl printableControl, System.Printing.PrintQueue queue) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, queue);
		}
		public static void ExportToXps(IPrintableControl printableControl, System.IO.Stream stream) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream);
		}
		public static void ExportToXps(IPrintableControl printableControl, System.IO.Stream stream, XtraPrinting.XpsExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream, options);
		}
		public static void ExportToXps(IPrintableControl printableControl, string filePath, XtraPrinting.XpsExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath, options);
		}
		public static void ExportToXps(IPrintableControl printableControl, string filePath) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath);
		}
		public static void ExportToCsv(IPrintableControl printableControl, System.IO.Stream stream) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream);
		}
		public static void ExportToCsv(IPrintableControl printableControl, System.IO.Stream stream, XtraPrinting.CsvExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream, options);
		}
		public static void ExportToCsv(IPrintableControl printableControl, string filePath) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath);
		}
		public static void ExportToCsv(IPrintableControl printableControl, string filePath, XtraPrinting.CsvExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath, options);
		}
		public static void ExportToHtml(IPrintableControl printableControl, System.IO.Stream stream) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream);
		}
		public static void ExportToHtml(IPrintableControl printableControl, System.IO.Stream stream, XtraPrinting.HtmlExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream, options);
		}
		public static void ExportToHtml(IPrintableControl printableControl, string filePath) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath);
		}
		public static void ExportToHtml(IPrintableControl printableControl, string filePath, XtraPrinting.HtmlExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath, options);
		}
		public static void ExportToImage(IPrintableControl printableControl, System.IO.Stream stream) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream);
		}
		public static void ExportToImage(IPrintableControl printableControl, System.IO.Stream stream, XtraPrinting.ImageExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream, options);
		}
		public static void ExportToImage(IPrintableControl printableControl, string filePath) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath);
		}
		public static void ExportToImage(IPrintableControl printableControl, string filePath, XtraPrinting.ImageExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath, options);
		}
		public static void ExportToMht(IPrintableControl printableControl, System.IO.Stream stream) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream);
		}
		public static void ExportToMht(IPrintableControl printableControl, System.IO.Stream stream, XtraPrinting.MhtExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream, options);
		}
		public static void ExportToMht(IPrintableControl printableControl, string filePath) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath);
		}
		public static void ExportToMht(IPrintableControl printableControl, string filePath, XtraPrinting.MhtExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath, options);
		}
		public static void ExportToPdf(IPrintableControl printableControl, System.IO.Stream stream) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream);
		}
		public static void ExportToPdf(IPrintableControl printableControl, System.IO.Stream stream, XtraPrinting.PdfExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream, options);
		}
		public static void ExportToPdf(IPrintableControl printableControl, string filePath) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath);
		}
		public static void ExportToPdf(IPrintableControl printableControl, string filePath, XtraPrinting.PdfExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath, options);
		}
		public static void ExportToRtf(IPrintableControl printableControl, System.IO.Stream stream) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream);
		}
		public static void ExportToRtf(IPrintableControl printableControl, System.IO.Stream stream, XtraPrinting.RtfExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream, options);
		}
		public static void ExportToRtf(IPrintableControl printableControl, string filePath) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath);
		}
		public static void ExportToRtf(IPrintableControl printableControl, string filePath, XtraPrinting.RtfExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath, options);
		}
		public static void ExportToText(IPrintableControl printableControl, System.IO.Stream stream) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream);
		}
		public static void ExportToText(IPrintableControl printableControl, System.IO.Stream stream, XtraPrinting.TextExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream, options);
		}
		public static void ExportToText(IPrintableControl printableControl, string filePath) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath);
		}
		public static void ExportToText(IPrintableControl printableControl, string filePath, XtraPrinting.TextExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath, options);
		}
		public static void ExportToXls(IPrintableControl printableControl, System.IO.Stream stream) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream);
		}
		public static void ExportToXls(IPrintableControl printableControl, string filePath) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath);
		}
		public static void ExportToXls(IPrintableControl printableControl, System.IO.Stream stream, XtraPrinting.XlsExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream, options);
		}
		public static void ExportToXls(IPrintableControl printableControl, string filePath, XtraPrinting.XlsExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath, options);
		}
		public static void ExportToXlsx(IPrintableControl printableControl, System.IO.Stream stream) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream);
		}
		public static void ExportToXlsx(IPrintableControl printableControl, System.IO.Stream stream, XtraPrinting.XlsxExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, stream, options);
		}
		public static void ExportToXlsx(IPrintableControl printableControl, string filePath) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath);
		}
		public static void ExportToXlsx(IPrintableControl printableControl, string filePath, XtraPrinting.XlsxExportOptions options) {
			InvokeMember(MethodBase.GetCurrentMethod(), printableControl, filePath, options);
		}
	}
}
