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

extern alias Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Ribbon;
using System.Linq.Expressions;
using Platform::System.Windows.Controls.Primitives;
using Platform::DevExpress.Xpf.Editors.Settings;
#if SILVERLIGHT
using Dock = Platform::DevExpress.Xpf.Core.Dock;
using DevExpress.Design.SmartTags;
#else
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design.SmartTags;
using DevExpress.Utils.Design;
using DevExpress.Images;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#endif
namespace DevExpress.Xpf.Core.Design {
	public abstract class BarItemPropertyLineProviderBase : PropertyLinesProviderBase {
		public BarItemPropertyLineProviderBase(Type itemType) : base(itemType) { }
		protected bool IsRibbonExist(ModelItem selectedItem) {
			if(selectedItem == null)
				return false;
			ModelService service = selectedItem.Context.Services.GetService<ModelService>();
			return service.Find(selectedItem.Root, typeof(IRibbonControl)).Any();
		}
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			viewModel.PropertyChanged += OnPropertyChanged;
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			if(!IsRibbonExist(XpfModelItem.ToModelItem(viewModel.RuntimeSelectedItem))) {
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarItem.AlignmentProperty), typeof(BarItemAlignment)));
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarItem.BarItemDisplayModeProperty), typeof(BarItemDisplayMode)));
			} else
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarItemLink.RibbonStyleProperty), typeof(RibbonItemStyles)));
			return lines;
		}
		void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			var viewModel = (DesignTimeViewModelBase)sender;
			if (viewModel.RuntimeSelectedItem == null || !typeof(BarItem).IsAssignableFrom(viewModel.RuntimeSelectedItem.ItemType)) {
				viewModel.PropertyChanged -= OnPropertyChanged;
				return;
			}
			if (e.PropertyName.Equals(BarButtonItem.GlyphProperty.Name)) {
				BarItemGlyphHelper.UpdateLargeGlyphProperty(viewModel.RuntimeSelectedItem);
			} else if (e.PropertyName.Equals(BarButtonItem.LargeGlyphProperty.Name)) {
				BarItemGlyphHelper.UpdateGlyphProperty(viewModel.RuntimeSelectedItem);
			}
		}
	}
	public sealed class BarItemPropertyLineProvider : BarItemPropertyLineProviderBase {
		public BarItemPropertyLineProvider() : base(typeof(BarItem)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ButtonBase.ContentProperty)));
#if !SILVERLIGHT
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarItem.CommandProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarItem.CommandParameterProperty)));
			lines.Add(() => new ImageSourcePropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarItem.GlyphProperty)));
			lines.Add(() => new ImageSourcePropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarItem.LargeGlyphProperty)));
#endif
			lines.AddRange(base.GetPropertiesImpl(viewModel));
			return lines;
		}
	}
	public sealed class BarCheckItemPropertyLineProvider : BarItemPropertyLineProviderBase {
		public BarCheckItemPropertyLineProvider() : base(typeof(BarCheckItem)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarCheckItem.IsCheckedProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarCheckItem.GroupIndexProperty)));
			return lines;
		}
	}
	public sealed class BarEditItemPropetyLineProvider : PropertyLinesProviderBase {
		public BarEditItemPropetyLineProvider() : base(typeof(BarEditItem)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarEditItem.Content2Property)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarEditItem.EditWidthProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarEditItem.EditValueProperty)));
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarEditItem.EditSettingsProperty), typeof(BaseEditSettings), DXTypeInfoInstanceSource.FromTypeList(TypeLists.EditSettingsTypes.Where(t => (t.Item2 & PropertyTarget.Bar) != 0).Select(t => t.Item1).ToArray())));
			lines.Add(() => new NestedPropertyLinesViewModel(viewModel, BarEditItem.EditSettingsProperty.Name));
			return lines;
		}
	}
	public sealed class BarItemLinkPropertyLineProvider : BarItemPropertyLineProviderBase {
		public BarItemLinkPropertyLineProvider() : base(typeof(BarItemLink)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			IEnumerable<string> barItemNames = GetBarItemNames(viewModel);
			if(barItemNames.Count() > 0)
				lines.Add(() => new ItemListPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarItemLink.BarItemNameProperty), typeof(string), ObjectInstanceSource.FromList(barItemNames)));
			return lines;
		}
		IEnumerable<string> GetBarItemNames(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			ModelItem barItemLink = XpfModelItem.ToModelItem(viewModel.RuntimeSelectedItem);
			ModelItem barManager = BarManagerDesignTimeHelper.FindBarManagerInParent(barItemLink);
			return BarManagerDesignTimeHelper.GetAvailableBarItemsNames(barManager, barItemLink);
		}
	}
	static class BarItemGlyphHelper {
		static IEnumerable<IDXImageInfo> Images { get; set; }
		static BarItemGlyphHelper() {
			Images = Native.DXImageConverter.GetDistinctImages(new DXImageServicesImp());
		}
		public static void UpdateGlyphProperty(IModelItem barItem) {
			UpdateGlypPropertyCore(barItem, BarButtonItem.LargeGlyphProperty, BarButtonItem.GlyphProperty, GetSmallGlyph);
		}
		public static void UpdateGlyphProperty(ModelItem barItem) {
			UpdateGlyphProperty(XpfModelItem.FromModelItem(barItem));
		}
		public static void UpdateLargeGlyphProperty(IModelItem barItem) {
			UpdateGlypPropertyCore(barItem, BarButtonItem.GlyphProperty, BarButtonItem.LargeGlyphProperty, GetLargeGlyph);
		}
		public static void UpdateLargeGlyphProperty(ModelItem barItem) {
			UpdateLargeGlyphProperty(XpfModelItem.FromModelItem(barItem));
		}
		static void UpdateGlypPropertyCore(IModelItem barItem, DependencyProperty sourceProperty, DependencyProperty propertyForSet, Func<IModelItem, DXImageExtension> getImageFunc) {
			if (barItem.Properties[propertyForSet.Name].IsSet || !barItem.Properties[sourceProperty.Name].IsSet)
				return;
			var currentGlyphValue = barItem.Properties[sourceProperty.Name].Value;
			if (currentGlyphValue.ItemType != typeof(DXImageExtension))
				return;
			var glyphValue = getImageFunc(currentGlyphValue);
			if (glyphValue != null)
				SetGlyphValue(barItem, propertyForSet, glyphValue);
		}
		static DXImageExtension GetSmallGlyph(IModelItem dxImageExtension) {
			var imageInfo = (DXImageInfo)dxImageExtension.Properties["Image"].ComputedValue;
			var str = imageInfo.MakeUri().OriginalString.Replace("_32x32.png", "_16x16.png");
			var image = Images.FirstOrDefault(img => img.MakeUri().Equals(str));
			return new DXImageExtension() { Image = new DXImageInfo(image) };
		}
		static DXImageExtension GetLargeGlyph(IModelItem dxImageExtension) {
			var imageInfo = (DXImageInfo)dxImageExtension.Properties["Image"].ComputedValue;
			var str = imageInfo.MakeUri().OriginalString.Replace("_16x16.png", "_32x32.png");
			var image = Images.FirstOrDefault(img => img.MakeUri().Equals(str));
			return new DXImageExtension() { Image = new DXImageInfo(image) };
		}
		static void SetGlyphValue(IModelItem barItem, DependencyProperty property, DXImageExtension glyphValue) {
			using (var scope = barItem.BeginEdit(property.Name)) {
				barItem.Properties[property.Name].SetValue(glyphValue);
				scope.Complete();
			}
		}
	}
}
