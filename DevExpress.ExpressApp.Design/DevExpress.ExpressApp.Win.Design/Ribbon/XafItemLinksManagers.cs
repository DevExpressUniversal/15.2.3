#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using DevExpress.XtraBars;
using DevExpress.XtraBars.Design.Frames;
using DevExpress.XtraBars.Ribbon.Design;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.ViewInfo;
namespace DevExpress.ExpressApp.Win.Design.Ribbon {
	internal static class XafItemLinksManagersHelper {
		public static void AddInplaceLinkContainerItem(ItemLinksBaseToolbar itemsToolbar) {
			if(itemsToolbar.Manager != null) {
				BarItemInfo info = new BarItemInfo("BarLinkContainerExItem", "Inplace Link Container", 4, typeof(BarLinkContainerExItem), typeof(BarLinkContainerExItemLink), typeof(BarLinkContainerLinkViewInfo), null, true, true);
				PopupMenu addItemMenu = itemsToolbar.AddItemButton.DropDownControl as PopupMenu;
				if(addItemMenu != null) {
					addItemMenu.ItemLinks.Add(itemsToolbar.CreateMenuButtonItem(info, itemsToolbar.ItemTypeImages));
				}
			}
		}
	}
	public class XafRibbonItemsManagerBase : RibbonItemsManagerBase {
		protected override void InitializeToolbar() {
			XafItemLinksManagersHelper.AddInplaceLinkContainerItem(ItemsToolbar);
			base.InitializeToolbar();
		}
	}
	public class XafQuickAccessToolbarManager : QuickAccessToolbarManager {
		protected override void InitializeToolbar() {
			XafItemLinksManagersHelper.AddInplaceLinkContainerItem(ItemsToolbar);
			base.InitializeToolbar();
		}
	}
	public class XafPageHeaderItemsManager : PageHeaderItemsManager {
		protected override void InitializeToolbar() {
			XafItemLinksManagersHelper.AddInplaceLinkContainerItem(ItemsToolbar);
			base.InitializeToolbar();
		}
	}
	public class XafStatusBarManager : StatusBarManager {
		protected override void InitializeToolbar() {
			XafItemLinksManagersHelper.AddInplaceLinkContainerItem(ItemsToolbar);
			base.InitializeToolbar();
		}
	}
}
