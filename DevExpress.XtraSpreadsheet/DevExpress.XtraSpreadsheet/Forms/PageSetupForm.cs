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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class PageSetupForm : XtraForm {
		#region Fields
		readonly PageSetupViewModel viewModel;
		#endregion
		PageSetupForm() {
			InitializeComponent();
		}
		public PageSetupForm(PageSetupViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			SetBinidingsForm();
			SubscribeEvents();
			SetActiveTabPage(viewModel.InitialTabPage);
			InitializeButtonsOnActiveTabPage();
		}
		void SubscribeEvents() {
			this.tabControl.SelectedPageChanging += OnPageChanging;
			this.tabControl.SelectedPageChanged += OnPageChanged;
		}
		void SetBinidingsForm() {
			SetBindingsPage();
			SetBindingsMargins();
			SetBindingsHeaderFooter();
			SetBindingsSheet();
		}
		void SetBindingsPage() {
			this.rgrpOrientation.DataBindings.Add("EditValue", viewModel, "OrientationPortrait", true, DataSourceUpdateMode.OnPropertyChanged);
			this.rgrpFitToPage.DataBindings.Add("EditValue", viewModel, "FitToPage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtScale.DataBindings.Add("EditValue", viewModel, "Scale", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtFitToWidth.DataBindings.Add("EditValue", viewModel, "FitToWidth", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtFitToHeight.DataBindings.Add("EditValue", viewModel, "FitToHeight", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtPaperSize.Properties.DataSource = viewModel.PaperTypes;
			this.edtPaperSize.DataBindings.Add("EditValue", viewModel, "PaperType", true, DataSourceUpdateMode.OnPropertyChanged, String.Empty);
			this.edtPrintQuality.Properties.DataSource = viewModel.PrintQuality;
			this.edtPrintQuality.DataBindings.Add("EditValue", viewModel, "PrintQualityMode", true, DataSourceUpdateMode.OnPropertyChanged, String.Empty);
			this.edtFirstPageNumber.DataBindings.Add("EditValue", viewModel, "FirstPageNumber", true, DataSourceUpdateMode.OnValidation);
		}
		void SetBindingsMargins() {
			this.pageMarginsPreviewControl.DataBindings.Add("TopMargin", viewModel, "TopMargin", true, DataSourceUpdateMode.OnPropertyChanged);
			this.pageMarginsPreviewControl.DataBindings.Add("BottomMargin", viewModel, "BottomMargin", true, DataSourceUpdateMode.OnPropertyChanged);
			this.pageMarginsPreviewControl.DataBindings.Add("LeftMargin", viewModel, "LeftMargin", true, DataSourceUpdateMode.OnPropertyChanged);
			this.pageMarginsPreviewControl.DataBindings.Add("RightMargin", viewModel, "RightMargin", true, DataSourceUpdateMode.OnPropertyChanged);
			this.pageMarginsPreviewControl.DataBindings.Add("HeaderMargin", viewModel, "HeaderMargin", true, DataSourceUpdateMode.OnPropertyChanged);
			this.pageMarginsPreviewControl.DataBindings.Add("FooterMargin", viewModel, "FooterMargin", true, DataSourceUpdateMode.OnPropertyChanged);
			this.pageMarginsPreviewControl.DataBindings.Add("DrawPanelPortraitOrientation", viewModel, "OrientationPortrait", false, DataSourceUpdateMode.OnPropertyChanged);
			this.pageMarginsPreviewControl.DataBindings.Add("DrawPanelLandscapeOrientation", viewModel, "OrientationPortrait", true, DataSourceUpdateMode.OnPropertyChanged);
			this.pageMarginsPreviewControl.DataBindings.Add("IsCenterHorizontally", viewModel, "CenterHorizontally", false, DataSourceUpdateMode.OnPropertyChanged);
			this.pageMarginsPreviewControl.DataBindings.Add("IsCenterVertically", viewModel, "CenterVertically", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetBindingsHeaderFooter() {
			this.edtHeader.Properties.DataSource = viewModel.PredefinedHeaderFooterList;
			this.edtHeader.DataBindings.Add("EditValue", viewModel, "PredefinedHeaderValueForList", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtFooter.Properties.DataSource = viewModel.PredefinedHeaderFooterList;
			this.edtFooter.DataBindings.Add("EditValue", viewModel, "PredefinedFooterValueForList", true, DataSourceUpdateMode.OnPropertyChanged);
			this.chkDifferentOddAndEvenPages.DataBindings.Add("EditValue", viewModel, "DifferentOddEven", true, DataSourceUpdateMode.OnPropertyChanged);
			this.chkDifferentFirstPage.DataBindings.Add("EditValue", viewModel, "DifferentFirstPage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.chkScaleWithDocument.DataBindings.Add("EditValue", viewModel, "ScaleWithDocument", true, DataSourceUpdateMode.OnPropertyChanged);
			this.chkAlignWithPageMargins.DataBindings.Add("EditValue", viewModel, "AlignWithMargins", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtHeader.DataBindings.Add("Enabled", viewModel, "PredefinedHeaderFooterListEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtFooter.DataBindings.Add("Enabled", viewModel, "PredefinedHeaderFooterListEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			this.headerPreview.DataBindings.Add("HeaderFooterValue", viewModel, "OddHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.footerPreview.DataBindings.Add("HeaderFooterValue", viewModel, "OddFooter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.headerPreview.DataBindings.Add("Provider", viewModel, "Provider", true, DataSourceUpdateMode.OnPropertyChanged);
			this.footerPreview.DataBindings.Add("Provider", viewModel, "Provider", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetBindingsSheet() {
			this.edtPrintArea.DataBindings.Add("EditValue", viewModel, "PrintArea", true, DataSourceUpdateMode.OnValidation);
			this.chkGridlines.DataBindings.Add("EditValue", viewModel, "PrintGridlines", true, DataSourceUpdateMode.OnPropertyChanged);
			this.chkDraftQuality.DataBindings.Add("EditValue", viewModel, "Draft", true, DataSourceUpdateMode.OnPropertyChanged);
			this.chkRowAndColumnHeadings.DataBindings.Add("EditValue", viewModel, "PrintHeadings", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtComments.Properties.DataSource = viewModel.CommentsPrintModeList;
			this.edtComments.DataBindings.Add("EditValue", viewModel, "CommentsPrintMode", true, DataSourceUpdateMode.OnPropertyChanged, String.Empty);
			this.edtCellErrorsAs.Properties.DataSource = viewModel.ErrorsPrintModeList;
			this.edtCellErrorsAs.DataBindings.Add("EditValue", viewModel, "ErrorsPrintMode", true, DataSourceUpdateMode.OnPropertyChanged, String.Empty);
			this.rgrpPageOrder.DataBindings.Add("EditValue", viewModel, "DownThenOver", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		protected internal void SetActiveTabPage(PageSetupFormInitialTabPage intitialTabPage) {
			this.tabControl.SelectedTabPageIndex = (int)intitialTabPage;
		}
		void InitializeButtonsOnActiveTabPage() {
			this.btnPrint.Parent = this.tabControl.SelectedTabPage;
			this.btnPrintPreview.Parent = this.tabControl.SelectedTabPage;
		}
		bool ValidatePageSetupChanges() {
			if (tabControl.SelectedTabPage.Equals(xtraTabPage))
				if (!viewModel.PageValidate())
					return false;
			if (tabControl.SelectedTabPage.Equals(xtraTabMargins))
				if (!viewModel.MarginsValidate())
					return false;
			if (tabControl.SelectedTabPage.Equals(xtraTabHeaderFooter))
				if (!viewModel.HeaderFooterValidate())
					return false;
			if (tabControl.SelectedTabPage.Equals(xtraTabSheet))
				if (!viewModel.SheetValidate())
					return false;
			return true;
		}
		void ApplyPageSetupChanges() {
			viewModel.ApplyChanges();
			this.DialogResult = DialogResult.OK;
			Close();
		}
		void OnPageChanging(object sender, XtraTab.TabPageChangingEventArgs e) {
			TopMost = false;
			try {
				if (!ValidatePageSetupChanges())
					e.Cancel = true;
			}
			finally {
				TopMost = true;
			}
		}
		void OnPageChanged(object sender, XtraTab.TabPageChangedEventArgs e) {
			XtraTab.XtraTabPage currentPage = (sender as XtraTab.XtraTabControl).SelectedTabPage;
			this.btnPrint.Parent = currentPage;
			this.btnPrintPreview.Parent = currentPage;
		}
		void btnPrint_Click(object sender, EventArgs e) {
			TopMost = false;
			viewModel.DocumentModel.BeginUpdate();
			try {
				if (ValidatePageSetupChanges()) {
					ApplyPageSetupChanges();
					Commands.PrintCommand command = new Commands.PrintCommand(viewModel.Control);
					command.Execute();
				}
			}
			finally {
				TopMost = true;
				viewModel.DocumentModel.EndUpdate();
			}
		}
		void btnPrintPreview_Click(object sender, EventArgs e) {
			TopMost = false;
			viewModel.DocumentModel.BeginUpdate();
			try {
				if (ValidatePageSetupChanges()) {
					ApplyPageSetupChanges();
					Commands.PrintPreviewCommand command = new Commands.PrintPreviewCommand(viewModel.Control);
					command.Execute();
				}
			}
			finally {
				TopMost = true;
				viewModel.DocumentModel.EndUpdate();
			}
		}
		void btnCustomHeaderFooter_Click(object sender, EventArgs e) {
			ShowHeaderFooterForm();
		}
		void ShowHeaderFooterForm() {
			HeaderFooterViewModel headerFooterViewModel = new HeaderFooterViewModel(viewModel);
			ShowHeaderFooterForm(headerFooterViewModel);
		}
		void ShowHeaderFooterForm(HeaderFooterViewModel headerFooterViewModel) {
			TopMost = false;
			SpreadsheetControl control = (SpreadsheetControl)headerFooterViewModel.Control;
			control.ShowHeaderFooterForm(headerFooterViewModel);
			if (headerFooterViewModel.IsUpdateAllowed)
				viewModel.UpdateHeaderFooter(headerFooterViewModel);
		}
		void btnOk_Click(object sender, EventArgs e) {
			TopMost = false;
			try {
				if (ValidatePageSetupChanges())
					ApplyPageSetupChanges();
			}
			finally {
				TopMost = true;
			}
		}
	}
}
