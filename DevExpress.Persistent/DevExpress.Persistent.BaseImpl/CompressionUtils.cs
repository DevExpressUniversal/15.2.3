#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections;
using System.Text;
using System.IO;
using System.IO.Compression;
using DevExpress.Xpo.Metadata;
using System.ComponentModel;
namespace DevExpress.Persistent.Base {
	public class CompressionUtils {
		private const int RatesArraySize = 256;
		private const float RangeWidth = 0.1F;
		private static Guid Version2Prefix = new Guid("DA088B12-6641-413b-BBFC-2829752DCF96");
		private const string Version2XafCompressedYesString = "+";
		private const string Version2XafCompressedNoString = "-";
		private const int MinAlwaysCompressedLenght = 1000000;
		private static bool IsGoodCompressionForecast(MemoryStream data) {
			if(data != null) {
				int[] rates = new int[RatesArraySize];
				int inRange = 0;
				int usedBytes = 0;
				for(int i = 0; i < RatesArraySize; i++) {
					rates[i] = 0;
				}
				while(data.Position != data.Length) {
					rates[data.ReadByte()]++;
				}
				for(int i = 0; i < RatesArraySize; i++) {
					if(rates[i] > 0) {
						usedBytes++;
					}
				}
				int lowBoundary = (int)((data.Length / usedBytes) * (1 - RangeWidth / 2));
				int highBoundary = (int)((data.Length / usedBytes) * (1 + RangeWidth / 2));
				for(int i = 0; i < RatesArraySize; i++) {
					if(rates[i] > lowBoundary && rates[i] < highBoundary) {
						inRange++;
					}
				}
				if(inRange < (int)(usedBytes * RangeWidth)) {
					return true;
				}
			}
			return false;
		}
		private static void WriteHeader(bool isCompressed, MemoryStream result) {
			byte[] versionPrefix = Version2Prefix.ToByteArray();
			string headerString = isCompressed ? Version2XafCompressedYesString : Version2XafCompressedNoString;
			byte[] header = System.Text.Encoding.UTF8.GetBytes(headerString.ToCharArray());
			result.Write(versionPrefix, 0, versionPrefix.Length);
			result.Write(header, 0, header.Length);
		}
		private static MemoryStream CreateVersion2CompressedStream(MemoryStream compressed, bool isCompressed) {
			MemoryStream result = new MemoryStream();
			WriteHeader(isCompressed, result);
			compressed.WriteTo(result);
			return result;
		}
		private static MemoryStream DecompressData(MemoryStream ms) {
			int BufferSize = 5196;
			MemoryStream result = new MemoryStream();
			using(GZipStream inStream = new GZipStream(ms, CompressionMode.Decompress, true)) {
				byte[] buffer = new byte[BufferSize];
				while(true) {
					int readCount = inStream.Read(buffer, 0, BufferSize);
					if(readCount == 0) {
						break;
					}
					result.Write(buffer, 0, readCount);
				}
			}
			return result;
		}
		private static MemoryStream DecompressVersion2Stream(MemoryStream ms) {
			byte[] header = new byte[System.Text.Encoding.UTF8.GetBytes(Version2XafCompressedYesString.ToCharArray()).Length];
			ms.Read(header, 0, header.Length);
			string headerString = System.Text.Encoding.UTF8.GetString(header, 0, header.Length);
			if(headerString == Version2XafCompressedYesString) {
				return DecompressData(ms);
			}
			if(headerString == Version2XafCompressedNoString) {
				MemoryStream result = new MemoryStream();
				while(ms.Position < ms.Length) {
					result.WriteByte((byte)ms.ReadByte());
				}
				return result;
			}
			throw new ArgumentException();
		}
		public static MemoryStream Compress(MemoryStream data) {
			if(data != null && data.Length > 0) {
				if((data.Length < MinAlwaysCompressedLenght) || IsGoodCompressionForecast(data)) {
					using(MemoryStream compressed = new MemoryStream()) {
						using(GZipStream deflater = new GZipStream(compressed, CompressionMode.Compress, true)) {
							data.WriteTo(deflater);
						}
						if(compressed.Length < data.Length) {
							return CreateVersion2CompressedStream(compressed, true);
						}
					}
				}
				return CreateVersion2CompressedStream(data, false);
			}
			return data;
		}
		public static MemoryStream Decompress(MemoryStream data) {
			if(data != null && data.Length > 0) {
				long startPosition = data.Position;
				byte[] guidPrefix = new byte[16];
				data.Read(guidPrefix, 0, guidPrefix.Length);
				if(new Guid(guidPrefix) == Version2Prefix) {
					return DecompressVersion2Stream(data);
				}
				else {
					data.Position = startPosition;
					return DecompressData(data);
				}
			}
			return data;
		}
		#region Obsolete 9.1
		private const int BufferSize = 5196;
		private class ReadItem {
			public byte[] Buffer = new byte[BufferSize];
			public int Length = 0;
		}
		[Obsolete("Use 'Compress' instead.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static byte[] CompressData(byte[] data) {
			throw new NotImplementedException();
		}
		[Obsolete("Use 'Decompress' instead.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static byte[] DecompressData(byte[] data) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class CompressionConverter : ValueConverter {
		public override object ConvertToStorageType(object value) {
			if(value != null && !(value is byte[])) {
				throw new ArgumentException();
			}
			if(value == null || ((byte[])value).Length == 0) {
				return value;
			}
			return CompressionUtils.Compress(new MemoryStream((byte[])value)).ToArray();
		}
		public override object ConvertFromStorageType(object value) {
			if(value != null && !(value is byte[])) {
				throw new ArgumentException();
			}
			if(value == null || ((byte[])value).Length == 0) {
				return value;
			}
			return CompressionUtils.Decompress(new MemoryStream((byte[])value)).ToArray();
		}
		public override Type StorageType {
			get { return typeof(byte[]); }
		}
	}
}
