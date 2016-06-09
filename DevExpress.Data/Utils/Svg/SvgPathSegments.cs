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

using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.Utils.Svg {
	public abstract class SvgPathSegment {
		public PointF Start { get; set; }
		public PointF End { get; set; }
		public abstract void AddToPath(GraphicsPath path, float scale, float offset);
		public PointF ScaleAndOffset(PointF point, float scale, float offset) {
			if(scale == 1.0f)
				return new PointF(point.X + offset, point.Y + offset);
			return new PointF((point.X + offset) * scale, (point.Y + offset) * scale);
		}
	}
	public class SvgPathMoveToSegment : SvgPathSegment {
		public SvgPathMoveToSegment(PointF moveToPoint) {
			Start = moveToPoint;
			End = moveToPoint;
		}
		public override void AddToPath(GraphicsPath path, float scale, float offset) { }
	}
	public class SvgPathCloseSegment : SvgPathSegment {
		public SvgPathCloseSegment() {
			End = PointF.Empty;
		}
		public override void AddToPath(GraphicsPath path, float scale, float offset) {
			path.CloseFigure();
		}
	}
	public class SvgPathLineToSegment : SvgPathSegment {
		public SvgPathLineToSegment(PointF start, PointF end) {
			Start = start;
			End = end;
		}
		public override void AddToPath(GraphicsPath path, float scale, float offset) {
			path.AddLine(
				ScaleAndOffset(Start, scale, offset),
				ScaleAndOffset(End, scale, offset));
		}
	}
	public class SvgPathCurveToCubicSegment : SvgPathSegment {
		public SvgPathCurveToCubicSegment(PointF start, PointF firstAdditionalPoint, PointF secondAdditionalPoint, PointF end) {
			Start = start;
			FirstAdditionalPoint = firstAdditionalPoint;
			SecondAdditionalPoint = secondAdditionalPoint;
			End = end;
		}
		public PointF FirstAdditionalPoint { get; set; }
		public PointF SecondAdditionalPoint { get; set; }
		public override void AddToPath(GraphicsPath path, float scale, float offset) {
			path.AddBezier(
				ScaleAndOffset(Start, scale, offset),
				ScaleAndOffset(FirstAdditionalPoint, scale, offset),
				ScaleAndOffset(SecondAdditionalPoint, scale, offset),
				ScaleAndOffset(End, scale, offset));
		}
	}
	public class SvgPathCurveToQuadraticSegment : SvgPathSegment {
		public SvgPathCurveToQuadraticSegment(PointF start, PointF additionalPoint, PointF end) {
			Start = start;
			AdditionalPoint = additionalPoint;
			End = end;
		}
		public PointF AdditionalPoint { get; set; }
		PointF firstAdditionalPointCore;
		public PointF FirstAdditionalPoint {
			get {
				if(firstAdditionalPointCore == PointF.Empty) {
					float x = Start.X + 2f / 3f * (AdditionalPoint.X - Start.X);
					float y = Start.Y + 2f / 3f * (AdditionalPoint.Y - Start.Y);
					firstAdditionalPointCore = new PointF(x, y);
				}
				return firstAdditionalPointCore;
			}
		}
		PointF secondAdditionalPointCore;
		public PointF SecondAdditionalPoint {
			get {
				if(secondAdditionalPointCore == PointF.Empty) {
					float x = End.X + 2f / 3f * (AdditionalPoint.X - End.X);
					float y = End.Y + 2f / 3f * (AdditionalPoint.Y - End.Y);
					secondAdditionalPointCore = new PointF(x, y);
				}
				return secondAdditionalPointCore;
			}
		}
		public override void AddToPath(GraphicsPath path, float scale, float offset) {
			path.AddBezier(
				ScaleAndOffset(Start, scale, offset),
				ScaleAndOffset(FirstAdditionalPoint, scale, offset),
				ScaleAndOffset(SecondAdditionalPoint, scale, offset),
				ScaleAndOffset(End, scale, offset));
		}
	}
	public class SvgPathArcSegment : SvgPathSegment {
		public override void AddToPath(GraphicsPath path, float scale, float offset) { }
	}
}
