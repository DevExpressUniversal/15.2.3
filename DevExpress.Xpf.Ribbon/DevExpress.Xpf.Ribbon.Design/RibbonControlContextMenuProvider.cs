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

extern alias Platform;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Model;
using System;
namespace DevExpress.Xpf.Ribbon.Design {
	abstract class RibbonContextMenuProviderBase : PrimarySelectionContextMenuProvider {
		protected MenuAction Delete { get; private set; }
		protected Action<ModelItem> DeleteAction { get; set; }
		public RibbonContextMenuProviderBase() {
			UpdateItemStatus += OnUpdateItemStatus;
			InitializeMenuItems();
			AddMenuItems();
		}
		protected virtual void AddMenuItems() {
			Items.Add(Delete);
		}
		protected virtual void InitializeMenuItems() {
			Delete = new MenuAction(string.Empty);
			Delete.Execute += OnDeleteExecute;
		}
		protected virtual void OnDeleteExecute(object sender, MenuActionEventArgs e) {
			if(DeleteAction != null && e.Selection.PrimarySelection != null)
				DeleteAction(e.Selection.PrimarySelection);
		}
		protected virtual void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			if(e.Selection.PrimarySelection == null) {
				Delete.Visible = false;
				return;
			}
			Delete.Visible = true;
			Delete.DisplayName = string.Format("Delete {0}", e.Selection.PrimarySelection.ItemType.Name);
		}
	}
	class RibbonControlContextMenuProvider : RibbonContextMenuProviderBase {
		MenuAction AddRibbonPageCategory { get; set; }
		MenuAction AddRibbonPage { get; set; }
		public RibbonControlContextMenuProvider() { }
		protected override void AddMenuItems() {
			Items.Add(AddRibbonPageCategory);
			Items.Add(AddRibbonPage);
		}
		protected override void InitializeMenuItems() {
			base.InitializeMenuItems();
			AddRibbonPageCategory = new MenuAction("Add RibbonPageCategory");
			AddRibbonPageCategory.Execute += OnAddRibbonPageCategoryExecute;
			AddRibbonPage = new MenuAction("Add RibbonPage");
			AddRibbonPage.Execute += OnAddRibbonPageExecute;
		}
		void OnAddRibbonPageCategoryExecute(object sender, MenuActionEventArgs e) {
			ModelItem ribbonControl = RibbonDesignTimeHelper.FindRibbonCotnrol(e.Selection.PrimarySelection);
			if(ribbonControl == null) return;
			RibbonDesignTimeHelper.AddRibbonPageCategory(ribbonControl);
		}
		void OnAddRibbonPageExecute(object sender, MenuActionEventArgs e) {
			ModelItem ribbon = e.Selection.PrimarySelection;
			using(var scope = ribbon.BeginEdit()) {
				ModelItem pageCategory = RibbonDesignTimeHelper.GetDefaultPageCategory(ribbon);
				if(pageCategory == null) {
					RibbonDesignTimeHelper.AddRibbonPageCategory(ribbon);
				} else if(pageCategory.IsItemOfType(typeof(RibbonPageCategoryBase)))
					RibbonDesignTimeHelper.AddRibbonPage(pageCategory);
				scope.Complete();
			}
		}
	}
	class RibbonPageContextMenuProvider : RibbonContextMenuProviderBase {
		MenuAction AddPageGroupAction { get; set; }
		public RibbonPageContextMenuProvider() {
			DeleteAction = RibbonDesignTimeHelper.RemoveRibbonPage;
		}
		protected override void InitializeMenuItems() {
			base.InitializeMenuItems();
			AddPageGroupAction = new MenuAction("Add RibbonPageGroup");
			AddPageGroupAction.Execute += OnAddPageGroupAction;
		}
		protected override void AddMenuItems() {
			Items.Add(AddPageGroupAction);
			base.AddMenuItems();
		}
		void OnAddPageGroupAction(object sender, MenuActionEventArgs e) {
			ModelItem page = e.Selection.PrimarySelection;
			RibbonDesignTimeHelper.AddRibbonPageGroup(page);
		}
	}
	class RibbonPageCategoryContextMenuProvider : RibbonContextMenuProviderBase {
		MenuAction AddRibbonPage { get; set; }
		public RibbonPageCategoryContextMenuProvider() {
			DeleteAction = RibbonDesignTimeHelper.RemoveRibbonPageCategory;
		}
		protected override void InitializeMenuItems() {
			base.InitializeMenuItems();
			AddRibbonPage = new MenuAction("Add RibbonPage");
			AddRibbonPage.Execute += OnAddRibbonPageExecute;
		}
		protected override void AddMenuItems() {
			Items.Add(AddRibbonPage);
			base.AddMenuItems();
		}
		void OnAddRibbonPageExecute(object sender, MenuActionEventArgs e) {
			ModelItem category = e.Selection.PrimarySelection;
			RibbonDesignTimeHelper.AddRibbonPage(category);
		}
	}
}
