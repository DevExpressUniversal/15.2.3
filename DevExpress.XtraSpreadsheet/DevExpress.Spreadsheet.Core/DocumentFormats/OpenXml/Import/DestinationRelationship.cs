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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using System.Xml;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region RelationshipsDestination
	public class RelationshipsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("Relationship", OnRelation);
			return result;
		}
		static RelationshipsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (RelationshipsDestination)importer.PeekDestination();
		}
		readonly OpenXmlRelationCollection relations;
		public RelationshipsDestination(SpreadsheetMLBaseImporter importer, OpenXmlRelationCollection relations)
			: base(importer) {
			Guard.ArgumentNotNull(relations, "relations");
			this.relations = relations;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnRelation(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new RelationshipDestination(importer, GetThis(importer).relations);
		}
	}
	#endregion
	#region RelationshipDestination
	public class RelationshipDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly OpenXmlRelationCollection relations;
		public RelationshipDestination(SpreadsheetMLBaseImporter importer, OpenXmlRelationCollection relations)
			: base(importer) {
			Guard.ArgumentNotNull(relations, "relations");
			this.relations = relations;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			OpenXmlRelation relation = new OpenXmlRelation();
			relation.Id = Importer.ReadAttribute(reader, "Id");
			if (String.IsNullOrEmpty(relation.Id))
				return;
			relation.Type = Importer.ReadAttribute(reader, "Type");
			if (String.IsNullOrEmpty(relation.Type))
				return;
			string target = Importer.ReadAttribute(reader, "Target");
			if (String.IsNullOrEmpty(target))
				return;
			relation.Target = DevExpress.XtraPrinting.HtmlExport.Native.DXHttpUtility.UrlDecode(target);
			relation.TargetMode = Importer.ReadAttribute(reader, "TargetMode");
			relations.Add(relation);
		}
	}
	#endregion
}
