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
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Resizing;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Customization;
using System.Collections.Generic;
namespace DevExpress.XtraLayout.Adapters {
	public class XAFLayoutItemInfo {
		BaseLayoutItem item;
		double relativeSize;
		Rectangle bounds;
		String idCore;
		bool allowGroupsCore = true;
		XAFLayoutItemInfo parent = null;
		public BaseLayoutItem Item {
			get { return item; }
			set { item = value; }
		}
		public bool AllowGroups {
			get { return allowGroupsCore; }
			set { allowGroupsCore = value; }
		}
		public String ID {
			get { return idCore; }
			set { idCore = value; }
		}
		public String ParentID {
			get { return parent.ID; }
		}
		public XAFLayoutItemInfo Parent {
			get { return parent; }
			set { parent = value; }
		}
		public virtual Rectangle Bounds {
			get {
				return bounds;
			}
			set {
				bounds = value;
			}
		}
		public double RelativeSize {
			get { return relativeSize; }
			set { relativeSize = value; }
		}
	}
	public class XAFLayoutGroupInfo : XAFLayoutItemInfo {
		ArrayList items;
		bool groupBoundsVisible = true;
		LayoutType layoutType;
		string tGroupID = "";
		XAFTabbedGroupInfo tGroup = null;
		public XAFLayoutGroupInfo() {
			items = new ArrayList();
		}
		public new LayoutGroup Item {
			get { return base.Item as LayoutGroup; }
			set { base.Item = value; }
		}
		public XAFTabbedGroupInfo TabbedGroupParent {
			get {
				return tGroup;
			}
			set {
				tGroup = value;
			}
		}
		public string TabbedGroupParentID {
			get {
				return tGroupID;
			}
			set {
				tGroupID = value;
			}
		}
		public Rectangle ClientBounds {
			get {
				Rectangle result = ((XAFLayoutItemInfo)Items[0]).Bounds;
				foreach(XAFLayoutItemInfo tempItem in Items)
					result = Rectangle.Union(tempItem.Bounds, result);
				return result;
			}
		}
		public LayoutType LayoutType {
			get { return layoutType; }
			set { layoutType = value; }
		}
		public bool IsGroupBoundsVisible {
			get { return groupBoundsVisible; }
			set { groupBoundsVisible = value; }
		}
		public ArrayList Items {
			get { return items; }
		}
		public void Add(XAFLayoutItemInfo info) {
			if(info == null) return;
			Items.Add(info);
			info.Parent = this;
		}
		public XAFLayoutItemInfo GetItem(int index) {
			return items[index] as XAFLayoutItemInfo;
		}
		internal List<XAFLayoutGroupInfo> GetFlatGroup() {
			List<XAFLayoutGroupInfo> returnList = new List<XAFLayoutGroupInfo>();
			AddGroupToList(ref returnList, this);
			return returnList;
		}
		private void AddGroupToList(ref List<XAFLayoutGroupInfo> returnList, XAFLayoutGroupInfo xAFLayoutGroupInfo) {
			foreach(XAFLayoutItemInfo item in xAFLayoutGroupInfo.Items) {
				if(item is XAFLayoutGroupInfo) AddGroupToList(ref returnList, (XAFLayoutGroupInfo)item);
			}
			returnList.Add(xAFLayoutGroupInfo);
		}
		internal bool ContainsCrossItems(XtraDashboardLayout.Crosshair cross) {
			bool firstItem = false;
			foreach(XAFLayoutItemInfo item in Items) {
				if(firstItem) {
					if(cross.Contains(item.Item)) return true;
				} else
					if(cross.Contains(item.Item)) firstItem = true;
			}
			return false;
		}
	}
	public class XAFTabbedGroupInfo : XAFLayoutItemInfo {
		ArrayList items;
		int activeTabIndex = 0;
		public XAFTabbedGroupInfo() {
			items = new ArrayList();
		}
		public ArrayList Items {
			get { return items; }
		}
		public int SelectedTabPageIndex {
			get { return activeTabIndex; }
			set { activeTabIndex = value; }
		}
		public XAFLayoutGroupInfo GetItem(int index) {
			return items[index] as XAFLayoutGroupInfo;
		}
		public void Add(XAFLayoutGroupInfo item) {
			items.Add(item);
			item.TabbedGroupParent = this;
		}
	}
	public interface ILayoutAdapter {
		LayoutGroupHandlerWithTabHelper CreateRootHandler(LayoutGroup group);
		XAFLayoutItemInfo GetXAFLayoutInfo();
		void SetXAFLayoutInfo(XAFLayoutItemInfo info);
	}
	public class ConstraintedLayoutGroupHandlerWithTabHelper : LayoutGroupHandlerWithTabHelper {
		public ConstraintedLayoutGroupHandlerWithTabHelper(LayoutGroup group)
			: base(group) {
		}
		public override bool CheckCustomizationConstraints(LayoutItemDragController controller) {
			if(controller == null) return false;
			if(controller.DragItem == null || controller.Item == null) return false;
			if(controller.Item.Parent == null) return false;
			if(controller.Item.Parent.DefaultLayoutType == controller.LayoutType) return true;
			return false;
		}
	}
	public enum SetInfoCoreStages { setParent, setSize, unGroup};
	public class OrderComparer : IComparer {
		LayoutType layoutTypeCore;
		public OrderComparer(LayoutType layoutType) {
			layoutTypeCore = layoutType;
		}
		public int Compare(object object1, object object2) {
			XAFLayoutItemInfo item2 = object2 as XAFLayoutItemInfo;
			XAFLayoutItemInfo item1 = object1 as XAFLayoutItemInfo;
			if(item1 != null && item2 != null) {
				LayoutRectangle rect2;
				LayoutRectangle rect1 = new LayoutRectangle(item1.Bounds, layoutTypeCore);
				rect2 = new LayoutRectangle(item2.Bounds, layoutTypeCore);
				if(rect1.X < rect2.X) return -1;
				if(rect1.X > rect2.X) return 1;
				if(rect1.X == rect2.X) return 1;
			}
			return -1;
		}
	}
	public class TreeStyleLayoutAdapter : ILayoutAdapter {
		ILayoutControl layoutControlCore;
		public TreeStyleLayoutAdapter(ILayoutControl layoutControl) {
			layoutControlCore = layoutControl;
		}
		public LayoutGroupHandlerWithTabHelper CreateRootHandler(LayoutGroup group) {
			return new ConstraintedLayoutGroupHandlerWithTabHelper(group);
		}
		void UpdateGroupBounds(XAFLayoutGroupInfo group) {
			if(group.Item != null) { group.Bounds = group.Item.Bounds; return; }
			if(group.Items.Count == 0) return;
			Rectangle rect = group.GetItem(0).Bounds;
			foreach(XAFLayoutItemInfo item in new ArrayList(group.Items)) {
				rect = Rectangle.Union(rect, item.Bounds);
			}
			group.Bounds = rect;
		}
		void UpdateRelativeBoundsInGroup(XAFLayoutGroupInfo group) {
			foreach(XAFLayoutItemInfo item in new ArrayList(group.Items)) {
				item.RelativeSize = 100.0 * new LayoutSize(item.Bounds.Size, group.LayoutType).Width / new LayoutSize(group.ClientBounds.Size, group.LayoutType).Width;
			}
		}
		XAFLayoutItemInfo CreateGroup(XAFLayoutItemInfo i1, XAFLayoutItemInfo i2, LayoutType lt) {
			XAFLayoutGroupInfo resultGroup = new XAFLayoutGroupInfo() { IsGroupBoundsVisible = false, Item = null, LayoutType = lt, ID = CreateGroupID() };
			resultGroup.Add(i1);
			resultGroup.Add(i2);
			UpdateGroupBounds(resultGroup);
			UpdateRelativeBoundsInGroup(resultGroup);
			return resultGroup;
		}
		protected virtual string CreateGroupID(){
			return string.Concat("Auto", Guid.NewGuid().ToString());
		}
		protected void GetResizeTabsList(BaseLayoutItem item, ArrayList list) {
			ResizeGroup grg = item as ResizeGroup;
			FakeGroup fg = item as FakeGroup;
			LayoutControlGroup lcg = item as LayoutControlGroup;
			if(fg != null) {
				list.Add(fg);
				return;
			}
			if(lcg != null) {
				list.Add(lcg);
				return;
			}
			if(grg != null) {
				GetResizeTabsList(grg.Item1, list);
				GetResizeTabsList(grg.Item2, list);
				return;
			}
		}
		protected XAFLayoutItemInfo GetXAFTabbedGroupInfo(TabbedGroupResizeGroup tgRg, LayoutType layoutType) {
			XAFTabbedGroupInfo tgInfo = new XAFTabbedGroupInfo();
			tgInfo.Bounds = tgRg.Group.Bounds;
			tgInfo.ID = tgRg.Group.Name;
			tgInfo.Item = tgRg.Group;
			TabbedGroup tg = tgRg.Group as TabbedGroup;
			ArrayList list = new ArrayList();
			GetResizeTabsList(tgRg.Item, list);
			foreach(BaseLayoutItem tempItem in list) {
				LayoutControlGroup tempGroup = tempItem as LayoutControlGroup;
				FakeGroup fGroup = tempItem as FakeGroup;
				if(tempGroup != null) {
					XAFLayoutGroupInfo newGroup = new XAFLayoutGroupInfo();
					newGroup.ID = tempGroup.Name;
					newGroup.Item = tempGroup;
					newGroup.TabbedGroupParentID = tempGroup.TabbedGroupParentName;
					tgInfo.Add(newGroup);
					UpdateRelativeBoundsInGroup(newGroup);
				}
				if(fGroup != null) {
					XAFLayoutGroupInfo newGroup = (XAFLayoutGroupInfo)GetXAFLayoutInfoCore(fGroup, LayoutType.Horizontal);
					UpdateRelativeBoundsInGroup(newGroup);
					tgInfo.Add(newGroup);
				}
			}
			tgInfo.SelectedTabPageIndex = tg.SelectedTabPageIndex;
			return tgInfo;
		}
		protected XAFLayoutItemInfo GetXAFGroupInfo(GroupResizeGroup groupResizeGroup, LayoutType layoutType) {
			XAFLayoutItemInfo tempInfo = GetXAFLayoutInfoCore(groupResizeGroup.Item, layoutType);
			XAFLayoutGroupInfo tempGroup = tempInfo as XAFLayoutGroupInfo;
			if(tempGroup != null && (tempGroup.Item == groupResizeGroup.Group | tempGroup.Item == null)) {
				InitProperties(groupResizeGroup, tempGroup);
				tempGroup.Items.Sort(new OrderComparer(tempGroup.LayoutType));
				tempGroup.Bounds = groupResizeGroup.Group.Bounds;
				tempGroup.RelativeSize = GetRelativeSize(tempGroup);
				UpdateRelativeBoundsInGroup(tempGroup);
				return tempGroup;
			}
			else {
				XAFLayoutGroupInfo newGroup = new XAFLayoutGroupInfo();
				InitProperties(groupResizeGroup, newGroup);
				newGroup.LayoutType = LayoutType.Vertical;
				newGroup.Add(tempInfo);
				newGroup.RelativeSize = GetRelativeSize(newGroup);
				UpdateRelativeBoundsInGroup(newGroup);
				return newGroup;
			}
		}
		protected XAFLayoutItemInfo GetXAF_H_or_V_GroupInfo(BaseLayoutItem item) {
			HorizontalResizeGroup horizontalResizeGroup = item as HorizontalResizeGroup;
			VerticalResizeGroup vrg = item as VerticalResizeGroup;
			LayoutType layoutType = vrg != null ? LayoutType.Vertical : LayoutType.Horizontal;
			XAFLayoutItemInfo item1 = GetXAFLayoutInfoCore(vrg != null ? vrg.Item1 : horizontalResizeGroup.Item1, layoutType);
			XAFLayoutItemInfo item2 = GetXAFLayoutInfoCore(vrg != null ? vrg.Item2 : horizontalResizeGroup.Item2, layoutType);
			XAFLayoutGroupInfo group1 = item1 as XAFLayoutGroupInfo;
			XAFLayoutGroupInfo group2 = item2 as XAFLayoutGroupInfo;
			XAFTabbedGroupInfo tgroup1 = item1 as XAFTabbedGroupInfo;
			XAFTabbedGroupInfo tgroup2 = item2 as XAFTabbedGroupInfo;
			XAFLayoutItemInfo resultInfo = null;
			try {
				if(group1 == null && group2 == null) {
					resultInfo = CreateGroup(item1, item2, layoutType);
					return resultInfo;
				}
				if(tgroup2 != null && group1 != null) {
					tgroup1 = tgroup2;
					tgroup2 = null;
					group2 = group1;
					group1 = null;
				}
				if(tgroup1 != null && group2 != null) {
					if(group2.Item == null && layoutType == group2.LayoutType) {
						group2.Add(tgroup1);
					}
					else {
						group2 = (XAFLayoutGroupInfo)CreateGroup(group2, tgroup1, layoutType);
					}
					UpdateGroupBounds(group2);
					UpdateRelativeBoundsInGroup(group2);
					resultInfo = group2;
					return resultInfo;
				}
				if(group1 != null) {
					if(group2 != null) {
						bool f1, f2, f3, f4;
						f1 = layoutType == group1.LayoutType;
						f2 = layoutType == group2.LayoutType;
						f3 = group1.Item == null;
						f4 = group2.Item == null;
						if(f1 && f2 && f3 && f4) {
							foreach(XAFLayoutItemInfo tempItem in group2.Items) {
								group1.Add(tempItem);
							}
						}
						if((f1 && f2 && f3 && !f4) || (f1 && !f2 && f3 )){
							group1.Add(group2);
						}
						if((f1 && f2 && !f3 && f4) ||
						  (!f1 && f2 && !f3 && f4)) {
							group2.Add(group1);
							group1 = group2;
						}
						if((!f1 && !f2 && f3 && f4) ||
						   (!f1 && !f2 && !f3 && !f4) ||
						   (!f1 && !f2 && f3 && !f4) ||
						   (!f1 && !f2 && !f3 && f4) ||
						   (!f1 && f2 && f3 && f4) ||
							(f1 && f2 && !f3 && !f4) ||
							(f1 && !f2 && !f3 && f4) ||
							(f1 && !f2 && !f3 && !f4) ||
							(!f1 && f2 && f3 && !f4) ||
							(!f1 && f2 && !f3 && !f4)
							) {
							group1 = (XAFLayoutGroupInfo)CreateGroup(group1, group2, layoutType);
						}
						UpdateGroupBounds(group1);
						UpdateRelativeBoundsInGroup(group1);
						resultInfo = group1;
						return resultInfo;
					}
					else {
						if(group1 != null && group1.Item == null && ((vrg != null && group1.LayoutType == LayoutType.Vertical) || (horizontalResizeGroup != null && group1.LayoutType == LayoutType.Horizontal))) {
							group1.Add(item2);
							UpdateGroupBounds(group1);
							UpdateRelativeBoundsInGroup(group1);
							resultInfo = group1;
							return resultInfo;
						}
						else {
							resultInfo = CreateGroup(item1, item2, layoutType);
							return resultInfo;
						}
					}
				}
				if(group2 != null && group2.Item == null && ((vrg != null && group2.LayoutType == LayoutType.Vertical) || (horizontalResizeGroup != null && group2.LayoutType == LayoutType.Horizontal))) {
					group2.Add(item1);
					UpdateGroupBounds(group2);
					UpdateRelativeBoundsInGroup(group2);
					resultInfo = group2;
					return resultInfo;
				}
				else {
					resultInfo = CreateGroup(item1, item2, layoutType);
					return resultInfo;
				}
			}
			finally {
				if(resultInfo == null) {
					throw new LayoutControlInternalException("result info == null");
				}
				else {
				}
			}
		}
		protected XAFLayoutItemInfo GetXAFLayoutInfoCore(BaseLayoutItem item, LayoutType layoutType) {
			GroupResizeGroup groupResizeGroup = item as GroupResizeGroup;
			HorizontalResizeGroup horizontalResizeGroup = item as HorizontalResizeGroup;
			VerticalResizeGroup vrg = item as VerticalResizeGroup;
			LayoutControlItem lItem = item as LayoutControlItem;
			LayoutControlGroup lGroup = item as LayoutControlGroup;
			TabbedGroupResizeGroup tgRg = item as TabbedGroupResizeGroup;
			if(lItem != null || lGroup != null) {
				XAFLayoutItemInfo tempXAFInfo = lGroup != null ? new XAFLayoutGroupInfo() : new XAFLayoutItemInfo();
				tempXAFInfo.Item = item;
				tempXAFInfo.Bounds = item.Bounds;
				tempXAFInfo.ID = item.Name;
				return tempXAFInfo;
			}
			if(tgRg != null) {
				return GetXAFTabbedGroupInfo(tgRg, layoutType);
			}
			if(groupResizeGroup != null) {
				return GetXAFGroupInfo(groupResizeGroup, layoutType);
			}
			if(vrg != null || horizontalResizeGroup != null) {
				return GetXAF_H_or_V_GroupInfo(item);
			}
			return null;
		}
		private double GetRelativeSize(XAFLayoutItemInfo item) {
			if(item.Item.Parent == null) return 100;
			LayoutGroup group = item.Item as LayoutGroup;
			if(group != null && group.ParentTabbedGroup != null) {
				return 100;
			}
			return 0;
		}
		private void InitProperties(GroupResizeGroup groupResizeGroup, XAFLayoutGroupInfo group) {
			group.IsGroupBoundsVisible = ((LayoutGroup)groupResizeGroup.Group).GroupBordersVisible;
			group.Item = (LayoutGroup)groupResizeGroup.Group;
			group.ID = groupResizeGroup.Group.Name;
			group.Bounds = groupResizeGroup.Group.Bounds;
		}
		protected XAFLayoutItemInfo SortXAFLayoutInfo(XAFLayoutItemInfo info) {
			XAFLayoutGroupInfo group = info as XAFLayoutGroupInfo;
			if(group != null) {
				group.Items.Sort(new XAFInfoComparer(group.LayoutType));
				foreach(XAFLayoutItemInfo tempInfo in new ArrayList(group.Items))
					SortXAFLayoutInfo(tempInfo);
			}
			XAFTabbedGroupInfo tGroup = info as XAFTabbedGroupInfo;
			if(tGroup != null){
				foreach(XAFLayoutItemInfo tempInfo in new ArrayList(tGroup.Items)) {
					SortXAFLayoutInfo(tempInfo);
				}
			}
			return info;
		}
		public XAFLayoutItemInfo GetXAFLayoutInfo() {
			((IXtraSerializable)layoutControlCore).OnStartSerializing();
			XAFLayoutItemInfo unsortedInfo = GetXAFLayoutInfoCore(layoutControlCore.RootGroup.Resizer.resultH, LayoutType.Horizontal);
			((IXtraSerializable)layoutControlCore).OnEndSerializing();
			return SortXAFLayoutInfo(unsortedInfo);
		}
		public XAFLayoutItemInfo TransformLayoutToXAFStyle() {
			layoutControlCore.BeginUpdate();
			XAFLayoutItemInfo result = GetXAFLayoutInfo();
			SetXAFLayoutInfo(result);
			layoutControlCore.EndUpdate();
			return result;
		}
		protected void SetXAFLayoutInfoCore(XAFLayoutItemInfo info, SetInfoCoreStages stage) {
			XAFLayoutGroupInfo group = info as XAFLayoutGroupInfo;
			XAFTabbedGroupInfo tgroup = info as XAFTabbedGroupInfo;
			if(tgroup != null) { ProcessTabbedGroup(tgroup, stage); return; }
			if(group != null) { ProcessGroup(group, stage); return; }
			ProcessItem(info, stage);
		}
		private void ProcessItem(XAFLayoutItemInfo info, SetInfoCoreStages stage) {
			if(stage == SetInfoCoreStages.setParent) {
				BaseLayoutItem bItem = null;
				LayoutControlGroup tempGroup =null;
				bool fContain = layoutControlCore.ItemsAndNames.ContainsKey(info.ID);
				if (fContain) bItem= layoutControlCore.ItemsAndNames[info.ID];
				bool fContainParent = layoutControlCore.ItemsAndNames.ContainsKey(info.ParentID);
				if(fContainParent) tempGroup = ((LayoutControlGroup)layoutControlCore.ItemsAndNames[info.ParentID]);
				if(tempGroup == null) tempGroup = (LayoutControlGroup)info.Parent.Item;
				tempGroup.Add(bItem);
				bItem.Owner = layoutControlCore;
				layoutControlCore.Items.Add(bItem);
			}
		}
		private void ProcessTabbedGroup(XAFTabbedGroupInfo tgroup, SetInfoCoreStages stage) {
			ProcessItem(tgroup, stage);
			TabbedControlGroup tGroup = null;
			bool fContainTab = layoutControlCore.ItemsAndNames.ContainsKey(tgroup.ID);
			if(fContainTab) tGroup = (TabbedControlGroup)layoutControlCore.ItemsAndNames[tgroup.ID];
			if(tGroup == null) throw new LayoutControlInternalException("Can not find TabbedGroup with name:" + tgroup.ID);
			foreach(XAFLayoutGroupInfo ginfo in tgroup.Items) {
				LayoutControlGroup lGroup = null;
				bool fContain = layoutControlCore.ItemsAndNames.ContainsKey(ginfo.ID);
				if(fContain) lGroup = (LayoutControlGroup)layoutControlCore.ItemsAndNames[ginfo.ID];
				if(lGroup == null) throw new LayoutControlInternalException("Can not find LayoutGroup with name:" + ginfo.ID);
				if(stage == SetInfoCoreStages.setParent) {
					lGroup.SetTabbedGroupParent(tGroup);
					lGroup.Parent = tGroup.Parent;
					tGroup.AddTabPage(lGroup);
					layoutControlCore.Items.Add(lGroup);
				}
				if(stage == SetInfoCoreStages.setSize) {
					SetGroupRelativeSize(ginfo, 0);
				}
				SetGroupItemsParent(ginfo, stage);
			}
			tGroup.SelectedTabPageIndex = tgroup.SelectedTabPageIndex;
		}
		private void ProcessGroup(XAFLayoutGroupInfo group, SetInfoCoreStages stage) {
			LayoutControlGroup lGroup = null;
			bool fContain = layoutControlCore.ItemsAndNames.ContainsKey(group.ID);
			if(fContain) lGroup = (LayoutControlGroup)layoutControlCore.ItemsAndNames[group.ID];
			LayoutControlGroup parent = group.Parent != null ? (LayoutControlGroup)group.Parent.Item : null;
			if(stage == SetInfoCoreStages.setParent) {
				bool added = false;
				if(lGroup == null) {
					lGroup = parent.AddGroup();
					lGroup.Name = group.ID;
					group.Item = lGroup;
					layoutControlCore.ItemsAndNames.Add(group.ID, lGroup);
					added = true;
				}
				if(!added) {
					if(parent != null)
						parent.Add(lGroup);
				}
				lGroup.DefaultLayoutType = group.LayoutType;
				lGroup.GroupBordersVisible = group.IsGroupBoundsVisible;
				lGroup.Owner = layoutControlCore;
				layoutControlCore.Items.Add(lGroup);
			}
			if(stage == SetInfoCoreStages.setSize) {
				if(lGroup.Parent == null) {
					lGroup.Size = group.Bounds.Size;
					layoutControlCore.RootGroup = lGroup;
				}
				SetGroupRelativeSize(group, 0);
			}
			SetGroupItemsParent(group, stage);
			if(stage == SetInfoCoreStages.unGroup) {
				if(lGroup.Parent != null) {
					lGroup.ViewInfo.CalculateViewInfo();
					lGroup.UngroupItems();
				}
			}
		}
		protected double CalcRelativeSize100(XAFLayoutGroupInfo group) {
			double sum = 0;
			foreach(XAFLayoutItemInfo xli in group.Items) sum += xli.RelativeSize > 0 ? xli.RelativeSize : (100.0 / group.Items.Count);
			return sum;
		}
		protected int CalcItemRelativeSizeCore(XAFLayoutItemInfo xitem, XAFLayoutGroupInfo parent, bool isLastItemInGroup, int right, bool isReparing) {
			double relativeSize = xitem.RelativeSize;
			parent.Item.ViewInfo.CalculateViewInfo();
			LayoutSize size = new LayoutSize(new Size(parent.Item.Size.Width - parent.Item.ViewInfo.Padding.Width, parent.Item.Size.Height - parent.Item.ViewInfo.Padding.Height), parent.LayoutType);
			if(size.Width <= 0) size.Width = 1;
			if(isReparing) xitem.RelativeSize = 0;
			if(xitem.RelativeSize == 0) relativeSize = 100.0 / parent.Items.Count;
			double tresult = relativeSize * size.Width / CalcRelativeSize100(parent);
			int result = (int)Math.Truncate(tresult);
			if(tresult > 0 && result == 0) result = 1;
			if(isLastItemInGroup) result = Math.Max(1, size.Width - right);
			return result;
		}
		private int SetItemRelativeSize(XAFLayoutItemInfo xitem, int right, bool isLastItemInGroup, bool isReparing) {
			BaseLayoutItem bItem = xitem.Item;
			XAFLayoutGroupInfo parent = xitem.Parent as XAFLayoutGroupInfo;
			LayoutSize parentLayoutSize = new LayoutSize(parent.Bounds.Size, parent.LayoutType);
			int relativeSize = CalcItemRelativeSizeCore(xitem, parent, isLastItemInGroup, right, isReparing);
			if(relativeSize <= 0) return -1;
			Rectangle rect = new Rectangle(
					right,
					new LayoutPoint(bItem.Location, parent.LayoutType).Y,
					relativeSize,
					parentLayoutSize.Height);
			LayoutRectangle lrect = new LayoutRectangle(new Rectangle(), parent.LayoutType) { X = rect.X, Y = rect.Y, Width = rect.Width, Height = rect.Height };
			xitem.Bounds = lrect.Rectangle;
			xitem.Item.SetBounds(xitem.Bounds);
			return right + lrect.Width;
		}
		private void SetGroupRelativeSize(XAFLayoutGroupInfo group, int right) {
			int result = SetGroupRelativeSizeCore(group, right, false);
			if(result < 0) {
				result = SetGroupRelativeSizeCore(group, right, true);
				if(result < 0) throw new LayoutControlInternalException("Could not restore layout. Internal problem");
			}
		}
		private int SetGroupRelativeSizeCore(XAFLayoutGroupInfo group, int right, bool isReparing) {
			for(int i = 0; i < group.Items.Count - 1; i++) {
				XAFLayoutItemInfo tInfo = group.GetItem(i);
				right = SetItemRelativeSize(tInfo, right, false, isReparing);
				if(right <= 0) return -1;
			}
			if(group.Items.Count > 0) {
				if(group.Bounds.Size == Size.Empty) throw new LayoutControlInternalException("group bounds is empty!");
				XAFLayoutItemInfo tInfo1 = group.GetItem(group.Items.Count - 1);
				return SetItemRelativeSize(tInfo1, right, true, isReparing);
			}
			return 1;
		}
		private void SetGroupItemsParent(XAFLayoutGroupInfo group, SetInfoCoreStages stage) {
			foreach(XAFLayoutItemInfo tInfo in new ArrayList(group.Items))
				SetXAFLayoutInfoCore(tInfo, stage);
		}
		public void SetXAFLayoutInfo(XAFLayoutItemInfo info) {
			if(info == null) return;
			layoutControlCore.BeginUpdate();
			StartDeserializing();
			SetXAFLayoutInfoCore(info, SetInfoCoreStages.setParent);
			SetXAFLayoutInfoCore(info, SetInfoCoreStages.setSize);
			if(!info.AllowGroups) SetXAFLayoutInfoCore(info, SetInfoCoreStages.unGroup);
			EndDeserializing();
			layoutControlCore.EndUpdate();
		}
		private void StartDeserializing() {
			layoutControlCore.BeginInit();
			layoutControlCore.IsDeserializing = true;
			layoutControlCore.ItemsAndNames = new Dictionary<string,BaseLayoutItem>();
			layoutControlCore.Items = null;
			int count = layoutControlCore.Items.Count;
			for(int i = 0; i < count; i++) {
				BaseLayoutItem bitem = layoutControlCore.Items[0] as BaseLayoutItem;
				LayoutGroup group = bitem as LayoutControlGroup;
				TabbedControlGroup tgroup = bitem as TabbedControlGroup;
				bitem.Parent = null;
				bitem.Owner = null;
				layoutControlCore.ItemsAndNames.Add(bitem.Name, bitem);
				if(group != null) {
					group.Items.Clear();
				}
				if(tgroup != null) {
					tgroup.TabPages.Clear();
				}
				layoutControlCore.Items.Remove(bitem);
			}
		}
		private void EndDeserializing() {
			layoutControlCore.HiddenItems.Clear();
			foreach(BaseLayoutItem item in layoutControlCore.Items) {
				if(item.Name == "") throw new LayoutControlInternalException("Wrong name");
				LayoutItem citem = item as LayoutItem;
				if(citem != null && citem.ParentName == "" && item.Parent != null) throw new LayoutControlInternalException("Wrong parentName name");
				LayoutGroup group = item as LayoutGroup;
				if(group != null) {
					if(group.ParentTabbedGroup != null && group.TabbedGroupParentName == "")
						throw new LayoutControlInternalException("Wrong tabbedGroupParent name");
					if((group.Parent != null && group.ParentName == "") && group.ParentTabbedGroup == null)
						throw new LayoutControlInternalException("Wrong parent name");
				}
				if(item.HasCustomizationParentName)
					layoutControlCore.HiddenItems.Add(item);
				item.UpdateChildren(false);
			}
			layoutControlCore.RootGroup.Size = layoutControlCore.RootGroup.PreferredSize;
			layoutControlCore.RootGroup.ResetResizer();
			layoutControlCore.RootGroup.Resizer.UpdateConstraints();
			layoutControlCore.EndInit();
			layoutControlCore.IsDeserializing = false;
		}
		public class XAFInfoComparer : IComparer {
			LayoutType layoutType;
			public XAFInfoComparer(LayoutType layoutType) {
				this.layoutType = layoutType;
			}
			int IComparer.Compare(object o1, object o2) {
				XAFLayoutItemInfo i1 = o1 as XAFLayoutItemInfo;
				XAFLayoutItemInfo i2 = o2 as XAFLayoutItemInfo;
				LayoutRectangle bounds_i1 = new LayoutRectangle(i1.Bounds, layoutType);
				LayoutRectangle bounds_i2 = new LayoutRectangle(i2.Bounds, layoutType);
				if(bounds_i1.X == bounds_i2.X) return 0;
				if(bounds_i1.X > bounds_i2.X) return 1;
				if(bounds_i1.X < bounds_i2.X) return -1;
				return 0;
			}
		}
	}
}
