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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class Top10FilterForm : XtraForm {
		readonly Top10FilterViewModel viewModel;
		public Top10FilterForm(Top10FilterViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			SetBindingsForm();
		}
		protected Top10FilterForm() {
			InitializeComponent();
		}
		void SetBindingsForm() {
			this.cbFilterOrder.Properties.DataSource = viewModel.OrderDataSource;
			this.cbFilterOrder.Properties.ValueMember = "Value";
			this.cbFilterOrder.Properties.DisplayMember = "Text";
			this.cbFilterOrder.DataBindings.Add("EditValue", viewModel, "IsTop", true, DataSourceUpdateMode.OnPropertyChanged);
			this.cbFilterType.Properties.DataSource = viewModel.TypeDataSource;
			this.cbFilterType.Properties.ValueMember = "Value";
			this.cbFilterType.Properties.DisplayMember = "Text";
			this.cbFilterType.DataBindings.Add("EditValue", viewModel, "IsPercent", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtValue.DataBindings.Add("EditValue", viewModel, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void btnOk_Click(object sender, EventArgs e) {
			if (viewModel.Validate()) {
				viewModel.ApplyChanges();
				this.DialogResult = DialogResult.OK;
			}
		}
	}
}
