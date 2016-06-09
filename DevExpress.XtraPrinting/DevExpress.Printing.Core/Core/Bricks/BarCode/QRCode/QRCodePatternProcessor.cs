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
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
namespace DevExpress.XtraPrinting.BarCode.Native {
#if !WINRT && !WP
	[CLSCompliant(false)]
#endif
	public abstract class QRCodePatternProcessor : IPatternProcessor {
		public static QRCodePatternProcessor CreateInstance(QRCodeCompactionMode mode) {
			if(mode == QRCodeCompactionMode.AlphaNumeric)
				return new QRCodeAlphaNumericPatternProcessor();
			else if(mode == QRCodeCompactionMode.Numeric)
				return new QRCodeNumericPatternProcessor();
			else return new QRCodeBytePatternProcessor();
		}
		List<List<bool>> pattern = new List<List<bool>>();
		QRCodeErrorCorrectionLevel errorCorrectionLevel = QRCodeErrorCorrectionLevel.L;
		QRCodeVersion version = QRCodeVersion.AutoVersion;
		byte[] data = null;
		int[] codeWordNumPlus;
		protected byte[] Data {
			get { return data; }
			set { data = value; }
		}
		[DefaultValue(QRCodeErrorCorrectionLevel.L)]
		public QRCodeErrorCorrectionLevel ErrorCorrectionLevel {
			get { return errorCorrectionLevel; }
			set {
				errorCorrectionLevel = value;
			}
		}
		[DefaultValue(QRCodeVersion.AutoVersion)]
		public QRCodeVersion Version {
			get { return version; }
			set { version = value; }
		}
		void CalculatePattern() {
			int dataCounter = 0;
			int[] dataValue = new int[Data.Length + 32];
			sbyte[] dataBits = new sbyte[Data.Length + 32];
			CreateEmptyPattern();
			dataBits[dataCounter] = 4;
			codeWordNumPlus = GetCodeWordNumPlus();
			int codeWordNumCounterValue = GetCodeWordCount(dataValue, ref dataCounter, Data.Length, dataBits);
			int totalDataBits = QRCodeHelper.GetTotalDataBits(dataCounter, dataBits);
			int errorCorrectionLevel = (int)ErrorCorrectionLevel;
			int maxDataBits = 0;
			int sizeVersion = GetValidSizeVersion(totalDataBits, errorCorrectionLevel, ref maxDataBits);
			QRCodeHelper qrCodeHelper = new QRCodeHelper(errorCorrectionLevel, sizeVersion);
			totalDataBits += codeWordNumPlus[sizeVersion];
			dataBits[codeWordNumCounterValue] = (sbyte)(dataBits[codeWordNumCounterValue] + codeWordNumPlus[sizeVersion]);
			int sideModules = 4 * sizeVersion + 17;
			int matrixTotalBits = sideModules * sideModules;
			sbyte[] frameData = new sbyte[matrixTotalBits + sideModules];
			QRCodeHelper.ValidateDataBits(dataCounter, dataValue, dataBits, totalDataBits, maxDataBits);
			sbyte[][] matrixContent = qrCodeHelper.CreateMatrixContent(dataValue, dataBits, maxDataBits, sideModules, frameData);
			sbyte maskNumber = qrCodeHelper.SelectMask(matrixContent);
			sbyte maskContent = (sbyte)(1 << maskNumber);
			qrCodeHelper.MaskApply(matrixContent, maskNumber);
			FillPattern(sideModules, frameData, matrixContent, maskContent);
		}
		void CreateEmptyPattern() {
			if(Data.Length <= 0) {
				pattern = new List<List<bool>>();
				pattern.Add(new List<bool>());
				pattern[0].Add(false);
			}
		}
		int GetValidSizeVersion(int totalDataBits, int errorCorrectionLevel, ref int maxDataBits) {
			int sizeVersion = (int)Version;
			int autoSizeVersion = GetAutoSizeVersion(totalDataBits, errorCorrectionLevel, ref maxDataBits);
			if(sizeVersion < autoSizeVersion) {
				sizeVersion = 0;
				foreach(QRCodeVersion version in Enum.GetValues(typeof(QRCodeVersion)))
					if((int)version == autoSizeVersion)
						Version = version;
			}
			if(sizeVersion == 0)
				sizeVersion = autoSizeVersion;
			else
				maxDataBits = QRCodeConstants.MaxDataBitValues[errorCorrectionLevel][sizeVersion];
			return sizeVersion;
		}
		int GetAutoSizeVersion(int totalDataBits, int ec, ref int maxDataBits) {
			int sizeVersion = 1;
			for(int i = 1; i <= 40; i++) {
				if((QRCodeConstants.MaxDataBitValues[ec][i]) >= totalDataBits + codeWordNumPlus[sizeVersion]) {
					maxDataBits = QRCodeConstants.MaxDataBitValues[ec][i];
					break;
				}
				sizeVersion++;
			}
			return sizeVersion;
		}
		void FillPattern(int sideModules, sbyte[] frameData, sbyte[][] matrixContent, sbyte maskContent) {
			pattern.Clear();
			int quietSideModules = sideModules + 2;
			for (int i3 = 0; i3 < quietSideModules; i3++)
				pattern.Add(new List<bool>(quietSideModules));
			int frameDataCounter = 0;
			for (int i = 0; i < quietSideModules; i++) {
				for (int j = 0; j < quietSideModules; j++) {
					if (i < 1 || i > quietSideModules - 2 || j < 1 || j > quietSideModules - 2) {
						pattern[j].Add(false);
						continue;
					}
					pattern[j].Add((matrixContent[i - 1][j - 1] & maskContent) != 0 || frameData[frameDataCounter] == (char)49);
					frameDataCounter++;
				}
				if (i >= 1 && i < quietSideModules - 1)
					frameDataCounter++;
			}
		}
		protected abstract void SetData(object data);
		protected abstract int[] GetCodeWordNumPlus();
		protected abstract int GetCodeWordCount(int[] dataValue, ref int dataCounter, int dataLength, sbyte[] dataBits);
		public abstract bool IsValidData(object data);
		public abstract string GetValidCharset();
		#region IPatternProcessor Members
		ArrayList IPatternProcessor.Pattern { get { return new ArrayList(pattern); } }
		void IPatternProcessor.RefreshPattern(object data) {
			if(IsValidData(data)) {
				SetData(data);
				CalculatePattern();
			}
		}
		void IPatternProcessor.Assign(IPatternProcessor source) {
			QRCodePatternProcessor sourceProcessor = source as QRCodePatternProcessor;
			this.errorCorrectionLevel = sourceProcessor.ErrorCorrectionLevel;
			this.version = sourceProcessor.Version;
		}
		#endregion
	}
#if !WINRT && !WP
	[CLSCompliant(false)]
#endif
	public class QRCodeNumericPatternProcessor : QRCodePatternProcessor {
		protected override void SetData(object data) {
			string value = data as string;
			byte[] ascii = GetAsciiBytes(value);
			byte[] unicode = Encoding.Unicode.GetBytes(value);
			string asciiString = GetAsciiString(ascii);
			string unicodeString = Encoding.Unicode.GetString(unicode, 0, unicode.Length);
			if(asciiString != unicodeString)
				Data = unicode;
			else
				Data = ascii;
		}
		static string GetAsciiString(byte[] ascii) {
#if !WP
			return Encoding.GetEncoding("ASCII").GetString(ascii, 0, ascii.Length);
#else
			return new string(ascii.Select(x => (char)x).ToArray());
#endif
		}
		static byte[] GetAsciiBytes(string value) {
#if !WP
			return Encoding.GetEncoding("ASCII").GetBytes(value);
#else
			byte[] result = new byte[value.Length];
			for(int i = 0; i < value.Length; ++i) {
				char c = value[i];
				if(c <= 0x7f)
					result[i] = (byte)c;
				else
					result[i] = (byte)'?';
			}
			return result;
#endif
		}
		protected override int[] GetCodeWordNumPlus() {
			return QRCodeConstants.NumericCodeWordNumPlus;
		}
		protected override int GetCodeWordCount(int[] dataValue, ref int dataCounter, int dataLength, sbyte[] dataBits) {
			dataValue[dataCounter] = 1;
			dataCounter++;
			dataValue[dataCounter] = dataLength;
			dataBits[dataCounter] = 10;
			int codewordNumCounterValue = dataCounter;
			dataCounter++;
			for(int i = 0; i < dataLength; i++) {
				if((i % 3) == 0) {
					dataValue[dataCounter] = (int)(Data[i] - 0x30);
					dataBits[dataCounter] = 4;
				}
				else {
					dataValue[dataCounter] = dataValue[dataCounter] * 10 + (int)(Data[i] - 0x30);
					if((i % 3) == 1) {
						dataBits[dataCounter] = 7;
					}
					else {
						dataBits[dataCounter] = 10;
						if(i < dataLength - 1) {
							dataCounter++;
						}
					}
				}
			}
			dataCounter++;
			return codewordNumCounterValue;
		}
		public override bool IsValidData(object data) {
			return data is string && !string.IsNullOrEmpty(data as string);
		}
		public override string GetValidCharset() {
			return "0123456789";
		}
	}
#if !WINRT && !WP
	[CLSCompliant(false)]
#endif
	public class QRCodeAlphaNumericPatternProcessor : QRCodeNumericPatternProcessor {
		protected override int GetCodeWordCount(int[] dataValue, ref int dataCounter, int dataLength, sbyte[] dataBits) {
			dataValue[dataCounter] = 2;
			dataCounter++;
			dataValue[dataCounter] = dataLength;
			dataBits[dataCounter] = 9;
			int codewordNumCounterValue = dataCounter;
			dataCounter++;
			for(int i = 0; i < dataLength; i++) {
				char chr = (char)Data[i];
				sbyte chrValue = 0;
				if(chr >= 48 && chr < 58) {
					chrValue = (sbyte)(chr - 48);
				}
				else {
					if(chr >= 65 && chr < 91)
						chrValue = (sbyte)(chr - 55);
					else {
						if(chr == 32)
							chrValue = 36;
						if(chr == 36)
							chrValue = 37;
						if(chr == 37)
							chrValue = 38;
						if(chr == 42)
							chrValue = 39;
						if(chr == 43)
							chrValue = 40;
						if(chr == 45)
							chrValue = 41;
						if(chr == 46)
							chrValue = 42;
						if(chr == 47)
							chrValue = 43;
						if(chr == 58)
							chrValue = 44;
					}
				}
				if((i % 2) == 0) {
					dataValue[dataCounter] = chrValue;
					dataBits[dataCounter] = 6;
				}
				else {
					dataValue[dataCounter] = dataValue[dataCounter] * 45 + chrValue;
					dataBits[dataCounter] = 11;
					if(i < dataLength - 1) {
						dataCounter++;
					}
				}
			}
			dataCounter++;
			return codewordNumCounterValue;
		}
		public override string GetValidCharset() {
			return "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:)";
		}
	}
#if !WINRT && !WP
	[CLSCompliant(false)]
#endif
	public class QRCodeBytePatternProcessor : QRCodePatternProcessor {
		protected override void SetData(object data) {
			if(data is byte[])
				Data = data as byte[];
			else
				Data = Encoding.UTF8.GetBytes(data as string);
		}
		protected override int[] GetCodeWordNumPlus() {
			return QRCodeConstants.ByteCodeWordNumPlus;
		}
		protected override int GetCodeWordCount(int[] dataValue, ref int dataCounter, int dataLength, sbyte[] dataBits) {
			dataValue[dataCounter] = 4;
			dataCounter++;
			dataValue[dataCounter] = dataLength;
			dataBits[dataCounter] = 8;
			int codewordNumCounterValue = dataCounter;
			dataCounter++;
			for(int i = 0; i < dataLength; i++) {
				dataValue[i + dataCounter] = (Data[i] & 0xFF);
				dataBits[i + dataCounter] = 8;
			}
			dataCounter += dataLength;
			return codewordNumCounterValue;
		}
		public override bool IsValidData(object data) {
			return true;
		}
		public override string GetValidCharset() {
			return string.Empty;
		}
	}
}
