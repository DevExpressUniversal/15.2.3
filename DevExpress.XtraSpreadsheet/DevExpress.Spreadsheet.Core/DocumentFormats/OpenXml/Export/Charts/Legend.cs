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
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Static
		internal static Dictionary<LegendPosition, string> LegendPositionTable = CreateLegendPositionTable();
		static Dictionary<LegendPosition, string> CreateLegendPositionTable() {
			Dictionary<LegendPosition, string> result = new Dictionary<LegendPosition, string>();
			result.Add(LegendPosition.Bottom, "b");
			result.Add(LegendPosition.Left, "l");
			result.Add(LegendPosition.Right, "r");
			result.Add(LegendPosition.Top, "t");
			result.Add(LegendPosition.TopRight, "tr");
			return result;
		}
		#endregion
		protected internal void GenerateChartLegend(Legend legend) {
			if (!legend.Visible)
				return;
			WriteStartElement("legend", DrawingMLChartNamespace);
			try {
				GenerateLegendPosContent(legend.Position);
				foreach (LegendEntry entry in legend.Entries)
					GenerateLegendEntryContent(entry);
				GenerateChartLayoutContent(legend.Layout);
				GenerateChartSimpleBoolAttributeTag("overlay", legend.Overlay);
				GenerateChartShapeProperties(legend.ShapeProperties);
				GenerateTextPropertiesContent(legend.TextProperties);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateLegendPosContent(LegendPosition value) {
			WriteStartElement("legendPos", DrawingMLChartNamespace);
			try {
				WriteStringValue("val", LegendPositionTable[value]);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateLegendEntryContent(LegendEntry entry) {
			if (!entry.Delete && entry.TextProperties.IsDefault)
				return;
			WriteStartElement("legendEntry", DrawingMLChartNamespace);
			try {
				GenerateChartSimpleIntAttributeTag("idx", entry.Index);
				if (entry.Delete)
					GenerateChartSimpleBoolAttributeTag("delete", true);
				else
					GenerateTextPropertiesContent(entry.TextProperties);
			}
			finally {
				WriteEndElement();
			}
		}
	}
}
