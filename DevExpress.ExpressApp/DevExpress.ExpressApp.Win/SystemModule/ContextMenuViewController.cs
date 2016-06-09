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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public interface IContextMenuHolder {
		PopupMenu ContextMenu { get; }
	}
	public interface IContextMenuTarget {
		void SetMenuManager(IDXMenuManager menuManager);
		bool CanShowContextMenu(Point position);
		Control ContextMenuSite { get; }
		bool ContextMenuEnabled { get; }
		event EventHandler ContextMenuEnabledChanged;
	}
	public class ContextMenuViewController : ViewController<ListView> {
		private bool isContextMenuAttached;
		private PopupMenu contextMenu;
		private BarManager barManager;
		private IContextMenuTarget contextMenuTarget;
		private void Frame_TemplateChanged(object sender, EventArgs e) {
			OnTemplateChanged();
		}
		private void OnTemplateChanged() {
			if(Frame.Template is IContextMenuHolder) {
				contextMenu = ((IContextMenuHolder)Frame.Template).ContextMenu;
				if(contextMenu == null) {
					string message = string.Format("Cannot initialize the Controller because the 'ContextMenu' property of the '{0}' template is null", Frame.Template);
					throw new InvalidOperationException(message);
				}
				barManager = contextMenu.Manager;
				if(barManager == null) {
					string message = string.Format("Cannot initialize the Controller because the 'Manager' property of the '{0}' template's ContextMenu is null", Frame.Template);
					throw new InvalidOperationException(message);
				}
				View.ControlsCreated += View_ControlsCreated;
				View.EditorChanging += View_EditorChanging;
				if(View.IsControlCreated) {
					AttachToContextMenu();
				}
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			AttachToContextMenu();
		}
		private void View_EditorChanging(object sender, EventArgs e) {
			DetachFromContextMenu();
		}
		private void AttachToContextMenu() {
			if(isContextMenuAttached) {
				string message = string.Format("Cannot attach the Controller to the '{0}' context menu because this Controller was already attached.", contextMenu);
				throw new InvalidOperationException(message);
			}
			contextMenuTarget = View.Editor as IContextMenuTarget;
			if(contextMenuTarget != null) {
				contextMenuTarget.ContextMenuEnabledChanged += contextMenuTarget_ContextMenuEnabledChanged;
				contextMenuTarget.SetMenuManager(barManager);
				barManager.QueryShowPopupMenu += barManager_QueryShowPopupMenu;
				if(contextMenuTarget.ContextMenuEnabled) {
					barManager.SetPopupContextMenu(GetContextMenuSite(), contextMenu);
				}
				isContextMenuAttached = true;
			}
		}
		private void DetachFromContextMenu() {
			if(contextMenuTarget != null) {
				contextMenuTarget.ContextMenuEnabledChanged -= contextMenuTarget_ContextMenuEnabledChanged;
				barManager.QueryShowPopupMenu -= barManager_QueryShowPopupMenu;
				if (contextMenuTarget.ContextMenuSite != null) {
					contextMenuTarget.SetMenuManager(null);
					barManager.SetPopupContextMenu(GetContextMenuSite(), null);
				}
				contextMenuTarget = null;
				isContextMenuAttached = false;
			}
		}
		private void barManager_QueryShowPopupMenu(object sender, QueryShowPopupMenuEventArgs e) {
			if(!e.Cancel && e.Menu == contextMenu) {
				bool canShowContextMenu = (GetContextMenuSite() == e.Control && contextMenuTarget.CanShowContextMenu(e.Position));
				e.Cancel = !canShowContextMenu;
			}
		}
		private void contextMenuTarget_ContextMenuEnabledChanged(object sender, EventArgs e) {
			if(contextMenuTarget.ContextMenuEnabled) {
				barManager.SetPopupContextMenu(GetContextMenuSite(), contextMenu);
			}
			else {
				barManager.SetPopupContextMenu(GetContextMenuSite(), null);
			}
		}
		private Control GetContextMenuSite() {
			if(contextMenuTarget.ContextMenuSite == null) {
				throw new InvalidOperationException("Cannot get 'ContextMenuSite' because it is null");
			}
			return contextMenuTarget.ContextMenuSite;
		}
		protected override void OnActivated() {
			base.OnActivated();
			Frame.TemplateChanged += Frame_TemplateChanged;
			if(Frame.Template != null) {
				OnTemplateChanged();
			}
		}
		protected override void OnDeactivated() {
			Frame.TemplateChanged -= Frame_TemplateChanged;
			View.ControlsCreated -= View_ControlsCreated;
			View.EditorChanging -= View_EditorChanging;
			DetachFromContextMenu();
			barManager = null;
			contextMenu = null;
			base.OnDeactivated();
		}
	}
}
