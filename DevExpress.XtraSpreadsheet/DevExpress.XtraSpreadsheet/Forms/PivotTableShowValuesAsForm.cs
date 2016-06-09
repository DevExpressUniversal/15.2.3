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
using System.Drawing;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class PivotTableShowValuesAsForm : XtraForm {
		readonly PivotTableShowValuesAsViewModel viewModel;
		PivotTableShowValuesAsForm() {
			InitializeComponent();
		}
		public PivotTableShowValuesAsForm(PivotTableShowValuesAsViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			CorrectHeight();
			SetBindingsForm();
			viewModel.PropertyChanged += viewModel_PropertyChanged;
		}
		void CorrectHeight() {
			if (!viewModel.BaseItemEnabled)
				this.Height = this.Height - 30;
		}
		void SetBindingsForm() {
			this.Text = this.Text + " (" + viewModel.DataFieldName + ")";
			this.lblCalculation.Text = viewModel.CalculationType;
			this.edtBaseField.Properties.DataSource = viewModel.BaseFieldNames;
			this.edtBaseField.DataBindings.Add("EditValue", viewModel, "CurrentBaseFieldName", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtBaseItem.Properties.DataSource = viewModel.BaseItemNames;
			this.edtBaseItem.DataBindings.Add("EditValue", viewModel, "CurrentBaseItemName", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtBaseItem.DataBindings.Add("Visible", viewModel, "BaseItemEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblBaseItem.DataBindings.Add("Visible", viewModel, "BaseItemEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if (e.PropertyName == "CurrentBaseItemName") {
				this.edtBaseItem.Properties.DataSource = null;
				this.edtBaseItem.Properties.DataSource = viewModel.BaseItemNames;
			}
		}
		void btnOk_Click(object sender, EventArgs e) {
			viewModel.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
	}
}
