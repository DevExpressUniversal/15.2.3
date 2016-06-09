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

using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public abstract class FreePositionResizing : ResizingOperation {
		protected FreePosition ShapePosition { get { return Annotation.ShapePosition as FreePosition; } }
		public FreePositionResizing(Annotation annotation, ResizeDirection resizeDirection) : base(annotation, resizeDirection) {
		}
		protected abstract void CalcNewSize(DiagramPoint cursorPoint, out int width, out int height);
		protected abstract void CorrectShapePosition(int dx, int dy);
		public override void Run(int x, int y, int dx, int dy) {
			if (Annotation == null || x < 0 || y < 0)
				return;
			int newWidth, newHeight;
			CalcNewSize(CalcCursorPoint(x, y, dx, dy), out newWidth, out newHeight);
			dx = newWidth - Annotation.Width;
			dy = newHeight - Annotation.Height;
			Annotation.SetWidth(newWidth);
			Annotation.SetHeight(newHeight);			
			CorrectShapePosition(dx, dy);
		}
	}
	public class FreePositionHorizontalResizing : FreePositionResizing {
		public FreePositionHorizontalResizing(Annotation annotation, ResizeDirection resizeDirection) : base(annotation, resizeDirection) {
		}
		protected override void CalcNewSize(DiagramPoint cursorPoint, out int width, out int height) {
			height = Annotation.Height;
			width = Annotation.Width;
			switch (ResizeDirection) {
				case ResizeDirection.Left:
					width = MathUtils.StrongRound(Annotation.LastLocation.X + Annotation.Width - cursorPoint.X);
					break;
				case ResizeDirection.Right:
					width = MathUtils.StrongRound(cursorPoint.X - Annotation.LastLocation.X);
					break;
				default:
					ChartDebug.Fail("The resize direction is incorrect for this operation.");
					return;
			}
			CorrectSize(ref width, ref height);
		}
		protected override void CorrectShapePosition(int dx, int dy) {
			int left, top, right, bottom;
			ShapePosition.GetIndents(out left, out top, out right, out bottom);
			switch (ResizeDirection) {
				case ResizeDirection.Left:
					if (ShapePosition.DockCorner == DockCorner.LeftTop || ShapePosition.DockCorner == DockCorner.LeftTop)
						ShapePosition.SetLeftIndent(left - dx);
					break;
				case ResizeDirection.Right:
					if (ShapePosition.DockCorner == DockCorner.RightTop || ShapePosition.DockCorner == DockCorner.RightTop)
						ShapePosition.SetRightIndent(right - dx);
					break;
				default:
					ChartDebug.Fail("The resize direction is incorrect for this operation.");
					return;
			}
		}
	}
	public class FreePositionVerticalResizing : FreePositionResizing {
		public FreePositionVerticalResizing(Annotation annotation, ResizeDirection resizeDirection) : base(annotation, resizeDirection) {
		}
		protected override void CalcNewSize(DiagramPoint cursorPoint, out int width, out int height) {
			height = Annotation.Height;
			width = Annotation.Width;
			switch (ResizeDirection) {
				case ResizeDirection.Top:
					height = MathUtils.StrongRound(Annotation.LastLocation.Y + Annotation.Height - cursorPoint.Y);
					break;
				case ResizeDirection.Bottom:
					height = MathUtils.StrongRound(cursorPoint.Y - Annotation.LastLocation.Y);
					break;
				default:
					ChartDebug.Fail("The resize direction is incorrect for this operation.");
					return;
			}
			CorrectSize(ref width, ref height);
		}
		protected override void CorrectShapePosition(int dx, int dy) {
			int left, top, right, bottom;
			ShapePosition.GetIndents(out left, out top, out right, out bottom);
			switch (ResizeDirection) {
				case ResizeDirection.Top:
					if (ShapePosition.DockCorner == DockCorner.LeftTop || ShapePosition.DockCorner == DockCorner.RightTop)
						ShapePosition.SetTopIndent(top - dy);
					break;
				case ResizeDirection.Bottom:
					if (ShapePosition.DockCorner == DockCorner.LeftBottom || ShapePosition.DockCorner == DockCorner.RightBottom)
						ShapePosition.SetBottomIndent(bottom - dy);
					break;
				default:
					ChartDebug.Fail("The resize direction is incorrect for this operation.");
					return;
			}
		}
	}
	public class FreePositionAllResizing : FreePositionResizing {
		public FreePositionAllResizing(Annotation annotation, ResizeDirection resizeDirection) : base(annotation, resizeDirection) {
		}
		protected override void CalcNewSize(DiagramPoint cursorPoint, out int width, out int height) {
			height = Annotation.Height;
			width = Annotation.Width;
			switch (ResizeDirection) {
				case ResizeDirection.LeftTop:
					width = MathUtils.StrongRound(Annotation.LastLocation.X + Annotation.Width - cursorPoint.X);
					height = MathUtils.StrongRound(Annotation.LastLocation.Y + Annotation.Height - cursorPoint.Y);
					break;
				case ResizeDirection.RightTop:
					width = MathUtils.StrongRound(cursorPoint.X - Annotation.LastLocation.X);
					height = MathUtils.StrongRound(Annotation.LastLocation.Y + Annotation.Height - cursorPoint.Y);
					break;
				case ResizeDirection.RightBottom:
					width = MathUtils.StrongRound(cursorPoint.X - Annotation.LastLocation.X);
					height = MathUtils.StrongRound(cursorPoint.Y - Annotation.LastLocation.Y);
					break;
				case ResizeDirection.LeftBottom:
					width = MathUtils.StrongRound(Annotation.LastLocation.X + Annotation.Width - cursorPoint.X);
					height = MathUtils.StrongRound(cursorPoint.Y - Annotation.LastLocation.Y);
					break;
				default:
					ChartDebug.Fail("The resize direction is incorrect for this operation.");
					return;
			}
			CorrectSize(ref width, ref height);
		}
		protected override void CorrectShapePosition(int dx, int dy) {
			int left, top, right, bottom;
			ShapePosition.GetIndents(out left, out top, out right, out bottom);
			switch (ResizeDirection) {
				case ResizeDirection.LeftTop:
					if (ShapePosition.DockCorner == DockCorner.LeftTop || ShapePosition.DockCorner == DockCorner.LeftBottom)
						ShapePosition.SetLeftIndent(left - dx);
					if (ShapePosition.DockCorner == DockCorner.LeftTop || ShapePosition.DockCorner == DockCorner.RightTop)
						ShapePosition.SetTopIndent(top - dy);
					break;
				case ResizeDirection.RightTop:
					if (ShapePosition.DockCorner == DockCorner.RightTop || ShapePosition.DockCorner == DockCorner.RightBottom)
						ShapePosition.SetRightIndent(right - dx);
					if (ShapePosition.DockCorner == DockCorner.LeftTop || ShapePosition.DockCorner == DockCorner.RightTop)
						ShapePosition.SetTopIndent(top - dy);
					break;
				case ResizeDirection.RightBottom:
					if (ShapePosition.DockCorner == DockCorner.RightTop || ShapePosition.DockCorner == DockCorner.RightBottom)
						ShapePosition.SetRightIndent(right - dx);
					if (ShapePosition.DockCorner == DockCorner.LeftBottom || ShapePosition.DockCorner == DockCorner.RightBottom)
						ShapePosition.SetBottomIndent(bottom - dy);
					break;
				case ResizeDirection.LeftBottom:
					if (ShapePosition.DockCorner == DockCorner.LeftTop || ShapePosition.DockCorner == DockCorner.LeftBottom)
						ShapePosition.SetLeftIndent(left - dx);
					if (ShapePosition.DockCorner == DockCorner.LeftBottom || ShapePosition.DockCorner == DockCorner.RightBottom)
						ShapePosition.SetBottomIndent(bottom - dy);
					break;
				default:
					ChartDebug.Fail("The resize direction is incorrect for this operation.");
					return;
			}
		}
	}
}
