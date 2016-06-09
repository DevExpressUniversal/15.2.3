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
using DevExpress.XtraEditors;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class OutlineSettingsForm: XtraForm {
		#region fields
		OutlineSettingsViewModel viewModel;
		#endregion
		public OutlineSettingsForm() {
			InitializeComponent();
		}
		public OutlineSettingsForm(OutlineSettingsViewModel viewModel) {
			this.viewModel = viewModel;
			InitializeComponent();
			this.ceRowsBelow.Checked = viewModel.ShowRowSumsBelow;
			this.ceColumnsRight.Checked = viewModel.ShowColumnSumsRight;
			this.ceAutomaticStyles.Checked = viewModel.ApplyStyles;
		}
		void ceRowsBelow_CheckedChanged(object sender, EventArgs e) {
			viewModel.ShowRowSumsBelow = this.ceRowsBelow.Checked;
		}
		void ceColumnsRight_CheckedChanged(object sender, EventArgs e) {
			viewModel.ShowColumnSumsRight = this.ceColumnsRight.Checked;
		}
		void ceAutomaticStyles_CheckedChanged(object sender, EventArgs e) {
			viewModel.ApplyStyles = this.ceAutomaticStyles.Checked;
		}
		void btn_Click(object sender, EventArgs e) {
			OkClose();
		}
		void btnCreate_Click(object sender, EventArgs e) {
			viewModel.CreateOutline();
			OkClose();
		}
		void OkClose() {
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
