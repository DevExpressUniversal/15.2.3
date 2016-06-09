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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class CreateDefinedNamesFromSelectionForm : XtraForm {
		CreateDefinedNamesFromSelectionViewModel viewModel;
		public CreateDefinedNamesFromSelectionForm() {
			InitializeComponent();
		}
		public CreateDefinedNamesFromSelectionForm(CreateDefinedNamesFromSelectionViewModel viewModel) {
			InitializeComponent();
			this.viewModel = viewModel;
			SetBindings();
		}
		public CreateDefinedNamesFromSelectionViewModel ViewModel { get { return viewModel; } }
		protected internal virtual void SetBindings() {
			if (ViewModel == null)
				return;
			chkTopRow.DataBindings.Add("Checked", ViewModel, "UseTopRow", true, DataSourceUpdateMode.OnPropertyChanged);
			chkLeftColumn.DataBindings.Add("Checked", ViewModel, "UseLeftColumn", true, DataSourceUpdateMode.OnPropertyChanged);
			chkBottomRow.DataBindings.Add("Checked", ViewModel, "UseBottomRow", true, DataSourceUpdateMode.OnPropertyChanged);
			chkRightColumn.DataBindings.Add("Checked", ViewModel, "UseRightColumn", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void btnOk_Click(object sender, EventArgs e) {
			if (ViewModel != null) {
				ViewModel.ApplyChanges();
				DialogResult = DialogResult.OK;
			}
		}
	}
}
