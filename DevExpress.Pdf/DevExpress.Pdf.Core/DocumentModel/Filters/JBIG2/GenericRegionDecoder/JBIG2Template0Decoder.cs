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
	public class JBIG2Template0Decoder : JBIG2GenericRegionDecoder {
		public JBIG2Template0Decoder(JBIG2Image image, JBIG2Decoder decoder)
			: base(image, decoder) {
		}
		public override void Decode() {
			JBIG2Image image = Image;
			int gbw = image.Width;
			int gbh = image.Height;
			int rowstride = image.Stride;
			int gbregLine = 0;
			byte[] data = image.Data;
			if (gbw <= 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			for (int y = 0; y < gbh; y++) {
				int line1 = (y >= 1) ? data[gbregLine - rowstride] : 0;
				int line2 = ((y >= 2) ? data[gbregLine - rowstride * 2] << 6 : 0);
				int paddedWidth = (gbw + 7) & -8;
				int context = (line1 & 0x7f0) | (line2 & 0xf800);
				for (int x = 0; x < paddedWidth; x += 8) {
					byte result = 0;
					int minorWidth = gbw - x > 8 ? 8 : gbw - x;
					int xSlide = x >> 3;
					if (y >= 1)
						line1 = (line1 << 8) | (x + 8 < gbw ? data[gbregLine - rowstride + xSlide + 1] : 0);
					if (y >= 2)
						line2 = (line2 << 8) | (x + 8 < gbw ? data[gbregLine - rowstride * 2 + xSlide + 1] << 6 : 0);
					for (int xMinor = 0; xMinor < minorWidth; xMinor++) {
						int minorDif = 7 - xMinor;
						byte bit = (byte)(Decoder.DecodeGB(context) ? 1 : 0);
						result |= (byte)(bit << minorDif);
						context = ((context & 0x7bf7) << 1) |
								  bit |
								  ((line1 >> minorDif) & 0x10) |
								  ((line2 >> minorDif) & 0x800);
					}
					data[gbregLine + xSlide] = result;
				}
				gbregLine += rowstride;
			}
		}
	}
}
