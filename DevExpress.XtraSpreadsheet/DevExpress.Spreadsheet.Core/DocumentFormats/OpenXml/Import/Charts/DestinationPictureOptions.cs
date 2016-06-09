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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public class PictureOptionsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static Dictionary<string, PictureFormat> pictureFormatTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.PictureFormatTable);
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("applyToEnd", OnApplyToEnd);
			result.Add("applyToFront", OnApplyToFront);
			result.Add("applyToSides", OnApplyToSides);
			result.Add("pictureFormat", OnPictureFormat);
			result.Add("pictureStackUnit", OnPictureStackUnit);
			return result;
		}
		static PictureOptionsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PictureOptionsDestination)importer.PeekDestination();
		}
		#endregion
		readonly PictureOptions options;
		public PictureOptionsDestination(SpreadsheetMLBaseImporter importer, PictureOptions options)
			: base(importer) {
			this.options = options;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnApplyToEnd(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PictureOptions options = GetThis(importer).options;
			return new OnOffValueDestination(importer,
				delegate(bool value) { options.ApplyToEnd = value; },
				"val", true);
		}
		static Destination OnApplyToFront(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PictureOptions options = GetThis(importer).options;
			return new OnOffValueDestination(importer,
				delegate(bool value) { options.ApplyToFront = value; },
				"val", true);
		}
		static Destination OnApplyToSides(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PictureOptions options = GetThis(importer).options;
			return new OnOffValueDestination(importer,
				delegate(bool value) { options.ApplyToSides = value; },
				"val", true);
		}
		static Destination OnPictureFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PictureOptions options = GetThis(importer).options;
			return new EnumValueDestination<PictureFormat>(importer, pictureFormatTable,
				delegate(PictureFormat value) { options.PictureFormat = value; },
				"val", PictureFormat.Stretch);
		}
		static Destination OnPictureStackUnit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PictureOptions options = GetThis(importer).options;
			return new FloatValueDestination(importer,
				delegate(float value) { options.PictureStackUnit = value; },
				"val");
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			this.options.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			this.options.EndUpdate();
		}
	}
}
