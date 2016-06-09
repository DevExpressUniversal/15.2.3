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

using DevExpress.XtraScheduler.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public class GrayScalePrintColorConverter : PrintColorConverter {
		static ColorMatrix grayScaleMatrix = new ColorMatrix(new float[5][]{
																				   new float[5] {0.3f, 0.3f, 0.3f, 0, 0},
																				   new float[5] {0.59f, 0.59f, 0.59f, 0, 0},
																				   new float[5] {0.11f, 0.11f, 0.11f, 0, 0},
																				   new float[5] {0, 0, 0, 1, 0},
																				   new float[5] {0, 0, 0, 0, 1}
																			   });
		static ImageAttributes grayScaleImageAttr = new ImageAttributes();
		static GrayScalePrintColorConverter() {
			grayScaleImageAttr.SetColorMatrix(grayScaleMatrix);
		}
		public GrayScalePrintColorConverter()
			: this(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ColorConverterGrayScale)) {
		}
		protected internal GrayScalePrintColorConverter(string displayName)
			: base(displayName) {
		}
		public override Color ConvertColor(Color color) {
			int luma = CalcLuma(color);
			return Color.FromArgb(luma, luma, luma);
		}
		public override Image GetConvertedImage(Image image) {
			Bitmap bmp = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bmp);
			g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, grayScaleImageAttr);
			g.Dispose();
			return bmp;
		}
		protected override PrintColorConverter CreateInstance() {
			return new GrayScalePrintColorConverter(DisplayName);
		}
	}
}
