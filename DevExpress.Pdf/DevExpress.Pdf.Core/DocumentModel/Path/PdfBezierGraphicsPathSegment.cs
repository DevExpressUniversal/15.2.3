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
	public class PdfBezierGraphicsPathSegment : PdfGraphicsPathSegment {
		PdfPoint controlPoint1;
		PdfPoint controlPoint2;
		public PdfPoint ControlPoint1 { get { return controlPoint1; } }
		public PdfPoint ControlPoint2 { get { return controlPoint2; } }
		protected internal override bool Flat { get { return false; } }
		public PdfBezierGraphicsPathSegment(PdfPoint controlPoint1, PdfPoint controlPoint2, PdfPoint endPoint) : base(endPoint) {
			this.controlPoint1 = controlPoint1;
			this.controlPoint2 = controlPoint2;
		}
		protected internal override void Transform(PdfTransformationMatrix matrix) {
			base.Transform(matrix);
			controlPoint1 = matrix.Transform(controlPoint1);
			controlPoint2 = matrix.Transform(controlPoint2);
		}
		protected internal override void GeneratePathSegmentCommands(IList<PdfCommand> commands) {
			PdfPoint endPoint = EndPoint;
			commands.Add(new PdfAppendBezierCurveCommand(controlPoint1.X, controlPoint1.Y, controlPoint2.X, controlPoint2.Y, endPoint.X, endPoint.Y));
		}
	}
}
