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

using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DrawingTextListStylesDestination
	public class DrawingTextListStylesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("defPPr", OnDefaultParagraphProperties);
			result.Add("lvl1pPr", OnLevel1ParagraphProperties);
			result.Add("lvl2pPr", OnLevel2ParagraphProperties);
			result.Add("lvl3pPr", OnLevel3ParagraphProperties);
			result.Add("lvl4pPr", OnLevel4ParagraphProperties);
			result.Add("lvl5pPr", OnLevel5ParagraphProperties);
			result.Add("lvl6pPr", OnLevel6ParagraphProperties);
			result.Add("lvl7pPr", OnLevel7ParagraphProperties);
			result.Add("lvl8pPr", OnLevel8ParagraphProperties);
			result.Add("lvl9pPr", OnLevel9ParagraphProperties);
			return result;
		}
		#endregion
		static DrawingTextListStylesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DrawingTextListStylesDestination)importer.PeekDestination();
		}
		static DrawingTextParagraphPropertiesDestination GetParagraphPropertiesDestination(SpreadsheetMLBaseImporter importer, int index) {
			return new DrawingTextParagraphPropertiesDestination(importer, GetThis(importer).styles[index]);
		}
		#region Handlers
		static Destination OnDefaultParagraphProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingTextParagraphPropertiesDestination(importer, GetThis(importer).styles.DefaultParagraphStyle);
		}
		static Destination OnLevel1ParagraphProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetParagraphPropertiesDestination(importer, 0);
		}
		static Destination OnLevel2ParagraphProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetParagraphPropertiesDestination(importer, 1);
		}
		static Destination OnLevel3ParagraphProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetParagraphPropertiesDestination(importer, 2);
		}
		static Destination OnLevel4ParagraphProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetParagraphPropertiesDestination(importer, 3);
		}
		static Destination OnLevel5ParagraphProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetParagraphPropertiesDestination(importer, 4);
		}
		static Destination OnLevel6ParagraphProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetParagraphPropertiesDestination(importer, 5);
		}
		static Destination OnLevel7ParagraphProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetParagraphPropertiesDestination(importer, 6);
		}
		static Destination OnLevel8ParagraphProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetParagraphPropertiesDestination(importer, 7);
		}
		static Destination OnLevel9ParagraphProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetParagraphPropertiesDestination(importer, 8);
		}
		#endregion
		#endregion
		#region Fields
		readonly DrawingTextListStyles styles;
		#endregion
		public DrawingTextListStylesDestination(SpreadsheetMLBaseImporter importer, DrawingTextListStyles styles)
			: base(importer) {
			this.styles = styles;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
	}
	#endregion
}
