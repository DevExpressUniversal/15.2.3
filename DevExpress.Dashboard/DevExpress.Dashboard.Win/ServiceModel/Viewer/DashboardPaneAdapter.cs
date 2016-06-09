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
using DevExpress.DashboardWin.Native;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Adapters;
using DevExpress.XtraLayout.Utils;
using DevExpress.Data.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.ServiceModel {
	public interface IDashboardPaneAdapter {
		DashboardPane GetRootPane();
		void SetRootPane(DashboardPane rootPane);
	}
	public class DashboardPaneAdapter : IDashboardPaneAdapter {
		readonly PrefixNameGenerator groupNameGenerator = new PrefixNameGenerator("#Group");
		readonly DashboardLayoutControl layoutControl;
		readonly TreeStyleLayoutAdapter layoutControlAdapter;
		readonly IServiceProvider serviceProvider;
		public DashboardPaneAdapter(DashboardLayoutControl layoutControl, IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(layoutControl, "layoutControl");
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.layoutControl = layoutControl;
			this.serviceProvider = serviceProvider;
			layoutControlAdapter = new TreeStyleLayoutAdapter(layoutControl);
		}
		public DashboardPane GetRootPane() {
			XAFLayoutItemInfo xafInfo = layoutControlAdapter.GetXAFLayoutInfo();
			return ConvertXAFInfoToDashboardPane(xafInfo);
		}
		public void SetRootPane(DashboardPane rootPane) {
			layoutControl.BeginInit();
			try {
				XAFLayoutItemInfo xafRoot = ConvertDashobardPaneToXAFInfo(null, rootPane);
				if (xafRoot != null) {
					LayoutControlGroup layoutControlRoot = layoutControl.Root;
					xafRoot.Item = layoutControlRoot;
					layoutControlRoot.Name = xafRoot.ID;
					xafRoot.Bounds = new Rectangle(new Point(0, 0), layoutControl.Size);
					xafRoot.AllowGroups = true;
					layoutControlAdapter.SetXAFLayoutInfo(xafRoot);
					layoutControl.UngroupInvisibleGroups(xafRoot);
				}
			}
			finally {
				layoutControl.EndInit();
			}
		}
		DashboardPane ConvertXAFInfoToDashboardPane(XAFLayoutItemInfo layoutInfoNode) {
			DashboardPane resLayoutItem = null;
			IDashboardLayoutControlItem layoutControlItem = layoutControl.Items.FindByName(layoutInfoNode.ID) as IDashboardLayoutControlItem;
			XAFLayoutGroupInfo xafLayoutGroupInfo = layoutInfoNode as XAFLayoutGroupInfo;
			if (layoutControlItem == null) { 
				DashboardPane groupPane = new DashboardPane() {
					Panes = new List<DashboardPane>(),
					Type = DashboardPaneType.Group,
					Orientation = xafLayoutGroupInfo.LayoutType == LayoutType.Horizontal ? DashboardLayoutGroupOrientation.Horizontal : DashboardLayoutGroupOrientation.Vertical
				};
				foreach (XAFLayoutItemInfo layoutInfoItem in xafLayoutGroupInfo.Items)
					groupPane.Panes.Add(ConvertXAFInfoToDashboardPane(layoutInfoItem));
				resLayoutItem = groupPane;
			}
			else {
				if (layoutControlItem.IsGroup) { 
					DashboardPane groupPane = new DashboardPane() {
						Panes = new List<DashboardPane>(),
						Type = DashboardPaneType.Group,
						Orientation = xafLayoutGroupInfo.LayoutType == LayoutType.Horizontal ? DashboardLayoutGroupOrientation.Horizontal : DashboardLayoutGroupOrientation.Vertical
					};
					if (!layoutControlItem.IsHidden)
						groupPane.Name = layoutControlItem.Name;
					foreach (XAFLayoutItemInfo layoutInfoItem in xafLayoutGroupInfo.Items)
						groupPane.Panes.Add(ConvertXAFInfoToDashboardPane(layoutInfoItem));
					resLayoutItem = groupPane;
				}
				else { 
					if (!layoutControlItem.IsHidden) {
						resLayoutItem = new DashboardPane() {
							Name = layoutControlItem.Name,
							Type = DashboardPaneType.Item
						};
					}
				}
			}
			if (layoutInfoNode.Parent != null)
				resLayoutItem.Size = layoutInfoNode.RelativeSize;
			return resLayoutItem;
		}
		XAFLayoutItemInfo ConvertDashobardPaneToXAFInfo(XAFLayoutGroupInfo parentGroupInfo, DashboardPane dashboardPane) {
			if (dashboardPane == null)
				return null;
			XAFLayoutItemInfo resLayoutNode = null;
			if (dashboardPane.Type == DashboardPaneType.Group) {
				XAFLayoutGroupInfo layoutGroupInfo = new XAFLayoutGroupInfo();
				if (string.IsNullOrEmpty(dashboardPane.Name))
					PrepareXAFLayoutGroupInfo(layoutGroupInfo, null, GenerateGroupID(parentGroupInfo), false);
				else {
					DashboardLayoutControlGroup newItem = FindLayoutControlGroup(dashboardPane);
					if (newItem != null)
						PrepareXAFLayoutGroupInfo(layoutGroupInfo, newItem, newItem.Name, newItem.GroupBordersVisible);
				}
				if (dashboardPane.Orientation.HasValue)
					layoutGroupInfo.LayoutType = dashboardPane.Orientation.Value == DashboardLayoutGroupOrientation.Horizontal ? LayoutType.Horizontal : LayoutType.Vertical;
				else
					layoutGroupInfo.LayoutType = LayoutType.Horizontal;
				foreach (DashboardPane innerPane in dashboardPane.Panes) {
					XAFLayoutItemInfo newLayoutItemInfo = ConvertDashobardPaneToXAFInfo(layoutGroupInfo, innerPane);
					if (newLayoutItemInfo != null)
						layoutGroupInfo.Add(newLayoutItemInfo);
				}
				if (layoutGroupInfo.Items.Count == 0 && layoutGroupInfo.Item == null)
					return null;
				resLayoutNode = layoutGroupInfo;
			}
			else {
				XAFLayoutItemInfo layoutInfoNode = new XAFLayoutItemInfo();
				DashboardLayoutControlItem newItem = FindLayoutControlItem(dashboardPane);
				if (newItem != null) {
					layoutInfoNode.ID = newItem.Name;
					layoutInfoNode.Item = newItem;
					resLayoutNode = layoutInfoNode;
				}
				else {
					return null;
				}
			}
			resLayoutNode.RelativeSize = dashboardPane.Size * 100;
			if (parentGroupInfo != null)
				resLayoutNode.Parent = parentGroupInfo;
			return resLayoutNode;
		}
		string GenerateGroupID(XAFLayoutGroupInfo parentGroup) {
			XAFLayoutGroupInfo rootGroup = null;
			if (parentGroup != null) {
				rootGroup = parentGroup;
				while (rootGroup.Parent != null)
					rootGroup = (XAFLayoutGroupInfo)rootGroup.Parent;
			}
			return groupNameGenerator.GenerateName(name => ContainsName(rootGroup, name));
		}
		bool ContainsName(XAFLayoutGroupInfo rootGroup, string name) {
			if (rootGroup != null)
				foreach (XAFLayoutGroupInfo group in GetGroups(rootGroup))
					if (name == group.ID)
						return true;
			return false;
		}
		IEnumerable<XAFLayoutGroupInfo> GetGroups(XAFLayoutGroupInfo layoutGroupInfo) {
			yield return layoutGroupInfo;
			foreach (XAFLayoutItemInfo childItemInfo in layoutGroupInfo.Items) {
				if (!(childItemInfo is XAFLayoutGroupInfo))
					continue;
				XAFLayoutGroupInfo groupInfo = childItemInfo as XAFLayoutGroupInfo;
				foreach (XAFLayoutGroupInfo childGroupInfo in GetGroups(groupInfo))
					yield return childGroupInfo;
			}
		}
		DashboardLayoutControlItem FindLayoutControlItem(DashboardPane dashboardPane) {
			IDashboardLayoutAccessService layoutAccessService = serviceProvider.RequestServiceStrictly<IDashboardLayoutAccessService>();
			return layoutAccessService.FindLayoutControlItem(dashboardPane.Name);
		}
		DashboardLayoutControlGroup FindLayoutControlGroup(DashboardPane dashboardPane) {
			IDashboardLayoutAccessService layoutAccessService = serviceProvider.RequestServiceStrictly<IDashboardLayoutAccessService>();
			return layoutAccessService.FindLayoutControlGroup(dashboardPane.Name);
		}
		static void PrepareXAFLayoutGroupInfo(XAFLayoutGroupInfo layoutGroupInfo, LayoutGroup item, string id, bool groupBordersVisible) {
			layoutGroupInfo.Item = item;
			layoutGroupInfo.ID = id;
			layoutGroupInfo.IsGroupBoundsVisible = groupBordersVisible;
		}
	}
}
