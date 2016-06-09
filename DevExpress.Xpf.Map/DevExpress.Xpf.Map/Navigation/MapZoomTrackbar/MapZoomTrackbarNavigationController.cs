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
	public abstract class MapZoomTrackbarNavigationController {
		readonly MapZoomTrackbar zoomTrackbar;
		bool draggingInProcess = false;
		protected MapZoomTrackbar ZoomTrackbar { get { return zoomTrackbar; } }
		public MapZoomTrackbarNavigationController(MapZoomTrackbar zoomTrackbar) {
			this.zoomTrackbar = zoomTrackbar;
		}
		void Reset() {
			draggingInProcess = false;
		}
		void Zoom(double zoomLevel) {
			zoomTrackbar.ExecuteCommand(zoomLevel);
		}
		protected abstract double CalculatePosition(Point mousePosition);
		protected internal void ZoomIn() {
			double newZoomLevel = zoomTrackbar.ZoomLevel + zoomTrackbar.ZoomingStep;
			Zoom(newZoomLevel < zoomTrackbar.MaxZoomLevel ? newZoomLevel : zoomTrackbar.MaxZoomLevel);
		}
		protected internal void ZoomOut() {
			double newZoomLevel = zoomTrackbar.ZoomLevel - zoomTrackbar.ZoomingStep;
			Zoom(newZoomLevel > zoomTrackbar.MinZoomLevel ? newZoomLevel : zoomTrackbar.MinZoomLevel);
		}
		public void MouseZoomInLeftButtonDown(object sender, MouseButtonEventArgs e) {
			ZoomIn();
			e.Handled = true;
		}
		public void MouseZoomOutLeftButtonDown(object sender, MouseButtonEventArgs e) {
			ZoomOut();
			e.Handled = true;
		}
		public void MouseTrackBarLeftButtonDown(object sender, MouseButtonEventArgs e) {
			zoomTrackbar.TrackBar.CaptureMouse();
			draggingInProcess = true;
			double position = CalculatePosition(e.GetPosition(zoomTrackbar.TrackBar));
			Zoom(zoomTrackbar.CalculateZoomLevel(position));
			e.Handled = true;
		}
		public void TrackBarMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			zoomTrackbar.TrackBar.ReleaseMouseCapture();
			Reset();
			e.Handled = true;
		}
		public void TrackBarMouseMove(object sender, MouseEventArgs e) {
			if (draggingInProcess) {
				double position = CalculatePosition(e.GetPosition(zoomTrackbar.TrackBar));
				Zoom(zoomTrackbar.CalculateZoomLevel(position));
			}
		}
	}
	public class MapVerticalZoomTrackbarNavigationController : MapZoomTrackbarNavigationController {
		public MapVerticalZoomTrackbarNavigationController(MapZoomTrackbar zoomTrackbar)
			: base(zoomTrackbar) {
		}
		protected override double CalculatePosition(Point mousePosition) {
			return ZoomTrackbar.TrackBar.RenderSize.Height - mousePosition.Y - ZoomTrackbar.Thumb.RenderSize.Height / 2;
		}
	}
	public class MapHorizontalZoomTrackbarNavigationController : MapZoomTrackbarNavigationController {
		public MapHorizontalZoomTrackbarNavigationController(MapZoomTrackbar zoomTrackbar)
			: base(zoomTrackbar) {
		}
		protected override double CalculatePosition(Point mousePosition) {
			return mousePosition.X - ZoomTrackbar.Thumb.RenderSize.Width / 2;
		}
	}
}
