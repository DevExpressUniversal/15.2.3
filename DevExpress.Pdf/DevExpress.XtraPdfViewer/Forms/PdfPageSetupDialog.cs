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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using DevExpress.Printing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Preview;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPdfViewer.Native;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.Forms {
	public partial class PdfPageSetupDialog : XtraForm {
		const string enabledPropertyName = "Enabled";
		static int CalcWidth(BaseEdit editor) {
			Padding margin = editor.Margin;
			return editor.CalcBestSize().Width + margin.Left + margin.Right;
		}
		int documentPageCount;
		string fitScaleText;
		string pagePreviewText;
		PdfPrintDialogViewModel model;
		Size PreviewImageSize { get { return Size.Subtract(pePreviewImage.ClientSize, new Size(6, 6)); } }
		public PdfPrinterSettings PrinterSettings { get { return model.PrinterSettings; } }
		public PdfPageSetupDialog() {
			InitializeComponent();
		}
		public void Initialize(PdfDocumentState documentState, string documentName, PdfViewer viewer, PdfPrinterSettings printerSettings) {
			documentPageCount = documentState.Document.Pages.Count;
			fitScaleText = lbFitScale.Text;
			pagePreviewText = cnPagePreviewNumber.TextStringFormat;
			Size previewImageSize = PreviewImageSize;
			model = new PdfPrintDialogViewModel(documentState, new Size(previewImageSize.Width, previewImageSize.Height), viewer.CurrentPageNumber,
				viewer.MaxPrintingDpi, message => XtraMessageBox.Show(LookAndFeel, this, message, Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK, printerSettings);
			Skin skin = PdfViewerSkins.GetSkin(LookAndFeel);
			if (skin != null) {
				SkinElement skinElement = skin[PdfViewerSkins.Selection];
				if (skinElement != null) {
					SkinColor skinColor = skinElement.Color;
					if (skinColor != null) {
						Color color = skinColor.BackColor;
						model.PreviewPrintAreaBorderColor = Color.FromArgb(color.R, color.G, color.B);
					}
				}
			}
			PrinterImagesContainer imagesContainer = new PrinterImagesContainer();
			RepositoryItemImageComboBox properties = icbInstalledPrinters.Properties;
			properties.LargeImages = imagesContainer.LargeImages;
			properties.SmallImages = imagesContainer.SmallImages;
			ImageComboBoxItemCollection items = properties.Items;
			foreach (PrinterItem item in model.PrinterItems)
				items.Add(new ImageComboBoxItem(item.DisplayName, imagesContainer.GetImageIndex(item)));
			BindSelectedIndexProperty(icbInstalledPrinters, PdfPrintDialogViewModel.PrinterIndexPropertyName);
			BindTextProperty(lbPrinterStatus, PdfPrintDialogViewModel.PrinterStatusPropertyName);
			BindTextProperty(lbPrinterLocation, PdfPrintDialogViewModel.PrinterLocationPropertyName);
			BindTextProperty(lbPrinterComment, PdfPrintDialogViewModel.PrinterCommentPropertyName);
			BindTextProperty(lbDocumentsInQueue, PdfPrintDialogViewModel.PrinterDocumentsInQueuePropertyName);
			spCopies.DataBindings.Add("Value", model, PdfPrintDialogViewModel.CopiesPropertyName, true, DataSourceUpdateMode.OnPropertyChanged);
			BindEnabledProperty(cbCollate, PdfPrintDialogViewModel.AllowCollatePropertyName, true, DataSourceUpdateMode.OnPropertyChanged);
			BindCheckedProperty(cbCollate, PdfPrintDialogViewModel.CollatePropertyName);
			BindRadioButton(rbPrintRangeAll, PdfPrintDialogViewModel.PrintRangePropertyName, PrintRange.AllPages);
			BindRadioButton(rbPrintRangeCurrent, PdfPrintDialogViewModel.PrintRangePropertyName, PrintRange.CurrentPage);
			BindRadioButton(rbPrintRangeSome, PdfPrintDialogViewModel.PrintRangePropertyName, PrintRange.SomePages);
			BindEnabledProperty(tePageRange, PdfPrintDialogViewModel.AllowSomePagesPropertyName, true, DataSourceUpdateMode.OnPropertyChanged);
			BindTextProperty(tePageRange, PdfPrintDialogViewModel.PageRangeTextPropertyName, true, DataSourceUpdateMode.OnValidation);
			BindEnabledProperty(lbPageRangeExample, PdfPrintDialogViewModel.AllowSomePagesPropertyName);
			BindSelectedIndexProperty(cbPaperSource, PdfPrintDialogViewModel.PaperSourceIndexPropertyName);
			BindTextProperty(beBrowse, PdfPrintDialogViewModel.PrintFileNamePropertyName, false, DataSourceUpdateMode.OnValidation);
			BindEnabledProperty(beBrowse, PdfPrintDialogViewModel.PrintToFilePropertyName, true, DataSourceUpdateMode.OnValidation);
			BindCheckedProperty(cbPrintToFile, PdfPrintDialogViewModel.PrintToFilePropertyName);
			BindRadioButton(rbPageScaleFit, PdfPrintDialogViewModel.ScaleModePropertyName, PdfPrintScaleMode.Fit);
			BindRadioButton(rbPageScaleActualSize, PdfPrintDialogViewModel.ScaleModePropertyName, PdfPrintScaleMode.ActualSize);
			BindRadioButton(rbPageScaleCustom, PdfPrintDialogViewModel.ScaleModePropertyName, PdfPrintScaleMode.CustomScale);
			BindEnabledProperty(teCustomScale, PdfPrintDialogViewModel.AllowCustomScalePropertyName, true, DataSourceUpdateMode.OnPropertyChanged);
			BindTextProperty(teCustomScale, PdfPrintDialogViewModel.ScalePropertyName, false, DataSourceUpdateMode.OnValidation);
			lbFitScale.DataBindings.Add("Visible", model, PdfPrintDialogViewModel.ShowFitScaleTextPropertyName, true, DataSourceUpdateMode.OnPropertyChanged);
			BindEnabledProperty(lbPercent, PdfPrintDialogViewModel.AllowCustomScalePropertyName);
			BindRadioButton(rbAutoOrientation, PdfPrintDialogViewModel.PageOrientationPropertyName, PdfPrintPageOrientation.Auto);
			BindRadioButton(rbPortraitOrientation, PdfPrintDialogViewModel.PageOrientationPropertyName, PdfPrintPageOrientation.Portrait);
			BindRadioButton(rbLandscapeOrientation, PdfPrintDialogViewModel.PageOrientationPropertyName, PdfPrintPageOrientation.Landscape);
			BindTextProperty(spPrintingDpi, PdfPrintDialogViewModel.PrintingDpiPropertyName);
			pePreviewImage.SetModel(model);
			cnPagePreviewNumber.NavigatableControl = pePreviewImage;
			BindEnabledProperty(cnPagePreviewNumber, PdfPrintDialogViewModel.EnablePageNumberPreviewPropertyName);
			pePreviewImage.DataBindings.Add("Image", model, PdfPrintDialogViewModel.PreviewImagePropertyName);
			BindEnabledProperty(pePreviewImage, PdfPrintDialogViewModel.EnablePageNumberPreviewPropertyName);
			UpdatePaperSources();
			UpdateMaxDpi();
			UpdatePagePreviewPageNumberText();
			UpdateFitScale();
			model.PropertyChanged += new PropertyChangedEventHandler(ModelPropertyChanged);
			BindEnabledProperty(btnPrint, PdfPrintDialogViewModel.EnableToPrintPropertyName, false, DataSourceUpdateMode.Never);
			errorProvider.DataSource = model;
			if (model.HasException) {
				XtraMessageBox.Show(viewer.LookAndFeel, viewer.ParentForm, XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessagePrintError),
					XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageErrorCaption), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				spPrintingDpi.Enabled = false;
				spCopies.Enabled = false;
				btnPrinterPreferences.Enabled = false;
				cbPaperSource.Enabled = false;
				cbPrintToFile.Enabled = false;
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null)
					components.Dispose();
				if (model != null) {
					model.PropertyChanged -= new PropertyChangedEventHandler(ModelPropertyChanged);
					model.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			tlpPager.Height = tlpButtons.Height;
			spPrintingDpi.Refresh();
			spCopies.Refresh();
			ColumnStyle columnStyle = tlpProperties.ColumnStyles[1];
			columnStyle.SizeType = SizeType.Absolute;
			columnStyle.Width = Math.Max(CalcWidth(rbPageScaleCustom), CalcWidth(rbPrintRangeSome));
			foreach (Control control in tlpProperties.Controls) {
				BaseControl edit = control as BaseControl;
				if (edit != null) {
					Size bestSize = edit.CalcBestSize();
					Padding margin = edit.Margin;
					int column = tlpProperties.GetColumn(control);
					if (column == 3 || column == 0) {
						columnStyle = tlpProperties.ColumnStyles[column];
						columnStyle.SizeType = SizeType.Absolute;
						columnStyle.Width = Math.Max(columnStyle.Width, bestSize.Width + margin.Left + margin.Right);
					}
					if (control.Dock == DockStyle.Fill && column > 0) {
						RowStyle rowStyle = tlpProperties.RowStyles[tlpProperties.GetRow(control)];
						rowStyle.SizeType = SizeType.Absolute;
						rowStyle.Height = Math.Max(rowStyle.Height, bestSize.Height + margin.Top + margin.Bottom);
					}
				}
			}
		}
		protected override void OnResizeEnd(EventArgs e) {
			base.OnResizeEnd(e);
			UpdatePreviewSize();
		}
		void BindEnabledProperty(Control control, string propertyName) {
			control.DataBindings.Add(enabledPropertyName, model, propertyName, true);
		}
		void BindEnabledProperty(Control control, string propertyName, bool enableFormatting, DataSourceUpdateMode updateMode) {
			control.DataBindings.Add(enabledPropertyName, model, propertyName, enableFormatting, updateMode);
		}
		void BindTextProperty(Control control, string propertyName, bool enableFormatting, DataSourceUpdateMode updateMode) {
			control.DataBindings.Add("Text", model, propertyName, enableFormatting, updateMode);
		}
		void BindTextProperty(Control control, string propertyName) {
			BindTextProperty(control, propertyName, true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void BindSelectedIndexProperty(Control control, string propertyName) {
			control.DataBindings.Add("SelectedIndex", model, propertyName, true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void BindCheckedProperty(Control control, string propertyName) {
			control.DataBindings.Add("Checked", model, propertyName, true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void BindRadioButton(Control control, string propertyName, Enum value) {
			control.DataBindings.Add(new PdfRadioButtonBinding("EditValue", model, propertyName, value));
		}
		void UpdatePreviewSize() {
			if (model != null) {
				Size previewImageSize = PreviewImageSize;
				model.UpdatePreviewSize(previewImageSize.Width, previewImageSize.Height);
			}
		}
		void UpdatePaperSources() {
			ComboBoxItemCollection items = cbPaperSource.Properties.Items;
			items.Clear();
			cbPaperSource.Text = String.Empty;
			foreach (string paperSource in model.PaperSources)
				items.Add(paperSource);
			cbPaperSource.SelectedItem = model.DefaultPaperSourceName;
			model.PaperSource = model.DefaultPaperSourceName;
		}
		void UpdateMaxDpi() {
			spPrintingDpi.Properties.MaxValue = model.MaxDpi;
		}
		void UpdatePagePreviewPageNumberText() {
			string text = model.PrintRange == PrintRange.AllPages ? (String.Format(pagePreviewText, model.PagePreviewIndex + 1, documentPageCount) + " ") :
																	(String.Format(pagePreviewText, model.PagePreviewIndex + 1, model.PageCount) + " (" + model.CurrentPreviewPageNumber + ") ");
			cnPagePreviewNumber.TextStringFormat = "  " + text;
		}
		void UpdateFitScale() {
			lbFitScale.Text = fitScaleText + model.FitScale.ToString("p");
		}
		void ModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch (e.PropertyName) {
				case PdfPrintDialogViewModel.PaperSourcesPropertyName:
					UpdatePaperSources();
					break;
				case PdfPrintDialogViewModel.MaxDpiPropertyName:
					UpdateMaxDpi();
					break;
				case PdfPrintDialogViewModel.PageNumbersPropertyName:
				case PdfPrintDialogViewModel.PagePreviewIndexPropertyName:
				case PdfPrintDialogViewModel.CurrentPreviewPageNumberPropertyName:
					UpdatePagePreviewPageNumberText();
					break;
				case PdfPrintDialogViewModel.FitScalePropertyName:
					UpdateFitScale();
					break;
			}
		}
		void OnSplitterMoved(object sender, SplitterEventArgs e) {
			UpdatePreviewSize();
		}
		void OnPrinterPreferencesClick(object sender, EventArgs e) {
			model.ShowPreferences(Handle);
		}
		void OnBrowseClick(object sender, ButtonPressedEventArgs e) {
			using (SaveFileDialog fileDialog = new SaveFileDialog()) {
				fileDialog.Filter = XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.PRNFileFilter);
				fileDialog.CheckPathExists = false;
				fileDialog.OverwritePrompt = false;
				fileDialog.RestoreDirectory = true;
				if (fileDialog.ShowDialog() == DialogResult.OK)
					model.PrintFileName = fileDialog.FileName;
			}
		}
	}
}
