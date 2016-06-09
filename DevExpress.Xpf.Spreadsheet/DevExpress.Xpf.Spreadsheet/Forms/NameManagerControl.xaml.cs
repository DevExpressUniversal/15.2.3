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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Spreadsheet.Localization;
using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class NameManagerControl : UserControl {
		NameManagerViewModel viewModel;
		public NameManagerControl(NameManagerViewModel viewModel) {
			InitializeComponent();
			this.viewModel = viewModel;
			DataContext = viewModel;
		}
		void btnNew_Click(object sender, RoutedEventArgs e) {
			if (viewModel != null)
				ShowNewDefinedNameForm();
		}
		void btnEdit_Click(object sender, RoutedEventArgs e) {
			if (viewModel != null)
				ShowEditDefinedNameForm();
		}
		void ShowNewDefinedNameForm() {
			DefineNameCommand command = new DefineNameCommand(viewModel.Control);
			ShowDefineNameForm(command.CreateViewModel());
		}
		void ShowEditDefinedNameForm() {
			DefineNameViewModel nameViewModel = viewModel.NamesDataSource[viewModel.CurrentNameIndex].Clone();
			if (!DefinedNameEditingCancelled(nameViewModel)) {
				nameViewModel.NewNameMode = false;
				nameViewModel.IsScopeChangeAllowed = false;
				ShowDefineNameForm(nameViewModel);
			}
		}
		bool DefinedNameEditingCancelled(DefineNameViewModel nameViewModel) {
			return viewModel.Control.InnerControl.RaiseDefinedNameEditing(
				nameViewModel.Name,
				nameViewModel.OriginalName,
				nameViewModel.Scope,
				nameViewModel.ScopeIndex,
				nameViewModel.Reference,
				nameViewModel.Comment);
		}
		void ShowDefineNameForm(DefineNameViewModel nameViewModel) {
			if (viewModel == null)
				return;
			SpreadsheetControl control = viewModel.Control as SpreadsheetControl;
			if (control == null)
				return;
			control.ShowDefineNameForm(nameViewModel, OnChildFormClosed);
		}
		void OnChildFormClosed(object sender, EventArgs e) {
			DXDialog form = sender as DXDialog;
			if (form != null)
				form.Closed -= OnChildFormClosed;
			viewModel.UpdateDefinedNames();
			this.grid.Focus();
		}
		void btnDelete_Click(object sender, System.Windows.RoutedEventArgs e) {
			if (viewModel != null) {
				viewModel.DeleteDefinedName();
				this.grid.Focus();
			}
		}
		void btnCancelReferenceChange_Click(object sender, RoutedEventArgs e) {
			if (viewModel != null)
				viewModel.CancelReferenceChange();
		}
		void btnApplyReferenceChange_Click(object sender, RoutedEventArgs e) {
			if (viewModel != null)
				viewModel.ApplyReferenceChange();
		}
		void grid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (viewModel == null)
				return;
			DependencyObject obj = (DependencyObject)e.OriginalSource;
			while (obj != null && obj != grid) {
				if (obj.GetType() == typeof(ListViewItem)) {
					ShowEditDefinedNameForm();
					break;
				}
				obj = VisualTreeHelper.GetParent(obj);
			}
		}
		void grid_KeyDown(object sender, KeyEventArgs e) {
			if (viewModel == null || !viewModel.IsCurrentNameValid)
				return;
			if (e.Key == Key.Enter)
				ShowEditDefinedNameForm();
			else if (e.Key == Key.Delete) {
				viewModel.DeleteDefinedName();
				this.grid.Focus();
			}
		}
	}
	public class NameManagerDXDialog : CustomDXDialog {
		public NameManagerDXDialog(string title)
			: base(title, DialogButtons.OkCancel) {
		}
		protected override void ApplyDialogButtonProperty() {
			base.ApplyDialogButtonProperty();
			SetButtonVisibilities(false, true, false, false, false);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetupCancelButton(this.CancelButton, SpreadsheetControlStringId.Caption_NameManagerFormCloseButton);
		}
		void SetupCancelButton(Button button, SpreadsheetControlStringId stringId) {
			if (button == null)
				return;
			button.Content = XpfSpreadsheetLocalizer.GetString(stringId);
		}
	}
}
