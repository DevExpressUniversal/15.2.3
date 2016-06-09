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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Xpf.Core.Native {
	public class ThemeQualifier : IBindableUriQualifier {
		const string nameValue = "theme";
		List<Tuple<string, int, Theme>> themes;
		int cachedAltitude = 0;
		List<Tuple<string, int, Theme>> Themes { get { return themes ?? (themes = new List<Tuple<string, int, Theme>>()).Do(x => Initialize(x)); } }
		public string Name { get { return nameValue; } }						
		public Binding GetBinding(DependencyObject source) { return new Binding() { Path = new PropertyPath(ThemeManager.TreeWalkerProperty), Source = source }; }
		public int GetAltitude(DependencyObject context, string value, IEnumerable<string> values, out int maxAltitude) {
			if (cachedAltitude == 0)
				cachedAltitude = Themes.Select(x => x.Item2).Max();
			maxAltitude = cachedAltitude;
			var theme = ThemeHelper.GetEditorThemeName(context);
			var elements = Themes.Where(x => x.Item3.Name == theme);
			var element = elements.FirstOrDefault(x => string.Equals(x.Item1, value, StringComparison.InvariantCultureIgnoreCase));
			if (element == null)
				return -1;			
			return element.Item2;
		}
		public bool IsValidValue(string value) {
			return Themes.Select(x => x.Item1).Any(x => string.Equals(x, value, StringComparison.InvariantCultureIgnoreCase));
		}
		void Initialize(List<Tuple<string, int, Theme>> x) {
			foreach (var theme in Theme.Themes) {
				var values = GetThemeValues(theme);
				for (int i = 0; i < values.Length; i++) {
					var currentValue = values[i];
					if (string.IsNullOrEmpty(currentValue))
						continue;
					x.Add(new Tuple<string, int, Theme>(currentValue, i+1, theme));
				}
			}
		}
		string[] GetThemeValues(Theme theme) {
			string[] result = new string[5];
			StringBuilder builder = new StringBuilder();
			int i = 0;
			foreach (var element in theme.Category.Replace("Themes", "").Split(' ').Where(x => !string.IsNullOrEmpty(x))) {
				if (i == 3)
					break;
				builder.Append(element);
				result[i] = builder.ToString();
				i++;
			}
			result[4] = theme.Name;
			return result;
		}
	}
}
