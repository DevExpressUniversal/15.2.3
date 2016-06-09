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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DifferentialFormatsDestination
	public class DifferentialFormatsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("dxf", OnDifferentialFormat);
			return result;
		}
		static Destination OnDifferentialFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DifferentialFormatDestination(importer);
		}
		#endregion
		public DifferentialFormatsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region DifferentialFormatDestination
	public class DifferentialFormatDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("font", OnFont);
			result.Add("numFmt", OnNumberFormat);
			result.Add("fill", OnFill);
			result.Add("alignment", OnCellAlignment);
			result.Add("border", OnBorder);
			result.Add("protection", OnCellProtection);
			result.Add("extLst", OnFutureFeatureDataStorageArea);
			return result;
		}
		static DifferentialFormatDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DifferentialFormatDestination)importer.PeekDestination();
		}
		static Destination OnFont(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DifferentialFontDestination(importer, GetThis(importer).Format);
		}
		static Destination OnNumberFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DifferentialNumberFormatDestination(importer, GetThis(importer).Format);
		}
		static Destination OnFill(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DifferentialFillDestination(importer, GetThis(importer).Format);
		}
		static Destination OnCellAlignment(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DifferentialCellAlignmentDestination(importer, GetThis(importer).Format);
		}
		static Destination OnBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DifferentialBorderDestination(importer, GetThis(importer).Format);
		}
		static Destination OnCellProtection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DifferentialProtectionDestination(importer, GetThis(importer).Format);
		}
		static Destination OnFutureFeatureDataStorageArea(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		#endregion
		readonly DifferentialFormat format;
		public DifferentialFormatDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			this.format = new DifferentialFormat(Importer.DocumentModel);
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal DifferentialFormat Format { get { return format; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			Importer.StyleSheet.RegisterDifferentialFormat(Format);
		}
	}
	#endregion
	#region DifferentialNumberFormatDestination
	public class DifferentialNumberFormatDestination : NumberFormatDestination {
		readonly DifferentialFormat format;
		public DifferentialNumberFormatDestination(SpreadsheetMLBaseImporter importer, DifferentialFormat format)
			: base(importer) {
			this.format = format;
		}
		public override void ProcessElementClose(XmlReader reader) {
			format.FormatString = NumberFormatCode;
		}
	}
	#endregion
	#region DifferentialProtectionDestination
	public class DifferentialProtectionDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly DifferentialFormat differentialFormat;
		public DifferentialProtectionDestination(SpreadsheetMLBaseImporter importer, DifferentialFormat differentialFormat)
			: base(importer) {
			Guard.ArgumentNotNull(differentialFormat, "differentialFormat");
			this.differentialFormat = differentialFormat;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			differentialFormat.BeginUpdate();
			try {
				bool? isHidden = Importer.GetWpSTOnOffNullValue(reader, "hidden");
				if (isHidden.HasValue)
					differentialFormat.Protection.Hidden = isHidden.Value;
				bool? isLocked = Importer.GetWpSTOnOffNullValue(reader, "locked");
				if (isLocked.HasValue)
					differentialFormat.Protection.Locked = isLocked.Value;
			} finally {
				differentialFormat.EndUpdate();
			}
		}
	}
	#endregion
}
