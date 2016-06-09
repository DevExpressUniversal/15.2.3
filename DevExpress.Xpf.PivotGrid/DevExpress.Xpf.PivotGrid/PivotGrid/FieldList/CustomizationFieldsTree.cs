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
using System.Linq;
using System.Text;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Data;
using PivotFieldsReadOnlyObservableCollection = System.Collections.ObjectModel.ReadOnlyObservableCollection<DevExpress.Xpf.PivotGrid.PivotGridField>;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class PivotCustomizationFieldsTree : PivotCustomizationFieldsTreeBase {
		public PivotCustomizationFieldsTree(CustomizationFormFields fields, PivotGridWpfData data)
			: base(fields, new PivotCustomizationTreeNodeFactory(data)) { }
		protected override void PopulateFields(System.Collections.IEnumerable fields) {
			if(CoreFields != null)
				base.PopulateFields(CoreFields.Select(f => Fields.FieldItems[f.FieldItem.Index]));
		}
		protected override void LoadExpandedState(PivotCustomizationTreeNodeRootBase oldRoot) {
			if(CoreFields != null)
				base.LoadExpandedState(oldRoot);
		}
		public IVisualCustomizationTreeItem GetRootNode() { return (IVisualCustomizationTreeItem)Root; }
		internal PivotFieldsReadOnlyObservableCollection CoreFields { get; set; }
	}
	public class PivotCustomizationTreeNodeFactory : PivotCustomizationTreeNodeFactoryBase {
		public PivotCustomizationTreeNodeFactory(PivotGridWpfData data)
			: base(data) {
		}
		public override PivotCustomizationTreeNodeRootBase CreateRootNode() {
			return new PivotCustomizationTreeNodeRoot();
		}
		public override PivotCustomizationTreeNodeFolderBase CreateFolderNode(string folderName) {
			return new PivotCustomizationTreeNodeFolder(folderName);
		}
		public override PivotCustomizationTreeNodeDimensionBase CreateDimensionNode(string dimensionName) {
			return new PivotCustomizationTreeNodeDimension(dimensionName, dimensionName == MeasuresFolderName, dimensionName == KPIsFolderName, GetHierarchyCaption(dimensionName));
		}
		protected override PivotCustomizationTreeNodeAttributeBase CreateAttributeNode(PivotFieldItemBase field) {
			return new PivotCustomizationTreeNodeAttribute(field);
		}
		protected override PivotCustomizationTreeNodeHierarchyBase CreateHierarchyNode(PivotFieldItemBase field) {
			return new PivotCustomizationTreeNodeHierarchy(field);
		}
		protected override PivotCustomizationTreeNodeMeasureBase CreateMeasureNode(PivotFieldItemBase field) {
			return new PivotCustomizationTreeNodeMeasure(field);
		}
		protected override PivotCustomizationTreeNodeKpiBase CreateKpiNode(PivotFieldItemBase field) {
			return new PivotCustomizationTreeNodeKpi(field);
		}
	}
	public interface IVisualCustomizationTreeItem : ICustomizationTreeItem {
		string ImageName { get; }
	}
	public static class TreeNodeImageNames {
		const string NamePrefix = "CustomizationTreeNode";
		public const string Measure = NamePrefix + "Measure";
		public const string CalculatedMeasure = NamePrefix + "CalculatedMeasure";
		public const string KPI = NamePrefix + "KPI";
		public const string Dimension = NamePrefix + "Dimension";
		public const string Attribute = NamePrefix + "Attribute";
		public const string Hierarchy = NamePrefix + "Hierarchy";
		public const string FolderClosed = NamePrefix + "FolderClosed";
		public const string FolderOpen = NamePrefix + "FolderOpen";
	}
	#region Custom tree nodes
	public class PivotCustomizationTreeNodeRoot : PivotCustomizationTreeNodeRootBase, IVisualCustomizationTreeItem {
		public string ImageName { get { return string.Empty; } }
	}
	public class PivotCustomizationTreeNodeMeasure : PivotCustomizationTreeNodeMeasureBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeMeasure(PivotFieldItemBase field)
			: base(field) {
		}
		public string ImageName { get { return TreeNodeImageNames.Measure; } }
	}
	public class PivotCustomizationTreeNodeKpi : PivotCustomizationTreeNodeKpiBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeKpi(PivotFieldItemBase field)
			: base(field) {
		}
		public string ImageName { get { return TreeNodeImageNames.KPI; } }
	}
	public class PivotCustomizationTreeNodeDimension : PivotCustomizationTreeNodeDimensionBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeDimension(string name, bool isMeasuresFolder, bool isKPIsFolder, string caption)
			: base(name, isMeasuresFolder, isKPIsFolder, caption) {
		}
		public string ImageName {
			get {
				if(IsMeasuresFolder) return TreeNodeImageNames.Measure;
				return IsKPIFolder ? TreeNodeImageNames.KPI : TreeNodeImageNames.Dimension;
			}
		}
	}
	public class PivotCustomizationTreeNodeAttribute : PivotCustomizationTreeNodeAttributeBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeAttribute(PivotFieldItemBase field)
			: base(field) {
		}
		public string ImageName { get { return TreeNodeImageNames.Attribute; } }
	}
	public class PivotCustomizationTreeNodeHierarchy : PivotCustomizationTreeNodeHierarchyBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeHierarchy(PivotFieldItemBase field)
			: base(field) {
		}
		public string ImageName { get { return TreeNodeImageNames.Hierarchy; } }
	}
	public class PivotCustomizationTreeNodeFolder : PivotCustomizationTreeNodeFolderBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeFolder(string folderName)
			: base(folderName) {
		}
		public string ImageName { get { return IsExpanded ? TreeNodeImageNames.FolderOpen : TreeNodeImageNames.FolderClosed; } }
	}
	#endregion
}
