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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using System.Collections.Generic;
using DevExpress.XtraGrid.Views.WinExplorer.ViewInfo;
using DevExpress.XtraGrid.Views.WinExplorer;
namespace DevExpress.XtraGrid.Design {
	public class GridControlElementBase {
		GridControl grid;
		string description;
		object tag;
		public GridControlElementBase(GridControl grid) {
			this.grid = grid;
		}
		public GridControl Grid { get { return grid;} }
		public string Description { get { return description; } set { description = value; } }
		public object Tag { get { return tag; } set { tag = value; } }
		public virtual void ExcludeClip(PaintEventArgs e) {
			Rectangle[] boundsArray = BoundsArray;
			if(boundsArray != null) {
				foreach(Rectangle rect in boundsArray) {
					e.Graphics.ExcludeClip(rect);
				}
			}
			if(!Bounds.IsEmpty) e.Graphics.ExcludeClip(Bounds);
		}
		public virtual void Draw(PaintEventArgs e) {
			Rectangle[] boundsArray = BoundsArray;
			if(boundsArray != null || e.Graphics.IsVisible(Bounds)) {
				Pen pen = new Pen(Color.Black);
				pen.DashStyle = DashStyle.Dash;
				if(boundsArray != null)
					e.Graphics.DrawRectangles(pen, boundsArray);
				else e.Graphics.DrawRectangle(pen, Bounds);
				pen.Dispose();
			}
		}
		protected virtual Rectangle[] BoundsArray { get { return null; } }
		protected virtual Rectangle Bounds { get { return Rectangle.Empty; } }
		protected ColumnView ColumnView { get { return Grid != null ? Grid.MainView as ColumnView : null; } }
		protected BaseViewInfo ViewInfo {
			get {
				if(ColumnView == null) return null;
				PropertyInfo propertyInfo = ColumnView.GetType().GetProperty("ViewInfo", BindingFlags.NonPublic | BindingFlags.Instance, null, ViewInfoType, new Type[0], null);
				if(propertyInfo != null) {
					object obj = propertyInfo.GetValue(ColumnView, null);
					return obj as BaseViewInfo;
				}
				return null;
			}
		}
		protected virtual Type ViewInfoType { get {	return typeof(BaseViewInfo); } }
	}
	public class GridControlColumnViewEmptyElement : GridControlElementBase {
		public GridControlColumnViewEmptyElement(GridControl grid) : base(grid) {}
	}
	public class GridControlColumnViewFilterPanelElement : GridControlElementBase {
		public GridControlColumnViewFilterPanelElement(GridControl grid) : base(grid) {}
		protected override Rectangle Bounds {
			get {
				ColumnViewInfo viewInfo = ViewInfo as ColumnViewInfo;
				if(viewInfo == null) return Rectangle.Empty;
				return viewInfo.FilterPanel.Bounds;
			}
		}
	}
	public class GridControlGridViewElementBase : GridControlElementBase {
		public GridControlGridViewElementBase(GridControl grid) : base(grid) {}
		protected GridView GridView  { get { return ColumnView as GridView; } }
		protected GridViewInfo GridViewInfo { get { return ViewInfo as GridViewInfo; } }
	}
	public class GridControlGridViewHeadersElement : GridControlGridViewElementBase {
		public GridControlGridViewHeadersElement(GridControl grid) : base(grid) {}
		protected override Rectangle Bounds {
			get {
				GridViewInfo viewInfo = GridViewInfo;
				if(viewInfo == null) return Rectangle.Empty;
				return viewInfo.ViewRects.ColumnPanel;
			}
		}
	}
	public class GridControlGridViewGroupPanelElement : GridControlGridViewElementBase {
		public GridControlGridViewGroupPanelElement(GridControl grid) : base(grid) {}
		protected override Rectangle Bounds {
			get {
				GridViewInfo viewInfo = GridViewInfo;
				if(viewInfo == null) return Rectangle.Empty;
				return viewInfo.ViewRects.GroupPanel;
			}
		}
	}
	public class GridControlGridViewFooterElement : GridControlGridViewElementBase {
		public GridControlGridViewFooterElement(GridControl grid) : base(grid) {}
		protected override Rectangle Bounds {
			get {
				GridViewInfo viewInfo = GridViewInfo;
				if(viewInfo == null) return Rectangle.Empty;
				return viewInfo.ViewRects.Footer;
			}
		}
	}
	public class GridControlGridViewFilterButtonsElement : GridControlGridViewElementBase {
		public GridControlGridViewFilterButtonsElement(GridControl grid) : base(grid) {}
		protected override Rectangle[] BoundsArray {
			get {
				GridViewInfo viewInfo = GridViewInfo;
				if(viewInfo == null) return null;
				Rectangle[] arrays = new Rectangle[viewInfo.ColumnsInfo.Count];
				for(int i = 0; i < viewInfo.ColumnsInfo.Count; i ++) {
					arrays[i] = Rectangle.Empty;
					foreach(DrawElementInfo info in viewInfo.ColumnsInfo[i].InnerElements) {
						GridFilterButtonInfoArgs fi = info.ElementInfo as GridFilterButtonInfoArgs;
						if(fi != null){
							arrays[i] =  fi.Bounds;
							break;
						}
					}
				}
				return arrays;
			}
		}
	}
	public abstract class GridControlGridViewRowsElementBase : GridControlGridViewElementBase {
		public GridControlGridViewRowsElementBase(GridControl grid) : base(grid) {}
		protected override Rectangle[] BoundsArray {
			get {
				GridViewInfo viewInfo = GridViewInfo;
				if(viewInfo == null) return null;
				Rectangle[] rects = new Rectangle[viewInfo.RowsInfo.Count];
				for(int i = 0; i < 	viewInfo.RowsInfo.Count; i ++) {
					rects[i] = GetRowBounds(viewInfo.RowsInfo[i]);
				}
				return rects;
			}
		}
		protected abstract Rectangle GetRowBounds(GridRowInfo rowInfo);
	}
	public class GridControlGridViewRowFooterElement : GridControlGridViewRowsElementBase {
		public GridControlGridViewRowFooterElement(GridControl grid) : base(grid) {}
		protected override Rectangle GetRowBounds(GridRowInfo rowInfo) {
			return rowInfo.RowFooters.Bounds;
		}
	}
	public class GridControlGridViewGroupRowElement : GridControlGridViewRowsElementBase {
		public GridControlGridViewGroupRowElement(GridControl grid) : base(grid) {}
		protected override Rectangle GetRowBounds(GridRowInfo rowInfo) {
			return rowInfo.IsGroupRow ? rowInfo.Bounds : Rectangle.Empty;
		}
	}
	public class GridControlGridViewRowElement : GridControlGridViewRowsElementBase {
		public GridControlGridViewRowElement(GridControl grid) : base(grid) {}
		protected override Rectangle GetRowBounds(GridRowInfo rowInfo) {
			return rowInfo is GridDataRowInfo && rowInfo.RowHandle >= 0 ? (rowInfo as GridDataRowInfo).DataBounds : Rectangle.Empty;
		}
	}
	public class WinExplorerViewItemElementBase : GridControlElementBase {
		public WinExplorerViewItemElementBase(GridControl grid, bool isGroup, bool useDescription) : base(grid) {
			IsGroup = isGroup;
			UseDescription = useDescription;
		}
		protected bool IsGroup { get; set; }
		protected bool UseDescription { get; set; }
		protected override Rectangle[] BoundsArray {
			get {
				if(!(Grid.MainView is WinExplorerView))
					return new Rectangle[]{};
				List<Rectangle> bounds = new List<Rectangle>();
				foreach(WinExplorerItemViewInfo itemInfo in ((WinExplorerViewInfo)ViewInfo).VisibleItems) {
					if(IsGroup == itemInfo.IsGroupItem)
						bounds.Add(UseDescription? itemInfo.DescriptionBounds: itemInfo.TextBounds);
				}
				return bounds.ToArray();
			}
		}
	}
	public class WinExplorerViewGroupElement : WinExplorerViewItemElementBase {
		public WinExplorerViewGroupElement(GridControl grid) : base(grid, true, false) { }
	}
	public class WinExplorerViewItemTextElement : WinExplorerViewItemElementBase {
		public WinExplorerViewItemTextElement(GridControl grid) : base(grid, false, false) { }
	}
	public class WinExplorerViewItemDescriptionElement : WinExplorerViewItemElementBase {
		public WinExplorerViewItemDescriptionElement(GridControl grid) : base(grid, false, true) { }
	}
	public class GridControlGridViewRowPreviewElement : GridControlGridViewRowsElementBase {
		public GridControlGridViewRowPreviewElement(GridControl grid) : base(grid) {}
		protected override Rectangle GetRowBounds(GridRowInfo rowInfo) {
			return rowInfo is GridDataRowInfo ? (rowInfo as GridDataRowInfo).PreviewBounds : Rectangle.Empty;
		}
	}
	public class GridControlGridViewIndicatorElement : GridControlGridViewRowsElementBase {
		public GridControlGridViewIndicatorElement(GridControl grid) : base(grid) {}
		protected override Rectangle GetRowBounds(GridRowInfo rowInfo) {
			Rectangle rect = rowInfo.IndicatorRect;
			if(!rowInfo.RowFooters.Bounds.IsEmpty) {
				rect.Height += rowInfo.RowFooters.Bounds.Bottom - rect.Bottom;
			}
			return rect;
		}
	}
	public abstract class GridControlGridViewAutoFilterRowElementBase : GridControlGridViewElementBase {
		public GridControlGridViewAutoFilterRowElementBase(GridControl grid) : base(grid) {}
		protected override Rectangle Bounds {
			get {
				GridViewInfo viewInfo = GridViewInfo;
				if(viewInfo == null) return Rectangle.Empty;
				for(int i = 0; i < viewInfo.RowsInfo.Count; i ++) {
					if(IsCorrectRowHandle(viewInfo.RowsInfo[i].RowHandle))
						return GetRowBounds(viewInfo.RowsInfo[i]);
				}
				return Rectangle.Empty;
			}
		}
		protected abstract bool IsCorrectRowHandle(int rowHandle);
		protected virtual Rectangle GetRowBounds(GridRowInfo rowInfo) {
			return rowInfo.Bounds;
		}
	}
	public class GridControlGridViewAutoFilterRowElement : GridControlGridViewAutoFilterRowElementBase {
		public GridControlGridViewAutoFilterRowElement(GridControl grid) : base(grid) {}
		protected override bool IsCorrectRowHandle(int rowHandle) {
			return rowHandle == GridControl.AutoFilterRowHandle;
		}
	}
	public class GridControlGridViewNewItemRowElement : GridControlGridViewAutoFilterRowElementBase {
		public GridControlGridViewNewItemRowElement(GridControl grid) : base(grid) {}
		protected override bool IsCorrectRowHandle(int rowHandle) {
			return rowHandle == GridControl.NewItemRowHandle;
		}
	}
	public class GridControlGridViewFocusedRowElement : GridControlGridViewAutoFilterRowElementBase {
		public GridControlGridViewFocusedRowElement(GridControl grid) : base(grid) {}
		protected override bool IsCorrectRowHandle(int rowHandle) {
			return rowHandle == GridView.FocusedRowHandle;
		}
		protected override Rectangle GetRowBounds(GridRowInfo rowInfo) {
			return rowInfo is GridDataRowInfo ? (rowInfo as GridDataRowInfo).DataBounds : Rectangle.Empty;
		}
	}
	public class GridControlBandedGridViewElementBase : GridControlElementBase {
		public GridControlBandedGridViewElementBase(GridControl grid) : base(grid) {}
		protected BandedGridView BandedGridView  { get { return ColumnView as BandedGridView; } }
		protected BandedGridViewInfo BandedGridViewInfo { get { return ViewInfo as BandedGridViewInfo; } }
	}
	public class GridControlBandedGridViewBandsElement : GridControlBandedGridViewElementBase {
		public GridControlBandedGridViewBandsElement(GridControl grid) : base(grid) {}
		protected override Rectangle Bounds {
			get {
				BandedGridViewInfo viewInfo = BandedGridViewInfo;
				if(viewInfo == null) return Rectangle.Empty;
				return viewInfo.ViewRects.BandPanel;
			}
		}
	}
	public class GridControlLayoutViewElementBase : GridControlElementBase {
		public GridControlLayoutViewElementBase(GridControl grid) : base(grid) { }
		protected LayoutView LayoutView { get { return ColumnView as LayoutView; } }
		protected LayoutViewInfo LayoutViewInfo { get { return ViewInfo as LayoutViewInfo; } }
	}
	public abstract class GridControlLayoutViewCardElementBase : GridControlLayoutViewElementBase {
		public GridControlLayoutViewCardElementBase(GridControl grid): base(grid) {}
		protected override Rectangle[] BoundsArray {
			get {
				LayoutViewInfo viewInfo = LayoutViewInfo;
				if(viewInfo == null) return null;
				Rectangle[] rects = new Rectangle[viewInfo.VisibleCards.Count];
				for(int i = 0; i < viewInfo.VisibleCards.Count; i++) {
					rects[i] = GetCardBounds(viewInfo.VisibleCards[i]);
				}
				return rects;
			}
		}
		protected abstract Rectangle GetCardBounds(LayoutViewCard card);
	}
	public class GridControlLayoutViewCaptionElement : GridControlLayoutViewCardElementBase {
		public GridControlLayoutViewCaptionElement(GridControl grid): base(grid) {}
		protected override Rectangle GetCardBounds(LayoutViewCard card) {
			return card.ViewInfo.BorderInfo.CaptionBounds;
		}
	}
	public abstract class GridControlLayoutViewFieldElementBase : GridControlLayoutViewCardElementBase {
		public GridControlLayoutViewFieldElementBase(GridControl grid): base(grid) {}
		protected override Rectangle GetCardBounds(LayoutViewCard card) {
			if(card.Items.Count == 0) return Rectangle.Empty;
			Rectangle firstRect = GetFieldBounds(card.Items[0].ViewInfo as LayoutViewFieldInfo);
			Rectangle lastRect = GetFieldBounds(card.Items[card.Items.Count-1].ViewInfo as LayoutViewFieldInfo);
			return new Rectangle(firstRect.Location, new Size(lastRect.Right - firstRect.Left, lastRect.Bottom - firstRect.Top));
		}
		protected abstract Rectangle GetFieldBounds(LayoutViewFieldInfo fieldInfo);
	}
	public class GridControlLayoutViewFieldCaptionElement : GridControlLayoutViewFieldElementBase {
		public GridControlLayoutViewFieldCaptionElement(GridControl grid): base(grid) {}
		protected override Rectangle GetFieldBounds(LayoutViewFieldInfo fieldInfo) {
			return fieldInfo.TextAreaRelativeToControl;
		}
	}
	public class GridControlLayoutViewFieldValueElement : GridControlLayoutViewFieldElementBase {
		public GridControlLayoutViewFieldValueElement(GridControl grid): base(grid) {}
		protected override Rectangle GetFieldBounds(LayoutViewFieldInfo fieldInfo) {
			return fieldInfo.RepositoryItemViewInfo.Bounds;
		}
	}
	public class GridControlCardViewElementBase : GridControlElementBase {
		public GridControlCardViewElementBase(GridControl grid) : base(grid) {}
		protected CardView CardView  { get { return ColumnView as CardView; } }
		protected CardViewInfo CardViewInfo { get { return ViewInfo as CardViewInfo; } }
	}
	public class GridControlCardViewQuickCustomizeButtonElement : GridControlCardViewElementBase {
		public GridControlCardViewQuickCustomizeButtonElement(GridControl grid) : base(grid) {}
		protected override Rectangle Bounds {
			get {
				CardViewInfo viewInfo = CardViewInfo;
				if(viewInfo == null) return Rectangle.Empty;
				return viewInfo.QuickCustomizeButton.Bounds;
			}
		}
	}
	public class GridControlCardViewSeparateLineElement : GridControlCardViewElementBase {
		public GridControlCardViewSeparateLineElement(GridControl grid) : base(grid) {
		}
		protected override Rectangle[] BoundsArray {
			get {
				CardViewInfo viewInfo = CardViewInfo;
				if(viewInfo == null) return null;
				if(viewInfo.Indents.Count == 0) return null;
				Rectangle[] rects = new Rectangle[viewInfo.Indents.Count];
				for(int i = 0; i < viewInfo.Indents.Count; i ++)
					rects[i] = viewInfo.Indents[i].Bounds;
				return rects;
			}
		}
	}
	public abstract class GridControlCardViewCardElementBase : GridControlCardViewElementBase {
		public GridControlCardViewCardElementBase(GridControl grid) : base(grid) {
		}
		protected override Rectangle[] BoundsArray {
			get {
				CardViewInfo viewInfo = CardViewInfo;
				if(viewInfo == null) return null;
				Rectangle[] rects = new Rectangle[viewInfo.Cards.Count];
				for(int i = 0; i < viewInfo.Cards.Count; i ++) {
					rects[i] = GetCardBounds(viewInfo.Cards[i]);
				}
				return rects;
			}
		}
		protected abstract Rectangle GetCardBounds(CardInfo cardInfo);
	}
	public class GridControlCardViewCaptionElement : GridControlCardViewCardElementBase {
		public GridControlCardViewCaptionElement(GridControl grid) : base(grid) {
		}
		protected override Rectangle GetCardBounds(CardInfo cardInfo) {
			return cardInfo.CaptionInfo.Bounds;
		}
	}
	public class GridControlCardViewCaptionExpandButtonElement : GridControlCardViewCardElementBase {
		public GridControlCardViewCaptionExpandButtonElement(GridControl grid) : base(grid) {
		}
		protected override Rectangle GetCardBounds(CardInfo cardInfo) {
			return cardInfo.CaptionInfo.ExpandButtonBounds;
		}
	}
	public abstract class GridControlCardViewFieldElementBase : GridControlCardViewCardElementBase {
		public GridControlCardViewFieldElementBase(GridControl grid) : base(grid) {
		}
		protected override Rectangle GetCardBounds(CardInfo cardInfo) {
			if(cardInfo.Fields.Count == 0) return Rectangle.Empty;
			Rectangle firstRect = GetFieldBounds(cardInfo.Fields[0]);
			Rectangle lastRect = GetFieldBounds(cardInfo.Fields[cardInfo.Fields.Count - 1]);
			return new Rectangle(firstRect.Location, new Size(lastRect.Right - firstRect.Left, lastRect.Bottom - firstRect.Top));			
		}
		protected abstract Rectangle GetFieldBounds(CardFieldInfo fieldInfo);
	}
	public class GridControlCardViewFieldCaptionElement : GridControlCardViewFieldElementBase {
		public GridControlCardViewFieldCaptionElement(GridControl grid) : base(grid) {
		}
		protected override Rectangle GetFieldBounds(CardFieldInfo fieldInfo) {
			return fieldInfo.CaptionBounds;
		}
	}
	public class GridControlCardViewFieldValueElement : GridControlCardViewFieldElementBase {
		public GridControlCardViewFieldValueElement(GridControl grid) : base(grid) {
		}
		protected override Rectangle GetFieldBounds(CardFieldInfo fieldInfo) {
			return fieldInfo.ValueBounds;
		}
	}
	public class GridControlElementSelector {
		Type elementType;
		string[] hitTestNames;
		string name;
		public GridControlElementSelector(Type elementType, string hitTestName, string name) 
			: this(elementType, new string[1] {hitTestName}, name) {}
		public GridControlElementSelector(Type elementType, string[] hitTestNames, string name) {
			if(hitTestNames == null)
				hitTestNames = new string[0] {};
			this.hitTestNames = hitTestNames;
			this.elementType = elementType;
			this.name = name;
		}
		public Type ElementType { get { return elementType; } }
		public string[] HitTestNames { get { return hitTestNames; } }
		public string Name { get { return name; } }
		public string Description {
			get {
				if(Name == string.Empty) return Name;
				string st = Name;
				int spacesCount = 0;
				for(int i = 1; i < Name.Length; i++) {
					if(char.IsUpper(Name, i)) {
						st.Insert(i + spacesCount++, " ");
					}
				}
				return st;
			}
		}
	}
	[ToolboxItem(false)]
	public class GridControlElementsSelector : GridControl {
		GridControlElementBase selectedElement;
		ArrayList highLightElements;
		GridControlElementBase focusedElement;
		DevExpress.Utils.ToolTipController toolTip;
		Hashtable elements;
		Hashtable elementSelectors;
		const string CustomHitTest_AutoFilterRow = "AutoFilterRow";
		const string CustomHitTest_NewItemRow = "NewItemRow";
		const string CustomHitTest_GroupRow = "GroupRow";
		const string CustomHitTest_FocusedRow = "FocusedRow";
		public GridControlElementsSelector() : this(typeof(GridView)) {}
		public GridControlElementsSelector(Type viewType) {
			CreateDefaultView(viewType);
			this.selectedElement = null;
			this.focusedElement = null;
			this.highLightElements = new ArrayList();
			this.toolTip = new DevExpress.Utils.ToolTipController();
			this.elements = new Hashtable();
			this.elementSelectors = new Hashtable();
			AddElements();
			AddElementSelectors();
		}
		public void InitSampleView() {
			CreateSampleView();
			SetupSampleView();
		}
		protected virtual void AddElements() {
			AddElement(new GridControlColumnViewEmptyElement(this));
			AddElement(new GridControlColumnViewFilterPanelElement(this));
			AddElement(new GridControlGridViewHeadersElement(this));
			AddElement(new GridControlGridViewGroupPanelElement(this));
			AddElement(new GridControlGridViewFilterButtonsElement(this));
			AddElement(new GridControlGridViewIndicatorElement(this));
			AddElement(new GridControlGridViewAutoFilterRowElement(this));
			AddElement(new GridControlGridViewNewItemRowElement(this));
			AddElement(new GridControlGridViewFooterElement(this));
			AddElement(new GridControlGridViewRowFooterElement(this));
			AddElement(new GridControlGridViewRowPreviewElement(this));
			AddElement(new GridControlGridViewGroupRowElement(this));
			AddElement(new GridControlGridViewFocusedRowElement(this));
			AddElement(new GridControlGridViewRowElement(this));
			AddElement(new GridControlBandedGridViewBandsElement(this));
			AddElement(new GridControlCardViewCaptionElement(this));
			AddElement(new GridControlCardViewFieldCaptionElement(this));
			AddElement(new GridControlCardViewFieldValueElement(this));
			AddElement(new GridControlCardViewCaptionExpandButtonElement(this));
			AddElement(new GridControlCardViewQuickCustomizeButtonElement(this));
			AddElement(new GridControlCardViewSeparateLineElement(this));
			AddElement(new GridControlLayoutViewCaptionElement(this));
			AddElement(new GridControlLayoutViewFieldCaptionElement(this));
			AddElement(new GridControlLayoutViewFieldValueElement(this));
			AddElement(new WinExplorerViewGroupElement(this));
			AddElement(new WinExplorerViewItemTextElement(this));
			AddElement(new WinExplorerViewItemDescriptionElement(this));
		}
		public override object DataSource {
			get {
				return base.DataSource;
			}
			set {
				base.DataSource = value;
			}
		}
		protected virtual void AddElementSelectors() {
			AddElementSelector(typeof(GridControlColumnViewEmptyElement), new string[] {"None", "EmptyRow"}, "Empty");
			AddElementSelector(typeof(GridControlColumnViewFilterPanelElement), 
				new string[] {"FilterPanel", "FilterPanelText", "FilterPanelActiveButton", "FilterPanelCloseButton", "FilterPanelMRUButton"},  "FilterPanel");
			AddElementSelector(typeof(GridControlGridViewHeadersElement), new string[] {"Column", "ColumnButton", "ColumnEdge"}, "ColumnHeaders");
			AddElementSelector(typeof(GridControlGridViewGroupPanelElement), new string[] {"GroupPanel", "GroupPanelColumn", "GroupPanelColumnFilterButton"}, "GroupPanel");
			AddElementSelector(typeof(GridControlGridViewFilterButtonsElement), "ColumnFilterButton", "Filter Buttons");
			AddElementSelector(typeof(GridControlGridViewIndicatorElement), new string[] {"RowIndicator", "RowDetailIndicator"}, "Indicator");
			AddElementSelector(typeof(GridControlGridViewAutoFilterRowElement), CustomHitTest_AutoFilterRow, "AutoFilterRow");
			AddElementSelector(typeof(GridControlGridViewNewItemRowElement), CustomHitTest_NewItemRow, "NewItemRow");
			AddElementSelector(typeof(GridControlGridViewFooterElement), "Footer", "Footer");
			AddElementSelector(typeof(GridControlGridViewRowFooterElement), "RowFooter", "RowFooter");
			AddElementSelector(typeof(GridControlGridViewRowPreviewElement), "RowPreview", "RowPreview");
			AddElementSelector(typeof(GridControlGridViewGroupRowElement), CustomHitTest_GroupRow, "GroupRow");
			AddElementSelector(typeof(GridControlGridViewFocusedRowElement), CustomHitTest_FocusedRow, "FocusedRow");
			AddElementSelector(typeof(GridControlGridViewRowElement), new string[] {"Row", "RowCell", "RowEdge"}, "Row");
			AddElementSelector(typeof(GridControlBandedGridViewBandsElement), new string[] {"Band", "BandPanel", "BandButton", "BandEdge"}, "Bands");
			AddElementSelector(typeof(GridControlCardViewCaptionElement), "CardCaption", "CardCaption");
			AddElementSelector(typeof(GridControlCardViewCaptionExpandButtonElement), "CardExpandButton", "CardExpandButton");
			AddElementSelector(typeof(GridControlCardViewFieldCaptionElement), "FieldCaption", "FieldCaption");
			AddElementSelector(typeof(GridControlCardViewFieldValueElement), "FieldValue", "FieldValue");
			AddElementSelector(typeof(GridControlCardViewQuickCustomizeButtonElement), "QuickCustomizeButton", "QuickCustomizeButton");
			AddElementSelector(typeof(GridControlCardViewSeparateLineElement), "Separator", "Separator");
			AddElementSelector(typeof(GridControlLayoutViewCaptionElement), "LayoutView.CardCaption", "CardCaption");
			AddElementSelector(typeof(GridControlLayoutViewFieldCaptionElement), "LayoutView.FieldCaption", "FieldCaption");
			AddElementSelector(typeof(GridControlLayoutViewFieldValueElement), "LayoutView.FieldValue", "FieldValue");
			AddElementSelector(typeof(GridControlColumnViewFilterPanelElement),
				new string[] { "LayoutView.FilterPanel", "LayoutView.FilterPanelText", "LayoutView.FilterPanelActiveButton", 
					"LayoutView.FilterPanelCloseButton", "LayoutView.FilterPanelMRUButton" }, "FilterPanel");
			AddElementSelector(typeof(WinExplorerViewGroupElement), new string[] { "WinExplorerView.GroupNormal", "WinExplorerView.GroupHovered", "WinExplorerView.GroupPressed" }, "Group");
			AddElementSelector(typeof(WinExplorerViewItemTextElement), new string[] { "WinExplorerView.ItemNormal", "WinExplorerView.ItemHovered", "WinExplorerView.ItemPressed" }, "ItemText");
			AddElementSelector(typeof(WinExplorerViewItemDescriptionElement), new string[] { "WinExplorerView.ItemDescriptionNormal", "WinExplorerView.ItemDescriptionHovered", "WinExplorerView.ItemDescriptionPressed" }, "ItemDescription");
		}
		protected void AddElement(GridControlElementBase element) {
			elements[element.GetType()] = element;
		}
		protected GridControlElementBase GetElementByType(Type elementType) {
			return elements[elementType] as GridControlElementBase;
		}
		protected void AddElementSelector(Type elementType, string hitTestName, string name) {
			AddElementSelector(new GridControlElementSelector(elementType, hitTestName, name));
		}
		protected void AddElementSelector(Type elementType, string[] hitTestNames, string name) {
			AddElementSelector(new GridControlElementSelector(elementType, hitTestNames, name));
		}
		protected void AddElementSelector(GridControlElementSelector elementSelector) {
			GridControlElementBase element = GetElementByType(elementSelector.ElementType);
			if(element != null)
				element.Description = elementSelector.Description;
			for(int i = 0; i < elementSelector.HitTestNames.Length; i ++) {
				elementSelectors[elementSelector.HitTestNames[i]] = element;
			}
		}
		protected override void OnDoubleClick(EventArgs e) {}
		protected override void OnKeyDown(KeyEventArgs e) {}
		protected override void OnKeyPress(KeyPressEventArgs e) {}
		protected override void OnMouseDown(MouseEventArgs e) {
			FocusedElement = GetElementAtPos(e.X, e.Y);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			SelectedElement = GetElementAtPos(e.X, e.Y);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			SelectedElement = null;
		}
		protected virtual GridControlElementBase GetElementAtPos(int x, int y) {
			GridControlElementBase element = null;
			BaseHitInfo info = MainView != null ? MainView.CalcHitInfo(x, y) : null;
			if(info != null) {
				element = elementSelectors[GetElementNameByHitInfo(info)] as GridControlElementBase;
			}
			return element;
		}
		protected virtual string GetElementNameByHitInfo(BaseHitInfo info) {
			string elementName = GetElementNameByHitInfoCore(info);
			return elementName == string.Empty ? GetElementNameByHitInfoReflection(info) : elementName;
		}
		protected string GetElementNameByHitInfoReflection(BaseHitInfo info) {
			PropertyDescriptorCollection collection = TypeDescriptor.GetProperties(info);
			for(int i = 0; i < collection.Count; i ++) {
				if(collection[i].Name == "HitTest") {
					object obj = collection[i].GetValue(info);
					string namespacePrefix = string.Empty;
					if(MainView is LayoutView) namespacePrefix = "LayoutView.";
					return obj != null ? namespacePrefix+obj.ToString() : string.Empty;
				}
			}
			return null;
		}
		protected string GetElementNameByHitInfoCore(BaseHitInfo info) {
			if(info is WinExplorerViewHitInfo) {
				WinExplorerViewHitInfo hitInfo = (WinExplorerViewHitInfo)info;
				if(hitInfo.HitTest == WinExplorerViewHitTest.GroupCaption)
					return "WinExplorerView.GroupNormal";
				if(hitInfo.InItem) {
					if(hitInfo.ItemInfo.DescriptionBounds.Contains(hitInfo.HitPoint))
						return "WinExplorerView.ItemDescriptionNormal";
					else if(hitInfo.ItemInfo.TextBounds.Contains(hitInfo.HitPoint))
						return "WinExplorerView.ItemNormal";
				}
			}
			if(info is GridHitInfo) 
				return GetElementNameByGridHitInfoCore(info as GridHitInfo);
			return string.Empty;
		}
		protected string GetElementNameByGridHitInfoCore(GridHitInfo info) {
			if(info.InRow && info.HitTest != GridHitTest.RowIndicator) {
				if(info.RowHandle == AutoFilterRowHandle)
					return CustomHitTest_AutoFilterRow;
				if(info.RowHandle == NewItemRowHandle)
					return CustomHitTest_NewItemRow;
				if(info.RowHandle < 0)
					return CustomHitTest_GroupRow;
				if(info.RowHandle == MainColumnView.FocusedRowHandle)
					return CustomHitTest_FocusedRow;
			}
			return string.Empty;
		}
		protected void SmartPaint(PaintEventArgs e) {
			GraphicsState state = e.Graphics.Save();
			base.OnPaint(e);
			GridControlElementBase[] elements = HighlightElements;
			for(int i = 0; i < elements.Length; i ++)
				elements[i].ExcludeClip(e);
			DrawBackground(e);
			e.Graphics.Restore(state);
			for(int i = 0; i < elements.Length; i ++)
				elements[i].Draw(e);
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(HighlightElements.Length > 0) {
				SmartPaint(e);
				return;
			}
			base.OnPaint(e);
			DrawBackground(e);
		}
		protected void DrawBackground(PaintEventArgs e) {
			using(Brush brush = new SolidBrush(Color.FromArgb(80, Color.Gray))) {
				e.Graphics.FillRectangle(brush, e.ClipRectangle == Rectangle.Empty ? ClientRectangle : e.ClipRectangle);
			}
		}
		protected GridControlElementBase SelectedElement {
			get { return selectedElement; }
			set {
				if(value == SelectedElement) return;
				selectedElement = value;
				this.highLightElements.Clear();
				OnSelectedElementChanged();
			}
		}
		protected GridControlElementBase[] HighlightElements {
			get { 
				ArrayList list = new ArrayList(highLightElements);
				if(SelectedElement != null && list.IndexOf(SelectedElement) < 0)
					list.Add(selectedElement);
				return (GridControlElementBase[]) list.ToArray(typeof(GridControlElementBase));
			}
			set {
				this.highLightElements.Clear();
				this.selectedElement = null;
				if(value != null) {
					for(int i = 0; i < value.Length; i ++)
						if(value != null)
							this.highLightElements.Add(value[i]);
				}
				Refresh();
			}
		}
		protected virtual void OnSelectedElementChanged() {
			Cursor = SelectedElement == null ? Cursors.Arrow : Cursors.Hand;
			Invalidate();
			HideHint();
			if(SelectedElement != null) {
				this.toolTip.ShowHint(SelectedElement.Description);
			}
		}
		protected void HideHint() {
			this.toolTip.HideHint();
		}
		public void ResetFocusedElement() { 
			focusedElement = null; 
		}
		protected GridControlElementBase FocusedElement {
			get { return focusedElement; }
			set {
				if(value == FocusedElement) return;
				focusedElement = value;
				OnFocusedElementChanged();
			}
		}
		protected virtual void OnFocusedElementChanged() {
		}
		protected ColumnView MainColumnView { get { return MainView as ColumnView; } }
		protected GridView MainGridView  { get { return MainView as GridView; } }
		protected CardView MainCardView  { get { return MainView as CardView; } }
		protected LayoutView MainLayoutView { get { return MainView as LayoutView; } }
		void CreateDefaultView(Type viewType) {
			if(viewType == null || !viewType.IsSubclassOf(typeof(BaseView))) return;
			if(MainView != null) {
				MainView.Dispose();
				MainView = null;
			}
			ConstructorInfo constructorInfoObj = viewType.GetConstructor(Type.EmptyTypes);
			BaseView view = constructorInfoObj.Invoke(null) as BaseView;
			MainView = view;
		}
		void CreateSampleView() {
			if(MainColumnView == null) return;
			MainColumnView.BeginDataUpdate();
			try {
				RepositoryItems.Clear();
				MainColumnView.Columns.Clear();
				DevExpress.XtraGrid.Design.XViewsPrinting xv = new DevExpress.XtraGrid.Design.XViewsPrinting(this);
			} finally {
				MainColumnView.EndDataUpdate();
			}
		}
		void SetupSampleView() {
			if(MainColumnView == null) return;
			MainColumnView.BeginUpdate();
			MainColumnView.BeginDataUpdate();
			try {
				if(MainColumnView.Columns["Category"] != null) 
					MainColumnView.Columns["Category"].FilterInfo = new ColumnFilterInfo(ColumnFilterType.Custom, 3, "[Category] = 3");
				if(MainColumnView.Columns["Unit Price"] != null) 
					MainColumnView.Columns["Unit Price"].FilterInfo = 
						new ColumnFilterInfo(ColumnFilterType.Custom, 10, "[Unit Price] > 10");
				SetupSampleGridView();
				SetupSampleCardView();
				SetupSampleLayoutView();
			} finally {
				MainColumnView.EndDataUpdate();
				if(MainGridView != null) {
					MainGridView.CollapseGroupRow(-2);
					MainGridView.FocusedRowHandle = 0;
				}
				MainColumnView.EndUpdate();
			}
		}
		void SetupSampleGridView() {
			if(MainGridView == null) return;
			MainGridView.OptionsView.ShowAutoFilterRow = true;
			MainGridView.OptionsView.ShowFooter = true;
			MainGridView.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleIfExpanded;
			MainGridView.OptionsView.ShowPreview = true;
			MainGridView.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;
			MainGridView.OptionsBehavior.AutoExpandAllGroups = true;
			MainGridView.VertScrollVisibility = ScrollVisibility.Never;
			MainGridView.OptionsView.AutoCalcPreviewLineCount = true;
			MainGridView.CalcPreviewText += new CalcPreviewTextEventHandler(OnCalcPreviewTest);
			MainGridView.GroupSummary.Add(DevExpress.Data.SummaryItemType.Min, "Unit Price", MainColumnView.Columns["Unit Price"]);
			if(MainGridView.Columns["Discontinued"] != null) {
				MainGridView.Columns["Discontinued"].Group();
				MainGridView.Columns["Discontinued"].SortOrder  = DevExpress.Data.ColumnSortOrder.Descending;
			}
		}
		void SetupSampleCardView() {
			if(MainCardView == null) return;
			MainCardView.VertScrollVisibility = ScrollVisibility.Never;
		}
		void SetupSampleLayoutView() {
			if(MainLayoutView == null) return;
		}
		void OnCalcPreviewTest(object sender, CalcPreviewTextEventArgs e) {
			if(e.RowHandle == 1)
				e.PreviewText = "Preview text test.";
		}
	}
}
