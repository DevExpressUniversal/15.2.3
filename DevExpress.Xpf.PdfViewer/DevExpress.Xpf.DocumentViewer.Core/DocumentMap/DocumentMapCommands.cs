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
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DocumentViewer.Extensions;
using DevExpress.Xpf.Grid;
namespace DevExpress.Xpf.DocumentViewer {
	public class DocumentMapCommands {
		public static ICommand ExpandTopLevelNodesCommand { get; private set; }
		public static ICommand CollapseTopLevelNodesCommand { get; private set; }
		public static ICommand ExpandCurrentNodeCommand { get; private set; }
		public static ICommand GoToNodeCommand { get; private set; }
		static DocumentMapCommands() {
			ExpandTopLevelNodesCommand = new RoutedCommand("ExpandTopLevelNodesCommand", typeof(DocumentMapCommands));
			CommandManager.RegisterClassCommandBinding(typeof(TreeListView), new CommandBinding(ExpandTopLevelNodesCommand,
				(d, e) => ExecuteExpandTopLevelNodesCommand((TreeListView)d, e), (d, e) => CanExecuteExpandTopLevelNodesCommand((TreeListView)d, e)));
			CollapseTopLevelNodesCommand = new RoutedCommand("CollapseTopLevelNodesCommand", typeof(DocumentMapCommands));
			CommandManager.RegisterClassCommandBinding(typeof(TreeListView), new CommandBinding(CollapseTopLevelNodesCommand,
				(d, e) => ExecuteCollapseTopLevelNodesCommand((TreeListView)d, e), (d, e) => CanExecuteCollapseTopLevelNodesCommand((TreeListView)d, e)));
			ExpandCurrentNodeCommand = new RoutedCommand("ExpandCurrentNodeCommand", typeof(DocumentMapCommands));
			CommandManager.RegisterClassCommandBinding(typeof(TreeListView), new CommandBinding(ExpandCurrentNodeCommand,
				(d, e) => ExecuteExpandCurrentNodeCommand((TreeListView)d, e), (d, e) => ExecuteCanExpandCurrentNodeCommand((TreeListView)d, e)));
			GoToNodeCommand = new RoutedCommand("GoToNode", typeof(DocumentMapCommands));
			CommandManager.RegisterClassCommandBinding(typeof(TreeListView), new CommandBinding(GoToNodeCommand,
				(d, e) => ExecuteGoToNodeCommand((FrameworkElement)d, e)));
			CommandManager.RegisterClassCommandBinding(typeof(GridControl), new CommandBinding(GoToNodeCommand,
				(d, e) => ExecuteGoToNodeCommand((FrameworkElement)d, e)));
		}
		static void ExecuteCanExpandCurrentNodeCommand(TreeListView d, CanExecuteRoutedEventArgs e) {
			e.CanExecute = d.GetNodeByContent(e.Parameter).Return(x => x.HasChildren, () => false);
			e.Handled = true;
		}
		public static void ExecuteGoToNodeCommand(FrameworkElement d, ExecutedRoutedEventArgs e) {
			var documentMap = (DocumentMapControl)LayoutHelper.FindLayoutOrVisualParentObject(d, o => o is DocumentMapControl);
			if (documentMap == null)
				return;
			documentMap.HighlightedItem = e.Parameter;
			documentMap.ActualSettings.GoToCommand.TryExecute(e.Parameter);
			e.Handled = true;
		}
		public static void CanExecuteExpandTopLevelNodesCommand(TreeListView d, CanExecuteRoutedEventArgs e) {
			e.CanExecute = d.Nodes.All(x => !x.IsExpanded);
			e.Handled = true;
		}
		static void ExecuteExpandTopLevelNodesCommand(TreeListView d, ExecutedRoutedEventArgs e) {
			foreach (var node in d.Nodes)
				node.IsExpanded = true;
			d.ScrollIntoView(d.FocusedNode.RowHandle);
			e.Handled = true;
		}
		public static void CanExecuteCollapseTopLevelNodesCommand(TreeListView d, CanExecuteRoutedEventArgs e) {
			e.CanExecute = d.Nodes.Any(x => x.IsExpanded);
			e.Handled = true;
		}
		static void ExecuteCollapseTopLevelNodesCommand(TreeListView d, ExecutedRoutedEventArgs e) {
			d.DataControl.UnselectAll();
			foreach (var node in d.Nodes)
				node.IsExpanded = false;
			d.DataControl.SelectItem(d.FocusedNode.RowHandle);
			d.ScrollIntoView(d.FocusedNode.RowHandle);
			e.Handled = true;
		}
		static void ExecuteExpandCurrentNodeCommand(TreeListView d, ExecutedRoutedEventArgs e) {
			var node = d.GetNodeByRowHandle(d.FocusedRowHandle);
			TreeListNode last = node;
			while (node != null) {
				last = node;
				node = node.Nodes.FirstOrDefault();
			}
			TreeListNode parent = last;
			while (parent != null) {
				parent.IsExpanded = true;
				parent = parent.ParentNode;
			}
			d.DataControl.UnselectAll();
			d.DataControl.SelectItem(last.RowHandle);
			d.FocusedNode = last;
			d.ScrollIntoView(d.FocusedNode.RowHandle);
		}
	}
}
