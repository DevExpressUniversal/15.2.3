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
	public class PdfLinearGradientBrush : PdfTilingBrush {
		Color[] linearColors;
		PdfRectangle rectangle;
		PdfBlend blend;
		PdfColorBlend interpolationColors;
		public PdfColorBlend InterpolationColors {
			get { return interpolationColors; }
			set {
				blend = null;
				interpolationColors = value;
			}
		}
		public PdfBlend Blend {
			get { return blend; }
			set { blend = value; }
		}
		public Color[] LinearColors {
			get { return linearColors; }
			set { linearColors = new[] { value[0], value[1] }; }
		}
		public PdfRectangle Rectangle {
			get { return rectangle; }
			set { rectangle = value; }
		}
		public PdfLinearGradientBrush(PdfPoint point1, PdfPoint point2, Color color1, Color color2)
			: this(new PdfRectangle(point1.X, point1.Y, point2.X - point1.X, point2.Y - point1.Y), color1, color2) {
		}
		public PdfLinearGradientBrush(PdfRectangle rectangle, Color color1, Color color2)
			: base(WrapMode.Tile) {
			this.rectangle = rectangle;
			this.linearColors = new[] { color1, color2 };
			blend = new PdfBlend(1);
			blend.Factors = new[] { 1.0 };
			blend.Positions = new[] { 0.0 };
		}
		public override PdfTransparentColor GetColor(PdfRectangle bBox, PdfDocumentCatalog documentCatalog) {
			PdfLinearGradientPatternConstructor constructor = PdfLinearGradientPatternConstructor.Create(this, bBox);
			byte alpha = 255;
			if (linearColors[0].A == linearColors[1].A)
				alpha = linearColors[1].A;
			return new PdfTransparentColor(alpha, constructor.CreatePattern(documentCatalog));
		}
	}
}
