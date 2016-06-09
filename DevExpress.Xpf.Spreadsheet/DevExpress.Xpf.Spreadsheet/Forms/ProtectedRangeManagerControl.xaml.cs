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
	public partial class ProtectedRangeManagerControl : UserControl {
		ProtectedRangeManagerViewModel viewModel;
		public ProtectedRangeManagerControl(ProtectedRangeManagerViewModel viewModel) {
			InitializeComponent();
			this.viewModel = viewModel;
			DataContext = viewModel;
		}
		void btnNew_Click(object sender, RoutedEventArgs e) {
			if (viewModel != null)
				ShowNewProtectedRangeForm();
		}
		void btnModify_Click(object sender, RoutedEventArgs e) {
			if (viewModel != null)
				ShowEditProtectedRangeForm();
		}
		void btnDelete_Click(object sender, RoutedEventArgs e) {
			if (viewModel != null)
				viewModel.DeleteCurrentProtectedRange();
		}
		void btnPermissions_Click(object sender, RoutedEventArgs e) {
			viewModel.EditCurrentRangePermissions();
		}
		void ShowNewProtectedRangeForm() {
			ProtectedRangeViewModel rangeViewModel = viewModel.CreateNewRangeViewModel();
			ShowProtectedRangeForm(rangeViewModel);
		}
		void ShowEditProtectedRangeForm() {
			ProtectedRangeViewModel rangeViewModel = viewModel.CreateEditCurrentRangeViewModel();
			ShowProtectedRangeForm(rangeViewModel);
		}
		void ShowProtectedRangeForm(ProtectedRangeViewModel rangeViewModel) {
			if (viewModel == null)
				return;
			SpreadsheetControl control = viewModel.Control as SpreadsheetControl;
			if (control == null)
				return;
			control.ShowProtectedRangeForm(rangeViewModel, OnChildFormClosed);
		}
		void OnChildFormClosed(object sender, EventArgs e) {
			DXDialog form = sender as DXDialog;
			if (form != null)
				form.Closed -= OnChildFormClosed;
			if (form.DialogResult != true)
				return;
			FrameworkElement content = form.Content as FrameworkElement;
			if (content == null)
				return;
			ProtectedRangeViewModel rangeViewModel = content.DataContext as ProtectedRangeViewModel;
			if (rangeViewModel == null)
				return;
			viewModel.ApplyRangeChange(rangeViewModel);
		}
		void grid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (viewModel == null)
				return;
			DependencyObject obj = (DependencyObject)e.OriginalSource;
			while (obj != null && obj != grid) {
				if (obj.GetType() == typeof(ListViewItem)) {
					ShowEditProtectedRangeForm();
					break;
				}
				obj = VisualTreeHelper.GetParent(obj);
			}
		}
		void grid_KeyDown(object sender, KeyEventArgs e) {
			if (viewModel == null || !viewModel.IsCurrentRangeValid)
				return;
			if (e.Key == Key.Enter)
				ShowEditProtectedRangeForm();
			else if (e.Key == Key.Delete)
				viewModel.DeleteCurrentProtectedRange();
		}
	}
}
