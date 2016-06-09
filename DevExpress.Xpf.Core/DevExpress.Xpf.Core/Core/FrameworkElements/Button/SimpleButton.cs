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

using DevExpress.Utils;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System;
using System.Windows.Threading;
using System.Windows.Input;
namespace DevExpress.Xpf.Core {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	[ToolboxTabName(AssemblyInfo.DXTabWpfCommon)]
	public class SimpleButton : Button {
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty GlyphToContentOffsetProperty;
		public static readonly DependencyProperty ButtonKindProperty;
		public static readonly DependencyProperty IsCheckedProperty;
		public static readonly DependencyProperty DelayProperty;
		public static readonly DependencyProperty IntervalProperty;
		public static readonly DependencyProperty IsThreeStateProperty;
		public static readonly RoutedEvent CheckedEvent;
		public static readonly RoutedEvent UncheckedEvent;
		public static readonly RoutedEvent IndeterminateEvent;
		private DispatcherTimer timer;
		static SimpleButton() {
			GlyphProperty = DependencyPropertyManager.Register("Glyph", typeof(ImageSource), typeof(SimpleButton), new FrameworkPropertyMetadata(null));			
			GlyphToContentOffsetProperty = DependencyPropertyManager.Register("GlyphToContentOffset", typeof(double), typeof(SimpleButton), new FrameworkPropertyMetadata(null));
			ButtonKindProperty = DependencyPropertyManager.Register("ButtonKind", typeof(ButtonKind), typeof(SimpleButton), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnButtonKindChanged)));
			IsCheckedProperty = ToggleButton.IsCheckedProperty.AddOwner(typeof(SimpleButton), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsChackedChanged)));		   
			DelayProperty = RepeatButton.DelayProperty.AddOwner(typeof(SimpleButton), new FrameworkPropertyMetadata(RepeatButton.DelayProperty.GetMetadata(typeof(RepeatButton)).DefaultValue));
			IntervalProperty = RepeatButton.IntervalProperty.AddOwner(typeof(SimpleButton), new FrameworkPropertyMetadata(RepeatButton.IntervalProperty.GetMetadata(typeof(RepeatButton)).DefaultValue));
			IsThreeStateProperty = ToggleButton.IsThreeStateProperty.AddOwner(typeof(SimpleButton), new FrameworkPropertyMetadata(false));			
			CheckedEvent = ToggleButton.CheckedEvent.AddOwner(typeof(SimpleButton));
			UncheckedEvent = ToggleButton.CheckedEvent.AddOwner(typeof(SimpleButton));
			IndeterminateEvent = ToggleButton.IndeterminateEvent.AddOwner(typeof(SimpleButton));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SimpleButton), new FrameworkPropertyMetadata(typeof(SimpleButton)));
		}
		static object OnButtonKindChanged(DependencyObject d, object baseValue) {
			if (d is DropDownButton)
				return ButtonKind.Simple;
			return baseValue;
		}
		static void OnIsChackedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SimpleButton button = (SimpleButton)d;
			bool? oldValue = (bool?)e.OldValue;
			bool? newValue = (bool?)e.NewValue;
			if (newValue == true) {
				button.OnChecked(new RoutedEventArgs(CheckedEvent));
			} else if (newValue == false) {
				button.OnUnchecked(new RoutedEventArgs(UncheckedEvent));
			} else {
				button.OnIndeterminate(new RoutedEventArgs(IndeterminateEvent));
			}
		}
		protected override void OnClick() {
			if (ButtonKind == ButtonKind.Toggle) 
				OnToggle();
			base.OnClick();
		}
		void OnToggle() {
			bool? isChecked = null;
			if (IsChecked == true)
				isChecked = IsThreeState ? (bool?)null : (bool?)false;
			else
				isChecked = IsChecked.HasValue;
			SetCurrentValue(IsCheckedProperty, isChecked);
		}
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }				
		}	  
		public double GlyphToContentOffset {
			get { return (double)GetValue(GlyphToContentOffsetProperty); }
			set { SetValue(GlyphToContentOffsetProperty, value); }
		}
		public ButtonKind ButtonKind {
			get { return (ButtonKind)GetValue(ButtonKindProperty); }
			set { SetValue(ButtonKindProperty, value); }
		}
		public bool? IsChecked {
			get { return (bool?)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		public int Delay {
			get { return (int)GetValue(DelayProperty); }
			set { SetValue(DelayProperty, value); }
		}
		public int Interval {
			get { return (int)GetValue(IntervalProperty); } 
			set { SetValue(IntervalProperty, value); }
		}
		public bool IsThreeState {
			get { return (bool)GetValue(IsThreeStateProperty); }
			set { SetValue(IsThreeStateProperty, value); }
		}
		public event RoutedEventHandler Checked {
			add {
				AddHandler(CheckedEvent, value);
			}
			remove {
				RemoveHandler(CheckedEvent, value);
			}
		}
		public event RoutedEventHandler Unchecked {
			add {
				AddHandler(UncheckedEvent, value);
			}
			remove {
				RemoveHandler(UncheckedEvent, value);
			}
		}
		public event RoutedEventHandler Indeterminate {
			add {
				AddHandler(IndeterminateEvent, value);
			}
			remove {
				RemoveHandler(IndeterminateEvent, value);
			}
		}
		protected virtual void OnChecked(RoutedEventArgs e) {
			RaiseEvent(e);
		}		
		protected virtual void OnUnchecked(RoutedEventArgs e) {
			RaiseEvent(e);
		}		
		protected virtual void OnIndeterminate(RoutedEventArgs e) {
			RaiseEvent(e);
		}		
		protected void StartTimer() {
			if (timer == null) {
				timer = new DispatcherTimer();
				timer.Tick += new EventHandler(OnTimeout);
			} else if (timer.IsEnabled)
				return;
			timer.Interval = TimeSpan.FromMilliseconds(Delay);
			timer.Start();
		}
		protected void StopTimer() {
			if (timer != null) {
				timer.Stop();
			}
		}
		void OnTimeout(object sender, EventArgs e) {
			TimeSpan interval = TimeSpan.FromMilliseconds(Interval);
			if (timer.Interval != interval)
				timer.Interval = interval;
			if (IsPressed) {
				OnClick();
			}
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if (ButtonKind == ButtonKind.Repeat) {
				if (IsPressed) {
					StartTimer();
				}
			}
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			if (ButtonKind == ButtonKind.Repeat) 
				StopTimer();
		}
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			base.OnLostMouseCapture(e);
			if (ButtonKind == ButtonKind.Repeat)
				StopTimer();
		}
	}
}
