#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using System.IO;
using System.Collections.Generic;
using DevExpress.Utils.Zip.Internal;
using System.Diagnostics;
using DevExpress.Utils.Zip;
namespace DevExpress.Compression.Internal {
	interface IZipExtraFieldFactory {
		IZipExtraField Create(int headerId);
	}
	public class ZipExtraFieldFactoryInstance : FactorySingleton<ZipExtraFieldFactory> { }
	public class ZipExtraFieldFactory : InternalZipExtraFieldFactory {
		public override IZipExtraField Create(int headerId) {
			IZipExtraField field = base.Create(headerId);
			if (field != null)
				return field;
			switch (headerId) {
				case ZipExtraFieldNTFS.HeaderId:
					return new ZipExtraFieldNTFS();
				case ZipExtraFieldAes.HeaderId:
					return new ZipExtraFieldAes();
			}
			return null;
		}
	}		
	public class ZipExtraFieldNTFS : ZipExtraField {
		public const int HeaderId = 0xa;
		public override short Id { get { return HeaderId; } }
		public override short ContentSize { get { return 32; } }
		public DateTime LastModificationTime { get; set; }
		public DateTime LastAccessTime { get; set; }
		public DateTime CreationTime { get; set; }
		public override ExtraFieldType Type { get { return ExtraFieldType.CentralDirectoryEntry; } }
		public override void AssignRawData(BinaryReader reader) {
			reader.ReadUInt32();
			while (reader.BaseStream.Position < reader.BaseStream.Length) {
				ushort tag = reader.ReadUInt16();
				ushort size = reader.ReadUInt16();
				if (tag != 1) {
					reader.BaseStream.Seek(size, SeekOrigin.Current);
					break;
				}
				System.Diagnostics.Debug.Assert(size == 8 * 3);
				LastModificationTime = DateTime.FromFileTime(reader.ReadInt64());
				LastAccessTime = DateTime.FromFileTime(reader.ReadInt64());
				CreationTime = DateTime.FromFileTime(reader.ReadInt64());
			}
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((int)0);
			writer.Write((short)1);
			writer.Write((short)24);
			writer.Write(LastModificationTime.ToFileTime());
			writer.Write(LastAccessTime.ToFileTime());
			writer.Write(CreationTime.ToFileTime());
		}
		public override void Apply(InternalZipFile zipFile) {
			InternalZipFileEx zipFileEx = zipFile as InternalZipFileEx;
			if (zipFileEx == null)
				return;
			zipFileEx.FileLastAccessTime = LastAccessTime;
			zipFileEx.FileLastModificationTime = LastModificationTime;
			zipFileEx.FileCreationTime = CreationTime;
		}
	}
}
