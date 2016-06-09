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

using DevExpress.DashboardCommon.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.Native {
	public static class DashboardLayoutConverter {
		public static DashboardPane CreatePanes(DashboardLayoutGroup layoutRoot) {
			DashboardPane rootPane = CreatePanesInternal(layoutRoot);
			return rootPane ?? DashboardPane.DefaultRootPane;
		}
		static DashboardPane CreatePanesInternal(DashboardLayoutNode layoutItem) {
			if (layoutItem == null)
				return null;
			DashboardPane pane = new DashboardPane();
			pane.Panes = new List<DashboardPane>();
			DashboardLayoutGroup group = layoutItem as DashboardLayoutGroup;
			if (group != null) {
				pane.Orientation = group.Orientation;
				pane.Type = DashboardPaneType.Group;
				DashboardItem dashboardItem = group.DashboardItem;
				if (dashboardItem != null)
					pane.Name = dashboardItem.ComponentName;
				foreach (DashboardLayoutNode childLayoutItem in group.ChildNodes) {
					DashboardPane newPane = CreatePanesInternal(childLayoutItem);
					if (newPane != null)
						pane.Panes.Add(newPane);
				}
				if (dashboardItem == null && pane.Panes.Count == 0) 
					return null;
			}
			else {
				DashboardLayoutItem item = layoutItem as DashboardLayoutItem;
				if (item != null) {
					DashboardItem dashboardItem = item.DashboardItem;
					if (dashboardItem != null) {
						pane.Orientation = null;
						pane.Name = dashboardItem.ComponentName;
						pane.Type = DashboardPaneType.Item;
					}
					else
						return null;
				}
			}
			pane.Size = layoutItem.Parent != null ? Math.Round((layoutItem.Weight / layoutItem.Parent.GetTotalChildrenWeight()), 2) : 1;
			return pane;
		}
		public static DashboardLayoutGroup CreateDashboardLayout(DashboardPane pane, IEnumerable<DashboardItem> items, IEnumerable<DashboardItemGroup> groups) {
			DashboardLayoutGroup layoutRoot = new DashboardLayoutGroup(pane.Orientation.Value, 1);
			CreateDashboardLayoutInternal(layoutRoot, pane, items, groups);
			return layoutRoot;
		}
		static void CreateDashboardLayoutInternal(DashboardLayoutGroup parentGroup, DashboardPane parentPane, IEnumerable<DashboardItem> items, IEnumerable<DashboardItemGroup> groups) {
			foreach(DashboardPane pane in parentPane.Panes){
				if(pane.Type == DashboardPaneType.Group) {
					DashboardLayoutGroup layoutGroup = new DashboardLayoutGroup();
					layoutGroup.Orientation = pane.Orientation.Value;
					layoutGroup.Weight = pane.Size;
					if(!string.IsNullOrEmpty(pane.Name))
						layoutGroup.DashboardItem = groups.FirstOrDefault(item => item.ComponentName == pane.Name);
					if(pane.Panes != null && pane.Panes.Count > 0)
						CreateDashboardLayoutInternal(layoutGroup, pane, items, groups);
					parentGroup.ChildNodes.Add(layoutGroup);
				} else {
					if(!string.IsNullOrEmpty(pane.Name)) {
						DashboardItem dashboardItem = items.FirstOrDefault(item => item.ComponentName == pane.Name);
						if(dashboardItem != null) {
							DashboardLayoutItem layoutItem = new DashboardLayoutItem();
							layoutItem.Weight = pane.Size;
							layoutItem.DashboardItem = dashboardItem;
							parentGroup.ChildNodes.Add(layoutItem);
						}
					}
				}
			}
		}
	}
}
