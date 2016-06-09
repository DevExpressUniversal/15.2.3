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
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web;
	public class RoundPanelExtension : ExtensionBase {
		public RoundPanelExtension(RoundPanelSettings settings)
			: base(settings) {
		}
		public RoundPanelExtension(RoundPanelSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxRoundPanel Control {
			get { return (MVCxRoundPanel)base.Control; }
		}
		protected internal new RoundPanelSettings Settings {
			get { return (RoundPanelSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ClientVisible = Settings.ClientVisible;
			Control.ContentPaddings.Assign(Settings.ContentPaddings);
			Control.ContentHeight = Settings.ContentHeight;
			Control.HeaderImage.Assign(Settings.HeaderImage);
			Control.EnableClientSideAPI = Settings.EnableClientSideAPI;
			Control.HeaderText = Settings.HeaderText;
			Control.HeaderNavigateUrl = Settings.HeaderNavigateUrl;
			Control.GroupBoxCaptionOffsetX = Settings.GroupBoxCaptionOffsetX;
			Control.GroupBoxCaptionOffsetY = Settings.GroupBoxCaptionOffsetY;
			Control.ShowHeader = Settings.ShowHeader;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.Target = Settings.Target;
			Control.View = Settings.View;
			Control.Collapsed = Settings.Collapsed;
			Control.ShowCollapseButton = Settings.ShowCollapseButton;
			Control.EnableAnimation = Settings.EnableAnimation;
			Control.AllowCollapsingByHeaderClick = Settings.AllowCollapsingByHeaderClick;
			Control.CornerRadius = Settings.CornerRadius;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.DefaultButton = Settings.DefaultButton;
			Control.LoadingPanelImage.CopyFrom(Settings.LoadingPanelImage);
			Control.LoadingPanelStyle.CopyFrom(Settings.LoadingPanelStyle);
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.CollapseButtonImage.CopyFrom(Settings.CollapseButtonImage);
			Control.ExpandButtonImage.CopyFrom(Settings.ExpandButtonImage);
			Control.CollapseButtonStyle.CopyFrom(Settings.CollapseButtonStyle);
			Control.RightToLeft = Settings.RightToLeft;
			Control.Parts.Assign(Settings.Parts);
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.Controls.Add(DevExpress.Web.Mvc.Internal.ContentControl.Create(Settings.Content, Settings.ContentMethod));
			Control.HeaderTemplate = ContentControlTemplate<RoundPanelHeaderTemplateContainer>.Create(
				Settings.HeaderTemplateContent, Settings.HeaderTemplateContentMethod,
				typeof(RoundPanelHeaderTemplateContainer));
			Control.HeaderContentTemplate = ContentControlTemplate<RoundPanelHeaderContentTemplateContainer>.Create(
				Settings.HeaderContentTemplateContent, Settings.HeaderContentTemplateContentMethod,
				typeof(RoundPanelHeaderContentTemplateContainer));
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxRoundPanel();
		}
		protected override bool IsSimpleIDsRenderModeSupported() {
			return Settings.ClientSideEvents.IsEmpty() && !Settings.EnableClientSideAPI && Settings.ClientVisible;
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
	}
}
