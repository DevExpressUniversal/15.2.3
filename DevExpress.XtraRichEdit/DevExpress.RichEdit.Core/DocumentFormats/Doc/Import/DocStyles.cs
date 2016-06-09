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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Export.Doc;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Utils;
using System.Collections.ObjectModel;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region StyleDescriptionFactory
	public static class StyleDescriptionFactory {
		public static StyleDescriptionBase CreateStyleDescription(BinaryReader reader, StyleType type, StyleSheetInformation info) {
			short size = info.BaseStyleDescriptionSize;
			bool isExtended = info.ContainsStdfPost2000;
			switch (type) {
				case StyleType.ParagraphStyle: return ParagraphStyleDescription.FromStream(reader, size, isExtended);
				case StyleType.CharacterStyle: return CharacterStyleDescription.FromStream(reader, size, isExtended);
				case StyleType.TableStyle: return TableStyleDescription.FromStream(reader, size, isExtended);
				case StyleType.NumberingListStyle: return ListStyleDescription.FromStream(reader, size, isExtended);
			}
			DocImporter.ThrowInvalidDocFile();
			return null;
		}
	}
	#endregion
	#region StyleDescriptionBase (abstract class)
	public abstract class StyleDescriptionBase {
		#region Fields
		public const short EmptyStyleIdentifier = 0x0fff;
		const short userDefinedStyleIdentifier = 0x0ffe;
		const short defaultFlags = 0x2000;
		const int sizeOfBytesCount = 2;
		const int unusedFlagsSize = 2;
		const int rsidSize = 4;
		const int styleSizeOffset = 6;
		long start;
		long end;
		short baseSize;
		StyleType styleType;
		readonly short upxCount;
		#endregion
		protected StyleDescriptionBase(StyleType styleType, short upxCount) {
			this.styleType = styleType;
			this.upxCount = upxCount;
			StyleIdentifier = userDefinedStyleIdentifier;
			this.baseSize = DocStyleSheet.ExtendedStyleDescriptionBaseInFile;
			BaseStyleIndex = EmptyStyleIdentifier;
		}
		#region Properties
		protected long Start { get { return start; } }
		protected short BaseSize { get { return baseSize; } }
		public short StyleIdentifier { get; set; }
		public StyleType StyleType { get { return styleType; } }
		public bool Hidden { get; set; }
		public bool QFormat { get; set; }
		public int StyleIndex { get; set; }
		public short Priority { get; set; }
		public short BaseStyleIndex { get; set; }
		public short NextStyleIndex { get; set; }
		public short LinkedStyleIndex { get; set; }
		public string StyleName { get; set; }
		#endregion
		protected void Read(BinaryReader reader, short baseSize, bool isExtended) {
			Guard.ArgumentNotNull(reader, "reader");
			this.start = reader.BaseStream.Position;
			this.baseSize = baseSize;
			StyleIdentifier = (short)(reader.ReadUInt16() & 0x0fff); 
			ushort styleKindAndBaseStyleIdentifier = reader.ReadUInt16(); 
			this.styleType = (StyleType)((styleKindAndBaseStyleIdentifier & 0x000f) - 1); 
			BaseStyleIndex = (short)(styleKindAndBaseStyleIdentifier >> 4);
			NextStyleIndex = (short)(reader.ReadUInt16() >> 4);
			reader.BaseStream.Seek(unusedFlagsSize, SeekOrigin.Current);
			int generalStyleProperties = reader.ReadInt16();
			Hidden = Convert.ToBoolean(generalStyleProperties & 0x0002);
			QFormat = Convert.ToBoolean(generalStyleProperties & 0x1000);
			if (isExtended) {
				LinkedStyleIndex = (short)(reader.ReadUInt16() & 0x0fff);
				reader.BaseStream.Seek(rsidSize, SeekOrigin.Current);
				Priority = (short)(reader.ReadUInt16() >> 4);
			}
			reader.BaseStream.Seek(Start + BaseSize, SeekOrigin.Begin);
			byte styleNameLength = reader.ReadByte();
			if (reader.ReadByte() == 0x00) {
				byte[] buffer = reader.ReadBytes(styleNameLength * 2);
				string styleName = Encoding.Unicode.GetString(buffer, 0, buffer.Length); 
				StyleName = StringHelper.RemoveSpecialSymbols(styleName);
				reader.ReadInt16(); 
			}
			else {
				reader.BaseStream.Seek(-1, SeekOrigin.Current);
				byte[] buffer = reader.ReadBytes(styleNameLength);
				string styleName = DXEncoding.ASCII.GetString(buffer, 0, buffer.Length);
				StyleName = StringHelper.RemoveSpecialSymbols(styleName);
				reader.ReadByte();
			}
			AlignOffset(reader);
			ReadUPX(reader);
		}
		public void Write(BinaryWriter writer, bool isExtended) {
			Guard.ArgumentNotNull(writer, "writer");
			this.start = writer.BaseStream.Position;
			writer.BaseStream.Seek(sizeOfBytesCount, SeekOrigin.Current); 
			writer.Write((ushort)(defaultFlags | StyleIdentifier));
			writer.Write((ushort)((BaseStyleIndex << 4) | (ushort)(this.styleType + 1)));
			writer.Write((ushort)((NextStyleIndex << 4) | (ushort)this.upxCount));
			writer.BaseStream.Seek(unusedFlagsSize, SeekOrigin.Current);
			writer.Write((ushort)((Convert.ToInt16(Hidden) << 1) + (Convert.ToInt16(QFormat) << 12)));
			if (isExtended) {
				LinkedStyleIndex = Math.Max((short)0, LinkedStyleIndex);
				writer.Write((ushort)LinkedStyleIndex);
				writer.Write(0);
				writer.Write((ushort)(Priority << 4));
			}
			writer.BaseStream.Seek(Start + BaseSize + sizeOfBytesCount, SeekOrigin.Begin);
			writer.Write((ushort)StyleName.Length);
			writer.Write(Encoding.Unicode.GetBytes(StyleName));
			writer.BaseStream.Seek(2, SeekOrigin.Current); 
			AlignOffset(writer);
			WriteUPX(writer);
			this.end = writer.BaseStream.Position;
			writer.BaseStream.Seek(this.start, SeekOrigin.Begin);
			short styleDescriptionSize = (short)(this.end - this.start - sizeOfBytesCount);
			writer.Write(styleDescriptionSize);
			writer.Seek(styleSizeOffset, SeekOrigin.Current);
			writer.Write(styleDescriptionSize);
			writer.BaseStream.Seek(this.end, SeekOrigin.Begin);
		}
		protected abstract void ReadUPX(BinaryReader reader);
		protected abstract void WriteUPX(BinaryWriter writer);
		protected internal virtual byte[] ReadParagraphUPX(BinaryReader reader, int paragraphUPXLength) {
			if (paragraphUPXLength == 0)
				return new byte[] { };
			return reader.ReadBytes(paragraphUPXLength - 2);
		}
		protected void AlignOffset(BinaryReader reader) { 
			if ((reader.BaseStream.Position - this.start) % 2 != 0)
				reader.BaseStream.Seek(1, SeekOrigin.Current);
		}
		protected void AlignOffset(BinaryWriter writer) {
			if ((writer.BaseStream.Position - this.start) % 2 != 0) {
				writer.BaseStream.Seek(1, SeekOrigin.Current);
			}
		}
	}
	#endregion
	#region StyleDescriptionTopologicalComparer
	public class StyleDescriptionTopologicalComparer : IComparer<StyleDescriptionBase> {
		public int Compare(StyleDescriptionBase x, StyleDescriptionBase y) {
			if (x == null || y == null)
				return 0;
			if (x.BaseStyleIndex == y.StyleIndex)
				return 1;
			if (y.BaseStyleIndex == x.StyleIndex)
				return -1;
			return 0;
		}
	}
	#endregion
	#region StyleDescriptionCollection
	public class StyleDescriptionCollection : List<StyleDescriptionBase> {
	}
	#endregion
	#region ParagraphStyleDescription
	public class ParagraphStyleDescription : StyleDescriptionBase {
		public static ParagraphStyleDescription FromStream(BinaryReader reader, short baseSize, bool isExtended) {
			ParagraphStyleDescription result = new ParagraphStyleDescription();
			result.Read(reader, baseSize, isExtended);
			return result;
		}
		#region Fields
		byte[] characterUPX; 
		byte[] paragraphUPX; 
		#endregion
		public ParagraphStyleDescription()
			: base(StyleType.ParagraphStyle, 2) {
		}
		public ParagraphStyleDescription(ParagraphStyle paragraphStyle, int fontNameIndex)
			: base(StyleType.ParagraphStyle, 2) {
			using (MemoryStream paragraphMemoryStream = new MemoryStream()) {
				DocParagraphPropertiesActions actions = new DocParagraphPropertiesActions(paragraphMemoryStream, paragraphStyle);
				actions.CreateParagarphPropertyModifiers();
				this.paragraphUPX = paragraphMemoryStream.ToArray();
			}
			using (MemoryStream characterMemoryStream = new MemoryStream()) {
				DocCharacterPropertiesActions actions = new DocCharacterPropertiesActions(characterMemoryStream, paragraphStyle.CharacterProperties, fontNameIndex);
				actions.CreateCharacterPropertiesModifiers();
				this.characterUPX = characterMemoryStream.ToArray();
			}
		}
		#region Properties
		public byte[] CharacterUPX { get { return this.characterUPX; } }
		public byte[] ParagraphUPX { get { return this.paragraphUPX; } }
		#endregion
		protected override void ReadUPX(BinaryReader reader) {
			ushort paragraphUPXLength = reader.ReadUInt16();
			this.StyleIndex = reader.ReadUInt16();
			this.paragraphUPX = ReadParagraphUPX(reader, paragraphUPXLength);
			AlignOffset(reader);
			ushort characterUPXLength = reader.ReadUInt16();
			this.characterUPX = reader.ReadBytes(characterUPXLength);
		}
		protected override void WriteUPX(BinaryWriter writer) {
			writer.Write((ushort)(ParagraphUPX.Length + 2));
			writer.Write((ushort)StyleIndex);
			writer.Write(ParagraphUPX);
			AlignOffset(writer);
			writer.Write((ushort)CharacterUPX.Length);
			writer.Write(CharacterUPX);
			AlignOffset(writer);
		}
	}
	#endregion
	#region CharacterStyleDescription
	public class CharacterStyleDescription : StyleDescriptionBase {
		public static CharacterStyleDescription FromStream(BinaryReader reader, short baseSize, bool isExtended) {
			CharacterStyleDescription result = new CharacterStyleDescription();
			result.Read(reader, baseSize, false);
			return result;
		}
		#region Fields
		byte[] characterUPX; 
		#endregion
		public CharacterStyleDescription()
			: base(StyleType.CharacterStyle, 1) {
		}
		public CharacterStyleDescription(CharacterStyle characterStyle, int fontNameIndex)
			: base(StyleType.CharacterStyle, 1) {
			using (MemoryStream characterMemoryStream = new MemoryStream()) {
				DocCharacterPropertiesActions actions = new DocCharacterPropertiesActions(characterMemoryStream, characterStyle.CharacterProperties, fontNameIndex);
				actions.CreateCharacterPropertiesModifiers();
				this.characterUPX = characterMemoryStream.ToArray();
			}
		}
		#region Properties
		public byte[] CharacterUPX { get { return this.characterUPX; } }
		#endregion
		protected override void ReadUPX(BinaryReader reader) {
			ushort characterUPXLength = reader.ReadUInt16(); 
			this.characterUPX = reader.ReadBytes(characterUPXLength);
		}
		protected override void WriteUPX(BinaryWriter writer) {
			writer.Write((ushort)CharacterUPX.Length);
			writer.Write(CharacterUPX);
			AlignOffset(writer);
		}
	}
	#endregion
	#region TableStyleDescription
	public class TableStyleDescription : StyleDescriptionBase {
		public static TableStyleDescription FromStream(BinaryReader reader, short baseSize, bool isExtended) {
			TableStyleDescription result = new TableStyleDescription();
			result.Read(reader, baseSize, isExtended);
			return result;
		}
		#region Fields
		byte[] tableUPX;
		byte[] paragraphUPX;
		byte[] characterUPX;
		#endregion
		public TableStyleDescription()
			: base(StyleType.TableStyle, 3) {
		}
		public TableStyleDescription(TableStyle tableStyle, int fontNameIndex)
			: base(StyleType.TableStyle, 3) {
			using (MemoryStream characterMemoryStream = new MemoryStream()) {
				DocCharacterPropertiesActions actions = new DocCharacterPropertiesActions(characterMemoryStream, tableStyle.CharacterProperties, fontNameIndex);
				actions.CreateCharacterPropertiesModifiers();
				this.characterUPX = characterMemoryStream.ToArray();
			}
			using (MemoryStream paragraphMemoryStream = new MemoryStream()) {
				DocParagraphPropertiesActions actions = new DocParagraphPropertiesActions(paragraphMemoryStream, tableStyle.ParagraphProperties, tableStyle.Tabs.Info);
				actions.CreateParagarphPropertyModifiers();
				this.paragraphUPX = paragraphMemoryStream.ToArray();
			}
			using (MemoryStream tableMemoryStream = new MemoryStream()) {
				DocTableActions actions = new DocTableActions(tableMemoryStream, tableStyle);
				actions.CreateTablePropertyModifiers();
				this.tableUPX = tableMemoryStream.ToArray();
			}
		}
		#region Properties
		public byte[] TableUPX { get { return this.tableUPX; } }
		public byte[] ParagraphUPX { get { return this.paragraphUPX; } }
		public byte[] CharacterUPX { get { return this.characterUPX; } }
		#endregion
		protected override void ReadUPX(BinaryReader reader) {
			ushort tableUPXLength = reader.ReadUInt16();
			this.tableUPX = reader.ReadBytes(tableUPXLength);
			AlignOffset(reader);
			ushort paragraphUPXLength = reader.ReadUInt16();
			this.StyleIndex = reader.ReadUInt16();
			this.paragraphUPX = ReadParagraphUPX(reader, paragraphUPXLength);
			AlignOffset(reader);
			ushort characterUPXLength = reader.ReadUInt16();
			this.characterUPX = reader.ReadBytes(characterUPXLength);
		}
		protected override void WriteUPX(BinaryWriter writer) {
			writer.Write((ushort)TableUPX.Length);
			writer.Write(TableUPX);
			AlignOffset(writer);
			writer.Write((ushort)(ParagraphUPX.Length + 2));
			writer.Write((ushort)StyleIndex);
			writer.Write(ParagraphUPX);
			AlignOffset(writer);
			writer.Write((ushort)CharacterUPX.Length);
			writer.Write(CharacterUPX);
			AlignOffset(writer);
		}
	}
	#endregion
	#region ListStyleDescription
	public class ListStyleDescription : StyleDescriptionBase {
		public static ListStyleDescription FromStream(BinaryReader reader, short baseSize, bool isExtended) {
			ListStyleDescription result = new ListStyleDescription();
			result.Read(reader, baseSize, isExtended);
			return result;
		}
		#region Fields
		byte[] paragraphUPX;
		ushort styleIndex;
		#endregion
		public ListStyleDescription()
			: base(StyleType.NumberingListStyle, 1) {
			this.paragraphUPX = new byte[] { };
		}
		public ListStyleDescription(NumberingListStyle listStyle)
			: base(StyleType.NumberingListStyle, 1) {
			NumberingListIndex index = listStyle.NumberingListIndex + 1;
			int value = ((IConvertToInt<NumberingListIndex>)index).ToInt();
			this.paragraphUPX = DocCommandHelper.CreateSinglePropertyModifier(0x460b, BitConverter.GetBytes(Convert.ToInt16(value)));
		}
		#region Properties
		public byte[] ParagraphUPX { get { return this.paragraphUPX; } }
		#endregion
		protected override void ReadUPX(BinaryReader reader) {
			ushort paragraphUPXLength = reader.ReadUInt16();
			this.styleIndex = reader.ReadUInt16();
			this.paragraphUPX = ReadParagraphUPX(reader, paragraphUPXLength);
		}
		protected override void WriteUPX(BinaryWriter writer) {
			writer.Write((ushort)(ParagraphUPX.Length + 2));
			writer.Write((ushort)StyleIndex);
			writer.Write(ParagraphUPX);
			AlignOffset(writer);
		}
	}
	#endregion
}
