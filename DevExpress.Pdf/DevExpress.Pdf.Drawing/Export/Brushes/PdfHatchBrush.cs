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
using System.Drawing.Drawing2D;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfHatchBrush : PdfBrush {
		HatchStyle style;
		Color foreColor;
		Color backColor;
		public HatchStyle HatchStyle { get { return style; } }
		public Color ForegroundColor { get { return foreColor; } }
		public Color BackgroundColor { get { return backColor; } }
		public PdfHatchBrush(HatchStyle style, Color foreColor, Color backColor) {
			this.style = style;
			this.foreColor = foreColor;
			this.backColor = backColor;
		}
		public override PdfTransparentColor GetColor(PdfRectangle bBox, PdfDocumentCatalog documentCatalog) {
			PdfHatchPatternConstructor constructor = PdfHatchPatternConstructor.Create(this);
			return new PdfTransparentColor(255, constructor.CreatePattern(bBox, documentCatalog));
		}
	}
}
