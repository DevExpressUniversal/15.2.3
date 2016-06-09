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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Controls {
	public interface IDXPopupMenuHolder {
		Control PopupSite { get; }
		bool CanShowPopupMenu(Point position);
		void SetMenuManager(IDXMenuManager manager);
	}
	public interface IBarManagerHolder {
		BarManager BarManager { get; }
		event EventHandler BarManagerChanged;
	}
	[ToolboxItem(false)]
	public class ActionsDXPopupMenu : PopupMenu, IContextMenuTemplate {
		private IBarManagerHolder barManagerHolder;
		private IDXPopupMenuHolder popupMenuHolder;
		private ICollection<ActionContainerBarItem> containers;
		private ICollection<IActionContainer> contextContainers;
		private void Manager_QueryShowPopupMenu(object sender, QueryShowPopupMenuEventArgs e) {
			if(popupMenuHolder.PopupSite == e.Control && !e.Cancel) {
				e.Cancel = !popupMenuHolder.CanShowPopupMenu(e.Position);
			}
		}
		private void barManagerHolder_BarManagerChanged(object sender, EventArgs e) {
			RecreateActionItems();
		}
		private void UnsubscribeBarManagerHolder() {
			if(barManagerHolder != null) {
				barManagerHolder.BarManagerChanged -= barManagerHolder_BarManagerChanged;
			}
		}
		private void RecreateActionItems() {
			DisposeCreatedContainers();
			ItemLinks.Clear();
			if(Manager == null || Manager != barManagerHolder.BarManager) {
				BarManager barManager;
				if(barManagerHolder.BarManager != null) {
					barManager = barManagerHolder.BarManager;
				}
				else {
					barManager = new BarManager();
					Control control = barManagerHolder as Control;
					if(control != null) {
						Form form = control.FindForm();
						if(form != null) {
							barManager.BeginInit();
							barManager.Form = form;
							barManager.EndInit();
						}
					}
				}
				BeginInit();
				Manager = barManager;
				Name = "ActionsPopupMenu";
				EndInit();
			}
			popupMenuHolder.SetMenuManager(Manager);
			SetupPopupContextMenuSite();
			foreach(IActionContainer sourceContainer in contextContainers) {
				ActionContainerBarItem barActionContainer = new ActionContainerBarItem();
				barActionContainer.Caption = sourceContainer.ContainerId + "_Popup";
				barActionContainer.ContainerId = sourceContainer.ContainerId;
				barActionContainer.Manager = Manager;
				containers.Add(barActionContainer);
				Manager.Items.Add(barActionContainer);
				foreach(ActionBase action in sourceContainer.Actions) {
					barActionContainer.Register(action);
				}
				AddItem(barActionContainer);
			}
		}
		private void DisposeCreatedContainers() {
			foreach(ActionContainerBarItem container in containers) {
				if(Manager != null) {
					if(Manager.Items.Contains(container)) {
						container.ItemLinks.Clear();
						Manager.Items.Remove(container);
					}
				}
				container.Dispose();
			}
			containers.Clear();
		}
		protected void RaiseBoundItemCreating() {
			if(BoundItemCreating != null) {
				BoundItemCreating(this, new BoundItemCreatingEventArgs(null, null));
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					DisposeCreatedContainers();
					if(Manager != null) {
						Manager.QueryShowPopupMenu -= Manager_QueryShowPopupMenu;
						ResetPopupContextMenuSite();
						popupMenuHolder.SetMenuManager(null);
					}
					popupMenuHolder = null;
					UnsubscribeBarManagerHolder();
					barManagerHolder = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public ActionsDXPopupMenu() {
			containers = new List<ActionContainerBarItem>();
		}
		public void CreateActionItems(IFrameTemplate parent, ListView context, ICollection<IActionContainer> contextContainers) {
			IBarManagerHolder barManagerHolder = parent as IBarManagerHolder;
			if(barManagerHolder != null) {
				Guard.ArgumentNotNull(context, "context");
				IDXPopupMenuHolder popupMenuHolder = context.Editor as IDXPopupMenuHolder;
				if(popupMenuHolder == null) {
					throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ListEditorShouldImplement));
				}
				CreateActionItems(barManagerHolder, popupMenuHolder, contextContainers);
			}
		}
		public void CreateActionItems(IBarManagerHolder barManagerHolder, IDXPopupMenuHolder popupMenuHolder, ICollection<IActionContainer> contextContainers) {
			Guard.ArgumentNotNull(barManagerHolder, "barManagerHolder");
			Guard.ArgumentNotNull(popupMenuHolder, "popupMenuHolder");
			Guard.ArgumentNotNull(contextContainers, "contextContainers");
			UnsubscribeBarManagerHolder();
			this.barManagerHolder = barManagerHolder;
			this.barManagerHolder.BarManagerChanged += barManagerHolder_BarManagerChanged;
			this.popupMenuHolder = popupMenuHolder;
			this.contextContainers = contextContainers;
			RecreateActionItems();
		}
		public void SetupPopupContextMenuSite() {
			if(Manager != null && popupMenuHolder != null && popupMenuHolder.PopupSite != null) {
				Manager.SetPopupContextMenu(popupMenuHolder.PopupSite, this);
			}
		}
		public void ResetPopupContextMenuSite() {
			if(Manager != null && popupMenuHolder != null && popupMenuHolder.PopupSite != null) {
				Manager.SetPopupContextMenu(popupMenuHolder.PopupSite, null);
			}
		}
		public override BarManager Manager {
			get { return base.Manager; }
			set {
				if(Manager != null) {
					Manager.QueryShowPopupMenu -= Manager_QueryShowPopupMenu;
				}
				base.Manager = value;
				if(Manager != null) {
					Manager.QueryShowPopupMenu += Manager_QueryShowPopupMenu;
				}
			}
		}
		public event EventHandler<BoundItemCreatingEventArgs> BoundItemCreating;
		#region Obsolete 14.2
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void SetHolder(IDXPopupMenuHolder holder) {
			this.popupMenuHolder = holder;
		}
		#endregion
	}
}
