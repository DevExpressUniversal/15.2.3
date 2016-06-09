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
using System.Security;
using System.Threading;
namespace DevExpress.XtraMap.Native {
	public class DiskCacheMap : MapDisposableObject {
		public const string constMapFileName = "dxcache.map";
		readonly DiskCache cache;
		readonly FileInfo file;
		readonly List<DiskCacheMapRecord> records;
		readonly List<DiskCacheMapRecord> timeline; 
		readonly RecordHashComparer hashComparer;
		readonly RecordTimeComparer timeComparer;
		readonly string newLine;
		readonly Mutex fileAccess;
		long size;
		public DirectoryInfo Root { get { return cache.Root; } }
		public FileInfo File { get { return file; } }
		public List<DiskCacheMapRecord> Records { get { return records; } }
		public List<DiskCacheMapRecord> Timeline { get { return timeline; } }
		public RecordHashComparer HashComparer { get { return hashComparer; } }
		public RecordTimeComparer TimeComparer { get { return timeComparer; } }
		public long Size { get { return size; } }
		[SecuritySafeCritical]
		public DiskCacheMap(DiskCache cache) {
			this.cache = cache;
			this.newLine = Environment.NewLine;
			this.records = new List<DiskCacheMapRecord>();
			this.timeline = new List<DiskCacheMapRecord>();
			this.hashComparer = new RecordHashComparer();
			this.timeComparer = new RecordTimeComparer();
			this.fileAccess = new Mutex(false, "DiskCacheMapFile");
			FileInfo[] files = Root.GetFiles(constMapFileName);
			if(files.Length > 0) {
				this.file = files[0];
				Read();
			}
			else {
				this.file = new FileInfo(String.Format("{0}\\{1}", Root.FullName, constMapFileName));
				using(this.file.Create()) { }
			}
		}
		void Flush() {
			this.fileAccess.WaitOne();
			StringWriter output = new StringWriter() { NewLine = newLine };
			for(int i = 0; i < records.Count; i++)
				records[i].Serialize(output);
			lock(File) {
				System.IO.File.WriteAllText(File.FullName, output.ToString());
				output.Dispose();
			}
			this.fileAccess.ReleaseMutex();
		}
		void Read() {
			this.fileAccess.WaitOne();
			lock(File) {
				using (StreamReader sr = System.IO.File.OpenText(File.FullName)) {
					string line = String.Empty;
					while ((line = sr.ReadLine()) != null) {
						DiskCacheMapRecord record = new DiskCacheMapRecord(line);
						PushRecord(record);
					}
				}
			}
			this.fileAccess.ReleaseMutex();
		}
		void PushRecord(DiskCacheMapRecord record) {
			if (records.Count == 0) {
				records.Add(record);
				timeline.Add(record);
				size += record.Size;
			}
			else {
				int index = records.BinarySearch(record, HashComparer);
				if (index < 0) {
					records.Insert(~index, record);
					InsertInTimeline(record);
					size += record.Size;
				}
			}
		}
		void InsertInTimeline(DiskCacheMapRecord record) {
			int index = timeline.BinarySearch(record, TimeComparer);
			if (index < 0)
				index = ~index;
			timeline.Insert(index, record);
		}
		public void PushRecord(string name, long size) {
			PushRecord(new DiskCacheMapRecord(name, size, DateTime.Now));
			Flush();
		}
		public void RemoveRecord(string name) {
			DiskCacheMapRecord toFind = new DiskCacheMapRecord(name, 0, DateTime.Now);
			int index = records.BinarySearch(toFind, HashComparer);
			if (index >= 0)
				RemoveRecord(records[index]);
		}
		public void RemoveRecord(DiskCacheMapRecord record) {
			records.Remove(record);
			timeline.Remove(record);
			Flush();
			size -= record.Size;
		}
		public void UpdateRecord(DiskCacheMapRecord record) {
			timeline.Remove(record);
			record.Time = DateTime.Now;
			InsertInTimeline(record);
		}
		public DiskCacheMapRecord GetRecord(string name) {
			DiskCacheMapRecord toFind = new DiskCacheMapRecord(name, 0, DateTime.Now);
			int index = records.BinarySearch(toFind, HashComparer);
			if (index >= 0)
				return records[index];
			return null;
		}
		protected override void DisposeOverride() {
			base.DisposeOverride();
			if(cache != null) {
				cache.Dispose();
			}
		}
	}
	public class DiskCacheMapRecord {
		public const Char SeparatorChar = ':';
		readonly string name;
		readonly int hash;
		readonly long size;
		DateTime time;
		public string Name { get { return name; } }
		public int Hash { get { return hash; } }
		public long Size { get { return size; } }
		public DateTime Time { get { return time; } set { time = value; } }
		public DiskCacheMapRecord(string name, long size, DateTime time) {
			this.name = name;
			this.time = time;
			this.hash = name.GetHashCode();
			this.size = size;
		}
		public DiskCacheMapRecord(string fileRecord) {
			string[] parts = fileRecord.Split(new char[] { SeparatorChar });
			this.name = parts[0];
			this.time = DateTime.FromBinary(long.Parse(parts[1]));
			this.size = long.Parse(parts[2]);
			this.hash = name.GetHashCode();
		}
		public void Serialize(StringWriter output) {
			output.Write(name);
			output.Write(SeparatorChar);
			output.Write(time.ToBinary());
			output.Write(SeparatorChar);
			output.Write(size);
			output.Write(output.NewLine);
		}
	}
	public class RecordHashComparer : IComparer<DiskCacheMapRecord> {
		public int Compare(DiskCacheMapRecord x, DiskCacheMapRecord y) {
			int hashResult = x.Hash.CompareTo(y.Hash);
			if (hashResult == 0)
				return x.Name.CompareTo(y.Name);
			return hashResult;
		}
	}
	public class RecordTimeComparer : IComparer<DiskCacheMapRecord> {
		public int Compare(DiskCacheMapRecord x, DiskCacheMapRecord y) {
			return -x.Time.CompareTo(y.Time);
		}
	}
}
