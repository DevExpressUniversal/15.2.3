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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.Xpf.Spreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Forms;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class OutlineSettingsControl : UserControl {
		public OutlineSettingsControl(OutlineSettingsViewModel viewModel) {
			InitializeComponent();
			DataContext = viewModel;
		}
	}
	public class OutlineDXDialog : CustomDXDialog {
		#region Fields
		OutlineSettingsViewModel viewModel;
		#endregion
		public OutlineDXDialog(string title, OutlineSettingsViewModel viewModel)
			: base(title) {
			this.viewModel = viewModel;
		}
		#region Properties
		Button CreateButton { get { return YesButton; } }
		Button ApplyStylesButton { get { return NoButton; } }
		#endregion
		void SubscribeEvents() {
			this.CreateButton.Click += CreateButton_Click;
			this.ApplyStylesButton.Click += ApplyStylesButton_Click;
		}
		protected override void ApplyDialogButtonProperty() {
			base.ApplyDialogButtonProperty();
			SetButtonVisibilities(true, true, true, true, false);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetupButton(this.CreateButton, SpreadsheetControlStringId.Caption_OutlineSettingFormCreateButton);
			SetupButton(this.ApplyStylesButton, SpreadsheetControlStringId.Caption_OutlineSettingFormApplyStylesButton);
			SubscribeEvents();
		}
		void SetupButton(Button button, SpreadsheetControlStringId stringId) {
			if (button == null)
				return;
			button.Content = XpfSpreadsheetLocalizer.GetString(stringId);
		}
		private void CreateButton_Click(object sender, RoutedEventArgs e) {
			viewModel.CreateOutline();
		}
		private void ApplyStylesButton_Click(object sender, RoutedEventArgs e) {
			viewModel.ApplyChanges();
		}
	}
}
