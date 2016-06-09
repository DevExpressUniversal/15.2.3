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
	public abstract class RelativePositionResizing : ResizingOperation {
		public RelativePositionResizing(Annotation annotation, ResizeDirection resizeDirection) : base(annotation, resizeDirection) {
		}
	}
	public class RelativePositionHorizontalResizing : RelativePositionResizing {
		public RelativePositionHorizontalResizing(Annotation annotation, ResizeDirection resizeDirection) : base(annotation, resizeDirection) {
		}
		public override void Run(int x, int y, int dx, int dy) {
			if (Annotation == null || x < 0 || y < 0)
				return;
			DiagramPoint cursorPoint = CalcCursorPoint(x, y, dx, dy);
			int width = Annotation.Width;
			int height = Annotation.Height;
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
			width = 2 * width - Annotation.Width;
			CorrectSize(ref width, ref height);
			Annotation.SetWidth(width);
		}
	}
	public class RelativePositionVerticalResizing : RelativePositionResizing {
		public RelativePositionVerticalResizing(Annotation annotation, ResizeDirection resizeDirection) : base(annotation, resizeDirection) {
		}
		public override void Run(int x, int y, int dx, int dy) {
			if (Annotation == null || x < 0 || y < 0)
				return;
			DiagramPoint cursorPoint = CalcCursorPoint(x, y, dx, dy);
			int width = Annotation.Width;
			int height = Annotation.Height;
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
			height = 2 * height - Annotation.Height;
			CorrectSize(ref width, ref height);
			Annotation.SetHeight(height);
		}
	}
	public class RelativePositionAllResizing : RelativePositionResizing {
		public RelativePositionAllResizing(Annotation annotation, ResizeDirection resizeDirection) : base(annotation, resizeDirection) {
		}
		public override void Run(int x, int y, int dx, int dy) {
			if (Annotation == null || x < 0 || y < 0)
				return;
			DiagramPoint cursorPoint = CalcCursorPoint(x, y, dx, dy);
			int width = Annotation.Width;
			int height = Annotation.Height;
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
			width = 2 * width - Annotation.Width;
			height = 2 * height - Annotation.Height;
			CorrectSize(ref width, ref height);
			Annotation.SetWidth(width);
			Annotation.SetHeight(height);
		}
	}
}
