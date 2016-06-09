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
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraPrinting {
	public interface INotificationService {
		void ShowException(UserLookAndFeel lookAndFeel, IWin32Window owner, Exception ex);
		DialogResult ShowMessage(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);
	}
	public class NotificationService : INotificationService {
		protected class MessageBoxLocalException : Exception {
			public MessageBoxLocalException(string message, Exception innerException) 
				: base(message, innerException) {
			}
		}
		static Dictionary<Type, INotificationService> services = new Dictionary<Type,INotificationService>();
		static INotificationService GetService<T>() {
			INotificationService service;
			if(!services.TryGetValue(typeof(T), out service)) {
				service = new NotificationService();
				services.Add(typeof(T), service);
			}
			return service;
		}
		public static void AddService(Type type, INotificationService service) {
			services[type] = service;
		}
		public static void ShowException<T>(UserLookAndFeel lookAndFeel, IWin32Window owner, Exception ex) {
			GetService<T>().ShowException(lookAndFeel, owner, ex);
		}
		public static DialogResult ShowMessage<T>(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
			return GetService<T>().ShowMessage(lookAndFeel, owner, text, caption, buttons, icon);
		}
		public virtual void ShowException(UserLookAndFeel lookAndFeel, IWin32Window owner, Exception ex) {
#if DEBUGTEST
			if(TestEnvironment.IsTestRunning())
				throw new InvalidOperationException(new InvalidOperationException().Message, ex);
#endif
			try {
				ShowMessageCore(lookAndFeel, owner, GetMessage(ex), GetCaption(owner as Form, ex), MessageBoxButtons.OK, MessageBoxIcon.Error);
			} catch(MessageBoxLocalException e) {
				throw new AggregateException(ex, e.InnerException);
			}
		}
		static string GetMessage(Exception e) {
			if(e == null)
				return string.Empty;
			if(e is System.Reflection.TargetInvocationException && e.InnerException != null)
				e = e.InnerException;
			return e.Message;
		}
		protected virtual string GetCaption(Form form, Exception ex) {
			return form != null ? form.Text : string.Empty;
		}
		public virtual DialogResult ShowMessage(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
			try {
				return ShowMessageCore(lookAndFeel, owner, text, caption, buttons, icon);
			} catch(MessageBoxLocalException e) {
				throw new Exception(e.InnerException.Message, e.InnerException);
			}
		}
		protected DialogResult ShowMessageCore(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
			if(PSNativeMethods.AspIsRunning)
				throw new Exception(text);
#if DEBUGTEST
			if(TestEnvironment.IsTestRunning())
				throw new InvalidOperationException();
#endif
			if(string.IsNullOrEmpty(caption))
				caption = DefaultCaption;
			try {
				return XtraMessageBox.Show(lookAndFeel, owner, text, caption, buttons, icon);
			} catch(Exception e) {
				throw new MessageBoxLocalException(e.Message, e);
			}
		}
		protected virtual string DefaultCaption {
			get {
				return PreviewLocalizer.GetString(PreviewStringId.Msg_Caption);
			}
		}
	}
}
