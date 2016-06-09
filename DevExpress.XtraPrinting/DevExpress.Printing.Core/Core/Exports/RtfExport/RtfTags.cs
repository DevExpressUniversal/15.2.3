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

namespace DevExpress.XtraPrinting.Export.Rtf {
	public static class RtfTags {
		public static bool IsRtfContent(string rtf) {
			return (rtf.Contains(RtfHeader) && rtf.IndexOf(RtfHeader) == 0);
		}
		public static string WrapTextInRtf(string text) {
			return "{\\rtf1 " + text + "}";
		}
		public static string RtfHeader { get { return @"{\rtf"; } }
		public static string ParagraphEnd { get { return @"\par"; } }
		public static string ParagraphDefault { get { return @"\pard"; } }
		public static string SectionEnd { get { return "\\sect \r\n"; } }
		public static string SectionDefault { get { return @"\sectd"; } }
		public static string SectionNoBreak { get { return @"\sbknone"; } }
		public static string SpaceBetweenColumns { get { return @"\colsx30"; } }
		public static string ColumnCount { get { return @"\cols{0}"; } }
		public static string PlainText { get { return @"\plain"; } }
		public static string RightIndent { get { return @"\ri{0}"; } }
		public static string LeftIndent { get { return @"\li{0}"; } }
		public static string SpaceBefore { get { return @"\sb{0}"; } }
		public static string SpaceAfter { get { return @"\sa{0}"; } }
		public static string RightToLeftParagraph { get { return @"\rtlpar"; } }
		public static string DefaultFrame { get { return @"\frmtxlrtb"; } }
		public static string TopBottomFrame { get { return @"\frmtxtbrl"; } }
		public static string BottomTopFrame { get { return @"\frmtxbtlr"; } }
		public static string RelativeFrameToPage { get { return @"\pvpg"; } }
		public static string ObjectBounds { get { return @"\posx{0}\posy{1}\absw{2}\absh{3}"; } }
		public static string ObjectPosition { get { return @"\posx{0}\posy{1}"; } }
		public static string ObjectSize { get { return @"\absw{0}\absh{1}"; } }
		public static string TextAround { get { return @"\nowrap"; } }
		public static string TextUnderneath { get { return @"\overlay"; } }
		public static string StartWithCodePage { get { return @"{{\rtf1\ansicpg{0}"; } }
		public static string ColorTable { get { return @"\colortbl"; } }
		public static string NumberingListTable { get { return @"\*\listtable"; } }
		public static string ListOverrideTable { get { return @"\*\listoverridetable"; } }
		public static string FontTable { get { return @"\fonttbl"; } }
		public static string DefineFont { get { return @"{{\f{0} {1};}}"; } }
		public static string RGB { get { return @"\red{0}\green{1}\blue{2};"; } }
		public static string TopAlignedInCell { get { return @"\clvertalt"; } }
		public static string VerticalCenteredInCell { get { return @"\clvertalc"; } }
		public static string BottomAlignedInCell { get { return @"\clvertalb"; } }
		public static string Centered { get { return @"\qc"; } }
		public static string Justified { get { return @"\qj"; } }
		public static string LeftAligned { get { return @"\ql"; } }
		public static string RightAligned { get { return @"\qr"; } }
		public static string UnicodeCharacter { get { return @"\u{0}"; } }
		public static string WindowsMetafile { get { return @"\wmetafile{0}"; } }
		public static string EnhancedMetafile { get { return @"\emfblip"; } }
		public static string PngPictType { get { return @"\pngblip";} }
		public static string Picture { get { return @"\pict{0}"; } }
		public static string PictureWidth { get { return @"\picw{0}"; } }
		public static string PictureHeight { get { return @"\pich{0}"; } }
		public static string PictureDesiredWidth { get { return @"\picwgoal{0}"; } }
		public static string PictureDesiredHeight { get { return @"\pichgoal{0}"; } }
		public static string PictureCropTop { get { return @"\piccropt{0}"; } }
		public static string PictureCropBottom { get { return @"\piccropb{0}"; } }
		public static string PictureCropLeft { get { return @"\piccropl{0}"; } }
		public static string PictureCropRight { get { return @"\piccropr{0}"; } }
		public static string HorizontalScaling { get { return @"\picscalex{0}"; } }
		public static string VerticalScaling { get { return @"\picscaley{0}"; } }
		public static string BackgroundPatternBackgroundColor { get { return @"\clcbpat{0}"; } }
		public static string BackgroundPatternColor { get { return @"\cbpat{0}"; } }
		public static string Color { get { return @"\cf{0}"; } }
		public static string SingleBorderWidth { get { return @"\brdrs"; } } 
		public static string DoubleBorderWidth { get { return @"\brdrth"; } }
		public static string DotBorderStyle { get { return @"\brdrdot"; } }
		public static string DashBorderStyle { get { return @"\brdrdash"; } }
		public static string DashDotBorderStyle { get { return @"\brdrdashd"; } }
		public static string DashDotDotBorderStyle { get { return @"\brdrdashdd"; } }
		public static string DoubleBorderStyle { get { return @"\brdrdb"; } }
		public static string TopBorder { get { return @"\brdrt"; } }
		public static string BottomBorder { get { return @"\brdrb"; } }
		public static string LeftBorder { get { return @"\brdrl"; } }
		public static string RightBorder { get { return @"\brdrr"; } }
		public static string BorderColor { get { return @"\brdrcf{0}"; } }
		public static string BorderWidth { get { return @"\brdrw{0}"; } }
		public static string LeftCellBorder { get { return @"\clbrdrl"; } }
		public static string RightCellBorder { get { return @"\clbrdrr"; } }
		public static string TopCellBorder { get { return @"\clbrdrt"; } }
		public static string BottomCellBorder { get { return @"\clbrdrb"; } }
		public static string SuggestToTable { get { return @"\intbl"; } }
		public static string LeftToRightWrite { get { return @"\ltrrow"; } }
		public static string EndOfRow { get { return @"\intbl\row"; } }
		public static string Hyperlink { get { return "{{\\field{{\\*\\fldinst{{HYPERLINK \"{0}\" }}}}{{\\fldrslt{{{1}}}}}}}"; } }
		public static string LocalHyperlink { get { return "{{\\field{{\\*\\fldinst{{HYPERLINK \\\\l \"{0}\" }}}}{{\\fldrslt{{{1}}}}}}}"; } }
		public static string Bookmark { get { return "{{\\*\\bkmkstart {0}}}{{\\*\\bkmkend {0}}}"; } }
		public static string EndOfLine { get { return @"\line "; } }
		public static string FirstMergedCell { get { return @"\clmgf "; } }
		public static string MergedCell { get { return @"\clmrg "; } }
		public static string FirstVerticalMergedCell { get { return @"\clvmgf "; } }
		public static string VerticalMergedCell { get { return @"\clvmrg "; } }
		public static string EmptyCell { get { return @"{\cell}"; } }
		public static string EndOfCell { get { return @"\cell"; } }
		public static string ExactlyRowHeight { get { return @"\trrh-{0}"; } }
		public static string AtLeastRowHeight { get { return @"\trrh{0}"; } }
		public static string RowHeight { get { return @"\trrh-{1}"; } }
		public static string DefaultRow { get { return @"\trowd"; } }
		public static string RowAutofit { get { return @"\trautofit1"; } }
		public static string CellRight { get { return @"\cellx"; } }
		public static string FontWithSize { get { return @"\f{0}\fs{1}"; } }
		public static string FontSize { get { return @"\fs{0}"; } }
		public static string Bold { get { return @"\b"; } }
		public static string Italic { get { return @"\i"; } }
		public static string Underline { get { return @"\ul"; } }
		public static string Strikeout { get { return @"\strike"; } }
		public static string PageSize { get { return @"\paperw{0}\paperh{1}"; } }
		public static string PageLandscape { get { return @"\lndscpsxn"; } }
		public static string Margins { get { return @"\margl{0}\margr{1}\margt{2}\margb{3}"; } }
		public static string NoneWrapShape { get { return @"\shpwr3"; } }
		public static string Shape { get { return @"{\shp"; } }
		public static string ShapeInstall { get { return @"{\*\shpinst"; } }
		public static string TextShape { get { return @"{\sp{\sn shapeType}{\sv 136}}"; } }
		public static string ImageShape { get { return @"{\sp{\sn shapeType}{\sv 75}}"; } }
		public static string ShapeRotation { get { return "{{\\sp{{\\sn rotation}}{{\\sv {0}}}}}"; } }
		public static string ShapeBounds { get { return @"\shpleft{0}\shptop{1}\shpright{2}\shpbottom{3}"; } }
		public static string UnicodeShapeString { get { return "{{\\sp{{\\sn gtextUNICODE}}{{\\sv {0}}}}}"; } }
		public static string ShapeFont { get { return "{{\\sp{{\\sn gtextFont}}{{\\sv {0}}}}}"; } }
		public static string ShapeColor { get { return "{{\\sp{{\\sn fillColor}}{{\\sv {0}}}}}"; } }
		public static string ShapeOpacity { get { return "{{\\sp{{\\sn fillOpacity}}{{\\sv {0}}}}}"; } }
		public static string NoShapeBorderLine { get { return @"{\sp{\sn fLine}{\sv 0}}"; } }
		public static string ShapeBehind { get { return "{{\\sp{{\\sn fBehindDocument}}{{\\sv {0}}}}}"; } }
		public static string ShapeBoldText { get { return "{{\\sp{{\\sn gtextFBold}}{{\\sv {0}}}}}"; } }
		public static string ShapeItalicText { get { return "{{\\sp{{\\sn gtextFItalic}}{{\\sv {0}}}}}"; } }
		public static string ShapePicture { get { return "{{\\sp{{\\sn pib}}{{\\sv {0}}}}}"; } }
		public static string NewLine { get { return "\r\n"; } }
		public static string Header { get { return @"\header"; } }
		public static string HeaderY { get { return @"\headery{0}"; } }
		public static string Footer { get { return @"\footer"; } }
		public static string FooterY { get { return @"\footery{0}"; } }
		public static string SpecialFirstPageHeaderFooter { get { return @" \titlepg"; } }
		public static string PageBreak { get { return @"\pagebb"; } }
		public static string FieldInstructionTemplate { get { return @"{{\field{{\*\fldinst{{ {0} }}}}{{\fldrslt {1}}}}}"; } }
		public static string FieldInstructionPageNumber { get { return @"PAGE"; } }
		public static string FieldInstructionPageCount { get { return @"NUMPAGES"; } }
		public static string FieldInstructionUserName { get { return @"USERNAME"; } }
		public static string FieldInstructionDate { get { return @"DATE"; } }  
		public static string NoGrowAutoFit { get { return @"\nogrowautofit"; } }
	}
}
