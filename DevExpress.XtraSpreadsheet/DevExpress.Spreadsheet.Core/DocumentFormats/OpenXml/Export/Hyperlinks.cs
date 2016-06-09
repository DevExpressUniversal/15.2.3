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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal virtual void GenerateHyperlinksContent() {
			int count = ActiveSheet.Hyperlinks.Count;
			if (count <= 0)
				return;
			WriteShStartElement("hyperlinks");
			try {
				ExportHyperlinks();
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void ExportHyperlinks() {
			ModelHyperlinkCollection hyperlinks = ActiveSheet.Hyperlinks;
			int count = hyperlinks.Count;
			for (int i = 0; i < count; i++)
				ExportHyperlink(hyperlinks[i]);
		}
		protected internal virtual void ExportHyperlink(ModelHyperlink hyperlink) {
			WriteShStartElement("hyperlink");
			try {
				WriteStringValue("ref", hyperlink.Range.ToString());
				if (hyperlink.IsExternal) {
					OpenXmlRelationCollection relations = SheetRelationsTable[ActiveSheet.Name];
					string id = GenerateIdByCollection(relations);
					string targetUri = hyperlink.TargetUri;
					string location = string.Empty;
					int pos = targetUri.IndexOf('#');
					if(pos != -1) {
						location = targetUri.Substring(pos + 1);
						targetUri = targetUri.Substring(0, pos);
					}
					relations.Add(new OpenXmlRelation(id, targetUri, OfficeHyperlinkType, "External"));
					if(!string.IsNullOrEmpty(location))
						WriteStringValue("location", EncodeXmlChars(location));
					DocumentContentWriter.WriteAttributeString(RelsPrefix, "id", RelsNamespace, id);
				}
				else {
					WriteStringValue("location", EncodeXmlChars(hyperlink.TargetUri));
				}
				if (!String.IsNullOrEmpty(hyperlink.DisplayText))
					WriteStringValue("display", EncodeXmlChars(hyperlink.DisplayText));
				if (!String.IsNullOrEmpty(hyperlink.TooltipText))
					WriteStringValue("tooltip", EncodeXmlChars(hyperlink.TooltipText));
			}
			finally {
				WriteShEndElement();
			}
		}
	}
}
