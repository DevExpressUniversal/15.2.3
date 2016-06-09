#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Web.UI;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using System.Web;
using System.Web.Hosting;
using System.IO;
using DevExpress.Web.Internal;
using System.ComponentModel;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Layout;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class CookieCurrentThemeManager : ICurrentThemeManager {
		public string GetCurrentTheme() {
			if(TemplateContentFactory.Instance.NewStyle) {
				return "XafTheme";
			}
			else {
				return new SettingsStorageOnCookies().LoadOption("", "Theme");
			}
		}
		public void SetCurrentTheme(string name) {
			new SettingsStorageOnCookies().SaveOption("", "Theme", name);
		}
	}
	public class ChooseThemeController : WindowController {
		public const string ActiveKey_CanManageCurrentTheme = "CanManageCurrentTheme";
		public const string NewStyleDisableReason = "NewTemplateStyle";
		private readonly object lockObject = new object();
		static ChooseThemeController() {
			BaseXafPage.CurrentThemeManager = new CookieCurrentThemeManager();
		}
		private SingleChoiceAction chooseThemeAction;
		private static bool useASPThemesMechanizmCalculated;
		private static bool useASPThemesMechanizm;
		internal static bool UseASPThemesMechanizm {
			get {
				if(!useASPThemesMechanizmCalculated) {
					useASPThemesMechanizm = HasThemesInSite();
					useASPThemesMechanizmCalculated = true;
				}
				return useASPThemesMechanizm;
			}
		}
		private static bool HasThemesInSite() {
			if(HttpContext.Current != null) {
				string themesPath = HttpContext.Current.Server.MapPath("~/App_Themes");
				if(Directory.Exists(themesPath)) {
					return Directory.GetDirectories(themesPath).Length > 0;
				}
			}
			return false;
		}
		private void chooseThemeAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			CookieCurrentThemeManager m = BaseXafPage.CurrentThemeManager as CookieCurrentThemeManager;
			if(m != null) {
				m.SetCurrentTheme(e.SelectedChoiceActionItem.Data.ToString());
			}
			WebApplication.Redirect(HttpContext.Current.Request.RawUrl, false);
		}
		protected virtual IEnumerable<string> GetThemesList() {
			List<string> result = new List<string>();
			if(UseASPThemesMechanizm) {
				if(HttpContext.Current != null) {
					string themesPath = HttpContext.Current.Server.MapPath("~/App_Themes");
					if(Directory.Exists(themesPath)) {
						string[] themes = Directory.GetDirectories(themesPath);
						foreach(string themeDirectory in themes) {
							result.Add(Path.GetFileName(themeDirectory));
						}
					}
				}
			}
			else {
				lock(lockObject) {
					result.AddRange(ThemesProvider.GetThemes()); 
					result.Remove("iOS"); 
					result.Remove("Metropolis"); 
					result.Remove("MetropolisBlue"); 
					result.Remove("Moderno"); 
					result.Remove("Mulberry"); 
					result.Remove("XafTheme"); 
				}
			}
			return result;
		}
		protected void FillActionItems() {
			chooseThemeAction.BeginUpdate();
			try {
				chooseThemeAction.Items.Clear();
				foreach(string theme in GetThemesList()) {
					ChoiceActionItem item = new ChoiceActionItem(CaptionHelper.ConvertCompoundName(theme), theme);
					chooseThemeAction.Items.Add(item);
				}
				chooseThemeAction.Active["HasSeveralThemes"] = chooseThemeAction.Items.Count > 1;
			}
			finally {
				chooseThemeAction.EndUpdate();
			}
		}
		public ChooseThemeController() {
			chooseThemeAction = new SingleChoiceAction(this, "ChooseTheme", PredefinedCategory.Appearance);
			chooseThemeAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			chooseThemeAction.ShowItemsOnClick = true;
			chooseThemeAction.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Image;
			chooseThemeAction.ImageName = "MenuBar_ChooseSkin";
			chooseThemeAction.Execute += new SingleChoiceActionExecuteEventHandler(chooseThemeAction_Execute);
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(TemplateContentFactory.Instance.NewStyle) {
				Active[NewStyleDisableReason] = false;
			}
			else {
				if(BaseXafPage.CurrentThemeManager is CookieCurrentThemeManager) {
					FillActionItems();
				}
				else {
					chooseThemeAction.Active[ActiveKey_CanManageCurrentTheme] = false;
				}
			}
		}
		public SingleChoiceAction ChooseThemeAction {
			get { return chooseThemeAction; }
		}
	}
	public class RegisterThemeAssemblyController : WindowController {
		public static bool EnableXafThemeAssembly = true;
		private static object lockObject = new object();
		private static List<string> cssFilePaths = new List<string>(new string[] {
			"App_Themes/{0}/Xaf/styles.css",
			"App_Themes/{0}/Xaf/styles_xaf.css",
			"App_Themes/{0}/Xaf/template.css",
			"App_Themes/{0}/Xaf/template_xaf.css"
		});
		private static List<string> newCssFilePaths = new List<string>(new string[] {
			"App_Themes/{0}/Xaf/Layout.css",
			"App_Themes/{0}/Xaf/NewStyle.css",
			"App_Themes/{0}/Xaf/Phone.css",
			"App_Themes/{0}/Xaf/Tablet.css",
			"App_Themes/{0}/Xaf/NotificationsPopupWindowCustomization.css",
		});
		private static List<string> GetCssFilePaths() {
			return WebApplicationStyleManager.IsNewStyle ? newCssFilePaths : cssFilePaths;
		}
		private void RegisterThemeAssemblyController_Load(object sender, EventArgs e) {
			RegisterThemeResources(sender as Page);
		}
		private void Frame_TemplateChanging(object sender, EventArgs e) {
			if(Frame.Template != null) {
				((Page)Frame.Template).Load -= new EventHandler(RegisterThemeAssemblyController_Load);
			}
		}
		private void Frame_TemplateChanged(object sender, EventArgs e) {
			if(Frame.Template != null) {
				Frame.TemplateChanging += new EventHandler(Frame_TemplateChanging);
				((Page)Frame.Template).Load += new EventHandler(RegisterThemeAssemblyController_Load);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Frame.Template != null) {
				Frame.TemplateChanging += new EventHandler(Frame_TemplateChanging);
				((Page)Frame.Template).Load += new EventHandler(RegisterThemeAssemblyController_Load);
			}
			else {
				Frame.TemplateChanged += new EventHandler(Frame_TemplateChanged);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			Frame.TemplateChanging -= new EventHandler(Frame_TemplateChanging);
			Frame.TemplateChanged -= new EventHandler(Frame_TemplateChanged);
		}
		public static void RegisterThemeResources(Page page) {
			string themeName = BaseXafPage.CurrentTheme;
			if(ChooseThemeController.UseASPThemesMechanizm) {
				themeName = page != null ? page.Theme : string.Empty;
			}
			lock(lockObject) {
				if(!string.IsNullOrEmpty(themeName) && EnableXafThemeAssembly) {
					foreach(string cssPathTemplate in GetCssFilePaths()) {
						string cssFilePath = string.Format(cssPathTemplate, themeName);
						ResourceManager.RegisterCssResource(page, cssFilePath);
					}
					ResourceManager.RenderCssResourcesInHeader(page);
				}
			}
		}
		public static void RegisterCss(string path) {
			lock(lockObject) {
				GetCssFilePaths().Add(path);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void UnregisterCss(string path) {
			lock(lockObject) {
				GetCssFilePaths().Remove(path);
			}
		}
	}
}
