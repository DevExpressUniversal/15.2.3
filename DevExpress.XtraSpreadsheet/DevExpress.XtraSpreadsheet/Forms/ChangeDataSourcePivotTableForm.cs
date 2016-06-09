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
using DevExpress.XtraSpreadsheet.Model;
using System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class ChangeDataSourcePivotTableForm : ReferenceEditForm {
		#region Fields
		readonly ChangeDataSourcePivotTableViewModel viewModel;
		#endregion
		ChangeDataSourcePivotTableForm() {
			InitializeComponent();
		}
		public ChangeDataSourcePivotTableForm(ChangeDataSourcePivotTableViewModel viewModel)
			: base(viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			editTableRange.SpreadsheetControl = viewModel.Control;
			editTableRange.PositionType = PositionType.Absolute;
			editTableRange.IncludeSheetName = true;
			SetBindings();
			SubscribeReferenceEditControlsEvents();
		}
		#region Properties
		protected new ChangeDataSourcePivotTableViewModel ViewModel { get { return viewModel; } }
		#endregion
		void SetBindings() {
			this.editTableRange.DataBindings.Add("EditValue", ViewModel, "Source", false, DataSourceUpdateMode.OnValidation);
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
		void btnCancel_Click(object sender, System.EventArgs e) {
			Close();
		}
	}
}
