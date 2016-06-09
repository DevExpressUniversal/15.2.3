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
#if SILVERLIGHT
 using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using System.Collections.Specialized;
#else
using System.Collections.Specialized;
using System.Windows.Input;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Utils;
#endif
using DevExpress.Utils;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Scheduler.Drawing.Components;
using DevExpress.XtraScheduler.Native;
using System.Windows.Documents;
using System.ComponentModel;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region IComponentProperties
	public interface IComponentProperties {
		void AddHandler(Type propertiesType, PropertyChangedEventHandler handler);
		void RemoveHandler(Type propertiesType, PropertyChangedEventHandler handler);
	}
	#endregion
	#region ISelectionComponentProperties
	public interface ISelectionComponentProperties : IComponentProperties {
		ControlTemplate SelectionTemplate { get; set; }
	}
	#endregion
	#region IDraggedAppointmentComponentProperty
	public interface IDraggedAppointmentComponentProperties : IComponentProperties {
		Style DraggedAppointmentStyle { get; set; }
	}
	#endregion
	#region IHorizontalAppointmentComponentProperties
	public interface IHorizontalAppointmentComponentProperties : IComponentProperties {
		AppointmentSnapToCellsMode SnapToCells { get; set; }
		bool AppointmentAutoHeight { get; set; }
		bool IsAppointmentStatusVisible { get; set; }
		int DefaultAppointmentHeight { get; set; }
	}
	#endregion
	#region ICellsComponentProperties
	public interface ICellsComponentProperties : IComponentProperties {
		Style CellStyle { get; set; }
	}
	#endregion
	#region IDateHeaderComponentProperties
	public interface IDateHeaderComponentProperties : IComponentProperties {
		Style DateHeaderStyle { get; set; }
		BaseCellsDateHeaderLevelInfo BaseCellsInfo { get; set; }
	}
	#endregion
	#region IMoreButtonsComponentProperties
	public interface IMoreButtonsComponentProperties : IComponentProperties {
		Style MoreButtonStyle { get; set; }
		bool ShowMoreButtons { get; set; }
	}
	#endregion
	#region INavigationButtonsComponentProperties
	public interface INavigationButtonsComponentProperties : IComponentProperties {
		Style NavigationPrevButtonStyle { get; set; }
		Style NavigationNextButtonStyle { get; set; }
	}
	#endregion
	#region IResourceComponentProperties
	public interface IResourceComponentProperties : IComponentProperties {
		SchedulerScrollBarVisibility ResourceScrollBarVisible { get; set; }
	}
	#endregion
	#region IResourceHeaderComponentProperties
	public interface IResourceHeaderComponentProperties : IComponentProperties {
		Style ResourceHeaderStyle { get; set; }
	}
	#endregion
	#region IResourceNavigatorComponentProperties
	public interface IResourceNavigatorComponentProperties : IComponentProperties {
		ResourceNavigatorVisibility ResourceNavigatorVisibility { get; set; }
		Style ResourceNavigatorStyle { get; set; }
	}
	#endregion
	#region ISelectionBarComponentProperties
	public interface ISelectionBarComponentProperties : IComponentProperties {
		Style SelectionBarCellStyle { get; set; }
	}
	#endregion
	#region ITimelineViewComponentProperties
	public interface ITimelineViewComponentProperties : ISelectionComponentProperties, IHorizontalAppointmentComponentProperties, ICellsComponentProperties, INavigationButtonsComponentProperties, IMoreButtonsComponentProperties, IDraggedAppointmentComponentProperties, IResourceComponentProperties, IDateHeaderComponentProperties, ISelectionBarComponentProperties {
	}
	#endregion
	#region ITimelineViewGroupByDateComponentProperties
	public interface ITimelineViewGroupByDateComponentProperties : ITimelineViewComponentProperties, IResourceHeaderComponentProperties, IResourceNavigatorComponentProperties {
	}
	#endregion
	#region IConvertible<T>
	public interface IConvertible<T> {
		T Convert();
	}
	#endregion
	#region ILayoutPanel
	public interface ILayoutPanel : IConvertible<Panel>, IUIElementCollectionChangedListener {
		event RoutedEventHandler Loaded;
		event RoutedEventHandler Unloaded;
		event EventHandler Measuring;
		UIElementCollection Children { get; }
		bool IsMeasureActual { get; }
		T GetProperties<T>() where T : class, IComponentProperties;
		int CalculateFirstChildIndexFromZIndex(int zIndex);
	}
	#endregion
	#region PropertyChangedSubscriber
	public static class PropertyChangedSubscriber {
		public static void AddHandler(IHorizontalAppointmentComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.AddHandler(typeof(IHorizontalAppointmentComponentProperties), handler);
		}
		public static void AddHandler(ISelectionComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.AddHandler(typeof(ISelectionComponentProperties), handler);
		}
		public static void AddHandler(ICellsComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.AddHandler(typeof(ICellsComponentProperties), handler);
		}
		public static void AddHandler(INavigationButtonsComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.AddHandler(typeof(INavigationButtonsComponentProperties), handler);
		}
		public static void AddHandler(IMoreButtonsComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.AddHandler(typeof(IMoreButtonsComponentProperties), handler);
		}
		public static void AddHandler(IDraggedAppointmentComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.AddHandler(typeof(IDraggedAppointmentComponentProperties), handler);
		}
		public static void AddHandler(IResourceComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.AddHandler(typeof(IResourceComponentProperties), handler);
		}
		public static void AddHandler(IDateHeaderComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.AddHandler(typeof(IDateHeaderComponentProperties), handler);
		}
		public static void AddHandler(IResourceHeaderComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.AddHandler(typeof(IResourceHeaderComponentProperties), handler);
		}
		public static void AddHandler(IResourceNavigatorComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.AddHandler(typeof(IResourceNavigatorComponentProperties), handler);
		}
		public static void AddHandler(ISelectionBarComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.AddHandler(typeof(ISelectionBarComponentProperties), handler);
		}
		public static void RemoveHandler(IHorizontalAppointmentComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.RemoveHandler(typeof(IHorizontalAppointmentComponentProperties), handler);
		}
		public static void RemoveHandler(ISelectionComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.RemoveHandler(typeof(ISelectionComponentProperties), handler);
		}
		public static void RemoveHandler(ICellsComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.RemoveHandler(typeof(ICellsComponentProperties), handler);
		}
		public static void RemoveHandler(INavigationButtonsComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.RemoveHandler(typeof(INavigationButtonsComponentProperties), handler);
		}
		public static void RemoveHandler(IMoreButtonsComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.RemoveHandler(typeof(IMoreButtonsComponentProperties), handler);
		}
		public static void RemoveHandler(IDraggedAppointmentComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.RemoveHandler(typeof(IDraggedAppointmentComponentProperties), handler);
		}
		public static void RemoveHandler(IResourceComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.RemoveHandler(typeof(IResourceComponentProperties), handler);
		}
		public static void RemoveHandler(IDateHeaderComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.RemoveHandler(typeof(IDateHeaderComponentProperties), handler);
		}
		public static void RemoveHandler(IResourceHeaderComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.RemoveHandler(typeof(IResourceHeaderComponentProperties), handler);
		}
		public static void RemoveHandler(IResourceNavigatorComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.RemoveHandler(typeof(IResourceNavigatorComponentProperties), handler);
		}
		public static void RemoveHandler(ISelectionBarComponentProperties properties, PropertyChangedEventHandler handler) {
			properties.RemoveHandler(typeof(ISelectionBarComponentProperties), handler);
		}
	}
	#endregion
	#region AppointmentsLayoutPanel
	public class AppointmentsLayoutPanel : SchedulerPanel, IVisualElement, ILayoutPanel {
		readonly ILayoutPanel owner;
		public AppointmentsLayoutPanel(ILayoutPanel owner) {
			this.owner = owner;
		}
		public HorizontalAppointmentsComponent AppointmentsComponent { get; set; }
		public MoreButtonsComponent MoreButtonsComponent { get; set; }
		public bool IsMeasureActual { get; private set; }
		protected override Size MeasureOverrideCore(Size availableSize) {
			IsMeasureActual = false;
			AppointmentsComponent.Measure(availableSize);
			IsMeasureActual = true;
			InvalidateMoreButtons();
			return AppointmentsComponent.DesiredSize;
		}
		void InvalidateMoreButtons() {
			if (MoreButtonsComponent == null)
				return;
			MoreButtonsComponent.InvalidateMeasure();
		}
		protected override Size ArrangeOverrideCore(Size finalSize) {
			Rect arrangeRect = new Rect(new Point(0, 0), finalSize);
			AppointmentsComponent.Arrange(arrangeRect);
			return AppointmentsComponent.RenderSize;
		}
		public T GetProperties<T>() where T : class, IComponentProperties {
			return this.owner.GetProperties<T>();
		}
		Panel IConvertible<Panel>.Convert() {
			return this;
		}
		void IUIElementCollectionChangedListener.OnElementInserted(object sender, int index) {
		}
		void IUIElementCollectionChangedListener.OnElementRemoved(object sender, int index) {
		}
		int ILayoutPanel.CalculateFirstChildIndexFromZIndex(int zIndex) {
			List<IVisualComponent> list = new List<IVisualComponent>();
			if (AppointmentsComponent != null)
				list.Add(AppointmentsComponent);
			int index = VisualComponentHelper.FindAndSort(list, zIndex);
			return (index >= 0) ? index : Children.Count;
		}
	}
	#endregion
	public static class VisualComponentHelper {
		public static int FindAndSort(List<IVisualComponent> components, int zIndex) {
			int count = components.Count;
			if (count == 0)
				return 0;
			components.Sort(Compare);
			for (int i = 0; i < count; i++) {
				IVisualComponent component = components[i];
				if (component.ZIndex > zIndex) {
					return component.FirstChildIndex;
				}
			}
			return -1;
		}
		static int Compare(IVisualComponent one, IVisualComponent two) {
			int result = one.ZIndex - two.ZIndex;
			if (result == 0) {
				return one.FirstChildIndex - two.FirstChildIndex;
			}
			return result;
		}
	}
	#region SchedulerViewLayoutPanelBase (abstract class)
	public abstract class SchedulerViewLayoutPanelBase : SchedulerPanel, ILayoutPanel, IVisualComponent, IDisposable, ISupportCheckIntegrity {
		const int MinPanelHeightOnInfinity = 100;
		protected SchedulerViewLayoutPanelBase() {
			Components = new VisualComponentAccessor<IVisualComponent>(this);
			this.Unloaded += OnUnloaded;
		}
		#region Properties
		#region ViewInfo
		public VisualViewInfoBase ViewInfo {
			get { return (VisualViewInfoBase)GetValue(ViewInfoProperty); }
			set { SetValue(ViewInfoProperty, value); }
		}
		public static readonly DependencyProperty ViewInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewLayoutPanelBase, VisualViewInfoBase>("ViewInfo", null, (d, e) => d.OnViewInfoChanged(e.OldValue, e.NewValue), null);
		void OnViewInfoChanged(VisualViewInfoBase oldValue, VisualViewInfoBase newValue) {
			if (oldValue != null)
				UnlinkFromViewInfo(oldValue);
			if (newValue != null)
				LinkToViewInfo(newValue);
		}
		#endregion
		#region VirtualizationMode
		public VirtualizationMode VirtualizationMode {
			get { return (VirtualizationMode)GetValue(VirtualizationModeProperty); }
			set { SetValue(VirtualizationModeProperty, value); }
		}
		public static readonly DependencyProperty VirtualizationModeProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewLayoutPanelBase, VirtualizationMode>("VirtualizationMode", VirtualizationMode.Standard, (d, e) => d.OnVirtualizationModeChanged(e.OldValue, e.NewValue));
		void OnVirtualizationModeChanged(VirtualizationMode oldValue, VirtualizationMode newValue) {
			Components.ForEach(component => component.VirtualizationMode = newValue);
		}
		#endregion
		public bool IsMeasureActual {
			get;
			private set;
		}
		protected VisualComponentAccessor<IVisualComponent> Components { get; private set; }
		#endregion
		public void Initialize() { 
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			Components.ForEach(component => component.InvalidateMeasure());
		}
		protected override Size MeasureOverrideCore(Size availableSize) {
			IsMeasureActual = false;
			base.MeasureOverrideCore(availableSize);
			if (ViewInfo == null) {
				IsMeasureActual = true;
				return Size.Empty;
			}
			if (double.IsInfinity(availableSize.Height) && double.IsInfinity(availableSize.Width)) {
				double minPanelHeight = CalculateMinPanelHeight();
				return new Size(0, minPanelHeight);
			}
			MeasureComponents(availableSize);
			IsMeasureActual = true;
			double width = double.IsInfinity(availableSize.Width) ? GetDesiredWidth() : availableSize.Width;
			double height = double.IsInfinity(availableSize.Height) ? GetDesiredHeight() : availableSize.Height;
			return new Size(width, height);
		}
		protected virtual double CalculateMinPanelHeight() {
			return MinPanelHeightOnInfinity;
		}
		protected virtual double GetDesiredWidth() {
			return 0;
		}
		protected virtual double GetDesiredHeight() {
			return 0;
		}
		protected override void SizeChangedOnArrange() {
			if (!IsMeasureActual)
				return;
			InvalidateMeasure();
			IsMeasureActual = false;
			base.SizeChangedOnArrange();
		}
		protected override Size ArrangeOverrideCore(Size finalSize) {
			base.ArrangeOverrideCore(finalSize);
			if (ViewInfo == null)
				return Size.Empty;
			if (!IsMeasureActual) {
				MeasureComponents(finalSize);
				IsMeasureActual = true;
			}
			ArrangeComponents(finalSize);
			return finalSize;
		}
		protected void UpdateChild(UIElement removedChild, UIElement insertedChild) {
			if (removedChild != null)
				RemoveChild(removedChild);
			if (insertedChild != null)
				InsertChild(insertedChild);
		}
		void InsertChild(UIElement element) {
			Children.Add(element);
			UIElementCollectionChangedNotifier.OnElementInserted(this, this, Children.Count - 1);
		}
		void RemoveChild(UIElement element) {
			int index = Children.IndexOf(element);
			Children.RemoveAt(index);
			UIElementCollectionChangedNotifier.OnElementRemoved(this, this, index);
		}
		protected void RegisterComponent(IVisualComponent component) {
			Components.Add(component);
		}
		public virtual void OnChildrenChanged(IVisualElement child) {
			if (!IsMeasureActual)
				return;
			InvalidateMeasure();
			IsMeasureActual = false;
		}
		public virtual void OnElementInserted(object sender, int index) {
			UIElementCollectionChangedNotifier.OnElementInserted(Components, sender, index);
		}
		public virtual void OnElementRemoved(object sender, int index) {
			UIElementCollectionChangedNotifier.OnElementRemoved(Components, sender, index);
		}
		public T GetProperties<T>() where T : class, IComponentProperties {
			return this as T;
		}
		protected abstract void LinkToViewInfo(VisualViewInfoBase viewInfo);
		protected abstract void UnlinkFromViewInfo(VisualViewInfoBase oldValue);
		protected abstract void MeasureComponents(Size availableSize);
		protected abstract void ArrangeComponents(Size finalSize);
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerViewLayoutPanelBase() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		#endregion
		#region IConvertible<Panel> Members
		Panel IConvertible<Panel>.Convert() {
			return this;
		}
		#endregion
		#region IEnumerable<IVisualElement> Members
		IEnumerator<IVisualElement> IEnumerable<IVisualElement>.GetEnumerator() {
			int count = Components.Count;
			for (int i = 0; i < count; i++)
				yield return Components[i];
		}
		#endregion
		#region IEnumerator Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((IEnumerable<IVisualElement>)this).GetEnumerator();
		}
		#endregion
		int IVisualComponent.ZIndex { get { return 0; } }
		int IVisualComponent.FirstChildIndex { get { return 0; } }
		List<IVisualComponent> IVisualComponent.GetComponents() {
			List<IVisualComponent> result = new List<IVisualComponent>();
			int count = Components.Count;
			for (int i = 0; i < count; i++) {
				IVisualComponent component = Components[i];
				SchedulerAssert.CheckIntegrity(component);
				List<IVisualComponent> childComponents = component.GetComponents();
				if (childComponents.Count <= 0)
					result.Add(component);
				else
					result.AddRange(childComponents);
			}
			SchedulerAssert.CheckIntegrity(result);
			return result;
		}
		int ILayoutPanel.CalculateFirstChildIndexFromZIndex(int zIndex) {
			List<IVisualComponent> components = ((IVisualComponent)this).GetComponents();
			int index = VisualComponentHelper.FindAndSort(components, zIndex);
			return (index >= 0) ? index : Children.Count;
		}
		void ISupportCheckIntegrity.CheckIntegrity() {
			SchedulerAssert.CheckIntegrity(((IVisualComponent)this).GetComponents());
		}
	}
	#endregion
	#region TimelineViewLayoutPanel (abstract class)
	public abstract class TimelineViewLayoutPanel : SchedulerViewLayoutPanelBase, ITimelineViewComponentProperties {
		protected const int DefaultSelectionBarHeight = 20;
		protected TimelineViewLayoutPanel() {
			HandlerList = new EventHandlerList();
			BaseCellsInfo = new BaseCellsDateHeaderLevelInfo();
			CreateComponents();
		}
		#region Properties
		public DateHeaderComponent DateHeader { get; private set; }
		public SelectionBarComponent SelectionBar { get; private set; }
		public ResourcesComponent VisualResources { get; private set; }
		public BaseCellsDateHeaderLevelInfo BaseCellsInfo { get; set; }
		public VerticalTimeIndicatorContainerComponent TimeIndicatorContainer { get; private set; }
		#region DateHeaderTopLevelElementPosition
		public static readonly DependencyProperty DateHeaderTopLevelElementPositionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, ElementPosition>("DateHeaderTopLevelElementPosition", ElementPosition.Standalone, (d, e) => d.OnDateHeaderTopLevelElementPositionChanged(e.OldValue, e.NewValue));
		public ElementPosition DateHeaderTopLevelElementPosition { get { return (ElementPosition)GetValue(DateHeaderTopLevelElementPositionProperty); } set { SetValue(DateHeaderTopLevelElementPositionProperty, value); } }
		public void OnDateHeaderTopLevelElementPositionChanged(ElementPosition oldValue, ElementPosition newValue) {
			if (DateHeader != null)
				DateHeader.TopLevelElementPosition = newValue;
		}
		#endregion
		#region DateHeaderLevelElementPosition
		public static readonly DependencyProperty DateHeaderLevelElementPositionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, ElementPosition>("DateHeaderLevelElementPosition", ElementPosition.Standalone, (d, e) => d.DateHeaderLevelElementPositionChanged(e.OldValue, e.NewValue));
		public ElementPosition DateHeaderLevelElementPosition { get { return (ElementPosition)GetValue(DateHeaderLevelElementPositionProperty); } set { SetValue(DateHeaderLevelElementPositionProperty, value); } }
		public void DateHeaderLevelElementPositionChanged(ElementPosition oldValue, ElementPosition newValue) {
			if (DateHeader != null)
				DateHeader.LevelElementPosition = newValue;
		}
		#endregion
		#region SelectionTemplate
		public ControlTemplate SelectionTemplate {
			get { return (ControlTemplate)GetValue(SelectionTemplateProperty); }
			set { SetValue(SelectionTemplateProperty, value); }
		}
		public static readonly DependencyProperty SelectionTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, ControlTemplate>("SelectionTemplate", null, (d, e) => d.OnSelectionTemplateChanged(e.OldValue, e.NewValue), null);
		void OnSelectionTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			NotifyPropertyChanged(typeof(ISelectionComponentProperties), "SelectionTemplate");
		}
		#endregion
		#region CellStyle
		public Style CellStyle {
			get { return (Style)GetValue(CellStyleProperty); }
			set { SetValue(CellStyleProperty, value); }
		}
		public static readonly DependencyProperty CellStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, Style>("CellStyle", null, (d, e) => d.OnCellStyleChanged(e.OldValue, e.NewValue), null);
		void OnCellStyleChanged(Style oldValue, Style newValue) {
			NotifyPropertyChanged(typeof(ICellsComponentProperties), "CellStyle");
		}
		#endregion
		#region DateHeaderStyle
		public Style DateHeaderStyle {
			get { return (Style)GetValue(DateHeaderStyleProperty); }
			set { SetValue(DateHeaderStyleProperty, value); }
		}
		public static readonly DependencyProperty DateHeaderStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, Style>("DateHeaderStyle", default(Style), (d, e) => d.OnDateHeaderStyleChanged(e.OldValue, e.NewValue));
		void OnDateHeaderStyleChanged(Style oldValue, Style newValue) {
			NotifyPropertyChanged(typeof(IDateHeaderComponentProperties), "DateHeaderStyle");
		}
		#endregion
		#region MoreButtonStyle
		public Style MoreButtonStyle {
			get { return (Style)GetValue(MoreButtonStyleProperty); }
			set { SetValue(MoreButtonStyleProperty, value); }
		}
		public static readonly DependencyProperty MoreButtonStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, Style>("MoreButtonStyle", null, (d, e) => d.OnMoreButtonStyleChanged(e.OldValue, e.NewValue));
		void OnMoreButtonStyleChanged(Style oldValue, Style newValue) {
			NotifyPropertyChanged(typeof(IMoreButtonsComponentProperties), "MoreButtonStyle");
		}
		#endregion
		#region ResourceScrollBarVisible
		public SchedulerScrollBarVisibility ResourceScrollBarVisible {
			get { return (SchedulerScrollBarVisibility)GetValue(ResourceScrollBarVisibleProperty); }
			set { SetValue(ResourceScrollBarVisibleProperty, value); }
		}
		public static readonly DependencyProperty ResourceScrollBarVisibleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, SchedulerScrollBarVisibility>("ResourceScrollBarVisible", SchedulerScrollBarVisibility.Never, (d, e) => d.OnResourceScrollBarVisibleChanged(e.OldValue, e.NewValue), null);
		void OnResourceScrollBarVisibleChanged(SchedulerScrollBarVisibility oldValue, SchedulerScrollBarVisibility newValue) {
			NotifyPropertyChanged(typeof(IResourceComponentProperties), "ResourceScrollBarVisible");
		}
		#endregion
		#region DateTimeScrollbarVisible
		public bool DateTimeScrollbarVisible {
			get { return (bool)GetValue(DateTimeScrollbarVisibleProperty); }
			set { SetValue(DateTimeScrollbarVisibleProperty, value); }
		}
		public static readonly DependencyProperty DateTimeScrollbarVisibleProperty = CreateDateTimeScrollbarVisibleProperty();
		static DependencyProperty CreateDateTimeScrollbarVisibleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, bool>("DateTimeScrollbarVisible", true, FrameworkPropertyMetadataOptions.AffectsMeasure);
		}
		#endregion
		#region ShowMoreButtons
		public bool ShowMoreButtons {
			get { return (bool)GetValue(ShowMoreButtonsProperty); }
			set { SetValue(ShowMoreButtonsProperty, value); }
		}
		public static readonly DependencyProperty ShowMoreButtonsProperty = CreateShowMoreButtonsProperty();
		static DependencyProperty CreateShowMoreButtonsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, bool>("ShowMoreButtons", true, (d, e) => d.OnShowMoreButtonsChanged(e.OldValue, e.NewValue), null);
		}
		void OnShowMoreButtonsChanged(bool oldValue, bool newValue) {
			NotifyPropertyChanged(typeof(IMoreButtonsComponentProperties), "ResourceScrollBarVisible");
		}
		#endregion
		#region TimeCellElementPosition
		public ElementPosition TimeCellElementPosition {
			get { return (ElementPosition)GetValue(TimeCellElementPositionProperty); }
			set { SetValue(TimeCellElementPositionProperty, value); }
		}
		public static readonly DependencyProperty TimeCellElementPositionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, ElementPosition>("TimeCellElementPosition", ElementPosition.Standalone, (d, e) => d.OnTimeCellElementPositionChanged(e.OldValue, e.NewValue), null);
		void OnTimeCellElementPositionChanged(ElementPosition oldValue, ElementPosition newValue) {
			VisualResources.OwnElementPosition = newValue;
		}
		#endregion
		#region SelectionBarHeight
		public int SelectionBarHeight {
			get { return (int)GetValue(SelectionBarHeightProperty); }
			set { SetValue(SelectionBarHeightProperty, value); }
		}
		public static readonly DependencyProperty SelectionBarHeightProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, int>("SelectionBarHeight", 0, (d, e) => d.OnSelectionBarHeightChanged(e.OldValue, e.NewValue), null);
		void OnSelectionBarHeightChanged(int oldValue, int newValue) {
			SelectionBar.InvalidateMeasure();
		}
		#endregion
		#region SelectionBarVisible
		public bool SelectionBarVisible {
			get { return (bool)GetValue(SelectionBarVisibleProperty); }
			set { SetValue(SelectionBarVisibleProperty, value); }
		}
		public static readonly DependencyProperty SelectionBarVisibleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, bool>("SelectionBarVisible", false, (d, e) => d.OnSelectionBarVisibleChanged(e.OldValue, e.NewValue));
		void OnSelectionBarVisibleChanged(bool oldValue, bool newValue) {
			SelectionBar.InvalidateMeasure();
		}
		#endregion
		#region TimeIndicatorVisibility
		public TimeIndicatorVisibility TimeIndicatorVisibility {
			get { return (TimeIndicatorVisibility)GetValue(TimeIndicatorVisibilityProperty); }
			set { SetValue(TimeIndicatorVisibilityProperty, value); }
		}
		public static readonly DependencyProperty TimeIndicatorVisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, TimeIndicatorVisibility>("TimeIndicatorVisibility", TimeIndicatorVisibility.TodayView, (d, e) => d.OnTimeIndicatorVisibilityChanged(e.OldValue, e.NewValue));
		void OnTimeIndicatorVisibilityChanged(TimeIndicatorVisibility oldValue, TimeIndicatorVisibility newValue) {
			TimeIndicatorContainer.TimeIndicatorVisibility = newValue;
		}
		#endregion
		#region SelectionBarCellStyle
		public Style SelectionBarCellStyle {
			get { return (Style)GetValue(SelectionBarCellStyleProperty); }
			set { SetValue(SelectionBarCellStyleProperty, value); }
		}
		public static readonly DependencyProperty SelectionBarCellStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, Style>("SelectionBarCellStyle", null, (d, e) => d.OnSelectionBarCellStyleChanged(e.OldValue, e.NewValue));
		void OnSelectionBarCellStyleChanged(Style oldValue, Style newValue) {
			NotifyPropertyChanged(typeof(ISelectionBarComponentProperties), "SelectionBarCellStyle");
		}
		#endregion
		#region SelectionBarElementPosition
		public ElementPosition SelectionBarElementPosition {
			get { return (ElementPosition)GetValue(SelectionBarElementPositionProperty); }
			set { SetValue(SelectionBarElementPositionProperty, value); }
		}
		public static readonly DependencyProperty SelectionBarElementPositionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, ElementPosition>("SelectionBarElementPosition", null, (d, e) => d.OnSelectionBarElementPositionChanged(e.OldValue, e.NewValue), null);
		void OnSelectionBarElementPositionChanged(ElementPosition oldValue, ElementPosition newValue) {
			SelectionBar.OwnElementPosition = newValue;
		}
		#endregion
		#region NavigationPrevButtonStyle
		public Style NavigationPrevButtonStyle {
			get { return (Style)GetValue(NavigationPrevButtonStyleProperty); }
			set { SetValue(NavigationPrevButtonStyleProperty, value); }
		}
		public static readonly DependencyProperty NavigationPrevButtonStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, Style>("NavigationPrevButtonStyle", null, (d, e) => d.OnNavigationPrevButtonStyleChanged(e.OldValue, e.NewValue), null);
		void OnNavigationPrevButtonStyleChanged(Style oldValue, Style newValue) {
			NotifyPropertyChanged(typeof(INavigationButtonsComponentProperties), "NavigationPrevButtonStyle");
		}
		#endregion
		#region NavigationNextButtonStyle
		public Style NavigationNextButtonStyle {
			get { return (Style)GetValue(NavigationNextButtonStyleProperty); }
			set { SetValue(NavigationNextButtonStyleProperty, value); }
		}
		public static readonly DependencyProperty NavigationNextButtonStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, Style>("NavigationNextButtonStyle", null, (d, e) => d.OnNavigationNextButtonStyleChanged(e.OldValue, e.NewValue), null);
		void OnNavigationNextButtonStyleChanged(Style oldValue, Style newValue) {
			NotifyPropertyChanged(typeof(INavigationButtonsComponentProperties), "NavigationNextButtonStyle");
		}
		#endregion
		#region DraggedAppointmentStyle
		public Style DraggedAppointmentStyle {
			get { return (Style)GetValue(DraggedAppointmentStyleProperty); }
			set { SetValue(DraggedAppointmentStyleProperty, value); }
		}
		public static readonly DependencyProperty DraggedAppointmentStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, Style>("DraggedAppointmentStyle", null, (d, e) => d.OnDraggedAppointmentStyleChanged(e.OldValue, e.NewValue), null);
		void OnDraggedAppointmentStyleChanged(Style oldValue, Style newValue) {
			NotifyPropertyChanged(typeof(IDraggedAppointmentComponentProperties), "DraggedAppointmentStyle");
		}
		#endregion
		#region DateTimeScrollBarAreaTemplate
		public ControlTemplate DateTimeScrollBarAreaTemplate {
			get { return (ControlTemplate)GetValue(DateTimeScrollBarAreaTemplateProperty); }
			set { SetValue(DateTimeScrollBarAreaTemplateProperty, value); }
		}
		public static readonly DependencyProperty DateTimeScrollBarAreaTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, ControlTemplate>("DateTimeScrollBarAreaTemplate", null, (d, e) => d.OnDateTimeScrollBarAreaTemplateChanged(e.OldValue, e.NewValue), null);
		void OnDateTimeScrollBarAreaTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			if (newValue == null && DateTimeScrollBarArea != null) {
				UpdateChild(DateTimeScrollBarArea, null);
				return;
			}
			if (DateTimeScrollBarArea == null) {
				DateTimeScrollBarArea = new SchedulerAreaControl();
				UpdateChild(null, DateTimeScrollBarArea);
			}
			DateTimeScrollBarArea.Template = newValue;
		}
		#endregion
		#region AdornedBorderStyle
		public Style AdornedBorderStyle {
			get { return (Style)GetValue(AdornedBorderStyleProperty); }
			set { SetValue(AdornedBorderStyleProperty, value); }
		}
		public static readonly DependencyProperty AdornedBorderStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, Style>("AdornedBorderStyle", null, (d, e) => d.OnAdornedBorderStyleChanged(e.OldValue, e.NewValue));
		void OnAdornedBorderStyleChanged(Style oldValue, Style newValue) {
			if (newValue == null && AdornedBorder != null) {
				UpdateChild(AdornedBorder, null);
				return;
			}
			if (AdornedBorder == null) {
				AdornedBorder = new Border();
				Canvas.SetZIndex(AdornedBorder, 1);
				UpdateChild(null, AdornedBorder);
			}
			AdornedBorder.Style = newValue;
			VisualResources.CellsPadding = new Thickness(0, 0, AdornedBorder.BorderThickness.Right, 0);
		}
		#endregion
		#region SnapToCells
		public AppointmentSnapToCellsMode SnapToCells {
			get { return (AppointmentSnapToCellsMode)GetValue(SnapToCellsProperty); }
			set { SetValue(SnapToCellsProperty, value); }
		}
		public static readonly DependencyProperty SnapToCellsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, AppointmentSnapToCellsMode>("SnapToCells", AppointmentSnapToCellsMode.Auto, (d, e) => d.OnSnapToCellsChanged(e.OldValue, e.NewValue), null);
		void OnSnapToCellsChanged(AppointmentSnapToCellsMode oldValue, AppointmentSnapToCellsMode newValue) {
			NotifyPropertyChanged(typeof(IHorizontalAppointmentComponentProperties), "SnapToCells");
		}
		#endregion
		#region AppointmentAutoHeight
		public bool AppointmentAutoHeight {
			get { return (bool)GetValue(AppointmentAutoHeightProperty); }
			set { SetValue(AppointmentAutoHeightProperty, value); }
		}
		public static readonly DependencyProperty AppointmentAutoHeightProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, bool>(PanelPropertyNames.AppointmentAutoHeightPropertyName, default(bool), (d, e) => d.OnAppointmentAutoHeightChanged(e.OldValue, e.NewValue));
		void OnAppointmentAutoHeightChanged(bool oldValue, bool newValue) {
			NotifyPropertyChanged(typeof(IHorizontalAppointmentComponentProperties), PanelPropertyNames.AppointmentAutoHeightPropertyName);
		}
		#endregion
		#region IsAppointmentStatusVisible
		public bool IsAppointmentStatusVisible {
			get { return (bool)GetValue(IsAppointmentStatusVisibleProperty); }
			set { SetValue(IsAppointmentStatusVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsAppointmentStatusVisibleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, bool>("IsAppointmentStatusVisible", false, (d, e) => d.OnIsAppointmentStatusVisibleChanged(e.OldValue, e.NewValue));
		void OnIsAppointmentStatusVisibleChanged(bool oldValue, bool newValue) {
			NotifyPropertyChanged(typeof(IHorizontalAppointmentComponentProperties), "IsAppointmentStatusVisible");
		}
		#endregion
		#region DefaultAppointmentHeight
		public int DefaultAppointmentHeight {
			get { return (int)GetValue(DefaultAppointmentHeightProperty); }
			set { SetValue(DefaultAppointmentHeightProperty, value); }
		}
		public static readonly DependencyProperty DefaultAppointmentHeightProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, int>("DefaultAppointmentHeight", 22, (d, e) => d.OnDefaultAppointmentHeightChanged(e.OldValue, e.NewValue));
		void OnDefaultAppointmentHeightChanged(int oldValue, int newValue) {
			NotifyPropertyChanged(typeof(IHorizontalAppointmentComponentProperties), "DefaultAppointmentHeight");
		}
		#endregion
		#region HeaderLevels
		public VisualTimeScaleHeaderLevelCollection HeaderLevels {
			get { return (VisualTimeScaleHeaderLevelCollection)GetValue(HeaderLevelsProperty); }
			set { SetValue(HeaderLevelsProperty, value); }
		}
		public static readonly DependencyProperty HeaderLevelsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, VisualTimeScaleHeaderLevelCollection>("HeaderLevels", default(VisualTimeScaleHeaderLevelCollection), (d, e) => d.OnHeaderLevelsChanged(e.OldValue, e.NewValue));
		void OnHeaderLevelsChanged(VisualTimeScaleHeaderLevelCollection oldValue, VisualTimeScaleHeaderLevelCollection newValue) {
			if (DateHeader != null)
				DateHeader.ViewModelItems = newValue;
		}
		#endregion
		#region SelectionBarCells
		public VisualTimeCellContentCollection SelectionBarCells {
			get { return (VisualTimeCellContentCollection)GetValue(SelectionBarCellsProperty); }
			set { SetValue(SelectionBarCellsProperty, value); }
		}
		public static readonly DependencyProperty SelectionBarCellsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, VisualTimeCellContentCollection>("SelectionBarCells", default(VisualTimeCellContentCollection), (d, e) => d.OnSelectionBarCellsChanged(e.OldValue, e.NewValue));
		void OnSelectionBarCellsChanged(VisualTimeCellContentCollection oldValue, VisualTimeCellContentCollection newValue) {
			if (SelectionBar != null)
				SelectionBar.ViewModelItems = newValue;
		}
		#endregion
		#region ResourceContainers
		public VisualResourcesCollection ResourceContainers {
			get { return (VisualResourcesCollection)GetValue(ResourceContainersProperty); }
			set { SetValue(ResourceContainersProperty, value); }
		}
		public static readonly DependencyProperty ResourceContainersProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewLayoutPanel, VisualResourcesCollection>("ResourceContainers", default(VisualResourcesCollection), (d, e) => d.OnResourceContainersChanged(e.OldValue, e.NewValue));
		protected virtual void OnResourceContainersChanged(VisualResourcesCollection oldValue, VisualResourcesCollection newValue) {
			VisualResources.ViewModelItems = newValue;
		}
		#endregion
		protected Control DateTimeScrollBarArea { get; set; }
		protected Border AdornedBorder { get; set; }
		protected EventHandlerList HandlerList { get; private set; }
		VisualTimelineViewGroupBase TimelineViewInfo { get { return (VisualTimelineViewGroupBase)ViewInfo; } }
		TimelineView View { get { return ViewInfo != null ? (TimelineView)ViewInfo.View : null; } }
		#endregion
		protected virtual void CreateComponents() {
			DateHeader = new DateHeaderComponent(this, this);
			DateHeader.Initialize();
			DateHeader.Orientation = Orientation.Vertical;
			RegisterComponent(DateHeader);
			SelectionBar = new SelectionBarComponent(this, this);
			SelectionBar.Initialize();
			SelectionBar.Orientation = Orientation.Horizontal;
			RegisterComponent(SelectionBar);
			VisualResources = new ResourcesComponent(this, this);
			VisualResources.Initialize();
			VisualResources.Orientation = Orientation.Vertical;
			RegisterComponent(VisualResources);
			TimeIndicatorContainer = new VerticalTimeIndicatorContainerComponent(this, this);
			TimeIndicatorContainer.Initialize();
			TimeIndicatorContainer.Resources = VisualResources;
			RegisterComponent(TimeIndicatorContainer);
		}
		protected override void LinkToViewInfo(VisualViewInfoBase viewInfo) {
			VisualTimelineViewGroupBase timelineViewInfo = viewInfo as VisualTimelineViewGroupBase;
			if (timelineViewInfo == null)
				return;
			LinkDateHeaderProperties(timelineViewInfo);
			LinkSelectionBarProperties(timelineViewInfo);
			LinkVisualResourcesProperties(timelineViewInfo);
		}
		protected virtual void LinkDateHeaderProperties(VisualTimelineViewGroupBase viewInfo) {
			InnerBindingHelper.SetBinding(this, viewInfo.Header, TimelineViewLayoutPanel.HeaderLevelsProperty, VisualTimelineHeader.HeaderLevelsPropertyName);
			InnerBindingHelper.SetBinding(this, ViewInfo.View, TimelineViewLayoutPanel.DateHeaderStyleProperty, TimelineView.DateHeaderStylePropertyName);
		}
		protected virtual void LinkVisualResourcesProperties(VisualTimelineViewGroupBase viewInfo) {
			InnerBindingHelper.SetBinding(this, ViewInfo, TimelineViewLayoutPanel.ResourceContainersProperty, VisualTimelineViewGroupBase.ResourceContainersPropertyName);
		}
		protected virtual void LinkSelectionBarProperties(VisualTimelineViewGroupBase viewInfo) {
			InnerBindingHelper.SetBinding(this, viewInfo.SelectionBarContainer, TimelineViewLayoutPanel.SelectionBarCellsProperty, VisualTimelineSelectionBar.CellsPropertyName);
		}
		protected override void UnlinkFromViewInfo(VisualViewInfoBase oldValue) {
			UnlinkDateHeaderProperties();
			UnlinkSelectionBarProperties();
			UnlinkVisualResourcesProperties();
		}
		protected virtual void UnlinkDateHeaderProperties() {
			InnerBindingHelper.ClearBinding(this, TimelineViewLayoutPanel.HeaderLevelsProperty);
			InnerBindingHelper.ClearBinding(this, TimelineViewLayoutPanel.DateHeaderStyleProperty);
		}
		protected virtual void UnlinkVisualResourcesProperties() {
			InnerBindingHelper.ClearBinding(this, TimelineViewLayoutPanel.ResourceContainersProperty);
		}
		protected virtual void UnlinkSelectionBarProperties() {
			InnerBindingHelper.ClearBinding(this, TimelineViewLayoutPanel.SelectionBarCellsProperty);
		}
		protected virtual void MeasureMainArea(Size availableSize) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.TimelineViewPanelGroupByNone, "->TimelineViewPanelGroupByNone.MeasureMainArea : finalSize={0}", availableSize);
			try {
				Size allSize = availableSize;
				DateHeader.Measure(availableSize);
				availableSize.Height -= DateHeader.DesiredSize.Height;
				SelectionBar.Measure(availableSize);
				double selectionBarHeight = CalculateSelectionBarHeight();
				if (availableSize.Height < selectionBarHeight)
					availableSize.Height = 0;
				else
					availableSize.Height -= selectionBarHeight;
				if (availableSize.Height <= 0)
					return;
				if (DateTimeScrollBarArea != null && DateTimeScrollbarVisible) {
					DateTimeScrollBarArea.Measure(availableSize);
					availableSize.Height -= DateTimeScrollBarArea.DesiredSize.Height;
				}
				if (AdornedBorder != null) {
					availableSize.Height = Math.Max(0, availableSize.Height - AdornedBorder.BorderThickness.Bottom);
				}
				VisualResources.Measure(availableSize);
				TimeIndicatorContainer.Measure(allSize);
			} finally {
				SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.TimelineViewPanelGroupByNone);
			}
		}
		protected double CalculateSelectionBarHeight() {
			if (SelectionBarVisible == false)
				return 0;
			return (SelectionBarHeight == 0) ? DefaultSelectionBarHeight : SelectionBarHeight;
		}
		protected override double GetDesiredWidth() {
			return VisualResources.DesiredSize.Width;
		}
		protected override double GetDesiredHeight() {
			return DateHeader.DesiredSize.Height + VisualResources.DesiredSize.Height;
		}
		protected double CalculateDateTimeScrollBarAreaHeight() {
			return (DateTimeScrollBarArea != null && DateTimeScrollbarVisible) ? DateTimeScrollBarArea.DesiredSize.Height : 0;
		}
		protected double CalculateCellsAreaHeight(double availableHeight, double topHeaderHeight, double dateTimeScrollBarHeight) {
			double result = availableHeight - (topHeaderHeight + dateTimeScrollBarHeight);
			if (AdornedBorder != null)
				result -= AdornedBorder.BorderThickness.Bottom;
			return Math.Max(0, result);
		}
		protected double CalculateDateTimeScrollBarWidth(double cellsAreaWidth) {
			if (AdornedBorder != null)
				return Math.Max(0, cellsAreaWidth - AdornedBorder.BorderThickness.Right);
			return cellsAreaWidth;
		}
		protected virtual void AddHandler(Type propertiesType, PropertyChangedEventHandler handler) {
			HandlerList.AddHandler(propertiesType, handler);
		}
		protected virtual void RemoveHandler(Type propertiesType, PropertyChangedEventHandler handler) {
			HandlerList.RemoveHandler(propertiesType, handler);
		}
		protected virtual void NotifyPropertyChanged(Type propertiesType, string propertyName) {
			PropertyChangedEventHandler handlder = HandlerList[propertiesType] as PropertyChangedEventHandler;
			if (handlder != null)
				handlder(this, new PropertyChangedEventArgs(propertyName));
		}
		#region IDisposable Members
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				if (HandlerList != null) {
					HandlerList.Dispose();
					HandlerList = null;
				}
			}
		}
		#endregion
		#region IComponentProperties Members
		void IComponentProperties.AddHandler(Type propertiesType, PropertyChangedEventHandler handler) {
			AddHandler(propertiesType, handler);
		}
		void IComponentProperties.RemoveHandler(Type propertiesType, PropertyChangedEventHandler handler) {
			RemoveHandler(propertiesType, handler);
		}
		#endregion
	}
	#endregion
	#region TimelineViewPanelGroupByNone
	public class TimelineViewPanelGroupByNone : TimelineViewLayoutPanel {
		public TimelineViewPanelGroupByNone() {
		}
		public VisualTimelineViewGroupByNone TimelineViewInfo { get { return (VisualTimelineViewGroupByNone)ViewInfo; } }
		protected override void MeasureComponents(Size availableSize) {
			if (AdornedBorder != null)
				AdornedBorder.Measure(availableSize);
			MeasureMainArea(availableSize);
		}
		protected override void ArrangeComponents(Size finalSize) {
			double selectionBarHeight = CalculateSelectionBarHeight();
			double dateHeaderHeight = DateHeader.DesiredSize.Height;
			double cellsAreaWidth = finalSize.Width;
			double topHeaderHeight = dateHeaderHeight + selectionBarHeight;
			double dateTimeScrollBarWidth = CalculateDateTimeScrollBarWidth(cellsAreaWidth);
			double dateTimeScrollBarHeight = CalculateDateTimeScrollBarAreaHeight();
			double cellsAreaHeight = CalculateCellsAreaHeight(finalSize.Height, topHeaderHeight, dateTimeScrollBarHeight);
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.TimelineViewPanelGroupByNone, "->TimelineViewPanelGroupByNone.ArrangeComponents : finalSize={0}", finalSize);
			Rect selectionBarBounds = new Rect(new Point(0, dateHeaderHeight), new Size(cellsAreaWidth, selectionBarHeight));
			SelectionBar.Arrange(selectionBarBounds);
			Rect resourceBounds = new Rect(new Point(0, topHeaderHeight), new Size(cellsAreaWidth, cellsAreaHeight));
			VisualResources.Arrange(resourceBounds);
			Rect dateHeaderBounds = new Rect(new Point(0, 0), new Size(cellsAreaWidth, dateHeaderHeight));
			SchedulerLogger.Trace(XpfLoggerTraceLevel.TimelineViewPanelGroupByNone, "| Arrange DateHeader: HeaderBounds={0}", dateHeaderBounds);
			BaseCellsInfo.UpdateBaseSize(VisualResources.GetBaseCellBounds(), cellsAreaWidth);
			DateHeader.Arrange(dateHeaderBounds);
			if (DateTimeScrollBarArea != null && DateTimeScrollbarVisible) {
				Rect scrollBarBounds = new Rect(new Point(0, resourceBounds.Bottom), new Size(dateTimeScrollBarWidth, dateTimeScrollBarHeight));
				DateTimeScrollBarArea.Arrange(scrollBarBounds);
			}
			if (AdornedBorder != null)
				AdornedBorder.Arrange(new Rect(new Point(0, 0), finalSize));
			TimeIndicatorContainer.Arrange(new Rect(new Point(0, 0), finalSize));
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.TimelineViewPanelGroupByNone);
		}
	}
	#endregion
	#region TimelineViewGroupByDateLayoutPanel
	public class TimelineViewGroupByDateLayoutPanel : TimelineViewLayoutPanel, ITimelineViewGroupByDateComponentProperties {
		#region Properties
		#region ResourceHeaderElementPosition
		public static readonly DependencyProperty ResourceHeaderElementPositionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewGroupByDateLayoutPanel, ElementPosition>("ResourceHeaderElementPosition", ElementPosition.Standalone, (d, e) => d.ResourceHeaderElementPositionChanged(e.OldValue, e.NewValue));
		public ElementPosition ResourceHeaderElementPosition { get { return (ElementPosition)GetValue(ResourceHeaderElementPositionProperty); } set { SetValue(ResourceHeaderElementPositionProperty, value); } }
		public void ResourceHeaderElementPositionChanged(ElementPosition oldValue, ElementPosition newValue) {
			if (ResourceHeader != null)
				ResourceHeader.OwnElementPosition = newValue;
		}
		#endregion
		#region ResourceNavigatorStyle
		public Style ResourceNavigatorStyle {
			get { return (Style)GetValue(ResourceNavigatorStyleProperty); }
			set { SetValue(ResourceNavigatorStyleProperty, value); }
		}
		public static readonly DependencyProperty ResourceNavigatorStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewGroupByDateLayoutPanel, Style>("ResourceNavigatorStyle", null, (d, e) => d.OnResourceNavigatorStyleChanged(e.OldValue, e.NewValue), null);
		void OnResourceNavigatorStyleChanged(Style oldValue, Style newValue) {
			NotifyPropertyChanged(typeof(IResourceNavigatorComponentProperties), "ResourceNavigatorStyle");
		}
		#endregion
		#region Scheduler
		public SchedulerControl Scheduler {
			get { return (SchedulerControl)GetValue(SchedulerProperty); }
			set { SetValue(SchedulerProperty, value); }
		}
		public static readonly DependencyProperty SchedulerProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewGroupByDateLayoutPanel, SchedulerControl>("Scheduler", null, (d, e) => d.OnSchedulerChanged(e.OldValue, e.NewValue), null);
		void OnSchedulerChanged(SchedulerControl oldValue, SchedulerControl newValue) {
			ResourceNavigator.Scheduler = newValue;
		}
		#endregion
		#region ResourceNavigatorVisibility
		public ResourceNavigatorVisibility ResourceNavigatorVisibility {
			get { return (ResourceNavigatorVisibility)GetValue(ResourceNavigatorVisibilityProperty); }
			set { SetValue(ResourceNavigatorVisibilityProperty, value); }
		}
		public static readonly DependencyProperty ResourceNavigatorVisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewGroupByDateLayoutPanel, ResourceNavigatorVisibility>("ResourceNavigatorVisibility", ResourceNavigatorVisibility.Auto, (d, e) => d.OnResourceNavigatorVisibilityChanged(e.OldValue, e.NewValue), null);
		void OnResourceNavigatorVisibilityChanged(ResourceNavigatorVisibility oldValue, ResourceNavigatorVisibility newValue) {
			ResourceNavigator.Visibility = CalculateResourceNavigatorVisibility();
		}
		#endregion
		#region TopRightCornerAreaTemplate
		public ControlTemplate TopRightCornerAreaTemplate {
			get { return (ControlTemplate)GetValue(TopRightCornerAreaTemplateProperty); }
			set { SetValue(TopRightCornerAreaTemplateProperty, value); }
		}
		public static readonly DependencyProperty TopRightCornerAreaTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewGroupByDateLayoutPanel, ControlTemplate>("TopRightCornerAreaTemplate", null, (d, e) => d.OnTopRightCornerAreaTemplateChanged(e.OldValue, e.NewValue));
		void OnTopRightCornerAreaTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			if (newValue == null && TopRightCornerArea != null) {
				UpdateChild(TopRightCornerArea, null);
				return;
			}
			if (TopRightCornerArea == null) {
				TopRightCornerArea = new SchedulerAreaControl();
				Canvas.SetZIndex(TopRightCornerArea, 2);
				UpdateChild(null, TopRightCornerArea);
			}
			TopRightCornerArea.Template = newValue;
		}
		#endregion
		#region TopLeftCornerAreaTemplate
		public ControlTemplate TopLeftCornerAreaTemplate {
			get { return (ControlTemplate)GetValue(TopLeftCornerAreaTemplateProperty); }
			set { SetValue(TopLeftCornerAreaTemplateProperty, value); }
		}
		public static readonly DependencyProperty TopLeftCornerAreaTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewGroupByDateLayoutPanel, ControlTemplate>("TopLeftCornerAreaTemplate", null, (d, e) => d.OnTopLeftCornerAreaTemplateChanged(e.OldValue, e.NewValue));
		void OnTopLeftCornerAreaTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			if (newValue == null && TopLeftCornerArea != null) {
				UpdateChild(TopLeftCornerArea, null);
				return;
			}
			if (TopLeftCornerArea == null) {
				TopLeftCornerArea = new SchedulerAreaControl();
				Canvas.SetZIndex(TopLeftCornerArea, 2);
				UpdateChild(null, TopLeftCornerArea);
			}
			TopLeftCornerArea.Template = newValue;
		}
		#endregion
		#region BottomRightCornerAreaTemplate
		public ControlTemplate BottomRightCornerAreaTemplate {
			get { return (ControlTemplate)GetValue(BottomRightCornerAreaTemplateProperty); }
			set { SetValue(BottomRightCornerAreaTemplateProperty, value); }
		}
		public static readonly DependencyProperty BottomRightCornerAreaTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewGroupByDateLayoutPanel, ControlTemplate>("BottomRightCornerAreaTemplate", null, (d, e) => d.OnBottomRightCornerAreaTemplateChanged(e.OldValue, e.NewValue));
		void OnBottomRightCornerAreaTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			if (newValue == null && BottomRightCornerArea != null) {
				UpdateChild(BottomRightCornerArea, null);
				return;
			}
			if (BottomRightCornerArea == null) {
				BottomRightCornerArea = new SchedulerAreaControl();
				UpdateChild(null, BottomRightCornerArea);
			}
			BottomRightCornerArea.Template = newValue;
		}
		#endregion
		#region BottomLeftCornerAreaTemplate
		public ControlTemplate BottomLeftCornerAreaTemplate {
			get { return (ControlTemplate)GetValue(BottomLeftCornerAreaTemplateProperty); }
			set { SetValue(BottomLeftCornerAreaTemplateProperty, value); }
		}
		public static readonly DependencyProperty BottomLeftCornerAreaTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewGroupByDateLayoutPanel, ControlTemplate>("BottomLeftCornerAreaTemplate", null, (d, e) => d.OnBottomLeftCornerAreaTemplateChanged(e.OldValue, e.NewValue));
		void OnBottomLeftCornerAreaTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			if (newValue == null && BottomLeftCornerArea != null) {
				UpdateChild(BottomLeftCornerArea, null);
				return;
			}
			if (BottomLeftCornerArea == null) {
				BottomLeftCornerArea = new SchedulerAreaControl();
				Canvas.SetZIndex(BottomLeftCornerArea, 2);
				UpdateChild(null, BottomLeftCornerArea);
			}
			BottomLeftCornerArea.Template = newValue;
		}
		#endregion
		#region ResourceHeaderStyle
		public Style ResourceHeaderStyle {
			get { return (Style)GetValue(ResourceHeaderStyleProperty); }
			set { SetValue(ResourceHeaderStyleProperty, value); }
		}
		public static readonly DependencyProperty ResourceHeaderStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineViewGroupByDateLayoutPanel, Style>("ResourceHeaderStyle", default(Style), (d, e) => d.OnResourceHeaderStyleChanged(e.OldValue, e.NewValue));
		void OnResourceHeaderStyleChanged(Style oldValue, Style newValue) {
			NotifyPropertyChanged(typeof(IResourceHeaderComponentProperties), "ResourceHeaderStyle");
		}
		#endregion
		protected Control TopRightCornerArea { get; set; }
		protected Control BottomLeftCornerArea { get; set; }
		protected Control TopLeftCornerArea { get; set; }
		protected Control BottomRightCornerArea { get; set; }
		public ResourceHeaderComponent ResourceHeader { get; private set; }
		public ResourceNavigatorComponent ResourceNavigator { get; private set; }
		#endregion
		protected override void OnResourceContainersChanged(VisualResourcesCollection oldValue, VisualResourcesCollection newValue) {
			base.OnResourceContainersChanged(oldValue, newValue);
			if (ResourceHeader != null)
				ResourceHeader.ViewModelItems = newValue;
		}
		protected override void CreateComponents() {
			base.CreateComponents();
			ResourceHeader = new ResourceHeaderComponent(this, this);
			ResourceHeader.Orientation = Orientation.Vertical;
			RegisterComponent(ResourceHeader);
			ResourceHeader.Initialize();
			ResourceNavigator = new ResourceNavigatorComponent(this, this);
			RegisterComponent(ResourceNavigator);
			ResourceNavigator.Initialize();
		}
		protected override void LinkToViewInfo(VisualViewInfoBase viewInfo) {
			base.LinkToViewInfo(viewInfo);
			VisualTimelineViewGroupBase timelineViewInfo = viewInfo as VisualTimelineViewGroupBase;
			if (timelineViewInfo == null)
				return;
			LinkResourceHeaderToViewInfo(timelineViewInfo);
		}
		protected virtual void LinkResourceHeaderToViewInfo(VisualTimelineViewGroupBase viewInfo) {
			InnerBindingHelper.SetBinding(this, viewInfo.View, TimelineViewGroupByDateLayoutPanel.ResourceHeaderStyleProperty, TimelineView.VerticalResourceHeaderStylePropertyName);
		}
		Visibility CalculateResourceNavigatorVisibility() {
			return (ResourceNavigatorVisibility == ResourceNavigatorVisibility.Never) ? Visibility.Collapsed : Visibility.Visible;
		}
		protected override double CalculateMinPanelHeight() {
			Size infinitySize = new Size(double.PositiveInfinity, double.PositiveInfinity);
			ResourceNavigator.Measure(infinitySize);
			DateTimeScrollBarArea.Measure(infinitySize);
			AdornedBorder.Measure(infinitySize);
			double resourceHeaderMinHeight = ResourceHeader.MeasureDesiredSize(infinitySize).Height;
			double resourceNavigatorOrHeaderMinHeight = Math.Max(ResourceNavigator.DesiredSize.Height, resourceHeaderMinHeight);
			double dateHeaderHeight = DateHeader.MeasureDesiredSize(infinitySize).Height;
			double selectionBarMeasureCoreInternalHeight = CalculateSelectionBarHeight();
			double DateTimeScrollBarAreaHeight = (DateTimeScrollbarVisible) ? DateTimeScrollBarArea.DesiredSize.Height : 0;
			return resourceNavigatorOrHeaderMinHeight + dateHeaderHeight + selectionBarMeasureCoreInternalHeight + DateTimeScrollBarAreaHeight + AdornedBorder.BorderThickness.Bottom;
		}
		protected override void MeasureComponents(Size availableSize) {
			if (AdornedBorder != null)
				AdornedBorder.Measure(availableSize);
			ResourceNavigator.Measure(availableSize);
			availableSize.Width -= ResourceNavigator.DesiredSize.Width;
			if (TopLeftCornerArea != null)
				TopLeftCornerArea.Measure(availableSize);
			long oldResourceHeaderVersion = ResourceHeader.MeasureVersion;
			ResourceHeader.Measure(availableSize);
			double newAvailableWidth = availableSize.Width - ResourceHeader.DesiredSize.Width;
			if (newAvailableWidth < 0)
				newAvailableWidth = 0;
			availableSize.Width = newAvailableWidth;
			long oldVisualResourcesVersion = VisualResources.MeasureVersion;
			MeasureMainArea(availableSize);
			bool isResourceHeaderMeasured = oldResourceHeaderVersion != ResourceHeader.MeasureVersion;
			bool isVisualResourcesMeasured = oldVisualResourcesVersion != VisualResources.MeasureVersion;
			if ((isResourceHeaderMeasured || isVisualResourcesMeasured) && VisualResources.IsMeasured) {
				ResourceHeader.SyncMeasure(VisualResources.GetChildrenPrimarySize());
			}
			if (TopRightCornerArea != null)
				TopRightCornerArea.Measure(availableSize);
			if (BottomRightCornerArea != null)
				BottomRightCornerArea.Measure(availableSize);
			if (BottomLeftCornerArea != null)
				BottomLeftCornerArea.Measure(availableSize);
		}
		protected override double GetDesiredWidth() {
			return ResourceHeader.DesiredSize.Width + VisualResources.DesiredSize.Width + ResourceNavigator.DesiredSize.Width;
		}
		protected override double GetDesiredHeight() {
			double resourceNavigatorHeight = ResourceNavigator.DesiredSize.Height;
			double resourceHeaderAreaHeight = Math.Max(ResourceHeader.DesiredSize.Height, resourceNavigatorHeight);
			double dateTimeScrollbarAreaHeight = (DateTimeScrollbarVisible) ? DateTimeScrollBarArea.DesiredSize.Height : 0;
			return DateHeader.DesiredSize.Height + CalculateSelectionBarHeight() + dateTimeScrollbarAreaHeight + resourceHeaderAreaHeight + AdornedBorder.BorderThickness.Bottom;
		}
		protected override void ArrangeComponents(Size finalSize) {
			double resourceHeaderWidth = CalculateResourceHeaderWidth();
			double selectionBarHeight = CalculateSelectionBarHeight();
			double resourceNavigatorWidth = ResourceNavigator.DesiredSize.Width;
			double dateTimeScrollBarHeight = CalculateDateTimeScrollBarAreaHeight();
			double dateHeaderHeight = DateHeader.DesiredSize.Height;
			double cellsAreaWidth = CalculateCellsAreaWidth(finalSize.Width, resourceHeaderWidth, resourceNavigatorWidth);
			double topHeaderHeight = dateHeaderHeight + selectionBarHeight;
			double cellsAreaHeight = CalculateCellsAreaHeight(finalSize.Height, topHeaderHeight, dateTimeScrollBarHeight);
			Rect selectionBarBounds = new Rect(new Point(resourceHeaderWidth, dateHeaderHeight), new Size(cellsAreaWidth, selectionBarHeight));
			SelectionBar.Arrange(selectionBarBounds);
			Rect resourceBounds = new Rect(new Point(resourceHeaderWidth, topHeaderHeight), new Size(cellsAreaWidth, cellsAreaHeight));
			VisualResources.Arrange(resourceBounds);
			double dateHeaderWidth = Math.Max(0, finalSize.Width - (resourceHeaderWidth + resourceNavigatorWidth));
			Rect dateHeaderBounds = new Rect(new Point(resourceHeaderWidth, 0), new Size(dateHeaderWidth, dateHeaderHeight));
			SchedulerLogger.Trace(XpfLoggerTraceLevel.TimelineViewPanelGroupByNone, "| Arrange DateHeader: HeaderBounds={0}", dateHeaderBounds);
			BaseCellsInfo.UpdateBaseSize(VisualResources.GetBaseCellBounds(), dateHeaderWidth);
			DateHeader.Arrange(dateHeaderBounds);
			if (DateTimeScrollBarArea != null && DateTimeScrollbarVisible) {
				double dateTimeScrollBarWidth = CalculateDateTimeScrollBarWidth(cellsAreaWidth);
				Rect scrollBarBounds = new Rect(new Point(resourceHeaderWidth, resourceBounds.Bottom), new Size(dateTimeScrollBarWidth, dateTimeScrollBarHeight));
				DateTimeScrollBarArea.Arrange(scrollBarBounds);
			}
			if (ResourceHeader != null) {
				Rect resourceHeaderBounds = new Rect(new Point(0, topHeaderHeight), new Size(resourceHeaderWidth, cellsAreaHeight));
				ResourceHeader.Arrange(VisualResources.GetChildrenPrimarySize(), resourceHeaderBounds);
			}
			if (AdornedBorder != null) {
				AdornedBorder.Arrange(new Rect(new Point(0, 0), finalSize));
			}
			if (ResourceNavigator != null) {
				Point location = CalculateResourceNavigatorLocation(resourceHeaderWidth, cellsAreaWidth, topHeaderHeight);
				Rect resourceNavigatorBounds = new Rect(location, new Size(resourceNavigatorWidth, cellsAreaHeight));
				ResourceNavigator.Arrange(resourceNavigatorBounds);
			}
			if (TopLeftCornerArea != null) {
				Rect topLeftCorner = new Rect(new Point(0, 0), new Point(resourceHeaderWidth, topHeaderHeight));
				TopLeftCornerArea.Arrange(topLeftCorner);
			}
			if (TopRightCornerArea != null) {
				Rect topRightCornerBounds = new Rect(dateHeaderBounds.TopRight(), new Size(resourceNavigatorWidth, topHeaderHeight));
				TopRightCornerArea.Arrange(topRightCornerBounds);
			}
			if (BottomRightCornerArea != null) {
				Point location = CalculateBottomRightCornerLocation(resourceHeaderWidth, cellsAreaWidth, topHeaderHeight, cellsAreaHeight);
				Rect bottomRightCornerBounds = new Rect(location, new Size(resourceNavigatorWidth, dateTimeScrollBarHeight));
				BottomRightCornerArea.Arrange(bottomRightCornerBounds);
			}
			if (BottomLeftCornerArea != null) {
				double bottomLeftCornerHeight = CalculateBottonLeftCornerHeight(dateTimeScrollBarHeight);
				Rect bottomLeftCornerBounds = new Rect(new Point(0, resourceBounds.Bottom), new Size(resourceHeaderWidth, bottomLeftCornerHeight));
				BottomLeftCornerArea.Arrange(bottomLeftCornerBounds);
			}
			TimeIndicatorContainer.Arrange(new Rect(new Point(resourceHeaderWidth, 0), finalSize));
			UpdateHotZoneBounds(resourceBounds);
		}
		void UpdateHotZoneBounds(Rect bounds) {
			if (Scheduler == null)
				return;
			Scheduler.HotZoneBounds = bounds;
		}
		double CalculateResourceHeaderWidth() {
			return (ResourceHeader != null) ? ResourceHeader.DesiredSize.Width : 0;
		}
		Point CalculateBottomRightCornerLocation(double resourceHeaderWidth, double cellsAreaWidth, double topHeaderHeight, double cellsAreaHeight) {
			Point location = new Point(resourceHeaderWidth + cellsAreaWidth, topHeaderHeight + cellsAreaHeight);
			if (AdornedBorder != null)
				location.X -= AdornedBorder.BorderThickness.Right;
			if (location.X < 0)
				location.X = 0;
			return location;
		}
		double CalculateBottonLeftCornerHeight(double dateTimeScrollBarHeight) {
			if (AdornedBorder != null)
				return dateTimeScrollBarHeight + AdornedBorder.BorderThickness.Bottom;
			return dateTimeScrollBarHeight;
		}
		Point CalculateResourceNavigatorLocation(double resourceHeaderWidth, double cellsAreaWidth, double topHeaderHeight) {
			Point location = new Point(resourceHeaderWidth + cellsAreaWidth, topHeaderHeight);
			if (AdornedBorder != null)
				location.X -= AdornedBorder.BorderThickness.Right;
			if (location.X < 0)
				location.X = 0;
			return location;
		}
		double CalculateCellsAreaWidth(double availableWidth, double resourceHeaderWidth, double resourceNavigatorWidth) {
			double result = availableWidth - (resourceHeaderWidth + resourceNavigatorWidth);
			if (result < 0)
				return 0;
			return result;
		}
	}
	#endregion
	#region PanelPropertyNames
	public class PanelPropertyNames {
		public const string AppointmentAutoHeightPropertyName = "AppointmentAutoHeight";
	}
	#endregion
}
