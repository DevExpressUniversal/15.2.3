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

using DevExpress.Xpf.DocumentViewer.Themes;
using DevExpress.Xpf.Editors.Helpers;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.DocumentViewer {
	public class CheckBarItemTemplateSelector : DataTemplateSelector {
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			if (item is CommandSetZoomFactorAndModeItem)
				return SelectTemplateForZoomItems(item, container);
			if (item is CommandToggleButton) {
				FrameworkContentElement contentElement = container as FrameworkContentElement;
				return (DataTemplate)contentElement.FindResource(new DocumentViewerThemeKeyExtension {
					ThemeName = ThemeHelper.GetEditorThemeName(container),
					ResourceKey = DocumentViewerThemeKeys.BarToggleButtonTemplate
				});
			}
			return base.SelectTemplate(item, container);
		}
		DataTemplate SelectTemplateForZoomItems(object item, DependencyObject container) {
			CommandSetZoomFactorAndModeItem commandItem = item as CommandSetZoomFactorAndModeItem;
			FrameworkContentElement contentElement = container as FrameworkContentElement;
			if (commandItem == null || contentElement == null)
				return base.SelectTemplate(item, container);
			if (commandItem.IsSeparator) {
				var selectTemplate = (DataTemplate)contentElement.FindResource(new DocumentViewerThemeKeyExtension {
					ThemeName = ThemeHelper.GetEditorThemeName(container),
					ResourceKey = DocumentViewerThemeKeys.SetZoomFactorAndModeItemSeparatorTemplate
				});
				return selectTemplate;
			}
			if (commandItem.ZoomMode == ZoomMode.Custom)
				return (DataTemplate)contentElement.FindResource(new DocumentViewerThemeKeyExtension {
					ThemeName = ThemeHelper.GetEditorThemeName(container),
					ResourceKey = DocumentViewerThemeKeys.SetZoomFactorItemTemplate
				});
			return (DataTemplate)contentElement.FindResource(new DocumentViewerThemeKeyExtension {
				ThemeName = ThemeHelper.GetEditorThemeName(container),
				ResourceKey = DocumentViewerThemeKeys.SetZoomModeItemTemplate
			});
		}
	}
}
