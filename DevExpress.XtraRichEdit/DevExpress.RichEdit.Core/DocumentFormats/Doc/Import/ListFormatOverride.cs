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
using DevExpress.XtraRichEdit.Export.Doc;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocListOverrideFormatInformation
	public class DocListOverrideFormatInformation {
		public static DocListOverrideFormatInformation FromStream(BinaryReader reader, int offset, int size) {
			DocListOverrideFormatInformation result = new DocListOverrideFormatInformation();
			result.Read(reader, offset, size);
			return result;
		}
		#region Fields
		List<DocListOverrideFormat> listFormatOverride;
		List<DocListOverrideLevelInformation> listFormatOverrideData;
		#endregion
		public DocListOverrideFormatInformation() {
			this.listFormatOverride = new List<DocListOverrideFormat>();
			this.listFormatOverrideData = new List<DocListOverrideLevelInformation>();
		}
		#region Properties
		public List<DocListOverrideFormat> FormatOverride { get { return this.listFormatOverride; } }
		public List<DocListOverrideLevelInformation> FormatOverrideData { get { return this.listFormatOverrideData; } }
		#endregion
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			if (size == 0)
				return;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int listCount = reader.ReadInt32();
			for (int i = 0; i < listCount; i++)
				this.listFormatOverride.Add(DocListOverrideFormat.FromStream(reader));
			for (int i = 0; i < listCount; i++)
				this.listFormatOverrideData.Add(DocListOverrideLevelInformation.FromStream(reader, this.listFormatOverride[i].LevelsCount));
		}
		public void Write(BinaryWriter writer, bool allowNonLinkedListDefinitions) {
			Guard.ArgumentNotNull(writer, "writer");
			BeforeWrite(allowNonLinkedListDefinitions);
			int count = this.listFormatOverride.Count;
			if (count == 0)
				return;
			writer.Write(count);
			for (int i = 0; i < count; i++)
				this.listFormatOverride[i].Write(writer);
			for (int i = 0; i < count; i++)
				this.listFormatOverrideData[i].Write(writer, this.listFormatOverride[i].LevelsCount);
		}
		void BeforeWrite(bool allowNonLinkedListDefinitions) {
			if (allowNonLinkedListDefinitions)
				return;
			int count = this.listFormatOverride.Count;
			for (int i = 0; i < count; i++) {
				if (this.listFormatOverrideData[i].CharacterPosition == DocListsExporter.EmptyCharacterPosition) {
					this.listFormatOverride[i].LevelsCount = 0;
					this.listFormatOverrideData[i].LevelFormatOverrideData.Clear();
				}
			}
		}
	}
	#endregion
	#region DocListOverrideFormat
	public class DocListOverrideFormat {
		public static DocListOverrideFormat FromStream(BinaryReader reader) {
			DocListOverrideFormat result = new DocListOverrideFormat();
			result.Read(reader);
			return result;
		}
		#region Fields
		int listIdentifier;
		byte levelsCount;
		byte autoNumberedFieldType;
		byte htmlCompatibilities;
		#endregion
		#region Properties
		public int ListIdentifier { 
			get { return this.listIdentifier; }
			protected internal set { this.listIdentifier = value; }
		}
		public byte LevelsCount {
			get { return this.levelsCount; }
			protected internal set { this.levelsCount = value; }
		}
		#endregion
		protected void Read(BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			this.listIdentifier = reader.ReadInt32();
			reader.BaseStream.Seek(8, SeekOrigin.Current); 
			this.levelsCount = reader.ReadByte();
			this.autoNumberedFieldType = reader.ReadByte();
			this.htmlCompatibilities = reader.ReadByte();
			reader.ReadByte();
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(this.listIdentifier);
			writer.BaseStream.Seek(8, SeekOrigin.Current);
			writer.Write(this.levelsCount);
			writer.BaseStream.Seek(3, SeekOrigin.Current);
		}
	}
	#endregion
	#region DocListOverrideLevelInformation
	public class DocListOverrideLevelInformation {
		public static DocListOverrideLevelInformation FromStream(BinaryReader reader, int levelsCount) {
			DocListOverrideLevelInformation result = new DocListOverrideLevelInformation();
			result.Read(reader, levelsCount);
			return result;
		}
		#region Fields
		int characterPosition;
		List<DocListOverrideLevelFormat> levelFormatOverrideData;
		#endregion
		public DocListOverrideLevelInformation() {
			this.levelFormatOverrideData = new List<DocListOverrideLevelFormat>();
		}
		#region Properties
		public int CharacterPosition {
			get { return characterPosition; }
			protected internal set { characterPosition = value; }
		}
		public List<DocListOverrideLevelFormat> LevelFormatOverrideData { get { return this.levelFormatOverrideData; } }
		#endregion
		protected void Read(BinaryReader reader, int levelsCount) {
			Guard.ArgumentNotNull(reader, "reader");
			this.characterPosition = (int)reader.ReadUInt32();
			for (int i = 0; i < levelsCount; i++)
				this.levelFormatOverrideData.Add(DocListOverrideLevelFormat.FromStream(reader));
		}
		public void Write(BinaryWriter writer, int levelsCount) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(this.characterPosition);
			for (int i = 0; i < levelsCount; i++)
				this.levelFormatOverrideData[i].Write(writer);
		}
	}
	#endregion
	#region DocListOverrideLevelFormat
	public class DocListOverrideLevelFormat {
		public static DocListOverrideLevelFormat FromStream(BinaryReader reader) {
			DocListOverrideLevelFormat result = new DocListOverrideLevelFormat();
			result.Read(reader);
			return result;
		}
		#region Fields
		int startAt;
		int overriddenLevel;
		bool overrideStart;
		bool overrideFormatting;
		DocListLevel overrideLevelFormatting;
		#endregion
		#region Properties
		public int StartAt {
			get { return this.startAt; }
			protected internal set { this.startAt = value; }
		}
		public int OverriddenLevel {
			get { return this.overriddenLevel; }
			protected internal set { this.overriddenLevel = value; }
		}
		public bool OverrideStart { 
			get { return this.overrideStart; }
			protected internal set { this.overrideStart = value; }
		}
		public bool OverrideFormatting { 
			get { return this.overrideFormatting; }
			protected internal set { this.overrideFormatting = value; }
		}
		public DocListLevel OverrideLevelFormatting {
			get { return this.overrideLevelFormatting; }
			protected internal set { this.overrideLevelFormatting = value; }
		}
		#endregion
		protected void Read(BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			this.startAt = reader.ReadInt32();
			int listLevelOptionsBitField = reader.ReadInt32();
			this.overriddenLevel = listLevelOptionsBitField & 0x0f;
			this.overrideStart = (listLevelOptionsBitField & 0x10) == 0x10;
			this.overrideFormatting = (listLevelOptionsBitField & 0x20) == 0x20;
			if (this.overrideFormatting)
				this.overrideLevelFormatting = DocListLevel.FromStream(reader);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			BeforeWrite();
			writer.Write(this.startAt);
			int listLevelOptionsBitField = this.overriddenLevel;
			if (this.overrideStart)
				listLevelOptionsBitField |= 0x10;
			if (this.overrideFormatting)
				listLevelOptionsBitField |= 0x20;
			writer.Write(listLevelOptionsBitField);
			if (this.overrideFormatting)
				this.overrideLevelFormatting.Write(writer);
		}
		void BeforeWrite() {
			if (!OverrideStart || (OverrideStart && OverrideFormatting))
				this.startAt = 0;
		}
	}
	#endregion
}
