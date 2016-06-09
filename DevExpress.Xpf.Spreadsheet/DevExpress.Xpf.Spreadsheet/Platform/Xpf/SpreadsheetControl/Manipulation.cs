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
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Office.Internal;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using System.Linq;
using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
namespace DevExpress.Xpf.Spreadsheet {
	public partial class SpreadsheetControl : IGestureClient {
		#region IGestureClient Members
		void IGestureClient.OnManipulationStarting(ManipulationStartingEventArgs e) {
			e.Mode = ManipulationModes.All;
		}
		void OnVerticalPan(ManipulationDeltaEventArgs e) {
			int deltaY = (int)Math.Round(e.DeltaManipulation.Translation.Y);
			ScrollVerticallyByPhysicalOffsetCommand command = new ScrollVerticallyByPhysicalOffsetCommand(this);
			command.PhysicalOffset = this.ActiveView.DocumentLayout.UnitConverter.PixelsToLayoutUnits(deltaY, this.DpiY);
			command.Execute();
		}
		double panX = 0;
		void OnHorizontalPan(ManipulationDeltaEventArgs e) {
			int deltaX = (int)Math.Round(e.DeltaManipulation.Translation.X);
			panX += deltaX;
			ScrollHorizontallyByPhysicalOffsetCommand command = new ScrollHorizontallyByPhysicalOffsetCommand(this);
			command.PhysicalOffset = this.ActiveView.DocumentLayout.UnitConverter.PixelsToLayoutUnits((int)panX, this.DpiX);
			command.Execute();
			if (command.ExecutedSuccessfully)
				panX = 0;
		}
		void OnZoom(ManipulationDeltaEventArgs e) {
			ActiveView.ZoomFactor *= (float)e.DeltaManipulation.Scale.X;
		}
		bool IGestureClient.ShouldRotate(ManipulationDeltaEventArgs e) {
			Point p = e.ManipulationOrigin;
			var result = ActiveView.CalculatePageHitTest(p.ToDrawingPoint());
			return result.PictureBox != null;
		}
		bool IGestureClient.ShouldProcessGesture(TouchEventArgs e) {
			var worksheetPoint = e.GetTouchPoint(ViewControl.WorksheetControl).Position;
			var result = ActiveView.CalculatePageHitTest(worksheetPoint.ToDrawingPoint());
			var hotZone = ActiveView.SelectionLayout.CalculateHotZone(worksheetPoint.ToDrawingPoint());
			if (GetHitTest(e.GetTouchPoint(this).Position) != SpreadsheetHitTestType.Worksheet || IsHeader(result) || hotZone != null) return false;
			return true;
		}
		void IGestureClient.OnTap(TouchEventArgs e) {
			var point = GetPointWithoutScale(e.GetTouchPoint(ViewControl.WorksheetControl).Position);
			PlatformIndependentMouseEventArgs args = new PlatformIndependentMouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, (int)point.X, (int)point.Y, 0);
			if (InnerControl != null) {
				InnerControl.OnMouseDown(args);
				InnerControl.OnMouseUp(args);
			}
		}
		private bool IsHeader(XtraSpreadsheet.Layout.SpreadsheetHitTestResult result) {
			return result != null && result.DetailsLevel == XtraSpreadsheet.Layout.DocumentLayoutDetailsLevel.Header;
		}
		Point manipulationStartedPoint;
		void IGestureClient.OnManipulationStarted(ManipulationStartedEventArgs e) {
			manipulationStartedPoint = e.ManipulationOrigin;
		}
		void IGestureClient.OnManipulationDelta(ManipulationDeltaEventArgs e, ManipulationTypes type) {
			if (ActiveView.CalculatePageHitTest(manipulationStartedPoint.ToDrawingPoint()).PictureBox != null)
				ProcessManipulationOfFloatingObject(e, type);
			else
				ProcessManipulationCommon(e, type);
		}
		private void ProcessManipulationOfFloatingObject(ManipulationDeltaEventArgs e, ManipulationTypes type) {
			switch (type) {
				case ManipulationTypes.VerticalPan:
					OnFloatingObjectPan(e);
					break;
				case ManipulationTypes.HorizontalPan:
					OnFloatingObjectPan(e);
					break;
				case ManipulationTypes.Zoom:
					OnFloatingObjectPanZoomOrRotate(e);
					break;
				case ManipulationTypes.Rotate:
					OnFloatingObjectPanZoomOrRotate(e);
					break;
			}
		}
		private void OnFloatingObjectPanZoomOrRotate(ManipulationDeltaEventArgs e) {
			ShapeRotateCommand command = new ShapeRotateCommand(this);
			command.RotationAngle = (float)e.DeltaManipulation.Rotation;
			command.Execute();
		}
		private void OnFloatingObjectPan(ManipulationDeltaEventArgs e) {
		}
		private void ProcessManipulationCommon(ManipulationDeltaEventArgs e, ManipulationTypes type) {
			switch (type) {
				case ManipulationTypes.VerticalPan:
					OnVerticalPan(e);
					break;
				case ManipulationTypes.HorizontalPan:
					OnHorizontalPan(e);
					break;
				case ManipulationTypes.Zoom:
					OnZoom(e);
					break;
			}
		}
		#endregion
	}
}
