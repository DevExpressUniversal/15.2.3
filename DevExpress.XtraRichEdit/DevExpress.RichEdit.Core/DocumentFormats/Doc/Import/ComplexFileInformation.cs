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
using System.Text;
using System.IO;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class ComplexFileInformation {
		public static ComplexFileInformation FromStream(BinaryReader reader, int offset, int size) {
			ComplexFileInformation result = new ComplexFileInformation();
			if (size != 0)
				result.Read(reader, offset, size);
			return result;
		}
		const byte pieceTableByteCode = 2; 
		const int complexTypeSize = 1;
		const int sizeofGrpprlSize = 2;
		const int sizeofPieceTableSize = 4;
		byte[] pieceTable;
		public byte[] PieceTable { get { return pieceTable; } protected internal set { pieceTable = value; } }
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			byte[] complexFileInformation = reader.ReadBytes(size);
			this.pieceTable = ExtractPieceTable(complexFileInformation);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(pieceTableByteCode);
			writer.Write((uint)PieceTable.Length);
			writer.Write(PieceTable);
		}
		byte[] ExtractPieceTable(byte[] complexFileInformation) {
			bool isPieceTableRetrieved = false;
			int currentPosition = 0;
			byte currentEntryType = complexFileInformation[0];
			byte[] pieceTable = null;
			while (!isPieceTableRetrieved && (currentPosition < complexFileInformation.Length)) {
				if (currentEntryType == pieceTableByteCode) {
					Int32 pieceTableSize = BitConverter.ToInt32(complexFileInformation, currentPosition + 1);
					pieceTable = new byte[pieceTableSize];
					Array.Copy(complexFileInformation, currentPosition + complexTypeSize + sizeofPieceTableSize, pieceTable, 0, pieceTable.Length);
					isPieceTableRetrieved = true;
				}
				else {
					short grpplSize = BitConverter.ToInt16(complexFileInformation, currentPosition + 1);
					currentPosition = currentPosition + complexTypeSize + sizeofGrpprlSize + grpplSize; 
					currentEntryType = complexFileInformation[currentPosition];
				}
			}
			return pieceTable;
		}
	}
}
