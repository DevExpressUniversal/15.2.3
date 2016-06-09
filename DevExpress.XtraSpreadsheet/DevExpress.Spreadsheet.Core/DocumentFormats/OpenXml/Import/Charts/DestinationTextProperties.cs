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
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region TextPropertiesDestination
	public class TextPropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("bodyPr", OnBodyProperties);
			result.Add("lstStyle", OnListStyles);
			result.Add("p", OnParagraphs);
			return result;
		}
		#endregion
		static TextPropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (TextPropertiesDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnBodyProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingTextBodyPropertiesDestination(importer, GetThis(importer).properties.BodyProperties);
		}
		static Destination OnListStyles(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingTextListStylesDestination(importer, GetThis(importer).properties.ListStyles);
		}
		static Destination OnParagraphs(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			TextProperties properties = GetThis(importer).properties;
			DrawingTextParagraph paragraph = new DrawingTextParagraph(importer.DocumentModel);
			properties.Paragraphs.AddWithoutHistoryAndNotifications(paragraph);
			return new DrawingTextParagraphDestination(importer, paragraph);
		}
		#endregion
		#endregion
		#region Fields
		readonly TextProperties properties;
		#endregion
		public TextPropertiesDestination(SpreadsheetMLBaseImporter importer, TextProperties properties)
			: base(importer) {
			this.properties = properties;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
	}
	#endregion
}
