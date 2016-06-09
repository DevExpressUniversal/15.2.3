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
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ChartTitleDestination
	public class ChartTitleDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tx", OnChartText);
			result.Add("layout", OnLayout);
			result.Add("overlay", OnOverlay);
			result.Add("spPr", OnShapeProperties);
			result.Add("txPr", OnTextProperties);
			return result;
		}
		static ChartTitleDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartTitleDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		TitleOptions title;
		#endregion
		public ChartTitleDestination(SpreadsheetMLBaseImporter importer, TitleOptions title)
			: base(importer) {
			this.title = title;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).title.ShapeProperties);
		}
		static Destination OnLayout(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new LayoutDestination(importer, GetThis(importer).title.Layout);
		}
		static Destination OnOverlay(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			TitleOptions title = GetThis(importer).title;
			return new OnOffValueDestination(importer,
				delegate(bool value) { title.SetOverlayCore(value); },
				"val",
				true);
		}
		static Destination OnChartText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			TitleOptions title = GetThis(importer).title;
			return new ChartTextDestination(importer, title.Parent, delegate(IChartText value) { title.Text = value; });
		}
		static Destination OnTextProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			TitleOptions title = GetThis(importer).title;
			return new TextPropertiesDestination(importer, title.TextProperties);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			this.title.Overlay = true;
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (this.title.Text.TextType == ChartTextType.None)
				this.title.Text = ChartText.Auto;
			base.ProcessElementClose(reader);
		}
	}
	#endregion
}
