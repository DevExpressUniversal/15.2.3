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
using System.Windows.Forms;
using System.Linq;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class DataFieldSettingsPivotTableForm : XtraForm {
		#region Fields
		readonly DataFieldSettingsPivotTableViewModel viewModel;
		bool isInitializeLists;
		#endregion
		DataFieldSettingsPivotTableForm() {
			InitializeComponent();
		}
		public DataFieldSettingsPivotTableForm(DataFieldSettingsPivotTableViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			isInitializeLists = true;
			InitializeBaseFieldListBox();
			InitializeBaseItemListBox();
			isInitializeLists = false;
			SetActiveTabPage(viewModel.InitialTabPage);
			SetBindings();
		}
		#region Properties
		protected DataFieldSettingsPivotTableViewModel ViewModel { get { return viewModel; } }
		#endregion
		void SetActiveTabPage(DataFieldSettingsInitialTabPage initialTabPage) {
			this.tabControl.SelectedTabPageIndex = (int)initialTabPage;
		}
		void SetBindings() {
			this.lblSourceNameValue.DataBindings.Add("Text", ViewModel, "SourceName", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtCustomName.DataBindings.Add("EditValue", ViewModel, "CustomName", false, DataSourceUpdateMode.OnValidation);
			this.lbFunctions.DataSource = ViewModel.SubtotalFunctions;
			this.lbFunctions.DataBindings.Add("SelectedValue", ViewModel, "Subtotal", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtShowValuesAs.Properties.DataSource = ViewModel.ShowDataAsList;
			this.edtShowValuesAs.DataBindings.Add("EditValue", ViewModel, "ShowDataAs", false, DataSourceUpdateMode.OnPropertyChanged);
			this.lblBaseField.DataBindings.Add("Enabled", ViewModel, "BaseFieldEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.lbBaseField.DataBindings.Add("Enabled", ViewModel, "BaseFieldEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.lblBaseItem.DataBindings.Add("Enabled", ViewModel, "BaseItemEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.lbBaseItem.DataBindings.Add("Enabled", ViewModel, "BaseItemEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		void InitializeBaseFieldListBox() {
			this.lbBaseField.Items.AddRange(viewModel.BaseFieldTable.Values.ToArray());
			if (!string.IsNullOrEmpty(ViewModel.BaseFieldName))
				this.lbBaseField.SetSelected(this.lbBaseField.FindString(ViewModel.BaseFieldName), true);
		}
		void InitializeBaseItemListBox() {
			PivotShowDataAs showDataAs = ViewModel.GetShowDataAsByString(ViewModel.ShowDataAs);
			if (showDataAs == PivotShowDataAs.Percent || showDataAs == PivotShowDataAs.Difference || showDataAs == PivotShowDataAs.PercentDifference)
				this.lbBaseItem.SetSelected(this.lbBaseItem.FindString(ViewModel.BaseItemName), true);
		}
		void lbBaseField_SelectedValueChanged(object sender, System.EventArgs e) {
			if (this.lbBaseField.SelectedValue != null)
				ViewModel.BaseFieldName = this.lbBaseField.SelectedValue as string;
			this.lbBaseItem.Items.Clear();
			if (this.lbBaseItem.Enabled) {
				ViewModel.BaseItemTable = ViewModel.PopulateBaseItemTable(ViewModel.BaseField, !isInitializeLists);
				this.lbBaseItem.Items.AddRange(ViewModel.BaseItemTable.Values.ToArray());
				int index = 0;
				if (!isInitializeLists && this.lbBaseItem.SelectedValue == null) 
					this.lbBaseItem.SelectedValue = ViewModel.GetBaseItemNameStringByInt(index);
			}
		}
		void lbBaseItem_SelectedValueChanged(object sender, System.EventArgs e) {
			if (this.lbBaseItem.SelectedValue != null)
				ViewModel.BaseItemName = this.lbBaseItem.SelectedValue as string;
		}
		void lbBaseItem_EnabledChanged(object sender, System.EventArgs e) {
			this.lbBaseItem.Items.Clear();
			if (this.lbBaseItem.Enabled) {
				ViewModel.BaseItemTable = ViewModel.PopulateBaseItemTable(ViewModel.BaseField, true);
				this.lbBaseItem.Items.AddRange(ViewModel.BaseItemTable.Values.ToArray());
				if (this.lbBaseItem.SelectedValue == null)
					this.lbBaseItem.SelectedValue = ViewModel.GetBaseItemNameStringByInt(ViewModel.BaseItem);
			}
		}
		void btnOk_Click(object sender, System.EventArgs e) {
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
