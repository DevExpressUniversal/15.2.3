#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.ComponentModel;
using DevExpress.Web;
using DevExpress.XtraReports.Web.Native.ClientControls;
namespace DevExpress.XtraReports.Web.ClientControls {
	public class ClientControlsMenuItem : CollectionItem {
		const string
			TextName = "Text",
			ImageClassNameName = "ImageClassName",
			DisabledName = "Disabled",
			VisibleName = "Visible",
			HotKeyName = "HotKey",
			HasSeparatorName = "HasSeparator",
			JSClickActionName = "JSClickAction",
			ContainerName = "Container";
		const MenuItemContainer DefaultMenuItemContainer = MenuItemContainer.Menu;
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		public string Text {
			get { return GetStringProperty(TextName, ""); }
			set { SetStringProperty(TextName, "", value); }
		}
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		public string ImageClassName {
			get { return GetStringProperty(ImageClassNameName, ""); }
			set { SetStringProperty(ImageClassNameName, "", value); }
		}
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		public string JSClickAction {
			get { return GetStringProperty(JSClickActionName, ""); }
			set { SetStringProperty(JSClickActionName, "", value); }
		}
		[DefaultValue(false)]
		[NotifyParentProperty(true)]
		public bool Disabled {
			get { return GetBoolProperty(DisabledName, false); }
			set { SetBoolProperty(DisabledName, false, value); }
		}
		[DefaultValue(true)]
		[NotifyParentProperty(true)]
		public bool Visible {
			get { return GetBoolProperty(VisibleName, true); }
			set { SetBoolProperty(VisibleName, true, value); }
		}
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		public string HotKey {
			get { return GetStringProperty(HotKeyName, ""); }
			set { SetStringProperty(HotKeyName, "", value); }
		}
		[DefaultValue(false)]
		[NotifyParentProperty(true)]
		public bool HasSeparator {
			get { return GetBoolProperty(HasSeparatorName, false); }
			set { SetBoolProperty(HasSeparatorName, false, value); }
		}
		[DefaultValue(DefaultMenuItemContainer)]
		[NotifyParentProperty(true)]
		public MenuItemContainer Container {
			get { return (MenuItemContainer)GetEnumProperty(ContainerName, DefaultMenuItemContainer); }
			set { SetEnumProperty(ContainerName, DefaultMenuItemContainer, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ClientControlsMenuItemModel ToModel() {
			return new ClientControlsMenuItemModel {
				Text = Text,
				ImageClassName = ImageClassName,
				JSClickAction = JSClickAction,
				Disabled = Disabled,
				Visible = Visible,
				HotKey = HotKey,
				HasSeparator = HasSeparator,
				Container = Container
			};
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var src = source as ClientControlsMenuItem;
			if(src == null) {
				return;
			}
			Text = src.Text;
			ImageClassName = src.ImageClassName;
			JSClickAction = src.JSClickAction;
			Disabled = src.Disabled;
			Visible = src.Visible;
			HotKey = src.HotKey;
			HasSeparator = src.HasSeparator;
			Container = src.Container;
		}
	}
}
