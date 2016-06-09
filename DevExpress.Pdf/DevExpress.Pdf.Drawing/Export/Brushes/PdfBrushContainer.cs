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
	public abstract class PdfBrushContainer : PdfDisposableObject {
		public static PdfBrushContainer CreateFromGdiBrush(Brush brush) {
			SolidBrush solidBrush = brush as SolidBrush;
			if (solidBrush != null)
				return new PdfSolidBrushContainer(solidBrush);
			HatchBrush hatchBrush = brush as HatchBrush;
			if (hatchBrush != null)
				return new PdfHatchBrushContainer(hatchBrush);
			LinearGradientBrush linearGradientBrush = brush as LinearGradientBrush;
			if (linearGradientBrush != null)
				return new PdfLinearGradientBrushContainer(linearGradientBrush);
			PathGradientBrush pathGradientBrush = brush as PathGradientBrush;
			if (pathGradientBrush != null)
				return PdfPathGradientBrushContainer.CreateFromPathGradientBrush(pathGradientBrush);
			TextureBrush textureBrush = brush as TextureBrush;
			if (textureBrush != null)
				return new PdfTextureBrushContainer(textureBrush);
			return null;
		}
		internal abstract PdfBrush GetBrush(PdfGraphicsCommandConstructor context);
		protected override void Dispose(bool disposing) {
		}
	}
}
