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
using System.Xml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Import.Xaml {
	#region ParagraphDestination
	public class ParagraphDestination : BlockElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = CreateBlockElementHandlerTable();
			return result;
		}
		public ParagraphDestination(XamlImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			ApplyTextDecorations(reader);
			ImportInputPosition position = Importer.Position;
			string indent = Importer.ReadAttribute(reader, "TextIndent");
			if (!String.IsNullOrEmpty(indent)) {
				int value = Importer.ParseMetricIntegerToModelUnits(indent);
				if (value >= 0) {
					position.ParagraphFormatting.FirstLineIndentType = ParagraphFirstLineIndent.Indented;
					position.ParagraphFormatting.FirstLineIndent = value;
				}
				else {
					position.ParagraphFormatting.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
					position.ParagraphFormatting.FirstLineIndent = -value;
				}
			}
		}
		public override void ProcessElementClose(XmlReader reader) {
			ImportInputPosition position = Importer.Position;
			position.CharacterFormatting.CopyFrom(position.ParagraphMarkCharacterFormatting);
			position.ParagraphMarkCharacterStyleIndex = position.CharacterStyleIndex;
			if (SuppressInsertParagraph())
				PieceTable.InsertTextCore(Importer.Position, " ");
			else
				InsertParagraph();
			base.ProcessElementClose(reader);
		}
		public override bool ProcessText(XmlReader reader) {
			RunDestination runDestination = new RunDestination(Importer);
			runDestination.ProcessText(reader);
			runDestination.ProcessElementClose(reader);
			return true;
		}
		protected internal virtual bool SuppressInsertParagraph() {
			return !DocumentModel.DocumentCapabilities.ParagraphsAllowed;
		}
		protected internal virtual Paragraph InsertParagraph() {
			ParagraphIndex paragraphIndex = Importer.Position.ParagraphIndex;
			PieceTable.InsertParagraphCore(Importer.Position);
			ApplyParagraphProperties(paragraphIndex);
			return PieceTable.Paragraphs[paragraphIndex];
		}
		protected internal virtual void ApplyParagraphProperties(ParagraphIndex paragraphIndex) {
			Paragraph paragraph = PieceTable.Paragraphs[paragraphIndex];
			paragraph.ParagraphStyleIndex = Importer.Position.ParagraphStyleIndex;
			paragraph.ParagraphProperties.CopyFrom(Importer.Position.ParagraphFormatting);
			paragraph.SetOwnTabs(Importer.Position.ParagraphTabs);
		}
	}
	#endregion
}
