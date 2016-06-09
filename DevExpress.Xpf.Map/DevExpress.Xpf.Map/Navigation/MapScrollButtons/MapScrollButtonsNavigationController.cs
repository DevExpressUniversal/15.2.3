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
using System.Windows.Input;
using System.Windows.Threading;
namespace DevExpress.Xpf.Map {
	public class MapScrollButtonsNavigationController {
		const double offsetFactor = 0.5;
		const double maxScrollOffset = 50.0;
		readonly MapScrollButtons scrollButtons;
		readonly DispatcherTimer eventTimer;
		bool draggingInProcess = false;
		Point scrollOffset = new Point(0, 0);
		internal bool DraggingInProcess { get { return draggingInProcess; } }
		public MapScrollButtonsNavigationController(MapScrollButtons scrollButtons) {
			this.scrollButtons = scrollButtons;
			eventTimer = new DispatcherTimer();
			eventTimer.Tick += new System.EventHandler(eventTimer_Tick);
			eventTimer.Interval = TimeSpan.FromMilliseconds(100.0);
		}
		void eventTimer_Tick(object sender, System.EventArgs e) {
			if (draggingInProcess)
				Move(scrollOffset);
		}
		void Reset() {
			draggingInProcess = false;
			eventTimer.Stop();
		}
		void Move(Point offset) {
			if (scrollButtons.CanExecuteCommand(offset))
				scrollButtons.ExecuteCommand(offset);
		}
		Point CalculateScrollOffset(Point mousePoint) {
			double centerX = scrollButtons.ActualWidth / 2;
			double centerY = scrollButtons.ActualHeight / 2;
			double offsetX = LimitedValue((mousePoint.X - centerX) * offsetFactor, maxScrollOffset);
			double offsetY = LimitedValue((mousePoint.Y - centerY) * offsetFactor, maxScrollOffset);
			return new Point(offsetX, offsetY);
		}
		double LimitedValue(double value, double limit) {
			return Math.Max(Math.Min(value, limit), -limit);
		}
		public void MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			scrollButtons.CaptureMouse();
			draggingInProcess = true;
			scrollOffset = CalculateScrollOffset(e.GetPosition(scrollButtons));
			eventTimer.Start();
			e.Handled = true;
			scrollButtons.UpdateVisualState();
		}
		public void MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			scrollButtons.ReleaseMouseCapture();
			Reset();
			e.Handled = true;
			scrollButtons.UpdateVisualState();
		}
		public void MouseMove(object sender, MouseEventArgs e) {
			if (draggingInProcess)
				scrollOffset = CalculateScrollOffset(e.GetPosition(scrollButtons));
		}
	}
}
