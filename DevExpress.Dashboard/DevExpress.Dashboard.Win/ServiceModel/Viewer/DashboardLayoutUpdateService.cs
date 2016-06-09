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

using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace DevExpress.DashboardWin.ServiceModel {
	public interface IDashboardLayoutUpdateService {
		event EventHandler<DashboardLayoutUpdatedEventsArgs> LayoutUpdated;
		void BeginUpdate();
		void EndUpdate();
		void LockLayoutControlUpdate();
		void UnlockLayoutControlUpdate(bool performLayout);
		void RenameLayoutItem(string oldComponentName, string newComponentName);
		void CreateLayoutItem(string name, string type);
		void CreateLayoutItem(string name, string type, LayoutItemDragController dragController);
		void RemoveLayoutItem(IDashboardLayoutControlItem layoutItem);
		void MoveLayoutItemToRoot(IDashboardLayoutControlItem layoutItem);
	}
	public class DashboardLayoutUpdateService : IDashboardLayoutUpdateService {
		readonly List<IDashboardLayoutControlItem> addedItems = new List<IDashboardLayoutControlItem>();
		readonly Locker locker = new Locker();
		readonly DashboardViewer viewer;
		readonly IServiceProvider serviceProvider;
		readonly LayoutControl layoutControl;		
		bool itemRemoved;
		IDashboardLayoutSelectionService SelectionService { get { return serviceProvider.RequestServiceStrictly<IDashboardLayoutSelectionService>(); } }
		public event EventHandler<DashboardLayoutUpdatedEventsArgs> LayoutUpdated;
		public DashboardLayoutUpdateService(DashboardViewer viewer, LayoutControl layoutControl, IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(viewer, "viewer");
			Guard.ArgumentNotNull(layoutControl, "layoutControl");
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.viewer = viewer;
			this.layoutControl = layoutControl;
			this.serviceProvider = serviceProvider;	
		}
		public void BeginUpdate() { 
			SelectionService.Lock();
			locker.Lock();
		}
		public void EndUpdate() {
			locker.Unlock();
			SelectionService.Unlock();
			OnLayoutUpdated();
		}
		public void LockLayoutControlUpdate() {
			layoutControl.BeginUpdate();   
		}
		public void UnlockLayoutControlUpdate(bool performLayout) {
			layoutControl.EndUpdate();
			if(performLayout)
				layoutControl.PerformLayout();
		}
		public void RenameLayoutItem(string oldComponentName, string newComponentName) {
			IDashboardLayoutAccessService accessService = serviceProvider.RequestServiceStrictly<IDashboardLayoutAccessService>();
			IDashboardLayoutControlItem layoutItem = accessService.FindLayoutItem(oldComponentName);
			if (layoutItem != null) {
				layoutItem.Name = newComponentName;
				layoutItem.ItemViewer.Name = newComponentName;
			}
		}
		public void RemoveLayoutItem(IDashboardLayoutControlItem layoutItem) {
			LayoutGroup parentGroup = layoutItem.Inner.Parent;
			if(parentGroup != null)
				parentGroup.Remove(layoutItem.Inner);
			layoutItem.Inner.Dispose();
			itemRemoved = true;
			OnLayoutUpdated();
		}
		public void CreateLayoutItem(string name, string type) {
			CreateLayoutItem(name, type, null);
		}
		public void CreateLayoutItem(string name, string type, LayoutItemDragController dragController) {
			IDashboardLayoutControlItem layoutItem;
			if(type == DashboardItemType.Group)
				layoutItem = new DashboardLayoutControlGroup(viewer, serviceProvider, false);
			else
				layoutItem = new DashboardLayoutControlItem(viewer, serviceProvider);
			layoutItem.Name = name;
			layoutItem.Type = type;
			viewer.CreateDashboardItemViewer(name, type, layoutItem);
			if(dragController == null) {
				if (type == DashboardItemType.Group)
					layoutControl.AddGroup((LayoutGroup)layoutItem.Inner);
				else
					layoutControl.AddItem(layoutItem.Inner);
			} else {
				LayoutItemDragController controller = new LayoutItemDragController(layoutItem.Inner, dragController);
				controller.DragWildItem();
			}
			addedItems.Add(layoutItem);
			OnLayoutUpdated();
		}
		public void MoveLayoutItemToRoot(IDashboardLayoutControlItem layoutItem) {
			layoutItem.BeginCustomization();
			try {
				layoutItem.Inner.HideToCustomization();
				layoutItem.Inner.RestoreFromCustomization(layoutControl.Root);
			}
			finally {
				layoutItem.EndCustomization();
			}
		}
		void OnLayoutUpdated() {
			if (!locker.IsLocked && (addedItems.Count > 0 || itemRemoved)) {
				if (LayoutUpdated != null)
					LayoutUpdated(this, new DashboardLayoutUpdatedEventsArgs(addedItems.ToArray()));
				addedItems.Clear();
				itemRemoved = false;
			}
		}
	}
}
