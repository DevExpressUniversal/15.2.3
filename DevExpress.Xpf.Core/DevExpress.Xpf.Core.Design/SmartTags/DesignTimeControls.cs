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

extern alias Platform;
using DevExpress.Design.UI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Guard = Platform::DevExpress.Utils.Guard;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public class DesignTimePopup : Popup {
		#region Dependency Properties
		static readonly DependencyProperty IsOpenListenProperty =
			DependencyProperty.Register("IsOpenListen", typeof(bool), typeof(DesignTimePopup), new PropertyMetadata(false,
				(d, e) => ((DesignTimePopup)d).OnIsOpenChanged(e)));
		public static readonly DependencyProperty IsOpenExtProperty =
			DependencyProperty.Register("IsOpenExt", typeof(bool), typeof(DesignTimePopup), new PropertyMetadata(false,
				(d, e) => ((DesignTimePopup)d).OnIsOpenExtChanged(e)));
		public static readonly DependencyProperty ActualIsOpenProperty =
			DependencyProperty.Register("ActualIsOpen", typeof(bool), typeof(DesignTimePopup), new PropertyMetadata(false));
		#endregion
		DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.SystemIdle) { Interval = new TimeSpan(0, 0, 0, 0, 80) };
		public DesignTimePopup() {
			SetBinding(IsOpenListenProperty, new Binding("IsOpen") { Source = this, Mode = BindingMode.OneWay });
			timer.Tick += (s, e) => {
				timer.Stop();
				IsOpen = false;
			};
		}
		public bool IsOpenExt { get { return (bool)GetValue(IsOpenExtProperty); } set { SetValue(IsOpenExtProperty, value); } }
		public bool ActualIsOpen { get { return (bool)GetValue(ActualIsOpenProperty); } set { SetValue(ActualIsOpenProperty, value); } }
		protected virtual void OnIsOpenChanged(DependencyPropertyChangedEventArgs e) {
			IsOpenExt = (bool)e.NewValue;
		}
		protected virtual void OnIsOpenExtChanged(DependencyPropertyChangedEventArgs e) {
			if((bool)e.NewValue) {
				IsOpen = true;
			} else {
				if(timer.IsEnabled)
					timer.Stop();
				timer.Start();
			}
		}
		protected override void OnOpened(EventArgs e) {
			base.OnOpened(e);
			ActualIsOpen = true;
			Mouse.Capture(Child as IInputElement);
			Dispatcher.BeginInvoke((Action)ReleaseChildMouseCapture, DispatcherPriority.ContextIdle);
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			ActualIsOpen = false;
		}
		void ReleaseChildMouseCapture() {
			if(Mouse.Captured == Child)
				Mouse.Capture(null);
		}
	}
	public class DesignTimeButton : Button {
		#region Dependency Properties
		public static readonly DependencyProperty IsMouseOverExProperty =
			DependencyProperty.Register("IsMouseOverEx", typeof(bool), typeof(DesignTimeButton), new PropertyMetadata(false));
		#endregion
		public bool IsMouseOverEx { get { return (bool)GetValue(IsMouseOverExProperty); } set { SetValue(IsMouseOverExProperty, value); } }
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			IsMouseOverEx = true;
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			IsMouseOverEx = false;
		}
	}
	public class DesignTimeDelegateCommand : WpfDelegateCommand {
		public static DesignTimeDelegateCommand Create(Action executeMethod, DispatcherPriority? dispatcherPriority = null) {
			return Create(executeMethod, null, dispatcherPriority);
		}
		public static DesignTimeDelegateCommand Create(Action executeMethod, Func<bool> canExecuteMethod, DispatcherPriority? dispatcherPriority = null) {
			DesignTimeDelegateCommand instance = null;
			Action actualExecuteMethod = () => instance.InvokeExecuteMethod(executeMethod);
			instance = new DesignTimeDelegateCommand(actualExecuteMethod, canExecuteMethod ?? (() => true), dispatcherPriority);
			return instance;
		}
		DispatcherPriority? dispatcherPriority;
		protected DesignTimeDelegateCommand(Action executeMethod, Func<bool> canExecuteMethod, DispatcherPriority? dispatcherPriority)
			: base(executeMethod, canExecuteMethod, false) {
			this.dispatcherPriority = dispatcherPriority;
		}
		void InvokeExecuteMethod(Action executeMethod) {
			if(dispatcherPriority == null)
				executeMethod();
			else
				Dispatcher.CurrentDispatcher.BeginInvoke(executeMethod, dispatcherPriority.Value);
		}
	}
	public class DesignTimeAdaptablePanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			double width = double.IsInfinity(availableSize.Width) ? 0.0 : availableSize.Width;
			double height = double.IsInfinity(availableSize.Height) ? 0.0 : availableSize.Height;
			foreach(UIElement child in Children) {
				child.Measure(availableSize);
				if(child.DesiredSize.Width > width)
					width = child.DesiredSize.Width;
				if(child.DesiredSize.Height > height)
					height = child.DesiredSize.Height;
			}
			return new Size(width, height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach(UIElement child in Children) {
				child.Arrange(new Rect(new Point(), finalSize));
			}
			return finalSize;
		}
	}
}
