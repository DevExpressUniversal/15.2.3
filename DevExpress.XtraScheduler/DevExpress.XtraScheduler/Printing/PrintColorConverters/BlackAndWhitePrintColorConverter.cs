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
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public class BlackAndWhitePrintColorConverter : PrintColorConverter {
		public BlackAndWhitePrintColorConverter()
			: this(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ColorConverterBlackAndWhite)) {
		}
		protected internal BlackAndWhitePrintColorConverter(string displayName)
			: base(displayName) {
		}
		protected internal override void Initialize() {
			base.Initialize();
			ApplyToAllDayArea = false;
			ApplyToAppointment = false;
			ApplyToAppointmentStatus = false;
			ApplyToHeader = false;
			ApplyToTimeRuler = false;
			ApplyToCells = false;
		}
		protected override PrintColorConverter CreateInstance() {
			return new BlackAndWhitePrintColorConverter(DisplayName);
		}
		public override Color ConvertColor(Color color) {
			int luma = CalcLuma(color) < 127 ? 0 : 255;
			return Color.FromArgb(luma, luma, luma);
		}
		public override Image GetConvertedImage(Image image) {
			Image grayScaleImage = PrintColorConverter.GrayScaleColor.GetConvertedImage(image);
			Bitmap bmp = grayScaleImage as Bitmap;
			if (bmp == null) {
				grayScaleImage.Dispose();
				return (Image)image.Clone();
			}
			for (int i = 0; i < bmp.Height; i++) {
				for (int j = 0; j < bmp.Width; j++) {
					Color c = bmp.GetPixel(j, i);
					int luma = c.B;
					int alpha = c.A;
					int realLuma = luma * alpha + 255 * (255 - alpha);
					realLuma = realLuma > 192 * 255 ? 255 : 0;
					bmp.SetPixel(j, i, Color.FromArgb(255 - realLuma, realLuma, realLuma, realLuma));
				}
			}
			return bmp;
		}
	}
}
