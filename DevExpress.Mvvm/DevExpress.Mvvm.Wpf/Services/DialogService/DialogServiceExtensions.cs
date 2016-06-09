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

#if SILVERLIGHT
#if !FREE
using MessageBoxButton = DevExpress.Xpf.Core.DXMessageBoxButton;
#else
using MessageBoxButton = DevExpress.Mvvm.DXMessageBoxButton;
#endif
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm.Native;
namespace DevExpress.Mvvm {
	public static class DialogServiceExtensions {
#if !SILVERLIGHT
		public static UICommand ShowDialog(this IDialogService service, IEnumerable<UICommand> dialogCommands, string title, object viewModel) {
#else
		public static Task<UICommand> ShowDialog(this IDialogService service, IEnumerable<UICommand> dialogCommands, string title, object viewModel) {
#endif
			VerifyService(service);
			return service.ShowDialog(dialogCommands, title, null, viewModel, null, null);
		}
#if !SILVERLIGHT
		public static MessageResult ShowDialog(this IDialogService service, MessageButton dialogButtons, string title, object viewModel) {
#else
		public static Task<MessageResult> ShowDialog(this IDialogService service, MessageButton dialogButtons, string title, object viewModel) {
#endif
			VerifyService(service);
			var res = service.ShowDialog(UICommand.GenerateFromMessageButton(dialogButtons, GetLocalizer(service)), title, null, viewModel, null, null);
			return GetMessageResult(res);
		}
#if !SILVERLIGHT
		public static UICommand ShowDialog(this IDialogService service, IEnumerable<UICommand> dialogCommands, string title, string documentType, object viewModel) {
#else
		public static Task<UICommand> ShowDialog(this IDialogService service, IEnumerable<UICommand> dialogCommands, string title, string documentType, object viewModel) {
#endif
			VerifyService(service);
			return service.ShowDialog(dialogCommands, title, documentType, viewModel, null, null);
		}
#if !SILVERLIGHT
		public static MessageResult ShowDialog(this IDialogService service, MessageButton dialogButtons, string title, string documentType, object viewModel) {
#else
		public static Task<MessageResult> ShowDialog(this IDialogService service, MessageButton dialogButtons, string title, string documentType, object viewModel) {
#endif
			VerifyService(service);
			var res = service.ShowDialog(UICommand.GenerateFromMessageButton(dialogButtons, GetLocalizer(service)), title, documentType, viewModel, null, null);
			return GetMessageResult(res);
		}
#if !SILVERLIGHT
		public static UICommand ShowDialog(this IDialogService service, IEnumerable<UICommand> dialogCommands, string title, string documentType, object parameter, object parentViewModel) {
#else
		public static Task<UICommand> ShowDialog(this IDialogService service, IEnumerable<UICommand> dialogCommands, string title, string documentType, object parameter, object parentViewModel) {
#endif
			VerifyService(service);
			return service.ShowDialog(dialogCommands, title, documentType, null, parameter, parentViewModel);
		}
#if !SILVERLIGHT
		public static MessageResult ShowDialog(this IDialogService service, MessageButton dialogButtons, string title, string documentType, object parameter, object parentViewModel) {
#else
		public static Task<MessageResult> ShowDialog(this IDialogService service, MessageButton dialogButtons, string title, string documentType, object parameter, object parentViewModel) {
#endif
			VerifyService(service);
			var res = service.ShowDialog(UICommand.GenerateFromMessageButton(dialogButtons, GetLocalizer(service)), title, documentType, null, parameter, parentViewModel);
			return GetMessageResult(res);
		}
		internal static void VerifyService(IDialogService service) {
			if (service == null)
				throw new ArgumentNullException("service");
		}
		internal static IMessageButtonLocalizer GetLocalizer(IDialogService service) {
			return service as IMessageButtonLocalizer ?? (service as IMessageBoxButtonLocalizer).With(x => x.ToMessageButtonLocalizer()) ?? new DefaultMessageButtonLocalizer();
		}
#if SILVERLIGHT
		static Task<MessageResult> GetMessageResult(Task<UICommand> result) {
			if(result == null)
				return null;
			return result.ContinueWith<MessageResult>(x => GetMessageResult(x.Result));
		}
#endif
		static MessageResult GetMessageResult(UICommand result) {
			if(result == null)
				return MessageResult.None;
			return (MessageResult)result.Tag;
		}
	}
}
