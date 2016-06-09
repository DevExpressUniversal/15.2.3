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
using System.Drawing.Printing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf.Drawing;
using DevExpress.Printing.Native.PrintEditor;
using DevExpress.Xpf.DocumentViewer;
using System.Reflection;
using DevExpress.Pdf;
using System.Collections;
using DevExpress.Xpf.Editors.DataPager;
using System.Collections.Generic;
using Control = System.Windows.Controls.Control;
using System.Windows.Forms;
using DevExpress.Xpf.Editors;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.PdfViewer.Internal {
	public class PdfPrintEditorAttachedBehavior : Behavior<FrameworkElement> {
		public static readonly DependencyProperty HasValidationErrorProperty;
		static PdfPrintEditorAttachedBehavior() {
			HasValidationErrorProperty = DependencyPropertyRegistrator.Register<PdfPrintEditorAttachedBehavior, bool>(owner => owner.HasValidationError, false, (d, oldValue, newValue) => d.OnHasValidationErrorChanged(newValue));
		}
		void OnHasValidationErrorChanged(bool newValue) {
			AssociatedObject.With(x => x.DataContext as PrintDialogViewModel).Do(x => x.HasValidationError = newValue);
			CommandManager.InvalidateRequerySuggested();
		}
		public bool HasValidationError {
			get { return (bool)GetValue(HasValidationErrorProperty); }
			set { SetValue(HasValidationErrorProperty, value); }
		}
	}
	public class SizeChangedEventArgsConverter : EventArgsConverterBase<SizeChangedEventArgs> {
		protected override object Convert(object sender, SizeChangedEventArgs args) {
			return args.NewSize;
		}
	}
	public class PrintDialogDataPager : DataPager {
		public PrintDialogDataPager() {
			DefaultStyleKey = typeof(PrintDialogDataPager);
		}
	}
	public class PrintDialogViewModel : ViewModelBase {
		static ReflectionHelper reflectionHelper = new ReflectionHelper();
		bool hasValidationError;
		public ICommand PreferencesCommand { get; private set; }
		public ICommand FileSelectCommand { get; private set; }
		public ICommand PrintCommand { get; private set; }
		public PdfPrintDialogViewModel PdfViewModel { get; private set; }
		public bool HasValidationError {
			get { return hasValidationError; }
			set { SetProperty(ref hasValidationError, value, () => HasValidationError, OnHasValidationErrorChanged); }
		}
		void OnHasValidationErrorChanged() {
			PdfViewModel.EnableToPrint = !HasValidationError;
		}
		public PrintDialogViewModel(PdfPrintDialogViewModel printDialogViewModel) {
			PreferencesCommand = DelegateCommandFactory.Create(OnPreferencesClick);
			FileSelectCommand = DelegateCommandFactory.Create(OnFileSelectClick);
			PdfViewModel = printDialogViewModel;
		}
		void OnPreferencesClick() {
			PdfViewModel.Do(x => x.ShowPreferences(IntPtr.Zero));
		}
		void OnFileSelectClick() {
			using (SaveFileDialog saveFileDialog = new SaveFileDialog()) {
				saveFileDialog.DefaultExt = PdfViewerLocalizer.GetString(PdfViewerStringId.PrintFileExtension);
				saveFileDialog.Filter = PdfViewerLocalizer.GetString(PdfViewerStringId.PrintFileFilter);
				saveFileDialog.OverwritePrompt = false;
				DialogResult saveFileResult = saveFileDialog.ShowDialog();
				if (saveFileResult == DialogResult.OK)
					PdfViewModel.PrintFileName = saveFileDialog.FileName;
			}
		}
	}
	public class ChangeTypeConverter : IValueConverter {
		public Type TargetType { get; set; }
		public Type SourceType { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (TargetType.If(x => x.IsEnum).ReturnSuccess())
				return Enum.Parse(TargetType, value.With(x => x.ToString()));
			return System.Convert.ChangeType(value, TargetType ?? typeof(object));
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (SourceType.If(x => x.IsEnum).ReturnSuccess())
				return Enum.Parse(SourceType, value.With(x => x.ToString()));
			return System.Convert.ChangeType(value, SourceType ?? typeof(object));
		}
	}
	public class PrintRangeConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			PrintRange range = (PrintRange)value;
			switch (range) {
				case PrintRange.AllPages:
					return 0;
				case PrintRange.CurrentPage:
					return 1;
				case PrintRange.SomePages:
					return 2;
				default:
					throw new NotSupportedException();
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			int range = (int)value;
			switch (range) {
				case 0:
					return PrintRange.AllPages;
				case 1:
					return PrintRange.CurrentPage;
				case 2:
					return PrintRange.SomePages;
				default:
					throw new NotSupportedException();
			}
		}
	}
	public class PrintRangeToRangeEditEnabledConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			PrintRange range = (PrintRange)value;
			return range == PrintRange.SomePages;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PrinterTypeToImageConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			PrinterType flags = (PrinterType)value;
			string dllName = Assembly.GetExecutingAssembly().GetName().Name;
			bool isDefault = flags.HasFlag(PrinterType.Default);
			if (flags.HasFlag(PrinterType.Fax) && flags.HasFlag(PrinterType.Network))
				return isDefault ? UriHelper.GetUri(dllName, @"\Images\Print\DefaultFaxNetwork_16x16.png") : UriHelper.GetUri(dllName, @"\Images\Print\FaxNetwork_16x16.png");
			if (flags.HasFlag(PrinterType.Fax))
				return isDefault ? UriHelper.GetUri(dllName, @"\Images\Print\DefaultFax_16x16.png") : UriHelper.GetUri(dllName, @"\Images\Print\Fax_16x16.png");
			if (flags.HasFlag(PrinterType.Printer) && flags.HasFlag(PrinterType.Network))
				return isDefault ? UriHelper.GetUri(dllName, @"\Images\Print\DefaultPrinterNetwork_16x16.png") : UriHelper.GetUri(dllName, @"\Images\Print\PrinterNetwork_16x16.png");
			if (flags.HasFlag(PrinterType.Printer))
				return isDefault ? UriHelper.GetUri(dllName, @"\Images\Print\DefaultPrinter_16x16.png") : UriHelper.GetUri(dllName, @"\Images\Print\Printer_16x16.png");
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class ScaleModeToScaleEditEnabledConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			PdfPrintScaleMode mode = (PdfPrintScaleMode)value;
			return mode == PdfPrintScaleMode.CustomScale;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PagedCollectionView : List<int>, IPagedCollectionView {
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
			if (!enumerable.Contains(pageIndex))
				return false;
			PageIndex = pageIndex;
			return true;
		}
		public int PageIndex { get; private set; }
		public int PageSize { get; set; }
		public int TotalItemCount { get { return ItemCount; } }
	}
	public class IEnumerableToPagedCollectionConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			IEnumerable<int> enumerable = (IEnumerable<int>)value;
			PagedCollectionView collection = new PagedCollectionView(enumerable);
			return collection;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PdfPrintPageOrientationConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			PdfPrintPageOrientation orientation = (PdfPrintPageOrientation)value;
			switch (orientation) {
				case PdfPrintPageOrientation.Auto:
					return PdfViewerLocalizer.GetString(PdfViewerStringId.PrintDialogPrintOrientationAuto);
				case PdfPrintPageOrientation.Landscape:
					return PdfViewerLocalizer.GetString(PdfViewerStringId.PrintDialogPrintOrientationLandscape);
				case PdfPrintPageOrientation.Portrait:
					return PdfViewerLocalizer.GetString(PdfViewerStringId.PrintDialogPrintOrientationPortrait);
			}
			return string.Empty;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
