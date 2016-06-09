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
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.ViewInfo;
using System.Windows.Forms;
namespace DevExpress.XtraPivotGrid {
	#region Event Args
	public class PivotCustomDrawCellBaseEventArgs : PivotCellEventArgs {
		IThreadSafeAccessible threadSafeAccess;
		PivotCustomDrawCellBaseThreadSafeEventArgs threadSafeArgs;
		PivotCustomDrawCellBaseEventArgs internalUnsafeCopy;
		public PivotCustomDrawCellBaseEventArgs(IThreadSafeAccessible threadSafeAccess, PivotGridCellItem cellItem, PivotGridViewInfo viewInfo)
			: this(threadSafeAccess, cellItem, viewInfo, null) { }
		public PivotCustomDrawCellBaseEventArgs(IThreadSafeAccessible threadSafeAccess, PivotGridCellItem cellItem, PivotGridViewInfo viewInfo, Rectangle? bounds)
			: base(cellItem, viewInfo, bounds) {
			this.threadSafeAccess = threadSafeAccess;
		}
		public PivotCustomDrawCellBaseThreadSafeEventArgs ThreadSafeArgs {
			get {
				if(threadSafeArgs == null)
					threadSafeArgs = CreateThreadSafeEventArgs();
				return threadSafeArgs;
			}
		}
		protected bool HasThreadSafeArgs { get { return threadSafeArgs != null; } }
		protected IThreadSafeAccessible ThreadSafeAccess {
			get { return threadSafeAccess; }
		}
		protected PivotCustomDrawCellBaseEventArgs InternalUnsafeCopy {
			get {
				if(internalUnsafeCopy == null)
					internalUnsafeCopy = CreateInternalUnsafeCopy();
				return internalUnsafeCopy;
			}
		}
		protected virtual PivotCustomDrawCellBaseEventArgs CreateInternalUnsafeCopy() {
			return new PivotCustomDrawCellBaseEventArgsInternal(ThreadSafeAccess, Item, ViewInfo);
		}
		protected virtual PivotCustomDrawCellBaseThreadSafeEventArgs CreateThreadSafeEventArgs() {
			return new PivotCustomDrawCellBaseThreadSafeEventArgs(ThreadSafeAccess, InternalUnsafeCopy);
		}
		protected virtual bool AllowUnsafeAccess { get { return false; } }
		protected void BeforeArgAccess() {
			if(AllowUnsafeAccess)
				return;
			ThrowExceptionIfAsyncIsInProgress();
		}
		void ThrowExceptionIfAsyncIsInProgress() {
			if(ThreadSafeAccess.IsAsyncInProgress)
				throw new PivotCustomDrawUnsafeAccessException();
		}
		public new bool Focused {
			get {
				BeforeArgAccess();
				return base.Focused;
			}
		}
		public new bool Selected {
			get {
				BeforeArgAccess();
				return base.Selected;
			}
		}
		public new string DisplayText {
			get {
				BeforeArgAccess();
				return base.DisplayText;
			}
		}
		public new Rectangle Bounds {
			get {
				BeforeArgAccess();
				PivotCellViewInfo CellViewInfo = Item as PivotCellViewInfo;
				if(CellViewInfo != null) {
					if(ViewInfo.IsHorzScrollControl || ViewInfo.Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree)
						return CellViewInfo.Bounds;
					return CellViewInfo.PaintBounds;
				} else {
					return base.Bounds;
				}
			}
		}
		public new PivotGridField DataField {
			get {
				BeforeArgAccess();
				return base.DataField;
			}
		}
		public new int ColumnIndex {
			get {
				BeforeArgAccess();
				return base.ColumnIndex;
			}
		}
		public new int RowIndex {
			get {
				BeforeArgAccess();
				return base.RowIndex;
			}
		}
		public new int ColumnFieldIndex {
			get {
				BeforeArgAccess();
				return Item.ColumnFieldIndex;
			}
		}
		public new int RowFieldIndex {
			get {
				BeforeArgAccess();
				return base.RowFieldIndex;
			}
		}
		public new PivotGridField ColumnField {
			get {
				BeforeArgAccess();
				return base.ColumnField;
			}
		}
		public new PivotGridField RowField {
			get {
				BeforeArgAccess();
				return base.RowField;
			}
		}
		public new object Value {
			get {
				BeforeArgAccess();
				return base.Value;
			}
		}
		public new PivotSummaryType SummaryType {
			get {
				BeforeArgAccess();
				return base.SummaryType;
			}
		}
		public new PivotSummaryValue SummaryValue {
			get {
				BeforeArgAccess();
				return base.SummaryValue;
			}
		}
		public new PivotGridValueType ColumnValueType {
			get {
				BeforeArgAccess();
				return base.ColumnValueType;
			}
		}
		public new PivotGridValueType RowValueType {
			get {
				BeforeArgAccess();
				return base.RowValueType;
			}
		}
		public new PivotGridCustomTotal ColumnCustomTotal {
			get {
				BeforeArgAccess();
				return base.ColumnCustomTotal;
			}
		}
		public new PivotGridCustomTotal RowCustomTotal {
			get {
				BeforeArgAccess();
				return base.RowCustomTotal;
			}
		}
		public new PivotDrillDownDataSource CreateDrillDownDataSource() {
			BeforeArgAccess();
			return base.CreateDrillDownDataSource();
		}
		public new PivotDrillDownDataSource CreateDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			BeforeArgAccess();
			return base.CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		public new PivotDrillDownDataSource CreateDrillDownDataSource(List<string> customColumns) {
			BeforeArgAccess();
			return base.CreateDrillDownDataSource(customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public new PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public new PivotDrillDownDataSource CreateOLAPDrillDownDataSource(List<string> customColumns) {
			return CreateDrillDownDataSource(customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public new PivotDrillDownDataSource CreateServerModeDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public new PivotDrillDownDataSource CreateServerModeDrillDownDataSource(List<string> customColumns) {
			return CreateDrillDownDataSource(customColumns);
		}
		public new PivotSummaryDataSource CreateSummaryDataSource() {
			BeforeArgAccess();
			return base.CreateSummaryDataSource();
		}
		public new object GetFieldValue(PivotGridField field) {
			BeforeArgAccess();
			return base.GetFieldValue(field);
		}
		public new object GetFieldValue(PivotGridField field, int cellIndex) {
			BeforeArgAccess();
			return base.GetFieldValue(field, cellIndex);
		}
		public new bool IsOthersFieldValue(PivotGridField field) {
			BeforeArgAccess();
			return base.IsOthersFieldValue(field);
		}
		public new bool IsFieldValueExpanded(PivotGridField field) {
			BeforeArgAccess();
			return base.IsFieldValueExpanded(field);
		}
		public new bool IsFieldValueRetrievable(PivotGridField field) {
			BeforeArgAccess();
			return base.IsFieldValueRetrievable(field);
		}
		public new PivotGridField[] GetColumnFields() {
			BeforeArgAccess();
			return base.GetColumnFields();
		}
		public new PivotGridField[] GetRowFields() {
			BeforeArgAccess();
			return base.GetRowFields();
		}
		public new object GetCellValue(PivotGridField dataField) {
			BeforeArgAccess();
			return base.GetCellValue(dataField);
		}
		public new object GetCellValue(object[] columnValues, object[] rowValues, PivotGridField dataField) {
			BeforeArgAccess();
			return base.GetCellValue(columnValues, rowValues, dataField);
		}
		public new object GetCellValue(int columnIndex, int rowIndex) {
			BeforeArgAccess();
			return base.GetCellValue(columnIndex, rowIndex);
		}
		public new object GetPrevRowCellValue(PivotGridField dataField) {
			BeforeArgAccess();
			return base.GetPrevRowCellValue(dataField);
		}
		public new object GetNextRowCellValue(PivotGridField dataField) {
			BeforeArgAccess();
			return base.GetNextRowCellValue(dataField);
		}
		public new object GetPrevColumnCellValue(PivotGridField dataField) {
			BeforeArgAccess();
			return base.GetPrevColumnCellValue(dataField);
		}
		public new object GetNextColumnCellValue(PivotGridField dataField) {
			BeforeArgAccess();
			return base.GetNextColumnCellValue(dataField);
		}
		public new object GetColumnGrandTotal(PivotGridField dataField) {
			BeforeArgAccess();
			return base.GetColumnGrandTotal(dataField);
		}
		public new object GetColumnGrandTotal(object[] rowValues, PivotGridField dataField) {
			BeforeArgAccess();
			return base.GetColumnGrandTotal(rowValues, dataField);
		}
		public new object GetRowGrandTotal(PivotGridField dataField) {
			BeforeArgAccess();
			return base.GetRowGrandTotal(dataField);
		}
		public new object GetRowGrandTotal(object[] columnValues, PivotGridField dataField) {
			BeforeArgAccess();
			return base.GetRowGrandTotal(columnValues, dataField);
		}
		public new object GetGrandTotal(PivotGridField dataField) {
			BeforeArgAccess();
			return base.GetGrandTotal(dataField);
		}
	}
	public class PivotCustomDrawCellEventArgs : PivotCustomDrawCellBaseEventArgs {
		AppearanceObject appearance;
		ViewInfoPaintArgs paintArgs;
		bool handle;
		MethodInvoker defaultDraw;
		public PivotCustomDrawCellEventArgs(IThreadSafeAccessible threadSafeAccess, PivotGridCellItem cellItem, AppearanceObject appearance,
			ViewInfoPaintArgs paintArgs, PivotGridViewInfo viewInfo, MethodInvoker defaultDraw)
			: base(threadSafeAccess, cellItem, viewInfo) {
			this.appearance = appearance;
			this.paintArgs = paintArgs;
			this.handle = false;
			this.defaultDraw = defaultDraw;
		}
		public void DefaultDraw() {
			if(defaultDraw != null && !Handled) {
				Handled = true;
				defaultDraw();
			}
		}
		protected override PivotCustomDrawCellBaseThreadSafeEventArgs CreateThreadSafeEventArgs() {
			return new PivotCustomDrawCellThreadSafeEventArgs(ThreadSafeAccess, InternalUnsafeCopy);
		}
		protected new PivotCustomDrawCellEventArgsInternal InternalUnsafeCopy {
			get { return (PivotCustomDrawCellEventArgsInternal)base.InternalUnsafeCopy; }
		}
		protected override PivotCustomDrawCellBaseEventArgs CreateInternalUnsafeCopy() {
			return new PivotCustomDrawCellEventArgsInternal(ThreadSafeAccess, Item, AppearanceInternal, PaintArgsInternal, ViewInfo, defaultDraw);
		}
		AppearanceObject AppearanceInternal { get { return appearance; } }
		ViewInfoPaintArgs PaintArgsInternal { get { return paintArgs; } }
		public new PivotCustomDrawCellThreadSafeEventArgs ThreadSafeArgs {
			get { return (PivotCustomDrawCellThreadSafeEventArgs)base.ThreadSafeArgs; }
		}
		public bool Handled {
			get {
				if(HasThreadSafeArgs)
					handle |= ThreadSafeArgs.Handled;
				return handle;
			}
			set {
				BeforeArgAccess();
				handle = value;
			}
		}
		public AppearanceObject Appearance {
			get {
				if(HasThreadSafeArgs)
					appearance = ThreadSafeArgs.Appearance;
				return appearance;
			}
			set {
				BeforeArgAccess();
				if(value == null)
					return;
				appearance = value;
			}
		}
		public Graphics Graphics {
			get {
				BeforeArgAccess();
				return paintArgs.Graphics;
			}
		}
		public GraphicsCache GraphicsCache {
			get {
				BeforeArgAccess();
				return paintArgs.GraphicsCache;
			}
		}
	}
	public class PivotCustomAppearanceEventArgs : PivotCustomDrawCellBaseEventArgs {
		AppearanceObject appearance;
		public PivotCustomAppearanceEventArgs(IThreadSafeAccessible threadSafeAccess, PivotGridCellItem cellItem,
			AppearanceObject appearance, PivotGridViewInfo viewInfo, Rectangle? bounds)
			: base(threadSafeAccess, cellItem, viewInfo, bounds) {
			this.appearance = appearance;
		}
		protected override PivotCustomDrawCellBaseThreadSafeEventArgs CreateThreadSafeEventArgs() {
			return new PivotCustomAppearanceThreadSafeEventArgs(ThreadSafeAccess, InternalUnsafeCopy);
		}
		protected new PivotCustomAppearanceEventArgs InternalUnsafeCopy {
			get { return (PivotCustomAppearanceEventArgsInternal)base.InternalUnsafeCopy; }
		}
		protected override PivotCustomDrawCellBaseEventArgs CreateInternalUnsafeCopy() {
			return new PivotCustomAppearanceEventArgsInternal(ThreadSafeAccess, Item, AppearanceInternal, ViewInfo, bounds);
		}
		AppearanceObject AppearanceInternal { get { return appearance; } }
		public new PivotCustomAppearanceThreadSafeEventArgs ThreadSafeArgs {
			get { return (PivotCustomAppearanceThreadSafeEventArgs)base.ThreadSafeArgs; }
		}
		public AppearanceObject Appearance {
			get {
				if(HasThreadSafeArgs)
					appearance = ThreadSafeArgs.Appearance;
				return appearance;
			}
			set {
				BeforeArgAccess();
				if(value == null)
					return;
				appearance = value;
			}
		}
	}
	public interface IPivotCustomDrawAppearanceOwner {
		AppearanceObject Appearance { get; set; }
	}
	public class PivotCustomDrawEventArgs : EventArgs {
		IThreadSafeAccessible threadSafeAccess;
		PivotCustomDrawBaseThreadSafeEventArgs threadSafeArgs;
		PivotCustomDrawEventArgs internalUnsafeCopy;
		IPivotCustomDrawAppearanceOwner appearanceOwner;
		ViewInfoPaintArgs paintArgs;
		Rectangle bounds;
		bool handled;
		MethodInvoker defaultDraw;
		public PivotCustomDrawEventArgs(IThreadSafeAccessible threadSafeAccess, IPivotCustomDrawAppearanceOwner appearanceOwner,
							ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw) {
			this.threadSafeAccess = threadSafeAccess;
			this.appearanceOwner = appearanceOwner;
			this.paintArgs = paintArgs;
			this.bounds = bounds;
			this.handled = false;
			this.defaultDraw = defaultDraw;
		}
		public void DefaultDraw() {
			BeforeArgAccess();
			if(defaultDraw != null && !Handled) {
				Handled = true;
				defaultDraw();
			}
		}
		protected IThreadSafeAccessible ThreadSafeAccess {
			get { return threadSafeAccess; }
		}
		protected PivotCustomDrawEventArgs InternalUnsafeCopy {
			get {
				if(internalUnsafeCopy == null)
					internalUnsafeCopy = CreateInternalUnsafeCopy();
				return internalUnsafeCopy;
			}
		}
		protected virtual PivotCustomDrawEventArgs CreateInternalUnsafeCopy() {
			return new PivotCustomDrawEventArgsInternal(ThreadSafeAccess, AppearanceOwnerInternal, PaintArgsInternal, BoundsInternal, defaultDraw);
		}
		protected virtual PivotCustomDrawBaseThreadSafeEventArgs CreateThreadSafeEventArgs() {
			return new PivotCustomDrawThreadSafeEventArgs(ThreadSafeAccess, InternalUnsafeCopy);
		}
		protected PivotCustomDrawBaseThreadSafeEventArgs ThreadSafeArgsInternal {
			get {
				if(threadSafeArgs == null)
					threadSafeArgs = CreateThreadSafeEventArgs();
				return threadSafeArgs;
			}
		}
		protected bool HasThreadSaveArgs { get { return threadSafeArgs != null; } }
		protected virtual bool AllowUnsafeAccess { get { return false; } }
		protected void BeforeArgAccess() {
			if(AllowUnsafeAccess)
				return;
			ThrowExceptionIfAsyncIsInProgress();
		}
		void ThrowExceptionIfAsyncIsInProgress() {
			if(ThreadSafeAccess.IsAsyncInProgress)
				throw new PivotCustomDrawUnsafeAccessException();
		}
		public PivotCustomDrawThreadSafeEventArgs ThreadSafeArgs {
			get { return (PivotCustomDrawThreadSafeEventArgs)ThreadSafeArgsInternal; }
		}
		protected IPivotCustomDrawAppearanceOwner AppearanceOwnerInternal { get { return appearanceOwner; } }
		protected ViewInfoPaintArgs PaintArgsInternal { get { return paintArgs; } }
		protected Rectangle BoundsInternal { get { return bounds; } }
		public bool Handled {
			get {
				if(HasThreadSaveArgs)
					handled |= ThreadSafeArgs.Handled;
				return handled;
			}
			set {
				BeforeArgAccess();
				handled = value;
			}
		}
		public AppearanceObject Appearance {
			get {
				BeforeArgAccess();
				return appearanceOwner.Appearance;
			}
			set {
				BeforeArgAccess();
				if(value == null)
					return;
				appearanceOwner.Appearance = value;
			}
		}
		public Graphics Graphics {
			get {
				BeforeArgAccess();
				return paintArgs.Graphics;
			}
		}
		public GraphicsCache GraphicsCache {
			get {
				BeforeArgAccess();
				return paintArgs.GraphicsCache;
			}
		}
		public Rectangle Bounds {
			get {
				BeforeArgAccess();
				return bounds;
			}
		}
	}
	public class PivotCustomDrawHeaderAreaEventArgs : PivotCustomDrawEventArgs {
		PivotHeadersViewInfoBase headersViewInfo;
		public PivotCustomDrawHeaderAreaEventArgs(IThreadSafeAccessible threadSafeAccess, PivotHeadersViewInfoBase headersViewInfo,
										ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw)
			: base(threadSafeAccess, headersViewInfo, paintArgs, bounds, defaultDraw) {
			this.headersViewInfo = headersViewInfo;
		}
		protected override PivotCustomDrawBaseThreadSafeEventArgs CreateThreadSafeEventArgs() {
			return new PivotCustomDrawHeaderAreaThreadSafeEventArgs(ThreadSafeAccess, InternalUnsafeCopy);
		}
		protected new PivotCustomDrawHeaderAreaEventArgs InternalUnsafeCopy {
			get { return (PivotCustomDrawHeaderAreaEventArgsInternal)base.InternalUnsafeCopy; }
		}
		protected override PivotCustomDrawEventArgs CreateInternalUnsafeCopy() {
			return new PivotCustomDrawHeaderAreaEventArgsInternal(ThreadSafeAccess, HeadersViewInfo, PaintArgsInternal, BoundsInternal, () => DefaultDraw());
		}
		public new PivotCustomDrawHeaderAreaThreadSafeEventArgs ThreadSafeArgs {
			get { return (PivotCustomDrawHeaderAreaThreadSafeEventArgs)base.ThreadSafeArgs; }
		}
		PivotHeadersViewInfoBase HeadersViewInfo { get { return headersViewInfo; } }
		public PivotArea Area {
			get {
				BeforeArgAccess();
				return headersViewInfo.Area;
			}
		}
	}
	public class PivotCustomDrawFieldHeaderEventArgs : PivotCustomDrawEventArgs {
		PivotHeaderViewInfoBase headerViewInfo;
		HeaderObjectPainter painter;
		public PivotCustomDrawFieldHeaderEventArgs(IThreadSafeAccessible threadSafeAccess, PivotHeaderViewInfoBase headerViewInfo,
									ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter, MethodInvoker defaultDraw)
			: base(threadSafeAccess, headerViewInfo, paintArgs, headerViewInfo.InfoArgs.Bounds, defaultDraw) {
			this.headerViewInfo = headerViewInfo;
			this.painter = painter;
		}
		protected override PivotCustomDrawBaseThreadSafeEventArgs CreateThreadSafeEventArgs() {
			return new PivotCustomDrawFieldHeaderThreadSafeEventArgs(ThreadSafeAccess, InternalUnsafeCopy);
		}
		protected new PivotCustomDrawFieldHeaderEventArgs InternalUnsafeCopy {
			get { return (PivotCustomDrawFieldHeaderEventArgsInternal)base.InternalUnsafeCopy; }
		}
		protected override PivotCustomDrawEventArgs CreateInternalUnsafeCopy() {
			return new PivotCustomDrawFieldHeaderEventArgsInternal(ThreadSafeAccess, HeaderViewInfo, PaintArgsInternal, PainterInternal, () => DefaultDraw());
		}
		public new PivotCustomDrawFieldHeaderThreadSafeEventArgs ThreadSafeArgs {
			get { return (PivotCustomDrawFieldHeaderThreadSafeEventArgs)base.ThreadSafeArgs; }
		}
		internal PivotFieldItem FieldItem { get { return headerViewInfo.Field; } }
		PivotHeaderViewInfoBase HeaderViewInfo { get { return headerViewInfo; } }
		protected HeaderObjectPainter PainterInternal { get { return painter; } }
		public PivotGridField Field {
			get {
				BeforeArgAccess();
				return headerViewInfo.OriginalField;
			}
		}
		public HeaderObjectInfoArgs Info {
			get {
				BeforeArgAccess();
				return headerViewInfo.InfoArgs;
			}
		}
		public HeaderObjectPainter Painter {
			get {
				BeforeArgAccess();
				return painter;
			}
		}
	}
	public class PivotCustomDrawFieldValueEventArgs : PivotCustomDrawEventArgs {
		PivotFieldsAreaCellViewInfo fieldCellViewInfo;
		PivotHeaderObjectInfoArgs info;
		PivotHeaderObjectPainter painter;
		public PivotCustomDrawFieldValueEventArgs(IThreadSafeAccessible threadSafeAccess, PivotFieldsAreaCellViewInfo fieldCellViewInfo,
								PivotHeaderObjectInfoArgs info, ViewInfoPaintArgs paintArgs, PivotHeaderObjectPainter painter, MethodInvoker defaultDraw)
			: base(threadSafeAccess, fieldCellViewInfo, paintArgs, info.Bounds, defaultDraw) {
			this.fieldCellViewInfo = fieldCellViewInfo;
			this.info = info;
			this.painter = painter;
		}
		protected override PivotCustomDrawBaseThreadSafeEventArgs CreateThreadSafeEventArgs() {
			return new PivotCustomDrawFieldValueThreadSafeEventArgs(ThreadSafeAccess, InternalUnsafeCopy);
		}
		protected new PivotCustomDrawFieldValueEventArgs InternalUnsafeCopy {
			get { return (PivotCustomDrawFieldValueEventArgsInternal)base.InternalUnsafeCopy; }
		}
		protected override PivotCustomDrawEventArgs CreateInternalUnsafeCopy() {
			return new PivotCustomDrawFieldValueEventArgsInternal(ThreadSafeAccess, FieldCellViewInfo, InfoInternal, PaintArgsInternal, PainterInternal, () => DefaultDraw());
		}
		public new PivotCustomDrawFieldValueThreadSafeEventArgs ThreadSafeArgs {
			get { return (PivotCustomDrawFieldValueThreadSafeEventArgs)base.ThreadSafeArgs; }
		}
		protected PivotGridField GetField(PivotFieldItemBase item) {
			return (PivotGridField)Data.GetField(item);
		}
		protected PivotFieldsAreaCellViewInfo FieldCellViewInfo { get { return fieldCellViewInfo; } }
		protected PivotHeaderObjectInfoArgs InfoInternal { get { return info; } }
		protected PivotHeaderObjectPainter PainterInternal { get { return painter; } }
		public object Value {
			get {
				BeforeArgAccess();
				return FieldCellViewInfo.Value;
			}
		}
		public int MinIndex {
			get {
				BeforeArgAccess();
				return FieldCellViewInfo != null ? FieldCellViewInfo.MinLastLevelIndex : -1;
			}
		}
		public int MaxIndex {
			get {
				BeforeArgAccess();
				return FieldCellViewInfo != null ? FieldCellViewInfo.MaxLastLevelIndex : -1;
			}
		}
		public int FieldIndex {
			get {
				BeforeArgAccess();
				return FieldCellViewInfo != null ? FieldCellViewInfo.VisibleIndex : -1;
			}
		}
		public string DisplayText {
			get {
				BeforeArgAccess();
				return Info.Caption;
			}
		}
		public PivotGridValueType ValueType {
			get {
				BeforeArgAccess();
				return FieldCellViewInfo.ValueType;
			}
		}
		public PivotGridCustomTotal CustomTotal {
			get {
				BeforeArgAccess();
				return FieldCellViewInfo.CustomTotal;
			}
		}
		public bool IsOthersValue {
			get {
				BeforeArgAccess();
				return FieldCellViewInfo != null ? FieldCellViewInfo.IsOthersValue : false;
			}
		}
		public PivotGridField Field {
			get {
				BeforeArgAccess();
				return GetField(FieldCellViewInfo.Field);
			}
		}
		public PivotArea Area {
			get {
				BeforeArgAccess();
				return FieldCellViewInfo.Item.Area;
			}
		}
		public PivotHeaderObjectInfoArgs Info {
			get {
				BeforeArgAccess();
				return info;
			}
		}
		public PivotHeaderObjectPainter Painter {
			get {
				BeforeArgAccess();
				return painter;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridViewInfoData Data { get { return FieldCellViewInfo.Data; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotFieldValueItem Item { get { return FieldCellViewInfo.Item; } }
		public PivotGridField[] GetHigherLevelFields() {
			BeforeArgAccess();
			if(Field == null)
				return new PivotGridField[0];
			if(Field.Area == PivotArea.DataArea) {
				PivotGridField[] fields = new PivotGridField[Data.GetFieldCountByArea(Data.OptionsDataField.DataFieldArea)];
				for(int i = 0; i < fields.Length; i++)
					fields[i] = (PivotGridField)Data.GetFieldByArea(Data.OptionsDataField.DataFieldArea, i);
				return fields;
			} else {
				PivotGridField[] fields = new PivotGridField[Field.AreaIndex];
				for(int i = Field.AreaIndex - 1; i >= 0; i--) {
					fields[i] = Data.GetFieldByLevel(FieldCellViewInfo.IsColumn, i) as PivotGridField;
				}
				return fields;
			}
		}
		public object GetHigherLevelFieldValue(PivotGridField field) {
			BeforeArgAccess();
			if(field.Area != Field.Area || field.AreaIndex > Field.AreaIndex || !field.Visible)
				return null;
			return Data.GetFieldValue(field, FieldCellViewInfo.VisibleIndex, FieldCellViewInfo.VisibleIndex);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			BeforeArgAccess();
			if(columnIndex < 0 || columnIndex >= Data.VisualItems.ColumnCount)
				throw new ArgumentOutOfRangeException("columnIndex");
			if(rowIndex < 0 || rowIndex >= Data.VisualItems.RowCount)
				throw new ArgumentOutOfRangeException("rowIndex");
			return Data.VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		public object GetFieldValue(PivotGridField field, int cellIndex) {
			BeforeArgAccess();
			if(field == null)
				throw new ArgumentNullException("field");
			if(cellIndex < 0)
				throw new ArgumentOutOfRangeException("cellIndex");
			return Data.VisualItems.GetFieldValue(field, cellIndex);
		}
	}
	public class PivotFieldImageIndexEventArgs : PivotFieldValueEventArgs {
		int imageIndex;
		IThreadSafeAccessible threadSafeAccess;
		PivotFieldImageIndexThreadSafeEventArgs threadSafeArgs;
		PivotFieldImageIndexEventArgs internalUnsafeCopy;
		public PivotFieldImageIndexEventArgs(IThreadSafeAccessible threadSafeAccess, PivotFieldValueItem item)
			: base(item) {
			this.imageIndex = -1;
			this.threadSafeAccess = threadSafeAccess;
		}
		IThreadSafeAccessible ThreadSafeAccess { get { return threadSafeAccess; } }
		protected PivotFieldImageIndexEventArgs InternalUnsafeCopy {
			get {
				if(internalUnsafeCopy == null)
					internalUnsafeCopy = CreateInternalUnsafeCopy();
				return internalUnsafeCopy;
			}
		}
		protected virtual PivotFieldImageIndexEventArgs CreateInternalUnsafeCopy() {
			return new PivotFieldImageIndexEventArgsInternal(ThreadSafeAccess, Item);
		}
		public PivotFieldImageIndexThreadSafeEventArgs ThreadSafeArgs {
			get {
				if(threadSafeArgs == null)
					threadSafeArgs = new PivotFieldImageIndexThreadSafeEventArgs(ThreadSafeAccess, InternalUnsafeCopy);
				return threadSafeArgs;
			}
		}
		protected bool HasThreadSafeArgs { get { return threadSafeArgs != null; } }
		protected virtual bool AllowUnsafeAccess { get { return false; } }
		protected void BeforeArgAccess() {
			if(AllowUnsafeAccess)
				return;
			ThrowExceptionIfAsyncIsInProgress();
		}
		void ThrowExceptionIfAsyncIsInProgress() {
			if(ThreadSafeAccess.IsAsyncInProgress)
				throw new PivotCustomDrawUnsafeAccessException();
		}
		public int ImageIndex {
			get {
				if(HasThreadSafeArgs)
					imageIndex = Math.Max(imageIndex, ThreadSafeArgs.ImageIndex);
				return imageIndex;
			}
			set {
				BeforeArgAccess();
				imageIndex = value;
			}
		}
		public new PivotGridField DataField {
			get {
				BeforeArgAccess();
				return base.DataField;
			}
		}
		public new bool IsColumn {
			get {
				BeforeArgAccess();
				return base.IsColumn;
			}
		}
		public new int MinIndex {
			get {
				BeforeArgAccess();
				return base.MinIndex;
			}
		}
		public new int MaxIndex {
			get {
				BeforeArgAccess();
				return base.MaxIndex;
			}
		}
		public new int FieldIndex {
			get {
				BeforeArgAccess();
				return base.FieldIndex;
			}
		}
		public new object Value {
			get {
				BeforeArgAccess();
				return base.Value;
			}
		}
		public new bool IsOthersValue {
			get {
				BeforeArgAccess();
				return base.IsOthersValue;
			}
		}
		public new PivotGridValueType ValueType {
			get {
				BeforeArgAccess();
				return base.ValueType;
			}
		}
		public new PivotGridCustomTotal CustomTotal {
			get {
				BeforeArgAccess();
				return base.CustomTotal;
			}
		}
		public new bool IsCollapsed {
			get {
				BeforeArgAccess();
				return base.IsCollapsed;
			}
		}
		public new void ChangeExpandedState() {
			BeforeArgAccess();
			base.ChangeExpandedState();
		}
		public new PivotGridField[] GetHigherLevelFields() {
			BeforeArgAccess();
			return base.GetHigherLevelFields();
		}
		public new object GetHigherLevelFieldValue(PivotGridField field) {
			BeforeArgAccess();
			return base.GetHigherLevelFieldValue(field);
		}
		public new PivotDrillDownDataSource CreateDrillDownDataSource() {
			BeforeArgAccess();
			return base.CreateDrillDownDataSource();
		}
		public new PivotDrillDownDataSource CreateDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			BeforeArgAccess();
			return base.CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public new PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public new PivotDrillDownDataSource CreateServerModeDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		public new object GetCellValue(int columnIndex, int rowIndex) {
			BeforeArgAccess();
			return base.GetCellValue(columnIndex, rowIndex);
		}
		public new object GetFieldValue(PivotGridField field, int cellIndex) {
			BeforeArgAccess();
			return base.GetFieldValue(field, cellIndex);
		}
		public new PivotGridField Field {
			get {
				BeforeArgAccess();
				return base.Field;
			}
		}
	}
	#endregion
	#region Internal Event Args
	public class PivotCustomDrawCellBaseEventArgsInternal : PivotCustomDrawCellBaseEventArgs {
		public PivotCustomDrawCellBaseEventArgsInternal(IThreadSafeAccessible threadSafeAccess, PivotGridCellItem cellItem, PivotGridViewInfo viewInfo)
			: base(threadSafeAccess, cellItem, viewInfo) { }
		protected override bool AllowUnsafeAccess { get { return true; } }
	}
	public class PivotCustomDrawCellEventArgsInternal : PivotCustomDrawCellEventArgs {
		public PivotCustomDrawCellEventArgsInternal(IThreadSafeAccessible threadSafeAccess, PivotGridCellItem cellItem,
						AppearanceObject appearance, ViewInfoPaintArgs paintArgs, PivotGridViewInfo viewInfo, MethodInvoker defaultDraw)
			: base(threadSafeAccess, cellItem, appearance, paintArgs, viewInfo, defaultDraw) { }
		protected override bool AllowUnsafeAccess { get { return true; } }
	}
	public class PivotCustomAppearanceEventArgsInternal : PivotCustomAppearanceEventArgs {
		public PivotCustomAppearanceEventArgsInternal(IThreadSafeAccessible threadSafeAccess, PivotGridCellItem cellItem,
			AppearanceObject appearance, PivotGridViewInfo viewInfo, Rectangle? bounds)
			: base(threadSafeAccess, cellItem, appearance, viewInfo, bounds) { }
		protected override bool AllowUnsafeAccess { get { return true; } }
	}
	public class PivotCustomDrawEventArgsInternal : PivotCustomDrawEventArgs {
		public PivotCustomDrawEventArgsInternal(IThreadSafeAccessible threadSafeAccess, IPivotCustomDrawAppearanceOwner appearanceOwner,
							ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw)
			: base(threadSafeAccess, appearanceOwner, paintArgs, bounds, defaultDraw) { }
		protected override bool AllowUnsafeAccess { get { return true; } }
	}
	public class PivotCustomDrawHeaderAreaEventArgsInternal : PivotCustomDrawHeaderAreaEventArgs {
		public PivotCustomDrawHeaderAreaEventArgsInternal(IThreadSafeAccessible threadSafeAccess, PivotHeadersViewInfoBase headersViewInfo,
										ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw)
			: base(threadSafeAccess, headersViewInfo, paintArgs, bounds, defaultDraw) { }
		protected override bool AllowUnsafeAccess { get { return true; } }
	}
	public class PivotCustomDrawFieldHeaderEventArgsInternal : PivotCustomDrawFieldHeaderEventArgs {
		public PivotCustomDrawFieldHeaderEventArgsInternal(IThreadSafeAccessible threadSafeAccess, PivotHeaderViewInfoBase headerViewInfo,
									ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter, MethodInvoker defaultDraw)
			: base(threadSafeAccess, headerViewInfo, paintArgs, painter, defaultDraw) { }
		protected override bool AllowUnsafeAccess { get { return true; } }
	}
	public class PivotCustomDrawFieldValueEventArgsInternal : PivotCustomDrawFieldValueEventArgs {
		public PivotCustomDrawFieldValueEventArgsInternal(IThreadSafeAccessible threadSafeAccess, PivotFieldsAreaCellViewInfo fieldCellViewInfo,
								PivotHeaderObjectInfoArgs info, ViewInfoPaintArgs paintArgs, PivotHeaderObjectPainter painter, MethodInvoker defaultDraw)
			: base(threadSafeAccess, fieldCellViewInfo, info, paintArgs, painter, defaultDraw) { }
		protected override bool AllowUnsafeAccess { get { return true; } }
	}
	public class PivotFieldImageIndexEventArgsInternal : PivotFieldImageIndexEventArgs {
		public PivotFieldImageIndexEventArgsInternal(IThreadSafeAccessible threadSafeAccess, PivotFieldValueItem item)
			: base(threadSafeAccess, item) { }
		protected override bool AllowUnsafeAccess { get { return true; } }
	}
	#endregion
	#region Thread-Safe Event Args
	public class PivotCustomDrawBaseThreadSafeEventArgs {
		IThreadSafeAccessible threadSafeAccess;
		public PivotCustomDrawBaseThreadSafeEventArgs(IThreadSafeAccessible threadSafeAccess) {
			this.threadSafeAccess = threadSafeAccess;
		}
		protected IThreadSafeAccessible ThreadSafeAccess {
			get { return threadSafeAccess; }
		}
		public IThreadSafeFieldCollection Fields {
			get { return ThreadSafeAccess.Fields; }
		}
		public IThreadSafeGroupCollection Groups {
			get { return ThreadSafeAccess.Groups; }
		}
		public IThreadSafeField GetFieldByArea(PivotArea area, int index) {
			return ThreadSafeAccess.GetFieldByArea(area, index);
		}
		public List<IThreadSafeField> GetFieldsByArea(PivotArea area) {
			return ThreadSafeAccess.GetFieldsByArea(area);
		}
		public string GetCellDisplayText(int columnIndex, int rowIndex) {
			return ThreadSafeAccess.GetCellDisplayText(columnIndex, rowIndex);
		}
		public string GetFieldValueDisplayText(IThreadSafeField field, int index) {
			return ThreadSafeAccess.GetFieldValueDisplayText(field, index);
		}
		public int ColumnCount { get { return ThreadSafeAccess.ColumnCount; } }
		public int RowCount { get { return ThreadSafeAccess.RowCount; } }
	}
	public class PivotCustomDrawThreadSafeEventArgs : PivotCustomDrawBaseThreadSafeEventArgs {
		PivotCustomDrawEventArgs ownerArgs;
		public PivotCustomDrawThreadSafeEventArgs(IThreadSafeAccessible threadSafeAccess, PivotCustomDrawEventArgs ownerArgs)
			: base(threadSafeAccess) {
			this.ownerArgs = ownerArgs;
		}
		protected PivotCustomDrawEventArgs OwnerArgs { get { return ownerArgs; } }
		public bool Handled {
			get { return OwnerArgs.Handled; }
			set { OwnerArgs.Handled = value; }
		}
		public AppearanceObject Appearance {
			get { return OwnerArgs.Appearance; }
			set { OwnerArgs.Appearance = value; }
		}
		public Graphics Graphics { get { return OwnerArgs.Graphics; } }
		public GraphicsCache GraphicsCache { get { return OwnerArgs.GraphicsCache; } }
		public Rectangle Bounds { get { return OwnerArgs.Bounds; } }
	}
	public class PivotCustomDrawHeaderAreaThreadSafeEventArgs : PivotCustomDrawThreadSafeEventArgs {
		public PivotCustomDrawHeaderAreaThreadSafeEventArgs(IThreadSafeAccessible threadSafeAccess, PivotCustomDrawHeaderAreaEventArgs ownerArgs)
			: base(threadSafeAccess, ownerArgs) { }
		protected new PivotCustomDrawHeaderAreaEventArgs OwnerArgs {
			get { return (PivotCustomDrawHeaderAreaEventArgs)base.OwnerArgs; }
		}
		public PivotArea Area { get { return OwnerArgs.Area; } }
	}
	public class PivotCustomDrawFieldHeaderThreadSafeEventArgs : PivotCustomDrawThreadSafeEventArgs {
		public PivotCustomDrawFieldHeaderThreadSafeEventArgs(IThreadSafeAccessible threadSafeAccess, PivotCustomDrawFieldHeaderEventArgs ownerArgs)
			: base(threadSafeAccess, ownerArgs) { }
		protected new PivotCustomDrawFieldHeaderEventArgs OwnerArgs {
			get { return (PivotCustomDrawFieldHeaderEventArgs)base.OwnerArgs; }
		}
		public IThreadSafeField Field { get { return (IThreadSafeField)OwnerArgs.FieldItem; } }
		public HeaderObjectInfoArgs Info { get { return OwnerArgs.Info; } }
		public HeaderObjectPainter Painter { get { return OwnerArgs.Painter; } }
	}
	public class PivotCustomDrawFieldValueThreadSafeEventArgs : PivotCustomDrawThreadSafeEventArgs {
		public PivotCustomDrawFieldValueThreadSafeEventArgs(IThreadSafeAccessible threadSafeAccess, PivotCustomDrawFieldValueEventArgs ownerArgs)
			: base(threadSafeAccess, ownerArgs) { }
		protected new PivotCustomDrawFieldValueEventArgs OwnerArgs {
			get { return (PivotCustomDrawFieldValueEventArgs)base.OwnerArgs; }
		}
		PivotGridViewInfoData Data { get { return OwnerArgs.Data; } }
		public IThreadSafeField Field { get { return (IThreadSafeField)Item.Field; } }
		public int MinIndex { get { return OwnerArgs.MinIndex; } }
		public int MaxIndex { get { return OwnerArgs.MaxIndex; } }
		public int FieldIndex { get { return OwnerArgs.FieldIndex; } }
		public string DisplayText { get { return OwnerArgs.DisplayText; } }
		public PivotGridValueType ValueType { get { return OwnerArgs.ValueType; } }
		public PivotGridCustomTotal CustomTotal { get { return OwnerArgs.CustomTotal; } }
		public PivotArea Area { get { return OwnerArgs.Area; } }
		public PivotHeaderObjectInfoArgs Info { get { return OwnerArgs.Info; } }
		public PivotHeaderObjectPainter Painter { get { return OwnerArgs.Painter; } }
		PivotFieldValueItem Item { get { return OwnerArgs.Item; } }
		public IThreadSafeField[] GetHigherLevelFields() {
			if(Field == null)
				return new IThreadSafeField[0];
			if(Field.Area == PivotArea.DataArea) {
				IThreadSafeField[] fields = new IThreadSafeField[ThreadSafeAccess.GetFieldCountByArea(Data.OptionsDataField.DataFieldArea)];
				for(int i = 0; i < fields.Length; i++)
					fields[i] = (IThreadSafeField)ThreadSafeAccess.GetFieldByArea(Data.OptionsDataField.DataFieldArea, i);
				return fields;
			} else {
				IThreadSafeField[] fields = new IThreadSafeField[Field.AreaIndex];
				for(int i = Field.AreaIndex - 1; i >= 0; i--) {
					fields[i] = ThreadSafeAccess.GetFieldByLevel(Item.IsColumn, i) as IThreadSafeField;
				}
				return fields;
			}
		}
	}
	public class PivotFieldImageIndexThreadSafeEventArgs : PivotCustomDrawBaseThreadSafeEventArgs {
		PivotFieldImageIndexEventArgs ownerArgs;
		IThreadSafeAccessible threadSafeAccess;
		public PivotFieldImageIndexThreadSafeEventArgs(IThreadSafeAccessible threadSafeAccess, PivotFieldImageIndexEventArgs ownerArgs)
			: base(threadSafeAccess) {
			this.threadSafeAccess = threadSafeAccess;
			this.ownerArgs = ownerArgs;
		}
		PivotFieldImageIndexEventArgs OwnerArgs {
			get { return ownerArgs; }
		}
		public int ImageIndex { get { return OwnerArgs.ImageIndex; } set { OwnerArgs.ImageIndex = value; } }
		PivotFieldValueItem Item { get { return OwnerArgs.Item; } }
		PivotGridViewInfoData Data { get { return OwnerArgs.Data; } }
		public IThreadSafeField DataField { get { return Item != null ? (IThreadSafeField)Item.DataField : null; } }
		public bool IsColumn { get { return OwnerArgs.IsColumn; } }
		public int MinIndex { get { return OwnerArgs.MinIndex; } }
		public int MaxIndex { get { return OwnerArgs.MaxIndex; } }
		public int Index { get { return Item.Index; } }
		public int FieldIndex { get { return OwnerArgs.FieldIndex; } }
		public PivotGridValueType ValueType { get { return OwnerArgs.ValueType; } }
		public PivotGridCustomTotal CustomTotal { get { return OwnerArgs.CustomTotal; } }
		public string DisplayText { get { return Item.DisplayText; } }
		public IThreadSafeField[] GetHigherLevelFields() {
			if(Field == null)
				return new IThreadSafeField[0];
			if(Field.Area == PivotArea.DataArea) {
				PivotFieldValueItem parent = Data.VisualItems.GetParentItem(IsColumn, Item);
				List<IThreadSafeField> fields = new List<IThreadSafeField>();
				while(parent != null) {
					if(parent.Field != null)
						fields.Insert(0, (IThreadSafeField)parent.Field);
					parent = Data.VisualItems.GetParentItem(IsColumn, parent);
				}
				return fields.ToArray();
			} else {
				List<IThreadSafeField> baseFields = ThreadSafeAccess.GetFieldsByArea(Field.Area);
				int index = baseFields.IndexOf(Field);
				List<IThreadSafeField> fields = new List<IThreadSafeField>();
				for(int i = 0; i < index; i++) {
					IThreadSafeField field = (IThreadSafeField)baseFields[i];
					if(field == Data.DataField)
						field = DataField;
					fields.Add(field);
				}
				return fields.ToArray();
			}
		}
		public IThreadSafeField Field { get { return (IThreadSafeField)OwnerArgs.Item.Field; } }
	}
	public class PivotCustomDrawCellBaseThreadSafeEventArgs : PivotCustomDrawBaseThreadSafeEventArgs {
		PivotCustomDrawCellBaseEventArgs ownerArgs;
		public PivotCustomDrawCellBaseThreadSafeEventArgs(IThreadSafeAccessible threadSafeAccess, PivotCustomDrawCellBaseEventArgs ownerArgs)
			: base(threadSafeAccess) {
			this.ownerArgs = ownerArgs;
		}
		protected PivotCustomDrawCellBaseEventArgs OwnerArgs {
			get { return ownerArgs; }
		}
		public bool Focused { get { return OwnerArgs.Focused; } }
		public bool Selected { get { return OwnerArgs.Selected; } }
		public string DisplayText { get { return OwnerArgs.DisplayText; } }
		public object Value { get { return OwnerArgs.Value; } }
		public Rectangle Bounds { get { return OwnerArgs.Bounds; } }
		PivotGridCellItem Item { get { return OwnerArgs.Item; } }
		PivotGridViewInfoData Data { get { return OwnerArgs.Data; } }
		public IThreadSafeField DataField { get { return (IThreadSafeField)Item.DataField; } }
		public int ColumnIndex { get { return OwnerArgs.ColumnIndex; } }
		public int RowIndex { get { return OwnerArgs.RowIndex; } }
		public int ColumnFieldIndex { get { return OwnerArgs.ColumnFieldIndex; } }
		public int RowFieldIndex { get { return OwnerArgs.RowFieldIndex; } }
		public IThreadSafeField ColumnField { get { return (IThreadSafeField)Item.ColumnField; } }
		public IThreadSafeField RowField { get { return (IThreadSafeField)Item.RowField; } }
		public PivotSummaryType SummaryType { get { return OwnerArgs.SummaryType; } }
		public PivotGridValueType ColumnValueType { get { return OwnerArgs.ColumnValueType; } }
		public PivotGridValueType RowValueType { get { return OwnerArgs.RowValueType; } }
		public PivotGridCustomTotal ColumnCustomTotal { get { return OwnerArgs.ColumnCustomTotal; } }
		public PivotGridCustomTotal RowCustomTotal { get { return OwnerArgs.RowCustomTotal; } }
	}
	public class PivotCustomDrawCellThreadSafeEventArgs : PivotCustomDrawCellBaseThreadSafeEventArgs {
		public PivotCustomDrawCellThreadSafeEventArgs(IThreadSafeAccessible threadSafeAccess, PivotCustomDrawCellEventArgs ownerArgs)
			: base(threadSafeAccess, ownerArgs) { }
		protected new PivotCustomDrawCellEventArgs OwnerArgs {
			get { return (PivotCustomDrawCellEventArgs)base.OwnerArgs; }
		}
		public bool Handled { get { return OwnerArgs.Handled; } set { OwnerArgs.Handled = value; } }
		public AppearanceObject Appearance {
			get { return OwnerArgs.Appearance; }
			set { OwnerArgs.Appearance = value; }
		}
		public Graphics Graphics { get { return OwnerArgs.Graphics; } }
		public GraphicsCache GraphicsCache { get { return OwnerArgs.GraphicsCache; } }
	}
	public class PivotCustomAppearanceThreadSafeEventArgs : PivotCustomDrawCellBaseThreadSafeEventArgs {
		public PivotCustomAppearanceThreadSafeEventArgs(IThreadSafeAccessible threadSafeAccess, PivotCustomAppearanceEventArgs ownerArgs)
			: base(threadSafeAccess, ownerArgs) { }
		protected new PivotCustomAppearanceEventArgs OwnerArgs {
			get { return (PivotCustomAppearanceEventArgs)base.OwnerArgs; }
		}
		public AppearanceObject Appearance {
			get { return OwnerArgs.Appearance; }
			set { OwnerArgs.Appearance = value; }
		}
	}
	public class PivotCustomDrawUnsafeAccessException : Exception {
		const string message = @"A thread unsafe event parameter member has been accessed while an asynchronous operation was in progress.

This property or method cannot be used when the pivot grid is performing background calculations. Use a thread-safe event parameter instead. You can access it via the event parameter's ThreadSafeArgs property. The returned object exposes properties and methods that can be used to access event data while an asynchronous operation is being executed.
To learn more, see the Asynchronous Mode documentation topic.
http://documentation.devexpress.com/#WindowsForms/CustomDocument9578";
		public PivotCustomDrawUnsafeAccessException()
			: base() { }
		public override string Message { get { return message; } }
	}
	#endregion
}
