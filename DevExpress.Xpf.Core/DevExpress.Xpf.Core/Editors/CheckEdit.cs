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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
#if !SL
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Automation;
#endif
#if SL
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Editors.Automation;
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using ToggleButton = DevExpress.Xpf.Editors.WPFCompatibility.SLToggleButton;
using UpdateSourceTrigger = DevExpress.Xpf.Core.WPFCompatibility.SLUpdateSourceTrigger;
#endif
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	[ContentProperty("Content")]
	public partial class CheckEdit : BaseEdit, IBooleanExportSettings, ICommandSource {
		#region static
		public static readonly DependencyProperty IsCheckedProperty;
		public static readonly DependencyProperty IsThreeStateProperty;
		public static readonly DependencyProperty ClickModeProperty;
		public static readonly DependencyProperty HasContentProperty;
		static readonly DependencyPropertyKey HasContentPropertyKey;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CommandTargetProperty;
#if !SL
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
#endif
		public static readonly RoutedEvent CheckedEvent;
		public static readonly RoutedEvent UncheckedEvent;
		public static readonly RoutedEvent IndeterminateEvent;
		static CheckEdit() {
			Type ownerType = typeof(CheckEdit);
			CommandProperty = DependencyPropertyManager.Register("Command", typeof(ICommand), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((CheckEdit)d).CommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue)));
			CommandParameterProperty = DependencyPropertyManager.Register("CommandParameter", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((CheckEdit)d).CommandParameterChanged(e.NewValue)));
			CommandTargetProperty = DependencyPropertyManager.Register("CommandTarget", typeof(IInputElement), ownerType, new FrameworkPropertyMetadata(null));
			IsCheckedProperty = ToggleButton.IsCheckedProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsCheckedChanged, CoerceIsCheckedProperty, true, UpdateSourceTrigger.PropertyChanged));
			IsThreeStateProperty = ToggleButton.IsThreeStateProperty.AddOwner(ownerType);
			ContentProperty = ToggleButton.ContentProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((CheckEdit)d).ContentChanged(e)));
			ContentTemplateProperty = ToggleButton.ContentTemplateProperty.AddOwner(ownerType);
			ClickModeProperty = ToggleButton.ClickModeProperty.AddOwner(ownerType);
			HasContentPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasContent", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			HasContentProperty = HasContentPropertyKey.DependencyProperty;
			CheckedEvent = EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), ownerType);
			UncheckedEvent = EventManager.RegisterRoutedEvent("Unchecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), ownerType);
			IndeterminateEvent = EventManager.RegisterRoutedEvent("Indeterminate", RoutingStrategy.Bubble, typeof(RoutedEventHandler), ownerType);
#if !SL
			EditValueProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, null, true, UpdateSourceTrigger.PropertyChanged));
			ContentTemplateSelectorProperty = ToggleButton.ContentTemplateSelectorProperty.AddOwner(ownerType);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			VerticalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(VerticalAlignment.Center));
#endif
		}
		protected static object CoerceIsCheckedProperty(DependencyObject obj, object value) {
			return ((CheckEdit)obj).CoerceIsChecked((bool?)value);
		}
		static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CheckEdit)d).OnIsCheckedChanged((bool?)e.OldValue, (bool?)e.NewValue);
		}
		#endregion
		#region constructers
		public CheckEdit() {
#if SL
			ConstructorSLPart();
#endif
		}
		#endregion
		#region properties
		new CheckEditStrategy EditStrategy { get { return (CheckEditStrategy)base.EditStrategy; } }
		protected internal new CheckEditSettings Settings { get { return (CheckEditSettings)base.Settings; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CheckEditHasContent")]
#endif
		protected override bool IsEnabledCore { get { return base.IsEnabledCore && EditStrategy.IsEnabledCore; } }
		public bool HasContent {
			get { return (bool)GetValue(HasContentProperty); }
			private set { this.SetValue(HasContentPropertyKey, value); }
		}
		[Category("Action")]
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		[Category("Action")]
		public object CommandParameter {
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		[Category("Action")]
		public IInputElement CommandTarget {
			get { return (IInputElement)GetValue(CheckEdit.CommandTargetProperty); }
			set { SetValue(CheckEdit.CommandTargetProperty, (object)value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("CheckEditIsChecked"),
#endif
		TypeConverter(typeof(NullableBoolConverter)), Category("Common Properties")]
		public bool? IsChecked {
			get { return (bool?)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("CheckEditIsThreeState"),
#endif
 Category("Behavior")]
		public bool IsThreeState {
			get { return (bool)GetValue(IsThreeStateProperty); }
			set { SetValue(IsThreeStateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("CheckEditClickMode"),
#endif
 Category("Behavior")]
		public ClickMode ClickMode {
			get { return (ClickMode)GetValue(ClickModeProperty); }
			set { SetValue(ClickModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("CheckEditContent"),
#endif
		TypeConverter(typeof(ObjectConverter)), Category("Content")]
		public object Content {
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("CheckEditContentTemplate"),
#endif
 Browsable(false)]
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
#if !SL
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("CheckEditContentTemplateSelector"),
#endif
 Browsable(false)]
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
#endif
		#endregion
		#region events
		[Category("Behavior")]
		public event RoutedEventHandler Checked {
			add { this.AddHandler(CheckedEvent, value); }
			remove { this.RemoveHandler(CheckedEvent, value); }
		}
		[Category("Behavior")]
		public event RoutedEventHandler Indeterminate {
			add { this.AddHandler(IndeterminateEvent, value); }
			remove { this.RemoveHandler(IndeterminateEvent, value); }
		}
		[Category("Behavior")]
		public event RoutedEventHandler Unchecked {
			add { this.AddHandler(UncheckedEvent, value); }
			remove { this.RemoveHandler(UncheckedEvent, value); }
		}
		#endregion
		#region methods
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new CheckEditAutomationPeer(this);
		}
		bool? GetCheckedState(object editValue) {
			if (editValue == null)
				return null;
			if (editValue is bool) {
				return (bool)editValue;
			}
			return false;
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if (e.Property == ControlHelper.IsFocusedProperty)
				UpdateEditCoreIsFocused();
#if SL
			OnPropertyChangedSLPart(e);
#endif
		}
		protected override void SubscribeEditEventsCore() {
			base.SubscribeEditEventsCore();
#if !SL
			EditCore.GotKeyboardFocus += OnEditCoreGotKeyboardFocus;
			EditCore.LostKeyboardFocus += OnEditCoreLostKeyboardFocus;
#else
			EditCore.GotFocus += OnEditCoreGotKeyboardFocus;
			EditCore.LostFocus += OnEditCoreLostKeyboardFocus;
#endif
			UpdateEditCoreIsFocused();
		}
		protected override void UnsubscribeEditEventsCore() {
#if !SL
			EditCore.GotKeyboardFocus -= OnEditCoreGotKeyboardFocus;
			EditCore.LostKeyboardFocus -= OnEditCoreLostKeyboardFocus;
#else
			EditCore.GotFocus -= OnEditCoreGotKeyboardFocus;
			EditCore.LostFocus -= OnEditCoreLostKeyboardFocus;
#endif
			base.UnsubscribeEditEventsCore();
		}
		void OnEditCoreLostKeyboardFocus(object sender, System.Windows.RoutedEventArgs e) {
			UpdateEditCoreIsFocused();
		}
		void OnEditCoreGotKeyboardFocus(object sender, System.Windows.RoutedEventArgs e) {
			UpdateEditCoreIsFocused();
		}
		void UpdateEditCoreIsFocused() {
			if (EditCore == null)
				return;
			if (GetBindingExpression(ControlHelper.IsFocusedProperty) == null)
				ControlHelper.SetShowFocusedState(EditCore, FocusHelper.IsKeyboardFocused(EditCore));
			else
				ControlHelper.SetShowFocusedState(EditCore, ControlHelper.GetIsFocused(this) || FocusHelper.IsKeyboardFocused(EditCore));
		}
		protected internal ToggleButton CheckBox { get { return EditCore as ToggleButton; } }
		protected internal virtual void OnChecked(RoutedEventArgs e) {
			this.RaiseEvent(e);
		}
		protected internal virtual void OnUnchecked(RoutedEventArgs e) {
			this.RaiseEvent(e);
		}
		protected internal virtual void OnIndeterminate(RoutedEventArgs e) {
			this.RaiseEvent(e);
		}
		protected virtual void OnIsCheckedChanged(bool? oldValue, bool? value) {
			EditStrategy.IsCheckedChanged(oldValue, value);
			UpdateVisualState(true);
			CheckEditAutomationPeer peer = (CheckEditAutomationPeer)FrameworkElementAutomationPeer.FromElement(this);
			if (peer != null)
				peer.RaiseToggleStatePropertyChangedEvent(oldValue, IsChecked);
		}
		protected override bool NeedsLeftRight() {
			return false;
		}
		protected override bool NeedsUpDown() {
			return false;
		}
		protected internal virtual void OnToggle() {
			EditStrategy.PerformToggle();
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new CheckEditStrategy(this);
		}
		protected virtual object CoerceIsChecked(object isChecked) {
			return EditStrategy.CoerceValue(CheckEdit.IsCheckedProperty, isChecked);
		}
		protected virtual void ContentChanged(DependencyPropertyChangedEventArgs e) {
			HasContent = e.NewValue != null;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if (!e.Handled)
				e.Handled = EditStrategy.ProcessKeyDown(e.Key);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if (!e.Handled)
				e.Handled = EditStrategy.ProcessKeyUp(e.Key);
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if (!e.Handled)
				e.Handled = EditStrategy.ProcessMouseLeftButtonDown();
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			if (!e.Handled)
				e.Handled = EditStrategy.ProcessMouseLeftButtonUp();
		}
		protected override void ProcessActivatingKeyCore(Key key, ModifierKeys modifiers) {
			base.ProcessActivatingKeyCore(key, modifiers);
			EditStrategy.ProcessKeyDown(key);
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			if (!e.GetHandled())
				e.SetHandled(EditStrategy.ProcessMouseEnter());
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
#if SL
			(CheckBox as DevExpress.Xpf.Editors.WPFCompatibility.SLToggleButton).Do(x => x.OnMouseLeaveInternal(e));
#endif
			if (!e.GetHandled())
				e.SetHandled(EditStrategy.ProcessMouseLeave());
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualState(false);
		}
		protected override string GetStateName() {
			if (IsChecked.HasValue) {
				if (IsChecked.Value)
					return "Checked";
				if (!IsChecked.Value)
					return "Unchecked";
			}
			return "Indeterminate";
		}
		protected virtual void CommandChanged(ICommand oldValue, ICommand newValue) {
			if (oldValue != null)
				oldValue.CanExecuteChanged -= CommandCanExecuteChanged;
			if (newValue != null)
				newValue.CanExecuteChanged += CommandCanExecuteChanged;
			EditStrategy.UpdateCanExecute(newValue);
		}
		protected virtual void CommandCanExecuteChanged(object sender, EventArgs e) {
			EditStrategy.UpdateCanExecute(Command);
		}
		protected virtual void CommandParameterChanged(object parameter) {
			EditStrategy.UpdateCanExecute(Command);
		}
		protected internal virtual void UpdateIsEnabledCore() {
#if SL
			IsEnabled = IsEnabledCore;
#else
			CoerceValue(IsEnabledProperty);
#endif
		}
		#endregion
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new CheckEditPropertyProvider(this);
		}
	}
	public class CheckEditPropertyProvider : ActualPropertyProvider {
		public CheckEditPropertyProvider(CheckEdit editor)
			: base(editor) {
		}
		public override bool CalcSuppressFeatures() {
			return false;
		}
	}
}
namespace DevExpress.Xpf.Editors {
	[ToolboxItem(false)]
	public class CheckEditBox : ToggleStateButton {
#if !SL
		static CheckEditBox() {
			IsCheckedProperty.OverrideMetadata(typeof(CheckEditBox), new FrameworkPropertyMetadata((d, e) => ((CheckEditBox)d).OnIsCheckedChanged(e)));
			HorizontalContentAlignmentProperty.OverrideMetadata(typeof(CheckEditBox), new FrameworkPropertyMetadata((d, e) => ((CheckEditBox)d).OnHorizontalContentAlignmentChanged(e)));
			VerticalContentAlignmentProperty.OverrideMetadata(typeof(CheckEditBox), new FrameworkPropertyMetadata((d, e) => ((CheckEditBox)d).OnVerticalContentAlignmentChanged(e)));
			PaddingProperty.OverrideMetadata(typeof(CheckEditBox), new FrameworkPropertyMetadata((d, e) => ((CheckEditBox)d).OnPaddingChanged(e)));
			SnapsToDevicePixelsProperty.OverrideMetadata(typeof(CheckEditBox), new FrameworkPropertyMetadata((d, e) => ((CheckEditBox)d).OnSnapsToDevicePixelsChanged(e)));
			ContentProperty.OverrideMetadata(typeof(CheckEditBox), new FrameworkPropertyMetadata((d, e) => ((CheckEditBox)d).OnContentChanged(e)));
			ContentTemplateProperty.OverrideMetadata(typeof(CheckEditBox), new FrameworkPropertyMetadata((d, e) => ((CheckEditBox)d).OnContentTemplateChanged(e)));
		}
		public event DependencyPropertyChangedEventHandler IsCheckedChanged;
		public event EventHandler RequestUpdate;
		protected virtual void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e) {
			if (IsCheckedChanged != null)
				IsCheckedChanged(this, e);
		}
		protected virtual void OnHorizontalContentAlignmentChanged(DependencyPropertyChangedEventArgs e) {
			RaiseRequestUpdate();
		}
		protected virtual void OnVerticalContentAlignmentChanged(DependencyPropertyChangedEventArgs e) {
			RaiseRequestUpdate();
		}
		protected virtual void OnPaddingChanged(DependencyPropertyChangedEventArgs e) {
			RaiseRequestUpdate();
		}
		protected virtual void OnSnapsToDevicePixelsChanged(DependencyPropertyChangedEventArgs e) {
			RaiseRequestUpdate();
		}
		protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e) {
			RaiseRequestUpdate();
		}
		protected virtual void OnContentTemplateChanged(DependencyPropertyChangedEventArgs e) {
			RaiseRequestUpdate();
		}
		protected virtual void RaiseRequestUpdate() {
			if (RequestUpdate != null)
				RequestUpdate(this, EventArgs.Empty);
		}
#endif
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new CheckEditBoxAutomationPeer(this);
		}
#if !SL
		protected override void OnKeyDown(KeyEventArgs e) { }
		protected override void OnKeyUp(KeyEventArgs e) { }
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) { }
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) { }
		protected override void OnMouseEnter(MouseEventArgs e) { }
		protected override void OnMouseLeave(MouseEventArgs e) { }
#else
		protected override bool GenerateNonPreviewEvents { get { return false; } }
#endif
	}
}
