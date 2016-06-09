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

using DevExpress.XtraDashboardLayout;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.Resizing;
using DevExpress.XtraLayout.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraLayout.Resizing {
	public class ResizerWithCrosshair : Resizer2 {
		public ResizerWithCrosshair(LayoutGroup group) : base(group) { }
		public GroupResizeGroup2 VResizerTree { get { return GetValidResizerTree(LayoutType.Vertical) as GroupResizeGroup2; } }
		public GroupResizeGroup2 HResizerTree { get { return GetValidResizerTree(LayoutType.Horizontal) as GroupResizeGroup2; } }
		BaseLayoutItem GetValidResizerTree(LayoutType layoutType) {
			return GetGroupResizeGroup(layoutType, group);
		}
		BaseLayoutItem GetGroupResizeGroup(LayoutType layoutType, LayoutGroup group) {
			List<BaseLayoutItem> items = new List<BaseLayoutItem>(group.Items);
			foreach(BaseLayoutItem item in group.Items) {
				var childGroup = item as LayoutControlGroup;
				if(childGroup != null) {
					items.Remove(childGroup);
					items.Add(GetGroupResizeGroup(layoutType, childGroup));
				}
			}
			return GetValidResizerTreeCore(layoutType, group, items);
		}
		BaseLayoutItem GetValidResizerTreeCore(LayoutType layoutType, LayoutGroup group,List<BaseLayoutItem> items) {
			var dashboardRootGroup = group as DashboardLayoutControlGroupBase;
			if(dashboardRootGroup == null) {
				return GroupItems(layoutType, new List<BaseLayoutItem>(new BaseLayoutItem[] { group }));
			}
			if(dashboardRootGroup.CrosshairCollection.Count == 0) {
				if(items.Count == 0) return GroupItems(layoutType, new List<BaseLayoutItem>(new BaseLayoutItem[] { dashboardRootGroup }));
				else return new GroupResizeGroup2(layoutType, GroupItems(layoutType, items), dashboardRootGroup);
			}
			ResizerGroupingList prevGroupingList = new ResizerGroupingList(items);
			ResizerGroupingList groupingList = new ResizerGroupingList(items);
			List<Crosshair> copyCrosshairList = new List<Crosshair>(dashboardRootGroup.CrosshairCollection);
			for(int i = 0; i < copyCrosshairList.Count; i++) {
				var crosshair = copyCrosshairList[i];
				if(crosshair.CrosshairGroupingType == CrosshairGroupingTypes.NotSet) continue;
				IntersectionInfo typeOfIntersection = GetIntersectionInfo(crosshair, groupingList, layoutType);
				switch(typeOfIntersection.Intersection) {
					case TypeOfIntersection.NonSet:
						var nonSetGroup = GroupTwoCrosshairItems(crosshair, false, layoutType);
						if(nonSetGroup == null) {
							i = RemoveCrosshair(copyCrosshairList, i, crosshair);
							break;
						}
						groupingList.Remove(nonSetGroup.Item1);
						groupingList.Remove(nonSetGroup.Item2);
						groupingList.Add(nonSetGroup);
						break;
					case TypeOfIntersection.Conflict:
						i = RemoveCrosshair(copyCrosshairList, i, crosshair);
						continue;
					case TypeOfIntersection.SameItems_Left:
						RemoveTwoItemsFromList(groupingList, crosshair.LeftTopItem, crosshair.LeftBottomItem);
						break;
					case TypeOfIntersection.SameItems_Right:
						RemoveTwoItemsFromList(groupingList, crosshair.RightTopItem, crosshair.RightBottomItem);
						break;
					case TypeOfIntersection.SameItems_Top:
						RemoveTwoItemsFromList(groupingList, crosshair.LeftTopItem, crosshair.RightTopItem);
						break;
					case TypeOfIntersection.SameItems_Bottom:
						RemoveTwoItemsFromList(groupingList, crosshair.LeftBottomItem, crosshair.RightBottomItem);
						break;
					case TypeOfIntersection.UnionFirst_RoB:
						UnionCrosshairItems(layoutType, groupingList, crosshair, typeOfIntersection.PositionInGroupList, true, 1, 0);
						break;
					case TypeOfIntersection.UnionFirst_NotRoB:
						UnionCrosshairItems(layoutType, groupingList, crosshair, typeOfIntersection.PositionInGroupList, false, 1, 0);
						break;
					case TypeOfIntersection.UnionSecond_RoB:
						UnionCrosshairItems(layoutType, groupingList, crosshair, typeOfIntersection.PositionInGroupList, true, 0, 1);
						break;
					case TypeOfIntersection.UnionSecond_NotRoB:
						UnionCrosshairItems(layoutType, groupingList, crosshair, typeOfIntersection.PositionInGroupList, false, 0, 1);
						break;																																	  
					case TypeOfIntersection.RightOrBottom:
						var RoBGroup = GroupTwoCrosshairItems(crosshair, true, layoutType);
						if(RoBGroup == null) {
							i = RemoveCrosshair(copyCrosshairList, i, crosshair);
							break;
						}
						groupingList.Remove(RoBGroup.Item1);
						groupingList.Remove(RoBGroup.Item2);
						groupingList.Add(RoBGroup);
						break;
				}
				CheckGroupingList(layoutType, ref prevGroupingList, ref groupingList, copyCrosshairList, ref i, crosshair);
			}
			BaseLayoutItem item = GroupTwoItems(groupingList, layoutType, false);
			FillRealCrosshairGroupingType(layoutType, dashboardRootGroup, copyCrosshairList, item);
#if DEBUGTEST
			if(item == null) NUnit.Framework.Assert.Fail("Wrong build crosshair");
#endif
			return new GroupResizeGroup2(layoutType, item, dashboardRootGroup);
		}
	   internal static void FillRealCrosshairGroupingType(LayoutType layoutType, DashboardLayoutControlGroupBase dashboardRootGroup, List<Crosshair> copyCrosshairList, BaseLayoutItem item) {
			if(item == null) return;
			if(layoutType == LayoutType.Horizontal) {
				FlatItemsList fil = new FlatItemsList();
				List<BaseLayoutItem> listTree = fil.GetItemsList(item);
				foreach(Crosshair cross in dashboardRootGroup.CrosshairCollection) {
				   SetRealCrossType(listTree, cross);
				}
			}
		}
		private static void SetRealCrossType(List<BaseLayoutItem> listTree, Crosshair cross) {
			if(cross.CrosshairGroupingType == CrosshairGroupingTypes.GroupBoth && cross.realCrosshairTypeCore != CrosshairGroupingTypes.NotSet) return;
			foreach(BaseLayoutItem bli in listTree) {
				if(!(bli is ResizeGroup)) continue;
				Rectangle tempRect = bli.Bounds;
				if(tempRect.X == cross.VerticalLine.Location.X || tempRect.X + tempRect.Width == cross.VerticalLine.Location.X) {
					if(tempRect.Y <= cross.VerticalLine.Location.Y && tempRect.Height >= cross.VerticalLine.Length) {
						cross.realCrosshairTypeCore = CrosshairGroupingTypes.GroupVertical;
						break;
					}
				}
				if(tempRect.Y == cross.HorizontalLine.Location.Y || tempRect.Y + tempRect.Height == cross.HorizontalLine.Location.Y) {
					if(tempRect.X <= cross.HorizontalLine.Location.X && tempRect.Width >= cross.HorizontalLine.Length) {
						cross.realCrosshairTypeCore = CrosshairGroupingTypes.GroupHorizontal;
						break;
					}
				}
			}
		}
		void CheckGroupingList(LayoutType layoutType, ref ResizerGroupingList prevGroupingList, ref ResizerGroupingList groupingList, List<Crosshair> copyCrosshairList, ref int i, Crosshair crosshair) {
			if(GroupTwoItems(new List<BaseLayoutItem>(groupingList), layoutType, false) == null) {
				groupingList = new ResizerGroupingList(prevGroupingList);
				i = RemoveCrosshair(copyCrosshairList, i, crosshair);
			} else {
				prevGroupingList = new ResizerGroupingList(groupingList);
			}
		}
		static void RemoveTwoItemsFromList(List<BaseLayoutItem> groupingList, BaseLayoutItem firstItem, BaseLayoutItem secondItem) {
			groupingList.Remove(firstItem);
			groupingList.Remove(secondItem);
		}
		void UnionCrosshairItems(LayoutType layoutType, List<BaseLayoutItem> groupingList, Crosshair crosshair, int PositionInGroupList, bool RoB, int itemToRemove, int itemToAdd) {
			List<BaseLayoutItem> tempList1 = new List<BaseLayoutItem>();
			tempList1.Add(groupingList[PositionInGroupList]);
			tempList1.Add(crosshair.GetTwoItems(RoB,layoutType)[itemToAdd]);
			ResizeGroup rGroup1 = GroupTwoItems(tempList1, layoutType, false) as ResizeGroup;
			groupingList.Remove(crosshair.GetTwoItems(RoB,layoutType)[itemToRemove]);
			groupingList.Remove(rGroup1.Item1);
			groupingList.Remove(rGroup1.Item2);
			groupingList.Add(rGroup1);
		}
		void GroupBottomOrRightCrosshairItems(List<BaseLayoutItem> groupingList, Crosshair crosshair, LayoutType ltype) {
			var borromOrRightGroup = GroupTwoCrosshairItems(crosshair, true, ltype);
			groupingList.Remove(borromOrRightGroup.Item1);
			groupingList.Remove(borromOrRightGroup.Item2);
			groupingList.Add(borromOrRightGroup);
		}
		static int RemoveCrosshair(List<Crosshair> copyCrosshairList, int i, Crosshair crosshair) {
			copyCrosshairList.Remove(crosshair);
			i--;
			return i;
		}
		IntersectionInfo GetIntersectionInfo(Crosshair crosshair, List<BaseLayoutItem> groupingList, LayoutType lType) {
			TypeOfIntersection typeRoot = TypeOfIntersection.NonSet;
			int position = -1;
			foreach(BaseLayoutItem bli in groupingList) {
				position++;
				var resizerGroup = bli as ResizeGroup;
				if(resizerGroup == null) continue;
				TypeOfIntersection type = GetTypeOfIntersection(crosshair, resizerGroup, lType);
				if(type != TypeOfIntersection.NonSet) {
					typeRoot = type;
					break;
				}
			}
			IntersectionInfo info = new IntersectionInfo() { Intersection = typeRoot, PositionInGroupList = position };
			return info;
		}
		static TypeOfIntersection GetTypeOfIntersection(Crosshair crosshair, ResizeGroup resizerGroup,LayoutType ltype) {
			if(resizerGroup.Item1.Equals(crosshair.LeftTopItem) && resizerGroup.Item2.Equals(crosshair.LeftBottomItem)) return TypeOfIntersection.SameItems_Left;
			if(resizerGroup.Item1.Equals(crosshair.LeftTopItem) && resizerGroup.Item2.Equals(crosshair.RightTopItem)) return TypeOfIntersection.SameItems_Top;
			if(resizerGroup.Item1.Equals(crosshair.LeftBottomItem) && resizerGroup.Item2.Equals(crosshair.RightBottomItem)) return TypeOfIntersection.SameItems_Bottom;
			if(resizerGroup.Item1.Equals(crosshair.RightTopItem) && resizerGroup.Item2.Equals(crosshair.RightBottomItem)) return TypeOfIntersection.SameItems_Right;
			List<BaseLayoutItem> NotRoBList = crosshair.GetTwoItems(false,ltype);
			List<BaseLayoutItem> RoBList = crosshair.GetTwoItems(true,ltype);
			ItemRelation iRelationFirstNotRoB = GetItemRelations(resizerGroup, NotRoBList[0]);
			ItemRelation iRelationSecondNotRoB = GetItemRelations(resizerGroup, NotRoBList[1]);
			ItemRelation iRelationFirstRoB = GetItemRelations(resizerGroup, RoBList[0]);
			ItemRelation iRelationSecondRoB = GetItemRelations(resizerGroup, RoBList[1]);
			if(resizerGroup.Bounds.Contains(RoBList[0].Bounds) && (iRelationSecondRoB == ItemRelation.Right || iRelationSecondRoB == ItemRelation.Bottom)) return TypeOfIntersection.UnionSecond_RoB;
			if(resizerGroup.Bounds.Contains(NotRoBList[0].Bounds) && (iRelationSecondNotRoB == ItemRelation.Right || iRelationSecondNotRoB == ItemRelation.Bottom)) return TypeOfIntersection.UnionSecond_NotRoB;
			if(resizerGroup.Bounds.Contains(RoBList[1].Bounds) && (iRelationFirstRoB == ItemRelation.Left || iRelationFirstRoB == ItemRelation.Top)) return TypeOfIntersection.UnionFirst_RoB;
			if(resizerGroup.Bounds.Contains(NotRoBList[1].Bounds) && (iRelationFirstNotRoB == ItemRelation.Left || iRelationFirstNotRoB == ItemRelation.Top)) return TypeOfIntersection.UnionFirst_NotRoB;
			if(!resizerGroup.Bounds.Contains(NotRoBList[0].Bounds) && !resizerGroup.Bounds.Contains(NotRoBList[1].Bounds)) return TypeOfIntersection.NonSet;
			if(!resizerGroup.Bounds.Contains(RoBList[0].Bounds) && !resizerGroup.Bounds.Contains(RoBList[1].Bounds)) return TypeOfIntersection.RightOrBottom;
			return TypeOfIntersection.Conflict;
		}
		ResizeGroup GroupTwoCrosshairItems(Crosshair crosshair, bool bottomOrRightItems, LayoutType lType) {
			ResizeGroup bli = null;
			List<BaseLayoutItem> items = crosshair.GetTwoItems(bottomOrRightItems,lType);
			bli = GroupTwoItems(items, lType, false) as ResizeGroup;
			return bli;
		}
		public override void CreateNewResizing() {
			resultH = null;
			resultV = null;
			BaseLayoutItem tresultH = GetValidResizerTree(LayoutType.Horizontal);
			BaseLayoutItem tresultV = GetValidResizerTree(LayoutType.Vertical);
			if(tresultH != null) resultH = tresultH;
			if(tresultV != null) resultV = tresultV;
			UpdateConstraints();
		}
	}
	internal class IntersectionInfo {
		internal TypeOfIntersection Intersection { get; set; }
		internal int PositionInGroupList { get; set; }
	}
	public enum TypeOfIntersection {
		UnionSecond_NotRoB,
		UnionFirst_NotRoB,
		UnionSecond_RoB,
		UnionFirst_RoB,
		RightOrBottom,
		SameItems_Left,
		SameItems_Right,
		SameItems_Top,
		SameItems_Bottom,
		Conflict,
		NonSet
	}
	internal static class Resizer2GroupType {
		internal static string Vertical = "DevExpress.XtraLayout.Resizing.VerticalResizeGroup2";
		internal static string Horizontal = "DevExpress.XtraLayout.Resizing.HorizontalResizeGroup2";
	}
	public class ResizerGroupingList : List<BaseLayoutItem> {
		public ResizerGroupingList(List<BaseLayoutItem> items) : base(items) { }
		public new void Remove(BaseLayoutItem bli){																					 
			if(bli is GroupResizeGroup) {
				for(int i = 0; i < Count; i++) {
					if(bli.Bounds == this[i].Bounds) {
						RemoveAt(i);
						return;
					}
				}
			}
			else{
				base.Remove(bli);
			}
		}
	}
}
