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
	public partial class HeaderFooterForm : XtraForm {
		#region Fields
		readonly HeaderFooterViewModel viewModel;
		#endregion
		public HeaderFooterForm() {
		}
		public HeaderFooterForm(HeaderFooterViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			InitializeTabPageAndBindingHeaderFooter();
		}
		void InitializeTabPageAndBindingHeaderFooter() {
			if (!viewModel.DifferentOddEven && !viewModel.DifferentFirstPage) {
				this.xtraTabFirstHeaderFooter.PageVisible = false;
				this.xtraTabOddHeaderFooter.PageVisible = false;
				this.xtraTabEvenHeaderFooter.PageVisible = false;
				SetBindingHeaderFooter();
			}
			else if (!viewModel.DifferentOddEven && viewModel.DifferentFirstPage) {
				this.xtraTabOddHeaderFooter.PageVisible = false;
				this.xtraTabEvenHeaderFooter.PageVisible = false;
				SetBindingHeaderFooter();
				SetBindingFirstHeaderFooter();
			}
			else if (viewModel.DifferentOddEven && !viewModel.DifferentFirstPage) {
				this.xtraTabHeaderFooter.PageVisible = false;
				this.xtraTabFirstHeaderFooter.PageVisible = false;
				SetBindingOddHeaderFooter();
				SetBindingEvenHeaderFooter();
			}
			else {
				this.xtraTabHeaderFooter.PageVisible = false;
				SetBindingFirstHeaderFooter();
				SetBindingOddHeaderFooter();
				SetBindingEvenHeaderFooter();
			}
		}
		void SetBindingHeaderFooter() {
			this.headerFooterControl.DataBindings.Add("LeftHeader", viewModel, "OddLeftHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.headerFooterControl.DataBindings.Add("CenterHeader", viewModel, "OddCenterHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.headerFooterControl.DataBindings.Add("RightHeader", viewModel, "OddRightHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.headerFooterControl.DataBindings.Add("LeftFooter", viewModel, "OddLeftFooter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.headerFooterControl.DataBindings.Add("CenterFooter", viewModel, "OddCenterFooter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.headerFooterControl.DataBindings.Add("RightFooter", viewModel, "OddRightFooter", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetBindingFirstHeaderFooter() {
			this.firstHeaderFooterControl.DataBindings.Add("LeftHeader", viewModel, "FirstLeftHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.firstHeaderFooterControl.DataBindings.Add("CenterHeader", viewModel, "FirstCenterHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.firstHeaderFooterControl.DataBindings.Add("RightHeader", viewModel, "FirstRightHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.firstHeaderFooterControl.DataBindings.Add("LeftFooter", viewModel, "FirstLeftFooter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.firstHeaderFooterControl.DataBindings.Add("CenterFooter", viewModel, "FirstCenterFooter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.firstHeaderFooterControl.DataBindings.Add("RightFooter", viewModel, "FirstRightFooter", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetBindingOddHeaderFooter() {
			this.oddHeaderFooterControl.DataBindings.Add("LeftHeader", viewModel, "OddLeftHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.oddHeaderFooterControl.DataBindings.Add("CenterHeader", viewModel, "OddCenterHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.oddHeaderFooterControl.DataBindings.Add("RightHeader", viewModel, "OddRightHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.oddHeaderFooterControl.DataBindings.Add("LeftFooter", viewModel, "OddLeftFooter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.oddHeaderFooterControl.DataBindings.Add("CenterFooter", viewModel, "OddCenterFooter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.oddHeaderFooterControl.DataBindings.Add("RightFooter", viewModel, "OddRightFooter", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetBindingEvenHeaderFooter() {
			this.evenHeaderFooterControl.DataBindings.Add("LeftHeader", viewModel, "EvenLeftHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.evenHeaderFooterControl.DataBindings.Add("CenterHeader", viewModel, "EvenCenterHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.evenHeaderFooterControl.DataBindings.Add("RightHeader", viewModel, "EvenRightHeader", true, DataSourceUpdateMode.OnPropertyChanged);
			this.evenHeaderFooterControl.DataBindings.Add("LeftFooter", viewModel, "EvenLeftFooter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.evenHeaderFooterControl.DataBindings.Add("CenterFooter", viewModel, "EvenCenterFooter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.evenHeaderFooterControl.DataBindings.Add("RightFooter", viewModel, "EvenRightFooter", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void btnOk_Click(object sender, EventArgs e) {
			if (viewModel.ValidateHeaderFooter()) {
				viewModel.IsUpdateAllowed = true;
				viewModel.ApplyChanges();
				this.DialogResult = DialogResult.OK;
			}
		}
		private void btnCancel_Click(object sender, EventArgs e) {
			viewModel.IsUpdateAllowed = false;
			this.DialogResult = DialogResult.OK;
		}
	}
}
