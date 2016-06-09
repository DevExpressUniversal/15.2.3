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

using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.UI {
	#region ChartControlCommandGalleryItemGroup
	public abstract class ChartControlCommandGalleryItemGroup : ControlCommandGalleryItemGroup<IChartContainer, ChartCommandId> {
		public ChartControlCommandGalleryItemGroup()
			: base() {
		}
	}
	#endregion
	#region ChartCommandGalleryItem
	public abstract class ChartCommandGalleryItem : ControlCommandGalleryItem<IChartContainer, ChartCommandId> {
		const int descriptionLeftIndent = 6;
		public ChartCommandGalleryItem()
			: base() {
		}
		protected override SuperToolTip GetSuperTip() {
			SuperToolTip superTip = new SuperToolTip();
			ToolTipTitleItem titleItem = new ToolTipTitleItem();
			ToolTipItem item = new ToolTipItem();
			titleItem.Text = Caption;
			item.Text = Hint;
			item.LeftIndent = descriptionLeftIndent;
			superTip.Items.Add(titleItem);
			superTip.Items.Add(item);
			return superTip;
		}
	}
	#endregion
	#region ChartCommandDropDownGalleryBarItem
	public abstract class ChartCommandDropDownGalleryBarItem : ControlCommandBarButtonGalleryDropDownItem<IChartContainer, ChartCommandId> {
		public ChartCommandDropDownGalleryBarItem()
			: base() {
		}
		public ChartCommandDropDownGalleryBarItem(BarManager manager)
			: base(manager) {
		}
		public ChartCommandDropDownGalleryBarItem(string caption)
			: base(caption) {
		}
		public ChartCommandDropDownGalleryBarItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override System.Drawing.Size GalleryImageSize { get { return new System.Drawing.Size(32, 32); } }
	}
	#endregion
	#region ChartCommandGalleryBarItem
	public abstract class ChartCommandGalleryBarItem : ControlCommandGalleryBarItem<IChartContainer, ChartCommandId> {
		public ChartCommandGalleryBarItem()
			: base() {
		}
	}
	#endregion
	#region ChartRibbonPageGroup
	public abstract class ChartRibbonPageGroup : ControlCommandBasedRibbonPageGroup<IChartContainer, ChartCommandId> {
		public ChartRibbonPageGroup() {
		}
		public ChartRibbonPageGroup(string text)
			: base(text) {
		}
		protected override ChartCommandId EmptyCommandId { get { return ChartCommandId.None; } }
	}
	#endregion
	#region ChartCommandBarButtonItem
	public abstract class ChartCommandBarButtonItem : ControlCommandBarButtonItem<IChartContainer, ChartCommandId> {
		public ChartCommandBarButtonItem()
			: base() {
		}
		public ChartCommandBarButtonItem(BarManager manager)
			: base(manager) {
		}
		public ChartCommandBarButtonItem(string caption)
			: base(caption) {
		}
		public ChartCommandBarButtonItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
	}
	#endregion
	#region ChartCommandBarSubItem
	public abstract class ChartCommandBarSubItem : ControlCommandBarSubItem<IChartContainer, ChartCommandId> {
		public ChartCommandBarSubItem()
			: base() {
			this.MenuDrawMode = MenuDrawMode.SmallImagesText;
		}
		public ChartCommandBarSubItem(BarManager manager)
			: base(manager) {
		}
		public ChartCommandBarSubItem(string caption)
			: base(caption) {
		}
		public ChartCommandBarSubItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
	}
	#endregion
}
