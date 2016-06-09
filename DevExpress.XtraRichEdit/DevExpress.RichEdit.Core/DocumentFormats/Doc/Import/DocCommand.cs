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
using System.Reflection;
using System.IO;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Printing;
#if !SL
using System.Drawing;
using System.Drawing.Printing;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region ChangeActionTypes
	[Flags]
	public enum ChangeActionTypes {
		None = 0x00,
		Character = 0x01,
		Paragraph = 0x02,
		Section = 0x04,
		TableCell = 0x08,
		TableRow = 0x10,
		Table = 0x20,
		Frame = 0x40
	}
	#endregion
	#region DocCommandInfo
	public class DocCommandInfo {
		short opcode;
		Type commandType;
		public DocCommandInfo(short opcode, Type commandType) {
			this.opcode = opcode;
			this.commandType = commandType;
		}
		public short Opcode { get { return this.opcode; } }
		public Type CommandType { get { return this.commandType; } }
	}
	#endregion
	#region DocCommandFactory
	public class DocCommandFactory : IDisposable {
		#region static
#if DEBUGTEST
		static HashSet<short> unknownTypecodes = new HashSet<short>();
#endif
		static List<DocCommandInfo> infos;
		static Dictionary<short, ConstructorInfo> commandTypes;
		static Dictionary<Type, short> commandOpcodes;
		static DocCommandFactory() {
			infos = new List<DocCommandInfo>();
			commandTypes = new Dictionary<short, ConstructorInfo>();
			commandOpcodes = new Dictionary<Type, short>();
			infos.Add(new DocCommandInfo(0x0000, typeof(DocCommandEmpty)));
			infos.Add(new DocCommandInfo(unchecked((short)0x083b), typeof(DocCommandAllCaps)));
			infos.Add(new DocCommandInfo(unchecked((short)0xca71), typeof(DocCommandBackColor)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2a0c), typeof(DocCommandBackColorIco)));
			infos.Add(new DocCommandInfo(unchecked((short)0x0835), typeof(DocCommandBold)));
			infos.Add(new DocCommandInfo(unchecked((short)0x0836), typeof(DocCommandItalic)));
			infos.Add(new DocCommandInfo(unchecked((short)0x4a4f), typeof(DocCommandFontName)));
			infos.Add(new DocCommandInfo(unchecked((short)0x4a50), typeof(DocCommandEastAsianFontName)));
			infos.Add(new DocCommandInfo(unchecked((short)0x4a51), typeof(DocCommandNonASCIIFontName)));
			infos.Add(new DocCommandInfo(unchecked((short)0x4a43), typeof(DocCommandFontSize)));
			infos.Add(new DocCommandInfo(unchecked((short)0xca49), typeof(DocCommandFontSizeNew)));
			infos.Add(new DocCommandInfo(unchecked((short)0x0837), typeof(DocCommandStrike)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2a53), typeof(DocCommandDoubleStrike)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2a3e), typeof(DocCommandUnderline)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6870), typeof(DocCommandForeColor)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2a42), typeof(DocCommandForeColorIco)));
			infos.Add(new DocCommandInfo(unchecked((short)0x083c), typeof(DocCommandHidden)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2a48), typeof(DocCommandScript)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6877), typeof(DocCommandUnderlineColor)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2403), typeof(DocCommandAlignment)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2461), typeof(DocCommandAlignmentNew)));
			infos.Add(new DocCommandInfo(unchecked((short)0x8411), typeof(DocCommandFirstLineIndent)));
			infos.Add(new DocCommandInfo(unchecked((short)0x8460), typeof(DocCommandFirstLineIndentNew)));
			infos.Add(new DocCommandInfo(unchecked((short)0x845e), typeof(DocCommandLogicalLeftIndent)));
			infos.Add(new DocCommandInfo(unchecked((short)0x840f), typeof(DocCommandPhysicalLeftIndent)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6412), typeof(DocCommandLineSpacing)));
			infos.Add(new DocCommandInfo(unchecked((short)0x845d), typeof(DocCommandLogicalRightIndent)));
			infos.Add(new DocCommandInfo(unchecked((short)0x840e), typeof(DocCommandPhysicalRightIndent)));
			infos.Add(new DocCommandInfo(unchecked((short)0xa414), typeof(DocCommandSpacingAfter)));
			infos.Add(new DocCommandInfo(unchecked((short)0xa413), typeof(DocCommandSpacingBefore)));
			infos.Add(new DocCommandInfo(unchecked((short)0x242a), typeof(DocCommandSuppressHyphenation)));
			infos.Add(new DocCommandInfo(unchecked((short)0x240c), typeof(DocCommandSuppressLineNumbers)));
			infos.Add(new DocCommandInfo(unchecked((short)0x246D), typeof(DocCommandContextualSpacing)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2407), typeof(DocCommandPageBreakBefore)));
			infos.Add(new DocCommandInfo(unchecked((short)0x245B), typeof(DocCommandBeforeAutoSpacing)));
			infos.Add(new DocCommandInfo(unchecked((short)0x245C), typeof(DocCommandAfterAutoSpacing)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2406), typeof(DocCommandKeepWithNext)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2405), typeof(DocCommandKeepLinesTogether)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2431), typeof(DocCommandWidowOrphanControl)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2640), typeof(DocCommandOutlineLevel)));
			infos.Add(new DocCommandInfo(unchecked((short)0xC64D), typeof(DocCommandParagraphShading)));
			infos.Add(new DocCommandInfo(unchecked((short)0x442D), typeof(DocCommandParagraphShading2))); 
			infos.Add(new DocCommandInfo(unchecked((short)0x6424), typeof(DocCommandParagraphTopBorder)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6425), typeof(DocCommandParagraphLeftBorder)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6426), typeof(DocCommandParagraphBottomBorder)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6427), typeof(DocCommandParagraphRightBorder)));
			infos.Add(new DocCommandInfo(unchecked((short)0xC64E), typeof(DocCommandParagraphTopBorderNew)));
			infos.Add(new DocCommandInfo(unchecked((short)0xC64F), typeof(DocCommandParagraphLeftBorderNew)));
			infos.Add(new DocCommandInfo(unchecked((short)0xC650), typeof(DocCommandParagraphBottomBorderNew)));
			infos.Add(new DocCommandInfo(unchecked((short)0xC651), typeof(DocCommandParagraphRightBorderNew)));
			infos.Add(new DocCommandInfo(unchecked((short)0xb021), typeof(DocCommandLeftMargin)));
			infos.Add(new DocCommandInfo(unchecked((short)0xb022), typeof(DocCommandRightMargin)));
			infos.Add(new DocCommandInfo(unchecked((short)0x9023), typeof(DocCommandTopMargin)));
			infos.Add(new DocCommandInfo(unchecked((short)0x9024), typeof(DocCommandBottomMargin)));
			infos.Add(new DocCommandInfo(unchecked((short)0xb025), typeof(DocCommandGutter)));
			infos.Add(new DocCommandInfo(unchecked((short)0xb017), typeof(DocCommandHeaderOffset)));
			infos.Add(new DocCommandInfo(unchecked((short)0xb018), typeof(DocCommandFooterOffset)));
			infos.Add(new DocCommandInfo(unchecked((short)0x322a), typeof(DocCommandRTLGutter)));
			infos.Add(new DocCommandInfo(unchecked((short)0x500b), typeof(DocCommandColumnCount)));
			infos.Add(new DocCommandInfo(unchecked((short)0xf203), typeof(DocCommandColumnWidth)));
			infos.Add(new DocCommandInfo(unchecked((short)0xf204), typeof(DocCommandNotEvenlyColumnsSpace)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3019), typeof(DocCommandDrawVerticalSeparator)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3005), typeof(DocCommandEqualWidthColumns)));
			infos.Add(new DocCommandInfo(unchecked((short)0x900c), typeof(DocCommandColumnSpace)));
			infos.Add(new DocCommandInfo(unchecked((short)0xb020), typeof(DocCommandPageHeight)));
			infos.Add(new DocCommandInfo(unchecked((short)0x301d), typeof(DocCommandPageOrientation)));
			infos.Add(new DocCommandInfo(unchecked((short)0xb01f), typeof(DocCommandPageWidth)));
			infos.Add(new DocCommandInfo(unchecked((short)0x300a), typeof(DocCommandDifferentFirstPage)));
			infos.Add(new DocCommandInfo(unchecked((short)0x5007), typeof(DocCommandFirstPagePaperSource)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3006), typeof(DocCommandOnlyAllowEditingOfFormFields)));
			infos.Add(new DocCommandInfo(unchecked((short)0x5008), typeof(DocCommandOtherPagePaperSource)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3009), typeof(DocCommandStartType)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3228), typeof(DocCommandTextDirection)));
			infos.Add(new DocCommandInfo(unchecked((short)0x301a), typeof(DocCommandVerticalTextAlignment)));
			infos.Add(new DocCommandInfo(unchecked((short)0x9016), typeof(DocCommandLineNumberingDistance)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3013), typeof(DocCommandNumberingRestartType)));
			infos.Add(new DocCommandInfo(unchecked((short)0x501b), typeof(DocCommandStartLineNumber)));
			infos.Add(new DocCommandInfo(unchecked((short)0x5015), typeof(DocCommandStep)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3001), typeof(DocCommandChapterHeaderStyle)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3000), typeof(DocCommandChapterSeparator)));
			infos.Add(new DocCommandInfo(unchecked((short)0x300e), typeof(DocCommandNumberingFormat)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3011), typeof(DocCommandUseStartingPageNumber)));
			infos.Add(new DocCommandInfo(unchecked((short)0x501c), typeof(DocCommandStartingPageNumber)));
			infos.Add(new DocCommandInfo(unchecked((short)0x7044), typeof(DocCommandStartingPageNumberNew)));
			infos.Add(new DocCommandInfo(unchecked((short)0x303b), typeof(DocCommandFootNotePosition)));
			infos.Add(new DocCommandInfo(unchecked((short)0x303c), typeof(DocCommandFootNoteNumberingRestartType)));
			infos.Add(new DocCommandInfo(unchecked((short)0x503f), typeof(DocCommandFootNoteStartingNumber)));
			infos.Add(new DocCommandInfo(unchecked((short)0x5040), typeof(DocCommandFootNoteNumberingFormat)));
			infos.Add(new DocCommandInfo(unchecked((short)0x303d), typeof(DocCommandEndNotePosition)));
			infos.Add(new DocCommandInfo(unchecked((short)0x303e), typeof(DocCommandEndNoteNumberingRestartType)));
			infos.Add(new DocCommandInfo(unchecked((short)0x5041), typeof(DocCommandEndNoteStartingNumber)));
			infos.Add(new DocCommandInfo(unchecked((short)0x5042), typeof(DocCommandEndNoteNumberingFormat)));
			infos.Add(new DocCommandInfo(unchecked((short)0x0800), typeof(DocCommandDeleted)));
			infos.Add(new DocCommandInfo(unchecked((short)0x4600), typeof(DocCommandChangeParagraphStyle)));
			infos.Add(new DocCommandInfo(unchecked((short)0x4a30), typeof(DocCommandChangeCharacterStyle)));
			infos.Add(new DocCommandInfo(unchecked((short)0x563a), typeof(DocCommandChangeTableStyle)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6a03), typeof(DocCommandPictureOffset)));
			infos.Add(new DocCommandInfo(unchecked((short)0xc60d), typeof(DocCommandChangeParagraphTabs)));
			infos.Add(new DocCommandInfo(unchecked((short)0xc615), typeof(DocCommandChangeParagraphTabsClose)));
			infos.Add(new DocCommandInfo(unchecked((short)0x646b), typeof(DocCommandReadTableProperties)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6645), typeof(DocCommandReadExtendedPropertyModifiers)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6646), typeof(DocCommandReadExtendedPropertyModifiersNew)));
			infos.Add(new DocCommandInfo(unchecked((short)0x460b), typeof(DocCommandListInfoIndex)));
			infos.Add(new DocCommandInfo(unchecked((short)0x260a), typeof(DocCommandListLevel)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6887), typeof(DocCommandPictureBulletCharacterPosition)));
			infos.Add(new DocCommandInfo(unchecked((short)0x4888), typeof(DocCommandPictureBulletProperties)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6a09), typeof(DocCommandSymbol)));
			infos.Add(new DocCommandInfo(unchecked((short)0x0855), typeof(DocCommandSpecial)));
			infos.Add(new DocCommandInfo(unchecked((short)0x0856), typeof(DocCommandEmbeddedObject)));
			infos.Add(new DocCommandInfo(unchecked((short)0x0806), typeof(DocCommandBinaryData)));
			infos.Add(new DocCommandInfo(unchecked((short)0x080a), typeof(DocCommandOle2Object)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2416), typeof(DocCommandInTable)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2417), typeof(DocCommandTableTrailer)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6649), typeof(DocCommandTableDepth)));
			infos.Add(new DocCommandInfo(unchecked((short)0x664a), typeof(DocCommandTableDepthIncrement)));
			infos.Add(new DocCommandInfo(unchecked((short)0x244b), typeof(DocCommandInnerTableCell)));
			infos.Add(new DocCommandInfo(unchecked((short)0x244c), typeof(DocCommandInnerTableTrailer)));
			infos.Add(new DocCommandInfo(unchecked((short)0x9407), typeof(DocCommandTableRowHeight)));
			infos.Add(new DocCommandInfo(unchecked((short)0xf614), typeof(DocCommandPreferredTableWidth)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd635), typeof(DocCommandPreferredTableCellWidth)));
			infos.Add(new DocCommandInfo(unchecked((short)0x5624), typeof(DocCommandMergeTableCells)));
			infos.Add(new DocCommandInfo(unchecked((short)0x5625), typeof(DocCommandSplitTableCells)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd62b), typeof(DocCommandVerticalMergeTableCells)));
			infos.Add(new DocCommandInfo(unchecked((short)0x7621), typeof(DocCommandInsertTableCell)));
			infos.Add(new DocCommandInfo(unchecked((short)0x7623), typeof(DocCommandTableCellWidth)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3615), typeof(DocCommandTableAutoFit)));
			infos.Add(new DocCommandInfo(unchecked((short)0x9601), typeof(DocCommandTableDxaLeft)));
			infos.Add(new DocCommandInfo(unchecked((short)0x9602), typeof(DocCommandTableDxaGapHalf)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd613), typeof(DocCommandTableBorders)));
			infos.Add(new DocCommandInfo(unchecked((short)0xf617), typeof(DocCommandWidthBefore)));
			infos.Add(new DocCommandInfo(unchecked((short)0xf618), typeof(DocCommandWidthAfter)));
			infos.Add(new DocCommandInfo(unchecked((short)0xf661), typeof(DocCommandWidthIndent)));
			infos.Add(new DocCommandInfo(unchecked((short)0x548a), typeof(DocCommandTableAlignment)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd633), typeof(DocCommandCellSpacing)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd632), typeof(DocCommandCellMargin)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd634), typeof(DocCommandCellMarginDefault)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd660), typeof(DocCommandTableBackgroundColor)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd670), typeof(DocCommandDefineTableShadings)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd671), typeof(DocCommandDefineTableShadings2nd)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd672), typeof(DocCommandDefineTableShadings3rd)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3465), typeof(DocCommandTableOverlap)));
			infos.Add(new DocCommandInfo(unchecked((short)0x940e), typeof(DocCommandTableHorizontalPosition)));
			infos.Add(new DocCommandInfo(unchecked((short)0x940f), typeof(DocCommandTableVerticalPosition)));
			infos.Add(new DocCommandInfo(unchecked((short)0x941f), typeof(DocCommandBottomFromText)));
			infos.Add(new DocCommandInfo(unchecked((short)0x9410), typeof(DocCommandLeftFromText)));
			infos.Add(new DocCommandInfo(unchecked((short)0x941e), typeof(DocCommandRightFromText)));
			infos.Add(new DocCommandInfo(unchecked((short)0x9411), typeof(DocCommandTopFromText)));
			infos.Add(new DocCommandInfo(unchecked((short)0x261b), typeof(DocCommandFramePosition)));
			infos.Add(new DocCommandInfo(unchecked((short)0x360d), typeof(DocCommandTablePosition)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3488), typeof(DocCommandTableStyleRowBandSize)));
			infos.Add(new DocCommandInfo(unchecked((short)0x3489), typeof(DocCommandTableStyleColBandSize)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd62f), typeof(DocCommandOverrideCellBorders)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd62c), typeof(DocCommandCellRangeVerticalAlignment)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd608), typeof(DocCommandTableDefinition)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd642), typeof(DocCommandHideCellMark)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd61a), typeof(DocCommandTopBorderColors)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd61b), typeof(DocCommandLeftBorderColors)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd61c), typeof(DocCommandBottomBorderColors)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd61d), typeof(DocCommandRightBorderColors)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd612), typeof(DocCommandTableShading)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd616), typeof(DocCommandTableShading2)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd60C), typeof(DocCommandTableShading3)));
			infos.Add(new DocCommandInfo(unchecked((short)0xd609), typeof(DocCommandDefTableShd80)));
			infos.Add(new DocCommandInfo(unchecked((short)0x740a), typeof(DocCommandTablePropertiesException)));
			infos.Add(new DocCommandInfo(unchecked((short)0x2423), typeof(DocCommandFrameWrapType)));
			infos.Add(new DocCommandInfo(unchecked((short)0x442b), typeof(DocCommandFrameHeight)));
			infos.Add(new DocCommandInfo(unchecked((short)0x841a), typeof(DocCommandFrameWidth)));
			infos.Add(new DocCommandInfo(unchecked((short)0x8418), typeof(DocCommandFrameHorizontalPosition)));
			infos.Add(new DocCommandInfo(unchecked((short)0x8419), typeof(DocCommandFrameVerticalPosition)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6467), typeof(DocCommandParagraphFormattingRsid)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6815), typeof(DocCommandCharacterFormattingRsid)));
			infos.Add(new DocCommandInfo(unchecked((short)0x6816), typeof(DocCommandInsertTextRsid)));
			for (int i = 0; i < infos.Count; i++) {
				commandTypes.Add(infos[i].Opcode, infos[i].CommandType.GetConstructor(Type.EmptyTypes));
				if (!commandOpcodes.ContainsKey(infos[i].CommandType))
					commandOpcodes.Add(infos[i].CommandType, infos[i].Opcode);
			}
		}
		public static short GetOpcodeByType(Type commandType) {
			short opcode;
			if (!commandOpcodes.TryGetValue(commandType, out opcode))
				opcode = 0x0000;
			return opcode;
		}
		#endregion
		DocumentModel model;
		public DocDocumentProperties DocumentProperties { get; set; }
		public int Version { get; set; }
		protected internal DocumentModelUnitConverter UnitConverter { get { return model.UnitConverter; } }
		public DocCommandFactory(DocumentModel model) {
			Guard.ArgumentNotNull(model, "model");
			this.model = model;
		}
		public IDocCommand CreateCommand(short opcode) {
			ConstructorInfo commandConstructor;
			if (commandTypes.TryGetValue(opcode, out commandConstructor))
				return (IDocCommand)commandConstructor.Invoke(new object[] { });
#if DEBUGTEST
			unknownTypecodes.Add(opcode);
#endif
			return new DocCommandEmpty();
		}
		public DocPropertyContainer CreatePropertyContainer(ChangeActionTypes changeType) {
			DocPropertyContainer result = new DocPropertyContainer(this, model.UnitConverter);
			UpdatePropertyContainer(result, changeType);
			return result;
		}
		public void UpdatePropertyContainer(DocPropertyContainer container, ChangeActionTypes changeType) {
			if (changeType == ChangeActionTypes.None)
				return;
			if (container.CharacterInfo == null && (changeType & ChangeActionTypes.Character) == ChangeActionTypes.Character)
				container.CharacterInfo = CharacterInfo.CreateDefault(model);
			if (container.ParagraphInfo == null && (changeType & ChangeActionTypes.Paragraph) == ChangeActionTypes.Paragraph)
				container.ParagraphInfo = ParagraphInfo.CreateDefault(model);
			if (container.FrameInfo == null && ((changeType & ChangeActionTypes.Frame) == ChangeActionTypes.Frame || (changeType & ChangeActionTypes.Table) == ChangeActionTypes.Table))
				container.FrameInfo = FrameInfo.CreateDefault(model);
			if (container.SectionInfo == null && (changeType & ChangeActionTypes.Section) == ChangeActionTypes.Section) {
				container.SectionInfo = SectionInfo.CreateDefault(model);
				if (DocumentProperties != null)
					ApplyDocumentProperties(container.SectionInfo);
			}
			if (container.TableCellInfo == null && (changeType & ChangeActionTypes.TableCell) == ChangeActionTypes.TableCell)
				container.TableCellInfo = TableCellInfo.CreateDefault(model);
			if (container.TableRowInfo == null && (changeType & ChangeActionTypes.TableRow) == ChangeActionTypes.TableRow)
				container.TableRowInfo = TableRowInfo.CreateDefault(model);
			if (container.TableInfo == null && (changeType & ChangeActionTypes.Table) == ChangeActionTypes.Table)
				container.TableInfo = TableInfo.CreateDefault(model);
		}
		void ApplyDocumentProperties(SectionInfo info) {
			info.FootNote.NumberingRestartType = DocumentProperties.FootNoteNumberingRestartType;
			info.FootNote.StartingNumber = DocumentProperties.FootNoteInitialNumber;
			info.FootNote.Position = DocumentProperties.FootNotePosition;
			info.EndNote.NumberingRestartType = DocumentProperties.EndNoteNumberingRestartType;
			info.EndNote.StartingNumber = DocumentProperties.EndnoteInitialNumber;
			info.EndNote.Position = DocumentProperties.EndNotePosition;
		}
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DocCommandFactory() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.model != null) {
					this.model.Dispose();
					this.model = null;
				}
			}
		}
		#endregion
	}
	#endregion
	#region IDocCommand
	public interface IDocCommand {
		ChangeActionTypes ChangeAction { get; }
		void Read(byte[] sprm);
		void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader);
		void Write(BinaryWriter writer);
	}
	#endregion
	#region DocCommandEmpty
	class DocCommandEmpty : IDocCommand {
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.None; } }
		public void Read(byte[] sprm) { }
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) { }
		public void Write(BinaryWriter writer) { }
		#endregion
	}
	#endregion
	#region DocCommandColorBase (abstract class)
	public abstract class DocCommandColorBase : IDocCommand {
		#region Fields
		DocColorReference colorReference;
		#endregion
		protected DocCommandColorBase() {
			this.colorReference = new DocColorReference();
		}
		#region Properties
		public Color Color { get { return colorReference.Color; } set { colorReference.Color = value; } }
		#endregion
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public void Read(byte[] sprm) {
			this.colorReference = DocColorReference.FromByteArray(sprm, 0);
		}
		public abstract void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader);
		public virtual void Write(BinaryWriter writer) {
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), this.colorReference.GetBytes());
			writer.Write(sprm);
		}
		#endregion
	}
	#endregion
	#region DocCommandIcoPropertyValueBase
	public abstract class DocCommandIcoPropertyValueBase : IDocCommand {
		public Color Color { get; set; }
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public void Read(byte[] sprm) {
			byte colorIndex = sprm[0];
			if (colorIndex < DocConstants.DefaultMSWordColor.Length)
				Color = DocConstants.DefaultMSWordColor[colorIndex];
			else
				Color = DXColor.Empty;
		}
		public abstract void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader);
		public void Write(BinaryWriter writer) {
			byte colorIndex = (byte)Array.BinarySearch(DocConstants.DefaultMSWordColor, Color);
			if (colorIndex < 0)
				colorIndex = 0;
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), new byte[] { colorIndex });
			writer.Write(sprm);
		}
		#endregion
	}
	#endregion
	public class DocCommandTableShading : DocCommandShadingListBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			if (propertyContainer.Factory.Version <= 0x00D9)
				propertyContainer.TableRowInfo.CellShading1.AddRange(CellColors);
		}
	}
	public class DocCommandTableShading2 : DocCommandShadingListBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			if (propertyContainer.Factory.Version <= 0x00D9)
				propertyContainer.TableRowInfo.CellShading2.AddRange(CellColors);
		}
	}
	public class DocCommandTableShading3 : DocCommandShadingListBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			if (propertyContainer.Factory.Version <= 0x00D9)
				propertyContainer.TableRowInfo.CellShading3.AddRange(CellColors);
		}
	}
	public class DocCommandDefTableShd80 : IDocCommand {
		List<Color> colors;
		public DocCommandDefTableShd80() {
			this.colors = new List<Color>();
		}
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			if (propertyContainer.Factory.Version <= 0x00D9) {
				propertyContainer.TableRowInfo.DefaultCellsShading.AddRange(colors);
			}
		}
		public void Read(byte[] sprm) {
			int count = sprm.Length / 2;
			for (int i = 0; i < count; i++) {
				DocShadingDescriptor80 descriptor = DocShadingDescriptor80.FromByteArray(sprm, i * 2);
				Color color = GetColor(descriptor);
				colors.Add(color);
			}
		}
		Color GetColor(DocShadingDescriptor80 descriptor) {
			if (descriptor.ShadingPattern == 0)
				return descriptor.BackgroundColor;
			else
				return descriptor.ForeColor;
		}
		public void Write(BinaryWriter writer) {
			throw new NotImplementedException();
		}
	}
	#region DocCommandShadingPropertyValueBase (abstract class)
	public abstract class DocCommandShadingPropertyValueBase : IDocCommand {
		protected DocCommandShadingPropertyValueBase() {
			ShadingDescriptor = new DocShadingDescriptor();
		}
		public DocShadingDescriptor ShadingDescriptor { get; set; }
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public void Read(byte[] sprm) {
			ShadingDescriptor = DocShadingDescriptor.FromByteArray(sprm, 0);
		}
		public abstract void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader);
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
			writer.Write(DocShadingDescriptor.Size);
			ShadingDescriptor.Write(writer);
		}
		#endregion
	}
	#endregion
	#region DocCommandShading80PropertyValueBase
	public abstract class DocCommandShading80PropertyValueBase : IDocCommand {
		protected DocCommandShading80PropertyValueBase() {
			ShadingDescriptor = new DocShadingDescriptor80();
		}
		public DocShadingDescriptor80 ShadingDescriptor { get; set; }
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public void Read(byte[] sprm) {
			ShadingDescriptor = DocShadingDescriptor80.FromByteArray(sprm);
		}
		public abstract void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader);
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
			ShadingDescriptor.Write(writer);
		}
		#endregion
	}
	#endregion
	#region DocCommandShadingListBase (abstract class)
	public abstract class DocCommandShadingListBase : IDocCommand {
		#region Fields
		readonly List<Color> cellColors;
		#endregion
		protected DocCommandShadingListBase() {
			this.cellColors = new List<Color>();
		}
		#region Properties
		public List<Color> CellColors { get { return this.cellColors; } }
		#endregion
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public void Read(byte[] sprm) {
			int count = sprm.Length / DocShadingDescriptor.Size;
			for (int i = 0; i < count; i++) {
				DocShadingDescriptor shadingDescriptor = DocShadingDescriptor.FromByteArray(sprm, i * DocShadingDescriptor.Size);
				CellColors.Add(shadingDescriptor.BackgroundColor);
			}
		}
		public abstract void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader);
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
			int count = this.cellColors.Count;
			writer.Write((byte)(count * DocShadingDescriptor.Size));
			for (int i = 0; i < count; i++) {
				DocShadingDescriptor descriptor = new DocShadingDescriptor();
				descriptor.BackgroundColor = this.cellColors[i];
				descriptor.Write(writer);
			}
		}
		#endregion
	}
	#endregion
	#region DocCommandBytePropertyValueBase
	public abstract class DocCommandBytePropertyValueBase : IDocCommand {
		public byte Value { get; set; }
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public void Read(byte[] sprm) {
			Value = sprm[0];
		}
		public abstract void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader);
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), new byte[] { GetValue() }));
		}
		protected virtual byte GetValue() {
			return Value;
		}
		#endregion
	}
	#endregion
	#region DocCommandShortPropertyValueBase
	public abstract class DocCommandShortPropertyValueBase : IDocCommand {
		public int Value { get; set; }
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public void Read(byte[] sprm) {
			Value = BitConverter.ToInt16(sprm, 0);
		}
		public abstract void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader);
		public virtual void Write(BinaryWriter writer) {
			writer.Write(DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), BitConverter.GetBytes((ushort)Value)));
		}
		#endregion
	}
	#endregion
	#region DocCommandXASPropertyValueBase
	public abstract class DocCommandXASPropertyValueBase : DocCommandShortPropertyValueBase {
		public override void Write(BinaryWriter writer) {
			Value = Math.Min(Value, DocConstants.MaxXASValue);
			base.Write(writer);
		}
	}
	#endregion
	#region DocCommandIntPropertyValueBase
	public abstract class DocCommandIntPropertyValueBase : IDocCommand {
		public int Value { get; set; }
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public void Read(byte[] sprm) {
			Value = BitConverter.ToInt32(sprm, 0);
		}
		public abstract void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader);
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), BitConverter.GetBytes(Value)));
		}
		#endregion
	}
	#endregion
	#region DocCommandBoolWrapperPropertyValueBase
	public abstract class DocCommandBoolWrapperPropertyValueBase : IDocCommand {
		DocBoolWrapper value;
		public bool Value {
			get {
				return this.value == DocBoolWrapper.True;
			}
			set {
				if (value)
					this.value = DocBoolWrapper.True;
				else
					this.value = DocBoolWrapper.False;
			}
		}
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public void Read(byte[] sprm) {
			value = (DocBoolWrapper)sprm[0];
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			switch (this.value) {
				case DocBoolWrapper.False:
					SetValueFalse(propertyContainer);
					SetUseValueTrue(propertyContainer);
					break;
				case DocBoolWrapper.True:
					SetValueTrue(propertyContainer);
					SetUseValueTrue(propertyContainer);
					break;
				case DocBoolWrapper.Leave:
					LeaveValue(propertyContainer);
					SetUseValueFalse(propertyContainer);
					break;
				case DocBoolWrapper.Inverse:
					InverseValue(propertyContainer);
					SetUseValueTrue(propertyContainer);
					break;
			}
		}
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), new byte[] { (byte)this.value }));
		}
		protected abstract void SetValueTrue(DocPropertyContainer propertyContainer);
		protected abstract void SetValueFalse(DocPropertyContainer propertyContainer);
		protected abstract void SetUseValueTrue(DocPropertyContainer propertyContainer);
		protected abstract void SetUseValueFalse(DocPropertyContainer propertyContainer);
		protected abstract void LeaveValue(DocPropertyContainer propertyContainer);
		protected abstract void InverseValue(DocPropertyContainer propertyContainer);
		#endregion
	}
	#endregion
	#region DocCommandBoolPropertyValueBase
	public abstract class DocCommandBoolPropertyValueBase : IDocCommand {
		public bool Value { get; set; }
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public void Read(byte[] sprm) {
			Value = sprm[0] == 0x01;
		}
		public abstract void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader);
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), new byte[] { Convert.ToByte(Value) }));
		}
		#endregion
	}
	#endregion
	#region DocCommandWidthUnitPropertyValueBase (abstract class)
	public abstract class DocCommandWidthUnitPropertyValueBase : IDocCommand {
		#region Fields
		WidthUnitOperand widthUnit;
		#endregion
		protected DocCommandWidthUnitPropertyValueBase() {
			this.widthUnit = new WidthUnitOperand();
		}
		#region Properties
		public WidthUnitOperand WidthUnit { get { return widthUnit; } }
		#endregion
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public void Read(byte[] sprm) {
			this.widthUnit = WidthUnitOperand.FromByteArray(sprm, 0);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			if (WidthUnit.Type == WidthUnitType.ModelUnits)
				WidthUnit.Value = (short)propertyContainer.UnitConverter.TwipsToModelUnits(WidthUnit.Value);
			ApplyWidthOperand(propertyContainer);
		}
		public void Write(BinaryWriter writer) {
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), WidthUnit.GetBytes());
			writer.Write(sprm);
		}
		#endregion
		protected abstract void ApplyWidthOperand(DocPropertyContainer propertyContainer);
	}
	#endregion
	#region DocCommandCellSpacingPropertyValueBase (abstract class)
	public abstract class DocCommandCellSpacingPropertyValueBase : IDocCommand {
		#region Fields
		CellSpacingOperand cellSpacing;
		#endregion
		protected DocCommandCellSpacingPropertyValueBase() {
			this.cellSpacing = new CellSpacingOperand();
		}
		#region Properties
		public CellSpacingOperand CellSpacing { get { return cellSpacing; } }
		#endregion
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public void Read(byte[] sprm) {
			this.cellSpacing = CellSpacingOperand.FromByteArray(sprm);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			ApplyCellSpacingOperand(propertyContainer);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
			writer.Write(CellSpacingOperand.Size);
			this.cellSpacing.Write(writer);
		}
		#endregion
		protected abstract void ApplyCellSpacingOperand(DocPropertyContainer propertyContainer);
	}
	#endregion
	#region DocCommandMergeTableCellsBase (abstract class)
	public abstract class DocCommandMergeTableCellsBase : IDocCommand {
		#region Fields
		byte firstMergedIndex;
		byte lastMergedIndex;
		#endregion
		#region IDocCommand Members
		public abstract ChangeActionTypes ChangeAction { get; }
		public byte FirstMergedIndex { get { return firstMergedIndex; } }
		public byte LastMergedIndex { get { return lastMergedIndex; } }
		public void Read(byte[] sprm) {
			this.firstMergedIndex = sprm[0];
			this.lastMergedIndex = sprm[1];
		}
		public abstract void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader);
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), new byte[] { this.firstMergedIndex, this.lastMergedIndex }));
		}
		#endregion
	}
	#endregion
	#region DocCommandsDeleted
	public class DocCommandDeleted : DocCommandBoolWrapperPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.None; } }
		protected override void SetValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.IsDeleted = true;
		}
		protected override void SetValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.IsDeleted = false;
		}
		protected override void LeaveValue(DocPropertyContainer propertyContainer) {
		}
		protected override void InverseValue(DocPropertyContainer propertyContainer) {
			propertyContainer.IsDeleted = !propertyContainer.IsDeleted;
		}
		protected override void SetUseValueTrue(DocPropertyContainer propertyContainer) {
		}
		protected override void SetUseValueFalse(DocPropertyContainer propertyContainer) {
		}
	}
	#endregion
	#region DocCommandReadTableProperties
	public class DocCommandReadTableProperties : DocCommandIntPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.None; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			dataStreamReader.BaseStream.Seek(Value, SeekOrigin.Begin);
			int size = dataStreamReader.ReadUInt16();
			byte[] grpprl = dataStreamReader.ReadBytes(size);
			DocCommandHelper.Traverse(grpprl, propertyContainer, dataStreamReader);
		}
	}
	#endregion
	#region DocCommandReadExtendedPropertyModifier
	public class DocCommandReadExtendedPropertyModifiers : DocCommandReadTableProperties {
	}
	#endregion
	#region DocCommandReadExtendedPropertyModifiersNew
	public class DocCommandReadExtendedPropertyModifiersNew : DocCommandReadTableProperties {
	}
	#endregion
	#region DocCommandAllCaps
	public class DocCommandAllCaps : DocCommandBoolWrapperPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		protected override void SetValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.AllCaps = DocBoolWrapper.True;
		}
		protected override void SetValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.AllCaps = DocBoolWrapper.False;
		}
		protected override void LeaveValue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.AllCaps = DocBoolWrapper.Leave;
		}
		protected override void InverseValue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.AllCaps = DocBoolWrapper.Inverse;
		}
		protected override void SetUseValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingOptions.UseAllCaps = true;
		}
		protected override void SetUseValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingOptions.UseAllCaps = false;
		}
	}
	#endregion
	#region DocCommandBackColor
	public class DocCommandBackColor : DocCommandShadingPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.FormattingInfo.BackColor = this.ShadingDescriptor.BackgroundColor;
			propertyContainer.CharacterInfo.FormattingOptions.UseBackColor = true;
		}
	}
	#endregion
	#region DocCommandBackColorIco
	public class DocCommandBackColorIco : DocCommandIcoPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.FormattingInfo.BackColor = Color;
			propertyContainer.CharacterInfo.FormattingOptions.UseBackColor = true;
		}
	}
	#endregion
	#region DocCommandBold
	public class DocCommandBold : DocCommandBoolWrapperPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		protected override void SetValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.FontBold = DocBoolWrapper.True;
		}
		protected override void SetValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.FontBold = DocBoolWrapper.False;
		}
		protected override void LeaveValue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.FontBold = DocBoolWrapper.Leave;
		}
		protected override void InverseValue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.FontBold = DocBoolWrapper.Inverse;
		}
		protected override void SetUseValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingOptions.UseFontBold = true;
		}
		protected override void SetUseValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingOptions.UseFontBold = false;
		}
	}
	#endregion
	#region DocCommandItalic
	public class DocCommandItalic : DocCommandBoolWrapperPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		protected override void SetValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.FontItalic = DocBoolWrapper.True;
		}
		protected override void SetValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.FontItalic = DocBoolWrapper.False;
		}
		protected override void LeaveValue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.FontItalic = DocBoolWrapper.Leave;
		}
		protected override void InverseValue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.FontItalic = DocBoolWrapper.Inverse;
		}
		protected override void SetUseValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingOptions.UseFontItalic = true;
		}
		protected override void SetUseValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingOptions.UseFontItalic = false;
		}
	}
	#endregion
	#region DocCommandFontName
	public class DocCommandFontName : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.FontFamilyNameIndex = (short)Value;
			propertyContainer.CharacterInfo.FormattingOptions.UseFontName = true;
		}
	}
	#endregion
	#region DocCommandEastAsianFontName
	public class DocCommandEastAsianFontName : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.None; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
		}
	}
	#endregion
	#region DocCommandNonASCIIFontName
	public class DocCommandNonASCIIFontName : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.None; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
		}
	}
	#endregion
	#region DocCommandFontSize
	public class DocCommandFontSize : IDocCommand {
		public int DoubleFontSize { get; set; }
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public void Read(byte[] sprm) {
			DoubleFontSize = BitConverter.ToInt16(sprm, 0);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.FormattingInfo.DoubleFontSize = Math.Max(DoubleFontSize, 1);
			propertyContainer.CharacterInfo.FormattingOptions.UseDoubleFontSize = true;
		}
		public void Write(BinaryWriter writer) {
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), BitConverter.GetBytes((short)(DoubleFontSize)));
			writer.Write(sprm);
		}
		#endregion
	}
	#endregion
	#region DocCommandFontSizeNew
	public class DocCommandFontSizeNew : DocCommandFontSize {
	}
	#endregion
	#region DocCommandStrike
	public class DocCommandStrike : DocCommandBoolWrapperPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		protected override void SetValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.Strike = DocBoolWrapper.True;
		}
		protected override void SetValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.Strike = DocBoolWrapper.False;
		}
		protected override void LeaveValue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.Strike = DocBoolWrapper.Leave;
		}
		protected override void InverseValue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.Strike = DocBoolWrapper.Inverse;
		}
		protected override void SetUseValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingOptions.UseFontStrikeoutType = true;
		}
		protected override void SetUseValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingOptions.UseFontStrikeoutType = false;
		}
	}
	#endregion
	#region DocCommandDoubleStrike
	public class DocCommandDoubleStrike : DocCommandBoolWrapperPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		protected override void SetValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.DoubleStrike = DocBoolWrapper.True;
		}
		protected override void SetValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.DoubleStrike = DocBoolWrapper.False;
		}
		protected override void LeaveValue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.DoubleStrike = DocBoolWrapper.Leave;
		}
		protected override void InverseValue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.DoubleStrike = DocBoolWrapper.Inverse;
		}
		protected override void SetUseValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingOptions.UseFontStrikeoutType = true;
		}
		protected override void SetUseValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingOptions.UseFontStrikeoutType = false;
		}
	}
	#endregion
	#region DocCommandUnderline
	public class DocCommandUnderline : IDocCommand {
		#region UnderlineInfo
		class UnderlineInfo {
			byte typeCode;
			UnderlineType underlineType;
			public UnderlineInfo(byte typeCode, UnderlineType underlineType) {
				this.typeCode = typeCode;
				this.underlineType = underlineType;
			}
			public byte TypeCode { get { return this.typeCode; } }
			public UnderlineType UnderlineType { get { return this.underlineType; } }
		}
		#endregion
		#region static
		static DocCommandUnderline() {
			infos = new List<UnderlineInfo>(17);
			underlineTypes = new Dictionary<byte, UnderlineType>(17);
			underlineTypeCodes = new Dictionary<UnderlineType, byte>(17);
			infos.Add(new UnderlineInfo(0, UnderlineType.None));
			infos.Add(new UnderlineInfo(1, UnderlineType.Single));
			infos.Add(new UnderlineInfo(3, UnderlineType.Double));
			infos.Add(new UnderlineInfo(4, UnderlineType.Dotted));
			infos.Add(new UnderlineInfo(6, UnderlineType.ThickSingle));
			infos.Add(new UnderlineInfo(7, UnderlineType.Dashed));
			infos.Add(new UnderlineInfo(9, UnderlineType.DashDotted));
			infos.Add(new UnderlineInfo(10, UnderlineType.DashDotDotted));
			infos.Add(new UnderlineInfo(11, UnderlineType.Wave));
			infos.Add(new UnderlineInfo(20, UnderlineType.ThickDotted));
			infos.Add(new UnderlineInfo(23, UnderlineType.ThickDashed));
			infos.Add(new UnderlineInfo(25, UnderlineType.ThickDashDotted));
			infos.Add(new UnderlineInfo(26, UnderlineType.ThickDashDotDotted));
			infos.Add(new UnderlineInfo(27, UnderlineType.HeavyWave));
			infos.Add(new UnderlineInfo(39, UnderlineType.LongDashed));
			infos.Add(new UnderlineInfo(43, UnderlineType.DoubleWave));
			infos.Add(new UnderlineInfo(55, UnderlineType.ThickLongDashed));
			for (int i = 0; i < infos.Count; i++) {
				underlineTypes.Add(infos[i].TypeCode, infos[i].UnderlineType);
				underlineTypeCodes.Add(infos[i].UnderlineType, infos[i].TypeCode);
			}
		}
		#endregion
		#region Fields
		byte underlineCode;
		bool underlineWordsOnly;
		UnderlineType fontUnderlineType;
		static List<UnderlineInfo> infos;
		static Dictionary<byte, UnderlineType> underlineTypes;
		static Dictionary<UnderlineType, byte> underlineTypeCodes;
		#endregion
		#region Properties
		public UnderlineType FontUnderlineType {
			get { return this.fontUnderlineType; }
			set {
				if (this.fontUnderlineType == value)
					return;
				this.fontUnderlineType = value;
				CalcUnderlineCode();
			}
		}
		public bool UnderlyneWordsOnly {
			get { return this.underlineWordsOnly; }
			set {
				if (this.underlineWordsOnly == value)
					return;
				this.underlineWordsOnly = value;
				CalcUnderlineCode();
			}
		}
		#endregion
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public void Read(byte[] sprm) {
			this.underlineCode = sprm[0];
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			UnderlineType underlineType;
			if (this.underlineCode == 2) {
				propertyContainer.CharacterInfo.FormattingInfo.UnderlineWordsOnly = true;
				propertyContainer.CharacterInfo.FormattingInfo.FontUnderlineType = UnderlineType.Single;
				propertyContainer.CharacterInfo.FormattingOptions.UseUnderlineWordsOnly = true;
			}
			else if (underlineTypes.TryGetValue(this.underlineCode, out underlineType)) {
				propertyContainer.CharacterInfo.FormattingInfo.UnderlineWordsOnly = this.underlineWordsOnly;
				propertyContainer.CharacterInfo.FormattingInfo.FontUnderlineType = underlineType;
			}
			propertyContainer.CharacterInfo.FormattingOptions.UseFontUnderlineType = true;
		}
		public void Write(BinaryWriter writer) {
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), new byte[] { this.underlineCode });
			writer.Write(sprm);
		}
		#endregion
		void CalcUnderlineCode() {
			if (this.underlineWordsOnly && fontUnderlineType == UnderlineType.Single) {
				this.underlineCode = 2;
			}
			else {
				byte underlineCode;
				underlineTypeCodes.TryGetValue(this.fontUnderlineType, out underlineCode);
				this.underlineCode = underlineCode;
			}
		}
	}
	#endregion
	#region DocCommandForeColor
	public class DocCommandForeColor : DocCommandColorBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.FormattingInfo.ForeColor = Color;
			propertyContainer.CharacterInfo.FormattingOptions.UseForeColor = true;
		}
	}
	#endregion
	#region DocCommandForeColorIco
	public class DocCommandForeColorIco : DocCommandIcoPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.FormattingInfo.ForeColor = Color;
			propertyContainer.CharacterInfo.FormattingOptions.UseForeColor = true;
		}
	}
	#endregion
	#region DocCommandHidden
	public class DocCommandHidden : DocCommandBoolWrapperPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		protected override void SetValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.Hidden = DocBoolWrapper.True;
		}
		protected override void SetValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.Hidden = DocBoolWrapper.False;
		}
		protected override void LeaveValue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.Hidden = DocBoolWrapper.Leave;
		}
		protected override void InverseValue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingInfo.Hidden = DocBoolWrapper.Inverse;
		}
		protected override void SetUseValueTrue(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingOptions.UseHidden = true;
		}
		protected override void SetUseValueFalse(DocPropertyContainer propertyContainer) {
			propertyContainer.CharacterInfo.FormattingOptions.UseHidden = false;
		}
	}
	#endregion
	#region DocCommandScript
	public class DocCommandScript : IDocCommand {
		#region static
		static DocCommandScript() {
			scriptCodes = new Dictionary<CharacterFormattingScript, byte>(3);
			scriptCodes.Add(CharacterFormattingScript.Normal, 0);
			scriptCodes.Add(CharacterFormattingScript.Superscript, 1);
			scriptCodes.Add(CharacterFormattingScript.Subscript, 2);
		}
		#endregion
		#region Fields
		byte scriptType;
		static Dictionary<CharacterFormattingScript, byte> scriptCodes;
		#endregion
		#region Properties
		public CharacterFormattingScript Script {
			get { return CalcFormattingScript(); }
			set { scriptType = scriptCodes[value]; }
		}
		#endregion
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public void Read(byte[] sprm) {
			this.scriptType = sprm[0];
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.FormattingInfo.Script = Script;
			propertyContainer.CharacterInfo.FormattingOptions.UseScript = true;
		}
		public void Write(BinaryWriter writer) {
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), new byte[] { this.scriptType });
			writer.Write(sprm);
		}
		CharacterFormattingScript CalcFormattingScript() {
			if (this.scriptType == 1)
				return CharacterFormattingScript.Superscript;
			if (this.scriptType == 2)
				return CharacterFormattingScript.Subscript;
			return CharacterFormattingScript.Normal;
		}
		#endregion
	}
	#endregion
	#region DocCommandUnderlineColor
	public class DocCommandUnderlineColor : DocCommandColorBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.FormattingInfo.UnderlineColor = Color;
			propertyContainer.CharacterInfo.FormattingOptions.UseUnderlineColor = true;
		}
	}
	#endregion
	#region DocCommandAlignment
	public class DocCommandAlignment : DocCommandBytePropertyValueBase {
		#region Properties
		public ParagraphAlignment Alignment {
			get { return AlignmentCalculator.CalcParagraphAlignment(Value); }
			set { Value = AlignmentCalculator.CalcParagraphAlignmentCode(value); }
		}
		#endregion
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.Alignment = Alignment;
			propertyContainer.ParagraphInfo.FormattingOptions.UseAlignment = true;
		}
	}
	#endregion
	#region DocCommandAlignmentNew
	public class DocCommandAlignmentNew : DocCommandAlignment {
	}
	#endregion
	#region DocCommandFirstLineIndent
	public class DocCommandFirstLineIndent : DocCommandXASPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			ParagraphFormattingInfo info = propertyContainer.ParagraphInfo.FormattingInfo;
			info.FirstLineIndent = Math.Abs(Value);
			if (Value == 0)
				info.FirstLineIndentType = ParagraphFirstLineIndent.None;
			else
				info.FirstLineIndentType = (Value > 0) ? ParagraphFirstLineIndent.Indented : ParagraphFirstLineIndent.Hanging;
			propertyContainer.ParagraphInfo.FormattingOptions.UseFirstLineIndent = true;
		}
	}
	#endregion
	#region DocCommandFirstLineIndentNew
	public class DocCommandFirstLineIndentNew : DocCommandFirstLineIndent {
	}
	#endregion
	#region DocCommandLogicalLeftIndent
	public class DocCommandLogicalLeftIndent : DocCommandXASPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.LeftIndent = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UseLeftIndent = true;
		}
	}
	#endregion
	#region DocCommandPhysicalLeftIndent
	public class DocCommandPhysicalLeftIndent : DocCommandLogicalLeftIndent {
	}
	#endregion
	#region DocCommandLineSpacing
	public class DocCommandLineSpacing : IDocCommand {
		#region Fields
		short docLineSpacing;
		short docLineSpacingType;
		#endregion
		#region Properties
		public short LineSpacing {
			get { return this.docLineSpacing; }
			set {
				if (this.docLineSpacing == value)
					return;
				this.docLineSpacing = value;
			}
		}
		public short LineSpacingType {
			get { return this.docLineSpacingType; }
			set {
				if (this.docLineSpacingType == value)
					return;
				this.docLineSpacingType = value;
			}
		}
		#endregion
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public void Read(byte[] sprm) {
			this.docLineSpacing = BitConverter.ToInt16(sprm, 0);
			this.docLineSpacingType = BitConverter.ToInt16(sprm, 2);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.LineSpacingType = CalcLineSpacingType();
			propertyContainer.ParagraphInfo.FormattingInfo.LineSpacing = CalcLineSpacing(propertyContainer.UnitConverter);
			propertyContainer.ParagraphInfo.FormattingOptions.UseLineSpacing = true;
		}
		public void Write(BinaryWriter writer) {
			byte[] operand = new byte[4];
			Array.Copy(BitConverter.GetBytes(this.docLineSpacing), 0, operand, 0, 2);
			Array.Copy(BitConverter.GetBytes(this.docLineSpacingType), 0, operand, 2, 2);
			writer.Write(DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), operand));
		}
		#endregion
		ParagraphLineSpacing CalcLineSpacingType() {
			if (docLineSpacingType == 0)
				return (docLineSpacing >= 0) ? ParagraphLineSpacing.AtLeast : ParagraphLineSpacing.Exactly;
			else if (docLineSpacing == 240)
				return ParagraphLineSpacing.Single;
			else if (docLineSpacing == 360)
				return ParagraphLineSpacing.Sesquialteral;
			else if (docLineSpacing == 480)
				return ParagraphLineSpacing.Double;
			else
				return ParagraphLineSpacing.Multiple;
		}
		float CalcLineSpacing(DocumentModelUnitConverter unitConverter) {
			if (docLineSpacingType == 0)
				return unitConverter.TwipsToModelUnits(Math.Abs(docLineSpacing));
			else if (docLineSpacing == 240)
				return 1.0f;
			else if (docLineSpacing == 360)
				return 1.5f;
			else if (docLineSpacing == 480)
				return 2.0f;
			else
				return Math.Abs(docLineSpacing / 240.0f);
		}
	}
	#endregion
	#region DocCommandLogicalRightIndent
	public class DocCommandLogicalRightIndent : DocCommandXASPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.RightIndent = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UseRightIndent = true;
		}
	}
	#endregion
	#region DocCommandPhysicalRightIndent
	public class DocCommandPhysicalRightIndent : DocCommandLogicalRightIndent {
	}
	#endregion
	#region DocCommandSpacingAfter
	public class DocCommandSpacingAfter : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.SpacingAfter = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UseSpacingAfter = true;
		}
	}
	#endregion
	#region DocCommandSpacingBefore
	public class DocCommandSpacingBefore : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.SpacingBefore = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UseSpacingBefore = true;
		}
	}
	#endregion
	#region DocCommandSuppressHyphenation
	public class DocCommandSuppressHyphenation : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.SuppressHyphenation = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UseSuppressHyphenation = true;
		}
	}
	#endregion
	#region DocCommandSuppressLineNumbers
	public class DocCommandSuppressLineNumbers : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.SuppressLineNumbers = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UseSuppressLineNumbers = true;
		}
	}
	#endregion
	#region DocCommandContextualSpacing
	public class DocCommandContextualSpacing : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.ContextualSpacing = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UseContextualSpacing = true;
		}
	}
	#endregion
	#region DocCommandPageBreakBefore
	public class DocCommandPageBreakBefore : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.PageBreakBefore = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UsePageBreakBefore = true;
		}
	}
	#endregion
	#region DocCommandBeforeAutoSpacing
	public class DocCommandBeforeAutoSpacing : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.BeforeAutoSpacing = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UseBeforeAutoSpacing = true;
		}
	}
	#endregion
	#region DocCommandAfterAutoSpacing
	public class DocCommandAfterAutoSpacing : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.AfterAutoSpacing = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UseAfterAutoSpacing = true;
		}
	}
	#endregion
	#region DocCommandKeepWithNext
	public class DocCommandKeepWithNext : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.KeepWithNext = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UseKeepWithNext = true;
		}
	}
	#endregion
	#region DocCommandKeepLinesTogether
	public class DocCommandKeepLinesTogether : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.KeepLinesTogether = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UseKeepLinesTogether = true;
		}
	}
	#endregion
	#region DocCommandWidowOrphanControl
	public class DocCommandWidowOrphanControl : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.WidowOrphanControl = Value;
			propertyContainer.ParagraphInfo.FormattingOptions.UseWidowOrphanControl = true;
		}
	}
	#endregion
	#region DocCommandOutlineLevel
	public class DocCommandOutlineLevel : DocCommandBytePropertyValueBase {
		const byte docBodyTextLevel = 9;
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			int actualValue = (Value < docBodyTextLevel) ? Value + 1 : 0;
			propertyContainer.ParagraphInfo.FormattingInfo.OutlineLevel = actualValue;
			propertyContainer.ParagraphInfo.FormattingOptions.UseOutlineLevel = true;
		}
		protected override byte GetValue() {
			if (Value == 0)
				return docBodyTextLevel;
			return (byte)(Value - 1);
		}
	}
	#endregion
	#region DocCommandParagraphShading
	public class DocCommandParagraphShading : DocCommandShadingPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.FormattingInfo.BackColor = ShadingDescriptor.BackgroundColor;
			propertyContainer.ParagraphInfo.FormattingOptions.UseBackColor = true;
		}
	}
	#endregion
	#region DocCommandParagraphShading2
	public class DocCommandParagraphShading2 : DocCommandShading80PropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			if (propertyContainer.ParagraphInfo.FormattingOptions.UseBackColor)
				return;
			if (ShadingDescriptor.BackgroundColor == DXColor.Empty)
				propertyContainer.ParagraphInfo.FormattingInfo.BackColor = ShadingDescriptor.ShadingPattern == 0 ? DXColor.Empty : DXColor.Black;
			else
				propertyContainer.ParagraphInfo.FormattingInfo.BackColor = ShadingDescriptor.BackgroundColor;
			propertyContainer.ParagraphInfo.FormattingOptions.UseBackColor = true;
		}
	}
	#endregion
	#region DocCommandLeftMargin
	public class DocCommandLeftMargin : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionMargins.Left = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
			propertyContainer.SectionInfo.SectionPage.PaperKind = PaperKind.Custom;
		}
	}
	#endregion
	#region DocCommandRightMargin
	public class DocCommandRightMargin : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionMargins.Right = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
			propertyContainer.SectionInfo.SectionPage.PaperKind = PaperKind.Custom;
		}
	}
	#endregion
	#region DocCommandTopMargin
	public class DocCommandTopMargin : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionMargins.Top = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
			propertyContainer.SectionInfo.SectionPage.PaperKind = PaperKind.Custom;
		}
	}
	#endregion
	#region DocCommandBottomMargin
	public class DocCommandBottomMargin : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionMargins.Bottom = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
			propertyContainer.SectionInfo.SectionPage.PaperKind = PaperKind.Custom;
		}
	}
	#endregion
	#region DocCommandGutter
	public class DocCommandGutter : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionMargins.Gutter = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
		}
	}
	#endregion
	#region DocCommandRTLGutter
	public class DocCommandRTLGutter : IDocCommand {
		#region Fields
		byte rtlGutter;
		#endregion
		#region Properties
		public SectionGutterAlignment GutterAlignment {
			get { return (rtlGutter == 0) ? SectionGutterAlignment.Left : SectionGutterAlignment.Right; }
			set { CalcGutterAlignment(value); }
		}
		#endregion
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public void Read(byte[] sprm) {
			this.rtlGutter = sprm[0];
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			if (this.rtlGutter == 1)
				propertyContainer.SectionInfo.SectionMargins.GutterAlignment = SectionGutterAlignment.Right;
			else
				propertyContainer.SectionInfo.SectionMargins.GutterAlignment = SectionGutterAlignment.Left;
		}
		public void Write(BinaryWriter writer) {
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), new byte[] { this.rtlGutter });
			writer.Write(sprm);
		}
		#endregion
		void CalcGutterAlignment(SectionGutterAlignment alignment) {
			if (alignment == SectionGutterAlignment.Left)
				this.rtlGutter = 0;
			if (alignment == SectionGutterAlignment.Right)
				this.rtlGutter = 1;
		}
	}
	#endregion
	#region DocCommandHeaderOffset
	public class DocCommandHeaderOffset : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionMargins.HeaderOffset = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
		}
	}
	#endregion
	#region DocCommandFooterOffset
	public class DocCommandFooterOffset : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionMargins.FooterOffset = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
		}
	}
	#endregion
	#region DocCommandColumnCount
	public class DocCommandColumnCount : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionColumns.ColumnCount = Value + 1;
			for (int i = 0; i < propertyContainer.SectionInfo.SectionColumns.ColumnCount; i++) {
				propertyContainer.SectionInfo.SectionColumns.Columns.Add(new ColumnInfo());
			}
		}
		public override void Write(BinaryWriter writer) {
			Value--;
			base.Write(writer);
		}
	}
	#endregion
	#region DocCommandColumnWidth
	public class DocCommandColumnWidth : IDocCommand {
		public byte ColumnIndex { get; set; }
		public short ColumnWidth { get; set; }
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public void Read(byte[] sprm) {
			ColumnIndex = sprm[0];
			ColumnWidth = BitConverter.ToInt16(sprm, 1);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			List<ColumnInfo> columns = propertyContainer.SectionInfo.SectionColumns.Columns;
			if (ColumnIndex >= columns.Count)
				return;
			columns[ColumnIndex].Width = ColumnWidth;
		}
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
			writer.Write(ColumnIndex);
			writer.Write(Math.Max((short)720, ColumnWidth));
		}
		#endregion
	}
	#endregion
	#region DocCommandNotEvenlyColumnsSpace
	public class DocCommandNotEvenlyColumnsSpace : IDocCommand {
		public byte ColumnIndex { get; set; }
		public short ColumnSpace { get; set; }
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public void Read(byte[] sprm) {
			ColumnIndex = sprm[0];
			ColumnSpace = BitConverter.ToInt16(sprm, 1);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			List<ColumnInfo> columns = propertyContainer.SectionInfo.SectionColumns.Columns;
			if (ColumnIndex >= columns.Count)
				return;
			columns[ColumnIndex].Space = ColumnSpace;
		}
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
			writer.Write(ColumnIndex);
			ColumnSpace = Math.Min(ColumnSpace, (short)DocConstants.MaxXASValue);
			ColumnSpace = Math.Max((short)0, ColumnSpace);
			writer.Write(ColumnSpace);
		}
		#endregion
	}
	#endregion
	#region DocCommandDrawVerticalSeparator
	public class DocCommandDrawVerticalSeparator : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionColumns.DrawVerticalSeparator = Value;
		}
	}
	#endregion
	#region DocCommandEqualWidthColumns
	public class DocCommandEqualWidthColumns : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			ColumnsInfo info = propertyContainer.SectionInfo.SectionColumns;
			if (info.ColumnCount == 1)
				return;
			propertyContainer.SectionInfo.SectionColumns.EqualWidthColumns = Value;
		}
	}
	#endregion
	#region DocCommandColumnSpace
	public class DocCommandColumnSpace : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionColumns.Space = Value;
		}
	}
	#endregion
	#region DocCommandPageHeight
	public class DocCommandPageHeight : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionPage.Height = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
		}
	}
	#endregion
	#region DocCommandPageOrientation
	public class DocCommandPageOrientation : DocCommandBytePropertyValueBase {
		#region Properties
		public bool Landscape {
			get { return Value == 0x02; }
			set { Value = (value) ? (byte)0x02 : (byte)0x01; }
		}
		#endregion
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionPage.Landscape = Landscape;
		}
	}
	#endregion
	#region DocCommandPageWidth
	public class DocCommandPageWidth : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionPage.Width = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
		}
	}
	#endregion
	#region DocCommandDifferentFirstPage
	public class DocCommandDifferentFirstPage : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionGeneralSettings.DifferentFirstPage = Value;
		}
	}
	#endregion
	#region DocCommandFirstPagePaperSource
	public class DocCommandFirstPagePaperSource : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionGeneralSettings.FirstPagePaperSource = Value;
		}
	}
	#endregion
	#region DocCommandOnlyAllowEditingOfFormFields
	public class DocCommandOnlyAllowEditingOfFormFields : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionGeneralSettings.OnlyAllowEditingOfFormFields = Value;
		}
	}
	#endregion
	#region DocCommandOtherPagePaperSource
	public class DocCommandOtherPagePaperSource : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionGeneralSettings.OtherPagePaperSource = Value;
		}
	}
	#endregion
	#region DocCommandStartType
	public class DocCommandStartType : DocCommandBytePropertyValueBase {
		#region static
		static DocCommandStartType() {
			startTypes = new Dictionary<SectionStartType, byte>(5);
			startTypes.Add(SectionStartType.Continuous, 0);
			startTypes.Add(SectionStartType.Column, 1);
			startTypes.Add(SectionStartType.NextPage, 2);
			startTypes.Add(SectionStartType.EvenPage, 3);
			startTypes.Add(SectionStartType.OddPage, 4);
		}
		#endregion
		#region Fields
		static Dictionary<SectionStartType, byte> startTypes;
		#endregion
		public SectionStartType StartType {
			get { return CalcSectionStartType(); }
			set { Value = startTypes[value]; }
		}
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionGeneralSettings.StartType = StartType;
		}
		SectionStartType CalcSectionStartType() {
			if (Value == 0)
				return SectionStartType.Continuous;
			if (Value == 1)
				return SectionStartType.Column;
			if (Value == 2)
				return SectionStartType.NextPage;
			if (Value == 3)
				return SectionStartType.EvenPage;
			return SectionStartType.OddPage;
		}
	}
	#endregion
	#region DocCommandTextDirection
	public class DocCommandTextDirection : DocCommandBytePropertyValueBase {
		TextDirection textDirection;
		public TextDirection TextDirection {
			get { return this.textDirection; }
			set {
				if (this.textDirection == value) return;
				this.textDirection = value;
				CalcTextDirectionTypeCode();
			}
		}
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			if (Value == 0) {
				propertyContainer.SectionInfo.SectionGeneralSettings.TextDirection = TextDirection.LeftToRightTopToBottom;
				return;
			}
			else
				propertyContainer.SectionInfo.SectionGeneralSettings.TextDirection = TextDirection.TopToBottomRightToLeft;
		}
		void CalcTextDirectionTypeCode() {
			if (this.textDirection == TextDirection.LeftToRightTopToBottom || this.textDirection == TextDirection.LeftToRightTopToBottomRotated)
				Value = 0;
			else
				Value = 1;
		}
	}
	#endregion
	#region DocCommandVerticalTextAlignment
	public class DocCommandVerticalTextAlignment : DocCommandBytePropertyValueBase {
		#region static
		static DocCommandVerticalTextAlignment() {
			verticalAlignmentTypes = new Dictionary<VerticalAlignment, byte>(4);
			verticalAlignmentTypes.Add(VerticalAlignment.Top, 0);
			verticalAlignmentTypes.Add(VerticalAlignment.Center, 1);
			verticalAlignmentTypes.Add(VerticalAlignment.Both, 2);
			verticalAlignmentTypes.Add(VerticalAlignment.Bottom, 3);
		}
		#endregion
		#region Fields
		static Dictionary<VerticalAlignment, byte> verticalAlignmentTypes;
		#endregion
		#region Properties
		public VerticalAlignment VerticalTextAlignment {
			get { return CalcVerticalAlignment(); }
			set { Value = verticalAlignmentTypes[value]; }
		}
		#endregion
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionGeneralSettings.VerticalTextAlignment = VerticalTextAlignment;
		}
		VerticalAlignment CalcVerticalAlignment() {
			if (Value == 0)
				return VerticalAlignment.Top;
			if (Value == 1)
				return VerticalAlignment.Center;
			if (Value == 2)
				return VerticalAlignment.Both;
			return VerticalAlignment.Bottom;
		}
	}
	#endregion
	#region DocCommandLineNumberingDistance
	public class DocCommandLineNumberingDistance : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionLineNumbering.Distance = Value;
		}
	}
	#endregion
	#region DocCommandNumberingRestartType
	public class DocCommandNumberingRestartType : DocCommandBytePropertyValueBase {
		#region static
		static DocCommandNumberingRestartType() {
			lineNumberingRestartTypes = new Dictionary<LineNumberingRestart, byte>(3);
			lineNumberingRestartTypes.Add(LineNumberingRestart.NewPage, 0x00);
			lineNumberingRestartTypes.Add(LineNumberingRestart.NewSection, 0x01);
			lineNumberingRestartTypes.Add(LineNumberingRestart.Continuous, 0x02);
		}
		#endregion
		#region Fields
		static Dictionary<LineNumberingRestart, byte> lineNumberingRestartTypes;
		#endregion
		#region Properties
		public LineNumberingRestart NumberingRestartType {
			get { return CalcLineNumberingRestart(); }
			set { Value = lineNumberingRestartTypes[value]; }
		}
		#endregion
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionLineNumbering.NumberingRestartType = NumberingRestartType;
		}
		LineNumberingRestart CalcLineNumberingRestart() {
			if (Value == 0x00)
				return LineNumberingRestart.NewPage;
			if (Value == 0x01)
				return LineNumberingRestart.NewSection;
			return LineNumberingRestart.Continuous;
		}
	}
	#endregion
	#region DocCommandStartingLineNumber
	public class DocCommandStartLineNumber : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionLineNumbering.StartingLineNumber = Value + 1; 
		}
		public override void Write(BinaryWriter writer) {
			Value--;
			base.Write(writer);
		}
	}
	#endregion
	#region DocCommandStep
	public class DocCommandStep : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionLineNumbering.Step = Value;
		}
	}
	#endregion
	#region DocCommandChapterHeaderStyle
	public class DocCommandChapterHeaderStyle : DocCommandBytePropertyValueBase {
		public int ChapterHeaderStyle {
			get { return Value; }
			set { Value = (byte)value; }
		}
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionPageNumbering.ChapterHeaderStyle = Value;
		}
	}
	#endregion
	#region DocCommandChapterSeparator
	public class DocCommandChapterSeparator : DocCommandBytePropertyValueBase {
		#region static
		static DocCommandChapterSeparator() {
			chapterSeparators = new Dictionary<byte, char>(5);
			chapterSeparators.Add(0x00, Characters.Hyphen);
			chapterSeparators.Add(0x01, Characters.Dot);
			chapterSeparators.Add(0x02, Characters.Colon);
			chapterSeparators.Add(0x03, Characters.EmDash);
			chapterSeparators.Add(0x04, Characters.EnDash);
		}
		#endregion
		#region Fields
		static Dictionary<byte, char> chapterSeparators;
		#endregion
		#region Properties
		public char ChapterSeparator {
			get { return chapterSeparators[Value]; }
			set { Value = CalcChapterSeparatorCode(value); }
		}
		#endregion
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			char chapterSeparator;
			if (chapterSeparators.TryGetValue(Value, out chapterSeparator))
				propertyContainer.SectionInfo.SectionPageNumbering.ChapterSeparator = chapterSeparator;
		}
		byte CalcChapterSeparatorCode(char chapterSeparator) {
			switch (chapterSeparator) {
				case Characters.Dot: return 0x01;
				case Characters.Colon: return 0x02;
				case Characters.EmDash: return 0x03;
				case Characters.EnDash: return 0x04;
				default: return 0x00;
			}
		}
	}
	#endregion
	#region DocCommandNumberingFormat
	public class DocCommandNumberingFormat : DocCommandBytePropertyValueBase {
		public NumberingFormat NumberingFormat {
			get { return NumberingFormatCalculator.CalcNumberingFormat(Value); }
			set { Value = NumberingFormatCalculator.CalcNumberingFormatCode(value); }
		}
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionPageNumbering.NumberingFormat = NumberingFormat;
		}
	}
	#endregion
	#region DocCommandUseStartinPageNumber
	public class DocCommandUseStartingPageNumber : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.None; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
		}
	}
	#endregion
	#region DocCommandStartingPageNumber
	public class DocCommandStartingPageNumber : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionPageNumbering.StartingPageNumber = Value;
		}
	}
	#endregion
	#region DocCommandStartingPageNumberNew
	public class DocCommandStartingPageNumberNew : DocCommandIntPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.SectionPageNumbering.StartingPageNumber = Value;
		}
	}
	#endregion
	#region DocCommandFootNotePositionBase
	public abstract class DocCommandFootNotePositionBase : DocCommandBytePropertyValueBase {
		public FootNotePosition Position {
			get { return FootNotePositionCalculator.CalcFootNotePosition(Value); }
			set { Value = FootNotePositionCalculator.CalcFootNotePositionTypeCode(value); }
		}
	}
	#endregion
	#region DocCommandFootNoteNumberingRestartTypeBase
	public abstract class DocCommandFootNoteNumberingRestartTypeBase : DocCommandBytePropertyValueBase {
		public LineNumberingRestart NumberingRestartType {
			get { return FootNoteNumberingRestartCalculator.CalcFootNoteNumberingRestart(Value); }
			set { Value = FootNoteNumberingRestartCalculator.CalcFootNoteNumberingRestartTypeCode(value); }
		}
	}
	#endregion
	#region DocCommandFootNoteNumberingFormatBase
	public abstract class DocCommandFootNoteNumberingFormatBase : DocCommandShortPropertyValueBase {
		public NumberingFormat NumberingFormat {
			get { return NumberingFormatCalculator.CalcNumberingFormat(Value); }
			set { Value = NumberingFormatCalculator.CalcNumberingFormatCode(value); }
		}
	}
	#endregion
	#region DocCommandFootNotePosition
	public class DocCommandFootNotePosition : DocCommandFootNotePositionBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.FootNote.Position = Position;
		}
	}
	#endregion
	#region DocCommandFootNoteStartingNumber
	public class DocCommandFootNoteStartingNumber : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.FootNote.StartingNumber = Value;
		}
	}
	#endregion
	#region DocCommandFootNoteNumberingRestartType
	public class DocCommandFootNoteNumberingRestartType : DocCommandFootNoteNumberingRestartTypeBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.FootNote.NumberingRestartType = NumberingRestartType;
		}
	}
	#endregion
	#region DocCommandFootNoteNumberingFormat
	public class DocCommandFootNoteNumberingFormat : DocCommandFootNoteNumberingFormatBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.FootNote.NumberingFormat = NumberingFormat;
		}
	}
	#endregion
	#region DocCommandEndNotePosition
	public class DocCommandEndNotePosition : DocCommandFootNotePositionBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.EndNote.Position = Position;
		}
	}
	#endregion
	#region DocCommandEndNoteStartingNumber
	public class DocCommandEndNoteStartingNumber : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.EndNote.StartingNumber = Value;
		}
	}
	#endregion
	#region DocCommandEndNoteNumberingRestartType
	public class DocCommandEndNoteNumberingRestartType : DocCommandFootNoteNumberingRestartTypeBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.EndNote.NumberingRestartType = NumberingRestartType;
		}
	}
	#endregion
	#region DocCommandEndNoteNumberingFormat
	public class DocCommandEndNoteNumberingFormat : DocCommandFootNoteNumberingFormatBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Section; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.SectionInfo.EndNote.NumberingFormat = NumberingFormat;
		}
	}
	#endregion
	#region DocCommandChangeCharacterStyle
	public class DocCommandChangeCharacterStyle : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.FormattingInfo.StyleIndex = Value;
		}
	}
	#endregion
	#region DocCommandChangeParagraphStyle
	public class DocCommandChangeParagraphStyle : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.ParagraphStyleIndex = Value;
		}
	}
	#endregion
	#region DocCommandChangeTableStyle
	public class DocCommandChangeTableStyle : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableInfo.TableStyleIndex = Value;
		}
	}
	#endregion
	#region DocCommandSpecial
	public class DocCommandSpecial : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.Special = Value;
		}
	}
	#endregion
	#region DocCommandBinaryData
	public class DocCommandBinaryData : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.BinaryData = Value;
		}
	}
	#endregion
	#region DocCommandEmbeddedObject
	public class DocCommandEmbeddedObject : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.EmbeddedObject = Value;
		}
	}
	#endregion
	#region DocCommandOle2Object
	public class DocCommandOle2Object : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.Ole2Object = Value;
		}
	}
	#endregion
	#region DocCommandSymbol
	public class DocCommandSymbol : IDocCommand {
		public short FontFamilyNameIndex { get; set; }
		public char Symbol { get; set; }
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public void Read(byte[] sprm) {
			FontFamilyNameIndex = BitConverter.ToInt16(sprm, 0);
			Symbol = ParseUnicodeChar(BitConverter.ToInt16(sprm, 2));
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.Special = true;
			propertyContainer.CharacterInfo.SpecialCharactersFontFamilyNameIndex = FontFamilyNameIndex;
			propertyContainer.CharacterInfo.FormattingOptions.UseFontName = true;
			propertyContainer.CharacterInfo.Symbol = Symbol;
		}
		public void Write(BinaryWriter writer) {
			byte[] operand = new byte[4];
			Array.Copy(BitConverter.GetBytes(FontFamilyNameIndex), 0, operand, 0, 2);
			Array.Copy(BitConverter.GetBytes(Symbol), 0, operand, 2, 2);
			BitConverter.GetBytes(FontFamilyNameIndex);
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), operand);
			writer.Write(sprm);
		}
		#endregion
		protected static char ParseUnicodeChar(short parameterValue) {
			char ch = (char)(parameterValue & 0xffff);
			if ((int)ch >= 0xf020 && (int)ch <= 0xf0ff)
				ch = (char)((int)ch - 0xf000);
			return ch;
		}
	}
	#endregion
	#region DocCommandPictureOffset
	public class DocCommandPictureOffset : DocCommandIntPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.DataStreamOffset = Value;
			propertyContainer.CharacterInfo.Special = true;
		}
	}
	#endregion
	#region DocCommandChangeParagraphTabsCommandBase
	public abstract class DocCommandChangeParagraphTabsBase : IDocCommand {
		#region Fields
		TabsOperandBase operand;
		TabFormattingInfo tabs;
		#endregion
		protected DocCommandChangeParagraphTabsBase() { }
		#region Properties
		public TabFormattingInfo Tabs {
			get { return this.tabs; }
			set {
				if (this.tabs == value) return;
				this.tabs = value;
			}
		}
		public DocumentModelUnitConverter UnitConverter { get; set; }
		#endregion
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public void Read(byte[] sprm) {
			this.operand = CreateOperandFromByteArray(sprm);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			this.operand.AddTabs(propertyContainer.ParagraphInfo.Tabs, propertyContainer.UnitConverter);
		}
		public void Write(BinaryWriter writer) {
			TryGetOperand();
			if (this.operand != null) {
				writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
				this.operand.Write(writer);
			}
		}
		#endregion
		void TryGetOperand() {
			if (this.tabs != null && UnitConverter != null) {
				this.operand = CreateOperand();
				this.operand.ConvertFromFormattingTabInfo(this.tabs, UnitConverter);
			}
		}
		protected abstract TabsOperandBase CreateOperand();
		protected abstract TabsOperandBase CreateOperandFromByteArray(byte[] sprm);
	}
	#endregion
	#region DocCommandChangeParagraphTabs
	public class DocCommandChangeParagraphTabs : DocCommandChangeParagraphTabsBase {
		public DocCommandChangeParagraphTabs() { }
		protected override TabsOperandBase CreateOperand() {
			return new TabsOperand();
		}
		protected override TabsOperandBase CreateOperandFromByteArray(byte[] sprm) {
			return TabsOperand.FromByteArray(sprm);
		}
	}
	#endregion
	#region DocCommandChangeParagraphTabsClose
	public class DocCommandChangeParagraphTabsClose : DocCommandChangeParagraphTabsBase {
		public DocCommandChangeParagraphTabsClose() { }
		protected override TabsOperandBase CreateOperand() {
			return new TabsOperandClose();
		}
		protected override TabsOperandBase CreateOperandFromByteArray(byte[] sprm) {
			return TabsOperandClose.FromByteArray(sprm);
		}
	}
	#endregion
	#region DocCommandListInfoIndex
	public class DocCommandListInfoIndex : DocCommandShortPropertyValueBase {
		const int preWord97Format = 0x7ff;
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			if (Value != preWord97Format)
				propertyContainer.ParagraphInfo.ListInfoIndex = Value;
		}
	}
	#endregion
	#region DocCommandListLevel
	public class DocCommandListLevel : DocCommandBytePropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.ListLevel = Value;
		}
	}
	#endregion
	#region DocCommandPictureBulletCharacterPosition
	public class DocCommandPictureBulletCharacterPosition : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.CharacterInfo.PictureBulletInformation.PictureCharacterPosition = Value;
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), BitConverter.GetBytes(Value)));
		}
	}
	#endregion
	#region DocCommandPictureBulletProperties
	public class DocCommandPictureBulletProperties : IDocCommand {
		short pictureBulletInformationBitField;
		public bool DefaultPicture { get; set; }
		public bool PictureBullet { get; set; }
		public bool SuppressBulletResize { get; set; }
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Character; } }
		public void Read(byte[] sprm) {
			pictureBulletInformationBitField = BitConverter.ToInt16(sprm, 0);
			PictureBullet = (this.pictureBulletInformationBitField & 0x01) != 0;
			SuppressBulletResize = (this.pictureBulletInformationBitField & 0x02) != 0;
			DefaultPicture = (this.pictureBulletInformationBitField & 0x04) != 0;
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			DocPictureBulletInformation pictureBulletInformation = propertyContainer.CharacterInfo.PictureBulletInformation;
			pictureBulletInformation.DefaultPicture = DefaultPicture;
			pictureBulletInformation.PictureBullet = PictureBullet;
			pictureBulletInformation.SuppressBulletResize = SuppressBulletResize;
		}
		public void Write(BinaryWriter writer) {
			if (PictureBullet)
				this.pictureBulletInformationBitField |= 0x01;
			if (SuppressBulletResize)
				this.pictureBulletInformationBitField |= 0x02;
			if (DefaultPicture)
				this.pictureBulletInformationBitField |= 0x04;
			writer.Write(DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), BitConverter.GetBytes(this.pictureBulletInformationBitField)));
		}
		#endregion
	}
	#endregion
	#region DocCommandDefineTable
	public class DocCommandTableDefinition : IDocCommand {
		#region Fields
		TableDefinitionOperand tableDefinition;
		#endregion
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public void Read(byte[] sprm) {
			this.tableDefinition = TableDefinitionOperand.FromByteArray(sprm);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableInfo.TableDefinition = this.tableDefinition;
		}
		public void Write(BinaryWriter writer) {
		}
		#endregion
	}
	#endregion
	#region DocCommandInTable
	public class DocCommandInTable : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.InTable = Value;
		}
	}
	#endregion
	#region DocCommandTableTrailer
	public class DocCommandTableTrailer : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction {
			get { return ChangeActionTypes.Paragraph; }
		}
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.TableTrailer = Value;
		}
	}
	#endregion
	#region DocCommandTableDepth
	public class DocCommandTableDepth : DocCommandIntPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.TableDepth = Value;
		}
	}
	#endregion
	#region DocCommandTableDepthIncrement
	public class DocCommandTableDepthIncrement : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.TableDepth += Value;
			if (propertyContainer.ParagraphInfo.TableDepth == 0)
				propertyContainer.ParagraphInfo.InTable = false;
		}
	}
	#endregion
	#region DocCommandPreferredTableWidth
	public class DocCommandPreferredTableWidth : DocCommandWidthUnitPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		protected override void ApplyWidthOperand(DocPropertyContainer propertyContainer) {
			propertyContainer.TableInfo.TableProperties.PreferredWidth.Type = WidthUnit.Type;
			propertyContainer.TableInfo.TableProperties.PreferredWidth.Value = WidthUnit.Value;
		}
	}
	#endregion
	#region DocCommandPreferredTableCellWidth
	public class DocCommandPreferredTableCellWidth : IDocCommand {
		#region Fields
		TableCellWidthOperand tableCellWidth;
		#endregion
		public DocCommandPreferredTableCellWidth() {
			this.tableCellWidth = new TableCellWidthOperand();
		}
		#region Properties
		public TableCellWidthOperand TableCellWidth { get { return this.tableCellWidth; } }
		#endregion
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public void Read(byte[] sprm) {
			this.tableCellWidth = TableCellWidthOperand.FromByteArray(sprm);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.PreferredCellWidths.Add(TableCellWidth);
		}
		public void Write(BinaryWriter writer) {
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), TableCellWidth.GetBytes());
			writer.Write(sprm);
		}
		#endregion
	}
	#endregion
	#region DocCommandInnerTableCell
	public class DocCommandInnerTableCell : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.InnerTableCell = Value;
		}
	}
	#endregion
	#region DocCommandInnerTableRow
	public class DocCommandInnerTableTrailer : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Paragraph; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.ParagraphInfo.NestedTableTrailer = Value;
		}
	}
	#endregion
	#region DocCommandInsertTableCells
	public class DocCommandInsertTableCell : IDocCommand {
		public DocCommandInsertTableCell() {
			Insert = new InsertOperand();
		}
		public InsertOperand Insert { get; protected internal set; }
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public void Read(byte[] sprm) {
			Insert = InsertOperand.FromByteArray(sprm);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.InsertActions.Add(Insert);
		}
		public void Write(BinaryWriter writer) {
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), Insert.GetBytes());
			writer.Write(sprm);
		}
		#endregion
	}
	#endregion
	#region DocCommandTableRowHeight
	public class DocCommandTableRowHeight : DocCommandShortPropertyValueBase {
		public HeightUnitType Type { get; set; }
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.TableRowProperties.Height.Type = CalcHeightUnitType();
			propertyContainer.TableRowInfo.TableRowProperties.Height.Value = propertyContainer.UnitConverter.TwipsToModelUnits(Math.Abs(Value));
		}
		public override void Write(BinaryWriter writer) {
			if (Type == HeightUnitType.Exact)
				Value *= -1;
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), BitConverter.GetBytes((short)Value));
			writer.Write(sprm);
		}
		HeightUnitType CalcHeightUnitType() {
			if (Value > 0)
				return HeightUnitType.Minimum;
			else if (Value < 0)
				return HeightUnitType.Exact;
			return HeightUnitType.Auto;
		}
	}
	#endregion
	#region DocCommandTableCellWidth
	public class DocCommandTableCellWidth : IDocCommand {
		public ColumnWidthOperand ColumnWidth { get; protected internal set; }
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public void Read(byte[] sprm) {
			ColumnWidth = ColumnWidthOperand.FromByteArray(sprm);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.ColumnWidthActions.Add(ColumnWidth);
		}
		public void Write(BinaryWriter writer) {
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), ColumnWidth.GetBytes());
			writer.Write(sprm);
		}
		#endregion
	}
	#endregion
	#region DocCommandMergeTableCells
	public class DocCommandMergeTableCells : DocCommandMergeTableCellsBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.HorizontalMerging.Add(new HorizontalMergeInfo(FirstMergedIndex, LastMergedIndex, false));
		}
	}
	#endregion
	#region DocCommandSplitTableCells
	public class DocCommandSplitTableCells : DocCommandMergeTableCellsBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.HorizontalMerging.Add(new HorizontalMergeInfo(FirstMergedIndex, LastMergedIndex, true));
		}
	}
	#endregion
	#region DocCommandVerticalMergeTableCells
	public class DocCommandVerticalMergeTableCells : IDocCommand {
		public byte CellIndex { get; set; }
		public MergingState VerticalMerging { get; set; }
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public void Read(byte[] sprm) {
			CellIndex = sprm[0];
			VerticalMerging = MergingStateCalculator.CalcVerticalMergingState(sprm[1]);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.VerticalMerging.Add(new VerticalMergeInfo(CellIndex, VerticalMerging));
		}
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), new byte[] { CellIndex, MergingStateCalculator.CalcVerticalMergingTypeCode(VerticalMerging) }));
		}
		#endregion
	}
	#endregion
	#region DocCommandTableAutoFit
	public class DocCommandTableAutoFit : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			if (Value)
				propertyContainer.TableInfo.TableProperties.TableLayout = TableLayoutType.Autofit;
			else
				propertyContainer.TableInfo.TableProperties.TableLayout = TableLayoutType.Fixed;
		}
	}
	#endregion
	#region DocCommandTableLeftOffset
	public class DocCommandTableDxaLeft : DocCommandXASPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.None; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			if (propertyContainer.TableRowInfo.WidthBeforeSetted)
				return;
			propertyContainer.TableRowInfo.TableRowProperties.WidthBefore.Type = WidthUnitType.ModelUnits;
			propertyContainer.TableRowInfo.TableRowProperties.WidthBefore.Value = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
		}
	}
	#endregion
	#region DocCommandTableLeftOffsetGap
	public class DocCommandTableDxaGapHalf : DocCommandXASPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.None; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
		}
	}
	#endregion
	#region DocCommandTableBorders
	public class DocCommandTableBorders : IDocCommand {
		#region Fields
		const byte tableBordersOperandSize = 0x30;
		TableBordersOperand tableBorders;
		#endregion
		public DocCommandTableBorders() {
			this.tableBorders = new TableBordersOperand();
		}
		#region Properties
		public TableBordersOperand TableBorders { get { return this.tableBorders; } }
		#endregion
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public void Read(byte[] sprm) {
			this.tableBorders = TableBordersOperand.FromByteArray(sprm);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			TableBorders.ApplyProperties(propertyContainer.TableInfo.TableProperties.Borders, propertyContainer.UnitConverter);
		}
		public void Write(BinaryWriter writer) {
			if (this.tableBorders != null) {
				writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
				writer.Write(tableBordersOperandSize);
				this.tableBorders.Write(writer);
			}
		}
		#endregion
	}
	#endregion
	#region DocCommandCellBorders
	public class DocCommandOverrideCellBorders : IDocCommand {
		#region Fields
		TableBordersOverrideOperand bordersOverrideOperand;
		#endregion
		public DocCommandOverrideCellBorders() {
			this.bordersOverrideOperand = new TableBordersOverrideOperand();
		}
		#region Properties
		public TableBordersOverrideOperand OverriddenBorders { get { return this.bordersOverrideOperand; } }
		#endregion
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public void Read(byte[] sprm) {
			this.bordersOverrideOperand = TableBordersOverrideOperand.FromByteArray(sprm);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.OverrideCellBordersActions.Add(OverriddenBorders);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
			writer.Write(TableBordersOverrideOperand.Size);
			OverriddenBorders.Write(writer);
		}
		#endregion
	}
	#endregion
	#region DocCommandWidthAfter
	public class DocCommandWidthAfter : DocCommandWidthUnitPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		protected override void ApplyWidthOperand(DocPropertyContainer propertyContainer) {
			propertyContainer.TableRowInfo.TableRowProperties.WidthAfter.Type = WidthUnit.Type;
			propertyContainer.TableRowInfo.TableRowProperties.WidthAfter.Value = WidthUnit.Value;
		}
	}
	#endregion
	#region DocCommandWidthBefore
	public class DocCommandWidthBefore : DocCommandWidthUnitPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		protected override void ApplyWidthOperand(DocPropertyContainer propertyContainer) {
			propertyContainer.TableRowInfo.TableRowProperties.WidthBefore.Type = WidthUnit.Type;
			propertyContainer.TableRowInfo.TableRowProperties.WidthBefore.Value = WidthUnit.Value;
			propertyContainer.TableRowInfo.WidthBeforeSetted = true;
		}
	}
	#endregion
	#region DocCommandWidthIndent
	public class DocCommandWidthIndent : DocCommandWidthUnitPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		protected override void ApplyWidthOperand(DocPropertyContainer propertyContainer) {
			propertyContainer.TableInfo.TableProperties.TableIndent.Type = WidthUnit.Type;
			propertyContainer.TableInfo.TableProperties.TableIndent.Value = WidthUnit.Value;
		}
	}
	#endregion
	#region DocCommandTableBackgroundColor
	public class DocCommandTableBackgroundColor : DocCommandShadingPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableInfo.TableProperties.BackgroundColor = ShadingDescriptor.BackgroundColor;
		}
	}
	#endregion
	#region DocCommandDefineTableShadingsBase
	public abstract class DocCommandDefineTableShadingsBase : DocCommandShadingListBase {
		protected abstract int StartIndex { get; }
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableCell; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			for (int i = 0; i < CellColors.Count; i++) {
				propertyContainer.TableCellInfo.CellColors.Insert(StartIndex + i, CellColors[i]);
			}
		}
	}
	#endregion
	#region DocCommandDefineTableShadings
	public class DocCommandDefineTableShadings : DocCommandDefineTableShadingsBase {
		protected override int StartIndex { get { return 0; } }
	}
	#endregion
	#region DocCommandDefineTableShadings2nd
	public class DocCommandDefineTableShadings2nd : DocCommandDefineTableShadingsBase {
		protected override int StartIndex { get { return 22; } }
	}
	#endregion
	#region DocCommandDefineTableShadings3rd
	public class DocCommandDefineTableShadings3rd : DocCommandDefineTableShadingsBase {
		protected override int StartIndex { get { return 44; } }
	}
	#endregion
	#region DocCommandTableAlignment
	public class DocCommandTableAlignment : DocCommandShortPropertyValueBase {
		#region Properties
		public TableRowAlignment TableAlignment {
			get { return CalcAlignmentType(); }
			set { Value = CalcAlignmentTypeCode(value); }
		}
		#endregion
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			CalcAlignmentType();
			propertyContainer.TableInfo.TableProperties.TableAlignment = TableAlignment;
		}
		TableRowAlignment CalcAlignmentType() {
			switch (Value) {
				case 0: return TableRowAlignment.Left;
				case 1: return TableRowAlignment.Center;
				case 2: return TableRowAlignment.Right;
				default: return TableRowAlignment.Left;
			}
		}
		int CalcAlignmentTypeCode(TableRowAlignment alignment) {
			switch (alignment) {
				case TableRowAlignment.Left: return 0;
				case TableRowAlignment.Center: return 1;
				case TableRowAlignment.Right: return 2;
				default: return 0;
			}
		}
	}
	#endregion
	#region DocCommandCellSpacing
	public class DocCommandCellSpacing : DocCommandCellSpacingPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		protected override void ApplyCellSpacingOperand(DocPropertyContainer propertyContainer) {
			propertyContainer.TableInfo.TableProperties.CellSpacing.Type = CellSpacing.WidthUnit.Type;
			propertyContainer.TableInfo.TableProperties.CellSpacing.Value = CellSpacing.WidthUnit.Value;
		}
	}
	#endregion
	#region DocCommandCellPaddingDefault
	public class DocCommandCellMarginDefault : DocCommandCellSpacingPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		protected override void ApplyCellSpacingOperand(DocPropertyContainer propertyContainer) {
			CellSpacing.ApplyProperties(propertyContainer.TableInfo.TableProperties.CellMargins, propertyContainer.UnitConverter);
		}
	}
	#endregion
	#region DocCommandCellPadding
	public class DocCommandCellMargin : DocCommandCellSpacingPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		protected override void ApplyCellSpacingOperand(DocPropertyContainer propertyContainer) {
			propertyContainer.TableRowInfo.CellMarginsActions.Add(CellSpacing);
		}
	}
	#endregion
	#region DocCommandTableOverlap
	public class DocCommandTableOverlap : DocCommandBoolPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableInfo.TableProperties.IsTableOverlap = !Value;
		}
	}
	#endregion
	#region DocCommandTableHorizontalPosition
	public class DocCommandTableHorizontalPosition : DocCommandShortPropertyValueBase {
		#region Fields
		public const int LeftAligned = 0x0000;
		public const int Centered = unchecked((int)0xfffffffc);
		public const int RightAligned = unchecked((int)0xfffffff8);
		public const int Inside = unchecked((int)0xfffffff4);
		public const int Outside = unchecked((int)0xfffffff0);
		#endregion
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			TableFloatingPosition floatingPosition = propertyContainer.TableInfo.TableProperties.FloatingPosition;
			switch (Value) {
				case LeftAligned:
					floatingPosition.HorizontalAlign = HorizontalAlignMode.Left;
					break;
				case Centered:
					floatingPosition.HorizontalAlign = HorizontalAlignMode.Center;
					break;
				case RightAligned:
					floatingPosition.HorizontalAlign = HorizontalAlignMode.Right;
					break;
				case Inside:
					floatingPosition.HorizontalAlign = HorizontalAlignMode.Inside;
					break;
				case Outside:
					floatingPosition.HorizontalAlign = HorizontalAlignMode.Outside;
					break;
				default:
					floatingPosition.TableHorizontalPosition = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
					break;
			}
		}
	}
	#endregion
	#region DocCommandTableVerticalPosition
	public class DocCommandTableVerticalPosition : DocCommandShortPropertyValueBase {
		#region Fields
		public const int Inline = 0x0000;
		public const int Top = unchecked((int)0xfffffffc);
		public const int Center = unchecked((int)0xfffffff8);
		public const int Bottom = unchecked((int)0xfffffff4);
		public const int Inside = unchecked((int)0xfffffff0);
		public const int Outside = unchecked((int)0xffffffec);
		#endregion
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			TableFloatingPosition floatingPosition = propertyContainer.TableInfo.TableProperties.FloatingPosition;
			switch (Value) {
				case Inline:
					floatingPosition.VerticalAlign = VerticalAlignMode.Inline;
					break;
				case Top:
					floatingPosition.VerticalAlign = VerticalAlignMode.Top;
					break;
				case Center:
					floatingPosition.VerticalAlign = VerticalAlignMode.Center;
					break;
				case Bottom:
					floatingPosition.VerticalAlign = VerticalAlignMode.Bottom;
					break;
				case Inside:
					floatingPosition.VerticalAlign = VerticalAlignMode.Inside;
					break;
				case Outside:
					floatingPosition.VerticalAlign = VerticalAlignMode.Outside;
					break;
				default:
					floatingPosition.TableVerticalPosition = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
					break;
			}
		}
	}
	#endregion
	#region DocCommandBottomFromText
	public class DocCommandBottomFromText : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableInfo.TableProperties.FloatingPosition.BottomFromText = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
		}
	}
	#endregion
	#region DocCommandLeftFromText
	public class DocCommandLeftFromText : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableInfo.TableProperties.FloatingPosition.LeftFromText = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
		}
	}
	#endregion
	#region DocCommandRightFromText
	public class DocCommandRightFromText : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableInfo.TableProperties.FloatingPosition.RightFromText = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
		}
	}
	#endregion
	#region DocCommandTopFromText
	public class DocCommandTopFromText : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableInfo.TableProperties.FloatingPosition.TopFromText = propertyContainer.UnitConverter.TwipsToModelUnits(Value);
		}
	}
	#endregion
	#region DocCommandFramePosition
	public class DocCommandFramePosition : IDocCommand {
		public HorizontalAnchorTypes HorizontalAnchor { get; set; }
		public VerticalAnchorTypes VerticalAnchor { get; set; }
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public void Read(byte[] sprm) {
			CalcAnchorTypes(sprm[0]);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			TableFloatingPosition position = propertyContainer.TableInfo.TableProperties.FloatingPosition;
			position.HorizontalAnchor = HorizontalAnchor;
			position.VerticalAnchor = VerticalAnchor;
			ParagraphFrameFormattingInfo frameFormattingInfo = propertyContainer.FrameInfo.FormattingInfo;
			frameFormattingInfo.HorizontalPositionType = DocHorizontalTypeToHorizontalType(HorizontalAnchor);
			frameFormattingInfo.VerticalPositionType = DocVerticalTypeToVerticalType(VerticalAnchor);
		}
		public ParagraphFrameHorizontalPositionType DocHorizontalTypeToHorizontalType(HorizontalAnchorTypes horizontalAnchor) {
			switch (horizontalAnchor) {
				case HorizontalAnchorTypes.Margin: return ParagraphFrameHorizontalPositionType.Margin;
				case HorizontalAnchorTypes.Page: return ParagraphFrameHorizontalPositionType.Page;
				default: return ParagraphFrameHorizontalPositionType.Column;
			}
		}
		public ParagraphFrameVerticalPositionType DocVerticalTypeToVerticalType(VerticalAnchorTypes verticalAnchorTypes) {
			switch (verticalAnchorTypes) {
				case VerticalAnchorTypes.Margin: return ParagraphFrameVerticalPositionType.Margin;
				case VerticalAnchorTypes.Page: return ParagraphFrameVerticalPositionType.Page;
				default: return ParagraphFrameVerticalPositionType.Paragraph;
			}
		}
		public HorizontalAnchorTypes HorizontalTypeToDocHorizontalType(ParagraphFrameHorizontalPositionType horizontalType) {
			switch (horizontalType) {
				case ParagraphFrameHorizontalPositionType.Margin: return HorizontalAnchorTypes.Margin;
				case ParagraphFrameHorizontalPositionType.Page: return HorizontalAnchorTypes.Page;
				default: return HorizontalAnchorTypes.Column;
			}
		}
		public VerticalAnchorTypes VerticalTypeToDocVerticalType(ParagraphFrameVerticalPositionType verticalType) {
			switch (verticalType) {
				case ParagraphFrameVerticalPositionType.Margin: return VerticalAnchorTypes.Margin;
				case ParagraphFrameVerticalPositionType.Page: return VerticalAnchorTypes.Page;
				default: return VerticalAnchorTypes.Paragraph;
			}
		}
		public void Write(BinaryWriter writer) {
			byte[] sprm = DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), new byte[] { GetAnchorTypeCodes() });
			writer.Write(sprm);
		}
		#endregion
		void CalcAnchorTypes(byte operand) {
			VerticalAnchor = CalcVerticalAnchor(operand);
			HorizontalAnchor = CalcHorizontalAnchor(operand);
		}
		VerticalAnchorTypes CalcVerticalAnchor(byte operand) {
			byte verticalAnchorTypeCode = (byte)((operand & 0x30) >> 4);
			switch (verticalAnchorTypeCode) {
				case 0: return VerticalAnchorTypes.Margin;
				case 1: return VerticalAnchorTypes.Page;
				case 2: return VerticalAnchorTypes.Paragraph;
				default: return VerticalAnchorTypes.Page;
			}
		}
		HorizontalAnchorTypes CalcHorizontalAnchor(byte operand) {
			byte horizontalAnchorTypeCode = (byte)((operand & 0xc0) >> 6);
			switch (horizontalAnchorTypeCode) {
				case 0: return HorizontalAnchorTypes.Column;
				case 1: return HorizontalAnchorTypes.Margin;
				case 2: return HorizontalAnchorTypes.Page;
				default: return HorizontalAnchorTypes.Column;
			}
		}
		byte GetAnchorTypeCodes() {
			return (byte)((GetVerticalAnchorTypeCode() << 4) | (GetHorizontalAnchorTypeCode() << 6));
		}
		byte GetVerticalAnchorTypeCode() {
			switch (VerticalAnchor) {
				case VerticalAnchorTypes.Margin: return 0;
				case VerticalAnchorTypes.Page: return 1;
				case VerticalAnchorTypes.Paragraph: return 2;
				default: return 3;
			}
		}
		byte GetHorizontalAnchorTypeCode() {
			switch (HorizontalAnchor) {
				case HorizontalAnchorTypes.Column: return 0;
				case HorizontalAnchorTypes.Margin: return 1;
				case HorizontalAnchorTypes.Page: return 2;
				default: return 3;
			}
		}
	}
	#endregion
	#region DocCommandTablePosition
	public class DocCommandTablePosition : DocCommandFramePosition {
		public DocCommandTablePosition() {
		}
		public DocCommandTablePosition(TableFloatingPositionInfo info) {
			HorizontalAnchor = info.HorizontalAnchor;
			VerticalAnchor = info.VerticalAnchor;
		}
	}
	#endregion
	#region TableStyleColBandSize
	public class DocCommandTableStyleColBandSize : DocCommandBytePropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableInfo.TableProperties.TableStyleColBandSize = Value;
		}
	}
	#endregion
	#region TableStyleRowBandSize
	public class DocCommandTableStyleRowBandSize : DocCommandBytePropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Table; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableInfo.TableProperties.TableStyleRowBandSize = Value;
		}
	}
	#endregion
	#region DocCommandCellRangeVerticalAlignment
	public class DocCommandCellRangeVerticalAlignment : IDocCommand {
		public DocCommandCellRangeVerticalAlignment() {
			CellRangeVerticalAlignment = new CellRangeVerticalAlignmentOperand();
		}
		public CellRangeVerticalAlignmentOperand CellRangeVerticalAlignment { get; set; }
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public void Read(byte[] sprm) {
			CellRangeVerticalAlignment = CellRangeVerticalAlignmentOperand.FromByteArray(sprm);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.CellRangeVerticalAlignmentActions.Add(CellRangeVerticalAlignment);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
			writer.Write(CellRangeVerticalAlignmentOperand.Size);
			CellRangeVerticalAlignment.Write(writer);
		}
		#endregion
	}
	#endregion
	public abstract class DocCommandTableRowBorderColorsBase : IDocCommand {
		List<DocTableBorderColorReference> colors;
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		protected List<DocTableBorderColorReference> Colors { get { return colors; } }
		public void Read(byte[] sprm) {
			int count = sprm.Length / DocColorReference.ColorReferenceSize;
			this.colors = new List<DocTableBorderColorReference>(count);
			for (int i = 0; i < count; i++)
				colors.Add(DocTableBorderColorReference.FromByteArray(sprm, i * DocColorReference.ColorReferenceSize));
		}
		public abstract void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader);
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
			for (int i = 0; i < this.colors.Count; i++)
				writer.Write(this.colors[i].GetBytes());
		}
	}
	public class DocCommandTopBorderColors : DocCommandTableRowBorderColorsBase {
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.TopBorders.AddRange(Colors);
		}
	}
	public class DocCommandBottomBorderColors : DocCommandTableRowBorderColorsBase {
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.BottomBorders.AddRange(Colors);
		}
	}
	public class DocCommandLeftBorderColors : DocCommandTableRowBorderColorsBase {
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.LeftBorders.AddRange(Colors);
		}
	}
	public class DocCommandRightBorderColors : DocCommandTableRowBorderColorsBase {
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.RightBorders.AddRange(Colors);
		}
	}
	#region DocCommandHideCellMark
	public class DocCommandHideCellMark : IDocCommand {
		public CellHideMarkOperand CellHideMark { get; set; }
		public DocCommandHideCellMark() {
			CellHideMark = new CellHideMarkOperand();
		}
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public void Read(byte[] sprm) {
			CellHideMark = CellHideMarkOperand.FromByteArray(sprm);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.CellHideMarkActions.Add(CellHideMark);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandFactory.GetOpcodeByType(this.GetType()));
			writer.Write(CellHideMarkOperand.Size);
			CellHideMark.Write(writer);
		}
		#endregion
	}
	#endregion
	#region DocParagraphTextWrapType
	public enum DocParagraphTextWrapType {
		Auto = 0x00,
		NotBeside = 0x01,
		Around = 0x02,
		None = 0x03,
		Tight = 0x04,
		Through = 0x05
	}
	#endregion
	#region DocWrapTypeCalculator
	public static class DocWrapTypeCalculator {
		public static DocParagraphTextWrapType MapToDocWrapTypeStyle(FloatingObjectTextWrapType wrapType) {
			switch (wrapType) {
				case FloatingObjectTextWrapType.TopAndBottom: return DocParagraphTextWrapType.NotBeside;
				case FloatingObjectTextWrapType.Square: return DocParagraphTextWrapType.Around;
				case FloatingObjectTextWrapType.Tight: return DocParagraphTextWrapType.Tight;
				case FloatingObjectTextWrapType.Through: return DocParagraphTextWrapType.Through;
				default: return DocParagraphTextWrapType.None;
			}
		}
		public static FloatingObjectTextWrapType MapToWrapTypeStyle(DocParagraphTextWrapType wrapType) {
			switch (wrapType) {
				case DocParagraphTextWrapType.NotBeside: return FloatingObjectTextWrapType.TopAndBottom;
				case DocParagraphTextWrapType.Around: return FloatingObjectTextWrapType.Square;
				case DocParagraphTextWrapType.Tight: return FloatingObjectTextWrapType.Tight;
				case DocParagraphTextWrapType.Through: return FloatingObjectTextWrapType.Through;
				default: return FloatingObjectTextWrapType.None;
			}
		}
		public static DocParagraphTextWrapType MapToDocWrapTypeStyle(ParagraphFrameTextWrapType wrapType) {
			switch (wrapType) {
				case ParagraphFrameTextWrapType.Auto: return DocParagraphTextWrapType.Auto;
				case ParagraphFrameTextWrapType.Around: return DocParagraphTextWrapType.Around;
				case ParagraphFrameTextWrapType.NotBeside: return DocParagraphTextWrapType.NotBeside;
				case ParagraphFrameTextWrapType.Through: return DocParagraphTextWrapType.Through;
				case ParagraphFrameTextWrapType.Tight: return DocParagraphTextWrapType.Tight;
				default: return DocParagraphTextWrapType.None;
			}
		}
		public static ParagraphFrameTextWrapType MapToParagraphFrameWrapTypeStyle(DocParagraphTextWrapType wrapType) {
			switch (wrapType) {
				case DocParagraphTextWrapType.Auto: return ParagraphFrameTextWrapType.Auto;
				case DocParagraphTextWrapType.Around: return ParagraphFrameTextWrapType.Around;
				case DocParagraphTextWrapType.NotBeside: return ParagraphFrameTextWrapType.NotBeside;
				case DocParagraphTextWrapType.Through: return ParagraphFrameTextWrapType.Through;
				case DocParagraphTextWrapType.Tight: return ParagraphFrameTextWrapType.Tight;
				default: return ParagraphFrameTextWrapType.None;
			}
		}
	}
	#endregion
	#region DocCommandFrameWrapType
	public class DocCommandFrameWrapType : IDocCommand {
		#region Fields
		DocParagraphTextWrapType wrapType;
		#endregion
		#region Properties
		public DocParagraphTextWrapType WrapType { get { return this.wrapType; } set { this.wrapType = value; } }
		#endregion
		public DocCommandFrameWrapType() {
		}
		#region IDocCommand Members
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Frame; } }
		public void Read(byte[] data) {
			this.wrapType = (DocParagraphTextWrapType)data[0];
		}
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), new byte[] { GetValue() }));
		}
		protected virtual byte GetValue() {
			return (byte)wrapType;
		}
		public virtual void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.FrameInfo.FormattingInfo.TextWrapType = DocWrapTypeCalculator.MapToParagraphFrameWrapTypeStyle(WrapType);
		}
		#endregion
	}
	#endregion
	#region DocCommandFrameHeight
	public class DocCommandFrameHeight : IDocCommand {
		ushort heightAbs;
		public int Value { get; set; }
		public bool MinHeight { get; set; }
		public void Read(byte[] sprm) {
			this.heightAbs = BitConverter.ToUInt16(sprm, 0);
			Value = heightAbs & 0x00007fff;
			MinHeight = (heightAbs & 0x08000) != 0;
		}
		public virtual void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.FrameInfo.FormattingInfo.Height = Value;
			if (Value != 0)
				propertyContainer.FrameInfo.FormattingInfo.HorizontalRule = MinHeight ? ParagraphFrameHorizontalRule.AtLeast : ParagraphFrameHorizontalRule.Exact;
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			this.heightAbs = (ushort)Value;
			if (MinHeight)
				this.heightAbs |= 0x08000;
			writer.Write(DocCommandHelper.CreateSinglePropertyModifier(DocCommandFactory.GetOpcodeByType(this.GetType()), BitConverter.GetBytes(this.heightAbs)));
		}
		public virtual ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Frame; } }
	}
	#endregion
	#region DocCommandFrameWidth
	public class DocCommandFrameWidth : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Frame; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.FrameInfo.FormattingInfo.Width = Value;
		}
	}
	#endregion
	#region DocCommandFrameHorizontalPosition
	public class DocCommandFrameHorizontalPosition : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Frame; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.FrameInfo.FormattingInfo.HorizontalPosition = Value;
			propertyContainer.FrameInfo.FormattingInfo.X = Value;
		}
	}
	#endregion
	#region DocCommandFrameVerticalPosition
	public class DocCommandFrameVerticalPosition : DocCommandShortPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.Frame; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.FrameInfo.FormattingInfo.VerticalPosition = Value;
			propertyContainer.FrameInfo.FormattingInfo.Y = Value;
		}
	}
	#endregion
	#region DocCommandTablePropertiesException
	public class DocCommandTablePropertiesException : IDocCommand {
		public DocCommandTablePropertiesException() {
			TableAutoformatLookSpecifier = new TLP();
		}
		public TLP TableAutoformatLookSpecifier { get; set; }
		public ChangeActionTypes ChangeAction { get { return ChangeActionTypes.TableRow; } }
		public void Read(byte[] sprm) {
			TableAutoformatLookSpecifier = TLP.FromByteArray(sprm);
		}
		public void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
			propertyContainer.TableRowInfo.TableAutoformatLookSpecifier = TableAutoformatLookSpecifier;
		}
		public void Write(BinaryWriter writer) {
			writer.Write(DocCommandFactory.GetOpcodeByType(typeof(DocCommandTablePropertiesException)));
			writer.Write(TLP.Size);
			TableAutoformatLookSpecifier.Write(writer);
		}
	}
	#endregion
	#region rsid
	public class DocCommandParagraphFormattingRsid : DocCommandIntPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.None; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
		}
	}
	public class DocCommandCharacterFormattingRsid : DocCommandIntPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.None; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
		}
	}
	public class DocCommandInsertTextRsid : DocCommandIntPropertyValueBase {
		public override ChangeActionTypes ChangeAction { get { return ChangeActionTypes.None; } }
		public override void Execute(DocPropertyContainer propertyContainer, BinaryReader dataStreamReader) {
		}
	}
	#endregion
}
