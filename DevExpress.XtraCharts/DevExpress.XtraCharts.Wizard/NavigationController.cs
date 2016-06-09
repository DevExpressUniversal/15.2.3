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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Gesture;
namespace DevExpress.XtraCharts.Native {
	public class ChartNavigationController : ChartNavigationControllerBase {
		public ChartNavigationController(IChartContainer container)
			: base(container) {
		}
		protected override Cursor GetCursorByType(CursorType cursorType) {
			switch (cursorType) {
				case CursorType.Move:
					return DragCursors.MoveCursor;
				case CursorType.Grab:
					return DragCursors.HandDragCursor;
				case CursorType.Hand:
					return DragCursors.HandCursor;
				case CursorType.Rotate:
					return DragCursors.RotateCursor;
				case CursorType.RotateDrag:
					return DragCursors.RotateDragCursor;
				case CursorType.SizeAll:
					return Cursors.SizeAll;
				case CursorType.SizeNESW:
					return Cursors.SizeNESW;
				case CursorType.SizeNS:
					return Cursors.SizeNS;
				case CursorType.SizeNWSE:
					return Cursors.SizeNWSE;
				case CursorType.SizeWE:
					return Cursors.SizeWE;
				case CursorType.ZoomIn:
					return DragCursors.ZoomInCursor;
				case CursorType.ZoomLimit:
					return DragCursors.ZoomLimitCursor;
				case CursorType.ZoomOut:
					return DragCursors.ZoomOutCursor;
				case CursorType.None:
				default:
					return Cursor.Current;
			}
		}
		public GestureAllowArgs[] CheckAllowGestures(Point point) {
			IAnnotationDragPoint annotationDragPoint = AnnotationDragPoint;
			if (annotationDragPoint != null) {
				Annotation annotation = annotationDragPoint.Annotation;
				List<GestureAllowArgs> gestures = new List<GestureAllowArgs>();
				if (annotation.RuntimeMoving)
					gestures.Add(GestureAllowArgs.Pan);
				if (annotation.RuntimeRotation)
					gestures.Add(GestureAllowArgs.Rotate);
				if (annotation.RuntimeResizing)
					gestures.Add(GestureAllowArgs.Zoom);
				return gestures.ToArray();
			}
			if (Chart.Diagram != null) {
				Diagram3D diagram3D = Chart.Diagram as Diagram3D;
				if (diagram3D != null) {
					List<GestureAllowArgs> gestures = new List<GestureAllowArgs>();
					if (diagram3D.RuntimeZooming)
						gestures.Add(GestureAllowArgs.Zoom);
					if (diagram3D.RuntimeRotation && diagram3D.RotationOptions.UseTouchDevice && diagram3D.RotationType != RotationType.UseAngles)
						gestures.Add(GestureAllowArgs.Rotate);
					gestures.Add(GestureAllowArgs.Pan);
					return gestures.ToArray();
				}
				XYDiagram2D xyDiagram2D = Chart.Diagram as XYDiagram2D;
				if (xyDiagram2D != null) {
					List<GestureAllowArgs> gestures = new List<GestureAllowArgs>();
					if (Chart.CanZoomIn || Chart.CanZoomOut)
						gestures.Add(GestureAllowArgs.Zoom);
					if (xyDiagram2D.IsScrollingEnabled)
						gestures.Add(GestureAllowArgs.Pan);
					return gestures.ToArray();
				}
			}
			return GestureAllowArgs.None;
		}
		protected override void ShowHint(string hint, Point point) {
			ToolTipController.DefaultController.ShowHint(hint, point);
		}
		protected override void HideHint() {
			ToolTipController.DefaultController.HideHint();
		}
	}
}
