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
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
#if !SL
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Automation;
#else
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public partial class SpinEdit : ButtonEdit {
		#region static
		public static readonly DependencyProperty ValueProperty;
		public static readonly DependencyProperty MinValueProperty;
		public static readonly DependencyProperty MaxValueProperty;
		public static readonly DependencyProperty IncrementProperty;
		public static readonly DependencyProperty IsFloatValueProperty;
		public static readonly DependencyProperty AllowRoundOutOfRangeValueProperty;
		static SpinEdit() {
			Type ownerType = typeof(SpinEdit);
			AllowRoundOutOfRangeValueProperty = DependencyPropertyManager.Register("AllowRoundOutOfRangeValue", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, (d, e) => ((SpinEdit)d).AllowRoundOutOfRangeValueChanged((bool)e.NewValue)));
			ValueProperty = DependencyPropertyManager.Register("Value", typeof(decimal), ownerType, new FrameworkPropertyMetadata(decimal.Zero, new PropertyChangedCallback(OnDecimalValueChanged), CoerceDecimalValue));
			MinValueProperty = DependencyPropertyManager.Register("MinValue", typeof(decimal?), ownerType, new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(CoerceMinValue)));
			MaxValueProperty = DependencyPropertyManager.Register("MaxValue", typeof(decimal?), ownerType, new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(CoerceMaxValue)));
			IncrementProperty = DependencyPropertyManager.Register("Increment", typeof(decimal), ownerType, new FrameworkPropertyMetadata(decimal.One));
			IsFloatValueProperty = DependencyPropertyManager.Register("IsFloatValue", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, OnIsFloatValueChanged));
#if !SL
			HorizontalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(HorizontalAlignment.Right));
			MaskTypeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(MaskType.Numeric));
			VerticalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(VerticalAlignment.Center));
#endif
		}
		protected static object CoerceDecimalValue(DependencyObject d, object baseValue) {
			return ((SpinEdit)d).CoerceDecimalValue((decimal)baseValue);
		}
		protected static object CoerceMaxValue(DependencyObject d, object baseValue) {
			return ((SpinEdit)d).CoerceMaxValue((decimal?)baseValue);
		}
		protected static object CoerceMinValue(DependencyObject d, object baseValue) {
			return ((SpinEdit)d).CoerceMinValue((decimal?)baseValue);
		}
		static void OnDecimalValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((SpinEdit)obj).OnDecimalValueChanged((decimal)e.OldValue, (decimal)e.NewValue);
		}
		static void OnMinValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((SpinEdit)obj).OnMinValueChanged((decimal?)e.NewValue);
		}
		static void OnMaxValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((SpinEdit)obj).OnMaxValueChanged((decimal?)e.NewValue);
		}
		static void OnIsFloatValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((SpinEdit)obj).OnIsFloatValueChanged();
		}
		#endregion
#if !SL
		public SpinEdit() {
			SetCommands();
		}
#endif
		private void SetCommands() {
			MinimizeCommand = DelegateCommandFactory.Create<object>(parameter => Minimize(), parameter => { return true; }, false);
			MaximizeCommand = DelegateCommandFactory.Create<object>(parameter => Maximize(), parameter => { return true; }, false);
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("SpinEditMinimizeCommand")]
#endif
		public ICommand MinimizeCommand { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("SpinEditMaximizeCommand")]
#endif
		public ICommand MaximizeCommand { get; private set; }
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("SpinEditValue"),
#endif
 Category("Common Properties")]
#if SL
		[TypeConverter(typeof(SLTypeConverter<decimal>))]
#endif
		public decimal Value {
			get { return (decimal)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		public bool AllowRoundOutOfRangeValue {
			get { return (bool)GetValue(AllowRoundOutOfRangeValueProperty); }
			set { SetValue(AllowRoundOutOfRangeValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("SpinEditMinValue"),
#endif
 Category("Behavior")]
#if SL
		[TypeConverter(typeof(NullableConverter<decimal>))]
#endif
		public decimal? MinValue {
			get { return (decimal?)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("SpinEditMaxValue"),
#endif
 Category("Behavior")]
#if SL
		[TypeConverter(typeof(NullableConverter<decimal>))]
#endif
		public decimal? MaxValue {
			get { return (decimal?)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("SpinEditIncrement"),
#endif
 Category("Behavior")]
#if SL
		[TypeConverter(typeof(NullableConverter<decimal>))]
#endif
		public decimal Increment {
			get { return (decimal)GetValue(IncrementProperty); }
			set { SetValue(IncrementProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("SpinEditIsFloatValue"),
#endif
 Category("Behavior")]
		public bool IsFloatValue {
			get { return (bool)GetValue(IsFloatValueProperty); }
			set { SetValue(IsFloatValueProperty, value); }
		}
		protected internal override MaskType DefaultMaskType { get { return MaskType.Numeric; } }
		new protected SpinEditStrategy EditStrategy {
			get { return base.EditStrategy as SpinEditStrategy; }
		}
		protected virtual decimal? CoerceMinValue(decimal? baseValue) {
			return EditStrategy.CoerceMinValue(baseValue);
		}
		protected virtual decimal CoerceDecimalValue(decimal baseValue) {
			return EditStrategy.CoerceDecimalValue(baseValue);
		}
		protected virtual decimal? CoerceMaxValue(decimal? baseValue) {
			return EditStrategy.CoerceMaxValue(baseValue);
		}
		protected virtual void OnDecimalValueChanged(decimal oldValue, decimal value) {
			EditStrategy.SyncWithDecimalValue(oldValue, value);
		}
		protected virtual void OnMinValueChanged(decimal? value) {
			EditStrategy.MinValueChanged(value);
		}
		protected virtual void OnMaxValueChanged(decimal? value) {
			EditStrategy.MaxValueChanged(value);
		}
		protected virtual void OnIsFloatValueChanged() {
			EditStrategy.OnIsFloatValueChanged();
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new SpinEditAutomationPeer(this);
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new SpinEditStrategy(this);
		}
		public void Maximize() {
			EditStrategy.Maximize();
		}
		public void Minimize() {
			EditStrategy.Minimize();
		}
		protected virtual void AllowRoundOutOfRangeValueChanged(bool value) {
			EditStrategy.RoundToBoundsChanged(value);
		}
#if SL
		protected override void ProcessNewButtonInternal(ButtonInfoBase info) {
			base.ProcessNewButtonInternal(info);
			var spinButton = info as SpinButtonInfo;
			if (spinButton == null)
				return;
			if (spinButton.IsDefaultButton) {
				spinButton.SpinUpCommand = SpinUpCommand;
				spinButton.SpinDownCommand = SpinDownCommand;
			}
		}
#endif
#if !SL
		protected void CanMaximize(object sender, CanExecuteRoutedEventArgs e) {
			if (EditStrategy != null)
				e.CanExecute = EditStrategy.CanMaximize();
		}
		protected void CanMinimize(object sender, CanExecuteRoutedEventArgs e) {
			if (EditStrategy != null)
				e.CanExecute = EditStrategy.CanMinimize();
		}
#endif
		protected internal override MaskType[] GetSupportedMaskTypes() {
			return new[] { MaskType.Numeric };
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new SpinEditPropertyProvider(this);
		}
		protected internal override TextInputSettingsBase CreateTextInputSettings() {
			return new TextInputMaskSettings(this) {};
		}
	}
	public class SpinEditPropertyProvider : ButtonEditPropertyProvider {
		new SpinEdit Editor { get { return base.Editor as SpinEdit; } }
		public SpinEditPropertyProvider(SpinEdit editor)
			: base(editor) {
		}
		public override bool CalcSuppressFeatures() {
			return base.CalcSuppressFeatures() && !(Editor.MinValue.HasValue || Editor.MaxValue.HasValue);
		}
	}
}
