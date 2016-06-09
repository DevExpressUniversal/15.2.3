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
using DevExpress.Xpf.Bars;
#if SL
using IInputElement = System.Windows.UIElement;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class TreeListBandMenuInfo : ColumnMenuInfoBase {
		public TreeListBandMenuInfo(TreeListPopupMenu menu) : base(menu) { }
		protected override void CreateItemsCore() {
			CreateColumnChooserItems();
			CreateFilterEditorItem(true);
			CreateSearchPanelItems();
			CreateFixedStyleItems();
		}
		protected override bool CanCreateFixedStyleMenu() {
			return Band.Owner == DataControl.BandsLayoutCore;
		}
		public override GridMenuType MenuType {
			get { return GridMenuType.Band; }
		}
		public override BarManagerMenuController MenuController {
			get { return ((TreeListView)View).BandMenuController; }
		}
		public BandBase Band { get { return (BandBase)BaseColumn; } }
	}
	public class TreeListColumnMenuInfo : ColumnMenuInfoBase {
		public TreeListColumnMenuInfo(TreeListPopupMenu menu)
			: base(menu) {
		}
		protected new TreeListView View { get { return base.View as TreeListView; } }
		protected override void CreateItemsCore() {
			CreateSortingItems();
			CreateColumnChooserItems();
			CreateBestFitItems();
			CreateExpressionEditorItems();
			CreateFilterControlItems();
			CreateFixedStyleItems();
			CreateSearchPanelItems();
			CreateConditionalFormattingMenuItems();
		}
		void CreateBestFitItems() {
			BarButtonItem bestFitItem = CreateBarButtonItem(DefaultColumnMenuItemNames.BestFit, GridControlStringId.MenuColumnBestFit, false, ImageHelper.GetImage(DefaultColumnMenuItemNames.BestFit), View.TreeListCommands.BestFitColumn);
			bestFitItem.CommandParameter = Column;
			bestFitItem.IsVisible = View.ViewBehavior.CanBestFitColumnCore(Column) && View.IsColumnVisibleInHeaders(Column);
			BarButtonItem bestFitColumnsItem = CreateBarButtonItem(DefaultColumnMenuItemNames.BestFitColumns, GridControlStringId.MenuColumnBestFitColumns, false, null, View.TreeListCommands.BestFitColumns);
			bestFitColumnsItem.IsVisible = View.ViewBehavior.CanBestFitAllColumns();
		}
	}
	public class TreeListPopupMenu : DataControlPopupMenu {
		public TreeListPopupMenu(TreeListView view)
			: base(view) {
		}
		protected override MenuInfoBase CreateMenuInfoCore(GridMenuType? type) {
			switch(type) {
				case GridMenuType.Column:
					return new TreeListColumnMenuInfo(this);
				case GridMenuType.TotalSummary:
					return new GridTotalSummaryMenuInfo(this);
				case GridMenuType.RowCell:
					return new GridCellMenuInfo(this);
				case GridMenuType.FixedTotalSummary:
					return new GridTotalSummaryPanelMenuInfo(this);
				case GridMenuType.GroupPanel:
					throw new NotImplementedException();
				case GridMenuType.Band:
					return new TreeListBandMenuInfo(this);
			}
			return null;
		}
	}
}
