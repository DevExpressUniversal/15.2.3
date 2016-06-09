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

using System.Text.RegularExpressions;
using System;
using DevExpress.Utils;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
namespace DevExpress.Utils.Design {
	public class ImageIndexesEditorForm : ObjectPickerControl {
		protected object imageListCore = null;
		protected override bool SupportCustomDrawItem { get { return true; } }
		public object ImageList { get {return imageListCore;} }
		Size GetImageListSize() {
			Size res = ImageCollection.GetImageListSize(ImageList);
			if(res.Height > 32) {
				float div = res.Height / 32f;
				res.Height = (int)((float)res.Height / div);
				res.Width = (int)((float)res.Width / div);
				if(res.Width < 0) res.Width = ImageCollection.GetImageListSize(ImageList).Width;
			}
			return res;
		}
		protected virtual string GetItemText(int imageIndex) { return imageIndex >= 0 ? imageIndex.ToString() : "none"; }
		protected override void CustomDrawItem(DrawItemEventArgs e) {
			int imageIndex = (int)listBox.Items[e.Index];
			string itemText = GetItemText(imageIndex);
			Size imageSize = GetImageListSize();
			DrawImageItemText(e, itemText, imageSize);
			if(ImageCollection.IsImageListImageExists(ImageList, imageIndex)) {
				DrawImageItemImage(e, ImageList, imageIndex, imageSize);
			} else {
				using(Pen redPen = (Pen)Pens.Red.Clone()) {
					redPen.Width = 2;
					e.Graphics.DrawLine(redPen,
						e.Bounds.X,
						e.Bounds.Y,
						e.Bounds.X + imageSize.Width,
						e.Bounds.Y + imageSize.Height);
					e.Graphics.DrawLine(redPen,
						e.Bounds.X + imageSize.Width,
						e.Bounds.Y,
						e.Bounds.X,
						e.Bounds.Y + imageSize.Height);
				}
			}
		}
		public ImageIndexesEditorForm(ImageIndexesEditor editor, object imageList, object editValue)
			: base(editor, editValue) {
			this.imageListCore = imageList;
		}
		public override void Initialize() {
			listBox.ItemHeight = GetImageListSize().Height + 2;
			for(int i = StartIndex; i < ImageCollection.GetImageListImageCount(ImageList); i++) {
				listBox.Items.Add(i);
			}
			int index = EditValue == null ? StartIndex : (int)EditValue;
			if(index + Math.Abs(StartIndex) < listBox.Items.Count && index > StartIndex)
				listBox.SelectedIndex = index + Math.Abs(StartIndex);
			base.Initialize();			
		}
		protected virtual int StartIndex { get { return -1; } }
	}
	public abstract class ObjectPickerEditor : UITypeEditor {
		#region static
		protected static bool IsContextInvalid(ITypeDescriptorContext context) {
			return context == null || context.Instance == null;
		}
		#endregion
		internal IWindowsFormsEditorService edSvc = null;
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null)
				return UITypeEditorEditStyle.DropDown;
			return base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(!CanEditValue(context, provider))
				return value;
			using(ObjectPickerControl objectPickerControl = CreateObjectPickerControl(context, value)) {
				objectPickerControl.Initialize();
				edSvc.DropDownControl(objectPickerControl);
				value = ConvertFromValue(value, objectPickerControl.EditValue);
			}
			return value;
		}
		protected virtual object ConvertFromValue(object oldValue, object newValue) {
			return newValue;
		}
		protected virtual bool CanEditValue(ITypeDescriptorContext context, IServiceProvider provider) {
			if(IsContextInvalid(context) || provider == null)
				return false;
			edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(edSvc == null)
				return false;
			return true;
		}
		protected abstract ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value);
	}
	[ToolboxItem(false)]
	public abstract class ObjectPickerControl : Panel {
		#region static
		protected static void DrawImageItemText(DrawItemEventArgs e, string itemText, Size imageSize) {
			Rectangle rect = new Rectangle(e.Bounds.X + imageSize.Width + 5,
				e.Bounds.Y,
				e.Bounds.Width - imageSize.Width - 5,
				e.Bounds.Height);
			using(StringFormat strFormat = new StringFormat()) {
				strFormat.LineAlignment = StringAlignment.Center;
				using(Brush foreBrush = new SolidBrush(e.ForeColor)) {
					e.Graphics.DrawString(itemText, e.Font, foreBrush, rect, strFormat);
				}
			}
		}
		protected static void DrawImageItemImage(DrawItemEventArgs e, object imageCollection, int imageIndex, Size imageSize) {
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				Rectangle imageBounds = new Rectangle(e.Bounds.Location, imageSize);
				imageBounds.Offset(1, 1);
				ImageCollection.DrawImageListImage(cache, imageCollection, imageIndex, imageBounds);
			}
		}
		protected static void DrawImageItemImage(DrawItemEventArgs e, Image image, Size imageSize) {
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				Rectangle imageBounds = new Rectangle(e.Bounds.Location, imageSize);
				imageBounds.Offset(1, 1);
				cache.Paint.DrawImage(cache.Graphics, image, imageBounds);
			}
		}
		#endregion
		protected ListBox listBox;
		protected object fEditValue;
		protected object originalValue;
		protected ObjectPickerEditor mainEditor;
		protected virtual bool SupportCustomDrawItem { get { return false; } }
		public object EditValue { get { return fEditValue; } }
		protected ObjectPickerControl(ObjectPickerEditor editor, object editValue) {
			mainEditor = editor;
			this.originalValue = this.fEditValue = editValue;
			BorderStyle = BorderStyle.None;
			listBox = new ListBox();
			listBox.Dock = DockStyle.Fill;
			listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			listBox.MouseUp += new MouseEventHandler(lbMouseUp);
			listBox.SelectedIndexChanged += new EventHandler(lbSelectedIndexChanged);
			Controls.Add(listBox);
			if(SupportCustomDrawItem) {
				listBox.DrawMode = DrawMode.OwnerDrawFixed;
				listBox.DrawItem += new DrawItemEventHandler(lbDrawItem);
			}
		}
		void lbDrawItem(object sender, DrawItemEventArgs e) {
			e.DrawBackground();
			CustomDrawItem(e);
			e.DrawFocusRectangle();
		}
		protected virtual void CustomDrawItem(DrawItemEventArgs e) {
		}
		protected virtual int DefaultVisibleItemsCount { get { return 7; } }
		protected virtual int DefaultVisibleWidth { get { return 0; } }
		public virtual void Initialize() {
			Size = new Size(DefaultVisibleWidth, listBox.ItemHeight * DefaultVisibleItemsCount);
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			this.listBox.Focus();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Enter) {
				mainEditor.edSvc.CloseDropDown();
				return true;
			}
			if(keyData == Keys.Escape) {
				fEditValue = originalValue;
				mainEditor.edSvc.CloseDropDown();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		void lbSelectedIndexChanged(object sender, EventArgs e) {
			fEditValue = listBox.SelectedItem;
		}
		void lbMouseUp(object sender, MouseEventArgs e) {
			mainEditor.edSvc.CloseDropDown();
		}
	}
	public class ImageIndexesEditor : ObjectPickerEditor {
		protected virtual object GetImageListInfo(ITypeDescriptorContext context) {
			object imageList = null;
			foreach (object attribute in context.PropertyDescriptor.Attributes) {
				if (attribute is ImageListAttribute) {
					string name = (attribute as ImageListAttribute).Name;
					object obj = DXObjectWrapper.GetInstance(context);
					Type t = obj.GetType();
					PropertyInfo p = t.GetProperty(name);
					if (p != null)
						imageList = p.GetValue(obj, null);
					break;
				}
			}
			return imageList;
		}
		protected override bool CanEditValue(ITypeDescriptorContext context, IServiceProvider provider) {
			return base.CanEditValue(context, provider) && GetImageListInfo(context) != null;
		}
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new ImageIndexesEditorForm(this, GetImageListInfo(context), value);
		}
		public override void PaintValue(PaintValueEventArgs e) {
			object list = GetImageListInfo(e.Context);
			Rectangle r = e.Bounds;
			r.Width--;
			r.Height--;
			e.Graphics.DrawRectangle(SystemPens.ControlText, r);
			r = e.Bounds;
			r.Inflate(-1, -1);
			int imgIndex = e.Value == null ? -1 : (int)e.Value;
			e.Graphics.FillRectangle(SystemBrushes.Window, r);
			Image image = ImageCollection.GetImageListImage(list, imgIndex);
			if(image != null) {
				e.Graphics.FillRectangle(SystemBrushes.Window, r);
				e.Graphics.DrawImage(image, r);
			}
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			if(!IsContextInvalid(context)) return true;
			return base.GetPaintValueSupported();
		}
		private void SaveChanges(object sender, EventArgs e, bool bNeedSave) {
			edSvc.CloseDropDown();
		}
	}
	public class BaseDesignTimeManager : IDisposable {
		ISelectionService selectionService;
		ICollection selectedObjects;
		object owner;
		ISite site;
		public BaseDesignTimeManager() { }
		public BaseDesignTimeManager(object owner, ISite site) {
			Initialize(owner, site);
		}
		public virtual void Initialize(object owner, ISite site) {
			this.owner = owner;
			this.site = site;
			this.selectedObjects = null;
			this.selectionService = null;
		}
		public object Owner { get { return owner; } }
		public virtual ISite Site { get { return site; } }
		public IDesignerHost DesignerHost { get { return GetService(typeof(IDesignerHost)) as IDesignerHost; } }
		public ISelectionService SelectionService { 
			get { 
				if(selectionService == null) InitSelectionService();
				return selectionService;
			}
		}
		public object GetService(Type serviceType) {
			return Site == null ? null : Site.GetService(serviceType);
		}
		public virtual void Dispose() {
			this.owner = null;
			this.site = null;
			this.selectedObjects = null;
			if(this.selectionService != null) {
				this.selectionService.SelectionChanged -= new EventHandler(OnDesignTimeSelectionChanged);
				this.selectionService = null;
			}
		}
		public bool IsComponentSelected(object component) {
			if(SelectionService != null) {
				ICollection components = SelectionService.GetSelectedComponents();
				if(components == null || components.Count == 0) return false;
				return CheckSelected(components, component);
			}
			return false;
		}
		bool CheckSelected(object checkObject, object component) {
			ICollection array = checkObject as ICollection;
			if(array != null) {
				foreach(object obj in array) {
					if(CheckSelected(obj, component)) return true;
				}
				return false;
			}
			return object.Equals(checkObject, component);
		}
		public virtual void DrawSelection(GraphicsCache cache, Rectangle bounds, int alpha, Color color) {
			DrawDesignSelection(cache, bounds, alpha, color);
		}
		public void DrawSelection(GraphicsCache cache, Rectangle bounds, Color color) {
			DrawSelection(cache, bounds, 160, color);
		}
		public static void DrawDesignSelection(GraphicsCache cache, Rectangle bounds, Color color) {
			DrawDesignSelection(cache, bounds, 160, color);
		}
		public static void DrawDesignSelection(GraphicsCache cache, Rectangle bounds, int alpha, Color color) {
			Pen pen = cache.GetPen(Color.FromArgb(alpha, ColorUtils.FlatBarBorderColor));
			Brush brush = cache.GetSolidBrush(Color.FromArgb(alpha, Color.LightPink));
			cache.Paint.DrawRectangle(cache.Graphics, pen, bounds);
			bounds.Inflate(-1, -1);
			cache.Paint.FillRectangle(cache.Graphics, brush, bounds);
		}
		public ICollection PrevSelectedObjects { get { return selectedObjects; } }
		public virtual void InvalidateComponent(object component) {
		}
		public void SelectComponent(object component) {
			SelectComponent(component, ControlConstants.SelectionNormal);
		}
		public void SelectComponent(object component, SelectionTypes selectionType) {
			if(SelectionService != null) SelectionService.SetSelectedComponents(new object[] { component }, selectionType);
		}
		public void SelectComponentDelayed(object component) { SelectComponentDelayed(component, ControlConstants.SelectionNormal); }
		public void SelectComponentDelayed(object component, SelectionTypes selectionType) {
			Timer t = new Timer() { Enabled = true, Interval = 50 };
			t.Tick += (s, e) => {
				if(IsDesignerReady) SelectComponent(component, selectionType);
				((Timer)s).Dispose();
			};
		}
		protected bool IsDesignerReady { get { return DesignerHost != null; } }
		public virtual void InitSelectionService() {
			if(selectionService != null) return;
			this.selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if(this.selectionService == null) return;
			this.selectionService.SelectionChanged += new EventHandler(OnDesignTimeSelectionChanged);
			this.selectedObjects = null;
		}
		protected virtual void OnDesignTimeSelectionChanged(object sender, EventArgs e) {
			if(SelectionService == null) return;
			ICollection current = SelectionService.GetSelectedComponents();
			ICollection diff = GetDifference(PrevSelectedObjects, current);
			this.selectedObjects = current;
			if(diff == null || diff.Count == 0) return;
			foreach(object obj in diff) OnDesignTimeSelectionChanged(obj);
		}
		protected virtual void OnDesignTimeSelectionChanged(object component) {
		}
		ICollection GetDifference(ICollection oldSelection, ICollection newSelection) {
			if(oldSelection == null || oldSelection.Count == 0) {
				if(newSelection == null || newSelection.Count == 0) return null;
				return newSelection;
			}
			if(newSelection == null || newSelection.Count == 0) return oldSelection;
			Hashtable table = new Hashtable();
			AddToHash(table, oldSelection, true);
			AddToHash(table, newSelection, false);
			return table.Keys;
		}
		void AddToHash(Hashtable table, object obj, bool old) {
			Array array = obj as Array;
			if(array != null) {
				foreach(object elem in array) AddToHash(table, elem, old);
				return;
			}
			if(old)	
				table[obj] = 1;
			else {
				if(table.ContainsKey(obj)) 
					table.Remove(obj);
				else 
					table[obj] = 2;
			}
		}
	}
	public delegate object DesignerServiceWrapperCreator();
	public abstract class DesignerServiceWrapper {
		IDesignerHost host = null;
		int refCount = 0;
		public static void InstallService(IDesignerHost host, Type serviceType, DesignerServiceWrapperCreator createMethod) {
			if(host == null) return;
			DesignerServiceWrapper service = host.GetService(serviceType) as DesignerServiceWrapper;
			if(service != null) {
				service.AddRef();
				return;
			}
			service = createMethod() as DesignerServiceWrapper;
			if(service != null) {
				host.AddService(serviceType, service);
				service.InstallService(serviceType, host);
			}
		}
		public static void UnInstallService(IDesignerHost host, Type serviceType) {
			if(host == null) return;
			DesignerServiceWrapper service = host.GetService(serviceType) as DesignerServiceWrapper;
			if(service != null) {
				service.Release();
			}
		}
		protected internal void Release() {
			this.refCount--;
			if(this.refCount < 1) {
				if(host != null) {
					host.RemoveService(serviceType);
					UnInstallService();
				}
				this.host = null;
			}
		}
		protected internal void AddRef() {
			this.refCount++;
		}
		public IDesignerHost Host { get { return host; } }
		Type serviceType;
		protected void InstallService(Type serviceType, IDesignerHost host) {
			this.serviceType = serviceType;
			this.host = host;
			AddRef();
			try {
				InstallServiceCore();
			}
			catch {
			}
		}
		protected abstract void UnInstallService();
		protected abstract void InstallServiceCore();
	}
	public interface IUndoEngine {
		bool Enabled { get; set; }
		bool UndoInProgress { get; }
	}
	public class UndoEngineHelper : IDisposable {
		bool undoEngineSaveState;
		IUndoEngine UndoEngine;
		public UndoEngineHelper(IComponent view) {
			UndoEngine = GetUndoEngine(view);
			DisableUndoEngine();
		}
		public static IUndoEngine GetUndoEngine(IComponent component) {
			if(component == null) return null;
			return EditorContextHelper.GetDesigner(component) as IUndoEngine;
		}
		public void Dispose() {
			EnableUndoEngine();
			UndoEngine = null;
		}
		protected void DisableUndoEngine() {
			if(UndoEngine == null) return;
			undoEngineSaveState = UndoEngine.Enabled;
			UndoEngine.Enabled = false;
		}
		protected void EnableUndoEngine() {
			if(UndoEngine == null) return;
			UndoEngine.Enabled = undoEngineSaveState;
		}
	}
	public class EditorContextHelper : IWindowsFormsEditorService, ITypeDescriptorContext, IServiceProvider {
		public EditorContextHelper(IDesigner designer, PropertyDescriptor prop) {
			this.designer = designer;
			this.targetProperty = prop;
			if(prop == null) {
				prop = TypeDescriptor.GetDefaultProperty(designer.Component);
				if((prop != null) && typeof(ICollection).IsAssignableFrom(prop.PropertyType)) {
					this.targetProperty = prop;
				}
			}
		}
		public static object GetDesigner(IComponent component) {
			if(component != null && component.Site != null) {
				IDesignerHost host = (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
				if(host != null) return host.GetDesigner(component);
			}
			return null;
		}
		public static object GetServiceFromDesigner(IDesigner designer, IComponent component, Type serviceType) {
			if(designer == null) {
				designer = GetDesigner(component) as IDesigner;
			}
			IServiceProvider provider = designer as IServiceProvider;
			if(provider != null) return provider.GetService(serviceType);
			if(component != null && component.Site != null) return component.Site.GetService(serviceType);
			return null;
		}
		public static void SetPropertyValue(IDesigner designer, object component, string property, object value) {
			SetPropertyValue(designer, component, property, value, null);
		}
		public static void SetPropertyValue(IServiceProvider provider, object component, string property, object value) {
			SetPropertyValue(provider, component, property, value, null);
		}
		public static void SetPropertyValue(IDesigner designer, object component, string property, object value, object notifyComponent) {
			if(designer == null || designer.Component == null || designer.Component.Site == null) return;
			SetPropertyValue(designer.Component.Site, component, property, value, notifyComponent);
		}
		public static void SetPropertyValue(IServiceProvider provider, object component, string property, object value, object notifyComponent) {
			if(provider == null) return;
			IDesignerHost host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			IComponentChangeService service = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			PropertyDescriptor desc = TypeDescriptor.GetProperties(component)[property];
			if(host != null && service != null && desc != null) {
				using(DesignerTransaction transaction1 = host.CreateTransaction(string.Format("Set property '{0}'", property))) {
					service.OnComponentChanging(component, desc);
					desc.SetValue(component, value);
					service.OnComponentChanged(component, desc, null, null);
					transaction1.Commit();
					if(notifyComponent != null) service.OnComponentChanged(notifyComponent, desc, null, null);
				}
			}
		}
		public static void FireChanged(IDesigner designer, object component) {
			if(designer == null || designer.Component == null || designer.Component.Site == null) return;
			FireChanged(designer.Component.Site, component);
		}
		public static void FireChanged(IServiceProvider provider, object component) {
			if(provider == null) return;
			IComponentChangeService service = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(service != null) service.OnComponentChanged(component, null, null, null);
		}
		public static object EditValue(IDesigner designer, object objectToChange, string propName) {
			PropertyDescriptor pd = TypeDescriptor.GetProperties(objectToChange)[propName];
			EditorContextHelper context = new EditorContextHelper(designer, pd);
			UITypeEditor editor = pd.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
			object obj1 = pd.GetValue(objectToChange);
			object obj2 = editor.EditValue(context, context, obj1);
			if(obj2 != obj1) {
				try {
					pd.SetValue(objectToChange, obj2);
				}
				catch(CheckoutException) { }
			}
			return obj2;
		}
		IComponentChangeService changeService;
		IDesigner designer;
		PropertyDescriptor targetProperty;
		void ITypeDescriptorContext.OnComponentChanged() {
			ChangeService.OnComponentChanged(this.designer.Component, this.targetProperty, null, null);
		}
		bool ITypeDescriptorContext.OnComponentChanging() {
			try {
				this.ChangeService.OnComponentChanging(this.designer.Component, this.targetProperty);
			}
			catch(CheckoutException e) {
				if(e != CheckoutException.Canceled) throw;
				return false;
			}
			return true;
		}
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(ITypeDescriptorContext) || serviceType == typeof(IWindowsFormsEditorService)) {
				return this;
			}
			if(this.designer.Component.Site != null) {
				return this.designer.Component.Site.GetService(serviceType);
			}
			return null;
		}
		IComponentChangeService ChangeService {
			get {
				if(changeService == null)
					changeService = (IComponentChangeService)((IServiceProvider)this).GetService(typeof(IComponentChangeService));
				return changeService;
			}
		}
		IContainer ITypeDescriptorContext.Container {
			get {
				if(designer.Component.Site != null) {
					return designer.Component.Site.Container;
				}
				return null;
			}
		}
		object ITypeDescriptorContext.Instance { get { return designer.Component; } }
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor { get { return targetProperty; } }
		DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog) {
			IUIService service1 = (IUIService)((IServiceProvider)this).GetService(typeof(IUIService));
			if(service1 != null) {
				return service1.ShowDialog(dialog);
			}
			return dialog.ShowDialog(this.designer.Component as IWin32Window);
		}
		void IWindowsFormsEditorService.DropDownControl(Control control) { }
		void IWindowsFormsEditorService.CloseDropDown() { }
	}
}
