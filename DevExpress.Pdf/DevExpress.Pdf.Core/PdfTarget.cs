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

namespace DevExpress.Pdf.Native {
	public enum PdfTargetMode { XYZ, Fit, FitHorizontally, FitVertically, FitRectangle, FitBBox, FitBBoxHorizontally, FitBBoxVertically }
	public class PdfTarget {
		readonly PdfTargetMode mode;
		readonly int pageIndex;
		readonly double? x;
		readonly double? y;
		readonly double width;
		readonly double height;
		readonly double? zoom;
		public PdfTargetMode Mode { get { return mode; } }
		public int PageIndex { get { return pageIndex; } }
		public double? X { get { return x; } }
		public double? Y { get { return y; } }
		public double Width { get { return width; } }
		public double Height { get { return height; } }
		public double? Zoom { get { return zoom; } }
		PdfTarget(PdfTargetMode mode, int pageIndex, double? x, double? y, double width, double height, double? zoom) {
			this.mode = mode;
			this.pageIndex = pageIndex;
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			this.zoom = zoom;
		}
		public PdfTarget(int pageIndex, double? x, double? y, double? zoom) : this(PdfTargetMode.XYZ, pageIndex, x, y, 0, 0, zoom) {
		}
		public PdfTarget(PdfTargetMode mode, int pageIndex, PdfRectangle rectangle) : this(mode, pageIndex, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height, null) {
		}
		public PdfTarget(PdfTargetMode mode, int pageIndex) : this(mode, pageIndex, null, null, 0, 0, null) {
		}
		public PdfTarget(PdfTargetMode mode, int pageIndex, double? x, double? y) : this(mode, pageIndex, x, y, 0, 0, null) {
		}
	}
}
