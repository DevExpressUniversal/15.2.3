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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Export.Rtf;
namespace DevExpress.XtraRichEdit.Export.Doc {
	public class FKPWriter : IDisposable {
		#region Fields
		BinaryWriter propertiesWriter;
		BinaryWriter dataStreamWriter;
		CHPXFormattedDiskPage currentCharacterFKP;
		PAPXFormattedDiskPage currentParagraphFKP;
		BinTable charactersBinTable;
		BinTable paragraphsBinTable;
		readonly int textStartPosition;
		#endregion
		public FKPWriter(int textStartPosition, MemoryStream propertiesStream, BinaryWriter dataStreamWriter) {
			Guard.ArgumentNotNull(propertiesStream, "propertiesStream");
			Guard.ArgumentNotNull(dataStreamWriter, "dataStreamWriter");
			this.textStartPosition = textStartPosition;
			this.propertiesWriter = new BinaryWriter(propertiesStream);
			this.dataStreamWriter = dataStreamWriter;
			this.charactersBinTable = new BinTable();
			this.paragraphsBinTable = new BinTable();
			this.currentCharacterFKP = new CHPXFormattedDiskPage();
			this.currentParagraphFKP = new PAPXFormattedDiskPage();
		}
		public int TextStartPosition { get { return this.textStartPosition; } }
		protected BinaryWriter PropertiesWriter { get { return this.propertiesWriter; } }
		protected BinaryWriter DataStreamWriter { get { return this.dataStreamWriter; } }
		public BinTable CharactersBinTable { get { return this.charactersBinTable; } }
		public BinTable ParagraphsBinTable { get { return this.paragraphsBinTable; } }
		public void WriteTextRun(int characterPosition, byte[] propertyModifiers) {
			WriteTextRunBase(characterPosition, propertyModifiers);
		}
		public void WriteTextRun(int characterPosition) {
			WriteTextRunBase(characterPosition, new byte[] { });
		}
		void WriteTextRunBase(int characterPosition, byte[] grpprl) {
			int filePosition = GetFilePositionByCharacterPosition(characterPosition);
			if (currentCharacterFKP.TryToAddGrpprlAndPosition(filePosition, grpprl))
				return;
			WriteCurrentCharacterFKP(filePosition);
			currentCharacterFKP = new CHPXFormattedDiskPage();
			if (currentCharacterFKP.TryToAddGrpprlAndPosition(filePosition, grpprl))
				return;
			Exceptions.ThrowInternalException();
		}
		public void WriteParagraph(int characterPosition, int paragraphStyleIndex) {
			WriteParagraphBase(characterPosition, paragraphStyleIndex, new byte[] { });
		}
		public void WriteParagraph(int characterPosition, int paragraphStyleIndex, byte[] propertyModifiers) {
			WriteParagraphBase(characterPosition, paragraphStyleIndex, propertyModifiers);
		}
		void WriteParagraphBase(int characterPosition, int paragraphStyleIndex, byte[] grpprl) {
			int filePosition = GetFilePositionByCharacterPosition(characterPosition);
			WriteParagarphGrpprlAndPosition(filePosition, paragraphStyleIndex, grpprl);
		}
		void WriteParagarphGrpprlAndPosition(int filePosition, int paragraphStyleIndex, byte[] grpprl) {
			if (currentParagraphFKP.TryToAddGrpprlAndPosition(filePosition, paragraphStyleIndex, grpprl))
				return;
			WriteCurrentParagraphFKP(filePosition);
			currentParagraphFKP = new PAPXFormattedDiskPage();
			if (currentParagraphFKP.TryToAddGrpprlAndPosition(filePosition, paragraphStyleIndex, grpprl))
				return;
			CreateHugeParagraphFKP(filePosition, paragraphStyleIndex, grpprl);
		}
		void CreateHugeParagraphFKP(int filePosition, int styleIndex, byte[] grpprl) {
			short opcode = DocCommandFactory.GetOpcodeByType(typeof(DocCommandReadExtendedPropertyModifiers));
			byte[] operand = BitConverter.GetBytes((short)DataStreamWriter.BaseStream.Position);
			byte[] extendedPropertyModifiersData = DocCommandHelper.CreateSinglePropertyModifier(opcode, operand);
			if (!currentParagraphFKP.TryToAddGrpprlAndPosition(filePosition, styleIndex, extendedPropertyModifiersData))
				Exceptions.ThrowInternalException();
			DataStreamWriter.Write((short)grpprl.Length);
			DataStreamWriter.Write(grpprl);
		}
		public void WriteTableProperties(int characterPosition, int styleIndex, byte[] grpprl) {
			int filePosition = GetFilePositionByCharacterPosition(characterPosition);
			short opcode = DocCommandFactory.GetOpcodeByType(typeof(DocCommandReadTableProperties));
			byte[] operand = BitConverter.GetBytes((int)DataStreamWriter.BaseStream.Position);
			byte[] tablePropertyModifiers = DocCommandHelper.CreateSinglePropertyModifier(opcode, operand);
			DataStreamWriter.Write((short)grpprl.Length);
			DataStreamWriter.Write(grpprl);
			WriteParagarphGrpprlAndPosition(filePosition, styleIndex, tablePropertyModifiers);
		}
		void WriteCurrentCharacterFKP(int lastPosition) {
			int fkpOffset = (int)PropertiesWriter.BaseStream.Position;
			int fcFirst = currentCharacterFKP.GetFirstOffset();
			currentCharacterFKP.AddLastPosition(lastPosition);
			currentCharacterFKP.Write(PropertiesWriter);
			CharactersBinTable.AddEntry(fcFirst, fkpOffset);
		}
		void WriteCurrentParagraphFKP(int lastFKPPosition) {
			int fkpOffset = (int)PropertiesWriter.BaseStream.Position;
			int fcFirst = currentParagraphFKP.GetFirstOffset();
			currentParagraphFKP.AddLastPosition(lastFKPPosition);
			currentParagraphFKP.Write(PropertiesWriter);
			ParagraphsBinTable.AddEntry(fcFirst, fkpOffset);
		}
		public void Finish(int lastCharacterPosition) {
			int lastFilePosition = GetFilePositionByCharacterPosition(lastCharacterPosition);
			WriteCurrentCharacterFKP(lastFilePosition);
			WriteCurrentParagraphFKP(lastFilePosition);
			CharactersBinTable.AddLastPosition(lastFilePosition);
			ParagraphsBinTable.AddLastPosition(lastFilePosition);
			int offset = (lastFilePosition % DocContentBuilder.SectorSize == 0) ? lastFilePosition / DocContentBuilder.SectorSize : lastFilePosition / DocContentBuilder.SectorSize + 1;
			UpdateBinTables(offset);
		}
		protected int GetFilePositionByCharacterPosition(int characterPosition) {
			return textStartPosition + characterPosition * 2;
		}
		void UpdateBinTables(int offset) {
			CharactersBinTable.UpdateSectorsOffsets(offset);
			ParagraphsBinTable.UpdateSectorsOffsets(offset);
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				IDisposable propertiesWriter = PropertiesWriter as IDisposable;
				if (propertiesWriter != null) {
					propertiesWriter.Dispose();
					propertiesWriter = null;
				}
				IDisposable dataWriter = DataStreamWriter as IDisposable;
				if (dataWriter != null) {
					dataWriter.Dispose();
					dataWriter = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~FKPWriter() {
			Dispose(false);
		}
		#endregion
	}
}
