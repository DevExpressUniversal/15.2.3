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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Forms.Design;
using System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class FunctionArgumentsForm : ReferenceEditForm {
		FunctionArgumentsViewModel viewModel;
		bool dataSourceBindingComplete;
		public FunctionArgumentsForm() {
			InitializeComponent();
		}
		public FunctionArgumentsForm(FunctionArgumentsViewModel viewModel)
			: base(viewModel) {
			InitializeComponent();
			this.ViewModel = viewModel;
		}
		public new FunctionArgumentsViewModel ViewModel {
			get { return viewModel; }
			set {
				if (value == viewModel)
					return;
				Guard.ArgumentNotNull(value, "viewModel");
				this.viewModel = value;
				SetBindings();
			}
		}
		protected internal virtual void SetBindings() {
			if (ViewModel == null)
				return;
			dataSourceBindingComplete = false;
			this.grpFunctionParameters.DataBindings.Clear();
			this.grpFunctionParameters.DataBindings.Add("Text", ViewModel, "FunctionName");
			this.lblFunctionDescription.DataBindings.Clear();
			this.lblFunctionDescription.DataBindings.Add("Text", ViewModel, "FunctionDescription");
			this.xtraScrollableControl.DataBindings.Clear();
			this.xtraScrollableControl.DataBindings.Add("DataSource", ViewModel, "Arguments", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnDataSourceBindingComplete;
			this.xtraScrollableControl.DataBindings.Add("CurrentItemIndex", ViewModel, "CurrentArgumentIndex", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblCurrentArgumentName.DataBindings.Clear();
			this.lblCurrentArgumentName.DataBindings.Add("Text", ViewModel, "CurrentArgumentName", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblCurrentArgumentDescription.DataBindings.Clear();
			this.lblCurrentArgumentDescription.DataBindings.Add("Text", ViewModel, "CurrentArgumentDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblResult.DataBindings.Clear();
			this.lblResult.DataBindings.Add("Text", ViewModel, "CallResult", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void OnCreateItemControl(object sender, CreateItemControlEventArgs e) {
			FunctionArgumentViewModel itemViewModel = e.Item as FunctionArgumentViewModel;
			if (itemViewModel != null) {
				if (!itemViewModel.IsVisible)
					return;
				FunctionArgumentControl control = new FunctionArgumentControl(itemViewModel);
				control.SpreadsheetControl = viewModel.SpreadsheetControl;
				control.DataBindings.Add("Visible", itemViewModel, "IsVisible", true, DataSourceUpdateMode.OnPropertyChanged);
				e.Control = control;
				if (dataSourceBindingComplete)
					SubscribeReferenceEditControlsEvents(control.EdtValue);
			}
		}
		void OnDataSourceBindingComplete(object sender, BindingCompleteEventArgs e) {
			if (dataSourceBindingComplete)
				return;
			UnsubscribeReferenceEditControlsEvents();
			SubscribeReferenceEditControlsEvents();
			dataSourceBindingComplete = Object.ReferenceEquals(xtraScrollableControl.DataSource, viewModel.Arguments);
		}
		private void btnOk_Click(object sender, EventArgs e) {
			ViewModel.PrepareChanges();
			DialogResult = DialogResult.OK;
			Close();
		}
		void FunctionArgumentsForm_FormClosed(object sender, FormClosedEventArgs e) {
			if (ViewModel != null)
				ViewModel.ApplyChanges();
		}
		private void btnCancel_Click(object sender, EventArgs e) {
			Close();
		}
	}
}
