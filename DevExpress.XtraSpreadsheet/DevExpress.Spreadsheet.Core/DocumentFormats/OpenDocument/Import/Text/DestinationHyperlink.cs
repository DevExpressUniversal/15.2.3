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

#if OPENDOCUMENT
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using System.Text;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument {
	public class HyperlinkDestination : TextBuilderElementBase {
#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("s", OnSpace);
			result.Add("tab", OnTab);
			result.Add("line-break", OnLineBreak);
			return result;
		}
		static HyperlinkDestination GetThis(OpenDocumentWorkbookImporter importer) {
			return (HyperlinkDestination)importer.PeekDestination();
		}
		#endregion
#region Fields
		CellRange hyperlinkRange;
		string uri;
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public HyperlinkDestination(OpenDocumentWorkbookImporter importer, StringBuilder cellTextBuilder, CellRange hyperlinkRange)
			: base(importer, cellTextBuilder) {
			this.hyperlinkRange = hyperlinkRange;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.uri = reader.GetAttribute("xlink:href");
			if (uri == null)
				uri = string.Empty;
		}
		public override bool ProcessText(XmlReader reader) {
			string displayName = Importer.DecodeXmlChars(reader.Value);
			ModelHyperlink hyperlink = new ModelHyperlink(Importer.CurrentSheet, hyperlinkRange, uri, true);
			hyperlink.DisplayText = displayName;
			Importer.CurrentSheet.Hyperlinks.Add(hyperlink);
			CellTextBuilder.Append(displayName);
			return true;
		}
#region Handlers
		static Destination OnSpace(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new SpaceDestianation(importer, GetThis(importer).CellTextBuilder);
		}
		static Destination OnTab(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new TabDestination(importer, GetThis(importer).CellTextBuilder);
		}
		static Destination OnLineBreak(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new LineBreakDestination(importer, GetThis(importer).CellTextBuilder);
		#endregion
		}
	}
}
#endif
