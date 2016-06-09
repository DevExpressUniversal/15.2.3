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
using System.Drawing;
namespace DevExpress.XtraBars.Docking.Controller {
	internal interface IDockControllerInternal {
		void CreateBarDockingMenuItemCommands(BarDockingMenuItem documentListItem);
	}
	public interface IDockController {
		bool Activate(DockPanel panel);
		bool Float(DockPanel panel);
		bool Dock(DockPanel panel);
		bool DockAsTabbedDocument(DockPanel panel);
		bool AutoHide(DockPanel panel);
		bool Close(DockPanel panel);
		bool ShowContextMenu(DockPanel panel, Point point);
		bool ShowContextMenu(AutoHideContainer container, Point point);
		IEnumerable<DockControllerCommand> GetCommands(DockPanel panel);
	}
	class DockController : IDockController, IDockControllerInternal {
		DockManager managerCore;
		public DockController(DockManager manager) {
			managerCore = manager;
		}
		protected static bool Check(DockPanel panel) {
			return (panel != null) && !panel.IsDisposing && !panel.IsDisposed && (panel.Visibility != DockVisibility.Hidden);
		}
		public bool Activate(DockPanel panel) {
			if(!Check(panel)) return false;
			if(!panel.CanActivate) return false;
			Manager.ActivePanel = panel;
			return Manager.ActivePanel == panel;
		}
		public bool Float(DockPanel panel) {
			if(!Check(panel)) return false;
			if(panel.Dock == DockingStyle.Float) return false;
			if(!panel.Options.AllowFloating) return false;
			if(panel.IsMdiDocument) return false;
			if(panel.Visibility == DockVisibility.AutoHide) {
				RegisterSavedDock(panel);
				panel.Visibility = DockVisibility.Visible;
			}
			panel.MakeFloat();
			return panel.Dock == DockingStyle.Float;
		}
		IDictionary<Guid, DockingStyle> savedDocks = new Dictionary<Guid, DockingStyle>();
		internal void RegisterSavedDock(DockPanel panel) {
			savedDocks[panel.ID] = panel.SavedDock;
		}
		public bool Dock(DockPanel panel) {
			if(!Check(panel)) return false;
			if(panel.IsMdiDocument) return false;
			bool savedMdiDocument = panel.SavedMdiDocument;
			if(savedMdiDocument)
				panel.DockLayout.Restore();
			DockingStyle savedDock = panel.SavedDock;
			if(savedDock != DockingStyle.Float)
				panel.Restore();
			else {
				if(!savedMdiDocument) {
					panel.DockLayout.CheckSavedParent();
					DockingStyle dock;
					if(!savedDocks.TryGetValue(panel.ID, out dock)) {
						dock = savedDock;
						savedDocks.Remove(panel.ID);
					}
					panel.DockTo(dock);
				}
			}
			return true;
		}
		public bool DockAsTabbedDocument(DockPanel panel) {
			if(!Check(panel)) return false;
			if(panel.IsMdiDocument || !panel.Options.AllowDockAsTabbedDocument) return false;
			return panel.DockAsMdiDocument();
		}
		public bool AutoHide(DockPanel panel) {
			if(!Check(panel)) return false;
			if(panel.Visibility == DockVisibility.AutoHide) return false;
			panel.Visibility = DockVisibility.AutoHide;
			return panel.ParentAutoHideContainer != null;
		}
		public bool Close(DockPanel panel) {
			if(!Check(panel)) return false;
			panel.Close();
			return panel.Visibility == DockVisibility.Hidden;
		}
		public DockManager Manager {
			get { return managerCore; }
		}
		public bool ShowContextMenu(AutoHideContainer container, Point point) {
			if(Manager.Disposing || !Manager.IsInitialized) return false;
			if(container != null) {
				DockControllerMenu menu = CreateContextMenu();
				menu.Init(container);
				menu.PlacementTarget = container;
				return ShowContextMenuCore(menu, point);
			}
			return false;
		}
		public bool ShowContextMenu(DockPanel panel, Point point) {
			if(Manager.Disposing || !Manager.IsInitialized) return false;
			if(panel != null) {
				DockControllerMenu menu = CreateContextMenu();
				menu.Init(panel);
				menu.PlacementTarget = panel;
				return ShowContextMenuCore(menu, point);
			}
			return false;
		}
		public void CreateBarDockingMenuItemCommands(BarDockingMenuItem documentListItem) {
			if(Manager.Disposing || !Manager.IsInitialized) return;
			if(Manager.ActivePanel != null) {
				DockControllerMenu menu = CreateContextMenu();
				menu.InitBarDocumentListItem(Manager.ActivePanel, documentListItem);
			}
		}
		protected bool ShowContextMenuCore(DockControllerMenu menu, Point point) {
			bool shown = false;
			DevExpress.XtraBars.Docking2010.Ref.Dispose(ref menuCore);
			if(Manager.CanShowContextMenu(menu, point)) {
				menuCore = menu;
				DevExpress.Utils.Menu.IDXMenuManager menuManager = Manager.GetMenuManager();
				if(menuManager != null) {
					if(Manager.DocumentManager != null)
						Manager.DocumentManager.CancelDragOperation();
					Menu.CloseUp += Menu_CloseUp;
					menuManager.ShowPopupMenu(Menu, Menu.PlacementTarget, point);
					shown = true;
				}
			}
			else menu.Dispose();
			return shown;
		}
		void Menu_CloseUp(object sender, EventArgs e) {
			DockControllerMenu menu = sender as DockControllerMenu;
			if(menu != null)
				menu.CloseUp -= Menu_CloseUp;
			DevExpress.XtraBars.Docking2010.Ref.Dispose(ref menuCore);
		}
		public void CloseMenu() {
			if(Menu != null && Menu.Visible)
				Menu.Visible = false;
		}
		protected DockControllerMenu menuCore;
		public DockControllerMenu Menu {
			get { return menuCore; }
		}
		protected virtual DockControllerMenu CreateContextMenu() {
			return new DockControllerMenu(this);
		}
		public IEnumerable<DockControllerCommand> GetCommands(DockPanel panel) {
			List<DockControllerCommand> commands = new List<DockControllerCommand>();
			GetCommandsCore(panel, commands);
			commands.Sort(DockControllerCommand.Compare);
			return commands;
		}
		protected virtual void GetCommandsCore(DockPanel panel, IList<DockControllerCommand> commands) {
			commands.Add(DockControllerCommand.Float);
			if(CanAddDockCommand(panel))
				commands.Add(DockControllerCommand.Dock);
			if(CanAddDockAsTabbedDocumentCommand(panel, Manager.DocumentManager))
				commands.Add(DockControllerCommand.DockAsTabbedDocument);
			if(CanAddAutoHideCommand(panel))
				commands.Add(DockControllerCommand.AutoHide);
			if(CanAddCloseCommand(panel))
				commands.Add(DockControllerCommand.Close);
		}
		protected virtual bool CanAddDockAsTabbedDocumentCommand(DockPanel panel, Docking2010.DocumentManager documentManager) {
			if(documentManager != null && !documentManager.IsNoDocumentsStrategyInUse && documentManager.View is Docking2010.Views.Tabbed.TabbedView)
				return documentManager.View.CanDockAsTabbedDocument(panel);
			return false;
		}
		protected virtual bool CanAddCloseCommand(DockPanel panel) {
			return panel.Options.ShowCloseButton && Manager.DockingOptions.ShowCloseButton;
		}
		protected virtual bool CanAddAutoHideCommand(DockPanel panel) {
			return panel.Options.ShowAutoHideButton && Manager.DockingOptions.ShowAutoHideButton;
		}
		protected virtual bool CanAddDockCommand(DockPanel panel) {
			if(panel.DockLayout.SavedInfo.Saved) {
				if(panel.DockLayout.SavedInfo.SavedParent == null && panel.DockLayout.SavedInfo.SavedDock == DockingStyle.Fill)
					return false;
			}
			return true;
		}
	}
}
