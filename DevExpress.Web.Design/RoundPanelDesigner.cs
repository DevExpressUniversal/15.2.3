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

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Design {
	public class ASPxRoundPanelDesigner : DevExpress.Web.Design.ASPxPanelBaseDesigner {
		protected ASPxRoundPanel RoundPanel { get; private set; }
		protected ASPxRoundPanel RoundPanelViewControl {
			get {
				return ViewControl as ASPxRoundPanel;
			}
		}
		protected override void AddDesignerRegions(DesignerRegionCollection regions) {
			base.AddDesignerRegions(regions);
			AddHeaderSelectableRegion("Collapse", regions);
		}
		protected void AddHeaderSelectableRegion(string name, DesignerRegionCollection regions) {
			regions.Add(new RoundPanelControlClickableRegion(this, name, RoundPanel));
			if(RoundPanelViewControl != null) {
				WebControl buttonElement = RoundPanelViewControl.GetCollapseButtonControl();
				if(buttonElement != null)
					buttonElement.Attributes[DesignerRegion.DesignerRegionAttributeName] = (regions.Count - 1).ToString();
			}
		}
		public override void Initialize(IComponent component) {
			RoundPanel = (ASPxRoundPanel)component;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override void OnClick(DesignerRegionMouseEventArgs e) {
			base.OnClick(e);
			RoundPanelControlClickableRegion region = e.Region as RoundPanelControlClickableRegion;
			if(region != null) {
				RoundPanel.Collapsed = !region.Panel.Collapsed;
				PropertyChanged("Collapsed");
			}
		}
		protected internal object GetProject() {
			return ((EnvDTE.ProjectItem)GetService(typeof(EnvDTE.ProjectItem))).ContainingProject;
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			templateGroups.Add(CreateTemplateGroup("Header Template", "HeaderTemplate", RoundPanel.GetHeaderStyle()));
			templateGroups.Add(CreateTemplateGroup("Header Content Template", "HeaderContentTemplate", RoundPanel.GetHeaderStyle()));
			return templateGroups;
		}
		private TemplateGroup CreateTemplateGroup(string name, string propertyName, DevExpress.Web.AppearanceStyle style) {
			return CreateTemplateGroup(name, name, propertyName, style);
		}
		private TemplateGroup CreateTemplateGroup(string name, string groupName, string propertyName, DevExpress.Web.AppearanceStyle style) {
			TemplateGroup templateGroup = new TemplateGroup(groupName);
			TemplateDefinition templateDefinition = new TemplateDefinition(this, name,
				Component, propertyName, style);
			templateGroup.AddTemplateDefinition(templateDefinition);
			return templateGroup;
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new RoundPanelDesignerActionsList(this);
		}
		public bool ShowHeader {
			get {
				return RoundPanel.ShowHeader;
			}
			set {
				RoundPanel.ShowHeader = value;
				PropertyChanged("ShowHeader");
			}
		}
		public bool ShowCollapseButton {
			get {
				return RoundPanel.ShowCollapseButton;
			}
			set {
				RoundPanel.ShowCollapseButton = value;
				PropertyChanged("ShowCollapseButton");
			}
		}
		public bool AllowCollapsingByHeaderClick {
			get {
				return RoundPanel.AllowCollapsingByHeaderClick;
			}
			set {
				RoundPanel.AllowCollapsingByHeaderClick = value;
				PropertyChanged("AllowCollapsingByHeaderClick");
			}
		}
		public View View {
			get {
				return RoundPanel.View;
			}
			set {
				RoundPanel.View = value;
				PropertyChanged("View");
			}
		}
		public bool LoadContentViaCallback {
			get {
				return RoundPanel.LoadContentViaCallback;
			}
			set {
				RoundPanel.LoadContentViaCallback = value;
				PropertyChanged("LoadContentViaCallback");
			}
		}
	}
	public class RoundPanelDesignerActionsList : ASPxWebControlDesignerActionList {
		private ASPxRoundPanelDesigner fDesigner;
		public RoundPanelDesignerActionsList(ASPxRoundPanelDesigner designer)
			: base(designer) {
			fDesigner = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Insert(0, new DesignerActionPropertyItem("ShowHeader", "Show Header", "Header Appereance"));
			collection.Insert(1, new DesignerActionPropertyItem("ShowCollapseButton", "Show Collapse Button", "Header Appereance"));
			collection.Insert(2, new DesignerActionPropertyItem("AllowCollapsingByHeaderClick", "Allow Collapsing By Header Click", "Collapsing"));
			collection.Insert(3, new DesignerActionPropertyItem("LoadContentViaCallback", "Load Content Via Callback", "Collapsing"));
			collection.Insert(4, new DesignerActionPropertyItem("View", "View", "Panel Appereance"));
			return collection;
		}
		public bool ShowHeader {
			get { return fDesigner.ShowHeader; }
			set { fDesigner.ShowHeader = value; }
		}
		public bool ShowCollapseButton {
			get { return fDesigner.ShowCollapseButton; }
			set { fDesigner.ShowCollapseButton = value; }
		}
		public bool AllowCollapsingByHeaderClick {
			get { return fDesigner.AllowCollapsingByHeaderClick; }
			set { fDesigner.AllowCollapsingByHeaderClick = value; }
		}
		public View View {
			get { return fDesigner.View; }
			set { fDesigner.View = value; }
		}
		public bool LoadContentViaCallback {
			get { return fDesigner.LoadContentViaCallback; }
			set { fDesigner.LoadContentViaCallback = value; }
		}
	}
	public class RoundPanelControlClickableRegion : DesignerRegion {
		public ASPxRoundPanel Panel { get; private set; }
		public RoundPanelControlClickableRegion(ASPxRoundPanelDesigner designer, string name, ASPxRoundPanel panel)
			: base(designer, name, true) {
			Panel = panel;
		}
	}
}
