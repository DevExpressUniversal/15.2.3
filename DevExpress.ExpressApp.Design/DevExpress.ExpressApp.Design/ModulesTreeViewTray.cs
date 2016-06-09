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
using System.ComponentModel;
using System.Windows.Forms;
namespace DevExpress.ExpressApp.Design {
	public class ModulesTreeViewTray : ModulesTreeViewTrayBase<XafApplication> {
		private readonly ModulesTreeBuilder treeBuilder = new ModulesTreeBuilder();
		protected override void OnDataSourceChanged(XafApplication oldDataSource, XafApplication newDataSource) {
			if(oldDataSource != null) {
				oldDataSource.Modules.ListChanged -= new ListChangedEventHandler(Modules_ListChanged);
			}
			base.OnDataSourceChanged(oldDataSource, newDataSource);
			if(newDataSource != null) {
				newDataSource.Modules.ListChanged += new ListChangedEventHandler(Modules_ListChanged);
			}
		}
		private void Modules_ListChanged(object sender, ListChangedEventArgs e) {
			RefreshNodes();
		}
		public ModulesTreeViewTray() { }
		public ModulesTreeViewTray(IContainer container) : base(container) { }
		public override void RefreshNodes() {
			treeBuilder.BuildTree(this, DataSource);
			EnsureModulesInComponentList();
			if(!DataSource.Modules.Contains(SelectedObject as ModuleBase)) {
				SelectedObject = null;
				SelectedNode = null;
			}
			base.RefreshNodes();
		}
		private void EnsureModulesInComponentList() {
			foreach(ModuleBase module in DataSource.Modules) {
				IContainer container = Designer.Component.Site.Container;
				if(module.Site == null) {
					container.Add(module, Designer.NameCreationService.CreateName(container, module.GetType()));
				}
				Designer.TypeResolutionService.ReferenceAssembly(module.GetType().Assembly.GetName());
			}
		}
	}
	public class ModulesTreeBuilder : TreeBuilder<XafApplication> {
		public void BuildTree(TreeView treeView, XafApplication dataSource) {
			ClearTreeViewNodes(treeView);
			if(dataSource != null) {
				foreach(ModuleBase module in dataSource.Modules) {
					TreeNode node = treeView.Nodes.Add(module.Name);
					string imageKey = DesignImagesLoader.GetImageKey(treeView.ImageList, module);
					if(imageKey == "") {
						imageKey = EmbeddedResourceImage.Module.ToString();
					}
					node.ImageKey = imageKey;
					node.SelectedImageKey = node.ImageKey;
					node.Tag = module;
					foreach(Type type in module.RequiredModuleTypes) {
						TreeNode referenceNode = CreateModuleReferenceTreeItem(node.TreeView, type);
						node.Nodes.Add(referenceNode);
					}
				}
				treeView.Sort();
			}
		}
	}
}
