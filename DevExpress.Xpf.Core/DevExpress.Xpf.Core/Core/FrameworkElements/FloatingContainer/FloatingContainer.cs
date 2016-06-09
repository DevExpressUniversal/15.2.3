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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
#if !DXWINDOW
using DevExpress.Data;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Utils;
#endif
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
	public class FloatingContainerClosedException : Exception {
		public static void CheckFloatingContainerIsNotClosed(FloatingContainer container) {
			if(container.IsClosed)
				throw new FloatingContainerClosedException();
		}
	}
	[ContentProperty("Content")]
	public abstract class BaseFloatingContainer : FrameworkElement {
		#region static
		public static readonly DependencyProperty FloatLocationProperty;
		public static readonly DependencyProperty FloatSizeProperty;
		public static readonly DependencyProperty IsOpenProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContainerTemplateProperty;
		public static readonly DependencyProperty ShowContentOnlyProperty;
		public static readonly DependencyProperty OwnerProperty;
		static BaseFloatingContainer() {
			Type ownerType = typeof(BaseFloatingContainer);
			FloatLocationProperty = DependencyProperty.Register("FloatLocation", typeof(Point), ownerType,
				new FrameworkPropertyMetadata(OnFloatingBoundsChanged));
			FloatSizeProperty = DependencyProperty.Register("FloatSize", typeof(Size), ownerType, new FrameworkPropertyMetadata(OnFloatingBoundsChanged));
			IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, OnIsOpenChanged));
			ContentProperty = DependencyProperty.Register("Content", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnContentChanged));
			ContainerTemplateProperty = DependencyProperty.Register("ContainerTemplate", typeof(DataTemplate), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((BaseFloatingContainer)d).UpdatePresenterContentTemplate()));
			ShowContentOnlyProperty = DependencyProperty.Register("ShowContentOnly", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((BaseFloatingContainer)d).UpdatePresenterContentTemplate()));
			OwnerProperty = DependencyProperty.Register("Owner", typeof(FrameworkElement), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		}
		static void OnFloatingBoundsChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			BaseFloatingContainer container = dObj as BaseFloatingContainer;
			container.OnFloatingBoundsChanged(new Rect(container.FloatLocation, container.FloatSize));
		}
		static void OnIsOpenChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			BaseFloatingContainer container = dObj as BaseFloatingContainer;
			container.OnIsOpenChanged((bool)e.NewValue);
			container.NotifyIsOpenChanged((bool)e.NewValue);
		}
		static void OnContentChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			((BaseFloatingContainer)dObj).OnContentChanged(e.NewValue);
		}
		#endregion static
		public BaseFloatingContainer() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		public Point FloatLocation {
			get { return (Point)GetValue(FloatLocationProperty); }
			set { SetValue(FloatLocationProperty, value); }
		}
		public Size FloatSize {
			get { return (Size)GetValue(FloatSizeProperty); }
			set { SetValue(FloatSizeProperty, value); }
		}
		public bool IsOpen {
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public DataTemplate ContainerTemplate {
			get { return (DataTemplate)GetValue(ContainerTemplateProperty); }
			set { SetValue(ContainerTemplateProperty, value); }
		}
		public bool ShowContentOnly {
			get { return (bool)GetValue(ShowContentOnlyProperty); }
			set { SetValue(ShowContentOnlyProperty, value); }
		}
		public FrameworkElement Owner {
			get { return (FrameworkElement)GetValue(OwnerProperty); }
			set { SetValue(OwnerProperty, value); }
		}
		public bool UseActiveStateOnly { get; set; }
		protected virtual void OnFloatingBoundsChanged(Rect bounds) {
			if(IsAlive) UpdateFloatingBoundsCore(bounds);
		}
		protected virtual void OnIsOpenChanged(bool isOpen) {
			if(IsAlive) UpdateIsOpenCore(isOpen);
		}
		protected virtual void NotifyIsOpenChanged(bool isOpen) { }
		protected virtual void OnContentChanged(object content) {
			UpdateContainer(content);
		}
		int lockUpdateCounter = 0;
		public bool IsUpdateLocked {
			get { return lockUpdateCounter > 0; }
		}
		public void BeginUpdate() {
			lockUpdateCounter++;
		}
		public void EndUpdate() {
			if(--lockUpdateCounter == 0)
				UpdateContainer(Content);
		}
		protected void UpdateContainer(object content) {
			if(IsUpdateLocked) return;
			EnsureContainerHierarchy(Owner);
			Presenter.Content = content;
			UpdatePresenterContentTemplate();
			if(Owner != null && Owner.IsLoaded) {
				CheckIsOpen();
				CheckBoundsInContainer();
			}
			OnContentUpdated(content, Owner);
		}
		protected virtual void UpdatePresenterContentTemplate() {
			if(Presenter != null) Presenter.ContentTemplate = ShowContentOnly ? null : ContainerTemplate;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			CheckIsOpen();
			CheckBoundsInContainer();
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) { }
		public void CheckIsOpen() {
			OnIsOpenChanged(IsOpen);
		}
		public void CheckBoundsInContainer() {
			OnFloatingBoundsChanged(new Rect(FloatLocation, FloatSize));
		}
		protected abstract bool IsAlive { get; }
		protected abstract void UpdateFloatingBoundsCore(Rect bounds);
		protected abstract void UpdateIsOpenCore(bool isOpen);
		protected abstract void OnContentUpdated(object content, FrameworkElement owner);
		protected ManagedContentPresenter Presenter { get; private set; }
		protected NonLogicalDecorator Decorator { get; private set; }
		protected UIElement ContentContainer { get; private set; }
		protected virtual NonLogicalDecorator CreateNonLogicalDecorator() {
			return new NonLogicalDecorator();
		}
		protected virtual ManagedContentPresenter CreateContentPresenter() {
			return new ManagedContentPresenter(this);
		}
		protected abstract UIElement CreateContentContainer();
		protected abstract void AddDecoratorToContentContainer(NonLogicalDecorator decorator);
		bool fHierarchyCreated;
		protected FlowDirection storedFlowDirrection;
		protected void EnsureContainerHierarchy(FrameworkElement owner) {
			if(owner != null) storedFlowDirrection = owner.FlowDirection;
			if(!fHierarchyCreated) {
				fHierarchyCreated = true;
				ContentContainer = CreateContentContainer();
				Decorator = CreateNonLogicalDecorator();
				Presenter = CreateContentPresenter();
				AddDecoratorToContentContainer(Decorator);
				Decorator.Child = Presenter;
				AddLogicalChild(Presenter);
				OnHierarchyCreated();
			}
		}
		protected virtual void OnHierarchyCreated() { }
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				var children = new List<object>();
				if(Presenter != null)
					children.Add(Presenter);
				return children.GetEnumerator();
			}
		}
		#region internal classes
		public class ManagedContentPresenter : ContentPresenter {
			public BaseFloatingContainer Container { get; private set; }
			public ManagedContentPresenter(BaseFloatingContainer container) {
				Container = container;
			}
			protected internal virtual void UpdatePresenter() { }
		}
		#endregion internal classes
	}
	public class FloatingContainerEventArgs : RoutedEventArgs {
		public FloatingContainerEventArgs(FloatingContainer container) {
			this.Container = container;
		}
		public FloatingContainer Container { get; private set; }
	}
	public delegate void FloatingContainerEventHandler(object sender, FloatingContainerEventArgs e);
	public class CancelRoutedEventArgs : RoutedEventArgs {
		public CancelRoutedEventArgs(RoutedEvent routedEvent, object source)
			: base(routedEvent, source) {
		}
		public CancelRoutedEventArgs(RoutedEvent routedEvent)
			: base(routedEvent) {
		}
		public CancelRoutedEventArgs() {
		}
		public bool Cancel { get; set; }
	}
	public delegate void CancelRoutedEventHandler(object sender, CancelRoutedEventArgs e);
	public class FloatingContainerParameters {
		public DialogClosedDelegate ClosedDelegate { get; set; }
		public string Title { get; set; }
		public bool AllowSizing { get; set; }
		public bool ShowApplyButton { get; set; }
		public bool CloseOnEscape { get; set; }
		public ImageSource Icon { get; set; }
		public bool ContainerFocusable { get; set; }
		public bool ShowModal { get; set; }
		public FloatingContainerParameters() {
			ClosedDelegate = null;
			Title = string.Empty;
			AllowSizing = false;
			ShowApplyButton = false;
			CloseOnEscape = false;
			ContainerFocusable = true;
			ShowModal = true;
		}
	}
	[ContentProperty("Content")]
#if !DXWINDOW
	[SupportDXTheme(TypeInAssembly = typeof(FloatingContainer))]
#endif
	public abstract class FloatingContainer : BaseFloatingContainer, IDialogOwner {
		#region ShowDialog
		class FloatingContainerHiddenHandler {
			readonly FloatingContainer container;
			readonly FrameworkElement rootElement;
			readonly DialogClosedDelegate closedDelegate;
			public FloatingContainerHiddenHandler(FloatingContainer container, FrameworkElement rootElement, DialogClosedDelegate closedDelegate) {
				this.container = container;
				this.rootElement = rootElement;
				this.closedDelegate = closedDelegate;
			}
			public void HiddenHandler(object sender, RoutedEventArgs e) {
				if(closedDelegate != null)
					closedDelegate(container.DialogResult);
				container.Hidden -= new RoutedEventHandler(HiddenHandler);
				if(rootElement is ILogicalOwner)
					((ILogicalOwner)rootElement).RemoveChild(container);
				else {
					if(LogicalTreeHelper.GetParent(container) == rootElement) {
						LogicalTreeIntruder.RemoveLogicalChild(container);
					}
				}
			}
		}
#if !DXWINDOW
		public static FloatingContainer ShowDialogContent(FrameworkElement dialogContent, FrameworkElement rootElement, Size size, FloatingContainerParameters parameters) {
			return ShowDialogContent(dialogContent, rootElement, size, parameters, null);
		}
		public static FloatingContainer ShowDialogContent(FrameworkElement dialogContent, FrameworkElement rootElement, Size size, FloatingContainerParameters parameters, DependencyObject owner) {
			DialogControl dialogControl = new DialogControl() { DialogContent = dialogContent, ShowApplyButton = parameters.ShowApplyButton };
			return FloatingContainer.ShowDialog(dialogControl, rootElement, size, parameters, owner);
		}
		public static FloatingContainer ShowDialogContent(FrameworkElement dialogContent, FrameworkElement rootElement, Size size, FloatingContainerParameters parameters, bool useContentIndents) {
			return ShowDialogContent(dialogContent, rootElement, size, parameters, useContentIndents, null);
		}
		public static FloatingContainer ShowDialogContent(FrameworkElement dialogContent, FrameworkElement rootElement, Size size, FloatingContainerParameters parameters, bool useContentIndents, DependencyObject owner) {
			DialogControl dialogControl = new DialogControl() { DialogContent = dialogContent, ShowApplyButton = parameters.ShowApplyButton, UseContentIndents = useContentIndents };
			return FloatingContainer.ShowDialog(dialogControl, rootElement, size, parameters, owner);
		}
		public static FloatingContainer ShowDialog(FrameworkElement dialogContent, FrameworkElement element, DialogClosedDelegate closedDelegate, Size size, string title, bool allowSizing) {
			return FloatingContainer.ShowDialog(dialogContent, element, size, new FloatingContainerParameters() { ClosedDelegate = closedDelegate, Title = title, AllowSizing = allowSizing, CloseOnEscape = true });
		}
		public static FloatingContainer ShowDialog(FrameworkElement dialogContent, FrameworkElement element, Size size, FloatingContainerParameters parameters) {
			return ShowDialog(dialogContent, element, size, parameters, null);
		}
		public static FloatingContainer ShowDialog(FrameworkElement dialogContent, FrameworkElement element, Size size, FloatingContainerParameters parameters, DependencyObject owner) {
			if(DesignerProperties.GetIsInDesignMode(element)) {
				ShowDesignDialog(dialogContent, element, parameters.ClosedDelegate, size, parameters.Title, parameters.AllowSizing, parameters.Icon);
				return null;
			}
			FloatingContainer container = FloatingContainerFactory.Create(FloatingContainerFactory.CheckPopupHost(element));
			if(owner != null)
				FloatingContainer.SetFloatingContainer(owner, container);
			size = InitDialog(dialogContent, element, parameters.ClosedDelegate, size, parameters.Title, parameters.AllowSizing, container, parameters.CloseOnEscape, parameters.Icon, parameters.ContainerFocusable, parameters.ShowModal);
			return container;
		}
		static internal Size InitDialog(FrameworkElement dialogContent, FrameworkElement element, DialogClosedDelegate closedDelegate, Size size, string title, bool allowSizing, FloatingContainer container, bool closeOnEscape) {
			return InitDialog(dialogContent, element, closedDelegate, size, title, allowSizing, container, closeOnEscape, null, true);
		}
		static internal Size InitDialog(FrameworkElement dialogContent, FrameworkElement element, DialogClosedDelegate closedDelegate, Size size, string title, bool allowSizing, FloatingContainer container, bool closeOnEscape, ImageSource icon, bool containerFocusable) {
			return InitDialog(dialogContent, element, closedDelegate, size, title, allowSizing, container, closeOnEscape, icon, containerFocusable, true);
		}
		static internal Size InitDialog(FrameworkElement dialogContent, FrameworkElement element, DialogClosedDelegate closedDelegate, Size size, string title, bool allowSizing, FloatingContainer container, bool closeOnEscape, ImageSource icon, bool containerFocusable, bool showModal) {
			FloatingContainer.SetDialogOwner(dialogContent, container);
			DialogControl dialog = dialogContent as DialogControl;
			if(dialog != null) {
				DependencyObject obj = dialog.DialogContent as DependencyObject;
				if(obj != null)
					FloatingContainer.SetDialogOwner(obj, container);
			}
			if(element is ILogicalOwner)
				((ILogicalOwner)element).AddChild(container);
			else {
				if(LogicalTreeHelper.GetParent(container) == null) {
					LogicalTreeIntruder.AddLogicalChild(element, container);
				}
			}
			container.BeginUpdate();
			if(size.IsEmpty || (double.IsNaN(size.Width) && double.IsNaN(size.Height))) {
				container.SizeToContent = SizeToContent.WidthAndHeight;
			}
			else {
				if(double.IsNaN(size.Width))
					container.SizeToContent = SizeToContent.Width;
				else container.MinWidth = size.Width;
				if(double.IsNaN(size.Height))
					container.SizeToContent = SizeToContent.Height;
				else container.MinHeight = size.Height;
				container.FloatSize = new Size(
					Math.Max(container.FloatSize.Width, container.MinWidth),
					Math.Max(container.FloatSize.Height, container.MinHeight));
			}
			container.LogicalOwner = element;
			container.Owner = (FrameworkElement)container.InitDialogCorrectOwner(element); 
			container.ShowModal = showModal;
			container.Icon = icon;
			container.Caption = title;
			container.AllowSizing = allowSizing;
			container.CloseOnEscape = closeOnEscape;
			container.DeactivateOnClose = true;
			container.ContainerFocusable = containerFocusable;
			FloatingContainerHiddenHandler hiddenHandler = new FloatingContainerHiddenHandler(container, element, closedDelegate);
			container.Hidden += new RoutedEventHandler(hiddenHandler.HiddenHandler);
			if(IsRealWindow(container) && IsMessageBox(dialogContent))
				container.ContainerStartupLocation = WindowStartupLocation.CenterScreen;
			else
				container.ContainerStartupLocation = WindowStartupLocation.CenterOwner;
			container.Content = dialogContent;
			container.EndUpdate();
			if (!container.IsUpdateLocked) container.IsOpen = true;
			return size;
		}
#endif
		static bool IsRealWindow(FloatingContainer container) {
			return !BrowserInteropHelper.IsBrowserHosted && container.GetFloatingMode() == FloatingMode.Window;
		}
		static bool IsMessageBox(FrameworkElement dialogContent) {
#if DXWINDOW
			return false;
#else
			return (dialogContent != null) && dialogContent is DXMessageBox;
#endif
		}
		protected virtual UIElement InitDialogCorrectOwner(UIElement element){
			return element;
		}
		static void ShowDesignDialog(FrameworkElement dialogContent, FrameworkElement element, DialogClosedDelegate closedDelegate, Size size, string title, bool allowSizing, ImageSource icon) {
			Window window = new DXWindow()
			{
				Title = title,
				ResizeMode = allowSizing ? ResizeMode.CanResize : ResizeMode.NoResize,
				Content = dialogContent,
				ShowInTaskbar = false,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				WindowStyle = WindowStyle.ToolWindow,
			};
			if(size != Size.Empty) {
				window.Width = size.Width;
				window.Height = size.Height;
				window.MinWidth = size.Width;
				window.MinHeight = size.Height;
			}
			WindowContentHolder.SetHwndSourceOwner(window, element);
			window.ShowDialog();
			if(closedDelegate != null)
				closedDelegate(window.DialogResult);
		}
		#endregion
		#region static
		public static readonly DependencyProperty IsActiveProperty;
		public static readonly DependencyProperty IsClosedProperty;
		static readonly DependencyPropertyKey IsClosedPropertyKey;
		public static readonly DependencyProperty FloatingContainerProperty;
		public static readonly DependencyProperty ContainerStartupLocationProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty AllowMovingProperty;
		public static readonly DependencyProperty AllowSizingProperty;
		public static readonly DependencyProperty AllowShowAnimationsProperty;
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty ContainerFocusableProperty;
		public static readonly DependencyProperty IconProperty;
		public static readonly RoutedEvent HiddenEvent;
		public static readonly RoutedEvent HidingEvent;
		public static readonly RoutedEvent FloatingContainerIsOpenChangedEvent;
		public static readonly DependencyProperty CloseOnEscapeProperty;
		public static readonly DependencyProperty ShowModalProperty;
		public static readonly DependencyProperty ShowActivatedProperty;
		public static readonly DependencyProperty IsMaximizedProperty;
		public static readonly DependencyProperty DialogResultProperty;
		public static readonly DependencyProperty DialogOwnerProperty;
		public static readonly DependencyProperty LogicalOwnerProperty;
		public static readonly DependencyProperty ShowCloseButtonProperty;
		public static readonly DependencyProperty SizeToContentProperty;		
		static FloatingContainer() {
			Type ownerType = typeof(FloatingContainer);
			IsClosedPropertyKey = DependencyProperty.RegisterReadOnly("IsClosed", typeof(bool), ownerType, new UIPropertyMetadata(false));
			IsClosedProperty = IsClosedPropertyKey.DependencyProperty;
			IsActiveProperty = DependencyProperty.RegisterAttached("IsActive", typeof(bool), typeof(FloatingContainer),
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, null));
			FloatingContainerProperty = DependencyProperty.RegisterAttached("FloatingContainer", typeof(FloatingContainer), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			ContainerStartupLocationProperty = DependencyProperty.Register("ContainerStartupLocation", typeof(WindowStartupLocation), ownerType,
							new FrameworkPropertyMetadata(WindowStartupLocation.Manual, FrameworkPropertyMetadataOptions.Inherits));
			ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			AllowMovingProperty = DependencyProperty.Register("AllowMoving", typeof(bool), ownerType, new UIPropertyMetadata(true));
			AllowSizingProperty = DependencyProperty.Register("AllowSizing", typeof(bool), ownerType, new UIPropertyMetadata(true));
			AllowShowAnimationsProperty = DependencyProperty.Register("AllowShowAnimations", typeof(bool), ownerType, new UIPropertyMetadata(true));
			CaptionProperty = DependencyProperty.Register("Caption", typeof(string), ownerType, new UIPropertyMetadata(string.Empty));
			ContainerFocusableProperty = DependencyProperty.Register("ContainerFocusable", typeof(bool), ownerType, new PropertyMetadata(true));
			IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), ownerType, new UIPropertyMetadata(null));
			HiddenEvent = EventManager.RegisterRoutedEvent("Hidden", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			HidingEvent = EventManager.RegisterRoutedEvent("Hiding", RoutingStrategy.Direct, typeof(CancelRoutedEventHandler), ownerType);
			FloatingContainerIsOpenChangedEvent = EventManager.RegisterRoutedEvent("FloatingContainerIsOpenChanged", RoutingStrategy.Bubble, typeof(FloatingContainerEventHandler), ownerType);
			CloseOnEscapeProperty = DependencyProperty.Register("CloseOnEscape", typeof(bool), ownerType, new PropertyMetadata(false));
			ShowModalProperty = DependencyProperty.Register("ShowModal", typeof(bool), ownerType, new PropertyMetadata(false));
			ShowActivatedProperty = DependencyProperty.Register("ShowActivated", typeof(bool), ownerType, new PropertyMetadata(Window.ShowActivatedProperty.DefaultMetadata.DefaultValue));
			IsMaximizedProperty = DependencyProperty.RegisterAttached("IsMaximized", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, null));
			DialogResultProperty = DependencyProperty.Register("DialogResult", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			DialogOwnerProperty = DependencyProperty.RegisterAttached("DialogOwner", typeof(IDialogOwner), ownerType, new FrameworkPropertyMetadata(null));
			LogicalOwnerProperty = DependencyProperty.Register("LogicalOwner", typeof(FrameworkElement), ownerType, new FrameworkPropertyMetadata(null));
			ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), ownerType,
				new PropertyMetadata(true, (dObj, e) => ((FloatingContainer)dObj).OnShowCloseButtonChanged()));
			SizeToContentProperty = DependencyProperty.Register("SizeToContent", typeof(SizeToContent), typeof(FloatingContainer),
				new PropertyMetadata(SizeToContent.Manual, new PropertyChangedCallback(OnSizeToContentChanged)));
		}		
		public static void CloseDialog(FrameworkElement dialogContent, bool? dialogResult) {
			IDialogOwner dialogOwner = GetDialogOwner(dialogContent);
			if(dialogOwner != null) {
				dialogOwner.CloseDialog(dialogResult);
			}
			else {
				Window window = Window.GetWindow(dialogContent);
				window.DialogResult = dialogResult;
				window.Close();
			}
		}
		public static void AddFloatingContainerIsOpenChangedHandler(DependencyObject dObj, FloatingContainerEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(FloatingContainerIsOpenChangedEvent, handler);
		}
		public static void RemoveFloatingContainerIsOpenChangedHandler(DependencyObject dObj, FloatingContainerEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(FloatingContainerIsOpenChangedEvent, handler);
		}
		public static IDialogOwner GetDialogOwner(DependencyObject obj) {
			return (IDialogOwner)obj.GetValue(DialogOwnerProperty);
		}
		public static void SetDialogOwner(DependencyObject obj, IDialogOwner value) {
			obj.SetValue(DialogOwnerProperty, value);
		}
		public static bool GetIsMaximized(DependencyObject obj) {
			return (bool)obj.GetValue(IsMaximizedProperty);
		}
		public static void SetIsMaximized(DependencyObject obj, bool value) {
			obj.SetValue(IsMaximizedProperty, value);
		}
		public static void SetFloatingContainer(DependencyObject obj, FloatingContainer value) {
			obj.SetValue(FloatingContainerProperty, value);
		}
		public static FloatingContainer GetFloatingContainer(DependencyObject obj) {
			return (FloatingContainer)obj.GetValue(FloatingContainerProperty);
		}
		public static bool GetIsActive(DependencyObject obj) {
			return (bool)obj.GetValue(IsActiveProperty);
		}
		public static void SetIsActive(DependencyObject obj, bool value) {
			obj.SetValue(IsActiveProperty, value);
		}
		static void OnSizeToContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((FloatingContainer)d).OnSizeToContentChangedCore((SizeToContent)e.NewValue);
		}
		#endregion static
		protected override void OnHierarchyCreated() {
			if(ShowModal) CreateModalAdorner();
			SetFloatingContainer(ContentContainer, this);
			SetFloatingContainer(Presenter, this);
			SetIsMaximized(Presenter, false); 
		}
		public virtual void Activate() { }
		public Point GetPosition(MouseEventArgs e) {
			if(ContentContainer == null) return new Point(-10000, -10000);
			return e.GetPosition(ContentContainer);
		}
		protected override ManagedContentPresenter CreateContentPresenter() {
			return new ThemedManagedContentPresenter(this);
		}
		public bool CloseOnEscape {
			get { return (bool)GetValue(CloseOnEscapeProperty); }
			set { SetValue(CloseOnEscapeProperty, value); }
		}
		public bool ShowModal {
			get { return (bool)GetValue(ShowModalProperty); }
			set { SetValue(ShowModalProperty, value); }
		}
		public WindowStartupLocation ContainerStartupLocation {
			get { return (WindowStartupLocation)GetValue(ContainerStartupLocationProperty); }
			set { SetValue(ContainerStartupLocationProperty, value); }
		}
		public bool IsClosed {
			get { return (bool)GetValue(IsClosedProperty); }
			private set { SetValue(IsClosedPropertyKey, value); }
		}
		public bool? DialogResult {
			get { return (bool?)GetValue(DialogResultProperty); }
			set { SetValue(DialogResultProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public bool AllowMoving {
			get { return (bool)GetValue(AllowMovingProperty); }
			set { SetValue(AllowMovingProperty, value); }
		}
		public bool AllowSizing {
			get { return (bool)GetValue(AllowSizingProperty); }
			set { SetValue(AllowSizingProperty, value); }
		}
		public bool AllowShowAnimations {
			get { return (bool)GetValue(AllowShowAnimationsProperty); }
			set { SetValue(AllowShowAnimationsProperty, value); }
		}
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		public bool ContainerFocusable {
			get { return (bool)GetValue(ContainerFocusableProperty); }
			set { SetValue(ContainerFocusableProperty, value); }
		}
		public ImageSource Icon {
			get { return (ImageSource)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}
		public bool ShowActivated {
			get { return (bool)GetValue(ShowActivatedProperty); }
			set { SetValue(ShowActivatedProperty, value); }
		}
		public event RoutedEventHandler Hidden {
			add { AddHandler(HiddenEvent, value); }
			remove { RemoveHandler(HiddenEvent, value); }
		}
		public event CancelRoutedEventHandler Hiding {
			add { AddHandler(HidingEvent, value); }
			remove { RemoveHandler(HidingEvent, value); }
		}
		public FrameworkElement LogicalOwner {
			get { return (FrameworkElement)GetValue(LogicalOwnerProperty); }
			set { SetValue(LogicalOwnerProperty, value); }
		}
		public SizeToContent SizeToContent {
			get { return (SizeToContent)GetValue(SizeToContentProperty); }
			set { SetValue(SizeToContentProperty, value); }
		}
		public bool ShowCloseButton {
			get { return (bool)GetValue(ShowCloseButtonProperty); }
			set { SetValue(ShowCloseButtonProperty, value); }
		}	   
		protected virtual void OnShowCloseButtonChanged() {
			if(Presenter != null)
				Presenter.UpdatePresenter();
		}
		protected virtual void OnSizeToContentChangedCore(SizeToContent newVal) { }
		public void ResetSizing() {
			canUpdateAutoSize = true;
			UpdateAutoSize();
		}
		public void UpdateAutoSize() {
			if(!canUpdateAutoSize) return;
			Dispatcher.BeginInvoke(
					new Action(UpdateAutoSizeCore),
					System.Windows.Threading.DispatcherPriority.Render
				);
		}
		public void UpdateAutoSize(Action restoreSizeHandler, Action allowResizingHandler) {
			Dispatcher.BeginInvoke(
					new Action(delegate(){
						UpdateAutoSizeCore();
						if(restoreSizeHandler != null) {
							restoreSizeHandler();
							isAutoSizeUpdating++;
							UpdateFloatingBoundsCore(new Rect(FloatLocation, FloatSize));
							isAutoSizeUpdating--;
							allowResizingHandler();
						}
					}),
					System.Windows.Threading.DispatcherPriority.Render
				);
		}	   
		protected int isAutoSizeUpdating = 0;
		void UpdateAutoSizeCore() {
			isAutoSizeUpdating++;
			UpdateFloatingBoundsCore(new Rect(FloatLocation, FloatSize));
			EnsureMinSize();
			isAutoSizeUpdating--;
		}
		int isSizing = 0;
		protected double MeasureAutoSize(double size, double autoSize, double realSize) {
			if(isSizing > 0) return size;
			double max = (isAutoSizeUpdating > 0 || double.IsNaN(realSize)) ? double.MaxValue : realSize;
			double min = (isAutoSizeUpdating > 0) ? 0 : size;
			return Math.Min(max, Math.Max(autoSize, min));
		}
		protected void EnsureMinSize() {
			if(ReadLocalValue(MinWidthProperty) == DependencyProperty.UnsetValue) MinWidth = 100;
			if(ReadLocalValue(MinHeightProperty) == DependencyProperty.UnsetValue) MinHeight = 100;
		}
		protected Size GetLayoutAutoSize() {
			UIElement element = Presenter as UIElement;
			if(element != null) {
				element.InvalidateMeasure();
				if(SizeToContent == SizeToContent.WidthAndHeight) {
					element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
					return element.DesiredSize;
				}
				else {
					Size size = new Size(
						(SizeToContent == SizeToContent.Width) ? double.PositiveInfinity : FloatSize.Width,
						(SizeToContent == SizeToContent.Height) ? double.PositiveInfinity : FloatSize.Height);
					element.Measure(size);
					if(SizeToContent == SizeToContent.Width)
						return new Size(element.DesiredSize.Width, FloatSize.Height);
					if(SizeToContent == SizeToContent.Height)
						return new Size(FloatSize.Width, element.DesiredSize.Height);
				}
			}
			return Size.Empty;
		}
		protected bool InvertRTLSizeMove {
			get { return (Owner != null) && (Owner.FlowDirection == FlowDirection) && FlowDirection == System.Windows.FlowDirection.RightToLeft; }
		}
		protected bool InvertRTLMove {
			get { return (Owner != null) && (Owner.FlowDirection != storedFlowDirrection); }
		}
		protected void CalcOffsets(DXWindowActiveResizeParts activePart, double absWChange, double absHChange, out double dx, out double dy, out double sx, out double sy) {
			dx = 0; dy = 0;
			sx = 0; sy = 0;
			switch(activePart) {
				case DXWindowActiveResizeParts.Top:
					CalcTopOffset(absHChange, ref dy, ref sy);
					break;
				case DXWindowActiveResizeParts.Bottom:
					CalcBottomOffset(absHChange, ref dy, ref sy);
					break;
				case DXWindowActiveResizeParts.Left:
					CalcLeftOffset(absWChange, ref dx, ref sx);
					break;
				case DXWindowActiveResizeParts.Right:
					CalcRightOffset(absWChange, ref dx, ref sx);
					break;
				case DXWindowActiveResizeParts.BottomRight:
					CalcBottomOffset(absHChange, ref dy, ref sy);
					CalcRightOffset(absWChange, ref dx, ref sx);
					break;
				case DXWindowActiveResizeParts.BottomLeft:
					CalcBottomOffset(absHChange, ref dy, ref sy);
					CalcLeftOffset(absWChange, ref dx, ref sx);
					break;
				case DXWindowActiveResizeParts.TopRight:
					CalcTopOffset(absHChange, ref dy, ref sy);
					CalcRightOffset(absWChange, ref dx, ref sx);
					break;
				case DXWindowActiveResizeParts.TopLeft:
					CalcTopOffset(absHChange, ref dy, ref sy);
					CalcLeftOffset(absWChange, ref dx, ref sx);
					break;
			}
		}
		protected virtual void CalcRightOffset(double absWChange, ref double dx, ref double sx) {
			sx = absWChange;
		}
		protected virtual void CalcBottomOffset(double absHChange, ref double dy, ref double sy) {
			sy = absHChange;
		}
		protected virtual void CalcLeftOffset(double absWChange, ref double dx, ref double sx) {
			dx = absWChange;
			sx = -absWChange;
		}
		protected virtual void CalcTopOffset(double absHChange, ref double dy, ref double sy) {
			dy = absHChange;
			sy = -absHChange;
		}
		public virtual Point CorrectRightToLeftLocation(Point location) {
			return location;
		}
		public Size ActualSize { get; protected set; }
		protected int lockInversion = 0;
		void ProcessMoving(DragDeltaEventArgs e) {
			if(!AllowMoving || (ModalContainer != null && ModalContainer != this)) return;
			double dx = e.HorizontalChange;
			if(lockInversion == 0 && InvertRTLMove)
				dx = -dx;
			OnLocationChanged(new Point(FloatLocation.X + dx, FloatLocation.Y + e.VerticalChange));
		}
		void ProcessSizing(DragDeltaEventArgs e) {
			if(!AllowSizing || (ModalContainer != null && ModalContainer != this)) return;
			isSizing++;
			Point change = ScreenToLogical(new Point(e.HorizontalChange, e.VerticalChange));
			Size newSize = new Size(
				ChangeValueForRange(ActualSize.Width, change.X, MinWidth, MaxWidth),
				ChangeValueForRange(ActualSize.Height, change.Y, MinHeight, MaxHeight));
			canUpdateAutoSize = false;
			OnSizeChanged(newSize);
			isSizing--;
		}
		protected virtual Point ScreenToLogical(Point point) {
			return point;
		}
		bool canUpdateAutoSize = true;
		protected void ProcessHiding() {
			if(ModalContainer != null && ModalContainer != this) return;
			canUpdateAutoSize = false;
			CancelRoutedEventArgs e = new CancelRoutedEventArgs(HidingEvent);
			RaiseEvent(e);
			IsClosingCanceled = e.Cancel;
			if(!e.Cancel)
				OnHided();
		}
		protected void ProcessClosing() {
			if(IsDialogContent(Content))
				CloseDialog((FrameworkElement)Content, null);
			else
				ProcessHiding();
		}
		static bool IsDialogContent(object content) {
			FrameworkElement dialogContent = content as FrameworkElement;
			return (dialogContent != null) && GetDialogOwner(dialogContent) != null;
		}
		static double ChangeValueForRange(double val, double offset, double min, double max) {
			double newVal = val + offset;
			newVal = newVal < min ? Math.Min(min, Math.Max(val, newVal)) : newVal;
			newVal = max < newVal ? Math.Max(max, Math.Min(val, newVal)) : newVal;
			return newVal;
		}
		protected static double CheckWChange(double x, double wChange, Rect constraints, Thickness threshold) {
			if(constraints.Width * constraints.Height != 0) {
				double left = x + wChange;
				if(left + threshold.Right > constraints.Right) left -= (left + threshold.Right - constraints.Right);
				if(left - threshold.Left < constraints.Left) left -= (left - threshold.Left - constraints.Left);
				return left - x;
			}
			return wChange;
		}
		protected static double CheckHChange(double y, double hChange, Rect constraints, Thickness threshold) {
			if(constraints.Width * constraints.Height != 0) {
				double top = y + hChange;
				if(top + threshold.Bottom > constraints.Bottom) top -= (top + threshold.Bottom - constraints.Bottom);
				if(top - threshold.Top < constraints.Top) top -= (top - threshold.Top - constraints.Top);
				return top - y;
			}
			return hChange;
		}
		protected static Point CheckLocation(Point location, Rect constraints, Thickness threshold) {
			if(constraints.Width * constraints.Height != 0) {
				double left = location.X;
				double top = location.Y;
				if(left + threshold.Right > constraints.Right) left -= (left + threshold.Right - constraints.Right);
				if(top + threshold.Bottom > constraints.Bottom) top -= (top + threshold.Bottom - constraints.Bottom);
				if(left - threshold.Left < constraints.Left) left -= (left - threshold.Left - constraints.Left);
				if(top - threshold.Top < constraints.Top) top -= (top - threshold.Top - constraints.Top);
				return new Point(left, top);
			}
			return location;
		}
		protected virtual void OnLocationChanged(Point newLocation) {
			FloatLocation = newLocation;
		}
		protected virtual void OnSizeChanged(Size newSize) {
			FloatSize = newSize;
		}
		protected bool IsClosingCanceled { get; private set; }
		protected virtual void OnHided() {
			IsOpen = false;
		}
		int closing = 0;
		public void Close() {
			if(closing > 0) return;
			closing++;
			try {
				CloseCore();
			}
			finally { closing--; }
		}
		protected virtual void CloseCore() {
			ProcessHiding();
			IsClosed = !IsOpen;
		}
		protected override void OnContentUpdated(object content, FrameworkElement owner) { }
		protected override void OnIsOpenChanged(bool isOpen) {
			FloatingContainerClosedException.CheckFloatingContainerIsNotClosed(this);
			if(!IsAlive) return;
			if(ShowModal) {
				if(isOpen) ShowModalAdorner();
				else HideModalAdorner();
			}
			UpdateIsOpenCore(isOpen);
		}
		protected override void NotifyIsOpenChanged(bool isOpen) {
			if(LogicalOwner != null)
				LogicalOwner.RaiseEvent(new FloatingContainerEventArgs(this) { RoutedEvent = FloatingContainer.FloatingContainerIsOpenChangedEvent });
			if(!isOpen) RaiseEvent(new RoutedEventArgs(HiddenEvent));
		}
		internal static ModalAdorner FindModalAdorner(UIElement owner) {
			AdornerLayer layer = AdornerHelper.FindAdornerLayer(owner);
			Adorner[] adorners = new Adorner[] { };
			if(layer != null)
				adorners = layer.GetAdorners(owner);
			return (adorners != null) ? (ModalAdorner)Array.Find(
					adorners, (adorner) => adorner is ModalAdorner
				) : null;
		}
		void CreateModalAdorner() {
			ModalAdorner modalAdorner = FindModalAdorner(Owner);
			if(modalAdorner == null) {
				AdornerLayer adornerLayer = AdornerHelper.FindAdornerLayer(Owner);
				if(adornerLayer != null) {
					modalAdorner = new ModalAdorner(Owner);
					adornerLayer.Add(modalAdorner);
				}
			}
		}
		protected abstract FloatingMode GetFloatingMode();
		protected static FloatingContainer ModalContainer { get; private set; }
		public static bool IsModalContainerOpened { get { return ModalContainer != null; } }
		void ShowModalAdorner() {
			ModalContainer = this;
			ModalAdorner modalAdorner = FindModalAdorner(Owner);
			if(modalAdorner != null) {
				modalAdorner.Visibility = Visibility.Visible;
				modalAdorner.LockPreviewEvents(GetFloatingMode());
			}
		}
		void HideModalAdorner() {
			ModalAdorner modalAdorner = FindModalAdorner(Owner);
			if(modalAdorner != null) {
				modalAdorner.UnLockPreviewEvents();
				modalAdorner.Visibility = Visibility.Hidden;
			}
			ModalContainer = null;
		}
		protected override void OnFloatingBoundsChanged(Rect bounds) {
			if(IsAlive) UpdateFloatingBoundsCore(bounds);
		}
		public bool DeactivateOnClose { get; set; }
		internal bool UseScreenCoordinates { get; set; }
		#region IDialogOwner members
		public virtual void CloseDialog(bool? dialogResult) {
			DialogResult = dialogResult;
			Close();
		}
		#endregion
		#region Internal Classes
		[TemplatePart(Name = "PART_DragWidget", Type = typeof(Thumb))]
		[TemplatePart(Name = "PART_SizeGrip", Type = typeof(Thumb))]
		[TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
		[TemplatePart(Name = "PART_ContainerContent", Type = typeof(ContentPresenter))]
		[TemplatePart(Name = "PART_StatusPanel", Type = typeof(StackPanel))]
		protected class ThemedManagedContentPresenter : ManagedContentPresenter, IWindowResizeHelperClient {
			protected Thumb DragWidget { get; private set; }
			protected Thumb SizeGrip { get; private set; }
			protected Button CloseButton { get; private set; }
			protected StackPanel StatusPanel { get; private set; }
			protected FrameworkElement ContainerContent { get; private set; }
			public ThemedManagedContentPresenter(FloatingContainer container)
				: base(container) {
			}
			public FloatingContainer FloatingContainer { get { return Container as FloatingContainer; } }
			public override void OnApplyTemplate() {
				base.OnApplyTemplate();
				HideButton("PART_Minimize");
				HideButton("PART_Maximize");
				HideButton("PART_Restore");
				UpdateCloseButtonVisibility();
				this.DelayedExecute(() => { SubscribeEvents(); });
			}
			void SubscribeEvents() {
				if(Container.UseActiveStateOnly) SetValue(FloatingContainer.IsActiveProperty, true);
				else SetBinding(FloatingContainer.IsActiveProperty, new Binding("IsActive") { Source = LayoutHelper.FindRoot(this), Mode = BindingMode.OneWay });
				DragWidget = (Thumb)GetTemplateChild("PART_DragWidget");
				if(DragWidget != null)
					DragWidget.DragDelta += (o, e) => FloatingContainer.ProcessMoving(e);
				SizeGrip = (Thumb)GetTemplateChild("PART_SizeGrip");
				if(SizeGrip != null)
					SizeGrip.DragDelta += (o, e) => FloatingContainer.ProcessSizing(e);
				WindowResizeHelper.Subscribe(this);
				StatusPanel = (StackPanel)GetTemplateChild("PART_StatusPanel");
				if(StatusPanel != null)
					StatusPanel.FlowDirection = FlowDirection.LeftToRight;
				CloseButton = (Button)GetTemplateChild("PART_CloseButton");
				if(CloseButton != null) {
					CloseButton.Click += (o, e) => FloatingContainer.ProcessClosing();
					UpdateCloseButtonVisibility();
				}
				ContainerContent = (FrameworkElement)GetTemplateChild("PART_ContainerContent");
				if(ContainerContent != null)
					ContainerContent.SetBinding(ContentPresenter.ContentTemplateProperty,
						new Binding() { Source = Container, Path = new PropertyPath(FloatingContainer.ContentTemplateProperty) });
			}
			protected internal override void UpdatePresenter() {
				base.UpdatePresenter();
				if(FloatingContainer == null) return;
				if(CloseButton != null) {
					UpdateCloseButtonVisibility();
				}
			}
			void UpdateCloseButtonVisibility() {
				CloseButton = (Button)GetTemplateChild("PART_CloseButton");
				if(CloseButton == null || FloatingContainer == null) return;
				CloseButton.Visibility = FloatingContainer.ShowCloseButton ? Visibility.Visible : Visibility.Collapsed;
			}
			void HideButton(String id) {
				Button button = (Button)GetTemplateChild(id);
				if(button != null)
					button.Visibility = Visibility.Collapsed;
			}
			#region IWindowResizeHelperClient Members
			FrameworkElement IWindowResizeHelperClient.GetVisualByName(string name) {
				using(IEnumerator<DependencyObject> enumerator = new VisualTreeEnumeratorWithConditionalStop(this, null)) {
					while(enumerator.MoveNext()) {
						DependencyObject dObj = enumerator.Current;
						if((string)dObj.GetValue(FrameworkElement.NameProperty) == name)
							return dObj as FrameworkElement;
					}
				}
				return null;
			}
			void IWindowResizeHelperClient.ActivePartMouseDown(object sender, MouseButtonEventArgs e) {
				FrameworkElement fe = sender as FrameworkElement;
				if(fe == null) return;
				fe.CaptureMouse();
				fe.MouseMove += fe_MouseMove;
				fe.MouseUp += fe_MouseUp;
				prevAbsPos = GetAbsolutePosition(e.GetPosition(this));
				e.Handled = true;
			}
			void fe_MouseUp(object sender, MouseButtonEventArgs e) {
				FrameworkElement fe = sender as FrameworkElement;
				if(fe == null) return;
				ResetSizing(fe);
			}
			void ResetSizing(FrameworkElement fe) {
				fe.MouseMove -= fe_MouseMove;
				fe.MouseUp -= fe_MouseUp;
				fe.ReleaseMouseCapture();
			}
			Point prevAbsPos;
			Point GetAbsolutePosition(Point point) {
				if(FloatingContainer is FloatingAdornerContainer || BrowserInteropHelper.IsBrowserHosted)
					return GetRootVisualPosition(point);
				return GetScreenPosition(point);
			}
			Point GetRootVisualPosition(Point point) {
				UIElement rootVisual = LayoutHelper.FindRoot(this) as UIElement;
				return TranslatePoint(point, rootVisual);
			}
			Point GetScreenPosition(Point point) {
				return PointToScreen(point);
			}
			void fe_MouseMove(object sender, MouseEventArgs e) {
				FrameworkElement fe = sender as FrameworkElement;
				if(fe == null) return;
				if(e.MouseDevice.LeftButton != MouseButtonState.Pressed) {
					ResetSizing(fe);
					return;
				}
				string name = fe.Name.Substring(fe.Name.LastIndexOf("_") + 1);
				DXWindowActiveResizeParts activePart = (DXWindowActiveResizeParts)Enum.Parse(typeof(DXWindowActiveResizeParts), name);
				activePart = WindowResizeHelper.CorrectResizePart(FlowDirection, activePart);
				Point absPos = GetAbsolutePosition(e.GetPosition(this));
				if(absPos != prevAbsPos) {
					double absWChange = absPos.X - prevAbsPos.X;
					double absHChange = absPos.Y - prevAbsPos.Y;
					double dx; double dy; double sx; double sy;
					FloatingContainer.CalcOffsets(activePart, absWChange, absHChange, out dx, out dy, out sx, out sy);
					Size fSizeBefore = FloatingContainer.FloatSize;
					FloatingContainer.ProcessSizing(new DragDeltaEventArgs(sx, sy));
					Size fSizeAfter = FloatingContainer.FloatSize;
					if(fSizeBefore.Width == 0 || fSizeBefore.Height == 0) fSizeBefore = fSizeAfter;
					double realWSizeChange = fSizeAfter.Width - fSizeBefore.Width;
					double realHSizeChange = fSizeAfter.Height - fSizeBefore.Height;
					dx = Math.Abs(realWSizeChange) * Math.Sign(dx);
					dy = Math.Abs(realHSizeChange) * Math.Sign(dy);
					FloatingContainer.lockInversion++;
					if(dx != 0 || dy != 0) FloatingContainer.ProcessMoving(new DragDeltaEventArgs(FloatingContainer.InvertRTLSizeMove ? -dx : dx, dy));
					FloatingContainer.lockInversion--;
					prevAbsPos.X = absPos.X + (realWSizeChange == 0 && dx == 0 ? (realWSizeChange - absWChange) : 0);
					prevAbsPos.Y = absPos.Y + (realHSizeChange == 0 && dy == 0 ? (realHSizeChange - absHChange) : 0);
					e.Handled = true;
				}
			}
			#endregion
		}
		#endregion
		#region ModalBorder
		public class ModalAdorner : AdornerContainer {
			readonly UIElement topContainer;
			public ModalAdorner(UIElement adornedElement)
				: base(adornedElement, new Border() { Background = Brushes.Transparent }) {
				Visibility = Visibility.Hidden;
				Subscriptions = new PreviewSubscriptions();
				topContainer = LayoutHelper.GetTopContainerWithAdornerLayer(AdornedElement);
			}
			PreviewSubscriptions Subscriptions;
			public void LockPreviewEvents(FloatingMode mode) {
				UIElement previewRoot = LayoutHelper.GetRoot(AdornedElement as FrameworkElement);
				if(!BrowserInteropHelper.IsBrowserHosted && mode == FloatingMode.Window)
					Subscriptions.Add(previewRoot);
				LogicalEnumerator enumerator = new LogicalEnumerator(previewRoot);
				while(enumerator.MoveNext()) {
					FloatingContainer container = enumerator.Current as FloatingContainer;
					if(container != null && container != FloatingContainer.ModalContainer) 
						Subscriptions.Add(container.ContentContainer);
				}
			}
			public void UnLockPreviewEvents() {
				Subscriptions.Clear();
			}
			protected override Size ArrangeOverride(Size finalSize) {
				if(Visibility == System.Windows.Visibility.Visible) {
					var adornedElementRect = LayoutHelper.GetRelativeElementRect(AdornedElement, topContainer);
					Child.Arrange(new Rect(new Point(-adornedElementRect.Left, -adornedElementRect.Top), topContainer.RenderSize));
				}
				return finalSize;
			}
			#region inner classes
			class LogicalEnumerator : LogicalTreeEnumerator {
				internal LogicalEnumerator(DependencyObject root)
					: base(root) {
				}
				protected override System.Collections.IEnumerator GetNestedObjects(object obj) {
					foreach(object logicalChild in LogicalTreeHelper.GetChildren((DependencyObject)obj)) {
						if(!(logicalChild is DependencyObject)) continue;
						yield return logicalChild;
					}
				}
			}
			class PreviewSubscriptions : IEnumerable<UIElement>{
				IList<UIElement> List;
				public PreviewSubscriptions() {
					List = new List<UIElement>();
				}
				public void Add(UIElement element) {
					if(element != null && !List.Contains(element)) {
						List.Add(element);
						element.PreviewMouseDown += OnPreviewMouseDown;
						element.PreviewKeyDown += OnPreviewKeyDown;
					}
				}
				public void Clear() {
					foreach(UIElement element in List) {
						element.PreviewMouseDown -= OnPreviewMouseDown;
						element.PreviewKeyDown -= OnPreviewKeyDown;
					}
					List.Clear();
				}
				public IEnumerator<UIElement> GetEnumerator() {
					return List.GetEnumerator();
				}
				System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
					return List.GetEnumerator();
				}
				void OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
					e.Handled = true;
				}
				void OnPreviewKeyDown(object sender, KeyEventArgs e) {
					e.Handled = true;
				}
			}
			#endregion inner classes
		}
		#endregion
	}
	public static class FloatingContainerFactory {
#if DEBUGTEST
		public static FloatingMode? ForcedFloatingMode = null;
#endif
		public static DataTemplate FindContainerTemplate(FrameworkElement container) {
#if DXWINDOW
			return null;
#else
			return (DataTemplate)container.FindResource(
					new FloatingContainerThemeKeyExtension() { ResourceKey = FloatingContainerThemeKey.FloatingContainerTemplate }
				);
#endif
		}
		static FloatingMode CheckOwnerMode(DependencyObject owner, FloatingMode mode) {
			return (owner != null && Window.GetWindow(owner) != null) ? mode : FloatingMode.Adorner;
		}
		public static FloatingMode CheckPopupHost(DependencyObject owner) {
			FrameworkElement popupRoot = LayoutHelper.FindRoot(owner) as FrameworkElement;
			if((popupRoot != null) && (popupRoot.Parent is Popup))
				return FloatingMode.Popup;
			return FloatingMode.Window;
		}
		public static FloatingContainer CreateWithOwner(FloatingMode mode, DependencyObject owner) {
			FloatingMode actualMode = BrowserInteropHelper.IsBrowserHosted ?
				FloatingMode.Adorner : CheckOwnerMode(owner, mode);
			if(mode == FloatingMode.Popup)
				return new PopupFloatingContainer();
			return (actualMode == FloatingMode.Adorner) ? (FloatingContainer)new FloatingAdornerContainer() : (FloatingContainer)new FloatingWindowContainer();
		}
		public static FloatingContainer Create(FloatingMode mode) {
#if DEBUGTEST
			if(ForcedFloatingMode != null)
				mode = ForcedFloatingMode.Value;
#endif
			FloatingMode actualMode = BrowserInteropHelper.IsBrowserHosted ? FloatingMode.Adorner : mode;
			if(mode == FloatingMode.Popup)
				return new PopupFloatingContainer();
			return (actualMode == FloatingMode.Adorner) ? (FloatingContainer)new FloatingAdornerContainer() :
				(FloatingContainer)new FloatingWindowContainer();
		}
	}
	public class FloatingContainerHeaderPanel : Border {
		public static readonly DependencyProperty EnableLayoutCorrectionProperty = DependencyProperty.RegisterAttached("EnableLayoutCorrection", typeof(bool), typeof(FloatingContainerHeaderPanel),
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
		public static bool GetEnableLayoutCorrection(DependencyObject obj) { return (bool)obj.GetValue(EnableLayoutCorrectionProperty); }
		public static void SetEnableLayoutCorrection(DependencyObject obj, bool value) { obj.SetValue(EnableLayoutCorrectionProperty, value); }
		protected override Size MeasureOverride(Size availableSize) {
			Size baseSize = base.MeasureOverride(availableSize);
			return GetEnableLayoutCorrection(this) ? new Size(0, baseSize.Height) : baseSize;
		}
	}
	public class PopupFloatingContainer : FloatingContainer {
		static PopupFloatingContainer() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupFloatingContainer), new FrameworkPropertyMetadata(typeof(PopupFloatingContainer)));
		}
		public Popup Popup { get; private set; }
		protected override UIElement CreateContentContainer() {
			Popup = new Popup() { Placement = PlacementMode.Relative, AllowsTransparency = true, PlacementTarget = Owner };
			return Popup;
		}
		public override Point CorrectRightToLeftLocation(Point location) {
			return new Point(location.X + FloatSize.Width, location.Y);
		}
		int lockFloatingBoundsChanging = 0;
		protected override void UpdateFloatingBoundsCore(Rect bounds) {
			if(lockFloatingBoundsChanging > 0) return;
			lockFloatingBoundsChanging++;
			try {
				bounds = CorrectBounds(new Rect(bounds.Location, EnsureAutoSize(bounds.Size)));
				ActualSize = bounds.Size;
				SetPopupBounds(bounds);
			}
			finally {
				lockFloatingBoundsChanging--;
			}
		}
		void SetPopupBounds(Rect bounds) {
			Popup.HorizontalOffset = bounds.X;
			Popup.VerticalOffset = bounds.Y;
			Popup.Width = bounds.Width;
			Popup.Height = bounds.Height;
		}
		Rect CorrectBounds(Rect bounds) {
			if(Owner == null || PresentationSource.FromVisual(Owner) == null)
				return bounds;
			Point location = UseScreenCoordinates ? Owner.PointFromScreen(bounds.Location) : bounds.Location;
			return new Rect(location, bounds.Size);
		}
		Size EnsureAutoSize(Size size) {
			if(SizeToContent != System.Windows.SizeToContent.Manual) {
				Size autoSize = GetLayoutAutoSize();
				if(autoSize != Size.Empty) {
					double w = autoSize.Width; double h = autoSize.Height;
					double realW = (Popup.Width == 0) ? double.NaN : Popup.Width;
					double realH = (Popup.Height == 0) ? double.NaN : Popup.Height;
					if(SizeToContent == System.Windows.SizeToContent.Width)
						w = MeasureAutoSize(size.Width, w, realW);
					if(SizeToContent == System.Windows.SizeToContent.Height)
						h = MeasureAutoSize(size.Height, h, realH);
					if(SizeToContent == System.Windows.SizeToContent.WidthAndHeight) {
						w = MeasureAutoSize(size.Width, w, realW);
						h = MeasureAutoSize(size.Height, h, realH);
					}
					size = new Size(w, h);
					if(FloatSize != size) {
						if(FloatSize != new Size(0, 0))
							UpdateFloatSize(size);
						if(realW != w)
							Popup.Width = w;
						if(realH != h)
							Popup.Height = h;
					}
				}
			}
			return size;
		}
		protected override void UpdateIsOpenCore(bool isOpen) {
			if(isOpen) {
				isAutoSizeUpdating++;
				try {
					Show();
				}
				finally {
					OnOpened();
					--isAutoSizeUpdating;
				}
			}
			else Hide();
		}
		protected void Show() {
			Popup.IsOpen = true;
		}
		protected void Hide() {
			Popup.IsOpen = false;
		}
		protected void OnOpened() {
			UpdateStartupLocation();
			EnsureMinSize();
		}
		protected virtual void UpdateStartupLocation() {
			if(ContainerStartupLocation == WindowStartupLocation.Manual || ContainerStartupLocation == WindowStartupLocation.CenterScreen)
				return;
			ActualSize = EnsureAutoSize(FloatSize);
			Point startupLocation = new Point((Owner.ActualWidth - ActualSize.Width) * 0.5, (Owner.ActualHeight - ActualSize.Height) * 0.5);
			UpdateFloatingBoundsCore(new Rect(startupLocation, FloatSize));
			if(FloatLocation != startupLocation)
				UpdateFloatLocation(startupLocation);
		}
		void UpdateFloatLocation(Point floatLocation) {
			lockFloatingBoundsChanging++;
			try {
				FloatLocation = floatLocation;
			}
			finally {
				lockFloatingBoundsChanging--;
			}
		}
		void UpdateFloatSize(Size floatSize) {
			lockFloatingBoundsChanging++;
			try {
				FloatSize = floatSize;
			}
			finally {
				lockFloatingBoundsChanging--;
			}
		}
		protected override void AddDecoratorToContentContainer(NonLogicalDecorator decorator) {
			Popup.Child = decorator;
		}
		protected override FloatingMode GetFloatingMode() {
			return FloatingMode.Popup;
		}
		protected override bool IsAlive {
			get { return Popup != null; }
		}
		protected override void CloseCore() {
			base.CloseCore();
			Popup = null;
		}
	}
	public static class LogicalTreeIntruder {
		static UIElement fakeVisualParent = new UIElement();
		public static void AddLogicalChild(FrameworkElement logicalParent, UIElement element) {
			new UIElementCollectionIntruder(fakeVisualParent, logicalParent).AddLogicalChild(element);
		}
		public static void RemoveLogicalChild(UIElement element) {
			FrameworkElement logicalParent = LogicalTreeHelper.GetParent(element) as FrameworkElement;
			new UIElementCollectionIntruder(fakeVisualParent, logicalParent).RemoveLogicalChild(element);
		}
		#region inner classes
		class UIElementCollectionIntruder : UIElementCollection {
			public UIElementCollectionIntruder(UIElement visualParent, FrameworkElement logicalParent)
				: base(visualParent, logicalParent) {
			}
			public void AddLogicalChild(UIElement element) { 
				SetLogicalParent(element); 
			}
			public void RemoveLogicalChild(UIElement element) { 
				ClearLogicalParent(element); 
			}
		}
		#endregion inner classes
	}
}
