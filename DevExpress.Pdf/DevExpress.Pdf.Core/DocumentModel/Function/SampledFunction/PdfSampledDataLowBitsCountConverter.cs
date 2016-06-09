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
	public class PdfSampledDataLowBitsCountConverter : PdfSampledDataConverter {
		readonly int divisor;
		readonly int dataLength;
		readonly int lastElementSize;
		public PdfSampledDataLowBitsCountConverter(int bitsPerSample, int samplesCount) : base(bitsPerSample, samplesCount) {
			divisor = 8 / bitsPerSample;
			dataLength = samplesCount / divisor;
			lastElementSize = samplesCount % divisor;
		}
		protected override long[] Convert(byte[] data) {
			if (data.Length < dataLength + (lastElementSize == 0 ? 0 : 1))
				PdfDocumentReader.ThrowIncorrectDataException();
			int bitsPerSample = BitsPerSample;
			long[] result = new long[SamplesCount];
			byte mask = (byte)((0xff >> bitsPerSample) ^ 0xff);
			int i = 0;
			int index = 0;
			for (; i < dataLength; i++) {
				byte value = data[i];
				byte currentMask = mask;
				for (int j = 0, shift = 8 - bitsPerSample; j < divisor; j++, shift -= bitsPerSample, currentMask >>= bitsPerSample)
					result[index++] = (value & currentMask) >> shift;
			}
			if (lastElementSize > 0) {
				byte value = data[i];
				byte currentMask = mask;
				for (int j = 0, shift = 8 - bitsPerSample; j < lastElementSize; j++, shift -= bitsPerSample)
					result[index++] = (value & mask) >> shift;
			}
			return result;
		}
		protected override byte[] ConvertBack(long[] data) {
			int bitsPerSample = BitsPerSample;
			int samplesCount = SamplesCount;
			byte[] result = new byte[dataLength + (lastElementSize == 0 ? 0 : 1)];
			byte value = 0;
			int component = 0;
			for (int i = 0, pos = 0; i < samplesCount; i++) {
				value = (byte)((value << bitsPerSample) + data[i]);
				if (++component == divisor) {
					result[pos++] = value;
					value = 0;
					component = 0;
				}
			}
			if (lastElementSize > 0) {
				for (; component < divisor; component++)
					value <<= bitsPerSample;
				result[dataLength] = value;
			}
			return result;
		}
	}
}
