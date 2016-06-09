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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.DocumentViewer.Extensions;
using DevExpress.Xpf.Printing.PreviewControl.Native.Models;
using DevExpress.ReportServer.Printing;
namespace DevExpress.Xpf.Printing.PreviewControl.Native {
	public static class DialogServiceExtensions {
		public static void ShowDialog(this Core.DialogService service, ICommand okCommand, ICommand cancelCommand, string title, ViewModelBase viewModel, bool suppressExecutingBeforeClosing = true) {
			var okUICommand = new UICommand {
				Id = MessageBoxResult.OK,
				Caption = PrintingLocalizer.GetString(PrintingStringId.OK),
				IsCancel = false,
				IsDefault = true,
				Command = new DelegateCommand(()=> {
					if(!suppressExecutingBeforeClosing)
						okCommand.Execute(viewModel);
				}, () => okCommand.CanExecute(viewModel))
			};
			var cancelUICommand = new UICommand {
				Id = MessageBoxResult.Cancel,
				Caption = PrintingLocalizer.GetString(PrintingStringId.Cancel),
				IsCancel = true,
				IsDefault = false,
				Command = cancelCommand
			};
			var command = service.ShowDialog(new[] { okUICommand, cancelUICommand }, title, viewModel);
			if(!suppressExecutingBeforeClosing)
				return;
			if(command == okUICommand)
				okCommand.TryExecute(viewModel);
		}
	}
	public static class IDocumentViewModelExtensions {
		public static bool IsReportDocumentSource(this IDocumentViewModel document) {
			return document is ReportDocumentViewModel;
		}
		public static bool IsRemoteReportDocumentSorce(this IDocumentViewModel document) {
			return (document as ReportDocumentViewModel).Return(x => x.Report is RemoteDocumentSource, () => false);
		}
		public static bool IsXpfLinkDocumentSource(this IDocumentViewModel document) {
			return (document as DocumentViewModel).Return(x => x.Link.Return(link => link is LinkBase, () => false), () => false);
		}
	}
}
