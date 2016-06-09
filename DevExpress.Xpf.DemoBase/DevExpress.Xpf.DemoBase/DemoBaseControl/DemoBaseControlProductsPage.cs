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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using DevExpress.Data.Async.Helpers;
using DevExpress.Data.XtraReports.ServiceModel.DataContracts;
using DevExpress.DemoData.Helpers;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.DemoBase.Internal;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.DemoData;
using DevExpress.Mvvm;
using System.Diagnostics;
using DevExpress.Xpf.Core;
using DevExpress.DemoData.Model;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.DemoBase {
	sealed class DemoBaseControlProductsPage : DemoBaseControlPart, IProductsPageViewModel {
		bool themeChanged = false;
		IEnumerable<IGroupedLinks> allGroups;
		IGroupedLinksControlService GroupedLinksControlService { get { return GetService<IGroupedLinksControlService>(); } }
		IColumnScrollControlService FeaturedDemosColumnScrollControlService { get { return GetService<IColumnScrollControlService>("FeaturedDemos"); } }
		IColumnScrollControlService ProductDemosColumnScrollControlService { get { return GetService<IColumnScrollControlService>("ProductDemos"); } }
		public override void OnNavigatedTo(CompletePageState prevState, CompletePageState newState) {
			GroupedLinksControlService.Enable();
			if(themeChanged) {
				FeaturedDemosColumnScrollControlService.UpdateAfterThemeChanged();
				ProductDemosColumnScrollControlService.UpdateAfterThemeChanged();
				themeChanged = false;
			}
		}
		class DemoCarouselItem : IDemoCarouselItem {
			public string Title { get; private set; }
			public string PlatformLabel { get; private set; }
			public Uri Preview { get; private set; }
			public bool IsAvailable { get; private set; }
			public ICommand OnRunCommand { get; private set; }
			public IEnumerable<DemoCarouselLink> Links { get; private set; }
			public DemoCarouselItem(ProductDescription demo) {
				Debug.Assert(demo.IsFeaturedDemo);
				Title = demo.Title.Text;
				Preview = demo.Screenshot.Uri;
				IsAvailable = demo.IsAvailable;
				OnRunCommand = demo.SelectCommand;
				Links = demo.Solutions.Select(s => new DemoCarouselLink {
					Title = s.Title.Text,
					SelectedCommand = s.OpenCommand
				}).ToList();
			}
		}
		public class GroupedLink : IGroupedLink {
			public string Title { get; set; }
			public string Description { get; set; }
			public ICommand SelectCommand { get; set; }
			public bool IsNew { get; set; }
			public bool IsUpdated { get; set; }
			public bool IsHighlighted { get; set; }
		}
		class GroupedLinks : IGroupedLinks {
			public string Header { get; private set; }
			public IEnumerable<IGroupedLink> Links { get; private set; }
			public ICommand ShowAllCommand { get; private set; }
			public GroupedLinks(ProductDescription product, bool createLinks) {
				Header = product.Title.Text;
				ShowAllCommand = product.SelectCommand;
				if(createLinks) {
					Links = product.Modules
						.OrderByDescending(m => m.IsHighlighted)
						.Select(m => new GroupedLink {
							Title = m.Title.Text,
							Description = m.ShortDescription.Text,
							SelectCommand = m.SelectCommand,
							IsNew = m.IsNew,
							IsUpdated = m.IsUpdated,
							IsHighlighted = m.IsHighlighted
						});
				} else {
					Links = new[] { new GroupedLink() };
				}
			}
		}
		public DemoBaseControlProductsPage(DemoBaseControl demoBaseControl)
			: base(demoBaseControl)
		{
			LinkSelectedCommand = new DelegateCommand<IGroupedLink>(link => {
				GroupedLinksControlService.Disable();
				((GroupedLink)link).SelectCommand.Do(c => c.Execute(null));
			});
			var products = demoBaseControl.Data.Products.Where(p => !p.IsFeaturedDemo && p.IsAvailable);
			EmptyGroups = products.Select(p => new GroupedLinks(p, false)).ToList();
			Groups = allGroups = products.Select(p => new GroupedLinks(p, true)).ToList();
			FeaturedDemos = demoBaseControl.Data.Products.Where(p => p.IsFeaturedDemo).Select(d => new DemoCarouselItem(d)).ToList();
			Title = DataLoader.DefaultPlatform.DisplayName.AsLString();
			Subtitle = DataLoader.DefaultPlatform.ProductListSubtitle.AsLString();
			ThemeManager.ThemeChanged += (s, e) => themeChanged = true;
			Initialized();
		}
		IEnumerable<IGroupedLinks> groups;
		public IEnumerable<IGroupedLinks> Groups {
			get { return groups; }
			set { SetProperty(ref groups, value, () => Groups); }
		}
		IEnumerable<IGroupedLinks> emptyGroups;
		public IEnumerable<IGroupedLinks> EmptyGroups {
			get { return emptyGroups; }
			set { SetProperty(ref emptyGroups, value, () => EmptyGroups); }
		}
		IEnumerable<IDemoCarouselItem> featuredDemos;
		public IEnumerable<IDemoCarouselItem> FeaturedDemos {
			get { return featuredDemos; }
			set { SetProperty(ref featuredDemos, value, () => FeaturedDemos); }
		}
		public LString Title {
			get { return GetProperty(() => Title); }
			set { SetProperty(() => Title, value); }
		}
		public LString Subtitle {
			get { return GetProperty(() => Subtitle); }
			set { SetProperty(() => Subtitle, value); }
		}
		public ProductDescription HoveredProduct {
			get { return GetProperty(() => HoveredProduct); }
			set { SetProperty(() => HoveredProduct, value); }
		}
		public ICommand LinkSelectedCommand { get; private set; }
		public int ProductsViewMaxGroupsPerColumn {
			get { return 7; }
		}
		public string PlatformName {
			get { return Repository.WpfPlatform.Name; }
		}
		public ImageSource PlatformIcon {
			get { return Repository.WpfPlatform.Icon.ImageSource; }
		}
	}
}
