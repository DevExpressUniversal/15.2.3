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

namespace DevExpress.XtraVerticalGrid.Events {
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using DevExpress.XtraEditors.Repository;
	using DevExpress.XtraVerticalGrid.Rows;
	using DevExpress.XtraVerticalGrid.ViewInfo;
	using DevExpress.Utils;
	using DevExpress.Utils.Drawing;
	using DevExpress.Utils.Menu;
	using DevExpress.XtraVerticalGrid.Data;
	using DevExpress.XtraEditors.Design;
	public delegate void RowChangedEventHandler(object sender, RowChangedEventArgs e);
	public delegate void RowChangingEventHandler(object sender, RowChangingEventArgs e);
	public delegate void FocusedRowChangedEventHandler(object sender, FocusedRowChangedEventArgs e);
	public delegate void CustomizationFormCreatingCategoryEventHandler(object sender, CustomizationFormCreatingCategoryEventArgs e);
	public delegate void CustomizationFormDeletingCategoryEventHandler(object sender, CustomizationFormDeletingCategoryEventArgs e);
	public delegate void IndexChangedEventHandler(object sender, IndexChangedEventArgs e);
	public delegate void CustomDrawRowHeaderIndentEventHandler(object sender, CustomDrawRowHeaderIndentEventArgs e);
	public delegate void CustomDrawRowHeaderCellEventHandler(object sender, CustomDrawRowHeaderCellEventArgs e);
	public delegate void CustomDrawRowValueCellEventHandler(object sender, CustomDrawRowValueCellEventArgs e);
	public delegate void CustomDrawSeparatorEventHandler(object sender, CustomDrawSeparatorEventArgs e);
	public delegate void CustomDrawTreeButtonEventHandler(object sender, CustomDrawTreeButtonEventArgs e);
	public delegate void CellValueChangedEventHandler(object sender, CellValueChangedEventArgs e);
	public delegate void GetCustomRowCellStyleEventHandler(object sender, GetCustomRowCellStyleEventArgs e);
	public delegate void GetCustomRowCellEditEventHandler(object sender, GetCustomRowCellEditEventArgs e);
	public delegate void StartDragRowEventHandler(object sender, StartDragRowEventArgs e);
	public delegate void ProcessDragRowEventHandler(object sender, DragRowEventArgs e);
	public delegate void EndDragRowEventHandler(object sender, EndDragRowEventArgs e);
	public delegate void ValidateRecordEventHandler(object sender, ValidateRecordEventArgs e);
	public delegate void InvalidRecordExceptionEventHandler(object sender, InvalidRecordExceptionEventArgs e);
	public delegate void RecordIndexEventHandler(object sender, RecordIndexEventArgs e);
	public delegate void RecordObjectEventHandler(object sender, RecordObjectEventArgs e);
	public delegate void CustomDataEventHandler(object sender, CustomDataEventArgs e);
	[Obsolete("Use 'PopupMenuShowingEventHandler' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void VGridControlMenuEventHandler(object sender, VGridControlMenuEventArgs e);
	[Obsolete("Use 'PopupMenuShowingEventHandler' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void PropertyGridMenuEventHandler(object sender, PropertyGridMenuEventArgs e);
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	public delegate void CustomPropertyDescriptorsEventHandler(object sender, CustomPropertyDescriptorsEventArgs e);
	public delegate void UnboundExpressionEditorEventHandler(object sender, UnboundExpressionEditorEventArgs e);
	public class RowEventArgs : EventArgs {
		BaseRow row;
		public RowEventArgs(BaseRow row) {
			this.row = row;
		}
		public BaseRow Row { get { return row; } }
	}
	public class CategoryEventArgs : EventArgs {
		CategoryRow category;
		public CategoryEventArgs(CategoryRow category) {
			this.category = category;
		}
		public CategoryRow Category { get { return category; } }
	}
	public class CustomDataEventArgs : RowEventArgs {
		int listSourceRow;
		object value;
		bool isGetData;
		RowProperties rowProperties;
		public CustomDataEventArgs(BaseRow row, int listSourceRow, object value, bool isGetData, RowProperties rowProperties)
			: base(row) {
			this.listSourceRow = listSourceRow;
			this.value = value;
			this.isGetData = isGetData;
			this.rowProperties = rowProperties;
		}
		public RowProperties RowProperties { get { return rowProperties; } }
		public bool IsGetData { get { return isGetData; } }
		public bool IsSetData { get { return !IsGetData; } }
		public int ListSourceRowIndex { get { return listSourceRow; } }
		public object Value { get { return this.value; } set { this.value = value; } }
	}
	public class RowChangedEventArgs : RowEventArgs {
		RowChangeTypeEnum changeType;
		RowProperties prop;
		public RowChangedEventArgs(BaseRow row, RowProperties prop, RowChangeTypeEnum changeType) : base(row) {
			this.changeType = changeType;
			this.prop = prop;
		}
		public RowChangeTypeEnum ChangeType { get { return changeType; } }
		public RowProperties Properties { get { return prop; } }
	}
	public class RowChangingEventArgs : RowChangedEventArgs {
		object propertyValue;
		bool canChange;
		public RowChangingEventArgs(BaseRow row, RowProperties prop, RowChangeTypeEnum changeType, object propertyValue) : base(row, prop, changeType) {
			this.propertyValue = propertyValue;
			this.canChange = true;
		} 
		public object PropertyValue {
			get { return propertyValue; }
			set { 
				if(PropertyValue == null || value == null)
					propertyValue = value; 
				else {
					Type propType = PropertyValue.GetType();
					if(propType.IsAssignableFrom(value.GetType())) {
						if(!PropertyValue.Equals(value))
							propertyValue = value;
					}
				}
			}
		}
		public bool CanChange {
			get { return canChange; }
			set { canChange = value; }
		}
	}
	public class FocusedRowChangedEventArgs : RowEventArgs {
		BaseRow oldRow;
		public FocusedRowChangedEventArgs(BaseRow row, BaseRow oldRow) : base(row) {
			this.oldRow = oldRow;
		}
		public BaseRow OldRow { get { return oldRow; } }
	}
	public class CustomizationFormCreatingCategoryEventArgs : CategoryEventArgs {
		bool canCreate;
		public CustomizationFormCreatingCategoryEventArgs(CategoryRow category) : base(category) {
			this.canCreate = true;
		}
		public bool CanCreate {
			get { return canCreate; }
			set { canCreate = value; }
		}
	}
	public class CustomizationFormDeletingCategoryEventArgs : CategoryEventArgs {
		bool canDelete;
		public CustomizationFormDeletingCategoryEventArgs(CategoryRow category) : base(category) {
			this.canDelete = true;
		}
		public bool CanDelete {
			get { return canDelete; }
			set { canDelete = value; }
		}
	}
	public class PixelIndexChangedEventArgs : IndexChangedEventArgs {
		public PixelIndexChangedEventArgs(int newPixel, int oldPixel, int newIndex, int oldIndex) : base(newIndex, oldIndex) {
			NewPixel = newPixel;
			OldPixel = oldPixel;
		}
		public int NewPixel { get; protected set; }
		public int OldPixel { get; protected set; }
	}
	public class IndexChangedEventArgs : EventArgs {
		int newIndex;
		int oldIndex;
		public IndexChangedEventArgs(int newIndex, int oldIndex) {
			this.newIndex = newIndex;
			this.oldIndex = oldIndex;
		}
		public int NewIndex { get { return newIndex; } }
		public int OldIndex { get { return oldIndex; } }
	}
	public class CustomDrawEventArgs : EventArgs {
		GraphicsCache cache;
		Rectangle bounds;
		AppearanceObject appearance;
		bool handled;
		public CustomDrawEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance) {
			this.cache = cache;
			this.bounds = r;
			this.appearance = appearance;
			this.handled = false;
		}
		public Graphics Graphics { get { return Cache.Graphics; } }
		public GraphicsCache Cache { get { return cache; } }
		public Rectangle Bounds { get { return bounds; } }
		public AppearanceObject Appearance { get { return appearance; } }
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
	}
	public class CustomDrawRowEventArgs : CustomDrawEventArgs {
		RowProperties properties;
		BaseRow row;
		public CustomDrawRowEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, BaseRow row)
			: base(cache, r, appearance) {
				this.row = row;
		}
		public CustomDrawRowEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, RowProperties properties)
			: this(cache, r, appearance, properties != null ? properties.Row : null) {
				this.properties = properties;
		}
		public BaseRow Row { get { return row; } protected set { row = value; } }
		public RowProperties Properties { get { return properties; } }
	}
	public class CustomDrawRowHeaderIndentEventArgs : CustomDrawRowEventArgs {
		Indents rowIndents, categoryIndents;
		public CustomDrawRowHeaderIndentEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance) : base(cache, r, appearance, (BaseRow)null) {
			rowIndents = null;
			categoryIndents = null;
		}
		internal void Init(BaseRowHeaderInfo rh) {
			rowIndents = rh.RowIndents;
			categoryIndents = rh.CategoryIndents;
			Row = rh.Row;
		}
		public Indents RowIndents { get { return rowIndents; } }
		public Indents CategoryIndents { get { return categoryIndents; } }
	}
	public class CustomDrawTreeButtonEventArgs : CustomDrawRowEventArgs {
		TreeButtonType treeButtonType;
		public CustomDrawTreeButtonEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, BaseRow row, TreeButtonType treeButtonType) : base(cache, r, appearance, row) {
			this.treeButtonType = treeButtonType;
		}
		public bool Expanded { get { return Row.Expanded; } }
		public TreeButtonType TreeButtonType { get { return treeButtonType; } }
	}
	public class CustomDrawRowHeaderCellEventArgs : CustomDrawRowEventArgs {
		Rectangle imageRect, captionRect, sortShapeRect, filterRect, focusRect;
		string caption;
		bool pressed, hotTrack, focused;
		int imageIndex, cellIndex;
		public CustomDrawRowHeaderCellEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance) : base(cache, r, appearance, (BaseRow)null) {
			imageRect = captionRect = sortShapeRect = filterRect = focusRect = Rectangle.Empty;
			caption = string.Empty;
			pressed = hotTrack = focused = false;
			imageIndex = -1;
		}
		internal void Offset(Point point) {
			imageRect.Offset(point);
			captionRect.Offset(point);
			focusRect.Offset(point);
			sortShapeRect.Offset(point);
			filterRect.Offset(point);
		}
		internal void Init(RowCaptionInfo rc) {
			Row = rc.Row;
			caption = rc.Caption;
			pressed = rc.Pressed;
			focused = rc.Focused;
			hotTrack = rc.HotTrack;
			imageRect = rc.ImageRect;
			captionRect = rc.CaptionTextRect;
			focusRect = rc.FocusRect;
			sortShapeRect = rc.SortShapeRect;
			filterRect = rc.FilterButtonRect;
			cellIndex = rc.RowCellIndex;
			imageIndex = rc.ImageIndex;
			AllowGlyphSkinning = rc.AllowGlyphSkinning;
			AllowHtmlText = rc.AllowHtmlText;
		}
		protected object GridImageList {
			get {
				if(Row == null)
					return null;
				if(!Row.IsConnected)
					return null;
				return Row.Grid.ImageList;
			}
		}
		public Rectangle ImageRect { get { return imageRect; } }
		public Rectangle CaptionRect { get { return captionRect; } }
		public Rectangle FocusRect { get { return focusRect; } }
		public bool Pressed { get { return pressed; } }
		public bool HotTrack { get { return hotTrack; } }
		public bool Focused { get { return focused; } }
		public string Caption {
			get { return caption; }
			set { caption = value; }
		}
		public int CellIndex { get { return cellIndex; } }
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(!ImageCollection.IsImageListImageExists(GridImageList, value)) value = -1;
				imageIndex = value;
			}
		}
		public bool AllowGlyphSkinning { get; set; }
		public bool AllowHtmlText { get; set; }
	}
	public class CustomDrawRowValueCellEventArgs : CustomDrawRowEventArgs {
		int recordIndex, cellIndex;
		object cellValue;
		string cellText;
		bool enabled;
		public CustomDrawRowValueCellEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, 
			RowProperties properties, int recordIndex, int cellIndex, object cellValue, string cellText) 
			: base(cache, r, appearance, properties) {
			this.recordIndex = recordIndex;
			this.cellValue = cellValue;
			this.cellText = cellText;
			this.cellIndex = cellIndex;
			this.enabled = true;
			if(Row is EditorRow)
				enabled = (Row as EditorRow).Enabled;
		}
		public bool Enabled { get { return enabled; } }
		public int RecordIndex { get { return recordIndex; } }
		public object CellValue { get { return cellValue; }	}
		public string CellText { 
			get { return cellText; }
			set {
				if(value == null) value = string.Empty;
				cellText = value;
			}
		}
		public int CellIndex { get { return cellIndex; } }
	}
	public class CustomDrawSeparatorEventArgs : CustomDrawRowEventArgs {
		string separatorString;
		SeparatorKind separatorKind;
		int separatorIndex;
		bool isHeaderSeparator;
		StringFormat stringFormat;
		public CustomDrawSeparatorEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, MultiEditorRow row) : base(cache, r, appearance, row) {
			this.separatorString = string.Empty;
			this.separatorKind = SeparatorKind.VertLine;
			this.separatorIndex = -1;
			this.isHeaderSeparator = true;
			this.stringFormat = null;
		}
		internal void Init(SeparatorInfo si, int index, bool inHeader) {
			this.separatorString = si.SeparatorString;
			this.separatorKind = si.SeparatorKind;
			this.separatorIndex = index;
			this.isHeaderSeparator = inHeader;
			this.stringFormat = si.SeparatorFormat;
		}
		public string SeparatorString {
			get { return separatorString; }
			set {
				if(value == null) value = string.Empty;
				separatorString = value;
			}
		}
		public SeparatorKind SeparatorKind { get { return separatorKind; } }
		public int SeparatorIndex { get { return separatorIndex; } }
		public new MultiEditorRow Row { get { return base.Row as MultiEditorRow; } }
		public bool IsHeaderSeparator { get { return isHeaderSeparator; } }
	}
	public class CellValueChangedEventArgs : RowCellEventArgs {
		protected object val;
		public CellValueChangedEventArgs(BaseRow row, int recordIndex, int cellIndex, object val) : 
			base(row, recordIndex, cellIndex) {
			this.val = val;
		}
		public object Value { get { return val; } }
	}
	public class RowCellEventArgs : RowEventArgs {
		int recordIndex;
		int cellIndex;
		public RowCellEventArgs(BaseRow row, int recordIndex, int cellIndex) : base(row) {
			this.recordIndex = recordIndex;
			this.cellIndex = cellIndex;
		}
		public int RecordIndex { get { return recordIndex; } }
		public int CellIndex { get { return cellIndex; } }
	}
	public class GetCustomRowCellStyleEventArgs : RowCellEventArgs {
		AppearanceObject appearance;
		public GetCustomRowCellStyleEventArgs(BaseRow row, int recordIndex, int cellIndex, AppearanceObject appearance) : base(row, recordIndex, cellIndex) {
			this.appearance = appearance;
		}
		public AppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value != null)
					appearance = value;
			}
		}
	}
	public class GetCustomRowCellEditEventArgs : RowCellEventArgs {
		RepositoryItem repositoryItem;
		public GetCustomRowCellEditEventArgs(BaseRow row, int recordIndex, int cellIndex, RepositoryItem repositoryItem) : base(row, recordIndex, cellIndex) {
			this.repositoryItem = repositoryItem;
		}
		public RepositoryItem RepositoryItem {
			get { return repositoryItem; }
			set {
				if(value != null)
					repositoryItem = value;
			}
		}
	}
	public class DragRowEventArgs : RowEventArgs {
		Point screenPt;
		RowDragEffect effect;
		public DragRowEventArgs(BaseRow row, Point pt, RowDragEffect effect) : base(row) {
			this.screenPt = pt;
			this.effect = effect;
		}
		public Point ScreenLocation { get { return screenPt; } }
		public RowDragEffect Effect {
			get { return effect; }
			set { effect = value; }
		}
	}
	public class StartDragRowEventArgs : DragRowEventArgs {
		RowDragSource source;
		public StartDragRowEventArgs(BaseRow row, Point pt, RowDragEffect effect, RowDragSource source) : base(row, pt, effect) {
			this.source = source;
		}
		public RowDragSource Source { get { return source; } }
	}
	public class EndDragRowEventArgs : DragRowEventArgs {
		bool canceled;
		public EndDragRowEventArgs(BaseRow row, Point pt, RowDragEffect effect, bool canceled) : base(row, pt, effect) {
			this.canceled  = canceled;
		}
		public bool Canceled { get { return canceled; } }
	}
	public class RecordIndexEventArgs : EventArgs {
		int recordIndex;
		public RecordIndexEventArgs(int recordIndex) {
			this.recordIndex = recordIndex;
		}
		public int RecordIndex { get { return recordIndex; } }
	}
	public class RecordObjectEventArgs : RecordIndexEventArgs {
		object record;
		public RecordObjectEventArgs(int recordIndex, object record) : base(recordIndex) {
			this.record = record;
		}
		public object Record { get { return record; } }
	}
	public class ValidateRecordEventArgs : RecordIndexEventArgs {
		bool valid;
		string errorText;
		public ValidateRecordEventArgs(int recordIndex) : base(recordIndex) {
			this.errorText = string.Empty;
			this.valid = true;
		}
		public string ErrorText { get { return errorText; } set { errorText = value; } }
		public bool Valid { get { return valid; } set { valid = value; } }
	}
	public class InvalidRecordExceptionEventArgs : DevExpress.XtraEditors.Controls.ExceptionEventArgs {
		int recordIndex;
		public InvalidRecordExceptionEventArgs(Exception except, string errorText, int recordIndex) : base(errorText, except) {
			this.recordIndex = recordIndex;
		}
		public int RecordIndex { get { return recordIndex; } }
	}
	public class PopupMenuShowingEventArgs : EventArgs {
		DXPopupMenu menu;
		BaseRow row;
		public DXPopupMenu Menu { get { return menu; } }
		public BaseRow Row { get { return row; } }
		public PopupMenuShowingEventArgs(DXPopupMenu menu, BaseRow row) {
			this.menu = menu;
			this.row = row;
		}
	}
	[Obsolete("Use 'PopupMenuShowingEventArgs' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public class VGridControlMenuEventArgs : PopupMenuShowingEventArgs {
		public VGridControlMenuEventArgs(DXPopupMenu menu, BaseRow row)
			: base(menu, row) {
		}
	}
	[Obsolete("Use 'PopupMenuShowingEventArgs' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public class PropertyGridMenuEventArgs : VGridControlMenuEventArgs {
		public PropertyGridMenuEventArgs(DXPopupMenu menu, BaseRow row)
			: base(menu, row) {
		}
	}
	public class CustomPropertyDescriptorsEventArgs : EventArgs {
		object source;
		ITypeDescriptorContext context;
		Attribute[] attributes,
			copyAttributes;
		PropertyDescriptorCollection properties;
		public CustomPropertyDescriptorsEventArgs(object source, ITypeDescriptorContext context, Attribute[] attributes) {
			this.source = source;
			this.context = context;
			this.attributes = attributes;
		}
		public PropertyDescriptorCollection Properties {
			get {
				if(properties == null)
					properties = PropertyHelper.GetProperties(Source, Context, Attributes);
				return properties;
			}
			set { properties = value; }
		}
		public Attribute[] Attributes {
			get {
				if(copyAttributes == null) {
					copyAttributes = new Attribute[attributes.Length];
					this.attributes.CopyTo(copyAttributes, 0);
				}
				return copyAttributes;
			}
		}
		public ITypeDescriptorContext Context { get { return context; } }
		public object Source { get { return source; } }
	}
	public class UnboundExpressionEditorEventArgs : EventArgs {
		ExpressionEditorForm form;
		RowProperties properties;
		bool show = true;
		public UnboundExpressionEditorEventArgs(ExpressionEditorForm form, RowProperties properties) {
			this.form = form;
			this.properties = properties;
		}
		public ExpressionEditorForm ExpressionEditorForm { get { return form; } }
		public RowProperties Properties { get { return properties; } }
		public bool ShowExpressionEditor { get { return show; } set { show = value; } }
	}
}
