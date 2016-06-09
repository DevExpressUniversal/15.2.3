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
using System.Linq;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraBars.Docking;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraReports.Design;
using DevExpress.XtraPrinting.Control.Native;
namespace DevExpress.XtraReports.UserDesigner {
	public interface IDesignControl {
		XRDesignPanel XRDesignPanel { get; set; }
	}
	public interface IDesignPanelListener {
		XRDesignPanel XRDesignPanel { get; set; }
	}
	internal interface IWeakServiceProvider : IServiceProvider {
		IServiceProvider ServiceProvider { get; set; }
	}
	[
	ToolboxItem(false),
	Designer("DevExpress.XtraReports.Design.DesignControlContainerDesigner, " + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(IDesigner)),
	]
	public class DesignControlContainer : ControlContainer {
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public new Control.ControlCollection Controls {
			get { return base.Controls; }
		}
	}
	public class DockPanelCollection : System.Collections.ObjectModel.Collection<DockPanel> {
		internal DockPanelCollection() {
		}
		public void AddRange(DockPanel[] panels) {
			foreach(DockPanel panel in panels)
				Add(panel);
		}
	}
	[
#if !DEBUG
#endif // DEBUG
Designer("DevExpress.XtraReports.Design.DesignDockManagerDesigner, " + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(IDesigner)),
	ToolboxBitmap(typeof(LocalResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRDesignDockManager.bmp"),
	Description("Allows you to embed predefined dock panels when creating an End-User Report Designer."),
	ToolboxItem(false),
]
	public class XRDesignDockManager : DockManager, IDesignControl, IDesignPanelListener {
		#region static
		static Dictionary<DesignDockPanelType, Guid> guids = new Dictionary<DesignDockPanelType, Guid>();
		static XRDesignDockManager() {
			guids.Add(DesignDockPanelType.FieldList, new System.Guid("faf69838-a93f-4114-83e8-d0d09cc5ce95"));
			guids.Add(DesignDockPanelType.PropertyGrid, new System.Guid("b38d12c3-cd06-4dec-b93d-63a0088e495a"));
			guids.Add(DesignDockPanelType.ReportExplorer, new System.Guid("fb3ec6cc-3b9b-4b9c-91cf-cff78c1edbf1"));
			guids.Add(DesignDockPanelType.ToolBox, new System.Guid("161a5a1a-d9b9-4f06-9ac4-d0c3e507c54f"));
			guids.Add(DesignDockPanelType.GroupAndSort, new System.Guid("4bab159e-c495-4d67-87dc-f4e895da443e"));
			guids.Add(DesignDockPanelType.ErrorList, new System.Guid("5a9a01fd-6e95-4e81-a8c4-ac63153d7488"));
		}
		#endregion
		#region PropertiesWindowCommandHandler
		class PropertiesWindowCommandHandler : ICommandHandler {
			XRDesignDockManager manager;
			public PropertiesWindowCommandHandler(XRDesignDockManager manager) {
				this.manager = manager;
			}
			public void HandleCommand(ReportCommand command, object[] args) {
				DockPanel panel = manager[DesignDockPanelType.PropertyGrid];
				if(panel != null) panel.Show();
			}
			public bool CanHandleCommand(ReportCommand command, ref bool useNextHandler) {
				return command == ReportCommand.PropertiesWindow;
			}
		}
		#endregion
		DesignDockPanelType designDockPanelType;
		bool createTypedDockPanels;
		PropertiesWindowCommandHandler handler;
		DockPanelCollection visiblePanels;
		DockPanelCollection autoHidePanels;
		Dictionary<Type, int> typeToindex = new Dictionary<Type, int>();
		#region properties
		XRDesignPanel xrDesignPanel;
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignDockManagerXRDesignPanel"),
#endif
		SRCategory(DevExpress.XtraReports.Localization.ReportStringId.CatUserDesigner),
		DefaultValue(null),
		]
		public XRDesignPanel XRDesignPanel {
			get { return xrDesignPanel; }
			set {
				SetXRDesignPanel(value);
				SetXRDesignPanelProperty();
			}
		}
		XRDesignPanel IDesignPanelListener.XRDesignPanel {
			get { return xrDesignPanel; }
			set { SetXRDesignPanel(value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete(),
		]
		public DesignDockPanel[] DesignDockPanels {
			get { return GetDesignDockPanels().ToArray<DesignDockPanel>(); }
			set { }
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignDockManagerImages"),
#endif
		SRCategory(DevExpress.XtraReports.Localization.ReportStringId.CatUserDesigner),
		DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new DevExpress.Utils.Images Images {
			get {
				return (base.Images as DevExpress.Utils.ImageCollection).Images;
			}
		}
		[
		DefaultValue(null),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public DevExpress.Utils.ImageCollectionStreamer ImageStream {
			get { return (base.Images as DevExpress.Utils.ImageCollection).ImageStream; }
			set { (base.Images as DevExpress.Utils.ImageCollection).ImageStream = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public DesignDockPanel this[DesignDockPanelType panelType] {
			get {
				foreach(TypedDesignDockPanel panel in GetDesignDockPanels()) {
					if(panel.PanelType == panelType)
						return panel;
				}
				return null;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public DockPanelCollection SavedVisiblePanels {
			get { return visiblePanels; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public DockPanelCollection SavedAutoHidePanels {
			get { return autoHidePanels; }
		}
		#endregion
		public XRDesignDockManager()
			: base() {
			InitializeComponent();
		}
		public XRDesignDockManager(IContainer container)
			: base(container) {
			InitializeComponent();
		}
		void InitializeComponent() {
			base.Images = ImageCollectionHelper.CreateVoidImageCollection();
			AddImage(XRBitmaps.FieldListDockPanel, typeof(FieldListDockPanel));
			AddImage(XRBitmaps.GroupSortDockPanel, typeof(GroupAndSortDockPanel));
			AddImage(XRBitmaps.PropertyGridDockPanel, typeof(PropertyGridDockPanel));
			AddImage(XRBitmaps.ReportExplorerDockPanel, typeof(ReportExplorerDockPanel));
			AddImage(XRBitmaps.ToolBoxDockPanel, typeof(ToolBoxDockPanel));
			AddImage(XRBitmaps.ErrorListDockPanel, typeof(ErrorListDockPanel));
			handler = new PropertiesWindowCommandHandler(this);
			visiblePanels = new DockPanelCollection();
			autoHidePanels = new DockPanelCollection();
		}
		void SetXRDesignPanel(XRDesignPanel value) {
			if(xrDesignPanel != null)
				UnSubscribeDesignPanelEvents();
			xrDesignPanel = value;
			if(xrDesignPanel != null)
				SubscribeDesignPanelEvents();
		}
		void AddImage(Image image, Type type) {
			Images.Add(image);
			typeToindex.Add(type, Images.Count - 1);
		}
		public virtual void Initialize(XRDesignPanel designPanel, DesignDockPanelType panelTypes) {
			Clear();
			createTypedDockPanels = true;
			SetDefaultPanels(panelTypes);
			createTypedDockPanels = false;
			XRDesignPanel = designPanel;
		}
		internal IEnumerable<TypedDesignDockPanel> GetDesignDockPanels() {
			foreach(DockPanel panel in Panels) {
				if(panel is TypedDesignDockPanel)
					yield return (TypedDesignDockPanel)panel;
			}
		}
		internal IEnumerable<TypedDesignDockPanel> GetDesignDockPanels2(IEnumerable controls) {
			foreach(Control item in controls) {
				if(item is TypedDesignDockPanel) {
					yield return (TypedDesignDockPanel)item;
					continue;
				}
				foreach(TypedDesignDockPanel panel in GetDesignDockPanels2(item.Controls))
					yield return panel;
			}
		}
		internal void SetTemporaryDockPanelsVisibility(bool visible) {
			if(DesignMode && ((IDesignerHost)GetService(typeof(IDesignerHost))).Loading)
				return;
			DockPanelHelper panelHelper = new DockPanelHelper();
			if(visible) {
				panelHelper.RestorePanelsVisibility(SavedVisiblePanels, DockVisibility.Visible);
				panelHelper.RestorePanelsVisibility(SavedAutoHidePanels, DockVisibility.AutoHide);
			} else {
				ArrayList allPanels = new ArrayList(Panels);
				panelHelper.HidePanels(allPanels, SavedVisiblePanels, DockVisibility.Visible);
				panelHelper.HidePanels(allPanels, SavedAutoHidePanels, DockVisibility.AutoHide);
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(autoHidePanels != null) {
					autoHidePanels.Clear();
					autoHidePanels = null;
				}
				if(visiblePanels != null) {
					visiblePanels.Clear();
					visiblePanels = null;
				}
				if(xrDesignPanel != null) {
					UnSubscribeDesignPanelEvents();
					xrDesignPanel = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override DockPanel CreateDockPanel(DockingStyle dock, bool createControlContainer) {
			if(!createControlContainer || !createTypedDockPanels) return base.CreateDockPanel(dock, createControlContainer);
			switch(designDockPanelType) {
				case DesignDockPanelType.FieldList:
					return new FieldListDockPanel(this, dock);
				case DesignDockPanelType.PropertyGrid:
					return new PropertyGridDockPanel(this, dock);
				case DesignDockPanelType.ReportExplorer:
					return new ReportExplorerDockPanel(this, dock);
				case DesignDockPanelType.ToolBox:
					return new ToolBoxDockPanel(this, dock);
				case DesignDockPanelType.GroupAndSort:
					return new GroupAndSortDockPanel(this, dock);
				case DesignDockPanelType.ErrorList:
					return new ErrorListDockPanel(this, dock);
			}
			return null;
		}
		protected override void OnControllerChanged(object sender) {
			base.OnControllerChanged(sender);
			if(!Disposing)
				UpdataLookAndFeel(LookAndFeel);
		}
		void UpdataLookAndFeel(DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel) {
			foreach(TypedDesignDockPanel panel in GetDesignDockPanels2(RootPanels))
				panel.UpdateLookAndFeel(lookAndFeel);
		}
		protected virtual void SetDefaultPanels(DesignDockPanelType panelTypes) {
			Dictionary<DesignDockPanelType, DesignDockPanel> panels = new Dictionary<DesignDockPanelType, DesignDockPanel>();
			if((panelTypes & DesignDockPanelType.FieldList) > 0)
				panels[DesignDockPanelType.FieldList] = AddDesignDockPanel(DesignDockPanelType.FieldList, DockingStyle.Right);
			if((panelTypes & DesignDockPanelType.PropertyGrid) > 0)
				panels[DesignDockPanelType.PropertyGrid] = AddDesignDockPanel(DesignDockPanelType.PropertyGrid, DockingStyle.Right);
			if((panelTypes & DesignDockPanelType.ReportExplorer) > 0)
				panels[DesignDockPanelType.ReportExplorer] = AddDesignDockPanel(DesignDockPanelType.ReportExplorer, DockingStyle.Right);
			if((panelTypes & DesignDockPanelType.ToolBox) > 0)
				panels[DesignDockPanelType.ToolBox] = AddDesignDockPanel(DesignDockPanelType.ToolBox, DockingStyle.Left);
			if((panelTypes & DesignDockPanelType.GroupAndSort) > 0)
				panels[DesignDockPanelType.GroupAndSort] = AddDesignDockPanel(DesignDockPanelType.GroupAndSort, DockingStyle.Bottom);
			if((panelTypes & DesignDockPanelType.ErrorList) > 0)
				panels[DesignDockPanelType.ErrorList] = AddDesignDockPanel(DesignDockPanelType.ErrorList, DockingStyle.Bottom);
			PerfromAction1(panels, DesignDockPanelType.ToolBox, panel => panel.Width = 165);
			PerfromAction1(panels, DesignDockPanelType.ReportExplorer, panel => panel.Width = 250);
			PerfromAction1(panels, DesignDockPanelType.GroupAndSort, panel => panel.Width = 160);
			PerfromAction1(panels, DesignDockPanelType.ErrorList, panel => panel.Width = 160);
			PerfromAction2(panels, DesignDockPanelType.PropertyGrid, DesignDockPanelType.ReportExplorer, (panel1, panel2) => panel1.DockTo(panel2));
			PerfromAction2(panels, DesignDockPanelType.FieldList, DesignDockPanelType.ReportExplorer, (panel1, panel2) => panel1.DockAsTab(panel2));
			PerfromAction2(panels, DesignDockPanelType.ErrorList, DesignDockPanelType.GroupAndSort, (panel1, panel2) => panel1.DockAsTab(panel2));
		}
		static void PerfromAction1(IDictionary<DesignDockPanelType, DesignDockPanel> panels, DesignDockPanelType paneltype, Action1<DesignDockPanel> action) {
			DesignDockPanel panel;
			if(panels.TryGetValue(paneltype, out panel))
				action(panel);
		}
		static void PerfromAction2(IDictionary<DesignDockPanelType, DesignDockPanel> panels, DesignDockPanelType paneltype1, DesignDockPanelType paneltype2, Action2<DesignDockPanel, DesignDockPanel> action) {
			DesignDockPanel panel1;
			DesignDockPanel panel2;
			if(panels.TryGetValue(paneltype1, out panel1) && panels.TryGetValue(paneltype2, out panel2))
				action(panel1, panel2);
		}
		protected DesignDockPanel AddDesignDockPanel(DesignDockPanelType panelType, DockingStyle dockingStyle) {
			Guid panelID = guids[panelType];
			foreach(DockPanel dockPanel in Panels)
				if(dockPanel.ID == panelID)
					return null;
			designDockPanelType = panelType;
			TypedDesignDockPanel panel = AddPanel(dockingStyle) as TypedDesignDockPanel;
			if(panel == null) return panel;
			panel.ID = panelID;
			AddDesignDockPanelImage(panel);
			return panel;
		}
		protected override string GetDockPanelText(DockPanel panel) {
			if(panel is DesignDockPanel)
				return panel.Text;
			return base.GetDockPanelText(panel);
		}
		void SubscribeDesignPanelEvents() {
			xrDesignPanel.Disposed += new EventHandler(OnDesignPanelDisposed);
			xrDesignPanel.SetCommandVisibility(ReportCommand.PropertiesWindow, CommandVisibility.All);
			xrDesignPanel.AddCommandHandler(handler);
		}
		void UnSubscribeDesignPanelEvents() {
			xrDesignPanel.Disposed -= new EventHandler(OnDesignPanelDisposed);
			xrDesignPanel.RemoveCommandHandler(handler);
		}
		void OnDesignPanelDisposed(object sender, EventArgs e) {
			UnSubscribeDesignPanelEvents();
			XRDesignPanel = null;
		}
		void AddDesignDockPanelImage(TypedDesignDockPanel panel) {
			int index = -1;
			if(typeToindex.TryGetValue(panel.GetType(), out index)) {
				panel.ImageIndex = index;
				return;
			}
			IDesignControl designControl = panel.DesignControl;
			try {
				Bitmap toolboxBitmap = DesignImageHelper.Get256ColorBitmap(designControl.GetType());
				AddImage(toolboxBitmap, panel.GetType());
				panel.ImageIndex = Images.Count - 1;
			} catch { }
		}
		void SetXRDesignPanelProperty() {
			if(xrDesignPanel == null) return;
			foreach(TypedDesignDockPanel panel in GetDesignDockPanels()) {
				panel.XRDesignPanel = xrDesignPanel;
			}
		}
		internal void SetWindowVisibility(DesignDockPanelType designDockPanels, bool visible) {
			EUDVisibility eudVisibility = visible ? EUDVisibility.Visible : EUDVisibility.Hidden;
			foreach(TypedDesignDockPanel panel in GetDesignDockPanels().ToArray<TypedDesignDockPanel>()) {
				if((designDockPanels & panel.PanelType) > 0)
					panel.EUDVisibility = eudVisibility;
			}
		}
		internal void ForceLocalize() {
			foreach(TypedDesignDockPanel panel in GetDesignDockPanels2(RootPanels)) {
				panel.LocalizeText();
			}
		}
		public override void EndInit() {
			base.EndInit();
			foreach(TypedDesignDockPanel panel in GetDesignDockPanels2(RootPanels)) {
				panel.LocalizeText();
				panel.UpdateLookAndFeel(LookAndFeel);
			}
		}
	}
}
namespace DevExpress.XtraReports.UserDesigner.Native {
	public static class DesignDockManagerTestHelper {
		public static int DockPanelsCount {
			get {
				return Enum.GetValues(typeof(DesignDockPanelType)).Length - 1;
			}
		}
		public static TypedDesignDockPanel[] GetDesignDockPanels(XRDesignDockManager manager) {
			return manager.GetDesignDockPanels().ToArray<TypedDesignDockPanel>();
		}
	}
}
