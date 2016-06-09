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
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	public class ButtonExtension: ExtensionBase {
		public ButtonExtension(ButtonSettings settings)
			: base(settings) {
		}
		public ButtonExtension(ButtonSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxButton Control {
			get { return (MVCxButton)base.Control; }
		}
		protected internal new ButtonSettings Settings {
			get { return (ButtonSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AllowFocus = Settings.AllowFocus;
			Control.CausesValidation = Settings.CausesValidation;
			Control.Checked = Settings.Checked;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ClientVisible = Settings.ClientVisible;
			Control.ClientEnabled = Settings.ClientEnabled;
			Control.EnableClientSideAPI = Settings.EnableClientSideAPI;
			Control.GroupName = Settings.GroupName;
			Control.Images.CopyFrom(Settings.Images);
			Control.ImagePosition = Settings.ImagePosition;
			Control.RenderMode = Settings.RenderMode;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.Text = Settings.Text;
			Control.ValidationGroup = Settings.ValidationGroup;
			Control.ValidationContainerID = Settings.ValidationContainerID;
			Control.ValidateInvisibleEditors = Settings.ValidateInvisibleEditors;
			Control.UseSubmitBehavior = Settings.UseSubmitBehavior;
			Control.AutoPostBack = Settings.UseSubmitBehavior;
			Control.RouteValues = Settings.RouteValues;
			Control.RightToLeft = Settings.RightToLeft;
			Control.CustomJSProperties += Settings.CustomJSProperties;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxButton();
		}
	}
}
