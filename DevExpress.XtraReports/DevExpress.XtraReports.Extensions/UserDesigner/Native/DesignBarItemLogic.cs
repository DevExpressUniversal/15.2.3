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
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel.Design;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Design.Commands;
using System.Drawing;
using System.Drawing.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.UserDesigner.Native {
	public class XRDesignBarMangerLogic : XRDesignItemsLogicBase {
		Dictionary<Bar, ToolboxBarLogic> logics;
		XRZoomBarEditItem zoomItem;
		XRDesignBarManager XRDesignBarManager { get { return this.Manager as XRDesignBarManager; } }
		internal XRZoomBarEditItem ZoomItem {
			get { return zoomItem; }
			set { zoomItem = value; }
		}
		public XRDesignBarMangerLogic(BarManager manager, IServiceProvider serviceProvider)
			: base(manager, serviceProvider) {
		}
		protected override void InitZoomItem() {
			if(zoomItem != null && XRDesignPanel != null) {
				zoomItem.Init();
				if(ZoomService != null)
					zoomItem.SetZoomService(ZoomService);
			}
		}
		protected override void xrDesignPanel_SelectedTabIndexChanged(object sender, EventArgs e) {
			XRDesignPanel panel = (XRDesignPanel)sender;
			SetToolboxItemsEnabled(panel.SelectedTabIndex == TabIndices.Designer);
		}
		protected override void XRDesignPanel_Activated(object sender, EventArgs e) {
			base.XRDesignPanel_Activated(sender, e);
			XRDesignPanel panel = (XRDesignPanel)sender;
			if(logics == null) {
				logics = new Dictionary<Bar,ToolboxBarLogic>();
				ForceToolboxBars();				
				foreach(BarInfo barInfo in XRDesignBarManager.BarInfos) {
					ToolboxBarLogic logic = new ToolboxBarLogic(this.XRDesignBarManager, barInfo.Bar);
					logics.Add(barInfo.Bar, logic);
				}
			}
			ForceToolboxBarItems();
			SetToolboxItemsEnabled(panel.SelectedTabIndex == TabIndices.Designer);
		}
		IEnumerable<KeyValuePair<Bar, ToolboxBarLogic>> EnumerateLogics() {
			if(logics != null) {
				foreach(var item in logics) {
					yield return item;
				}
			}
		}
		void SetToolboxItemsEnabled(bool value) {
			foreach(KeyValuePair<Bar, ToolboxBarLogic> item in EnumerateLogics()) {
				item.Value.SetBarItemsEnabled(value);
			}
		}
		void ForceToolboxBarItems() {
			foreach(KeyValuePair<Bar, ToolboxBarLogic> item in EnumerateLogics()) {
				item.Value.ClearBarItems();
				ToolBoxBarItemsConfigurator configurator = new ToolBoxBarItemsConfigurator(XRDesignBarManager, item.Key, item.Key.Text);
				configurator.ConfigInternal();
				item.Value.ApplyBarItems();
			}
		}
		void ForceToolboxBars() {
			if(!XRDesignBarManager.Updates.Contains(XRDesignBarManagerBarNames.Toolbox)) {
				new ToolBoxBarsConfigurator(XRDesignBarManager).ConfigInternal();
				XRDesignBarManager.Updates.Add(XRDesignBarManagerBarNames.Toolbox);
			}
		}
		protected override void OnDesignerDeactivated(object sender, EventArgs e) {
			base.OnDesignerDeactivated(sender, e);
			ClearToolboxBarItems();
		}
		public void ClearToolboxBarItems() {
			foreach(var logic in EnumerateLogics()) {
				logic.Value.ClearBarItems();
			}
		}
		protected override void Manager_PressedLinkChanged(object sender, HighlightedLinkChangedEventArgs e) {
			if(e.Link == null || e.Link.Bar == null || !(e.Link.Item is BarCheckItem))
				return;
			ToolboxBarLogic logic;
			if(!logics.TryGetValue(e.Link.Bar, out logic))
				return;
			BarCheckItem barButtonItem = (BarCheckItem)e.Link.Item;
			ToolboxItem tag = null;
			if(barButtonItem == logic.CursorBarItem)
				PressCursorBarItem();
			else {
				UnpressItemsExcept(logic, barButtonItem);
				tag = barButtonItem.Tag as ToolboxItem;
			} 
			this.Manager.SelectLink(null);
			if(ToolboxService != null)
				ToolboxService.SetSelectedToolboxItem(tag);
		}
		void UnpressItemsExcept(ToolboxBarLogic logic, BarCheckItem barButtonItem) {
			foreach(var item in EnumerateLogics())
				if(item.Value != logic)
					item.Value.UnpressAllItems();
			logic.UnpressItemsExcept(barButtonItem);
		}
		protected override void PressCursorBarItem() {
			foreach(var item in EnumerateLogics())
				item.Value.UnpressItemsExcept(item.Value.CursorBarItem);
		}
	}
}
