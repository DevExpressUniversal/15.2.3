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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Gesture;
using DevExpress.Office.Layout;
using System.Collections.Generic;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetControl : IGestureClient {
		#region Fields
		GestureHelper gestureHelper;
		int panX;
		int panY;
		int overPanX;
		int overPanY;
		#endregion
		#region IGestureClient Members
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			gestureHelper.PanWithGutter = true;
			List<GestureAllowArgs> result = new List<GestureAllowArgs>();
			SpreadsheetBehaviorOptions behaviorOptions = Options.Behavior;
			if (!ClientBounds.Contains(point) || !behaviorOptions.TouchAllowed)
				return result.ToArray();
			if (behaviorOptions.ShowPopupMenuAllowed)
				result.Add(GestureAllowArgs.PressAndTap);
			if (behaviorOptions.ZoomingAllowed)
				result.Add(GestureAllowArgs.Zoom);
			Point physicalPoint = GetPhysicalPoint(point);
			SpreadsheetHitTestResult hitTestResult = ActiveView.CalculatePageHitTest(physicalPoint);
			bool isPanAllowed = true;
			if (hitTestResult != null) {
				HotZone hotZone = ActiveView.SelectionLayout.CalculateHotZone(hitTestResult.LogicalPoint);
				isPanAllowed = hitTestResult.PictureBox == null && hotZone == null;
			}
			if (isPanAllowed) {
				result.Add(GestureAllowArgs.Pan);
			}
			else {
				gestureHelper.PanWithGutter = false;
				GestureAllowArgs args = new GestureAllowArgs();
				args.GID = GestureAllowArgs.Pan.GID;
				args.AllowID = 0; 
				args.BlockID = GestureHelper.GC_PAN_ALL;
				result.Add(args);
				result.Add(GestureAllowArgs.Rotate);
			}
			(InnerControl as IGestureStateIndicator).OnGestureBegin();
			return result.ToArray();
		}
		IntPtr IGestureClient.Handle { get { return this.Handle; } }
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
			if (!Options.Behavior.TouchAllowed)
				return;
		}
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if (!Options.Behavior.TouchAllowed)
				return;
			if (info.IsBegin) {
				panX = 0;
				panY = 0;
				overPanX = 0;
				overPanY = 0;
			}
			if (delta.IsEmpty)
				return;
			BeginUpdate();
			try {
				if (delta.X != 0)
					ScrollHorizontally(delta.X, ref overPan);
				else
					ScrollVertically(delta.Y, ref overPan);
			}
			finally {
				EndUpdate();
			}
		}
		void ScrollHorizontally(int delta, ref Point overPan) {
			panX += delta;
			DocumentLayoutUnitConverter unitConverter = ActiveView.SelectionLayout.LayoutUnitConverter;
			if (ScrollByPhysicalOffset(new ScrollHorizontallyByPhysicalOffsetCommand(this), unitConverter.PixelsToLayoutUnits(panX, DpiX)))
				panX = 0;
			if ((ActiveView.IsHorizontalScrollMinimum() && delta > 0) || (ActiveView.IsHorizontalScrollMaximum() && delta < 0)) {
				overPanX += delta;
				overPan.X = overPanX;
			}
		}
		void ScrollVertically(int delta, ref Point overPan) {
			panY += delta;
			DocumentLayoutUnitConverter unitConverter = ActiveView.SelectionLayout.LayoutUnitConverter;
			if (ScrollByPhysicalOffset(new ScrollVerticallyByPhysicalOffsetCommand(this), unitConverter.PixelsToLayoutUnits(panY, DpiY)))
				panY = 0;
			if ((ActiveView.IsVerticalScrollMinimum() && delta > 0) || (ActiveView.IsVerticalScrollMaximum() && delta < 0)) {
				overPanY += delta;
				overPan.Y = overPanY;
			}
		}
		bool ScrollByPhysicalOffset(ScrollByPhysicalOffsetCommandBase command, int offset) {
			command.PhysicalOffset = offset;
			command.Execute();
			return command.ExecutedSuccessfully;
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) {
			if (!Options.Behavior.TouchAllowed)
				return;
			ShapeRotateCommand command = new ShapeRotateCommand(this);
			command.RotationAngle = (float)degreeDelta;
			command.Execute();
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs info) {
			if (!Options.Behavior.TouchAllowed)
				return;
		}
		[System.Security.SecuritySafeCritical]
		void IGestureClient.OnPressAndTap(GestureArgs info) {
			if (!Options.Behavior.TouchAllowed)
				return;
			Point screenPoint = this.PointToScreen(new Point(info.Start.X, info.Start.Y));
			Message msg = new Message();
			msg.LParam = new IntPtr((long)(screenPoint.X | (screenPoint.Y << 0x10)));
			OnWmContextMenu(ref msg);
		}
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
			if (!Options.Behavior.TouchAllowed)
				return;
			if (this.Options.Behavior.ZoomingAllowed)
				ActiveView.ZoomFactor *= (float)zoomDelta;
		}
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		Point IGestureClient.PointToClient(Point p) {
			return this.PointToClient(p);
		}
		#endregion
	}
}
