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
using DevExpress.Utils;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Services;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using LayoutDrawingBox = DevExpress.XtraSpreadsheet.Layout.DrawingBox;
using LayoutPictureBox = DevExpress.XtraSpreadsheet.Layout.PictureBox;
using DevExpress.Compatibility.System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region SpreadsheetRectangularObjectResizeMouseHandlerState
	public class SpreadsheetRectangularObjectResizeMouseHandlerState : CancellableMouseHandlerStateBase {
		#region Fields
		readonly DrawingObjectHotZone hotZone;
		readonly SpreadsheetRectangularObjectResizeMouseHandlerStateStrategy platformStrategy;
		float rotationAngle;
		Rectangle initialShapeBounds;
		Rectangle initialContentBounds;
		LayoutDrawingBox box;
		OfficeImage feedbackImage;
		#endregion
		public SpreadsheetRectangularObjectResizeMouseHandlerState(SpreadsheetMouseHandler mouseHandler, DrawingObjectHotZone hotZone, SpreadsheetHitTestResult initialHitTestResult)
			: base(mouseHandler, initialHitTestResult) {
			Guard.ArgumentNotNull(hotZone, "hotZone");
			Guard.ArgumentNotNull(initialHitTestResult, "initialHitTestResult");
			this.hotZone = hotZone;
			this.platformStrategy = CreatePlatformStrategy();
			this.box = hotZone.Box;
			Rectangle boxBounds = box.Bounds;
			this.initialShapeBounds = boxBounds;
			this.initialContentBounds = boxBounds;
			this.rotationAngle = DocumentModel.GetBoxRotationAngleInDegrees(box);
			this.feedbackImage = null;
		}
		#region Properties
		public DrawingObjectHotZone HotZone { get { return hotZone; } }
		public override bool SuppressDefaultMouseWheelProcessing { get { return true; } }
		public Rectangle InitialShapeBounds { get { return initialShapeBounds; } }
		public Rectangle InitialContentBounds { get { return initialContentBounds; } }
		public LayoutDrawingBox Box { get { return box; } }
		protected internal override DocumentLayoutDetailsLevel HitTestDetailsLevel { get { return DocumentLayoutDetailsLevel.Cell; } }
		public float RotationAngle { get { return rotationAngle; } set { rotationAngle = value; } }
		#endregion
		protected virtual SpreadsheetRectangularObjectResizeMouseHandlerStateStrategy CreatePlatformStrategy() {
			return MouseHandler.GetPlatformStrategyFactory().CreateSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy(this);
		}
		protected internal override SpreadsheetCursor CalculateMouseCursor() {
			return SpreadsheetCursors.Cross;
		}
		public override void OnMouseWheel(MouseEventArgs e) {
		}
		protected internal override void CommitChanges(Point point) {
			if (CurrentHitTestResult == null)
				return;
			DocumentModel.BeginUpdate();
			try {
				Rectangle bounds = HotZone.CreateValidBoxBounds(CurrentHitTestResult.LogicalPoint);
				bounds = ForceKeepOriginalAspectRatio(bounds);
				int drawingIndex = Box.DrawingIndex;
				Worksheet activeSheet = DocumentModel.ActiveSheet;
				activeSheet.SetDrawingSizeIndependent(drawingIndex, bounds.Size);
				Point offset = CalculateOffset(bounds);
				activeSheet.MoveDrawingInLayoutUnits(drawingIndex, offset.X, offset.Y);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal Point CalculateOffset(Rectangle bounds) {
			Matrix transform = new Matrix();
			transform.RotateAt(RotationAngle, RectangleUtils.CenterPoint(InitialShapeBounds));
			Point topLeft = transform.TransformPoint(bounds.Location);
			Point center = transform.TransformPoint(RectangleUtils.CenterPoint(bounds));
			transform = new Matrix();
			transform.RotateAt(-RotationAngle, center);
			Point newTopLeft = transform.TransformPoint(topLeft);
			return new Point(newTopLeft.X - InitialShapeBounds.X, newTopLeft.Y - InitialShapeBounds.Y);
		}
		public virtual Matrix CreateVisualFeedbackTransform() {
			if (CurrentHitTestResult == null)
				return null;
			return TransformMatrixExtensions.CreateTransformUnsafe(rotationAngle, InitialShapeBounds);
		}
		protected internal override void BeginVisualFeedback() {
			FeedbackImageFactory factory = new FeedbackImageFactory();
			this.feedbackImage = factory.CreateImage(Box);
			platformStrategy.BeginVisualFeedback();
		}
		protected internal override void EndVisualFeedback() {
			platformStrategy.EndVisualFeedback();
			if (this.feedbackImage != null) {
				this.feedbackImage.Dispose();
				this.feedbackImage = null;
			}
		}
		protected internal override void ShowVisualFeedback() {
			Rectangle bounds = HotZone.CreateValidBoxBounds(CurrentHitTestResult.LogicalPoint);
			bounds = ForceKeepOriginalAspectRatio(bounds);
			platformStrategy.ShowVisualFeedback(bounds, CurrentHitTestResult.Page, feedbackImage);
		}
		protected internal Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds) {
			Size actualSize = InitialShapeBounds.Size;
			if (actualSize == bounds.Size)
				return bounds;
			return HotZone.ForceKeepOriginalAspectRatio(bounds, actualSize);
		}
		protected internal override void HideVisualFeedback() {
			platformStrategy.HideVisualFeedback();
		}
	}
	#endregion
}
