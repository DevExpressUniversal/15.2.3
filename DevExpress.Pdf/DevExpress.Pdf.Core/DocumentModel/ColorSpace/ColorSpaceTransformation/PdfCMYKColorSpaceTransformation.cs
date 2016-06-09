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

using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfCMYKColorSpaceTransformation : PdfColorSpaceTransformation {
		protected override int ComponentsCount { get { return 4; } }
		public PdfCMYKColorSpaceTransformation(int imageWidth, int imageHeight, int bitsPerComponent, IList<PdfRange> colorKeyMask) : base(imageWidth, imageHeight, bitsPerComponent, colorKeyMask) {
		}
		public override PdfColorSpaceTransformResult Transform(byte[] data) {
			if (BitsPerComponent != 8)
				return null;
			int dataLength = data.Length / 4;
			int length = dataLength * 3;
			byte[] rgbData = new byte[length];
			IList<PdfRange> colorKeyMask = ColorKeyMask;
			if (colorKeyMask == null) {
				for (int i = 0, src = 0, trg = 0; i < length; i += 3) {
					byte[] r = PdfRGBColor.FromCMYKBytes(data[src++], data[src++], data[src++], data[src++]);
					rgbData[trg++] = r[0];
					rgbData[trg++] = r[1];
					rgbData[trg++] = r[2];
				}
				return new PdfColorSpaceTransformResult(rgbData);
			}
			PdfRange range = colorKeyMask[0];
			int cyanMin = PdfMathUtils.ToInt32(range.Min);
			int cyanMax = PdfMathUtils.ToInt32(range.Max);
			range = colorKeyMask[1];
			int magentaMin = PdfMathUtils.ToInt32(range.Min);
			int magentaMax = PdfMathUtils.ToInt32(range.Max);
			range = colorKeyMask[2];
			int yellowMin = PdfMathUtils.ToInt32(range.Min);
			int yellowMax = PdfMathUtils.ToInt32(range.Max);
			range = colorKeyMask[3];
			int blackMin = PdfMathUtils.ToInt32(range.Min);
			int blackMax = PdfMathUtils.ToInt32(range.Max);
			byte[] maskData = new byte[dataLength];
			for (int i = 0, src = 0, trg = 0, mask = 0; i < length; i += 3) {
				byte cyan = data[src++];
				byte magenta = data[src++];
				byte yellow = data[src++];
				byte black = data[src++];
				byte[] r = PdfRGBColor.FromCMYKBytes(cyan, magenta, yellow, black);
				rgbData[trg++] = r[0];
				rgbData[trg++] = r[1];
				rgbData[trg++] = r[2];
				maskData[mask++] = (cyan >= cyanMin && cyan <= cyanMax && magenta >= magentaMin && magenta <= magentaMax && yellow >= yellowMin && yellow <= yellowMax && black >= blackMin && black <= blackMax) ? 
						(byte)0 : (byte)255;
			}
			return new PdfColorSpaceTransformResult(rgbData, maskData);
		}
	}
}
