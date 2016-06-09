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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Rendering;
using DevExpress.Web.Data;
namespace DevExpress.Web {
	public abstract class GridViewBaseTemplateContainer : GridBaseTemplateContainer {
		public GridViewBaseTemplateContainer(ASPxGridView grid) : base(grid) { }
		public GridViewBaseTemplateContainer(ASPxGridView grid, int index, object dataItem)
			: base(grid, index, dataItem) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewBaseTemplateContainerGrid")]
#endif
		public new ASPxGridView Grid { get { return (ASPxGridView)base.Grid; } }
	}
	public class GridViewHeaderTemplateContainer : GridViewBaseTemplateContainer {
		GridHeaderLocation headerLocation;
		public GridViewHeaderTemplateContainer(GridViewColumn column, GridHeaderLocation headerLocation)
			: base(column.Grid, column.Grid.GetColumnGlobalIndex(column), column) {
			this.headerLocation = headerLocation;
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewHeaderTemplateContainerColumn")]
#endif
		public GridViewColumn Column { get { return DataItem as GridViewColumn; } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewHeaderTemplateContainerHeaderLocation")]
#endif
		public GridHeaderLocation HeaderLocation { get { return headerLocation; } }
		protected internal override string GetID() {
			string location = "";
			if (HeaderLocation == GridHeaderLocation.Group) {
				location = "G";
			}
			if (HeaderLocation == GridHeaderLocation.Customization) {
				location = "C";
			}
			return string.Format("header{0}{1}", location, Grid.GetColumnGlobalIndex(Column));
		}
	}
	public class GridViewFilterCellTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewFilterCellTemplateContainer(GridViewColumn column)
			: base(column.Grid, column.Grid.GetColumnGlobalIndex(column), column) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewFilterCellTemplateContainerColumn")]
#endif
		public GridViewColumn Column { get { return DataItem as GridViewColumn; } }
		protected internal override string GetID() {
			return string.Format("FC{0}", Grid.GetColumnGlobalIndex(Column));
		}
		protected override bool NeedLoadPostData { get { return true; } }
	}
	public class GridViewFilterRowTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewFilterRowTemplateContainer(ASPxGridView grid)
			: base(grid) {
		}
		protected internal override string GetID() {
			return "FR";
		}
		protected override bool NeedLoadPostData { get { return true; } }
	}
	public abstract class GridViewBaseRowTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewBaseRowTemplateContainer(ASPxGridView grid, object row, int visibleIndex)
			: base(grid, visibleIndex, row) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewBaseRowTemplateContainerKeyValue")]
#endif
		public virtual object KeyValue { get { return Grid.DataProxy.GetRowKeyValue(VisibleIndex); } }
		protected abstract string IdPrefix { get; }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected GridViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewBaseRowTemplateContainerVisibleIndex")]
#endif
		public int VisibleIndex { get { return ItemIndex; } }
		protected virtual string VisibleIndexPrefix { get { return VisibleIndex < 0 ? "new" : VisibleIndex.ToString(); } }
		protected override bool OnBubbleEvent(object source, EventArgs args) {
			CommandEventArgs cmdArgs = args as CommandEventArgs;
			if (cmdArgs != null) {
				object keyValue = null;
#pragma warning disable 168
				try { keyValue = KeyValue; }
				catch(System.Data.MissingPrimaryKeyException e) { };
#pragma warning restore 168
				RaiseBubbleEvent(this, new ASPxGridViewRowCommandEventArgs(VisibleIndex, keyValue, cmdArgs, source));
				return true;
			}
			return base.OnBubbleEvent(source, args);
		}
		protected internal override string GetID() {
			return IdPrefix + VisibleIndexPrefix;
		}
	}
	public class GridViewPreviewRowTemplateContainer : GridViewBaseRowTemplateContainer {
		public GridViewPreviewRowTemplateContainer(ASPxGridView grid, object row, int visibleIndex)
			: base(grid, row, visibleIndex) {
		}
		protected override string IdPrefix { get { return "pr"; } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewPreviewRowTemplateContainerText")]
#endif
		public string Text { get { return Grid.RenderHelper.TextBuilder.GetPreviewText(VisibleIndex); } }
	}
	public class GridViewDetailRowTemplateContainer : GridViewBaseRowTemplateContainer {
		bool bound = false;
		public GridViewDetailRowTemplateContainer(ASPxGridView grid, object row, int visibleIndex)
			: base(grid, row, visibleIndex) {
		}
		protected override string IdPrefix { get { return "dxdt"; } }
		public override void DataBind() {
			if (this.bound) return; 
			base.DataBind();
			this.bound = true;
		}
	}
	public class GridViewDataRowTemplateContainer : GridViewBaseRowTemplateContainer {
		public GridViewDataRowTemplateContainer(ASPxGridView grid, object row, int visibleIndex)
			: base(grid, row, visibleIndex) {
		}
		protected override string IdPrefix { get { return "row"; } }
	}
	public class GridViewDataItemTemplateContainer : GridViewDataRowTemplateContainer {
		GridViewDataColumn column;
		public GridViewDataItemTemplateContainer(ASPxGridView grid, object row, int visibleIndex, GridViewDataColumn column)
			: base(grid, row, visibleIndex) {
			this.column = column;
		}
		protected override string IdPrefix { get { return string.Empty; } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewDataItemTemplateContainerColumn")]
#endif
		public GridViewDataColumn Column { get { return column; } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewDataItemTemplateContainerText")]
#endif
		public string Text {
			get {
				string text = RenderHelper.TextBuilder.GetRowDisplayText(Column, VisibleIndex);
				return string.IsNullOrEmpty(text) ? "&nbsp;" : text;
			}
		}
		protected internal override string GetID() {
			return string.Format("cell{0}_{1}", VisibleIndexPrefix, Grid.GetColumnGlobalIndex(Column));
		}
	}
	public class GridViewEditItemTemplateContainer : GridViewDataItemTemplateContainer {
		public GridViewEditItemTemplateContainer(ASPxGridView grid, object row, int visibleIndex, GridViewDataColumn column)
			: base(grid, row, visibleIndex, column) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewEditItemTemplateContainerValidationGroup")]
#endif
		public string ValidationGroup { get { return Grid.EditTemplateValidationGroup; } }
		protected internal override string GetID() {
			return string.Format("edit{0}_{1}", VisibleIndexPrefix, Grid.GetColumnGlobalIndex(Column));
		}
		protected override bool NeedLoadPostData { get { return true; } }
	}
	public class GridViewBatchEditItemTemplateContainer : GridViewEditItemTemplateContainer {
		public GridViewBatchEditItemTemplateContainer(ASPxGridView grid, GridViewDataColumn column)
			: base(grid, null, -1, column) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewBatchEditItemTemplateContainerKeyValue")]
#endif
		public override object KeyValue { get { return null; } }
		protected override string VisibleIndexPrefix { get { return "B"; } }
		protected override bool NeedLoadPostData { get { return false; } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewBatchEditItemTemplateContainerText")]
#endif
		public new string Text { get { return string.Empty; } }
		protected override bool OnBubbleEvent(object source, EventArgs args) {
			return false;
		}
	}
	public class GridViewGroupRowTemplateContainer : GridViewBaseRowTemplateContainer {
		GridViewDataColumn column;
		public GridViewGroupRowTemplateContainer(ASPxGridView grid, GridViewDataColumn column, object row, int visibleIndex)
			: base(grid, row, visibleIndex) {
			this.column = column;
		}
		protected override string IdPrefix { get { return string.Empty; } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewGroupRowTemplateContainerKeyValue")]
#endif
		public override object KeyValue { get { return Grid.DataProxy.GetRowValue(VisibleIndex, Column.FieldName); } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewGroupRowTemplateContainerExpanded")]
#endif
		public bool Expanded { get { return DataProxy.IsRowExpanded(VisibleIndex); } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewGroupRowTemplateContainerColumn")]
#endif
		public GridViewDataColumn Column { get { return column; } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewGroupRowTemplateContainerGroupText")]
#endif
		public string GroupText { get { return RenderHelper.TextBuilder.GetGroupRowDisplayText(Column, VisibleIndex); } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewGroupRowTemplateContainerSummaryText")]
#endif
		public string SummaryText { get { return Grid.GetGroupRowSummaryText(VisibleIndex); } }
		protected internal override string GetID() {
			return string.Format("gr{0}_{1}", VisibleIndexPrefix, Grid.GetColumnGlobalIndex(Column));
		}
	}
	public class GridViewTitleTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewTitleTemplateContainer(ASPxGridView grid)
			: base(grid) {
		}
		protected override bool NeedLoadPostData { get { return true; } }
		protected internal override string GetID() { return "Title"; }
	}
	public class GridViewStatusBarTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewStatusBarTemplateContainer(ASPxGridView grid)
			: base(grid) {
		}
		protected override bool NeedLoadPostData { get { return true; } }
		protected internal override string GetID() { return "StatusBar"; }
	}
	public class GridViewPagerBarTemplateContainer : GridViewBaseTemplateContainer {
		GridViewPagerBarPosition position;
		string pagerId;
		public GridViewPagerBarTemplateContainer(ASPxGridView grid, GridViewPagerBarPosition position, string pagerId)
			: base(grid) {
			this.position = position;
			this.pagerId = pagerId;
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewPagerBarTemplateContainerPosition")]
#endif
		public GridViewPagerBarPosition Position { get { return position; } }
		protected internal string PagerId { get { return pagerId; } }
		protected override bool NeedLoadPostData { get { return true; } }
		protected string GetIDSuffix() { return Position == GridViewPagerBarPosition.Top ? "T" : "B"; }
		protected internal override string GetID() { return "PagerBar" + GetIDSuffix(); }
	}
	public class GridViewEmptyDataRowTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewEmptyDataRowTemplateContainer(ASPxGridView grid)
			: base(grid) {
		}
		protected internal override string GetID() { return "EmptyRow"; }
	}
	public class GridViewFooterRowTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewFooterRowTemplateContainer(ASPxGridView grid)
			: base(grid) {
		}
		protected internal override string GetID() { return "FooterRow"; }
	}
	public class GridViewFooterCellTemplateContainer : GridViewBaseTemplateContainer {
		public GridViewFooterCellTemplateContainer(GridViewColumn column)
			: base(column.Grid, column.Grid.GetColumnGlobalIndex(column), column) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewFooterCellTemplateContainerColumn")]
#endif
		public GridViewColumn Column { get { return DataItem as GridViewColumn; } }
		protected override bool NeedLoadPostData { get { return true; } }
		protected internal override string GetID() { return string.Format("footer{0}", Grid.GetColumnGlobalIndex(Column)); }
	}
	public class GridViewGroupFooterRowTemplateContainer : GridViewGroupRowTemplateContainer {
		public GridViewGroupFooterRowTemplateContainer(ASPxGridView grid, GridViewDataColumn column, object row, int visibleIndex)
			: base(grid, column, row, visibleIndex) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string GroupText { get { return base.GroupText; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string SummaryText { get { return base.SummaryText; } }
		protected internal override string GetID() { return string.Format("gfr{0}", VisibleIndex); }
	}
	public class GridViewGroupFooterCellTemplateContainer : GridViewGroupRowTemplateContainer {
		GridViewColumn column;
		public GridViewGroupFooterCellTemplateContainer(ASPxGridView grid, GridViewDataColumn groupedColumn, GridViewColumn column, object row, int visibleIndex)
			: base(grid, groupedColumn, row, visibleIndex) {
			this.column = column;
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewGroupFooterCellTemplateContainerGroupedColumn")]
#endif
		public GridViewDataColumn GroupedColumn { get { return base.Column; } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewGroupFooterCellTemplateContainerColumn")]
#endif
		public new GridViewColumn Column { get { return column; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string GroupText { get { return base.GroupText; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string SummaryText { get { return base.SummaryText; } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewGroupFooterCellTemplateContainerText")]
#endif
		public string Text { get { return RenderHelper.TextBuilder.GetGroupRowFooterText(Column, VisibleIndex); } }
		protected internal override string GetID() { return string.Format("gfc{0}_{1}", VisibleIndex, Grid.GetColumnGlobalIndex(Column)); }
	}
	public enum GridViewTemplateReplacementType { EditFormContent, EditFormCancelButton, EditFormUpdateButton, EditFormEditors, EditFormCellEditor, Pager }
	[DXWebToolboxItem(true), ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxGridViewTemplateReplacement.bmp")]
	public class ASPxGridViewTemplateReplacement : WebControl, IASPxWebControl, IStopLoadPostDataOnCallbackMarker {
		const string designTimeHtmlTemplate = "<table cellpadding=4 cellspacing=0 style=\"font:messagebox;color:buttontext;background-color:buttonface;border: solid 1px;border-top-color:buttonhighlight;border-left-color:buttonhighlight;border-bottom-color:buttonshadow;border-right-color:buttonshadow\"><tr><td nowrap><span style=\"font-weight:bold\">{0}</span> - {1}</td></tr><tr><td></td></tr></table>";
		const string OutOfTemplateExceptionMessage = "A control of type 'ASPxGridViewTemplateReplacement' can only be placed inside a ASPxGridView template.";
		const string InvalidReplacementTypeExceptionMessage = "A GridViewColumnLayoutItem template can only contain a ASPxGridViewTemplateReplacement control whose ReplacementType is set to EditFormCellEditor, EditFormUpdateButton or EditFormCancelButton.";
		string columnKey = string.Empty;
		GridViewBaseTemplateContainer container;
		GridViewTemplateReplacementType replacementType = GridViewTemplateReplacementType.EditFormContent;
		public ASPxGridViewTemplateReplacement() : base(HtmlTextWriterTag.Unknown) { }
#if !SL
	[DevExpressWebLocalizedDescription("ASPxGridViewTemplateReplacementReplacementType")]
#endif
		public GridViewTemplateReplacementType ReplacementType {
			get { return replacementType; }
			set { replacementType = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxGridViewTemplateReplacementColumnID")]
#endif
		public string ColumnID {
			get { return columnKey; }
			set { columnKey = value; }
		}
		protected GridViewBaseTemplateContainer Container { get { return container; } }
		protected override void Render(HtmlTextWriter writer) {
			EnsureChildControls();
			if(DesignMode) {
				writer.Write(string.Format(designTimeHtmlTemplate, GetType().Name, ID));
				return;
			}
			RenderContents(writer);
		}
		protected override void CreateChildControls() {
			this.container = null;
			if(DesignMode)
				return;
			this.container = FindTemplateContainer<GridViewBaseTemplateContainer>();
			if(Container == null)
				throw new InvalidOperationException(OutOfTemplateExceptionMessage);
			if(Container is GridViewEditFormLayoutItemTemplateContainer && !IsReplacementTypeValidForLayoutItemTemplate())
				throw new InvalidOperationException(InvalidReplacementTypeExceptionMessage);
			CreateStuffing();
			RenderUtils.EnsureChildControlsRecursive(this, false);
			base.CreateChildControls();
		}
		bool IsReplacementTypeValidForLayoutItemTemplate() {
			return ReplacementType == GridViewTemplateReplacementType.EditFormCellEditor
				|| ReplacementType == GridViewTemplateReplacementType.EditFormUpdateButton
				|| ReplacementType == GridViewTemplateReplacementType.EditFormCancelButton;
		}
		void CreateStuffing() {
			switch(ReplacementType) {
				case GridViewTemplateReplacementType.Pager:
					CreatePager();
					break;
				case GridViewTemplateReplacementType.EditFormContent:
					CreateEditorsTable(true);
					break;
				case GridViewTemplateReplacementType.EditFormEditors:
					CreateEditorsTable(false);
					break;
				case GridViewTemplateReplacementType.EditFormUpdateButton:
					CreateUpdateButton();
					break;
				case GridViewTemplateReplacementType.EditFormCancelButton:
					CreateCancelButton();
					break;
				case GridViewTemplateReplacementType.EditFormCellEditor:
					CreateCellEditor();
					break;
				default:
					throw new NotSupportedException();
			}			
		}
		void CreatePager() {
			GridViewPagerBarTemplateContainer pagerBarContainer = (GridViewPagerBarTemplateContainer)Container;
			Controls.Add(pagerBarContainer.Grid.CreatePagerControl(pagerBarContainer.PagerId));
		}
		void CreateEditorsTable(bool renderUpdateCancelButtons) {
			GridViewEditFormTemplateContainer editFormContainer = (GridViewEditFormTemplateContainer)Container;
			Controls.Add(new GridViewEditFormTable(editFormContainer.Grid.RenderHelper, editFormContainer.VisibleIndex, renderUpdateCancelButtons));
		}
		void CreateUpdateButton() {
			var postponeClick = Container is GridViewEditFormTemplateContainer && !Container.Grid.RenderHelper.RequireRenderEditFormPopup;
			GridViewCommandItemsCell.CreateUpdateButton(this, Container.Grid, postponeClick);
		}
		void CreateCancelButton() {
			var postponeClick = Container is GridViewEditFormTemplateContainer && !Container.Grid.RenderHelper.RequireRenderEditFormPopup;
			GridViewCommandItemsCell.CreateCancelButton(this, Container.Grid, postponeClick);
		}
		void CreateCellEditor() {
			GridViewBaseRowTemplateContainer editFormContainer = (GridViewBaseRowTemplateContainer)Container;
			ASPxGridView grid = editFormContainer.Grid;
			try {
				GridViewDataColumn column = grid.ColumnHelper.FindColumnByKey(ColumnID) as GridViewDataColumn;
				object editorValue = grid.DataProxy.GetEditingRowValue(editFormContainer.VisibleIndex, column.FieldName);
				ASPxEditBase editor = grid.RenderHelper.CreateGridEditor(column, editorValue, DevExpress.Web.EditorInplaceMode.EditForm, false);
				Controls.Add(editor);
				grid.RenderHelper.ApplyEditorSettings(editor, column);
				var e = grid.CreateCellEditorInitializeEventArgs(column, editFormContainer.VisibleIndex, editor, grid.DataProxy.EditingKeyValue, editorValue);
				grid.RaiseEditorInitialize(e);
			} catch {
				string msg = String.Format("[Replacement failed for column '{0}']", ColumnID);
				Controls.Add(RenderUtils.CreateLiteralControl(msg));
			}
		}
		T FindTemplateContainer<T>() where T : GridViewBaseTemplateContainer  {
			Control current = Parent;
			while(current != null) {
				T result = current as T;
				if(result != null)
					return result;
				current = current.Parent;
			}
			return null;
		}				
		void IASPxWebControl.EnsureChildControls() {
			EnsureChildControls();
		}
		void IASPxWebControl.PrepareControlHierarchy() {
		}
		protected override object SaveViewState() {
			return null;
		}
		protected override object SaveControlState() {
			return null;
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			EnsureChildControls();
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxGridViewTemplateReplacementClientID")]
#endif
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
		#region Hide non-usable props
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string AccessKey { get { return base.AccessKey; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override System.Drawing.Color BackColor { get { return base.BackColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EnableTheming { get { return base.EnableTheming; } set { base.EnableTheming = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor { get { return base.ForeColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height { get { return base.Height; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string SkinID { get { return base.SkinID; } set { base.SkinID = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override short TabIndex { get { return base.TabIndex; } }
		[Browsable(false), Localizable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string ToolTip { get { return base.ToolTip; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Visible { get { return base.Visible; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Width { get { return base.Width; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BorderColor { get { return base.BorderColor; } set { base.BorderColor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderStyle BorderStyle { get { return base.BorderStyle; } set { base.BorderStyle = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit BorderWidth { get { return base.BorderWidth; } set { base.BorderWidth = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssClass { get { return base.CssClass; } set { base.CssClass = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EnableViewState { get { return base.EnableViewState; } set { base.EnableViewState = value; } }
		#endregion
	}
	public class GridViewEditFormTemplateContainer : GridViewBaseRowTemplateContainer {
		public GridViewEditFormTemplateContainer(ASPxGridView grid, object row, int visibleIndex)
			: base(grid, row, visibleIndex) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewEditFormTemplateContainerValidationGroup")]
#endif
		public string ValidationGroup { get { return Grid.EditTemplateValidationGroup; } }
		protected override string IdPrefix { get { return "ef"; } }
		protected ASPxGridViewScripts Scripts { get { return Grid.RenderHelper.Scripts; } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewEditFormTemplateContainerCancelAction")]
#endif
		public string CancelAction { get { return Scripts.GetCancelEditFunction(); } }
#if !SL
	[DevExpressWebLocalizedDescription("GridViewEditFormTemplateContainerUpdateAction")]
#endif
		public string UpdateAction { get { return Scripts.GetUpdateEditFunction(); } }
		protected GridViewTableEditFormCell EditForm { get { return Parent as GridViewTableEditFormCell; } }
		protected override bool NeedLoadPostData { get { return true; } }
		public override void DataBind() {
			GridRenderHelper.EnsureTemplateReplacements(this);
			base.DataBind();
		}
	}
	public class GridViewEditFormLayoutItemTemplateContainer : GridViewEditItemTemplateContainer {
		public GridViewEditFormLayoutItemTemplateContainer(ASPxGridView grid, object row, int visibleIndex, GridViewDataColumn column, ColumnLayoutItem layoutItem)
			: base(grid, row, visibleIndex, column) {
			LayoutItem = layoutItem;
		}
		protected internal override string GetID() {
			return string.Format("li_{0}_row_{1}", LayoutItem.Path, VisibleIndexPrefix);
		}
		public ColumnLayoutItem LayoutItem { get; private set; }
		public override void DataBind() {
			GridRenderHelper.EnsureTemplateReplacements(this);
			base.DataBind();
		}
	}
	public class GridViewTemplates : PropertiesBase {
		ITemplate header, headerCaption, filterCell, filterRow, dataItem, groupRowContent,
			groupRow, dataRow, detailRow, previewRow, titlePanel, statusBar, pagerBar, editForm, emptyDataRow, footerRow, groupFooterRow, footerCell, groupFooterCell;
		public GridViewTemplates(IPropertiesOwner owner)
			: base(owner) {
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewHeaderTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate Header {
			get { return header; }
			set {
				if (Header == value) return;
				header = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewHeaderTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate HeaderCaption {
			get { return headerCaption; }
			set {
				if (HeaderCaption == value) return;
				headerCaption = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewFilterCellTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate FilterCell {
			get { return filterCell; }
			set {
				if(FilterCell == value)
					return;
				filterCell = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewFilterRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate FilterRow {
			get { return filterRow; }
			set {
				if(FilterRow == value)
					return;
				filterRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewDataItemTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate DataItem {
			get { return dataItem; }
			set {
				if (DataItem == value) return;
				dataItem = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewGroupRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate GroupRowContent {
			get { return groupRowContent; }
			set {
				if (GroupRowContent == value) return;
				groupRowContent = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewGroupRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate GroupRow {
			get { return groupRow; }
			set {
				if (GroupRow == value) return;
				groupRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewDataRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate DataRow {
			get { return dataRow; }
			set {
				if (DataRow == value) return;
				dataRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewDetailRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate DetailRow {
			get { return detailRow; }
			set {
				if (DetailRow == value) return;
				detailRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewPreviewRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate PreviewRow {
			get { return previewRow; }
			set {
				if (PreviewRow == value) return;
				previewRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewEmptyDataRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate EmptyDataRow {
			get { return emptyDataRow; }
			set {
				if (EmptyDataRow == value) return;
				emptyDataRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewFooterRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate FooterRow {
			get { return footerRow; }
			set {
				if (FooterRow == value) return;
				footerRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewGroupFooterRowTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate GroupFooterRow {
			get { return groupFooterRow; }
			set {
				if (GroupFooterRow == value) return;
				groupFooterRow = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewFooterCellTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate FooterCell {
			get { return footerCell; }
			set {
				if (FooterCell == value) return;
				footerCell = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewGroupFooterCellTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate GroupFooterCell {
			get { return groupFooterCell; }
			set {
				if(GroupFooterCell == value) return;
				groupFooterCell = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewTitleTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate TitlePanel {
			get { return titlePanel; }
			set {
				if (TitlePanel == value) return;
				titlePanel = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewStatusBarTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate StatusBar {
			get { return statusBar; }
			set {
				if (StatusBar == value) return;
				statusBar = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewPagerBarTemplateContainer)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate PagerBar {
			get { return pagerBar; }
			set {
				if (PagerBar == value) return;
				pagerBar = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(GridViewEditFormTemplateContainer), BindingDirection.TwoWay), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate EditForm {
			get { return editForm; }
			set {
				if (EditForm == value) return;
				editForm = value;
				TemplatesChanged();
			}
		}
		protected virtual void TemplatesChanged() {
			Changed();
		}
	}
}
