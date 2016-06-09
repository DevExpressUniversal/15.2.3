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
using System.Text;
using DevExpress.Web.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.Web.ASPxGauges.Gauges.Circular;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Base;
using DevExpress.Web.ASPxGauges.Base;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Drawing;
using System.ComponentModel.Design;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.Web.ASPxGauges.Gauges.Linear;
using DevExpress.Web.ASPxGauges.Gauges.State;
using DevExpress.Web.ASPxGauges.Gauges;
using DevExpress.Web.ASPxGauges.Gauges.Digital;
namespace DevExpress.Web.ASPxGauges.Design {
	[Obsolete]
	public static class DesignerMessages {
		public static string CollectionEditorWarning =
			"Editing this Collection via the Collection Editor is strongly not recommended.\r\n" +
			"Please use the Visual Designer instead. Do you still want to edit the collection via the Collection Editor?";
	}
	public class GaugeCollectionTypeEditor : DevExpress.Web.Design.CollectionEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			using(new ASPxGaugeControl.ComponentsHost(context, provider, value)) {
				return base.EditValue(context, provider, value);
			}
		}
		public override Form CreateEditorForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue) {
			return new GaugeCollectionEditorForm(component, context, provider, propertyValue);
		}
	}
	public class ComponentCollectionTypeEditor : DevExpress.Web.Design.CollectionEditor {
		public override Form CreateEditorForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue) {
			return new ComponentCollectionEditorForm(component, context, provider, propertyValue);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			DialogResult result = DevExpress.XtraGauges.Presets.PresetManager.CollectionEditorWarning.Show("Gauges");
			switch(result) {
				case DialogResult.No:
					using(new BaseGaugeWeb.ComponentsHost(context, provider, value)) {
						return base.EditValue(context, provider, value);
					}
				case DialogResult.Yes:
					ASPxGaugeControl control = FindGaugeControl(context, provider);
					if(control != null) {
						ASPxGaugeControlDesigner designer = control.Designer as ASPxGaugeControlDesigner;
						if(designer != null)
							designer.CustomizeCurrent();
					}
					break;
			}
			return value;
		}
		ASPxGaugeControl FindGaugeControl(ITypeDescriptorContext context, IServiceProvider provider) {
			ISelectionService selectionService = (ISelectionService)provider.GetService(typeof(ISelectionService));
			ASPxGaugeControl control = selectionService.PrimarySelection as ASPxGaugeControl;
			if(control == null) {
				IGauge gauge = context.Instance as IGauge;
				if(gauge != null)
					control = gauge.Container as ASPxGaugeControl;
			}
			return control;
		}
	}
	public class ComponentCollectionEditorForm : ItemsEditorFormBase {
		ListBox itemsListBoxCore;
		ListBoxItemPainter fListBoxItemPainter;
		public ComponentCollectionEditorForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue) {
			this.fListBoxItemPainter = new ListBoxItemPainter(true);
		}
		protected override void AssignControls() {
			FillListBox(ItemsList);
			if(ItemsList.Items.Count > 0) ItemsList.SelectedIndex = 0;
			base.AssignControls();
		}
		protected override void SetVisiblePropertyInViewer() {
			ItemsList.Refresh();
		}
		protected override void FocusItemViewer() {
			ItemsList.Focus();
		}
		protected override IList GetParentViewerItemCollection() {
			return ItemsList.Items;
		}
		protected override Control CreateItemViewer() {
			this.itemsListBoxCore = this.fListBoxItemPainter.CreateListBox(this.Font);
			this.ItemsList.ContextMenuStrip = base.PopupMenu;
			this.ItemsList.Dock = DockStyle.Fill;
			this.ItemsList.Margin = new Padding(0);
			this.ItemsList.SelectedIndexChanged += new EventHandler(this.OnSelectedIndexChanged);
			this.ItemsList.MouseDown += new MouseEventHandler(this.OnListBoxMouseDown);
			this.ItemsList.SelectionMode = SelectionMode.MultiExtended;
			return this.ItemsList;
		}
		void OnListBoxMouseDown(object sender, MouseEventArgs e) {
			if((e.Button == MouseButtons.Right) && (ItemsList.Items.Count != 0)) {
				int num = e.Y / ItemsList.ItemHeight;
				ItemsList.SelectedItems.Clear();
				ItemsList.SelectedItem = GetListBoxItemName(CalcSelectedItem(num));
				UpdateTools();
			}
		}
		object CalcSelectedItem(int num) {
			return (num < ItemsList.Items.Count) ? Components[num] : Components[ItemsList.Items.Count - 1];
		}
		void OnSelectedIndexChanged(object sender, EventArgs e) {
			PropertyGrid.SelectedObjects = GetSelectedItems();
			UpdateToolStrip();
		}
		void FillListBox(ListBox listBox) {
			listBox.Items.Clear();
			foreach(object item in Components) {
				listBox.Items.Add(GetListBoxItemName(item));
			}
		}
		string GetListBoxItemName(object item) {
			return (item is INamed) ? ((INamed)item).Name : null;
		}
		protected override string GetFormCaption() {
			return EditingPropertyName + " Editor";
		}
		protected object[] GetSelectedItems() {
			object[] objArray = new object[ItemsList.SelectedItems.Count];
			for(int i = 0; i < objArray.Length; i++) {
				objArray[i] = GetItemByName(ItemsList.SelectedItems[i] as string);
			}
			return objArray;
		}
		object GetItemByName(string name) {
			foreach(INamed item in Components) {
				if(item.Name == name) return item;
			}
			return null;
		}
		protected override void SelectAll() {
			for(int i = 0; i < ItemsList.Items.Count; i++) {
				this.ItemsList.SetSelected(i, true);
			}
		}
		protected override object CreateNewItem() {
			Type componentType = null;
			Type[] genericTypes = Components.GetType().BaseType.GetGenericArguments();
			if(genericTypes.Length > 0)
				componentType = genericTypes[0];
			else {
				List<CollectionItemType> types = GetCollectionItemTypes();
				if(types.Count > 0) componentType = types[0].Type;
			}
			object item = Activator.CreateInstance(componentType, true);
			CheckName(item);
			return item;
		}
		protected override bool IsMoveDownButtonEnabled() {
			return ((ItemsList.SelectedIndex < (ItemsList.Items.Count - 1)) && (ItemsList.SelectedIndex != -1));
		}
		protected override bool IsMoveUpButtonEnabled() {
			return (ItemsList.SelectedIndex > 0);
		}
		protected ListBox ItemsList {
			get { return this.itemsListBoxCore; }
		}
		protected override bool IsRemoveAllButtonEnable() {
			return Components.Count > 0;
		}
		protected override bool IsRemoveButtonEnable() {
			return Components.Count > 0;
		}
		protected override bool IsInsertButtonEnable() {
			return Components.Count > 0;
		}
		protected override string GetPropertyStorePathPrefix() {
			return "ComponentCollectionEditorForm";
		}
		protected IList Components {
			get { return (PropertyValue as IList); }
		}
		protected override void OnAddNewItem(object item) {
			Components.Add(item);
			ItemsList.Items.Add(GetListBoxItemName(item));
			UpdateSelection(item);
		}
		protected override void OnInsertItem(object item) {
			Components.Insert(ItemsList.SelectedIndex, item);
			ItemsList.Items.Insert(ItemsList.SelectedIndex, GetListBoxItemName(item));
			UpdateSelection(item);
		}
		void UpdateSelection(object item) {
			ItemsList.SelectedItems.Clear();
			ItemsList.SelectedItem = GetListBoxItemName(item);
			ItemsList.Update();
		}
		protected override void RemoveItem() {
			int selectedIndex = ItemsList.SelectedIndex;
			object[] objArray = GetSelectedItems();
			for(int i = 0; i < objArray.Length; i++) {
				Components.Remove(objArray[i]);
				ItemsList.Items.Remove(GetListBoxItemName(objArray[i]));
			}
			ItemsList.SelectedIndex = Math.Min(selectedIndex, ItemsList.Items.Count - 1);
			if(Components.Count == 0) PropertyGrid.SelectedObject = null;
			base.RemoveItem();
		}
		protected override void RemoveAllItems() {
			Components.Clear();
			ItemsList.Items.Clear();
		}
		protected void CheckName(object item) {
			INamed named = item as INamed;
			if(named != null && string.IsNullOrEmpty(named.Name)) {
				string[] names = new string[Components.Count];
				for(int i = 0; i < Components.Count; i++) {
					names[i] = ((INamed)Components[i]).Name;
				}
				named.Name = UniqueNameHelper.GetUniqueName(item.GetType().Name, names, 0);
			}
		}
	}
	public class GaugeCollectionEditorForm : ComponentCollectionEditorForm {
		public GaugeCollectionEditorForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue) {
			((ASPxGaugeControl)Component).EditorForm = this;
		}
		protected override List<CollectionItemType> GetCollectionItemTypes() {
			List<CollectionItemType> list = new List<CollectionItemType>();
			list.Add(new CollectionItemType(typeof(CircularGauge), "Circular Gauge"));
			list.Add(new CollectionItemType(typeof(LinearGauge), "Linear Gauge"));
			list.Add(new CollectionItemType(typeof(DigitalGauge), "Digital Gauge"));
			list.Add(new CollectionItemType(typeof(StateIndicatorGauge), "StateIndicator Gauge"));
			return list;
		}
		protected override void OnItemCreated(object item) {
			CheckName(item);
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			((ASPxGaugeControl)Component).EditorForm = null;
		}
	}
	public class IndicatorStateWebCollectionEditor : System.ComponentModel.Design.CollectionEditor {
		public IndicatorStateWebCollectionEditor(Type type)
			: base(type) {
		}
		protected override object CreateInstance(Type type) {
			if(Context != null && (Context.Instance is IScaleComponent)) return new ScaleIndicatorStateWeb();
			else return new IndicatorStateWeb();
		}
		protected override Type CreateCollectionItemType() {
			return typeof(IIndicatorState);
		}
	}
	public class RangeWebCollectionEditor : System.ComponentModel.Design.CollectionEditor {
		public RangeWebCollectionEditor(Type type)
			: base(type) {
		}
		protected override object CreateInstance(Type type) {
			if(Context == null) return null;
			return (Context.Instance is ArcScaleComponent) ? (IRange)new ArcScaleRangeWeb() : (IRange)new LinearScaleRangeWeb();
		}
		protected override Type CreateCollectionItemType() {
			return typeof(IRange);
		}
	}
	public class LabelWebCollectionEditor : System.ComponentModel.Design.CollectionEditor {
		public LabelWebCollectionEditor(Type type)
			: base(type) {
		}
		protected override object CreateInstance(Type type) {
			return new ScaleLabelWeb();
		}
		protected override Type CreateCollectionItemType() {
			return typeof(ILabel);
		}
	}
}
