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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils.Design;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Designer.Utils;
namespace DevExpress.Web.ASPxHtmlEditor.Design {
	public class ToolbarEditorFrame : EditFrameBase {
		XtraFrame toolbarEditorFrame;
		HtmlEditorToolbarInitializer toolbarInitializer;
		List<ToolbarEditorFrameManager> frameManagers;
		protected PanelControl EditorFramePanel { get; private set; }
		protected PanelControl SelectorPanel { get; private set; }
		protected SplitterControl Splitter { get; private set; }
		protected ComboBoxEdit ToolbarModeSelector { get; private set; }
		protected ASPxHtmlEditor HtmlEditor { get { return ToolbarInitializer.HtmlEditor; } }
		protected IServiceProvider Provider { get { return ToolbarInitializer.ServiceProvider; } }
		protected virtual XtraFrame ToolbarEditFrame {
			get { return toolbarEditorFrame; }
			set {
				if(ToolbarEditFrame == value)
					return;
				if(ToolbarEditFrame != null) {
					ToolbarEditFrame.Dispose();
				}
				toolbarEditorFrame = value;
			}
		}
		protected HtmlEditorToolbarInitializer ToolbarInitializer {
			get {
				if(toolbarInitializer == null)
					toolbarInitializer = (HtmlEditorToolbarInitializer)DesignerItem.Tag;
				return toolbarInitializer;
			}
		}
		protected List<ToolbarEditorFrameManager> FrameManagers {
			get {
				if(frameManagers == null) {
					frameManagers = new List<ToolbarEditorFrameManager>();
					frameManagers.Add(CreateFrameManager(HtmlEditorToolbarMode.Menu, typeof(HtmlEditorToolbarsEditorForm), new HtmlEditorToolbarItemsOwner(HtmlEditor, Provider)));
					frameManagers.Add(CreateFrameManager(HtmlEditorToolbarMode.Ribbon, typeof(HtmlEditorRibbonToolbarEmbeddedFrame), null));
					frameManagers.Add(CreateFrameManager(HtmlEditorToolbarMode.ExternalRibbon, typeof(ExternalRibbonEditorFrame), null));
					frameManagers.Add(CreateFrameManager(HtmlEditorToolbarMode.None, typeof(ToolbarNoneEditorFrame), null));
				}
				return frameManagers;
			}
		}
		protected override string FrameName { get { return "ToolbarsEditorFrame"; } }
		protected ToolbarEditorFrameManager CreateFrameManager(HtmlEditorToolbarMode toolbarMode, Type frameType, object tag) {
			var frame = new ToolbarEditorFrameManager(toolbarMode, frameType, tag);
			if(tag is IOwnerEditingProperty)
				ToolbarInitializer.PropertyOwners.Add((IOwnerEditingProperty)tag);
			return frame;
		}
		public override void DoInitFrame() {
			base.DoInitFrame();
			SuspendLayout();
			CreateEditorPanel();
			CreateSelectorPanel();
			InitToolbarModeSelector();
			ResumeLayout(false);
		}
		protected void CreateEditorPanel() {
			EditorFramePanel = DesignTimeFormHelper.CreatePanel(MainPanel, "EditorFramePanel", System.Windows.Forms.DockStyle.Fill);
		}
		protected void CreateSelectorPanel() {
			SelectorPanel = DesignTimeFormHelper.CreatePanel(MainPanel, "SelectorPanel", System.Windows.Forms.DockStyle.Top);
			SelectorPanel.Height = 30;
			var label = new LabelControl();
			label.Location = new Point(0, 3);
			label.Name = "ToolbarModeLabel";
			label.Text = "Toolbar Mode: ";
			SelectorPanel.Controls.Add(label);
		}
		protected void InitToolbarModeSelector() {
			ToolbarModeSelector = new ComboBoxEdit();
			ToolbarModeSelector.Location = new Point(80, 0);
			ToolbarModeSelector.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			ToolbarModeSelector.Properties.Items.AddRange(FrameManagers);
			ToolbarModeSelector.SelectedIndexChanged += ToolbarModeSelector_SelectedIndexChanged;
			SelectorPanel.Controls.Add(ToolbarModeSelector);
			ToolbarModeSelector.SelectedItem = FrameManagers.FirstOrDefault(i => i.ToolbarMode == HtmlEditor.ToolbarMode);
		}
		protected void InitEditorFrameCore(ToolbarEditorFrameManager frameManager) {
			if(frameManager == null) {
				ToolbarEditFrame = null;
				return;
			}
			SuspendLayout();
			EditorFramePanel.SuspendLayout();
			var savedCursor = this.Cursor;
			try {
				this.Cursor = Cursors.WaitCursor;
				ToolbarEditFrame = frameManager.CreateFrame();
				ToolbarEditFrame.SuspendLayout();
				WindowsFormsDesignTimeSettings.ApplyDesignSettings(ToolbarEditFrame);
				ToolbarEditFrame.Bounds = EditorFramePanel.DisplayRectangle;
				ToolbarEditFrame.Dock = DockStyle.Fill;
				ToolbarEditFrame.InitFrame(EditingObject, string.Empty, null);
				ControlContainerLookAndFeelHelper.UpdateChildrenLookAndFeel(ToolbarEditFrame, LookAndFeel);
			} finally {
				ToolbarEditFrame.ResumeLayout();
				ToolbarEditFrame.EndInitialize();
				this.Cursor = savedCursor;
			}
			EditorFramePanel.Controls.Add(ToolbarEditFrame);
			EditorFramePanel.ResumeLayout();
			ResumeLayout(true);
		}
		private void ToolbarModeSelector_SelectedIndexChanged(object sender, EventArgs e) {
			var frameManager = (ToolbarEditorFrameManager)ToolbarModeSelector.SelectedItem;
			if(frameManager == null)
				return;
			InitEditorFrameCore(frameManager);
			var toolbarMode = frameManager.ToolbarMode;
			if(HtmlEditor.ToolbarMode != toolbarMode) {
				HtmlEditor.ToolbarMode = toolbarMode;
				HtmlEditor.PropertyChanged("ToolbarMode");
			}
		}
	}
	public class ToolbarEditorFrameManager {
		public Type EditorFrameType { get; private set; }
		public DesignerItem FrameDesignerItem { get; private set; }
		public HtmlEditorToolbarMode ToolbarMode { get; private set; }
		public ToolbarEditorFrameManager(HtmlEditorToolbarMode toolbarMode, Type frameType, object tag) {
			ToolbarMode = toolbarMode;
			EditorFrameType = frameType;
			FrameDesignerItem = new DesignerItem() {
				Caption = ToString(),
				FrameType = EditorFrameType,
				Tag = tag
			};
		}
		public XtraFrame CreateFrame() {
			var frame = Activator.CreateInstance(EditorFrameType) as XtraFrame;
			frame.DesignerItem = FrameDesignerItem;
			return frame;
		}
		public override string ToString() {
			return ToolbarMode.ToString();
		}
	}
	public class ExternalRibbonEditorFrame : EditFrameBase {
		Dictionary<string, ASPxRibbon> externalRibbons;
		protected DevExpress.XtraEditors.LabelControl FillRibbonTabsLink { get; set; }
		protected ComboBoxEdit RibbonSelector { get; set; }
		protected override string FrameName { get { return "ExternalRibbonEditorFrame"; } }
		protected ASPxHtmlEditor HtmlEditor { get { return (EditingObject as ASPxHtmlEditor); } }
		protected Dictionary<string, ASPxRibbon> ExternalRibbons {
			get {
				if(externalRibbons == null)
					externalRibbons = RibbonControlIDConverter.GetRibbonControlsDictionary(HtmlEditor);
				return externalRibbons;
			}
		}
		public override void DoInitFrame() {
			base.DoInitFrame();
			SuspendLayout();
			CreateLink();
			CreateRibbonSelector();
			ResumeLayout(false);
		}
		protected void CreateRibbonSelector() {
			var label = new LabelControl();
			label.Location = new Point(0, 3);
			label.Name = "RibbonSelectorLabel";
			label.Text = "External Ribbon ID: ";
			MainPanel.Controls.Add(label);
			RibbonSelector = new ComboBoxEdit();
			RibbonSelector.Location = new Point(100, 0);
			RibbonSelector.Properties.Items.AddRange(ExternalRibbons.Keys);
			RibbonSelector.EditValueChanged += RibbonSelector_EditValueChanged;
			MainPanel.Controls.Add(RibbonSelector);
			RibbonSelector.Text = HtmlEditor.AssociatedRibbonID;
		}
		void RibbonSelector_EditValueChanged(object sender, EventArgs e) {
			var text = RibbonSelector.Text;
			FillRibbonTabsLink.Visible = ExternalRibbons.Keys.Contains(text);
			if(HtmlEditor.AssociatedRibbonID != text) {
				HtmlEditor.AssociatedRibbonID = text;
				HtmlEditor.PropertyChanged("AssociatedRibbonID");
			}
		}
		protected void CreateLink() {
			FillRibbonTabsLink = new LabelControl();
			var url = DesignerItem.Tag as string;
			FillRibbonTabsLink.Location = new Point(0, 33);
			FillRibbonTabsLink.Name = "FillRibbonTabsLink";
			FillRibbonTabsLink.AllowHtmlString = true;
			FillRibbonTabsLink.Text = "<href> Fill external ribbon with default HTML editor items </href>";
			FillRibbonTabsLink.HyperlinkClick += FillRibbonTabsLink_HyperlinkClick;
			MainPanel.Controls.Add(FillRibbonTabsLink);
		}
		private void FillRibbonTabsLink_HyperlinkClick(object sender, Utils.HyperlinkClickEventArgs e) {
			if(!string.IsNullOrEmpty(HtmlEditor.AssociatedRibbonID))
				RibbonDesignerHelper.AddTabCollectionToRibbonControl(HtmlEditor.AssociatedRibbonID, new HtmlEditorDefaultRibbon(HtmlEditor).DefaultRibbonTabs, HtmlEditor);
		}
	}
	public class ToolbarNoneEditorFrame : EditFrameBase {
		protected override string FrameName { get { return "ToolbarNoneEditorFrame"; } }
		protected ASPxHtmlEditor HtmlEditor { get { return (EditingObject as ASPxHtmlEditor); } }
		protected string GetDocumentationLink() {
			object[] attrs = HtmlEditor.GetType().GetCustomAttributes(typeof(DXDocumentationProviderAttribute), true);
			var attr = attrs[0] as DXDocumentationProviderAttribute;
			return attr != null ? attr.GetUrl() : string.Empty;
		}
		public override void DoInitFrame() {
			base.DoInitFrame();
			SuspendLayout();
			CreateLinks();
			ResumeLayout(false);
		}
		protected void CreateLinks() {
			var link = new LabelControl();
			var url = GetDocumentationLink();
			link.Location = new Point(0, 3);
			link.Name = "InfoLink";
			link.AllowHtmlString = true;
			link.Text = "HTML Editor will not be linked with any toolbar but all its command are available via <href=" + url + "> Client-Side API </href>";
			link.HyperlinkClick += link_HyperlinkClick;
			MainPanel.Controls.Add(link);
		}
		private void link_HyperlinkClick(object sender, Utils.HyperlinkClickEventArgs e) {
			System.Diagnostics.Process.Start(e.Link);
		}
	}
	public class HtmlEditorToolbarInitializer : IOwnerEditingProperty {
		List<IOwnerEditingProperty> propertyOwners;
		object[] properties;
		HtmlEditorToolbarMode savedToolbarMode;
		string savedAssociatedRibbonID;
		public bool ToolbarModeChanged { get { return savedToolbarMode != HtmlEditor.ToolbarMode; } }
		public bool AssociatedRibbonIDChanged { get { return savedAssociatedRibbonID != HtmlEditor.AssociatedRibbonID; } }
		public List<IOwnerEditingProperty> PropertyOwners {
			get {
				if(propertyOwners == null)
					propertyOwners = new List<IOwnerEditingProperty>();
				return propertyOwners;
			}
		}
		public HtmlEditorToolbarInitializer(object component, IServiceProvider provider) {
			HtmlEditor = (ASPxHtmlEditor)component;
			ServiceProvider = provider;
			SaveUndo();
		}
		public ASPxHtmlEditor HtmlEditor { get; private set; }
		public IServiceProvider ServiceProvider { get; private set; }
		public object PropertyInstance {
			get {
				if(properties == null)
					properties = new object[] { HtmlEditor.ToolbarMode, HtmlEditor.Toolbars, HtmlEditor.RibbonTabs };
				return properties;
			}
		}
		public bool ItemsChanged {
			get {
				return ToolbarModeChanged || AssociatedRibbonIDChanged || PropertyOwners.Any(o => o.ItemsChanged);
			}
			set { }
		}
		public void SaveUndo() {
			savedToolbarMode = HtmlEditor.ToolbarMode;
			savedAssociatedRibbonID = HtmlEditor.AssociatedRibbonID;
		}
		public void UndoChanges() {
			if(ToolbarModeChanged)
				HtmlEditor.ToolbarMode = savedToolbarMode;
			if(AssociatedRibbonIDChanged)
				HtmlEditor.AssociatedRibbonID = savedAssociatedRibbonID;
			IterateByPropertyOwners(o => {
				if(o.ItemsChanged)
					o.UndoChanges();
			});
		}
		void IOwnerEditingProperty.SaveChanges() {
		}
		void IOwnerEditingProperty.BeforeClosed() {
		}
		void IterateByPropertyOwners(Action<IOwnerEditingProperty> action) {
			PropertyOwners.ForEach(action);
		}
	}
}
