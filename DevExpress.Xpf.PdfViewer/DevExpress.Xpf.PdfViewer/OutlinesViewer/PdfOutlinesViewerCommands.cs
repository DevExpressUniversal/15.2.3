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

using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Grid;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfOutlinesViewerCommands : DocumentMapCommands {
		public static ICommand PrintCommand { get; private set; }
		public static ICommand PrintSectionCommand { get; private set; }
		static PdfOutlinesViewerCommands() {
			PrintCommand = new RoutedCommand("PrintCommand", typeof(PdfOutlinesViewerCommands));
			CommandManager.RegisterClassCommandBinding(typeof(TreeListView), new CommandBinding(PrintCommand,
				(d, e) => ExecutePrintCommand((TreeListView)d, e), (d, e) => CanExecutePrintCommand((TreeListView)d, e)));
			PrintSectionCommand = new RoutedCommand("PrintSectionCommand", typeof(PdfOutlinesViewerCommands));
			CommandManager.RegisterClassCommandBinding(typeof(TreeListView), new CommandBinding(PrintSectionCommand,
				(d, e) => ExecutePrintSectionCommand((TreeListView)d, e), (d, e) => CanExecutePrintSectionCommand((TreeListView)d, e)));
		}
		static void CanExecutePrintCommand(TreeListView d, CanExecuteRoutedEventArgs e) {
			var documentMap = (DocumentMapControl)LayoutHelper.FindLayoutOrVisualParentObject(d, o => o is DocumentMapControl);
			e.CanExecute = documentMap.With(x => x.ActualSettings as PdfOutlinesViewerSettings).If(x => x.PrintCommand.CanExecute(e.Parameter)).ReturnSuccess();
			e.Handled = true;
		}
		static void ExecutePrintCommand(TreeListView d, ExecutedRoutedEventArgs e) {
			var documentMap = (DocumentMapControl)LayoutHelper.FindLayoutOrVisualParentObject(d, o => o is DocumentMapControl);
			documentMap.With(x => x.ActualSettings as PdfOutlinesViewerSettings).Do(x => x.PrintCommand.Execute(e.Parameter));
			e.Handled = true;
		}
		static void CanExecutePrintSectionCommand(TreeListView d, CanExecuteRoutedEventArgs e) {
			var documentMap = (DocumentMapControl)LayoutHelper.FindLayoutOrVisualParentObject(d, o => o is DocumentMapControl);
			e.CanExecute = documentMap.With(x => x.ActualSettings as PdfOutlinesViewerSettings).If(x => x.PrintSectionCommand.CanExecute(e.Parameter)).ReturnSuccess();
			e.Handled = true;
		}
		static void ExecutePrintSectionCommand(TreeListView d, ExecutedRoutedEventArgs e) {
			var documentMap = (DocumentMapControl)LayoutHelper.FindLayoutOrVisualParentObject(d, o => o is DocumentMapControl);
			documentMap.With(x => x.ActualSettings as PdfOutlinesViewerSettings).Do(x => x.PrintSectionCommand.Execute(e.Parameter));
			e.Handled = true;
		}
	}
}
