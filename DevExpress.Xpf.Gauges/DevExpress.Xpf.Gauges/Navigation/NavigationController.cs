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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
namespace DevExpress.Xpf.Gauges.Native {
	public class NavigationController {
		readonly AnalogGaugeControl gauge;
		readonly HitTestController hitTestController;
		bool draggingInProcess = false;
		object selectedObject = null;
		TouchDevice touchDevice = null;
		Scale Scale { get { return selectedObject as Scale; } }
		ValueIndicatorBase Indicator { get { return selectedObject as ValueIndicatorBase; } } 
		public NavigationController(AnalogGaugeControl gauge) {
			this.gauge = gauge;
			hitTestController = new HitTestController(gauge);
		}
		void SelectObject(Point point) {
			foreach (IHitTestableElement element in hitTestController.FindElements(point)) {
				if (element.Element is Scale || element.Element is ValueIndicatorBase) {
					selectedObject = element.Element;
					break;
				}
			}
		}
		void ResetSelection() {
			gauge.ReleaseMouseCapture();
			draggingInProcess = false;
			selectedObject = null;
		}
		void StartDragging() {
			Mouse.Capture(gauge, CaptureMode.SubTree);
			draggingInProcess = true;
		}
		void MoveIndicatorsToPoint(Scale scale, MouseEventArgs e) {
			foreach (ValueIndicatorBase indicator in scale.Indicators)
				if (indicator.IsInteractive)
					MoveIndicatorToPoint(indicator, false, e);
		}
		void MoveIndicatorToPoint(ValueIndicatorBase indicator, bool shouldLockAnimation, MouseEventArgs e) {
			double? value = indicator.Scale.GetValueByMousePosition(e);
			if (value.HasValue) {
				try {
					if (shouldLockAnimation)
						indicator.IsLockedAnimation = true;
					indicator.SetValue(ValueIndicatorBase.ValueProperty, value.Value);
				}
				finally {
					indicator.IsLockedAnimation = false;
				}
			}
		}
		void ReleaseTouchDevice() {
			if (touchDevice != null) {
				TouchDevice previousTouchDevice = touchDevice;
				touchDevice = null;
				gauge.ReleaseTouchCapture(previousTouchDevice);
			}
		}
		void CaptureTouchDevice(TouchEventArgs e) {
			if (gauge.CaptureTouch(e.TouchDevice))
				touchDevice = e.TouchDevice;
		}
		public void MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			ResetSelection();
			SelectObject(e.GetPosition(gauge));
			if (Indicator != null && Indicator.IsInteractive) {
				StartDragging();
				e.Handled = true;
			}
		}
		public void MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			if (!draggingInProcess) {
				SelectObject(e.GetPosition(gauge));
				if (Scale != null) 
					MoveIndicatorsToPoint(Scale, e);
			}
			ResetSelection();
		}		
		public void MouseMove(object sender, MouseEventArgs e) {
			if (draggingInProcess) {
				e.Handled = true;
				MoveIndicatorToPoint(Indicator, true, e);
			}
		}
		public void MouseLeave(object sender, MouseEventArgs e) {
			ResetSelection();
		}
		public void PreviewTouchDown(object sender, TouchEventArgs e) {
			ReleaseTouchDevice();
			CaptureTouchDevice(e);
		}
		public void PreviewTouchUp(object sender, TouchEventArgs e) {
			ReleaseTouchDevice();
		}
		public void LostTouchCapture(object sender, TouchEventArgs e) {
			if (touchDevice != null)
				CaptureTouchDevice(e);
		}
	}
}
