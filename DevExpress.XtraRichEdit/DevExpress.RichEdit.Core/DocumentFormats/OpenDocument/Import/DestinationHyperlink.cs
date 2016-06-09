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
using DevExpress.XtraRichEdit.Export.OpenDocument;
using System.Xml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region HyperlinkDestination
	class HyperlinkDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("span", OnSpanDestination);
			return result;
		}
		HyperlinkInfo hyperlinkInfo;
		ImportFieldHelper helper;
		public HyperlinkDestination(OpenDocumentTextImporter importer)
			: base(importer) {
			helper = new ImportFieldHelper(Importer.PieceTable);
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		internal ImportFieldHelper Helper { get { return helper; } }
		static Destination OnSpanDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new SpanDestination(importer);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			hyperlinkInfo = new HyperlinkInfo();
			if (hyperlinkInfo == null)
				Importer.ThrowInvalidFile();
			string uri = ImportHelper.GetXlinkStringAttribute(reader, "href");
			if (!String.IsNullOrEmpty(uri) && uri[0] == '#'){
				int separatorPos = uri.LastIndexOf('|');
				hyperlinkInfo.Anchor = (separatorPos > 0) ? uri.Substring(1, separatorPos - 1) : uri.Substring(1);
			}
			else
				hyperlinkInfo.NavigateUri = uri;
			hyperlinkInfo.ToolTip = ImportHelper.GetOfficeStringAttribute(reader, "name");
			if (!DocumentFormatsHelper.ShouldInsertHyperlinks(Importer.DocumentModel))
				return;
			ImportFieldInfo fieldInfo = new ImportFieldInfo(Importer.PieceTable);
			Importer.FieldInfoStack.Push(fieldInfo);
			helper.ProcessFieldBegin(fieldInfo, Importer.InputPosition);
			helper.InsertHyperlinkInstruction(hyperlinkInfo, Importer.InputPosition);
			helper.ProcessFieldSeparator(fieldInfo, Importer.InputPosition);
			Importer.InputPosition.SaveCharacterFormatting();
			int styleIndex = GetCharacterStyleIndexByName(CharacterStyleCollection.HyperlinkStyleName);
			Importer.InputPosition.CharacterStyleIndex = styleIndex;
		}
		protected internal virtual int GetCharacterStyleIndexByName(string name) {
			return Math.Max(0, Importer.DocumentModel.CharacterStyles.GetStyleIndexByName(name));
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!DocumentFormatsHelper.ShouldInsertHyperlinks(Importer.DocumentModel))
				return;
			ImportFieldInfo fieldInfo = Importer.FieldInfoStack.Pop();
			helper.ProcessFieldEnd(fieldInfo, Importer.InputPosition);
			if (Importer.FieldInfoStack.Count > 0)
				fieldInfo.Field.Parent = Importer.FieldInfoStack.Peek().Field;
			Importer.InputPosition.RestoreCharacterFormatting();
		}
		public override bool ProcessText(XmlReader reader) {
			string fieldValue = ReadValue(reader);
			if (String.IsNullOrEmpty(fieldValue))
				return true;
			if (reader.XmlSpace != XmlSpace.Preserve)
				fieldValue = ImportHelper.RemoveRedundantSpaces(fieldValue, false);
			if (!String.IsNullOrEmpty(fieldValue))
				Importer.PieceTable.InsertTextCore(Importer.InputPosition, fieldValue);
			return true;
		}
		private static string ReadValue(XmlReader reader) {
			string readerValue = reader.Value;
			return (readerValue == null) ?  String.Empty : readerValue;
		}
	}
	#endregion
}
