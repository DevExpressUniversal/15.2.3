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
	public partial class PivotTableDateFilterForm : XtraForm {
		readonly PivotTableDateFiltersViewModel viewModel;
		PivotTableDateFilterForm() {
			InitializeComponent();
		}
		public PivotTableDateFilterForm(PivotTableDateFiltersViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			SetBindingsForm();
		}
		void SetBindingsForm() {
			this.Text = this.Text + " (" + viewModel.FieldName + ")";
			this.edtFilterOperator.Properties.DataSource = viewModel.FilterTypesDataSource;
			this.edtFilterOperator.DataBindings.Add("EditValue", viewModel, "FilterTypeValue", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblAnd.DataBindings.Add("Visible", viewModel, "IsTwoValueFilter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.dateEdit.DataBindings.Add("Visible", viewModel, "IsOneValueFilter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.dateEdit.DataBindings.Add("EditValue", viewModel, "FirstDate", true, DataSourceUpdateMode.OnPropertyChanged);
			this.dateEdit1.DataBindings.Add("Visible", viewModel, "IsTwoValueFilter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.dateEdit1.DataBindings.Add("EditValue", viewModel, "FirstDate", true, DataSourceUpdateMode.OnPropertyChanged);
			this.dateEdit2.DataBindings.Add("Visible", viewModel, "IsTwoValueFilter", true, DataSourceUpdateMode.OnPropertyChanged);
			this.dateEdit2.DataBindings.Add("EditValue", viewModel, "SecondDate", true, DataSourceUpdateMode.OnPropertyChanged);
			this.dateEdit.Properties.NullDateCalendarValue = DateTime.Today;
			this.dateEdit1.Properties.NullDateCalendarValue = DateTime.Today;
			this.dateEdit2.Properties.NullDateCalendarValue = DateTime.Today;
		}
		void btnOk_Click(object sender, EventArgs e) {
			SetViewModelStringValues();
			if (viewModel.Validate()) {
				viewModel.ApplyChanges();
				this.DialogResult = DialogResult.OK;
			}
		}
		void SetViewModelStringValues() {
			if (viewModel.IsOneValueFilter)
				viewModel.FirstStringValue = dateEdit.Text;
			else {
				viewModel.FirstStringValue = dateEdit1.Text;
				viewModel.SecondStringValue = dateEdit2.Text;
			}
		}
		void dateEdit_EditValueChanged(object sender, EventArgs e) {
			CorrectDateEditText(dateEdit);
		}
		void dateEdit1_EditValueChanged(object sender, EventArgs e) {
			CorrectDateEditText(dateEdit1);
		}
		void dateEdit2_EditValueChanged(object sender, EventArgs e) {
			CorrectDateEditText(dateEdit2);
		}
		void CorrectDateEditText(DateEdit dateEdit) {
			DateTime date = dateEdit.DateTime;
			if (date.Year < 1900)
				dateEdit.Text = null;
			else {
				string formatString = date.Hour > 0 || date.Minute > 0 || date.Second > 0 ? "G" : "d";
				dateEdit.Properties.DisplayFormat.FormatString = formatString;
				dateEdit.Properties.EditFormat.FormatString = formatString;
			}
		}
	}
}
