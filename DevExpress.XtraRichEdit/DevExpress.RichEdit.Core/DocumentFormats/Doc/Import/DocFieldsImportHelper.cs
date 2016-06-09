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
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using System.IO;
using DevExpress.XtraRichEdit.Export.Doc;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Fields;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocFieldsImporter
	public class DocFieldsImporter {
		#region Fields
		readonly ImportFieldHelper importFieldHelper;
		Stack<ImportFieldInfo> fieldInfoStack;
		#endregion
		public DocFieldsImporter(PieceTable pieceTable, Stack<ImportFieldInfo> fieldInfoStack) {
			this.importFieldHelper = new ImportFieldHelper(pieceTable);
			this.fieldInfoStack = fieldInfoStack;
		}
		#region Properties
		protected internal Stack<ImportFieldInfo> ImportFieldStack { get { return this.fieldInfoStack; } }
		protected ImportFieldHelper ImportFieldHelper { get { return importFieldHelper; } }
		#endregion
		public virtual void ProcessFieldBegin(InputPosition position) {
			ImportFieldInfo fieldInfo = new ImportFieldInfo(ImportFieldHelper.PieceTable);
			ImportFieldHelper.ProcessFieldBegin(fieldInfo, position);
			ImportFieldStack.Push(fieldInfo);
		}
		public virtual void ProcessFieldSeparator(InputPosition position) {
			if (ImportFieldStack.Count == 0)
				return;
			ImportFieldInfo fieldInfo = ImportFieldStack.Peek();
			ImportFieldHelper.ProcessFieldSeparator(fieldInfo, position);
		}
		public virtual void ProcessFieldEnd(InputPosition position, DocPropertyContainer propertyContainer) {
			if (ImportFieldStack.Count == 0)
				return;
			ImportFieldInfo fieldInfo = ImportFieldStack.Pop();
			fieldInfo.Locked = (propertyContainer.FieldProperties & FieldProperties.Locked) != 0;
			importFieldHelper.ProcessFieldEnd(fieldInfo, position);
			if (ImportFieldStack.Count > 0)
				fieldInfo.Field.Parent = ImportFieldStack.Peek().Field;
		}
		public virtual void ProcessHyperlinkData(DocHyperlinkFieldData hyperlinkData) {
		}
	}
	#endregion
	#region DocFieldTable
	public class DocFieldTable {
		#region static
		public static DocFieldTable FromStream(BinaryReader reader, int offset, int size) {
			DocFieldTable result = new DocFieldTable();
			result.Read(reader, offset, size);
			return result;
		}
		#endregion
		#region Fields
		const int fieldDescriptorSize = 2;
		List<int> characterPositions;
		List<IDocFieldDescriptor> fieldDescriptors;
		#endregion
		public DocFieldTable() {
			this.characterPositions = new List<int>();
			this.fieldDescriptors = new List<IDocFieldDescriptor>();
		}
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int count = (size - DocConstants.CharacterPositionSize) / (DocConstants.CharacterPositionSize + fieldDescriptorSize);
			for (int i = 0; i < count + 1; i++)
				this.characterPositions.Add(reader.ReadInt32());
			for (int i = 0; i < count; i++)
				this.fieldDescriptors.Add(DocFieldDescriptorFactory.Instance.CreateInstance(reader));
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			int count = this.characterPositions.Count;
			if (count == 0)
				return;
			for (int i = 0; i < count; i++)
				writer.Write(this.characterPositions[i]);
			for (int i = 0; i < count - 1; i++)
				this.fieldDescriptors[i].Write(writer);
		}
		public IDocFieldDescriptor GetFieldDescriptorByIndex(int index) {
			if (this.fieldDescriptors.Count > index)
				return this.fieldDescriptors[index];
			return null;
		}
		public void AddEntry(int characterPosition, IDocFieldDescriptor fieldDescriptor) {
			this.characterPositions.Add(characterPosition);
			this.fieldDescriptors.Add(fieldDescriptor);
		}
		public void Finish(int lastCharacterPosition) {
			this.characterPositions.Add(lastCharacterPosition);
		}
	}
	#endregion
	#region DocFieldDescriptorFactory
	public sealed class DocFieldDescriptorFactory {
		static readonly DocFieldDescriptorFactory instance = new DocFieldDescriptorFactory();
		public static DocFieldDescriptorFactory Instance { get { return instance; } }
		public IDocFieldDescriptor CreateInstance(BinaryReader reader) {
			byte fieldCode = reader.ReadByte();
			byte info = reader.ReadByte();
			switch (fieldCode & 0x7) {
				case 0x3:
					return new DocFieldBeginDescriptor(info);
				case 0x4:
					return new DocFieldSeparatorDescriptor();
				case 0x5:
					return new DocFieldEndDescriptor(info);
				default:
					DocImporter.ThrowInvalidDocFile();
					return null;
			}
		}
	}
	#endregion
	#region IDocFieldDescriptor
	public interface IDocFieldDescriptor {
		char BoundaryType { get; }
		void Write(BinaryWriter writer);
	}
	#endregion
	#region DocFieldBeginDescriptor
	public class DocFieldBeginDescriptor : IDocFieldDescriptor {
		#region Fields
		static Dictionary<string, byte> fieldTypes;
		const byte unknownFieldType = 0x01;
		const byte refFieldType = 0x03;
		const byte evaluateExpressionFieldType = 0x22;
		const byte noteRefFieldType = 0x48;
		const byte hyperlinkFieldType = 0x58;
		readonly char boundaryType = TextCodes.FieldBegin;
		byte fieldType;
		#endregion
		#region static
		static readonly DocFieldBeginDescriptor empty = new DocFieldBeginDescriptor(unknownFieldType);
		public static DocFieldBeginDescriptor Empty { get { return empty; } }
		static DocFieldBeginDescriptor() {
			fieldTypes = new Dictionary<string, byte>(87);
			fieldTypes.Add("REF", 3);
			fieldTypes.Add("FTNREF", 5);
			fieldTypes.Add("SET", 6);
			fieldTypes.Add("IF", 7);
			fieldTypes.Add("INDEX", 8);
			fieldTypes.Add("STYLEREF", 10);
			fieldTypes.Add("SEQ", 12);
			fieldTypes.Add("TOC", 13);
			fieldTypes.Add("INFO", 14);
			fieldTypes.Add("TITLE", 15);
			fieldTypes.Add("SUBJECT", 16);
			fieldTypes.Add("AUTHOR", 17);
			fieldTypes.Add("KEYWORDS", 18);
			fieldTypes.Add("COMMENTS", 19);
			fieldTypes.Add("LASTSAVEDBY", 20);
			fieldTypes.Add("CREATEDATE", 21);
			fieldTypes.Add("SAVEDATE", 22);
			fieldTypes.Add("PRINTDATE", 23);
			fieldTypes.Add("REVNUM", 24);
			fieldTypes.Add("EDITTIME", 25);
			fieldTypes.Add("NUMPAGES", 26);
			fieldTypes.Add("NUMWORDS", 27);
			fieldTypes.Add("NUMCHARS", 28);
			fieldTypes.Add("FILENAME", 29);
			fieldTypes.Add("TEMPLATE", 30);
			fieldTypes.Add("DATE", 31);
			fieldTypes.Add("TIME", 32);
			fieldTypes.Add("PAGE", 33);
			fieldTypes.Add("=", 34);
			fieldTypes.Add("QUOTE", 35);
			fieldTypes.Add("INCLUDE", 36);
			fieldTypes.Add("PAGEREF", 37);
			fieldTypes.Add("ASK", 38);
			fieldTypes.Add("FILLIN", 39);
			fieldTypes.Add("DATA", 40);
			fieldTypes.Add("NEXT", 41);
			fieldTypes.Add("NEXTIF", 42);
			fieldTypes.Add("SKIPIF", 43);
			fieldTypes.Add("MERGEREC", 44);
			fieldTypes.Add("DDE", 45);
			fieldTypes.Add("DDEAUTO", 46);
			fieldTypes.Add("GLOSSARY", 47);
			fieldTypes.Add("PRINT", 48);
			fieldTypes.Add("EQ", 49);
			fieldTypes.Add("GOTOBUTTON", 50);
			fieldTypes.Add("MACROBUTTON", 51);
			fieldTypes.Add("AUTONUMOUT", 52);
			fieldTypes.Add("AUTONUMLGL", 53);
			fieldTypes.Add("AUTONUM", 54);
			fieldTypes.Add("IMPORT", 55);
			fieldTypes.Add("LINK", 56);
			fieldTypes.Add("SYMBOL", 57);
			fieldTypes.Add("EMBED", 58);
			fieldTypes.Add("MERGEFIELD", 59);
			fieldTypes.Add("USERNAME", 60);
			fieldTypes.Add("USERINITIALS", 61);
			fieldTypes.Add("USERADDRESS", 62);
			fieldTypes.Add("BARCODE", 63);
			fieldTypes.Add("DOCVARIABLE", 64);
			fieldTypes.Add("SECTION", 65);
			fieldTypes.Add("SECTIONPAGES", 66);
			fieldTypes.Add("INCLUDEPICTURE", 67);
			fieldTypes.Add("INCLUDETEXT", 68);
			fieldTypes.Add("FILESIZE", 69);
			fieldTypes.Add("FORMTEXT", 70);
			fieldTypes.Add("FORMCHECKBOX", 71);
			fieldTypes.Add("NOTEREF", 72);
			fieldTypes.Add("TOA", 73);
			fieldTypes.Add("TA", 74);
			fieldTypes.Add("MERGESEQ", 75);
			fieldTypes.Add("MACRO", 76);
			fieldTypes.Add("DATABASE", 78);
			fieldTypes.Add("AUTOTEXT", 79);
			fieldTypes.Add("COMPARE", 80);
			fieldTypes.Add("ADDIN", 81);
			fieldTypes.Add("FORMDROPDOWN", 83);
			fieldTypes.Add("ADVANCE", 84);
			fieldTypes.Add("DOCPROPERTY", 85);
			fieldTypes.Add("CONTROL", 87);
			fieldTypes.Add("HYPERLINK", 88);
			fieldTypes.Add("AUTOTEXTLIST", 89);
			fieldTypes.Add("LISTNUM", 90);
			fieldTypes.Add("HTMLCONTROL", 91);
			fieldTypes.Add("BIDIOUTLINE", 92);
			fieldTypes.Add("ADDRESSBLOCK", 93);
			fieldTypes.Add("GREETINGLINE", 94);
			fieldTypes.Add("SHAPE", 95);
		}
		#endregion
		#region Constructors
		public DocFieldBeginDescriptor(byte fieldType) {
			this.fieldType = fieldType;
		}
		public DocFieldBeginDescriptor(Token token) {
			switch (token.ActualKind) {
				case TokenKind.Eq:
					this.fieldType = unknownFieldType;
					break;
				case TokenKind.OpEQ:
					this.fieldType = evaluateExpressionFieldType;
					break;
				default:
					this.fieldType = CalcFieldType(token.Value);
					break;
			}
		}
		#endregion
		#region IDocFieldDescriptor Members
		public char BoundaryType { get { return this.boundaryType; } }
		public void Write(BinaryWriter writer) {
			writer.Write(BoundaryType);
			writer.Write(this.fieldType);
		}
		#endregion
		public FieldType CalcFieldType() {
			switch (this.fieldType) {
				case refFieldType:
					return FieldType.Ref;
				case noteRefFieldType:
					return FieldType.NoteRef;
				case hyperlinkFieldType:
					return FieldType.Hyperlink;
				default:
					return FieldType.None;
			}
		}
		byte CalcFieldType(string val) {
			byte result;
			if(!fieldTypes.TryGetValue(val, out result))
				result = unknownFieldType;
			return result;
		}
	}
	#endregion
	#region DocFieldSeparatorDescriptor
	public class DocFieldSeparatorDescriptor : IDocFieldDescriptor {
		const byte separatorValue = 0xff;
		#region IDocFieldDescriptor Members
		public char BoundaryType { get { return TextCodes.FieldSeparator; } }
		public void Write(BinaryWriter writer) {
			writer.Write(this.BoundaryType);
			writer.Write(separatorValue);
		}
		#endregion
	}
	#endregion
	#region FieldProperties
	[Flags]
	public enum FieldProperties {
		Differ = 0x01,
		ZombieEmbed = 0x02,
		ResultsDirty = 0x04,
		ResultsEdited = 0x08,
		Locked = 0x10,
		PrivateResult = 0x20,
		Nested = 0x40,
		HasSeparator = 0x80
	}
	#endregion
	#region DocFieldEndDescriptor
	public class DocFieldEndDescriptor : IDocFieldDescriptor {
		#region Fields
		const byte boundaryType = 0x95;
		FieldProperties properties;
		#endregion
		public DocFieldEndDescriptor() {
			this.properties = FieldProperties.HasSeparator;
		}
		public DocFieldEndDescriptor(byte bitField){
			this.properties = (FieldProperties)bitField;
		}
		#region Properties
		public FieldProperties Properties {
			get { return this.properties; }
			protected internal set { this.properties = value; }
		}
		#endregion
		#region IDocFieldDescriptor Members
		public char BoundaryType { get { return TextCodes.FieldEnd; } }
		public void Write(BinaryWriter writer) {
			writer.Write(boundaryType);
			writer.Write((byte)this.properties);
		}
		#endregion
	}
	#endregion
	#region FieldStartComparer
	public class FieldStartComparer : IComparer<Field> {
		#region IComparer<Field> Members
		public int Compare(Field x, Field y) {
			return x.FirstRunIndex - y.FirstRunIndex;
		}
		#endregion
	}
	#endregion
	#region DocFieldBinaryData (abstract class)
	public abstract class DocFieldBinaryData {
		#region Fields
		const short headerSize = 0x44;
		const int ignoredDataSize = 0x3e;
		#endregion
		protected DocFieldBinaryData() {
		}
		protected DocFieldBinaryData(BinaryReader reader, int offset) {
			Guard.ArgumentNotNull(reader, "reader");
			Read(reader, offset);
		}
		protected void Read(BinaryReader reader, int offset){
			Guard.ArgumentNotNull(reader, "reader");
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			reader.ReadInt32(); 
			if (reader.ReadInt16() != headerSize)
				DocImporter.ThrowInvalidDocFile();
			reader.BaseStream.Seek(ignoredDataSize, SeekOrigin.Current);
			Traverse(reader);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(GetSize());
			writer.BaseStream.Seek(ignoredDataSize, SeekOrigin.Current);
			WriteBinaryData(writer);
		}
		protected abstract void Traverse(BinaryReader reader);
		protected abstract void WriteBinaryData(BinaryWriter writer);
		protected abstract int GetSize();
	}
	#endregion
	#region DocHyperlinkInformationFlags
	[Flags]
	public enum DocHyperlinkInformationFlags {
		NewWindow = 0x01,
		NoHistory = 0x02,
		ImageMap = 0x04,
		Location = 0x08,
		Tooltip = 0x10
	}
	#endregion
	#region DocHyperlinkObjectFlags
	[Flags]
	public enum DocHyperlinkObjectFlags {
		HasMoniker = 0x0001,
		IsAbsolute = 0x0002,
		SiteGaveDisplayName = 0x0004,
		HasLocationString = 0x0008,
		HasDisplayName = 0x0010,
		HasGUID = 0x0020,
		HasCreationTime = 0x0040,
		HasFrameName = 0x0080,
		MonikerSavedAsString = 0x0100,
		AbsoluteFromRelative = 0x0200
	}
	#endregion
	#region DocHyperlinkInfo
	public class DocHyperlinkInfo : DocFieldBinaryData {
		#region Fields
		const int comIdSize = 16;
		const int streamVersion = 2;
		const int streamVersionSize = 4;
		const int delimiterSize = 2;
		const int baseSize = 25;
		DocHyperlinkInformationFlags hyperlinkInformationFlags;
		DocHyperlinkObjectFlags hyperlinkObjectFlags;
		string displayName;
		string targetFrameName;
		#endregion
		public DocHyperlinkInfo(string displayName, string targetFrameName) {
			this.hyperlinkObjectFlags = DocHyperlinkObjectFlags.HasDisplayName & DocHyperlinkObjectFlags.HasFrameName;
			this.displayName = displayName;
			this.targetFrameName = targetFrameName;
		}
		public DocHyperlinkInfo(BinaryReader reader, int offset)
			: base(reader, offset) { }
		#region Properties
		public DocHyperlinkInformationFlags HyperlinkInformationFlags {
			get { return this.hyperlinkInformationFlags; }
		}
		public DocHyperlinkObjectFlags HyperlinkObjectFlags {
			get { return this.hyperlinkObjectFlags; }
		}
		#endregion
		protected override void Traverse(BinaryReader reader) {
			this.hyperlinkInformationFlags = (DocHyperlinkInformationFlags)reader.ReadByte();
			reader.BaseStream.Seek(comIdSize, SeekOrigin.Current);
			reader.BaseStream.Seek(streamVersionSize, SeekOrigin.Current); 
			this.hyperlinkObjectFlags = (DocHyperlinkObjectFlags)reader.ReadInt32();
			if ((this.hyperlinkObjectFlags & DocHyperlinkObjectFlags.HasDisplayName) == DocHyperlinkObjectFlags.HasDisplayName)
				this.displayName = ReadHyperlinkString(reader);
			if ((this.hyperlinkObjectFlags & DocHyperlinkObjectFlags.HasFrameName) == DocHyperlinkObjectFlags.HasFrameName)
				this.targetFrameName = ReadHyperlinkString(reader);
		}
		protected override void WriteBinaryData(BinaryWriter writer) {
			writer.Write((byte)this.hyperlinkInformationFlags);
			writer.Seek(comIdSize, SeekOrigin.Current);
			writer.Write(streamVersion);
			writer.Write((int)this.hyperlinkObjectFlags);
			writer.Write(this.displayName);
			writer.Write(this.targetFrameName);
		}
		protected override int GetSize() {
			return baseSize + CalcHyperlinkStringLength(this.displayName) + CalcHyperlinkStringLength(this.targetFrameName);
		}
		string ReadHyperlinkString(BinaryReader reader) {
			int length = reader.ReadInt32();
			byte[] buffer = reader.ReadBytes(length * 2 - delimiterSize);
			string result = Encoding.Unicode.GetString(buffer, 0, buffer.Length);
			reader.ReadInt16(); 
			return result;
		}
		int CalcHyperlinkStringLength(string hyperLinkString) {
			return 4 + (hyperLinkString.Length * 2) + delimiterSize;
		}
	}
	#endregion
}
