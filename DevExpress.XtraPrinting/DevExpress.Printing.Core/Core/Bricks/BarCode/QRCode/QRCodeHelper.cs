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

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
#if !WP
using DevExpress.Utils.Zip;
#endif
namespace DevExpress.XtraPrinting.BarCode.Native {
#if !WINRT && !WP
	[CLSCompliant(false)]
#endif
	public class QRCodeHelper {
		#region static
		public static int GetTotalDataBits(int dataCounter, sbyte[] dataBits) {
			int totalDataBits = 0;
			for(int i = 0; i < dataCounter; i++)
				totalDataBits += dataBits[i];
			return totalDataBits;
		}
		public static void ValidateDataBits(int dataCounter, int[] dataValue, sbyte[] dataBits, int totalDataBits, int maxDataBits) {
			if(totalDataBits <= maxDataBits - 4) {
				dataValue[dataCounter] = 0;
				dataBits[dataCounter] = 4;
			}
			else {
				if(totalDataBits < maxDataBits) {
					dataValue[dataCounter] = 0;
					dataBits[dataCounter] = (sbyte)(maxDataBits - totalDataBits);
				}
				else if(totalDataBits > maxDataBits)
					throw (new OverflowException());
			}
		}
		static sbyte[] XorByteArrays(sbyte[] array1, sbyte[] array2) {
			sbyte[] largeArray;
			sbyte[] smallArray;
			if(array1.Length > array2.Length) {
				largeArray = new sbyte[array1.Length];
				array1.CopyTo(largeArray, 0);
				smallArray = new sbyte[array2.Length];
				array2.CopyTo(smallArray, 0);
			}
			else {
				largeArray = new sbyte[array2.Length];
				array2.CopyTo(largeArray, 0);
				smallArray = new sbyte[array1.Length];
				array1.CopyTo(smallArray, 0);
			}
			sbyte[] result = new sbyte[largeArray.Length];
			for(int i = 0; i < largeArray.Length; i++)
				if(i < smallArray.Length)
					result[i] = (sbyte)(largeArray[i] ^ smallArray[i]);
				else
					result[i] = largeArray[i];
			return result;
		}
		static sbyte[] DivideDataBy8Bits(int[] data, sbyte[] bits, int maxDataCodewords) {
			int codewordsCounter = 0;
			int remainingBits = 8;
			int bitsSum = 0;
			int buffer;
			int bufferBits;
			bool flag;
			for(int i = 0; i < bits.Length; i++)
				bitsSum += bits[i];
			int divadedLength = (bitsSum - 1) / 8 + 1;
			sbyte[] codewords = new sbyte[maxDataCodewords];
			for(int i = 0; i < divadedLength; i++)
				codewords[i] = 0;
			for(int i = 0; i < bits.Length; i++) {
				buffer = data[i];
				bufferBits = bits[i];
				flag = true;
				if(bufferBits == 0)
					break;
				while(flag) {
					if(remainingBits > bufferBits) {
						codewords[codewordsCounter] = (sbyte)((codewords[codewordsCounter] << bufferBits) | buffer);
						remainingBits -= bufferBits;
						flag = false;
					}
					else {
						bufferBits -= remainingBits;
						codewords[codewordsCounter] = (sbyte)((codewords[codewordsCounter] << remainingBits) | (buffer >> bufferBits));
						if(bufferBits == 0)
							flag = false;
						else {
							buffer = (buffer & ((1 << bufferBits) - 1));
							flag = true;
						}
						codewordsCounter++;
						remainingBits = 8;
					}
				}
			}
			if(remainingBits != 8)
				codewords[codewordsCounter] = (sbyte)(codewords[codewordsCounter] << remainingBits);
			else {
				codewordsCounter--;
			}
			if(codewordsCounter < maxDataCodewords - 1) {
				flag = true;
				while(codewordsCounter < maxDataCodewords - 1) {
					codewordsCounter++;
					if(flag)
						codewords[codewordsCounter] = -20;
					else
						codewords[codewordsCounter] = 17;
					flag = !(flag);
				}
			}
			return codewords;
		}
		static int URShift(int number, int bits) {
			if(number >= 0)
				return number >> bits;
			else
				return (number >> bits) + (2 << ~bits);
		}
		static Int32 ReadArray(System.IO.Stream sourceStream, sbyte[] target, int start, int count) {
			if(target.Length == 0)
				return 0;
			byte[] receiver = new byte[target.Length];
			int bytesRead = sourceStream.Read(receiver, start, count);
			if(bytesRead == 0)
				return -1;
			for(int i = start; i < start + bytesRead; i++)
				target[i] = (sbyte)receiver[i];
			return bytesRead;
		}
		#endregion
		int errorCorrectionLevel;
		int sizeVersion;
		public QRCodeHelper(int errorCorrectionLevel, int sizeVersion) {
			this.errorCorrectionLevel = errorCorrectionLevel;
			this.sizeVersion = sizeVersion;
		}
		public sbyte SelectMask(sbyte[][] matrixContent) {
			int maxCodewordsBitWithRemain = QRCodeConstants.MatrixRemainBitValues[sizeVersion] + QRCodeConstants.MaxCodewordsValues[sizeVersion] * 8;
			int l = matrixContent.Length;
			int[] d1 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			int[] d2 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			int[] d3 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			int[] d4 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			int d2And = 0;
			int d2Or = 0;
			int[] d4Counter = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			for(int y = 0; y < l; y++) {
				int[] xData = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
				int[] yData = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
				bool[] xD1Flag = new bool[] { false, false, false, false, false, false, false, false };
				bool[] yD1Flag = new bool[] { false, false, false, false, false, false, false, false };
				for(int x = 0; x < l; x++) {
					if(x > 0 && y > 0) {
						d2And = matrixContent[x][y] & matrixContent[x - 1][y] & matrixContent[x][y - 1] & matrixContent[x - 1][y - 1] & 0xFF;
						d2Or = (matrixContent[x][y] & 0xFF) | (matrixContent[x - 1][y] & 0xFF) | (matrixContent[x][y - 1] & 0xFF) | (matrixContent[x - 1][y - 1] & 0xFF);
					}
					for(int maskNumber = 0; maskNumber < 8; maskNumber++) {
						xData[maskNumber] = ((xData[maskNumber] & 63) << 1) | ((URShift((matrixContent[x][y] & 0xFF), maskNumber)) & 1);
						yData[maskNumber] = ((yData[maskNumber] & 63) << 1) | ((URShift((matrixContent[y][x] & 0xFF), maskNumber)) & 1);
						if((matrixContent[x][y] & (1 << maskNumber)) != 0)
							d4Counter[maskNumber]++;
						if(xData[maskNumber] == 93)
							d3[maskNumber] += 40;
						if(yData[maskNumber] == 93)
							d3[maskNumber] += 40;
						if(x > 0 && y > 0) {
							if(((d2And & 1) != 0) || ((d2Or & 1) == 0))
								d2[maskNumber] += 3;
							d2And = d2And >> 1;
							d2Or = d2Or >> 1;
						}
						if(((xData[maskNumber] & 0x1F) == 0) || ((xData[maskNumber] & 0x1F) == 0x1F)) {
							if(x > 3) {
								if(xD1Flag[maskNumber])
									d1[maskNumber]++;
								else {
									d1[maskNumber] += 3;
									xD1Flag[maskNumber] = true;
								}
							}
						}
						else
							xD1Flag[maskNumber] = false;
						if(((yData[maskNumber] & 0x1F) == 0) || ((yData[maskNumber] & 0x1F) == 0x1F)) {
							if(x > 3) {
								if(yD1Flag[maskNumber])
									d1[maskNumber]++;
								else {
									d1[maskNumber] += 3;
									yD1Flag[maskNumber] = true;
								}
							}
						}
						else
							yD1Flag[maskNumber] = false;
					}
				}
			}
			int minValue = 0;
			sbyte res = 0;
			int[] d4Value = new int[] { 90, 80, 70, 60, 50, 40, 30, 20, 10, 0, 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 90 };
			for(int maskNumber = 0; maskNumber < 8; maskNumber++) {
				d4[maskNumber] = d4Value[(int)((20 * d4Counter[maskNumber]) / maxCodewordsBitWithRemain)];
				int demerit = d1[maskNumber] + d2[maskNumber] + d3[maskNumber] + d4[maskNumber];
				if(demerit < minValue || maskNumber == 0) {
					res = (sbyte)maskNumber;
					minValue = demerit;
				}
			}
			return res;
		}
		public sbyte[][] CreateMatrixContent(int[] dataValue, sbyte[] dataBits, int maxDataBits, int sideModules, sbyte[] frameData) {
			int maxCodeWords = QRCodeConstants.MaxCodewordsValues[sizeVersion];
			int sideLength = QRCodeConstants.MatrixRemainBitValues[sizeVersion] + (maxCodeWords << 3);
			sbyte[] matrixX = new sbyte[sideLength];
			sbyte[] matrixY = new sbyte[sideLength];
			sbyte[] maskArray = new sbyte[sideLength];
			byte[] emptyMatrix = new byte[sideLength*3];
			try {
#if !WINRT && !WP
				Stream sourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Printing.Core.Bricks.BarCode.QRCode.QRMatrixData.dat");
				Stream dataStream = InternalZipArchive.Open(sourceStream)[sizeVersion - 1].FileDataStream;
#else
				Stream sourceStream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("DevExpress.UI.Xaml.Controls.Controls.BarCodeControl.Capability.Bricks.BarCode.QRCode.QRMatrixData.dat");
				var dataStream = new System.IO.Compression.ZipArchive(sourceStream).Entries[sizeVersion - 1].Open();
#endif
				ReadArray(dataStream, frameData, 0, frameData.Length);
				for(int i = 0; i < errorCorrectionLevel; i++)
					dataStream.Read(emptyMatrix, 0, emptyMatrix.Length);
				ReadArray(dataStream, matrixX, 0, matrixX.Length);
				ReadArray(dataStream, matrixY, 0, matrixY.Length);
				ReadArray(dataStream, maskArray, 0, maskArray.Length);
				dataStream.Dispose();
				sourceStream.Dispose();
			}
			catch(Exception e) {
				throw (e);
			}
			sbyte[] rsEccCodewords = CalculateRSECC(dataValue, dataBits, maxCodeWords, maxDataBits);
			sbyte[][] matrixContent = new sbyte[sideModules][];
			for(int i2 = 0; i2 < sideModules; i2++)
				matrixContent[i2] = new sbyte[sideModules];
			for(int i = 0; i < sideModules; i++)
				for(int j = 0; j < sideModules; j++)
					matrixContent[j][i] = 0;
			for(int i = 0; i < maxCodeWords; i++) {
				sbyte codeword_i = (rsEccCodewords != null) ? rsEccCodewords[i] : ((sbyte)0);
				for(int j = 7; j >= 0; j--) {
					int codewordBitsNumber = (i * 8) + j;
					matrixContent[matrixX[codewordBitsNumber] & 0xFF][matrixY[codewordBitsNumber] & 0xFF] = (sbyte)((255 * (codeword_i & 1)) ^ maskArray[codewordBitsNumber]);
					codeword_i = (sbyte)URShift((codeword_i & 0xFF), 1);
				}
			}
			for(int matrixRemain = QRCodeConstants.MatrixRemainBitValues[sizeVersion]; matrixRemain > 0; matrixRemain--) {
				int remainBitTemp = matrixRemain + (maxCodeWords * 8) - 1;
				matrixContent[matrixX[remainBitTemp] & 0xFF][matrixY[remainBitTemp] & 0xFF] = (sbyte)(255 ^ maskArray[remainBitTemp]);
			}
			return matrixContent;
		}
		public void MaskApply(sbyte[][] matrixContent, sbyte maskNumber) {
			sbyte[] formatInformationX2 = QRCodeConstants.FormatInformationX2Values[sizeVersion - 1];
			sbyte[] formatInformationY2 = QRCodeConstants.FormatInformationY2Values[sizeVersion - 1];
			sbyte ec1 = (sbyte)(errorCorrectionLevel << 3);
			sbyte formatInformationValue = (sbyte)(ec1 | maskNumber);
			for(int i = 0; i < 15; i++) {
				sbyte content = QRCodeConstants.FormatInformationValues[formatInformationValue, i];
				matrixContent[QRCodeConstants.FormatInformationX1[i] & 0xFF][QRCodeConstants.FormatInformationY1[i] & 0xFF] = (sbyte)(content * 255);
				matrixContent[formatInformationX2[i] & 0xFF][formatInformationY2[i] & 0xFF] = (sbyte)(content * 255);
			}
		}
		sbyte[] CalculateRSECC(int[] data, sbyte[] bits, int maxCodewords, int maxDataBits) {
			sbyte[] rsBlockOrder = CreateRsBlockOrder();
			int maxDataCodewords = maxDataBits >> 3;
			sbyte rsEccCodewordsCount = QRCodeConstants.RSEccCodewordsValues[sizeVersion - 1, errorCorrectionLevel];
			sbyte[] codeWords = DivideDataBy8Bits(data, bits, maxDataCodewords);
			sbyte[][] rsCalTableArray = new sbyte[256][];
			for(int i = 0; i < 256; i++)
				rsCalTableArray[i] = new sbyte[rsEccCodewordsCount];
			try {
#if !WINRT && !WP
				using(Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Printing.Core.Bricks.BarCode.QRCode.QRRscData.dat")) {
#else
				using(Stream stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream("DevExpress.UI.Xaml.Controls.Controls.BarCodeControl.Capability.Bricks.BarCode.QRCode.QRRscData.dat")) {
#endif
					int offset;
					if(QRCodeConstants.RSEccCodewordsToOffset.TryGetValue(rsEccCodewordsCount, out offset))
						stream.Seek(offset, SeekOrigin.Begin);
					for(int i = 0; i < 256; i++)
						ReadArray(stream, rsCalTableArray[i], 0, rsCalTableArray[i].Length);
				}
			}
			catch(Exception e) {
				throw (e);
			}
			int i2 = 0;
			int j = 0;
			int rsBlockNumber = 0;
			sbyte[][] rsTemp = new sbyte[rsBlockOrder.Length][];
			sbyte[] res = new sbyte[maxCodewords];
			Array.Copy(codeWords, 0, res, 0, codeWords.Length);
			i2 = 0;
			while(i2 < rsBlockOrder.Length) {
				rsTemp[i2] = new sbyte[(rsBlockOrder[i2] & 0xFF) - rsEccCodewordsCount];
				i2++;
			}
			i2 = 0;
			while(i2 < maxDataCodewords) {
				rsTemp[rsBlockNumber][j] = codeWords[i2];
				j++;
				if(j >= (rsBlockOrder[rsBlockNumber] & 0xFF) - rsEccCodewordsCount) {
					j = 0;
					rsBlockNumber++;
				}
				i2++;
			}
			rsBlockNumber = 0;
			while(rsBlockNumber < rsBlockOrder.Length) {
				sbyte[] rsTempData;
				rsTempData = new sbyte[rsTemp[rsBlockNumber].Length];
				rsTemp[rsBlockNumber].CopyTo(rsTempData, 0);
				int rsCodewords = (rsBlockOrder[rsBlockNumber] & 0xFF);
				int rsDataCodewords = rsCodewords - rsEccCodewordsCount;
				j = rsDataCodewords;
				while(j > 0) {
					sbyte first = rsTempData[0];
					if(first != 0) {
						sbyte[] leftChr = new sbyte[rsTempData.Length - 1];
						Array.Copy(rsTempData, 1, leftChr, 0, rsTempData.Length - 1);
						sbyte[] cal = rsCalTableArray[(first & 0xFF)];
						rsTempData = XorByteArrays(leftChr, cal);
					}
					else {
						if(rsEccCodewordsCount < rsTempData.Length) {
							sbyte[] rsTempNew = new sbyte[rsTempData.Length - 1];
							Array.Copy(rsTempData, 1, rsTempNew, 0, rsTempData.Length - 1);
							rsTempData = new sbyte[rsTempNew.Length];
							rsTempNew.CopyTo(rsTempData, 0);
						}
						else {
							sbyte[] rsTempNew = new sbyte[rsEccCodewordsCount];
							Array.Copy(rsTempData, 1, rsTempNew, 0, rsTempData.Length - 1);
							rsTempNew[rsEccCodewordsCount - 1] = 0;
							rsTempData = new sbyte[rsTempNew.Length];
							rsTempNew.CopyTo(rsTempData, 0);
						}
					}
					j--;
				}
				Array.Copy(rsTempData, 0, res, codeWords.Length + rsBlockNumber * rsEccCodewordsCount, (byte)rsEccCodewordsCount);
				rsBlockNumber++;
			}
			return res;
		}
		sbyte[] CreateRsBlockOrder() {
			sbyte[] rsBlockOrderTemp = QRCodeConstants.RSBlockOrderTempValues[sizeVersion - 1, errorCorrectionLevel];
			sbyte rsBlockOrderLength = 1;
			for(sbyte i = 1; i <= 127; i++) {
				if(rsBlockOrderTemp[i] == 0) {
					rsBlockOrderLength = i;
					break;
				}
			}
			sbyte[] rsBlockOrder = new sbyte[rsBlockOrderLength];
			Array.Copy(rsBlockOrderTemp, 0, rsBlockOrder, 0, (byte)rsBlockOrderLength);
			return rsBlockOrder;
		}
	}
}
