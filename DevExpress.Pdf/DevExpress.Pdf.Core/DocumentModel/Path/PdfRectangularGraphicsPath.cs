#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Collections.Generic;
namespace DevExpress.Pdf {
	public class PdfRectangularGraphicsPath : PdfGraphicsPath {
		PdfRectangle rectangle;
		public PdfRectangle Rectangle { get { return rectangle; } }
		protected internal override bool Flat { get { return true; } }
		public PdfRectangularGraphicsPath(double left, double bottom, double width, double height) : base(new PdfPoint(left, bottom)) {
			double right = left + width;
			double top = bottom + height;
			AppendLineSegment(new PdfPoint(right, bottom));
			AppendLineSegment(new PdfPoint(right, top));
			AppendLineSegment(new PdfPoint(left, top));
			AppendLineSegment(new PdfPoint(left, bottom));
			UpdateRectangle(left, bottom, right, top);
			Closed = true;
		}
		void UpdateRectangle(double left, double bottom, double right, double top) {
			if (right < left) {
				double temp = left;
				left = right;
				right = temp;
			}
			if (top < bottom) {
				double temp = bottom;
				bottom = top;
				top = temp;
			}
			rectangle = new PdfRectangle(left, bottom, right, top);
		}
		public override void Transform(PdfTransformationMatrix matrix) {
			base.Transform(matrix);
			PdfPoint bottomLeft = matrix.Transform(rectangle.BottomLeft);
			PdfPoint topRight = matrix.Transform(rectangle.TopRight);
			UpdateRectangle(bottomLeft.X, bottomLeft.Y, topRight.X, topRight.Y);
		}
		protected internal override void GeneratePathCommands(IList<PdfCommand> commands) {
			commands.Add(new PdfBeginPathCommand(StartPoint));
			commands.Add(new PdfAppendRectangleCommand(rectangle.Left, rectangle.Bottom, rectangle.Right, rectangle.Top));
		}
	}
}
