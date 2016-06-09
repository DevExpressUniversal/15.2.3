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
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Design;
namespace DevExpress.Web.Design {
	using DevExpress.Web;
	using DevExpress.Utils.About;
	public class ASPxPanelBaseDesigner : ASPxWebControlDesigner {
		private ASPxPanelContainerBase panelBase = null;
		public ASPxPanelContainerBase PanelBase {
			get {
				return panelBase;
			}
		}
		public override void Initialize(IComponent component) {
			this.panelBase = (ASPxPanelContainerBase)component;
			base.Initialize(component);
		}
		public override string GetEditableDesignerRegionContent(EditableDesignerRegion region) {
			PanelBaseEditableRegion panelBaseRegion = region as PanelBaseEditableRegion;
			if(panelBaseRegion == null)
				throw new InvalidCastException(StringResources.InvalidRegion);
			return GetEditableDesignerRegionContent(panelBaseRegion.PanelBase.Controls);
		}
		public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content) {
			PanelBaseEditableRegion panelBaseRegion = region as PanelBaseEditableRegion;
			if(panelBaseRegion == null)
				throw new InvalidCastException(StringResources.InvalidRegion);
			SetEditableDesignerRegionContent(panelBaseRegion.PanelBase.Controls, content);
		}
		protected override void AddDesignerRegions(DesignerRegionCollection regions) {
			regions.Add(new PanelBaseEditableRegion(this, "PSingleRegion", PanelBase));
			((ASPxPanelContainerBase)ViewControl).PanelContent.DesignerRegionAttribute = "0";
		}
	}
	public class PanelBaseEditableRegion : EditableDesignerRegion {
		private ASPxPanelContainerBase fPanelBase = null;
		public PanelBaseEditableRegion(ASPxPanelBaseDesigner designer, string name, ASPxPanelContainerBase panel)
			: base(designer, name, false) {
			fPanelBase = panel;
		}
		public ASPxPanelContainerBase PanelBase {
			get { return fPanelBase; }
		}
	}
	public class ASPxCollapsiblePanelDesigner : ASPxPanelBaseDesigner {
		private ASPxCollapsiblePanel panel = null;
		public ASPxCollapsiblePanel Panel {
			get { return panel; }
		}
		public override void Initialize(IComponent component) {
			this.panel = (ASPxCollapsiblePanel)component;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		public override void ShowAbout() {
			PanelAboutDialogHelper.ShowAbout(Component.Site);
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			TemplateGroup templateGroup = new TemplateGroup("Control Template");
			TemplateDefinition templateDefinition = new TemplateDefinition(this, "ExpandBar Template",
				Panel, "ExpandBarTemplate", Panel.GetExpandBarStyle());
			templateDefinition.SupportsDataBinding = true;
			templateGroup.AddTemplateDefinition(templateDefinition);
			templateDefinition = new TemplateDefinition(this, "ExpandButton Template",
				Panel, "ExpandButtonTemplate", Panel.GetExpandButtonStyle());
			templateDefinition.SupportsDataBinding = true;
			templateGroup.AddTemplateDefinition(templateDefinition);
			templateDefinition = new TemplateDefinition(this, "Expanded Template",
				Panel, "ExpandedPanelTemplate", Panel.GetExpandedPanelStyle());
			templateDefinition.SupportsDataBinding = true;
			templateGroup.AddTemplateDefinition(templateDefinition);
			templateGroups.Add(templateGroup);
			return templateGroups;
		}
	}
	public class ASPxPanelDesigner : ASPxCollapsiblePanelDesigner {
		public override void Initialize(IComponent component) {
 			base.Initialize(component);
		}
	}
	public class PanelAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxCollapsiblePanel), new ProductKind[] { ProductKind.FreeOffer, ProductKind.DXperienceASP });
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(ASPxCollapsiblePanel)))
				ShowAbout(provider);
		}
	}
}
