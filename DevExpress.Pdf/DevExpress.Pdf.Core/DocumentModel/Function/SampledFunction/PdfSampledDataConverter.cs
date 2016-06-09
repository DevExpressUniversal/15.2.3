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
	public abstract class PdfSampledDataConverter {
		static PdfSampledDataConverter Create(int bitsPerSample, int samplesCount) {
			switch (bitsPerSample) {
				case 1:
				case 2:
				case 4:
					return new PdfSampledDataLowBitsCountConverter(bitsPerSample, samplesCount);
				case 8:
					return new PdfSampledDataSingleByteConverter(bitsPerSample, samplesCount);
				case 16:
				case 24:
				case 32:
					return new PdfSampledDataHighBitsCountConverter(bitsPerSample, samplesCount);
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		public static long[] Convert(int bitsPerSample, int samplesCount, byte[] data) {
			return Create(bitsPerSample, samplesCount).Convert(data);
		}
		public static byte[] ConvertBack(int bitsPerSample, int samplesCount, long[] data) {
			return Create(bitsPerSample, samplesCount).ConvertBack(data);
		}
		readonly int bitsPerSample;
		readonly int samplesCount;
		protected int BitsPerSample { get { return bitsPerSample; } }
		protected int SamplesCount { get { return samplesCount; } }
		protected PdfSampledDataConverter(int bitsPerSample, int samplesCount) {
			this.bitsPerSample = bitsPerSample;
			this.samplesCount = samplesCount;
		}
		protected abstract long[] Convert(byte[] data);
		protected abstract byte[] ConvertBack(long[] data);
	}
}
