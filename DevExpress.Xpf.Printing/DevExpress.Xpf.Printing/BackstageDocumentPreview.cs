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

using DevExpress.Printing;
using DevExpress.Printing.Native.PrintEditor;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Printing.PreviewControl.Native;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Printing {
	public enum BackstagePreviewOptionsMode {
		Printing,
		Export,
	}
	public class BackstageDocumentPreview : Control {
		static BackstageDocumentPreview() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageDocumentPreview), new FrameworkPropertyMetadata(typeof(BackstageDocumentPreview)));
		}
		internal static readonly DependencyPropertyKey IsPrintersAvailablePropertyKey =
			DependencyProperty.RegisterReadOnly("IsPrintersAvailable", typeof(bool), typeof(BackstageDocumentPreview), new FrameworkPropertyMetadata(true));
		public static readonly DependencyProperty IsPrintersAvailableProperty = IsPrintersAvailablePropertyKey.DependencyProperty;
		public bool IsPrintersAvailable {
			get { return (bool)GetValue(IsPrintersAvailableProperty); }
			protected set { SetValue(IsPrintersAvailablePropertyKey, value); }
		}
		internal static readonly DependencyPropertyKey PrintersErrorMessagePropertyKey =
			DependencyProperty.RegisterReadOnly("PrintersErrorMessage", typeof(string), typeof(BackstageDocumentPreview), new FrameworkPropertyMetadata(string.Empty));
		public static readonly DependencyProperty PrintersErrorMessageProperty = PrintersErrorMessagePropertyKey.DependencyProperty;
		public string PrintersErrorMessage {
			get { return (string)GetValue(PrintersErrorMessageProperty); }
			protected set { SetValue(PrintersErrorMessagePropertyKey, value); }
		}
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
			"Model",
			typeof(IDocumentPreviewModel),
			typeof(BackstageDocumentPreview),
			new PropertyMetadata(null));
		public IDocumentPreviewModel Model {
			get { return (IDocumentPreviewModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		public PrinterViewModel SelectedPrinter {
			get { return (PrinterViewModel)GetValue(SelectedPrinterProperty); }
			set { SetValue(SelectedPrinterProperty, value); }
		}
		public static readonly DependencyProperty SelectedPrinterProperty =
			DependencyProperty.Register("SelectedPrinter", typeof(PrinterViewModel), typeof(BackstageDocumentPreview), new PropertyMetadata(null));
		public List<PrinterViewModel> Printers {
			get { return (List<PrinterViewModel>)GetValue(PrintersProperty); }
			protected set { SetValue(PrintersPropertyKey, value); }
		}
		static readonly DependencyPropertyKey PrintersPropertyKey =
			DependencyProperty.RegisterReadOnly("Printers", typeof(List<PrinterViewModel>), typeof(BackstageDocumentPreview), new PropertyMetadata(null));
		public static readonly DependencyProperty PrintersProperty = PrintersPropertyKey.DependencyProperty;
		public BackstagePreviewOptionsMode OptionsMode {
			get { return (BackstagePreviewOptionsMode)GetValue(OptionsModeProperty); }
			set { SetValue(OptionsModeProperty, value); }
		}
		public static readonly DependencyProperty OptionsModeProperty =
			DependencyProperty.Register("OptionsMode", typeof(BackstagePreviewOptionsMode), typeof(BackstageDocumentPreview), new PropertyMetadata(BackstagePreviewOptionsMode.Printing, (d, e) => ((BackstageDocumentPreview)d).IsPrintOptionsVisible = ((BackstageDocumentPreview)d).OptionsMode == BackstagePreviewOptionsMode.Printing));
		public bool IsPrintOptionsVisible {
			get { return (bool)GetValue(IsPrintOptionsVisibleProperty); }
			protected set { SetValue(IsPrintOptionsVisiblePropertyKey, value); }
		}
		static readonly DependencyPropertyKey IsPrintOptionsVisiblePropertyKey =
			DependencyProperty.RegisterReadOnly("IsPrintOptionsVisible", typeof(bool), typeof(BackstageDocumentPreview), new PropertyMetadata(true));
		public static readonly DependencyProperty IsPrintOptionsVisibleProperty = IsPrintOptionsVisiblePropertyKey.DependencyProperty;
		public string SelectedFormat {
			get { return (string)GetValue(SelectedFormatProperty); }
			set { SetValue(SelectedFormatProperty, value); }
		}
		public static readonly DependencyProperty SelectedFormatProperty =
			DependencyProperty.Register("SelectedFormat", typeof(string), typeof(BackstageDocumentPreview), new PropertyMetadata(null));
		public IEnumerable<string> ExportFormats {
			get { return (IEnumerable<string>)GetValue(ExportFormatsProperty); }
			protected set { SetValue(ExportFormatsPropertyKey, value); }
		}
		static readonly DependencyPropertyKey ExportFormatsPropertyKey =
			DependencyProperty.RegisterReadOnly("ExportFormats", typeof(IEnumerable<string>), typeof(BackstageDocumentPreview), new PropertyMetadata(null));
		public static readonly DependencyProperty ExportFormatsProperty = ExportFormatsPropertyKey.DependencyProperty;
		public object CustomSettingsHeader {
			get { return (object)GetValue(CustomSettingsHeaderProperty); }
			set { SetValue(CustomSettingsHeaderProperty, value); }
		}
		public static readonly DependencyProperty CustomSettingsHeaderProperty =
			DependencyProperty.Register("CustomSettingsHeader", typeof(object), typeof(BackstageDocumentPreview), new PropertyMetadata(null));
		public object CustomSettings {
			get { return (object)GetValue(CustomSettingsProperty); }
			set { SetValue(CustomSettingsProperty, value); }
		}
		public static readonly DependencyProperty CustomSettingsProperty =
			DependencyProperty.Register("CustomSettings", typeof(object), typeof(BackstageDocumentPreview), new PropertyMetadata(null));
		public BackstageDocumentPreview() {
			try {
				Printers = new List<PrinterViewModel>();
				LoadPrinters();
			} catch(Exception ex) {
				IsPrintersAvailable = false;
				PrintersErrorMessage = ex.Message;
			}
			ExportFormats = Enum.GetNames(typeof(ExportFormat)).Except(new string[] { "Prnx" });
			SelectedFormat = ExportFormats.FirstOrDefault();
		}
		void LoadPrinters() {
			var imageConverter = new PrinterTypeToImageConverter();
			using (var printContainer = new PrinterItemContainer()) {
				foreach (var item in printContainer.Items) {
					var model = new PrinterViewModel() {
						IsOffline = item.PrinterType.HasFlag(PrinterType.Offline),
						DisplayName = item.DisplayName,
						Name = item.FullName,
						Status = item.Status,
						ImageUri = imageConverter.Convert(item.PrinterType, null, null, null).ToString()
					};
					Printers.Add(model);
					if (item.PrinterType.HasFlag(PrinterType.Default) && !model.IsOffline)
						SelectedPrinter = model;
				}
				if (SelectedPrinter == null)
					SelectedPrinter = Printers.FirstOrDefault(x => !x.IsOffline);
			}
		}
	}
	public class CustomSettingsHeaderedContentControl : HeaderedContentControl {
		static CustomSettingsHeaderedContentControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomSettingsHeaderedContentControl), new FrameworkPropertyMetadata(typeof(CustomSettingsHeaderedContentControl)));
		}
	}
	public class PrinterViewModel {
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public string ImageUri { get; set; }
		public string Status { get; set; }
		public bool IsOffline { get; set; }
	}
}
