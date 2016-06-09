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
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class DocPieceTable {
		#region static
		public static DocPieceTable FromByteArray(byte[] pieceTable) {
			DocPieceTable result = new DocPieceTable();
			result.Read(pieceTable);
			return result;
		}
		public static DocPieceTable CreateDefault(int textStartOffset, int lastCharacterPosition) {
			PieceDescriptor docPieceDescriptor = PieceDescriptor.FromFileOffset(textStartOffset);
			DocPieceTable result = new DocPieceTable();
			result.isDefault = true;
			result.pcdCount = 1;
			result.AddEntry(0, docPieceDescriptor);
			result.AddLastPosition(lastCharacterPosition);
			return result;
		}
		#endregion
		#region Fields
		const int pieceDescriptorSize = 8;
		bool isDefault = false;
		int pcdCount;
		List<int> characterPositions;
		List<PieceDescriptor> pieceDescriptors;
		#endregion
		public DocPieceTable() {
			this.characterPositions = new List<int>();
			this.pieceDescriptors = new List<PieceDescriptor>();
		}
		#region Properties
		public int PcdCount { get { return this.pcdCount; } }
		#endregion
		protected void Read(byte[] pieceTable) {
			this.pcdCount = (pieceTable.Length - DocConstants.CharacterPositionSize) / (DocConstants.CharacterPositionSize + pieceDescriptorSize);
			this.characterPositions.AddRange(GetCharacterPositions(pieceTable));
			this.pieceDescriptors.AddRange(GetPieceDescriptors(pieceTable));
		}
		public byte[] ToByteArray() {
			int positionsCount = characterPositions.Count;
			int pieceDescriptorsCount = pieceDescriptors.Count;
			byte[] buffer = new byte[positionsCount * DocConstants.CharacterPositionSize + pieceDescriptorsCount * pieceDescriptorSize];
			for (int i = 0; i < positionsCount; i++) {
				Array.Copy(BitConverter.GetBytes((uint)characterPositions[i]), 0, buffer, i * DocConstants.CharacterPositionSize, DocConstants.CharacterPositionSize);
			}
			int pieceDescriptorsOffset = positionsCount * DocConstants.CharacterPositionSize;
			for (int i = 0; i < pieceDescriptorsCount; i++) {
				Array.Copy(pieceDescriptors[i].ToByteArray(), 0, buffer, pieceDescriptorsOffset + i * pieceDescriptorSize, pieceDescriptorSize);
			}
			return buffer;
		}
		protected int[] GetCharacterPositions(byte[] pieceTable) {
			int[] characterPositions = new int[PcdCount + 1];
			int count = characterPositions.Length;
			for (int i = 0; i < count; i++) {
				characterPositions[i] = BitConverter.ToInt32(pieceTable, i << 2);
			}
			return characterPositions;
		}
		protected PieceDescriptor[] GetPieceDescriptors(byte[] pieceTable) {
			PieceDescriptor[] pieceDescriptors = new PieceDescriptor[PcdCount];
			int pcdOffset = (this.PcdCount + 1) * 4;
			int count = pieceDescriptors.Length;
			for (int i = 0; i < count; i++) {
				byte[] pieceDescriptor = new byte[PieceDescriptor.PieceDescriptorSize];
				Array.Copy(pieceTable, (i << 3) + pcdOffset, pieceDescriptor, 0, PieceDescriptor.PieceDescriptorSize);
				pieceDescriptors[i] = PieceDescriptor.FromByteArray(pieceDescriptor);
			}
			return pieceDescriptors;
		}
		public Encoding GetEncoding(int pcdIndex) {
			if (isDefault)
				return DXEncoding.GetEncoding(1252); 
			return this.pieceDescriptors[pcdIndex].GetEncoding();
		}
		public int GetOffset(int pcdIndex) {
			return this.pieceDescriptors[pcdIndex].GetOffset();
		}
		public int GetLength(int pcdIndex) {
			int length = this.characterPositions[pcdIndex + 1] - this.characterPositions[pcdIndex];
			int fc = this.pieceDescriptors[pcdIndex].FC;
			if ((fc & 0x40000000) != 0) return length;
			return length * 2;
		}
		public void AddEntry(int characterPosition, PieceDescriptor pieceDescriptor) {
			characterPositions.Add(characterPosition);
			pieceDescriptors.Add(pieceDescriptor);
		}
		public void AddLastPosition(int characterPosition) { 
			characterPositions.Add(characterPosition);
		}
	}
}
