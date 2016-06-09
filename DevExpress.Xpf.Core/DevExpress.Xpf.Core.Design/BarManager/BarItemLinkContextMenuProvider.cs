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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Core.Native;
using Platform::DevExpress.Xpf.Ribbon;
namespace DevExpress.Xpf.Core.Design {
	abstract class BarItemContextMenuProviderBase : PrimarySelectionContextMenuProvider {
		protected MenuAction Remove { get; set; }
		public BarItemContextMenuProviderBase() {
			InitializeRemoveAction();
			Items.Add(Remove);
			UpdateItemStatus += OnUpdateItemStatus;
		}
		protected virtual void InitializeRemoveAction() {
			Remove = new MenuAction("Delete");
			Remove.Execute += OnRemoveExecute;
		}
		protected virtual void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			Remove.DisplayName = "Delete";
		}
		protected abstract void OnRemoveExecute(object sender, MenuActionEventArgs e);
	}
	class BarItemContextMenuProvider : BarItemContextMenuProviderBase {
		protected override void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			base.OnUpdateItemStatus(sender, e);
			ModelItem barItem = e.Selection.PrimarySelection;
			if(!barItem.IsItemOfType(typeof(BarItem))) return;
			Remove.DisplayName = string.Format("Delete {0}", barItem.ItemType.Name);
		}
		protected override void OnRemoveExecute(object sender, MenuActionEventArgs e) {
			ModelItem barItem = e.Selection.PrimarySelection;
			if(!barItem.IsItemOfType(typeof(BarItem))) return;
			using(var scope = barItem.Parent.BeginEdit(string.Format("Remove {0}", barItem.ItemType.Name))) {
				BarManagerDesignTimeHelper.RemoveBarItem(barItem);
				scope.Complete();
			}
		}
	}
	class BarItemLinkContextMenuProvider : BarItemContextMenuProviderBase {
		MenuAction BeginGroupMenuAction { get; set; }
		public BarItemLinkContextMenuProvider() {
			BeginGroupMenuAction = new MenuAction("Begin a group");
			BeginGroupMenuAction.Execute += OnBeginGroupMenuActionExecute;
			BeginGroupMenuAction.Checkable = true;
			Items.Insert(0, BeginGroupMenuAction);
		}
		protected override void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			base.OnUpdateItemStatus(sender, e);
			ModelItem barItemLink = e.Selection.PrimarySelection;
#if SL
			Remove.Visible = false;
#else
			if(barItemLink.IsItemOfType(typeof(BarItemLink))) {
				Remove.DisplayName = string.Format("Delete {0}", barItemLink.ItemType.Name);
			}
#endif
			int idx = BarManagerDesignTimeHelper.GetIndexInCollection(barItemLink);
			BeginGroupMenuAction.Visible = idx > 0 && !IsInRibbon(barItemLink);
			if(idx > 0) {
				BeginGroupMenuAction.Checked = barItemLink.Source.Parent.Source.Collection[idx - 1].IsItemOfType(typeof(BarItemLinkSeparator));
			}
		}
		private bool IsInRibbon(ModelItem barItemLink) {
			if(BarManagerDesignTimeHelper.FindParentByType<IRibbonControl>(barItemLink) != null) return true;
			return Platform::DevExpress.Xpf.Core.Native.LayoutHelper.FindLayoutOrVisualParentObject<IRibbonControl>(barItemLink.Properties["LinkControl"].ComputedValue as BarItemLinkControlBase) != null;
		}
		protected override void OnRemoveExecute(object sender, MenuActionEventArgs e) {
			ModelItem barItemLink = e.Selection.PrimarySelection;
			ModelItem parent = barItemLink.Parent;
			using(ModelEditingScope scope = parent.BeginEdit(string.Format("Remove {0}", barItemLink.ItemType.Name))) {
				BarManagerDesignTimeHelper.RemoveBarItemLink(barItemLink);
				scope.Complete();
			}
		}
		protected void OnBeginGroupMenuActionExecute(object sender, MenuActionEventArgs e) {
			ModelItem selectedLink = e.Selection.PrimarySelection;
			if(selectedLink == null || !selectedLink.IsItemOfType(typeof(BarItemLink))) return;
			if(((MenuAction)sender).Checked) {
				AddSeparator(selectedLink);
			} else RemoveSeparator(selectedLink);
		}
		private void AddSeparator(ModelItem selectedLink) {
			int idx = BarManagerDesignTimeHelper.GetIndexInCollection(selectedLink);
			ModelItem separator = ModelFactory.CreateItem(selectedLink.Context, typeof(BarItemLinkSeparator), CreateOptions.None);
			selectedLink.Source.Parent.Source.Collection.Insert(idx, separator);
			SelectionOperations.Select(selectedLink.Context, selectedLink);
		}
		private void RemoveSeparator(ModelItem selectedLink) {
			int idx = BarManagerDesignTimeHelper.GetIndexInCollection(selectedLink) - 1;
			selectedLink.Source.Parent.Source.Collection.RemoveAt(idx);
			SelectionOperations.SelectOnly(selectedLink.Context, selectedLink);
		}
	}
}
