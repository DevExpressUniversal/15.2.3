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
using System.Data;
using System.Linq;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid.Customization;
using DevExpress.XtraGrid.Localization;
namespace DevExpress.XtraGrid.Views.BandedGrid.Customization {
	[ToolboxItem(false)]
	public class BandCustomizationListBox : CustomCustomizationListBox {
		public int LevelIndent = 8;
		int maxLevel;
		public BandCustomizationListBox(CustomizationForm form) : base(form) {
			maxLevel = 0;
		}
		public int MaxLevel { get { return maxLevel; } }
		public new BandedGridView View { get { return base.View as BandedGridView; } }
		public override int GetItemHeight() { return (View.ViewInfo as BandedGridViewInfo).BandRowHeight; }
		protected override bool IsDragging { get { return View.State == BandedGridState.BandDragging; } }
		public override void Populate() {
			Items.BeginUpdate();
			try {
				this.maxLevel = 0;
				Items.Clear();
				foreach(GridBand band in View.Bands.OrderBy(q=>View.GetNonFormattedCaption(q.GetCustomizationCaption()))) {
					if(!CanAddBand(band)) continue;
					Items.Add(band);
					if(band.HasChildren) AddBandChildren(band);
				}
			}
			finally {
				Items.EndUpdate();
			}
		}
		protected override bool CanPressItem(object item) { 
			GridBand band = item as GridBand;
			if(band == null || band.Visible) return false;
			return true; 
		}
		protected virtual bool CanAddBand(GridBand band) {
			if(!band.Visible) 
				return band.OptionsBand.ShowInCustomizationForm;
			if(!band.HasChildren) return false;
			bool result = false;
			foreach(GridBand bd in band.Children) {
				result |= CanAddBand(bd);
			}
			return result; 
		}
		protected virtual void AddBandChildren(GridBand band) {
			maxLevel = Math.Max(band.BandLevel, maxLevel);
			if(!band.HasChildren) return;
			foreach(GridBand bd in band.Children.OrderBy(q=>View.GetNonFormattedCaption(q.GetCustomizationCaption()))) {
				if(!CanAddBand(bd)) continue;
				Items.Add(bd);
				AddBandChildren(bd);
			}
		}
		protected override void DrawItemObject(GraphicsCache cache, int index, Rectangle bounds, DrawItemState itemState) {
			GridBand band = GetItemValue(index) as GridBand;
			if(band == null) return;
			int indent = band.BandLevel * LevelIndent;
			Rectangle itemBounds = bounds;
			itemBounds.Width -= indent;
			if(!IsRightToLeft) itemBounds.X += indent;
			DrawBand(cache, band, itemBounds, indent > 0 ? 1 : 0);
			if(indent > 0) {
				if(IsRightToLeft) bounds.X += bounds.Width - indent;
				bounds.Width = indent;
				using(Brush brush = new SolidBrush(BackColor))
					cache.FillRectangle(brush, bounds);
			}
		}
		protected void DrawBand(GraphicsCache cache, GridBand band, Rectangle bounds, int level) {
			if(IsRightToLeft) bounds.X--;
			bounds.Width++;
			((BandedGridPainter)View.Painter).DrawBandDrag(cache, View.ViewInfo as BandedGridViewInfo,
				band, bounds, 
				band == CustomizationForm.PressedItem, true, level > 0 ? HeaderPositionKind.Left: HeaderPositionKind.Center);
		}
		protected override string GetHintCaptionForEmptyList() {
			return GridLocalizer.Active.GetLocalizedString(GridStringId.CustomizationFormBandHint);
		}
		protected override void DoShowItem(object item) {
			base.DoShowItem(item);
			GridBand band = item as GridBand;
			if(band == null) return;
			band.Visible = true;
		}
	}
	[ToolboxItem(false)]
	public class BandedCustomizationForm : CustomizationForm {
		CustomCustomizationListBox _columns, _bands;
		DevExpress.XtraTab.XtraTabControl tabControl;
		XtraTabPage columnsPage, bandsPage;
		DevExpress.XtraEditors.SearchControl searchBox = null;
		string columnFilter = string.Empty, bandFilter = string.Empty;
		public BandedCustomizationForm(BandedGridView view) : base(view) {
		}
		public new BandedGridView View { get { return base.View as BandedGridView; } }
		public virtual XtraTabControl TabControl { get { return tabControl; } }
		protected virtual XtraTabPage ColumnsPage { get { return columnsPage; } }
		protected virtual XtraTabPage BandsPage { get { return bandsPage; } }
		protected override void CreateListBox() {
			tabControl = new XtraTabControl();
			tabControl.LookAndFeel.Assign(View.ElementsLookAndFeel);
			tabControl.Dock = DockStyle.Fill;
			columnsPage = TabControl.TabPages.Add(GridLocalizer.Active.GetLocalizedString(GridStringId.CustomizationColumns));
			bandsPage = TabControl.TabPages.Add(GridLocalizer.Active.GetLocalizedString(GridStringId.CustomizationBands));
			_columns = new ColumnCustomizationListBox(this);
			_columns.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			_columns.Populate();
			_bands = new BandCustomizationListBox(this);
			_bands.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			_bands.Populate();
			columnsPage.Controls.Add(Columns);
			bandsPage.Controls.Add(Bands);
			SetActiveListBox(Columns);
			if(!View.OptionsCustomization.ShowBandsInCustomizationForm) {
				bandsPage = null;
				TabControl.ShowTabHeader = DefaultBoolean.False;
			}
			TabControl.Size = new Size(200, 200);
			TabControl.SelectedPageChanged += new TabPageChangedEventHandler(OnTabSelectedPageChanged);
			this.Padding = new Padding(6);
			Controls.Add(TabControl);
			searchBox = AddSearchControl(Columns, this);
		}
		protected virtual void OnTabSelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			if(e.Page == BandsPage) SetActiveListBox(Bands);
			else SetActiveListBox(Columns);
			if(searchBox != null) {
				if(e.Page == BandsPage) {
					columnFilter = searchBox.Text;
					searchBox.Properties.NullValuePrompt = GridLocalizer.Active.GetLocalizedString(GridStringId.SearchForBand); ;
					searchBox.Client = Bands;
					searchBox.SetFilter(bandFilter);
				} else {
					bandFilter = searchBox.Text;
					searchBox.Properties.NullValuePrompt = DevExpress.XtraEditors.Controls.Localizer.Active.GetLocalizedString(DevExpress.XtraEditors.Controls.StringId.SearchForColumn);
					searchBox.Client = Columns;
					searchBox.SetFilter(columnFilter);
				}
			}
		}
		public virtual CustomCustomizationListBox Columns { get { return _columns; } }
		public virtual CustomCustomizationListBox Bands { get { return _bands; } }
		public override void CheckAndUpdate() {
			bool needUpdate = false;
			if(Columns.ItemHeight != Columns.GetItemHeight()) {
				Columns.ItemHeight = Columns.GetItemHeight();
				needUpdate = true;
			}
			if(Bands.ItemHeight != Bands.GetItemHeight()) {
				Bands.ItemHeight = Bands.GetItemHeight();
				needUpdate = true;
			}
			if(needUpdate) UpdateSize();
			UpdateListBox();
			Refresh();
		}
		public virtual void ShowBands() {
			if(BandsPage == null) return;
			TabControl.SelectedTabPage = BandsPage;
		}
		public virtual void ShowColumns() {
			if(ColumnsPage == null) return;
			TabControl.SelectedTabPage = ColumnsPage;
		}
		protected virtual int GetListBoxWidth() {
			return 200;
		}
		protected override void SetDefaultFormSize() {
			TabControl.LookAndFeel.Assign(View.ElementsLookAndFeel);
			int maxListBox = Math.Max(Columns.GetItemHeight() * 7, Bands.GetItemHeight() * 7);
			Size size = Size.Empty;
			size = TabControl.CalcSizeByPageClient(new Size(GetListBoxWidth(), maxListBox + this.Padding.Top + this.Padding.Bottom));
			this.ClientSize = size;
		}
		public override void UpdateListBox() {
			LookAndFeel.Assign(View.LookAndFeel);
			Columns.LookAndFeel.Assign(View.LookAndFeel);
			Bands.LookAndFeel.Assign(View.LookAndFeel);
			TabControl.LookAndFeel.Assign(View.LookAndFeel);
			Columns.Populate();
			Bands.Populate();
		}
	}
}
