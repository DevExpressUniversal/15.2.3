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
using DevExpress.Utils.StructuredStorage.Internal;
using DevExpress.Utils.StructuredStorage.Internal.Reader;
namespace DevExpress.Utils.StructuredStorage.Reader {
	#region StructuredStorageReader
	[CLSCompliant(false)]
	public class StructuredStorageReader {
		#region Fields
		readonly InputHandler fileHandler;
		readonly Header header;
		readonly Fat fat;
		readonly MiniFat miniFat;
		readonly DirectoryTree directory;
		#endregion
		public StructuredStorageReader(Stream stream) 
			: this(stream, false) {
		}
		public StructuredStorageReader(Stream stream, bool keepOpen) {
			Guard.ArgumentNotNull(stream, "stream");
			this.fileHandler = keepOpen ? new KeepOpenInputHandler(stream) : new InputHandler(stream);
			this.header = new Header(fileHandler);
			this.fat = new Fat(header, fileHandler);
			this.directory = new DirectoryTree(fat, header, fileHandler);
			this.miniFat = new MiniFat(fat, header, fileHandler, directory.GetMiniStreamStart(), directory.GetSizeOfMiniStream());
		}
		#region Properties
		public ICollection<string> FullNameOfAllEntries { get { return directory.GetPathsOfAllEntries(); } }
		public ICollection<string> FullNameOfAllStreamEntries { get { return directory.GetPathsOfAllStreamEntries(); } }
		public ICollection<DirectoryEntry> AllEntries { get { return directory.GetAllEntries(); } }
		public ICollection<DirectoryEntry> AllStreamEntries { get { return directory.GetAllStreamEntries(); } }
		public DirectoryEntry RootDirectoryEntry { get { return directory.GetDirectoryEntry(0x0); } }
		#endregion
		public VirtualStream GetStream(string path) {
			DirectoryEntry entry = directory.GetDirectoryEntry(path);
			if (entry == null)
				directory.ThrowStreamNotFoundException(path);
			if (entry.Type != DirectoryEntryType.Stream)
				throw new Exception("The directory entry is not of type STGTY_STREAM.");
			if (entry.StreamLength > long.MaxValue)
				header.ThrowUnsupportedSizeException(entry.StreamLength.ToString());
			if (entry.StreamLength < header.MiniSectorCutoff)
				return new VirtualStream(miniFat, entry.StartSector, (long)entry.StreamLength, path);
			else
				return new VirtualStream(fat, entry.StartSector, (long)entry.StreamLength, path);
		}
		public DirectoryEntry GetEntry(string path) {
			DirectoryEntry entry = directory.GetDirectoryEntry(path);
			if (entry == null)
				throw new Exception("DirectoryEntry with name '" + path + "' not found.");
			return entry;
		}
		public void Close() {
			fileHandler.CloseStream();
		}
		public void Dispose() {
			this.Close();
		}
	}
	#endregion
}
