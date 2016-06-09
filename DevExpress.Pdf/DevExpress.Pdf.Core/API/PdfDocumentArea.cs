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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfDocumentArea {
		public static PdfDocumentArea Create(PdfDocumentPosition position1, PdfDocumentPosition position2) {
			try {
				int pageNumber = position1.PageNumber;
				if (pageNumber != position2.PageNumber)
					return null;
				PdfPoint point1 = position1.Point;
				PdfPoint point2 = position2.Point;
				return new PdfDocumentArea(pageNumber, 
					new PdfRectangle(PdfMathUtils.Min(point1.X, point2.X), PdfMathUtils.Min(point1.Y, point2.Y), PdfMathUtils.Max(point1.X, point2.X), PdfMathUtils.Max(point1.Y, point2.Y)));
			}
			catch {
				return null;
			}
		}
		readonly int pageNumber;
		readonly PdfRectangle area;
		internal int PageIndex { get { return pageNumber - 1; } }
		public int PageNumber { get { return pageNumber; } }
		public PdfRectangle Area { get { return area; } }
		public PdfDocumentArea(int pageNumber, PdfRectangle area) {
			this.pageNumber = pageNumber;
			this.area = area;
		}
	}
}
