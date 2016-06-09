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
using DevExpress.Utils;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Native;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class ChartSelectDataForm : ReferenceEditForm {
		#region Fields
		readonly ChartSelectDataViewModel viewModel;
		#endregion
		public ChartSelectDataForm() {
		}
		public ChartSelectDataForm(ChartSelectDataViewModel viewModel)
			: base(viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			editControl.SpreadsheetControl = viewModel.Control;
			editControl.Controller.IncludeSheetName = true;
			editControl.Controller.EditValuePrefix = "=";
			editControl.Controller.PositionType = PositionType.Absolute;
			SetBindingsForm();
			SubscribeReferenceEditControlsEvents();
		}
		#region Properties
		protected new ChartSelectDataViewModel ViewModel { get { return viewModel; } }
		#endregion
		void SetBindingsForm() {
			this.editControl.DataBindings.Add("EditValue", viewModel, "Reference", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void btnOk_Click(object sender, EventArgs e) {
			this.TopMost = false;
			try {
				if (ViewModel.ApplyChanges()) {
					this.DialogResult = DialogResult.OK;
					Close();
				}
			}
			finally {
				this.TopMost = true;
			}
		}
		void btnCancel_Click(object sender, EventArgs e) {
			Close();
		}
		void ChartSelectDataForm_FormClosing(object sender, FormClosingEventArgs e) {
			ViewModel.RestoreActiveSheet();
		}
	}
}
