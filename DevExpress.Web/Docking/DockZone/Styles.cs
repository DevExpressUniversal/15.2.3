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
using System.Collections.Generic;
using System.Text;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	public class DockZoneStyles : StylesBase {
		ASPxDockZone zone = null;
		internal const string
			PanelPlaceholderStyleName = "PanelPlaceholder",
			DockingForbiddenStyleName = "DockingForbidden",
			DockingAllowedStyleName = "DockingAllowed",
			DisabledCssClassName = "dxdzDisabled",
			PanelPlaceholderCssClassName = "dxdz-pnlPlcHolder";
		readonly Dictionary<DockZoneOrientation, string> ControlClassNames = new Dictionary<DockZoneOrientation, string> {
			{ DockZoneOrientation.Vertical,  "dxdzControlVert" },
			{ DockZoneOrientation.Horizontal, "dxdzControlHor" },
			{ DockZoneOrientation.Fill, "dxdzControlFill" }
		};
		public DockZoneStyles(ASPxDockZone zone)
			: base(zone) {
			this.zone = zone;
		}
		protected ASPxDockZone Zone { get { return this.zone; } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(PanelPlaceholderStyleName, delegate() { return new AppearanceSelectedStyle(); }));
			list.Add(new StyleInfo(DockingAllowedStyleName, delegate() { return new AppearanceSelectedStyle(); }));
			list.Add(new StyleInfo(DockingForbiddenStyleName, delegate() { return new AppearanceSelectedStyle(); }));
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockZoneStylesDisabled"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DisabledStyle Disabled
		{
			get { return base.DisabledInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockZoneStylesPanelPlaceholder"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceSelectedStyle PanelPlaceholder
		{
			get { return (AppearanceSelectedStyle)GetStyle(PanelPlaceholderStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockZoneStylesDockingAllowedStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceSelectedStyle DockingAllowedStyle
		{
			get { return (AppearanceSelectedStyle)GetStyle(DockingAllowedStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockZoneStylesDockingForbiddenStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceSelectedStyle DockingForbiddenStyle
		{
			get { return (AppearanceSelectedStyle)GetStyle(DockingForbiddenStyleName); }
		}
		protected T GetDefaultStyle<T>(string cssClassName) where T :
		  AppearanceStyleBase, new() {
			T style = new T();
			style.CssClass = cssClassName;
			return style;			
		}
		protected internal override AppearanceStyle GetDefaultControlStyle() {
			return GetDefaultStyle<AppearanceStyle>(ControlClassNames[Zone.Orientation]);
		}
		protected internal override AppearanceStyleBase GetDefaultDisabledStyle() {
			return GetDefaultStyle<AppearanceStyleBase>(DisabledCssClassName);
		}
		protected internal virtual AppearanceSelectedStyle GetDefaultPanelPlaceholderStyle() {
			return GetDefaultStyle<AppearanceSelectedStyle>(PanelPlaceholderCssClassName);
		}
	}
}
