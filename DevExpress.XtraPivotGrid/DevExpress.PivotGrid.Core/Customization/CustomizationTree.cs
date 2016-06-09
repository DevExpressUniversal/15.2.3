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

using DevExpress.XtraPivotGrid.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPivotGrid.Customization {
	public class PivotCustomizationFieldsTreeBase : IEnumerable<PivotCustomizationTreeNodeBase> {
		readonly CustomizationFormFields fields;
		readonly PivotCustomizationTreeNodeFactoryBase nodeFactory;
		PivotCustomizationTreeNodeRootBase root;
		bool isGrouping;
		public PivotCustomizationFieldsTreeBase(CustomizationFormFields fields, PivotCustomizationTreeNodeFactoryBase nodeFactory) {
			if(fields == null)
				throw new ArgumentNullException("fields");
			this.fields = fields;
			this.nodeFactory = nodeFactory;
		}
		protected bool IsGrouping { get { return isGrouping; } }
		protected CustomizationFormFields Fields { get { return fields; } }
		protected PivotCustomizationTreeNodeRootBase Root { get { return root; } }
		protected PivotCustomizationTreeNodeFactoryBase NodeFactory { get { return nodeFactory; } }
		public void Update(bool isGrouping) {
			this.isGrouping = isGrouping;
			PivotCustomizationTreeNodeRootBase oldRoot = Root;
			this.root = NodeFactory.CreateRootNode();
			PopulateTree(Fields.HiddenFields);
			LoadExpandedState(oldRoot);
		}
		protected virtual void LoadExpandedState(PivotCustomizationTreeNodeRootBase oldRoot) {
			if(oldRoot == null) return;
			foreach(PivotCustomizationTreeNodeBase oldNode in oldRoot) {
				if(!oldNode.IsExpanded) continue;
				PivotCustomizationTreeNodeBase newNode = FindChild(oldNode.GetBranch());
				if(newNode != null)
					newNode.IsExpanded = true;
			}
		}
		protected PivotCustomizationTreeNodeBase FindChild(List<PivotCustomizationTreeNodeBase> branch) {
			PivotCustomizationTreeNodeBase current = Root;
			for(int i = 1; i < branch.Count; i++) {
				current = current.FindChild(branch[i].Name, branch[i].Field);
				if(current == null)
					return null;
			}
			return current;
		}
		protected void PopulateTree(IEnumerable fields) {
			PopulateFields(fields);
			Root.Sort();
		}
		protected virtual void PopulateFields(IEnumerable fields) {
			foreach(PivotFieldItemBase field in fields)
				 AddNode(NodeFactory.CreateNode(field));
		}
		protected virtual void AddNode(PivotCustomizationTreeNodeBase node) {
			PivotCustomizationTreeNodeBase current = root;
			if(IsGrouping) {
				current = FindOrCreateDimensionNode(node.Field, current);
				current = FindOrCreateFolderPath(node.Field.DisplayFolder, current);
				if(node.Field.KPIType != PivotKPIType.None)
					current = FindOrCreateKPIFolder(node.Field, current);
			}
			current.AddChild(node);
		}
		protected virtual PivotCustomizationTreeNodeBase FindOrCreateDimensionNode(PivotFieldItemBase field, PivotCustomizationTreeNodeBase node) {
			string dimensionName = NodeFactory.GetDimensionName(field);
			if(string.IsNullOrEmpty(dimensionName)) return node;
			PivotCustomizationTreeNodeBase dimension = node.FindChild(dimensionName, null);
			if(dimension == null) {
				dimension = NodeFactory.CreateDimensionNode(dimensionName);
				node.AddChild(dimension);
			}
			return dimension;
		}
		protected virtual PivotCustomizationTreeNodeBase FindOrCreateFolderPath(string displayFolder, PivotCustomizationTreeNodeBase node) {
			if(string.IsNullOrEmpty(displayFolder)) return node;
			PivotCustomizationTreeNodeBase current = node;
			foreach(string folderName in NodeFactory.GetFoldersFromPath(displayFolder)) {
				if(string.IsNullOrEmpty(folderName)) continue;
				PivotCustomizationTreeNodeBase child = current.FindChild(folderName, null);
				if(child == null) {
					child = NodeFactory.CreateFolderNode(folderName);
					current.AddChild(child);
				}
				current = child;
			}
			return current;
		}
		protected virtual PivotCustomizationTreeNodeBase FindOrCreateKPIFolder(PivotFieldItemBase field, PivotCustomizationTreeNodeBase current) {
			string kpiName = NodeFactory.GetKPIName(field);
			PivotCustomizationTreeNodeBase child = current.FindChild(kpiName, null);
			if(child == null) {
				child = NodeFactory.CreateFolderNode(kpiName);
				current.AddChild(child);
			}
			return child;
		}
		public int GetNodeVisibleIndex(ICustomizationTreeItem node) {
			int index = 0;
			foreach(PivotCustomizationTreeNodeBase item in this) {
				if(!item.IsVisible) continue;
				if(item == node) return index;
				index++;
			}
			return -1;
		}
		public bool Contains(ICustomizationTreeItem node) {
			return FindChild(((PivotCustomizationTreeNodeBase)node).GetBranch()) != null;
		}
		IEnumerator<PivotCustomizationTreeNodeBase> IEnumerable<PivotCustomizationTreeNodeBase>.GetEnumerator() {
			return ((IEnumerable<PivotCustomizationTreeNodeBase>)Root).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)Root).GetEnumerator();
		}
	}
	public class PivotCustomizationTreeNodeFactoryBase {
		readonly PivotGridData data;
		public PivotCustomizationTreeNodeFactoryBase(PivotGridData data) {
			this.data = data;
		}
		public virtual PivotCustomizationTreeNodeBase CreateNode(PivotFieldItemBase field) {
			string[] fullName = GetNames(field.FieldName);
			if(fullName[0] == MeasuresFolderName)
				if(field.KPIType == PivotKPIType.None)
					return CreateMeasureNode(field);
				else
					return CreateKpiNode(field);
			PivotGroupItem group = field.Group;
			if(group != null && field.IsFirstFieldInGroup)
				return CreateHierarchyNode(field);
			if(field != null)
				return CreateAttributeNode(field);
			throw new ArgumentException("field");
		}
		public virtual string KPIsFolderName { get { return "[KPIs]"; } }
		public virtual string MeasuresFolderName { get { return "[Measures]"; } }
		protected virtual char FolderSeparator { get { return '\\'; } }
		protected virtual char NameSeparator { get { return '.'; } }
		public virtual string GetDimensionName(PivotFieldItemBase field) {
			if(field.KPIType != PivotKPIType.None) return KPIsFolderName;
			string[] nameParts = field.FieldName.Split(NameSeparator);
			return data != null && data.IsOLAP && nameParts.Length >= 2 ? nameParts[0] : null;
		}
		protected string GetHierarchyCaption(string dimensionName) {
			return data != null ? data.GetHierarchyCaption(dimensionName) : null;
		}
		public string GetKPIName(PivotFieldItemBase field) {
			return data.GetKPIName(field.FieldName, field.KPIType);
		}
		public virtual IEnumerable<string> GetFoldersFromPath(string folderPath) {
			return folderPath.Split(FolderSeparator);
		}
		public virtual PivotCustomizationTreeNodeRootBase CreateRootNode() {
			return new PivotCustomizationTreeNodeRootBase();
		}
		public virtual PivotCustomizationTreeNodeFolderBase CreateFolderNode(string folderName) {
			return new PivotCustomizationTreeNodeFolderBase(folderName);
		}
		public virtual PivotCustomizationTreeNodeDimensionBase CreateDimensionNode(string dimensionName) {
			return new PivotCustomizationTreeNodeDimensionBase(dimensionName, dimensionName == MeasuresFolderName, dimensionName == KPIsFolderName, GetHierarchyCaption(dimensionName));
		}
		protected virtual PivotCustomizationTreeNodeMeasureBase CreateMeasureNode(PivotFieldItemBase field) {
			return new PivotCustomizationTreeNodeMeasureBase(field);
		}
		protected virtual PivotCustomizationTreeNodeKpiBase CreateKpiNode(PivotFieldItemBase field) {
			return new PivotCustomizationTreeNodeKpiBase(field);
		}
		protected virtual PivotCustomizationTreeNodeHierarchyBase CreateHierarchyNode(PivotFieldItemBase field) {
			return new PivotCustomizationTreeNodeHierarchyBase(field);
		}
		protected virtual PivotCustomizationTreeNodeAttributeBase CreateAttributeNode(PivotFieldItemBase field) {
			return new PivotCustomizationTreeNodeAttributeBase(field);
		}
		protected string[] GetNames(string name) {
			return CorrectFullName(name.Split('.'));
		}
		protected virtual string[] CorrectFullName(string[] FullName) {
			List<string> list = new List<string>(FullName);
			if(list.Count >= 2)
				list = list.GetRange(0, 2);
			return list.ToArray();
		}
	}
}
