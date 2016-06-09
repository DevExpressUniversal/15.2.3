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
using System.Drawing.Drawing2D;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfPathGradientBrush : PdfTilingBrush {
		const byte graphicsPathPointTypeMask = 0x7;
		readonly PdfGraphicsPathData path;
		Color centerColor = Color.White;
		PdfPoint? centerPoint;
		Color[] surroundColors = new[] { Color.White };
		PdfTransformationMatrix transform = new PdfTransformationMatrix();
		WrapMode wrapMode = WrapMode.Clamp;
		PdfBlend blend = new PdfBlend() { Factors = new[] { 0, 1.0 }, Positions = new[] { 0, 1.0 } };
		PdfColorBlend colorBlend;
		PdfPoint focusScales;
		public PdfBlend Blend {
			get { return blend; }
			set { blend = value; }
		}
		public PdfColorBlend InterpolationColors {
			get { return colorBlend; }
			set { colorBlend = value; }
		}
		public PdfGraphicsPathData Path { get { return path; } }
		public PdfPoint FocusScales {
			get { return focusScales; }
			set { focusScales = value; }
		}
		public Color CenterColor {
			get { return centerColor; }
			set { centerColor = value; }
		}
		public PdfPoint CenterPoint {
			get { return centerPoint.GetValueOrDefault(new PdfPoint(Rectangle.Width / 2 + Rectangle.Left, Rectangle.Height / 2 + Rectangle.Top)); }
			set { centerPoint = value; }
		}
		public Color[] SurroundColors {
			get { return surroundColors; }
			set { surroundColors = value; }
		}
		public PdfRectangle Rectangle { get { return path.GetBounds(transform); } }
		public PdfPathGradientBrush(PdfGraphicsPathData path)
			: base(WrapMode.Clamp) {
			this.path = path;
		}
		public PdfPathGradientBrush(GraphicsPath path)
			: this() {
			if (path.PointCount == 0)
				this.path = new PdfGraphicsPathData();
			else {
				PdfGraphicsPathPointTypes[] types = Array.ConvertAll<byte, PdfGraphicsPathPointTypes>(path.PathTypes, b =>
					PdfEnumToValueConverter.Parse<PdfGraphicsPathPointTypes>(b & graphicsPathPointTypeMask));
				this.path = new PdfGraphicsPathData(path.PathPoints, types);
			}
		}
		public PdfPathGradientBrush(PointF[] points)
			: this() {
			if (points.Length < 3)
				throw new ArgumentException();
			PdfGraphicsPathPointTypes[] types = new PdfGraphicsPathPointTypes[points.Length];
			types[0] = PdfGraphicsPathPointTypes.StartSubPathPoint;
			for (int i = 1; i < points.Length; i++)
				types[i] = PdfGraphicsPathPointTypes.LineEndPoint;
			path = new PdfGraphicsPathData(points, types);
		}
		public PdfPathGradientBrush(PdfPathGradientBrush brush)
			: base(brush.WrapMode) {
			this.centerColor = brush.centerColor;
			this.centerPoint = brush.centerPoint;
			this.path = brush.path;
			this.surroundColors = brush.surroundColors;
			this.wrapMode = brush.wrapMode;
			this.transform = brush.transform;
			this.blend = brush.blend;
			this.colorBlend = brush.colorBlend;
			this.FocusScales = brush.FocusScales;
		}
		PdfPathGradientBrush()
			: base(WrapMode.Clamp) {
		}
		public override PdfTransparentColor GetColor(PdfRectangle bBox, PdfDocumentCatalog documentCatalog) {
			PdfPathGradientPatternConstructor constructor = new PdfPathGradientPatternConstructor(this, bBox);
			return new PdfTransparentColor(255, constructor.CreatePattern(documentCatalog));
		}
	}
}
