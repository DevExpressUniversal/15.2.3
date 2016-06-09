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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraBars.Design;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Docking2010;
using DevExpress.Utils.Mdi;
namespace DevExpress.XtraBars.Docking.Design {
	public class DesignerSelectionHelper {
		static Type[] DockControlTypes = new Type[] {
														typeof(ControlContainer),
														typeof(DockPanel),
														typeof(AutoHideContainer),
		};
		static CommandID[][] DisabledCommands = new CommandID[][] {
																	  new CommandID[] { MenuCommands.Copy, MenuCommands.Cut, MenuCommands.Delete },
																	  new CommandID[] { MenuCommands.Copy, MenuCommands.Cut, MenuCommands.Paste },
																	  new CommandID[] { MenuCommands.Copy, MenuCommands.Cut, MenuCommands.Paste },
		};
		static ArrayList enabledState = new ArrayList();
		static int savedIndex = -1;
		protected static void SetMenuComandsEnabledState(bool enable, int index, IServiceProvider serviceProvider) {
			if(index < 0) return;
			CommandID[] commandsToDisable = DisabledCommands[index];
			if(!enable) enabledState.Clear();
			int i = 0;
			IMenuCommandService servMenu = serviceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			foreach(CommandID commId in commandsToDisable) {
				MenuCommand command = servMenu.FindCommand(commId);
				if(command != null && enabledState.Count > i) {
					if(enable) {
						command.Enabled = (bool)enabledState[i];
					}
					else {
						enabledState.Add(command.Enabled);
						command.Enabled = false;
					}
				}
				i++;
			}
		}
		public static void SelectionChanged(ISelectionService selServ, IServiceProvider serviceProvider) {
			if(selServ.PrimarySelection == null) return;
			Type componentType = selServ.PrimarySelection.GetType();
			int index = Array.IndexOf(DockControlTypes, componentType);
			if(index == -1) {
				if(componentType.Equals(typeof(DevExpress.XtraBars.Docking.FloatForm)))
					selServ.SetSelectedComponents(null);
				if(savedIndex > 0) SetMenuComandsEnabledState(true, savedIndex, serviceProvider);
				return;
			}
			if(savedIndex != index) SetMenuComandsEnabledState(true, index, serviceProvider);
			CommandID[] commandsToDisable = DisabledCommands[index];
			savedIndex = index;
			SetMenuComandsEnabledState(false, savedIndex, serviceProvider);
		}
	}
	public class DockManagerActionList : DesignerActionList {
		DockManagerDesigner designerCore;
		public DockManagerActionList(DockManagerDesigner designer)
			: base(designer.Component) {
			this.designerCore = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionMethodItem(this, "AddLeft", "Add Panel at Left", "Add Panel"));
			res.Add(new DesignerActionMethodItem(this, "AddTop", "Add Panel at Top", "Add Panel"));
			res.Add(new DesignerActionMethodItem(this, "AddRight", "Add Panel at Right", "Add Panel"));
			res.Add(new DesignerActionMethodItem(this, "AddBottom", "Add Panel at Bottom", "Add Panel"));
			res.Add(new DesignerActionMethodItem(this, "AddFloat", "Add Float Panel", "Add Panel"));
			if(DocumentManager == null || (DocumentManager != null && DocumentManager.ContainerControl != null && (DocumentManager.View == null || DocumentManager.View is DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView)))
				res.Add(new DesignerActionMethodItem(this, "AddPanelAsTabbedDocument", "Add Panel as Tabbed Document", "Dock as Tabbed Document"));
			res.Add(new DesignerActionMethodItem(this, "Customize", "Customize", "Actions", true));
			res.Add(new DesignerActionMethodItem(this, "RunDesigner", "Run Designer", "Actions", true));
			if(DocumentManager == null)
				res.Add(new DesignerActionMethodItem(this, "AddDocumentManager", "Add DocumentManager", "Actions"));
			res.Add(new DesignerActionMethodItem(this, "About", "About", null, true));
			return res;
		}
		protected DockManagerDesigner Designer {
			get { return designerCore; }
		}
		protected DockManager Manager {
			get { return Component as DockManager; }
		}
		public void About() {
			Docking.DockManager.About();
		}
		[RefreshProperties(RefreshProperties.All)]
		public void AddLeft() {
			Manager.AddPanel(DockingStyle.Left);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void AddTop() {
			Manager.AddPanel(DockingStyle.Top);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void AddRight() {
			Manager.AddPanel(DockingStyle.Right);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void AddBottom() {
			Manager.AddPanel(DockingStyle.Bottom);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void AddFloat() {
			Point floatLocation = GetFloatLocation(Manager.Form.Location);
			Manager.AddPanel(floatLocation);
		}
		private Point GetFloatLocation(Point point) {
			Screen screen = Screen.FromPoint(point);
			if(screen == null) return Point.Empty;
			return screen.WorkingArea.Location;
		}
		[RefreshProperties(RefreshProperties.All)]
		public void Customize() {
			using(DockCustomizationForm customizationForm = new DockCustomizationForm(Manager)) {
				if(customizationForm.ShowDialog() == DialogResult.OK) { }
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		public void RunDesigner() {
			IUIService srv = GetService(typeof(IUIService)) as IUIService;
			using(DockEditorForm form = new DockEditorForm()) {
				form.InitEditingObject(Manager);
				form.ShowDialog(srv != null ? srv.GetDialogOwnerWindow() : null);
			}
		}
		protected DocumentManager DocumentManager {
			get {
				DocumentManager documentManagerCore = null;
				if(Manager == null) return null;
				Control parentControl = Manager.Form;
				documentManagerCore = DocumentManager.FromControl(parentControl);
				return documentManagerCore;
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		public void AddDocumentManager() {
			if(Manager == null && Manager.Container == null) return;
			DocumentManager manager = new DocumentManager(Manager.Container);
			manager.ContainerControl = DesignHelpers.GetContainerControl(Manager.Container);
			Component.Site.Container.Add(manager);
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void AddPanelAsTabbedDocument() {
			if(Manager == null && Manager.Container == null) return;
			if(DocumentManager == null) AddDocumentManager();
			if(DocumentManager.View == null) DocumentManager.CreateView(DevExpress.XtraBars.Docking2010.Views.ViewType.Tabbed);
			DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView view = DocumentManager.View as DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView;
			if(view != null) {
				DockPanel panel = Manager.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Float);
				panel.DockedAsTabbedDocument = true;
			}
		}
		class DockWindowSorter : IComparer {
			static int[] styleCompareIndex = new int[] { 100, 4, 1, 3, 2, 99 };
			int IComparer.Compare(object a, object b) {
				BarDockWindowCore w1 = a as BarDockWindowCore, w2 = b as BarDockWindowCore;
				if(w1 == w2) return 0;
				if(w1.DockStyle == w2.DockStyle) {
					return Comparer.Default.Compare(w1.DockRow, w2.DockRow);
				}
				return Comparer.Default.Compare(styleCompareIndex[(int)w1.DockStyle], styleCompareIndex[(int)w2.DockStyle]);
			}
		}
		void ConvertProperties(BarDockWindowCore oldWindow, DockPanel panel) {
			panel.Text = oldWindow.DockCaption;
			panel.TabText = oldWindow.TabCaption;
			panel.ImageIndex = oldWindow.TabImageIndex;
			if(oldWindow.Controls.Count > 0) {
				Control[] controls = new Control[oldWindow.Controls.Count];
				oldWindow.Controls.CopyTo(controls, 0);
				panel.Controls.AddRange(controls);
			}
		}
		DockingStyle[] styles = new DockingStyle[] { DockingStyle.Float, DockingStyle.Left, DockingStyle.Top, DockingStyle.Right, DockingStyle.Bottom };
		DockingStyle ConvertDockStyle(BarDockStyle dockStyle) {
			return styles[(int)dockStyle];
		}
	}
	public class DockManagerDesigner : BaseComponentDesigner {
		IDesignerHost host;
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(CreateDocumentManagerActionList());
			base.RegisterActionLists(list);
		}
		protected virtual DockManagerActionList CreateDocumentManagerActionList() {
			return new DockManagerActionList(this);
		}
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		protected override bool AllowEditInherited { get { return false; } }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			DevExpress.Utils.Design.LoaderPatcherService.InstallService(host);
			HookSelectionService(true);
		}
#if DXWhidbey
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			OnInitialize();
		}
#else
		public override void OnSetComponentDefaults() {
			base.OnSetComponentDefaults();
			OnInitialize();
		}
#endif
		protected virtual void OnInitialize() {
			if(Manager == null || Manager.Container == null) return;
			if(Manager.Form != null || Manager.Container.Components == null) return;
			Manager.Form = DesignHelpers.GetContainerControl(Manager.Container);
			Manager.Controller = DesignHelpers.FindComponent(Manager.Container, typeof(BarAndDockingController)) as BarAndDockingController;
			BarManager barManager = GetBarManager();
			if(barManager != null) {
				barManager.DockManager = Manager;
				if(Manager.MenuManager == null)
					Manager.MenuManager = barManager;
			}
			ForceLoaded();
		}
		protected BarManager GetBarManager() {
			if(Manager == null || Manager.Container == null) return null;
			return DesignHelpers.FindComponent(Manager.Container, typeof(BarManager)) as BarManager;
		}
		protected virtual void ForceLoaded() {
			MethodInfo mi = typeof(DockManager).GetMethod("OnLoaded", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);
			if(mi != null) mi.Invoke(Manager, null);
		}
		protected override void Dispose(bool disposing) {
			DevExpress.Utils.Design.LoaderPatcherService.UnInstallService(host);
			HookSelectionService(false);
			this.host = null;
			base.Dispose(disposing);
		}
		void HookSelectionService(bool addHandler) {
			ISelectionService selServ = GetSelectionService();
			if(selServ == null) return;
			if(addHandler) {
				selServ.SelectionChanged += new EventHandler(Designer_SelectionChanged);
				selServ.SelectionChanging += new EventHandler(Designer_SelectionChanging);
			}
			else {
				selServ.SelectionChanged -= new EventHandler(Designer_SelectionChanged);
				selServ.SelectionChanging -= new EventHandler(Designer_SelectionChanging);
			}
		}
		ISelectionService GetSelectionService() {
			return (GetService(typeof(ISelectionService)) as ISelectionService);
		}
		int lockSelection = 0;
		protected virtual void Designer_SelectionChanging(object sender, EventArgs e) {
			if(Manager == null || lockSelection != 0) return;
			ISelectionService srv = sender as ISelectionService;
			if(srv != null) {
				ArrayList list = new ArrayList(srv.GetSelectedComponents());
				if(list.Count > 0) {
					Control ctrl = list[0] as Control;
					if(ctrl != null && ctrl.IsDisposed) {
						lockSelection++;
						try {
							srv.SetSelectedComponents(new Component[] { Manager.Form }, SelectionTypes.Replace);
						}
						finally {
							lockSelection--;
						}
					}
				}
			}
		}
		protected virtual void Designer_SelectionChanged(object sender, EventArgs e) {
			if(Manager == null) return;
			DesignerSelectionHelper.SelectionChanged(sender as ISelectionService, Manager.Site);
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DevExpress.Utils.Design.DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		protected DockManager Manager { get { return (Component as DockManager); } }
		public bool SaveLayout() {
			if(Manager == null) return false;
			string fileName = GetLayoutFile(false);
			if(string.IsNullOrEmpty(fileName)) return false;
			try {
				Manager.SaveLayoutToXml(fileName);
				return true;
			}
			catch { return false; }
		}
		public bool RestoreLayout() {
			if(Manager == null) return false;
			DialogResult warningRes = DevExpress.XtraEditors.XtraMessageBox.Show(
				"Do you want to load a layout from an XML file and override the current layout?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if(warningRes != DialogResult.Yes) return false;
			string fileName = GetLayoutFile(true);
			if(string.IsNullOrEmpty(fileName)) return false;
			try {
				Manager.RestoreLayoutFromXml(fileName);
				return true;
			}
			catch {
				MessageBox.Show("Can't load layout " + fileName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}
		static string GetLayoutFile(bool load) {
			FileDialog fileDialog;
			if(load)
				fileDialog = new OpenFileDialog();
			else
				fileDialog = new SaveFileDialog();
			using(fileDialog) {
				fileDialog.Filter = "Layouts (*.xml)|*.xml";
				fileDialog.Title = (load ? "Restore DockManager layout" : "Save DockManager layout");
				fileDialog.CheckFileExists = load;
				if(fileDialog.ShowDialog() == DialogResult.OK) {
					return fileDialog.FileName;
				}
				return null;
			}
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(About));
		}
		void About() {
		}
	}
	public class AutoHideContainerDesigner : BaseParentDockControlDesigner {
		public override bool CanParent(Control control) { return false; }
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			if(DebuggingState) return;
			MouseHelper.ProcessMouseEvent(ref m);
		}
		protected override void OnMouseEnter() {
			base.OnMouseEnter();
			MouseHelper.MouseEnter();
		}
		protected override void OnMouseLeave() {
			base.OnMouseLeave();
			MouseHelper.MouseLeave();
		}
		protected override void OnDragOver(DragEventArgs de) {
			de.Effect = DragDropEffects.None;
		}
		protected override bool GetHitTest(Point pt) { return false; }
		protected override bool DrawGrid { get { return false; } }
		public override SelectionRules SelectionRules { get { return SelectionRules.Locked; } }
	}
	public class DockManagerSerializer : CodeDomSerializer {
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			CodeDomSerializer baseClassSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(DockManager).BaseType, typeof(CodeDomSerializer));
			return baseClassSerializer.Deserialize(manager, codeObject);
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			CodeDomSerializer baseClassSerializer = (CodeDomSerializer)manager.
				GetSerializer(typeof(DockManager).BaseType, typeof(CodeDomSerializer));
			DockManager dockManager = value as DockManager;
			if(manager != null && dockManager != null) {
				MethodInfo mi = typeof(DockManager).GetMethod("BeforeSerialize", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);
				if(mi != null) mi.Invoke(dockManager, new object[] { manager });
			}
			return baseClassSerializer.Serialize(manager, value);
		}
	}
	public class AutoHideContainerSerializer : CodeDomSerializer {
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			CodeDomSerializer baseClassSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(AutoHideContainer).BaseType, typeof(CodeDomSerializer));
			return baseClassSerializer.Deserialize(manager, codeObject);
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			CodeDomSerializer baseClassSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(AutoHideContainer).BaseType, typeof(CodeDomSerializer));
			AutoHideContainer container = value as AutoHideContainer;
			if(manager != null && container != null) {
				MethodInfo mi = typeof(AutoHideContainer).GetMethod("BeforeSerialize", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);
				if(mi != null) mi.Invoke(container, new object[] { manager });
			}
			return baseClassSerializer.Serialize(manager, value);
		}
	}
	public class CustomHeaderButtonCollectionEditor : DXCollectionEditorBase {
		public CustomHeaderButtonCollectionEditor(Type type)
			: base(type) {
		}
		protected override Type[] CreateNewItemTypes() {
			return new Type[] { typeof(CustomHeaderButton) };
		}
		protected override object CreateCustomInstance(Type itemType) {
			return new CustomHeaderButton();
		}
		protected override bool AllowLiveUpdates {
			get { return true; }
		}
	}
}
