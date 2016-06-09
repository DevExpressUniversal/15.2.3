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
using System.Drawing;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.ExpressApp.Win.Layout {
#if DebugTest
	public
#endif
	sealed class XafLayoutStyleManager : LayoutStyleManager {
		private XafLayoutConstants xafLayoutConstants = new XafLayoutConstants();
		private readonly Dictionary<BaseLayoutItem, Padding> itemsPaddingCache = new Dictionary<BaseLayoutItem, Padding>();
		private readonly Dictionary<LayoutGroup, bool> needAlignWithChildrenCache = new Dictionary<LayoutGroup, bool>();
		public XafLayoutStyleManager(ILayoutControl control)
			: base(control) {
			control.Changed += control_Changed;
		}
		private void ClearCache() {
			needAlignWithChildrenCache.Clear();
			itemsPaddingCache.Clear();
		}
		public override TextAlignModeGroup CorrectGroupTextAlignMode(LayoutGroup item, TextAlignModeGroup proposedValue) {
			if(Owner.OptionsView.PaddingSpacingMode != PaddingMode.MSGuidelines) {
				return proposedValue;
			}
			return NeedAlignWithChildren(item) ? TextAlignModeGroup.AlignWithChildren : proposedValue;
		}
		private bool NeedAlignWithChildren(LayoutGroup group) {
			if(group.OptionsItemText.AlignControlsWithHiddenText) {
				return false;
			}
			bool needAlignWithChildren;
			if(!needAlignWithChildrenCache.TryGetValue(group, out needAlignWithChildren)) {
				foreach(BaseLayoutItem item in group.Items) {
					if(item is SimpleLabelItem) {
						needAlignWithChildren = false;
						break;
					}
					if(item is EmptySpaceItem) {
						continue;
					}
					if(item is LayoutControlItem) {
						needAlignWithChildren = true;
						if(item.TextVisible) {
							needAlignWithChildren = false;
							break;
						}
					}
				}
				needAlignWithChildrenCache.Add(group, needAlignWithChildren);
			}
			return needAlignWithChildren;
		}
		protected override Padding CalcMSGuidelinesPaddingSpacing(BaseLayoutItem item, bool calcSpacing) {
			if(item == null || !calcSpacing) return Padding.Empty;
			Padding result;
			if(this.control.RootGroup.IsUpdateLocked) {
				ClearCache();
			}
			if(!itemsPaddingCache.TryGetValue(item, out result)) {
				LayoutClassificationArgs itemClassification = LayoutClassifier.Default.Classify(item);
				if(item == null || (itemClassification.IsGroup && !itemClassification.Group.GroupBordersVisible)) {
					return Padding.Empty;
				}
				LayoutGroup nearestGroupWithVisibleBorder = FindNearestGroupWithVisibleBorder(item);
				if(IsRootGroup(nearestGroupWithVisibleBorder) && itemClassification.IsGroup && !itemClassification.Group.GroupBordersVisible) {
					return Padding.Empty;
				}
				if(IsOneItemOnTabPage(item)) {
					return new Padding(XafLayoutConstants.ItemToTabBorderDistance);
				}
				LayoutClassificationArgs itemParentClassification = LayoutClassifier.Default.Classify(item.Parent);
				result = CalcItemSpacing(item, nearestGroupWithVisibleBorder.Items);
				if(itemParentClassification.IsGroup && !itemParentClassification.Group.GroupBordersVisible) {
					SeparateInvisibleGroup(item, nearestGroupWithVisibleBorder, ref result);
				}
				itemsPaddingCache[item] = result;
			}
			return result;
		}
		internal XafLayoutConstants XafLayoutConstants {
			get { return xafLayoutConstants; }
			set { xafLayoutConstants = value; }
		}
		public static Point CalcItemPositionInLayout(BaseLayoutItem item) {
			Point pos = new Point(item.X, item.Y);
			BaseLayoutItem itemTemp = item;
			while(itemTemp.Parent != null) {
				itemTemp = itemTemp.Parent;
				if(!itemTemp.Location.IsEmpty) {
					pos.X += itemTemp.X;
					pos.Y += itemTemp.Y;
				}
			}
			return pos;
		}
		private bool IsRootGroup(LayoutGroup group) {
			return (group.Parent == null);
		}
		private LayoutGroup FindNearestGroupWithVisibleBorder(BaseLayoutItem item) {
			BaseLayoutItem parent = item.Parent;
			LayoutClassificationArgs parentItemClassification = LayoutClassifier.Default.Classify(parent);
			while(parent != null && parentItemClassification.IsGroup && !parentItemClassification.Group.GroupBordersVisible) {
				parent = parent.Parent;
				parentItemClassification = LayoutClassifier.Default.Classify(parent);
			}
			if(parent == null) {
				return item.Owner.RootGroup;
			}
			return parentItemClassification.Group;
		}
		private bool IsOneItemOnTabPage(BaseLayoutItem item) {
			if(item != null && item.Parent != null) {
				LayoutClassificationArgs itemParentClassification = LayoutClassifier.Default.Classify(item.Parent);
				return (itemParentClassification.IsGroup
					&& itemParentClassification.Group.Items.Count == 1
					&& itemParentClassification.IsTabPage);
			}
			return false;
		}
		internal static ItemPosition CalcItemPosition(BaseItemCollection items, BaseLayoutItem item) {
			Point itemPositionInLayout = CalcItemPositionInLayout(item);
			int minPos_Y = itemPositionInLayout.Y;
			int maxPos_Y = itemPositionInLayout.Y + item.Height;
			int minPos_X = itemPositionInLayout.X;
			int maxPos_X = itemPositionInLayout.X + item.Width;
			int itemMinPos_Y = minPos_Y;
			int itemMaxPosY = maxPos_Y;
			int itemMinPos_X = minPos_X;
			int itemMaxPos_X = maxPos_X;
			foreach(BaseLayoutItem groupItem in items) {
				if(groupItem.Visibility == LayoutVisibility.Always) {
					Point groupItemPositionInLayout = CalcItemPositionInLayout(groupItem);
					int groupItemPos = groupItemPositionInLayout.Y;
					if(minPos_Y > groupItemPos) {
						minPos_Y = groupItemPos;
					}
					groupItemPos += groupItem.Height;
					if(maxPos_Y < groupItemPos) {
						maxPos_Y = groupItemPos;
					}
					groupItemPos = groupItemPositionInLayout.X;
					if(minPos_X > groupItemPos) {
						minPos_X = groupItemPos;
					}
					groupItemPos += groupItem.Width;
					if((groupItemPos - maxPos_X) > 1) {
						maxPos_X = groupItemPos;
					}
				}
			}
			return new ItemPosition(Math.Abs(itemMinPos_Y - minPos_Y) <= 1,
				Math.Abs(itemMaxPos_X - maxPos_X) <= 1,
				Math.Abs(itemMinPos_X - minPos_X) <= 1,
				Math.Abs(itemMaxPosY - maxPos_Y) <= 1);
		}
		private bool IsValidInvisibleGroup(BaseLayoutItem item) {
			LayoutClassificationArgs itemClassification = LayoutClassifier.Default.Classify(item);
			if(itemClassification.IsGroup && !itemClassification.Group.GroupBordersVisible && itemClassification.Group.Items.Count > 0) {
				return true;
			}
			return false;
		}
		private bool IsInvisibleGroupAbove(LayoutGroup parentGroup, BaseLayoutItem item) {
			if(parentGroup != null) {
				BaseLayoutItem aboveItem = null;
				int aboveItemDistance = int.MaxValue;
				foreach(BaseLayoutItem groupItem in parentGroup.Items) {
					if((groupItem.Y < item.Y) && ((item.Y - groupItem.Y) < aboveItemDistance)) {
						aboveItemDistance = item.Y - groupItem.Y;
						aboveItem = groupItem;
					}
				}
				return IsValidInvisibleGroup(aboveItem);
			}
			return false;
		}
		private bool IsInvisibleGroupBelow(LayoutGroup parentGroup, BaseLayoutItem item) {
			if(parentGroup != null) {
				BaseLayoutItem belowItem = null;
				int belowItemDistance = int.MaxValue;
				foreach(BaseLayoutItem groupItem in parentGroup.Items) {
					if((groupItem.Y > item.Y) && ((groupItem.Y - item.Y) < belowItemDistance)) {
						belowItemDistance = groupItem.Y - item.Y;
						belowItem = groupItem;
					}
				}
				return IsValidInvisibleGroup(belowItem);
			}
			return false;
		}
		private Padding CalcItemSpacing(BaseLayoutItem item, BaseItemCollection items) {
			int itemToBorderHorizontal = XafLayoutConstants.ItemToBorderHorizontalDistance;
			int itemToBorderVertical = XafLayoutConstants.ItemToBorderVerticalDistance;
			DevExpress.Skins.SkinElementInfo groupSkin = new DevExpress.Skins.SkinElementInfo(DevExpress.Skins.CommonSkins.GetSkin(item.Owner.LookAndFeel)[DevExpress.Skins.CommonSkins.SkinGroupPanel], Rectangle.Empty);
			if(item is LayoutGroup && groupSkin.Element.Image == null) {
				itemToBorderHorizontal = 0;
				itemToBorderVertical = 0;
			}
			Padding result = new Padding(XafLayoutConstants.ItemToItemHorizontalDistance / 2, XafLayoutConstants.ItemToItemHorizontalDistance / 2, XafLayoutConstants.ItemToItemVerticalDistance / 2, XafLayoutConstants.ItemToItemVerticalDistance / 2);
			ItemPosition itemPosition = CalcItemPosition(items, item);
			if(itemPosition.IsLeftItemInItems) {
				result.Left = itemToBorderHorizontal; 
			}
			if(itemPosition.IsRightItemInItems) {
				result.Right = itemToBorderHorizontal; 
			}
			if(itemPosition.IsTopItemInItems) {
				result.Top = XafLayoutConstants.ItemToBorderVerticalDistance;
			}
			if(itemPosition.IsBottomItemInItems) {
				result.Bottom = itemToBorderVertical; 
			}
			return result;
		}
		private void SeparateInvisibleGroup(BaseLayoutItem item, LayoutGroup nearestGroupWithVisibleBorder, ref Padding result) {
			LayoutClassificationArgs itemParentClassification = LayoutClassifier.Default.Classify(item.Parent);
			LayoutClassificationArgs itemParentParentClassification = LayoutClassifier.Default.Classify(item.Parent.Parent);
			ItemPosition itemPosition = CalcItemPosition(itemParentClassification.Group.Items, item);
			ItemPosition groupPosition;
			if(itemPosition.IsTopItemInItems || itemPosition.IsBottomItemInItems) {
				groupPosition = CalcItemPosition(nearestGroupWithVisibleBorder.Items, itemParentClassification.Group);
			}
			else {
				groupPosition = null;
			}
			if(itemPosition.IsTopItemInItems) {
				if(groupPosition != null && groupPosition.IsTopItemInItems) {
					result.Top = XafLayoutConstants.ItemToBorderVerticalDistance;
				}
				else {
					if(IsInvisibleGroupAbove(itemParentParentClassification.Group, itemParentClassification.Group)) {
						result.Top = XafLayoutConstants.InvisibleGroupVerticalDistance / 2;
					}
					else {
						result.Top = XafLayoutConstants.InvisibleGroupVerticalDistance - XafLayoutConstants.ItemToItemVerticalDistance / 2;
					}
				}
			}
			if(itemPosition.IsBottomItemInItems) {
				if(groupPosition != null && groupPosition.IsBottomItemInItems) {
					result.Bottom = XafLayoutConstants.ItemToBorderVerticalDistance;
				}
				else {
					if(IsInvisibleGroupBelow(itemParentParentClassification.Group, itemParentClassification.Group)) {
						result.Bottom = XafLayoutConstants.InvisibleGroupVerticalDistance / 2;
					}
					else {
						result.Bottom = XafLayoutConstants.InvisibleGroupVerticalDistance - XafLayoutConstants.ItemToItemVerticalDistance / 2;
					}
				}
			}
		}
		private void control_Changed(object sender, EventArgs e) {
			ClearCache();
		}
		internal class ItemPosition {
			private readonly bool isTopItemInItems;
			private readonly bool isRightItemInItems;
			private readonly bool isLeftItemInItems;
			private readonly bool isBottomItemInItems;
			public ItemPosition(bool isTopItemInItems, bool isRightItemInItems, bool isLeftItemInItems, bool isBottomItemInItems) {
				this.isTopItemInItems = isTopItemInItems;
				this.isRightItemInItems = isRightItemInItems;
				this.isLeftItemInItems = isLeftItemInItems;
				this.isBottomItemInItems = isBottomItemInItems;
			}
			public bool IsTopItemInItems {
				get {
					return isTopItemInItems;
				}
			}
			public bool IsRightItemInItems {
				get {
					return isRightItemInItems;
				}
			}
			public bool IsLeftItemInItems {
				get {
					return isLeftItemInItems;
				}
			}
			public bool IsBottomItemInItems {
				get {
					return isBottomItemInItems;
				}
			}
		}
#if DebugTest
		public bool IsTopItemInItems(BaseItemCollection items, BaseLayoutItem item) {
			ItemPosition result = CalcItemPosition(items, item);
			return result.IsTopItemInItems;
		}
		public bool IsBottomItemInItems(BaseItemCollection items, BaseLayoutItem item) {
			ItemPosition result = CalcItemPosition(items, item);
			return result.IsBottomItemInItems;
		}
		public bool IsLeftItemInItems(BaseItemCollection items, BaseLayoutItem item) {
			ItemPosition result = CalcItemPosition(items, item);
			return result.IsLeftItemInItems;
		}
		public bool IsRightItemInItems(BaseItemCollection items, BaseLayoutItem item) {
			ItemPosition result = CalcItemPosition(items, item);
			return result.IsRightItemInItems;
		}
#endif
	}
}
