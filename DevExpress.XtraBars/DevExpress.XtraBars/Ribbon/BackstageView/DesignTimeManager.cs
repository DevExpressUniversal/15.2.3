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

using System.ComponentModel;
using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Ribbon {
	public class BackstageViewDesignTimeManager : BaseDesignTimeManager {
		public BackstageViewDesignTimeManager(BackstageViewControl owner, ISite site)
			: base(owner, site) {
		}
		public virtual void OnAddCommand() {
			BackstageViewButtonItem item = new BackstageViewButtonItem();
			BackstageView.Container.Add(item);
			BackstageView.Items.Add(item);
			item.Caption = item.Name;
		}
		public virtual void OnAddTab() {
			BackstageViewTabItem item = new BackstageViewTabItem();
			BackstageView.Container.Add(item.ContentControl);
			BackstageView.Container.Add(item);
			BackstageView.Items.Add(item);
			item.Caption = item.Name;
		}
		public virtual void OnAddSeparator() {
			BackstageViewItemSeparator item = new BackstageViewItemSeparator();
			BackstageView.Container.Add(item);
			BackstageView.Items.Add(item);
		}
		public virtual void OnFillTabs() {
			foreach(BackstageViewItemBase item in BackstageView.Items) {
				BackstageViewTabItem tab = item as BackstageViewTabItem;
				if(tab != null && !tab.HasClientBackstageView) AddChildBackstageView(tab);
			}
		}
		public virtual void AddChildBackstageView(BackstageViewTabItem tab) {
			if(tab.HasClientBackstageView) return;
			BackstageViewControl bsv = new BackstageViewControl();
			bsv.Dock = System.Windows.Forms.DockStyle.Fill;
			DesignerHost.Container.Add(bsv);
			tab.ContentControl.Controls.Add(bsv);
		}
		public virtual void AddRecentItemControl(BackstageViewTabItem tab) {
			if(tab.HasChildRecentItemControl) return;
			RecentItemControl recent = new RecentItemControl();
			recent.Dock = DockStyle.Fill;
			DesignerHost.Container.Add(recent);
			tab.ContentControl.Controls.Add(recent);
		}
		public BackstageViewControl BackstageView { get { return Owner as BackstageViewControl; } }
	}
}
