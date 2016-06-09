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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Grid {
	public partial class ProgressControl : UserControl {
		DispatcherTimer stopTimer;
		ManualResetEvent stopEvent;
		bool shutdownDispatcher;
		DXWindow progressWindow;
		void Stop() {
			stopTimer.Stop();
#if !SL
			ProgressWindow.Close();
			if(shutdownDispatcher) {
				Dispatcher.InvokeShutdown();
			}
#else
			ProgressWindow.Hide();
#endif
		}
		void stopTimer_Tick(object sender, EventArgs e) {
			if(stopTimer.IsEnabled && (stopEvent != null) && stopEvent.WaitOne(1)) {
				Stop();
			}
		}
		void btnCancel_Click(object sender, RoutedEventArgs e) {
			Stop();
#if !SL
			e.Handled = true;
#endif
		}
		public ProgressControl(DXWindow progressWindow, ManualResetEvent stopEvent, bool shutdownDispatcher, string cancelCaption) {
			InitializeComponent();
			this.progressWindow = progressWindow;
			this.progressWindow.Closed += (s, e) => { stopTimer.Stop(); };
			this.stopEvent = stopEvent;
			this.shutdownDispatcher = shutdownDispatcher;
			btnCancel.Content = cancelCaption;
#if !SL
			stopTimer = new DispatcherTimer(DispatcherPriority.Background);
#else
			stopTimer = new DispatcherTimer();
#endif
			stopTimer.Interval = TimeSpan.FromMilliseconds(100.0);
			stopTimer.Tick += new EventHandler(stopTimer_Tick);
			stopTimer.Start();
		}
		public DXWindow ProgressWindow { get { return progressWindow; } }
		public static DXWindow CreateProgressWindow(ManualResetEvent stopEvent, bool shutdownDispatcher, string title, string cancelCaption) {
			DXWindow progressWindow = new DXWindow();
			progressWindow.Content = new ProgressControl(progressWindow, stopEvent, shutdownDispatcher, cancelCaption);
			progressWindow.Title = title;
			progressWindow.Width = 500;
#if !SL
			progressWindow.SizeToContent = SizeToContent.Height;
			progressWindow.ShowIcon = false;
			progressWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			progressWindow.WindowStyle = System.Windows.WindowStyle.ToolWindow;
#else
			progressWindow.Height = 82;
#endif
			return progressWindow;
		}
	}
}
