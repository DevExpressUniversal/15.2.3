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
using DevExpress.Design.SmartTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Windows.Design;
using System.Threading;
using System.Reflection;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System.Windows.Input;
#if SL
using DependencyProperty = Platform::System.Windows.DependencyProperty;
using Platform::DevExpress.Xpf.Core;
#else
using DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
using DevExpress.Xpf.Core.Design.CoreUtils;
using DevExpress.Xpf.Core.Design.SmartTags;
using System.Configuration;
using System.IO;
#endif
namespace DevExpress.Xpf.Core.Design {
	public interface IPropertyLineProvider {
		Type ItemType { get; }
		IEnumerable<SmartTagLineViewModelBase> GetProperties(FrameworkElementSmartTagPropertiesViewModel viewModel);
	}
	[Flags]
	public enum PropertyTarget {
		None = 0,
		Editor = 1,
		Grid = 2,
		PropertyGrid = 4,
		Bar = 8,
		ExceptGrid = Editor | PropertyGrid | Bar,
		All = ExceptGrid | Grid
	}
	public class SmartTagLineViewModelFactory {
		public SmartTagLineViewModelFactory(Func<SmartTagLineViewModelBase> createAction, PropertyTarget propertyTarget) {
			CreatePropertyLine = createAction;
			PropertyTarget = propertyTarget;
		}
		public Func<SmartTagLineViewModelBase> CreatePropertyLine { get; private set; }
		public PropertyTarget PropertyTarget { get; private set; }
	}
	public class SmartTagLineViewModelFactoryList : List<SmartTagLineViewModelFactory> {
		public void Add(Func<SmartTagLineViewModelBase> createAction, PropertyTarget propertyTarget = PropertyTarget.All) {
			Add(new SmartTagLineViewModelFactory(createAction, propertyTarget));
		}
		public void Insert(int n, Func<SmartTagLineViewModelBase> createAction, PropertyTarget propertyTarget = PropertyTarget.All) {
			Insert(n, new SmartTagLineViewModelFactory(createAction, propertyTarget));
		}
	}
	public abstract class PropertyLinesProviderBase : IPropertyLineProvider {
		public PropertyLinesProviderBase(Type itemType, PropertyTarget target = PropertyTarget.All) {
#if DEBUG
			if(itemType == null)
				throw new ArgumentNullException("itemType");
#endif
			ItemType = itemType;
			Target = target;
		}
		public Type ItemType { get; private set; }
		public PropertyTarget Target { get; private set; }
		public IEnumerable<SmartTagLineViewModelBase> GetProperties(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			return GetPropertiesImpl(viewModel).Concat(GetPropertiesFooter(viewModel)).Where(p => (p.PropertyTarget & Target) != 0).Select(p => p.CreatePropertyLine()).ToArray();
		}
		protected virtual SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) { return new SmartTagLineViewModelFactoryList(); }
		protected virtual SmartTagLineViewModelFactoryList GetPropertiesFooter(FrameworkElementSmartTagPropertiesViewModel viewModel) { return new SmartTagLineViewModelFactoryList(); }
		protected string GetPropertyName(Expression<Func<DependencyProperty>> expression) { return DependencyPropertyHelper.GetPropertyName(expression); }
		protected Func<SmartTagLineViewModelBase> GetDataContextPropertyLineFactory(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			return () => new DataContextPropertyLineViewModel(viewModel, FrameworkElement.DataContextProperty.Name);
		}
	}
	public class ButtonBasePropertyLinesProvider : ContentControlPropertyLinesProvider {
		protected ButtonBasePropertyLinesProvider(Type itemType) : base(itemType) { }
		public ButtonBasePropertyLinesProvider() : this(typeof(ButtonBase)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, ButtonBase.CommandProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, ButtonBase.CommandParameterProperty.Name));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, ButtonBase.IsEnabledProperty.Name));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, ButtonBase.VisibilityProperty.Name, typeof(Visibility)));
			return lines;
		}
	}
	public sealed class TextBoxPropertyLinesProvider : PropertyLinesProviderBase {
		public TextBoxPropertyLinesProvider() : base(typeof(TextBox)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, TextBox.TextProperty.Name));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, TextBox.IsEnabledProperty.Name));
			return lines;
		}
	}
	public sealed class TextBlockPropertyLinesProvider : PropertyLinesProviderBase {
		public TextBlockPropertyLinesProvider() : base(typeof(TextBlock)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, TextBlock.TextProperty.Name));
			return lines;
		}
	}
	public sealed class ImagePropertyLinesProvider : PropertyLinesProviderBase {
		public ImagePropertyLinesProvider() : base(typeof(Image)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
#if !SILVERLIGHT
			lines.Add(() => new ImageSourcePropertyLineViewModel(viewModel, Image.SourceProperty.Name));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, Image.StretchProperty.Name, typeof(Stretch)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, Image.StretchDirectionProperty.Name, typeof(StretchDirection)));
#endif
			return lines;
		}
	}
	public class ContentControlPropertyLinesProvider : PropertyLinesProviderBase {
		protected ContentControlPropertyLinesProvider(Type itemType) : base(itemType) { }
		public ContentControlPropertyLinesProvider() : this(typeof(ContentControl)) { }
#if !SL
		protected virtual bool IncludeTooltipProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) { return true; }
#endif
		protected virtual bool IncludeContentProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) { return true; }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			var selectedItem = viewModel.RuntimeSelectedItem;
			if(selectedItem.IsRoot()) {
#if DEBUGTEST
				lines.Add(() => new ThemeSelectorPropertyLineViewModel(viewModel));
#else
				if(FrameworkElementSmartTagAdorner.VSVersion > 10)
					lines.Add(() => new ThemeSelectorPropertyLineViewModel(viewModel));
#endif
				lines.Add(GetDataContextPropertyLineFactory(viewModel));
			}
			if(IncludeContentProperty(viewModel))
				lines.Add(() => new ObjectPropertyLineViewModel(viewModel, ContentControl.ContentProperty.Name));
#if !SL
			if(IncludeTooltipProperty(viewModel))
				lines.Add(() => new ObjectPropertyLineViewModel(viewModel, ContentControl.ToolTipProperty.Name));
#endif
			return lines;
		}
	}
#if !SL
	public class WindowPropertyLinesProvider : ContentControlPropertyLinesProvider {
		protected WindowPropertyLinesProvider(Type itemType) : base(itemType) { }
		public WindowPropertyLinesProvider() : this(typeof(Window)) { }
		protected override bool IncludeContentProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) { return false; }
		protected override bool IncludeTooltipProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) { return false; }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new SeparatorLineViewModel(viewModel) { Text = "Window Properties" });
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, Window.TitleProperty.Name));
			lines.Add(() => new ImageSourcePropertyLineViewModel(viewModel, Window.IconProperty.Name));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, Window.ResizeModeProperty.Name, typeof(ResizeMode)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, Window.ShowInTaskbarProperty.Name));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, Window.SizeToContentProperty.Name, typeof(SizeToContent)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, "WindowStartupLocation", typeof(WindowStartupLocation)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, Window.WindowStateProperty.Name, typeof(WindowState)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, Window.WindowStyleProperty.Name, typeof(WindowStyle)));
			return lines;
		}
		protected override SmartTagLineViewModelFactoryList GetPropertiesFooter(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesFooter(viewModel);
			lines.Add(() => new SeparatorLineViewModel(viewModel) { IsLineVisible = false });
			if(FrameworkElementSmartTagAdorner.VSVersion > 10) {
				Type itemType = viewModel.RuntimeSelectedItem.ItemType;
				foreach(Type windowType in TypeLists.WindowTypes) {
					if(itemType != windowType)
						lines.Add(() => ActionPropertyLineViewModel.CreateLine(new ConvertBaseClassActionProvider(viewModel, windowType)));
				}
			}
			return lines;
		}
	}
	public class DXWindowPropertyLinesProvider : WindowPropertyLinesProvider {
		protected DXWindowPropertyLinesProvider(Type itemType) : base(itemType) { }
		public DXWindowPropertyLinesProvider() : this(typeof(DXWindow)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			var effect = GetBorderEffectValue(viewModel.RuntimeSelectedItem);
			lines.Add(() => new SeparatorLineViewModel(viewModel) { Text = "DXWindow Properties" });
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, "BorderEffect", typeof(BorderEffect)));
			lines.Add(() => new ColorPropertyLineViewModel(viewModel, DXWindow.BorderEffectActiveColorProperty.Name) { IsVisible = GetIsBorderEffectColorLineVisible(effect), OnSelectedItemPropertyChangedAction = OnSelectedItemPropertyChanged });
			lines.Add(() => new ColorPropertyLineViewModel(viewModel, DXWindow.BorderEffectInactiveColorProperty.Name) { IsVisible = GetIsBorderEffectColorLineVisible(effect), OnSelectedItemPropertyChangedAction = OnSelectedItemPropertyChanged });
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DXWindow.IsAeroModeProperty.Name));
			return lines;
		}
		void OnSelectedItemPropertyChanged(PropertyLineViewModelBase line) {
			if(line is ColorPropertyLineViewModel) {
				line.IsVisible = GetIsBorderEffectColorLineVisible(GetBorderEffectValue(line.SelectedItem));
			}
		}
		bool GetIsBorderEffectColorLineVisible(BorderEffect borderEffect) {
			return borderEffect != BorderEffect.None;
		}
		BorderEffect GetBorderEffectValue(IModelItem modelItem) {
			if(typeof(DXWindow).IsAssignableFrom(modelItem.ItemType))
				return BorderEffect.None;
			return (BorderEffect)modelItem.Properties["BorderEffect"].ComputedValue;
		}
	}
	public sealed class UserControlPropertyLinesProvider : ContentControlPropertyLinesProvider {
		public UserControlPropertyLinesProvider() : base(typeof(UserControl)) { }
		protected override bool IncludeContentProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) { return false; }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			var selectedItem = viewModel.RuntimeSelectedItem;
			if(!selectedItem.IsRoot()) {
				lines.Insert(0, GetDataContextPropertyLineFactory(viewModel));
				lines.Add(() => new ObjectPropertyLineViewModel(viewModel, "ParentViewModel", RuntimeTypes.ViewModelExtensions.ResolveType()));
				lines.Add(() => new ObjectPropertyLineViewModel(viewModel, "Parameter", RuntimeTypes.ViewModelExtensions.ResolveType()));
			}
			return lines;
		}
	}
	public sealed class PagePropertyLinesProvider : PropertyLinesProviderBase {
		public PagePropertyLinesProvider() : base(typeof(Page)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new DataContextPropertyLineViewModel(viewModel, FrameworkElement.DataContextProperty.Name));
			return lines;
		}
	}
	public sealed class RangeBasePropertyLinesProvider : PropertyLinesProviderBase {
		public RangeBasePropertyLinesProvider() : base(typeof(RangeBase)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeBase.SmallChangeProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeBase.LargeChangeProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeBase.MinimumProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeBase.MaximumProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => RangeBase.ValueProperty)));
			return lines;
		}
	}
	public sealed class ProgressBarPropertyLinesProvider : PropertyLinesProviderBase {
		public ProgressBarPropertyLinesProvider() : base(typeof(ProgressBar)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ProgressBar.IsIndeterminateProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ProgressBar.OrientationProperty), typeof(Orientation)));
			return lines;
		}
	}
#endif
	public sealed class ItemsControlPropertyLinesProvider : PropertyLinesProviderBase {
		public ItemsControlPropertyLinesProvider() : base(typeof(ItemsControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			var itemType = viewModel.RuntimeSelectedItem.ItemType;
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			if(!typeof(BarContainerControl).IsAssignableFrom(itemType))
				lines.Add(() => new ObjectPropertyLineViewModel(viewModel, Selector.ItemsSourceProperty.Name));
			return lines;
		}
	}
	public sealed class SelectorPropertyLinesProvider : PropertyLinesProviderBase {
		public SelectorPropertyLinesProvider() : base(typeof(Selector)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, Selector.SelectedIndexProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, Selector.SelectedItemProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, Selector.DisplayMemberPathProperty.Name));
			return lines;
		}
	}
	public class ToggleButtonPropertyLinesProvider : ButtonBasePropertyLinesProvider {
		protected ToggleButtonPropertyLinesProvider(Type itemType) : base(itemType) { }
		public ToggleButtonPropertyLinesProvider() : this(typeof(ToggleButton)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, ToggleButton.IsCheckedProperty.Name));
			return lines;
		}
	}
	public static class IModelItemExtension {
		public static bool IsRoot(this IModelItem item) {
			return item.Root != null && item.Root.Equals(item);
		}
	}
#if SL
	public class DataContextPropertyLineViewModel : PropertyLineViewModelBase {
		public DataContextPropertyLineViewModel(IPropertyLineContext context, string propertyName)
			: base(context, propertyName, typeof(object), null, context.PlatformInfoFactory.ForStandardProperty(FrameworkElement.DataContextProperty.Name)) { }
	}
#endif
}
