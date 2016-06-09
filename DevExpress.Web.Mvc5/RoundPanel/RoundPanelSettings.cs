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
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Utils;
namespace DevExpress.Web.Mvc {
	public class RoundPanelSettings : SettingsBase {
		public RoundPanelSettings() {
			Parts = new RoundPanelParts(null);
			ContentPaddings = new Paddings();
			HeaderImage = new ImageProperties();
			LoadingPanelImage = new ImageProperties();
			LoadingPanelStyle = new LoadingPanelStyle();
			SettingsLoadingPanel = new SettingsLoadingPanel(null);
			CollapseButtonImage = new CollapseButtonImageProperties();
			CollapseButtonStyle = new CollapseButtonStyle();
			ExpandButtonImage = new ImageProperties();
			ShowHeader = true;
			View = DevExpress.Web.View.Standard;
			EnableAnimation = true;
		}
		public object CallbackRouteValues { get; set; }
		public RoundPanelClientSideEvents ClientSideEvents { get { return (RoundPanelClientSideEvents)ClientSideEventsInternal; } }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public Paddings ContentPaddings { get; private set; }
		public Unit ContentHeight { get; set; }
		public ImageProperties HeaderImage { get; private set; }
		public bool EnableClientSideAPI { get { return EnableClientSideAPIInternal; } set { EnableClientSideAPIInternal = value; } }
		public string HeaderText { get; set; }
		public string HeaderNavigateUrl { get; set; }
		public Unit GroupBoxCaptionOffsetX { get; set; }
		public Unit GroupBoxCaptionOffsetY { get; set; }
		public Unit CornerRadius { get; set; }
		public RoundPanelParts Parts { get; private set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowHeader { get; set; }
		public RoundPanelStyles Styles { get { return (RoundPanelStyles)StylesInternal; } }
		public string Target { get; set; }
		public DevExpress.Web.View View { get; set; }
		public bool Collapsed { get; set; }
		public bool ShowCollapseButton { get; set; }
		public bool EnableAnimation { get; set; }
		public bool AllowCollapsingByHeaderClick { get; set; }
		public string DefaultButton { get; set; }
		public CollapseButtonImageProperties CollapseButtonImage { get; private set; }
		public ImageProperties ExpandButtonImage { get; private set; }
		public CollapseButtonStyle CollapseButtonStyle { get; private set; }
		public ImageProperties LoadingPanelImage { get; private set; }
		public LoadingPanelStyle LoadingPanelStyle { get; private set; }
		public SettingsLoadingPanel SettingsLoadingPanel { get; private set; }
		protected internal string Content { get; set; }
		protected internal Action ContentMethod { get; set; }
		public void SetContent(Action contentMethod) {
			ContentMethod = contentMethod;
		}
		public void SetContent(string content) {
			Content = content;
		}
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		protected internal string HeaderContentTemplateContent { get; set; }
		protected internal Action<RoundPanelHeaderContentTemplateContainer> HeaderContentTemplateContentMethod { get; set; }
		public void SetHeaderContentTemplateContent(Action<RoundPanelHeaderContentTemplateContainer> contentMethod) {
			HeaderContentTemplateContentMethod = contentMethod;
		}
		public void SetHeaderContentTemplateContent(string content) {
			HeaderContentTemplateContent = content;
		}
		protected internal string HeaderTemplateContent { get; set; }
		protected internal Action<RoundPanelHeaderTemplateContainer> HeaderTemplateContentMethod { get; set; }
		public void SetHeaderTemplateContent(Action<RoundPanelHeaderTemplateContainer> contentMethod) {
			HeaderTemplateContentMethod = contentMethod;
		}
		public void SetHeaderTemplateContent(string content) {
			HeaderTemplateContent = content;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new RoundPanelClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new RoundPanelImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new RoundPanelStyles(null);
		}
	}
}
