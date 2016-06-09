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
using IDocument = DevExpress.Xpf.DocumentViewer.IDocument;
using DevExpress.XtraPrinting;
using DevExpress.Mvvm;
using DevExpress.Xpf.DocumentViewer;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using System.Linq;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Parameters.Native;
using DevExpress.ReportServer.Printing;
using DevExpress.Xpf.Printing.Parameters.Models;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Printing.Parameters.Models.Native;
using DevExpress.Xpf.Printing.Native;
using DevExpress.ReportServer.Printing.Services;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.ReportServer.ServiceModel.Native.RemoteOperations;
using System.IO;
using DevExpress.Xpf.Printing.Exports;
using DevExpress.XtraPrinting.Preview;
using DevExpress.Xpf.Printing.Platform.Wpf.Native;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native.DrillDown;
using DevExpress.Xpf.Printing.PreviewControl.Native;
using DevExpress.XtraPrinting.Localization;
using System.Windows.Threading;
using System.Threading;
using DevExpress.XtraPrinting.Native.Navigation;
using System.Threading.Tasks;
using DevExpress.Printing.Native;
namespace DevExpress.Xpf.Printing.PreviewControl.Native.Models {
	public class DocumentViewModel : BindableBase, IDocument, IDocumentViewModel, IProgressSettings {
		bool isCreating = false;
		bool isExporting = false;
		readonly ObservableCollection<PageViewModel> pages;
		ILink link;
		SearchHelper searchHelper;
		object source = null;
		#region events
		public event EventHandler DocumentCreated;
		public event EventHandler StartDocumentCreation;
		public event EventHandler DocumentChanged;
		public event ExceptionEventHandler DocumentException;
		#endregion
		#region properties
		protected internal ILink Link { get { return link; } }
		protected internal PrintingSystemBase PrintingSystem { get; protected set; }
		protected internal Document Document { get { return PrintingSystem.Return(x => x.Document, () => null); } }
		public bool IsLoaded { get { return PrintingSystem != null; } }
		public bool IsCreated { get { return IsLoaded && !IsCreating && Pages.Count > 0; } }
		public bool IsCreating {
			get { return isCreating; }
			protected set {
				SetProperty(ref isCreating, value, () => IsCreating, () => {
					RaisePropertyChanged(() => IsLoaded);
					RaiseProgressChanged();
				});
			}
		}
		protected bool IsExporting {
			get { return isExporting; }
			set { SetProperty(ref isExporting, value, () => IsExporting, () => RaiseProgressChanged()); }
		}
		string IDocumentViewModel.DeafultFileName {
			get { return PrintingSystem.ExportOptions.PrintPreview.DefaultFileName; }
		}
		string IDocumentViewModel.InitialDirectory {
			get { return PrintingSystem.ExportOptions.PrintPreview.DefaultDirectory; }
		}
		ExportFormat IDocumentViewModel.DefaultExportFormat {
			get { return ExportFormatConverter.ToExportFormat(PrintingSystem.ExportOptions.PrintPreview.DefaultExportFormat); }
			set { PrintingSystem.ExportOptions.PrintPreview.DefaultExportFormat = ExportFormatConverter.ToExportCommand(value); }
		}
		ExportFormat IDocumentViewModel.DefaultSendFormat {
			get { return ExportFormatConverter.ToExportFormat(PrintingSystem.ExportOptions.PrintPreview.DefaultSendFormat); }
			set { PrintingSystem.ExportOptions.PrintPreview.DefaultSendFormat = ExportFormatConverter.ToExportCommand(value); }
		}
		IEnumerable<IPage> IDocument.Pages {
			get { return pages; }
		}
		public ObservableCollection<PageViewModel> Pages {
			get { return pages; }
		}
		public Watermark Watermark {
			get { return PrintingSystem.Return(x => x.Watermark, () => null); }
		}
		public bool CanChangePageSettings {
			get { return Document.Return(x => x.CanChangePageSettings, ()=> false); }
		}
		public XtraPageSettingsBase PageSettings {
			get { return PrintingSystem.Return(x => x.PageSettings, ()=> null); }
		}
		#region IProgressViewModel
		public bool InProgress { get { return IsCreating || IsExporting; } }
		public ProgressType ProgressType { get { return IsProgressMarquee() ? ProgressType.Marquee : ProgressType.Default; } }
		public int ProgressPosition { get { return PrintingSystem.Return(x => x.ProgressReflector.PositionCore, () => 0); } }
		bool IsProgressMarquee() {
			return PrintingSystem.Return(x => x.ProgressReflector.Ranges.Count != 0 && float.IsNaN((float)x.ProgressReflector.Ranges[0]), () => false);
		}
		protected void RaiseProgressChanged() {
			RaisePropertiesChanged(
				() => InProgress,
				() => ProgressType,
				() => ProgressPosition);
		}
		#endregion
		#endregion
		#region ctor & Initialize
		protected internal DocumentViewModel() {
			pages = new ObservableCollection<PageViewModel>();
		}
		public virtual void Load(object source) {
			this.source = source;
			if(source is ILink)
				this.link = (ILink)source;
			else {
				try {
					this.link = LoadDocument(source);
				} catch (Exception e) {
					RaiseDocumentException(e);
					return;
				}
			}
			PrintingSystem = link.Return(x => (PrintingSystemBase)x.PrintingSystem, () => null);
			Initialize();
			RaisePropertiesChanged(() => HasBookmarks, () => CanChangePageSettings);
		}
		protected virtual ILink LoadDocument(object source) {
			SimpleLink link = new SimpleLink();
			if (source is Stream) {
				link.PrintingSystem.LoadDocument((Stream)source);
				return link;
			} else if (source is string) {
				link.PrintingSystem.LoadDocument((string)source);
				return link;
			} else
				throw new NotSupportedException(PrintingLocalizer.GetString(PrintingStringId.DocumentSourceNotSupported));
		}
		protected void Initialize() {
			PrintingSystem.Do(ps => {
				PreparePrintingSystem(PrintingSystem);
				Subscribe();
			});
			searchHelper = new SearchHelper();
		}
		protected internal  virtual void Subscribe() {
			Unsubscribe();
			PrintingSystem.Do(ps => {
				ps.BeforeBuildPages += OnBeforeBuildPages;
				ps.DocumentChanged += OnDocumentChanged;
				ps.AfterBuildPages += OnAfterBuildPages;
				ps.CreateDocumentException += OnCreateDocumentException;
				ps.ProgressReflector.PositionChanged += OnProgressChanged;
				ResolveNewPages();
			});
		}
		protected internal virtual void Unsubscribe() {
			PrintingSystem.Do(ps => {
				ps.BeforeBuildPages -= OnBeforeBuildPages;
				ps.DocumentChanged -= OnDocumentChanged;
				ps.AfterBuildPages -= OnAfterBuildPages;
				ps.ProgressReflector.PositionChanged -= OnProgressChanged;
				ps.CreateDocumentException -= OnCreateDocumentException;
			});
		}
		protected virtual void PreparePrintingSystem(PrintingSystemBase ps) {
			if(!(ps.Extender is PrintingSystemExtenderPrint))
				ps.Extender = new PrintingSystemExtenderPrint(PrintingSystem);
			if(!ps.ExportOptions.Options.ContainsKey(typeof(XpsExportOptions)))
				ps.ExportOptions.Options.Add(typeof(XpsExportOptions), new XpsExportOptions());
			ps.ReplaceService<IBackgroundService>(new BackgroundService());
		}
		protected void ResolveNewPages() {
			if(PrintingSystem == null)
				return;
			var invalidPages = Pages.Where(x => x.PageIndex >= PrintingSystem.PageCount).ToList();
			invalidPages.ForEach(x => Pages.Remove(x));
			foreach(Page page in PrintingSystem.Pages) {
				var pageIndex = page.Index;
				var pageModel = Pages.SingleOrDefault(x => x.PageIndex == pageIndex);
				if(pageModel == null) {
					Pages.Insert(page.Index, new PageViewModel(page));
				} else if(!object.ReferenceEquals(pageModel.Page, page)) {
					Pages[pageIndex].Page = page;
				}
			}
		}
		#endregion
		#region Document handlers
		protected virtual void OnBeforeBuildPages(object sender, EventArgs e) {
			IsCreating = true;
			Pages.Clear();
			var cancellationService = PrintingSystem.GetService<ICancellationService>();
			cancellationService.Do(x => x.StateChanged += OnCancelationServiceStateChanged);
			RaiseStartDocumentCreation();
		}
		protected virtual void OnDocumentChanged(object sender, EventArgs e) {
			ResolveNewPages();
			RaiseDocumentChanged();
		}
		protected virtual void OnAfterBuildPages(object sender, EventArgs e) {
			IsCreating = false;
			var cancellationService = PrintingSystem.GetService<ICancellationService>();
			cancellationService.Do(x => x.StateChanged -= OnCancelationServiceStateChanged);
			searchHelper = new SearchHelper();
			RaiseDocumentCreated();
			RaisePropertiesChanged(() => HasBookmarks, () => CanChangePageSettings);
		}
		protected virtual void OnCreateDocumentException(object sender, ExceptionEventArgs args) {
			if(!args.Handled) {
				args.Handled = true;
				RaiseDocumentException(args.Exception);
			}
		}
		void OnProgressChanged(object sender, EventArgs e) {
			RaisePropertyChanged(() => ((IProgressSettings)this).ProgressPosition);
			if(PrintingSystem.Document.State == XtraPrinting.Native.DocumentState.Created) {
				TimeSpan timeout = new TimeSpan(100000);
				Dispatcher.CurrentDispatcher.Invoke(new ThreadStart(delegate { }), timeout, DispatcherPriority.Background);
			}
		}
		internal void AfterDrawPages(int[] pageIndexes) {
			((DevExpress.DocumentView.IDocument)PrintingSystem).AfterDrawPages(this, pageIndexes);
		}
		void OnCancelationServiceStateChanged(object sender, EventArgs e) {
			var cancellationService = (ICancellationService)sender;
			IsCreating = cancellationService.CanBeCanceled() || PrintingSystem.Document.IsCreating;
		}
		#endregion
		#region bookmarks
		public bool HasBookmarks {
			get { return Document.Return(x => IsCreated && x.BookmarkNodes.Count != 0, () => false); }
		}
		public IEnumerable<PreviewBookmarkNode> GetBookmarks() {
			List<PreviewBookmarkNode> previewNodes = new List<PreviewBookmarkNode>();
			var currentId = 0;
			previewNodes.Add(new PreviewBookmarkNode(new BookmarkNode(Document.Name), currentId, -1));
			Document.Do(x => CollectBookmarks(x.BookmarkNodes, previewNodes, ref currentId));
			return previewNodes;
		}
		void CollectBookmarks(IBookmarkNodeCollection nodes, List<PreviewBookmarkNode> previewNodes, ref int currentId) {
			var parentId = currentId;
			foreach(BookmarkNode node in nodes) {
				currentId++;
				previewNodes.Add(new PreviewBookmarkNode(node, currentId, parentId));
				CollectBookmarks(node.Nodes, previewNodes, ref currentId);
			}
		}
		public void MarkBrick(PreviewBookmarkNode bookmark) {
			PrintingSystem.ClearMarkedBricks();
			BrickPagePair pair = bookmark.BookmarkNode.Pair;
			PrintingSystem.MarkBrick(pair.GetBrick(PrintingSystem.Pages), pair.GetPage(PrintingSystem.Pages));
		}
		public void ResetMarkedBricks() {
			PrintingSystem.Do(x => x.ClearMarkedBricks());
		}
		#endregion
		#region commands
		public virtual void CreateDocument() {
			if(source == null || PrintingSystem == null)
				return;
			if(source == link)
				link.CreateDocument(true);
			else if(source is Stream)
				PrintingSystem.LoadDocument((Stream)source);
			else if(source is string)
				PrintingSystem.LoadDocument((string)source);
		}
		public virtual void Print(PrintOptionsViewModel model) {
			var document = ((DevExpress.Printing.Native.PrintEditor.IPrintForm)model).Document;
			try {
				PrintingSystem.OnStartPrint(new PrintDocumentEventArgs(document));
				PrintingSystem.PrintDocument(document);
				PrintingSystem.OnEndPrint(EventArgs.Empty);
			} catch(Exception e) {
				RaiseDocumentException(e);
			}
		}
		public virtual void PrintDirect(string printerName = null) {
			try {
				PrintingSystem.Extender.Print(printerName);
			} catch(Exception e) {
				RaiseDocumentException(e);
			}
		}
		public virtual void Send(SendOptionsViewModel options) {
			IsExporting = true;
			string[] fileNames = new string[0];
			try {
				var controller = ExportOptionsControllerBase.GetControllerByOptions(options.ExportOptions);
				fileNames = controller.GetExportedFileNames(PrintingSystem, options.ExportOptions, options.FileName);
				((IDocumentViewModel)this).DefaultSendFormat = options.ExportFormat;
			} catch (Exception e) {
				RaiseDocumentException(e);
			}
			if(fileNames.Return(x => x.Length > 0, () => false)) {
				var sender = new EmailSender();
				sender.Send(fileNames, options.EmailOptions);
			}
			IsExporting = false;
		}
		public virtual void Export(ExportOptionsViewModel options) {
			IsExporting = true;
			string[] fileNames = new string[0];
			try {
				var controller = ExportOptionsControllerBase.GetControllerByOptions(options.ExportOptions);
				fileNames = controller.GetExportedFileNames(PrintingSystem, options.ExportOptions, options.FileName);
				((IDocumentViewModel)this).DefaultExportFormat = options.ExportFormat;
			} catch(Exception e) {
				RaiseDocumentException(e);
			}
			if(options.OpenFileAfterExport && fileNames.Return(x => x.Length > 0, () => false)) {
				if(File.Exists(fileNames[0])) {
					ProcessLaunchHelper.StartProcess(fileNames[0], false);
				}
			}
			IsExporting = false;
		}
		public virtual void SetWatermark(XpfWatermark xpfWatermark) {
			PrintingSystem.Watermark.CopyFrom(xpfWatermark);
		}
		public void Save(string filePath) {
			try {
				PrintingSystem.SaveDocument(filePath);
			} catch(Exception e) {
				RaiseDocumentException(e);
			}
		}
		public void StopPageBuilding() {
			Document.Do(x => x.StopPageBuilding());
		}
		#endregion
		internal void EnsureBrickOnPage(BrickPagePair pair, Action<BrickPagePair> onEnsured) {
			PrintingSystem.EnsureBrickOnPage(pair, onEnsured);
		}
		protected void RaiseDocumentCreated() {
			DocumentCreated.Do(x => x(this, EventArgs.Empty));
		}
		protected void RaiseStartDocumentCreation() {
			StartDocumentCreation.Do(x => x(this, EventArgs.Empty));
		}
		protected void RaiseDocumentChanged() {
			DocumentChanged.Do(x => x(this, EventArgs.Empty));
		}
		protected void RaiseDocumentException(Exception e) {
			DocumentException.Do(x=> x(this, new ExceptionEventArgs(e)));
		}
		public BrickPagePair PerformSearch(TextSearchParameter parameter) {
			return searchHelper.FindNext(PrintingSystem, parameter);
		}
		internal PSPrintDocument CreatePrintDocument() {
			PrinterSettings printerSettings = new PrinterSettings();
			printerSettings.MinimumPage = 1;
			printerSettings.MaximumPage = PrintingSystem.PageCount;
			PageScope pageScope = new PageScope(PrintingSystem.Extender.PredefinedPageRange, PrintingSystem.Document.PageCount);
			printerSettings.FromPage = pageScope.FromPage;
			printerSettings.ToPage = pageScope.ToPage;
			return new PSPrintDocument(PrintingSystem, PrintingSystem.Graph.PageBackColor, null, printerSettings, () => true) { PageRange = PrintingSystem.Extender.PredefinedPageRange };
		}
		internal static ExportOptionsViewModel GetExportViewModel(DocumentViewModel documentViewModel, ExportFormat format, IEnumerable<ExportFormat> hiddenFormats = null) {
			var model = new ExportOptionsViewModel(documentViewModel.PrintingSystem.ExportOptions) { HiddenExportFormats = hiddenFormats, AvailableExportModes = documentViewModel.PrintingSystem.Document.AvailableExportModes, ExportFormat = format };
			model.HiddenOptions = new ObservableCollection<ExportOptionKind>(documentViewModel.PrintingSystem.ExportOptions.HiddenOptions);
			return model;
		}
		internal static SendOptionsViewModel GetSendViewModel(DocumentViewModel documentViewModel, ExportFormat format, IEnumerable<ExportFormat> hiddenFormats = null) {
			var model = new SendOptionsViewModel(documentViewModel.PrintingSystem.ExportOptions) { HiddenExportFormats = hiddenFormats, AvailableExportModes = documentViewModel.PrintingSystem.Document.AvailableExportModes, ExportFormat = format };
			model.HiddenOptions = new ObservableCollection<ExportOptionKind>(documentViewModel.PrintingSystem.ExportOptions.HiddenOptions);
			return model;
		}
		internal static ScaleOptionsViewModel GetScaleViewModel(DocumentViewModel documentViewModel) {
			return new ScaleOptionsViewModel(documentViewModel.PrintingSystem.Document.ScaleFactor, documentViewModel.PrintingSystem.Document.AutoFitToPagesWidth);
		}
		internal static PrintOptionsViewModel GetPrintOptionsViewModel(DocumentViewModel documentViewModel) {
			return new PrintOptionsViewModel(documentViewModel);
		}
		public void Scale(ScaleOptionsViewModel model) {
			if(model.ScaleMode == ScaleMode.AdjustToPercent) {
				PrintingSystem.Document.ScaleFactor = (float)model.ScaleFactor;
			} else {
				PrintingSystem.Document.AutoFitToPagesWidth = model.PagesToFit;
				return;
			}
		}
	}
	public class ReportDocumentViewModel : DocumentViewModel, IReportDocument {
		internal IReport Report { get { return (IReport)base.Link; } }
		internal event ReportParametersRecievedEventHandler ReportParametersRecieved;
		internal ReportDocumentViewModel() : base() { }
		public override void Load(object source) {
			base.Load(source);
			RequestParameters();
		}
		void RequestParameters() {
			try {
				List<Parameter> parameters = new List<Parameter>();
				Report.CollectParameters(parameters, (parameter) => true);
				Report.RaiseParametersRequestBeforeShow(parameters.ConvertAll(x => ParameterInfoFactory.CreateWithoutEditor(x)));
				var dataContext = Report.GetService<DataContext>();
				var lookUpsProvider = new LookUpValuesProvider(parameters, dataContext);
				var models = ModelsCreator.CreateParameterModels(parameters, dataContext);
				ReportParametersRecieved.Do(x=> x(this, new ReportParametersRecievedEventArgs(models, lookUpsProvider)));
			} catch { }
		}
		public void Submit(IList<XtraReports.Parameters.Parameter> parameters) {
			try {
				Report.RaiseParametersRequestSubmit(parameters.Select(ParameterInfoFactory.CreateWithoutEditor).ToList(), true);
			} catch(Exception e) {
				Report.Do(x => x.PrintingSystemBase.OnCreateDocumentException(new ExceptionEventArgs(e)));
			}
		}
		protected override void PreparePrintingSystem(PrintingSystemBase ps) {
			base.PreparePrintingSystem(ps);
			if(!(Report is RemoteDocumentSource)) Report.PrintTool = new DevExpress.XtraReports.UI.ReportPrintToolWpf(Report, null);
			if(ps as RemotePrintingSystem == null)
				ps.ReplaceService(typeof(BackgroundPageBuildEngineStrategy), new DispatcherPageBuildStrategy());
		}
		protected internal override void Subscribe() {
			base.Subscribe();
			PrintingSystem.Do(ps => {
				(ps as RemotePrintingSystem).Do(x => x.GetRemoteParametersCompleted += OnGetRemoteParametersCompleted);
				ps.AfterChange += OnPrintingSystemChanged;
			});
		}
		protected internal override void Unsubscribe() {
			base.Unsubscribe();
			PrintingSystem.Do(ps => {
				(ps as RemotePrintingSystem).Do(x => x.GetRemoteParametersCompleted -= OnGetRemoteParametersCompleted);
				ps.AfterChange -= OnPrintingSystemChanged;
			});
		}
		void OnPrintingSystemChanged(object sender, ChangeEventArgs e) {
			if(e.EventName != DevExpress.XtraPrinting.SR.BrickClick)
				return;
			VisualBrick brick = e.ValueOf(DevExpress.XtraPrinting.SR.Brick) as VisualBrick;
			if(brick == null || !(brick.Value is DrillDownKey))
				return;
			DrillDownKey key = (DrillDownKey)brick.Value;
			IDrillDownServiceBase serv = ((IServiceProvider)Report).GetService(typeof(IDrillDownServiceBase)) as IDrillDownServiceBase;
			bool expanded;
			if(serv != null && serv.Keys.TryGetValue(key, out expanded)) {
				serv.Keys[key] = !expanded;
				RefreshDocument();
			}
		}
		void RefreshDocument() {
			Unsubscribe();
			Report.ReleasePrintingSystem();
			Report.PrintingSystemBase.PageSettings.Assign(PrintingSystem.PageSettings.Data);
			Report.PrintingSystemBase.PageSettings.IsPresetted = true;
			PreparePrintingSystem(Report.PrintingSystemBase);
			Report.PrintingSystemBase.AfterBuildPages += OnRefreshDocumentCompleted;
			IDrillDownServiceBase serv = ((IServiceProvider)Report).GetService<IDrillDownServiceBase>();
			if(serv != null)
				serv.IsDrillDowning = true;
			var printTool = Report.PrintTool;
			try {
				Report.PrintTool = null;
				Report.CreateDocument(true);
			} finally {
				Report.PrintTool = printTool;
				if(serv != null)
					serv.IsDrillDowning = false;
			}
		}
		void OnRefreshDocumentCompleted(object sender, EventArgs e) {
			Report.PrintingSystemBase.AfterBuildPages -= OnRefreshDocumentCompleted;
			var prevPrintingSystem = PrintingSystem;
			var updateStrategy = Report.PrintingSystemBase.GetService<IUpdateDrillDownReportStrategy>() ?? new UpdateDrillDownReportStrategy();
			updateStrategy.Update(Report, prevPrintingSystem);
			PrintingSystem = Report.PrintingSystemBase;
			PreparePrintingSystem(Report.PrintingSystemBase);
			prevPrintingSystem.Dispose();
			ResolveNewPages();
			RaiseDocumentChanged();
			Subscribe();
		}
		public override void PrintDirect(string printerName = null) {
			if(Report is RemoteDocumentSource) {
				RemotePrintDirect(printerName);
				return;
			}
			base.PrintDirect(printerName);
		}
		public override void Print(PrintOptionsViewModel model) {
			if(Report is RemoteDocumentSource) {
				RemotePrint(model);
				return;
			}
			base.Print(model);
		}
		public override void Export(ExportOptionsViewModel options) {
			IsExporting = true;
			if(Report is RemoteDocumentSource) {
				RemoteExport(options, (fileName) => {
					if(options.OpenFileAfterExport && !string.IsNullOrEmpty(fileName))
						ProcessLaunchHelper.StartProcess(fileName, false);
				});
				return;
			}
			base.Export(options);
		}
		public override void Send(SendOptionsViewModel options) {
			IsExporting = true;
			if(Report is RemoteDocumentSource) {
				RemoteExport(options, (fileName) => {
					if(!string.IsNullOrEmpty(fileName))
						new EmailSender().Send(new[] { fileName }, options.EmailOptions);
				});
				return;
			}
			base.Send(options);
		}
		string WriteRemoteDocument(byte[] data, string fileName) {
			Stream stream = null;
			try {
				stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
				stream.Write(data, 0, data.Length);
				stream.Close();
				return fileName;
			} catch(Exception e) {
				stream.Do(x => {
					stream.Close();
					File.Delete(fileName);
				});
				RaiseDocumentException(e);
				return string.Empty;
			}
		}
		#region remote reports
		void RemoteExport(ExportOptionsViewModelBase options, Action<string> afterExport) {
			var remoteOperationFactory = PrintingSystem.GetService<RemoteOperationFactory>();
			var exportOperation = remoteOperationFactory.CreateExportDocumentOperation(options.ExportFormat, PrintingSystem.ExportOptions);
			exportOperation.Completed += (s, e) => {
				if(e.Error != null) {
					RaiseDocumentException(e.Error);
				} else {
					WriteRemoteDocument(e.Data, options.FileName);
					afterExport(options.FileName);
				}
				IsExporting = false;
			};
			exportOperation.Start();
		}
		void RemotePrint(PrintOptionsViewModel model) {
			var remotePrintService = PrintingSystem.GetService<IRemotePrintService>();
			remotePrintService.Print(document => base.Print(model), () => ((DevExpress.Printing.Native.PrintEditor.IPrintForm)model).Document);
		}
		void RemotePrintDirect(string printerName) {
			var remotePrintService = PrintingSystem.GetService<IRemotePrintService>();
			remotePrintService.PrintDirect(0, PrintingSystem.PageCount - 1, printer => base.PrintDirect(printerName));
		}
		void OnGetRemoteParametersCompleted(object sender, GetRemoteParametersCompletedEventArgs e) {
			(PrintingSystem as RemotePrintingSystem).Do(x => x.GetRemoteParametersCompleted -= OnGetRemoteParametersCompleted);
			var models = ModelsCreator.CreateParameterModels(e.Parameters);
			ReportParametersRecieved.Do(x => x(this, new ReportParametersRecievedEventArgs(models, e.LookUpValuesProvider)));
		}
		#endregion
		public override void SetWatermark(XpfWatermark xpfWatermark) {
			base.SetWatermark(xpfWatermark);
			Report.Watermark.CopyFrom(xpfWatermark);
		}
		internal void HandleBrickClick(Page page, Brick brick) {
			var e = PrintingSystemBase.CreateEventArgs(SR.BrickClick, new object[,] {
				{SR.Brick, brick}, {SR.Page, page}
			});
			PrintingSystem.OnAfterChange(e);
		}
	}
	internal class ReportParametersRecievedEventArgs : EventArgs {
		public IList<ParameterModel> Parameters { get; private set; }
		public ILookUpValuesProvider LookUpValuesProvider { get; private set; }
		public ReportParametersRecievedEventArgs(IList<ParameterModel> parameters, ILookUpValuesProvider lookUpValuesProvider) {
			Parameters = parameters;
			LookUpValuesProvider = lookUpValuesProvider;
		}
	}
	internal delegate void ReportParametersRecievedEventHandler(object sender, ReportParametersRecievedEventArgs e);
}
