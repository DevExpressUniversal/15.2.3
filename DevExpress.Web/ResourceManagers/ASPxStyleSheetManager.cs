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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum Suite {
		None,
		All,
		Charting,
		Editors,
		Gauges,
		Grid,
		HtmlEditor,
		NavigationAndLayout,
		PivotGrid,
		Reporting,
		Scheduling,
		SpellChecker,
		Spreadsheet,
		TreeList,
		RichEdit
	}
	[ToolboxItem(false)
]
	public abstract class ASPxResourceManagerBase : ASPxWebComponent {
		readonly string ManagerContextKey;
		ResourceItemCollection items = null;
		public ASPxResourceManagerBase()
			: base() {
			ManagerContextKey = ManagerType.Name.Replace("ASPx", "DX");
		}
		protected internal ResourceItemCollection ResourceItems {
			get {
				if(items == null)
					items = CreateItemCollection(this);
				return items;
			}
		}
		protected Type ManagerType { get { return this.GetType(); } }
		protected internal bool IsMultipleInstances() {
			return Page.Items[ManagerType] != this;
		}
		protected internal void Register() {
			SetActiveValue(true);
		}
		protected internal void Unregister() {
			SetActiveValue(false);
		}
		protected override bool IsScriptEnabled() {
			return false;
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return false;
		}
		protected virtual ResourceItemCollection CreateItemCollection(ASPxResourceManagerBase manager) {
			return null;
		}
		protected virtual void SetActiveValue(bool value) {
			Page.Items[ManagerType] = value && Page.Items[ManagerType] == null ? this : null;
			if(HttpContext.Current != null)
				HttpUtils.SetContextValue<bool>(ManagerContextKey, value);
		}
		protected override void OnInit(EventArgs e) {
			Register();
			if(IsMultipleInstances())
				throw new Exception("An " + ManagerType.Name + " instance already exists on the page.");
			base.OnInit(e);
		}
		protected internal static bool GetActiveValueFromContext(string contextKey) {
			return HttpUtils.GetContextValue<bool>(contextKey, false);
		}
	}
	[ToolboxItem(false)
]
	public class ASPxStyleSheetManager : ASPxResourceManagerBase {
		const string StyleSheetManagerContextKey = "DXStyleSheetManager";
		static bool active;
		public ASPxStyleSheetManager()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxStyleSheetManagerItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public StyleSheetCollection Items {
			get { return ResourceItems as StyleSheetCollection; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxStyleSheetManagerActive")]
#endif
		public static bool Active {
			get {
				return HttpContext.Current != null ? GetActiveValueFromContext(StyleSheetManagerContextKey) : active;
			}
		}
		protected override void RenderInternal(HtmlTextWriter writer) {
			if(!DesignMode)
				writer.Write(RenderCssLinkTags());
		}
		protected override ResourceItemCollection CreateItemCollection(ASPxResourceManagerBase manager) {
			return new StyleSheetCollection(manager as ASPxStyleSheetManager);
		}
		protected override void SetActiveValue(bool value) {
			base.SetActiveValue(value);
			if(Active != value)
				active = value;
		}
		protected internal string RenderCssLinkTags() {
			foreach(ResourceStyleSheet styleSheet in Items) {
				string dxTheme = styleSheet.Theme;
				string msTheme = GetMsTheme();
				IList<ASPxWebControl> controls = RegistrationControlsFactory.GetControlsForStyleSheetRegistration(styleSheet.Suite);
				foreach(var control in controls) {
					control.SkinID = styleSheet.SkinID;
					if(!string.IsNullOrEmpty(msTheme)) {
						using(DummyPage page = new DummyPage()) {
							page.ApplyTheme(msTheme, control);
						}
					}
					control.Theme = dxTheme;
					control.ApplyThemeInternal();
					control.RegisterStyleSheets();
				}
			}
			return RenderCssResources();
		}
		protected internal string GetMsTheme() {
			if(!String.IsNullOrEmpty(Page.Theme))
				return Page.Theme;
			try {
				var section = (PagesSection)WebConfigurationManager.GetSection("system.web/pages");
				if(section != null && !string.IsNullOrEmpty(section.Theme))
					return section.Theme;
			}
			catch(System.Security.SecurityException) { }
			return string.Empty;
		}
		protected internal string RenderCssResources() {
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			ResourceManager.RenderCssResources(Page, new HtmlTextWriter(sw));
			return sb.ToString();
		}
		internal class DummyPage : Page {
			public void ApplyTheme(string theme, Control control) {
				try {
					StyleSheetTheme = theme;
					FrameworkInitialize();
					control.ApplyStyleSheetSkin(this);
				} catch { }
			}
		}
	}
}
