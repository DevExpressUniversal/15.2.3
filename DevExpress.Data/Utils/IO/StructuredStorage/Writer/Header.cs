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
using DevExpress.Office.Utils;
namespace DevExpress.Utils.StructuredStorage.Internal.Writer {
	#region Header
	[CLSCompliant(false)]
	public class Header : AbstractHeader {
		#region Fields
		readonly List<byte> diFatSectors = new List<byte>();
		readonly StructuredStorageContext context;
		int diFatSectorCount;
		#endregion
		public Header(StructuredStorageContext context)
			: base(new OutputHandler(new ChunkedMemoryStream())) {
			Guard.ArgumentNotNull(context, "context");
			IoHandler.SetHeaderReference(this);
			IoHandler.InitBitConverter(true);
			this.context = context;
			Init();
		}
		void Init() {
			MiniSectorShift = 6;
			SectorShift = 9;
			NoSectorsInDirectoryChain4KB = 0;
			MiniSectorCutoff = 4096;
		}
		internal void WriteNextDiFatSector(UInt32 sector) {
			if (diFatSectorCount >= 109)
				context.Fat.ThrowInconsistencyException();
			diFatSectors.AddRange(context.InternalBitConverter.GetBytes(sector));
			diFatSectorCount++;
		}
		internal void Write() {
			OutputHandler outputHandler = ((OutputHandler)IoHandler);
			outputHandler.Write(BitConverter.GetBytes(MAGIC_NUMBER));
			outputHandler.Write(new byte[16]);
			outputHandler.WriteUInt16(0x3E);
			outputHandler.WriteUInt16(0x03);
			outputHandler.WriteUInt16(0xFFFE);
			outputHandler.WriteUInt16(SectorShift);
			outputHandler.WriteUInt16(MiniSectorShift);
			outputHandler.WriteUInt16(0x0);
			outputHandler.WriteUInt32(0x0);
			outputHandler.WriteUInt32(NoSectorsInDirectoryChain4KB);
			outputHandler.WriteUInt32(NoSectorsInFatChain);
			outputHandler.WriteUInt32(DirectoryStartSector);
			outputHandler.WriteUInt32(0x0);
			outputHandler.WriteUInt32(MiniSectorCutoff);
			outputHandler.WriteUInt32(MiniFatStartSector);
			outputHandler.WriteUInt32(NoSectorsInMiniFatChain);
			outputHandler.WriteUInt32(DiFatStartSector);
			outputHandler.WriteUInt32(NoSectorsInDiFatChain);
			outputHandler.Write(diFatSectors.ToArray());
			if (SectorSize == 4096)
				outputHandler.Write(new byte[4096 - 512]);
		}
		internal void writeToStream(Stream stream) {
			OutputHandler outputHandler = ((OutputHandler)IoHandler);
			outputHandler.WriteToStream(stream);
		}
	}
	#endregion
}
