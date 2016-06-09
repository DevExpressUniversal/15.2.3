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
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using System.Globalization;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region RunDestination
	public class RunDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			FillElementHandlerTable(result);
			return result;
		}
		protected static void FillElementHandlerTable(ElementHandlerTable handlerTable) {
			handlerTable.Add("r", OnRun);
			handlerTable.Add("rPr", OnRunProperties);
			handlerTable.Add("t", OnText);
			handlerTable.Add("instrText", OnText); 
			handlerTable.Add("cr", OnCarriageReturn);
			handlerTable.Add("br", OnBreak);
			handlerTable.Add("tab", OnTab);
			handlerTable.Add("pict", OnPicture);
			handlerTable.Add("object", OnObject);
			handlerTable.Add("fldChar", OnComplexFieldMarker);
			handlerTable.Add("drawing", OnDrawing);
			handlerTable.Add("footnoteReference", OnFootNoteReference);
			handlerTable.Add("endnoteReference", OnEndNoteReference);
			handlerTable.Add("footnoteRef", OnFootNoteSelfReference);
			handlerTable.Add("endnoteRef", OnEndNoteSelfReference);
			handlerTable.Add("commentReference", OnCommentReference);
			handlerTable.Add("spr", OnSeparator);
			handlerTable.Add("customObject", OnCustomRun);
			handlerTable.Add("dataContainer", OnDataContainerRun);
			handlerTable.Add("sym", OnSymbol);
		}
		public RunDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
			importer.ResetPositionCharacterProperties();
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override bool IsChoiceNamespaceSupported(string requeriesNamespaceUri) {
			if (String.Compare(requeriesNamespaceUri, OpenXmlExporter.WpsNamespace, StringComparison.OrdinalIgnoreCase) == 0)
				return true;
			if (String.Compare(requeriesNamespaceUri, OpenXmlExporter.DrawingMLPicturePrefix, StringComparison.OrdinalIgnoreCase) == 0)
				return true;
			return base.IsChoiceNamespaceSupported(requeriesNamespaceUri);
		}
		protected static Destination OnRunProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			CharacterFormattingBase characterFormatting = importer.Position.CharacterFormatting;
			characterFormatting.ReplaceInfo(importer.DocumentModel.Cache.CharacterFormattingInfoCache.DefaultItem, new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseNone));
			return new RunPropertiesDestination(importer, characterFormatting);
		}
		static Destination OnRun(WordProcessingMLBaseImporter importer, XmlReader reader) {						
			return importer.CreateRunDestination();
		}
		protected static Destination OnText(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TextDestination(importer);
		}
		protected static Destination OnCarriageReturn(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CarriageReturnDestination(importer);
		}
		protected static Destination OnBreak(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new RunBreakDestination(importer);
		}
		protected static Destination OnTab(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new RunTabDestination(importer);
		}
		static Destination OnPicture(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new InlinePictureDestination(importer);
		}
		protected static Destination OnDrawing(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingDestination(importer);
		}
		static Destination OnFootNoteReference(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FootNoteReferenceDestination(importer);
		}
		static Destination OnEndNoteReference(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new EndNoteReferenceDestination(importer);
		}
		protected static Destination OnObject(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new InlineObjectDestination(importer);
		}
		protected static Destination OnComplexFieldMarker(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldCharDestination(importer);
		}
		protected static Destination OnFootNoteSelfReference(WordProcessingMLBaseImporter importer, XmlReader reader) {
			Destination[] destinations = importer.DestinationStack.ToArray();
			int count = destinations.Length;
			for (int i = 0; i < count; i++) {
				FootNoteDestination destination = destinations[i] as FootNoteDestination;
				if (destination != null)
					return destination.OnFootNoteReference(reader);
			}
			return null;
		}
		protected static Destination OnEndNoteSelfReference(WordProcessingMLBaseImporter importer, XmlReader reader) {
			Destination[] destinations = importer.DestinationStack.ToArray();
			int count = destinations.Length;
			for (int i = 0; i < count; i++) {
				EndNoteDestination destination = destinations[i] as EndNoteDestination;
				if (destination != null)
					return destination.OnEndNoteReference(reader);
			}
			return null;
		}		
		protected static Destination OnCommentReference(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CommentReferenceElementDestination(importer);
		}
		protected static Destination OnSeparator(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SeparatorDestination(importer);
		}
		protected static Destination OnCustomRun(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateCustomRunDestination(reader);
		}
		protected static Destination OnDataContainerRun(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateDataContainerRunDestination(reader);
		}
		protected static Destination OnSymbol(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SymbolDestination(importer);
		}
	}
	#endregion
	#region CarriageReturnDestination
	public class CarriageReturnDestination : LeafElementDestination {
		public CarriageReturnDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			switch (Importer.InnerOptions.LineBreakSubstitute) {
				case LineBreakSubstitute.Space:
					Importer.PieceTable.InsertTextCore(Importer.Position, new String(Characters.Space, 1));
					break;
				case LineBreakSubstitute.Paragraph:
					Importer.PieceTable.InsertParagraphCore(Importer.Position);
					break;
				default:
			Importer.PieceTable.InsertTextCore(Importer.Position, new String(Characters.LineBreak, 1));
					break;
			}			
		}
	}
	#endregion
	#region RunBreakDestination
	public class RunBreakDestination : LeafElementDestination {
		public RunBreakDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			char breakCharacter = Importer.GetWpEnumValue(reader, "type", OpenXmlExporter.runBreaksTable, Characters.LineBreak);
			Importer.PieceTable.InsertTextCore(Importer.Position, new String(breakCharacter, 1));
		}
	}
	#endregion
	#region RunTabDestination
	public class RunTabDestination : LeafElementDestination {
		public RunTabDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			char ch;
			if (Importer.DocumentModel.DocumentCapabilities.TabSymbolAllowed)
				ch = Characters.TabMark;
			else
				ch = ' ';
			Importer.PieceTable.InsertTextCore(Importer.Position, new String(ch, 1));
		}
	}
	#endregion
	#region SymbolDestination
	public class SymbolDestination : ElementDestination {
		public SymbolDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return ElementHandlerTable.Empty; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string fontName = reader.GetAttribute("font", Importer.WordProcessingNamespaceConst);
			string code = reader.GetAttribute("char", Importer.WordProcessingNamespaceConst);
			if (String.IsNullOrEmpty(fontName) || String.IsNullOrEmpty(code))
				return;
			int charCode;
			if (!Int32.TryParse(code, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out charCode))
				return;
			if (charCode < Char.MinValue || charCode > Char.MaxValue)
				return;
			char symbol = (char)charCode;
			CharacterFormattingBase characterFormatting = Importer.Position.CharacterFormatting;
			string oldFontName = characterFormatting.Options.UseFontName ? characterFormatting.FontName : null;
			characterFormatting.FontName = fontName;
			try {
				PieceTable.InsertTextCore(Importer.Position, new string(symbol, 1));
			}
			finally {
				characterFormatting.ResetUse(CharacterFormattingOptions.Mask.UseFontName);
				if (oldFontName != null)
					characterFormatting.FontName = oldFontName;
			}
		}
	}
	#endregion
}
