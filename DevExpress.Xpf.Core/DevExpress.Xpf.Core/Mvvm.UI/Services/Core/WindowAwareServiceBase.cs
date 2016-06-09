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
using System.Windows;
using DevExpress.Mvvm.Native;
namespace DevExpress.Mvvm.UI {
	public abstract class WindowAwareServiceBase : ServiceBase {
		public static readonly DependencyProperty WindowSourceProperty =
			DependencyProperty.Register("WindowSource", typeof(FrameworkElement), typeof(WindowAwareServiceBase), new PropertyMetadata(null,
				(d, e) => ((WindowAwareServiceBase)d).OnWindowSourceChanged(e)));
		public static readonly DependencyProperty WindowProperty =
			DependencyProperty.Register("Window", typeof(Window), typeof(WindowAwareServiceBase), new PropertyMetadata(null,
				(d, e) => ((WindowAwareServiceBase)d).OnWindowChanged(e)));
		static readonly DependencyPropertyKey ActualWindowPropertyKey =
			DependencyProperty.RegisterReadOnly("ActualWindow", typeof(Window), typeof(WindowAwareServiceBase), new PropertyMetadata(null,
				(d, e) => ((WindowAwareServiceBase)d).OnActualWindowChanged((Window)e.OldValue)));
		public static readonly DependencyProperty ActualWindowProperty = ActualWindowPropertyKey.DependencyProperty;
		public FrameworkElement WindowSource {
			get { return (FrameworkElement)GetValue(WindowSourceProperty); }
			set { SetValue(WindowSourceProperty, value); }
		}
		public Window Window {
			get { return (Window)GetValue(WindowProperty); }
			set { SetValue(WindowProperty, value); }
		}
		public Window ActualWindow {
			get { return (Window)GetValue(ActualWindowProperty); }
			private set { SetValue(ActualWindowPropertyKey, value); }
		}
		protected abstract void OnActualWindowChanged(Window oldWindow);
		void OnWindowChanged(DependencyPropertyChangedEventArgs e) {
			UpdateActualWindow();
		}
		void OnWindowSourceIsLoadedChanged(object sender, RoutedEventArgs e) {
			UpdateActualWindow();
		}
		void UpdateActualWindow() {
			ActualWindow = Window ?? WindowSource.With(Window.GetWindow) ?? AssociatedObject.With(Window.GetWindow);
		}
		void OnWindowSourceChanged(DependencyPropertyChangedEventArgs e) {
			var oldValue = (FrameworkElement)e.OldValue;
			var newValue = (FrameworkElement)e.NewValue;
			if(oldValue != null)
				Detach(oldValue);
			if(newValue != null)
				Attach(newValue);
			UpdateActualWindow();
		}
		void Attach(FrameworkElement windowSource) {
			windowSource.Loaded += OnWindowSourceIsLoadedChanged;
			windowSource.Unloaded += OnWindowSourceIsLoadedChanged;
		}
		void Detach(FrameworkElement windowSource) {
			windowSource.Loaded -= OnWindowSourceIsLoadedChanged;
			windowSource.Unloaded -= OnWindowSourceIsLoadedChanged;
		}
		protected override void OnAttached() {
			base.OnAttached();
			Attach(AssociatedObject);
			UpdateActualWindow();
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			Detach(AssociatedObject);
			UpdateActualWindow();
		}
	}
}
