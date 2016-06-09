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
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using EnvDTE80;
namespace DevExpress.ExpressApp.Design {
	public class BusinessClassesTreeBuilder : TreeBuilder<ModuleBase> {
		public const string DerivedTypesNodeName = "Derived Types";
		public const string BaseTypesNodeName = "Base Types";
		private int PropertyInfoAlphabeticalComparision(IMemberInfo x, IMemberInfo y) {
			return x.Name.CompareTo(y.Name);
		}
		private int ClassInfoAlphabeticalComparision(Type x, Type y) {
			return x.Name.CompareTo(y.Name);
		}
		private void BuildBaseAndDerivedTypesNodes(TreeView treeView, TreeNode node, ITypeInfo typeInfo) {
			if(typeInfo.Base != null) {
				TreeNode baseTypesNode = CreateBaseTypesTreeNode(typeInfo);
				if(baseTypesNode != null) {
					if(treeView.ImageList != null) {
						baseTypesNode.ImageKey = DesignImagesLoader.GetImageKey(treeView.ImageList, EmbeddedResourceImage.BaseTypesNode);
					}
					baseTypesNode.SelectedImageKey = baseTypesNode.ImageKey;
					node.Nodes.Insert(0, baseTypesNode);
				}
			}
			if(typeInfo.HasDescendants) {
				TreeNode derivedTypesNode = CreateDerivedTypesTreeNode(typeInfo);
				if(derivedTypesNode != null) {
					if(treeView.ImageList != null) {
						derivedTypesNode.ImageKey = DesignImagesLoader.GetImageKey(treeView.ImageList, EmbeddedResourceImage.DerivedTypesNode);
					}
					derivedTypesNode.SelectedImageKey = derivedTypesNode.ImageKey;
					node.Nodes.Insert(1, derivedTypesNode);
				}
			}
		}
		private TreeNode CreateDerivedTypesTreeNode(ITypeInfo typeInfo) {
			TreeNode derivedTypesNode = new TreeNode(DerivedTypesNodeName);
			BuildDerivedTypesNodeRecursive(typeInfo, derivedTypesNode);
			return derivedTypesNode;
		}
		private void BuildDerivedTypesNodeRecursive(ITypeInfo typeInfo, TreeNode parentNode) {
			if(!typeInfo.HasDescendants) {
				return;
			}
			foreach(ITypeInfo info in typeInfo.Descendants) {
				if(info.Base == typeInfo) {
					TreeNode classNode = parentNode.Nodes.Add(info.Type.Name);
					classNode.Tag = info;
					classNode.ImageKey = EmbeddedResourceImage.DomainObject.ToString();
					classNode.SelectedImageKey = classNode.ImageKey;
					BuildDerivedTypesNodeRecursive(info, classNode);
				}
			}
		}
		private TreeNode CreateBaseTypesTreeNode(ITypeInfo typeInfo) {
			TreeNode baseTypesNode = new TreeNode(BaseTypesNodeName);
			BuildBaseTypesNodeRecursive(typeInfo, baseTypesNode);
			return baseTypesNode;
		}
		private void BuildBaseTypesNodeRecursive(ITypeInfo typeInfo, TreeNode parentNode) {
			if(typeInfo.Base == null) {
				return;
			}
			TreeNode classNode = parentNode.Nodes.Add(typeInfo.Base.Type.Name);
			classNode.Tag = typeInfo.Base;
			classNode.ImageKey = EmbeddedResourceImage.DomainObject.ToString();
			classNode.SelectedImageKey = classNode.ImageKey;
			BuildBaseTypesNodeRecursive(typeInfo.Base, classNode);
		}
		private TreeNode CreateAssemblyTreeItem(Assembly assembly) {
			string userFriendlyName = assembly.GetName().Name;
			object[] description = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
			if(description.Length > 0) {
				userFriendlyName = ((AssemblyDescriptionAttribute)description[0]).Description;
			}
			TreeNode assemblyNode = new TreeNode(assembly.GetName().Name);
			assemblyNode.Tag = assembly;
			assemblyNode.Name = assembly.GetName().FullName;
			assemblyNode.ImageKey = EmbeddedResourceImage.PersistentAssembly.ToString();
			assemblyNode.SelectedImageKey = assemblyNode.ImageKey;
			assemblyNode.ToolTipText = userFriendlyName;
			return assemblyNode;
		}
		private void UpdateUsed(TreeView treeView, TreeNodeCollection nodes, IList<Type> used) {
			foreach(Type ci in used) {
				TreeNode[] usedNodes = nodes.Find(ci.FullName, true);
				if(usedNodes.Length == 1) {
					UpdatePersistentNodeChecked(treeView, usedNodes[0], true);
				}
				else {
					Tracing.Tracer.LogWarning("Update used warning [" + usedNodes.Length + "] " + ci.FullName);
				}
			}
		}
		private void UpdatePersistentNodeChecked(TreeView treeView, TreeNode node, bool checkState) {
			node.Checked = checkState;
			if(checkState) {
				node.NodeFont = new Font(treeView.Font, FontStyle.Bold);
				treeView.Invalidate(node.Bounds);
			}
			else {
				node.NodeFont = null;
			}
		}
		private void CollectAllPersistentObjectsTree(TreeView treeView, TreeNodeCollection nodes, ModuleBase compiledModule, IList<Assembly> referencedAssemblies) {
			CollectPersistentObjectsOfAssemblyTree(treeView, nodes, compiledModule.GetType().Assembly, compiledModule.IsExportedType);
			TreeNode referencedAssembliesNode = new TreeNode("Referenced assemblies");
			referencedAssembliesNode.ImageKey = EmbeddedResourceImage.PersistentAssembly.ToString();
			referencedAssembliesNode.SelectedImageKey = referencedAssembliesNode.ImageKey;
			foreach(Assembly assembly in referencedAssemblies) {
				TreeNode assemblyNode = CreateAssemblyTreeItem(assembly);
				CollectPersistentObjectsOfAssemblyTree(treeView, assemblyNode.Nodes, assembly, compiledModule.IsExportedType);
				if(assemblyNode.Nodes.Count > 0) {
					referencedAssembliesNode.Nodes.Add(assemblyNode);
				}
			}
			if(referencedAssembliesNode.Nodes.Count > 0) {
				nodes.Add(referencedAssembliesNode);
			}
			UpdateUsed(treeView, nodes, ExportedTypeHelper.GetExportedTypes(compiledModule));
		}
		private void CollectUsedPersistentObjectsTree(TreeView treeView, TreeNodeCollection nodes, ModuleBase module, bool markUsed) {
			treeView.BeginUpdate();
			List<Type> types = ExportedTypeHelper.GetExportedTypes(module);
			int start = nodes.Count;
			CollectPersistentObjectsTree(treeView, nodes, types);
			if(markUsed) {
				for(int i = start; i < nodes.Count; i++) {
					UpdatePersistentNodeChecked(treeView, nodes[i], true);
				}
			}
			treeView.EndUpdate();
		}
		private void CollectPersistentObjectsOfAssemblyTree(TreeView treeView, TreeNodeCollection nodes, Assembly assembly, Predicate<Type> isExportedType) {
			List<Type> exportedTypes = ExportedTypeHelper.CollectExportedTypesFromAssembly(assembly, isExportedType);
			if(exportedTypes.Count > 0) {
				CollectPersistentObjectsTree(treeView, nodes, exportedTypes);
			}
		}
		private void CollectPersistentObjectsTree(TreeView treeView, TreeNodeCollection nodes, List<Type> persistentTypeList) {
			persistentTypeList.Sort(ClassInfoAlphabeticalComparision);
			treeView.BeginUpdate();
			foreach(Type type in persistentTypeList) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
				if(typeInfo.IsVisible) {
					TreeNode node = nodes.Add(typeInfo.Type.Name);
					node.Name = typeInfo.Type.FullName;
					node.Tag = typeInfo;
					node.ImageKey = EmbeddedResourceImage.DomainObject.ToString();
					node.SelectedImageKey = node.ImageKey;
					node.ToolTipText = typeInfo.Type.GetType().FullName;
					BuildBaseAndDerivedTypesNodes(treeView, node, typeInfo);
					List<IMemberInfo> propertyList = new List<IMemberInfo>(typeInfo.OwnMembers);
					propertyList.Sort(PropertyInfoAlphabeticalComparision);
					foreach(IMemberInfo mi in propertyList) {
						if(mi.IsPublic) {
							TreeNode memberNode = node.Nodes.Add(mi.Name + " : " + mi.MemberType.Name);
							memberNode.Tag = mi;
							memberNode.ImageKey = EmbeddedResourceImage.Property.ToString();
							memberNode.SelectedImageKey = memberNode.ImageKey;
						}
					}
				}
			}
			treeView.EndUpdate();
		}
		public void BuildTree(TreeView treeView, ModuleBase dataSource) {
			ClearTreeViewNodes(treeView);
			if(dataSource != null) {
				CollectUsedPersistentObjectsTree(treeView, treeView.Nodes, dataSource, false);
			}
		}
		public void BuildTreeWithReferencedAssemblies(TreeView treeView, ModuleBase dataSource, List<Assembly> referencedAssemblies) {
			ClearTreeViewNodes(treeView);
			if(dataSource != null) {
				CollectAllPersistentObjectsTree(treeView, treeView.Nodes, dataSource, referencedAssemblies);
				TreeNode requiredModulesNode = new TreeNode("Required modules");
				requiredModulesNode.ImageKey = EmbeddedResourceImage.ModuleLink.ToString();
				requiredModulesNode.SelectedImageKey = requiredModulesNode.ImageKey;
				foreach(Type moduleType in dataSource.RequiredModuleTypes) {
					TreeNode objectsModuleNode = CreateModuleReferenceTreeItem(treeView, moduleType);
					ModuleBase refModule = (ModuleBase)ReflectionHelper.CreateObject(moduleType);
					CollectUsedPersistentObjectsTree(treeView, objectsModuleNode.Nodes, refModule, true);
					if(objectsModuleNode.Nodes.Count > 0) {
						UpdatePersistentNodeChecked(treeView, objectsModuleNode, true);
						requiredModulesNode.Nodes.Add(objectsModuleNode);
					}
				}
				if(requiredModulesNode.Nodes.Count > 0) {
					treeView.Nodes.Add(requiredModulesNode);
				}
			}
		}
	}
	public class SwitchExportedTypeUsageEventArgs : EventArgs {
		private Type exportedType;
		public SwitchExportedTypeUsageEventArgs(Type exportedType) {
			this.exportedType = exportedType;
		}
		public Type ExportedType {
			get {
				return exportedType;
			}
		}
	}
	public class SwitchAssemblyUsageEventArgs : EventArgs {
		private Assembly assembly;
		public SwitchAssemblyUsageEventArgs(Assembly assembly) {
			this.assembly = assembly;
		}
		public Assembly Assembly {
			get {
				return assembly ;
			}
		}
	}
	public delegate void SwitchExportedTypeUsageEventHandler(object sender, SwitchExportedTypeUsageEventArgs e);
	public delegate void SwitchAssemblyUsageEventHandler(object sender, SwitchAssemblyUsageEventArgs e);
	public class BusinessClassesTreeViewTray : TreeViewTray<ModuleBase> {
		private BusinessClassesTreeBuilder treeBuilder = new BusinessClassesTreeBuilder();
		private void UpdateNodeCheckedState(TreeNode node, Boolean checkState) {
			if(checkState) {
				node.NodeFont = new Font(Font, FontStyle.Bold);
				node.Checked = true;
			}
			else {
				node.NodeFont = null;
				node.Checked = false;
			}
		}
		private void UpdateTypeNodeState(TreeNode node, IList<Type> exportedTypes) {
			ITypeInfo typeInfo = node.Tag as ITypeInfo;
			if(typeInfo != null) {
				UpdateNodeCheckedState(node, exportedTypes.Contains(typeInfo.Type));
			}
		}
		private void UpdateAssemblyNodeCheckedState(TreeNode node, IList<Type> exportedTypes) {
			Assembly assembly = node.Tag as Assembly;
			if(assembly == null) {
				return;
			}
			BeginUpdate();
			UpdateNodeCheckedState(node, ExportedTypeHelper.IsRegisteredAssembly(assembly, DataSource.IsExportedType, exportedTypes));
			foreach(TreeNode childNode in node.Nodes) {
				UpdateTypeNodeState(childNode, exportedTypes);
			}
			EndUpdate();
		}
		private void UpdateTreeNodesCheckedState(TreeNodeCollection nodes, IList<Type> exportedTypes) {
			BeginUpdate();
			foreach(TreeNode node in nodes) {
				if(CanSwitchNodeUsing(node)) {
					ITypeInfo typeInfo = node.Tag as ITypeInfo;
					if(typeInfo != null) {
						UpdateTypeNodeState(node, exportedTypes);
					}
					else if(node.Tag is Assembly) {
						UpdateAssemblyNodeCheckedState(node, exportedTypes);
					}
				}
				else if(node.Nodes.Count > 0) {
					UpdateTreeNodesCheckedState(node.Nodes, exportedTypes);
				}
			}
			EndUpdate();
		}
		public bool IsBuilding {
			get {
				if(Designer != null) {
					DTE2 dte2 = Designer.DTE as DTE2;
					if(dte2 != null && dte2.Solution != null) {
						EnvDTE80.SolutionBuild2 solutionBuild = (EnvDTE80.SolutionBuild2)dte2.Solution.SolutionBuild;
						return solutionBuild.BuildState == EnvDTE.vsBuildState.vsBuildStateInProgress;
					}
				}
				return false;
			}
		}
		private Boolean CanSwitchNodeUsing(TreeNode node) {
			if(IsBuilding) {return false;}
			if(node == null) { return false; }
			if(node.Tag is Assembly) { return true; }
			if(!(node.Tag is ITypeInfo)) { return false; }
			if(node.Parent == null) { return true; }
			if(node.Parent.Tag is Assembly) { return true; }
			return false;
		}
		private void OnSwitchExportedTypeUsage(Type classType) {
			if(SwitchExportedTypeUsage != null) {
				SwitchExportedTypeUsage(this, new SwitchExportedTypeUsageEventArgs(classType));
			}
		}
		private void OnSwitchAssemblyUsage(Assembly assembly) {
			if(SwitchAssemblyUsage != null) {
				SwitchAssemblyUsage(this, new SwitchAssemblyUsageEventArgs(assembly));
			}
		}
		private void DataSourceListChanged(object sender, EventArgs e) {
			UpdateTreeNodesCheckedState(Nodes, ExportedTypeHelper.GetExportedTypes(DataSource));
		}
		protected TreeNode FindClassNode(TreeNodeCollection nodes, string classTypeName) {
			TreeNode result = null;
			foreach(TreeNode node in nodes) {
				if((node.Tag != null) && (node.Tag is ITypeInfo) && (((ITypeInfo)node.Tag).Type.FullName == classTypeName)) {
					result = node;
				}
				else if((node.Text != BusinessClassesTreeBuilder.BaseTypesNodeName) && (node.Text != BusinessClassesTreeBuilder.DerivedTypesNodeName)) {
					result = FindClassNode(node.Nodes, classTypeName);
				}
				if(result != null) {
					node.Expand();
					break;
				}
			}
			return result;
		}
		protected override IComponent GetComponentCore(object obj) {
			bool canUse = false;
			if(SelectedNode != null && SelectedNode.Tag == obj) {
				canUse = CanSwitchNodeUsing(SelectedNode);
			}
			if(obj is ITypeInfo) {
				return new XPClassProperiesProvider((ITypeInfo)obj, canUse);
			}
			else if(obj is IMemberInfo) {
				return new XPMemberProperiesProvider((IMemberInfo)obj);
			}
			else if(obj is Assembly) {
				return new AssemblyPropertiesProvider((Assembly)obj, canUse);
			}
			return base.GetComponentCore(obj);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			if(e.KeyChar == ' ') {
				if(CanSwitchNodeUsing(SelectedNode)) {
					if(SelectedNode.Tag is ITypeInfo) {
						OnSwitchExportedTypeUsage(((ITypeInfo)SelectedNode.Tag).Type);
					}
					else if(SelectedNode.Tag is Assembly) {
						OnSwitchAssemblyUsage((Assembly)SelectedNode.Tag);
					}
				}
				e.Handled = true;
			}
			base.OnKeyPress(e);
		}
		protected override void OnDataSourceChanged(ModuleBase oldDataSource, ModuleBase newDataSource) {
			if(oldDataSource != null) {
				oldDataSource.AdditionalExportedTypes.Changed -= new EventHandler(DataSourceListChanged);
			}
			if(newDataSource != null) {
				newDataSource.AdditionalExportedTypes.Changed += new EventHandler(DataSourceListChanged);
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(DataSource != null) {
					DataSource.AdditionalExportedTypes.Changed -= new EventHandler(DataSourceListChanged);
				}
				SwitchExportedTypeUsage = null;
				SwitchAssemblyUsage = null;
			}
			base.Dispose(disposing);
		}
		public BusinessClassesTreeViewTray() { }
		public BusinessClassesTreeViewTray(IContainer container)
			: base(container) {
			canShowPlaceholder = false;
		}
		public TreeNode FindClassNode(string classTypeName) {
			return FindClassNode(this.Nodes, classTypeName);
		}
		public override void RefreshNodes() {
			if(Designer is XafApplicationRootDesigner) {
				treeBuilder.BuildTree(this, DataSource);
			}
			else if(Designer is XafModuleRootDesigner) {
				ITypeDiscoveryService typeDiscoveryService = (ITypeDiscoveryService)Designer.Component.Site.GetService(typeof(ITypeDiscoveryService));
				List<Assembly> referencedAssemblies = new List<Assembly>();
				if(typeDiscoveryService != null) {
					foreach(Type type in typeDiscoveryService.GetTypes(typeof(object), false)) {
						Assembly assembly = type.Assembly;
						if(!referencedAssemblies.Contains(assembly) && assembly != DataSource.GetType().Assembly) {
							bool isInModulesList = false;
							foreach(Type moduleType in DataSource.RequiredModuleTypes) {
								if(moduleType.Assembly == assembly) {
									isInModulesList = true;
									break;
								}
							}
							if(!isInModulesList) {
								referencedAssemblies.Add(assembly);
							}
						}
					}
				}
				treeBuilder.BuildTreeWithReferencedAssemblies(this, DataSource, referencedAssemblies);
				UpdateTreeNodesCheckedState(Nodes, ExportedTypeHelper.GetExportedTypes(DataSource));
				Invalidate();
			}
			base.RefreshNodes();
		}
		public event SwitchExportedTypeUsageEventHandler SwitchExportedTypeUsage;
		public event SwitchAssemblyUsageEventHandler SwitchAssemblyUsage;
	}
}
