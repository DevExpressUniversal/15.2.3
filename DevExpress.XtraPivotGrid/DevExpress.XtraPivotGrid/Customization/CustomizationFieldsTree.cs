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
namespace DevExpress.XtraPivotGrid.Customization {
	public class PivotCustomizationFieldsTree : PivotCustomizationFieldsTreeBase {
		public PivotCustomizationFieldsTree(CustomizationFormFields fields, PivotGridData data)
			: base(fields, new PivotCustomizationTreeNodeFactory(data)) {
		}		 
	}
	public class PivotCustomizationTreeNodeFactory : PivotCustomizationTreeNodeFactoryBase {
		public PivotCustomizationTreeNodeFactory(PivotGridData data)
			: base(data) {
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
		int ImageIndex { get; }
	}
	#region Custom tree nodes
	public class PivotCustomizationTreeNodeMeasure : PivotCustomizationTreeNodeMeasureBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeMeasure(PivotFieldItemBase field)
			: base(field) {
		}
		public int ImageIndex { get { return 0; } }
	}
	public class PivotCustomizationTreeNodeKpi : PivotCustomizationTreeNodeKpiBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeKpi(PivotFieldItemBase field)
			: base(field) {
		}
		public int ImageIndex { get { return 4; } }
	}
	public class PivotCustomizationTreeNodeDimension : PivotCustomizationTreeNodeDimensionBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeDimension(string name, bool isMeasuresFolder, bool isKPIsFolder, string caption)
			: base(name, isMeasuresFolder, isKPIsFolder, caption) {
		}
		public int ImageIndex {
			get {
				if(IsMeasuresFolder) return 0;
				return IsKPIFolder ? 4 : 1;
			}
		}
	}
	public class PivotCustomizationTreeNodeAttribute : PivotCustomizationTreeNodeAttributeBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeAttribute(PivotFieldItemBase field)
			: base(field) {
		}
		public int ImageIndex { get { return 2; } }
	}
	public class PivotCustomizationTreeNodeHierarchy : PivotCustomizationTreeNodeHierarchyBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeHierarchy(PivotFieldItemBase field)
			: base(field) {
		}
		public int ImageIndex { get { return 3; } }
	}
	public class PivotCustomizationTreeNodeFolder : PivotCustomizationTreeNodeFolderBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeFolder(string folderName)
			: base(folderName) {
		}
		public int ImageIndex { get { return IsExpanded ? 6 : 5; } }
	}
	#endregion
}
