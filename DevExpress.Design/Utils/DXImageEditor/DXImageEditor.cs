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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using EnvDTE;
using DevExpress.Utils.Menu;
namespace DevExpress.Utils.Design {
	public class DXImageEditor : UITypeEditor {
		static DXAsyncImagePickerForm pickerFormCore = null;
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(provider == null)
				return value;
			IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if(svc == null)
				return value;
			if(ShouldUseRunTimePicker(context)) {
				UITypeEditor ed = GetRuntimeEditor();
				return ed.EditValue(context, provider, value);
			}
			if(pickerFormCore == null) {
				pickerFormCore = new DXAsyncImagePickerForm();
			}
			pickerFormCore.InitServices(provider, CreateResourcePickerUIWrapper(context, provider, value));
			if(!DXImageGalleryStorage.Default.IsLoaded) {
				new AsyncLoadHelper().Run(_ => DXImageGalleryStorage.Default.Load(), _ => {
					if(!pickerFormCore.IsDisposed) pickerFormCore.OnDataLoaded();
				});
			}
			else {
				pickerFormCore.OnDataLoaded();
			}
			if(IdeHasOpenedTopForm) pickerFormCore.TopMost = true;
			DialogResult dlgRes = svc.ShowDialog(pickerFormCore);
			pickerFormCore.TopMost = false;
			if(dlgRes == DialogResult.OK) {
				value = pickerFormCore.EditValue;
				RelatedPropertyResolver.Check(pickerFormCore.Options, context, provider, value);
			}
			return value;
		}
		protected virtual bool IdeHasOpenedTopForm {
			get {
				foreach(Form form in Application.OpenForms) {
					if(form is IDesignTimeTopForm) return true;
				}
				return false;
			}
		}
		protected ResourcePickerUIWrapper CreateResourcePickerUIWrapper(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			return new ResourcePickerUIWrapper(value, context.PropertyDescriptor.PropertyType, provider);
		}
		protected bool ShouldUseRunTimePicker(ITypeDescriptorContext context) {
			if(!ResourcePickerUIWrapper.IsAssemblyAccessible) return true;
			IComponent component = context.Instance as IComponent;
			if(component is DevExpress.XtraEditors.Repository.RepositoryItem)
				component = (component as DevExpress.XtraEditors.Repository.RepositoryItem).OwnerEdit;
			if(component == null)
				return false;
			return component.Site == null;
		}
		protected UITypeEditor GetRuntimeEditor() {
			return new BitmapEditor();
		}
	}
	public class AsyncLoadHelper {
		BackgroundWorker loader;
		public AsyncLoadHelper() {
			this.loader = CreateLoader();
		}
		protected BackgroundWorker CreateLoader() {
			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += DoWork;
			worker.RunWorkerCompleted += OnCompleted;
			return worker;
		}
		Action<DoWorkEventArgs> doWorkAction;
		Action<RunWorkerCompletedEventArgs> onCompleteAction;
		public void Run(Action<DoWorkEventArgs> doWork, Action<RunWorkerCompletedEventArgs> onComplete) {
			this.doWorkAction = doWork;
			this.onCompleteAction = onComplete;
			this.loader.RunWorkerAsync();
		}
		protected void DoWork(object sender, DoWorkEventArgs e) {
			doWorkAction(e);
		}
		protected void OnCompleted(object sender, RunWorkerCompletedEventArgs e) {
			onCompleteAction(e);
		}
	}
	public interface IDefaultResourcePickerServiceProvider {
		void EmulateOkClick();
		object EditValue { get; }
		void AddToContainer(Control container);
		Image AddItemToProject(DXImageGalleryItem itemInfo);
	}
	public class ResourcePickerUIWrapper : IDefaultResourcePickerServiceProvider, IDisposable {
		Form pickerForm;
		IServiceProvider serviceProvider;
		public ResourcePickerUIWrapper(object value, Type propertyType, IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			this.pickerForm = GetDefaultResourcePickerDialog();
			Start(value, propertyType);
			Customize(PickerForm);
		}
		#region IDefaultResourcePickerServiceProvider
		void IDefaultResourcePickerServiceProvider.EmulateOkClick() {
			EmulateClickCore(OkButton);
		}
		object IDefaultResourcePickerServiceProvider.EditValue {
			get { return EditValueCore; }
		}
		void IDefaultResourcePickerServiceProvider.AddToContainer(Control container) {
			AddToContainerCore(container);
		}
		Image IDefaultResourcePickerServiceProvider.AddItemToProject(DXImageGalleryItem itemInfo) {
			return AddItemToProjectCore(itemInfo);
		}
		#endregion
		PropertyInfo editValuePropertyInfo = null;
		protected object EditValueCore {
			get {
				if(editValuePropertyInfo == null)
					editValuePropertyInfo = PickerForm.GetType().GetProperty("EditValue", BindingFlags.Instance | BindingFlags.Public);
				return editValuePropertyInfo.GetValue(PickerForm, null);
			}
		}
		protected void AddToContainerCore(Control container) {
			container.Controls.Clear();
			container.Controls.Add(PickerForm);
			PickerForm.Show();
		}
		protected Image AddItemToProjectCore(DXImageGalleryItem itemInfo) {
			string dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			if(!Directory.Exists(dir)) {
				Directory.CreateDirectory(dir);
			}
			string filePath = Path.Combine(dir, itemInfo.FriendlyName);
			itemInfo.Image.Save(filePath);
			return AddItemToProjectCore(itemInfo, filePath);
		}
		protected Image AddItemToProjectCore(DXImageGalleryItem itemInfo, string imagePath) {
			object importInfo = CreateImportInfo(imagePath, itemInfo.PropertyName, SelectedResource);
			IDictionary resources = ImportedResources;
			if(!resources.Contains(SelectedResource)) {
				Type dictType = typeof(Dictionary<,>);
				IDictionary iDict = Activator.CreateInstance(dictType.MakeGenericType(new Type[] { typeof(string), ImportedResourceInfoType })) as IDictionary;
				ImportedResources[SelectedResource] = iDict;
			}
			string key = itemInfo.PropertyName;
			IDictionary dict = resources[SelectedResource] as IDictionary;
			dict.Add(key, importInfo);
			RefreshListBoxItems(key);
			EmulateOkClick(ResourceView.Project);
			return EditValueCore as Image;
		}
		FieldInfo importedResourcesFieldInfoCore = null;
		protected IDictionary ImportedResources {
			get {
				if(importedResourcesFieldInfoCore == null)
					importedResourcesFieldInfoCore = PickerForm.GetType().GetField("importedResources", BindingFlags.Instance | BindingFlags.NonPublic);
				return importedResourcesFieldInfoCore.GetValue(PickerForm) as IDictionary;
			}
		}
		protected void EmulateOkClick(ResourceView resourceView) {
			FieldInfo fieldInfo = PickerForm.GetType().GetField("resourceView", BindingFlags.Instance | BindingFlags.NonPublic);
			if(fieldInfo != null) {
				fieldInfo.SetValue(PickerForm, resourceView);
				EmulateClickCore(OkButton);
			}
		}
		protected string SelectedResource {
			get { return ResxSelectorComboBox.SelectedItem as string; }
		}
		Type importedResourceInfoTypeCore = null;
		protected Type ImportedResourceInfoType {
			get {
				if(importedResourceInfoTypeCore == null)
					importedResourceInfoTypeCore = PickerForm.GetType().GetNestedType(DefaultResourcePickerDialogImportedResourceInfoTypeName, BindingFlags.NonPublic);
				return importedResourceInfoTypeCore;
			}
		}
		protected object CreateImportInfo(string filePath, string name, string resxName) {
			object res = Activator.CreateInstance(ImportedResourceInfoType);
			FieldInfo fi = null;
			fi = res.GetType().GetField("Filename", BindingFlags.Instance | BindingFlags.Public);
			fi.SetValue(res, filePath);
			fi = res.GetType().GetField("Name", BindingFlags.Instance | BindingFlags.Public);
			fi.SetValue(res, name);
			fi = res.GetType().GetField("ResXName", BindingFlags.Instance | BindingFlags.Public);
			fi.SetValue(res, resxName);
			return res;
		}
		protected void EmulateClickCore(Button button) {
			MethodInfo methodInfo = button.GetType().GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
			methodInfo.Invoke(button, new object[] { EventArgs.Empty });
		}
		protected virtual void Customize(Form form) {
			form.TopLevel = false;
			form.Dock = DockStyle.Fill;
			form.FormBorderStyle = FormBorderStyle.None;
			OkButton.Visible = CancelButton.Visible = false;
		}
		protected void Start(object value, Type propertyType) {
			MethodInfo startMethodInfo = PickerForm.GetType().GetMethod("Start", BindingFlags.Instance | BindingFlags.Public);
			startMethodInfo.Invoke(PickerForm, new object[] { value, propertyType, ResourcePickerService, ServiceProvider });
		}
		protected void End() {
			if(PickerForm.IsDisposed) return;
			MethodInfo endMethodInfo = PickerForm.GetType().GetMethod("End", BindingFlags.Instance | BindingFlags.Public);
			endMethodInfo.Invoke(PickerForm, null);
		}
		Button cancelButtonCore = null;
		protected Button CancelButton {
			get {
				if(cancelButtonCore == null) {
					FieldInfo fieldInfo = PickerForm.GetType().GetField("cancelButton", BindingFlags.Instance | BindingFlags.NonPublic);
					cancelButtonCore = fieldInfo.GetValue(PickerForm) as Button;
				}
				return cancelButtonCore;
			}
		}
		Button okButtonCore = null;
		protected Button OkButton {
			get {
				if(okButtonCore == null) {
					FieldInfo fieldInfo = PickerForm.GetType().GetField("okButton", BindingFlags.Instance | BindingFlags.NonPublic);
					okButtonCore = fieldInfo.GetValue(PickerForm) as Button;
				}
				return okButtonCore;
			}
		}
		ComboBox resxSelectorComboBoxCore = null;
		protected ComboBox ResxSelectorComboBox {
			get {
				if(resxSelectorComboBoxCore == null) {
					FieldInfo fieldInfo = PickerForm.GetType().GetField("resxCombo", BindingFlags.Instance | BindingFlags.NonPublic);
					resxSelectorComboBoxCore = fieldInfo.GetValue(PickerForm) as ComboBox;
				}
				return resxSelectorComboBoxCore;
			}
		}
		object resourcePickerServiceCore = null;
		protected object ResourcePickerService {
			get {
				if(resourcePickerServiceCore == null) {
					resourcePickerServiceCore = GetResourcePickerService();
				}
				return resourcePickerServiceCore;
			}
		}
		protected object GetResourcePickerService() {
			Type type = ResourcePickerAssembly.GetType(ResourcePickerServiceTypeName);
			ConstructorInfo ctorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(_DTE), typeof(IServiceProvider) }, null);
			return ctorInfo.Invoke(new object[] { Dte, ServiceProvider });
		}
		_DTE dteCore = null;
		internal _DTE Dte {
			get {
				if(dteCore == null) {
					dteCore = ServiceProvider.GetService(typeof(_DTE)) as _DTE;
				}
				return dteCore;
			}
		}
		Assembly assemblyCore = null;
		protected Assembly ResourcePickerAssembly {
			get {
				if(assemblyCore == null) {
					assemblyCore = GetResourcePickerAssembly();
				}
				return assemblyCore;
			}
		}
		internal static Assembly GetResourcePickerAssembly() {
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if(string.Equals(assembly.GetName().Name, ResourcePickerAssemblyName, StringComparison.OrdinalIgnoreCase))
					return assembly;
			}
			return null;
		}
		internal static bool IsAssemblyAccessible { get { return GetResourcePickerAssembly() != null; } }
		protected virtual Form GetDefaultResourcePickerDialog() {
			Type type = ResourcePickerAssembly.GetType(DefaultResourcePickerDialogTypeName);
			return Activator.CreateInstance(type) as Form;
		}
		protected void RefreshListBoxItems(string key) {
			MethodInfo mi = PickerForm.GetType().GetMethod("RefreshListBoxItems", BindingFlags.Instance | BindingFlags.NonPublic);
			mi.Invoke(PickerForm, null);
			ResourceList.SelectedItem = key;
		}
		ListBox resourceListCore = null;
		protected ListBox ResourceList {
			get {
				if(resourceListCore == null) {
					FieldInfo fi = PickerForm.GetType().GetField("resourceList", BindingFlags.Instance | BindingFlags.NonPublic);
					resourceListCore = fi.GetValue(PickerForm) as ListBox;
				}
				return resourceListCore;
			}
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				End();
				if(this.pickerForm != null)
					this.pickerForm.Dispose();
				this.pickerForm = null;
			}
		}
		public static readonly string ResourcePickerAssemblyName = "Microsoft.VisualStudio.Windows.Forms";
		public static readonly string DefaultResourcePickerDialogTypeName = "Microsoft.VisualStudio.Windows.Forms.ResourcePickerDialog+ResourcePickerUI";
		public static readonly string ResourcePickerServiceTypeName = "Microsoft.VisualStudio.Windows.Forms.ResourcePickerService";
		public static readonly string DefaultResourcePickerDialogImportedResourceInfoTypeName = "ImportedResourceInfo";
		#region ResourceView
		public enum ResourceView {
			Form,
			Project
		}
		#endregion
		protected Form PickerForm { get { return pickerForm; } }
		protected IServiceProvider ServiceProvider { get { return serviceProvider; } }
	}
	public static class RelatedPropertyResolver {
		public static void Check(DXImagePickerFormOptions options, ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(!ShouldRefreshRelatedProperty(context, options, value))
				return;
			PropertyDescriptor relatedPd = GetTargetProperty(options, context);
			if(relatedPd == null)
				return;
			DXImageGalleryItem targetItem = GetTargetItem(options);
			if(targetItem == null)
				return;
			Image image;
			if(options.ResourceType == DXImageGalleryResourceType.Form) {
				image = targetItem.Image;
			}
			else {
				IDefaultResourcePickerServiceProvider defaultPsp = new ResourcePickerUIWrapper(value, context.PropertyDescriptor.PropertyType, provider);
				image = defaultPsp.AddItemToProject(targetItem);
			}
			relatedPd.SetValue(context.Instance, image);
		}
		static bool ShouldRefreshRelatedProperty(ITypeDescriptorContext context, DXImagePickerFormOptions options, object value) {
			if(value == null || options.SelectedItem == null)
				return false;
			if(options.ImageSource != DXImageGalleryImageSource.Gallery)
				return false;
			bool large = IsLarge(context.PropertyDescriptor);
			return (large && options.SelectedItem.ItemSize == DXImageGalleryItemSize.Large) || (!large && options.SelectedItem.ItemSize == DXImageGalleryItemSize.Small);
		}
		static PropertyDescriptor GetTargetProperty(DXImagePickerFormOptions options, ITypeDescriptorContext context) {
			PropertyDescriptor resPd = null;
			PropertyDescriptor srcPd = context.PropertyDescriptor;
			string propertyName = IsLarge(srcPd) ? srcPd.Name.Substring(LargePropertyPrefix.Length) : LargePropertyPrefix + srcPd.Name;
			try {
				resPd = TypeDescriptor.GetProperties(context.Instance)[propertyName];
			}
			catch { }
			return resPd;
		}
		static DXImageGalleryItem GetTargetItem(DXImagePickerFormOptions options) {
			string targetItemName = options.SelectedItem.RelatedName;
			return DXImageGalleryStorage.Default.DataModel.FindObjectByName(targetItemName, options.SelectedItem.ItemType);
		}
		static bool IsLarge(PropertyDescriptor pd) {
			return pd.Name.StartsWith(LargePropertyPrefix, StringComparison.Ordinal);
		}
		static readonly string LargePropertyPrefix = "Large";
	}
	public static class DXImageEditorUtils {
		static Timer Timer { get; set; }
		public static void PostponedCall(Action<object> callback, object state, int delay = 200) {
			Timer = CreateTimer(callback, state, delay);
			Timer.Start();
		}
		static Timer CreateTimer(Action<object> callback, object state, int delay) {
			Timer timer = new Timer();
			timer.Tick += (sender, e) => {
				if(Timer == null) return;
				Release();
				callback(state);
			};
			timer.Interval = delay;
			return timer;
		}
		static void Release() {
			if(Timer == null) return;
			Timer.Dispose();
			Timer = null;
		}
	}
}
