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
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.Xpf.Spreadsheet.Internal;
using System.Windows;
using DevExpress.Xpf.Spreadsheet.Localization;
using System.Windows.Data;
using System.Windows.Threading;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class FindReplaceControl : UserControl {
		readonly FindReplaceViewModel viewModel;
		public FindReplaceControl(FindReplaceViewModel viewModel) {
			InitializeComponent();
			DataContext = viewModel;
			this.viewModel = viewModel;
			this.Loaded += OnLoaded;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			int selectedIndex = viewModel.ReplaceMode ? 1 : 0;
			this.tabControl.SelectedIndex = selectedIndex;
			UpdateContent(selectedIndex);
		}
		void DXTabControl_SelectionChanging(object sender, TabControlSelectionChangingEventArgs e) {
			UpdateContent(e.NewSelectedIndex);
		}
		void UpdateContent(int selectedIndex) {
			bool isReplaceMode = selectedIndex == 1;
			if (viewModel != null)
				viewModel.ReplaceMode = isReplaceMode;
			if (isReplaceMode) {
				tabFind.Content = null;
				tabReplace.Content = formContent;
			}
			else {
				tabReplace.Content = null;
				tabFind.Content = formContent;
			}
		}
		void DXTabControl_SelectionChanged(object sender, TabControlSelectionChangedEventArgs e) {
			Dispatcher.BeginInvoke(new Action(() => edtFindWhat.Focus()), DispatcherPriority.Render);
		}
	}
	public class FindReplaceDXDialog : CustomDXDialog {
		readonly FindReplaceViewModel viewModel;
		public FindReplaceDXDialog(string title, FindReplaceViewModel viewModel)
			: base(title, DialogButtons.YesNoCancel) {
			this.viewModel = viewModel;
		}
		Button FindButton { get { return OkButton; } }
		Button ReplaceButton { get { return NoButton; } }
		Button ReplaceAllButton { get { return YesButton; } }
		#region Events
		#region FindButtonClick
		EventHandler onFindButtonClick;
		public event EventHandler FindButtonClick { add { onFindButtonClick += value; } remove { onFindButtonClick -= value; } }
		void RaiseFindButtonClick() {
			if (onFindButtonClick != null)
				onFindButtonClick(this, EventArgs.Empty);
		}
		#endregion
		#region ReplaceButtonClick
		EventHandler onReplaceButtonClick;
		public event EventHandler ReplaceButtonClick { add { onReplaceButtonClick += value; } remove { onReplaceButtonClick -= value; } }
		void RaiseReplaceButtonClick() {
			if (onReplaceButtonClick != null)
				onReplaceButtonClick(this, EventArgs.Empty);
		}
		#endregion
		#region ReplaceAllButtonClick
		EventHandler onReplaceAllButtonClick;
		public event EventHandler ReplaceAllButtonClick { add { onReplaceAllButtonClick += value; } remove { onReplaceAllButtonClick -= value; } }
		void RaiseReplaceAllButtonClick() {
			if (onReplaceAllButtonClick != null)
				onReplaceAllButtonClick(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected override void OnButtonClick(bool? result, MessageBoxResult messageBoxResult) {
			if (messageBoxResult == MessageBoxResult.Yes && ReplaceAllButton != null) {
				ReplaceAllButton.Focus();
				RaiseReplaceAllButtonClick();
				return;
			}
			if (messageBoxResult == MessageBoxResult.No && ReplaceButton != null) {
				ReplaceButton.Focus();
				RaiseReplaceButtonClick();
				return;
			}
			if (messageBoxResult == MessageBoxResult.OK && FindButton != null) {
				FindButton.Focus();
				RaiseFindButtonClick();
				return;
			}
			base.OnButtonClick(result, messageBoxResult);
		}
		protected override void ApplyDialogButtonProperty() {
			base.ApplyDialogButtonProperty();
			SetButtonVisibilities(true, true, true, true, false);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetupReplaceButton(this.ReplaceButton, SpreadsheetControlStringId.Caption_FindReplaceFormReplaceButton);
			SetupReplaceButton(this.ReplaceAllButton, SpreadsheetControlStringId.Caption_FindReplaceFormReplaceAllButton);
			SetupFindButton(this.FindButton, SpreadsheetControlStringId.Caption_FindReplaceFormFindButton);
		}
		void SetupReplaceButton(Button button, SpreadsheetControlStringId stringId) {
			if (button == null)
				return;
			SetupFindButton(button, stringId);
			if (viewModel != null) {
				Binding binding = new Binding();
				binding.Source = viewModel;
				binding.Path = new PropertyPath("ReplaceMode");
				binding.Converter = new BooleanToVisibilityConverter();
				button.SetBinding(UIElement.VisibilityProperty, binding);
			}
		}
		void SetupFindButton(Button button, SpreadsheetControlStringId stringId) {
			if (button == null)
				return;
			button.Content = XpfSpreadsheetLocalizer.GetString(stringId);
			button.IsDefault = false;
			button.IsCancel = false;
		}
	}
}
