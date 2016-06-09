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

using DevExpress.XtraSpreadsheet.Forms;
using System;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class PageSetupControl : UserControl {
		public PageSetupControl(PageSetupViewModel viewModel) {
			InitializeComponent();
			DataContext = viewModel;
		}
		void tabControl_SelectionChanging(object sender, Core.TabControlSelectionChangingEventArgs e) {
			PageSetupViewModel viewModel = DataContext as PageSetupViewModel;
			ValidatePageSetupSelectionChanging(viewModel, e);
		}
		public bool ValidatePageSetupChanges() {
			PageSetupViewModel viewModel = DataContext as PageSetupViewModel;
			if (tabControl.SelectedValue.Equals(tabPage))
				if (!viewModel.PageValidate())
					return false;
			if (tabControl.SelectedValue.Equals(tabMargins))
				if (!viewModel.MarginsValidate())
					return false;
			if (tabControl.SelectedValue.Equals(tabHeaderFooter))
				if (!viewModel.HeaderFooterValidate())
					return false;
			if (tabControl.SelectedValue.Equals(tabSheet))
				if (!viewModel.SheetValidate())
					return false;
			return true;
		}
		void ValidatePageSetupSelectionChanging(PageSetupViewModel viewModel, Core.TabControlSelectionChangingEventArgs e) {
			if (e.OldSelectedItem == null)
				return;
			if (e.OldSelectedItem.Equals(tabPage))
				if (!viewModel.PageValidate()) {
					e.Cancel = true;
					return;
				}
			if (e.OldSelectedItem.Equals(tabMargins))
				if (!viewModel.MarginsValidate()) {
					e.Cancel = true;
					return;
				}
			if (e.OldSelectedItem.Equals(tabHeaderFooter))
				if (!viewModel.HeaderFooterValidate()) {
					e.Cancel = true;
					return;
				}
			if (e.OldSelectedItem.Equals(tabSheet))
				if (!viewModel.SheetValidate()) {
					e.Cancel = true;
					return;
				}
			e.Cancel = false;
		}
	}
}
