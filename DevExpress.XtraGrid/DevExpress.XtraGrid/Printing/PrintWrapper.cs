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
using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid.Views.Base;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraTab;
using DevExpress.XtraPrintingLinks;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Threading;
using DevExpress.Data;
namespace DevExpress.XtraGrid.Printing {
	public class ViewPrintWrapperBase : IPrintableEx, IPrintHeaderFooter, IDisposable {
		bool cancelAsked = false;
		UserControl propertyEditor;
		#region IPrintable empty members
		bool IPrintable.CreatesIntersectedBricks { get { return false; } }
		void IPrintable.ShowHelp() { }
		bool IPrintable.SupportsHelp() { return false; }
		#endregion
		#region IPrintable Members
		bool IPrintable.HasPropertyEditor() { return true; }
		UserControl IPrintable.PropertyEditorControl { 
			get { 
				this.propertyEditor = CreatePropertyEditor();
				return this.propertyEditor;
			} 
		}
		void IPrintable.AcceptChanges() { AcceptChanges();  }
		void IPrintable.RejectChanges() { RejectChanges();  }
		#endregion
		#region IBasePrintable Members
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			CreateArea(areaName, graph);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			PrintingSystem = null;
			this.link = null;
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			Initialize(ps, link);
		}
		void IPrintableEx.OnEndActivity() { OnPrintEnd(); }
		void IPrintableEx.OnStartActivity() { OnPrintStart(); }
		#endregion
		#region IPrintHeaderFooter Members
		string IPrintHeaderFooter.InnerPageHeader { get { return InnerPageHeader; } }
		string IPrintHeaderFooter.InnerPageFooter { get { return InnerPageFooter; } }
		string IPrintHeaderFooter.ReportHeader { get { return ReportHeader; } }
		string IPrintHeaderFooter.ReportFooter { get { return ReportFooter; } }
		#endregion
		IPrintingSystem ps;
		PrintableComponentLinkBase link;
		protected virtual void OnPrintEnd() { }
		protected virtual void OnPrintStart() {
			this.cancelAsked = false;
		}
		protected virtual void CreateArea(string areaName, IBrickGraphics graph) { }
		protected virtual void Initialize(IPrintingSystem ps, ILink link) {
			PrintingSystem = ps;
			this.link = (PrintableComponentLinkBase)link;
		}
		protected internal PrintingSystemBase PrintingSystemBase { get { return (PrintingSystemBase)ps; } }
		protected internal IPrintingSystem PrintingSystem {
			get { return ps; }
			set {
				if(PrintingSystem == value) return;
				if(PrintingSystem != null) {
					PrintingSystem.AfterChange -= new DevExpress.XtraPrinting.ChangeEventHandler(OnPrintingSystem_AfterChange);
				}
				ps = value;
				if(PrintingSystem != null) {
					PrintingSystem.AfterChange += new DevExpress.XtraPrinting.ChangeEventHandler(OnPrintingSystem_AfterChange);
				}
			}
		}
		public virtual bool CancelPending { get { return cancelAsked; } }
		public void CancelPrint() {
			this.cancelAsked = true;
			if(PrintingSystemBase != null) PrintingSystemBase.Cancel();
		}
		protected virtual void OnPrintingSystem_AfterChange(object sender, DevExpress.XtraPrinting.ChangeEventArgs e) { }
		protected internal PrintableComponentLinkBase Link { get { return link; } }
		protected UserControl PropertyEditor { get { return propertyEditor; } }
		protected virtual void AcceptChanges() { }
		protected virtual void RejectChanges() { }
		protected virtual UserControl CreatePropertyEditor() { return null;  }
		public virtual void Dispose() {
			this.propertyEditor = null;
			PrintingSystem = null;
			this.link = null;
		}
		protected virtual void OnProgress(int overallProgress) { }
		protected internal bool IsDocumentCreating {
			get {
				return PrintingSystemBase != null && PrintingSystemBase.Document != null && PrintingSystemBase.Document.IsCreating;
			}
		}
		protected virtual string InnerPageHeader { get { return string.Empty; } }
		protected virtual string InnerPageFooter { get { return string.Empty; } }
		protected virtual string ReportHeader { get { return string.Empty; } }
		protected virtual string ReportFooter { get { return string.Empty; } }
	}
	public class ViewPrintWrapper : ViewPrintWrapperBase {
		bool isPrintingDetail = false;
		int printIndent = 0;
		BaseView _view, activeDetailView;
		bool isRootWrapper;
		CancelDelegate pendingCancelMethod = null;
		bool shouldDisposeView = false;
		bool isViewValid = true;
		public ViewPrintWrapper(BaseView view, bool isRootWrapper) : this(view, isRootWrapper, null) { }
		public ViewPrintWrapper(BaseView view, bool isRootWrapper, CancelDelegate pendingCancelMethod) {
			this.isRootWrapper = isRootWrapper;
			this._view = view;
		}
		public bool AfterCreated() {
			if(_view != null) {
				this._view = CheckServerModePrint(_view);
				if(CancelPending) {
					OnPrintEnd();
					return false;
				}
				this._view.PrintPrepareProgress += new ProgressChangedEventHandler(OnPrintPrepareProgress);
			}
			if(!IsValid) {
				OnPrintEnd();
				return false;
			}
			return true;
		}
		public bool IsValid { get { return _view != null && isViewValid; } }
		public virtual ComponentPrinterBase Printer { get { return isRootWrapper ? GridControl.Printer : printer; } }
		protected internal PrintingSystemActivity PrinterActivity { get { return Printer == null ? PrintingSystemActivity.Idle : Printer.Activity; } }
		public override bool CancelPending { get { return base.CancelPending || (pendingCancelMethod != null && pendingCancelMethod()); } }
		public override void Dispose() {
			base.Dispose();
			this.activeDetailView = null;
			if(shouldDisposeView) {
				if(View != null) View.Dispose();
			}
			if(_view != null) {
				this._view.PrintPrepareProgress -= new ProgressChangedEventHandler(OnPrintPrepareProgress);
			}
			this._view = null;
		}
		protected override string InnerPageHeader { get { return View != null ? View.OptionsPrint.RtfPageHeader : string.Empty; } }
		protected override string InnerPageFooter { get { return View != null ? View.OptionsPrint.RtfPageFooter : string.Empty; } }
		protected override string ReportHeader { get { return View != null ? View.OptionsPrint.RtfReportHeader : string.Empty; } }
		protected override string ReportFooter { get { return View != null ? View.OptionsPrint.RtfReportFooter : string.Empty; } }
		protected void SetShouldDisposeView() { this.shouldDisposeView = true; }
		public BaseView PrintView { get { return activeDetailView == null ? View : activeDetailView; } }
		public BaseView View { get { return _view; } }
		protected GridControl GridControl { get { return View.GridControl; } }
		protected override void OnPrintStart() {
			base.OnPrintStart();
			if(View != null) View.OnPrintStart(this);
		}
		protected override void OnPrintEnd() {
			base.OnPrintEnd();
			if(View != null) View.OnPrintEnd(CancelPending);
		}
		protected override void AcceptChanges() {
			if(PropertyEditor == null) return;
			AcceptChanges(PropertyEditor.Controls);
		}
		void AcceptChanges(System.Windows.Forms.Control.ControlCollection controls) {
			if(controls == null) return;
			foreach(Control control in controls) {
				if(control is DevExpress.XtraGrid.Design.IPrintDesigner) {
					DevExpress.XtraGrid.Design.IPrintDesigner ip = control as DevExpress.XtraGrid.Design.IPrintDesigner;
					ip.ApplyOptions(true);
				}
				AcceptChanges(control.Controls);
			}
		}
		protected bool AllowPrintDetails { get { return isRootWrapper; } }
		protected override void OnProgress(int overallProgress) {
			if(View == null) return;
			View.OnPrintExportProgress(overallProgress, overallProgress < 0 ? true : false);
		}
		protected override UserControl CreatePropertyEditor() {
			UserControl ctrl = new UserControl();
			Size minSize = new Size(300, 300);
			XtraTabControl tab = new XtraTabControl();
			tab.Dock = DockStyle.Fill;
			ctrl.Controls.Add(tab);
			minSize = AddPrintDesigner(tab, View, View.GetViewCaption());
			if(AllowPrintDetails) AddLevelTree(tab, ref minSize);
			if(tab.TabPages.Count == 0) return null;
			if(tab.TabPages.Count == 1) tab.ShowTabHeader = DefaultBoolean.False;
			minSize.Height += 30;
			ctrl.Size = minSize;
			ctrl.Visible = true;
			return ctrl;
		}
		void AddLevelTree(XtraTabControl tab, ref Size minSize) {
			if(View.GridControl == null) return;
			BaseView[] templates = View.GridControl.LevelTree.GetTemplates();
			foreach(BaseView view in templates) {
				if(view.GridControl == null || view == View) continue;
				Size size = AddPrintDesigner(tab, view, view.GetViewCaption());
				minSize.Width = Math.Max(size.Width, minSize.Width);
				minSize.Height = Math.Max(size.Height, minSize.Height);
			}
		}
		Size AddPrintDesigner(XtraTabControl tab, BaseView view, string name) {
			Size size = Size.Empty;
			if(!view.IsSupportPrinting) return size;
			UserControl des = view.PrintDesigner;
			if(des == null) return size;
			size = des.Size;
			XtraTabPage page = new XtraTabPage();
			page.Text = name;
			page.Controls.Add(des);
			tab.TabPages.Add(page);
			return size;
		}
		protected internal int PrintIndent { get { return printIndent; } }
		protected internal virtual bool IsPrintingDetail { get { return isPrintingDetail; } }
		protected override void CreateArea(string areaName, IBrickGraphics graph) {
			PrintView.CreatePrintArea(this, areaName, graph);
		}
		protected override void Initialize(IPrintingSystem ps, ILink link) {
			if(GridControl == null) return;
			if(IsPrintingDetail) return;
			base.Initialize(ps, link);
			this.printIndent = 0;
			SetCommandsVisibility();
			if(View != null) View.OnPrintInitialize(PrintingSystem, Link);
		}
		protected internal void OnPrintingSystemProgress(int value) {
			if(IsDocumentCreating) {
				OnProgress(-1);
				return;
			}
			int progress = value;
			if((PrinterActivity & PrintingSystemActivity.Preparing) != 0) {
				progress = value / 2;
				progress += 50;
			}
			OnProgress(progress);
		}
		protected void OnPrintPrepareProgress(object sender, ProgressChangedEventArgs e) {
			int progress = e.ProgressPercentage;
			if((PrinterActivity & (PrintingSystemActivity.Printing | PrintingSystemActivity.Exporting)) != 0) {
				progress = (progress / 2);
			}
			OnProgress(progress);
		}
		protected override void OnPrintingSystem_AfterChange(object sender, DevExpress.XtraPrinting.ChangeEventArgs e) {
			if(PrintingSystem == null || Link == null || View == null) return;
			switch(e.EventName) {
				case DevExpress.XtraPrinting.SR.ProgressPositionChanged:
					OnPrintingSystemProgress(PrintingSystemBase.ProgressReflector.Position);
					break;
				case DevExpress.XtraPrinting.SR.PageSettingsChanged:
				case DevExpress.XtraPrinting.SR.AfterMarginsChange:
					if(View.IsRecreateOnMarginChanged) {
						Link.Margins = PrintingSystemBase.PageMargins;
						Link.CreateDocument();
					}
					break;
			}
		}
		protected virtual void SetCommandsVisibility() {
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportCsv, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportFile, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportGraphic, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportHtm, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportMht, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportPdf, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportRtf, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportTxt, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportXls, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportXlsx, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendCsv, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendFile, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendGraphic, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendMht, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendPdf, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendRtf, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendTxt, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendXls, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendXlsx, true);
		}
		int printCount = 0;
		public virtual void BeginPrint() { this.printCount ++; }
		public virtual void EndPrint() { 
			if(--this.printCount < 0) this.printCount = 0; 
		}
		public virtual bool IsPrinting { get { return printCount != 0; } }
		[ThreadStatic]
		static int rowLevelCounter = 0;
		protected internal virtual void PrintDetail(BaseView detailView, int rowLevel, int levelIndent, int verticalDetailIndent) {
			if(!AllowPrintDetails) return;
			bool prevPrintingDetail = IsPrintingDetail;
			int prevIndent = PrintIndent;
			try {
				rowLevelCounter += rowLevel;
				this.activeDetailView = detailView;
				this.isPrintingDetail = true;
				this.printIndent = detailView.DetailLevel * levelIndent + rowLevelCounter * levelIndent;
				detailView.CheckSynchronize();
				Link.AddSubreport(new PointF(0, verticalDetailIndent));
			}
			finally {
				this.isPrintingDetail = prevPrintingDetail;
				this.activeDetailView = null;
				detailView.ResetPrintInfo();
				rowLevelCounter -= rowLevel;
			}
		}
		protected virtual BaseView CheckServerModePrint(BaseView view) {
			if(view == null || !view.IsServerMode) return view;
			if(TryLoadAllData(view)) return view;
			if(view.ProgressWindowCancelPending) {
				CancelPrint();
				return view;
			}
			this.shouldDisposeView = true;
			BaseView clonedView = CloneView((ColumnView)view, true);
			if(clonedView.DataSource == null) {
				this.isViewValid = false;
				CancelPrint();
			}
			if(clonedView.ProgressWindowCancelPending) {
				CancelPrint();
			}
			return clonedView;
		}
		bool TryLoadAllData(BaseView view) {
			view.ShowMarqueProgressForm();
			if(!view.DataController.PrefetchAllData(delegate() {
				if(view.ProgressWindow != null) {
					view.ProgressWindow.SetMarqueProgress("");
					if(view.ProgressWindow.CancelPending) return true;
				}
				return false;
			})) {
				isViewValid = false;
				return false;
			}
			return true;
		}
		internal static BaseView CloneView(ColumnView view, bool showProgress) {
			ColumnView clonedView = (ColumnView)view.CreateInstance();
			clonedView.SetPrintingOnly(view.OptionsPrint.ShowPrintExportProgress);
			clonedView.PaintStyleName = "Flat";
			clonedView.Assign(view, true);
			((ColumnView)clonedView).DisableCurrencyManager = true;
			clonedView.fSourceView = view;
			CloneViewDataSource(clonedView, view, showProgress);
			((ColumnView)clonedView).RefreshVisibleColumnsList();
			CloneViewSelection(view as ColumnView, clonedView as ColumnView);
			((ColumnView)clonedView).ViewInfo.Calc(null, view.ViewRect);
			return clonedView;
		}
		static void CloneViewDataSource(BaseView clonedView, ColumnView sourceView, bool showProgress) {
			object dataSource = sourceView.DataSource;
			if(sourceView.IsServerMode) {
				if(showProgress) {
					clonedView.ShowMarqueProgressForm();
				}
				dataSource = sourceView.DataController.GetAllFilteredAndSortedRows(delegate() {
					if(clonedView.ProgressWindow != null) {
						clonedView.ProgressWindow.SetMarqueProgress("");
						if(clonedView.ProgressWindow.CancelPending) return true;
					}
					return false;
				});
				clonedView.DataController.ByPassFilter = true;
			}
			clonedView.SetDataSource(null, WrapDataSource(sourceView.DataController, dataSource), string.Empty);
		}
		static object WrapDataSource(DataController controller, object dataSource) {
			if(dataSource is IList)
				return new WrappedList(controller, dataSource as IList);
			return dataSource;
		}
		class WrappedList : ITypedList, IList {
			#region IList Members
			IList source;
			PropertyDescriptorCollection properties;
			internal WrappedList(DataController controller, IList source) {
				this.source = source;
				List<PropertyDescriptor> props = new List<PropertyDescriptor>();
				foreach(DataColumnInfo dc in controller.Columns) {
					if(dc.PropertyDescriptor == null) continue;
					props.Add(dc.PropertyDescriptor);
				}
				this.properties = new PropertyDescriptorCollection(props.ToArray());
			}
			int IList.Add(object value) {
				throw new NotImplementedException();
			}
			void IList.Clear() {
				throw new NotImplementedException();
			}
			bool IList.Contains(object value) {
				throw new NotImplementedException();
			}
			int IList.IndexOf(object value) {
				throw new NotImplementedException();
			}
			void IList.Insert(int index, object value) {
				throw new NotImplementedException();
			}
			bool IList.IsFixedSize { get { return true; } }
			bool IList.IsReadOnly { get { return true; } }
			void IList.Remove(object value) {
				throw new NotImplementedException();
			}
			void IList.RemoveAt(int index) {
				throw new NotImplementedException();
			}
			object IList.this[int index] {
				get { return source[index]; }
				set {
					throw new NotImplementedException();
				}
			}
			void ICollection.CopyTo(Array array, int index) {
				throw new NotImplementedException();
			}
			int ICollection.Count { get { return source.Count; } }
			bool ICollection.IsSynchronized { get { return true; } }
			object ICollection.SyncRoot { get { return this; } }
			IEnumerator IEnumerable.GetEnumerator() { return source.GetEnumerator(); }
			PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) { return properties; }
			string ITypedList.GetListName(PropertyDescriptor[] listAccessors) { return "List"; }
			#endregion
		}
		internal static void CloneViewSelection(ColumnView sourceView, ColumnView targetView) {
			targetView.FocusedRowHandle = GridControl.InvalidRowHandle;
			sourceView.DataController.ResetRowsKeeper();
			using(MemoryStream ms = new MemoryStream()) {
				sourceView.DataController.SaveRowState(ms);
				ms.Seek(0, SeekOrigin.Begin);
				targetView.DataController.RestoreRowState(ms);
			}
			if(!sourceView.IsMultiSelect) return;
			int[] rows = sourceView.GetSelectedRows();
			targetView.BeginSelection();
			targetView.ClearSelection();
			foreach(int row in rows) targetView.SelectRow(row);
			targetView.CancelSelection();
		}
		ComponentPrinterBase printer;
		internal void SetPrinter(ComponentPrinterBase printer) {
			this.printer = printer;
		}
	}
	public delegate bool CancelDelegate();
	public class ViewBackgroundPrinter : IDisposable {
		ColumnView view, sourceView;
		BackgroundWorker worker;
		bool isWorking = false;
		int progressPercentage = 0;
		public ViewBackgroundPrinter(ColumnView sourceView) {
			this.sourceView = sourceView;
		}
		public int ProgressPercentage { get { return progressPercentage; } }
		public bool IsWorking { get { return isWorking; } }
		public void Cancel() {
			if(worker != null) worker.CancelAsync();
			while(worker != null) {
				Thread.Sleep(50);
			}
		}
		public event ProgressChangedEventHandler Progress;
		void OnProgressChanged(object sender, ProgressChangedEventArgs e) {
			this.progressPercentage = e.ProgressPercentage;
			if(Progress != null) Progress(this, e);
		}
		protected virtual ColumnView CloneView(ColumnView sourceView) {
			BaseView clonedView = ViewPrintWrapper.CloneView(sourceView, false);
			return clonedView as ColumnView;
		}
		public virtual void Dispose() {
			if(worker != null) {
				worker.CancelAsync();
				worker.Dispose();
			}
			if(view != null) {
				view.Dispose();
			}
			this.Progress = null;
		}
		public void ExportTo(ExportTarget target, object targetFileNameOrStream, ExportOptionsBase options) {
			if(IsWorking) return;
			this.progressPercentage = 0;
			this.isWorking = true;
			this.worker = new BackgroundWorker();
			this.worker.DoWork += new DoWorkEventHandler(OnBackgroundExport);
			this.worker.ProgressChanged += new ProgressChangedEventHandler(OnProgressChanged);
			this.worker.WorkerReportsProgress = true;
			this.worker.WorkerSupportsCancellation = true;
			worker.RunWorkerAsync(new object[] { target, targetFileNameOrStream, options });
		}
		void OnBackgroundExport(object sender, DoWorkEventArgs e) {
			try {
				object[] args = (object[])e.Argument;
				if(sourceView == null) return;
				this.view = CloneView(sourceView);
				this.view.PrintExportProgress += new ProgressChangedEventHandler(OnViewPrintProgress);
				this.sourceView = null;
				using(ViewPrintWrapper printWrapper = new ViewPrintWrapper(view, false, new CancelDelegate(IsPendingCancel))) {
					using(ComponentPrinterBase printer = new XtraGridComponentPrinter(printWrapper)) {
						printWrapper.SetPrinter(printer);
						printer.Export((ExportTarget)args[0], args[1].ToString());
					}
				}
				ReportWorkerProgress(new ProgressChangedEventArgs(100, true));
			}
			catch(Exception ex) {
				ReportWorkerProgress(new ProgressChangedEventArgs(100, ex));
			}
			finally {
				this.isWorking = false;
				if(worker != null) worker.Dispose();
				worker = null;
			}
		}
		void ReportWorkerProgress(ProgressChangedEventArgs e) {
			if(worker != null) worker.ReportProgress(e.ProgressPercentage, e.UserState);
		}
		bool IsPendingCancel() { return worker != null && worker.CancellationPending; }
		void OnViewPrintProgress(object sender, ProgressChangedEventArgs e) {
			if(worker == null) return;
			worker.ReportProgress(e.ProgressPercentage); 
		}
	}
}
