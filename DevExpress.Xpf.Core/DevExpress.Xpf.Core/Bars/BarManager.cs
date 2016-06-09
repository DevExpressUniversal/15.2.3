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
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Bars.Automation;
using DevExpress.Xpf.Bars.Customization;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
namespace DevExpress.Xpf.Bars {
	public enum KeyGestureWorkingMode { UnhandledKeyGesture, AllKeyGesture, AllKeyGestureFromRoot }
	public enum SpacingMode { Mouse, Touch }
	[DXToolboxBrowsableAttribute, ContentProperty("Child")]
	[ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	public class BarManager : Decorator, IBarManager, IRuntimeCustomizationHost {
		#region static
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]	 
		public static readonly DependencyProperty BarManagerProperty;
		[Browsable(false)]
		public static readonly DependencyProperty BarManagerInfoProperty;
		public static readonly DependencyProperty IsMenuModeProperty;
		static readonly DependencyPropertyKey IsMenuModePropertyKey;
		public static readonly DependencyProperty MainMenuProperty;
		public static readonly DependencyProperty StatusBarProperty;
		public static readonly DependencyProperty ShowScreenTipsProperty;
		public static readonly DependencyProperty ShowShortcutInScreenTipsProperty;
		public static readonly DependencyProperty AllowHotCustomizationProperty;
		public static readonly DependencyProperty AllowQuickCustomizationProperty;
		public static readonly DependencyProperty AllowCustomizationProperty;
		public static readonly DependencyProperty ToolbarGlyphSizeProperty;
		public static readonly DependencyProperty MenuGlyphSizeProperty;
		public static readonly DependencyProperty MenuAnimationTypeProperty;
		public static readonly DependencyProperty CreateStandardLayoutProperty;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static readonly DependencyProperty DXContextMenuProperty;
		public static readonly DependencyProperty DXContextMenuPlacementProperty;
		public static readonly DependencyProperty MenuShowMouseButtonProperty;
		public static readonly DependencyProperty IsMDIChildManagerProperty;
		static readonly DependencyPropertyKey IsMDIChildManagerPropertyKey;
		public static readonly RoutedEvent ItemClickEvent;
		public static readonly RoutedEvent ItemDoubleClickEvent;
		public static readonly RoutedEvent LayoutUpgradingEvent;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty SkipMeasureByDockPanelLayoutHelperProperty;
		public static readonly DependencyProperty BarsSourceProperty;
		public static readonly DependencyProperty BarTemplateProperty;
		public static readonly DependencyProperty BarStyleProperty;
		public static readonly DependencyProperty BarTemplateSelectorProperty;
		public static readonly DependencyProperty AllowUIAutomationSupportProperty;
		public static readonly DependencyProperty AddNewItemsProperty;
		public static readonly DependencyProperty ShowScreenTipsInPopupMenusProperty;
		protected static readonly DependencyPropertyKey IsMenuVisiblePropertyKey;
		public static readonly DependencyProperty IsMenuVisibleProperty;
		public static readonly DependencyProperty AllowGlyphThemingProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty BarsAttachedBehaviorProperty =
			DependencyPropertyManager.RegisterAttached("BarsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<BarManager, Bar>), typeof(BarManager), new UIPropertyMetadata(null));
		public static readonly DependencyProperty MDIMergeStyleProperty;
		public static readonly DependencyProperty AllowNavigationFromEditorOnTabPressProperty;
		public static KeyGestureWorkingMode? GetKeyGestureWorkingMode(DependencyObject obj) { return (KeyGestureWorkingMode?)obj.GetValue(KeyGestureWorkingModeProperty); }
		public static void SetKeyGestureWorkingMode(DependencyObject obj, KeyGestureWorkingMode? value) { obj.SetValue(KeyGestureWorkingModeProperty, value); }
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty KeyGestureWorkingModeProperty = DependencyPropertyManager.RegisterAttached("KeyGestureWorkingMode", typeof(KeyGestureWorkingMode?), typeof(BarManager), new FrameworkPropertyMetadata((KeyGestureWorkingMode?)null));
		static BarManager() {
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarManager), typeof(BarManagerAutomationPeer), owner => new BarManagerAutomationPeer((BarManager)owner));
			MergingProperties.AllowMergingInternalProperty.OverrideMetadata(typeof(BarManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, null, (d, v) => ((BarManager)d).CoerceAllowMerging((bool?)v)));
			AllowNavigationFromEditorOnTabPressProperty = DependencyPropertyManager.Register("AllowNavigationFromEditorOnTabPress", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(false, new PropertyChangedCallback((d, e) => ((BarManager)d).OnActivateMenuModeWhenBarItemFocusedChanged((bool)e.OldValue))));
			DXSerializer.SerializationIDDefaultProperty.OverrideMetadata(typeof(BarManager), new UIPropertyMetadata("BarManager"));
			Control.IsTabStopProperty.OverrideMetadata(typeof(BarManager), new FrameworkPropertyMetadata(false));
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(BarManager), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
			InputMethod.IsInputMethodSuspendedProperty.OverrideMetadata(typeof(BarManager), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));			
			EventManager.RegisterClassHandler(typeof(BarManager), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
			BarManagerProperty = DependencyPropertyManager.RegisterAttached("BarManager", typeof(BarManager), typeof(BarManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnBarManagerPropertyChanged)));
#pragma warning disable 612,618
			BarManagerInfoProperty = DependencyPropertyManager.RegisterAttached("BarManagerInfo", typeof(BarManagerInfo), typeof(BarManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
#pragma warning restore 612,618
			IsMDIChildManagerPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsMDIChildManager", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(false));
			IsMDIChildManagerProperty = IsMDIChildManagerPropertyKey.DependencyProperty;
			IsMenuModePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsMenuMode", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, (d,e)=>((BarManager)d).OnIsMenuModeChanged((bool)e.OldValue, (bool)e.NewValue)));
			IsMenuModeProperty = IsMenuModePropertyKey.DependencyProperty;
			MainMenuProperty = DependencyPropertyManager.Register("MainMenu", typeof(Bar), typeof(BarManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnMainMenuPropertyChanged)));
			StatusBarProperty = DependencyPropertyManager.Register("StatusBar", typeof(Bar), typeof(BarManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStatusBarPropertyChanged)));
			ShowScreenTipsProperty = DependencyPropertyManager.Register("ShowScreenTips", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(true));
			ShowShortcutInScreenTipsProperty = DependencyPropertyManager.Register("ShowShortcutInScreenTips", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None));
			AllowHotCustomizationProperty = DependencyPropertyManager.Register("AllowHotCustomization", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(true));
			AllowQuickCustomizationProperty = DependencyPropertyManager.Register("AllowQuickCustomization", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAllowQuckCustomizationPropertyChanged)));
			AllowCustomizationProperty = DependencyPropertyManager.Register("AllowCustomization", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(true));
			ToolbarGlyphSizeProperty = DependencyPropertyManager.RegisterAttached("ToolbarGlyphSize", typeof(GlyphSize), typeof(BarManager), new FrameworkPropertyMetadata(GlyphSize.Default, FrameworkPropertyMetadataOptions.Inherits));
			MenuGlyphSizeProperty = DependencyPropertyManager.RegisterAttached("MenuGlyphSize", typeof(GlyphSize), typeof(BarManager), new FrameworkPropertyMetadata(GlyphSize.Default, FrameworkPropertyMetadataOptions.Inherits));
			MenuAnimationTypeProperty = DependencyPropertyManager.Register("MenuAnimationType", typeof(PopupAnimation), typeof(BarManager), new FrameworkPropertyMetadata(PopupAnimation.None));
			CreateStandardLayoutProperty = DependencyPropertyManager.Register("CreateStandardLayout", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(true));
			DXContextMenuProperty = DependencyPropertyManager.RegisterAttached("DXContextMenu", typeof(IPopupControl), typeof(BarManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnDXContextMenuChanged)));
			DXContextMenuPlacementProperty = DependencyPropertyManager.RegisterAttached("DXContextMenuPlacement", typeof(PlacementMode), typeof(BarManager), new FrameworkPropertyMetadata(PlacementMode.Mouse, FrameworkPropertyMetadataOptions.Inherits));
			MenuShowMouseButtonProperty = DependencyPropertyManager.RegisterAttached("MenuShowMouseButton", typeof(ButtonSwitcher), typeof(BarManager), new FrameworkPropertyMetadata(ButtonSwitcher.RightButton, FrameworkPropertyMetadataOptions.Inherits));
			ItemClickEvent = EventManager.RegisterRoutedEvent("ItemClick", RoutingStrategy.Direct, typeof(ItemClickEventHandler), typeof(BarManager));
			ItemDoubleClickEvent = EventManager.RegisterRoutedEvent("ItemDoubleClick", RoutingStrategy.Direct, typeof(ItemClickEventHandler), typeof(BarManager));
			LayoutUpgradingEvent = EventManager.RegisterRoutedEvent("LayoutUpgrading", RoutingStrategy.Direct, typeof(LayoutUpgradingEventHandler), typeof(BarManager));
			SkipMeasureByDockPanelLayoutHelperProperty = DependencyPropertyManager.RegisterAttached("SkipMeasureByDockPanelLayoutHelper", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(false));
			BarsSourceProperty = DependencyProperty.Register("BarsSource", typeof(object), typeof(BarManager), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnBarsSourcePropertyChanged)));
			BarTemplateProperty = DependencyProperty.Register("BarTemplate", typeof(DataTemplate), typeof(BarManager), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnBarTemplatePropertyChanged)));
			BarStyleProperty = DependencyProperty.Register("BarStyle", typeof(Style), typeof(BarManager), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnBarTemplatePropertyChanged)));
			BarTemplateSelectorProperty = DependencyProperty.Register("BarTemplateSelector", typeof(DataTemplateSelector), typeof(BarManager), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnBarTemplateSelectorPropertyChanged)));
			AllowUIAutomationSupportProperty = DependencyPropertyManager.Register("AllowUIAutomationSupport", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAllowUIAutomationSupportPropertyChanged)));
			AddNewItemsProperty = DependencyPropertyManager.Register("AddNewItems", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(false));
			ShowScreenTipsInPopupMenusProperty = DependencyPropertyManager.Register("ShowScreenTipsInPopupMenus", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(true));
			IsMenuVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsMenuVisible", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback((o, e) => { ((BarManager)o).OnIsMenuVisibleChanged(); })));
			IsMenuVisibleProperty = IsMenuVisiblePropertyKey.DependencyProperty;
			MDIMergeStyleProperty = DependencyPropertyManager.Register("MDIMergeStyle", typeof(MDIMergeStyle), typeof(BarManager), new FrameworkPropertyMetadata(MDIMergeStyle.Always, new PropertyChangedCallback(OnMDIMergeStylePropertyChanged)));
			AllowGlyphThemingProperty = DependencyPropertyManager.Register("AllowGlyphTheming", typeof(bool), typeof(BarManager), new FrameworkPropertyMetadata(false, (d, e) => ((BarManager)d).OnAllowGlyphThemingChanged((bool)e.OldValue)));						
		}		
		static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e) {
			((BarManager)sender).OnAccessKeyPressed(e);
		}
		protected virtual void OnAccessKeyPressed(AccessKeyPressedEventArgs e) {
			if(Child != null && e.OriginalSource is DependencyObject && LayoutHelper.IsChildElement(Child, (DependencyObject)e.OriginalSource))
				return;
			if(!Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt)) {
				e.Scope = this;
				e.Handled = true;
			}
		}
		protected static void OnAllowUIAutomationSupportPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarManager)d).OnAllowUIAutomationSupportChanged((bool)e.OldValue);
		}
		protected static void OnBarTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarManager)d).OnBarTemplateChanged(e);
		}
		protected static void OnBarTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarManager)d).OnBarTemplateSelectorChanged(e);
		}		
		protected static void OnAllowQuckCustomizationPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarManager)obj).OnAllowQuickCustomizationChanged(e);
		}		
		protected static void OnMainMenuPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarManager)obj).OnMainMenuChanged(e);
		}
		protected static void OnStatusBarPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarManager)obj).OnStatusBarChanged(e);
		}
		protected static void OnBarsSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarManager)d).OnBarsSourceChanged(e);
		}
		protected virtual void OnMDIMergeStyleChanged(MDIMergeStyle oldValue) {
			CoerceValue(MergingProperties.AllowMergingInternalProperty);
			UpdateMenuVisibility();			
		}
		protected virtual bool? CoerceAllowMerging(bool? v) {
			if (MDIMergeStyle == MDIMergeStyle.Never)
				return false;
			return v;
		}
		protected virtual void OnAllowGlyphThemingChanged(bool oldValue) {
			UpdateGlyphColorization();
		}
		internal void UpdateGlyphColorization() {
			Items.ToList().ForEach(x => x.ExecuteActionOnLinkControls(lc => lc.UpdateActualAllowGlyphTheming()));
		}
		static void OnDXContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UIElement contextElement = d as UIElement;
			IPopupControl contextMenu = e.NewValue as IPopupControl;
			IPopupControl oldContextMenu = e.OldValue as IPopupControl;
			oldContextMenu.With(x => x.Popup).Do(x => x.PlacementTarget = null);			
			if(contextElement == null) return;
			if (contextMenu != null && contextMenu.Popup!=null && contextMenu.Popup.AttachToPlacementTargetWhenClosed) {
				contextMenu.Popup.PlacementTarget = contextElement;
			}
			Action<WeakReference, WeakReference> registryAction = new Action<WeakReference, WeakReference>((cE, cM) => {
				var ceR = cE.IsAlive ? (UIElement)cE.Target : null;
				var cmR = cM.IsAlive ? (IPopupControl)cM.Target : null;
				if (cmR == null && ceR == null)
					return;
				if (cmR != null) ContextMenuManager.RegistryContextElement(ceR, cmR);
				else ContextMenuManager.UnregistryContextElement(ceR);
			});			
			if(contextElement is FrameworkElement && !((FrameworkElement)contextElement).IsLoaded)
				contextElement.Dispatcher.BeginInvoke(registryAction, new WeakReference(contextElement), new WeakReference(contextMenu));
			else
				registryAction(new WeakReference(contextElement), new WeakReference(contextMenu));
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static void SetDXContextMenu(UIElement element, IPopupControl value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(DXContextMenuProperty, value);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static IPopupControl GetDXContextMenu(UIElement element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (IPopupControl)element.GetValue(DXContextMenuProperty);
		}
		public static void SetDXContextMenuPlacement(UIElement element, PlacementMode value) {		
			element.SetValue(DXContextMenuPlacementProperty, value);
		}	   
		public static PlacementMode GetDXContextMenuPlacement(UIElement element) {
			return (PlacementMode)element.GetValue(DXContextMenuPlacementProperty);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static void SetMenuShowMouseButton(UIElement element, ButtonSwitcher value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(MenuShowMouseButtonProperty, value);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static ButtonSwitcher GetMenuShowMouseButton(UIElement element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (ButtonSwitcher)element.GetValue(MenuShowMouseButtonProperty);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool GetSkipMeasureByDockPanelLayoutHelper(DependencyObject obj) {
			return (bool)obj.GetValue(SkipMeasureByDockPanelLayoutHelperProperty);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetSkipMeasureByDockPanelLayoutHelper(DependencyObject obj, bool value) {
			obj.SetValue(SkipMeasureByDockPanelLayoutHelperProperty, value);
		}
		protected static void OnBarManagerPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {			
			BarContainerControlPanel panel = obj as BarContainerControlPanel;			
			DragWidget dw = obj as DragWidget;
			if(dw != null)
				dw.OnBarManagerChanged(e);
			FloatingBarPopupContentControl fpc = obj as FloatingBarPopupContentControl;
			if(fpc != null)
				fpc.OnManagerChanged(e);
			UIElement element = obj as UIElement;			
			LinksControl lc = obj as LinksControl;
			if(lc != null)
				lc.OnManagerChanged(e);
		}
		protected static void OnBarManagerInfoPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
#pragma warning disable 612,618
			var newInfo = e.NewValue as BarManagerInfo;
			if (newInfo == null)
				obj.ClearValue(BarManagerProperty);
			else
				obj.SetValue(BarManagerProperty, newInfo.Manager);
#pragma warning restore 612,618
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static BarManager GetBarManager(DependencyObject dobj) { return (BarManager)dobj.GetValue(BarManagerProperty); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("Use the BarManager property instead")]
		[Browsable(false)]
		public static BarManagerInfo GetBarManagerInfo(DependencyObject dobj) { return (BarManagerInfo)dobj.GetValue(BarManagerInfoProperty); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static void SetBarManager(DependencyObject dobj, BarManager value) { dobj.SetValue(BarManagerProperty, value); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("Use the BarManager property instead")]
		[Browsable(false)]
		public static void SetBarManagerInfo(DependencyObject dobj, BarManagerInfo value) { dobj.SetValue(BarManagerInfoProperty, value); }
		#endregion
		public BarManager() {
			new BarManagerSerializationStrategy(this);
			IsMenuVisible = true;
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);			
			ValuesProvider = new BarManagerThemeDependentValuesProvider(this);
			LogicalChildrenContainer = new BarManagerILogicalChildrenContainerWrapper(this);
			Focusable = true;
			FocusVisualStyle = null;
			MDIChildHostMenuIsVisibleChangedHandler = new EventHandler((o, e) => { UpdateMenuVisibility(); });			
			BarManager.SetBarManager(this, this);
			BarNameScope.SetIsScopeOwner(this, true);
			IsEnabledChanged += OnIsEnabledChanged;
		}
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {			
			foreach(object logicalChild in LogicalTreeHelper.GetChildren(this)) {
				if(logicalChild is FrameworkContentElement || logicalChild is FrameworkElement)
					((DependencyObject)logicalChild).CoerceValue(e.Property);
			}			
		}
		#region fields
		static bool checkBarItemNames = true;
		private static bool ignoreMenuDropAlignment;
		List<BarContainerControl> standardContainers = new List<BarContainerControl>();
		internal BarManagerThemeDependentValuesProvider ValuesProvider;
		BarContainerControlCollection containers;
		BarManagerCategoryCollection categories;
		BarCollection bars;
		protected internal ILogicalChildrenContainer LogicalChildrenContainer { get; private set; }
		BarItemCollection items;
		Delegate onEnterMenuModeDelegate = null;
		WeakReference iMDIChildHostWR = new WeakReference(null);
		EventHandler MDIChildHostMenuIsVisibleChangedHandler;
		#endregion
		#region props
		public static bool CheckBarItemNames {
			get { return checkBarItemNames; }
			set { checkBarItemNames = value; }
		}
		public static bool IgnoreMenuDropAlignment {
			get { return ignoreMenuDropAlignment; }
			set { ignoreMenuDropAlignment = value; }
		}
		IMDIChildHost MDIChildHost {
			get { return iMDIChildHostWR.Target as IMDIChildHost; }
			set {
				IMDIChildHost old = iMDIChildHostWR.Target as IMDIChildHost;
				if (old != null)
					old.IsChildMenuVisibleChanged -= MDIChildHostMenuIsVisibleChangedHandler;
				if (value == null)
					IsMenuVisible = true;
				iMDIChildHostWR = new WeakReference(value);
				if (value != null) {
					value.IsChildMenuVisibleChanged += MDIChildHostMenuIsVisibleChangedHandler;
					UpdateMenuVisibility();
				}
			}
		}
		internal WeakReference RibbonControl { get; set; }
		internal WeakReference RibbonStatusBar { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public KeyGestureWorkingMode KeyGestureWorkingMode {
			get { return GetKeyGestureWorkingMode(this).Return(x => x.Value, () => KeyGestureWorkingMode.UnhandledKeyGesture); }
			set { SetKeyGestureWorkingMode(this, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool LockKeyGestureEventAfterHandling {
			get { return lockKeyGestureEventAfterHandling; }
			set { lockKeyGestureEventAfterHandling = value; }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarManagerControllers"),
#endif
 EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public ObservableCollection<IControllerAction> Controllers {
			get { return ControllerBehavior.Actions; }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerCustomizationHelper")]
#endif
		public BarManagerCustomizationHelper CustomizationHelper {
			get { return BarNameScope.GetService<ICustomizationService>(this).CustomizationHelper; }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerCreateStandardLayout")]
#endif
		public bool CreateStandardLayout {
			get { return (bool)GetValue(CreateStandardLayoutProperty); }
			set { SetValue(CreateStandardLayoutProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerToolbarGlyphSize")]
#endif
		public GlyphSize ToolbarGlyphSize {
			get { return (GlyphSize)GetValue(ToolbarGlyphSizeProperty); }
			set { SetValue(ToolbarGlyphSizeProperty, value); }
		}
		public static GlyphSize GetToolbarGlyphSize(DependencyObject dObj) {
			return (GlyphSize)dObj.GetValue(ToolbarGlyphSizeProperty);
		}
		public static void SetToolbarGlyphSize(DependencyObject dObj, GlyphSize value) {
			dObj.SetValue(ToolbarGlyphSizeProperty, value);
		}
		public static GlyphSize GetMenuGlyphSize(DependencyObject dObj) {
			return (GlyphSize)dObj.GetValue(MenuGlyphSizeProperty);
		}
		public static void SetMenuGlyphSize(DependencyObject dObj, GlyphSize value) {
			dObj.SetValue(MenuGlyphSizeProperty, value);
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerMenuGlyphSize")]
#endif
		public GlyphSize MenuGlyphSize {
			get { return (GlyphSize)GetValue(MenuGlyphSizeProperty); }
			set { SetValue(MenuGlyphSizeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerAddNewItems")]
#endif
		public bool AddNewItems {
			get { return (bool)GetValue(AddNewItemsProperty); }
			set { SetValue(AddNewItemsProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerMDIMergeStyle")]
#endif
		public MDIMergeStyle MDIMergeStyle {
			get { return (MDIMergeStyle)GetValue(MDIMergeStyleProperty); }
			set { SetValue(MDIMergeStyleProperty, value); }
		}
		public bool AllowNavigationFromEditorOnTabPress {
			get { return (bool)GetValue(AllowNavigationFromEditorOnTabPressProperty); }
			set { SetValue(AllowNavigationFromEditorOnTabPressProperty, value); }
		}
		protected internal BarContainerControlCollection Containers {
			get {
				if (containers == null)
					containers = CreateContainers();
				return containers;
			}
		}
		public DataTemplate BarTemplate {
			get { return (DataTemplate)GetValue(BarTemplateProperty); }
			set { SetValue(BarTemplateProperty, value); }
		}
		public DataTemplateSelector BarTemplateSelector {
			get { return (DataTemplateSelector)GetValue(BarTemplateSelectorProperty); }
			set { SetValue(BarTemplateSelectorProperty, value); }
		}
		public Style BarStyle {
			get { return (Style)GetValue(BarStyleProperty); }
			set { SetValue(BarStyleProperty, value); }
		}
		public bool AllowUIAutomationSupport {
			get { return (bool)GetValue(AllowUIAutomationSupportProperty); }
			set { SetValue(AllowUIAutomationSupportProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarManagerBars"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BarCollection Bars {
			get {
				if (bars == null)
					bars = CreateBars();
				return bars;
			}
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarManagerItems"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BarItemCollection Items {
			get {
				if (items == null)
					items = CreateItems();
				return items;
			}
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarManagerCategories"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BarManagerCategoryCollection Categories {
			get {
				if (categories == null)
					categories = CreateCategories();
				return categories;
			}
		}
		public object BarsSource {
			get { return GetValue(BarsSourceProperty); }
			set { SetValue(BarsSourceProperty, value); }
		}
		public bool ShowScreenTipsInPopupMenus {
			get { return (bool)GetValue(ShowScreenTipsInPopupMenusProperty); }
			set { SetValue(ShowScreenTipsInPopupMenusProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarManagerMainMenu"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public Bar MainMenu {
			get { return (Bar)GetValue(MainMenuProperty); }
			set { SetValue(MainMenuProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarManagerStatusBar"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public Bar StatusBar {
			get { return (Bar)GetValue(StatusBarProperty); }
			set { SetValue(StatusBarProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerIsMDIChildManager")]
#endif
		public bool IsMDIChildManager {
			get { return (bool)GetValue(IsMDIChildManagerProperty); }
			internal set { this.SetValue(IsMDIChildManagerPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerIsMenuMode")]
#endif
		public bool IsMenuMode {
			get { return (bool)GetValue(IsMenuModeProperty); }
			internal set { this.SetValue(IsMenuModePropertyKey, value); }
		}
		public bool IsMenuVisible {
			get { return (bool)GetValue(IsMenuVisibleProperty); }
			protected internal set { this.SetValue(IsMenuVisiblePropertyKey, value); }
		}
		public bool AllowGlyphTheming {
			get { return (bool)GetValue(AllowGlyphThemingProperty); }
			set { SetValue(AllowGlyphThemingProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerShowScreenTips")]
#endif
		public bool ShowScreenTips {
			get { return (bool)GetValue(ShowScreenTipsProperty); }
			set { SetValue(ShowScreenTipsProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerShowShortcutInScreenTips")]
#endif
		public bool ShowShortcutInScreenTips {
			get { return (bool)GetValue(ShowShortcutInScreenTipsProperty); }
			set { SetValue(ShowShortcutInScreenTipsProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerAllowHotCustomization")]
#endif
		public bool AllowHotCustomization {
			get { return (bool)GetValue(AllowHotCustomizationProperty); }
			set { SetValue(AllowHotCustomizationProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerAllowQuickCustomization")]
#endif
		public bool AllowQuickCustomization {
			get { return (bool)GetValue(AllowQuickCustomizationProperty); }
			set { SetValue(AllowQuickCustomizationProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerAllowCustomization")]
#endif
		public bool AllowCustomization {
			get { return (bool)GetValue(AllowCustomizationProperty); }
			set { SetValue(AllowCustomizationProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerMenuAnimationType")]
#endif
		public PopupAnimation MenuAnimationType {
			get { return (PopupAnimation)GetValue(MenuAnimationTypeProperty); }
			set { SetValue(MenuAnimationTypeProperty, value); }
		}
		public event ItemClickEventHandler ItemClick {
			add { this.AddHandler(ItemClickEvent, value); }
			remove { this.RemoveHandler(ItemClickEvent, value); }
		}
		public event LayoutUpgradingEventHandler LayoutUpgrading {
			add { this.AddHandler(LayoutUpgradingEvent, value); }
			remove { this.RemoveHandler(LayoutUpgradingEvent, value); }
		}
		public event ItemClickEventHandler ItemDoubleClick {
			add { this.AddHandler(ItemDoubleClickEvent, value); }
			remove { this.RemoveHandler(ItemDoubleClickEvent, value); }
		}		
		protected bool ManagerInitialized { get; private set; }
		#endregion
		#region initialization
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			BarNameScope.EnsureRegistrator(this);
		}
		protected virtual BarContainerControlCollection CreateContainers() { return new BarContainerControlCollection(this); }
		protected virtual BarCollection CreateBars() { return new BarCollection(this); }
		protected virtual BarItemCollection CreateItems() {
			BarItemCollection res = new BarItemCollection();
			res.CollectionChanged += OnItemsCollectionChanged;
			return res;
		}
		protected virtual BarManagerCategoryCollection CreateCategories() { return new BarManagerCategoryCollection(this); }
		protected virtual void OnLoaded(object sender, EventArgs e) {
			DevExpress.Xpf.Core.FrameworkElementHelper.SetIsLoaded(this, true);
			MDIChildHost = LayoutHelper.FindParentObject<IMDIChildHost>(this);
			IsMDIChildManager = (MDIChildHost != null);
			Initialize();						
		}
		protected virtual void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {		
			DevExpress.Xpf.Core.FrameworkElementHelper.SetIsLoaded(this, false);
		}
		#endregion
		#region menu mode
		protected virtual void UpdateMenuVisibility() {
			if(MDIChildHost == null) {
				IsMenuVisible = true;
				return;
			}
			IsMenuVisible = MDIChildHost.IsChildMenuVisible || (MDIMergeStyle == Xpf.Bars.MDIMergeStyle.Never);
			MergingProperties.SetHideElements(this, !IsMenuVisible);
		}		
		protected virtual void SubscribeKeyboardNavigationEvents() {
			PropertyInfo info = typeof(KeyboardNavigation).GetProperty("Current", BindingFlags.NonPublic | BindingFlags.Static);
			KeyboardNavigation current = (KeyboardNavigation)info.GetValue(null, null);
			EventInfo eventInfo = typeof(KeyboardNavigation).GetEvent("EnterMenuMode", BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo mi = eventInfo.GetAddMethod(true);
			Type tDelegate = eventInfo.EventHandlerType;
			if(this.onEnterMenuModeDelegate == null)
				this.onEnterMenuModeDelegate = Delegate.CreateDelegate(tDelegate, this, this.GetType().GetMethod("OnEnterMenuMode", BindingFlags.Instance | BindingFlags.NonPublic));
			mi.Invoke(current, new object[] { this.onEnterMenuModeDelegate });
		}
		protected virtual void UnsubscribeKeyboardNavigationEvents() {
			PropertyInfo info = typeof(KeyboardNavigation).GetProperty("Current", BindingFlags.NonPublic | BindingFlags.Static);
			KeyboardNavigation current = (KeyboardNavigation)info.GetValue(null, null);
			EventInfo eventInfo = typeof(KeyboardNavigation).GetEvent("EnterMenuMode", BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo mi = eventInfo.GetRemoveMethod(true);
			Type tDelegate = eventInfo.EventHandlerType;
			if(this.onEnterMenuModeDelegate != null)
				mi.Invoke(current, new object[] { this.onEnterMenuModeDelegate });
			this.onEnterMenuModeDelegate = null;
		}
		protected virtual bool OnEnterMenuMode(object sender, EventArgs e) {
			Window window = Window.GetWindow(this);
			if(window != null && !window.IsActive) return false;
			var targetBar = GetMenuModeTarget();
			if (targetBar != null) {
				ActivateMenu();
				return true;
			}
			return false; 
		}
		protected virtual Bar GetMenuModeTarget() {
			var targetBar = MainMenu.If(x => x.DockInfo != null && x.DockInfo.BarControl != null) ?? Bars.FirstOrDefault(bar => bar.With(x => x.DockInfo).With(x => x.BarControl).ReturnSuccess());
			return targetBar;
		}
		#endregion               
		#region controllers
		protected internal virtual void OnControllerAdded(IActionContainer controller) {
			if (controller.AssociatedObject == null)
				controller.AssociatedObject = this;
		}
		protected internal virtual void OnControllerRemoved(IActionContainer controller) {
			if (controller.AssociatedObject == this)
				controller.AssociatedObject = null;			
		}
		#endregion
		#region customization        
		protected internal bool GetHotQuickCustomization() {
			return AllowCustomization && AllowHotCustomization;
		}
		protected virtual void OnAllowQuickCustomizationChanged(DependencyPropertyChangedEventArgs e) {
			if (Containers == null)
				return;
			foreach (BarContainerControl cont in Containers) {
				if (!cont.With(x => x.ClientPanel).With(x => x.Children).ReturnSuccess())
					continue;
				foreach (BarControl bc in cont.ClientPanel.Children) {
					bc.UpdateBarControlProperties();
				}
			}
		}
		#endregion
		#region obsolete
		[Obsolete]
		protected virtual BarManagerCustomizationHelper CreateCustomizationHelper() {
			return new BarManagerCustomizationHelper();
		}
		[Obsolete]
		public virtual void CloseAllPopups() {
			PopupMenuManager.CloseAllPopups();
		}
		#endregion
		#region events
		protected internal virtual void RaiseItemClick(BarItem item, BarItemLink link) {
			this.RaiseEvent(new ItemClickEventArgs(item, link) { RoutedEvent = ItemClickEvent });
		}
		protected internal virtual void RaiseItemDoubleClick(BarItem item, BarItemLink link) {
			this.RaiseEvent(new ItemClickEventArgs(item, link) { RoutedEvent = ItemDoubleClickEvent });
		}
		#endregion
		#region logical children
		new protected internal void AddLogicalChild(object child) {
			base.AddLogicalChild(child);
		}
		new protected internal void RemoveLogicalChild(object child) {
			base.RemoveLogicalChild(child);
		}
		protected internal virtual void OnBarItemAdded(BarItem item) {
			if (!item.SkipAddToBarManagerLogicalTree && item.Parent==null)
				AddLogicalChild(item);
		}
		protected internal virtual void OnBarItemRemoved(BarItem item) {
			if (item.Parent == this)
				RemoveLogicalChild(item);
		}
		protected internal void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.OldItems != null) {
				foreach (BarItem item in e.OldItems)
					OnBarItemRemoved(item);
			}
			if (e.NewItems != null) {
				foreach (BarItem item in e.NewItems)
					OnBarItemAdded(item);
			}
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get { return GetLogicalChildrenEnumerator(); }
		}
		IEnumerator GetLogicalChildrenEnumerator() {
			return GetLogicalChildrenEnumeratorImpl().OfType<DependencyObject>().Where(x => LogicalTreeHelper.GetParent(x) == this).ToList().GetEnumerator();
		}
		IEnumerable GetLogicalChildrenEnumeratorImpl() {
			if (Child != null)
				yield return Child;
			foreach (BarItem item in Items) {
				if (item.Parent == this)
					yield return item;
			}
			yield return ValuesProvider;			
			foreach (Bar bar in Bars)
				yield return bar;
			foreach(BarManagerCategory category in Categories) {
				yield return category;
			}
			foreach (var lChild in ((BarManagerILogicalChildrenContainerWrapper)LogicalChildrenContainer).LogicalChildren)
				yield return lChild;
		}
		#endregion
		#region visual children
		protected internal virtual void AddVisualChildInternal(UIElement child) {
			AddVisualChild(child);
		}
		protected internal virtual void RemoveVisualChildInternal(UIElement child) {
			RemoveVisualChild(child);
		}
		protected override int VisualChildrenCount { get { return base.VisualChildrenCount + 1 + StandardContainers.Count; } }
		protected override Visual GetVisualChild(int index) {
			int baseChildrenCount = base.VisualChildrenCount;
			if (index < StandardContainers.Count)
				return StandardContainers[index];
			if (index < base.VisualChildrenCount + StandardContainers.Count)
				return base.GetVisualChild(index - StandardContainers.Count);
			if (index == baseChildrenCount + StandardContainers.Count)
				return ValuesProvider;
			return null;
		}
		#endregion
		protected virtual void OnActivateMenuModeWhenBarItemFocusedChanged(bool oldValue) {   
		}		
		protected internal List<BarContainerControl> StandardContainers { get { return standardContainers; } }
		internal void AddStandardContainer(BarContainerControl control) {
			AddVisualChildInternal(control);
			StandardContainers.Add(control);
		}
		void OnIsMenuVisibleChanged() {
			MergingProperties.SetHideElements(this, !IsMenuVisible);			
		}
		protected virtual void Initialize() {
			if (ManagerInitialized)
				return;
			ManagerInitialized = true;
			AddValuesProvider();			
			ControllerBehavior.Execute();
			InitializeItemsCategories();
		}
		void AddValuesProvider() {
			if (ValuesProvider.Parent == this)
				return;
			AddLogicalChild(ValuesProvider);
			AddVisualChild(ValuesProvider);
		}						
		protected internal virtual void InitializeItemLinks(BarItemLinkCollection coll) {
			BarItemLinkBase[] linkArray =  coll.ToArray<BarItemLinkBase>();
			foreach(BarItemLinkBase link in linkArray) {
				link.Initialize();
			}
		}
		protected virtual void InitializeItemsCategories() {
			foreach(BarItem item in Items) {
				item.Category = Categories[item.CategoryName];
			}
		}		
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event MouseEventHandler BeforeCheckCloseAllPopups;
		protected internal virtual bool CheckCloseAllPopups(MouseEventArgs e) {
			if (BeforeCheckCloseAllPopups != null) BeforeCheckCloseAllPopups(this, e);
			return PopupMenuManager.CloseAllPopups(null, e);									
		}
		protected virtual bool IsVisualOrLogicAncestor(DependencyObject parent, DependencyObject child) {
			do {
				object result = child.VisualParents(true).FirstOrDefault((e) => e == this || e == parent || ((e is Visual || e is System.Windows.Media.Media3D.Visual3D) && VisualTreeHelper.GetParent(e) == null));
				if(result == parent) return true;
				if(result == this) return false;
				if(result is FrameworkElement && ((FrameworkElement)result).Parent is Popup)
					child = ((FrameworkElement)result).Parent as DependencyObject;
				else return false;
			} while(true);
		}						
		void EnableKeyboardCues(DependencyObject element, bool enable) { 
			MethodInfo mi = typeof(KeyboardNavigation).GetMethod("EnableKeyboardCues", BindingFlags.NonPublic | BindingFlags.Static);
			if(mi == null)
				return;
			mi.Invoke(null, new object[] { element, enable });
		}
		protected void EnableKeyboardCues(bool enable) {
			foreach(Bar bar in Bars) {
				if(bar.DockInfo.BarControl != null)
					EnableKeyboardCues(bar.DockInfo.BarControl, enable);
			}
		}
		protected internal virtual void DeactivateMenu() {
			NavigationTree.ExitMenuMode();
			PopupMenuManager.CloseAllPopups();
			EnableKeyboardCues(false);
		}
		protected internal virtual void ActivateMenuOnGotFocus() {
		}
		protected internal virtual void ActivateMenu() { }
		protected virtual void OnIsMenuModeChanged(bool oldValue, bool newValue) {			
		}
		protected override void OnLostFocus(RoutedEventArgs e) {
			base.OnLostFocus(e);
			if(!IsAnyPopupFocused() && NavigationTree.CurrentElement!=null && (!IsKeyboardFocusWithin || (Child!=null && Child.IsKeyboardFocusWithin))) 
				DeactivateMenu();
		}		
		bool lockKeyGestureEventAfterHandling = true;	   
		bool IsAltKeyPressed(KeyEventArgs e) { 
			return e.SystemKey == Key.LeftAlt;
		}   
		protected virtual bool IsAnyPopupFocused() {
			return (Keyboard.FocusedElement as DependencyObject).With(x => BarManagerHelper.GetPopup(x)) != null;			
		}
		protected virtual bool IsFocusedManager() {
			bool isFocused = FocusHelper.IsKeyboardFocusWithin(this) || FocusHelper.IsKeyboardFocused(this);
			if(IsAnyPopupFocused() || isFocused) return true;
			return NavigationTree.CurrentElement != null;
		}						
		protected virtual Window GetActiveWindow() {
			foreach(Window window in Application.Current.Windows) {
				if(window.IsActive) return window;
			}
			return null;
		}
		internal static bool SkipFloatingBarHiding { get; set; }				
		bool IsManagerInvisible {
			get {
				UIElement elem = this as UIElement;
				while(elem != null) {
					if(elem.Visibility != Visibility.Visible)
						return true;
					elem = VisualTreeHelper.GetParent(elem) as UIElement;
				}
				return false;
			}
		}
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
			Point pt = hitTestParameters.HitPoint;
			if(pt.X < 0 || pt.Y < 0 || pt.X > ActualWidth || pt.Y > ActualHeight) return null;
			return new PointHitTestResult(this, pt);
		}		
		public void AddBarToContainer(Bar bar, BarContainerControl container) {
			int row = -1;
			foreach(Bar b in container.BindedBars) {
				row = Math.Max(row, b.DockInfo.Row);
			}
			bar.DockInfo.Row = row + 1;
			bar.DockInfo.Column = 0;
			bar.DockInfo.Offset = 0;
			bar.DockInfo.Container = container;
		}
		#region layout serializaion
		public virtual void SaveLayoutToXml(string xmlFile) {
			SaveLayoutCore(xmlFile);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			RestoreLayoutCore(xmlFile);
		}
		public virtual void SaveLayoutToStream(Stream stream) {
			SaveLayoutCore(stream);
		}
		public virtual void RestoreLayoutFromStream(Stream stream) {
			RestoreLayoutCore(stream);
		}
		void SaveLayoutCore(object path) {
			DXSerializer.SerializeSingleObject(this, path, GetSerializationAppName());
		}
		void RestoreLayoutCore(object path) {
			DXSerializer.DeserializeSingleObject(this, path, GetSerializationAppName());
		}
		string GetSerializationAppName() {
			return typeof(BarManager).Name;
		}
		void ClearCollectionsBeforeRestoreLayout() {
			Bar[] bars = Bars.ToArray();
			foreach(Bar bar in bars) {
				if(bar.CreatedByCustomizationDialog)
					Bars.Remove(bar);
				else
					ClearItemLinkCollectionBeforeRestoreLayout(bar.ItemLinks);
			}
			foreach(BarItem item in Items) {
				if(item is ILinksHolder)
					ClearItemLinkCollectionBeforeRestoreLayout(((ILinksHolder)item).Links);
			}
		}		
		void ClearItemLinkCollectionBeforeRestoreLayout(BarItemLinkCollection linkCollection) {
			BarItemLinkBase[] links = linkCollection.ToArray();
			foreach(BarItemLinkBase link in links) {
				if(link.CreatedByCustomizationDialog)
					linkCollection.Remove(link);
			}
		}
		string oldVersion = String.Empty;		
		ControllerBehavior controllerBehavior;
		protected internal ControllerBehavior ControllerBehavior {
			get { return controllerBehavior ?? (controllerBehavior = CreateControllerBehavior()); }
		}
		protected virtual ControllerBehavior CreateControllerBehavior() {
			ControllerBehavior behavior = new ControllerBehavior();			
			behavior.Attach(this);
			behavior.Actions.Add(new CreateStandardLayoutAction(this));
			return behavior;
		}		
		#endregion        
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}	  
		protected virtual void OnMainMenuChanged(DependencyPropertyChangedEventArgs e) {
			Bar prev = e.OldValue as Bar;
			if(prev != null)
				prev.IsMainMenu = false;
			Bar curr = e.NewValue as Bar;
			if(curr != null)
				curr.IsMainMenu = true;
		}
		protected virtual void OnStatusBarChanged(DependencyPropertyChangedEventArgs e) {
			Bar prev = e.OldValue as Bar;
			if(prev != null)
				prev.IsStatusBar = false;
			Bar curr = e.NewValue as Bar;
			if(curr != null)
				curr.IsStatusBar = true;
		}		
		protected virtual void OnAllowUIAutomationSupportChanged(bool oldValue) {
			BarsAutomationHelper.AllowUIAutomationSupport = AllowUIAutomationSupport;
		}
		protected virtual void OnBarTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<BarManager, Bar>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				BarsAttachedBehaviorProperty);
		}
		protected virtual void OnBarTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<BarManager, Bar>.OnItemsGeneratorTemplatePropertyChanged(this, 
				e, 
				BarsAttachedBehaviorProperty);
		}
		protected static void OnMDIMergeStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarManager)d).OnMDIMergeStyleChanged((MDIMergeStyle)e.OldValue);
		}
		protected internal virtual void OnBarsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			if(e.NewValue is IEnumerable || e.OldValue is IEnumerable) {
				ItemsAttachedBehaviorCore<BarManager, Bar>.OnItemsSourcePropertyChanged(this,
					e,
					BarsAttachedBehaviorProperty,
					BarTemplateProperty,
					BarTemplateSelectorProperty,
					BarStyleProperty,
					barManager => barManager.Bars,
					barManager => new Bar(), useDefaultTemplateSelector: true);
			} else {
				PopulateItemsHelper.GenerateItems(e, () => new BarsGenerator(this));
			}
		}		
		protected override Size MeasureOverride(Size constraint) { return DockPanelLayoutHelper.MeasureDockPanelLayout(this, constraint); }
		protected override Size ArrangeOverride(Size arrangeSize) { return DockPanelLayoutHelper.ArrangeDockPanelLayout(this, arrangeSize, true, null, new IsLastChild(x => x == Child)); }
		void IBarManager.CorrectBarManagerInDesignTime() { }
		RuntimeCustomizationCollection runtimeCustomizations;
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, false, 0, XtraSerializationFlags.None)]
		public RuntimeCustomizationCollection RuntimeCustomizations { get { return runtimeCustomizations ?? (runtimeCustomizations = new RuntimeCustomizationCollection(this)); } }
		DependencyObject IRuntimeCustomizationHost.FindTarget(string targetName) {
			var scope = BarNameScope.GetScope(this);
			IEnumerable<DependencyObject> elements = null;
			if (scope != null) {
				elements = scope.GetService<IElementRegistratorService>()
				.GetElements<IFrameworkInputElement>(ScopeSearchSettings.Local).OfType<DependencyObject>();
			} else {
				elements = TreeHelper.GetChildren<DependencyObject>(this);
			}
			return elements.FirstOrDefault(x => BarManagerCustomizationHelper.GetSerializationName(x) == targetName);
		}
	}
	[Browsable(false)]
	public class ShortCutCommand : ICommand {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarItem Item { get; set; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string BarItemName { get; set; }		
		public ShortCutCommand() { }
		public ShortCutCommand(BarItem item) {
			Item = item;
		}
		#region ICommand Members
		bool ICommand.CanExecute(object parameter) {
			if(Item == null)
				return false;
			Item.ForceUpdateCanExecute();
			return Item.IsEnabled;
		}
		event EventHandler ICommand.CanExecuteChanged {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		void ICommand.Execute(object parameter) {
			if(Item != null) Item.PerformClick();
		}
		#endregion
	}
	public class BarManagerFocusHelper {
		public delegate bool IsLostFocusCheckDelegate();
		public IsLostFocusCheckDelegate IsLostFocusCheck;
		public BarManagerFocusHelper(FrameworkElement owner) {
			Owner = owner;
			SubscribeOwner();
		}
		public BarManagerFocusHelper(FrameworkElement owner, IsLostFocusCheckDelegate isLostFocusCheck)
			: this(owner) {
				IsLostFocusCheck = isLostFocusCheck;
		}
		public void SubscribeOwner() {
			UnsubscribeOwner();
			Owner.LostFocus += OnLostFocus;
		}
		public void UnsubscribeOwner() {
			Owner.LostFocus -= OnLostFocus;
		}
		public virtual void ReleaseFocus() {
			if(isLostFocus || lastFocusedElement == null) return;
			isLostFocus = true;
			lastFocusedElement.Focus();
			lastFocusedElement = null;
		}
		public virtual void CaptureFocus(bool makeOwnerFocused = true) {
			lastFocusedElement = FocusHelper.GetFocusedElement() as UIElement;
			isLostFocus = false;
			if(makeOwnerFocused) Owner.Focus();
		}
		protected FrameworkElement Owner { get; private set; }
		protected UIElement lastFocusedElement = null;
		protected bool isLostFocus = false;
		protected virtual void OnLostFocus(object sender, EventArgs e) {
			if(isLostFocus) return;
			if(IsLostFocusCheck != null) isLostFocus = IsLostFocusCheck();
			else isLostFocus = !FocusHelper.IsKeyboardFocusWithin(Owner);
		}
		public static bool IsKeyboardFocusedWithin(DependencyObject dObj) {
			return (dObj is UIElement) ? (dObj as UIElement).IsKeyboardFocusWithin : false;
		}
	}
	public static class BarManagerHelper {
		public static readonly DependencyProperty PopupProperty;
		public static readonly DependencyProperty LinksHolderProperty;
		internal static readonly DependencyPropertyKey PopupPropertyKey;
		public static readonly DependencyProperty RemovableProperty;
		static BarManagerHelper() {
			PopupPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("Popup", typeof(BarPopupBase), typeof(BarManagerHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			PopupProperty = PopupPropertyKey.DependencyProperty;
			RemovableProperty = DependencyPropertyManager.RegisterAttached("Removable", typeof(bool), typeof(BarManagerHelper), new FrameworkPropertyMetadata(false));
			LinksHolderProperty = DependencyPropertyManager.RegisterAttached("LinksHolder", typeof(ILinksHolder), typeof(BarManagerHelper), new FrameworkPropertyMetadata(null, CollectionActionHelper.SyncContainerProperty));
		}
		public static void CheckCloseAllPopups(BarManager manager, MouseEventArgs e) {
			if (manager != null)
				manager.CheckCloseAllPopups(e);
		}
		public static void AddLogicalChild(BarManager manager, UIElement child) {
			manager.AddLogicalChild(child);
		}
		public static bool CompareItemsCreatedFromItemsSource(BarItem first, BarItem second) {
			if (first == null)
				return false;
			return first.CompareWithItemCreatedFromSource(second);
		}
		public static void RemoveLogicalChild(BarManager manager, UIElement child) {
			manager.RemoveLogicalChild(child);
		}
		public static IRibbonControl GetRibbonControl(BarManager manager) {
			return manager == null ? null : manager.RibbonControl.Target as IRibbonControl;
		}
		public static IRibbonStatusBarControl GetRibbonStatusBar(BarManager manager) {
			return manager == null ? null : manager.RibbonStatusBar.Target as IRibbonStatusBarControl;
		}
		public static void SetRemovable(DependencyObject obj, bool value) {
			obj.SetValue(RemovableProperty, value);
		}
		public static bool GetRemovable(DependencyObject obj) {
			return (bool)obj.GetValue(RemovableProperty);
		}
		public static ILinksHolder GetLinksHolder(DependencyObject obj) {
			return (ILinksHolder)obj.GetValue(LinksHolderProperty);
		}
		public static void SetLinksHolder(DependencyObject obj, ILinksHolder value) {
			obj.SetValue(LinksHolderProperty, value);			
		}
		internal static void SetPopup(DependencyObject element, BarPopupBase value) { 
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PopupPropertyKey, value);
		}
		public static BarPopupBase GetPopup(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (BarPopupBase)element.GetValue(PopupProperty);
		}
		internal static PlacementMode GetPopupPlacement(BarItemLinkControl linkControl) {
			PlacementMode placement = (linkControl.IsLinkInApplicationMenu || linkControl.IsLinkControlInMenu)? PlacementMode.Right : PlacementMode.Bottom;
			Orientation orientation = GetLinkContainerOrientation(linkControl);
			if(orientation == Orientation.Vertical) {
					placement = PlacementMode.Right;
			}
			return placement;
		}
		internal static Orientation GetLinkContainerOrientation(BarItemLinkControl linkControl) {
			BarContainerControl container = linkControl.Container;
			if(container != null) return container.Orientation;
			return Orientation.Horizontal;
		}		
		public static void UpdateSeparatorsVisibility(ILinksHolder links, bool force = false) {
			BarItemLinkBase.UpdateSeparatorsVisibility(links, force);
		}
		public static string GetThemeName(DependencyObject obj) {
			string themeName;
			ThemeTreeWalker treeWalker = ThemeManager.GetTreeWalker(obj);
			themeName = treeWalker != null ? treeWalker.ThemeName : DevExpress.Xpf.Editors.Helpers.ThemeHelper.GetRealThemeName(obj);
			return themeName == Theme.DefaultThemeName ? null : themeName;
		}
		public static BarContainerControl FindContainerControl(DependencyObject d) {
			return LayoutHelper.FindParentObject<BarContainerControl>(d);
		}
		public static BarControl FindBarControl(DependencyObject d) {
			return LayoutHelper.FindParentObject<BarControl>(d);
		}
		public static BarControl FindBarControl(Bar bar) {
			return bar == null ? null : bar.DockInfo.BarControl;
		}
		public static BarManager FindBarManager(DependencyObject d) {
			BarManager manager = BarManager.GetBarManager(d);
			if(manager != null)
				return manager;
			DependencyObject node = GetParent(d);
			while(node != null) {
				manager = BarManager.GetBarManager(node);
				if(manager == null)
					manager = node as BarManager;
				if(manager != null) {
					break;
				}
				node = GetParent(node);
			}
			return manager;
		}
		[Obsolete]
		public static BarManagerInfo FindBarManagerInfo(DependencyObject d) {
#pragma warning disable 612,618
			return new BarManagerInfo(BarManager.GetBarManager(d));
#pragma warning restore 612,618
		}
		[Obsolete]
		public static BarManagerInfo GetOrFindBarManagerInfo(DependencyObject d) {
#pragma warning disable 612,618
			return FindBarManagerInfo(d);
#pragma warning restore 612,618
		}
		public static BarManager GetOrFindBarManager(DependencyObject d) {			 
			BarManager manager = BarManager.GetBarManager(d);
			if (manager != null)
				return manager;
			manager = LayoutHelper.FindLayoutOrVisualParentObject(d, x => BarManager.GetBarManager(d) != null, true).With(x => BarManager.GetBarManager(x));			
			return manager;
		}		
		public static ImageSource GetDefaultBarItemGlyph(bool isLarge) {
			return ImageHelper.CreateImageFromCoreEmbeddedResource("Bars.Images.Default_" + (isLarge ? "32x32" : "16x16") + ".png");
		}
		static DependencyObject GetParent(DependencyObject d) { 
			return LayoutHelper.GetParent(d, true) ?? (d is Visual ? VisualTreeHelper.GetParent(d) : null);
		}
		public static Popup GetItemLinkControlPopup(BarItemLinkControlBase control) {
			if(control is BarSubItemLinkControl) {
				return ((BarSubItemLinkControl)control).Popup;
			}
			if(control is BarSplitButtonItemLinkControl && ((BarSplitButtonItemLinkControl)control).GetPopupControl()!=null) {
				return ((BarSplitButtonItemLinkControl)control).GetPopupControl().Popup;
			}
			if(control is BarSplitCheckItemLinkControl && ((BarSplitCheckItemLinkControl)control).GetPopupControl()!=null) {
				return ((BarSplitCheckItemLinkControl)control).GetPopupControl().Popup;
			}
			return null;
		}
		public static Image GetItemLinkControlGlyph(BarItemLinkControl control) {
			return control!=null && control.LayoutPanel!=null ? control.LayoutPanel.ElementGlyph : null;
		}
		public static FrameworkElement GetItemLinkControlGlyphBorder(BarItemLinkControl control) {
			return null;
		}
		public static FrameworkElement GetItemLinkControlArrow(BarItemLinkControl control) {
			if(control == null || control.LayoutPanel == null) return null;
			if(control is BarSplitButtonItemLinkControl) {
				return ((BarSplitButtonItemLinkControl)control).LayoutPanel.ElementArrowControl;
			}
			if(control is BarSplitCheckItemLinkControl) {
				return ((BarSplitCheckItemLinkControl)control).LayoutPanel.ElementArrowControl;
			}
			return null;
		}		
		public static void InitializeItemLinks(BarManager manager, BarItemLinkCollection links) {
			if(manager != null && links != null)
				manager.InitializeItemLinks(links);		   
		}
		public static bool Merge(BarManager parentManager, BarManager childManager, ILinksHolder extraItems) {
			return BarManagerMergingHelper.Merge(parentManager, childManager, extraItems);
		}
		public static bool IsMergedParent(BarManager parentManager) {
			return BarManagerMergingHelper.IsMergedParent(parentManager);
		}
		public static bool IsMergedChild(BarManager childManager) {
			return BarManagerMergingHelper.IsMergedChild(childManager);
		}
		public static void UnMerge(BarManager parentManager, BarManager childManager, ILinksHolder extraItems) {
			BarManagerMergingHelper.UnMerge(parentManager, childManager, extraItems);
		}
		public static void ShowFloatingBars(DependencyObject treeNode) {
			ShowFloatingBars(treeNode, false);
		}
		public static void HideFloatingBars(DependencyObject treeNode) {
			HideFloatingBars(treeNode, false);
		}
		public static void ShowFloatingBars(DependencyObject treeNode, bool includeChildren) {
			var scope = BarNameScope.FindScope(treeNode);
			if (scope == null)
				return;
			foreach (var innerScope in scope.ScopeTree.Find(searchDirection: ScopeSearchSettings.Local | ScopeSearchSettings.Descendants)) {
				innerScope.GetService<ICustomizationService>().RestoreCustomizationForm();
				innerScope.GetService<IElementRegistratorService>().GetElements<IFrameworkInputElement>().OfType<Bar>().ToList().ForEach(x => x.ShowIfFloating());
				if (!includeChildren)
					break;
			}
		}
		public static void HideFloatingBars(DependencyObject treeNode, bool includeChildren) {
			HideFloatingBars(treeNode, includeChildren, true);
		}
		public static void HideFloatingBars(DependencyObject treeNode, bool includeChildren, bool hideForm) {
			var scope = BarNameScope.FindScope(treeNode);
			if (scope == null)
				return;
			foreach (var innerScope in scope.ScopeTree.Find(searchDirection: ScopeSearchSettings.Local | ScopeSearchSettings.Descendants)) {
				if (hideForm)
					innerScope.GetService<ICustomizationService>().HideCustomizationForm();
				innerScope.GetService<IElementRegistratorService>().GetElements<IFrameworkInputElement>().OfType<Bar>().ToList().ForEach(x => x.HideIfFloating());
				if (!includeChildren)
					break;
			}
		}
		public static void SetChildRibbonStatusBar(BarManager manager, object ribbonStatusBar) {
			manager.RibbonStatusBar = new WeakReference(ribbonStatusBar);
		}
		public static void SetChildRibbonControl(BarManager manager, object ribbonControl) {
			manager.RibbonControl = new WeakReference(ribbonControl);
		}
		public static object GetChildRibbonStatusBar(BarManager manager) {
			if(manager == null || manager.RibbonStatusBar == null)
				return null;
			return manager.RibbonStatusBar.IsAlive ? manager.RibbonStatusBar.Target : null;
		}
		public static object GetChildRibbonControl(BarManager manager) {
			if(manager==null || manager.RibbonControl == null)
				return null;
			return manager.RibbonControl.IsAlive ? manager.RibbonControl.Target : null;
		}
		internal static BarItemLinkBase GetLinkByItemName(string itemName, BarItemLinkCollection links) {
			foreach(BarItemLinkBase link in links) {
				if(link is BarItemLink) {
					if(((BarItemLink)link).BarItemName == itemName)
						return link;
					if(((BarItemLink)link).Item is ILinksHolder) {
						BarItemLinkBase itemLink = GetLinkByItemName(itemName, ((ILinksHolder)((BarItemLink)link).Item).Links);
						if(itemLink != null)
							return itemLink;
					}
				}
			}
			return null;
		}
		public static void SetBarIsPrivate(Bar bar, bool isPrivate) {
			if(bar == null) return;
			bar.IsPrivate = isPrivate;
		}
		public static bool GetIsPrivate(Bar bar) {
			return bar.IsPrivate;
		}
		public static IEnumerable<BarItem> GetPrivateItems(BarManager manager) {
			return manager.Items.AsEnumerable().Where(item => item.IsPrivate);
		}
	}	
	public class BarItemLinksAsLogicalChildrenEnumerator : IEnumerator {
		protected ILinksHolder Holder { get; set; }
		protected IEnumerator Enumerator { get; set; }
		public BarItemLinksAsLogicalChildrenEnumerator(ILinksHolder holder) {
			Holder = holder;
			Enumerator = Holder.Links.GetEnumerator();
		}
		#region IEnumerator Members
		object IEnumerator.Current {
			get {
				return Enumerator.Current;
			}
		}
		bool IEnumerator.MoveNext() {
			BarItemLinkBase linkBase = null;
			while(linkBase == null || linkBase.IsPrivate) {
				if(!Enumerator.MoveNext()) return false;
				linkBase = Enumerator.Current as BarItemLinkBase;
			}
			return true;
		}
		void IEnumerator.Reset() {
			Enumerator.Reset();
		}
		#endregion
	}
	internal static class InvokeHelper {
		public static void InvokeIfRequired(this BarManager manager, Action operation) {
			if(manager.Dispatcher.CheckAccess()) {
				operation();
			}
			else {
				manager.Dispatcher.BeginInvoke(operation);
			}
		}
	}
	public interface IMDIChildHost {
		bool IsChildMenuVisible { get; }
		event EventHandler IsChildMenuVisibleChanged;
	}
	public interface IBarManager {
		void CorrectBarManagerInDesignTime();
	}
	public enum ButtonSwitcher {
		LeftButton,
		RightButton,
		LeftRightButton
	}
	[Obsolete]
	public class BarManagerInfo {
		public BarManager Manager { get; private set; }
		public BarManagerInfo(BarManager manager) {
			Manager = manager;
		}
	}
public delegate void LayoutUpgradingEventHandler (object sender, LayoutUpgradingEventArgs args );
	public class LayoutUpgradingEventArgs : RoutedEventArgs {
		public LayoutUpgradingEventArgs(BarManager manager)
			: base() {
		}
		public BarManagerActionCollection NewItems { get; protected internal set; }
	}	
	[Browsable(false)]
	public class BarManagerThemeDependentValuesProvider : FrameworkElement {
		public static readonly DependencyProperty ToolbarCaptionEditorWindowFloatSizeProperty;
		public static readonly DependencyProperty CustomizationFormMinWidthProperty;
		public static readonly DependencyProperty CustomizationFormMinHeightProperty;
		public static readonly DependencyProperty CustomizationFormFloatSizeProperty;		
		public static readonly DependencyProperty ColorizeGlyphProperty;
		static BarManagerThemeDependentValuesProvider() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarManagerThemeDependentValuesProvider), new FrameworkPropertyMetadata(typeof(BarManagerThemeDependentValuesProvider)));
			ToolbarCaptionEditorWindowFloatSizeProperty = DependencyPropertyManager.Register("ToolbarCaptionEditorWindowFloatSize", typeof(SizeEx), typeof(BarManagerThemeDependentValuesProvider), new FrameworkPropertyMetadata(new SizeEx()));
			CustomizationFormMinWidthProperty = DependencyPropertyManager.Register("CustomizationFormMinWidth", typeof(double), typeof(BarManagerThemeDependentValuesProvider), new FrameworkPropertyMetadata(0d));
			CustomizationFormMinHeightProperty = DependencyPropertyManager.Register("CustomizationFormMinHeight", typeof(double), typeof(BarManagerThemeDependentValuesProvider), new FrameworkPropertyMetadata(0d));
			CustomizationFormFloatSizeProperty = DependencyPropertyManager.Register("CustomizationFormFloatSize", typeof(SizeEx), typeof(BarManagerThemeDependentValuesProvider), new FrameworkPropertyMetadata(new SizeEx()));			
			ColorizeGlyphProperty = DependencyPropertyManager.Register("ColorizeGlyph", typeof(bool), typeof(BarManagerThemeDependentValuesProvider), new FrameworkPropertyMetadata(false, (d, e) => ((BarManagerThemeDependentValuesProvider)d).Manager.UpdateGlyphColorization()));
		}
		public SizeEx ToolbarCaptionEditorWindowFloatSize {
			get { return (SizeEx)GetValue(ToolbarCaptionEditorWindowFloatSizeProperty); }
			set { SetValue(ToolbarCaptionEditorWindowFloatSizeProperty, value); }
		}
		public SizeEx CustomizationFormFloatSize {
			get { return (SizeEx)GetValue(CustomizationFormFloatSizeProperty); }
			set { SetValue(CustomizationFormFloatSizeProperty, value); }
		}
		public double CustomizationFormMinHeight {
			get { return (double)GetValue(CustomizationFormMinHeightProperty); }
			set { SetValue(CustomizationFormMinHeightProperty, value); }
		}
		public double CustomizationFormMinWidth {
			get { return (double)GetValue(CustomizationFormMinWidthProperty); }
			set { SetValue(CustomizationFormMinWidthProperty, value); }
		}
		public bool ColorizeGlyph {
			get { return (bool)GetValue(ColorizeGlyphProperty); }
			set { SetValue(ColorizeGlyphProperty, value); }
		}		
		protected BarManager Manager { get; private set; }
		public BarManagerThemeDependentValuesProvider(BarManager manager) {			
			this.Manager = manager;
		}
	}
	public class SizeEx {
		public double Width { get; set; }
		public double Height { get; set; }
		public SizeEx() {
			Width = 0d;
			Height = 0d;
		}
		public SizeEx(double w, double h) {
			this.Width = w;
			this.Height = h;
		}		
		public static implicit operator Size (SizeEx second) {
			return new Size(second.Width, second.Height);
		}
	}
	[TypeConverter(typeof(ElementMergingBehaviorTypeConverter))]
	public enum ElementMergingBehavior {
		Undefined,
		None,		
		All,
		InternalWithInternal,
		InternalWithExternal,		
	}	
	[Flags]
	public enum ToolBarMergeStyle {
		None		= 0x0,
		MainMenu	= 0x1,
		StatusBar   = 0x2,
		ToolBars	= 0x4,
		MainMenuAndStatusBar = MainMenu | StatusBar,
		MainMenuAndToolBars = MainMenu | ToolBars,
		StatusBarAndToolBars = StatusBar | ToolBars,
		All = MainMenu | StatusBar | ToolBars,
	}
	class BarManagerILogicalChildrenContainerWrapper : ILogicalChildrenContainer {
		List<object> children;
		BarManager manager;
		public BarManagerILogicalChildrenContainerWrapper(BarManager manager) {
			this.manager = manager;
			this.children = new List<object>();
		}
		public IEnumerable LogicalChildren {
			get { return children; }
		}
		public void AddLogicalChild(object child) {
			if ((child as DependencyObject).With(LogicalTreeHelper.GetParent) != null)
				return;
			children.Add(child);
			manager.AddLogicalChild(child);
		}
		public void RemoveLogicalChild(object child) {
			if (!children.Remove(child))
				return;
			manager.RemoveLogicalChild(child);
		}
	}
	public static class MergingProperties {
		class RegionID {
			int hCode;
			RegionID parent;
			public RegionID(RegionID parent) {
				this.parent = parent;
				hCode = GetHashCodeInternal();
			}
			int GetHashCodeInternal() {
				unchecked
				{
					if (parent == null)
						return base.GetHashCode();
					return (base.GetHashCode() * 397) ^ parent.GetHashCode();
				}				
			}					
			public override bool Equals(object obj) {
				if (ReferenceEquals(null, obj)) return false;
				return obj is RegionID && Equals((RegionID)obj);
			}			
			public override int GetHashCode() {
				return hCode;
			}
			bool Equals(RegionID obj) {
				return hCode == obj.hCode;
			}
		}
		public static readonly DependencyProperty ToolBarMergeStyleProperty = DependencyPropertyManager.RegisterAttached("ToolBarMergeStyle", typeof(ToolBarMergeStyle), typeof(MergingProperties), new FrameworkPropertyMetadata(ToolBarMergeStyle.All, FrameworkPropertyMetadataOptions.Inherits));
		[Obsolete("Use the ElementMergingBehavior property instead")]
		public static readonly DependencyProperty AllowMergingProperty = DependencyPropertyManager.RegisterAttached("AllowMerging", typeof(bool?), typeof(MergingProperties), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAllowMergingPropertyChanged)));
#pragma warning disable 612,618
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly internal DependencyProperty AllowMergingInternalProperty = AllowMergingProperty;
#pragma warning disable 612,618
		public static readonly DependencyProperty NameProperty = DependencyPropertyManager.RegisterAttached("Name", typeof(string), typeof(MergingProperties), new FrameworkPropertyMetadata(null));
		public static string GetName(DependencyObject obj) { return (string)obj.GetValue(NameProperty); }
		public static void SetName(DependencyObject obj, string value) { obj.SetValue(NameProperty, value); }
		[Obsolete("Use the ElementMergingBehavior property instead")]
		public static bool? GetAllowMerging(DependencyObject obj) { return (bool?)obj.GetValue(AllowMergingProperty); }
		[Obsolete("Use the ElementMergingBehavior property instead")]
		public static void SetAllowMerging(DependencyObject obj, bool? value) { obj.SetValue(AllowMergingProperty, value); }
		public static ToolBarMergeStyle GetToolBarMergeStyle(DependencyObject obj) { return (ToolBarMergeStyle)obj.GetValue(ToolBarMergeStyleProperty); }
		public static void SetToolBarMergeStyle(DependencyObject obj, ToolBarMergeStyle value) { obj.SetValue(ToolBarMergeStyleProperty, value); }
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty OverridesAllowMergingBehaviorProperty = DependencyPropertyManager.RegisterAttached("OverridesAllowMergingBehavior", typeof(bool), typeof(MergingProperties), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnOverridesAllowMergingBehaviorPropertyChanged)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty BlockMergingRegionProperty = DependencyPropertyManager.RegisterAttached("BlockMergingRegion", typeof(bool), typeof(MergingProperties), new FrameworkPropertyMetadata(false, (d, e) => d.CoerceValue(BlockMergingRegionIDCoreProperty)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty MergingRegionProperty = DependencyPropertyManager.RegisterAttached("MergingRegion", typeof(bool), typeof(MergingProperties), new FrameworkPropertyMetadata(false, (d,e)=>d.CoerceValue(RegionIDCoreProperty)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty AllowMergingCoreProperty = DependencyPropertyManager.RegisterAttached("AllowMergingCore", typeof(bool?), typeof(MergingProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, null, new CoerceValueCallback(CoerceAllowMergingCoreProperty)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty BlockMergingRegionIDCoreProperty = DependencyPropertyManager.RegisterAttached("BlockMergingRegionIDCore", typeof(RegionID), typeof(MergingProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, null, new CoerceValueCallback(CoerceBlockMergingRegionIDCore)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty RegionIDCoreProperty = DependencyPropertyManager.RegisterAttached("RegionIDCore", typeof(RegionID), typeof(MergingProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, null, new CoerceValueCallback(CoerceRegionIDCore)));
		static object CoerceRegionIDCore(DependencyObject d, object baseValue) {
			if (!((bool)d.GetValue(MergingRegionProperty)))
				return baseValue;
			return new RegionID((RegionID)baseValue);
		}
		static object CoerceBlockMergingRegionIDCore(DependencyObject d, object baseValue) {
			if (!((bool)d.GetValue(BlockMergingRegionProperty)))
				return baseValue;
			return new RegionID((RegionID)baseValue);
		}
		public static readonly DependencyProperty ElementMergingBehaviorProperty = DependencyPropertyManager.RegisterAttached("ElementMergingBehavior", typeof(ElementMergingBehavior), typeof(MergingProperties), new FrameworkPropertyMetadata(ElementMergingBehavior.Undefined, new PropertyChangedCallback(OnElementMergingBehaviorPropertyChanged)));
		public static ElementMergingBehavior GetElementMergingBehavior(DependencyObject obj) { return (ElementMergingBehavior)obj.GetValue(ElementMergingBehaviorProperty); }
		public static void SetElementMergingBehavior(DependencyObject obj, ElementMergingBehavior value) { obj.SetValue(ElementMergingBehaviorProperty, value); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty HideElementsProperty = DependencyPropertyManager.RegisterAttached("HideElements", typeof(bool), typeof(MergingProperties), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool GetHideElements(DependencyObject obj) { return (bool)obj.GetValue(HideElementsProperty); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetHideElements(DependencyObject obj, bool value) { obj.SetValue(HideElementsProperty, value); }
#if DEBUGTEST
		public
#else
		internal
#endif
		static bool? GetAllowMergingCore(DependencyObject obj) { return (bool?)obj.GetValue(AllowMergingCoreProperty); }
		static object CoerceAllowMergingCoreProperty(DependencyObject d, object baseValue) {
			bool? bBaseValue = (bool?)baseValue;
			if ((bool)d.GetValue(OverridesAllowMergingBehaviorProperty))
				return GetAllowMerging(d);
			if (Equals(false, bBaseValue)) {
				return false;
			}			
			var allowMerging = GetAllowMerging(d);
			if (allowMerging.HasValue)
				return allowMerging;
			return bBaseValue;
		}
		internal static bool CheckRegions(object first, object second) {
			var doFirst = first as DependencyObject;
			var doSecond = second as DependencyObject;
			if (doFirst == null || doSecond == null)
				return false;
			var firstRegion = (RegionID)doFirst.GetValue(RegionIDCoreProperty);
			var secondRegion = (RegionID)doSecond.GetValue(RegionIDCoreProperty);
			var firstBlock = (RegionID)doFirst.GetValue(BlockMergingRegionIDCoreProperty);
			var secondBlock = (RegionID)doSecond.GetValue(BlockMergingRegionIDCoreProperty);
			var sameRegionConflict = firstRegion != null && secondRegion != null && Equals(firstRegion, secondRegion);
			var differentBlockRegionConflict = !Equals(firstBlock, secondBlock);
			return !sameRegionConflict && !differentBlockRegionConflict;
		}
		static void OnElementMergingBehaviorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var behavior = (ElementMergingBehavior)e.NewValue;
			if (behavior == ElementMergingBehavior.None) {
				d.SetValue(AllowMergingProperty, false);
			} else if (behavior == ElementMergingBehavior.Undefined) {
				d.ClearValue(AllowMergingProperty);
			} else {
				d.SetValue(AllowMergingProperty, true);
			}
			if(behavior!= ElementMergingBehavior.None) {
				if (System.Windows.DependencyPropertyHelper.GetValueSource(d, BarNameScope.IsScopeOwnerProperty).BaseValueSource == BaseValueSource.Default)
					BarNameScope.SetIsScopeOwner(d, true);
			}
			switch (behavior) {
				case ElementMergingBehavior.All:
				case ElementMergingBehavior.Undefined:				
					d.ClearValue(OverridesAllowMergingBehaviorProperty);
					d.SetValue(BlockMergingRegionProperty, false);
					d.SetValue(MergingRegionProperty, false);
					break;
				case ElementMergingBehavior.None:
				case ElementMergingBehavior.InternalWithInternal:
					d.SetValue(OverridesAllowMergingBehaviorProperty, true);
					d.SetValue(BlockMergingRegionProperty, true);
					d.SetValue(MergingRegionProperty, false);
					break;
				case ElementMergingBehavior.InternalWithExternal:
					d.SetValue(OverridesAllowMergingBehaviorProperty, true);
					d.SetValue(BlockMergingRegionProperty, false);
					d.SetValue(MergingRegionProperty, true);
					break;
			}
		}
		static void OnOverridesAllowMergingBehaviorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			d.CoerceValue(AllowMergingCoreProperty);
		}
		static void OnAllowMergingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			d.CoerceValue(AllowMergingCoreProperty);
		}
		static string GetActualString(params object[] param) {
			return (string)param.FirstOrDefault(x => !String.IsNullOrEmpty(x as string));
		}
	}
	public abstract class BaseBarManagerSerializationStrategy {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static BaseBarManagerSerializationStrategy GetStrategy(DependencyObject obj) { return (BaseBarManagerSerializationStrategy)obj.GetValue(StrategyProperty); }
		public static void SetStrategy(DependencyObject obj, BaseBarManagerSerializationStrategy value) { obj.SetValue(StrategyProperty, value); }
		public static readonly DependencyProperty StrategyProperty = DependencyProperty.RegisterAttached("Strategy", typeof(BaseBarManagerSerializationStrategy), typeof(BaseBarManagerSerializationStrategy), new PropertyMetadata(null));
	}
	public abstract class BaseBarManagerSerializationStrategyGeneric<TOwner> : BaseBarManagerSerializationStrategy where TOwner : DependencyObject {
		readonly TOwner owner;
		protected TOwner Owner { get { return owner; } }
		public BaseBarManagerSerializationStrategyGeneric(TOwner owner) {
			this.owner = owner;
			SetStrategy(owner, this);
			SubscribeEvents();
		}
		void SubscribeEvents() {
			AddHandler(DXSerializer.StartSerializingEvent, new RoutedEventHandler(OnStartSerializingEvent));
			AddHandler(DXSerializer.EndSerializingEvent, new RoutedEventHandler(OnEndSerializingEvent));
			AddHandler(DXSerializer.StartDeserializingEvent, new StartDeserializingEventHandler(OnStartDeserializingEvent));
			AddHandler(DXSerializer.EndDeserializingEvent, new EndDeserializingEventHandler(OnEndDeserializingEvent));
			AddHandler(DXSerializer.CustomGetSerializableChildrenEvent, new CustomGetSerializableChildrenEventHandler(OnCustomGetSerializableChildrenEvent));
			AddHandler(DXSerializer.CustomGetSerializablePropertiesEvent, new CustomGetSerializablePropertiesEventHandler(OnCustomGetSerializablePropertiesEvent));
			AddHandler(DXSerializer.ClearCollectionEvent, new XtraItemRoutedEventHandler(OnClearCollectionEvent));
			AddHandler(DXSerializer.CreateCollectionItemEvent, new XtraCreateCollectionItemEventHandler(OnCreateCollectionItemEvent));
			AddHandler(DXSerializer.FindCollectionItemEvent, new XtraFindCollectionItemEventHandler(OnFindCollectionItemEvent));
			AddHandler(DXSerializer.AllowPropertyEvent, new AllowPropertyEventHandler(OnAllowPropertyEvent));
			AddHandler(DXSerializer.CustomShouldSerializePropertyEvent, new CustomShouldSerializePropertyEventHandler(OnCustomShouldSerializePropertyEvent));
			AddHandler(DXSerializer.ResetPropertyEvent, new ResetPropertyEventHandler(OnResetPropertyEvent));
			AddHandler(DXSerializer.DeserializePropertyEvent, new XtraPropertyInfoEventHandler(OnDeserializePropertyEvent));
			AddHandler(DXSerializer.CreateContentPropertyValueEvent, new XtraCreateContentPropertyValueEventHandler(OnCreateContentPropertyValueEvent));
			AddHandler(DXSerializer.BeforeLoadLayoutEvent, new BeforeLoadLayoutEventHandler(OnBeforeLoadLayoutEvent));
			AddHandler(DXSerializer.LayoutUpgradeEvent, new DevExpress.Xpf.Core.Serialization.LayoutUpgradeEventHandler(OnLayoutUpgradeEvent));
			AddHandler(DXSerializer.ShouldSerializeCollectionItemEvent, new XtraShouldSerializeCollectionItemEventHandler(OnShouldSerializeCollectionItemEvent));
		}
		void OnStartSerializingEvent(object sender, RoutedEventArgs e) {
			if (e.Source != Owner)
				return;
			OnStartSerializing(e);
		}		
		private void OnEndSerializingEvent(object sender, RoutedEventArgs e) {
			if (e.Source != Owner)
				return;
			OnEndSerializing(e);
		}
		private void OnStartDeserializingEvent(object sender, StartDeserializingEventArgs e) {
			if (e.Source != Owner)
				return;
			OnStartDeserializing(e);
		}
		private void OnEndDeserializingEvent(object sender, EndDeserializingEventArgs e) {
			if (e.Source != Owner)
				return;
			OnEndDeserializing(e);
		}
		private void OnCustomGetSerializableChildrenEvent(object sender, CustomGetSerializableChildrenEventArgs e) {
			if (e.Source != Owner)
				return;
			OnCustomGetSerializableChildren(e);
		}
		private void OnCustomGetSerializablePropertiesEvent(object sender, CustomGetSerializablePropertiesEventArgs e) {
			if (e.Source != Owner)
				return;
			OnCustomGetSerializableProperties(e);
		}
		private void OnClearCollectionEvent(object sender, XtraItemRoutedEventArgs e) {
			if (e.Source != Owner)
				return;
			OnClearCollection(e);
		}
		private void OnCreateCollectionItemEvent(object sender, XtraCreateCollectionItemEventArgs e) {
			if (e.Source != Owner)
				return;
			OnCreateCollectionItem(e);
		}
		private void OnFindCollectionItemEvent(object sender, XtraFindCollectionItemEventArgs e) {
			if (e.Source != Owner)
				return;
			OnFindCollectionItem(e);
		}
		private void OnAllowPropertyEvent(object sender, AllowPropertyEventArgs e) {
			if (e.Source != Owner)
				return;
			OnAllowProperty(e);
		}
		private void OnCustomShouldSerializePropertyEvent(object sender, CustomShouldSerializePropertyEventArgs e) {
			if (e.Source != Owner)
				return;
			OnCustomShouldSerializeProperty(e);
		}
		private void OnResetPropertyEvent(object sender, ResetPropertyEventArgs e) {
			if (e.Source != Owner)
				return;
			OnResetProperty(e);
		}
		private void OnDeserializePropertyEvent(object sender, XtraPropertyInfoEventArgs e) {
			if (e.Source != Owner)
				return;
			OnDeserializeProperty(e);
		}
		private void OnCreateContentPropertyValueEvent(object sender, XtraCreateContentPropertyValueEventArgs e) {
			if (e.Source != Owner)
				return;
			OnCreateContentPropertyValue(e);
		}
		private void OnBeforeLoadLayoutEvent(object sender, BeforeLoadLayoutEventArgs e) {
			if (e.Source != Owner)
				return;
			OnBeforeLoadLayoutEvent(e);
		}
		private void OnLayoutUpgradeEvent(object sender, Core.Serialization.LayoutUpgradeEventArgs e) {
			if (e.Source != Owner)
				return;
			OnLayoutUpgrade(e);
		}
		private void OnShouldSerializeCollectionItemEvent(object sender, XtraShouldSerailizeCollectionItemEventArgs e) {
			if (e.Source != Owner)
				return;
			OnShouldSerializeCollectionItem(e);
		}
		protected virtual void OnStartSerializing(RoutedEventArgs e) { }
		protected virtual void OnEndSerializing(RoutedEventArgs e) { }
		protected virtual void OnStartDeserializing(StartDeserializingEventArgs e) { }
		protected virtual void OnEndDeserializing(EndDeserializingEventArgs e) { }
		protected virtual void OnCustomGetSerializableChildren(CustomGetSerializableChildrenEventArgs e) { }
		protected virtual void OnCustomGetSerializableProperties(CustomGetSerializablePropertiesEventArgs e) { }
		protected virtual void OnClearCollection(XtraItemRoutedEventArgs e) { }
		protected virtual void OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e) { }
		protected virtual void OnFindCollectionItem(XtraFindCollectionItemEventArgs e) { }
		protected virtual void OnAllowProperty(AllowPropertyEventArgs e) { }
		protected virtual void OnCustomShouldSerializeProperty(CustomShouldSerializePropertyEventArgs e) { }
		protected virtual void OnResetProperty(ResetPropertyEventArgs e) { }
		protected virtual void OnDeserializeProperty(XtraPropertyInfoEventArgs e) { }
		protected virtual void OnCreateContentPropertyValue(XtraCreateContentPropertyValueEventArgs e) { }
		protected virtual void OnBeforeLoadLayoutEvent(BeforeLoadLayoutEventArgs e) { }
		protected virtual void OnLayoutUpgrade(Core.Serialization.LayoutUpgradeEventArgs e) { }
		protected virtual void OnShouldSerializeCollectionItem(XtraShouldSerailizeCollectionItemEventArgs e) { }
		void AddHandler(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo = false) {
			if (owner is FrameworkElement)
				(owner as FrameworkElement).AddHandler(routedEvent, handler, handledEventsToo);
			if (owner is FrameworkContentElement)
				(owner as FrameworkContentElement).AddHandler(routedEvent, handler, handledEventsToo);
		}
	}
	public class BarManagerSerializationStrategy : RuntimeCustomizationHostSerializationStrategy<BarManager> {
		public BarManagerSerializationStrategy(BarManager manager) : base(manager) { }
	}
	public abstract class RuntimeCustomizationHostSerializationStrategy<THost> : BaseBarManagerSerializationStrategyGeneric<THost> where THost : DependencyObject, IRuntimeCustomizationHost {
		static RuntimeCustomizationHostSerializationStrategy() {
			RuntimeCustomization.RegisterCustomization(() => new RuntimePropertyCustomization());
			RuntimeCustomization.RegisterCustomization(() => new RuntimeUndoCustomization());
			RuntimeCustomization.RegisterCustomization(() => new RuntimeCopyLinkCustomization());
			RuntimeCustomization.RegisterCustomization(() => new RuntimeRemoveLinkCustomization());
			RuntimeCustomization.RegisterCustomization(() => new RuntimeCreateNewBarCustomization());
			BarNameScopeTreeWalker.WalkerProperty.AddPropertyChangedCallback(typeof(THost), OnScopeWalkerPropertyChanged);
		}
		static void OnScopeWalkerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var strategy = GetStrategy(d) as RuntimeCustomizationHostSerializationStrategy<THost>;
			if (strategy != null)
				strategy.OnScopeWalkerChanged(e.NewValue != null);
		}
		PostponedAction applyAction;
		public RuntimeCustomizationHostSerializationStrategy(THost owner) : base(owner) {
			applyAction = new PostponedAction(() => BarNameScopeTreeWalker.GetWalker(owner) == null);
		}
		protected override void OnStartDeserializing(StartDeserializingEventArgs e) {
			base.OnStartDeserializing(e);
			InitializeElementNames();
			Owner.RuntimeCustomizations.Undo(true);
		}
		protected virtual void InitializeElementNames() {
			BarNameScope
				.GetService<IElementRegistratorService>(Owner)
				.GetElements<IFrameworkInputElement>(ScopeSearchSettings.Local)
				.OfType<DependencyObject>()
				.ForEach(x => BarManagerCustomizationHelper.GetSerializationName(x));
		}
		protected override void OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			if (e.CollectionName == "RuntimeCustomizations") {
				var typeName = e.Item.ChildProperties["CustomizationType"].Value as string;
				var customization = RuntimeCustomization.CreateInstance(typeName) ?? new ObjectCreator().Create(typeName) as RuntimeCustomization;
				e.CollectionItem = customization;
				Owner.RuntimeCustomizations.Add(customization);
			}
			base.OnCreateCollectionItem(e);
		}
		protected override void OnEndDeserializing(EndDeserializingEventArgs e) {
			base.OnEndDeserializing(e);
			applyAction.PerformPostpone(ApplyCustomizations);
		}
		protected virtual void OnScopeWalkerChanged(bool hasWalker) {
			if (hasWalker)
				Owner.Dispatcher.BeginInvoke(new Action(applyAction.PerformForce));
		}
		protected virtual void ApplyCustomizations() {
			InitializeElementNames();
			Owner.RuntimeCustomizations.Apply();
		}
	}
	public interface IRuntimeCustomizationHost {
		DependencyObject FindTarget(string targetName);
		RuntimeCustomizationCollection RuntimeCustomizations { get; }
	}
	public class RuntimeCustomizationCollection : ObservableCollection<RuntimeCustomization> {
		IRuntimeCustomizationHost host;
		public RuntimeCustomizationCollection(IRuntimeCustomizationHost host) {
			this.host = host;
		}
		protected override void InsertItem(int index, RuntimeCustomization item) {
			if (executionLocker.IsLocked)
				return;
			int correction = 0;
			if (item.Overwrite && !String.IsNullOrEmpty(item.TargetName)) {
				for (int i = Count - 1; i >= 0; i--) {
					if (item.Applied) {
						if (item.Owerwrite(this[i])) {
							RemoveAt(i);
							if (i <= index)
								correction++;
						}
					}
				}
			}
			index -= correction;
			base.InsertItem(index, item);
			item.Host = host;
		}
		protected override void RemoveItem(int index) {
			var item = this[index];
			item.Host = null;
			base.RemoveItem(index);
		}
		readonly Locker executionLocker = new Locker();
		public void Apply() {
			Apply(0);
		}
		public void Apply(int startingIndex) {
			using (executionLocker.Lock()) {
				for(int i = startingIndex; i < Count; i++) {					
					this[i].Apply();
				}
				for (int i = 0; i < Count; i++) {
					for (int j = 0; j < i; j++)
						if (this[i].Owerwrite(this[j])) {
							RemoveAt(j);
							i--;
							j--;
						}
				}
			}
		}
		public void Undo(int startingIndex, bool clear = false, bool nonAppliedOnly = false) {
			using (executionLocker.Lock()) {
				if (!nonAppliedOnly) {
					for (int i = Count - 1; i >= startingIndex; i--) {
						this[i].Undo();
					}
				}
				if (clear) {
					if (nonAppliedOnly) {
						for (int i = Count - 1; i >= startingIndex; i--) {
							if (!this[i].Applied)
								RemoveAt(i);
						}
					} else {
						Clear();
					}
				}
			}
		}
		public void Undo(bool clear = false, bool nonappliedOnly = false) {
			Undo(0, clear, nonappliedOnly);
		}		
		public IDisposable Lock() { return executionLocker.Lock(); }
		public void Unlock() { executionLocker.Unlock(); }
	}
	public abstract class RuntimeCustomization {
		static readonly Func<string, Type, DependencyProperty> fromName;
		static readonly Dictionary<string, Func<RuntimeCustomization>> allocators;
		public static void RegisterCustomization<T>(Func<T> createInstance) where T : RuntimeCustomization {
			allocators[typeof(T).FullName] = createInstance;
		}
		public static RuntimeCustomization CreateInstance(string typeName) {
			Func<RuntimeCustomization> rCust = null;
			if (allocators.TryGetValue(typeName, out rCust)) {
				return rCust();
			} return null;
		}
		readonly List<string> affectedTargets;
		static RuntimeCustomization() {
			allocators = new Dictionary<string, Func<RuntimeCustomization>>();
			fromName = DevExpress.Xpf.Core.Internal.ReflectionHelper.CreateInstanceMethodHandler<DependencyProperty, Func<string, Type, DependencyProperty>>(null, "FromName", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
		}
		protected DependencyProperty FromName(DependencyObject dObj, string propertyName) { return fromName(propertyName, dObj.GetType()); }
		protected virtual DependencyObject FindTarget() {
			return forcedTarget ?? Host.FindTarget(TargetName);
		}
		public bool Applied { get; private set; }
		[XtraSerializableProperty]
		public string TargetName { get; set; }
		[XtraSerializableProperty]
		public bool Overwrite { get; set; }
		[XtraSerializableProperty]
		public int Timestamp { get; set; }
		DependencyObject forcedTarget;
		[XtraSerializableProperty]
		public string CustomizationType { get { return this.GetType().FullName; } set { } }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, false, 0, XtraSerializationFlags.None)]
		public List<string> AffectedTargets { get { return affectedTargets; } }
		protected internal IRuntimeCustomizationHost Host { get; set; }
		public RuntimeCustomization() : this(null) {			
		}
		public RuntimeCustomization(DependencyObject forcedTarget) { 
			this.forcedTarget = forcedTarget;
			this.TargetName = forcedTarget.With(BarManagerCustomizationHelper.GetSerializationName);
			this.Timestamp = Environment.TickCount;
			affectedTargets = new List<string>(0);
		}
		public void Apply() {
			if (Applied)
				return;
			ApplyOverride();
			Applied = true;
		}		
		public void Undo() {
			if (!Applied)
				return;
			UndoOverride();
			Applied = false;
		}
		public bool Owerwrite(RuntimeCustomization second) {
			if (second == null || TargetName != second.TargetName)
				return false;
			if (Timestamp < second.Timestamp)
				return false;
			return OwerwriteOverride(second);
		}
		protected object StringToObject(string value, Type targetType) {
			return new DevExpress.Utils.Serializing.Helpers.ObjectConverterImplementation()
				.StringToObject(value, targetType);
		}
		protected abstract void ApplyOverride();
		protected abstract void UndoOverride();
		protected abstract bool OwerwriteOverride(RuntimeCustomization second);
	}
	public class RuntimeUndoCustomization : RuntimeCustomization {
		public RuntimeUndoCustomization():this(null) { }
		public RuntimeUndoCustomization(string name) : base(null) { TargetName = name; }
		protected override void ApplyOverride() {
			if (Host == null)
				return;
			bool removeAll = String.IsNullOrEmpty(TargetName);
			using (Host.RuntimeCustomizations.Lock()) {
				for (int i = Host.RuntimeCustomizations.Count - 1; i >= 0; i--) {
					var current = Host.RuntimeCustomizations[i];
					if (removeAll || current.AffectedTargets.Contains(TargetName))
						current.Undo();
				}
			}
		}
		protected override void UndoOverride() {
			if (Host == null)
				return;
			bool removeAll = String.IsNullOrEmpty(TargetName);
			using (Host.RuntimeCustomizations.Lock()) {
				for (int i = 0; i < Host.RuntimeCustomizations.Count; i++) {
					var current = Host.RuntimeCustomizations[i];
					if (removeAll || current.AffectedTargets.Contains(TargetName))
						current.Apply();
				}
			}
		}
		protected override bool OwerwriteOverride(RuntimeCustomization second) {
			return false;
		}		
	}
	public class RuntimePropertyCustomization : RuntimeCustomization {
		[XtraSerializableProperty]
		public string PropertyName { get; set; }
		[XtraSerializableProperty]
		public object NewValue { get; set; }
		[XtraSerializableProperty]
		public object OldValue { get; set; }
		[XtraSerializableProperty]
		public bool ActOnHost { get; set; }
		public RuntimePropertyCustomization() : this(null) { }
		public RuntimePropertyCustomization(DependencyObject forcedTarget)
			: base(forcedTarget) {
			OldValue = DependencyProperty.UnsetValue;
		}
		protected override DependencyObject FindTarget() {
			return (ActOnHost ? (DependencyObject)Host : null) ?? base.FindTarget();
		}
		protected override void ApplyOverride() {
			SetCurrentValue(NewValue);
		}		
		protected override void UndoOverride() {
			SetCurrentValue(OldValue);
			OldValue = DependencyProperty.UnsetValue;
		}
		protected override sealed bool OwerwriteOverride(RuntimeCustomization second) {
			var rp = second as RuntimePropertyCustomization;
			if (rp == null)
				return false;
			if (PropertyName != rp.PropertyName || ActOnHost != rp.ActOnHost)
				return false;
			return OwerwriteImpl(rp);
		}
		protected virtual bool OwerwriteImpl(RuntimePropertyCustomization rp) {	
				OldValue = rp.OldValue;
			return true;
		}
		void SetCurrentValue(object value) {
			var target = FindTarget();
			if (target == null)
				return;
			SetValue(target, PropertyName, value);
		}
		void SetValue(object element, string propertyName, object value) {
			element = GetValue(element, propertyName, true);
			propertyName = propertyName.Split('.').LastOrDefault();
			var property = (element as DependencyObject).With(x => FromName(x, propertyName));
			if (property != null)
				SetCurrentValueForDependencyProperty(value, element as DependencyObject, property);
			else
				SetValueForSimpleProperty(element, value);
		}
		object GetValue(object element, string propertyName, bool lastButOne) {
			if (string.IsNullOrEmpty(propertyName))
				return null;
			var properties = propertyName.Split('.').ToList();
			for (int i = 0; i < properties.Count - (lastButOne ? 1 : 0); i++) {
				var prop = properties[i];
				var dObj = element as DependencyObject;
				var dProp = dObj.With(x => FromName(x, prop));
				if (dProp != null) {
					element = dObj.GetValue(dProp);
					continue;
				}
				if (element == null)
					break;
				element = element.GetType().GetProperty(prop, BindingFlags.Instance | BindingFlags.Public).GetValue(element, null);
			}
			return element;
		}	 
		void SetValueForSimpleProperty(object target, object value) {
			var pInfo = target.GetType().GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.Public);
			OldValue = pInfo.GetValue(target, null);
			pInfo.SetValue(target, value, null);
		}
		void SetCurrentValueForDependencyProperty(object value, DependencyObject target, DependencyProperty property) {
			if (value is string && !property.PropertyType.IsAssignableFrom(typeof(string)))
				value = StringToObject((string)value, property.PropertyType);
			OldValue = target.GetValue(property);
			target.SetCurrentValue(property, value);
		}
	}
	public abstract class RuntimeCollectionCustomization : RuntimeCustomization {
		[XtraSerializableProperty]
		public string ContainerName { get; set; }
		DependencyObject forcedContainer;
		protected virtual DependencyObject FindContainer() {
			return forcedContainer ?? Host.FindTarget(ContainerName);
		}
		public RuntimeCollectionCustomization()
			: this(null, null) {
		}
		public RuntimeCollectionCustomization(DependencyObject forcedTarget, DependencyObject forcedContainer)
			: base(forcedTarget) {
			this.forcedContainer = forcedContainer;
			this.ContainerName = forcedContainer.With(BarManagerCustomizationHelper.GetSerializationName);
		}
		protected override bool OwerwriteOverride(RuntimeCustomization second) { return false; }
	}
	public class RuntimeCopyLinkCustomization : RuntimeCollectionCustomization {
		[XtraSerializableProperty]
		public string ResultName { get; set; }
		[XtraSerializableProperty]
		public string InsertAfter { get; set; }
		[XtraSerializableProperty]
		public string InsertBefore { get; set; }
		[XtraSerializableProperty]
		public int InsertIndex { get; set; }
		public RuntimeCopyLinkCustomization()
			: this(null) {
		}
		public RuntimeCopyLinkCustomization(IBarItem forcedTarget)
			: this (forcedTarget, null, null, false) {				
		}
		public RuntimeCopyLinkCustomization(IBarItem forcedTarget, ILinksHolder forcedHolder, BarItemLink insert, bool after)
			: base(forcedTarget as DependencyObject, forcedHolder as DependencyObject) {			
			InsertIndex = -1;
			if (insert == null)
				return;
			if (after) {
				InsertAfter = BarManagerCustomizationHelper.GetSerializationName(insert);
			} else
				InsertBefore = BarManagerCustomizationHelper.GetSerializationName(insert);
		}
		BarItemLink result;
		public BarItemLink CreateLinkClone() {
			if (result != null)
				return result;
			var target = FindTarget();
			var item = target as BarItem;
			result = null;
			if (item != null) {
				result = item.CreateLink();
				result.Name = CloneNameHelper.GetCloneName(item, result);
			}
			var sourceLink = target as BarItemLink;
			if (sourceLink != null) {
				result = ((ICloneable)sourceLink).Clone() as BarItemLink;
			}
			if (result == null)
				return null;
			if (string.IsNullOrEmpty(ResultName))
				ResultName = BarManagerCustomizationHelper.GetSerializationName(result);
			else {
				result.Name = ResultName;
				CloneNameHelper.Register(result.Name);
			}
			result.CreatedByCustomizationDialog = true;
			return result;
		}
		protected override void ApplyOverride() {
			CreateLinkClone();
			if (result == null)
				return;
			BarItemLink placementTarget = null;
			ILinksHolder holder = FindContainer() as ILinksHolder;
			bool insertAfter = false;
			do {
				if (!String.IsNullOrEmpty(InsertAfter)) {
					placementTarget = Host.FindTarget(InsertAfter) as BarItemLink;
					if (placementTarget != null) {
						insertAfter = true;						
						break;
					}
				}
				if (!String.IsNullOrEmpty(InsertBefore)) {
					placementTarget = Host.FindTarget(InsertBefore) as BarItemLink;
					if (placementTarget != null) {
						insertAfter = false;
						break;
					}
				}
				if (!String.IsNullOrEmpty(ContainerName)) {
					if (holder == null)
						return;
					if (!holder.Links.IsValidIndex(InsertIndex))
						break;
					placementTarget = holder.Links[InsertIndex] as BarItemLink;
					if (placementTarget != null) {
						insertAfter = false;
						break;
					}
				}
			} while (false);
			if (placementTarget != null) {
				holder = placementTarget.Links.Holder;
				ContainerName = BarManagerCustomizationHelper.GetSerializationName(holder as DependencyObject);
				InsertIndex = holder.Links.IndexOf(placementTarget);				
			}
			if (insertAfter)
				InsertIndex++;
			if (InsertIndex == -1) {
				holder.Items.Add(result);
			} else
				holder.Items.Insert(InsertIndex, result);
		}
		protected override void UndoOverride() {
			var result = Host.FindTarget(ResultName) as BarItemLink;
			if (result == null || result.Links == null)
				return;
			result.Links.Remove(result);
		}
	}
	public class CloneNameHelper {
		struct CloneNameData {
			readonly WeakReference sourceWR;
			readonly WeakReference targetWR;
			readonly int hashCode;
			public CloneNameData(object source, object target) {
				sourceWR = new WeakReference(source);
				targetWR = new WeakReference(target);
				hashCode = 0;
				hashCode = GetHashCodeImpl();
			}
			int GetHashCodeImpl() {
				unchecked {
					var sO = sourceWR.Target;
					var tO = targetWR.Target;
					int hash = 0;
					hash = (hash * 397) ^ (sO == null ? 0 : sO.GetHashCode());
					hash = (hash * 397) ^ (tO == null ? 0 : tO.GetHashCode());
					return hash;
				}
			}
			public override int GetHashCode() {
				return hashCode;
			}
			public override bool Equals(object obj) {
				if (!(obj is CloneNameData))
					return false;
				var cnd = (CloneNameData)obj;
				return sourceWR.Target == cnd.sourceWR.Target
					&& targetWR.Target == cnd.targetWR.Target;
			}
		}
		static Dictionary<string, int> clonedNames = new Dictionary<string, int>();
		static Dictionary<CloneNameData, string> generatedNames = new Dictionary<CloneNameData, string>();
		public static string GetCloneName(IFrameworkInputElement source, IFrameworkInputElement target) {
			if (source == null)
				return string.Empty;
			var key = new CloneNameData(source, target);
			string result = string.Empty;
			if (generatedNames.TryGetValue(key, out result))
				return result;
			var name = source.Name;
			if (!String.IsNullOrEmpty(name)) {
				int index = 0;
				name = GetNameSubstring(name, ref index);
				clonedNames.TryGetValue(name, out index);
				clonedNames[name] = ++index;
				result = String.Format("{0}CL{1:D3}", name, index);
				generatedNames[key] = result;
				return result;
			}
			return string.Empty;
		}
		static string GetNameSubstring(string name, ref int index) {
			if (name.Length > 5) {
				var skipped = name.Substring(name.Length - 5, 5);
				if (skipped.StartsWith("CL")) {
					var indexSubs = skipped.Substring(2, 3).ToString();
					if (int.TryParse(indexSubs, out index)) {
						name = name.Substring(0, name.Length - 5);
					}
				}
			}
			return name;
		}
		public static void Register(string nameString) {
			int index = 0;
			int currentIndex = 0;
			var name = GetNameSubstring(nameString, ref index);
			clonedNames.TryGetValue(name, out currentIndex);
			if (index > currentIndex)
				clonedNames[name] = index;
		}
	}
	public class RuntimeRemoveLinkCustomization : RuntimeCollectionCustomization {
		[XtraSerializableProperty]
		public string HolderName { get; set; }
		[XtraSerializableProperty]
		public int Index { get; set; }
		[XtraSerializableProperty]
		public string BasedOn { get; set; }
		public RuntimeRemoveLinkCustomization()
			: this(null) {
		}
		public RuntimeRemoveLinkCustomization(BarItemLinkBase forcedTarget)
			: base(forcedTarget, forcedTarget.With(x => x.Links.Holder) as DependencyObject) {
		}
		protected BarItemLink FindLink() {
			return FindTarget() as BarItemLink;
		}
		protected override void ApplyOverride() {
			var link = FindLink();
			var holder = link.Links.Holder;
			HolderName = (holder as IFrameworkInputElement).With(x => x.Name);
			Index = holder.Links.IndexOf(link);
			BasedOn = link.BasedOn;
			holder.Links.Remove(link);
		}
		protected override void UndoOverride() {
			var holder = Host.FindTarget(HolderName) as ILinksHolder;
			if (holder == null)
				return;
			var link = FindLink();
			if (link == null) {
				var sourceLink = Host.FindTarget(BasedOn);
				if (sourceLink == null)
					return;
				link = ((ICloneable)sourceLink).Clone() as BarItemLink;
			}
			if (link == null)
				return;
			link.Name = TargetName;
			CloneNameHelper.Register(TargetName);
			holder.Links.Insert(Index, link);
		}
	}
	public class RuntimeCreateNewBarCustomization : RuntimeCollectionCustomization {
		const string namePrefix = "guid167C22D3B3E749C9A0435237A138E3A5";
		static int index;
		[XtraSerializableProperty]
		public string Caption { get; set; }
		public RuntimeCreateNewBarCustomization() : this(null) { }
		public RuntimeCreateNewBarCustomization(string caption) {
			Caption = caption;
		}
		protected override void ApplyOverride() {
			var manager = Host as BarManager;
			if (manager == null)
				return;
			var bar = new Bar();
			bar.Caption = Caption;
			bar.AllowRename = true;
			bar.CreatedByCustomizationDialog = true;
			if (string.IsNullOrEmpty(TargetName))
				TargetName = namePrefix + index++;
			bar.Name = TargetName;
			manager.Bars.Add(bar);
		}
		protected override void UndoOverride() {
			var manager = Host as BarManager;
			if (manager == null)
				return;
			if (string.IsNullOrEmpty(TargetName))
				return;
			var bar = FindTarget() as Bar;
			if (bar == null) return;
			manager.Bars.Remove(bar);
		}
	}
}
