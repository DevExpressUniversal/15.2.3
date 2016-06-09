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
using DevExpress.ExpressApp.Utils;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Designer.Interfaces;
namespace DevExpress.ExpressApp.Design {
	public abstract class ListViewTray<DataSourceType> : System.Windows.Forms.ListView {
		private DataSourceType dataSource;
		private ToolTip toolTip = new ToolTip();
		private string toolTipMessage;
		protected bool canShowToolTip;
		protected Label placeholderLabel;
		protected bool canShowPlaceholder;
		protected XafRootDesignerBase designer;
		private void SetSelectedComponents() {
			bool needToChangeSelection = false;
			List<IComponent> components = new List<IComponent>();
			foreach(ListViewItem item in SelectedItems) {
				if(!needToChangeSelection && !designer.SelectionService.GetComponentSelected(item.Tag)) {
					needToChangeSelection = true;
				}
				components.Add((Component)item.Tag);
			}
			if(needToChangeSelection) {
				designer.SelectionService.SetSelectedComponents(components, SelectionTypes.Replace);
				IVSMDPropertyBrowser propertyBrowser = (IVSMDPropertyBrowser)designer.DesignerHost.GetService(typeof(IVSMDPropertyBrowser));
				if(propertyBrowser != null) {
					propertyBrowser.Refresh();
				}
			}
		}
		protected abstract void RefreshItemsCore();
		protected void SetTooltip() {
			if(canShowToolTip) {
				toolTip.SetToolTip(this, ToolTipMessage);
			}
		}
		protected void AddListViewItem(object obj, string caption) {
			ListViewItem viewItem = new ListViewItem(CaptionHelper.ConvertCompoundName(caption));
			viewItem.Tag = obj;
			Type componentType = obj.GetType();
			if(!LargeImageList.Images.ContainsKey(componentType.FullName)) {
				Bitmap bitmap = new ToolboxItem(componentType).Bitmap;
				ToolboxBitmapAttribute bitmapAttribute = (ToolboxBitmapAttribute)System.Attribute.GetCustomAttribute(componentType, typeof(ToolboxBitmapAttribute));
				if(bitmapAttribute != null) {
					Bitmap largeBitmap = (Bitmap)bitmapAttribute.GetImage(obj, true);
					if(largeBitmap != null) {
						bitmap = largeBitmap;
					}
				}
				LargeImageList.Images.Add(componentType.FullName, bitmap);
			}
			viewItem.ImageKey = componentType.FullName;
			this.Items.Add(viewItem);
		}
		protected void ProcessToolboxItem(ToolboxItem item) {
			if(designer.DesignerHost != null) {
				using(DesignerTransaction trans = designer.DesignerHost.CreateTransaction("Creating " + item.DisplayName)) {
					IComponent[] newComponents = null;
					if(CanProcessToolboxItem(item)) {
						newComponents = CreateComponentsCore(item);
						ComponentsCreated(newComponents);
					}
					trans.Commit();
					designer.ToolboxService.SelectedToolboxItemUsed();
					if(designer.SelectionService != null && newComponents != null) {
						designer.SelectionService.SetSelectedComponents(newComponents, SelectionTypes.Replace);
					}
				}
			}
		}
		protected virtual bool CanProcessToolboxItem(ToolboxItem item) {
			return false;
		}
		protected virtual IComponent[] CreateComponentsCore(ToolboxItem item) {
			return item.CreateComponents(designer.DesignerHost);
		}
		protected virtual void ComponentsCreated(IComponent[] createdComponents){
			foreach(IComponent component in createdComponents) {
				IDesigner componentDesigner = designer.DesignerHost.GetDesigner(component);
				if(componentDesigner != null) {
					try {
						componentDesigner.Initialize(component);
					}
					finally {
						componentDesigner.Dispose();
					}
				}
			}
		}
		protected virtual void OnDataSourceChanged(DataSourceType oldDataSource, DataSourceType newDataSource) { 
		}
		protected virtual void OnSetDesigner(XafRootDesignerBase designer) {
		}
		protected virtual bool AllowAddItem(ToolboxItem item) {
			return true;
		}
		protected override void OnDragDrop(DragEventArgs e) {
			base.OnDragDrop(e);
			ToolboxItem item = designer.DeserializeToolboxItem(e.Data);
			if(item != null) {
				ProcessToolboxItem(item);
			}		
		}
		protected override void OnDragEnter(DragEventArgs e) {
			base.OnDragEnter(e);
			if(designer.ToolboxService != null && designer.ToolboxService.IsToolboxItem(e.Data, designer.DesignerHost)) {
				ToolboxItem item = designer.ToolboxService.DeserializeToolboxItem(e.Data, designer.DesignerHost);
				if(AllowAddItem(item)) {
					e.Effect = DragDropEffects.Copy;
				}
				else {
					e.Effect = DragDropEffects.None;
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
		protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e) {
			base.OnItemSelectionChanged(e);
			if(e.IsSelected) {
				SetSelectedComponents();
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(e.Button == MouseButtons.Right) {
				if(designer.SelectionService != null) {
					ListViewHitTestInfo hitInfo = this.HitTest(new Point(e.X, e.Y));
					if(hitInfo.Item != null && !hitInfo.Item.Selected) {
						designer.SelectionService.SetSelectedComponents(new IComponent[] { (IComponent)hitInfo.Item.Tag }, SelectionTypes.Replace);
					}
					designer.ShowContextMenu(this.PointToScreen(new Point(e.X, e.Y)));
				}
			}
		}
		protected override void OnEnter(EventArgs e) {
			this.Focus();
			if(SelectedItems.Count == 0 && this.Items.Count != 0) {
				this.Items[0].Selected = true;
			}
			else {
				SetSelectedComponents();
			}
			base.OnEnter(e);
		}
		public ListViewTray() {
			this.AllowDrop = true;
			this.LargeImageList = new ImageList();
			this.LargeImageList.ImageSize = new Size(32, 32);
			this.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
			placeholderLabel = new Label();
			Controls.Add(this.placeholderLabel);
			placeholderLabel.BackColor = Color.Transparent;
			placeholderLabel.ForeColor = Color.LightSteelBlue;
			placeholderLabel.Font = new Font(placeholderLabel.Font, FontStyle.Bold);
			placeholderLabel.TextAlign = ContentAlignment.MiddleCenter;
			placeholderLabel.Dock = DockStyle.Fill;
			placeholderLabel.Visible = false;
			ToolTipMessage = "Drag Toolbox items here";
			toolTip.BackColor = Color.AliceBlue;
		}
		public bool ContainsComponent(IComponent component) {
			foreach(ListViewItem item in Items) {
				if(item.Tag == component) {
					return true;
				}
			}
			return false;
		}
		public void SetHideSelection(object enteredControl) {
			bool hide = enteredControl != this;
			if(this.HideSelection != hide) {
				this.HideSelection = hide;
			}
		}
		public void RefreshItems() {
			RefreshItemsCore();
			placeholderLabel.Visible = canShowPlaceholder && (Items.Count == 0);
		}
		public string ToolTipMessage {
			get { return this.placeholderLabel.Text; }
			set {
				toolTipMessage = value;
				if(placeholderLabel != null) {
					placeholderLabel.Text = toolTipMessage;
				}
			}
		}
		public DataSourceType DataSource {
			get {
				return dataSource;
			}
			set {
				if((object)dataSource != (object)value) {
					DataSourceType oldDataSource = dataSource;
					dataSource = value;
					OnDataSourceChanged(oldDataSource, dataSource);
					RefreshItems();
				}
			}
		}
		public XafRootDesignerBase Designer {
			get {
				return designer;
			}
			set {
				designer = value;
				OnSetDesigner(designer);
			}
		}
	}
}
