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
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.Web.WebClientUIControl.Internal;
namespace DevExpress.Web {
	public abstract class ASPxWebClientUIControl : ASPxWebControl {
		#region registrations
		protected static void RegisterGlobalizeScript(Page page) {
			RegisterGlobalizeScript(page, false);
		}
		protected static void RegisterGlobalizeScript(Page page, bool alwaysRegisterGlobalizeScript) {
			if(alwaysRegisterGlobalizeScript || ConfigurationSettings.EmbedRequiredClientLibraries || GlobalEmbedRequiredClientLibraries)
				ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.GlobalizeScriptResourceName);
		}
		protected static void RegisterGlobalizeCulturesScript(Page page) {
			RegisterGlobalizeCulturesScript(page, false);
		}
		protected static void RegisterGlobalizeCulturesScript(Page page, bool alwaysRegisterGlobalizeCulturesScript) {
			if(alwaysRegisterGlobalizeCulturesScript || ConfigurationSettings.EmbedRequiredClientLibraries || GlobalEmbedRequiredClientLibraries)
				ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.GlobalizeCulturesScriptResourceName);
		}
		protected static void RegisterKnockoutScript(Page page) {
			if(ConfigurationSettings.EmbedRequiredClientLibraries || GlobalEmbedRequiredClientLibraries)
				ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.KnockoutScriptResourceName);
		}
		protected static void RegisterDevExtremeBaseScript(Page page) {
			ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.DevExtremeCoreScriptResourceName);
			ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.DevExtremeWidgetsBaseScriptResourceName);
		}
		protected static void RegisterDevExtremeWebWidgetsScript(Page page) {
			ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.DevExtremeWidgetsWebScriptResourceName);
		}
		protected static void RegisterDevExtremeVizWidgetsScript(Page page) {
			ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.DevExtremeVizCoreScriptResourceName);
			ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.DevExtremeVizChartsScriptResourceName);
			ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.DevExtremeVizGaugesScriptResourceName);
			ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.DevExtremeVizRangeSelectorScriptResourceName);
			ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.DevExtremeVizSparklinesScriptResourceName);
			ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.DevExtremeVizVectorMapScriptResourceName);
		}
		protected static void RegisterDevExtremeCss(Page page, string themeName = DefaultColorScheme) {
			ResourceManager.RegisterCssResource(page, typeof(RenderUtils), RenderUtils.DevExtremeCommonCssResourceName);
			ResourceManager.RegisterCssResource(page, typeof(RenderUtils), string.Format(RenderUtils.DevExtremeThemeCssResourceFormat, themeName));
			ResourceManager.RegisterCssResource(page, typeof(RenderUtils), RenderUtils.DevExtremeCommonOverridesCssResourceName);
		}
		#endregion
		const string
			ColorSchemeName = "ColorScheme",
			GlobalColorSchemeName = "DXColorScheme",
			ColorSchemeCompactPostfix = ".compact";
		public const string
			ColorSchemeLight = "light",
			ColorSchemeDark = "dark";
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string
			ColorSchemeLightCompact = ColorSchemeLight + ColorSchemeCompactPostfix,
			ColorSchemeDarkCompact = ColorSchemeDark + ColorSchemeCompactPostfix;
		const string DefaultColorScheme = ColorSchemeLight;
		public static bool UseDevExtremeFontHandler { get; set; }
		#region ctors
		static ASPxWebClientUIControl() {
			UseDevExtremeFontHandler = true;
			ASPxHttpHandlerModule.Subscribe(new DevExtremeFontProviderSubscriber(() => UseDevExtremeFontHandler), true);
		}
		#endregion
		public static void StaticInitialize() { }
		internal static readonly string[] AvailableColorSchemes = { ColorSchemeLight, ColorSchemeDark  };
		public static bool RegisterDevExtremeFontHandler { get; set; }
#if !SL
	[DevExpressWebLocalizedDescription("ASPxWebClientUIControlGlobalColorScheme")]
#endif
		public static string GlobalColorScheme {
			get { return HttpUtils.GetContextValue<string>(GlobalColorSchemeName, null); }
			set { HttpUtils.SetContextValue<string>(GlobalColorSchemeName, value); }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Theme {
			get { return base.Theme; }
			set { base.Theme = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EnableTheming {
			get { return base.EnableTheming; }
			set { base.EnableTheming = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxWebClientUIControlColorScheme")]
#endif
		[AutoFormatDisable]
		[DefaultValue(DefaultColorScheme)]
		[Localizable(false)]
		[Category("Styles")]
		[TypeConverter("DevExpress.Web.Design.ColorSchemeConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string ColorScheme {
			get { return GetStringProperty(ColorSchemeName, DefaultColorScheme); }
			set { SetStringProperty(ColorSchemeName, DefaultColorScheme, value); }
		}
		protected string UsefulColorScheme {
			get {
				foreach(var availableColorScheme in AvailableColorSchemes) {
					if(string.Equals(GlobalColorScheme, availableColorScheme, StringComparison.OrdinalIgnoreCase)) {
						return availableColorScheme;
					}
				}
				foreach(var availableColorScheme in AvailableColorSchemes) {
					if(string.Equals(ColorScheme, availableColorScheme, StringComparison.OrdinalIgnoreCase)) {
						return availableColorScheme;
					}
				}
				return DefaultColorScheme;
			}
		}
		internal event EventHandler<DesignModeControlEventArgs> CreateDesignModeControl;
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(DesignMode && CreateDesignModeControl != null) {
				var args = new DesignModeControlEventArgs();
				CreateDesignModeControl(this, args);
				if(args.DesignModeControl != null)
					Controls.Add(args.DesignModeControl);
			}
		}
		protected override bool HasLoadingPanel() {
			return false;
		}
	}
}
