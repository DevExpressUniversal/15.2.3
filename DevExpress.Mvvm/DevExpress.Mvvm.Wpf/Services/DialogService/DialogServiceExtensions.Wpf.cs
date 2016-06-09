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
namespace DevExpress.Mvvm.Xpf {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public static class DialogServicePlatformExtension {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#if !SILVERLIGHT
		public static MessageBoxResult ShowDialog(this IDialogService service, MessageBoxButton dialogButtons, string title, object viewModel) {
#else
		public static Task<MessageBoxResult> ShowDialog(this IDialogService service, MessageBoxButton dialogButtons, string title, object viewModel) {
#endif
			DialogServiceExtensions.VerifyService(service);
			var res = service.ShowDialog(UICommand.GenerateFromMessageBoxButton(dialogButtons, DialogServiceExtensions.GetLocalizer(service)), title, null, viewModel, null, null);
			return GetMessageBoxResult(res);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#if !SILVERLIGHT
		public static MessageBoxResult ShowDialog(this IDialogService service, MessageBoxButton dialogButtons, string title, string documentType, object parameter, object parentViewModel) {
#else
		public static Task<MessageBoxResult> ShowDialog(this IDialogService service, MessageBoxButton dialogButtons, string title, string documentType, object parameter, object parentViewModel) {
#endif
			DialogServiceExtensions.VerifyService(service);
			var res = service.ShowDialog(UICommand.GenerateFromMessageBoxButton(dialogButtons, DialogServiceExtensions.GetLocalizer(service)), title, documentType, null, parameter, parentViewModel);
			return GetMessageBoxResult(res);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#if !SILVERLIGHT
		public static MessageBoxResult ShowDialog(this IDialogService service, MessageBoxButton dialogButtons, string title, string documentType, object viewModel) {
#else
		public static Task<MessageBoxResult> ShowDialog(this IDialogService service, MessageBoxButton dialogButtons, string title, string documentType, object viewModel) {
#endif
			DialogServiceExtensions.VerifyService(service);
			var res = service.ShowDialog(UICommand.GenerateFromMessageBoxButton(dialogButtons, DialogServiceExtensions.GetLocalizer(service)), title, documentType, viewModel, null, null);
			return GetMessageBoxResult(res);
		}
#if SILVERLIGHT
		static Task<MessageBoxResult> GetMessageBoxResult(Task<UICommand> result) {
			if(result == null)
				return null;
			return result.ContinueWith<MessageBoxResult>(x => GetMessageBoxResult(x.Result));
		}
#endif
		static MessageBoxResult GetMessageBoxResult(UICommand result) {
			if(result == null)
				return MessageBoxResult.None;
			return (MessageBoxResult)result.Tag;
		}
	}
}
