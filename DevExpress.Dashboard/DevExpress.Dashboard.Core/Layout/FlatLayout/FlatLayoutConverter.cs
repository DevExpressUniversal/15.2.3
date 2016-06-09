#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
namespace DevExpress.DashboardCommon.Native {
	public class FlatLayoutConverter {
		static void CreateDashboardLayout(DashboardLayoutGroup group, DashboardFlatLayoutItemGroupInfo parent) {
			if (parent.Items.Count == 0) return;
			foreach (IDashboardFlatLayoutItemInfo item in parent.Items) {
				DashboardFlatLayoutItemGroupInfo flatParentItem = item as DashboardFlatLayoutItemGroupInfo;
				double weight = parent.GetSize(item);
				if(weight > 0) {
					if(flatParentItem != null) {
						DashboardLayoutGroup childGroup = new DashboardLayoutGroup(flatParentItem.Orientation, weight);
						group.ChildNodes.Add(childGroup);
						CreateDashboardLayout(childGroup, flatParentItem);
					} else {
						DashboardFlatLayoutItem flatLayoutItem = item as DashboardFlatLayoutItem;
						if(flatLayoutItem != null) {
							DashboardLayoutItem childItem = new DashboardLayoutItem(flatLayoutItem.DashboardItem, weight);
							group.ChildNodes.Add(childItem);
						}
					}
				}
			}
		}
		static void PatchParallelPanes(DashboardFlatLayoutItemGroupInfo rootParent) {
			for (int i = 0; i < rootParent.Items.Count; i++) {
				IDashboardFlatLayoutItemInfo item = rootParent.Items[i];
				DashboardFlatLayoutItemGroupInfo parent = item as DashboardFlatLayoutItemGroupInfo;
				if (parent != null) {
					PatchParallelPanes(parent);
					if (parent.Orientation == rootParent.Orientation && parent.HasOnlyParallelPanes) {
						int index = rootParent.Items.IndexOf(item);
						rootParent.Items.Remove(item);
						rootParent.Items.InsertRange(index, parent.Items);
						break;
					}
				}
			}
		}
		static void RoundPanes(DashboardLayoutGroup group) {
			foreach(DashboardLayoutNode childNode in group.ChildNodes) {
				DashboardLayoutGroup childGroup = childNode as DashboardLayoutGroup;
				if(childGroup != null)
					RoundPanes(childGroup);
				childNode.Weight = Math.Round(childNode.Weight, 2);
			}
		}
		readonly List<IDashboardFlatLayoutItemInfo> paneItems = new List<IDashboardFlatLayoutItemInfo>();
		readonly DashboardLayoutGroup layoutRoot;
		public DashboardLayoutGroup LayoutRoot { get { return layoutRoot; } }
		public FlatLayoutConverter(IList<DashboardFlatLayoutItem> items) {
			foreach(DashboardFlatLayoutItem flatLayoutItem in items) {
				this.paneItems.Add(flatLayoutItem);
			}
			ProcessPanes();
			DashboardFlatLayoutItemGroupInfo parent = this.paneItems.Count == 1 ? this.paneItems[0] as DashboardFlatLayoutItemGroupInfo : null;
			if (parent != null)
				PatchParallelPanes(parent);
			else {
				parent = new DashboardFlatLayoutItemGroupInfo();
				foreach (IDashboardFlatLayoutItemInfo item in this.paneItems)
					parent.Items.Add(item);
			}
			this.layoutRoot = new DashboardLayoutGroup();
			this.layoutRoot.Orientation = parent.Orientation;
			this.layoutRoot.Weight = 1;
			CreateDashboardLayout(this.layoutRoot, parent);
			RoundPanes(this.layoutRoot);
		}
		void ProcessPanes() {
			DashboardFlatLayoutItemGroupInfo parent = null;
			foreach (IDashboardFlatLayoutItemInfo paneItem in this.paneItems) {
				parent = GetParent(paneItem);
				if (parent != null) break;
			}
			if (parent != null) {
				foreach (IDashboardFlatLayoutItemInfo item in parent.Items)
					this.paneItems.Remove(item);
				this.paneItems.Add(parent);
				ProcessPanes();
			}
		}
		DashboardFlatLayoutItemGroupInfo GetParent(IDashboardFlatLayoutItemInfo item) {
			DashboardFlatLayoutItemGroupInfo parent = new DashboardFlatLayoutItemGroupInfo();
			parent.AddItem(item);
			int index = 0;
			while (index < this.paneItems.Count) {
				IDashboardFlatLayoutItemInfo paneItem = this.paneItems[index];
				if (item != paneItem && parent.CanAdd(paneItem)) {
					parent.AddItem(paneItem);
					index = 0;
				}
				index++;
			}
			return parent.Items.Count > 1 ? parent : null;
		}
	}
}
