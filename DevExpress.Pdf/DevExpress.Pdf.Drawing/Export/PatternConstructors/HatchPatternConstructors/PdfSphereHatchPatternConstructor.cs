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
namespace DevExpress.Pdf.Drawing {
	public class PdfSphereHatchPatternConstructor : PdfRectangleBasedHatchPatternConstructor {
		static PdfHatchPatternRect[] rectangles;
		static PdfSphereHatchPatternConstructor() {
			rectangles = new PdfHatchPatternRect[12];
			int index = 0;
			for (int i = 0; i < 2; i++) {
				int b = i * 4;
				for (int j = 0; j < 2; j++) {
					int y = 1 + 4 * j;
					rectangles[index++] = new PdfHatchPatternRect(b, y, 1, 3);
					rectangles[index++] = new PdfHatchPatternRect(y, b, 3, 1);
				}
			}
			rectangles[index++] = new PdfHatchPatternRect(1, 6, 2, 2);
			rectangles[index++] = new PdfHatchPatternRect(3, 5, 1, 3);
			rectangles[index++] = new PdfHatchPatternRect(4, 2, 3, 2);
			rectangles[index++] = new PdfHatchPatternRect(7, 1, 1, 3);
		}
		public PdfSphereHatchPatternConstructor(Color foreColor, Color backColor)
			: base(foreColor, backColor, rectangles) {
		}
	}
}
