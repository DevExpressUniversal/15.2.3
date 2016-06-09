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
using System.IO;
using System.Xml;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ChartViewDestinationBase
	public abstract class ChartViewDestinationBase : ElementDestination<SpreadsheetMLBaseImporter> {
		internal static Dictionary<string, ChartGrouping> chartGroupingTable = DictionaryUtils.CreateBackTranslationTable(OpenXmlExporter.ChartGroupingTable);
		#region Handler Table
		protected static void AddAxisIdHandler(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("axId", OnAxisId);
		}
		static ChartViewDestinationBase GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartViewDestinationBase)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ChartViewBase view;
		readonly List<int> viewAxesList;
		#endregion
		protected ChartViewDestinationBase(SpreadsheetMLBaseImporter importer, ChartViewBase view, List<int> viewAxesList)
			: base(importer) {
			this.view = view;
			this.viewAxesList = viewAxesList;
		}
		#region Handlers
		static Destination OnAxisId(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			List<int> viewAxesList = GetThis(importer).viewAxesList;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (viewAxesList.Count != 2 || value != 0)
						viewAxesList.Add(value); 
				},
				"val", 0);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			this.view.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			this.view.EndUpdate();
			base.ProcessElementClose(reader);
		}
	}
	#endregion
	#region InnerShapePropertiesDestination
	public class InnerShapePropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("spPr", OnShapeProperties);
			return result;
		}
		static InnerShapePropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (InnerShapePropertiesDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ShapeProperties shapeProperties;
		#endregion
		public InnerShapePropertiesDestination(SpreadsheetMLBaseImporter importer, ShapeProperties shapeProperties)
			: base(importer) {
			this.shapeProperties = shapeProperties;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).shapeProperties);
		}
		#endregion
	}
	#endregion
}
