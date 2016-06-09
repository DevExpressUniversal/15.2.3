#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System.IO.MemoryMappedFiles;
using System.Security;
namespace DevExpress.DashboardCommon.DataProcessing {
	public abstract class DataWorker {
		string path;
		public string Path {
			get { return path; }
			set { path = value; }
		}
		public abstract void OpenFileForRead();
		public abstract void OpenFileForWrite();
		public abstract void WriteToFile(int offsetInFile, int start, int count, byte[] buff);
		public abstract void ReadFromFile(int offsetInFile, int start, int count, byte[] buff);
		public abstract void CloseFile();
		public abstract byte[] ReadBytes<T>();
		public abstract void WriteData(int data);
		public abstract void WriteData(byte data);
		public abstract void WriteData(char[] data);
		public abstract void SetPositionInFile(int position);
	}
	public class FileWorker : DataWorker {
		FileStream fs;
		BinaryReader br;
		BinaryWriter bw;
		public FileWorker(string path) {
			this.Path = path;
		}
		public override byte[] ReadBytes<T>() {
			return br.ReadBytes(DXMarshal.SizeOf<T>());
		}
		public override void WriteData(int data) {
			bw.Write(data);
		}
		public override void WriteData(byte data) {
			bw.Write(data);
		}
		public override void WriteData(char[] data) {
			bw.Write(data);
		}
		public override void SetPositionInFile(int position) {
			fs.Position = position;
		}
		public override void OpenFileForRead() {
			fs = new FileStream(Path, FileMode.Open);
			br = new BinaryReader(fs);
		}
		public override void OpenFileForWrite() {
			fs = new FileStream(Path, FileMode.OpenOrCreate);
			bw = new BinaryWriter(fs);
		}
		public override void CloseFile() {
			if (br != null) {
				br.Dispose();
				br = null;
			}
			if (bw != null) {
				bw.Dispose();
				bw = null;
			}
			fs.Dispose();
		}
		public override void WriteToFile(int offsetInFile, int start, int count, byte[] buff) {
			using (fs = new FileStream(Path, FileMode.OpenOrCreate)) {
				fs.Position = offsetInFile;
				using (bw = new BinaryWriter(fs))
					bw.Write(buff, start, count);
			}
		}
		public override void ReadFromFile(int offsetInFile, int start, int count, byte[] buff) {
			using (fs = new FileStream(Path, FileMode.OpenOrCreate)) {
				fs.Position = offsetInFile;
				using (br = new BinaryReader(fs))
					br.Read(buff, start, count);
			}
		}
	}
	public class MMFileWorker : DataWorker {
		MemoryMappedFile mmf;
		MemoryMappedViewAccessor accessor;
		int position;
		public MMFileWorker(string path) {
			this.Path = path;
		}
		long GetFileLength() {
			if (File.Exists(Path))
				return new FileInfo(Path).Length;
			return 0;
		}
		public override void OpenFileForRead() {
			mmf = MemoryMappedFile.CreateFromFile(Path, FileMode.Open);
			position = 0;
		}
		public override void OpenFileForWrite() {
			position = 0;
		}
		public override void CloseFile() {
			if (accessor != null)
				accessor.Dispose();
			if (mmf != null)
				mmf.Dispose();
		}
		[System.Security.SecuritySafeCritical]
		public override byte[] ReadBytes<T>() {
			int length = DXMarshal.SizeOf<T>();
			byte[] buff = new byte[length];
			using (accessor = mmf.CreateViewAccessor(position, length)) {
				accessor.ReadArray(0, buff, 0, length);
			}
			position += length;
			return buff;
		}
		[System.Security.SecuritySafeCritical]
		public override void WriteData(int data) {
			CloseFile();
			using (mmf = MemoryMappedFile.CreateFromFile(this.Path, FileMode.OpenOrCreate, null, Math.Max(GetFileLength(), position) + 4)) {
				using (accessor = mmf.CreateViewAccessor(position, 4)) {
					accessor.Write(0, data);
				}
			}
			position += 4;
		}
		[System.Security.SecuritySafeCritical]
		public override void WriteData(byte data) {
			CloseFile();
			using (mmf = MemoryMappedFile.CreateFromFile(this.Path, FileMode.OpenOrCreate, null, Math.Max(GetFileLength(), position) + 1)) {
				using (accessor = mmf.CreateViewAccessor(position, 1)) {
					accessor.Write(0, data);
				}
			}
			position += 1;
		}
		[System.Security.SecuritySafeCritical]
		public override void WriteData(char[] data) {
			int length = data.Length;
			byte[] bytes = new byte[length];
			for (int i = 0; i < length; i++)
				bytes[i] = Convert.ToByte(data[i]);
			CloseFile();
			using (mmf = MemoryMappedFile.CreateFromFile(this.Path, FileMode.OpenOrCreate, null, Math.Max(GetFileLength(), position) + length)) {
				using (accessor = mmf.CreateViewAccessor(position, length)) {
					accessor.WriteArray(0, bytes, 0, length);
				}
			}
			position += length;
		}
		[System.Security.SecuritySafeCritical]
		public override void SetPositionInFile(int position) {
			this.position = position;
		}
		[System.Security.SecuritySafeCritical]
		public override void WriteToFile(int offsetInFile, int start, int count, byte[] buff) {
			CloseFile();
			using (mmf = MemoryMappedFile.CreateFromFile(Path, FileMode.OpenOrCreate, null, Math.Max(GetFileLength(), offsetInFile) + count)) {
				using (accessor = mmf.CreateViewAccessor(offsetInFile, count)) {
					accessor.WriteArray(start, buff, 0, count);
				}
			}
		}
		[System.Security.SecuritySafeCritical]
		public override void ReadFromFile(int offsetInFile, int start, int count, byte[] buff) {
			CloseFile();
			using (mmf = MemoryMappedFile.CreateFromFile(this.Path, FileMode.Open)) {
				using (accessor = mmf.CreateViewAccessor(offsetInFile, count)) {
					accessor.ReadArray(start, buff, 0, count);
				}
			}
		}
	}
	public class NonPersistentMMFileWorker : DataWorker {
		public const int MMFLimit = 1024 * 1024 * 10;
		MemoryMappedFile mmf;
		MemoryMappedViewAccessor accessor;
		int position;
		string mapName = "test";
		public NonPersistentMMFileWorker(MemoryMappedFile file) {
			mmf = file;
		}
		public NonPersistentMMFileWorker() { }
		public override void OpenFileForRead() {
			if (mmf == null) {
				CloseFile();
				mmf = MemoryMappedFile.CreateFromFile(Path, FileMode.Open);
			}
			position = 0;
		}
		public override void OpenFileForWrite() {
			if (mmf == null) {
				mmf = MemoryMappedFile.CreateNew(mapName + Path, MMFLimit);
			}
			position = 0;
		}
		public override void CloseFile() { }
		[System.Security.SecuritySafeCritical]
		public override byte[] ReadBytes<T>() {
			int length = DXMarshal.SizeOf<T>();
			byte[] buff = new byte[length];
			using (accessor = mmf.CreateViewAccessor(position, length)) {
				accessor.ReadArray(0, buff, 0, length);
			}
			position += length;
			return buff;
		}
		public override void WriteData(int data) {
			using (MemoryMappedViewStream stream = mmf.CreateViewStream(position, 4)) {
				stream.Write(BitConverter.GetBytes(data), 0, 4);
			}
			position += 4;
		}
		public override void WriteData(byte data) {
			using (MemoryMappedViewStream stream = mmf.CreateViewStream(position, 1)) {
				stream.WriteByte(data);
			}
			position += 1;
		}
		public override void WriteData(char[] data) {
			int length = data.Length;
			byte[] bytes = new byte[length];
			for (int i = 0; i < length; i++)
				bytes[i] = Convert.ToByte(data[i]);
			using (MemoryMappedViewStream stream = mmf.CreateViewStream(position, length)) {
				stream.Write(bytes, 0, length);
			}
			position += length;
		}
		public override void SetPositionInFile(int position) {
			this.position = position;
		}
		[System.Security.SecuritySafeCritical]
		public override void WriteToFile(int offsetInFile, int start, int count, byte[] buff) {
			using (accessor = mmf.CreateViewAccessor(offsetInFile, count)) {
				accessor.WriteArray(start, buff, 0, count);
			}
		}
		[System.Security.SecuritySafeCritical]
		public override void ReadFromFile(int offsetInFile, int start, int count, byte[] buff) {
			using (accessor = mmf.CreateViewAccessor(offsetInFile, count)) {
				accessor.ReadArray(start, buff, 0, count);
			}
		}
	}
}
