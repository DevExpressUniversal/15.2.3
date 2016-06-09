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

using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ImportPivotCacheSourceInfo
	public class ImportPivotCacheSourceInfo {
		#region Properties
		public PivotCacheType CacheType { get; set; }
		public ImportPivotCacheWorksheetReference WorksheetInfo { get; set; }
		public PivotCacheSourceConsolidation Consolidation { get; set; }
		public int? ConnectionId { get; set; }
		public bool HasWorksheetInfo { get { return CacheType == PivotCacheType.Worksheet && WorksheetInfo != null; } }
		public bool HasConsolidationInfo { get { return CacheType == PivotCacheType.Consolidation && Consolidation != null; } }
		#endregion
		public IPivotCacheSource CreateSource(OpenXmlImporter importer) {
			Guard.ArgumentNotNull(importer, "importer");
			if (HasWorksheetInfo)
				return WorksheetInfo.CreatePivotCacheSourceWorksheet(importer);
			if (HasConsolidationInfo)
				return Consolidation;
			if (CacheType == PivotCacheType.External)
				return CreateSourceExternal(importer); 
			if (CacheType == PivotCacheType.Scenario)
				return CreateSourceScenario(importer); 
			throw new ArgumentException("Invalid PivotCacheType: " + CacheType.ToString());
		}
		PivotCacheSourceExternal CreateSourceExternal(OpenXmlImporter importer) {
			PivotCacheSourceExternal result = new PivotCacheSourceExternal();
			if (!ConnectionId.HasValue)
				importer.ThrowInvalidFile();
			result.ConnectionId = ConnectionId.Value; 
			return result;
		}
		PivotCacheSourceScenario CreateSourceScenario(OpenXmlImporter importer) {
			PivotCacheSourceScenario result = new PivotCacheSourceScenario();
			return result;
		}
	}
	#endregion
	#region ImportPivotCacheWorksheetReference
	public class ImportPivotCacheWorksheetReference {
		#region Properties
		public string RelationId { get; set; }
		public string SheetName { get; set; }
		public string NamedRange { get; set; }
		public string Reference { get; set; }
		public bool Invalid {
			get {
				bool emptyNamedRange = String.IsNullOrEmpty(NamedRange);
				bool emptyReference = String.IsNullOrEmpty(Reference);
				return (emptyNamedRange && emptyReference) || (!emptyNamedRange && !emptyReference);
			}
		}
		#endregion
		public PivotCacheSourceWorksheet CreatePivotCacheSourceWorksheet(OpenXmlImporter importer) {
			string bookFileName = importer.TryGetPivotCacheExternalLinkPath(RelationId, importer.ActualPivotCacheDefinitionId);
			return PivotCacheSourceWorksheetHelper.GetSource(SheetName, NamedRange, Reference, bookFileName, importer.DocumentModel.DataContext);
		}
		public PivotCacheSourceConsolidationRangeSet CreatePivotCacheSourceConsolidationRangeSet(OpenXmlImporter importer, int index1, int index2, int index3, int index4) {
			PivotCacheSourceWorksheet reference = CreatePivotCacheSourceWorksheet(importer);
			return new PivotCacheSourceConsolidationRangeSet(reference, index1, index2, index3, index4);
		}
		public void ReadFrom(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Reference = importer.ReadAttribute(reader, "ref");
			NamedRange = importer.ReadAttribute(reader, "name");
			SheetName = importer.ReadAttribute(reader, "sheet");
			RelationId = reader.GetAttribute("id", importer.RelationsNamespace);
			if (Invalid)
				importer.ThrowInvalidFile();
		}
	}
	#endregion
	#region PivotCacheSourceDestination
	public class PivotCacheSourceDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("worksheetSource", OnWorksheetSource);
			result.Add("consolidation", OnConsolidation);
			return result;
		}
		static Destination OnWorksheetSource(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ImportPivotCacheSourceInfo sourceInfo = GetThis(importer).info;
			if (sourceInfo.HasConsolidationInfo)
				importer.ThrowInvalidFile();
			ImportPivotCacheWorksheetReference info = new ImportPivotCacheWorksheetReference();
			sourceInfo.WorksheetInfo = info;
			return new PivotCacheSourceWorksheetDestination(importer, info);
		}
		static Destination OnConsolidation(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ImportPivotCacheSourceInfo sourceInfo = GetThis(importer).info;
			if (sourceInfo.HasWorksheetInfo)
				importer.ThrowInvalidFile();
			PivotCacheSourceConsolidation info = new PivotCacheSourceConsolidation();
			sourceInfo.Consolidation = info;
			return new PivotCacheSourceConsolidationDestination(importer, info);
		}
		static PivotCacheSourceDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheSourceDestination)importer.PeekDestination();
		}
		#endregion
		readonly ImportPivotCacheSourceInfo info;
		public PivotCacheSourceDestination(SpreadsheetMLBaseImporter importer, ImportPivotCacheSourceInfo info)
			: base(importer) {
			Guard.ArgumentNotNull(info, "info");
			this.info = info;
		}
		#region Properties
		protected internal ImportPivotCacheSourceInfo Info { get { return info; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			PivotCacheType? type = Importer.GetWpEnumOnOffNullValue(reader, "type", OpenXmlExporter.PivotCacheTypeTable);
			if (!type.HasValue)
				Importer.ThrowInvalidFile();
			info.CacheType = type.Value;
			info.ConnectionId = Importer.GetIntegerNullableValue(reader, "connectionId");
		}
	}
	#endregion
	#region PivotCacheSourceWorksheetDestination
	public class PivotCacheSourceWorksheetDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly ImportPivotCacheWorksheetReference info;
		public PivotCacheSourceWorksheetDestination(SpreadsheetMLBaseImporter importer, ImportPivotCacheWorksheetReference info)
			: base(importer) {
			Guard.ArgumentNotNull(info, "info");
			this.info = info;
		}
		protected internal ImportPivotCacheWorksheetReference Info { get { return info; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			info.ReadFrom(Importer, reader);
		}
	}
	#endregion
	#region PivotCacheSourceConsolidationDestination
	public class PivotCacheSourceConsolidationDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pages", OnPages);
			result.Add("rangeSets", OnRangeSets);
			return result;
		}
		static Destination OnPages(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheSourceConsolidationPagesDestination(importer, GetThis(importer).source.Pages);
		}
		static Destination OnRangeSets(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheSourceConsolidationRangeSetsDestination(importer, GetThis(importer).source.RangeSets);
		}
		static PivotCacheSourceConsolidationDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheSourceConsolidationDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheSourceConsolidation source;
		public PivotCacheSourceConsolidationDestination(SpreadsheetMLBaseImporter importer, PivotCacheSourceConsolidation source)
			: base(importer) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
		}
		#region Properties
		protected internal PivotCacheSourceConsolidation Source { get { return source; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			source.AutoPage = Importer.GetOnOffValue(reader, "autoPage", true);
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (source.RangeSets.Count == 0)
				Importer.ThrowInvalidFile();
		}
	}
	#endregion
	#region PivotCacheSourceConsolidationPagesDestination
	public class PivotCacheSourceConsolidationPagesDestination : ElementDestination<SpreadsheetMLBaseImporter> { 
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("page", OnPage);
			return result;
		}
		static Destination OnPage(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			IList<PivotCacheSourceConsolidationPage> pages = GetThis(importer).pages;
			PivotCacheSourceConsolidationPage currentPage = new PivotCacheSourceConsolidationPage();
			pages.Add(currentPage);
			return new PivotCacheSourceConsolidationPageDestination(importer, currentPage);
		}
		static PivotCacheSourceConsolidationPagesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheSourceConsolidationPagesDestination)importer.PeekDestination();
		}
		#endregion
		readonly IList<PivotCacheSourceConsolidationPage> pages;
		public PivotCacheSourceConsolidationPagesDestination(SpreadsheetMLBaseImporter importer, IList<PivotCacheSourceConsolidationPage> pages)
			: base(importer) {
			Guard.ArgumentNotNull(pages, "pages");
			this.pages = pages;
		}
		#region Properties
		protected internal IList<PivotCacheSourceConsolidationPage> Pages { get { return pages; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (pages.Count < 1 || pages.Count > 4)
				Importer.ThrowInvalidFile();
		}
	}
	#endregion
	#region PivotCacheSourceConsolidationPageDestination
	public class PivotCacheSourceConsolidationPageDestination : ElementDestination<SpreadsheetMLBaseImporter> { 
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pageItem", OnPageItem);
			return result;
		}
		static Destination OnPageItem(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheSourceConsolidationPageItemDestination(importer, GetThis(importer).page);
		}
		static PivotCacheSourceConsolidationPageDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheSourceConsolidationPageDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheSourceConsolidationPage page;
		public PivotCacheSourceConsolidationPageDestination(SpreadsheetMLBaseImporter importer, PivotCacheSourceConsolidationPage page)
			: base(importer) {
			Guard.ArgumentNotNull(page, "page");
			this.page = page;
		}
		#region Properties
		protected internal PivotCacheSourceConsolidationPage Page { get { return page; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
	}
	#endregion
	#region PivotCacheSourceConsolidationPageItemDestination
	public class PivotCacheSourceConsolidationPageItemDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotCacheSourceConsolidationPage page;
		public PivotCacheSourceConsolidationPageItemDestination(SpreadsheetMLBaseImporter importer, PivotCacheSourceConsolidationPage page)
			: base(importer) {
			Guard.ArgumentNotNull(page, "page");
			this.page = page;
		}
		protected internal PivotCacheSourceConsolidationPage Page { get { return page; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string name = reader.GetAttribute("name");
			if (name == null)
				Importer.ThrowInvalidFile();
			page.ItemNames.Add(name);
		}
	}
	#endregion
	#region PivotCacheSourceConsolidationRangeSetsDestination
	public class PivotCacheSourceConsolidationRangeSetsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("rangeSet", OnRangeSet);
			return result;
		}
		static Destination OnRangeSet(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheSourceConsolidationRangeSetDestination(importer, GetThis(importer).rangeSets);
		}
		static PivotCacheSourceConsolidationRangeSetsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheSourceConsolidationRangeSetsDestination)importer.PeekDestination();
		}
		#endregion
		readonly IList<PivotCacheSourceConsolidationRangeSet> rangeSets;
		public PivotCacheSourceConsolidationRangeSetsDestination(SpreadsheetMLBaseImporter importer, IList<PivotCacheSourceConsolidationRangeSet> rangeSets)
			: base(importer) {
			Guard.ArgumentNotNull(rangeSets, "rangeSets");
			this.rangeSets = rangeSets;
		}
		#region Properties
		protected internal IList<PivotCacheSourceConsolidationRangeSet> RangeSets { get { return rangeSets; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
	}
	#endregion
	#region PivotCacheSourceConsolidationRangeSetDestination
	public class PivotCacheSourceConsolidationRangeSetDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly IList<PivotCacheSourceConsolidationRangeSet> rangeSets;
		public PivotCacheSourceConsolidationRangeSetDestination(SpreadsheetMLBaseImporter importer, IList<PivotCacheSourceConsolidationRangeSet> rangeSets)
			: base(importer) {
			Guard.ArgumentNotNull(rangeSets, "rangeSets");
			this.rangeSets = rangeSets;
		}
		protected internal IList<PivotCacheSourceConsolidationRangeSet> RangeSets { get { return rangeSets; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			ImportPivotCacheWorksheetReference reference = new ImportPivotCacheWorksheetReference();
			reference.ReadFrom(Importer, reader);
			int index1 = Importer.GetIntegerValue(reader, "i1", -1);
			int index2 = Importer.GetIntegerValue(reader, "i2", -1);
			int index3 = Importer.GetIntegerValue(reader, "i3", -1);
			int index4 = Importer.GetIntegerValue(reader, "i4", -1);
			PivotCacheSourceConsolidationRangeSet rangeSet = reference.CreatePivotCacheSourceConsolidationRangeSet(Importer as OpenXmlImporter, index1, index2, index3, index4);
			rangeSets.Add(rangeSet);
		}
	}
	#endregion
}
