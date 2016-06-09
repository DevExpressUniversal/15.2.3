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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.TabControlAutomation;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.Core {
	[TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
	[TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
	[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
	[TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
	public class DXTabItem : HeaderedSelectorItemBase<DXTabControl, DXTabItem> {
		#region Properties
		public static readonly DependencyProperty BindableNameProperty = DependencyProperty.Register("BindableName", typeof(string), typeof(DXTabItem),
			new FrameworkPropertyMetadata(null, (d, e) => ((DXTabItem)d).Name = (string)e.NewValue));
		public static readonly DependencyProperty IsNewProperty = DependencyProperty.Register("IsNew", typeof(bool), typeof(DXTabItem),
			new PropertyMetadata(false, (d, e) => ((DXTabItem)d).UpdateViewProperties()));
		static readonly DependencyPropertyKey ViewInfoPropertyKey = DependencyProperty.RegisterReadOnly("ViewInfo", typeof(TabViewInfo), typeof(DXTabItem), null);
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute, EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty ViewInfoProperty = ViewInfoPropertyKey.DependencyProperty;
		public static readonly DependencyProperty NormalBackgroundTemplateProperty = DependencyProperty.Register("NormalBackgroundTemplate", typeof(DataTemplate), typeof(DXTabItem));
		public static readonly DependencyProperty HoverBackgroundTemplateProperty = DependencyProperty.Register("HoverBackgroundTemplate", typeof(DataTemplate), typeof(DXTabItem));
		public static readonly DependencyProperty SelectedBackgroundTemplateProperty = DependencyProperty.Register("SelectedBackgroundTemplate", typeof(DataTemplate), typeof(DXTabItem));
		public static readonly DependencyProperty FocusedBackgroundTemplateProperty = DependencyProperty.Register("FocusedBackgroundTemplate", typeof(DataTemplate), typeof(DXTabItem));
		public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(DXTabItem), 
			new PropertyMetadata(Colors.Transparent, (d, e) => ((DXTabItem)d).UpdateViewProperties()));
		public static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(Color), typeof(DXTabItem), 
			new PropertyMetadata(Colors.Transparent, (d, e) => ((DXTabItem)d).UpdateViewProperties()));
		public static readonly DependencyProperty ShowToolTipForNonTrimmedHeaderProperty = DependencyProperty.Register("ShowToolTipForNonTrimmedHeader", typeof(bool), typeof(DXTabItem), new PropertyMetadata(true));
		public static readonly DependencyProperty VisibleInHeaderMenuProperty = DependencyProperty.Register("VisibleInHeaderMenu", typeof(bool), typeof(DXTabItem), new PropertyMetadata(true));
		public static readonly DependencyProperty HeaderMenuContentProperty = DependencyProperty.Register("HeaderMenuContent", typeof(object), typeof(DXTabItem),
			new PropertyMetadata((d, e) => ((DXTabItem)d).UpdateHeaderMenuActualProperties()));
		public static readonly DependencyProperty HeaderMenuContentTemplateProperty = DependencyProperty.Register("HeaderMenuContentTemplate", typeof(DataTemplate), typeof(DXTabItem),
			new PropertyMetadata((d, e) => ((DXTabItem)d).UpdateHeaderMenuActualProperties()));
		public static readonly DependencyProperty HeaderMenuIconProperty = DependencyProperty.Register("HeaderMenuIcon", typeof(object), typeof(DXTabItem),
			new PropertyMetadata((d, e) => ((DXTabItem)d).UpdateHeaderMenuActualProperties()));
		public static readonly DependencyProperty HeaderMenuGlyphProperty = DependencyProperty.Register("HeaderMenuGlyph", typeof(ImageSource), typeof(DXTabItem),
			new PropertyMetadata((d, e) => ((DXTabItem)d).UpdateHeaderMenuActualProperties()));
		static readonly DependencyPropertyKey ActualHeaderMenuContentPropertyKey = DependencyProperty.RegisterReadOnly("ActualHeaderMenuContent", typeof(object), typeof(DXTabItem), null);
		static readonly DependencyPropertyKey ActualHeaderMenuContentTemplatePropertyKey = DependencyProperty.RegisterReadOnly("ActualHeaderMenuContentTemplate", typeof(DataTemplate), typeof(DXTabItem), null);
		static readonly DependencyPropertyKey ActualHeaderMenuGlyphPropertyKey = DependencyProperty.RegisterReadOnly("ActualHeaderMenuGlyph", typeof(ImageSource), typeof(DXTabItem), null);
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute, EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty ActualHeaderMenuContentProperty = ActualHeaderMenuContentPropertyKey.DependencyProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute, EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty ActualHeaderMenuContentTemplateProperty = ActualHeaderMenuContentTemplatePropertyKey.DependencyProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute, EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty ActualHeaderMenuGlyphProperty = ActualHeaderMenuGlyphPropertyKey.DependencyProperty;
		public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(DXTabItem));
		public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register("IconWidth", typeof(double), typeof(DXTabItem), new PropertyMetadata(16d));
		public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register("IconHeight", typeof(double), typeof(DXTabItem), new PropertyMetadata(16d));
		public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(ImageSource), typeof(DXTabItem));
		public static readonly DependencyProperty GlyphTemplateProperty = DependencyProperty.Register("GlyphTemplate", typeof(DataTemplate), typeof(DXTabItem));
		public static readonly DependencyProperty AllowHideProperty = DependencyProperty.Register("AllowHide", typeof(DefaultBoolean), typeof(DXTabItem),
			new PropertyMetadata(DefaultBoolean.Default, (d, e) => ((DXTabItem)d).UpdateActualAllowHide()));
		public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(DXTabItem),
			new PropertyMetadata((d, e) => ((DXTabItem)d).OnCloseCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue)));
		public static readonly DependencyProperty CloseCommandParameterProperty = DependencyProperty.Register("CloseCommandParameter", typeof(object), typeof(DXTabItem),
			new PropertyMetadata((d, e) => ((DXTabItem)d).OnCloseCommandParameterChanged()));
		public static readonly DependencyProperty CloseCommandTargetProperty = DependencyProperty.Register("CloseCommandTarget", typeof(IInputElement), typeof(DXTabItem));
		static readonly DependencyPropertyKey ActualCloseCommandPropertyKey = DependencyProperty.RegisterReadOnly("ActualCloseCommand", typeof(DelegateCommand), typeof(DXTabItem), null);
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute, EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty ActualCloseCommandProperty = ActualCloseCommandPropertyKey.DependencyProperty;
		static readonly DependencyPropertyKey ActualHideButtonVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("ActualHideButtonVisibility", typeof(Visibility), typeof(DXTabItem), null);
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute, EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty ActualHideButtonVisibilityProperty = ActualHideButtonVisibilityPropertyKey.DependencyProperty;
		public string BindableName { get { return (string)GetValue(BindableNameProperty); } set { SetValue(BindableNameProperty, value); } }
		public bool IsNew { get { return (bool)GetValue(IsNewProperty); } set { SetValue(IsNewProperty, value); } }
		public DataTemplate NormalBackgroundTemplate { get { return (DataTemplate)GetValue(NormalBackgroundTemplateProperty); } set { SetValue(NormalBackgroundTemplateProperty, value); } }
		public DataTemplate HoverBackgroundTemplate { get { return (DataTemplate)GetValue(HoverBackgroundTemplateProperty); } set { SetValue(HoverBackgroundTemplateProperty, value); } }
		public DataTemplate SelectedBackgroundTemplate { get { return (DataTemplate)GetValue(SelectedBackgroundTemplateProperty); } set { SetValue(SelectedBackgroundTemplateProperty, value); } }
		public DataTemplate FocusedBackgroundTemplate { get { return (DataTemplate)GetValue(FocusedBackgroundTemplateProperty); } set { SetValue(FocusedBackgroundTemplateProperty, value); } }
		public Color BackgroundColor { get { return (Color)GetValue(BackgroundColorProperty); } set { SetValue(BackgroundColorProperty, value); } }
		public Color BorderColor { get { return (Color)GetValue(BorderColorProperty); } set { SetValue(BorderColorProperty, value); } }
		public bool ShowToolTipForNonTrimmedHeader { get { return (bool)GetValue(ShowToolTipForNonTrimmedHeaderProperty); } set { SetValue(ShowToolTipForNonTrimmedHeaderProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXTabItemVisibleInHeaderMenu")]
#endif
		public bool VisibleInHeaderMenu { get { return (bool)GetValue(VisibleInHeaderMenuProperty); } set { SetValue(VisibleInHeaderMenuProperty, value); } }
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("DXTabItemHeaderMenuContent"),
#endif
 TypeConverter(typeof(ObjectConverter))]
		public object HeaderMenuContent { get { return (object)GetValue(HeaderMenuContentProperty); } set { SetValue(HeaderMenuContentProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXTabItemHeaderMenuContentTemplate")]
#endif
		public DataTemplate HeaderMenuContentTemplate { get { return (DataTemplate)GetValue(HeaderMenuContentTemplateProperty); } set { SetValue(HeaderMenuContentTemplateProperty, value); } }
		[Obsolete("Use the HeaderMenuGlyph property.")]
		public object HeaderMenuIcon { get { return (object)GetValue(HeaderMenuIconProperty); } set { SetValue(HeaderMenuIconProperty, value); } }
		public ImageSource HeaderMenuGlyph { get { return (ImageSource)GetValue(HeaderMenuGlyphProperty); } set { SetValue(HeaderMenuGlyphProperty, value); } }
		[Obsolete("Use the Glyph or GlyphTemplate property.")]
		public object Icon { get { return (object)GetValue(IconProperty); } set { SetValue(IconProperty, value); } }
		[Obsolete("Use the GlyphTemplate property.")]
		public double IconWidth { get { return (double)GetValue(IconWidthProperty); } set { SetValue(IconWidthProperty, value); } }
		[Obsolete("Use the GlyphTemplate property.")]
		public double IconHeight { get { return (double)GetValue(IconHeightProperty); } set { SetValue(IconHeightProperty, value); } }
		public ImageSource Glyph { get { return (ImageSource)GetValue(GlyphProperty); } set { SetValue(GlyphProperty, value); } }
		public DataTemplate GlyphTemplate { get { return (DataTemplate)GetValue(GlyphTemplateProperty); } set { SetValue(GlyphTemplateProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXTabItemAllowHide")]
#endif
		public DefaultBoolean AllowHide { get { return (DefaultBoolean)GetValue(AllowHideProperty); } set { SetValue(AllowHideProperty, value); } }
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("DXTabItemCloseCommand"),
#endif
 TypeConverter(typeof(CommandConverter))]
		public ICommand CloseCommand { get { return (ICommand)GetValue(CloseCommandProperty); } set { SetValue(CloseCommandProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXTabItemCloseCommandParameter")]
#endif
		public object CloseCommandParameter { get { return (object)GetValue(CloseCommandParameterProperty); } set { SetValue(CloseCommandParameterProperty, value); } }
		[Obsolete]
		public IInputElement CloseCommandTarget { get; set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXTabItemCloseButton")]
#endif
		public Button CloseButton { get; private set; }
		#endregion Properties
		internal static bool IsMouseLeftButtonDownOnDXTabItem(object sender, MouseButtonEventArgs e) {
			if(!(sender is DXTabItem)) return false;
			DXTabItem tab = (DXTabItem)sender;
			DependencyObject source = (DependencyObject)e.Source;
			if(tab != source) {
				var actualTab = LayoutTreeHelper.GetVisualParents(source, tab.Owner).OfType<DXTabItem>().FirstOrDefault();
				if(tab != actualTab) return false;
			}
			return true;
		}
		static DXTabItem() {
			NavigationAutomationPeersCreator.Default.RegisterObject(typeof(DXTabItem), typeof(DXTabItemAutomationPeer), owner => new DXTabItemAutomationPeer((DXTabItem)owner));
			ContentProperty.OverrideMetadata(typeof(DXTabItem), new FrameworkPropertyMetadata((d, e) => ((DXTabItem)d).UpdateHeaderMenuActualProperties()));
			ContentTemplateProperty.OverrideMetadata(typeof(DXTabItem), new FrameworkPropertyMetadata((d, e) => ((DXTabItem)d).UpdateHeaderMenuActualProperties()));
			ContentTemplateSelectorProperty.OverrideMetadata(typeof(DXTabItem), new FrameworkPropertyMetadata((d, e) => ((DXTabItem)d).UpdateHeaderMenuActualProperties()));
			HeaderProperty.OverrideMetadata(typeof(DXTabItem), new FrameworkPropertyMetadata((d, e) => ((DXTabItem)d).UpdateHeaderMenuActualProperties()));
			HeaderTemplateProperty.OverrideMetadata(typeof(DXTabItem), new FrameworkPropertyMetadata((d, e) => ((DXTabItem)d).UpdateHeaderMenuActualProperties()));
			HeaderTemplateSelectorProperty.OverrideMetadata(typeof(DXTabItem), new FrameworkPropertyMetadata((d, e) => ((DXTabItem)d).UpdateHeaderMenuActualProperties()));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DXTabItem), new FrameworkPropertyMetadata(typeof(DXTabItem)));
		}
		public DXTabItem() {
			SetValue(ActualCloseCommandPropertyKey, new DelegateCommand(() => Close(), CanClose, false));
			BarNameScope.SetIsScopeOwner(this, true);
			GotFocus += (d, e) => UpdateState();
			LostFocus += (d, e) => UpdateState();
			MouseEnter += (d, e) => { if(IsEnabled) VisualStateManager.GoToState(this, "MouseOver", true); };
			MouseLeave += (d, e) => { if(IsEnabled) VisualStateManager.GoToState(this, "Normal", true); };
			IsEnabledChanged += (d, e) => UpdateState();
			Loaded += OnLoaded;
			UpdateActualCloseCommand();
		}
		public FrameworkElement GetLayoutChild(string childName) {
			return (FrameworkElement)GetTemplateChild(childName);
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		void UpdateViewProperties() {
			Owner.Do(x => x.UpdateViewProperties());
		}
		internal void UpdateViewPropertiesСore() {
			if(Owner == null) return;
			UpdateActualCloseCommand();
			TabViewInfo newViewInfo = new TabViewInfo(this);
			TabViewInfo oldViewInfo = (TabViewInfo)GetValue(ViewInfoProperty);
			if(!newViewInfo.Equals(oldViewInfo)) 
				SetValue(ViewInfoPropertyKey, newViewInfo);
			var mergingBehavior = ElementMergingBehavior.Undefined;
			if ((Owner as DXTabControl).Return(x => x.AllowMerging, () => false).Return(x => x.Value, () => true)) {
				if (IsSelected)
					mergingBehavior = ElementMergingBehavior.InternalWithExternal;
				else
					mergingBehavior = ElementMergingBehavior.None;
			}
			MergingProperties.SetElementMergingBehavior(this, mergingBehavior);
			var doContent = Content as DependencyObject;
			if (doContent != null && LogicalTreeHelper.GetParent(doContent) != this) {
				MergingProperties.SetElementMergingBehavior(doContent, mergingBehavior);
			}
		}		
		public void Close() {
			if(!CanClose()) return;
			var view = Owner.View;
			Owner.HideTabItem(this);
			if(Visibility != Visibility.Collapsed) return;
			CloseCommand.Do(x => x.Execute(CloseCommandParameter));
			if(view.RemoveTabItemsOnHiding) 
				Owner.Do(x => x.RemoveTabItem(this));
			view.OnTabItemClosed(this);
		}
		public bool CanClose() {
			if(Owner == null || Visibility == Visibility.Collapsed) return false;
			if(AllowHide == DefaultBoolean.False) return false;
			if(!Owner.View.CanCloseTabItem(this)) return false;
			return CloseCommand.Return(x => x.CanExecute(CloseCommandParameter), () => true);
		}
		void OnCloseCommandChanged(ICommand oldCommand, ICommand newCommand) {
			oldCommand.Do(x => x.CanExecuteChanged -= OnCloseCommandCanExecuteChanged);
			newCommand.Do(x => x.CanExecuteChanged += OnCloseCommandCanExecuteChanged);
			UpdateActualCloseCommand();
		}
		void OnCloseCommandParameterChanged() {
			UpdateActualCloseCommand();
		}
		void OnCloseCommandCanExecuteChanged(object sender, EventArgs e) {
			UpdateActualCloseCommand();
		}
		void UpdateActualCloseCommand() {
			DelegateCommand actualCloseCommand = GetValue(ActualCloseCommandPropertyKey.DependencyProperty) as DelegateCommand;
			UpdateActualAllowHide();
			actualCloseCommand.RaiseCanExecuteChanged();
		}
		void UpdateActualAllowHide() {
			if(Owner == null) return;
			if(!Owner.View.CanCloseTabItem(this)) {
				SetValue(ActualHideButtonVisibilityPropertyKey, Visibility.Collapsed);
				return;
			}
			if(AllowHide == DefaultBoolean.True) {
				SetValue(ActualHideButtonVisibilityPropertyKey, Visibility.Visible);
				return;
			}
			if(AllowHide == DefaultBoolean.False) {
				SetValue(ActualHideButtonVisibilityPropertyKey, Visibility.Collapsed);
				return;
			}
			switch(Owner.View.HideButtonShowMode) {
				case HideButtonShowMode.InActiveTabAndHeaderArea:
				case HideButtonShowMode.InActiveTab:
					SetValue(ActualHideButtonVisibilityPropertyKey, IsSelected ? Visibility.Visible : Visibility.Hidden);
					return;
				case HideButtonShowMode.InAllTabsAndHeaderArea:
				case HideButtonShowMode.InAllTabs:
					SetValue(ActualHideButtonVisibilityPropertyKey, Visibility.Visible);
					return;
				case HideButtonShowMode.NoWhere:
				case HideButtonShowMode.InHeaderArea:
					SetValue(ActualHideButtonVisibilityPropertyKey, Visibility.Collapsed);
					return;
			}
		}
		void UpdateHeaderMenuActualProperties() {
			if(HeaderMenuContent != null)
				SetValue(ActualHeaderMenuContentPropertyKey, HeaderMenuContent);
			else if(Header is UIElement)
				SetValue(ActualHeaderMenuContentPropertyKey, Header.ToString());
			else if(Header != null)
				SetValue(ActualHeaderMenuContentPropertyKey, Header);
			else SetValue(ActualHeaderMenuContentPropertyKey, null);
			if(HeaderMenuContentTemplate != null)
				SetValue(ActualHeaderMenuContentTemplatePropertyKey, HeaderMenuContentTemplate);
			else if(HeaderTemplate != null || HeaderTemplateSelector != null)
				SetValue(ActualHeaderMenuContentTemplatePropertyKey, HeaderTemplate ?? HeaderTemplateSelector.SelectTemplate(this.Header, this));
			else SetValue(ActualHeaderMenuContentTemplatePropertyKey, null);
			if(HeaderMenuGlyph != null)
				SetValue(ActualHeaderMenuGlyphPropertyKey, HeaderMenuGlyph);
			else if(Glyph != null)
				SetValue(ActualHeaderMenuGlyphPropertyKey, Glyph);
			else SetValue(ActualHeaderMenuGlyphPropertyKey, null);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CloseButton = (Button)GetTemplateChild("PART_CloseButton");
			UpdateState();
		}
		protected override void OnOwnerChanged(DXTabControl oldValue, DXTabControl newValue) {
			base.OnOwnerChanged(oldValue, newValue);
			if(newValue == null) ClearVisibleIndex();
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateViewProperties();
			UpdateState();
		}
		protected override void OnIsSelectedChanged(bool oldValue, bool newValue) {
			base.OnIsSelectedChanged(oldValue, newValue);
			if(newValue && Owner.Return(x => x.IsKeyboardFocusWithin, () => false)) 
				Focus();
			UpdateViewProperties();
			UpdateState();
		}
		protected override void OnVisibilityChanged(Visibility oldValue, Visibility newValue) {
			base.OnVisibilityChanged(oldValue, newValue);
			if(oldValue != newValue) UpdateViewProperties();
		}
		protected override void OnIsEnabledChanged(bool oldValue, bool newValue) {
			base.OnIsEnabledChanged(oldValue, newValue);
			if(oldValue != newValue) UpdateViewProperties();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			TabControlKeyboardController.OnTabItemKeyDown(this, e);
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if(!IsMouseLeftButtonDownOnDXTabItem(this, e)) return;
			Focus();
			if(IsSelected) return;
			Owner.Do(x => x.SelectTabItem(this));
			e.Handled = IsSelected;
		}
		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			if(Owner != null && e.ChangedButton == MouseButton.Middle) {
				if(Owner.View.HideButtonShowMode == HideButtonShowMode.NoWhere && CloseCommand == null) return;
				Close();
				e.Handled = true;
			}
		}
		void UpdateState() {
			VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", true);
			VisualStateManager.GoToState(this, IsSelected ? "Selected" : "Unselected", true);
			VisualStateManager.GoToState(this, IsFocused ? "Focused" : "Unfocused", true);
		}
		internal void DisableFocusedState() {
			VisualStateManager.GoToState(this, "Unfocused", false);
		}
		internal object UnderlyingData;
		internal void Insert(DXTabControl target, int index) {
			object item = this;
			if(target.ItemsSource != null) {
				item = UnderlyingData;
				UnderlyingData = null;
			}
			target.InsertTabItem(item, index);
			target.SelectTabItem(item);
		}
		internal void Remove() {
			if(Owner == null) return;
			if(Owner.ItemsSource != null) {
				var item = Owner.ItemContainerGenerator.ItemFromContainer(this);
				UnderlyingData = item;
			}
			Owner.RemoveTabItem(this);
		}
		internal void Move(int index, bool realMove = true) {
			if(Owner == null) return;
			if(realMove) {
				Owner.MoveTabItem(this, index);
				return;
			}
			UpdateVisibleIndexes();
			var containers = Owner.GetContainers().OrderBy(x => x.VisibleIndex).ToList();
			containers.Remove(this);
			containers.Insert(index, this);
			for(int i = 0; i < containers.Count; i++)
				containers[i].VisibleIndex = i;
		}
		void UpdateVisibleIndexes() {
			var containers = Owner.GetContainers().OrderBy(x => x.VisibleIndex).ToList();
			for(int i = 0; i < containers.Count; i++)
				containers[i].VisibleIndex = i;
		}
		void ClearVisibleIndex() {
			VisibleIndex = int.MaxValue;
		}
		int visibleIndex = int.MaxValue;
		internal int VisibleIndex { get { return visibleIndex; } set { visibleIndex = value; } }
	}
}
