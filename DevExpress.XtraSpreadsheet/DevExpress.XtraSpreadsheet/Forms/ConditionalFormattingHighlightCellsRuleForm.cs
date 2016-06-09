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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Native;
using DevExpress.Utils.Menu;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class ConditionalFormattingHighlightCellsRuleForm : ReferenceEditForm {
		readonly ConditionalFormattingViewModelBase viewModel;
		readonly IConditionalFormattingHighlightCellsRuleViewModel highlightCellsViewModel;
		public ConditionalFormattingHighlightCellsRuleForm(ConditionalFormattingHighlightCellsRuleViewModel viewModel)
			: this(viewModel, viewModel) {
		}
		public ConditionalFormattingHighlightCellsRuleForm(ConditionalFormattingTextRuleViewModel viewModel)
			: this(viewModel, viewModel) {
		}
		ConditionalFormattingHighlightCellsRuleForm(ConditionalFormattingViewModelBase viewModel, IConditionalFormattingHighlightCellsRuleViewModel highlightCellsViewModel)
			: base(highlightCellsViewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			Guard.ArgumentNotNull(highlightCellsViewModel, "highlightCellsViewModel");
			this.viewModel = viewModel;
			this.highlightCellsViewModel = highlightCellsViewModel;
			InitializeComponent();
			this.edtRange.SpreadsheetControl = highlightCellsViewModel.Control;
			this.edtRange.Controller.EditValuePrefix = "=";
			this.edtRange.Controller.PositionType = PositionType.Absolute;
			SubscribeReferenceEditControlsEvents();
			SetBindingsForm();
			SetConditionalFormattingBindings();
		}
		public ConditionalFormattingHighlightCellsRuleForm() {
			InitializeComponent();
		}
		void SetConditionalFormattingBindings() {
			cbFormat.Properties.Items.AddRange(highlightCellsViewModel.Styles);
			cbFormat.DataBindings.Add("SelectedItem", highlightCellsViewModel, "Style", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetBindingsForm() {
			this.DataBindings.Add("Text", highlightCellsViewModel, "FormText", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblHeader.DataBindings.Add("Text", highlightCellsViewModel, "LabelHeaderText", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtRange.DataBindings.Add("EditValue", highlightCellsViewModel, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void btnOk_Click(object sender, EventArgs e) {
			this.TopMost = false;
			try {
				if (viewModel.ApplyChanges()) {
					this.DialogResult = DialogResult.OK;
					Close();
				}
			}
			finally {
				this.TopMost = true;
			}
		}
		void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
