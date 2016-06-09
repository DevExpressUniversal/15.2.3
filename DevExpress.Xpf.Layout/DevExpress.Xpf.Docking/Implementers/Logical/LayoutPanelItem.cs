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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Data.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Docking.Base;
#if SILVERLIGHT
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Docking {
	public class LayoutPanel : ContentItem, IGeneratorHost, IClosable {
		#region static
		public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty;
		public static readonly DependencyProperty VerticalScrollBarVisibilityProperty;
		public static readonly DependencyProperty ShowBorderProperty;
		public static readonly DependencyProperty LayoutProperty;
		static readonly DependencyPropertyKey LayoutPropertyKey;
		public static readonly DependencyProperty ControlProperty;
		static readonly DependencyPropertyKey ControlPropertyKey;
		public static readonly DependencyProperty ContentPresenterProperty;
		static readonly DependencyPropertyKey ContentPresenterPropertyKey;
		public static readonly DependencyProperty HasBorderProperty;
		static readonly DependencyPropertyKey HasBorderPropertyKey;
		public static readonly DependencyProperty AllowDockToDocumentGroupProperty;
		public static readonly DependencyProperty ActualTabBackgroundColorProperty;
		static readonly DependencyPropertyKey ActualTabBackgroundColorPropertyKey;
		public static readonly DependencyProperty TabBackgroundColorProperty;
		public static readonly DependencyProperty UriProperty;
		internal static readonly DependencyPropertyKey UriPropertyKey;
		public static readonly DependencyProperty IsMaximizedProperty;
		protected internal static readonly DependencyPropertyKey IsMaximizedPropertyKey;
		public static readonly DependencyProperty AutoHiddenProperty;
		public static readonly DependencyProperty AutoHideExpandStateProperty;
		public static readonly DependencyProperty DockItemStateProperty;
		static readonly DependencyPropertyKey DockItemStatePropertyKey;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty TabPinLocationProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty PinnedProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty ShowPinButtonInTabProperty;
		public static readonly DependencyProperty ShowHideButtonProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsHideButtonVisibleProperty;
		static readonly DependencyPropertyKey IsHideButtonVisiblePropertyKey;
		public static readonly DependencyProperty ShowExpandButtonProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsExpandButtonVisibleProperty;
		static readonly DependencyPropertyKey IsExpandButtonVisiblePropertyKey;
		public static readonly DependencyProperty ShowCollapseButtonProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsCollapseButtonVisibleProperty;
		static readonly DependencyPropertyKey IsCollapseButtonVisiblePropertyKey;
#if !SILVERLIGHT
		public static readonly DependencyProperty ShowInDocumentSelectorProperty;
#endif
		static LayoutPanel() {
			var dProp = new DependencyPropertyRegistrator<LayoutPanel>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.OverrideMetadata(AllowSelectionProperty, false);
			dProp.Register("HorizontalScrollBarVisibility", ref HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
			dProp.Register("VerticalScrollBarVisibility", ref VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
			dProp.RegisterReadonly("Layout", ref LayoutPropertyKey, ref LayoutProperty, (LayoutGroup)null,
				(dObj, e) => ((LayoutPanel)dObj).OnLayoutChanged((LayoutGroup)e.NewValue, (LayoutGroup)e.OldValue));
			dProp.RegisterReadonly("Control", ref ControlPropertyKey, ref ControlProperty, (UIElement)null,
				(dObj, e) => ((LayoutPanel)dObj).OnControlChanged((UIElement)e.NewValue, (UIElement)e.OldValue));
			dProp.RegisterReadonly("ContentPresenter", ref ContentPresenterPropertyKey, ref ContentPresenterProperty, (UIElement)null);
			dProp.Register("ShowBorder", ref ShowBorderProperty, true,
				(dObj, e) => ((LayoutPanel)dObj).OnShowBorderChanged());
			dProp.RegisterReadonly("HasBorder", ref HasBorderPropertyKey, ref HasBorderProperty, true, null,
				(dObj, value) => ((LayoutPanel)dObj).CoerceHasBorder((bool)value));
			dProp.Register("AllowDockToDocumentGroup", ref AllowDockToDocumentGroupProperty, true);
			dProp.Register("TabBackgroundColor", ref TabBackgroundColorProperty, Colors.Transparent,
				(dObj, e) => ((LayoutPanel)dObj).OnTabBackgroundColorChanged((Color)e.OldValue, (Color)e.NewValue));
			dProp.RegisterReadonly("ActualTabBackgroundColor", ref ActualTabBackgroundColorPropertyKey, ref ActualTabBackgroundColorProperty, Colors.Transparent, null,
				(dObj, value) => ((LayoutPanel)dObj).CoerceActualTabBackgroundColor((Color)value));
			dProp.RegisterReadonly("Uri", ref UriPropertyKey, ref UriProperty, (Uri)null);
			dProp.RegisterReadonly("IsMaximized", ref IsMaximizedPropertyKey, ref IsMaximizedProperty, false,
				(dObj, e) => ((LayoutPanel)dObj).OnIsMaximizedChanged((bool)e.NewValue));
			dProp.Register("AutoHidden", ref AutoHiddenProperty, false, 
				(dObj, e)=>((LayoutPanel)dObj).OnAutoHiddenChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.Register("AutoHideExpandState", ref AutoHideExpandStateProperty, AutoHideExpandState.Hidden,
				(dObj, e) => ((LayoutPanel)dObj).OnAutoHideExpandStateChanged((AutoHideExpandState)e.OldValue, (AutoHideExpandState)e.NewValue));
			dProp.Register("ShowInDocumentSelector", ref ShowInDocumentSelectorProperty, true);
			dProp.Register("TabPinLocation", ref TabPinLocationProperty, TabHeaderPinLocation.Default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(dObj, e) => ((LayoutPanel)dObj).OnTabPinLocationChanged((TabHeaderPinLocation)e.OldValue, (TabHeaderPinLocation)e.NewValue));
			dProp.Register("Pinned", ref PinnedProperty, false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(dObj, e) => ((LayoutPanel)dObj).OnPinnedChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.RegisterReadonly("DockItemState", ref DockItemStatePropertyKey, ref DockItemStateProperty, DockItemState.Undefined,
				(dObj, e) => ((LayoutPanel)dObj).OnDockItemStateChanged((DockItemState)e.OldValue, (DockItemState)e.NewValue),
				(dObj, value) => ((LayoutPanel)dObj).CoerceDockItemState((DockItemState)value));
			dProp.Register("ShowHideButton", ref ShowHideButtonProperty, true,				
				(dObj, e) => ((LayoutPanel)dObj).OnShowHideButtonChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.Register("ShowExpandButton", ref ShowExpandButtonProperty, true,
				(dObj, e) => ((LayoutPanel)dObj).OnShowExpandButtonChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.Register("ShowCollapseButton", ref ShowCollapseButtonProperty, true,
				(dObj, e) => ((LayoutPanel)dObj).OnShowCollapseButtonChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.Register("ShowPinButtonInTab", ref ShowPinButtonInTabProperty, false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(dObj, e) => ((LayoutPanel)dObj).OnShowPinButtonInTabChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.RegisterReadonly("IsHideButtonVisible", ref IsHideButtonVisiblePropertyKey, ref IsHideButtonVisibleProperty, false, null,
				(dObj, value) => ((LayoutPanel)dObj).CoerceIsHideButtonVisible(value));
			dProp.RegisterReadonly("IsExpandButtonVisible", ref IsExpandButtonVisiblePropertyKey, ref IsExpandButtonVisibleProperty, false, null,
				(dObj, value) => ((LayoutPanel)dObj).CoerceIsExpandButtonVisible(value));
			dProp.RegisterReadonly("IsCollapseButtonVisible", ref IsCollapseButtonVisiblePropertyKey, ref IsCollapseButtonVisibleProperty, false, null,
				(dObj, value) => ((LayoutPanel)dObj).CoerceIsCollapseButtonVisible(value));
		}
		#endregion static
		public LayoutPanel() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(LayoutPanel);
			AllowSelection = false;
			AddHandler(UIElement.MouseLeftButtonDownEvent, new System.Windows.Input.MouseButtonEventHandler(OnMouseLeftButtonDown), true);
#endif
			GotFocus += new RoutedEventHandler(Control_GotFocus);
			SetBinding(TabPinLocationProperty, new System.Windows.Data.Binding() { Path = new PropertyPath(DocumentGroup.PinLocationProperty), Source = this, Mode = System.Windows.Data.BindingMode.TwoWay });
			SetBinding(PinnedProperty, new System.Windows.Data.Binding() { Path = new PropertyPath(DocumentGroup.PinnedProperty), Source = this, Mode = System.Windows.Data.BindingMode.TwoWay });
			SetBinding(ShowPinButtonInTabProperty, new System.Windows.Data.Binding() { Path = new PropertyPath(DocumentGroup.ShowPinButtonProperty), Source = this, Mode = System.Windows.Data.BindingMode.TwoWay });
		}
		protected internal override IUIElement FindUIScopeCore() {
			IUIElement scope = base.FindUIScopeCore();
			if(scope is LayoutPanel) return null;
#if SILVERLIGHT
			return (IsAutoHidden && scope is DockLayoutManager) ? null : scope;
#else
			return scope;
#endif
		}
		void OnControlChanged(UIElement control, UIElement oldControl) {
			CoerceValue(IsControlItemsHostProperty);
			if(!IsControlItemsHost)
				OnControlPropertyChanged(control, oldControl);
		}
		void OnLayoutChanged(LayoutGroup group, LayoutGroup oldGroup) {
			CoerceValue(IsControlItemsHostProperty);
			if(oldGroup != null)
				oldGroup.ClearValue(IsControlItemsHostPropertyKey);
			if(group != null) {
				group.SetValue(IsControlItemsHostPropertyKey, true);
				if(group.ItemType != LayoutItemType.Group)
					throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.WrongLayoutRoot));
			}
			OnLayoutPropertyChanged(oldGroup, group);
		}
		protected override void OnContentChanged(object content, object oldContent) {
			ClearContent(oldContent);
			CheckContent(content);
		}
		object CheckContentAsUri(object content) {
			Uri uri = content as Uri;
			if(uri != null) {
				SetValue(UriPropertyKey, uri);
				var contentFromUri = XamlLoaderHelper.LoadContentFromUri(uri);
				if(contentFromUri != null) content = contentFromUri;
			}
			return content;
		}
		protected virtual void CheckContent(object content) {
			content = CheckContentAsUri(content);
			LayoutGroup group = content as LayoutGroup;
			if(group != null) {
				group.ParentPanel = this;
				group.Accept((item) => item.CoerceValue(IsControlItemsHostProperty));
				SetValue(LayoutPropertyKey, group);
			} else {
				UIElement uiElement = content as UIElement;
				if(uiElement != null)
					SetValue(ControlPropertyKey, content);
				else
					SetValue(IsDataBoundPropertyKey, content != null);
			}
			SetContentPresenterContainer(content);
		}
		void SetContentPresenterContainer(object content) {
			RemoveLogicalChild(Presenter);
			if(!IsControlItemsHost)
				AddLogicalChild(Presenter);
			Presenter.Content = content;
		}
		protected virtual void ClearContent(object oldContent) {
			UIElement uiElement = oldContent as UIElement;
			if(uiElement != null) {
				uiElement.ClearValue(DockLayoutManager.LayoutItemProperty);
			}
			ClearValue(UriPropertyKey);
			ClearValue(LayoutPropertyKey);
			ClearValue(ControlPropertyKey);
			ClearValue(IsDataBoundPropertyKey);
			LayoutGroup group = oldContent as LayoutGroup;
			if(group != null)
				group.ParentPanel = null;
		}
		protected virtual void OnShowBorderChanged() {
			CoerceValue(HasBorderProperty);
			RaiseGeometryChanged();
		}
		protected virtual bool CoerceHasBorder(bool value) {
			return ShowBorder;
		}
		protected virtual bool CoerceHasBackground(bool value) {
			return Background != null;
		}
		void Control_GotFocus(object sender, RoutedEventArgs e) {
			OnControlGotFocus();
		}
		internal virtual void OnControlGotFocus() {
			if(!IsActive && Manager != null) {
#if SILVERLIGHT
				if(IsAutoHidden && !Parent.IsExpanded) return;
				Dispatcher.BeginInvoke(new Action(() =>
				{
					if(LayoutHelper.IsChildElement(this, FocusManager.GetFocusedElement() as DependencyObject))
						if(!IsTabPage || IsSelectedItem)
							ActivateItemCore();
				}));
#else
				if(!IsLoaded) Dispatcher.BeginInvoke(new Action(() => ActivateItemCore()));
				else ActivateItemCore();
#endif
			}
		}
		protected virtual void ActivateItemCore() {
			if(!Manager.IsDisposing)
				Manager.DockController.Activate(this, false);
		}
		protected virtual void OnControlPropertyChanged(UIElement control, UIElement oldControl) {
			OnControlRemoved(oldControl);
			OnControlAdded(control);
		}
		void OnControlAdded(UIElement control) {
			if(control != null) {
#if !SILVERLIGHT
				control.SetValue(DockLayoutManager.LayoutItemProperty, this);
#else
				DockLayoutManager.SetLayoutItemData(control, this.LayoutItemData);
#endif
				if(!isInDesignTime)
					ExpandableChild = control as Core.IExpandableChild;
				if(ExpandableChild != null) {
					ExpandableChild.IsExpandedChanged += OnExpandableChildIsExpandedChanged;
					if(!ExpandableChild.IsExpanded) SetCurrentValue(ItemWidthProperty, GridLength.Auto);
					CoerceValue(ActualMinSizeProperty);
				}
			}
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			if(ExpandableChild != null) CoerceValue(ActualMinSizeProperty);
		}
		void OnExpandableChildIsExpandedChanged(object sender, Core.ValueChangedEventArgs<bool> e) {
			if(!e.NewValue) {
				GridLength autoWidth = ExpandableChild != null ? new GridLength(ExpandableChild.CollapseWidth) : GridLength.Auto;
				SetCurrentValue(ItemWidthProperty, autoWidth);
				if(IsAutoHidden) UpdateDockSituationWidth(autoWidth);
			}
			else {
				if(!ResizeLockHelper.IsLocked || IsAutoHidden) {
					SetCurrentValue(ItemWidthProperty, GridLength.Auto);
					if(IsAutoHidden) UpdateDockSituationWidth(GridLength.Auto);
				}
				ExpandableChild.Do(x => x.ExpandedWidth = double.NaN);
			}
		}
		void UpdateDockSituationWidth(GridLength newWidth) {
			DockSituation.Do(x => x.Width = newWidth);
		}
		protected internal override void UpdateSizeToContent() {
			ParentLockHelper.DoWhenUnlocked(() => {
				(this.GetRoot() as FloatGroup).Do(x => x.UpdateSizeToContent());
			});
		}
		protected override Size CalcMinSizeValue(Size value) {
			Size baseSize = base.CalcMinSizeValue(value);
			return ExpandableChild != null ? new Size(Math.Max(baseSize.Width, ExpandableChild.CollapseWidth), baseSize.Height) : baseSize;
		}
		protected override void OnRenderSizeChangedCore(SizeChangedInfo sizeInfo) {
			base.OnRenderSizeChangedCore(sizeInfo);
			if(ExpandableChild != null && IsVisible) {
				double value = sizeInfo.NewSize.Width;
				if(!ItemWidth.IsAuto || ExpandableChild.IsExpanded) {
					bool isSizing = ResizeLockHelper.IsLocked;
					if(value > ExpandableChild.CollapseWidth) {
						if(!ExpandableChild.IsExpanded) {
							if(!IsAutoHidden || isSizing)
								ExpandableChild.IsExpanded = true;
						}
						else {
							if(isSizing) {
								ExpandableChild.ExpandedWidth = value;
							}
							if(IsAutoHidden) UpdateDockSituationWidth(new GridLength(value));
						}
					}
					else {
						if(ExpandableChild.IsExpanded) {
							if(!IsAutoHidden || isSizing)
								ExpandableChild.IsExpanded = false;
						}
					}
				}
			}
		}
		void OnControlRemoved(UIElement oldControl) {
			if(oldControl != null) {
				oldControl.ClearValue(DockLayoutManager.LayoutItemProperty);
#if !SILVERLIGHT
#else
				oldControl.ClearValue(DockLayoutManager.LayoutItemDataProperty);
#endif
			}
			if(ExpandableChild != null) {
				ExpandableChild.IsExpandedChanged -= OnExpandableChildIsExpandedChanged;
				ExpandableChild = null;
				CoerceValue(ActualMinSizeProperty);
			}
		}
		protected virtual void OnLayoutPropertyChanged(LayoutGroup oldValue, LayoutGroup newValue) {
			if(oldValue != null) {
				RemoveLogicalChild(oldValue);
			}
			if(newValue != null) {
				AddLogicalChild(newValue);
			}
		}
#if !SILVERLIGHT
		protected override System.Collections.IEnumerator LogicalChildren {
			get { return new Bars.SingleLogicalChildEnumerator(IsControlItemsHost ? Layout : (UIElement)Presenter); }
		}
#endif
		internal TabHeaderPinLocation TabPinLocation {
			get { return (TabHeaderPinLocation)GetValue(TabPinLocationProperty); }
			set { SetValue(TabPinLocationProperty, value); }
		}
		internal bool Pinned {
			get { return (bool)GetValue(PinnedProperty); }
			set { SetValue(PinnedProperty, value); }
		}
		public DockItemState DockItemState {
			get { return (DockItemState)GetValue(DockItemStateProperty); }
			private  set { SetValue(DockItemStatePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutPanelHorizontalScrollBarVisibility")]
#endif
		public ScrollBarVisibility HorizontalScrollBarVisibility {
			get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
			set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutPanelVerticalScrollBarVisibility")]
#endif
		public ScrollBarVisibility VerticalScrollBarVisibility {
			get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
			set { SetValue(VerticalScrollBarVisibilityProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutPanelLayout")]
#endif
		public LayoutGroup Layout {
			get { return (LayoutGroup)GetValue(LayoutProperty); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutPanelControl")]
#endif
		public UIElement Control {
			get { return (UIElement)GetValue(ControlProperty); }
		}
		public UIElement ContentPresenter {
			get { return (UIElement)GetValue(ContentPresenterProperty); }
		}
		internal Core.IExpandableChild ExpandableChild { get; private set; }
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutPanelUri")]
#endif
		public Uri Uri {
			get { return (Uri)GetValue(UriProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutPanelShowBorder"),
#endif
		XtraSerializableProperty, Category("Content")]
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutPanelHasBorder")]
#endif
		public bool HasBorder {
			get { return (bool)GetValue(HasBorderProperty); }
		}
		[XtraSerializableProperty(), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Rect SerializableFloatingBounds {
			get { return new Rect(FloatLocationBeforeClose, FloatSizeBeforeClose); }
			set {
				if(!value.IsEmpty) {
					FloatLocationBeforeClose = new Point(value.X, value.Y);
					FloatSizeBeforeClose = new Size(value.Width, value.Height);
				}
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool SerializableIsMaximized {
			get { return IsMaximized; }
			set { }
		}
		[XtraSerializableProperty]
		public bool AllowDockToDocumentGroup {
			get { return (bool)GetValue(AllowDockToDocumentGroupProperty); }
			set { SetValue(AllowDockToDocumentGroupProperty, value); }
		}
		protected internal Size LayoutSizeBeforeHide { get; set; }
		protected internal Point FloatLocationBeforeClose { get; set; }
		protected internal Size FloatSizeBeforeClose { get; set; }
		protected internal Point FloatOffsetBeforeClose { get; set; }
		protected internal DateTime LastActivationDateTime { get; set; }
		[XtraSerializableProperty(), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Point SerializableFloatingOffset {
			get { return FloatOffsetBeforeClose; }
			set { FloatOffsetBeforeClose = value; }
		}
		protected override LayoutItemType GetLayoutItemTypeCore() {
			return LayoutItemType.Panel;
		}
		protected override BaseLayoutItem[] GetNodesCore() {
			LayoutGroup group = Layout;
			return (group != null) ? new BaseLayoutItem[] { group } : EmptyNodes;
		}
		protected override bool CoerceIsTabPage(bool value) {
			return (Parent != null) && Parent is TabbedGroup;
		}
		protected override bool CoerceIsControlItemsHost(bool value) {
			return Layout != null;
		}
		protected override string CoerceCaptionFormat(string captionFormat) {
			if(!string.IsNullOrEmpty(captionFormat)) return captionFormat;
			return DockLayoutManagerParameters.LayoutPanelCaptionFormat;
		}
		protected override void OnVisibilityChangedOverride(Visibility visibility) {
			base.OnVisibilityChangedOverride(visibility);
			if(IsControlItemsHost) Layout.CoerceValue(BaseLayoutItem.IsVisibleProperty);
		}
		protected override void OnIsVisibleChanged(bool isVisible) {
			base.OnIsVisibleChanged(isVisible);
			if(IsControlItemsHost) Layout.CoerceValue(BaseLayoutItem.IsVisibleProperty);
			if(!isVisible) {
				if(IsAutoHidden && Manager != null)
					Manager.HideView(Parent);
			}
			else CheckExpandState();
		}
		protected override bool CoerceIsCaptionVisible(bool visible) {
			return Parent is DocumentGroup ? false : base.CoerceIsCaptionVisible(visible);
		}
		LayoutPanelContentPresenter presenter;
		LayoutPanelContentPresenter Presenter {
			get {
				if(presenter == null) {
					presenter = new LayoutPanelContentPresenter();
					BindingHelper.SetBinding(presenter, System.Windows.Controls.ContentPresenter.ContentTemplateProperty, this, ContentTemplateProperty);
					BindingHelper.SetBinding(presenter, System.Windows.Controls.ContentPresenter.ContentTemplateSelectorProperty, this, ContentTemplateSelectorProperty);
					BindingHelper.SetBinding(presenter, System.Windows.Controls.ContentPresenter.DataContextProperty, this, DataContextProperty);
					SetValue(ContentPresenterPropertyKey, presenter);
				}
				return presenter;
			}
		}
		internal bool IsDockedAsDocument { get { return Parent is DocumentGroup; } }
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutPanelIsPinButtonVisible")]
#endif
		public bool IsPinButtonVisible {
			get { return (bool)GetValue(IsPinButtonVisibleProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutPanelShowPinButton"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public bool ShowPinButton {
			get { return (bool)GetValue(ShowPinButtonProperty); }
			set { SetValue(ShowPinButtonProperty, value); }
		}
		[XtraSerializableProperty, Category("TabHeader")]
		public Color TabBackgroundColor {
			get { return (Color)GetValue(TabBackgroundColorProperty); }
			set { SetValue(TabBackgroundColorProperty, value); }
		}
		public Color ActualTabBackgroundColor {
			get { return (Color)GetValue(ActualTabBackgroundColorProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutPanelShowMaximizeButton"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public bool ShowMaximizeButton {
			get { return (bool)GetValue(ShowMaximizeButtonProperty); }
			set { SetValue(ShowMaximizeButtonProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutPanelShowRestoreButton"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public bool ShowRestoreButton {
			get { return (bool)GetValue(ShowRestoreButtonProperty); }
			set { SetValue(ShowRestoreButtonProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutPanelIsMaximized")]
#endif
		public bool IsMaximized {
			get { return (bool)GetValue(IsMaximizedProperty); }
			private set { SetValue(IsMaximizedPropertyKey, value); }
		}
#if !SILVERLIGHT
		public bool ShowInDocumentSelector {
			get { return (bool)GetValue(ShowInDocumentSelectorProperty); }
			set { SetValue(ShowInDocumentSelectorProperty, value); }
		}
#endif
		public bool AutoHidden {
			get { return (bool)GetValue(AutoHiddenProperty); }
			set { SetValue(AutoHiddenProperty, value); }
		}
		public AutoHideExpandState AutoHideExpandState {
			get { return (AutoHideExpandState)GetValue(AutoHideExpandStateProperty); }
			set { SetValue(AutoHideExpandStateProperty, value); }
		}
		public bool ShowHideButton {
			get { return (bool)GetValue(ShowHideButtonProperty); }
			set { SetValue(ShowHideButtonProperty, value); }
		}
		public bool ShowExpandButton {
			get { return (bool)GetValue(ShowExpandButtonProperty); }
			set { SetValue(ShowExpandButtonProperty, value); }
		}
		public bool ShowCollapseButton {
			get { return (bool)GetValue(ShowCollapseButtonProperty); }
			set { SetValue(ShowCollapseButtonProperty, value); }
		}
		internal bool ShowPinButtonInTab {
			get { return (bool)GetValue(ShowPinButtonInTabProperty); }
			set { SetValue(ShowPinButtonInTabProperty, value); }
		}
		protected virtual void OnShowHideButtonChanged(bool oldValue, bool newValue) {
			CoerceValue(IsHideButtonVisibleProperty);
		}
		protected virtual void OnShowExpandButtonChanged(bool oldValue, bool newValue) {
			CoerceValue(IsExpandButtonVisibleProperty);
		}
		protected virtual void OnShowCollapseButtonChanged(bool oldValue, bool newValue) {
			CoerceValue(IsCollapseButtonVisibleProperty);
		}
		protected virtual void OnShowPinButtonInTabChanged(bool oldValue, bool newValue) {
			CoerceValue(IsPinButtonVisibleProperty);
		}
		protected virtual object CoerceIsHideButtonVisible(object baseValue) {
			return IsInlineMode && ShowHideButton && IsAutoHidden;
		}
		protected virtual bool CoerceIsExpandButtonVisible(object baseValue) {
			return IsInlineMode && ShowExpandButton && IsAutoHidden && AutoHideExpandState != Base.AutoHideExpandState.Expanded;
		}
		protected virtual bool CoerceIsCollapseButtonVisible(object baseValue) {
			return IsInlineMode && ShowCollapseButton && IsAutoHidden && AutoHideExpandState == Base.AutoHideExpandState.Expanded;
		}
		internal Core.Locker ExpandAnimationLocker = new Core.Locker();
		protected virtual void OnAutoHideExpandStateChanged(AutoHideExpandState oldValue, AutoHideExpandState newValue) {
			if(ExpandAnimationLocker) return;
			ExpandStateLocker.Lock();
			CheckExpandState();
		}
		protected virtual void OnAutoHiddenChanged(bool oldValue, bool newValue) {
			CheckAutoHiddenState();
			InvokeCoerceDataContext();
		}
		private void CheckAutoHiddenState() {
			ParentLockHelper.DoWhenUnlocked(CheckAutoHiddenStateCore);
		}
		private void CheckExpandState() {
			if(!IsVisible || Manager == null) return;
			if(ParentLockHelper.IsLocked) ParentLockHelper.AddUnlockAction(CheckExpandStateCore);
			else {
				if(Parent != null)
					CheckExpandStateCore();
			}
		}
		void CheckAutoHiddenStateCore() {
			DockLayoutManager manager = this.FindDockLayoutManager();
			if(manager == null || Parent == null || AutoHidden == IsAutoHidden || isInDesignTime) return;
			manager.CheckAutoHiddenState(this);
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			CheckExpandState();
		}
		LockHelper expandStateLocker;
		internal LockHelper ExpandStateLocker {
			get {
				if(expandStateLocker == null) expandStateLocker = new LockHelper();
				return expandStateLocker;
			}
		}
		void CheckExpandStateCore() {
			if(ExpandStateLocker.IsLocked) {
				DockLayoutManager manager = this.FindDockLayoutManager();
				bool res = manager.Return(x => x.CheckAutoHideExpandState(this), () => false);
				if(res) ExpandStateLocker.Unlock();
			}
			if(AutoHideExpandState != Base.AutoHideExpandState.Hidden) UpdateAutoHideButtons();
			else Parent.With(x => x.IsAnimatedLockHelper).Do(x => x.DoWhenUnlocked(UpdateAutoHideButtons));
		}
		void UpdateAutoHideButtons() {
			CoerceValue(IsHideButtonVisibleProperty);
			CoerceValue(IsExpandButtonVisibleProperty);
			CoerceValue(IsCollapseButtonVisibleProperty);
		}
		protected override void OnAllowMaximizeChanged(bool value) {
			base.OnAllowMaximizeChanged(value);
			if(Parent is FloatGroup) Parent.CoerceValue(FloatGroup.CanMaximizeProperty);
		}
		protected override bool CoerceIsPinButtonVisible(bool visible) {
			if(IsTabDocument) return ShowPinButtonInTab;
			return ShowPinButton && (IsAutoHidden ? AllowDock && Parent.AllowDock : AllowHide) && !IsFloating;
		}
		protected override bool CoerceIsMaximizeButtonVisible(bool visible) {
			return IsMaximizable && AllowMaximize && ShowMaximizeButton && !IsMaximized;
		}
		protected override bool CoerceIsRestoreButtonVisible(bool visible) {
			return IsMaximizable && ShowRestoreButton && IsMaximized;
		}
		internal override void SetIsMaximized(bool isMaximized) {
			SetValue(IsMaximizedPropertyKey, isMaximized);
		}
		protected internal override bool IsMaximizable { get { return IsFloatingRootItem || (Parent as TabbedGroup).Return(x => x.IsFloatingRootItem, () => false); } }
		protected internal override void UpdateButtons() {
			CoerceValue(IsMaximizeButtonVisibleProperty);
			CoerceValue(IsPinButtonVisibleProperty);
			CoerceValue(IsRestoreButtonVisibleProperty);
		}
		protected virtual void OnIsMaximizedChanged(bool maximized) {
			UpdateButtons();
			RaiseVisualChanged();
		}
		protected override void OnDockLayoutManagerChanged() {
			base.OnDockLayoutManagerChanged();
			CheckAutoHiddenState();
			CheckExpandState();
		}
		protected virtual void OnTabPinLocationChanged(TabHeaderPinLocation oldValue, TabHeaderPinLocation newValue) {
			if(IsPinnedTab)
				NotifyParentPinStatusChanged();
		}
		protected virtual void OnPinnedChanged(bool oldValue, bool newValue) {
			if(IsTabDocument)
				NotifyParentPinStatusChanged();
		}
		protected internal override bool ToggleTabPinStatus() {
			Pinned = !Pinned;
			return true;
		}
		internal void NotifyViewPinStatusChanged() {
			RaiseVisualChanged();
			InvalidateTabHeader();
			if(Manager != null) Manager.InvalidateView(Parent);
		}
		void NotifyParentPinStatusChanged() {
			ParentLockHelper.DoWhenUnlocked(() => {
				if(Parent != null)
					Parent.OnItemPinStatusChanged(this);
			});
			NotifyViewPinStatusChanged();
		}
		void CheckPinStatusOnParentChanged(LayoutGroup oldParent) {
			bool wasPinned = oldParent != null && oldParent.ContainsPinnedItem(this);
			if(wasPinned) {
				Dispatcher.BeginInvoke(new Action(() => {
					Pinned = false;
				}));
			}
			else {
				if(Pinned) {
					if(IsTabDocument) NotifyParentPinStatusChanged();
					else Pinned = false;
				}
			}
		}
		protected virtual void OnDockItemStateChanged(DockItemState oldState, DockItemState newState) { }
		protected virtual DockItemState CoerceDockItemState(DockItemState dockItemState) {
			DockItemState state = Parent != null ? DockItemState.Docked : DockItemState.Undefined;
			if(IsFloating) state = DockItemState.Floating;
			if(IsAutoHidden) state = DockItemState.AutoHidden;
			if(IsClosed) state = DockItemState.Closed;
			return state;
		}
		protected override void InvokeCoerceDockItemStateCore() {
			CoerceValue(DockItemStateProperty);
		}
		AutoHideDisplayModePropertyChangedWeakEventHandler<LayoutPanel> autoHideDisplayModePropertyChangedHandler;
		AutoHideDisplayModePropertyChangedWeakEventHandler<LayoutPanel> AutoHideDisplayModePropertyChangedHandler {
			get {
				if(autoHideDisplayModePropertyChangedHandler == null) {
					autoHideDisplayModePropertyChangedHandler = new AutoHideDisplayModePropertyChangedWeakEventHandler<LayoutPanel>(this,
						(owner, o, e) => { owner.OnManagerAutoHideDisplayModeChanged(o, e); });
				}
				return autoHideDisplayModePropertyChangedHandler;
			}
		}
		AutoHideMode autoHideDisplayMode;
		bool IsInlineMode { get { return autoHideDisplayMode == AutoHideMode.Inline; } }
		protected internal override bool IsPinnedTab { get { return Pinned && IsTabDocument; } }
		protected override void OnDockLayoutManagerChanged(DockLayoutManager oldValue, DockLayoutManager newValue) {
			base.OnDockLayoutManagerChanged(oldValue, newValue);
			UnsubscribeAutoHideDisplayModeChanged(oldValue);
			SubscribeAutoHideDisplayModeChanged(newValue);
			autoHideDisplayMode = newValue.Return(x => x.AutoHideMode, () => AutoHideMode.Default);
		}
		protected virtual void UnsubscribeAutoHideDisplayModeChanged(DockLayoutManager manager) {
			manager.Do(x => x.AutoHideDisplayModeChanged -= AutoHideDisplayModePropertyChangedHandler.Handler);
		}
		protected virtual void SubscribeAutoHideDisplayModeChanged(DockLayoutManager manager) {
			manager.Do(x => x.AutoHideDisplayModeChanged += AutoHideDisplayModePropertyChangedHandler.Handler);
		}
		void OnManagerAutoHideDisplayModeChanged(object sender, PropertyChangedEventArgs e) {
			autoHideDisplayMode = Manager.Return(x => x.AutoHideMode, () => AutoHideMode.Default);
			CoerceValue(IsHideButtonVisibleProperty);
			CoerceValue(IsExpandButtonVisibleProperty);
			CoerceValue(IsCollapseButtonVisibleProperty);
		}
		protected internal override void OnParentItemsChanged() {
			base.OnParentItemsChanged();
			InvokeCoerceDockItemState();
			if(IsFloating)
				UpdateButtons();
		}
		protected override void OnParentChanged(LayoutGroup oldParent, LayoutGroup newParent) {
			base.OnParentChanged(oldParent, newParent);
			if(!isDeserializing)
				CheckPinStatusOnParentChanged(oldParent);
			InvokeCoerceDockItemState();
		}
		protected override void OnParentChanged() {
			base.OnParentChanged();
			CoerceValue(IsCaptionVisibleProperty);
			if(Parent != null) {
				if(this.IsInTree()) UpdateButtons();
				else Dispatcher.BeginInvoke(new Action(() => { UpdateButtons(); }));
				if(Parent is FloatGroup) Parent.CoerceValue(FloatGroup.CanMaximizeProperty);
			}
			LayoutSizeBeforeHide = Parent is AutoHideGroup ? LayoutSize : Size.Empty;
			CheckAutoHiddenState();
			CheckExpandState();
		}
		protected override void OnShowCloseButtonChanged(bool show) {
			base.OnShowCloseButtonChanged(show);
			if(Parent != null)
				Parent.CoerceValue(IsCloseButtonVisibleProperty);
		}
		protected override void OnIsCloseButtonVisibleChanged(bool visible) {
			base.OnIsCloseButtonVisibleChanged(visible);
			if(Parent != null)
				Parent.CoerceValue(IsCloseButtonVisibleProperty);
		}
		protected override void OnIsMaximizeButtonVisibleChanged(bool visible) {
			base.OnIsMaximizeButtonVisibleChanged(visible);
			if(Parent != null)
				Parent.CoerceValue(IsMaximizeButtonVisibleProperty);
		}
		protected override void OnIsRestoreButtonVisibleChanged(bool visible) {
			base.OnIsRestoreButtonVisibleChanged(visible);
			if(Parent != null)
				Parent.CoerceValue(IsRestoreButtonVisibleProperty);
		}
		protected override void OnActualCaptionChanged(string value) {
			base.OnActualCaptionChanged(value);
			if(Parent is FloatGroup) CoerceParentProperty(ActualCaptionProperty);
		}
		protected override void OnAllowSizingChanged(bool value) {
			base.OnAllowSizingChanged(value);
			if(Parent is FloatGroup) CoerceParentProperty(AllowSizingProperty);
		}
		protected override void OnShowCaptionChanged(bool value) {
			base.OnShowCaptionChanged(value);
			if(Parent is FloatGroup) CoerceParentProperty(ShowCaptionProperty);
		}
#if !SILVERLIGHT
		protected bool fClearTemplateRequested = false;
		protected override void OnUnloaded() {
			if(fClearTemplateRequested) ClearTemplateCore();
			base.OnUnloaded();
		}
#endif
#if !SILVERLIGHT
		protected internal override void ClearTemplate() {
			if(!EnvironmentHelper.IsNet45OrNewer) {
				base.ClearTemplate();
				return;
			}
			if(fClearTemplateRequested) return;
			fClearTemplateRequested = true;
			Dispatcher.BeginInvoke(new Action(() =>
			{
				if(fClearTemplateRequested) ClearTemplateCore();
			}));
			this.ClearValue(DockLayoutManager.UIScopeProperty);
		}
#endif
		protected internal override void ClearTemplateCore() {
#if !SILVERLIGHT
			fClearTemplateRequested = false;
#endif
			base.ClearTemplateCore();
			if(Layout != null) Layout.ClearTemplateCore();
		}
		protected internal override void SelectTemplate() {
#if !SILVERLIGHT
			if(fClearTemplateRequested) {
				if(PartMultiTemplateControl != null && PartMultiTemplateControl.LayoutItem == this) PartMultiTemplateControl.LayoutItem = null;
				fClearTemplateRequested = false;
			}
#endif
			base.SelectTemplate();
			if(Layout != null) Layout.SelectTemplate();
		}
		protected internal override void SelectTemplateIfNeeded() {
			if(fClearTemplateRequested || (PartMultiTemplateControl != null && PartMultiTemplateControl.LayoutItem == null))
				SelectTemplate();
		}
		protected internal override void SetAutoHidden(bool autoHidden) {
			AutoHidden = autoHidden;
		}
		protected internal override void SetSelected(DockLayoutManager manager, bool value) { }
		protected override void OnActualAppearanceObjectChanged(AppearanceObject newValue) {
			this.CoerceValue(ActualTabBackgroundColorProperty);
			base.OnActualAppearanceObjectChanged(newValue);
		}
		protected virtual void OnTabBackgroundColorChanged(Color oldValue, Color newValue) {
			CoerceValue(ActualTabBackgroundColorProperty);
		}
		protected virtual Color CoerceActualTabBackgroundColor(Color value) {
			if(TabBackgroundColor != Colors.Transparent) return TabBackgroundColor;
			if(ActualAppearanceObject != null) return ActualAppearanceObject.TabBackgroundColor;
			return value;
		}
		protected virtual void OnClosed() { }
		protected virtual void OnClosing(CancelEventArgs e) { }
		protected override void OnIsActiveChanged(bool value) {
			base.OnIsActiveChanged(value);
			if(value)
				LastActivationDateTime = DateTime.Now;
		}
		protected override void OnAllowCloseChanged(bool value) {
			bool isCloseButtonVisible = IsCloseButtonVisible;
			base.OnAllowCloseChanged(value);
			if(IsCloseButtonVisible == isCloseButtonVisible && Parent != null) Parent.CoerceValue(IsCloseButtonVisibleProperty);
		}
		protected override void OnIsSelectedItemChanged() {
			base.OnIsSelectedItemChanged();
			RaiseVisualChanged();
		}
#if SILVERLIGHT
		Action mouseDownAction = null;
		void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			if(MouseDoubleClickHelper.IsDoubleClick(e)) return;
			mouseDownAction = () => { if(!IsActive && Manager != null) ActivateItemCore(); };
			Dispatcher.BeginInvoke(new Action(() =>
			{
				var action = mouseDownAction;
				if(action != null) action();
			}));
		}
		protected override void OnIsActiveChangedCore() {
			base.OnIsActiveChangedCore();
			if(IsActive && mouseDownAction != null) mouseDownAction = null;
		}
#endif
		#region IGeneratorHost Members
		DependencyObject IGeneratorHost.GenerateContainerForItem(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
			SetValue(ContentProperty, item);
			return this;
		}
		DependencyObject IGeneratorHost.LinkContainerToItem(DependencyObject container, object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
			SetValue(ContentProperty, item);
			return this;
		}
		void IGeneratorHost.ClearContainer(DependencyObject container, object item) {
			if(object.Equals(this, container))
				ClearValue(ContentProperty);
		}
		#endregion
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
		class AutoHideDisplayModePropertyChangedWeakEventHandler<TOwner> : WeakEventHandler<TOwner, PropertyChangedEventArgs, PropertyChangedEventHandler> where TOwner : class {
			static Action<WeakEventHandler<TOwner, PropertyChangedEventArgs, PropertyChangedEventHandler>, object> action = (h, o) => ((DockLayoutManager)o).AutoHideDisplayModeChanged -= h.Handler;
			static Func<WeakEventHandler<TOwner, PropertyChangedEventArgs, PropertyChangedEventHandler>, PropertyChangedEventHandler> create = h => new PropertyChangedEventHandler(h.OnEvent);
			public AutoHideDisplayModePropertyChangedWeakEventHandler(TOwner owner, Action<TOwner, object, PropertyChangedEventArgs> onEventAction)
				: base(owner, onEventAction, action, create) {
			}
		}
		class LayoutPanelContentPresenter : ContentPresenter, Core.ILogicalOwner {
			#region static
			[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
			internal static readonly DependencyProperty ContentInternalProperty;
			static LayoutPanelContentPresenter() {
				var dProp = new DependencyPropertyRegistrator<LayoutPanelContentPresenter>();
				dProp.Register("ContentInternal", ref ContentInternalProperty, (object)null,
					(dObj, e) => ((LayoutPanelContentPresenter)dObj).OnContentChanged(e.NewValue, e.OldValue));
			}
			#endregion static
			public LayoutPanelContentPresenter() {
				Focusable = false;
				VisualElements.ControlExtension.StartListen(this, ContentInternalProperty, "Content");
			}
			protected virtual void OnContentChanged(object content, object oldContent) {
				RemoveChild(oldContent);
				AddChild(content);
			}
			System.Collections.Generic.List<object> logicalChildren = new System.Collections.Generic.List<object>();
			public void AddChild(object child) {
				if((child as DependencyObject).Return(x => LogicalTreeHelper.GetParent(x) == null, () => false) && !logicalChildren.Contains(child)) {
					logicalChildren.Add(child);
					AddLogicalChild(child);
				}
			}
			public void RemoveChild(object child) {
				logicalChildren.Remove(child);
				RemoveLogicalChild(child);
			}
			protected override System.Collections.IEnumerator LogicalChildren {
				get { return logicalChildren.GetEnumerator(); }
			}
		}
	}
}
