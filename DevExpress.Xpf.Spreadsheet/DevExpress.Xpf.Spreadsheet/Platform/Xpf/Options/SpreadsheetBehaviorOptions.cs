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
using System.Windows;
using System.Windows.Data;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet;
using InnerSpreadsheetBehaviorOptions = DevExpress.XtraSpreadsheet.SpreadsheetBehaviorOptions;
using InnerSpreadsheetWorksheetBehaviorOptions = DevExpress.XtraSpreadsheet.SpreadsheetWorksheetBehaviorOptions;
using InnerSpreadsheetRowBehaviorOptions = DevExpress.XtraSpreadsheet.SpreadsheetRowBehaviorOptions;
using InnerSpreadsheetColumnBehaviorOptions = DevExpress.XtraSpreadsheet.SpreadsheetColumnBehaviorOptions;
using InnerSpreadsheetDrawingBehaviorOptions = DevExpress.XtraSpreadsheet.SpreadsheetDrawingBehaviorOptions;
using InnerSpreadsheetSelectionBehaviorOptions = DevExpress.XtraSpreadsheet.SpreadsheetSelectionOptions;
using InnerSpreadsheetCommentBehaviorOptions = DevExpress.XtraSpreadsheet.SpreadsheetCommentBehaviorOptions;
using InnerSpreadsheetGroupBehaviorOptions = DevExpress.XtraSpreadsheet.SpreadsheetGroupBehaviorOptions;
using InnerSpreadsheetProtectionBehaviorOptions = DevExpress.XtraSpreadsheet.SpreadsheetProtectionBehaviorOptions;
namespace DevExpress.Xpf.Spreadsheet {
	#region SpreadsheetBehaviorOptions
	public class SpreadsheetBehaviorOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty WorksheetProperty;
		public static readonly DependencyProperty RowProperty;
		public static readonly DependencyProperty ColumnProperty;
		public static readonly DependencyProperty DrawingProperty;
		public static readonly DependencyProperty SelectionProperty;
		public static readonly DependencyProperty CommentProperty;
		public static readonly DependencyProperty GroupProperty;
		public static readonly DependencyProperty ProtectionProperty;
		public static readonly DependencyProperty CellEditorCommitModeProperty;
		public static readonly DependencyProperty CutProperty;
		public static readonly DependencyProperty CopyProperty;
		public static readonly DependencyProperty PasteProperty;
		public static readonly DependencyProperty CreateNewProperty;
		public static readonly DependencyProperty OpenProperty;
		public static readonly DependencyProperty SaveProperty;
		public static readonly DependencyProperty SaveAsProperty;
		public static readonly DependencyProperty PrintProperty;
		public static readonly DependencyProperty MinZoomFactorProperty;
		public static readonly DependencyProperty MaxZoomFactorProperty;
		public static readonly DependencyProperty ZoomProperty;
		public static readonly DependencyProperty FreezePanesProperty;
		public static readonly DependencyProperty ShowPopupMenuProperty;
		public static readonly DependencyProperty TouchProperty;
		public static readonly DependencyProperty FunctionNameCultureProperty;
		public static readonly DependencyProperty FillHandleEnabledProperty;
		public static readonly DependencyProperty DragProperty;
		public static readonly DependencyProperty DropProperty;
		InnerSpreadsheetBehaviorOptions source;
		#endregion
		static SpreadsheetBehaviorOptions() {
			Type ownerType = typeof(SpreadsheetBehaviorOptions);
			WorksheetProperty = DependencyProperty.Register("Worksheet", typeof(SpreadsheetWorksheetBehaviorOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetBehaviorOptions)d).OnWorksheetChanged((SpreadsheetWorksheetBehaviorOptions)e.OldValue, (SpreadsheetWorksheetBehaviorOptions)e.NewValue)));
			RowProperty = DependencyProperty.Register("Row", typeof(SpreadsheetRowBehaviorOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetBehaviorOptions)d).OnRowChanged((SpreadsheetRowBehaviorOptions)e.OldValue, (SpreadsheetRowBehaviorOptions)e.NewValue)));
			ColumnProperty = DependencyProperty.Register("Column", typeof(SpreadsheetColumnBehaviorOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetBehaviorOptions)d).OnColumnChanged((SpreadsheetColumnBehaviorOptions)e.OldValue, (SpreadsheetColumnBehaviorOptions)e.NewValue)));
			DrawingProperty = DependencyProperty.Register("Drawing", typeof(SpreadsheetDrawingBehaviorOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetBehaviorOptions)d).OnDrawingChanged((SpreadsheetDrawingBehaviorOptions)e.OldValue, (SpreadsheetDrawingBehaviorOptions)e.NewValue)));
			SelectionProperty = DependencyProperty.Register("Selection", typeof(SpreadsheetSelectionBehaviorOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetBehaviorOptions)d).OnSelectionChanged((SpreadsheetSelectionBehaviorOptions)e.OldValue, (SpreadsheetSelectionBehaviorOptions)e.NewValue)));
			CommentProperty = DependencyProperty.Register("Comment", typeof(SpreadsheetCommentBehaviorOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetBehaviorOptions)d).OnCommentChanged((SpreadsheetCommentBehaviorOptions)e.OldValue, (SpreadsheetCommentBehaviorOptions)e.NewValue)));
			GroupProperty = DependencyProperty.Register("Group", typeof(SpreadsheetGroupBehaviorOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetBehaviorOptions)d).OnGroupChanged((SpreadsheetGroupBehaviorOptions)e.OldValue, (SpreadsheetGroupBehaviorOptions)e.NewValue)));
			ProtectionProperty = DependencyProperty.Register("Protection", typeof(SpreadsheetProtectionBehaviorOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetBehaviorOptions)d).OnProtectionChanged((SpreadsheetProtectionBehaviorOptions)e.OldValue, (SpreadsheetProtectionBehaviorOptions)e.NewValue)));
			CellEditorCommitModeProperty = DependencyProperty.Register("CellEditorCommitMode", typeof(CellEditorCommitMode), ownerType,
				new FrameworkPropertyMetadata(CellEditorCommitMode.Auto));
			Type capabilityType = typeof(DocumentCapability);
			CutProperty = DependencyProperty.Register("Cut", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			CopyProperty = DependencyProperty.Register("Copy", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			PasteProperty = DependencyProperty.Register("Paste", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			CreateNewProperty = DependencyProperty.Register("CreateNew", capabilityType, ownerType,
				 new FrameworkPropertyMetadata(DocumentCapability.Default));
			OpenProperty = DependencyProperty.Register("Open", capabilityType, ownerType,
				 new FrameworkPropertyMetadata(DocumentCapability.Default));
			SaveProperty = DependencyProperty.Register("Save", capabilityType, ownerType,
				 new FrameworkPropertyMetadata(DocumentCapability.Default));
			SaveAsProperty = DependencyProperty.Register("SaveAs", capabilityType, ownerType,
				 new FrameworkPropertyMetadata(DocumentCapability.Default));
			PrintProperty = DependencyProperty.Register("Print", capabilityType, ownerType,
				 new FrameworkPropertyMetadata(DocumentCapability.Default));
			MinZoomFactorProperty = DependencyProperty.Register("MinZoomFactor", typeof(float), ownerType,
				new FrameworkPropertyMetadata(0.09f));
			MaxZoomFactorProperty = DependencyProperty.Register("MaxZoomFactor", typeof(float), ownerType,
				new FrameworkPropertyMetadata(float.PositiveInfinity));
			ZoomProperty = DependencyProperty.Register("Zoom", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			FreezePanesProperty = DependencyProperty.Register("FreezePanes", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			ShowPopupMenuProperty = DependencyProperty.Register("ShowPopupMenu", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			TouchProperty = DependencyProperty.Register("Touch", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			FunctionNameCultureProperty = DependencyProperty.Register("FunctionNameCulture", typeof(FunctionNameCulture), ownerType,
				new FrameworkPropertyMetadata(FunctionNameCulture.Auto));
			FillHandleEnabledProperty = DependencyProperty.Register("FillHandleEnabled", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			DragProperty = DependencyProperty.Register("Drag", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			DropProperty = DependencyProperty.Register("Drop", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
		}
		public SpreadsheetBehaviorOptions() {
			Worksheet = new SpreadsheetWorksheetBehaviorOptions();
			Row = new SpreadsheetRowBehaviorOptions();
			Column = new SpreadsheetColumnBehaviorOptions();
			Drawing = new SpreadsheetDrawingBehaviorOptions();
			Selection = new SpreadsheetSelectionBehaviorOptions();
			Comment = new SpreadsheetCommentBehaviorOptions();
			Group = new SpreadsheetGroupBehaviorOptions();
			Protection = new SpreadsheetProtectionBehaviorOptions();
		}
		#region Properties
		public SpreadsheetWorksheetBehaviorOptions Worksheet {
			get { return (SpreadsheetWorksheetBehaviorOptions)GetValue(WorksheetProperty); }
			set { SetValue(WorksheetProperty, value); }
		}
		public SpreadsheetRowBehaviorOptions Row {
			get { return (SpreadsheetRowBehaviorOptions)GetValue(RowProperty); }
			set { SetValue(RowProperty, value); }
		}
		public SpreadsheetColumnBehaviorOptions Column {
			get { return (SpreadsheetColumnBehaviorOptions)GetValue(ColumnProperty); }
			set { SetValue(ColumnProperty, value); }
		}
		public SpreadsheetDrawingBehaviorOptions Drawing {
			get { return (SpreadsheetDrawingBehaviorOptions)GetValue(DrawingProperty); }
			set { SetValue(DrawingProperty, value); }
		}
		public SpreadsheetSelectionBehaviorOptions Selection {
			get { return (SpreadsheetSelectionBehaviorOptions)GetValue(SelectionProperty); }
			set { SetValue(SelectionProperty, value); }
		}
		public SpreadsheetCommentBehaviorOptions Comment {
			get { return (SpreadsheetCommentBehaviorOptions)GetValue(CommentProperty); }
			set { SetValue(CommentProperty, value); }
		}
		public SpreadsheetGroupBehaviorOptions Group {
			get { return (SpreadsheetGroupBehaviorOptions)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		public SpreadsheetProtectionBehaviorOptions Protection {
			get { return (SpreadsheetProtectionBehaviorOptions)GetValue(ProtectionProperty); }
			set { SetValue(ProtectionProperty, value); }
		}
		public CellEditorCommitMode CellEditorCommitMode {
			get { return (CellEditorCommitMode)GetValue(CellEditorCommitModeProperty); }
			set { SetValue(CellEditorCommitModeProperty, value); }
		}
		public DocumentCapability Cut {
			get { return (DocumentCapability)GetValue(CutProperty); }
			set { SetValue(CutProperty, value); }
		}
		public DocumentCapability Copy {
			get { return (DocumentCapability)GetValue(CopyProperty); }
			set { SetValue(CopyProperty, value); }
		}
		public DocumentCapability Paste {
			get { return (DocumentCapability)GetValue(PasteProperty); }
			set { SetValue(PasteProperty, value); }
		}
		public DocumentCapability CreateNew {
			get { return (DocumentCapability)GetValue(CreateNewProperty); }
			set { SetValue(CreateNewProperty, value); }
		}
		public DocumentCapability Open {
			get { return (DocumentCapability)GetValue(OpenProperty); }
			set { SetValue(OpenProperty, value); }
		}
		public DocumentCapability Save {
			get { return (DocumentCapability)GetValue(SaveProperty); }
			set { SetValue(SaveProperty, value); }
		}
		public DocumentCapability SaveAs {
			get { return (DocumentCapability)GetValue(SaveAsProperty); }
			set { SetValue(SaveAsProperty, value); }
		}
		public DocumentCapability Print {
			get { return (DocumentCapability)GetValue(PrintProperty); }
			set { SetValue(PrintProperty, value); }
		}
		public float MinZoomFactor {
			get { return (float)GetValue(MinZoomFactorProperty); }
			set { SetValue(MinZoomFactorProperty, value); }
		}
		public float MaxZoomFactor {
			get { return (float)GetValue(MaxZoomFactorProperty); }
			set { SetValue(MaxZoomFactorProperty, value); }
		}
		public DocumentCapability Zoom {
			get { return (DocumentCapability)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); }
		}
		public DocumentCapability FreezePanes {
			get { return (DocumentCapability)GetValue(FreezePanesProperty); }
			set { SetValue(FreezePanesProperty, value); }
		}
		public DocumentCapability ShowPopupMenu {
			get { return (DocumentCapability)GetValue(ShowPopupMenuProperty); }
			set { SetValue(ShowPopupMenuProperty, value); }
		}
		public DocumentCapability Touch {
			get { return (DocumentCapability)GetValue(TouchProperty); }
			set { SetValue(TouchProperty, value); }
		}
		public FunctionNameCulture FunctionNameCulture {
			get { return (FunctionNameCulture)GetValue(FunctionNameCultureProperty); }
			set { SetValue(FunctionNameCultureProperty, value); }
		}
		public bool FillHandleEnabled {
			get { return (bool)GetValue(FillHandleEnabledProperty); }
			set { SetValue(FillHandleEnabledProperty, value); }
		}
		public DocumentCapability Drag {
			get { return (DocumentCapability)GetValue(DragProperty); }
			set { SetValue(DragProperty, value); }
		}
		public DocumentCapability Drop {
			get { return (DocumentCapability)GetValue(DropProperty); }
			set { SetValue(DropProperty, value); }
		}
		#endregion
		void OnWorksheetChanged(SpreadsheetWorksheetBehaviorOptions oldValue, SpreadsheetWorksheetBehaviorOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Worksheet);
		}
		void OnRowChanged(SpreadsheetRowBehaviorOptions oldValue, SpreadsheetRowBehaviorOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Row);
		}
		void OnColumnChanged(SpreadsheetColumnBehaviorOptions oldValue, SpreadsheetColumnBehaviorOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Column);
		}
		void OnDrawingChanged(SpreadsheetDrawingBehaviorOptions oldValue, SpreadsheetDrawingBehaviorOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Drawing);
		}
		void OnSelectionChanged(SpreadsheetSelectionBehaviorOptions oldValue, SpreadsheetSelectionBehaviorOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Selection);
		}
		void OnCommentChanged(SpreadsheetCommentBehaviorOptions oldValue, SpreadsheetCommentBehaviorOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Comment);
		}
		void OnGroupChanged(SpreadsheetGroupBehaviorOptions oldValue, SpreadsheetGroupBehaviorOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Group);
		}
		void OnProtectionChanged(SpreadsheetProtectionBehaviorOptions oldValue, SpreadsheetProtectionBehaviorOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Protection);
		}
		internal void SetSource(InnerSpreadsheetBehaviorOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
			Worksheet.SetSource(source.Worksheet);
			Row.SetSource(source.Row);
			Column.SetSource(source.Column);
			Drawing.SetSource(source.Drawing);
			Selection.SetSource(source.Selection);
			Comment.SetSource(source.Comment);
			Group.SetSource(source.Group);
			Protection.SetSource(source.Protection);
		}
		void UpdateSourceProperties() {
			if (CellEditorCommitMode != (CellEditorCommitMode)GetDefaultValue(CellEditorCommitModeProperty))
				source.CellEditor.CommitMode = CellEditorCommitMode;
			if (Cut != (DocumentCapability)GetDefaultValue(CutProperty))
				source.Cut = Cut;
			if (Copy != (DocumentCapability)GetDefaultValue(CopyProperty))
				source.Copy = Copy;
			if (Paste != (DocumentCapability)GetDefaultValue(PasteProperty))
				source.Paste = Paste;
			if (CreateNew != (DocumentCapability)GetDefaultValue(CreateNewProperty))
				source.CreateNew = CreateNew;
			if (Open != (DocumentCapability)GetDefaultValue(OpenProperty))
				source.Open = Open;
			if (Save != (DocumentCapability)GetDefaultValue(SaveProperty))
				source.Save = Save;
			if (SaveAs != (DocumentCapability)GetDefaultValue(SaveAsProperty))
				source.SaveAs = SaveAs;
			if (Print != (DocumentCapability)GetDefaultValue(PrintProperty))
				source.Print = Print;
			if (MinZoomFactor != (float)GetDefaultValue(MinZoomFactorProperty))
				source.MinZoomFactor = MinZoomFactor;
			if (MaxZoomFactor != (float)GetDefaultValue(MaxZoomFactorProperty))
				source.MaxZoomFactor = MaxZoomFactor;
			if (Zoom != (DocumentCapability)GetDefaultValue(ZoomProperty))
				source.Zooming = Zoom;
			if (FreezePanes != (DocumentCapability)GetDefaultValue(FreezePanesProperty))
				source.FreezePanes = FreezePanes;
			if (ShowPopupMenu != (DocumentCapability)GetDefaultValue(ShowPopupMenuProperty))
				source.ShowPopupMenu = ShowPopupMenu;
			if (Touch != (DocumentCapability)GetDefaultValue(TouchProperty))
				source.Touch = Touch;
			if (FunctionNameCulture != (FunctionNameCulture)GetDefaultValue(FunctionNameCultureProperty))
				source.FunctionNameCulture = FunctionNameCulture;
			if (FillHandleEnabled != (bool)GetDefaultValue(FillHandleEnabledProperty))
				source.FillHandle.Enabled = FillHandleEnabled;
			if (Drag != (DocumentCapability)GetDefaultValue(DragProperty))
				source.Drag = Drag;
			if (Drop != (DocumentCapability)GetDefaultValue(DropProperty))
				source.Drop = Drop;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindCellEditorCommitModeProperty();
			BindCutProperty();
			BindCopyProperty();
			BindPasteProperty();
			BindCreateNewProperty();
			BindOpenProperty();
			BindSaveProperty();
			BindSaveAsProperty();
			BindPrintProperty();
			BindMinZoomFactorProperty();
			BindMaxZoomFactorProperty();
			BindZoomProperty();
			BindFreezePanesProperty();
			BindShowPopupMenuProperty();
			BindTouchProperty();
			BindFunctionNameCultureProperty();
			BindFillHandleEnabledProperty();
			BindDragProperty();
			BindDropProperty();
		}
		void BindCellEditorCommitModeProperty() {
			Binding bind = new Binding("CommitMode") { Source = source.CellEditor, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CellEditorCommitModeProperty, bind);
		}
		void BindCutProperty() {
			Binding bind = new Binding("Cut") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CutProperty, bind);
		}
		void BindCopyProperty() {
			Binding bind = new Binding("Copy") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CopyProperty, bind);
		}
		void BindPasteProperty() {
			Binding bind = new Binding("Paste") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, PasteProperty, bind);
		}
		void BindCreateNewProperty() {
			Binding bind = new Binding("CreateNew") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CreateNewProperty, bind);
		}
		void BindOpenProperty() {
			Binding bind = new Binding("Open") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, OpenProperty, bind);
		}
		void BindSaveProperty() {
			Binding bind = new Binding("Save") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, SaveProperty, bind);
		}
		void BindSaveAsProperty() {
			Binding bind = new Binding("SaveAs") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, SaveAsProperty, bind);
		}
		void BindPrintProperty() {
			Binding bind = new Binding("Print") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, PrintProperty, bind);
		}
		void BindMinZoomFactorProperty() {
			Binding bind = new Binding("MinZoomFactor") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, MinZoomFactorProperty, bind);
		}
		void BindMaxZoomFactorProperty() {
			Binding bind = new Binding("MaxZoomFactor") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, MaxZoomFactorProperty, bind);
		}
		void BindZoomProperty() {
			Binding bind = new Binding("Zooming") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ZoomProperty, bind);
		}
		void BindFreezePanesProperty() {
			Binding bind = new Binding("FreezePanes") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, FreezePanesProperty, bind);
		}
		void BindShowPopupMenuProperty() {
			Binding bind = new Binding("ShowPopupMenu") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ShowPopupMenuProperty, bind);
		}
		void BindTouchProperty() {
			Binding bind = new Binding("Touch") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, TouchProperty, bind);
		}
		void BindFunctionNameCultureProperty() {
			Binding bind = new Binding("FunctionNameCulture") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, FunctionNameCultureProperty, bind);
		}
		void BindFillHandleEnabledProperty() {
			Binding bind = new Binding("Enabled") { Source = source.FillHandle, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, FillHandleEnabledProperty, bind);
		}
		void BindDragProperty() {
			Binding bind = new Binding("Drag") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, DragProperty, bind);
		}
		void BindDropProperty() {
			Binding bind = new Binding("Drop") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, DropProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetWorksheetBehaviorOptions
	public class SpreadsheetWorksheetBehaviorOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty InsertProperty;
		public static readonly DependencyProperty DeleteProperty;
		public static readonly DependencyProperty RenameProperty;
		public static readonly DependencyProperty HideProperty;
		public static readonly DependencyProperty UnhideProperty;
		public static readonly DependencyProperty TabColorProperty;
		InnerSpreadsheetWorksheetBehaviorOptions source;
		#endregion
		static SpreadsheetWorksheetBehaviorOptions() {
			Type ownerType = typeof(SpreadsheetWorksheetBehaviorOptions);
			Type capabilityType = typeof(DocumentCapability);
			InsertProperty = DependencyProperty.Register("Insert", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			DeleteProperty = DependencyProperty.Register("Delete", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			RenameProperty = DependencyProperty.Register("Rename", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			HideProperty = DependencyProperty.Register("Hide", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			UnhideProperty = DependencyProperty.Register("Unhide", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			TabColorProperty = DependencyProperty.Register("TabColor", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
		}
		#region Properties
		public DocumentCapability Insert {
			get { return (DocumentCapability)GetValue(InsertProperty); }
			set { SetValue(InsertProperty, value); }
		}
		public DocumentCapability Delete {
			get { return (DocumentCapability)GetValue(DeleteProperty); }
			set { SetValue(DeleteProperty, value); }
		}
		public DocumentCapability Rename {
			get { return (DocumentCapability)GetValue(RenameProperty); }
			set { SetValue(RenameProperty, value); }
		}
		public DocumentCapability Hide {
			get { return (DocumentCapability)GetValue(HideProperty); }
			set { SetValue(HideProperty, value); }
		}
		public DocumentCapability Unhide {
			get { return (DocumentCapability)GetValue(UnhideProperty); }
			set { SetValue(UnhideProperty, value); }
		}
		public DocumentCapability TabColor {
			get { return (DocumentCapability)GetValue(TabColorProperty); }
			set { SetValue(TabColorProperty, value); }
		}
		#endregion
		internal void SetSource(InnerSpreadsheetWorksheetBehaviorOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (Insert != (DocumentCapability)GetDefaultValue(InsertProperty))
				source.Insert = Insert;
			if (Delete != (DocumentCapability)GetDefaultValue(DeleteProperty))
				source.Delete = Delete;
			if (Rename != (DocumentCapability)GetDefaultValue(RenameProperty))
				source.Rename = Rename;
			if (Hide != (DocumentCapability)GetDefaultValue(HideProperty))
				source.Hide = Hide;
			if (Unhide != (DocumentCapability)GetDefaultValue(UnhideProperty))
				source.Unhide = Unhide;
			if (TabColor != (DocumentCapability)GetDefaultValue(TabColorProperty))
				source.TabColor = TabColor;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindInsertProperty();
			BindDeleteProperty();
			BindRenameProperty();
			BindHideProperty();
			BindUnhideProperty();
			BindTabColorProperty();
		}
		void BindInsertProperty() {
			Binding bind = new Binding("Insert") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, InsertProperty, bind);
		}
		void BindDeleteProperty() {
			Binding bind = new Binding("Delete") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, DeleteProperty, bind);
		}
		void BindRenameProperty() {
			Binding bind = new Binding("Rename") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, RenameProperty, bind);
		}
		void BindHideProperty() {
			Binding bind = new Binding("Hide") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, HideProperty, bind);
		}
		void BindUnhideProperty() {
			Binding bind = new Binding("Unhide") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, UnhideProperty, bind);
		}
		void BindTabColorProperty() {
			Binding bind = new Binding("TabColor") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, TabColorProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetRowBehaviorOptions
	public class SpreadsheetRowBehaviorOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty InsertProperty;
		public static readonly DependencyProperty DeleteProperty;
		public static readonly DependencyProperty ResizeProperty;
		public static readonly DependencyProperty AutoFitProperty;
		public static readonly DependencyProperty HideProperty;
		public static readonly DependencyProperty UnhideProperty;
		InnerSpreadsheetRowBehaviorOptions source;
		#endregion
		static SpreadsheetRowBehaviorOptions() {
			Type ownerType = typeof(SpreadsheetRowBehaviorOptions);
			Type capabilityType = typeof(DocumentCapability);
			InsertProperty = DependencyProperty.Register("Insert", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			DeleteProperty = DependencyProperty.Register("Delete", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			ResizeProperty = DependencyProperty.Register("Resize", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			AutoFitProperty = DependencyProperty.Register("AutoFit", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			HideProperty = DependencyProperty.Register("Hide", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			UnhideProperty = DependencyProperty.Register("Unhide", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
		}
		#region Properties
		public DocumentCapability Insert {
			get { return (DocumentCapability)GetValue(InsertProperty); }
			set { SetValue(InsertProperty, value); }
		}
		public DocumentCapability Delete {
			get { return (DocumentCapability)GetValue(DeleteProperty); }
			set { SetValue(DeleteProperty, value); }
		}
		public DocumentCapability Resize {
			get { return (DocumentCapability)GetValue(ResizeProperty); }
			set { SetValue(ResizeProperty, value); }
		}
		public DocumentCapability AutoFit {
			get { return (DocumentCapability)GetValue(AutoFitProperty); }
			set { SetValue(AutoFitProperty, value); }
		}
		public DocumentCapability Hide {
			get { return (DocumentCapability)GetValue(HideProperty); }
			set { SetValue(HideProperty, value); }
		}
		public DocumentCapability Unhide {
			get { return (DocumentCapability)GetValue(UnhideProperty); }
			set { SetValue(UnhideProperty, value); }
		}
		#endregion
		internal void SetSource(InnerSpreadsheetRowBehaviorOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (Insert != (DocumentCapability)GetDefaultValue(InsertProperty))
				source.Insert = Insert;
			if (Delete != (DocumentCapability)GetDefaultValue(DeleteProperty))
				source.Delete = Delete;
			if (Resize != (DocumentCapability)GetDefaultValue(ResizeProperty))
				source.Resize = Resize;
			if (AutoFit != (DocumentCapability)GetDefaultValue(AutoFitProperty))
				source.AutoFit = AutoFit;
			if (Hide != (DocumentCapability)GetDefaultValue(HideProperty))
				source.Hide = Hide;
			if (Unhide != (DocumentCapability)GetDefaultValue(UnhideProperty))
				source.Unhide = Unhide;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindInsertProperty();
			BindDeleteProperty();
			BindResizeProperty();
			BindAutoFitProperty();
			BindHideProperty();
			BindUnhideProperty();
		}
		void BindInsertProperty() {
			Binding bind = new Binding("Insert") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, InsertProperty, bind);
		}
		void BindDeleteProperty() {
			Binding bind = new Binding("Delete") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, DeleteProperty, bind);
		}
		void BindResizeProperty() {
			Binding bind = new Binding("Resize") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ResizeProperty, bind);
		}
		void BindAutoFitProperty() {
			Binding bind = new Binding("AutoFit") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, AutoFitProperty, bind);
		}
		void BindHideProperty() {
			Binding bind = new Binding("Hide") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, HideProperty, bind);
		}
		void BindUnhideProperty() {
			Binding bind = new Binding("Unhide") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, UnhideProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetColumnBehaviorOptions
	public class SpreadsheetColumnBehaviorOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty InsertProperty;
		public static readonly DependencyProperty DeleteProperty;
		public static readonly DependencyProperty ResizeProperty;
		public static readonly DependencyProperty AutoFitProperty;
		public static readonly DependencyProperty HideProperty;
		public static readonly DependencyProperty UnhideProperty;
		InnerSpreadsheetColumnBehaviorOptions source;
		#endregion
		static SpreadsheetColumnBehaviorOptions() {
			Type ownerType = typeof(SpreadsheetColumnBehaviorOptions);
			Type capabilityType = typeof(DocumentCapability);
			InsertProperty = DependencyProperty.Register("Insert", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			DeleteProperty = DependencyProperty.Register("Delete", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			ResizeProperty = DependencyProperty.Register("Resize", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			AutoFitProperty = DependencyProperty.Register("AutoFit", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			HideProperty = DependencyProperty.Register("Hide", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			UnhideProperty = DependencyProperty.Register("Unhide", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
		}
		#region Properties
		public DocumentCapability Insert {
			get { return (DocumentCapability)GetValue(InsertProperty); }
			set { SetValue(InsertProperty, value); }
		}
		public DocumentCapability Delete {
			get { return (DocumentCapability)GetValue(DeleteProperty); }
			set { SetValue(DeleteProperty, value); }
		}
		public DocumentCapability Resize {
			get { return (DocumentCapability)GetValue(ResizeProperty); }
			set { SetValue(ResizeProperty, value); }
		}
		public DocumentCapability AutoFit {
			get { return (DocumentCapability)GetValue(AutoFitProperty); }
			set { SetValue(AutoFitProperty, value); }
		}
		public DocumentCapability Hide {
			get { return (DocumentCapability)GetValue(HideProperty); }
			set { SetValue(HideProperty, value); }
		}
		public DocumentCapability Unhide {
			get { return (DocumentCapability)GetValue(UnhideProperty); }
			set { SetValue(UnhideProperty, value); }
		}
		#endregion
		internal void SetSource(InnerSpreadsheetColumnBehaviorOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (Insert != (DocumentCapability)GetDefaultValue(InsertProperty))
				source.Insert = Insert;
			if (Delete != (DocumentCapability)GetDefaultValue(DeleteProperty))
				source.Delete = Delete;
			if (Resize != (DocumentCapability)GetDefaultValue(ResizeProperty))
				source.Resize = Resize;
			if (AutoFit != (DocumentCapability)GetDefaultValue(AutoFitProperty))
				source.AutoFit = AutoFit;
			if (Hide != (DocumentCapability)GetDefaultValue(HideProperty))
				source.Hide = Hide;
			if (Unhide != (DocumentCapability)GetDefaultValue(UnhideProperty))
				source.Unhide = Unhide;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindInsertProperty();
			BindDeleteProperty();
			BindResizeProperty();
			BindAutoFitProperty();
			BindHideProperty();
			BindUnhideProperty();
		}
		void BindInsertProperty() {
			Binding bind = new Binding("Insert") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, InsertProperty, bind);
		}
		void BindDeleteProperty() {
			Binding bind = new Binding("Delete") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, DeleteProperty, bind);
		}
		void BindResizeProperty() {
			Binding bind = new Binding("Resize") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ResizeProperty, bind);
		}
		void BindAutoFitProperty() {
			Binding bind = new Binding("AutoFit") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, AutoFitProperty, bind);
		}
		void BindHideProperty() {
			Binding bind = new Binding("Hide") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, HideProperty, bind);
		}
		void BindUnhideProperty() {
			Binding bind = new Binding("Unhide") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, UnhideProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetDrawingBehaviorOptions
	public class SpreadsheetDrawingBehaviorOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty MoveProperty;
		public static readonly DependencyProperty ResizeProperty;
		public static readonly DependencyProperty ChangeZOrderProperty;
		public static readonly DependencyProperty RotateProperty;
		InnerSpreadsheetDrawingBehaviorOptions source;
		#endregion
		static SpreadsheetDrawingBehaviorOptions() {
			Type ownerType = typeof(SpreadsheetDrawingBehaviorOptions);
			Type capabilityType = typeof(DocumentCapability);
			MoveProperty = DependencyProperty.Register("Move", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			ResizeProperty = DependencyProperty.Register("Resize", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			ChangeZOrderProperty = DependencyProperty.Register("ChangeZOrder", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			RotateProperty = DependencyProperty.Register("Rotate", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
		}
		#region Properties
		public DocumentCapability Move {
			get { return (DocumentCapability)GetValue(MoveProperty); }
			set { SetValue(MoveProperty, value); }
		}
		public DocumentCapability Resize {
			get { return (DocumentCapability)GetValue(ResizeProperty); }
			set { SetValue(ResizeProperty, value); }
		}
		public DocumentCapability ChangeZOrder {
			get { return (DocumentCapability)GetValue(ChangeZOrderProperty); }
			set { SetValue(ChangeZOrderProperty, value); }
		}
		public DocumentCapability Rotate {
			get { return (DocumentCapability)GetValue(RotateProperty); }
			set { SetValue(RotateProperty, value); }
		}
		#endregion
		internal void SetSource(InnerSpreadsheetDrawingBehaviorOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (Move != (DocumentCapability)GetDefaultValue(MoveProperty))
				source.Move = Move;
			if (Resize != (DocumentCapability)GetDefaultValue(ResizeProperty))
				source.Resize = Resize;
			if (ChangeZOrder != (DocumentCapability)GetDefaultValue(ChangeZOrderProperty))
				source.ChangeZOrder = ChangeZOrder;
			if (Rotate != (DocumentCapability)GetDefaultValue(RotateProperty))
				source.Rotate = Rotate;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindMoveProperty();
			BindResizeProperty();
			BindChangeZOrderProperty();
			BindRotateProperty();
		}
		void BindMoveProperty() {
			Binding bind = new Binding("Move") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, MoveProperty, bind);
		}
		void BindResizeProperty() {
			Binding bind = new Binding("Resize") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ResizeProperty, bind);
		}
		void BindChangeZOrderProperty() {
			Binding bind = new Binding("ChangeZOrder") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ChangeZOrderProperty, bind);
		}
		void BindRotateProperty() {
			Binding bind = new Binding("Rotate") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, RotateProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetSelectionBehaviorOptions
	public class SpreadsheetSelectionBehaviorOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty AllowMultiSelectionProperty;
		public static readonly DependencyProperty AllowExtendSelectionProperty;
		public static readonly DependencyProperty ShowSelectionModeProperty;
		public static readonly DependencyProperty MoveActiveCellModeProperty;
		public static readonly DependencyProperty HideSelectionProperty;
		InnerSpreadsheetSelectionBehaviorOptions source;
		#endregion
		static SpreadsheetSelectionBehaviorOptions() {
			Type ownerType = typeof(SpreadsheetSelectionBehaviorOptions);
			AllowMultiSelectionProperty = DependencyProperty.Register("AllowMultiSelection", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			AllowExtendSelectionProperty = DependencyProperty.Register("AllowExtendSelection", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			ShowSelectionModeProperty = DependencyProperty.Register("ShowSelectionMode", typeof(ShowSelectionMode), ownerType,
				new FrameworkPropertyMetadata(ShowSelectionMode.Always));
			MoveActiveCellModeProperty = DependencyProperty.Register("MoveActiveCellMode", typeof(MoveActiveCellModeOnEnterPress), ownerType,
				new FrameworkPropertyMetadata(MoveActiveCellModeOnEnterPress.Down));
			HideSelectionProperty = DependencyProperty.Register("HideSelection", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
		}
		#region Properties
		public bool AllowMultiSelection {
			get { return (bool)GetValue(AllowMultiSelectionProperty); }
			set { SetValue(AllowMultiSelectionProperty, value); }
		}
		public bool AllowExtendSelection {
			get { return (bool)GetValue(AllowExtendSelectionProperty); }
			set { SetValue(AllowExtendSelectionProperty, value); }
		}
		public ShowSelectionMode ShowSelectionMode {
			get { return (ShowSelectionMode)GetValue(ShowSelectionModeProperty); }
			set { SetValue(ShowSelectionModeProperty, value); }
		}
		public MoveActiveCellModeOnEnterPress MoveActiveCellMode {
			get { return (MoveActiveCellModeOnEnterPress)GetValue(MoveActiveCellModeProperty); }
			set { SetValue(MoveActiveCellModeProperty, value); }
		}
		public bool HideSelection {
			get { return (bool)GetValue(HideSelectionProperty); }
			set { SetValue(HideSelectionProperty, value); }
		}
		#endregion
		internal void SetSource(InnerSpreadsheetSelectionBehaviorOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (AllowMultiSelection != (bool)GetDefaultValue(AllowMultiSelectionProperty))
				source.AllowMultiSelection = AllowMultiSelection;
			if (AllowExtendSelection != (bool)GetDefaultValue(AllowExtendSelectionProperty))
				source.AllowExtendSelection = AllowExtendSelection;
			if (ShowSelectionMode != (ShowSelectionMode)GetDefaultValue(ShowSelectionModeProperty))
				source.ShowSelectionMode = ShowSelectionMode;
			if (MoveActiveCellMode != (MoveActiveCellModeOnEnterPress)GetDefaultValue(MoveActiveCellModeProperty))
				source.MoveActiveCellMode = MoveActiveCellMode;
			if (HideSelection != (bool)GetDefaultValue(HideSelectionProperty))
				source.HideSelection = HideSelection;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindAllowMultiSelectionProperty();
			BindAllowExtendSelectionProperty();
			BindShowSelectionModeProperty();
			BindMoveActiveCellModeProperty();
			BindHideSelectionProperty();
		}
		void BindAllowMultiSelectionProperty() {
			Binding bind = new Binding("AllowMultiSelection") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, AllowMultiSelectionProperty, bind);
		}
		void BindAllowExtendSelectionProperty() {
			Binding bind = new Binding("AllowExtendSelection") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, AllowExtendSelectionProperty, bind);
		}
		void BindShowSelectionModeProperty() {
			Binding bind = new Binding("ShowSelectionMode") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ShowSelectionModeProperty, bind);
		}
		void BindMoveActiveCellModeProperty() {
			Binding bind = new Binding("MoveActiveCellMode") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, MoveActiveCellModeProperty, bind);
		}
		void BindHideSelectionProperty() {
			Binding bind = new Binding("HideSelection") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, HideSelectionProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetCommentBehaviorOptions
	public class SpreadsheetCommentBehaviorOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty InsertProperty;
		public static readonly DependencyProperty EditProperty;
		public static readonly DependencyProperty DeleteProperty;
		public static readonly DependencyProperty ShowHideProperty;
		public static readonly DependencyProperty MoveProperty;
		public static readonly DependencyProperty ResizeProperty;
		InnerSpreadsheetCommentBehaviorOptions source;
		#endregion
		static SpreadsheetCommentBehaviorOptions() {
			Type ownerType = typeof(SpreadsheetCommentBehaviorOptions);
			Type capabilityType = typeof(DocumentCapability);
			InsertProperty = DependencyProperty.Register("Insert", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			EditProperty = DependencyProperty.Register("Edit", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			DeleteProperty = DependencyProperty.Register("Delete", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			ShowHideProperty = DependencyProperty.Register("ShowHide", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			MoveProperty = DependencyProperty.Register("Move", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			ResizeProperty = DependencyProperty.Register("Resize", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
		}
		#region Properties
		public DocumentCapability Insert {
			get { return (DocumentCapability)GetValue(InsertProperty); }
			set { SetValue(InsertProperty, value); }
		}
		public DocumentCapability Edit {
			get { return (DocumentCapability)GetValue(EditProperty); }
			set { SetValue(EditProperty, value); }
		}
		public DocumentCapability Delete {
			get { return (DocumentCapability)GetValue(DeleteProperty); }
			set { SetValue(DeleteProperty, value); }
		}
		public DocumentCapability ShowHide {
			get { return (DocumentCapability)GetValue(ShowHideProperty); }
			set { SetValue(ShowHideProperty, value); }
		}
		public DocumentCapability Move {
			get { return (DocumentCapability)GetValue(MoveProperty); }
			set { SetValue(MoveProperty, value); }
		}
		public DocumentCapability Resize {
			get { return (DocumentCapability)GetValue(ResizeProperty); }
			set { SetValue(ResizeProperty, value); }
		}
		#endregion
		internal void SetSource(InnerSpreadsheetCommentBehaviorOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (Insert != (DocumentCapability)GetDefaultValue(InsertProperty))
				source.Insert = Insert;
			if (Edit != (DocumentCapability)GetDefaultValue(EditProperty))
				source.Edit = Edit;
			if (Delete != (DocumentCapability)GetDefaultValue(DeleteProperty))
				source.Delete = Delete;
			if (ShowHide != (DocumentCapability)GetDefaultValue(ShowHideProperty))
				source.ShowHide = ShowHide;
			if (Move != (DocumentCapability)GetDefaultValue(MoveProperty))
				source.Move = Move;
			if (Resize != (DocumentCapability)GetDefaultValue(ResizeProperty))
				source.Resize = Resize;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindInsertProperty();
			BindEditProperty();
			BindDeleteProperty();
			BindShowHideProperty();
			BindMoveProperty();
			BindResizeProperty();
		}
		void BindInsertProperty() {
			Binding bind = new Binding("Insert") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, InsertProperty, bind);
		}
		void BindEditProperty() {
			Binding bind = new Binding("Edit") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, EditProperty, bind);
		}
		void BindDeleteProperty() {
			Binding bind = new Binding("Delete") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, DeleteProperty, bind);
		}
		void BindShowHideProperty() {
			Binding bind = new Binding("ShowHide") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ShowHideProperty, bind);
		}
		void BindMoveProperty() {
			Binding bind = new Binding("Move") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, MoveProperty, bind);
		}
		void BindResizeProperty() {
			Binding bind = new Binding("Resize") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ResizeProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetGroupBehaviorOptions
	public class SpreadsheetGroupBehaviorOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty GroupProperty;
		public static readonly DependencyProperty UngroupProperty;
		public static readonly DependencyProperty CollapseProperty;
		public static readonly DependencyProperty ExpandProperty;
		public static readonly DependencyProperty ChangeSettingsProperty;
		public static readonly DependencyProperty CollapseExpandOnProtectedSheetProperty;
		InnerSpreadsheetGroupBehaviorOptions source;
		#endregion
		static SpreadsheetGroupBehaviorOptions() {
			Type ownerType = typeof(SpreadsheetGroupBehaviorOptions);
			Type capabilityType = typeof(DocumentCapability);
			GroupProperty = DependencyProperty.Register("Group", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			UngroupProperty = DependencyProperty.Register("Ungroup", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			CollapseProperty = DependencyProperty.Register("Collapse", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			ExpandProperty = DependencyProperty.Register("Expand", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			ChangeSettingsProperty = DependencyProperty.Register("ChangeSettings", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			CollapseExpandOnProtectedSheetProperty = DependencyProperty.Register("CollapseExpandOnProtectedSheet", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
		}
		#region Properties
		public DocumentCapability Group {
			get { return (DocumentCapability)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		public DocumentCapability Ungroup {
			get { return (DocumentCapability)GetValue(UngroupProperty); }
			set { SetValue(UngroupProperty, value); }
		}
		public DocumentCapability Collapse {
			get { return (DocumentCapability)GetValue(CollapseProperty); }
			set { SetValue(CollapseProperty, value); }
		}
		public DocumentCapability Expand {
			get { return (DocumentCapability)GetValue(ExpandProperty); }
			set { SetValue(ExpandProperty, value); }
		}
		public DocumentCapability ChangeSettings {
			get { return (DocumentCapability)GetValue(ChangeSettingsProperty); }
			set { SetValue(ChangeSettingsProperty, value); }
		}
		public DocumentCapability CollapseExpandOnProtectedSheet {
			get { return (DocumentCapability)GetValue(CollapseExpandOnProtectedSheetProperty); }
			set { SetValue(CollapseExpandOnProtectedSheetProperty, value); }
		}
		#endregion
		internal void SetSource(InnerSpreadsheetGroupBehaviorOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (Group != (DocumentCapability)GetDefaultValue(GroupProperty))
				source.Group = Group;
			if (Ungroup != (DocumentCapability)GetDefaultValue(UngroupProperty))
				source.Ungroup = Ungroup;
			if (Collapse != (DocumentCapability)GetDefaultValue(CollapseProperty))
				source.Collapse = Collapse;
			if (Expand != (DocumentCapability)GetDefaultValue(ExpandProperty))
				source.Expand = Expand;
			if (ChangeSettings != (DocumentCapability)GetDefaultValue(ChangeSettingsProperty))
				source.ChangeSettings = ChangeSettings;
			if (CollapseExpandOnProtectedSheet != (DocumentCapability)GetDefaultValue(CollapseExpandOnProtectedSheetProperty))
				source.CollapseExpandOnProtectedSheet = CollapseExpandOnProtectedSheet;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindGroupProperty();
			BindUngroupProperty();
			BindCollapseProperty();
			BindExpandProperty();
			BindChangeSettingsProperty();
			BindCollapseExpandOnProtectedSheetProperty();
		}
		void BindGroupProperty() {
			Binding bind = new Binding("Group") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, GroupProperty, bind);
		}
		void BindUngroupProperty() {
			Binding bind = new Binding("Ungroup") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, UngroupProperty, bind);
		}
		void BindCollapseProperty() {
			Binding bind = new Binding("Collapse") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CollapseProperty, bind);
		}
		void BindExpandProperty() {
			Binding bind = new Binding("Expand") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ExpandProperty, bind);
		}
		void BindChangeSettingsProperty() {
			Binding bind = new Binding("ChangeSettings") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ChangeSettingsProperty, bind);
		}
		void BindCollapseExpandOnProtectedSheetProperty() {
			Binding bind = new Binding("CollapseExpandOnProtectedSheet") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CollapseExpandOnProtectedSheetProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetProtectionBehaviorOptions
	public class SpreadsheetProtectionBehaviorOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty ProtectSheetProperty;
		public static readonly DependencyProperty UnprotectSheetProperty;
		public static readonly DependencyProperty ProtectWorkbookProperty;
		public static readonly DependencyProperty UnprotectWorkbookProperty;
		public static readonly DependencyProperty AllowUsersToEditRangeProperty;
		InnerSpreadsheetProtectionBehaviorOptions source;
		#endregion
		static SpreadsheetProtectionBehaviorOptions() {
			Type ownerType = typeof(SpreadsheetProtectionBehaviorOptions);
			Type capabilityType = typeof(DocumentCapability);
			ProtectSheetProperty = DependencyProperty.Register("ProtectSheet", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			UnprotectSheetProperty = DependencyProperty.Register("UnprotectSheet", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			ProtectWorkbookProperty = DependencyProperty.Register("ProtectWorkbook", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			UnprotectWorkbookProperty = DependencyProperty.Register("UnprotectWorkbook", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
			AllowUsersToEditRangeProperty = DependencyProperty.Register("AllowUsersToEditRange", capabilityType, ownerType,
				new FrameworkPropertyMetadata(DocumentCapability.Default));
		}
		#region Properties
		public DocumentCapability ProtectSheet {
			get { return (DocumentCapability)GetValue(ProtectSheetProperty); }
			set { SetValue(ProtectSheetProperty, value); }
		}
		public DocumentCapability UnprotectSheet {
			get { return (DocumentCapability)GetValue(UnprotectSheetProperty); }
			set { SetValue(UnprotectSheetProperty, value); }
		}
		public DocumentCapability ProtectWorkbook {
			get { return (DocumentCapability)GetValue(ProtectWorkbookProperty); }
			set { SetValue(ProtectWorkbookProperty, value); }
		}
		public DocumentCapability UnprotectWorkbook {
			get { return (DocumentCapability)GetValue(UnprotectWorkbookProperty); }
			set { SetValue(UnprotectWorkbookProperty, value); }
		}
		public DocumentCapability AllowUsersToEditRange {
			get { return (DocumentCapability)GetValue(AllowUsersToEditRangeProperty); }
			set { SetValue(AllowUsersToEditRangeProperty, value); }
		}
		#endregion
		internal void SetSource(InnerSpreadsheetProtectionBehaviorOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (ProtectSheet != (DocumentCapability)GetDefaultValue(ProtectSheetProperty))
				source.ProtectSheet = ProtectSheet;
			if (UnprotectSheet != (DocumentCapability)GetDefaultValue(UnprotectSheetProperty))
				source.UnprotectSheet = UnprotectSheet;
			if (ProtectWorkbook != (DocumentCapability)GetDefaultValue(ProtectWorkbookProperty))
				source.ProtectWorkbook = ProtectWorkbook;
			if (UnprotectWorkbook != (DocumentCapability)GetDefaultValue(UnprotectWorkbookProperty))
				source.UnprotectWorkbook = UnprotectWorkbook;
			if (AllowUsersToEditRange != (DocumentCapability)GetDefaultValue(AllowUsersToEditRangeProperty))
				source.AllowUsersToEditRange = AllowUsersToEditRange;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindProtectSheetProperty();
			BindUnprotectSheetProperty();
			BindProtectWorkbookProperty();
			BindUnprotectWorkbookProperty();
			BindAllowUsersToEditRangeProperty();
		}
		void BindProtectSheetProperty() {
			Binding bind = new Binding("ProtectSheet") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ProtectSheetProperty, bind);
		}
		void BindUnprotectSheetProperty() {
			Binding bind = new Binding("UnprotectSheet") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, UnprotectSheetProperty, bind);
		}
		void BindProtectWorkbookProperty() {
			Binding bind = new Binding("ProtectWorkbook") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ProtectWorkbookProperty, bind);
		}
		void BindUnprotectWorkbookProperty() {
			Binding bind = new Binding("UnprotectWorkbook") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, UnprotectWorkbookProperty, bind);
		}
		void BindAllowUsersToEditRangeProperty() {
			Binding bind = new Binding("AllowUsersToEditRange") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, AllowUsersToEditRangeProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
}
