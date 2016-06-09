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
using System.ComponentModel;
using System.Web.UI.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxTreeViewDesigner : ASPxHierarchicalDataWebControlDesigner {
		const string 
			NodeCollectionPropertyName = "Nodes",
			NodeTemplateGroupNamePattern = "Node[{0}]";
		const char NodeTemplateNameLevelSeparator = '.'; 
		readonly string[] NodeTemplatePropertyNames = new string[] { "Template", "TextTemplate" };
		readonly string[] ControlTemplatePropertyNames = new string[] { "NodeTemplate", "NodeTextTemplate" };
		ASPxTreeView treeView = null;
		protected ASPxTreeView TreeView { get { return this.treeView; } }
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			CreateNodeTemplateDefinitions(templateGroups, TreeView.Nodes, string.Empty);
			for (int i = 0; i < ControlTemplatePropertyNames.Length; i++) {
				TemplateGroup templateGroup = new TemplateGroup(ControlTemplatePropertyNames[i]);
				TemplateDefinition templateDefinition = new TemplateDefinition(this,
					ControlTemplatePropertyNames[i], Component, ControlTemplatePropertyNames[i], false);
				templateDefinition.SupportsDataBinding = true;
				templateGroup.AddTemplateDefinition(templateDefinition);
				templateGroups.Add(templateGroup);
			}
			return templateGroups;
		}
		private void CreateNodeTemplateDefinitions(TemplateGroupCollection templateGroups, 
			TreeViewNodeCollection nodes, string namePrefix) {
			for (int i = 0; i < nodes.Count; i++) {
				string groupName = namePrefix + string.Format(NodeTemplateGroupNamePattern, i);
				TemplateGroup templateGroup = new TemplateGroup(groupName);
				for (int j = 0; j < NodeTemplatePropertyNames.Length; j++) {
					TemplateDefinition templateDefinition = new TemplateDefinition(this,
						NodeTemplatePropertyNames[j], nodes[i], NodeTemplatePropertyNames[j], false);
					templateDefinition.SupportsDataBinding = true;
					templateGroup.AddTemplateDefinition(templateDefinition);
				}
				templateGroups.Add(templateGroup);
				CreateNodeTemplateDefinitions(templateGroups, nodes[i].Nodes, 
					groupName + NodeTemplateNameLevelSeparator);
			}
		}
		public override void Initialize(IComponent component) {
			this.treeView = component as ASPxTreeView;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override string GetBaseProperty() {
			return NodeCollectionPropertyName;
		}
		protected internal override string[] GetDataBindingSchemaFields() {
			return new string[] {
				RealModeTreeViewDataMediator.DefaultNavigateUrlFieldName,
				RealModeTreeViewDataMediator.DefaultTextFieldName,
				RealModeTreeViewDataMediator.DefaultToolTipFieldName,
				RealModeTreeViewDataMediator.DefaultImageUrlFieldName,
				RealModeTreeViewDataMediator.DefaultNameFieldName,
				RealModeTreeViewDataMediator.EnabledFieldName,
				RealModeTreeViewDataMediator.CheckedFieldName,
				RealModeTreeViewDataMediator.TargetFieldName
			};
		}
		protected internal override Type GetDataBindingSchemaItemType() {
			return typeof(TreeViewNode);
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxTreeView treeView = dataControl as ASPxTreeView;
			if (!string.IsNullOrEmpty(treeView.DataSourceID) || treeView.DataSource != null ||
				treeView.Nodes.GetVisibleItemCount() == 0) {
				treeView.Nodes.Clear();
				base.DataBind(treeView);
			}
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			propertyNameToCaptionMap.Add(NodeCollectionPropertyName, NodeCollectionPropertyName);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new TreeViewNodesOwner(TreeView, DesignerHost)));
		}
	}
	public class TreeViewNodesOwner : HierarchicalItemOwner<TreeViewNode> {
		public TreeViewNodesOwner(ASPxTreeView treeView, IServiceProvider provider) 
			: base(treeView, provider, treeView.Nodes) {
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Nodes";
		}
	}
}
