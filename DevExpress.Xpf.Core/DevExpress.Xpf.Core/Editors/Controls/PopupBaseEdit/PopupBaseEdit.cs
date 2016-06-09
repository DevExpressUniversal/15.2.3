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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Data.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	public enum PopupCloseMode {
		Normal,
		Cancel
	}
	public enum PopupFooterButtons {
		None,
		OkCancel,
		Close
	}
	public interface IPopupContentOwner {
		FrameworkElement Child { get; set; }
	}
	[DXToolboxBrowsable(false)]
	public class PopupBaseEdit : ButtonEdit, IPopupContentOwner {
		Locker CreatePopupLocker { get; set; }
		readonly VisualClientOwner visualClient;
		readonly PopupSettings popupSettings;
		#region static
		public static readonly DependencyProperty StaysPopupOpenProperty;
		public static readonly DependencyProperty IgnorePopupSizeConstraintsProperty;
		public static readonly DependencyProperty AllowRecreatePopupContentProperty;
		public static readonly DependencyProperty IsPopupOpenProperty;
		public static readonly DependencyProperty PopupTemplateProperty;
		public static readonly DependencyProperty PopupContentTemplateProperty;
		public static readonly DependencyProperty PopupContentContainerTemplateProperty;
		public static readonly DependencyProperty PopupTopAreaTemplateProperty;
		public static readonly DependencyProperty PopupBottomAreaTemplateProperty;
		public static readonly DependencyProperty PopupWidthProperty;
		protected static readonly DependencyPropertyKey ActualPopupWidthPropertyKey;
		public static readonly DependencyProperty ActualPopupWidthProperty;
		public static readonly DependencyProperty PopupMaxWidthProperty;
		public static readonly DependencyProperty PopupMinWidthProperty;
		protected static readonly DependencyPropertyKey ActualPopupMinWidthPropertyKey;
		public static readonly DependencyProperty ActualPopupMinWidthProperty;
		public static readonly DependencyProperty PopupHeightProperty;
		protected static readonly DependencyPropertyKey ActualPopupHeightPropertyKey;
		public static readonly DependencyProperty ActualPopupHeightProperty;
		public static readonly DependencyProperty PopupMaxHeightProperty;
		public static readonly DependencyProperty PopupMinHeightProperty;
		public static readonly DependencyProperty PopupFooterButtonsProperty;
		public static readonly DependencyProperty ShowSizeGripProperty;
		public static readonly RoutedEvent PopupOpeningEvent;
		public static readonly RoutedEvent PopupOpenedEvent;
		public static readonly RoutedEvent PopupClosingEvent;
		public static readonly RoutedEvent PopupClosedEvent;
		protected static readonly DependencyPropertyKey PopupOwnerEditPropertyKey;
		public static readonly DependencyProperty PopupOwnerEditProperty;
		public static readonly DependencyProperty FocusPopupOnOpenProperty;
		public static readonly DependencyProperty ClosePopupOnClickModeProperty;
		static PopupBaseEdit() {
			Type ownerType = typeof(PopupBaseEdit);
			StaysPopupOpenProperty = DependencyPropertyManager.Register("StaysPopupOpen", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			IgnorePopupSizeConstraintsProperty = DependencyPropertyManager.Register("IgnorePopupSizeConstraints", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			FocusPopupOnOpenProperty = DependencyPropertyManager.Register("FocusPopupOnOpen", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			AllowRecreatePopupContentProperty = DependencyPropertyManager.Register("AllowRecreatePopupContent", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			PopupTemplateProperty = DependencyPropertyManager.Register("PopupTemplate", typeof(ControlTemplate), ownerType);
			IsPopupOpenProperty = DependencyPropertyManager.Register("IsPopupOpen", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, OnIsPopupOpenChanged, OnIsPopupOpenCoerce));
			PopupContentTemplateProperty = DependencyPropertyManager.Register("PopupContentTemplate", typeof(ControlTemplate), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnPopupContentTemplateChanged));
			PopupContentContainerTemplateProperty = DependencyPropertyManager.Register("PopupContentContainerTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null));
			PopupTopAreaTemplateProperty = DependencyPropertyManager.Register("PopupTopAreaTemplate", typeof(ControlTemplate), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((PopupBaseEdit)d).PopupTopAreaTemplateChanged()));
			PopupBottomAreaTemplateProperty = DependencyPropertyManager.Register("PopupBottomAreaTemplate", typeof(ControlTemplate), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((PopupBaseEdit)d).PopupBottomAreaTemplateChanged()));
			PopupOpeningEvent = EventManager.RegisterRoutedEvent("PopupOpening", RoutingStrategy.Tunnel, typeof(OpenPopupEventHandler), ownerType);
			PopupOpenedEvent = EventManager.RegisterRoutedEvent("PopupOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), ownerType);
			PopupClosingEvent = EventManager.RegisterRoutedEvent("PopupClosing", RoutingStrategy.Tunnel, typeof(ClosePopupEventHandler), ownerType);
			PopupClosedEvent = EventManager.RegisterRoutedEvent("PopupClosed", RoutingStrategy.Bubble, typeof(ClosePopupEventHandler), ownerType);
			PopupMinWidthProperty = DependencyPropertyManager.Register("PopupMinWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(17d, OnPopupMinWidthChanged, CoercePopupMinWidth));
			ActualPopupMinWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPopupMinWidth", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0d, OnActualPopupMinWidthChanged));
			ActualPopupMinWidthProperty = ActualPopupMinWidthPropertyKey.DependencyProperty;
			PopupMaxWidthProperty = DependencyPropertyManager.Register("PopupMaxWidth", typeof(double), ownerType,
				new FrameworkPropertyMetadata(double.PositiveInfinity, OnPopupMaxWidthChanged, CoercePopupMaxWidth));
			PopupWidthProperty = DependencyPropertyManager.Register("PopupWidth", typeof(double), ownerType,
				new FrameworkPropertyMetadata(double.NaN, (d, e) => ((PopupBaseEdit)d).OnPopupWidthChanged((double)e.NewValue), CoercePopupWidth));
			ActualPopupWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPopupWidth", typeof(double), ownerType,
				new FrameworkPropertyMetadata(double.NaN, OnActualPopupWidthChanged, CoerceActualPopupWidth));
			ActualPopupWidthProperty = ActualPopupWidthPropertyKey.DependencyProperty;
			PopupMinHeightProperty = DependencyPropertyManager.Register("PopupMinHeight", typeof(double), ownerType, new FrameworkPropertyMetadata(35d, OnPopupMinHeightChanged, CoercePopupMinHeight));
			PopupMaxHeightProperty = DependencyPropertyManager.Register("PopupMaxHeight", typeof(double), ownerType,
				new FrameworkPropertyMetadata(double.PositiveInfinity, OnPopupMaxHeightChanged, CoercePopupMaxHeight));
			PopupHeightProperty = DependencyPropertyManager.Register("PopupHeight", typeof(double), ownerType,
				new FrameworkPropertyMetadata(double.NaN, (d, e) => ((PopupBaseEdit)d).OnPopupHeightChanged((double)e.NewValue), CoercePopupHeight));
			ActualPopupHeightPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPopupHeight", typeof(double), ownerType,
				new FrameworkPropertyMetadata(double.NaN, OnActualPopupHeightChanged, CoerceActualPopupHeight));
			ActualPopupHeightProperty = ActualPopupHeightPropertyKey.DependencyProperty;
			PopupFooterButtonsProperty = DependencyPropertyManager.Register("PopupFooterButtons", typeof(PopupFooterButtons?), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnPopupFooterButtonsChanged, CoercePopupFooterButtons));
			ShowSizeGripProperty = DependencyPropertyManager.Register("ShowSizeGrip", typeof(bool?), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((PopupBaseEdit)d).ShowSizeGripChanged((bool?)e.NewValue), CoerceShowSizeGrip));
			ClosePopupOnClickModeProperty = DependencyPropertyRegistrator.Register<PopupBaseEdit, PopupCloseMode?>(owner => owner.ClosePopupOnClickMode, null);
			PopupOwnerEditPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PopupOwnerEdit", ownerType, ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			PopupOwnerEditProperty = PopupOwnerEditPropertyKey.DependencyProperty;
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(typeof(PopupBaseEdit)));
		}
		internal static void SetPopupOwnerEdit(DependencyObject element, PopupBaseEdit value) {
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PopupOwnerEditPropertyKey, value);
		}
		public static PopupBaseEdit GetPopupOwnerEdit(DependencyObject element) {
			if (element == null)
				throw new ArgumentNullException("element");
			return (PopupBaseEdit)DependencyObjectHelper.GetValueWithInheritance(element, PopupOwnerEditProperty);
		}
		protected virtual void ClosePopupInternal(object parameter) {
			PopupCloseMode mode = (PopupCloseMode)parameter;
			ClosePopup(mode);
		}
		static object CoerceSize(double value, double minLen, double maxLen) {
			double result = double.NaN;
			if (!double.IsNaN(value)) {
				result = double.Epsilon;
				double newSize = CalcRestrictedValue(value, minLen, maxLen);
				if (newSize > 0d)
					result = newSize;
			}
			return result;
		}
		internal static double CalcRestrictedValue(double currentValue, double minValue, double maxValue) {
			double m1 = currentValue;
			if (!double.IsNaN(minValue))
				m1 = Math.Max(currentValue, minValue);
			double m2 = m1;
			if (!double.IsNaN(maxValue))
				m2 = Math.Min(m1, maxValue);
			return m2;
		}
		static object CoercePopupMinWidth(DependencyObject obj, object baseValue) {
			return ((PopupBaseEdit)obj).CoercePopupMinWidth((double)baseValue);
		}
		static object CoercePopupMaxWidth(DependencyObject obj, object baseValue) {
			return ((PopupBaseEdit)obj).CoercePopupMaxWidth((double)baseValue);
		}
		static object CoercePopupWidth(DependencyObject obj, object baseValue) {
			PopupBaseEdit pe = (PopupBaseEdit)obj;
			return CoerceSize((double)baseValue, pe.ActualPopupMinWidth, pe.PopupMaxWidth);
		}
		protected static object CoerceActualPopupWidth(DependencyObject obj, object baseValue) {
			return ((PopupBaseEdit)obj).CoerceActualPopupWidth((double)baseValue);
		}
		protected static object CoercePopupMinHeight(DependencyObject obj, object baseValue) {
			return ((PopupBaseEdit)obj).CoercePopupMinHeight((double)baseValue);
		}
		protected static object CoercePopupMaxHeight(DependencyObject obj, object baseValue) {
			return ((PopupBaseEdit)obj).CoercePopupMaxHeight((double)baseValue);
		}
		static object CoercePopupHeight(DependencyObject obj, object baseValue) {
			PopupBaseEdit pe = (PopupBaseEdit)obj;
			return CoerceSize((double)baseValue, pe.PopupMinHeight, pe.PopupMaxHeight);
		}
		protected static object CoerceActualPopupHeight(DependencyObject obj, object baseValue) {
			return ((PopupBaseEdit)obj).CoerceActualPopupHeight((double)baseValue);
		}
		protected static object CoercePopupFooterButtons(DependencyObject obj, object baseValue) {
			return ((PopupBaseEdit)obj).CoercePopupFooterButtons((PopupFooterButtons?)baseValue);
		}
		protected static object CoerceShowSizeGrip(DependencyObject obj, object baseValue) {
			return ((PopupBaseEdit)obj).CoerceShowSizeGrip((bool?)baseValue);
		}
		static void OnPopupContentTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupBaseEdit)obj).OnPopupContentTemplateChanged();
		}
		static void OnIsPopupOpenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupBaseEdit)obj).OnIsPopupOpenChanged();
		}
		static object OnIsPopupOpenCoerce(DependencyObject obj, object baseValue) {
			return ((PopupBaseEdit)obj).OnIsPopupOpenCoerce(baseValue);
		}
		static void OnPopupFooterButtonsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupBaseEdit)obj).OnPopupFooterButtonsChanged();
		}
		static void OnActualPopupWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupBaseEdit)obj).OnActualPopupWidthChanged();
		}
		static void OnActualPopupHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupBaseEdit)obj).OnActualPopupHeightChanged();
		}
		static void OnPopupMinWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupBaseEdit)obj).OnPopupMinWidthChanged();
		}
		static void OnActualPopupMinWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupBaseEdit)obj).OnActualPopupMinWidthChanged();
		}
		static void OnPopupMinHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupBaseEdit)obj).OnPopupMinHeightChanged();
		}
		static void OnPopupMaxWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupBaseEdit)obj).OnPopupMaxWidthChanged();
		}
		static void OnPopupMaxHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupBaseEdit)obj).OnPopupMaxHeightChanged();
		}
		#endregion
		public PopupBaseEdit() {
			SetPopupOwnerEdit(this, this);
			visualClient = CreateVisualClient();
			this.popupSettings = CreatePopupSettings();
			SizeChanged += OnSizeChanged;
			ClosePopupCommand = DelegateCommandFactory.Create<object>(ClosePopupInternal, false);
			OpenPopupCommand = DelegateCommandFactory.Create<object>(OpenPopupInternal, false);
			CreatePopupLocker = new Locker();
		}
		protected virtual VisualClientOwner CreateVisualClient() {
			return new DummyVisualClient(this);
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new PopupBaseEditPropertyProvider(this);
		}
		protected override void OnLoadedInternal() {
			base.OnLoadedInternal();
			if (IsPopupOpen)
				OpenPopupInternal(OpenPopupSyncFlagInternal);
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			PopupSettings.UpdateActualPopupMinWidth();
		}
		readonly Locker closingPopupLocker = new Locker();
		internal bool IsPopupCloseInProgress { get { return closingPopupLocker; } }
		protected internal override Type StyleSettingsType { get { return typeof(PopupBaseEditStyleSettings); } }
		protected new PopupBaseEditPropertyProvider PropertyProvider { get { return (PopupBaseEditPropertyProvider)base.PropertyProvider; } }
		protected PopupCloseMode PopupCloseMode { get { return this.popupSettings.PopupCloseMode; } }
		bool CreatedFromSettings { get { return !AllowRecreatePopupContent && PropertyProvider.CreatedFromSettings; } }
		protected internal IPopupContentOwner PopupContentOwner { get { return CreatedFromSettings ? Settings : (IPopupContentOwner)this; } }
		protected internal VisualClientOwner VisualClient { get { return visualClient; } }
		public PopupCloseMode? ClosePopupOnClickMode {
			get { return (PopupCloseMode?)GetValue(ClosePopupOnClickModeProperty); }
			set { SetValue(ClosePopupOnClickModeProperty, value); }
		}
		public bool? IgnorePopupSizeConstraints {
			get { return (bool?)GetValue(IgnorePopupSizeConstraintsProperty); }
			set { SetValue(IgnorePopupSizeConstraintsProperty, value); }
		}
		public bool? StaysPopupOpen {
			get { return (bool?)GetValue(StaysPopupOpenProperty); }
			set { SetValue(StaysPopupOpenProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupBaseEditAllowRecreatePopupContent")]
#endif
		public bool AllowRecreatePopupContent {
			get { return (bool)GetValue(AllowRecreatePopupContentProperty); }
			set { SetValue(AllowRecreatePopupContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditPopupWidth"),
#endif
 Category("Appearance")]
		public double PopupWidth {
			get { return (double)GetValue(PopupWidthProperty); }
			set { SetValue(PopupWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditPopupHeight"),
#endif
 Category("Appearance")]
		public double PopupHeight {
			get { return (double)GetValue(PopupHeightProperty); }
			set { SetValue(PopupHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditActualPopupWidth"),
#endif
 Category("Appearance")]
		public double ActualPopupWidth {
			get { return (double)GetValue(ActualPopupWidthProperty); }
			protected internal set { this.SetValue(ActualPopupWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditActualPopupHeight"),
#endif
 Category("Appearance")]
		public double ActualPopupHeight {
			get { return (double)GetValue(ActualPopupHeightProperty); }
			protected internal set { this.SetValue(ActualPopupHeightPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditPopupMaxWidth"),
#endif
 Category("Appearance")]
		public double PopupMaxWidth {
			get { return (double)GetValue(PopupMaxWidthProperty); }
			set { SetValue(PopupMaxWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditPopupMinWidth"),
#endif
 Category("Appearance")]
		public double PopupMinWidth {
			get { return (double)GetValue(PopupMinWidthProperty); }
			set { SetValue(PopupMinWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditActualPopupMinWidth"),
#endif
 Browsable(false)]
		public double ActualPopupMinWidth {
			get { return (double)GetValue(ActualPopupMinWidthProperty); }
			protected internal set { this.SetValue(ActualPopupMinWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditPopupMaxHeight"),
#endif
 Category("Appearance")]
		public double PopupMaxHeight {
			get { return (double)GetValue(PopupMaxHeightProperty); }
			set { SetValue(PopupMaxHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditPopupMinHeight"),
#endif
 Category("Appearance")]
		public double PopupMinHeight {
			get { return (double)GetValue(PopupMinHeightProperty); }
			set { SetValue(PopupMinHeightProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public ICommand ClosePopupCommand { get; private set; }
		[Category(EditSettingsCategories.Behavior)]
		public ICommand OpenPopupCommand { get; private set; }
		public bool? FocusPopupOnOpen {
			get { return (bool?)GetValue(FocusPopupOnOpenProperty); }
			set { SetValue(FocusPopupOnOpenProperty, value); }
		}
		protected internal PopupSettings PopupSettings { get { return this.popupSettings; } }
		protected internal virtual FrameworkElement PopupRoot { get { return Popup.With(x => x.Child as FrameworkElement); } }
		protected internal virtual FrameworkElement PopupElement { get { return null; } }
		internal FrameworkElement PopupFocusElement { get { return PopupElement ?? (PopupContentControl)PopupContentOwner.Child; } }
		void FocusPopup() {
			if (PopupFocusElement == null)
				return;
			if (PropertyProvider.FocusPopupOnOpen) {
				FocusHelper.SetFocusable(PopupFocusElement, PropertyProvider.FocusPopupOnOpen);
				PopupFocusElement.Focus();
			}
		}
		double CoercePopupMinWidth(double value) {
			return Math.Max(value, 0d);
		}
		double CoercePopupMaxWidth(double value) {
			return Math.Max(value, 0d);
		}
		double CoerceActualPopupWidth(double value) {
			return Math.Max(value, 0d);
		}
		double CoercePopupMinHeight(double value) {
			return Math.Max(value, 0d);
		}
		double CoercePopupMaxHeight(double value) {
			return Math.Max(value, 0d);
		}
		double CoerceActualPopupHeight(double value) {
			return Math.Max(value, 0d);
		}
		protected virtual PopupFooterButtons? CoercePopupFooterButtons(PopupFooterButtons? buttons) {
			return buttons;
		}
		protected virtual bool? CoerceShowSizeGrip(bool? show) {
			return show;
		}
		protected void UpdatePopupWidth() {
			CoerceValue(PopupWidthProperty);
		}
		protected void UpdatePopupHeight() {
			CoerceValue(PopupHeightProperty);
		}
		protected virtual object OnIsPopupOpenCoerce(object baseValue) {
			if (IsPopupOpen)
				baseValue = CoercePopupClosing(baseValue);
			return ShouldProcessIsPopupOpenedChanging((bool)baseValue) ? baseValue : IsPopupOpen;
		}
		object CoercePopupClosing(object baseValue) {
			if (PropertyProvider.StaysPopupOpen)
				return true;
			return baseValue;
		}
		bool ShouldProcessIsPopupOpenedChanging(bool isOpening) {
			return isOpening ? RaisePopupOpening() && CanShowPopup : RaisePopupClosing(PopupCloseMode, EditValue);
		}
		protected virtual void OnPopupFooterButtonsChanged() {
			UpdatePopupButtons();
		}
		protected virtual void ShowSizeGripChanged(bool? value) {
			UpdatePopupElements();
		}
		void UpdateSettingsPopupSize(DependencyProperty property, double value) {
			if (Settings.IsSharedPopupSize)
				Settings.SetValue(property, value);
		}
		protected virtual void OnPopupWidthChanged(double value) {
			UpdateSettingsPopupSize(PopupBaseEditSettings.PopupWidthProperty, value);
			popupSettings.UpdateActualPopupWidth(value);
		}
		protected virtual void OnPopupHeightChanged(double height) {
			UpdateSettingsPopupSize(PopupBaseEditSettings.PopupHeightProperty, height);
			popupSettings.UpdateActualPopupHeight(height);
		}
		protected virtual void OnPopupMinWidthChanged() {
			UpdateSettingsPopupSize(PopupBaseEditSettings.PopupMinWidthProperty, PopupMinWidth);
			PopupSettings.UpdateActualPopupMinWidth();
		}
		protected virtual void OnActualPopupMinWidthChanged() {
			popupSettings.SetMinWidthToPopup();
			UpdatePopupWidth();
		}
		protected virtual void OnActualPopupWidthChanged() {
			PopupSettings.SetActualWidthToPopup();
		}
		protected virtual void OnActualPopupHeightChanged() {
			PopupSettings.SetActualHeightToPopup();
		}
		protected virtual void OnPopupMaxWidthChanged() {
			popupSettings.SetMaxWidthToPopup();
			UpdatePopupWidth();
			UpdateSettingsPopupSize(PopupBaseEditSettings.PopupMaxWidthProperty, PopupMaxWidth);
		}
		protected virtual void OnPopupMinHeightChanged() {
			popupSettings.SetMinHeightToPopup();
			UpdatePopupHeight();
			UpdateSettingsPopupSize(PopupBaseEditSettings.PopupMinHeightProperty, PopupMinHeight);
		}
		protected virtual void OnPopupMaxHeightChanged() {
			popupSettings.SetMaxHeightToPopup();
			UpdatePopupHeight();
			UpdateSettingsPopupSize(PopupBaseEditSettings.PopupMaxHeightProperty, PopupMaxHeight);
		}
		protected virtual void OnIsPopupOpenChanged() {
			if (IsLoaded)
				ProcessIsPopupOpenChanged();
		}
		void ProcessIsPopupOpenChanged() {
			if (IsPopupOpen)
				OpenPopupInternal(OpenPopupSyncFlagInternal);
			else
				ClosePopupInternal();
		}
		protected virtual void OnPopupContentChanged() {
		}
		protected override void NullValueButtonPlacementChanged(EditorPlacement? newValue) {
			UpdatePopupElements();
		}
		protected internal virtual void UpdatePopupTemplates() {
			if (IsInSupportInitializing || !IsPopupOpen)
				return;
			PropertyProvider.UpdatePopupTemplates();
		}
		protected internal virtual void UpdatePopupButtons() {
			if (IsInSupportInitializing)
				return;
			UpdateButtonInfoCollections();
			if (!IsPopupOpen)
				return;
			PropertyProvider.PopupViewModel.Update();
		}
		static bool openPopupSyncFlag;
#if DEBUGTEST
		public static bool OpenPopupSyncFlag {
			get { return OpenPopupSyncFlagInternal; }
			set { OpenPopupSyncFlagInternal = value; }
		}
#endif
		internal static bool OpenPopupSyncFlagInternal {
			get {
#if DEBUGTEST
				return openPopupSyncFlag;
#else 
				return false;
#endif
			}
			set { openPopupSyncFlag = value; }
		}
		void OpenPopupInternal(object parameter) {
			ShowPopup();
		}
		void OpenPopupInternal(bool sync) {
			if (sync)
				OpenPopupInternal();
			else
				Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(OpenPopupInternal));
		}
		internal void OpenPopupInternal() {
			if (!IsPopupOpen)
				return;
			CreatePopupLocker.DoLockedAction(OpenPopupInternalImpl);
		}
		void OpenPopupInternalImpl() {
			popupSettings.OpenPopup();
			OnPopupOpened();
			RaisePopupOpened();
		}
		protected internal virtual void FindPopupContent() {
			VisualClient.PopupContentLoaded();
		}
		internal void ClosePopupInternal() {
			OnPopupClosed();
			PopupSettings.ClosePopup();
			RaisePopupClosed(this.popupSettings.PopupCloseMode, EditValue);
			PopupBaseEditHelper.SetIsValueChangedViaPopup(Settings, false);
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new PopupBaseEditAutomationPeer(this);
		}
		protected internal override bool IsChildElement(DependencyObject element, DependencyObject root = null) {
			return base.IsChildElement(element, root) ||
				   (Popup != null && Popup.Child != null && LayoutHelper.IsChildElementEx(Popup.Child, element));
		}
		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			MakeFocused();
			if (e.ChangedButton != MouseButton.Left)
				return;
			if (!PropertyProvider.IsTextEditable && CanShowByClicking) {
				ShowPopup();
				e.Handled = IsPopupOpen;
			}
		}
		protected virtual void OnPopupOpened() {
			OnPopupOpenedInternal();
			VisualClient.PopupOpened();
			PopupBaseEditHelper.SetIsValueChangedViaPopup(Settings, false);
		}
		protected virtual void OnPopupOpenedInternal() {
			if (Popup != null && ShouldCapturePopup())
				Mouse.Capture(Popup.Child, CaptureMode.SubTree);
			if (WindowClosingHandler == null)
				WindowClosingHandler = new WindowClosingEventHandler<PopupBaseEdit>(this, (editor, sender, eventArgs) => editor.CancelPopup());
			Window hostWindow = LayoutHelper.GetTopLevelVisual(this) as Window;
			if (hostWindow != null)
				hostWindow.Closing += WindowClosingHandler.Handler;
			FocusPopup();
		}
		WindowClosingEventHandler<PopupBaseEdit> WindowClosingHandler { get; set; }
		IInputElement capturedSource;
		protected IInputElement CapturedSource {
			get { return capturedSource; }
			set {
				if (CapturedSource == value)
					return;
				if (CapturedSource != null)
					CapturedSource.LostMouseCapture -= OnSourceLostMouseCapture;
				capturedSource = value;
				if (CapturedSource != null)
					CapturedSource.LostMouseCapture += OnSourceLostMouseCapture;
			}
		}
		bool ShouldCapturePopup() {
			PopupBaseEditStyleSettings settings = (PopupBaseEditStyleSettings)PropertyProvider.StyleSettings;
			return settings.ShouldCaptureMouseOnPopup;
		}
		void OnSourceLostMouseCapture(object sender, MouseEventArgs e) {
			CapturedSource = null;
			if (!IsPopupOpen || Popup == null || !ShouldCapturePopup())
				return;
			if (Popup.Child == Mouse.Captured || GetIsParentPopupClosed())
				return;
			if (Mouse.Captured == null) {
				Mouse.Capture(Popup.Child, CaptureMode.SubTree);
				e.Handled = true;
			}
			else
				CapturedSource = Mouse.Captured;
		}
		bool GetIsParentPopupClosed() {
			Popup popup = LayoutHelper.FindLayoutOrVisualParentObject<Popup>(this, true);
			if (popup != null && !popup.IsOpen)
				return true;
			return false;
		}
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			base.OnLostMouseCapture(e);
			if (!IsPopupOpen || Popup == null || !ShouldCapturePopup())
				return;
			CapturedSource = Mouse.Captured;
		}
		protected internal virtual void BeforePopupOpened() {
			if (!IsLoaded)
				return;
			UpdatePopupTemplates();
			UpdatePopupElements();
			VisualClient.BeforePopupOpened();
		}
		protected virtual void UnCaptureChild() {
			CapturedSource = null;
			if (Popup != null && Mouse.Captured == Popup.Child)
				Mouse.Capture(null);
		}
		protected virtual void OnPopupClosed() {
			UnCaptureChild();
			if (PopupCloseMode != PopupCloseMode.Cancel)
				AcceptPopupValue();
			if (IsEditorKeyboardFocused)
				Focus();
		}
		protected virtual void AcceptPopupValue() {
			PopupBaseEditHelper.SetIsValueChangedViaPopup(Settings, true);
			PopupSettings.AcceptPopupValue();
		}
		protected internal new PopupBaseEditSettings Settings {
			get { return (PopupBaseEditSettings)base.Settings; }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupBaseEditPopupContentTemplate")]
#endif
		public ControlTemplate PopupContentTemplate {
			get { return (ControlTemplate)GetValue(PopupContentTemplateProperty); }
			set { SetValue(PopupContentTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditPopupContentContainerTemplate"),
#endif
 Browsable(false)]
		public ControlTemplate PopupContentContainerTemplate {
			get { return (ControlTemplate)GetValue(PopupContentContainerTemplateProperty); }
			set { SetValue(PopupContentContainerTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditPopupTopAreaTemplate"),
#endif
 Browsable(false)]
		public ControlTemplate PopupTopAreaTemplate {
			get { return (ControlTemplate)GetValue(PopupTopAreaTemplateProperty); }
			set { SetValue(PopupTopAreaTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditPopupBottomAreaTemplate"),
#endif
 Browsable(false)]
		public ControlTemplate PopupBottomAreaTemplate {
			get { return (ControlTemplate)GetValue(PopupBottomAreaTemplateProperty); }
			set { SetValue(PopupBottomAreaTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditPopupFooterButtons"),
#endif
 Category("Behavior")]
		public PopupFooterButtons? PopupFooterButtons {
			get { return (PopupFooterButtons?)GetValue(PopupFooterButtonsProperty); }
			set { SetValue(PopupFooterButtonsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditShowSizeGrip"),
#endif
 Category("Behavior")]
		[TypeConverter(typeof(NullableBoolConverter))]
		public bool? ShowSizeGrip {
			get { return (bool?)GetValue(ShowSizeGripProperty); }
			set { SetValue(ShowSizeGripProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditPopupTemplate"),
#endif
 Browsable(false)]
		public ControlTemplate PopupTemplate {
			get { return (ControlTemplate)GetValue(PopupTemplateProperty); }
			set { SetValue(PopupTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("PopupBaseEditIsPopupOpen"),
#endif
 Category("Common Properties")]
		public bool IsPopupOpen {
			get { return (bool)GetValue(IsPopupOpenProperty); }
			set { SetValue(IsPopupOpenProperty, value); }
		}
		protected override bool IsEditorKeyboardFocused {
			get {
				return base.IsEditorKeyboardFocused || (Popup != null && Popup.Child != null && FocusHelper.IsKeyboardFocusWithin(Popup))
					   || (Keyboard.FocusedElement as FrameworkElement)
						   .With<FrameworkElement, ContextMenu>(LayoutHelper.FindParentObject<ContextMenu>)
						   .With(x => x.PlacementTarget).With(GetPopupOwnerEdit) == this
					   || (Keyboard.FocusedElement as FrameworkElement)
						   .With(x => BarManagerHelper.GetPopup(x))
						   .With(x => x.PlacementTarget).With(GetPopupOwnerEdit) == this
					;
			}
		}
		protected override bool HandlesScrolling {
			get { return false; }
		}
		bool closePopupOnLostFocus = true;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ClosePopupOnLostFocus {
			get { return closePopupOnLostFocus; }
			set { closePopupOnLostFocus = value; }
		}
		protected internal virtual void OnPopupIsKeyboardFocusWithinChanged(EditorPopupBase popupBase) {
			if (!IsEditorKeyboardFocused && ClosePopupOnLostFocus)
				CancelPopup();
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			if (CreatePopupLocker.IsLocked)
				return;
			if (!IsEditorKeyboardFocused && ClosePopupOnLostFocus)
				CancelPopup();
		}
		[ Browsable(false)]
		protected internal virtual bool CanShowPopup {
			get { return IsEditingMode() && this.IsInVisualTree(); }
		}
		public void ShowPopup() {
			IsPopupOpen = true;
		}
		protected void ClosePopup(PopupCloseMode closeMode) {
			if (IsPopupOpen && !PropertyProvider.StaysPopupOpen) {
				this.popupSettings.SetPopupCloseMode(closeMode);
				closingPopupLocker.DoLockedAction(() => IsPopupOpen = false);
			}
		}
		public void ClosePopup() {
			ClosePopup(PopupCloseMode.Normal);
		}
		public void CancelPopup() {
			ClosePopup(PopupCloseMode.Cancel);
		}
		protected virtual void RaisePopupOpened() {
			this.RaiseEvent(new RoutedEventArgs(PopupOpenedEvent));
		}
		protected virtual void RaisePopupClosed(PopupCloseMode mode, object value) {
			this.RaiseEvent(new ClosePopupEventArgs(PopupClosedEvent, mode, value));
		}
		protected virtual bool RaisePopupOpening() {
			OpenPopupEventArgs args = new OpenPopupEventArgs(PopupOpeningEvent);
			this.RaiseEvent(args);
			return !args.Cancel;
		}
		protected virtual bool RaisePopupClosing(PopupCloseMode mode, object value) {
			this.RaiseEvent(new ClosePopupEventArgs(PopupClosingEvent, mode, value));
			return true;
		}
		public event RoutedEventHandler PopupOpened {
			add { this.AddHandler(PopupOpenedEvent, value); }
			remove { this.RemoveHandler(PopupOpenedEvent, value); }
		}
		public event OpenPopupEventHandler PopupOpening {
			add { this.AddHandler(PopupOpeningEvent, value); }
			remove { this.RemoveHandler(PopupOpeningEvent, value); }
		}
		public event ClosePopupEventHandler PopupClosed {
			add { this.AddHandler(PopupClosedEvent, value); }
			remove { this.RemoveHandler(PopupClosedEvent, value); }
		}
		public event ClosePopupEventHandler PopupClosing {
			add { this.AddHandler(PopupClosingEvent, value); }
			remove { this.RemoveHandler(PopupClosingEvent, value); }
		}
		protected virtual void OnPopupContentTemplateChanged() {
		}
		protected virtual void PopupTopAreaTemplateChanged() {
			UpdatePopupTemplates();
		}
		protected virtual void PopupBottomAreaTemplateChanged() {
			UpdatePopupTemplates();
		}
		protected internal EditorPopupBase Popup { get { return this.popupSettings.Popup; } }
		protected internal virtual bool ProcessVisualClientKeyDown(KeyEventArgs e) {
			return VisualClient.ProcessKeyDown(e);
		}
		protected internal virtual bool ShouldApplyPopupSize { get { return true; } }
		protected internal virtual void DestroyPopupContent(EditorPopupBase popup) {
		}
		protected virtual PopupSettings CreatePopupSettings() {
			return new PopupSettings(this);
		}
		protected internal override void OnDefaultButtonClick(object sender, RoutedEventArgs e) {
			base.OnDefaultButtonClick(sender, e);
			MakeFocused();
			if (CanShowByClicking)
				IsPopupOpen = !IsPopupOpen;
		}
		bool CanShowByClicking {
			get { return IsEnabled; }
		}
		void MakeFocused() {
			BaseEditingSettingsService tes = PropertyProvider.GetService<BaseEditingSettingsService>();
			if (!tes.AllowKeyHandling)
				return;
			Focus();
		}
		protected internal void UpdatePopupElements() {
			UpdatePopupButtons();
			UpdateSizeGrip();
		}
		protected internal void UpdateSizeGrip() {
			if (!IsPopupOpen)
				return;
			PropertyProvider.ResizeGripViewModel.Update();
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			BeforePreviewKeyDown(e);
			base.OnPreviewKeyDown(e);
			AfterPreviewKeyDown(e);
		}
		protected virtual void BeforePreviewKeyDown(KeyEventArgs e) {
		}
		protected virtual void AfterPreviewKeyDown(KeyEventArgs e) {
			ProcessPopupKeyDown(e);
		}
		protected virtual bool IsTogglePopupOpenGesture(Key key, ModifierKeys modifiers) {
			return Settings.IsTogglePopupOpenGesture(key, modifiers);
		}
		protected internal virtual bool ProcessPopupKeyDown(KeyEventArgs e) {
			if (e.Handled)
				return true;
			ModifierKeys mod = ModifierKeysHelper.GetKeyboardModifiers(e);
			Key key = BaseEditHelper.GetKey(e);
			if (IsClosePopupWithCancelGesture(key, mod)) {
				if (IsPopupOpen) {
					ClosePopup(PopupCloseMode.Cancel);
					return e.Handled = true;
				}
			}
			if (IsClosePopupWithAcceptGesture(key, mod)) {
				if (IsPopupOpen) {
					ClosePopup(PopupCloseMode.Normal);
					return e.Handled = true;
				}
			}
			if (IsTogglePopupOpenGesture(key, mod)) {
				if (IsPopupOpen)
					ClosePopup(PopupCloseMode.Cancel);
				else
					IsPopupOpen = true;
				return e.Handled = true;
			}
			return false;
		}
		protected internal virtual void ClosePopupOnClick() {
			PopupCloseMode mode = PropertyProvider.GetClosePopupOnClickMode();
			ClosePopup(mode);
		}
		protected internal virtual bool LeavePopupGesture(KeyEventArgs e) {
			ModifierKeys mod = ModifierKeysHelper.GetKeyboardModifiers(e);
			Key key = BaseEditHelper.GetKey(e);
			return IsPopupOpen && IsClosePopupWithCancelGesture(key, mod);
		}
		protected virtual bool IsClosePopupWithCancelGesture(Key key, ModifierKeys modifiers) {
			return key == Key.Escape && ModifierKeysHelper.NoModifiers(modifiers) || key == Key.Apps;
		}
		protected virtual bool IsClosePopupWithAcceptGesture(Key key, ModifierKeys modifiers) {
			return false;
		}
		protected override void ProcessActivatingKeyCore(Key key, ModifierKeys modifiers) {
			if (IsTogglePopupOpenGesture(key, modifiers)) {
				ShowPopup();
				return;
			}
			base.ProcessActivatingKeyCore(key, modifiers);
		}
		protected internal override bool NeedsKey(Key key, ModifierKeys modifiers) {
			if (IsTogglePopupOpenGesture(key, modifiers))
				return true;
			return IsPopupOpen || base.NeedsKey(key, modifiers);
		}
		#region IPopupOwner Members
		protected override IEnumerator LogicalChildren {
			get {
				var list = new List<object>();
				IEnumerator enumerator = base.LogicalChildren;
				if (enumerator != null) {
					while (enumerator.MoveNext())
						list.Add(enumerator.Current);
				}
				if (child != null)
					list.Add(child);
				return list.GetEnumerator();
			}
		}
		FrameworkElement child;
		FrameworkElement IPopupContentOwner.Child {
			get { return child; }
			set {
				if (value == child)
					return;
				RemoveLogicalChild(child);
				child = value;
				AddLogicalChild(child);
			}
		}
		#endregion
		protected internal override MaskType[] GetSupportedMaskTypes() {
			return null;
		}
	}
	public class DummyVisualClient : VisualClientOwner {
		public DummyVisualClient(PopupBaseEdit editor)
			: base(editor) {
		}
		protected override FrameworkElement FindEditor() {
			return null;
		}
		protected override void SetupEditor() {
		}
		public override void SyncProperties(bool syncDataSource) {
		}
		public override bool ProcessKeyDownInternal(KeyEventArgs e) {
			return true;
		}
		public override object GetSelectedItem() {
			return null;
		}
		public override IEnumerable GetSelectedItems() {
			return null;
		}
	}
	public class WindowClosingEventHandler<TOwner> : WeakEventHandler<TOwner, EventArgs, CancelEventHandler> where TOwner : class {
		static readonly Action<WeakEventHandler<TOwner, EventArgs, CancelEventHandler>, object> detachAction = (h, o) => ((Window)o).Closing -= h.Handler;
		static readonly Func<WeakEventHandler<TOwner, EventArgs, CancelEventHandler>, CancelEventHandler> create = h => h.OnEvent;
		public WindowClosingEventHandler(TOwner owner, Action<TOwner, object, EventArgs> onEventAction)
			: base(owner, onEventAction, detachAction, create) {
		}
	}
}
namespace DevExpress.Xpf.Editors.Helpers {
	public static class PopupBaseEditHelper {
		public static bool GetIsValueChangedViaPopup(BaseEditSettings edit) {
			return (edit as PopupBaseEditSettings).Return(x => x.IsValueChangedViaPopup, () => false);
		}
		public static void SetIsValueChangedViaPopup(PopupBaseEditSettings edit, bool value) {
			if (edit == null)
				return;
			edit.IsValueChangedViaPopup = value;
		}
		public static EditorPopupBase GetPopup(this PopupBaseEdit edit) {
			return edit.Popup;
		}
		public static Button GetCloseButton(this PopupBaseEdit edit) {
			return FindButton(edit, "PART_CloseButton");
		}
		public static Button GetCancelButton(this PopupBaseEdit edit) {
			return FindButton(edit, "PART_CancelButton");
		}
		public static Button GetOkButton(this PopupBaseEdit edit) {
			return FindButton(edit, "PART_OkButton");
		}
		public static PopupSizeGrip GetSizeGrip(this PopupBaseEdit edit) {
			return (PopupSizeGrip)LayoutHelper.FindElement((FrameworkElement)edit.Popup.Child, e => e is PopupSizeGrip);
		}
		public static ContentControl GetFooter(this PopupBaseEdit edit) {
			return (ContentControl)LayoutHelper.FindElement((FrameworkElement)edit.Popup.Child, e => e is ContentControl && e.Name == "PART_Footer");
		}
		public static Border GetFooterLayer(this PopupBaseEdit edit) {
			return (Border)LayoutHelper.FindElement((FrameworkElement)edit.Popup.Child, e => e is Border && e.Name == "PART_FooterLayer");
		}
		public static ContentControl GetTop(this PopupBaseEdit edit) {
			return (ContentControl)LayoutHelper.FindElement((FrameworkElement)edit.Popup.Child, e => e is ContentControl && e.Name == "PART_Top");
		}
		static Button FindButton(PopupBaseEdit edit, string name) {
			return (Button)LayoutHelper.FindElement((FrameworkElement)edit.Popup.Child, e => e is Button && e.Name == name);
		}
	}
}
