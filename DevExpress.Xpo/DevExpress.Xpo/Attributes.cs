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

namespace DevExpress.Xpo {
	using System;
	using System.Collections.Specialized;
	using System.Xml;
	using System.Globalization;
	using DevExpress.Data.Filtering;
	using DevExpress.Xpo.Metadata;
	using System.ComponentModel;
using System.Collections.Generic;
	using System.Collections.ObjectModel;
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public sealed class ValueConverterAttribute : Attribute {
		Type converterType;
#if !SL
	[DevExpressXpoLocalizedDescription("ValueConverterAttributeConverterType")]
#endif
		public Type ConverterType { get { return converterType; } set { converterType = value; } }
		public ValueConverterAttribute(Type converterType) {
			this.converterType = converterType;
		}
#if !SL
		ValueConverterAttribute(XmlNode attributeNode) {
			if(attributeNode.Attributes["ConverterType"] != null)
				converterType = Type.GetType(attributeNode.Attributes["ConverterType"].Value);
		}
#endif
		ValueConverter converter;
#if !SL
	[DevExpressXpoLocalizedDescription("ValueConverterAttributeConverter")]
#endif
		public ValueConverter Converter {
			get {
				if(converter == null) {
					converter = (ValueConverter)Activator.CreateInstance(ConverterType);
				}
				return converter;
			}
		}
	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public sealed class NullValueAttribute : Attribute {
		object value;
#if !SL
	[DevExpressXpoLocalizedDescription("NullValueAttributeValue")]
#endif
		public object Value { get { return value; } set { this.value = value; } }
		public NullValueAttribute(object value) {
			this.value = value;
		}
		public NullValueAttribute(Byte value) : this((object)value) { }
		public NullValueAttribute(short value) : this((object)value) { }
		public NullValueAttribute(int value) : this((object)value) { }
		public NullValueAttribute(long value) : this((object)value) { }
		public NullValueAttribute(Single value) : this((object)value) { }
		public NullValueAttribute(Double value) : this((object)value) { }
		public NullValueAttribute(Char value) : this((object)value) { }
		public NullValueAttribute(string value) : this((object)value) { }
		public NullValueAttribute(bool value) : this((object)value) { }
		public NullValueAttribute(Type type, string value) {
			try {
#if !CF && !SL
				this.value = System.ComponentModel.TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
#else
				((IConvertible)value).ToType(type, CultureInfo.InvariantCulture);
#endif
			} catch { }
		}
#if !SL
		NullValueAttribute(XmlNode attributeNode) {
			if(attributeNode.Attributes["Value"] != null)
				value = attributeNode.Attributes["Value"].Value;
		}
#endif
	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public sealed class ExplicitLoadingAttribute : Attribute {
		int depth;
#if !SL
	[DevExpressXpoLocalizedDescription("ExplicitLoadingAttributeDepth")]
#endif
		public int Depth { get { return depth; } set { depth = value; } }
		public ExplicitLoadingAttribute() : this(1) { }
		public ExplicitLoadingAttribute(int depth) {
			this.depth = depth;
		}
#if !SL
		ExplicitLoadingAttribute(XmlNode attributeNode) {
			if(attributeNode.Attributes["Depth"] != null)
				depth = ((IConvertible)attributeNode.Attributes["Depth"].Value).ToInt32(CultureInfo.InvariantCulture);
		}
#endif
	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public sealed class DbTypeAttribute : Attribute {
		string dbColumnTypeName = string.Empty;
#if !SL
	[DevExpressXpoLocalizedDescription("DbTypeAttributeDbColumnTypeName")]
#endif
		public string DbColumnTypeName { get { return dbColumnTypeName; } set { dbColumnTypeName = value; } }
		public DbTypeAttribute(string dbColumnTypeName) {
			this.dbColumnTypeName = dbColumnTypeName;
		}
#if !SL
		DbTypeAttribute(XmlNode attributeNode) {
			if(attributeNode.Attributes["DbColumnTypeName"] != null)
				dbColumnTypeName = attributeNode.Attributes["DbColumnTypeName"].Value;
		}
#endif
	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public sealed class AggregatedAttribute : Attribute { }
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public sealed class MergeCollisionBehaviorAttribute : Attribute {
		OptimisticLockingReadMergeBehavior behavior;
		public OptimisticLockingReadMergeBehavior Behavior {
			get { return behavior; }
		}
		public MergeCollisionBehaviorAttribute(OptimisticLockingReadMergeBehavior behavior) {
			this.behavior = behavior;
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
	public sealed class OptimisticLockingReadBehaviorAttribute : Attribute {
		bool? trackPropertiesModifications;
		public bool? TrackPropertiesModifications {
			get { return trackPropertiesModifications; }
		}
		OptimisticLockingReadBehavior behavior;
		public OptimisticLockingReadBehavior Behavior {
			get { return behavior; }
		}
		public OptimisticLockingReadBehaviorAttribute(OptimisticLockingReadBehavior behavior) {
			this.behavior = behavior;
		}
		public OptimisticLockingReadBehaviorAttribute(OptimisticLockingReadBehavior behavior, bool trackPropertiesModifications)
			: this(behavior) {			
			this.trackPropertiesModifications = trackPropertiesModifications;
		}
	}
	public enum OptimisticLockingBehavior {
		NoLocking,
		ConsiderOptimisticLockingField,
		LockModified,
		LockAll
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
	public sealed class OptimisticLockingAttribute : Attribute {
		OptimisticLockingBehavior lockingKind;
		public const string DefaultFieldName = "OptimisticLockField";
		string fieldName;
#if !SL
	[DevExpressXpoLocalizedDescription("OptimisticLockingAttributeEnabled")]
#endif
		public bool Enabled { get { return lockingKind == OptimisticLockingBehavior.ConsiderOptimisticLockingField; } set { lockingKind = value ? OptimisticLockingBehavior.ConsiderOptimisticLockingField : OptimisticLockingBehavior.NoLocking; } }
#if !SL
	[DevExpressXpoLocalizedDescription("OptimisticLockingAttributeFieldName")]
#endif
		public string FieldName { get { return fieldName; } }
		public OptimisticLockingBehavior LockingKind {
			get { return lockingKind; }
			set { lockingKind = value; }
		}
		public OptimisticLockingAttribute() : this(true) { }
		public OptimisticLockingAttribute(bool enabled) {
			this.Enabled = enabled;
			fieldName = DefaultFieldName;
		}
		public OptimisticLockingAttribute(string fieldName) {
			this.fieldName = fieldName;
			this.Enabled = true;
		}
		public OptimisticLockingAttribute(OptimisticLockingBehavior lockingKind) {
			fieldName = DefaultFieldName;
			this.lockingKind = lockingKind;
		}
#if !SL
		OptimisticLockingAttribute(XmlNode attributeNode) {
			if(attributeNode.Attributes["Enabled"] != null)
				Enabled = attributeNode.Attributes["Enabled"].Value.ToLower(CultureInfo.InvariantCulture) == "true";
			if(attributeNode.Attributes["FieldName"] != null)
				fieldName = attributeNode.Attributes["FieldName"].Value;
		}
#endif
	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public sealed class KeyAttribute : Attribute {
		bool autoGenerate;
#if !SL
	[DevExpressXpoLocalizedDescription("KeyAttributeAutoGenerate")]
#endif
		public bool AutoGenerate { get { return autoGenerate; } set { autoGenerate = value; } }
		public KeyAttribute() : this(false) { }
		public KeyAttribute(bool autoGenerate) {
			this.autoGenerate = autoGenerate;
		}
#if !SL
		KeyAttribute(XmlNode attributeNode) {
			if(attributeNode.Attributes["AutoGenerate"] != null)
				autoGenerate = attributeNode.Attributes["AutoGenerate"].Value.ToLower(CultureInfo.InvariantCulture) == "true";
		}
#endif
	}
	public enum MapInheritanceType { ParentTable, OwnTable }
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
	public sealed class MapInheritanceAttribute : Attribute {
		MapInheritanceType mapType;
#if !SL
	[DevExpressXpoLocalizedDescription("MapInheritanceAttributeMapType")]
#endif
		public MapInheritanceType MapType { get { return mapType; } }
		public MapInheritanceAttribute(MapInheritanceType mapType) {
			this.mapType = mapType;
		}
#if !SL
		MapInheritanceAttribute(XmlNode attributeNode) {
			this.mapType = (MapInheritanceType)Enum.Parse(typeof(MapInheritanceType), attributeNode.Attributes["MapType"].Value, false);
		}
#endif
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
	public sealed class PersistentAttribute : Attribute {
		string mapTo;
#if !SL
	[DevExpressXpoLocalizedDescription("PersistentAttributeMapTo")]
#endif
		public string MapTo { get { return mapTo; } }
		public static readonly Type AttributeType = typeof(PersistentAttribute);
		public PersistentAttribute() { }
		public PersistentAttribute(string mapTo) {
			this.mapTo = mapTo;
		}
#if !SL
		PersistentAttribute(XmlNode attributeNode) {
			if(attributeNode.Attributes["MapTo"] != null)
				mapTo = attributeNode.Attributes["MapTo"].Value;
		}
#endif
	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public sealed class AssociationAttribute : Attribute {
		string name;
		string assemblyName;
		string elementTypeName;
		Type elementType;
		bool _UseAssociationNameAsIntermediateTableName = false;
#if !SL
	[DevExpressXpoLocalizedDescription("AssociationAttributeName")]
#endif
		public string Name { get { return name; } }
#if !SL
	[DevExpressXpoLocalizedDescription("AssociationAttributeElementTypeName")]
#endif
		public string ElementTypeName {
			get { return elementTypeName; }
			set { elementTypeName = value; }
		}
		internal Type ElementType {
			get { return elementType; }
		}
#if !SL
	[DevExpressXpoLocalizedDescription("AssociationAttributeAssemblyName")]
#endif
		public string AssemblyName {
			get { return assemblyName; }
			set { assemblyName = value; }
		}
#if !SL
	[DevExpressXpoLocalizedDescription("AssociationAttributeUseAssociationNameAsIntermediateTableName")]
#endif
		public bool UseAssociationNameAsIntermediateTableName {
			get { return _UseAssociationNameAsIntermediateTableName; }
			set { _UseAssociationNameAsIntermediateTableName = value; }
		}
#if !SL
		AssociationAttribute(XmlNode attributeNode)
			: this(attributeNode.Attributes["Name"].Value, attributeNode.Attributes["AssemblyName"].Value, attributeNode.Attributes["ElementTypeName"].Value) {
		}
#endif
		public AssociationAttribute(string name, string elementAssemblyName, string elementTypeName) {
			this.name = name;
			this.assemblyName = elementAssemblyName;
			this.elementTypeName = elementTypeName;
		}
		public AssociationAttribute() : this(string.Empty, string.Empty, string.Empty) { }
		public AssociationAttribute(string name) : this(name, string.Empty, string.Empty) { }
		public AssociationAttribute(string name, Type elementType)
			: this(name, XPClassInfo.GetShortAssemblyName(elementType.Assembly), elementType.FullName) {
			this.elementType = elementType;
		}
		public AssociationAttribute(Type elementType) : this(string.Empty, elementType) { }
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Interface)]
	public sealed class NonPersistentAttribute : Attribute {
		public static readonly Type AttributeType = typeof(NonPersistentAttribute);
	}
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class MapToAttribute : Attribute {
		string mappingName;
#if !SL
	[DevExpressXpoLocalizedDescription("MapToAttributeMappingName")]
#endif
		public string MappingName { get { return mappingName; } }
#if !SL
		MapToAttribute(XmlNode attributeNode) {
			this.mappingName = attributeNode.Attributes["MappingName"].Value;
		}
#endif
		[Obsolete("Please use Persistent attribute with mapTo parameter instead")]
		public MapToAttribute(string mappingName) {
			this.mappingName = mappingName;
		}
	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public sealed class SizeAttribute : Attribute {
		Int32 size = 0;
#if !SL
	[DevExpressXpoLocalizedDescription("SizeAttributeSize")]
#endif
		public Int32 Size { get { return size; } }
#if !SL
		SizeAttribute(XmlNode attributeNode) {
			string ssize = attributeNode.Attributes["Size"].Value;
			this.size = ssize == "Unlimited" ? SizeAttribute.Unlimited : Convert.ToInt32(ssize);
		}
#endif
		public SizeAttribute(Int32 size) {
			this.size = size;
		}
		public const Int32 Unlimited = -1;
		public const Int32 DefaultStringMappingFieldSize = 100;
	}
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public sealed class DelayedAttribute : Attribute {
		bool updateModifiedOnly;
		string fieldName;
		string groupName;
#if !SL
		DelayedAttribute(XmlNode attributeNode) {
			XmlAttribute fld = attributeNode.Attributes["FieldName"];
			if(fld != null)
				this.fieldName = fld.Value;
			XmlAttribute gr = attributeNode.Attributes["GroupName"];
			if(gr != null)
				this.groupName = gr.Value;
		}
#endif
		public DelayedAttribute() : this(null, null) { }
		public DelayedAttribute(bool updateModifiedOnly) : this(null, null, updateModifiedOnly) { }
		public DelayedAttribute(string fieldName)
			: this(fieldName, null) { }
		public DelayedAttribute(string fieldName, bool updateModifiedOnly)
			: this(fieldName, null, updateModifiedOnly) { }
		public DelayedAttribute(string fieldName, string groupName)
			: this(fieldName, groupName, false) { }
		public DelayedAttribute(string fieldName, string groupName, bool updateModifiedOnly) {
			this.fieldName = fieldName;
			this.groupName = groupName;
			this.updateModifiedOnly = updateModifiedOnly;
		}
#if !SL
	[DevExpressXpoLocalizedDescription("DelayedAttributeFieldName")]
#endif
		public string FieldName { get { return fieldName; } }
#if !SL
	[DevExpressXpoLocalizedDescription("DelayedAttributeGroupName")]
#endif
		public string GroupName { get { return groupName; } }
#if !SL
	[DevExpressXpoLocalizedDescription("DelayedAttributeUpdateModifiedOnly")]
#endif
		public bool UpdateModifiedOnly { get { return updateModifiedOnly; } }
	}
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public sealed class PersistentAliasAttribute : Attribute {
		string _aliasExpression;
		CriteriaOperator _expression;
#if !SL
		PersistentAliasAttribute(XmlNode attributeNode) {
			this._aliasExpression = attributeNode.Attributes["AliasExpression"].Value;
		}
#endif
		public PersistentAliasAttribute(string aliasExpression) { this._aliasExpression = aliasExpression; }
#if !SL
	[DevExpressXpoLocalizedDescription("PersistentAliasAttributeAliasExpression")]
#endif
		public string AliasExpression { get { return _aliasExpression; } }
		internal CriteriaOperator Criteria {
			get {
				if(ReferenceEquals(_expression, null))
					_expression = CriteriaOperator.Parse(AliasExpression);
				return _expression;
			}
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
	public sealed class CustomAttribute : Attribute {
		string theName;
		string theValue;
#if !SL
	[DevExpressXpoLocalizedDescription("CustomAttributeName")]
#endif
		public string Name { get { return theName; } }
#if !SL
	[DevExpressXpoLocalizedDescription("CustomAttributeValue")]
#endif
		public string Value { get { return theValue; } }
#if !SL
		CustomAttribute(XmlNode attributeNode) {
			theName = attributeNode.Attributes["Name"].Value;
			theValue = attributeNode.Attributes["Value"].Value;
		}
#endif
		public CustomAttribute(string theName, string theValue) {
			this.theName = theName;
			this.theValue = theValue;
		}
	}
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public sealed class IndicesAttribute : Attribute {
		static char[] splitter = new char[] { ';' };
		IList<StringCollection> indices;
		public IList<StringCollection> Indices {
			get { return indices; }
		}
		public IndicesAttribute()
			: this((string[])null) {
		}
		public IndicesAttribute(string index)
			: this(new string[] { index }) {
		}
		public IndicesAttribute(string index1, string index2)
			: this(new string[] { index1, index2 }) {
		}
		public IndicesAttribute(string index1, string index2, string index3)
			: this(new string[] { index1, index2, index3 }) {
		}
		public IndicesAttribute(params string[] indices) {
			if(indices == null || indices.Length == 0) {
				this.indices = new StringCollection[0];
				return;
			}
			List<StringCollection> result = new List<StringCollection>(indices.Length);
			foreach(string index in indices) {
				StringCollection indexColumns = new StringCollection();
				indexColumns.AddRange(index.Split(splitter, StringSplitOptions.RemoveEmptyEntries));
				result.Add(indexColumns);
			}
			this.indices = result.AsReadOnly();
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
	public sealed class IndexedAttribute : Attribute {
		bool unique;
		string name;
		StringCollection additionalFields = new StringCollection();
#if !SL
	[DevExpressXpoLocalizedDescription("IndexedAttributeUnique")]
#endif
		public bool Unique {
			get { return unique; }
			set { unique = value; }
		}
#if !SL
	[DevExpressXpoLocalizedDescription("IndexedAttributeName")]
#endif
		public string Name {
			get { return name; }
			set { name = value; }
		}
#if !SL
	[DevExpressXpoLocalizedDescription("IndexedAttributeAdditionalFields")]
#endif
		public StringCollection AdditionalFields {
			get { return additionalFields; }
		}
#if !SL
		IndexedAttribute(XmlNode attributeNode) {
			if(attributeNode.Attributes["Unique"] != null)
				unique = Convert.ToBoolean(attributeNode.Attributes["Unique"].Value);
			if(attributeNode.Attributes["Name"] != null)
				name = attributeNode.Attributes["Name"].Value;
			XmlNode additionalFieldsNode = null;
			foreach(XmlNode chNode in attributeNode.ChildNodes) {
				if(chNode.Name == "AdditionalFields") {
					additionalFieldsNode = chNode;
					break;
				}
			}
			if(additionalFieldsNode != null) {
				foreach(XmlNode chNode in additionalFieldsNode.ChildNodes) {
					if(chNode.Name == "string") {
						additionalFields.Add(chNode.InnerText);
					}
				}
			}
		}
#endif
		public IndexedAttribute(string additionalFields)
			: this(additionalFields.Split(';')) {
		}
		public IndexedAttribute() {
			unique = false;
		}
		public IndexedAttribute(params string[] additionalFields) {
			unique = false;
			this.additionalFields.AddRange(additionalFields);
		}
		public IndexedAttribute(string additionalField1, string additionalField2)
			: this(new string[] { additionalField1, additionalField2 }) {
		}
		public IndexedAttribute(string additionalField1, string additionalField2, string additionalField3)
			: this(new string[] { additionalField1, additionalField2, additionalField3 }) {
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, Inherited = true)]
	public sealed class MemberDesignTimeVisibilityAttribute : Attribute {
		bool isVisible = true;
#if !SL
		MemberDesignTimeVisibilityAttribute(XmlNode attributeNode) {
			if(attributeNode.Attributes["IsVisible"] != null)
				this.isVisible = Convert.ToBoolean(attributeNode.Attributes["IsVisible"].Value);
		}
#endif
		public MemberDesignTimeVisibilityAttribute() { }
		public MemberDesignTimeVisibilityAttribute(bool isVisible) { this.isVisible = isVisible; }
#if !SL
	[DevExpressXpoLocalizedDescription("MemberDesignTimeVisibilityAttributeIsVisible")]
#endif
		public bool IsVisible { get { return isVisible; } }
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
	public sealed class NoForeignKeyAttribute : Attribute {
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
	public sealed class DeferredDeletionAttribute : Attribute {
		bool enabled;
#if !SL
	[DevExpressXpoLocalizedDescription("DeferredDeletionAttributeEnabled")]
#endif
		public bool Enabled { get { return enabled; } set { enabled = value; } }
		public DeferredDeletionAttribute() : this(true) { }
		public DeferredDeletionAttribute(bool enabled) {
			this.enabled = enabled;
		}
#if !SL
		DeferredDeletionAttribute(XmlNode attributeNode) {
			enabled = true;
			if(attributeNode.Attributes["Enabled"] != null)
				enabled = attributeNode.Attributes["Enabled"].Value.ToLower(CultureInfo.InvariantCulture) == "true";
		}
#endif
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
	public class DisplayNameAttribute : Attribute {
		string displayName;
#if !SL
		DisplayNameAttribute(XmlNode attributeNode) {
			if(attributeNode.Attributes["DisplayName"] != null)
				this.displayName = attributeNode.Attributes["DisplayName"].Value;
		}
#endif
		public DisplayNameAttribute() { }
		public DisplayNameAttribute(string displayName) { this.displayName = displayName; }
#if !SL
	[DevExpressXpoLocalizedDescription("DisplayNameAttributeDisplayName")]
#endif
		public virtual string DisplayName { get { return displayName; } }
	}
	public enum DefaultMembersPersistence { Default, OnlyDeclaredAsPersistent }
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public sealed class DefaultMembersPersistenceAttribute : Attribute {
		DefaultMembersPersistence defaultPersistence;
#if !SL
	[DevExpressXpoLocalizedDescription("DefaultMembersPersistenceAttributeDefaultMembersPersistence")]
#endif
		public DefaultMembersPersistence DefaultMembersPersistence { get { return defaultPersistence; } }
		public DefaultMembersPersistenceAttribute(DefaultMembersPersistence defaultPersistence) {
			this.defaultPersistence = defaultPersistence;
		}
#if !SL
		DefaultMembersPersistenceAttribute(XmlNode attributeNode) {
			this.defaultPersistence = (DefaultMembersPersistence)Enum.Parse(typeof(DefaultMembersPersistence), attributeNode.Attributes["DefaultMembersPersistence"].Value, false);
		}
#endif
	}
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public sealed class ManyToManyAliasAttribute : Attribute {
		string _OneToManyCollectionName;
		string _ReferenceInTheIntermediateTableName;
		public string OneToManyCollectionName {
			get { return _OneToManyCollectionName; }
			set { _OneToManyCollectionName = value; }
		}
		public string ReferenceInTheIntermediateTableName {
			get { return _ReferenceInTheIntermediateTableName; }
			set { _ReferenceInTheIntermediateTableName = value; }
		}
#if !SL
		ManyToManyAliasAttribute(XmlNode attributeNode)
			: this(attributeNode.Attributes["OneToManyCollectionName"].Value, attributeNode.Attributes["ReferenceInTheIntermediateTableName"].Value) {
		}
#endif
		public ManyToManyAliasAttribute(string oneToManyCollectionName, string referenceInTheIntermediateTableName) {
			this._OneToManyCollectionName = oneToManyCollectionName;
			this._ReferenceInTheIntermediateTableName = referenceInTheIntermediateTableName;
		}
	}
}
