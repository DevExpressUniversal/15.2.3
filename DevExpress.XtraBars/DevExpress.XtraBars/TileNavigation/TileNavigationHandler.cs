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
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Navigation {
	class OfficeNavigationBarHandler : TileControlHandler {
		public OfficeNavigationBarHandler(ITileControl control)
			: base(control) {
		}
		protected OfficeNavigationBarViewInfo ViewInfo {
			get { return Control.ViewInfo as OfficeNavigationBarViewInfo; }
		}
		protected OfficeNavigationBar NavigationBar {
			get { return (ViewInfo != null && ViewInfo.OwnerCore != null) ? ViewInfo.OwnerCore.Owner : null; }
		}
		protected NavigationBarCore NavigationBarCore {
			get { return Control as NavigationBarCore; }
		}
		public override void OnMouseUp(MouseEventArgs e) {
			if(!ProceedPopupMenuClick(e))
				base.OnMouseUp(e);
			else {
				ViewInfo.PeekHoverInfo = null;
				ViewInfo.PressedInfo = null;
				if(NavigationBarCore != null)
					NavigationBarCore.Handler.ResetMouseScroll();
			}
		}
		bool ProceedPopupMenuClick(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left)
				return ProceedPopupMenuLeftClick(e);
			if(e.Button == MouseButtons.Right)
				return ProceedPopupMenuRightClick(e);
			return false;
		}
		bool ProceedPopupMenuLeftClick(MouseEventArgs e) {
			if(State == TileControlHandlerState.DragMode) return false;
			NavigationBarCore controlCore = Control as NavigationBarCore;
			DevExpress.XtraEditors.ButtonPanel.BaseButtonInfo upInfo = null;
			if(controlCore != null)
				upInfo = controlCore.ButtonsPanel.ViewInfo.CalcHitInfo(e.Location);
			if(upInfo != null && upInfo.Button is NavigationBarCustomizationButton)
				return controlCore.ShowCustomizationMenu(e.Location);
			return false;
		}
		bool ProceedPopupMenuRightClick(MouseEventArgs e) {
			TileControlHitInfo upInfo = Control.ViewInfo.CalcHitInfo(e.Location);
			DevExpress.XtraEditors.ButtonPanel.BaseButtonInfo panelUpInfo = null;
			NavigationBarCore controlCore = Control as NavigationBarCore;
			if(controlCore != null)
				panelUpInfo = controlCore.ButtonsPanel.ViewInfo.CalcHitInfo(e.Location);
			if(!upInfo.InItem || upInfo.ItemInfo == null || controlCore == null)
				if(panelUpInfo != null && panelUpInfo.Button is NavigationBarCustomizationButton)
					return controlCore.ShowCustomizationMenu(e.Location);
				else
					return false;
			NavigationBarItemCore item = (upInfo.ItemInfo.Item as NavigationBarItemCore);
			return (item != null) && controlCore.ShowItemMenu(item.OwnerItem, e.Location);
		}
		protected override bool ShouldProcessMouseDown(TileControlHitInfo pressedInfo) {
			if(pressedInfo.InItem && pressedInfo.ItemInfo != null) {
				PressedItemDragGutter = pressedInfo.ItemInfo.Bounds.Location;
				ViewInfo.PeekHoverInfo = null;
			}
			return base.ShouldProcessMouseDown(pressedInfo);
		}
		Point PressedItemDragGutter { get; set; }
		protected override void UpdateDragItemBounds(System.Drawing.Point pt) {
			Point res;
			if(Control.Properties.Orientation == System.Windows.Forms.Orientation.Horizontal)
				res = new Point(pt.X, PressedItemDragGutter.Y);
			else
				res = new Point(PressedItemDragGutter.X, pt.Y);
			base.UpdateDragItemBounds(res);
		}
		protected override void UpdateItemBounds(TileItemViewInfo itemInfo, Point pt) {
			Point contentPoint = Control.ViewInfo.PointToContent(pt);
			if(Control.Properties.Orientation == System.Windows.Forms.Orientation.Horizontal)
				itemInfo.RenderImageBounds = new Rectangle(contentPoint.X - ContentOffset.X, contentPoint.Y, itemInfo.RenderImageBounds.Width, itemInfo.RenderImageBounds.Height);
			else
				itemInfo.RenderImageBounds = new Rectangle(contentPoint.X, contentPoint.Y - ContentOffset.Y, itemInfo.RenderImageBounds.Width, itemInfo.RenderImageBounds.Height);
		}
		protected override void DropNewGroup(TileControlDropInfo dropInfo, TileGroup newGroup) {
			TileItemViewInfo dragItem = DragItem;
			Control.BeginUpdate();
			try {
				TileGroup group = dragItem.Item.Group;
				if(dropInfo.NearestGroupInfo != null && dropInfo.NearestGroupInfo.Group == group && group.Items.Count == 1) {
					ViewInfo.PrepareItemsForExitDragMode();
					return;
				}
				ViewInfo.ShouldResetSelectedItem = false;
				if(ViewInfo.DropTargetInfo.GroupDropSide == TileControlDropSide.Right || ViewInfo.DropTargetInfo.GroupDropSide == TileControlDropSide.Bottom)
					AddItemToEnd(group, dragItem.Item);
				else
					AddItemToStart(group, dragItem.Item);
				if(Control.IsDesignMode) {
					ViewInfo.DesignTimeManager.ComponentChanged(Control.Control);
				}
				ViewInfo.CacheItems();
				ViewInfo.ShouldMakeTransition = true;
			}
			finally { Control.EndUpdate(); }
		}
		void AddItemToEnd(TileGroup group, TileItem item) {
			if(ViewInfo.HiddenItems.Count == 0) {
				int maxVisible = ViewInfo.OwnerCore.MaxItemCount;
				if(maxVisible > 0) {
					group.Items.Insert(Math.Max(maxVisible - 1, 0), item);
				}
				else {
					group.Items.Add(item);
				}
			}
			else {
				int insertIndex = group.Items.IndexOf(ViewInfo.HiddenItems[0]) - 1;
				group.Items.Insert(Math.Max(insertIndex, 0), item);
			}
		}
		void AddItemToStart(TileGroup group, TileItem item) {
			group.Items.Insert(0, item);
		}
		protected override void OnMouseMoveButtonsNone(MouseEventArgs e) {
			base.OnMouseMoveButtonsNone(e);
			if(Control is NavigationBarCore && ((NavigationBarCore)Control).ShowPeekFormOnItemHover)
				ProceedItemPeekHovering(e);
		}
		protected virtual void ProceedItemPeekHovering(MouseEventArgs e) {
			if(ViewInfo == null || (NavigationBar != null && NavigationBar.IsMenuOpen)) return;
			ViewInfo.PeekHoverInfo = ViewInfo.CalcHitInfo(e.Location);
		}
		public override void OnMouseLeave(EventArgs e) {
			ViewInfo.PeekHoverInfo = null;
			base.OnMouseLeave(e);
		}
		protected override void OnDragDrop(bool cancelDrop) {
			base.OnDragDrop(cancelDrop);
			SyncItemsAndVisibleItems();
		}
		private void SyncItemsAndVisibleItems() {
			var controlCore = Control as NavigationBarCore;
			if(controlCore != null) controlCore.SyncItemsAndVisibleItems();
		}
	}
}
