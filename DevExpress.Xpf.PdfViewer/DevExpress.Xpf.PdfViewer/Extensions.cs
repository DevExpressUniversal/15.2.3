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

using System.Drawing;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf.Drawing;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using Rectange = System.Drawing.Rectangle;
using System.Collections.Generic;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm;
using System;
using DevExpress.Xpf.PdfViewer.Internal;
using DevExpress.Xpf.DocumentViewer.Extensions;
namespace DevExpress.Xpf.PdfViewer.Extensions {
	public static class WinFormsCompatibilityExtensions {
		public static Size Rotate(this Size size, double angle) {
			return (int)angle % 180 == 0 ? size : new Size(size.Height, size.Width);
		}
		public static PdfModifierKeys ToPdfModifierKeys(this ModifierKeys modifierKeys) {
			return (PdfModifierKeys)modifierKeys;
		}
	}
	public static class PdfDialogServiceExtensions {
		public static void ShowDialog(this IDialogService service, ICommand okCommand, string okCommandCaption, ICommand cancelCommand, string cancelCommandCaption, string title, Func<object, object> convertViewModel, Func<object> getDefaultContent) {
			var vm = (service as PdfDialogService).With(x => x.Content) ?? getDefaultContent();
			var printDialogViewModel = convertViewModel(vm) as PrintDialogViewModel;
			var okUICommand = new UICommand {
				Id = MessageBoxResult.OK,
				Caption = okCommandCaption,
				IsCancel = false,
				IsDefault = true,
				Command = new DelegateCommand(() => okCommand.Do(x => x.TryExecute(printDialogViewModel.PdfViewModel)), () => okCommand.Return(x => x.CanExecute(printDialogViewModel.PdfViewModel), () => true))
			};
			var cancelUICommand = new UICommand {
				Id = MessageBoxResult.Cancel,
				Caption = cancelCommandCaption,
				IsCancel = true,
				IsDefault = false,
				Command = new DelegateCommand(() => cancelCommand.Do(x => x.TryExecute(printDialogViewModel.PdfViewModel)), () => cancelCommand.Return(x => x.CanExecute(printDialogViewModel.PdfViewModel), () => true))
			};
			service.ShowDialog(new[] { okUICommand, cancelUICommand }, title, printDialogViewModel);
		}
	}
	public static class StackExtensions {
		public static T Remove<T>(this Stack<T> stack, T element) {
			T item = stack.Pop();
			if (item.Equals(element))
				return item;
			T result = stack.Remove(element);
			stack.Push(item);
			return result;
		}
	}
}
