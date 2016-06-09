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
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Skins;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Ribbon.Internal;
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class RibbonPageContentEnumerator : RibbonItemViewInfoEnumerator {
		RibbonPanelViewInfo panelInfo;
		int enumRow;
		RibbonPageGroupViewInfo currentGroup;
		public RibbonPageContentEnumerator(RibbonPanelViewInfo panelInfo, int enumRow)
			: base() {
			this.panelInfo = panelInfo;
			this.enumRow = enumRow;
			this.currentGroup = BeginGroup;
			if(BeginGroup == null)
				Items = new RibbonItemViewInfoCollection(this);
			else
				Items = BeginGroup.Items;
		}
		protected virtual bool IsIntersectByX(RibbonItemViewInfo item1, RibbonItemViewInfo item2) {
			if(item1.Bounds.Left > item2.Bounds.Right || item1.Bounds.Right < item2.Bounds.Left) return false;
			return true;
		}
		protected virtual bool IsAboveItem(RibbonItemViewInfo current, RibbonItemViewInfo aboveItem) {
			if(aboveItem.Bounds.Bottom < current.Bounds.Top) return true;
			return false;
		}
		protected virtual bool IsBelowItem(RibbonItemViewInfo current, RibbonItemViewInfo belowItem) {
			if(belowItem.Bounds.Top > current.Bounds.Bottom) return true;
			return false;
		}
		public virtual int EnumRow { get { return enumRow; } }
		protected bool InRow(RibbonItemViewInfo itemInfo, int rowIndex) {
			int beginY = PanelInfo.ContentBounds.Y + (PanelInfo.ContentBounds.Height - PanelInfo.ViewInfo.GroupContentHeight) / 2;
			beginY += PanelInfo.ViewInfo.ButtonGroupHeight * rowIndex;
			int endY = beginY + PanelInfo.ViewInfo.ButtonGroupHeight;
			if(itemInfo.Bounds.Contains(new Point(itemInfo.Bounds.X + 1, (beginY + endY) / 2))) return true;
			return false;
		}
		protected int GetButtonGroupItemRowIndex(RibbonItemViewInfo itemInfo) {
			for(int i = 0; i < 3; i++) {
				if(InRow(itemInfo, i)) return i;
			}
			return 0;
		}
		protected bool IsInButtonGroup(RibbonItemViewInfo itemInfo) { return (itemInfo.Item as BarItemLink).LinkedObject is BarButtonGroup; }
		protected int GetRowIndexInButtonGroup(RibbonItemViewInfo itemInfo) {
			if(itemInfo.OwnerButtonGroup == null) return 0;
			if(itemInfo.OwnerButtonGroup.RowCount == 3) return GetButtonGroupItemRowIndex(itemInfo);
			else if(itemInfo.OwnerButtonGroup.RowCount == 1) return 1;
			if(itemInfo.Bounds.Bottom < PanelInfo.ContentBounds.Top + PanelInfo.ContentBounds.Height / 2) return 0;
			return 2;
		}
		protected virtual int GetRowIndex(RibbonPageGroupViewInfo groupInfo, RibbonItemViewInfo itemInfo) {
			if(IsInButtonGroup(itemInfo)) { 
				int i = GetRowIndexInButtonGroup(itemInfo);
				return i;
			}
			bool hasAboveItem = false, hasBelowItem = false;
			RibbonItemViewInfoEnumerator itemEnum = new RibbonItemViewInfoEnumerator(groupInfo.Items);
			for(; !itemEnum.End; itemEnum.Next()) {
				RibbonItemViewInfo item = itemEnum.CurrentItem;
				if(item != itemInfo && IsIntersectByX(itemInfo, item)) {
					if(hasAboveItem == false) hasAboveItem = IsAboveItem(itemInfo, item);
					if(hasBelowItem == false) hasBelowItem = IsBelowItem(itemInfo, item);
				}
			}
			if(!hasAboveItem && !hasBelowItem) return 1;
			else if(hasAboveItem) {
				if(hasBelowItem) return 1;
				return 2;
			}
			return 0;
		}
		public virtual RibbonPanelViewInfo PanelInfo { get { return panelInfo; } }
		public virtual RibbonPageGroupViewInfo CurrentGroup { get { return currentGroup; } }
		protected virtual void GetBeginPos(ref RibbonPageGroupViewInfo beginGroup, ref RibbonItemViewInfo beginItem) {
			for(int j = 0; j < PanelInfo.Groups.Count; j++) {
				for(int i = 0; i < PanelInfo.Groups[j].Items.Count; i++) {
					if(PanelInfo.Groups[j].Items[i] is RibbonSeparatorItemViewInfo) continue;
					if(GetRowIndex(PanelInfo.Groups[j], PanelInfo.Groups[j].Items[i]) != EnumRow) continue;
					beginGroup = PanelInfo.Groups[j];
					beginItem = PanelInfo.Groups[j].Items[i];
					return;
				}
			}
			beginGroup = null;
			beginItem = null;
		}
		protected virtual RibbonPageGroupViewInfo BeginGroup {
			get {
				RibbonPageGroupViewInfo groupInfo = null;
				RibbonItemViewInfo itemInfo = null;
				GetBeginPos(ref groupInfo, ref itemInfo);
				return groupInfo;
			}
		}
		protected override RibbonItemViewInfo BeginItem {
			get {
				RibbonPageGroupViewInfo groupInfo = null;
				RibbonItemViewInfo itemInfo = null;
				GetBeginPos(ref groupInfo, ref itemInfo);
				return itemInfo;
			}
		}
		protected virtual RibbonItemViewInfo NextCore() {
			base.Next();
			if(!base.End) return CurrentItem;
			int groupIndex = PanelInfo.Groups.IndexOf(CurrentGroup);
			for(groupIndex = groupIndex + 1; groupIndex < PanelInfo.Groups.Count; groupIndex++) {
				Items = PanelInfo.Groups[groupIndex].Items;
				if(!base.End) break;
			}
			if(groupIndex == PanelInfo.Groups.Count) {
				this.currItem = null;
				return null;
			}
			this.currentGroup = PanelInfo.Groups[groupIndex];
			this.currItem = base.BeginItem;
			return CurrentItem;
		}
		public override RibbonItemViewInfo Next() {
			for(NextCore(); !End; NextCore()) {
				if(GetRowIndex(CurrentGroup, CurrentItem) == EnumRow) return CurrentItem;
			}
			return null;
		}
	}
	public class RibbonItemViewInfoEnumerator {
		RibbonItemViewInfoCollection items;
		protected RibbonItemViewInfo currItem;
		int buttonGroupItemIndex;
		public RibbonItemViewInfoEnumerator() {
			this.items = null;
			this.currItem = null;
			this.buttonGroupItemIndex = 0;
		}
		public RibbonItemViewInfoEnumerator(RibbonItemViewInfoCollection items) {
			this.items = items;
			this.currItem = BeginItem;
			this.buttonGroupItemIndex = 0;
		}
		public RibbonItemViewInfo CurrentItem {
			get {
				RibbonButtonGroupItemViewInfo buttonGroup = currItem as RibbonButtonGroupItemViewInfo;
				if(buttonGroup != null) {
					buttonGroup.CheckViewInfo(null);
					return buttonGroup.Items[buttonGroupItemIndex];
				}
				return currItem;
			}
		}
		public virtual bool End { get { return CurrentItem == null; } }
		protected virtual RibbonItemViewInfo BeginItem {
			get {
				for(int i = 0; i < Items.Count; i++) {
					if(Items[i] is RibbonSeparatorItemViewInfo) continue;
					return Items[i];
				}
				return null;
			}
		}
		public int ButtonGroupItemIndex { get { return buttonGroupItemIndex; } }
		public RibbonItemViewInfoCollection Items {
			get { return items; }
			set {
				this.items = value;
				Reset();
			}
		}
		public virtual RibbonItemViewInfo Next() {
			if(this.currItem == null) return null;
			int itemIndex = Items.IndexOf(currItem);
			RibbonButtonGroupItemViewInfo buttonGroup = this.currItem as RibbonButtonGroupItemViewInfo;
			if(buttonGroup != null) {
				buttonGroup.CheckViewInfo(null);
				if(buttonGroup.Items.Count - 1 > buttonGroupItemIndex) {
					buttonGroupItemIndex++;
					return buttonGroup.Items[buttonGroupItemIndex];
				}
			}
			for(itemIndex = itemIndex + 1; itemIndex < Items.Count; itemIndex++) {
				if(!(Items[itemIndex] is RibbonSeparatorItemViewInfo)) break;
			}
			if(itemIndex == Items.Count) {
				this.currItem = null;
				return null;
			}
			currItem = Items[itemIndex];
			buttonGroup = this.currItem as RibbonButtonGroupItemViewInfo;
			if(buttonGroup != null) {
				buttonGroup.CheckViewInfo(null);
				buttonGroupItemIndex = 0;
				return buttonGroup.Items[buttonGroupItemIndex];
			}
			return currItem;
		}
		public virtual void Reset() {
			this.currItem = BeginItem;
			this.buttonGroupItemIndex = 0;
		}
	}
	public class NavigationObjectRow : CollectionBase {
		NavigationObjectRowCollection rowCollection;
		public NavigationObjectRow() {
			rowCollection = null;
		}
		public NavigationObjectRow(NavigationObjectRowCollection coll) : base() { this.rowCollection = coll; }
		public int Add(NavigationObject obj) { return List.Add(obj); }
		public void Insert(int index, NavigationObject obj) { List.Insert(index, obj); }
		public NavigationObjectRowCollection RowCollection { get { return rowCollection; } set { rowCollection = value; } }
		public NavigationObject this[int index] { get { return List[index] as NavigationObject; } set { List[index] = value; } }
		public int FindNavObject(NavigationObject navObject) {
			for(int n = 0; n < Count; n++) {
				NavigationObject obj = this[n];
				if(obj.IsNavObjectEquals(navObject)) return n;
			}
			return -1;
		}
		public int FindNavObjectByLink(BarItemLink link) {
			for(int n = 0; n < Count; n++) {
				NavigationObjectRibbonItem nobj = this[n] as NavigationObjectRibbonItem;
				if(nobj != null && (nobj.ItemLink == link || (link.Bounds != Rectangle.Empty && nobj.ItemLink.Bounds == link.Bounds))) return n;
			}
			return -1;
		}
		public int FindNavObjectByObject(object obj) {
			for(int i = 0; i < Count; i++) {
				if(this[i].Object == obj)
					return i;
			}
			return -1;
		}
		public int IndexOf(NavigationObject obj) { return List.IndexOf(obj); }
		public virtual NavigationObject Move(NavigationObject current, bool forward) {
			if(Count == 0)
				return null;
			int delta = forward ? +1 : -1;
			int index = IndexOf(current) + delta;
			if(index < 0)
				index = Count - 1;
			index = index % Count;
			return this[index];
		}
	}
	public class NavigationObjectRowCollection : CollectionBase {
		public NavigationObjectRowCollection() : base() { }
		public int Add(NavigationObjectRow obj) {
			if(obj == null || obj.Count == 0) return -1;
			return List.Add(obj); 
		}
		public NavigationObjectRow this[int index] { get { return List[index] as NavigationObjectRow; } set { List[index] = value; } }
		protected override void OnInsertComplete(int index, object value) {
			NavigationObjectRow row = value as NavigationObjectRow;
			if(row != null) row.RowCollection = this;
			base.OnInsertComplete(index, value);
		}
		public Point FindNavObjectByObject(object obj) {
			for(int i = 0; i < Count; i++) {
				int pos = this[i].FindNavObjectByObject(obj);
				if(pos >= 0)
					return new Point(pos,i);
			}
			return new Point(-1, -1);
		}
		public Point FindNavObjectByLink(BarItemLink link) {
			for(int n = 0; n < Count; n++) {
				NavigationObjectRow row = this[n];
				int pos = row.FindNavObjectByLink(link);
				if(pos != -1) return new Point(pos, n);
			}
			return new Point(-1, -1);
		}
		public Point FindNavObject(NavigationObject navObject) {
			for(int n = 0; n < Count; n++) {
				NavigationObjectRow row = this[n];
				int pos = row.FindNavObject(navObject);
				if(pos != -1) return new Point(pos, n);
			}
			return new Point(-1, -1);
		}
		protected internal class ObjectInfo {
			public ObjectInfo(NavigationObject navObject) {
				NavObject = navObject;
				Delta = new Point(10000, 10000);
			}
			public Point Delta;
			public NavigationObject NavObject;
		}
		protected internal void CheckMinX(ObjectInfo info, int dx, int dy, NavigationObject current) {
			if(dx < info.Delta.X) {
				info.Delta = new Point(dx, dy);
				info.NavObject = current;
				return;
			}
			if(dx == info.Delta.X && dy < info.Delta.Y) {
				info.Delta = new Point(dx, dy);
				info.NavObject = current;
			}
		}
		protected internal void CheckMinY(ObjectInfo info, int dx, int dy, NavigationObject current) {
			if(dy < info.Delta.Y) {
				info.Delta = new Point(dx, dy);
				info.NavObject = current;
				return;
			}
			if(dy == info.Delta.Y && dx < info.Delta.X) {
				info.Delta = new Point(dx, dy);
				info.NavObject = current;
			}
		}
		internal delegate bool ShouldIgnore(int delta, NavigationObject current, NavigationObject o);
		protected internal bool ShouldIgnoreHorz(int delta, NavigationObject current, NavigationObject o) {
			if(o is NavigationObjectApplicationButton && current is NavigationObjectRibbonToolbarItem) return false;
			if(delta > 0 && o.Bounds.X < current.Bounds.Right) return true;
			if(delta < 0 && o.Bounds.Right > current.Bounds.X) return true;
			if(!o.GetType().Equals(current.GetType())) return true;
			return false;
		}
		protected internal bool ShouldIgnoreVert(int delta, NavigationObject current, NavigationObject o) {
			if(delta > 0 && o.Bounds.Y < current.Bounds.Bottom) return true;
			if(delta < 0 && o.Bounds.Bottom > current.Bounds.Y) return true;
			return false;
		}
		internal NavigationObject Move(NavigationObject current, int delta, ShouldIgnore ignoreMethod, bool isVertNavigation) {
			if(current == null) return null;
			Rectangle bounds = current.Bounds;
			int x = bounds.X + bounds.Width / 2;
			int y = bounds.Y + bounds.Height / 2;
			ObjectInfo minX = new ObjectInfo(null), minY = new ObjectInfo(null);
			for(int row = 0; row < Count; row++) {
				NavigationObjectRow rowObject = this[row];
				for(int col = 0; col < rowObject.Count; col++) {
					NavigationObject o = rowObject[col];
					if(o == current || o.Object == current.Object) continue;
					if(ignoreMethod(delta, current, o)) continue;
					int dy = Math.Abs(y - GetNearY(y, o));
					int dx = Math.Abs(x - GetNearX(x, o));
					CheckMinX(minX, dx, dy, o);
					CheckMinY(minY, dx, dy, o);
				}
			}
			if(minY.NavObject == null) return minX.NavObject == null ? current : minX.NavObject;
			if(minX.NavObject == null) return minY.NavObject == null ? current : minY.NavObject;
			if(minX.NavObject == minY.NavObject) return minX.NavObject;
			if(!minX.NavObject.GetType().Equals(minY.NavObject.GetType())) {
				if(current.GetType().Equals(minX.NavObject.GetType())) return minX.NavObject;
				if(current.GetType().Equals(minY.NavObject.GetType())) return minY.NavObject;
			}
			if(isVertNavigation) {
				if(minY.NavObject is NavigationObjectPage) return minY.NavObject;
				if(minX.NavObject is NavigationObjectPage) return minX.NavObject;
			}
			if((minX.Delta.X * minX.Delta.X + minX.Delta.Y * minX.Delta.Y) < (minY.Delta.X * minY.Delta.X + minY.Delta.Y * minY.Delta.Y))
				return minX.NavObject;
			return minY.NavObject;
		}
		internal NavigationObject MoveIterated(NavigationObject current, int delta, ShouldIgnore shouldIgnoreMethod, bool isVertical) {
			if(current == null) return null;
			NavigationObject obj = current;
			NavigationObject next = current;
			int tryCount = 0;
			while(tryCount < MaxTryCount) {
				next = Move(obj, delta, shouldIgnoreMethod, isVertical);
				if(next == current || next == obj)
					return current;
				if(next.Enabled)
					break;
				obj = next;
				tryCount++;
			}
			return next;
		}
		const int MaxTryCount = 20;
		internal NavigationObject MoveHorz(NavigationObject current, int delta) {
			return MoveIterated(current, delta, new ShouldIgnore(ShouldIgnoreHorz), false);
		}
		internal NavigationObject MoveVert(NavigationObject current, int delta) {
			return MoveIterated(current, delta, new ShouldIgnore(ShouldIgnoreVert), true);
		}
		protected internal int GetNearX(int currentX, NavigationObject check) {
			if(check.Bounds.Right < currentX) return check.Bounds.X;
			if(check.Bounds.X > currentX) return check.Bounds.X;
			return currentX;
		}
		protected internal int GetNearY(int currentY, NavigationObject check) {
			if(check.Bounds.Bottom < currentY) return check.Bounds.Bottom;
			if(check.Bounds.Top > currentY) return check.Bounds.Y;
			return currentY;
		}
	}
	public class NavigationObject {
		Rectangle bounds;
		object _object;
		RibbonControl ribbon;
		public NavigationObject(RibbonControl ribbon, object _object, Rectangle bounds) {
			this._object = _object;
			this.bounds = bounds;
			this.ribbon = ribbon;
		}
		protected RibbonControl Ribbon { get { return ribbon; } }
		protected BarSelectionInfo SelectionInfo { get { return Ribbon.Manager.SelectionInfo; } }
		public object Object { get { return _object; } set { _object = value; } }
		public virtual void Select(RibbonViewInfo viewInfo) {
			viewInfo.KeyboardActiveObject = this;
			SelectionInfo.KeyboardHighlightedLink = null;
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public virtual bool IsEquals(object navObject) {
			return navObject != null && object.Equals(Object, navObject);
		}
		public virtual bool IsNavObjectEquals(NavigationObject info) { return object.ReferenceEquals(this, info); }
		public virtual RibbonHitInfo CreateHitInfo(RibbonViewInfo viewInfo) { return viewInfo.CreateHitInfo(); }
		public virtual void Click() { }
		protected internal virtual bool CanMoveTo(NavigationObject newObject) {
			return true;
		}
		public virtual bool Enabled { get { return true; } }
	}
	public class NavigationObjectApplicationButton : NavigationObject {
		public NavigationObjectApplicationButton(RibbonControl ribbon, RibbonApplicationButtonInfo buttonInfo) : base(ribbon, buttonInfo, buttonInfo.Bounds) { }
		public RibbonApplicationButtonInfo ApplicationButton { get { return Object as RibbonApplicationButtonInfo; } }
		public override RibbonHitInfo CreateHitInfo(RibbonViewInfo viewInfo) {
			RibbonHitInfo res = viewInfo.CreateHitInfo();
			res.SetHitTest(RibbonHitTest.ApplicationButton);
			return res;
		}
		public override void Click() {
			base.Click();
			(Ribbon.Handler as RibbonHandler).OnApplicationButtonClickCore();
		}
	}
	public class NavigationObjectPage : NavigationObject {
		public NavigationObjectPage(RibbonControl ribbon, RibbonPageViewInfo pageInfo) : base(ribbon, pageInfo, pageInfo == null? Rectangle.Empty: pageInfo.Bounds) { }
		public RibbonPage Page { get { return ((RibbonPageViewInfo)Object).Page; } }
		public override bool IsEquals(object navObject) {
			if(base.IsEquals(navObject)) return true;
			if(Page == navObject) return true;
			RibbonPageViewInfo pageInfo = navObject as RibbonPageViewInfo;
			return pageInfo != null && pageInfo.Page == Page;
		}
		public override RibbonHitInfo CreateHitInfo(RibbonViewInfo viewInfo) {
			RibbonHitInfo res = viewInfo.CreateHitInfo();
			res.SetHitTest(RibbonHitTest.PageHeader);
			res.Page = Page;
			return res;
		}
		public override bool IsNavObjectEquals(NavigationObject info) {
			if(object.ReferenceEquals(this, info)) return true;
			NavigationObjectPage infoPage = info as NavigationObjectPage;
			return infoPage != null && infoPage.Page == Page;
		}
		public override void Click() {
			if(Ribbon.Minimized) {
				Ribbon.ShowMinimizedRibbon();
			}
		}
	}
	public class NavigationObjectPageGroupButton : NavigationObject {
		public RibbonPageGroup PageGroup { get { return ((RibbonPageGroupViewInfo)Object).PageGroup; } }
		public NavigationObjectPageGroupButton(RibbonControl ribbon, RibbonPageGroupViewInfo groupInfo) : base(ribbon, groupInfo, groupInfo.CaptionBounds) { }
		public override RibbonHitInfo CreateHitInfo(RibbonViewInfo viewInfo) {
			RibbonHitInfo res = viewInfo.CreateHitInfo();
			res.SetHitTest(RibbonHitTest.PageGroupCaptionButton);
			res.PageGroupInfo = Object as RibbonPageGroupViewInfo;
			return res;
		}
		public override bool IsNavObjectEquals(NavigationObject info) {
			if(object.ReferenceEquals(this, info)) return true;
			NavigationObjectPageGroupButton infoPage = info as NavigationObjectPageGroupButton;
			return infoPage != null && infoPage.PageGroup == PageGroup;
		}
		public override void Click() {
			Ribbon.DeactivateKeyboardNavigation();
			Ribbon.RaisePageGroupCaptionButtonClick(new RibbonPageGroupEventArgs(PageGroup));
		}
	}
	public abstract class NavigationObjectRibbonItem : NavigationObject {
		public NavigationObjectRibbonItem(RibbonControl ribbon, RibbonItemViewInfo itemInfo) : base(ribbon, itemInfo, itemInfo.Bounds) { }
		public RibbonItemViewInfo ItemInfo { get { return ((RibbonItemViewInfo)Object); } }
		public IRibbonItem Item { get { return ItemInfo.Item; } }
		public BarItemLink ItemLink { get { return Item as BarItemLink; } }
		public override bool IsEquals(object navObject) {
			if(base.IsEquals(navObject)) return true;
			if(Item == navObject) return true;
			RibbonItemViewInfo itemInfo = navObject as RibbonItemViewInfo;
			return itemInfo != null && itemInfo.Item == Item;
		}
		public override RibbonHitInfo CreateHitInfo(RibbonViewInfo viewInfo) {
			RibbonHitInfo res = viewInfo.CreateHitInfo();
			RibbonItemViewInfo itemInfo = Object as RibbonItemViewInfo;
			res.SetItem(itemInfo, RibbonHitTest.Item);
			return res;
		}
		public override void Select(RibbonViewInfo viewInfo) {
			base.Select(viewInfo);
			SelectionInfo.KeyboardHighlightedLink = ItemLink;
		}
		public override bool Enabled {
			get { return ItemInfo.Enabled; }
		}
	}
	public class NavigationObjectRibbonToolbarItem : NavigationObjectRibbonItem {
		public NavigationObjectRibbonToolbarItem(RibbonControl ribbon, RibbonItemViewInfo itemInfo) : base(ribbon, itemInfo) { }
		public override RibbonHitInfo CreateHitInfo(RibbonViewInfo viewInfo) {
			RibbonHitInfo res = base.CreateHitInfo(viewInfo);
			res.Toolbar = ToolbarInfo;
			return res;
		}
		public RibbonQuickAccessToolbarViewInfo ToolbarInfo { get { return ItemInfo.GetOwner() as RibbonQuickAccessToolbarViewInfo; } }
		public override bool IsNavObjectEquals(NavigationObject info) {
			if(object.ReferenceEquals(this, info)) return true;
			NavigationObjectRibbonToolbarItem infoItem = info as NavigationObjectRibbonToolbarItem;
			if(infoItem == null || infoItem.Item != Item) return false;
			return infoItem.ToolbarInfo == ToolbarInfo;
		}
	}
	public class NavigationObjectRibbonPageGroupItem : NavigationObjectRibbonItem {
		public NavigationObjectRibbonPageGroupItem(RibbonControl ribbon, RibbonItemViewInfo itemInfo) : base(ribbon, itemInfo) { }
		public override RibbonHitInfo CreateHitInfo(RibbonViewInfo viewInfo) {
			RibbonHitInfo res = base.CreateHitInfo(viewInfo);
			res.PageGroupInfo = PageGroupInfo;
			return res;
		}
		public RibbonPageGroupViewInfo PageGroupInfo { get { return ItemInfo.GetOwner() as RibbonPageGroupViewInfo; } }
		public RibbonPageGroup PageGroup { get { return PageGroupInfo == null ? null : PageGroupInfo.PageGroup; } }
		public override bool IsNavObjectEquals(NavigationObject info) {
			if(object.ReferenceEquals(this, info)) return true;
			NavigationObjectRibbonPageGroupItem infoItem = info as NavigationObjectRibbonPageGroupItem;
			if(infoItem == null || infoItem.Item != Item) return false;
			return infoItem.PageGroup == PageGroup;
		}
		protected internal override bool CanMoveTo(NavigationObject newObject) {
			NavigationObjectPageGroupButton pb = newObject as NavigationObjectPageGroupButton;
			if(pb != null) return pb.PageGroup == PageGroup;
			return true;
		}
	}
}
