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
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region ExternalReferences
		protected internal virtual void GenerateExternalReferences() {
			List<ExternalLink> links = GetExternalLinks();
			int linksCount = links.Count;
			if (linksCount == 0)
				return;
			WriteShStartElement("externalReferences");
			try {
				for (int i = 0; i < linksCount; i++) {
					ExportExternalReference(links[i].Workbook);
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		List<ExternalLink> GetExternalLinks() {
			List<ExternalLink> links = new List<ExternalLink>();
			foreach (ExternalLink link in Workbook.ExternalLinks) {
				DdeExternalWorkbook ddeConnection = link.Workbook as DdeExternalWorkbook;
				if (ddeConnection != null && ddeConnection.IsOleLink)
					continue;
				links.Add(link);
			}
			return links;
		}
		protected internal virtual void ExportExternalReference(ExternalWorkbook workbook) {
			WriteShStartElement("externalReference");
			try {
				string externalRefId = PopulateExternalReferenceTable(workbook);
				WriteStringAttr(RelsPrefix, "id", null, externalRefId);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual string PopulateExternalReferenceTable(ExternalWorkbook workbook) {
			this.externalReferenceCounter++;
			string id = GenerateIdByCollection(Builder.WorkbookRelations);
			string fileName = String.Format("externalLink{0}.xml", this.externalReferenceCounter);
			string externalRefRelationTarget = "externalLinks/" + fileName;
			Builder.WorkbookRelations.Add(new OpenXmlRelation(id, externalRefRelationTarget, RelsExternalLinkNamespace));
			string externalRefPath = @"xl\externalLinks\" + fileName;
			ExternalReferencePathsTable.Add(workbook, externalRefPath);
			Builder.OverriddenContentTypes.Add("/xl/externalLinks/" + fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.externalLink+xml");
			return id;
		}
		#endregion
	}
}
