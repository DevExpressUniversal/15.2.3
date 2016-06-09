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
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Designer.Utils;
using DevExpress.XtraNavBar;
using Microsoft.Win32;
namespace DevExpress.Utils.Design {
	[ToolboxItem(false)]
	public class BaseDesignerForm : XtraDesignForm, IModuleNavigationSupports {
		protected PanelControl pnlFrame;
		private System.ComponentModel.IContainer components = null;
		private DevExpress.XtraNavBar.NavBarControl navBarControl; 
		BaseDesigner activeDesigner = null;
		private System.Windows.Forms.Panel pnlMainForm;
		private System.Windows.Forms.Splitter splitter1;
		PropertyStore store = null;
		protected PropertyStore Store { get { return store; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseDesignerForm));
			this.pnlFrame = new DevExpress.XtraEditors.PanelControl();
			this.navBarControl = new DevExpress.XtraNavBar.NavBarControl();
			this.pnlMainForm = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			((System.ComponentModel.ISupportInitialize)(this.pnlFrame)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.navBarControl)).BeginInit();
			this.pnlMainForm.SuspendLayout();
			this.SuspendLayout();
			this.pnlFrame.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlFrame, "pnlFrame");
			this.pnlFrame.Name = "pnlFrame";
			this.navBarControl.ActiveGroup = null;
			resources.ApplyResources(this.navBarControl, "navBarControl");
			this.navBarControl.DragDropFlags = DevExpress.XtraNavBar.NavBarDragDrop.None;
			this.navBarControl.Name = "navBarControl";
			this.navBarControl.LinkSelectionMode = LinkSelectionModeType.OneInGroupAndAllowAutoSelect;
			this.navBarControl.OptionsNavPane.ExpandedWidth = ((int)(resources.GetObject("resource.ExpandedWidth")));
			this.navBarControl.StoreDefaultPaintStyleName = true;
			this.navBarControl.SelectedLinkChanged += OnSelectedLinkChanged;
			this.pnlMainForm.Controls.Add(this.pnlFrame);
			this.pnlMainForm.Controls.Add(this.splitter1);
			this.pnlMainForm.Controls.Add(this.navBarControl);
			resources.ApplyResources(this.pnlMainForm, "pnlMainForm");
			this.pnlMainForm.Name = "pnlMainForm";
			resources.ApplyResources(this.splitter1, "splitter1");
			this.splitter1.Name = "splitter1";
			this.splitter1.TabStop = false;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlMainForm);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.MinimizeBox = false;
			this.Name = "BaseDesignerForm";
			this.ShowInTaskbar = false;
			this.Controls.SetChildIndex(this.pnlMainForm, 0);
			((System.ComponentModel.ISupportInitialize)(this.pnlFrame)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.navBarControl)).EndInit();
			this.pnlMainForm.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		bool? useOfficeStyleCore;
		bool UseOfficeStyle {
			get {
				if(!useOfficeStyleCore.HasValue)
					useOfficeStyleCore = CanUseOfficeStyle();
				return useOfficeStyleCore.Value;
			}
		}
		const string RegistryOfficeStyleOptionPath = "Software\\Developer Express\\Designer\\";
		public virtual bool CanUseOfficeStyle() {
			RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryOfficeStyleOptionPath);
			if(key == null) return false;
			var value = key.GetValue("UseOfficeStyleInControlDesigner");
			if(value != null) {
				key.Close();
				return Boolean.Parse(value.ToString());
			}
			return false;
		}
		protected void CheckNavBarView() {
			if(NavBar == null) return;
			this.SuspendLayout();
			try {
				CheckExpandedGroups(true);
				NavBar.OptionsNavPane.GroupImageShowMode = GroupImageShowMode.InCollapsedState;
				NavBar.ResetStyles();
				NavBar.PaintStyleKind = UseOfficeStyle ? NavBarViewKind.ExplorerBar : NavBarViewKind.NavigationPane;
				NavBar.LinkSelectionMode = UseOfficeStyle ? LinkSelectionModeType.OneInControl : LinkSelectionModeType.OneInGroupAndAllowAutoSelect;
				if(NavBar.OptionsNavPane.NavPaneState == NavPaneState.Expanded) {
					NavBar.Width = UseOfficeStyle ? 200 : 240;
					NavBar.OptionsNavPane.MaxPopupFormWidth = 2 * NavBar.Width;
					if(initializing) {
						bool collapsed = "Collapsed".Equals(string.Format("{0}", Store.RestoreProperty("NavPaneState", string.Empty)));
						if(collapsed) NavBar.OptionsNavPane.NavPaneState = NavPaneState.Collapsed;
					}
				}
				CheckExpandedGroups(false);
			}
			finally {
				this.ResumeLayout();
			}
		}
		DesignerItem activeDesignerItem = null;
		protected DesignerItem ActiveDesignerItem {
			get {
				return activeDesignerItem;
			}
		}
		protected virtual BaseDesigner ActiveDesigner {
			get { return activeDesigner; }
			set {
				if(ActiveDesigner == value) return;
				if(ActiveDesigner != null) ActiveDesigner.Dispose();
				this.activeDesignerItem = null;
				activeDesigner = value;
				if(ActiveDesigner != null) ActiveDesigner.Init();
			}
		}
		protected virtual DevExpress.XtraNavBar.NavBarControl NavBar { get { return navBarControl; } }
		Hashtable expandedGroups = new Hashtable();
		protected virtual void CheckExpandedGroups(bool keep) {
			if(keep) expandedGroups.Clear();
			foreach(NavBarGroup group in NavBar.Groups) {
				if(keep) {
					if(group.Expanded)
						expandedGroups[group.Caption] = true;
				}
				else {
					if(expandedGroups.ContainsKey(group.Caption)) group.Expanded = true;
				}
			}
		}
		public virtual void UpdateActiveDesigner(string caption) {
			NavBarItemLink link = NavBarItemLinkByCaption(caption);
			if(link == null) return;
			link.Group.Expanded = true;
			NavBar.SelectedLink = link;
			InitXF(link, false);
		}
		public void InitNavBar() { InitNavBar(false); }
		public void InitNavBar(bool updateActiveFrame) {
			NavBar.AllowGlyphSkinning = true;
			StoreNavBar();
			NavBarItemLink link = NavBar.SelectedLink;
			string selectedCaption = link != null ? link.Caption : null;
			NavBarItemLink selectedLink = null;
			NavBar.BeginUpdate();
			try {
				CheckNavBarView();
				InitNavBarGroupStyle();
				CheckExpandedGroups(true);
				NavBar.Groups.Clear();
				NavBar.Items.Clear();
				if(ActiveDesigner == null) return;
				for(int i = 0; i < ActiveDesigner.Groups.Count; i++) {
					NavBarGroup group = NavBar.Groups.Add();
					group.Caption = ActiveDesigner.Groups[i].Caption;
					group.SmallImage = ActiveDesigner.Groups[i].Image;
					group.Hint = ActiveDesigner.Groups[i].Description;
					for(int j = 0; j < ActiveDesigner.Groups[i].Count; j++) {
						NavBarItem item = NavBar.Items.Add();
						item.Caption = ActiveDesigner.Groups[i][j].Caption;
						item.SmallImage = ActiveDesigner.Groups[i][j].SmallImage;
						item.LargeImage = ActiveDesigner.Groups[i][j].LargeImage;
						item.Hint = ActiveDesigner.Groups[i][j].Description;
						item.Tag = ActiveDesigner.Groups[i][j];
						NavBarItemLink addedLink = group.ItemLinks.Add(item);
						addedLink.AllowAutoSelect = ActiveDesigner.Groups[i][j].ClickEvent == null;
						if(item.Caption == selectedCaption)
							selectedLink = addedLink;
					}
				}
				CheckExpandedGroups(false);
				RestoreNavBar();
			}
			finally {
				if(selectedLink == null) {
					if(NavBar.Groups.Count > 0 && NavBar.Groups[0].ItemLinks.Count > 0)
						selectedLink = NavBar.Groups[0].ItemLinks[0];
				}
				if(selectedLink != null)
					NavBar.SelectedLink = selectedLink;
				if(updateActiveFrame && EditingComponent != null) {
					InitXFByLink(selectedLink, true);
				}
				NavBar.EndUpdate();
			}
		}
		protected NavBarItemLink NavBarItemLinkByCaption(string caption) {
			foreach(NavBarGroup g in NavBar.Groups)
				foreach(NavBarItemLink l in g.ItemLinks)
					if(l.Item.Caption == caption) {
						return l;
					}
			return null;
		}
		protected NavBarItemLink NavBarItemLinkByDesigner(DesignerItem item) {
			if(NavBar == null) return null;
			foreach(NavBarGroup g in NavBar.Groups)
				foreach(NavBarItemLink l in g.ItemLinks)
					if(l.Item.Tag == item) {
						return l;
					}
			return null;
		}
		#region ShowHints
		private string DescriptionByLink(DevExpress.XtraNavBar.NavBarItemLink link) {
			DesignerItem item = link.Item.Tag as DesignerItem;
			if(item != null) return item.Description;
			return "";
		}
		private string DescriptionByGroup(NavBarGroup group) {
			DesignerGroup dGroup = ActiveDesigner == null ? null : ActiveDesigner.Groups[group.Caption];
			if(dGroup == null) {
				if(group.Hint == "") return group.Caption;
				return group.Hint;
			}
			return dGroup.Description;
		}
		protected virtual string GetDefaultProductInfo() {
			System.Reflection.Assembly assembly = this.GetType().Assembly;
			string s = assembly.ToString();
			return s.Substring(0, s.IndexOf(", Culture"));
		}
		#endregion
		bool initialized = false;
		protected bool Initialized { get { return initialized; } }
		bool initializing = false;
		object editingComponent = null;
		public BaseDesignerForm()
			: this(null) {
		}
		protected BaseDesignerForm(DevExpress.LookAndFeel.UserLookAndFeel parentLookAndFeel) {
			if(parentLookAndFeel != null)
				this.LookAndFeel.ParentLookAndFeel = parentLookAndFeel;
			InitializeComponent();
			WindowsFormsDesignTimeSettings.ApplyDesignSettings(navBarControl.Appearance);
			this.AutoScaleMode = AutoScaleMode.None;
			this.store = new PropertyStore(RegistryStorePath);
			Store.Restore();
		}
		public virtual void Initialize() {
			if(Initialized) return;
			initializing = true;
			try {
				SuspendLayout();
				this.initialized = true;
				CreateDesigner();
				InitNavBar();
				ResumeLayout();
			} finally {
				initializing = false;
			}
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			RestoreProperties();
		}
		protected void HideNavBar() {
			navBarControl.Visible = false;
			splitter1.Visible = false;
		}
		public object EditingComponent { get { return editingComponent; } }
		public virtual void InitEditingObject(object editingObject) { InitEditingObject(editingObject, ""); }
		public virtual void InitEditingObject(object editingObject, string designerName) {
			this.editingComponent = editingObject;
			IServiceProvider host = null;
			ILookAndFeelService serv = null;
			if(editingObject is IComponent)
				host = ((IComponent)editingObject).Site;
			if(host != null)
				serv = host.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
			if(serv != null && !CanUseDefaultControlDesignersSkin())
				serv.InitializeRootLookAndFeel(this.LookAndFeel);
			else
				LookAndFeel.SetSkinStyle("DevExpress Design");
			Initialize();
			XF = null;
			if(ActiveDesigner == null) return;
			InitNavBar();
			DesignerItem item = designerName == "" ? ActiveDesigner.DefaultItem : ActiveDesigner.ItemByName(designerName);
			if(item == null) item = ActiveDesigner.DefaultItem;
			if(item == null) return;
			InitXF(item);
		}
		protected virtual bool CanUseDefaultControlDesignersSkin() {
			return RegistryDesignerSkinHelper.CanUseDefaultControlDesignersSkin;
		}
		XtraFrame xf = null;
		public virtual XtraFrame XF {
			get { return xf; }
			set {
				if(XF == value) return;
				if(XF != null) {
					XF.RefreshWizard -= new RefreshWizardEventHandler(OnXF_RefreshWizard);
					if(Store != null)
						XF.StoreProperties(Store);
					XF.Dispose();
				}
				xf = value;
				if(XF != null)
					XF.RefreshWizard += new RefreshWizardEventHandler(OnXF_RefreshWizard);
			}
		}
		protected void InitXF(NavBarItemLink link, bool byNavBar) {
			DesignerItem item = link.Item.Tag as DesignerItem;
			if(item == null) return;
			if(byNavBar) {
				if(item.ClickEvent != null) return;
			}
			InitXF(item);
		}
		protected void InitXF(DesignerItem item) {
			InitXFCore(item, false);
		}
		protected void InitXFByLink(NavBarItemLink link, bool force) {
			DesignerItem item = link.Item.Tag as DesignerItem;
			if(item != null) InitXFCore(item, force);
		}
		protected void InitXFCore(DesignerItem item, bool force) {
			this.activeDesignerItem = item;
			if(item == null) {
				XF = null;
				return;
			}
			if(!force && XF != null && XF.DesignerItem == item) return;
			this.SuspendLayout();
			pnlFrame.SuspendLayout();
			Type type = item.FrameType;
			string classname = item.FrameTypeName;
			if(classname != "" && type == null) {
				type = Type.GetType(classname);
				if(type == null) {
					type = ResolveType(classname);
				}
			}
			if(type != null) {
				Cursor savedCursor = this.Cursor;
				try {
					this.Cursor = Cursors.WaitCursor;
					this.activeDesignerItem = item;
					XF = Activator.CreateInstance(type) as XtraFrame;
					XF.SuspendLayout();
					WindowsFormsDesignTimeSettings.ApplyDesignSettings(XF);
					XF.Bounds = pnlFrame.DisplayRectangle;
					XF.Dock = DockStyle.Fill;
					XF.DesignerItem = item;
					Bitmap bmp = item.SmallImage == null ? null : (Bitmap)item.SmallImage;
					if(Store != null) XF.RestoreProperties(Store);
					InitFrame(item.Caption, bmp);
					ControlContainerLookAndFeelHelper.UpdateControlChildrenLookAndFeel(XF, LookAndFeel);
				}
				finally {
					XF.ResumeLayout();
					XF.EndInitialize();
					this.Cursor = savedCursor;
				}
				this.pnlFrame.Controls.Add((Control)XF);
			}
			NavBarItemLink link = NavBarItemLinkByDesigner(item);
			if(link != null) {
				link.Group.Expanded = true;
				NavBar.SelectedLink = link;
			}
			pnlFrame.ResumeLayout();
			this.ResumeLayout(true);
		}
		protected virtual Type ResolveType(string type) {
			IComponent component = EditingComponent as IComponent;
			if(component != null && component.Site != null) {
				ITypeResolutionService srv = component.Site.GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
				if(srv != null) return srv.GetType(type, false, false);
			}
			return null;
		}
		protected override void OnClosed(EventArgs e) {
			XF = null;
			if(Store != null) {
				StoreProperties();
				Store.Store();
			}
			base.OnClosed(e);
		}
		protected virtual void InitNavBarGroupStyle() {
			NavBar.Appearance.GroupBackground.BackColor = Color.FromArgb(100, SystemColors.Control);
		}
		void OnSelectedLinkChanged(object sender, XtraNavBar.ViewInfo.NavBarSelectedLinkChangedEventArgs e) {
			if(EditingComponent != null && e.Link != null) {
				InitModuleCore(e.Link);
				DesignerItem item = e.Link.Item.Tag as DesignerItem;
				if(item == null) return;
				if(item.ClickEvent != null) {
					item.ClickEvent(this, EventArgs.Empty);
					if(ActiveDesignerItem != null) {
						NavBarItemLink link = NavBarItemLinkByDesigner(ActiveDesignerItem);
						if(NavBar != null) {
							link.Group.Expanded = true;
							NavBar.SelectedLink = link;
						}
					}
					return;
				}
			}
		}
		protected void InitModuleCore(NavBarItemLink link) {
			InitXF(link, true);
		}
		#region IModuleNavigationSupports
		bool IModuleNavigationSupports.AllowNavigation { get { return AllowModuleNavigationCore; } }
		void IModuleNavigationSupports.NavigateTo(Type moduleType) {
			if(!AllowModuleNavigationCore)
				return;
			if(NavBar == null || moduleType == null)
				return;
			foreach(NavBarItemLink link in NavBar.GetAllLinks()) {
				DesignerItem designerItem = link.Item.Tag as DesignerItem;
				if(designerItem != null && (designerItem.FrameType == moduleType || designerItem.FrameTypeName == moduleType.FullName)) {
					InitModuleCore(link);
					return;
				}
			}
		}
		#endregion
		protected virtual bool AllowModuleNavigationCore { get { return true; } }
		protected virtual void RestoreProperties() {
			if(Store == null) return;
			Store.RestoreForm(this);
			CanUseOfficeStyle();
		}
		protected virtual void StoreProperties() {
			if(Store == null) return;
			Store.AddProperty("ActiveItem", ActiveDesignerItem != null ? ActiveDesignerItem.Caption : "");
			Store.AddProperty("NavPaneState", NavBar.OptionsNavPane.ActualNavPaneState);
			StoreNavBar();
			Store.AddForm(this);
		}
		protected virtual void RestoreNavBar() {
			if(NavBar == null || Store == null) return;
			PropertyStore ps = Store.RestoreProperty("$NavBar", null) as PropertyStore;
			if(ps == null) return;
			ps.Restore();
			NavBar.BeginUpdate();
			try {
				foreach(NavBarGroup group in NavBar.Groups) {
					group.Expanded = ps.RestoreBoolProperty(group.Caption, group.Expanded);
				}
			}
			finally {
				NavBar.EndUpdate();
			}
		}
		protected virtual void StoreNavBar() {
			if(NavBar == null || Store == null) return;
			PropertyStore ps = new PropertyStore(Store, "$NavBar");
			foreach(NavBarGroup group in NavBar.Groups) {
				ps.AddProperty(group.Caption, group.Expanded);
			}
			ps.Store();
			Store.AddProperty("$NavBar", ps);
		}
		protected virtual void InitNavBarImages() {
			NavBar.LargeImages = GetImageListByResourceName("MyRes.bmp");
		}
		protected virtual void InitFrame(string caption, Bitmap bitmap) {
			XF.InitFrame(EditingComponent, caption, bitmap);
		}
		protected virtual void OnXF_RefreshWizard(object sender, RefreshWizardEventArgs e) {
		}
		protected virtual ImageList GetImageListByResourceName(string resourceName) { return null; }
		protected virtual string RegistryStorePath { get { return "Software\\Developer Express\\Designer\\"; } }
		protected virtual void CreateDesigner() {
			ActiveDesigner = new TestDesigner();
		}
	}
	public interface IModuleNavigationSupports {
		bool AllowNavigation { get; }
		void NavigateTo(Type moduleType);
	}
	public class TestDesigner : BaseDesigner {
		protected override void CreateGroups() {
			Groups.Clear();
			DesignerGroup group = Groups.Add("Group1", "CoolGroup", null, true);
			group.Add("DoSome", "Do some item :)", typeof(XtraPGFrame), null, null, null);
			group.Add("DoSome2", "Do some item2 :)", typeof(XtraPGFrame), null, null, null);
			group = Groups.Add("Group2", "MegaCoolGroup", null, true);
			group.Add("DoSome21", "Do some item21 :)", typeof(XtraFrame), null, null, null);
			group.Add("DoSome22", "Do some item22 :)", typeof(XtraPGFrame), null, null, null);
		}
	}
}
