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
using DevExpress.Utils.StructuredStorage.Writer;
using DevExpress.Utils.StructuredStorage.Internal.Writer;
using DevExpress.Office.Utils;
namespace DevExpress.Utils.StructuredStorage.Internal.Writer {
	#region BaseDirectoryEntry (abstract class)
	[CLSCompliant(false)]
	public abstract class BaseDirectoryEntry : AbstractDirectoryEntry {
		readonly StructuredStorageContext context;
		internal BaseDirectoryEntry(string name, StructuredStorageContext context) {
			Guard.ArgumentNotNull(context, "context");
			this.context = context;
			Name = name;
			Init();
		}
		internal StructuredStorageContext Context { get { return context; } }
		void Init() {
			this.ChildSiblingSid = SectorType.Free;
			this.LeftSiblingSid = SectorType.Free;
			this.RightSiblingSid = SectorType.Free;
			this.ClsId = Guid.Empty;
			this.Color = DirectoryEntryColor.Black;
			this.StartSector = 0x0;
			this.ClsId = Guid.Empty;
			this.UserFlags = 0x0;
			this.StreamLength = 0x0;
		}
		internal void Write() {
			OutputHandler directoryStream = context.DirectoryStream;
			char[] unicodeName = InnerName.ToCharArray();
			int paddingCounter = 0;
			foreach (UInt16 unicodeChar in unicodeName) {
				directoryStream.WriteUInt16(unicodeChar);
				paddingCounter++;
			}
			while (paddingCounter < 32) {
				directoryStream.WriteUInt16(0x0);
				paddingCounter++;
			}
			directoryStream.WriteUInt16(this.LengthOfName);
			directoryStream.WriteByte((byte)this.Type);
			directoryStream.WriteByte((byte)this.Color);
			directoryStream.WriteUInt32(this.LeftSiblingSid);
			directoryStream.WriteUInt32(this.RightSiblingSid);
			directoryStream.WriteUInt32(this.ChildSiblingSid);
			directoryStream.Write(this.ClsId.ToByteArray());
			directoryStream.WriteUInt32(this.UserFlags);
			directoryStream.Write(new byte[16]); 
			directoryStream.WriteUInt32(this.StartSector);
			directoryStream.WriteUInt64(this.StreamLength);
		}
		protected internal abstract void WriteReferencedStream();
	}
	#endregion
	#region EmptyDirectoryEntry
	[CLSCompliant(false)]
	public class EmptyDirectoryEntry : BaseDirectoryEntry {
		public EmptyDirectoryEntry(StructuredStorageContext context)
			: base(String.Empty, context) {
			Color = DirectoryEntryColor.Red; 
			Type = DirectoryEntryType.Invalid;
		}
		protected internal override void WriteReferencedStream() {
		}
	}
	#endregion
	#region RootDirectoryEntry
	[CLSCompliant(false)]
	public class RootDirectoryEntry : StorageDirectoryEntry {
		readonly OutputHandler miniStream = new OutputHandler(new ChunkedMemoryStream());
		internal RootDirectoryEntry(StructuredStorageContext context)
			: base("Root Entry", context) {
			Type = DirectoryEntryType.Root;
			Sid = 0x0;
		}
		internal OutputHandler MiniStream { get { return miniStream; } }
		protected internal override void WriteReferencedStream() {
			VirtualStream virtualMiniStream = new VirtualStream(miniStream.BaseStream, Context.Fat, Context.Header.SectorSize, Context.TempOutputStream);
			virtualMiniStream.Write();
			this.StartSector = virtualMiniStream.StartSector;
			this.StreamLength = virtualMiniStream.Length;
		}
	}
	#endregion
	#region StorageDirectoryEntry
	[CLSCompliant(false)]
	public class StorageDirectoryEntry : BaseDirectoryEntry {
		#region Fields
		readonly List<StorageDirectoryEntry> storageDirectoryEntries = new List<StorageDirectoryEntry>();
		readonly List<StreamDirectoryEntry> streamDirectoryEntries = new List<StreamDirectoryEntry>();
		readonly List<BaseDirectoryEntry> allDirectoryEntries = new List<BaseDirectoryEntry>();
		#endregion
		public StorageDirectoryEntry(string name, StructuredStorageContext context)
			: base(name, context) {
			Type = DirectoryEntryType.Storage;
		}
		internal List<StreamDirectoryEntry> StreamDirectoryEntries { get { return streamDirectoryEntries; } }
		internal List<StorageDirectoryEntry> StorageDirectoryEntries { get { return storageDirectoryEntries; } }
		public void AddStreamDirectoryEntry(string name, Stream stream) {
			if (streamDirectoryEntries.Find(delegate(StreamDirectoryEntry a) { return name == a.Name; }) != null)
				return;
			StreamDirectoryEntry newDirEntry = new StreamDirectoryEntry(name, stream, Context);
			streamDirectoryEntries.Add(newDirEntry);
			allDirectoryEntries.Add(newDirEntry);
		}
		public StorageDirectoryEntry AddStorageDirectoryEntry(string name) {
			StorageDirectoryEntry result = null;
			result = storageDirectoryEntries.Find(delegate(StorageDirectoryEntry a) { return name == a.Name; });
			if (result != null) 
				return result;
			result = new StorageDirectoryEntry(name, Context);
			storageDirectoryEntries.Add(result);
			allDirectoryEntries.Add(result);
			return result;
		}
		public void setClsId(Guid clsId) {
			ClsId = clsId;
		}
		internal List<BaseDirectoryEntry> RecursiveGetAllDirectoryEntries() {
			List<BaseDirectoryEntry> result = new List<BaseDirectoryEntry>();
			return RecursiveGetAllDirectoryEntries(result);
		}
		List<BaseDirectoryEntry> RecursiveGetAllDirectoryEntries(List<BaseDirectoryEntry> result) {
			foreach (StorageDirectoryEntry entry in storageDirectoryEntries)
				result.AddRange(entry.RecursiveGetAllDirectoryEntries());
			foreach (StreamDirectoryEntry entry in streamDirectoryEntries)
				result.Add(entry);
			if (!result.Contains(this))
				result.Add(this);
			return result;
		}
		internal void RecursiveCreateRedBlackTrees() {
			this.ChildSiblingSid = CreateRedBlackTree();
			foreach (StorageDirectoryEntry entry in storageDirectoryEntries)
				entry.RecursiveCreateRedBlackTrees();
		}
		UInt32 CreateRedBlackTree() {
			allDirectoryEntries.Sort(DirectoryEntryComparison);
			foreach (BaseDirectoryEntry entry in allDirectoryEntries)
				entry.Sid = Context.GetNewSid();
			return SetRelationsAndColorRecursive(this.allDirectoryEntries, (int)Math.Floor(Math.Log(allDirectoryEntries.Count, 2)), 0);
		}
		UInt32 SetRelationsAndColorRecursive(List<BaseDirectoryEntry> entryList, int treeHeight, int treeLevel) {
			if (entryList.Count < 1)
				return SectorType.Free;
			if (entryList.Count == 1) {
				if (treeLevel == treeHeight)
					entryList[0].Color = DirectoryEntryColor.Red;
				return entryList[0].Sid;
			}
			int middleIndex = GetMiddleIndex(entryList);
			List<BaseDirectoryEntry> leftSubTree = entryList.GetRange(0, middleIndex);
			List<BaseDirectoryEntry> rightSubTree = entryList.GetRange(middleIndex + 1, entryList.Count - middleIndex - 1);
			int leftmiddleIndex = GetMiddleIndex(leftSubTree);
			int rightmiddleIndex = GetMiddleIndex(rightSubTree);
			if (leftSubTree.Count > 0) {
				entryList[middleIndex].LeftSiblingSid = leftSubTree[leftmiddleIndex].Sid;
				SetRelationsAndColorRecursive(leftSubTree, treeHeight, treeLevel + 1);
			}
			if (rightSubTree.Count > 0) {
				entryList[middleIndex].RightSiblingSid = rightSubTree[rightmiddleIndex].Sid;
				SetRelationsAndColorRecursive(rightSubTree, treeHeight, treeLevel + 1);
			}
			return entryList[middleIndex].Sid;
		}
		static int GetMiddleIndex(List<BaseDirectoryEntry> list) {
			return (int)Math.Floor((list.Count - 1) / 2.0);
		}
		protected int DirectoryEntryComparison(BaseDirectoryEntry a, BaseDirectoryEntry b) {
			if (a.Name.Length != b.Name.Length)
				return a.Name.Length.CompareTo(b.Name.Length);
			String aU = a.Name.ToUpper();
			String bU = b.Name.ToUpper();
			for (int i = 0; i < aU.Length; i++) {
				if (aU[i] != bU[i])
					return ((UInt32)aU[i]).CompareTo((UInt32)bU[i]);
			}
			return 0;
		}
		protected internal override void WriteReferencedStream() {
		}
	}
	#endregion
	#region StreamDirectoryEntry
	[CLSCompliant(false)]
	public class StreamDirectoryEntry : BaseDirectoryEntry {
		readonly Stream stream;
		public StreamDirectoryEntry(string name, Stream stream, StructuredStorageContext context)
			: base(name, context) {
			Guard.ArgumentNotNull(stream, "stream");
			this.stream = stream;
			Type = DirectoryEntryType.Stream;
		}
		protected internal override void WriteReferencedStream() {
			VirtualStream virtualStream;
			if (stream.Length < Context.Header.MiniSectorCutoff)
				virtualStream = new VirtualStream(stream, Context.MiniFat, Context.Header.MiniSectorSize, Context.RootDirectoryEntry.MiniStream);
			else
				virtualStream = new VirtualStream(stream, Context.Fat, Context.Header.SectorSize, Context.TempOutputStream);
			virtualStream.Write();
			this.StartSector = virtualStream.StartSector;
			this.StreamLength = virtualStream.Length;
		}
	}
	#endregion
}
