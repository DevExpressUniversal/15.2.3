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
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.WordML {
	#region NumberingsDestination
	public class NumberingsDestination : DevExpress.XtraRichEdit.Import.OpenXml.NumberingsDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("listDef", OnListDef);
			result.Add("list", OnList);
			return result;
		}
		public NumberingsDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected static Destination OnListDef(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListDefDestination(importer);
		}
		protected static Destination OnList(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WordMLListDestination(importer);
		}
		public override void ProcessElementClose(XmlReader reader) {
			Importer.CreateNumberingLists();
		}
	}
	#endregion
	#region ListDefDestination
	public class ListDefDestination : DevExpress.XtraRichEdit.Import.OpenXml.AbstractNumberingListDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("lvl", OnLevel);
			result.Add("plt", OnMultilevelType);		   
			result.Add("name", OnName);
			result.Add("lsid", OnUniqueId);
			result.Add("numStyleLink", OnNumberingStyleLink);
			result.Add("styleLink", OnStyleLink);
			result.Add("tmpl", OnTemplate);
			return result;
		}
		public ListDefDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnLevel(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelDestination(importer, GetThis(importer).List);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.Id = reader.GetAttribute("listDefId", Importer.WordProcessingNamespaceConst);
		}
	}
	#endregion
	#region ListLevelDestination
	public class ListLevelDestination : DevExpress.XtraRichEdit.Import.OpenXml.NumberingLevelDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		protected static new ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("isLgl", OnLegalNumbering);
			result.Add("lvlJc", OnTextAlignment);
			result.Add("lvlRestart", OnRestart);
			result.Add("lvlText", OnText);
			result.Add("start", OnStart);
			result.Add("suff", OnSuffix);
			result.Add("nfc", OnNumberingFormat);
			result.Add("pPr", OnParagraphProperties);
			result.Add("rPr", OnRunProperties);
			result.Add("pStyle", OnParagraphStyleReference);
			return result;
		}
		public ListLevelDestination(WordProcessingMLBaseImporter importer, AbstractNumberingList list)
			: base(importer, list) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region WordMLListDestination
	public class WordMLListDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("ilst", OnAbstractNumberingId);
			result.Add("lvlOverride", OnLevelOverride);
			return result;
		}
		readonly OpenXmlNumberingListInfo listInfo;
		public WordMLListDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
			this.listInfo = new OpenXmlNumberingListInfo();
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static WordMLListDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (WordMLListDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			listInfo.Id = Importer.GetWpSTIntegerValue(reader, "ilfo", Int32.MinValue);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (String.IsNullOrEmpty(listInfo.AbstractNumberingListId) || listInfo.Id == Int32.MinValue)
				return;
			Importer.ListInfos.Add(listInfo);
		}
		static Destination OnAbstractNumberingId(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AbstractNumberingListReferenceDestination(importer, GetThis(importer).listInfo);
		}
		static Destination OnLevelOverride(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelOverrideDestination(importer, GetThis(importer).listInfo);
		}
	}
	#endregion
}
