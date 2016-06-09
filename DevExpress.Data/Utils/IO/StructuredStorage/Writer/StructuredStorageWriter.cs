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
using DevExpress.Utils.StructuredStorage.Internal.Writer;
namespace DevExpress.Utils.StructuredStorage.Writer {
	#region StructuredStorageWriter
	[CLSCompliant(false)]
	public class StructuredStorageWriter {
		readonly StructuredStorageContext context;
		public StructuredStorageWriter() {
			this.context = new StructuredStorageContext();
		}
		public StorageDirectoryEntry RootDirectoryEntry { get { return context.RootDirectoryEntry; } }
		public void Write(Stream outputStream) {
			context.RootDirectoryEntry.RecursiveCreateRedBlackTrees();
			List<BaseDirectoryEntry> allEntries = context.RootDirectoryEntry.RecursiveGetAllDirectoryEntries();
			allEntries.Sort(delegate(BaseDirectoryEntry a, BaseDirectoryEntry b) { return a.Sid.CompareTo(b.Sid); });
			foreach (BaseDirectoryEntry entry in allEntries) {
				if (entry.Sid == 0x0) 
					continue;
				entry.WriteReferencedStream();
			}
			context.RootDirectoryEntry.WriteReferencedStream();
			foreach (BaseDirectoryEntry entry in allEntries)
				entry.Write();
			UInt32 dirEntriesPerSector = context.Header.SectorSize / 128u;
			UInt32 numToPad = dirEntriesPerSector - ((UInt32)allEntries.Count % dirEntriesPerSector);
			EmptyDirectoryEntry emptyEntry = new EmptyDirectoryEntry(context);
			for (int i = 0; i < numToPad; i++)
				emptyEntry.Write();
			VirtualStream virtualDirectoryStream = new VirtualStream(context.DirectoryStream.BaseStream, context.Fat, context.Header.SectorSize, context.TempOutputStream);
			virtualDirectoryStream.Write();
			context.Header.DirectoryStartSector = virtualDirectoryStream.StartSector;
			if (context.Header.SectorSize == 0x1000)
				context.Header.NoSectorsInDirectoryChain4KB = virtualDirectoryStream.SectorCount;
			context.MiniFat.Write();
			context.Header.MiniFatStartSector = context.MiniFat.MiniFatStart;
			context.Header.NoSectorsInMiniFatChain = context.MiniFat.NumMiniFatSectors;
			context.Fat.Write();
			context.Header.NoSectorsInDiFatChain = context.Fat.NumDiFatSectors;
			context.Header.NoSectorsInFatChain = context.Fat.NumFatSectors;
			context.Header.DiFatStartSector = context.Fat.DiFatStartSector;
			context.Header.Write();
			context.Header.writeToStream(outputStream);
			context.TempOutputStream.WriteToStream(outputStream);
		}
	}
	#endregion
}
