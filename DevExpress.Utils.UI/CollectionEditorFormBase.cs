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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Utils;
using DevExpress.Utils.UI.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.Utils.UI {
	public class CollectionEditorFormBase : ReportsEditorFormBase {
		#region fields
		SimpleButton btnCancel;
		SimpleButton btnOk;
		LabelControl bottomLine;
		bool isRTLChanged;
		bool sortProperties = true;
		IServiceProvider provider;
		Container components = null;
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private XtraLayout.LayoutControlGroup Root;
		private XtraLayout.LayoutControlGroup grpButtons;
		private XtraLayout.LayoutControlItem layoutControlItem3;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.LayoutControlItem layoutControlItem4;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		protected CollectionEditorContentControl collectionEditorContent;
		#endregion
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public bool AllowGlyphSkinning {
			get { return this.collectionEditorContent.AllowGlyphSkinning; }
			set {
				this.collectionEditorContent.AllowGlyphSkinning = value;
			}
		}
		CollectionEditorFormBase()
			: base(null) {
			InitializeComponent();
		}
		public CollectionEditorFormBase(IServiceProvider provider, CollectionEditor collectionEditor)
			: base(provider) {
			this.provider = provider;
			this.collectionEditorContent = CreateCollectionEditorContentControl(provider, collectionEditor);
			InitializeComponent();
		}
		public void SelectPrimarySelection() {
			if(provider != null) {
				ISelectionService selectionService = (ISelectionService)provider.GetService(typeof(ISelectionService));
				if(selectionService != null && selectionService.PrimarySelection.GetType() == collectionEditorContent.CollectionItemType &&
					((IComponent)selectionService.PrimarySelection).Site != null) {
					collectionEditorContent.SelectNodeByName(((IComponent)selectionService.PrimarySelection).Site.Name);
				}
			}
		}
		protected virtual CollectionEditorContentControl CreateCollectionEditorContentControl(IServiceProvider provider, CollectionEditor collectionEditor) {
			CollectionEditorContentControl control = new CollectionEditorContentControl(provider, collectionEditor);
			ApplySortProperties(control);
			return control;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectionEditorFormBase));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.bottomLine = new DevExpress.XtraEditors.LabelControl();
			this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl1;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.btnOk);
			this.layoutControl1.Controls.Add(this.bottomLine);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(619, 112, 959, 740);
			this.layoutControl1.Root = this.Root;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.StyleController = this.layoutControl1;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.bottomLine, "bottomLine");
			this.bottomLine.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.bottomLine.LineVisible = true;
			this.bottomLine.Name = "bottomLine";
			this.bottomLine.StyleController = this.layoutControl1;
			this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.Root.GroupBordersVisible = false;
			this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.grpButtons,
			this.layoutControlItem1});
			this.Root.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.Root.Location = new System.Drawing.Point(0, 0);
			this.Root.Name = "Root";
			columnDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition6.Width = 522D;
			this.Root.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition6});
			rowDefinition4.Height = 100D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition5.Height = 44D;
			rowDefinition5.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.Root.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition4,
			rowDefinition5});
			this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.Root.Size = new System.Drawing.Size(522, 370);
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem3,
			this.layoutControlItem2,
			this.layoutControlItem4});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 326);
			this.grpButtons.Name = "grpButtons";
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition1.Width = 100D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition2.Width = 80D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition3.Width = 2D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition4.Width = 80D;
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition5.Width = 5D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4,
			columnDefinition5});
			rowDefinition1.Height = 13D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition2.Height = 26D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition3.Height = 5D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1,
			rowDefinition2,
			rowDefinition3});
			this.grpButtons.OptionsTableLayoutItem.RowIndex = 1;
			this.grpButtons.Size = new System.Drawing.Size(522, 44);
			this.layoutControlItem3.Control = this.btnOk;
			this.layoutControlItem3.Location = new System.Drawing.Point(355, 13);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem3.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem3.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem2.Control = this.bottomLine;
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem2.Size = new System.Drawing.Size(522, 13);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem4.Control = this.btnCancel;
			this.layoutControlItem4.Location = new System.Drawing.Point(437, 13);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem4.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem4.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem1.Control = this.collectionEditorContent;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(522, 326);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			if(this.collectionEditorContent != null) {
				this.collectionEditorContent.AllowGlyphSkinning = false;
				resources.ApplyResources(this.collectionEditorContent, "collectionEditorContent");
				this.collectionEditorContent.Name = "collectionEditorContent";
				this.collectionEditorContent.ShowDescription = true;
				this.layoutControl1.Controls.Add(this.collectionEditorContent);
			}
			this.CancelButton = this.btnCancel;
			resources.ApplyResources(this, "$this");
			this.ControlBox = false;
			this.Controls.Add(this.layoutControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CollectionEditorFormBase";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Load += new System.EventHandler(this.CollectionEditorFormBase_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public object EditValue {
			get { return this.collectionEditorContent.EditValue; }
			set {
				this.collectionEditorContent.EditValue = value;
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public bool ShowDescription {
			get { return collectionEditorContent.ShowDescription; }
			set { collectionEditorContent.ShowDescription = value; }
		}
		protected object[] Items {
			get {
				return this.collectionEditorContent.Items;
			}
			set {
				this.collectionEditorContent.Items = value;
			}
		}
		public bool SortProperties {
			get { return sortProperties; }
			set {
				sortProperties = value;
				ApplySortProperties(collectionEditorContent);
			}
		}
		void ApplySortProperties(CollectionEditorContentControl control) {
			control.propertyGrid.PropertyGridControl.OptionsBehavior.PropertySort = SortProperties ? XtraVerticalGrid.PropertySort.Alphabetical : XtraVerticalGrid.PropertySort.NoSort;
		}
		public virtual DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc) {
			return edSvc.ShowDialog(this);
		}
		public void SelectLastItem() {
			this.collectionEditorContent.SelectLastItem();
		}
		private void btnCancel_Click(object sender, EventArgs e) {
			this.collectionEditorContent.Cancel();
		}
		private void btnOk_Click(object sender, EventArgs e) {
			if(!IsValueValid()) {
				ProcessInvalidValue();
				return;
			}
			this.collectionEditorContent.Finish();
		}
		protected virtual void ProcessInvalidValue() { }
		protected virtual bool IsValueValid() {
			return true;
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			isRTLChanged = true;
		}
		private void CollectionEditorFormBase_Load(object sender, EventArgs e) {
			InitializeLayout();
		}
		void InitializeLayout() {
			InitializeGroupButtonsLayout();
			Size minLayoutControlSize = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			if(minLayoutControlSize.Width > ClientSize.Width || minLayoutControlSize.Height > ClientSize.Height) {
				int newClientSizeHeight = Math.Max(minLayoutControlSize.Height, ClientSize.Height);
				if(minLayoutControlSize.Width > ClientSize.Width)
					newClientSizeHeight = Math.Max(newClientSizeHeight, (int)ClientSize.Height * minLayoutControlSize.Width / ClientSize.Width);
				ClientSize = new Size(Math.Max(minLayoutControlSize.Width, ClientSize.Width), newClientSizeHeight);
			}
			if(isRTLChanged)
			   DevExpress.XtraPrinting.Native.RTLHelper.ConvertGroupControlAlignments(grpButtons);
		}
		void InitializeGroupButtonsLayout() {
			int btnOkBestWidth = btnOk.CalcBestSize().Width;
			int btnCancelBestWidth = btnCancel.CalcBestSize().Width;
			if(btnOkBestWidth <= btnOk.Width && btnCancelBestWidth <= btnCancel.Width)
				return;
			int btnCancelOKActualSize = Math.Max(btnOkBestWidth, btnCancelBestWidth);
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[1].Width =
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width = btnCancelOKActualSize + 2 + 2;
		}
	}
	public class CollectionEditor : UITypeEditor, IItemsContainer, IServiceProvider {
		protected ITypeDescriptorContext currentContext;
		Type type;
		Type collectionItemType;
		Type[] newItemTypes;
		IList values;
		protected internal Type CollectionType {
			get {
				return this.type;
			}
		}
		protected internal Type CollectionItemType {
			get {
				if(this.collectionItemType == null) {
					this.collectionItemType = this.CreateCollectionItemType();
				}
				return this.collectionItemType;
			}
		}
		public CollectionEditor(Type type) {
			this.type = type;
		}
		public bool SelectLastItem { get; set; }
		public bool SelectPrimarySelection { get; set; }
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		protected virtual Type[] CreateNewItemTypes() {
			return new Type[] { this.CollectionItemType };
		}
		protected internal Type[] NewItemTypes {
			get {
				if (this.newItemTypes == null) {
					this.newItemTypes = this.CreateNewItemTypes();
				}
				return this.newItemTypes;
			}
		}
		protected virtual Type CreateCollectionItemType() {
			PropertyInfo[] properties = TypeDescriptor.GetReflectionType(this.CollectionType).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			for (int i = 0; i < properties.Length; i++) {
				if (properties[i].Name.Equals("Item") || properties[i].Name.Equals("Items")) {
					return properties[i].PropertyType;
				}
			}
			return typeof(object);
		}
		protected internal virtual object[] GetItems(object editValue) {
			if ((editValue == null) || !(editValue is ICollection)) {
				return new object[0];
			}
			ArrayList list = new ArrayList();
			ICollection is2 = (ICollection)editValue;
			foreach (object obj2 in is2) {
				list.Add(obj2);
			}
			object[] array = new object[list.Count];
			list.CopyTo(array, 0);
			return array;
		}
		protected internal virtual object SetItems(object editValue, object[] value) {
			if ((editValue != null) && (editValue is IList)) {
				IList list = (IList)editValue;
				list.Clear();
				for (int i = 0; i < value.Length; i++) {
					list.Add(value[i]);
				}
			}
			return editValue;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.currentContext = context;
			IWindowsFormsEditorService edSvc = provider.GetService<IWindowsFormsEditorService>();
			IComponentChangeService changeService = null;
			bool isChanged = true;
			bool skipChangedEvents = false;
			bool skipChangingEvents = false;
			ComponentChangedEventHandler changed = (s, e) => {
				if(!skipChangedEvents && (s != Context.Instance)) {
					skipChangedEvents = true;
					Context.OnComponentChanged();
				}
			};
			ComponentChangingEventHandler changing = (s, e) => {
				if(!skipChangingEvents && (s != Context.Instance)) {
					skipChangingEvents = true;
					Context.OnComponentChanging();
				}
			};
			DesignerTransaction transaction = BeginTransaction();
			try {
				IDesignerHost designerHost = this.GetService<IDesignerHost>();
				changeService = (designerHost != null) ? designerHost.GetService<IComponentChangeService>() : null;
				if(changeService != null) {
					changeService.ComponentChanged += changed;
					changeService.ComponentChanging += changing;
				}
				using(CollectionEditorFormBase form = CreateCollectionForm(provider)) {
					values = GetEditValue(value);
					form.EditValue = values;
					if(SelectLastItem)
						form.SelectLastItem();
					else if(SelectPrimarySelection)
						form.SelectPrimarySelection();
					if(form.ShowEditorDialog(edSvc) == DialogResult.OK) {
						value = ApplyNewEditValue(value, form.EditValue);
						return value;
					}
					isChanged = false;
				}
			} finally {
				EndTransaction(transaction, isChanged);
				if(changeService != null) {
					changeService.ComponentChanged -= changed;
					changeService.ComponentChanging -= changing;
				}
			}
			return value;
		}
		protected virtual IList GetEditValue(object value) {
			return (IList)value;
		}
		protected virtual object ApplyNewEditValue(object oldValue, object formEditValue) {
			return formEditValue;
		}
		protected virtual DesignerTransaction BeginTransaction() {
			IDesignerHost designerHost = this.GetService<IDesignerHost>();
			try {
				if(designerHost != null) {
					string transactionName = string.Format(UtilsUILocalizer.GetString(UtilsUIStringId.CollectionEditor_Cancel), new object[] { CollectionItemType.Name });
					return designerHost.CreateTransaction(transactionName);
				} else {
					return null;
				}
			} catch(CheckoutException exception) {
				if(exception != CheckoutException.Canceled)
					throw exception;
				return null;
			}
		}
		protected virtual void EndTransaction(DesignerTransaction transaction, bool isChanged) {
			if(transaction != null) {
				if(isChanged) {
					transaction.Commit();
				} else {
					transaction.Cancel();
				}
			}
		}
		protected virtual CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			CollectionEditorFormBase form = new CollectionEditorFormBase(serviceProvider, this);
			form.ShowDescription = CultureHelper.IsEnglishCulture(CultureInfo.CurrentCulture);
			return form;
		}
		public ITypeDescriptorContext Context { get { return currentContext; } }
		protected internal virtual object CreateInstance(Type itemType) {
			IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			object component = null;
			if(host != null && typeof(IComponent).IsAssignableFrom(itemType)) {
				component = host.CreateComponent(itemType);
				IComponentInitializer initializer = host.GetDesigner((IComponent)component) as IComponentInitializer;
				if(initializer != null) {
					initializer.InitializeNewComponent(null);
				}
			}
			if(component == null) {
				component = TypeDescriptor.CreateInstance(this, itemType, null, null);
			}
			return component;
		}
		protected internal virtual void DisposeInstance(object instance) {
			IComponent component = instance as IComponent;
			if(component != null) {
				IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
				if(designerHost != null) {
					designerHost.DestroyComponent(component);
				} else {
					component.Dispose();
				}
			} else {
				IDisposable disposable = instance as IDisposable;
				if(disposable != null) {
					disposable.Dispose();
				}
			}
		}
		protected internal virtual IList GetObjectsFromInstance(object instance) {
			ArrayList list = new ArrayList();
			list.Add(instance);
			return list;
		}
		protected internal virtual string GetItemName(object item, int index) {
			PropertyInfo pi = item.GetType().GetProperty("Name");
			if(pi != null && pi.PropertyType == typeof(string))
				return pi.GetValue(item, null) as string;
			string str = TypeDescriptor.GetConverter(item).ConvertToString(item);
			if(!string.IsNullOrEmpty(str)) {
				return str;
			}
			return item.GetType().Name;
		}
		protected virtual internal bool CanRemoveInstance(object item, int count) {
			return item != null;
		}
		public IEnumerable Items {
			get { return this.values; }
		}
		public object GetService(Type serviceType) {
			if(serviceType == typeof(IItemsContainer))
				return this;
			return Context != null ? Context.GetService(serviceType) : null;
		}
	}
	public interface IItemsContainer {
		IEnumerable Items { get; }
	}
	public static class TreeListNodeHelper {
		public static object GetItem(this TreeListNode node) {
			return node != null ? node.Tag : null;
		}
		public static void SetItem(this TreeListNode node, object value) {
			if(node != null)
				node.Tag = value;
		}
	}
}
