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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	using DevExpress.XtraSpreadsheet.Export.OpenXml;
	#region WorksheetExtensionListDestination
	public class WorksheetExtensionListDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("ext", OnExt);
			return result;
		}
		#endregion
		public WorksheetExtensionListDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static Destination OnExt(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new WorksheetExtensionDestination(importer);
		}
	}
	#endregion
	#region WorksheetExtensionDestination
	public class WorksheetExtensionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		string uri;
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("conditionalFormattings", OnConditionalFormattings);
			result.Add("dataValidations", OnDataValidations);
			result.Add("sparklineGroups", OnSparklineGroups);
			return result;
		}
		static WorksheetExtensionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (WorksheetExtensionDestination)importer.PeekDestination();
		}
		#endregion
		public WorksheetExtensionDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public string Uri { get { return uri; } protected set { uri = value; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			Uri = Importer.ReadAttribute(reader, "uri");
		}
		static Destination OnConditionalFormattings(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			WorksheetExtensionDestination self = GetThis(importer);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(self.Uri, ConditionalFormatting.OpenXmlUri) == 0) {
				return new ConditionalFormattingExtCondFormattingsDestination(importer);
			}
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnDataValidations(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			WorksheetExtensionDestination self = GetThis(importer);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(self.Uri, OpenXmlExporter.DataValidationExtUri) == 0)
				return new ExtDataValidationsDestination(importer);
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnSparklineGroups(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			WorksheetExtensionDestination self = GetThis(importer);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(self.Uri, OpenXmlExporter.SparklineGroupsExtUri) == 0 &&
				importer.DocumentModel.DocumentCapabilities.SparklinesAllowed)
				return new SparklineGroupsDestination(importer);
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
	}
	#endregion
}
