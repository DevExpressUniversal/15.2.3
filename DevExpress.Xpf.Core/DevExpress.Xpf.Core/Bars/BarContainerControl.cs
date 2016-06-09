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

using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using System.Collections.Specialized;
using DevExpress.Xpf.Core;
using System.Windows.Input;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using DevExpress.Utils;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Bars.Automation;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Bars.Native;
using System.Windows.Markup;
using System.Collections;
using System.Windows.Data;
using System.Collections.Generic;
using DevExpress.Mvvm.Native;
using System.Linq;
using System.Windows.Controls.Primitives;
namespace DevExpress.Xpf.Bars {
	[DXToolboxBrowsable]
	[ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	[ContentProperty("Bars")]
	public class BarContainerControl : BarItemsControl, IMultipleElementRegistratorSupport {
		#region static
		public static readonly DependencyProperty BarVertIndentProperty;
		public static readonly DependencyProperty BarHorzIndentProperty;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty DrawBorderProperty;
		internal static readonly DependencyPropertyKey IsFloatingPropertyKey;
		public static readonly DependencyProperty IsFloatingProperty;
		public static readonly DependencyProperty ContainerTypeProperty;
		internal static readonly DependencyPropertyKey ActualPaddingPropertyKey;
		public static readonly DependencyProperty ActualPaddingProperty;
		protected static readonly DependencyPropertyKey BarsPropertyKey;
		public static readonly DependencyProperty BarsProperty;
		public static readonly DependencyProperty BarItemDisplayModeProperty;		
		static BarContainerControl() {
			BarItemDisplayModeProperty = DependencyPropertyManager.Register("BarItemDisplayMode", typeof(BarItemDisplayMode), typeof(BarContainerControl), new FrameworkPropertyMetadata(BarItemDisplayMode.Default, new PropertyChangedCallback((d, e) => ((BarContainerControl)d).OnBarItemDisplayModeChanged((BarItemDisplayMode)e.OldValue))));
			BarVertIndentProperty = DependencyPropertyManager.Register("BarVertIndent", typeof(double), typeof(BarContainerControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, null, new CoerceValueCallback(OnIndentCoerce)));
			BarHorzIndentProperty = DependencyPropertyManager.Register("BarHorzIndent", typeof(double), typeof(BarContainerControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, null, new CoerceValueCallback(OnIndentCoerce)));
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(BarContainerControl), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnOrientationPropertyChanged), new CoerceValueCallback(OnOrientationPropertyCoerce)));
			DrawBorderProperty = DependencyPropertyManager.Register("DrawBorder", typeof(bool), typeof(BarContainerControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnDrawBorderPropertyChanged)));
			IsFloatingPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsFloating", typeof(bool), typeof(BarContainerControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIsFloatingPropertyChanged)));
			IsFloatingProperty = IsFloatingPropertyKey.DependencyProperty;
			ContainerTypeProperty = DependencyPropertyManager.Register("ContainerType", typeof(BarContainerType), typeof(BarContainerControl), new PropertyMetadata(BarContainerType.None, new PropertyChangedCallback(OnContainerTypePropertyChanged)));
			ActualPaddingPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPadding", typeof(Thickness), typeof(BarContainerControl), new PropertyMetadata(new Thickness()));
			ActualPaddingProperty = ActualPaddingPropertyKey.DependencyProperty;
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarContainerControl), new FrameworkPropertyMetadata(typeof(BarContainerControl)));
			NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarContainerControl), typeof(BarContainerControlAutomationPeer), owner => new BarContainerControlAutomationPeer((BarContainerControl)owner));
			NameProperty.OverrideMetadata(typeof(BarContainerControl), new FrameworkPropertyMetadata(String.Empty, (d, e) => ((BarContainerControl)d).OnNameChanged((string)e.OldValue, (string)e.NewValue)));
			BarsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Bars", typeof(ObservableCollection<IBar>), typeof(BarContainerControl), new FrameworkPropertyMetadata(null));
			BarsProperty = BarsPropertyKey.DependencyProperty;
		}		
		public void Unlink(IBar bar) {
			if (Bars.Contains(bar)) {
				var wasNotHidden = bar.ShowInOriginContainer;
				bar.ShowInOriginContainer = false;
				if (wasNotHidden)
					Refresh();
				return;
			} else if (bar is Bar) {
				var tbc = Bars.OfType<ToolBarControlBase>().FirstOrDefault(x => x.Bar == bar);
				if (tbc != null) {
					tbc.ForcedVisibility = Visibility.Collapsed;
					return;
				}				
			}		  
			BindedBars.Remove(bar);
			var currentBarContainer = bar.DockInfo.Container;
			bar.ShowInOriginContainer = currentBarContainer == this || currentBarContainer == null || currentBarContainer == bar.OriginContainer;
			if (bar.ShowInOriginContainer)
				bar.OriginContainer.If(x => !x.HasToolBar(bar)).Do(x => x.Refresh());
		}
		public void Link(IBar bar) {
			if (Bars.Contains(bar)) {
				var wasHidden = !bar.ShowInOriginContainer;
				bar.ShowInOriginContainer = true;
				if (wasHidden)
					Refresh();
				return;
			} else if (bar is Bar) {
				var tbc = Bars.OfType<ToolBarControlBase>().FirstOrDefault(x => x.Bar == bar);
				if (tbc != null) {
					tbc.ForcedVisibility = null;
					return;
				}
			}
			bar.ShowInOriginContainer = false;
			bar.OriginContainer.If(x => !x.HasToolBar(bar)).Do(x => x.Refresh());
			if (!BindedBars.Contains(bar))
				BindedBars.Add(bar);   
		}
		ToolBarControlBase GetToolBar(IBar bar) {
			if(bar is Bar) {
				return Bars.OfType<ToolBarControlBase>().FirstOrDefault(x => x.Bar == bar);
			}
			return null;
		}
		bool HasToolBar(IBar bar) {
			return GetToolBar(bar).ReturnSuccess();
		}
		protected static void OnDrawBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarContainerControl)d).OnDrawBorderChanged(e);
		}
		protected static object OnOrientationPropertyCoerce(DependencyObject d, object baseValue) {
			return ((BarContainerControl)d).OnOrientationCoerce(baseValue);
		}
		protected static void OnContainerTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarContainerControl)d).OnContainerTypeChanged(e);
		}
		protected static void OnIsFloatingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarContainerControl)d).OnIsFloatingChanged(e);
		}
		protected static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarContainerControl)d).OnOrientationChanged(e);
		}
		protected static object OnIndentCoerce(DependencyObject d, object baseValue) {
			double val = (double)baseValue;
			return Math.Max(0.0, val);
		}
		#endregion
		public BarContainerControl() {
			barsInLogicalChildren = new List<IBar>();
			CreateBindedBars();
			CreateBars();
			CreateItemsSource();			
			FrameworkElementHelper.SetAllowDrop(this, true);
			Loaded += new RoutedEventHandler(OnLoaded);
			DragOver += new DragEventHandler(BarContainerControl_DragEvent);
			DragEnter += new DragEventHandler(BarContainerControl_DragEvent);
			DragLeave += new DragEventHandler(BarContainerControl_DragEvent);
			IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
		}		
		private ObservableCollection<IBar> barsCore;
		public ObservableCollection<IBar> Bars {
			get { return (ObservableCollection<IBar>)GetValue(BarsProperty); }
		}
		public BarItemDisplayMode BarItemDisplayMode {
			get { return (BarItemDisplayMode)GetValue(BarItemDisplayModeProperty); }
			set { SetValue(BarItemDisplayModeProperty, value); }
		}		
		protected internal ObservableCollection<IBar> BindedBars {
			get { return barsCore; }
		}
		protected IEnumerable ActualSource { get { return ItemsSource; } set { ItemsSource = value; } }
		protected virtual void CreateBars() {
			SetValue(BarsPropertyKey, new SimpleBarCollection<IBar>());
			Bars.CollectionChanged += OnBarsCollectionChanged;
		}
		protected virtual void CreateBindedBars() {
			barsCore = new ObservableCollection<IBar>();
			barsCore.CollectionChanged += OnBindedBarsCollectionChanged;
		}		
		protected virtual void CreateItemsSource() {
			CompositeCollection composite = new CompositeCollection();
			composite.Add(new CollectionContainer() { Collection = Bars });
			composite.Add(new CollectionContainer() { Collection = BindedBars });
			var compositeView = ((ICollectionViewFactory)composite).CreateView();
			var wrappedCollection = new ObservableCollectionConverter<object, IBar>() { Source = compositeView, Selector = x => (IBar)x };
			var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(wrappedCollection);
			lcv.CustomSort = BarComparer.Instance;
			lcv.Filter = new Predicate<object>(x => ((IBar)x).OriginContainer != this || ((IBar)x).ShowInOriginContainer);
			ActualSource = lcv;			
		}		
		protected internal void Refresh() {
			((ListCollectionView)ActualSource).Do(x => x.Refresh());
		}
		protected virtual void OnBarItemDisplayModeChanged(BarItemDisplayMode oldValue) {
			foreach (IBar element in ItemsSource) {
				var bc = element as ToolBarControlBase;
				if (bc != null)
					bc.BarControl.Do(x => x.UpdateItemsDisplayMode());
				var b = element as Bar;
				if (b != null)
					b.DockInfo.BarControl.Do(x => x.UpdateItemsDisplayMode());
			}
		}
		sealed class BarComparer : IComparer<IBar>, IComparer {
			static BarComparer instance;
			public static BarComparer Instance { get { return instance ?? (instance = new BarComparer()); } }
			BarComparer() { }			
			int IComparer<IBar>.Compare(IBar x, IBar y) {
				int result = -x.DockInfo.Compare(y.DockInfo);
				if (result != 0)
					return result;
				if (IsBinded(x) && !IsBinded(y))
					return 1;
				if (IsBinded(y) && !IsBinded(x))
					return -1;
				return Math.Sign(x.Index - y.Index);
			}
			int IComparer.Compare(object x, object y) {
				return ((IComparer<IBar>)this).Compare((IBar)x, (IBar)y);
			}
			bool IsBinded(IBar bar) {
				return bar.OriginContainer == null;
			}
		}
		[Obsolete("Use BarManager.Bars property instead.")]
		new public ItemCollection Items { get { return null; } }
		void BarContainerControl_DragEvent(object sender, DragEventArgs e) {
			e.Effects = DragDropEffects.None;
			e.Handled = true;
		}
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			foreach (Bar bar in BindedBars) {
				bar.CoerceValue(ContentElement.IsEnabledProperty);
			}
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected internal bool ShouldRestoreOnActivate { get; set; }
		BarContainerControlPanel clientPanel = null;
		protected internal BarContainerControlPanel ClientPanel {
			get { return clientPanel; }
			set {
				if (ClientPanel == value)
					return;
				clientPanel = value;
				OnClientPanelChanged();
			} 
		}
		protected virtual void OnClientPanelChanged() {
			if (ClientPanel != null)
				ClientPanel.Orientation = this.Orientation;
		}
		protected internal FloatingBarPopup OwnerPopup { get { return FloatingBarPopup.GetOwnerPopup(this); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarContainerControlContainerType")]
#endif
		public BarContainerType ContainerType {
			get { return (BarContainerType)GetValue(ContainerTypeProperty); }
			set { SetValue(ContainerTypeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarContainerControlBarVertIndent")]
#endif
		public double BarVertIndent {
			get { return (double)GetValue(BarVertIndentProperty); }
			set { SetValue(BarVertIndentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarContainerControlBarHorzIndent")]
#endif
		public double BarHorzIndent {
			get { return (double)GetValue(BarHorzIndentProperty); }
			set { SetValue(BarHorzIndentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarContainerControlOrientation")]
#endif
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarContainerControlDrawBorder")]
#endif
		public bool DrawBorder {
			get { return (bool)GetValue(DrawBorderProperty); }
			set { SetValue(DrawBorderProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarContainerControlIsFloating")]
#endif
		public bool IsFloating {
			get { return (bool)GetValue(IsFloatingProperty); }
			internal set { SetValue(IsFloatingPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarContainerControlActualPadding")]
#endif
		public Thickness ActualPadding {
			get { return (Thickness)GetValue(ActualPaddingProperty); }
			internal set { SetValue(ActualPaddingPropertyKey, value); }
		}				
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarContainerControlManager")]
#endif
		public BarManager Manager { get { return BarManager.GetBarManager(this); } }		
		protected virtual void InitializeClientPanel() {
			ItemsPresenter pr = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;
			if (pr == null)
				return;
			if (VisualTreeHelper.GetChildrenCount(pr) > 0)
				ClientPanel = VisualTreeHelper.GetChild(pr, 0) as BarContainerControlPanel;
			else
				pr.Loaded += OnItemsPresenterLoaded;
		}
		protected virtual void OnItemsPresenterLoaded(object sender, RoutedEventArgs e) {
			if (VisualTreeHelper.GetChildrenCount((DependencyObject)sender) == 0)
				return;
			ClientPanel = (BarContainerControlPanel)VisualTreeHelper.GetChild((DependencyObject)sender, 0);
			((ItemsPresenter)sender).Loaded -= OnItemsPresenterLoaded;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateBorderVisualState();
		}		
		protected bool IsPointInControl(Control c, MouseButtonEventArgs e) {
			if (c == null) return false;
			Point pt = e.GetPosition(c);
			return pt.X >= 0 && pt.Y >= 0 && pt.X <= c.ActualWidth && pt.Y <= c.ActualHeight;
		}
		protected virtual object OnOrientationCoerce(object baseValue) {
			Orientation o = (Orientation)baseValue;
			if (ContainerType == BarContainerType.Top || ContainerType == BarContainerType.Bottom)
				return Orientation.Horizontal;
			if (ContainerType == BarContainerType.Left || ContainerType == BarContainerType.Right)
				return Orientation.Vertical;
			return baseValue;
		}
		protected virtual void OnNameChanged(string oldValue, string newValue) {
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, BarRegistratorKeys.ContainerNameKey, oldValue, newValue);
		}
		protected virtual void OnContainerTypeChanged(DependencyPropertyChangedEventArgs e) {
			if (ContainerType == BarContainerType.Bottom || ContainerType == BarContainerType.Top)
				Orientation = Orientation.Horizontal;
			if (ContainerType == BarContainerType.Left || ContainerType == BarContainerType.Right)
				Orientation = Orientation.Vertical;
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, BarRegistratorKeys.ContainerTypeKey, e.OldValue, e.NewValue);
		}
		protected virtual bool ShouldProcessDoubleClick(MouseButtonEventArgs e) {
			if (e.Handled)
				return false;
			DependencyObject node = e.OriginalSource as DependencyObject;
			var result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
			node = (result == null ? null : result.VisualHit) ?? node;
			if(!(node is UIElement)) return false;
			BarItemLinkControlBase bc = LayoutHelper.FindParentObject<BarItemLinkControlBase>(node);
			return bc == null && (LayoutHelper.FindParentObject<BarClientPanel>(node) != null || LayoutHelper.FindParentObject<LinksControl>(node) == null);
		}
		protected override void OnMouseDoubleClick(MouseButtonEventArgs e) {
			base.OnMouseDoubleClick(e);
			if (e.Handled)
				return;
			if (e.ChangedButton == MouseButton.Left)
				if (BarManagerCustomizationHelper.IsInCustomizationMode(this)) {
					if (ShouldProcessDoubleClick(e))
						BarNameScope.GetService<ICustomizationService>(this).ShowCustomizationForm();   
			}
		}
		protected virtual void OnOrientationChanged(DependencyPropertyChangedEventArgs e) {
			Orientation orientation = (Orientation)e.NewValue;
			UpdatePaddings();
			if(ClientPanel != null)
				ClientPanel.Orientation = Orientation;
			foreach(var item in base.Items) {
				BarControl bc = ItemContainerGenerator.ContainerFromItem(item) as BarControl;
				if(bc != null) bc.ContainerOrientation = orientation;
			}
		}
		protected virtual void UpdatePaddings() {
			bool hasVisibleItems = false;
			foreach (var item in BindedBars) {
				BarControl bc = ItemContainerGenerator.ContainerFromItem(item) as BarControl;
				if (bc != null && bc.IsVisible()) {
					hasVisibleItems = true;
					break;
				}
			}
			if (hasVisibleItems) {
				ActualPadding = Orientation == Orientation.Horizontal ?
					BarContainerControlTemplateProvider.GetHorizontalPadding(this) : BarContainerControlTemplateProvider.GetVerticalPadding(this);
			} else ActualPadding = new Thickness();
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new BarControl();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is IBarLayoutTableInfo;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			Bar bar = item as Bar ?? ((ToolBarControlBase)item).With(x=>x.Bar);
			var iBar = (IBar)bar;
			if (iBar.OriginContainer == this && !iBar.ShowInOriginContainer && bar.DockInfo.Container != this)
				return;
			bar.DockInfo.Container = this;
			if (IsItemItsOwnContainerOverride(item))
				return;
			ClientPanel = (BarContainerControlPanel)VisualTreeHelper.GetParent(element);			
			BarControl control = element as BarControl;			
			control.Visibility = bar.Visible ? Visibility.Visible : Visibility.Collapsed;
			control.LinksHolder = bar;
			control.ContainerOrientation = Orientation;
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);
			if (IsItemItsOwnContainerOverride(item))
				return;
			Bar bar = (Bar)item;
			ClearBarControl(bar);
		}
		protected virtual void ClearBarControl(Bar bar) {			
			if (bar.DockInfo.BarControl != null) {
				bar.DockInfo.BarControl.OnClear();
			}
			bar.DockInfo.BarControl = null;
			if (!BindedBars.Contains(bar) 
				&& !Bars.Contains(bar)
				&& bar.DockInfo.Container == this)
				bar.DockInfo.Container = null;
		}
		protected virtual void OnIsFloatingChanged(DependencyPropertyChangedEventArgs e) {
			ClientPanel = null;
			foreach (Bar bar in BindedBars) {
				bar.DockInfo.BarControl.UpdateBarControlProperties();
			}
		}
		readonly List<IBar> barsInLogicalChildren;
		protected virtual void OnBarsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.NewItems != null)
				foreach (IBar value in e.NewItems) {
					value.OriginContainer = this;
					barsInLogicalChildren.Add(value);
					AddLogicalChild(value);
				}					
			if(e.OldItems!=null)
				foreach(IBar value in e.OldItems) {
					value.OriginContainer = null;
					barsInLogicalChildren.Remove(value);
					RemoveLogicalChild(value);
				}					
			if(e.Action == NotifyCollectionChangedAction.Reset) {
				List<IBar> bilc = barsInLogicalChildren.ToList();
				barsInLogicalChildren.Clear();
				bilc.ForEach(RemoveLogicalChild);
				barsInLogicalChildren.AddRange(Bars);
				barsInLogicalChildren.ForEach(AddLogicalChild);
			}
		}
		protected virtual void OnBindedBarsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {			
			if (e.Action == NotifyCollectionChangedAction.Add) {
				for(int i = 0; i < e.NewItems.Count; i++) {
					IBar bar = e.NewItems[i] as IBar;
					if (e.NewItems[i] != null && bar == null)
						throw new ArgumentException("Only IBar objects can be added to the BarContainerControl");					
				}
			}
			UpdatePaddings();	
		}
		protected override IEnumerator LogicalChildren {
			get {
				return new MergedEnumerator(base.LogicalChildren, Bars.GetEnumerator());
			}
		}
		protected internal ContentControl Border { get; private set; }
		protected internal ContentControl BackControl { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Border = (ContentControl)GetTemplateChild("PART_Border");
			BackControl = (ContentControl)GetTemplateChild("PART_Background");
			if (BackControl == null) return;
			UpdatePaddings();
			InitializeClientPanel();
			UpdateBorderVisualState();
		}
		protected internal virtual Bar GetFirstNonEmptyBar(bool skipStatusBar) {
			foreach (Bar bar in BindedBars) {
				if (bar.ItemLinks.Count > 0 && (!bar.IsStatusBar || !skipStatusBar)) return bar;
			}
			return null;
		}		
		protected virtual void UpdateBorderVisualState() {
			if (Border != null) {
				Border.Template = DrawBorder ? BarContainerControlTemplateProvider.GetBorderTemplate(this) : BarContainerControlTemplateProvider.GetEmptyBorderTemplate(this);
			}
			VisualStateManager.GoToState(this, DrawBorder ? "ShowBorder" : "HideBorder", false);
		}
		protected virtual void OnDrawBorderChanged(DependencyPropertyChangedEventArgs e) {
			UpdateBorderVisualState();
		}
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get {
				yield return BarRegistratorKeys.ContainerNameKey;
				yield return BarRegistratorKeys.ContainerTypeKey;
				yield return typeof(IFrameworkInputElement);
			}
		}		
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {
			if (Equals(BarRegistratorKeys.ContainerNameKey, registratorKey))
				return Name;
			if (Equals(BarRegistratorKeys.ContainerTypeKey, registratorKey))
				return ContainerType;
			if (Equals(typeof(IFrameworkInputElement), registratorKey))
				return Name;
			throw new ArgumentException();
		}
		protected internal virtual bool CanBind { get { return true; } }
	}
	public class BarContainerControlTemplateProvider : DependencyObject {
		public static readonly DependencyProperty BorderTemplateProperty = DependencyPropertyManager.RegisterAttached("BorderTemplate", typeof(ControlTemplate), typeof(BarContainerControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty EmptyBorderTemplateProperty = DependencyPropertyManager.RegisterAttached("EmptyBorderTemplate", typeof(ControlTemplate), typeof(BarContainerControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BackgroundTemplateProperty = DependencyPropertyManager.RegisterAttached("BackgroundTemplate", typeof(ControlTemplate), typeof(BarContainerControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HorizontalPaddingProperty = DependencyPropertyManager.RegisterAttached("HorizontalPadding", typeof(Thickness), typeof(BarContainerControlTemplateProvider), new FrameworkPropertyMetadata(new Thickness()));
		public static readonly DependencyProperty VerticalPaddingProperty = DependencyPropertyManager.RegisterAttached("VerticalPadding", typeof(Thickness), typeof(BarContainerControlTemplateProvider), new FrameworkPropertyMetadata(new Thickness()));
		public static Thickness GetVerticalPadding(DependencyObject obj) {
			return (Thickness)obj.GetValue(VerticalPaddingProperty);
		}
		public static void SetVerticalPadding(DependencyObject obj, Thickness value) {
			obj.SetValue(VerticalPaddingProperty, value);
		}
		public static Thickness GetHorizontalPadding(DependencyObject obj) {
			return (Thickness)obj.GetValue(HorizontalPaddingProperty);
		}
		public static void SetHorizontalPadding(DependencyObject obj, Thickness value) {
			obj.SetValue(HorizontalPaddingProperty, value);
		}
		public static ControlTemplate GetBackgroundTemplate(DependencyObject target) {
			return (ControlTemplate)target.GetValue(BackgroundTemplateProperty);
		}
		public static void SetBackgroundTemplate(DependencyObject target, ControlTemplate value) {
			target.SetValue(BackgroundTemplateProperty, value);
		}
		public static ControlTemplate GetEmptyBorderTemplate(DependencyObject target) {
			return (ControlTemplate)target.GetValue(EmptyBorderTemplateProperty);
		}
		public static void SetEmptyBorderTemplate(DependencyObject target, ControlTemplate value) {
			target.SetValue(EmptyBorderTemplateProperty, value);
		}
		public static ControlTemplate GetBorderTemplate(DependencyObject target) {
			return (ControlTemplate)target.GetValue(BorderTemplateProperty);
		}
		public static void SetBorderTemplate(DependencyObject target, ControlTemplate value) {
			target.SetValue(BorderTemplateProperty, value);
		}
	}
}
namespace DevExpress.Xpf.Bars.Native {
	public static class BarContainerControlPropertyAccessor {
		public static ObservableCollection<IBar> GetBars(BarContainerControl barContainerControl) {
			return barContainerControl.BindedBars;
		}
	}
}
