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
using System.Globalization;
using System.Text;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ChartNumberReferenceDestination
	public class ChartNumberReferenceDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("f", OnFormula);
			result.Add("numCache", OnNumberCache);
			return result;
		}
		static ChartNumberReferenceDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartNumberReferenceDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly OpenXmlNumberReference reference;
		#endregion
		public ChartNumberReferenceDestination(SpreadsheetMLBaseImporter importer, OpenXmlNumberReference reference)
			: base(importer) {
			this.reference = reference;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			OpenXmlNumberReference reference = GetThis(importer).reference;
			return new StringValueTagDestination(importer, delegate(string value) { reference.FormulaBody = value; return true; });
		}
		static Destination OnNumberCache(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartNumberReferenceDestination thisDestination = GetThis(importer);
			return new ChartNumberCacheDestination(importer, thisDestination.reference);
		}
		#endregion
	}
	#endregion
	#region ChartNumberCacheDestination
	public class ChartNumberCacheDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("formatCode", OnFormatCode);
			result.Add("pt", OnPoint);
			return result;
		}
		static ChartNumberCacheDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartNumberCacheDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly OpenXmlNumberReference reference;
		#endregion
		public ChartNumberCacheDestination(SpreadsheetMLBaseImporter importer, OpenXmlNumberReference reference)
			: base(importer) {
			this.reference = reference;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnFormatCode(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			OpenXmlNumberReference reference = GetThis(importer).reference;
			return new StringValueTagDestination(importer, delegate(string value) { reference.FormatCode = value; return true; });
		}
		static Destination OnPoint(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			OpenXmlNumberReference reference = GetThis(importer).reference;
			NumberPoint point = new NumberPoint();
			point.FormatCode = reference.FormatCode;
			reference.Points.Add(point);
			return new ChartNumericPointDestination(importer, point);
		}
		#endregion
	}
	#endregion
	#region ChartNumericPointDestination
	public class ChartNumericPointDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("v", OnValue);
			return result;
		}
		static ChartNumericPointDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartNumericPointDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		NumberPoint point;
		#endregion
		public ChartNumericPointDestination(SpreadsheetMLBaseImporter importer, NumberPoint point)
			: base(importer) {
			this.point = point;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			int index = Importer.GetIntegerValue(reader, "idx", -1);
			if (index < 0)
				Importer.ThrowInvalidFile();
			point.Index = index;
			string formatCode = Importer.ReadAttribute(reader, "formatCode");
			if (!string.IsNullOrEmpty(formatCode))
				point.FormatCode = formatCode ;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnValue(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			NumberPoint point = GetThis(importer).point;
			return new StringValueTagDestination(importer, delegate(string value) {
				point.Value = value;
				return true;
			});
		}
		#endregion
	}
	#endregion
}
