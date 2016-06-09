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
using DevExpress.Utils;
using DevExpress.Xpf.Core.Design.SmartTags;
using Microsoft.Windows.Design.Model;
using System;
namespace DevExpress.Xpf.Core.Design {
	sealed class DXTabControlPropertyLinesProvider : PropertyLinesProviderBase {
		public DXTabControlPropertyLinesProvider() : base(typeof(DXTabControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new NewDXTabItemCommnadProvider(viewModel)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DXTabControl.TabContentCacheModeProperty), typeof(TabContentCacheMode)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DXTabControl.SelectedIndexProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DXTabControl.SelectedItemProperty)));
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DXTabControl.ViewProperty), typeof(TabControlViewBase), DXTypeInfoInstanceSource.FromTypeList(new Type[] { typeof(TabControlMultiLineView), typeof(TabControlScrollView), typeof(TabControlStretchView) })));
			lines.Add(() => new NestedPropertyLinesViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DXTabControl.ViewProperty)));
			return lines;
		}
	}
	sealed class DXTabItemPropertyLinesProvider : ContentControlPropertyLinesProvider {
		public DXTabItemPropertyLinesProvider() : base(typeof(DXTabItem)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DXTabItem.AllowHideProperty.Name, typeof(DefaultBoolean)));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DXTabItem.CloseCommandProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DXTabItem.CloseCommandParameterProperty.Name));
			lines.Add(() => new ImageSourcePropertyLineViewModel(viewModel, DXTabItem.GlyphProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DXTabItem.HeaderProperty.Name));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DXTabItem.IsSelectedProperty.Name));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DXTabItem.VisibleInHeaderMenuProperty.Name));
			return lines;
		}
	}
	abstract class DXTabControlViewBasePropertyLinesProvider : PropertyLinesProviderBase {
		public DXTabControlViewBasePropertyLinesProvider(Type viewType) : base(viewType) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, TabControlViewBase.HeaderLocationProperty.Name, typeof(HeaderLocation)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, TabControlViewBase.HideButtonShowModeProperty.Name, typeof(HideButtonShowMode)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, TabControlViewBase.ShowHeaderMenuProperty.Name));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, TabControlViewBase.NewButtonShowModeProperty.Name, typeof(NewButtonShowMode)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, TabControlViewBase.RemoveTabItemsOnHidingProperty.Name));
			return lines;
		}
	}
	sealed class DXTabControlScrollViewPropertyLinesViewModel : DXTabControlViewBasePropertyLinesProvider {
		public DXTabControlScrollViewPropertyLinesViewModel() : base(typeof(TabControlScrollView)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Insert(0, () => new BooleanPropertyLineViewModel(viewModel, TabControlScrollView.AllowAnimationProperty.Name));
			lines.Insert(3, () => new EnumPropertyLineViewModel(viewModel, TabControlScrollView.HeaderOrientationProperty.Name, typeof(HeaderOrientation)));
			return lines;
		}
	}
	sealed class DXTabControlMultiLineViewPropertyLinesViewModel : DXTabControlViewBasePropertyLinesProvider {
		public DXTabControlMultiLineViewPropertyLinesViewModel() : base(typeof(TabControlMultiLineView)) { }
	}
	sealed class TabControlStretchViewPropertyLinesViewModel : DXTabControlViewBasePropertyLinesProvider {
		public TabControlStretchViewPropertyLinesViewModel() : base(typeof(TabControlStretchView)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			var lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, TabControlStretchView.CloseWindowOnSingleTabItemHidingProperty.Name));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, TabControlStretchView.DragDropModeProperty.Name, typeof(TabControlDragDropMode)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, TabControlStretchView.SelectedTabMinSizeProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, TabControlStretchView.TabMinSizeProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, TabControlStretchView.TabNormalSizeProperty.Name));
			return lines;
		}
	}
	class NewDXTabItemCommnadProvider : CommandActionLineProvider {
		public NewDXTabItemCommnadProvider(IPropertyLineContext context) :
			base(context) { }
		protected override string GetCommandText() {
			return "Add Tab";
		}
		protected override void OnCommandExecute(object param) {
			using(var scope = Context.ModelItem.BeginEdit(CommandText)) {
				var tabControl = XpfModelItem.ToModelItem(Context.ModelItem);
				ModelItem dxTabItem = ModelFactory.CreateItem(tabControl.Context, typeof(DXTabItem), CreateOptions.InitializeDefaults);
				Context.ModelItem.Properties["Items"].Collection.Add(dxTabItem);
				scope.Complete();
			}
		}
	}
}
