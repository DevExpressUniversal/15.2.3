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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardLayoutCreator {
		readonly int clientWidth;
		readonly int clientHeight;
		readonly IEnumerable<DashboardItem> items;
		readonly IEnumerable<DashboardItemGroup> groups;
		public DashboardLayoutGroup LayoutRoot { get; private set; }
		public DashboardLayoutCreator(int clientWidth, int clientHeight, DashboardLayoutGroup layoutRoot, IEnumerable<DashboardItem> items, IEnumerable<DashboardItemGroup> groups) {
			Guard.ArgumentPositive(clientWidth, "clientWidth");
			Guard.ArgumentPositive(clientHeight, "clientHeight");
			Guard.ArgumentNotNull(items, "items");
			Guard.ArgumentNotNull(groups, "groups");
			this.clientWidth = clientWidth;
			this.clientHeight = clientHeight;
			this.items = items;
			this.groups = groups;
			LayoutRoot = layoutRoot ?? new DashboardLayoutGroup();
			CreateLayout();
		}
		void CreateLayout() {
			LayoutRoot.BeginUpdate();
			try {
				RemoveIncorrectLayoutNodes();
				foreach (DashboardItemGroup itemGroup in groups)
					if (!LayoutRoot.ContainsRecursive(itemGroup))
						CreateDashboardLayoutNode(itemGroup);
				foreach (DashboardItem dashboardItem in items)
					if (!LayoutRoot.ContainsRecursive(dashboardItem))
						CreateDashboardLayoutNode(dashboardItem);
			}
			finally {
				LayoutRoot.CancelUpdate();
			}
		}
		void RemoveIncorrectLayoutNodes() {
			List<DashboardLayoutNode> layoutNodesToRemove = new List<DashboardLayoutNode>();
			foreach (DashboardLayoutNode layoutNode in LayoutRoot.GetNodesRecursive()) {
				DashboardItem item = layoutNode.DashboardItem;
				if (item != null) {
					if (!items.Contains(item) && !groups.Contains(item))
						layoutNodesToRemove.Add(layoutNode);
					else {
						DashboardLayoutGroup parent = layoutNode.Parent;
						while (IsHiddenGroup(parent))
							parent = parent.Parent;
						if (parent.DashboardItem != item.Group)
							layoutNodesToRemove.Add(layoutNode);
					}
				}
			}
			foreach (DashboardLayoutNode layoutNode in layoutNodesToRemove) {
				DashboardLayoutGroup parent = layoutNode.Parent;
				parent.ChildNodes.Remove(layoutNode);
				while (IsHiddenGroup(parent)) {
					DashboardLayoutGroup parentsParent = parent.Parent;
					if (parent.ChildNodes.Count == 0)
						parentsParent.ChildNodes.Remove(parent);
					else if (parent.ChildNodes.Count == 1) {
						DashboardLayoutNode child = parent.ChildNodes[0];
						child.Weight = parent.Weight;
						int parentIndex = parentsParent.ChildNodes.IndexOf(parent);
						parentsParent.ChildNodes.RemoveAt(parentIndex);
						parentsParent.ChildNodes.Insert(parentIndex, child);
					}
					else
						break;
					parent = parentsParent;
				}
			}
		}
		bool IsHiddenGroup(DashboardLayoutGroup group) {
			return group != LayoutRoot && group.DashboardItem == null;
		}
		void CreateDashboardLayoutNode(DashboardItem dashboardItem) {
			DashboardLayoutGroup layoutGroup = dashboardItem.Group != null ? LayoutRoot.FindRecursive(dashboardItem.Group) : LayoutRoot;
			CreateDashboardLayoutNode(dashboardItem, layoutGroup);
		}
		void CreateDashboardLayoutNode(DashboardItem dashboardItem, DashboardLayoutGroup layoutGroup) {
			if (layoutGroup.ChildNodes.Count == 0) {
				layoutGroup.ChildNodes.Add(dashboardItem.CreateDashboardLayoutNode(1));
				return;
			}
			layoutGroup.UpdateActualSize();
			double maxArea = 0;
			double maxItemWidth = 0;
			double maxItemHeight = 0;
			DashboardLayoutNode maxItem = null;
			IEnumerable<DashboardLayoutNode> nodes = layoutGroup.Parent == null ? layoutGroup.GetDashboardItemsRecursive() : layoutGroup.ChildNodes;
			foreach (DashboardLayoutNode layoutItem in nodes) {
				double itemWidth = layoutItem.ActualRelativeWidth * clientWidth;
				double itemHeight = layoutItem.ActualRelativeHeight * clientHeight;
				double area = itemWidth * itemHeight;
				if (Math.Abs(area) > maxArea) {
					maxArea = Math.Abs(area);
					maxItemWidth = Math.Abs(itemWidth);
					maxItemHeight = Math.Abs(itemHeight);
					maxItem = layoutItem;
				}
			}
			if (maxItem == null) {
				layoutGroup.ChildNodes.Add(dashboardItem.CreateDashboardLayoutNode(1));
				return;
			}
			if (maxItem.Parent.Orientation == DashboardLayoutGroupOrientation.Horizontal) {
				if (maxItemHeight < maxItemWidth) {
					maxItem.Weight = maxItem.Weight / 2;
					maxItem.InsertRight(dashboardItem.CreateDashboardLayoutNode(maxItem.Weight));
				}
				else
					maxItem.InsertBelow(dashboardItem.CreateDashboardLayoutNode(maxItem.Weight));
			}
			else {
				if (maxItemHeight > maxItemWidth) {
					maxItem.Weight = maxItem.Weight / 2;
					maxItem.InsertBelow(dashboardItem.CreateDashboardLayoutNode(maxItem.Weight));
				}
				else
					maxItem.InsertRight(dashboardItem.CreateDashboardLayoutNode(maxItem.Weight));
			}
		}
	}
}
