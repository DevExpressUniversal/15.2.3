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
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
namespace DevExpress.Utils.StoredObjects {
	interface IPersistentDictionary : IDisposable {
		bool TryGetValue(long key, out byte[] value);
		void SetValue(long key, byte[] value);
		long AddValue(byte[] value);
		long CurrentIndex { get; }
	}
	abstract class PersistentDictionaryBase : IPersistentDictionary {
		protected readonly uint valueLength;
		const int casheSize = 1000;
		long currentIndex = 0;
		protected System.IO.FileStream stream;
		string path;
		Dictionary<long, byte[]> memDictionary = new Dictionary<long, byte[]>(casheSize);
		long IPersistentDictionary.CurrentIndex { 
			get { return currentIndex; }
		}
		public PersistentDictionaryBase(string path, uint valueLength) {
			stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, Environment.SystemPageSize);
			this.valueLength = valueLength;
			this.path = path;
		}
		public virtual void Dispose() {
			CloseStream();
			DeleteFile();
			memDictionary.Clear();
		}
		void DeleteFile() {
			if(File.Exists(path))
				File.Delete(path);
		}
		void CloseStream() {
			if(stream != null) {
				stream.Close();
				stream = null;
			}
		}
		public virtual bool TryGetValue(long key, out byte[] value) {
			if(memDictionary.TryGetValue(key, out value))
				return true;
			value = new byte[valueLength];
			ReadFromStream(key * valueLength, value);
			return true;
		}
		public void SetValue(long key, byte[] value) {
			if(memDictionary.ContainsKey(key)) {
				memDictionary[key] = value;
				return;
			}
			WriteToStream(key * valueLength, value);
		}
		public long AddValue(byte[] value) {
			if(value.Length > valueLength)
				throw new ArgumentOutOfRangeException();
			memDictionary[currentIndex] = value;
			if(memDictionary.Count == casheSize)
				FlushStream();
			try {
				return currentIndex;
			} finally {
				currentIndex += 1;
			}
		}
		void FlushStream() {
			foreach(KeyValuePair<long, byte[]> pair in memDictionary)
				WriteToStream(pair.Key * valueLength, pair.Value);
			memDictionary.Clear();
			stream.Flush(true);
		}
		protected abstract void WriteToStream(long key, byte[] value);
		protected abstract void ReadFromStream(long pos, byte[] value);
	}
	class FilePersistentDictionary : PersistentDictionaryBase {
		public FilePersistentDictionary(string path, uint valueLength)
			: base(path, valueLength) {
		}
		protected override void WriteToStream(long key, byte[] value) {
			SetStreamPosition(key);
			stream.Write(value, 0, value.Length);
		}
		private void SetStreamPosition(long position) {
			if(stream.Position != position)
				stream.Position = position;
		}
		protected override void ReadFromStream(long pos, byte[] value) {
			SetStreamPosition(pos);
			stream.Read(value, 0, value.Length);
		}
	}
	abstract class MMFDictionaryBase : IPersistentDictionary {
		static Int16 globalIndex;
		static object synch = new object();
		static Int16 GetGlobalIndex() {
			lock(synch) {
				if(globalIndex < Int16.MaxValue)
					return globalIndex++;
				throw new InvalidOperationException();
			}
		}
		const long capacityIncrement = 16 * 1024 * 1024;
		static readonly long accessorCapacity = 16 * Environment.SystemPageSize;
		const long streamCapacity = 1024 * 1024;
		string path;
		long capacity = 0;
		protected long currentIndex = 0;
		long accessorPos = 0;
		long streamPos = 0;
		string mapName;
		MemoryMappedFile mmf;
		MemoryMappedViewAccessor accessor;
		MemoryMappedViewStream stream;
		long IPersistentDictionary.CurrentIndex {
			get { return currentIndex; }
		}
		protected abstract long CurrentPos { get; }
		protected string MapName {
			get {
				if(string.IsNullOrEmpty(mapName))
					mapName = GetType().Name + GetGlobalIndex();
				return mapName;
			}
		}
		protected MMFDictionaryBase(string path) {
			using(FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
				fs.Close();
			this.path = path;
			mmf = CreateMMF();
			stream = CreateStream(0);
		}
		MemoryMappedViewAccessor CreateAccessor(long pos) {
			accessorPos = (pos / Environment.SystemPageSize) * Environment.SystemPageSize;
			if(accessorPos + accessorCapacity > capacity) {
				DisposeStream();
				DisposeMMF();
				mmf = CreateMMF();
			}
			return mmf.CreateViewAccessor(accessorPos, accessorCapacity);
		}
		MemoryMappedViewStream CreateStream(long pos) {
			streamPos = pos;
			return mmf.CreateViewStream(pos, streamCapacity);
		}
		MemoryMappedFile CreateMMF() {
			capacity += capacityIncrement;
			return MemoryMappedFile.CreateFromFile(path, FileMode.Open, MapName, capacity);
		}
		public virtual void Dispose() {
			DisposeAccessor();
			DisposeStream();
			DisposeMMF();
			DeleteFile();
		}
		void DeleteFile() {
			if(File.Exists(path))
				File.Delete(path);
		}
		void DisposeMMF() {
			if(mmf != null) {
				mmf.Dispose();
				mmf = null;
			}
		}
		void DisposeStream() {
			if(stream != null) {
				stream.Dispose();
				stream = null;
			}
		}
		void DisposeAccessor() {
			if(accessor != null) {
				accessor.Dispose();
				accessor = null;
			}
		}
		public abstract bool TryGetValue(long key, out byte[] value);
		[System.Security.SecuritySafeCritical]
		protected void ReadArrayCore(long pos, byte[] value) {
			accessor.ReadArray<byte>(pos - accessorPos, value, 0, value.Length);
		}
		[System.Security.SecuritySafeCritical]
		protected void ReadCore<T>(long pos, out T value) where T : struct {
			accessor.Read<T>(pos - accessorPos, out value);
		}
		protected void EnsureAccessor(long pos, int length) {
			if(accessor == null || pos < accessorPos || pos + length > accessorPos + accessorCapacity) {
				DisposeAccessor();
				accessor = CreateAccessor(pos);
			}
		}
		public abstract void SetValue(long key, byte[] value);
		[System.Security.SecuritySafeCritical]
		protected void WriteArrayCore(long pos, byte[] value) {
			accessor.WriteArray<byte>(pos - accessorPos, value, 0, value.Length);
		}
		public abstract long AddValue(byte[] value);
		protected void AddValueCore(byte[] value) {
			if(CurrentPos + value.Length > capacity) {
				DisposeStream();
				DisposeAccessor();
				DisposeMMF();
				mmf = CreateMMF();
			} else if(CurrentPos + value.Length > streamPos + streamCapacity) {
				DisposeStream();
				if(CurrentPos + streamCapacity > capacity) {
					DisposeAccessor();
					DisposeMMF();
					mmf = CreateMMF();
				}
			}
			if(stream == null)
				stream = CreateStream(CurrentPos);
			stream.Write(value, 0, value.Length);
		}
	}
	class MMFFixedMemberDictionary : MMFDictionaryBase {
		readonly int valueLength;
		protected override long CurrentPos {
			get { return currentIndex * valueLength; }
		}
		public MMFFixedMemberDictionary(string path, int valueLength) : base(path) {
			this.valueLength = valueLength;
		}
		public override bool TryGetValue(long key, out byte[] value) {
			long pos = key * valueLength;
			EnsureAccessor(pos, valueLength);
			value = new byte[valueLength];
			ReadArrayCore(pos, value);
			return true;
		}
		public override void SetValue(long key, byte[] value) {
			long pos = key * valueLength;
			EnsureAccessor(pos, valueLength);
			WriteArrayCore(pos, value);
		}
		public override long AddValue(byte[] value) {
			byte[] writeValue = new byte[valueLength];
			Array.Copy(value, writeValue, value.Length);
			AddValueCore(writeValue);
			try {
				return currentIndex;
			} finally {
				currentIndex += 1;
			}
		}
	}
	class MMFFloatMemberDictionary : MMFDictionaryBase {
		long currentPos = 0;
		protected override long CurrentPos {
			get { return currentPos; }
		}
		public MMFFloatMemberDictionary(string path) : base(path) {
		}
		public override bool TryGetValue(long key, out byte[] value) {
			EnsureAccessor(key, sizeof(Int32));
			Int32 length;
			ReadCore<Int32>(key, out length);
			EnsureAccessor(key + sizeof(Int32), length);
			value = new byte[length];
			ReadArrayCore(key + sizeof(Int32), value);
			return true;
		}
		public override void SetValue(long key, byte[] value) {
			byte[] prevValue;
			if(!TryGetValue(key, out prevValue))
				throw new InvalidOperationException();
			if(prevValue.Length < value.Length)
				throw new InvalidOperationException();
			EnsureAccessor(key, sizeof(Int32));
			WriteArrayCore(key, ToWriteValue(value));
		}
		public override long AddValue(byte[] value) {
			byte[] writeValue = ToWriteValue(value);
			AddValueCore(writeValue);
			try {
				return currentPos;
			} finally {
				currentPos += writeValue.Length;
				currentIndex += 1;
			}
		}
		static byte[] ToWriteValue(byte[] value) {
			byte[] writeValue = new byte[sizeof(Int32) + value.Length];
			byte[] lengthValue = BitConverter.GetBytes(value.Length);
			Array.Copy(lengthValue, writeValue, lengthValue.Length);
			Array.Copy(value, 0, writeValue, lengthValue.Length, value.Length);
			return writeValue;
		}
	}
}
