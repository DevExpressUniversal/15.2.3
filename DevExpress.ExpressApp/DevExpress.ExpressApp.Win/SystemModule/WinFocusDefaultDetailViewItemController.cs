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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraLayout;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class WinFocusDefaultDetailViewItemController : FocusDefaultDetailViewItemController {
		private Dictionary<string, LayoutGroup> layoutGroupsInTabbedGroup = new Dictionary<string, LayoutGroup>();
		private void WinLayoutManager_ItemCreated(object sender, ItemCreatedEventArgs e) {
			if(e.ModelLayoutElement != null && e.ModelLayoutElement.Parent is IModelTabbedGroup && e.ModelLayoutElement is IModelLayoutGroup) {
				layoutGroupsInTabbedGroup.Add(GetFullPathNode(e.ModelLayoutElement), (LayoutGroup)e.Item);
			}
		}
		private void WinFocusDefaultDetailViewItemController_Shown(object sender, EventArgs e) {
			FocusDefaultItemControl();
		}
		private void WinLayoutManager_LayoutInfoApplied(object sender, EventArgs e) {
			if(NeedActivateDefaultTab()) {
				foreach(IModelLayoutGroup currentNode in tabPageNodesWithDefaultItemList) {
					LayoutGroup currentGroup = layoutGroupsInTabbedGroup[GetFullPathNode(currentNode)];
					if(currentGroup != null) {
						currentGroup.ParentTabbedGroup.SelectedTabPage = currentGroup;
					}
				}
			}
		}
		protected override void FocusDefaultItemControlCore() {
			Control defaultItemControl = defaultItem.Control as Control;
			if(defaultItemControl != null) {
				defaultItemControl.Focus();
			}
		}
		protected override void OnViewControlsCreating() {
			layoutGroupsInTabbedGroup.Clear();
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Window != null) {
				Window.ViewChanged += new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
				if(Window.Template is Form) {
					((Form)Window.Template).Shown += new EventHandler(WinFocusDefaultDetailViewItemController_Shown);
				}
			}
			if(WinLayoutManager != null) {
				WinLayoutManager.ItemCreated += new EventHandler<ItemCreatedEventArgs>(WinLayoutManager_ItemCreated);
				WinLayoutManager.LayoutInfoApplied += new EventHandler(WinLayoutManager_LayoutInfoApplied);
			}
		}
		private void Window_ViewChanged(object sender, ViewChangedEventArgs e) {
			if(Active && View.IsRoot && Frame.Template is Form) {
				if(((Form)Window.Template).IsHandleCreated) {
					FocusDefaultItemControl();
				}
			}
		}
		protected override void OnDeactivated() {
			if(Window != null) {
				Window.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
				if(Window.Template is Form) {
					((Form)Window.Template).Shown -= new EventHandler(WinFocusDefaultDetailViewItemController_Shown);
				}
			}
			if(WinLayoutManager != null) {
				WinLayoutManager.ItemCreated -= new EventHandler<ItemCreatedEventArgs>(WinLayoutManager_ItemCreated);
				WinLayoutManager.LayoutInfoApplied -= new EventHandler(WinLayoutManager_LayoutInfoApplied);
			}
			base.OnDeactivated();
		}
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public WinLayoutManager WinLayoutManager {
			get { return View.LayoutManager as WinLayoutManager; }
		}
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public Window Window {
			get { return Frame as Window; }
		}
	}
}
