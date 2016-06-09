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
	public enum PdfTextSearchStatus { Found, NotFound, Finished }
	public class PdfTextSearchResults {
		readonly PdfPage page;
		readonly int pageNumber;
		readonly IList<PdfWord> words;
		readonly IList<PdfOrientedRectangle> rectangles;
		readonly PdfTextSearchStatus status;
		PdfRectangle boundingRectangle;
		public PdfPage Page { get { return page; } }
		[Obsolete("The PageIndex property is now obsolete. Use the PageNumber property instead.")]
		public int PageIndex { get { return pageNumber - 1; } }
		public int PageNumber { get { return pageNumber; } }
		public IList<PdfWord> Words { get { return words; } }
		public IList<PdfOrientedRectangle> Rectangles { get { return rectangles; } }
		public PdfTextSearchStatus Status { get { return status; } }
		internal PdfRectangle BoundingRectangle {
			get {
				if (boundingRectangle != null)
					return boundingRectangle;
				if (rectangles.Count == 0)
					return new PdfRectangle(0, 0, 1, 1);
				double minx = Double.MaxValue;
				double miny = Double.MaxValue;
				double maxx = Double.MinValue;
				double maxy = Double.MinValue;
				foreach (PdfOrientedRectangle rect in rectangles) {
					minx = PdfMathUtils.Min(minx, rect.Left);
					miny = PdfMathUtils.Min(miny, rect.Bottom);
					maxx = PdfMathUtils.Max(maxx, rect.Right);
					maxy = PdfMathUtils.Max(maxy, rect.Top);
				}
				boundingRectangle = new PdfRectangle(PdfMathUtils.Min(minx, maxx), PdfMathUtils.Min(miny, maxy), PdfMathUtils.Max(minx, maxx), PdfMathUtils.Max(miny, maxy));
				return boundingRectangle;
			}
		}
		internal PdfPoint Position {
			get {
				PdfPoint topLeft = BoundingRectangle.TopLeft;
				return new PdfPoint(topLeft.X + boundingRectangle.Width / 2, topLeft.Y - boundingRectangle.Height / 2); 
			} 
		}
		internal PdfTextSearchResults(PdfPage page, int pageNumber, IList<PdfWord> words, IList<PdfOrientedRectangle> rectangles, PdfTextSearchStatus status) {
			this.page = page;
			this.pageNumber = pageNumber;
			this.words = words;
			this.rectangles = rectangles;
			this.status = status;
		}
	}
}
