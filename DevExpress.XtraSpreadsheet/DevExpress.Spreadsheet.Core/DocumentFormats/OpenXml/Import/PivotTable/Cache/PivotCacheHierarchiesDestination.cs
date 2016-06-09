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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PivotCacheHierarchiesDestination
	public class PivotCacheHierarchiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cacheHierarchy", OnCacheHierarchy);
			return result;
		}
		static Destination OnCacheHierarchy(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheHierarchyCollection hierarchies = GetThis(importer).hierarchies;
			PivotCacheHierarchy hierarchy = new PivotCacheHierarchy();
			hierarchies.Add(hierarchy);
			return new PivotCacheHierarchyDestination(importer, hierarchy);
		}
		static PivotCacheHierarchiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheHierarchiesDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheHierarchyCollection hierarchies;
		public PivotCacheHierarchiesDestination(SpreadsheetMLBaseImporter importer, PivotCacheHierarchyCollection hierarchies)
			: base(importer) {
			this.hierarchies = hierarchies;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotCacheHierarchyDestination
	public class PivotCacheHierarchyDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("fieldsUsage", OnFieldsUsage);
			result.Add("groupLevels", OnGroupLevels);
			return result;
		}
		static Destination OnFieldsUsage(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheHierarchy hierarchy = GetThis(importer).hierarchy;
			PivotCacheFieldUsageCollection fieldsUsage = new PivotCacheFieldUsageCollection();
			hierarchy.FieldsUsage = fieldsUsage;
			return new PivotCacheFieldsUsageDestination(importer, fieldsUsage);
		}
		static Destination OnGroupLevels(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheHierarchy hierarchy = GetThis(importer).hierarchy;
			PivotCacheGroupingLevelCollection levels = new PivotCacheGroupingLevelCollection();
			hierarchy.GroupingLevels = levels;
			return new PivotCacheGroupingLevelsDestination(importer, levels);
		}
		static PivotCacheHierarchyDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheHierarchyDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheHierarchy hierarchy;
		public PivotCacheHierarchyDestination(SpreadsheetMLBaseImporter importer, PivotCacheHierarchy hierarchy)
			: base(importer) {
			this.hierarchy = hierarchy;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			hierarchy.UniqueName = Importer.GetWpSTXString(reader, "uniqueName");
			if (string.IsNullOrEmpty(hierarchy.UniqueName))
				Importer.ThrowInvalidFile("PivotCacheHierarchy has no uniqueName.");
			hierarchy.Caption = Importer.GetWpSTXString(reader, "caption");
			hierarchy.IsMeasure = Importer.GetOnOffValue(reader, "measure", false);
			hierarchy.IsSet = Importer.GetOnOffValue(reader, "set", false);
			hierarchy.ParentSet = Importer.GetIntegerValue(reader, "parentSet", -1);
			hierarchy.KpiIconSet = Importer.GetIntegerValue(reader, "iconSet", 0);
			hierarchy.Attribute = Importer.GetOnOffValue(reader, "attribute", false);
			hierarchy.Time = Importer.GetOnOffValue(reader, "time", false);
			hierarchy.IsKeyAttribute = Importer.GetOnOffValue(reader, "keyAttribute", false);
			hierarchy.DefaultMemberUniqueName = Importer.GetWpSTXString(reader, "defaultMemberUniqueName");
			hierarchy.AllUniqueName = Importer.GetWpSTXString(reader, "allUniqueName");
			hierarchy.AllCaption = Importer.GetWpSTXString(reader, "allCaption");
			hierarchy.DimensionUniqueName = Importer.GetWpSTXString(reader, "dimensionUniqueName");
			hierarchy.DisplayFolder = Importer.GetWpSTXString(reader, "displayFolder");
			hierarchy.MeasureGroupName = Importer.GetWpSTXString(reader, "measureGroup");
			hierarchy.Measures = Importer.GetOnOffValue(reader, "measures", false);
			hierarchy.OneField = Importer.GetOnOffValue(reader, "oneField", false);
			hierarchy.MemberValueDataType = Importer.GetIntegerValue(reader, "memberValueDatatype", -1);
			hierarchy.Unbalanced = Importer.GetOnOffValue(reader, "unbalanced", false);
			hierarchy.UnbalancedGroup = Importer.GetOnOffValue(reader, "unbalancedGroup", false);
			hierarchy.Hidden = Importer.GetOnOffValue(reader, "hidden", false);
		}
	}
	#endregion
	#region PivotCacheFieldsUsageDestination
	public class PivotCacheFieldsUsageDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("fieldUsage", OnFieldUsage);
			return result;
		}
		static Destination OnFieldUsage(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheFieldUsageDestination(importer, GetThis(importer).fieldsUsage);
		}
		static PivotCacheFieldsUsageDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheFieldsUsageDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheFieldUsageCollection fieldsUsage;
		public PivotCacheFieldsUsageDestination(SpreadsheetMLBaseImporter importer, PivotCacheFieldUsageCollection fieldsUsage)
			: base(importer) {
			this.fieldsUsage = fieldsUsage;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotCacheFieldUsageDestination
	public class PivotCacheFieldUsageDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotCacheFieldUsageCollection fieldsUsage;
		public PivotCacheFieldUsageDestination(SpreadsheetMLBaseImporter importer, PivotCacheFieldUsageCollection fieldsUsage)
			: base(importer) {
			this.fieldsUsage = fieldsUsage;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int fieldIndex = Importer.GetIntegerValue(reader, "x");
			if (fieldIndex == int.MinValue)
				Importer.ThrowInvalidFile("PivotCacheFieldUsage has no field index.");
			fieldsUsage.Add(fieldIndex);
		}
	}
	#endregion
	#region PivotCacheGroupingLevelsDestination
	public class PivotCacheGroupingLevelsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("groupLevel", OnGroupLevel);
			return result;
		}
		static Destination OnGroupLevel(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheGroupingLevelCollection levels = GetThis(importer).collection;
			PivotCacheGroupingLevel level = new PivotCacheGroupingLevel();
			levels.Add(level);
			return new PivotCacheGroupingLevelDestination(importer, level);
		}
		static PivotCacheGroupingLevelsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheGroupingLevelsDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheGroupingLevelCollection collection;
		public PivotCacheGroupingLevelsDestination(SpreadsheetMLBaseImporter importer, PivotCacheGroupingLevelCollection collection)
			: base(importer) {
			this.collection = collection;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotCacheGroupingLevelDestination
	public class PivotCacheGroupingLevelDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("groups", OnGroups);
			return result;
		}
		static Destination OnGroups(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheGroupingLevel level = GetThis(importer).level;
			PivotCacheGroupCollection groups = new PivotCacheGroupCollection();
			level.Groups = groups;
			return new PivotCacheGroupsDestination(importer, groups);
		}
		static PivotCacheGroupingLevelDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheGroupingLevelDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheGroupingLevel level;
		public PivotCacheGroupingLevelDestination(SpreadsheetMLBaseImporter importer, PivotCacheGroupingLevel level)
			: base(importer) {
			this.level = level;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			level.UniqueName = Importer.GetWpSTXString(reader, "uniqueName");
			level.Caption = Importer.GetWpSTXString(reader, "caption");
			if (string.IsNullOrEmpty(level.UniqueName) || string.IsNullOrEmpty(level.Caption))
				Importer.ThrowInvalidFile("PivotCacheGroupingLevel has no uniqueName or caption.");
			level.User = Importer.GetOnOffValue(reader, "user", false);
			level.CustomRollUp = Importer.GetOnOffValue(reader, "customRollUp", false);
		}
	}
	#endregion
	#region PivotCacheGroupsDestination
	public class PivotCacheGroupsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("group", OnGroup);
			return result;
		}
		static Destination OnGroup(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheGroupCollection groups = GetThis(importer).groups;
			PivotCacheGroup group = new PivotCacheGroup();
			groups.Add(group);
			return new PivotCacheGroupDestination(importer, group);
		}
		static PivotCacheGroupsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheGroupsDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheGroupCollection groups;
		public PivotCacheGroupsDestination(SpreadsheetMLBaseImporter importer, PivotCacheGroupCollection groups)
			: base(importer) {
			this.groups = groups;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotCacheGroupDestination
	public class PivotCacheGroupDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("groupMembers", OnGroupMembers);
			return result;
		}
		static Destination OnGroupMembers(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheGroupMembersDestination(importer, GetThis(importer).group.Members);
		}
		static PivotCacheGroupDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheGroupDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheGroup group;
		public PivotCacheGroupDestination(SpreadsheetMLBaseImporter importer, PivotCacheGroup group)
			: base(importer) {
			this.group = group;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			group.Name = Importer.GetWpSTXString(reader, "name");
			group.UniqueName = Importer.GetWpSTXString(reader, "uniqueName");
			group.Caption = Importer.GetWpSTXString(reader, "caption");
			if (string.IsNullOrEmpty(group.Name) || string.IsNullOrEmpty(group.UniqueName) || string.IsNullOrEmpty(group.Caption))
				Importer.ThrowInvalidFile("PivotCacheGroup has no name, uniqueName or caption.");
			group.UniqueParent = Importer.GetWpSTXString(reader, "uniqueParent");
			group.Id = Importer.GetIntegerValue(reader, "id", int.MinValue);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (group.Members == null | group.Members.Count == 0)
				Importer.ThrowInvalidFile("PivtoCacheGroup has no members.");
		}
	}
	#endregion
	#region PivotCacheGroupMembersDestination
	public class PivotCacheGroupMembersDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("groupMember", OnGroupMember);
			return result;
		}
		static Destination OnGroupMember(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheGroupMemberCollection members = GetThis(importer).members;
			PivotCacheGroupMember member = new PivotCacheGroupMember();
			members.Add(member);
			return new PivotCacheGroupMemberDestination(importer, member);
		}
		static PivotCacheGroupMembersDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheGroupMembersDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheGroupMemberCollection members ;
		public PivotCacheGroupMembersDestination(SpreadsheetMLBaseImporter importer, PivotCacheGroupMemberCollection members)
			: base(importer) {
			this.members = members;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotCacheGroupMemberDestination
	public class PivotCacheGroupMemberDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotCacheGroupMember member;
		public PivotCacheGroupMemberDestination(SpreadsheetMLBaseImporter importer, PivotCacheGroupMember member)
			: base(importer) {
			this.member = member;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			member.UniqueName = Importer.GetWpSTXString(reader, "uniqueName");
			if (string.IsNullOrEmpty(member.UniqueName))
				Importer.ThrowInvalidFile("PivotCacheGroupMember has no uniqueName.");
			member.Group = Importer.GetOnOffValue(reader, "group", false);
		}
	}
	#endregion
	#region PivotCacheDimensionsDestination
	public class PivotCacheDimensionsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("dimension", OnCacheDimension);
			return result;
		}
		static Destination OnCacheDimension(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheDimensionCollection dimensions = GetThis(importer).dimensions;
			PivotCacheDimension dimension = new PivotCacheDimension();
			dimensions.Add(dimension);
			return new PivotCacheDimensionDestination(importer, dimension);
		}
		static PivotCacheDimensionsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheDimensionsDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheDimensionCollection dimensions;
		public PivotCacheDimensionsDestination(SpreadsheetMLBaseImporter importer, PivotCacheDimensionCollection dimensions)
			: base(importer) {
			this.dimensions = dimensions;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotCacheDimensionDestination
	public class PivotCacheDimensionDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotCacheDimension dimension;
		public PivotCacheDimensionDestination(SpreadsheetMLBaseImporter importer, PivotCacheDimension dimension)
			: base(importer) {
			this.dimension = dimension;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			dimension.Measure = Importer.GetOnOffValue(reader, "measure", false);
			dimension.Name = Importer.GetWpSTXString(reader, "name");
			dimension.UniqueName = Importer.GetWpSTXString(reader, "uniqueName");
			dimension.Caption = Importer.GetWpSTXString(reader, "caption");
			if (string.IsNullOrEmpty(dimension.Name) || string.IsNullOrEmpty(dimension.UniqueName) || string.IsNullOrEmpty(dimension.Caption))
				Importer.ThrowInvalidFile("PivotCacheDimension has no name, uniqueName or caption.");
		}
	}
	#endregion
	#region PivotCacheKpisDestination
	public class PivotCacheKpisDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("kpi", OnCacheKpi);
			return result;
		}
		static Destination OnCacheKpi(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheKpiCollection kpis = GetThis(importer).kpis;
			PivotCacheKpi kpi = new PivotCacheKpi();
			kpis.Add(kpi);
			return new PivotCacheKpiDestination(importer, kpi);
		}
		static PivotCacheKpisDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheKpisDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheKpiCollection kpis;
		public PivotCacheKpisDestination(SpreadsheetMLBaseImporter importer, PivotCacheKpiCollection kpis)
			: base(importer) {
			this.kpis = kpis;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotCacheKpiDestination
	public class PivotCacheKpiDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotCacheKpi kpi;
		public PivotCacheKpiDestination(SpreadsheetMLBaseImporter importer, PivotCacheKpi kpi)
			: base(importer) {
			this.kpi = kpi;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			kpi.UniqueName = Importer.GetWpSTXString(reader, "uniqueName");
			kpi.Caption = Importer.GetWpSTXString(reader, "caption");
			kpi.DisplayFolder = Importer.GetWpSTXString(reader, "displayFolder");
			kpi.MeasureGroupName = Importer.GetWpSTXString(reader, "measureGroup");
			kpi.Parent = Importer.GetWpSTXString(reader, "parent");
			kpi.ValueUniqueName = Importer.GetWpSTXString(reader, "value");
			if (string.IsNullOrEmpty(kpi.UniqueName) || string.IsNullOrEmpty(kpi.ValueUniqueName))
				Importer.ThrowInvalidFile("PivotCacheKpi has no uniqueName or value.");
			kpi.GoalUniqueName= Importer.GetWpSTXString(reader, "goal");
			kpi.StatusUniqueName = Importer.GetWpSTXString(reader, "status");
			kpi.TrendUniqueName = Importer.GetWpSTXString(reader, "trend");
			kpi.WeightUniqueName = Importer.GetWpSTXString(reader, "weight");
			kpi.TimeMemberUniqueName = Importer.GetWpSTXString(reader, "time");
		}
	}
	#endregion
	#region PivotCacheDimensionMapsDestination
	public class PivotCacheDimensionMapsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("map", OnMap);
			return result;
		}
		static Destination OnMap(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheDimensionMapCollection maps = GetThis(importer).maps;
			PivotCacheDimensionMap map = new PivotCacheDimensionMap();
			maps.Add(map);
			return new PivotCacheDimensionMapDestination(importer, map);
		}
		static PivotCacheDimensionMapsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheDimensionMapsDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheDimensionMapCollection maps;
		public PivotCacheDimensionMapsDestination(SpreadsheetMLBaseImporter importer, PivotCacheDimensionMapCollection maps)
			: base(importer) {
			this.maps = maps;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotCacheDimensionMapDestination
	public class PivotCacheDimensionMapDestination: LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotCacheDimensionMap map;
		public PivotCacheDimensionMapDestination(SpreadsheetMLBaseImporter importer, PivotCacheDimensionMap map)
			: base(importer) {
			this.map = map;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			map.MeasureGroup = Importer.GetIntegerValue(reader, "measureGroup", int.MinValue);
			map.Dimension = Importer.GetIntegerValue(reader, "dimension", int.MinValue);
		}
	}
	#endregion
	#region PivotCacheMeasureGroupsDestination
	public class PivotCacheMeasureGroupsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("measureGroup", OnMeasureGroup);
			return result;
		}
		static Destination OnMeasureGroup(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheMeasureGroupCollection groups = GetThis(importer).groups;
			PivotCacheMeasureGroup group = new PivotCacheMeasureGroup();
			groups.Add(group);
			return new PivotCacheMeasureGroupDestination(importer, group);
		}
		static PivotCacheMeasureGroupsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheMeasureGroupsDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheMeasureGroupCollection groups;
		public PivotCacheMeasureGroupsDestination(SpreadsheetMLBaseImporter importer, PivotCacheMeasureGroupCollection groups)
			: base(importer) {
			this.groups = groups;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotCacheMeasureGroupDestination
	public class PivotCacheMeasureGroupDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotCacheMeasureGroup group;
		public PivotCacheMeasureGroupDestination(SpreadsheetMLBaseImporter importer, PivotCacheMeasureGroup group)
			: base(importer) {
			this.group = group;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			group.Name = Importer.GetWpSTXString(reader, "name");
			group.Caption = Importer.GetWpSTXString(reader, "caption");
			if (string.IsNullOrEmpty(group.Name) || string.IsNullOrEmpty(group.Caption))
				Importer.ThrowInvalidFile("PivotCacheMeasureGroup has no name or caption.");
		}
	}
	#endregion
}
