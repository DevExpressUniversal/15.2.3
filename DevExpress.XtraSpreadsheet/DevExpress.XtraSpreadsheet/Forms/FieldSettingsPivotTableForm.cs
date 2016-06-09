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

using DevExpress.Utils;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class FieldSettingsPivotTableForm : XtraForm {
		#region Fields
		readonly FieldSettingsPivotTableViewModel viewModel;
		#endregion
		FieldSettingsPivotTableForm() {
			InitializeComponent();
		}
		public FieldSettingsPivotTableForm(FieldSettingsPivotTableViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			InitializeSubtotalFunctionList();
			SetBindings();
		}
		#region Properties
		protected FieldSettingsPivotTableViewModel ViewModel { get { return viewModel; } }
		#endregion
		void SetBindings() {
			this.lblSourceNameValue.DataBindings.Add("Text", ViewModel, "SourceName", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtCustomName.DataBindings.Add("EditValue", ViewModel, "CustomName", false, DataSourceUpdateMode.OnValidation);
			this.chkAutomaticSubtotal.DataBindings.Add("EditValue", ViewModel, "IsAutomaticSubtotal", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkNoneSubtotal.DataBindings.Add("EditValue", ViewModel, "IsNoneSubtotal", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkCustomSubtotal.DataBindings.Add("EditValue", ViewModel, "IsCustomSubtotal", false, DataSourceUpdateMode.OnPropertyChanged);
			this.lbFunctions.DataBindings.Add("Enabled", ViewModel, "IsSubtotalFunctionListEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkIncludeNewItems.DataBindings.Add("EditValue", ViewModel, "IncludeNewItemsInFilter", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShowItemLabelsOutlineForm.DataBindings.Add("EditValue", ViewModel, "Outline", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkCompactForm.DataBindings.Add("EditValue", ViewModel, "Compact", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkSubtotalTop.DataBindings.Add("EditValue", ViewModel, "SubtotalTop", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShowItemLabelsTabularForm.DataBindings.Add("EditValue", ViewModel, "Tabular", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkRepeatItemLabels.DataBindings.Add("EditValue", ViewModel, "FillDownLabels", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkInsertBlankLine.DataBindings.Add("EditValue", ViewModel, "InsertBlankRow", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShowItemWithNoData.DataBindings.Add("EditValue", ViewModel, "ShowItemsWithNoData", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkInsertPageBreak.DataBindings.Add("EditValue", ViewModel, "InsertPageBreak", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		void InitializeSubtotalFunctionList() {
			this.lbFunctions.Items.AddRange(viewModel.FunctionList.ToArray());
			foreach (PivotFieldItemType value in Enum.GetValues(typeof(PivotFieldItemType)))
				if (ViewModel.Subtotal.HasFlag(value) && value != PivotFieldItemType.Blank && value != PivotFieldItemType.DefaultValue) 
					this.lbFunctions.SetSelected(this.lbFunctions.FindString(viewModel.FunctionTable[value]), true);
		}
		List<string> CastToListSelectedFunctions() {
			return this.lbFunctions.SelectedItems.Cast<string>().ToList();
		}
		private void btnOk_Click(object sender, EventArgs e) {
			ViewModel.SetSubtotal(CastToListSelectedFunctions());
			this.TopMost = false;
			try {
				if (ViewModel.Validate()) {
					ViewModel.ApplyChanges();
					this.DialogResult = DialogResult.OK;
					Close();
				}
			}
			finally {
				this.TopMost = true;
			}
		}
	}
}
