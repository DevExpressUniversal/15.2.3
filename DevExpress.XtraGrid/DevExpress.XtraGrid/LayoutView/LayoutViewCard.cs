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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Layout.Drawing;
using DevExpress.XtraGrid.Views.Layout.Events;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraLayout.Resizing;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.ViewInfo;
namespace DevExpress.XtraGrid.Views.Layout {
	public class LayoutViewCard : LayoutControlGroup, IRowConditionFormatProvider {
		Point position = Point.Empty;
		int rowHandle = -1;
		int visibleIndex = -1;
		int visibleColumn = -1;
		int visibleRow = -1;
		GridRowCellState state = GridRowCellState.Dirty;
		ConditionInfo conditionInfoCore = null;
		RowFormatRuleInfo formatInfoCore = null;
		bool isPartiallyVisibleCore = false;
		public LayoutViewCard() {
			this.conditionInfoCore = new ConditionInfo();
			this.formatInfoCore = new RowFormatRuleInfo(null);
		}
		protected override void CloneCommonProperties(LayoutGroup parent, ILayoutControl owner, ref BaseLayoutItem clone) {
			base.CloneCommonProperties(parent, owner, ref clone);
			LayoutViewCard card = clone as LayoutViewCard;
			card.conditionInfoCore = new Base.ViewInfo.ConditionInfo();
			card.formatInfoCore = new RowFormatRuleInfo(owner as LayoutView);
		}
		internal void CheckRTL(bool isRTL) {
			base.SetRTL(isRTL, true);
		}
		protected override XtraLayout.Utils.Padding DefaultSpaces {
			get { return new XtraLayout.Utils.Padding(0); }
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("LayoutViewCardIsPartiallyVisible")]
#endif
		public bool IsPartiallyVisible {
			get { return isPartiallyVisibleCore; }
			internal set { isPartiallyVisibleCore = value; }
		}
		protected override bool NeedSupressDrawBorder {
			get {
				if(Owner != null && Owner.DesignMode) return true;
				else return base.NeedSupressDrawBorder;
			}
		}
		protected internal void Assign(LayoutViewCard template) {
			AssignInternal(template);
		}
		protected internal RowFormatRuleInfo FormatInfo {
			get {
				formatInfoCore.SetView(View);
				return formatInfoCore; 
			}
		}
		protected internal AppearanceObjectEx GetConditionAppearance(GridColumn column) {
			var res = FormatInfo.GetCellAppearance(column);
			if(res != null) return res;
			return ConditionInfo.GetCellAppearance(column);
		}
		protected internal bool CheckDrawFormat(GraphicsCache cache, GridColumn column,
			XtraEditors.ViewInfo.BaseEditViewInfo viewInfo,
			XtraEditors.Helpers.DrawAppearanceMethod drawMethod,
			AppearanceObject originalContentAppearance) {
			if(column == null) return false;
			return FormatInfo.ApplyDrawFormat(cache, column, viewInfo.Bounds, RowHandle, viewInfo, drawMethod, originalContentAppearance);
		}
		protected internal ConditionInfo ConditionInfo { get { return conditionInfoCore; } }
		protected internal LayoutPaintStyle CardPaintStyle { get { return base.PaintStyle; } }
		[XtraSerializableProperty(), DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string TypeName {
			get { return "LayoutViewCard"; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowCustomizeChildren {
			get { return base.AllowCustomizeChildren; }
			set { base.AllowCustomizeChildren = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Point Location { get { return base.Location; } set { base.Location = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleIndex { get { return visibleIndex; } internal set { visibleIndex = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleColumn { get { return visibleColumn; } internal set { visibleColumn = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleRow { get { return visibleRow; } internal set { visibleRow = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int RowHandle { get { return rowHandle; } set { rowHandle = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point Position { get { return position; } set { position = value; Location = position; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size Size {
			get { return base.Size; }
			set { PreferredSize = value; base.Size = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridRowCellState State { get { return state; } internal set { state = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ExpandButtonVisible {
			get { return base.ExpandButtonVisible; }
			set { base.ExpandButtonVisible = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ExpandOnDoubleClick {
			get { return base.ExpandOnDoubleClick; }
			set { base.ExpandOnDoubleClick = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool AllowBorderColorBlending {
			get { return base.AllowBorderColorBlending; }
			set { base.AllowBorderColorBlending = value; }
		}
		protected internal BaseLayoutItem GetItemAtPoint(Point pt) {
			DevExpress.XtraLayout.HitInfo.BaseLayoutItemHitInfo hi = GetLayoutItemHitInfo(pt);
			return (hi.Item != this) ? hi.Item : null;
		}
		protected internal DevExpress.XtraLayout.HitInfo.BaseLayoutItemHitInfo GetLayoutItemHitInfo(Point pt) {
			return CalcHitInfo(pt, true, false, true);
		}
		int disposeCounter = 0;
		protected internal bool IsDisposingInProgress {
			get { return disposeCounter > 0; }
		}
		protected override void Dispose(bool disposing) {
			disposeCounter++;
			try {
				DenyResetResizer();
				base.Dispose(disposing);
			}
			finally { disposeCounter--; }
		}
		protected internal void DenyResetResizer() {
			Accept(new GroupVisitor(DenyResetResizer));
		}
		class GroupVisitor : BaseVisitor {
			Action<LayoutGroup> action;
			public GroupVisitor(Action<LayoutGroup> action) {
				this.action = action;
			}
			public override void Visit(BaseLayoutItem item) {
				action(item as LayoutGroup);
			}
		}
		protected internal void UpdateChildrenToBeRefactored() {
			UpdateChildren(true);
		}
		protected override BaseLayoutItemViewInfo CreateViewInfo() {
			return new LayoutViewCardViewInfo(this);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LayoutViewCardViewInfo ViewInfo {
			get { return base.ViewInfo as LayoutViewCardViewInfo; }
		}
		bool allowChangeTextLocationOnExpanding = true;
		protected internal bool AllowChangeTextLocationOnExpanding {
			get { return allowChangeTextLocationOnExpanding; }
			set { allowChangeTextLocationOnExpanding = value; }
		}
		protected internal virtual Locations GetCollapsedCaptionLocation(Locations captionLocation) {
			if(captionLocation == Locations.Default || captionLocation == Locations.Top || captionLocation == Locations.Bottom) return Locations.Left;
			return captionLocation;
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("LayoutViewCardMaxSize")]
#endif
		public override Size MaxSize {
			get {
				if(!AllowChangeTextLocationOnExpanding) return base.MaxSize;
				if(Expanded) {
					return base.MaxSize;
				}
				else {
					Size size = DefaultMaxSize;
					if(TextLocation == Locations.Default || TextLocation == Locations.Top || TextLocation == Locations.Bottom)
						return new Size(MinSize.Width, size.Height);
					else
						return new Size(size.Width, MinSize.Height);
				}
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override object ImageList {
			get { return (View != null) ? View.Images : base.ImageList; }
		}
		protected internal virtual LayoutView View {
			get { return Owner as LayoutView; }
		}
		protected virtual bool IsTemplateCard {
			get { return (View != null) && (this == View.TemplateCard); }
		}
		protected override void FakeOwnerUpdate() {
			Resizer.UpdateConstraints();
			UpdateLayout();
		}
		internal void UpdateResizerConstraints() {
			Resizer.UpdateConstraints();
		}
		public override void Update() {
			if(IsTemplateCard) View.FireTemplateCardChanging();
			FakeOwnerUpdate();
			if(IsTemplateCard) View.FireTemplateCardChanged();
		}
		int? invalidateFromCard;
		public override void Invalidate() {
			if(IsTemplateCard) View.FireTemplateCardChanging();
			invalidateFromCard = RowHandle;
			base.Invalidate();
			invalidateFromCard = null;
			if(IsTemplateCard) View.FireTemplateCardChanged();
		}
		internal void InvalidateCore() {
			if(IsDisposing) return;
			if(invalidateFromCard.HasValue && (invalidateFromCard.Value != RowHandle))
				return;
			ViewInfo.Offset = Location;
			UpdateChildrenToBeRefactored();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override XtraEditors.ButtonPanel.BaseButtonCollection CustomHeaderButtons {
			get { return base.CustomHeaderButtons; }
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("LayoutViewCardMinSize")]
#endif
		public override Size MinSize {
			get {
				if(!AllowChangeTextLocationOnExpanding)
					return base.MinSize;
				if(Expanded)
					return base.MinSize;
				else {
					Size labelIndentionsSize = ViewInfo.AddLabelIndentions(Size.Empty);
					Size itemsSize = DefaultMinItemSize;
					if(TextLocation == Locations.Default || TextLocation == Locations.Top || TextLocation == Locations.Bottom) {
						return new Size(labelIndentionsSize.Width + 3, labelIndentionsSize.Height + itemsSize.Height);
					}
					else return base.MinSize;
				}
			}
		}
		bool defaultMinSizeIsAlreadyCalculated;
		Size defaultMinItemSizeCore = Size.Empty;
		protected override Size DefaultMinItemSize {
			get {
				if(View == null)
					return base.DefaultMinItemSize;
				if(!IsTemplateCard)
					return View.TemplateCard.DefaultMinItemSize;
				else {
					if(!defaultMinSizeIsAlreadyCalculated) {
						defaultMinItemSizeCore = CalcDefaultMinSize();
						defaultMinSizeIsAlreadyCalculated = true;
					}
					return defaultMinItemSizeCore;
				}
			}
		}
		public Size CalcDefaultMinSize() {
			return new Size(base.DefaultMinItemSize.Width, CalcDefaultMinHeight());
		}
		int CalcDefaultMinHeight() {
			if(View == null || View.ViewInfo == null || View.ViewInfo.GInfo == null || View.ViewInfo.GInfo.Graphics == null)
				return base.DefaultMinItemSize.Height;
			ViewInfo.BorderInfo.ButtonsPanel.ViewInfo.SetDirty();
			ViewInfo.BorderInfo.ButtonsPanel.BeginUpdate();
			ViewInfo.BorderInfo.ShowButton = true;
			ViewInfo.BorderInfo.ButtonsPanel.Buttons[0].Properties.Visible = true;
			ViewInfo.BorderInfo.ButtonsPanel.CancelUpdate();
			Size buttonsPanelSize = ViewInfo.BorderInfo.ButtonsPanel.ViewInfo.CalcMinSize(View.ViewInfo.GInfo.Graphics);
			GroupObjectPainter painter = GetCardBorderPainter();
			Margins contentMargins = painter.GetActualMargins(ViewInfo.BorderInfo);
			return contentMargins.Left + contentMargins.Right + (Expanded ? buttonsPanelSize.Width : buttonsPanelSize.Height);
		}
		GroupObjectPainter GetCardBorderPainter() {
			return ((LayoutViewCardPainter)((ILayoutControl)View).PaintStyle.GetPainter(this)).GetBorderPainter(ViewInfo) as GroupObjectPainter;
		}
		public void ResetDefaultMinSize() {
			defaultMinSizeIsAlreadyCalculated = false;
		}
		protected override Type GetDefaultWrapperType() {
			return typeof(LayoutViewCardWrapper);
		}
		public override string GetDefaultText() { return string.Empty; }
		#region IRowConditionFormatProvider Members
		ConditionInfo IRowConditionFormatProvider.ConditionInfo { get { return ConditionInfo; } }
		RowFormatRuleInfo IRowConditionFormatProvider.FormatInfo { get { return formatInfoCore; } }
		#endregion
		internal void ResetFormatInfo() {
			FormatInfo.Clear();
			ConditionInfo.Clear();
		}
	}
	public class LayoutViewCardWrapper : LayoutControlGroupWrapper {
		protected LayoutViewCard Card { get { return WrappedObject as LayoutViewCard; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceObject AppearanceItemCaption { get { return Item.AppearanceItemCaption; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceObject AppearanceGroup { get { return Group.AppearanceGroup; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowCustomizeChildren { get { return Group.AllowCustomizeChildren; } set { Group.AllowCustomizeChildren = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Enabled { get { return Group.Enabled; } set { Group.Enabled = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Expanded { get { return Group.Expanded; } set { Group.Expanded = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override LayoutVisibility Visibility { get { return Item.Visibility; } set { Item.Visibility = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowHide { get { return Item.AllowHide; } set { Item.AllowHide = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Point Location { get { return Item.Location; } set { Item.Location = value; } }
		public override Size Size { get { return Item.Size; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size MaxSize { get { return Item.MaxSize; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size MinSize { get { return Item.MinSize; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override DevExpress.XtraLayout.Utils.Padding Padding { get { return Item.Padding; } set { Item.Padding = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override DevExpress.XtraLayout.Utils.Padding Spacing { get { return Item.Spacing; } set { Item.Spacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override GroupElementLocation HeaderButtonsLocation { get { return Group.HeaderButtonsLocation; } set { Group.HeaderButtonsLocation = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ExpandButtonMode ExpandButtonMode { get { return Group.ExpandButtonMode; } set { Group.ExpandButtonMode = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool ExpandButtonVisible { get { return Group.ExpandButtonVisible; } set { Group.ExpandButtonVisible = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool ShowInCustomizationForm { get { return Item.ShowInCustomizationForm; } set { Item.ShowInCustomizationForm = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool GroupBordersVisible { get { return Group.GroupBordersVisible; } set { Group.GroupBordersVisible = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool TextVisible { get { return Item.TextVisible; } set { Item.TextVisible = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text { get { return Item.Text; } set { Item.Text = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Locations TextLocation { get { return Item.TextLocation; } set { Item.TextLocation = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size TextSize { get { return Item.TextSize; } set { Item.TextSize = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override int TextToControlDistance { get { return Item.TextToControlDistance; } set { Item.TextToControlDistance = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CustomizationFormText { get { return Item.CustomizationFormText; } set { Item.CustomizationFormText = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override BaseLayoutItemOptionsToolTip OptionsToolTip { get { return Item.OptionsToolTip; } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new LayoutViewCardWrapper();
		}
	}
	public class LayoutViewCardViewInfo : LayoutControlGroupViewInfo {
		public LayoutViewCardViewInfo(LayoutGroup owner)
			: base(owner) {
		}
		protected override ObjectInfoArgs CreateBorderInfo() {
			var borderInfo = new LayoutViewCardGroupObjectInfoArgs();
			borderInfo.SetButtonsPanelOwner(Owner);
			return borderInfo;
		}
		protected override void UpdateBorderInfo(DevExpress.Utils.Drawing.ObjectInfoArgs borderInfo) {
			base.UpdateBorderInfo(borderInfo);
			LayoutViewCard card = Owner as LayoutViewCard;
			GroupObjectInfoArgs tempBorderInfo = borderInfo as GroupObjectInfoArgs;
			if(!Owner.Expanded && card.AllowChangeTextLocationOnExpanding) {
				tempBorderInfo.CaptionLocation = card.GetCollapsedCaptionLocation(card.TextLocation);
			}
			if(card != null && card.View != null) {
				tempBorderInfo.BorderStyle = (!card.View.OptionsView.ShowCardCaption && !card.View.OptionsView.ShowCardBorderIfCaptionHidden) ?
					DevExpress.XtraEditors.Controls.BorderStyles.NoBorder :
					DevExpress.XtraEditors.Controls.BorderStyles.Default;
			}
		}
		protected new CardCaptionImageInfo ImageInfo {
			get { return (CardCaptionImageInfo)base.ImageInfo; }
		}
		protected override CaptionImageInfo CreateCaptionImageInfo() {
			return new CardCaptionImageInfo(Owner);
		}
		protected internal void UpdateBorderInternal(bool fMultiSelect, LayoutViewCardCaptionImageEventArgs e, bool allowHotTrackCards, bool isHot) {
			LayoutViewCard card = Owner as LayoutViewCard;
			UpdateCardCaptionImage(card, e);
			UpdateCardBorderState(card, fMultiSelect, allowHotTrackCards, isHot);
			UpdateBorder();
		}
		protected virtual void UpdateCardCaptionImage(LayoutViewCard card, LayoutViewCardCaptionImageEventArgs e) {
			ImageInfo.UpdateCustomParameters(e);
		}
		protected virtual void UpdateCardBorderState(LayoutViewCard card, bool fMultiSelect, bool allowHotTrackCards, bool isHot) {
			bool selected = ((card.State & GridRowCellState.Focused) != 0) || ((card.State & GridRowCellState.Selected) != 0);
			if(allowHotTrackCards) {
				if(selected)
					BorderInfo.State |= ObjectState.Selected;
				else
					BorderInfo.State &= ~ObjectState.Selected;
				if(isHot)
					BorderInfo.State |= ObjectState.Hot;
				else
					BorderInfo.State &= ~ObjectState.Hot;
			}
			else {
				BorderInfo.State = selected ? ObjectState.Selected : ObjectState.Normal;
				if(selected && card.View != null && card.View.IsFocusedView)
					BorderInfo.State ^= ObjectState.Hot;
			}
		}
		protected class CardCaptionImageInfo : CaptionImageInfo {
			public CardCaptionImageInfo(LayoutGroup owner)
				: base(owner) {
			}
			Image customImage;
			public override Image Image {
				get { return customImage ?? base.Image; }
			}
			GroupElementLocation? customImageLocation;
			public override GroupElementLocation ImageLocation {
				get { return customImageLocation.GetValueOrDefault(base.ImageLocation); }
			}
			bool? customImageVisibility;
			public override bool ImageVisible {
				get { return customImageVisibility.GetValueOrDefault(base.ImageVisible); }
			}
			internal void UpdateCustomParameters(LayoutViewCardCaptionImageEventArgs e) {
				this.customImage = null;
				if(e.Image != null)
					this.customImage = e.Image;
				else {
					bool fImageExist = ImageCollection.IsImageListImageExists(e.ImageList, e.ImageIndex);
					if(fImageExist)
						this.customImage = ImageCollection.GetImageListImage(e.ImageList, e.ImageIndex);
				}
				this.customImageLocation = e.CaptionImageLocation;
				this.customImageVisibility = e.CaptionImageVisible;
			}
		}
	}
	internal class BeginMeasureVisitor : BaseVisitor {
		public override void Visit(BaseLayoutItem item) {
			LayoutViewField field = item as LayoutViewField;
			if(field != null) {
				field.BeginMeasure();
			}
		}
	}
	internal class EndMeasureVisitor : BaseVisitor {
		public override void Visit(BaseLayoutItem item) {
			LayoutViewField field = item as LayoutViewField;
			if(field != null) {
				field.EditorPreferredWidth = field.ViewInfo.ClientArea.Width;
				field.EndMeasure();
			}
		}
	}
	[ToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	[Designer("DevExpress.XtraGrid.Design.LayoutViewFieldDesigner, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.ComponentModel.Design.IDesigner))]
	public class LayoutViewField : LayoutRepositoryItem {
		string columnNameCore = string.Empty;
		LayoutViewColumn columnCore = null;
		public LayoutViewField() : base() { }
		public LayoutViewField(RepositoryItem editor) : base(editor) { }
		[XtraSerializableProperty(), DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string TypeName {
			get { return "LayoutViewField"; }
		}
		int measuring = 0;
		protected override Size GetContentMinMaxSize(bool getMin) {
			if(measuring > 0) {
				int measureHeight = 20;
				if(this.RepositoryItem.GetType().Name == "RepositoryItemRichTextEdit") measureHeight = 40;
				return getMin ? new Size(0, measureHeight) : Size.Empty;
			}
			return base.GetContentMinMaxSize(getMin);
		}
		protected internal void BeginMeasure() {
			measuring++;
		}
		protected internal void EndMeasure() {
			measuring--;
		}
		protected internal void SetLayoutViewColumn(LayoutViewColumn column) { this.columnCore = column; }
		protected override void Dispose(bool disposing) {
			if(disposing) { this.columnCore = null; }
			base.Dispose(disposing);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LayoutViewColumn Column { get { return columnCore; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewFieldFieldName"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string FieldName {
			get { return columnCore != null ? columnCore.FieldName : ""; }
			set { if(columnCore != null) columnCore.FieldName = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty()]
		public string ColumnName {
			get {
				if(columnCore != null) return columnCore.Name;
				else return columnNameCore;
			}
			set {
				if(columnCore != null) columnCore.Name = value;
				columnNameCore = value;
			}
		}
		protected override DevExpress.XtraLayout.ViewInfo.BaseLayoutItemViewInfo CreateViewInfo() {
			return new LayoutViewFieldInfo(this);
		}
		protected override void XtraDeserializeText(XtraEventArgs e) {
			if(Column != null) return; 
			else base.XtraDeserializeText(e);
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewFieldText"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return columnCore != null ? GetFieldCaption() : string.Empty; }
			set {
				if(Column != null) {
					if(Column.Caption == value) return;
					Column.Caption = value;
					OnUpdateText();
				}
			}
		}
		internal void OnUpdateText() {
			this.SetShouldUpdateViewInfo();
			if(Owner is Designer.DesignerLayoutControl)
				UpdateText();
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowHide {
			get {
				bool allowHideColumn = (Column != null) && Column.OptionsColumn.AllowShowHide;
				return base.AllowHide && allowHideColumn;
			}
			set { base.AllowHide = value; }
		}
		private string GetFieldCaption() {
			LayoutView view = columnCore.View;
			if(view != null) return view.GetFieldCaption(columnCore);
			return columnCore.GetCaption();
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowInCustomizationForm {
			get { return columnCore != null ? columnCore.CanShowInCustomizationForm : true; }
			set { }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewFieldRepositoryItem"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RepositoryItem RepositoryItem {
			get {
				if(columnCore != null) {
					if(columnCore.ColumnEdit != null) return columnCore.ColumnEdit;
					else return base.RepositoryItem;
				}
				else return base.RepositoryItem;
			}
			set { base.RepositoryItem = value; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewFieldCustomizationFormText"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CustomizationFormText {
			get {
				if(columnCore != null) {
					return columnCore.GetCustomizationCaption();
				}
				return string.Empty;
			}
			set { if(columnCore != null) columnCore.CustomizationCaption = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Control Control { get { return base.Control; } set { base.Control = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ContentAlignment ControlAlignment { get { return base.ControlAlignment; } set { base.ControlAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size ControlMaxSize { get { return base.ControlMaxSize; } set { base.ControlMaxSize = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size ControlMinSize { get { return base.ControlMinSize; } set { base.ControlMinSize = value; } }
		object imagesCore = null;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object Images {
			get {
				if(imagesCore != null) return imagesCore;
				if(columnCore != null) return columnCore.Images;
				return null;
			}
			set { imagesCore = value; }
		}
		protected override Type GetDefaultWrapperType() {
			return typeof(LayoutViewFieldWrapper);
		}
		protected internal void PerformUpdateViewInfo() {
			SetShouldUpdateViewInfo();
		}
	}
	public class LayoutViewFieldWrapper : LayoutControlItemWrapper {
		protected new LayoutViewField Item { get { return WrappedObject as LayoutViewField; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceObject AppearanceItemCaption { get { return Item.AppearanceItemCaption; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ContentAlignment ControlAlignment { get { return Item.ControlAlignment; } set { Item.ControlAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DXCategory(CategoryName.Layout)]
		public override Size ControlMaxSize { get { return Item.ControlMaxSize; } set { Item.ControlMaxSize = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size ControlMinSize { get { return Item.ControlMinSize; } set { Item.ControlMinSize = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool ShowInCustomizationForm { get { return Item.ShowInCustomizationForm; } set { Item.ShowInCustomizationForm = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override LayoutVisibility Visibility { get { return Item.Visibility; } set { Item.Visibility = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowHide { get { return Item.AllowHide; } set { Item.AllowHide = value; } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new LayoutViewFieldWrapper();
		}
		[DXCategory(CategoryName.Appearance), RefreshProperties(RefreshProperties.All)]
		public string ColumnCaption {
			get { return (Item.Column != null) ? Item.Column.Caption : null; }
			set {
				if(Item.Column != null) {
					if(Item.Column.Caption == value) return;
					Item.Column.Caption = value;
					Item.OnUpdateText();
				}
			}
		}
		[ReadOnly(true)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
	}
	public class LayoutViewFieldInfo : LayoutRepositoryItemViewInfo {
		ObjectState stateCore = ObjectState.Normal;
		bool fIsReady = false;
		GridRowCellState fieldStateCore = GridRowCellState.Dirty;
		public Rectangle PopupActionArea {
			get { return CheckRTL(GetPopupActionArea(), GetButtonsArea()); }
		}
		public Rectangle SortButton {
			get { return CheckRTL(CalcSortButton(GetPopupActionArea(), GetButtonSize()), GetButtonsArea()); }
		}
		public Rectangle FilterButton {
			get { return CheckRTL(CalcFilterButton(GetPopupActionArea(), GetButtonSize()), GetButtonsArea()); }
		}
		Rectangle GetPopupActionArea() {
			return CalcSortFilterButtonArea(GetButtonsArea(), GetButtonSize());
		}
		Rectangle GetButtonsArea() {
			SortFilterButtonShowMode placement = CalcSortFilterButtonPlacement();
			if(placement == SortFilterButtonShowMode.Nowhere)
				return Rectangle.Empty;
			Rectangle bounds = RepositoryItemViewInfo.Bounds;
			if(placement == SortFilterButtonShowMode.InFieldCaption && Owner.TextVisible)
				bounds = TextAreaRelativeToControl;
			return bounds;
		}
		Rectangle CheckRTL(Rectangle r, Rectangle bounds) {
			if(IsRightToLeft)
				LayoutViewRTLHelper.UpdateRTLCore(ref r, bounds);
			return r;
		}
		protected Rectangle CalcSortButton(Rectangle buttons, Size btnSize) {
			return !buttons.IsEmpty && CanShowSortButton() ?
				new Rectangle(buttons.Right - btnSize.Width, buttons.Top, btnSize.Width, btnSize.Height) : Rectangle.Empty;
		}
		protected Rectangle CalcFilterButton(Rectangle buttons, Size btnSize) {
			if(buttons.IsEmpty || !CanShowFilterButton()) return Rectangle.Empty;
			if(!CanShowSortButton())
				return new Rectangle(buttons.Right - btnSize.Width, buttons.Top, btnSize.Width, btnSize.Height);
			return new Rectangle(buttons.Right - btnSize.Width * 2, buttons.Top, btnSize.Width, btnSize.Height);
		}
		protected Rectangle CalcSortFilterButtonArea(Rectangle bounds, Size btnSize) {
			SortFilterButtonLocation location = CalcSortFilterButtonLocation();
			int top = bounds.Top;
			int middle = bounds.Top + (bounds.Height - btnSize.Height) / 2;
			int bottom = bounds.Bottom - btnSize.Height;
			int left = bounds.Left + 2;
			int center = bounds.Left + bounds.Width / 2 - btnSize.Width;
			int right = bounds.Right - btnSize.Width * 2 - 2;
			switch(location) {
				case SortFilterButtonLocation.TopRight: return new Rectangle(right, top, btnSize.Width * 2, btnSize.Height);
				case SortFilterButtonLocation.TopLeft: return new Rectangle(left, top, btnSize.Width * 2, btnSize.Height);
				case SortFilterButtonLocation.TopCenter: return new Rectangle(center, top, btnSize.Width * 2, btnSize.Height);
				case SortFilterButtonLocation.MiddleLeft: return new Rectangle(left, middle, btnSize.Width * 2, btnSize.Height);
				case SortFilterButtonLocation.MiddleCenter: return new Rectangle(center, middle, btnSize.Width * 2, btnSize.Height);
				case SortFilterButtonLocation.MiddleRight: return new Rectangle(right, middle, btnSize.Width * 2, btnSize.Height);
				case SortFilterButtonLocation.BottomLeft: return new Rectangle(left, bottom, btnSize.Width * 2, btnSize.Height);
				case SortFilterButtonLocation.BottomCenter: return new Rectangle(center, bottom, btnSize.Width * 2, btnSize.Height);
				case SortFilterButtonLocation.BottomRight: return new Rectangle(right, bottom, btnSize.Width * 2, btnSize.Height);
				default:
					return new Rectangle(right, middle, btnSize.Width * 2, btnSize.Height);
			}
		}
		protected SortFilterButtonLocation CalcSortFilterButtonLocation() {
			SortFilterButtonLocation location = SortFilterButtonLocation.Default;
			if(Owner != null && Owner.Column != null) location = Owner.Column.OptionsField.SortFilterButtonLocation;
			return location;
		}
		protected SortFilterButtonShowMode CalcSortFilterButtonPlacement() {
			SortFilterButtonShowMode placement = SortFilterButtonShowMode.Default;
			if(Owner != null && Owner.Column != null) placement = Owner.Column.OptionsField.SortFilterButtonShowMode;
			return placement;
		}
		protected bool CanShowSortButton() {
			if(Owner != null && Owner.Column == null) return false;
			return Owner.Column.View.CanSortLayoutViewColumn(Owner.Column);
		}
		protected bool CanShowFilterButton() {
			if(Owner != null && Owner.Column == null) return false;
			return Owner.Column.View.CanFilterLayoutViewColumn(Owner.Column);
		}
		public LayoutViewFieldInfo(LayoutViewField owner)
			: base(owner) {
		}
		public new LayoutViewField Owner {
			get { return (LayoutViewField)base.Owner; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsReady { get { return fIsReady; } set { fIsReady = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridRowCellState GridRowCellState {
			get { return fieldStateCore; }
			set { fieldStateCore = value; }
		}
		public override ObjectState State {
			get {
				if(Owner != null) {
					if(Owner.Selected)
						return ObjectState.Selected;
					if(Owner.Column != null && !Owner.Column.AllowHotTrack)
						return stateCore & (~ObjectState.Hot);
				}
				return stateCore;
			}
			set { stateCore = value; }
		}
		Size GetButtonSize() {
			if(Owner != null && Owner.Column != null && Owner.Column.View != null) {
				return Owner.Column.View.ViewInfo.GetFieldButtonSize();
			}
			return new Size(17, 17);
		}
		protected internal void UpdateLabelAppearanceInternal(LayoutViewFieldCaptionImageEventArgs e) {
			Owner.Image = e.Image;
			Owner.ImageAlignment = e.ImageAlignment;
			Owner.ImageIndex = e.ImageIndex;
			Owner.Images = e.ImageList;
			Owner.ImageToTextDistance = e.ImageToTextDistance;
			UpdateAppearance();
		}
	}
}
