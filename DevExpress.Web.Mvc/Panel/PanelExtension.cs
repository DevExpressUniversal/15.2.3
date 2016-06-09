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

using System.Web.Mvc;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Mvc.Internal;
	public class PanelExtension : CollapsiblePanelExtension {
		public PanelExtension(PanelSettings settings)
			: base(settings) {
		}
		public PanelExtension(PanelSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxPanel Control {
			get { return (MVCxPanel)base.Control; }
		}
		protected internal new PanelSettings Settings {
			get { return (PanelSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			Control.Styles.CopyFrom(Settings.Styles);
			base.AssignInitialProperties();
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.Images.CopyFrom(Settings.Images);
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxPanel();
		}
	}
	public abstract class CollapsiblePanelExtension : ExtensionBase {
		public CollapsiblePanelExtension(CollapsiblePanelSettings settings)
			: base(settings) {
		}
		public CollapsiblePanelExtension(CollapsiblePanelSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new CollapsiblePanelSettings Settings {
			get { return (CollapsiblePanelSettings)base.Settings; }
		}
		protected internal new ASPxCollapsiblePanel Control {
			get { return (ASPxCollapsiblePanel)base.Control; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.EnableClientSideAPI = Settings.EnableClientSideAPI;
			Control.DefaultButton = Settings.DefaultButton;
			Control.ClientVisible = Settings.ClientVisible;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.RightToLeft = Settings.RightToLeft;
			Control.RenderMode = RenderMode.Div;
			Control.Collapsible = Settings.Collapsible;
			Control.FixedPosition = Settings.FixedPosition;
			Control.FixedPositionOverlap = Settings.FixedPositionOverlap;
			Control.SettingsCollapsing.Assign(Settings.SettingsCollapsing);
			Control.SettingsAdaptivity.Assign(Settings.SettingsAdaptivity);
			Control.ScrollBars = Settings.ScrollBars;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.Controls.Add(ContentControl.Create(Settings.Content, Settings.ContentMethod));
			Control.ExpandBarTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.ExpandBarTemplateContent, Settings.ExpandBarTemplateContentMethod, typeof(TemplateContainerBase));
			Control.ExpandButtonTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.ExpandButtonTemplateContent, Settings.ExpandButtonTemplateContentMethod, typeof(TemplateContainerBase));
			Control.ExpandedPanelTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.ExpandedPanelTemplateContent, Settings.ExpandedPanelTemplateContentMethod, typeof(TemplateContainerBase));
		}
	}
}
