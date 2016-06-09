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
using DevExpress.Web.Localization;
namespace DevExpress.Web.Mvc {
	public class LoadingPanelSettings: SettingsBase {
		public LoadingPanelSettings() {
			ContainerElementID = "";
			HorizontalOffset = 0;
			ImagePosition = ImagePosition.Left;
			Modal = false;
			ShowImage = true;
			Text = ASPxperienceLocalizer.GetString(ASPxperienceStringId.Loading);
			VerticalOffset = 0;
		}
		public ClientSideEvents ClientSideEvents { get { return (ClientSideEvents)ClientSideEventsInternal; } }
		public string ContainerElementID { get; set; }
		public new AppearanceStyle ControlStyle { get { return (AppearanceStyle)base.ControlStyle; } }
		public int HorizontalOffset { get; set; }
		public ImagesBase Images { get { return (ImagesBase)ImagesInternal; } }
		public ImagePosition ImagePosition { get; set; }
		public bool Modal { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowImage { get; set; }
		public LoadingPanelStyles Styles { get { return (LoadingPanelStyles)StylesInternal; } }
		public string Text { get; set; }
		public int VerticalOffset { get; set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		public EventHandler<ControlResolveEventArgs> ContainerElementResolve { get; set; }
		protected internal string TemplateContent { get; set; }
		protected internal Action<TemplateContainerBase> TemplateContentMethod { get; set; }
		public void SetTemplateContent(Action<TemplateContainerBase> contentMethod) {
			TemplateContentMethod = contentMethod;
		}
		public void SetTemplateContent(string content) {
			TemplateContent = content;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new ImagesBase(null);
		}
		protected override StylesBase CreateStyles() {
			return new LoadingPanelStyles(null);
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new AppearanceStyle();
		}
	}
}
