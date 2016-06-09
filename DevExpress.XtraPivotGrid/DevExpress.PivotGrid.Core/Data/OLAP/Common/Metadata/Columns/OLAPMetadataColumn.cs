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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.IO;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data.PivotGrid;
using System.Globalization;
namespace DevExpress.PivotGrid.OLAP {
	public class UnboundSummaryLevelErrorMetadataColumn : OLAPMetadataColumn, IUnboundSummaryLevelMetadataColumn {
		public override bool IsMeasure { get { return true; } }
		bool IUnboundMetadataColumn.IsServer { get { return false; } }
		public UnboundSummaryLevelErrorMetadataColumn(Type dataType, OLAPHierarchy columnHierarchy, OLAPDataType olapDataType)
			: base(0, 0, dataType, null, columnHierarchy, null, null, null, olapDataType) {
		}
		public object EvaluateValue(MeasuresStorage storage) {
			return PivotSummaryValue.ErrorValue;
		}
		public PivotCellValue EvaluatePivotCellValue(MeasuresStorage storage) {
			return ErrorCell;
		}
		CriteriaOperator IUnboundSummaryLevelMetadataColumn.Criteria {
			get { return new OperandValue(PivotSummaryValue.ErrorValue); }
		}
		bool IUnboundMetadataColumn.UpdateCriteria(PivotGridFieldBase field) {
			return false;
		}
	}
	public class UnboundSummaryLevelMetadataColumn : OLAPMetadataColumn, IUnboundSummaryLevelMetadataColumn { 
		CriteriaOperator criteria;
		EvaluatorContextDescriptor descriptor;
		IDataSourceHelpersOwner<OLAPCubeColumn> currentOwner;
		ExpressionEvaluator expressionEvaluator;
		public override bool IsMeasure { get { return true; } }
		public CriteriaOperator Criteria {
			get { return criteria; }
			set {
				if(object.Equals(criteria, value))
					return;
				criteria = value;
				expressionEvaluator = null;
			}
		}
		public ExpressionEvaluator ExpressionEvaluator { get { return expressionEvaluator; } }
		public UnboundSummaryLevelMetadataColumn(Type dataType, OLAPHierarchy columnHierarchy, OLAPDataType olapDataType, CriteriaOperator criteria, EvaluatorContextDescriptor descriptor, IDataSourceHelpersOwner<OLAPCubeColumn> currentOwner)
			: base(0, 0, dataType, null, columnHierarchy, null, null, null, olapDataType) {
			this.criteria = criteria;
			this.descriptor = descriptor;
			this.currentOwner = currentOwner;
		}
		public object EvaluateValue(MeasuresStorage storage) {
			object value;
			try {
				value = expressionEvaluator.Evaluate(storage);
				if(value == null)
					return null;
				if(DataType != typeof(object))
					value = Convert.ChangeType(value, DataType, CultureInfo.CurrentCulture);
			} catch {
				value = null; 
			}
			if(value == null)
				return null;
			storage.SetFormattedValue(this, value, null, -1);
			return value;
		}
		public PivotCellValue EvaluatePivotCellValue(MeasuresStorage storage) {
			object value;
			try {
				value = expressionEvaluator.Evaluate(storage);
				if(value == null)
					return null;
				if(DataType != typeof(object))
					value = Convert.ChangeType(value, DataType, CultureInfo.CurrentCulture);
			} catch {
				value = null; 
			}
			if(value == null)
				return null;
			storage.SetFormattedValue(this, value, null, -1);
			return new PivotCellValue(value);
		}
		public void EnsureExpressionEvaluator() {
			expressionEvaluator = new ExpressionEvaluator(descriptor, criteria);
		}
		bool IUnboundMetadataColumn.UpdateCriteria(PivotGridFieldBase field) {
			CriteriaOperator newCriteria = currentOwner.PatchCriteria(CriteriaOperator.Parse(field.UnboundExpression));
			bool result = !object.ReferenceEquals(newCriteria, Criteria);
			Criteria = newCriteria;
			EnsureExpressionEvaluator();
			return result;
		}
		bool IUnboundMetadataColumn.IsServer { get { return false; } }
	}
	public class OLAPMetadataColumn : MetadataColumnBase, IOLAPLevel, IOLAPEditableMemberCollection {
		#region statics
		public static OLAPMetadataColumn CreateFromTypeCode(byte typeCode) {
			switch(typeCode) {
				case OLAPCubeColumnTypeCode:
					return new OLAPMetadataColumn();
				case OLAPKPIColumnTypeCode:
					return new OLAPKPIMetadataColumn();
				case OLAPKPIMeasureTypeCode:
					return new OLAPKPIMeasureMetadataColumn();
			}
			throw new ArgumentException("Unknown typeCode");
		}
		static OLAPDataType GetMemberValueType(Dictionary<string, OLAPDataType> dic) {
			OLAPDataType result;
			if(dic.TryGetValue(OlapProperty.MEMBERVALUE, out result))
				return result;
			return OLAPDataType.WChar;
		}
		#endregion
		public const int OLAPCubeColumnTypeCode = 0, OLAPKPIColumnTypeCode = 1, OLAPKPIMeasureTypeCode = 2;
		string totalMemberUniqueName;
		OLAPHierarchy hierarchy;
		OLAPHierarchy columnHierarchy;
		string defaultMemberName;
		OLAPDataType baseDataType;
		OLAPMember allMember;
		string drilldownColumn;
		string expression;
		int cardinality;
		bool allMembersLoaded;
		OLAPMemberCollection members;
		int level;
		Dictionary<OLAPMember, object> calculatedMembers;
		string defaultSortProperty;
		int keyCount;
		string formatString;
		Dictionary<string, OLAPDataType> properties;
		public override string Name { get { return columnHierarchy.Name; } }
		public string Dimension { get { return columnHierarchy.Dimension; } }
		public override string Caption { get { return columnHierarchy.Caption; } }
		public override string UniqueName { get { return columnHierarchy.UniqueName; } }
		public override Type SafeDataType { get { return HasCalculatedMembers ? DataType == typeof(string) ? DataType : typeof(object) : DataType; } }
		public int Cardinality { get { return cardinality; } }
		public new IOLAPMetadata Owner {
			get { return (IOLAPMetadata)base.Owner; }
			set { base.Owner = value; }
		}
		public int Level { get { return level; } }
		public virtual byte TypeCode { get { return OLAPCubeColumnTypeCode; } }
		public int NonTotalMembersCount { get { return allMember != null ? MembersCount - 2 : MembersCount - 1; } }
		public OLAPDataType BaseDataType { get { return baseDataType; } }
		public int CalculatedMembersCount {
			get { return calculatedMembers == null ? 0 : calculatedMembers.Count; }
		}
		public bool HasCalculatedMembers { get { return CalculatedMembersCount > 0; } }
		public OLAPHierarchy Hierarchy { get { return hierarchy; } }
		public OLAPHierarchy ColumnHierarchy { get { return columnHierarchy; } }
		public override bool IsMeasure { get { return Hierarchy.IsMeasure; } }
		public override string DisplayFolder { get { return (IsMeasure ? this.columnHierarchy : Hierarchy).DisplayFolder; } }
		public string DefaultMemberName { get { return defaultMemberName; } }
		public bool IsAggregatable { get { return !string.IsNullOrEmpty(AllMemberUniqueName); } }
		public string TotalMemberUniqueName { get { return totalMemberUniqueName; } }
		public string Expression { get { return expression; } }
		public string AllMemberUniqueName {
			get { return AllMember != null ? AllMember.UniqueName : string.Empty; }
		}
		public bool HasCustomDefaultMember {
			get { return !IsMeasure && DefaultMemberName != AllMemberUniqueName; }
		}
		public OLAPMember AllMember {
			get { return allMember; }
			set {
				if(value == allMember)
					return;
				if(value != null && string.IsNullOrEmpty(value.UniqueName))
					throw new OLAPException("Invalid AllMember");
				Members.Remove(allMember);
				allMember = value;
				if(allMember != null)
					Members.Add(allMember);
			}
		}
		int IQueryMemberCollection.Count { get { return members.Count; } }
		public int MembersCount { get { return members.Count; } }
		public new OLAPMetadataColumn ParentColumn { get { return (OLAPMetadataColumn)base.ParentColumn; } }
		public new OLAPMetadataColumn ChildColumn { get { return (OLAPMetadataColumn)base.ChildColumn; } }
		public string DrillDownColumn {
			get {
				if(!string.IsNullOrEmpty(drilldownColumn))
					return drilldownColumn;
				string columnName = IsMeasure ? UniqueName : Hierarchy.UniqueName;
				return columnName.Insert(1, "$");
			}
		}
		protected OLAPMemberCollection Members { get { return members; } }
		public OLAPMember this[string uniqueName] {
			get { return Members[uniqueName]; }
		}
		public bool AllMembersLoaded {
			get { return allMembersLoaded || cardinality != 0 && NonTotalMembersCount >= cardinality + CalculatedMembersCount; }
			set { allMembersLoaded = value; }
		}
		public string FormatString {  
			get { return formatString; }
		}
		public OLAPMetadataColumn() {
			this.members = new OLAPMemberCollection(this);
		}
		public string DefaultSortProperty {
			get { return defaultSortProperty; }
		}
		public int KeyCount {
			get { return keyCount; }
		}
		public Dictionary<string, OLAPDataType> Properties {
			get { return properties; }
		}
		public OLAPMetadataColumn(int level, int cardinality, Type dataType, MetadataColumnBase parentColumn,
			OLAPHierarchy columnHierarchy, string drilldownColumn, string defaultMemberName, OLAPHierarchy hierarchy, OLAPDataType olapDataType, string formatString, string expression) :
			this(level, cardinality, dataType, parentColumn, columnHierarchy, drilldownColumn, defaultMemberName, hierarchy, olapDataType) {
			this.formatString = formatString;
			this.expression = expression;
		}
		public OLAPMetadataColumn(int level, int cardinality, MetadataColumnBase parentColumn,
			OLAPHierarchy columnHierarchy, string drilldownColumn, string defaultMemberName, OLAPHierarchy hierarchy, string sortProperty, int keyCount, Dictionary<string, OLAPDataType> properties) :
			this(level, cardinality, OLAPDataTypeConverter.Convert(GetMemberValueType(properties)), parentColumn, columnHierarchy, drilldownColumn, defaultMemberName, hierarchy, GetMemberValueType(properties)) {
			if(columnHierarchy.Name != sortProperty && properties.ContainsKey(sortProperty))
				this.defaultSortProperty = sortProperty;
			this.keyCount = keyCount;
			this.properties = new Dictionary<string, OLAPDataType>(properties);
			this.properties.Remove(OlapProperty.MEMBERVALUE);
			this.properties.Remove("Name");
		}
		protected OLAPMetadataColumn(int level, int cardinality, Type dataType, MetadataColumnBase parentColumn,
			OLAPHierarchy columnHierarchy, string drilldownColumn, string defaultMemberName, OLAPHierarchy hierarchy, OLAPDataType olapDataType)
			: base(dataType, parentColumn) {
			this.level = level;
			this.members = new OLAPMemberCollection(this);
			this.columnHierarchy = columnHierarchy;
			this.drilldownColumn = drilldownColumn;
			this.defaultMemberName = defaultMemberName;
			this.hierarchy = hierarchy;
			this.baseDataType = olapDataType;
			this.cardinality = cardinality;
			totalMemberUniqueName = GetTotalMemberUniqueName();
			AddTotalMember();
		}
		public OLAPMember GetDefaultMember(OLAPCubeColumn parent) {
			if(IsMeasure)
				return null;
			if(!HasCustomDefaultMember)
				return AllMember;
			if(this[DefaultMemberName] == null)
				Owner.QueryMembers(parent, DefaultMemberName);
			return this[DefaultMemberName];
		}
		void AddTotalMember() {
			Members.Add(new OLAPVirtualMember(this, TotalMemberUniqueName));
		}
		public void EnsureColumnMembersLoaded(OLAPCubeColumn column) {
			if(!AllMembersLoaded)
				Owner.QueryMembers(column, null);
		}
		public List<string> GetMembersNames() {
			List<string> result = new List<string>(Members.UniqueNames);
			result.Remove(TotalMemberUniqueName);
			if(AllMember != null)
				result.Remove(AllMember.UniqueName);
			return result;
		}
		string GetTotalMemberUniqueName() {
			return Hierarchy + ".[" + Name + TotalMemberString;
		}
		public bool IsTotalMember(string memberUniqueName) {
			return memberUniqueName == TotalMemberUniqueName || AllMember != null && memberUniqueName == AllMemberUniqueName;
		}
		OLAPLevelType IOLAPLevel.LevelType {
			get { throw new NotImplementedException(); }
		}
		long IOLAPLevel.Cardinality {
			get { return Cardinality; }
		}
		int IOLAPLevel.LevelNumber {
			get { return Level; }
		}
		public List<QueryMember> GetQueryMembers() {
			List<QueryMember> res = new List<QueryMember>(Members.Count);
			foreach(QueryMember member in Members) {
				if(member.IsTotal)
					continue;
				res.Add(member);
			}
			return res;
		}
		public List<OLAPMember> GetMembers(bool includeAllMember) {  
			List<OLAPMember> res = new List<OLAPMember>(Members.Count);
			foreach(OLAPMember member in Members) {
				if(member.IsTotal && (!includeAllMember || member.UniqueName.EndsWith(DevExpress.PivotGrid.OLAP.OLAPMetadataColumn.TotalMemberString)))
					continue;
				res.Add(member);
			}
			return res;
		}
		protected override void SaveToStream(IQueryMetadata owner, TypedBinaryWriter writer) {
			ColumnHierarchy.SaveToStream(writer);
			writer.Write(Hierarchy.UniqueName);
			base.SaveToStream(owner, writer);
			writer.Write(Level);
			writer.Write(Cardinality);
			writer.Write((bool)(AllMember == null));
			if(AllMember != null)
				owner.SaveMember(AllMember, writer);
#if DEBUGTEST
			writer.Write((int)12345);
#endif
			writer.Write(DefaultMemberName != null ? DefaultMemberName : string.Empty);
			writer.Write(DrillDownColumn != null ? DrillDownColumn : string.Empty);
			if(!IsMeasure) {
				writer.WriteNullableString(defaultSortProperty);
				writer.Write(keyCount);
				writer.Write(properties.Count);
				foreach(KeyValuePair<string, OLAPDataType> pair in properties) {
					writer.Write(pair.Key);
					writer.Write((short)pair.Value);
				}
			}
		}
		protected override void WriteDataTypeAndColumnName(TypedBinaryWriter writer) {
			writer.Write((int)BaseDataType);
			writer.Write(ParentColumn != null ? ParentColumn.UniqueName : string.Empty);
		}
		protected override Type ReadDataType(TypedBinaryReader reader) {
			baseDataType = (OLAPDataType)reader.ReadInt32();
			return OLAPDataTypeConverter.Convert(baseDataType);
		}
		public override void RestoreFromStream(IQueryMetadata metadata, TypedBinaryReader reader) {
			columnHierarchy = new OLAPHierarchy();
			columnHierarchy.RestoreFromStream(reader);
			hierarchy = ((IOLAPMetadata)metadata).Hierarchies[reader.ReadString()];
			base.RestoreFromStream(metadata, reader);
			level = reader.ReadInt32();
			cardinality = reader.ReadInt32();
			bool allMemberIsNull = reader.ReadBoolean();
			if(!allMemberIsNull)
				AllMember = (OLAPMember)metadata.LoadMember(this, reader);
#if DEBUGTEST
			int sign = reader.ReadInt32();
			if(sign != 12345)
				throw new Exception("corrupted");
#endif
			defaultMemberName = reader.ReadString();
			drilldownColumn = reader.ReadString();
			if(!IsMeasure) {
				defaultSortProperty = reader.ReadNullableString();
				keyCount = reader.ReadInt32();
				int pCount = reader.ReadInt32();
				properties = new Dictionary<string, OLAPDataType>();
				for(int i = 0; i < pCount; i++)
					properties.Add(reader.ReadString(), (OLAPDataType)reader.ReadInt16());
			}
			totalMemberUniqueName = GetTotalMemberUniqueName();
			AddTotalMember();
		}
		public void SaveMembersToStream(IQueryMetadata owner, TypedBinaryWriter writer) {
			if(IsMeasure)
				throw new Exception("Can't store members for measure");
			IList<OLAPMember> columnMembers = GetMembers(false);
			writer.Write(AllMembersLoaded);
			writer.Write(UndefinedBoolToByte(HasNullValues));
			writer.Write(columnMembers.Count);
			for(int j = 0; j < columnMembers.Count; j++) {
				owner.SaveMember(columnMembers[j], writer);
			}
		}
		public void RestoreMembersFromStream(IQueryMetadata owner, TypedBinaryReader reader) {
			if(calculatedMembers != null)
				calculatedMembers = null;
			if(IsMeasure)
				throw new Exception("Can't store members for measure");
			AllMembersLoaded = reader.ReadBoolean();
			HasNullValues = UndefinedBoolFromByte(reader.ReadByte());
			int membersCount = reader.ReadInt32();
			for(int j = 0; j < membersCount; j++) {
				QueryMember member = owner.LoadMember(this, reader);
				Members.Add(member);
			}
		}
		public void AddCalculatedMember(OLAPMember member) {
			if(member.IsTotal)
				return;
			if(calculatedMembers == null)
				calculatedMembers = new Dictionary<OLAPMember, object>();
			calculatedMembers.Add(member, null);
			Members.Add(member);
		}
		public bool GetIsCalculatedMember(OLAPMember member) {
			return member.IsTotal || calculatedMembers != null && calculatedMembers.ContainsKey(member);
		}
		internal IEnumerable<object> EnumerateUniqueNames() {
			return Members.EnumerateUniqueNames();
		}
		internal IEnumerable<object> EnumerateValues() {
			return Members.EnumerateValues();
		}
		internal IEnumerable<OLAPMember> EnumerateMembers() {
			return Members.EnumerateMembers();
		}
		protected override QueryMember GetAllMember() {
			return AllMember;
		}
		#region IOLAPEditableMemberCollection
		IList<OLAPMember> IOLAPEditableMemberCollection.GetMembersByValue(bool recursive, int level, object value) {
			return Members.GetMembersByValue(recursive, level, value);
		}
		OLAPMember IOLAPEditableMemberCollection.GetMemberByUniqueLevelValue(string uniqueLevelValue) {
			return GetMemberByUniqueLevelValue(uniqueLevelValue);
		}
		public OLAPMember GetMemberByUniqueLevelValue(string uniqueLevelValue) {
			return this[uniqueLevelValue];
		}
		public void Add(QueryMember member) {
			Members.Add(member);
		}
		void IOLAPEditableMemberCollection.AddRange(IEnumerable<QueryMember> members) {
			Members.AddRange(members);
		}
		bool IOLAPEditableMemberCollection.Remove(QueryMember member) {
			return Members.Remove(member);
		}
		public void Clear() {
			Members.Clear();
			allMembersLoaded = false;
			if(allMember != null)
				Members.Add(allMember);
			AddTotalMember();
		}
		IQueryMetadataColumn IQueryMemberCollection.Column { get { return this; } }
		IEnumerator<OLAPMember> IEnumerable<OLAPMember>.GetEnumerator() {
			return ((IEnumerable<OLAPMember>)members).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<OLAPMember>)members).GetEnumerator();
		}
		#endregion
		public OLAPMember CreateMemberWithoutProps(IOLAPMemberSource source) {
			string uniqueName = source.UniqueName;
			OLAPMember member;
			if(ChildColumn != null || ParentColumn != null)
				member = new OLAPChildMember(this, uniqueName, source.GetValue(DataType), source.Caption);
			else {
				string caption = source.Caption;
				if(caption == null || caption.Equals(source.GetValue(DataType) as string))
					member = new OLAPMember(this, uniqueName, source.GetValue(DataType));
				else
					member = new CaptionableOLAPMember(this, uniqueName, source.GetValue(DataType), caption);
			}
			Add(member);
			return member;
		}
		List<CalculatedMemberSource> IOLAPLevel.CalculatedMembers {
			get { return null; }
		}
#if DEBUGTEST
		internal void SetCardinalityAccess(int value) {
			cardinality = value;
		}
#endif
	}
	public class CalculatedMemberSource {
		string uniqueName;
		string name;
		string levelName;
		public CalculatedMemberSource(string uniqueName, string name, string levelName) {
			Guard.ArgumentNotNull(uniqueName, "uniqueName");
			Guard.ArgumentNotNull(name, "name");
			Guard.ArgumentNotNull(levelName, "levelName");
			this.uniqueName = uniqueName;
			this.name = name;
			this.levelName = levelName;
		}
		public OLAPMember GetMember(OLAPMetadataColumn meta) {
			if(meta.UniqueName != levelName)
				throw new ArgumentException("calc member column");
			OLAPMember member = meta.ChildColumn != null || meta.ParentColumn != null ? (OLAPMember)new OLAPChildMember(meta, uniqueName, name, name) : new OLAPMember(meta, uniqueName, name);
			meta.AddCalculatedMember(member);
			return member;
		}
	}
}
