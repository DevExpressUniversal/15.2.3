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
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public class CalcPropertiesDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		const string ModeA1 = "A1";
		const string ModeR1C1 = "R1C1";
		static Dictionary<ModelCalculationMode, string> CalculationModeTable = OpenXmlExporter.calculationModeTable;
		#endregion
		public CalcPropertiesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		public WorkbookProperties WorkbookProperties { get { return Importer.DocumentModel.Properties; } }
		public CalculationOptions CalculationOptions { get { return WorkbookProperties.CalculationOptions; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			CalculationOptions.BeginUpdate();
			try {
				ReadAttributes(reader);
			}
			finally {
				CalculationOptions.EndUpdate();
			}
		}
		void ReadAttributes(XmlReader reader) {
			ParseRefMode(reader);
			CalculationOptions.CalculationMode = Importer.GetWpEnumValue<ModelCalculationMode>(reader, "calcMode", CalculationModeTable, ModelCalculationMode.Automatic);
			CalculationOptions.RecalculateBeforeSaving = Importer.GetWpSTOnOffValue(reader, "calcOnSave", true);
			CalculationOptions.FullCalculationOnLoad = Importer.GetWpSTOnOffValue(reader, "fullCalcOnLoad", false);
			CalculationOptions.SetPrecisionAsDisplayedCore(!Importer.GetWpSTOnOffValue(reader, "fullPrecision", true));
			CalculationOptions.SetIterationsEnabledCore(Importer.GetWpSTOnOffValue(reader, "iterate", false));
			CalculationOptionsInfo defaultItem = Importer.DocumentModel.Cache.CalculationOptionsInfoCache.DefaultItem;
			CalculationOptions.MaximumIterations = Math.Max(0, Importer.GetWpSTIntegerValue(reader, "iterateCount", defaultItem.MaximumIterations));
			CalculationOptions.IterativeCalculationDelta = Importer.GetWpDoubleValue(reader, "iterateDelta", defaultItem.IterativeCalculationDelta);
		}
		void ParseRefMode(XmlReader reader) {
			string refMode = Importer.ReadAttribute(reader, "refMode");
			if (String.IsNullOrEmpty(refMode))
				return;
			if (refMode == ModeR1C1)
				WorkbookProperties.UseR1C1ReferenceStyle = true;
			else if (refMode == ModeA1)
				WorkbookProperties.UseR1C1ReferenceStyle = false;
		}
	}
}
