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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using System.Drawing.Design;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Native;
using System.Collections;
namespace DevExpress.Utils.UI {
	public partial class CollectionEditorContentControl : XtraUserControl {
		class SelectionWrapper : PropertyDescriptor, ICustomTypeDescriptor {
			ICollection collection;
			Type collectionItemType;
			Type collectionType;
			Control control;
			PropertyDescriptorCollection properties;
			object value;
			public override Type ComponentType { get { return this.collectionType; } }
			public override bool IsReadOnly { get { return false; } }
			public override Type PropertyType { get { return this.collectionItemType; } }
			public SelectionWrapper(Type collectionType, Type collectionItemType, Control control, ICollection collection) :
				base("Value", new Attribute[] { new CategoryAttribute(collectionItemType.Name) }) {
				this.collectionType = collectionType;
				this.collectionItemType = collectionItemType;
				this.control = control;
				this.collection = collection;
				this.properties = new PropertyDescriptorCollection(new PropertyDescriptor[] { this });
				this.value = this;
				foreach(XtraListNode node in collection) {
					object newValue = node.GetItem();
					if(this.value == this) {
						this.value = newValue;
					}
					else {
						if(this.value != null) {
							if(newValue == null) {
								this.value = null;
							}
							else {
								if(this.value.Equals(newValue)) {
									continue;
								}
								this.value = null;
							}
							break;
						}
						if(newValue != null) {
							this.value = null;
							break;
						}
					}
				}
			}
			public override bool CanResetValue(object component) {
				return false;
			}
			public override object GetValue(object component) {
				return this.value;
			}
			public override void ResetValue(object component) {
			}
			public override void SetValue(object component, object value) {
				this.value = value;
				foreach(XtraListNode node in collection) {
					node.SetItem(value);
				}
				this.control.Invalidate();
				this.OnValueChanged(component, EventArgs.Empty);
			}
			public override bool ShouldSerializeValue(object component) {
				return false;
			}
			#region ICustomTypeDescriptor Members
			AttributeCollection ICustomTypeDescriptor.GetAttributes() {
				return TypeDescriptor.GetAttributes(this.collectionItemType);
			}
			string ICustomTypeDescriptor.GetClassName() {
				return this.collectionItemType.Name;
			}
			string ICustomTypeDescriptor.GetComponentName() {
				return null;
			}
			TypeConverter ICustomTypeDescriptor.GetConverter() {
				return null;
			}
			EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
				return null;
			}
			PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
				return this;
			}
			object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
				return null;
			}
			EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
				return EventDescriptorCollection.Empty;
			}
			EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
				return EventDescriptorCollection.Empty;
			}
			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
				return this.properties;
			}
			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
				return this.properties;
			}
			object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
				return this;
			}
			#endregion
		}
		bool allowGlyphSkinning;
		bool isRTLChanged;
		ArrayList addedItems = new ArrayList();
		ArrayList initialItems = new ArrayList();
		ArrayList removedItems = new ArrayList();
		CollectionEditor editor;
		object editValue;
		IServiceProvider serviceProvider;
		protected IServiceProvider ServiceProvider { get { return serviceProvider; } }
		public CollectionEditorContentControl() {
			InitializeComponent();
			propertyGrid.PropertyGridControl.CellValueChanged += new XtraVerticalGrid.Events.CellValueChangedEventHandler(OnPropertyGridCellChanged);
			tv.KeyDown += new KeyEventHandler(tv_KeyDown);
			tv.CustomDrawNodeCell += new CustomDrawNodeCellEventHandler(tv_CustomDrawNodeCell);
		}
		public CollectionEditorContentControl(IServiceProvider provider, CollectionEditor collectionEditor) : this() {
			SetServiceProvider(provider);
			SetEditor(collectionEditor);
		}
		public void SetServiceProvider(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			propertyGrid.ServiceProvider = serviceProvider;
			propertyGrid.SetLookAndFeel(serviceProvider);
		}
		public void SetEditor(CollectionEditor collectionEditor) {
			this.editor = collectionEditor;
		}
		public void SelectNodeByName(string name) {
			TreeListNode selectedNode = tv.Nodes.FirstOrDefault(x => name == ((XtraListNode)x).Text);
			if(selectedNode != null)
				tv.SelectNode(selectedNode);
		}
		public Type CollectionItemType {
			get { return this.editor.CollectionItemType; }
		}
		public void SelectLastItem() {
			SelectTreeNode(Items.Length - 1);
		}
		protected TService GetService<TService>() where TService : class {
			if(serviceProvider != null)
				return serviceProvider.GetService(typeof(TService)) as TService;
			return null;
		}
		protected string GetItemName(object item, int index) {
			return editor.GetItemName(item, index);
		}
		protected virtual void buttonAdd_Click(object sender, System.EventArgs e) {
			Type type = NewItemTypes[0];
			this.CreateAndAddInstance(type);
		}
		protected Type[] NewItemTypes {
			get {
				return this.editor.NewItemTypes;
			}
		}
		protected void CreateAndAddInstance(Type type) {
			try {
				object instance = CreateInstance(type);
				AddInstance(instance);
			}
			catch {
			}
		}
		protected object CreateInstance(Type itemType) {
			return this.editor.CreateInstance(itemType);
		}
		protected void DisposeInstance(object instance) {
			this.editor.DisposeInstance(instance);
		}
		protected virtual void DisposeInstanceOnFinish(object instance) {
			DisposeInstance(instance);
		}
		protected virtual void OnPropertyGridCellChanged(object sender, XtraVerticalGrid.Events.CellValueChangedEventArgs e) {
			this.tv.Invalidate();
			this.tv.Update();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object EditValue {
			get { return editValue; }
			set {
				this.editValue = value;
				object[] items = Items;
				foreach(object item in items) {
					initialItems.Add(item);
				}
				AddItems(items);
				if(tv.Nodes.Count > 0)
					SelectTreeNode(0);
				UpdateUI();
			}
		}
		protected ITypeDescriptorContext Context {
			get {
				return this.editor.Context;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object[] Items {
			get {
				return this.editor.GetItems(this.EditValue);
			}
			set {
				if(!Object.ReferenceEquals(this.editor.Items, null) && value.SequenceEqual(this.editor.Items.Cast<object>()))
					return;
				bool canChanged = false;
				try {
					if(Context != null)
						canChanged = Context.OnComponentChanging();
				}
				catch {
					canChanged = false;
				}
				object obj = this.editor.SetItems(this.EditValue, value);
				if(obj != this.EditValue)
					this.EditValue = obj;
				if(canChanged && Context != null)
					Context.OnComponentChanged();
			}
		}
		void tv_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e) {
			object item = e.Node.GetItem();
			UITypeEditor itemEditor = TypeDescriptor.GetEditor(item, typeof(UITypeEditor)) as UITypeEditor;
			string text = GetItemName(item, tv.Nodes.IndexOf(e.Node));
			if(itemEditor != null && itemEditor.GetPaintValueSupported()) {
				Rectangle bounds = new Rectangle(e.Bounds.X + 2, e.Bounds.Y, e.Bounds.Width - 2, e.Bounds.Height);
				Rectangle rect = new Rectangle(bounds.X, bounds.Y + 1, 20, bounds.Height - 3);
				e.Graphics.DrawRectangle(SystemPens.ControlText, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
				rect.Inflate(-1, -1);
				itemEditor.PaintValue(item, e.Graphics, rect);
				Rectangle textBounds = new Rectangle(bounds.X + 27, bounds.Y, bounds.Width - 27, bounds.Height);
				e.Cache.DrawString(text, e.Appearance.Font, e.Appearance.GetForeBrush(e.Cache), textBounds, e.Appearance.GetStringFormat());
				e.CellText = null;
			}
			else
				e.CellText = text;
		}
		protected virtual void tv_AfterSelect(object sender, NodeEventArgs e) {
			UpdateUI();
		}
		protected void SelectTreeNode(int index) {
			try {
				if(index < tv.Nodes.Count)
					tv.SelectNode(tv.Nodes[index]);
			}
			catch { }
		}
		void AddItems(IList items) {
			tv.BeginUpdate();
			foreach(object item in items) {
				XtraListNode node = new XtraListNode(GetItemName(item, tv.Nodes.Count), tv.Nodes);
				((IList)tv.Nodes).Add(node);
				node.SelectImageIndex = node.StateImageIndex = GetItemImageIndex(item);
				node.SetItem(item);
			}
			tv.EndUpdate();
		}
		public void Finish() {
			UpdateItems();
			foreach(object item in removedItems) {
				DisposeInstanceOnFinish(item);
			}
			removedItems.Clear();
			addedItems.Clear();
			initialItems.Clear();
			tv.ClearNodes();
		}
		protected void AddInstance(object instance) {
			IList objectsFromInstance = this.editor.GetObjectsFromInstance(instance);
			if(objectsFromInstance != null) {
				foreach(object item in objectsFromInstance) {
					addedItems.Add(item);
				}
				AddItems(objectsFromInstance);
				UpdateItems();
				SelectTreeNode(tv.Nodes.Count - 1);
				UpdateUI();
			}
		}
		private void buttonRemove_Click(object sender, System.EventArgs e) {
			RemoveItem(tv.SelectedNode);
		}
		protected virtual void buttonUp_Click(object sender, System.EventArgs e) {
			TreeListNode prevNode = tv.SelectedNode.PrevNode;
			int prevNodeIndex = tv.Nodes.IndexOf(prevNode);
			tv.SetNodeIndex(tv.SelectedNode, prevNodeIndex);
			UpdateItems();
			UpdateUI();
		}
		protected virtual void buttonDown_Click(object sender, System.EventArgs e) {
			TreeListNode nextNode = tv.SelectedNode.NextNode;
			int nextNodeIndex = tv.Nodes.IndexOf(nextNode);
			tv.SetNodeIndex(tv.SelectedNode, nextNodeIndex);
			UpdateItems();
			UpdateUI();
		}
		public void Cancel() {
			tv.ClearNodes();
			if(addedItems.Count > 0) {
				IComponent component = addedItems[0] as IComponent;
				if(component != null && component.Site != null)
					return;
			}
			foreach(object item in addedItems) {
				DisposeInstance(item);
			}
			addedItems.Clear();
			removedItems.Clear();
			Items = initialItems.ToArray();
			initialItems.Clear();
		}
		protected virtual int GetItemImageIndex(object item) {
			return 0;
		}
		private void tv_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete)
				RemoveItem(tv.SelectedNode);
		}
		protected void RemoveItem(TreeListNode node) {
			object item = node.GetItem();
			if(CanRemove(item)) {
				RemoveItemCore(node, item);
				UpdateItems();
				UpdateUI();
			}
		}
		protected virtual void UpdateUI() {
			UpdatePropertyGrid();
			UpdateRemoveButton();
			UpdateUpDownButtons();
		}
		protected void UpdateRemoveButton() {
			buttonRemove.Enabled = tv.Nodes.Count > 0 && CanRemove(tv.SelectedNode.GetItem());
		}
		protected virtual void UpdateUpDownButtons() {
			buttonUp.Enabled = tv.SelectedNode != null && tv.SelectedNode.PrevNode != null;
			buttonDown.Enabled = tv.SelectedNode != null && tv.SelectedNode.NextNode != null;
		}
		protected void UpdatePropertyGrid() {
			try {
				XtraListNode selectedNode = tv.SelectedNode;
				if(selectedNode != null) {
					object[] selectedNodes = new object[] { selectedNode };
					if(IsImmutable(selectedNodes)) {
						propertyGrid.SelectedObject = new SelectionWrapper(editor.CollectionType, editor.CollectionItemType, tv, selectedNodes);
					}
					else {
						propertyGrid.SelectedObject = selectedNode.GetItem();
					}
				}
				else {
					propertyGrid.SelectedObject = null;
				}
			}
			catch {
				propertyGrid.SelectedObject = null;
			}
		}
		bool IsImmutable(ICollection selectedNodes) {
			foreach(XtraListNode node in selectedNodes) {
				Type type = node.GetItem().GetType();
				if(!TypeDescriptor.GetConverter(type).GetCreateInstanceSupported()) {
					foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(type)) {
						if(!descriptor.IsReadOnly) {
							return false;
						}
					}
				}
			}
			return true;
		}
		void UpdateItems() {
			object[] objArray = new object[tv.Nodes.Count];
			for(int i = 0; i < tv.Nodes.Count; i++) {
				objArray.SetValue(tv.Nodes[i].GetItem(), i);
			}
			Items = objArray;
		}
		protected virtual void RemoveItemCore(TreeListNode node, object item) {
			TreeListNode prevNode = node.PrevNode;
			TreeListNode nextNode = node.NextNode;
			if(addedItems.Contains(item)) {
				DisposeInstance(item);
				addedItems.Remove(item);
			}
			else {
				removedItems.Add(item);
			}
			tv.DeleteNode(node);
			node.SetItem(null);
			if(prevNode != null) {
				prevNode.Selected = true;
			}
			else {
				if(nextNode != null)
					nextNode.Selected = true;
			}
		}
		protected bool CanRemove(object item) {
			return editor.CanRemoveInstance(item, tv.Nodes.Count);
		}
		public bool AllowGlyphSkinning {
			get { return this.allowGlyphSkinning; }
			set {
				if(value != allowGlyphSkinning) {
					this.allowGlyphSkinning = value;
					OnAllowGlyphSkinningChanged();
				}
			}
		}
		protected virtual void OnAllowGlyphSkinningChanged() {
			this.buttonAdd.AllowGlyphSkinning = this.buttonRemove.AllowGlyphSkinning = allowGlyphSkinning ? DefaultBoolean.True : DefaultBoolean.False;
			this.propertyGrid.AllowGlyphSkinning = allowGlyphSkinning;
			this.tv.OptionsView.AllowGlyphSkinning = allowGlyphSkinning;
		}
		public bool ShowDescription {
			get { return this.propertyGrid.ShowDescription; }
			set { this.propertyGrid.ShowDescription = value; }
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			isRTLChanged = true;
		}
		private void CollectionEditorContentControl_Load(object sender, EventArgs e) {
			InitializeLayout();
		}
		void InitializeLayout() {
			InitializeButtonsLayout();
			Size minLayoutControlSize = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			if(minLayoutControlSize.Width > ClientSize.Width || minLayoutControlSize.Height > ClientSize.Height)
				ClientSize = new Size(Math.Max(minLayoutControlSize.Width, ClientSize.Width), Math.Max(minLayoutControlSize.Height, ClientSize.Height));
			if(isRTLChanged)
				DevExpress.XtraPrinting.Native.RTLHelper.ConvertGroupControlAlignments(layoutControlGroup1);
		}
		protected virtual void InitializeButtonsLayout() {
			InitializeButtonLayout(buttonAdd, 0, grpButtons);
			InitializeButtonLayout(buttonRemove, 2, grpButtons);
			InitializeButtonLayout(buttonUp, 4, grpButtons);
			InitializeButtonLayout(buttonDown, 6, grpButtons);
		}
		protected void InitializeButtonLayout(SimpleButton button, int col, DevExpress.XtraLayout.LayoutControlGroup group) {
			int buttonBestWidth = button.CalcBestSize().Width;
			if(buttonBestWidth <= button.Width)
				return;
			int delta = buttonBestWidth - button.Width;
			group.OptionsTableLayoutGroup.ColumnDefinitions[col].Width = buttonBestWidth + 2 + 2;
			grpButtons.Width += delta;
		}
	}
}
