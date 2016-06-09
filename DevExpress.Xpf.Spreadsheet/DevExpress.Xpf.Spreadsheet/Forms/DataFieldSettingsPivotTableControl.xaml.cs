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
using DevExpress.XtraSpreadsheet.Model;
using System.Windows.Controls;
using System.Linq;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class DataFieldSettingsPivotTableControl : UserControl {
		DataFieldSettingsPivotTableViewModel viewModel;
		bool isInitializeLists;
		public DataFieldSettingsPivotTableControl(DataFieldSettingsPivotTableViewModel viewModel) {
			InitializeComponent();
			this.viewModel = viewModel;
			DataContext = viewModel;
			isInitializeLists = true;
			InitializeBaseFieldListBox();
			InitializeBaseItemListBox();
			isInitializeLists = false;
		}
		void InitializeBaseFieldListBox() {
			this.lbBaseField.Items.AddRange(viewModel.BaseFieldTable.Values.ToArray());
			if (!string.IsNullOrEmpty(viewModel.BaseFieldName))
				this.lbBaseField.SelectedItems.Add(viewModel.BaseFieldName);
		}
		void InitializeBaseItemListBox() {
			PivotShowDataAs showDataAs = viewModel.GetShowDataAsByString(viewModel.ShowDataAs);
			if (showDataAs == PivotShowDataAs.Percent || showDataAs == PivotShowDataAs.Difference || showDataAs == PivotShowDataAs.PercentDifference)
				this.lbBaseItem.SelectedItems.Add(viewModel.BaseItemName);
		}
		void lbBaseField_EditValueChanged(object sender, Editors.EditValueChangedEventArgs e) {
			if (this.lbBaseField.EditValue != null)
				viewModel.BaseFieldName = this.lbBaseField.EditValue as string;
			this.lbBaseItem.Items.Clear();
			if (this.lbBaseItem.IsEnabled) {
				viewModel.BaseItemTable = viewModel.PopulateBaseItemTable(viewModel.BaseField, !isInitializeLists); 
				this.lbBaseItem.Items.AddRange(viewModel.BaseItemTable.Values.ToArray());
				int index = 0;
				if (!isInitializeLists && this.lbBaseItem.SelectedItem == null)
					this.lbBaseItem.SelectedItem = viewModel.GetBaseItemNameStringByInt(index);
			}
		}
		void lbBaseItem_EditValueChanged(object sender, Editors.EditValueChangedEventArgs e) {
			if (this.lbBaseItem.SelectedItem != null)
				viewModel.BaseItemName = this.lbBaseItem.EditValue as string;
		}
		private void lbBaseItem_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			this.lbBaseItem.Items.Clear();
			if (this.lbBaseItem.IsEnabled) {
				viewModel.BaseItemTable = viewModel.PopulateBaseItemTable(viewModel.BaseField, true);
				this.lbBaseItem.Items.AddRange(viewModel.BaseItemTable.Values.ToArray());
				if (this.lbBaseItem.SelectedItem == null)
					this.lbBaseItem.SelectedItem = viewModel.GetBaseItemNameStringByInt(viewModel.BaseItem);
			}
		}
	}
}
