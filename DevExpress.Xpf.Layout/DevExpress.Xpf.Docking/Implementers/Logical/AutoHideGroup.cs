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

using DevExpress.Mvvm.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Layout.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking {
	public class AutoHideGroup : LayoutGroup {
		#region static
		public static readonly DependencyProperty DockTypeProperty;
		public static readonly DependencyProperty AutoHideSizeProperty;
		public static readonly DependencyProperty AutoHideSpeedProperty;
		public static readonly DependencyProperty AutoHideTypeProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty HasAutoHideSizeProperty;
		public static readonly DependencyProperty SizeToContentProperty;
		readonly static RoutedEvent DockTypeChangedEvent;
		static AutoHideGroup() {
			var dProp = new DependencyPropertyRegistrator<AutoHideGroup>();
			dProp.OverrideMetadata(AllowSelectionProperty, false);
			dProp.OverrideMetadata(AllowExpandProperty, false);
			dProp.OverrideMetadata(IsExpandedPropertyKey, false);
			dProp.Register("DockType", ref DockTypeProperty, SWC.Dock.Left,
				(dObj, e) => ((AutoHideGroup)dObj).OnDockTypeChanged((SWC.Dock)e.OldValue, (SWC.Dock)e.NewValue));
			dProp.RegisterAttached("AutoHideSize", ref AutoHideSizeProperty, new Size(150, 150), OnAutoHideSizeChanged, CoerceAutoHideSize);
			dProp.Register("AutoHideSpeed", ref AutoHideSpeedProperty, 150);
			dProp.RegisterAttached("AutoHideType", ref AutoHideTypeProperty, AutoHideType.Default);
			dProp.RegisterAttached("HasAutoHideSize", ref HasAutoHideSizeProperty, false);
			dProp.RegisterAttached("SizeToContent", ref SizeToContentProperty, SizeToContent.Manual);
			DockTypeChangedEvent = EventManager.RegisterRoutedEvent("DockTypeChanged", RoutingStrategy.Direct, typeof(DockTypeChangedEventHandler), typeof(AutoHideGroup));
		}
		[XtraSerializableProperty]
		public static Size GetAutoHideSize(DependencyObject target) {
			return (Size)target.GetValue(AutoHideSizeProperty);
		}
		public static void SetAutoHideSize(DependencyObject target, Size value) {
			target.SetValue(AutoHideSizeProperty, value);
		}
		[XtraSerializableProperty]
		public static AutoHideType GetAutoHideType(DependencyObject obj) {
			return (AutoHideType)obj.GetValue(AutoHideTypeProperty);
		}
		public static void SetAutoHideType(DependencyObject obj, AutoHideType value) {
			obj.SetValue(AutoHideTypeProperty, value);
		}
		static object CoerceAutoHideSize(DependencyObject dObj, object value) {
			if(!(dObj is BaseLayoutItem)) return value;
			BaseLayoutItem item = (BaseLayoutItem)dObj;
			return MathHelper.MeasureSize(item.ActualMinSize, item.ActualMaxSize, (Size)value);
		}
		static void OnAutoHideSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Size s = (Size)e.NewValue;
			AutoHideGroup.SetHasAutoHideSize(d, MathHelper.IsConstraintValid(s.Width) || MathHelper.IsConstraintValid(s.Height));
		}
		internal static bool GetHasAutoHideSize(DependencyObject target) {
			return (bool)target.GetValue(HasAutoHideSizeProperty);
		}
		internal static void SetHasAutoHideSize(DependencyObject target, bool value) {
			target.SetValue(HasAutoHideSizeProperty, value);
		}
		[XtraSerializableProperty]
		public static SizeToContent GetSizeToContent(DependencyObject target) {
			return (SizeToContent)target.GetValue(SizeToContentProperty);
		}
		public static void SetSizeToContent(DependencyObject target, SizeToContent value) {
			target.SetValue(SizeToContentProperty, value);
		}
		#endregion static
		public AutoHideGroup() {
		}
		protected override void CoerceSizes() {
			base.CoerceSizes();
			CoerceValue(AutoHideSizeProperty);
		}
		protected override Size CalcMinSizeValue(Size value) {
			return value;
		}
		protected override Size CalcMaxSizeValue(Size value) {
			return value;
		}
		public event DockTypeChangedEventHandler DockTypeChanged;
		protected virtual void OnDockTypeChanged(SWC.Dock prev, SWC.Dock value) {
			if(DockTypeChanged != null) {
				DockTypeChanged(this, new DockTypeChangedEventArgs(value, prev) { RoutedEvent = DockTypeChangedEvent, Source = this });
			}
		}
		protected override BaseLayoutItemCollection CreateItems() {
			return new AutoHideItemsCollection(this);
		}
		protected internal override bool IgnoreOrientation { get { return true; } }
		protected override bool CanCreateItemsInternal() { return false; }
		protected override LayoutItemType GetLayoutItemTypeCore() {
			return LayoutItemType.AutoHideGroup;
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("AutoHideGroupDockType"),
#endif
		XtraSerializableProperty, Category("Layout")]
		public SWC.Dock DockType {
			get { return (SWC.Dock)GetValue(DockTypeProperty); }
			set { SetValue(DockTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("AutoHideGroupAutoHideSize"),
#endif
		XtraSerializableProperty, Category("Layout")]
		public Size AutoHideSize {
			get { return (Size)GetValue(AutoHideSizeProperty); }
			set { SetValue(AutoHideSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("AutoHideGroupAutoHideSpeed"),
#endif
		XtraSerializableProperty]
		public int AutoHideSpeed {
			get { return (int)GetValue(AutoHideSpeedProperty); }
			set { SetValue(AutoHideSpeedProperty, value); }
		}
		protected override BaseLayoutItem CoerceSelectedItem(BaseLayoutItem item) {
			return IsValid(SelectedTabIndex) ? Items[SelectedTabIndex] : item;
		}
		protected override void OnIsExpandedChanged(bool expanded) {
			base.OnIsExpandedChanged(expanded);
			if(Manager != null) {
				ItemEventArgs ea = expanded ?
					(ItemEventArgs)new DockItemExpandedEventArgs(this) :
					(ItemEventArgs)new DockItemCollapsedEventArgs(this);
				Manager.RaiseEvent(ea);
			}
		}
		protected override void OnSelectedItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			base.OnSelectedItemChanged(item, oldItem);
			LayoutPanel oldPanel = oldItem as LayoutPanel;
			LayoutPanel newPanel = item as LayoutPanel;
			oldPanel.Do(x => x.AutoHideExpandState = AutoHideExpandState.Hidden);
		}
		protected override void OnAllowDockChanged(bool value) {
			base.OnAllowDockChanged(value);
			foreach(BaseLayoutItem item in Items)
				item.CoerceValue(IsPinButtonVisibleProperty);
		}
		internal Size GetActualAutoHideSize(BaseLayoutItem item) {
			if(!AutoHideGroup.GetHasAutoHideSize(item)) {
				LayoutPanel panel = item as LayoutPanel;
				Size layoutSize = panel != null ? panel.LayoutSizeBeforeHide : Size.Empty;
				return MathHelper.IsEmpty(layoutSize) ? item.GetSize(AutoHideSize) : layoutSize;
			}
			return AutoHideGroup.GetAutoHideSize(item);
		}
		internal bool HasPersistentGroups = false;
		internal void OnOwnerCollectionChanged() {
			if(Manager != null && !Manager.AutoHideGroups.Contains(this)) {
				DockLayoutManager.RemoveLogicalChild(Manager, this);
			}
		}
		protected override void OnDockLayoutManagerChanged(DockLayoutManager oldValue, DockLayoutManager newValue) {
			base.OnDockLayoutManagerChanged(oldValue, newValue);
			if(newValue != null) DockLayoutManager.AddLogicalChild(newValue, this);
		}
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new UIAutomation.AutoHideGroupAutomationPeer(this);
		}
		#endregion UIAutomation
	}
	public class AutoHideGroupCollection : ObservableCollection<AutoHideGroup>, IDisposable {
		ObservableCollection<AutoHideGroup> leftItemsCore;
		ObservableCollection<AutoHideGroup> rightItemsCore;
		ObservableCollection<AutoHideGroup> topItemsCore;
		ObservableCollection<AutoHideGroup> bottomItemsCore;
		CompositeCollection mergedLeftItems;
		CompositeCollection mergedRightItems;
		CompositeCollection mergedTopItems;
		CompositeCollection mergedBottomItems;
		bool isDisposing;
		public AutoHideGroupCollection() {
			leftItemsCore = CreateTarget();
			rightItemsCore = CreateTarget();
			topItemsCore = CreateTarget();
			bottomItemsCore = CreateTarget();
			mergedLeftItems = CreateMergedTarget(leftItemsCore);
			mergedRightItems = CreateMergedTarget(rightItemsCore);
			mergedTopItems = CreateMergedTarget(topItemsCore);
			mergedBottomItems = CreateMergedTarget(bottomItemsCore);
		}
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		public void AddRange(AutoHideGroup[] items) {
			Array.ForEach(items, Add);
		}
		protected virtual void OnDisposing() {
			ClearItems();
			Clear(ref leftItemsCore);
			Clear(ref rightItemsCore);
			Clear(ref topItemsCore);
			Clear(ref bottomItemsCore);
		}
		static void Clear(ref ObservableCollection<AutoHideGroup> collection) {
			if(collection != null) collection.Clear();
			collection = null;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public CompositeCollection LeftItems { get { return mergedLeftItems; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public CompositeCollection RightItems { get { return mergedRightItems; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public CompositeCollection TopItems { get { return mergedTopItems; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public CompositeCollection BottomItems { get { return mergedBottomItems; } }
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach(AutoHideGroup item in e.NewItems) OnItemAdded(item);
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach(AutoHideGroup item in e.OldItems) OnItemRemoved(item);
					break;
				case NotifyCollectionChangedAction.Reset:
					GetTarget(SWC.Dock.Bottom).Clear();
					GetTarget(SWC.Dock.Left).Clear();
					GetTarget(SWC.Dock.Right).Clear();
					GetTarget(SWC.Dock.Top).Clear();
					break;
			}
		}
		void Subscribe(AutoHideGroup item) {
			item.DockTypeChanged += OnItemDockTypeChanged;
		}
		void Unsubscribe(AutoHideGroup item) {
			item.DockTypeChanged -= OnItemDockTypeChanged;
		}
		protected virtual void OnItemAdded(AutoHideGroup item) {
			GetTarget(item.DockType).Add(item);
			Unsubscribe(item);
			Subscribe(item);
			item.IsRootGroup = true;
		}
		protected virtual void OnItemRemoved(AutoHideGroup item) {
			Unsubscribe(item);
			GetTarget(item.DockType).Remove(item);
			item.IsRootGroup = false;
		}
		void OnItemDockTypeChanged(object sender, DockTypeChangedEventArgs e) {
			AutoHideGroup item = sender as AutoHideGroup;
			GetTarget(e.PrevValue).Remove(item);
			GetTarget(e.Value).Add(item);
		}
		protected virtual ObservableCollection<AutoHideGroup> CreateTarget() {
			return new ObservableCollection<AutoHideGroup>();
		}
		protected ObservableCollection<AutoHideGroup> GetTarget(SWC.Dock type) {
			ObservableCollection<AutoHideGroup> result = leftItemsCore;
			switch(type) {
				case SWC.Dock.Right: result = rightItemsCore; break;
				case SWC.Dock.Top: result = topItemsCore; break;
				case SWC.Dock.Bottom: result = bottomItemsCore; break;
			}
			return result;
		}
		internal ObservableCollection<AutoHideGroup> GetXtraTarget(SWC.Dock type) {
			ObservableCollection<AutoHideGroup> result = new ObservableCollection<AutoHideGroup>();
			foreach(var container in containers.Values) {
				var items = container.GetItems(type);
				items.ForEach(result.Add);
			}
			return result;
		}
		protected CompositeCollection GetMergetTarget(SWC.Dock type) {
			CompositeCollection result = LeftItems;
			switch(type) {
				case SWC.Dock.Right: result = RightItems; break;
				case SWC.Dock.Top: result = TopItems; break;
				case SWC.Dock.Bottom: result = BottomItems; break;
			}
			return result;
		}
		public BaseLayoutItem this[string name] {
			get { return Array.Find(this.GetItems(), (item) => item.Name == name); }
		}
		public AutoHideGroup[] ToArray() {
			AutoHideGroup[] groups = new AutoHideGroup[Count];
			Items.CopyTo(groups, 0);
			return groups;
		}
		protected override void InsertItem(int index, AutoHideGroup item) {
			base.InsertItem(index, item);
			NotifyItemAdded(item);
		}
		protected override void SetItem(int index, AutoHideGroup item) {
			base.SetItem(index, item);
			NotifyItemAdded(item);
		}
		protected override void RemoveItem(int index) {
			AutoHideGroup item = Items[index];
			base.RemoveItem(index);
			NotifyItemRemoved(item);
		}
		protected override void ClearItems() {
			var groups = this.ToArray();
			groups.ForEach(x => x.BeginLayoutChange());
			base.ClearItems();
			groups.ForEach(x => x.EndLayoutChange());
			for(int i = 0; i < groups.Length; i++)
				NotifyItemRemoved(groups[i]);
		}
		protected virtual void NotifyItemAdded(AutoHideGroup item) {
			if(item != null) item.OnOwnerCollectionChanged();
		}
		protected virtual void NotifyItemRemoved(AutoHideGroup item) {
			if(item != null) {
				item.Do((x) => x.BeginLayoutChange());
				item.OnOwnerCollectionChanged();
				item.Do((x) => x.EndLayoutChange());
			}
		}
		#region Merge
		Dictionary<object, ahCollectionContainer> containers = new Dictionary<object, ahCollectionContainer>();
		private ahCollectionContainer GetContainer(object key) {
			ahCollectionContainer container = null;
			if(!containers.TryGetValue(key, out container)) {
				container = new ahCollectionContainer();
				containers[key] = container;
				mergedBottomItems.Add(container.GetContainer(SWC.Dock.Bottom));
				mergedLeftItems.Add(container.GetContainer(SWC.Dock.Left));
				mergedRightItems.Add(container.GetContainer(SWC.Dock.Right));
				mergedTopItems.Add(container.GetContainer(SWC.Dock.Top));
			}
			return container;
		}
		internal void Merge(object key, AutoHideGroupCollection extraItems) {
			extraItems.OnMerge();
			ahCollectionContainer container = GetContainer(key);
			container.GetItems(SWC.Dock.Bottom).Clear();
			container.GetItems(SWC.Dock.Left).Clear();
			container.GetItems(SWC.Dock.Right).Clear();
			container.GetItems(SWC.Dock.Top).Clear();
			foreach(AutoHideGroup ahGroup in extraItems) {
				var items = container.GetItems(ahGroup.DockType);
				items.Add(ahGroup);
			}
		}
		internal void Unmerge(object key, AutoHideGroupCollection extraItems) {
			ahCollectionContainer container = null;
			if(!containers.TryGetValue(key, out container)) {
				container = new ahCollectionContainer();
				containers[key] = container;
			}
			container.GetItems(SWC.Dock.Bottom).Clear();
			container.GetItems(SWC.Dock.Left).Clear();
			container.GetItems(SWC.Dock.Right).Clear();
			container.GetItems(SWC.Dock.Top).Clear();
			extraItems.OnUnmerge();
		}
		void OnMerge() {
			LeftItems.Clear();
			RightItems.Clear();
			TopItems.Clear();
			BottomItems.Clear();
		}
		void OnUnmerge() {
			mergedLeftItems.Add(new CollectionContainer() { Collection = leftItemsCore });
			mergedRightItems.Add(new CollectionContainer() { Collection = rightItemsCore });
			mergedTopItems.Add(new CollectionContainer() { Collection = topItemsCore });
			mergedBottomItems.Add(new CollectionContainer() { Collection = bottomItemsCore });
		}
		CompositeCollection CreateMergedTarget(ObservableCollection<AutoHideGroup> items) {
			CompositeCollection composite = new CompositeCollection();
			composite.Add(new CollectionContainer() { Collection = items });
			return composite;
		}
		class ahCollectionContainer {
			ObservableCollection<AutoHideGroup> extraLeftItems;
			ObservableCollection<AutoHideGroup> extraRightItems;
			ObservableCollection<AutoHideGroup> extraTopItems;
			ObservableCollection<AutoHideGroup> extraBottomItems;
			CollectionContainer leftContainer;
			CollectionContainer rightContainer;
			CollectionContainer topContainer;
			CollectionContainer bottomContainer;
			public ahCollectionContainer() {
				extraLeftItems = CreateTarget();
				extraRightItems = CreateTarget();
				extraTopItems = CreateTarget();
				extraBottomItems = CreateTarget();
				bottomContainer = new CollectionContainer() { Collection = extraBottomItems };
				leftContainer = new CollectionContainer() { Collection = extraLeftItems };
				rightContainer = new CollectionContainer() { Collection = extraRightItems };
				topContainer = new CollectionContainer() { Collection = extraTopItems };
			}
			protected ObservableCollection<AutoHideGroup> CreateTarget() {
				return new ObservableCollection<AutoHideGroup>();
			}
			public ObservableCollection<AutoHideGroup> GetItems(SWC.Dock dock) {
				ObservableCollection<AutoHideGroup> result = extraLeftItems;
				switch(dock) {
					case SWC.Dock.Right: result = extraRightItems; break;
					case SWC.Dock.Top: result = extraTopItems; break;
					case SWC.Dock.Bottom: result = extraBottomItems; break;
				}
				return result;
			}
			public CollectionContainer GetContainer(SWC.Dock dock) {
				CollectionContainer result = leftContainer;
				switch(dock) {
					case SWC.Dock.Right: result = rightContainer; break;
					case SWC.Dock.Top: result = topContainer; break;
					case SWC.Dock.Bottom: result = bottomContainer; break;
				}
				return result;
			}
		}
		#endregion
	}
	public class AutoHideItemsCollection : BaseLayoutItemCollection {
		public AutoHideItemsCollection(AutoHideGroup group)
			: base(group) {
		}
		protected override void InsertItem(int index, BaseLayoutItem item) {
			if(item is LayoutGroup) {
				BaseLayoutItem[] panels = DockControllerHelper.Decompose(item);
				for(int i = 0; i < panels.Length; i++) {
					base.InsertItem(index + i, panels[i]);
				}
			}
			else base.InsertItem(index, item);
		}
		protected override void BeforeItemAdded(BaseLayoutItem item) {
			base.BeforeItemAdded(item);
			item.SetAutoHidden(true);
		}
		protected override void OnItemRemoved(BaseLayoutItem item) {
			base.OnItemRemoved(item);
			item.SetAutoHidden(false);
		}
		protected override void SetItem(int index, BaseLayoutItem item) {
			item.SetAutoHidden(true);
			base.SetItem(index, item);
		}
		protected override void ClearItems() {
			BaseLayoutItem[] panels = new BaseLayoutItem[Items.Count];
			Items.CopyTo(panels, 0);
			base.ClearItems();
			foreach(BaseLayoutItem item in panels)
				item.SetAutoHidden(false);
		}
	}
}
