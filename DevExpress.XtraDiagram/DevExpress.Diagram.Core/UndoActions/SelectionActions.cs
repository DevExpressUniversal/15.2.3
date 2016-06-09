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

using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Diagram.Core {
	public static class SelectionActions {
		public static void ExecuteWithSelectionRestore(this IDiagramControl diagram, Action<Transaction> action, IDiagramItem[] restoreSelectionItems = null, bool allowMerge = true) {
			diagram.UndoManager().ExecuteWithSelectionRestore(diagram, action, restoreSelectionItems, allowMerge: allowMerge);
		}
		public static void ExecuteWithSelectionRestore(this UndoManagerBase transaction, IDiagramControl diagram, Action<Transaction> action, IDiagramItem[] restoreSelectionItems = null, bool allowMerge = true) {
			transaction.Execute(innerTransaction => {
				restoreSelectionItems = restoreSelectionItems ?? diagram.SelectedItems().ToArray();
				innerTransaction.RestoreSelectionOnUndo(diagram);
				if(innerTransaction.Execute(nestedTransaction => action(nestedTransaction))) {
					innerTransaction.RestoreSelectionOnRedo(diagram, restoreSelectionItems);
				} else {
					innerTransaction.Clear();
				}
			}, allowMerge);
		}
		static void RestoreSelectionOnUndo(this Transaction transaction, IDiagramControl diagram) {
			transaction.RestoreCore(diagram, diagram.SelectedItems().ToArray(), true);
		}
		static void RestoreSelectionOnRedo(this Transaction transaction, IDiagramControl diagram, IDiagramItem[] items) {
			items = items ?? diagram.SelectedItems().ToArray();
			transaction.RestoreCore(diagram, items, false);
		}
		static void RestoreCore(this Transaction transaction, IDiagramControl diagram, IDiagramItem[] items, bool restoreOnUndo) {
			var state = new { Finders = items.Select(x => x.GetFinder()).ToArray(), Diagram = diagram, RestoreOnUndo = restoreOnUndo };
			transaction.Execute(state,
				x => {
					if(!x.RestoreOnUndo)
						RestoreSelection(x.Diagram, x.Finders);
					return x;
				},
				x => {
					if(x.RestoreOnUndo)
						RestoreSelection(x.Diagram, x.Finders);
					return x;
				},
				(x1, x2) => x1.Finders.SequenceEqual(x2.Finders) ? x1 : null
			);
		}
		static void RestoreSelection(IDiagramControl diagram, IItemFinder<IDiagramItem>[] finders) {
			diagram.SelectItems(finders.Select(finder => finder.FindItem()));
		}
	}
}
