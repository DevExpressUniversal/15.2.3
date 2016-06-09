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
using DevExpress.Xpf.Editors;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Automation;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Printing;
using System.Windows.Media;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
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
	[DXToolboxBrowsable(false)]
	public abstract class RangeBaseEdit : BaseEdit, ITextExportSettings {
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ValueProperty;
		public static readonly DependencyProperty MaximumProperty;
		public static readonly DependencyProperty MinimumProperty;
		public static readonly DependencyProperty SmallStepProperty;
		public static readonly DependencyProperty LargeStepProperty;
		static RangeBaseEdit() {
			Type ownerType = typeof(RangeBaseEdit);
			SmallStepProperty = DependencyPropertyManager.Register("SmallStep", typeof(double), ownerType, new FrameworkPropertyMetadata(1d));
			LargeStepProperty = DependencyPropertyManager.Register("LargeStep", typeof(double), ownerType, new FrameworkPropertyMetadata(5d));
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), ownerType,
				new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.None, OrientationPropertyChanged));
			ValueProperty = DependencyPropertyManager.Register("Value", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.None, ValuePropertyChanged, CoerceValueProperty));
			MaximumProperty = DependencyPropertyManager.Register("Maximum", typeof(double), ownerType, new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.None,
				MaximumPropertyChanged));
			MinimumProperty = DependencyPropertyManager.Register("Minimum", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.None,
				MinimumPropertyChanged));
		}
		static TrackBarIncrementTargetEnum GetTargetFromObject(object parameter) {
			TrackBarIncrementTargetEnum result;
			if(parameter is string)
				Enum.TryParse<TrackBarIncrementTargetEnum>((string)parameter, out result);
			else
				result = parameter == null ? TrackBarIncrementTargetEnum.Value : (TrackBarIncrementTargetEnum)parameter;
			return result;
		}
		static object CoerceValueProperty(DependencyObject d, object value) {
			return ((RangeBaseEdit)d).CoerceValue(value);
		}
		static void OrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RangeBaseEdit)d).OrientationChanged((Orientation)e.NewValue);
		}
		static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RangeBaseEdit)d).ValuePropertyChanged((double)e.OldValue, (double)e.NewValue);
		}
		static void MaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RangeBaseEdit)d).MaximumChanged((double)e.NewValue);
		}
		static void MinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RangeBaseEdit)d).MinimumChanged((double)e.NewValue);
		}
		public RangeBaseEdit() {
			IncrementLargeCommand = DelegateCommandFactory.Create<object>(parameter => IncrementLargeInternal(GetTargetFromObject(parameter)), false);
			DecrementLargeCommand = DelegateCommandFactory.Create<object>(parameter => DecrementLargeInternal(GetTargetFromObject(parameter)), false);
			IncrementSmallCommand = DelegateCommandFactory.Create<object>(parameter => IncrementSmallInternal(GetTargetFromObject(parameter)), false);
			DecrementSmallCommand = DelegateCommandFactory.Create<object>(parameter => DecrementSmallInternal(GetTargetFromObject(parameter)), false);
			MinimizeCommand = DelegateCommandFactory.Create<object>(parameter => MinimizeInternal(GetTargetFromObject(parameter)), false);
			MaximizeCommand = DelegateCommandFactory.Create<object>(parameter => MaximizeInternal(GetTargetFromObject(parameter)), false);
		}
		internal new RangeBaseEditStrategyBase EditStrategy { get { return (RangeBaseEditStrategyBase)base.EditStrategy; } }
		internal RangeEditBasePanel Panel { get { return EditCore as RangeEditBasePanel; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("RangeBaseEditIncrementLargeCommand")]
#endif
		public ICommand IncrementLargeCommand { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("RangeBaseEditDecrementLargeCommand")]
#endif
		public ICommand DecrementLargeCommand { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("RangeBaseEditIncrementSmallCommand")]
#endif
		public ICommand IncrementSmallCommand { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("RangeBaseEditDecrementSmallCommand")]
#endif
		public ICommand DecrementSmallCommand { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("RangeBaseEditMinimizeCommand")]
#endif
		public ICommand MinimizeCommand { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("RangeBaseEditMaximizeCommand")]
#endif
		public ICommand MaximizeCommand { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("RangeBaseEditSmallStep")]
#endif
		public double SmallStep {
			get { return (double)GetValue(SmallStepProperty); }
			set { SetValue(SmallStepProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("RangeBaseEditLargeStep")]
#endif
		public double LargeStep {
			get { return (double)GetValue(LargeStepProperty); }
			set { SetValue(LargeStepProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("RangeBaseEditOrientation")]
#endif
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("RangeBaseEditValue")]
#endif
		public double Value {
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("RangeBaseEditMinimum")]
#endif
		public double Minimum {
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("RangeBaseEditMaximum")]
#endif
		public double Maximum {
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}
		void MinimizeInternal(object parameter) {
			if (BeginInplaceEditing())
				EditStrategy.Minimize(parameter);
		}
		void MaximizeInternal(object parameter) {
			if (BeginInplaceEditing())
				EditStrategy.Maximize(parameter);
		}
		void IncrementLargeInternal(object parameter) {
			if (BeginInplaceEditing())
				EditStrategy.IncrementLarge(parameter);
		}
		void DecrementLargeInternal(object parameter) {
			if (BeginInplaceEditing())
				EditStrategy.DecrementLarge(parameter);
		}
		void IncrementSmallInternal(object parameter) {
			if (BeginInplaceEditing())
				EditStrategy.IncrementSmall(parameter);
		}
		void DecrementSmallInternal(object parameter) {
			if (BeginInplaceEditing())
				EditStrategy.DecrementSmall(parameter);
		}
		public void Minimize() {
			EditStrategy.Minimize(null);
		}
		public void Maximize() {
			EditStrategy.Maximize(null);
		}
		public void IncrementSmall() {
			EditStrategy.IncrementSmall(null);
		}
		public void DecrementSmall() {
			EditStrategy.DecrementSmall(null);
		}
		public void IncrementLarge() {
			EditStrategy.IncrementLarge(null);
		}
		public void DecrementLarge() {
			EditStrategy.DecrementLarge(null);
		}
		public void Increment(double value) {
			EditStrategy.Increment(value);
		}
		public void Decrement(double value) {
			EditStrategy.Decrement(value);
		}
		protected virtual object CoerceValue(object value) {
			return EditStrategy.CoerceValue(RangeBaseEdit.ValueProperty, value);
		}
		protected virtual void ValuePropertyChanged(double oldValue, double value) {
			EditStrategy.ValuePropertyChanged(oldValue, value);
			RangeBaseEditAutomationPeer peer = (RangeBaseEditAutomationPeer)FrameworkElementAutomationPeer.FromElement(this);
			if (peer != null)
				peer.RaiseValuePropertyChangedEvent(oldValue, value);
		}
		protected virtual void MinimumChanged(double value) {
			EditStrategy.MinimumChanged(value);
		}
		protected virtual void MaximumChanged(double value) {
			EditStrategy.MaximumChanged(value);
		}
		protected virtual void OrientationChanged(Orientation orientation) {
			EditStrategy.OrientationChanged(orientation);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(Panel != null && IsPrintingMode)
				Panel.ForceLoaded();
		}
		#region ITextExportSettings Members
		HorizontalAlignment ITextExportSettings.HorizontalAlignment {
			get {
				if(HorizontalContentAlignment == System.Windows.HorizontalAlignment.Stretch)
					return ExportSettingDefaultValue.HorizontalAlignment;
				return HorizontalContentAlignment;
			}
		}
		VerticalAlignment ITextExportSettings.VerticalAlignment {
			get {
				if(VerticalContentAlignment == System.Windows.VerticalAlignment.Stretch)
					return ExportSettingDefaultValue.VerticalAlignment;
				return VerticalContentAlignment;
			}
		}
		string ITextExportSettings.Text { get { return GetExportText(); } }
		object ITextExportSettings.TextValue { get { return GetTextValueInternal(); } }
		string ITextExportSettings.TextValueFormatString { get { return DisplayFormatString; } }
		FontFamily ITextExportSettings.FontFamily { get { return FontFamily; } }
		FontStyle ITextExportSettings.FontStyle { get { return FontStyle; } }
		FontWeight ITextExportSettings.FontWeight { get { return FontWeight; } }
		double ITextExportSettings.FontSize { get { return FontSize; } }
		Thickness ITextExportSettings.Padding { get { return Padding; } }
		TextWrapping ITextExportSettings.TextWrapping { get { return ExportSettingDefaultValue.TextWrapping; } }
		bool ITextExportSettings.NoTextExport { get { return ExportSettingDefaultValue.NoTextExport; } }
		bool? ITextExportSettings.XlsExportNativeFormat { get { return ExportSettingDefaultValue.XlsExportNativeFormat; } }
		string ITextExportSettings.XlsxFormatString { get { return ExportSettingDefaultValue.XlsxFormatString; } }
		TextDecorationCollection ITextExportSettings.TextDecorations { get { return ExportSettingDefaultValue.TextDecorations; } }
		TextTrimming ITextExportSettings.TextTrimming { get { return ExportSettingDefaultValue.TextTrimming; } }
		protected virtual string GetExportText() {
			return PropertyProvider.DisplayText;
		}
		protected virtual object GetTextValueInternal() {
			return EditStrategy.IsNullValue(EditValue) ? null : EditValue;
		}
		protected int ToInt(double value) {
			return (int)Math.Round(value);
		}
		protected double GetPrintPosition() {
			return EditStrategy.GetNormalValue();
		}
		#endregion
	}
}
namespace DevExpress.Xpf.Editors.Internal {
	public class Fake { }
}
