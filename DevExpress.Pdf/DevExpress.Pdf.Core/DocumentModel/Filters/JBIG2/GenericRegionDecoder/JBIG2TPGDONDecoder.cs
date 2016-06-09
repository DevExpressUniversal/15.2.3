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

using System;
namespace DevExpress.Pdf.Native {
	public abstract class JBIG2TPGDONDecoder : JBIG2GenericRegionDecoder {
		internal static JBIG2TPGDONDecoder Create(JBIG2Image image, JBIG2Decoder decoder, int[] gbat, int gbtemplate) {
			switch (gbtemplate) {
				case 0:
					return new JBIG2TPGDON0Decoder(image, decoder, gbat);
				case 1:
					return new JBIG2TPGDON1Decoder(image, decoder, gbat);
				case 2:
					return new JBIG2TPGDON2Decoder(image, decoder, gbat);
				case 3:
					return new JBIG2TPGDON3Decoder(image, decoder, gbat);
			}
			PdfDocumentReader.ThrowIncorrectDataException();
			return null;
		}
		readonly int[] adaptiveTemplate;
		protected abstract int InitialContext { get; }
		protected int[] AdaptiveTemplate { get { return adaptiveTemplate; } }
		protected JBIG2TPGDONDecoder(JBIG2Image image, JBIG2Decoder decoder, int[] gbat)
			: base(image, decoder) {
			adaptiveTemplate = CreateAdaptiveTemplate(gbat);
		}
		protected abstract int[] CreateAdaptiveTemplate(int[] gbat);
		public override void Decode() {
			JBIG2Image image = Image;
			int[] adaptiveTemplate = AdaptiveTemplate;
			int ltp = 0;
			for (int y = 0; y < image.Height; y++) {
				bool bit = Decoder.DecodeGB(InitialContext);
				ltp ^= (bit ? 1 : 0);
				if (ltp == 0)
					for (int x = 0; x < image.Width; x++) {
						int context = 0;
						int i = 0;
						int j = 0;
						while (i < adaptiveTemplate.Length)
							context |= image.GetPixelInt(x + adaptiveTemplate[i++], y + adaptiveTemplate[i++]) << j++;
						bit = Decoder.DecodeGB(context);
						image.SetPixel(x, y, bit);
					}
				else
					CopyPrevRow(image, y);
			}
		}
		void CopyPrevRow(JBIG2Image image, int y) {
			if (y == 0)
				return;
			int src = (y - 1) * image.Stride;
			Array.Copy(image.Data, src, image.Data, src + image.Stride, image.Stride);
		}
	}
}
