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
using System.Drawing.Drawing2D;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraLayout.Customization {
	public class DragBoundsCalculator {
		LayoutItemDragController controller = null;
		protected internal LayoutRectangle CalculateEmptyGroupInsideRectangle(LayoutRectangle lBounds) {
			Rectangle tempRect = lBounds.Rectangle;
			int inflateRatingX = 4;
			int inflateRatingY = 4;
			int deltaX = tempRect.Width/inflateRatingX;
			int deltaY = tempRect.Height/inflateRatingY;
			tempRect.X += deltaX;
			tempRect.Y += deltaY;
			tempRect.Width -= deltaX<<1;
			tempRect.Height -= deltaY<<1;
			return new LayoutRectangle(tempRect, lBounds.LayoutType);
		}
		protected LayoutRectangle CalculateItemDragInsideRectangle(LayoutRectangle lBounds, InsertLocation insertLocation) {
			lBounds.Width -= lBounds.Width >>1;
			if(insertLocation == InsertLocation.After)
				lBounds.X += lBounds.Width;
			return lBounds;
		}
		protected LayoutRectangle CalculateInsideDragRectangle(BaseLayoutItem baseItem, InsertLocation insertLocation, LayoutType layoutType) {
			LayoutRectangle lBounds;
			lBounds = baseItem.GetLayoutBounds(layoutType);
			lBounds = CalculateItemDragInsideRectangle(lBounds, insertLocation);
			return lBounds;
		}
		Rectangle TranslateRectangle(Rectangle rect, BaseLayoutItem item) {
			rect.Offset(item.ViewInfo.Offset);
			rect.Offset(new Point(-item.Location.X, -item.Location.Y));
			return rect;
		}
		protected Rectangle GetDragRectangleTabbedGroup(LayoutGroup group) {
			TabbedGroup tgroup = group.ParentTabbedGroup;
			int hotTabIndex = tgroup.VisibleTabPages.IndexOf(group);
			TabbedGroupHitInfo hitInfo = controller.HitInfo as TabbedGroupHitInfo;
			InsertLocation insertLocation = controller.InsertLocation;
			if(hitInfo != null) insertLocation = hitInfo.TabbedGroupInsertLocation;
			Rectangle rect = tgroup.ViewInfo.GetScreenTabCaptionRect(hotTabIndex);
			Rectangle realClientArea = tgroup.ViewInfo.RealClientArea;
			if(realClientArea.Contains(controller.HitInfo.HitPoint)) {
				if(group.Items.Count == 0) {
					return group.ViewInfo.BoundsRelativeToControl;
				}
			}
			if(hotTabIndex == 0 && insertLocation == InsertLocation.Before) {
				if (tgroup.GetLayoutTypeForHinInfo() == LayoutType.Horizontal)
					rect.Width = rect.Width / 3;
				else rect.Height = rect.Height / 3;
			} else {
				if(hotTabIndex == (tgroup.VisibleTabPages.Count - 1) && insertLocation == InsertLocation.After) {
					if (tgroup.GetLayoutTypeForHinInfo() == LayoutType.Horizontal) {
						rect.Width = rect.Width / 3;
						rect.X = rect.X + rect.Width * 2;
					} else {
						rect.Height = rect.Height / 3;
						rect.Y = rect.Y + rect.Height * 2;
					}
				} else {
					if(insertLocation == InsertLocation.Before) {
						rect = GetDragBoundsBetwenItems(tgroup.ViewInfo.GetScreenTabCaptionRect(hotTabIndex - 1), tgroup.ViewInfo.GetScreenTabCaptionRect(hotTabIndex), tgroup.GetLayoutTypeForHinInfo());
					}
					if(insertLocation == InsertLocation.After) {
						rect = GetDragBoundsBetwenItems(tgroup.ViewInfo.GetScreenTabCaptionRect(hotTabIndex), tgroup.ViewInfo.GetScreenTabCaptionRect(hotTabIndex + 1), tgroup.GetLayoutTypeForHinInfo());
					}
				}
			}
			return rect;
		}
		Rectangle GetDragBoundsBetwenItems(Rectangle rect1, Rectangle rect2, LayoutType layoutType) {
			Rectangle result = new Rectangle();
			if (layoutType == LayoutType.Horizontal) {
				result.Height = Math.Max(rect1.Height, rect2.Height);
				result.Width = rect1.Width / 3 + rect2.Width / 3;
				result.X = rect1.X + rect1.Width / 3 * 2;
				result.Y = Math.Min(rect1.Y, rect2.Y);
			} else {
				result.Width = Math.Max(rect1.Width, rect2.Width);
				result.Height = rect1.Height / 3 + rect2.Height / 3;
				result.Y = rect1.Y + rect1.Height / 3 * 2;
				result.X = Math.Min(rect1.X, rect2.X);
			}
			return result;
		}
		Rectangle CalcLayoutItemDragBounds(BaseLayoutItem item) {
			LayoutRectangle lBounds;
			if(controller.MoveType == MoveType.Inside) {
				lBounds = CalculateInsideDragRectangle(item, controller.InsertLocation, controller.LayoutType);
			} else {
				if(item.Parent == null)
					return Rectangle.Empty;
				else
					lBounds = ((LayoutGroup)item.Parent).GetMovedOutsideItemRectangle(new Size(5, 5), controller);
			}
			return TranslateRectangle(lBounds.Rectangle, item);
		}
		Rectangle CalcLayoutGroupDragBounds(LayoutGroup group) {
			if(group.ParentTabbedGroup != null && controller.DragItem is LayoutGroup) {
				return GetDragRectangleTabbedGroup(group);
			}
			if(controller.HitInfo.Item != null && controller.HitInfo.IsGroup && (controller.HitInfo.Item as LayoutGroup).LayoutMode == LayoutMode.Table && controller.HitInfo is LayoutGroupHitInfo) {
				LayoutGroupHitInfo groupHitInfo = controller.HitInfo as LayoutGroupHitInfo;
				if(groupHitInfo.AdditionalHitType == LayoutGroupHitTypes.TableDefinition) {
					Rectangle baseRect = (controller.HitInfo.Item as LayoutGroup).GetTableItemBounds(groupHitInfo.rowIndex, groupHitInfo.columnIndex);
					baseRect.Offset(controller.HitInfo.Item.ViewInfo.ClientAreaRelativeToControl.Location);
					return baseRect;
				}
			}
			if(group.Expanded && group.Items.Count == 0 && (controller.MoveType == MoveType.Inside || group.Parent == null || group.ParentTabbedGroup != null)) {
				LayoutRectangle lBounds = new LayoutRectangle(group.ViewInfo.BoundsRelativeToControl, LayoutType.Horizontal);
				return CalculateEmptyGroupInsideRectangle(lBounds).Rectangle;
			} else {
				return CalcLayoutItemDragBounds(group);
			}
		}
		Rectangle CalcLayoutTabbedGroupDragBounds(TabbedGroup tabbedGroup) {
			if(tabbedGroup.VisibleTabPages.Count == 0 && controller.MoveType == MoveType.Inside && controller.DragItem is LayoutGroup) {
				LayoutRectangle lBounds = new LayoutRectangle(tabbedGroup.ViewInfo.BoundsRelativeToControl, LayoutType.Horizontal);
				return CalculateEmptyGroupInsideRectangle(lBounds).Rectangle;
			} else {
				TabbedGroupHitInfo hitInfo = controller.HitInfo as TabbedGroupHitInfo;
				if(hitInfo != null && hitInfo.TabbedGroupHotPageIndex >= 0 && controller.DragItem is LayoutGroup) {
					return GetDragRectangleTabbedGroup(tabbedGroup.VisibleTabPages[hitInfo.TabbedGroupHotPageIndex]);
				} else {
					return CalcLayoutItemDragBounds(tabbedGroup);
				}
			}
		}
		Rectangle GetDragBoundsRect() {
			BaseLayoutItem bitem = controller.Item as BaseLayoutItem;
			LayoutControlGroup group = controller.Item as LayoutControlGroup;
			TabbedGroup tgroup = controller.Item as TabbedGroup;
			if(group != null) {
				return CalcLayoutGroupDragBounds(group);
			}
			if(tgroup != null) {
				return CalcLayoutTabbedGroupDragBounds(tgroup);
			}
			if(bitem != null) {
				return CalcLayoutItemDragBounds(bitem);
			}
			return Rectangle.Empty;
		}
		public Rectangle Calculate(LayoutItemDragController controller) {
			this.controller = controller;
			if(controller.Item != null) {
				if (controller.DragItem != null && !controller.DragItem.ActualItemVisibility) return Rectangle.Empty;
				if(controller.Item is LayoutGroup && ((LayoutGroup)controller.Item).ParentTabbedGroup != null) return Rectangle.Empty;
				if(controller.Item == controller.DragItem) return Rectangle.Empty;
				return GetDragBoundsRect();
			} else
				return Rectangle.Empty;
		}
	}
	public class LayoutItemDragController {
		internal protected BaseItemCollection insertToItems; 
		Size ratingCore;
		MoveType moveTypeCore;
		LayoutType layoutTypeCore;
		InsertLocation insertLocation;
		BaseLayoutItem dragItemCore;
		BaseLayoutItemHitInfo hitInfoCore;
		DragBoundsCalculator dragBoundsCalculatorCore = null;
		bool shouldRestoreOriginalSizeCore = true;
		public LayoutItemDragController(BaseLayoutItem dragItem, LayoutItemDragController controller) {
			Init(dragItem, controller.Item, controller.MoveType, controller.InsertLocation, controller.LayoutType, controller.HitPoint);
			this.insertToItems = controller.insertToItems;
			this.ratingCore = controller.Rating;
		}
		public LayoutItemDragController(BaseLayoutItem dragItem, BaseLayoutItem baseItem, MoveType moveType, InsertLocation insertLocation, LayoutType layoutType, Size rating) {
			Init(dragItem, baseItem, moveType, insertLocation, layoutType, Point.Empty);
			this.ratingCore = rating;
		}
		public LayoutItemDragController(BaseLayoutItem dragItem, BaseLayoutItem baseItem, MoveType moveType, InsertLocation insertLocation, LayoutType layoutType) {
			Init(dragItem, baseItem, moveType, insertLocation, layoutType, Point.Empty);
			this.ratingCore = new Size(100, 50);
		}
		public LayoutItemDragController(BaseLayoutItem dragItem, BaseLayoutItem baseItem, InsertLocation insertLocation, LayoutType layoutType) {
			Init(dragItem, baseItem, MoveType.Inside, insertLocation, layoutType, Point.Empty);
		}
		public LayoutItemDragController(BaseLayoutItem dragItem, BaseLayoutItem baseItem) {
			Init(dragItem, baseItem, MoveType.Inside, InsertLocation.After, LayoutType, Point.Empty);
		}
		internal LayoutItemDragController(BaseLayoutItem dragItem, MoveType moveType, InsertLocation insertLocation, LayoutType layoutType) {
			Init(dragItem, null, moveType, insertLocation, layoutType, Point.Empty);
		}
		public LayoutItemDragController(BaseLayoutItem dragItem, LayoutGroup group, Point pt) {
			if(group != null) {
				this.ratingCore = Size.Empty;
				this.dragItemCore = dragItem;
				this.hitInfoCore = group.CalcHitInfo(pt, true);
				if(HitInfo.Item == null) HitInfo.SetItem(group);
				if(HitInfo.Item.Parent != null && !HitInfo.Item.Parent.AllowCustomizeChildren) HitInfo.SetItem(HitInfo.Item.Parent);
				Calculate();
			} else
				throw new NullReferenceException("Group is null");
		}
		protected DragBoundsCalculator DragBoundsCalculator {
			get {
				if(dragBoundsCalculatorCore == null) dragBoundsCalculatorCore = new DragBoundsCalculator();
				return dragBoundsCalculatorCore;
			}
		}
		protected internal void SetItem(BaseLayoutItem item) {
			HitInfo.SetItem(item);
		}
		protected virtual void Init(BaseLayoutItem dragItem, BaseLayoutItem baseItem, MoveType moveType, InsertLocation insertLocation, LayoutType layoutType, Point hitPoint) {
			ratingCore = Size.Empty;
			this.dragItemCore = dragItem;
			this.layoutTypeCore = layoutType;
			this.insertLocation = insertLocation;
			this.moveTypeCore = moveType;
			this.hitInfoCore = new BaseLayoutItemHitInfo();																							 
			HitInfo.SetItem(baseItem);
			HitInfo.SetHitPoint(hitPoint);
			CheckSpltterItemMoveType();
			CheckFlowLayoutMoveType();
			CheckTableLayoutModeType();
		}
		private void CheckTableLayoutModeType() {
			if(HitInfo.Item != null && HitInfo.Item.IsGroup && (HitInfo.Item as LayoutGroup).LayoutMode == LayoutMode.Table && HitInfo is LayoutGroupHitInfo) {
				LayoutGroupHitInfo groupHitInfo = HitInfo as LayoutGroupHitInfo;
				if(groupHitInfo.AdditionalHitType != LayoutGroupHitTypes.TableDefinition) return;
				moveTypeCore = Utils.MoveType.Inside;
			}
		}
		protected void CheckSpltterItemMoveType() {
			if(DragItem is SplitterItem) moveTypeCore = MoveType.Outside;
		}
		protected void CheckFlowLayoutMoveType() {
			if(HitInfo.Item != null && HitInfo.Item.Parent != null && HitInfo.Item.Parent.LayoutMode == LayoutMode.Flow) {
				moveTypeCore = MoveType.Inside;
				if(layoutTypeCore == Utils.LayoutType.Vertical) layoutTypeCore = Utils.LayoutType.Horizontal;
				if(DragItem is LayoutGroup) {
					moveTypeCore = Utils.MoveType.Outside;
					HitInfo.SetItem(HitInfo.Item.Parent);
				}
			}
		}
		protected virtual void Calculate() {
			CalculateGroupItemDragging();
		}
		protected virtual void CalculateGroupItemDragging() {
			CalculateGroupItemDraggingBase();
			LayoutGroup group = Item as LayoutGroup;
			TabbedControlGroup tabbedGroup = Item as TabbedControlGroup;
			if(group != null) {
				if(group.ParentTabbedGroup == null) {
					if(group.Items.Count != 0) {
						LayoutRectangle groupRect = new LayoutRectangle(group.ViewInfo.BoundsRelativeToControl, LayoutType);
						LayoutRectangle groupClientRect = new LayoutRectangle(group.ViewInfo.ClientAreaRelativeToControl, LayoutType);
						LayoutPoint hpoint = new LayoutPoint(HitPoint, LayoutType);
						if(!groupRect.Rectangle.Contains(hpoint.Point)) return;
						int diff = 0;
						int wdiff = 0;
						if(insertLocation == InsertLocation.After) {
							diff = hpoint.X - groupClientRect.Right;
							wdiff = groupRect.Right - groupClientRect.Right;
						} else {
							diff = hpoint.X - groupClientRect.X;
							wdiff = groupRect.X - groupClientRect.X;
						}
						ratingCore.Width = 100 - (int)(((float)(diff)) / (float)wdiff * 100);
					}
				} else {
					TabbedGroup tabbedGroupParent = group.ParentTabbedGroup;
					Rectangle rect = tabbedGroupParent.ViewInfo.TabsCaptionArea;
					if(rect.Contains(HitPoint)) {
						Rectangle topHalfRect = rect;
						topHalfRect.Height = topHalfRect.Height >> 1;
						if(topHalfRect.Contains(HitPoint)) {
							HitInfo.SetItem(tabbedGroup);
							moveTypeCore = MoveType.Outside;
							ratingCore.Width = 100 - (int)(((float)(HitPoint.Y - topHalfRect.Y)) / (float)(topHalfRect.Height * 100));
						}
					}
				}
			}
			CheckSpltterItemMoveType();
			CheckFlowLayoutMoveType();
			CheckTableLayoutModeType();
		}
		protected virtual void CalculateGroupItemDraggingBase() {
			if(Item == null || dragItemCore == Item) return;
			if(HitPoint.Y - Item.ViewInfo.BoundsRelativeToControl.Y <= Item.Size.Height / 4 || (Item.ViewInfo.BoundsRelativeToControl.Bottom - HitPoint.Y) <= Item.Size.Height / 4)
				this.layoutTypeCore = LayoutType.Vertical;
			else 
				this.layoutTypeCore = LayoutType.Horizontal;
			LayoutPoint diff = new LayoutPoint(new Point(HitPoint.X - Item.ViewInfo.BoundsRelativeToControl.Location.X, HitPoint.Y - Item.ViewInfo.BoundsRelativeToControl.Location.Y), LayoutType);
			LayoutRectangle itemRect = new LayoutRectangle(Item.Bounds, LayoutType);
			if(diff.X <= DragOutsideSize) {
				this.moveTypeCore = MoveType.Outside;
				ratingCore.Width = 100 - (int)(((float)(diff.X)) / (float)DragOutsideSize * 100);
				ratingCore.Height = (int)((float)diff.Y / itemRect.Height * 100);
			} else {
				if(itemRect.Width - diff.X <= DragOutsideSize) {
					this.moveTypeCore = MoveType.Outside;
					ratingCore.Width = 100 - Math.Abs((int)((float)(itemRect.Width - diff.X) / (float)DragOutsideSize * 100));
					ratingCore.Height = (int)((float)diff.Y / itemRect.Height * 100);
				} else {
					this.moveTypeCore = MoveType.Inside;
				}
			}
			this.insertLocation = (new LayoutPoint(HitPoint, LayoutType)).X - (new LayoutPoint(Item.ViewInfo.BoundsRelativeToControl.Location, LayoutType)).X > new LayoutSize(Item.Size, LayoutType).Width / 2 
				? InsertLocation.After : InsertLocation.Before;
		}
		public virtual bool ShouldRestoreOriginalSize {
			get { return shouldRestoreOriginalSizeCore; }
			set { shouldRestoreOriginalSizeCore = value; }
		}
		public virtual BaseLayoutItem Item { get { return hitInfoCore.Item; } }
		public virtual Point HitPoint { get { return hitInfoCore.HitPoint; } }
		public virtual MoveType MoveType { get { return moveTypeCore; } }
		public virtual BaseLayoutItemHitInfo HitInfo { get { return hitInfoCore; } }
		public virtual LayoutType LayoutType { get { return layoutTypeCore; } }
		public virtual InsertLocation InsertLocation { get { return insertLocation; } }
		public virtual BaseLayoutItem DragItem { get { return dragItemCore; } }
		public virtual Size Rating { get { return ratingCore; } }
		public virtual int DragOutsideSize {
			get {
				LayoutSize layoutSize = new LayoutSize(Item.Size, LayoutType);
				int result = layoutSize.Width / 6;
				if(result == 0) result++;
				return result;
			}
		}
		public virtual void Drag() {
			DragItem.Move(this);
		}
		public virtual bool DragWildItem() {
			if(DragItem.Parent != null || DragItem.Owner != null) throw new LayoutControlInternalException("DragItem is not wild, use Drag() method");
			if(Item == null) return false;
			LayoutGroup groupItem = Item as LayoutGroup;
			if(hitInfoCore!= null && hitInfoCore is LayoutGroupHitInfo && (hitInfoCore as LayoutGroupHitInfo).AdditionalHitType == LayoutGroupHitTypes.TableDefinition) {
				DragItem.OptionsTableLayoutItem.ColumnIndex = (HitInfo as LayoutGroupHitInfo).columnIndex;
				DragItem.OptionsTableLayoutItem.RowIndex = (HitInfo as LayoutGroupHitInfo).rowIndex;
			}
			if(groupItem != null) {
				if(groupItem.Parent != null && MoveType == Utils.MoveType.Outside) {
					return groupItem.Parent.InsertItem(this);   
				}
					groupItem.AddCore(DragItem);
					return true;
			} else {
				return (Item.Parent != null) ? Item.Parent.InsertItem(this) : false;
			}
		}
		public virtual Rectangle DragBounds {
			get { return DragBoundsCalculator.Calculate(this); }
		}
	}
}
