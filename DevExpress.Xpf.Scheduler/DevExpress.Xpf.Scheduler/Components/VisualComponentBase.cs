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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Core;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Xpf.Scheduler.Internal;
using System.Diagnostics;
using DevExpress.XtraScheduler.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using System.Collections.Specialized;
using ScrollViewer = System.Windows.Controls.ScrollViewer;
#else
using System.Collections.Specialized;
using System.Windows.Input;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Utils;
using ScrollViewer = DevExpress.Xpf.Scheduler.Drawing.SchedulerScrollViewer;
#endif
using DevExpress.Utils;
using System.Collections.ObjectModel;
using System.CodeDom.Compiler;
using System.ComponentModel;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region IVisualElement interface
	public interface IVisualElement {
		Size DesiredSize { get; }
		Size RenderSize { get; }
		Visibility Visibility { get; set; }
		void Measure(Size availableSize);
		void Arrange(Rect arrangeBounds);
		void InvalidateMeasure();
		void InvalidateArrange();
	}
	#endregion
	#region IUIElementCollectionChangedListener interface
	public interface IUIElementCollectionChangedListener {
		void OnElementInserted(object sender, int index);
		void OnElementRemoved(object sender, int index);
	}
	#endregion
	#region VisualComponentType
	[Flags]
	public enum VisualComponentType {
		Unknown = 0,
		DateHeader = 0x1,
		Resources = 0x2,
		TopLeftCorner = 0x4,
		SelectionBar = 0x8,
		ResourceNavigator = 0x10,
		Selection = 0x20,
		ResourceHeader = 0x40,
		Cells = 0x80,
		Appointments = 0x100
	}
	#endregion
	#region IVisualComponent interface
	public interface IVisualComponent : IVisualElement, IUIElementCollectionChangedListener, IEnumerable<IVisualElement>, IDisposable {
		VirtualizationMode VirtualizationMode { get; set; }
		void Initialize();
		int ZIndex { get; }
		int FirstChildIndex { get; }
		List<IVisualComponent> GetComponents();
		void OnChildrenChanged(IVisualElement child);
	}
	#endregion
}
namespace DevExpress.Xpf.Scheduler.Drawing.Components {
	#region UIElementCollectionChangedNotifier
	public static class UIElementCollectionChangedNotifier {
		public static void OnElementInserted(IUIElementCollectionChangedListener listener, object sender, int index) {
			if (listener != null)
				listener.OnElementInserted(sender, index);
		}
		public static void OnElementRemoved(IUIElementCollectionChangedListener listener, object sender, int index) {
			if (listener != null)
				listener.OnElementRemoved(sender, index);
		}
	}
	#endregion
	public interface ICellsComponentChangedListener {
		void OnCellsChanged();
	}
	public class VisualElementEnumerator : IEnumerator<IVisualElement> {
		readonly IVisualElementAccessor<IVisualElement> accessor;
		int currentIndex;
		public VisualElementEnumerator(IVisualElementAccessor<IVisualElement> accessor) {
			this.accessor = accessor;
			Reset();
		}
		public IVisualElement Current { get { return accessor[currentIndex]; } }
		object System.Collections.IEnumerator.Current { get { return Current; } }
		public bool MoveNext() {
			this.currentIndex++;
			if (this.currentIndex >= accessor.Count)
				return false;
			return true;
		}
		public void Reset() {
			this.currentIndex = -1;
		}
		public void Dispose() {
		}
	}
	#region IPanelOwner (interface)
	public interface IPanelOwner {
		ILayoutPanel Panel { get; }
	}
	#endregion
	public interface IChildAccessor<T> where T : IVisualElement {
		int Count { get; }
		T this[int index] { get; }
	}
	#region IVisualElementAccessor interface
	public interface IVisualElementAccessor<T> : IPanelOwner, IChildAccessor<T>, IUIElementCollectionChangedListener where T : IVisualElement {
		void Add(T newItem);
		void RemoveAt(int index);
		void ForEach(Action<T> action);
		void Clear();
	}
	#endregion
	#region PanelChildrenAccessor
	public class PanelChildrenAccessor<T> : IVisualElementAccessor<T>, IDisposable, ISupportCheckIntegrity where T : UIElement, IVisualElement {
		readonly ILayoutPanel panel;
		int zIndex;
		public PanelChildrenAccessor(ILayoutPanel panel, int zIndex) {
			this.panel = panel;
			Offset = Children.Count;
			this.zIndex = zIndex;
		}
		#region Properties
		ILayoutPanel IPanelOwner.Panel { get { return panel; } }
		public int Count { get { return Length; } }
		protected UIElementCollection Children { get { return panel.Children; } }
		public T this[int index] { get { return (T)Children[GetActualIndex(index)]; } }
		public T First { get { return (Length > 0) ? this[0] : null; } }
		public T Last { get { return (Length > 0) ? this[Length - 1] : null; } }
		internal int Offset { get; private set; }
		internal int Length { get; private set; }
		protected int InputPosition { get { return GetInputPosition(); } }
		#endregion
		protected int GetActualIndex(int nextIndex) {
			return nextIndex + Offset;
		}
		int GetInputPosition() {
			if (Length == 0) {
				Offset = this.panel.CalculateFirstChildIndexFromZIndex(this.zIndex);
			}
			return Length + Offset;
		}
		public virtual void Add(T item) {
			int index = InputPosition;
			Children.Insert(index, item);
			Length++;
			UIElementCollectionChangedNotifier.OnElementInserted(panel, this, index);
			SchedulerAssert.CheckIntegrity(this.panel);
		}		
		public virtual void RemoveAt(int index) {
			int actualIndex = GetActualIndex(index);
			Children.RemoveAt(actualIndex);
			Length--;
			UIElementCollectionChangedNotifier.OnElementRemoved(panel, this, actualIndex);
			SchedulerAssert.CheckIntegrity(this.panel);
		}
		public virtual void Clear() {
			for (int i = Count - 1; i >= 0; i--)
				RemoveAt(i);
		}
		public virtual void ForEach(Action<T> action) {
			for (int i = 0; i < Count; i++)
				action(this[i]);
		}
		protected virtual void OnElementInserted(int index) {
			if (Length == 0)
				Offset++;
			else if (index >= InputPosition) 
				return;
			else if (index <= Offset)
				Offset++;
			else
				Length++;
		}
		protected virtual void OnElementRemoved(int index) {
			if (Length == 0)
				Offset--;
			else if (index >= InputPosition)
				return;
			else if (index < Offset)
				Offset--;
			else
				Length--;
		}
		#region IUIElementCollectionChangeListener Members
		void IUIElementCollectionChangedListener.OnElementInserted(object sender, int index) {
			if (!Object.ReferenceEquals(this, sender))
				OnElementInserted(index);
		}
		void IUIElementCollectionChangedListener.OnElementRemoved(object sender, int index) {
			if (!Object.ReferenceEquals(this, sender))
				OnElementRemoved(index);
		}
		#endregion
		#region IDisposable Members
		public virtual void Dispose() {
			Clear();
		}
		#endregion
		#region debug
		void ISupportCheckIntegrity.CheckIntegrity() {
			for (int i = 0; i < Length; i++) {
				T element = Children[GetActualIndex(i)] as T;
				System.Diagnostics.Debug.Assert(element != null);
			}
		} 
		#endregion
	}
	#endregion
	#region VisualComponentAccessor
	public class VisualComponentAccessor<T> : IVisualElementAccessor<T>, ISupportCheckIntegrity where T : IVisualComponent {
		#region Fields
		readonly List<T> innerList = new List<T>();
		ILayoutPanel panel;
		#endregion
		public VisualComponentAccessor(ILayoutPanel panel) {
			this.panel = panel;
		}
		#region Properties
		ILayoutPanel IPanelOwner.Panel { get { return panel; } }
		public int Count { get { return InnerList.Count; } }
		public T this[int index] { get { return InnerList[index]; } }
		protected internal List<T> InnerList { get { return innerList; } }
		#endregion
		public virtual void Add(T newItem) {
			InnerList.Add(newItem);
		}
		public virtual void RemoveAt(int index) {
			InnerList[index].Dispose();
			InnerList.RemoveAt(index);
		}
		public virtual void Clear() {
			for (int i = InnerList.Count - 1; i >= 0; i--)
				RemoveAt(i);
		}
		public virtual void ForEach(Action<T> action) {
			InnerList.ForEach(action);
		}
		#region IUIElementCollectionChangedListener
		void IUIElementCollectionChangedListener.OnElementInserted(object sender, int index) {
			for (int i = 0; i < Count; i++)
				UIElementCollectionChangedNotifier.OnElementInserted(InnerList[i], sender, index);
		}
		void IUIElementCollectionChangedListener.OnElementRemoved(object sender, int index) {
			for (int i = 0; i < Count; i++)
				UIElementCollectionChangedNotifier.OnElementRemoved(InnerList[i], sender, index);
		}
		#endregion
		void ISupportCheckIntegrity.CheckIntegrity() {
			SchedulerAssert.CheckIntegrity(InnerList);
		}
	}
	#endregion
	public interface IVisualElementGeneratorCore<TVisualItem, TContext>
		where TVisualItem : IVisualElement
		where TContext : class {
		TVisualItem GenerateNext(TContext context);
		void MoveBack();
	}
	#region IVisualElementGenerator (interface)
	public interface IVisualElementGenerator<TVisualItem, TContext> : IVisualElementGeneratorCore<TVisualItem, TContext>
		where TVisualItem : IVisualElement
		where TContext : class {
		void Release(bool removeUnusedItems);
		void Reset();
	}
	#endregion
	#region VisualElementGeneratorBase (abstract class)
	public abstract class VisualElementGeneratorBase<TVisualItem, TContext> : IVisualElementGenerator<TVisualItem, TContext>
		where TVisualItem : IVisualElement
		where TContext : class {
		protected VisualElementGeneratorBase(IVisualComponent owner, IVisualElementAccessor<TVisualItem> elementAccessor) {
			Owner = owner;
			ElementAccessor = elementAccessor;
			Reset();
		}
		public IVisualElementAccessor<TVisualItem> ElementAccessor { get; private set; }
		public IVisualComponent Owner { get; set; }
		protected internal int CurrentIndex { get; protected set; }
		public virtual TVisualItem GenerateNext(TContext content) {
			CurrentIndex++;
			TVisualItem result = (CurrentIndex < ElementAccessor.Count) ? GetExistingItem(content) : CreateNewItem(content);
			PrepareItemOverride(result, content);
			SchedulerAssert.CheckIntegrity(ElementAccessor);
			return result;
		}
		protected virtual void UpdateVisibility(TVisualItem item) {
			if (item.Visibility != Visibility.Visible)
				item.Visibility = Visibility.Visible;
		}
		protected internal virtual TVisualItem CreateNewItem(TContext context) {
			TVisualItem control = CreateNewItemOverride(context);
			ElementAccessor.Add(control);
			InitializeNewItem(control);
			SchedulerAssert.CheckIntegrity(ElementAccessor);
			return control;
		}
		protected virtual void InitializeNewItem(TVisualItem item) {
		}
		protected internal virtual TVisualItem GetExistingItem(TContext context) {
			TVisualItem item = ElementAccessor[CurrentIndex];
			UpdateVisibility(item);
			return item;
		}
		public virtual void Reset() {
			CurrentIndex = -1;
		}
		public virtual void MoveBack() {
			if (CurrentIndex >= 0)
				CurrentIndex--;
		}
		public virtual void Release(bool removeUnusedItems) {
			for (int i = ElementAccessor.Count - 1; i > CurrentIndex; i--) {
				if (removeUnusedItems)
					ElementAccessor.RemoveAt(i);
				else
					ProcessUnusedItem(i);
			}
		}
		protected virtual void ProcessUnusedItem(int index) {
			TVisualItem item = ElementAccessor[index];
			if (item.Visibility != Visibility.Collapsed)
				item.Visibility = Visibility.Collapsed;
		}
		protected abstract void PrepareItemOverride(TVisualItem item, TContext context);
		protected internal abstract TVisualItem CreateNewItemOverride(TContext context);
	}
	#endregion
	#region ControlGenerator (abstract class)
	public abstract class ControlGenerator<TVisualItem, TContext> : VisualElementGeneratorBase<TVisualItem, TContext>
		where TVisualItem : Control, IVisualElement
		where TContext : class {
		protected ControlGenerator(IVisualComponent owner, IVisualElementAccessor<TVisualItem> elementAccessor, Style style)
			: base(owner, elementAccessor) {
			Style = style;
		}
		public Style Style { get; private set; }
		protected virtual int ZIndex { get { return 0; } }
		protected override void PrepareItemOverride(TVisualItem item, TContext context) {
			if (item.DataContext == null || !Object.ReferenceEquals(context, item.DataContext))
				item.DataContext = context;
			if (item.Style == null || !Object.ReferenceEquals(item.Style, Style))
				item.Style = Style;
			Canvas.SetZIndex(item, ZIndex);
		}
		protected override void ProcessUnusedItem(int index) {
			base.ProcessUnusedItem(index);
			TVisualItem control = ElementAccessor[index];
			control.ClearValue(FrameworkElement.DataContextProperty);
		}
	}
	#endregion
	#region ContentControlGenerator
	public class ContentControlGenerator<TVisualItem, TContext> : ControlGenerator<TVisualItem, TContext>
		where TVisualItem : SchedulerContentControl, new()
		where TContext : class {
		public ContentControlGenerator(IVisualComponent owner, IVisualElementAccessor<TVisualItem> elementAccessor, Style style)
			: base(owner, elementAccessor, style) {
		}
		protected internal override TVisualItem CreateNewItemOverride(TContext context) {
			return new TVisualItem();
		}
		protected override void PrepareItemOverride(TVisualItem item, TContext context) {
			base.PrepareItemOverride(item, context);
			object content = GetContainerContent(context);
			if (item.Content == null || !Object.ReferenceEquals(content, item.Content))
				item.Content = content;
			item.Owner = Owner;
		}
		protected virtual object GetContainerContent(TContext context) {
			return context;
		}
		protected override void ProcessUnusedItem(int index) {
			base.ProcessUnusedItem(index);
			TVisualItem control = ElementAccessor[index];
			control.ClearValue(ContentControl.ContentProperty);
		}
	}
	#endregion
	#region ResourceHeaderGenerator
	public class ResourceHeaderGenerator : ContentControlGenerator<VisualResourceHeader, VisualResource> {
		public ResourceHeaderGenerator(IVisualComponent owner, IVisualElementAccessor<VisualResourceHeader> elementAccessor, Style elementStyle)
			: base(owner, elementAccessor, elementStyle) {
		}
		protected override int ZIndex { get { return 2; } }
		protected override object GetContainerContent(VisualResource context) {
			return context.ResourceHeader;
		}
	}
	#endregion
	#region VisualTimeScaleHeaderGenerator
	public class VisualTimeScaleHeaderGenerator : ContentControlGenerator<VisualTimeScaleHeader, VisualTimeScaleHeaderContent> {
		public VisualTimeScaleHeaderGenerator(IVisualComponent owner, IVisualElementAccessor<VisualTimeScaleHeader> elementAccessor, Style style)
			: base(owner, elementAccessor, style) {
		}
		protected override int ZIndex { get { return 2; } }
	}
	#endregion
	#region VisualTimeCellGenerator
	public class VisualTimeCellGenerator : ContentControlGenerator<VisualTimeCell, VisualTimeCellBaseContent> {
		public VisualTimeCellGenerator(IVisualComponent owner, IVisualElementAccessor<VisualTimeCell> elementAccessor, Style style)
			: base(owner, elementAccessor, style) {
		}
	}
	#endregion
	#region VisualSingleTimelineCellGenerator
	public class VisualSingleTimelineCellGenerator : ContentControlGenerator<VisualSingleTimelineCell, VisualTimeCellBaseContent> {
		public VisualSingleTimelineCellGenerator(IVisualComponent owner, IVisualElementAccessor<VisualSingleTimelineCell> elementAccessor, Style style)
			: base(owner, elementAccessor, style) {
		}
	}
	#endregion
	#region VisualComponentGenerator (abstract class)
	public abstract class VisualComponentGenerator<TVisualItem, TContext> : VisualElementGeneratorBase<TVisualItem, TContext>
		where TVisualItem : IVisualComponent, IDisposable
		where TContext : class {
		protected VisualComponentGenerator(IVisualComponent owner, IVisualElementAccessor<TVisualItem> elementAccessor)
			: base(owner, elementAccessor) {
		}
		protected override void PrepareItemOverride(TVisualItem item, TContext context) {
			item.VirtualizationMode = Owner.VirtualizationMode;
			AssignProperties(item, context);
		}
		protected virtual void AssignProperties(TVisualItem item, TContext context) {
		}
		protected override void InitializeNewItem(TVisualItem item) {
			base.InitializeNewItem(item);
			item.Initialize();
		}
	}
	#endregion
	#region ResourceComponentGenerator
	public class ResourceComponentGenerator : VisualComponentGenerator<ResourceComponent, VisualResource> {
		public ResourceComponentGenerator(IVisualComponent owner, IVisualElementAccessor<ResourceComponent> elementAccessor)
			: base(owner, elementAccessor) {
		}
		public Thickness CellsPadding { get; set; }
		protected override void AssignProperties(ResourceComponent item, VisualResource context) {
			base.AssignProperties(item, context);
			item.Resource = context;
			item.ResourceIntervals = context.SimpleIntervals;
			item.CellsPadding = CellsPadding;
		}
		protected internal override ResourceComponent CreateNewItemOverride(VisualResource context) {
			return new ResourceComponent(ElementAccessor.Panel, Owner);
		}
	}
	#endregion
	#region DateHeaderLevelGenerator
	public class DateHeaderLevelGenerator : VisualComponentGenerator<DateHeaderLevelComponent, VisualTimeScaleHeaderLevel> {
		public DateHeaderLevelGenerator(IVisualComponent owner, IVisualElementAccessor<DateHeaderLevelComponent> elementAccessor)
			: base(owner, elementAccessor) {
		}
		protected override void AssignProperties(DateHeaderLevelComponent item, VisualTimeScaleHeaderLevel context) {
			base.AssignProperties(item, context);
			item.Orientation = Orientation.Horizontal;
			item.ViewModelItems = context.Headers;
		}
		protected internal override DateHeaderLevelComponent CreateNewItemOverride(VisualTimeScaleHeaderLevel context) {
			return new DateHeaderLevelComponent(ElementAccessor.Panel, Owner);
		}
	}
	#endregion
	#region CellContainerGenerator
	public class CellContainerGenerator : VisualComponentGenerator<CellContainerVisualComponent, VisualSimpleResourceInterval> {
		public CellContainerGenerator(IVisualComponent owner, IVisualElementAccessor<CellContainerVisualComponent> elementAccessor)
			: base(owner, elementAccessor) {
		}
		public Thickness CellsPadding { get; set; }
		protected override void AssignProperties(CellContainerVisualComponent item, VisualSimpleResourceInterval context) {
			base.AssignProperties(item, context);
			VisualTimeline visualTimeline = (VisualTimeline)context;
			VisualHorizontalCellContainer cellContainer = visualTimeline.CellContainer;
			item.VisualTimeline = visualTimeline;
			item.Cells = cellContainer.Cells;
			item.Appointments = cellContainer.AppointmentControlCollection;
			item.DraggedAppointments = cellContainer.DraggedAppointmentControlCollection;
			item.CellsPadding = CellsPadding;
		}
		protected internal override CellContainerVisualComponent CreateNewItemOverride(VisualSimpleResourceInterval context) {
			return new CellContainerVisualComponent(ElementAccessor.Panel, Owner);
		}
	}
	#endregion
	#region MoreButtonGenerator
	public class MoreButtonGenerator : ControlGenerator<MoreButton, MoreButtonViewModel> {
		public MoreButtonGenerator(IVisualComponent owner, IVisualElementAccessor<MoreButton> elementAccessor, Style style)
			: base(owner, elementAccessor, style) {
		}
		protected override void PrepareItemOverride(MoreButton item, MoreButtonViewModel context) {
			base.PrepareItemOverride(item, context);
			if (item.ViewModel == null || !Object.ReferenceEquals(item.ViewModel, context))
				item.ViewModel = context;
			item.ApplyTemplate();
			item.Owner = Owner;
		}
		protected override void UpdateVisibility(MoreButton item) {
		}
		protected override void ProcessUnusedItem(int index) {
		}
		protected internal override MoreButton CreateNewItemOverride(MoreButtonViewModel context) {
			return new MoreButton();
		}
	}
	#endregion
	#region VisualComponent<T> (abstract class)
	public abstract class VisualComponent<T> : DependencyObject, IVisualComponent, IUIElementCollectionChangedListener, IDisposable, ISupportCheckIntegrity
		where T : IVisualElement {
		#region Fields
		Visibility visibility;
		VirtualizationMode virtualizationMode;
		long measureVersion;
		#endregion
		protected VisualComponent(ILayoutPanel panel, IVisualComponent owner) {
			Panel = panel;
			Owner = owner;
			InvalidateState();
		}
		#region Properties
		public Size DesiredSize { get; private set; }
		public Size RenderSize { get; private set; }
		public Size AvailableSize { get; private set; }
		public virtual int ZIndex { get { return 0; } }
		protected internal bool MeasureInProgress { get; protected set; }
		protected internal bool MeasureDuringArrnge { get; private set; }
		protected internal bool IsStateValid { get; set; }
		protected internal bool IsMeasured { get; private set; }
		protected IVisualComponent Owner { get; private set; }
		protected ILayoutPanel Panel { get; private set; }
		protected internal IVisualElementAccessor<T> VisualItemsAccessor { get; private set; }
		protected internal virtual ElementPosition OwnElementPosition {
			get { return SchedulerItemsControl.GetElementPosition(this); }
			set { SchedulerItemsControl.SetElementPosition(this, value); }
		}
		protected internal long MeasureVersion { get { return measureVersion; } }
		protected internal bool IsDisposed { get; private set; }
		#region Visibility
		public Visibility Visibility {
			get { return visibility; }
			set {
				if (Visibility == value)
					return;
				visibility = value;
				OnVisibilityChanged();
			}
		}
		#endregion
		#region VirtualizationMode
		public VirtualizationMode VirtualizationMode {
			get { return virtualizationMode; }
			set {
				if (VirtualizationMode == value)
					return;
				virtualizationMode = value;
				OnVirtualizationModeChanged();
			}
		}
		#endregion
		#endregion
		public virtual void Initialize() {
			VisualItemsAccessor = CreateVisualItemsAccessor(Panel);
			SubscribeToEvents();
		}
		protected virtual void SubscribeToEvents() {
		}
		protected virtual void UnsubscribeFromEvents() {
		}
		public virtual void Arrange(Rect arrangeBounds) {
			if (Visibility != Visibility.Visible) {
				RenderSize = new Size();
				return;
			}
			if (!IsMeasured) {
				MeasureDuringArrnge = true;
				try {
					Measure(arrangeBounds.Size());
				} finally {
					MeasureDuringArrnge = false;
				}
			}
			RenderSize = ArrangeCore(arrangeBounds);
		}
		public virtual void Measure(Size availableSize) {
			if (CanUseCurrentState(availableSize) && IsMeasured)
				return;
			InvalidateState();
			if (Visibility != Visibility.Visible) {
				DesiredSize = new Size();
				return;
			}
			MeasureInProgress = true;
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.GanttViewGroupByResourceLayoutPanel, "->{0}.Measure", GetType().Name);
			try {
				Size desiredSize = MeasureCore(availableSize);
				if (!SchedulerSizeHelper.AreClose(desiredSize, DesiredSize)) {
					if (!MeasureDuringArrnge)
						Owner.OnChildrenChanged(this);
					DesiredSize = desiredSize;
				}
				AvailableSize = availableSize;
				IsStateValid = true;
				IsMeasured = true;
				IncreaseMeasureVersion();
			} finally {
				SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.GanttViewGroupByResourceLayoutPanel);
				MeasureInProgress = false;
			}
		}
		protected virtual void IncreaseMeasureVersion() {
			this.measureVersion++;
		}
		protected virtual bool CanUseCurrentState(Size availableSize) {
			return IsStateValid && SchedulerSizeHelper.AreClose(AvailableSize, availableSize);
		}
		protected virtual void OnVirtualizationModeChanged() {
			RaiseComponentChanged();
		}
		protected virtual void RaiseComponentChanged() {
			InvalidateState();
			Owner.OnChildrenChanged(this);
		}
		protected internal virtual void OnChildrenChanged(IVisualElement child) {
			if (MeasureInProgress)
				return;
			SchedulerLogger.Trace(XpfLoggerTraceLevel.GanttViewGroupByResourceLayoutPanel, "->{0}.OnChildrenChanged", GetType().Name);
			RaiseComponentChanged();
		}
		protected virtual void OnElementInserted(object sender, int index) {
			UIElementCollectionChangedNotifier.OnElementInserted(VisualItemsAccessor, sender, index);
		}
		protected virtual void OnElementRemoved(object sender, int index) {
			UIElementCollectionChangedNotifier.OnElementRemoved(VisualItemsAccessor, sender, index);
		}
		public virtual void InvalidateState() {
			IsStateValid = false;
		}
		public virtual void InvalidateMeasure() {
			VisualItemsAccessor.ForEach(element => element.InvalidateMeasure());
			RaiseComponentChanged();
		}
		public virtual void InvalidateArrange() {
			VisualItemsAccessor.ForEach(element => element.InvalidateArrange());
		}
		protected virtual void OnVisibilityChanged() {
			VisualItemsAccessor.ForEach(element => element.Visibility = Visibility);
			RaiseComponentChanged();
		}
		protected abstract IVisualElementAccessor<T> CreateVisualItemsAccessor(ILayoutPanel panel);
		protected abstract Size MeasureCore(Size availableSize);
		protected abstract Size ArrangeCore(Rect arrangeBounds);
		#region IUIElementCollectionChangedListener Members
		void IUIElementCollectionChangedListener.OnElementInserted(object sender, int index) {
			OnElementInserted(sender, index);
		}
		void IUIElementCollectionChangedListener.OnElementRemoved(object sender, int index) {
			OnElementRemoved(sender, index);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~VisualComponent() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (VisualItemsAccessor != null) {
					VisualItemsAccessor.Clear();
					VisualItemsAccessor = null;
				}
				UnsubscribeFromEvents();
			}
			IsDisposed = true;
		}
		#endregion
		#region IVisualComponent Members
		void IVisualComponent.OnChildrenChanged(IVisualElement child) {
			OnChildrenChanged(child);
		}
		int IVisualComponent.FirstChildIndex {
			get {
				if (VisualItemsAccessor.Count <= 0)
					return 0;
				T item = VisualItemsAccessor[0];
				return Panel.Children.IndexOf(item as UIElement);
			}
		}
		List<IVisualComponent> IVisualComponent.GetComponents() {
			List<IVisualComponent> result = new List<IVisualComponent>();
			int count = VisualItemsAccessor.Count;
			for (int i = 0; i < count; i++) {
				try {
					IVisualComponent component = VisualItemsAccessor[i] as IVisualComponent;
					if (component == null)
						continue;
					List<IVisualComponent> childComponents = component.GetComponents();
					if (childComponents.Count == 0)
						result.Add(component);
					else
						result.AddRange(childComponents);
				} catch {
					break;
				}
			}
			return result;
		}
		#endregion
		#region IEnumerable<IVisualElement> Members
		IEnumerator<IVisualElement> IEnumerable<IVisualElement>.GetEnumerator() {
			int count = VisualItemsAccessor.Count;
			for (int i = 0; i < count; i++)
				yield return VisualItemsAccessor[i];
		}
		#endregion
		#region IEnumerator Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((IEnumerable<IVisualElement>)this).GetEnumerator();
		}
		#endregion
		void ISupportCheckIntegrity.CheckIntegrity() {
			SchedulerAssert.CheckIntegrity(VisualItemsAccessor);
		}
	}
	#endregion
	#region ItemsBasedComponent (abstract class)
	[GeneratedCode("Suppress FxCop check", "")]
	public abstract class ItemsBasedComponent<TVisualItem, TViewModelItem> : VisualComponent<TVisualItem>, IWeakEventListener, ICellInfoProvider
		where TVisualItem : DependencyObject, IVisualElement
		where TViewModelItem : class {
		Orientation orientation;
		ObservableCollection<TViewModelItem> viewModelItems;
		protected ItemsBasedComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		#region Properties
		protected internal Rect[] Bounds { get; protected set; }
		protected bool RemoveUnusedItems { get { return VirtualizationMode == VirtualizationMode.Standard; } }
		public TVisualItem this[int index] { get { return VisualItemsAccessor[index]; } }
		#region ViewModelItems
		public ObservableCollection<TViewModelItem> ViewModelItems {
			get { return viewModelItems; }
			set {
				if (Object.ReferenceEquals(viewModelItems, value))
					return;
				ObservableCollection<TViewModelItem> oldValue = ViewModelItems;
				ObservableCollection<TViewModelItem> newValue = value;
				viewModelItems = newValue;
				OnViewModelItemsChanged(oldValue, newValue);
			}
		}
		void OnViewModelItemsChanged(ObservableCollection<TViewModelItem> oldValue, ObservableCollection<TViewModelItem> newValue) {
			if (oldValue != null)
				CollectionChangedEventManager.RemoveListener(oldValue, this);
			if (newValue != null)
				CollectionChangedEventManager.AddListener(newValue, this);
			RaiseComponentChanged();
		}
		#endregion
		#region Orientation
		public Orientation Orientation {
			get { return orientation; }
			set {
				if (Orientation == value)
					return;
				orientation = value;
				RaiseComponentChanged();
			}
		}
		#endregion
		#endregion
		protected virtual void OnViewModelItemsChanged() {
			RaiseComponentChanged();
		}
		public Size MeasureDesiredSize(Size availableSize) {
			return MeasureCore(availableSize);
		}
		protected override Size MeasureCore(Size availableSize) {
			if (ViewModelItems == null)
				return new Size();
			int count = ViewModelItems.Count;
			Bounds = GetBoundsForMeasure(availableSize, count);
			IVisualElementGenerator<TVisualItem, TViewModelItem> itemsGenerator = GetItemsGenerator();
			itemsGenerator.Reset();
			Size totalSize = new Size();
			for (int i = 0; i < count; i++) {
				TViewModelItem viewModelItem = ViewModelItems[i];
				bool isFirst = i == 0;
				bool isLast = i == (count - 1);
				SchedulerAssert.CheckIntegrity(Panel);
				TVisualItem control = itemsGenerator.GenerateNext(viewModelItem);
				CalculateElementPosition(control, isFirst, isLast);
				SchedulerAssert.CheckIntegrity(Panel);
				Size childAvailableSize;
				if (i < Bounds.Length)
					childAvailableSize = Bounds[i].Size();
				else
					childAvailableSize = Size.Empty;
				MeasureChild(control, childAvailableSize);
				totalSize = SchedulerSizeHelper.Union(totalSize, control.DesiredSize, Orientation);
				SchedulerAssert.CheckIntegrity(Panel);
			}
			itemsGenerator.Release(RemoveUnusedItems);
			SchedulerAssert.CheckIntegrity(Panel);
			return totalSize;
		}
		protected virtual void MeasureChild(TVisualItem child, Size childAvailableSize) {
			child.Measure(childAvailableSize);
		}
		protected override Size ArrangeCore(Rect arrangeBounds) {
			Point location = arrangeBounds.Location();
			Size finalSize = arrangeBounds.Size();
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.ItemsBasedComponent, "->ItemsBasedComponent<{0},{1}>.ArrangeCore: {2}-({3}), location={4}, finalSize={5}", typeof(TVisualItem).Name, typeof(TViewModelItem).Name, VisualElementHelper.GetElementName(this), VisualElementHelper.GetTypeName(this), location, finalSize);
			if (ViewModelItems == null)
				return new Size();
			int count = ViewModelItems.Count;
			Bounds = GetBoundsForArrange(finalSize, count);
			Size totalSize = new Size();
			ErrorBetweenExpectedAndActualBoundsCalculator errorCalculator = new ErrorBetweenExpectedAndActualBoundsCalculator(Orientation);
			for (int i = 0; i < count; i++) {
				TVisualItem control = VisualItemsAccessor[i];
				SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.ItemsBasedComponent, "| process child[{0}]: {1}-({2})", i, VisualElementHelper.GetElementName(control), VisualElementHelper.GetTypeName(control));
				Rect childBounds;
				if (i < Bounds.Length) {
					childBounds = Bounds[i];
					RectHelper.Offset(ref childBounds, location.X, location.Y);
				} else
					childBounds = Rect.Empty;
				Rect expectedBounds = CalculateCorrectedPoint(errorCalculator, childBounds, i);
				ArrangeChild(control, expectedBounds);
				CalculateBoundsError(errorCalculator, expectedBounds.Size(), control.RenderSize);
				SchedulerLogger.Trace(XpfLoggerTraceLevel.ItemsBasedComponent, "childBounds:{0}, correctedExpectedBounds={1}, actualBounds={2}", childBounds, expectedBounds, control.RenderSize);
				totalSize = SchedulerSizeHelper.Union(totalSize, control.RenderSize, Orientation);
				SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.ItemsBasedComponent);
			}
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.ItemsBasedComponent, "<-ItemsBasedComponent.ArrangeCore: totalSize={0}", totalSize);
			return totalSize;
		}
		protected virtual void CalculateBoundsError(ErrorBetweenExpectedAndActualBoundsCalculator errorCalculator, Size size, Size actualSize) {
			HelpCalculateBoundsError(errorCalculator, actualSize, size);
		}
		protected virtual Rect CalculateCorrectedPoint(ErrorBetweenExpectedAndActualBoundsCalculator errorCalculator, Rect bounds, int childIndex) {
			return HelpCalculateCorrectedPoint(errorCalculator, bounds, childIndex);
		}
		protected virtual void ArrangeChild(TVisualItem child, Rect expectedBounds) {
			child.Arrange(expectedBounds);
		}
		protected virtual void CalculateElementPosition(TVisualItem element, bool isFirst, bool isLast) {
			bool isHorizontalFirst = Orientation == Orientation.Horizontal ? isFirst : true;
			bool isHorizontalLast = Orientation == Orientation.Horizontal ? isLast : true;
			bool isVerticalFirst = Orientation == Orientation.Vertical ? isFirst : true;
			bool isVerticalLast = Orientation == Orientation.Vertical ? isLast : true;
			ElementPositionCore horizontalElementPosition = ElementPositionPropertyHelper.CalculateCore(OwnElementPosition.HorizontalElementPosition, isHorizontalFirst, isHorizontalLast);
			ElementPositionCore verticalElementPosition = ElementPositionPropertyHelper.CalculateCore(OwnElementPosition.VerticalElementPosition, isVerticalFirst, isVerticalLast);
			ElementPosition position = new ElementPosition(horizontalElementPosition, verticalElementPosition);
			SchedulerItemsControl.SetElementPosition(element, position);
		}
		protected internal virtual double[] GetChildrenPrimarySize() {
			return Bounds != null ? SchedulerSizeHelper.GetPrimarySizes(Bounds, Orientation) : null;
		}
		protected internal virtual double[] GetChildrenDisredSize() {
			int count = VisualItemsAccessor.Count;
			double[] result = new double[count];
			for (int i = 0; i < count; i++)
				result[i] = SchedulerSizeHelper.GetPrimarySize(VisualItemsAccessor[i].DesiredSize.ToRect(), Orientation);
			return result;
		}
		protected virtual Rect[] SplitSize(Size size, int count) {
			return PixelSnappedUniformGridLayoutHelper.SplitSizeDpiAware(size, count, OwnElementPosition.InnerContentPadding, Orientation);
		}
		protected virtual Rect[] GetBoundsForMeasure(Size size, int count) {
			return SplitSize(size, count);
		}
		protected virtual Rect[] GetBoundsForArrange(Size size, int count) {
			return SplitSize(size, count);
		}
		protected abstract IVisualElementGenerator<TVisualItem, TViewModelItem> GetItemsGenerator();
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			OnViewModelItemsChanged();
			return true;
		}
		#endregion
		#region Helper methods
		protected Rect HelpCalculateCorrectedPoint(ErrorBetweenExpectedAndActualBoundsCalculator errorCalculator, Rect bounds, int childIndex) {
			Point correctedPoint = errorCalculator.CorrectPoint(bounds.Location());
			Size correctedSize = bounds.Size();
			if (childIndex == ViewModelItems.Count - 1)
				correctedSize = errorCalculator.CorrectLastItemSize(correctedSize);
			return new Rect(correctedPoint, correctedSize);
		}
		protected void HelpCalculateBoundsError(ErrorBetweenExpectedAndActualBoundsCalculator errorCalculator, Size expectedSize, Size actualSize) {
			errorCalculator.CalculateError(actualSize, expectedSize);
		}
		#endregion
		#region ICellInfoProvider
		Rect ICellInfoProvider.GetCellRectByIndex(int index) {
			return Bounds[index];
		}
		int ICellInfoProvider.GetCellCount() {
			return Bounds.Length;
		}
		#endregion
	}
	#endregion
	#region DateHeaderLevelComponent
	public class DateHeaderLevelComponent : ItemsBasedComponent<VisualTimeScaleHeader, VisualTimeScaleHeaderContent> {
		public DateHeaderLevelComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		protected IDateHeaderComponentProperties Properties { get { return Panel.GetProperties<IDateHeaderComponentProperties>(); } }
		protected Style DateHeaderStyle { get { return Properties.DateHeaderStyle; } }
		protected BaseCellsDateHeaderLevelInfo BaseCellsInfo { get { return Properties.BaseCellsInfo; } }
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			PropertyChangedSubscriber.AddHandler(Properties, OnPropertyChanged);
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			PropertyChangedSubscriber.RemoveHandler(Properties, OnPropertyChanged);
		}
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			RaiseComponentChanged();
		}
		protected override IVisualElementAccessor<VisualTimeScaleHeader> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new PanelChildrenAccessor<VisualTimeScaleHeader>(panel, ZIndex);
		}
		protected override IVisualElementGenerator<VisualTimeScaleHeader, VisualTimeScaleHeaderContent> GetItemsGenerator() {
			return new VisualTimeScaleHeaderGenerator(this, VisualItemsAccessor, DateHeaderStyle);
		}
		protected override void ArrangeChild(VisualTimeScaleHeader child, Rect childBounds) {
			base.ArrangeChild(child, childBounds);
			child.MaxWidth = childBounds.Width;
		}
		protected override Rect[] GetBoundsForMeasure(Size size, int count) {
			Rect[] result = new Rect[count];
			for (int index = 0; index < count; index++)
				result[index] = new Rect(new Point(0, 0), size);
			return result;
		}
		protected override Rect[] GetBoundsForArrange(Size size, int count) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.TimelineViewPanelGroupByNone, "->DateHeaderLevelComponent.GetBoundsForArrange: childCount", count);
			List<Rect> bounds = BaseCellsInfo.GetBaseCellsRects();
			Rect[] result = new Rect[count];
			for (int index = 0; index < count; index++) {
				VisualTimeScaleHeader header = VisualItemsAccessor[index];
				result[index] = GetChildFinalRect(bounds, header);
				SchedulerLogger.Trace(XpfLoggerTraceLevel.TimelineViewPanelGroupByNone, "child[{0}].Bounds={1}", index, result[index]);
			}
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.TimelineViewPanelGroupByNone);
			return result;
		}
		protected virtual int GetBaseCellCount() {
			int count = ViewModelItems.Count;
			if (count > 0)
				return VisualItemsAccessor[count - 1].EndOffset.LinkCellIndex + 1;
			else
				return 0;
		}
		protected Rect GetChildFinalRect(List<Rect> childrenBounds, VisualTimeScaleHeader header) {
			double left = GetPosition(childrenBounds[header.StartOffset.LinkCellIndex], header.StartOffset);
			double right = GetPosition(childrenBounds[header.EndOffset.LinkCellIndex], header.EndOffset);
			return new Rect(left, 0, Math.Max(0, right - left), header.DesiredSize.Height);
		}
		internal double GetPosition(Rect childrenBounds, SingleTimelineHeaderCellOffset cellOffset) {
			return childrenBounds.Left + (int)(childrenBounds.Width * cellOffset.RelativeOffset);
		}
	}
	#endregion
	#region DateHeaderComponent
	public class DateHeaderComponent : ItemsBasedComponent<DateHeaderLevelComponent, VisualTimeScaleHeaderLevel> {
		public DateHeaderComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		#region TopLevelElementPosition
		ElementPosition lastSettedTopLevelElementPosition = ElementPosition.Standalone;
		public ElementPosition TopLevelElementPosition {
			get { return (ElementPosition)GetValue(TopLevelElementPositionProperty); }
			set {
				if (!Object.ReferenceEquals(lastSettedTopLevelElementPosition, value))
					SetValue(TopLevelElementPositionProperty, value);
			}
		}
		public static readonly DependencyProperty TopLevelElementPositionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DateHeaderComponent, ElementPosition>("TopLevelElementPosition", ElementPosition.Standalone, (d, e) => d.OnTopLevelElementPositionChanged(e.OldValue, e.NewValue), null);
		void OnTopLevelElementPositionChanged(ElementPosition oldValue, ElementPosition newValue) {
			RaiseComponentChanged();
			this.lastSettedTopLevelElementPosition = newValue;
		}
		#endregion
		#region LevelElementPosition
		ElementPosition lastSettedLevelElementPosition = ElementPosition.Standalone;
		public ElementPosition LevelElementPosition {
			get { return (ElementPosition)GetValue(LevelElementPositionProperty); }
			set {
				if (!Object.ReferenceEquals(lastSettedLevelElementPosition, value))
					SetValue(LevelElementPositionProperty, value);
			}
		}
		public static readonly DependencyProperty LevelElementPositionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DateHeaderComponent, ElementPosition>("LevelElementPosition", ElementPosition.Standalone, (d, e) => d.OnOtherElementPositionChanged(e.OldValue, e.NewValue), null);
		void OnOtherElementPositionChanged(ElementPosition oldValue, ElementPosition newValue) {
			RaiseComponentChanged();
			this.lastSettedLevelElementPosition = newValue;
		}
		#endregion
		protected override void CalculateElementPosition(DateHeaderLevelComponent element, bool isFirst, bool isLast) {
			ElementPosition position = isFirst ? TopLevelElementPosition : LevelElementPosition;
			SchedulerItemsControl.SetElementPosition(element, position);
		}
		protected override IVisualElementAccessor<DateHeaderLevelComponent> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new VisualComponentAccessor<DateHeaderLevelComponent>(panel);
		}
		protected override IVisualElementGenerator<DateHeaderLevelComponent, VisualTimeScaleHeaderLevel> GetItemsGenerator() {
			DateHeaderLevelGenerator generator = new DateHeaderLevelGenerator(this, VisualItemsAccessor);
			return generator;
		}
		protected override Rect CalculateCorrectedPoint(ErrorBetweenExpectedAndActualBoundsCalculator errorCalculator, Rect bounds, int childIndex) {
			return HelpCalculateCorrectedPoint(errorCalculator, bounds, childIndex);
		}
		protected override void CalculateBoundsError(ErrorBetweenExpectedAndActualBoundsCalculator errorCalculator, Size expectedSize, Size actualSize) {
			HelpCalculateBoundsError(errorCalculator, expectedSize, actualSize);
		}
	}
	#endregion
	#region ResourceHeaderComponent
	public class ResourceHeaderComponent : ItemsBasedComponent<VisualResourceHeader, VisualResource> {
		Rect[] headersBounds;
		public ResourceHeaderComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		IResourceHeaderComponentProperties Properties { get { return Panel.GetProperties<IResourceHeaderComponentProperties>(); } }
		protected Style ResourceHeaderStyle { get { return Properties.ResourceHeaderStyle; } }
		protected override void OnVisibilityChanged() {
			base.OnVisibilityChanged();
		}
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			PropertyChangedSubscriber.AddHandler(Properties, OnPropertyChanged);
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			PropertyChangedSubscriber.RemoveHandler(Properties, OnPropertyChanged);
		}
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			RaiseComponentChanged();
		}
		protected override void OnViewModelItemsChanged() {
			this.headersBounds = null;
			base.OnViewModelItemsChanged();
		}
		protected override Rect[] GetBoundsForMeasure(Size size, int count) {
			if (headersBounds != null)
				return headersBounds;
			Rect[] result = new Rect[count];
			for (int i = 0; i < count; i++)
				result[i] = new Rect(new Point(0, 0), size);
			return result;
		}
		protected override Rect[] GetBoundsForArrange(Size size, int count) {
			return this.headersBounds;
		}
		Rect[] CreateRourceHeaderBounds(double[] resourceHeights) {
			int count = resourceHeights.Length;
			Rect[] result = new Rect[count];
			double y = 0;
			for (int i = 0; i < count; i++) {
				double height = resourceHeights[i];
				result[i] = new Rect(new Point(0, y), new Size(DesiredSize.Width, height));
				y += height;
			}
			return result;
		}
		public virtual void SyncMeasure(double[] resourceHeights) {
			if (ViewModelItems == null)
				return;
			Rect[] childrenBounds = CreateRourceHeaderBounds(resourceHeights);
			int count = Math.Min(ViewModelItems.Count, childrenBounds.Length);
			MeasureInProgress = true;
			for (int i = 0; i < count; i++)
				VisualItemsAccessor[i].Measure(childrenBounds[i].Size());
			IsStateValid = true;
			MeasureInProgress = false;
			IncreaseMeasureVersion();
		}
		public virtual void Arrange(double[] resourceHeights, Rect arrangeBounds) {
			this.headersBounds = CreateRourceHeaderBounds(resourceHeights);
			Arrange(arrangeBounds);
		}
		protected override IVisualElementAccessor<VisualResourceHeader> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new PanelChildrenAccessor<VisualResourceHeader>(panel, ZIndex);
		}
		protected override IVisualElementGenerator<VisualResourceHeader, VisualResource> GetItemsGenerator() {
			return new ResourceHeaderGenerator(this, VisualItemsAccessor, ResourceHeaderStyle);
		}
	}
	#endregion
	#region SelectionBarComponent
	public class SelectionBarComponent : ItemsBasedComponent<VisualTimeCell, VisualTimeCellBaseContent> {
		public SelectionBarComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		ISelectionBarComponentProperties Properties { get { return Panel.GetProperties<ISelectionBarComponentProperties>(); } }
		protected Style CellStyle { get { return Properties.SelectionBarCellStyle; } }
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			PropertyChangedSubscriber.AddHandler(Properties, OnPropertyChanged);
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			PropertyChangedSubscriber.RemoveHandler(Properties, OnPropertyChanged);
		}
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			RaiseComponentChanged();
		}
		protected override IVisualElementAccessor<VisualTimeCell> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new PanelChildrenAccessor<VisualTimeCell>(panel, ZIndex);
		}
		protected override IVisualElementGenerator<VisualTimeCell, VisualTimeCellBaseContent> GetItemsGenerator() {
			return new VisualTimeCellGenerator(this, VisualItemsAccessor, CellStyle);
		}
		protected override Rect CalculateCorrectedPoint(ErrorBetweenExpectedAndActualBoundsCalculator errorCalculator, Rect bounds, int childIndex) {
			return HelpCalculateCorrectedPoint(errorCalculator, bounds, childIndex);
		}
		protected override void CalculateBoundsError(ErrorBetweenExpectedAndActualBoundsCalculator errorCalculator, Size expectedSize, Size actualSize) {
			HelpCalculateBoundsError(errorCalculator, expectedSize, actualSize);
		}
	}
	#endregion
	#region CellsVisualComponent
	public class CellsVisualComponent : ItemsBasedComponent<VisualSingleTimelineCell, VisualTimeCellBaseContent> {
		protected readonly static Thickness EmptyPadding = new Thickness();
		Thickness padding;
		public CellsVisualComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		protected ICellsComponentProperties Properties { get { return Panel.GetProperties<ICellsComponentProperties>(); } }
		protected Style CellStyle { get { return Properties.CellStyle; } set { Properties.CellStyle = value; } }
		#region Padding
		public Thickness Padding {
			get { return padding; }
			set {
				if (Padding == value)
					return;
				padding = value;
				OnPaddingChanged();
			}
		}
		protected virtual void OnPaddingChanged() {
			RaiseComponentChanged();
		}
		#endregion
		public override int ZIndex { get { return 100; } }
		public double ConvertTimeToPosition(DateTime dateTime) {
			int cellCount = ViewModelItems.Count;
			int targetCellIndx = -1;
			double correctionFactor = 0;
			for (int i = 0; i < cellCount; i++) {
				VisualTimeCellBaseContent cellModel = ViewModelItems[i];
				TimeInterval cellInterval = cellModel.GetInterval();
				if (cellInterval.Contains(dateTime)) {
					targetCellIndx = i;
					TimeSpan nowOffset = dateTime - cellInterval.Start;
					correctionFactor = (double)nowOffset.Ticks / cellInterval.Duration.Ticks;
					break;
				}
			}
			if (targetCellIndx < 0) 
				return -1;
			Rect targetCellBounds = Bounds[targetCellIndx];
			double posX = targetCellBounds.X + targetCellBounds.Width * correctionFactor;
			return posX;
		}
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			PropertyChangedSubscriber.AddHandler(Properties, OnPropertyChanged);
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			PropertyChangedSubscriber.RemoveHandler(Properties, OnPropertyChanged);
		}
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			RaiseComponentChanged();
		}
		protected override void RaiseComponentChanged() {
			base.RaiseComponentChanged();
			ICellsComponentChangedListener listener = Owner as ICellsComponentChangedListener;
			if (listener != null)
				listener.OnCellsChanged();
		}
		protected override Rect[] SplitSize(Size size, int count) {
			Thickness contentPadding = Padding != EmptyPadding ? Padding : OwnElementPosition.InnerContentPadding;
			return PixelSnappedUniformGridLayoutHelper.SplitSizeDpiAware(size, count, contentPadding, Orientation);
		}
		protected override IVisualElementAccessor<VisualSingleTimelineCell> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new PanelChildrenAccessor<VisualSingleTimelineCell>(panel, ZIndex);
		}
		protected override IVisualElementGenerator<VisualSingleTimelineCell, VisualTimeCellBaseContent> GetItemsGenerator() {
			return new VisualSingleTimelineCellGenerator(this, VisualItemsAccessor, CellStyle);
		}
		protected override Rect CalculateCorrectedPoint(ErrorBetweenExpectedAndActualBoundsCalculator errorCalculator, Rect bounds, int childIndex) {
			return HelpCalculateCorrectedPoint(errorCalculator, bounds, childIndex);
		}
		protected override void CalculateBoundsError(ErrorBetweenExpectedAndActualBoundsCalculator errorCalculator, Size expectedSize, Size actualSize) {
			HelpCalculateBoundsError(errorCalculator, expectedSize, actualSize);
		}
	}
	#endregion
	#region CellContainerVisualComponent
	public class CellContainerVisualComponent : VisualComponent<IVisualComponent>, ICellsComponentChangedListener {
		#region Fields
		AppointmentControlObservableCollection appointments;
		AppointmentControlObservableCollection draggedAppointments;
		Thickness cellsPadding;
		VisualTimeCellContentCollection cells;
		SchedulerControl scheduler;
		#endregion
		public CellContainerVisualComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		#region Properties
		#region Cells
		public VisualTimeCellContentCollection Cells {
			get { return cells; }
			set {
				if (Object.ReferenceEquals(Cells, value))
					return;
				cells = value;
				OnCellsChanged();
			}
		}
		void OnCellsChanged() {
			CellsComponent.ViewModelItems = Cells;
		}
		#endregion
		#region Appointments
		public AppointmentControlObservableCollection Appointments {
			get { return appointments; }
			set {
				if (Object.ReferenceEquals(Appointments, value))
					return;
				appointments = value;
				OnAppointmentsChanged();
			}
		}
		void OnAppointmentsChanged() {
			AppointmentsComponent.Appointments = Appointments;
		}
		#endregion
		#region DraggedAppointments
		public AppointmentControlObservableCollection DraggedAppointments {
			get { return draggedAppointments; }
			set {
				if (Object.ReferenceEquals(DraggedAppointments, value))
					return;
				draggedAppointments = value;
				OnDraggedAppointmentsChanged();
			}
		}
		void OnDraggedAppointmentsChanged() {
			DraggedAppointmentsComponent.ViewModelItems = DraggedAppointments;
		}
		#endregion
		#region VisualTimeline
		VisualTimeline lastSettedVisualTimeline;
		public VisualTimeline VisualTimeline {
			get { return (VisualTimeline)GetValue(VisualTimelineProperty); }
			set {
				if (!Object.ReferenceEquals(lastSettedVisualTimeline, value))
					SetValue(VisualTimelineProperty, value);
			}
		}
		public static readonly DependencyProperty VisualTimelineProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<CellContainerVisualComponent, VisualTimeline>("VisualTimeline", null, (d, e) => d.OnVisualTimelineChanged(e.OldValue, e.NewValue));
		void OnVisualTimelineChanged(VisualTimeline oldValue, VisualTimeline newValue) {
			if (newValue != null) {
				Scheduler = newValue.View.Control;
				InnerBindingHelper.SetBinding(this, newValue.CellContainer, CellContainerVisualComponent.SelectedCellsProperty, "SelectedCells");
			}
			this.lastSettedVisualTimeline = newValue;
		}
		#endregion
		#region Scheduler
		public SchedulerControl Scheduler {
			get { return scheduler; }
			set {
				if (Object.ReferenceEquals(Scheduler, value))
					return;
				scheduler = value;
				OnSchedulerChanged();
			}
		}
		void OnSchedulerChanged() {
			AppointmentsComponent.Scheduler = Scheduler;
			DraggedAppointmentsComponent.Scheduler = Scheduler;
		}
		#endregion
		#region SelectedCells
		SelectedCellIndexesInterval lastSettedSelectedCells;
		public SelectedCellIndexesInterval SelectedCells {
			get { return (SelectedCellIndexesInterval)GetValue(SelectedCellsProperty); }
			set {
				if (!Object.ReferenceEquals(lastSettedSelectedCells, value))
					SetValue(SelectedCellsProperty, value);
			}
		}
		public static readonly DependencyProperty SelectedCellsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<CellContainerVisualComponent, SelectedCellIndexesInterval>("SelectedCells", null, (d, e) => d.OnSelectedCellsChanged(e.OldValue, e.NewValue));
		void OnSelectedCellsChanged(SelectedCellIndexesInterval oldValue, SelectedCellIndexesInterval newValue) {
			if (SelectionComponent != null)
				SelectionComponent.SelectedCells = newValue;
			this.lastSettedSelectedCells = newValue;
		}
		#endregion
		#region CellsPadding
		public Thickness CellsPadding {
			get { return cellsPadding; }
			set {
				if (CellsPadding == value)
					return;
				cellsPadding = value;
				OnPaddingChanged();
			}
		}
		protected virtual void OnPaddingChanged() {
			CellsComponent.Padding = CellsPadding;
		}
		#endregion
		public CellsVisualComponent CellsComponent { get; private set; }
		public ScrollableAppointmentsComponent AppointmentsComponent { get; private set; }
		public HorizontalDraggedAppointmentsComponent DraggedAppointmentsComponent { get; private set; }
		public SelectionComponent SelectionComponent { get; private set; }
		public MoreButtonsComponent MoreButtonsComponent { get; private set; }
		public double RightOffset { get { return AppointmentsComponent.RightOffset; } }
		protected IResourceComponentProperties ResourceComponentProperties { get { return Panel.GetProperties<IResourceComponentProperties>(); } }
		#endregion
		public override void Initialize() {
			base.Initialize();
			CreateComponents(Panel);
			AssignVirtualizationMode();
		}
		protected virtual void CreateComponents(ILayoutPanel panel) {
			CellsComponent = new CellsVisualComponent(panel, this);
			VisualItemsAccessor.Add(CellsComponent);
			CellsComponent.Orientation = Orientation.Horizontal;
			CellsComponent.Initialize();
			SelectionComponent = new SelectionComponent(panel, this);
			VisualItemsAccessor.Add(SelectionComponent);
			SelectionComponent.Initialize();
			SelectionComponent.CellsComponent = CellsComponent;
			AppointmentsComponent = new ScrollableAppointmentsComponent(panel, this);
			VisualItemsAccessor.Add(AppointmentsComponent);
			AppointmentsComponent.Initialize();
			AppointmentsComponent.CellsComponent = CellsComponent;
			DraggedAppointmentsComponent = new HorizontalDraggedAppointmentsComponent(panel, this);
			VisualItemsAccessor.Add(DraggedAppointmentsComponent);
			DraggedAppointmentsComponent.CellsComponent = CellsComponent;
			DraggedAppointmentsComponent.Initialize();
			MoreButtonsComponent = new MoreButtonsComponent(panel, this);
			VisualItemsAccessor.Add(MoreButtonsComponent);
			MoreButtonsComponent.Initialize();
			MoreButtonsComponent.ViewModelItems = new ObservableCollection<MoreButtonViewModel>();
			MoreButtonsComponent.CellsComponent = CellsComponent;
			AppointmentsComponent.MoreButtonsComponent = MoreButtonsComponent;
			SchedulerAssert.CheckIntegrity(Panel);
		}		
		protected override void OnVirtualizationModeChanged() {
			base.OnVirtualizationModeChanged();
			AssignVirtualizationMode();
		}
		protected void AssignVirtualizationMode() {
			VisualItemsAccessor.ForEach(element => element.VirtualizationMode = VirtualizationMode);
		}
		protected override Size MeasureCore(Size availableSize) {
			SchedulerItemsControl.SetElementPosition(CellsComponent, OwnElementPosition);
			CellsComponent.Measure(availableSize);
			SelectionComponent.Measure(availableSize);
			availableSize.Height = Math.Max(0, availableSize.Height - 1);
			AppointmentsComponent.Measure(availableSize);
			DraggedAppointmentsComponent.Measure(availableSize);
			CalculateMoreButtonViewModelItems();
			MoreButtonsComponent.Measure(availableSize);
			return CellsComponent.DesiredSize;
		}
		protected override Size ArrangeCore(Rect arrangeBounds) {
			CellsComponent.Arrange(arrangeBounds);
			SelectionComponent.Arrange(arrangeBounds);
			arrangeBounds.Height = Math.Max(0, arrangeBounds.Height - 1);
			AppointmentsComponent.Arrange(arrangeBounds);
			DraggedAppointmentsComponent.Arrange(arrangeBounds);
			MoreButtonsComponent.Arrange(arrangeBounds);
			return CellsComponent.RenderSize;
		}
		protected override IVisualElementAccessor<IVisualComponent> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new VisualComponentAccessor<IVisualComponent>(panel);
		}
		void CalculateMoreButtonViewModelItems() {
			ObservableCollection<MoreButtonViewModel> viewModelItems = MoreButtonsComponent.ViewModelItems;
			int cellsCount = CellsComponent.ViewModelItems.Count;
			for (int i = 0; i < cellsCount; i++) {
				if (i >= viewModelItems.Count)
					viewModelItems.Add(new MoreButtonViewModel());
				MoreButtonViewModel viewModel = viewModelItems[i];
				CalculateMoreButtonViewModel(viewModel, i);
			}
			for (int i = viewModelItems.Count - 1; i >= cellsCount; i--)
				viewModelItems.RemoveAt(i);
		}
		void CalculateMoreButtonViewModel(MoreButtonViewModel viewModel, int cellIndex) {
			TimeInterval interval = CellsComponent.ViewModelItems[cellIndex].GetInterval();
			long ticks = interval.Duration.Ticks / 2;
			DateTime date = interval.Start.AddTicks(ticks);
			viewModel.Date = date;
			viewModel.Scheduler = Scheduler;
			Visibility newValue = CalculateMoreButtonVisibility(cellIndex);
			viewModel.Visibility = newValue;
		}
		System.Windows.Visibility CalculateMoreButtonVisibility(int cellIndex) {
			if (!MoreButtonsComponent.ShowMoreButtons)
				return System.Windows.Visibility.Collapsed;
			if (ResourceComponentProperties.ResourceScrollBarVisible != SchedulerScrollBarVisibility.Never)
				return Visibility.Collapsed;
			return AppointmentsComponent.LayoutInfo.GetFitIntoCell(cellIndex) ? Visibility.Collapsed : Visibility.Visible;
		}
		#region ICellsComponentChangedListener
		void ICellsComponentChangedListener.OnCellsChanged() {
			OnCellsComponentChanged(SelectionComponent);
			OnCellsComponentChanged(AppointmentsComponent);
			OnCellsComponentChanged(DraggedAppointmentsComponent);
			OnCellsComponentChanged(MoreButtonsComponent);
		}
		void OnCellsComponentChanged(ICellsComponentChangedListener listener) {
			if (listener != null)
				listener.OnCellsChanged();
		}
		#endregion
		public List<Size> GetBaseCellBounds() {
			if (CellsComponent.VisualItemsAccessor.Count < 1)
				return null;
			List<Size> result = new List<Size>();
			CellsComponent.VisualItemsAccessor.ForEach(item => result.Add(item.RenderSize));
			return result;
		}
	}
	#endregion
	#region ScrollableAppointmentsComponent
	public class ScrollableAppointmentsComponent : VisualComponent<SchedulerContentPresenter>, ICellsComponentChangedListener, ISupportCheckIntegrity {
		#region Fields
		SchedulerControl scheduler;
		CellsVisualComponent cellsComponent;
		AppointmentControlObservableCollection appointments;
		MoreButtonsComponent moreButtonsComponent;
		#endregion
		public ScrollableAppointmentsComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		#region Properties
		#region Appointments
		public AppointmentControlObservableCollection Appointments {
			get { return appointments; }
			set {
				if (Object.ReferenceEquals(Appointments, value))
					return;
				appointments = value;
				OnAppointmentsChanged();
			}
		}
		void OnAppointmentsChanged() {
			InnerAppointmentsComponent.ViewModelItems = Appointments;
			RaiseComponentChanged();
		}
		#endregion
		#region Scheduler
		public SchedulerControl Scheduler {
			get { return scheduler; }
			set {
				if (Object.ReferenceEquals(Scheduler, value))
					return;
				scheduler = value;
				OnSchedulerChanged();
			}
		}
		void OnSchedulerChanged() {
			InnerAppointmentsComponent.Scheduler = Scheduler;
		}
		#endregion
		#region CellsComponent
		public CellsVisualComponent CellsComponent {
			get { return cellsComponent; }
			set {
				if (Object.ReferenceEquals(CellsComponent, value))
					return;
				cellsComponent = value;
				OnCellsComponentChanged();
			}
		}
		void OnCellsComponentChanged() {
			InnerAppointmentsComponent.CellsComponent = CellsComponent;
		}
		#endregion
		internal double RightOffset { get { return CalculateRightOffset(); } }
		internal AppointmentsLayoutInfo LayoutInfo { get { return InnerAppointmentsComponent.LayoutInfo; } }
		protected AppointmentsLayoutPanel AppointmentsPanel { get; private set; }
		protected SchedulerContentPresenter AppointmentsContainer { get; private set; }
		protected internal HorizontalAppointmentsComponent InnerAppointmentsComponent { get; private set; }
		protected IResourceComponentProperties Properties { get { return Panel.GetProperties<IResourceComponentProperties>(); } }
		protected SchedulerScrollBarVisibility ResourceScrollBarVisible { get { return Properties.ResourceScrollBarVisible; } }
		public MoreButtonsComponent MoreButtonsComponent {
			get {
				return moreButtonsComponent;
			}
			set {
				if (moreButtonsComponent == value)
					return;
				moreButtonsComponent = value;
				OnMoreButtonsComponentChanged();
			}
		}
		public override int ZIndex { get { return 150; } }
		#endregion
		public override void Initialize() {
			base.Initialize();
			InitializeAppointmentComponent();
		}
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			PropertyChangedSubscriber.AddHandler(Properties, OnPropertyChanged);
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			PropertyChangedSubscriber.RemoveHandler(Properties, OnPropertyChanged);
		}
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "ResourceScrollBarVisible")
				InitializeAppointmentComponent();
			RaiseComponentChanged();
		}
		protected double CalculateRightOffset() {
			return AppointmentsContainer.ActualWidth - AppointmentsPanel.ActualWidth;
		}
		protected virtual void InitializeAppointmentComponent() {
			AppointmentsPanel = new AppointmentsLayoutPanel(Panel);
			AppointmentsContainer = new SchedulerContentPresenter();
			AppointmentsContainer.Content = GetAppointmentsContainerContent(AppointmentsPanel);
			AddAppointmentsContainer(AppointmentsContainer);
			if (InnerAppointmentsComponent != null)
				InnerAppointmentsComponent.Dispose();
			InnerAppointmentsComponent = new HorizontalAppointmentsComponent(AppointmentsPanel, this);
			InnerAppointmentsComponent.Initialize();
			InnerAppointmentsComponent.IsResizable = ResourceScrollBarVisible != SchedulerScrollBarVisibility.Never;
			InnerAppointmentsComponent.ViewModelItems = Appointments;
			InnerAppointmentsComponent.CellsComponent = CellsComponent;
			InnerAppointmentsComponent.Scheduler = Scheduler;
			InnerAppointmentsComponent.VirtualizationMode = VirtualizationMode;
			InnerAppointmentsComponent.Orientation = Orientation.Horizontal;
			AppointmentsPanel.AppointmentsComponent = InnerAppointmentsComponent;
			SchedulerAssert.CheckIntegrity(Panel);
		}
		void ISupportCheckIntegrity.CheckIntegrity() {
			SchedulerAssert.CheckIntegrity(VisualItemsAccessor);
			SchedulerAssert.CheckIntegrity((Owner as CellContainerVisualComponent).SelectionComponent.VisualItemsAccessor);			
		}
		void AddAppointmentsContainer(SchedulerContentPresenter presenter) {
			VisualItemsAccessor.Clear();
			VisualItemsAccessor.Add(presenter);
		}
		protected override void OnVirtualizationModeChanged() {
			base.OnVirtualizationModeChanged();
			InnerAppointmentsComponent.VirtualizationMode = VirtualizationMode;
		}
		protected virtual object GetAppointmentsContainerContent(AppointmentsLayoutPanel appointmentPanel) {
			return ResourceScrollBarVisible != SchedulerScrollBarVisibility.Never ? (object)CreateScrollViewer(appointmentPanel) : appointmentPanel;
		}
		protected virtual ScrollViewer CreateScrollViewer(AppointmentsLayoutPanel appointmentPanel) {
			ScrollViewer result = new ScrollViewer();
			result.BorderThickness = new Thickness(0);
			result.Padding = new Thickness(0);
			result.Content = appointmentPanel;
			result.VerticalScrollBarVisibility = CalculateVerticalScrollBarVisibility(ResourceScrollBarVisible);
			result.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Hidden;
			appointmentPanel.MoreButtonsComponent = moreButtonsComponent;
			new LoadedUnloadedSubscriber(result, SubscribeScrollViewerEvents, UnsubscribeScrollViewerEvents);
			return result;
		}
		ScrollBarVisibility CalculateVerticalScrollBarVisibility(SchedulerScrollBarVisibility schedulerScrollBarVisible) {
			if (schedulerScrollBarVisible == SchedulerScrollBarVisibility.Never)
				return ScrollBarVisibility.Hidden;
			if (schedulerScrollBarVisible == SchedulerScrollBarVisibility.Always)
				return ScrollBarVisibility.Visible;
			if (schedulerScrollBarVisible == SchedulerScrollBarVisibility.Auto)
				return ScrollBarVisibility.Auto;
			return ScrollBarVisibility.Disabled;
		}
		void SubscribeScrollViewerEvents(FrameworkElement element) {
			ScrollViewer sw = element as ScrollViewer;
			sw.SizeChanged += OnViewportSizeChanged;
			InnerAppointmentsComponent.ViewportSize = new Size(sw.ViewportWidth, sw.ViewportHeight);
		}
		void UnsubscribeScrollViewerEvents(FrameworkElement element) {
			ScrollViewer sw = element as ScrollViewer;
			sw.SizeChanged -= OnViewportSizeChanged;
		}
		void OnViewportSizeChanged(object sender, SizeChangedEventArgs e) {
			ScrollViewer sw = (ScrollViewer)sender;
			InnerAppointmentsComponent.ViewportSize = new Size(sw.ViewportWidth, sw.ViewportHeight);
		}
		void OnMoreButtonsComponentChanged() {
			AppointmentsPanel.MoreButtonsComponent = moreButtonsComponent;
		}
		protected internal override void OnChildrenChanged(IVisualElement child) {
			AppointmentsContainer.InvalidateMeasure();
			AppointmentsPanel.InvalidateMeasure();
			base.OnChildrenChanged(child);
		}
		public override void InvalidateMeasure() {
			base.InvalidateMeasure();
			AppointmentsPanel.InvalidateMeasure();
		}
		protected override Size MeasureCore(Size availableSize) {
			AppointmentsContainer.Measure(availableSize);
			return InnerAppointmentsComponent.DesiredSize;
		}
		protected override Size ArrangeCore(Rect arrangeBounds) {
			AppointmentsContainer.Arrange(arrangeBounds);
			return InnerAppointmentsComponent.RenderSize;
		}
		protected override IVisualElementAccessor<SchedulerContentPresenter> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new PanelChildrenAccessor<SchedulerContentPresenter>(panel, ZIndex);
		}
		void ICellsComponentChangedListener.OnCellsChanged() {
			RaiseComponentChanged();
		}
	}
	#endregion
	#region SelectionComponent
	public class SelectionComponent : VisualComponent<SchedulerSelection>, ICellsComponentChangedListener {
		CellsVisualComponent cellsComponent;
		SelectedCellIndexesInterval selectedCells;
		public SelectionComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		public SchedulerSelection SelectionControl { get; private set; }
		public override int ZIndex { get { return 140; } }
		ISelectionComponentProperties Properties { get { return Panel.GetProperties<ISelectionComponentProperties>(); } }
		protected ControlTemplate SelectionTemplate { get { return Properties.SelectionTemplate; } }
		#region CellsComponent
		public CellsVisualComponent CellsComponent {
			get { return cellsComponent; }
			set {
				if (cellsComponent == value)
					return;
				cellsComponent = value;
				OnCellsComponentChanged();
			}
		}
		#endregion
		#region SelectedCells
		public SelectedCellIndexesInterval SelectedCells {
			get { return selectedCells; }
			set {
				if (Object.ReferenceEquals(SelectedCells, value))
					return;
				selectedCells = value;
				OnSelectedCellsChanged();
			}
		}
		void OnSelectedCellsChanged() {
			RaiseComponentChanged();
			SchedulerLogger.Trace(XpfLoggerTraceLevel.VisualComponent, "!!!!!!!!!!!!!!!!!!->SelectionComponent.OnSelectedCellsChanged: {0}", this.GetHashCode());
		}
		#endregion
		void OnCellsComponentChanged() {
			RaiseComponentChanged();
		}
		public override void Initialize() {
			base.Initialize();
			SelectionControl = new SchedulerSelection();
			SelectionControl.Template = SelectionTemplate;
			VisualItemsAccessor.Add(SelectionControl);
		}
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			PropertyChangedSubscriber.AddHandler(Properties, OnPropertyChanged);
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			PropertyChangedSubscriber.RemoveHandler(Properties, OnPropertyChanged);
		}
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "SelectionTemplate")
				SelectionControl.Template = SelectionTemplate;
			RaiseComponentChanged();
		}
		protected override Size MeasureCore(Size availableSize) {
			ReFillSelectionControl();
			SelectionControl.Recalculate();
			if (CellsComponent != null)
				availableSize = CellsComponent.DesiredSize;
			SelectionControl.Measure(availableSize);
			return new Size(1, 1);
		}
		protected override Size ArrangeCore(Rect arrangeBounds) {
			Size actualFinalSize = arrangeBounds.Size();
			if (CellsComponent != null)
				actualFinalSize = CellsComponent.RenderSize;
			SelectionControl.Arrange(new Rect(arrangeBounds.Location(), actualFinalSize));
			return arrangeBounds.Size();
		}
		void ReFillSelectionControl() {
			SelectionControl.ClearBounds();
			if (SelectedCells == null)
				return;
			SelectionControl.Interval = SelectedCells.Interval;
			SelectionControl.Resource = SelectedCells.Resource;
			int firstCellIndex = SelectedCells.StartCellIndex;
			int lastCellIndex = SelectedCells.EndCellIndex;
			Rect[] cellBounds = CellsComponent.Bounds;
			if (cellBounds == null)
				return;
			for (int i = firstCellIndex; i <= lastCellIndex; i++) {
				Rect rect = cellBounds[i];
				SelectionControl.AddRect(rect);
			}
		}
		protected override IVisualElementAccessor<SchedulerSelection> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new PanelChildrenAccessor<SchedulerSelection>(panel, ZIndex);
		}
		void ICellsComponentChangedListener.OnCellsChanged() {
			RaiseComponentChanged();
		}
	}
	#endregion
	#region ResourceComponent
	public class ResourceComponent : VisualComponent<IVisualComponent>, ISupportElementPosition {
		#region Fields
		VisualResource resource;
		VisualSimpleResourceIntervalCollection resourceIntervals;
		Thickness cellsPadding;
		#endregion
		public ResourceComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		#region Resource
		public VisualResource Resource {
			get { return resource; }
			set {
				if (Object.ReferenceEquals(Resource, value))
					return;
				resource = value;
				OnResourceChanged();
			}
		}
		void OnResourceChanged() {
			if (Resource == null)
				return;
			InnerBindingHelper.SetBinding(this, Resource, PrevNavButtonInfoProperty, "PrevNavButtonInfo");
			InnerBindingHelper.SetBinding(this, Resource, NextNavButtonInfoProperty, "NextNavButtonInfo");
		}
		#endregion
		#region ResourceIntervals
		public VisualSimpleResourceIntervalCollection ResourceIntervals {
			get { return resourceIntervals; }
			set {
				if (Object.ReferenceEquals(ResourceIntervals, value))
					return;
				resourceIntervals = value;
				OnResourceIntervalsChanged();
			}
		}
		void OnResourceIntervalsChanged() {
			ResourceIntervalsComponent.ViewModelItems = ResourceIntervals;
		}
		#endregion
		#region PrevNavButtonInfo
		public static readonly DependencyProperty PrevNavButtonInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceComponent, NavigationButtonViewModel>("PrevNavButtonInfo", null, (d, e) => d.OnPrevNavButtonInfoChanged(e.OldValue, e.NewValue));
		void OnPrevNavButtonInfoChanged(NavigationButtonViewModel oldValue, NavigationButtonViewModel newValue) {
			NavigationButtonsComponent.PrevButtonInfo = newValue;
		}
		public NavigationButtonViewModel PrevNavButtonInfo {
			get { return (NavigationButtonViewModel)GetValue(PrevNavButtonInfoProperty); }
			set { SetValue(PrevNavButtonInfoProperty, value); }
		}
		#endregion
		#region NextNavButtonInfo
		public static readonly DependencyProperty NextNavButtonInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceComponent, NavigationButtonViewModel>("NextNavButtonInfo", null, (d, e) => d.OnNextNavButtonInfoChanged(e.OldValue, e.NewValue));
		void OnNextNavButtonInfoChanged(NavigationButtonViewModel oldValue, NavigationButtonViewModel newValue) {
			NavigationButtonsComponent.NextButtonInfo = newValue;
		}
		public NavigationButtonViewModel NextNavButtonInfo {
			get { return (NavigationButtonViewModel)GetValue(NextNavButtonInfoProperty); }
			set { SetValue(NextNavButtonInfoProperty, value); }
		}
		#endregion
		#region CellsPadding
		public Thickness CellsPadding {
			get { return cellsPadding; }
			set {
				if (CellsPadding == value)
					return;
				cellsPadding = value;
				OnPaddingChanged();
			}
		}
		protected virtual void OnPaddingChanged() {
			ResourceIntervalsComponent.CellsPadding = CellsPadding;
		}
		#endregion
		public SimpleIntervalsComponent ResourceIntervalsComponent { get; private set; }
		protected NavigationButtonsComponent NavigationButtonsComponent { get; private set; }
		public override void Initialize() {
			base.Initialize();
			ResourceIntervalsComponent = new SimpleIntervalsComponent(Panel, this);
			VisualItemsAccessor.Add(ResourceIntervalsComponent);
			ResourceIntervalsComponent.Initialize();
			ResourceIntervalsComponent.Orientation = Orientation.Vertical;
			NavigationButtonsComponent = new NavigationButtonsComponent(Panel, this);
			VisualItemsAccessor.Add(NavigationButtonsComponent);
			NavigationButtonsComponent.Initialize();
		}
		protected override void OnVirtualizationModeChanged() {
			base.OnVirtualizationModeChanged();
			ResourceIntervalsComponent.VirtualizationMode = VirtualizationMode;
		}
		protected override Size MeasureCore(Size availableSize) {
			SchedulerItemsControl.SetElementPosition(ResourceIntervalsComponent, OwnElementPosition);
			ResourceIntervalsComponent.Measure(availableSize);
			NavigationButtonsComponent.Measure(availableSize);
			return ResourceIntervalsComponent.DesiredSize;
		}
		protected override Size ArrangeCore(Rect arrangeBounds) {
			ResourceIntervalsComponent.Arrange(arrangeBounds);
			Rect navigationButtonsBounds = arrangeBounds;
			navigationButtonsBounds.X += CellsPadding.Left;
			double buttonsWidth = navigationButtonsBounds.Width - Math.Max(CellsPadding.Right, CalculateRightOffset());
			navigationButtonsBounds.Width = Math.Max(buttonsWidth, 0);
			NavigationButtonsComponent.Arrange(navigationButtonsBounds);
			return ResourceIntervalsComponent.RenderSize;
		}
		double CalculateRightOffset() {
			IVisualElementAccessor<CellContainerVisualComponent> accessor = ResourceIntervalsComponent.VisualItemsAccessor;
			int count = accessor.Count;
			double result = 0;
			for (int i = 0; i < count; i++)
				result = Math.Max(accessor[i].RightOffset, result);
			return result;
		}
		protected override IVisualElementAccessor<IVisualComponent> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new VisualComponentAccessor<IVisualComponent>(panel);
		}
		internal List<Size> GetBaseCellBounds() {
			if (ResourceIntervalsComponent.VisualItemsAccessor.Count < 1)
				return null;
			return ResourceIntervalsComponent.VisualItemsAccessor[0].GetBaseCellBounds();
		}
		void ISupportElementPosition.OnRecalculatePostions(ElementPosition oldValue, ElementPosition newValue) {
			SchedulerItemsControl.SetElementPosition(ResourceIntervalsComponent, newValue);
		}
	}
	#endregion
	#region NavigationButtonsComponent
	public class NavigationButtonsComponent : VisualComponent<VisualNavigationButton> {
		#region Fields
		NavigationButtonViewModel prevButtonInfo;
		NavigationButtonViewModel nextButtonInfo;
		#endregion
		public NavigationButtonsComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		#region PrevButtonInfo
		public NavigationButtonViewModel PrevButtonInfo {
			get { return prevButtonInfo; }
			set {
				if (Object.ReferenceEquals(PrevButtonInfo, value))
					return;
				prevButtonInfo = value;
				OnPrevButtonInfoChanged();
			}
		}
		void OnPrevButtonInfoChanged() {
			if (PrevButton != null) {
				PrevButton.ButtonInfo = PrevButtonInfo;
				RaiseComponentChanged();
			}
		}
		#endregion
		#region NextButtonInfo
		public NavigationButtonViewModel NextButtonInfo {
			get { return nextButtonInfo; }
			set {
				if (Object.ReferenceEquals(NextButtonInfo, value))
					return;
				nextButtonInfo = value;
				OnNextButtonInfoChanged();
			}
		}
		void OnNextButtonInfoChanged() {
			if (NextButton != null) {
				NextButton.ButtonInfo = NextButtonInfo;
				RaiseComponentChanged();
			}
		}
		#endregion
		INavigationButtonsComponentProperties Properties { get { return Panel.GetProperties<INavigationButtonsComponentProperties>(); } }
		protected Style PrevButtonStyle { get { return Properties.NavigationPrevButtonStyle; } set { Properties.NavigationPrevButtonStyle = value; } }
		protected Style NextButtonStyle { get { return Properties.NavigationNextButtonStyle; } set { Properties.NavigationNextButtonStyle = value; } }
		protected VisualNavigationButton PrevButton { get; private set; }
		protected VisualNavigationButton NextButton { get; private set; }
		public override void Initialize() {
			base.Initialize();
			PrevButton = new VisualNavigationButton();
			PrevButton.Owner = this;
			PrevButton.Style = PrevButtonStyle;
			Canvas.SetZIndex(PrevButton, Int16.MaxValue - 1);
			VisualItemsAccessor.Add(PrevButton);
			NextButton = new VisualNavigationButton();
			NextButton.Owner = this;
			NextButton.Style = NextButtonStyle;
			Canvas.SetZIndex(NextButton, Int16.MaxValue - 1);
			VisualItemsAccessor.Add(NextButton);
		}
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			PropertyChangedSubscriber.AddHandler(Properties, OnPropertyChanged);
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			PropertyChangedSubscriber.RemoveHandler(Properties, OnPropertyChanged);
		}
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "NavigationNextButtonStyle")
				NextButton.Style = NextButtonStyle;
			else if (e.PropertyName == "NavigationPrevButtonStyle")
				PrevButton.Style = PrevButtonStyle;
			RaiseComponentChanged();
		}
		protected override Size MeasureCore(Size availableSize) {
			PrevButton.Measure(availableSize);
			NextButton.Measure(availableSize);
			return availableSize;
		}
		protected override Size ArrangeCore(Rect arrangeBounds) {
			double centerY = arrangeBounds.Y + arrangeBounds.Height / 2;
			Point location = arrangeBounds.Location();
			double prevButtonX = location.X;
			double prevButtonY = centerY - PrevButton.DesiredSize.Height / 2;
			Point prevButtonLocation = new Point(prevButtonX, prevButtonY);
			PrevButton.Arrange(new Rect(prevButtonLocation, PrevButton.DesiredSize));
			double nextButtonX = location.X + arrangeBounds.Width - NextButton.DesiredSize.Width;
			double nextButtonY = centerY - NextButton.DesiredSize.Height / 2;
			Point nextButtonLocation = new Point(nextButtonX, nextButtonY);
			NextButton.Arrange(new Rect(nextButtonLocation, NextButton.DesiredSize));
			return arrangeBounds.Size();
		}
		protected override IVisualElementAccessor<VisualNavigationButton> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new PanelChildrenAccessor<VisualNavigationButton>(panel, ZIndex);
		}
	}
	#endregion
	#region SimpleIntervalsComponent
	public class SimpleIntervalsComponent : ItemsBasedComponent<CellContainerVisualComponent, VisualSimpleResourceInterval>, ISupportElementPosition {
		#region Fields
		Thickness cellsPadding;
		#endregion
		public SimpleIntervalsComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		#region CellsPadding
		public Thickness CellsPadding {
			get { return cellsPadding; }
			set {
				if (CellsPadding == value)
					return;
				cellsPadding = value;
				OnPaddingChanged();
			}
		}
		protected virtual void OnPaddingChanged() {
			RaiseComponentChanged();
		}
		#endregion
		protected override IVisualElementAccessor<CellContainerVisualComponent> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new VisualComponentAccessor<CellContainerVisualComponent>(panel);
		}
		protected override IVisualElementGenerator<CellContainerVisualComponent, VisualSimpleResourceInterval> GetItemsGenerator() {
			CellContainerGenerator generator = new CellContainerGenerator(this, VisualItemsAccessor);
			generator.CellsPadding = CellsPadding;
			return generator;
		}
		#region ISupportElementPosition Members
		void ISupportElementPosition.OnRecalculatePostions(ElementPosition oldValue, ElementPosition newValue) {
			if (Object.Equals(oldValue, newValue))
				return;
			InvalidateMeasure();
		}
		#endregion
	}
	#endregion
	#region ResourcesComponent
	public class ResourcesComponent : ItemsBasedComponent<ResourceComponent, VisualResource> {
		#region Fields
		Thickness cellsPadding;
		#endregion
		public ResourcesComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		#region CellsPadding
		public Thickness CellsPadding {
			get { return cellsPadding; }
			set {
				if (CellsPadding == value)
					return;
				cellsPadding = value;
				OnPaddingChanged();
			}
		}
		protected virtual void OnPaddingChanged() {
			RaiseComponentChanged();
		}
		#endregion
		public List<Size> GetBaseCellBounds() {
			if (VisualItemsAccessor.Count < 1)
				return null;
			return VisualItemsAccessor[0].GetBaseCellBounds();
		}
		public virtual void SyncMeasure(double[] resourceHeights) {
			if (ViewModelItems == null)
				return;
			Rect[] childrenBounds = CreateRourceHeaderBounds(resourceHeights);
			int count = ViewModelItems.Count;
			MeasureInProgress = true;
			for (int i = 0; i < count; i++)
				VisualItemsAccessor[i].Measure(childrenBounds[i].Size());
			IsStateValid = true;
			MeasureInProgress = false;
			IncreaseMeasureVersion();
		}
		public virtual double ConvertTimeToPosition(DateTime dateTime) {
			CellsVisualComponent cellsComponent = ObtainFirstCellsComponent();
			if (cellsComponent == null)
				return -1;
			return cellsComponent.ConvertTimeToPosition(dateTime);
		}
		CellsVisualComponent ObtainFirstCellsComponent() {
			if (VisualItemsAccessor.Count <= 0)
				return null;
			ResourceComponent resourceComponent = VisualItemsAccessor[0];
			SimpleIntervalsComponent intervalsComponent = resourceComponent.ResourceIntervalsComponent;
			if (intervalsComponent.VisualItemsAccessor.Count <= 0)
				return null;
			return intervalsComponent.VisualItemsAccessor[0].CellsComponent;
		}
		protected override IVisualElementGenerator<ResourceComponent, VisualResource> GetItemsGenerator() {
			ResourceComponentGenerator generator = new ResourceComponentGenerator(this, VisualItemsAccessor);
			generator.CellsPadding = CellsPadding;
			return generator;
		}
		protected override IVisualElementAccessor<ResourceComponent> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new VisualComponentAccessor<ResourceComponent>(panel);
		}
		Rect[] CreateRourceHeaderBounds(double[] resourceHeights) {
			int count = resourceHeights.Length;
			Rect[] result = new Rect[count];
			double y = 0;
			for (int i = 0; i < count; i++) {
				double height = resourceHeights[i];
				result[i] = new Rect(new Point(0, y), new Size(DesiredSize.Width, height));
				y += height;
			}
			return result;
		}
	}
	#endregion
	#region MoreButtonsComponent
	public class MoreButtonsComponent : ItemsBasedComponent<MoreButton, MoreButtonViewModel>, ICellsComponentChangedListener {
		public MoreButtonsComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		IMoreButtonsComponentProperties Properties { get { return Panel.GetProperties<IMoreButtonsComponentProperties>(); } }
		protected Style Style { get { return Properties.MoreButtonStyle; } }
		internal CellsVisualComponent CellsComponent { get; set; }
		public bool ShowMoreButtons { get { return Properties.ShowMoreButtons; } }
		public override int ZIndex { get { return 600; } }
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			PropertyChangedSubscriber.AddHandler(Properties, OnPropertyChanged);
			Panel.Measuring += OnPanelMeasuring;
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			PropertyChangedSubscriber.RemoveHandler(Properties, OnPropertyChanged);
			Panel.Measuring -= OnPanelMeasuring;
		}
		void OnPanelMeasuring(object sender, EventArgs e) {
		}
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			RaiseComponentChanged();
		}
		protected override void OnVisibilityChanged() {
			if (Visibility == Visibility.Collapsed)
				VisualItemsAccessor.ForEach(element => element.ViewModel = null);
			base.OnVisibilityChanged();
		}
		protected override Rect[] GetBoundsForMeasure(Size size, int count) {
			return GetCellRects();
		}
		protected override Rect[] GetBoundsForArrange(Size size, int count) {
			return GetCellRects();
		}
		Rect[] GetCellRects() {
			return CellsComponent.Bounds;
		}
		protected override void ArrangeChild(MoreButton child, Rect childBounds) {
			childBounds.BottomMiddle();
			Size desiredSize = child.DesiredSize;
			double x = childBounds.X + desiredSize.Width / 2;
			double y = childBounds.Bottom - desiredSize.Height;
			base.ArrangeChild(child, new Rect(new Point(x, y), desiredSize));
		}
		protected override IVisualElementGenerator<MoreButton, MoreButtonViewModel> GetItemsGenerator() {
			return new MoreButtonGenerator(this, VisualItemsAccessor, Style);
		}
		protected override IVisualElementAccessor<MoreButton> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new PanelChildrenAccessor<MoreButton>(panel, ZIndex);
		}
		#region ICellsComponentChangedListener
		void ICellsComponentChangedListener.OnCellsChanged() {
			RaiseComponentChanged();
		}
		#endregion
	}
	#endregion
}
