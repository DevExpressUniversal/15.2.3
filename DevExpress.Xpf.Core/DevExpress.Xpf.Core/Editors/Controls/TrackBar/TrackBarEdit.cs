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
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Settings;
#if !SL
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Printing;
using System.Windows.Media;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using FrameworkContentElement = System.Windows.FrameworkElement;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RepeatButton = DevExpress.Xpf.Editors.WPFCompatibility.SLRepeatButton;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public partial class TrackBarEdit : RangeBaseEdit, ITrackBarExportSettings {
		public static readonly DependencyProperty IsSnapToTickEnabledProperty;
		public static readonly DependencyProperty IsZoomProperty;
		static readonly DependencyPropertyKey IsZoomPropertyKey;
		public static readonly DependencyProperty IsRangeProperty;
		static readonly DependencyPropertyKey IsRangePropertyKey;
		public static readonly DependencyProperty TickPlacementProperty;
		public static readonly DependencyProperty TickFrequencyProperty;
		public static readonly DependencyProperty SelectionStartProperty;
		public static readonly DependencyProperty SelectionEndProperty;
		public static readonly DependencyProperty IsMoveToPointEnabledProperty;
		new TrackBarEditStrategy EditStrategy { get { return base.EditStrategy as TrackBarEditStrategy; } }
		static TrackBarEdit() {
			Type ownerType = typeof(TrackBarEdit);
#if !SL
			TickPlacementProperty = DependencyPropertyManager.Register("TickPlacement", typeof(TickPlacement), ownerType, new FrameworkPropertyMetadata(TickPlacement.BottomRight));
			TickFrequencyProperty = DependencyPropertyManager.Register("TickFrequency", typeof(double), ownerType, new FrameworkPropertyMetadata(5d));
			EditValueProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, null, true, UpdateSourceTrigger.PropertyChanged));
#endif
			IsMoveToPointEnabledProperty = DependencyPropertyManager.Register("IsMoveToPointEnabled", typeof(bool?), ownerType);
			IsSnapToTickEnabledProperty = DependencyPropertyManager.Register("IsSnapToTickEnabled", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, IsSnapToTickEnabledPropertyChanged));
			SelectionStartProperty = DependencyPropertyManager.Register("SelectionStart", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.None, SelectionStartPropertyChanged, CoerceSelectionStart));
			SelectionEndProperty = DependencyPropertyManager.Register("SelectionEnd", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.None, SelectionEndPropertyChanged, CoerceSelectionEnd));
			IsRangePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsRange", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsRangeProperty = IsRangePropertyKey.DependencyProperty;
			IsZoomPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsZoom", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsZoomProperty = IsZoomPropertyKey.DependencyProperty;
		}
		static object CoerceSelectionStart(DependencyObject d, object value) {
			return ((TrackBarEdit)d).EditStrategy.CoerceSelectionStart((double)value);
		}
		static object CoerceSelectionEnd(DependencyObject d, object value) {
			return ((TrackBarEdit)d).EditStrategy.CoerceSelectionEnd((double)value);
		}
		static void IsSnapToTickEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TrackBarEdit)d).IsSnapToTickEnabledChanged((bool)e.NewValue);
		}
		static void SelectionStartPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TrackBarEdit)d).SelectionStartChanged((double)e.OldValue, (double)e.NewValue);
		}
		static void SelectionEndPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TrackBarEdit)d).SelectionEndChanged((double)e.OldValue, (double)e.NewValue);
		}
#if !SL
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TrackBarEditTickPlacement")]
#endif
		public TickPlacement TickPlacement {
			get { return (TickPlacement)GetValue(TickPlacementProperty); }
			set { SetValue(TickPlacementProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TrackBarEditTickFrequency")]
#endif
		public double TickFrequency {
			get { return (double)GetValue(TickFrequencyProperty); }
			set { SetValue(TickFrequencyProperty, value); }
		}
#endif
		public bool? IsMoveToPointEnabled {
			get { return (bool?)GetValue(IsMoveToPointEnabledProperty); }
			set { SetValue(IsMoveToPointEnabledProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TrackBarEditIsSnapToTickEnabled")]
#endif
		public bool IsSnapToTickEnabled {
			get { return (bool)GetValue(IsSnapToTickEnabledProperty); }
			set { SetValue(IsSnapToTickEnabledProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TrackBarEditSelectionStart")]
#endif
		public double SelectionStart {
			get { return (double)GetValue(SelectionStartProperty); }
			set { SetValue(SelectionStartProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TrackBarEditSelectionEnd")]
#endif
		public double SelectionEnd {
			get { return (double)GetValue(SelectionEndProperty); }
			set { SetValue(SelectionEndProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TrackBarEditIsRange")]
#endif
		public bool IsRange {
			get { return (bool)GetValue(IsRangeProperty); }
			internal set { this.SetValue(IsRangePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TrackBarEditIsZoom")]
#endif
		public bool IsZoom {
			get { return (bool)GetValue(IsZoomProperty); }
			internal set { this.SetValue(IsZoomPropertyKey, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		[Browsable(true)]
		new public BaseEditStyleSettings StyleSettings { get { return base.StyleSettings; } set { base.StyleSettings = value; } }
		public TrackBarEdit() {
#if SILVERLIGHT
			ThemeLoadingTypeManager.RegisterType("DevExpress.Xpf.Editors.TrackBarEdit");
#endif
			this.SetDefaultStyleKey(typeof(TrackBarEdit));
		}
		protected internal override Type StyleSettingsType { get { return typeof(TrackBarStyleSettings); } }
		protected override EditStrategyBase CreateEditStrategy() {
			return new TrackBarEditStrategy(this);
		}
		protected virtual void SelectionStartChanged(double oldValue, double value) {
			EditStrategy.SelectionStartChanged(oldValue, value);
			TrackBarEditAutomationPeer peer = (TrackBarEditAutomationPeer)FrameworkElementAutomationPeer.FromElement(this);
			if (peer != null)
				peer.RaiseSelectionStartPropertyChangedEvent(oldValue, value);
		}
		protected virtual void IsSnapToTickEnabledChanged(bool value) {
			EditStrategy.IsSnapToTickEnabledChanged(value);
		}
		protected virtual void SelectionEndChanged(double oldValue, double value) {
			EditStrategy.SelectionEndChanged(oldValue, value);
			TrackBarEditAutomationPeer peer = (TrackBarEditAutomationPeer)FrameworkElementAutomationPeer.FromElement(this);
			if (peer != null)
				peer.RaiseSelectionEndPropertyChangedEvent(oldValue, value);
		}
		public void Minimize(TrackBarIncrementTargetEnum target) {
			EditStrategy.Minimize(target);
		}
		public void Maximize(TrackBarIncrementTargetEnum target) {
			EditStrategy.Maximize(target);
		}
		public void IncrementSmall(TrackBarIncrementTargetEnum target) {
			EditStrategy.IncrementSmall(target);
		}
		public void DecrementSmall(TrackBarIncrementTargetEnum target) {
			EditStrategy.DecrementSmall(target);
		}
		public void IncrementLarge(TrackBarIncrementTargetEnum target) {
			EditStrategy.IncrementLarge(target);
		}
		public void DecrementLarge(TrackBarIncrementTargetEnum target) {
			EditStrategy.DecrementLarge(target);
		}
		public void Increment(double value, TrackBarIncrementTargetEnum target) {
			EditStrategy.Increment(value, target);
		}
		public void Decrement(double value, TrackBarIncrementTargetEnum target) {
			EditStrategy.Decrement(value, target);
		}
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseDown(e);
			EditStrategy.PreviewMouseDown(e);
		}
		protected override void OnPreviewMouseUp(MouseButtonEventArgs e) {
			base.OnPreviewMouseUp(e);
			EditStrategy.PreviewMouseUp(e);
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new TrackBarEditPropertyProvider(this);
		}
		protected override string GetExportText() {
			return string.Format("{0}", GetTextValueInternal());
		}
#if !SL
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new TrackBarEditAutomationPeer(this);
		}
#endif
		#region ITrackBarExportSettings Members
		int ITrackBarExportSettings.Position {
			get { return ToInt(EditStrategy.GetRealValue(GetPrintPosition())); }
		}
		int ITrackBarExportSettings.Minimum {
			get { return ToInt(Minimum); }
		}
		int ITrackBarExportSettings.Maximum {
			get { return ToInt(Maximum); }
		}
		Color IExportSettings.Foreground {
			get { return Colors.DimGray; }
		}
		#endregion
	}
}
namespace DevExpress.Xpf.Editors.Controls {
	public partial class TrackBarEditThumbContentControl : MultiContentControl {
		public static readonly DependencyProperty IsDraggingProperty;
		static TrackBarEditThumbContentControl() {
			Type ownerType = typeof(TrackBarEditThumbContentControl);
			IsDraggingProperty = DependencyPropertyManager.Register("IsDragging", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((TrackBarEditThumbContentControl)d).PropertyChangedIsDragging()));
#if SL
			IsMouseOverPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsMouseOver", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((TrackBarEditThumbContentControl)d).PropertyChangedIsMouseOver()));
			IsMouseOverProperty = IsMouseOverPropertyKey.DependencyProperty;
#endif
		}
		public TrackBarEditThumbContentControl()
			: base() {
			IsEnabledChanged += OnIsEnabledChanged;
			Focusable = false;
		}
		public bool IsDragging {
			get { return (bool)GetValue(IsDraggingProperty); }
			set { SetValue(IsDraggingProperty, value); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualState(false);
		}
		protected virtual List<string> GetVisualStateNames() {
			List<string> result = new List<string>();
			if (!IsEnabled)
				result.Add("Disabled");
			else if (IsDragging)
				result.Add("IsDragging");
			else if (IsMouseOver)
				result.Add("IsMouseOver");
			else
				result.Add("Normal");
			return result;
		}
		protected virtual void OnIsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			UpdateVisualState(true);
		}
#if !SL
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if (e.Property == IsMouseOverProperty)
				UpdateVisualState(true);
		}
#endif
		protected virtual void PropertyChangedIsDragging() {
			UpdateVisualState(true);
		}
		protected void UpdateVisualState(bool useTransitions) {
			foreach (string visualStateName in GetVisualStateNames())
				if (VisualStateManager.GoToState(this, visualStateName, useTransitions)) break;
			if (!IsEnabled)
				VisibleChildIndex = 3;
			else if (IsDragging)
				VisibleChildIndex = 2;
			else if (IsMouseOver)
				VisibleChildIndex = 1;
			else
				VisibleChildIndex = 0;
		}
	}
}
namespace DevExpress.Xpf.Editors.Internal {
	public class TrackBarEditFocusElementVisibilityConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (((EditMode)value) == EditMode.Standalone) ? Visibility.Visible : Visibility.Collapsed;
		}
		object IValueConverter.ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new System.NotImplementedException();
		}
		#endregion
	}
}
