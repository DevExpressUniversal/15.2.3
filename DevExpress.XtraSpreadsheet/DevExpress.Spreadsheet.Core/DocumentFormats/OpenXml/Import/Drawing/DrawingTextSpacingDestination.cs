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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DrawingTextSpacingDestination
	public class DrawingTextSpacingDestination : DrawingTextParagraphPropertiesDestinationBase {
		#region Static Members
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("spcPct", OnSpacingPercent);
			result.Add("spcPts", OnSpacingPoints);
			return result;
		}
		#endregion
		static DrawingTextSpacingDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DrawingTextSpacingDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnSpacingPercent(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextSpacingInfo info = GetThis(importer).info;
			info.Type = DrawingTextSpacingValueType.Percent;
			return new DrawingTextSpacingValueDestination(importer, info);
		}
		static Destination OnSpacingPoints(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextSpacingInfo info = GetThis(importer).info;
			info.Type = DrawingTextSpacingValueType.Points;
			return new DrawingTextSpacingValueDestination(importer, info);
		}
		#endregion
		#endregion
		readonly int index;
		readonly DrawingTextSpacingInfo info;
		public DrawingTextSpacingDestination(SpreadsheetMLBaseImporter importer, DrawingTextParagraphProperties properties, int index)
			: base(importer, properties) {
			this.index = index;
			this.info = new DrawingTextSpacingInfo();
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			int textSpacingInfoIndex = Properties.DrawingCache.DrawingTextSpacingInfoCache.AddItem(info);
			Properties.AssignTextSpacingInfoIndex(index, textSpacingInfoIndex);
		}
	}
	#endregion
	#region DrawingTextSpacingValueDestination
	public class DrawingTextSpacingValueDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly DrawingTextSpacingInfo info;
		public DrawingTextSpacingValueDestination(SpreadsheetMLBaseImporter importer, DrawingTextSpacingInfo info)
			: base(importer) {
			this.info = info;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int value = Importer.GetIntegerValue(reader, "val", Int32.MinValue);
			if (info.Type == DrawingTextSpacingValueType.Percent)
				DrawingValueChecker.CheckTextSpacingPercentValue(value);
			if (info.Type == DrawingTextSpacingValueType.Points)
				DrawingValueChecker.CheckTextSpacingPointsValue(value);
			info.Value = value;
		}
	}
	#endregion
}
