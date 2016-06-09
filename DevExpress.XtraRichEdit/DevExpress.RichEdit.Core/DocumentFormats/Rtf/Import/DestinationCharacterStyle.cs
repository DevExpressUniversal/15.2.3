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
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region DestinationCharacterStyle
	public class DestinationCharacterStyle : DestinationPieceTable {
		KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("sbasedon", OnParentStyleIndex);
			table.Add("slink", OnStyleLinkKeyword);
			table.Add("sqformat", OnStyleQFormatKeyword);
			AddCharacterPropertiesKeywords(table);
			return table;
		}
		static void OnParentStyleIndex(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.RtfFormattingInfo.ParentStyleIndex = parameterValue;
		}
		static void OnStyleLinkKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.StyleLink = parameterValue;
		}
		static void OnStyleQFormatKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			((DestinationCharacterStyle)importer.Destination).QFormat = true;
			importer.Position.RtfFormattingInfo.ParentStyleIndex = parameterValue;
		}
		string styleName;
		public DestinationCharacterStyle(RtfImporter importer, int styleIndex)
			: base(importer, importer.PieceTable) {
			importer.Position.CharacterStyleIndex = styleIndex;
			this.styleName = String.Empty;
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected internal override bool CanAppendText { get { return false; } }
		bool QFormat { get; set; }
		public override void BeforePopRtfState() {
			string name = StyleSheetDestination.GetPrimaryStyleName(styleName);
			if (!Importer.CharacterStyleCollectionIndex.ContainsKey(Importer.Position.CharacterStyleIndex)) {
				CharacterStyle style = GetCharacterStyleByName(name);
				style.Primary = QFormat;
				if (Importer.Position.ParagraphFormattingInfo.StyleLink >= 0 && !Importer.LinkParagraphStyleIndexToCharacterStyleIndex.ContainsKey(Importer.Position.ParagraphFormattingInfo.StyleLink))
					Importer.LinkParagraphStyleIndexToCharacterStyleIndex.Add(Importer.Position.ParagraphFormattingInfo.StyleLink, Importer.Position.CharacterStyleIndex);
				if (name != CharacterStyleCollection.DefaultCharacterStyleName) {
					MergedCharacterProperties parentCharacterProperties = Importer.GetStyleMergedCharacterProperties(Importer.Position.RtfFormattingInfo.ParentStyleIndex);
					Importer.ApplyCharacterProperties(style.CharacterProperties, Importer.Position.CharacterFormatting.Info, parentCharacterProperties);
				}
				else if (DocumentModel.ShouldApplyAppearanceProperties) {
					style.CharacterProperties.BeginUpdate();
					try {
						if (Importer.Position.CharacterFormatting.FontName != style.CharacterProperties.FontName)
							style.CharacterProperties.FontName = Importer.Position.CharacterFormatting.FontName;
						if (Importer.Position.CharacterFormatting.DoubleFontSize != style.CharacterProperties.DoubleFontSize)
							style.CharacterProperties.DoubleFontSize = Importer.Position.CharacterFormatting.DoubleFontSize;
						if (Importer.Position.CharacterFormatting.ForeColor != style.CharacterProperties.ForeColor)
							style.CharacterProperties.ForeColor = Importer.Position.CharacterFormatting.ForeColor;
						else
							style.CharacterProperties.ForeColor = Importer.DocumentProperties.Colors.GetRtfColorById(0);
					}
					finally {
						style.CharacterProperties.EndUpdate();
					}
				}
				if (Importer.CharacterStyleCollectionIndex.ContainsKey(Importer.Position.RtfFormattingInfo.ParentStyleIndex))
					style.Parent = Importer.DocumentModel.CharacterStyles[Importer.CharacterStyleCollectionIndex[Importer.Position.RtfFormattingInfo.ParentStyleIndex]];
			}
		}
		protected virtual CharacterStyle GetCharacterStyleByName(string styleName) {
			DocumentModel documentModel = Importer.DocumentModel;
			CharacterStyleCollection characterStyles = documentModel.CharacterStyles;
			int styleIndex = characterStyles.GetStyleIndexByName(styleName);
			if (styleIndex >= 0) {
				Importer.CharacterStyleCollectionIndex[Importer.Position.CharacterStyleIndex] = styleIndex;
				return characterStyles[styleIndex];
			}
			CharacterStyle result = new CharacterStyle(documentModel);
			result.StyleName = styleName;
			int index = characterStyles.Add(result);
			Importer.CharacterStyleCollectionIndex.Add(Importer.Position.CharacterStyleIndex, index);
			return result;
		}
		protected override DestinationBase CreateClone() {
			return new DestinationCharacterStyle(Importer, Importer.Position.CharacterStyleIndex);
		}
		protected override void ProcessCharCore(char ch) {
			if (ch != ';')
				styleName += ch;
		}
		public override void FinalizePieceTableCreation() {
		}
	}
	#endregion
}
