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

using System.Windows;
using DevExpress.Xpf.Core;
using System;
using System.Windows.Input;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Utils;
using System.ComponentModel;
using System.Globalization;
#if !SL
using DevExpress.Xpf.Utils;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using SelectionChangedEventHandler = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventHandler;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public class PopupCalcEdit : PopupBaseEdit {
		public static readonly DependencyProperty IsPopupAutoWidthProperty;
		public static readonly DependencyProperty PrecisionProperty;
		public static readonly DependencyProperty ValueProperty;
		public static readonly RoutedEvent CustomErrorTextEvent;
		static PopupCalcEdit() {
			Type ownerType = typeof(PopupCalcEdit);
			IsPopupAutoWidthProperty = DependencyPropertyManager.Register("IsPopupAutoWidth", typeof(bool), ownerType,
			  new FrameworkPropertyMetadata(true, OnIsPopupAutoWidthChanged));
			PrecisionProperty = DependencyPropertyManager.Register("Precision", typeof(int), ownerType, new FrameworkPropertyMetadata(6, OnPrecisionChanged), PropertyValueValidatePrecision);
			ValueProperty = DependencyPropertyManager.Register("Value", typeof(decimal), ownerType, new FrameworkPropertyMetadata(decimal.Zero, OnDecimalValueChanged, CoerceDecimalValueProperty));
#if !SL
			CustomErrorTextEvent = DevExpress.Xpf.Editors.Calculator.CustomErrorTextEvent.AddOwner(ownerType);
			HorizontalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(HorizontalAlignment.Right));
			MaskTypeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(MaskType.Numeric));
#else
			CustomErrorTextEvent = EventManager.RegisterRoutedEvent("CustomErrorText", RoutingStrategy.Direct, typeof(CalculatorCustomErrorTextEventHandler), ownerType);
#endif
		}
		static object CoerceDecimalValueProperty(DependencyObject d, object value) {
			return ((PopupCalcEdit)d).CoerceDecimalValue((decimal)value);
		}
		static void OnDecimalValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupCalcEdit)obj).OnDecimalValueChanged((decimal)e.OldValue, (decimal)e.NewValue);
		}
		static void OnIsPopupAutoWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupCalcEdit)obj).OnIsPopupAutoWidthChanged();
		}
		static void OnPrecisionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupCalcEdit)obj).OnPrecisionChanged((int)e.NewValue);
		}
		static bool PropertyValueValidatePrecision(object value) {
			int precision = (int)value;
			return precision >= 0 && precision <= DevExpress.Xpf.Editors.Calculator.MaxPrecision;
		}
		PopupCalcEditCalculator calculator;
		decimal prevCalculatorMemory = 0;
		protected internal override Type StyleSettingsType { get { return typeof(PopupCalcEditStyleSettings); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupCalcEditIsPopupAutoWidth")]
#endif
		public bool IsPopupAutoWidth {
			get { return (bool)GetValue(IsPopupAutoWidthProperty); }
			set { SetValue(IsPopupAutoWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupCalcEditPrecision")]
#endif
		public int Precision {
			get { return (int)GetValue(PrecisionProperty); }
			set { SetValue(PrecisionProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupCalcEditValue")]
#endif
		public decimal Value {
			get { return (decimal)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		protected internal override MaskType DefaultMaskType { get { return MaskType.Numeric; } }
		protected new CalcEditStrategy EditStrategy {
			get { return base.EditStrategy as CalcEditStrategy; }
			set { base.EditStrategy = value; }
		}
		internal PopupCalcEditCalculator Calculator {
			get { return calculator; }
			set {
				if (calculator == value)
					return;
				PopupCalcEditCalculator oldCalculator = calculator;
				if (calculator != null) {
					calculator.CustomErrorText -= OnCalculatorCustomErrorText;
					prevCalculatorMemory = calculator.Memory;
				}
				calculator = value;
				if (calculator != null) {
					calculator.CustomErrorText += OnCalculatorCustomErrorText;
					calculator.InitMemory(prevCalculatorMemory);
				}
				EditStrategy.CalculatorAssigned(oldCalculator);
			}
		}
		protected internal override FrameworkElement PopupElement {
			get { return Calculator; }
		}
		public event CalculatorCustomErrorTextEventHandler CustomErrorText {
			add { this.AddHandler(CustomErrorTextEvent, value); }
			remove { this.RemoveHandler(CustomErrorTextEvent, value); }
		}
		public PopupCalcEdit() {
			this.SetDefaultStyleKey(typeof(PopupCalcEdit));
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new PopupCalcEditPropertyProvider(this);
		}
		protected internal override bool ShouldApplyPopupSize {
			get { return !IsPopupAutoWidth; }
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new CalcEditStrategy(this);
		}
		protected override bool IsClosePopupWithAcceptGesture(Key key, ModifierKeys modifiers) {
			return base.IsClosePopupWithAcceptGesture(key, modifiers) ||
				key == Key.Enter && ModifierKeysHelper.IsCtrlPressed(modifiers);
		}
		protected override bool IsClosePopupWithCancelGesture(Key key, ModifierKeys modifiers) {
			if (key == Key.Escape && ModifierKeysHelper.NoModifiers(modifiers) && EditMode == EditMode.Standalone)
				return false;
			return base.IsClosePopupWithCancelGesture(key, modifiers);
		}
		protected override void OnPopupClosed() {
			base.OnPopupClosed();
			EditStrategy.UpdateDisplayText();
			Calculator = null;
		}
		protected override void OnPopupOpened() {
			base.OnPopupOpened();
		}
		protected override void AcceptPopupValue() {
			base.AcceptPopupValue();
			EditStrategy.AcceptCalculatorValue();
		}
		protected virtual void OnCalculatorCustomErrorText(object sender, CalculatorCustomErrorTextEventArgs e) {
			this.RaiseEvent(e);
		}
		protected virtual void OnDecimalValueChanged(decimal oldValue, decimal newValue) {
			EditStrategy.SyncWithValue(PopupCalcEdit.ValueProperty, oldValue, newValue);
		}
		protected virtual void OnIsPopupAutoWidthChanged() { }
		protected virtual void OnPrecisionChanged(int newValue) {
		}
		protected virtual object CoerceDecimalValue(decimal value) {
			return EditStrategy.CoerceDecimalValue(value);
		}
		protected override void OnEditModeChanged(EditMode oldValue, EditMode newValue) {
			base.OnEditModeChanged(oldValue, newValue);
			if (newValue == EditMode.InplaceInactive)
				prevCalculatorMemory = 0;
		}
	}
	public class PopupCalcEditPropertyProvider : PopupBaseEditPropertyProvider {
		public PopupCalcEditPropertyProvider(PopupCalcEdit editor) : base(editor) {
		}
		protected override bool GetFocusPopupOnOpenInternal() {
			return true;
		}
	}
	public class PopupCalcEditStyleSettings : PopupBaseEditStyleSettings {
		public PopupCalcEditStyleSettings() {
		}
		protected internal override PopupCloseMode GetClosePopupOnClickMode(PopupBaseEdit editor) {
			return PopupCloseMode.Normal;
		}
	}
}
