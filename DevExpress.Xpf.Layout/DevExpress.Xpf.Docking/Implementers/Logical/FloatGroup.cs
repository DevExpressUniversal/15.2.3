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
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Docking {
	public class FloatGroup : LayoutGroup, IClosable {
		#region static
		public static readonly DependencyProperty IsOpenProperty;
		public static readonly DependencyProperty FloatLocationProperty;
		static readonly DependencyPropertyKey BorderStylePropertyKey;
		public static readonly DependencyProperty BorderStyleProperty;
		static readonly DependencyPropertyKey IsMaximizedPropertyKey;
		public static readonly DependencyProperty IsMaximizedProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ActualVisibilityProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty IsActuallyVisibleProperty;
		static readonly DependencyPropertyKey IsActuallyVisiblePropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty CanMaximizeProperty;
		static readonly DependencyPropertyKey CanMaximizePropertyKey;
		public static readonly DependencyProperty SizeToContentProperty;
		static FloatGroup() {
			var dProp = new DependencyPropertyRegistrator<FloatGroup>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.OverrideMetadata(AllowSelectionProperty, false);
			dProp.Register("IsOpen", ref IsOpenProperty, true);
			dProp.Register("FloatLocation", ref FloatLocationProperty, new Point(0, 0)
#if DEBUG
				, OnFloatLocationChanged
#endif
			);
			dProp.RegisterReadonly("BorderStyle", ref BorderStylePropertyKey, ref BorderStyleProperty, FloatGroupBorderStyle.Empty,
				(dObj, e) => ((FloatGroup)dObj).OnBorderStyleChanged(),
				(dObj, value) => ((FloatGroup)dObj).CoerceBorderStyle((FloatGroupBorderStyle)value));
			dProp.RegisterReadonly("IsMaximized", ref IsMaximizedPropertyKey, ref IsMaximizedProperty, false,
				(dObj, e) => ((FloatGroup)dObj).OnIsMaximizedChanged((bool)e.NewValue),
				(dObj, value) => ((FloatGroup)dObj).CoerceIsMaximized((bool)value));
			dProp.Register("ActualVisibility", ref ActualVisibilityProperty, Visibility.Visible,
				(dObj, e) => ((FloatGroup)dObj).OnActualVisibilityChanged((Visibility)e.OldValue, (Visibility)e.NewValue),
				(dObj, value) => ((FloatGroup)dObj).CoerceActualVisibility((Visibility)value));
			dProp.RegisterReadonly("IsActuallyVisible", ref IsActuallyVisiblePropertyKey, ref IsActuallyVisibleProperty, true);
			dProp.RegisterReadonly("CanMaximize", ref CanMaximizePropertyKey, ref CanMaximizeProperty, true, null,
				(dObj, value) => ((FloatGroup)dObj).CoerceCanMaximize((bool)value));
			dProp.RegisterAttached("SizeToContent", ref SizeToContentProperty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SizeToContent.Manual, OnSizeToContentChanged, CoerceSizeToContent);
		}
#if DEBUG
		static void OnFloatLocationChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			((FloatGroup)dObj).OnFloatLocationChanged((Point)e.OldValue, (Point)e.NewValue);
		}
		protected virtual void OnFloatLocationChanged(Point oldValue, Point newValue) { }
#endif
		public static SizeToContent GetSizeToContent(DependencyObject target) {
			return (SizeToContent)target.GetValue(SizeToContentProperty);
		}
		public static void SetSizeToContent(DependencyObject target, SizeToContent value) {
			target.SetValue(SizeToContentProperty, value);
		}
		static void OnSizeToContentChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			FloatGroup floatGroup = dObj as FloatGroup;
			if(floatGroup != null)
				floatGroup.OnSizeToContentChanged((SizeToContent)e.OldValue, (SizeToContent)e.NewValue);
			else
				(dObj as BaseLayoutItem).Do(x => x.UpdateSizeToContent());
		}
		static object CoerceSizeToContent(DependencyObject dObj, object baseValue) {
			FloatGroup floatGroup = dObj as FloatGroup;
			return floatGroup != null ? floatGroup.CoerceSizeToContent((SizeToContent)baseValue) : baseValue;
		}
		#endregion static
		protected override void OnCreate() {
			base.OnCreate();
			CoerceValue(ActualVisibilityProperty);
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("FloatGroupIsOpen"),
#endif
		XtraSerializableProperty]
		public bool IsOpen {
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("FloatGroupFloatLocation"),
#endif
		XtraSerializableProperty, Category("Layout")]
		public Point FloatLocation {
			get { return (Point)GetValue(FloatLocationProperty); }
			set { SetValue(FloatLocationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("FloatGroupBorderStyle"),
#endif
 Category("Content")]
		public FloatGroupBorderStyle BorderStyle {
			get { return (FloatGroupBorderStyle)GetValue(BorderStyleProperty); }
			internal set { SetValue(BorderStylePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("FloatGroupIsMaximized")]
#endif
		public bool IsMaximized {
			get { return (bool)GetValue(IsMaximizedProperty); }
			private set { SetValue(IsMaximizedPropertyKey, value); }
		}
		public SizeToContent SizeToContent {
			get { return (SizeToContent)GetValue(SizeToContentProperty); }
			set { SetValue(SizeToContentProperty, value); }
		}
		internal bool IsActuallyVisible {
			get { return (bool)GetValue(IsActuallyVisibleProperty); }
		}
		public Visibility ActualVisibility {
			get { return (Visibility)GetValue(ActualVisibilityProperty); }
		}
		internal bool CanMaximize {
			get { return (bool)GetValue(CanMaximizeProperty); }
		}
		internal Rect FloatBounds { get { return new Rect(FloatLocation, FloatSize); } }
		LockHelper _MaximizationLockHelper;
		internal LockHelper MaximizationLockHelper {
			get {
				if(_MaximizationLockHelper == null) _MaximizationLockHelper = new LockHelper(OnMaximizationUnlock);
				return _MaximizationLockHelper;
			}
		}
		Locker _AutoSizeLocker;
		internal Locker AutoSizeLocker {
			get {
				if(_AutoSizeLocker == null) _AutoSizeLocker = new Locker();
				return _AutoSizeLocker;
			}
		}
		protected virtual void OnSizeToContentChanged(SizeToContent oldValue, SizeToContent newValue) {
			if(HasSingleItem) (Items[0] as LayoutPanel).Do(x => SetSizeToContent(x, newValue));
		}
		protected virtual object CoerceSizeToContent(SizeToContent baseValue) {
			if(HasSingleItem && sizeToContentLocker) return GetSizeToContent(Items[0]);
			return baseValue;
		}
		protected override void OnFloatSizeChanged(Size newValue) {
			base.OnFloatSizeChanged(newValue);
			if(AutoSizeLocker || SizeToContent == SizeToContent.Manual || FloatingWindowLock.Return(x => x.IsLocked(FloatingWindowLock.LockerKey.FloatingBounds), () => false)) return;
			DisableSizeToContent();
		}
		Locker sizeToContentLocker = new Locker();
		protected internal override void UpdateSizeToContent() {
			using(sizeToContentLocker.Lock()) {
				CoerceValue(SizeToContentProperty);
			}
		}
		internal void DisableSizeToContent() {
			if(SizeToContent != SizeToContent.Manual)
				SizeToContent = SizeToContent.Manual;
		}
		internal void UpdateAutoFloatingSize(Size floatSize) {
			if(FloatSize != floatSize) {
				using(AutoSizeLocker.Lock()) {
					FloatSize = floatSize;
				}
			}
		}
		internal void FitSizeToContent(Size screenSize) {
			FloatSize = SizeToContentHelper.FitSizeToContent(SizeToContent, FloatSize, screenSize);
		}
		internal override void SetIsMaximized(bool isMaximized) {
			SetValue(IsMaximizedPropertyKey, isMaximized);
		}
		protected virtual void OnBorderStyleChanged() {
			CoerceValue(ActualMinSizeProperty);
		}
		protected virtual FloatGroupBorderStyle CoerceBorderStyle(FloatGroupBorderStyle borderStyle) {
			if(Items.Count > 0) {
				if(HasSingleItem) {
					return Items[0] is DocumentPanel ? FloatGroupBorderStyle.Empty : FloatGroupBorderStyle.Single;
				}
				else return FloatGroupBorderStyle.Form;
			}
			return borderStyle;
		}
		protected override void OnItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsCollectionChanged(sender, e);
			CoerceValue(BorderStyleProperty);
		}
		bool HasSingleDocument { get { return HasSingleItem && Items[0] is DocumentPanel; } }
		protected override Thickness CoerceActualMargin(Thickness value) {
			return HasSingleDocument ? (MathHelper.AreEqual(Margin, new Thickness(double.NaN)) ? value : Margin) : base.CoerceActualMargin(value);
		}
		internal bool IsDocumentHost { get { return this.GetNestedPanels().Count(x => x is DocumentPanel) > 0; } }
		protected override Size CalcGroupMinSize(Size[] minSizes, bool fHorz) {
			Size groupMinSize = base.CalcGroupMinSize(minSizes, fHorz);
			groupMinSize = CalcMinSizeWithBorderMargin(groupMinSize);
			return groupMinSize;
		}
		Size CalcMinSizeWithBorderMargin(Size minSize) {
			if(BorderStyle == FloatGroupBorderStyle.Single)
				return new Size(minSize.Width + 14, minSize.Height + 14);
			else
				return new Size(minSize.Width + 22, minSize.Height + 31);
		}
		protected override void OnAllowMaximizeChanged(bool value) {
			base.OnAllowMaximizeChanged(value);
			CoerceValue(CanMaximizeProperty);
		}
		protected virtual bool CoerceCanMaximize(bool canMaximize) {
			return IsMaximizable && AllowMaximize && (!HasSingleItem || (Items[0].IsMaximizable && Items[0].AllowMaximize));
		}
		protected virtual bool CoerceIsMaximized(bool maximized) {
			return maximized && CanMaximize;
		}
		protected virtual void OnIsMaximizableChanged(bool maximized) {
			CoerceValue(IsMaximizeButtonVisibleProperty);
			CoerceValue(IsRestoreButtonVisibleProperty);
		}
		void OnMaximizationUnlock() {
			if(IsMaximized) {
				OnIsMaximizationChangedCore(IsMaximized);
			}
		}
		protected virtual void OnIsMaximizedChanged(bool maximized) {
			DisableSizeToContent();
			if(!MaximizationLockHelper.IsLocked)
				OnIsMaximizationChangedCore(maximized);
			UpdateSingleItemIsMaximized();
		}
		void OnIsMaximizationChangedCore(bool maximized) {
			CoerceValue(IsMaximizeButtonVisibleProperty);
			CoerceValue(IsRestoreButtonVisibleProperty);
			Rect bounds = DocumentPanel.GetRestoreBounds(this);
			Rect restoreBounds = new Rect();
			if(maximized) {
				restoreBounds = bounds == new Rect() ? new Rect(FloatLocation, FloatSize) : bounds;
				bounds = CalcMaximizedBounds(restoreBounds);
			}
			DocumentPanel.SetRestoreBounds(this, restoreBounds);
			FloatLocation = new Point(bounds.X, bounds.Y);
			FloatSize = new Size(bounds.Width, bounds.Height);
		}
		protected internal void UpdateMaximizedBounds() {
			if(!IsMaximized) return;
			Rect bounds = CalcMaximizedBounds(DocumentPanel.GetRestoreBounds(this));
			FloatLocation = new Point(bounds.X, bounds.Y);
			FloatSize = new Size(bounds.Width, bounds.Height);
		}
		protected Rect CalcMaximizedBounds(Rect restoreBounds) {
			Rect bounds = WindowHelper.GetMaximizeBounds(Manager, restoreBounds);
			if(Manager != null && Manager.FlowDirection == FlowDirection.RightToLeft && Manager.GetRealFloatingMode() == Core.FloatingMode.Window)
				bounds.X = -bounds.X - bounds.Width;
			return bounds;
		}
		protected internal void ResetMaximized() {
			if(Items.Count == 1 && Items[0] is DocumentPanel) {
				DocumentPanel.SetRestoreBounds(this, new Rect());
				MDIStateHelper.SetMDIState(Items[0], MDIState.Normal);
				CoerceValue(IsMaximizedProperty);
			}
		}
		protected internal void ResetMaximized(Point location) {
			if(IsMaximized) {
				Rect bounds = DocumentPanel.GetRestoreBounds(this);
				DocumentPanel.SetRestoreBounds(this, new Rect(new Point(location.X - 15, location.Y - 15), MathHelper.IsEmpty(bounds.Size()) ? FloatSize : bounds.Size()));
				if(Items.Count == 1 && Items[0] is LayoutPanel) {
					if(Items[0] is DocumentPanel)
						MDIStateHelper.SetMDIState(Items[0], MDIState.Normal);
				}
				SetIsMaximized(false);
				CoerceValue(IsMaximizedProperty);
				DockLayoutManagerExtension.Update(this);
			}
			else
				ResetMaximized();
		}
		protected override LayoutItemType GetLayoutItemTypeCore() {
			return LayoutItemType.FloatGroup;
		}
		System.Windows.Threading.DispatcherOperation serializableIsMaximizedOperation;
		bool _SerializableIsMaximized;
		[XtraSerializableProperty,
		System.ComponentModel.Browsable(false),
		System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never),
		System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public bool ShouldRestoreOnActivate { get; set; }
		[XtraSerializableProperty(), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool SerializableIsMaximized {
			get { return IsMaximized; }
			set {
				_SerializableIsMaximized = value;
				serializableIsMaximizedOperation.Do(x => x.Abort());
				Action action = new Action(SetSerializableIsMaximized);
				if(value) serializableIsMaximizedOperation = Dispatcher.BeginInvoke(action);
				else action();
			}
		}
		void SetSerializableIsMaximized() {
			DockLayoutManager manager = Manager ?? ManagerReference.Return(x => x.Target, () => null) as DockLayoutManager;
			if(manager != null && !manager.IsDisposing) {
				if(_SerializableIsMaximized)
					manager.MDIController.Maximize(this);
				else manager.MDIController.Restore(this);
			}
		}
		[XtraSerializableProperty(), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int SerializableZOrder {
			get {
				var presenter = UIElements.GetElement<DevExpress.Xpf.Docking.VisualElements.FloatPanePresenter>();
				IView view = Manager.GetView(presenter);
				return view != null ? view.ZOrder : 0;
			}
			set {
				serizalizableZOrder = value;
			}
		}
		internal FloatingWindowLock FloatingWindowLock {
			get {
				var presenter = UIElements.GetElement<DevExpress.Xpf.Docking.VisualElements.FloatingWindowPresenter>();
				return presenter != null && presenter.Window != null ? presenter.Window.LockHelper : null;
			}
		}
		internal bool RequestFloatingBoundsUpdate() {
			var presenter = UIElements.GetElement<DevExpress.Xpf.Docking.VisualElements.FloatingWindowPresenter>();
			bool hasWindow = presenter != null && presenter.Window != null;
			if(hasWindow) presenter.Window.UpdateFloatGroupBounds();
			return hasWindow;
		}
		int serizalizableZOrder;
		internal int GetSerializableZOrder() {
			return serizalizableZOrder;
		}
		protected override void OnHasSingleItemChanged(bool hasSingleItem) {
			CoerceValue(HasNotCollapsedItemsProperty);
			CoerceValue(HasVisibleItemsProperty);
			CoerceValue(ActualVisibilityProperty);
			CoerceValue(IsMaximizedProperty);
			CoerceValue(AllowSizingProperty);
			CoerceValue(IsMaximizeButtonVisibleProperty);
			UpdateSizeToContent();
		}
		protected internal override void AfterItemAdded(int index, BaseLayoutItem item) {
			base.AfterItemAdded(index, item);
			UpdateSingleItemIsMaximized();
		}
		protected internal override void AfterItemRemoved(BaseLayoutItem item) {
			base.AfterItemRemoved(item);
			UpdateSingleItemIsMaximized();
		}
		private void UpdateSingleItemIsMaximized() {
			if(HasSingleItem && Items.Count == 1) Items[0].SetIsMaximized(IsMaximized);
		}
		protected override void OnLayoutChangedCore() {
			base.OnLayoutChangedCore();
			CoerceValue(CanMaximizeProperty);
		}
		protected internal override bool IsMaximizable { get { return true; } }
		protected override bool CoerceIsMaximizeButtonVisible(bool visible) {
			return IsMaximizable && AllowMaximize && !IsMaximized;
		}
		protected override bool CoerceIsRestoreButtonVisible(bool visible) {
			return IsMaximized;
		}
		protected internal override void OnItemVisibilityChanged(BaseLayoutItem item, Visibility visibility) {
			base.OnItemVisibilityChanged(item, visibility);
			CoerceValue(HasNotCollapsedItemsProperty);
			if(HasSingleItem)
				CoerceValue(ActualVisibilityProperty);
		}
		protected override void OnVisibilityChangedOverride(Visibility visibility) {
			base.OnVisibilityChangedOverride(visibility);
			CoerceValue(ActualVisibilityProperty);
		}
		protected virtual Visibility CoerceActualVisibility(Visibility visibility) {
			if(isInDesignTime) return Visibility;
			return HasItems && HasVisibleItems && HasNotCollapsedItems ? Visibility : Visibility.Collapsed;
		}
		protected virtual void OnActualVisibilityChanged(System.Windows.Visibility oldValue, System.Windows.Visibility newValue) {
			SetValue(IsActuallyVisiblePropertyKey, ActualVisibility == System.Windows.Visibility.Visible);
			if(newValue == System.Windows.Visibility.Visible) {
				Manager.Do(x => x.InvalidateView(this));
			}
		}
		protected override bool CoerceIsCloseButtonVisible(bool visible) {
			if(HasSingleItem && Items[0] is LayoutPanel) return Items[0].IsCloseButtonVisible;
			return AllowClose && ShowCloseButton;
		}
		internal void OnOwnerCollectionChanged() {
			OnParentChanged();
#if !SILVERLIGHT
			if(Manager != null) {
				if(Manager.FloatGroups.Contains(this))
					DockLayoutManager.AddLogicalChild(Manager, this);
				else
					DockLayoutManager.RemoveLogicalChild(Manager, this);
			}
#endif
		}
		protected override object CoerceAllowSizing(bool value) {
			return HasSingleItem ? Items[0].AllowSizing : value;
		}
		protected override void OnHasNotCollapsedItemsChanged(bool hasVisibleItems) {
			base.OnHasNotCollapsedItemsChanged(hasVisibleItems);
			CoerceValue(HasVisibleItemsProperty);
			CoerceValue(ActualVisibilityProperty);
		}
#if !SILVERLIGHT
		protected override Size CoerceFloatSize(Size value) {
			if(Manager != null && Manager.GetRealFloatingMode() == Core.FloatingMode.Window) {
				var maxW = SystemParameters.VirtualScreenWidth;
				var maxH = SystemParameters.VirtualScreenHeight;
				value = MathHelper.MeasureSize(new Size(), new Size(maxW, maxH), value);
			}
			return base.CoerceFloatSize(value);
		}
		protected override void OnDockLayoutManagerChanged() {
			base.OnDockLayoutManagerChanged();
			if(Manager != null) {
				DockLayoutManager.AddLogicalChild(Manager, this);
				CoerceValue(FloatSizeProperty);
			}
		}
		internal void EnsureFloatLocation(Point prevOffset, Point newOffset) {
			int direction = FlowDirection == FlowDirection.RightToLeft ? -1 : 1;
			EnsureFloatLocation(new Point(FloatLocation.X - direction * (newOffset.X - prevOffset.X), FloatLocation.Y - (newOffset.Y - prevOffset.Y)));
		}
		internal void EnsureFloatLocation(Point floatLocation) {
			Point diff = new Point(FloatLocation.X - floatLocation.X, FloatLocation.Y - floatLocation.Y);
			if(IsMaximized) {
				Rect restoreBounds = DocumentPanel.GetRestoreBounds(this);
				var restoreLocation = restoreBounds.Location;
				restoreBounds.Location = new Point(restoreLocation.X - diff.X, restoreLocation.Y - diff.Y);
				DocumentPanel.SetRestoreBounds(this, restoreBounds);
			}
			FloatLocation = floatLocation;
		}
#endif
		protected override void OnLoaded() {
		}
		protected virtual void OnClosed() { }
		protected virtual void OnClosing(CancelEventArgs e) { }
		internal override void PrepareForModification(bool isDeserializing) {
			base.PrepareForModification(isDeserializing);
			if(isDeserializing) {
				LockDataContext();
				Manager = null;
				Dispatcher.BeginInvoke(new Action(UnlockDataContext), System.Windows.Threading.DispatcherPriority.Loaded);
			}
		}
		#region IClosable Members
		void IClosable.OnClosed() {
			OnClosed();
		}
		bool IClosable.CanClose() {
			CancelEventArgs e = new CancelEventArgs();
			OnClosing(e);
			return !e.Cancel;
		}
		#endregion
	}
	public class FloatGroupCollection : ObservableCollection<FloatGroup>, IDisposable {
		bool isDisposing;
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDisposing() {
			for(int i = 0; i < Count; i++) {
				Items[i].IsOpen = false;
			}
		}
		public void AddRange(FloatGroup[] items) {
			Array.ForEach(items, Add);
		}
		public BaseLayoutItem this[string name] {
#if !SILVERLIGHT
			get { return Array.Find(this.GetItems(), (item) => item.Name == name); }
#else
			get { return ArrayEx.Find(this.GetItems(), (item) => item.Name == name); }
#endif
		}
		public FloatGroup[] ToArray() {
			FloatGroup[] groups = new FloatGroup[Count];
			Items.CopyTo(groups, 0);
			return groups;
		}
		protected override void InsertItem(int index, FloatGroup item) {
			base.InsertItem(index, item);
			OnItemAdded(item);
		}
		protected override void RemoveItem(int index) {
			FloatGroup item = Items[index];
			base.RemoveItem(index);
			OnItemRemoved(item);
		}
		protected override void SetItem(int index, FloatGroup item) {
			base.SetItem(index, item);
			OnItemAdded(item);
		}
		protected override void ClearItems() {
			FloatGroup[] groups = new FloatGroup[Count];
			Items.CopyTo(groups, 0);
			base.ClearItems();
			for(int i = 0; i < groups.Length; i++)
				OnItemRemoved(groups[i]);
		}
		protected virtual void OnItemAdded(FloatGroup item) {
			if(item != null) {
				item.OnOwnerCollectionChanged();
				item.IsRootGroup = true;
			}
		}
		protected virtual void OnItemRemoved(FloatGroup item) {
			if(item != null) {
				item.BeginLayoutChange();
				item.OnOwnerCollectionChanged();
				item.EndLayoutChange();
				item.IsRootGroup = false;
			}
		}
	}
}
