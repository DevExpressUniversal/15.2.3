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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfCaret {
		readonly PdfTextPosition position;
		readonly PdfCaretViewData viewData;
		readonly PdfPoint startCoordinates;
		PdfRectangle boundingBox;
		public PdfTextPosition Position { get { return position; } }
		public PdfCaretViewData ViewData { get { return viewData; } }
		public PdfPoint StartCoordinates { get { return startCoordinates; } }
		public PdfRectangle BoundingBox { 
			get {
				if (boundingBox == null) {
					PdfPoint topLeft = viewData.TopLeft;
					double height = viewData.Height;
					double angle = viewData.Angle;
					double rotatedBottom = topLeft.Y - Math.Cos(angle) * height;
					double rotatedRight = topLeft.X + Math.Sin(angle) * height;
					double left = Math.Min(topLeft.X, rotatedRight);
					double bottom = Math.Min(topLeft.Y, rotatedBottom);
					double right = Math.Max(topLeft.X, rotatedRight);
					double top = Math.Max(topLeft.Y, rotatedBottom);
					boundingBox = new PdfRectangle(left, bottom, left == right ? left + 1 : right, bottom == top ? bottom + 1 : top);
				}
				return boundingBox;
			}
		}
		public PdfCaret(PdfTextPosition position, PdfCaretViewData viewData, PdfPoint startCoordinates) {
			this.position = position;
			this.viewData = viewData;
			this.startCoordinates = startCoordinates;
		}
	}
}
