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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Helpers;
using System.Collections.Generic;
using DevExpress.Utils.Design;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors;
using System.Collections;
namespace DevExpress.XtraBars.Design {
	public class BarDesignTimeUtils {
		public static string GetBarManagerRequiredWarningMessage(IComponent component) {
			string controlName = component.Site.Name;
			return string.Format("The {0} cannot be customized when it's not bound either to a BarManager or RibbonControl. Do you want the BarManager to be automatically created?\r\n\r\nClick No to cancel this dialog. Then you can manually drop either a BarManager or RibbonControl onto the form, and bind the {0} to one of them via either the Manager or Ribbon property.\r\n\r\nClick Yes to automatically create a BarManager and bind the {0} to it.", controlName);
		}
		public static string GetRibbonRequiredWarningMessage(IComponent component) {
			string controlName = component.Site.Name;
			return string.Format("The {0} cannot be customized when it's not bound to a RibbonControl. Do you want the RibbonControl to be automatically created?\r\n\r\nClick No to cancel this dialog. Then you can manually drop a RibbonControl onto the form, and bind the {0} to it via the Ribbon property.\r\n\r\nClick Yes to automatically create a RibbonControl and bind the {0} to it.", controlName);
		}
		public static BarManager CreateDefaultBarManager(IComponent component) {
			IDesignerHost host = GetDesignerHost(component);
			if(host == null)
				return null;
			Control topControl = GetTopControl(component);
			if(topControl == null)
				return null;
			BarManager manager = host.CreateComponent(typeof(BarManager)) as BarManager;
			topControl.Container.Add(manager);
			manager.Form = topControl;
			FireChanged(manager);
			return manager;
		}
		public static RibbonControl CreateDefaultRibbonControl(IComponent component, bool checkTopControlSize) {
			IDesignerHost host = GetDesignerHost(component);
			if(host == null)
				return null;
			Control topControl = GetTopControl(component);
			if(topControl == null)
				return null;
			RibbonControl ribbon = host.CreateComponent(typeof(RibbonControl)) as RibbonControl;
			topControl.Controls.Add(ribbon);
			topControl.Site.Container.Add(ribbon);
			RibbonDesignTimeManager designTimeManager = ribbon.GetDesignTimeManager() as RibbonDesignTimeManager;
			if(designTimeManager != null) {
				designTimeManager.OnAddPage(ribbon, new RibbonDefaultRegistrationInfo(ribbon));
				designTimeManager.OnAddPageGroup(ribbon, new RibbonDefaultRegistrationInfo(ribbon));
				designTimeManager.OnAddStatusBar(ribbon, EventArgs.Empty);
			}
			FireChanged(ribbon);
			if(checkTopControlSize) {
				if(topControl.Width < TopControlMinWidth) {
					topControl.Width = TopControlMinWidth;
				}
				if(topControl.Height < TopControlMinHeight) {
					topControl.Height = TopControlMinHeight;
				}
				FireChanged(topControl);	
			}
			return ribbon;
		}
		public static readonly int TopControlMinWidth = 350;
		public static readonly int TopControlMinHeight = 300;
		public static bool IsReferenceExists<T>(ISite site) where T : class {
			IReferenceService svc = site.GetService(typeof(IReferenceService)) as IReferenceService;
			if(svc == null) {
				throw new InvalidOperationException("Can't get IReferenceService service");
			}
			object[] objects = null;
			try {
				objects = svc.GetReferences(typeof(T));
			}
			catch { }
			return objects != null && objects.Length > 0;
		}
		public static void FireChanged(IComponent component) {
			IComponentChangeService srv = component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(component, null, null, null);
		}
		public static void Save(IComponent component) {
			EnvDTE.ProjectItem item = component.Site.GetService(typeof(EnvDTE.ProjectItem)) as EnvDTE.ProjectItem;
			if(item == null) return;
			try {
				item.Save();
			}
			catch {
			}
		}
		static IDesignerHost GetDesignerHost(IComponent component) {
			if(component.Site == null)
				return null;
			return component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
		}
		static Control GetTopControl(IComponent component) {
			IDesignerHost designerHost = GetDesignerHost(component);
			return designerHost != null ? designerHost.RootComponent as Control : null;
		}
	}
	public interface IKeyCommandProcessInfo {
		IServiceProvider ServiceProvider { get; }
		BaseDesignTimeManager DesignTimeManager { get; }
		IComponent Component { get; }
	}
	public class DesignTimeKeyCommandProcessHelperBase : IDisposable {
		IKeyCommandProcessInfo info;
		public DesignTimeKeyCommandProcessHelperBase(IKeyCommandProcessInfo info) {
			this.info = info;
			if(DesignerHost != null) menuCommandService = MenuCommandServiceHelper.RegisterMenuCommandService(DesignerHost);
			InitializeMenuCommands();
			if(NeedHandleDelete) {
				this.pStore = new PropertyStore(BarEditorForm.BarSettings);
				pStore.Restore();
			}
		}
		PropertyStore pStore;
		MenuCommand oldCancelHandler;
		MenuCommand oldDeleteHandler;
		MenuCommand oldCutHandler;
		public IKeyCommandProcessInfo Info { get { return info; } }
		public virtual bool NeedHandleDelete { get { return true; } }
		protected void InitializeMenuCommands() {
			if(oldDeleteHandler != null) return;
			oldDeleteHandler = InitializeMenuCommand(new EventHandler(OnKeyDel), StandardCommands.Delete);
			oldCutHandler = InitializeMenuCommand(new EventHandler(OnKeyCut), StandardCommands.Cut);
			oldCancelHandler = InitializeMenuCommand(new EventHandler(OnKeyCancel), MenuCommands.KeyCancel);
		}
		protected void ClearMenuCommands() {
			RestoreMenuCommand(null, StandardCommands.Delete);
			RestoreMenuCommand(null, MenuCommands.KeyCancel);
			RestoreMenuCommand(null, MenuCommands.Cut);
		}
		protected void RestoreMenuCommands() {
			RestoreMenuCommand(oldDeleteHandler, StandardCommands.Delete);
			RestoreMenuCommand(oldCancelHandler, MenuCommands.KeyCancel);
			RestoreMenuCommand(oldCutHandler, MenuCommands.Cut);
			System.Diagnostics.Debug.WriteLine("MenuCommands were restored");
		}
		protected void RestoreMenuCommand(MenuCommand command, CommandID id) {
			MenuCommand mc_old = MenuCommandService.FindCommand(id);
			if(mc_old != null)
				MenuCommandService.RemoveCommand(mc_old);
			if(MenuCommandService.FindCommand(id) != null) return;
			if(command != null)
				MenuCommandService.AddCommand(command);
		}
		IMenuCommandService menuCommandService;
		protected IMenuCommandService MenuCommandService {
			get {
				if(menuCommandService == null) {
					menuCommandService = (IMenuCommandService)Info.ServiceProvider.GetService(typeof(IMenuCommandService));
				}
				return menuCommandService;
			}
		}
		IDesignerHost designerHost = null;
		protected IDesignerHost DesignerHost {
			get {
				if(designerHost == null) {
					designerHost = Info.ServiceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				}
				return designerHost;
			}
		}
		ISelectionService selectionService = null;
		protected ISelectionService SelectionService {
			get {
				if(selectionService == null) {
					try {
						selectionService = Info.ServiceProvider.GetService(typeof(ISelectionService)) as ISelectionService;
					}
					catch { return null; }
				}
				return selectionService;
			}
		}
		protected virtual ArrayList SelectedComponents {
			get {
				return new ArrayList(SelectionService.GetSelectedComponents());
			}
		}
		protected MenuCommand InitializeMenuCommand(EventHandler handler, CommandID id) {
			MenuCommand mc_old = MenuCommandService.FindCommand(id);
			if(mc_old != null) {
				MenuCommandService.RemoveCommand(mc_old);
				if(MenuCommandService.FindCommand(id) != null) return null;
				MenuCommand mc_new = new MenuCommand(handler, id);
				mc_new.Enabled = true;
				mc_new.Visible = true;
				mc_new.Supported = true;
				MenuCommandService.AddCommand(mc_new);
			}
			return mc_old;
		}
		protected void PassControlToOldKeyCancelHandler() {
			oldCancelHandler.Invoke();
		}
		protected void PassControlToOldKeyDelHandler() {
			oldDeleteHandler.Invoke();
		}
		protected void PassControlToOldKeyCutHandler() {
			oldCutHandler.Invoke();
		}
		public virtual void OnKeyCancel(object sender, EventArgs e) {
			PassControlToOldKeyCancelHandler();
		}
		public virtual void OnKeyDel(object sender, EventArgs e) {
			if(NeedHandleDelete) {
				object[] selectedObjects = SelectedComponents.ToArray();
				if(selectedObjects.Length > 0 && selectedObjects[0] != null) {
					try {
						foreach(IDisposable disposable in selectedObjects) {
							SelectionService.SetSelectedComponents(
									new object[] { disposable }, SelectionTypes.Replace);
							BarItemHandleDelete(disposable);
						}
					}
					catch { }
				}
			}
			else {
				RestoreMenuCommands();
				PassControlToOldKeyDelHandler();
			}
		}
		#region BarItemHandleDelete
		public virtual void BarItemHandleDelete(IDisposable disposable) {
			if(SelectionService == null) {
				RestoreMenuCommands();
				PassControlToOldKeyDelHandler();
				return;
			}
			if(disposable is BarItem){
				BarItem item = disposable as BarItem;
				if(SelectionService.PrimarySelection is BarItem) {
					if((SelectionService.PrimarySelection as BarItem) == item) {
						pStore.Restore();
						if(pStore.RestoreBoolProperty("ShowBarElementDeletionConfirmation", true)) {
							QuestionsForm frm = new QuestionsForm();
							frm.ShowDialog();
							if(frm.DialogResult == DialogResult.Cancel) return;
							item.Refresh();
						}
						DelByRegistry(item);
						return;
					}
				}
				else return;
			}
			oldDeleteHandler.Invoke();
		}
		enum DeleteObjectType { ItemWithLinks, Link }
		void DelByRegistry(BarItem item) {
			pStore.Restore();
			if((DeleteObjectType)pStore.RestoreProperty("DeletedBarElementType", typeof(DeleteObjectType), DeleteObjectType.ItemWithLinks) == DeleteObjectType.ItemWithLinks) {
				IComponent disposedComponent = item as IComponent;
				if(disposedComponent != null)
					DesignerHost.DestroyComponent(disposedComponent);
				else item.Dispose();
			}
			else DelLink(item);
		}
		void DelLink(BarItem item) {
			item.GetBarLinkInfoProvider().Link.Dispose();
		}
		public enum ResultQuestionFormSelectionType { Links, All, Cancel, LinksRemember, AllRemember }
		public class QuestionsForm : XtraForm {
			public QuestionsForm()
				: base() {
				InitializeComponent();
				Result = ResultQuestionFormSelectionType.Cancel;
				NeedSave = false;
				this.pStore = new PropertyStore(BarEditorForm.BarSettings);
				pStore.Restore();
			}
			PropertyStore pStore;
			private void simpleButton2_Click(object sender, EventArgs e) {
				if(checkEdit3.Checked) {
					NeedSave = true;
					if(checkEdit1.Checked) pStore.AddProperty("DeletedBarElementType", DeleteObjectType.Link);
					if(checkEdit2.Checked) pStore.AddProperty("DeletedBarElementType", DeleteObjectType.ItemWithLinks);
					pStore.AddProperty("ShowBarElementDeletionConfirmation", false);
					pStore.Store();
				}
				else {
					if(checkEdit1.Checked) pStore.AddProperty("DeletedBarElementType", DeleteObjectType.Link);
					if(checkEdit2.Checked) pStore.AddProperty("DeletedBarElementType", DeleteObjectType.ItemWithLinks);
					pStore.Store();
				}
			}
			public bool NeedSave { get; set; }
			public ResultQuestionFormSelectionType Result { get; set; }
			#region Windows Form Designer generated code
			private void InitializeComponent() {
				this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
				this.checkEdit3 = new DevExpress.XtraEditors.CheckEdit();
				this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
				this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
				this.checkEdit2 = new DevExpress.XtraEditors.CheckEdit();
				this.checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
				this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
				this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
				this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
				this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
				this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
				this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
				this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
				this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
				this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
				this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
				this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
				this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
				((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
				this.layoutControl1.SuspendLayout();
				((System.ComponentModel.ISupportInitialize)(this.checkEdit3.Properties)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.checkEdit2.Properties)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
				this.SuspendLayout();
				this.layoutControl1.Controls.Add(this.checkEdit3);
				this.layoutControl1.Controls.Add(this.simpleButton2);
				this.layoutControl1.Controls.Add(this.simpleButton1);
				this.layoutControl1.Controls.Add(this.checkEdit2);
				this.layoutControl1.Controls.Add(this.checkEdit1);
				this.layoutControl1.Controls.Add(this.labelControl1);
				this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
				this.layoutControl1.Location = new System.Drawing.Point(0, 0);
				this.layoutControl1.Name = "layoutControl1";
				this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(486, 67, 450, 350);
				this.layoutControl1.OptionsView.UseParentAutoScaleFactor = true;
				this.layoutControl1.Root = this.layoutControlGroup1;
				this.layoutControl1.Size = new System.Drawing.Size(756, 308);
				this.layoutControl1.Text = "layoutControl1";
				this.checkEdit3.AutoSizeInLayoutControl = true;
				this.checkEdit3.Location = new System.Drawing.Point(15, 268);
				this.checkEdit3.Name = "checkEdit3";
				this.checkEdit3.Properties.Caption = "Remember this selection and do not ask me next time";
				this.checkEdit3.Size = new System.Drawing.Size(431, 23);
				this.checkEdit3.StyleController = this.layoutControl1;
				this.checkEdit3.TabIndex = 2;
				this.simpleButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
				this.simpleButton2.Location = new System.Drawing.Point(490, 268);
				this.simpleButton2.Name = "simpleButton2";
				this.simpleButton2.Size = new System.Drawing.Size(120, 26);
				this.simpleButton2.StyleController = this.layoutControl1;
				this.simpleButton2.LookAndFeel.UseDefaultLookAndFeel = false;
				this.simpleButton2.LookAndFeel.UseWindowsXPTheme = true;
				this.simpleButton2.TabIndex = 3;
				this.simpleButton2.Text = "OK";
				this.simpleButton2.Click +=simpleButton2_Click;
				this.simpleButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
				this.simpleButton1.Location = new System.Drawing.Point(622, 268);
				this.simpleButton1.Name = "simpleButton1";
				this.simpleButton1.Size = new System.Drawing.Size(119, 26);
				this.simpleButton1.StyleController = this.layoutControl1;
				this.simpleButton1.LookAndFeel.UseDefaultLookAndFeel = false;
				this.simpleButton1.LookAndFeel.UseWindowsXPTheme = true;
				this.simpleButton1.TabIndex = 4;
				this.simpleButton1.Text = "Cancel";
				this.checkEdit2.AutoSizeInLayoutControl = true;
				this.checkEdit2.Location = new System.Drawing.Point(44, 101);
				this.checkEdit2.Name = "checkEdit2";
				this.checkEdit2.Properties.Caption = "Delete the source BarItem along with all of its links";
				this.checkEdit2.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
				this.checkEdit2.Size = new System.Drawing.Size(668, 23);
				this.checkEdit2.StyleController = this.layoutControl1;
				this.checkEdit2.TabIndex = 1;
				this.checkEdit2.CheckedChanged += new System.EventHandler(this.checkEdit2_CheckedChanged);
				this.checkEdit1.AutoSizeInLayoutControl = true;
				this.checkEdit1.Checked = true;
				this.checkEdit1.Location = new System.Drawing.Point(44, 74);
				this.checkEdit1.Name = "checkEdit1";
				this.checkEdit1.Properties.Caption = "Delete only this link. The source BarItem and its other links will not be deleted";
				this.checkEdit1.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
				this.checkEdit1.Size = new System.Drawing.Size(416, 42);
				this.checkEdit1.StyleController = this.layoutControl1;
				this.checkEdit1.TabIndex = 0;
				this.checkEdit1.CheckedChanged += new System.EventHandler(this.checkEdit1_CheckedChanged);
				this.labelControl1.Location = new System.Drawing.Point(44, 41);
				this.labelControl1.Name = "labelControl1";
				this.labelControl1.Size = new System.Drawing.Size(199, 19);
				this.labelControl1.StyleController = this.layoutControl1;
				this.labelControl1.TabIndex = 6;
				this.labelControl1.Text = "Choose the object to delete:";
				this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
				this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
				this.layoutControlGroup1.GroupBordersVisible = false;
				this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlGroup2,
			this.layoutControlGroup3});
				this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
				this.layoutControlGroup1.Name = "layoutControlGroup1";
				this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
				this.layoutControlGroup1.Size = new System.Drawing.Size(756, 308);
				this.layoutControlGroup1.Text = "layoutControlGroup1";
				this.layoutControlGroup1.TextVisible = false;
				this.layoutControlGroup2.CustomizationFormText = "layoutControlGroup2";
				this.layoutControlGroup2.GroupBordersVisible = false;
				this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlItem2,
			this.layoutControlItem3,
			this.emptySpaceItem1});
				this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
				this.layoutControlGroup2.Name = "layoutControlGroup2";
				this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(42, 42, 39, 9);
				this.layoutControlGroup2.Size = new System.Drawing.Size(756, 266);
				this.layoutControlGroup2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
				this.layoutControlGroup2.Text = "layoutControlGroup2";
				this.layoutControlGroup2.TextVisible = false;
				this.layoutControlItem1.Control = this.labelControl1;
				this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
				this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
				this.layoutControlItem1.Name = "layoutControlItem1";
				this.layoutControlItem1.Size = new System.Drawing.Size(756, 72);
				this.layoutControlItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(39, 0, 34, 10);
				this.layoutControlItem1.Text = "layoutControlItem1";
				this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
				this.layoutControlItem1.TextToControlDistance = 0;
				this.layoutControlItem1.TextVisible = false;
				this.layoutControlItem2.Control = this.checkEdit1;
				this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
				this.layoutControlItem2.Location = new System.Drawing.Point(0, 72);
				this.layoutControlItem2.Name = "layoutControlItem2";
				this.layoutControlItem2.Size = new System.Drawing.Size(756, 27);
				this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
				this.layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(39, 39, 0, 0);
				this.layoutControlItem2.Text = "layoutControlItem2";
				this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
				this.layoutControlItem2.TextToControlDistance = 0;
				this.layoutControlItem2.TextVisible = false;
				this.layoutControlItem3.Control = this.checkEdit2;
				this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
				this.layoutControlItem3.Location = new System.Drawing.Point(0, 99);
				this.layoutControlItem3.Name = "layoutControlItem3";
				this.layoutControlItem3.Size = new System.Drawing.Size(756, 27);
				this.layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(39, 39, 0, 0);
				this.layoutControlItem3.Text = "layoutControlItem3";
				this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
				this.layoutControlItem3.TextToControlDistance = 0;
				this.layoutControlItem3.TextVisible = false;
				this.emptySpaceItem1.AllowHotTrack = false;
				this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
				this.emptySpaceItem1.Location = new System.Drawing.Point(0, 126);
				this.emptySpaceItem1.Name = "emptySpaceItem1";
				this.emptySpaceItem1.Size = new System.Drawing.Size(756, 140);
				this.emptySpaceItem1.Text = "emptySpaceItem1";
				this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
				this.layoutControlGroup3.CustomizationFormText = "layoutControlGroup3";
				this.layoutControlGroup3.GroupBordersVisible = false;
				this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.emptySpaceItem2,
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.layoutControlItem6});
				this.layoutControlGroup3.Location = new System.Drawing.Point(0, 266);
				this.layoutControlGroup3.Name = "layoutControlGroup3";
				this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(13, 12, 0, 12);
				this.layoutControlGroup3.Size = new System.Drawing.Size(756, 42);
				this.layoutControlGroup3.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
				this.layoutControlGroup3.Text = "layoutControlGroup3";
				this.layoutControlGroup3.TextVisible = false;
				this.emptySpaceItem2.AllowHotTrack = false;
				this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
				this.emptySpaceItem2.Location = new System.Drawing.Point(448, 0);
				this.emptySpaceItem2.Name = "emptySpaceItem2";
				this.emptySpaceItem2.Size = new System.Drawing.Size(32, 42);
				this.emptySpaceItem2.Text = "emptySpaceItem2";
				this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
				this.layoutControlItem4.Control = this.simpleButton1;
				this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
				this.layoutControlItem4.Location = new System.Drawing.Point(612, 0);
				this.layoutControlItem4.Name = "layoutControlItem4";
				this.layoutControlItem4.Size = new System.Drawing.Size(144, 42);
				this.layoutControlItem4.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 9, 0, 9);
				this.layoutControlItem4.Text = "layoutControlItem4";
				this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
				this.layoutControlItem4.TextToControlDistance = 0;
				this.layoutControlItem4.TextVisible = false;
				this.layoutControlItem5.Control = this.simpleButton2;
				this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
				this.layoutControlItem5.Location = new System.Drawing.Point(480, 0);
				this.layoutControlItem5.Name = "layoutControlItem5";
				this.layoutControlItem5.Size = new System.Drawing.Size(132, 42);
				this.layoutControlItem5.Spacing = new DevExpress.XtraLayout.Utils.Padding(8, 0, 0, 0);
				this.layoutControlItem5.Text = "layoutControlItem5";
				this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
				this.layoutControlItem5.TextToControlDistance = 0;
				this.layoutControlItem5.TextVisible = false;
				this.layoutControlItem6.Control = this.checkEdit3;
				this.layoutControlItem6.CustomizationFormText = "layoutControlItem6";
				this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
				this.layoutControlItem6.Name = "layoutControlItem6";
				this.layoutControlItem6.Size = new System.Drawing.Size(314, 42);
				this.layoutControlItem6.Spacing = new DevExpress.XtraLayout.Utils.Padding(10, 0, 0, 0);
				this.layoutControlItem6.Text = "layoutControlItem6";
				this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
				this.layoutControlItem6.TextToControlDistance = 0;
				this.layoutControlItem6.TextVisible = false;
				this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
				this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
				this.CancelButton = this.simpleButton1;
				this.ClientSize = new System.Drawing.Size(504, 211);
				this.ControlBox = false;
				this.Controls.Add(this.layoutControl1);
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
				this.Name = "Form1";
				this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
				this.Text = "Delete Confirmation";
				((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
				this.layoutControl1.ResumeLayout(false);
				((System.ComponentModel.ISupportInitialize)(this.checkEdit3.Properties)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.checkEdit2.Properties)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
				this.ResumeLayout(false);
			}
			private void checkEdit1_CheckedChanged(object sender, EventArgs e) {
				if(checkEdit1.Checked) checkEdit2.Checked = false;
			}
			private void checkEdit2_CheckedChanged(object sender, EventArgs e) {
				if(checkEdit2.Checked) checkEdit1.Checked = false;
			}
		#endregion
			private DevExpress.XtraLayout.LayoutControl layoutControl1;
			private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
			private DevExpress.XtraEditors.CheckEdit checkEdit1;
			private DevExpress.XtraEditors.LabelControl labelControl1;
			private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
			private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
			private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
			private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
			private DevExpress.XtraEditors.CheckEdit checkEdit2;
			private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
			private DevExpress.XtraEditors.CheckEdit checkEdit3;
			private DevExpress.XtraEditors.SimpleButton simpleButton2;
			private DevExpress.XtraEditors.SimpleButton simpleButton1;
			private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
			private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
			private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
			private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
			private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		}
		#endregion
		public virtual void OnKeyCut(object sender, EventArgs e) {
			PassControlToOldKeyCutHandler();
		}
		public virtual void Dispose() {
			RestoreMenuCommands();
		}
	}
}
