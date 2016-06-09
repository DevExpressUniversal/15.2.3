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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core.Serialization;
using DevExpress.Internal;
using System.Windows.Controls;
namespace DevExpress.Diagram.Core {
	sealed class ModifiedDiagram {
		public ModifiedDiagram(IDiagramControl diagram) {
			this.diagram = diagram;
		}
		readonly IDiagramControl diagram;
		readonly Dictionary<IDiagramItem, Rect> changedBounds = new Dictionary<IDiagramItem, Rect>();
		readonly Dictionary<IDiagramItem, IDiagramItem> changedOwners = new Dictionary<IDiagramItem, IDiagramItem>();
		readonly Dictionary<IDiagramItem, List<IDiagramItem>> itemsWithChangedOwnersByOwner = new Dictionary<IDiagramItem, List<IDiagramItem>>();
		public Rect GetBounds(IDiagramItem item) {
			Rect bounds;
			return changedBounds.TryGetValue(item, out bounds) ? bounds : item.ActualDiagramBounds();
		}
		public IDiagramItem GetOwner(IDiagramItem item) {
			IDiagramItem owner;
			return changedOwners.TryGetValue(item, out owner) ? owner : item.Owner();
		}
		public IEnumerable<IDiagramItem> GetNestedItems(IDiagramItem item) {
			var unchangedItems = item.NestedItems.Where(x => !changedOwners.ContainsKey(x));
			List<IDiagramItem> addedItems;
			return itemsWithChangedOwnersByOwner.TryGetValue(item, out addedItems) ? unchangedItems.Concat(addedItems) : unchangedItems;
		}
		public void SetOwner(IDiagramItem item, IDiagramItem owner) {
			IDiagramItem oldOwner;
			if(changedOwners.TryGetValue(item, out oldOwner) && oldOwner != null)
				itemsWithChangedOwnersByOwner[oldOwner].Remove(item);
			changedOwners[item] = owner;
			if(owner != null)
				itemsWithChangedOwnersByOwner.GetOrAdd(owner, () => new List<IDiagramItem>()).Add(item);
		}
		public void SetBounds(IDiagramItem item, Rect bounds) {
			changedBounds[item] = bounds;
		}
		public bool Apply(Transaction transaction) {
			bool hasChanges = false;
			foreach(var change in changedOwners) {
				hasChanges = true;
				if(change.Value.IsInDiagram())
					transaction.ChangeItemOwner(change.Value, change.Key);
				else
					transaction.AddItem(change.Value, change.Key);
			}
			foreach(var change in changedBounds) {
				hasChanges = true;
				transaction.SetItemBounds(change.Key, change.Value);
			}
			changedOwners.Clear();
			changedBounds.Clear();
			return hasChanges;
		}
	}
	static class MoveHelper2 {
		public static bool DoMoveItems(IDiagramControl diagram, Transaction transaction, Item_Owner_Bounds[] moveInfos, bool useAnchors, IEnumerable<Direction> directions = null, bool cutOutOfBounds = false) {
			var modifiedDiagram = new ModifiedDiagram(diagram);
			DoMoveItems(modifiedDiagram, moveInfos);
			return modifiedDiagram.Apply(transaction);
		}
		public static void DoMoveItems(ModifiedDiagram diagram, Item_Owner_Bounds[] moveInfos) {
			foreach(var moveInfo in moveInfos)
				diagram.SetBounds(moveInfo.Item, moveInfo.Bounds);
		}
	}
}
