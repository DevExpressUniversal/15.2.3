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
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Validation.Native;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Popups.Calendar;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Editors.Automation;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Popups.Calendar;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
using DateTimeTypeConverter = DevExpress.Xpf.Core.DateTimeTypeConverter;
#endif
#if !SL
using PopupFooterButtonsType = DevExpress.Xpf.Editors.PopupFooterButtons;
using DevExpress.Xpf.Editors.Settings;
#else
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PopupFooterButtonsType = DevExpress.Xpf.Editors.PopupFooterButtons;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DevExpress.Xpf.Editors.Settings;
#endif
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public partial class DateEdit : PopupBaseEdit {
		#region static
		public static readonly DependencyProperty MinValueProperty;
		public static readonly DependencyProperty MaxValueProperty;
		public static readonly DependencyProperty ShowWeekNumbersProperty;
		public static readonly DependencyProperty DateTimeProperty;
		public static readonly DependencyProperty ShowClearButtonProperty;
		public static readonly DependencyProperty ShowTodayProperty;
		public static readonly DependencyProperty AllowRoundOutOfRangeValueProperty;
		internal static readonly DependencyPropertyKey DateEditPopupContentTypePropertyKey;
		public static readonly DependencyProperty DateEditPopupContentTypeProperty;
		static DateEdit() {
			Type ownerType = typeof(DateEdit);
			AllowRoundOutOfRangeValueProperty = DependencyPropertyManager.Register("AllowRoundOutOfRangeValue", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, (d, e) => ((DateEdit)d).AllowRoundOutOfRangeValueChanged((bool)e.NewValue)));
			MinValueProperty = DependencyPropertyManager.RegisterAttached("MinValue", typeof(DateTime?), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnMinValueChanged)));
			MaxValueProperty = DependencyPropertyManager.RegisterAttached("MaxValue", typeof(DateTime?), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnMaxValueChanged)));
			ShowWeekNumbersProperty = DependencyPropertyManager.Register("ShowWeekNumbers", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnShowWeekNumbersChanged)));
			ShowClearButtonProperty = DateEditCalendar.ShowClearButtonProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, OnShowClearButtonChanged));
			ShowTodayProperty = DateEditCalendar.ShowTodayProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, OnShowTodayChanged));
			DateEditPopupContentTypePropertyKey = DependencyPropertyManager.RegisterReadOnly("DateEditPopupContentType", typeof(DateEditPopupContentType), ownerType, new FrameworkPropertyMetadata(DateEditPopupContentType.Calendar, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnDateEditPopupContentTypeChanged)));
			DateEditPopupContentTypeProperty = DateEditPopupContentTypePropertyKey.DependencyProperty;
#if !SL
			DateTimeProperty = DependencyPropertyManager.Register("DateTime", typeof(DateTime), ownerType, new FrameworkPropertyMetadata(DateTime.Today, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnDateTimePropertyChanged), new CoerceValueCallback(OnCoerceDateTimeProperty), true, UpdateSourceTrigger.LostFocus));
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			DisplayFormatStringProperty.AddOwner(typeof(DateEdit), new FrameworkPropertyMetadata("d"));
			MaskTypeProperty.AddOwner(typeof(DateEdit), new FrameworkPropertyMetadata(MaskType.DateTime));
			MaskProperty.AddOwner(typeof(DateEdit), new FrameworkPropertyMetadata("d"));
			AllowNullInputProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
			ShowSizeGripProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
#else
			DateTimeProperty = DependencyPropertyManager.Register("DateTime", typeof(DateTime), ownerType, new FrameworkPropertyMetadata(DateTime.Today, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnDateTimePropertyChanged), new CoerceValueCallback(OnCoerceDateTimeProperty)));
#endif
		}
		protected static void OnDateEditPopupContentTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateEdit)obj).OnDateEditPopupContentTypeChanged();
		}
		protected static void OnShowWeekNumbersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateEdit)obj).OnShowWeekNumbersChanged();
		}
		protected static void OnDateTimePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateEdit)obj).OnDateTimeChanged((DateTime)e.OldValue, (DateTime)e.NewValue);
		}
		protected static object OnCoerceDateTimeProperty(DependencyObject obj, object value) {
			return ((DateEdit)obj).CoerceDateTimeProperty(value);
		}
		protected static void OnMinValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateEdit)obj).OnMinValueChanged((DateTime?)e.NewValue);
		}
		protected static void OnMaxValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateEdit)obj).OnMaxValueChanged((DateTime?)e.NewValue);
		}
		protected static void OnShowClearButtonChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateEdit)obj).OnShowClearButtonChanged();
		}
		protected static void OnShowTodayChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateEdit)obj).OnShowTodayChanged();
		}
		#endregion
		protected virtual void OnDateEditPopupContentTypeChanged() {
		}
		protected virtual void OnShowClearButtonChanged() {
			UpdateCalendarShowClearButton();
		}
		protected override void OnAllowNullInputChanged() {
			base.OnAllowNullInputChanged();
			UpdateCalendarShowClearButton();
		}
		protected virtual void OnShowTodayChanged() {
			UpdateCalendarShowToday();
		}
		void UpdateCalendarShowClearButton() {
			EditStrategy.UpdateCalendarShowClearButton();
		}
		void UpdateCalendarShowToday() {
			if (Calendar != null)
				Calendar.ShowToday = ShowToday;
		}
		protected internal override FrameworkElement PopupElement {
			get { return (FrameworkElement)Calendar; }
		}
		DateEditCalendarBase calendar;
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("DateEditDateTime"),
#endif
 Category("Common Properties")]
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime DateTime {
			get { return (DateTime)GetValue(DateTimeProperty); }
			set { SetValue(DateTimeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("DateEditShowClearButton"),
#endif
 Category("Behavior")]
		public bool ShowClearButton {
			get { return (bool)GetValue(ShowClearButtonProperty); }
			set { SetValue(ShowClearButtonProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("DateEditShowToday"),
#endif
 Category("Behavior")]
		public bool ShowToday {
			get { return (bool)GetValue(ShowTodayProperty); }
			set { SetValue(ShowTodayProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("DateEditShowWeekNumbers"),
#endif
 Category("Behavior")]
		public bool ShowWeekNumbers {
			get { return (bool)GetValue(ShowWeekNumbersProperty); }
			set { SetValue(ShowWeekNumbersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("DateEditMinValue"),
#endif
 Category("Behavior")]
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime? MinValue {
			get { return (DateTime?)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("DateEditMaxValue"),
#endif
 Category("Behavior")]
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime? MaxValue {
			get { return (DateTime?)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}
		public bool AllowRoundOutOfRangeValue {
			get { return (bool)GetValue(AllowRoundOutOfRangeValueProperty); }
			set { SetValue(AllowRoundOutOfRangeValueProperty, value); }
		}
		public DateEditPopupContentType DateEditPopupContentType {
			get { return (DateEditPopupContentType)GetValue(DateEditPopupContentTypeProperty); }
			internal set { this.SetValue(DateEditPopupContentTypePropertyKey, value); }
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new DateEditAutomationPeer(this);
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new DateEditPropertyProvider(this);
		}
		protected override bool? CoerceShowSizeGrip(bool? show) {
			return false;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#if SL
		[TypeConverter(typeof(NullableConverter<PopupFooterButtons>))]
#endif
		new public PopupFooterButtons? PopupFooterButtons {
			get { return (PopupFooterButtons?)GetValue(PopupFooterButtonsProperty); }
			set { SetValue(PopupFooterButtonsProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[TypeConverter(typeof(NullableBoolConverter))]
		new public bool? ShowSizeGrip {
			get { return (bool?)GetValue(ShowSizeGripProperty); }
			set { SetValue(ShowSizeGripProperty, value); }
		}
		protected internal new DateEditSettings Settings { get { return (DateEditSettings)base.Settings; } }
		protected internal new Type StyleSettingsType { get { return typeof(PopupBaseEditStyleSettings); } }
		protected internal override MaskType DefaultMaskType { get { return MaskType.DateTime; } }
		protected internal override BaseEditStyleSettings CreateStyleSettings() {
			return new DateEditCalendarStyleSettings();
		}
		protected internal DateEditCalendarBase Calendar {
			get { return calendar; }
			set {
				if (Calendar == value)
					return;
				calendar = value;
				OnCalendarChanged();
			}
		}
		public DateEdit() {
#if SL
			ConstructorSLPart();
#endif
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new DateEditStrategy(this);
		}
		protected virtual void OnCalendarChanged() {
			if (Calendar == null)
				return;
			FlushPendingEditActions(UpdateEditorSource.ValueChanging);
			UpdateCalendarValues();
			Calendar.ShowWeekNumbers = ShowWeekNumbers;
			UpdateCalendarShowClearButton();
			UpdateCalendarShowToday();
		}
#if !SL
		protected override void AcceptPopupValue() {
			if (DateEditPopupContentType == DateEditPopupContentType.DateTimePicker)
				SetDateTime(Calendar.DateTime, UpdateEditorSource.ValueChanging);
			base.AcceptPopupValue();
		}
#endif
		void UpdateCalendarValues() {
			DateTime date = EditStrategy.CreateValueConverter(EditStrategy.EditValue).Value;
			if (EditStrategy.IsNullValue(EditStrategy.EditValue))
				date = DateTime.Today;
			if (MinValue != null && date < MinValue.Value)
				date = MinValue.Value;
			if (MaxValue != null && date > MaxValue.Value)
				date = MaxValue.Value;
			Calendar.Mask = Mask;
			Calendar.DateTime = date;
			Calendar.MinValue = MinValue;
			Calendar.MaxValue = MaxValue;
		}
		internal void SetDateTime(DateTime editValue, UpdateEditorSource updateEditorSource) {
			EditStrategy.SetDateTime(editValue, updateEditorSource);
		}
		protected internal virtual void OnApplyCalendarTemplate(DateEditCalendarBase calendar) {
			Calendar = calendar;
		}
		protected new DateEditStrategy EditStrategy {
			get { return base.EditStrategy as DateEditStrategy; }
		}
		protected virtual void OnMinValueChanged(DateTime? value) {
			EditStrategy.MinValueChanged(value);
			if (Calendar != null)
				Calendar.MinValue = MinValue;
		}
		protected virtual void OnMaxValueChanged(DateTime? value) {
			EditStrategy.MaxValueChanged(value);
			if (Calendar != null)
				Calendar.MaxValue = MaxValue;
		}
		protected virtual void OnDateTimeChanged(DateTime oldDateTime, DateTime dateTime) {
			EditStrategy.DateTimeChanged(oldDateTime, dateTime);
		}
		protected virtual object CoerceDateTimeProperty(object value) {
			return EditStrategy.CoerceDateTime(value);
		}
		protected virtual void OnShowWeekNumbersChanged() {
			if (Calendar != null)
				Calendar.ShowWeekNumbers = ShowWeekNumbers;
		}
		protected internal override bool ShouldApplyPopupSize { get { return false; } }
		protected override void BeforePreviewKeyDown(System.Windows.Input.KeyEventArgs e) {
			base.BeforePreviewKeyDown(e);
			e.Handled = Calendar != null && IsPopupOpen && Calendar.ProcessKeyDown(e);
		}
		protected virtual void AllowRoundOutOfRangeValueChanged(bool value) {
			EditStrategy.RoundToBoundsChanged(value);
		}
		protected internal override MaskType[] GetSupportedMaskTypes() {
			return new MaskType[] { MaskType.DateTime, MaskType.DateTimeAdvancingCaret };
		}
		protected internal override TextInputSettingsBase CreateTextInputSettings() {
			return new TextInputMaskSettings(this) { };
		}
		protected override bool IsClosePopupWithCancelGesture(Key key, ModifierKeys modifiers) {
			return base.IsClosePopupWithCancelGesture(key, modifiers) || EditStrategy.IsClosePopupWithCancelGesture(key, modifiers);
		}
		protected override bool IsClosePopupWithAcceptGesture(Key key, ModifierKeys modifiers) {
			return base.IsClosePopupWithAcceptGesture(key, modifiers) || EditStrategy.IsClosePopupWithAcceptGesture(key, modifiers);
		}
	}
}
