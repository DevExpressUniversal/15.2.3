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
namespace DevExpress.Xpf.Map {
	public class MiniMapNavigationController {
		readonly MiniMap miniMap;
		bool draggingInProcess = false;
		bool moved;
		Point startPoint;
		double mouseWheelDeltaStore;
		public MiniMapNavigationController(MiniMap miniMap) {
			this.miniMap = miniMap;
			miniMap.MouseLeftButtonUp += new MouseButtonEventHandler(MouseLeftButtonUp);
			miniMap.MouseLeftButtonDown += new MouseButtonEventHandler(MouseLeftButtonDown);
			miniMap.MouseMove += new MouseEventHandler(MouseMove);
			miniMap.MouseWheel += new MouseWheelEventHandler(MouseWheel);
			miniMap.TouchDown += TouchDown;
			miniMap.TouchUp += TouchUp;
		}
		void MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			OnClickDown(e.GetPosition(miniMap));
			e.Handled = true;
		}
		void MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			OnClickUp(e.GetPosition(miniMap));
			e.Handled = true;
		}
		void TouchUp(object sender, TouchEventArgs e) {
			OnClickUp(e.GetTouchPoint(miniMap).Position);
			e.Handled = true;
		}
		void TouchDown(object sender, TouchEventArgs e) {
			OnClickDown(e.GetTouchPoint(miniMap).Position);
			e.Handled = true;
		}
		void OnClickUp(Point point) {
			if(miniMap.CanScroll) {
				miniMap.ReleaseMouseCapture();
				draggingInProcess = false;
				if(!moved && miniMap.SetMapCenterOnClick)
					miniMap.SetCenterPointInPixels(point);
			}
		}
		void OnClickDown(Point point) {
			if(miniMap.CanScroll) {
				miniMap.CaptureMouse();
				moved = false;
				draggingInProcess = true;
				this.startPoint = point;
				if(miniMap.ActualBehavior.Center != null)
					miniMap.SetCenterPointInPixels(point);
			}
		}
		int UpdateWheelDelta(double delta) {
			mouseWheelDeltaStore += delta;
			double absValue = Math.Abs(mouseWheelDeltaStore);
			int quants = (int)Math.Floor(absValue) * Math.Sign(mouseWheelDeltaStore);
			mouseWheelDeltaStore -= quants;
			return quants;
		}
		void MouseWheel(object sender, MouseWheelEventArgs e) {
			if (miniMap.CanZoom) {
				double zoomOffset = UpdateWheelDelta((double)e.Delta / (double)120);
				if (zoomOffset != 0) {
					mouseWheelDeltaStore = 0.0f;
					miniMap.Zoom(zoomOffset);
				}
			}
			e.Handled = true;
		}
		void MouseMove(object sender, MouseEventArgs e) {
			if (draggingInProcess) {
				Point point = e.GetPosition(miniMap);
				if (point != startPoint) {
					moved = true;
					if (miniMap.ActualBehavior.Center != null)
						miniMap.SetCenterPointInPixels(e.GetPosition(miniMap));
					else
						miniMap.Move(new Point(startPoint.X - point.X, startPoint.Y - point.Y));
					startPoint = point;
				}
			}
		}
	}
}
