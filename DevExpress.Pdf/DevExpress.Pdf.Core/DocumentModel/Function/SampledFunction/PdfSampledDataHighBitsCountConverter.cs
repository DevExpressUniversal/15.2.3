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
	public class PdfSampledDataHighBitsCountConverter : PdfSampledDataConverter {
		readonly int multiplier;
		public PdfSampledDataHighBitsCountConverter(int bitsPerSample, int samplesCount) : base(bitsPerSample, samplesCount) {
			multiplier = bitsPerSample / 8;
		}
		protected override long[] Convert(byte[] data) {
			int samplesCount = SamplesCount;
			if (data.Length < samplesCount * multiplier) 
				PdfDocumentReader.ThrowIncorrectDataException();
			long[] result = new long[samplesCount];
			for (int i = 0, index = 0; i < samplesCount; i++) {
				long sample = data[index++];
				for (int j = 1; j < multiplier; j++)
					sample = (sample << 8) + data[index++];
				result[i] = sample;
			}
			return result;
		}
		protected override byte[] ConvertBack(long[] data) {
			int samplesCount = SamplesCount;
			int startIndex = multiplier - 1;
			byte[] result = new byte[samplesCount * multiplier];
			byte[] temp = new byte[multiplier];
			for (int i = 0, pos = 0; i < samplesCount; i++) {
				long sample = data[i];
				for (int j = 0; j < multiplier; j++) {
					temp[j] = (byte)(sample & 0xff);
					sample >>= 8;
				}
				for (int j = startIndex; j >= 0; j--) 
					result[pos++] = temp[j];
			}
			return result;
		}
	}
}
