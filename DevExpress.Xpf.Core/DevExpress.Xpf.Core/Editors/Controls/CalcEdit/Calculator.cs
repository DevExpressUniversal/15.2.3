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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using System.ComponentModel;
#if !SL
using DevExpress.Xpf.Utils;
using System.Collections;
using DevExpress.Xpf.Utils.Themes;
#else
using DevExpress.Utils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Editors {
	public enum CalcStatus { Ok, OkEnteringDigits, Error };
	public class CalculatorCustomErrorTextEventArgs : RoutedEventArgs {
		public CalculatorCustomErrorTextEventArgs(string errorText)
			: base(Calculator.CustomErrorTextEvent) {
			ErrorText = errorText;
		}
		public string ErrorText { get; set; }
	}
	public delegate void CalculatorCustomErrorTextEventHandler(object sender, CalculatorCustomErrorTextEventArgs e);
	public class CalculatorValueChangedEventArgs : RoutedEventArgs {
		public CalculatorValueChangedEventArgs(decimal oldValue, decimal newValue)
			: base(Calculator.ValueChangedEvent) {
			NewValue = newValue;
			OldValue = oldValue;
		}
		public decimal NewValue { get; private set; }
		public decimal OldValue { get; private set; }
	}
	public delegate void CalculatorValueChangedEventHandler(object sender, CalculatorValueChangedEventArgs e);
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public class Calculator : Control, ICalculatorViewOwner {
		internal const int MaxPrecision = 28;
		public static readonly DependencyProperty DisplayTextProperty;
		public static readonly DependencyProperty HasErrorProperty;
		public static readonly DependencyProperty HistoryProperty;
		public static readonly DependencyProperty IsDigitalDisplayProperty;
		public static readonly DependencyProperty MemoryProperty;
		public static readonly DependencyProperty PrecisionProperty;
		public static readonly DependencyProperty ShowBorderProperty;
		public static readonly DependencyProperty ShowFocusedStateProperty;
		public static readonly DependencyProperty ValueProperty;
		protected static readonly DependencyPropertyKey DisplayTextPropertyKey;
		protected static readonly DependencyPropertyKey HasErrorPropertyKey;
		protected static readonly DependencyPropertyKey HistoryPropertyKey;
		protected static readonly DependencyPropertyKey MemoryPropertyKey;
		public static readonly RoutedEvent CustomErrorTextEvent;
		public static readonly RoutedEvent ValueChangedEvent;
		static Calculator() {
			Type ownerType = typeof(Calculator);
			DisplayTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("DisplayText", typeof(string), ownerType,
				new PropertyMetadata((d, e) => ((Calculator)d).PropertyChangedDisplayText((string)e.OldValue)));
			DisplayTextProperty = DisplayTextPropertyKey.DependencyProperty;
			HasErrorPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasError", typeof(bool), typeof(Calculator),
			  new PropertyMetadata((d, e) => ((Calculator)d).PropertyChangedHasError()));
			HasErrorProperty = HasErrorPropertyKey.DependencyProperty;
			HistoryPropertyKey = DependencyPropertyManager.RegisterReadOnly("History", typeof(ReadOnlyObservableCollection<string>), typeof(Calculator),
			  new PropertyMetadata());
			HistoryProperty = HistoryPropertyKey.DependencyProperty;
			IsDigitalDisplayProperty = DependencyPropertyManager.Register("IsDigitalDisplay", typeof(bool), typeof(Calculator),
			  new PropertyMetadata(true, (d, e) => ((Calculator)d).PropertyChangedIsDigitalDisplay()));
			MemoryPropertyKey = DependencyPropertyManager.RegisterReadOnly("Memory", typeof(decimal), ownerType,
				new PropertyMetadata((d, e) => ((Calculator)d).PropertyChangedMemory((decimal)e.OldValue)));
			MemoryProperty = MemoryPropertyKey.DependencyProperty;
			PrecisionProperty = DependencyPropertyManager.Register("Precision", typeof(int), ownerType,
				new PropertyMetadata(6, (d, e) => ((Calculator)d).PropertyChangedPrecision((int)e.OldValue)), PropertyValueValidatePrecision);
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(Calculator),
			  new PropertyMetadata(true, (d, e) => ((Calculator)d).PropertyChangedShowBorder()));
			ShowFocusedStateProperty = DependencyPropertyManager.Register("ShowFocusedState", typeof(bool), typeof(Calculator),
			  new PropertyMetadata(true, (d, e) => ((Calculator)d).PropertyChangedShowFocusedState()));
			ValueProperty = DependencyPropertyManager.Register("Value", typeof(decimal), ownerType,
				new PropertyMetadata((d, e) => ((Calculator)d).PropertyChangedValue((decimal)e.OldValue)));
			CustomErrorTextEvent = EventManager.RegisterRoutedEvent("CustomErrorText", RoutingStrategy.Direct, typeof(CalculatorCustomErrorTextEventHandler), ownerType);
			ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct, typeof(CalculatorValueChangedEventHandler), ownerType);
#if !SL
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Copy, (d, e) => ((Calculator)d).Copy(), (d, e) => ((Calculator)d).CanCopy(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Paste, (d, e) => ((Calculator)d).Paste(), (d, e) => ((Calculator)d).CanPaste(e)));
#endif
		}
		static bool PropertyValueValidatePrecision(object value) {
			int precision = (int)value;
			return precision >= 0 && precision <= MaxPrecision;
		}
		public Calculator() {
			this.SetDefaultStyleKey(typeof(Calculator));
			ButtonClickCommand = DelegateCommandFactory.Create<object>(buttonID => View.ProcessButtonClick(buttonID), false);
			HistoryList = new ObservableCollection<string>();
			History = new ReadOnlyObservableCollection<string>(HistoryList);
			View = CreateView();
			View.Precision = Precision;
#if !SL
			AddHandler(MouseDownEvent, new MouseButtonEventHandler(OnMouseDownInternal), true);
#else
			AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseDownInternal), true);
#endif
			Reset();
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CalculatorButtonClickCommand")]
#endif
public ICommand ButtonClickCommand { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CalculatorDisplayText")]
#endif
public string DisplayText {
			get { return (string)GetValue(DisplayTextProperty); }
			protected set { this.SetValue(DisplayTextPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CalculatorHasError")]
#endif
public bool HasError {
			get { return (bool)GetValue(HasErrorProperty); }
			protected set { this.SetValue(HasErrorPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CalculatorHistory")]
#endif
public ReadOnlyObservableCollection<string> History {
			get { return (ReadOnlyObservableCollection<string>)GetValue(HistoryProperty); }
			protected set { this.SetValue(HistoryPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CalculatorIsDigitalDisplay")]
#endif
public bool IsDigitalDisplay {
			get { return (bool)GetValue(IsDigitalDisplayProperty); }
			set { SetValue(IsDigitalDisplayProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CalculatorMemory")]
#endif
public decimal Memory {
			get { return (decimal)GetValue(MemoryProperty); }
			protected set { this.SetValue(MemoryPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CalculatorPrecision")]
#endif
public int Precision {
			get { return (int)GetValue(PrecisionProperty); }
			set { SetValue(PrecisionProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CalculatorShowBorder")]
#endif
public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CalculatorShowFocusedState")]
#endif
public bool ShowFocusedState {
			get { return (bool)GetValue(ShowFocusedStateProperty); }
			set { SetValue(ShowFocusedStateProperty, value); }
		}
#if SL
		[TypeConverter(typeof(SLTypeConverter<decimal>))]
#endif
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CalculatorValue")]
#endif
public decimal Value {
			get { return (decimal)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		protected ObservableCollection<string> HistoryList { get; private set; }
		internal protected CalculatorViewBase View { get; private set; }
		public event CalculatorCustomErrorTextEventHandler CustomErrorText {
			add { this.AddHandler(CustomErrorTextEvent, value); }
			remove { this.RemoveHandler(CustomErrorTextEvent, value); }
		}
		public event CalculatorValueChangedEventHandler ValueChanged {
			add { this.AddHandler(ValueChangedEvent, value); }
			remove { this.RemoveHandler(ValueChangedEvent, value); }
		}
		public virtual void ClearHistory() {
			HistoryList.Clear();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (VisualTreeHelper.GetChildrenCount(this) != 0) {
				FrameworkElement element = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
				if (element != null)
					element.DataContext = this;
			}
			UpdateVisualState(false);
		}
		public virtual void Reset() {
			View.Init(0, true);
		}
		protected virtual CalculatorViewBase CreateView() {
			return new CalculatorStandardView(this);
		}
		protected virtual ButtonBase GetButton(object buttonID) {
			return (ButtonBase)LayoutHelper.FindElement(this, (element) => IsButtonWithID(element, buttonID));
		}
#if !SL
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnGotKeyboardFocus(e);
#else
		protected override void  OnGotFocus(System.Windows.RoutedEventArgs e) {
 			base.OnGotFocus(e);
#endif
			UpdateVisualState(true);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
#if SL
			if (!e.Handled)
				InternalProcessKeyDown(e);
#endif
			if (!e.Handled)
				View.OnKeyDown(e);
		}
#if !SL
		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnLostKeyboardFocus(e);
#else
		protected override void  OnLostFocus(System.Windows.RoutedEventArgs e) {
			base.OnLostFocus(e);
#endif
			UpdateVisualState(true);
		}
		protected override void OnTextInput(TextCompositionEventArgs e) {
			base.OnTextInput(e);
			if (!e.Handled)
				View.OnTextInput(e);
		}
		protected virtual void PropertyChangedDisplayText(string oldValue) { }
		protected virtual void PropertyChangedHasError() {
			UpdateVisualState(true);
		}
		protected virtual void PropertyChangedIsDigitalDisplay() {
			UpdateVisualState(true);
		}
		protected virtual void PropertyChangedMemory(decimal oldValue) { }
		protected virtual void PropertyChangedPrecision(int oldValue) {
			View.Precision = Precision;
		}
		protected virtual void PropertyChangedShowBorder() {
			UpdateVisualState(true);
		}
		protected virtual void PropertyChangedShowFocusedState() {
			UpdateVisualState(true);
		}
		protected virtual void PropertyChangedValue(decimal oldValue) {
			View.Value = Value;
			RaiseValueChanged(oldValue);
		}
		protected virtual void RaiseValueChanged(decimal oldValue) {
			CalculatorValueChangedEventArgs e = new CalculatorValueChangedEventArgs(oldValue, Value);
			this.RaiseEvent(e);
		}
		protected virtual void UpdateVisualState(bool useTransitions) {
			VisualStateManager.GoToState(this, HasError ? "Error" : "NoError", useTransitions);
			VisualStateManager.GoToState(this, IsDigitalDisplay ? "DigitalDisplay" : "TextDisplay", useTransitions);
			VisualStateManager.GoToState(this, ShowBorder ? "ShowBorder" : "EmptyBorder", useTransitions);
			VisualStateManager.GoToState(this, FocusHelper.IsKeyboardFocusWithin(this) && ShowFocusedState ? "Focused" : "Unfocused", useTransitions);
		}
		bool IsButtonWithID(FrameworkElement element, object buttonID) {
			return (element is ButtonBase) && object.Equals(buttonID, (element as ButtonBase).CommandParameter);
		}
		void OnMouseDownInternal(object sender, MouseButtonEventArgs e) {
			Focus();
		}
		#region Clipboard
		protected virtual bool CanCopy() {
			return View.CanCopy();
		}
		protected virtual bool CanPaste() {
			return View.CanPaste();
		}
		protected virtual void Copy() {
			View.Copy();
		}
		protected virtual void Paste() {
			View.Paste();
		}
#if !SL
		void CanCopy(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanCopy();
		}
		void CanPaste(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanPaste();
		}
#else
		protected virtual void InternalProcessKeyDown(KeyEventArgs e) {
			ClipboardCommandType? clipboardCommandType;
			ClipboardHelper.CheckKey(e, out clipboardCommandType);
			switch (clipboardCommandType) {
				case ClipboardCommandType.Copy:
					if (CanCopy()) {
						Copy();
						e.Handled = true;
					}
					break;
				case ClipboardCommandType.Paste:
					if (CanPaste()) {
						Paste();
						e.Handled = true;
					}
					break;
			}
		}
#endif
		#endregion
		#region ICalculatorViewOwner Members
		struct ButtonClickAnimationData {
			public List<ButtonBase> Buttons;
			public List<DispatcherTimer> Timers;
		}
		ButtonClickAnimationData buttonClickAnimationData = new ButtonClickAnimationData() {
			Buttons = new List<ButtonBase>(),
			Timers = new List<DispatcherTimer>()
		};
		void OnButtonClickAnimationTimerTick(object sender, EventArgs e) {
			DispatcherTimer timer = (DispatcherTimer)sender;
			timer.Stop();
			timer.Tick -= OnButtonClickAnimationTimerTick;
			int index = buttonClickAnimationData.Timers.IndexOf(timer);
			buttonClickAnimationData.Timers.RemoveAt(index);
			ButtonBase button = buttonClickAnimationData.Buttons[index];
			buttonClickAnimationData.Buttons.RemoveAt(index);
			VisualStateManager.GoToState(button, button.IsMouseOver ? "MouseOver" : "Normal", true);
		}
		void ICalculatorViewOwner.AddToHistory(string text) {
			HistoryList.Add(text);
		}
		void ICalculatorViewOwner.AnimateButtonClick(object buttonID) {
			ButtonBase button = GetButton(buttonID);
			if (button != null) {
				VisualStateManager.GoToState(button, "Pressed", true);
				DispatcherTimer timer;
				if (buttonClickAnimationData.Buttons.Contains(button)) {
					timer = buttonClickAnimationData.Timers[buttonClickAnimationData.Buttons.IndexOf(button)];
					timer.Stop();
				} else {
					buttonClickAnimationData.Buttons.Add(button);
					timer = new DispatcherTimer();
					buttonClickAnimationData.Timers.Add(timer);
					timer.Interval = TimeSpan.FromMilliseconds(100);
					timer.Tick += new EventHandler(OnButtonClickAnimationTimerTick);
				}
				timer.Start();
			}
		}
		void ICalculatorViewOwner.GetCustomErrorText(ref string errorText) {
			CalculatorCustomErrorTextEventArgs e = new CalculatorCustomErrorTextEventArgs(errorText);
			this.RaiseEvent(e);
			errorText = e.ErrorText;
		}
		void ICalculatorViewOwner.SetDisplayText(string value) {
			DisplayText = value;
		}
		void ICalculatorViewOwner.SetHasError(bool value) {
			HasError = value;
		}
		void ICalculatorViewOwner.SetMemory(decimal value) {
			Memory = value;
		}
		void ICalculatorViewOwner.SetValue(decimal value) {
			Value = value;
		}
		#endregion
	}
	public class CalculatorMemoryIndicatorVisibilityConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (((decimal)value) != 0) ? Visibility.Visible : Visibility.Collapsed;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	[DXToolboxBrowsable(false)]
	public class PopupCalcEditCalculator : Calculator {
		public static readonly DependencyProperty IsMemoryIndicatorProperty;
		static PopupCalcEditCalculator() {
			IsMemoryIndicatorProperty = DependencyPropertyManager.RegisterAttached("IsMemoryIndicator", typeof(bool), typeof(PopupCalcEditCalculator), new PropertyMetadata());
		}
		public static bool GetIsMemoryIndicator(DependencyObject obj) {
			return (bool)obj.GetValue(IsMemoryIndicatorProperty);
		}
		public static void SetIsMemoryIndicator(DependencyObject obj, bool value) {
			obj.SetValue(IsMemoryIndicatorProperty, value);
		}
		public PopupCalcEditCalculator() {
			this.SetDefaultStyleKey(typeof(PopupCalcEditCalculator));
			LayoutUpdated += OnLayoutUpdated;
		}
		readonly List<Control> memoryIndicators = new List<Control>();
		public PopupCalcEdit OwnerEdit {
			get { return (PopupCalcEdit)BaseEdit.GetOwnerEdit(this); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (OwnerEdit == null)
				return;
			OwnerEdit.Calculator = this;
			UpdateDisplayText();
		}
		protected override void PropertyChangedDisplayText(string oldValue) {
			base.PropertyChangedDisplayText(oldValue);
			UpdateDisplayText();
		}
		protected override void OnTextInput(TextCompositionEventArgs e) {
			base.OnTextInput(e);
			UpdateDisplayText();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			UpdateDisplayText();
		}
		protected override void PropertyChangedMemory(decimal oldValue) {
			base.PropertyChangedMemory(oldValue);
			UpdateMemoryIndicators(true);
		}
		internal void InitMemory(decimal value) {
			View.Memory = value;
		}
		void CheckMemoryIndicator(FrameworkElement element) {
			if (GetIsMemoryIndicator(element) && element is Control)
				memoryIndicators.Add(element as Control);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			LayoutUpdated -= OnLayoutUpdated;
			memoryIndicators.Clear();
			LayoutHelper.ForEachElement(this, CheckMemoryIndicator);
			UpdateMemoryIndicators(false);
		}
		void UpdateDisplayText() {
			if (OwnerEdit == null)
				return;
			OwnerEdit.ForceChangeDisplayText();
		}
		void UpdateMemoryIndicators(bool useTransitions) {
			foreach (Control indicator in memoryIndicators)
				VisualStateManager.GoToState(indicator, (Memory == Decimal.Zero) ? "EmptyMemory" : "MemoryAssigned", useTransitions);
		}
	}
	public class CalculatorPanel : Panel {
		protected override Size ArrangeOverride(Size finalSize) {
			if (Children.Count != 2)
				return base.ArrangeOverride(finalSize);
			finalSize.Width = Math.Max(finalSize.Width, Children[1].DesiredSize.Width);
			finalSize.Height = Math.Max(finalSize.Height, Children[0].DesiredSize.Height + Children[1].DesiredSize.Height);
			Children[0].Arrange(new Rect(0, 0, finalSize.Width, Children[0].DesiredSize.Height));
			Children[1].Arrange(new Rect(0, Children[0].DesiredSize.Height, finalSize.Width, finalSize.Height - Children[0].DesiredSize.Height));
			return finalSize;
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (Children.Count != 2)
				return base.MeasureOverride(availableSize);
			Calculator calculator = LayoutHelper.FindParentObject<Calculator>(this);
			Children[1].Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
			Size result = new Size();
			result.Width = Children[1].DesiredSize.Width;
			Children[0].Measure(new Size(result.Width, Double.PositiveInfinity));
			result.Height = Children[0].DesiredSize.Height + Children[1].DesiredSize.Height;
			return result;
		}
	}
	public class CalculatorMemoryIndicator : Control {
		public CalculatorMemoryIndicator() {
			this.SetDefaultStyleKey(typeof(CalculatorMemoryIndicator));
		}
	}
}
