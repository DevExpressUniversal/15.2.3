#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Core {
	public class Messaging {
		public static event EventHandler<ConfirmationDialogClosedEventArgs> ConfirmationDialogClosed;
		private static Messaging defaultMessaging;
		static Messaging() {
		}
		private XafApplication application;
		public Messaging(XafApplication application) {
			this.application = application;
		}
		protected XafApplication Application {
			get { return application; }
		}
		protected static void OnConfirmationDialogClosed(DialogResult result) {
			if (ConfirmationDialogClosed != null) {
				ConfirmationDialogClosed(null, new ConfirmationDialogClosedEventArgs(result));
			}
		}
		protected virtual DialogResult ShowCore(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
			return XtraMessageBox.Show(Form.ActiveForm, message, caption, buttons, icon);
		}
		protected virtual void ShowSystemException(string caption, string exceptionMessage) {
			ExceptionDialogForm.ShowMessage(caption, exceptionMessage);
		}
		protected virtual void ShowSystemException(string caption, Exception exception) {
			ShowSystemException(caption, exception.Message);
		}
		public DialogResult Show(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {			
			DialogResult result = ShowCore(message, caption, buttons, icon);
			OnConfirmationDialogClosed(result);
			return result;
		}		
		public DialogResult Show(string caption, string message) {
			return Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
		}
		public void Show(Exception exception) {
			Show((application == null) ? CaptionHelper.GetLocalizedText("Texts", "Error") : application.Title, exception);
		}
		public virtual void Show(string caption, Exception exception) {
			if(exception is IUserFriendlyException) {
				Show(CaptionHelper.GetLocalizedText("Texts", "Error"), exception.Message);
			}
			else {
				ShowSystemException(caption, exception.Message);
			}
		}
		public DialogResult GetUserChoice(string message, string caption, MessageBoxButtons buttons) {
			return Show(message, caption, buttons, MessageBoxIcon.Warning);
		}
		public static Messaging GetMessaging(XafApplication application) {
			if(application != null && application.Model.Options is IModelOptionsWin) {
				ITypeInfo messagingType = XafTypesInfo.Instance.FindTypeInfo(((IModelOptionsWin)application.Model.Options).Messaging);
				if(messagingType != null) {
					defaultMessaging = (Messaging)TypeHelper.CreateInstance(messagingType.Type, application);
				}
			}
			if(defaultMessaging == null) {
				defaultMessaging = new Messaging(application);
			}
			return defaultMessaging;
		}
		public static Messaging DefaultMessaging {
			get {
				if(defaultMessaging == null) {
					defaultMessaging = new Messaging(null);
				}
				return defaultMessaging;
			}
			set { defaultMessaging = value; }
		}
	}
	public class ConfirmationDialogClosedEventArgs : EventArgs {
		private DialogResult result;
		public ConfirmationDialogClosedEventArgs(DialogResult dialogResult) {
			result = dialogResult;
		}
		public DialogResult DialogResult {
			get { return result; }
		}
	}
}
