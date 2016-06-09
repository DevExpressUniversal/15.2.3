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
using System.Globalization;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DocumentApplicationPropertiesDestination
	public class DocumentApplicationPropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("Application", OnApplication);
			result.Add("Manager", OnManager);
			result.Add("Company", OnCompany);
			result.Add("AppVersion", OnVersion);
			result.Add("DocSecurity", OnDocSecurity);
			return result;
		}
		#endregion
		public DocumentApplicationPropertiesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		ModelDocumentApplicationProperties Properties { get { return Importer.DocumentModel.DocumentApplicationProperties; } }
		static DocumentApplicationPropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DocumentApplicationPropertiesDestination)importer.PeekDestination();
		}
		static Destination OnApplication(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignApplication);
		}
		void AssignApplication(string value) {
			Properties.Application = value;
		}
		static Destination OnManager(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignManager);
		}
		void AssignManager(string value) {
			Properties.Manager = value;
		}
		static Destination OnCompany(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignCompany);
		}
		void AssignCompany(string value) {
			Properties.Company = value;
		}
		static Destination OnVersion(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignVersion);
		}
		void AssignVersion(string value) {
			Properties.Version = value;
		}
		static Destination OnDocSecurity(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentPropertyDestination(importer, GetThis(importer).AssignDocSecurity);
		}
		void AssignDocSecurity(string value) {
			if (!String.IsNullOrEmpty(value)) {
				long propValue;
				if (Int64.TryParse(value, NumberStyles.Integer, Importer.DocumentModel.Culture, out propValue))
					Properties.Security = (ModelDocumentSecurity)propValue;
			}
		}
	}
	#endregion
}
