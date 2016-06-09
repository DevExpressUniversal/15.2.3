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
using System.ComponentModel;
using DevExpress.Mvvm.Native;
#if !SILVERLIGHT
using WindowBase = System.Windows.Window;
using System.Windows;
#else
using WindowBase = DevExpress.Xpf.Core.DXWindowBase;
#endif
#if !FREE
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Mvvm.UI.Native {
	public interface IWindowSurrogate {
		WindowBase RealWindow { get; }
		void Show();
		event CancelEventHandler Closing;
		event EventHandler Closed;
		event EventHandler Activated;
		event EventHandler Deactivated;
		bool? ShowDialog();
		void Close();
		bool Activate();
		void Hide();
	}
	public class WindowProxy : IWindowSurrogate {
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ActualWindowSurrogateProperty =
			DependencyProperty.RegisterAttached("ActualWindowSurrogate", typeof(IWindowSurrogate), typeof(WindowProxy), new PropertyMetadata(null));
		static IWindowSurrogate GetActualWindowSurrogate(DependencyObject obj) {
			return (IWindowSurrogate)obj.GetValue(ActualWindowSurrogateProperty);
		}
		static void SetActualWindowSurrogate(DependencyObject obj, IWindowSurrogate value) {
			obj.SetValue(ActualWindowSurrogateProperty, value);
		}
		public static IWindowSurrogate GetWindowSurrogate(object window) {
			IWindowSurrogate res = (window as DependencyObject).With(x => GetActualWindowSurrogate(x));
			if(res != null) 
				return res;
			res = window as IWindowSurrogate ?? new WindowProxy((WindowBase)window);
			(window as DependencyObject).Do(x => SetActualWindowSurrogate(x, res));
			return res;
		}
		public WindowProxy(WindowBase window) {
			if(window == null) throw new ArgumentNullException("window");
			RealWindow = window;
		}
		public WindowBase RealWindow { get; private set; }
		public void Show() {
			RealWindow.Show();
		}
		public void Close() {
			RealWindow.Close();
		}
		public bool? ShowDialog() {
			return RealWindow.ShowDialog();
		}
		public event CancelEventHandler Closing {
			add { RealWindow.Closing += value; }
			remove { RealWindow.Closing -= value; }
		}
		public event EventHandler Closed {
			add { RealWindow.Closed += value; }
			remove { RealWindow.Closed -= value; }
		}
		public event EventHandler Activated {
			add { RealWindow.Activated += value; }
			remove { RealWindow.Activated -= value; }
		}
		public event EventHandler Deactivated {
			add { RealWindow.Deactivated += value; }
			remove { RealWindow.Deactivated -= value; }
		}
		public bool Activate() {
			return RealWindow.Activate();
		}
		public void Hide() {
			RealWindow.Hide();
		}
	}
}
