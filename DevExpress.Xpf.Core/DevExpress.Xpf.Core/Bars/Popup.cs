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
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using System.Globalization;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using System.Windows.Interop;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Mvvm.Native;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Editors;
using DevExpress.Utils.Serializing;
namespace DevExpress.Xpf.Bars {
	public interface IPopupControl {
		void ShowPopup(UIElement control);
		void ClosePopup();
		bool IsPopupOpen { get; }
		event EventHandler Closed;
		event EventHandler Opened;
		BarPopupBase Popup { get; }
		WeakReference ContextElement { get; set; }
	}
	public interface IPopupOwner {
		bool IsPopupOpen { get; }
		void ShowPopup();
		void ClosePopup();
		void ClosePopup(bool ignoreSetMenuMode);
		bool IsOnBar { get; }
		bool ActAsDropdown { get; }
		IPopupControl Popup { get; }
	}
	public interface IApplicationMenu {
		UIElement GetRightPaneContainer();
	}	
	public enum PopupMenuItemsDisplayMode { Default, SmallImagesText, LargeImagesText, LargeImagesTextDescription }
	public enum PopupItemClickBehaviour {
		None = 0x0,
		ClosePopup = 0x1,
		CloseChildren = 0x2,
		CloseCurrentBranch = 0x4,
		CloseAllPopups = ClosePopup | CloseChildren | CloseCurrentBranch | 0x8,
		Undefined = 0x100,
	}
	public enum MenuDropAlignmentBehaviour { Default, Ignore, UseSystemValue }
	public abstract class BarPopupBase : PopupBase, IPopupControl, IMultipleElementRegistratorSupport {
		#region static
		public static readonly DependencyProperty DefaultVerticalOffsetProperty;
		public static readonly DependencyProperty IgnoreMenuDropAlignmentProperty;		
		static BarPopupBase() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarPopupBase), new FrameworkPropertyMetadata(typeof(BarPopupBase)));
			NameProperty.OverrideMetadata(typeof(BarPopupBase), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None, (d, e) => ((BarPopupBase)d).OnNameChanged((string)e.OldValue, (string)e.NewValue)));
			IsOpenProperty.OverrideMetadata(typeof(BarPopupBase), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsOpenChanged), new CoerceValueCallback(OnIsOpenCoerce)));
			EventManager.RegisterClassHandler(typeof(UIElement), ContextMenuOpeningEvent, new ContextMenuEventHandler(ContextMenuManager.OnContextMenuOpening), false);
			EventManager.RegisterClassHandler(typeof(UIElement), PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(ContextMenuManager.OnLeftClickContextMenuOpening), false);
			EventManager.RegisterClassHandler(typeof(FrameworkElement), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
			EventManager.RegisterClassHandler(typeof(BarPopupBase), Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnPreviewMouseOutside));
			EventManager.RegisterClassHandler(typeof(BarPopupBase), ButtonBase.ClickEvent, new RoutedEventHandler((d,e)=>((BarPopupBase)d).OnItemClick(e.OriginalSource, null)));
			HorizontalOffsetProperty.OverrideMetadata(typeof(BarPopupBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback((o, e) => ((BarPopupBase)o).OnHorizontalOffsetChanged(e)), new CoerceValueCallback((o, e) => ((BarPopupBase)o).OnHorizontalOffsetChangedCoerce(e))));
			PlacementProperty.OverrideMetadata(typeof(BarPopupBase), new FrameworkPropertyMetadata(PlacementMode.Bottom, null, new CoerceValueCallback((o, e) => ((BarPopupBase)o).OnPlacementChangedCoerce(e))));
			PlacementTargetProperty.OverrideMetadata(typeof(BarPopupBase), new FrameworkPropertyMetadata(null, (d, e) => ((BarPopupBase)d).OnPlacementTargetChanged(e)));
			IgnoreMenuDropAlignmentProperty = DependencyProperty.Register("IgnoreMenuDropAlignment", typeof(DefaultBoolean), typeof(BarPopupBase), new FrameworkPropertyMetadata(DefaultBoolean.Default, OnIgnoreMenuDropAlignmentPropertyChanged));
			DefaultVerticalOffsetProperty = DependencyPropertyManager.Register("DefaultVerticalOffset", typeof(double), typeof(BarPopupBase), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));			
		}
		static void OnIgnoreMenuDropAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarPopupBase)d).OnIgnoreMenuDropAlignmentChanged((DefaultBoolean)e.OldValue);
		}
		protected internal virtual bool AttachToPlacementTargetWhenClosed { get { return true; } }
		protected virtual void OnIgnoreMenuDropAlignmentChanged(DefaultBoolean oldValue) { }
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean IgnoreMenuDropAlignment {
			get { return (DefaultBoolean)GetValue(IgnoreMenuDropAlignmentProperty); }
			set { SetValue(IgnoreMenuDropAlignmentProperty, value); }
		}
		protected virtual DefaultBoolean GetIngoreMenuDropAlignment() {
			if(IgnoreMenuDropAlignment == DefaultBoolean.Default) {
				BarItemLinkControl linkControl = Owner as BarItemLinkControl ?? OwnerLinkControl;
				if(linkControl != null && linkControl.IsLinkInApplicationMenu)
					return DefaultBoolean.True;
				return BarManager.IgnoreMenuDropAlignment ? DefaultBoolean.True : DefaultBoolean.False;
			}
			return IgnoreMenuDropAlignment;
		}
		double originHorizontalOffset = 0d;
		protected internal double HorizontalOffsetCore {
			get {
				return SystemParameters.MenuDropAlignment ? originHorizontalOffset : HorizontalOffset;
			}
			set {
				if(SystemParameters.MenuDropAlignment)
					originHorizontalOffset = value;
				else
					HorizontalOffset = value;
			}
		}
		private object OnPlacementChangedCoerce(object e) {
			if(!SystemParameters.MenuDropAlignment || GetIngoreMenuDropAlignment() == DefaultBoolean.False) return e;
			switch((PlacementMode)e) {
				case PlacementMode.Left:
					return PlacementMode.Right;
				case PlacementMode.Right:
					return PlacementMode.Left;
				default:
					return e;
			}
		}
		protected internal virtual object OnHorizontalOffsetChangedCoerce(object e) {
			double value = (double)e;
			originHorizontalOffset = value;
			value += GetAdditionalHorizontalOffset();
			return value;
		}
		protected void CheckCoerceHorizontalOffset() {
			if(HorizontalOffsetCore + GetAdditionalHorizontalOffset(true) != HorizontalOffset)
				CoerceValue(HorizontalOffsetProperty);
		}
		protected internal virtual double GetAdditionalHorizontalOffset(bool useArrangeSize = false) {
			if(GetIngoreMenuDropAlignment() == DefaultBoolean.False)
				return 0d;
			if(!SystemParameters.MenuDropAlignment || PlacementTarget == null || Child == null) return 0d;
			if(Placement == PlacementMode.Left || Placement == PlacementMode.Right) return 0d;
			if(!Child.IsMeasureValid && !useArrangeSize) {
				FrameworkElement fe = Child as FrameworkElement;
				if(fe != null) {
					fe.ApplyTemplate();
					fe.InvalidateMeasure();
					fe.Measure(SizeHelper.Infinite);
				}
			}
			double childWidth = (!useArrangeSize ? Child.RenderSize : Child.DesiredSize).Width;
			double placementTargetWidthNoMargin = (!useArrangeSize ? PlacementTarget.DesiredSize : PlacementTarget.RenderSize).Width
				- (this.PlacementTarget as FrameworkElement).Margin.Right
				- (this.PlacementTarget as FrameworkElement).Margin.Left;
			double result = 0d;
			switch(Placement) {
				case PlacementMode.Relative:
				case PlacementMode.RelativePoint:
				case PlacementMode.Bottom:
				case PlacementMode.Top:
					if(placementTargetWidthNoMargin == 0) return 0d;
					break;
				case PlacementMode.Center:
					placementTargetWidthNoMargin /= 2d;
					break;
				default:
					placementTargetWidthNoMargin = 0d;
					break;
			}
			result = Math.Abs(childWidth - placementTargetWidthNoMargin);
			return result;
		}
		protected internal virtual void OnHorizontalOffsetChanged(DependencyPropertyChangedEventArgs e) { }
		public static void ShowElementContextMenu(object contextMenuElement) {
			ContextMenuManager.ShowElementContextMenu(contextMenuElement);
		}
		protected internal bool AllowMouseCapturing;
		protected bool GetShouldCaptureMouse() {
			if (AllowMouseCapturing)
				return true;
			if (PopupMenuManager.GetParentPopup(this) != null)
				return false;
			return !BarNameScope.GetService<IEventListenerDecoratorService>(this).HasWindow;
		}
		static void OnLostMouseCapture(object sender, MouseEventArgs e) {
			(sender as PopupMenuBarControl).Do(p => p.OnFrameworkElementLostMouseCapture(e));
		}
		private static void OnPreviewMouseOutside(object sender, MouseButtonEventArgs e) {
			((BarPopupBase)sender).If(x => PopupMenuManager.IsInPopup(Mouse.Captured, x)).Do(x => x.OnPreviewMouseOutside(e));
		}
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseDown(e);
			if(GetShouldCaptureMouse() && StaysOpen) {
				e.Handled = PopupMenuManager.CloseAllPopups(null, e);
			}
		}
		static object OnIsOpenCoerce(DependencyObject d, object value) {
			BarPopupBase popup = (BarPopupBase)d;
			var bValue = (bool)value;			
			bool res = (bool)popup.OnIsOpenCoerce(bValue);
			if (res)
				popup.CoerceValue(HorizontalOffsetProperty);			
			popup.OnIsOpenPropertyCoerced(res);
			return res;
		}
		static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var popup = (BarPopupBase)d;
			TextEditorHelper.SetIsContextMenuOpen(popup.PlacementTarget, (bool)e.NewValue);
			popup.OnIsOpenChanged(e);
			if (!(bool)e.NewValue)
				popup.Dispatcher.BeginInvoke(new Action<BarPopupBase>(x => x.UpdatePlacementTargetRegistration(null, x.PlacementTarget)), DispatcherPriority.Input, popup);			
		}
		protected static UIElement GetMenuPlacementTarget(BarItemLinkControl linkControl) {
			if(!linkControl.IsLinkInApplicationMenu) return null;
			IApplicationMenu menu = linkControl.Link.Links.Holder as IApplicationMenu;
			if(menu == null) return null;
			return menu.GetRightPaneContainer();
		}
		protected static Point GetMenuRelativePoint(BarItemLinkControl linkControl, UIElement target) {
			return linkControl.TransformToVisual(target).Transform(new Point(linkControl.ActualWidth, linkControl.ActualHeight));
		}
		public static void UpdateSubMenuBounds(BarItemLinkControl linkControl, BarPopupBase p) {
			UIElement target = GetMenuPlacementTarget(linkControl);
			if(target == null) return;
			Point offset = GetMenuRelativePoint(linkControl, target);
			if(p.Placement == PlacementMode.Bottom) {
				p.VerticalOffset = -offset.Y;
				p.HorizontalOffset = linkControl.ActualWidth - offset.X;
			} else if(p.Placement == GetRightPlacementMode()) {
				p.VerticalOffset = linkControl.ActualHeight - offset.Y;
				if(((FrameworkElement)target).FlowDirection == FlowDirection.LeftToRight) {
					p.HorizontalOffset = -offset.X;
				} else {
					p.HorizontalOffset = target.RenderSize.Width - offset.X;
				}
			}
			BarPopupBorderControl popupBorder = p.Child as BarPopupBorderControl;
			double verticalMargin = 0;
			if(popupBorder != null)
				verticalMargin = popupBorder.Margin.Top + popupBorder.Margin.Bottom;
			p.Width = target.RenderSize.Width;
			p.Height = target.RenderSize.Height + verticalMargin;
		}
		internal static PlacementMode GetRightPlacementMode() {
			return SystemParameters.MenuDropAlignment ? PlacementMode.Left : PlacementMode.Right;
		}
		#endregion
		public BarPopupBase() {
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
			IsRoot = true;
			ItemClickBehaviour = PopupItemClickBehaviour.Undefined;
			PopupContent = CreatePopupContent();
			FocusManager.SetIsFocusScope(this, true);
			if ((PopupContent as DependencyObject) != null) {
				FocusManager.SetIsFocusScope(PopupContent as DependencyObject, true);
				if (PopupContent as FrameworkElement != null) {
					((FrameworkElement)PopupContent).Focusable = true;
				}
			}
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			if (OwnerLinkControl == null)
				BarNameScope.EnsureRegistrator(this);
		}
		internal BarPopupBorderControl BorderControl { get { return Child as BarPopupBorderControl; } }
		WeakReference IPopupControl.ContextElement { get; set; }
		protected internal virtual bool CanShowToolTip() { return false; }
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, true);
			UseLayoutRounding = true;
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, false);
		}
		protected virtual void OnPreviewMouseOutside(MouseButtonEventArgs e) {
			if(Mouse.Captured is FrameworkElement) {
				if (new Rect(new Point(), (Mouse.Captured as FrameworkElement).RenderSize).Contains(e.GetPosition(Mouse.Captured))) {
					return;
				}
			}			
			e.Handled = PopupMenuManager.CloseAllPopups(null, e);
		}
		protected override void Initialize() {
			base.Initialize();
			BarManagerHelper.SetPopup(Child, this);
			if (BorderControl != null) {
				BorderControl.MouseEnter += OnBorderControlMouseEnter;
			}			
		}
		protected virtual void OnBorderControlMouseEnter(object sender, MouseEventArgs e) {
			if(this==(e.OriginalSource as DependencyObject).With(BarManagerHelper.GetPopup)) {
				PopupMenuManager.CancelPopupClosing(this);
			}
		}
		protected virtual LinkContainerType GetLinkContainerType() { return LinkContainerType.Menu; }
		internal bool IsRoot { get; set; }
		protected internal bool IsBranchHeader { get; set; }
		protected internal PopupItemClickBehaviour ItemClickBehaviour { get; set; }
		protected internal bool StaysOpenOnOuterClick { get; set; }
		protected internal PopupItemClickBehaviour ActualItemClickBehaviour {
			get {
				PopupItemClickBehaviour result = PopupItemClickBehaviour.Undefined;
				if(OwnerLinkControl is BarSubItemLinkControl)
					result = ((BarSubItemLinkControl)OwnerLinkControl).ItemClickBehaviour;
				if(OwnerLinkControl is BarSplitButtonItemLinkControl)
					result = ((BarSplitButtonItemLinkControl)OwnerLinkControl).ItemClickBehaviour;
				if(result == PopupItemClickBehaviour.Undefined)
					result = ItemClickBehaviour;
				return result;
			}
		}								
		protected virtual void OnPlacementTargetChanged(DependencyPropertyChangedEventArgs e) {
			UpdatePlacementTargetRegistration(e.OldValue as UIElement, e.NewValue as UIElement);		 
		}
		Action<Popup> forceClose;
		protected void ForceClose() {
			(forceClose ?? (forceClose = ReflectionHelper.CreateInstanceMethodHandler<Popup, Action<Popup>>(null, "ForceClose", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)))(this);
		}
		void UpdatePlacementTargetRegistration(UIElement oldValue, UIElement newValue) {
			if(AttachToPlacementTargetWhenClosed) {
				LogicalTreeWrapper.UpdatePlacementTargetRegistration(this, oldValue, null);
				LogicalTreeWrapper.UpdatePlacementTargetRegistration(this, null, newValue);
			}
		}
		public virtual void ShowPopup(UIElement control) {			
			if(control == null)
				throw new NullReferenceException("control");
			PlacementTarget = control;
			UpdatePlacement(control);
			double childOpacity = 1d;
			if(!IsOpen && Child != null && SystemParameters.MenuDropAlignment) {
				childOpacity = Child.Opacity;
				Child.Opacity = 0;
			}
			SetCurrentValue(IsOpenProperty, true);
			if(Child != null && SystemParameters.MenuDropAlignment)
				Dispatcher.BeginInvoke(new Action(() => Child.Opacity = childOpacity));
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarPopupBaseDefaultVerticalOffset")]
#endif
		public double DefaultVerticalOffset {
			get { return (double)GetValue(DefaultVerticalOffsetProperty); }
			set { SetValue(DefaultVerticalOffsetProperty, value); }
		}
		protected internal double DefaultHorizontalOffset { get; set; }
		protected virtual void CloseChildren() {
			PopupMenuManager.CloseChildren(this);
		}
		protected virtual bool IsChildMenu(BarPopupBase popup) {
			if(popup == this)
				return false;
			while(popup != this && popup != null) {
				if(popup.OwnerLinkControl == null)
					return false;
				SubMenuBarControl subMenuControl = popup.OwnerLinkControl.LinksControl as SubMenuBarControl;
				if(subMenuControl == null)
					return false;
				popup = subMenuControl.Popup;
			}
			return popup != null;
		}
		internal bool isClosing = false;
		readonly Locker popupLocker = new Locker();
		protected internal Locker PopupLocker { get { return popupLocker; } }
		internal event EventHandler IsOpenChanged;
		void RaiseInternalIsOpenChanged() {
			if(IsOpenChanged != null)
				IsOpenChanged(this, null);
		}
		protected virtual void OnIsOpenChanged(DependencyPropertyChangedEventArgs e) {
			RaiseInternalIsOpenChanged();
			if (!(bool)e.NewValue) {
				if (!isClosing) {
					isClosing = true;
					PopupMenuManager.ClosePopup(this, true);
					CloseChildren();
				}
				if (!IsRoot)
					OnClosed(EventArgs.Empty);
				if (GetShouldCaptureMouse() && GetIsMouseCaptured() && StaysOpen)
					Mouse.Capture(null);
				isClosing = false;
			} else {
				PopupMenuManager.ShowPopup(this, true);
				Dispatcher.BeginInvoke(new Action(() => {
					if (PopupContent as DependencyObject != null)
						VisualEffectsInheritanceHelper.SetTextAndRenderOptions(PopupContent as DependencyObject, this);
				}), System.Windows.Threading.DispatcherPriority.Background);
				var focusedElement = FocusHelper.GetFocusedElement() as DependencyObject;
				if (focusedElement != null) {
					var edit = DevExpress.Xpf.Editors.BaseEdit.GetOwnerEdit(focusedElement as DependencyObject);
					if (edit != null) {
						edit.DoValidate();
						edit.ForceChangeDisplayText();
					}
				}
				CheckCoerceHorizontalOffset();
				if (GetShouldCaptureMouse())
					SetMouseCapture();
			}
			ContextMenuManager.OpenedMenu = (bool)e.NewValue ? this : null;
		}
		protected virtual bool GetIsMouseCaptured() {
			return Mouse.Captured == Child;
		}
		protected virtual void SetMouseCapture() {
			Mouse.Capture(Child, CaptureMode.SubTree);
		}
		protected internal virtual void OnItemClick(object sender, RoutedEventArgs e) {
			var oSource = e.With(x => x.OriginalSource as DependencyObject);
			if (oSource != null && BarManagerHelper.GetPopup(oSource) != this)
				return;
			if (sender is ButtonBase && !(sender is Button))
				return;
			PopupMenuManager.CloseAllPopups(sender, null);
		}
		protected virtual void UpdatePlacement(UIElement control) {
			if(control is BarQuickCustomizationButton) {
				UpdatePlacementForBarQuickCustomizationButton();
				return;
			}
			if(control is BarItemLinkControl) {
				HorizontalOffset = DefaultHorizontalOffset;
				VerticalOffset = DefaultVerticalOffset;
				Placement = !IsRoot ? PlacementMode.Right : BarManagerHelper.GetPopupPlacement((BarItemLinkControl)control);
			}
			if(control is BarSubItemLinkControl && ((BarSubItemLinkControl)control).IsLinkControlInMainMenu) {
				if(((BarSubItemLinkControl)control).LayoutPanel != null)
					HorizontalOffset = ((BarSubItemLinkControl)control).LayoutPanel.Margin.Left;
			}
			if(control is RadialMenuControl) {
				Placement = PlacementMode.Center;
				HorizontalOffset = 0;
				VerticalOffset = 0;
			}
		}
		protected virtual void UpdatePlacementForBarQuickCustomizationButton() {
			BarQuickCustomizationButton control = (BarQuickCustomizationButton)PlacementTarget;
			if(control == null)
				return;
			if(control.Orientation == Orientation.Vertical) {
				Placement = PlacementMode.Right;
				if(SystemParameters.MenuDropAlignment)
					VerticalOffset += ((BarQuickCustomizationButton)control).ActualHeight;
				else
					HorizontalOffset -= ((BarQuickCustomizationButton)control).ActualWidth;
			} else
				Placement = PlacementMode.Bottom;
		}
		protected internal void ShowPopup(UIElement control, bool isRoot) {
			IsRoot = isRoot;
			ShowPopup(control);
		}
		public virtual void ClosePopup() {
			SetCurrentValue(IsOpenProperty, false);
		}
		bool ignoreSetMenuMode = false;
		internal void ClosePopup(bool ignoreSetMenuMode) {
			this.ignoreSetMenuMode = ignoreSetMenuMode;
			ClosePopup();
		}
		public virtual DependencyObject Owner { get { return PlacementTarget; } }
		bool HasOwner { get { return Owner != null; } }
		protected virtual bool IsContextMenuMode {
			get {
				if(Owner == null)
					return true;
				if(Owner is BarSplitButtonItemLinkControl && ((BarSplitButtonItemLinkControl)Owner).Link != null) {
					if(((BarSplitButtonItemLinkControl)Owner).Link.Links.Holder is Bar) {
						return !((Bar)((BarSplitButtonItemLinkControl)Owner).Link.Links.Holder).IsMainMenu;
					}
					return true;
				}
				if(Owner is BarSubItemLinkControl && ((BarSubItemLinkControl)Owner).Link != null) {
					if(((BarSubItemLinkControl)Owner).Link.Links.Holder is Bar && !((Bar)((BarSubItemLinkControl)Owner).Link.Links.Holder).IsMainMenu) {
						return true;
					}
				}
				return LayoutHelper.FindParentObject<LinksControl>(Owner) == null;
			}
		}		
		protected override void OnOpened(EventArgs e) {
			ignoreSetMenuMode = false;
			if (HasOwner) {
				if (MenuControl != null) {
					if (MenuControl.OpenPopupsAsMenu)
						SetMenuMode(true);
				}
			}			
			base.OnOpened(e);
		}
		protected override void OnClosed(EventArgs e) {
			OnClosedCore();
			base.OnClosed(e);
		}
		protected virtual internal void OnClosedCore() {
			if (HasOwner && !ignoreSetMenuMode) {
				if (PopupMenuManager.IsAnyPopupOpened)
					SetMenuMode(false);
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			CloseOnEscapeKeyDown(e);
		}
		protected virtual void CloseOnEscapeKeyDown(KeyEventArgs e) {
			if (e.Key == Key.Escape) {
				if (IsOpen && !e.Handled) {
					ClosePopup(true);
					e.Handled = true;
				}
			}
		}
		protected internal void RestoreFocus() {
			Control contextElement = Owner as Control;
			if(contextElement != null)
				contextElement.Focus();
		}		
		protected internal BarItemLinkControl OwnerLinkControl { get; set; }
		protected internal LinksControl MenuControl {
			get { return Owner == null ? null : LayoutHelper.FindParentObject<LinksControl>(Owner); }
		}
		protected internal bool SetMenuMode(bool value) {
			if(IsContextMenuMode) return false;
			if(MenuControl != null) {				
				return true;
			}
			return false;
		}
		protected abstract object CreatePopupContent();
		bool IPopupControl.IsPopupOpen { get { return IsOpen; } }
		BarPopupBase IPopupControl.Popup { get { return this; } }
		protected internal virtual bool ContainsLinkControl(BarItemLinkControlBase linkControl) {
			return false;
		}
		protected virtual void OnIsOpenPropertyCoerced(bool value) {
			if(!value)
				return;
			if (Child is FrameworkElement)
				((FrameworkElement)Child).FlowDirection = ((LogicalTreeHelper.GetParent(this) as FrameworkElement) ?? (PlacementTarget as FrameworkElement)).Return(x => x.FlowDirection, () => FlowDirection.LeftToRight);
			PopupAnimation = BarManager.GetBarManager(this).Return(x => x.MenuAnimationType, () => PopupAnimation.None);
		}
		protected virtual object OnIsOpenCoerce(object value) {
			if (((IPopupControl)this).ContextElement != null && ((IPopupControl)this).ContextElement.Target != null)
				ContextMenuManager.SetBarManager((DependencyObject)((IPopupControl)this).ContextElement.Target, BarManagerHelper.GetOrFindBarManager((DependencyObject)((IPopupControl)this).ContextElement.Target));
			if ((bool)value) {
				if (!RaiseOpening()) return false;
			}
			return value;
		}
		protected virtual bool RaiseOpening() {
			CancelEventArgs e = new CancelEventArgs();
			if(Opening != null)
				Opening(this, e);
			return !e.Cancel;
		}
		public event CancelEventHandler Opening;
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { return GetRegistratorKeys(); }
		}		
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {			
			return GetRegistratorName(registratorKey);
		}
		protected virtual void OnNameChanged(string oldValue, string newValue) {
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(IFrameworkInputElement), oldValue, newValue);
		}
		protected virtual IEnumerable<object> GetRegistratorKeys() {
			yield return typeof(IFrameworkInputElement);
		}
		protected virtual object GetRegistratorName(object registratorKey) {
			if (Equals(typeof(IFrameworkInputElement), registratorKey))
				return Name;
			return null;
		}
	}
	internal delegate void ManagerChangedEventHandler(object sender, ManagerChangedEventArgs e);
	internal class ManagerChangedEventArgs : EventArgs {
		public BarManager OldManager { get; internal set; }
		public BarManager NewManager { get; internal set; }
	}
	public class BarPopupBorderControl : PopupBorderControl {
		static BarPopupBorderControl() {
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarPopupBorderControl), typeof(DevExpress.Xpf.Bars.Automation.PopupMenuAutomationPeer), owner => new DevExpress.Xpf.Bars.Automation.PopupMenuAutomationPeer((BarPopupBorderControl)owner));
		}
		public BarPopupBorderControl() {
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			DragManager.SetDropTargetFactory(this, new EmptyDropTargetFactory());
		}	
	}
	public enum BarPopupExpandMode {
		Classic,
		TabletOffice
	}
	public class ExpandablePanel : Panel {
		protected bool SizeAnimationActive { get { return OfficeTabletItemsControl.GetSizeAnimationInProgress(this); } }
		public ExpandablePanel() {
			ClipToBounds = true;
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size desiredSize = new Size();
			Size measureSize = new Size(double.PositiveInfinity, SizeAnimationActive ? double.PositiveInfinity : availableSize.Height);
			foreach(UIElement child in Children) {
				child.Measure(measureSize);
				desiredSize = child.DesiredSize;
			}
			return desiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			int count = InternalChildren.Count;
			for(int i = 0; i < count; i++) {
				UIElement child = InternalChildren[i];
				if(i == count - 1)
					child.Arrange(new Rect(finalSize));
				else
					child.Arrange(new Rect(child.DesiredSize));
			}
			return finalSize;
		}
	}
	[TemplatePart(Name = "PART_ItemsPresenter", Type = typeof(ItemsPresenter))]
	public class OfficeTabletItemsControl : ItemsControl {
		public static readonly DependencyProperty AddRemoveAnimationAccelerationRatioProperty;
		public static readonly DependencyProperty AddRemoveAnimationDecelerationRationProperty;
		public static readonly DependencyProperty AddRemoveAnimationDurationProperty;
		public static readonly DependencyProperty BackCommandProperty;
		public static readonly DependencyProperty BackCommandTextProperty;
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty SizeAnimationAccelerationRatioProperty;
		public static readonly DependencyProperty SizeAnimationBeginTimeProperty;
		public static readonly DependencyProperty SizeAnimationDecelerationRationProperty;
		public static readonly DependencyProperty SizeAnimationDurationProperty;
		public static readonly DependencyProperty SizeAnimationInProgressProperty;
		public static bool GetSizeAnimationInProgress(DependencyObject obj) {
			return (bool)obj.GetValue(SizeAnimationInProgressProperty);
		}
		public static void SetSizeAnimationInProgress(DependencyObject obj, bool value) {
			obj.SetValue(SizeAnimationInProgressProperty, value);
		}
		static OfficeTabletItemsControl() {
			Type ownerType = typeof(OfficeTabletItemsControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			AddRemoveAnimationAccelerationRatioProperty = DependencyProperty.Register("AddRemoveAnimationAccelerationRatio", typeof(double), ownerType, new PropertyMetadata(default(double)));
			AddRemoveAnimationDecelerationRationProperty = DependencyProperty.Register("AddRemoveAnimationDecelerationRation", typeof(double), ownerType, new PropertyMetadata(default(double)));
			AddRemoveAnimationDurationProperty = DependencyProperty.Register("AddRemoveAnimationDuration", typeof(Duration), ownerType, new FrameworkPropertyMetadata(default(Duration)));
			SizeAnimationAccelerationRatioProperty = DependencyProperty.Register("SizeAnimationAccelerationRatio", typeof(double), ownerType, new PropertyMetadata(default(double)));
			SizeAnimationBeginTimeProperty = DependencyProperty.Register("SizeAnimationBeginTime", typeof(TimeSpan), ownerType, new PropertyMetadata(TimeSpan.FromSeconds(0)));
			SizeAnimationDecelerationRationProperty = DependencyProperty.Register("SizeAnimationDecelerationRation", typeof(double), ownerType, new PropertyMetadata(default(double)));
			SizeAnimationDurationProperty = DependencyProperty.Register("SizeAnimationDuration", typeof(Duration), ownerType, new PropertyMetadata(default(Duration)));
			CaptionProperty = DependencyProperty.Register("Caption", typeof(object), ownerType, new PropertyMetadata(null));
			BackCommandProperty = DependencyProperty.Register("BackCommand", typeof(ICommand), ownerType, new PropertyMetadata(null));
			BackCommandTextProperty = DependencyProperty.Register("BackCommandText", typeof(object), ownerType, new PropertyMetadata(null));
			SizeAnimationInProgressProperty = DependencyProperty.RegisterAttached("SizeAnimationInProgress", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
		[TypeConverter(typeof(DoubleConverter))]
		public double AddRemoveAnimationAccelerationRatio {
			get { return (double)GetValue(AddRemoveAnimationAccelerationRatioProperty); }
			set { SetValue(AddRemoveAnimationAccelerationRatioProperty, value); }
		}
		[TypeConverter(typeof(DoubleConverter))]
		public double AddRemoveAnimationDecelerationRation {
			get { return (double)GetValue(AddRemoveAnimationDecelerationRationProperty); }
			set { SetValue(AddRemoveAnimationDecelerationRationProperty, value); }
		}
		[TypeConverter(typeof(DurationConverter))]
		public Duration AddRemoveAnimationDuration  {
			get { return (Duration)GetValue(AddRemoveAnimationDurationProperty); }
			set { SetValue(AddRemoveAnimationDurationProperty, value); }
		}
		public ICommand BackCommand {
			get { return (ICommand)GetValue(BackCommandProperty); }
			set { SetValue(BackCommandProperty, value); }
		}
		public object BackCommandText {
			get { return (object)GetValue(BackCommandTextProperty); }
			set { SetValue(BackCommandTextProperty, value); }
		}
		public object Caption {
			get { return (object)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		[TypeConverter(typeof(DoubleConverter))]
		public double SizeAnimationAccelerationRatio {
			get { return (double)GetValue(SizeAnimationAccelerationRatioProperty); }
			set { SetValue(SizeAnimationAccelerationRatioProperty, value); }
		}
		[TypeConverter(typeof(DoubleConverter))]
		public double SizeAnimationDecelerationRation {
			get { return (double)GetValue(SizeAnimationDecelerationRationProperty); }
			set { SetValue(SizeAnimationDecelerationRationProperty, value); }
		}
		[TypeConverter(typeof(DurationConverter))]
		public Duration SizeAnimationDuration {
			get { return (Duration)GetValue(SizeAnimationDurationProperty); }
			set { SetValue(SizeAnimationDurationProperty, value); }
		}
		[TypeConverter(typeof(TimeSpanConverter))]
		public TimeSpan SizeAnimationBeginTime {
			get { return (TimeSpan)GetValue(SizeAnimationBeginTimeProperty); }
			set { SetValue(SizeAnimationBeginTimeProperty, value); }
		}
		protected Storyboard AddAnimation {
			get {
				if(addAnimation == null)
					addAnimation = CreateBaseStoryboard(new EventHandler(OnAddAnimationCompleted));
				return addAnimation;
			}
		}
		protected DoubleAnimation AddAnimationPrevContent { get { return (DoubleAnimation)AddAnimation.Children[0]; } }
		protected DoubleAnimation AddAnimationNewContent { get { return (DoubleAnimation)AddAnimation.Children[1]; } }
		protected Storyboard RemoveAnimation {
			get {
				if(removeAnimation == null)
					removeAnimation = CreateBaseStoryboard(new EventHandler(OnRemoveAnimationCompleted));
				return removeAnimation;
			}
		}
		protected DoubleAnimation RemoveAnimationPrevContent { get { return (DoubleAnimation)RemoveAnimation.Children[1]; } }
		protected DoubleAnimation RemoveAnimationNewContent { get { return (DoubleAnimation)RemoveAnimation.Children[0]; } }
		protected Storyboard SizeAnimation {
			get {
				if(sizeAnimation == null)
					sizeAnimation = CreateSizeAnimation();
				return sizeAnimation;
			}
		}
		public OfficeTabletItemsControl() { }
		public void AddChildWithAnimation(object child) {
			if(Items.Contains(child))
				return;
			var prevContent = Items.Count > 0 ? Items[Items.Count - 1] as FrameworkElement : null;
			var newContent = child as FrameworkElement;
			if(newContent == null || prevContent == null)
				return;
			Width = ActualWidth;
			Height = ActualHeight;
			SetSizeAnimationInProgress(ItemsPresenter, true);
			newContent.Loaded += OnItemLoaded;
			Items.Add(newContent);
		}
		public void RemoveChildWithAnimation(object child) {
			if(!Items.Contains(child))
				return;
			if(Items.Count > 1) {
				var prevContent = child as FrameworkElement;
				var newContent = Items[Items.IndexOf(child) - 1] as FrameworkElement;
				AnimateRemoveChild(prevContent, newContent);
			} else
				Items.Remove(child);
		}
		public void Reset() {
			foreach(FrameworkElement child in Items) {
				if(child.RenderTransform.Value != Transform.Identity.Value)
					child.RenderTransform.BeginAnimation(TranslateTransform.XProperty, null);
			}
			Items.Clear();
			ClearSize();
			ClearValue(OfficeTabletItemsControl.BackCommandTextProperty);
			ClearValue(OfficeTabletItemsControl.CaptionProperty);
		}
		protected ItemsPresenter ItemsPresenter { get; private set; }
		protected FrameworkElement Header { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ItemsPresenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;
			Header = GetTemplateChild("header") as FrameworkElement;
		}
		protected virtual void OnItemLoaded(object sender, RoutedEventArgs e) {
			FrameworkElement newContent = e.OriginalSource as FrameworkElement;
			newContent.Loaded -= OnItemLoaded;
			FrameworkElement prevContent = ItemContainerGenerator.ContainerFromIndex(Items.IndexOf(newContent) - 1) as FrameworkElement;
			Storyboard.SetTarget(AddAnimationPrevContent, prevContent);
			AddAnimationPrevContent.To = -prevContent.ActualWidth;
			Storyboard.SetTarget(AddAnimationNewContent, newContent);
			AddAnimationNewContent.From = prevContent.ActualWidth;
			AddAnimation.Begin();
		}
		protected virtual void AnimateRemoveChild(FrameworkElement prevContent, FrameworkElement newContent) {
			Storyboard.SetTarget(RemoveAnimationNewContent, newContent);
			RemoveAnimationNewContent.From = -newContent.ActualWidth;
			Storyboard.SetTarget(RemoveAnimationPrevContent, prevContent);
			RemoveAnimationPrevContent.To = newContent.ActualWidth;
			Width = ActualWidth;
			Height = ActualHeight;
			SetSizeAnimationInProgress(ItemsPresenter, true);
			RemoveAnimation.Begin();
		}
		protected virtual void AnimateSize(Size newSize) {
			Header.Measure(SizeHelper.Infinite);
			var widthAnimation = SizeAnimation.Children[0] as DoubleAnimation;
			widthAnimation.To = Math.Max(Header.DesiredSize.Width, newSize.Width);
			var heightAnimation = SizeAnimation.Children[1] as DoubleAnimation;
			heightAnimation.To = Header.ActualHeight + newSize.Height;
			Storyboard.SetTarget(SizeAnimation, this);
			SizeAnimation.Begin();
		}
		protected virtual Storyboard CreateBaseStoryboard(EventHandler completedHandler) {
			Storyboard storyboard = new Storyboard();
			if(completedHandler != null)
				storyboard.Completed += completedHandler;
			DoubleAnimation prevContentAnimation = new DoubleAnimation();
			BindingOperations.SetBinding(prevContentAnimation, DoubleAnimation.DurationProperty, CreateAnimantionBinding("AddRemoveAnimationDuration"));
			BindingOperations.SetBinding(prevContentAnimation, DoubleAnimation.AccelerationRatioProperty, CreateAnimantionBinding("AddRemoveAnimationAccelerationRatio"));
			BindingOperations.SetBinding(prevContentAnimation, DoubleAnimation.DecelerationRatioProperty, CreateAnimantionBinding("AddRemoveAnimationDecelerationRation"));
			prevContentAnimation.FillBehavior = FillBehavior.HoldEnd;
			Storyboard.SetTargetProperty(prevContentAnimation, new PropertyPath("RenderTransform.X"));
			storyboard.Children.Add(prevContentAnimation);
			DoubleAnimation newContentAnimation = new DoubleAnimation();
			BindingOperations.SetBinding(newContentAnimation, DoubleAnimation.DurationProperty, CreateAnimantionBinding("AddRemoveAnimationDuration"));
			BindingOperations.SetBinding(newContentAnimation, DoubleAnimation.AccelerationRatioProperty, CreateAnimantionBinding("AddRemoveAnimationAccelerationRatio"));
			BindingOperations.SetBinding(newContentAnimation, DoubleAnimation.DecelerationRatioProperty, CreateAnimantionBinding("AddRemoveAnimationDecelerationRation"));
			newContentAnimation.FillBehavior = FillBehavior.HoldEnd;
			Storyboard.SetTargetProperty(newContentAnimation, new PropertyPath("RenderTransform.X"));
			storyboard.Children.Add(newContentAnimation);
			newContentAnimation.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
			prevContentAnimation.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
			return storyboard;
		}
		protected virtual Storyboard CreateSizeAnimation() {
			var storyboard = new Storyboard();
			storyboard.Completed += OnSizeAnimationCompleted;
			DoubleAnimation widthAnimation = new DoubleAnimation();
			BindingOperations.SetBinding(widthAnimation, DoubleAnimation.BeginTimeProperty, CreateAnimantionBinding("SizeAnimationBeginTime"));
			BindingOperations.SetBinding(widthAnimation, DoubleAnimation.DurationProperty, CreateAnimantionBinding("SizeAnimationDuration"));
			BindingOperations.SetBinding(widthAnimation, DoubleAnimation.AccelerationRatioProperty, CreateAnimantionBinding("SizeAnimationAccelerationRatio"));
			BindingOperations.SetBinding(widthAnimation, DoubleAnimation.DecelerationRatioProperty, CreateAnimantionBinding("SizeAnimationDecelerationRation"));
			widthAnimation.FillBehavior = FillBehavior.Stop;
			Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("Width"));
			DoubleAnimation heightAnimation = new DoubleAnimation();
			BindingOperations.SetBinding(heightAnimation, DoubleAnimation.BeginTimeProperty, CreateAnimantionBinding("SizeAnimationBeginTime"));
			BindingOperations.SetBinding(heightAnimation, DoubleAnimation.DurationProperty, CreateAnimantionBinding("SizeAnimationDuration"));
			BindingOperations.SetBinding(heightAnimation, DoubleAnimation.AccelerationRatioProperty, CreateAnimantionBinding("SizeAnimationAccelerationRatio"));
			BindingOperations.SetBinding(heightAnimation, DoubleAnimation.DecelerationRatioProperty, CreateAnimantionBinding("SizeAnimationDecelerationRation"));
			heightAnimation.FillBehavior = FillBehavior.Stop;
			Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("Height"));
			storyboard.Children.Add(widthAnimation);
			storyboard.Children.Add(heightAnimation);
			widthAnimation.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
			heightAnimation.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
			return storyboard;
		}
		protected virtual void OnAddAnimationCompleted(object sender, EventArgs e) {
			if(Items.Count == 0)
				ClearSize();
			else {
				FrameworkElement newContent = ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as FrameworkElement;
				AnimateSize(newContent.DesiredSize);
			}
		}
		protected virtual void OnRemoveAnimationCompleted(object sender, EventArgs e) {
			if(Items.Count > 1)
				Items.RemoveAt(Items.Count - 1);
			if(Items.Count > 0) {
				FrameworkElement last = ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as FrameworkElement;
				AnimateSize(last.DesiredSize);
			} else
				ClearSize();
		}
		protected virtual void OnSizeAnimationCompleted(object sender, EventArgs e) {
			ClearSize();
		}
		protected void ClearSize() {
			SetSizeAnimationInProgress(ItemsPresenter, false);
			BeginAnimation(FrameworkElement.WidthProperty, null);
			BeginAnimation(FrameworkElement.HeightProperty, null);
			ClearValue(FrameworkElement.WidthProperty);
			ClearValue(FrameworkElement.HeightProperty);
		}
		protected BindingBase CreateAnimantionBinding(string bindingPath) {
			Binding bind = new Binding(bindingPath);
			bind.Source = this;
			bind.Mode = BindingMode.OneWay;
			return bind;
		}
		Storyboard addAnimation, removeAnimation, sizeAnimation;
	}
	public abstract class PopupOpenStrategy {
		public static PopupOpenStrategy Create(BarPopupExpandable owner) {
			if(owner.ExpandMode == BarPopupExpandMode.TabletOffice) {
				return new OfficeTabletPopupOpenStrategy(owner);
			}
			return new ClassicPopupOpenStrategy(owner);
		}
		public BarPopupExpandable Owner { get; private set; }
		protected PopupOpenStrategy(BarPopupExpandable owner) {
			Owner = owner;
		}
		public abstract bool OnShow(UIElement control);
		public abstract void OnClose();
		internal virtual void OnChildShowing(BarPopupBase popup) { }
		internal virtual void OnChildClosing(BarPopupBase popup) {
			popup.OnClosedCore();
		}
	}
	public class ClassicPopupOpenStrategy : PopupOpenStrategy {
		public ClassicPopupOpenStrategy(BarPopupExpandable owner) : base(owner) { }
		public override bool OnShow(UIElement control) { return true; }
		public override void OnClose() { }
	}
	public class OfficeTabletPopupOpenStrategy : PopupOpenStrategy {
		protected Stack<BarPopupBase> SubPopups {
			get {
				if(subPopups == null) {
					subPopups = new Stack<BarPopupBase>();
				}
				return subPopups;
			}
		}
		protected BarPopupExpandable TopPopup { get; private set; }
		protected OfficeTabletItemsControl ItemsControl { get; private set; }
		public OfficeTabletPopupOpenStrategy(BarPopupExpandable owner) : base(owner) {
			ItemsControl = new OfficeTabletItemsControl();
			ItemsControl.BackCommand = DelegateCommandFactory.Create(CloseLastChild);
			ItemsControl.SizeChanged += OnItemsControlSizeChanged;
		}
		public override bool OnShow(UIElement control) {
			TopPopup = (GetTopPopup(control)) as BarPopupExpandable;
			if(TopPopup == null) {
				AttachItemsControl();
				UpdateItemsControlHeader(Owner, null);
				return true;
			} else {
				TopPopup.ShowChild(Owner);
				return false;
			}
		}
		public override void OnClose() {
			ItemsControl.Reset();
			if(TopPopup == null) {
				foreach(var popup in SubPopups) {
					AttachContent(popup);
					popup.OnClosedCore();
				}
				SubPopups.Clear();
			} else
				TopPopup.CloseChild(Owner);
			TopPopup = null;
			AttachContent(Owner);
		}
		public void CloseLastChild() {
			OnChildClosing(GetLastChild());
		}
		internal override void OnChildShowing(BarPopupBase popup) {
			if(!SubPopups.Contains(popup)) {
				UpdateItemsControlHeader(popup, (SubPopups.LastOrDefault() ?? Owner));
				((ContentControl)popup.Child).Content = null;
				SubPopups.Push(popup);
				ItemsControl.AddChildWithAnimation(popup.PopupContent);
			}
			base.OnChildShowing(popup);
		}
		internal override void OnChildClosing(BarPopupBase popup) {
			ItemsControl.RemoveChildWithAnimation(popup.PopupContent);
			SubPopups.Pop();
			BarPopupBase current = (SubPopups.LastOrDefault() ?? Owner);
			BarPopupBase previous = null;
			if(SubPopups.Count > 0) {
				previous = SubPopups.Count > 1 ? SubPopups.Peek() : Owner;
			}
			UpdateItemsControlHeader(current, previous);
			AttachContent(popup);
			base.OnChildClosing(popup);
		}
		protected virtual void AttachContent(BarPopupBase popup) {
			((PopupBorderControl)popup.Child).Content = popup.PopupContent;
		}
		protected virtual void AttachItemsControl() {
			((PopupBorderControl)Owner.Child).Content = ItemsControl;
			var content = Owner.PopupContent;
			if(!ItemsControl.Items.Contains(content)) {
				ItemsControl.Items.Add(content);
			}
		}
		protected BarPopupBase GetLastChild() {
			return SubPopups.LastOrDefault();
		}
		protected virtual BarPopupBase GetTopPopup(UIElement owner) {
			BarPopupBase element = null;
			var linkControl = LayoutHelper.FindParentObject<BarItemLinkControl>(owner) ?? owner;
			while (linkControl != null && LayoutHelper.FindLayoutOrVisualParentObject<BarPopupBase>(linkControl, true) != null) {
				element = LayoutHelper.FindLayoutOrVisualParentObject<BarPopupBase>(linkControl, true);
				linkControl = element.With(elem => elem.OwnerLinkControl);
			}
			return element;
		}
		protected virtual void OnItemsControlSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateOffset();
		}
		protected virtual void UpdateItemsControlHeader(BarPopupBase currentPopup, BarPopupBase previousPopup) {
			if(currentPopup == null || currentPopup.OwnerLinkControl == null)
				ItemsControl.ClearValue(OfficeTabletItemsControl.CaptionProperty);
			else
				ItemsControl.Caption = currentPopup.OwnerLinkControl.ActualContent;
			if(previousPopup == null || previousPopup.OwnerLinkControl == null)
				ItemsControl.ClearValue(OfficeTabletItemsControl.BackCommandTextProperty);
			else
				ItemsControl.BackCommandText = previousPopup.OwnerLinkControl.ActualContent;
		}
		protected virtual void UpdateOffset() {
			if (Owner.Placement == PlacementMode.MousePoint)
				return;
			var target = Owner.PlacementTarget;
			double horizOffset = -(Owner.Child.RenderSize.Width / 2 - target.RenderSize.Width / 2);
			if(SystemParameters.MenuDropAlignment)
				horizOffset *= -1;
			Owner.HorizontalOffset = horizOffset;
		}
		Stack<BarPopupBase> subPopups;
	}
	public abstract class BarPopupExpandable : BarPopupBase {
		#region static
		public static readonly DependencyProperty ExpandModeProperty;
		static BarPopupExpandable() {
			ExpandModeProperty = DependencyProperty.Register("ExpandMode", typeof(BarPopupExpandMode), typeof(BarPopupExpandable), new PropertyMetadata(BarPopupExpandMode.Classic, OnExpandModePropertyChanged));
		}
		static void OnExpandModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarPopupExpandable)d).OnExpandModeChanged((BarPopupExpandMode)e.OldValue);
		}
		#endregion
		public BarPopupExpandMode ExpandMode {
			get { return (BarPopupExpandMode)GetValue(ExpandModeProperty); }
			set { SetValue(ExpandModeProperty, value); }
		}
		public BarPopupExpandable() : base() {
			OpenStrategy = CreateOpenStrategy();
		}
		protected PopupOpenStrategy OpenStrategy { get; private set; }
		protected virtual void OnExpandModeChanged(BarPopupExpandMode oldValue) {
			OpenStrategy = CreateOpenStrategy();
		}
		protected virtual PopupOpenStrategy CreateOpenStrategy() {
			return PopupOpenStrategy.Create(this);
		}
		internal void ShowChild(BarPopupBase popup) {
			OpenStrategy.OnChildShowing(popup);
		}
		internal void CloseChild(BarPopupBase popup) {
			OpenStrategy.OnChildClosing(popup);
		}
		protected override object OnIsOpenCoerce(object value) {
			UpdateExpandMode(); 
			var result = (bool)base.OnIsOpenCoerce(value);
			if(result) {
				result = OpenStrategy.OnShow(OwnerLinkControl);
			}
			return result;
		}
		protected override void OnIsOpenChanged(DependencyPropertyChangedEventArgs e) {
			if(!(bool)e.NewValue) {
				OpenStrategy.OnClose();
			}
			base.OnIsOpenChanged(e);
		}
		protected override void UpdatePlacement(UIElement control) {
			if(ExpandMode != BarPopupExpandMode.TabletOffice)
				base.UpdatePlacement(control);
		}
		protected override PopupBorderControl CreateBorderControl() {
			OpenStrategy = CreateOpenStrategy();
			return new BarPopupBorderControl();
		}
		protected void UpdateExpandMode() {
			if(OwnerLinkControl != null) {
				BarPopupExpandable top = BarManagerHelper.GetPopup(OwnerLinkControl) as BarPopupExpandable;
				if(top != null && top != this) {
					SetCurrentValue(ExpandModeProperty, top.ExpandMode);
				}
			}
		}
	}
	public class PopupMenuBase : BarPopupExpandable {
		#region static
		public static readonly DependencyProperty ItemsDisplayModeProperty;
		public static readonly DependencyProperty StretchRowsProperty;
		public static readonly DependencyProperty MaxRowCountProperty;
		public static readonly DependencyProperty MultiColumnProperty;
		public static readonly DependencyProperty MaxColumnHeightProperty;
		static PopupMenuBase() {
			ItemsDisplayModeProperty = DependencyPropertyManager.Register("ItemsDisplayMode", typeof(PopupMenuItemsDisplayMode), typeof(PopupMenuBase), new FrameworkPropertyMetadata(PopupMenuItemsDisplayMode.Default, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnItemsDisplayModePropertyChanged)));
			StretchRowsProperty = DependencyPropertyManager.Register("StretchRows", typeof(bool), typeof(PopupMenuBase), new FrameworkPropertyMetadata(true));
			MaxRowCountProperty = DependencyPropertyManager.Register("MaxRowCount", typeof(int), typeof(PopupMenuBase), new FrameworkPropertyMetadata(int.MaxValue));
			MultiColumnProperty = DependencyPropertyManager.Register("MultiColumn", typeof(bool), typeof(PopupMenuBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
			MaxColumnHeightProperty = DependencyPropertyManager.Register("MaxColumnHeight", typeof(double), typeof(PopupMenuBase), new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
		protected static void OnItemsDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PopupMenuBase)d).OnItemsDisplayModeChanged(e);
		}
		#endregion
		public PopupMenuBase()
			: base() {
			ItemClickBehaviour = PopupItemClickBehaviour.CloseAllPopups;
		}
		protected override object CreatePopupContent() {
			var popupContent = new PopupMenuBarControl(this);
			popupContent.ContainerType = GetLinkContainerType();
			return popupContent;
		}
		protected override void SetMouseCapture() {
			Mouse.Capture((IInputElement)PopupContent, CaptureMode.SubTree);
		}
		protected override bool GetIsMouseCaptured() {
			return Equals(PopupContent, Mouse.Captured);			
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupMenuBaseItemsDisplayMode")]
#endif
		public PopupMenuItemsDisplayMode ItemsDisplayMode {
			get { return (PopupMenuItemsDisplayMode)GetValue(ItemsDisplayModeProperty); }
			set { SetValue(ItemsDisplayModeProperty, value); }
		}
		public double MaxColumnHeight {
			get { return (double)GetValue(MaxColumnHeightProperty); }
			set { SetValue(MaxColumnHeightProperty, value); }
		}
		public int MaxRowCount {
			get { return (int)GetValue(MaxRowCountProperty); }
			set { SetValue(MaxRowCountProperty, value); }
		}
		public bool MultiColumn {
			get { return (bool)GetValue(MultiColumnProperty); }
			set { SetValue(MultiColumnProperty, value); }
		}
		public bool StretchRows {
			get { return (bool)GetValue(StretchRowsProperty); }
			set { SetValue(StretchRowsProperty, value); }
		}
		protected internal PopupMenuBarControl ContentControl { get { return PopupContent as PopupMenuBarControl; } }
		protected internal virtual void OnPreviewLinkControlClick(object sender, RoutedEventArgs e) {
			OnPreviewItemClicked(sender);
		}
		protected internal virtual void OnLinkControlClick(object sender, RoutedEventArgs e) {
			OnItemClick(sender, e);
		}		
		protected internal virtual void OnLinkControlIsSelectedChanged(object sender, RoutedEventArgs e) {
			BarItemLinkControl item = sender as BarItemLinkControl;
			OnLinkControlSelectedChangedCore(sender, e, item.IsSelected);
		}
		protected internal virtual void OnLinkControlIsHighlightedChanged(object sender, RoutedEventArgs e) {
		}
		protected virtual void OnLinkControlSelectedChangedCore(object sender, RoutedEventArgs e, bool newValue) {
			BarItemLinkControl item = sender as BarItemLinkControl;
			if(item != null) {
				e.SetHandled(true);
			}
		}
		protected virtual void CloseSubMenu(BarItemLinkControl item) {
			if(item == null)
				return;
			item.IsSelected = false;
			item.IsHighlighted = false;
			item.IsPressed = false;
			var iowner = item as IPopupOwner;
			if(iowner != null && iowner.IsPopupOpen) {
				PopupMenuManager.ClosePopup(iowner.Popup.With(x => x.Popup), false);
			}
		}
		protected override void OnOpened(EventArgs e) {
			base.OnOpened(e);
			using (ContentControl.NavigationManager.Lock()) {
				NavigationTree.StartNavigation(ContentControl);
			}
		}
		protected override object OnIsOpenCoerce(object value) {
			ContentControl.OnIsOpenCoerce(value);
			object result = base.OnIsOpenCoerce(value);
			if(!(bool)result) return false;
			if(ContentControl != null && !HasVisibleItems()) return false;
			return true;
		}
		protected virtual bool HasVisibleItems() {
			foreach(BarItemLinkInfo linkInfo in ContentControl.Items) {
				if(linkInfo.LinkBase.ActualIsVisible)
					return true;
			}
			return false;
		}
		protected override void OnIsOpenChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsOpenChanged(e);
			ContentControl.OnIsOpenChanged(e);
		}
		void OnPreviewItemClicked(object originalSource) { }
		BarItemLinkControl currentItem;
		protected internal BarItemLinkControl CurrentItem {
			get { return currentItem; }
			set {
				if(currentItem != null) {
					currentItem.IsSelected = currentItem.IsHighlighted = false;
				}
				currentItem = value;
				if(currentItem != null) {
					if(currentItem is BarEditItemLinkControl)
						((BarEditItemLinkControl)currentItem).focusEditorOnIsSelected = false;
					currentItem.IsSelected = currentItem.IsHighlighted = true;
					if(currentItem is BarEditItemLinkControl)
						((BarEditItemLinkControl)currentItem).focusEditorOnIsSelected = true;
				}
			}
		}
		protected override void CloseOnEscapeKeyDown(KeyEventArgs e) { }
		protected internal override void OnClosedCore() {
			base.OnClosedCore();
			CurrentItem = null;
			if(OwnerLinkControl != null && BarManagerHelper.GetItemLinkControlPopup(OwnerLinkControl) == this) {
				OwnerLinkControl.IsPressed = false;
			}			
		}						
		protected internal override bool ContainsLinkControl(BarItemLinkControlBase linkControl) {
			SubMenuBarControl lc = linkControl.LinksControl as SubMenuBarControl;
			while(lc != null && lc != PopupContent) {
				BarPopupBase pm = lc.Popup;
				if(pm == null || pm.OwnerLinkControl == null)
					lc = null;
				else
					lc = pm.OwnerLinkControl.LinksControl as SubMenuBarControl;
			}
			return lc != null;
		}
		protected virtual void OnItemsDisplayModeChanged(DependencyPropertyChangedEventArgs e) {
		}
	}
	[ContentProperty("Items")]
	public class PopupMenu : PopupMenuBase, ILinksHolder, ILogicalChildrenContainer {
		#region static
		public static readonly DependencyProperty GlyphSizeProperty;
		public static readonly DependencyProperty ItemLinksSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty ItemsProperty;
		public static readonly DependencyProperty ItemLinksProperty;
		public static readonly DependencyProperty ShowKeyGesturesProperty;
		protected internal static readonly DependencyPropertyKey ItemLinksPropertyKey;
		static readonly DependencyPropertyKey ItemsPropertyKey;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ItemLinksAttachedBehaviorProperty;
		public static readonly DependencyProperty ItemLinksSourceElementGeneratesUniqueBarItemProperty;
		static PopupMenu() {			
			ShowKeyGesturesProperty = DependencyPropertyManager.Register("ShowKeyGestures", typeof(bool), typeof(PopupMenu), new FrameworkPropertyMetadata(true));
			GlyphSizeProperty = DependencyPropertyManager.Register("GlyphSize", typeof(GlyphSize), typeof(PopupMenu), new FrameworkPropertyMetadata(GlyphSize.Default, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnGlyphSizePropertyChanged)));
			ItemLinksSourceProperty = DependencyProperty.Register("ItemLinksSource", typeof(IEnumerable), typeof(PopupMenu), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksSourcePropertyChanged)));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(PopupMenu), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(PopupMenu), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplateSelectorPropertyChanged)));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), typeof(PopupMenu), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			ItemLinksSourceElementGeneratesUniqueBarItemProperty = DependencyPropertyManager.Register("ItemLinksSourceElementGeneratesUniqueBarItem", typeof(bool), typeof(PopupMenu), new FrameworkPropertyMetadata(false, new PropertyChangedCallback((d, e) => ((PopupMenu)d).OnItemLinksSourceElementGeneratesUniqueBarItemChanged((bool)e.OldValue))));
			ItemLinksPropertyKey = DependencyPropertyManager.RegisterReadOnly("ItemLinks", typeof(BarItemLinkCollection), typeof(PopupMenu), new FrameworkPropertyMetadata(null, (d, e) => ((PopupMenu)d).OnItemLinksProperyChanged((BarItemLinkCollection)e.OldValue, (BarItemLinkCollection)e.NewValue)));
			ItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Items", typeof(CommonBarItemCollection), typeof(PopupMenu), new FrameworkPropertyMetadata(null));
			ItemsProperty = ItemsPropertyKey.DependencyProperty;
			ItemLinksProperty = ItemLinksPropertyKey.DependencyProperty;
			ItemLinksAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("ItemLinksAttachedBehavior", typeof(ItemsAttachedBehaviorCore<PopupMenu, BarItem>), typeof(PopupMenu), new UIPropertyMetadata(null));
		}
		protected static void OnGlyphSizePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupMenu)obj).OnGlyphSizeChanged(e);
		}
		protected static void OnItemLinksSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((PopupMenu)d).OnItemLinksSourceChanged(e);
		}
		protected static void OnItemLinksTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((PopupMenu)d).OnItemLinksTemplateChanged(e);
		}
		protected static void OnItemLinksTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((PopupMenu)d).OnItemLinksTemplateSelectorChanged(e);
		}
		#endregion
		public PopupMenu() : this(null) { }
		public PopupMenu(object context)
			: base() {
			SetValue(ItemLinksPropertyKey, CreateItemLinksCollection());
			SetValue(ItemsPropertyKey, new CommonBarItemCollection(this));
			if(ContentControl != null) {
				InitContentControl(context);
			}
			ContextLayoutManagerHelper.AddLayoutUpdatedHandler(this, OnLayoutUpdated);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			immediateActionsManager.ExecuteActions();
		}
		protected virtual void InitContentControl(object context) {
			ContentControl.LinksHolder = this;
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupMenuItems"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CommonBarItemCollection Items { get { return (CommonBarItemCollection)GetValue(ItemsProperty); } }
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupMenuItemLinks"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public BarItemLinkCollection ItemLinks { get { return (BarItemLinkCollection)GetValue(ItemLinksProperty); } }
		public bool ShowKeyGestures {
			get { return (bool)GetValue(ShowKeyGesturesProperty); }
			set { SetValue(ShowKeyGesturesProperty, value); }
		}
		public IEnumerable ItemLinksSource {
			get { return (IEnumerable)GetValue(ItemLinksSourceProperty); }
			set { SetValue(ItemLinksSourceProperty, value); }
		}
		public bool ItemLinksSourceElementGeneratesUniqueBarItem {
			get { return (bool)GetValue(ItemLinksSourceElementGeneratesUniqueBarItemProperty); }
			set { SetValue(ItemLinksSourceElementGeneratesUniqueBarItemProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
		ObservableCollection<ILinksHolder> mergedLinksHolders;
		protected internal ObservableCollection<ILinksHolder> MergedLinksHolders {
			get {
				if(mergedLinksHolders == null) {
					mergedLinksHolders = new ObservableCollection<ILinksHolder>();
					mergedLinksHolders.CollectionChanged += new NotifyCollectionChangedEventHandler(OnMergedLinksHoldersChanged);
				}
				return mergedLinksHolders;
			}
		}
		protected virtual void OnMergedLinksHoldersChanged(object sender, NotifyCollectionChangedEventArgs e) {
			BarItemLinkMergeHelper helper = new BarItemLinkMergeHelper();
			helper.Merge(((ILinksHolder)this).Links, MergedLinksHolders, ((ILinksHolder)this).MergedLinks);
			if(e.OldItems!=null)
			foreach (ILinksHolder value in e.OldItems) {
				value.MergedParent = null;
			}
			if(e.NewItems!=null)
			foreach (ILinksHolder value in e.NewItems) {
				value.MergedParent = this;
			}
		}
		protected virtual void OnItemLinksProperyChanged(BarItemLinkCollection oldValue, BarItemLinkCollection newValue) { }
		protected virtual BarItemLinkCollection GetNewItemLinksCollection() {
			return new BarItemLinkCollection(this);
		}		
		protected virtual BarItemLinkCollection CreateItemLinksCollection() {
			BarItemLinkCollection res = GetNewItemLinksCollection();
			res.CollectionChanged += new NotifyCollectionChangedEventHandler(OnItemLinksCollectionChanged);
			return res;
		}
		void OnItemLinksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			FrameworkElement elem = PlacementTarget as FrameworkElement;
			if(elem != null && elem.FlowDirection != System.Windows.FlowDirection.RightToLeft)
				return;
			PopupMenuBarControl popupMenuControl = PopupContent as PopupMenuBarControl;
			if(popupMenuControl == null || popupMenuControl.ItemsPresenter == null)
				return;
			BarLayoutHelper.InvalidateMeasureTree(popupMenuControl.ItemsPresenter, popupMenuControl);
		}
		protected override IEnumerator LogicalChildren {
			get {
				return ((ILinksHolder)this).GetLogicalChildrenEnumerator();
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupMenuGlyphSize")]
#endif
		public GlyphSize GlyphSize {
			get { return (GlyphSize)GetValue(GlyphSizeProperty); }
			set { SetValue(GlyphSizeProperty, value); }
		}		
		protected virtual void OnActualLinksChanged() {
		}
		protected override void OnNameChanged(string oldValue, string newValue) {
			base.OnNameChanged(oldValue, newValue);
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(ILinksHolder), oldValue, newValue);			
		}
		BarItemLinkCollection ILinksHolder.Links { get { return ItemLinks; } }
		IEnumerable ILinksHolder.ItemsSource { get { return ItemLinksSource; } }
		BarItemLinkCollection mergedLinks;
		protected virtual BarItemLinkCollection CreateMergedLinks() { return new MergedItemLinkCollection(this); }
		BarItemLinkCollection ILinksHolder.MergedLinks {
			get {
				if(mergedLinks == null)
					mergedLinks = CreateMergedLinks();
				return mergedLinks;
			}
		}
		readonly ImmediateActionsManager immediateActionsManager = new ImmediateActionsManager();
		ImmediateActionsManager ILinksHolder.ImmediateActionsManager {
			get { return immediateActionsManager; }
		}
		BarItemLinkCollection ILinksHolder.ActualLinks {
			get { return ((ILinksHolder)this).IsMergedState ? ((ILinksHolder)this).MergedLinks : ((ILinksHolder)this).Links; }
		}
		protected virtual bool ShowDescriptionCore() {
			return ItemsDisplayMode == PopupMenuItemsDisplayMode.LargeImagesTextDescription;
		}
		bool ILinksHolder.ShowDescription { get { return ShowDescriptionCore(); } }
		ILinksHolder mergedParent;
		ILinksHolder ILinksHolder.MergedParent {
			get { return mergedParent; }
			set { mergedParent = value; }
		}
		void ILinksHolder.Merge(ILinksHolder holder) {
			if(MergedLinksHolders.Contains(holder))
				return;
			MergedLinksHolders.Add(holder);
			if(MergedLinksHolders.Count == 1)
				OnActualLinksChanged();
		}
		void ILinksHolder.UnMerge(ILinksHolder holder) {
			MergedLinksHolders.Remove(holder);
			if(MergedLinksHolders.Count == 0)
				OnActualLinksChanged();
		}
		void ILinksHolder.UnMerge() {
			MergedLinksHolders.Clear();
			OnActualLinksChanged();
		}
		bool ILinksHolder.IsMergedState { get { return MergedLinksHolders.Count > 0; } }
		GlyphSize ILinksHolder.ItemsGlyphSize { get { return GlyphSize; } }
		protected virtual GlyphSize GetDefaultItemsGlyphSizeCore(LinkContainerType linkContainerType) {
			return (GlyphSize)GetValue(BarManager.MenuGlyphSizeProperty);
		}
		GlyphSize ILinksHolder.GetDefaultItemsGlyphSize(LinkContainerType linkContainerType) {
			return GetDefaultItemsGlyphSizeCore(linkContainerType);
		}
		IEnumerator ILinksHolder.GetLogicalChildrenEnumerator() {
			return new MergedEnumerator(logicalChildrenContainerItems.GetEnumerator(), new SingleObjectEnumerator(Child));
		}
		void ILinksHolder.OnLinkAdded(BarItemLinkBase link) {
			if (LogicalTreeHelper.GetParent(link) == null)
				AddLogicalChild(link);
		}
		void ILinksHolder.OnLinkRemoved(BarItemLinkBase link) {			
			RemoveLogicalChild(link);
		}
		LinksHolderType ILinksHolder.HolderType { get { return LinksHolderType.PopupMenu; } }
		protected virtual void UpdateItemsDisplayModeByGlyphSize() {
			if(GlyphSize == Bars.GlyphSize.Large) {
				if(ItemsDisplayMode != PopupMenuItemsDisplayMode.LargeImagesText && ItemsDisplayMode != PopupMenuItemsDisplayMode.LargeImagesTextDescription)
					ItemsDisplayMode = PopupMenuItemsDisplayMode.LargeImagesText;
			} else {
				if(ItemsDisplayMode != PopupMenuItemsDisplayMode.SmallImagesText && ItemsDisplayMode != PopupMenuItemsDisplayMode.Default)
					ItemsDisplayMode = PopupMenuItemsDisplayMode.SmallImagesText;
			}
		}
		private void OnGlyphSizeChanged(DependencyPropertyChangedEventArgs e) {
			UpdateItemsDisplayModeByGlyphSize();
			foreach(BarItemLinkBase link in ItemLinks) {
				link.UpdateProperties();
			}
		}
		protected virtual void OnItemLinksSourceElementGeneratesUniqueBarItemChanged(bool oldValue) {
			OnItemLinksSourceChanged(new System.Windows.DependencyPropertyChangedEventArgs(ItemLinksSourceProperty, ItemLinksSource, ItemLinksSource));
		}
		private void OnItemLinksTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<PopupMenu, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e, ItemLinksAttachedBehaviorProperty);
		}
		private void OnItemLinksTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<PopupMenu, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemLinksAttachedBehaviorProperty);
		}
		protected virtual void OnItemLinksSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemGeneratorHelper.OnItemsSourceChanged(e);
		}
		protected virtual void UpdateGlyphSizeByItemsDisplayMode() {
			if(ItemsDisplayMode == PopupMenuItemsDisplayMode.LargeImagesText || ItemsDisplayMode == PopupMenuItemsDisplayMode.LargeImagesTextDescription) {
				GlyphSize = Bars.GlyphSize.Large;
			} else {
				GlyphSize = Bars.GlyphSize.Small;
			}
		}		
		protected override void OnItemsDisplayModeChanged(DependencyPropertyChangedEventArgs e) {
			base.OnItemsDisplayModeChanged(e);
			UpdateGlyphSizeByItemsDisplayMode();
		}				
		BarItemGeneratorHelper<PopupMenu> itemGeneratorHelper;
		protected BarItemGeneratorHelper<PopupMenu> ItemGeneratorHelper {
			get {
				if(itemGeneratorHelper == null)
					itemGeneratorHelper = new BarItemGeneratorHelper<PopupMenu>(this, ItemLinksAttachedBehaviorProperty, ItemStyleProperty, ItemTemplateProperty, Items, ItemTemplateSelectorProperty, ItemLinksSourceElementGeneratesUniqueBarItem);
				return itemGeneratorHelper;
			}
		}
		#region ILogicalChildrenContainer
		readonly List<object> logicalChildrenContainerItems = new List<object>();
		void ILogicalChildrenContainer.AddLogicalChild(object child) {
			logicalChildrenContainerItems.Add(child);
			AddLogicalChild(child);
		}
		void ILogicalChildrenContainer.RemoveLogicalChild(object child) {
			logicalChildrenContainerItems.Remove(child);
			RemoveLogicalChild(child);
		}
		#endregion
		#region IMultipleElementRegistratorSupport Members
		protected override IEnumerable<object> GetRegistratorKeys() {
			foreach (var elem in base.GetRegistratorKeys())
				yield return elem;
			yield return typeof(ILinksHolder);
		}
		protected override object GetRegistratorName(object registratorKey) {
			if (Equals(registratorKey, typeof(ILinksHolder)))
				return Name;
			return base.GetRegistratorName(registratorKey);
		}
		#endregion
	}
	public abstract class PopupInfo<PopupType> : FrameworkElement, IPopupControl where PopupType : BarPopupBase {
		PopupType popup;
		public PopupInfo() {
			this.popup = CreatePopup();
		}
		public PopupType Popup { get { return popup; } }
		protected abstract PopupType CreatePopup();
		#region IPopupControl Members
		public virtual void ShowPopup(UIElement control) {
			Popup.ShowPopup(control);
		}
		public void ClosePopup() {
			Popup.ClosePopup();
		}
		public bool IsPopupOpen {
			get { return Popup.IsOpen; }
		}
		public event EventHandler Closed {
			add { Popup.Closed += value; }
			remove { Popup.Closed -= value; }
		}
		public event EventHandler Opened {
			add { Popup.Opened += value; }
			remove { Popup.Opened -= value; }
		}
		public event CancelEventHandler Opening {
			add { Popup.Opening += value; }
			remove { Popup.Opening -= value; }
		}
		BarPopupBase IPopupControl.Popup {
			get { return Popup; }
		}
		private WeakReference contextElement;
		WeakReference IPopupControl.ContextElement {
			get { return contextElement; }
			set {
				bool raiseChange = value != contextElement;
				if(raiseChange)
					OnContextElementChanging(value);
				WeakReference oldValue = contextElement;
				contextElement = value;
				if(raiseChange)
					OnContextElementChanged(oldValue);
			}
		}
		protected virtual void OnContextElementChanging(WeakReference newValue) {
		}
		protected virtual void OnContextElementChanged(WeakReference oldValue) {
		}
		#endregion
	}
	[Obsolete("PopupMenuInfo is no longer needed. Please replace PopupMenuInfo with PopupMenu.", false)]
	[ContentProperty("ItemLinks")]
	public class PopupMenuInfo : PopupInfo<PopupMenu> {
		#region static
		public static readonly DependencyProperty StretchRowsProperty;
		public static readonly DependencyProperty MaxRowCountProperty;
		public static readonly DependencyProperty MultiColumnProperty;
		public static readonly DependencyProperty MaxColumnHeightProperty;
		public static readonly DependencyProperty ItemsDisplayModeProperty;
		public static readonly DependencyProperty GlyphSizeProperty;
		public static readonly DependencyProperty ItemLinksSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ItemLinksAttachedBehaviorProperty;
		static PopupMenuInfo() {
			ItemsDisplayModeProperty = DependencyPropertyManager.Register("ItemsDisplayMode", typeof(PopupMenuItemsDisplayMode), typeof(PopupMenuInfo), new FrameworkPropertyMetadata(PopupMenuItemsDisplayMode.Default));
			GlyphSizeProperty = DependencyPropertyManager.Register("GlyphSize", typeof(GlyphSize), typeof(PopupMenuInfo), new FrameworkPropertyMetadata(GlyphSize.Default));
			ItemLinksSourceProperty = DependencyProperty.Register("ItemLinksSource", typeof(IEnumerable), typeof(PopupMenuInfo), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksSourcePropertyChanged)));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(PopupMenuInfo), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(PopupMenuInfo), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplateSelectorPropertyChanged)));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), typeof(PopupMenuInfo), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			ItemLinksAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("ItemLinksAttachedBehavior", typeof(ItemsAttachedBehaviorCore<PopupMenuInfo, BarItem>), typeof(PopupMenuInfo), new UIPropertyMetadata(null));
			StretchRowsProperty = DependencyPropertyManager.Register("StretchRows", typeof(bool), typeof(PopupMenuInfo), new FrameworkPropertyMetadata(true));
			MaxRowCountProperty = DependencyPropertyManager.Register("MaxRowCount", typeof(int), typeof(PopupMenuInfo), new FrameworkPropertyMetadata(int.MaxValue));
			MultiColumnProperty = DependencyPropertyManager.Register("MultiColumn", typeof(bool), typeof(PopupMenuInfo), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
			MaxColumnHeightProperty = DependencyPropertyManager.Register("MaxColumnHeight", typeof(double), typeof(PopupMenuInfo), new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
		protected static void OnItemLinksSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((PopupMenuInfo)d).OnItemLinksSourceChanged(e);
		}
		protected static void OnItemLinksTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((PopupMenuInfo)d).OnItemLinksTemplateChanged(e);
		}
		protected static void OnItemLinksTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((PopupMenuInfo)d).OnItemLinksTemplateSelectorChanged(e);
		}
		#endregion
		#region dep props
		public PopupMenuItemsDisplayMode ItemsDisplayMode {
			get { return (PopupMenuItemsDisplayMode)GetValue(ItemsDisplayModeProperty); }
			set { SetValue(ItemsDisplayModeProperty, value); }
		}
		public GlyphSize GlyphSize {
			get { return (GlyphSize)GetValue(GlyphSizeProperty); }
			set { SetValue(GlyphSizeProperty, value); }
		}
		public IEnumerable ItemLinksSource {
			get { return (IEnumerable)GetValue(ItemLinksSourceProperty); }
			set { SetValue(ItemLinksSourceProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
		public double MaxColumnHeight {
			get { return (double)GetValue(MaxColumnHeightProperty); }
			set { SetValue(MaxColumnHeightProperty, value); }
		}
		public int MaxRowCount {
			get { return (int)GetValue(MaxRowCountProperty); }
			set { SetValue(MaxRowCountProperty, value); }
		}
		public bool MultiColumn {
			get { return (bool)GetValue(MultiColumnProperty); }
			set { SetValue(MultiColumnProperty, value); }
		}
		public bool StretchRows {
			get { return (bool)GetValue(StretchRowsProperty); }
			set { SetValue(StretchRowsProperty, value); }
		}
		#endregion
		ObservableCollection<BarItemLinkBase> links;
		public PopupMenuInfo()
			: base() {
			this.links = new ObservableCollection<BarItemLinkBase>();
			this.links.CollectionChanged += OnLinkCollectionChanged;
			CreateBindings();
		}						
		public ObservableCollection<BarItemLinkBase> ItemLinks { get { return links; } }
		protected override PopupMenu CreatePopup() { return new PopupMenu(); }
		protected virtual void OnLinkCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					Popup.ItemLinks.Insert(e.NewStartingIndex, e.NewItems[0] as BarItemLinkBase);
					break;
				case NotifyCollectionChangedAction.Remove:
					Popup.ItemLinks.RemoveAt(e.OldStartingIndex);
					break;
				case NotifyCollectionChangedAction.Reset: {
						while(Popup.ItemLinks.Count != 0) {
							BarItemLinkBase link = Popup.ItemLinks[0];
							BarItem item = null;
							if(link is BarItemLink)
								item = ((BarItemLink)link).Item;
							if(link is BarItemLinkSeparator)
								item = ((BarItemLinkSeparator)link).Item;
							Popup.ItemLinks.RemoveAt(0);
							if(link is BarItemLink)
								((BarItemLink)link).Link(item);
						}
						foreach(BarItemLinkBase link in ItemLinks)
							Popup.ItemLinks.Add(link);
					}
					break;
				case NotifyCollectionChangedAction.Move:
					Popup.ItemLinks.Move(e.OldStartingIndex, e.NewStartingIndex);
					break;
				case NotifyCollectionChangedAction.Replace:
					Popup.ItemLinks.RemoveAt(e.OldStartingIndex);
					Popup.ItemLinks.Add(ItemLinks[e.NewStartingIndex]);
					break;
			}
		}
		protected internal PopupMenu Menu { get { return (PopupMenu)Popup; } }
		protected virtual void CreateBindings() {
			Binding bnd = new Binding("ItemsDisplayMode") { Source = this };
			bnd.Mode = BindingMode.TwoWay;
			Popup.SetBinding(PopupMenu.ItemsDisplayModeProperty, bnd);
			bnd = new Binding("GlyphSize") { Source = this };
			bnd.Mode = BindingMode.TwoWay;
			Popup.SetBinding(PopupMenu.GlyphSizeProperty, bnd);
			Popup.SetBinding(PopupMenu.MaxRowCountProperty, new Binding("MaxRowCount") { Source = this, Mode = BindingMode.TwoWay });
			Popup.SetBinding(PopupMenu.MaxColumnHeightProperty, new Binding("MaxColumnHeight") { Source = this, Mode = BindingMode.TwoWay });
			Popup.SetBinding(PopupMenu.MultiColumnProperty, new Binding("MultiColumn") { Source = this, Mode = BindingMode.TwoWay });
			Popup.SetBinding(PopupMenu.StretchRowsProperty, new Binding("StretchRows") { Source = this, Mode = BindingMode.TwoWay });
			Popup.SetBinding(PopupMenu.MaxHeightProperty, new Binding("MaxHeight") { Source = this, Mode = BindingMode.TwoWay });
			Popup.SetBinding(PopupMenu.MaxWidthProperty, new Binding("MaxWidth") { Source = this, Mode = BindingMode.TwoWay });
			Popup.SetBinding(PopupMenu.MinHeightProperty, new Binding("MinHeight") { Source = this, Mode = BindingMode.TwoWay });
			Popup.SetBinding(PopupMenu.MinWidthProperty, new Binding("MinWidth") { Source = this, Mode = BindingMode.TwoWay });
			Popup.SetBinding(PopupMenu.HeightProperty, new Binding("Height") { Source = this, Mode = BindingMode.TwoWay });
			Popup.SetBinding(PopupMenu.WidthProperty, new Binding("Width") { Source = this, Mode = BindingMode.TwoWay });
		}
		protected override void OnContextElementChanged(WeakReference oldValue) {
			if(Popup != null)
				((IPopupControl)Popup).ContextElement = ((IPopupControl)this).ContextElement;
		}
		private void OnItemLinksTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<PopupMenuInfo, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e, ItemLinksAttachedBehaviorProperty);
		}
		private void OnItemLinksTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<PopupMenuInfo, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemLinksAttachedBehaviorProperty);
		}
		protected virtual void OnItemLinksSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<PopupMenuInfo, BarItem>.OnItemsSourcePropertyChanged(this,
				e,
				ItemLinksAttachedBehaviorProperty,
				ItemTemplateProperty,
				ItemTemplateSelectorProperty,
				ItemStyleProperty,
				subItem => subItem.ItemLinks,
				subItem => new BarButtonItem(),
				(index, item) => {
					ItemLinks.Insert(index, (item as BarItem).CreateLink());
				}, useDefaultTemplateSelector: true);
		}
	}
	[Obsolete("MenuInfo property is no longer needed. Please replace MenuInfo with Menu property.", false)]
	[ContentProperty("ItemLinks")]
	public class PopupMenuBarInfo : PopupMenuInfo {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void ShowPopup(UIElement control) {
		}
	}
	[ContentProperty("Content")]
	public class PopupControlContainerInfo : PopupInfo<PopupControlContainer> {
		#region static
		public static readonly DependencyProperty ContentProperty;
		static PopupControlContainerInfo() {
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(PopupControlContainerInfo), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentPropertyChanged)));
		}
		protected static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PopupControlContainerInfo)d).OnContentChanged(e);
		}
		#endregion
		public PopupControlContainerInfo() : base() { }
		public object Content {
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		protected override PopupControlContainer CreatePopup() { return new PopupControlContainer(); }
		protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e) {
			Popup.Content = (UIElement)e.NewValue;
		}
	}
	[ContentProperty("Content")]
	public class PopupControlContainer : BarPopupExpandable {
		public static readonly DependencyProperty CloseOnClickProperty =
			DependencyPropertyManager.Register("CloseOnClick", typeof(bool), typeof(PopupControlContainer), new FrameworkPropertyMetadata(false, (d,e)=>((PopupControlContainer)d).OnCloseOnClickChanged()));		
		public static readonly DependencyProperty ContentProperty =
			DependencyPropertyManager.Register("Content", typeof(UIElement), typeof(PopupControlContainer),
			new FrameworkPropertyMetadata(null, (d, e) => ((PopupControlContainer)d).OnContentChanged()));
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupControlContainerContent"),
#endif
Bindable(true)]
		public UIElement Content {
			get { return (UIElement)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public bool CloseOnClick {
			get { return (bool)GetValue(CloseOnClickProperty); }
			set { SetValue(CloseOnClickProperty, value); }
		}
		public PopupControlContainer() : base() { ItemClickBehaviour = PopupItemClickBehaviour.CloseChildren; }		
		PopupContainer container;
		protected PopupContainer Container {
			get { return container; }
			set {
				if(container == value)
					return;
				var oldValue = container;
				container = value;
				OnContainerChanged(oldValue);
			}
		}		
		protected virtual void OnContentChanged() {
			Container.Content = this.Content;
		}
		protected virtual void OnCloseOnClickChanged() {
			if (CloseOnClick)
				ItemClickBehaviour = PopupItemClickBehaviour.CloseAllPopups;
			else
				ItemClickBehaviour = PopupItemClickBehaviour.CloseChildren;
		}
		protected override object OnIsOpenCoerce(object value) {
			if(Content == null) return false;
			return base.OnIsOpenCoerce(value);
		}
		protected override object CreatePopupContent() {
			Container = new PopupContainer() { Content = Content };
			return Container;
		}
		protected virtual void OnContainerChanged(PopupContainer oldValue) {
			if(oldValue != null)
				oldValue.RemoveHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnContainerMouseLeftButtonUp)); ;
			if(Container != null)
				Container.AddHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnContainerMouseLeftButtonUp), true);
		}
		protected virtual void OnContainerMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			if(CloseOnClick)
				SetCurrentValue(IsOpenProperty, false);
		}
		protected override void CloseOnEscapeKeyDown(KeyEventArgs e) {
			if (!(Child as FrameworkElement).With(c => LayoutHelper.FindElement(c, x => x is IBarsNavigationSupport)).ReturnSuccess())
				base.CloseOnEscapeKeyDown(e);
		}
	}
	public class PopupContainer : ContentControl {
		public PopupContainer() {
			DefaultStyleKey = typeof(PopupContainer);
			Loaded += new RoutedEventHandler(OnLoaded);
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			BarLayoutHelper.InvalidateMeasureTree(this, null);
		}
	}
	public class SubMenuScrollingVisibilityConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			SubMenuScrollViewer sv = (SubMenuScrollViewer)values[4];
			Visibility calculatedVisibility = (Visibility)values[0];
			if(calculatedVisibility != Visibility.Visible) return Visibility.Collapsed;
			double value = double.Parse((string)parameter, NumberFormatInfo.InvariantInfo);
			double verticalOffset = (double)values[1];
			double extentHeight = (double)values[2];
			double viewPortHeight = (double)values[3];
			if((extentHeight != viewPortHeight) && AreClose(Math.Min(100.0, Math.Max((double)0.0, (double)((verticalOffset * 100.0) / (extentHeight - viewPortHeight)))), value))
				return Visibility.Collapsed;
			return Visibility.Visible;
		}
		bool AreClose(double value1, double value2) { return Math.Abs(value1 - value2) < 1; }
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			return new object[] { Binding.DoNothing };
		}
	}
	public class SubMenuScrollViewer : Decorator {
		#region static
		public static readonly DependencyProperty VerticalOffsetProperty;
		public static readonly DependencyProperty ViewportHeightProperty;
		public static readonly DependencyProperty ScrollAreaHeightProperty;
		public static readonly DependencyProperty ScrollAreaWidthProperty;
		public static readonly DependencyProperty ViewportWidthProperty;
		public static readonly DependencyProperty HorizontalOffsetProperty;
		static SubMenuScrollViewer() {
			VerticalOffsetProperty = DependencyPropertyManager.Register("VerticalOffset", typeof(double), typeof(SubMenuScrollViewer), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnVerticalOffsetPropertyChanged), new CoerceValueCallback(OnVerticalOffsetPropertyCoerce)));
			ViewportHeightProperty = DependencyPropertyManager.Register("ViewportHeight", typeof(double), typeof(SubMenuScrollViewer), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnViewportHeightPropertyChanged)));
			ScrollAreaHeightProperty = DependencyPropertyManager.Register("ScrollAreaHeight", typeof(double), typeof(SubMenuScrollViewer), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnScrollAreaHeightPropertyChanged)));
			ScrollAreaWidthProperty = DependencyPropertyManager.Register("ScrollAreaWidth", typeof(double), typeof(SubMenuScrollViewer), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnScrollAreaWidthPropertyChanged)));
			ViewportWidthProperty = DependencyPropertyManager.Register("ViewportWidth", typeof(double), typeof(SubMenuScrollViewer), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnViewportWidthPropertyChanged)));
			HorizontalOffsetProperty = DependencyPropertyManager.Register("HorizontalOffset", typeof(double), typeof(SubMenuScrollViewer), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnHorizontalOffsetPropertyChanged), new CoerceValueCallback(OnHorizontalOffsetPropertyCoerce)));
		}
		protected static object OnVerticalOffsetPropertyCoerce(DependencyObject d, object baseValue) {
			return ((SubMenuScrollViewer)d).OnVerticalOffsetCoerce(baseValue);
		}
		protected static object OnHorizontalOffsetPropertyCoerce(DependencyObject d, object baseValue) {
			return ((SubMenuScrollViewer)d).OnHorizontalOffsetCoerce(baseValue);
		}
		protected static void OnVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SubMenuScrollViewer)d).OnVerticalOffsetChanged(e);
		}
		protected static void OnViewportHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SubMenuScrollViewer)d).OnViewportHeightChanged(e);
		}
		protected static void OnScrollAreaHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SubMenuScrollViewer)d).OnScrollAreaHeightChanged(e);
		}
		protected static void OnScrollAreaWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SubMenuScrollViewer)d).OnScrollAreaWidthChanged((double)e.OldValue);
		}
		protected static void OnViewportWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SubMenuScrollViewer)d).OnViewportWidthChanged((double)e.OldValue);
		}
		protected static void OnHorizontalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SubMenuScrollViewer)d).OnHorizontalOffsetChanged((double)e.OldValue);
		}
		#endregion
		public double VerticalOffset {
			get { return (double)GetValue(VerticalOffsetProperty); }
			set { SetValue(VerticalOffsetProperty, value); }
		}
		public double ViewportHeight {
			get { return (double)GetValue(ViewportHeightProperty); }
			set { SetValue(ViewportHeightProperty, value); }
		}
		public double ScrollAreaHeight {
			get { return (double)GetValue(ScrollAreaHeightProperty); }
			set { SetValue(ScrollAreaHeightProperty, value); }
		}
		public double HorizontalOffset {
			get { return (double)GetValue(HorizontalOffsetProperty); }
			set { SetValue(HorizontalOffsetProperty, value); }
		}
		public double ViewportWidth {
			get { return (double)GetValue(ViewportWidthProperty); }
			set { SetValue(ViewportWidthProperty, value); }
		}
		public double ScrollAreaWidth {
			get { return (double)GetValue(ScrollAreaWidthProperty); }
			set { SetValue(ScrollAreaWidthProperty, value); }
		}
		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			base.OnMouseWheel(e);
			if(e.Handled)
				return;
			int lineDelta = e.Delta / 120;
			VerticalOffset += lineDelta * 10;
		}
		protected override void OnKeyDown(KeyEventArgs e) { }
		protected override Size MeasureOverride(Size availableSize) {
			if(Child == null)
				return new Size(0, 0);
			Child.Measure(SizeHelper.Infinite);
			ScrollAreaHeight = Child.DesiredSize.Height;
			ScrollAreaWidth = Child.DesiredSize.Width;
			Size res = new Size();
			res.Width = Math.Min(availableSize.Width, Child.DesiredSize.Width);
			res.Height = Math.Min(availableSize.Height, Child.DesiredSize.Height);
			return res;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(Child != null) {
				Point pos = new Point(HorizontalOffset, VerticalOffset);
				Size size = new Size(Math.Max(finalSize.Width, Child.DesiredSize.Width), Math.Max(finalSize.Height, Child.DesiredSize.Height));
				Child.Arrange(new Rect(pos, size));
			}
			ViewportHeight = finalSize.Height;
			ViewportWidth = finalSize.Width;
			return finalSize;
		}
		public void LineDown() {
			if(ShowBottomScroll)
				VerticalOffset -= 10;
		}
		public void LineUp() {
			if(ShowTopScroll)
				VerticalOffset += 10;
		}
		public void LineLeft() {
			if(ShowLeftScroll)
				HorizontalOffset += 10;
		}
		public void LineRight() {
			if(ShowRightScroll)
				HorizontalOffset -= 10;
		}
		protected virtual void OnVerticalOffsetChanged(DependencyPropertyChangedEventArgs e) {
			InvalidateArrange();
			if(Child != null)
				Child.InvalidateArrange();
		}
		protected virtual void OnViewportHeightChanged(DependencyPropertyChangedEventArgs e) {
			VerticalOffset = (double)OnVerticalOffsetCoerce(VerticalOffset);
		}
		protected virtual void OnScrollAreaHeightChanged(DependencyPropertyChangedEventArgs e) {
			VerticalOffset = (double)OnVerticalOffsetCoerce(VerticalOffset);
		}
		protected virtual void OnScrollAreaWidthChanged(double oldValue) {
			HorizontalOffset = (double)OnHorizontalOffsetCoerce(HorizontalOffset);
		}
		protected virtual void OnViewportWidthChanged(double oldValue) {
			HorizontalOffset = (double)OnHorizontalOffsetCoerce(HorizontalOffset);
		}
		protected virtual void OnHorizontalOffsetChanged(double oldValue) {
			InvalidateArrange();
			if(Child != null)
				Child.InvalidateArrange();
		}
		protected virtual double OnHorizontalOffsetCoerce(object baseValue) {
			double value = (double)baseValue;
			if(ScrollAreaWidth < ViewportWidth)
				return 0.0;
			value = Math.Max(ViewportWidth - ScrollAreaWidth, value);
			value = Math.Min(0, value);
			return value;
		}
		protected virtual object OnVerticalOffsetCoerce(object baseValue) {
			double value = (double)baseValue;
			if(ScrollAreaHeight < ViewportHeight)
				return 0.0;
			value = Math.Max(ViewportHeight - ScrollAreaHeight, value);
			value = Math.Min(0, value);
			return value;
		}
		public virtual void BringIntoView(BarItemLinkControlBase linkControlBase) {
		}
		public bool ShowTopScroll { get { return VerticalOffset < 0; } }
		public bool ShowLeftScroll { get { return HorizontalOffset < 0; } }
		public bool ShowRightScroll { get { return ScrollAreaWidth - ViewportWidth > -HorizontalOffset; } }
		public bool ShowBottomScroll { get { return ScrollAreaHeight - ViewportHeight > -VerticalOffset; } }
	}
}
namespace DevExpress.Xpf.Core.Native {
	public class VisualEffectsInheritanceHelper {
		[Flags]
		enum VisualEffects {
			None = 0x0,
			BitmapScalingMode = 0x1,
			CachingHint = 0x2,
			ClearTypeHint = 0x4,
			EdgeMode = 0x8,
			TextFormattingMode = 0x10,
			TextHintingMode = 0x20,
			TextRenderingMode = 0x40,
			All = BitmapScalingMode | CachingHint | ClearTypeHint | EdgeMode | TextFormattingMode | TextHintingMode | TextRenderingMode 
		}
		public static void SetTextAndRenderOptions(DependencyObject element, DependencyObject treeRoot = null) {
			VisualEffects currentEffects = VisualEffects.All;
			treeRoot = treeRoot ?? element;
			while(treeRoot != null && currentEffects != VisualEffects.None) {
				CheckUpdateValue(treeRoot, element, ref currentEffects, VisualEffects.BitmapScalingMode, RenderOptions.BitmapScalingModeProperty, default(BitmapScalingMode));
				CheckUpdateValue(treeRoot, element, ref currentEffects, VisualEffects.CachingHint, RenderOptions.CachingHintProperty, default(CachingHint));
				CheckUpdateValue(treeRoot, element, ref currentEffects, VisualEffects.ClearTypeHint, RenderOptions.ClearTypeHintProperty, default(ClearTypeHint));
				CheckUpdateValue(treeRoot, element, ref currentEffects, VisualEffects.EdgeMode, RenderOptions.EdgeModeProperty, default(EdgeMode));
				CheckUpdateValue(treeRoot, element, ref currentEffects, VisualEffects.TextFormattingMode, TextOptions.TextFormattingModeProperty, default(TextFormattingMode));
				CheckUpdateValue(treeRoot, element, ref currentEffects, VisualEffects.TextHintingMode, TextOptions.TextFormattingModeProperty, default(TextHintingMode));
				CheckUpdateValue(treeRoot, element, ref currentEffects, VisualEffects.TextRenderingMode, TextOptions.TextRenderingModeProperty, default(TextRenderingMode));
				treeRoot = VisualTreeHelper.GetParent(treeRoot);
			}
		}
		static void CheckUpdateValue(DependencyObject source, DependencyObject target, ref VisualEffects allEffects, VisualEffects currentEffect, DependencyProperty currentProperty, object defaultValue) {
			if((allEffects & currentEffect) == currentEffect) {
				var value = source.GetValue(currentProperty);
				if(!object.Equals(value, defaultValue)) {
					target.SetValue(currentProperty, value);
					allEffects ^= currentEffect;
				}
			}
		}
	}
}
