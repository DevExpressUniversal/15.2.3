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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public class SurfaceDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("thickness", OnThickness);
			result.Add("spPr", OnShapeProperties);
			result.Add("pictureOptions", OnPictureOptions);
			return result;
		}
		static SurfaceDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (SurfaceDestination)importer.PeekDestination();
		}
		#endregion
		readonly SurfaceOptions options;
		public SurfaceDestination(SpreadsheetMLBaseImporter importer, SurfaceOptions options)
			: base(importer) {
			this.options = options;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnThickness(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SurfaceOptions options = GetThis(importer).options;
			return new IntegerValueDestination(importer,
				delegate(int value) { options.Thickness = value; },
				"val", 0);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SurfaceOptions options = GetThis(importer).options;
			return new ChartShapePropertiesDestination(importer, options.ShapeProperties);
		}
		static Destination OnPictureOptions(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SurfaceOptions options = GetThis(importer).options;
			return new PictureOptionsDestination(importer, options.PictureOptions);
		}
		#endregion
	}
}
