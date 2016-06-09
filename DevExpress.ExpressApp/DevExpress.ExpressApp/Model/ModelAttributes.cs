#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Model {
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
	public sealed class KeyPropertyAttribute : Attribute {
		public KeyPropertyAttribute(string keyPropertyName) {
			KeyPropertyName = keyPropertyName;
		}
		public string KeyPropertyName { get; set; }
	}
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
	public sealed class DisplayPropertyAttribute : Attribute {
		public DisplayPropertyAttribute(string keyPropertyName) {
			PropertyName = keyPropertyName;
		}
		public string PropertyName { get; set; }
	}
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
	public sealed class OmitDefaultGenerationAttribute : Attribute {
	}
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class ModelAbstractClassAttribute : Attribute {
	}
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ModelValueCalculatorAttribute : Attribute {
		Type type;
		string typeName;
		string linkValue;
		string nodeName;
		Type nodeType;
		string nodeTypeName;
		string propertyName;
		public static readonly ModelValueCalculatorAttribute Default = new ModelValueCalculatorAttribute(string.Empty);
		public ModelValueCalculatorAttribute(Type type) {
			this.type = type;
			this.typeName = type != null ? type.FullName : string.Empty;
		}
		public ModelValueCalculatorAttribute(string linkValue) {
			this.linkValue = linkValue;
		}
		public ModelValueCalculatorAttribute(string nodeName, string propertyName)
			: this(nodeName, null, propertyName) {
		}
		public ModelValueCalculatorAttribute(string nodeName, Type nodeType, string propertyName) {
			this.nodeName = nodeName;
			this.nodeType = nodeType;
			this.nodeTypeName = nodeType != null ? nodeType.FullName : string.Empty;
			this.propertyName = propertyName;
		}
		public Type Type { get { return type; } }
		public string LinkValue { get { return linkValue; } }
		public string NodeName { get { return nodeName; } }
		public Type NodeType { get { return nodeType; } }
		public string PropertyName { get { return propertyName; } }
		public override bool Equals(object obj) {
			ModelValueCalculatorAttribute attribute = obj as ModelValueCalculatorAttribute;
			return (attribute != null)
				&& attribute.Type == Type
				&& attribute.LinkValue == LinkValue
				&& attribute.NodeName == NodeName
				&& attribute.PropertyName == PropertyName
				&& attribute.NodeType == NodeType;
		}
		public override int GetHashCode() { return base.GetHashCode(); }
		public override bool IsDefaultAttribute() { return Equals(Default); }
	}
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	sealed class ModelNodeValueSourceAttribute : Attribute { 
		readonly string targetValueName;
		readonly string sourceNodePath;
		readonly string sourceValueName;
		internal ModelNodeValueSourceAttribute(string targetValueName, string sourceNodePath, string sourceValueName) {
			Guard.ArgumentNotNullOrEmpty(targetValueName, "targetValueName");
			Guard.ArgumentNotNullOrEmpty(sourceNodePath, "sourceNodePath");
			Guard.ArgumentNotNullOrEmpty(sourceValueName, "sourceValueName");
			this.targetValueName = targetValueName;
			this.sourceNodePath = sourceNodePath;
			this.sourceValueName = sourceValueName;
		}
		internal string TargetValueName { get { return targetValueName; } }
		internal string SourceNodePath { get { return sourceNodePath; } }
		internal string SourceValueName { get { return sourceValueName; } }
		public override bool Equals(object obj) {
			if(this != obj) {
				ModelNodeValueSourceAttribute other = obj as ModelNodeValueSourceAttribute;
				return other != null && TargetValueName == other.TargetValueName && SourceNodePath == other.SourceNodePath && SourceValueName == other.SourceValueName;
			}
			return true;
		}
		public override int GetHashCode() {
			return TargetValueName.GetHashCode() ^ SourceNodePath.GetHashCode() ^ SourceValueName.GetHashCode();
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] 
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
	public sealed class ModelDefaultValueCustomCodeAttribute : Attribute {
		readonly string targetValueName;
		readonly string customCode;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelDefaultValueCustomCodeAttribute(string targetValueName, string customCode) {
			Guard.ArgumentNotNullOrEmpty(targetValueName, "targetValueName");
			Guard.ArgumentNotNullOrEmpty(customCode, "customCode");
			this.targetValueName = targetValueName;
			this.customCode = customCode;
		}
		public string TargetValueName { get { return targetValueName; } }
		public string CustomCode { get { return customCode; } }
		public override int GetHashCode() {
			return TargetValueName.GetHashCode() ^ CustomCode.GetHashCode();
		}
		public override bool Equals(object obj) {
			if(this != obj) {
				ModelDefaultValueCustomCodeAttribute other = obj as ModelDefaultValueCustomCodeAttribute;
				return other != null && TargetValueName == other.TargetValueName && CustomCode == other.CustomCode;
			}
			return true;
		}
	}
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class ModelNodesGeneratorAttribute : Attribute {
		string typeName;
		public static readonly ModelNodesGeneratorAttribute Default = new ModelNodesGeneratorAttribute(typeof(object));
		public ModelNodesGeneratorAttribute(Type type) {
			this.typeName = type != null ? type.FullName : string.Empty;
		}
		public string TypeName { get { return typeName; } }
		public override bool Equals(object obj) {
			ModelNodesGeneratorAttribute attribute = obj as ModelNodesGeneratorAttribute;
			return (attribute != null) && (attribute.TypeName == TypeName);
		}
		public override int GetHashCode() { return base.GetHashCode(); }
		public override bool IsDefaultAttribute() { return Equals(Default); }
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface)]
	public sealed class ModelPersistentNameAttribute : Attribute {
		string name;
		public ModelPersistentNameAttribute(string name) {
			this.name = name;
		}
		public string Name { get { return name; } }
		public override bool Equals(object obj) {
			ModelPersistentNameAttribute attribute = obj as ModelPersistentNameAttribute;
			return (attribute != null) && (attribute.Name == Name);
		}
		public override int GetHashCode() { return base.GetHashCode(); }
	}
	[AttributeUsage(AttributeTargets.Interface)]
	public class ModelInterfaceImplementorAttribute : Attribute {
		string propertyName;
		Type implementedInterface;
		public ModelInterfaceImplementorAttribute(Type implementedInterface, string propertyName) {
			this.implementedInterface = implementedInterface;
			this.propertyName = propertyName;
		}
		public string PropertyName { get { return propertyName; } set { propertyName = value; } }
		public Type ImplementedInterface { get { return implementedInterface; } set { implementedInterface = value; } }
		public override bool Equals(object obj) {
			ModelInterfaceImplementorAttribute attribute = obj as ModelInterfaceImplementorAttribute;
			return (attribute != null) && (attribute.PropertyName == PropertyName && attribute.ImplementedInterface == ImplementedInterface);
		}
		public override int GetHashCode() { return base.GetHashCode(); }
	}
	[AttributeUsage(AttributeTargets.Class)]
	public class ModelGenerateContentActionAttribute : Attribute {
		private bool visible;
		public ModelGenerateContentActionAttribute(bool visible) {
			this.visible = visible;
		}
		public ModelGenerateContentActionAttribute() : this(true) { }
		public bool Visible {
			get { return visible; }
			set {
				visible = value;
			}
		}
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class ModelBrowsableAttribute : Attribute {
		private Type visibilityCalculatorType;
		public ModelBrowsableAttribute(Type visibilityCalculatorType) {
			this.visibilityCalculatorType = visibilityCalculatorType;
		}
		public Type VisibilityCalculatorType {
			get { return visibilityCalculatorType; }
		}
	}
	[AttributeUsage(AttributeTargets.Interface)]
	public class ModelDisplayNameAttribute : Attribute {
		private string modelDisplayName;
		public ModelDisplayNameAttribute(string modelDisplayName) {
			this.modelDisplayName = modelDisplayName;
		}
		public string ModelDisplayName {
			get { return modelDisplayName; }
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface)]
	public class ModelReadOnlyAttribute : Attribute {
		private Type readOnlyCalculatorType;
		public ModelReadOnlyAttribute(Type readOnlyCalculatorType) {
			this.readOnlyCalculatorType = readOnlyCalculatorType;
		}
		public Type ReadOnlyCalculatorType {
			get { return readOnlyCalculatorType; }
		}
	}
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class RequiredAttribute : Attribute {
		private Type requiredCalculatorType;
		public RequiredAttribute(Type requiredCalculatorType) {
			this.requiredCalculatorType = requiredCalculatorType;
		}
		public RequiredAttribute() {
		}
		public Type RequiredCalculatorType {
			get { return requiredCalculatorType; }
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface, Inherited = true, AllowMultiple = true)]
	public sealed class ModelDefaultAttribute : Attribute {
		private readonly String propertyName;
		private readonly String propertyValue;
		public ModelDefaultAttribute(String propertyName, String propertyValue) {
			this.propertyName = propertyName;
			this.propertyValue = propertyValue;
		}
		public String PropertyName {
			get { return propertyName; }
		}
		public String PropertyValue {
			get { return propertyValue; }
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Interface)]
	public class ModelEditorBrowsableAttribute : Attribute {
		public ModelEditorBrowsableAttribute(bool visible) {
			Visible = visible;
		}
		public bool Visible {
			get;
			set;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class ModelVirtualTreeAttribute : Attribute {
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class ModelVirtualTreeCreatableItemsFilterAttribute : Attribute {
		public ModelVirtualTreeCreatableItemsFilterAttribute(params Type[] filteredTypes) {
			this.FilteredTypes = filteredTypes;
		}
		public Type[] FilteredTypes {
			get;
			set;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class ModelVirtualTreeDragDropItemAttribute : Attribute {
		public ModelVirtualTreeDragDropItemAttribute(params Type[] supportedTypes) {
			this.SupportedTypes = supportedTypes;
		}
		public Type[] SupportedTypes {
			get;
			private set;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class ModelVirtualTreeAddItemAttribute : Attribute {
		public ModelVirtualTreeAddItemAttribute(Type realParentNode, params Type[] supportedTypes) {
			this.RealParentNode = realParentNode;
			this.SupportedTypes = supportedTypes;
		}
		public Type RealParentNode {
			get;
			private set;
		}
		public Type[] SupportedTypes {
			get;
			private set;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class ModelVirtualTreeParentAttribute : Attribute {
		Type logicType;
		public ModelVirtualTreeParentAttribute(Type logicType) {
			this.logicType = logicType;
		}
		public Type LogicType {
			get { return logicType; }
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IModelVirtualTreeParentLogic {
		bool SetParent(IModelNode draggedModelNode, IModelNode targer);
		IModelNode GetParent(IModelNode item);
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class ModelVirtualTreeGetChildrenAttribute : Attribute {
		Type childrenProviderType;
		public ModelVirtualTreeGetChildrenAttribute(Type childrenProviderType) {
			this.childrenProviderType = childrenProviderType;
		}
		public Type ChildrenProviderType {
			get { return childrenProviderType; }
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class ModelVirtualTreeDisplayPropertiesAttribute : Attribute {
		public ModelVirtualTreeDisplayPropertiesAttribute(bool all) {
			All = all;
		}
		public ModelVirtualTreeDisplayPropertiesAttribute(params string[] displayProperties) {
			DisplayProperties = displayProperties;
		}
		public bool All {
			get;
			private set;
		}
		public string[] DisplayProperties {
			get;
			private set;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class ModelVirtualTreeHideChildrenAttribute : Attribute {
		private Type visibilityCalculatorType;
		public ModelVirtualTreeHideChildrenAttribute()
			: this(typeof(ModelVirtualTreeHideChildren)) {
		}
		public ModelVirtualTreeHideChildrenAttribute(Type visibilityCalculatorType) {
			this.visibilityCalculatorType = visibilityCalculatorType;
		}
		public Type VisibilityCalculatorType {
			get { return visibilityCalculatorType; }
		}
		private class ModelVirtualTreeHideChildren : IModelIsVisible {
			public bool IsVisible(IModelNode node, string propertyName) {
				return false;
			}
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class ModelHideLinksAttribute : Attribute {
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ModelRefreshTreeNode : Attribute { }
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class ModelHidePropertiesAttribute : Attribute {
		public ModelHidePropertiesAttribute(params string[] hideProperties) {
			HideProperties = hideProperties;
		}
		public string[] HideProperties {
			get;
			private set;
		}
	}
	[AttributeUsage(AttributeTargets.Property)]
	public class DetailViewLayoutAttribute : Attribute {
		private LayoutGroupType groupType;
		private string groupId;
		private LayoutColumnPosition? columnPosition;
		private int groupIndex;
		public DetailViewLayoutAttribute(LayoutColumnPosition? columnPosition, string groupId, LayoutGroupType groupType, int groupIndex) {
			this.columnPosition = columnPosition;
			this.groupType = groupType;
			this.groupId = groupId;
			this.groupIndex = groupIndex;
		}
		public DetailViewLayoutAttribute(string groupId) :
			this(null, groupId, LayoutGroupType.SimpleEditorsGroup, -1) {
		}
		public DetailViewLayoutAttribute(LayoutGroupType groupType) :
			this(null, null, groupType, -1) {
		}
		public DetailViewLayoutAttribute(LayoutColumnPosition columnPosition) :
			this(columnPosition, null, LayoutGroupType.SimpleEditorsGroup, -1) {
		}
		public DetailViewLayoutAttribute(LayoutColumnPosition columnPosition, string groupId) :
			this(columnPosition, groupId, LayoutGroupType.SimpleEditorsGroup, -1) {
		}
		public DetailViewLayoutAttribute(string groupId, int groupIndex) :
			this(null, groupId, LayoutGroupType.SimpleEditorsGroup, groupIndex) {
		}
		public DetailViewLayoutAttribute(LayoutColumnPosition? columnPosition, LayoutGroupType groupType) :
			this(columnPosition, null, groupType, -1) {
		}
		public DetailViewLayoutAttribute(string groupId, LayoutGroupType groupType) :
			this(null, groupId, groupType, -1) {
		}
		public DetailViewLayoutAttribute(LayoutColumnPosition columnPosition, string groupId, int groupIndex) :
			this(columnPosition, groupId, LayoutGroupType.SimpleEditorsGroup, groupIndex) {
		}
		public DetailViewLayoutAttribute(string groupId, LayoutGroupType groupType, int groupIndex) :
			this(null, groupId, groupType, groupIndex) {
		}
		public LayoutGroupType GroupType {
			get { return groupType; }
		}
		public string GroupId {
			get { return groupId; }
		}
		public LayoutColumnPosition? ColumnPosition {
			get { return columnPosition; }
		}
		public int GroupIndex {
			get { return groupIndex; }
		}
	}
	public enum LayoutColumnPosition { Left, Right }
	public enum LayoutGroupType { SimpleEditorsGroup, SizableEditorsGroup, TabbedGroup}
}
