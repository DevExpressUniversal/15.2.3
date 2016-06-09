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
using System.Collections.ObjectModel;
using DevExpress.Utils.StructuredStorage.Reader;
using DevExpress.Utils.StructuredStorage.Internal;
using DevExpress.Utils.StructuredStorage.Internal.Reader;
namespace DevExpress.Utils.StructuredStorage.Reader {
	#region DirectoryEntry
	[CLSCompliant(false)]
	public class DirectoryEntry : AbstractDirectoryEntry {
		readonly InputHandler fileHandler;
		readonly Header header;
		internal DirectoryEntry(Header header, InputHandler fileHandler, UInt32 sid, string path)
			: base(sid) {
			Guard.ArgumentNotNull(header, "header");
			Guard.ArgumentNotNull(fileHandler, "fileHandler");
			this.header = header;
			this.fileHandler = fileHandler;
			ReadDirectoryEntry();
			InnerPath = path;
		}
		void ReadDirectoryEntry() {
			string entryName = fileHandler.ReadString(64);
			if (entryName.Length >= 32) {
				Name = string.Empty;
				return;
			}
			Name = entryName;
			fileHandler.ReadUInt16(); 
			Type = (DirectoryEntryType)fileHandler.ReadByte();
			Color = (DirectoryEntryColor)fileHandler.ReadByte();
			LeftSiblingSid = fileHandler.ReadUInt32();
			RightSiblingSid = fileHandler.ReadUInt32();
			ChildSiblingSid = fileHandler.ReadUInt32();
			byte[] array = new byte[16];
			fileHandler.Read(array);
			ClsId = new Guid(array);
			UserFlags = fileHandler.ReadUInt32();
			fileHandler.ReadUInt64(); 
			fileHandler.ReadUInt64(); 
			StartSector = fileHandler.ReadUInt32();
			UInt32 sizeLow = fileHandler.ReadUInt32();
			UInt32 sizeHigh = fileHandler.ReadUInt32();
			if (header.SectorSize == 512 && sizeHigh != 0x0) {
				sizeHigh = 0x0;
			}
			StreamLength = ((UInt64)sizeHigh << 32) + sizeLow;
		}
	}
	#endregion
}
namespace DevExpress.Utils.StructuredStorage.Internal.Reader {
	#region DirectoryTree
	[CLSCompliant(false)]
	public class DirectoryTree {
		#region Fields
		readonly Fat fat;
		readonly Header header;
		readonly InputHandler fileHandler;
		readonly List<DirectoryEntry> directoryEntries = new List<DirectoryEntry>();
		List<UInt32> sectorsUsedByDirectory;
		readonly Dictionary<string, DirectoryEntry> entryCacheByName = new Dictionary<string, DirectoryEntry>();
		readonly Dictionary<string, DirectoryEntry> entryCacheByPath = new Dictionary<string, DirectoryEntry>();
		readonly Dictionary<UInt32, DirectoryEntry> entryCacheBySid = new Dictionary<UInt32, DirectoryEntry>();
		#endregion
		public DirectoryTree(Fat fat, Header header, InputHandler fileHandler) {
			this.fat = fat;
			this.header = header;
			this.fileHandler = fileHandler;
			Init(header.DirectoryStartSector);
		}
		void Init(UInt32 startSector) {
			if (header.NoSectorsInDirectoryChain4KB > 0)
				sectorsUsedByDirectory = fat.GetSectorChain(startSector, header.NoSectorsInDirectoryChain4KB, "Directory");
			else
				sectorsUsedByDirectory = fat.GetSectorChain(startSector, (UInt64)Math.Ceiling((double)fileHandler.IOStreamSize / header.SectorSize), "Directory", true);
			GetAllDirectoryEntriesRecursive(0, "");
		}
		void GetAllDirectoryEntriesRecursive(UInt32 sid, string path) {
			DirectoryEntry entry = ReadDirectoryEntry(sid, path);
			if (string.IsNullOrEmpty(entry.Name))
				return;
			if (GetDirectoryEntry(entry.Sid) != null)
				return;
			directoryEntries.Add(entry);
			if (!entryCacheByName.ContainsKey(entry.Name))
				entryCacheByName[entry.Name] = entry;
			if (!entryCacheByPath.ContainsKey(entry.Path))
				entryCacheByPath[entry.Path] = entry;
			if (!entryCacheBySid.ContainsKey(entry.Sid))
				entryCacheBySid[entry.Sid] = entry;
			if (entry.LeftSiblingSid != SectorType.NoStream)
				GetAllDirectoryEntriesRecursive(entry.LeftSiblingSid, path);
			if (entry.RightSiblingSid != SectorType.NoStream)
				GetAllDirectoryEntriesRecursive(entry.RightSiblingSid, path);
			if (entry.ChildSiblingSid != SectorType.NoStream)
				GetAllDirectoryEntriesRecursive(entry.ChildSiblingSid, path + ((sid == 0) ? String.Empty : entry.Name) + '\\');
		}
		DirectoryEntry ReadDirectoryEntry(UInt32 sid, string path) {
			SeekToDirectoryEntry(sid);
			DirectoryEntry result = new DirectoryEntry(header, fileHandler, sid, path);
			return result;
		}
		private void SeekToDirectoryEntry(UInt32 sid) {
			int sectorInDirectoryChain = (int)(sid * Measures.DirectoryEntrySize) / header.SectorSize;
			if (sectorInDirectoryChain < 0)
				throw new ArgumentOutOfRangeException();
			fileHandler.SeekToPositionInSector(sectorsUsedByDirectory[sectorInDirectoryChain], (sid * Measures.DirectoryEntrySize) % header.SectorSize);
		}
		internal DirectoryEntry GetDirectoryEntry(string path) {
			if (path.Length < 1)
				return null;
			DirectoryEntry entry;
			if (path[0] == '\\') {
				if (entryCacheByPath.TryGetValue(path, out entry))
					return entry;
				else
					return null;
			}
			if (entryCacheByName.TryGetValue(path, out entry))
				return entry;
			else
				return null;
		}
		internal DirectoryEntry GetDirectoryEntry(UInt32 sid) {
			DirectoryEntry entry;
			if (entryCacheBySid.TryGetValue(sid, out entry))
				return entry;
			else
				return null;
		}
		internal UInt32 GetMiniStreamStart() {
			DirectoryEntry root = GetDirectoryEntry(0);
			if (root == null)
				ThrowStreamNotFoundException("Root Entry");
			UInt32 result = root.StartSector;
			return (result == SectorType.NoStream) ? SectorType.EndOfChain : result;
		}
		internal UInt64 GetSizeOfMiniStream() {
			DirectoryEntry root = GetDirectoryEntry(0);
			if (root == null)
				ThrowStreamNotFoundException("Root Entry");
			return root.StreamLength;
		}
		internal void ThrowStreamNotFoundException(string name) {
			throw new Exception("Stream with name '" + name + "' not found.");
		}
		internal ReadOnlyCollection<string> GetPathsOfAllEntries() {
			List<string> result = new List<string>();
			foreach (DirectoryEntry entry in directoryEntries)
				result.Add(entry.Path);
			return new ReadOnlyCollection<string>(result);
		}
		internal ReadOnlyCollection<string> GetPathsOfAllStreamEntries() {
			List<string> result = new List<string>();
			foreach (DirectoryEntry entry in directoryEntries)
				if (entry.Type == DirectoryEntryType.Stream)
					result.Add(entry.Path);
			return new ReadOnlyCollection<string>(result);
		}
		internal ReadOnlyCollection<DirectoryEntry> GetAllEntries() {
			return new ReadOnlyCollection<DirectoryEntry>(directoryEntries);
		}
		internal ReadOnlyCollection<DirectoryEntry> GetAllStreamEntries() {
			List<DirectoryEntry> entries = new List<DirectoryEntry>();
			int count = directoryEntries.Count;
			for (int i = 0; i < count; i++)
				if (directoryEntries[i].Type == DirectoryEntryType.Stream)
					entries.Add(directoryEntries[i]);
			return new ReadOnlyCollection<DirectoryEntry>(entries);
		}
	}
	#endregion
}
