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
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.Diagram.Core {
	public static class ArrangeActions {
		public static void BringSelectionToFront(this IDiagramControl diagram) {
			ChangeIndex(diagram, (index, firstIndex, lastIndex) => lastIndex, ListSortDirection.Descending);
		}
		public static void SendSelectionToBack(this IDiagramControl diagram) {
			ChangeIndex(diagram, (index, firstIndex, lastIndex) => firstIndex, ListSortDirection.Ascending);
		}
		public static void BringSelectionForward(this IDiagramControl diagram) {
			ChangeIndex(diagram, (index, firstIndex, lastIndex) => Math.Min(index + 1, lastIndex), ListSortDirection.Descending);
		}
		public static void SendSelectionBackward(this IDiagramControl diagram) {
			ChangeIndex(diagram, (index, firstIndex, lastIndex) => Math.Max(index - 1, firstIndex), ListSortDirection.Ascending);
		}
		static void ChangeIndex(IDiagramControl diagram, Func<int, int, int, int> getIndex, ListSortDirection sortDirection) {
			diagram.ExecuteWithSelectionRestore(transaction => {
				diagram.SelectedItems()
					.Where(x => x.Owner() != null)
					.GroupBy(x => x.Owner())
					.ForEach(items => {
						items
							.OrderBy(x => x.GetIndexInOwnerCollection(), sortDirection)
							.ToArray()
							.ForEach((item, index) => {
								var oldIndex = item.GetIndexInOwnerCollection();
								var newIndex = getIndex(oldIndex, index, item.OwnerCollection().Count - 1 - index);
								if(oldIndex == newIndex)
									return;
								var changeZOrderState = ChangeZOrderState.Create(item.Owner(), oldIndex, newIndex);
								transaction.Execute(changeZOrderState, x => x.ChangeZOrder(), x => x.ChangeZOrder());
							});
					});
			});
		}
		class ChangeZOrderState {
			public static ChangeZOrderState Create(IDiagramItem owner, int oldIndex, int newIndex) {
				return new ChangeZOrderState(owner.GetFinder(), oldIndex, newIndex);
			}
			readonly IItemFinder<IDiagramItem> ownerFinder;
			readonly int oldIndex, newIndex;
			ChangeZOrderState(IItemFinder<IDiagramItem> ownerFinder, int oldIndex, int newIndex) {
				this.ownerFinder = ownerFinder;
				this.oldIndex = oldIndex;
				this.newIndex = newIndex;
			}
			public ChangeZOrderState ChangeZOrder() {
				var container = ownerFinder.FindItem();
				var item = container.NestedItems[oldIndex];
				item.Controller.MoveToOwner(container, newIndex);
				return new ChangeZOrderState(ownerFinder, newIndex, oldIndex);
			}
		}
	}
}
