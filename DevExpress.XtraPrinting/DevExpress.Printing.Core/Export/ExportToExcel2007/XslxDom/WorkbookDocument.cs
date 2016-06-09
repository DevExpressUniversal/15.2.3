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
	public class SheetNode : OpenXmlElement{
		string sheetName;
		int index;
		public SheetNode(XmlDocument document, int index)
			: base(document) {
			this.index = index;
			XlsxHelper.AppendAttribute(this, "id", string.Format(XlsxHelper.SheetRelID, index), "r", XlsxHelper.RelationshipsNs);
		}
		public override string LocalName {
			get { return "sheet"; }
		}
		public string SheetName { get { return sheetName; } set { sheetName = value; } }
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(this, "sheetId", (this.ParentNode.ChildNodes.Count - index).ToString());
			XlsxHelper.AppendAttribute(this, "name", sheetName);
		}
	}
	public class SheetsNode : OpenXmlElement {
		List<SheetNode> sheetNodes;
		public SheetsNode(XmlDocument document)
			: base(document) {
			sheetNodes = new List<SheetNode>();
		}
		public override string LocalName {
			get { return "sheets"; }
		}
		public SheetNode SheetNode { get { return sheetNodes[sheetNodes.Count-1]; } }
		public void AppendSheetNode(string sheetName) {
			SheetNode sheetNode = new SheetNode(OwnerDocument, sheetNodes.Count);
			AppendChild(sheetNode);
			sheetNodes.Add(sheetNode);
			sheetNode.SheetName = sheetName;
		}
	}
	public class WorkBookProperiesNode : OpenXmlElement {
		public WorkBookProperiesNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "workbookPr"; }
		}
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(this, "date1904", "0");
		}
	}
	public class WorkbookDocument : OpenXmlDocument {
		SheetsNode sheetsNode;
		public WorkbookDocument() {
			XmlNode workbookNode = CreateNode(XmlNodeType.Element, "workbook", XlsxHelper.MainNs);
			AppendChild(workbookNode);
			XlsxHelper.AppendAttribute(workbookNode, "xmlns:r", XlsxHelper.RelationshipsNs);
			sheetsNode = new SheetsNode(this);
			workbookNode.AppendChild(new WorkBookProperiesNode(this));
			workbookNode.AppendChild(sheetsNode);
		}
		public SheetsNode SheetsNode { get { return sheetsNode; } }
		public override string NamespaceURI {
			get { return XlsxHelper.MainNs; }
		}
		public void AppendSheet(string sheetName) {
			sheetsNode.AppendSheetNode(sheetName);
		}
   }
}
