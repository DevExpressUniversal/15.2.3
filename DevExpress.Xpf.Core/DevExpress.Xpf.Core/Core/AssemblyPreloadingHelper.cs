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
using System.Reflection;
using System.Threading;
using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Core {
	[Flags]
	public enum ProductPreloading {
		Bars = 0x0,
		Editors = 0x1,
		Grid = 0x2,
		All = Bars | Editors | Grid
	}
	public interface IProductPreloadingItem {
		string AssemblyFullName { get; }
		IEnumerable<FrameworkElement> Controls { get; }
	}
	public class BarsProductPreloadingItem : IProductPreloadingItem {
		public string AssemblyFullName { get { return AssemblyInfo.SRAssemblyXpfCore + AssemblyInfo.FullAssemblyVersionExtension; } }
		public IEnumerable<FrameworkElement> Controls { get { return new List<FrameworkElement> { new ToolBarControl() }; } }
	}
	public class EditorsProductPreloadingItem : IProductPreloadingItem {
		public string AssemblyFullName {
			get { return AssemblyInfo.SRAssemblyXpfCore + AssemblyInfo.FullAssemblyVersionExtension; }
		}
		public IEnumerable<FrameworkElement> Controls {
			get { return new List<FrameworkElement>() { new ComboBoxEdit() }; }
		}
	}
	public class GridProductPreloadingItem : IProductPreloadingItem {
		public string AssemblyFullName {
			get { return AssemblyInfo.SRAssemblyXpfGrid + AssemblyInfo.FullAssemblyVersionExtension; }
		}
		public IEnumerable<FrameworkElement> Controls {
			get { return new List<FrameworkElement>() { (FrameworkElement)Activator.CreateInstance(AssemblyFullName, "DevExpress.Xpf.Grid.GridControl").Unwrap()}; }
		}
	}
	public static class ProductPreloadingHelper {
		static volatile bool isPreloaded = false;
		static readonly HashSet<string> PreloadedAssemblies = new HashSet<string>();
		internal static readonly Dictionary<ProductPreloading, IProductPreloadingItem> RegisteredItems = new Dictionary<ProductPreloading, IProductPreloadingItem>();
		static ProductPreloadingHelper() {
			RegisteredItems.Add(ProductPreloading.Editors, new EditorsProductPreloadingItem());
			RegisteredItems.Add(ProductPreloading.Bars, new BarsProductPreloadingItem());
			RegisteredItems.Add(ProductPreloading.Grid, new GridProductPreloadingItem());
		}
		public static void Perform(ProductPreloading products) {
			isPreloaded = false;
			BackgroundHelper.DoInBackground(() => {
				PerformPreload(products);
			},
			() => {
				while (!isPreloaded) {
					Thread.Sleep(50);
				}
			},
			200, apartmentState: ApartmentState.STA);
		}
		static void PerformPreload(ProductPreloading value) {
			Window window = new ProductPreloadingWindow(value);
			window.ShowDialog();
			isPreloaded = true;
		}
		internal static void PreloadAssembly(string assemblyFullName) {
			if (PreloadedAssemblies.Contains(assemblyFullName))
				return;
			PreloadedAssemblies.Add(assemblyFullName);
			if (AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName == assemblyFullName))
				return;
			Assembly.Load(assemblyFullName);
		}
	}
	public class ProductPreloadingWindow : Window {
		readonly Queue<Func<bool>> actions = new Queue<Func<bool>>();
		readonly ProductPreloading preloading;
		Func<bool> currentAction;
		public ProductPreloadingWindow(ProductPreloading preloading) {
			this.preloading = preloading;
			Loaded += OnLoaded;
			LayoutUpdated += OnLayoutUpdated;
		}
		void OnLayoutUpdated(object sender, EventArgs eventArgs) {
			if (actions.Count == 0)
				return;
			if (currentAction == null)
				currentAction = actions.Dequeue();
			if (currentAction())
				currentAction = null;
			InvalidateMeasure();
		}
		void OnLoaded(object sender, RoutedEventArgs routedEventArgs) {
			foreach (ProductPreloading productInfo in Enum.GetValues(typeof(ProductPreloading))) {
				if (!preloading.HasFlag(productInfo))
					continue;
				IProductPreloadingItem item = ProductPreloadingHelper.RegisteredItems.GetValueOrDefault(productInfo);
				if (item == null)
					continue;
				ProductPreloadingHelper.PreloadAssembly(item.AssemblyFullName);
				foreach (var product in item.Controls) {
					var p = product;
					actions.Enqueue(() => {
						Content = p;
						DispatcherHelper.UpdateLayoutAndDoEvents(this);
						return true;
					});
					actions.Enqueue(() => p.IsLoaded);
				}
			}
			actions.Enqueue(() => {
				Close();
				return true;
			});
			InvalidateMeasure();
		}
	}
}
