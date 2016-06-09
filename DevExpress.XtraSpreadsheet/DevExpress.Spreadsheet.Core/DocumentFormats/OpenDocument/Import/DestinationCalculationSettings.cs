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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Globalization;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument {
	#region CalculationSettingsDestination
	public class CalculationSettingsDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("iteration", OnInteration); 
			result.Add("null-date", OnNullDate); 
			return result;
		}
		#endregion
		public CalculationOptions CalculationOptions { get { return Importer.DocumentModel.Properties.CalculationOptions; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public CalculationSettingsDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CalculationOptions.PrecisionAsDisplayed = Importer.GetWpSTOnOffValue(reader, "table:precision-as-shown", false);
			CalculationOptions.AcceptLabelsInFormulas = Importer.GetWpSTOnOffValue(reader, "table:automatic-find-labels", true);
			bool caseSensitive = Importer.GetWpSTOnOffValue(reader, "table:case-sensitive", true);
			if (caseSensitive)
				Importer.DocumentModel.LogMessage(Office.Services.LogCategory.Warning, Localization.XtraSpreadsheetStringId.Msg_OdsCaseSensitiveIgnored);
		}
		#region Handlers
		static Destination OnInteration(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new CalculationSettingsIterationDestination(importer);
		}
		static Destination OnNullDate(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new CalculationSettingsNullDateDestination(importer);
		}
		#endregion
	}
	#endregion
	#region CalculationSettingsIterationDestination
	public class CalculationSettingsIterationDestination : LeafElementDestination {
		public CalculationSettingsIterationDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CalculationOptions options = Importer.DocumentModel.Properties.CalculationOptions;
			options.IterativeCalculationDelta = Importer.GetWpDoubleValue(reader, "table:maximum-difference", 0.001);
			options.MaximumIterations = Math.Max(0, Importer.GetWpSTIntegerValue(reader, "table:steps", 100));
			string status = Importer.ReadAttribute(reader, "table:status");
			options.IterationsEnabled = string.Compare(status, "enable", StringComparison.InvariantCultureIgnoreCase) == 0;
		}
	}
	#endregion
	#region CalculationSettingsNullDateDestination
	public class CalculationSettingsNullDateDestination : LeafElementDestination {
		public CalculationSettingsNullDateDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CalculationOptions options = Importer.DocumentModel.Properties.CalculationOptions;
			DateTime timeSystemStartDate;
			string strDate = Importer.GetAttribute(reader, "table:date-value", "1899-12-30");
			timeSystemStartDate = DateTime.ParseExact(strDate, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
			if (timeSystemStartDate == VariantValue.BaseDate1904)
				options.DateSystem = DateSystem.Date1904;
			else
				if (timeSystemStartDate == VariantValue.BaseDate)
					options.DateSystem = DateSystem.Date1900;
				else
					return;
		}
	}
	#endregion
}
#endif
