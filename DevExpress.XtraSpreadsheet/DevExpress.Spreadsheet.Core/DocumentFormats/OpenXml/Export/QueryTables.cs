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
using System.IO;
using System.Xml;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Utils;
using DevExpress.Office;
using DevExpress.Utils.Zip;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		int queryTableCounter;
		int currentQueryTableId;
		#region QueryTables
		protected internal virtual void AddQueryTablesPackageContent() {
			if (!ShouldExportQueryTables())
				return;
			foreach (KeyValuePair<int, string> pair in QueryTablePathsTable) {
				AddPackageContent(pair.Value, ExportQueryTable(pair.Key));
			}
		}
		protected internal virtual bool ShouldExportQueryTables() {
			return Workbook.QueryTablesContent != null && Workbook.QueryTablesContent.Count > 0;
		}
		protected internal virtual CompressedStream ExportQueryTable(int id) {
			currentQueryTableId = id;
			return CreateXmlContent(GenerateQueryTableXmlContent);
		}
		protected internal virtual void GenerateQueryTableXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateQueryTableContent();
		}
		protected internal virtual void GenerateQueryTableContent() {
			string content = Workbook.QueryTablesContent[currentQueryTableId];
			DocumentContentWriter.WriteRaw(content);
		}
		protected internal virtual void PopulateQueryTablesTable(Table table) {
			if (table.TableType != TableType.QueryTable)
				return;
			OpenXmlRelationCollection relations;
			if(!TableRelationsTable.TryGetValue(ActiveTable.Name, out relations))
				return;
			this.queryTableCounter++;
			string fileName = String.Format("queryTable{0}.xml", this.queryTableCounter);
			string queryTableRelationTarget = "../queryTables/" + fileName;
			string id = GenerateIdByCollection(relations);
			OpenXmlRelation relation = new OpenXmlRelation(id, queryTableRelationTarget, RelsQueryTablesNamespace);
			relations.Add(relation);
			string tablePath = @"xl\queryTables\" + fileName;
			QueryTablePathsTable.Add(table.QueryTableContentId, tablePath);
			Builder.OverriddenContentTypes.Add("/xl/queryTables/" + fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.queryTable+xml");
		}
		#endregion
	}
}
