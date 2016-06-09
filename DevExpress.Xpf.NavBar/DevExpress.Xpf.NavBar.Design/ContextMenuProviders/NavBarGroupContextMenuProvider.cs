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
using Microsoft.Windows.Design.Model;
#if SL
using Platform::DevExpress.Xpf.NavBar;
#endif
namespace DevExpress.Xpf.NavBar.Design {
	class NavBarGroupContextMenuProvider : PrimarySelectionContextMenuProvider {
		MenuAction AddNavBarItemMenuAction { get; set; }
		MenuAction ContentDisplaySourceAction { get; set; }
		MenuAction ItemsDisplaySourceAction { get; set; }
		MenuGroup SetDisplaySourceGroup { get; set; }
		public NavBarGroupContextMenuProvider() {
			InitializeAddNavBarItemMenu();
			InitializeDisplaySourceMenu();
			UpdateItemStatus += OnUpdateItemStatus;
		}
		void InitializeAddNavBarItemMenu() {
			AddNavBarItemMenuAction = new MenuAction("Add NavBarItem");
			AddNavBarItemMenuAction.Execute += OnAddNavBarItemMenuActionExecute;
			Items.Add(AddNavBarItemMenuAction);
		}
		void OnAddNavBarItemMenuActionExecute(object sender, MenuActionEventArgs e) {
			ModelItem group = e.Selection.PrimarySelection;
			if(group == null) return;
			NavBarDesignTimeHelper.AddNewNavBarItem(group);
		}
		void InitializeDisplaySourceMenu() {
			SetDisplaySourceGroup = new MenuGroup("Set DisplaySource") { HasDropDown = true };
			ContentDisplaySourceAction = new MenuAction("Content") { Checkable = true };
			ContentDisplaySourceAction.Execute += OnSetDisplaySourceExecute;
			ItemsDisplaySourceAction = new MenuAction("Items") { Checkable = true };
			ItemsDisplaySourceAction.Execute += OnSetDisplaySourceExecute;
			SetDisplaySourceGroup.Items.Add(ContentDisplaySourceAction);
			SetDisplaySourceGroup.Items.Add(ItemsDisplaySourceAction);
			Items.Add(SetDisplaySourceGroup);
		}
		void OnSetDisplaySourceExecute(object sender, MenuActionEventArgs e) {
			ModelItem group = e.Selection.PrimarySelection;
			if(group == null) return;
			MenuAction menuAction = (MenuAction)sender;
			DisplaySource value;
			if(menuAction.Equals(ContentDisplaySourceAction))
				value = DisplaySource.Content;
			else if(menuAction.Equals(ItemsDisplaySourceAction))
				value = DisplaySource.Items;
			else return;
			using(ModelEditingScope scope = group.BeginEdit("Set DisplaySource")) {
				group.Properties["DisplaySource"].SetValue(value);
				scope.Complete();
			}
		}
		void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			UpdateDisplaySourceMenuActions(e.Selection.PrimarySelection);
		}
		void UpdateDisplaySourceMenuActions(ModelItem navBarGroup) {
			if(navBarGroup == null) return;
			DisplaySource displaySource = (DisplaySource)navBarGroup.Properties["DisplaySource"].ComputedValue;
			ContentDisplaySourceAction.Checked = displaySource == DisplaySource.Content;
			ItemsDisplaySourceAction.Checked = displaySource == DisplaySource.Items;
		}
	}
}
