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
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using DevExpress.Utils.Serializing;
using Accessibility;
using DevExpress.Accessibility;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Resizing;
using System.Collections.Generic;
using DevExpress.XtraDashboardLayout;
namespace DevExpress.XtraLayout.Accessibility {
	public class BaseLayoutItemAccessable : BaseAccessible {
		protected BaseLayoutItem item;
		public BaseLayoutItemAccessable(BaseLayoutItem item) {
			this.item = item;
		}
		protected override string GetDefaultAction() {
			return LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.DefaultActionText);
		}
		protected override string GetName() {
			return item.Name;
		}
		protected override string GetKeyboardShortcut() {
			return LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.DefaultEmptyText);
		}
		protected override string GetDescription() {
			return item.Text;
		}
		public override Rectangle ScreenBounds {
			get {
				Rectangle controlRect = item.ViewInfo.BoundsRelativeToControl;
				if(item.Owner != null) {
					controlRect.Location = item.Owner.Control.PointToScreen(controlRect.Location);
				}
				return controlRect;
			}
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.Cell; }
		public override Control GetOwnerControl() {
			if(item != null && item.Owner != null)
				return item.Owner.Control;
			return null;
		}
	}
	public class LayoutControlItemAccessable : BaseLayoutItemAccessable {
		protected LayoutControlItem citem {
			get {
				return item as LayoutControlItem;
			}
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.Cell; }
		public LayoutControlItemAccessable(BaseLayoutItem item) : base(item) { }
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			if(citem.Control != null) AddChild(new StandardAccessible(citem.Control));
		}
		protected override ChildrenInfo GetChildrenInfo() { return new ChildrenInfo(ChildType.Item, citem.Control != null ? 1 : 0); }
	}
	public class TabInternal : DevExpress.Accessibility.Tab.TabControlAccessible {
		public TabInternal(DevExpress.XtraTab.IXtraTab item) : base(item) { }
		DevExpress.XtraLayout.Tab.LayoutTab Tab {
			get { return base.TabControl as DevExpress.XtraLayout.Tab.LayoutTab; }
		}
		public override Rectangle ScreenBounds {
			get {
				Rectangle controlRect = Tab.Owner.ViewInfo.BoundsRelativeToControl;
				if(Tab.Owner != null) {
					controlRect.Location = Tab.Owner.Owner.Control.PointToScreen(controlRect.Location);
				}
				return controlRect;
			}
		}
	}
	public class TabbedGroupAccessable : BaseLayoutItemAccessable {
		protected TabbedControlGroup tabbedGroup {
			get {
				return item as TabbedControlGroup;
			}
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.PageTabList; }
		public TabbedGroupAccessable(BaseLayoutItem item) : base(item) { }
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo info = new ChildrenInfo("TabPages", tabbedGroup.TabPages.Count);
			info["TabControl"] = 1;
			return info;
		}
		protected override void OnChildrenCountChanged() {
			foreach(LayoutGroup page in tabbedGroup.TabPages) {
				AddChild(new LayoutControlGroupAccessable(page));
			}
			AddChild(new TabInternal(tabbedGroup.ViewInfo.BorderInfo.Tab));
		}
	}
	public class LayoutControlGroupAccessable : BaseLayoutItemAccessable {
		protected LayoutControlGroup Group {
			get { return item as LayoutControlGroup; }
		}
		public LayoutControlGroupAccessable(BaseLayoutItem item) : base(item) { }
		protected override AccessibleRole GetRole() { return Group.ParentTabbedGroup == null ? AccessibleRole.Table : AccessibleRole.PageTab; }
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo info = new ChildrenInfo("Items", Group.Items.Count);
			return info;
		}
		protected override void OnChildrenCountChanged() {
			foreach(BaseLayoutItem bitem in new ArrayList(Group.Items)) {
				LayoutControlItem citem = bitem as LayoutControlItem;
				LayoutControlGroup cgroup = bitem as LayoutControlGroup;
				TabbedGroup tabbedGroup = bitem as TabbedControlGroup;
				if(citem != null) AddChild(new LayoutControlItemAccessable(bitem));
				if(tabbedGroup != null) AddChild(new TabbedGroupAccessable(bitem));
				if(cgroup != null) AddChild(new LayoutControlGroupAccessable(bitem));
			}
		}
		protected override string GetDescription() {
			return LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.LayoutGroupDescription);
		}
		protected override string GetName() {
			return Group.Name;
		}
	}
	public class LayoutControlAccessible : BaseControlAccessible {
		public LayoutControlAccessible(ILayoutControl lc)
			: base(lc.Control) {
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.Pane; }
		protected override AccessibleStates GetState() { return AccessibleStates.None; }
		public LayoutControl Layout { get { return Control as LayoutControl; } }
		public override Rectangle ClientBounds { get { return Layout.ClientRectangle; } }
		protected override string GetName() { return "The XtraLayoutControl"; }
		public override string Value { get { return GetName(); } }
		protected override ChildrenInfo GetChildrenInfo() { return new ChildrenInfo(ChildType.Item, 3); }
		public override Control GetOwnerControl() { return Layout; }
		public override AccessibleObject Parent { get { return Layout.Parent != null ? Layout.Parent.AccessibilityObject : null; } }
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			AddChild(new LayoutControlGroupAccessable(Layout.Root));
			AddChild(new ScrollBarAccessible(Layout.Scroller.HScroll));
			AddChild(new ScrollBarAccessible(Layout.Scroller.VScroll));
		}
		public override bool RaiseQueryAccessibilityHelp(QueryAccessibilityHelpEventArgs e) {
			return base.RaiseQueryAccessibilityHelp(e);
		}
	}
	public class LayoutControlAccessibleOld : BaseAccessible {
		LayoutControl control;
		public LayoutControlAccessibleOld(LayoutControl owner) {
			control = owner;
		}
		public override Control GetOwnerControl() { return control; }
		protected override string GetDescription() {
			return LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.LayoutControlDescription);
		}
		protected override string GetName() {
			return control.Name;
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.Pane; }
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo info = new ChildrenInfo("Scroll", 2);
			info["RootGroup"] = 1;
			return info;
		}
		protected override string GetDefaultAction() {
			return LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.DefaultActionText);
		}
		protected override void OnChildrenCountChanged() {
			AddChild(new ScrollBarAccessible(control.Scroller.HScroll));
			AddChild(new ScrollBarAccessible(control.Scroller.VScroll));
			AddChild(new LayoutControlGroupAccessable(control.Root));
		}
	}
}
namespace DevExpress.XtraLayout.Utils {
	public delegate void LayoutGroupEventHandler(object sender, LayoutGroupEventArgs e);
	public delegate void UniqueNameRequestHandler(object sender, UniqueNameRequestArgs e);
	public delegate void LayoutGroupCancelEventHandler(object sender, LayoutGroupCancelEventArgs e);
	public class LayoutGroupCancelEventArgs : LayoutGroupEventArgs {
		bool cancel;
		public LayoutGroupCancelEventArgs(LayoutGroup group, bool cancel)
			: base(group) {
			Cancel = cancel;
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public class UniqueNameRequestArgs : EventArgs {
		BaseLayoutItem targetItem;
		ArrayList existingNames;
		public UniqueNameRequestArgs(BaseLayoutItem item, ArrayList names) {
			this.targetItem = item;
			this.existingNames = names;
		}
		public BaseLayoutItem TargetItem { get { return targetItem; } }
		public ArrayList ExistingNames { get { return existingNames; } }
	}
	public class LayoutGroupEventArgs : EventArgs {
		LayoutGroup group;
		public LayoutGroupEventArgs(LayoutGroup group) {
			this.group = group;
		}
		public LayoutGroup Group { get { return group; } }
	}
	public enum LayoutType { Horizontal, Vertical };
	public enum LayoutMode { Regular, Flow, Table };
	public enum InsertLocation { After, Before };
	public enum InsertType { Left, Right, Top, Bottom };
	public enum MoveType { Inside, Outside };
	public class InsertLocationLayoutTypeToInsertTypeConverter {
		public static InsertType Convert(InsertLocation insertLocation, LayoutType layoutType) {
			if(insertLocation == InsertLocation.After) {
				if(layoutType == LayoutType.Horizontal) return InsertType.Right;
				else return InsertType.Bottom;
			}
			else {
				if(layoutType == LayoutType.Horizontal) return InsertType.Left;
				else return InsertType.Top;
			}
		}
	}
	public class InsertTypeToInsertLocationLayoutTypesConverter {
		public static void Convert(InsertType insertType, out InsertLocation insertLocation, out LayoutType layoutType) {
			insertLocation = InsertLocation.After;
			layoutType = LayoutType.Vertical;
			switch(insertType) {
				case InsertType.Bottom:
					insertLocation = InsertLocation.After;
					layoutType = LayoutType.Vertical;
					break;
				case InsertType.Left:
					insertLocation = InsertLocation.Before;
					layoutType = LayoutType.Horizontal;
					break;
				case InsertType.Right:
					insertLocation = InsertLocation.After;
					layoutType = LayoutType.Horizontal;
					break;
				case InsertType.Top:
					insertLocation = InsertLocation.Before;
					layoutType = LayoutType.Vertical;
					break;
			}
		}
	}
	public enum LayoutVisibility { Always, Never, OnlyInCustomization, OnlyInRuntime };
	public class LayoutVisibilityConvertor {
		public static LayoutVisibility FromBoolean(bool val) {
			if(val) return LayoutVisibility.Always;
			else return LayoutVisibility.Never;
		}
		public static bool ToBoolean(LayoutVisibility val) {
			return (val == LayoutVisibility.Always);
		}
	}
	public class BaseVisitor {
		public virtual void Visit(BaseLayoutItem item) { }
		public virtual bool StartVisit(BaseLayoutItem item) { return true; }
		public virtual void EndVisit(BaseLayoutItem item) { }
		public virtual IList ArrangeElements(BaseLayoutItem item) {
			return null;
		}
	}
	class UpdateControlsHelper : BaseVisitor {
		protected virtual void Update(LayoutControlItem item) {
			item.UpdateControl();
		}
		public override void Visit(BaseLayoutItem item) {
			LayoutControlItem citem = item as LayoutControlItem;
			if(citem != null) {
				Update(citem);
			}
		}
	}
	[TypeConverter(typeof(PaddingConverter))]
	[RefreshProperties(RefreshProperties.All)]
	public struct Padding {
		static Padding invalidP = new Padding(-999);
		int left;
		int right;
		int bottom;
		int top;
		public static Padding Empty {
			get { return new Padding(); }
		}
		public static Padding Invalid {
			get { return invalidP; }
		}
		public static Padding operator +(Padding p1, Padding p2) {
			return new Padding(p1.Left + p2.Left, p1.Right + p2.Right, p1.Top + p2.Top, p1.Bottom + p2.Bottom);
		}
		public static bool operator ==(Padding p1, Padding p2) {
			return
				(p1.Left == p2.Left) && (p1.Right == p2.Right) &&
				(p1.Top == p2.Top) && (p1.Bottom == p2.Bottom);
		}
		public override int GetHashCode() {
			return left + right + top + bottom;
		}
		public static bool operator !=(Padding p1, Padding p2) {
			return !(p1 == p2);
		}
		public override bool Equals(object o) {
			if(!(o is Padding)) return false;
			return this == (Padding)o;
		}
		public Padding(int left, int right, int top, int bottom) {
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}
		public Padding(int all)
			: this(all, all, all, all) {
		}
		[RefreshProperties(RefreshProperties.All)]
		public int All {
			get { return (left == right && left == top && left == bottom) ? left : 0; }
			set { left = right = top = bottom = value; }
		}
		public int Left {
			get { return left; }
			set { left = value; }
		}
		public int Width {
			get { return left + right; }
		}
		public int Height {
			get { return top + bottom; }
		}
		public int Right {
			get { return right; }
			set { right = value; }
		}
		public int Bottom {
			get { return bottom; }
			set { bottom = value; }
		}
		public int Top {
			get { return top; }
			set { top = value; }
		}
	}
	public class ContainsItemVisitor : BaseVisitor {
		bool contains;
		BaseLayoutItem item;
		public ContainsItemVisitor(BaseLayoutItem item) {
			this.item = item;
		}
		public bool Contains {
			get { return contains; }
		}
		public override void Visit(BaseLayoutItem item) {
			if(this.item == item) contains = true;
		}
	}
	public static class LayoutGeometry {
		public static LayoutType InvertLayout(LayoutType lt) {
			if(lt == LayoutType.Vertical) return LayoutType.Horizontal;
			else return LayoutType.Vertical;
		}
	}
	public class LayoutClassifier {
		static LayoutClassifier defaultClassifier = null;
		public static LayoutClassifier Default {
			get {
				if(defaultClassifier == null)
					defaultClassifier = CreateDefaultLayoutClassifier();
				return defaultClassifier;
			}
		}
		protected static LayoutClassifier CreateDefaultLayoutClassifier() {
			return new LayoutClassifier();
		}
		internal LayoutClassificationArgs ClassifyFull(BaseLayoutItem item) {
			return new LayoutClassificationArgs(item, true);
		}
		public LayoutClassificationArgs Classify(BaseLayoutItem item) {
			return new LayoutClassificationArgs(item, false);
		}
	}
	public struct LayoutClassificationArgs {
		LayoutControlItem lci;
		EmptySpaceItem esi;
		LayoutControlGroup lcGroup;
		SplitterItem sItem;
		TabbedControlGroup tabbedGroup;
		GroupResizeGroup grg;
		TableGroupResizeGroup tgrg;
		HorizontalResizeGroup hrg;
		VerticalResizeGroup vrg;
		ResizeGroup resizeGroup;
		BaseLayoutItem bitem;
		LayoutRepositoryItem repitem;
		DashboardLayoutControlItemBase ditem;
		DashboardLayoutControlGroupBase dgroup;
		internal LayoutClassificationArgs(BaseLayoutItem item, bool doFullClassification) {
			bitem = item;
			lci = item as LayoutControlItem;
			esi = item as EmptySpaceItem;
			lcGroup = item as LayoutControlGroup;
			sItem = item as SplitterItem;
			tabbedGroup = item as TabbedControlGroup;
			repitem = item as LayoutRepositoryItem;
			dgroup = item as DashboardLayoutControlGroupBase;
			ditem = item as DashboardLayoutControlItemBase;
			if(doFullClassification) {
				grg = item as GroupResizeGroup;
				tgrg = item as TableGroupResizeGroup;
				hrg = item as HorizontalResizeGroup;
				vrg = item as VerticalResizeGroup;
				resizeGroup = item as ResizeGroup;
			}
			else {
				grg = null;
				tgrg = null;
				hrg = null;
				vrg = null;
				resizeGroup = null;
			}
		}
		public DashboardLayoutControlGroupBase DGroup { get { return dgroup; } }
		public DashboardLayoutControlItemBase DItem { get { return ditem; } }
		public LayoutControlGroup Group { get { return lcGroup; } }
		public LayoutControlItem LayoutControlItem { get { return lci; } }
		public SplitterItem Splitter { get { return sItem; } }
		public TabbedControlGroup TabbedGroup { get { return tabbedGroup; } }
		public EmptySpaceItem EmptySpaceItem { get { return esi; } }
		public BaseLayoutItem BaseLayoutItem { get { return bitem; } }
		public LayoutRepositoryItem RepositoryItem { get { return repitem; } }
		internal GroupResizeGroup GroupResizeGroup { get { return grg; } }
		internal TableGroupResizeGroup TableGroupResizeGroup { get { return tgrg; } }
		internal ResizeGroup ResizeGroup { get { return resizeGroup; } }
		internal HorizontalResizeGroup HResizeGroup { get { return hrg; } }
		internal VerticalResizeGroup VResizeGroup { get { return vrg; } }
		public bool IsDashboardGroup { get { return DGroup != null; } }
		public bool IsDashboardItem { get { return DItem != null; } }
		public bool IsGroup { get { return Group != null; } }
		public bool IsContainer { get { return (Group != null) || (TabbedGroup != null); } }
		public bool IsTabPage { get { return IsGroup && (Group.Parent != null) && (Group.ParentTabbedGroup != null); } }
		[Obsolete("Use IsTabbedGroup instead")]
		public bool IsTab { get { return TabbedGroup != null; } }
		public bool IsTabbedGroup { get { return TabbedGroup != null; } }
		public bool IsLayoutControlItem { get { return LayoutControlItem != null; } }
		public bool IsSplitterItem { get { return Splitter != null; } }
		public bool IsEmptySpaceItem { get { return EmptySpaceItem != null; } }
		public bool IsRepositoryItem { get { return RepositoryItem != null; } }
		internal bool IsGroupResizeGroup { get { return GroupResizeGroup != null; } }
		internal bool IsTableGroupResizeGroup { get { return TableGroupResizeGroup != null; } }
		internal bool IsResizeGroup { get { return ResizeGroup != null; } }
		internal bool IsHResizeGroup { get { return HResizeGroup != null; } }
		internal bool IsVResizeGroup { get { return VResizeGroup != null; } }
	}
	public class PointOperations {
		public static Point Add(Point pt1, Point pt2) {
			return new Point(pt1.X + pt2.X, pt1.Y + pt2.Y);
		}
		public static Point Subtract(Point pt1, Point pt2) {
			return new Point(pt1.X - pt2.X, pt1.Y - pt2.Y);
		}
	}
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay("LayoutSize: {size}, {layoutType}")]
#endif
	public struct LayoutSize {
		Size size;
		LayoutType layoutType;
		public LayoutType LayoutType { get { return layoutType; } }
		public LayoutSize(Size size, LayoutType layoutType) {
			this.size = size;
			this.layoutType = layoutType;
		}
		public Size Size { get { return size; } }
		public int Width {
			get { return LayoutType == LayoutType.Horizontal ? Size.Width : Size.Height; }
			set {
				if(LayoutType == LayoutType.Horizontal)
					size.Width = value;
				else size.Height = value;
			}
		}
		public int Height {
			get { return LayoutType == LayoutType.Horizontal ? Size.Height : Size.Width; }
			set {
				if(LayoutType == LayoutType.Horizontal)
					size.Height = value;
				else size.Width = value;
			}
		}
	}
	public struct LayoutPoint {
		Point point;
		LayoutType layoutType;
		public LayoutType LayoutType { get { return layoutType; } }
		public LayoutPoint(Point point, LayoutType layoutType) {
			this.point = point;
			this.layoutType = layoutType;
		}
		public Point Point { get { return point; } }
		public int X {
			get { return LayoutType == LayoutType.Horizontal ? Point.X : Point.Y; }
			set {
				if(LayoutType == LayoutType.Horizontal)
					point.X = value;
				else point.Y = value;
			}
		}
		public int Y {
			get { return LayoutType == LayoutType.Horizontal ? Point.Y : Point.X; }
			set {
				if(LayoutType == LayoutType.Horizontal)
					point.Y = value;
				else point.X = value;
			}
		}
	}
	public struct LayoutRectangle {
		LayoutPoint point;
		LayoutSize size;
		LayoutType layoutType;
		public LayoutType LayoutType { get { return layoutType; } }
		public LayoutRectangle(Rectangle rectangle, LayoutType layoutType) {
			point = new LayoutPoint(rectangle.Location, layoutType);
			size = new LayoutSize(rectangle.Size, layoutType);
			this.layoutType = layoutType;
		}
		public LayoutPoint LayoutLocation { get { return point; } }
		public LayoutSize LayoutSize { get { return size; } }
		public LayoutRectangle Clone() { return new LayoutRectangle(this.Rectangle, LayoutType); }
		public Point Location { get { return point.Point; } }
		public Rectangle Rectangle { get { return new Rectangle(point.Point, size.Size); } }
		public Size Size { get { return size.Size; } }
		public int X { get { return point.X; } set { point.X = value; } }
		public int Y { get { return point.Y; } set { point.Y = value; } }
		public int Width { get { return size.Width; } set { size.Width = value; } }
		public int Height { get { return size.Height; } set { size.Height = value; } }
		public int Left {
			get { return X; }
			set {
				Width = Width + Left - value;
				X = value;
			}
		}
		public int Top {
			get { return Y; }
			set {
				Height = Height + Top - value;
				Y = value;
			}
		}
		public int Right {
			get { return X + Width; }
			set {
				if(value < X) return;
				Width = value - X;
			}
		}
		public int Bottom {
			get { return Y + Height; }
			set {
				if(value < Y) return;
				Height = value - Y;
			}
		}
		public bool IntersectsWith(LayoutRectangle lRect) {
			return this.Rectangle.IntersectsWith(lRect.Rectangle);
		}
	}
	[ListBindable(BindableSupport.No), Editor("DevExpress.XtraLayout.Design.TabbedGroupCollectionEditor, " + AssemblyInfo.SRAssemblyLayoutControlDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class TabbedGroupsCollection : BaseItemCollection {
		public new TabbedGroup Owner {
			get { return (TabbedGroup)ownerItem; }
		}
		public LayoutGroup AddTabPage() {
			return Owner.AddTabPage();
		}
		public new LayoutGroup this[int index] {
			get { return base[index] as LayoutGroup; }
		}
	}
	[ListBindable(BindableSupport.No), Editor("DevExpress.XtraLayout.Design.GroupCollectionEditor, " + AssemblyInfo.SRAssemblyLayoutControlDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class LayoutGroupItemCollection : BaseItemCollection {
		public new LayoutGroup Owner {
			get { return (LayoutGroup)ownerItem; }
		}
		public LayoutGroup AddGroup() {
			return (LayoutGroup)Owner.AddGroup();
		}
		public LayoutItem AddItem() {
			return (LayoutItem)Owner.AddItem();
		}
		public TabbedGroup AddTabbedGroup() {
			return (TabbedGroup)Owner.AddTabbedGroup();
		}
		public void RaiseOnChanged(CollectionChangeEventArgs e) {
			base.OnChanged(e);
		}
		internal bool Contains(int row, int column) {
			foreach(BaseLayoutItem item in this) {
				if(item.OptionsTableLayoutItem.RowIndex == row && item.OptionsTableLayoutItem.ColumnIndex == column) return true;
			}
			return false;
		}
	}
	public class ReadOnlyItemCollection : BaseItemCollection {
		public ReadOnlyItemCollection() : base() { }
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override void AddRange(BaseLayoutItem[] items) {
			base.AddRange(items);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public new void RemoveAt(int index) { base.RemoveAt(index); }
	}
	[ListBindable(BindableSupport.No)]
	public class BaseItemCollection : CollectionBase, IEnumerable<BaseLayoutItem> {
		public event CollectionChangeEventHandler Changed;
		protected BaseLayoutItem ownerItem;
		public BaseItemCollection(BaseLayoutItem owner) {
			ownerItem = owner;
		}
		public BaseItemCollection() {
		}
		protected internal BaseItemCollection(BaseLayoutItem[] items)
			: this() {
			AddRange(items);
		}
		public BaseItemCollection(BaseItemCollection source, BaseLayoutItem ownerItem)
			: this(ownerItem) {
			if(source != null)
				AddRange(System.Linq.Enumerable.ToArray(source));
		}
		public BaseLayoutItem Owner {
			get { return ownerItem; }
		}
		public virtual BaseLayoutItem this[int index] { 
			get { return InnerList[index] as BaseLayoutItem; } 
		}
		public int IndexOf(BaseLayoutItem item) { 
			return InnerList.IndexOf(item); 
		}
		public virtual void AddRange(BaseLayoutItem[] items) {
			if(Owner != null) Owner.BeginInit();
			for(int i = 0; i < items.Length; i++) {
				Add(items[i]);
			}
			if(Owner != null) Owner.EndInit();
		}
		public static Rectangle CalcItemsBounds(List<BaseLayoutItem> targetItems) {	  
			if(targetItems.Count == 0) return Rectangle.Empty;
			Rectangle r = targetItems[0].Bounds;
			for(int i = 1; i < targetItems.Count; i++) {
				Rectangle ir = targetItems[i].Bounds;
				if(!targetItems[i].ActualItemVisibility && targetItems[i].Parent != null && targetItems[i].Parent.ResizeManager != null && targetItems[i].Parent.ResizeManager.resizer != null && !targetItems[i].DisposingFlag)
					ir = targetItems[i].Parent.Resizer.GetHiddenItemRealBounds(targetItems[i]);
				if(r.X > ir.X) {
					r.Width += r.X - ir.X;
					r.X = ir.X;
				}
				if(r.Y > ir.Y) {
					r.Height += r.Y - ir.Y;
					r.Y = ir.Y;
				}
				if(r.Right < ir.Right) r.Width = ir.Right - r.X;
				if(r.Bottom < ir.Bottom) r.Height = ir.Bottom - r.Y;
			}
			return r;
		}
		public List<BaseLayoutItem> ConvertToTypedList() {
			return new List<BaseLayoutItem>(this);
		}
		public virtual Rectangle ItemsBounds {
			get { return CalcItemsBounds(ConvertToTypedList()); }
		}
		public bool Contains(BaseLayoutItem item) {
			return InnerList.Contains(item);
		}
		public int ItemCount {
			get { return Count - GroupCount; }
		}
		public int GroupCount {
			get {
				int count = 0;
				for(int i = 0; i < Count; i++) {
					if(this[i].IsGroup) count++;
				}
				return count;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public new void Clear() { base.Clear(); }
		protected internal virtual void Add(BaseLayoutItem item) {
			List.Add(item);
		}
		protected internal void Insert(int index, BaseLayoutItem item) { List.Insert(index, item); }
		protected virtual internal void Remove(BaseLayoutItem item) {
			List.Remove(item);
		}
		protected internal LayoutGroup GetGroup(int index) {
			return GetItem(index, true) as LayoutGroup;
		}
		protected internal LayoutItem GetItem(int index) {
			return GetItem(index, false) as LayoutItem;
		}
		BaseLayoutItem GetItem(int index, bool isGroup) {
			int count = 0;
			for(int i = 0; i < Count; i++) {
				if(this[i].IsGroup == isGroup) {
					if(index == count) return this[i];
					count++;
				}
			}
			return null;
		}
		int GetNeighborWigth(LayoutRectangle lrect, LayoutType lt, InsertLocation il) {
			bool haveZero = false;
			foreach(BaseLayoutItem item in this) {
				LayoutRectangle irect = new LayoutRectangle(item.Bounds, lt);
				if(il == InsertLocation.Before) {
					Point testPoint = new Point(lrect.X, lrect.Y);
					if(irect.Right == testPoint.X && irect.Y == testPoint.Y) {
						if(irect.Width == 0)
							haveZero = true;
						else
							return irect.Width;
					}
				}
				else {
					Point testPoint = new Point(lrect.Right, lrect.Y);
					if(irect.X == testPoint.X && irect.Y == testPoint.Y) {
						if(irect.Width == 0)
							haveZero = true;
						else
							return irect.Width;
					}
				}
			}
			return haveZero ? 0 : -1;
		}
		Rectangle CreateRectangle(Rectangle rect, LayoutType lt, InsertLocation il) {
			LayoutRectangle newRect = new LayoutRectangle(rect, lt);
			int dif = GetNeighborWigth(newRect, lt, il);
			if(dif == -1)
				return Rectangle.Empty;
			newRect.Width += dif;
			if(il == InsertLocation.Before)
				newRect.X = newRect.X - dif;
			return newRect.Rectangle;
		}
		bool IsRectangleContainsAllItsItems(Rectangle rect) {
			foreach(BaseLayoutItem item in this) {
				Rectangle trect = rect;
				trect.Intersect(item.Bounds);
				if(trect.Height != 0 && trect.Width != 0 && trect != item.Bounds)
					return false;
			}
			return true;
		}
		bool CanMergeItems(BaseItemCollection itemCollection, LayoutType lt, InsertLocation il) {
			Rectangle rect = itemCollection.ItemsBounds;
			do {
				rect = CreateRectangle(rect, lt, il);
				if(rect == Rectangle.Empty) return false;
			} while(!IsRectangleContainsAllItsItems(rect));
			return true;
		}
		protected internal bool CanMergeItems(BaseItemCollection itemCollection) {
			if(itemCollection == null) return false;
			if(this.Count == itemCollection.Count) return true;
			if(CanMergeItems(itemCollection, LayoutType.Horizontal, InsertLocation.Before)) return true;
			if(CanMergeItems(itemCollection, LayoutType.Horizontal, InsertLocation.After)) return true;
			if(CanMergeItems(itemCollection, LayoutType.Vertical, InsertLocation.Before)) return true;
			if(CanMergeItems(itemCollection, LayoutType.Vertical, InsertLocation.After)) return true;
			return false;
		}
		protected internal LayoutRectangle GetLayoutItemsBounds(LayoutType layoutType) {
			return new LayoutRectangle(ItemsBounds, layoutType);
		}
		protected override void OnRemoveComplete(int position, object item) {
			OnChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected override void OnInsertComplete(int position, object item) {
			OnChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected virtual void OnChanged(CollectionChangeEventArgs e) {
			if(Changed != null) Changed(this, e);
		}
		public BaseLayoutItem FindByName(string name) {
			foreach(BaseLayoutItem item in this) {
				if(item.Name == name) return item;
			}
			return null;
		}
		IEnumerator<BaseLayoutItem> IEnumerable<BaseLayoutItem>.GetEnumerator() {
			foreach(BaseLayoutItem item in InnerList)
				yield return item;
		}
	}
	public class LayoutItemEventArgs : EventArgs {
		BaseLayoutItem item;
		public LayoutItemEventArgs(BaseLayoutItem item) {
			this.item = item;
		}
		public BaseLayoutItem Item {
			get { return item; }
		}
	}
	public delegate void LayoutItemEventHandler(
		object sender, LayoutItemEventArgs e
	);
}
