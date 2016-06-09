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
using System.Reflection;
using System.Windows.Forms;
namespace DevExpress.ExpressApp.Design {
	public class ControllersTreeBuilder : TreeBuilder<ModuleBase> {
		private int ControllerAlphabeticalComparision(Controller x, Controller y) {
			return x.GetType().Name.CompareTo(y.GetType().Name);
		}
		private void CollectControllersTree(TreeView treeView, TreeNodeCollection nodes, Assembly assembly) {
			List<Controller> controllerList = ControllersHelper.CollectControllersFromAssembly(assembly);
			controllerList.Sort(ControllerAlphabeticalComparision);
			treeView.BeginUpdate();
			try {
				foreach(Controller controller in controllerList) {
					TreeNode node = nodes.Add(controller.GetType().Name);
					node.Tag = controller;
					node.ImageKey = DesignImagesLoader.GetImageKey(treeView.ImageList, controller);
					node.SelectedImageKey = node.ImageKey;
					node.ToolTipText = controller.GetType().FullName;
					foreach(DevExpress.ExpressApp.Actions.ActionBase action in controller.Actions) {
						TreeNode actionNode = node.Nodes.Add(action.Caption);
						actionNode.Tag = action;
						actionNode.ImageKey = DesignImagesLoader.GetImageKey(treeView.ImageList, action);
						actionNode.SelectedImageKey = actionNode.ImageKey;
						actionNode.ToolTipText = action.Id;
					}
				}
			}
			finally {
				treeView.EndUpdate();
			}
		}
		public void BuildTree(TreeView treeView, ModuleBase dataSource) {
			ClearTreeViewNodes(treeView);
			if(dataSource != null) {
				CollectControllersTree(treeView, treeView.Nodes, dataSource.GetType().Assembly);
			}
		}
		public void BuildTreeWithRequiredModules(TreeView treeView, ModuleBase dataSource, IList<Type> requiredModuleTypes) {
			ClearTreeViewNodes(treeView);
			if(dataSource != null) {
				CollectControllersTree(treeView, treeView.Nodes, dataSource.GetType().Assembly);
				TreeNode requiredModulesControllersNode = new TreeNode("Required modules");
				requiredModulesControllersNode.ImageKey = EmbeddedResourceImage.ModuleLink.ToString();
				requiredModulesControllersNode.SelectedImageKey = requiredModulesControllersNode.ImageKey;
				foreach(Type moduleType in requiredModuleTypes) {
					TreeNode controllerModuleNode = CreateModuleReferenceTreeItem(treeView, moduleType);
					CollectControllersTree(treeView, controllerModuleNode.Nodes, moduleType.Assembly);
					if(controllerModuleNode.Nodes.Count > 0) {
						requiredModulesControllersNode.Nodes.Add(controllerModuleNode);
					}
				}
				if(requiredModulesControllersNode.Nodes.Count > 0) {
					treeView.Nodes.Add(requiredModulesControllersNode);
				}
			}
		}
	}
	public class ControllersTreeViewTray : TreeViewTray<ModuleBase> {
		private ControllersTreeBuilder treeBuilder = new ControllersTreeBuilder();
		public ControllersTreeViewTray() { }
		public ControllersTreeViewTray(IContainer container)
			: base(container) {
			canShowPlaceholder = false;
		}
		public override void RefreshNodes() {
			if(Designer is XafApplicationRootDesigner) {
				treeBuilder.BuildTree(this, DataSource);
			}
			else if(Designer is XafModuleRootDesigner) {
				treeBuilder.BuildTreeWithRequiredModules(this, DataSource, DataSource.RequiredModuleTypes);
			}
			base.RefreshNodes();
		}
	}
}
