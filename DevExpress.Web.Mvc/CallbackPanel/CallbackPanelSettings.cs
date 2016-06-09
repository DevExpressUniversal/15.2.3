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
using DevExpress.Utils;
namespace DevExpress.Web.Mvc {
	public class CallbackPanelSettings: CollapsiblePanelSettings {
		SettingsLoadingPanel settingsLoadingPanel;
		public CallbackPanelSettings() {
			this.settingsLoadingPanel = new SettingsLoadingPanel(null);
		}
		public object CallbackRouteValues { get; set; }
		public CallbackPanelClientSideEvents ClientSideEvents { get { return (CallbackPanelClientSideEvents)ClientSideEventsInternal; } }
		[Obsolete("This property is now obsolete. Use the EnableCallbackAnimation property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool EnableAnimation { get { return EnableCallbackAnimation; } set { EnableCallbackAnimation = value; } }
		public bool EnableCallbackAnimation { get; set; }
		public bool HideContentOnCallback { get; set; }
		public ImagesBase Images { get { return (ImagesBase)ImagesInternal; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int LoadingPanelDelay { get { return SettingsLoadingPanel.Delay; } set { SettingsLoadingPanel.Delay = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImagePosition LoadingPanelImagePosition { get { return SettingsLoadingPanel.ImagePosition; } set { SettingsLoadingPanel.ImagePosition = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string LoadingPanelText { get { return SettingsLoadingPanel.Text; } set { SettingsLoadingPanel.Text = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowLoadingPanel { get { return SettingsLoadingPanel.Enabled; } set { SettingsLoadingPanel.Enabled = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowLoadingPanelImage { get { return SettingsLoadingPanel.ShowImage; } set { SettingsLoadingPanel.ShowImage = value; } }
		public SettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public CallbackPanelStyles Styles { get { return (CallbackPanelStyles)StylesInternal; } }
		public EventHandler BeforeGetCallbackResult { get { return BeforeGetCallbackResultInternal; } set { BeforeGetCallbackResultInternal = value; } }
		[Obsolete("Use the CallbackPanelSettings.CallbackRouteValues property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public CallbackEventHandlerBase Callback {
			get { return CallbackInternal; }
			set { CallbackInternal = value; } 
		}
		protected internal CallbackEventHandlerBase CallbackInternal { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new CallbackPanelClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new ImagesBase(null);
		}
		protected override StylesBase CreateStyles() {
			return new CallbackPanelStyles(null);
		}
	}
}
