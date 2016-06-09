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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Grid {
	public class TemplatesContainer : Control {
		public static readonly DependencyProperty GridDataRowTemplateProperty =
			DependencyProperty.Register("GridDataRowTemplate", typeof(DataTemplate), typeof(TemplatesContainer), null);
		public static readonly DependencyProperty GridHeadersTemplateProperty =
			DependencyProperty.Register("GridHeadersTemplate", typeof(DataTemplate), typeof(TemplatesContainer), null);
		public static readonly DependencyProperty ColumnHeaderResourcesProperty =
			DependencyProperty.Register("ColumnHeaderResources", typeof(ResourceDictionary), typeof(TemplatesContainer), null);
		public static readonly DependencyProperty BandColumnHeaderTemplateProperty =
			DependencyProperty.Register("BandColumnHeaderTemplate", typeof(DataTemplate), typeof(TemplatesContainer), null);
		public DataTemplate GridDataRowTemplate {
			get { return (DataTemplate)GetValue(GridDataRowTemplateProperty); }
			set { SetValue(GridDataRowTemplateProperty, value); }
		}
		public DataTemplate GridHeadersTemplate {
			get { return (DataTemplate)GetValue(GridHeadersTemplateProperty); }
			set { SetValue(GridHeadersTemplateProperty, value); }
		}
		public DataTemplate BandColumnHeaderTemplate {
			get { return (DataTemplate)GetValue(BandColumnHeaderTemplateProperty); }
			set { SetValue(BandColumnHeaderTemplateProperty, value); }
		}
		[Browsable(false)]
		public ResourceDictionary ColumnHeaderResources {
			get { return (ResourceDictionary)GetValue(ColumnHeaderResourcesProperty); }
			set { SetValue(ColumnHeaderResourcesProperty, value); }
		}
		public TemplatesContainer() {
			DefaultStyleKey = typeof(TemplatesContainer);
		}
		[Browsable(false)]
		public Thickness GetTopColumnHeaderIndent() {
			return GetColumnHeaderIndentThemedResource("TopRowColumnIndent");
		}
		[Browsable(false)]
		public Thickness GetMiddleColumnHeaderIndent() {
			return GetColumnHeaderIndentThemedResource("MiddleRowColumnIndent");
		}
		[Browsable(false)]
		public Thickness GetBottomColumnHeaderIndent() {
			return GetColumnHeaderIndentThemedResource("BottomRowColumnIndent");
		}
		Thickness GetColumnHeaderIndentThemedResource(string resourceName) {
			return (Thickness)(GetColumnHeaderThemedResource(resourceName) ?? new Thickness());
		}
		object GetColumnHeaderThemedResource(string resourceName) {
			string themeName = ThemeManager.ApplicationThemeName;
#if SILVERLIGHT
			themeName = ThemeManager.GetThemeName(this);
			if(string.IsNullOrEmpty(themeName)) themeName = "LightGray";
#else
			if(ThemeManager.GetTreeWalker(this) != null)
				themeName = ThemeManager.GetTreeWalker(this).ThemeName;
			if(string.IsNullOrEmpty(themeName)) themeName = "DeepBlue";
#endif
			string themedResourceName = resourceName + themeName;
			if(ColumnHeaderResources == null || !ColumnHeaderResources.Contains(themedResourceName))
				return null;
			return ColumnHeaderResources[themedResourceName];
		}
	}
}
