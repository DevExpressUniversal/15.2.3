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
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraEditors.Design {
	public partial class ContextItemsDesignerForm : XtraForm {
		public ContextItemsDesignerForm() {
			InitializeComponent();
		}
		public ITypeDescriptorContext Context { get; set; }
		public IServiceProvider Provider { get; set; }
		ContextItemCollection contextItems;
		public ContextItemCollection ContextItems {
			get { return contextItems; }
			set {
				if(ContextItems == value)
					return;
				contextItems = value;
				OnContextItemsChanged();
			}
		}
		ContextItemCollection editableItems;
		protected ContextItemCollection EditableItems {
			get { return editableItems; }
			set {
				if(EditableItems == value)
					return;
				editableItems = value;
				OnEditableItemsChanged();
			}
		}
		protected virtual void OnEditableItemsChanged() {
			EditableItems.Options = ContextItems.Options.Clone();
			InitializePreviewControl();
			InitializePropertyGrid();
			IntializeItemCreationComboBox();
			InitializeListBox();
		}
		private void InitializePreviewControl() {
			if(this.pePreview.Image == null)
				this.pePreview.Image = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Design.Images.ContextButtonsSampleImage.jpg", typeof(ContextButtonsPreviewControl).Assembly);
			this.pePreview.ContextItems = EditableItems;
			this.pePreview.Options.Assign(EditableItems.Options);
			this.pePreview.UseRightToLeft = ContextItems.Owner.IsRightToLeft;
		}
		private void btnOk_Click(object sender, System.EventArgs e) {
			foreach(ContextItem item in EditableItems) {
				SetItemForcedVisibility(item);
			}
			ContextItems.Options.Assign(this.pePreview.Options);
			ContextItems.Clear();
			foreach(ContextItem item in EditableItems) {
				ContextItems.Add(item);
			}
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}
		void MoveItem(int delta) {
			if(this.lbItems.SelectedItem == null)
				return;
			int index = this.lbItems.SelectedIndex;
			if((delta < 0 && index == 0) || delta > 0 && index == this.lbItems.Items.Count -1)
				return;
			ContextButtonInfo info = (ContextButtonInfo)this.lbItems.SelectedValue;
			ImageListBoxItem item = (ImageListBoxItem)this.lbItems.SelectedItem;
			EditableItems.Remove(info.Item);
			this.lbItems.Items.RemoveAt(index);
			index += delta;
			EditableItems.Insert(index, info.Item);
			this.lbItems.Items.Insert(index, item);
			this.lbItems.SelectedIndex = index;
		}
		private void btnUp_Click(object sender, EventArgs e) {
			MoveItem(-1);
		}
		private void btnDown_Click(object sender, EventArgs e) {
			MoveItem(+1);
		}
		private void btnDelete_Click(object sender, EventArgs e) {
			if(this.lbItems.SelectedItem == null)
				return;
			EditableItems.RemoveAt(this.lbItems.SelectedIndex);
			this.lbItems.Items.RemoveAt(this.lbItems.SelectedIndex);
		}
		private void InitializePropertyGrid() {
			this.propertyGrid1.Site = new PropGridSite(Provider, GetComponent());
		}
		protected IComponent GetComponent() {
			Component component = Context.Instance as Component;
			if(component != null) {
				if(component is RepositoryItem) {
					RepositoryItem rep = component as RepositoryItem;
					if(rep.OwnerEdit != null)
						component = rep.OwnerEdit;
				}
				return component as IComponent;			
			}
			IDesignerHost host = Provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			return host == null ? null : host.RootComponent;			
		}
		private void IntializeItemCreationComboBox() {
			this.cbItems.Properties.Items.Clear();
			this.cbItems.Properties.Items.BeginUpdate();
			this.cbItems.Properties.Items.Add(new ContextButtonTypeInfo() { Type = typeof(ContextButton) });
			this.cbItems.Properties.Items.Add(new ContextButtonTypeInfo() { Type = typeof(CheckContextButton) });
			this.cbItems.Properties.Items.Add(new ContextButtonTypeInfo() { Type = typeof(RatingContextButton) });
			this.cbItems.Properties.Items.Add(new ContextButtonTypeInfo() { Type = typeof(TrackBarContextButton) });
			this.cbItems.Properties.Items.EndUpdate();
			this.cbItems.SelectedIndex = 0;
		}
		private void InitializeListBox() {
			this.lbItems.BeginUpdate();
			this.lbItems.Items.Clear();
			ImageCollection images = new ImageCollection();
			this.lbItems.ImageList = images;
			int imageIndex = 0;
			foreach(ContextItem item in EditableItems) {
				images.AddImage(item.Glyph != null? item.Glyph: new Bitmap(16,16));
				this.lbItems.Items.Add(new ImageListBoxItem(new ContextButtonInfo() { Item = item }, imageIndex));
				imageIndex++;
			}
			this.lbItems.EndUpdate();
		}
		protected virtual void OnContextItemsChanged() {
			EditableItems = (ContextItemCollection)ContextItems.Clone();   
		}
		private void lbItems_SelectedValueChanged(object sender, EventArgs e) {
			ContextButtonInfo info = (ContextButtonInfo)this.lbItems.SelectedValue;
			this.lbClassName.Text = info == null ? "" : "<b>" + info.Item.GetType().Name + "</b>";
			this.lbObjectName.Text = info == null ? "" : info.Item.Name;
			this.propertyGrid1.SelectedObject = info == null ? null : info.Item;
		}
		void btnAdd_Click(object sender, System.EventArgs e) {
			ContextItem item = CreateContextItem();
			SetItemForcedVisibility(item);
			this.lbItems.Items.Add(new ContextButtonInfo() { Item = item });
			this.lbItems.SelectedItem = item;
			this.pePreview.ContextItems.Add(item);
			this.pePreview.Refresh();
		}
		protected virtual ContextItem CreateContextItem() {
			ContextButtonTypeInfo info = (ContextButtonTypeInfo)this.cbItems.SelectedItem;
			IContextItemProvider provider = ContextItems.Owner as IContextItemProvider;
			ContextItem item;
			if(provider != null) {
				item = provider.CreateContextItem(info.Type);
				if(item != null) return item;
			}
			ConstructorInfo ci = info.Type.GetConstructor(new Type[] { });
			item = (ContextItem)ci.Invoke(new object[] { });
			item.Name = info.Type.Name;
			return item;
		}
		private void btnCancel_Click(object sender, System.EventArgs e) {
			this.Close();
		}
		private void chkShowItems_CheckedChanged(object sender, EventArgs e) {
			foreach(ContextItem item in this.pePreview.ContextItems) {
				SetItemForcedVisibility(item);
			}
		}
		void SetItemForcedVisibility(ContextItem item) {
			if(this.chkShowItems.Checked)
				item.SetForcedVisibility(ContextItemVisibility.Visible);
			else
				item.SetForcedVisibility(null);
			this.pePreview.Refresh();
		}
	}
	public class ContextButtonTypeInfo {
		public Type Type { get; set; }
		public override string ToString() {
			return Type.Name;
		}
	}
	public class ContextButtonInfo {
		public ContextItem Item { get; set; }
		public override string ToString() {
			return Item.Name + " [" + Item.GetType().Name + "]";
		}
	}
	public class PropGridSite : ISite, IServiceProvider {
		private IServiceProvider sp;
		private IComponent comp;
		private bool inGetService;
		public PropGridSite(IServiceProvider sp, IComponent comp) {
			this.sp = sp;
			this.comp = comp;
		}
		public IComponent Component { get { return this.comp; } }
		public IContainer Container { get { return (IContainer)null; } }
		public bool DesignMode { get { return false; } }
		public string Name { get { return (string)null; } set { } }		 
		public object GetService(System.Type t) {
			if(!this.inGetService) {
				if(this.sp != null) {
					try {
						this.inGetService = true;
						return this.sp.GetService(t);
					}
					finally {
						this.inGetService = false;
					}
				}
			}
			return (object)null;
		}
	}
}
