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

using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
namespace DevExpress.Mvvm {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public static class MessageBoxServicePlatformExtensions {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static MessageBoxResult Show(this IMessageBoxService service, string messageBoxText) {
			return service.Show(messageBoxText, string.Empty);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static MessageBoxResult Show(this IMessageBoxService service, string messageBoxText, string caption) {
			return service.ShowMessage(messageBoxText, caption, MessageButton.OK).ToMessageBoxResult();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static MessageBoxResult Show(this IMessageBoxService service, string messageBoxText, string caption, MessageBoxButton button) {
#if !SILVERLIGHT
			return service.Show(messageBoxText, caption, button, MessageBoxImage.None);
#else
			return service.Show(messageBoxText, caption, button, MessageBoxResult.None);
#endif
		}
#if !SILVERLIGHT
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static MessageBoxResult Show(this IMessageBoxService service, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon) {
			return service.Show(messageBoxText, caption, button, icon, MessageBoxResult.None);
		}
#endif
#if SILVERLIGHT
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static MessageBoxResult Show(this IMessageBoxService service, string messageBoxText, string caption, MessageBoxButton button, MessageBoxResult defaultResult) {
			VerifyService(service);
			return service.Show(messageBoxText, caption, button.ToMessageButton(), defaultResult.ToMessageResult()).ToMessageBoxResult();
		}
#else
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static MessageBoxResult Show(this IMessageBoxService service, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult) {
			VerifyService(service);
			return service.Show(messageBoxText, caption, button.ToMessageButton(), icon.ToMessageIcon(), defaultResult.ToMessageResult()).ToMessageBoxResult();
		}
#endif
		static void VerifyService(IMessageBoxService service) {
			if(service == null)
				throw new ArgumentNullException("service");
		}
	}
}
