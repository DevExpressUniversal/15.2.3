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
using System.Diagnostics;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.Office.Layout;
using System.Collections.Generic;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Compatibility.System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region IHotZoneVisitor
	public interface IHotZoneVisitor {
		void Visit(IResizeHotZone hotZone);
		void Visit(DrawingObjectRotationHotZone hotZone);
		void Visit(RangeDragHotZone hotZone);
		void Visit(RangeResizeHotZone hotZone);
		void Visit(FormulaRangeDragHotZone hotZone);
		void Visit(FormulaRangeResizeHotZone hotZone);
		void Visit(IFilterHotZone hotZone);
		void Visit(CommentDragHotZone hotZone);
		void Visit(DataValidationHotZone hotZone);
		void Visit(PivotTableExpandCollapseHotZone hotZone);
	}
	#endregion
	#region ResizeHotZoneType
	public enum ResizeHotZoneType {
		Rectangle,
		Ellipse
	}
	#endregion
	public interface IResizeHotZone {
		Rectangle Bounds { get; }
		ResizeHotZoneType Type { get; }
	}
	#region HotZone (abstract class)
	public abstract class HotZone {
		#region Fields
		const int minHotZoneSizeInPixels = 4;
		const float extendedSizeCoeff = 13 / 5.0F; 
		Rectangle bounds;
		Rectangle extendedBounds;
		Matrix hitTestTransform;
		IGestureStateIndicator gestureStateIndicator;
		#endregion
		protected HotZone(IGestureStateIndicator gestureStateIndicator) {
			Guard.ArgumentNotNull(gestureStateIndicator, "gestureStateIndicator");
			this.gestureStateIndicator = gestureStateIndicator;
		}
		#region Properties
		public Rectangle Bounds {
			get { return bounds; }
			set {
				bounds = value;
				extendedBounds = CalculateExtendedBounds(value);
			}
		}
		public Rectangle ExtendedBounds { get { return extendedBounds; } }
		public IGestureStateIndicator GestureStateIndicator { get { return gestureStateIndicator; } }
		bool UseExtendedBounds { get { return GestureStateIndicator.GestureActivated; } }
		public abstract SpreadsheetCursor Cursor { get; }
		public Matrix HitTestTransform { get { return hitTestTransform; } set { hitTestTransform = value; } }
		#endregion
		public virtual bool HitTest(Point point, DocumentLayoutUnitConverter unitConverter, float dpi, float zoomFactor) {
			Rectangle hotZoneBounds = CalculateActualBounds(unitConverter, dpi, zoomFactor);
			if (hitTestTransform != null)
				point = hitTestTransform.TransformPoint(point);
			return hotZoneBounds.Contains(point);
		}
		public Rectangle CalculateActualBounds(DocumentLayoutUnitConverter unitConverter, float dpiX, float zoomFactor) {
			int minHotZoneSize = unitConverter.PixelsToLayoutUnits(minHotZoneSizeInPixels, dpiX);
			minHotZoneSize = (int)Math.Round(minHotZoneSize / zoomFactor);
			Rectangle result = (UseExtendedBounds) ? ExtendedBounds : Bounds;
			if (result.Width < minHotZoneSize) {
				result.X += (result.Width - minHotZoneSize) / 2;
				result.Width = minHotZoneSize;
			}
			if (result.Height < minHotZoneSize) {
				result.Y += (result.Height - minHotZoneSize) / 2;
				result.Height = minHotZoneSize;
			}
			return result;
		}
		Rectangle CalculateExtendedBounds(Rectangle bounds) {
			int width = bounds.Width;
			int extendedWidth = (int)(width * extendedSizeCoeff);
			int height = bounds.Height;
			int extendedHeight = (int)(height * extendedSizeCoeff);
			int deltaX = (extendedWidth - width) / 2;
			int deltaY = (extendedHeight - height) / 2;
			return Rectangle.Inflate(bounds, deltaX, deltaY);
		}
		public abstract void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result);
		public abstract void Visit(IHotZoneVisitor visitor);
	}
	#endregion
	#region DrawingObjectHotZone (abstract class)
	public abstract class DrawingObjectHotZone : HotZone {
		#region Fields
		readonly DrawingBox box;
		readonly DocumentModel documentModel;
		#endregion
		protected DrawingObjectHotZone(DocumentModel documentModel, DrawingBox box, IGestureStateIndicator gestureStateIndicator)
			: base(gestureStateIndicator) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(box, "box");
			this.documentModel = documentModel;
			this.box = box;
		}
		#region Properties
		public DrawingBox Box { get { return box; } }
		public DocumentModel DocumentModel { get { return documentModel; } }
		public virtual bool CanKeepAspectRatio {
			get {
				return Box.NoChangeAspect;
			}
		}
		#endregion
		protected internal virtual Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, Size originalSize) {
			if (!CanKeepAspectRatio)
				return bounds;
			float aspectX = Math.Max(1, 100 * bounds.Width / (float)originalSize.Width);
			float aspectY = Math.Max(1, 100 * bounds.Height / (float)originalSize.Height);
			if (aspectX > aspectY)
				return ForceKeepOriginalAspectRatio(bounds, bounds.Width, (int)Math.Round(originalSize.Height * aspectX / 100.0f));
			else
				return ForceKeepOriginalAspectRatio(bounds, (int)Math.Round(originalSize.Width * aspectY / 100.0f), bounds.Height);
		}
		protected internal virtual Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			return bounds;
		}
		public Rectangle CreateValidBoxBounds(Point point) {
			if (HitTestTransform != null)
				point = HitTestTransform.TransformPoint(point);
			return CreateValidBoxBoundsCore(point);
		}
		public Point CalculateOffset(Point point) {
			if (HitTestTransform != null)
				point = HitTestTransform.TransformPoint(point);
			return CalculateOffsetCore(point);
		}
		protected SpreadsheetCursor CalculateCursor(SpreadsheetCursor defaultCursor) {
			if (HitTestTransform == null)
				return defaultCursor;
			float cursorAngle = CalculateCursorAngle(defaultCursor);
			if (cursorAngle < 0)
				return defaultCursor;
			float rotationAngle = -DocumentModel.GetBoxRotationAngleInDegrees(box);
			return CalculateCursorByAngle(rotationAngle + cursorAngle);
		}
		float CalculateCursorAngle(SpreadsheetCursor cursor) {
			if (cursor == SpreadsheetCursors.SizeWE)
				return 0;
			if (cursor == SpreadsheetCursors.SizeNS)
				return 90;
			if (cursor == SpreadsheetCursors.SizeNESW)
				return 45;
			if (cursor == SpreadsheetCursors.SizeNWSE)
				return 135;
			return -1;
		}
		SpreadsheetCursor CalculateCursorByAngle(float angle) {
			angle %= 360f;
			if (angle >= 180)
				angle -= 180;
			if (angle <= -180)
				angle += 180;
			if (angle < 0)
				angle += 180;
			System.Diagnostics.Debug.Assert(angle >= 0 && angle <= 180);
			if (angle < 22.5f || angle >= 157.5f) 
				return SpreadsheetCursors.SizeWE;
			if (angle >= 22.5f && angle <= 67.5f) 
				return SpreadsheetCursors.SizeNESW;
			if (angle > 67.5f && angle < 112.5f) 
				return SpreadsheetCursors.SizeNS;
			return SpreadsheetCursors.SizeNWSE;
		}
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			SpreadsheetRectangularObjectResizeMouseHandlerState state = handler.CreateRectangularObjectResizeState(this, result);
			handler.SwitchStateCore(state, Point.Empty);
		}
		protected internal abstract Rectangle CreateValidBoxBoundsCore(Point point);
		protected internal abstract Point CalculateOffsetCore(Point point);
	}
	#endregion
	#region DrawingObjectResizeHotZoneBase (abstract class)
	public abstract class DrawingObjectResizeHotZoneBase : DrawingObjectHotZone, IResizeHotZone {
		protected DrawingObjectResizeHotZoneBase(DocumentModel documentModel, DrawingBox box, IGestureStateIndicator gestureStateIndicator)
			: base(documentModel, box, gestureStateIndicator) {
		}
		#region Properties
		public abstract ResizeHotZoneType Type { get; }
		#endregion
		public override void Visit(IHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
	#region  DrawingObjectTopLeftHotZone
	public class DrawingObjectTopLeftHotZone : DrawingObjectResizeHotZoneBase {
		public DrawingObjectTopLeftHotZone(DocumentModel documentModel, DrawingBox box, IGestureStateIndicator gestureStateIndicator)
			: base(documentModel, box, gestureStateIndicator) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return CalculateCursor(SpreadsheetCursors.SizeNWSE); } }
		public override ResizeHotZoneType Type { get { return ResizeHotZoneType.Ellipse; } }
		#endregion
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return Rectangle.FromLTRB(
				Math.Min(point.X, boxBounds.Right - 1),
				Math.Min(point.Y, boxBounds.Bottom - 1),
				boxBounds.Right,
				boxBounds.Bottom);
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return new Point(boxBounds.Left - point.X, boxBounds.Top - point.Y);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			bounds.X = bounds.Right - desiredWidth;
			bounds.Y = bounds.Bottom - desiredHeight;
			bounds.Width = desiredWidth;
			bounds.Height = desiredHeight;
			return bounds;
		}
	}
	#endregion
	#region DrawingObjectBottomRightHotZone
	public class DrawingObjectBottomRightHotZone : DrawingObjectResizeHotZoneBase {
		public DrawingObjectBottomRightHotZone(DocumentModel documentModel, DrawingBox box, IGestureStateIndicator gestureStateIndicator)
			: base(documentModel, box, gestureStateIndicator) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return CalculateCursor(SpreadsheetCursors.SizeNWSE); } }
		public override ResizeHotZoneType Type { get { return ResizeHotZoneType.Ellipse; } }
		#endregion
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return Rectangle.FromLTRB(
				boxBounds.Left,
				boxBounds.Top,
				Math.Max(point.X, boxBounds.Left + 1),
				Math.Max(point.Y, boxBounds.Top + 1));
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return new Point(boxBounds.Right - point.X, boxBounds.Bottom - point.Y);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			bounds.Width = desiredWidth;
			bounds.Height = desiredHeight;
			return bounds;
		}
	}
	#endregion
	#region DrawingObjectTopRightHotZone
	public class DrawingObjectTopRightHotZone : DrawingObjectResizeHotZoneBase {
		public DrawingObjectTopRightHotZone(DocumentModel documentModel, DrawingBox box, IGestureStateIndicator gestureStateIndicator)
			: base(documentModel, box, gestureStateIndicator) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return CalculateCursor(SpreadsheetCursors.SizeNESW); } }
		public override ResizeHotZoneType Type { get { return ResizeHotZoneType.Ellipse; } }
		#endregion
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return Rectangle.FromLTRB(
				boxBounds.Left,
				Math.Min(point.Y, boxBounds.Bottom - 1),
				Math.Max(point.X, boxBounds.Left + 1),
				boxBounds.Bottom);
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return new Point(boxBounds.Right - point.X, boxBounds.Top - point.Y);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			bounds.Width = desiredWidth;
			bounds.Y = bounds.Bottom - desiredHeight;
			bounds.Height = desiredHeight;
			return bounds;
		}
	}
	#endregion
	#region DrawingObjectBottomLeftHotZone
	public class DrawingObjectBottomLeftHotZone : DrawingObjectResizeHotZoneBase {
		public DrawingObjectBottomLeftHotZone(DocumentModel documentModel, DrawingBox box, IGestureStateIndicator gestureStateIndicator)
			: base(documentModel, box, gestureStateIndicator) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return CalculateCursor(SpreadsheetCursors.SizeNESW); } }
		public override ResizeHotZoneType Type { get { return ResizeHotZoneType.Ellipse; } }
		#endregion
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return Rectangle.FromLTRB(
				Math.Min(point.X, boxBounds.Right - 1),
				boxBounds.Top,
				boxBounds.Right,
				Math.Max(point.Y, boxBounds.Top + 1));
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return new Point(boxBounds.Left - point.X, boxBounds.Bottom - point.Y);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			bounds.X = bounds.Right - desiredWidth;
			bounds.Width = desiredWidth;
			bounds.Height = desiredHeight;
			return bounds;
		}
	}
	#endregion
	#region DrawingObjectTopMiddleHotZone
	public class DrawingObjectTopMiddleHotZone : DrawingObjectResizeHotZoneBase {
		public DrawingObjectTopMiddleHotZone(DocumentModel documentModel, DrawingBox box, IGestureStateIndicator gestureStateIndicator)
			: base(documentModel, box, gestureStateIndicator) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return CalculateCursor(SpreadsheetCursors.SizeNS); } }
		public override ResizeHotZoneType Type { get { return ResizeHotZoneType.Rectangle; } }
		#endregion
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return Rectangle.FromLTRB(
				boxBounds.Left,
				Math.Min(point.Y, boxBounds.Bottom - 1),
				boxBounds.Right,
				boxBounds.Bottom);
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return new Point(0, boxBounds.Top - point.Y);
		}
	}
	#endregion
	#region DrawingObjectBottomMiddleHotZone
	public class DrawingObjectBottomMiddleHotZone : DrawingObjectResizeHotZoneBase {
		public DrawingObjectBottomMiddleHotZone(DocumentModel documentModel, DrawingBox box, IGestureStateIndicator gestureStateIndicator)
			: base(documentModel, box, gestureStateIndicator) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return CalculateCursor(SpreadsheetCursors.SizeNS); } }
		public override ResizeHotZoneType Type { get { return ResizeHotZoneType.Rectangle; } }
		#endregion
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return Rectangle.FromLTRB(
				boxBounds.Left,
				boxBounds.Top,
				boxBounds.Right,
				Math.Max(point.Y, boxBounds.Top + 1));
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return new Point(0, boxBounds.Bottom - point.Y);
		}
	}
	#endregion
	#region DrawingObjectMiddleLeftHotZone
	public class DrawingObjectMiddleLeftHotZone : DrawingObjectResizeHotZoneBase {
		public DrawingObjectMiddleLeftHotZone(DocumentModel documentModel, DrawingBox box, IGestureStateIndicator gestureStateIndicator)
			: base(documentModel, box, gestureStateIndicator) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return CalculateCursor(SpreadsheetCursors.SizeWE); } }
		public override ResizeHotZoneType Type { get { return ResizeHotZoneType.Rectangle; } }
		#endregion
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return Rectangle.FromLTRB(
				Math.Min(point.X, boxBounds.Right - 1),
				boxBounds.Top,
				boxBounds.Right,
				boxBounds.Bottom);
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return new Point(boxBounds.Left - point.X, 0);
		}
	}
	#endregion
	#region DrawingObjectMiddleRightHotZone
	public class DrawingObjectMiddleRightHotZone : DrawingObjectResizeHotZoneBase {
		public DrawingObjectMiddleRightHotZone(DocumentModel documentModel, DrawingBox box, IGestureStateIndicator gestureStateIndicator)
			: base(documentModel, box, gestureStateIndicator) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return CalculateCursor(SpreadsheetCursors.SizeWE); } }
		public override ResizeHotZoneType Type { get { return ResizeHotZoneType.Rectangle; } }
		#endregion
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return Rectangle.FromLTRB(
				boxBounds.Left,
				boxBounds.Top,
				Math.Max(point.X, boxBounds.Left + 1),
				boxBounds.Bottom);
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.Bounds;
			return new Point(boxBounds.Right - point.X, 0);
		}
	}
	#endregion
	#region DrawingObjectRotationHotZone
	public class DrawingObjectRotationHotZone : DrawingObjectHotZone {
		#region Fields
		Point lineEnd;
		#endregion
		public DrawingObjectRotationHotZone(DocumentModel documentModel, DrawingBox box, IGestureStateIndicator gestureStateIndicator)
			: base(documentModel, box, gestureStateIndicator) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return CalculateCursor(SpreadsheetCursors.BeginRotate); } }
		public Point LineEnd { get { return lineEnd; } set { lineEnd = value; } }
		#endregion
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			return Box.Bounds;
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Point rotationPoint = RectangleUtils.CenterPoint(Bounds);
			return new Point(rotationPoint.X - point.X, rotationPoint.Y - point.Y);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			return bounds;
		}
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			SpreadsheetRectangularObjectRotateMouseHandlerState state = handler.CreateRectangularObjectRotateState(this, result);
			handler.SwitchStateCore(state, Point.Empty);
		}
		public override void Visit(IHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
	#region RangeDragHotZone
	public class RangeDragHotZone : HotZone {
		public RangeDragHotZone(IGestureStateIndicator gestureStateIndicator)
			: base(gestureStateIndicator) {
		}
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.DragRange; } }
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			MouseHandlerState dragState = handler.CreateDragRangeState(result);
			BeginMouseDragHelperState newState = new BeginMouseDragHelperState(handler, dragState, result.PhysicalPoint);
			handler.SwitchStateCore(newState, result.PhysicalPoint);
		}
		public override void Visit(IHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
	#region RangeResizeHotZone
	public class RangeResizeHotZone : HotZone {
		public RangeResizeHotZone(IGestureStateIndicator gestureStateIndicator)
			: base(gestureStateIndicator) {
		}
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.SmallCross; } }
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			MouseHandlerState resizeState = handler.CreateResizeRangeState(result);
			BeginMouseDragHelperState newState = new BeginMouseDragHelperState(handler, resizeState, result.PhysicalPoint);
			handler.SwitchStateCore(newState, result.PhysicalPoint);
		}
		public override void Visit(IHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
	#region FormulaRangeDragHotZone
	public class FormulaRangeDragHotZone : HotZone {
		readonly int pageIndex;
		readonly int referencedRangeIndex;
		public FormulaRangeDragHotZone(IGestureStateIndicator gestureStateIndicator, int pageIndex, int referencedRangeIndex)
			: base(gestureStateIndicator) {
			this.pageIndex = pageIndex;
			this.referencedRangeIndex = referencedRangeIndex;
		}
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.DragRange; } }
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			if (pageIndex != CalculatePageIndex(result))
				return;
			if (!handler.Control.InnerControl.IsInplaceEditorActive)
				return;
			InnerCellInplaceEditor inplaceEditor = handler.Control.InnerControl.InplaceEditor;
			MouseHandlerState dragState = new DragFormulaRangeManuallyMouseHandlerState(inplaceEditor.MouseHandler, result, referencedRangeIndex);
			BeginMouseDragHelperState newState = new BeginMouseDragHelperState(handler, dragState, result.PhysicalPoint);
			handler.SwitchStateCore(newState, result.PhysicalPoint);
		}
		public override void Visit(IHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
		int CalculatePageIndex(SpreadsheetHitTestResult result) {
			DocumentLayout documentLayout = result.DocumentLayout;
			if (documentLayout == null)
				return -1;
			Page page = result.Page;
			if (page == null)
				return -1;
			return documentLayout.Pages.IndexOf(page);
		}
	}
	#endregion
	#region FormulaRangeResizeHotZone
	public class FormulaRangeResizeHotZone : HotZone {
		readonly int pageIndex;
		readonly int referencedRangeIndex;
		readonly FormulaRangeAnchor anchor;
		public FormulaRangeResizeHotZone(IGestureStateIndicator gestureStateIndicator, int pageIndex, int referencedRangeIndex, FormulaRangeAnchor anchor)
			: base(gestureStateIndicator) {
			this.pageIndex = pageIndex;
			this.referencedRangeIndex = referencedRangeIndex;
			this.anchor = anchor;
		}
		public override SpreadsheetCursor Cursor { get { return anchor == FormulaRangeAnchor.TopLeft || anchor == FormulaRangeAnchor.BottomRight ? SpreadsheetCursors.ResizeNWSE : SpreadsheetCursors.ResizeNESW; } }
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			if (pageIndex != CalculatePageIndex(result))
				return;
			if (!handler.Control.InnerControl.IsInplaceEditorActive)
				return;
			InnerCellInplaceEditor inplaceEditor = handler.Control.InnerControl.InplaceEditor;
			MouseHandlerState dragState = new ResizeFormulaRangeManuallyMouseHandlerState(inplaceEditor.MouseHandler, result, referencedRangeIndex, anchor);
			BeginMouseDragHelperState newState = new BeginMouseDragHelperState(handler, dragState, result.PhysicalPoint);
			handler.SwitchStateCore(newState, result.PhysicalPoint);
		}
		public override void Visit(IHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
		int CalculatePageIndex(SpreadsheetHitTestResult result) {
			DocumentLayout documentLayout = result.DocumentLayout;
			if (documentLayout == null)
				return -1;
			Page page = result.Page;
			if (page == null)
				return -1;
			return documentLayout.Pages.IndexOf(page);
		}
	}
	#endregion
	#region MailMergeRangeResize
	public class MailMergeRangeResizeHotZone : HotZone {
		#region
		readonly SpreadsheetCursor cursor;
		bool top;
		bool left;
		string mailMergeDefinedName;
		#endregion
		public MailMergeRangeResizeHotZone(IGestureStateIndicator gestureStateIndicator, bool top, bool left, string mailMergeDefinedName)
			: base(gestureStateIndicator) {
			this.top = top;
			this.left = left;
			cursor = top == left ? SpreadsheetCursors.ResizeNWSE : SpreadsheetCursors.ResizeNESW;
			this.mailMergeDefinedName = mailMergeDefinedName;
		}
		public override SpreadsheetCursor Cursor { get { return cursor; } }
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			MouseHandlerState resizeState = handler.CreateMailMergreResizeRangeState(result, top, left, mailMergeDefinedName);
			BeginMouseDragHelperState newState = new BeginMouseDragHelperState(handler, resizeState, result.PhysicalPoint);
			handler.SwitchStateCore(newState, result.PhysicalPoint);
		}
		public override void Visit(IHotZoneVisitor visitor) {
		}
	}
	#endregion
	#region MailMergeRangeMove
	public class MailMergeRangeMoveHotZone : HotZone {
		string mailMergeDefinedName;
		public MailMergeRangeMoveHotZone(IGestureStateIndicator gestureStateIndicator, string mailMergeDefinedName)
			: base(gestureStateIndicator) {
			this.mailMergeDefinedName = mailMergeDefinedName;
		}
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.DragRange; } }
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			MouseHandlerState resizeState = handler.CreateMailMergreDragRangeState(result, mailMergeDefinedName);
			BeginMouseDragHelperState newState = new BeginMouseDragHelperState(handler, resizeState, result.PhysicalPoint);
			handler.SwitchStateCore(newState, result.PhysicalPoint);
		}
		public override void Visit(IHotZoneVisitor visitor) {
		}
	}
	#endregion
	#region HotZoneCollection
	public class HotZoneCollection : List<HotZone> {
	}
	#endregion
}
