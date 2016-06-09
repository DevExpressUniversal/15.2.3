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
using XpfDesign = SL::DevExpress.Xpf.Design;
#else
using XpfDesign = DevExpress.Xpf.Design;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design;
#if SILVERLIGHT
using SL::DevExpress.Xpf.WindowsUI;
using platformGrid = SL::System.Windows.Controls.Grid;
using platformContentControl = SL::System.Windows.Controls.ContentControl;
#else
using DevExpress.Xpf.WindowsUI;
using platformGrid = System.Windows.Controls.Grid;
using platformContentControl = System.Windows.Controls.ContentControl;
using System.Windows.Controls;
using DevExpress.Xpf.Navigation;
#endif
namespace DevExpress.Xpf.Controls.Design.Features {
	class BookInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			item.Properties["Width"].SetValue(100.0);
			item.Properties["Height"].SetValue(100.0);
			XpfDesign.InitializerHelper.Initialize(item);
		}
	}
	class GridInitializer {
		public static ModelItem CreateContentGrid(EditingContext context) {
			ModelItem grid = ModelFactory.CreateItem(context, typeof(platformGrid));
			grid.Properties["Background"].SetValue("#FFE5E5E5");
			return grid;
		}
	}
	class SlideViewInitializer : DefaultInitializer {
		static string DefaultHeader = "Slide View";
		static string DefaultItemHeader = "Slide View Item";
		static void InitializeChild(ModelItem parent, ModelItem item) {
			item.Properties["Header"].SetValue(string.IsNullOrEmpty(item.Name) ? DefaultItemHeader : item.Name);
			item.Properties["Width"].SetValue(200d);
			item.Properties["Content"].SetValue(GridInitializer.CreateContentGrid(parent.Context));
		}
		public static ModelItem CreateItemToParentCollection(ModelItem parent) {
			ModelItem newItem = CreateChild(parent);
			parent.Properties["Items"].Collection.Add(newItem);
			return newItem;
		}
		static ModelItem CreateChild(ModelItem parent) {
			var child = ModelFactory.CreateItem(parent.Context, typeof(SlideViewItem), CreateOptions.None, null);
			InitializeChild(parent, child);
			return child;
		}
		public override void InitializeDefaults(ModelItem item) {
			item.Properties["Header"].SetValue(string.IsNullOrEmpty(item.Name) ? DefaultHeader : item.Name);
			CreateItemToParentCollection(item);
			CreateItemToParentCollection(item);
			XpfDesign.InitializerHelper.Initialize(item);
		}
	}
	class PageViewInitializer : DefaultInitializer {
		static string DefaultHeader = "Page View";
		static string DefaultItemHeader = "Page View Item";
		static void InitializeChild(ModelItem parent, ModelItem item) {
			item.Properties["Header"].SetValue(string.IsNullOrEmpty(item.Name) ? DefaultItemHeader : item.Name);
			item.Properties["Content"].SetValue(GridInitializer.CreateContentGrid(parent.Context));
		}
		static ModelItem CreateChild(ModelItem parent) {
			var child = ModelFactory.CreateItem(parent.Context, typeof(PageViewItem), CreateOptions.None, null);
			InitializeChild(parent, child);
			return child;
		}
		public static ModelItem CreateItemToParentCollection(ModelItem parent) {
			ModelItem newItem = CreateChild(parent);
			parent.Properties["Items"].Collection.Add(newItem);
			return newItem;
		}
		public override void InitializeDefaults(ModelItem item) {
			item.Properties["Header"].SetValue(string.IsNullOrEmpty(item.Name) ? DefaultHeader : item.Name);
			CreateItemToParentCollection(item);
			CreateItemToParentCollection(item);
			XpfDesign.InitializerHelper.Initialize(item);
		}
	}
	class FlipViewInitializer : DefaultInitializer {
		static void InitializeChild(ModelItem parent, ModelItem item) {
			item.Properties["Content"].SetValue(GridInitializer.CreateContentGrid(parent.Context));
		}
		static ModelItem CreateChild(ModelItem parent) {
			var child = ModelFactory.CreateItem(parent.Context, typeof(FlipViewItem), CreateOptions.None, null);
			InitializeChild(parent, child);
			return child;
		}
		public static ModelItem CreateItemToParentCollection(ModelItem parent) {
			ModelItem newItem = CreateChild(parent);
			parent.Properties["Items"].Collection.Add(newItem);
			return newItem;
		}
		public override void InitializeDefaults(ModelItem item) {
			CreateItemToParentCollection(item);
			CreateItemToParentCollection(item);
			XpfDesign.InitializerHelper.Initialize(item);
		}
	}
	class AppBarInitializer : DefaultInitializer {
		static void InitializeChild(ModelItem parent, ModelItem item) {
			if(item.ItemType.IsAssignableFrom(typeof(AppBarButton))) {
				item.Properties["Content"].SetValue("");
				item.Properties["Label"].SetValue("Button");
			}
		}
		static ModelItem CreateChild(ModelItem parent, Type childType) {
			var child = ModelFactory.CreateItem(parent.Context, childType, CreateOptions.None, null);
			InitializeChild(parent, child);
			return child;
		}
		public static ModelItem CreateItemToParentCollection(ModelItem parent, Type childType) {
			ModelItem newItem = CreateChild(parent, childType);
			parent.Properties["Items"].Collection.Add(newItem);
			return newItem;
		}
		static ModelItem CreateChild(ModelItem parent) {
			var child = ModelFactory.CreateItem(parent.Context, typeof(AppBarButton), CreateOptions.None, null);
			InitializeChild(parent, child);
			return child;
		}
		public static ModelItem CreateItemToParentCollection(ModelItem parent) {
			ModelItem newItem = CreateChild(parent);
			parent.Properties["Items"].Collection.Add(newItem);
			return newItem;
		}
		public override void InitializeDefaults(ModelItem item) {
			CreateItemToParentCollection(item);
			CreateItemToParentCollection(item);
			XpfDesign.InitializerHelper.Initialize(item);
		}
	}
#if !SILVERLIGHT
	class BarCodeControlInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			ModelItem newItem = ModelFactory.CreateItem(item.Context, typeof(Code128Symbology), CreateOptions.None, null);
			item.Properties["Symbology"].SetValue(newItem);
			XpfDesign.InitializerHelper.Initialize(item);
		}
	}
	class TileBarInitializer : DefaultInitializer {
		static void InitializeChild(ModelItem parent, ModelItem item) {
			item.Properties["Content"].SetValue("TileBarItem");
		}
		static ModelItem CreateChild(ModelItem parent) {
			var child = ModelFactory.CreateItem(parent.Context, typeof(TileBarItem), CreateOptions.None, null);
			InitializeChild(parent, child);
			return child;
		}
		public static ModelItem CreateItemToParentCollection(ModelItem parent) {
			ModelItem newItem = CreateChild(parent);
			parent.Properties["Items"].Collection.Add(newItem);
			return newItem;
		}
		public override void InitializeDefaults(ModelItem item) {
			CreateItemToParentCollection(item);
			CreateItemToParentCollection(item);
			XpfDesign.InitializerHelper.Initialize(item);
		}
	}
	class TileNavPaneInitializer : DefaultInitializer {
		public static ModelItem CreateNavButton(ModelItem parent, bool isMain = false) {
			var child = ModelFactory.CreateItem(parent.Context, typeof(NavButton), CreateOptions.None, null);
			child.Properties["Content"].SetValue("NavButton");
			child.Properties["IsMain"].SetValue(isMain);
			parent.Properties["NavButtons"].Collection.Add(child);
			return child;
		}
		public static ModelItem CreateCategory(ModelItem parent) {
			var item1 = ModelFactory.CreateItem(parent.Context, typeof(TileNavItem), CreateOptions.None, null);
			var item2 = ModelFactory.CreateItem(parent.Context, typeof(TileNavItem), CreateOptions.None, null);
			item1.Properties["Content"].SetValue("TileNavItem");
			item2.Properties["Content"].SetValue("TileNavItem");
			var category = ModelFactory.CreateItem(parent.Context, typeof(TileNavCategory), CreateOptions.None, null);
			category.Properties["Content"].SetValue("TileNavCategory");
			category.Properties["Items"].Collection.Add(item1);
			category.Properties["Items"].Collection.Add(item2);
			parent.Properties["Categories"].Collection.Add(category);
			return category;
		}
		private static void InitializeDefaultLayout(ModelItem item) {
			CreateNavButton(item, true);
			CreateNavButton(item);
			CreateNavButton(item);
			CreateCategory(item);
			CreateCategory(item);
			CreateCategory(item);
		}
		public override void InitializeDefaults(ModelItem item) {
			InitializeDefaultLayout(item);
			XpfDesign.InitializerHelper.Initialize(item);
		}
	}
	class OfficeNavigationBarInitializer : DefaultInitializer {
		static void InitializeChild(ModelItem parent, ModelItem item) {
			item.Properties["Content"].SetValue("NavigationBarItem");
		}
		static ModelItem CreateChild(ModelItem parent) {
			var child = ModelFactory.CreateItem(parent.Context, typeof(NavigationBarItem), CreateOptions.None, null);
			InitializeChild(parent, child);
			return child;
		}
		public static ModelItem CreateItemToParentCollection(ModelItem parent) {
			ModelItem newItem = CreateChild(parent);
			parent.Properties["Items"].Collection.Add(newItem);
			return newItem;
		}
		public override void InitializeDefaults(ModelItem item) {
			CreateItemToParentCollection(item);
			CreateItemToParentCollection(item);
			XpfDesign.InitializerHelper.Initialize(item);
		}
	}
#endif
}
