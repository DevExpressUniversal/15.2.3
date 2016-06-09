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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace DevExpress.DashboardWin.ServiceModel {
	public interface IDashboardLayoutAccessService {
		IEnumerable<IDashboardLayoutControlItem> VisibleLayoutControlItems { get; }
		IEnumerable<DashboardItemViewer> ItemViewers { get; }
		IDashboardLayoutControlItem FindLayoutItem(string componentName);
		DashboardLayoutControlItem FindLayoutControlItem(string componentName);
		DashboardLayoutControlGroup FindLayoutControlGroup(string componentName);
		DashboardItemViewer FindDashboardItemViewer(string componentName);
	}
	public class DashboardLayoutAccessService : IDashboardLayoutAccessService {
		readonly DashboardLayoutControl layoutControl;
		public IEnumerable<IDashboardLayoutControlItem> VisibleLayoutControlItems { 
			get { return LayoutControlItems.Where(layoutItem => !layoutItem.IsHidden); } 
		}
		public IEnumerable<DashboardItemViewer> ItemViewers {
			get {
				return LayoutControlItems.
					Where(item => item.ItemViewer != null).
					Select<IDashboardLayoutControlItem, DashboardItemViewer>(item => item.ItemViewer);
			}
		}
		IEnumerable<IDashboardLayoutControlItem> LayoutControlItems {
			get {
				return layoutControl.Items.
					Where(item => item is IDashboardLayoutControlItem).
					Cast<IDashboardLayoutControlItem>();
			}
		}
		public DashboardLayoutAccessService(DashboardLayoutControl layoutControl) {
			Guard.ArgumentNotNull(layoutControl, "layoutControl");
			this.layoutControl = layoutControl;
		}
		public IDashboardLayoutControlItem FindLayoutItem(string componentName) {
			return FindLayoutItem<IDashboardLayoutControlItem>(componentName);
		}
		public DashboardLayoutControlItem FindLayoutControlItem(string componentName) {
			return FindLayoutItem<DashboardLayoutControlItem>(componentName);
		}
		public DashboardLayoutControlGroup FindLayoutControlGroup(string componentName) {
			return FindLayoutItem<DashboardLayoutControlGroup>(componentName);
		}
		public DashboardItemViewer FindDashboardItemViewer(string componentName) {
			IDashboardLayoutControlItem layoutItem = FindLayoutItem(componentName);
			if (layoutItem != null)
				return layoutItem.ItemViewer;
			return null;
		}
		TLayoutItem FindLayoutItem<TLayoutItem>(string componentName) where TLayoutItem : class, IDashboardLayoutControlItem {
			foreach(BaseLayoutItem item in layoutControl.Items) {
				TLayoutItem layoutItem = item as TLayoutItem;
				if(layoutItem != null && layoutItem.Name == componentName)
					return layoutItem;
			}
			return null;
		}
	}
	public class DashboardLayoutUpdatedEventsArgs : EventArgs {
		public ReadOnlyCollection<IDashboardLayoutControlItem> AddedItems { get; private set; }
		public DashboardLayoutUpdatedEventsArgs(IList<IDashboardLayoutControlItem> addedItems) {
			AddedItems = new ReadOnlyCollection<IDashboardLayoutControlItem>(addedItems);
		}
	}
}
