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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid;
using DevExpress.Mvvm.Native;
using System;
using System.Windows.Input;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfAttachmentsViewerCommands {
		public static ICommand OpenAttachmentCommand { get; private set; }
		public static ICommand SaveAttachmentCommand { get; private set; }
		static PdfAttachmentsViewerCommands() {
			Type ownerType = typeof(PdfAttachmentsViewerCommands);
			OpenAttachmentCommand = new RoutedCommand("OpenAttachment", ownerType);
			CommandManager.RegisterClassCommandBinding(typeof(TableView), new CommandBinding(OpenAttachmentCommand,
				(d, e) => ExecuteOpenAttachmentCommand((TableView)d, e), (d, e) => CanExecuteOpenAttachmentCommand((TableView)d, e)));
			SaveAttachmentCommand = new RoutedCommand("SaveAttachment", ownerType);
			CommandManager.RegisterClassCommandBinding(typeof(TableView), new CommandBinding(SaveAttachmentCommand,
				(d, e) => ExecuteSaveAttachmentCommand((TableView)d, e), (d, e) => CanExecuteSaveAttachmentCommand((TableView)d, e)));
		}
		static void CanExecuteOpenAttachmentCommand(TableView d, CanExecuteRoutedEventArgs e) {
			var attachmentsViewer = (PdfAttachmentsViewerControl)LayoutHelper.FindLayoutOrVisualParentObject(d, o => o is PdfAttachmentsViewerControl);
			e.CanExecute = attachmentsViewer.With(x => x.ActualSettings as PdfAttachmentsViewerSettings).With(x => x.OpenAttachmentCommand).If(x => x.CanExecute(e.Parameter)).ReturnSuccess();
			e.Handled = true;
		}
		static void ExecuteOpenAttachmentCommand(TableView d, ExecutedRoutedEventArgs e) {
			var attachmentsViewer = (PdfAttachmentsViewerControl)LayoutHelper.FindLayoutOrVisualParentObject(d, o => o is PdfAttachmentsViewerControl);
			attachmentsViewer.With(x => x.ActualSettings as PdfAttachmentsViewerSettings).Do(x => x.OpenAttachmentCommand.Execute(e.Parameter));
			e.Handled = true;
		}
		static void CanExecuteSaveAttachmentCommand(TableView d, CanExecuteRoutedEventArgs e) {
			var attachmentsViewer = (PdfAttachmentsViewerControl)LayoutHelper.FindLayoutOrVisualParentObject(d, o => o is PdfAttachmentsViewerControl);
			e.CanExecute = attachmentsViewer.With(x => x.ActualSettings as PdfAttachmentsViewerSettings).With(x => x.SaveAttachmentCommand).If(x => x.CanExecute(e.Parameter)).ReturnSuccess();
			e.Handled = true;
		}
		static void ExecuteSaveAttachmentCommand(TableView d, ExecutedRoutedEventArgs e) {
			var attachmentsViewer = (PdfAttachmentsViewerControl)LayoutHelper.FindLayoutOrVisualParentObject(d, o => o is PdfAttachmentsViewerControl);
			attachmentsViewer.With(x => x.ActualSettings as PdfAttachmentsViewerSettings).Do(x => x.SaveAttachmentCommand.Execute(e.Parameter));
			e.Handled = true;
		}
	}
}
