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
#region Base
	public abstract class TextBuilderElementBase : ElementDestination {
#region Properties
		protected StringBuilder CellTextBuilder { get; private set; }
		#endregion
		protected TextBuilderElementBase(OpenDocumentWorkbookImporter importer, StringBuilder cellBuilder)
			: base(importer) {
			this.CellTextBuilder = cellBuilder;
		}
	}
	public abstract class TextBuilderLeafElementBase : LeafElementDestination {
#region Properties
		protected StringBuilder CellTextBuilder { get; private set; }
		#endregion
		protected TextBuilderLeafElementBase(OpenDocumentWorkbookImporter importer, StringBuilder cellBuilder)
			: base(importer) {
			this.CellTextBuilder = cellBuilder;
		}
	}
	#endregion
	public class ParagraphDestination : TextBuilderElementBase {
#region Static members
		static int paragraphCount = 0;
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("s", OnSpace);
			result.Add("tab", OnTab);
			result.Add("line-break", OnLineBreak);
			result.Add("a", OnA);
			result.Add("span", OnSpan);
			return result;
		}
		static ParagraphDestination GetThis(OpenDocumentWorkbookImporter importer) {
			return (ParagraphDestination)importer.PeekDestination();
		}
		public static void RefreshParagraphsCount() {
			paragraphCount = 0;
		}
		#endregion
		CellRange hyperlinkRange;
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public ParagraphDestination(OpenDocumentWorkbookImporter importer, CellRange hyperlinkRange, StringBuilder cellTextBuilder)
			: base(importer, cellTextBuilder) {
			this.hyperlinkRange = hyperlinkRange;
			++paragraphCount;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			if (paragraphCount > 1)
				CellTextBuilder.Append(System.Environment.NewLine);
		}
		public override bool ProcessText(XmlReader reader) {
			string value = reader.Value;
			if (!string.IsNullOrEmpty(value)) {
				CellTextBuilder.Append(Importer.DecodeXmlChars(value));
			}
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
		}
		static Destination OnA(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new HyperlinkDestination(importer, GetThis(importer).CellTextBuilder, GetThis(importer).hyperlinkRange);
		}
		static Destination OnSpan(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new SpanDestination(importer, GetThis(importer).CellTextBuilder);
		}
		#endregion
	}
}
#endif
