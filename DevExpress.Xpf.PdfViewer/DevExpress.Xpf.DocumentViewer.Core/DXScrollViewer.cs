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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Flyout;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Data;
using System.Globalization;
namespace DevExpress.Xpf.DocumentViewer {
	public class DXScrollViewer : ScrollViewer {
		public static readonly DependencyProperty CanMouseScrollProperty;
		static readonly TimeSpan FlyoutHideInterval = TimeSpan.FromMilliseconds(1000);
		static DXScrollViewer() {
			Type ownerType = typeof(DXScrollViewer);
			CanMouseScrollProperty = DependencyPropertyManager.Register("CanMouseScroll", typeof(bool), ownerType, 
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DXScrollViewer)obj).OnCanMouseScrollChanged((bool)args.NewValue)));
		}
		public bool CanMouseScroll {
			get { return (bool)GetValue(CanMouseScrollProperty); }
			set { SetValue(CanMouseScrollProperty, value); }
		}
		Locker ShowFlyoutLocker { get; set; }
		Point? PreviousPosition { get; set; }
		FlyoutControl FlyoutControl { get; set; }
		ScrollBar VerticalScrollBar { get; set; }
		DispatcherTimer HideFlyoutTimer { get; set; }
		public DXScrollViewer() {
			ShowFlyoutLocker = new Locker();
			HideFlyoutTimer = new DispatcherTimer();
			HideFlyoutTimer.Tick += OnHideFlyoutTimerTick;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UnsubscribeFromEvents();
			VerticalScrollBar = (ScrollBar)GetTemplateChild("PART_VerticalScrollBar");
			FlyoutControl = (FlyoutControl)GetTemplateChild("PART_FlyoutControl");
			SubscribeToEvents();
		}
		void OnHideFlyoutTimerTick(object sender, EventArgs e) {
			FlyoutControl.Do(x => x.IsOpen = false);
		}
		void OnVerticalScrollBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			ShowFlyoutLocker.LockOnce();
		}
		void OnVerticalScrollBarMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			ShowFlyoutLocker.Unlock();
		}
		void OnVerticalScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			if (ShowFlyoutLocker.IsLocked) {
				FlyoutControl.Do(x => x.IsOpen = true);
				HideFlyoutTimer.Stop();
				HideFlyoutTimer.Interval = FlyoutHideInterval;
				HideFlyoutTimer.Start();
			}
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			PreviousPosition = null;
			ReleaseMouseCapture();
			if (CanMouseScroll)
				e.Handled = true;
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if (!CanMouseScroll)
				return;
			PreviousPosition = e.GetPosition(this);
			CaptureMouse();
		}
		protected override void OnPreviewMouseMove(MouseEventArgs e) {
			base.OnPreviewMouseMove(e);
			if (!PreviousPosition.HasValue)
				return;
			Point currentPosition = e.GetPosition(this);
			double dx = currentPosition.X - PreviousPosition.Value.X;
			double dy = currentPosition.Y - PreviousPosition.Value.Y;
			ScrollToVerticalOffset(VerticalOffset - dy);
			ScrollToHorizontalOffset(HorizontalOffset - dx);
			PreviousPosition = currentPosition;
		}
		protected virtual void OnCanMouseScrollChanged(bool newValue) {
		}
		protected virtual void UnsubscribeFromEvents() {
			VerticalScrollBar.Do(x => x.ValueChanged -= OnVerticalScrollBarValueChanged);
			VerticalScrollBar.Do(x => x.PreviewMouseLeftButtonDown -= OnVerticalScrollBarMouseLeftButtonDown);
			VerticalScrollBar.Do(x => x.PreviewMouseLeftButtonUp -= OnVerticalScrollBarMouseLeftButtonUp);
		}
		protected virtual void SubscribeToEvents() {
			VerticalScrollBar.Do(x => x.ValueChanged += OnVerticalScrollBarValueChanged);
			VerticalScrollBar.Do(x => x.PreviewMouseLeftButtonDown += OnVerticalScrollBarMouseLeftButtonDown);
			VerticalScrollBar.Do(x => x.PreviewMouseLeftButtonUp += OnVerticalScrollBarMouseLeftButtonUp);
		}
	}
}
