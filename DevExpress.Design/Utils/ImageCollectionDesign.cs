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
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils.About;
using DevExpress.Utils.Design.SolutionImagePicker;
using VSLangProj;
using DevExpress.Utils.Design.ProjectImagePicker;
using DevExpress.XtraEditors;
namespace DevExpress.Utils.Design {
	public class ImageInfoImageEditor : UITypeEditor {
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) { return true; }
		public override void PaintValue(PaintValueEventArgs e) {
			ImageInfo ii = e.Value as ImageInfo;
			if(ii != null && ii.Image != null) {
				Rectangle bounds = e.Bounds;
				bounds.Width--;
				bounds.Height--;
				e.Graphics.DrawRectangle(SystemPens.WindowFrame, bounds);
				e.Graphics.DrawImage(ii.Image, e.Bounds);
				return;
			}
			base.PaintValue(e);
		}
	}
	public class ImageCollectionCodeDomSerializer : CodeDomSerializer {
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			if(manager == null || codeObject == null) {
				throw new ArgumentNullException();
			}
			CodeDomSerializer serializer = (CodeDomSerializer)manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));
			if(serializer == null) return null;
			return serializer.Deserialize(manager, codeObject);
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			object res = ((CodeDomSerializer)manager.GetSerializer(typeof(ImageCollection).BaseType, typeof(CodeDomSerializer))).Serialize(manager, value);
			ImageCollection list = value as ImageCollection;
			if(list == null) return res;
			if(!(res is CodeStatementCollection)) return res;
			CodeExpression targetObject = GetExpression(manager, value);
			if(targetObject == null) return res;
			CodeExpression cx = new CodePropertyReferenceExpression(targetObject, "Images");
			if(cx == null) return res;
			CodeStatementCollection statements = (CodeStatementCollection)res;
			ImageCollectionImageSerializationContext context = new ImageCollectionImageSerializationContext(list, targetObject, statements);
			DoSerializeRegisteredAssemblies(context);
			for(int n = 0; n < list.Images.Count; n++) {
				ImageInfo ii = list.Images.InnerImages[n];
				if(string.IsNullOrEmpty(ii.Name)) continue;
				var helper = ImageInfoCodeDomSerializationHelperBase.Create(manager, ii.GetType());
				helper.Serialize(context, ii, n);
			}
			return res;
		}
		protected virtual void DoSerializeRegisteredAssemblies(ImageCollectionImageSerializationContext context) {
			if(context.RegisteredAssemblies == null || context.RegisteredAssemblies.IsEmpty) return;
			context.Statements.Add(CreateRegisterAssembliesMethodCall(context));
		}
		protected virtual CodeExpression CreateRegisterAssembliesMethodCall(ImageCollectionImageSerializationContext context) {
			return new CodeMethodInvokeExpression(context.TargetObject, "RegisterAssemblies", GetRegisterAssembliesMethodArg(context));
		}
		protected virtual CodeExpression GetRegisterAssembliesMethodArg(ImageCollectionImageSerializationContext context) {
			IList<CodeExpression> asms = new List<CodeExpression>();
			foreach(string assemblyName in context.RegisteredAssemblies) {
				asms.Add(new CodePrimitiveExpression(assemblyName));
			}
			return new CodeArrayCreateExpression(typeof(string[]), asms.ToArray());
		}
	}
	public class ImageCollectionImageSerializationContext {
		ImageCollection collection;
		CodeExpression targetObject;
		CodeStatementCollection statements;
		ImageCollectionRegisteredAssemblies registeredAssemblies;
		public ImageCollectionImageSerializationContext(ImageCollection collection, CodeExpression targetObject, CodeStatementCollection statements) {
			this.collection = collection;
			this.targetObject = targetObject;
			this.statements = statements;
			this.registeredAssemblies = ImageCollectionRegisteredAssemblies.Create(collection.Images.InnerImages);
		}
		public ImageCollection Collection { get { return collection; } }
		public CodeExpression TargetObject { get { return targetObject; } }
		public CodeStatementCollection Statements { get { return statements; } }
		public ImageCollectionRegisteredAssemblies RegisteredAssemblies { get { return registeredAssemblies; } }
	}
	public abstract class ImageInfoCodeDomSerializationHelperBase {
		public static ImageInfoCodeDomSerializationHelperBase Create(IDesignerSerializationManager manager, Type type) {
			if(type == typeof(ImageInfo)) return new ImageInfoCodeDomSerializationHelper();
			if(type == typeof(ProjectImageInfo)) return new ProjectImageInfoCodeDomSerializationHelper();
			if(type == typeof(DXGalleryImageInfo)) {
				var project = GetProject(manager);
				if(ProjectHelper.IsManagedCppProject(project)) return new ManagedCppDXGalleryImageInfoCodeDomSerializationHelper();
				return new DXGalleryImageInfoCodeDomSerializationHelper();
			}
			if(type == typeof(SolutionImageInfo)) return new SolutionImageInfoCodeDomSerializationHelper();
			throw new ArgumentException("type");
		}
		public void Serialize(ImageCollectionImageSerializationContext context, ImageInfo imageInfo, int n) {
			SerializeCore(context, imageInfo, n);
			AddSetKeyNameStatement(context, imageInfo, n);
		}
		protected void AddSetKeyNameStatement(ImageCollectionImageSerializationContext context, ImageInfo imageInfo, int n) {
			CodeExpression cx = new CodePropertyReferenceExpression(context.TargetObject, "Images");
			var cme = new CodeMethodInvokeExpression(cx, "SetKeyName", new CodeExpression[] { new CodePrimitiveExpression(n), new CodePrimitiveExpression(imageInfo.Name) });
			context.Statements.Add(cme);
		}
		static EnvDTE.Project GetProject(IDesignerSerializationManager manager) {
			if(manager == null) return null;
			var item = manager.GetService(typeof(EnvDTE.ProjectItem)) as EnvDTE.ProjectItem;
			if(item == null) return null;
			return item.ContainingProject;
		}
		protected abstract void SerializeCore(ImageCollectionImageSerializationContext context, ImageInfo imageInfo, int n);
	}
	public class ImageInfoCodeDomSerializationHelper : ImageInfoCodeDomSerializationHelperBase {
		protected override void SerializeCore(ImageCollectionImageSerializationContext context, ImageInfo imageInfo, int n) {
		}
	}
	public class ProjectImageInfoCodeDomSerializationHelper : ImageInfoCodeDomSerializationHelperBase {
		protected override void SerializeCore(ImageCollectionImageSerializationContext context, ImageInfo imageInfo, int n) {
			var mie = CreateAddProjectImageExpression(context, (ProjectImageInfo)imageInfo, n);
			context.Statements.Add(mie);
		}
		protected CodeExpression CreateAddProjectImageExpression(ImageCollectionImageSerializationContext context, ProjectImageInfo pii, int n) {
			return new CodeMethodInvokeExpression(context.TargetObject, "InsertImage", GetArgsList(pii, n));
		}
		protected CodeExpression[] GetArgsList(ProjectImageInfo pi, int index) {
			CodeTypeReference tre = new CodeTypeReference(pi.ParentType, CodeTypeReferenceOptions.GlobalReference);
			List<CodeExpression> args = new List<CodeExpression>();
			args.Add(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(tre), pi.ResourceName));
			args.Add(new CodePrimitiveExpression(pi.Name));
			args.Add(new CodeTypeOfExpression(tre));
			args.Add(new CodePrimitiveExpression(index));
			if(!string.Equals(pi.Name, pi.ResourceName, StringComparison.Ordinal)) {
				args.Add(new CodePrimitiveExpression(pi.ResourceName));
			}
			return args.ToArray();
		}
	}
	public class DXGalleryImageInfoCodeDomSerializationHelper : ImageInfoCodeDomSerializationHelperBase {
		protected override void SerializeCore(ImageCollectionImageSerializationContext context, ImageInfo imageInfo, int n) {
			var mie = CreateAddGalleryItemExpression(context, (DXGalleryImageInfo)imageInfo, n);
			context.Statements.Add(mie);
		}
		protected CodeExpression CreateAddGalleryItemExpression(ImageCollectionImageSerializationContext context, DXGalleryImageInfo imageInfo, int n) {
			return new CodeMethodInvokeExpression(context.TargetObject, "InsertGalleryImage", GetArgsList(imageInfo, n));
		}
		protected CodeExpression[] GetArgsList(DXGalleryImageInfo imageInfo, int n) {
			var mie = new CodeMethodInvokeExpression(CreateCacheReferenceExp(), "GetImage", new CodePrimitiveExpression(imageInfo.Uri));
			return new CodeExpression[] { new CodePrimitiveExpression(imageInfo.Name), new CodePrimitiveExpression(imageInfo.Uri), mie, new CodePrimitiveExpression(n) };
		}
		protected virtual CodeExpression CreateCacheReferenceExp() {
			return new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(CacheTypeName), "Default");
		}
		public static readonly string CacheTypeName = "DevExpress.Images.ImageResourceCache";
	}
	public class SolutionImageInfoCodeDomSerializationHelper : ImageInfoCodeDomSerializationHelperBase {
		protected override void SerializeCore(ImageCollectionImageSerializationContext context, ImageInfo imageInfo, int n) {
			context.Statements.Add(CreateAddSolutionImageExpression(context, (SolutionImageInfo)imageInfo, n));
		}
		protected virtual CodeExpression CreateAddSolutionImageExpression(ImageCollectionImageSerializationContext context, SolutionImageInfo info, int n) {
			return new CodeMethodInvokeExpression(context.TargetObject, "InsertImage", GetArgsList(context, info, n));
		}
		protected virtual CodeExpression[] GetArgsList(ImageCollectionImageSerializationContext context, SolutionImageInfo info, int index) {
			List<CodeExpression> args = new List<CodeExpression>();
			args.Add(new CodePrimitiveExpression(context.RegisteredAssemblies.GetAssemblyId(info.AssemblyName)));
			args.Add(new CodePrimitiveExpression(info.Name));
			args.Add(new CodePrimitiveExpression(info.Path));
			args.Add(new CodePrimitiveExpression(index));
			return args.ToArray();
		}
	}
	public class ManagedCppDXGalleryImageInfoCodeDomSerializationHelper : DXGalleryImageInfoCodeDomSerializationHelper {
		protected override CodeExpression CreateCacheReferenceExp() {
			var exp = base.CreateCacheReferenceExp();
			return new CodeCastExpression(CacheTypeName, exp);
		}
	}
	public class SharedImageCollectionCodeDomSerializer : CodeDomSerializer {
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			if(manager == null || codeObject == null) {
				throw new ArgumentNullException();
			}
			CodeDomSerializer serializer = (CodeDomSerializer)manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));
			if(serializer == null) return null;
			return serializer.Deserialize(manager, codeObject);
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			SharedImageCollection list = value as SharedImageCollection;
			object result = ((CodeDomSerializer)manager.GetSerializer(typeof(SharedImageCollection).BaseType, typeof(CodeDomSerializer))).Serialize(manager, value);
			if(SharedImageCollection.AllowModifyingResources) {
				SerializeResourceInvariant(manager, TimestampResourceName, TimestampValue);
				SerializeResourceInvariant(manager, ImageSizeResourceName, GetImageSizeValue(list));
			}
			return result;
		}
		protected string TimestampResourceName {
			get { return SharedImageCollection.TimestampResourceName; }
		}
		protected string ImageSizeResourceName {
			get { return SharedImageCollection.ImageSizeResourceName; }
		}
		protected DateTime TimestampValue {
			get { return DateTime.Now; }
		}
		protected Size GetImageSizeValue(SharedImageCollection list) {
			return list.ImageSource.ImageSize;
		}
	}
	public class ImageCollectionEditor : CollectionEditor {
		IServiceProvider provider;
		public ImageCollectionEditor(Type type)
			: base(type) {
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.provider = provider;
			return base.EditValue(context, provider, value);
		}
		protected override IList GetObjectsFromInstance(object instance) {
			if(instance == null)
				return null;
			List<ImageInfo> list = instance as List<ImageInfo>;
			if(list != null)
				return list;
			return base.GetObjectsFromInstance(instance);
		}
		protected override Type[] CreateNewItemTypes() {
			return new Type[] { typeof(ImageInfo), typeof(DXGalleryImageInfo), typeof(SolutionImageInfo), typeof(ProjectImageInfo) };
		}
		protected override object CreateInstance(Type type) {
			EnsureItemCreatorRegistry();
			return this.itemCreatorRegistry.GetItemCreator(type).Create(true);
		}
		ImageCollectionItemCreatorRegistry itemCreatorRegistry = null;
		protected void EnsureItemCreatorRegistry() {
			if(this.itemCreatorRegistry == null) this.itemCreatorRegistry = new ImageCollectionItemCreatorRegistry(GetInstance(), this.provider);
		}
		protected virtual ImageCollection GetInstance() {
			if(Context == null) return null;
			object instance = Context.Instance;
			ImageCollection col = instance as ImageCollection;
			if(col == null) {
				if(instance is SharedImageCollection) col = ((SharedImageCollection)instance).ImageSource;
			}
			return col;
		}
		protected override string GetDisplayText(object value) {
			if(value == null) return string.Empty;
			ImageInfo ii = value as ImageInfo;
			if(ii != null) {
				if(!string.IsNullOrEmpty(ii.Name))
					return ii.Name;
				string res = TypeDescriptor.GetConverter(ii.Image).ConvertToString(ii.Image);
				if(!string.IsNullOrEmpty(res))
					return res;
				return value.GetType().Name;
			}
			return string.Empty;
		}
		protected override object[] GetItems(object editValue) {
			Images images = editValue as Images;
			if(images != null)
				return images.InnerImages.ToArray();
			return base.GetItems(editValue);
		}
		protected override CollectionEditor.CollectionForm CreateCollectionForm() {
			CollectionForm frm = base.CreateCollectionForm();
			CustomizePopupMenu(frm);
			return frm;
		}
		ContextMenuStrip popup = null;
		void CustomizePopupMenu(CollectionForm frm) {
			FieldInfo fi = frm.GetType().GetField("addDownMenu", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic);
			if(fi == null)
				return;
			popup = fi.GetValue(frm) as ContextMenuStrip;
			if(popup == null)
				return;
			foreach(ToolStripItem item in popup.Items) item.Text = GetCustomizedText(item);
		}
		string GetCustomizedText(ToolStripItem item) {
			if(string.Equals(typeof(ImageInfo).Name, item.Text, StringComparison.Ordinal))
				return "Load From Disk";
			if(string.Equals(typeof(ProjectImageInfo).Name, item.Text, StringComparison.Ordinal))
				return "Load From Project Resources";
			if(string.Equals(typeof(DXGalleryImageInfo).Name, item.Text, StringComparison.Ordinal))
				return "Load From DevExpress Gallery";
			if(string.Equals(typeof(SolutionImageInfo).Name, item.Text, StringComparison.Ordinal))
				return "Load From Referenced Assemblies";
			return item.Text;
		}
	}
	public class ImageCollectionDesigner : ComponentDesigner {
		string lastFileName;
		public ImageCollectionDesigner() {
			this.lastFileName = string.Empty;
		}
		public const string OpenImageFilterString = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.PNG;*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
		DesignerActionListCollection actionLists = null;
		public override DesignerActionListCollection ActionLists {
			get {
				if(actionLists == null) {
					this.actionLists = new DesignerActionListCollection();
					this.actionLists.Add(CreateActionList());
					DXSmartTagsHelper.CreateDefaultLinks(this, actionLists);
				}
				return actionLists;
			}
		}
		protected virtual DesignerActionList CreateActionList() {
			return new ImageCollectionActionList(Collection, this);
		}
		internal void SetLastFileName(string fileName) {
			this.lastFileName = fileName;
		}
		internal string LastFileName {
			get { return this.lastFileName; }
		}
		DesignerVerbCollection verbs = null;
		public override DesignerVerbCollection Verbs {
			get {
				if(this.verbs == null) {
					this.verbs = CreateVerbs();
				}
				return this.verbs;
			}
		}
		protected DesignerVerbCollection CreateVerbs() {
			DesignerVerbCollection col = new DesignerVerbCollection();
			col.Add(new DesignerVerb("Edit ImageCollection", (s, e) => ImageCollectionAction.EditImages(this, Collection)));
			col.Add(new DesignerVerb("Clear", (s, e) => ImageCollectionAction.Clear(Collection)));
			col.Add(new DesignerVerb("From DevExpress Gallery", (s, e) => ImageCollectionAction.LoadFromGallery(Collection, Component, ServiceProvider)));
			col.Add(new DesignerVerb("From Disk", (s, e) => ImageCollectionAction.AddImage(Collection, this)));
			col.Add(new DesignerVerb("From ImageStrip", (s, e) => ImageCollectionAction.AddImageStrip(Collection)));
			col.Add(new DesignerVerb("From Referenced Assemblies", (s, e) => ImageCollectionAction.LoadFromAssembly(Collection, Component, ServiceProvider)));
			col.Add(new DesignerVerb("From Project Resources", (s, e) => ImageCollectionAction.LoadFromProject(Collection, Component, ServiceProvider)));
			DXSmartTagsHelper.CreateDefaultVerbs(this, col);
			return col;
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			PropertyDescriptor id = (PropertyDescriptor)properties["Images"];
			if(id != null) {
				Attribute[] array = new Attribute[id.Attributes.Count];
				id.Attributes.CopyTo(array, 0);
				properties["Images"] = TypeDescriptor.CreateProperty(typeof(ImageCollectionDesigner), "Images", typeof(InnerImagesList), array);
			}
		}
		protected override void Dispose(bool disposing) {
			ImageCollectionImageCache.Reset();
			base.Dispose(disposing);
		}
		InnerImagesList Images {
			get {
				if(Collection == null) return null;
				return Collection.Images.InnerImages;
			}
		}
		protected IServiceProvider ServiceProvider {
			get { return Component.Site; }
		}
		protected virtual ImageCollection Collection { get { return Component as ImageCollection; } }
	}
	public class ImageCollectionActionList : DesignerActionList {
		IDesigner designer;
		ImageCollection collection;
		public ImageCollectionActionList(ImageCollection collection, IDesigner designer) : base(collection) {
			this.collection = collection;
			this.designer = designer;
		}
		static readonly string LoadingOptionsCategory = "Load Images";
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection items = new DesignerActionItemCollection();
			items.Add(new DesignerActionMethodItem(this, "OnEditImages", "Edit ImageCollection"));
			items.Add(new DesignerActionMethodItem(this, "OnClear", "Clear"));
			items.Add(new DesignerActionHeaderItem(LoadingOptionsCategory, LoadingOptionsCategory));
			items.Add(new DesignerActionMethodItem(this, "OnLoadFromGallery", "From DevExpress Gallery", LoadingOptionsCategory));
			items.Add(new DesignerActionMethodItem(this, "OnAddImageClick", "From Disk", LoadingOptionsCategory));
			items.Add(new DesignerActionMethodItem(this, "OnAddImageStripClick", "From ImageStrip", LoadingOptionsCategory));
			items.Add(new DesignerActionMethodItem(this, "OnLoadFromReferencedAssemblies", "From Referenced Assemblies", LoadingOptionsCategory));
			items.Add(new DesignerActionMethodItem(this, "OnLoadFromProjectResources", "From Project Resources", LoadingOptionsCategory));
			return items;
		}
		protected virtual void OnAddImageStripClick() {
			ImageCollectionAction.AddImageStrip(Collection);
		}
		protected virtual void OnAddImageClick() {
			ImageCollectionAction.AddImage(Collection, Designer as ImageCollectionDesigner);
		}
		protected virtual void OnClear() {
			ImageCollectionAction.Clear(Collection);
		}
		protected virtual void OnEditImages() {
			ImageCollectionAction.EditImages(Designer, Collection);
		}
		protected virtual void OnLoadFromProjectResources() {
			ImageCollectionAction.LoadFromProject(Collection, Component, ServiceProvider);
		}
		protected virtual void OnLoadFromGallery() {
			ImageCollectionAction.LoadFromGallery(Collection, Component, ServiceProvider);
		}
		protected virtual void OnLoadFromReferencedAssemblies() {
			ImageCollectionAction.LoadFromAssembly(Collection, Component, ServiceProvider);
		}
		public IDesigner Designer { get { return designer; } }
		protected IServiceProvider ServiceProvider {
			get { return Designer.Component.Site; }
		}
		public ImageCollection Collection { get { return collection; } }
	}
	internal static class ImageCollectionAction {
		public static void AddImageStrip(ImageCollection col) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = ImageCollectionDesigner.OpenImageFilterString;
			if(DialogResult.OK == dlg.ShowDialog()) {
				Image img = null;
				try {
					img = Image.FromFile(dlg.FileName);
				}
				catch { }
				col.AddImageStrip(img);
			}
		}
		public static void AddImage(ImageCollection col, ImageCollectionDesigner designer) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Multiselect = true;
			dlg.Filter = ImageCollectionDesigner.OpenImageFilterString;
			dlg.FileName = designer.LastFileName;
			if(DialogResult.OK == dlg.ShowDialog()) {
				foreach(string file in dlg.FileNames) {
					designer.SetLastFileName(file);
					Image img = null;
					img = ImageCollection.ImageFromFile(file);
					col.AddImage(img, Path.GetFileName(file));
				}
			}
		}
		public static void Clear(ImageCollection col) {
			if(col.Images.Count < 1) return;
			if(MessageBox.Show("Are you sure you want to clear image collection?", string.Format("Remove {0} images", col.Images.Count), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				col.Clear();
		}
		public static void EditImages(IDesigner designer, ImageCollection col) {
			EditorContextHelper.EditValue(designer, col, "Images");
		}
		public static void LoadFromProject(ImageCollection col, IComponent component, IServiceProvider serviceProvider) {
			EditImages(col, component, serviceProvider, typeof(ProjectImageInfo), false);
		}
		public static void LoadFromGallery(ImageCollection col, IComponent component, IServiceProvider serviceProvider) {
			EditImages(col, component, serviceProvider, typeof(DXGalleryImageInfo), true);
		}
		public static void LoadFromAssembly(ImageCollection col, IComponent component, IServiceProvider serviceProvider) {
			EditImages(col, component, serviceProvider, typeof(SolutionImageInfo), false);
		}
		static void EditImages(ImageCollection col, IComponent component, IServiceProvider serviceProvider, Type itemType, bool preserveSelected) {
			ImageCollectionItemCreatorRegistry reg = new ImageCollectionItemCreatorRegistry(col, serviceProvider);
			IComponentChangeService componentChangeSvc = (IComponentChangeService)serviceProvider.GetService(typeof(IComponentChangeService));
			IList<ImageInfo> list = reg.GetItemCreator(itemType).Create(false);
			if(list == null) return;
			try {
				InnerImagesList innerImages = col.Images.InnerImages;
				componentChangeSvc.OnComponentChanging(component, null);
				ImageCollectionUtils.DoMerge(innerImages, list, itemType, preserveSelected);
			}
			finally {
				componentChangeSvc.OnComponentChanged(component, null, null, null);
			}
		}
	}
	public class ImageCollectionItemCreatorRegistry {
		Dictionary<Type, ImageCollectionItemCreatorBase> registry;
		public ImageCollectionItemCreatorRegistry(ImageCollection collection, IServiceProvider provider) {
			this.registry = CreateRegistry(collection, provider);
		}
		protected virtual Dictionary<Type, ImageCollectionItemCreatorBase> CreateRegistry(ImageCollection collection, IServiceProvider provider) {
			Dictionary<Type, ImageCollectionItemCreatorBase> res = new Dictionary<Type, ImageCollectionItemCreatorBase>();
			res.Add(typeof(ImageInfo), new ImageCollectionImageInfoCreator(collection, provider));
			res.Add(typeof(ProjectImageInfo), new ImageCollectionProjectImageInfoCreator(collection, provider));
			res.Add(typeof(DXGalleryImageInfo), new ImageCollectionGalleryImageInfoCreator(collection, provider));
			res.Add(typeof(SolutionImageInfo), new ImageCollectionSolutionImageInfoCreator(collection, provider));
			return res;
		}
		public ImageCollectionItemCreatorBase GetItemCreator(Type type) {
			if(!registry.ContainsKey(type)) throw new ArgumentException(string.Format("{0} type is not supported yet"));
			return registry[type];
		}
	}
	public abstract class ImageCollectionItemCreatorBase {
		ImageCollection collection;
		IServiceProvider serviceProvider;
		public ImageCollectionItemCreatorBase(ImageCollection collection, IServiceProvider provider) {
			this.collection = collection;
			this.serviceProvider = provider;
		}
		public abstract IList<ImageInfo> Create(bool addOnly);
		protected IServiceProvider ServiceProvider { get { return serviceProvider; } }
		protected ImageCollection Collection { get { return collection; } }
	}
	public class ImageCollectionImageInfoCreator : ImageCollectionItemCreatorBase {
		public ImageCollectionImageInfoCreator(ImageCollection collection, IServiceProvider provider)
			: base(collection, provider) {
		}
		public override IList<ImageInfo> Create(bool addOnly) {
			ImageInfo res = null;
			using(OpenFileDialog dlg = new OpenFileDialog()) {
				dlg.Filter = ImageCollectionDesigner.OpenImageFilterString;
				dlg.Multiselect = true;
				if(DialogResult.OK == dlg.ShowDialog()) {
					List<ImageInfo> list = new List<ImageInfo>();
					foreach(string fname in dlg.FileNames) {
						res = new ImageInfo(Path.GetFileName(fname), ImageCollection.ImageFromFile(fname));
						if(res.Image != null)
							list.Add(res);
					}
					return list;
				}
			}
			return null;
		}
	}
	[CLSCompliant(false)]
	public class ImageCollectionProjectImageInfoCreator : ImageCollectionItemCreatorBase {
		public ImageCollectionProjectImageInfoCreator(ImageCollection collection, IServiceProvider provider)
			: base(collection, provider) {
		}
		public override IList<ImageInfo> Create(bool addOnly) {
			IUIService svc = (IUIService)ServiceProvider.GetService(typeof(IUIService));
			using(ProjectImageSelectionForm form = CreateForm(addOnly, new ProjectImageHolder(Collection, ServiceProvider))) {
				if(svc.ShowDialog(form) == DialogResult.OK) {
					ProjectResourceInfo dlgRes = form.GetValues();
					if(dlgRes != null) {
						List<ImageInfo> res = new List<ImageInfo>();
						foreach(ProjectImage piInfo in dlgRes) {
							ImageInfo li = new ProjectImageInfo(piInfo.Name, piInfo.Image, piInfo.ParentType, piInfo.Name);
							res.Add(li);
						}
						return res;
					}
				}
			}
			return null;
		}
		protected virtual ProjectImageSelectionForm CreateForm(bool addOnly, IProjectImagePickerOwner owner) {
			if(addOnly) return new AddOnlyProjectImageSelectionForm(owner);
			return new ProjectImageSelectionForm(owner);
		}
		class ProjectImageHolder : IProjectImagePickerOwner {
			ImageCollection collection;
			IServiceProvider provider;
			public ProjectImageHolder(ImageCollection collection, IServiceProvider provider) {
				this.collection = collection;
				this.provider = provider;
			}
			public IServiceProvider ServiceProvider {
				get { return provider; }
			}
			public bool IsSelected(ProjectImage img) {
				foreach(ProjectImageInfo imgInfo in Images) {
					if(img.IsMatch(imgInfo)) return true;
				}
				return false;
			}
			IList<ProjectImageInfo> images = null;
			protected IList<ProjectImageInfo> Images {
				get {
					if(this.images == null) {
						this.images = DoLoadImages();
					}
					return this.images;
				}
			}
			protected IList<ProjectImageInfo> DoLoadImages() {
				List<ProjectImageInfo> list = new List<ProjectImageInfo>();
				foreach(ImageInfo img in collection.Images.InnerImages) {
					ProjectImageInfo p = img as ProjectImageInfo;
					if(p != null) list.Add(p);
				}
				return list;
			}
		}
	}
	public class ImageCollectionGalleryImageInfoCreator : ImageCollectionItemCreatorBase {
		public ImageCollectionGalleryImageInfoCreator(ImageCollection collection, IServiceProvider provider)
			: base(collection, provider) {
		}
		public override IList<ImageInfo> Create(bool addOnly) {
			IUIService svc = ServiceProvider.GetService(typeof(IUIService)) as IUIService;
			if(svc == null)
				return null;
			using(DXAsyncImagePickerForm frm = new DXAsyncImagePickerForm(false, true, GetDesiredImageSize())) {
				var picker = new ResourcePickerUIWrapper(null, typeof(Image), ServiceProvider);
				frm.InitServices(ServiceProvider, picker);
				if(!DXImageGalleryStorage.Default.IsLoaded) {
					new AsyncLoadHelper().Run(_ => DXImageGalleryStorage.Default.Load(), _ => {
						if(!frm.IsDisposed) frm.OnDataLoaded();
					});
				}
				else frm.OnDataLoaded();
				if(svc.ShowDialog(frm) == DialogResult.OK) {
					var items = frm.GetGalleryValues();
					if(items.Count() > 0) {
						CheckDevExpressImagesReference();
						List<ImageInfo> res = new List<ImageInfo>();
						foreach(DXImageGalleryItem gi in items) {
							var imageInfo = new DXGalleryImageInfo(gi.FriendlyName, gi.Uri, gi.Image);
							res.Add(imageInfo);
						}
						return res;
					}
				}
			}
			return null;
		}
		void CheckDevExpressImagesReference() {
			EnvDTE.Project project = GetProject();
			if(project == null) return;
			try {
				if(ProjectHelper.IsReferenceExists(project, AssemblyInfo.SRAssemblyImages))
					return;
				ProjectHelper.AddReference(project, AssemblyInfo.SRAssemblyImagesFull);
			}
			catch {
			}
		}
		protected Size? GetDesiredImageSize() {
			return Collection != null ? (Size?)Collection.ImageSize : null;
		}
		EnvDTE.Project GetProject() {
			if(ServiceProvider == null) return null;
			var item = ServiceProvider.GetService(typeof(EnvDTE.ProjectItem)) as EnvDTE.ProjectItem;
			return item != null ? item.ContainingProject : null;
		}
	}
	public class ImageCollectionSolutionImageInfoCreator : ImageCollectionItemCreatorBase {
		public ImageCollectionSolutionImageInfoCreator(ImageCollection collection, IServiceProvider provider)
			: base(collection, provider) {
		}
		public override IList<ImageInfo> Create(bool addOnly) {
			SolutionImageHolder imageHolder = new SolutionImageHolder(Collection, ServiceProvider);
			RootObject rootObj = DataSource.Load(imageHolder);
			if(rootObj == null || rootObj.Projects.Count == 0) {
				XtraMessageBox.Show(WarningMsg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return null;
			}
			IUIService svc = (IUIService)ServiceProvider.GetService(typeof(IUIService));
			using(SolutionImagePickerForm form = CreateForm(addOnly, imageHolder, rootObj)) {
				if(svc.ShowDialog(form) == DialogResult.OK) {
					return form.GetValues();
				}
			}
			return null;
		}
		[CLSCompliant(false)]
		protected SolutionImagePickerForm CreateForm(bool addOnly, ISolutionImagePickerOwner imageHolder, RootObject rootObj) {
			if(addOnly) return new AddOnlySolutionImagePickerForm(imageHolder, rootObj);
			return new SolutionImagePickerForm(imageHolder, rootObj);
		}
		static readonly string WarningMsg = "No referenced assembly with images is found.\nPlease add this assembly to the project's references and build the solution.";
		class SolutionImageHolder : ISolutionImagePickerOwner {
			ImageCollection collection;
			IServiceProvider serviceProvider;
			public SolutionImageHolder(ImageCollection collection, IServiceProvider serviceProvider) {
				this.collection = collection;
				this.serviceProvider = serviceProvider;
			}
			public bool IsSelected(string resourceName) {
				return this.collection.Images.InnerImages.Exists(imageInfo => {
					SolutionImageInfo si = imageInfo as SolutionImageInfo;
					return si != null && string.Equals(si.Path, resourceName, StringComparison.OrdinalIgnoreCase);
				});
			}
			public AssemblyEntityCategory GetAssemblyCategory(EnvDTE.Project ownerProject,  Reference reference) {
				if(IsMsOrOurAssembly(reference)) return AssemblyEntityCategory.Disabled;
				return IsLocalProjectReference(reference, ownerProject) ? AssemblyEntityCategory.Base : AssemblyEntityCategory.Advanced;
			}
			protected virtual bool IsMsOrOurAssembly(Reference assembly) {
				string publicKeyToken = assembly.PublicKeyToken;
				if(string.IsNullOrEmpty(publicKeyToken)) return false;
				return DisabledKeys.Contains(publicKeyToken, StringComparer.OrdinalIgnoreCase); 
			}
			protected bool IsLocalProjectReference(Reference reference, EnvDTE.Project ownerProject) {
				EnvDTE.Project sourceProject = reference.SourceProject;
				if(sourceProject == null) return false;
				foreach(EnvDTE.Project p in ownerProject.DTE.Solution) {
					if(object.ReferenceEquals(p, sourceProject)) return true;
				}
				return false;
			}
			static string[] DisabledKeys = new string[] {
				AssemblyInfo.PublicKeyToken, "b77a5c561934e089", "31bf3856ad364e35", "b03f5f7f11d50a3a", "96d09a1eb7f44a77"
			};
			public IServiceProvider ServiceProvider { get { return serviceProvider; } }
		}
	}
	public class SharedImageCollectionDesigner : ImageCollectionDesigner {
		IDesignerHost root = null;
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			CheckDesignSurface();
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			SubscribeEvents();
		}
		protected override DesignerActionList CreateActionList() {
			return new SharedImageCollectionActionList(DefaultCollection, this);
		}
		protected override ImageCollection Collection { get { return DefaultCollection.ImageSource; } }
		void CheckDesignSurface() {
			bool shouldDelete = false;
			if(Component.Site == null || Component.Site.Container == null)
				return;
			foreach(object component in Component.Site.Container.Components) {
				if(object.ReferenceEquals(component, Component)) continue;
				if(object.ReferenceEquals(component.GetType(), typeof(SharedImageCollection))) {
					shouldDelete = true;
					break;
				}
			}
			if(shouldDelete) {
				Component.Site.Container.Remove(Component);
				MessageBox.Show("SharedImageCollection has already been added to the form. Using multiple SharedImageCollections within a single form is not supported.", "Warning");
			}
		}
		void SubscribeEvents() {
			if(DefaultCollection.ImageSource != null)
				DefaultCollection.ImageSource.Changed += ImageSource_Changed;
			this.root = (IDesignerHost)GetService(typeof(IDesignerHost));
			if(this.root != null) 
				this.root.LoadComplete += Root_LoadComplete;
		}
		void UnsubscribeEvents() {
			if(DefaultCollection.ImageSource != null)
				DefaultCollection.ImageSource.Changed -= ImageSource_Changed;
			if(this.root != null) 
				this.root.LoadComplete -= Root_LoadComplete;
		}
		void ImageSource_Changed(object sender, EventArgs e) {
			if(AllowRefreshImageSource()) RefreshImageSourceCore();
		}
		void Root_LoadComplete(object sender, EventArgs e) {
			if(AllowRefreshImageSourceForNewDesignerInstance()) RefreshImageSourceCore();
		}
		void RefreshImageSourceCore() {
			DesignTimeHelper.RefreshProperty(DefaultCollection, "ImageSource");
		}
		protected override void Dispose(bool disposing) {
			UnsubscribeEvents();
			base.Dispose(disposing);
		}
		protected bool AllowRefreshImageSource() { return !this.root.InTransaction && !ProjectHelper.IsDebuggerLaunched(Component.Site); }
		protected bool AllowRefreshImageSourceForNewDesignerInstance() {
			if(DefaultCollection.InstanceCount <= 1 || ProjectHelper.IsDebuggerLaunched(Component.Site)) return false;
			return DefaultCollection.HasChanges();
		}
		protected SharedImageCollection DefaultCollection { get { return (SharedImageCollection)Component; } }
	}
	public class SharedImageCollectionActionList : ImageCollectionActionList {
		public SharedImageCollectionActionList(SharedImageCollection col, IDesigner designer)
			: base(col.ImageSource, designer) {
		}
	}
}
