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

using DevExpress.Utils;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Mvc {
	public class FormLayoutSettings<ModelType> : SettingsBase {
		public FormLayoutSettings() {
			AlignItemCaptions = true;
			Items = new MVCxFormLayoutItemCollection<ModelType>();
			ColCount = 1;
			SettingsItems = new LayoutGroupItemSettings(null);
			SettingsItemCaptions = new LayoutItemCaptionSettings(null);
			SettingsItemHelpTexts = new LayoutItemHelpTextSettings(null);
			ShowItemCaptionColon = true;
			RequiredMarkDisplayMode = RequiredMarkMode.RequiredOnly;
			RequiredMark = "*";
			OptionalMark = "(optional)";
			RightToLeft = DefaultBoolean.Default;
			NestedExtensionWidth = Unit.Empty;
			UseDefaultPaddings = true;
			SettingsAdaptivity = new FormLayoutAdaptivitySettings(null);
		}
		public bool AlignItemCaptions { get; set; }
		public bool AlignItemCaptionsInAllGroups { get; set; }
		public int LeftAndRightCaptionsWidth { get; set; }
		public MVCxFormLayoutItemCollection<ModelType> Items { get; private set; }
		public int ColCount { get; set; }
		public LayoutGroupItemSettings SettingsItems { get; private set; }
		public LayoutItemCaptionSettings SettingsItemCaptions { get; private set; }
		public LayoutItemHelpTextSettings SettingsItemHelpTexts { get; private set; }
		public bool ShowItemCaptionColon { get; set; }
		public RequiredMarkMode RequiredMarkDisplayMode { get; set; }
		public string RequiredMark { get; set; }
		public string OptionalMark { get; set; }
		public DefaultBoolean RightToLeft { get; set; }
		public Unit NestedExtensionWidth { get; set; }
		public bool UseDefaultPaddings { get; set; }
		public FormLayoutAdaptivitySettings SettingsAdaptivity { get; private set; }
		public ClientSideEvents ClientSideEvents { get { return (ClientSideEvents)ClientSideEventsInternal; } }
		public FormLayoutStyles Styles { get { return (FormLayoutStyles)StylesInternal; } }
		public LayoutItemDataBindingEventHandler LayoutItemDataBinding { get; set; }
		public LayoutItemDataBoundEventHandler LayoutItemDataBound { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return null;
		}
		protected override StylesBase CreateStyles() {
			return new FormLayoutStyles(null);
		}
	}
}
