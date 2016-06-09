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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraBars.Commands.Design {
	#region RibbonStatusBarGenerationManager<TControl, TCommandId> (abstract class)
	public abstract class RibbonStatusBarGenerationManager<TControl, TCommandId> : RibbonGenerationManager<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		protected RibbonStatusBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<TControl, TCommandId> barController)
			: base(creator, container, barController) {
		}
		protected override Component CreateBarItemGroup(ControlCommandBarCreator creator) {
			RibbonStatusBar statusBar = RibbonControl.StatusBar;
			if (statusBar != null)
				return statusBar;
			statusBar = GenerationStrategy.CreateComponent(typeof(RibbonStatusBar)) as RibbonStatusBar;
			if (statusBar == null)
				return null;
			GenerationStrategy.OnComponentChanging(statusBar, null);
			statusBar.Ribbon = RibbonControl;
			GenerationStrategy.OnComponentChanged(statusBar, null, null, null);
			GenerationStrategy.OnComponentChanging(RibbonControl, null);
			RibbonControl.StatusBar = statusBar;
			GenerationStrategy.OnComponentChanged(RibbonControl, null, null, null);
			Control parent = RibbonControl.Parent;
			if (parent != null) {
				GenerationStrategy.OnComponentChanging(parent, null);
				parent.Controls.Add(statusBar);
				GenerationStrategy.OnComponentChanged(parent, null, null, null);
			}
			RibbonForm form = RibbonControl.FindForm() as RibbonForm;
			if (form != null && form.Ribbon == RibbonControl) {
				GenerationStrategy.OnComponentChanging(form, null);
				form.StatusBar = statusBar;
				GenerationStrategy.OnComponentChanged(form, null, null, null);
			}
			return statusBar;
		}
		protected override void InitializeBarItemGroup(Component barItemGroup, ControlCommandBarControllerBase<TControl, TCommandId> barController) {
		}
		protected override void AddItemsToBarItemGroup(Component barItemGroup, List<BarItem> items) {
			RibbonStatusBar statusBar = barItemGroup as RibbonStatusBar;
			foreach (BarItem item in items) {
				AddItemToStatusBar(statusBar, item);
			}
		}
		protected virtual void AddItemToStatusBar(RibbonStatusBar statusBar, BarItem item) {
			IBarButtonGroupMember buttonGroupMember = item as IBarButtonGroupMember;
			if (buttonGroupMember != null) {
				BarButtonGroup buttonGroup = FindButtonGroupByTag(statusBar, buttonGroupMember.ButtonGroupTag);
				if (buttonGroup != null)
					AddItemLink(buttonGroup.ItemLinks, item);
				else
					AddItemLink(statusBar.ItemLinks, item);
			}
			else
				AddItemLink(statusBar.ItemLinks, item);
			IBarSubItem subItem = item as IBarSubItem;
			if (subItem != null) {
				List<BarItem> subItems = subItem.GetSubItems();
				AddBarItems(RibbonControl.Items, subItems.ToArray());
			}
		}
		BarButtonGroup FindButtonGroupByTag(RibbonStatusBar statusBar, object tag) {
			BarItemLinkCollection links = statusBar.ItemLinks;
			int count = links.Count;
			for (int i = 0; i < count; i++) {
				BarButtonGroup item = links[i].Item as BarButtonGroup;
				if (item != null && Object.Equals(item.Tag, tag))
					return item;
			}
			return CreateButtonGroup(statusBar, tag);
		}
		BarButtonGroup CreateButtonGroup(RibbonStatusBar statusBar, object tag) {
			BarButtonGroup buttonGroup = new BarButtonGroup();
			buttonGroup.Tag = tag;
			statusBar.ItemLinks.Add(buttonGroup);
			GenerationStrategy.AddToContainer(buttonGroup);
			return buttonGroup;
		}
		protected override void FilterMergedItems(List<BarItem> mergedItems, Component barItemGroup) {
		}
		protected internal override void SetupItemLink(ICommandBarItem barItem, BarItemLink link) {
			barItem.SetupStatusBarLink(link);
		}
	}
	#endregion
}
