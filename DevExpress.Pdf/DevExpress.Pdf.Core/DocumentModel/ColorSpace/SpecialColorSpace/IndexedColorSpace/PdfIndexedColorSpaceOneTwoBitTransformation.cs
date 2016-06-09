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
	public class PdfIndexedColorSpaceOneTwoBitTransformation : PdfIndexedColorSpaceTransformation {
		readonly int bitsPerComponent;
		readonly byte startMask;
		readonly int startShift;
		public PdfIndexedColorSpaceOneTwoBitTransformation(PdfIndexedColorSpace colorSpace, int width, int height, IList<PdfRange> colorKeyMask, int bitsPerComponent) 
				: base(colorSpace, width, height, colorKeyMask) {
			this.bitsPerComponent = bitsPerComponent;
			startMask = bitsPerComponent == 1 ? (byte)0x80 : (byte)0xC0;
			startShift = 8 - bitsPerComponent;
		}
		protected override void Transform(byte[] data) {
			int width = Width;
			int height = Height;
			int maxIndex = MaxIndex;
			int componentsCount = ComponentsCount;
			byte[] lookupTable = LookupTable;
			byte[] result = Result;
			PdfRange transparentRange = TransparentRange;
			byte[] maskData = MaskData;
			for (int y = 0, src = 0, index = 0, maskIndex = 0; y < height; y++) {
				byte b = 0;
				byte mask = startMask;
				int shift = startShift;
				for (int x = 0; x < width; x++, maskIndex++) {
					if (shift == startShift)
						b = data[src++];
					byte value = (byte)((b & mask) >> shift);
					if (value <= maxIndex) {
						int position = value * componentsCount;
						for (int j = 0; j < componentsCount; j++)
							result[index++] = lookupTable[position++];
					}
					else 
						index += componentsCount;
					mask >>= bitsPerComponent;
					shift -= bitsPerComponent;
					if (shift < 0) {
						mask = startMask;
						shift = startShift;
					}
					if (transparentRange != null)
						maskData[maskIndex] = (value >= transparentRange.Min && value <= transparentRange.Max) ? (byte)0 : (byte)255;
				}
			}   
		}
	}
}
