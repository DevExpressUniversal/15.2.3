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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Docking {
	public static class LayoutItemsHelper {
		internal static Size GetResizingMinSize(FloatGroup fGroup) {
			if(fGroup.Items.Count > 0) {
				LayoutGroup group = fGroup[0] as LayoutGroup;
				if(group != null && !group.IgnoreOrientation) {
					bool horz = group.Orientation == System.Windows.Controls.Orientation.Horizontal;
					return new Size(horz ? 42 * group.Items.Count + 16 : 82,
						(horz ? 42 : 42 * group.Items.Count) + 32);
				}
			}
			return MathHelper.MeasureMinSize(new Size[] { new Size(82, 42), fGroup.ActualMinSize });
		}
		internal static Size GetResizingMaxSize(FloatGroup fGroup) {
			return fGroup.ActualMaxSize;
		}
		internal static bool IsActuallyVisibleInTree(BaseLayoutItem item) {
			if(item == null) return false;
			var def = DefinitionsHelper.GetDefinition(item);
			return def != null && !DefinitionsHelper.IsZero(def);
		}
#if !SILVERLIGHT
		internal static DependencyObject FindElementInVisualTree(DependencyObject root, Predicate<DependencyObject> filter, Predicate<DependencyObject> result) {
			using(var enumerator = new VisualTreeEnumeratorWithConditionalStop(root, filter)) {
				while(enumerator.MoveNext()) {
					var dObj = enumerator.Current;
					if(result != null && result(dObj)) return dObj;
				}
			}
			return null;
		}
#endif
		internal static LayoutGroup GetOwnerGroup(this LayoutGroup group) {
			LayoutGroup owner = LayoutGroup.GetOwnerGroup(group);
			return owner ?? group;
		}
		internal static bool IsRoot(this LayoutGroup group) {
			return group != null && group.IsRootGroup;
		}
		public static bool IsInTree(this BaseLayoutItem item) {
			LayoutGroup root = item.GetRoot();
			return IsRoot(root) && !root.IsUngroupped;
		}
		internal static bool GetIsDocumentHost(this BaseLayoutItem item) {
			return item is LayoutGroup ? ((LayoutGroup)item).GetIsDocumentHost() : false;
		}
		internal static bool GetAllowDockToDocumentGroup(this BaseLayoutItem item) {
			if(item is LayoutGroup)
				return ((LayoutGroup)item).Items.AllowDockToDocumentGroup();
			return item is LayoutPanel && ((LayoutPanel)item).AllowDockToDocumentGroup;
		}
		public static bool IsEmptyLayoutGroup(BaseLayoutItem target) {
			LayoutGroup group = target as LayoutGroup;
			if(group != null && (group.Items.Count == 0)) {
				return group.ItemType == LayoutItemType.Group;
			}
			return false;
		}
		public static UIElement GetUIElement(this BaseLayoutItem item) {
			if(item is LayoutPanel)
				return ((LayoutPanel)item).Control;
			if(item is LayoutControlItem)
				return ((LayoutControlItem)item).Control;
			if(item is LabelItem)
				return ((LabelItem)item).Content as UIElement;
			return null;
		}
		public static void UpdateZIndexes(DocumentGroup dGroup) {
			BaseLayoutItem[] items = dGroup.GetItems();
			BaseLayoutItem selectedItem = dGroup.SelectedItem;
			if(items.Length > 0 && selectedItem != null) {
				UpdateZIndexes(items, selectedItem);
			}
		}
		public static void UpdateZIndexes(BaseLayoutItem[] items, BaseLayoutItem item) {
			items = SortByZIndex(items);
			int index = Array.IndexOf(items, item);
			int size = items.Length - 1;
			if(index < size) {
				Array.Copy(items, index + 1, items, index, size - index);
				items[size] = item;
			}
			for(int i = 0; i < items.Length; i++) {
				items[i].ZIndex = i;
			}
		}
		internal static BaseLayoutItem[] SortByZIndex(BaseLayoutItem[] items) {
			return items.OrderBy(item => item.ZIndex).ToArray();
		}
		static int ZIndexCompare(BaseLayoutItem item1, BaseLayoutItem item2) {
			if(item1 == item2) return 0;
			return item1.ZIndex.CompareTo(item2.ZIndex);
		}
		public static BaseLayoutItem[] GetItems(this LayoutGroup group) {
			BaseLayoutItem[] items = new BaseLayoutItem[group.Items.Count];
			group.Items.CopyTo(items, 0);
			return items;
		}
		public static BaseLayoutItem[] GetItems(this FloatGroupCollection collection) {
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			CollectionHelper.Accept(collection, (fGroup) => fGroup.Accept(items.Add));
			return items.ToArray();
		}
		public static BaseLayoutItem[] GetItems(this AutoHideGroupCollection collection) {
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			CollectionHelper.Accept(collection, (ahGroup) => ahGroup.Accept(items.Add));
			return items.ToArray();
		}
		internal static IEnumerable<BaseLayoutItem> GetNestedPanels(this LayoutGroup group) {
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			group.Accept(items.Add);
			return items.ToArray();
		}
		public static LayoutGroup GetRoot(this BaseLayoutItem item) {
			if(item == null) return null;
			BaseLayoutItem root = item;
			while(root.Parent != null) {
				root = root.Parent;
			}
			return root as LayoutGroup;
		}
		internal static Size GetSize(this BaseLayoutItem item) {
			return GetSize(item, new Size(200, 200));
		}
		internal static Size GetSize(this BaseLayoutItem item, Size defaultSize) {
			double w = item.ItemWidth.IsAbsolute ? item.ItemWidth.Value : defaultSize.Width;
			double h = item.ItemHeight.IsAbsolute ? item.ItemHeight.Value : defaultSize.Height;
			return MathHelper.MeasureSize(item.ActualMinSize, item.ActualMaxSize, new Size(w, h));
		}
		public static bool IsParent(BaseLayoutItem item, BaseLayoutItem parent) {
			if(item != null && parent != null) {
				BaseLayoutItem pr = item.Parent;
				while(pr != null) {
					if(pr == parent) return true;
					pr = pr.Parent;
				}
			}
			return false;
		}
		internal static BaseLayoutItem GetNextItem(BaseLayoutItem item) {
			if(item == null || item.Parent == null) return null;
			BaseLayoutItemCollection items = item.Parent.Items;
			BaseLayoutItem next = null, prev = null;
			int index = items.IndexOf(item);
			if(index - 1 >= 0) prev = items[index - 1];
			if(index + 1 < items.Count) next = items[index + 1];
			return prev ?? next;
		}
		internal static bool? IsNextNeighbour(BaseLayoutItem item, BaseLayoutItem next) {
			if(next == null || item == null || next.Parent == null || item.Parent == null) return null;
			if(next == item || next.Parent != item.Parent) return null;
			LayoutGroup parent = next.Parent;
			int index1 = parent.Items.IndexOf(next);
			int index2 = parent.Items.IndexOf(item);
			if(Math.Abs(index1 - index2) == 1) {
				return index1 - index2 == 1;
			}
			return null;
		}
		internal static bool AreInSameGroup(BaseLayoutItem item1, BaseLayoutItem item2) {
			return item1 != null && item2 != null && item1.Parent != null && item2.Parent != null && item1.Parent == item2.Parent;
		}
		internal static bool AreInSameGroup(BaseLayoutItem[] items) {
			if(items == null || items.Length <= 0) return false;
			BaseLayoutItem item0 = items[0];
			foreach(BaseLayoutItem item in items) {
				if(!AreInSameGroup(item0, item)) return false;
			}
			return true;
		}
		public static DockLayoutManager GetDockLayoutManager(this BaseLayoutItem item) {
			return item != null ? item.Manager : null;
		}
		internal static DockLayoutManager FindDockLayoutManager(this BaseLayoutItem item) {
			if(item.Manager != null) return item.Manager;
			LayoutGroup root = item.GetRoot();
			if(root != null) {
				return root.Manager ?? (root.ParentPanel != null ? root.ParentPanel.FindDockLayoutManager() : null);
			}
			return null;
		}
		public static IEnumerator<BaseLayoutItem> GetEnumerator(LayoutGroup group) {
			return new LayoutEnumerator(group, null);
		}
		public static IEnumerator<BaseLayoutItem> GetEnumerator(LayoutGroup group, Predicate<BaseLayoutItem> filter) {
			return new LayoutEnumerator(group, filter);
		}
		public static IEnumerator<DependencyObject> GetEnumerator(DependencyObject element, Predicate<DependencyObject> filter) {
			return new IUIElementVisualTreeEnumerator(element, filter);
		}
		public static T GetChild<T>(DependencyObject element) where T : DependencyObject {
			if(element != null) {
				if(VisualTreeHelper.GetChildrenCount(element) == 1) {
					return VisualTreeHelper.GetChild(element, 0) as T;
				}
			}
			return null;
		}
		public static T GetTemplateChild<T>(DependencyObject element) where T : DependencyObject {
			T result = null;
			using(var e = GetEnumerator(element, null)) {
				while(e.MoveNext()) {
					if(e.Current is T) {
						result = e.Current as T;
						break;
					}
				}
			}
			return result;
		}
		public static T GetTemplateChild<T>(DependencyObject element, bool acceptRoot) where T : DependencyObject {
			T result = null;
			using(var e = GetEnumerator(element, null)) {
				while(e.MoveNext()) {
					if(e.Current is T) {
						if(acceptRoot || e.Current != element) {
							result = e.Current as T;
							break;
						}
					}
				}
			}
			return result;
		}
		public static T GetVisualChild<T>(DependencyObject element) where T : DependencyObject {
			T result = null;
			var e = new DevExpress.Xpf.Core.Native.VisualTreeEnumerator(element);
			while(e.MoveNext()) {
				if(e.Current is T) {
					result = e.Current as T;
					break;
				}
			}
			return result;
		}
		public static IUIElement GetIUIParent(DependencyObject dObj) {
			if(dObj == null) return null;
			DependencyObject parent = GetVisualParent(dObj);
			while(parent != null) {
				if(parent is Layout.Core.IUIElement) break;
				parent = GetVisualParent(parent);
			}
			return (IUIElement)parent;
		}
		public static T GetTemplateParent<T>(DependencyObject dObj) where T : DependencyObject {
			if(dObj == null) return null;
			DependencyObject parent = GetVisualParent(dObj);
			while(parent != null) {
				if(parent is T || parent is Layout.Core.IUIElement) break;
				parent = GetVisualParent(parent);
			}
			return parent as T;
		}
		public static DependencyObject GetVisualParent(DependencyObject dObj) {
			return System.Windows.Media.VisualTreeHelper.GetParent(dObj);
		}
		public static bool IsTemplateChild<T>(DependencyObject dObj, T root) where T : DependencyObject {
			return Core.Native.LayoutHelper.FindParentObject<T>(dObj) == root;
		}
		class LayoutEnumerator : IEnumerator<BaseLayoutItem> {
			LayoutGroup Root;
			Stack<BaseLayoutItem> Stack;
			Predicate<BaseLayoutItem> Filter;
			public LayoutEnumerator(LayoutGroup rootGroup, Predicate<BaseLayoutItem> filter) {
				Stack = new Stack<BaseLayoutItem>(8);
				Filter = filter;
				Root = rootGroup;
			}
			public void Dispose() {
				Reset();
				Stack = null;
				Filter = null;
			}
			#region IEnumerator Members
			BaseLayoutItem current;
			object System.Collections.IEnumerator.Current {
				get { return current; }
			}
			public BaseLayoutItem Current {
				get { return current; }
			}
			public bool MoveNext() {
				if(current == null) {
					current = Root;
				}
				else {
					LayoutGroup group = current as LayoutGroup;
					if(group != null) {
						BaseLayoutItem[] children = group.GetItems();
						if(children.Length > 0) {
							for(int i = 0; i < children.Length; i++) {
								BaseLayoutItem child = children[(children.Length - 1) - i];
								if(Filter == null || Filter(child))
									Stack.Push(child);
							}
						}
					}
					current = Stack.Count > 0 ? Stack.Pop() : null;
				}
				return current != null;
			}
			public void Reset() {
				Stack.Clear();
				current = null;
			}
			#endregion
		}
		class IUIElementVisualTreeEnumerator : IEnumerator<DependencyObject> {
			DependencyObject Root;
			Stack<DependencyObject> Stack;
			Predicate<DependencyObject> Filter;
			public IUIElementVisualTreeEnumerator(DependencyObject root, Predicate<DependencyObject> filter) {
				Stack = new Stack<DependencyObject>(16);
				Filter = filter;
				Root = root;
			}
			public void Dispose() {
				Reset();
				Stack = null;
				Filter = null;
			}
			#region IEnumerator Members
			DependencyObject current;
			object System.Collections.IEnumerator.Current {
				get { return current; }
			}
			public DependencyObject Current {
				get { return current; }
			}
			public bool MoveNext() {
				if(current == null) {
					current = Root;
				}
				else {
					int count = VisualTreeHelper.GetChildrenCount(current);
					DependencyObject[] children = new DependencyObject[count];
					for(int i = 0; i < count; i++) {
						children[i] = VisualTreeHelper.GetChild(current, i);
					}
					if(children.Length > 0) {
						for(int i = 0; i < children.Length; i++) {
							DependencyObject child = children[(children.Length - 1) - i];
							if(child is IUIElement) continue;
							if(Filter == null || Filter(child))
								Stack.Push(child);
						}
					}
					current = Stack.Count > 0 ? Stack.Pop() : null;
				}
				return current != null;
			}
			public void Reset() {
				Stack.Clear();
				current = null;
			}
			#endregion
		}
		public static bool IsDockItem(BaseLayoutItem item) {
			switch(item.ItemType) {
				case LayoutItemType.Document:
				case LayoutItemType.Panel:
				case LayoutItemType.AutoHidePanel:
					return true;
			}
			return false;
		}
		public static bool IsLayoutItem(BaseLayoutItem item) {
			switch(item.ItemType) {
				case LayoutItemType.ControlItem:
				case LayoutItemType.FixedItem:
				case LayoutItemType.LayoutSplitter:
				case LayoutItemType.EmptySpaceItem:
				case LayoutItemType.Separator:
				case LayoutItemType.Label:
					return true;
			}
			return false;
		}
		internal static bool IsDataBound(BaseLayoutItem item) {
			LayoutGroup parent = item.Parent;
#if !SILVERLIGHT
			return parent != null && (parent.ItemsSource != null || System.Windows.Data.BindingOperations.IsDataBound(parent, LayoutGroup.ItemsSourceProperty));
#else
			return parent != null && parent.ItemsSource != null;
#endif
		}
		internal static bool CanRecognizeAccessKey(BaseLayoutItem item) {
			return item is LayoutControlItem;
		}
		internal static LayoutGroup GetFirstNoBorderParent(BaseLayoutItem item) {
			if(item == null) return null;
			LayoutGroup parent = item.Parent;
			while(parent != null && parent.GroupBorderStyle != GroupBorderStyle.NoBorder) {
				parent = parent.Parent;
			}
			return parent;
		}
		internal static bool IsResizable(BaseLayoutItem item, bool isHorizontal = true) {
			bool result = item.AllowSizing && !(item is LayoutSplitter) && !(item is SeparatorItem);
			result = result && (isHorizontal ? item.ActualMinSize.Width != item.ActualMaxSize.Width :
				item.ActualMinSize.Height != item.ActualMaxSize.Height);
			if(result && item is LayoutGroup) result = IsResizableGroup((LayoutGroup)item, isHorizontal);
			return result;
		}
		static bool IsResizableGroup(LayoutGroup group, bool isHorizontal) {
			if(group == null || IsEmptyGroup(group) || !IsActuallyVisibleInTree(group)) return false;
			if(group is DocumentGroup && !((DocumentGroup)group).IsTabbed) return true;
			if(!isHorizontal && group.GroupBorderStyle == GroupBorderStyle.GroupBox && !group.IsExpanded) return false;
			foreach(BaseLayoutItem item in group.Items) {
				if(IsResizable(item)) return true;
			}
			return !group.HasItems;
		}
		static bool IsEmptyGroup(LayoutGroup group) {
			if(group == null || group.HasNotCollapsedItems) return false;
			DocumentGroup dGroup = group as DocumentGroup;
			return (group.ItemType == LayoutItemType.TabPanelGroup || group.ItemType == LayoutItemType.Group && group.GroupBorderStyle == GroupBorderStyle.NoBorder);
		}
		public static void BeginWidthAnimation(this BaseLayoutItem layoutItem, GridLength from, GridLength to, TimeSpan duration, EventHandler completed = null) {
			BeginGridLengthAnimation(layoutItem, BaseLayoutItem.ItemWidthProperty, from, to, duration, completed);
		}
		public static void BeginHeightAnimation(this BaseLayoutItem layoutItem, GridLength from, GridLength to, TimeSpan duration, EventHandler completed = null) {
			BeginGridLengthAnimation(layoutItem, BaseLayoutItem.ItemHeightProperty, from, to, duration, completed);
		}
		static void BeginGridLengthAnimation(BaseLayoutItem layoutItem, DependencyProperty property, GridLength from, GridLength to, TimeSpan duration, EventHandler completed) {
			var animation = CreateAnimation(from, to, duration);
			animation.Completed += delegate(object sender, EventArgs e) {
				layoutItem.BeginAnimation(property, null);
				layoutItem.SetValue(property, to);
			};
			if(completed != null)
				animation.Completed += completed;
			layoutItem.BeginAnimation(property, animation);
		}
		static GridLengthAnimation CreateAnimation(GridLength from, GridLength to, TimeSpan duration) {
			return new GridLengthAnimation()
			{
				From = from,
				To = to,
				Duration = new Duration(duration),
				FillBehavior = System.Windows.Media.Animation.FillBehavior.Stop
			};
		}
		public static bool AreTreePathEqual(BaseLayoutItem item1, BaseLayoutItem item2) {
			if(item1 == item2) return true;
			return AreEqual(GetPath(item1), GetPath(item2));
		}
		static bool AreEqual(PathNode[] path1, PathNode[] path2) {
			if(path1 == null || path2 == null) return false;
			if(path1.Length != path2.Length) return false;
			for(int i = path1.Length - 1; i >= 0; i--) {
				if(path1[i].Type != path2[i].Type) return false;
				if(path1[i].Index != path2[i].Index) return false;
			}
			return true;
		}
		static PathNode[] GetPath(BaseLayoutItem item) {
			List<PathNode> path = new List<PathNode>();
			while(item != null) {
				PathNode node = PathNode.FromItem(item);
				path.Add(node);
				LayoutGroup parentGroup = item.Parent;
				if(parentGroup != null) {
					node.Index = parentGroup.Items.IndexOf(item);
				}
				else {
					LayoutGroup group = item as LayoutGroup;
					if(group != null) {
						item = group.ParentPanel;
						continue;
					}
				}
				item = parentGroup;
			}
			return path.ToArray();
		}
		class PathNode {
			public int Index = -1;
			public Type Type;
			public static PathNode FromItem(BaseLayoutItem item) {
				return new PathNode() { Type = item.GetType() };
			}
		}
	}
}
