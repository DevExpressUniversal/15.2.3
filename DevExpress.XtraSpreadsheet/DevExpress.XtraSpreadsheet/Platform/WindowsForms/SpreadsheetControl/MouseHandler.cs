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
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Office.Layout;
using DevExpress.XtraSpreadsheet.Model;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
using DevExpress.Office.Utils;
using System.Drawing.Imaging;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Office;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region WinFormsSpreadsheetMouseHandlerStrategyFactory
	public class WinFormsSpreadsheetMouseHandlerStrategyFactory : SpreadsheetMouseHandlerStrategyFactory {
		public override SpreadsheetMouseHandlerStrategy CreateMouseHandlerStrategy(SpreadsheetMouseHandler mouseHandler) {
			return new WinFormsSpreadsheetMouseHandlerStrategy(mouseHandler);
		}
		public override DragFloatingObjectManuallyMouseHandlerStateStrategy CreateDragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state) {
			return new WinFormsDragFloatingObjectManuallyMouseHandlerStateStrategy(state);
		}
		public override DragRangeManuallyMouseHandlerStateStrategy CreateDragRangeManuallyMouseHandlerStateStrategy(DragRangeManuallyMouseHandlerStateBase state) {
			return new WinFormsDragRangeManuallyMouseHandlerStateStrategy(state);
		}
		public override SpreadsheetRectangularObjectResizeMouseHandlerStateStrategy CreateSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy(SpreadsheetRectangularObjectResizeMouseHandlerState state) {
			return new WinFormsSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy(state);
		}
		public override ResizeRowMouseHandlerStateStrategy CreateResizeRowMouseHandlerStateStrategy(ContinueResizeRowsMouseHandlerState state) {
			return new WinFormsResizeRowMouseHandlerStateStrategy(state);
		}
		public override ResizeColumnMouseHandlerStateStrategy CreateResizeColumnMouseHandlerStateStrategy(ContinueResizeColumnsMouseHandlerState state) {
			return new WinFormsResizeColumnMouseHandlerStateStrategy(state);
		}
		public override CommentMouseHandlerStateStrategy CreateCommentMouseHandlerStateStrategy(CommentMouseHandlerStateBase state) {
			return new WinFormsCommentMouseHandlerStateStrategy(state);
		}
	}
	#endregion
	#region WinFormsSpreadsheetMouseHandlerStrategy
	public class WinFormsSpreadsheetMouseHandlerStrategy : SpreadsheetMouseHandlerStrategy {
		public WinFormsSpreadsheetMouseHandlerStrategy(SpreadsheetMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public SpreadsheetControl WinControl { get { return (SpreadsheetControl)Control; } }
		protected override void StartOfficeScroller(Point clientPoint) {
			if (!Control.InnerControl.DocumentModel.BehaviorOptions.OfficeScrollingAllowed)
				return;
			SpreadsheetControl control = (SpreadsheetControl)Control;
			Point screenPoint = control.PointToScreen(clientPoint);
			OfficeScroller.Start(control, screenPoint);
		}
		protected override IOfficeScroller CreateOfficeScroller() {
			return new SpreadsheetOfficeScroller(Control);
		}
		protected override PlatformIndependentMouseEventArgs CreateFakeMouseMoveEventArgs() {
			Point screenMousePos = Cursor.Position;
			Point clientMousePoint = WinControl.PointToClient(screenMousePos);
			PlatformIndependentMouseEventArgs args = new PlatformIndependentMouseEventArgs(MouseButtons.None, 0, clientMousePoint.X, clientMousePoint.Y, 0);
			return ConvertMouseEventArgs(args);
		}
		protected override PlatformIndependentMouseEventArgs ConvertMouseEventArgs(PlatformIndependentMouseEventArgs screenMouseEventArgs) {
			Point location = WinControl.GetPhysicalPoint(screenMouseEventArgs.Location);
			PlatformIndependentMouseEventArgs result = ConvertMouseWheelEventArgs(screenMouseEventArgs, location);
			if (result != null)
				return result;
			return new PlatformIndependentMouseEventArgs(screenMouseEventArgs.Button, screenMouseEventArgs.Clicks, location.X, location.Y, screenMouseEventArgs.Delta);
		}
		OfficeMouseWheelEventArgs ConvertMouseWheelEventArgs(PlatformIndependentMouseEventArgs screenMouseEventArgs, Point recalculatedLocation) {
			OfficeMouseWheelEventArgs ea = screenMouseEventArgs as OfficeMouseWheelEventArgs;
			if (ea == null)
				return null;
			int delta = CalculateDelta(screenMouseEventArgs);
			OfficeMouseWheelEventArgs result = new OfficeMouseWheelEventArgs(screenMouseEventArgs.Button, screenMouseEventArgs.Clicks, recalculatedLocation.X, recalculatedLocation.Y, delta);
			result.IsHorizontal = ea.IsHorizontal;
			return result;
		}
		int CalculateDelta(PlatformIndependentMouseEventArgs screenMouseEventArgs) {
			MouseWheelScrollClientArgs ea = screenMouseEventArgs as MouseWheelScrollClientArgs;
			if (ea == null)
				return screenMouseEventArgs.Delta;
			return -ea.Distance * SystemInformation.MouseWheelScrollDelta;
		}
		protected override PlatformIndependentDragEventArgs ConvertDragEventArgs(PlatformIndependentDragEventArgs screenDragEventArgs) {
			Point screenMousePos = new Point(screenDragEventArgs.X, screenDragEventArgs.Y);
			Point clientMousePoint = WinControl.PointToClient(screenMousePos);
			DocumentLayoutUnitConverter unitConverter = Control.InnerControl.DocumentModel.LayoutUnitConverter;
			int x = unitConverter.PixelsToLayoutUnits(clientMousePoint.X, DocumentModel.DpiX) - Control.ViewBounds.Left;
			int y = unitConverter.PixelsToLayoutUnits(clientMousePoint.Y, DocumentModel.DpiY) - Control.ViewBounds.Top;
			return new DragEventArgs(screenDragEventArgs.Data, screenDragEventArgs.KeyState, x, y, screenDragEventArgs.AllowedEffect, screenDragEventArgs.Effect);
		}
		protected override void AutoScrollerOnDragOver(Point pt) {
			MouseHandler.AutoScroller.OnMouseMove(WinControl.PointToClient(pt));
		}
		protected override void OnMouseUp(PlatformIndependentMouseEventArgs e) {
		}
	}
	#endregion
	#region WinFormsDragFloatingObjectManuallyMouseHandlerStateStrategy
	public class WinFormsDragFloatingObjectManuallyMouseHandlerStateStrategy : DragFloatingObjectManuallyMouseHandlerStateStrategy {
		#region Fields
		WinFormsMouseHandlerStateStrategyHelper strategyHelper;
		#endregion
		public WinFormsDragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state)
			: base(state) {
			this.strategyHelper = new WinFormsMouseHandlerStateStrategyHelper(DocumentModel);
		}
		#region Properties
		public SpreadsheetControl WinControl { get { return (SpreadsheetControl)Control; } }
		#endregion
		protected override void BeginVisualFeedback() {
			Color outlineColor = DXColor.Gray;
			Picture picture = DocumentModel.ActiveSheet.DrawingObjects[Box.DrawingIndex] as Picture;
			if (picture != null)
				outlineColor = picture.ShapeProperties.OutlineColor.FinalColor;
			this.strategyHelper.InitializePainter(InitialShapeBounds, InitialContentBounds, outlineColor);
		}
		protected override void EndVisualFeedback() {
			this.strategyHelper.InvalidatePainter();
		}
		protected override void ShowVisualFeedback(Rectangle bounds, Page page, OfficeImage image) {
			if (image == null)
				return;
			ShadowedFloatingObjectPainter painter = this.strategyHelper.Painter;
			painter.Image = image;
			painter.Bounds = bounds;
			painter.Transform = State.CreateVisualFeedbackTransform();
			WinControl.Painter.DeferredDraw(page, painter.Paint);
			WinControl.Refresh();
		}
		protected override void HideVisualFeedback(Rectangle bounds, Page page) {
			WinControl.Invalidate();
		}
	}
	#endregion
	#region WinFormsSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy
	public class WinFormsSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy : SpreadsheetRectangularObjectResizeMouseHandlerStateStrategy {
		#region Fields
		WinFormsMouseHandlerStateStrategyHelper strategyHelper;
		#endregion
		public WinFormsSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy(SpreadsheetRectangularObjectResizeMouseHandlerState state)
			: base(state) {
			this.strategyHelper = new WinFormsMouseHandlerStateStrategyHelper(DocumentModel);
		}
		#region Properties
		public SpreadsheetControl WinControl { get { return (SpreadsheetControl)Control; } }
		#endregion
		protected override void BeginVisualFeedback() {
			Color outlineColor = DXColor.Gray;
			Picture picture = DocumentModel.ActiveSheet.DrawingObjects[Box.DrawingIndex] as Picture;
			if (picture != null)
				outlineColor = picture.ShapeProperties.OutlineColor.FinalColor;
			this.strategyHelper.InitializePainter(InitialShapeBounds, InitialContentBounds, outlineColor);
		}
		protected override void EndVisualFeedback() {
			this.strategyHelper.InvalidatePainter();
		}
		protected override void ShowVisualFeedback(Rectangle bounds, Page page, OfficeImage image) {
			if (image == null)
				return;
			ShadowedFloatingObjectPainter painter = this.strategyHelper.Painter;
			painter.Image = image;
			painter.Bounds = bounds;
			painter.Transform = State.CreateVisualFeedbackTransform();
			WinControl.Painter.DeferredDraw(page, painter.Paint);
			WinControl.Refresh();
		}
		protected override void HideVisualFeedback() {
			WinControl.Invalidate();
		}
	}
	#endregion
	#region WinFormsResizeRowMouseHandlerStateStrategy
	public class WinFormsResizeRowMouseHandlerStateStrategy : ResizeRowMouseHandlerStateStrategy {
		public WinFormsResizeRowMouseHandlerStateStrategy(ContinueResizeRowsMouseHandlerState state)
			: base(state) {
		}
		#region Properties
		public SpreadsheetControl WinControl { get { return (SpreadsheetControl)Control; } }
		#endregion
		protected override void DrawReversibleLine(int y) {
			WinControl.Painter.DrawReversibleHorizontalLine(y);
		}
		protected override void BeginVisualFeedback() {
		}
		protected override void EndVisualFeedback() {
		}
		protected override void ShowVisualFeedback() {
		}
		protected override void HideVisualFeedback() {
		}
	}
	#endregion
	#region WinFormsResizeColumnMouseHandlerStateStrategy
	public class WinFormsResizeColumnMouseHandlerStateStrategy : ResizeColumnMouseHandlerStateStrategy {
		public WinFormsResizeColumnMouseHandlerStateStrategy(ContinueResizeColumnsMouseHandlerState state)
			: base(state) {
		}
		#region Properties
		public SpreadsheetControl WinControl { get { return (SpreadsheetControl)Control; } }
		#endregion
		protected override void DrawReversibleLine(int y) {
			WinControl.Painter.DrawReversibleVerticalLine(y);
		}
		protected override void BeginVisualFeedback() {
		}
		protected override void EndVisualFeedback() {
		}
		protected override void ShowVisualFeedback() {
		}
		protected override void HideVisualFeedback() {
		}
	}
	#endregion
	#region WinFormsDragRangeManuallyMouseHandlerStateStrategy
	public class WinFormsDragRangeManuallyMouseHandlerStateStrategy : DragRangeManuallyMouseHandlerStateStrategy {
		#region Fields
		const int rangeBorderWidthInPixels = 3;
		int rangeBorderWidthInModelUnits;
		#endregion
		public WinFormsDragRangeManuallyMouseHandlerStateStrategy(DragRangeManuallyMouseHandlerStateBase state)
			: base(state) {
			rangeBorderWidthInModelUnits = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(rangeBorderWidthInPixels, DocumentModel.Dpi);
		}
		#region Properties
		public SpreadsheetControl WinControl { get { return (SpreadsheetControl)Control; } }
		#endregion
		protected override void HideVisualFeedback() {
			WinControl.Refresh();
		}
		protected override void ShowVisualFeedback(Rectangle bounds, Page page) {
			DrawAtPageDelegate draw = delegate(GraphicsCache cache) {
				PageSpreadsheetSelectionPainter selectionPainter = WinControl.ViewPainter.CreatePageSelectionPainter(cache);
				selectionPainter.DrawSelectionBorder(bounds, rangeBorderWidthInModelUnits);
			};
			WinControl.Painter.DeferredDraw(page, draw);
			WinControl.Refresh();
		}
	}
	#endregion
	#region WinFormsCommentMouseHandlerStateStrategy
	public class WinFormsCommentMouseHandlerStateStrategy : CommentMouseHandlerStateStrategy {
		static Color boundsColor = Color.FromArgb(100, 100, 100);
		public WinFormsCommentMouseHandlerStateStrategy(CommentMouseHandlerStateBase state)
			: base(state) {
		}
		#region Properties
		public SpreadsheetControl WinControl { get { return (SpreadsheetControl)Control; } }
		#endregion
		protected override void ShowVisualFeedback(Rectangle bounds, Page page) {
			DrawAtPageDelegate draw = delegate(GraphicsCache cache) {
				cache.DrawRectangle(cache.GetPen(boundsColor, 1), bounds);
			};
			WinControl.Painter.DeferredDraw(page, draw);
			WinControl.Refresh();
		}
		protected override void HideVisualFeedback() {
			WinControl.Invalidate();
		}
	}
	#endregion
	#region SpreadsheetOfficeScroller
	public class SpreadsheetOfficeScroller : OfficeScroller {
		readonly ISpreadsheetControl control;
		public SpreadsheetOfficeScroller(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public ISpreadsheetControl Control { get { return control; } }
		protected override bool AllowHScroll { get { return true; } }
		protected override void OnVScroll(int delta) {
			base.OnVScroll(delta);
			Control.InnerControl.ActiveView.ScrollLineUpDown(delta);
		}
		protected override void OnHScroll(int delta) {
			base.OnHScroll(delta);
			Control.InnerControl.ActiveView.ScrollLineLeftRight(delta);
		}
	}
	#endregion
	#region WinFormsMouseHandlerStateStrategyHelper
	public class WinFormsMouseHandlerStateStrategyHelper {
		#region Fields
		const float alpha = 0.6f;
		readonly DocumentModel documentModel;
		ShadowedFloatingObjectPainter painter;
		#endregion
		public WinFormsMouseHandlerStateStrategyHelper(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public ShadowedFloatingObjectPainter Painter { get { return painter; } }
		#endregion
		protected internal void InitializePainter(Rectangle initialShapeBounds, Rectangle initialContentBounds, Color outlineColor) {
			this.painter = new ShadowedFloatingObjectPainter(DocumentModel.LayoutUnitConverter);
			this.painter.Alpha = alpha;
			this.painter.InitialShapeBounds = initialShapeBounds;
			this.painter.InitialContentBounds = initialContentBounds;
			this.painter.OutlineColor = Color.FromArgb((int)(alpha * outlineColor.A), outlineColor);
		}
		protected internal void InvalidatePainter() {
			this.painter = null;
		}
	}
	#endregion
}
