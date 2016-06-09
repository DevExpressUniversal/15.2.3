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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfParagraphPropertiesExporter
	public class RtfParagraphPropertiesExporter : RtfPropertiesExporter {
		#region Fields
		const int DefaultParagraphFirstLineIndent = 0;
		const int DefaultParagraphLeftIndent = 0;
		const int DefaultParagraphRightIndent = 0;
		const bool DefaultSuppressHyphenation = false;
		const bool DefaultPageBreakBefore = false;
		const bool DefaultBeforeAutoSpacing = false;
		const bool DefaultAfterAutoSpacing = false;
		const bool DefaultKeepWithNext = false;
		const bool DefaultKeepLinesTogether = false;
		const bool DefaultWidowOrphanControl = true;
		const int DoubleIntervalRtfLineSpacingValue = 480;
		const int SesquialteralIntervalRtfLineSpacingValue = 360;
		const int SingleIntervalRtfLineSpacingValue = 240;
		const int AtLeastLineSpacingMultiple = 0;
		const int ExactlyLineSpacingMultiple = 0;
		const int MultipleLineSpacing = 1;
		const int DefaultParagraphSpacingBefore = 0;
		const int DefaultParagraphSpacingAfter = 0;
		#endregion
		public RtfParagraphPropertiesExporter(DocumentModel documentModel, IRtfExportHelper rtfExportHelper, RtfBuilder rtfBuilder)
			: base(documentModel, rtfExportHelper, rtfBuilder) {
		}
		public virtual void ExportParagraphProperties(Paragraph paragraph, int tableNestingLevel) {
			if (paragraph.IsInList())
				ExportParagraphNumberingProperties(paragraph);
			if (RtfExportHelper.SupportStyle && paragraph.ParagraphStyleIndex != ParagraphStyleCollection.EmptyParagraphStyleIndex)
				WriteParagraphStyle(paragraph.ParagraphStyle);
			if (paragraph.TopBorder.Style != BorderLineStyle.None)
				WriteParagraphBorder(paragraph.TopBorder, RtfExportSR.TopParagraphBorder);
			if (paragraph.LeftBorder.Style != BorderLineStyle.None)
				WriteParagraphBorder(paragraph.LeftBorder, RtfExportSR.LeftParagraphBorder);
			if (paragraph.BottomBorder.Style != BorderLineStyle.None)
				WriteParagraphBorder(paragraph.BottomBorder, RtfExportSR.BottomParagraphBorder);
			if (paragraph.RightBorder.Style != BorderLineStyle.None)
				WriteParagraphBorder(paragraph.RightBorder, RtfExportSR.RightParagraphBorder);
			if (paragraph.FrameProperties != null)
				WriteFrameProperties(paragraph.FrameProperties);
			WriteParagraphAlignment(paragraph.Alignment);
			WriteParagraphTableProperties(paragraph, tableNestingLevel);
			WriteParagraphGroupPropertiesId(paragraph);
			WriteParagraphIndents(paragraph.GetMergedParagraphProperties());
			WriteParagraphSuppressHyphenation(paragraph.SuppressHyphenation);
			WriteParagraphSuppressLineNumbers(paragraph.SuppressLineNumbers);
			WriteParagraphContextualSpacing(paragraph.ContextualSpacing);
			WriteParagraphPageBreakBefore(paragraph.PageBreakBefore);
			WriteParagraphBeforeAutoSpacing(paragraph.BeforeAutoSpacing);
			WriteParagraphAfterAutoSpacing(paragraph.AfterAutoSpacing);
			WriteParagraphKeepWithNext(paragraph.KeepWithNext);
			WriteParagraphKeepLinesTogether(paragraph.KeepLinesTogether);
			WriteParagraphWidowOrphanControl(paragraph.WidowOrphanControl);
			WriteParagraphOutlineLevel(paragraph.OutlineLevel);
			WriteParagraphBackColor(paragraph.BackColor);
			WriteParagraphLineSpacing(paragraph.LineSpacingType, paragraph.LineSpacing);
			WriteParagraphSpacingBefore(paragraph.SpacingBefore);
			WriteParagraphSpacingAfter(paragraph.SpacingAfter);
			WriteParagraphTabs(paragraph.GetTabs());
		}
		protected virtual void WriteParagraphGroupPropertiesId(Paragraph paragraph) {
			if (DocumentModel.WebSettings.IsBodyMarginsSet())
				RtfBuilder.WriteCommand(RtfExportSR.ParagraphGroupPropertiesId, 1);
		}
		public virtual void ExportParagraphPropertiesCore(MergedParagraphProperties properties) {
			ExportParagraphPropertiesCore(properties, false);
		}
		public virtual void ExportParagraphPropertiesCore(MergedParagraphProperties properties, bool checkDefaultAlignment) {
			ParagraphFormattingInfo info = properties.Info;
			if (info.TopBorder.Style != BorderLineStyle.None)
				WriteParagraphBorder(info.TopBorder, RtfExportSR.TopParagraphBorder);
			if (info.LeftBorder.Style != BorderLineStyle.None)
				WriteParagraphBorder(info.LeftBorder, RtfExportSR.LeftParagraphBorder);
			if (info.BottomBorder.Style != BorderLineStyle.None)
				WriteParagraphBorder(info.BottomBorder, RtfExportSR.BottomParagraphBorder);
			if (info.RightBorder.Style != BorderLineStyle.None)
				WriteParagraphBorder(info.RightBorder, RtfExportSR.RightParagraphBorder);
			if (!checkDefaultAlignment || info.Alignment != ParagraphAlignment.Left)
				WriteParagraphAlignment(info.Alignment);
			WriteParagraphIndents(properties);
			WriteParagraphSuppressHyphenation(info.SuppressHyphenation);
			WriteParagraphSuppressLineNumbers(info.SuppressLineNumbers);
			WriteParagraphContextualSpacing(info.ContextualSpacing);
			WriteParagraphPageBreakBefore(info.PageBreakBefore);
			WriteParagraphBeforeAutoSpacing(info.BeforeAutoSpacing);
			WriteParagraphAfterAutoSpacing(info.AfterAutoSpacing);
			WriteParagraphKeepWithNext(info.KeepWithNext);
			WriteParagraphKeepLinesTogether(info.KeepLinesTogether);
			WriteParagraphWidowOrphanControl(info.WidowOrphanControl);
			WriteParagraphOutlineLevel(info.OutlineLevel);
			WriteParagraphBackColor(info.BackColor);
			WriteParagraphLineSpacing(info.LineSpacingType, info.LineSpacing);
			WriteParagraphSpacingBefore(info.SpacingBefore);
			WriteParagraphSpacingAfter(info.SpacingAfter);
		}
		protected virtual void ExportParagraphNumberingProperties(Paragraph paragraph) {
			if (!paragraph.ShouldExportNumbering())
				return;
			RtfBuilder.WriteCommand(RtfExportSR.LevelIndex, paragraph.GetListLevelIndex());
			WriteParagraphListIndex(paragraph.GetNumberingListIndex());
		}
		#region WriteParagraphTableProperties
		void WriteParagraphTableProperties(Paragraph paragraph, int nestingLevel) {
			if (!paragraph.DocumentModel.DocumentCapabilities.TablesAllowed)
				return;
			if (nestingLevel > 0)
				RtfBuilder.WriteCommand(RtfExportSR.InTableParagraph);
			if (nestingLevel > 1)
				RtfBuilder.WriteCommand(RtfExportSR.ParagraphNestingLevel, nestingLevel);
		}
		#endregion
		#region WriteParagraphAlignment
		protected internal void WriteParagraphAlignment(ParagraphAlignment alignment) {
			switch (alignment) {
				case ParagraphAlignment.Left:
					RtfBuilder.WriteCommand(RtfExportSR.LeftAlignment);
					break;
				case ParagraphAlignment.Center:
					RtfBuilder.WriteCommand(RtfExportSR.CenterAlignment);
					break;
				case ParagraphAlignment.Justify:
					RtfBuilder.WriteCommand(RtfExportSR.JustifyAlignment);
					break;
				case ParagraphAlignment.Right:
					RtfBuilder.WriteCommand(RtfExportSR.RightAlignment);
					break;
			}
		}
		#endregion
		#region WriteParagraphBorder
		protected internal void WriteParagraphBorder(BorderInfo topBorder, string command) {
			BorderInfo defaultBorder = DocumentModel.Cache.BorderInfoCache.DefaultItem;
			if (topBorder != defaultBorder) {
				RtfBuilder.WriteCommand(command);
				WriteBorderProperties(topBorder);
			}
		}
		#endregion
		#region WriteFrameProperties
		protected internal void WriteFrameProperties(FrameProperties properties) {
			if (properties.UseVerticalPositionType)
				WriteParagraphVerticalPositionType(properties.VerticalPositionType);
			if (properties.UseHorizontalPositionType)
				WriteParagraphHorizontalPositionType(properties.HorizontalPositionType);
			if (properties.UseHorizontalPosition)
				RtfBuilder.WriteCommand(RtfExportSR.FrameHorizontalPosition, properties.HorizontalPosition);
			if (properties.UseVerticalPosition)
				RtfBuilder.WriteCommand(RtfExportSR.FrameVerticalPosition, properties.VerticalPosition);
			if (properties.UseHeight) {
				if (properties.HorizontalRule == ParagraphFrameHorizontalRule.Exact)
					RtfBuilder.WriteCommand(RtfExportSR.FrameHeight, -properties.Height);
				else
					RtfBuilder.WriteCommand(RtfExportSR.FrameHeight, properties.Height);
			}
			if (properties.UseWidth)
				RtfBuilder.WriteCommand(RtfExportSR.FrameWidth, properties.Width);
			if (properties.UseTextWrapType)
				WriteParagraphWrapType(properties.TextWrapType);
		}
		#endregion
		#region WriteParagraphVerticalPositionType
		protected internal void WriteParagraphVerticalPositionType(ParagraphFrameVerticalPositionType positionType) {
			switch (positionType) {
				case ParagraphFrameVerticalPositionType.Margin:
					RtfBuilder.WriteCommand(RtfExportSR.ParagraphVerticalPositionTypeMargin);
					break;
				case ParagraphFrameVerticalPositionType.Page:
					RtfBuilder.WriteCommand(RtfExportSR.ParagraphVerticalPositionTypePage);
					break;
				case ParagraphFrameVerticalPositionType.Paragraph:
					RtfBuilder.WriteCommand(RtfExportSR.ParagraphVerticalPositionTypeLine);
					break;
			}
		}
		#endregion
		#region WriteParagraphHorizontalPositionType
		protected internal void WriteParagraphHorizontalPositionType(ParagraphFrameHorizontalPositionType positionType) {
			switch (positionType) {
				case ParagraphFrameHorizontalPositionType.Margin:
					RtfBuilder.WriteCommand(RtfExportSR.ParagraphHorizontalPositionTypeMargin);
					break;
				case ParagraphFrameHorizontalPositionType.Page:
					RtfBuilder.WriteCommand(RtfExportSR.ParagraphHorizontalPositionTypePage);
					break;
				case ParagraphFrameHorizontalPositionType.Column:
					RtfBuilder.WriteCommand(RtfExportSR.ParagraphHorizontalPositionTypeColumn);
					break;
			}
		}
		#endregion
		#region WriteParagraphWrapType
		protected internal virtual void WriteParagraphWrapType(ParagraphFrameTextWrapType wrapType) {
			switch (wrapType) {
				case ParagraphFrameTextWrapType.Auto:
					break;
				case ParagraphFrameTextWrapType.Around:
					RtfBuilder.WriteCommand(RtfExportSR.FrameWrapAround);
					break;
				case ParagraphFrameTextWrapType.None:
					RtfBuilder.WriteCommand(RtfExportSR.FrameWrapOverlay);
					break;
				case ParagraphFrameTextWrapType.NotBeside:
					RtfBuilder.WriteCommand(RtfExportSR.FrameNoWrap);
					break;
				case ParagraphFrameTextWrapType.Through:
					RtfBuilder.WriteCommand(RtfExportSR.FrameWrapThrough);
					break;
				case ParagraphFrameTextWrapType.Tight:
					RtfBuilder.WriteCommand(RtfExportSR.FrameWrapTight);
					break;
			}
		}
		#endregion
		#region WriteParagraphIndents
		protected internal void WriteParagraphIndents(MergedParagraphProperties mergedParagraphProperties) {
			ParagraphFormattingInfo paragraphPropertiesInfo = mergedParagraphProperties.Info;
			int firstLineIndent = CalcRtfFirstLineIndent(paragraphPropertiesInfo.FirstLineIndentType, paragraphPropertiesInfo.FirstLineIndent);
			if (firstLineIndent != DefaultParagraphFirstLineIndent)
				RtfBuilder.WriteCommand(RtfExportSR.FirstLineIndentInTwips, firstLineIndent);
			int leftIndent = CalcRtfLeftIndent(paragraphPropertiesInfo.FirstLineIndentType, paragraphPropertiesInfo.FirstLineIndent, paragraphPropertiesInfo.LeftIndent);
			if (leftIndent != DefaultParagraphLeftIndent) {
				RtfBuilder.WriteCommand(RtfExportSR.LeftIndentInTwips, leftIndent);
				RtfBuilder.WriteCommand(RtfExportSR.LeftIndentInTwips_Lin, leftIndent);
			}
			int rightIndent = CalcRtfRightIndent(paragraphPropertiesInfo.RightIndent);
			if (rightIndent != DefaultParagraphRightIndent) {
				RtfBuilder.WriteCommand(RtfExportSR.RightIndentInTwips, rightIndent);
				RtfBuilder.WriteCommand(RtfExportSR.RightIndentInTwips_Rin, rightIndent);
			}
		}
		#endregion
		#region WriteParagraphSuppressHyphenation
		protected internal void WriteParagraphSuppressHyphenation(bool paragraphSuppressHyphenation) {
			if (paragraphSuppressHyphenation == DefaultSuppressHyphenation)
				return;
			RtfBuilder.WriteCommand(RtfExportSR.AutomaticParagraphHyphenation, paragraphSuppressHyphenation ? 0 : 1);
		}
		#endregion
		protected internal void WriteParagraphSuppressLineNumbers(bool paragraphSuppressLineNumbers) {
			if (paragraphSuppressLineNumbers)
				RtfBuilder.WriteCommand(RtfExportSR.SuppressLineNumbering);
		}
		protected internal void WriteParagraphContextualSpacing(bool value) {
			if (value)
				RtfBuilder.WriteCommand(RtfExportSR.ContextualSpacing);
		}
		protected internal void WriteParagraphPageBreakBefore(bool value) {
			if (value == DefaultPageBreakBefore)
				return;
			RtfBuilder.WriteCommand(RtfExportSR.PageBreakBefore, value ? 1 : 0);
		}
		protected internal void WriteParagraphBeforeAutoSpacing(bool value) {
			if (value == DefaultBeforeAutoSpacing)
				return;
			RtfBuilder.WriteCommand(RtfExportSR.BeforeAutoSpacing, value ? 1 : 0);
		}
		protected internal void WriteParagraphAfterAutoSpacing(bool value) {
			if (value == DefaultAfterAutoSpacing)
				return;
			RtfBuilder.WriteCommand(RtfExportSR.AfterAutoSpacing, value ? 1 : 0);
		}
		protected internal void WriteParagraphKeepWithNext(bool value) {
			if (value == DefaultKeepWithNext)
				return;
			RtfBuilder.WriteCommand(RtfExportSR.KeepWithNext, value ? 1 : 0);
		}
		protected internal void WriteParagraphKeepLinesTogether(bool value) {
			if (value == DefaultKeepLinesTogether)
				return;
			RtfBuilder.WriteCommand(RtfExportSR.KeepLinesTogether, value ? 1 : 0);
		}
		protected internal void WriteParagraphWidowOrphanControl(bool value) {
			if (value == DefaultWidowOrphanControl)
				return;
			if (value)
				RtfBuilder.WriteCommand(RtfExportSR.WidowOrphanControlOn);
			else
				RtfBuilder.WriteCommand(RtfExportSR.WidowOrphanControlOff);
		}
		protected internal void WriteParagraphOutlineLevel(int outlineLevel) {
			int level = outlineLevel;
			if (level < 0 || level >= 10)
				return;
			if (outlineLevel > 0)
				RtfBuilder.WriteCommand(RtfExportSR.OutlineLevel, outlineLevel - 1);
		}
		protected internal void WriteParagraphBackColor(Color value) {
			if (DXColor.IsTransparentOrEmpty(value))
				return;
			int colorIndex = RtfExportHelper.GetColorIndex(value);
			RtfBuilder.WriteCommand(RtfExportSR.ParagraphBackgroundColor, colorIndex);
		}
		#region WriteParagraphLineSpacing
		protected internal void WriteParagraphLineSpacing(ParagraphLineSpacing paragraphLineSpacingType, float paragraphLineSpacing) {
			switch (paragraphLineSpacingType) {
				case ParagraphLineSpacing.AtLeast:
					WriteRtfLineSpacing(UnitConverter.ModelUnitsToTwips((int)paragraphLineSpacing), AtLeastLineSpacingMultiple);
					break;
				case ParagraphLineSpacing.Exactly:
					WriteRtfLineSpacing(-UnitConverter.ModelUnitsToTwips((int)paragraphLineSpacing), ExactlyLineSpacingMultiple);
					break;
				case ParagraphLineSpacing.Double:
					WriteRtfLineSpacing(DoubleIntervalRtfLineSpacingValue, MultipleLineSpacing);
					break;
				case ParagraphLineSpacing.Sesquialteral:
					WriteRtfLineSpacing(SesquialteralIntervalRtfLineSpacingValue, MultipleLineSpacing);
					break;
				case ParagraphLineSpacing.Multiple:
					WriteRtfLineSpacing((int)(paragraphLineSpacing * SingleIntervalRtfLineSpacingValue), MultipleLineSpacing);
					break;
			}
		}
		#endregion
		#region WriteRtfLineSpacing
		protected internal void WriteRtfLineSpacing(int rtfLineSpacingValue, int rtfLineSpacingMultiple) {
			RtfBuilder.WriteCommand(RtfExportSR.RtfLineSpacingValue, rtfLineSpacingValue);
			RtfBuilder.WriteCommand(RtfExportSR.RtfLineSpacingMultiple, rtfLineSpacingMultiple);
		}
		#endregion
		#region WriteParagraphSpacingBefore
		protected internal void WriteParagraphSpacingBefore(int spacingBefore) {
			if (spacingBefore == DefaultParagraphSpacingBefore)
				return;
			RtfBuilder.WriteCommand(RtfExportSR.SpaceBefore, UnitConverter.ModelUnitsToTwips(spacingBefore));
		}
		#endregion
		#region WriteParagraphSpacingAfter
		protected internal void WriteParagraphSpacingAfter(int spacingAfter) {
			if (spacingAfter == DefaultParagraphSpacingAfter)
				return;
			RtfBuilder.WriteCommand(RtfExportSR.SpaceAfter, UnitConverter.ModelUnitsToTwips(spacingAfter));
		}
		#endregion
		#region WriteParagraphTabs
		void WriteParagraphTabs(TabFormattingInfo tabFormattingInfo) {
			int count = tabFormattingInfo.Count;
			for (int i = 0; i < count; i++)
				WriteParagraphTab(tabFormattingInfo[i]);
		}
		#endregion
		#region WriteParagraphTab
		void WriteParagraphTab(TabInfo tabInfo) {
			WriteTabKind(tabInfo.Alignment);
			WriteTabLeader(tabInfo.Leader);
			WriteTabPosition(tabInfo.Position);
		}
		void WriteTabKind(TabAlignmentType alignmentType) {
			switch (alignmentType) {
				case TabAlignmentType.Center:
					RtfBuilder.WriteCommand(RtfExportSR.CenteredTab);
					break;
				case TabAlignmentType.Decimal:
					RtfBuilder.WriteCommand(RtfExportSR.DecimalTab);
					break;
				case TabAlignmentType.Right:
					RtfBuilder.WriteCommand(RtfExportSR.FlushRightTab);
					break;
				case TabAlignmentType.Left:
					break;
			}
		}
		void WriteTabLeader(TabLeaderType leaderType) {
			switch (leaderType) {
				case TabLeaderType.Dots:
					RtfBuilder.WriteCommand(RtfExportSR.TabLeaderDots);
					break;
				case TabLeaderType.EqualSign:
					RtfBuilder.WriteCommand(RtfExportSR.TabLeaderEqualSign);
					break;
				case TabLeaderType.Hyphens:
					RtfBuilder.WriteCommand(RtfExportSR.TabLeaderHyphens);
					break;
				case TabLeaderType.MiddleDots:
					RtfBuilder.WriteCommand(RtfExportSR.TabLeaderMiddleDots);
					break;
				case TabLeaderType.ThickLine:
					RtfBuilder.WriteCommand(RtfExportSR.TabLeaderThickLine);
					break;
				case TabLeaderType.Underline:
					RtfBuilder.WriteCommand(RtfExportSR.TabLeaderUnderline);
					break;
				case TabLeaderType.None:
					break;
			}
		}
		void WriteTabPosition(int position) {
			RtfBuilder.WriteCommand(RtfExportSR.TabPosition, UnitConverter.ModelUnitsToTwips(position));
		}
		#endregion
		#region WriteParagraphListIndex
		protected virtual void WriteParagraphListIndex(NumberingListIndex index) {
			RtfBuilder.WriteCommand(RtfExportSR.ListIndex, DocumentModel.NumberingLists[index].Id);
		}
		#endregion
		#region WriteParagraphStyle
		void WriteParagraphStyle(ParagraphStyle paragraphStyle) {
			string styleName = paragraphStyle.StyleName;
			Dictionary<string, int> styleCollection = RtfExportHelper.ParagraphStylesCollectionIndex;
			if (styleCollection.ContainsKey(styleName))
				RtfBuilder.WriteCommand(RtfExportSR.ParagraphStyle, styleCollection[styleName]);
		}
		#endregion
		#region CalcRtfFirstLineIndent
		int CalcRtfFirstLineIndent(ParagraphFirstLineIndent firstLineIndentType, int firstLineIndent) {
			switch (firstLineIndentType) {
				case ParagraphFirstLineIndent.Indented:
					return UnitConverter.ModelUnitsToTwips(firstLineIndent);
				case ParagraphFirstLineIndent.Hanging:
					return -UnitConverter.ModelUnitsToTwips(firstLineIndent);
				case ParagraphFirstLineIndent.None:
				default:
					return 0;
			}
		}
		#endregion
		#region CalcRtfLeftIndent
		int CalcRtfLeftIndent(ParagraphFirstLineIndent firstLineIndentType, int firstLineIndent, int leftIndent) {
			return UnitConverter.ModelUnitsToTwips(leftIndent);
		}
		#endregion
		#region CalcRtfRightIndent
		int CalcRtfRightIndent(int rightIndent) {
			return UnitConverter.ModelUnitsToTwips(rightIndent);
		}
		#endregion
	}
	#endregion
}
