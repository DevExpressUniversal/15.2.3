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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class PageSetupPrintButtonsControl : Control {
		#region Fields
		Button btnPrint;
		Button btnPrintPreview;
		#endregion
		public PageSetupPrintButtonsControl() {
			DefaultStyleKey = typeof(PageSetupPrintButtonsControl);
		}
		public override void OnApplyTemplate() {
			UnsubscribeEvents();
			base.OnApplyTemplate();
			btnPrint = LayoutHelper.FindElementByName(this, "btnPrint") as Button;
			btnPrintPreview = LayoutHelper.FindElementByName(this, "btnPrintPreview") as Button;
			SubscribeEvents();
		}
		void UnsubscribeEvents() {
			if (btnPrint != null)
				btnPrint.Click -= btnPrint_Click;
			if (btnPrintPreview != null)
				btnPrintPreview.Click -= btnPrintPreview_Click;
		}
		void SubscribeEvents() {
			if (btnPrint != null)
				btnPrint.Click += btnPrint_Click;
			if (btnPrintPreview != null)
				btnPrintPreview.Click += btnPrintPreview_Click;
		}
		void btnPrint_Click(object sender, RoutedEventArgs e) {
			PageSetupViewModel viewModel = DataContext as PageSetupViewModel;
			viewModel.ApplyChanges();
			CloseWindow();
			PrintCommand command = new PrintCommand(viewModel.Control);
			command.Execute();
		}
		void btnPrintPreview_Click(object sender, RoutedEventArgs e) {
			PageSetupViewModel viewModel = DataContext as PageSetupViewModel;
			viewModel.ApplyChanges();
			CloseWindow();
			PrintPreviewCommand command = new PrintPreviewCommand(viewModel.Control);
			command.Execute();
		}
		void CloseWindow() {
			Window parentwin = Window.GetWindow(this);
			parentwin.Close();
		}
	}
}
