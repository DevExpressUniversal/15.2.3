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
using System.Xml;
using DevExpress.Data;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.Snap.Core.API;
namespace DevExpress.Snap.Core.Import {
	#region FieldContextsDestionation
	public class FieldContextsDestionation : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("fieldContext", OnFieldContext);			
			return result;
		}
		Dictionary<int, FieldContextImportInfo> importedInfos;
		public FieldContextsDestionation(WordProcessingMLBaseImporter importer)
			: base(importer) {
			this.importedInfos = new Dictionary<int, FieldContextImportInfo>();
		}
		public Dictionary<int, FieldContextImportInfo> ImportedInfos { get { return importedInfos; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnFieldContext(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldContextDestination(importer, ((FieldContextsDestionation)importer.PeekDestination()).ImportedInfos);
		}		
	}
	#endregion
	public class FieldContextImportInfo {
		List<String> filters;
		List<GroupProperties> groups;
		public string Type { get; set; }
		public int Id { get; set; }
		public string Source { get; set; }
		public int VisibleIndex { get; set; }
		public int RowHandle { get; set; }
		public int ParentId { get; set; }
		public int CurrentIndexInGroup { get; set; }
		public string Path { get; set; }
		public List<String> Filters { get { return filters; } }
		public List<GroupProperties> Groups { get { return groups; } }
		public void AddFilter(string filterString) {
			if (this.filters == null)
				filters = new List<string>();
			filters.Add(filterString);			
		}
		public void AddGroup(GroupProperties groupProperties) {
			if (this.groups == null)
				groups = new List<GroupProperties>();
			groups.Add(groupProperties);
		}		
	}
	#region FieldContextDestination
	public class FieldContextDestination : ElementDestination {		
		readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		readonly FieldContextImportInfo importInfo = new FieldContextImportInfo();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("filters", OnFilters);
			result.Add("groups", OnGroups);
			return result;
		}
		protected internal new SnapImporter Importer { get { return (SnapImporter)base.Importer; } }
		Dictionary<int, FieldContextImportInfo> importedInfos;
		public FieldContextDestination(WordProcessingMLBaseImporter importer, Dictionary<int, FieldContextImportInfo> importedInfos)
			: base(importer) {
			this.importedInfos = importedInfos;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			importInfo.Type = Importer.ReadDxStringAttr("type", reader);
			importInfo.Id = Importer.ReadDxIntAttr("id", reader);
			importInfo.Source = Importer.ReadDxStringAttr("source", reader);
			importInfo.ParentId = Importer.ReadDxIntAttr("parentId", reader);
			importInfo.Path = Importer.ReadDxStringAttr("path", reader);
			importInfo.RowHandle = Importer.ReadDxIntAttr("rowHandle", reader);
			importInfo.VisibleIndex = Importer.ReadDxIntAttr("visibleIndex", reader);
			importInfo.CurrentIndexInGroup = Importer.ReadDxIntAttr("indexInGroup", reader);
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			importedInfos.Add(importInfo.Id, importInfo);
		}
		static Destination OnFilters(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldContextFiltersDestination((SnapImporter)importer, ((FieldContextDestination)importer.PeekDestination()).importInfo);
		}
		static Destination OnGroups(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldContextGroupsDestination((SnapImporter)importer, ((FieldContextDestination)importer.PeekDestination()).importInfo);
		}
	}
	#endregion
	#region FieldContextFiltersDestination
	public class FieldContextFiltersDestination : ElementDestination {
		readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();		
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("filter", OnFilter);
			return result;
		}
		readonly FieldContextImportInfo importInfo;
		public FieldContextFiltersDestination(WordProcessingMLBaseImporter importer, FieldContextImportInfo importInfo)
			: base(importer) {
				this.importInfo = importInfo;
		}
		protected internal new SnapImporter Importer { get { return (SnapImporter)base.Importer; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnFilter(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldContextFilterDestination((SnapImporter)importer, ((FieldContextFiltersDestination)importer.PeekDestination()).importInfo);
		}
	}
	#endregion
	#region FieldContextFilterDestination
	public class FieldContextFilterDestination : SnapLeafElementDestination {		
		readonly FieldContextImportInfo importInfo;
		protected internal new SnapImporter Importer { get { return (SnapImporter)base.Importer; } }
		public FieldContextFilterDestination(SnapImporter importer, FieldContextImportInfo importInfo)
			: base(importer) {
			this.importInfo = importInfo;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			importInfo.AddFilter(Importer.ReadDxStringAttr("filterstring", reader));
		}
	}
	#endregion
	#region FieldContextGroupsDestination
	public class FieldContextGroupsDestination : ElementDestination {
		readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("group", OnGroup);
			return result;
		}
		readonly FieldContextImportInfo importInfo;
		public FieldContextGroupsDestination(WordProcessingMLBaseImporter importer, FieldContextImportInfo importInfo)
			: base(importer) {
				this.importInfo = importInfo;
		}
		protected internal new SnapImporter Importer { get { return (SnapImporter)base.Importer; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnGroup(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldContextGroupDestination((SnapImporter)importer, ((FieldContextGroupsDestination)importer.PeekDestination()).importInfo);
		}
	}
	#endregion
	#region FieldContextFilterDestination
	public class FieldContextGroupDestination : ElementDestination {
		readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();	   
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("groupField", OnGroupField);
			return result;
		}
		readonly FieldContextImportInfo importInfo;
		readonly GroupProperties groupProperties;
		public FieldContextGroupDestination(SnapImporter importer, FieldContextImportInfo importInfo)
			: base(importer) {
			this.importInfo = importInfo;
			this.groupProperties = new GroupProperties();
		}
		protected internal new SnapImporter Importer { get { return (SnapImporter)base.Importer; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			groupProperties.TemplateFooterSwitch = Importer.ReadDxStringAttr("tf", reader);
			groupProperties.TemplateHeaderSwitch = Importer.ReadDxStringAttr("th", reader);			
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			importInfo.AddGroup(groupProperties);
		}
		static Destination OnGroupField(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldContextGroupFieldDestination((SnapImporter)importer, ((FieldContextGroupDestination)importer.PeekDestination()).groupProperties);
		}
	}
	#endregion
	#region FieldContextGroupFieldDestination
	public class FieldContextGroupFieldDestination : SnapLeafElementDestination {
		readonly GroupProperties groupProperties;
		public FieldContextGroupFieldDestination(SnapImporter importer, GroupProperties groupProperties)
			: base(importer) {
			this.groupProperties = groupProperties;
		}
		protected internal new SnapImporter Importer { get { return (SnapImporter)base.Importer; } }
		public override void ProcessElementOpen(XmlReader reader) {
			GroupFieldInfo fieldInfo = new GroupFieldInfo();
			fieldInfo.FieldName = Importer.ReadDxStringAttr("name", reader);
			fieldInfo.SortOrder = (ColumnSortOrder)Enum.Parse(typeof(ColumnSortOrder), Importer.ReadDxStringAttr("sortOrder", reader));
			fieldInfo.GroupInterval = (GroupInterval)Enum.Parse(typeof(GroupInterval), Importer.ReadDxStringAttr("groupInterval", reader));
			groupProperties.GroupFieldInfos.Add(fieldInfo);
		}
	}
	#endregion
}
