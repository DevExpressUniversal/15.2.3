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
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.PropertyGrid.Design {
	sealed class PropertyGridPropertyLineProvider : PropertyLinesProviderBase {
		public PropertyGridPropertyLineProvider() : base(typeof(PropertyGridControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PropertyGridControl.SelectedObjectProperty)));
			lines.Add(() => new SeparatorLineViewModel(viewModel));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PropertyGridControl.ShowCategoriesProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PropertyGridControl.SortModeProperty), typeof(PropertyGridSortMode)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PropertyGridControl.ShowDescriptionInProperty), typeof(DescriptionLocation)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PropertyGridControl.ShowSearchBoxProperty)));
			lines.Add(() => new SeparatorLineViewModel(viewModel));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PropertyGridControl.UseCollectionEditorProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PropertyGridControl.AllowListItemInitializerProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PropertyGridControl.AllowInstanceInitializerProperty)));
			return lines;
		}
	}
}
