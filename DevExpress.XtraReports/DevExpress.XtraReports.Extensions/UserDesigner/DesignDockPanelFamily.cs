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
using System.Collections;
using System.Diagnostics;
using DevExpress.XtraBars.Docking;
using System.ComponentModel.Design;
using System.Windows.Forms; 
using System.Reflection;
using System.Drawing;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraNavBar;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraTreeList;
using DevExpress.XtraReports.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.UI;
using DevExpress.Data.Browsing.Design;
using System.Globalization;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraReports.UserDesigner {
	internal enum EUDVisibility { Default, Visible, Hidden }
	[
	ToolboxItem(false),
	]
	public class DesignDockPanel : DockPanel {
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override string Text {
			get {
				return base.Text;
			}
			set {
				base.Text = value;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual XRDesignPanel XRDesignPanel { get { return null; } set { } 
		}
		public DesignDockPanel(DockManager dockManager, DockingStyle dock) : base(true, dock, dockManager) {
		}
		public DesignDockPanel()  {
		}
	}
	public abstract class TypedDesignDockPanel : DesignDockPanel, IDesignPanelListener {
		EUDVisibility eudVisibility = EUDVisibility.Default;
		IDesignControl designControl;
		XRDesignPanel xrDesignPanel;
		internal IDesignControl DesignControl { get { return designControl; }
		}
		protected internal abstract DesignDockPanelType PanelType { get; }
		protected abstract string LocalizedText { get; }
		internal EUDVisibility EUDVisibility {
			get { return eudVisibility; }
			set {
				if(eudVisibility == value)
					return;
				eudVisibility = value;
				ApplyEUDVisibility();
			}
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("TypedDesignDockPanelXRDesignPanel"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		]
		public override XRDesignPanel XRDesignPanel {
			get { return xrDesignPanel; }
			set {
				xrDesignPanel = value;
				if (designControl != null)
					designControl.XRDesignPanel = XRDesignPanel;
			}
		}
		protected TypedDesignDockPanel(DockManager dockManager, DockingStyle dock) : base(dockManager, dock) {
			InitDesignControl();
			LocalizeText();
		}
		protected TypedDesignDockPanel() {
			InitDesignControl();
		}
		internal void LocalizeText() {
			Text = LocalizedText;
		}
		internal void UpdateLookAndFeel(UserLookAndFeel lookAndFeel) {
			ISupportLookAndFeel supportLookAndFeel = designControl as ISupportLookAndFeel;
			if(supportLookAndFeel != null && !ReferenceEquals(supportLookAndFeel.LookAndFeel.ParentLookAndFeel, lookAndFeel))
				supportLookAndFeel.LookAndFeel.ParentLookAndFeel = lookAndFeel;
		}
		protected override void CreateControlContainer() {
			InitDesignControl();
			Controls.Add(new DesignControlContainer());
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			Control control = designControl as Control;
			if(control != null && control.Parent == null && ControlContainer != null)
				ControlContainer.Controls.Add(control);
		}
		protected virtual IDesignControl CreateDesignControl() {
			return null;
		}
		void InitDesignControl() {
			if(designControl == null) {
				designControl = CreateDesignControl();
				if(designControl != null) 
					((Control)designControl).Dock = DockStyle.Fill;
			}
		}
		void ApplyEUDVisibility() {
			if(eudVisibility == EUDVisibility.Visible) {
				if(Visibility == DockVisibility.Hidden)
					Visibility = DockVisibility.Visible;
			}
			if(eudVisibility == EUDVisibility.Hidden)
				Visibility = DockVisibility.Hidden;
		}
	}
	public abstract class TreeViewDesignDockPanel : TypedDesignDockPanel {
		IDesignControl treeView;
		protected TreeViewDesignDockPanel(DockManager dockManager, DockingStyle dock) : base(dockManager, dock) {
		}
		protected TreeViewDesignDockPanel() {
		}
		public void ExpandAll() {
			if(treeView is TreeView) ((TreeView)treeView).ExpandAll();
			else if(treeView is TreeList) ((TreeList)treeView).ExpandAll();
		}
		public void CollapseAll() {
			if(treeView is TreeView) ((TreeView)treeView).CollapseAll();
			else if(treeView is TreeList) ((TreeList)treeView).CollapseAll();
		}
		protected override IDesignControl CreateDesignControl() {
			treeView = CreateTreeViewDesignControl();
			return (IDesignControl)treeView;
		}
		protected abstract IDesignControl CreateTreeViewDesignControl();
	}
	public class FieldListDockPanel : TreeViewDesignDockPanel {
		bool showNodeToolTips = true;
		XRDesignFieldList fieldList;
		protected internal override DesignDockPanelType PanelType { get { return DesignDockPanelType.FieldList; } 
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("FieldListDockPanelShowNodeToolTips"),
#endif
		Category("Appearance"),
		DefaultValue(true),
		]
		public bool ShowNodeToolTips {
			get { return showNodeToolTips; }
			set {
				showNodeToolTips = value;
				if (fieldList != null) fieldList.ShowNodeToolTips = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("FieldListDockPanelShowParametersNode"),
#endif
		Category("Behavior"),
		DefaultValue(true),
		]
		public bool ShowParametersNode {
			get { return fieldList.ShowParametersNode; }
			set { fieldList.ShowParametersNode = value; }
		}
		protected override string LocalizedText {
			get { return ReportLocalizer.GetString(ReportStringId.UD_Title_FieldList); }
		}
		public FieldListDockPanel(DockManager dockManager, DockingStyle dock) : base(dockManager, dock) {
		}
		public FieldListDockPanel()  {
		}
		public void UpdateDataSource(IDesignerHost designerHost) {
			if (fieldList != null) {
				fieldList.UpdateDataSource(designerHost);
			}
		}
		protected override IDesignControl CreateTreeViewDesignControl() {
			fieldList = new XRDesignFieldList();
			return fieldList;
		}
	}
	[Designer("DevExpress.XtraReports.Design.PropertyGridDockPanelDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class PropertyGridDockPanel : TypedDesignDockPanel {
		bool showDescription = true;
		bool showCategories = true;
		XRDesignPropertyGrid xrDesignPropertyGrid;
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("PropertyGridDockPanelShowCategories"),
#endif
		DefaultValue(true),
		SRCategory(ReportStringId.CatBehavior),
		]
		public bool ShowCategories {
			get { 
				return xrDesignPropertyGrid != null && xrDesignPropertyGrid.PropertyGrid != null ?
					xrDesignPropertyGrid.PropertyGrid.ShowCategories :
					showCategories; 
			}
			set {
				showCategories = value;
				SynchShowCategories();
			}
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("PropertyGridDockPanelShowDescription"),
#endif
		DefaultValue(true),
		SRCategory(ReportStringId.CatBehavior),
		]
		public bool ShowDescription {
			get { return showDescription; }
			set {
				showDescription = value;
				SynchShowDescription();
			}
		}
		protected internal override DesignDockPanelType PanelType {
			get { return DesignDockPanelType.PropertyGrid; } 
		}
		protected override string LocalizedText { get { return ReportLocalizer.GetString(ReportStringId.UD_Title_PropertyGrid); }
		}
		public PropertyGridDockPanel(DockManager dockManager, DockingStyle dock) : base(dockManager, dock) {
			DXDisplayNameAttribute.UseResourceManager = DevExpress.XtraReports.Configuration.Settings.Default.ShowUserFriendlyNamesInUserDesigner;
		}
		public PropertyGridDockPanel()  {
			DXDisplayNameAttribute.UseResourceManager = DevExpress.XtraReports.Configuration.Settings.Default.ShowUserFriendlyNamesInUserDesigner;
		}
		protected override IDesignControl CreateDesignControl() {
			xrDesignPropertyGrid = new XRDesignPropertyGrid();
			SynchShowDescription();
			SynchShowCategories();
			return xrDesignPropertyGrid;
		}
		void SynchShowDescription() { 
			if(xrDesignPropertyGrid != null)
				xrDesignPropertyGrid.PropertyGrid.ShowDescription = this.ShowDescription;
		}
		void SynchShowCategories() {
			if(xrDesignPropertyGrid != null)
				xrDesignPropertyGrid.PropertyGrid.ShowCategories = this.showCategories;
		}
	}
	public class ToolBoxDockPanel : TypedDesignDockPanel {
		XRDesignToolBox XRDesignToolBox { get { return DesignControl as XRDesignToolBox; }
		}
		protected internal override DesignDockPanelType PanelType { get { return DesignDockPanelType.ToolBox; } 
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("ToolBoxDockPanelGroupsStyle"),
#endif
		Category("Appearance"),
		DefaultValue(NavBarGroupStyle.SmallIconsText),
		]
		public NavBarGroupStyle GroupsStyle {
			get { return XRDesignToolBox != null ? XRDesignToolBox.GroupsStyle : NavBarGroupStyle.Default; }
			set {
				if (XRDesignToolBox == null) return;
				XRDesignToolBox.GroupsStyle = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("ToolBoxDockPanelPaintStyleName"),
#endif
		TypeConverter("DevExpress.XtraReports.Design.NavBarViewNamesConverter, " + AssemblyInfo.SRAssemblyReportsDesignFull),
		Category("Appearance"),
		DefaultValue("Default")
		]
		public string PaintStyleName {
			get { return XRDesignToolBox != null ? XRDesignToolBox.PaintStyleName : "Default"; }
			set {
				if (XRDesignToolBox == null) return;
				XRDesignToolBox.PaintStyleName = value;
			}
		}
		protected override string LocalizedText { get { return ReportLocalizer.GetString(ReportStringId.UD_Title_ToolBox); }
		}
		public ToolBoxDockPanel(DockManager dockManager, DockingStyle dock) : base(dockManager, dock) {
		}
		public ToolBoxDockPanel()  {
		}
		protected override IDesignControl CreateDesignControl() {
			XRDesignToolBox xrDesignToolBox = new XRDesignToolBox();
			xrDesignToolBox.PaintStyleName = "Default";
			return xrDesignToolBox;
		}
	}
	public class ReportExplorerDockPanel : TreeViewDesignDockPanel {
		protected internal override DesignDockPanelType PanelType { get { return DesignDockPanelType.ReportExplorer; } 
		}
		protected override string LocalizedText { get { return ReportLocalizer.GetString(ReportStringId.UD_Title_ReportExplorer); }
		}
		public ReportExplorerDockPanel(DockManager dockManager, DockingStyle dock) : base(dockManager, dock) {
		}
		public ReportExplorerDockPanel()  {
		}
		protected override IDesignControl CreateTreeViewDesignControl() {
			return new XRDesignReportExplorer();
		}
	}
	public class GroupAndSortDockPanel : TypedDesignDockPanel {
		protected internal override DesignDockPanelType PanelType {
			get { return DesignDockPanelType.GroupAndSort; }
		}
		protected override string LocalizedText {
			get { return ReportLocalizer.GetString(ReportStringId.UD_Title_GroupAndSort); }
		}
		public GroupAndSortDockPanel(DockManager dockManager, DockingStyle dock) : base(dockManager, dock) {
		}
		public GroupAndSortDockPanel() {
		}
		protected override IDesignControl CreateDesignControl() {
			return new XRDesignGroupAndSort();
		}
	}
	public class ErrorListDockPanel : TypedDesignDockPanel {
		protected internal override DesignDockPanelType PanelType {
			get { return DesignDockPanelType.ErrorList; }
		}
		protected override string LocalizedText {
			get { return ReportLocalizer.GetString(ReportStringId.UD_Title_ErrorList); }
		}
		public ErrorListDockPanel(DockManager dockManager, DockingStyle dock)
			: base(dockManager, dock) {
		}
		public ErrorListDockPanel() {
		}
		protected override IDesignControl CreateDesignControl() {
			return new XRDesignErrorList();
		}
	}
}
