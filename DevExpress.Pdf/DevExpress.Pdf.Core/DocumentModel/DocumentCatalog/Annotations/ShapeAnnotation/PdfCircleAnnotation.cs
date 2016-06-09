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

using System;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfCircleAnnotation : PdfShapeAnnotation {
		internal const string Type = "Circle";
		static readonly double factor = 0.5 - (1 / Math.Sqrt(2) - 0.5) / 0.75;
		protected override string AnnotationType { get { return Type; } }
		internal PdfCircleAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(page, dictionary) {
		}
		protected override void GenerateShapeCommands(IList<PdfCommand> commands, PdfRectangle rect) {
			double left = rect.Left;
			double right = rect.Right;
			double centerX = (left + right) / 2;
			double bottom = rect.Bottom;
			double top = rect.Top;
			double centerY = (bottom + top) / 2;
			double horizontalOffset = (right  - left) * factor;
			double leftControlPoint = left + horizontalOffset;
			double rightControlPoint = right - horizontalOffset;
			double verticalOffset = (top - bottom) * factor;
			double bottomControlPoint = bottom + verticalOffset;
			double topControlPoint = top - verticalOffset;
			commands.Add(new PdfBeginPathCommand(new PdfPoint(right, centerY)));
			commands.Add(new PdfAppendBezierCurveCommand(right, topControlPoint, rightControlPoint, top, centerX, top));
			commands.Add(new PdfAppendBezierCurveCommand(leftControlPoint, top, left, topControlPoint, left, centerY));
			commands.Add(new PdfAppendBezierCurveCommand(left, bottomControlPoint, leftControlPoint, bottom, centerX, bottom));
			commands.Add(new PdfAppendBezierCurveCommand(rightControlPoint, bottom, right, bottomControlPoint, right, centerY));
			commands.Add(new PdfClosePathCommand());
		}
	}
}
