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

using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.Windows;
#if !NETFX_CORE
using System.Windows.Controls;
using System.Windows.Threading;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
#endif
namespace DevExpress.Mvvm.UI {
	[TargetType(typeof(UserControl)), TargetType(typeof(Window))]
	public class DispatcherService : ServiceBase, IDispatcherService {
		public static readonly DependencyProperty DelayProperty =
			DependencyProperty.Register("Delay", typeof(TimeSpan), typeof(DispatcherService),
			new PropertyMetadata(TimeSpan.Zero, (d, e) => ((DispatcherService)d).OnDelayChanged()));
		public TimeSpan Delay {
			get { return (TimeSpan)GetValue(DelayProperty); }
			set { SetValue(DelayProperty, value); }
		}
		TimeSpan delay;
#if NETFX_CORE
		public CoreDispatcherPriority DispatcherPriority { get; set; }
#elif !SILVERLIGHT
		public DispatcherPriority DispatcherPriority { get; set; }
		public DispatcherService() {
			DispatcherPriority = DispatcherPriority.Normal;
		}
#endif
		public void BeginInvoke(Action action) {
			if(delay == TimeSpan.Zero) {
				BeginInvokeCore(action);
				return;
			}
#if SILVERLIGHT || NETFX_CORE
			DispatcherTimer timer = new DispatcherTimer();
#else
			DispatcherTimer timer = new DispatcherTimer(DispatcherPriority, Dispatcher);
#endif
#if NETFX_CORE
			EventHandler<object> onTimerTick = null;
#else
			EventHandler onTimerTick = null;
#endif
			onTimerTick = (s, e) => {
				timer.Tick -= onTimerTick;
				timer.Stop();
				BeginInvokeCore(action);
			};
			timer.Tick += onTimerTick;
			timer.Interval = delay;
			timer.Start();
		}
		void BeginInvokeCore(Action action) {
#if SILVERLIGHT
			Dispatcher.BeginInvoke(action);
#elif !NETFX_CORE
			Dispatcher.BeginInvoke(action, DispatcherPriority, null);
#else
			Dispatcher.RunAsync(DispatcherPriority, new DispatchedHandler(action)).AsTask();
#endif
		}
		void OnDelayChanged() {
			delay = Delay;
		}
	}
}
