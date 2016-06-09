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
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region HyperlinksDestination
	public class HyperlinksDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("hyperlink", OnHyperlink);
			return result;
		}
		static Destination OnHyperlink(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new HyperlinkDestination(importer);
		}
		public HyperlinksDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region HyperlinkDestination
	public class HyperlinkDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public HyperlinkDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string reference = reader.GetAttribute("ref");
			if (String.IsNullOrEmpty(reference))
				return;
			string id = reader.GetAttribute("id", Importer.RelationsNamespace);
			if (!String.IsNullOrEmpty(id))
				ProcessExternalHyperlink(reader, reference, id);
			else
				ProcessInternalHyperlink(reader, reference);
		}
		void ProcessExternalHyperlink(XmlReader reader, string reference, string relationId) {
			OpenXmlRelation relation = ((OpenXmlImporter)Importer).DocumentRelations.LookupRelationById(relationId);
			if (relation == null)
				return;
			if (relation.Type == OpenXmlExporter.OfficeHyperlinkType &&
				StringExtensions.CompareInvariantCultureIgnoreCase(relation.TargetMode, "External") == 0) {
				string targetUri = relation.Target;
				string location = reader.GetAttribute("location");
				if(!string.IsNullOrEmpty(location))
					targetUri += "#" + location;
				CellRange range = CellRange.Create(Importer.CurrentWorksheet, reference);
				ModelHyperlink newHyperlink = Importer.CurrentWorksheet.CreateHyperlinkCoreFromImport(range, targetUri, true);
				Importer.CurrentWorksheet.Hyperlinks.Add(newHyperlink);
				ProcessHyperlinkCore(reader, newHyperlink);
			}
		}
		void ProcessInternalHyperlink(XmlReader reader, string reference) {
			string location = reader.GetAttribute("location"); 
			if (String.IsNullOrEmpty(location))
				return;
			CellRange range = CellRange.Create(Importer.CurrentWorksheet, reference);
			ModelHyperlink newHyperlink = Importer.CurrentWorksheet.CreateHyperlinkCoreFromImport(range, location, false);
			Importer.CurrentWorksheet.Hyperlinks.Add(newHyperlink);
			ProcessHyperlinkCore(reader, newHyperlink);
		}
		void ProcessHyperlinkCore(XmlReader reader, ModelHyperlink link) {
			string toolTip = reader.GetAttribute("tooltip");
			if (!String.IsNullOrEmpty(toolTip))
				link.TooltipText = toolTip;
			string display = reader.GetAttribute("display");
			if (!String.IsNullOrEmpty(display))
				link.DisplayText = display;
		}
	}
	#endregion
}
