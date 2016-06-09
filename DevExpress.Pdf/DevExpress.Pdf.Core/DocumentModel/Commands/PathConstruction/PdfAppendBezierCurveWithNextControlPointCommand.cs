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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfAppendBezierCurveWithNextControlPointCommand : PdfCommand {
		internal const string Name = "y";
		readonly double x1;
		readonly double y1;
		readonly double x3;
		readonly double y3;
		public double X1 { get { return x1; } }
		public double Y1 { get { return y1; } }
		public double X3 { get { return x3; } }
		public double Y3 { get { return y3; } }
		public PdfAppendBezierCurveWithNextControlPointCommand(double x1, double y1, double x3, double y3) {
			this.x1 = x1;
			this.y1 = y1;
			this.x3 = x3;
			this.y3 = y3;
		}
		internal PdfAppendBezierCurveWithNextControlPointCommand(PdfOperands operands) {
			x1 = operands.PopDouble();
			y1 = operands.PopDouble();
			x3 = operands.PopDouble();
			y3 = operands.PopDouble();
		}
		protected internal override void Write(PdfResources resources, PdfDocumentWritableStream writer) {
			writer.WriteSpace();
			writer.WriteDouble(x1);
			writer.WriteSpace();
			writer.WriteDouble(y1);
			writer.WriteSpace();
			writer.WriteDouble(x3);
			writer.WriteSpace();
			writer.WriteDouble(y3);
			writer.WriteSpace();
			writer.WriteString(Name);
		}
		protected internal override void Execute(PdfCommandInterpreter interpreter) {
			interpreter.AppendPathBezierSegment(new PdfPoint(x1, y1), new PdfPoint(x3, y3), new PdfPoint(x3, y3));
		}
	}
}
