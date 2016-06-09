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
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfCharacterPropertiesExporter
	public class RtfCharacterPropertiesExporter : RtfPropertiesExporter {
		#region Fields
		const int DefaultRtfFontSize = 24;
		static Dictionary<UnderlineType, string> defaultUnderlineTypes;
		private RtfDocumentExporterOptions options;
		#endregion
		#region PopulateUnderlineTypesTable
		static void PopulateUnderlineTypesTable() {
			defaultUnderlineTypes = new Dictionary<UnderlineType, string>();
			defaultUnderlineTypes.Add(UnderlineType.None, String.Empty);
			defaultUnderlineTypes.Add(UnderlineType.Single, RtfExportSR.FontUnderline);
			defaultUnderlineTypes.Add(UnderlineType.Dotted, RtfExportSR.FontUnderlineDotted);
			defaultUnderlineTypes.Add(UnderlineType.Dashed, RtfExportSR.FontUnderlineDashed);
			defaultUnderlineTypes.Add(UnderlineType.DashDotted, RtfExportSR.FontUnderlineDashDotted);
			defaultUnderlineTypes.Add(UnderlineType.DashDotDotted, RtfExportSR.FontUnderlineDashDotDotted);
			defaultUnderlineTypes.Add(UnderlineType.Double, RtfExportSR.FontUnderlineDouble);
			defaultUnderlineTypes.Add(UnderlineType.HeavyWave, RtfExportSR.FontUnderlineHeavyWave);
			defaultUnderlineTypes.Add(UnderlineType.LongDashed, RtfExportSR.FontUnderlineLongDashed);
			defaultUnderlineTypes.Add(UnderlineType.ThickSingle, RtfExportSR.FontUnderlineThickSingle);
			defaultUnderlineTypes.Add(UnderlineType.ThickDotted, RtfExportSR.FontUnderlineThickDotted);
			defaultUnderlineTypes.Add(UnderlineType.ThickDashed, RtfExportSR.FontUnderlineThickDashed);
			defaultUnderlineTypes.Add(UnderlineType.ThickDashDotted, RtfExportSR.FontUnderlineThickDashDotted);
			defaultUnderlineTypes.Add(UnderlineType.ThickDashDotDotted, RtfExportSR.FontUnderlineThickDashDotDotted);
			defaultUnderlineTypes.Add(UnderlineType.ThickLongDashed, RtfExportSR.FontUnderlineThickLongDashed);
			defaultUnderlineTypes.Add(UnderlineType.DoubleWave, RtfExportSR.FontUnderlineDoubleWave);
			defaultUnderlineTypes.Add(UnderlineType.Wave, RtfExportSR.FontUnderlineWave);
		}
		#endregion
		static RtfCharacterPropertiesExporter() {
			PopulateUnderlineTypesTable();
		}
		public RtfCharacterPropertiesExporter(DocumentModel documentModel, IRtfExportHelper rtfExportHelper, RtfBuilder rtfBuilder, RtfDocumentExporterOptions options) 
			: base(documentModel, rtfExportHelper, rtfBuilder){
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
		}
		public void ExportCharacterProperties(MergedCharacterProperties characterProperties) {
			ExportCharacterProperties(characterProperties, false, false, false);
		}
		public void ExportCharacterProperties(MergedCharacterProperties characterProperties, bool checkDefaultColor, bool checkUseFontSize, bool checkUseFontName) {
			ExportCharacterPropertiesCore(characterProperties, checkUseFontSize, checkUseFontName);
			CharacterFormattingInfo info = characterProperties.Info;
			WriteForegroundColor(info.ForeColor, checkDefaultColor);
			Color backColor = info.BackColor;
			if (backColor != DXColor.Empty && backColor != DXColor.Transparent)
				WriteBackgroundColor(info.BackColor);
			Color underlineColor = info.UnderlineColor;
			if (underlineColor != DXColor.Empty && underlineColor != DXColor.Transparent)
				WriteUnderlineColor(underlineColor);
		}		
		public void ExportCharacterPropertiesCore(MergedCharacterProperties characterProperties, bool checkUseFontSize, bool checkUseFontName) {
			CharacterFormattingInfo info = characterProperties.Info;
#if !SL
			if (info.LangInfo.Latin != null) {
				if (info.NoProof) {
					RtfBuilder.WriteCommand(RtfExportSR.LangInfo2, info.LangInfo.Latin.GetLCID());
					RtfBuilder.WriteCommand(RtfExportSR.LangInfo3, info.LangInfo.Latin.GetLCID());
				}
				else {
					RtfBuilder.WriteCommand(RtfExportSR.LangInfo, info.LangInfo.Latin.GetLCID());
					RtfBuilder.WriteCommand(RtfExportSR.LangInfo1, info.LangInfo.Latin.GetLCID());
				}
			}
			else {
				if (info.LangInfo.Bidi != null) {
					if (info.NoProof) {
						RtfBuilder.WriteCommand(RtfExportSR.LangInfo2, info.LangInfo.Bidi.GetLCID());
						RtfBuilder.WriteCommand(RtfExportSR.LangInfo3, info.LangInfo.Bidi.GetLCID());
					}
					else {
						RtfBuilder.WriteCommand(RtfExportSR.LangInfo, info.LangInfo.Bidi.GetLCID());
						RtfBuilder.WriteCommand(RtfExportSR.LangInfo1, info.LangInfo.Bidi.GetLCID());
					}
				}
				else
					if (info.LangInfo.EastAsia != null) {
						if (info.NoProof) {
							RtfBuilder.WriteCommand(RtfExportSR.LangInfo2, info.LangInfo.EastAsia.GetLCID());
							RtfBuilder.WriteCommand(RtfExportSR.LangInfo3, info.LangInfo.EastAsia.GetLCID());
						}
						else {
							RtfBuilder.WriteCommand(RtfExportSR.LangInfo, info.LangInfo.EastAsia.GetLCID());
							RtfBuilder.WriteCommand(RtfExportSR.LangInfo1, info.LangInfo.EastAsia.GetLCID());
						}
					}
			}
			if (info.NoProof)
				RtfBuilder.WriteCommand(RtfExportSR.NoProof);
#endif
			if (info.AllCaps)
				RtfBuilder.WriteCommand(RtfExportSR.AllCapitals);
			if (info.Hidden)
				RtfBuilder.WriteCommand(RtfExportSR.HiddenText);
			if (info.FontBold)
				RtfBuilder.WriteCommand(RtfExportSR.FontBold);
			if (info.FontItalic)
				RtfBuilder.WriteCommand(RtfExportSR.FontItalic);
			if (info.FontStrikeoutType != StrikeoutType.None)
				RtfBuilder.WriteCommand(info.FontStrikeoutType == StrikeoutType.Single ? RtfExportSR.FontStrikeout : RtfExportSR.FontDoubleStrikeout);
			if (info.Script != CharacterFormattingScript.Normal)
				RtfBuilder.WriteCommand(info.Script == CharacterFormattingScript.Subscript ? RtfExportSR.RunSubScript : RtfExportSR.RunSuperScript);
			if (info.UnderlineWordsOnly && info.FontUnderlineType == UnderlineType.Single) {
				RtfBuilder.WriteCommand(RtfExportSR.FontUnderlineWordsOnly);
			}
			else {
				if (info.FontUnderlineType != UnderlineType.None)
					WriteFontUnderline(info.FontUnderlineType);
			}
			if (!checkUseFontName || characterProperties.Options.UseFontName) {
				int fontNameIndex = WriteFontName(info.FontName);
				if (fontNameIndex >= 0)
					RegisterFontCharset(info, fontNameIndex);
			}
			if(!checkUseFontSize || characterProperties.Options.UseDoubleFontSize)
				WriteFontSize(info.DoubleFontSize);
		}
		public void ExportParagraphCharacterProperties(MergedCharacterProperties characterProperties) {
			ExportCharacterPropertiesCore(characterProperties, false, false);
			Color foreColor = characterProperties.Info.ForeColor;
			if (foreColor != DXColor.Empty)
				WriteForegroundColorCore(foreColor);
		}
		protected internal void RegisterFontCharset(CharacterFormattingInfo info, int fontNameIndex) {
			int charset;
			if (!RtfExportHelper.FontCharsetTable.TryGetValue(fontNameIndex, out charset)) {
				int fontInfoIndex = DocumentModel.FontCache.CalcFontIndex(info.FontName, info.DoubleFontSize, info.FontBold, info.FontItalic, info.Script, false, false);
				FontInfo fontInfo = DocumentModel.FontCache[fontInfoIndex];
				RtfExportHelper.FontCharsetTable.Add(fontNameIndex, fontInfo.Charset);
			}
		}
		protected internal void WriteFontUnderline(UnderlineType underlineType) {
			if (underlineType == UnderlineType.None)
				return;
			string keyword;
			if (!defaultUnderlineTypes.TryGetValue(underlineType, out keyword) || keyword == null || keyword.Length == 0)
				keyword = defaultUnderlineTypes[UnderlineType.Single];
			RtfBuilder.WriteCommand(keyword);
		}		
		protected internal int WriteFontName(string fontName) {
			int fontNameIndex = RtfExportHelper.GetFontNameIndex(fontName);
			if (fontNameIndex == RtfExportHelper.DefaultFontIndex)
				return -1;
			RtfBuilder.WriteCommand(RtfExportSR.FontNumber, fontNameIndex);
			return fontNameIndex;
		}
		protected internal void WriteFontSize(int rtfFontSize) {
			if (rtfFontSize == DefaultRtfFontSize)
				return;
			RtfBuilder.WriteCommand(RtfExportSR.FontSize, rtfFontSize);
		}
		protected internal void WriteBackgroundColor(Color backColor) {
			if (backColor != DXColor.Empty)
				backColor = RtfExportHelper.BlendColor(backColor);
			int colorIndex = RtfExportHelper.GetColorIndex(backColor);
			RtfRunBackColorExportMode mode = options.Compatibility.BackColorExportMode;
			if (mode == RtfRunBackColorExportMode.Chcbpat)
				RtfBuilder.WriteCommand(RtfExportSR.RunBackgroundColor, colorIndex);
			else if (mode == RtfRunBackColorExportMode.Highlight)
				RtfBuilder.WriteCommand(RtfExportSR.RunBackgroundColor2, colorIndex);
			else {
				RtfBuilder.WriteCommand(RtfExportSR.RunBackgroundColor2, colorIndex);
				RtfBuilder.WriteCommand(RtfExportSR.RunBackgroundColor, colorIndex);
			}
		}
		protected internal void WriteForegroundColor(Color foreColor) {
			WriteForegroundColor(foreColor, false);
		}
		protected internal void WriteForegroundColor(Color foreColor, bool checkDefaultColor) {
			if (foreColor == DXColor.Transparent)
				foreColor = DXColor.Empty;
			WriteForegroundColorCore(foreColor, checkDefaultColor);
		}
		protected internal void WriteForegroundColorCore(Color foreColor) {
			WriteForegroundColorCore(foreColor, false);
		}
		protected internal void WriteForegroundColorCore(Color foreColor, bool checkDefaultColor) {
			if (foreColor != DXColor.Empty)
				foreColor = RtfExportHelper.BlendColor(foreColor);
			int colorIndex = RtfExportHelper.GetColorIndex(foreColor);
			if(!checkDefaultColor || colorIndex != 0)
				RtfBuilder.WriteCommand(RtfExportSR.RunForegroundColor, colorIndex);
		}
		protected internal void WriteUnderlineColor(Color underlineColor) {
			underlineColor = RtfExportHelper.BlendColor(underlineColor);
			int colorIndex = RtfExportHelper.GetColorIndex(underlineColor);
			RtfBuilder.WriteCommand(RtfExportSR.RunUnderlineColor, colorIndex);
		}
	}
	#endregion
}
