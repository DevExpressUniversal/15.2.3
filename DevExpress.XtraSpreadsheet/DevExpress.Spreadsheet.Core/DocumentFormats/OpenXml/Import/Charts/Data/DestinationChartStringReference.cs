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

using System.Collections.Generic;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ChartStringReferenceDestination
	public class ChartStringReferenceDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("f", OnFormula);
			result.Add("strCache", OnStringCache);
			return result;
		}
		static ChartStringReferenceDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartStringReferenceDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly OpenXmlStringReference reference;
		#endregion
		public ChartStringReferenceDestination(SpreadsheetMLBaseImporter importer, OpenXmlStringReference reference)
			: base(importer) {
			this.reference = reference;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (string.IsNullOrEmpty(reference.FormulaBody))
				Importer.ThrowInvalidFile();
		}
		#region Handlers
		static Destination OnFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			OpenXmlStringReference reference = GetThis(importer).reference;
			return new StringValueTagDestination(importer, delegate(string value) { reference.FormulaBody = value; return true; });
		}
		static Destination OnStringCache(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartStringReferenceDestination thisDestination = GetThis(importer);
			return new ChartStringCacheDestination(importer, thisDestination.reference);
		}
		#endregion
	}
	#endregion
	#region ChartStringCacheDestination
	public class ChartStringCacheDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pt", OnPoint);
			return result;
		}
		static ChartStringCacheDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartStringCacheDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly OpenXmlStringReference reference;
		#endregion
		public ChartStringCacheDestination(SpreadsheetMLBaseImporter importer, OpenXmlStringReference reference)
			: base(importer) {
			this.reference = reference;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
		}
		#region Handlers
		static Destination OnPoint(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			StringPoint point = new StringPoint();
			GetThis(importer).reference.Points.Add(point);
			return new ChartStringPointDestination(importer, point);
		}
		#endregion
	}
	#endregion
	#region ChartStringPointDestination
	public class ChartStringPointDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("v", OnValue);
			return result;
		}
		static ChartStringPointDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartStringPointDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		StringPoint point;
		#endregion
		public ChartStringPointDestination(SpreadsheetMLBaseImporter importer, StringPoint point)
			: base(importer) {
				this.point = point;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			point.Index = Importer.GetIntegerValue(reader, "idx", -1);
			if (point.Index < 0)
				Importer.ThrowInvalidFile();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
		}
		#region Handlers
		static Destination OnValue(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			StringPoint point = GetThis(importer).point;
			return new StringValueTagDestination(importer, delegate(string value) {
				point.Value = value;
				return true;
			});
		}
		#endregion
	}
	#endregion
}
