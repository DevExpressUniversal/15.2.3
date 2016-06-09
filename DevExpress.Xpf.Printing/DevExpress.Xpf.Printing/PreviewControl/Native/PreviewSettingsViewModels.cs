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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Printing;
using DevExpress.Printing.Native.PrintEditor;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.DataPager;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Printing.Native.Lines;
using DevExpress.Xpf.Printing.PreviewControl.Native.Models;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
using Microsoft.Win32;
namespace DevExpress.Xpf.Printing.PreviewControl.Native {
	public enum SettingsType {
		Export,
		Send,
		Print,
		PageSettings,
		Scale,
		Watermark
	}
	public abstract class PreviewSettingsViewModelBase : ViewModelBase {
		public abstract SettingsType SettingsType { get; }
	}
	public abstract class ExportOptionsViewModelBase : PreviewSettingsViewModelBase {
		readonly ExportOptions options;
		readonly PrintPreviewOptions printPreviewOptions;
		readonly string proposedFileName;
		ExportFormat exportFormat;
		IEnumerable<ExportFormat> hiddenExportFormats;
		ExportOptionsBase exportOptions;
		ExportOptionsControllerBase controller;
		LineBase[] exportOptionLines;
		ObservableCollection<ExportOptionKind> hiddenOptions;
		string fileName = string.Empty;
		protected ExportOptions Options { get { return options; } }
		public ExportFormat ExportFormat {
			get { return exportFormat; }
			set {
				exportFormat = value;
				OnExportFormatChagned();
				RaisePropertyChanged(() => ExportFormat);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LineBase[] ExportOptionLines {
			get { return exportOptionLines; }
			private set { SetProperty(ref exportOptionLines, value, () => ExportOptionLines); }
		}
		public ExportOptionsBase ExportOptions {
			get { return exportOptions; }
			set { SetProperty(ref exportOptions, value, () => ExportOptions); }
		}
		public string FileName {
			get { return fileName; }
			set { SetProperty(ref fileName, value, () => FileName); }
		}
		internal AvailableExportModes AvailableExportModes { get; set; }
		public IEnumerable<ExportFormat> HiddenExportFormats {
			get { return hiddenExportFormats; }
			set { SetProperty(ref hiddenExportFormats, value, () => HiddenExportFormats, () => RaisePropertyChanged(() => AvailableExportFormats)); }
		}
		internal ObservableCollection<ExportOptionKind> HiddenOptions {
			get {
				return hiddenOptions ?? (hiddenOptions = CreateEmptyHiddenOptions());
			}
			set {
				hiddenOptions.Do(x => x.CollectionChanged -= OnHiddenOptionsCollectionChanged);
				SetProperty(ref hiddenOptions, value, () => HiddenOptions, () => OnHiddenOptionsCollectionChanged());
				hiddenOptions.Do(x => x.CollectionChanged += OnHiddenOptionsCollectionChanged);
			}
		}
		public virtual IEnumerable<ExportFormat> AvailableExportFormats {
			get {
				var formats = Enum.GetValues(typeof(ExportFormat)).Cast<ExportFormat>().Except(new[] { ExportFormat.Prnx });
				return hiddenExportFormats != null ? formats.Except(hiddenExportFormats) : formats;
			}
		}
		public bool ShowOptionsBeforeExport {
			get {
				return ExportOptionsHelper.GetShowOptionsBeforeExport(ExportOptions, printPreviewOptions.ShowOptionsBeforeExport)
					|| (ExportOptionsHelper.GetUseActionAfterExportAndSaveModeValue(ExportOptions));
			}
		}
		public ICommand SelectFileCommand { get; private set; }
		public ExportOptionsViewModelBase(ExportOptions options) {
			this.options = options;
			this.printPreviewOptions = options.PrintPreview;
			proposedFileName = ValidateFileName(printPreviewOptions.DefaultFileName, PrintPreviewOptions.DefaultFileNameDefault);
			SelectFileCommand = DelegateCommandFactory.Create(SelectFile);
		}
		protected virtual void OnExportFormatChagned() {
			ExportOptions = GetExportOptions(ExportFormat).CloneOptions();
			controller = ExportOptionsControllerBase.GetControllerByOptions(ExportOptions);
			FileName = GetDefaultFileName();
			SetLines();
		}
		void SetLines() {
			var lines = controller.GetExportLines(ExportOptions, new LineFactory(), AvailableExportModes ?? new AvailableExportModes(), new List<ExportOptionKind>()).ToList();
			var last = lines.LastOrDefault();
			if(last != null && last is EmptyLine)
				lines.Remove(last);
			ExportOptionLines = Array.ConvertAll(lines.ToArray(), (line) => (LineBase)line);
		}
		void SelectFile() {
			Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
			saveFileDialog.Title = PreviewLocalizer.GetString(PreviewStringId.SaveDlg_Title);
			saveFileDialog.ValidateNames = true;
			if(!string.IsNullOrEmpty(FileName)) {
				saveFileDialog.InitialDirectory = Path.GetDirectoryName(FileName);
				saveFileDialog.FileName = Path.GetFileName(FileName);
			} else {
				saveFileDialog.InitialDirectory = printPreviewOptions.DefaultDirectory;
				saveFileDialog.FileName = proposedFileName;
			}
			saveFileDialog.Filter = controller.Filter;
			saveFileDialog.FilterIndex = controller.GetFilterIndex(exportOptions);
			saveFileDialog.OverwritePrompt = controller.ValidateInputFileName(exportOptions);
			if(saveFileDialog.ShowDialog() == true && !string.IsNullOrEmpty(saveFileDialog.FileName))
				FileName = FileHelper.SetValidExtension(saveFileDialog.FileName, controller.GetFileExtension(exportOptions), controller.FileExtensions);
			CommandManager.InvalidateRequerySuggested();
		}
		static string ValidateFileName(string fileName, string defaultFileName) {
			if(File.Exists(fileName))
				return fileName;
			if(string.IsNullOrEmpty(fileName))
				return defaultFileName;
			try {
				string tempFileName = Path.Combine(Path.GetTempPath(), fileName);
				File.Create(tempFileName).Close();
				File.Delete(tempFileName);
			} catch(Exception) {
				return defaultFileName;
			}
			return fileName;
		}
		ExportOptionsBase GetExportOptions(ExportFormat format) {
			switch(format) {
				case ExportFormat.Pdf:
					return options.Options[typeof(PdfExportOptions)];
				case ExportFormat.Htm:
					return options.Options[typeof(HtmlExportOptions)];
				case ExportFormat.Mht:
					return options.Options[typeof(MhtExportOptions)];
				case ExportFormat.Rtf:
					return options.Options[typeof(RtfExportOptions)];
				case ExportFormat.Xls:
					return options.Options[typeof(XlsExportOptions)];
				case ExportFormat.Xlsx:
					return options.Options[typeof(XlsxExportOptions)];
				case ExportFormat.Csv:
					return options.Options[typeof(CsvExportOptions)];
				case ExportFormat.Txt:
					return options.Options[typeof(TextExportOptions)];
				case ExportFormat.Image:
					return options.Options[typeof(ImageExportOptions)];
				case ExportFormat.Xps:
					return options.Options[typeof(XpsExportOptions)];
				default:
					throw new ArgumentException("format");
			}
		}
		string GetDefaultFileName() {
			string path = string.Empty;
			if(printPreviewOptions.SaveMode == SaveMode.UsingDefaultPath) {
				string directory = string.IsNullOrEmpty(printPreviewOptions.DefaultDirectory) ? Directory.GetCurrentDirectory() : printPreviewOptions.DefaultDirectory;
				path = Path.Combine(directory, proposedFileName + controller.GetFileExtension(ExportOptions));
			}
			return path;
		}
		ObservableCollection<ExportOptionKind> CreateEmptyHiddenOptions() {
			var emptyHiddenOptions = new ObservableCollection<ExportOptionKind>();
			emptyHiddenOptions.CollectionChanged += OnHiddenOptionsCollectionChanged;
			return emptyHiddenOptions;
		}
		void OnHiddenOptionsCollectionChanged(object s = null, NotifyCollectionChangedEventArgs e = null) {
			SetLines();
		}
	}
	public class ExportOptionsViewModel : ExportOptionsViewModelBase {
		bool openFileAfterExport;
		public ExportOptionsViewModel(XtraPrinting.ExportOptions exportOptions) : base(exportOptions) { }
		public bool CanUseActionAfterExport {
			get { return ExportOptions.Return(x => x.UseActionAfterExportAndSaveModeValue, () => false); }
		}
		public bool OpenFileAfterExport {
			get { return openFileAfterExport; }
			set { SetProperty(ref openFileAfterExport, value, () => OpenFileAfterExport); }
		}
		protected override void OnExportFormatChagned() {
			base.OnExportFormatChagned();
			OpenFileAfterExport = CanUseActionAfterExport;
		}
		public override SettingsType SettingsType {
			get { return SettingsType.Export; }
		}
	}
	public class SendOptionsViewModel : ExportOptionsViewModelBase {
		public SendOptionsViewModel(ExportOptions options) : base(options) { }
		public EmailOptions EmailOptions { get { return Options.Email; } }
		public override IEnumerable<ExportFormat> AvailableExportFormats {
			get { return base.AvailableExportFormats.Except(new[] { ExportFormat.Htm }); }
		}
		public override SettingsType SettingsType {
			get { return SettingsType.Send; }
		}
	}
	public class ScaleOptionsViewModel : PreviewSettingsViewModelBase, IDataErrorInfo {
		const string scaleFactorPropertyName = "ScaleFactor";
		const string pagesToFitPropertyName = "PagesToFit";
		bool isInputValid = true;
		readonly double currentScaleFactor;
		int pagesToFit;
		float scaleFactor;
		ScaleMode scaleMode;
		public override SettingsType SettingsType {
			get { return Native.SettingsType.Scale; }
		}
		public ScaleOptionsViewModel(float scaleFactor, int pagesToFit) {
			ScaleFactorValues = new List<int>(new int[] { 10, 25, 50, 100, 200, 300, 500, 700, 1000 });
			PagesToFitValues = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			bool isPreviousScaleModeAdjustToPercent = pagesToFit == 0;
			this.pagesToFit = isPreviousScaleModeAdjustToPercent ? 1 : pagesToFit;
			this.scaleFactor = scaleFactor;
			currentScaleFactor = this.scaleFactor;
		}
		static int MinPagesToFit {
			get { return 1; }
		}
		static int MaxPagesToFit {
			get { return 10; }
		}
		static float MinScaleFactor {
			get { return .01f; }
		}
		static float MaxScaleFactor {
			get { return 10f; }
		}
		public List<int> ScaleFactorValues {
			get;
			private set;
		}
		public List<int> PagesToFitValues {
			get;
			private set;
		}
		public ScaleMode ScaleMode {
			get { return scaleMode; }
			set { 
				SetProperty(ref scaleMode, value, () => ScaleMode);
				CommandManager.InvalidateRequerySuggested();
			}
		}
		public float ScaleFactor {
			get { return scaleFactor; }
			set {
				if(value >= MinScaleFactor && value <= MaxScaleFactor)
					SetProperty(ref scaleFactor, value, () => ScaleFactor);
				CommandManager.InvalidateRequerySuggested();
			}
		}
		public int PagesToFit {
			get { return pagesToFit; }
			set {
				if(value >= MinPagesToFit && value <= MaxPagesToFit)
					SetProperty(ref pagesToFit, value, () => PagesToFit);
				CommandManager.InvalidateRequerySuggested();
			}
		}
		bool IsInputValid {
			get { return isInputValid; }
			set { SetProperty(ref isInputValid, value, () => IsInputValid); }
		}
		public bool CanApply {
			get {
				return Validate();
			}
		}
		bool Validate() {
			if(ScaleMode == ScaleMode.AdjustToPercent)
				return ValidateScaleFactor(ScaleFactor).IsValid && currentScaleFactor != ScaleFactor;
			else
				return ValidatePagesToFit(PagesToFit).IsValid;
		}
		static float ToFloatFactor(int value) {
			return Convert.ToSingle(value) / 100f;
		}
		public ValidationResult ValidateScaleFactor(object value) {
			return Validate(value, MinScaleFactor, MaxScaleFactor);
		}
		public ValidationResult ValidatePagesToFit(object value) {
			return Validate(value, MinPagesToFit, MaxPagesToFit);
		}
		ValidationResult Validate(object validatingValue, double minValue, double maxValue) {
			if(validatingValue == null) {
				IsInputValid = false;
				return new ValidationResult(false, PrintingLocalizer.GetString(PrintingStringId.Scaling_ComboBoxEdit_Validation_Error));
			}
			double intValue = 0;
			string text = validatingValue as String;
			if(string.IsNullOrEmpty(text)) {
				try {
					intValue = Convert.ToDouble(validatingValue);
				} catch {
					IsInputValid = false;
					return new ValidationResult(false, PrintingLocalizer.GetString(PrintingStringId.Scaling_ComboBoxEdit_Validation_Error));
				}
			} else if(!double.TryParse(text, out intValue)) {
				IsInputValid = false;
				return new ValidationResult(false, PrintingLocalizer.GetString(PrintingStringId.Scaling_ComboBoxEdit_Validation_Error));
				;
			}
			if(intValue >= minValue && intValue <= maxValue) {
				IsInputValid = true;
				return new ValidationResult(true);
			} else {
				IsInputValid = false;
				return new ValidationResult(false, PrintingLocalizer.GetString(PrintingStringId.Scaling_ComboBoxEdit_Validation_OutOfRange_Error));
			}
		}
		public string Error {
			get { return null; }
		}
		string IDataErrorInfo.this[string columnName] {
			get { return Validate(columnName); }
		}
		string Validate(string columnName) {
			ValidationResult result = null;
			if(columnName == scaleFactorPropertyName)
				result = ValidateScaleFactor(ScaleFactor);
			else if(columnName == pagesToFitPropertyName)
				result = ValidatePagesToFit(PagesToFit);
			return result != null ? result.ErrorMessage : null;
		}
	}
	public class ScaleFactorToPercentsConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var scaleFactor = (float)value;
			return (int)(scaleFactor * 100);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var percents = System.Convert.ToSingle(value);
			return percents / 100f;
		}
	}
}
