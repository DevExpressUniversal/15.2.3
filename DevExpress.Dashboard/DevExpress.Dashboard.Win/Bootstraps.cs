#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.ComponentModel;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Design {
	[	
	ToolboxItemFilter(AssemblyInfo.SRAssemblyDashboardWin, ToolboxItemFilterType.Require),
	EditorBrowsable(EditorBrowsableState.Never)
	]
	public abstract class DashboardItemComponent : Component {
		public abstract DashboardItem CreateDashboardItem();
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "Cards.bmp")
	]
	public class Cards : DashboardItemComponent {
		public Cards() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new CardDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "Gauges.bmp")
	]
	public class Gauges : DashboardItemComponent {
		public Gauges() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new GaugeDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "Pies.bmp")
	]
	public class Pies : DashboardItemComponent {
		public Pies() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new PieDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "Chart.bmp")
	]
	public class Chart : DashboardItemComponent {
		public Chart() : base() { }
		public override DashboardItem CreateDashboardItem() {
			ChartDashboardItem chart = new ChartDashboardItem();
			ChartPane pane = new ChartPane();
			chart.Panes.Add(pane);
			return chart;
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "ScatterChart.bmp")
	]
	public class ScatterChart : DashboardItemComponent {
		public ScatterChart() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new ScatterChartDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "Grid.bmp")
	]
	public class Grid : DashboardItemComponent {
		public Grid() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new GridDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "Pivot.bmp")
	]
	public class Pivot : DashboardItemComponent {
		public Pivot() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new PivotDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "RangeFilter.bmp")
	]
	public class RangeFilter : DashboardItemComponent {
		public RangeFilter() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new RangeFilterDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "ChoroplethMap.bmp")
	]
	public class ChoroplethMap : DashboardItemComponent {
		public ChoroplethMap() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new ChoroplethMapDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "GeoPointMap.bmp")
	]
	public class GeoPointMap : DashboardItemComponent {
		public GeoPointMap() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new GeoPointMapDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "BubbleMap.bmp")
	]
	public class BubbleMap : DashboardItemComponent {
		public BubbleMap() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new BubbleMapDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "PieMap.bmp")
	]
	public class PieMap : DashboardItemComponent {
		public PieMap() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new PieMapDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "TextBox.bmp")
	]
	public class TextBox : DashboardItemComponent {
		public TextBox() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new TextBoxDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "Image.bmp")
	]
	public class Image : DashboardItemComponent {
		public Image() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new ImageDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "ComboBox.bmp")
	]
	public class ComboBox : DashboardItemComponent {
		public ComboBox() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new ComboBoxDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "ListBox.bmp")
	]
	public class ListBox : DashboardItemComponent {
		public ListBox() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new ListBoxDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "TreeView.bmp")
	]
	public class TreeView : DashboardItemComponent {
		public TreeView() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new TreeViewDashboardItem();
		}
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabDashboardItems),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "Group.bmp")
	]
	public class Group : DashboardItemComponent {
		public Group() : base() { }
		public override DashboardItem CreateDashboardItem() {
			return new DashboardItemGroup();
		}
	}
}
