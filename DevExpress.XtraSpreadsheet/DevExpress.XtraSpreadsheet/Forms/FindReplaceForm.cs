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
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class FindReplaceForm : XtraForm {
		readonly FindReplaceViewModel viewModel;
		bool alreadyActivated;
		FindReplaceForm() {
			InitializeComponent();
		}
		public FindReplaceForm(FindReplaceViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			tabControl.SelectedTabPageIndex = ViewModel.ReplaceMode ? 1 : 0;
			SetBindings();
			pageReplace.PageEnabled = viewModel.ReplaceEnabled; 
		}
		public FindReplaceViewModel ViewModel { get { return viewModel; } }
		protected internal virtual void SetBindings() {
			this.edtFindWhat.DataBindings.Add("Text", ViewModel, "FindWhat", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtReplaceWith.DataBindings.Add("Text", ViewModel, "ReplaceWith", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtReplaceWith.DataBindings.Add("Visible", ViewModel, "ReplaceMode", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblReplaceWith.DataBindings.Add("Visible", ViewModel, "ReplaceMode", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnReplace.DataBindings.Add("Visible", ViewModel, "ReplaceMode", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnReplaceAll.DataBindings.Add("Visible", ViewModel, "ReplaceMode", true, DataSourceUpdateMode.OnPropertyChanged);
			this.chkMatchCase.DataBindings.Add("EditValue", ViewModel, "MatchCase", true, DataSourceUpdateMode.OnPropertyChanged);
			this.chkMatchEntireCellContents.DataBindings.Add("EditValue", ViewModel, "MatchEntireCellContents", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtSearchBy.Properties.DataSource = ViewModel.SearchByDataSource;
			this.edtSearchIn.Properties.DataSource = ViewModel.SearchInDataSource;
			this.edtSearchBy.DataBindings.Add("EditValue", ViewModel, "SearchBy", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtSearchIn.DataBindings.Add("EditValue", ViewModel, "SearchIn", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);
			if (alreadyActivated)
				return;
			alreadyActivated = true;
			edtFindWhat.Focus();
		}
		void tabControl_SelectedPageChanging(object sender, TabPageChangingEventArgs e) {
			if (ViewModel == null)
				return;
			XtraTabPage activePage = tabControl.SelectedTabPage;
			viewModel.ReplaceMode = (activePage == pageFind);
			XtraTabPage newPage = viewModel.ReplaceMode ? pageReplace : pageFind;
			if (activePage.Controls.Contains(panel)) {
				activePage.Controls.Remove(panel);
				newPage.Controls.Add(panel);
			}
			this.edtSearchBy.Properties.DataSource = ViewModel.SearchByDataSource;
			this.edtSearchIn.Properties.DataSource = ViewModel.SearchInDataSource;
		}
		void btnClose_Click(object sender, EventArgs e) {
			Close();
		}
		void btnFindNext_Click(object sender, EventArgs e) {
			if (ViewModel != null)
				ViewModel.FindNext();
		}
		void btnReplace_Click(object sender, EventArgs e) {
			if (ViewModel != null)
				ViewModel.ReplaceNext();
		}
		void btnReplaceAll_Click(object sender, EventArgs e) {
			if (ViewModel != null)
				ViewModel.ReplaceAll();
		}
	}
}
