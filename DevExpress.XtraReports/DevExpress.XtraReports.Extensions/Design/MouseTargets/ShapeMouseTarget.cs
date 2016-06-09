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
using System.Text;
using DevExpress.XtraReports.UI;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraReports.Localization;
using System.ComponentModel.Design;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design.MouseTargets {
	class ShapeMouseTarget : ControlMouseTarget {
		#region inner classes
		class RotationState {
			XRShapeDesigner designer;
			IBandViewInfoService bandViewSvc;
			int startRotationAngle;
			Point startRotationPoint;
			XRShape XRShape {
				get { return (XRShape)designer.Component; }
			}
			public RotationState(XRShapeDesigner designer, IBandViewInfoService bandViewSvc) {
				this.designer = designer;
				this.bandViewSvc = bandViewSvc;
				startRotationAngle = XRShape.Angle;
				startRotationPoint = CalcCurrentRotationPoint();
			}
			public void EndRotation() {
				designer.ApplyAngle(startRotationAngle);
			}
			public void PerformRotation() {
				XRShape.Angle = startRotationAngle + XRShapeDesigner.AngleBetweenPointsInDeg(startRotationPoint, CalcCurrentRotationPoint());
				bandViewSvc.Invalidate();
			}
			Point CalcCurrentRotationPoint() {
				Point offsetFromShapeCenter = System.Windows.Forms.Control.MousePosition;
				Point shapeScreenCenter = Point.Round(RectHelper.CenterOf(bandViewSvc.GetControlScreenBounds(XRShape)));
				offsetFromShapeCenter.Offset(-shapeScreenCenter.X, -shapeScreenCenter.Y);
				return offsetFromShapeCenter;
			}
		}
		#endregion
		const Keys RotationModifierKey = Keys.Control;
		RotationState rotationState;
		Point lastMousePosition;
		Timer updateCursorTimer;
		new XRShapeDesigner Designer {
			get { return (XRShapeDesigner)base.Designer; }
		}
		public ShapeMouseTarget(XRControl xrControl, IServiceProvider servProvider)
			: base(xrControl, servProvider) {
			updateCursorTimer = new Timer();
			updateCursorTimer.Tick += new EventHandler(updateCursorTimer_Tick);
		}
		public override void Dispose() {
			if(updateCursorTimer != null) {
				updateCursorTimer.Tick -= new EventHandler(updateCursorTimer_Tick);
				updateCursorTimer.Dispose();
				updateCursorTimer = null;
			}
		}
		void updateCursorTimer_Tick(object sender, EventArgs e) {
			((Control)BandViewSvc).Cursor = GetCursor(lastMousePosition);
		}
		public override void HandleMouseMove(object sender, BandMouseEventArgs e) {
			SetToolTip(e.Button);
			if(CanPerformRotation(e.Button) && rotationState != null)
				rotationState.PerformRotation();
			else
				EndRotation();
			if(!updateCursorTimer.Enabled) {
				updateCursorTimer.Enabled = true;
				lastMousePosition = e.Location;
			}
		}
		void SetToolTip(MouseButtons buttons) {
			string hint = String.Empty;
			if(buttons.IsNone() && ReportDesigner.ShowDesignerHints)
				hint = ReportLocalizer.GetString(ReportStringId.Msg_ShapeRotationToolTip);
			ToolTipService.GetInstance(servProvider).ShowHint(hint);
		}
		static bool CanPerformRotation(MouseButtons buttons) {
			return Control.ModifierKeys == RotationModifierKey && buttons.IsLeft();
		}
		public override void HandleMouseDown(object sender, BandMouseEventArgs e) {
			base.HandleMouseDown(sender, e);
			SetToolTip(e.Button);
			if(CanPerformRotation(e.Button))
				SetInRotationState();
			else
				EndRotation();
		}
		public override void HandleMouseUp(object sender, BandMouseEventArgs e) {
			base.HandleMouseUp(sender, e);
			SetToolTip(e.Button);
			if(e.Button.IsLeft())
				EndRotation();
		}
		public override void HandleMouseLeave(object sender, EventArgs e) {
			base.HandleMouseLeave(sender, e);
			ToolTipService.GetInstance(servProvider).HideHint();
			EndRotation();
			updateCursorTimer.Enabled = false;
		}
		protected override Cursor GetCursor(Point pt) {
			return Designer.InRotation ? DragCursors.HandDragCursor :
				Control.ModifierKeys == RotationModifierKey ? DragCursors.HandCursor :
				Cursors.SizeAll;
		}
		void EndRotation() {
			rotationState = null;
			Designer.InRotation = false;
		}
		void SetInRotationState() {
			if(!Designer.Locked) {
				rotationState = new RotationState(Designer, BandViewSvc);
				Designer.InRotation = true;
			}
		}
	}
}
