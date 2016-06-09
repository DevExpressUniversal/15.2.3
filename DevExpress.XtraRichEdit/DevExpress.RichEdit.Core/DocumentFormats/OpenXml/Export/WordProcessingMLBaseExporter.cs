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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Office.NumberConverters;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.OpenXml {
	public abstract class WordProcessingMLBaseExporter : XmlBasedDocumentModelExporter {
		#region Fields
		readonly DocumentExporterOptions options;
		XmlWriter documentContentWriter;
		Section currentSection;
		int imageCounter;
		public const string OfficeNamespaceConst = "urn:schemas-microsoft-com:office:office";
		public const string W10MLNamespace = "urn:schemas-microsoft-com:office:word";
		public const string W10MLPrefix = "w10";
		public const string OPrefix = "o";
		public const string W15Prefix = "w15";
		public const string W15NamespaceConst = "http://schemas.microsoft.com/office/word/2012/wordml";
		public const string W14Prefix = "w14";
		public const string W14NamespaceConst = "http://schemas.microsoft.com/office/word/2010/wordml";
		public const string APrefix = "a";
		public const string DrawingMLNamespaceConst = "http://schemas.openxmlformats.org/drawingml/2006/main";
		const int transparentClr = 16777215;
		int drawingElementId = 1;
		bool forbidExecuteBaseExportTableCell; 
		#endregion
		protected WordProcessingMLBaseExporter(DocumentModel documentModel, DocumentExporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
		}
		#region Translation tables
		internal static readonly Dictionary<Color, WordProcessingMLValue> predefinedBackgroundColors = CreatePredefinedBackgroundColors();
		internal static readonly Dictionary<string, Color> presetColors = CreatePresetColors();
		internal static readonly Dictionary<UnderlineType, WordProcessingMLValue> underlineTable = CreateUnderlineTable();
		internal static readonly Dictionary<char, WordProcessingMLValue> runBreaksTable = CreateRunBreaksTable();
		internal static readonly Dictionary<ParagraphAlignment, WordProcessingMLValue> paragraphAlignmentTable = CreateParagraphAlignmentTable();
		internal static readonly Dictionary<ParagraphLineSpacing, WordProcessingMLValue> lineSpacingTable = CreateParagraphLineSpacingTable();
		internal static readonly Dictionary<TabAlignmentType, WordProcessingMLValue> tabAlignmentTable = CreateTabAlignmentTable();
		internal static readonly Dictionary<TabLeaderType, WordProcessingMLValue> tabLeaderTable = CreateTabLeaderTable();
		internal static readonly Dictionary<TextDirection, WordProcessingMLValue> textDirectionTable = CreateTextDirectionTable();
		internal static readonly Dictionary<ShadingPattern, WordProcessingMLValue> shadingPatternTable = CreateShadingPatternTable();
		internal static readonly Dictionary<VerticalAlignment, WordProcessingMLValue> verticalAlignmentTable = CreateVerticalAlignmentTable();
		internal static readonly Dictionary<SectionStartType, WordProcessingMLValue> sectionStartTypeTable = CreateSectionStartTypeTable();
		internal static readonly Dictionary<LineNumberingRestart, WordProcessingMLValue> lineNumberingRestartTable = CreateLineNumberingRestartTable();
		internal static readonly Dictionary<NumberingFormat, WordProcessingMLValue> pageNumberingFormatTable = CreatePageNumberingFormatTable();
		internal static readonly Dictionary<char, WordProcessingMLValue> chapterSeparatorsTable = CreateChapterSeparatorsTable();
		internal static readonly Dictionary<NumberingType, WordProcessingMLValue> numberingListTypeTable = CreateNumberingListTypeTable();
		internal static readonly Dictionary<ListNumberAlignment, WordProcessingMLValue> listNumberAlignmentTable = CreateListNumberAlignmentTable();
		internal static readonly Dictionary<char, WordProcessingMLValue> listNumberSeparatorTable = CreateListNumberSeparatorTable();
		internal static readonly Dictionary<string, string> contentTypeTable = CreateContentTypeTable();
		internal static readonly Dictionary<WidthUnitType, WordProcessingMLValue> widthUnitTypesTable = CreateWidthUnitTypesTable();
		internal static readonly Dictionary<TableLayoutType, WordProcessingMLValue> tableLayoutTypeTable = CreateTableLayoutTypeTable();
		internal static readonly Dictionary<BorderLineStyle, WordProcessingMLValue> borderLineStyleTable = CreateBorderLineStyleTable();
		internal static readonly Dictionary<HorizontalAlignMode, WordProcessingMLValue> horizontalAlignModeTable = CreateHorizontalAlignModeTable();
		internal static readonly Dictionary<HorizontalAnchorTypes, WordProcessingMLValue> horizontalAnchorTypesTable = CreateHorizontalAnchorTypesTable();
		internal static readonly Dictionary<VerticalAlignMode, WordProcessingMLValue> verticalAlignModeTable = CreateVerticalAlignModeTable();
		internal static readonly Dictionary<VerticalAnchorTypes, WordProcessingMLValue> verticalAnchorTypesTable = CreateVerticalAnchorTypesTable();
		internal static readonly Dictionary<HeightUnitType, WordProcessingMLValue> heightUnitTypeTable = CreateHeightUnitTypeTable();
		internal static readonly Dictionary<MergingState, WordProcessingMLValue> mergingStateTable = CreateMergingStateTable();
		internal static readonly Dictionary<TableRowAlignment, WordProcessingMLValue> tableRowAlignmentTable = CreateTableRowAlignmentTable();
		internal static readonly Dictionary<FootNotePosition, WordProcessingMLValue> footNotePlacementTable = CreateFootNotePlacementTable();
		internal static readonly Dictionary<FloatingObjectHorizontalPositionAlignment, WordProcessingMLValue> floatingObjectHorizontalPositionAlignmentTable = CreateFloatingObjectHorizontalPositionAlignmentTable();
		internal static readonly Dictionary<FloatingObjectVerticalPositionAlignment, WordProcessingMLValue> floatingObjectVerticalPositionAlignmentTable = CreateFloatingObjectVerticalPositionAlignmentTable();
		internal static readonly Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> floatingObjectHorizontalPositionTypeTable = CreateFloatingObjectHorizontalPositionTypeTable();
		internal static readonly Dictionary<FloatingObjectRelativeFromHorizontal, WordProcessingMLValue> floatingObjectCssRelativeFromHorizontalTable = CreateFloatingObjectCssRelativeFromHorizontalTable();
		internal static readonly Dictionary<FloatingObjectRelativeFromVertical, WordProcessingMLValue> floatingObjectCssRelativeFromVerticalTable = CreateFloatingObjectCssRelativeFromVerticalTable();
		internal static readonly Dictionary<FloatingObjectRelativeFromHorizontal, WordProcessingMLValue> floatingObjectRelativeFromHorizontalTable = CreateFloatingObjectRelativeFromHorizontalTable();
		internal static readonly Dictionary<FloatingObjectRelativeFromVertical, WordProcessingMLValue> floatingObjectRelativeFromVerticalTable = CreateFloatingObjectRelativeFromVerticalTable();
		internal static readonly Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> floatingObjectVerticalPositionTypeTable = CreateFloatingObjectVerticalPositionTypeTable();
		internal static readonly Dictionary<FloatingObjectTextWrapSide, WordProcessingMLValue> floatingObjectTextWrapSideTable = CreateFloatingObjectTextWrapSideTable();
		internal static readonly Dictionary<FloatingObjectTextWrapType, WordProcessingMLValue> floatingObjectTextWrapTypeTable = CreateFloatingObjectTextWrapTypeTable();
		internal static readonly Dictionary<VerticalAlignment, WordProcessingMLValue> textBoxVerticalAlignmentTable = CreateTextBoxVerticalAlignmentTable();
		internal static readonly Dictionary<ConditionalTableStyleFormattingTypes, WordProcessingMLValue> conditionalTableStyleFormattingTypesTable = CreateConditionalTableStyleFormattingTypesTable();
		internal static readonly Dictionary<string, string> predefinedGroupNames = CreatePredefinedGroupNames();
		internal static readonly Dictionary<FontTypeHint, string> fontTypeHintTable = CreateFontTypeHintTable();
		internal static readonly Dictionary<ThemeFontType, string> themeFontTypeTable = CreateThemeFontTypeTable();
		internal static readonly Dictionary<ThemeColorIndex, WordProcessingMLValue> themeColorIndexTable = CreateThemeColorIndexTable();
		static void Add<T>(Dictionary<T, WordProcessingMLValue> table, T key, string openXmlValue) {
			table.Add(key, new WordProcessingMLValue(openXmlValue));
		}
		#region CreateFontTypeHintTable
		static Dictionary<FontTypeHint, string> CreateFontTypeHintTable() {
			Dictionary<FontTypeHint, string> result = new Dictionary<FontTypeHint, string>();
			result.Add(FontTypeHint.Cs, "cs");
			result.Add(FontTypeHint.Default, "default");
			result.Add(FontTypeHint.EastAsia, "eastAsia");
			result.Add(FontTypeHint.None, "none");
			return result;
		}
		#endregion
		#region CreateThemeFontTypeTable
		static Dictionary<ThemeFontType, string> CreateThemeFontTypeTable() {
			Dictionary<ThemeFontType, string> result = new Dictionary<ThemeFontType, string>();
			result.Add(ThemeFontType.MajorAscii, "majorAscii");
			result.Add(ThemeFontType.MajorBidi, "majorBidi");
			result.Add(ThemeFontType.MajorEastAsia, "majorEastAsia");
			result.Add(ThemeFontType.MajorHAnsi, "majorHAnsi");
			result.Add(ThemeFontType.MinorAscii, "minorAscii");
			result.Add(ThemeFontType.MinorBidi, "minorBidi");
			result.Add(ThemeFontType.MinorEastAsia, "minorEastAsia");
			result.Add(ThemeFontType.MinorHAnsi, "minorHAnsi");
			result.Add(ThemeFontType.None, "none");
			return result;
		}
		#endregion
		#region CreateThemeColorIndexTable
		static Dictionary<ThemeColorIndex, WordProcessingMLValue> CreateThemeColorIndexTable() {
			Dictionary<ThemeColorIndex, WordProcessingMLValue> result = new Dictionary<ThemeColorIndex, WordProcessingMLValue>();
			Add(result, ThemeColorIndex.Accent1, "accent1");
			Add(result, ThemeColorIndex.Accent2, "accent2");
			Add(result, ThemeColorIndex.Accent3, "accent3");
			Add(result, ThemeColorIndex.Accent4, "accent4");
			Add(result, ThemeColorIndex.Accent5, "accent5");
			Add(result, ThemeColorIndex.Accent6, "accent6");
			Add(result, ThemeColorIndex.Dark1, "dark1");
			Add(result, ThemeColorIndex.Dark2, "dark2");
			Add(result, ThemeColorIndex.FollowedHyperlink, "followedHyperlink");
			Add(result, ThemeColorIndex.Hyperlink, "hyperlink");
			Add(result, ThemeColorIndex.Light1, "light1");
			Add(result, ThemeColorIndex.Light2, "light2");
			Add(result, ThemeColorIndex.None, "none");
			Add(result, ThemeColorIndex.Background1, "bg1");
			Add(result, ThemeColorIndex.Background2, "bg2");
			Add(result, ThemeColorIndex.Text1, "t1");
			Add(result, ThemeColorIndex.Text2, "t2");
			return result;
		}
		#endregion
		#region CreateUnderlineTable
		static Dictionary<UnderlineType, WordProcessingMLValue> CreateUnderlineTable() {
			Dictionary<UnderlineType, WordProcessingMLValue> result;
			result = new Dictionary<UnderlineType, WordProcessingMLValue>();
			Add(result, UnderlineType.None, "none");
			Add(result, UnderlineType.Single, "single");
			Add(result, UnderlineType.Double, "double");
			Add(result, UnderlineType.Dotted, "dotted");
			Add(result, UnderlineType.Dashed, "dash");
			Add(result, UnderlineType.LongDashed, "dashLong");
			Add(result, UnderlineType.DashDotted, "dotDash");
			Add(result, UnderlineType.DashDotDotted, "dotDotDash");
			Add(result, UnderlineType.DoubleWave, "wavyDouble");
			Add(result, UnderlineType.HeavyWave, "wavyHeavy");
			Add(result, UnderlineType.ThickDashDotDotted, "dashDotDotHeavy");
			Add(result, UnderlineType.ThickDashDotted, "dashDotHeavy");
			Add(result, UnderlineType.ThickDashed, "dashedHeavy");
			Add(result, UnderlineType.ThickDotted, "dottedHeavy");
			Add(result, UnderlineType.ThickLongDashed, "dashLongHeavy");
			Add(result, UnderlineType.ThickSingle, "thick");
			Add(result, UnderlineType.Wave, "wave");
			return result;
		}
		#endregion
		#region CreatePredefinedBackgroundColors
		static Dictionary<Color, WordProcessingMLValue> CreatePredefinedBackgroundColors() {
			Dictionary<Color, WordProcessingMLValue> result = new Dictionary<Color, WordProcessingMLValue>();
			Add(result, DXColor.Empty, "none");
			Add(result, DXColor.Black, "black");
			Add(result, DXColor.FromArgb(0x00, 0x00, 0xFF), "blue");
			Add(result, DXColor.FromArgb(0x00, 0xFF, 0xFF), "cyan");
			Add(result, DXColor.FromArgb(0x00, 0x00, 0x80), "darkBlue");
			Add(result, DXColor.FromArgb(0x00, 0x80, 0x80), "darkCyan");
			Add(result, DXColor.FromArgb(0x80, 0x80, 0x80), "darkGray");
			Add(result, DXColor.FromArgb(0x00, 0x80, 0x00), "darkGreen");
			Add(result, DXColor.FromArgb(0x80, 0x00, 0x80), "darkMagenta");
			Add(result, DXColor.FromArgb(0x80, 0x00, 0x00), "darkRed");
			Add(result, DXColor.FromArgb(0x80, 0x80, 0x00), "darkYellow");
			Add(result, DXColor.FromArgb(0x00, 0xFF, 0x00), "green");
			Add(result, DXColor.FromArgb(0xC0, 0xC0, 0xC0), "lightGray");
			Add(result, DXColor.FromArgb(0xFF, 0x00, 0xFF), "magenta");
			Add(result, DXColor.FromArgb(0xFF, 0x00, 0x00), "red");
			Add(result, DXColor.FromArgb(0xFF, 0xFF, 0xFF), "white");
			Add(result, DXColor.FromArgb(0xFF, 0xFF, 0x00), "yellow");
			return result;
		}
		#endregion
		#region CreatePresetColors
		static Dictionary<string, Color> CreatePresetColors() {
			Dictionary<string, Color> result = new Dictionary<string, Color>();
			result.Add("aliceBlue", DXColor.FromArgb(240, 248, 255));
			result.Add("antiqueWhite", DXColor.FromArgb(250, 235, 215));
			result.Add("aqua", DXColor.FromArgb(0, 255, 255));
			result.Add("aquamarine", DXColor.FromArgb(127, 255, 212));
			result.Add("azure", DXColor.FromArgb(240, 255, 255));
			result.Add("beige", DXColor.FromArgb(245, 245, 220));
			result.Add("bisque", DXColor.FromArgb(255, 228, 196));
			result.Add("black", DXColor.FromArgb(0, 0, 0));
			result.Add("blanchedAlmond", DXColor.FromArgb(255, 235, 205));
			result.Add("blue", DXColor.FromArgb(0, 0, 255));
			result.Add("blueViolet", DXColor.FromArgb(138, 43, 226));
			result.Add("brown", DXColor.FromArgb(165, 42, 42));
			result.Add("burlyWood", DXColor.FromArgb(222, 184, 135));
			result.Add("cadetBlue", DXColor.FromArgb(95, 158, 160));
			result.Add("chartreuse", DXColor.FromArgb(127, 255, 0));
			result.Add("chocolate", DXColor.FromArgb(210, 105, 30));
			result.Add("coral", DXColor.FromArgb(255, 127, 80));
			result.Add("cornflowerBlue", DXColor.FromArgb(100, 149, 237));
			result.Add("cornsilk", DXColor.FromArgb(255, 248, 220));
			result.Add("crimson", DXColor.FromArgb(220, 20, 60));
			result.Add("cyan", DXColor.FromArgb(0, 255, 255));
			result.Add("deepPink", DXColor.FromArgb(255, 20, 147));
			result.Add("deepSkyBlue", DXColor.FromArgb(0, 191, 255));
			result.Add("dimGray", DXColor.FromArgb(105, 105, 105));
			result.Add("dkBlue", DXColor.FromArgb(0, 0, 139));
			result.Add("dkCyan", DXColor.FromArgb(0, 139, 139));
			result.Add("dkGoldenrod", DXColor.FromArgb(184, 134, 11));
			result.Add("dkGray", DXColor.FromArgb(169, 169, 169));
			result.Add("dkGreen", DXColor.FromArgb(0, 100, 0));
			result.Add("dkKhaki", DXColor.FromArgb(189, 183, 107));
			result.Add("dkMagenta", DXColor.FromArgb(139, 0, 139));
			result.Add("dkOliveGreen", DXColor.FromArgb(85, 107, 47));
			result.Add("dkOrange", DXColor.FromArgb(255, 140, 0));
			result.Add("dkOrchid", DXColor.FromArgb(153, 50, 204));
			result.Add("dkRed", DXColor.FromArgb(139, 0, 0));
			result.Add("dkSalmon", DXColor.FromArgb(233, 150, 122));
			result.Add("dkSeaGreen", DXColor.FromArgb(143, 188, 139));
			result.Add("dkSlateBlue", DXColor.FromArgb(72, 61, 139));
			result.Add("dkSlateGray", DXColor.FromArgb(47, 79, 79));
			result.Add("dkTurquoise", DXColor.FromArgb(0, 206, 209));
			result.Add("dkViolet", DXColor.FromArgb(148, 0, 211));
			result.Add("dodgerBlue", DXColor.FromArgb(30, 144, 255));
			result.Add("firebrick", DXColor.FromArgb(178, 34, 34));
			result.Add("floralWhite", DXColor.FromArgb(255, 250, 240));
			result.Add("forestGreen", DXColor.FromArgb(34, 139, 34));
			result.Add("fuchsia", DXColor.FromArgb(255, 0, 255));
			result.Add("gainsboro", DXColor.FromArgb(220, 220, 220));
			result.Add("ghostWhite", DXColor.FromArgb(248, 248, 255));
			result.Add("gold", DXColor.FromArgb(255, 215, 0));
			result.Add("goldenrod", DXColor.FromArgb(218, 165, 32));
			result.Add("gray", DXColor.FromArgb(128, 128, 128));
			result.Add("green", DXColor.FromArgb(0, 128, 0));
			result.Add("greenYellow", DXColor.FromArgb(173, 255, 47));
			result.Add("honeydew", DXColor.FromArgb(240, 255, 240));
			result.Add("hotPink", DXColor.FromArgb(255, 105, 180));
			result.Add("indianRed", DXColor.FromArgb(205, 92, 92));
			result.Add("indigo", DXColor.FromArgb(75, 0, 130));
			result.Add("ivory", DXColor.FromArgb(255, 255, 240));
			result.Add("khaki", DXColor.FromArgb(240, 230, 140));
			result.Add("lavender", DXColor.FromArgb(230, 230, 250));
			result.Add("lavenderBlush", DXColor.FromArgb(255, 240, 245));
			result.Add("lawnGreen", DXColor.FromArgb(124, 252, 0));
			result.Add("lemonChiffon", DXColor.FromArgb(255, 250, 205));
			result.Add("lime", DXColor.FromArgb(0, 255, 0));
			result.Add("limeGreen", DXColor.FromArgb(50, 205, 50));
			result.Add("linen", DXColor.FromArgb(250, 240, 230));
			result.Add("ltBlue", DXColor.FromArgb(173, 216, 230));
			result.Add("ltCoral", DXColor.FromArgb(240, 128, 128));
			result.Add("ltCyan", DXColor.FromArgb(224, 255, 255));
			result.Add("ltGoldenrodYellow", DXColor.FromArgb(250, 250, 120));
			result.Add("ltGray", DXColor.FromArgb(211, 211, 211));
			result.Add("ltGreen", DXColor.FromArgb(144, 238, 144));
			result.Add("ltPink", DXColor.FromArgb(255, 182, 193));
			result.Add("ltSalmon", DXColor.FromArgb(255, 160, 122));
			result.Add("ltSeaGreen", DXColor.FromArgb(32, 178, 170));
			result.Add("ltSkyBlue", DXColor.FromArgb(135, 206, 250));
			result.Add("ltSlateGray", DXColor.FromArgb(119, 136, 153));
			result.Add("ltSteelBlue", DXColor.FromArgb(176, 196, 222));
			result.Add("ltYellow", DXColor.FromArgb(255, 255, 224));
			result.Add("magenta", DXColor.FromArgb(255, 0, 255));
			result.Add("maroon", DXColor.FromArgb(128, 0, 0));
			result.Add("medAquamarine", DXColor.FromArgb(102, 205, 170));
			result.Add("medBlue", DXColor.FromArgb(0, 0, 205));
			result.Add("medOrchid", DXColor.FromArgb(186, 85, 211));
			result.Add("medPurple", DXColor.FromArgb(147, 112, 219));
			result.Add("medSeaGreen", DXColor.FromArgb(60, 179, 113));
			result.Add("medSlateBlue", DXColor.FromArgb(123, 104, 238));
			result.Add("medSpringGreen", DXColor.FromArgb(0, 250, 154));
			result.Add("medTurquoise", DXColor.FromArgb(72, 209, 204));
			result.Add("medVioletRed", DXColor.FromArgb(199, 21, 133));
			result.Add("midnightBlue", DXColor.FromArgb(25, 25, 112));
			result.Add("mintCream", DXColor.FromArgb(245, 255, 250));
			result.Add("mistyRose", DXColor.FromArgb(255, 228, 225));
			result.Add("moccasin", DXColor.FromArgb(255, 228, 181));
			result.Add("navajoWhite", DXColor.FromArgb(255, 222, 173));
			result.Add("navy", DXColor.FromArgb(0, 0, 128));
			result.Add("oldLace", DXColor.FromArgb(253, 245, 230));
			result.Add("olive", DXColor.FromArgb(128, 128, 0));
			result.Add("oliveDrab", DXColor.FromArgb(107, 142, 35));
			result.Add("orange", DXColor.FromArgb(255, 165, 0));
			result.Add("orangeRed", DXColor.FromArgb(255, 69, 0));
			result.Add("orchid", DXColor.FromArgb(218, 112, 214));
			result.Add("paleGoldenrod", DXColor.FromArgb(238, 232, 170));
			result.Add("paleGreen", DXColor.FromArgb(152, 251, 152));
			result.Add("paleTurquoise", DXColor.FromArgb(175, 238, 238));
			result.Add("paleVioletRed", DXColor.FromArgb(219, 112, 147));
			result.Add("papayaWhip", DXColor.FromArgb(255, 239, 213));
			result.Add("peachPuff", DXColor.FromArgb(255, 218, 185));
			result.Add("peru", DXColor.FromArgb(205, 133, 63));
			result.Add("pink", DXColor.FromArgb(255, 192, 203));
			result.Add("plum", DXColor.FromArgb(221, 160, 221));
			result.Add("powderBlue", DXColor.FromArgb(176, 224, 230));
			result.Add("purple", DXColor.FromArgb(128, 0, 128));
			result.Add("red", DXColor.FromArgb(255, 0, 0));
			result.Add("rosyBrown", DXColor.FromArgb(188, 143, 143));
			result.Add("royalBlue", DXColor.FromArgb(65, 105, 225));
			result.Add("saddleBrown", DXColor.FromArgb(139, 69, 19));
			result.Add("salmon", DXColor.FromArgb(250, 128, 114));
			result.Add("sandyBrown", DXColor.FromArgb(244, 164, 96));
			result.Add("seaGreen", DXColor.FromArgb(46, 139, 87));
			result.Add("seaShell", DXColor.FromArgb(255, 245, 238));
			result.Add("sienna", DXColor.FromArgb(160, 82, 45));
			result.Add("silver", DXColor.FromArgb(192, 192, 192));
			result.Add("skyBlue", DXColor.FromArgb(135, 206, 235));
			result.Add("slateBlue", DXColor.FromArgb(106, 90, 205));
			result.Add("slateGray", DXColor.FromArgb(112, 128, 144));
			result.Add("snow", DXColor.FromArgb(255, 250, 250));
			result.Add("springGreen", DXColor.FromArgb(0, 255, 127));
			result.Add("steelBlue", DXColor.FromArgb(70, 130, 180));
			result.Add("tan", DXColor.FromArgb(210, 180, 140));
			result.Add("teal", DXColor.FromArgb(0, 128, 128));
			result.Add("thistle", DXColor.FromArgb(216, 191, 216));
			result.Add("tomato", DXColor.FromArgb(255, 99, 71));
			result.Add("turquoise", DXColor.FromArgb(64, 224, 208));
			result.Add("violet", DXColor.FromArgb(238, 130, 238));
			result.Add("wheat", DXColor.FromArgb(245, 222, 179));
			result.Add("white", DXColor.FromArgb(255, 255, 255));
			result.Add("whiteSmoke", DXColor.FromArgb(245, 245, 245));
			result.Add("yellow", DXColor.FromArgb(255, 255, 0));
			result.Add("yellowGreen", DXColor.FromArgb(154, 205, 50));
			return result;
		}
		#endregion
		#region CreateRunBreaksTable
		static Dictionary<char, WordProcessingMLValue> CreateRunBreaksTable() {
			Dictionary<char, WordProcessingMLValue> result = new Dictionary<char, WordProcessingMLValue>();
			Add(result, Characters.LineBreak, "textWrapping");
			Add(result, Characters.PageBreak, "page");
			Add(result, Characters.ColumnBreak, "column");
			return result;
		}
		#endregion
		#region CreateParagraphAlignmentTable
		static Dictionary<ParagraphAlignment, WordProcessingMLValue> CreateParagraphAlignmentTable() {
			Dictionary<ParagraphAlignment, WordProcessingMLValue> result = new Dictionary<ParagraphAlignment, WordProcessingMLValue>();
			Add(result, ParagraphAlignment.Left, "left");
			Add(result, ParagraphAlignment.Right, "right");
			Add(result, ParagraphAlignment.Center, "center");
			Add(result, ParagraphAlignment.Justify, "both");
			return result;
		}
		#endregion
		#region CreateParagraphLineSpacingTable
		static Dictionary<ParagraphLineSpacing, WordProcessingMLValue> CreateParagraphLineSpacingTable() {
			Dictionary<ParagraphLineSpacing, WordProcessingMLValue> result = new Dictionary<ParagraphLineSpacing, WordProcessingMLValue>();
			Add(result, ParagraphLineSpacing.Single, "auto");
			Add(result, ParagraphLineSpacing.Double, "auto");
			Add(result, ParagraphLineSpacing.Sesquialteral, "auto");
			Add(result, ParagraphLineSpacing.Multiple, "auto");
			Add(result, ParagraphLineSpacing.Exactly, "exact");
			Add(result, ParagraphLineSpacing.AtLeast, "atLeast");
			return result;
		}
		#endregion
		#region CreateTabAlignmentTable
		static Dictionary<TabAlignmentType, WordProcessingMLValue> CreateTabAlignmentTable() {
			Dictionary<TabAlignmentType, WordProcessingMLValue> result = new Dictionary<TabAlignmentType, WordProcessingMLValue>();
			Add(result, TabAlignmentType.Left, "left");
			Add(result, TabAlignmentType.Right, "right");
			Add(result, TabAlignmentType.Center, "center");
			Add(result, TabAlignmentType.Decimal, "decimal");
			return result;
		}
		#endregion
		#region CreateTabAlignmentTable
		static Dictionary<TabLeaderType, WordProcessingMLValue> CreateTabLeaderTable() {
			Dictionary<TabLeaderType, WordProcessingMLValue> result = new Dictionary<TabLeaderType, WordProcessingMLValue>();
			Add(result, TabLeaderType.None, "none");
			Add(result, TabLeaderType.Dots, "dot");
			Add(result, TabLeaderType.Hyphens, "hyphen");
			Add(result, TabLeaderType.EqualSign, "hyphen");
			Add(result, TabLeaderType.MiddleDots, "middleDot");
			Add(result, TabLeaderType.ThickLine, "heavy");
			Add(result, TabLeaderType.Underline, "underscore");
			return result;
		}
		#endregion
		#region CreateTextDirectionTable
		static Dictionary<TextDirection, WordProcessingMLValue> CreateTextDirectionTable() {
			Dictionary<TextDirection, WordProcessingMLValue> result = new Dictionary<TextDirection, WordProcessingMLValue>();
			Add(result, TextDirection.LeftToRightTopToBottom, "lrTb");
			Add(result, TextDirection.LeftToRightTopToBottomRotated, "lrTbV");
			Add(result, TextDirection.BottomToTopLeftToRight, "btLr");
			Add(result, TextDirection.TopToBottomLeftToRightRotated, "tbLrV");
			Add(result, TextDirection.TopToBottomRightToLeft, "tbRl");
			Add(result, TextDirection.TopToBottomRightToLeftRotated, "tbRlV");
			return result;
		}
		#endregion
		#region CreateShadingPatternTable
		static Dictionary<ShadingPattern, WordProcessingMLValue> CreateShadingPatternTable() {
			Dictionary<ShadingPattern, WordProcessingMLValue> result = new Dictionary<ShadingPattern, WordProcessingMLValue>();
			Add(result, ShadingPattern.Clear, "clear");
			Add(result, ShadingPattern.DiagCross, "diagCross");
			Add(result, ShadingPattern.DiagStripe, "diagStripe");
			Add(result, ShadingPattern.HorzCross, "horzCross");
			Add(result, ShadingPattern.HorzStripe, "horzStripe");
			Add(result, ShadingPattern.Nil, "nil");
			Add(result, ShadingPattern.Pct10, "pct10");
			Add(result, ShadingPattern.Pct12, "pct12");
			Add(result, ShadingPattern.Pct15, "pct15");
			Add(result, ShadingPattern.Pct20, "pct20");
			Add(result, ShadingPattern.Pct25, "pct25");
			Add(result, ShadingPattern.Pct30, "pct30");
			Add(result, ShadingPattern.Pct35, "pct35");
			Add(result, ShadingPattern.Pct37, "pct37");
			Add(result, ShadingPattern.Pct40, "pct40");
			Add(result, ShadingPattern.Pct45, "pct45");
			Add(result, ShadingPattern.Pct5, "pct5");
			Add(result, ShadingPattern.Pct50, "pct50");
			Add(result, ShadingPattern.Pct55, "pct55");
			Add(result, ShadingPattern.Pct60, "pct60");
			Add(result, ShadingPattern.Pct62, "pct62");
			Add(result, ShadingPattern.Pct65, "pct65");
			Add(result, ShadingPattern.Pct70, "pct70");
			Add(result, ShadingPattern.Pct75, "pct75");
			Add(result, ShadingPattern.Pct80, "pct80");
			Add(result, ShadingPattern.Pct85, "pct85");
			Add(result, ShadingPattern.Pct87, "pct87");
			Add(result, ShadingPattern.Pct90, "pct90");
			Add(result, ShadingPattern.Pct95, "pct95");
			Add(result, ShadingPattern.ReverseDiagStripe, "reverseDiagStripe");
			Add(result, ShadingPattern.Solid, "solid");
			Add(result, ShadingPattern.ThinDiagCross, "thinDiagCross");
			Add(result, ShadingPattern.ThinDiagStripe, "ThinDiagStripe");
			Add(result, ShadingPattern.ThinHorzCross, "thinHorzCross");
			Add(result, ShadingPattern.ThinHorzStripe, "thinHorzStripe");
			Add(result, ShadingPattern.ThinReverseDiagStripe, "thinReverseDiagStripe");
			Add(result, ShadingPattern.ThinVertStripe, "thinVertStripe");
			Add(result, ShadingPattern.VertStripe, "vertStripe");
			return result;
		}
		#endregion
		#region CreateVerticalAlignmentTable
		static Dictionary<VerticalAlignment, WordProcessingMLValue> CreateVerticalAlignmentTable() {
			Dictionary<VerticalAlignment, WordProcessingMLValue> result = new Dictionary<VerticalAlignment, WordProcessingMLValue>();
			Add(result, VerticalAlignment.Top, "top");
			Add(result, VerticalAlignment.Center, "center");
			Add(result, VerticalAlignment.Bottom, "bottom");
			Add(result, VerticalAlignment.Both, "both");
			return result;
		}
		#endregion
		#region CreateSectionStartTypeTable
		static Dictionary<SectionStartType, WordProcessingMLValue> CreateSectionStartTypeTable() {
			Dictionary<SectionStartType, WordProcessingMLValue> result = new Dictionary<SectionStartType, WordProcessingMLValue>();
			Add(result, SectionStartType.NextPage, "nextPage");
			Add(result, SectionStartType.OddPage, "oddPage");
			Add(result, SectionStartType.EvenPage, "evenPage");
			Add(result, SectionStartType.Column, "nextColumn");
			Add(result, SectionStartType.Continuous, "continuous");
			return result;
		}
		#endregion
		#region CreateLineNumberingRestartTable
		static Dictionary<LineNumberingRestart, WordProcessingMLValue> CreateLineNumberingRestartTable() {
			Dictionary<LineNumberingRestart, WordProcessingMLValue> result = new Dictionary<LineNumberingRestart, WordProcessingMLValue>();
			Add(result, LineNumberingRestart.Continuous, "continuous");
			Add(result, LineNumberingRestart.NewPage, "newPage");
			Add(result, LineNumberingRestart.NewSection, "newSection");
			return result;
		}
		#endregion
		#region CreatePageNumberingFormatTable
		static Dictionary<NumberingFormat, WordProcessingMLValue> CreatePageNumberingFormatTable() {
			Dictionary<NumberingFormat, WordProcessingMLValue> result = new Dictionary<NumberingFormat, WordProcessingMLValue>();
			Add(result, NumberingFormat.None, "none");
			Add(result, NumberingFormat.Decimal, "decimal");
			Add(result, NumberingFormat.AIUEOFullWidthHiragana, "aiueoFullWidth");
			Add(result, NumberingFormat.AIUEOHiragana, "aiueo");
			Add(result, NumberingFormat.ArabicAbjad, "arabicAbjad");
			Add(result, NumberingFormat.ArabicAlpha, "arabicAlpha");
			Add(result, NumberingFormat.Bullet, "bullet");
			Add(result, NumberingFormat.CardinalText, "cardinalText");
			Add(result, NumberingFormat.Chicago, "chicago");
			Add(result, NumberingFormat.ChineseCounting, "chineseCounting");
			Add(result, NumberingFormat.ChineseCountingThousand, "chineseCountingThousand");
			Add(result, NumberingFormat.ChineseLegalSimplified, "chineseLegalSimplified");
			Add(result, NumberingFormat.Chosung, "chosung");
			Add(result, NumberingFormat.DecimalEnclosedCircle, "decimalEnclosedCircle");
			Add(result, NumberingFormat.DecimalEnclosedCircleChinese, "decimalEnclosedCircleChinese");
			Add(result, NumberingFormat.DecimalEnclosedFullstop, "decimalEnclosedFullstop");
			Add(result, NumberingFormat.DecimalEnclosedParenthses, "decimalEnclosedParen");
			Add(result, NumberingFormat.DecimalFullWidth, "decimalFullWidth");
			Add(result, NumberingFormat.DecimalFullWidth2, "decimalFullWidth2");
			Add(result, NumberingFormat.DecimalHalfWidth, "decimalHalfWidth");
			Add(result, NumberingFormat.DecimalZero, "decimalZero");
			Add(result, NumberingFormat.Ganada, "ganada");
			Add(result, NumberingFormat.Hebrew1, "hebrew1");
			Add(result, NumberingFormat.Hebrew2, "hebrew2");
			Add(result, NumberingFormat.Hex, "hex");
			Add(result, NumberingFormat.HindiConsonants, "hindiConsonants");
			Add(result, NumberingFormat.HindiDescriptive, "hindiCounting");
			Add(result, NumberingFormat.HindiNumbers, "hindiNumbers");
			Add(result, NumberingFormat.HindiVowels, "hindiVowels");
			Add(result, NumberingFormat.IdeographDigital, "ideographDigital");
			Add(result, NumberingFormat.IdeographEnclosedCircle, "ideographEnclosedCircle");
			Add(result, NumberingFormat.IdeographLegalTraditional, "ideographLegalTraditional");
			Add(result, NumberingFormat.IdeographTraditional, "ideographTraditional");
			Add(result, NumberingFormat.IdeographZodiac, "ideographZodiac");
			Add(result, NumberingFormat.IdeographZodiacTraditional, "ideographZodiacTraditional");
			Add(result, NumberingFormat.Iroha, "iroha");
			Add(result, NumberingFormat.IrohaFullWidth, "irohaFullWidth");
			Add(result, NumberingFormat.JapaneseCounting, "japaneseCounting");
			Add(result, NumberingFormat.JapaneseDigitalTenThousand, "japaneseDigitalTenThousand");
			Add(result, NumberingFormat.JapaneseLegal, "japaneseLegal");
			Add(result, NumberingFormat.KoreanCounting, "koreanCounting");
			Add(result, NumberingFormat.KoreanDigital, "koreanDigital");
			Add(result, NumberingFormat.KoreanDigital2, "koreanDigital2");
			Add(result, NumberingFormat.KoreanLegal, "koreanLegal");
			Add(result, NumberingFormat.LowerLetter, "lowerLetter");
			Add(result, NumberingFormat.LowerRoman, "lowerRoman");
			Add(result, NumberingFormat.NumberInDash, "numberInDash");
			Add(result, NumberingFormat.Ordinal, "ordinal");
			Add(result, NumberingFormat.OrdinalText, "ordinalText");
			Add(result, NumberingFormat.RussianLower, "russianLower");
			Add(result, NumberingFormat.RussianUpper, "russianUpper");
			Add(result, NumberingFormat.TaiwaneseCounting, "taiwaneseCounting");
			Add(result, NumberingFormat.TaiwaneseCountingThousand, "taiwaneseCountingThousand");
			Add(result, NumberingFormat.TaiwaneseDigital, "taiwaneseDigital");
			Add(result, NumberingFormat.ThaiDescriptive, "thaiCounting");
			Add(result, NumberingFormat.ThaiLetters, "thaiLetters");
			Add(result, NumberingFormat.ThaiNumbers, "thaiNumbers");
			Add(result, NumberingFormat.UpperLetter, "upperLetter");
			Add(result, NumberingFormat.UpperRoman, "upperRoman");
			Add(result, NumberingFormat.VietnameseDescriptive, "vietnameseCounting");
			return result;
		}
		#endregion
		#region CreateChapterSeparatorsTable
		static Dictionary<char, WordProcessingMLValue> CreateChapterSeparatorsTable() {
			Dictionary<char, WordProcessingMLValue> result = new Dictionary<char, WordProcessingMLValue>();
			Add(result, ':', "colon");
			Add(result, Characters.EmDash, "emDash");
			Add(result, Characters.EnDash, "enDash");
			Add(result, Characters.Hyphen, "hyphen");
			Add(result, '.', "period");
			return result;
		}
		#endregion
		#region CreateNumberingListTypeTable
		static Dictionary<NumberingType, WordProcessingMLValue> CreateNumberingListTypeTable() {
			Dictionary<NumberingType, WordProcessingMLValue> result = new Dictionary<NumberingType, WordProcessingMLValue>();
			result.Add(NumberingType.Bullet, new WordProcessingMLValue("hybridMultilevel", "HybridMultilevel"));
			result.Add(NumberingType.Simple, new WordProcessingMLValue("hybridMultilevel", "HybridMultilevel"));
			result.Add(NumberingType.MultiLevel, new WordProcessingMLValue("multilevel", "Multilevel"));
			return result;
		}
		#endregion
		#region CreateListNumberAlignmentTable
		static Dictionary<ListNumberAlignment, WordProcessingMLValue> CreateListNumberAlignmentTable() {
			Dictionary<ListNumberAlignment, WordProcessingMLValue> result = new Dictionary<ListNumberAlignment, WordProcessingMLValue>();
			Add(result, ListNumberAlignment.Left, "left");
			Add(result, ListNumberAlignment.Right, "right");
			Add(result, ListNumberAlignment.Center, "center");
			return result;
		}
		#endregion
		#region CreateListNumberSeparatorTable
		static Dictionary<char, WordProcessingMLValue> CreateListNumberSeparatorTable() {
			Dictionary<char, WordProcessingMLValue> result = new Dictionary<char, WordProcessingMLValue>();
			Add(result, '\0', "nothing");
			Add(result, ' ', "space");
			Add(result, Characters.TabMark, "tab");
			return result;
		}
		#endregion
		#region CreateContentTypeTable
		static Dictionary<string, string> CreateContentTypeTable() {
			Dictionary<string, string> result = new Dictionary<string, string>();
			result.Add("jpg", "image/jpeg"); 
			result.Add("png", "image/png");
			result.Add("bmp", "image/bitmap");
			result.Add("tif", "image/tiff"); 
			result.Add("gif", "image/gif"); 
			result.Add("ico", "image/x-icon");
			result.Add("wmf", "application/x-msmetafile");
			result.Add("emf", "application/x-msmetafile");
			return result;
		}
		#endregion
		#region CreateWidthUnitTypesTable
		static Dictionary<WidthUnitType, WordProcessingMLValue> CreateWidthUnitTypesTable() {
			Dictionary<WidthUnitType, WordProcessingMLValue> result = new Dictionary<WidthUnitType, WordProcessingMLValue>();
			Add(result, WidthUnitType.Auto, "auto");
			Add(result, WidthUnitType.FiftiethsOfPercent, "pct");
			Add(result, WidthUnitType.ModelUnits, "dxa");
			Add(result, WidthUnitType.Nil, "nil");
			return result;
		}
		#endregion
		#region CreateTableLayoutTypeTable
		static Dictionary<TableLayoutType, WordProcessingMLValue> CreateTableLayoutTypeTable() {
			Dictionary<TableLayoutType, WordProcessingMLValue> result = new Dictionary<TableLayoutType, WordProcessingMLValue>();
			Add(result, TableLayoutType.Autofit, "autofit");
			Add(result, TableLayoutType.Fixed, "fixed");
			return result;
		}
		#endregion
		#region CreateBorderLineStyleTable
		static Dictionary<BorderLineStyle, WordProcessingMLValue> CreateBorderLineStyleTable() {
			Dictionary<BorderLineStyle, WordProcessingMLValue> result = new Dictionary<BorderLineStyle, WordProcessingMLValue>();
			Add(result, BorderLineStyle.Nil, "nil");
			Add(result, BorderLineStyle.DashDotStroked, "dashDotStroked");
			Add(result, BorderLineStyle.Dashed, "dashed");
			Add(result, BorderLineStyle.DashSmallGap, "dashSmallGap");
			Add(result, BorderLineStyle.DotDash, "dotDash");
			Add(result, BorderLineStyle.DotDotDash, "dotDotDash");
			Add(result, BorderLineStyle.Dotted, "dotted");
			Add(result, BorderLineStyle.Double, "double");
			Add(result, BorderLineStyle.DoubleWave, "doubleWave");
			Add(result, BorderLineStyle.Inset, "inset");
			Add(result, BorderLineStyle.Disabled, "disabled");
			Add(result, BorderLineStyle.None, "none");
			Add(result, BorderLineStyle.Outset, "outset");
			Add(result, BorderLineStyle.Single, "single");
			Add(result, BorderLineStyle.Thick, "thick");
			Add(result, BorderLineStyle.ThickThinLargeGap, "thickThinLargeGap");
			Add(result, BorderLineStyle.ThickThinMediumGap, "thickThinMediumGap");
			Add(result, BorderLineStyle.ThickThinSmallGap, "thickThinSmallGap");
			Add(result, BorderLineStyle.ThinThickLargeGap, "thinThickLargeGap");
			Add(result, BorderLineStyle.ThinThickMediumGap, "thinThickMediumGap");
			Add(result, BorderLineStyle.ThinThickSmallGap, "thinThickSmallGap");
			Add(result, BorderLineStyle.ThinThickThinLargeGap, "thinThickThinLargeGap");
			Add(result, BorderLineStyle.ThinThickThinMediumGap, "thinThickThinMediumGap");
			Add(result, BorderLineStyle.ThinThickThinSmallGap, "thinThickThinSmallGap");
			Add(result, BorderLineStyle.ThreeDEmboss, "threeDEmboss");
			Add(result, BorderLineStyle.ThreeDEngrave, "threeDEngrave");
			Add(result, BorderLineStyle.Triple, "triple");
			Add(result, BorderLineStyle.Wave, "wave");
			Add(result, BorderLineStyle.Apples, "apples");
			Add(result, BorderLineStyle.ArchedScallops, "archedScallops");
			Add(result, BorderLineStyle.BabyPacifier, "babyPacifier");
			Add(result, BorderLineStyle.BabyRattle, "babyRattle");
			Add(result, BorderLineStyle.Balloons3Colors, "balloons3Colors");
			Add(result, BorderLineStyle.BalloonsHotAir, "balloonsHotAir");
			Add(result, BorderLineStyle.BasicBlackDashes, "basicBlackDashes");
			Add(result, BorderLineStyle.BasicBlackDots, "basicBlackDots");
			Add(result, BorderLineStyle.BasicBlackSquares, "basicBlackSquares");
			Add(result, BorderLineStyle.BasicThinLines, "basicThinLines");
			Add(result, BorderLineStyle.BasicWhiteDashes, "basicWhiteDashes");
			Add(result, BorderLineStyle.BasicWhiteDots, "basicWhiteDots");
			Add(result, BorderLineStyle.BasicWhiteSquares, "basicWhiteSquares");
			Add(result, BorderLineStyle.BasicWideInline, "basicWideInline");
			Add(result, BorderLineStyle.BasicWideMidline, "basicWideMidline");
			Add(result, BorderLineStyle.BasicWideOutline, "basicWideOutline");
			Add(result, BorderLineStyle.Bats, "bats");
			Add(result, BorderLineStyle.Birds, "birds");
			Add(result, BorderLineStyle.BirdsFlight, "birdsFlight");
			Add(result, BorderLineStyle.Cabins, "cabins");
			Add(result, BorderLineStyle.CakeSlice, "cakeSlice");
			Add(result, BorderLineStyle.CandyCorn, "candyCorn");
			Add(result, BorderLineStyle.CelticKnotwork, "celticKnotwork");
			Add(result, BorderLineStyle.CertificateBanner, "certificateBanner");
			Add(result, BorderLineStyle.ChainLink, "chainLink");
			Add(result, BorderLineStyle.ChampagneBottle, "champagneBottle");
			Add(result, BorderLineStyle.CheckedBarBlack, "checkedBarBlack");
			Add(result, BorderLineStyle.CheckedBarColor, "checkedBarColor");
			Add(result, BorderLineStyle.Checkered, "checkered");
			Add(result, BorderLineStyle.ChristmasTree, "christmasTree");
			Add(result, BorderLineStyle.CirclesLines, "circlesLines");
			Add(result, BorderLineStyle.CirclesRectangles, "circlesRectangles");
			Add(result, BorderLineStyle.ClassicalWave, "classicalWave");
			Add(result, BorderLineStyle.Clocks, "clocks");
			Add(result, BorderLineStyle.Compass, "compass");
			Add(result, BorderLineStyle.Confetti, "confetti");
			Add(result, BorderLineStyle.ConfettiGrays, "confettiGrays");
			Add(result, BorderLineStyle.ConfettiOutline, "confettiOutline");
			Add(result, BorderLineStyle.ConfettiStreamers, "confettiStreamers");
			Add(result, BorderLineStyle.ConfettiWhite, "confettiWhite");
			Add(result, BorderLineStyle.CornerTriangles, "cornerTriangles");
			Add(result, BorderLineStyle.CouponCutoutDashes, "couponCutoutDashes");
			Add(result, BorderLineStyle.CouponCutoutDots, "couponCutoutDots");
			Add(result, BorderLineStyle.CrazyMaze, "crazyMaze");
			Add(result, BorderLineStyle.CreaturesButterfly, "creaturesButterfly");
			Add(result, BorderLineStyle.CreaturesFish, "creaturesFish");
			Add(result, BorderLineStyle.CreaturesInsects, "creaturesInsects");
			Add(result, BorderLineStyle.CreaturesLadyBug, "creaturesLadyBug");
			Add(result, BorderLineStyle.CrossStitch, "crossStitch");
			Add(result, BorderLineStyle.Cup, "cup");
			Add(result, BorderLineStyle.DecoArch, "decoArch");
			Add(result, BorderLineStyle.DecoArchColor, "decoArchColor");
			Add(result, BorderLineStyle.DecoBlocks, "decoBlocks");
			Add(result, BorderLineStyle.DiamondsGray, "diamondsGray");
			Add(result, BorderLineStyle.DoubleD, "doubleD");
			Add(result, BorderLineStyle.DoubleDiamonds, "doubleDiamonds");
			Add(result, BorderLineStyle.Earth1, "earth1");
			Add(result, BorderLineStyle.Earth2, "earth2");
			Add(result, BorderLineStyle.EclipsingSquares1, "eclipsingSquares1");
			Add(result, BorderLineStyle.EclipsingSquares2, "eclipsingSquares2");
			Add(result, BorderLineStyle.EggsBlack, "eggsBlack");
			Add(result, BorderLineStyle.Fans, "fans");
			Add(result, BorderLineStyle.Film, "film");
			Add(result, BorderLineStyle.Firecrackers, "firecrackers");
			Add(result, BorderLineStyle.FlowersBlockPrint, "flowersBlockPrint");
			Add(result, BorderLineStyle.FlowersDaisies, "flowersDaisies");
			Add(result, BorderLineStyle.FlowersModern1, "flowersModern1");
			Add(result, BorderLineStyle.FlowersModern2, "flowersModern2");
			Add(result, BorderLineStyle.FlowersPansy, "flowersPansy");
			Add(result, BorderLineStyle.FlowersRedRose, "flowersRedRose");
			Add(result, BorderLineStyle.FlowersRoses, "flowersRoses");
			Add(result, BorderLineStyle.FlowersTeacup, "flowersTeacup");
			Add(result, BorderLineStyle.FlowersTiny, "flowersTiny");
			Add(result, BorderLineStyle.Gems, "gems");
			Add(result, BorderLineStyle.GingerbreadMan, "gingerbreadMan");
			Add(result, BorderLineStyle.Gradient, "gradient");
			Add(result, BorderLineStyle.Handmade1, "handmade1");
			Add(result, BorderLineStyle.Handmade2, "handmade2");
			Add(result, BorderLineStyle.HeartBalloon, "heartBalloon");
			Add(result, BorderLineStyle.HeartGray, "heartGray");
			Add(result, BorderLineStyle.Hearts, "hearts");
			Add(result, BorderLineStyle.HeebieJeebies, "heebieJeebies");
			Add(result, BorderLineStyle.Holly, "holly");
			Add(result, BorderLineStyle.HouseFunky, "houseFunky");
			Add(result, BorderLineStyle.Hypnotic, "hypnotic");
			Add(result, BorderLineStyle.IceCreamCones, "iceCreamCones");
			Add(result, BorderLineStyle.LightBulb, "lightBulb");
			Add(result, BorderLineStyle.Lightning1, "lightning1");
			Add(result, BorderLineStyle.Lightning2, "lightning2");
			Add(result, BorderLineStyle.MapleLeaf, "mapleLeaf");
			Add(result, BorderLineStyle.MapleMuffins, "mapleMuffins");
			Add(result, BorderLineStyle.MapPins, "mapPins");
			Add(result, BorderLineStyle.Marquee, "marquee");
			Add(result, BorderLineStyle.MarqueeToothed, "marqueeToothed");
			Add(result, BorderLineStyle.Moons, "moons");
			Add(result, BorderLineStyle.Mosaic, "mosaic");
			Add(result, BorderLineStyle.MusicNotes, "musicNotes");
			Add(result, BorderLineStyle.Northwest, "northwest");
			Add(result, BorderLineStyle.Ovals, "ovals");
			Add(result, BorderLineStyle.Packages, "packages");
			Add(result, BorderLineStyle.PalmsBlack, "palmsBlack");
			Add(result, BorderLineStyle.PalmsColor, "palmsColor");
			Add(result, BorderLineStyle.PaperClips, "paperClips");
			Add(result, BorderLineStyle.Papyrus, "papyrus");
			Add(result, BorderLineStyle.PartyFavor, "partyFavor");
			Add(result, BorderLineStyle.PartyGlass, "partyGlass");
			Add(result, BorderLineStyle.Pencils, "pencils");
			Add(result, BorderLineStyle.People, "people");
			Add(result, BorderLineStyle.PeopleHats, "peopleHats");
			Add(result, BorderLineStyle.PeopleWaving, "peopleWaving");
			Add(result, BorderLineStyle.Poinsettias, "poinsettias");
			Add(result, BorderLineStyle.PostageStamp, "postageStamp");
			Add(result, BorderLineStyle.Pumpkin1, "pumpkin1");
			Add(result, BorderLineStyle.PushPinNote1, "pushPinNote1");
			Add(result, BorderLineStyle.PushPinNote2, "pushPinNote2");
			Add(result, BorderLineStyle.Pyramids, "pyramids");
			Add(result, BorderLineStyle.PyramidsAbove, "pyramidsAbove");
			Add(result, BorderLineStyle.Quadrants, "quadrants");
			Add(result, BorderLineStyle.Rings, "rings");
			Add(result, BorderLineStyle.Safari, "safari");
			Add(result, BorderLineStyle.Sawtooth, "sawtooth");
			Add(result, BorderLineStyle.SawtoothGray, "sawtoothGray");
			Add(result, BorderLineStyle.ScaredCat, "scaredCat");
			Add(result, BorderLineStyle.Seattle, "seattle");
			Add(result, BorderLineStyle.ShadowedSquares, "shadowedSquares");
			Add(result, BorderLineStyle.SharksTeeth, "sharksTeeth");
			Add(result, BorderLineStyle.ShorebirdTracks, "shorebirdTracks");
			Add(result, BorderLineStyle.Skyrocket, "skyrocket");
			Add(result, BorderLineStyle.SnowflakeFancy, "snowflakeFancy");
			Add(result, BorderLineStyle.Snowflakes, "snowflakes");
			Add(result, BorderLineStyle.Sombrero, "sombrero");
			Add(result, BorderLineStyle.Southwest, "southwest");
			Add(result, BorderLineStyle.Stars, "stars");
			Add(result, BorderLineStyle.Stars3d, "stars3d");
			Add(result, BorderLineStyle.StarsBlack, "starsBlack");
			Add(result, BorderLineStyle.StarsShadowed, "starsShadowed");
			Add(result, BorderLineStyle.StarsTop, "starsTop");
			Add(result, BorderLineStyle.Sun, "sun");
			Add(result, BorderLineStyle.Swirligig, "swirligig");
			Add(result, BorderLineStyle.TornPaper, "tornPaper");
			Add(result, BorderLineStyle.TornPaperBlack, "tornPaperBlack");
			Add(result, BorderLineStyle.Trees, "trees");
			Add(result, BorderLineStyle.TriangleParty, "triangleParty");
			Add(result, BorderLineStyle.Triangles, "triangles");
			Add(result, BorderLineStyle.Tribal1, "tribal1");
			Add(result, BorderLineStyle.Tribal2, "tribal2");
			Add(result, BorderLineStyle.Tribal3, "tribal3");
			Add(result, BorderLineStyle.Tribal4, "tribal4");
			Add(result, BorderLineStyle.Tribal5, "tribal5");
			Add(result, BorderLineStyle.Tribal6, "tribal6");
			Add(result, BorderLineStyle.TwistedLines1, "twistedLines1");
			Add(result, BorderLineStyle.TwistedLines2, "twistedLines2");
			Add(result, BorderLineStyle.Vine, "vine");
			Add(result, BorderLineStyle.Waveline, "waveline");
			Add(result, BorderLineStyle.WeavingAngles, "weavingAngles");
			Add(result, BorderLineStyle.WeavingBraid, "weavingBraid");
			Add(result, BorderLineStyle.WeavingRibbon, "weavingRibbon");
			Add(result, BorderLineStyle.WeavingStrips, "weavingStrips");
			Add(result, BorderLineStyle.WhiteFlowers, "whiteFlowers");
			Add(result, BorderLineStyle.Woodwork, "woodwork");
			Add(result, BorderLineStyle.XIllusions, "xIllusions");
			Add(result, BorderLineStyle.ZanyTriangles, "zanyTriangles");
			Add(result, BorderLineStyle.ZigZag, "zigZag");
			Add(result, BorderLineStyle.ZigZagStitch, "zigZagStitch");
			return result;
		}
		#endregion
		#region CreateHorizontalAlignModeTable
		static Dictionary<HorizontalAlignMode, WordProcessingMLValue> CreateHorizontalAlignModeTable() {
			Dictionary<HorizontalAlignMode, WordProcessingMLValue> result = new Dictionary<HorizontalAlignMode, WordProcessingMLValue>();
			Add(result, HorizontalAlignMode.Center, "center");
			Add(result, HorizontalAlignMode.Inside, "inside");
			Add(result, HorizontalAlignMode.Left, "left");
			Add(result, HorizontalAlignMode.Outside, "outside");
			Add(result, HorizontalAlignMode.Right, "right");
			return result;
		}
		#endregion
		#region CreateHorizontalAnchorTypesTable
		static Dictionary<HorizontalAnchorTypes, WordProcessingMLValue> CreateHorizontalAnchorTypesTable() {
			Dictionary<HorizontalAnchorTypes, WordProcessingMLValue> result = new Dictionary<HorizontalAnchorTypes, WordProcessingMLValue>();
			Add(result, HorizontalAnchorTypes.Column, "text");
			Add(result, HorizontalAnchorTypes.Margin, "margin");
			Add(result, HorizontalAnchorTypes.Page, "page");
			return result;
		}
		#endregion
		#region CreateVerticalAlignModeTable
		static Dictionary<VerticalAlignMode, WordProcessingMLValue> CreateVerticalAlignModeTable() {
			Dictionary<VerticalAlignMode, WordProcessingMLValue> result = new Dictionary<VerticalAlignMode, WordProcessingMLValue>();
			Add(result, VerticalAlignMode.Bottom, "bottom");
			Add(result, VerticalAlignMode.Center, "center");
			Add(result, VerticalAlignMode.Inline, "inline");
			Add(result, VerticalAlignMode.Inside, "inside");
			Add(result, VerticalAlignMode.Outside, "outside");
			Add(result, VerticalAlignMode.Top, "top");
			return result;
		}
		#endregion
		#region CreateVerticalAnchorTypesTable
		static Dictionary<VerticalAnchorTypes, WordProcessingMLValue> CreateVerticalAnchorTypesTable() {
			Dictionary<VerticalAnchorTypes, WordProcessingMLValue> result = new Dictionary<VerticalAnchorTypes, WordProcessingMLValue>();
			Add(result, VerticalAnchorTypes.Paragraph, "text");
			Add(result, VerticalAnchorTypes.Margin, "margin");
			Add(result, VerticalAnchorTypes.Page, "page");
			return result;
		}
		#endregion
		#region CreateHeightUnitTypeTable
		static Dictionary<HeightUnitType, WordProcessingMLValue> CreateHeightUnitTypeTable() {
			Dictionary<HeightUnitType, WordProcessingMLValue> result = new Dictionary<HeightUnitType, WordProcessingMLValue>();
			Add(result, HeightUnitType.Auto, "auto");
			Add(result, HeightUnitType.Exact, "exact");
			Add(result, HeightUnitType.Minimum, "atLeast");
			return result;
		}
		#endregion
		#region CreateMergingStateTable
		static Dictionary<MergingState, WordProcessingMLValue> CreateMergingStateTable() {
			Dictionary<MergingState, WordProcessingMLValue> result = new Dictionary<MergingState, WordProcessingMLValue>();
			Add(result, MergingState.Restart, "restart");
			Add(result, MergingState.Continue, "continue");
			return result;
		}
		#endregion
		#region CreateTableRowAlignmentTable
		static Dictionary<TableRowAlignment, WordProcessingMLValue> CreateTableRowAlignmentTable() {
			Dictionary<TableRowAlignment, WordProcessingMLValue> result = new Dictionary<TableRowAlignment, WordProcessingMLValue>();
			Add(result, TableRowAlignment.Both, "both");
			Add(result, TableRowAlignment.Center, "center");
			Add(result, TableRowAlignment.Distribute, "distribute");
			Add(result, TableRowAlignment.Left, "left");
			Add(result, TableRowAlignment.NumTab, "numTab");
			Add(result, TableRowAlignment.Right, "right");
			return result;
		}
		#endregion
		#region CreateFootNotePlacementTable
		static Dictionary<FootNotePosition, WordProcessingMLValue> CreateFootNotePlacementTable() {
			Dictionary<FootNotePosition, WordProcessingMLValue> result = new Dictionary<FootNotePosition, WordProcessingMLValue>();
			Add(result, FootNotePosition.BelowText, "beneathText");
			Add(result, FootNotePosition.BottomOfPage, "pageBottom");
			Add(result, FootNotePosition.EndOfDocument, "docEnd");
			Add(result, FootNotePosition.EndOfSection, "sectEnd");
			return result;
		}
		#endregion
		#region CreateFloatingObjectHorizontalPositionAlignmentTable
		static Dictionary<FloatingObjectHorizontalPositionAlignment, WordProcessingMLValue> CreateFloatingObjectHorizontalPositionAlignmentTable() {
			Dictionary<FloatingObjectHorizontalPositionAlignment, WordProcessingMLValue> result = new Dictionary<FloatingObjectHorizontalPositionAlignment, WordProcessingMLValue>();
			Add(result, FloatingObjectHorizontalPositionAlignment.Left, "left");
			Add(result, FloatingObjectHorizontalPositionAlignment.Center, "center");
			Add(result, FloatingObjectHorizontalPositionAlignment.Right, "right");
			Add(result, FloatingObjectHorizontalPositionAlignment.Inside, "inside");
			Add(result, FloatingObjectHorizontalPositionAlignment.Outside, "outside");
			return result;
		}
		#endregion
		#region CreateFloatingObjectVerticalPositionAlignmentTable
		static Dictionary<FloatingObjectVerticalPositionAlignment, WordProcessingMLValue> CreateFloatingObjectVerticalPositionAlignmentTable() {
			Dictionary<FloatingObjectVerticalPositionAlignment, WordProcessingMLValue> result = new Dictionary<FloatingObjectVerticalPositionAlignment, WordProcessingMLValue>();
			Add(result, FloatingObjectVerticalPositionAlignment.Top, "top");
			Add(result, FloatingObjectVerticalPositionAlignment.Center, "center");
			Add(result, FloatingObjectVerticalPositionAlignment.Bottom, "bottom");
			Add(result, FloatingObjectVerticalPositionAlignment.Inside, "inside");
			Add(result, FloatingObjectVerticalPositionAlignment.Outside, "outside");
			return result;
		}
		#endregion
		#region CreateFloatingObjectHorizontalPositionTypeTable
		static Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> CreateFloatingObjectHorizontalPositionTypeTable() {
			Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> result = new Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue>();
			result.Add(FloatingObjectHorizontalPositionType.Column, new WordProcessingMLValue("column", "text"));
			Add(result, FloatingObjectHorizontalPositionType.Margin, "margin");
			Add(result, FloatingObjectHorizontalPositionType.Page, "page");
			result.Add(FloatingObjectHorizontalPositionType.Character, new WordProcessingMLValue("character", "char"));
			result.Add(FloatingObjectHorizontalPositionType.LeftMargin, new WordProcessingMLValue("leftMargin", "page"));
			result.Add(FloatingObjectHorizontalPositionType.RightMargin, new WordProcessingMLValue("rightMargin", "page"));
			result.Add(FloatingObjectHorizontalPositionType.InsideMargin, new WordProcessingMLValue("insideMargin", "page"));
			result.Add(FloatingObjectHorizontalPositionType.OutsideMargin, new WordProcessingMLValue("outsideMargin", "page"));
			return result;
		}
		#endregion
		#region CreateFloatingObjectCssRelativeFromHorizontalTable
		static Dictionary<FloatingObjectRelativeFromHorizontal, WordProcessingMLValue> CreateFloatingObjectCssRelativeFromHorizontalTable() {
			Dictionary<FloatingObjectRelativeFromHorizontal, WordProcessingMLValue> result = new Dictionary<FloatingObjectRelativeFromHorizontal, WordProcessingMLValue>();
			Add(result, FloatingObjectRelativeFromHorizontal.Margin, "margin");
			Add(result, FloatingObjectRelativeFromHorizontal.Page, "page");
			Add(result, FloatingObjectRelativeFromHorizontal.LeftMargin, "left-margin-area");
			Add(result, FloatingObjectRelativeFromHorizontal.RightMargin, "right-margin-area");
			Add(result, FloatingObjectRelativeFromHorizontal.OutsideMargin, "outer-margin-area");
			Add(result, FloatingObjectRelativeFromHorizontal.InsideMargin, "inner-margin-area");
			return result;
		}
		#endregion
		#region CreateFloatingObjectCssRelativeFromVerticalTable
		static Dictionary<FloatingObjectRelativeFromVertical, WordProcessingMLValue> CreateFloatingObjectCssRelativeFromVerticalTable() {
			Dictionary<FloatingObjectRelativeFromVertical, WordProcessingMLValue> result = new Dictionary<FloatingObjectRelativeFromVertical, WordProcessingMLValue>();
			Add(result, FloatingObjectRelativeFromVertical.Margin, "margin");
			Add(result, FloatingObjectRelativeFromVertical.Page, "page");
			Add(result, FloatingObjectRelativeFromVertical.TopMargin, "top-margin-area");
			Add(result, FloatingObjectRelativeFromVertical.BottomMargin, "bottom-margin-area");
			Add(result, FloatingObjectRelativeFromVertical.OutsideMargin, "outer-margin-area");
			Add(result, FloatingObjectRelativeFromVertical.InsideMargin, "inner-margin-area");
			return result;
		}
		#endregion
		#region CreateFloatingObjectRelativeFromHorizontalTable
		static Dictionary<FloatingObjectRelativeFromHorizontal, WordProcessingMLValue> CreateFloatingObjectRelativeFromHorizontalTable() {
			Dictionary<FloatingObjectRelativeFromHorizontal, WordProcessingMLValue> result = new Dictionary<FloatingObjectRelativeFromHorizontal, WordProcessingMLValue>();
			Add(result, FloatingObjectRelativeFromHorizontal.Margin, "margin");
			Add(result, FloatingObjectRelativeFromHorizontal.Page, "page");
			Add(result, FloatingObjectRelativeFromHorizontal.LeftMargin, "leftMargin");
			Add(result, FloatingObjectRelativeFromHorizontal.RightMargin, "rightMargin");
			Add(result, FloatingObjectRelativeFromHorizontal.OutsideMargin, "outsideMargin");
			Add(result, FloatingObjectRelativeFromHorizontal.InsideMargin, "insideMargin");
			return result;
		}
		#endregion
		#region CreateFloatingObjectRelativeFromVerticalTable
		static Dictionary<FloatingObjectRelativeFromVertical, WordProcessingMLValue> CreateFloatingObjectRelativeFromVerticalTable() {
			Dictionary<FloatingObjectRelativeFromVertical, WordProcessingMLValue> result = new Dictionary<FloatingObjectRelativeFromVertical, WordProcessingMLValue>();
			Add(result, FloatingObjectRelativeFromVertical.Margin, "margin");
			Add(result, FloatingObjectRelativeFromVertical.Page, "page");
			Add(result, FloatingObjectRelativeFromVertical.TopMargin, "topMargin");
			Add(result, FloatingObjectRelativeFromVertical.BottomMargin, "bottomMargin");
			Add(result, FloatingObjectRelativeFromVertical.OutsideMargin, "outsideMargin");
			Add(result, FloatingObjectRelativeFromVertical.InsideMargin, "insideMargin");
			return result;
		}
		#endregion
		#region CreateFloatingObjectVerticalPositionTypeTable
		static Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> CreateFloatingObjectVerticalPositionTypeTable() {
			Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> result = new Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue>();
			Add(result, FloatingObjectVerticalPositionType.Margin, "margin");
			result.Add(FloatingObjectVerticalPositionType.Page, new WordProcessingMLValue("page", "margin"));
			Add(result, FloatingObjectVerticalPositionType.Line, "line");
			result.Add(FloatingObjectVerticalPositionType.Paragraph, new WordProcessingMLValue("paragraph", "text"));
			result.Add(FloatingObjectVerticalPositionType.TopMargin, new WordProcessingMLValue("topMargin", "page"));
			result.Add(FloatingObjectVerticalPositionType.BottomMargin, new WordProcessingMLValue("bottomMargin", "page"));
			result.Add(FloatingObjectVerticalPositionType.InsideMargin, new WordProcessingMLValue("insideMargin", "page"));
			result.Add(FloatingObjectVerticalPositionType.OutsideMargin, new WordProcessingMLValue("outsideMargin", "page"));
			return result;
		}
		#endregion
		#region CreateFloatingObjectTextWrapSideTable
		static Dictionary<FloatingObjectTextWrapSide, WordProcessingMLValue> CreateFloatingObjectTextWrapSideTable() {
			Dictionary<FloatingObjectTextWrapSide, WordProcessingMLValue> result = new Dictionary<FloatingObjectTextWrapSide, WordProcessingMLValue>();
			result.Add(FloatingObjectTextWrapSide.Both, new WordProcessingMLValue("bothSides", "both-sides"));
			Add(result, FloatingObjectTextWrapSide.Left, "left");
			Add(result, FloatingObjectTextWrapSide.Right, "right");
			Add(result, FloatingObjectTextWrapSide.Largest, "largest");
			return result;
		}
		#endregion
		#region FloatingObjectTextWrapTypeTable
		static Dictionary<FloatingObjectTextWrapType, WordProcessingMLValue> CreateFloatingObjectTextWrapTypeTable() {
			Dictionary<FloatingObjectTextWrapType, WordProcessingMLValue> result = new Dictionary<FloatingObjectTextWrapType, WordProcessingMLValue>();
			result.Add(FloatingObjectTextWrapType.TopAndBottom, new WordProcessingMLValue("wrapTopAndBottom", "topAndBottom"));
			result.Add(FloatingObjectTextWrapType.Square, new WordProcessingMLValue("wrapSquare", "square"));
			result.Add(FloatingObjectTextWrapType.Through, new WordProcessingMLValue("wrapThrough", "through"));
			result.Add(FloatingObjectTextWrapType.Tight, new WordProcessingMLValue("wrapTight", "tight"));
			result.Add(FloatingObjectTextWrapType.None, new WordProcessingMLValue("wrapNone", "none"));
			return result;
		}
		#endregion
		#region CreateTextBoxVerticalAlignmentTable
		static Dictionary<VerticalAlignment, WordProcessingMLValue> CreateTextBoxVerticalAlignmentTable() {
			Dictionary<VerticalAlignment, WordProcessingMLValue> result = new Dictionary<VerticalAlignment, WordProcessingMLValue>();
			result.Add(VerticalAlignment.Top, new WordProcessingMLValue("t", "top"));
			result.Add(VerticalAlignment.Center, new WordProcessingMLValue("ctr", "middle"));
			result.Add(VerticalAlignment.Both, new WordProcessingMLValue("just"));
			result.Add(VerticalAlignment.Bottom, new WordProcessingMLValue("b", "bottom"));
			return result;
		}
		#endregion
		#region CreateConditionalTableStyleFormattingTypesTable
		static Dictionary<ConditionalTableStyleFormattingTypes, WordProcessingMLValue> CreateConditionalTableStyleFormattingTypesTable() {
			Dictionary<ConditionalTableStyleFormattingTypes, WordProcessingMLValue> result = new Dictionary<ConditionalTableStyleFormattingTypes, WordProcessingMLValue>();
			result.Add(ConditionalTableStyleFormattingTypes.BottomLeftCell, new WordProcessingMLValue("swCell", "swCell"));
			result.Add(ConditionalTableStyleFormattingTypes.BottomRightCell, new WordProcessingMLValue("seCell", "seCell"));
			result.Add(ConditionalTableStyleFormattingTypes.EvenColumnBanding, new WordProcessingMLValue("band2Vert", "band2Vert"));
			result.Add(ConditionalTableStyleFormattingTypes.EvenRowBanding, new WordProcessingMLValue("band2Horz", "band2Horz"));
			result.Add(ConditionalTableStyleFormattingTypes.FirstColumn, new WordProcessingMLValue("firstCol", "firstCol"));
			result.Add(ConditionalTableStyleFormattingTypes.FirstRow, new WordProcessingMLValue("firstRow", "firstRow"));
			result.Add(ConditionalTableStyleFormattingTypes.LastColumn, new WordProcessingMLValue("lastCol", "lastCol"));
			result.Add(ConditionalTableStyleFormattingTypes.LastRow, new WordProcessingMLValue("lastRow", "lastRow"));
			result.Add(ConditionalTableStyleFormattingTypes.OddColumnBanding, new WordProcessingMLValue("band1Vert", "band1Vert"));
			result.Add(ConditionalTableStyleFormattingTypes.OddRowBanding, new WordProcessingMLValue("band1Horz", "band1Horz"));
			result.Add(ConditionalTableStyleFormattingTypes.TopLeftCell, new WordProcessingMLValue("nwCell", "nwCell"));
			result.Add(ConditionalTableStyleFormattingTypes.TopRightCell, new WordProcessingMLValue("neCell", "neCell"));
			result.Add(ConditionalTableStyleFormattingTypes.WholeTable, new WordProcessingMLValue("wholeTable", "wholeTable"));
			return result;
		}
		#endregion
		static Dictionary<string, string> CreatePredefinedGroupNames() {
			Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			result.Add("Everyone", "everyone");
			result.Add("Current User", "current");
			result.Add("Editors", "editors");
			result.Add("Owners", "owners");
			result.Add("Contributors", "contributors");
			result.Add("Administrators", "administrators");
			return result;
		}
		#endregion
		#region Properties
		protected DocumentExporterOptions Options { get { return options; } }
		protected internal Section CurrentSection { get { return currentSection; } set { currentSection = value; } }
		protected internal XmlWriter DocumentContentWriter { get { return documentContentWriter; } set { documentContentWriter = value; } }
		protected abstract string WordProcessingNamespace { get; }
		protected abstract string WordProcessingPrefix { get; }
		public const string VMLPrefix = "v";
		public const string VMLNamespace = "urn:schemas-microsoft-com:vml";
		protected string OfficeNamespace { get { return "urn:schemas-microsoft-com:office:office"; } }
		protected string OfficePrefix { get { return "o"; } }
		protected internal int ImageCounter { get { return imageCounter; } set { imageCounter = value; } }
		protected internal DocumentModelUnitConverter UnitConverter { get { return DocumentModel.UnitConverter; } }
		protected int DrawingElementId { get { return drawingElementId; } }
		protected internal bool ForbidExecuteBaseExportTableCell { get { return forbidExecuteBaseExportTableCell; } set { forbidExecuteBaseExportTableCell = value; } } 
		protected internal abstract Dictionary<OfficeImage, string> ExportedImageTable { get; }
		protected internal override InternalZipArchive Package { get { return null; } }
		protected abstract Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> HorizontalPositionTypeAttributeTable { get; }
		protected abstract Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> VerticalPositionTypeAttributeTable { get; }
		#endregion
		#region Write Helpers
		protected internal virtual void WriteWpBoolValue(string tag, bool value) {
			WriteWpStringValue(tag, ConvertBoolToString(value));
		}
		protected internal virtual void WriteWpBoolValueAsTag(string tag, bool value) {
			if (value) {
				WriteWpStartElement(tag);
				WriteWpEndElement();
			}
		}
		protected internal virtual void WriteBoolValue(string tag, bool value) {
			WriteStringAttr(null, tag, null, ConvertBoolToString(value));
		}
		protected internal virtual void WriteWpIntValue(string tag, int value) {
			WriteWpStringValue(tag, value.ToString());
		}
		protected internal virtual void WriteIntValue(string tag, int value) {
			WriteStringAttr(null, tag, null, value.ToString());
		}
		protected internal virtual void WriteWpStringValue(string tag, string value) {
			WriteStringValue(WordProcessingPrefix, tag, value);
		}
		protected internal virtual void WriteStringValue(string tag, string value) {
			WriteStringAttr(null, tag, null, value);
		}
		protected internal virtual void WriteWpBoolAttr(string attr, bool value) {
			WriteWpStringAttr(attr, ConvertBoolToString(value));
		}
		protected internal virtual void WriteWpIntAttr(string attr, int value) {
			WriteWpStringAttr(attr, value.ToString());
		}
		protected internal virtual void WriteW15StringAttr(string attr, string value) {
			documentContentWriter.WriteAttributeString(W15Prefix, attr, W15NamespaceConst, value);
		}
		protected internal virtual void WriteWpStringAttr(string attr, string value) {
			documentContentWriter.WriteAttributeString(WordProcessingPrefix, attr, WordProcessingNamespace, value);
		}
		protected internal virtual void WriteStringAttr(string prefix, string attr, string ns, string value) {
			documentContentWriter.WriteAttributeString(prefix, attr, ns, value);
		}
		protected internal virtual void WriteWpStartElement(string tag) {
			documentContentWriter.WriteStartElement(WordProcessingPrefix, tag, WordProcessingNamespace);
		}
		protected internal virtual void WriteWpEndElement() {
			documentContentWriter.WriteEndElement();
		}
		protected internal virtual void WriteW15StartElement(string tag) {
			DocumentContentWriter.WriteStartElement(W15Prefix, tag, W15NamespaceConst);
		}
		protected internal virtual void WriteW15EndElement() {
			documentContentWriter.WriteEndElement();
		}
		protected internal virtual void WriteStringValue(string prefix, string tag, string value) {
			documentContentWriter.WriteStartElement(prefix, tag, WordProcessingNamespace);
			try {
				documentContentWriter.WriteAttributeString(prefix, "val", WordProcessingNamespace, value);
			}
			finally {
				documentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void WriteWpEmptyElement(string tag) {
			WriteWpStartElement(tag);
			WriteWpEndElement();
		}
		protected internal virtual void WriteWpEmptyOrFalseValue(string tag, bool value) {
			if (value)
				WriteWpEmptyElement(tag);
			else
				WriteWpBoolValue(tag, false);
		}
		#endregion
		#region Run
		protected internal override void ExportTextRun(TextRun run) {
			string runText = run.GetPlainText(PieceTable.TextBuffer);
			WriteWpStartElement("r");
			try {
				ExportRunProperties(run);
				ExportTextRunCore(run, runText);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual bool ShouldPreserveSpace(string text) {
			return text.StartsWith(" ") || text.EndsWith(" ") || text.Contains("  ");
		}
		protected internal virtual bool ShouldExportRunProperties(TextRunBase run) {
			CharacterProperties characterProperties = run.CharacterProperties;
			return
				characterProperties.UseFontName ||
				characterProperties.UseDoubleFontSize ||
				characterProperties.UseFontBold ||
				characterProperties.UseFontItalic ||
				characterProperties.UseFontUnderlineType || (characterProperties.UseUnderlineColor && !DXColor.IsTransparentOrEmpty(characterProperties.UnderlineColor)) || characterProperties.UseUnderlineWordsOnly ||
				characterProperties.UseFontStrikeoutType ||
				characterProperties.UseAllCaps ||
				characterProperties.UseForeColor ||
				characterProperties.UseScript ||
				characterProperties.UseBackColor ||
				characterProperties.UseHidden ||
				characterProperties.UseLangInfo ||
				characterProperties.UseNoProof ||
				run.CharacterStyleIndex > 0;
		}
		protected virtual void ExportTextRunCore(TextRun run, string runText) {
			int from = 0;
			int count = runText.Length;
			for (int i = 0; i < count; i++) {
				char character = runText[i];
				WordProcessingMLValue runBreakValue;
				if (runBreaksTable.TryGetValue(character, out runBreakValue)) {
					ExportTextCore(runText, from, i - from);
					from = i + 1;
					ExportBreak(GetWordProcessingMLValue(runBreakValue));
				}
				else {
					if (character == Characters.TabMark) {
						ExportTextCore(runText, from, i - from);
						from = i + 1;
						WriteWpStartElement("tab");
						WriteWpEndElement();
					}
				}
			}
			ExportTextCore(runText, from, count - from);
		}
		protected virtual void ExportTextCore(string runText, int from, int length) {
			if (length <= 0)
				return;
			if (from == 0 && length == runText.Length)
				ExportTextCore(runText);
			else
				ExportTextCore(runText.Substring(from, length));
		}
		protected internal virtual void ExportBreak(string value) {
			DocumentContentWriter.WriteStartElement(WordProcessingPrefix, "br", WordProcessingNamespace);
			try {
				DocumentContentWriter.WriteAttributeString(WordProcessingPrefix, "type", WordProcessingNamespace, value);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected virtual void ExportTextCore(string runText) {
			if (String.IsNullOrEmpty(runText))
				return;
			runText = XmlTextHelper.DeleteIllegalXmlCharacters(runText);
			if (String.IsNullOrEmpty(runText))
				return;
			WriteWpStartElement(fieldCodeDepth == 0 ? "t" : "instrText");
			try {
				if (ShouldPreserveSpace(runText))
					documentContentWriter.WriteAttributeString("xml", "space", null, "preserve");
				documentContentWriter.WriteString(runText);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportRunProperties(TextRunBase run) {
			if (!ShouldExportRunProperties(run))
				return;
			WriteWpStartElement("rPr");
			try {
				if (run.CharacterStyleIndex > 0)
					WriteWpStringValue("rStyle", GetCharacterStyleId(run.CharacterStyleIndex));
				ExportRunPropertiesCore(run.CharacterProperties);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportRunPropertiesCore(CharacterProperties characterProperties) {
			ExportRunFontName(characterProperties);
			ExportRunFontBold(characterProperties);
			ExportRunFontItalic(characterProperties);
			ExportRunAllCaps(characterProperties);
			ExportRunFontStrikeout(characterProperties);
			ExportRunNoProof(characterProperties);
			ExportRunHidden(characterProperties);
			ExportRunForeColor(characterProperties);
			ExportDoubleFontSize(characterProperties);
			ExportRunFontUnderline(characterProperties);
			ExportRunBackColor(characterProperties);
			ExportRunFontScript(characterProperties);
			ExportRunLangInfo(characterProperties);
		}
		protected internal virtual void ExportRunFontName(CharacterProperties characterProperties) {
			if (!characterProperties.UseFontName)
				return;
			WriteWpStartElement("rFonts");
			try {
				string fontName = PrepareFontName(characterProperties.FontName);
				WriteWpStringAttr("ascii", fontName);
				WriteWpStringAttr(GetWordProcessingMLValue("hAnsi"), fontName);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected virtual string PrepareFontName(string name) {
			return name;
		}
		void ExportRunFontBold(CharacterProperties characterProperties) {
			if (characterProperties.UseFontBold)
				WriteWpBoolValue("b", characterProperties.FontBold);
		}
		void ExportRunFontItalic(CharacterProperties characterProperties) {
			if (characterProperties.UseFontItalic)
				WriteWpBoolValue("i", characterProperties.FontItalic);
		}
		void ExportRunAllCaps(CharacterProperties characterProperties) {
			if (characterProperties.UseAllCaps)
				WriteWpBoolValue("caps", characterProperties.AllCaps);
		}
		void ExportRunHidden(CharacterProperties characterProperties) {
			if (characterProperties.UseHidden)
				WriteWpBoolValue("vanish", characterProperties.Hidden);
		}
		void ExportRunForeColor(CharacterProperties characterProperties) {
			if (!characterProperties.UseForeColor)
				return;
			 WriteWpStringValue("color", ConvertColorToString(characterProperties.ForeColor));
		}
		void ExportDoubleFontSize(CharacterProperties characterProperties) {
			if (characterProperties.UseDoubleFontSize)
				WriteWpIntValue("sz", characterProperties.DoubleFontSize);
		}
		protected internal virtual void ExportRunFontUnderline(CharacterProperties characterProperties) {
			if (!characterProperties.UseFontUnderlineType && (!characterProperties.UseUnderlineColor || DXColor.IsTransparentOrEmpty(characterProperties.UnderlineColor)) && !characterProperties.UseUnderlineWordsOnly)
				return;
			WriteWpStartElement("u");
			try {
				if (characterProperties.UseUnderlineWordsOnly && characterProperties.UnderlineWordsOnly && characterProperties.UseFontUnderlineType && characterProperties.FontUnderlineType == UnderlineType.Single)
					WriteWpStringAttr("val", "words");
				else if (characterProperties.UseFontUnderlineType)
					WriteWpStringAttr("val", ConvertUnderlineType(characterProperties.FontUnderlineType));
				if (characterProperties.UseUnderlineColor && !DXColor.IsTransparentOrEmpty(characterProperties.UnderlineColor))
					  WriteWpStringAttr("color", ConvertColorToString(characterProperties.UnderlineColor));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportRunFontStrikeout(CharacterProperties characterProperties) {
			if (!characterProperties.UseFontStrikeoutType)
				return;
			if (characterProperties.FontStrikeoutType == StrikeoutType.Double)
				WriteWpBoolValue("dstrike", true);
			else if (characterProperties.FontStrikeoutType == StrikeoutType.None)
				WriteWpBoolValue("strike", false);
			else 
				WriteWpBoolValue("strike", true);
		}
		protected internal virtual void ExportRunFontScript(CharacterProperties characterProperties) {
			if (characterProperties.UseScript)
				WriteWpStringValue("vertAlign", ConvertScript(characterProperties.Script));
		}
		protected internal virtual void ExportRunLangInfo(CharacterProperties characterProperties) {
			if (!characterProperties.UseLangInfo)
				return;
			WriteWpStartElement("lang");
			try {
				if (characterProperties.LangInfo.Latin != null)
					WriteWpStringAttr("val", characterProperties.LangInfo.Latin.Name);
				if (characterProperties.LangInfo.Bidi != null)
					WriteWpStringAttr("bidi", characterProperties.LangInfo.Bidi.Name);
				if (characterProperties.LangInfo.EastAsia != null)
					WriteWpStringAttr("eastAsia", characterProperties.LangInfo.EastAsia.Name);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportRunNoProof(CharacterProperties characterProperties) {
			if (!characterProperties.UseNoProof)
				return;
			WriteWpStartElement("noProof");
			WriteWpBoolAttr("val", characterProperties.NoProof);
			WriteWpEndElement();
		}
		#endregion
		#region Paragraph
		protected internal override ParagraphIndex ExportParagraph(Paragraph paragraph) {
			WriteWpStartElement("p");
			try {
				ExportParagraphProperties(paragraph);
				return base.ExportParagraph(paragraph);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportParagraphProperties(Paragraph paragraph) {
			if (!ShouldExportParagraphProperties(paragraph))
				return;
			WriteWpStartElement("pPr");
			try {
				Action paragraphNumberingExporter = null;
				if (ShouldExportParagraphNumbering(paragraph.GetOwnNumberingListIndex()) && paragraph.ShouldExportNumbering())
					paragraphNumberingExporter = () => ExportParagraphListReference(paragraph);
				ExportParagraphPropertiesCore(paragraph, paragraphNumberingExporter);
				if (ShouldExportSectionProperties(paragraph))
					ExportSectionProperties(CurrentSection);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual bool ShouldExportParagraphNumbering(NumberingListIndex numberingListIndex) {
			return numberingListIndex == NumberingListIndex.NoNumberingList || numberingListIndex >= NumberingListIndex.MinValue;
		}
		protected internal virtual void ExportParagraphPropertiesCore(Paragraph paragraph, Action paragraphNumberingExporter) {
			if (paragraph.ParagraphStyleIndex > 0)
				WriteWpStringValue("pStyle", GetParagraphStyleId(paragraph.ParagraphStyleIndex));
			Action frameExporter = null;
			if (paragraph.FrameProperties != null)
				frameExporter = () => ExportParagraphFrame(paragraph.FrameProperties);
			Action tabsExporter = () => ExportTabProperties(paragraph.GetOwnTabs());
			ExportParagraphPropertiesCore(paragraph.ParagraphProperties, false, frameExporter, paragraphNumberingExporter, tabsExporter);
			WebSettings webSettings = DocumentModel.WebSettings;
			if (webSettings.IsBodyMarginsSet())
				WriteWpIntValue("divId", webSettings.GetHashCode());
			ExportRunProperties(PieceTable.Runs[paragraph.LastRunIndex]);
		}
		protected internal virtual void ExportParagraphPropertiesCore(ParagraphProperties properties, bool defaultParagraphProperties, Action framePropertiesExporter, Action paragraphNumberingExporter, Action tabsExporter) {
			if (properties.UseKeepWithNext)
				WriteWpBoolValue("keepNext", properties.KeepWithNext);
			if (properties.UseKeepLinesTogether)
				WriteWpBoolValue("keepLines", properties.KeepLinesTogether);
			if (properties.UsePageBreakBefore && properties.PageBreakBefore)
				WriteWpBoolValue("pageBreakBefore", properties.PageBreakBefore);
			if (framePropertiesExporter != null)
				framePropertiesExporter();
			if (properties.UseWidowOrphanControl)
				WriteWpBoolValue("widowControl", properties.WidowOrphanControl);
			if (paragraphNumberingExporter != null)
				paragraphNumberingExporter();
			if (properties.UseSuppressLineNumbers)
				WriteWpBoolValue(GetWordProcessingMLValue(new WordProcessingMLValue("suppressLineNumbers", "supressLineNumbers")), properties.SuppressLineNumbers);
			if (properties.UseLeftBorder || properties.UseRightBorder || properties.UseTopBorder || properties.UseBottomBorder)
				ExportParagraphBorders(properties, defaultParagraphProperties);
			if (properties.UseBackColor) {
				ExportParagraphBackground(properties.BackColor);
			}
			if (tabsExporter != null)
				tabsExporter();
			if (properties.UseSuppressHyphenation)
				WriteWpBoolValue("suppressAutoHyphens", properties.SuppressHyphenation);
			if (properties.UseSpacingBefore || properties.UseSpacingAfter || properties.UseLineSpacingType || properties.UseLineSpacing || properties.UseBeforeAutoSpacing || properties.UseAfterAutoSpacing)
				ExportParagraphSpacing(properties);
			if (properties.UseFirstLineIndentType || properties.UseFirstLineIndent || properties.UseLeftIndent || properties.UseRightIndent)
				ExportParagraphIndentation(properties);
			if (properties.UseContextualSpacing)
				WriteWpBoolValue("contextualSpacing", properties.ContextualSpacing);
			if (properties.UseAlignment)
				WriteWpStringValue("jc", ConvertAlignment(properties.Alignment));
			if (properties.UseOutlineLevel)
				ExportParagraphOutlineLevel(properties);
		}
		protected internal abstract void ExportParagraphListReference(Paragraph paragraph);
		protected internal abstract void ExportParagraphStyleListReference(NumberingListIndex numberingListIndex, int listLevelIndex);
		protected internal int GetNumberingListIndexForExport(NumberingListIndex numberingListIndex) {
			if (numberingListIndex == NumberingListIndex.NoNumberingList)
				return 0;
			else
				return ((IConvertToInt<NumberingListIndex>)numberingListIndex).ToInt() + 1;
		}
		protected internal virtual void ExportTabProperties(TabFormattingInfo tabs) {
			if (!ShouldExportTabProperties(tabs))
				return;
			WriteWpStartElement("tabs");
			try {
				int count = tabs.Count;
				for (int i = 0; i < count; i++)
					if (!tabs[i].IsDefault)
						ExportTab(tabs[i]);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportTab(TabInfo tab) {
			WriteWpStartElement("tab");
			try {
				if (tab.Deleted)
					WriteWpStringAttr("val", "clear");
				else
					WriteWpStringAttr("val", ConvertTabAlignment(tab.Alignment));
				int pos = Math.Max(UnitConverter.ModelUnitsToTwips(tab.Position), -31680);
				pos = Math.Min(pos, 31680);
				WriteWpIntAttr("pos", pos);
				WriteWpStringAttr("leader", ConvertTabLeader(tab.Leader));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportParagraphFrame(FrameProperties properties) {
			WriteWpStartElement("framePr");
			try {
				if (properties.UseWidth)
				WriteWpIntAttr("w", UnitConverter.ModelUnitsToTwips(properties.Width));
				if (properties.UseHeight)
				WriteWpIntAttr("h", UnitConverter.ModelUnitsToTwips(properties.Height));
				if (properties.UseHorizontalRule)
				WriteWpStringAttr(GetWordProcessingMLValue("hRule"), ConvertHorizontalRule(properties.HorizontalRule));
				if (properties.UseTextWrapType)
				WriteWpStringAttr(GetWordProcessingMLValue("wrap"), ConvertWrapType(properties.TextWrapType));
				if (properties.UseVerticalPositionType)
				WriteWpStringAttr(GetWordProcessingMLValue("vAnchor"), ConvertVerticalPositionType(properties.VerticalPositionType));
				if (properties.UseHorizontalPositionType)
				WriteWpStringAttr(GetWordProcessingMLValue("hAnchor"), ConvertHorizontalPositionType(properties.HorizontalPositionType));
				if (properties.UseX)
				WriteWpIntAttr("x", UnitConverter.ModelUnitsToTwips(properties.X));
				if (properties.HorizontalPositionAlignment != ParagraphFrameHorizontalPositionAlignment.None)
					WriteWpStringAttr(GetWordProcessingMLValue("xAlign"), ConvertHorizontalPositionAlignment(properties.HorizontalPositionAlignment));
				if (properties.UseY)
				WriteWpIntAttr("y", UnitConverter.ModelUnitsToTwips(properties.Y));
				if (properties.VerticalPositionAlignment != ParagraphFrameVerticalPositionAlignment.None)
					WriteWpStringAttr(GetWordProcessingMLValue("yAlign"), ConvertVerticalPositionAlignment(properties.VerticalPositionAlignment));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual string ConvertHorizontalRule(ParagraphFrameHorizontalRule horizontalRule) {
			switch (horizontalRule) {
				case ParagraphFrameHorizontalRule.AtLeast:
					return "atLeast";
				case ParagraphFrameHorizontalRule.Exact:
					return "exact";
				default:
					return "auto";
			}
		}
		protected internal virtual string ConvertWrapType(FloatingObjectTextWrapType wrapType) {
			switch (wrapType) {
				case FloatingObjectTextWrapType.Square:
					return "around";
				case FloatingObjectTextWrapType.None:
					return "none";
				case FloatingObjectTextWrapType.TopAndBottom:
					return "notBeside";
				case FloatingObjectTextWrapType.Through:
					return "through";
				case FloatingObjectTextWrapType.Tight:
					return "tight";
				default:
					return "auto";
			}
		}
		protected internal virtual string ConvertVerticalPositionType(FloatingObjectVerticalPositionType verticalPositionType) {
			switch (verticalPositionType) {
				case FloatingObjectVerticalPositionType.Page:
					return "page";
				case FloatingObjectVerticalPositionType.Margin:
					return "margin";
				default:
					return "text";
			}
		}
		protected internal virtual string ConvertHorizontalPositionType(FloatingObjectHorizontalPositionType horizontalPositionType) {
			switch (horizontalPositionType) {
				case FloatingObjectHorizontalPositionType.Margin:
					return "margin";
				case FloatingObjectHorizontalPositionType.Page:
					return "page";
				default:
					return "text";
			}
		}
		protected internal virtual string ConvertHorizontalPositionAlignment(FloatingObjectHorizontalPositionAlignment horizontalPositionAlignment) {
			switch (horizontalPositionAlignment) {
				case FloatingObjectHorizontalPositionAlignment.Center:
					return "center";
				case FloatingObjectHorizontalPositionAlignment.Inside:
					return "inside";
				case FloatingObjectHorizontalPositionAlignment.Left:
					return "left";
				case FloatingObjectHorizontalPositionAlignment.Outside:
					return "outside";
				case FloatingObjectHorizontalPositionAlignment.Right:
					return "right";
				default:
					return "";
			}
		}
		protected internal virtual string ConvertVerticalPositionAlignment(FloatingObjectVerticalPositionAlignment verticalPositionAlignment) {
			switch (verticalPositionAlignment) {
				case FloatingObjectVerticalPositionAlignment.Bottom:
					return "bottom";
				case FloatingObjectVerticalPositionAlignment.Center:
					return "center";
				case FloatingObjectVerticalPositionAlignment.Inside:
					return "inside";
				case FloatingObjectVerticalPositionAlignment.Outside:
					return "outside";
				case FloatingObjectVerticalPositionAlignment.Top:
					return "top";
				default:
					return "";
			}
		}
		protected internal virtual string ConvertWrapType(ParagraphFrameTextWrapType wrapType) {
			switch (wrapType) {
				case ParagraphFrameTextWrapType.Around:
					return "around";
				case ParagraphFrameTextWrapType.None:
					return "none";
				case ParagraphFrameTextWrapType.NotBeside:
					return "notBeside";
				case ParagraphFrameTextWrapType.Through:
					return "through";
				case ParagraphFrameTextWrapType.Tight:
					return "tight";
				default:
					return "auto";
			}
		}
		protected internal virtual string ConvertVerticalPositionType(ParagraphFrameVerticalPositionType verticalPositionType) {
			switch (verticalPositionType) {
				case ParagraphFrameVerticalPositionType.Margin:
					return "margin";
				case ParagraphFrameVerticalPositionType.Page:
					return "page";
				default:
					return "text";
			}
		}
		protected internal virtual string ConvertHorizontalPositionType(ParagraphFrameHorizontalPositionType horizontalPositionType) {
			switch (horizontalPositionType) {
				case ParagraphFrameHorizontalPositionType.Margin:
					return "margin";
				case ParagraphFrameHorizontalPositionType.Page:
					return "page";
				default:
					return "text";
			}
		}
		protected internal virtual string ConvertHorizontalPositionAlignment(ParagraphFrameHorizontalPositionAlignment horizontalPositionAlignment) {
			switch (horizontalPositionAlignment) {
				case ParagraphFrameHorizontalPositionAlignment.Center:
					return "center";
				case ParagraphFrameHorizontalPositionAlignment.Inside:
					return "inside";
				case ParagraphFrameHorizontalPositionAlignment.Left:
					return "left";
				case ParagraphFrameHorizontalPositionAlignment.Outside:
					return "outside";
				case ParagraphFrameHorizontalPositionAlignment.Right:
					return "right";
				default:
					return "";
			}
		}
		protected internal virtual string ConvertVerticalPositionAlignment(ParagraphFrameVerticalPositionAlignment verticalPositionAlignment) {
			switch (verticalPositionAlignment) {
				case ParagraphFrameVerticalPositionAlignment.Bottom:
					return "bottom";
				case ParagraphFrameVerticalPositionAlignment.Center:
					return "center";
				case ParagraphFrameVerticalPositionAlignment.Inline:
					return "inline";
				case ParagraphFrameVerticalPositionAlignment.Inside:
					return "inside";
				case ParagraphFrameVerticalPositionAlignment.Outside:
					return "outside";
				case ParagraphFrameVerticalPositionAlignment.Top:
					return "top";
				default:
					return "";
			}
		}
		protected internal virtual void ExportParagraphSpacing(ParagraphProperties properties) {
			WriteWpStartElement("spacing");
			try {
				if (properties.UseLineSpacingType)
					WriteWpStringAttr(GetWordProcessingMLValue("lineRule"), ConvertLineSpacing(properties.LineSpacingType));
				if (properties.UseLineSpacing || properties.UseLineSpacingType)
					WriteWpIntAttr("line", ConvertLineSpacingValue(properties.LineSpacingType, properties.LineSpacing));
				if (properties.UseSpacingBefore)
					WriteWpIntAttr("before", UnitConverter.ModelUnitsToTwips(properties.SpacingBefore));
				if (properties.UseSpacingAfter)
					WriteWpIntAttr("after", UnitConverter.ModelUnitsToTwips(properties.SpacingAfter));
				if (properties.UseBeforeAutoSpacing)
					WriteWpBoolAttr(GetWordProcessingMLValue(new WordProcessingMLValue("beforeAutospacing", "before-autospacing")), properties.BeforeAutoSpacing);
				if (properties.UseAfterAutoSpacing)
					WriteWpBoolAttr(GetWordProcessingMLValue(new WordProcessingMLValue("afterAutospacing", "after-autospacing")), properties.AfterAutoSpacing);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportParagraphOutlineLevel(ParagraphProperties properties) {
			int level = properties.OutlineLevel;
			if (level <= 0 || level >= 10)
				return;
			level--;
			WriteWpIntValue("outlineLvl", level);
		}
		protected internal virtual void ExportParagraphBackground(Color background) {
			WriteWpStartElement("shd");
			try {
				if (DXColor.IsTransparentOrEmpty(background)) {
					WriteWpStringAttr("val", ConvertShadingPattern(ShadingPattern.Clear));
					WriteWpStringAttr("fill", "auto");
				}
				else {
					WriteWpStringAttr("val", ConvertShadingPattern(ShadingPattern.Clear));
					WriteWpStringAttr("fill", ConvertColorToString(background));
				}
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportParagraphIndentation(ParagraphProperties properties) {
			WriteWpStartElement("ind");
			try {
				if (properties.UseFirstLineIndentType) {
					switch (properties.FirstLineIndentType) {
						case ParagraphFirstLineIndent.Hanging:
							WriteWpIntAttr("hanging", UnitConverter.ModelUnitsToTwips(properties.FirstLineIndent));
							if (properties.UseLeftIndent)
								WriteWpIntAttr("left", UnitConverter.ModelUnitsToTwips(properties.LeftIndent));
							break;
						case ParagraphFirstLineIndent.Indented:
							WriteWpIntAttr(GetWordProcessingMLValue("firstLine"), UnitConverter.ModelUnitsToTwips(properties.FirstLineIndent));
							if (properties.UseLeftIndent)
								WriteWpIntAttr("left", UnitConverter.ModelUnitsToTwips(properties.LeftIndent));
							break;
						case ParagraphFirstLineIndent.None:
							WriteWpIntAttr(GetWordProcessingMLValue("firstLine"), 0);
							if (properties.UseLeftIndent)
								WriteWpIntAttr("left", UnitConverter.ModelUnitsToTwips(properties.LeftIndent));
							break;
					}
				}
				else
					if (properties.UseLeftIndent)
						WriteWpIntAttr("left", UnitConverter.ModelUnitsToTwips(properties.LeftIndent));
				if (properties.UseRightIndent)
					WriteWpIntAttr("right", UnitConverter.ModelUnitsToTwips(properties.RightIndent));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportParagraphBorders(ParagraphProperties properties, bool defaultParagraphProperties) {
			bool shouldExportBottomBorder = properties.UseBottomBorder && ShouldExportParagraphBorder(defaultParagraphProperties, properties.BottomBorder);
			bool shouldExportLeftBorder = properties.UseLeftBorder && ShouldExportParagraphBorder(defaultParagraphProperties, properties.LeftBorder);
			bool shouldExportRightBorder = properties.UseRightBorder && ShouldExportParagraphBorder(defaultParagraphProperties, properties.RightBorder);
			bool shouldExportTopBorder = properties.UseTopBorder && ShouldExportParagraphBorder(defaultParagraphProperties, properties.TopBorder);
			bool shouldExportBorder = shouldExportBottomBorder || shouldExportLeftBorder || shouldExportRightBorder || shouldExportTopBorder;
			if (!shouldExportBorder)
				return;
			WriteWpStartElement("pBdr");
			try {
				if (shouldExportTopBorder)
					ExportParagraphBorder("top", properties.TopBorder);
				if (shouldExportLeftBorder)
					ExportParagraphBorder("left", properties.LeftBorder);
				if (shouldExportBottomBorder)
					ExportParagraphBorder("bottom", properties.BottomBorder);
				if (shouldExportRightBorder)
					ExportParagraphBorder("right", properties.RightBorder);
			}
			finally {
				WriteWpEndElement();
			}
		}
		bool ShouldExportParagraphBorder(bool defaultParagraphProperties, BorderInfo borderInfo) {
			if (defaultParagraphProperties)
				return borderInfo.Style != BorderLineStyle.None || borderInfo.Width != 0;
			else
				return true;
		}
		protected internal virtual void ExportParagraphBorder(string tag, BorderInfo border) {
			WriteWpStartElement(tag);
			try {
				WriteWpStringAttr("val", ConvertBorderLineStyle(border.Style));
				WriteWpIntAttr("sz", (int)UnitConverter.ModelUnitsToPointsF(border.Width * 8.0f));
				WriteWpIntAttr("space", (int)UnitConverter.ModelUnitsToPointsF(border.Offset));
				WriteWpBoolAttr("shadow", border.Shadow);
				WriteWpBoolAttr("frame", border.Frame);
				WriteWpStringAttr("color", ConvertColorToString(border.Color));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual int ConvertLineSpacingValue(ParagraphLineSpacing lineSpacing, float value) {
			if (lineSpacing == ParagraphLineSpacing.AtLeast || lineSpacing == ParagraphLineSpacing.Exactly)
				return (int)Math.Round(UnitConverter.ModelUnitsToTwipsF(value));
			else {
				if (lineSpacing == ParagraphLineSpacing.Single)
					value = 1;
				else if (lineSpacing == ParagraphLineSpacing.Double)
					value = 2;
				else if (lineSpacing == ParagraphLineSpacing.Sesquialteral)
					value = 1.5f;
				return (int)Math.Round(value * 240);
			}
		}
		protected internal virtual string ConvertTabAlignment(TabAlignmentType alignment) {
			WordProcessingMLValue result;
			if (!tabAlignmentTable.TryGetValue(alignment, out result))
				result = tabAlignmentTable[TabAlignmentType.Left];
			return GetWordProcessingMLValue(result);
		}
		protected internal virtual string ConvertTabLeader(TabLeaderType leaderType) {
			WordProcessingMLValue result;
			if (!tabLeaderTable.TryGetValue(leaderType, out result))
				result = tabLeaderTable[TabLeaderType.None];
			return GetWordProcessingMLValue(result);
		}
		protected internal virtual string ConvertAlignment(ParagraphAlignment alignment) {
			WordProcessingMLValue result;
			if (!paragraphAlignmentTable.TryGetValue(alignment, out result))
				result = paragraphAlignmentTable[ParagraphAlignment.Left];
			return GetWordProcessingMLValue(result);
		}
		protected internal virtual string ConvertLineSpacing(ParagraphLineSpacing lineSpacing) {
			WordProcessingMLValue result;
			if (!lineSpacingTable.TryGetValue(lineSpacing, out result))
				result = lineSpacingTable[ParagraphLineSpacing.Single];
			return GetWordProcessingMLValue(result);
		}
		protected string ConvertTextDirection(TextDirection value) {
			WordProcessingMLValue result;
			if (textDirectionTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(textDirectionTable[TextDirection.LeftToRightTopToBottom]);
		}
		protected string ConvertShadingPattern(ShadingPattern value) {
			WordProcessingMLValue result;
			if (shadingPatternTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(shadingPatternTable[ShadingPattern.Clear]);
		}
		protected string ConvertVerticalAlignement(VerticalAlignment value) {
			WordProcessingMLValue result;
			if (verticalAlignmentTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(verticalAlignmentTable[VerticalAlignment.Top]);
		}
		protected string ConvertWidthUnitTypes(WidthUnitType value) {
			WordProcessingMLValue result;
			if (widthUnitTypesTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(widthUnitTypesTable[WidthUnitType.ModelUnits]);
		}
		protected string ConvertTableLayoutType(TableLayoutType value) {
			WordProcessingMLValue result;
			if (tableLayoutTypeTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(tableLayoutTypeTable[TableLayoutType.Autofit]);
		}
		protected string ConvertBorderLineStyle(BorderLineStyle value) {
			WordProcessingMLValue result;
			if (borderLineStyleTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(borderLineStyleTable[BorderLineStyle.None]);
		}
		protected string ConvertHorizontalAlignMode(HorizontalAlignMode value) {
			WordProcessingMLValue result;
			if (horizontalAlignModeTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(horizontalAlignModeTable[HorizontalAlignMode.Left]);
		}
		protected string ConvertHorizontalAnchorTypes(HorizontalAnchorTypes value) {
			WordProcessingMLValue result;
			if (horizontalAnchorTypesTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(horizontalAnchorTypesTable[HorizontalAnchorTypes.Column]);
		}
		string ConvertConditionalStyleType(ConditionalTableStyleFormattingTypes styleType) {
			WordProcessingMLValue result;
			if (conditionalTableStyleFormattingTypesTable.TryGetValue(styleType, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(conditionalTableStyleFormattingTypesTable[ConditionalTableStyleFormattingTypes.WholeTable]);
		}
		protected string ConvertVerticalAlignMode(VerticalAlignMode value) {
			WordProcessingMLValue result;
			if (verticalAlignModeTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(verticalAlignModeTable[VerticalAlignMode.Bottom]);
		}
		protected string ConvertVerticalAnchorTypes(VerticalAnchorTypes value) {
			WordProcessingMLValue result;
			if (verticalAnchorTypesTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(verticalAnchorTypesTable[VerticalAnchorTypes.Margin]);
		}
		protected string ConvertHeightUnitType(HeightUnitType value) {
			WordProcessingMLValue result;
			if (heightUnitTypeTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(heightUnitTypeTable[HeightUnitType.Auto]);
		}
		protected string ConvertMergingState(MergingState value) {
			WordProcessingMLValue result;
			if (mergingStateTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(mergingStateTable[MergingState.Restart]);
		}
		protected string ConvertTableRowAlignment(TableRowAlignment value) {
			if (value == TableRowAlignment.Both)
				return GetWordProcessingMLValue(tableRowAlignmentTable[TableRowAlignment.Center]);
			WordProcessingMLValue result;
			if (tableRowAlignmentTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(tableRowAlignmentTable[TableRowAlignment.Left]);
		}
		protected internal virtual bool ShouldExportParagraphProperties(Paragraph paragraph) {
			ParagraphProperties properties = paragraph.ParagraphProperties;
			return
				properties.UseAlignment ||
				properties.UseFirstLineIndent ||
				properties.UseFirstLineIndentType ||
				properties.UseLeftIndent ||
				properties.UseRightIndent ||
				properties.UseLineSpacing ||
				properties.UseLineSpacingType ||
				properties.UseSpacingAfter ||
				properties.UseSpacingBefore ||
				properties.UseSuppressHyphenation ||
				properties.UseSuppressLineNumbers ||
				properties.UseContextualSpacing ||
				properties.UsePageBreakBefore ||
				properties.UseBeforeAutoSpacing ||
				properties.UseAfterAutoSpacing ||
				properties.UseKeepWithNext ||
				properties.UseKeepLinesTogether ||
				properties.UseWidowOrphanControl ||
				properties.UseOutlineLevel ||
				properties.UseBackColor ||
				paragraph.ParagraphStyleIndex > 0 ||
				ShouldExportRunProperties(PieceTable.Runs[paragraph.LastRunIndex]) ||
				paragraph.IsInList() ||
				ShouldExportSectionProperties(paragraph) ||
				ShouldExportTabProperties(paragraph.GetOwnTabs()) ||
				paragraph.FrameProperties != null;
		}
		protected internal virtual bool ShouldExportSectionProperties(Paragraph paragraph) {
			return PieceTable.IsMain &&
				paragraph.Index == CurrentSection.LastParagraphIndex &&
				paragraph.Index != new ParagraphIndex(PieceTable.Paragraphs.Count - 1) &&
				DocumentModel.DocumentCapabilities.SectionsAllowed;
		}
		protected internal virtual bool ShouldExportTabProperties(TabFormattingInfo tabs) {
			int nonDefaultTabCount = 0;
			int count = tabs.Count;
			for (int i = 0; i < count; i++)
				if (!tabs[i].IsDefault)
					nonDefaultTabCount++;
			return nonDefaultTabCount > 0;
		}
		#endregion
		#region Section
		protected internal override void ExportSection(Section section) {
			this.currentSection = section;
			base.ExportSection(section);
		}
		protected internal override void ExportSectionHeadersFooters(Section section) {
		}
		protected internal virtual void ExportSectionProperties(Section section) {
			WriteWpStartElement("sectPr");
			try {
				ExportSectionHeadersFootersCore(section);
				ExportSectionPropertiesCore(section);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportSectionPropertiesCore(Section section) {
			ExportSectionFootNote(section.FootNote);
			ExportSectionEndNote(section.EndNote);
			WriteWpStringValue("type", ConvertSectionStartType(section.GeneralSettings.StartType));
			ExportSectionPage(section.Page);
			ExportSectionMargins(section.Margins);
			ExportSectionLineNumbering(section.LineNumbering);
			ExportSectionPageNumbering(section.PageNumbering);
			ExportSectionColumns(section.Columns);
			ExportSectionGeneralSettings(section.GeneralSettings);
		}
		protected internal virtual void ExportSectionGeneralSettings(SectionGeneralSettings settings) {
			GeneralSectionInfo defaultSettings = DocumentModel.Cache.GeneralSectionInfoCache.DefaultItem;
			if (settings.VerticalTextAlignment != defaultSettings.VerticalTextAlignment)
				WriteWpStringValue("vAlign", ConvertVerticalAlignement(settings.VerticalTextAlignment));
			if (settings.DifferentFirstPage != defaultSettings.DifferentFirstPage)
				WriteWpBoolValue("titlePg", settings.DifferentFirstPage);
			if (settings.TextDirection != defaultSettings.TextDirection) {
				WordProcessingMLValue tag = new WordProcessingMLValue("textDirection", "textFlow");
				WriteWpStringValue(GetWordProcessingMLValue(tag), ConvertTextDirection(settings.TextDirection)); 
			}
			if (ShouldExportPaperSource(settings)) {
				WriteWpStartElement("paperSrc");
				try {
					if (settings.FirstPagePaperSource != defaultSettings.FirstPagePaperSource)
						WriteWpIntAttr("first", settings.FirstPagePaperSource);
					if (settings.OtherPagePaperSource != defaultSettings.OtherPagePaperSource)
						WriteWpIntAttr("other", settings.OtherPagePaperSource);
				}
				finally {
					WriteWpEndElement();
				}
			}
		}
		protected internal virtual bool ShouldExportPaperSource(SectionGeneralSettings settings) {
			GeneralSectionInfo defaultSettings = DocumentModel.Cache.GeneralSectionInfoCache.DefaultItem;
			return settings.FirstPagePaperSource != defaultSettings.FirstPagePaperSource ||
				settings.OtherPagePaperSource != defaultSettings.OtherPagePaperSource;
		}
		protected internal virtual void ExportSectionPage(SectionPage page) {
			if (!ShouldExportSectionPage(page))
				return;
			PageInfo defaultPage = DocumentModel.Cache.PageInfoCache.DefaultItem;
			WriteWpStartElement("pgSz");
			try {
				if (page.Width != defaultPage.Width)
					WriteWpIntAttr("w", UnitConverter.ModelUnitsToTwips(page.Width));
				if (page.Height != defaultPage.Height)
					WriteWpIntAttr("h", UnitConverter.ModelUnitsToTwips(page.Height));
				if (page.PaperKind != defaultPage.PaperKind)
					WriteWpIntAttr("code", (int)page.PaperKind);
				if (page.Landscape)
					WriteWpStringAttr("orient", page.Landscape ? "landscape" : "portrait");
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual bool ShouldExportSectionPage(SectionPage page) {
			PageInfo defaultPage = DocumentModel.Cache.PageInfoCache.DefaultItem;
			return page.Width != defaultPage.Width ||
				page.Height != defaultPage.Height ||
				page.PaperKind != defaultPage.PaperKind ||
				page.Landscape;
		}
		protected internal virtual void ExportSectionMargins(SectionMargins margins) {
			WriteWpStartElement("pgMar");
			try {
				WriteWpIntAttr("left", UnitConverter.ModelUnitsToTwips(margins.Left));
				WriteWpIntAttr("right", UnitConverter.ModelUnitsToTwips(margins.Right));
				WriteWpIntAttr("top", UnitConverter.ModelUnitsToTwips(margins.Top));
				WriteWpIntAttr("bottom", UnitConverter.ModelUnitsToTwips(margins.Bottom));
				WriteWpIntAttr("header", UnitConverter.ModelUnitsToTwips(margins.HeaderOffset));
				WriteWpIntAttr("footer", UnitConverter.ModelUnitsToTwips(margins.FooterOffset));
				WriteWpIntAttr("gutter", UnitConverter.ModelUnitsToTwips(margins.Gutter));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual bool ShouldExportSectionMargins(SectionMargins margins) {
			MarginsInfo defaultMargins = DocumentModel.Cache.MarginsInfoCache.DefaultItem;
			return margins.Left != defaultMargins.Left ||
				margins.Right != defaultMargins.Right ||
				margins.Top != defaultMargins.Top ||
				margins.Bottom != defaultMargins.Bottom ||
				margins.HeaderOffset != defaultMargins.HeaderOffset ||
				margins.FooterOffset != defaultMargins.FooterOffset ||
				margins.Gutter != defaultMargins.Gutter;
		}
		protected internal virtual void ExportSectionColumns(SectionColumns columns) {
			if (!DocumentFormatsHelper.ShouldExportSectionColumns(columns, DocumentModel))
				return;
			WriteWpStartElement("cols");
			try {
				WriteWpBoolAttr("equalWidth", columns.EqualWidthColumns);
				ColumnsInfo defaultColumns = DocumentModel.Cache.ColumnsInfoCache.DefaultItem;
				if (columns.EqualWidthColumns)
					ExportEqualWidthColumns(columns);
				else
					ExportNonUniformColumns(columns);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportEqualWidthColumns(SectionColumns columns) {
			ColumnsInfo defaultColumns = DocumentModel.Cache.ColumnsInfoCache.DefaultItem;
			if (columns.ColumnCount != defaultColumns.ColumnCount)
				WriteWpIntAttr("num", columns.ColumnCount);
			if (columns.Space != defaultColumns.Space)
				WriteWpIntAttr("space", UnitConverter.ModelUnitsToTwips(columns.Space));
			if (columns.DrawVerticalSeparator != defaultColumns.DrawVerticalSeparator)
				WriteWpBoolAttr("sep", columns.DrawVerticalSeparator);
		}
		protected internal virtual void ExportNonUniformColumns(SectionColumns columns) {
			ColumnInfoCollection collection = columns.GetColumns();
			int count = collection.Count;
			ColumnsInfo defaultColumns = DocumentModel.Cache.ColumnsInfoCache.DefaultItem;
			if (count != defaultColumns.ColumnCount)
				WriteWpIntAttr("num", count);
			for (int i = 0; i < count; i++)
				ExportColumn(collection[i]);
		}
		protected internal virtual void ExportColumn(ColumnInfo column) {
			WriteWpStartElement("col");
			try {
				WriteWpIntAttr("w", UnitConverter.ModelUnitsToTwips(column.Width));
				WriteWpIntAttr("space", UnitConverter.ModelUnitsToTwips(column.Space));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportSectionPageNumbering(SectionPageNumbering numbering) {
			if (!ShouldExportPageNumbering(numbering))
				return;
			PageNumberingInfo defaultPageNumbering = DocumentModel.Cache.PageNumberingInfoCache.DefaultItem;
			WriteWpStartElement("pgNumType");
			try {
				if (numbering.StartingPageNumber != defaultPageNumbering.StartingPageNumber)
					WriteWpIntAttr("start", numbering.StartingPageNumber);
				if (numbering.NumberingFormat != defaultPageNumbering.NumberingFormat)
					WriteWpStringAttr("fmt", ConvertNumberFormat(numbering.NumberingFormat));
				if (numbering.ChapterSeparator != defaultPageNumbering.ChapterSeparator)
					WriteWpStringAttr(GetWordProcessingMLValue("chapSep"), ConvertChapterSeparator(numbering.ChapterSeparator));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual bool ShouldExportPageNumbering(SectionPageNumbering numbering) {
			PageNumberingInfo defaultPageNumbering = DocumentModel.Cache.PageNumberingInfoCache.DefaultItem;
			return numbering.StartingPageNumber != defaultPageNumbering.StartingPageNumber ||
				numbering.NumberingFormat != defaultPageNumbering.NumberingFormat ||
				numbering.ChapterSeparator != defaultPageNumbering.ChapterSeparator;
		}
		protected internal virtual void ExportSectionLineNumbering(SectionLineNumbering numbering) {
			if (!ShouldExportLineNumbering(numbering))
				return;
			LineNumberingInfo defaultLineNumbering = DocumentModel.Cache.LineNumberingInfoCache.DefaultItem;
			WriteWpStartElement("lnNumType");
			try {
				if (numbering.StartingLineNumber != defaultLineNumbering.StartingLineNumber)
					WriteWpIntAttr("start", numbering.StartingLineNumber);
				if (numbering.Step != defaultLineNumbering.Step)
					WriteWpIntAttr(GetWordProcessingMLValue("countBy"), numbering.Step);
				if (numbering.Distance != defaultLineNumbering.Distance)
					WriteWpIntAttr("distance", UnitConverter.ModelUnitsToTwips(numbering.Distance));
				if (numbering.NumberingRestartType != defaultLineNumbering.NumberingRestartType)
					WriteWpStringAttr("restart", ConvertLineNumberingRestartType(numbering.NumberingRestartType));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual bool ShouldExportLineNumbering(SectionLineNumbering numbering) {
			LineNumberingInfo defaultLineNumbering = DocumentModel.Cache.LineNumberingInfoCache.DefaultItem;
			return numbering.Step != defaultLineNumbering.Step;
		}
		protected internal virtual void ExportSectionFootNote(SectionFootNote note) {
			if (!ShouldExportSectionFootNote(note))
				return;
			ExportSectionFootNoteCore("footnotePr", note, DocumentModel.Cache.FootNoteInfoCache[FootNoteInfoCache.DefaultFootNoteItemIndex]);
		}
		protected internal virtual bool ShouldExportSectionFootNote(SectionFootNote note) {
			return DocumentModel.DocumentCapabilities.FootNotesAllowed && note.Index != FootNoteInfoCache.DefaultFootNoteItemIndex;
		}
		protected internal virtual void ExportSectionEndNote(SectionFootNote note) {
			if (!ShouldExportSectionEndNote(note))
				return;
			ExportSectionFootNoteCore("endnotePr", note, DocumentModel.Cache.FootNoteInfoCache[FootNoteInfoCache.DefaultEndNoteItemIndex]);
		}
		protected internal virtual bool ShouldExportSectionEndNote(SectionFootNote note) {
			return DocumentModel.DocumentCapabilities.EndNotesAllowed && note.Index != FootNoteInfoCache.DefaultEndNoteItemIndex;
		}
		protected internal virtual void ExportSectionFootNoteCore(string tagName, SectionFootNote note, FootNoteInfo defaultInfo) {
			WriteWpStartElement(tagName);
			try {
				if (note.Position != defaultInfo.Position)
					WriteWpStringValue("pos", ConvertFootNotePlacement(note.Position));
				if (note.StartingNumber != defaultInfo.StartingNumber)
					WriteWpIntValue("numStart", note.StartingNumber);
				if (note.NumberingFormat != defaultInfo.NumberingFormat)
					WriteWpStringValue("numFmt", ConvertNumberFormat(note.NumberingFormat));
				if (note.NumberingRestartType != defaultInfo.NumberingRestartType)
					WriteWpStringValue("numRestart", ConvertLineNumberingRestartType(note.NumberingRestartType));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected string ConvertSectionStartType(SectionStartType value) {
			WordProcessingMLValue result;
			if (sectionStartTypeTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(sectionStartTypeTable[SectionStartType.NextPage]);
		}
		protected string ConvertLineNumberingRestartType(LineNumberingRestart value) {
			WordProcessingMLValue result;
			if (lineNumberingRestartTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(lineNumberingRestartTable[LineNumberingRestart.NewPage]);
		}
		protected string ConvertFootNotePlacement(FootNotePosition value) {
			WordProcessingMLValue result;
			if (footNotePlacementTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(footNotePlacementTable[FootNotePosition.BottomOfPage]);
		}
		protected string ConvertNumberFormat(NumberingFormat value) {
			WordProcessingMLValue result;
			if (pageNumberingFormatTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(pageNumberingFormatTable[NumberingFormat.Decimal]);
		}
		protected string ConvertChapterSeparator(char value) {
			WordProcessingMLValue result;
			if (chapterSeparatorsTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(chapterSeparatorsTable['.']);
		}
		#endregion
		#region Table
		protected internal override ParagraphIndex ExportTable(TableInfo tableInfo) {
			WriteWpStartElement("tbl");
			try {
				Table table = tableInfo.Table;
				if (table.TableProperties.Info.Value != TablePropertiesOptions.Mask.UseNone || table.StyleIndex > 0)
					ExportTableProperties(table);
				else
					WriteWpEmptyElement("tblPr");
				WriteWpEmptyElement("tblGrid");
				return base.ExportTable(tableInfo);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportTableProperties(Table table) {
			WriteWpStartElement("tblPr");
			try {
				if (table.StyleIndex > 0)
					WriteWpStringValue("tblStyle", GetTableStyleId(table.StyleIndex));
				ExportTablePropertiesCore(table.TableProperties);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportTablePropertiesCore(TableProperties tableProperties) {
			ExportTablePropertiesCore(tableProperties, true);
		}
		protected void ExportTablePropertiesCore(TableProperties tableProperties, bool exportTableLayout) {
			if (ShouldExportFloatingPosition(tableProperties.FloatingPosition))
				ExportFloatingPosition(tableProperties.FloatingPosition);
			if (tableProperties.IsTableOverlap != true)
				ExportIsTableOverlap(tableProperties.IsTableOverlap);
			if (tableProperties.UseTableStyleRowBandSize)
				WriteWpIntValue("tblStyleRowBandSize", Math.Max(tableProperties.TableStyleRowBandSize, 1));
			if (tableProperties.UseTableStyleColBandSize)
				WriteWpIntValue("tblStyleColBandSize", Math.Max(tableProperties.TableStyleColBandSize, 1));
			if (tableProperties.UsePreferredWidth)
				ExportWidthUnitValue("tblW", tableProperties.PreferredWidth);
			if (tableProperties.UseTableAlignment)
				WriteWpStringValue("jc", ConvertTableRowAlignment(tableProperties.TableAlignment));
			if (tableProperties.UseCellSpacing)
				ExportWidthUnitValue("tblCellSpacing", tableProperties.CellSpacing);
			if (tableProperties.UseTableIndent)
				ExportWidthUnitValue("tblInd", tableProperties.TableIndent);
			ExportTableBorders(tableProperties.Borders);
			if (tableProperties.BackgroundColor != DXColor.Empty)
				ExportTableBackground(tableProperties.BackgroundColor);
			if (tableProperties.UseTableLayout && exportTableLayout)
				ExportTableLayout(tableProperties.TableLayout);
			ExportCellMargins(tableProperties);
			if (tableProperties.TableLook != TableLookTypes.None)
				ExportTableLook(tableProperties.TableLook);
		}
		void ExportCellMargins(TableProperties tableProperties) {
			if (!tableProperties.UseLeftMargin && !tableProperties.UseTopMargin && !tableProperties.UseRightMargin && !tableProperties.UseBottomMargin)
				return;
			CellMargins cellMargins = tableProperties.CellMargins;
			WriteWpStartElement("tblCellMar");
			try {
				if (tableProperties.UseTopMargin)
					ExportCellMargin("top", cellMargins.Top);
				if (tableProperties.UseLeftMargin)
					ExportCellMargin("left", cellMargins.Left);
				if (tableProperties.UseBottomMargin)
					ExportCellMargin("bottom", cellMargins.Bottom);
				if (tableProperties.UseRightMargin)
					ExportCellMargin("right", cellMargins.Right);
			}
			finally {
				WriteWpEndElement();
			}
		}
		void ExportCellMargin(string tag, MarginUnitBase margin) {
			WriteWpStartElement(tag);
			try {
				int value = margin.Value;
				WidthUnitType type = margin.Type;
				switch (margin.Type) {
					case WidthUnitType.ModelUnits:
						value = DocumentModel.UnitConverter.ModelUnitsToTwips(margin.Value);
						break;
					case WidthUnitType.Nil:
						if (value == 0)
							type = WidthUnitType.ModelUnits;
						break;
					default:
						break;
				}
				WriteWpIntAttr("w", value);
				WriteWpStringAttr("type", ConvertWidthUnitTypes(type));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportTableBorders(TableBorders borders) {
			if (!ShouldExportTableBorders(borders))
				return;
			WriteWpStartElement("tblBorders");
			try {
				if (borders.UseTopBorder)
					ExportTableBorder("top", borders.TopBorder);
				if (borders.UseLeftBorder)
					ExportTableBorder("left", borders.LeftBorder);
				if (borders.UseBottomBorder)
					ExportTableBorder("bottom", borders.BottomBorder);
				if (borders.UseRightBorder)
					ExportTableBorder("right", borders.RightBorder);
				if (borders.UseInsideHorizontalBorder)
					ExportTableBorder("insideH", borders.InsideHorizontalBorder);
				if (borders.UseInsideVerticalBorder)
					ExportTableBorder("insideV", borders.InsideVerticalBorder);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual bool ShouldExportTableBorders(TableBorders borders) {
			return borders.UseBottomBorder || borders.UseLeftBorder || borders.UseRightBorder || borders.UseTopBorder ||
							borders.UseInsideHorizontalBorder || borders.UseInsideVerticalBorder;
		}
		protected internal virtual void ExportTableBorder(string tag, BorderBase border) {
			WriteWpStartElement(tag);
			try {
				WriteWpStringAttr("val", ConvertBorderLineStyle(border.Style));
				WriteWpIntAttr("sz", (int)UnitConverter.ModelUnitsToPointsF(border.Width * 8.0f));
				WriteWpIntAttr("space", (int)UnitConverter.ModelUnitsToPointsF(border.Offset));
				WriteWpBoolAttr("shadow", border.Shadow);
				WriteWpBoolAttr("frame", border.Frame);
				WriteWpStringAttr("color", ConvertColorToString(border.Color));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual bool ShouldExportCellMargins(CellMargins cellMargins) {
			return !ForbidExportWidthUnit(cellMargins.Bottom) || !ForbidExportWidthUnit(cellMargins.Left) ||
				!ForbidExportWidthUnit(cellMargins.Top) || !ForbidExportWidthUnit(cellMargins.Right);
		}
		protected internal virtual void ExportCellMargins(string tag, CellMargins cellMargins) {
			WriteWpStartElement(tag);
			try {
				ExportWidthUnitValue("top", cellMargins.Top);
				ExportWidthUnitValue("left", cellMargins.Left);
				ExportWidthUnitValue("bottom", cellMargins.Bottom);
				ExportWidthUnitValue("right", cellMargins.Right);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportWidthUnitValue(string tag, WidthUnit value) {
			if (ForbidExportWidthUnit(value))
				return;
			WriteWpStartElement(tag);
			try {
				ExportWidthUnit(value);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual bool ForbidExportWidthUnit(WidthUnit widthUnit) {
			return widthUnit.Type == WidthUnitType.Nil && widthUnit.Value == 0;
		}
		protected internal virtual void ExportWidthUnit(WidthUnit widthUnit) {
			int value = widthUnit.Type == WidthUnitType.ModelUnits ? DocumentModel.UnitConverter.ModelUnitsToTwips(widthUnit.Value) : widthUnit.Value;
			WriteWpIntAttr("w", value);
			WriteWpStringAttr("type", ConvertWidthUnitTypes(widthUnit.Type));
		}
		protected internal virtual void ExportTableLook(TableLookTypes tableLook) {
			int intValue = Convert.ToInt32(tableLook);
			string strValue = Convert.ToString(intValue, 16).ToUpper(CultureInfo.InvariantCulture);
			strValue = String.Format("{0,4}", strValue);
			Regex rgx = new Regex("\\s");
			strValue = rgx.Replace(strValue, "0");
			WriteWpStringValue("tblLook", strValue);
		}
		protected internal virtual void ExportTableLayout(TableLayoutType tableLayout) {
			WriteWpStartElement("tblLayout");
			try {
				WriteWpStringAttr("type", ConvertTableLayoutType(tableLayout));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportTableBackground(Color background) {
			ExportTextShading(background);
		}
		void ExportRunBackColor(CharacterProperties characterProperties) {
			if (characterProperties.UseBackColor)
				if (characterProperties.BackColor.ToArgb() != transparentClr) {
					  ExportTextShading(characterProperties.BackColor);
				}
		}
		protected internal void ExportTextShading(Color background) {
			WriteWpStartElement("shd");
			try {
				if (background == DXColor.Empty) {
					WriteWpStringAttr("val", ConvertShadingPattern(ShadingPattern.Nil));
					WriteWpStringAttr("fill", "auto");
				}
				else {
					WriteWpStringAttr("val", ConvertShadingPattern(ShadingPattern.Clear));
					WriteWpStringAttr("fill", ConvertColorToString(background));
				}
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportIsTableOverlap(bool isTableOverlap) {
			if (isTableOverlap)
				WriteWpStringValue("tblOverlap", "overlap");
			else
				WriteWpStringValue("tblOverlap", "never");
		}
		protected internal virtual bool ShouldExportFloatingPosition(TableFloatingPosition floatingPosition) {
			return floatingPosition.TextWrapping != TextWrapping.Never;
		}
		protected internal virtual void ExportFloatingPosition(TableFloatingPosition floatingPosition) {
			WriteWpStartElement("tblpPr");
			try {
				if (floatingPosition.BottomFromText != 0)
					WriteWpIntAttr("bottomFromText", UnitConverter.ModelUnitsToTwips(floatingPosition.BottomFromText));
				if (floatingPosition.LeftFromText != 0)
					WriteWpIntAttr("leftFromText", UnitConverter.ModelUnitsToTwips(floatingPosition.LeftFromText));
				if (floatingPosition.RightFromText != 0)
					WriteWpIntAttr("rightFromText", UnitConverter.ModelUnitsToTwips(floatingPosition.RightFromText));
				if (floatingPosition.TopFromText != 0)
					WriteWpIntAttr("topFromText", UnitConverter.ModelUnitsToTwips(floatingPosition.TopFromText));
				if (floatingPosition.TableHorizontalPosition != 0)
					WriteWpIntAttr("tblpX", UnitConverter.ModelUnitsToTwips(floatingPosition.TableHorizontalPosition));
				else
					WriteWpIntAttr("tblpX", 1);
				if (floatingPosition.TableVerticalPosition != 0)
					WriteWpIntAttr("tblpY", UnitConverter.ModelUnitsToTwips(floatingPosition.TableVerticalPosition));
				else
					WriteWpIntAttr("tblpY", 1);
				if (floatingPosition.HorizontalAnchor != HorizontalAnchorTypes.Column)
					WriteWpStringAttr("horzAnchor", ConvertHorizontalAnchorTypes(floatingPosition.HorizontalAnchor));
				if (floatingPosition.VerticalAnchor != VerticalAnchorTypes.Margin)
					WriteWpStringAttr("vertAnchor", ConvertVerticalAnchorTypes(floatingPosition.VerticalAnchor));
				if (floatingPosition.IsHorizontalRelativePositionUse)
					WriteWpStringAttr("tblpXSpec", ConvertHorizontalAlignMode(floatingPosition.HorizontalAlign));
				if (floatingPosition.IsVerticalRelativePositionUse)
					WriteWpStringAttr("tblpYSpec", ConvertVerticalAlignMode(floatingPosition.VerticalAlign));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportRow(TableRow row, TableInfo tableInfo) {
			WriteWpStartElement("tr");
			try {
				ExportTablePropertiesException(row.TablePropertiesException);
				if (row.Properties.Info.Value != TableRowPropertiesOptions.Mask.UseNone)
					ExportTableRowProperties(row.Properties);
				base.ExportRow(row, tableInfo);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportTablePropertiesException(TableProperties tableProperties) {
			bool shouldExport = tableProperties.UsePreferredWidth || tableProperties.UseTableAlignment || tableProperties.UseCellSpacing ||
				tableProperties.UseTableIndent || ShouldExportTableBorders(tableProperties.Borders) || tableProperties.UseBackgroundColor ||
				tableProperties.UseTableLayout || ShouldExportCellMargins(tableProperties.CellMargins) ||
				tableProperties.TableLook != TableLookTypes.None;
			if (!shouldExport)
				return;
			WriteWpStartElement("tblPrEx");
			try {
				if (tableProperties.UsePreferredWidth)
					ExportWidthUnitValue("tblW", tableProperties.PreferredWidth);
				if (tableProperties.UseTableAlignment)
					WriteWpStringValue("jc", ConvertTableRowAlignment(tableProperties.TableAlignment));
				if (tableProperties.UseCellSpacing)
					ExportWidthUnitValue("tblCellSpacing", tableProperties.CellSpacing);
				if (tableProperties.UseTableIndent)
					ExportWidthUnitValue("tblInd", tableProperties.TableIndent);
				ExportTableBorders(tableProperties.Borders);
				if (tableProperties.UseBackgroundColor)
					ExportTableBackground(tableProperties.BackgroundColor);
				if (tableProperties.UseTableLayout)
					ExportTableLayout(tableProperties.TableLayout);
				if (ShouldExportCellMargins(tableProperties.CellMargins))
					ExportCellMargins("tblCellMar", tableProperties.CellMargins);
				if (tableProperties.TableLook != TableLookTypes.None)
					ExportTableLook(tableProperties.TableLook);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportTableRowProperties(TableRowProperties rowProperties) {
			ExportTableRowProperties(rowProperties, false);
		}
		protected internal virtual void ExportTableRowProperties(TableRowProperties rowProperties, bool isStyle) {
			WriteWpStartElement("trPr");
			try {
				if (rowProperties.UseGridBefore)
					WriteWpIntValue("gridBefore", rowProperties.GridBefore);
				if (rowProperties.UseGridAfter)
					WriteWpIntValue("gridAfter", rowProperties.GridAfter);
				if (rowProperties.UseWidthBefore && !isStyle)
					ExportWidthUnitValue("wBefore", rowProperties.WidthBefore);
				if (rowProperties.UseWidthAfter && !isStyle)
					ExportWidthUnitValue("wAfter", rowProperties.WidthAfter);
				if (rowProperties.CantSplit)
					WriteWpEmptyElement("cantSplit");
				if (rowProperties.Height.Value != 0)
					ExportTableRowHeight(rowProperties.Height);
				if (rowProperties.Header)
					WriteWpEmptyElement("tblHeader");
				if (rowProperties.UseCellSpacing)
					ExportWidthUnitValue("tblCellSpacing", rowProperties.CellSpacing);
				if (rowProperties.TableRowAlignment != TableRowAlignment.Left)
					WriteWpStringValue("jc", ConvertTableRowAlignment(rowProperties.TableRowAlignment));
				if (rowProperties.HideCellMark)
					WriteWpBoolValue("hidden", rowProperties.HideCellMark);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportTableRowHeight(HeightUnit height) {
			WriteWpStartElement("trHeight");
			try {
				ExportHeightUnit(height);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportHeightUnit(HeightUnit heightUnit) {
			if (heightUnit.Type != HeightUnitType.Auto)
				WriteWpStringAttr("hRule", ConvertHeightUnitType(heightUnit.Type));
			WriteWpIntAttr("val", DocumentModel.UnitConverter.ModelUnitsToTwips(heightUnit.Value));
		}
		protected internal override void ExportCell(TableCell cell, TableInfo tableInfo) {
			WriteWpStartElement("tc");
			try {
				if (AllowExportTableCellProperties(cell)) 
					ExportTableCellProperties(cell);
				if (!ForbidExecuteBaseExportTableCell)
					base.ExportCell(cell, tableInfo);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual bool AllowExportTableCellProperties(TableCell cell) {
			TableCellProperties cellProperties = cell.Properties;
			return cellProperties.Info.Value != TableCellPropertiesOptions.Mask.UseNone || cellProperties.ColumnSpan > 1 ||
				cellProperties.VerticalMerging != MergingState.None;
		}
		void ExportTableCellPropertiesCore(TableCellProperties cellProperties) {
			ExportTableCellPropertiesCore(cellProperties, true);
		}
		void ExportTableCellPropertiesCore(TableCellProperties cellProperties, bool exportBorders) {
			if (cellProperties.UseCellConditionalFormatting)
				ExportConditionalFormatting(cellProperties.CellConditionalFormatting);
			if (cellProperties.UsePreferredWidth)
				ExportWidthUnitValue("tcW", cellProperties.PreferredWidth);
			if (cellProperties.ColumnSpan > 1)
				WriteWpIntValue("gridSpan", cellProperties.ColumnSpan);
			if (cellProperties.VerticalMerging != MergingState.None)
				ExportTableCellPropertiesVerticalMerging(cellProperties.VerticalMerging);
			if (exportBorders)
				ExportTableCellBorders(cellProperties.Borders);
			ExportTableCellBackgroundColor(cellProperties);
			if (cellProperties.UseNoWrap)
				WriteWpEmptyOrFalseValue("noWrap", cellProperties.NoWrap);
			if (ShouldExportCellMargins(cellProperties.CellMargins))
				ExportCellMargins("tcMar", cellProperties.CellMargins);
			if (cellProperties.UseTextDirection)
				WriteWpStringValue("textDirection", ConvertTextDirection(cellProperties.TextDirection));
			if (cellProperties.UseFitText)
				WriteWpEmptyOrFalseValue("tcFitText", cellProperties.FitText);
			if (cellProperties.UseVerticalAlignment)
				WriteWpStringValue("vAlign", ConvertVerticalAlignement(cellProperties.VerticalAlignment));
			if (cellProperties.UseHideCellMark)
				WriteWpEmptyOrFalseValue("hideMark", cellProperties.HideCellMark);
		}
		protected internal virtual void ExportTableCellProperties(TableCellProperties cellProperties) {
			ExportTableCellProperties(cellProperties, true);
		}
		protected internal virtual void ExportTableCellProperties(TableCellProperties cellProperties, bool exportBorders) {
			WriteWpStartElement("tcPr");
			try {
				ExportTableCellPropertiesCore(cellProperties, exportBorders);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void WriteTableCellStyle(TableCell cell) {
		}
		protected internal virtual void ExportTableCellProperties(TableCell cell) {
			WriteWpStartElement("tcPr");
			try {
				WriteTableCellStyle(cell);
				ExportTableCellPropertiesCore(cell.Properties);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportTableCellPropertiesVerticalMerging(MergingState verticalMerging) {
			WriteWpStringValue("vMerge", ConvertMergingState(verticalMerging));
		}
		protected internal virtual void ExportTableCellBackgroundColor(TableCellProperties cellProperties) {
			if (!cellProperties.UseBackgroundColor && !cellProperties.UseForegroundColor)
				return;
			documentContentWriter.WriteStartElement(WordProcessingPrefix, "shd", WordProcessingNamespace);
			try {
				if (cellProperties.UseShading)
					documentContentWriter.WriteAttributeString(WordProcessingPrefix, "val", WordProcessingNamespace, ConvertShadingPattern(cellProperties.ShadingPattern));
				else
					documentContentWriter.WriteAttributeString(WordProcessingPrefix, "val", WordProcessingNamespace, "clear");
				if (cellProperties.UseForegroundColor && !DXColor.IsEmpty(cellProperties.ForegroundColor))
					 documentContentWriter.WriteAttributeString(WordProcessingPrefix, "color", WordProcessingNamespace, ConvertColorToString(cellProperties.ForegroundColor));
				else
					documentContentWriter.WriteAttributeString(WordProcessingPrefix, "color", WordProcessingNamespace, "auto");
				Color color = cellProperties.BackgroundColor;
				if (cellProperties.UseBackgroundColor)
					 documentContentWriter.WriteAttributeString(WordProcessingPrefix, "fill", WordProcessingNamespace, (DXColor.IsEmpty(color)) ? "FFFFFF" : ConvertColorToString(color));
				else
					documentContentWriter.WriteAttributeString(WordProcessingPrefix, "fill", WordProcessingNamespace, "FFFFFF");
			}
			finally {
				documentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void ExportTableCellBorders(TableCellBorders borders) {
			bool hasBorders = borders.UseBottomBorder || borders.UseInsideHorizontalBorder || borders.UseInsideVerticalBorder ||
				borders.UseLeftBorder || borders.UseRightBorder || borders.UseTopBorder || borders.UseTopLeftDiagonalBorder || borders.UseTopRightDiagonalBorder;
			if (!hasBorders)
				return;
			WriteWpStartElement("tcBorders");
			try {
				if (borders.UseTopBorder)
					ExportTableBorder("top", borders.TopBorder);
				if (borders.UseLeftBorder)
					ExportTableBorder("left", borders.LeftBorder);
				if (borders.UseBottomBorder)
					ExportTableBorder("bottom", borders.BottomBorder);
				if (borders.UseRightBorder)
					ExportTableBorder("right", borders.RightBorder);
				if (borders.UseInsideHorizontalBorder)
					ExportTableBorder("insideH", borders.InsideHorizontalBorder);
				if (borders.UseInsideVerticalBorder)
					ExportTableBorder("insideV", borders.InsideVerticalBorder);
				if (borders.UseTopLeftDiagonalBorder)
					ExportTableBorder("tl2br", borders.TopLeftDiagonalBorder);
				if (borders.UseTopRightDiagonalBorder)
					ExportTableBorder("tr2bl", borders.TopRightDiagonalBorder);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportConditionalFormatting(ConditionalTableStyleFormattingTypes conditionalFormatting) {
			int intValue = Convert.ToInt32(conditionalFormatting);
			string strValue = Convert.ToString(intValue, 2);
			strValue = String.Format("{0,12}", strValue);
			Regex rgx = new Regex("\\s");
			strValue = rgx.Replace(strValue, "0");
			WriteWpStringValue("cnfStyle", strValue);
		}
		#endregion
		#region Style
		protected internal virtual void ExportStylesCore() {
			WriteWpStartElement("styles");
			try {
				ExportDocumentDefaults();
				ExportParagraphStyles();
				ExportCharacterStyles();
				ExportTableStyles();
				ExportTableCellStyles();
				ExportNumberingListStyles();
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal abstract void ExportDocumentDefaults();
		protected internal abstract void ExportDocumentCharacterDefaults();
		protected internal abstract void ExportDocumentParagraphDefaults();
		protected internal virtual void ExportParagraphStyles() {
			ParagraphStyleCollection styles = DocumentModel.ParagraphStyles;
			int count = styles.Count;
			for (int i = 0; i < count; i++)
				ExportParagraphStyle(i);
		}
		protected internal virtual void ExportCharacterStyles() {
			CharacterStyleCollection styles = DocumentModel.CharacterStyles;
			int count = styles.Count;
			for (int i = 0; i < count; i++)
				ExportCharacterStyle(i);
		}
		protected internal virtual void ExportTableStyles() {
			TableStyleCollection styles = DocumentModel.TableStyles;
			int count = styles.Count;
			for (int i = 0; i < count; i++) {
				ExportTableStyle(i);
			}
		}
		protected internal virtual void ExportNumberingListStyles() {
			NumberingListStyleCollection styles = DocumentModel.NumberingListStyles;
			int count = styles.Count;
			for (int i = 0; i < count; i++)
				ExportNumberingListStyle(i);
		}
		protected internal virtual void ExportTableCellStyles() {
		}
		protected internal virtual void ExportStyleName(IStyle style) {
			WriteWpStringValue("name", style.StyleName);
		}
		protected internal virtual void ExportCharacterStyle(int styleIndex) {
			CharacterStyle style = DocumentModel.CharacterStyles[styleIndex];
			if (style.Deleted)
				return;
			WriteWpStartElement("style");
			try {
				WriteWpStringAttr("type", "character");
				WriteWpStringAttr("styleId", GetCharacterStyleId(styleIndex));
				if (styleIndex == 0)
					WriteWpBoolAttr("default", true);
				ExportStyleName(style);
				ExportParentCharacterStyle(style);
				ExportLinkedCharacterStyle(style);
				if (style.Hidden)
					WriteWpEmptyElement("hidden");
				if (style.Semihidden)
					WriteWpEmptyElement("semiHidden");
				WriteWpBoolValueAsTag("qFormat", style.Primary);
				ExportStyleCharacterProperties(style.CharacterProperties);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportNumberingListStyle(int styleIndex) {
			NumberingListStyle style = DocumentModel.NumberingListStyles[styleIndex];
			if (style.Deleted)
				return;
			WriteWpStartElement("style");
			try {
				WriteWpStringAttr("type", "numbering");
				WriteWpStringAttr("styleId", GetNumberingStyleId(styleIndex));
				ExportStyleName(style);
				if (style.Hidden)
					WriteWpEmptyElement("hidden");
				if (style.Semihidden)
					WriteWpEmptyElement("semiHidden");
				WriteWpBoolValueAsTag("qFormat", style.Primary);
				if (style.NumberingListIndex >= NumberingListIndex.MinValue) {
					WriteWpStartElement("pPr");
					WriteWpStartElement("numPr");
					WriteWpStartElement("numId");
					WriteWpIntAttr("val", GetNumberingListIndexForExport(style.NumberingListIndex));
					WriteWpEndElement();
					WriteWpEndElement();
					WriteWpEndElement();
				}
			}
			finally {
				WriteWpEndElement();
			}
		}
		void ExportParentCharacterStyle(CharacterStyle style) {
			if (style.Parent != null) {
				int parentStyleIndex = DocumentModel.CharacterStyles.IndexOf(style.Parent);
				if (parentStyleIndex >= 0)
					WriteWpStringValue("basedOn", GetCharacterStyleId(parentStyleIndex));
			}
		}
		void ExportLinkedCharacterStyle(CharacterStyle style) {
			if (style.HasLinkedStyle) {
				ParagraphStyleCollection paragraphStyles = DocumentModel.ParagraphStyles;
				int linkedStyleIndex = paragraphStyles.IndexOf(style.LinkedStyle);
				if (linkedStyleIndex >= 0)
					WriteWpStringValue("link", GetParagraphStyleId(linkedStyleIndex));
			}
		}
		protected internal virtual void ExportParagraphStyle(int styleIndex) {
			ParagraphStyle style = DocumentModel.ParagraphStyles[styleIndex];
			if (style.Deleted)
				return;
			WriteWpStartElement("style");
			try {
				WriteWpStringAttr("type", "paragraph");
				WriteWpStringAttr("styleId", GetParagraphStyleId(styleIndex));
				if (styleIndex == 0)
					WriteWpBoolAttr("default", true);
				ExportStyleName(style);
				ExportParentParagraphStyle(style);
				ExportNextParagraphStyle(style);
				ExportLinkedParagraphStyle(style);
				if (style.AutoUpdate)
					WriteWpBoolValue("autoRedefine", style.AutoUpdate);
				if (style.Hidden)
					WriteWpEmptyElement("hidden");
				if (style.Semihidden)
					WriteWpEmptyElement("semiHidden");
				WriteWpBoolValueAsTag("qFormat", style.Primary);
				ExportStyleParagraphProperties(style.ParagraphProperties, style.Tabs.GetTabs(), style.GetOwnNumberingListIndex(), style.GetOwnListLevelIndex(), style.GetNumberingListIndex());
				ExportStyleCharacterProperties(style.CharacterProperties);
			}
			finally {
				WriteWpEndElement();
			}
		}
		void ExportParentParagraphStyle(ParagraphStyle style) {
			if (style.Parent != null) {
				int parentStyleIndex = DocumentModel.ParagraphStyles.IndexOf(style.Parent);
				if (parentStyleIndex >= 0)
					WriteWpStringValue("basedOn", GetParagraphStyleId(parentStyleIndex));
			}
		}
		void ExportNextParagraphStyle(ParagraphStyle style) {
			if (style.NextParagraphStyle != null) {
				int nextStyleIndex = DocumentModel.ParagraphStyles.IndexOf(style.NextParagraphStyle);
				if (nextStyleIndex >= 0)
					WriteWpStringValue("next", GetParagraphStyleId(nextStyleIndex));
			}
		}
		void ExportLinkedParagraphStyle(ParagraphStyle style) {
			if (style.HasLinkedStyle) {
				CharacterStyleCollection characterStyles = DocumentModel.CharacterStyles;
				int linkedStyleIndex = characterStyles.IndexOf(style.LinkedStyle);
				if (linkedStyleIndex >= 0)
					WriteWpStringValue("link", GetCharacterStyleId(linkedStyleIndex));
			}
		}
		#endregion
		protected internal virtual void ExportTableStyle(int styleIndex) {
			TableStyle style = DocumentModel.TableStyles[styleIndex];
			if (style.Deleted)
				return;
			WriteWpStartElement("style");
			try {
				WriteWpStringAttr("type", "table");
				WriteWpStringAttr("styleId", GetTableStyleId(styleIndex));
				if (styleIndex == 0)
					WriteWpBoolAttr("default", true);
				ExportStyleName(style);
				ExportParentTableStyle(style);
				if (style.Hidden)
					WriteWpEmptyElement("hidden");
				if (style.Semihidden)
					WriteWpEmptyElement("semiHidden");
				WriteWpBoolValueAsTag("qFormat", style.Primary);
				if (style.ParagraphProperties.Info.FormattingOptions.Value != ParagraphFormattingOptions.Mask.UseNone)
					ExportStyleParagraphProperties(style.ParagraphProperties, style.Tabs.GetTabs(), NumberingListIndex.ListIndexNotSetted, 0, NumberingListIndex.ListIndexNotSetted);
				if (style.CharacterProperties.Info.FormattingOptions.Value != CharacterFormattingOptions.Mask.UseNone)
					ExportStyleCharacterProperties(style.CharacterProperties);
				WriteWpStartElement("tblPr");
				try {
					ExportTablePropertiesCore(style.TableProperties, false);
				}
				finally {
					WriteWpEndElement();
				}
				ExportTableRowProperties(style.TableRowProperties, true);
				ExportTableCellProperties(style.TableCellProperties, false);
				if (style.HasConditionalStyleProperties) {
					ExportTableStyleConditionalProperties(style.ConditionalStyleProperties);
				}
			}
			finally {
				WriteWpEndElement();
			}
		}
		void ExportParentTableStyle(TableStyle style) {
			if (style.Parent != null) {
				int parentStyleIndex = DocumentModel.TableStyles.IndexOf(style.Parent);
				if (parentStyleIndex >= 0)
					WriteWpStringValue("basedOn", GetTableStyleId(parentStyleIndex));
			}
		}
		protected internal virtual void ExportTableCellStyle(int styleIndex) {
			TableCellStyle style = DocumentModel.TableCellStyles[styleIndex];
			if (style.Deleted)
				return;
			WriteWpStartElement("style");
			try {
				WriteWpStringAttr("type", "tableCell");
				WriteWpStringAttr("styleId", GetTableCellStyleId(styleIndex));
				if (styleIndex == 0)
					WriteWpBoolAttr("default", true);
				ExportStyleName(style);
				ExportParentTableCellStyle(style);
				if (style.Hidden)
					WriteWpEmptyElement("hidden");
				if (style.Semihidden)
					WriteWpEmptyElement("semiHidden");
				WriteWpBoolValueAsTag("qFormat", style.Primary);
				if (style.ParagraphProperties.Info.FormattingOptions.Value != ParagraphFormattingOptions.Mask.UseNone)
					ExportStyleParagraphProperties(style.ParagraphProperties, style.Tabs.GetTabs(), NumberingListIndex.ListIndexNotSetted, 0, NumberingListIndex.ListIndexNotSetted);
				if (style.CharacterProperties.Info.FormattingOptions.Value != CharacterFormattingOptions.Mask.UseNone)
					ExportStyleCharacterProperties(style.CharacterProperties);
				ExportTableCellProperties(style.TableCellProperties);
			}
			finally {
				WriteWpEndElement();
			}
		}
		void ExportParentTableCellStyle(TableCellStyle style) {
			if (style.Parent != null) {
				int parentStyleIndex = DocumentModel.TableCellStyles.IndexOf(style.Parent);
				if (parentStyleIndex >= 0)
					WriteWpStringValue("basedOn", GetTableCellStyleId(parentStyleIndex));
			}
		}
		protected internal virtual void ExportTableStyleConditionalProperties(TableConditionalStyleProperties tableConditionalStyleProperties) {
			foreach (ConditionalTableStyleFormattingTypes styleType in TableConditionalStyleProperties.StyleTypes) {
				TableConditionalStyle conditionalStyle = tableConditionalStyleProperties[styleType];
				if (conditionalStyle != null)
					ExportTableConditionalStyle(styleType, conditionalStyle);
			}
		}
		protected internal virtual void ExportTableConditionalStyle(ConditionalTableStyleFormattingTypes styleType, TableConditionalStyle style) {
			WriteWpStartElement("tblStylePr");
			try {
				WriteWpStringAttr("type", ConvertConditionalStyleType(styleType));
				if (style.ParagraphProperties.Info.FormattingOptions.Value != ParagraphFormattingOptions.Mask.UseNone)
					ExportStyleParagraphProperties(style.ParagraphProperties, style.Tabs.GetTabs(), NumberingListIndex.ListIndexNotSetted, 0, NumberingListIndex.ListIndexNotSetted);
				if (style.CharacterProperties.Info.FormattingOptions.Value != CharacterFormattingOptions.Mask.UseNone)
					ExportStyleCharacterProperties(style.CharacterProperties);
				WriteWpStartElement("tblPr");
				try {
					ExportTablePropertiesCore(style.TableProperties);
				}
				finally {
					WriteWpEndElement();
				}
				ExportTableRowProperties(style.TableRowProperties);
				ExportTableCellProperties(style.TableCellProperties);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportStyleParagraphProperties(ParagraphProperties paragraphProperties, TabFormattingInfo tabInfo, NumberingListIndex ownNumberingListIndex, int listLevelIndex, NumberingListIndex mergedStyleListIndex) {
			WriteWpStartElement("pPr");
			try {
				Action paragraphNumberingExporter = null;
				if (ShouldExportParagraphNumbering(ownNumberingListIndex) || ShouldExportParagraphNumbering(mergedStyleListIndex))
					paragraphNumberingExporter = () => ExportParagraphStyleListReference(ownNumberingListIndex, listLevelIndex);
				Action tabsExporter = () => ExportTabProperties(tabInfo);
				ExportParagraphPropertiesCore(paragraphProperties, false, null, paragraphNumberingExporter, tabsExporter);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportStyleCharacterProperties(CharacterProperties characterProperties) {
			WriteWpStartElement("rPr");
			try {
				ExportRunPropertiesCore(characterProperties);
			}
			finally {
				WriteWpEndElement();
			}
		}
		#region Settings
		protected internal void WriteSettingsCore() {
			DocumentProperties properties = DocumentModel.DocumentProperties;
			WriteWpBoolValue("displayBackgroundShape", properties.DisplayBackgroundShape);
			ExportDocumentProtectionSettings();
			WriteWpIntValue("defaultTabStop", UnitConverter.ModelUnitsToTwips(properties.DefaultTabWidth));
			WriteWpBoolValue("autoHyphenation", properties.HyphenateDocument);
			WriteWpBoolValue("evenAndOddHeaders", properties.DifferentOddAndEvenPages);
			ExportDocumentVariablesSettings();
			WriteWpEmptyElement("clrSchemeMapping");
			ExportColorSchemeMapping();
		}
		protected internal virtual void ExportDocumentProtectionSettings() {
			DocumentProtectionProperties properties = DocumentModel.ProtectionProperties;
			if (!properties.EnforceProtection)
				return;
			WriteWpStartElement("documentProtection");
			try {
				WriteWpBoolAttr("enforcement", properties.EnforceProtection);
				if (properties.ProtectionType == DocumentProtectionType.ReadOnly) {
					WordProcessingMLValue val = new WordProcessingMLValue("readOnly", "read-only");
					WriteWpStringAttr("edit", GetWordProcessingMLValue(val));
				}
				ExportDocumentProtectionSettingsCore();
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal abstract void ExportDocumentProtectionSettingsCore();
		protected internal virtual void ExportColorSchemeMapping() {
		}
		protected internal virtual void ExportDocumentVariablesSettings() {
			DocumentVariableCollection variables = DocumentModel.Variables;
			if (variables.Count == 0 && DocumentModel.DocumentProperties.UpdateDocVariablesBeforePrint == UpdateDocVariablesBeforePrint.Auto)
				return;
			WriteWpStartElement("docVars");
			try {
				foreach (string name in variables.GetVariableNames()) {
					WriteWpStartElement("docVar");
					try {
						WriteWpStringAttr("name", name);
						object value = variables[name];
						if (value == null || value == DocVariableValue.Current)
							value = String.Empty;
						WriteWpStringAttr("val", EncodeVariableValue(value.ToString()));
					}
					finally {
						WriteWpEndElement();
					}
				}
				 UpdateDocVariablesBeforePrint updateDocVariablesBeforePrint = DocumentModel.DocumentProperties.UpdateDocVariablesBeforePrint;
				 if (updateDocVariablesBeforePrint != UpdateDocVariablesBeforePrint.Auto) {
					 WriteWpStartElement("docVar");
					 try {
						 WriteWpStringAttr("name", DocumentProperties.UpdateDocVariableFieldsBeforePrintDocVarName);
						 WriteWpStringAttr("val", EncodeVariableValue(DocumentModel.DocumentProperties.GetUpdateFieldsBeforePrintDocVarValue()));
					 }
					 finally {
						 WriteWpEndElement();
					 }
				 }
			}
			finally {
				WriteWpEndElement();
			}
		}
		#endregion
		string EncodeVariableValue(string value) {
			return XmlBasedExporterUtils.Instance.EncodeXmlChars(value);
		}
		#region Numbering
		protected internal void ExportNumberingCore() {
			WordProcessingMLValue tag = new WordProcessingMLValue("numbering", "lists");
			WriteWpStartElement(GetWordProcessingMLValue(tag));
			try {
				ExportAbstractNumberingLists();
				ExportNumberingLists();
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal abstract void ExportAbstractNumberingList(AbstractNumberingList list, int id);
		protected internal abstract void ExportNumberingList(NumberingList list, int id);
		protected internal virtual void ExportAbstractNumberingLists() {
			AbstractNumberingListCollection lists = DocumentModel.AbstractNumberingLists;
			int count = lists.Count;
			for (int i = 0; i < count; i++) {
				AbstractNumberingListIndex index = new AbstractNumberingListIndex(i);
				ExportAbstractNumberingList(lists[index], i);
			}
		}
		protected internal virtual void ExportNumberingLists() {
			NumberingListCollection lists = DocumentModel.NumberingLists;
			int count = lists.Count;
			for (int i = 0; i < count; i++)
				ExportNumberingList(lists[new NumberingListIndex(i)], i);
		}
		protected internal void ExportOverrideLevels(ListLevelCollection<IOverrideListLevel> levels) {
			int count = levels.Count;
			for (int i = 0; i < count; i++) {
				if (levels[i].OverrideStart || levels[i] is OverrideListLevel)
					ExportLevelOverride(levels[i], i);
			}
		}
		protected internal void ExportLevelOverride(IOverrideListLevel level, int levelIndex) {
			WriteWpStartElement("lvlOverride");
			try {
				WriteWpIntAttr("ilvl", levelIndex);
				if (level.OverrideStart)
					ExportStartOverride(level.NewStart);
				if (level is OverrideListLevel)
					ExportLevel(level, levelIndex);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal void ExportStartOverride(int newStart) {
			WriteWpIntValue("startOverride", newStart);
		}
		protected internal void ExportLevel(IListLevel level, int levelIndex) {
			WriteWpStartElement("lvl");
			try {
				WriteWpIntAttr("ilvl", levelIndex);
				if (level.ListLevelProperties.TemplateCode != 0)
					WriteWpStringAttr("tplc", ConvertToHexBinary(level.ListLevelProperties.TemplateCode));
				ExportLevelProperties(level, levelIndex);
				ExportLevelParagraphProperties(level.ParagraphProperties);
				ExportLevelCharacterProperties(level.CharacterProperties);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal void ExportAbstractLevelProperties(ListLevel level) {
			int paragraphStyleIndex = level.ParagraphStyleIndex;
			if (paragraphStyleIndex < 0 || paragraphStyleIndex >= DocumentModel.ParagraphStyles.Count)
				return;
			WriteWpStringValue("pStyle", GetParagraphStyleId(paragraphStyleIndex));
		}
		void ExportLevelParagraphProperties(ParagraphProperties properties) {
			WriteWpStartElement("pPr");
			try {
				ExportParagraphPropertiesCore(properties, false, null, null, null);
			}
			finally {
				WriteWpEndElement();
			}
		}
		void ExportLevelCharacterProperties(CharacterProperties properties) {
			WriteWpStartElement("rPr");
			try {
				ExportRunPropertiesCore(properties);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal void ExportLevels(ListLevelCollection<ListLevel> levels) {
			int count = levels.Count;
			for (int i = 0; i < count; i++)
				ExportLevel(levels[i], i);
		}
		protected internal virtual void ExportLevelProperties(IListLevel level, int levelIndex) {
			ListLevelProperties properties = level.ListLevelProperties;
			WriteWpIntValue("start", properties.Start);
			ExportNumberFormatValue(properties);
			if (properties.SuppressRestart)
				WriteWpIntValue("lvlRestart", 0);
			else if (properties.RelativeRestartLevel != 0)
				WriteWpIntValue("lvlRestart", levelIndex - properties.RelativeRestartLevel);
			ListLevel abstractLevel = level as ListLevel;
			if (abstractLevel != null)
				ExportAbstractLevelProperties(abstractLevel);
			if (properties.ConvertPreviousLevelNumberingToDecimal)
				WriteWpBoolValue("isLgl", properties.ConvertPreviousLevelNumberingToDecimal);
			WriteWpStringValue("suff", ConvertNumberingSeparator(properties.Separator));
			WriteWpStringValue("lvlText", ConvertFormatString(properties.DisplayFormatString));
			if (properties.Legacy) {
				WriteWpStartElement("legacy");
				try {
					WriteWpIntAttr("legacy", 1);
					WriteWpIntAttr("legacyIndent", UnitConverter.ModelUnitsToTwips(properties.LegacyIndent));
					WriteWpIntAttr("legacySpace", UnitConverter.ModelUnitsToTwips(properties.LegacySpace));
				}
				finally {
					WriteWpEndElement();
				}
			}
			WriteWpStringValue("lvlJc", ConvertListNumberAlignment(properties.Alignment));
		}
		protected internal abstract void ExportNumberFormatValue(ListLevelProperties properties);
		#endregion
		#region FloatingObjectProperties
		protected internal override void ExportFloatingObjectAnchorRun(FloatingObjectAnchorRun run) {
			WriteWpStartElement("r");
			try {
				ExportImageReference(run);
			}
			finally {
				WriteWpEndElement();
			}
		}
		#endregion
		#region Pictures
		protected abstract void ExportImageReference(InlinePictureRun run);
		protected internal override void ExportInlinePictureRun(InlinePictureRun run) {
			WriteWpStartElement("r");
			try {
				CharacterFormattingBase info = run.CharacterProperties.Info;
				if (InlinePictureRun.ShouldExportInlinePictureRunCharacterProperties(info.Info, info.Options))
					ExportRunProperties(run);
				ExportInlinePictureImageReference(run);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected virtual void ExportInlinePictureImageReference(InlinePictureRun run) {
			ExportImageReference(run);
		}
		protected internal virtual string GenerateImageId() {
			ImageCounter++;
			return "image" + ImageCounter.ToString();
		}
		protected internal virtual string GenerateImageRelationId(string imageId) {
			return "Rel" + imageId;
		}
		#endregion
		#region Converters
		protected internal abstract string ConvertBoolToString(bool value);
		protected internal virtual string ConvertColorToString(Color value) {
			return String.Format("{0:X2}{1:X2}{2:X2}", (int)value.R, (int)value.G, (int)value.B);
		}
		protected internal virtual string ConvertToHexString(int value) {
			return String.Format("{0:X}", value);
		}
		protected string ConvertToHexBinary(int number) {
			string hexBinary = number.ToString("X", CultureInfo.InvariantCulture).PadLeft(8, '0');
			return hexBinary;
		}
		protected internal virtual string ConvertBackColorToString(Color value) {
			if (DXColor.IsTransparentOrEmpty(value))
				return GetWordProcessingMLValue(predefinedBackgroundColors[DXColor.Empty]);
			WordProcessingMLValue result;
			if (predefinedBackgroundColors.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			Color bestMatchColor = DXColor.CalculateNearestColor(predefinedBackgroundColors.Keys, value);
			Color currentTableBackgroundColor = CurrentTableBackgroundColor;
			if (!DXColor.IsTransparentOrEmpty(currentTableBackgroundColor) && currentTableBackgroundColor == value && bestMatchColor != value)
				return String.Empty; 
			return GetWordProcessingMLValue(predefinedBackgroundColors[bestMatchColor]);
		}
		protected internal virtual string ConvertScript(CharacterFormattingScript script) {
			switch (script) {
				default:
				case CharacterFormattingScript.Normal:
					return "baseline";
				case CharacterFormattingScript.Subscript:
					return "subscript";
				case CharacterFormattingScript.Superscript:
					return "superscript";
			}
		}
		protected internal virtual string ConvertUnderlineType(UnderlineType underline) {
			WordProcessingMLValue value;
			if (underlineTable.TryGetValue(underline, out value))
				return GetWordProcessingMLValue(value);
			else
				return GetWordProcessingMLValue(underlineTable[UnderlineType.Single]);
		}
		protected internal string ConvertFormatString(string value) {
			return String.Format(value, "%1", "%2", "%3", "%4", "%5", "%6", "%7", "%8", "%9");
		}
		protected internal string ConvertListNumberAlignment(ListNumberAlignment value) {
			WordProcessingMLValue result;
			if (listNumberAlignmentTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(listNumberAlignmentTable[ListNumberAlignment.Left]);
		}
		protected internal string ConvertNumberingSeparator(char value) {
			WordProcessingMLValue result;
			if (listNumberSeparatorTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return "nothing";
		}
		protected internal string ConvertNumberingListType(NumberingType value) {
			WordProcessingMLValue result;
			if (numberingListTypeTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(numberingListTypeTable[NumberingType.Bullet]);
		}
		#endregion
		#region Fields
		int fieldCodeDepth = 0;
		protected internal override void ExportFieldCodeStartRun(FieldCodeStartRun run) {
			ExportFieldChar(run, "begin", true);
			fieldCodeDepth++;
		}
		protected internal override void ExportFieldCodeEndRun(FieldCodeEndRun run) {
			ExportFieldChar(run, "separate", false);
			fieldCodeDepth--;
		}
		protected internal override void ExportFieldResultEndRun(FieldResultEndRun run) {
			ExportFieldChar(run, "end", false);
		}
		protected internal virtual void ExportFieldChar(TextRun run, string fieldCharType, bool fieldStart) {
			WriteWpStartElement("r");
			try {
				ExportRunProperties(run);
				WriteWpStartElement("fldChar");
				try {
					WriteWpStringAttr("fldCharType", fieldCharType);					
					Field field = PieceTable.FindFieldByRunIndex(run.GetRunIndex());
					if (fieldStart && field.DisableUpdate)
						WriteWpBoolAttr("disableUpdate", true);
					if (fieldStart & field.Locked)
						WriteWpBoolAttr("fldLock", true);
				}
				finally {
					WriteWpEndElement();
				}
			}
			finally {
				WriteWpEndElement();
			}
		}
		#endregion
		#region RangePermissions
		int rangePermissionCounter;
		Dictionary<RangePermission, string> rangePermissionIdMap = new Dictionary<RangePermission, string>();
		protected internal override void ExportRangePermissionStart(RangePermission rangePermission) {
			WriteWpStartElement("permStart");
			try {
				string id = rangePermissionCounter.ToString();
				rangePermissionCounter++;
				rangePermissionIdMap.Add(rangePermission, id);
				WriteWpStringAttr("id", id);
				if (!String.IsNullOrEmpty(rangePermission.UserName))
					WriteWpStringAttr("ed", rangePermission.UserName);
				if (!String.IsNullOrEmpty(rangePermission.Group))
					WriteWpStringAttr("edGrp", GetGroupName(rangePermission.Group));
			}
			finally {
				WriteWpEndElement();
			}
		}
		string GetGroupName(string groupName) {
			string result;
			if (predefinedGroupNames.TryGetValue(groupName, out result))
				return result;
			return groupName;
		}
		protected internal override void ExportRangePermissionEnd(RangePermission rangePermission) {
			string id;
			if (!rangePermissionIdMap.TryGetValue(rangePermission, out id))
				return;
			rangePermissionIdMap.Remove(rangePermission);
			WriteWpStartElement("permEnd");
			try {
				WriteWpStringAttr("id", id);
			}
			finally {
				WriteWpEndElement();
			}
		}
		#endregion
		#region FootNotes
		protected internal override void ExportFootNoteRun(FootNoteRun run) {
			if (!DocumentModel.DocumentCapabilities.FootNotesAllowed)
				return;
			WriteWpStartElement("r");
			try {
				ExportRunProperties(run);
				if (PieceTable.IsFootNote)
					ExportFootNoteSelfReference(run);
				else
					ExportFootNoteReference(run);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportFootNoteReference(FootNoteRun run) {
			ExportFootNoteCore(DocumentContentWriter, run.Note, -1);
		}
		protected internal virtual void ExportFootNoteSelfReference(FootNoteRun run) {
			WriteWpStartElement("footnoteRef");
			WriteWpEndElement();
		}
		protected internal virtual void ExportFootNoteCore(XmlWriter writer, FootNote footNote, int id) {
			ExportFootNoteCore(writer, footNote, id, "footnote");
		}
		protected internal virtual void ExportFootNoteCore<T>(XmlWriter writer, FootNoteBase<T> note, int id, string tagName) where T : FootNoteBase<T> {
			XmlWriter oldWriter = this.DocumentContentWriter;
			try {
				this.DocumentContentWriter = writer;
				WriteWpStartElement(tagName);
				try {
					if (id >= 0)
						WriteWpIntAttr("id", id);
					PerformExportPieceTable(note.PieceTable, ExportPieceTable);
				}
				finally {
					WriteWpEndElement();
				}
			}
			finally {
				this.DocumentContentWriter = oldWriter;
			}
		}
		#endregion
		#region EndNotes
		protected internal override void ExportEndNoteRun(EndNoteRun run) {
			if (!DocumentModel.DocumentCapabilities.EndNotesAllowed)
				return;
			WriteWpStartElement("r");
			try {
				ExportRunProperties(run);
				if (PieceTable.IsEndNote)
					ExportEndNoteSelfReference(run);
				else
					ExportEndNoteReference(run);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportEndNoteReference(EndNoteRun run) {
			ExportEndNoteCore(DocumentContentWriter, run.Note, -1);
		}
		protected internal virtual void ExportEndNoteSelfReference(EndNoteRun run) {
			WriteWpStartElement("endnoteRef");
			WriteWpEndElement();
		}
		protected internal virtual void ExportEndNoteCore(XmlWriter writer, EndNote endNote, int id) {
			ExportFootNoteCore(writer, endNote, id, "endnote");
		}
		#endregion
		protected internal string GetParagraphStyleId(int styleIndex) {
			return "P" + styleIndex.ToString();
		}
		protected internal string GetCharacterStyleId(int styleIndex) {
			return "C" + styleIndex.ToString();
		}
		protected internal string GetTableStyleId(int styleIndex) {
			return "T" + styleIndex.ToString();
		}
		protected internal string GetTableCellStyleId(int styleIndex) {
			return "TC" + styleIndex.ToString();
		}
		protected internal string GetNumberingStyleId(int styleIndex) {
			return "N" + styleIndex.ToString();
		}
		protected internal virtual string GetWordProcessingMLValue(string openXmlValue) {
			return GetWordProcessingMLValue(new WordProcessingMLValue(openXmlValue));
		}
		protected internal abstract string GetWordProcessingMLValue(WordProcessingMLValue value);
		#region FloatingObjectProperties
		protected string ExportImageStyle(FloatingObjectProperties floatingObjectProperties, TextBoxFloatingObjectContent textBoxContent, Shape shape) {
			float finalWidth = UnitConverter.ModelUnitsToPointsF(floatingObjectProperties.ActualSize.Width);
			float finalHeight = UnitConverter.ModelUnitsToPointsF(floatingObjectProperties.ActualSize.Height);
			int zOrder = floatingObjectProperties.ZOrder;
			float topDistance = UnitConverter.ModelUnitsToPointsF(floatingObjectProperties.TopDistance);
			float bottomDistance = UnitConverter.ModelUnitsToPointsF(floatingObjectProperties.BottomDistance);
			float leftDistance = UnitConverter.ModelUnitsToPointsF(floatingObjectProperties.LeftDistance);
			float rightDistance = UnitConverter.ModelUnitsToPointsF(floatingObjectProperties.RightDistance);
			float offsetX = UnitConverter.ModelUnitsToPointsF(floatingObjectProperties.Offset.X);
			float offsetY = UnitConverter.ModelUnitsToPointsF(floatingObjectProperties.Offset.Y);
			string hPositionType = GetHorizontalPositionType(floatingObjectProperties.HorizontalPositionType);
			string vPositionType = GetVerticalPositionType(floatingObjectProperties.VerticalPositionType);
			string hPositionAlignment = GetHorizontalPositionAlignment(floatingObjectProperties.HorizontalPositionAlignment);
			string vPositionAlignment = GetVerticalPositionAlignment(floatingObjectProperties.VerticalPositionAlignment);
			string relativeWidth = floatingObjectProperties.UseRelativeWidth ? GetRelativeWidth(floatingObjectProperties.RelativeWidth) : String.Empty;
			string relativeHeight = floatingObjectProperties.UseRelativeHeight ? GetRelativeHeight(floatingObjectProperties.RelativeHeight) : String.Empty;
			string imageStyle = String.Format(CultureInfo.InvariantCulture, "position:absolute;width:{0}pt;height:{1}pt;z-index:{2};mso-wrap-distance-left:{3}pt;" +
				"mso-wrap-distance-top:{4}pt;mso-wrap-distance-right:{5}pt;mso-wrap-distance-bottom:{6}pt;margin-left:{7}pt;margin-top:{8}pt;" +
				"mso-position-horizontal:{9};mso-position-horizontal-relative:{10};mso-position-vertical:{11};mso-position-vertical-relative:{12}",
							finalWidth, finalHeight, zOrder, leftDistance, topDistance, rightDistance, bottomDistance, offsetX, offsetY, hPositionAlignment, hPositionType,
							vPositionAlignment, vPositionType);
			if (!String.IsNullOrEmpty(relativeWidth))
				imageStyle += ";" + relativeWidth;
			if (!String.IsNullOrEmpty(relativeHeight))
				imageStyle += ";" + relativeHeight;
			if (floatingObjectProperties.UsePercentOffset) {
				if (floatingObjectProperties.PercentOffsetX != 0)
					imageStyle += ";" + String.Format(CultureInfo.InvariantCulture, "mso-left-percent:{0}", floatingObjectProperties.PercentOffsetX / 100);
				if (floatingObjectProperties.PercentOffsetY != 0)
					imageStyle += ";" + String.Format(CultureInfo.InvariantCulture, "mso-top-percent:{0}", floatingObjectProperties.PercentOffsetY / 100);
			}
			if (!Object.Equals(textBoxContent, null)) {
				TextBoxProperties textBoxProperties = textBoxContent.TextBoxProperties;
				if (textBoxProperties.UseWrapText)
					imageStyle += ";mso-wrap-style:" + GetWrapText(textBoxProperties.WrapText);
				if (textBoxProperties.UseVerticalAlignment)
					imageStyle += ";v-text-anchor:" + GetVerticalAlignment(textBoxProperties.VerticalAlignment);
			}
			if (!Object.Equals(shape, null) && shape.UseRotation) {
				if (shape.Rotation % 60000 == 0)
					imageStyle += ";rotation:" + UnitConverter.ModelUnitsToDegree(shape.Rotation);
				else
					imageStyle += ";rotation:" + UnitConverter.ModelUnitsToFD(shape.Rotation) + "fd";
			}
			return imageStyle;
		}
		string GetVerticalAlignment(VerticalAlignment verticalAlignment) {
			WordProcessingMLValue vAlignment;
			if (textBoxVerticalAlignmentTable.TryGetValue(verticalAlignment, out vAlignment)) {
				if (vAlignment.WordMLValue == "just")
					return "top";
				else
					return vAlignment.WordMLValue;
			}
			else
				return "top";
		}
		string GetWrapText(bool WrapText) {
			return WrapText ? "square" : "none";
		}
		protected string GetRelativeWidth(FloatingObjectRelativeWidth relativeWidth) {
			if (relativeWidth.From != FloatingObjectRelativeFromHorizontal.Page)
				return String.Format(CultureInfo.InvariantCulture, "mso-width-relative:{0};mso-width-percent:{1}", GetRelativeSizeFromHorizontal(relativeWidth), relativeWidth.Width / 100);
			else
				return String.Format(CultureInfo.InvariantCulture, "mso-width-percent:{0}", relativeWidth.Width / 100);
		}
		protected string GetRelativeHeight(FloatingObjectRelativeHeight relativeHeight) {
			if (relativeHeight.From != FloatingObjectRelativeFromVertical.Page)
				return String.Format(CultureInfo.InvariantCulture, "mso-height-relative:{0};mso-height-percent:{1}", GetRelativeSizeFromVertical(relativeHeight), relativeHeight.Height / 100);
			else
				return String.Format(CultureInfo.InvariantCulture, "mso-height-percent:{0}", relativeHeight.Height / 100);
		}
		protected string GetRelativeSizeFromHorizontal(FloatingObjectRelativeWidth relativeWidth) {
			WordProcessingMLValue relativeFrom;
			if (floatingObjectCssRelativeFromHorizontalTable.TryGetValue(relativeWidth.From, out relativeFrom))
				return relativeFrom.WordMLValue;
			else
				return floatingObjectCssRelativeFromHorizontalTable[FloatingObjectRelativeFromHorizontal.Margin].WordMLValue;
		}
		protected string GetRelativeSizeFromVertical(FloatingObjectRelativeHeight relativeHeight) {
			WordProcessingMLValue relativeFrom;
			if (floatingObjectCssRelativeFromVerticalTable.TryGetValue(relativeHeight.From, out relativeFrom))
				return relativeFrom.WordMLValue;
			else
				return floatingObjectCssRelativeFromVerticalTable[FloatingObjectRelativeFromVertical.Margin].WordMLValue;
		}
		protected string GetVerticalPositionAlignment(FloatingObjectVerticalPositionAlignment floatingObjectVerticalPositionAlignment) {
			WordProcessingMLValue hPositionAlignment;
			if (floatingObjectVerticalPositionAlignmentTable.TryGetValue(floatingObjectVerticalPositionAlignment, out hPositionAlignment))
				return hPositionAlignment.WordMLValue;
			else
				return "absolute";
		}
		protected string GetHorizontalPositionAlignment(FloatingObjectHorizontalPositionAlignment floatingObjectHorizontalPositionAlignment) {
			WordProcessingMLValue vPositionAlignment;
			if (floatingObjectHorizontalPositionAlignmentTable.TryGetValue(floatingObjectHorizontalPositionAlignment, out vPositionAlignment)) {
				return vPositionAlignment.WordMLValue;
			}
			else
				return "absolute";
		}
		protected string GetVerticalPositionType(FloatingObjectVerticalPositionType floatingObjectVerticalPositionType) {
			WordProcessingMLValue vPositionType;
			if (VerticalPositionTypeAttributeTable.TryGetValue(floatingObjectVerticalPositionType, out vPositionType))
				return vPositionType.OpenXmlValue;
			else
				return "margin";
		}
		protected string GetHorizontalPositionType(FloatingObjectHorizontalPositionType floatingObjectHorizontalPositionType) {
			WordProcessingMLValue hPositionType;
			if (HorizontalPositionTypeAttributeTable.TryGetValue(floatingObjectHorizontalPositionType, out hPositionType))
				return hPositionType.OpenXmlValue;
			else
				return "page";
		}
		protected void ExportLockAspectRatio(FloatingObjectProperties floatingObjectProperties) {
			if (!floatingObjectProperties.UseLockAspectRatio)
				return;
			DocumentContentWriter.WriteStartElement(OPrefix, "lock", OfficeNamespaceConst);
			try {
				DocumentContentWriter.WriteAttributeString("aspectratio", ConvertBoolValueToString(floatingObjectProperties.LockAspectRatio));
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected string ConvertBoolValueToString(bool value) {
			return value ? "t" : "f";
		}
		protected void WriteWrap(FloatingObjectProperties floatingObjectProperties) {
			WordProcessingMLValue wrapType;
			string textWrapType = "";
			if (floatingObjectTextWrapTypeTable.TryGetValue(floatingObjectProperties.TextWrapType, out wrapType)) {
				textWrapType = wrapType.WordMLValue;
				if (textWrapType == "none")
					return;
			}
			DocumentContentWriter.WriteStartElement(W10MLPrefix, "wrap", W10MLNamespace);
			try {
				DocumentContentWriter.WriteAttributeString("type", textWrapType);
				switch (floatingObjectProperties.TextWrapType) {
					case FloatingObjectTextWrapType.Through:
					case FloatingObjectTextWrapType.Tight:
						DocumentContentWriter.WriteAttributeString("anchory", "line");
						break;
				}
				if (floatingObjectProperties.UseTextWrapSide) {
					string textWrapSide = "";
					WordProcessingMLValue wrapSide;
					if (floatingObjectTextWrapSideTable.TryGetValue(floatingObjectProperties.TextWrapSide, out wrapSide))
						textWrapSide = wrapSide.WordMLValue;
					if (!String.IsNullOrEmpty(textWrapSide) && textWrapSide != "both-sides")
						DocumentContentWriter.WriteAttributeString("side", textWrapSide);
				}
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected void WriteFloatingObjectTextBox(TextBoxFloatingObjectContent textBoxContent) {
			DocumentContentWriter.WriteStartElement(VMLPrefix, "textbox", VMLNamespace);
			try {
				TextBoxProperties textBoxProperties = textBoxContent.TextBoxProperties;
				if (textBoxProperties.UseResizeShapeToFitText)
					DocumentContentWriter.WriteAttributeString("style", "mso-fit-shape-to-text:" + ConvertBoolValueToString(textBoxProperties.ResizeShapeToFitText));
				if (textBoxProperties.UseLeftMargin || textBoxProperties.UseRightMargin || textBoxProperties.UseTopMargin || textBoxProperties.UseBottomMargin)
					DocumentContentWriter.WriteAttributeString("inset", (int)Math.Round(UnitConverter.ModelUnitsToMillimetersF(textBoxProperties.LeftMargin)) + "mm," +
																(int)Math.Round(UnitConverter.ModelUnitsToMillimetersF(textBoxProperties.TopMargin)) + "mm," +
																(int)Math.Round(UnitConverter.ModelUnitsToMillimetersF(textBoxProperties.RightMargin)) + "mm," +
																(int)Math.Round(UnitConverter.ModelUnitsToMillimetersF(textBoxProperties.BottomMargin)) + "mm");
				WriteFloatingObjectTxbxContent(textBoxContent);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected void WriteFloatingObjectTxbxContent(TextBoxFloatingObjectContent content) {
			DocumentContentWriter.WriteStartElement(WordProcessingPrefix, "txbxContent", WordProcessingNamespace);
			try {
				PerformExportPieceTable(content.TextBox.PieceTable, ExportPieceTable);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected void WriteFloatingObjectAnchorLock(bool locked) {
			if (locked) {
				DocumentContentWriter.WriteStartElement(W10MLPrefix, "anchorlock", W10MLNamespace);
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected string GenerateFloatingObjectName(string name, string defaultNamePrefix, int id) {
			if (!String.IsNullOrEmpty(name))
				return name;
			else
				return defaultNamePrefix + " " + id.ToString();
		}
		protected void WriteFloatingObjectPict(FloatingObjectProperties floatingObjectProperties, TextBoxFloatingObjectContent textBoxContent, PictureFloatingObjectContent pictureContent, Shape shape, string name) {
			DocumentContentWriter.WriteStartElement(WordProcessingPrefix, "pict", WordProcessingNamespace);
			try {
				int shapeTypeId = WriteFloatingObjectShapeType();
				WriteFloatingObjectShape(floatingObjectProperties, textBoxContent, pictureContent, shape, shapeTypeId, name);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		int WriteFloatingObjectShapeType() {
			DocumentContentWriter.WriteStartElement(VMLPrefix, "shapetype", VMLNamespace);
			int id = drawingElementId;
			try {
				WriteIntValue("id", id);
				DocumentContentWriter.WriteAttributeString("path", "m,l,21600r21600,l21600,xe");
				IncrementDrawingElementId();
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
			return id;
		}
		protected void IncrementDrawingElementId() {
			drawingElementId++;
		}
		void WriteFloatingObjectShape(FloatingObjectProperties floatingObjectProperties, TextBoxFloatingObjectContent textBoxContent, PictureFloatingObjectContent pictureContent, Shape shape, int shapeTypeId, string name) {
			string imagePath = "";
			if (pictureContent != null)
				imagePath = ExportBinData(pictureContent.Image);
			DocumentContentWriter.WriteStartElement(VMLPrefix, "shape", VMLNamespace);
			try {
				string imageStyle = ExportImageStyle(floatingObjectProperties, textBoxContent, shape);
				DocumentContentWriter.WriteAttributeString("type", "#" + shapeTypeId);
				DocumentContentWriter.WriteAttributeString("id", name);
				DocumentContentWriter.WriteAttributeString("style", imageStyle);
				if (textBoxContent != null)
					WriteFloatingObjectShapeAllColorsAndOutlineWeight(shape);
				if (floatingObjectProperties.UseLayoutInTableCell)
					WriteStringAttr(OPrefix, "allowincell", OfficeNamespaceConst, GetBoolValueAsString(floatingObjectProperties.LayoutInTableCell));
				if (floatingObjectProperties.UseAllowOverlap)
					WriteStringAttr(OPrefix, "allowoverlap", OfficeNamespaceConst, GetBoolValueAsString(floatingObjectProperties.AllowOverlap));
				if (pictureContent != null)
					ExportImageData(imagePath);
				else if (textBoxContent != null)
					WriteFloatingObjectTextBox(textBoxContent);
				WriteFloatingObjectAnchorLock(floatingObjectProperties.Locked);
				ExportLockAspectRatio(floatingObjectProperties);
				WriteWrap(floatingObjectProperties);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		void WriteFloatingObjectShapeAllColorsAndOutlineWeight(Shape shape) {
			if (shape.UseFillColor)
				DocumentContentWriter.WriteAttributeString("fillcolor", "#" + ConvertColorToString(shape.FillColor));
			if (shape.UseOutlineColor)
				DocumentContentWriter.WriteAttributeString("strokecolor", "#" + ConvertColorToString(shape.OutlineColor));
			if (shape.UseOutlineWidth)
				DocumentContentWriter.WriteAttributeString("strokeweight", (int)UnitConverter.ModelUnitsToPointsF(shape.OutlineWidth) + "pt");
			if (DXColor.IsTransparentOrEmpty(shape.OutlineColor))
				DocumentContentWriter.WriteAttributeString("stroked", "f");
		}
		protected string GetBoolValueAsString(bool value) {
			return value ? "t" : "f";
		}
		protected internal virtual void ExportImageData(string imagePath) {
			DocumentContentWriter.WriteStartElement(VMLPrefix, "imagedata", VMLNamespace);
			try {
				DocumentContentWriter.WriteAttributeString("src", imagePath);
				DocumentContentWriter.WriteAttributeString(OfficePrefix, "title", OfficeNamespace, String.Empty);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual string ExportBinData(OfficeImage image) {
			string imagePath;
			OfficeImage rootImage = image.RootImage;
			if (ExportedImageTable.TryGetValue(rootImage, out imagePath))
				return imagePath;
			imagePath = GetImagePath(image);
			WriteWpStartElement("binData");
			try {
				WriteWpStringAttr("name", imagePath);
				Stream imageStream = GetImageBytesStream(image);
				byte[] buffer = new byte[8192];
				for (; ; ) {
					int bytesRead = imageStream.Read(buffer, 0, buffer.Length);
					DocumentContentWriter.WriteBase64(buffer, 0, bytesRead);
					if (bytesRead < buffer.Length)
						break;
				}
			}
			finally {
				WriteWpEndElement();
			}
			ExportedImageTable.Add(rootImage, imagePath);
			return imagePath;
		}
		protected internal virtual string GetImagePath(OfficeImage image) {
			string extenstion = GetImageExtension(image);
			return "wordml://" + GenerateImageId() + "." + extenstion;
		}
		#endregion
	}
	#region WordProcessingMLValue
	public class WordProcessingMLValue {
		readonly string openXmlValue;
		readonly string wordMLValue;
		public WordProcessingMLValue(string openXmlValue) {
			this.openXmlValue = openXmlValue;
			this.wordMLValue = ConvertToWordML(openXmlValue);
		}
		public WordProcessingMLValue(string openXmlValue, string wordMLValue) {
			this.openXmlValue = openXmlValue;
			this.wordMLValue = wordMLValue;
		}
		string ConvertToWordML(string openXmlValue) {
			int count = openXmlValue.Length;
			StringBuilder result = new StringBuilder(count);
			for (int i = 0; i < count; i++) {
				char currentChar = openXmlValue[i];
				if (Char.IsUpper(currentChar)) {
					result.Append('-');
					result.Append(Char.ToLowerInvariant(currentChar));
				}
				else
					result.Append(currentChar);
			}
			return result.ToString();
		}
		public string OpenXmlValue { get { return openXmlValue; } }
		public string WordMLValue { get { return wordMLValue; } }
	}
	#endregion
}
