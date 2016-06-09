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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DocumentCorePropertiesDestination
	public class DocumentCorePropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("title", OnTitle);
			result.Add("subject", OnSubject);
			result.Add("creator", OnCreator);
			result.Add("keywords", OnKeywords);
			result.Add("description", OnDescription);
			result.Add("lastModifiedBy", OnLastModifiedBy);
			result.Add("category", OnCategory);
			result.Add("created", OnCreated);
			result.Add("modified", OnModified);
			result.Add("lastPrinted", OnLastPrinted);
			return result;
		}
		#endregion
		public DocumentCorePropertiesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		ModelDocumentCoreProperties Properties { get { return Importer.DocumentModel.DocumentCoreProperties; } }
		static DocumentCorePropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DocumentCorePropertiesDestination)importer.PeekDestination();
		}
		static Destination OnTitle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignTitle);
		}
		void AssignTitle(string value) {
			Properties.Title = value;
		}
		static Destination OnSubject(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignSubject);
		}
		void AssignSubject(string value) {
			Properties.Subject = value;
		}
		static Destination OnCreator(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignCreator);
		}
		void AssignCreator(string value) {
			Properties.Creator = value;
		}
		static Destination OnKeywords(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignKeywords);
		}
		void AssignKeywords(string value) {
			Properties.Keywords = value;
		}
		static Destination OnDescription(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignDescription);
		}
		void AssignDescription(string value) {
			Properties.Description = value;
		}
		static Destination OnLastModifiedBy(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignLastModifiedBy);
		}
		void AssignLastModifiedBy(string value) {
			Properties.LastModifiedBy = value;
		}
		static Destination OnCategory(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignCategory);
		}
		void AssignCategory(string value) {
			Properties.Category = value;
		}
		static Destination OnCreated(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignCreated);
		}
		void AssignCreated(string value) {
			DateTime dateTime;
			if (DateTime.TryParse(value, Importer.DocumentModel.Culture, System.Globalization.DateTimeStyles.None, out dateTime))
				Properties.Created = dateTime;
		}
		static Destination OnModified(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignModified);
		}
		void AssignModified(string value) {
			DateTime dateTime;
			if (DateTime.TryParse(value, Importer.DocumentModel.Culture, System.Globalization.DateTimeStyles.None, out dateTime))
				Properties.Modified = dateTime;
		}
		static Destination OnLastPrinted(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignLastPrinted);
		}
		void AssignLastPrinted(string value) {
			DateTime dateTime;
			if (DateTime.TryParse(value, Importer.DocumentModel.Culture, System.Globalization.DateTimeStyles.None, out dateTime))
				Properties.LastPrinted = dateTime;
		}
	}
	#endregion
	public delegate void AssignPropertyDelegate(string value);
	#region DocumentPropertyDestination
	public class DocumentPropertyDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly AssignPropertyDelegate action;
		public DocumentPropertyDestination(SpreadsheetMLBaseImporter importer, AssignPropertyDelegate action)
			: base(importer) {
			Guard.ArgumentNotNull(action, "action");
			this.action = action;
		}
		public override bool ProcessText(XmlReader reader) {
			action(reader.Value);
			return true;
		}
	}
	#endregion
}
