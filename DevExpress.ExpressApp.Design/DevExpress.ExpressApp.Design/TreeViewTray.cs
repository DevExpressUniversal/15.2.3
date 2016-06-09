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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Design {
	public class TreeViewTray<DataSourceType> : TreeView {
		private DataSourceType dataSource;
		private bool isDisposing;
		private XafRootDesignerBase designer;
		private IComponent selectedObject;
		private ToolTip toolTip;
		private string toolTipMessage;
		protected bool canShowToolTip;
		protected Label placeholderLabel;
		protected bool canShowPlaceholder;
		protected bool includeDependencies;
		protected override void OnEnter(EventArgs e) {
			if(!isDisposing) {
				base.OnEnter(e);
				if(SelectedNode != null) {
					SelectComponent(GetComponent(SelectedNode));
				}
			}
		}
		protected override void Dispose(bool disposing) {
			isDisposing = disposing;
			base.Dispose(disposing);
		}
		protected override void OnAfterSelect(TreeViewEventArgs e) {
			if(!isDisposing) {
				if(SelectedNode != null) {
					SelectComponent(GetComponent(SelectedNode));
				}
				base.OnAfterSelect(e);
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(e.Button == MouseButtons.Right && designer.SelectionService != null) {
				TreeViewHitTestInfo hitInfo = HitTest(new Point(e.X, e.Y));
				if(hitInfo.Node != null) {
					if(!hitInfo.Node.IsSelected) {
						SelectedNode = hitInfo.Node;
						SelectComponent(GetComponent(SelectedNode));
					}
					designer.ShowContextMenu(PointToScreen(new Point(e.X, e.Y)));
				}
				else {
					SelectedNode = null;
				}
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			try {
				base.OnPaint(e);
			}
			catch(Exception ex) {
				if(designer != null) {
					designer.TraceException(ex);
				}
				throw;
			}
		}
		protected override void OnDragEnter(DragEventArgs e) {
			base.OnDragEnter(e);
			if(designer.ToolboxService.IsToolboxItem(e.Data, designer.DesignerHost)) {
				ToolboxItem item = designer.ToolboxService.DeserializeToolboxItem(e.Data, designer.DesignerHost);
				if(AllowAddItem(item)) {
					e.Effect = DragDropEffects.Copy;
				}
				else {
					e.Effect = DragDropEffects.None;
				}
			}
		}
		protected virtual bool GetHide(object enteredControl) {
			return enteredControl != this;
		}
		protected virtual IComponent GetComponentCore(object obj) {
			if(obj is Type && typeof(IComponent).IsAssignableFrom((Type)obj)) {
				return (IComponent)ReflectionHelper.CreateObject((Type)obj);
			}
			return obj as IComponent;
		}
		protected virtual bool AllowAddItem(ToolboxItem item) {
			return false;
		}
		protected virtual void OnDataSourceChanged(DataSourceType oldDataSource, DataSourceType newDataSource) {
		}
		protected virtual void OnSetDesigner(XafRootDesignerBase designer) {
		}
		protected void SelectComponent(IComponent component) {
			if(!isDisposing) {
				SelectedObject = component;
				ICollection components = component != null ? new IComponent[] { component } : null;
				designer.SelectionService.SetSelectedComponents(components, SelectionTypes.Replace);
			}
		}
		protected void SetToolTip() {
			if(canShowToolTip) {
				toolTip.SetToolTip(this, ToolTipMessage);
			}
		}
		protected IComponent GetComponent(TreeNode node) {
			if(node == null) {
				return null;
			}
			IComponent component = GetComponentCore(node.Tag);
			if(component != null && (component.Site == null || component.Site.GetService(typeof(IDesignerHost)) != designer.Component.Site.GetService(typeof(IDesignerHost)))) {
				TypeDescriptor.AddAttributes(component, new Attribute[] { new ReadOnlyAttribute(true) });
			}
			return component;
		}
		protected TreeNode FindNode(TreeNodeCollection nodes, string fullName) {
			TreeNode result = null;
			foreach(TreeNode node in nodes) {
				if((node.Tag != null) && (node.Tag.GetType().FullName == fullName)) {
					result = node;
				}
				else {
					result = FindNode(node.Nodes, fullName);
				}
				if(result != null) {
					break;
				}
			}
			return result;
		}
		public TreeViewTray() {
			placeholderLabel = new Label();
			placeholderLabel.BackColor = Color.Transparent;
			placeholderLabel.ForeColor = Color.LightSteelBlue;
			placeholderLabel.Font = new Font(placeholderLabel.Font, FontStyle.Bold);
			placeholderLabel.TextAlign = ContentAlignment.MiddleCenter;
			placeholderLabel.Location = new Point(0, 0);
			placeholderLabel.Dock = DockStyle.Fill;
			placeholderLabel.Visible = false;
			Controls.Add(placeholderLabel);
			toolTip = new ToolTip();
			toolTip.BackColor = Color.AliceBlue;
			ToolTipMessage = "You can drop items here from ToolBox";
		}
		public TreeViewTray(IContainer container)
			: this() {
			container.Add(this);
		}
		public bool ContainsComponent(IComponent component) {
			foreach(TreeNode node in Nodes) {
				if(node.Tag == component) {
					return true;
				}
			}
			return false;
		}
		public TreeNode FindNode(string fullName) {
			return FindNode(Nodes, fullName);
		}
		public virtual void RefreshNodes() {
			placeholderLabel.Visible = canShowPlaceholder && (Nodes.Count == 0);
		}
		public void SetHideSelection(object enteredControl) {
			bool hide = GetHide(enteredControl);
			if(HideSelection != hide) {
				HideSelection = hide;
			}
		}
		public IComponent SelectedObject {
			get { return selectedObject; }
			set { selectedObject = value; }
		}
		public string ToolTipMessage {
			get { return placeholderLabel.Text; }
			set {
				toolTipMessage = value;
				if(placeholderLabel != null) {
					placeholderLabel.Text = toolTipMessage;
				}
			}
		}
		public XafRootDesignerBase Designer {
			get { return designer; }
			set {
				designer = value;
				OnSetDesigner(designer);
			}
		}
		public DataSourceType DataSource {
			get { return dataSource; }
			set {
				if((object)dataSource != (object)value) {
					DataSourceType oldDataSource = dataSource;
					dataSource = value;
					OnDataSourceChanged(oldDataSource, dataSource);
					if(!Disposing) {
						RefreshNodes();
					}
				}
			}
		}
	}
	public abstract class TreeBuilder<DataSourceType> {
		protected void ClearTreeViewNodes(TreeView treeView) {
			if(treeView != null) {
				treeView.Nodes.Clear();
			}
		}
		protected TreeNode CreateModuleReferenceTreeItem(TreeView treeView, Type moduleType) {
			return CreateModuleReferenceTreeItem(treeView, moduleType, false);
		}
		protected TreeNode CreateModuleReferenceTreeItem(TreeView treeView, Type moduleType, bool useToolboxImage) {
			string moduleName = ((ModuleBase)ReflectionHelper.CreateObject(moduleType)).Name;
			TreeNode node = new TreeNode(moduleName);
			node.Name = moduleType.FullName;
			node.Tag = moduleType;
			node.ImageKey = EmbeddedResourceImage.ModuleLink.ToString();
			if(useToolboxImage) {
				string imageKey = DesignImagesLoader.GetImageKey(treeView.ImageList, moduleType);
				if(imageKey != "") {
					node.ImageKey = imageKey;
				}
			}
			node.SelectedImageKey = node.ImageKey;
			node.ToolTipText = moduleType.FullName;
			return node;
		}
	}
}
