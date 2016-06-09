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
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Charts.Designer.Native {
	public sealed class RibbonPageCategoryTemplateSelector : DataTemplateSelector {
		const string CategoryTemplatesDictionaryPath = ";component/Themes/RibbonElementTemplates/RibbonPageCategories.xaml";
		ResourceDictionary categoryTemplatesDictionary = new ResourceDictionary();
		public RibbonPageCategoryTemplateSelector() {
			string asm = (typeof(ChartDesigner)).Assembly.GetName().Name;
			Uri uriToCategoryTemplatesDictionary = new Uri("pack://application:,,,/" + asm + CategoryTemplatesDictionaryPath, UriKind.Absolute);
			categoryTemplatesDictionary.Source = uriToCategoryTemplatesDictionary;
		}
		object SelectTemplateInternal(object item) {
			RibbonPageCategoryViewModel rpc = item as RibbonPageCategoryViewModel;
			if (rpc == null)
				return null;
			if (rpc.IsDefaultCategory)
				return categoryTemplatesDictionary["ribbonDefaultPageCategory"];
			else
				return categoryTemplatesDictionary["ribbonPageCategory"];
		}
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			DataTemplate dt = (DataTemplate)SelectTemplateInternal(item);
			if (dt == null) {
				string message = GetType().Name + " can not give a template for " + item.GetType().Name + " in the " + container.GetType().Name + ".";
				throw new ChartDesignerException(message);
			}
			return dt;
		}
	}
}
