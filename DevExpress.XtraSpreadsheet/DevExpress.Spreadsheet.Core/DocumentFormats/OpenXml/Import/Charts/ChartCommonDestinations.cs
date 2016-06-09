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
using System.Text;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region LayoutDestination
	public class LayoutDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("manualLayout", OnManualLayout);
			return result;
		}
		static LayoutDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (LayoutDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		LayoutOptions layout;
		#endregion
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable{ get { return handlerTable; } }
		#endregion
		public LayoutDestination(SpreadsheetMLBaseImporter importer, LayoutOptions layout)
			: base(importer) {
			this.layout = layout;
			this.layout.SetAutoCore();
		}
		#region Handlers
		static Destination OnManualLayout(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ManualLayoutDestination(importer, GetThis(importer).layout);
		}
		#endregion
	}
	#endregion
	#region ManualLayoutDestination
	public class ManualLayoutDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static Dictionary<string, LayoutMode> layoutModeTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.LayoutModeTable);
		static Dictionary<string, LayoutTarget> layoutTargetTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.LayoutTargetTable);
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("layoutTarget", OnLayoutTarget); 
			result.Add("hMode", OnHMode); 
			result.Add("wMode", OnWMode); 
			result.Add("xMode", OnXMode); 
			result.Add("yMode", OnYMode); 
			result.Add("x", OnXPosition);
			result.Add("y", OnYPosition);
			result.Add("w", OnWidth);
			result.Add("h", OnHeight);
			return result;
		}
		static ManualLayoutDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ManualLayoutDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		LayoutOptions layout;
		double leftValue = 0;
		double topValue = 0;
		double widthValue = 0;
		double heightValue = 0;
		LayoutMode leftMode = LayoutMode.Auto;
		LayoutMode topMode = LayoutMode.Auto;
		LayoutMode widthMode = LayoutMode.Auto;
		LayoutMode heightMode = LayoutMode.Auto;
		#endregion
		public ManualLayoutDestination(SpreadsheetMLBaseImporter importer, LayoutOptions layout)
			: base(importer) {
			this.layout = layout;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if(topMode != LayoutMode.Auto)
				layout.SetTopCore(new ManualLayoutPosition(topValue, topMode));
			if (leftMode != LayoutMode.Auto)
				layout.SetLeftCore(new ManualLayoutPosition(leftValue, leftMode));
			if (widthMode != LayoutMode.Auto)
				layout.SetWidthCore(new ManualLayoutPosition(widthValue, widthMode));
			if (heightMode != LayoutMode.Auto)
				layout.SetHeightCore(new ManualLayoutPosition(heightValue, heightMode));
		}
		#region Handlers
		static Destination OnXPosition(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ManualLayoutDestination thisDestination = GetThis(importer);
			if (thisDestination.leftMode == LayoutMode.Auto)
				thisDestination.leftMode = LayoutMode.Factor;
			return new FloatValueDestination(importer,
				delegate(float value) { thisDestination.leftValue = value; },
				"val");
		}
		static Destination OnYPosition(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ManualLayoutDestination thisDestination = GetThis(importer);
			if (thisDestination.topMode == LayoutMode.Auto)
				thisDestination.topMode = LayoutMode.Factor;
			return new FloatValueDestination(importer,
				delegate(float value) { thisDestination.topValue = value; },
				"val");
		}
		static Destination OnWidth(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ManualLayoutDestination thisDestination = GetThis(importer);
			if (thisDestination.widthMode == LayoutMode.Auto)
				thisDestination.widthMode = LayoutMode.Factor;
			return new FloatValueDestination(importer,
				delegate(float value) { thisDestination.widthValue = value; },
				"val");
		}
		static Destination OnHeight(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ManualLayoutDestination thisDestination = GetThis(importer);
			if (thisDestination.heightMode == LayoutMode.Auto)
				thisDestination.heightMode = LayoutMode.Factor;
			return new FloatValueDestination(importer,
				delegate(float value) { thisDestination.heightValue = value; },
				"val");
		}
		static Destination OnHMode(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ManualLayoutDestination thisDestination = GetThis(importer);
			return new EnumValueDestination<LayoutMode>(importer,
				layoutModeTable,
				delegate(LayoutMode value) { thisDestination.heightMode = value; },
				"val",
				LayoutMode.Factor
				);
		}
		static Destination OnWMode(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ManualLayoutDestination thisDestination = GetThis(importer);
			return new EnumValueDestination<LayoutMode>(importer,
				layoutModeTable,
				delegate(LayoutMode value) { thisDestination.widthMode = value; },
				"val",
				LayoutMode.Factor
				);
		}
		static Destination OnXMode(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ManualLayoutDestination thisDestination = GetThis(importer);
			return new EnumValueDestination<LayoutMode>(importer,
				layoutModeTable,
				delegate(LayoutMode value) { thisDestination.leftMode = value; },
				"val",
				LayoutMode.Factor
				);
		}
		static Destination OnYMode(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ManualLayoutDestination thisDestination = GetThis(importer);
			return new EnumValueDestination<LayoutMode>(importer,
				layoutModeTable,
				delegate(LayoutMode value) { thisDestination.topMode = value; },
				"val",
				LayoutMode.Factor
				);
		}
		static Destination OnLayoutTarget(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ManualLayoutDestination thisDestination = GetThis(importer);
			return new EnumValueDestination<LayoutTarget>(importer,
				layoutTargetTable,
				delegate(LayoutTarget value) { thisDestination.layout.Target = value; },
				"val",
				LayoutTarget.Outer
				);
		}
		#endregion
	}
	#endregion
}
