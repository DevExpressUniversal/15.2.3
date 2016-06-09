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

using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.XtraBars.Ribbon;
using System;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Bars.Native;
namespace DevExpress.DashboardWin.Bars {
	[DXToolboxItem(false)]
	public class DashboardBackstageViewControl : BackstageViewControl {
		DashboardBackstageRecentTab recentTab;
		int separatorIndex;
		public DashboardBackstageRecentTab DashboardRecentTab {
			get { return recentTab; }
			set { recentTab = value; }
		}
		internal int SeparatorIndex { get { return separatorIndex; } }
		internal IList<DashboardRecentItem> RecentItems {
			get {
				List<DashboardRecentItem> recentItems = new List<DashboardRecentItem>();
				foreach(BackstageViewItemBase item in Items) {
					DashboardRecentItem recentItem = item as DashboardRecentItem;
					if(recentItem != null)
						recentItems.Add(recentItem);
				}
				return recentItems;
			}
		}
		internal void SetSeparatorIndex() {
			if(DashboardRecentTab != null)
				separatorIndex = Items.IndexOf(DashboardRecentTab);
		}
		internal void RemoveSeparator() {
			BackstageViewItemSeparator separator = Items[separatorIndex] as BackstageViewItemSeparator;
			if(separator != null) {
				Items.Remove(separator);
				separator.Dispose();
			}
		}
		internal void Initialize(IServiceProvider serviceProvider) {
			Items.Add(new DashboardBackstageNewButton { ServiceProvider = serviceProvider, Glyph = ImageHelper.GetImage("Bars.NewDashboard_16x16") });
			Items.Add(new DashboardBackstageOpenButton { ServiceProvider = serviceProvider, Glyph = ImageHelper.GetImage("Bars.OpenDashboard_16x16") });
			Items.Add(new DashboardBackstageSaveButton { ServiceProvider = serviceProvider, Glyph = ImageHelper.GetImage("Bars.SaveDashboard_16x16") });
			Items.Add(new DashboardBackstageSaveAsButton { ServiceProvider = serviceProvider, Glyph = ImageHelper.GetImage("Bars.SaveDashboardAs_16x16") });
			recentTab = new DashboardBackstageRecentTab();
			Items.Add(recentTab);
			recentTab.Initialize(serviceProvider, Controller);
		}
		internal void HideContentControl() {
			if(Ribbon != null)
				Ribbon.HideApplicationButtonContentControl();
		}
		internal void ClearRecentItems() {
			foreach(BackstageViewItemBase item in RecentItems) {
				Items.Remove(item);
				item.Dispose();
			}
			RemoveSeparator();
		}
		internal void RemoveRecentItem(DashboardMenuFileLabel label) {
			foreach(DashboardRecentItem recentItem in RecentItems)
				if(recentItem != null && recentItem.FileName == label.Path) {
					Items.Remove(recentItem);
					recentItem.Dispose();
				}
			if(RecentItems.Count == 0)
				RemoveSeparator();
		}
	}
}
