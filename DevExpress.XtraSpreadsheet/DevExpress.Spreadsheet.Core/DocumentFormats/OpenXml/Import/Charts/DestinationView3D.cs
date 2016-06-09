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
	public class View3DDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("rotX", OnXRotation);
			result.Add("rotY", OnYRotation);
			result.Add("hPercent", OnHeightPercent);
			result.Add("depthPercent", OnDepthPercent);
			result.Add("rAngAx", OnRightAngleAxes);
			result.Add("perspective", OnPerspective);
			return result;
		}
		static View3DDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (View3DDestination)importer.PeekDestination();
		}
		#endregion
		readonly View3DOptions options;
		public View3DDestination(SpreadsheetMLBaseImporter importer, View3DOptions options)
			: base(importer) {
			this.options = options;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnXRotation(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			View3DOptions options = GetThis(importer).options;
			return new IntegerValueDestination(importer,
				delegate(int value) { options.XRotation = value; },
				"val", 0);
		}
		static Destination OnYRotation(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			View3DOptions options = GetThis(importer).options;
			return new IntegerValueDestination(importer,
				delegate(int value) { options.YRotation = value; },
				"val", 0);
		}
		static Destination OnHeightPercent(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			View3DOptions options = GetThis(importer).options;
			options.AutoHeight = false;
			return new IntegerValueDestination(importer,
				delegate(int value) { options.HeightPercent = value; },
				"val", 100);
		}
		static Destination OnDepthPercent(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			View3DOptions options = GetThis(importer).options;
			return new IntegerValueDestination(importer,
				delegate(int value) { options.DepthPercent = value; },
				"val", 100);
		}
		static Destination OnRightAngleAxes(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			View3DOptions options = GetThis(importer).options;
			return new OnOffValueDestination(importer,
				delegate(bool value) { options.RightAngleAxes = value; },
				"val", true);
		}
		static Destination OnPerspective(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			View3DOptions options = GetThis(importer).options;
			return new IntegerValueDestination(importer,
				delegate(int value) { options.Perspective = value; },
				"val", 30);
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
