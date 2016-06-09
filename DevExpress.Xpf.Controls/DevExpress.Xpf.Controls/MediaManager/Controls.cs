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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using System.ComponentModel;
namespace DevExpress.Xpf.Controls {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class HorizontalControlPanel : Control {
		public static readonly DependencyProperty ChaptersVisibilityProperty =
			DependencyProperty.Register("ChaptersVisibility", typeof(Visibility), typeof(HorizontalControlPanel), new PropertyMetadata(Visibility.Visible));
		public static readonly DependencyProperty ChaptersDisplayTypeProperty =
			DependencyProperty.Register("ChaptersDisplayType", typeof(ChaptersDisplayType), typeof(HorizontalControlPanel), new PropertyMetadata(ChaptersDisplayType.Text));
		public ChaptersDisplayType ChaptersDisplayType {
			get { return (ChaptersDisplayType)GetValue(ChaptersDisplayTypeProperty); }
			set { SetValue(ChaptersDisplayTypeProperty, value); }
		}
		public Visibility ChaptersVisibility {
			get { return (Visibility)GetValue(ChaptersVisibilityProperty); }
			set { SetValue(ChaptersVisibilityProperty, value); }
		}
		public bool SliderIsBottom {
			get { return _controlsIsBottom; }
			set {
				if(_controlsIsBottom == value) return;
				_controlsIsBottom = value;
				ChangeControlsPosition();
			}
		}
		public HorizontalControlPanel() {
			DefaultStyleKey = typeof(HorizontalControlPanel);
			HorizontalContentAlignment = HorizontalAlignment.Center;
			VerticalContentAlignment = VerticalAlignment.Center;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			panel = (StackPanel)FindControlHelper.FindChildControlByType(this, typeof(StackPanel));
			slider = (MediaSliderControl)FindControlHelper.FindChildControlByType(this, typeof(MediaSliderControl));
			ChangeControlsPosition();
		}
		void ChangeControlsPosition() {
			if(panel == null || slider == null) return;
			if(SliderIsBottom) {
				System.Windows.Controls.Grid.SetRow(panel, 0);
				System.Windows.Controls.Grid.SetRow(slider, 1);
			} else {
				System.Windows.Controls.Grid.SetRow(panel, 1);
				System.Windows.Controls.Grid.SetRow(slider, 0);
			}
		}
		StackPanel panel;
		MediaSliderControl slider;
		bool _controlsIsBottom;
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class VerticalControlPanel : Control {
		public VerticalControlPanel() {
			DefaultStyleKey = typeof(VerticalControlPanel);
			HorizontalContentAlignment = HorizontalAlignment.Center;
			VerticalContentAlignment = VerticalAlignment.Center;
		}
	}
	#region MediaSlider
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public abstract class MediaSlider : Slider {
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty MediaManagerProperty = DependencyProperty.Register("MediaManager", typeof(MediaManager), typeof(MediaSlider), new PropertyMetadata(null));
		public MediaManager MediaManager {
			get {
				return (MediaManager)base.GetValue(MediaManagerProperty);
			}
			internal set {
				base.SetValue(MediaManagerProperty, value);
			}
		}
		protected MediaElement MediaElement { get { return ((this.MediaManager == null) ? null : this.MediaManager.MediaElement); } }
		public MediaSlider() {
			Loaded += OnLoaded;
#if WPF
			IsMoveToPointEnabled = true;
#endif
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			InitializeMediaManager();
			InitializeBindings();
		}
		protected virtual void InitializeBindings() { }
		void InitializeMediaManager() {
			MediaManager = FindControlHelper.GetMediaManager(VisualTreeHelper.GetParent(this));
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class BalanceMediaSlider : MediaSlider {
		public BalanceMediaSlider() {
			DefaultStyleKey = typeof(BalanceMediaSlider);
			Minimum = -1.0;
			Maximum = 1.0;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
		}
		protected override void OnValueChanged(double oldValue, double newValue) {
			base.OnValueChanged(oldValue, newValue);
#if WPF
			ToolTip = string.Format("{0:0.0}", newValue);
#endif
		}
		protected override void InitializeBindings() {
			base.InitializeBindings();
			Binding binding = new Binding {
				Source = base.MediaElement,
				Path = new PropertyPath("Balance"),
				Mode = BindingMode.TwoWay
			};
			SetBinding(RangeBase.ValueProperty, binding);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class VolumeMediaSlider : MediaSlider {
		public VolumeMediaSlider() {
			Minimum = 0.0;
			Value = 0.5;
			Maximum = 1.0;
		}
		protected override void OnValueChanged(double oldValue, double newValue) {
			base.OnValueChanged(oldValue, newValue);
#if WPF
			ToolTip = string.Format("{0:0}", newValue * 100.0);
#endif
		}
		protected override void InitializeBindings() {
			Binding binding = new Binding {
				Source = base.MediaElement,
				Path = new PropertyPath("Volume"),
				Mode = BindingMode.TwoWay
			};
			SetBinding(RangeBase.ValueProperty, binding);
		}
	}
	#endregion
	#region MediaButton
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public abstract class MediaButton : Button {
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty MediaManagerProperty = DependencyProperty.Register("MediaManager", typeof(MediaManager), typeof(MediaButton), new PropertyMetadata(null));
		public MediaManager MediaManager {
			get { return (MediaManager)base.GetValue(MediaManagerProperty); }
			internal set { base.SetValue(MediaManagerProperty, value); }
		}
		protected MediaElement MediaElement { get { return ((MediaManager == null) ? null : MediaManager.MediaElement); } }
		public MediaButton() {
			Loaded += OnLoaded;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			if(MediaManager == null) {
				InitializeMediaManager();
			}
			if(MediaElement != null) {
				MediaManager.SelectedIndexChanged += OnMediaManagerSelectedIndexChanged;
				MediaManager.MediaList.CollectionChanged += OnMediaListCollectionChanged;
			}
			IsEnabled = SetEnabled();
		}
		protected virtual void OnMediaListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			IsEnabled = SetEnabled();
		}
		protected virtual void OnMediaManagerSelectedIndexChanged(object sender, SelectedIndexChangedEventArgs e) {
			IsEnabled = SetEnabled();
		}
		protected virtual bool SetEnabled() {
			if(MediaElement == null) {
				return false;
			} else {
				return MediaManager.MediaList.Count != 0;
			}
		}
		void InitializeMediaManager() { MediaManager = FindControlHelper.GetMediaManager(VisualTreeHelper.GetParent(this)); }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class NextMediaButton : MediaButton {
		public NextMediaButton() {
			Click += OnClick;
		}
		public virtual void OnClick(object sender, RoutedEventArgs e) {
			MediaManager.Next();
		}
		protected override bool SetEnabled() {
			bool result = base.SetEnabled();
			return result ? MediaManager.SelectedIndex < (MediaManager.MediaList.Count - 1) : result;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class PreviousMediaButton : MediaButton {
		public PreviousMediaButton() {
			Click += OnClick;
		}
		public virtual void OnClick(object sender, RoutedEventArgs e) {
			MediaManager.Previous();
		}
		protected override bool SetEnabled() {
			bool result = base.SetEnabled();
			return result ? MediaManager.SelectedIndex > 0 : result;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class StopMediaButton : MediaButton {
		public StopMediaButton() {
			DefaultStyleKey = typeof(StopMediaButton);
			Click += OnClick;
		}
		public virtual void OnClick(object sender, RoutedEventArgs e) {
			MediaManager.Stop();
		}
		protected override bool SetEnabled() {
			bool result = base.SetEnabled();
			return result ? base.MediaElement.Source != null : result;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class PositionInfoButton : MediaButton {
		public PositionInfoButton() {
			Click += OnClick;
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			InitializeBindings();
		}
		protected virtual void OnClick(object sender, RoutedEventArgs e) {
			if(MediaManager == null) return;
			switch(MediaManager.PositionInfoType) {
				case PositionInfoType.PositionDuration:
					MediaManager.PositionInfoType = PositionInfoType.Position;
					break;
				case PositionInfoType.Position:
					MediaManager.PositionInfoType = PositionInfoType.DurationLeft;
					break;
				case PositionInfoType.DurationLeft:
					MediaManager.PositionInfoType = PositionInfoType.PositionDuration;
					break;
			}
		}
		void InitializeBindings() {
			if(MediaManager == null) return;
			Binding bind = new Binding();
			bind.Source = MediaManager;
			bind.Path = new PropertyPath("PositionInfoText");
			SetBinding(ContentProperty, bind);
		}
	}
	#endregion
	#region ToggleButton
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public abstract class MediaToggleButton : ToggleButton {
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty MediaManagerProperty = DependencyProperty.Register("MediaManager", typeof(MediaManager), typeof(MediaToggleButton),
			new PropertyMetadata(null));
		public MediaManager MediaManager {
			get { return (MediaManager)base.GetValue(MediaManagerProperty); }
			internal set { base.SetValue(MediaManagerProperty, value); }
		}
		protected MediaElement MediaElement { get { return ((this.MediaManager == null) ? null : this.MediaManager.MediaElement); } }
		public MediaToggleButton() {
			Loaded += new RoutedEventHandler(this.OnLoaded);
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			InitializeMediaManager();
		}
		void InitializeMediaManager() {
			MediaManager = FindControlHelper.GetMediaManager(VisualTreeHelper.GetParent(this));
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class PlayPauseControl : MediaToggleButton {
		public PlayPauseControl() {
			DefaultStyleKey = typeof(PlayPauseControl);
			Click += OnClick;
			Loaded += OnLoaded;
			IsThreeState = false;
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			if(MediaManager != null) {
				MediaManager.CurrentStateChanged += OnMediaManagerCurrentStateChanged;
			}
			InitializeIsChecked();
		}
		protected virtual void OnClick(object sender, RoutedEventArgs e) {
			if(MediaElement != null) {
				if(IsChecked.Value) {
					MediaManager.Play();
				} else {
					MediaManager.Pause();
				}
				InitializeIsChecked();
			}
		}
		protected virtual void OnMediaManagerCurrentStateChanged(object sender, CurrentStateChangedEventArgs e) {
			InitializeIsChecked();
		}
		void InitializeIsChecked() {
			if(MediaManager != null && MediaManager.CurrentState == MediaManagerState.Playing) {
				IsChecked = true;
			} else {
				IsChecked = false;
			}
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class IsMutedControl : MediaToggleButton {
		public IsMutedControl() {
			DefaultStyleKey = typeof(IsMutedControl);
			Loaded += OnLoaded;
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			if(MediaElement != null) {
				Binding binding = new Binding {
					Source = MediaElement,
					Path = new PropertyPath("IsMuted"),
					Mode = BindingMode.TwoWay
				};
				SetBinding(ToggleButton.IsCheckedProperty, binding);
			}
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class FullscreenControl : MediaToggleButton {
		public FullscreenControl() {
			DefaultStyleKey = typeof(FullscreenControl);
			Click += OnClick;
			IsThreeState = false;
			Loaded += OnLoaded;
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			if(MediaManager != null) {
				MediaManager.IsFullscreenChanged += OnMediaManagerIsFullscreenChanged;
				if(base.MediaElement != null) {
					MediaElement.MediaOpened += OnMediaElementMediaOpened;
					MediaElement.MediaFailed += OnMediaElementMediaFailed;
				}
			}
		}
		protected virtual void OnClick(object sender, RoutedEventArgs e) {
			MediaManager.IsFullscreen = IsChecked.Value;
		}
		protected virtual void OnMediaElementMediaFailed(object sender, ExceptionRoutedEventArgs e) {
			SetVisibility();
		}
		protected virtual void OnMediaElementMediaOpened(object sender, RoutedEventArgs e) {
			SetVisibility();
		}
		protected virtual void OnMediaManagerIsFullscreenChanged(object sender, IsFullscreenChangedEventArgs e) {
			IsChecked = new bool?(e.IsFullscreen);
		}
		void SetVisibility() {
#if WPF
			if(!MediaElement.HasVideo) {
				MediaManager.IsFullscreen = false;
				Visibility = Visibility.Collapsed;
			} else {
				Visibility = Visibility.Visible;
			}
#endif
		}
	}
	#endregion
}
