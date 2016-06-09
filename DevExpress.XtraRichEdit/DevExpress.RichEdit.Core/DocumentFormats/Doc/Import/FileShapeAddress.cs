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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region FileShapeAddress
	public class FileShapeAddress {
		#region static
		public static FileShapeAddress FromStream(BinaryReader reader) {
			FileShapeAddress result = new FileShapeAddress();
			result.Read(reader);
			return result;
		}
		#endregion
		#region Fields
		public const int Size = 26;
		int shapeIdentifier;
		int left;
		int top;
		int right;
		int bottom;
		FloatingObjectHorizontalPositionType horizontalPositionType;
		FloatingObjectVerticalPositionType verticalPositionType;
		FloatingObjectTextWrapType textWrapType;
		FloatingObjectTextWrapSide textWrapSide;
		bool isBehindDoc;
		bool anchorLock;
		int textBoxesCount;
		#endregion
		#region Properties
		public int ShapeIdentifier { get { return shapeIdentifier; } protected internal set { shapeIdentifier = value; } }
		public int WidhtInTwips { get { return right - left; } }
		public int HeightInTwips { get { return bottom - top; } }
		public int Left { get { return left; } protected internal set { left = value; } }
		public int Top { get { return top; } protected internal set { top = value; } }
		public int Right { get { return right; } protected internal set { right = value; } }
		public int Bottom { get { return bottom; } protected internal set { bottom = value; } }
		public FloatingObjectHorizontalPositionType HorisontalPositionType { get { return horizontalPositionType; } protected internal set { horizontalPositionType = value; } }
		public FloatingObjectVerticalPositionType VericalPositionType { get { return verticalPositionType; } protected internal set { verticalPositionType = value; } }
		public FloatingObjectTextWrapType TextWrapType { get { return textWrapType; } protected internal set { textWrapType = value; } }
		public FloatingObjectTextWrapSide TextWrapSide { get { return textWrapSide; } protected internal set { textWrapSide = value; } }
		public bool UseIsBehindDoc { get; set; }
		public bool IsBehindDoc { get { return isBehindDoc; } protected internal set { isBehindDoc = value; } }
		public bool Locked { get { return anchorLock; } protected internal set { anchorLock = value; } }
		#endregion
		protected internal void Read(BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			this.shapeIdentifier = reader.ReadInt32();
			this.left = reader.ReadInt32();
			this.top = reader.ReadInt32();
			this.right = reader.ReadInt32();
			this.bottom = reader.ReadInt32();
			short flags = reader.ReadInt16();
			this.horizontalPositionType = DocFloatingObjectHorizontalPositionTypeCalculator.CalcHorizontalPositionType97((flags & 0x06) >> 1);
			this.verticalPositionType = DocFloatingObjectVerticalPositionTypeCalculator.CalcVerticalPositionType97((flags & 0x18) >> 3);
			this.textWrapType = DocFloatingObjectTextWrapTypeCalculator.CalcTextWrapType((flags & 0x01e0) >> 5);
			this.textWrapSide = DocFloatingObjectTextWrapSideCalculator.CalcTextWrapSide((flags & 0x1e00) >> 9);
			bool belowText = Convert.ToBoolean(flags & 0x4000);
			if (TextWrapType == DocFloatingObjectTextWrapTypeCalculator.WrapTypeBehindText) {
				TextWrapType = FloatingObjectTextWrapType.None;
				UseIsBehindDoc = true;
				IsBehindDoc = belowText;
			}
			this.anchorLock = Convert.ToBoolean(flags & 0x8000);
			this.textBoxesCount = reader.ReadInt32();
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(ShapeIdentifier);
			writer.Write(Left);
			writer.Write(Top);
			writer.Write(Right);
			writer.Write(Bottom);
			ushort flags = 0;
			flags |= (ushort)(DocFloatingObjectHorizontalPositionTypeCalculator.CalcHorizontalPositionTypeCode97(HorisontalPositionType) << 1);
			flags |= (ushort)(DocFloatingObjectVerticalPositionTypeCalculator.CalcVerticalPositionTypeCode97(VericalPositionType) << 3);
			flags |= (ushort)(DocFloatingObjectTextWrapTypeCalculator.CalcTextWrapTypeCode(TextWrapType) << 5);
			flags |= (ushort)(DocFloatingObjectTextWrapSideCalculator.CalcTextWrapSideTypeCode(TextWrapSide) << 9);
			if (TextWrapType == DocFloatingObjectTextWrapTypeCalculator.WrapTypeBehindText && IsBehindDoc)
				flags |= 0x4000;
			if (Locked)
				flags |= 0x8000;
			writer.Write(flags);
			writer.Write(this.textBoxesCount);
		}
	}
	#endregion
	#region FileShapeAddressTable
	public class FileShapeAddressTable {
		#region static
		public static FileShapeAddressTable FromStream(BinaryReader reader, int offset, int size) {
			FileShapeAddressTable result = new FileShapeAddressTable();
			result.Read(reader, offset, size);
			return result;
		}
		#endregion
		#region Fields
		List<int> characterPositions;
		List<FileShapeAddress> shapeAddresses;
		Dictionary<int, FileShapeAddress> translationTable;
		#endregion
		public FileShapeAddressTable() {
			this.characterPositions = new List<int>();
			this.shapeAddresses = new List<FileShapeAddress>();
			this.translationTable = new Dictionary<int, FileShapeAddress>();
		}
		#region Properties
		protected List<int> CharacterPositions { get { return characterPositions; } }
		protected List<FileShapeAddress> ShapeAddresses { get { return shapeAddresses; } }
		public int AddressesCount { get { return shapeAddresses.Count; } }
		protected internal Dictionary<int, FileShapeAddress> TranslationTable { get { return translationTable; } }
		#endregion
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			if (size == 0)
				return;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int count = (size - DocConstants.CharacterPositionSize) / (DocConstants.CharacterPositionSize + FileShapeAddress.Size);
			for (int i = 0; i < count + 1; i++)
				CharacterPositions.Add(reader.ReadInt32());
			for (int i = 0; i < count; i++)
				ShapeAddresses.Add(FileShapeAddress.FromStream(reader));
			for (int i = 0; i < count; i++)
				TranslationTable.Add(CharacterPositions[i], ShapeAddresses[i]);
		}
		public void AddEntry(int characterPosition, FileShapeAddress address) {
			CharacterPositions.Add(characterPosition);
			ShapeAddresses.Add(address);
		}
		public void Finish(int characterPosition) {
			CharacterPositions.Add(characterPosition);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			int count = CharacterPositions.Count;
			if (count == 0)
				return;
			for (int i = 0; i < count; i++)
				writer.Write(CharacterPositions[i]);
			for (int i = 0; i < count - 1; i++)
				ShapeAddresses[i].Write(writer);
		}
	}
	#endregion
}
