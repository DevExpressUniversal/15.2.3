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

using DevExpress.Design.SmartTags;
using DevExpress.Mvvm.UI;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using Microsoft.Windows.Design.Interaction;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public class ThemeSelectorPropertyLineViewModel : SmartTagLineViewModelBase {
		static ThemeSelectorPropertyLineViewModel() { }
		public ListCollectionView Themes {
			get {
				if(themesView == null) {
					themesView = CreateThemesListCollectionView();
				}
				return themesView;
			}
		}
		public Theme SelectedTheme {
			get { return selectedTheme; }
			set { SetProperty(ref selectedTheme, value, () => SelectedTheme, OnSelectedThemeChanged); }
		}
		public ThemeSelectorPropertyLineViewModel(IPropertyLineContext context) :
			base(context) {
			Initialize();
		}
		void Initialize() {
			selectedTheme = DesignTimeThemeSelectorService.CurrentTheme ?? Theme.DeepBlue;
		}
		ListCollectionView CreateThemesListCollectionView() {
			ListCollectionView lcv = new ListCollectionView(Theme.Themes);
			lcv.Filter = t => t != Theme.HybridApp && ((Theme)t).ShowInThemeSelector;
			lcv.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
			lcv.CustomSort = new CategoryComparer();
			return lcv;
		}
		void OnSelectedThemeChanged() {
			DesignTimeThemeSelectorService.CurrentTheme = SelectedTheme;
		}
		Theme selectedTheme;
		ListCollectionView themesView;
	}
	class CategoryComparer : IComparer {
		static Dictionary<string, int> dict;
		static CategoryComparer() {
			dict = new Dictionary<string, int>();
			dict.Add(Theme.StandardCategory, 0);
			dict.Add(Theme.MetropolisCategory, 1);
			dict.Add(Theme.Office2016Category, 2);
			dict.Add(Theme.Office2013Category, 3);
			dict.Add(Theme.Office2010Category, 4);
			dict.Add(Theme.Office2007Category, 5);
			dict.Add(Theme.DevExpressCategory, 6);
		}
		public int Compare(object x, object y) {
			Theme firstTheme = x as Theme;
			Theme secondTheme = y as Theme;
			if(firstTheme == null || secondTheme == null)
				return 0;
			int firstIndex = dict[firstTheme.Category];
			int secondIndex = dict[secondTheme.Category];
			int res = firstIndex - secondIndex;
			if(res == 0)
				return firstTheme.Name.CompareTo(secondTheme.Name);
			return res;
		}
	}
	public class ThemeItemTemplateSelector : DataTemplateSelector {
		public DataTemplate SimpleTemplate { get; set; }
		public DataTemplate DetailedTemplate { get; set; }
		public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container) {
			ContentPresenter presenter = (ContentPresenter)container;
			if(presenter.TemplatedParent is ComboBox) {
				return SimpleTemplate;
			} else if(presenter.TemplatedParent is ComboBoxItem) {
				return DetailedTemplate;
			}
			return base.SelectTemplate(item, container);
		}
	}
}
