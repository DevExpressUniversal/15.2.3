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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.Utils.Design {
	public partial class DXImagePickerForm : XtraForm, ITopFormOptionsProvider {
		DXImagePickerFormOptions options;
		static DXImagePickerForm() {
			SkinManager.EnableFormSkins();
		}
		public DXImagePickerForm()
			: this(true, false, null) {
			WindowsFormsDesignTimeSettings.ApplyDesignSettings(this);
		}
		public DXImagePickerForm(bool useDefaultPicker, bool allowGalleryMultiSelect, Size? desiredImageSize) {
			this.options = CreateOptions(useDefaultPicker, allowGalleryMultiSelect, desiredImageSize);
			InitializeComponent();
			this.dxImageGalleryControl = CreateImageGalleryControl();
			if(!IsAsyncSupported) LoadImageGallery();
		}
		DXImagePickerFormOptions CreateOptions(bool useDefaultPicker, bool allowGalleryMultiSelect, Size? desiredImageSize) {
			return new DXImagePickerFormOptions(useDefaultPicker, allowGalleryMultiSelect, desiredImageSize);
		}
		IServiceProvider serviceProvider;
		IDefaultResourcePickerServiceProvider defaultResourcePickerSvcProvider;
		public void InitServices(IServiceProvider serviceProvider, IDefaultResourcePickerServiceProvider defaultResourcePickerSvcProvider) {
			this.serviceProvider = serviceProvider;
			this.defaultResourcePickerSvcProvider = defaultResourcePickerSvcProvider;
			ImageGalleryControl.InitServices(serviceProvider, defaultResourcePickerSvcProvider, this);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			InitializeControls();
			MinimumSize = Size;
		}
		void InitializeControls() {
			if(ServiceProvider == null)
				return;
			DefaultResourcePickerSvcProvider.AddToContainer(DefaultPage);
			TabControl.SelectedTabPage = (Options.ImageSource == DXImageGalleryImageSource.Default ? DefaultPage : GalleryPage);
			if(!Options.UseDefaultPicker) {
				DefaultPage.PageVisible = false;
				TabControl.SelectedTabPage = GalleryPage;
			}
			rgResourceTypeSelector.Visible = Options.UseDefaultPicker;
			rgResourceTypeSelector.EditValue = (int)Options.ResourceType;
		}
		protected void ShowResourceTypeSelector() {
			this.rgResourceTypeSelector.Visible = true;
		}
		protected void HideResourceTypeSelector() {
			rgResourceTypeSelector.Visible = false;
		}
		protected void EnableOkButton() {
			btnOk.Enabled = true;
		}
		protected void DisableOkButton() {
			btnOk.Enabled = false;
		}
		protected virtual void LoadImageGallery() {
			DXImageGalleryStorage.Default.LoadAsync();
		}
		EnvDTE.Project Project {
			get {
				if(ServiceProvider == null)
					return null;
				EnvDTE.ProjectItem item = ServiceProvider.GetService(typeof(EnvDTE.ProjectItem)) as EnvDTE.ProjectItem;
				return item != null ? item.ContainingProject : null;
			}
		}
		IServiceProvider ServiceProvider {
			get { return serviceProvider; }
		}
		void OnTabControlSelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			OnSelectedPageChanged(e.Page);
		}
		protected virtual void OnSelectedPageChanged(XtraTabPage page) {
			Options.ImageSource = (page == DefaultPage ? DXImageGalleryImageSource.Default : DXImageGalleryImageSource.Gallery);
		}
		void OnOkClick(object sender, EventArgs e) {
			PrepareExit();
			DialogResult = DialogResult.OK;
		}
		void PrepareExit() {
			DefaultResourcePickerSvcProvider.EmulateOkClick();
		}
		void OnImageGalleryControlSelectedItemChanged(object sender, DXImageGalleryItem item) {
			Options.SelectedItem = item;
		}
		void OnImageGalleryControlResourceTypeSelectorChanged(object sender, DXImageGalleryResourceType resourceType) {
			Options.ResourceType = resourceType;
		}
		void OnResourceTypeSelectedIndexChanged(object sender, EventArgs e) {
			RadioGroup radioGroup = sender as RadioGroup;
			DXImageGalleryResourceType resourceType = (DXImageGalleryResourceType)radioGroup.EditValue;
			Options.ResourceType = resourceType;
		}
		public object EditValue {
			get {
				if(TabControl.SelectedTabPage == DefaultPage) {
					return DefaultResourcePickerSvcProvider.EditValue;
				}
				return ImageGalleryControl.EditValue;
			}
		}
		public IEnumerable<DXImageGalleryItem> GetGalleryValues() {
			return ImageGalleryControl.GetValues();
		}
		protected virtual bool IsAsyncSupported { get { return false; } }
		#region ITopFormOptionsProvider
		DXImageGalleryResourceType ITopFormOptionsProvider.ResourceType {
			get { return Options.ResourceType; }
		}
		bool ITopFormOptionsProvider.AllowMultiSelect {
			get { return Options.AllowGalleryMultiSelect; }
		}
		Size? ITopFormOptionsProvider.DesiredImageSize {
			get { return Options.DesiredImageSize; }
		}
		bool ITopFormOptionsProvider.IsAsync {
			get { return IsAsyncSupported; }
		}
		#endregion
		protected XtraTabPage DefaultPage { get { return tabDefault; } }
		protected XtraTabPage GalleryPage { get { return tabGallery; } }
		protected XtraTabControl TabControl { get { return xtraTabControl1; } }
		protected internal DXImageGalleryControl dxImageGalleryControl;
		protected DXImageGalleryControl CreateImageGalleryControl() {
			DXImageGalleryControl control = CreateImageGalleryControlCore();
			this.tabGallery.Controls.Add(control);
			this.tabGallery.Controls.Add(this.panelControl1);
			control.Dock = DockStyle.Fill;
			control.Location = new Point(12, 2);
			control.Margin = new Padding(0);
			control.Name = "dxImageGalleryControl";
			control.Size = new Size(612, 370);
			control.TabIndex = 0;
			control.SelectedItemChanged += OnImageGalleryControlSelectedItemChanged;
			return control;
		}
		protected virtual DXImageGalleryControl CreateImageGalleryControlCore() {
			return new DXImageGalleryControl();
		}
		protected internal DXImageGalleryControl ImageGalleryControl { get { return dxImageGalleryControl; } }
		IDefaultResourcePickerServiceProvider DefaultResourcePickerSvcProvider { get { return defaultResourcePickerSvcProvider; } }
		public DXImagePickerFormOptions Options { get { return options; } }
		#region Disposing
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#endregion
	}
	public class DXAsyncImagePickerForm : DXImagePickerForm {
		WaitPanelControl waitPanel;
		public DXAsyncImagePickerForm() {
			CreateWaitPanel();
		}
		public DXAsyncImagePickerForm(bool useDefaultPicker, bool allowMultiSelect, Size? desiredImageSize) : base(useDefaultPicker, allowMultiSelect, desiredImageSize) {
			CreateWaitPanel();
		}
		protected void CreateWaitPanel() {
			this.waitPanel = CreateWaitPanelInstance();
		}
		protected virtual WaitPanelControl CreateWaitPanelInstance() {
			return new WaitPanelControl();
		}
		protected bool IsWaitPanelVisible { get { return WaitPanel != null && WaitPanel.Parent != null; } }
		public void ShowWaitPanel(XtraTabPage page) {
			HideResourceTypeSelector();
			WaitPanel.Parent = page;
			WaitPanel.Dock = DockStyle.Fill;
			WaitPanel.BringToFront();
			DisableOkButton();
		}
		protected override void OnSelectedPageChanged(XtraTabPage page) {
			base.OnSelectedPageChanged(page);
			if(page == GalleryPage) {
				OnGalleryPageSelected();
			}
			else {
				OnGalleryPageClosed();
			}
		}
		protected virtual void OnGalleryPageSelected() {
			if(dataLoaded) return;
			if(IsWaitPanelVisible) {
				DisableOkButton();
				return;
			}
			ShowWaitPanel(GalleryPage);
		}
		protected virtual void OnGalleryPageClosed() {
			EnableOkButton();
		}
		bool dataLoaded = false;
		public void OnDataLoaded() {
			this.dataLoaded = true;
			ImageGalleryControl.DoLoad();
			ShowResourceTypeSelector();
			EnableOkButton();
			DisposeWaitPanel();
		}
		protected void DisposeWaitPanel() {
			if(WaitPanel != null) WaitPanel.Dispose();
			this.waitPanel = null;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DisposeWaitPanel();
			}
			base.Dispose(disposing);
		}
		protected WaitPanelControl WaitPanel { get { return waitPanel; } }
		protected override bool IsAsyncSupported { get { return true; } }
		#region Wait Indicator
		protected class WaitPanelControl : PictureEdit {
			public WaitPanelControl() {
				BackColor = Color.Transparent;
				BorderStyle = BorderStyles.NoBorder;
				Properties.AllowFocused = Properties.ShowMenu = false;
				LookAndFeel.SkinName = "Office 2010 Silver";
				LookAndFeel.UseDefaultLookAndFeel = false;
				Image = CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinLoadingBig].Image.Image;
			}
		}
		#endregion
	}
	public class DXImagePickerFormOptions {
		public DXImagePickerFormOptions(bool useDefaultPicker, bool allowGalleryMultiselect, Size? desiredImageSize) {
			this.UseDefaultPicker = useDefaultPicker;
			this.AllowGalleryMultiSelect = allowGalleryMultiselect;
			this.DesiredImageSize = desiredImageSize;
			this.ImageSource = DXImageGalleryImageSource.Default;
			this.SelectedItem = null;
			this.ResourceType = DXImageGalleryResourceType.Form;
		}
		public bool UseDefaultPicker { get; set; }
		public bool AllowGalleryMultiSelect { get; set; }
		public Size? DesiredImageSize { get; set; }
		public DXImageGalleryImageSource ImageSource { get; set; }
		public DXImageGalleryItem SelectedItem { get; set; }
		public DXImageGalleryResourceType ResourceType { get; set; }
	}
}
