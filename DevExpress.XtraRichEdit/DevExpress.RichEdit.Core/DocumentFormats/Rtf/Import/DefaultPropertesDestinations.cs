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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
#if !SL
using System.Drawing;
using System.Drawing.Printing;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region DefaultCharacterPropertiesDestination
	public class DefaultCharacterPropertiesDestination : DestinationPieceTable {
		KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();			
			AddCharacterPropertiesKeywords(table);			
			return table;
		}
		public DefaultCharacterPropertiesDestination(RtfImporter importer)
			: base(importer, importer.PieceTable) {			
		}
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected internal override bool CanAppendText { get { return false; } }
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		public override void BeforePopRtfState() {
			CharacterFormattingBase formattingInfo = Importer.Position.CharacterFormatting;
			CharacterFormattingBase info = DocumentModel.DefaultCharacterProperties.Info;
			Importer.ApplyCharacterProperties(DocumentModel.DefaultCharacterProperties, formattingInfo.Info, new MergedCharacterProperties(info.Info, info.Options), false);
		}
		protected override DestinationBase CreateClone() {
			DefaultCharacterPropertiesDestination result = new DefaultCharacterPropertiesDestination(Importer);			
			return result;
		}
		protected override void ProcessCharCore(char ch) {			
		}
		public override void FinalizePieceTableCreation() {
		}
	}
	#endregion
	#region DefaultParagraphPropertiesDestination
	public class DefaultParagraphPropertiesDestination : DestinationPieceTable {
		KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			AddParagraphPropertiesKeywords(table);
			return table;
		}
		public DefaultParagraphPropertiesDestination(RtfImporter importer)
			: base(importer, importer.PieceTable) {
		}
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected internal override bool CanAppendText { get { return false; } }
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		public override void BeforePopRtfState() {
			RtfParagraphFormattingInfo formattingInfo = Importer.Position.ParagraphFormattingInfo;
			ParagraphFormattingBase info = DocumentModel.DefaultParagraphProperties.Info;
			Importer.ApplyLineSpacing(Importer.Position.ParagraphFormattingInfo);
			Importer.ApplyParagraphProperties(DocumentModel.DefaultParagraphProperties, formattingInfo, new MergedParagraphProperties(info.Info, info.Options), false);
		}
		protected override DestinationBase CreateClone() {
			DefaultParagraphPropertiesDestination result = new DefaultParagraphPropertiesDestination(Importer);
			return result;
		}
		protected override void ProcessCharCore(char ch) {
		}
		public override void FinalizePieceTableCreation() {
		}
	}
	#endregion
}
