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
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.DemoData.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Threading.Tasks;
using System.Windows.Threading;
namespace DevExpress.Xpf.DemoBase.Helpers {
	enum LoadingSplashBackground { None, White, Black }
	internal class ThemeSelectorHelper : DependencyObject, IDisposable {
		#region Dependency Properties
		public static readonly DependencyProperty LoadingInProgressProperty;
		static ThemeSelectorHelper() {
			Type ownerType = typeof(ThemeSelectorHelper);
			LoadingInProgressProperty = DependencyProperty.Register("LoadingInProgress", typeof(bool), ownerType, new PropertyMetadata(false));
			InitThemes();
		}
		#endregion
		static ReadOnlyCollection<ThemeData> themes;
		public ThemeSelectorHelper() {
			ThemeManager.ThemeChanged += OnThemeManagerThemeChanged;
			OnThemeManagerThemeChanged(typeof(ThemeManager), EventArgs.Empty);
		}
		static void InitThemes() {
			themes = Theme.Themes
				.Where(t => !t.Name.Contains("Demo") && t.ShowInThemeSelector)
				.Select(t => new ThemeData(t))
				.ToList().AsReadOnly();
		}
		public bool LoadingInProgress { get { return (bool)GetValue(LoadingInProgressProperty); } set { SetValue(LoadingInProgressProperty, value); } }
		public ReadOnlyCollection<ThemeData> Themes { get { return themes; } }
		public ReadOnlyCollection<ThemeData> ThemesWithoutTouchlines { get { return themes.Where(t => !t.IsTouchline).ToList().AsReadOnly(); } }
		public ReadOnlyCollection<ThemeData> TouchlineThemes { get { return themes.Where(t => t.IsTouchline).ToList().AsReadOnly(); } }
		void OnThemeManagerThemeChanged(object sender, EventArgs e) {
			if(Dispatcher.CheckAccess())
				UpdateApplicationTheme();
			else
				Dispatcher.BeginInvoke((Action)UpdateApplicationTheme);
		}
		void UpdateApplicationTheme() {
			ApplicationTheme = ThemeManager.ApplicationThemeName == null ? null : Theme.FindTheme(ThemeManager.ApplicationThemeName);
		}
		public static LoadingSplashBackground ThemeToLoadingSplashBackground(string theme) {
			return theme != null && (theme == Theme.MetropolisDarkName || theme == Theme.TouchlineDarkName) ? LoadingSplashBackground.Black : LoadingSplashBackground.White;
		}
		public void Dispose() {
			ThemeManager.ThemeChanged -= OnThemeManagerThemeChanged;
		}
		public Theme ApplicationTheme {
			get { return (Theme)GetValue(ApplicationThemeProperty); }
			set { SetValue(ApplicationThemeProperty, value); }
		}
		public static readonly DependencyProperty ApplicationThemeProperty =
			DependencyProperty.Register("ApplicationTheme", typeof(Theme), typeof(ThemeSelectorHelper), new PropertyMetadata(Theme.Default,
				(d, e) => ((ThemeSelectorHelper)d).OnApplicationThemeChanged()));
		void OnApplicationThemeChanged() {
			Dispatcher.BeginInvoke((Action)(() => {
				ThemeManager.ApplicationThemeName = (ApplicationTheme ?? Theme.Default).Name;
			}), DispatcherPriority.Render);
		}
	}
	internal class ThemeData : System.ComponentModel.INotifyPropertyChanged {
		public static bool IsTouchlineTheme(Theme theme) {
			return theme == Theme.HybridApp || theme.Name.ToLower().Contains("touch");
		}
		public static void EnsureAllowedTheme(ModuleDescription module) {
			var currentTheme = Theme.Themes.FirstOrDefault(t => t.Name == ThemeManager.ApplicationThemeName);
			if (currentTheme == null)
				return;
			bool isTouchTheme = IsTouchlineTheme(currentTheme);
			if(!module.SupportTouchThemes && isTouchTheme) {
				ThemeManager.ApplicationThemeName = Theme.Office2013.Name;
			}
			if(!module.SupportDarkThemes) {
				var darkThemes = new Dictionary<Theme, Theme> {
					{ Theme.MetropolisDark, Theme.MetropolisLight },
					{ Theme.TouchlineDark, Theme.Office2013Touch },
					{ Theme.Office2007Black, Theme.Office2013 },
					{ Theme.Office2010Black, Theme.Office2013 },
				};
				Theme lightTheme;
				if(darkThemes.TryGetValue(currentTheme, out lightTheme)) {
					ThemeManager.ApplicationThemeName = lightTheme.Name;
				}
			}
		}
		bool isEnabled;
		public ThemeData(Theme theme) {
			Theme = theme;
			IsEnabled = true;
		}
		public Theme Theme { get; private set; }
		public bool IsTouchline { get { return IsTouchlineTheme(Theme); } }
		public bool IsEnabled {
			get { return isEnabled; }
			set {
				isEnabled = value;
				if(PropertyChanged != null)
					PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("IsEnabled"));
			}
		}
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
	}
	class ThemeAsNoTouchlineConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Theme theme = value as Theme;
			return theme == null || ThemeData.IsTouchlineTheme(theme) ? null : theme;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
	}
	class ThemeAsTouchlineConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Theme theme = value as Theme;
			return theme == null || !ThemeData.IsTouchlineTheme(theme) ? null : theme;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
	}
}
