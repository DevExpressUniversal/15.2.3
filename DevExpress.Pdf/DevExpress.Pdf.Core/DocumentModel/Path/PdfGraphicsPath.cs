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
	public class PdfGraphicsPath {
		readonly List<PdfGraphicsPathSegment> segments = new List<PdfGraphicsPathSegment>();
		PdfPoint startPoint;
		bool closed;
		public IList<PdfGraphicsPathSegment> Segments { get { return segments; } }
		public PdfPoint StartPoint { 
			get { return startPoint; } 
			internal set { startPoint = value; } 
		}
		internal PdfPoint EndPoint { 
			get {
				int count = segments.Count;
				return count == 0 ? startPoint : segments[count - 1].EndPoint;
			}
		}
		internal bool Closed { 
			get { return closed; } 
			set { closed = value; }
		}
		protected internal virtual bool Flat {
			get {
				double x = startPoint.X;
				double y = startPoint.Y;
				foreach (PdfGraphicsPathSegment segment in segments) {
					if (!segment.Flat)
						return false;
					PdfPoint endPoint = segment.EndPoint;
					double endX = endPoint.X;
					double endY = endPoint.Y;
					if (endX != x && endY != y)
						return false;
					x = endX;
					y = endY;
				}
				return true;
			}
		}
		public PdfGraphicsPath(PdfPoint startPoint) {
			this.startPoint = startPoint;
		}
		public void AppendLineSegment(PdfPoint endPoint) {
			segments.Add(new PdfLineGraphicsPathSegment(endPoint));
		}
		public void AppendBezierSegment(PdfPoint controlPoint1, PdfPoint controlPoint2, PdfPoint endPoint) {
			segments.Add(new PdfBezierGraphicsPathSegment(controlPoint1, controlPoint2, endPoint));
		}
		public virtual void Transform(PdfTransformationMatrix matrix) {
			startPoint = matrix.Transform(startPoint);
			foreach (PdfGraphicsPathSegment segment in segments)
				segment.Transform(matrix);
		}
		protected internal virtual void GeneratePathCommands(IList<PdfCommand> commands) {
			commands.Add(new PdfBeginPathCommand(startPoint));
			foreach (PdfGraphicsPathSegment segment in segments)
				segment.GeneratePathSegmentCommands(commands);
			if (closed)
				commands.Add(new PdfClosePathCommand());
		}
	}
}
