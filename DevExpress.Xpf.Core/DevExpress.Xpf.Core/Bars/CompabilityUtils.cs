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

using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Windows;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Bars {
	public class DoubleClickImplementer {
		object owner;
		public DoubleClickImplementer(object owner) {
			this.owner = owner;
			this.doubleClickTimer = new System.Windows.Threading.DispatcherTimer();
			this.doubleClickTimer.Tick += OnDoubleClickTimerTick;
			SetDoubleClickTimerInterval(TimeSpan.FromSeconds(0.5));
		}
		public void SetDoubleClickTimerInterval(TimeSpan interval) {
			this.doubleClickTimer.Interval = interval;
		}
		public void OnMouseLeftButtonUpDoubleClickForce(MouseButtonEventArgs e, MouseButtonEventHandler mouseDoubleClickHandlerOverride, MouseButtonEventHandler mouseDoubleClickEvent) {
			if(this.isTimerWorking) {
				OnMouseDoubleClickCore(e, mouseDoubleClickHandlerOverride, mouseDoubleClickEvent);
				StopTimer();
			}
			StartTimer();
		}
		protected virtual void OnMouseDoubleClickCore(MouseButtonEventArgs e, MouseButtonEventHandler mouseDoubleClickHandlerOverride, MouseButtonEventHandler mouseDoubleClickEvent) {
			mouseDoubleClickHandlerOverride(this.owner, e);
			RaiseMouseDoubleClick(e, mouseDoubleClickEvent);
		}
		protected virtual void OnDoubleClickTimerTick(object sender, EventArgs e) {
			StopTimer();
		}
		void RaiseMouseDoubleClick(MouseButtonEventArgs e, MouseButtonEventHandler mouseDoubleClickEvent) {
			if(mouseDoubleClickEvent == null)
				return;
			mouseDoubleClickEvent(this, e);
		}
		void StartTimer() {
			if(this.isTimerWorking)
				return;
			this.doubleClickTimer.Start();
			this.isTimerWorking = true;
		}
		void StopTimer() {
			if(!this.isTimerWorking)
				return;
			this.doubleClickTimer.Stop();
			this.isTimerWorking = false;
		}
		System.Windows.Threading.DispatcherTimer doubleClickTimer;
		bool isTimerWorking = false;
	}
	public abstract class BarLayoutItemsControlBase : BarItemsControl {
		public static readonly DependencyProperty ContainerKeyProperty;
		public static readonly DependencyProperty OrientationProperty;
		static BarLayoutItemsControlBase() {
			ContainerKeyProperty = DependencyPropertyRegistrator.Register<BarLayoutItemsControlBase, object>(owner => owner.ContainerKey, null);
			OrientationProperty = DependencyPropertyRegistrator.Register<BarLayoutItemsControlBase, Orientation>(owner => owner.Orientation, Orientation.Horizontal);
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public object ContainerKey {
			get { return GetValue(ContainerKeyProperty); }
			set { SetValue(ContainerKeyProperty, value); }
		}
	}
	public class BarItemsControl : ItemsControl {
		protected virtual void SetIsTabStopCore() {
			IsTabStop = false;
			KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.None);
		}
		public BarItemsControl() {
			SetIsTabStopCore();
		}
	}
}
