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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.DragDrop;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Grid {
	class WeakDictionary<TKey, TValue> where TValue : class {
		readonly Dictionary<WeakReference, WeakReference> internalDict = new Dictionary<WeakReference, WeakReference>();
		public TValue this[TKey key] {
			get { 
				WeakReference weakKey = GetWeakKey(key);
				if(weakKey == null)
					return null;
				return this.internalDict[weakKey].Target as TValue; 
			}
			set {
				WeakReference weakKey = GetWeakKey(key) ?? new WeakReference(key);
				this.internalDict[weakKey] = new WeakReference(value);
			}
		}
		private WeakReference GetWeakKey(TKey key) {
			List<WeakReference> keys = this.internalDict.Keys.ToList();
			foreach(var wref in keys) {
				if(!wref.IsAlive) {
					this.internalDict.Remove(wref);
					continue;
				}
				if(wref.Target.Equals(key)) return wref;
			}
			return null;
		}
		public void Remove(TKey key) {
			WeakReference weakKey = GetWeakKey(key);
			if(weakKey == null) return;
			this.internalDict.Remove(weakKey);
		}
		public bool TryGetValue(TKey key, out TValue value) {
			WeakReference weakKey = GetWeakKey(key);
			value = weakKey != null ? this.internalDict[weakKey].Target as TValue : null;
			return weakKey != null;
		}
		public IEnumerable<TValue> Values { get { return this.internalDict.Values.Select(wr => wr.Target as TValue); } }
	}
	static class RootVisualsContainer {
		readonly static WeakDictionary<DragDropManagerBase, UIElement> roots = new WeakDictionary<DragDropManagerBase, UIElement>();
		readonly static List<WeakReference> RegisteredManagers = new List<WeakReference>();
		public static void Register(DragDropManagerBase manager) {
			RegisteredManagers.Add(new WeakReference(manager));
			if(manager.AssociatedControl != null) {
#if SL
				if(manager.AssociatedControl.IsLoaded())
#else
				if(manager.AssociatedControl.IsLoaded)
#endif
					RegisterControl(manager.AssociatedControl);
				manager.AssociatedControl.Loaded -= AssociatedControl_Loaded;
				manager.AssociatedControl.Loaded += AssociatedControl_Loaded;
			}
		}
		static void AssociatedControl_Loaded(object sender, RoutedEventArgs e) {
			RegisterControl(sender);
		}
		static void RegisterControl(object control) {
			WeakReference weakRef = RegisteredManagers.FirstOrDefault<WeakReference>(wr => wr.IsAlive &&
#if !SL
				(wr.Target as DragDropManagerBase).Dispatcher == Dispatcher.CurrentDispatcher && 
#endif
				(wr.Target as DragDropManagerBase).AssociatedControl == control);
			if(weakRef == null) return;
			DragDropManagerBase manager = weakRef.Target as DragDropManagerBase;
			if(manager != null) {
				RegisterManagerRoot(manager);
			}			
		}
		static bool RegisterManagerRoot(DragDropManagerBase manager) {
			FrameworkElement root = manager.GetRootElement();
			if(manager.RootElement == null && root == manager.AssociatedControl)
				return false;
			roots[manager] = root;
			return true;
		}
		public static void Unregister(DragDropManagerBase manager) {
			roots.Remove(manager);
			RegisteredManagers.Remove(RegisteredManagers.FirstOrDefault<WeakReference>(wr => wr.Target == manager));
		}
		public static List<UIElement> GetRootVisuals(DragDropManagerBase manager) {
			UIElement callerRoot;
			RegisterManagerRoot(manager);
			if(manager == null || !roots.TryGetValue(manager, out callerRoot)) return null;
			List<UIElement> allRoots = GetRootVisuals();
			allRoots.Remove(callerRoot);
			allRoots.Insert(0, callerRoot);
			return allRoots;
		}
		public static List<UIElement> GetRootVisuals() {
			return roots.Values.Distinct().ToList();
		}
	}
	public abstract class DragDropManagerBase : Behavior<DependencyObject> {
		readonly static WeakDictionary<UIElement, DragDropManagerBase> managersDictionary = new WeakDictionary<UIElement, DragDropManagerBase>();
		DragLeaveEventHandler DragLeaveEventHandler;
		bool isDragLeaved;
		bool isDropMarkerVisible = true;
#if SL
		bool needDragInfoReopen;
#endif
		void ReopenDragInfoIfNeeded() {
#if SL
			DataControlDragElement dragElement = DragDropHelper.DragElement as DataControlDragElement;
			if(!this.needDragInfoReopen || dragElement == null) return;
			if(!dragElement.FloatingContainer.IsOpen || !adorner.IsOpen) {
				Dispatcher.BeginInvoke(() => ReopenDragInfoIfNeeded());
				return;
			}
			Point loc = DragDropHelper.SourceElement.TranslatePoint(dragElement.FloatingContainer.FloatLocation, null);
			Rect bounds = new Rect(loc, dragElement.FloatingContainer.FloatSize);
			bounds.Intersect(MarkerControl.GetBounds(null));
			if(bounds.IsEmpty) return;
			dragElement.FloatingContainer.IsOpen = false;
			dragElement.FloatingContainer.IsOpen = true;
			this.needDragInfoReopen = false;
#endif
		}
		[Category("Events")]
		public event DragLeaveEventHandler DragLeave {
			add { DragLeaveEventHandler += value; }
			remove { DragLeaveEventHandler -= value; }
		}
		[Browsable(false)]
		public void SetDropMarkerVisibility(bool visible) {
			IsDropMarkerVisible = visible;
			if(!visible)
				HideDropMarker();
		}
		protected static void SetDragDropManager(UIElement element, DragDropManagerBase value) {
			managersDictionary[element] = value;
		}
		protected static DragDropManagerBase GetDragDropManager(UIElement element) {
#if !SL
			DragDropManagerBase manager = managersDictionary[element];
			if(manager != null && manager.Dispatcher == Dispatcher.CurrentDispatcher) {
				return manager;
			}
			return null;
#else
			return managersDictionary[element];
#endif
		}
		public bool AllowDrop {
			get { return (bool)GetValue(AllowDropProperty); }
			set { SetValue(AllowDropProperty, value); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			RootVisualsContainer.Register(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Control AssociatedControl { get { return AssociatedObject as Control; } }
		protected override void OnDetaching() {
			base.OnDetaching();
			RootVisualsContainer.Unregister(this);
			managersDictionary.Remove(AssociatedControl);
		}
		internal FrameworkElement GetRootElement() {
			return RootElement ?? LayoutHelper.FindRoot(AssociatedObject) as FrameworkElement;
		}
		protected bool IsDropMarkerVisible { get { return this.isDropMarkerVisible; } set { this.isDropMarkerVisible = value; } }
		protected bool IsDragLeaved { get { return this.isDragLeaved; } }
		public static readonly DependencyProperty AllowDropProperty;
		public static readonly DependencyProperty TemplatesContainerProperty;
		public static readonly DependencyProperty RootElementProperty;
		public static readonly DependencyProperty DragElementTemplateProperty;
		public static readonly DependencyProperty AllowDragProperty;
		static DragDropManagerBase() {
			Type ownerType = typeof(DragDropManagerBase);
			AllowDropProperty =
				DependencyPropertyManager.Register("AllowDrop", typeof(bool), ownerType, new PropertyMetadata(true));
			TemplatesContainerProperty =
				DependencyPropertyManager.Register("TemplatesContainer", typeof(DragDropTemplatesContainer), ownerType, new PropertyMetadata(null));
			RootElementProperty =
				DependencyPropertyManager.Register("RootElement", typeof(FrameworkElement), ownerType, new PropertyMetadata(null));
			DragElementTemplateProperty =
				DependencyPropertyManager.Register("DragElementTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			AllowDragProperty =
				DependencyPropertyManager.Register("AllowDrag", typeof(bool), ownerType, new UIPropertyMetadata(true));
		}
		public bool AllowDrag {
			get { return (bool)GetValue(AllowDragProperty); }
			set { SetValue(AllowDragProperty, value); }
		}
		public DragDropTemplatesContainer TemplatesContainer {
			get { return (DragDropTemplatesContainer)GetValue(TemplatesContainerProperty); }
			set { SetValue(TemplatesContainerProperty, value); }
		}
		protected internal DragDropElementHelper DragDropHelper { get; set; }
		protected internal bool IsDragging { get { return DragDropHelper.IsDragging; } }
		#region inner classes
		public class DragDropManagerDropTargetFactory : IDropTargetFactory {
			IDropTarget IDropTargetFactory.CreateDropTarget(UIElement dropTargetElement) {
				return new DragDropManagerDropTarget(DragDropManagerBase.GetDragDropManagerBySourceElement(dropTargetElement));
			}
		}
		public class DragDropManagerDropTarget : IDropTarget {
			DragDropManagerBase dragDropManager;
			public DragDropManagerDropTarget(DragDropManagerBase dragDropManager) {
				this.dragDropManager = dragDropManager;
			}
			void IDropTarget.Drop(UIElement source, Point pt) {
				DragDropManagerBase sourceManager = GridDragDropManager.GetDragDropManagerBySourceElement(source);
				if(!dragDropManager.AllowDrop) {
					return;
				}
				if(AssociatedObjectIsVisible)
					dragDropManager.OnDrop(sourceManager, source, pt);
			}
			void IDropTarget.OnDragLeave() {
				dragDropManager.OnDragLeave();
			}
			void IDropTarget.OnDragOver(UIElement source, Point pt) {
				DragDropManagerBase sourceManager = GridDragDropManager.GetDragDropManagerBySourceElement(source);
				if(AssociatedObjectIsVisible)
					dragDropManager.OnDragOver(sourceManager, source, pt);
				if(!dragDropManager.AllowDrop) {
					sourceManager.SetDropTargetType(DropTargetType.None);
					return;
				}
			}
			bool AssociatedObjectIsVisible {
				get {
					FrameworkElement element = dragDropManager.AssociatedObject as FrameworkElement;
					return element != null && element.ActualWidth != 0d && element.ActualHeight != 0d
#if SL
						&& element.IsVisible();
#else
						&& element.IsVisible;
#endif
				}
			}	
			public int Index { get; set; }
		}
		public abstract class SupportDragDropBase : DragDropObjectBase, ISupportDragDrop {
			protected SupportDragDropBase(DragDropManagerBase dragDropManager)
				: base(dragDropManager) {
			}
			#region ISupportDragDrop Members
			bool ISupportDragDrop.CanStartDrag(object sender, MouseButtonEventArgs e) {
				return dragDropManager.AllowDrag && dragDropManager.CanStartDrag(e);
			}
			protected abstract FrameworkElement Owner { get; }
			IDragElement ISupportDragDrop.CreateDragElement(Point offset) {
				IList dragRows = dragDropManager.DraggingRows;
				dragDropManager.ViewInfo.DraggingRows = dragRows;
				if(dragRows != null && dragRows.Count > 0)
					dragDropManager.ViewInfo.FirstDraggingObject = dragDropManager.GetObject(dragRows[0]);
				else
					dragDropManager.ViewInfo.FirstDraggingObject = null;
				DestroyPreviousDragElement();
				return dragDropManager.CreateDragElement(offset, Owner);
			}
			IDropTarget ISupportDragDrop.CreateEmptyDropTarget() { return null; }
			IEnumerable<UIElement> ISupportDragDrop.GetTopLevelDropContainers() {
				return RootVisualsContainer.GetRootVisuals(dragDropManager);
			}
			bool ISupportDragDrop.IsCompatibleDropTargetFactory(IDropTargetFactory factory, UIElement dropTargetElement) {
				return factory is DragDropManagerDropTargetFactory;
			}
			FrameworkElement ISupportDragDrop.SourceElement { get { return SourceElementCore; } }
			protected abstract FrameworkElement SourceElementCore { get; }
			#endregion
			void DestroyPreviousDragElement() {
				DragDropElementHelper helper = dragDropManager.DragDropHelper;
				if(helper.DragElement != null)
					helper.DragElement.Destroy();
			}
		}
		#endregion
		internal static DragDropManagerBase GetDragDropManagerBySourceElement(UIElement source) {
			return DragDropManagerBase.GetDragDropManager(source);
		}
		public DataTemplate DragElementTemplate {
			get { return (DataTemplate)GetValue(DragElementTemplateProperty); }
			set { SetValue(DragElementTemplateProperty, value); }
		}
		public FrameworkElement RootElement {
			get { return (FrameworkElement)GetValue(RootElementProperty); }
			set { SetValue(RootElementProperty, value); }
		}
		public DragDropViewInfo ViewInfo { get; protected set; }
		protected internal abstract IList ItemsSource { get; }
		protected DragDropManagerBase dragOverSourceManager;
		public IList DraggingRows { get; protected internal set; }
#if SL
		FloatingContainer adorner;
		protected FloatingContainer Adorner { get { return this.adorner; } }
#else
		AdornerContainer adorner;
		AdornerLayer adornerLayer;
#endif
		protected TableDropMarkerControl MarkerControl { get; set; }
		protected DragDropManagerBase() {
			ViewInfo = new DragDropViewInfo();
#if SL
			adorner = FloatingContainerFactory.Create(FloatingMode.Adorner);
			adorner.ShowContentOnly = true;
#endif
			this.TemplatesContainer = new DragDropTemplatesContainer();
		}
		protected virtual bool CanShowDropMarker() {
			return true;
		}
		protected virtual FrameworkElement CreateLogicalOwner() {
			return null;
		}
		protected virtual bool NeedAdornerLogicalOwner { get { return true; } }
		internal void ShowDropMarker(UIElement element, TableDragIndicatorPosition dragIndicatorPosition, double leftIndent) {
			ShowDropMarker(element, dragIndicatorPosition);
			if(MarkerControl != null)
				MarkerControl.Margin = new Thickness(leftIndent, 0, 0, 0);
		}
		public virtual void ShowDropMarker(UIElement element, TableDragIndicatorPosition dragIndicatorPosition) {
			if(!IsDropMarkerVisible || !CanShowDropMarker()) return;
#if !SL
			HideDropMarker();
#endif
			if(MarkerControl == null)
				MarkerControl = CreateDropMarkerControl();
#if SL
			if(adorner.Owner != element && element != null) {
				FrameworkElement fe = element as FrameworkElement;
				MarkerControl.Height = fe.ActualHeight;
				MarkerControl.Width = fe.ActualWidth;
				adorner.Content = MarkerControl;
				adorner.Owner = fe;
				if(NeedAdornerLogicalOwner)
					adorner.LogicalOwner = CreateLogicalOwner() ?? fe;
				adorner.UpdateFloatingBounds();
				adorner.IsOpen = true;
			}
#else
			if((adorner == null || adorner.AdornedElement != element) && element != null) {
				adorner = new AdornerContainer(element, MarkerControl);
				adornerLayer = AdornerLayer.GetAdornerLayer(element);
				adornerLayer.Add(adorner);
			}
#endif
			MarkerControl.DragIndicatorPosition = dragIndicatorPosition;
		}
		protected virtual TableDropMarkerControl CreateDropMarkerControl() {
			return new TableDropMarkerControl() { IsHitTestVisible = false };
		}
		protected virtual void HideDropMarker() {
#if !SL
			if(adorner != null) {
				adornerLayer.Remove(adorner);
				adorner = null;
				adornerLayer = null;
				MarkerControl = null;
			}
#endif
		}
		protected internal virtual void ClearDragInfo(DragDropManagerBase sourceManager) {
			sourceManager.HideDropMarkerForce();
			sourceManager.SetDropTargetType(DropTargetType.None);
		}
		protected internal virtual void OnDrop(DragDropManagerBase sourceManager, UIElement source, Point pt) { }
		protected internal virtual void OnDragLeave() {
			this.isDragLeaved = true;
			if(dragOverSourceManager != null)
				ClearDragInfo(dragOverSourceManager);
			if(!ReferenceEquals(this, dragOverSourceManager))
				ClearDragInfo(this);
#if SL
			if(dragOverSourceManager != null)
				dragOverSourceManager.needDragInfoReopen = true;
#endif
			RaiseDragLeaveEvent();
		}
		protected virtual void RaiseDragLeaveEvent() {
			if(DragLeaveEventHandler != null)
				DragLeaveEventHandler(this, new DragLeaveEventArgs(this, dragOverSourceManager));
		}
		[Browsable(false)]
		public virtual void OnDragOver(DragDropManagerBase sourceManager, UIElement source, Point pt) {
			this.isDragLeaved = false;
			this.dragOverSourceManager = sourceManager;
			sourceManager.ReopenDragInfoIfNeeded();
		}
		void HideDropMarkerForce() {
#if SL
			adorner.Content = null;
			adorner.Owner = null;
			adorner.IsOpen = false;
#else
			HideDropMarker();
#endif
		}
		public void SetDropTargetType(DropTargetType dropTargetType) {
			ViewInfo.DropTargetType = dropTargetType;
		}
		protected virtual IDragElement CreateDragElement(Point offset, FrameworkElement owner) {
			return new DataControlDragElement(this, offset, owner);
		}
		public virtual object GetObject(object obj) {
			return obj;
		}
		public virtual IList GetSource(object row) {
			return ItemsSource;
		}
		public virtual void RemoveObject(object obj){
			object o = GetObject(obj);
			GetSource(obj).If(p => p.Contains(o)).Do(p => p.Remove(o));
		}
		protected internal virtual bool CustomAllowDrag(IndependentMouseEventArgs e) {
			return false;
		}
		public virtual void SetDragInfoVisibility(bool visible) {
			DataControlDragElement dragElement = DragDropHelper.DragElement as DataControlDragElement;
			if(dragElement != null) {
				dragElement.ShouldOpenContainer = visible;
				dragElement.FloatingContainer.IsOpen = visible;
			}
		}
		protected internal abstract StartDragEventArgs RaiseStartDragEvent(IndependentMouseEventArgs e);
		protected internal abstract IList CalcDraggingRows(IndependentMouseEventArgs e);
		protected internal abstract bool CanStartDrag(MouseButtonEventArgs e);
		protected bool DropEventIsLocked { get; set; }
	}
	public abstract class DragDropObjectBase {
		public DragDropManagerBase DragDropManagerBase { get { return dragDropManager; } }
		protected readonly DragDropManagerBase dragDropManager;
		protected DragDropObjectBase(DragDropManagerBase dragDropManager) {
			this.dragDropManager = dragDropManager;
		}
	}
	public class RowDragDropElementHelper : DragDropElementHelperBounded {
		public RowDragDropElementHelper(ISupportDragDrop supportDragDrop, bool isRelativeMode = true)
			: base(supportDragDrop, isRelativeMode) { }
		protected override BaseDragDropStrategy CreateDragDropStrategy() {
			return new RowDragDropElementHelperStrategy(SupportDragDrop, this);
		}
		protected override Point CorrectDragElementLocation(Point newPos) {
			return PointHelper.Subtract(base.CorrectDragElementLocation(newPos), MouseDownPositionCorrection);
		}
		DragDropManagerBase DragDropManager {
			get {
				return ((DragDropObjectBase)SupportDragDrop).DragDropManagerBase as DragDropManagerBase;
			}
		}
		protected override void StartDragging(Point offset, IndependentMouseEventArgs e) {
			DragDropManagerBase DDManager = DragDropManager;
			if(DDManager != null && DDManager.CustomAllowDrag(e) && DDManager.DraggingRows != null && DDManager.DraggingRows.Count > 0 && DDManager.AllowDrag)
				base.StartDragging(offset, e);
		}
		protected override void EndDragging(IndependentMouseButtonEventArgs e) {
			base.EndDragging(e);
			DragDropManagerBase DDManager = DragDropManager;
			if(DDManager != null) {
				DDManager.DraggingRows = null;
				DDManager.ViewInfo.DraggingRows = null;
				DDManager.ViewInfo.FirstDraggingObject = null;
			}
		}
		protected override Point GetStartDraggingOffset(IndependentMouseEventArgs e, FrameworkElement sourceElement) {
			return GetPosition(e, SupportDragDrop.SourceElement);
		}
	}
	public class RowDragDropElementHelperStrategy : BaseDragDropStrategy {
		public RowDragDropElementHelperStrategy(ISupportDragDrop supportDragDrop, DragDropElementHelper helper)
			: base(supportDragDrop, helper) { }
		public override FrameworkElement GetTopVisual(FrameworkElement node) {
			return (FrameworkElement)LayoutHelper.GetTopLevelVisual(node);
		}
		public override BaseLocationStrategy CreateLocationStrategy() {
#if SL
			if(Helper.SourceElement.FlowDirection == FlowDirection.RightToLeft || TopVisual.FlowDirection == FlowDirection.RightToLeft) {
				if(TopVisual is Popup)
					return new RTL_LocationStrategy();
				return new RTL_MouseLocationStrategy();
			}
#endif
			return base.CreateLocationStrategy();
		}
	}
}
