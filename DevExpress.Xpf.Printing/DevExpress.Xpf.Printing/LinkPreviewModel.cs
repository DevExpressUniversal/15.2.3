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
using System.Windows;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
#if SL
using DevExpress.Xpf.Printing.Export;
using System.ServiceModel;
using DevExpress.Utils;
using System.ComponentModel;
#else
using System.Diagnostics;
using System.IO;
using DevExpress.Xpf.Printing.Parameters.Models;
#endif
namespace DevExpress.Xpf.Printing {
	public class LinkPreviewModel : PrintingSystemPreviewModel {
		static readonly TimeSpan blockingExportDelay = TimeSpan.FromMilliseconds(100);
		#region Fields
		LinkBase link;
#if SL
		IModelProgressStrategy progressStrategy;
#endif
		#endregion
		#region Events
#if SL
		public event EventHandler<FaultEventArgs> PrintError;
		public event EventHandler<FaultEventArgs> ExportError;
#endif
		#endregion
		#region ctor
		public LinkPreviewModel()
			: this(null) {
		}
		public LinkPreviewModel(LinkBase link) {
			this.link = link;
			OnSourceChanged();
#if SL
			progressStrategy = CreateBaseProgressStrategy();
#endif
		}
		#endregion
		#region Properties
		public LinkBase Link {
			get {
				return link;
			}
			set {
				if(link == value)
					return;
				OnSourceChanging();
				link = value;
				OnSourceChanged();
			}
		}
		protected internal override PrintingSystemBase PrintingSystem {
			get { return link == null ? null : link.PrintingSystem; }
		}
#if SL
		public override int ProgressValue {
			get { return progressStrategy.ProgressValue; }
		}
		public override int ProgressMaximum {
			get { return progressStrategy.ProgressMaximum; }
		}
		public override bool ProgressVisibility {
			get { return progressStrategy.ProgressVisibility; }
		}
#endif
		protected override ExportOptions DocumentExportOptions {
			get { return PrintingSystem.ExportOptions; }
			set { PrintingSystem.ExportOptions.Assign(value); }
		}
		protected override AvailableExportModes DocumentExportModes {
			get {
				var exportModes = PrintingSystem.Document.AvailableExportModes;
				return new AvailableExportModes(
					exportModes.Rtf,
					exportModes.Html.Exclude(HtmlExportMode.DifferentFiles),
					exportModes.Image.Exclude(ImageExportMode.DifferentFiles),
					exportModes.Xls.Exclude(XlsExportMode.DifferentFiles),
					exportModes.Xlsx.Exclude(XlsxExportMode.DifferentFiles));
			}
		}
		protected override List<ExportOptionKind> DocumentHiddenOptions {
			get { return PrintingSystem.ExportOptions.HiddenOptions; }
		}
		public override bool IsEmptyDocument {
			get { return !IsCreating && PageCount==0; }
		}
		#endregion
		#region Methods
		protected override bool CanSetWatermark(object parameter) {
			return false;
		}
		protected override void SetWatermark(object parameter) {
			throw new NotImplementedException();
		}
#if SL        
		protected override void Refresh(object parameter) {
			throw new NotSupportedException();
		}
		protected override bool CanRefresh(object parameter) {
			return false;
		}
		public override void Clear() {
			throw new NotImplementedException();
		}
		public override void CreateDocument() {
			CreateDocument(false);
		}
		protected override void Print(object parameter) {
			Link.PrintCompleted += Link_PrintCompleted;
			PrintMethodSelector(parameter, Link.Print, delegate() { PrintPdf(parameter); });
		}
		protected override void PrintPdf(object Parameter) {
			DefaultPrintMode = PrintMode.Pdf;
			ExportToWindow(new PdfExportOptions() { ShowPrintDialogOnOpen = true });
		}
#endif
		protected override void Export(ExportFormat format) {
#if SL
			var stream = ShowSaveExportedFileDialog(format);
			if(stream == null)
				return;
			InitiateExport(() => {
				Link.ExportCompleted += Link_ExportCompleted;
				Link.StartExport(stream, DocumentExportOptions.GetByFormat(format), true);
			});
#else
			throw new NotImplementedException();
#endif
		}
#if SL
		protected override void ExportToWindow(ExportFormat format) {
			ExportToWindow(Helper.GetByFormat(PrintingSystem.ExportOptions, format));
		}
		void ExportToWindow(ExportOptionsBase exportOptions) {
			InitiateExport(() => {
				Link.ExportAndGetResultUriCompleted += Link_ExportAndGetResultUriCompleted;
				Link.ExportAndGetResultUriAsync(exportOptions);
			});
		}
		void InitiateExport(Action exportLink) {
			IsExporting = true;
			progressStrategy = CreateExportServiceProgressStrategy();
			if (progressStrategy.CanSetProgressValue)
				progressStrategy.ProgressValue = 0;
			if (progressStrategy.CanSetProgressVisibility)
				progressStrategy.ProgressVisibility = true;
			new Delayer(blockingExportDelay).Execute(exportLink);
		}
		IModelProgressStrategy CreateBaseProgressStrategy() {
			return new DelegateModelProgressStrategy(() => base.ProgressValue, () => base.ProgressMaximum, () => base.ProgressVisibility);
		}
		IModelProgressStrategy CreateExportServiceProgressStrategy() {
			Link.ExportStarted += Link_ExportStarted;
			Link.ExportProgress += Link_ExportProgress;
			return new ModelExportProgressStrategy(100, () => RaisePropertyChanged(() => ProgressValue),
				() => RaisePropertyChanged(() => ProgressVisibility));
		}
		void Link_ExportStarted(object sender, EventArgs e) {
			if(progressStrategy.CanSetProgressValue)
				progressStrategy.ProgressValue = 0;
			if(progressStrategy.CanSetProgressVisibility)
				progressStrategy.ProgressVisibility = true;
		}
		void Link_ExportProgress(object sender, DocumentExportingServiceProgressEventArgs e) {
			if(progressStrategy.CanSetProgressValue)
				progressStrategy.ProgressValue = e.ProgressPosition;
		}
		void Link_ExportCompleted(object sender, AsyncCompletedEventArgs e) {
			var link = (LinkBase)sender;
			LinkExportCompleted(link, e.Error);
			link.ExportCompleted -= Link_ExportCompleted;
		}
		void Link_ExportAndGetResultUriCompleted(object sender, ExportAndGetResultUriCompletedEventArgs e) {
			var link = (LinkBase)sender;
			LinkExportCompleted(link, e.Error);
			link.ExportAndGetResultUriCompleted -= Link_ExportAndGetResultUriCompleted;
			if(e.Error == null)
				DialogService.ShowHtmlPage(e.Result);
		}
		void LinkExportCompleted(LinkBase link, Exception fault) {
			if(progressStrategy.CanSetProgressVisibility)
				progressStrategy.ProgressVisibility = false;
			progressStrategy = CreateBaseProgressStrategy();
			link.ExportStarted -= Link_ExportStarted;
			link.ExportProgress -= Link_ExportProgress;
			IsExporting = false;
			if(fault != null){
				var showFault = true;
				if(ExportError != null){
					var args = new FaultEventArgs(fault);
					ExportError(this, args);
					showFault = !args.Handled;
				}
				if(showFault)
					DialogService.ShowError(fault.Message, PrintingLocalizer.GetString(PrintingStringId.Error));
			}
		}
		void Link_PrintCompleted(object sender, LinkPrintCompleteEventArgs e) {
			var link = (LinkBase)sender;
			link.PrintCompleted -= Link_PrintCompleted;
			if(e.Fault != null){
				var showFault = true;
				if(PrintError != null) {
					var args = new FaultEventArgs(e.Fault);
					PrintError(this, args);
					showFault = !args.Handled;
				}
				if(showFault)
					DialogService.ShowError(e.Fault.Message, PrintingLocalizer.GetString(PrintingStringId.Error));
			}
		}
#endif
		protected override void CreateDocument(bool buildPagesInBackground) {
			link.CreateDocument(buildPagesInBackground);
		}
		protected override bool CanShowParametersPanel(object parameter) {
			return false;
		}
		public override ParametersModel ParametersModel {
			get { return null; }
		}
#if SL
		protected override bool CanExport(object parameter) {
			return BuildPagesComplete && Link.CanCreateExportServiceClient && !IsExporting && base.CanExport(parameter);
		}
#endif
		protected override bool CanScale(object parameter) {
			return false;
		}
		protected override void Scale(object parameter) {
			throw new NotSupportedException();
		}
		protected override FrameworkElement VisualizePage(int pageIndex) {
			return link.VisualizePage(pageIndex);
		}
		protected override void HookPrintingSystem() {
			base.HookPrintingSystem();
			if(Link != null) {
				Link.CreateDocumentFinished += Link_CreateDocumentFinished;
			}
		}
		protected override void UnhookPrintingSystem() {
			base.UnhookPrintingSystem();
			if(Link != null) {
				Link.CreateDocumentFinished -= Link_CreateDocumentFinished;
			}
		}
		void Link_CreateDocumentFinished(object sender, EventArgs e) {
			ToggleDocumentMap();
		}
		#endregion
	}
}
