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
using System.Collections.Generic;
namespace DevExpress.Pdf.Drawing {
	public class PdfGraphicsPathData {
		readonly List<PdfPoint> points;
		readonly List<PdfGraphicsPathPointTypes> types;
		public PdfPoint[] Points { get { return points.ToArray(); } }
		public PdfGraphicsPathPointTypes[] PathTypes { get { return types.ToArray(); } }
		public PdfGraphicsPathData() {
			this.points = new List<PdfPoint>();
			this.types = new List<PdfGraphicsPathPointTypes>();
		}
		public PdfGraphicsPathData(PdfPoint[] points, PdfGraphicsPathPointTypes[] types) {
			this.points = new List<PdfPoint>(points);
			this.types = new List<PdfGraphicsPathPointTypes>(types);
		}
		public PdfGraphicsPathData(PointF[] points, PdfGraphicsPathPointTypes[] types) {
			this.points = new List<PdfPoint>(Array.ConvertAll<PointF, PdfPoint>(points, p => new PdfPoint(p.X, p.Y)));
			this.types = new List<PdfGraphicsPathPointTypes>(types);
		}
		public PdfRectangle GetBounds(PdfTransformationMatrix transform) {
			if (points.Count < 1)
				return new PdfRectangle(0, 0, 0, 0);
			PdfPoint[] transformedPoints = Points;
			transform.TransformPoints(transformedPoints);
			double minX = transformedPoints[0].X;
			double minY = transformedPoints[0].Y;
			double maxX = transformedPoints[0].X;
			double maxY = transformedPoints[0].Y;
			for (int i = 1; i < transformedPoints.Length; i++) {
				minX = Math.Min(transformedPoints[i].X, minX);
				minY = Math.Min(transformedPoints[i].Y, minY);
				maxX = Math.Max(transformedPoints[i].X, maxX);
				maxY = Math.Max(transformedPoints[i].Y, maxY);
			}
			return new PdfRectangle(minX, minY, maxX, maxY);
		}
	}
}
