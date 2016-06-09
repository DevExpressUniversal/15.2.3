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
	public partial class PivotTableTop10FilterForm : XtraForm {
		readonly PivotTableTop10FiltersViewModel viewModel;
		PivotTableTop10FilterForm() {
			InitializeComponent();
		}
		public PivotTableTop10FilterForm(PivotTableTop10FiltersViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			SetBindingsForm();
		}
		void SetBindingsForm() {
			this.Text = this.Text + " (" + viewModel.FieldName + ")";
			this.edtTopBottom.Properties.DataSource = viewModel.TopBottomList;
			this.edtTopBottom.Properties.ValueMember = "Value";
			this.edtTopBottom.Properties.DisplayMember = "Text";
			this.edtTopBottom.DataBindings.Add("EditValue", viewModel, "FilterByTop", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtDataFields.Properties.DataSource = viewModel.DataFieldNames;
			this.edtDataFields.DataBindings.Add("EditValue", viewModel, "CurrentDataFieldName", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtFilterOperator.Properties.DataSource = viewModel.FilterTypesDataSource;
			this.edtFilterOperator.DataBindings.Add("EditValue", viewModel, "FilterTypeValue", true, DataSourceUpdateMode.OnPropertyChanged);
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
