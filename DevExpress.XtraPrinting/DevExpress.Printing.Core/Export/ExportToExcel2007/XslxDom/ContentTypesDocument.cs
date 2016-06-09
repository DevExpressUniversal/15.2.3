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
using System.Xml;
namespace DevExpress.XtraExport {
	public class ContentTypesDocument : OpenXmlDocument {
		string uri = @"http://schemas.openxmlformats.org/package/2006/content-types";
		public ContentTypesDocument(int sheetCount, int drawingsCount) {
			XmlNode typesNode = CreateNode(XmlNodeType.Element, "Types", uri);
			AppendChild(typesNode);
			XmlNode defaultNode0 = CreateNode(XmlNodeType.Element, "Default", uri);
			typesNode.AppendChild(defaultNode0);
			XlsxHelper.AppendAttribute(defaultNode0, "Extension", "png");
			XlsxHelper.AppendAttribute(defaultNode0, "ContentType", @"image/png");
			XmlNode defaultNode1 = CreateNode(XmlNodeType.Element, "Default", uri);
			typesNode.AppendChild(defaultNode1);
			XlsxHelper.AppendAttribute(defaultNode1, "Extension", "xml");
			XlsxHelper.AppendAttribute(defaultNode1, "ContentType", @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml");
			XmlNode defaultNode2 = CreateNode(XmlNodeType.Element, "Default", uri);
			typesNode.AppendChild(defaultNode2);
			XlsxHelper.AppendAttribute(defaultNode2, "Extension", "rels");
			XlsxHelper.AppendAttribute(defaultNode2, "ContentType", @"application/vnd.openxmlformats-package.relationships+xml");
			for(int i = 0; i < drawingsCount; i++) {
				XmlNode overrideNode0 = CreateNode(XmlNodeType.Element, "Override", uri);
				typesNode.AppendChild(overrideNode0);
				XlsxHelper.AppendAttribute(overrideNode0, "PartName", XlsxHelper.Separator + string.Format(XlsxHelper.DrawingPath, i));
				XlsxHelper.AppendAttribute(overrideNode0, "ContentType", @"application/vnd.openxmlformats-officedocument.drawing+xml");
			}
			for(int i = 0; i < sheetCount; i++) {
				XmlNode overrideNode1 = CreateNode(XmlNodeType.Element, "Override", uri);
				typesNode.AppendChild(overrideNode1);
				XlsxHelper.AppendAttribute(overrideNode1, "PartName", XlsxHelper.Separator + string.Format(XlsxHelper.WorksheetPath, i));
				XlsxHelper.AppendAttribute(overrideNode1, "ContentType", @"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml");
			}
			XmlNode overrideNode2 = CreateNode(XmlNodeType.Element, "Override", uri);
			typesNode.AppendChild(overrideNode2);
			XlsxHelper.AppendAttribute(overrideNode2, "PartName", XlsxHelper.Separator + XlsxHelper.SharedStringsPath);
			XlsxHelper.AppendAttribute(overrideNode2, "ContentType", @"application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml");
			XmlNode overrideNode3 = CreateNode(XmlNodeType.Element, "Override", uri);
			typesNode.AppendChild(overrideNode3);
			XlsxHelper.AppendAttribute(overrideNode3, "PartName", XlsxHelper.Separator + XlsxHelper.StyleSheetPath);
			XlsxHelper.AppendAttribute(overrideNode3, "ContentType", @"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml");
		}
		public override string NamespaceURI {
			get { return uri; }
		}
	}
}
