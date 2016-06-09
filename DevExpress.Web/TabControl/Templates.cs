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
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.Web {
	public class TabControlTemplateContainerBase : ItemTemplateContainerBase {
		private bool active;
		private TabBase tab;
#if !SL
	[DevExpressWebLocalizedDescription("TabControlTemplateContainerBaseActive")]
#endif
		public bool Active {
			get { return active; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("TabControlTemplateContainerBaseTabBase")]
#endif
		public TabBase TabBase {
			get { return tab; }
		}
		public TabControlTemplateContainerBase(TabBase tab, bool active)
			: base(tab.Index, tab) {
			this.active = active;
			this.tab = tab;
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if(e is CommandEventArgs) {
				RaiseBubbleEvent(this, new TabControlCommandEventArgs(TabBase, source, e as CommandEventArgs));
				return true;
			}
			return false;
		}
	}
	public class TabControlTemplateContainer : TabControlTemplateContainerBase {
#if !SL
	[DevExpressWebLocalizedDescription("TabControlTemplateContainerTab")]
#endif
		public Tab Tab {
			get { return TabBase as Tab; }
		}
		public TabControlTemplateContainer(Tab tab, bool active)
			: base(tab, active) {
		}
		protected override object GetDataItem() {
			return Tab.DataItem;
		}
	}
	public class PageControlTemplateContainer : TabControlTemplateContainerBase {
#if !SL
	[DevExpressWebLocalizedDescription("PageControlTemplateContainerTabPage")]
#endif
		public TabPage TabPage {
			get { return TabBase as TabPage; }
		}
		public PageControlTemplateContainer(TabPage tabPage, bool active)
			: base(tabPage, active) {
		}
	}
	public class TabsSpaceTemplateContainer : TemplateContainerBase {
		TabSpaceTemplatePosition position;
		public TabsSpaceTemplateContainer(ASPxTabControlBase tabControl, TabSpaceTemplatePosition position)
			: base(0, tabControl) {
			Position = position;
		}
#if !SL
	[DevExpressWebLocalizedDescription("TabsSpaceTemplateContainerTabControl")]
#endif
		public ASPxTabControlBase TabControl {
			get { return DataItem as ASPxTabControlBase; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("TabsSpaceTemplateContainerPosition")]
#endif
		public TabSpaceTemplatePosition Position {
			get { return position; }
			set { position = value; }
		}
	}
}
