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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.DocumentServices.ServiceModel.Native;
using DevExpress.DocumentServices.ServiceModel.ServiceOperations;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Printing.Parameters;
using DevExpress.Xpf.Printing.Parameters.Models;
using DevExpress.Xpf.Printing.Parameters.Models.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Parameters.Native;
namespace DevExpress.Xpf.Printing {
	public class ReportServicePreviewModel : DocumentPreviewModelBase {
		#region Fields
		static readonly PageCompatibility PageCompatibility = PageCompatibility.WPF;
		TimeSpan statusUpdateInterval = Helper.DefaultStatusUpdateInterval;
		IReportServiceClient client;
		bool isCreating;
		bool isReportInformationRequested;
		InstanceIdentity instanceIdentity;
		IParameterContainer parameters = new DefaultValueParameterContainer();
		FrameworkElement pageContent;
		int pageCount;
		DocumentId documentId;
		string documentName;
		string[] printData;
		ReportServiceOperation serviceOperation;
		BinaryWriter exportWriter;
		bool canRefresh;
		DocumentMapTreeViewNode documentMapRootNode;
		readonly HighlightingService highlightingService = new HighlightingService();
		ExportOptions exportOptions;
		AvailableExportModes exportModes;
		Action<ExportId, byte[], Exception> completeSaveExportedDocumentDelegate;
		bool shouldRequestParameters;
		readonly ParametersModel parametersModel;
		readonly IPageSettingsConfiguratorService pageSetupConfigurator = new PageSettingsConfiguratorService();
		bool progressVisibility;
		int progressValue;
		PrintingSystemBase printingSystem;
		readonly ServiceClientCreator<IReportServiceClient, IReportServiceClientFactory> clientCreator =
			new ServiceClientCreator<IReportServiceClient, IReportServiceClientFactory>(address => new ReportServiceClientFactory(address));
		IWatermarkService watermarkService = new WatermarkService();
		XpfWatermark xpfWatermark = new XpfWatermark();
		bool isWatermarkChanged = false;
		Func<bool> startSaveExportedDocumentDelegate;
		bool autoShowParametersPanel = true;
		bool canChangePageSettings = false;
		#endregion
		#region Properties
#if DEBUGTEST
		internal string[] TEST_PrintData {
			get { return printData; }
			set { printData = value; }
		}
		internal PrintingSystemBase TEST_PrintingSystem {
			get { return printingSystem; }
		}
#endif
		XtraPageSettingsBase DocumentPageSettings {
			get {
				if(printingSystem == null)
					return null;
				return printingSystem.PageSettings;
			}
		}
		protected DocumentId DocumentId {
			get { return documentId; }
			set { documentId = value; }
		}
		protected IReportServiceClient Client {
			get { return client; }
		}
		internal bool CanCreateServiceClient {
			get {
				return clientCreator.CanCreateClient;
			}
		}
		public string ReportName {
			get {
				var reportNameIdentity = instanceIdentity as ReportNameIdentity;
				return reportNameIdentity != null ? reportNameIdentity.Value : null;
			}
			set {
				InstanceIdentity = new ReportNameIdentity(value);
			}
		}
		public InstanceIdentity InstanceIdentity {
			get {
				return instanceIdentity;
			}
			set {
				instanceIdentity = value;
				isReportInformationRequested = false;
				RaisePropertyChanged(() => ReportName);
				RaisePropertyChanged(() => InstanceIdentity);
			}
		}
		public string ServiceUri {
			get {
				return clientCreator.ServiceUri;
			}
			set {
				clientCreator.ServiceUri = value;
			}
		}
		public IReportServiceClientFactory ServiceClientFactory {
			get {
				return clientCreator.Factory;
			}
			set {
				clientCreator.Factory = value;
			}
		}
		[Obsolete("This property has become obsolete. To specify the default parameter values, call the CreateDocument(DefaultValueParameterCotnainer parameters) method overload.")]
		public IParameterContainer Parameters {
			get {
				return parameters;
			}
		}
		public TimeSpan StatusUpdateInterval {
			get {
				return statusUpdateInterval;
			}
			set {
				if(statusUpdateInterval != value) {
					statusUpdateInterval = value;
					RaisePropertyChanged(() => StatusUpdateInterval);
				}
			}
		}
		public bool AutoShowParametersPanel { get { return autoShowParametersPanel; } set { autoShowParametersPanel = value; } }
		public override ParametersModel ParametersModel {
			get {
				return parametersModel;
			}
		}
		internal bool ShouldRequestParameters {
			get { return shouldRequestParameters; }
		}
		internal IWatermarkService WatermarkService {
			get { return watermarkService; }
			set { watermarkService = value; }
		}
		public override bool IsSetWatermarkVisible { get { return true; } }
		public override bool IsOpenButtonVisible { get { return false; } }
		public override bool IsSaveButtonVisible { get { return false; } }
		public override bool IsSendVisible { get { return false; } }
		#endregion
		#region Events
		public event EventHandler<SimpleFaultEventArgs> GetPageError;
		public event EventHandler<FaultEventArgs> CreateDocumentError;
		public event EventHandler<FaultEventArgs> PrintError;
		public event EventHandler<FaultEventArgs> ExportError;
		public event EventHandler<ParametersMergingErrorEventArgs> ParametersMergingError;
		#endregion
		#region Constructors
		public ReportServicePreviewModel()
			: this(null) {
		}
		public ReportServicePreviewModel(string serviceUri) {
			ServiceUri = serviceUri;
			parametersModel = ParametersModel.CreateParametersModel();
			ParametersModel.Submit += OnSubmit;
			watermarkService.EditCompleted += watermarkService_EditCompleted;
		}
		#endregion
		#region Methods
		public void CreateDocument() {
			CreateDocumentInternal(null);
		}
		public void CreateDocument(DefaultValueParameterContainer parameters) {
			this.parameters = parameters;
			CreateDocumentInternal(null);
		}
		protected void CreateDocumentInternal(object customArgs) {
			ValidateInstanceIdentity();
			Clear();
			client = CreateClient();
			client.GetPagesCompleted += client_GetPagesCompleted;
			var wm = isWatermarkChanged ? xpfWatermark : null;
			if(parameters is ClientParameterContainer) {
				ParametersModel.Parameters.ForEach(x => x.UpdateParameter(UpdateAction.Submit));
				var parametersStub = new ReportParameterContainer() { Parameters = ParametersModel.GetReportParameterStubs() };
				parameters = new ClientParameterContainer(parametersStub);
			}
			var buildArgs = Helper.CreateReportBuildArgs(parameters, DocumentPageSettings, wm, customArgs);
			CreateDocumentCore(ConstructCreateDocumentOperation(buildArgs));
		}
		protected virtual CreateDocumentOperation ConstructCreateDocumentOperation(ReportBuildArgs buildArgs) {
			return new CreateDocumentOperation(client, InstanceIdentity, buildArgs, !isReportInformationRequested, statusUpdateInterval);
		}
		protected virtual void ValidateInstanceIdentity() {
			if(InstanceIdentity == null) {
				throw new InvalidOperationException("Can't create a document because InstanceIdentity or ReportName is not set.");
			}
			var reportNameIdentity = InstanceIdentity as ReportNameIdentity;
			if(reportNameIdentity != null && string.IsNullOrEmpty(reportNameIdentity.Value)) {
				throw new InvalidOperationException("Can't create a document because ReportName is not set.");
			}
		}
		public void Clear() {
			Clear(false);
		}
		void Clear(bool enableRefresh) {
			if(exportWriter != null) {
				exportWriter.Dispose();
				exportWriter = null;
			}
			if(serviceOperation != null) {
				serviceOperation.Abort();
				serviceOperation = null;
			}
			ClearDocumentOnServer();
			if(client != null) {
				client.GetPagesCompleted -= client_GetPagesCompleted;
				client.GetDocumentDataCompleted -= client_GetDocumentDataCompleted;
				client.CloseAsync();
				client = null;
			}
			documentMapRootNode = null;
			IsDocumentMapVisible = false;
			canRefresh = enableRefresh;
			DocumentExportOptions = null;
			exportModes = null;
			IsLoading = false;
			SetProgressVisibility(false);
			IsCreating = false;
			printData = null;
			ClearParameters();
			SetCurrentPageIndex(-1);
			SetPageCount(0);
			RaisePropertyChanged(() => IsEmptyDocument);
			RaiseAllCommandsCanExecuteChanged();
		}
		protected virtual void ClearDocumentOnServer() {
			if(documentId != null) {
				client.ClearDocumentAsync(documentId, null);
				documentId = null;
			}
		}
		void CreateDocumentCore(CreateDocumentOperation operation) {
			Debug.Assert(CurrentPageIndex == -1);
			Debug.Assert(PageCount == 0);
			operation.GetReportParameters += createDocumentOperation_GetReportParameters;
			operation.Started += createDocumentOperation_Started;
			operation.Progress += createDocumentOperation_Progress;
			operation.Completed += createDocumentOperation_Completed;
			operation.CanStopChanged += createDocumentOperation_CanStopChanged;
			serviceOperation = operation;
			serviceOperation.Start();
			IsCreating = true;
			IsLoading = true;
			RaiseAllCommandsCanExecuteChanged();
		}
		void createDocumentOperation_Started(object sender, CreateDocumentStartedEventArgs e) {
			Debug.Assert(e.DocumentId != null);
			documentId = e.DocumentId;
			StartShowingProgress();
		}
		void createDocumentOperation_Progress(object sender, CreateDocumentProgressEventArgs e) {
			Debug.Assert(documentId != null);
			if(pageCount == 0 && e.PageCount > 0) {
				SetPageCount(e.PageCount);
				if(!IsValidPageIndex(CurrentPageIndex)) {
					CurrentPageIndex = 0;
				}
			} else
				SetPageCount(e.PageCount);
			SetProgressValue(e.ProgressPosition);
			RaiseNavigationCommandsCanExecuteChanged();
		}
		void createDocumentOperation_Completed(object sender, CreateDocumentCompletedEventArgs e) {
			Debug.Assert(e.Cancelled == false);
			if(e.Error != null) {
				Clear(true);
				RaiseOperationError(e.Error, CreateDocumentError);
				return;
			}
			IsCreating = false;
			IsLoading = false;
			if(documentId == null)
				return;
			RefreshPageOnReportBuildCompleted();
			client.GetDocumentDataCompleted += client_GetDocumentDataCompleted;
			client.GetDocumentDataAsync(documentId, null);
			RaisePropertyChanged(() => IsEmptyDocument);
		}
		protected virtual void RefreshPageOnReportBuildCompleted() {
			if(PageCount > 0)
				client.GetPagesAsync(documentId, new[] { CurrentPageIndex }, PageCompatibility, CurrentPageIndex);
		}
		void client_GetDocumentDataCompleted(object sender, ScalarOperationCompletedEventArgs<DocumentData> e) {
			var client = sender as IReportServiceClient;
			client.GetDocumentDataCompleted -= client_GetDocumentDataCompleted;
			Debug.Assert(!e.Cancelled);
			if(e.Error != null) {
				RaiseOperationError(e.Error, CreateDocumentError);
				return;
			}
			serviceOperation = null;
			if(PageCount != 0) {
				var documentData = e.Result;
				canChangePageSettings = documentData.CanChangePageSettings;
				documentName = documentData.Name;
				documentMapRootNode = documentData.DocumentMap;
				RaisePropertyChanged(() => DocumentMapRootNode);
				commands[PrintingSystemCommand.DocumentMap].RaiseCanExecuteChanged();
				IsDocumentMapVisible = true;
				CreatePrintingSystemIfNeeded();
				UnsubscribePageSettingsChangedEvent();
				Helper.DeserializePageSettings(DocumentPageSettings, e.Result.SerializedPageData);
				SubscribePageSettingsChangedEvent();
				exportModes = documentData.AvailableExportModes;
				Helper.DeserializeExportOptions(DocumentExportOptions, documentData.ExportOptions);
				DocumentExportOptions.HiddenOptions.Clear();
				DocumentExportOptions.HiddenOptions.AddRange(documentData.HiddenOptions.FlagsToList());
				DefaultExportFormat = ExportFormatConverter.ToExportFormat(DocumentExportOptions.PrintPreview.DefaultExportFormat);
				RestoreWatermark(documentData.SerializedWatermark);
			}
			SetProgressVisibility(false);
			RaiseAllCommandsCanExecuteChanged();
		}
		void CreatePrintingSystemIfNeeded() {
			if(printingSystem != null)
				return;
			printingSystem = new PrintingSystemBase();
			SubscribePageSettingsChangedEvent();
		}
		void createDocumentOperation_CanStopChanged(object sender, EventArgs e) {
			commands[PrintingSystemCommand.StopPageBuilding].RaiseCanExecuteChanged();
		}
		void createDocumentOperation_GetReportParameters(object sender, CreateDocumentReportParametersEventArgs e) {
			var createDocumentOperation = (CreateDocumentOperation)sender;
			var reportParameters = new ClientParameterContainer(e.ReportParameters);
			if(parameters is DefaultValueParameterContainer) {
				CopyParameters((DefaultValueParameterContainer)parameters, reportParameters);
				createDocumentOperation.SetParameters(reportParameters);
			}
			parameters = reportParameters;
			e.ReportParameters.Parameters = reportParameters.ToParameterStubs();
			PrepareParametersModel();
			IsParametersPanelVisible = AutoShowParametersPanel && ParametersModel.HasVisibleParameters;
			if(reportParameters != null) {
				RaisePropertyChanged(() => ParametersModel);
				commands[PrintingSystemCommand.Parameters].RaiseCanExecuteChanged();
			}
			shouldRequestParameters = e.ReportParameters.ShouldRequestParameters;
			isReportInformationRequested = true;
		}
		void PrepareParametersModel() {
			var models = ModelsCreator.CreateParameterModels(parameters);
			var lookUpValuesProvider = new RemoteLookUpValuesProvider((ClientParameterContainer)parameters, InstanceIdentity, () => Client);
			ParametersModel.AssignParameters(models);
			ParametersModel.LookUpValuesProvider = lookUpValuesProvider;
		}
		void OnSubmit(object sender, EventArgs e) {
			CreateDocument();
		}
		void CopyParameters(DefaultValueParameterContainer source, ClientParameterContainer dest) {
			Exception error = null;
			if(!source.CopyTo(dest, out error) && ParametersMergingError != null)
				ParametersMergingError(this, new ParametersMergingErrorEventArgs(error));
		}
		void ClearParameters() {
			if(isReportInformationRequested)
				return;
			var clientParameters = parameters as ClientParameterContainer;
			if(clientParameters == null)
				return;
			var defaultParameters = new DefaultValueParameterContainer();
			defaultParameters.CopyFrom(clientParameters);
			parameters = defaultParameters;
		}
		void exportDocumentOperation_Started(object sender, ExportDocumentStartedEventArgs e) {
			StartShowingProgress();
		}
		void exportDocumentOperation_Progress(object sender, ExportDocumentProgressEventArgs e) {
			SetProgressValue(e.ProgressPosition);
		}
		void exportDocumentOperation_Completed(object sender, ExportDocumentCompletedEventArgs e) {
			Debug.Assert(serviceOperation != null);
			Debug.Assert(e.Cancelled == false);
			if(e.Error == null) {
				Debug.Assert(e.OperationId != null);
			} else {
				RaiseOperationError(e.Error, ExportError);
			}
			serviceOperation = null;
			SetProgressVisibility(false);
			RaiseStopPrintExportCommandsCanExecuteChanged();
			if(completeSaveExportedDocumentDelegate != null) {
				completeSaveExportedDocumentDelegate(e.OperationId, e.Data, e.Error);
				completeSaveExportedDocumentDelegate = null;
			}
		}
		void printDocumentOperation_Completed(object sender, PrintDocumentCompletedEventArgs e) {
			Debug.Assert(!e.Cancelled);
			var printDocumentOperation = (PrintDocumentOperation)sender;
			printDocumentOperation.Completed -= printDocumentOperation_Completed;
			SetProgressVisibility(false);
			if(e.Error != null) {
				RaiseOperationError(e.Error, PrintError);
			} else {
				printData = e.Pages;
				ContinuePrint(printDocumentOperation.IsDirectPrinting);
			}
			serviceOperation = null;
			RaiseStopPrintExportCommandsCanExecuteChanged();
		}
		void client_GetPagesCompleted(object sender, ScalarOperationCompletedEventArgs<byte[]> e) {
			if((int)e.UserState != CurrentPageIndex) {
				return;
			}
			IsLoading = false;
			if(e.Error != null) {
				IsIncorrectPageContent = true;
				if(GetPageError != null) {
					GetPageError(this, new SimpleFaultEventArgs(e.Error));
				}
			} else {
				try {
					var pages = Helper.DeserializePages(e.Result);
					SetPageContent(XamlReaderHelper.Load(pages.First()));
					IsIncorrectPageContent = false;
				} catch(XamlParseException) {
					IsIncorrectPageContent = true;
				}
			}
		}
		void printingSystem_PageSettingsChanged(object sender, EventArgs e) {
			if(serviceOperation == null)
				CreateDocument();
		}
		IReportServiceClient CreateClient() {
			Debug.Assert(client == null);
			return clientCreator.Create();
		}
		void SetPageCount(int pageCount) {
			Debug.Assert(CurrentPageIndex == 0 || CurrentPageIndex < pageCount);
			this.pageCount = pageCount;
			RaisePropertyChanged(() => PageCount);
		}
		void SetPageContent(FrameworkElement pageContent) {
			if(this.pageContent == pageContent)
				return;
			this.pageContent = pageContent;
			RaisePropertyChanged(() => PageContent);
			RaisePropertyChanged(() => PageViewWidth);
			RaisePropertyChanged(() => PageViewHeight);
		}
		void RaiseStopPrintExportCommandsCanExecuteChanged() {
			commands[PrintingSystemCommand.StopPageBuilding].RaiseCanExecuteChanged();
			commands[PrintingSystemCommand.Print].RaiseCanExecuteChanged();
			commands[PrintingSystemCommand.PageSetup].RaiseCanExecuteChanged();
			commands[PrintingSystemCommand.ExportFile].RaiseCanExecuteChanged();
			commands[PrintingSystemCommand.Watermark].RaiseCanExecuteChanged();
			commands[PrintingSystemCommand.PrintDirect].RaiseCanExecuteChanged();
		}
		void ShowPageSetupDialog() {
			pageSetupConfigurator.Configure(DocumentPageSettings, DialogService.GetParentWindow());
		}
		void StartShowingProgress() {
			SetProgressValue(0);
			SetProgressVisibility(true);
		}
		void SubscribePageSettingsChangedEvent() {
			printingSystem.PageSettingsChanged += printingSystem_PageSettingsChanged;
		}
		void UnsubscribePageSettingsChangedEvent() {
			printingSystem.PageSettingsChanged -= printingSystem_PageSettingsChanged;
		}
		void SetProgressVisibility(bool value) {
			if(progressVisibility == value)
				return;
			progressVisibility = value;
			RaisePropertyChanged(() => ProgressVisibility);
		}
		void SetProgressValue(int value) {
			if(progressValue == value)
				return;
			progressValue = value;
			RaisePropertyChanged(() => ProgressValue);
		}
		void RestoreWatermark(byte[] watermark) {
			if(xpfWatermark == null || watermark == null)
				return;
			using(var stream = new MemoryStream(watermark)) {
				try {
					xpfWatermark.RestoreFromStream(stream);
					isWatermarkChanged = false;
				} catch {
					IsIncorrectPageContent = true;
				}
			}
		}
		bool IsValidPageIndex(int pageIndex) {
			return pageIndex >= 0 && pageIndex < PageCount;
		}
		#endregion
		#region DocumentPreviewModelBase overrides
		public override bool IsCreating {
			get {
				return isCreating;
			}
			protected set {
				if(isCreating == value)
					return;
				isCreating = value;
				RaisePropertyChanged(() => IsCreating);
			}
		}
		public override bool IsEmptyDocument {
			get { return documentId != null && PageCount == 0; }
		}
		protected override ExportOptions DocumentExportOptions {
			get {
				if(exportOptions == null)
					exportOptions = new ExportOptionsContainer();
				return exportOptions;
			}
			set {
				exportOptions = value;
				commands[PrintingSystemCommand.ExportFile].RaiseCanExecuteChanged();
			}
		}
		protected override AvailableExportModes DocumentExportModes {
			get { return exportModes; }
		}
		protected override List<ExportOptionKind> DocumentHiddenOptions {
			get { return exportOptions.HiddenOptions; }
		}
		public override int PageCount {
			get { return pageCount; }
		}
		public override bool ProgressVisibility {
			get { return progressVisibility; }
		}
		public override int ProgressMaximum {
			get { return 100; }
		}
		public override int ProgressValue {
			get { return progressValue; }
		}
		public override bool ProgressMarqueeVisibility {
			get { return false; }
		}
		public override DocumentMapTreeViewNode DocumentMapRootNode {
			get { return documentMapRootNode; }
		}
		protected override IHighlightingService HighlightingService {
			get { return highlightingService; }
		}
		protected override FrameworkElement GetPageContent() {
			return pageContent;
		}
		protected override bool CanScale(object parameter) {
			return false;
		}
		protected override void Scale(object parameter) {
			throw new NotSupportedException("Scaling is not supported by this Preview Model");
		}
		protected override bool CanShowSearchPanel(object parameter) {
			return !IsCreating && serviceOperation == null && PageCount > 0;
		}
		protected override void Print(object parameter) {
			StartPrinting(parameter, false);
		}
		protected void StartPrinting(object parameter, bool isDirectPrinting) {
			Debug.Assert(documentId != null);
			Debug.Assert(PageCount > 0);
			Debug.Assert(serviceOperation == null);
			if(printData == null) {
				var printDocumentOperation = new PrintDocumentOperation(client, documentId, PageCompatibility, documentName, statusUpdateInterval, isDirectPrinting);
				printDocumentOperation.Completed += printDocumentOperation_Completed;
				printDocumentOperation.Started += printDocumentOperation_Started;
				printDocumentOperation.Progress += printDocumentOperation_Progress;
				serviceOperation = printDocumentOperation;
				serviceOperation.Start();
				RaiseStopPrintExportCommandsCanExecuteChanged();
			} else {
				ContinuePrint(isDirectPrinting);
			}
		}
		void printDocumentOperation_Progress(object sender, PrintDocumentProgressEventArgs e) {
			SetProgressValue(e.ProgressPosition);
		}
		void printDocumentOperation_Started(object sender, PrintDocumentStartedEventArgs e) {
			StartShowingProgress();
		}
		void OnPrintingCancelled(object sender, EventArgs e) {
			if(serviceOperation != null) {
				Debug.Assert(serviceOperation is PrintDocumentOperation);
				Debug.Assert(serviceOperation.CanStop);
				serviceOperation.Stop();
			}
		}
		void ContinuePrint(bool isDirectPrinting) {
			Debug.Assert(printData != null);
			var documentPrinter = new DocumentPrinter();
			ReadonlyPageData[] pageData = new ReadonlyPageData[] { new ReadonlyPageData(DocumentPageSettings.Data) };
			if(isDirectPrinting)
				documentPrinter.PrintDirect(new XamlDocumentPaginator(printData), pageData, documentName, true);
			else
				documentPrinter.PrintDialog(new XamlDocumentPaginator(printData), pageData, documentName, true);
		}
		protected override bool CanPrint(object parameter) {
			return serviceOperation == null && PageCount > 0;
		}
		protected override void PageSetup(object parameter) {
			ShowPageSetupDialog();
		}
		protected override bool CanPageSetup(object parameter) {
			return serviceOperation == null && canChangePageSettings && PageCount > 0;
		}
		protected override void BeginExport(ExportFormat format) {
			Debug.Assert(documentId != null);
			Debug.Assert(serviceOperation == null);
			startSaveExportedDocumentDelegate = () => StartExportShowSaveFileDialog(format);
		}
		protected override void Export(ExportFormat format) {
			Export(format, null);
		}
		protected void Export(ExportFormat format, object customArgs) {
			ExportConfiguredCompleted(new ExportDocumentOperation(client, documentId, format, DocumentExportOptions, StatusUpdateInterval, customArgs));
		}
		protected override void EndExport() {
			startSaveExportedDocumentDelegate = null;
			completeSaveExportedDocumentDelegate = ExportCompletedSaveData;
		}
		bool StartExportShowSaveFileDialog(ExportFormat format) {
			var stream = ShowSaveExportedFileDialog(format);
			var result = stream != null;
			if(result)
				exportWriter = new BinaryWriter(stream);
			return result;
		}
		void ExportCompletedSaveData(ExportId exportId, byte[] data, Exception error) {
			Debug.Assert(exportWriter != null);
			try {
				if(error == null)
					exportWriter.Write(data);
			} finally {
				exportWriter.Dispose();
				exportWriter = null;
			}
		}
		void ExportConfiguredCompleted(ExportDocumentOperation exportDocumentOperation) {
			if(startSaveExportedDocumentDelegate != null && !startSaveExportedDocumentDelegate())
				return;
			exportDocumentOperation.Started += exportDocumentOperation_Started;
			exportDocumentOperation.Progress += exportDocumentOperation_Progress;
			exportDocumentOperation.Completed += exportDocumentOperation_Completed;
			serviceOperation = exportDocumentOperation;
			serviceOperation.Start();
			RaiseStopPrintExportCommandsCanExecuteChanged();
		}
		protected override bool CanExport(object parameter) {
			return serviceOperation == null && base.CanExport(parameter);
		}
		protected override void Stop(object parameter) {
			serviceOperation.Stop();
		}
		protected override bool CanStop(object parameter) {
			return serviceOperation != null && serviceOperation.CanStop;
		}
		protected override void OnCurrentPageIndexChanged() {
			Debug.Assert(CurrentPageIndex == -1 || documentId != null);
			if(documentId != null) {
				IsLoading = true;
				if(pageContent != null)
					pageContent = new System.Windows.Controls.Canvas() { Width = pageContent.Width, Height = pageContent.Height };
				RaisePropertyChanged(() => PageContent);
				client.GetPagesAsync(documentId, new[] { CurrentPageIndex }, PageCompatibility, CurrentPageIndex);
			}
		}
		protected override bool CanShowDocumentMap(object parameter) {
			return serviceOperation == null && DocumentMapRootNode != null;
		}
		protected override bool CanShowParametersPanel(object parameter) {
			return ParametersModel.Parameters.Count != 0;
		}
		protected override bool IsHitTestResult(FrameworkElement element) {
			return VisualHelper.GetIsVisualBrickBorder(element);
		}
		protected override bool CanSetWatermark(object parameter) {
			return documentId != null && printingSystem != null && serviceOperation == null && !IsCreating;
		}
		protected override void SetWatermark(object parameter) {
			if(printingSystem == null || printingSystem.PageSettings == null) {
				return;
			}
			Window ownerWindow = DialogService.GetParentWindow();
			watermarkService.Edit(ownerWindow, printingSystem.PageSettings, PageCount, xpfWatermark);
		}
		void watermarkService_EditCompleted(object sender, WatermarkServiceEventArgs e) {
			if(e.IsWatermarkAssigned == true) {
				xpfWatermark = e.Watermark;
				isWatermarkChanged = true;
				CreateDocument();
			}
		}
		protected override void PrintDirect(object parameter) {
			StartPrinting(parameter, true);
		}
		protected override bool CanPrintDirect(object parameter) {
			return serviceOperation == null && PageCount > 0;
		}
		protected override void Send(object parameter) {
			throw new NotImplementedException();
		}
		protected override bool CanSend(object parameter) {
			return false;
		}
		protected override void Open(object parameter) {
			throw new NotImplementedException();
		}
		protected override bool CanOpen(object parameter) {
			return false;
		}
		protected override void Save(object parameter) {
			throw new NotImplementedException();
		}
		protected override bool CanSave(object parameter) {
			return false;
		}
		#endregion
	}
}
