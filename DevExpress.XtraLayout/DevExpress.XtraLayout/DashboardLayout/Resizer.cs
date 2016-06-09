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
#if DXWhidbey
using System.Collections.Generic;
#endif
using System.Drawing;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraDashboardLayout;
namespace DevExpress.XtraLayout.Resizing {
	public enum ItemRelation { Left, Right, Top, Bottom, None }
	public class FakeEmptySpaceItem2 : FakeEmptySpaceItem {
		public FakeEmptySpaceItem2(string FakeName) : base(FakeName) { }
		protected override void SetSize(Size value) {
			if(Size.Equals(value)) return;
			if(!IsUpdateLocked && Parent != null) {
				Parent.ChangeItemSize(this, value);
				if(Parent != null) Parent.ResetResizerProportions();
			} else {
				SetSizeWithoutCorrection(value);
			}
		}
		protected internal override void ChangeSize(int dif, LayoutType layoutType) {
			LayoutSize newSize = new LayoutSize(Size, layoutType);
			newSize.Width += dif;
			SetInternalSize(newSize.Size);
		}
		public override Size MinSize {
			get {
				return Size.Empty;
			}
			set { base.MinSize = value; }
		}
	}
	public class HorizontalResizeGroup2 : HorizontalResizeGroup {
		public HorizontalResizeGroup2(BaseLayoutItem item_a, BaseLayoutItem item_b, LayoutType layoutType) : base(item_a, item_b, layoutType) { }
		protected override FakeEmptySpaceItem CreateEmptyItem(BaseLayoutItem item) {
			FakeEmptySpaceItem2 esItem = new FakeEmptySpaceItem2(item.Text);
			esItem.SizeConstraintsType = SizeConstraintsType.Custom;
			esItem.Visibility = LayoutVisibility.Never;
			esItem.MaxSize = Size.Empty;
			return esItem;
		}
		public override void UpdateProportion() {
			LayoutSize size1 = new LayoutSize(Item1.Size, layoutType);
			if(size1.Width == 0) size1.Width++;
			LayoutSize size2 = new LayoutSize(Item2.Size, layoutType);
			Relation = (double)size1.Width / (size2.Width + size1.Width);
		}
		protected override int PatchWidth(int width, LayoutSize newSize) {
			if(IsHiddenPart(Item1, true)) width = 0;
			if(IsHiddenPart(Item2, true)) width = newSize.Width;
			return width;
		}
		protected override bool CanPatchMaxSize(BaseLayoutItem item) {
			return !(item is FakeEmptySpaceItem) && IsHiddenPart(item, true) && !(item is LayoutControlItem);
		}
		protected bool CanApplyEdgeResizing(BaseLayoutItem item) {
			if(item == null) return false;
			if(item.Parent == null) return false;
			Resizer2 r2 = (Resizer2)item.Parent.Resizer;
			if(r2 == null) return false;
			if(r2.SizingEdge == null) return false;
			return r2.SizingEdge.IsEdgeForGroup(this);
		}
		protected override bool SetInternalSize(Size size) {
			return base.SetInternalSize(size);
		}
		protected override void SetLocation(Point location) {
			base.SetLocation(location);
		}
		protected internal override int ChangeItemSize(BaseLayoutItem item, int dif, LayoutType layoutType, Constraints constraint) {
			Check();
			int rest;
			if(CanApplyEdgeResizing(item)) {
				int originalDif = dif;
				LayoutSize item1Size = new LayoutSize(Item1.Size, layoutType);
				LayoutSize item2Size = new LayoutSize(Item2.Size, layoutType);
				Constraints item1Constraints = GetConstraint(Item1, layoutType);
				Constraints item2Constraints = GetConstraint(Item2, layoutType);
				int newItem1Width = item1Size.Width + dif;
				int newItem2Width = item2Size.Width - dif;
				if(item1Constraints.min > newItem1Width)
					dif = CorrectDif(dif, Math.Abs(newItem1Width - item1Constraints.min));
				if(item1Constraints.max != 0 && item1Constraints.max < newItem1Width)
					dif = CorrectDif(dif, Math.Abs(newItem1Width - item1Constraints.max));
				if(item2Constraints.min > newItem2Width)
					dif = CorrectDif(dif, Math.Abs(item2Constraints.min - newItem2Width));
				if(item2Constraints.max != 0 && item2Constraints.max < newItem2Width)
					dif = CorrectDif(dif, Math.Abs(newItem2Width - item2Constraints.max));
				if(originalDif != 0 && dif == 0) {
					if(item2Constraints.min > newItem2Width) {
					} else {
						BaseLayoutItem targetItem = null;
						if(Contains(Item1, item)) targetItem = Item1; else targetItem = Item2;
						LayoutSize cSize = new LayoutSize(targetItem.Size, layoutType);
						targetItem.ChangeItemSize(item, originalDif, layoutType, new Constraints(cSize.Width, cSize.Width));
					}
				}
			}
			if(Contains(Item1, item)) {
				Constraints item1Constaints = GetConstraint(Item1, layoutType);
				Constraints item2Constaints = GetConstraint(Item2, layoutType);
				Constraints constaintsToSet = GetConstraint(constraint, item2Constaints);
				Constraints ownConstaints = item1Constaints;
				if(ownConstaints.max != 0 && constaintsToSet.min > ownConstaints.max)
					constaintsToSet.min = ownConstaints.max;
				int restdif = Item1.ChangeItemSize(item, dif, layoutType, constaintsToSet);
				Item2.ChangeLocation(restdif, layoutType);
				rest = restdif + Item2.ChangeItemSize(item, -restdif, layoutType, GetConstraint(constraint, item1Constaints));
			} else {
				if(Contains(Item2, item)) {
					LayoutSize ls1 = new LayoutSize(Item1.Size, layoutType);
					LayoutSize ls2 = new LayoutSize(Item2.Size, layoutType);
					LayoutSize lsm1 = new LayoutSize(Item1.MinSize, layoutType);
					if(dif > 0 && ls1.Width + ls2.Width + dif > constraint.max && constraint.max != 0) {
						rest = Item2.ChangeItemSize(item, dif, layoutType, new Constraints(0, constraint.max - lsm1.Width));
						if(rest != 0) {
							int restdif = Item1.ChangeItemSize(item, -rest, layoutType, new Constraints(0, 0));
							Item2.ChangeLocation(restdif, layoutType);
							rest += restdif;
						}
					} else {
						rest = Item2.ChangeItemSize(item, dif, layoutType,
							GetConstraint(constraint, new Constraints(Item1, layoutType)));
					}
				} else {
					if(item != null) {
						int restdif = Item1.ChangeItemSize(item, dif, layoutType, new Constraints());
						Item2.ChangeLocation(restdif, layoutType);
						rest = restdif + Item2.ChangeItemSize(item, dif - restdif, layoutType,
							GetConstraint(constraint, new Constraints(Item1, layoutType)));
					} else {
						int restdif = Item2.ChangeItemSize(item, dif, layoutType, GetConstraint(constraint, GetConstraint(Item1, layoutType)));
						rest = restdif + Item1.ChangeItemSize(item, dif - restdif, layoutType,
							GetConstraint(constraint, new Constraints(Item2, layoutType)));
						Item2.ChangeLocation(rest - restdif, layoutType);
					}
				}
			}
			UpdateSize(layoutType);
			return rest;
		}
		protected override void SynchronizeBefore(bool workWithItem1, ResizeItemStatus oldStatus, ResizeItemStatus newStatus) {
			LayoutRectangle item1Rect = new LayoutRectangle(Item1.Bounds, this.layoutType);
			LayoutRectangle item2Rect = new LayoutRectangle(Item2.Bounds, this.layoutType);
			if(workWithItem1) {
				if(oldStatus == ResizeItemStatus.Normal && newStatus == ResizeItemStatus.Hidden) EmptyItem1.SetBounds(GetRealItemSize(RealItem1));
				if(oldStatus == ResizeItemStatus.Hidden && newStatus == ResizeItemStatus.Normal && !(RealItem1 is FakeGroup)) {
					if(item1Rect.Width == 0) {
						item1Rect.Width++;
						item2Rect.Left++;
						item2Rect.Width--;
						Item1.SetBounds(item1Rect);
						Item2.SetBounds(item2Rect);
					}
					RealItem1.SetBounds(EmptyItem1.Bounds);
				}
			} else {
				if(oldStatus == ResizeItemStatus.Normal && newStatus == ResizeItemStatus.Hidden) EmptyItem2.SetBounds(GetRealItemSize(RealItem2));
				if(oldStatus == ResizeItemStatus.Hidden && newStatus == ResizeItemStatus.Normal && !(RealItem2 is FakeGroup)) {
					if(item2Rect.Width == 0) {
						item1Rect.Width--;
						item2Rect.Left--;
						item2Rect.Width++;
						Item1.SetBounds(item1Rect);
						Item2.SetBounds(item2Rect);
					}
					RealItem2.SetBounds(EmptyItem2.Bounds);
				}
			}
		}
	}
	public class VerticalResizeGroup2 :VerticalResizeGroup {
		public VerticalResizeGroup2(BaseLayoutItem item_a, BaseLayoutItem item_b, LayoutType layoutType) : base(item_a, item_b, layoutType) { }
		protected override void UpdateMaxSize() {
			base.UpdateMaxSize();
			LayoutSize ls2;
			LayoutSize lsMinSize = new LayoutSize(CalcualteMinSize(), layoutType);
			LayoutSize lsMaxSize = new LayoutSize(this.Item1.MaxSize, this.layoutType);
			ls2 = new LayoutSize(this.Item2.MaxSize, this.layoutType);
			if((ls2.Size == Size.Empty || lsMaxSize.Size == Size.Empty) && IsLockedItems(Item1,Item2)) {
				groupMaxSize = Size.Empty;
				return;
			}
			if(lsMaxSize.Width == 0 || lsMaxSize.Width > ls2.Width && ls2.Width != 0)
				lsMaxSize = ls2;
			if(lsMaxSize.Width != 0 && lsMaxSize.Width < lsMinSize.Width)
				groupMaxSize = groupMinSize;
			else
				groupMaxSize = lsMaxSize.Size;
		}
		protected override FakeEmptySpaceItem CreateEmptyItem(BaseLayoutItem item) {
			FakeEmptySpaceItem2 esItem = new FakeEmptySpaceItem2(item.Text);
			esItem.SizeConstraintsType = SizeConstraintsType.Custom;
			esItem.Visibility = LayoutVisibility.Never;
			esItem.MaxSize = Size.Empty;
			return esItem;
		}																		   
		protected internal override int ChangeItemSize(BaseLayoutItem item, int dif, LayoutType layoutType, Constraints constraint) {
			BaseLayoutItem item1, item2;
			if(Contains(Item1, item)) {
				item1 = Item1;
				item2 = Item2;
			} else {
				if(!Contains(Item2, item))
					item = null;
				item2 = Item1;
				item1 = Item2;
			}
			Constraints item1Constraints = GetConstraint(item1, layoutType);
			Constraints item2Constraints = GetConstraint(item2, layoutType);
			Constraints resultConstraints = item2Constraints;
			if(resultConstraints.min < item1Constraints.min) { resultConstraints.min = item1Constraints.min; }
			if(item1Constraints.max != 0)
				if(resultConstraints.max > item1Constraints.max) resultConstraints.max = item1Constraints.max;
			Constraints tmpConstraints = GetConstraint(constraint, resultConstraints);
			if(((item1Constraints.max == 0 && item2Constraints.max != 0) || (item2Constraints.max == 0 && item1Constraints.max != 0)) &&
			   IsLockedItems(item1, item2)) {
				resultConstraints.max = 0; tmpConstraints.max = 0;
				tmpConstraints.min = (IsLockedItem(item1) ? GetMinConstraintFromSize(item1, layoutType) : GetMinConstraintFromSize(item2, layoutType)) + dif;
				int min = IsLockedItem(item1) ? GetConstraint(item1, layoutType).min : GetConstraint(item2, layoutType).min; ;
				if(tmpConstraints.min < min) {
					tmpConstraints.min = min;
				}
			}
			int realdif = item1.ChangeItemSize(item, dif, layoutType, tmpConstraints);
			item2.ChangeItemSize(null, realdif, layoutType,
				new Constraints(item1, layoutType));
			UpdateSize(layoutType);
			return realdif;
		}
		private int GetMinConstraintFromSize(BaseLayoutItem item, LayoutType layoutType) {
			return new LayoutSize(item.Size, layoutType).Width; 
		}
		private static bool IsLockedItem(BaseLayoutItem item) {
			return (item is DashboardLayoutControlItemBase) && (item as DashboardLayoutControlItemBase).IsLocked;
		}
		private static bool IsLockedItems(BaseLayoutItem item1, BaseLayoutItem item2) {
			return (IsLockedItem(item1) || IsLockedItem(item2));
		}
	}
	public class GroupResizeGroup2 :GroupResizeGroup {
		public GroupResizeGroup2(LayoutType layoutType, BaseLayoutItem item, BaseLayoutItem group) : base(layoutType, item, group) { }
		protected override LayoutSize ChangeExpandedGroupItemSize(LayoutType layoutType, LayoutSize newGroupSize) {
			SetInternalSize(newGroupSize.Size);
			return newGroupSize;
		}
	}
	public class Resizer2 : Resizer {
		public Resizer2(LayoutGroup group) : base(group) { }
		public Edge SizingEdge { get; set; }
	 protected static ItemRelation GetItemRelations(BaseLayoutItem item1, BaseLayoutItem item2) {
			LayoutRectangle item1Rect = new LayoutRectangle(item1.Bounds, LayoutType.Horizontal);
			LayoutRectangle item2Rect = new LayoutRectangle(item2.Bounds, LayoutType.Horizontal);
			if(item1Rect.X == item2Rect.X && item1Rect.Width == item2Rect.Width) {
				if(item1Rect.Bottom == item2Rect.Top) return ItemRelation.Bottom;
				if(item2Rect.Bottom == item1Rect.Top) return ItemRelation.Top;
			}
			item1Rect = new LayoutRectangle(item1.Bounds, LayoutType.Vertical);
			item2Rect = new LayoutRectangle(item2.Bounds, LayoutType.Vertical);
			if(item1Rect.X == item2Rect.X && item1Rect.Width == item2Rect.Width) {
				if(item1Rect.Bottom == item2Rect.Top) return ItemRelation.Right;
				if(item2Rect.Bottom == item1Rect.Top) return ItemRelation.Left;
			}
			return ItemRelation.None;
		}
		protected override HorizontalResizeGroup CreateHorizontalResizeGruopInstance(BaseLayoutItem item1, BaseLayoutItem item2, LayoutType layoutType) {
			return new HorizontalResizeGroup2(item1, item2, layoutType);
		}
		protected override VerticalResizeGroup CreateVerticalResizeGruopInstance(BaseLayoutItem item1, BaseLayoutItem item2, LayoutType layoutType) {
			return new VerticalResizeGroup2(item1, item2, layoutType);
		}
		protected virtual void Reduce(BaseLayoutItem item1, BaseLayoutItem item2, List<BaseLayoutItem> items, LayoutType layoutType, bool createHRG) {
			items.Remove(item1);
			items.Remove(item2);
			BaseLayoutItem groupedItem1 = GroupItems(layoutType, new List<BaseLayoutItem>() { item1 });
			BaseLayoutItem groupedItem2 = GroupItems(layoutType, new List<BaseLayoutItem>() { item2 });
			BaseLayoutItem newItem;
			if(createHRG)
				newItem = CreateHorizontalResizeGruop(groupedItem1, groupedItem2, layoutType);
			else
				newItem = CreateVerticalResizeGruopInstance(groupedItem1, groupedItem2, layoutType);
			items.Add(newItem);
		}
		protected delegate bool ReduceRule(ItemRelation relation, LayoutType layoutType);
		protected delegate void ReduceAction(BaseLayoutItem item1, BaseLayoutItem item2, List<BaseLayoutItem> items, LayoutType layoutType, bool createHRG);
		protected bool ReduceByRule(List<BaseLayoutItem> items, LayoutType layoutType, bool createHRG, ReduceRule rule, ReduceAction action) {
			int watchdog = 4000;
			while(ReduceByRuleCore(items, layoutType, createHRG, rule, action)) {
				watchdog--;
				if(watchdog == 0) throw new LayoutControlInternalException("resizer watchdog");
			}
			return watchdog < 4000;
		}
		protected bool ReduceByRuleCore(List<BaseLayoutItem> items, LayoutType layoutType, bool createHRG, ReduceRule rule, ReduceAction action) {
			if(items.Count < 2) return false;
			List<BaseLayoutItem> itemsCopy = new List<BaseLayoutItem>(items);
			foreach(BaseLayoutItem item1 in itemsCopy) {
				foreach(BaseLayoutItem item2 in itemsCopy) {
					if(item1 == item2) continue;
					ItemRelation relation = GetItemRelations(item1, item2);
					if(rule(relation, layoutType)) {
						action(item1, item2, items, layoutType, createHRG);
						return true;
					}
				}
			}
			return false;
		}
	   internal bool CreateHRG_H_Rule(ItemRelation relation, LayoutType layoutType) {
			return relation == ItemRelation.Right && layoutType == LayoutType.Horizontal;
		}
	   internal bool CreateHRG_V_Rule(ItemRelation relation, LayoutType layoutType) {
			return relation == ItemRelation.Bottom && layoutType == LayoutType.Vertical;
		}
	   internal bool CreateVRG_H_Rule(ItemRelation relation, LayoutType layoutType) {
			return relation == ItemRelation.Bottom && layoutType == LayoutType.Horizontal;
		}
	   internal bool CreateVRG_V_Rule(ItemRelation relation, LayoutType layoutType) {
			return relation == ItemRelation.Right && layoutType == LayoutType.Vertical;
		}
		protected internal override BaseLayoutItem GroupTwoItems(List<BaseLayoutItem> items, LayoutType layoutType, bool invert) {
			if(items.Count == 1) return items[0];
			if(layoutType == LayoutType.Horizontal) {
				bool wasReduced;
				do {
					wasReduced = false;
					wasReduced |= ReduceByRule(items, layoutType, true, CreateHRG_H_Rule, Reduce);
					if(items.Count == 1) return items[0];
					wasReduced |= ReduceByRule(items, layoutType, false, CreateVRG_H_Rule, Reduce);
					if(items.Count == 1) return items[0];
				}
				while(wasReduced);
			} else {
				bool wasReduced;
				do {
					wasReduced = false;
					wasReduced |= ReduceByRule(items, layoutType, false, CreateVRG_V_Rule, Reduce);
					if(items.Count == 1) return items[0];
					wasReduced |= ReduceByRule(items, layoutType, true, CreateHRG_V_Rule, Reduce);
					if(items.Count == 1) return items[0];
				}
				while(wasReduced);
			}
			return null;
		}
	}
	public class Edge {
		Rectangle edgeBoundsCore;
		LayoutType layoutType;
		public Edge(BaseLayoutItemHitInfo hitInfo, LayoutType layoutType) {
			this.layoutType = layoutType;
			LayoutRectangle temp = new LayoutRectangle(hitInfo.Item.Bounds, layoutType);
			LayoutPoint lp = new LayoutPoint(hitInfo.HitPoint, temp.LayoutType);
			InsertLocation insertLocaton = InsertLocation.After;
			if(lp.X < (temp.X + temp.Width / 2)) { insertLocaton = InsertLocation.Before; }
			if(insertLocaton == InsertLocation.After) {
				temp.X = temp.X + temp.Width;
			}
			temp.Width = 0;
			edgeBoundsCore = temp.Rectangle;
		}
		public Rectangle EdgeBounds { get { return edgeBoundsCore; } }
		HorizontalResizeGroup cachedGroup = null;
		internal bool IsEdgeForGroup(HorizontalResizeGroup group) {
			if(cachedGroup != null && cachedGroup == group) return true;
			if(group.layoutType != layoutType) return false;
			LayoutRectangle item1Rect, item2Rect, edgeRect;
			item1Rect = new LayoutRectangle(group.Item1.Bounds, group.layoutType);
			item2Rect = new LayoutRectangle(group.Item2.Bounds, group.layoutType);
			edgeRect = new LayoutRectangle(edgeBoundsCore, group.layoutType);
			if(
				(edgeRect.Y <= item1Rect.Y && edgeRect.Bottom >= item1Rect.Bottom || item1Rect.Y <= edgeRect.Y && item1Rect.Bottom >= edgeRect.Bottom) &&
				item1Rect.Right == edgeRect.X && item2Rect.X == edgeRect.X) {
				cachedGroup = group;
				return true;
			}
			return false;
		}
	}
	public class ResizeManager2 : ResizeManager {
		public ResizeManager2(LayoutGroup owner) : base(owner) { }
		protected override Resizer CreateResizer(LayoutGroup ownerGroupCore) {
			return new ResizerWithCrosshair(ownerGroupCore);
		}
	}
}																														   
