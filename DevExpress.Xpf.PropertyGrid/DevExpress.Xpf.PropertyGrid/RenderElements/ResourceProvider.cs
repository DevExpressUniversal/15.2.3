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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.PropertyGrid.Themes;
using DevExpress.Xpf.Utils.Themes;
using System;
using System.Windows;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public interface IPropertyGridRenderResourceProvider {
		RenderTemplate GetRowTemplate();
		double GetRowMinHeight();
		double GetExpanderWidth();
		double GetSplitterWidth();
	}
	public static class PropertyGridViewExtensions {
		class NullPropertyGridRenderResourceProvider : IPropertyGridRenderResourceProvider {
			public static readonly NullPropertyGridRenderResourceProvider Instance = new NullPropertyGridRenderResourceProvider();
			double IPropertyGridRenderResourceProvider.GetExpanderWidth() { return 22d; }
			double IPropertyGridRenderResourceProvider.GetRowMinHeight() { return 20; }
			RenderTemplate IPropertyGridRenderResourceProvider.GetRowTemplate() { return null; }
			double IPropertyGridRenderResourceProvider.GetSplitterWidth() { return 5; }
		}
		public static IPropertyGridRenderResourceProvider GetResourceProvider(this PropertyGridView view) {
			if (view == null)
				return NullPropertyGridRenderResourceProvider.Instance;
			else
				return view.ResourceProvider;
		}
	}
	public class PropertyGridRenderResourceProvider : IPropertyGridRenderResourceProvider {
		readonly string themeName;
		readonly bool isTouch;
		readonly FrameworkElement target;
		RenderTemplate rowTemplate;
		double? rowMinHeight;
		double? expanderWidth;
		double? splitterWidth;
		public static PropertyGridRenderResourceProvider From(FrameworkElement dObj) {
			var treewalker = ThemeManager.GetTreeWalker(dObj);
			return treewalker != null ? new PropertyGridRenderResourceProvider(ThemeHelper.GetTreewalkerThemeName(treewalker, false), treewalker.IsTouch, dObj) : new PropertyGridRenderResourceProvider(null, false, dObj);
		}
		public PropertyGridRenderResourceProvider(string themeName, bool isTouch, FrameworkElement target) {
			this.themeName = themeName;
			this.isTouch = isTouch;
			this.target = target;
		}
		protected T GetResource<T>(FrameworkElement element, ref T field, VSViewRenderTemplateThemeKeys key) where T : class {
			return field ?? (field = (T)element.FindResource(new VSViewRenderTemplateThemeKeyExtension() {
				ResourceKey = key,
				ThemeName = themeName,
			}));
		}		
		protected TResource GetResource<TResource, TThemeKey, TExtension>(ref TResource field, TThemeKey key)
			where TResource : class
			where TExtension : ThemeKeyExtensionBase<TThemeKey>, new() {
			return field ?? (field = (TResource)target.FindResource(new TExtension() { ResourceKey = key, ThemeName = themeName }));
		}
		protected TResource GetResource<TResource, TThemeKey, TExtension>(ref TResource? field, TThemeKey key)
			where TResource : struct
			where TExtension : ThemeKeyExtensionBase<TThemeKey>, new() {
			if (!field.HasValue)
				field = new TResource?(((TResource)(target.FindResource(new TExtension() { ResourceKey = key, ThemeName = themeName }))));
			return field.Value;
		}
		RenderTemplate IPropertyGridRenderResourceProvider.GetRowTemplate() { return GetResource<RenderTemplate, VSViewRenderTemplateThemeKeys, VSViewRenderTemplateThemeKeyExtension>(ref rowTemplate, VSViewRenderTemplateThemeKeys.RowControl); }
		double IPropertyGridRenderResourceProvider.GetRowMinHeight() { return GetResource<double, PropertyGridViewThemeKeys, PropertyGridViewThemeKeyExtension>(ref rowMinHeight, PropertyGridViewThemeKeys.RowMinHeight); }
		double IPropertyGridRenderResourceProvider.GetExpanderWidth() { return GetResource<double, DataRowThemeKeys, DataRowThemeKeyExtension>(ref expanderWidth, DataRowThemeKeys.ExpanderWidth); }
		double IPropertyGridRenderResourceProvider.GetSplitterWidth() { return GetResource<double, DataRowThemeKeys, DataRowThemeKeyExtension>(ref splitterWidth, DataRowThemeKeys.DataRowHeaderThumbWidth); }		
	}
}
