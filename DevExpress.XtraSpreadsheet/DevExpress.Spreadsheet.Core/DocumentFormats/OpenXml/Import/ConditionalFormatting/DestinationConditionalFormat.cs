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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	internal class ConditionalFormattingCreatorInitData {
		internal Worksheet Sheet { get; set; }
		internal bool IsPivot { get; set; }
		internal CellRangeBase CellRange { get; set; }
		internal bool ShowValue { get; set; }
		internal bool IsActivePresent { get; set; }
		internal string ActivePresent { get; set; }
		internal bool StopIfTrue { get; set; }
		internal int Priority { get; set; }
		internal int DxfId { get; set; }
		internal ConditionalFormattingRuleType RuleType { get; set; }
		internal ConditionalFormattingCreatorData Details;
		internal ConditionalFormatting FormattingObject;
	}
	#region ConditionalFormattingDestination
	public class ConditionalFormattingDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cfRule", OnRule);
			result.Add("extLst", OnFutureExtension);
			return result;
		}
		static ConditionalFormattingDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ConditionalFormattingDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		ConditionalFormattingCreatorInitData initData;
		#endregion
		public ConditionalFormattingDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			initData = new ConditionalFormattingCreatorInitData();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		internal ConditionalFormattingCreatorInitData InitData { get { return initData; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			InitData.Sheet = Importer.CurrentWorksheet;
			InitData.CellRange = Importer.GetWpSTSqref(reader, "sqref", Importer.CurrentWorksheet);
			InitData.IsPivot = Importer.GetOnOffValue(reader, "pivot", false);
		}
		static Destination OnRule(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ConditionalFormattingRuleDestination(importer, GetThis(importer).InitData);
		}
		static Destination OnFutureExtension(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
	}
	#endregion
	#region ConditionalFormattingRuleExtLstDestination
	internal delegate void ConditionalFormattingExtRuleSqRefRegistrar(ConditionalFormattingCreatorData item);
	public class ConditionalFormattingExtCondFmtDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		CellRangeBase cellRange;
		List<ConditionalFormattingCreatorData> sqRefTargetCollection;
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cfRule", OnRule); 
			result.Add("sqref", OnSqRef); 
			return result;
		}
		#endregion
		public ConditionalFormattingExtCondFmtDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			sqRefTargetCollection = new List<ConditionalFormattingCreatorData>();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public CellRangeBase CellRange { get { return cellRange; } }
		List<ConditionalFormattingCreatorData> SqRefTargetCollection { get { return sqRefTargetCollection; } }
		#endregion
		static ConditionalFormattingExtCondFmtDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ConditionalFormattingExtCondFmtDestination)importer.PeekDestination();
		}
		void SqRefTargetRegistrar(ConditionalFormattingCreatorData item) {
			SqRefTargetCollection.Add(item);
		}
		static Destination OnRule(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingExtCondFmtDestination self = GetThis(importer) as ConditionalFormattingExtCondFmtDestination;
			return new ConditionalFormattingExtRuleDestination(importer, self.SqRefTargetRegistrar);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if(CellRange != null)
				foreach(ConditionalFormattingCreatorData item in SqRefTargetCollection)
					item.CellRange = CellRange;
		}
		void SqRefAction(string value) {
			if(String.IsNullOrEmpty(value))
				return;
			cellRange = CellRangeBase.CreateRangeBase(Importer.CurrentWorksheet, value, ' ');
		}
		static Destination OnSqRef(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingExtCondFmtDestination self = GetThis(importer);
			return new ConditionalFormattingFormulaDestination(importer, self.SqRefAction);
		}
	}
	#endregion
}
