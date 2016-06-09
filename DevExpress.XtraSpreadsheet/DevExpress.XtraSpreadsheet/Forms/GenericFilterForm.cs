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
	public partial class GenericFilterForm : XtraForm {
		readonly GenericFilterViewModel viewModel;
		public GenericFilterForm(GenericFilterViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			SetBindingsForm();
			if (!viewModel.IsDateTimeFilter) {
				cbFilterValue.Width += edtValueDatePicker.Bounds.Right - cbFilterValue.Right;
				cbSecondaryFilterValue.Width += edtSecondaryValueDatePicker.Bounds.Right - cbSecondaryFilterValue.Right;
			}
		}
		protected GenericFilterForm() {
			InitializeComponent();
		}
		void SetBindingsForm() {
			this.cbFilterOperator.Properties.DataSource = viewModel.FilterOperatorDataSource;
			this.cbFilterOperator.Properties.ValueMember = "Value";
			this.cbFilterOperator.Properties.DisplayMember = "Text";
			this.cbFilterOperator.DataBindings.Add("EditValue", viewModel, "FilterOperator", true, DataSourceUpdateMode.OnPropertyChanged);
			this.cbSecondaryFilterOperator.Properties.DataSource = viewModel.FilterOperatorDataSource;
			this.cbSecondaryFilterOperator.Properties.ValueMember = "Value";
			this.cbSecondaryFilterOperator.Properties.DisplayMember = "Text";
			this.cbSecondaryFilterOperator.DataBindings.Add("EditValue", viewModel, "SecondaryFilterOperator", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblColumnName.DataBindings.Add("Text", viewModel, "ColumnCaption", true, DataSourceUpdateMode.OnPropertyChanged);
			this.cbFilterValue.DataBindings.Add("EditValue", viewModel, "FilterValue", true, DataSourceUpdateMode.OnPropertyChanged);
			this.cbSecondaryFilterValue.DataBindings.Add("EditValue", viewModel, "SecondaryFilterValue", true, DataSourceUpdateMode.OnPropertyChanged);
			this.rgrpAndOr.DataBindings.Add("EditValue", viewModel, "OperatorAnd", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtValueDatePicker.DataBindings.Add("Visible", viewModel, "IsDateTimeFilter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtSecondaryValueDatePicker.DataBindings.Add("Visible", viewModel, "IsDateTimeFilter", true, DataSourceUpdateMode.OnPropertyChanged);
			AppendItems(this.cbFilterValue.Properties.Items, viewModel.UniqueFilterValues);
			AppendItems(this.cbSecondaryFilterValue.Properties.Items, viewModel.UniqueFilterValues);
		}
		void AppendItems(DevExpress.XtraEditors.Controls.ComboBoxItemCollection items, IList<string> values) {
			int count = values.Count;
			for (int i = 0; i < count; i++)
				items.Add(values[i]);
		}
		void btnOk_Click(object sender, EventArgs e) {
			if (viewModel.Validate()) {
				viewModel.ApplyChanges();
				this.DialogResult = DialogResult.OK;
			}
		}
		void edtValueDatePicker_EditValueChanged(object sender, EventArgs e) {
			cbFilterValue.EditValue = edtValueDatePicker.Text;
		}
		void edtSecondaryValueDatePicker_EditValueChanged(object sender, EventArgs e) {
			cbSecondaryFilterValue.EditValue = edtSecondaryValueDatePicker.Text;
		}
	}
}
