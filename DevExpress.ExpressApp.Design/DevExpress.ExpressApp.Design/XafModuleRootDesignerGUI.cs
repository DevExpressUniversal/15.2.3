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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Design.Core;
namespace DevExpress.ExpressApp.Design {
	public partial class XafModuleRootDesignerGUI : UserControl, IEditorNavigation {
		private string oldModuleName;
		private XafModuleRootDesigner designer;
		private void control_MouseUp(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Right) {
				designer.ShowContextMenu(((Control)sender).PointToScreen(new Point(e.X, e.Y)));
			}
		}
		private void moduleListView_MouseClick(object sender, MouseEventArgs e) {
			designer.SelectionService.SetSelectedComponents(
				new IComponent[] { designer.Module }, SelectionTypes.Replace);
		}
		private void moduleListView_AfterLabelEdit(object sender, LabelEditEventArgs e) {
			if(e.Label == null) {
				return;
			}
			try {
				using(DesignerTransaction trans = designer.DesignerHost.CreateTransaction("Changing " + designer.Component.Site.Name)) {
					designer.ComponentChangeService.OnComponentChanging(designer.Component, null);
					designer.Component.Site.Name = e.Label;
					designer.ComponentChangeService.OnComponentChanged(designer.Component, null, null, null);
					trans.Commit();
				}
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				if(!string.IsNullOrEmpty(oldModuleName) && designer.Component.Site.Name != oldModuleName) {
					designer.Component.Site.Name = oldModuleName;
				}
				oldModuleName = "";
			}
		}
		private void moduleListView_BeforeLabelEdit(object sender, LabelEditEventArgs e) {
			oldModuleName = e.Label;
		}
		private void SelectionService_SelectionChanged(object sender, EventArgs e) {
			if(FocusChanged != null) {
				FocusChanged(this, EventArgs.Empty);
			}
		}
		private void modulesTreeView_DragDrop(object sender, DragEventArgs e) {
			RefreshView();
		}
		private void Tray_Enter(object sender, EventArgs e) {
			modulesTreeView.SetHideSelection(sender);
			businessClassesTreeView.SetHideSelection(sender);
			controllersTreeView.SetHideSelection(sender);
			bool moduleListViewHideSelection = sender != moduleListView;
			if(moduleListViewHideSelection != moduleListView.HideSelection) {
				moduleListView.HideSelection = moduleListViewHideSelection;
			}
		}
		private void RefreshView() {
			this.modulesTreeView.RefreshNodes();
			this.businessClassesTreeView.RefreshNodes();
			this.controllersTreeView.RefreshNodes();
		}
		private void OnMenuDelete(object sender, EventArgs e) {
			IDesignerHost host = designer.DesignerHost;
			ISelectionService selectionService = designer.SelectionService;
			if(host != null && selectionService != null && selectionService.SelectionCount > 0) {
				using(DesignerTransaction trans = host.CreateTransaction("Delete " + selectionService.SelectionCount + " component(s)")) {
					foreach(object obj in selectionService.GetSelectedComponents()) {
						if(obj is ModuleBase && obj != designer.Module) {
							if(designer.Module.RequiredModuleTypes.Contains(obj.GetType())) {
								PropertyDescriptor member = TypeDescriptor.GetProperties(designer.Module)["RequiredModuleTypes"];
								designer.ComponentChangeService.OnComponentChanging(designer.Component, member);
								designer.Module.RequiredModuleTypes.Remove(obj.GetType());
								designer.CompiledModule.RequiredModuleTypes.Remove(obj.GetType());
								designer.ComponentChangeService.OnComponentChanged(designer.Component, member, null, null);
							}
							else {
								MessageBox.Show(string.Format("Cannot remove the {0} module because it's referenced in the current module.", obj.GetType().Name), "Unable to remove module", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
						}
					}
					trans.Commit();
					RefreshView();
				}
			}
		}
		private void objectsTreeView_SwitchAssemblyUsage(object sender, SwitchAssemblyUsageEventArgs e) {
			designer.SwitchAssemblyUsage(e.Assembly);
		}
		private void objectsTreeView_SwitchExportedTypeUsage(object sender, SwitchExportedTypeUsageEventArgs e) {
			designer.SwitchExportedTypeUsage(e.ExportedType);
		}
		protected void RaiseFocusChanged() {
			if(FocusChanged != null) {
				FocusChanged(this, EventArgs.Empty);
			}
		}
		public XafModuleRootDesignerGUI() {
			InitializeComponent();
			DesignImagesLoader.AddResourceImage(this.itemsImageList, EmbeddedResourceImage.ModuleLink);
			DesignImagesLoader.AddResourceImage(this.itemsImageList, EmbeddedResourceImage.DomainObject);
			DesignImagesLoader.AddResourceImage(this.itemsImageList, EmbeddedResourceImage.PersistentAssembly);
			DesignImagesLoader.AddResourceImage(this.itemsImageList, EmbeddedResourceImage.Property);
			modulesTreeView.TreeViewNodeSorter = null;
			modulesTreeView.ToolTipMessage = "To add references to modules, drag them from the Toolbox.";
			modulesTreeView.Enter += new EventHandler(Tray_Enter);
			businessClassesTreeView.Enter += new EventHandler(Tray_Enter);
			controllersTreeView.Enter += new EventHandler(Tray_Enter);
			moduleListView.Enter += new EventHandler(Tray_Enter);
			modulesTreeView.DragDrop += new DragEventHandler(modulesTreeView_DragDrop);
		}
		public void Initialize(XafModuleRootDesigner designer) {
			this.designer = designer;
			modulesTreeView.Designer = designer;
			modulesTreeView.DataSource = designer.CompiledModule;
			businessClassesTreeView.Designer = designer;
			businessClassesTreeView.DataSource = designer.CompiledModule;
			businessClassesTreeView.SwitchExportedTypeUsage += new SwitchExportedTypeUsageEventHandler(objectsTreeView_SwitchExportedTypeUsage);
			businessClassesTreeView.SwitchAssemblyUsage += new SwitchAssemblyUsageEventHandler(objectsTreeView_SwitchAssemblyUsage);
			controllersTreeView.Designer = designer;
			controllersTreeView.DataSource = designer.CompiledModule;
			moduleListView.Items[0].Text = designer.Component.Site.Name;
			this.MouseUp += new MouseEventHandler(control_MouseUp);
			if(XafRootDesignerBase.Enable_PopupMenu) {
				designer.AddMenuCommand(new MenuCommand(new EventHandler(OnMenuDelete), StandardCommands.Delete));
			}
			designer.SelectionService.SelectionChanged += new EventHandler(SelectionService_SelectionChanged);
		}
		[System.Security.SecuritySafeCritical]
		public void NavigateTo(string text) {
			try {
				string[] parts = text.Split(':');
				string paramName = parts[0].Trim();
				string paramString = parts[1].Trim();
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
							if(paramString != designer.Component.Site.Name) {
								throw new ArgumentException("The '" + designer.Component.Site.Name + "' module name is allowed while the '" + paramString + "' module was requested");
							}
							moduleListView.Focus();
							moduleListView.Select();
							moduleListView.Items[0].Selected = true;
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
			catch(Exception e) {
				designer.TraceException(e);
				throw;
			}
		}
		public string GetState() {
			try {
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
			catch(Exception e) {
				designer.TraceException(e);
				throw;
			}
		}
		public void SetValue(string name, object value) {
		}
		public event EventHandler FocusChanged;
	}
}
