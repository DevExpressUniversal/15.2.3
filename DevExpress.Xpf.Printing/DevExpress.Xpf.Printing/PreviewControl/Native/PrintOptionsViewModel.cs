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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.DocumentView;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Printing;
using DevExpress.Printing.Native.PrintEditor;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.DataPager;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Printing.PreviewControl.Native.Models;
using DevExpress.Xpf.Printing.PreviewControl.Rendering;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using UriHelper = DevExpress.Xpf.DocumentViewer.UriHelper;
using DevExpress.Printing.Native;
namespace DevExpress.Xpf.Printing.PreviewControl.Native {
	public class PrintOptionsViewModel : PreviewSettingsViewModelBase, IPrintForm, IDataErrorInfo {
		class PagedCollectionView : List<int>, IPagedCollectionView {
			readonly IEnumerable<int> enumerable;
			public PagedCollectionView(IEnumerable<int> enumerable)
				: base(enumerable) {
				this.enumerable = enumerable;
			}
			public bool CanChangePage {
				get { return true; }
			}
			public int ItemCount {
				get { return enumerable.Count(); }
			}
			public bool MoveToFirstPage() {
				PageIndex = enumerable.First();
				return true;
			}
			public bool MoveToPage(int pageIndex) {
				if(!enumerable.Contains(pageIndex))
					return false;
				PageIndex = pageIndex;
				return true;
			}
			public int PageIndex { get; private set; }
			public int PageSize { get; set; }
			public int TotalItemCount { get { return ItemCount; } }
		}
		readonly DocumentViewModel documentModel;
		int pagePreviewIndex;
		readonly PrintEditorController controller;
		PrintDocument printDocument;
		IPagedCollectionView dataPagerSource;
		public IPagedCollectionView DataPagerSource { get { return dataPagerSource; } }
		PrinterSettings PrinterSettings { get { return PrintDocument.PrinterSettings; } }
		Func<string, bool> showFileReplacingRequest;
		public Func<string, bool> ShowFileReplacingRequest {
			get { return showFileReplacingRequest; }
			set {
				if(showFileReplacingRequest != value)
					showFileReplacingRequest = value;
			}
		}
		public ICommand PreferencesCommand { get; private set; }
		public ICommand FileSelectCommand { get; private set; }
		public ICommand PrintCommand { get; private set; }
		public ICommand UpdatePreviewCommand { get; private set; }
		public PrintOptionsViewModel(DocumentViewModel documentModel) {
			this.documentModel = documentModel;
			PrinterItems = new ObservableCollection<PrinterItem>();
			this.printDocument = documentModel.CreatePrintDocument();
			UpdatePreview();
			dataPagerSource = new PagedCollectionView(documentModel.Pages.Select(x => x.PageIndex));
			controller = new PrintEditorController(this);
			controller.LoadForm(new PrinterItemContainer());
			PreferencesCommand = DelegateCommandFactory.Create(OnPreferencesClick);
			FileSelectCommand = DelegateCommandFactory.Create(OnFileSelectClick);
			UpdatePreviewCommand = DelegateCommandFactory.Create(UpdatePreview);
		}
		public int PagePreviewIndex {
			get { return pagePreviewIndex; }
			set {
				SetProperty(ref pagePreviewIndex, value, () => PagePreviewIndex, () => {
					PopulatePageNumbers();
					UpdatePreview();
				});
			}
		}
		ImageSource previewImage;
		public ImageSource PreviewImage {
			get { return previewImage; }
			set { SetProperty(ref previewImage, value, () => PreviewImage); }
		}
		void UpdatePreview() {
			var previewAreaProvider = GetService<IPreviewAreaProvder>();
			if(previewAreaProvider == null)
				return;
			var size = previewAreaProvider.PreviewArea;
			if(size == null || size.IsEmpty) {
				return;
			}
			var page = documentModel.Pages[PagePreviewIndex];
			PreviewImage = GetPreview(page, size);
		}
		ImageSource GetPreview(PageViewModel page, System.Windows.Size size) {
			var scale = CalculatePreviewScale(size.Width, size.Height, page);
			var bitmap = new Bitmap((int)(page.PageSize.Width * scale), (int)(page.PageSize.Height * scale));
			var graphics = Graphics.FromImage(bitmap);
			graphics.ScaleTransform((float)scale, (float)scale);
			((IPage)page.Page).Draw(graphics, PointF.Empty);
			documentModel.AfterDrawPages(new[] { PagePreviewIndex });
			return Convert(bitmap);
		}
		ImageSource Convert(Bitmap bitmap) {
			var stream = new MemoryStream();
			bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			stream.Seek(0, SeekOrigin.Begin);
			image.StreamSource = stream;
			image.EndInit();
			return image;
		}
		double CalculatePreviewScale(double width, double height, PageViewModel page) {
			var verticalScaling = height / page.PageSize.Height;
			var horizontalScaling = width / page.PageSize.Width;
			return Math.Min(verticalScaling, horizontalScaling);
		}
		public ObservableCollection<PrinterItem> PrinterItems { get; private set; }
		public IEnumerable<string> PaperSources { get { return PrinterSettings.PaperSources.Cast<PaperSource>().Select(x => x.SourceName); } }
		PrinterItem selectedPrinter;
		public PrinterItem SelectedPrinter {
			get { return selectedPrinter; }
			set {
				PrinterSettings.PrinterName = value.FullName;
				SetProperty(ref selectedPrinter, value, () => SelectedPrinter, () => {
					CoerceCopies();
					PopulatePaperSources();
				});
			}
		}
		void CoerceCopies() {
			Copies = Copies > MaxCopies ? Copies = System.Convert.ToInt16(MaxCopies) : Copies;
			RaisePropertiesChanged(() => AllowCopies, ()=> MaxCopies);
		}
		public override SettingsType SettingsType {
			get { return Native.SettingsType.Print; }
		}
		#region IPrintForm
		void IPrintForm.AddPrinterItem(DevExpress.Printing.PrinterItem item) {
			PrinterItems.Add(item);
		}
		public bool AllowSomePages {
			get { return PrinterSettings.PrintRange == PrintRange.SomePages; }
			set {
				if(value && PrinterSettings.PrintRange != PrintRange.SomePages)
					((IPrintForm)this).SetPrintRange(PrintRange.SomePages);
			}
		}
		public bool AllowCollate { get { return Copies > 1; } }
		public bool AllowCopies { get { return PrinterSettings.MaximumCopies > 1; } }
		public bool Collate {
			get { return PrinterSettings.Collate; }
			set {
				PrinterSettings.Collate = value;
				RaisePropertyChanged(() => Collate);
			}
		}
		public int MaxCopies { get { return PrinterSettings.MaximumCopies; } }
		public short Copies {
			get { return PrinterSettings.Copies; }
			set {
				value = value > 0 ? value : (short)1;
				if(PrinterSettings.Copies != value) {
					PrinterSettings.Copies = value;
					RaisePropertyChanged(() => Copies);
					RaisePropertyChanged(() => AllowCollate);
				}
			}
		}
		PSPrintDocument PrintDocument {
			get { return (PSPrintDocument)printDocument; }
		}
		PrintDocument IPrintForm.Document {
			get {
				if(printDocument == null) {
					printDocument = documentModel.CreatePrintDocument();
					printDocument.PrinterSettings.PrintRange = System.Drawing.Printing.PrintRange.AllPages;
				}
				return printDocument;
			}
			set { SetProperty(ref printDocument, value, () => ((IPrintForm)this).Document); }
		}
		string customPageRange;
		public string CustomPageRange {
			get { return customPageRange ?? (customPageRange = PrintDocument.PageRange); }
			set { SetProperty(ref customPageRange, value, () => CustomPageRange, PopulatePageNumbers); }
		}
		string IPrintForm.PageRangeText {
			get { return PrintDocument.PageRange; }
			set { PrintDocument.PageRange = value; }
		}
		string paperSource;
		public string PaperSource {
			get { return paperSource; }
			set {
				if(paperSource == value)
					return;
				paperSource = value;
				RaisePropertyChanged(() => PaperSource);
			}
		}
		public PrintRange PrintRange {
			get { return PrinterSettings.PrintRange; }
			set {
				SetPrintRange(value);
				RaisePropertyChanged(() => PrintRange);
			}
		}
		public bool PrintToFile {
			get { return PrinterSettings.PrintToFile; }
			set {
				if(PrinterSettings.PrintToFile != value) {
					PrinterSettings.PrintToFile = value;
					RaisePropertiesChanged(()=>PrintToFile, ()=> PrintFileName);
				}
			}
		}
		string printFileName;
		public string PrintFileName {
			get { return printFileName; }
			set { SetProperty(ref printFileName, value, () => PrintFileName, () => { if(!string.IsNullOrEmpty(PrintFileName)) PrinterSettings.PrintFileName = PrintFileName; }); }
		}
		public void SetPrintRange(System.Drawing.Printing.PrintRange printRange) {
			if(PrintRange != printRange) {
				PrintRange previousPrintRange = PrintRange;
				PrinterSettings.PrintRange = printRange;
				RaisePropertiesChanged(() => AllowSomePages, ()=> CustomPageRange);
				PopulatePageNumbers();
			}
		}
		void PopulatePageNumbers() {
			switch(PrintRange) {
				case PrintRange.AllPages:
					((IPrintForm)this).PageRangeText = new PageScope(1, documentModel.Pages.Count).PageRange;
					break;
				case PrintRange.CurrentPage:
					((IPrintForm)this).PageRangeText = string.Format("{0}", PagePreviewIndex + 1);
					break;
				case PrintRange.SomePages:
					((IPrintForm)this).PageRangeText = CustomPageRange;
					break;
				default:
					break;
			}
		}
		void IPrintForm.SetSelectedPrinter(string printerName) {
			for(int i = 0; i < PrinterItems.Count; i++) {
				if(PrinterItems[i].FullName == printerName) {
					SelectedPrinter = PrinterItems[i];
					return;
				}
			}
		}
		#endregion
		void OnPreferencesClick() {
			try {
				PrintRange previousPrintRange = PrintRange;
				var windowHandler = new WindowInteropHelper(Application.Current.MainWindow).Handle;
				controller.ShowPrinterPreferences(windowHandler);
			} catch { }
		}
		void PopulatePaperSources() {
			RaisePropertyChanged(() => PaperSources);
			PaperSource = PaperSources.FirstOrDefault();
		}
		void OnFileSelectClick() {
			Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
			saveFileDialog.DefaultExt = ".prn";
			saveFileDialog.Filter = "Printable files (.prn)|*.prn";
			if(saveFileDialog.ShowDialog() == true)
				PrintFileName = saveFileDialog.FileName;
		}
		#region IDataErrorInfo
		public string Error {
			get { return string.Empty; }
		}
		public string this[string columnName] {
			get { return Validate(columnName); }
		}
		#endregion
		const string printFileNameProperty = "PrintFileName";
		const string printToFileProperty = "PrintToFile";
		const string pageRangeTextProperty = "CustomPageRange";
		string Validate(string columnName) {
			ValidationResult result = null;
			if(columnName == printFileNameProperty || columnName == printToFileProperty)
				result = ValidateFileName();
			else if(columnName == pageRangeTextProperty)
				result = ValidatePageNumbers();
			return result != null ? result.ErrorMessage : null;
		}
		ValidationResult ValidateFileName() {
			string message = String.Empty;
			if(PrintToFile) {
				if(!PrintEditorController.ValidateFilePath(PrintFileName, out message)) {
					return new ValidationResult(false, message);
				} else if(File.Exists(PrintFileName) && (File.GetAttributes(PrintFileName) & FileAttributes.ReadOnly) > 0) {
					return new ValidationResult(false, string.Format(PreviewStringId.Msg_FileReadOnly.GetString(), PrintFileName));
				}
			}
			return null;
		}
		ValidationResult ValidatePageNumbers() {
			int[] pageIndices = PageRangeParser.GetIndices(((IPrintForm)this).PageRangeText, PrinterSettings.MaximumPage);
			if(PrintRange == System.Drawing.Printing.PrintRange.SomePages && (string.IsNullOrEmpty(((IPrintForm)this).PageRangeText)
				|| pageIndices.Length <= 0
				|| (pageIndices.Length == 1 && pageIndices[0] == -1))) {
				return new ValidationResult(false, PreviewStringId.Msg_IncorrectPageRange.GetString());
			}
			return null;
		}
		public bool IsValid {
			get {
				return ValidateFileName().Return(v1 => v1.IsValid, () => true) && ValidatePageNumbers().Return(v2 => v2.IsValid, () => true);
			}
		}
	}
	public class PrinterTypeToImageConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			string dllName = Assembly.GetExecutingAssembly().GetName().Name;
			PrinterType flags = (PrinterType)value;
			bool isDefault = flags.HasFlag(PrinterType.Default);
			if(flags.HasFlag(PrinterType.Fax) && flags.HasFlag(PrinterType.Network))
				return isDefault ? UriHelper.GetUri(dllName, @"\Images\Printers\DefaultFaxNetwork_16x16.png") : UriHelper.GetUri(dllName, @"\Images\Printers\FaxNetwork_16x16.png");
			if(flags.HasFlag(PrinterType.Fax))
				return isDefault ? UriHelper.GetUri(dllName, @"\Images\Printers\DefaultFax_16x16.png") : UriHelper.GetUri(dllName, @"\Images\Printers\Fax_16x16.png");
			if(flags.HasFlag(PrinterType.Printer) && flags.HasFlag(PrinterType.Network))
				return isDefault ? UriHelper.GetUri(dllName, @"\Images\Printers\DefaultPrinterNetwork_16x16.png") : UriHelper.GetUri(dllName, @"\Images\Printers\PrinterNetwork_16x16.png");
			if(flags.HasFlag(PrinterType.Printer))
				return isDefault ? UriHelper.GetUri(dllName, @"\Images\Printers\DefaultPrinter_16x16.png") : UriHelper.GetUri(dllName, @"\Images\Printers\Printer_16x16.png");
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	interface IPreviewAreaProvder {
		System.Windows.Size PreviewArea { get; }
	}
	public class PreviewAreaProvider : ServiceBase, IPreviewAreaProvder {
		public System.Windows.Size PreviewArea { get; private set; }
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.Loaded += OnAssociatedObjectLoaded;
		}
		void OnAssociatedObjectLoaded(object sender, System.Windows.RoutedEventArgs e) {
			AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
			var preview = LayoutHelper.FindElementByName(AssociatedObject.GetRootVisual(), "PART_PreviewImage");
			preview.Do(x=> PreviewArea = x.RenderSize);
		}
	}
}
