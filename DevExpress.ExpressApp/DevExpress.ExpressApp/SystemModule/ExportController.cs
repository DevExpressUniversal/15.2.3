#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing.Imaging;
using System.IO;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using System.Linq;
using DevExpress.Export;
namespace DevExpress.ExpressApp.SystemModule {
	public interface IExportableAnalysisEditor {
		IList<PrintingSystemCommand> ExportTypes { get; }
		IPrintable Printable { get; set; }
		event EventHandler<PrintableChangedEventArgs> PrintableChanged;
	}
	public interface IExportable {
		List<ExportTarget> SupportedExportFormats { get; }
		IPrintable Printable { get; }
		void OnExporting(); 
		event EventHandler<PrintableChangedEventArgs> PrintableChanged;
	}
	public interface IDataAwareExportable { }
	public interface IDataAwareExportableXls : IDataAwareExportable {
		void Export(Stream stream, XlsExportOptionsEx options);
	}
	public interface IDataAwareExportableXlsx : IDataAwareExportable {
		void Export(Stream stream, XlsxExportOptionsEx options);
	}
	public interface IDataAwareExportableCsv : IDataAwareExportable {
		void Export(Stream stream, CsvExportOptionsEx options);
	}
	public interface IStreamProvider {
		Stream GetExportStream(PrintingSystemCommand exportType);
	}
	public class MemoryStreamProvider : IStreamProvider {
		#region IStreamProvider Members
		public Stream GetExportStream(PrintingSystemCommand exportType) {
			return new MemoryStream();
		}
		#endregion
	}
	public class CustomExportEventArgs : HandledEventArgs {
		private IPrintable printable;
		private ExportTarget exportTarget;
		private Stream stream;
		public IPrintable Printable { get { return printable; } }
		public Stream Stream { get { return stream; } }
		public ExportTarget ExportTarget { get { return exportTarget; } }
		public ExportOptionsBase ExportOptions { get; set; }
		public CustomExportEventArgs(IPrintable printable, Stream stream, ExportTarget exportTarget, ExportOptionsBase exportOptions) { 
			this.printable = printable;
			this.stream = stream;
			this.exportTarget = exportTarget;
			ExportOptions = exportOptions;
		}
	}
	public class CustomGetDefaultFileNameEventArgs : EventArgs {
		public CustomGetDefaultFileNameEventArgs(string fileName) {
			this.FileName = fileName;
		}
		public string FileName { get; set; }
	}
	public abstract class ExportController : ViewController {
		public interface IComponentExporter : IDisposable {
			void Export(ExportTarget target, Stream stream);
			void Export(ExportTarget target, Stream stream, ExportOptionsBase options);
		}
		public class ComponentExporterImpl : IComponentExporter {
			ComponentExporter componentExporter;
			public ComponentExporterImpl(IPrintable printable) {
				Guard.ArgumentNotNull(printable, "printable");
				componentExporter = new ComponentExporter(printable); ;
			}
			public void Export(ExportTarget target, Stream stream) {
				componentExporter.Export(target, stream);
			}
			public void Export(ExportTarget target, Stream stream, ExportOptionsBase options) {
				componentExporter.Export(target, stream, options);
			}
			public void Dispose() {
				if(componentExporter != null) {
					componentExporter.Dispose();
					componentExporter = null;
				}
			}
		}
		public const string ActionCaption_Unknown = "Unknown";
		private SingleChoiceAction exportAction;
		private IExportable exportable;
		private bool isAutoUpdatingExportable = false;
		private bool autoUpdateExportable = true;
		private void exportAction_OnExecute(Object sender, SingleChoiceActionExecuteEventArgs e) {
			Guard.ArgumentNotNull(e.SelectedChoiceActionItem.Data, "args.SelectedChoiceActionItem.Data");
			ExportTarget exportTarget = (ExportTarget)e.SelectedChoiceActionItem.Data;
			Export(exportTarget);
		}
		private void view_EditorChanged(object sender, EventArgs e) {
			UpdateExportable();
		}
		private void exportable_PrintableChanged(object sender, PrintableChangedEventArgs e) {
			RefreshExportAction();
		}
		private string GetItemImage(ExportTarget exportTarget) {
			return "Action_Export_To" + Enum.GetName(typeof(ExportTarget), exportTarget);
		}
		private ExportOptionsBase GetExportOptions(ExportTarget exportTarget, ImageFormat imageFormat) {
			switch(exportTarget) {
				case ExportTarget.Csv:
					CsvExportOptionsEx csvExportOptions = new CsvExportOptionsEx();
					csvExportOptions.ExportType = ExportSettings.DefaultExportType;
					csvExportOptions.Encoding = System.Text.Encoding.UTF8;
					return csvExportOptions;
				case ExportTarget.Xls:
					XlsExportOptionsEx xlsExportOptions = new XlsExportOptionsEx();
					xlsExportOptions.ExportType = ExportSettings.DefaultExportType;
					return xlsExportOptions;
				case ExportTarget.Xlsx:
					XlsxExportOptionsEx xlsxExportOptions = new XlsxExportOptionsEx();
					xlsxExportOptions.ExportType = ExportSettings.DefaultExportType;
					return xlsxExportOptions;
				case ExportTarget.Text:
					return new TextExportOptions(TextExportOptions.DefaultSeparator, System.Text.Encoding.UTF8);
				case ExportTarget.Mht:
					return new MhtExportOptions("utf-8", View.Caption, true);
				case ExportTarget.Html:
					return new HtmlExportOptions("utf-8", View.Caption, true);
				case ExportTarget.Image:
					return new ImageExportOptions(imageFormat);
				default: return null;
			}
		}
		private void UpdateExportable() {
			ListView listView = View as ListView;
			if(AutoUpdateExportable && (listView != null)) {
				isAutoUpdatingExportable = true;
				try {
					Exportable = listView.Editor as IExportable;
				}
				finally {
					isAutoUpdatingExportable = false;
				}
			}
		}
		protected string GetItemCaption(ExportTarget exportTarget) {
			switch(exportTarget) {
				case ExportTarget.Csv:
					return PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportCsv_Caption);
				case ExportTarget.Mht:
					return PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportMht_Caption);
				case ExportTarget.Xls:
					return PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportXls_Caption);
				case ExportTarget.Xlsx:
					return PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportXlsx_Caption);
				case ExportTarget.Pdf:
					return PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportPdf_Caption);
				case ExportTarget.Rtf:
					return PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportRtf_Caption);
				case ExportTarget.Text:
					return PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportTxt_Caption);
				case ExportTarget.Html:
					return PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportHtm_Caption);
				case ExportTarget.Image:
					return PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportGraphic_Caption);
			}
			return CaptionHelper.GetLocalizedText("Captions", ActionCaption_Unknown);
		}
		protected string GetDefaultExtension(ExportTarget exportTarget) {
			switch(exportTarget) {
				case ExportTarget.Csv: return "csv";
				case ExportTarget.Html: return "htm";
				case ExportTarget.Image: return "png";
				case ExportTarget.Mht: return "mht";
				case ExportTarget.Pdf: return "pdf";
				case ExportTarget.Rtf: return "rtf";
				case ExportTarget.Text: return "txt";
				case ExportTarget.Xls: return "xls";
				case ExportTarget.Xlsx: return "xlsx";
			}
			throw new ArgumentException("UnknownExportFormat");
		}
		protected internal string GetDefaultFileName() {
			CustomGetDefaultFileNameEventArgs args = new CustomGetDefaultFileNameEventArgs((View != null) ? View.Caption : String.Empty);
			if(CustomGetDefaultFileName != null) {
				CustomGetDefaultFileName(this, args);
			}
			return args.FileName;
		}
		protected virtual IComponentExporter CreateComponentExporter(IPrintable printable) {
			return new ComponentExporterImpl(printable);
		}
		protected void ExportCore(ExportTarget exportTarget, Stream stream, ImageFormat imageFormat) {
			Guard.ArgumentNotNull(Exportable, "Exportable");
			IExportable localExportable = Exportable;
			IPrintable localPrintable = localExportable.Printable;
			ExportOptionsBase exportOptions = GetExportOptions(exportTarget, imageFormat);
			CustomExportEventArgs args = new CustomExportEventArgs(localPrintable, stream, exportTarget, exportOptions);
			OnCustomExport(args);
			if(!args.Handled) {
				localExportable.OnExporting();
				bool isDataAwareExported = false;
				switch(exportTarget) {
					case ExportTarget.Csv:
						if(Exportable is IDataAwareExportableCsv) {
							Guard.TypeArgumentIs(typeof(CsvExportOptionsEx), args.ExportOptions.GetType(), "CustomExportEventArgs.ExportOptions"); 
							((IDataAwareExportableCsv)localExportable).Export(stream, (CsvExportOptionsEx)args.ExportOptions);
							isDataAwareExported = true;
						}
						break;
					case ExportTarget.Xls:
						if(Exportable is IDataAwareExportableXls) {
							Guard.TypeArgumentIs(typeof(XlsExportOptionsEx), args.ExportOptions.GetType(), "CustomExportEventArgs.ExportOptions"); 
							((IDataAwareExportableXls)localExportable).Export(stream, (XlsExportOptionsEx)args.ExportOptions);
							isDataAwareExported = true;
						}
						break;
					case ExportTarget.Xlsx:
						if(Exportable is IDataAwareExportableXlsx) {
							Guard.TypeArgumentIs(typeof(XlsxExportOptionsEx), args.ExportOptions.GetType(), "CustomExportEventArgs.ExportOptions"); 
							((IDataAwareExportableXlsx)localExportable).Export(stream, (XlsxExportOptionsEx)args.ExportOptions);
							isDataAwareExported = true;
						}
						break;
				}
				if(!isDataAwareExported) {
					if(localPrintable == null) {
						throw new InvalidOperationException("IPrintable is null");
					}
					using(IComponentExporter exporter = CreateComponentExporter(localPrintable)) {
						if(args.ExportOptions != null) {
							exporter.Export(exportTarget, stream, args.ExportOptions);
						}
						else {
							exporter.Export(exportTarget, stream);
						}
					}
				}
			}
			OnExported(args);
		}
		protected abstract void Export(ExportTarget exportTarget);
		protected virtual void OnCustomExport(CustomExportEventArgs args) {
			if(CustomExport != null) {
				CustomExport(this, args);
			}
		}
		protected virtual void OnExported(CustomExportEventArgs args) {
			if(Exported != null) {
				Exported(this, args);
			}
		}
		protected virtual void OnExportActionItemsCreated() {
			if(ExportActionItemsCreated != null) {
				ExportActionItemsCreated(this, EventArgs.Empty);
			}
		}
		protected override void OnActivated() {
			AutoUpdateExportable = true;
			base.OnActivated();
			if(View is ListView && AutoUpdateExportable) {
				((ListView)View).EditorChanged += new EventHandler(view_EditorChanged);
				UpdateExportable();
			}
		}
		protected override void OnDeactivated() {
			if(View is ListView) {
				((ListView)View).EditorChanged -= new EventHandler(view_EditorChanged);
			}
			Exportable = null;
			base.OnDeactivated();
		}
		public ExportController() {
			this.exportAction = new SingleChoiceAction(this, "Export", PredefinedCategory.Export);
			this.exportAction.Caption = "Export to";
			this.exportAction.ImageName = "MenuBar_Export";
			this.exportAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.Independent;
			this.exportAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			this.exportAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.exportAction_OnExecute);
			this.exportAction.EmptyItemsBehavior = EmptyItemsBehavior.Deactivate;
		}
		private void RefreshExportAction() {
			exportAction.BeginUpdate();
			try {
				ExportAction.Items.Clear();
				List<ExportTarget> supportedExportFormats = new List<ExportTarget>();
				if(exportable != null) {
					if(exportable != null && exportable.Printable != null && exportable.SupportedExportFormats != null) {
						supportedExportFormats.AddRange(exportable.SupportedExportFormats);
					}
				}
				foreach(ExportTarget exportTarget in supportedExportFormats) {
					ChoiceActionItem exportItem = new ChoiceActionItem(GetItemCaption(exportTarget), exportTarget);
					exportItem.ImageName = GetItemImage(exportTarget);
					exportAction.Items.Add(exportItem);
				}
				OnExportActionItemsCreated();
			}
			finally {
				exportAction.EndUpdate();
			}
		}
		[DefaultValue(true)]
		public bool AutoUpdateExportable {
			get { return autoUpdateExportable; }
			set {
				if(AutoUpdateExportable != value) {
					autoUpdateExportable = value;
					if(AutoUpdateExportable) {
						UpdateExportable();
					}
				}
			} 
		}
		public IExportable Exportable {
			get { return exportable; }
			set {
				if(exportable != value) {
					if(exportable != null) {
						exportable.PrintableChanged -= new EventHandler<PrintableChangedEventArgs>(exportable_PrintableChanged);
					}
					exportable = value;
					if(exportable != null) {
						exportable.PrintableChanged += new EventHandler<PrintableChangedEventArgs>(exportable_PrintableChanged);
					}
					if(!isAutoUpdatingExportable) {
						AutoUpdateExportable = false;
					}
					RefreshExportAction();
				}
			}
		}
		public SingleChoiceAction ExportAction { get { return exportAction; } }
		public event EventHandler<CustomExportEventArgs> CustomExport;
		public event EventHandler<CustomExportEventArgs> Exported;
		public event EventHandler<EventArgs> ExportActionItemsCreated;
		public event EventHandler<CustomGetDefaultFileNameEventArgs> CustomGetDefaultFileName;
	}
	public abstract class ExportAnalysisController : ViewController {
		private SingleChoiceAction exportAction;
		private IStreamProvider streamProvider;
		protected internal IExportableAnalysisEditor exportableEditor;
		private IList<PrintingSystemCommand> exportTypes;
		private Dictionary<PrintingSystemCommand, string> actionItemsCaption;
		protected ComponentExporter exporter;
		private void exportable_PrintableChanged(object sender, PrintableChangedEventArgs e) {
			Update();
		}
		private void exportAction_OnExecute(Object sender, SingleChoiceActionExecuteEventArgs e) {
			PrintingSystemCommand exportType = (PrintingSystemCommand)e.SelectedChoiceActionItem.Data;
			Guard.ArgumentNotNull(exportType, "args.SelectedChoiceActionItem.Data");
			Export(exportType);
		}
		private IExportableAnalysisEditor GetExportableEditor(View view) {
			IExportableAnalysisEditor exportable = null;
			if(view != null) {
				if(view is DetailView) {
					exportable = ((DetailView)view).GetItems<IExportableAnalysisEditor>().Count != 0 ? ((DetailView)view).GetItems<IExportableAnalysisEditor>()[0] : null;
				}
			}
			return exportable;
		}
		private void CreateExportActionItems(IList<PrintingSystemCommand> exportTypes) {
			exportAction.Items.Clear();
			foreach(PrintingSystemCommand exportType in exportTypes) {
				ChoiceActionItem exportItem = new ChoiceActionItem(ActionItemsCaption[exportType], exportType);
				exportItem.ImageName = "MenuBar_" + Enum.GetName(typeof(PrintingSystemCommand), exportType);
				exportAction.Items.Add(exportItem);
			}
		}
		private void SubscribeToExportableEditor() {
			if(exportableEditor != null) {
				exportableEditor.PrintableChanged += new EventHandler<PrintableChangedEventArgs>(exportable_PrintableChanged);
			}
		}
		private void UnsubscribeFromExportableEditor() {
			if(exportableEditor != null) {
				exportableEditor.PrintableChanged -= new EventHandler<PrintableChangedEventArgs>(exportable_PrintableChanged);
			}
		}
		private void DisposeExporter() {
			if(exporter != null) {
				exporter.Dispose();
				exporter = null;
			}
		}
		private void UpdateExportableEditor() {
			if(exportableEditor != null) {
				UnsubscribeFromExportableEditor();
			}
			exportableEditor = GetExportableEditor(View);
			if(exportableEditor != null && exportableEditor.Printable != null) {
				DisposeExporter();
				exporter = new ComponentExporter(exportableEditor.Printable);
			}
			if(exportableEditor != null) {
				SubscribeToExportableEditor();
			}
		}
		private void Update() {
			UpdateExportableEditor();
			UpdateExportAction();
			UpdateActionState();
		}
		protected virtual IList<PrintingSystemCommand> GetExportTypes() {
			return exportableEditor.ExportTypes;
		}
		protected virtual void OnCustomExport(CustomExportAnalysisEventArgs args) {
			if(CustomExport != null) {
				CustomExport(this, args);
			}
		}
		protected virtual void OnExported(CustomExportAnalysisEventArgs args) {
			if(Exported != null) {
				Exported(this, args);
			}
		}
		protected virtual void OnExporting(CustomExportAnalysisEventArgs args) {
			if(Exporting != null) {
				Exporting(this, args);
			}
		}
		protected virtual ImageFormat GetImageFormat() {
			return ImageFormat.Png;
		}
		protected virtual void UpdateExportAction() {
			exportTypes = exportableEditor != null && exportableEditor.Printable != null ? GetExportTypes() : new List<PrintingSystemCommand>();
			ExportAction.BeginUpdate();
			CreateExportActionItems(exportTypes);
			ExportAction.EndUpdate();
		}
		protected virtual void UpdateActionState() {
			ExportAction.Active.SetItemValue("The XtraPrinting Library is available", ComponentPrinterDynamic.IsPrintingAvailable(false));
		}
		protected virtual void ExportToMht(Stream outputStream) {
			exporter.Export(ExportTarget.Mht, outputStream, new MhtExportOptions("utf-8", View.Caption, true));
		}
		protected virtual void ExportToXls(Stream outputStream) {
			exporter.Export(ExportTarget.Xls, outputStream);
		}
		protected virtual void ExportToXlsx(Stream outputStream) {
			exporter.Export(ExportTarget.Xlsx, outputStream);
		}
		protected virtual void ExportToPdf(Stream outputStream) {
			exporter.Export(ExportTarget.Pdf, outputStream);
		}
		protected virtual void ExportToRtf(Stream outputStream) {
			exporter.Export(ExportTarget.Rtf, outputStream);
		}
		protected virtual void ExportToTxt(Stream outputStream) {
			exporter.Export(ExportTarget.Text, outputStream);
		}
		protected virtual void ExportToHtml(Stream outputStream) {
			exporter.Export(ExportTarget.Html, outputStream, new HtmlExportOptions("utf-8", View.Caption, true));
		}
		protected virtual void ExportToImage(Stream outputStream, ImageFormat imageFormat) {
			exporter.Export(ExportTarget.Image, outputStream, new ImageExportOptions(imageFormat));
		}
		protected virtual void Export(PrintingSystemCommand exportType) {
			using(Stream outputStream = streamProvider.GetExportStream(exportType)) {
				if(outputStream != null && exportableEditor != null && exportableEditor.Printable != null) {
					CustomExportAnalysisEventArgs customExportEventArgs = new CustomExportAnalysisEventArgs(exportType, outputStream);
					OnExporting(customExportEventArgs);
					OnCustomExport(customExportEventArgs);
					if(!customExportEventArgs.Handled) {
						exporter.ClearDocument();
						switch(exportType) {
							case PrintingSystemCommand.ExportMht:
								ExportToMht(outputStream);
								break;
							case PrintingSystemCommand.ExportXls:
								ExportToXls(outputStream);
								break;
							case PrintingSystemCommand.ExportXlsx:
								ExportToXlsx(outputStream);
								break;
							case PrintingSystemCommand.ExportPdf:
								ExportToPdf(outputStream);
								break;
							case PrintingSystemCommand.ExportRtf:
								ExportToRtf(outputStream);
								break;
							case PrintingSystemCommand.ExportTxt:
								ExportToTxt(outputStream);
								break;
							case PrintingSystemCommand.ExportHtm:
								ExportToHtml(outputStream);
								break;
							case PrintingSystemCommand.ExportGraphic:
								ExportToImage(outputStream, GetImageFormat());
								break;
						}
					}
					OnExported(new CustomExportAnalysisEventArgs(exportType, outputStream));
					outputStream.Close();
				}
			}
		}
		protected abstract IStreamProvider CreateStreamProvider();
		protected override void OnViewControlsCreated() {
			if(Active) {
				Update();
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(View != null && View.IsControlCreated) {
				Update();
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			DisposeExporter();
			UnsubscribeFromExportableEditor();
			exportableEditor = null;
		}
		protected override void OnViewChanging(View view) {
			Active.SetItemValue("Editor is exportable", GetExportableEditor(view) != null);
			base.OnViewChanging(view);
		}
		protected string DefaultFileName {
			get {
				SplitString splitString = new SplitString();
				splitString.FirstPart = View is DetailView ? ((DetailView)View).GetCurrentObjectCaption() : String.Empty;
				splitString.SecondPart = View.Caption;
				return splitString.Text;
			}
		}
		protected IList<PrintingSystemCommand> ExportTypes {
			get { return exportTypes; }
		}
		public ExportAnalysisController() {
			Active.SetItemValue("Editor is exportable", false);
			streamProvider = CreateStreamProvider();
			this.exportAction = new SingleChoiceAction(this, "ExportAnalysis", PredefinedCategory.Export);
			this.exportAction.Caption = "Export Analysis to";
			this.exportAction.ImageName = "MenuBar_Export";
			this.exportAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.Independent;
			this.exportAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			this.exportAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.exportAction_OnExecute);
			this.exportAction.EmptyItemsBehavior = EmptyItemsBehavior.Disable;
		}
		private IDictionary<PrintingSystemCommand, string> ActionItemsCaption {
			get {
				if(actionItemsCaption == null) {
					actionItemsCaption = new Dictionary<PrintingSystemCommand, string>();
					actionItemsCaption.Add(PrintingSystemCommand.ExportXls, PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportXls_Caption));
					actionItemsCaption.Add(PrintingSystemCommand.ExportXlsx, PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportXlsx_Caption));
					actionItemsCaption.Add(PrintingSystemCommand.ExportHtm, PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportHtm_Caption));
					actionItemsCaption.Add(PrintingSystemCommand.ExportMht, PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportMht_Caption));
					actionItemsCaption.Add(PrintingSystemCommand.ExportPdf, PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportPdf_Caption));
					actionItemsCaption.Add(PrintingSystemCommand.ExportTxt, PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportTxt_Caption));
					actionItemsCaption.Add(PrintingSystemCommand.ExportRtf, PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportRtf_Caption));
					actionItemsCaption.Add(PrintingSystemCommand.ExportGraphic, PreviewLocalizer.GetString(PreviewStringId.RibbonPreview_ExportGraphic_Caption));
				}
				return actionItemsCaption;
			}
		}
		public SingleChoiceAction ExportAction {
			get { return exportAction; }
		}
		public IStreamProvider StreamProvider {
			get { return streamProvider; }
			set { streamProvider = value; }
		}
		public event EventHandler<CustomExportAnalysisEventArgs> CustomExport;
		public event EventHandler<CustomExportAnalysisEventArgs> Exporting;
		public event EventHandler<CustomExportAnalysisEventArgs> Exported;
	}
	public class CustomExportAnalysisEventArgs : HandledEventArgs {
		private Stream stream;
		private PrintingSystemCommand exportType;
		public CustomExportAnalysisEventArgs(PrintingSystemCommand exportType, Stream stream) {
			this.exportType = exportType;
			this.stream = stream;
		}
		public Stream Stream {
			get { return stream; }
		}
		public PrintingSystemCommand ExportType {
			get { return exportType; }
		}
	}
	public class PrintableChangedEventArgs : HandledEventArgs {
		private IPrintable printable;
		public PrintableChangedEventArgs(IPrintable printable) {
			this.printable = printable;
		}
		public IPrintable Printable {
			get { return printable; }
		}
	}
}
