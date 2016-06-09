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

#if SILVERLIGHT
extern alias SL;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if SILVERLIGHT
using SL::DevExpress.Xpf.WindowsUI;
using SL::DevExpress.Xpf.Core.Design;
using SL::DevExpress.Design.SmartTags;
#else
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.WindowsUI;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Controls.Design.Services.PropertyLinesProviders;
using DevExpress.Xpf.Navigation;
using DevExpress.Xpf.Core.Design.SmartTags;
using DevExpress.Xpf.Controls.Design.Features.TileNavPaneDesigner;
using Microsoft.Windows.Design.Model;
using System.Windows.Media;
#endif
namespace DevExpress.Xpf.Controls.Design.Features {
	static class ControlsPropertyLineRegistrator {
		public static void RegisterPropertyLines() {
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new SlideViewPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new SlideViewItemPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new PageViewPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new PageViewItemPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new FlipViewPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new FlipViewItemPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new AppBarPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new AppBarButtonPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new AppBarToggleButtonPropertyLineProvider());
#if !SILVERLIGHT
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new FrameDocumentUIServicePropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new FrameNavigationServicePropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new WinUIDialogServicePropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new WinUIMessageBoxServicePropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new BarCodeControlPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new CodabarSymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Code11SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Code128SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Code39ExtendedSymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Code39SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Code93ExtendedSymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Code93SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new CodeMSISymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new DataBarSymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new DataMatrixGS1SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new DataMatrixSymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new EAN128SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new EAN13SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new EAN8SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Industrial2of5SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new IntelligentMailSymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Interleaved2of5SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new ITF14SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Matrix2of5SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new PDF417SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new PostNetSymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new QRCodeSymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new UPCASymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new UPCE0SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new UPCE1SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new UPCSupplemental2SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new UPCSupplemental5SymbologyPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Code128SymbologyBaseCode128GeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Code39SymbologyBaseCode39ExtendedGeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new SymbologyCheckSumBaseCode39ExtendedGeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Code39SymbologyBaseCode39GeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new SymbologyCheckSumBaseCode39GeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new SymbologyCheckSumBaseCode93ExtendedGeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new SymbologyCheckSumBaseCode93GeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Code128SymbologyBaseEAN128GeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Industrial2of5SymbologyBaseIndustrial2of5GeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new SymbologyCheckSumBaseIndustrial2of5GeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Interleaved2of5SymbologyBaseInterleaved2of5GeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Interleaved2of5SymbologyBaseITF14GeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new Industrial2of5SymbologyBaseMatrix2of5GeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new SymbologyCheckSumBaseMatrix2of5GeneratorPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new TileBarPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new TileBarItemPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new OfficeNavigationBarPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new NavigationBarItemPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new TileNavPanePropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new NavButtonPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new PageAdornerControlPropertyLineProvider());
#endif
		}
	}
	sealed class SlideViewPropertyLineProvider : PropertyLinesProviderBase {
		public SlideViewPropertyLineProvider() : base(typeof(SlideView)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => SlideView.HeaderProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => SlideView.ShowBackButtonProperty)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SlideViewPropertyLineCommandProvider(viewModel)));
			return lines;
		}
		class SlideViewPropertyLineCommandProvider : CommandActionLineProvider {
			public SlideViewPropertyLineCommandProvider(IPropertyLineContext context)
				: base(context) {
			}
			protected override string GetCommandText() {
				return "New Slide View Item";
			}
			protected override void OnCommandExecute(object param) {
				XpfModelItem owner = Context.ModelItem as XpfModelItem;
				if(owner != null) {
					var modelItem = owner.Value;
					SlideViewInitializer.CreateItemToParentCollection(modelItem);
				}
			}
		}
	}
	sealed class SlideViewItemPropertyLineProvider : ContentControlPropertyLinesProvider {
		public SlideViewItemPropertyLineProvider() : base(typeof(SlideViewItem)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => SlideViewItem.HeaderProperty)));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => SlideViewItem.CommandProperty)));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => SlideViewItem.CommandParameterProperty)));
			return lines;
		}
	}
	sealed class PageViewPropertyLineProvider : PropertyLinesProviderBase {
		public PageViewPropertyLineProvider() : base(typeof(PageView)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PageView.HeaderProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PageView.ShowBackButtonProperty)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new PageViewPropertyLineCommandProvider(viewModel)));
			return lines;
		}
		class PageViewPropertyLineCommandProvider : CommandActionLineProvider {
			public PageViewPropertyLineCommandProvider(IPropertyLineContext context)
				: base(context) {
			}
			protected override string GetCommandText() {
				return "New Page View Item";
			}
			protected override void OnCommandExecute(object param) {
				XpfModelItem owner = Context.ModelItem as XpfModelItem;
				if(owner != null) {
					var modelItem = owner.Value;
					PageViewInitializer.CreateItemToParentCollection(modelItem);
				}
			}
		}
	}
	sealed class PageViewItemPropertyLineProvider : ContentControlPropertyLinesProvider {
		public PageViewItemPropertyLineProvider() : base(typeof(PageViewItem)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PageViewItem.HeaderProperty)));
			return lines;
		}
	}
	sealed class FlipViewPropertyLineProvider : PropertyLinesProviderBase {
		public FlipViewPropertyLineProvider() : base(typeof(FlipView)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new FlipViewPropertyLineCommandProvider(viewModel)));
			return lines;
		}
		class FlipViewPropertyLineCommandProvider : CommandActionLineProvider {
			public FlipViewPropertyLineCommandProvider(IPropertyLineContext context)
				: base(context) {
			}
			protected override string GetCommandText() {
				return "New Flip View Item";
			}
			protected override void OnCommandExecute(object param) {
				XpfModelItem owner = Context.ModelItem as XpfModelItem;
				if(owner != null) {
					var modelItem = owner.Value;
					FlipViewInitializer.CreateItemToParentCollection(modelItem);
				}
			}
		}
	}
	sealed class FlipViewItemPropertyLineProvider : ContentControlPropertyLinesProvider {
		public FlipViewItemPropertyLineProvider() : base(typeof(FlipViewItem)) { }
	}
	sealed class AppBarPropertyLineProvider : PropertyLinesProviderBase {
		public AppBarPropertyLineProvider() : base(typeof(AppBar)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBar.IsOpenProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBar.IsStickyProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBar.IsBackButtonEnabledProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBar.IsExitButtonEnabledProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBar.ItemSpacingProperty)));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBar.BackCommandProperty)));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBar.ExitCommandProperty)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new AppBarButtonProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new AppBarToggleButtonProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new AppBarSeparatorProvider(viewModel)));
			return lines;
		}
		class AppBarButtonProvider : CommandActionLineProvider {
			public AppBarButtonProvider(IPropertyLineContext context)
				: base(context) {
			}
			protected override string GetCommandText() {
				return "Add AppBarButton";
			}
			protected override void OnCommandExecute(object param) {
				XpfModelItem owner = Context.ModelItem as XpfModelItem;
				if(owner != null) {
					var modelItem = owner.Value;
					var newModelItem = AppBarInitializer.CreateItemToParentCollection(modelItem, typeof(AppBarButton));
					newModelItem.Properties["Label"].SetValue("Button");
					newModelItem.Properties["Content"].SetValue("\ue19b");
				}
			}
		}
		class AppBarToggleButtonProvider : CommandActionLineProvider {
			public AppBarToggleButtonProvider(IPropertyLineContext context)
				: base(context) {
			}
			protected override string GetCommandText() {
				return "Add AppBarToggleButton";
			}
			protected override void OnCommandExecute(object param) {
				XpfModelItem owner = Context.ModelItem as XpfModelItem;
				if(owner != null) {
					var modelItem = owner.Value;
					var newModelItem = AppBarInitializer.CreateItemToParentCollection(modelItem, typeof(AppBarToggleButton));
					newModelItem.Properties["Label"].SetValue("Toggle Button");
					newModelItem.Properties["Content"].SetValue("\ue001");
				}
			}
		}
		class AppBarSeparatorProvider : CommandActionLineProvider {
			public AppBarSeparatorProvider(IPropertyLineContext context)
				: base(context) {
			}
			protected override string GetCommandText() {
				return "Add AppBarSeparator";
			}
			protected override void OnCommandExecute(object param) {
				XpfModelItem owner = Context.ModelItem as XpfModelItem;
				if(owner != null) {
					var modelItem = owner.Value;
					AppBarInitializer.CreateItemToParentCollection(modelItem, typeof(AppBarSeparator));
				}
			}
		}
	}
	sealed class AppBarButtonPropertyLineProvider : ContentControlPropertyLinesProvider {
		public AppBarButtonPropertyLineProvider() : base(typeof(AppBarButton)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBarButton.CommandProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBarButton.LabelProperty)));
			lines.Add(() => new ImageSourcePropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBarButton.GlyphProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBarButton.AllowGlyphThemingProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBarButton.IsEllipseEnabledProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBarButton.GlyphStretchProperty), typeof(Stretch)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBarButton.HorizontalAlignmentProperty), typeof(System.Windows.HorizontalAlignment)));
			return lines;
		}
	}
	sealed class AppBarToggleButtonPropertyLineProvider : ContentControlPropertyLinesProvider {
		public AppBarToggleButtonPropertyLineProvider() : base(typeof(AppBarToggleButton)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => AppBarToggleButton.IsCheckedProperty)));
			return lines;
		}
	}
#if !SILVERLIGHT
	sealed class TileBarPropertyLineProvider : PropertyLinesProviderBase {
		public TileBarPropertyLineProvider() : base(typeof(TileBar)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TileBar.OrientationProperty), typeof(System.Windows.Controls.Orientation)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TileBar.FlyoutShowDirectionProperty), typeof(FlyoutShowDirection)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TileBar.ItemColorModeProperty), typeof(TileColorMode)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new TileBarPropertyLineCommandProvider(viewModel)));
			return lines;
		}
		class TileBarPropertyLineCommandProvider : CommandActionLineProvider {
			public TileBarPropertyLineCommandProvider(IPropertyLineContext context)
				: base(context) {
			}
			protected override string GetCommandText() {
				return "New Tile Bar Item";
			}
			protected override void OnCommandExecute(object param) {
				XpfModelItem owner = Context.ModelItem as XpfModelItem;
				if(owner != null) {
					var modelItem = owner.Value;
					TileBarInitializer.CreateItemToParentCollection(modelItem);
				}
			}
		}
	}
	sealed class TileBarItemPropertyLineProvider : ContentControlPropertyLinesProvider {
		public TileBarItemPropertyLineProvider() : base(typeof(TileBarItem)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TileBarItem.AllowGlyphThemingProperty)));
			lines.Add(() => new ImageSourcePropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TileBarItem.TileGlyphProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TileBarItem.ColorModeProperty), typeof(TileColorMode)));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TileBarItem.CommandProperty)));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TileBarItem.CommandParameterProperty)));
			return lines;
		}
	}
	sealed class OfficeNavigationBarPropertyLineProvider : PropertyLinesProviderBase {
		public OfficeNavigationBarPropertyLineProvider() : base(typeof(OfficeNavigationBar)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => OfficeNavigationBar.MaxItemCountProperty)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new OfficeNavigationBarPropertyLineCommandProvider(viewModel)));
			return lines;
		}
		class OfficeNavigationBarPropertyLineCommandProvider : CommandActionLineProvider {
			public OfficeNavigationBarPropertyLineCommandProvider(IPropertyLineContext context)
				: base(context) {
			}
			protected override string GetCommandText() {
				return "New Navigation Bar Item";
			}
			protected override void OnCommandExecute(object param) {
				XpfModelItem owner = Context.ModelItem as XpfModelItem;
				if(owner != null) {
					var modelItem = owner.Value;
					OfficeNavigationBarInitializer.CreateItemToParentCollection(modelItem);
				}
			}
		}
	}
	sealed class NavigationBarItemPropertyLineProvider : ContentControlPropertyLinesProvider {
		public NavigationBarItemPropertyLineProvider() : base(typeof(NavigationBarItem)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ImageSourcePropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavigationBarItem.GlyphProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavigationBarItem.GlyphAlignmentProperty), typeof(System.Windows.Controls.Dock)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavigationBarItem.DisplayModeProperty), typeof(DevExpress.Xpf.Controls.Primitives.ItemDisplayMode)));
			return lines;
		}
	}
	sealed class TileNavPanePropertyLineProvider : PropertyLinesProviderBase {
		public TileNavPanePropertyLineProvider() : base(typeof(TileNavPane)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TileNavPane.CategoriesSourceProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TileNavPane.NavButtonsSourceProperty)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new TileNavCategoryPropertyLineCommandProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new NavButtonPropertyLineCommandProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new RunDesignerPropertyLineCommandProvider(viewModel)));
			return lines;
		}
		class NavButtonPropertyLineCommandProvider : CommandActionLineProvider {
			public NavButtonPropertyLineCommandProvider(IPropertyLineContext context)
				: base(context) {
			}
			protected override string GetCommandText() {
				return "New Nav Button";
			}
			protected override void OnCommandExecute(object param) {
				XpfModelItem owner = Context.ModelItem as XpfModelItem;
				if(owner != null) {
					var modelItem = owner.Value;
					TileNavPaneInitializer.CreateNavButton(modelItem);
				}
			}
		}
		class TileNavCategoryPropertyLineCommandProvider : CommandActionLineProvider {
			public TileNavCategoryPropertyLineCommandProvider(IPropertyLineContext context)
				: base(context) {
			}
			protected override string GetCommandText() {
				return "New TileNav Category";
			}
			protected override void OnCommandExecute(object param) {
				XpfModelItem owner = Context.ModelItem as XpfModelItem;
				if(owner != null) {
					var modelItem = owner.Value;
					TileNavPaneInitializer.CreateCategory(modelItem);
				}
			}
		}
		class RunDesignerPropertyLineCommandProvider : CommandActionLineProvider {
			public RunDesignerPropertyLineCommandProvider(IPropertyLineContext context) 
				:base(context) {  
			}
			protected override string GetCommandText() {
				return "Run Designer";
			}
			protected override void OnCommandExecute(object param) {
				TileNavPaneDesigner.TileNavPaneDesigner.Show(Context.ModelItem);
			}
		}
	}
	sealed class NavButtonPropertyLineProvider : ContentControlPropertyLinesProvider {
		public NavButtonPropertyLineProvider() : base(typeof(NavButton)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavButton.HorizontalAlignmentProperty), typeof(System.Windows.HorizontalAlignment)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavButton.DisplayModeProperty), typeof(DevExpress.Xpf.Controls.Primitives.ItemDisplayMode)));
			lines.Add(() => new ImageSourcePropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavButton.GlyphProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavButton.GlyphAlignmentProperty), typeof(System.Windows.Controls.Dock)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavButton.AllowGlyphThemingProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavButton.IsMainProperty)));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavButton.CommandProperty)));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavButton.CommandParameterProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => NavButton.ItemsSourceProperty)));
			return lines;
		}
	}
	sealed class PageAdornerControlPropertyLineProvider : ContentControlPropertyLinesProvider {
		public PageAdornerControlPropertyLineProvider() : base(typeof(PageAdornerControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);			
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PageAdornerControl.BackCommandProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PageAdornerControl.BackCommandParameterProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PageAdornerControl.HeaderProperty)));
			return lines;
		}
	}
#endif
}
