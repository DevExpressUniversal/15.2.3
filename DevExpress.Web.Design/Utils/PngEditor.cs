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
namespace DevExpress.Web.Design {
	public class RandomString {
		private Random fRandom = null;
		public RandomString() {
			fRandom = new Random();
		}
		public string GetRandomString(int length) {
			List<char> str = new List<char>(length);
			for(int i = 0; i < length; i++) {
				str.Add(fRandom.Next(10).ToString()[0]);
			}
			return new string(str.ToArray());
		}
	}
	[CLSCompliant(false)]
	public class FastCRC {
		static readonly UInt32[] Table = new UInt32[256];
		public FastCRC() {
			InitTable();
		}
		public UInt32 GetDataCRC(byte[] data) {
			UInt32 crc = 0 ^ 0xFFFFFFFF;
			crc = UpdateCRC(crc, data);
			return (UInt32)(crc ^ 0xFFFFFFFF);
		}
		public UInt32 GetDataChunksCRC(List<byte[]> data) {
			UInt32 crc = 0 ^ 0xFFFFFFFF;
			foreach(byte[] datachunk in data) {
				crc = UpdateCRC(crc, datachunk);
			}
			return (UInt32)(crc ^ 0xFFFFFFFF);
		}
		private void InitTable() {
			UInt32 dword;
			int i;
			byte k;
			for(i = 0; i < 256; i++) {
				dword = (UInt32)i;
				for(k = 0; k < 8; k++)
					dword = ((dword & (UInt32)1) != 0) ?
						(UInt32)0xEDB88320L ^ (dword >> 1) : dword >> 1;
				Table[i] = dword;
			}
		}
		private UInt32 UpdateCRC(UInt32 oldCRC, byte[] data) {
			UInt32 crc = oldCRC;
			for(int n = 0; n < data.Length; n++)
				crc = Table[(crc ^ data[n]) & 0xFF] ^ (crc >> 8);
			return crc;
		}
	}
	public class PngChunk {
		private string fType;
		private byte[] fData;
		private UInt32 fCRC;
		private FastCRC fFastCRC;
		private bool fVerifyCRC;
		private bool fRepairInvalidCRC;
		public PngChunk()
			: this(true, true) {
		}
		public PngChunk(bool verifyCRC, bool repairInvalidCRC) {
			fVerifyCRC = verifyCRC;
			fRepairInvalidCRC = repairInvalidCRC;
			if(fVerifyCRC)
				fFastCRC = new FastCRC();
		}
		public string Type {
			get {
				return fType;
			}
			set {
				fType = value;
			}
		}
		public byte[] Data {
			get {
				return fData;
			}
			set {
				fData = value;
			}
		}
		public bool Read(FileStream stream) {
			if(!stream.CanRead)
				return false;
			byte[] datalen, type, crc;
			try {
				datalen = new byte[4];
				stream.Read(datalen, 0, 4);
				type = new byte[4];
				stream.Read(type, 0, 4);
				fData = new byte[(int)ByteArrayToUInt32(datalen)];
				stream.Read(this.fData, 0, fData.Length);
				crc = new byte[4];
				stream.Read(crc, 0, 4);
			} catch {
				return false;
			}
			fType = ByteArrayToString(type);
			fCRC = ByteArrayToUInt32(crc);
			if(fVerifyCRC) {
				List<byte[]> CRCDataArrays = new List<byte[]>();
				CRCDataArrays.Add(type);
				CRCDataArrays.Add(fData);
				UInt32 validCRC = fFastCRC.GetDataChunksCRC(CRCDataArrays);
				if(this.fCRC != validCRC) {
					if(fRepairInvalidCRC)
						fCRC = validCRC;
					else
						return false;
				}
			}
			return true;
		}
		public bool Write(FileStream stream) {
			if(!stream.CanWrite)
				return false;
			try {
				stream.Write(UInt32ToByteArray((UInt32)fData.Length), 0, 4);
				stream.Write(StringToByteArray(fType), 0, 4);
				stream.Write(fData, 0, fData.Length);
				stream.Write(UInt32ToByteArray(fCRC), 0, 4);
			} catch {
				return false;
			}
			return true;
		}
		private string ByteArrayToString(byte[] array) {
			char[] list = new char[array.Length];
			for(int i = 0; i < array.Length; i++) {
				list[i] = (char)array[i];
			}
			return new string(list);
		}
		private UInt32 ByteArrayToUInt32(byte[] array) {
			if(array.Length != 4)
				throw new ArgumentException(
					String.Format("PngChunk: bytes array must contain 4 bytes, but contains {0}.",
						array.Length));
			else {
				return (((UInt32)array[0] << 24) | ((UInt32)array[1] << 16) |
					((UInt32)array[2] << 8) | ((UInt32)array[3]));
			}
		}
		private byte[] UInt32ToByteArray(UInt32 n) {
			byte[] array = new byte[4];
			array[0] = (byte)(n >> 24);
			array[1] = (byte)((n & 0x00FF0000) >> 16);
			array[2] = (byte)((n & 0x0000FF00) >> 8);
			array[3] = (byte)(n & 0x000000FF);
			return array;
		}
		private byte[] StringToByteArray(string s) {
			byte[] array = new byte[s.Length];
			for(int i = 0; i < array.Length; i++)
				array[i] = (byte)s[i];
			return array;
		}
	}
	public class PngEditor {
		static readonly byte[] fSignaturePNG = { 137, 80, 78, 71, 13, 10, 26, 10 };
		static readonly string[] fExcludableChunkTypes = { "gAMA" };
		static readonly string fFinalChunkType = "IEND";
		private List<PngChunk> fChunks;
		private RandomString fRandomString;
		private string fOriginalPath;
		public PngEditor() {
			fRandomString = new RandomString();
			fChunks = new List<PngChunk>();
		}
		public void ExcludeChunks(string path) {
			if(Open(path))
				Save();
		}
		public bool Open(string path) {
			fChunks.Clear();
			fOriginalPath = path;
			using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				byte[] signature = new byte[8];
				stream.Read(signature, 0, 8);
				bool valid = true;
				for(int j = 0; j < 8; j++)
					if(!signature[j].Equals(fSignaturePNG[j]))
						valid = false;
				if(!valid)
					return false;
				string previousType = "";
				string type = "";
				while(previousType != fFinalChunkType) {
					PngChunk chunk = new PngChunk();
					if(!chunk.Read(stream))
						throw new Exception("PngEditor: can't read chunk.");
					fChunks.Add(chunk);
					previousType = type;
					type = chunk.Type;
				}
			}
			return true;
		}
		public void Save() {
			Save(fOriginalPath);
		}
		public void Save(string path) {
			string fileName = Path.GetFileName(path);
			string dirPath = path.Remove(path.LastIndexOf(fileName));
			string tmpFileName = fRandomString.GetRandomString(64);
			string tmpPath = dirPath + tmpFileName;
			using(FileStream stream = new FileStream(tmpPath, FileMode.Create, FileAccess.Write)) {
				stream.Write(fSignaturePNG, 0, fSignaturePNG.Length);
				bool excludeChunk = false;
				foreach(PngChunk chunk in fChunks) {
					excludeChunk = false;
					foreach(string type in fExcludableChunkTypes) {
						if(chunk.Type == type)
							excludeChunk = true;
					}
					if(!excludeChunk) {
						if(!chunk.Write(stream))
							throw new Exception("PngEditor: can't write chunk.");
					}
				}
			}
			if(path == fOriginalPath)
				File.Delete(path);
			File.Move(dirPath + tmpFileName, dirPath + fileName);
		}
	}
}
