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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.Remoting;
using DevExpress.XtraPrinting.Preview;
using System.ComponentModel.Design;
using DevExpress.XtraPrintingLinks;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Links;
namespace DevExpress.XtraPrinting.Design
{
	internal class LinkItemsEditorForm : System.Windows.Forms.Form
	{
		#region inner classes
		class TypeMenuItem : MenuItem {
			Type type;
			public Type Type { get { return type; } 
			}
			public TypeMenuItem(Type type) {
				this.type = type;
				Text = type.FullName;
			}
		}
		#endregion
		private System.Windows.Forms.Label labelProperties;
		private System.Windows.Forms.Label labelDelimit;
		private System.Windows.Forms.Label labelMembers;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.Button buttonRemove;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TreeView treeView;
		private DevExpress.XtraPrinting.Design.MyPropertyGrid propertyGrid;
		private IList links;
		private Type[] linkTypes;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonPreview;
		private ITypeDescriptorContext context;
		private System.Windows.Forms.Button buttonRefresh;
		private ArrayList removedItems;
		public object EditValue { 
			get { return links; } 
			set {
				if(value is IList) { 
					links = (IList)value; 
					OnEditValueChanged();
				}
			} 
		}
		private PrintingSystem PrintingSystem { get { return (PrintingSystem)context.GetComponent(); }
		}
		private LinkBase SelectedLink { get { return (LinkBase)propertyGrid.SelectedObject; }
		}
		public LinkItemsEditorForm(ITypeDescriptorContext context) {
			InitializeComponent();
			this.MinimumSize = new System.Drawing.Size(312, 248);
			if(context.Instance is Component) {
				propertyGrid.UpdateSite((context.Instance as Component).Site);
			}
			this.context = context;
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		private void OnEditValueChanged() {
			treeView.Nodes.Clear();
			CreateTreeView(links, treeView.Nodes);
			List<Type> list = new List<Type>(LinkDesigner.LinkTypes);
			list.Sort((t1, t2) => {
				return t1.Namespace != t2.Namespace ? string.CompareOrdinal(t1.Namespace, t2.Namespace) :
					string.CompareOrdinal(t1.Name, t2.Name);
			});
			this.linkTypes = list.ToArray();
			CreateContextMenu();
			removedItems = new ArrayList();
		}
		private LinkBase CreateLink(Type type) {
			LinkBase link = (LinkBase)Activator.CreateInstance(type);
			link.PrintingSystemBase = this.PrintingSystem;
			return link;
		}
		private void CreateContextMenu() {
			contextMenu1.MenuItems.Clear();
			foreach(Type type in linkTypes) {
				MenuItem item = new TypeMenuItem(type);
				item.Index = contextMenu1.MenuItems.Count;
				item.Click += new System.EventHandler(menuItem_Click);
				contextMenu1.MenuItems.Add(item);
			}
		}
		private void CreateTreeView(IList links, TreeNodeCollection Nodes) {
			if(links == null) return;
			foreach(LinkBase link in links) {
				TreeNode treeNode = new TreeNode(link.Site.Name);
				Nodes.Add(treeNode);
			}
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LinkItemsEditorForm));
			this.labelProperties = new System.Windows.Forms.Label();
			this.treeView = new System.Windows.Forms.TreeView();
			this.buttonPreview = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.labelMembers = new System.Windows.Forms.Label();
			this.labelDelimit = new System.Windows.Forms.Label();
			this.propertyGrid = new DevExpress.XtraPrinting.Design.MyPropertyGrid();
			this.buttonRefresh = new System.Windows.Forms.Button();
			this.SuspendLayout();
			this.labelProperties.Location = new System.Drawing.Point(216, 8);
			this.labelProperties.Name = "labelProperties";
			this.labelProperties.Size = new System.Drawing.Size(71, 14);
			this.labelProperties.TabIndex = 4;
			this.labelProperties.Text = "Properties:";
			this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left);
			this.treeView.HideSelection = false;
			this.treeView.ImageIndex = -1;
			this.treeView.Indent = 19;
			this.treeView.Location = new System.Drawing.Point(8, 24);
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = -1;
			this.treeView.Size = new System.Drawing.Size(184, 296);
			this.treeView.TabIndex = 0;
			this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
			this.buttonPreview.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonPreview.Enabled = false;
			this.buttonPreview.Location = new System.Drawing.Point(8, 360);
			this.buttonPreview.Name = "buttonPreview";
			this.buttonPreview.Size = new System.Drawing.Size(72, 25);
			this.buttonPreview.TabIndex = 13;
			this.buttonPreview.Text = "Preview";
			this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
			this.buttonOK.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(216, 400);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(72, 25);
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			this.buttonCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(304, 400);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(72, 25);
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "Cancel";
			this.buttonRemove.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonRemove.Enabled = false;
			this.buttonRemove.Location = new System.Drawing.Point(88, 328);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(72, 25);
			this.buttonRemove.TabIndex = 10;
			this.buttonRemove.Text = "Remove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			this.buttonHelp.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonHelp.Enabled = false;
			this.buttonHelp.Location = new System.Drawing.Point(392, 400);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(72, 25);
			this.buttonHelp.TabIndex = 9;
			this.buttonHelp.Text = "Help";
			this.buttonAdd.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonAdd.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonAdd.Image")));
			this.buttonAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonAdd.Location = new System.Drawing.Point(8, 328);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(72, 25);
			this.buttonAdd.TabIndex = 12;
			this.buttonAdd.Text = "Add";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			this.labelMembers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.labelMembers.Location = new System.Drawing.Point(8, 8);
			this.labelMembers.Name = "labelMembers";
			this.labelMembers.Size = new System.Drawing.Size(55, 14);
			this.labelMembers.TabIndex = 3;
			this.labelMembers.Text = "Members:";
			this.labelDelimit.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.labelDelimit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelDelimit.Location = new System.Drawing.Point(8, 392);
			this.labelDelimit.Name = "labelDelimit";
			this.labelDelimit.Size = new System.Drawing.Size(472, 3);
			this.labelDelimit.TabIndex = 2;
			this.propertyGrid.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.DrawFlat = false;
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(200, 24);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(280, 360);
			this.propertyGrid.TabIndex = 11;
			this.propertyGrid.Text = "propertyGrid";
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.propertyGrid.SelectedObjectsChanged += new System.EventHandler(this.propertyGrid_SelectedObjectChanged);
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
			this.buttonRefresh.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonRefresh.Enabled = false;
			this.buttonRefresh.Location = new System.Drawing.Point(88, 360);
			this.buttonRefresh.Name = "buttonRefresh";
			this.buttonRefresh.Size = new System.Drawing.Size(72, 25);
			this.buttonRefresh.TabIndex = 14;
			this.buttonRefresh.Text = "Refresh";
			this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
#if DXWhidbey
			this.AutoScaleMode = AutoScaleMode.None;
#else
			this.AutoScale = false;
#endif
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(488, 429);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonRefresh,
																		  this.buttonPreview,
																		  this.buttonAdd,
																		  this.propertyGrid,
																		  this.buttonRemove,
																		  this.buttonHelp,
																		  this.buttonOK,
																		  this.buttonCancel,
																		  this.labelMembers,
																		  this.labelDelimit,
																		  this.treeView,
																		  this.labelProperties});
			this.MinimumSize = new System.Drawing.Size(312, 248);
			this.Name = "LinkItemsEditorForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Link Collection Editor";
			this.Closed += new System.EventHandler(this.LinkItemsEditorForm_Closed);
			this.ResumeLayout(false);
		}
		#endregion
		private void UpdateForm() {
			bool enabled = links.Count > 0;
			buttonRemove.Enabled = enabled;
			buttonPreview.Enabled = enabled;
			buttonRefresh.Enabled = enabled && PrintingSystem.PreviewFormEx.Visible;
		}
		private void treeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			ShowNodeProperty(e.Node);
			UpdateForm();
		}
		protected object ShowNodeProperty(System.Windows.Forms.TreeNode treeNode) {
			if(treeNode != null) {
				int index = treeView.Nodes.IndexOf(treeNode);
				try {
					object obj = links[index];
					if(obj != null) {
						this.propertyGrid.SelectedObject = obj;
						return obj;
					}
				}
				catch {}
			}
			this.propertyGrid.SelectedObject = null;
			return null;
		}
		private void ClearContainer(ArrayList items) {
			foreach(IComponent comp in items) {
				context.Container.Remove(comp);
				comp.Dispose();
			}
		}
		private void EnableAfterSelect() {
			treeView.AfterSelect += new TreeViewEventHandler(treeView_AfterSelect);
		}
		private void DisableAfterSelect() {
			treeView.AfterSelect -= new TreeViewEventHandler(treeView_AfterSelect);
		}
		private void propertyGrid_SelectedObjectChanged(object sender, EventArgs e) {
			object selObject = propertyGrid.GetSelectedObject();
			if(selObject is Component) {
				propertyGrid.UpdateSite((selObject as Component).Site);
				propertyGrid.ShowEvents(true);
			} else
				propertyGrid.ShowEvents(false);
			UpdatePreviewForm();
		}
		private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			object obj = ((PropertyGrid)s).SelectedObject;
			TreeNode treeNode = treeView.SelectedNode;
			if(obj != null && treeNode != null)
				if(obj is LinkBase) treeNode.Text = ((LinkBase)obj).Site.Name;
		}
		private void buttonRemove_Click(object sender, System.EventArgs e) {
			object obj = propertyGrid.SelectedObject;
			TreeNode treeNode = this.treeView.SelectedNode;
			if(treeNode != null && obj != null) {
				propertyGrid.SelectedObject = null;
				if(obj is LinkBase) {
					links.Remove(obj);
					removedItems.Add(obj);
				}
				DisableAfterSelect();
				treeNode.Remove();
				treeNode = treeView.SelectedNode;
				ShowNodeProperty(treeNode);
				UpdateForm();
				EnableAfterSelect();
				propertyGrid.Refresh();
			}
		}
		private void buttonAdd_Click(object sender, System.EventArgs e) {
			contextMenu1.Show(buttonAdd, new Point(0, buttonAdd.Size.Height));
		}
		private void menuItem_Click(object sender, System.EventArgs e) {
			if(sender is MenuItem) {
				LinkBase link = CreateLink(((TypeMenuItem)sender).Type);
				if(link != null) {
					context.Container.Add(link);
					links.Add(link);
					TreeNode node = treeView.Nodes.Add(link.Site.Name);
					treeView.SelectedNode = node;
				}
			}
		}
		private void buttonPreview_Click(object sender, System.EventArgs e) {
			if(treeView.SelectedNode != null && SelectedLink != null) {
				LinkBase owner = SelectedLink.Owner;
				SelectedLink.Owner = null;
				PrintPreviewFormEx form = PrintingSystem.PreviewFormEx;
				try {
					SelectedLink.CreateDocument();
				} catch(Exception ex) {
					NotificationService.ShowException<PrintingSystemBase>(form.LookAndFeel, form, ex);
					return;
				}
				SelectedLink.Owner = owner;
				form.SelectedPageIndex = 0;
				if(form.Visible == false) {
					form.ShowInTaskbar = false;
					((IWinLink)SelectedLink).ShowPreview();
					form.Owner = this;
					form.Disposed += new EventHandler(PreviewForm_Disposed);
				} else {
					form.Invalidate();
				}
				UpdateForm();
			}
		}
		private void buttonRefresh_Click(object sender, System.EventArgs e) {
			UpdatePreviewForm();
		}
		protected void PreviewForm_Disposed(object sender, EventArgs e) {
			propertyGrid.Refresh();
			PrintPreviewFormEx form = (PrintPreviewFormEx)sender;
			form.Disposed -= new EventHandler(PreviewForm_Disposed);
			form.Owner = null;
			UpdateForm();
		}
		private void LinkItemsEditorForm_Closed(object sender, System.EventArgs e) {
			PrintPreviewFormEx form = PrintingSystem.PreviewFormEx;
			if(form.IsDisposed == false) form.Dispose();
		}
		private void buttonOK_Click(object sender, System.EventArgs e) {
			ClearContainer(removedItems);
		}
		private void UpdatePreviewForm() {
			PrintPreviewFormEx form = PrintingSystem.PreviewFormEx;
			if(form.Visible) {
				try {
					SelectedLink.CreateDocument();
					form.SelectedPageIndex = 0;
					form.Invalidate();
				} catch {;}
			}
		}
	}
}
namespace System.ComponentModel {
	static class ITypeDescriptorContextExtentions {
		public static IComponent GetComponent(this ITypeDescriptorContext context) { 
			return context.Instance is DesignerActionList ? ((DesignerActionList)context.Instance).Component :  context.Instance as IComponent;
		}
	}
}
