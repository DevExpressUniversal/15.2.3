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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking.Helpers;
namespace DevExpress.XtraBars.Docking {
	public delegate void CreateResizeZoneEventHandler(object sender, CreateResizeZoneEventArgs e);
	public delegate void ActivePanelChangedEventHandler(object sender, ActivePanelChangedEventArgs e);
	public delegate void DockPanelEventHandler(object sender, DockPanelEventArgs e);
	public delegate void DockPanelCancelEventHandler(object sender, DockPanelCancelEventArgs e);
	public delegate void VisibilityChangedEventHandler(object sender, VisibilityChangedEventArgs e);
	public delegate void TabsPositionChangedEventHandler(object sender, TabsPositionChangedEventArgs e);
	public delegate void DockingEventHandler(object sender, DockingEventArgs e);
	public delegate void EndDockingEventHandler(object sender, EndDockingEventArgs e);
	public delegate void StartSizingEventHandler(object sender, StartSizingEventArgs e);
	public delegate void SizingEventHandler(object sender, SizingEventArgs e);
	public delegate void EndSizingEventHandler(object sender, EndSizingEventArgs e);
	public delegate void AutoHideContainerEventHandler(object sender, AutoHideContainerEventArgs e);
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	public class CreateResizeZoneEventArgs: DockPanelCancelEventArgs{
		ResizeDirection directionCore = ResizeDirection.None;
		public CreateResizeZoneEventArgs(DockPanel panel,ResizeDirection direction): base(panel){
			directionCore = direction;
		}
		public ResizeDirection Direction { get { return directionCore;} }
	}
	public class DockPanelEventArgs : EventArgs {
		DockPanel panel;
		public DockPanelEventArgs(DockPanel panel) {
			this.panel = panel;
		}
		public DockPanel Panel { get { return panel; } }
	}
	public class ActivePanelChangedEventArgs : DockPanelEventArgs {
		DockPanel oldPanel;
		public ActivePanelChangedEventArgs(DockPanel panel, DockPanel oldPanel) : base(panel) {
			this.oldPanel = oldPanel;
		}
		public DockPanel OldPanel { get { return oldPanel; } }
	}
	public class DockPanelCancelEventArgs : DockPanelEventArgs {
		bool cancel;
		public DockPanelCancelEventArgs(DockPanel panel) : base(panel) {
			this.cancel = false;
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public class VisibilityChangedEventArgs : DockPanelEventArgs {
		DockVisibility oldVisibility;
		public VisibilityChangedEventArgs(DockPanel panel, DockVisibility oldVisibility) : base(panel) {
			this.oldVisibility = oldVisibility;
		}
		public DockVisibility OldVisibility { get { return oldVisibility; } }
		public DockVisibility Visibility { get { return Panel.Visibility; } }
	}
	public class TabsPositionChangedEventArgs : DockPanelEventArgs {
		TabsPosition oldTabsPosition;
		public TabsPositionChangedEventArgs(DockPanel panel, TabsPosition oldTabsPosition) : base(panel) {
			this.oldTabsPosition = oldTabsPosition;
		}
		public TabsPosition OldTabsPosition { get { return oldTabsPosition; } }
		public TabsPosition TabsPosition { get { return Panel.TabsPosition; } }
	}
	public class DockingEventArgs : DockPanelCancelEventArgs {
		Control target;
		DockingStyle dock;
		bool targetTabbed;
		Point dragPoint;
		int index;
		public DockingEventArgs(DockPanel panel, Control target, Point dragPoint, DockingStyle dock, bool targetTabbed, int index) : base(panel) {
			this.target = target;
			this.dragPoint = dragPoint;
			this.dock = dock;
			this.targetTabbed = targetTabbed;
			this.index = index;
		}
		public DockPanel TargetPanel { get { return (target as DockPanel); } }
		public ContainerControl TargetForm { get { return (target as ContainerControl); } }
		public DockingStyle Dock { get { return dock; } }
		public Point DragPoint { get { return dragPoint; } }
		public bool TargetTabbed { get { return targetTabbed; } }
		public int Index { get { return index; } }
	}
	public class EndDockingEventArgs : DockPanelEventArgs {
		bool canceled;
		ReadOnlyPanelCollection dockedPanels, children;
		public EndDockingEventArgs(DockPanel panel, bool canceled) : base(panel) {
			this.canceled = canceled;
			this.dockedPanels = null;
			this.children = new ReadOnlyPanelCollection();
			for(int i = 0; i < Panel.Count; i++)
				children.Add(Panel[i]);
		}
		void PopulateDockedPanels() {
			this.dockedPanels = new ReadOnlyPanelCollection();
			if(Canceled) return;
			if(Panel.IsDisposed)
				dockedPanels = children;
			else
				dockedPanels.Add(Panel);
		}
		public ReadOnlyPanelCollection DockedPanels { 
			get { 
				if(this.dockedPanels == null)
					PopulateDockedPanels();
				return this.dockedPanels; 
			}
		}
		public bool Canceled { get { return canceled; } }
	}
	public class StartSizingEventArgs : DockPanelCancelEventArgs {
		SizingSide sizingSide;
		public StartSizingEventArgs(DockPanel panel, SizingSide sizingSide) : base(panel) {
			this.sizingSide = sizingSide;
		}
		public SizingSide SizingSide { get { return sizingSide; } }
	}
	public class SizingEventArgs : StartSizingEventArgs {
		Point ptClient;
		Size newSize;
		public SizingEventArgs(DockPanel panel, SizingSide sizingSide, Point ptClient, Size newSize) : base(panel, sizingSide) {
			this.ptClient = ptClient;
			this.newSize = newSize;
		}
		public Point PtClient { get { return ptClient; } }
		public Size NewSize { get { return newSize; } }
	}
	public class EndSizingEventArgs : DockPanelEventArgs {
		SizingSide sizingSide;
		bool canceled;
		public EndSizingEventArgs(DockPanel panel, bool canceled, SizingSide sizingSide) : base(panel) {
			this.sizingSide = sizingSide;
			this.canceled = canceled;
		}
		public SizingSide SizingSide { get { return sizingSide; } }
		public bool Canceled { get { return canceled; } }
	}
	public class AutoHideContainerEventArgs : EventArgs {
		AutoHideContainer container;
		public AutoHideContainerEventArgs(AutoHideContainer container) {
			this.container = container;
		}
		public AutoHideContainer Container { get { return container; } }
		public TabsPosition Position { get { return Container.Position; } }
	}
	public class PopupMenuShowingEventArgs : System.ComponentModel.CancelEventArgs {
		Point pointCore;
		DockControllerMenu menuCore;
		public PopupMenuShowingEventArgs(DockControllerMenu menu, Point point) {
			pointCore = point;
			menuCore = menu;
		}
		public Point Point {
			get { return pointCore; }
		}
		public Control Control {
			get { return Menu.PlacementTarget; }
		}
		public DockControllerMenu Menu {
			get { return menuCore; }
		}
	}
	public class ShowingDockGuidesEventArgs : DockPanelEventArgs {
		public ShowingDockGuidesEventArgs(DockPanel dockPanel, DockPanel targetPanel, Docking2010.Customization.DockGuidesConfiguration configuration)
			: base(dockPanel) {
			this.TargetPanel = targetPanel;
			this.Configuration = configuration;
		}
		public DockPanel TargetPanel { get; private set; }
		public Docking2010.Customization.DockGuidesConfiguration Configuration { get; private set; }
	}
	public delegate void ShowingDockGuidesEventHandler(
		object sender, ShowingDockGuidesEventArgs e);
}
