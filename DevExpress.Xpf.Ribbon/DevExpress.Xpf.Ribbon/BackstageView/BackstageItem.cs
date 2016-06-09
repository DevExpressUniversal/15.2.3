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
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Ribbon {
	public class BackstageItemBase : Control {
		#region static
		private static readonly object clickEventHandler = new object();
		public static readonly DependencyProperty BackstageProperty;
		public static readonly DependencyProperty ActualIsEnabledProperty;
		protected static readonly DependencyPropertyKey ActualIsEnabledPropertyKey;
		public static readonly DependencyProperty BorderStyleProperty;
		public static readonly DependencyProperty NormalTextStyleProperty;
		public static readonly DependencyProperty HoverTextStyleProperty;
		public static readonly DependencyProperty ContentStyleProperty;
		public static readonly DependencyProperty ActualTextStyleProperty;
		protected static readonly DependencyPropertyKey ActualTextStylePropertyKey;
		public static readonly DependencyProperty ActualIsFocusedProperty;
		protected internal static readonly DependencyPropertyKey ActualIsFocusedPropertyKey;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		protected static readonly DependencyProperty IsEnabledInternalProperty;
		static BackstageItemBase() {
			ActualIsEnabledPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsEnabled", typeof(bool), typeof(BackstageItemBase),
				new FrameworkPropertyMetadata(true));
			ActualIsEnabledProperty = ActualIsEnabledPropertyKey.DependencyProperty;
			BackstageProperty = DependencyPropertyManager.Register("Backstage", typeof(BackstageViewControl), typeof(BackstageItemBase),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBackstagePropertyChanged)));
			BorderStyleProperty = DependencyPropertyManager.Register("BorderStyle", typeof(Style), typeof(BackstageItemBase), new FrameworkPropertyMetadata(null));
			NormalTextStyleProperty = DependencyPropertyManager.Register("NormalTextStyle", typeof(Style),
				typeof(BackstageItemBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNormalTextStylePropertyChanged)));
			HoverTextStyleProperty = DependencyPropertyManager.Register("HoverTextStyle", typeof(Style), typeof(BackstageItemBase),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHoverTextStylePropertyChanged)));
			ContentStyleProperty = DependencyPropertyManager.Register("ContentStyle", typeof(Style), typeof(BackstageItemBase), new FrameworkPropertyMetadata(null));
			ActualTextStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualTextStyle", typeof(Style), typeof(BackstageItemBase), new FrameworkPropertyMetadata(null));
			ActualTextStyleProperty = ActualTextStylePropertyKey.DependencyProperty;
			IsEnabledInternalProperty = DependencyPropertyManager.Register("IsEnabledInternal", typeof(bool), typeof(BackstageItemBase),
				new FrameworkPropertyMetadata(false, OnIsEnabledInternalPropertyChanged));
			ActualIsFocusedPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsFocused", typeof(bool), typeof(BackstageItemBase), new FrameworkPropertyMetadata(false));
			ActualIsFocusedProperty = ActualIsFocusedPropertyKey.DependencyProperty;
		}
		protected static void OnBackstagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageItemBase)d).OnBackstageChanged(e.OldValue as BackstageViewControl);
		}
		protected static void OnNormalTextStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageItemBase)d).OnNormalTextStyleChanged(e.OldValue as Style);
		}
		protected static void OnHoverTextStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageItemBase)d).OnHoverTextStyleChanged(e.OldValue as Style);
		}
		protected static void OnIsEnabledInternalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageItemBase)d).OnIsEnabledChanged((bool)e.OldValue);
		}
		#endregion
		#region dep props
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public BackstageViewControl Backstage {
			get { return (BackstageViewControl)GetValue(BackstageProperty); }
			set { SetValue(BackstageProperty, value); }
		}
		[Browsable(false)]
		public Style BorderStyle {
			get { return (Style)GetValue(BorderStyleProperty); }
			set { SetValue(BorderStyleProperty, value); }
		}
		[Browsable(false)]
		public Style NormalTextStyle {
			get { return (Style)GetValue(NormalTextStyleProperty); }
			set { SetValue(NormalTextStyleProperty, value); }
		}
		[Browsable(false)]
		public Style HoverTextStyle {
			get { return (Style)GetValue(HoverTextStyleProperty); }
			set { SetValue(HoverTextStyleProperty, value); }
		}
		[Browsable(false)]
		public Style ContentStyle {
			get { return (Style)GetValue(ContentStyleProperty); }
			set { SetValue(ContentStyleProperty, value); }
		}
		[Browsable(false)]
		public Style ActualTextStyle {
			get { return (Style)GetValue(ActualTextStyleProperty); }
			protected set { this.SetValue(ActualTextStylePropertyKey, value); }
		}		
		protected bool IsEnabledInternal {
			get { return (bool)GetValue(IsEnabledInternalProperty); }
			set { SetValue(IsEnabledInternalProperty, value); }
		}
		public bool ActualIsEnabled {
			get { return (bool)GetValue(ActualIsEnabledProperty); }
			protected set { this.SetValue(ActualIsEnabledPropertyKey, value); }
		}
		public bool ActualIsFocused {
			get { return (bool)GetValue(ActualIsFocusedProperty); }
			protected internal set { this.SetValue(ActualIsFocusedPropertyKey, value); }
		}
		#endregion
		#region events
		EventHandlerList events;
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		public event EventHandler Click {
			add { Events.AddHandler(clickEventHandler, value); }
			remove { Events.RemoveHandler(clickEventHandler, value); }
		}
		protected virtual void RaiseEvent(object eventHandler) {
			EventHandler handler = Events[eventHandler] as EventHandler;
			if(handler != null)
				handler(this, new EventArgs());
		}
		protected virtual void RaiseClick() {
			RaiseEvent(clickEventHandler);			  
		}
		#endregion
		public BackstageItemBase() {
			IsEnabledInternal = IsEnabled;
			Binding bnd = new Binding("IsEnabled");
			bnd.Source = this;
			bnd.Mode = BindingMode.TwoWay;
			SetBinding(BackstageButtonItem.IsEnabledInternalProperty, bnd);
			DefaultStyleKey = typeof(BackstageItemBase);			
			UpdateActualTextStyle();
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
		}
		bool backstageWasSetOnLoaded = true;
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			if(backstageWasSetOnLoaded)
				Backstage = null;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			if(Backstage == null) {
				var leftPartContentControl = LayoutHelper.FindParentObject<BackstageViewLeftPartContentControl>(this);
				if(leftPartContentControl != null) {
					BackstageViewControl parentBackstageView = LayoutHelper.FindParentObject<BackstageViewControl>(leftPartContentControl);
					if(parentBackstageView != null) {
						Backstage = parentBackstageView;
						backstageWasSetOnLoaded = true;
					}
				}
			}
		}
		protected internal virtual void OnClick() {
			if(Backstage != null)
				Backstage.OnItemClick(this);
			RaiseClick();
		}
		protected bool IsMouseLeftButtonDown { get; private set; }
		protected virtual void OnItemChanged(BackstageItemBase oldValue) { }
		protected virtual void OnBackstageChanged(BackstageViewControl oldValue) {
			backstageWasSetOnLoaded = false;
			if (!(this is BackstageTabItem)) return;
			if(oldValue != null)
				oldValue.TabsCore.Remove(this);
			if(Backstage != null && !Backstage.TabsCore.Contains(this))
				Backstage.TabsCore.Add(this);
		}
		protected virtual void OnContentChanged(object oldValue) {
		}
		protected virtual void OnIsEnabledChanged(bool oldValue) {
			UpdateActualIsEnabled();
		}
		protected virtual void OnHoverTextStyleChanged(Style oldValue) {
			UpdateActualTextStyle();			
		}
		protected virtual void OnNormalTextStyleChanged(Style oldValue) {
			UpdateActualTextStyle();
		}
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			IsMouseLeftButtonDown = true;
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			IsMouseLeftButtonDown = false;
			UpdateActualTextStyle();
		}
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateActualTextStyle();
		}
		protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			if (IsMouseLeftButtonDown)
				OnClick();
			IsMouseLeftButtonDown = false;
		}
		protected virtual void UpdateActualTextStyle() {
			ActualTextStyle = IsMouseOver ? HoverTextStyle : NormalTextStyle;
		}
		protected virtual void UpdateActualIsEnabled() {
			ActualIsEnabled = IsEnabled;
		}		
	}
	public class BackstageItem : BackstageItemBase  {
		#region static
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty RibbonStyleProperty;
		public static readonly DependencyProperty KeyTipProperty;
		static BackstageItem() {
			Type ownerType = typeof(BackstageItem);
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(BackstageItem),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnContentPropertyChanged)));
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), typeof(BackstageItem), new FrameworkPropertyMetadata(null));
			KeyTipProperty = DependencyPropertyManager.Register("KeyTip", typeof(string), typeof(BackstageItem), new FrameworkPropertyMetadata(string.Empty));
			RibbonStyleProperty = RibbonControl.RibbonStyleProperty.AddOwner(ownerType, new PropertyMetadata(OnRibbonStylePropertyChanged));
		}
		static void OnRibbonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageItem)d).RibbonStyleChaged((RibbonStyle)e.OldValue);
		}
		protected static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageItem)d).OnContentChanged(e.OldValue);
		}
		#endregion
		#region dep props
		[TypeConverter(typeof(ObjectConverter)), Category("Common")]
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { this.SetValue(ContentTemplateProperty, value); }
		}
		public RibbonStyle RibbonStyle {
			get { return (RibbonStyle)GetValue(RibbonStyleProperty); }
			set { SetValue(RibbonStyleProperty, value); }
		}
		public string KeyTip {
			get { return (string)GetValue(KeyTipProperty); }
			set { SetValue(KeyTipProperty, value); }
		}
		#endregion
		#region events
		#endregion
		public BackstageItem() {
		}
		protected virtual void RibbonStyleChaged(RibbonStyle ribbonStyle) {
		}
	}
}
