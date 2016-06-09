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

using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
namespace DevExpress.Pdf.Drawing {
	public class PdfPen {
		static IDictionary<DashStyle, float[]> dashPatterns;
		static PdfPen() {
			dashPatterns = new Dictionary<DashStyle, float[]>();
			using (Pen pen = new Pen(Color.Black)) {
				pen.DashStyle = DashStyle.Dot;
				dashPatterns.Add(DashStyle.Dot, pen.DashPattern);
				pen.DashStyle = DashStyle.DashDotDot;
				dashPatterns.Add(DashStyle.DashDotDot, pen.DashPattern);
				pen.DashStyle = DashStyle.DashDot;
				dashPatterns.Add(DashStyle.DashDot, pen.DashPattern);
				pen.DashStyle = DashStyle.Dash;
				dashPatterns.Add(DashStyle.Dash, pen.DashPattern);
			}
		}
		PdfBrushContainer brushContainer;
		double width;
		PenAlignment aligment;
		LineCap startCap;
		LineCap endCap;
		double miterLimit = 10;
		DashCap dashCap;
		DashStyle dashStyle;
		double dashOffset;
		float[] dashPattern;
		LineJoin lineJoin;
		PdfTransformationMatrix transform = new PdfTransformationMatrix();
		public double Width { get { return width; } }
		public PdfBrushContainer Brush {
			get { return brushContainer; }
			set { brushContainer = value; }
		}
		public PenType PenType { get { return PenType.SolidColor; } }
		public PenAlignment Alignment {
			get { return aligment; }
			set { aligment = value; }
		}
		public LineCap StartCap {
			get { return startCap; }
			set { startCap = value; }
		}
		public LineCap EndCap {
			get { return endCap; }
			set { endCap = value; }
		}
		public double MiterLimit {
			get { return miterLimit; }
			set { miterLimit = value; }
		}
		public DashCap DashCap {
			get { return dashCap; }
			set { dashCap = value; }
		}
		public DashStyle DashStyle {
			get { return dashStyle; }
			set {
				if (value != DashStyle.Custom && value != DashStyle.Solid)
					dashPattern = dashPatterns[value];
				dashStyle = value;
			}
		}
		public double DashOffset {
			get { return dashOffset; }
			set { dashOffset = value; }
		}
		public float[] DashPattern {
			get { return dashStyle == DashStyle.Solid ? null : dashPattern; }
			set {
				dashStyle = DashStyle.Custom;
				dashPattern = value;
			}
		}
		public LineJoin LineJoin {
			get { return lineJoin; }
			set { lineJoin = value; }
		}
		public PdfTransformationMatrix Transform {
			get { return transform; }
			set { transform = value; }
		}
		public PdfPen(Color color)
			: this(color, 1) {
		}
		public PdfPen(Color color, double width) 
			: this(new PdfSolidBrushContainer(color), width) {
		}
		public PdfPen(PdfBrushContainer brushContainer) 
			: this(brushContainer, 1) {
		}
		public PdfPen(PdfBrushContainer brushContainer, double width) {
			this.brushContainer = brushContainer;
			this.width = width;
		}
	}
}
