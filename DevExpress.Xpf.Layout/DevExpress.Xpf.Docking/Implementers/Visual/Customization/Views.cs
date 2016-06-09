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

using System.Windows;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.UIInteraction;
namespace DevExpress.Xpf.Docking.Platform {
	public class CustomizationViewElementFactory : LayoutElementFactory {
		protected override void InitializeFactory() {
			Initializers[typeof(VisualElements.CustomizationControl)] = (element, view) => new CustomizationControlElement(element, view);
			Initializers[typeof(VisualElements.HiddenItem)] = (element, view) => new HiddenItemElement(element, view);
			Initializers[typeof(VisualElements.TreeItem)] = (element, view) => new TreeItemElement(element, view);
			Initializers[typeof(VisualElements.HiddenItemsPanel)] = (element, view) => new HiddenItemsListElement(element, view);
		}
	}
	public class CustomizationView : LayoutView {
		public CustomizationView(IUIElement viewUIElement)
			: base(viewUIElement) {
		}
		protected override ILayoutElementFactory ResolveDefaultFactory() {
			return new CustomizationViewElementFactory();
		}
		protected override void RegisterListeners() {
			RegisterUIServiceListener(new LayoutViewSelectionListener());
			RegisterUIServiceListener(new CustomizationViewUIInteractionListener());
			RegisterUIServiceListener(new CustomizationViewClientDraggingListener());
		}
	}
	public class CustomizationViewUIInteractionListener : UIInteractionServiceListener {
		public CustomizationView View {
			get { return ServiceProvider as CustomizationView; }
		}
		public override void OnActivate() {
			View.Container.CustomizationController.CloseMenu();
		}
		public override bool OnActiveItemChanging(ILayoutElement element) {
			BaseLayoutItem itemToActivate = ((IDockLayoutElement)element).Item;
			return (itemToActivate != null) && itemToActivate.AllowActivate && !itemToActivate.IsHidden;
		}
		public override bool OnActiveItemChanged(ILayoutElement element) {
			BaseLayoutItem itemToActivate = ((IDockLayoutElement)element).Item;
			LayoutGroup activationRoot = View.Container.ActivateCore(itemToActivate);
			if(activationRoot != null)
				View.Container.CustomizationController.CustomizationRoot = activationRoot;
			return itemToActivate != null && itemToActivate.IsActive;
		}
		public override bool OnMenuAction(LayoutElementHitInfo clickInfo) {
			View.Container.RenameHelper.CancelRenamingAndResetClickedState();
			IDockLayoutElement dockElement = clickInfo.Element as IDockLayoutElement;
			if(dockElement is HiddenItemElement) {
				if(dockElement.Item is FixedItem) return false;
				View.Container.CustomizationController.MenuSource = dockElement.Element;
				View.Container.CustomizationController.ShowHiddenItemMenu(dockElement.Item);
				return true;
			}
			if(dockElement is TreeItemElement) {
				View.Container.CustomizationController.MenuSource = dockElement.Element;
				return new MenuHelper(View.Container).ShowMenu(dockElement.Item);
			}
			return false;
		}
		public override bool OnClickPreviewAction(LayoutElementHitInfo clickInfo) {
			View.Container.RenameHelper.CancelRenamingAndResetClickedState();
			return base.OnClickPreviewAction(clickInfo);
		}
	}
	public class CustomizationViewClientDraggingListener : LayoutViewClientDraggingListener {
		public override void OnDragging(Point point, ILayoutElement element) {
			DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, point, element);
			ILayoutElement target = (ILayoutElement)dragInfo.DropTarget;
			Point screenPoint = View.ClientToScreen(point);
			if(!IsLayoutRoot(dragInfo.Target) && dragInfo.DropTarget is TreeItemElement && dragInfo.MoveType != MoveType.None) {
				Rect targetRect = ElementHelper.GetScreenRect(View, target);
				screenPoint = new CursorLocationHelper(160, 40).CorrectPositon(targetRect, dragInfo.MoveType);
				if(View.Container.FlowDirection == FlowDirection.RightToLeft)
					screenPoint.X += 160;
			}
			View.Container.CustomizationController.SetDragCursorPosition(screenPoint);
		}
		bool IsLayoutRoot(BaseLayoutItem item) {
			return item is LayoutGroup && ((LayoutGroup)item).IsLayoutRoot;
		}
		public override bool CanDrag(Point point, ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			if(item == null) return false;
			return true;
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, point, element);
			if((dragInfo.DropTarget is HiddenItemElement || dragInfo.DropTarget is HiddenItemsListElement) && dragInfo.Item.AllowHide) {
				LayoutItemType type = dragInfo.Item.ItemType;
				return LayoutItemsHelper.IsLayoutItem(dragInfo.Item) || type == LayoutItemType.Group;
			}
			return base.CanDrop(point, element);
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			ResetVisualization();
			DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, point, element);
			if((dragInfo.DropTarget is HiddenItemElement || dragInfo.DropTarget is HiddenItemsListElement) && dragInfo.Item.AllowHide) {
				View.Container.LayoutController.Hide(dragInfo.Item);
				return;
			}
			base.OnDrop(point, element);
		}
	}
	class CursorLocationHelper {
		int width, height;
		public CursorLocationHelper(int w, int h) {
			this.width = w; this.height = h;
		}
		public Point CorrectPositon(Rect targetRect, MoveType type) {
			double x = targetRect.X; double y = targetRect.Y;
			switch(type) {
				case MoveType.Left:
					x = targetRect.Left - width;
					y = targetRect.Top - (height - targetRect.Height) / 2;
					break;
				case MoveType.Right:
					x = targetRect.Right;
					y = targetRect.Top - (height - targetRect.Height) / 2;
					break;
				case MoveType.Top:
					x = targetRect.Left - (width - targetRect.Width) / 2;
					y = targetRect.Top - height;
					break;
				case MoveType.Bottom:
					x = targetRect.Left - (width - targetRect.Width) / 2;
					y = targetRect.Bottom;
					break;
			}
			return new Point(x, y);
		}
	}
}
