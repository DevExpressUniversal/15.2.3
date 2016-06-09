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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design;
using System.Reflection;
using DevExpress.ExpressApp.Core;
using DevExpress.Xpo.Metadata;
using DevExpress.ExpressApp.Actions;
using System.Collections;
using DevExpress.ExpressApp.Security;
using System.Data.Common;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Design.Core;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Design {
	public partial class XafApplicationRootDesignerGUI : UserControl, IDesignerGUI, IEditorNavigation {
		private XafApplicationRootDesigner designer;
		private void RefreshSelectedModulePanel() {
			ModuleBase selectedModule = modulesTreeView.SelectedObject as ModuleBase;
			controllersTreeView.DataSource = selectedModule;
			businessClassesTreeView.DataSource = selectedModule;
			if(selectedModule == null) {
				moduleNameLabel.Text = "Module: ";
				moduleAssemblyLabel.Text = "Assembly: ";
				moduleDescriptionLabel.Text = "Description: ";
				return;
			}
			moduleNameLabel.Text = "Module: " + selectedModule.Name;
			moduleAssemblyLabel.Text = "Assembly: " + selectedModule.AssemblyName;
			moduleDescriptionLabel.Text = "Description: " + selectedModule.Description;
		}
		private void SelectionService_SelectionChanged(object sender, EventArgs e) {
			if(designer.SelectionService.PrimarySelection is ModuleBase) {
				RefreshSelectedModulePanel();
			}
			if(FocusChanged != null) {
				FocusChanged(this, EventArgs.Empty);
			}
		}
		private void Tray_Enter(object sender, EventArgs e) {
			applicationTray.SetHideSelection(sender);
			securityTray.SetHideSelection(sender);
			connectionTray.SetHideSelection(sender);
			businessClassesTreeView.SetHideSelection(sender);
			controllersTreeView.SetHideSelection(sender);
			modulesTreeView.SetHideSelection(sender);
		}
		public XafApplicationRootDesignerGUI() {
			InitializeComponent();
			this.applicationTray.Enter += new EventHandler(Tray_Enter);
			this.securityTray.Enter += new EventHandler(Tray_Enter);
			this.connectionTray.Enter += new EventHandler(Tray_Enter);
			this.businessClassesTreeView.Enter += new EventHandler(Tray_Enter);
			this.controllersTreeView.Enter += new EventHandler(Tray_Enter);
			this.modulesTreeView.Enter += new EventHandler(Tray_Enter);
		}
		public void Initialize(XafApplicationRootDesigner designer) {
			this.designer = designer;
			applicationTray.Designer = designer;
			applicationTray.DataSource = designer.Application;
			securityTray.Designer = designer;
			securityTray.DataSource = designer.Application;
			connectionTray.Designer = designer;
			connectionTray.DataSource = designer.Application;
			modulesTreeView.Designer = designer;
			modulesTreeView.DataSource = designer.Application;
			businessClassesTreeView.Designer = designer;
			businessClassesTreeView.DataSource = null;
			controllersTreeView.Designer = designer;
			controllersTreeView.DataSource = null;
			designer.SelectionService.SelectionChanged += new EventHandler(SelectionService_SelectionChanged);
			DesignImagesLoader.AddResourceImage(modulesImageList, EmbeddedResourceImage.Module);
			DesignImagesLoader.AddResourceImage(modulesImageList, EmbeddedResourceImage.ModuleLink);
			DesignImagesLoader.AddResourceImage(persistentObjectsImageList, EmbeddedResourceImage.DomainObject);
			DesignImagesLoader.AddResourceImage(persistentObjectsImageList, EmbeddedResourceImage.Property);
		}
		void IEditorNavigation.NavigateTo(string text) {
			string[] parts = text.Split(':');
			string paramName = parts[0].Trim();
			string paramString = parts[1].Trim();
			SelectModuleNodeByClassName(paramString);
			switch(paramName) {
				case EditorNavigationConsts.ControllerKeyName: {
						controllersTreeView.Focus();
						controllersTreeView.Select();
						TreeNode node = controllersTreeView.FindNode(paramString);
						if(node != null) {
							controllersTreeView.SelectedNode = node;
						}
						else {
							throw new ArgumentException("The '" + parts[1].Trim() + "' controller was not found");
						}
						break;
					}
				case EditorNavigationConsts.ActionKeyName: {
						controllersTreeView.Focus();
						controllersTreeView.Select();
						bool isFound = false;
						string[] actionPath = paramString.Split(';');
						string controllerTypeName = actionPath[0].Trim();
						string actionId = actionPath[1].Trim();
						TreeNode node = controllersTreeView.FindNode(controllerTypeName);
						if(node != null) {
							controllersTreeView.SelectedNode = node;
							node.Expand();
							foreach(TreeNode actionNode in node.Nodes) {
								if((actionNode.Tag != null) && (((ActionBase)actionNode.Tag).Id == actionId)) {
									controllersTreeView.SelectedNode = actionNode;
									isFound = true;
									break;
								}
							}
						}
						if(!isFound) {
							throw new ArgumentException("The '" + paramString + "' action was not found");
						}
						break;
					}
				case EditorNavigationConsts.ModuleIconKeyName: {
						break;
					}
				case EditorNavigationConsts.BusinessClassKeyName: {
						businessClassesTreeView.Focus();
						businessClassesTreeView.Select();
						TreeNode node = businessClassesTreeView.FindClassNode(paramString);
						if(node != null) {
							businessClassesTreeView.SelectedNode = node;
						}
						else {
							throw new ArgumentException("The '" + parts[1].Trim() + "' persistent class was not found");
						}
						break;
					}
				case EditorNavigationConsts.BusinessClassMemberKeyName: {
						businessClassesTreeView.Focus();
						businessClassesTreeView.Select();
						bool isFound = false;
						string[] memberPath = paramString.Split(';');
						string classTypeName = memberPath[0].Trim();
						string memberName = memberPath[1].Trim();
						TreeNode node = businessClassesTreeView.FindClassNode(classTypeName);
						if(node != null) {
							node.Expand();
							foreach(TreeNode memberNode in node.Nodes) {
								if((memberNode.Tag != null) && (((IMemberInfo)memberNode.Tag).Name == memberName)) {
									businessClassesTreeView.SelectedNode = memberNode;
									isFound = true;
									break;
								}
							}
						}
						if(!isFound) {
							throw new ArgumentException("The '" + paramString + "' member was not found");
						}
						break;
					}
				case EditorNavigationConsts.ReferencedModuleKeyName: {
						throw new Exception("Not supported");
					}
				default: {
						throw new ArgumentException("Unknown value: '" + text + "'", "text");
					}
			}
		}
		private void SelectModuleNodeByClassName(string paramString) {
			Type classType = Type.GetType(paramString);
			TreeNode targetModuleNode = null;
			foreach (TreeNode nodeM in this.modulesTreeView.Nodes) {
				foreach(Type type in ((ModuleBase)nodeM.Tag).AdditionalExportedTypes) {
					if(type.FullName == classType.FullName) {
						targetModuleNode = nodeM;
						break;
					}
				}
				if(targetModuleNode != null) { break; }
			}
			if (targetModuleNode != null) {
				targetModuleNode.TreeView.SelectedNode = targetModuleNode;
			}
		}
		string IEditorNavigation.GetState() {
			if(designer.SelectionService.PrimarySelection is ActionBase) {
				ActionBase action = (ActionBase)designer.SelectionService.PrimarySelection;
				return EditorNavigationConsts.ActionKeyName + ": " + action.Controller.GetType().FullName + ';' + action.Id;
			}
			else if(designer.SelectionService.PrimarySelection is Controller) {
				Controller controller = (Controller)designer.SelectionService.PrimarySelection;
				return EditorNavigationConsts.ControllerKeyName + ": " + controller.GetType().FullName;
			}
			else if(designer.SelectionService.PrimarySelection is XPMemberProperiesProvider) {
				XPMemberProperiesProvider memberProperiesProvider = (XPMemberProperiesProvider)designer.SelectionService.PrimarySelection;
				return EditorNavigationConsts.BusinessClassMemberKeyName + ": " + memberProperiesProvider.OwnerClassType.FullName + ";" + memberProperiesProvider.Name;
			}
			else if(designer.SelectionService.PrimarySelection is XPClassProperiesProvider) {
				XPClassProperiesProvider classProperiesProvider = (XPClassProperiesProvider)designer.SelectionService.PrimarySelection;
				return EditorNavigationConsts.BusinessClassKeyName + ": " + classProperiesProvider.FullName;
			}
			else if(designer.SelectionService.PrimarySelection is ModuleBase) {
				ModuleBase module = (ModuleBase)designer.SelectionService.PrimarySelection;
				return EditorNavigationConsts.ReferencedModuleKeyName + ": " + module.GetType().FullName;
			}
			return "";
		}
		void IEditorNavigation.SetValue(string name, object value) {
		}
		public bool ContainsComponent(IComponent component) {
			if(modulesTreeView.ContainsComponent(component) || securityTray.ContainsComponent(component) || applicationTray.ContainsComponent(component)) {
				return true;
			}
			return false;
		}
		public event EventHandler FocusChanged;
		public BusinessClassesTreeViewTray BusinessClassesTreeView {
			get { return businessClassesTreeView; }
		}
		public ModulesTreeViewTray ModulesTreeView {
			get { return modulesTreeView; }
		}
		public ControllersTreeViewTray ControllersTreeView {
			get { return controllersTreeView; }
		}
		public ConnectionTray ConnectionTray {
			get { return connectionTray; }
		}
		public SecurityTray SecurityTray {
			get { return securityTray; }
		}
		public ApplicationTray ApplicationTray {
			get { return applicationTray; }
		}
	}
}
