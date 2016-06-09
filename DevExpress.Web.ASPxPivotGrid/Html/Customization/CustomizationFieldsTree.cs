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

using DevExpress.XtraPivotGrid.Customization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.Data;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public class PivotCustomizationFieldsTree : PivotCustomizationFieldsTreeBase {
		public PivotCustomizationFieldsTree(CustomizationFormFields fields, PivotGridWebData data)
			: base(fields, new PivotCustomizationTreeNodeFactory(data)) {
		}
		public new PivotCustomizationTreeNodeRoot Root { get { return (PivotCustomizationTreeNodeRoot)base.Root; } }
		protected override void PopulateFields(IEnumerable fields) {
			base.PopulateFields(Fields.FieldItems.Where(item => item.CanShowInCustomizationForm && (item.Group == null || item.InnerGroupIndex == 0)));
		}
	}
	public class PivotCustomizationTreeNodeFactory : PivotCustomizationTreeNodeFactoryBase {
		public PivotCustomizationTreeNodeFactory(PivotGridWebData data)
			: base(data) {
		}
		protected override PivotCustomizationTreeNodeAttributeBase CreateAttributeNode(XtraPivotGrid.Data.PivotFieldItemBase field) {
			return new PivotCustomizationTreeNodeAttribute(field);
		}
		public override PivotCustomizationTreeNodeDimensionBase CreateDimensionNode(string dimensionName) {
			return new PivotCustomizationTreeNodeDimension(dimensionName, dimensionName == MeasuresFolderName, dimensionName == KPIsFolderName, GetHierarchyCaption(dimensionName));
		}
		public override PivotCustomizationTreeNodeFolderBase CreateFolderNode(string folderName) {
			return new PivotCustomizationTreeNodeFolder(folderName);
		}
		protected override PivotCustomizationTreeNodeHierarchyBase CreateHierarchyNode(PivotFieldItemBase field) {
			return new PivotCustomizationTreeNodeHierarchy(field);
		}
		protected override PivotCustomizationTreeNodeKpiBase CreateKpiNode(PivotFieldItemBase field) {
			return new PivotCustomizationTreeNodeKpi(field);
		}
		protected override PivotCustomizationTreeNodeMeasureBase CreateMeasureNode(PivotFieldItemBase field) {
			return new PivotCustomizationTreeNodeMeasure(field);
		}
		public override PivotCustomizationTreeNodeRootBase CreateRootNode() {
			return new PivotCustomizationTreeNodeRoot();
		}
	}
	public interface IVisualCustomizationTreeItem : ICustomizationTreeItem {
		string CssClass { get; }
	}
	public static class CustomizationTreeNodeCssClasses {
		const string CssClassPrefix = "dxPivotGrid_CTN";
		public const string Measure = CssClassPrefix + "Measure";
		public const string Dimension = CssClassPrefix + "Dimension";
		public const string Attribute = CssClassPrefix + "Attribute";
		public const string Hierarchy = CssClassPrefix + "Hierarchy";
		public const string KPI = CssClassPrefix + "Kpi";
		public const string FolderClosed = CssClassPrefix + "FolderClosed";
		public const string FolderOpen = CssClassPrefix + "FolderOpen";
		public const string CalculatedMeasure = CssClassPrefix + "CalculatedMeasure";
	}
	#region Custom nodes
	public class PivotCustomizationTreeNodeAttribute : PivotCustomizationTreeNodeAttributeBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeAttribute(PivotFieldItemBase field) : base(field) { }
		public string CssClass { get { return CustomizationTreeNodeCssClasses.Attribute; } }
	}
	public class PivotCustomizationTreeNodeDimension : PivotCustomizationTreeNodeDimensionBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeDimension(string name, string caption) : base(name, caption) { }
		public PivotCustomizationTreeNodeDimension(string name, bool isMeasuresFolder, bool isKPIsFolder, string caption) : base(name, isMeasuresFolder, isKPIsFolder, caption) { }
		public string CssClass {
			get {
				if(IsMeasuresFolder) return CustomizationTreeNodeCssClasses.Measure;
				return IsKPIFolder ? CustomizationTreeNodeCssClasses.KPI : CustomizationTreeNodeCssClasses.Dimension;
			}
		}
	}
	public class PivotCustomizationTreeNodeFolder : PivotCustomizationTreeNodeFolderBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeFolder(string folderName) : base(folderName) { }
		public string CssClass { get { return IsExpanded ? CustomizationTreeNodeCssClasses.FolderOpen : CustomizationTreeNodeCssClasses.FolderClosed; } }
	}
	public class PivotCustomizationTreeNodeHierarchy : PivotCustomizationTreeNodeHierarchyBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeHierarchy(PivotFieldItemBase field) : base(field) { }
		public string CssClass { get { return CustomizationTreeNodeCssClasses.Hierarchy; } }
	}
	public class PivotCustomizationTreeNodeKpi : PivotCustomizationTreeNodeKpiBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeKpi(PivotFieldItemBase field) : base(field) { }
		public string CssClass { get { return CustomizationTreeNodeCssClasses.KPI; } }
	}
	public class PivotCustomizationTreeNodeMeasure : PivotCustomizationTreeNodeMeasureBase, IVisualCustomizationTreeItem {
		public PivotCustomizationTreeNodeMeasure(PivotFieldItemBase field) : base(field) { }
		public string CssClass { get { return CustomizationTreeNodeCssClasses.Measure; } }
	}
	public class PivotCustomizationTreeNodeRoot : PivotCustomizationTreeNodeRootBase, IVisualCustomizationTreeItem {
		public string CssClass { get { return string.Empty; } }
	}
	#endregion
}
