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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using System.Xml;
using DevExpress.Office.Utils;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region WorksheetPropertiesDestination
	public class WorksheetPropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tabColor", OnSheetTabColor);
			result.Add("outlinePr", OnOutlineProperties);
			result.Add("pageSetUpPr", OnPageSetupProperties);
			return result;
		}
		static WorksheetPropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (WorksheetPropertiesDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ColorModelInfo tabColorInfo;
		#endregion
		public WorksheetPropertiesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			this.tabColorInfo = new ColorModelInfo();
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnOutlineProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetOutlinePropertiesDestination(importer, importer.CurrentWorksheet.Properties.GroupAndOutlineProperties);
		}
		static Destination OnPageSetupProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetPageSetupPropertiesDestination(importer, importer.CurrentWorksheet.PrintSetup);
		}
		static Destination OnSheetTabColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorDestination(importer, GetThis(importer).tabColorInfo);
		}
		WorksheetProperties GetProperties() {
			WorksheetProperties properties = Importer.CurrentSheet.Properties as WorksheetProperties;
			if (properties == null)
				Exceptions.ThrowInvalidOperationException("Unexpected tag sheetPr");
			return properties;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			WorksheetProperties properties = GetProperties();
			TransitionOptions options = properties.TransitionOptions;
			options.TransitionFormulaEvaluation = Importer.GetOnOffValue(reader, "transitionEvaluation", false);
			options.TransitionFormulaEntry = Importer.GetOnOffValue(reader, "transitionEntry", false);
			string codeName = Importer.ReadAttribute(reader, "codeName");
			if(!string.IsNullOrEmpty(codeName)) {
				if(!CodeNameHelper.IsValidCodeName(codeName))
					codeName = CodeNameHelper.CleanUp(codeName);
				properties.CodeName = codeName;
			}
		}
		public override void ProcessElementClose(XmlReader reader) {
			WorksheetProperties properties = GetProperties();
			ColorModelInfoCache cache = Importer.DocumentModel.Cache.ColorModelInfoCache;
			properties.TabColorIndex = cache.GetItemIndex(tabColorInfo);
		}
	}
	#endregion
}
