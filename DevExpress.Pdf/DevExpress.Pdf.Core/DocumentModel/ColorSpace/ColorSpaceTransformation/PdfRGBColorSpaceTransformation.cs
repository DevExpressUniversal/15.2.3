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
	public class PdfRGBColorSpaceTransformation : PdfColorSpaceTransformation {
		protected override int ComponentsCount { get { return 3; } }
		public PdfRGBColorSpaceTransformation(int imageWidth, int imageHeight, int bitsPerComponent, IList<PdfRange> colorKeyMask) : base(imageWidth, imageHeight, bitsPerComponent, colorKeyMask) {
		}
		public override PdfColorSpaceTransformResult Transform(byte[] data) {
			if (BitsPerComponent != 8)
				data = UnpackData(data);
			IList<PdfRange> colorKeyMask = ColorKeyMask;
			if (colorKeyMask == null || colorKeyMask.Count < 3)
				return new PdfColorSpaceTransformResult(data);
			PdfRange range = colorKeyMask[0];
			int redMin = PdfMathUtils.ToInt32(range.Min);
			int redMax = PdfMathUtils.ToInt32(range.Max);
			range = colorKeyMask[1];
			int greenMin = PdfMathUtils.ToInt32(range.Min);
			int greenMax = PdfMathUtils.ToInt32(range.Max);
			range = colorKeyMask[2];
			int blueMin = PdfMathUtils.ToInt32(range.Min);
			int blueMax = PdfMathUtils.ToInt32(range.Max);
			int length = data.Length / 3;
			byte[] maskData = new byte[length];
			for (int i = 0, src = 0; i < length; i++) {
				byte red = data[src++];
				byte green = data[src++];
				byte blue = data[src++];
				maskData[i] = (red >= redMin && red <= redMax && green >= greenMin && green <= greenMax && blue >= blueMin && blue <= blueMax) ? (byte)0 : (byte)255;
			}
			return new PdfColorSpaceTransformResult(data, maskData);
		}
	}
}
