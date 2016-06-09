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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPrinting.DataNodes;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
#endif
namespace DevExpress.Xpf.PivotGrid.Printing {
	public class PivotPrintPage : PivotDataNodeBase, IDataNode
#if DEBUGTEST
		, System.ComponentModel.INotifyPropertyChanged
#endif
	{
		const string mainGridName = "mainGrid";
		internal const string filtersName = "filters";
		PivotGridWpfData data;
		PivotPrintPageValueItemsList rowItems;
		PivotPrintPageValueItemsList columnItems;
		double width;
		double height;
		bool showColumnAreaHeadersBottomBorder, showDataAreaHeadersBottomBorder, showRowAreaHeadersTopBorder;
		Visibility showColumnFieldValues;
		public PivotPrintPage(PivotGridWpfData data) :
			this(data, null, null) { }
		public PivotPrintPage(PivotGridWpfData data, PivotPrintPageValueItemsList columnItems, PivotPrintPageValueItemsList rowItems) {
			this.data = data;
			this.rowItems = rowItems;
			this.columnItems = columnItems;
			this.height = 0;
			this.width = 0;
			this.showColumnAreaHeadersBottomBorder = true;
			this.showRowAreaHeadersTopBorder = true;
			this.showDataAreaHeadersBottomBorder = true;
			this.showColumnFieldValues = Visibility.Visible;
#if DEBUGTEST
			DevExpress.Xpf.PivotGrid.Tests.PrintingTests.PrintPageGCTestHelper.Add(this);
#endif
		}
		public PivotGridWpfData Data {
			get { return data; }
		}
		public PivotGridControl PivotGrid {
			get { return Data != null ? Data.PivotGrid : null; }
		}
		public bool FilterHeadersVisible {
			get { return GetHeadersVisibility(XtraPivotGrid.PivotArea.FilterArea); }
		}
		public bool DataHeadersVisible {
			get { return GetHeadersVisibility(XtraPivotGrid.PivotArea.DataArea); }
		}
		public bool ColumnHeadersVisible {
			get { return GetHeadersVisibility(XtraPivotGrid.PivotArea.ColumnArea); }
		}
		public bool RowHeadersVisible {
			get { return GetHeadersVisibility(XtraPivotGrid.PivotArea.RowArea); }
		}
		public double RowValuesWidth {
			get {
				return Data.VisualItems.GetWidthDifference(0, Data.VisualItems.GetLevelCount(false), false);
			}
		}
		public bool HeadersVisible {
			get { return GetHeadersVisibility(Data, Index); }
		}
		public PivotPrintPageValueItemsList RowItems {
			get { return rowItems; }
			internal protected set { rowItems = value; }
		}
		public PivotPrintPageValueItemsList ColumnItems {
			get { return columnItems; }
			internal set { columnItems = value; }
		}
		public int Top {
			get { return RowItems == null ? 0 : RowItems.StartIndex; }
		}
		public int Bottom {
			get { return RowItems == null ? Data.VisualItems.GetLastLevelItemCount(false) - 1 : RowItems.EndIndex; }
		}
		public int Left {
			get { return ColumnItems == null ? 0 : ColumnItems.StartIndex; }
		}
		public int Right {
			get { return ColumnItems == null ? Data.VisualItems.GetLastLevelItemCount(true) - 1 : ColumnItems.EndIndex; }
		}
		public bool ShowColumnAreaHeadersBottomBorder {
			get { return showColumnAreaHeadersBottomBorder; }
			set { showColumnAreaHeadersBottomBorder = value; }
		}
		public bool ShowDataAreaHeadersBottomBorder {
			get { return showDataAreaHeadersBottomBorder; }
			set { showDataAreaHeadersBottomBorder = value; }
		}
		public bool ShowRowAreaHeadersTopBorder {
			get { return showRowAreaHeadersTopBorder; }
			set { showRowAreaHeadersTopBorder = value; }
		}
		public Visibility ShowColumnFieldValues {
			get { return showColumnFieldValues; }
			set { showColumnFieldValues = value; }
		}
		public double Height {
			get { return height; }
			set { height = value; }
		}
		public double Width {
			get { return width; }
			set { width = value; }
		}
		bool GetHeadersVisibility(PivotArea area) {
			bool print = Data.PivotGrid.GetPrintHeaders(area) && HeadersVisible;
			if(print && area == PivotArea.FilterArea) {
				List<PivotGridField> fields = data.GetFieldsByArea(FieldArea.FilterArea, true);
				if(fields.Count == 0) return false;
				if(!data.PivotGrid.PrintUnusedFilterFields) {
					foreach(PivotGridField field in fields)
						if(field.IsFiltered)
							return true;
					return false;
				}
			}
			return print;
		}
		public static bool GetHeadersVisibility(PivotGridWpfData data, int index) {
			return data.PivotGrid.PrintHeadersOnEveryPage || index == 0;
		}
		class PrintPageScrollableAreaItem : PrintFieldValueItem  {
			bool isColumn;
			int maxLevel;
			public PrintPageScrollableAreaItem(bool isColumn, int maxLevel, PivotFieldValueItem item, PivotVisualItems visualItems)
				: base(item, visualItems) {
				this.isColumn = isColumn;
				this.maxLevel = maxLevel;
			}
			public override string DisplayText { get { return string.Empty; } }
			public override PivotGridField Field { get { return null; } }
			public override object Value { get { return null; } }
			public override int MinLevel { get { return 0; } }
			public override int MaxLevel { get { return isColumn ? maxLevel : 0; } }
			public override int MinIndex { get { return 0; } }
			public override int MaxIndex { get { return isColumn ? 0 : maxLevel; } }
		}
		protected internal static double GetHeadersSize(PivotGridWpfData data, PivotPrintRoot printRoot, ref Size ColumnHeadersSize, ref Size DataHeadersSize, ref Size RowHeadersSize) {
			double defaultHeadersHeight = 50;
			double headersHeight = 0;
			PivotPrintPage page = new PivotPrintPage(data,
				new PivotPrintPageValueItemsList() { StartIndex = 0, EndIndex = 0 },
				new PivotPrintPageValueItemsList() { StartIndex = 0, EndIndex = 0 });
			page.ColumnItems.Add(new PrintPageScrollableAreaItem(true, data.VisualItems.GetLevelCount(true) - 1, data.VisualItems.GetLastLevelItem(false, 0), data.VisualItems));
			page.RowItems.Add(new PrintPageScrollableAreaItem(false, data.VisualItems.GetLevelCount(false) - 1, data.VisualItems.GetLastLevelItem(false, 0), data.VisualItems));
			printRoot.Add(page);
			PivotPrintPageControl pageCtrl = new PivotPrintPageControl();
			pageCtrl.DataContext = page;
			NonLogicalDecorator decorator = new NonLogicalDecorator();
			decorator.Child = pageCtrl;
#if !SL
			data.PivotGrid.AddVisualchild(decorator);
#else
			data.BestWidthCalculator.CellsDecorator.Child = decorator;
#endif
			MeasureContent(decorator); 
			Grid grid = (Grid)pageCtrl.GetElementByName(mainGridName);
			if(grid == null || grid.RowDefinitions.Count < 2) {
				printRoot.Remove(page);
				return defaultHeadersHeight;
			}
			for(int i = 0; i < 2; i++) {
				headersHeight += grid.RowDefinitions[i].ActualHeight;
			}
			PivotPrintHeaderArea filters = (PivotPrintHeaderArea)pageCtrl.GetElementByName(filtersName);
			headersHeight += filters.ActualHeight;
			PivotPrintFieldValues values = (PivotPrintFieldValues)LayoutHelper.FindElement(grid,
												 (d) => { PivotPrintFieldValues vals = d as PivotPrintFieldValues; return vals != null && vals.IsColumn == true; });
			headersHeight -= values.ActualHeight;
			ColumnHeadersSize = GetHeaderSize(grid, FieldArea.ColumnArea);
			DataHeadersSize = GetHeaderSize(grid, FieldArea.DataArea);
			RowHeadersSize = GetHeaderSize(grid, FieldArea.RowArea);
			decorator.Child = null;
#if !SL
			data.PivotGrid.RemoveVisualchild(decorator);
#else
			data.BestWidthCalculator.CellsDecorator.Child = null;
#endif
			pageCtrl.DataContext = null;
			pageCtrl = null;
			printRoot.Remove(page);
			return Math.Round(headersHeight);
		}
		protected internal static Size GetHeaderSize(Grid grid, FieldArea area) {
#if DEBUGTEST
			if(area == FieldArea.FilterArea)
				throw new NotImplementedException();
#endif
			Size headersSize = new Size() { Height = 0, Width = 0 };
			PivotPrintHeaderArea headers = (PivotPrintHeaderArea)LayoutHelper.FindElement(grid,
																(d) => { PivotPrintHeaderArea ar = d as PivotPrintHeaderArea; return ar != null && ar.Area.ToFieldArea() == area; });
			if(headers != null) {
				LayoutHelper.ForEachElement(headers, (e) => {
					if(e as FieldHeader != null && e as GroupHeader == null) {
						headersSize.Width += e.DesiredSize.Width;
						headersSize.Height = Math.Max(headersSize.Height, e.DesiredSize.Height);
					}
				});
			}
			headersSize.Height = Math.Round(headersSize.Height);
			headersSize.Width = Math.Round(headersSize.Width);
			return headersSize;
		}
		protected internal static void MeasureContent(UIElement element) {
#if !SL
			element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			element.Arrange(new Rect(new Point(0, 0), element.DesiredSize));
#endif
			DevExpress.Xpf.Core.LayoutUpdatedHelper.GlobalLocker.DoLockedAction(element.UpdateLayout);
#if SL
			element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			element.Arrange(new Rect(new Point(0, 0), element.DesiredSize));
#endif
		}
		#region PivotDataNodeBase members
		public bool PageBreakAfter { get; set; }
		public override XtraPrinting.DataNodes.IDataNode Parent {
			get {
				return base.Parent;
			}
			protected set {
				base.Parent = value;
			}
		}
		public override void Add(PivotDataNodeBase node) {
			throw new NotImplementedException();
		}
		public override RowViewInfo GetDetail(bool allowContentReuse) {
			DataTemplate pageTemplate = (DataTemplate)ResourceHelper.FindResource(Data.PivotGrid, new PrintingThemeKeyExtension() {
				ResourceKey = PrintingThemeKeys.PageTemplate,
				ThemeName = DevExpress.Xpf.Editors.Helpers.ThemeHelper.GetEditorThemeName(Data.PivotGrid)
			});
			return new RowViewInfo(pageTemplate, this);
		}
		#endregion
#if DEBUGTEST
		#region INotifyPropertyChanged Members
		event System.ComponentModel.PropertyChangedEventHandler System.ComponentModel.INotifyPropertyChanged.PropertyChanged {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		#endregion
#endif
	}
	public class PivotPrintPageControl : Control {
		#region static
		public PivotPrintPageControl() {
			this.SetDefaultStyleKey(typeof(PivotPrintPageControl));
		}
		#endregion
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			PivotPrintPage page = (PivotPrintPage)DataContext;
			if(page == null)
				return;
		}
	}
	public class PivotPrintPageValueItemsList : List<ScrollableAreaItemBase> {
		int startIndex;
		int endIndex;
		public PivotPrintPageValueItemsList()
			: base() {
			startIndex = int.MaxValue;
			endIndex = -1;
		}
		public int StartIndex {
			get { return startIndex; }
			set { startIndex = value; }
		}
		public int EndIndex {
			get { return endIndex; }
			set { endIndex = value; }
		}
	}
}
