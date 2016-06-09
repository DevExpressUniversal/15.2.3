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
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Customization;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonCaptionButtonClickEventArgs : EventArgs {
		public RibbonCaptionButtonClickEventArgs(object captionButton) { CaptionButton = captionButton; }
		public object CaptionButton { get; set; }
	}
	public delegate void RibbonCaptionButtonClickEventHandler(object sender, RibbonCaptionButtonClickEventArgs e);
	[ContentProperty("Items")]
	public class RibbonPageGroup : FrameworkContentElement, ILinksHolder, ICloneable, ILogicalChildrenContainer, IBarManagerControllerAction {
		#region static
		private static readonly object captionButtonClickEventHandler;
		private static readonly object afterMergeEventHandler;
		private static readonly object afterUnMergeEventHandler;
		private static readonly object isVisibleChangedEventHandler;
		public static readonly DependencyProperty ActualIsVisibleProperty;
		protected static readonly DependencyPropertyKey ActualIsVisiblePropertyKey;
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty PageProperty;
		protected internal static readonly DependencyPropertyKey PagePropertyKey;
		public static readonly DependencyProperty ShowCaptionButtonProperty;
		public static readonly DependencyProperty IsCaptionButtonEnabledProperty;
		public static readonly DependencyProperty CaptionButtonCommandProperty;
		public static readonly DependencyProperty CaptionButtonCommandTargetProperty;
		public static readonly DependencyProperty CaptionButtonCommandParameterProperty;
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty SmallGlyphProperty;
		public static readonly DependencyProperty AllowCollapseProperty;
		public static readonly DependencyProperty SuperTipProperty;
		public static readonly DependencyProperty IsVisibleProperty;
		public static readonly DependencyProperty ItemLinksSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty RibbonBarItemsAttachedBehaviorProperty; 
		public static readonly DependencyProperty KeyTipProperty;
		public static readonly DependencyProperty KeyTipGroupExpandingProperty;
		public static readonly DependencyProperty AllowRemoveFromParentDuringCustomizationProperty;
		public static readonly DependencyProperty ItemLinksSourceElementGeneratesUniqueBarItemProperty;
		public static readonly DependencyProperty HideWhenEmptyProperty;
		public static readonly DependencyProperty MergeOrderProperty;
		public static readonly DependencyProperty MergeTypeProperty;
		public static readonly DependencyProperty IndexProperty;
		public static readonly DependencyProperty IsRemovedProperty;		
		static RibbonPageGroup() {
			captionButtonClickEventHandler = new Object();
			afterMergeEventHandler = new Object();
			afterUnMergeEventHandler = new Object();
			isVisibleChangedEventHandler = new Object();
			Type ribbonPageGroupType = typeof(RibbonPageGroup);
			IsRemovedProperty = DependencyPropertyManager.Register("IsRemoved", typeof(bool), typeof(RibbonPageGroup), new FrameworkPropertyMetadata(false, (d, e) => ((RibbonPageGroup)d).UpdateActualIsVisible()));
			IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), typeof(RibbonPageGroup), new FrameworkPropertyMetadata(0));
			HideWhenEmptyProperty = DependencyPropertyManager.Register("HideWhenEmpty", typeof(bool?), ribbonPageGroupType, new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((RibbonPageGroup)d).OnHideWhenEmptyChanged((bool?)e.OldValue))));
			AllowRemoveFromParentDuringCustomizationProperty = DependencyPropertyManager.Register("AllowRemoveFromParentDuringCustomization", typeof(bool), ribbonPageGroupType, new FrameworkPropertyMetadata(true));
			ActualIsVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsVisible", typeof(bool), ribbonPageGroupType, new FrameworkPropertyMetadata(true));
			ActualIsVisibleProperty = ActualIsVisiblePropertyKey.DependencyProperty;
			CaptionProperty = DependencyPropertyManager.Register("Caption", typeof(string), ribbonPageGroupType, new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure));
			GlyphProperty = DependencyPropertyManager.Register("Glyph", typeof(ImageSource), ribbonPageGroupType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			SmallGlyphProperty = DependencyPropertyManager.Register("SmallGlyph", typeof(ImageSource), ribbonPageGroupType, new PropertyMetadata(null));
			PagePropertyKey = DependencyPropertyManager.RegisterReadOnly("Page", typeof(RibbonPage), ribbonPageGroupType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnPagePropertyChanged)));
			PageProperty = PagePropertyKey.DependencyProperty;
			ShowCaptionButtonProperty = DependencyPropertyManager.Register("ShowCaptionButton", typeof(bool), ribbonPageGroupType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));			
			IsCaptionButtonEnabledProperty = DependencyPropertyManager.Register("IsCaptionButtonEnabled", typeof(bool), ribbonPageGroupType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIsCaptionButtonEnabledPropertyChanged), new CoerceValueCallback(OnIsCaptionButtonEnabledPropertyCoerce)));
			CaptionButtonCommandProperty = DependencyPropertyManager.Register("CaptionButtonCommand", typeof(ICommand), ribbonPageGroupType, new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCaptionButtonCommandPropertyChanged)));
			CaptionButtonCommandTargetProperty = DependencyPropertyManager.Register("CaptionButtonCommandTarget", typeof(IInputElement), ribbonPageGroupType, new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCaptionButtonCommandTargetPropertyChanged)));
			CaptionButtonCommandParameterProperty = DependencyPropertyManager.Register("CaptionButtonCommandParameter", typeof(object), ribbonPageGroupType, new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCaptionButtonCommandParameterPropertyChanged)));
			AllowCollapseProperty = DependencyPropertyManager.Register("AllowCollapse", typeof(bool), ribbonPageGroupType, new UIPropertyMetadata(true));
			SuperTipProperty = DependencyPropertyManager.Register("SuperTip", typeof(SuperTip), ribbonPageGroupType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnSuperTipPropertyChanged)));			
			KeyTipProperty = DependencyPropertyManager.Register("KeyTip", typeof(string), ribbonPageGroupType, new FrameworkPropertyMetadata(string.Empty));
			KeyTipGroupExpandingProperty = DependencyProperty.Register("KeyTipGroupExpanding", typeof(string), ribbonPageGroupType, new FrameworkPropertyMetadata(string.Empty));
			IsVisibleProperty = DependencyPropertyManager.Register("IsVisible", typeof(bool), ribbonPageGroupType,
				new FrameworkPropertyMetadata(true, OnIsVisiblePropertyChanged));
			ItemLinksSourceProperty = DependencyProperty.Register("ItemLinksSource", typeof(object), ribbonPageGroupType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksSourcePropertyChanged)));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), ribbonPageGroupType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), ribbonPageGroupType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplateSelectorPropertyChanged)));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), ribbonPageGroupType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			RibbonBarItemsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("RibbonBarItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<RibbonPageGroup, BarItem>), ribbonPageGroupType, new UIPropertyMetadata(null));
			ItemLinksSourceElementGeneratesUniqueBarItemProperty = DependencyPropertyManager.Register("ItemLinksSourceElementGeneratesUniqueBarItem", typeof(bool), ribbonPageGroupType, new FrameworkPropertyMetadata(false, new PropertyChangedCallback((d, e) => ((RibbonPageGroup)d).OnItemLinksSourceElementGeneratesUniqueBarItemChanged((bool)e.OldValue))));
			MergeOrderProperty = DependencyPropertyManager.Register("MergeOrder", typeof(int), ribbonPageGroupType, new FrameworkPropertyMetadata(-1));
			MergeTypeProperty = DependencyPropertyManager.Register("MergeType", typeof(RibbonMergeType), ribbonPageGroupType, new FrameworkPropertyMetadata(RibbonMergeType.MergeItems));
			NameProperty.OverrideMetadata(ribbonPageGroupType, new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback((d, e) => ((RibbonPageGroup)d).OnNameChanged(e.OldValue as string))));
		}
		private void OnHideWhenEmptyChanged(bool? oldValue) {
			ExecuteActionOnPageGroupControl(x => x.OnInfoVisibilityChanged(null, EventArgs.Empty));			
		}		
		protected static void OnSuperTipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroup)d).OnSuperTipChanged(e);
		}
		protected static void OnPagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroup)d).OnPageChanged(e);
		}
		protected static void OnIsCaptionButtonEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroup)d).IsCaptionButtonEnabledChanged((bool)e.OldValue);
		}
		protected static object OnIsCaptionButtonEnabledPropertyCoerce(DependencyObject d, object value) {
			return ((RibbonPageGroup)d).CoerceIsCaptionButtonEnabled((bool)value);
		}
		protected static void OnCaptionButtonCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroup)d).CaptionButtonCommandChanged((ICommand)e.OldValue);
		}
		protected static void OnCaptionButtonCommandTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroup)d).CaptionButtonCommandTargetChanged((IInputElement)e.OldValue);
		}
		protected static void OnCaptionButtonCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroup)d).CaptionButtonCommandParameterChanged(e.OldValue);
		}
		protected static void OnIsVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroup)d).OnIsVisibleChanged((bool)e.OldValue);
		}
		protected static void OnItemLinksSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroup)d).OnItemLinksSourceChanged(e);
		}
		protected static void OnItemLinksTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroup)d).OnItemLinksTemplateChanged(e);
		}
		protected static void OnItemLinksTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroup)d).OnItemLinksTemplateSelectorChanged(e);
		}
		#endregion
		public RibbonPageGroup() {
			if(this.IsInDesignTool())
				SetCurrentValue(CaptionProperty, "Ribbon Page Group");
			InitBindings();
			DXSerializer.SetSerializationIDDefault(this, "RibbonPageGroup");
			ContextLayoutManagerHelper.AddLayoutUpdatedHandler(this, OnLayoutUpdated);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			immediateActionsManager.ExecuteActions();
		}
		protected virtual void InitBindings() {			
		}
		internal List<RibbonPageGroupControl> PageGroupControls = new List<RibbonPageGroupControl>();
		BarItemLinkCollection itemLinks;
		EventHandler CaptionButtonCommandCanExecuteChangedEventHandler; 
		#region dep props
		public bool ActualIsVisible {
			get { return (bool)GetValue(ActualIsVisibleProperty); }
			protected set { this.SetValue(ActualIsVisiblePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageGroupShowCaptionButton")]
#endif
		public bool ShowCaptionButton {
			get { return (bool)GetValue(ShowCaptionButtonProperty); }
			set { SetValue(ShowCaptionButtonProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageGroupIsCaptionButtonEnabled")]
#endif
		public bool IsCaptionButtonEnabled {
			get { return (bool)GetValue(IsCaptionButtonEnabledProperty); }
			set { SetValue(IsCaptionButtonEnabledProperty, value); }
		}
		public ICommand CaptionButtonCommand {
			get { return (ICommand)GetValue(CaptionButtonCommandProperty); }
			set { SetValue(CaptionButtonCommandProperty, value); }
		}
		public IInputElement CaptionButtonCommandTarget {
			get { return (IInputElement)GetValue(CaptionButtonCommandTargetProperty); }
			set { SetValue(CaptionButtonCommandTargetProperty, value); }
		}
		public object CaptionButtonCommandParameter {
			get { return (object)GetValue(CaptionButtonCommandParameterProperty); }
			set { SetValue(CaptionButtonCommandParameterProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageGroupCaption")]
#endif
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageGroupGlyph")]
#endif
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageGroupSmallGlyph")]
#endif
		public ImageSource SmallGlyph {
			get { return (ImageSource)GetValue(SmallGlyphProperty); }
			set { SetValue(SmallGlyphProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageGroupAllowCollapse")]
#endif
		public bool AllowCollapse {
			get { return (bool)GetValue(AllowCollapseProperty); }
			set { SetValue(AllowCollapseProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageGroupIsVisible")]
#endif
		public bool IsVisible {
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}		
		public object ItemLinksSource {
			get { return GetValue(ItemLinksSourceProperty); }
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
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageGroupKeyTip")]
#endif
		public string KeyTip {
			get { return (string)GetValue(KeyTipProperty); }
			set { SetValue(KeyTipProperty, value); }
		}
		public string KeyTipGroupExpanding {
			get { return (string)GetValue(KeyTipGroupExpandingProperty); }
			set { SetValue(KeyTipGroupExpandingProperty, value); }
		}
		public bool? HideWhenEmpty {
			get { return ((bool?)GetValue(HideWhenEmptyProperty)); }
			set { SetValue(HideWhenEmptyProperty, value); }
		}
		#endregion                
		public int MergeOrder {
			get { return (int)GetValue(MergeOrderProperty); }
			set { SetValue(MergeOrderProperty, value); }
		}
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public bool IsRemoved {
			get { return (bool)GetValue(IsRemovedProperty); }
			set { SetValue(IsRemovedProperty, value); }
		}		
		public RibbonMergeType MergeType {
			get { return (RibbonMergeType)GetValue(MergeTypeProperty); }
			set { SetValue(MergeTypeProperty, value); }
		}
		public RibbonControl Ribbon { get { return Page == null ? null : Page.Ribbon; } }
		public RibbonPage Page {
			get { return (RibbonPage)GetValue(PageProperty); }
			protected internal set { this.SetValue(PagePropertyKey, value); }
		}
		public bool ActualHideWhenEmpty { get { return HideWhenEmpty ?? Ribbon.Return(ribbon => ribbon.HideEmptyGroups, () => false); } }
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageGroupAllowRemoveFromParentDuringCustomization")]
#endif
		public bool AllowRemoveFromParentDuringCustomization {
			get { return (bool)GetValue(AllowRemoveFromParentDuringCustomizationProperty); }
			set { SetValue(AllowRemoveFromParentDuringCustomizationProperty, value); }
		}
		public SuperTip SuperTip {
			get { return (SuperTip)GetValue(SuperTipProperty); }
			set { SetValue(SuperTipProperty, value); }
		}
		EventHandlerList events;
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		protected void RaiseEventByHandler(object eventHandler, RibbonCaptionButtonClickEventArgs args) {
			RibbonCaptionButtonClickEventHandler h = Events[eventHandler] as RibbonCaptionButtonClickEventHandler;
			if(h != null) h(this, args);
		}
		protected void RaiseEventByHandler(object eventHandler, EventArgs args) {
			EventHandler h = Events[eventHandler] as EventHandler;
			if(h != null) h(this, args);
		}
		#region events
		public event RibbonCaptionButtonClickEventHandler CaptionButtonClick {
			add { Events.AddHandler(captionButtonClickEventHandler, value); }
			remove { Events.RemoveHandler(captionButtonClickEventHandler, value); }
		}
		protected internal virtual void OnCaptionButtonClick(object captionButton) {
			RaiseEventByHandler(captionButtonClickEventHandler, new RibbonCaptionButtonClickEventArgs(captionButton));
			ExecuteCaptionButtonCommand();
		}
		protected internal event EventHandler AfterMerge {
			add { Events.AddHandler(afterMergeEventHandler, value); }
			remove { Events.RemoveHandler(afterMergeEventHandler, value); }
		}
		protected internal event EventHandler AfterUnMerge {
			add { Events.AddHandler(afterUnMergeEventHandler, value); }
			remove { Events.RemoveHandler(afterUnMergeEventHandler, value); }
		}
		protected internal event EventHandler IsVisibleChanged {
			add { Events.AddHandler(isVisibleChangedEventHandler, value); }
			remove { Events.RemoveHandler(isVisibleChangedEventHandler, value); }
		}
		#endregion
		internal RibbonPageGroupControl FirstPageGroupControl { get { return PageGroupControls.Count == 0 ? null : PageGroupControls[0]; } }
		protected delegate void PageGroupControlAction(RibbonPageGroupControl pageGroupControl);
		protected void ExecuteActionOnPageGroupControl(PageGroupControlAction action) {
			foreach(RibbonPageGroupControl control in PageGroupControls)
				action(control);
		}
		protected virtual void OnSuperTipChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnPageGroupControl((pg) => pg.UpdateSuperTip());
		}
		protected virtual void OnPageChanged(DependencyPropertyChangedEventArgs e) {
			RibbonPage oldValue = e.OldValue as RibbonPage;
			UpdateActualIsVisible();
		}
		protected internal void UpdateActualIsVisible() {
			if (IsRemoved)
				ActualIsVisible = false;
			else				
				ActualIsVisible = Page == null ? IsVisible : Page.ActualIsVisible && IsVisible;
			UpdateLinksActualIsVisible();
		}
		protected virtual void UpdateIsCaptionButtoonEnabled() {
			this.CoerceValue(IsCaptionButtonEnabledProperty);
		}
		protected virtual void IsCaptionButtonEnabledChanged(bool p) {
		}		
		protected virtual bool CoerceIsCaptionButtonEnabled(bool value) {			
			return value && GetCaptionButtonCommandCanExecute();
		}		
		protected virtual void CaptionButtonCommandChanged(ICommand oldValue) {
			UnhookCommand(oldValue);
			HookCommand(CaptionButtonCommand);
			UpdateIsCaptionButtoonEnabled();
		}
		protected virtual void CaptionButtonCommandTargetChanged(IInputElement oldValue) {
			UpdateIsCaptionButtoonEnabled();
		}
		protected virtual void CaptionButtonCommandParameterChanged(object oldValue) {
			UpdateIsCaptionButtoonEnabled();
		}
		protected virtual void OnCaptionButtonCommandCanExecuteChanged(object sender, EventArgs e) {
			UpdateIsCaptionButtoonEnabled();
		}
		protected virtual bool GetCaptionButtonCommandCanExecute() {
			if(CaptionButtonCommand == null)
				return true;
			RoutedCommand routedCommand = CaptionButtonCommand as RoutedCommand;
			if(routedCommand != null)
				return routedCommand.CanExecute(CaptionButtonCommandParameter, CaptionButtonCommandTarget);
			return CaptionButtonCommand.CanExecute(CaptionButtonCommandParameter);
		}
		protected virtual void HookCommand(ICommand command) {
			if(command == null)
				return;
			CaptionButtonCommandCanExecuteChangedEventHandler = new EventHandler(OnCaptionButtonCommandCanExecuteChanged);
			command.CanExecuteChanged += CaptionButtonCommandCanExecuteChangedEventHandler;
		}		
		protected virtual void UnhookCommand(ICommand command) {
			if(command == null)
				return;
			command.CanExecuteChanged -= OnCaptionButtonCommandCanExecuteChanged;
			CaptionButtonCommandCanExecuteChangedEventHandler = null;
		}
		protected virtual void ExecuteCaptionButtonCommand() {
			if(CaptionButtonCommand == null)
				return;
			RoutedCommand routedCommand = CaptionButtonCommand as RoutedCommand;
			if(routedCommand != null) {
				routedCommand.Execute(CaptionButtonCommandParameter, CaptionButtonCommandTarget);
				return;
			}
			CaptionButtonCommand.Execute(CaptionButtonCommandParameter);
		}
		CommonBarItemCollection itemsCore;
		public CommonBarItemCollection Items {
			get {
				if(itemsCore == null)
					itemsCore = new CommonBarItemCollection(this);
				return itemsCore;
			}
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageGroupItemLinks")]
#endif
		public BarItemLinkCollection ItemLinks {
			get {
				if(itemLinks == null)
					itemLinks = CreateItemLinks();
				return itemLinks;
			}
		}
		ObservableCollection<ILinksHolder> mergedLinksHolders;
		protected ObservableCollection<ILinksHolder> MergedLinksHolders {
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
			if (e.OldItems != null)
			foreach (ILinksHolder value in e.OldItems) {
				value.MergedParent = null;
			}
			if (e.NewItems != null)
			foreach (ILinksHolder value in e.NewItems) {
				value.MergedParent = this;
			}
		}
		protected virtual BarItemLinkCollection CreateItemLinks() {
			return new BarItemLinkCollection(this);
		}
		protected virtual void OnIsVisibleChanged(bool oldValue) {
			UpdateActualIsVisible();
			RaiseEventByHandler(isVisibleChangedEventHandler, new EventArgs());
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			this.SafeOnPropertyChanged(e);
		}
		protected virtual void OnNameChanged(string oldValue) {
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(IFrameworkInputElement), oldValue, Name);
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(ILinksHolder), oldValue, Name);
		}		
		protected virtual void UpdateLinksActualIsVisible() {
			foreach(BarItemLinkBase link in ItemLinks) {
				link.HoldersIsVisible = ActualIsVisible;
			}
		}
		protected virtual void OnItemLinksSourceElementGeneratesUniqueBarItemChanged(bool oldValue) {
			OnItemLinksSourceChanged(new System.Windows.DependencyPropertyChangedEventArgs(ItemLinksSourceProperty, ItemLinksSource, ItemLinksSource));
		}
		private void OnItemLinksTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonPageGroup, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				RibbonBarItemsAttachedBehaviorProperty);
		}
		private void OnItemLinksTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonPageGroup, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				RibbonBarItemsAttachedBehaviorProperty);
		}
		protected virtual void OnItemLinksSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemGeneratorHelper.OnItemsSourceChanged(e);
		}
		BarItemGeneratorHelper<RibbonPageGroup> itemGeneratorHelper;
		protected BarItemGeneratorHelper<RibbonPageGroup> ItemGeneratorHelper {
			get {
				if(itemGeneratorHelper == null)
					itemGeneratorHelper = new BarItemGeneratorHelper<RibbonPageGroup>(this, RibbonBarItemsAttachedBehaviorProperty, ItemStyleProperty, ItemTemplateProperty, Items, ItemTemplateSelectorProperty, ItemLinksSourceElementGeneratesUniqueBarItem);
				return itemGeneratorHelper;
			}
		}
		#region ILinksHolder Members
		bool ILinksHolder.ShowDescription { get { return false; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public BarItemLinkCollection Links {
			get { return ItemLinks; }
		}
		internal BarItemLinkCollection mergedLinks;
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
		IEnumerable ILinksHolder.ItemsSource { get { return ItemLinksSource as IEnumerable; } }
		BarItemLinkCollection ILinksHolder.ActualLinks {
			get { return ((ILinksHolder)this).IsMergedState ? ((ILinksHolder)this).MergedLinks : ((ILinksHolder)this).Links; }
		}
		ILinksHolder mergedParent;
		ILinksHolder ILinksHolder.MergedParent {
			get { return mergedParent; }
			set { mergedParent = value; }
		}
		void ILinksHolder.Merge(ILinksHolder holder) {
			if(MergedLinksHolders.Contains(holder))
				return;
			MergedLinksHolders.Add(holder);
			RaiseEventByHandler(afterMergeEventHandler, new EventArgs());
		}
		void ILinksHolder.UnMerge(ILinksHolder holder) {
			MergedLinksHolders.Remove(holder);
			RaiseEventByHandler(afterUnMergeEventHandler, new EventArgs());
		}
		void ILinksHolder.UnMerge() {
			MergedLinksHolders.Clear();
		}
		bool ILinksHolder.IsMergedState { get { return MergedLinksHolders.Count > 0; } }
		LinksHolderType ILinksHolder.HolderType { get { return LinksHolderType.RibbonPageGroup; } }
		public GlyphSize ItemsGlyphSize {
			get { return GlyphSize.Small; }
		}
		public GlyphSize GetDefaultItemsGlyphSize(LinkContainerType linkContainerType) {
			return GlyphSize.Small;
		}
		public System.Collections.IEnumerator GetLogicalChildrenEnumerator() {
			return logicalChildrenContainerItems.GetEnumerator();
		}
		void ILinksHolder.OnLinkAdded(BarItemLinkBase link) {
			link.HoldersIsVisible = ActualIsVisible;
			if(link.IsPrivate) return;
			AddLogicalChild(link);
		}
		void ILinksHolder.OnLinkRemoved(BarItemLinkBase link) {
			if(link.IsPrivate) return;
			RemoveLogicalChild(link);
		}
		#endregion
		#region ICloneable Members
		object ICloneable.Clone() {
			RibbonPageGroup res = new RibbonPageGroup();
			res.AssignPropertiesFromSource(this);
			res.CloneLinksFromSource(this);
			return res;
		}
		#endregion
		protected override IEnumerator LogicalChildren {
			get {
				return ((ILinksHolder)this).GetLogicalChildrenEnumerator();
			}
		}
		protected virtual void AssignPropertiesFromSource(RibbonPageGroup source) {
			MergeType = source.MergeType;
			MergeOrder = source.MergeOrder;
			Caption = source.Caption;
		}
		protected virtual void CloneLinksFromSource(RibbonPageGroup source) {
			foreach(BarItemLinkBase linkBase in source.ItemLinks) {				
				BarItemLinkBase cloneLink = (BarItemLinkBase)((ICloneable)linkBase).Clone();
				ItemLinks.Add(cloneLink);
			}
		}
		#region merging
		internal int ActualMergeOrder { get { return MergedChildren.Count == 0 ? MergeOrder : MergedChildren[MergedChildren.Count - 1].MergeOrder; } }
		internal RibbonPageGroup MergedParent { get; set; }
		internal RibbonPageGroup ReplacedPageGroup { get; set; }
		List<RibbonPageGroup> MergedChildren = new List<RibbonPageGroup>();
		internal void Merge(RibbonPageGroup childGroup) {
			if(MergedChildren.Contains(childGroup))
				return;
			MergedChildren.Add(childGroup);
			((ILinksHolder)this).Merge(childGroup);
		}
		internal void UnMerge(RibbonPageGroup childGroup) {
			if(!MergedChildren.Contains(childGroup))
				return;
			MergedChildren.Remove(childGroup);
			((ILinksHolder)this).UnMerge(childGroup);
		}
		#endregion        
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
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { return new object[] { typeof(ILinksHolder), typeof(IFrameworkInputElement) }; }
		}
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {
			if (Equals(registratorKey, typeof(IFrameworkInputElement)))
				return Name;
			if (Equals(registratorKey, typeof(ILinksHolder)))
				return Name;
			throw new ArgumentException();
		}
		#endregion
		IActionContainer IControllerAction.Container { get; set; }
		object IBarManagerControllerAction.GetObject() { return this; }
		void IControllerAction.Execute(DependencyObject context) { CollectionActionHelper.Execute(this); }
		protected internal bool CreatedByCustomizationDialog { get; set; }
	}
}
