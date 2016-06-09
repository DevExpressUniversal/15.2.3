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
	public class JBIG2Template3Decoder : JBIG2GenericRegionDecoder {
		readonly int gbat0;
		readonly int gbat1;
		public JBIG2Template3Decoder(JBIG2Image image, JBIG2Decoder decoder, int gbat0, int gbat1)
			: base(image, decoder) {
			this.gbat0 = gbat0;
			this.gbat1 = gbat1;
		}
		public override void Decode() {
			JBIG2Image image = Image;
			int context;
			for (int y = 0; y < image.Height; y++) {
				for (int x = 0; x < image.Width; x++) {
					context = image.GetPixelInt(x - 1, y);
					context |= image.GetPixelInt(x - 2, y) << 1;
					context |= image.GetPixelInt(x - 3, y) << 2;
					context |= image.GetPixelInt(x - 4, y) << 3;
					context |= image.GetPixelInt(x + gbat0, y + gbat1) << 4;
					context |= image.GetPixelInt(x + 1, y - 1) << 5;
					context |= image.GetPixelInt(x + 0, y - 1) << 6;
					context |= image.GetPixelInt(x - 1, y - 1) << 7;
					context |= image.GetPixelInt(x - 2, y - 1) << 8;
					context |= image.GetPixelInt(x - 3, y - 1) << 9;
					bool bit = Decoder.DecodeGB(context);
					image.SetPixel(x, y, bit);
				}
			}
		}
	}
}
