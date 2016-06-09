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
	public class ButtonSettings : SettingsBase {
		public ButtonSettings() {
			AllowFocus = true;
			CausesValidation = true;
			ClientVisible = true;
			ClientEnabled = true;
			ImagePosition = ImagePosition.Left;
			RenderMode = ButtonRenderMode.Button;
			Text = "Button";
		}
		public bool AllowFocus { get; set; }
		public bool CausesValidation { get; set; }
		public bool Checked { get; set; }
		public bool ClientEnabled { get; set; }
		public ButtonClientSideEvents ClientSideEvents { get { return (ButtonClientSideEvents)ClientSideEventsInternal; } }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public bool EnableClientSideAPI { get { return EnableClientSideAPIInternal; } set { EnableClientSideAPIInternal = value; } }
		public string GroupName { get; set; }
		public ButtonImages Images { get { return (ButtonImages)ImagesInternal; } }
		public ImagePosition ImagePosition { get; set; }
		public ButtonRenderMode RenderMode { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public ButtonControlStyles Styles { get { return (ButtonControlStyles)StylesInternal; } }
		public string Text { get; set; }
		public string ValidationGroup { get; set; }
		public string ValidationContainerID { get; set; }
		public bool ValidateInvisibleEditors { get; set; }
		public bool UseSubmitBehavior { get; set; }
		public object RouteValues { get; set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ButtonClientSideEvents();
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new ButtonControlStyle();
		}
		protected override ImagesBase CreateImages() {
			return new ButtonImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new ButtonControlStyles(null);
		}
	}
}
