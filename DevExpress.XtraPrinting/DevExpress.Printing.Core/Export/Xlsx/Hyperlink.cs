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
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xlsx {
	using DevExpress.Office;
	partial class XlsxDataAwareExporter {
		const string officeHyperlinkType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink";
		void GenerateHyperlinks(IList<XlHyperlink> hyperlinks) {
			if(!ShouldExportHyperlinks(hyperlinks))
				return;
			WriteShStartElement("hyperlinks");
			try {
				foreach(XlHyperlink hyperlink in hyperlinks)
					GenerateHyperlink(hyperlink);
			}
			finally {
				WriteShEndElement();
			}
		}
		bool ShouldExportHyperlinks(IList<XlHyperlink> hyperlinks) {
			if(hyperlinks == null || hyperlinks.Count == 0)
				return false;
			foreach(XlHyperlink item in hyperlinks) {
				if(item.Reference != null && !string.IsNullOrEmpty(item.TargetUri))
					return true;
			}
			return false;
		}
		void GenerateHyperlink(XlHyperlink hyperlink) {
			if(hyperlink.Reference == null || string.IsNullOrEmpty(hyperlink.TargetUri))
				return;
			WriteShStartElement("hyperlink");
			try {
				WriteStringValue("ref", hyperlink.Reference.ToString());
				string targetUri = hyperlink.TargetUri;
				string location = string.Empty;
				int pos = targetUri.IndexOf('#');
				if(pos != -1) {
					location = targetUri.Substring(pos + 1);
					targetUri = targetUri.Substring(0, pos);
				}
				if(!string.IsNullOrEmpty(targetUri)) {
					string rId = sheetRelations.GenerateId();
					sheetRelations.Add(new OpenXmlRelation(rId, Uri.EscapeUriString(targetUri), officeHyperlinkType, "External"));
					WriteStringAttr(XlsxPackageBuilder.RelsPrefix, "id", null, rId);
				}
				if(!string.IsNullOrEmpty(location))
					WriteStringValue("location", EncodeXmlChars(location));
				if(!string.IsNullOrEmpty(hyperlink.Tooltip))
					WriteStringValue("tooltip", EncodeXmlChars(hyperlink.Tooltip));
				if(!string.IsNullOrEmpty(hyperlink.DisplayText))
					WriteStringValue("display", EncodeXmlChars(hyperlink.DisplayText));
			}
			finally {
				WriteShEndElement();
			}
		}
	}
}
