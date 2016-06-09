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
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class InsertFunctionForm : XtraForm {
		#region Fields
		readonly InsertFunctionViewModel viewModel;
		#endregion
		public InsertFunctionForm() {
			InitializeComponent();
		}
		public InsertFunctionForm(InsertFunctionViewModel viewModel) {
			this.viewModel = viewModel;
			InitializeComponent();
			SetBindingsCategories();
		}
		void SetBindingsCategories() {
			this.edtCategories.Properties.DataSource = viewModel.GetFunctionCategories();
			this.edtCategories.DataBindings.Add("EditValue", viewModel, "Category", false, DataSourceUpdateMode.OnPropertyChanged);
			this.lbFunctions.DataBindings.Add("DataSource", viewModel, "Functions", false, DataSourceUpdateMode.Never);
			this.lbFunctions.DataBindings.Add("SelectedIndex", viewModel, "FunctionIndex", false, DataSourceUpdateMode.OnPropertyChanged);
			this.lblDescription.DataBindings.Add("Text", viewModel, "FunctionDescription", false, DataSourceUpdateMode.Never);
			this.lblShortDescription.DataBindings.Add("Text", viewModel, "FunctionShortDescription", false, DataSourceUpdateMode.Never);
		}
		private void btnOk_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
		}
		private void lbFunctions_DoubleClick(object sender, EventArgs e) {
			if(lbFunctions.SelectedIndex == -1)
				return;
			btnOk_Click(sender, e);
		}
	}
}
