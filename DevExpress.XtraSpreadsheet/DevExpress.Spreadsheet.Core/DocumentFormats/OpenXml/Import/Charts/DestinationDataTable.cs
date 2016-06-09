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
using System.IO;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DataTableDestination
	public class DataTableDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("showHorzBorder", OnShowHorzBorder);
			result.Add("showVertBorder", OnShowVertBorder);
			result.Add("showOutline", OnShowOutline);
			result.Add("showKeys", OnShowKeys);
			result.Add("spPr", OnShapeProperties);
			result.Add("txPr", OnTextProperties);
			return result;
		}
		static DataTableDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DataTableDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly DataTableOptions options;
		#endregion
		public DataTableDestination(SpreadsheetMLBaseImporter importer, DataTableOptions options)
			: base(importer) {
			this.options = options;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnShowHorzBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataTableOptions options = GetThis(importer).options;
			return new OnOffValueDestination(importer,
				delegate(bool value) { options.ShowHorizontalBorder = value; },
				"val", true);
		}
		static Destination OnShowVertBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataTableOptions options = GetThis(importer).options;
			return new OnOffValueDestination(importer,
				delegate(bool value) { options.ShowVerticalBorder = value; },
				"val", true);
		}
		static Destination OnShowOutline(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataTableOptions options = GetThis(importer).options;
			return new OnOffValueDestination(importer,
				delegate(bool value) { options.ShowOutline = value; },
				"val", true);
		}
		static Destination OnShowKeys(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataTableOptions options = GetThis(importer).options;
			return new OnOffValueDestination(importer,
				delegate(bool value) { options.ShowLegendKeys = value; },
				"val", true);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).options.ShapeProperties);
		}
		static Destination OnTextProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataTableOptions options = GetThis(importer).options;
			return new TextPropertiesDestination(importer, options.TextProperties);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			this.options.BeginUpdate();
			this.options.Visible = true;
		}
		public override void ProcessElementClose(XmlReader reader) {
			this.options.EndUpdate();
		}
	}
	#endregion
}
