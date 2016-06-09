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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class MenuStripItemFactory {
		private static T CreateItem<T>(ActionBase action) where T : ToolStripMenuItem, new() {
			T result = new T();
			result.Text = action.Caption;
			result.ShortcutKeyDisplayString = action.Shortcut;
			if(action is SimpleAction) {
				result.Click += new EventHandler(
					delegate(object sender, EventArgs args) {
						try {
							((SimpleAction)action).DoExecute();
						}
						catch { }
					});
			}
			result.Image = ImageLoader.Instance.GetImageInfo(action.ImageName).Image;
			result.Enabled = action.Enabled;
			result.Visible = action.Active;
			return result;
		}
		private struct MenuItemActionItemLink {
			public SingleChoiceAction action;
			public ChoiceActionItem actionItem;
		}
		private static T CreateItem<T>(SingleChoiceAction action, ChoiceActionItem actionItem) where T : ToolStripItem, new() {
			T result = new T();
			result.Text = actionItem.Caption;
			MenuItemActionItemLink link = new MenuItemActionItemLink();
			link.action = action;
			link.actionItem = actionItem;
			result.Click += new EventHandler(
				delegate(object sender, EventArgs args) {
					try {
						link.action.DoExecute(link.actionItem);
					}
					catch { }
				});
			result.Image = ImageLoader.Instance.GetImageInfo(actionItem.ImageName).Image;
			result.Tag = actionItem;
			result.Enabled = actionItem.Enabled;
			result.Visible = actionItem.Active;
			return result;
		}
		private static void CreateLinks(SingleChoiceAction action, ToolStripItemCollection menuItems, ChoiceActionItemCollection links) {
			foreach(ChoiceActionItem item in links) {
				ToolStripMenuItem currentMenuItem = CreateItem<ToolStripMenuItem>(action, item);
				menuItems.Add(currentMenuItem);
				CreateLinks(action, currentMenuItem.DropDownItems, item.Items);
			}
		}
		public static void BuildMenu(ContextMenuStrip contextMenu, IEnumerable<ActionBase> actions) {
			contextMenu.Items.Clear();
			foreach(ActionBase action in actions) {
				if(!(action is SingleChoiceAction)) {
					contextMenu.Items.Add(CreateItem<ToolStripMenuItem>(action));
				} else {
					ToolStripMenuItem menuItem = CreateItem<ToolStripMenuItem>(action);
					contextMenu.Items.Add(menuItem);
					CreateLinks((SingleChoiceAction)action, menuItem.DropDownItems, ((SingleChoiceAction)action).Items);
				}
			}
		}
	}
}
