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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Diagram {
	public class DiagramMenu : PopupMenu {
		readonly Func<IEnumerable<IBarManagerControllerAction>> getMenuActions;
		BarManagerActionCollection Actions { get { return Controller.ActionContainer.Actions; } }
		bool initialized = false;
		public DiagramMenu(Func<IEnumerable<IBarManagerControllerAction>> getMenuActions) {
			this.getMenuActions = getMenuActions;
		}
		public override void ShowPopup(UIElement control) {
			if(!initialized) {
				initialized = true;
				getMenuActions().ForEach(x => Actions.Add(x));
				Controller.Execute();
			}
			base.ShowPopup(control);
		}
		public virtual void Destroy() {
			ItemLinks.Clear();
			Actions.Clear();
			initialized = false;
		}
		BarManagerMenuController controller;
		BarManagerMenuController Controller {
			get {
				if(controller == null) {
					controller = new BarManagerMenuController(this);
					AddLogicalChild(controller);
				}
				return controller;
			}
		}
		protected override IEnumerator LogicalChildren { get { return new MergedEnumerator(base.LogicalChildren, new SingleLogicalChildEnumerator(controller)); } }
	}
}
namespace DevExpress.Xpf.Diagram.Native {
	public class MenuController {
		readonly DiagramControl diagram;
		readonly Lazy<DiagramMenu> menu;
		readonly Lazy<DiagramPopupToolBar> toolBar;
		protected DiagramControl Diagram { get { return diagram; } }
		protected DiagramMenu Menu { get { return menu.Value; } }
		protected DiagramPopupToolBar ToolBar { get { return toolBar.Value; } }
#if DEBUGTEST
		internal DiagramMenu MenuForTests { get { return Menu; } }
		internal DiagramPopupToolBar ToolbarForTests { get { return ToolBar; } }
#endif
		public MenuController(DiagramControl diagram) {
			this.diagram = diagram;
			menu = new Lazy<DiagramMenu>(() => CreateMenu(() => Diagram.CreateContextMenu()));
			toolBar = new Lazy<DiagramPopupToolBar>(() => {
				EventManager.RegisterClassHandler(typeof(FrameworkElement), FrameworkElement.PreviewKeyDownEvent, new KeyEventHandler(OnDiagramKeyDown));
				var createdToolbar = CreateToolBar(() => Diagram.CreateContextToolBar());
				createdToolbar.PlacementTarget = Diagram;
				createdToolbar.Opened += OnToolbarOpened;
				return createdToolbar;
			});
		}
		protected virtual DiagramMenu CreateMenu(Func<IEnumerable<IBarManagerControllerAction>> getMenuActions) {
			return new DiagramMenu(getMenuActions);
		}
		protected virtual DiagramPopupToolBar CreateToolBar(Func<IEnumerable<IBarManagerControllerAction>> getToolBarItems) {
			return new DiagramPopupToolBar(getToolBarItems);
		}
		public void ShowPopupMenu(DiagramMenuPlacement placement) {
			if(ToolBar.IsOpen || Menu.IsOpen)
				return;
			OnShowMenu(placement);
		}
		public void DestroyPopupMenu() {
			CloseMenu();
			Menu.Destroy();
			CloseToolBar();
			Diagram.DestroyContextMenu();
			Diagram.DestroyContextToolBar();
		}
		protected virtual void CloseMenu() {
			Menu.IsOpen = false;
		}
		protected virtual void CloseToolBar() {
			ToolBar.IsOpen = false;
		}
		protected virtual void OnShowMenu(DiagramMenuPlacement placement) {
			UIElement placementTarget = Diagram.PrimarySelection ?? Diagram.RootItem;
			if(placement == DiagramMenuPlacement.Mouse) {
				ToolBar.PlacementTarget = placementTarget;
				ToolBar.IsOpen = true;
				placementTarget = ToolBar;
				Menu.Placement = PlacementMode.Bottom;
			} else {
				Menu.Placement = SystemParameters.MenuDropAlignment ? PlacementMode.Left : PlacementMode.Right;
			}
			Menu.ShowPopup(placementTarget);
			Menu.Dispatcher.BeginInvoke(new Action(() => UpdateMenuOffsets(placement, placementTarget)), DispatcherPriority.Render);
		}
		void UpdateMenuOffsets(DiagramMenuPlacement placement, UIElement placementTarget) {
			if(placement == DiagramMenuPlacement.Mouse) {
				Menu.HorizontalOffset = SystemParameters.MenuDropAlignment ? Menu.Child.RenderSize.Width - ToolBar.ActualWidth : 0d;
				Menu.VerticalOffset = 20;
			} else {
				double horizontalOffset = -placementTarget.RenderSize.Width / 2;
				if(placementTarget.RenderSize.Width * diagram.ZoomFactor <= Menu.Child.RenderSize.Width)
					horizontalOffset = 0;
				Menu.HorizontalOffset = horizontalOffset;
				Menu.VerticalOffset = placementTarget.RenderSize.Height / 2;
			}
		}
		void OnDiagramKeyDown(object sender, KeyEventArgs e) {
			e.HandleEvent(() => CloseOnEscapePressed(e));
		}
		bool CloseOnEscapePressed(KeyEventArgs e) {
			if(ToolBar.IsOpen && e.Key == Key.Escape) {
				CloseToolBar();
				return true;
			}
			return false;
		}
		protected virtual void OnToolbarOpened(object sender, EventArgs e) {
			var placementTarget = ToolBar.PlacementTarget;
			var position = Mouse.GetPosition(placementTarget);
			double horizontalPosition = placementTarget.RenderSize.Width - position.X;
			double horizontalOffset = 0; 
			if(horizontalPosition * Diagram.ZoomFactor > ToolBar.ActualWidth)
				horizontalOffset = -horizontalPosition;
			ToolBar.HorizontalOffset = horizontalOffset;
		}
	}
}
