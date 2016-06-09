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
using DevExpress.Utils;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Mvc {
	public class PanelSettings : CollapsiblePanelSettings {
		public PanelClientSideEvents ClientSideEvents { get { return (PanelClientSideEvents)ClientSideEventsInternal; } }
		public PanelStyles Styles { get { return (PanelStyles)StylesInternal; } }
		public PanelImages Images { get { return (PanelImages)ImagesInternal; } }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new PanelClientSideEvents();
		}
		protected override StylesBase CreateStyles() {
			return new PanelStyles(null);
		}
		protected override ImagesBase CreateImages() {
			return new PanelImages(null);
		}
	}
	public abstract class CollapsiblePanelSettings : SettingsBase {
		public CollapsiblePanelSettings() {
			SettingsCollapsing = new PanelCollapsingSettings(null);
			SettingsAdaptivity = new PanelAdaptivitySettings(null);
		}
		public bool EnableClientSideAPI { get { return EnableClientSideAPIInternal; } set { EnableClientSideAPIInternal = value; } }
		public string DefaultButton { get; set; }		
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool Collapsible { get; set; }
		public PanelFixedPosition FixedPosition { get; set; }
		public bool FixedPositionOverlap { get; set; }
		public PanelCollapsingSettings SettingsCollapsing { get; private set; }
		public PanelAdaptivitySettings SettingsAdaptivity { get; private set; }
		public ScrollBars ScrollBars { get; set; }
		protected internal string Content { get; set; }
		protected internal Action ContentMethod { get; set; }
		public void SetContent(Action contentMethod) {
			ContentMethod = contentMethod;
		}
		public void SetContent(string content) {
			Content = content;
		}
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		protected internal string ExpandBarTemplateContent { get; set; }
		protected internal Action<TemplateContainerBase> ExpandBarTemplateContentMethod { get; set; }
		protected internal string ExpandButtonTemplateContent { get; set; }
		protected internal Action<TemplateContainerBase> ExpandButtonTemplateContentMethod { get; set; }
		protected internal string ExpandedPanelTemplateContent { get; set; }
		protected internal Action<TemplateContainerBase> ExpandedPanelTemplateContentMethod { get; set; }
		public void SetExpandBarTemplateContent(Action<TemplateContainerBase> contentMethod) {
			ExpandBarTemplateContentMethod = contentMethod;
		}
		public void SetExpandBarTemplateContent(string content) {
			ExpandBarTemplateContent = content;
		}
		public void SetExpandButtonTemplateContent(Action<TemplateContainerBase> contentMethod) {
			ExpandButtonTemplateContentMethod = contentMethod;
		}
		public void SetExpandButtonTemplateContent(string content) {
			ExpandButtonTemplateContent = content;
		}
		public void SetExpandedPanelTemplateContent(Action<TemplateContainerBase> contentMethod) {
			ExpandedPanelTemplateContentMethod = contentMethod;
		}
		public void SetExpandedPanelTemplateContent(string content) {
			ExpandedPanelTemplateContent = content;
		}
	}
}
