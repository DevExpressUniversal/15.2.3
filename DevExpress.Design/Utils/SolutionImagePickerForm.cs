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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using VSLangProj;
namespace DevExpress.Utils.Design.SolutionImagePicker {
	[CLSCompliant(false)]
	public class SolutionImagePickerForm : XtraForm {
		private PanelControl pnlContent;
		private PanelControl pnlBottom;
		private SimpleButton btnCancel;
		private SimpleButton btnOK;
		private PictureEdit previewImage;
		private ImageCollection images;
		private System.ComponentModel.IContainer components;
		private DXSolutionTreeView solutionTreeView;
		RootObject rootObj;
		ISolutionImagePickerOwner imageHolder;
		static SolutionImagePickerForm() { SkinManager.EnableFormSkins(); }
		public SolutionImagePickerForm()
			: this(null, null) {
		}
		public SolutionImagePickerForm(ISolutionImagePickerOwner imageHolder, RootObject rootObj) {
			this.imageHolder = imageHolder;
			this.rootObj = rootObj;
			InitializeComponent();
		}
		#region InitializeComponent
		void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SolutionImagePickerForm));
			this.pnlContent = new DevExpress.XtraEditors.PanelControl();
			this.previewImage = new DevExpress.XtraEditors.PictureEdit();
			this.solutionTreeView = new DevExpress.Utils.Design.DXSolutionTreeView();
			this.images = new DevExpress.Utils.ImageCollection(this.components);
			this.pnlBottom = new DevExpress.XtraEditors.PanelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlContent)).BeginInit();
			this.pnlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.previewImage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.images)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).BeginInit();
			this.pnlBottom.SuspendLayout();
			this.SuspendLayout();
			this.pnlContent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlContent.Controls.Add(this.previewImage);
			this.pnlContent.Controls.Add(this.solutionTreeView);
			this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlContent.Location = new System.Drawing.Point(0, 0);
			this.pnlContent.Name = "pnlContent";
			this.pnlContent.Padding = new System.Windows.Forms.Padding(4);
			this.pnlContent.Size = new System.Drawing.Size(749, 375);
			this.pnlContent.TabIndex = 0;
			this.previewImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.previewImage.Location = new System.Drawing.Point(445, 12);
			this.previewImage.Name = "previewImage";
			this.previewImage.Properties.AllowFocused = false;
			this.previewImage.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.previewImage.Properties.Appearance.Options.UseBackColor = true;
			this.previewImage.Properties.ReadOnly = true;
			this.previewImage.Properties.ShowMenu = false;
			this.previewImage.Size = new System.Drawing.Size(292, 363);
			this.previewImage.TabIndex = 0;
			this.solutionTreeView.AllowSkinning = true;
			this.solutionTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.solutionTreeView.CheckBoxes = true;
			this.solutionTreeView.DefaultExpandCollapseButtonOffset = 5;
			this.solutionTreeView.Location = new System.Drawing.Point(12, 12);
			this.solutionTreeView.Name = "solutionTreeView";
			this.solutionTreeView.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildrenSameBranch;
			this.solutionTreeView.Size = new System.Drawing.Size(421, 362);
			this.solutionTreeView.TabIndex = 0;
			this.solutionTreeView.SelectionChanged += new System.EventHandler(this.OnImageTreeViewSelectionChanged);
			this.solutionTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.OnImageTreeViewAfterCheck);
			this.images.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("images.ImageStream")));
			this.images.Images.SetKeyName(0, "image.png");
			this.images.Images.SetKeyName(1, "folder.png");
			this.images.Images.SetKeyName(2, "project.png");
			this.pnlBottom.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlBottom.Controls.Add(this.btnCancel);
			this.pnlBottom.Controls.Add(this.btnOK);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 375);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(749, 47);
			this.pnlBottom.TabIndex = 1;
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(662, 12);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(579, 12);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.OnOKClick);
			this.AcceptButton = this.btnOK;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(749, 422);
			this.Controls.Add(this.pnlContent);
			this.Controls.Add(this.pnlBottom);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(500, 350);
			this.Name = "SolutionImagePickerForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Images from Referenced Assemblies";
			((System.ComponentModel.ISupportInitialize)(this.pnlContent)).EndInit();
			this.pnlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.previewImage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.images)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).EndInit();
			this.pnlBottom.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			InitControls();
		}
		void InitControls() {
			InitTreeView();
		}
		void InitTreeView() {
			SolutionTreeView.BeginUpdate();
			SolutionTreeView.Nodes.Clear();
			try {
				DoInitTreeView();
				DoExpandDefault();
			}
			finally {
				SolutionTreeView.EndUpdate();
				SolutionTreeView.UpdateStates();
			}
		}
		void DoInitTreeView() {
			foreach(AssemblyEntity entity in this.rootObj.Projects) {
				TreeNode node = new DXSolutionTreeNode(entity.ToString()) { Tag = entity };
				SolutionTreeView.Nodes.Add(node);
				DoLoadChildren(node, entity);
			}
			SolutionTreeView.SelectFirstNode();
		}
		void DoExpandDefault() {
			TreeNodeCollection nodes = SolutionTreeView.Nodes;
			if(nodes.Count == 1) {
				nodes[0].ExpandAll();
			}
			else {
				foreach(TreeNode n in nodes) {
					AssemblyEntity entity = (AssemblyEntity)n.Tag;
					if(entity.Category == AssemblyEntityCategory.Base) n.ExpandAll();
				}
			}
			DoSelectDefaultNode();
		}
		void DoSelectDefaultNode() {
			TreeNodeCollection nodes = SolutionTreeView.Nodes;
			if(nodes.Count > 0) SolutionTreeView.SelectedNode = nodes[0];
		}
		protected virtual void DoLoadChildren(TreeNode node, EntityBase entity) {
			foreach(EntityBase childEntity in entity.GetChildren()) {
				TreeNode childNode = CreateNode(childEntity);
				node.Nodes.Add(childNode);
				DoLoadChildren(childNode, childEntity);
			}
		}
		protected virtual DXSolutionTreeNode CreateNode(EntityBase childEntity) {
			DXSolutionTreeNode node = new DXSolutionTreeNode(childEntity.ToString()) { Tag = childEntity };
			ImageEntity img = childEntity as ImageEntity;
			if(img != null) {
				node.Checked = ImageHoder.IsSelected(img.Path);
				if(node.Checked) {
					img.SetCheck(true);
					img.MarkAsPreChecked();
				}
			}
			return node;
		}
		void DoWarningMsg() {
			this.pnlBottom.Enabled = this.pnlContent.Enabled = false;
		}
		void OnOKClick(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
		}
		DXSolutionTreeView SolutionTreeView { get { return solutionTreeView; } }
		#region TreeList
		void OnImageTreeViewSelectionChanged(object sender, EventArgs e) {
			TreeNode node = ((DXTreeView)sender).SelNode;
			ImageEntity entity = node.Tag as ImageEntity;
			if(entity != null) {
				previewImage.Image = entity.GetImage();
			}
			else previewImage.Image = null;
		}
		void OnImageTreeViewAfterCheck(object sender, TreeViewEventArgs e) {
			DXSolutionTreeView treeView = (DXSolutionTreeView)sender;
			OnNodeCheckedChanged(e.Node);
			DoCheckBranch(treeView, e.Node);
		}
		void OnNodeCheckedChanged(TreeNode node) {
			EntityBase entiry = (EntityBase)node.Tag;
			entiry.SetCheck(node.Checked);
		}
		bool branchUpdating = false;
		void DoCheckBranch(DXSolutionTreeView treeView, TreeNode node) {
			if(branchUpdating) return;
			this.branchUpdating = true;
			try {
				treeView.CheckBranch(node, node.Checked);
			}
			finally {
				this.branchUpdating = false;
			}
		}
		#endregion
		public IList<ImageInfo> GetValues() {
			ImageCollectionVisitor visitor = CreateImageCollectionVisitor();
			this.rootObj.Visit(visitor);
			return visitor.GetImages();
		}
		protected virtual ImageCollectionVisitor CreateImageCollectionVisitor() {
			return new ImageCollectionVisitor();
		}
		public ISolutionImagePickerOwner ImageHoder { get { return imageHolder; } }
	}
	[CLSCompliant(false)]
	public class AddOnlySolutionImagePickerForm : SolutionImagePickerForm {
		public AddOnlySolutionImagePickerForm(ISolutionImagePickerOwner owner, RootObject rootObj)
			: base(owner, rootObj) {
		}
		protected override DXSolutionTreeNode CreateNode(EntityBase childEntity) {
			DXSolutionTreeNode node = base.CreateNode(childEntity);
			if(node.Checked) node.Enabled = false;
			return node;
		}
		protected override ImageCollectionVisitor CreateImageCollectionVisitor() {
			return new AddOnlyImageCollectionVisitor();
		}
	}
	[CLSCompliant(false)]
	public interface ISolutionImagePickerOwner {
		bool IsSelected(string resourceName);
		IServiceProvider ServiceProvider { get; }
		AssemblyEntityCategory GetAssemblyCategory(EnvDTE.Project ownerProject, Reference assembly);
	}
	class DTEUtils {
		public static EnvDTE.Project GetProject(IServiceProvider serviceProvider) {
			EnvDTE.ProjectItem item = serviceProvider.GetService(typeof(EnvDTE.ProjectItem)) as EnvDTE.ProjectItem;
			if(item == null) return null;
			return item.ContainingProject;
		}
		public static IEnumerable<Reference> GetReferences(EnvDTE.Project ownerProject) {
			VSProject vsp = ownerProject.Object as VSProject;
			foreach(Reference reference in vsp.References) {
				yield return reference;
			}
		}
		public static string MakeAssemblyName(Reference reference) {
			if(!reference.StrongName) return reference.Identity;
			return string.Format("{0}, Version={1}, Culture=neutral, PublicKeyToken={2}", reference.Identity, reference.Version, reference.PublicKeyToken);
		}
	}
	public class RootObject {
		IList projects;
		public RootObject(IList projects) {
			this.projects = projects;
		}
		public void Visit(EntityVisitorBase visitor) {
			VisitCore(visitor, Projects);
		}
		protected virtual void VisitCore(EntityVisitorBase visitor, IList elements) {
			foreach(EntityBase entity in elements) {
				entity.Accept(visitor);
				VisitCore(visitor, entity.GetChildren());
			}
		}
		public IList Projects { get { return projects; } }
	}
	public abstract class EntityBase {
		object value;
		bool isChecked;
		public EntityBase(object value) {
			this.isChecked = false;
			this.value = value;
		}
		public bool IsChecked { get { return isChecked; } }
		public void SetCheck(bool isChecked) {
			this.isChecked = isChecked;
		}
		public override string ToString() {
			return GetToString();
		}
		protected object Value { get { return value; } }
		public abstract IList GetChildren();
		protected abstract string GetToString();
		public abstract void Accept(EntityVisitorBase visitor);
	}
	public abstract class CompositeEntryBase : EntityBase {
		Assembly assembly;
		IList<string> resources;
		public CompositeEntryBase(object value, Assembly assembly, IList<string> resources)
			: base(value) {
			this.assembly = assembly;
			this.resources = resources;
		}
		protected IList DoLoadChildren() {
			List<EntityBase> list = new List<EntityBase>();
			foreach(string resource in resources) {
				string basePath = GetBasePath(resource);
				if(!resource.StartsWith(basePath)) continue;
				EntityBase entity = CreateEntry(this.assembly, resource);
				if(list.Contains(entity)) continue;
				list.Add(entity);
			}
			if(list.Count > 1) list.Sort((x, y) => { return string.CompareOrdinal(x.ToString(), y.ToString()); });
			return list;
		}
		protected abstract string GetBasePath(string resource);
		protected virtual EntityBase CreateEntry(Assembly asm, string resource) {
			string basePath = GetBasePath(resource);
			string effectiveId = resource.Substring(basePath.Length);
			string[] parts = effectiveId.Split('.');
			if(parts.Length < 2) {
				throw new InvalidOperationException(string.Format("ImageCollection: Can't create an entity for the '{0}' resource", resource));
			}
			if(parts.Length == 2) return new ImageEntity(asm, resource);
			return new FolderEntity(string.Concat(basePath, parts[0]), asm, this.resources);
		}
	}
	public enum AssemblyEntityCategory { Base, Advanced, Disabled }
	public class AssemblyEntity : CompositeEntryBase {
		AssemblyEntityCategory category;
		public AssemblyEntity(AssemblyEntityCategory category, Assembly asm, IList<string> resources) : base(asm, asm, resources) {
			this.category = category;
		}
		public override void Accept(EntityVisitorBase visitor) {
			visitor.VisitAssembly(this);
		}
		IList children = null;
		public override IList GetChildren() {
			if(this.children == null) {
				this.children = DoLoadChildren();
			}
			return this.children;
		}
		protected void EnsureChildren() {
			if(this.children != null) return;
			var children = GetChildren();
		}
		public bool IsEmpty {
			get {
				EnsureChildren();
				return this.children.Count == 0;
			}
		}
		protected override string GetBasePath(string resource) {
			if(string.IsNullOrEmpty(resource)) return string.Empty;
			string[] parts = resource.Split('.');
			if(parts.Length == 0) return string.Empty;
			return string.Concat(parts[0], ".");
		}
		public AssemblyEntityCategory Category { get { return category; } }
		protected override string GetToString() {
			string text = Assembly.GetName().Name;
			if(IsEmpty) text += " <no images found>";
			return text;
		}
		public Assembly Assembly { get { return base.Value as Assembly; } }
	}
	public class FolderEntity : CompositeEntryBase {
		public FolderEntity(string path, Assembly asm, IList<string> resources)
			: base(path, asm, resources) {
		}
		IList children = null;
		public override IList GetChildren() {
			if(this.children == null) {
				this.children = DoLoadChildren();
			}
			return this.children;
		}
		string folderNameCore = null;
		public string FolderName {
			get {
				if(this.folderNameCore == null) {
					this.folderNameCore = GetFolderName(Path);
				}
				return this.folderNameCore;
			}
		}
		protected string GetFolderName(string path) {
			string[] parts = path.Split('.');
			if(parts.Length < 1) {
				throw new InvalidOperationException(string.Format("ImageCollection: Can't get name from the '{0}' path", path));
			}
			return parts[parts.Length - 1];
		}
		protected override string GetToString() {
			return FolderName;
		}
		public override void Accept(EntityVisitorBase visitor) {
			visitor.VisitFolder(this);
		}
		protected override string GetBasePath(string resource) {
			return string.Concat(Path, ".");
		}
		public override bool Equals(object obj) {
			FolderEntity s = obj as FolderEntity;
			if(s == null) return false;
			return s.Path == Path;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public string Path { get { return base.Value as string; } }
	}
	public class ImageEntity : EntityBase {
		Assembly assembly;
		bool preChecked;
		public ImageEntity(Assembly assembly, string path) : base(path) {
			this.preChecked = false;
			this.assembly = assembly;
		}
		public override IList GetChildren() {
			return new List<EntityBase>() { };
		}
		public void MarkAsPreChecked() {
			this.preChecked = true;
		}
		string imageNameCore = null;
		public string ImageName {
			get {
				if(this.imageNameCore == null) {
					this.imageNameCore = GetImageName(Path);
				}
				return this.imageNameCore;
			}
		}
		public bool IsPreChecked { get { return preChecked; } }
		protected string GetImageName(string path) {
			string[] parts = path.Split('.');
			if(parts.Length < 2) {
				throw new InvalidOperationException(string.Format("ImageCollection: Can't get name from the '{0}' path", path));
			}
			return string.Concat(parts[parts.Length - 2], ".", parts[parts.Length - 1]);
		}
		Image image = null;
		public Image GetImage() {
			if(this.image == null) {
				this.image = ImageCollectionUtils.GetImage(this.assembly, Path);
			}
			return this.image;
		}
		protected override string GetToString() {
			return ImageName;
		}
		public Assembly Assembly { get { return assembly; } }
		public override void Accept(EntityVisitorBase visitor) {
			visitor.VisitImage(this);
		}
		public string Path { get { return base.Value as string; } }
	}
	#region Visitors
	public abstract class EntityVisitorBase {
		public abstract void VisitAssembly(AssemblyEntity entity);
		public abstract void VisitFolder(FolderEntity entity);
		public abstract void VisitImage(ImageEntity entity);
	}
	public class ImageCollectionVisitor : EntityVisitorBase {
		IList<ImageInfo> images;
		public ImageCollectionVisitor() {
			this.images = new List<ImageInfo>();
		}
		public override void VisitAssembly(AssemblyEntity entity) {
		}
		public override void VisitFolder(FolderEntity entity) {
		}
		public override void VisitImage(ImageEntity entity) {
			if(AcceptImage(entity)) this.images.Add(CreateSolutionImageInfo(entity));
		}
		protected virtual bool AcceptImage(ImageEntity entity) {
			return entity.IsChecked;
		}
		protected virtual SolutionImageInfo CreateSolutionImageInfo(ImageEntity entity) {
			return new SolutionImageInfo(entity.ToString(), entity.GetImage(), GetAssemblyName(entity.Assembly), entity.Path);
		}
		protected string GetAssemblyName(Assembly asm) {
			AssemblyName n = asm.GetName();
			byte[] publicKeyToken = n.GetPublicKeyToken();
			return publicKeyToken == null || publicKeyToken.Length == 0 ? n.Name : n.FullName;
		}
		public IList<ImageInfo> GetImages() { return images; }
	}
	public class AddOnlyImageCollectionVisitor : ImageCollectionVisitor {
		protected override bool AcceptImage(ImageEntity entity) {
			if(entity.IsPreChecked) return false;
			return base.AcceptImage(entity);
		}
	}
	#endregion
	static class DataSource {
		public static RootObject Load(ISolutionImagePickerOwner owner) {
			if(owner.ServiceProvider == null) return null;
			RootObject root = new RootObject(GetProjects(owner));
			foreach(EntityBase entity in root.Projects) {
				DrillDown(entity);
			}
			return root;
		}
		static IList GetProjects(ISolutionImagePickerOwner owner) {
			EnvDTE.Project ownerProject = DTEUtils.GetProject(owner.ServiceProvider);
			if(ownerProject == null) {
				throw new InvalidOperationException("ImageCollection: Can't accept current EnvDTE.Project");
			}
			List<EntityBase> list = new List<EntityBase>();
			EnvDTE.Project proj = DTEUtils.GetProject(owner.ServiceProvider);
			foreach(Reference reference in DTEUtils.GetReferences(proj)) {
				AssemblyEntityCategory category = owner.GetAssemblyCategory(ownerProject, reference);
				if(category == AssemblyEntityCategory.Disabled) continue;
				Assembly asm = DoLoadAssembly(reference);
				if(asm != null)
					list.Add(new AssemblyEntity(category, asm, ImageCollectionUtils.GetImageResourceNames(asm)));
			}
			if(list.Count > 1) list.Sort((x, y) => x.ToString().CompareTo(y.ToString()));
			return list;
		}
		static void DrillDown(EntityBase entity) {
			foreach(EntityBase child in entity.GetChildren()) {
				DrillDown(child);
			}
		}
		static Assembly DoLoadAssembly(Reference reference) {
			Assembly asm = null;
			string assemblyName = DTEUtils.MakeAssemblyName(reference);
			try {
				asm = DesignTimeTools.LoadAssembly(assemblyName, true);
			}
			catch(Exception) {
			}
			return asm;
		}
	}
}
