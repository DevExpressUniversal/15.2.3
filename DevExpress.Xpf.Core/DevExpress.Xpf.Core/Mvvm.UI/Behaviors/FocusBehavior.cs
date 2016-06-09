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

#if !FREE
using DevExpress.Xpf.Core.Native;
#else 
using DevExpress.Mvvm.UI.Native;
#endif
using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
#if !NETFX_CORE
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif
namespace DevExpress.Mvvm.UI {
	[TargetType(typeof(Control))]
	public class FocusBehavior : EventTriggerBase<Control> {
#if !SILVERLIGHT
		public readonly static TimeSpan DefaultFocusDelay = TimeSpan.FromMilliseconds(0);
#else
		public readonly static TimeSpan DefaultFocusDelay = TimeSpan.FromMilliseconds(500);
#endif
		public static readonly DependencyProperty FocusDelayProperty =
			DependencyProperty.Register("FocusDelay", typeof(TimeSpan?), typeof(FocusBehavior),
			new PropertyMetadata(null));
		public static readonly DependencyProperty PropertyNameProperty =
			DependencyProperty.Register("PropertyName", typeof(string), typeof(FocusBehavior),
			new PropertyMetadata(string.Empty));
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty PropertyValueProperty =
			DependencyProperty.Register("PropertyValue", typeof(object), typeof(FocusBehavior),
			new PropertyMetadata(null, (d, e) => ((FocusBehavior)d).OnPropertyValueChanged()));
		public TimeSpan? FocusDelay {
			get { return (TimeSpan?)GetValue(FocusDelayProperty); }
			set { SetValue(FocusDelayProperty, value); }
		}
		public string PropertyName {
			get { return (string)GetValue(PropertyNameProperty); }
			set { SetValue(PropertyNameProperty, value); }
		}
		bool lockPropertyValueChanged;
		protected override void OnEvent(object sender, object eventArgs) {
			base.OnEvent(sender, eventArgs);
			if(!string.IsNullOrEmpty(PropertyName)) return;
			DoFocus();
		}
		protected override void OnSourceChanged(object oldSource, object newSource) {
			base.OnSourceChanged(oldSource, newSource);
			lockPropertyValueChanged = true;
			ClearValue(PropertyValueProperty);
			lockPropertyValueChanged = false;
			if(!string.IsNullOrEmpty(PropertyName) && newSource != null) {
				lockPropertyValueChanged = true;
				BindingOperations.SetBinding(this, PropertyValueProperty,
					new Binding() { Path = new PropertyPath(PropertyName), Source = newSource, Mode = BindingMode.OneWay });
				lockPropertyValueChanged = false;
			}
		}
		void OnPropertyValueChanged() {
			if(lockPropertyValueChanged) return;
			DoFocus();
		}
		internal TimeSpan GetFocusDelay() {
			if(EventName == "Loaded")
				return FocusDelay ?? DefaultFocusDelay;
			else
				return FocusDelay ?? TimeSpan.FromMilliseconds(0);
		}
		void AssociatedObjectFocus() {
#if !NETFX_CORE
			AssociatedObject.Focus();
#else
			AssociatedObject.Focus(FocusState.Programmatic);
#endif
		}
		void DoFocus() {
			if(!IsAttached) return;
			var focusDelay = GetFocusDelay();
			if(focusDelay == TimeSpan.FromMilliseconds(0)) {
				AssociatedObjectFocus();
				return;
			}
			DispatcherTimer timer = new DispatcherTimer() {
				Interval = focusDelay,
			};
			timer.Tick += OnTimerTick;
			timer.Start();
		}
 #if NETFX_CORE
		void OnTimerTick(object sender, object e) {
#else
		void OnTimerTick(object sender, EventArgs e) {
#endif
			DispatcherTimer timer = (DispatcherTimer)sender;
			timer.Tick -= OnTimerTick;
			timer.Stop();
			AssociatedObjectFocus();
		}
	}
}
