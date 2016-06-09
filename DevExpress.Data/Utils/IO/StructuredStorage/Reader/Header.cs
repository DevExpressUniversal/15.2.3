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
using DevExpress.Office.Utils;
namespace DevExpress.Utils.StructuredStorage.Internal.Reader {
	#region Header
	[CLSCompliant(false)]
	public class Header : AbstractHeader {
		public Header(InputHandler fileHandler) : base(fileHandler) {
			IoHandler.SetHeaderReference(this);
			ReadHeader();
		}
		void ReadHeader() {
			InputHandler fileHandler = ((InputHandler)IoHandler);
			byte[] byteArray16 = new byte[2];
			fileHandler.ReadPosition(byteArray16, 0x1C);
			bool isLittleEndian = byteArray16[0] == 0xFE && byteArray16[1] == 0xFF;
			fileHandler.InitBitConverter(isLittleEndian);
			if (fileHandler.ReadUInt64(0x0) != MAGIC_NUMBER)
				throw new InvalidOperationException("The file you are trying to open is in different format than specified by the file extension.");
			SectorShift = fileHandler.ReadUInt16(0x1E);
			MiniSectorShift = fileHandler.ReadUInt16();
			NoSectorsInDirectoryChain4KB = fileHandler.ReadUInt32(0x28);
			NoSectorsInFatChain = fileHandler.ReadUInt32();
			DirectoryStartSector = fileHandler.ReadUInt32();
			UInt32 cutOff = fileHandler.ReadUInt32(0x38);
			MiniFatStartSector = fileHandler.ReadUInt32();
			MiniSectorCutoff = (MiniFatStartSector == SectorType.EndOfChain) ? 0x1000 : cutOff;
			NoSectorsInMiniFatChain = fileHandler.ReadUInt32();
			UInt32 startSector = fileHandler.ReadUInt32();
			UInt32 sectorCount = fileHandler.ReadUInt32();
			if (startSector == SectorType.NoStream && sectorCount == 0)
				DiFatStartSector = SectorType.EndOfChain;
			else
				DiFatStartSector = startSector;
			NoSectorsInDiFatChain = sectorCount;
		}
		protected override void ThrowArgumentException(string propName, object val) {
			string valueStr = (val != null) ? val.ToString() : "null";
			string s = String.Format("'{0}' is not a valid value for '{1}'", valueStr, propName);
			throw new ArgumentException(s);
		}
	}
	#endregion
}
