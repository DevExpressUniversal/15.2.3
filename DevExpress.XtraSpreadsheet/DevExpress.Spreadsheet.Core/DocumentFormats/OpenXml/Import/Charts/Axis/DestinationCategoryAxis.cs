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
	#region CategoryAxisDestination
	public class CategoryAxisDestination : ChartAxisDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("auto", OnAutoCategoryAxis);
			result.Add("lblAlgn", OnLabelAlignment);
			result.Add("lblOffset", OnLabelOffset);
			result.Add("tickLblSkip", OnTickLabelSkip);
			result.Add("tickMarkSkip", OnTickMarkSkip);
			result.Add("noMultiLvlLbl", OnNoMultiLevelLabels);
			result.Add("crossesAt", OnCrossesAt);
			AddAxisHandlers(result);
			return result;
		}
		static CategoryAxisDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (CategoryAxisDestination)importer.PeekDestination();
		}
		#endregion
		readonly CategoryAxis axis;
		public CategoryAxisDestination(SpreadsheetMLBaseImporter importer, CategoryAxis axis, List<ChartAxisImportInfo> axisList)
			: base(importer, axisList) {
			this.axis = axis;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected override AxisBase Axis { get { return axis; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			axis.BeginUpdate();
			axis.Auto = true;
			axis.NoMultilevelLabels = true;
			axis.LabelOffset = 100;
			axis.SetTickLabelSkipCore(1);
			axis.SetTickMarkSkipCore(1);
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			axis.EndUpdate();
		}
		#region Handlers
		static Destination OnAutoCategoryAxis(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			CategoryAxis axis = GetThis(importer).axis;
			return new OnOffValueDestination(importer,
				delegate(bool value) { axis.Auto = value; },
				"val", true);
		}
		static Destination OnNoMultiLevelLabels(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			CategoryAxis axis = GetThis(importer).axis;
			return new OnOffValueDestination(importer,
				delegate(bool value) { axis.NoMultilevelLabels = value; },
				"val", true);
		}
		static Destination OnLabelAlignment(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			CategoryAxis axis = GetThis(importer).axis;
			return new EnumValueDestination<LabelAlignment>(importer,
				LabelAlignmentTable,
				delegate(LabelAlignment value) { axis.LabelAlign = value; },
				"val",
				LabelAlignment.Center);
		}
		static Destination OnLabelOffset(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			CategoryAxis axis = GetThis(importer).axis;
			return new IntegerValueDestination(importer,
				delegate(int value) { axis.LabelOffset = value; },
				"val", 100);
		}
		static Destination OnTickLabelSkip(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			CategoryAxis axis = GetThis(importer).axis;
			return new IntegerValueDestination(importer,
				delegate(int value) { axis.SetTickLabelSkipCore(value); },
				"val", 1);
		}
		static Destination OnTickMarkSkip(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			CategoryAxis axis = GetThis(importer).axis;
			return new IntegerValueDestination(importer,
				delegate(int value) { axis.SetTickMarkSkipCore(value); },
				"val", 1);
		}
		static Destination OnCrossesAt(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			CategoryAxis axis = GetThis(importer).axis;
			axis.Crosses = AxisCrosses.AtValue;
			return new FloatValueDestination(importer,
				delegate(float value) { axis.SetCrossesValueCore(value); }, "val");
		}
		#endregion
	}
	#endregion
}
