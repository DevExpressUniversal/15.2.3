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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.RichEdit.Localization;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class DocumentProtectionQueryNewPasswordControl : UserControl, IDialogContent {
		readonly DocumentProtectionQueryNewPasswordFormController controller;
		public DocumentProtectionQueryNewPasswordControl() {
			InitializeComponent();
		}
		public DocumentProtectionQueryNewPasswordControl(DocumentProtectionQueryNewPasswordFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controller = CreateController(controllerParameters);
			this.Loaded += OnLoaded;
			InitializeComponent();
		}
		public DocumentProtectionQueryNewPasswordFormController Controller { get { return controller; } }
		protected virtual DocumentProtectionQueryNewPasswordFormController CreateController(DocumentProtectionQueryNewPasswordFormControllerParameters controllerParameters) {
			return new DocumentProtectionQueryNewPasswordFormController(controllerParameters);
		}
		protected internal virtual void OnLoaded(object sender, RoutedEventArgs e) {
			edtPassword.Focus();
		}
		protected internal virtual void ApplyChanges() {
			Controller.Password = edtPassword.Text;
			Controller.ApplyChanges();
		}
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			if (edtPassword.Text != edtRepeatPassword.Text) {
				string errorMessage = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_DocumentProtectionInvalidPasswordConfirmation);
#if SL
				DXDialog dialog = new DXDialog();
				dialog.Title = XpfRichEditLocalizer.GetString(RichEditControlStringId.Msg_Warning);
				dialog.Buttons = DialogButtons.Ok;
				dialog.Content = errorMessage;
				dialog.IsSizable = false;
				dialog.Padding = new Thickness(20);
				dialog.ShowDialog();
#else
				DXMessageBox.Show(errorMessage, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Warning);
#endif
				return false;
			}
			else
				return true;
		}
		void IDialogContent.OnApply() {
			ApplyChanges();
		}
		void IDialogContent.OnOk() {
			ApplyChanges();
		}
		#endregion
	}
}
