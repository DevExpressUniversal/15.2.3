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
using DevExpress.XtraRichEdit.Native;
using System.IO;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Export.Doc {
	public class DocParagraphPropertiesActions : IParagraphPropertiesActions, IDisposable {
		#region Fields
		BinaryWriter writer;
		ParagraphProperties properties;
		TabFormattingInfo tabs;
		NumberingListIndex numberingListIndex;
		int listLevelIndex;
		DocumentModelUnitConverter unitConverter;
		FrameProperties frameProperties;
		#endregion
		#region Constructors
		DocParagraphPropertiesActions(MemoryStream output) {
			Guard.ArgumentNotNull(output, "output");
			this.writer = new BinaryWriter(output);
		}
		public DocParagraphPropertiesActions(MemoryStream output, Paragraph paragraph)
			: this(output) {
			this.properties = paragraph.ParagraphProperties;
			this.tabs = paragraph.Tabs.Info;
			this.numberingListIndex = paragraph.GetOwnNumberingListIndex();
			this.listLevelIndex = paragraph.GetOwnListLevelIndex();
			this.unitConverter = paragraph.DocumentModel.UnitConverter;
			this.frameProperties = paragraph.FrameProperties;
		}
		public DocParagraphPropertiesActions(MemoryStream output, ParagraphProperties properties, TabFormattingInfo tabs)
			: this(output) {
			this.properties = properties;
			this.tabs = tabs;
			this.numberingListIndex = new NumberingListIndex(-1);
			this.unitConverter = properties.DocumentModel.UnitConverter;
		}
		public DocParagraphPropertiesActions(MemoryStream output, ListLevel listLevel)
			: this(output) {
			this.properties = listLevel.ParagraphProperties;
			this.numberingListIndex = new NumberingListIndex(-1);
			this.unitConverter = listLevel.DocumentModel.UnitConverter;
		}
		public DocParagraphPropertiesActions(MemoryStream output, ParagraphStyle style)
			: this(output) {
			this.properties = style.ParagraphProperties;
			this.tabs = style.Tabs.Info;
			this.numberingListIndex = style.GetNumberingListIndex();
			this.listLevelIndex = style.GetListLevelIndex();
			this.unitConverter = style.DocumentModel.UnitConverter;
		}
		#endregion
		#region Properties
		ParagraphProperties ParagraphProperties { get { return this.properties; } }
		TabFormattingInfo Tabs { get { return this.tabs; } }
		int ListLevelIndex { get { return this.listLevelIndex; } }
		NumberingListIndex NumberingListIndex { get { return this.numberingListIndex; } }
		DocumentModelUnitConverter UnitConverter { get { return this.unitConverter; } }
		FrameProperties FrameProperties { get { return this.frameProperties; } }
		#endregion
		public void CreateTableParagraphPropertyModifiers(int tableDepth) {
			InTableAction(tableDepth);
			CreateParagarphPropertyModifiers();
		}
		public void CreateParagarphPropertyModifiers() {
			ListAction();
			TabsAction();
			ParagraphPropertiesHelper.ForEach(this);
		}
		#region IParagraphPropertiesActions Members
		public void AlignmentAction() {
			if (!ParagraphProperties.UseAlignment)
				return;
			DocCommandAlignmentNew command = new DocCommandAlignmentNew();
			command.Alignment = ParagraphProperties.Info.Alignment;
			command.Write(writer);
		}
		public void FirstLineIndentAction() {
			if (!ParagraphProperties.UseFirstLineIndent)
				return;
			int value = (ParagraphProperties.Info.FirstLineIndentType == ParagraphFirstLineIndent.Indented) ? ParagraphProperties.Info.FirstLineIndent : (-1) * ParagraphProperties.Info.FirstLineIndent;
			DocCommandFirstLineIndent command = new DocCommandFirstLineIndent();
			command.Value = value;
			command.Write(writer);
			DocCommandFirstLineIndentNew commandNew = new DocCommandFirstLineIndentNew();
			commandNew.Value = value;
			commandNew.Write(writer);
		}
		public void FirstLineIndentTypeAction() {
		}
		public void LeftIndentAction() {
			if (!ParagraphProperties.UseLeftIndent)
				return;
			int value = ParagraphProperties.Info.LeftIndent;
			DocCommandLogicalLeftIndent logicalCommand = new DocCommandLogicalLeftIndent();
			logicalCommand.Value = value;
			logicalCommand.Write(writer);
			DocCommandPhysicalLeftIndent physicalCommand = new DocCommandPhysicalLeftIndent();
			physicalCommand.Value = value;
			physicalCommand.Write(writer);
		}
		public void LineSpacingAction() {
			if (!ParagraphProperties.UseLineSpacing)
				return;
			CalcLineSpacing();
		}
		public void LineSpacingTypeAction() {
		}
		public void RightIndentAction() {
			if (!ParagraphProperties.UseRightIndent)
				return;
			int value = ParagraphProperties.Info.RightIndent;
			DocCommandLogicalRightIndent logicalCommand = new DocCommandLogicalRightIndent();
			logicalCommand.Value = value;
			logicalCommand.Write(writer);
			DocCommandPhysicalRightIndent physicalCommand = new DocCommandPhysicalRightIndent();
			physicalCommand.Value = value;
			physicalCommand.Write(writer);
		}
		public void SpacingAfterAction() {
			if (!ParagraphProperties.UseSpacingAfter)
				return;
			DocCommandSpacingAfter command = new DocCommandSpacingAfter();
			command.Value = ParagraphProperties.Info.SpacingAfter;
			command.Write(writer);
		}
		public void SpacingBeforeAction() {
			if (!ParagraphProperties.UseSpacingBefore)
				return;
			DocCommandSpacingBefore command = new DocCommandSpacingBefore();
			command.Value = ParagraphProperties.Info.SpacingBefore;
			command.Write(writer);
		}
		public void SuppressHyphenationAction() {
			if (!ParagraphProperties.UseSuppressHyphenation)
				return;
			DocCommandSuppressHyphenation command = new DocCommandSuppressHyphenation();
			command.Value = ParagraphProperties.Info.SuppressHyphenation;
			command.Write(writer);
		}
		public void SuppressLineNumbersAction() {
			if (!ParagraphProperties.UseSuppressLineNumbers)
				return;
			DocCommandSuppressLineNumbers command = new DocCommandSuppressLineNumbers();
			command.Value = ParagraphProperties.Info.SuppressLineNumbers;
			command.Write(writer);
		}
		public void ContextualSpacingAction() {
			if (!ParagraphProperties.UseContextualSpacing)
				return;
			DocCommandContextualSpacing command = new DocCommandContextualSpacing();
			command.Value = ParagraphProperties.Info.ContextualSpacing;
			command.Write(writer);
		}
		public void PageBreakBeforeAction() {
			if (!ParagraphProperties.UsePageBreakBefore)
				return;
			DocCommandPageBreakBefore command = new DocCommandPageBreakBefore();
			command.Value = ParagraphProperties.Info.PageBreakBefore;
			command.Write(writer);
		}
		public void BeforeAutoSpacingAction() {
			if (!ParagraphProperties.UseBeforeAutoSpacing)
				return;
			DocCommandBeforeAutoSpacing command = new DocCommandBeforeAutoSpacing();
			command.Value = ParagraphProperties.Info.BeforeAutoSpacing;
			command.Write(writer);
		}
		public void AfterAutoSpacingAction() {
			if (!ParagraphProperties.UseAfterAutoSpacing)
				return;
			DocCommandAfterAutoSpacing command = new DocCommandAfterAutoSpacing();
			command.Value = ParagraphProperties.Info.AfterAutoSpacing;
			command.Write(writer);
		}
		public void KeepWithNextAction() {
			if (!ParagraphProperties.UseKeepWithNext)
				return;
			DocCommandKeepWithNext command = new DocCommandKeepWithNext();
			command.Value = ParagraphProperties.Info.KeepWithNext;
			command.Write(writer);
		}
		public void KeepLinesTogetherAction() {
			if (!ParagraphProperties.UseKeepLinesTogether)
				return;
			DocCommandKeepLinesTogether command = new DocCommandKeepLinesTogether();
			command.Value = ParagraphProperties.Info.KeepLinesTogether;
			command.Write(writer);
		}
		public void WidowOrphanControlAction() {
			if (!ParagraphProperties.UseWidowOrphanControl)
				return;
			DocCommandWidowOrphanControl command = new DocCommandWidowOrphanControl();
			command.Value = ParagraphProperties.Info.WidowOrphanControl;
			command.Write(writer);
		}
		public void OutlineLevelAction() {
			if (!ParagraphProperties.UseOutlineLevel)
				return;
			DocCommandOutlineLevel command = new DocCommandOutlineLevel();
			command.Value = (byte)ParagraphProperties.Info.OutlineLevel;
			command.Write(writer);
		}
		public void BackColorAction() {
			if (!ParagraphProperties.UseBackColor || ParagraphProperties.Info.BackColor == DXColor.Transparent)
				return;
			DocCommandParagraphShading command = new DocCommandParagraphShading();
			command.ShadingDescriptor.BackgroundColor = ParagraphProperties.Info.BackColor;
			command.Write(writer);
		}
		public void ShadingAction() {
		}
		public void FramePropertiesAction() {
			if (FrameProperties == null)
				return;
			DocCommandFrameWrapType frameWrapTypeCommand = new DocCommandFrameWrapType();
			frameWrapTypeCommand.WrapType = DocWrapTypeCalculator.MapToDocWrapTypeStyle(FrameProperties.TextWrapType);
			frameWrapTypeCommand.Write(writer);
			DocCommandFrameHeight frameHeightCommand = new DocCommandFrameHeight();
			frameHeightCommand.Value = FrameProperties.Height;
			frameHeightCommand.MinHeight = FrameProperties.HorizontalRule == ParagraphFrameHorizontalRule.AtLeast ? true : false;
			frameHeightCommand.Write(writer);
			DocCommandFrameWidth frameWidthCommand = new DocCommandFrameWidth();
			frameWidthCommand.Value = FrameProperties.Width;
			frameWidthCommand.Write(writer);
			DocCommandFrameHorizontalPosition horizontalPositionCommand = new DocCommandFrameHorizontalPosition();
			horizontalPositionCommand.Value = FrameProperties.HorizontalPosition;
			horizontalPositionCommand.Write(writer);
			DocCommandFrameVerticalPosition verticalPositionCommand = new DocCommandFrameVerticalPosition();
			verticalPositionCommand.Value = FrameProperties.VerticalPosition;
			verticalPositionCommand.Write(writer);
			DocCommandFramePosition framePositionCommand = new DocCommandFramePosition();
			framePositionCommand.HorizontalAnchor = framePositionCommand.HorizontalTypeToDocHorizontalType(FrameProperties.HorizontalPositionType);
			framePositionCommand.VerticalAnchor = framePositionCommand.VerticalTypeToDocVerticalType(FrameProperties.VerticalPositionType);
			framePositionCommand.Write(writer);
		}
		public void LeftBorderAction() {
			if (!ParagraphProperties.UseLeftBorder)
				return;
			DocCommandParagraphLeftBorderNew command = new DocCommandParagraphLeftBorderNew();
			command.CurrentBorder.ConvertFromBorderInfo(ParagraphProperties.LeftBorder, unitConverter);
			command.Write(writer);
		}
		public void RightBorderAction() {
			if (!ParagraphProperties.UseRightBorder)
				return;
			DocCommandParagraphRightBorderNew command = new DocCommandParagraphRightBorderNew();
			command.CurrentBorder.ConvertFromBorderInfo(ParagraphProperties.RightBorder, unitConverter);
			command.Write(writer);
		}
		public void TopBorderAction() {
			if (!ParagraphProperties.UseTopBorder)
				return;
			DocCommandParagraphTopBorderNew command = new DocCommandParagraphTopBorderNew();
			command.CurrentBorder.ConvertFromBorderInfo(ParagraphProperties.TopBorder, unitConverter);
			command.Write(writer);
		}
		public void BottomBorderAction() {
			if (!ParagraphProperties.UseBottomBorder)
				return;
			DocCommandParagraphBottomBorderNew command = new DocCommandParagraphBottomBorderNew();
			command.CurrentBorder.ConvertFromBorderInfo(ParagraphProperties.BottomBorder, unitConverter);
			command.Write(writer);
		}
		#endregion
		void InTableAction(int tableDepth) {
			DocCommandInTable inTableCommand = new DocCommandInTable();
			inTableCommand.Value = true;
			inTableCommand.Write(this.writer);
			DocCommandTableDepth tableDepthCommand = new DocCommandTableDepth();
			tableDepthCommand.Value = tableDepth;
			tableDepthCommand.Write(writer);
		}
		void TabsAction() {
			if (Tabs == null || Tabs.Count == 0)
				return;
			DocCommandChangeParagraphTabs command = new DocCommandChangeParagraphTabs();
			command.Tabs = Tabs;
			command.UnitConverter = UnitConverter;
			command.Write(this.writer);
		}
		void ListAction() {
			if (NumberingListIndex == NumberingListIndex.NoNumberingList) {
				DocCommandListInfoIndex listInfoCommand = new DocCommandListInfoIndex();
				listInfoCommand.Value = 0;
				listInfoCommand.Write(this.writer);
			}
			else {
				int numberingListIndex = ((IConvertToInt<NumberingListIndex>)NumberingListIndex).ToInt();
				if (numberingListIndex < 0)
					return;
				DocCommandListInfoIndex listInfoCommand = new DocCommandListInfoIndex();
				listInfoCommand.Value = numberingListIndex + 1;
				listInfoCommand.Write(this.writer);
				DocCommandListLevel levelCommand = new DocCommandListLevel();
				levelCommand.Value = (byte)ListLevelIndex;
				levelCommand.Write(writer);
			}
		}
		void CalcLineSpacing() {
			DocCommandLineSpacing command = new DocCommandLineSpacing();
			ParagraphLineSpacing type = ParagraphProperties.Info.LineSpacingType;
			command.LineSpacingType = CalcLineSpacingTypeCode(type);
			command.LineSpacing = CalcLineSpacing(type);
			command.Write(writer);
		}
		short CalcLineSpacingTypeCode(ParagraphLineSpacing type) {
			if (type == ParagraphLineSpacing.AtLeast || type == ParagraphLineSpacing.Exactly)
				return 0;
			return 1;
		}
		short CalcLineSpacing(ParagraphLineSpacing type) {
			int lineSpacing = (int)ParagraphProperties.Info.LineSpacing;
			switch (type) {
				case ParagraphLineSpacing.Single: return 240;
				case ParagraphLineSpacing.Sesquialteral: return 360;
				case ParagraphLineSpacing.Double: return 480;
				case ParagraphLineSpacing.Multiple: return (short)(ParagraphProperties.Info.LineSpacing * 240);
				case ParagraphLineSpacing.Exactly: return (short)(-1 * UnitConverter.ModelUnitsToTwips(lineSpacing));
				default: return (short)UnitConverter.ModelUnitsToTwips(lineSpacing);
			}
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				IDisposable resource = this.writer as IDisposable;
				if (resource != null) {
					resource.Dispose();
					resource = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
}
