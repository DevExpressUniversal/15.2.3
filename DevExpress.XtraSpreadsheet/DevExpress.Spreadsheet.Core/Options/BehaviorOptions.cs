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
using System.Text;
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet {
	#region FunctionNameCulture
	public enum FunctionNameCulture {
		English,
		Local,
		Auto,
	}
	#endregion
	#region SpreadsheetBehaviorOptions
	[ComVisible(true)]
	public class SpreadsheetBehaviorOptions : SpreadsheetNotificationOptions {
		#region Fields
		const float defaultMaxZoomFactor = float.PositiveInfinity;
		const float defaultMinZoomFactor = 0.09f;
		DocumentCapability allowDrag;
		DocumentCapability allowDrop;
		DocumentCapability allowCopy;
		DocumentCapability allowPaste;
		DocumentCapability allowCut;
		DocumentCapability allowPrint;
		DocumentCapability allowZooming;
		DocumentCapability allowSaveAs;
		DocumentCapability allowSave;
		DocumentCapability allowCreateNew;
		DocumentCapability allowOpen;
		DocumentCapability allowShowPopupMenu;
		DocumentCapability allowOfficeScrolling;
		DocumentCapability allowTouch;
		DocumentCapability allowFreezePanes;
		SpreadsheetWorksheetBehaviorOptions worksheet = new SpreadsheetWorksheetBehaviorOptions();
		SpreadsheetRowBehaviorOptions row = new SpreadsheetRowBehaviorOptions();
		SpreadsheetColumnBehaviorOptions column = new SpreadsheetColumnBehaviorOptions();
		SpreadsheetCellEditorBehaviorOptions cellEditor = new SpreadsheetCellEditorBehaviorOptions();
		SpreadsheetFillHandleBehaviorOptions fillHandle = new SpreadsheetFillHandleBehaviorOptions();
		SpreadsheetDrawingBehaviorOptions drawing = new SpreadsheetDrawingBehaviorOptions();
		SpreadsheetSelectionOptions selection = new SpreadsheetSelectionOptions();
		SpreadsheetCommentBehaviorOptions comment = new SpreadsheetCommentBehaviorOptions();
		SpreadsheetGroupBehaviorOptions group = new SpreadsheetGroupBehaviorOptions();
		SpreadsheetProtectionBehaviorOptions protection = new SpreadsheetProtectionBehaviorOptions();
		float minZoomFactor;
		float maxZoomFactor;
		bool useSkinColors;
		FunctionNameCulture functionNameCulture;
		#endregion
		public SpreadsheetBehaviorOptions() {
			Worksheet.Changed += OnWorksheetChanged;
			Row.Changed += OnRowChanged;
			Column.Changed += OnColumnChanged;
			CellEditor.Changed += OnCellEditorChanged;
			FillHandle.Changed += OnFillHandleChanged;
			Drawing.Changed += OnDrawingChanged;
			Selection.Changed += OnSelectionChanged;
			Comment.Changed += OnCommentChanged;
			Group.Changed += OnGroupChanged;
			Protection.Changed += OnProtectionChanged;
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float DefaultMinZoomFactor { get { return defaultMinZoomFactor; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float DefaultMaxZoomFactor { get { return defaultMaxZoomFactor; } }
		#region UseSkinColors
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsUseSkinColors"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool UseSkinColors {
			get { return useSkinColors; }
			set {
				if (useSkinColors == value)
					return;
				bool oldValue = this.useSkinColors;
				this.useSkinColors = value;
				OnChanged("UseSkinColors", oldValue, value);
			}
		}
		#endregion
		#region Drag
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsDrag"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Drag {
			get { return allowDrag; }
			set {
				if (this.allowDrag == value)
					return;
				DocumentCapability oldValue = this.allowDrag;
				this.allowDrag = value;
				OnChanged("Drag", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DragAllowed { get { return IsAllowed(Drag); } }
		#endregion
		#region Drop
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsDrop"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Drop {
			get { return allowDrop; }
			set {
				if (this.allowDrop == value)
					return;
				DocumentCapability oldValue = this.allowDrop;
				this.allowDrop = value;
				OnChanged("Drop", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DropAllowed { get { return IsAllowed(Drop); } }
		#endregion
		#region Copy
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsCopy"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Copy {
			get { return allowCopy; }
			set {
				if (this.allowCopy == value)
					return;
				DocumentCapability oldValue = this.allowCopy;
				this.allowCopy = value;
				OnChanged("Copy", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CopyAllowed { get { return IsAllowed(Copy); } }
		#endregion
		#region Printing
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsPrint"),
#endif
 NotifyParentProperty(true)]
		public DocumentCapability Print {
			get { return allowPrint; }
			set {
				if (this.allowPrint == value)
					return;
				DocumentCapability oldValue = this.allowPrint;
				this.allowPrint = value;
				OnChanged("Print", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializePrint() { return Print != DocumentCapability.Default; }
		protected internal virtual void ResetPrint() { Print = DocumentCapability.Default; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool PrintAllowed { get { return IsAllowed(Print); } }
		[Obsolete("Use the Print property instead.", true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DocumentCapability Printing { get { return Print; } set { Print = value; } }
		protected internal virtual bool ShouldSerializePrinting() { return false; }
		protected internal virtual void ResetPrinting() { ResetPrinting(); }
		[Obsolete("Use the PrintAllowed property instead.", true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool PrintingAllowed { get { return IsAllowed(Print); } }
		#endregion
		#region SaveAs
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsSaveAs"),
#endif
 NotifyParentProperty(true)]
		public DocumentCapability SaveAs {
			get { return allowSaveAs; }
			set {
				if (this.allowSaveAs == value)
					return;
				DocumentCapability oldValue = this.allowSaveAs;
				this.allowSaveAs = value;
				OnChanged("SaveAs", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeSaveAs() { return SaveAs != DocumentCapability.Default; }
		protected internal virtual void ResetSaveAs() { SaveAs = DocumentCapability.Default; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SaveAsAllowed { get { return IsAllowed(SaveAs); } }
		#endregion
		#region Save
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsSave"),
#endif
 NotifyParentProperty(true)]
		public DocumentCapability Save {
			get { return allowSave; }
			set {
				if (this.allowSave == value)
					return;
				DocumentCapability oldValue = this.allowSave;
				this.allowSave = value;
				OnChanged("Save", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeSave() { return Save != DocumentCapability.Default; }
		protected internal virtual void ResetSave() { Save = DocumentCapability.Default; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SaveAllowed { get { return IsAllowed(Save); } }
		#endregion
		#region Zooming
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsZooming"),
#endif
 NotifyParentProperty(true)]
		public DocumentCapability Zooming {
			get { return allowZooming; }
			set {
				if (this.allowZooming == value)
					return;
				DocumentCapability oldValue = this.allowZooming;
				this.allowZooming = value;
				OnChanged("Zooming", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeZooming() { return Zooming != DocumentCapability.Default; }
		protected internal virtual void ResetZooming() { Zooming = DocumentCapability.Default; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ZoomingAllowed { get { return IsAllowed(Zooming); } }
		#endregion
		#region ShowPopupMenu
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsShowPopupMenu"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability ShowPopupMenu {
			get { return allowShowPopupMenu; }
			set {
				if (this.allowShowPopupMenu == value)
					return;
				DocumentCapability oldValue = this.allowShowPopupMenu;
				this.allowShowPopupMenu = value;
				OnChanged("ShowPopupMenu", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowPopupMenuAllowed { get { return IsAllowed(ShowPopupMenu); } }
		#endregion
		#region OfficeScrolling
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsOfficeScrolling"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability OfficeScrolling {
			get { return allowOfficeScrolling; }
			set {
				if (this.allowOfficeScrolling == value)
					return;
				DocumentCapability oldValue = this.allowOfficeScrolling;
				this.allowOfficeScrolling = value;
				OnChanged("OfficeScrolling", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool OfficeScrollingAllowed { get { return IsAllowed(OfficeScrolling); } }
		#endregion
		#region Touch
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsTouch"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Touch {
			get { return allowTouch; }
			set {
				if (this.allowTouch == value)
					return;
				DocumentCapability oldValue = this.allowTouch;
				this.allowTouch = value;
				OnChanged("Touch", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool TouchAllowed { get { return IsAllowed(Touch); } }
		#endregion
		#region FreezePanes
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsFreezePanes"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability FreezePanes {
			get { return allowFreezePanes; }
			set {
				if (this.allowFreezePanes == value)
					return;
				DocumentCapability oldValue = this.allowFreezePanes;
				this.allowFreezePanes = value;
				OnChanged("FreezePanes", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool FreezePanesAllowed { get { return IsAllowed(FreezePanes); } }
		#endregion
		#region MinZoomFactor
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsMinZoomFactor"),
#endif
 DefaultValue(defaultMinZoomFactor), NotifyParentProperty(true)]
		public float MinZoomFactor {
			get { return minZoomFactor; }
			set {
				float newZoom = Math.Max(defaultMinZoomFactor, value);
				newZoom = Math.Min(newZoom, MaxZoomFactor);
				if (this.minZoomFactor == newZoom)
					return;
				float oldValue = this.minZoomFactor;
				this.minZoomFactor = newZoom;
				OnChanged("MinZoomFactor", oldValue, newZoom);
			}
		}
		#endregion
		#region MaxZoomFactor
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsMaxZoomFactor"),
#endif
 DefaultValue(defaultMaxZoomFactor), NotifyParentProperty(true)]
		public float MaxZoomFactor {
			get { return maxZoomFactor; }
			set {
				float newZoom = Math.Max(minZoomFactor, value);
				if (this.maxZoomFactor == newZoom)
					return;
				float oldValue = this.maxZoomFactor;
				this.maxZoomFactor = newZoom;
				OnChanged("MaxZoomFactor", oldValue, newZoom);
			}
		}
		#endregion
		#region Paste
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsPaste"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Paste {
			get { return allowPaste; }
			set {
				if (this.allowPaste == value)
					return;
				DocumentCapability oldValue = this.allowPaste;
				this.allowPaste = value;
				OnChanged("Paste", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool PasteAllowed { get { return IsAllowed(Paste); } }
		#endregion
		#region Cut
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsCut"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Cut {
			get { return allowCut; }
			set {
				if (this.allowCut == value)
					return;
				DocumentCapability oldValue = this.allowCut;
				this.allowCut = value;
				OnChanged("Cut", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CutAllowed { get { return IsAllowed(Cut); } }
		#endregion
		#region CreateNew
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsCreateNew"),
#endif
 NotifyParentProperty(true)]
		public DocumentCapability CreateNew {
			get { return allowCreateNew; }
			set {
				if (this.allowCreateNew == value)
					return;
				DocumentCapability oldValue = this.allowCreateNew;
				this.allowCreateNew = value;
				OnChanged("CreateNew", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeCreateNew() { return CreateNew != DocumentCapability.Default; }
		protected internal virtual void ResetCreateNew() { CreateNew = DocumentCapability.Default; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CreateNewAllowed { get { return IsAllowed(CreateNew); } }
		#endregion
		#region Open
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsOpen"),
#endif
 NotifyParentProperty(true)]
		public DocumentCapability Open {
			get { return allowOpen; }
			set {
				if (this.allowOpen == value)
					return;
				DocumentCapability oldValue = this.allowOpen;
				this.allowOpen = value;
				OnChanged("Open", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeOpen() { return Open != DocumentCapability.Default; }
		protected internal virtual void ResetOpen() { Open = DocumentCapability.Default; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool OpenAllowed { get { return IsAllowed(Open); } }
		#endregion
		#region FunctionNameCulture
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsFunctionNameCulture"),
#endif
 DefaultValue(FunctionNameCulture.Auto)]
		public FunctionNameCulture FunctionNameCulture {
			get { return functionNameCulture; }
			set {
				if (this.FunctionNameCulture == value)
					return;
				FunctionNameCulture oldValue = this.FunctionNameCulture;
				this.functionNameCulture = value;
				OnChanged("FunctionNameCulture", oldValue, value);
			}
		}
		#endregion
		#region Worksheet
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsWorksheet"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetWorksheetBehaviorOptions Worksheet { get { return worksheet; } }
		#endregion
		#region Row
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsRow"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetRowBehaviorOptions Row { get { return row; } }
		#endregion
		#region Column
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsColumn"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetColumnBehaviorOptions Column { get { return column; } }
		#endregion
		#region CellEditor
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsCellEditor"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetCellEditorBehaviorOptions CellEditor { get { return cellEditor; } }
		#endregion
		#region FillHandle
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsFillHandle"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetFillHandleBehaviorOptions FillHandle { get { return fillHandle; } }
		#endregion
		#region Drawing
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsDrawing"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetDrawingBehaviorOptions Drawing { get { return drawing; } }
		#endregion
		#region Selection
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsSelection"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetSelectionOptions Selection { get { return selection; } }
		#endregion
		#region Comment
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsComment"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetCommentBehaviorOptions Comment { get { return comment; } }
		#endregion
		#region Group
		[
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetGroupBehaviorOptions Group { get { return group; } }
		#endregion
		#region Protection
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetBehaviorOptionsProtection"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetProtectionBehaviorOptions Protection { get { return protection; } }
		#endregion
		#endregion
		void OnWorksheetChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged("Worksheet." + e.Name, e.OldValue, e.NewValue);
		}
		void OnRowChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged("Row." + e.Name, e.OldValue, e.NewValue);
		}
		void OnColumnChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged("Column." + e.Name, e.OldValue, e.NewValue);
		}
		void OnCellEditorChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged("CellEditor." + e.Name, e.OldValue, e.NewValue);
		}
		void OnFillHandleChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged("FillHandle." + e.Name, e.OldValue, e.NewValue);
		}
		void OnDrawingChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged("Drawing." + e.Name, e.OldValue, e.NewValue);
		}
		void OnSelectionChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged("Selection." + e.Name, e.OldValue, e.NewValue);
		}
		void OnCommentChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged("Comment." + e.Name, e.OldValue, e.NewValue);
		}
		void OnGroupChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged("Group." + e.Name, e.OldValue, e.NewValue);
		}
		void OnProtectionChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged("Protection." + e.Name, e.OldValue, e.NewValue);
		}
		protected internal override void ResetCore() {
			this.Drag = DocumentCapability.Default;
			this.Drop = DocumentCapability.Default;
			this.Copy = DocumentCapability.Default;
			this.Paste = DocumentCapability.Default;
			this.Cut = DocumentCapability.Default;
			ResetPrint();
			ResetZooming();
			ResetSaveAs();
			ResetSave();
			ResetCreateNew();
			ResetOpen();
			this.ShowPopupMenu = DocumentCapability.Default;
			this.OfficeScrolling = DocumentCapability.Default;
			this.Touch = DocumentCapability.Default;
			this.FreezePanes = DocumentCapability.Default;
			this.MaxZoomFactor = DefaultMaxZoomFactor;
			this.MinZoomFactor = DefaultMinZoomFactor;
			this.FunctionNameCulture = FunctionNameCulture.Auto;
			this.UseSkinColors = true;
			Worksheet.Reset();
			Row.Reset();
			Column.Reset();
			CellEditor.Reset();
			FillHandle.Reset();
			Drawing.Reset();
			Selection.Reset();
			Comment.Reset();
			Group.Reset();
			Protection.Reset();
		}
		protected internal void CopyFrom(SpreadsheetBehaviorOptions value) {
			this.allowDrag = value.Drag;
			this.allowDrop = value.Drop;
			this.allowCopy = value.Copy;
			this.allowPaste = value.Paste;
			this.allowCut = value.Cut;
			this.allowPrint = value.Print;
			this.allowZooming = value.Zooming;
			this.allowSaveAs = value.SaveAs;
			this.allowSave = value.Save;
			this.allowCreateNew = value.CreateNew;
			this.allowOpen = value.Open;
			this.allowShowPopupMenu = value.ShowPopupMenu;
			this.allowOfficeScrolling = value.OfficeScrolling;
			this.allowTouch = value.Touch;
			this.allowFreezePanes = value.FreezePanes;
			this.Worksheet.CopyFrom(value.Worksheet);
			this.Row.CopyFrom(value.Row);
			this.Column.CopyFrom(value.Column);
			this.CellEditor.CopyFrom(value.CellEditor);
			this.FillHandle.CopyFrom(value.FillHandle);
			this.Drawing.CopyFrom(value.Drawing);
			this.Selection.CopyFrom(value.Selection);
			this.Comment.CopyFrom(value.Comment);
			this.Group.CopyFrom(value.Group);
			this.Protection.CopyFrom(value.Protection);
			this.minZoomFactor = value.MinZoomFactor;
			this.maxZoomFactor = value.MaxZoomFactor;
			this.functionNameCulture = value.FunctionNameCulture;
			this.useSkinColors = value.UseSkinColors;
		}
	}
	#endregion
	#region SpreadsheetWorksheetBehaviorOptions
	[ComVisible(true)]
	public class SpreadsheetWorksheetBehaviorOptions : SpreadsheetNotificationOptions {
		#region Fields
		DocumentCapability insert;
		DocumentCapability delete;
		DocumentCapability rename;
		DocumentCapability hide;
		DocumentCapability unhide;
		DocumentCapability move;
		DocumentCapability copy;
		DocumentCapability tabColor;
		#endregion
		#region Properties
		#region Insert
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetWorksheetBehaviorOptionsInsert"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Insert {
			get { return insert; }
			set {
				if (this.Insert == value)
					return;
				DocumentCapability oldValue = this.Insert;
				this.insert = value;
				OnChanged("Insert", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool InsertAllowed { get { return IsAllowed(Insert); } }
		#endregion
		#region Delete
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetWorksheetBehaviorOptionsDelete"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Delete {
			get { return delete; }
			set {
				if (this.Delete == value)
					return;
				DocumentCapability oldValue = this.Delete;
				this.delete = value;
				OnChanged("Delete", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DeleteAllowed { get { return IsAllowed(Delete); } }
		#endregion
		#region Rename
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetWorksheetBehaviorOptionsRename"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Rename {
			get { return rename; }
			set {
				if (this.Rename == value)
					return;
				DocumentCapability oldValue = this.Rename;
				this.rename = value;
				OnChanged("Rename", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RenameAllowed { get { return IsAllowed(Rename); } }
		#endregion
		#region Hide
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetWorksheetBehaviorOptionsHide"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Hide {
			get { return hide; }
			set {
				if (this.Hide == value)
					return;
				DocumentCapability oldValue = this.Hide;
				this.hide = value;
				OnChanged("Hide", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HideAllowed { get { return IsAllowed(Hide); } }
		#endregion
		#region Unhide
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetWorksheetBehaviorOptionsUnhide"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Unhide {
			get { return unhide; }
			set {
				if (this.Unhide == value)
					return;
				DocumentCapability oldValue = this.Unhide;
				this.unhide = value;
				OnChanged("Unhide", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UnhideAllowed { get { return IsAllowed(Unhide); } }
		#endregion
		#region Move
		[
		DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		protected internal DocumentCapability Move {
			get { return move; }
			set {
				if (this.Move == value)
					return;
				DocumentCapability oldValue = this.Move;
				this.move = value;
				OnChanged("Move", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal bool MoveAllowed { get { return IsAllowed(Move); } }
		#endregion
		#region Copy
		[
		DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		protected internal DocumentCapability Copy {
			get { return copy; }
			set {
				if (this.Copy == value)
					return;
				DocumentCapability oldValue = this.Copy;
				this.copy = value;
				OnChanged("Copy", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal bool CopyAllowed { get { return IsAllowed(Copy); } }
		#endregion
		#region TabColor
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetWorksheetBehaviorOptionsTabColor"),
#endif
		DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability TabColor {
			get { return tabColor; }
			set {
				if (this.tabColor == value)
					return;
				DocumentCapability oldValue = this.TabColor;
				this.tabColor = value;
				OnChanged("TabColor", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool TabColorAllowed { get { return IsAllowed(TabColor); } }
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.Insert = DocumentCapability.Default;
			this.Delete = DocumentCapability.Default;
			this.Rename = DocumentCapability.Default;
			this.Hide = DocumentCapability.Default;
			this.Unhide = DocumentCapability.Default;
			this.Move = DocumentCapability.Default;
			this.Copy = DocumentCapability.Default;
			this.TabColor = DocumentCapability.Default;
		}
		protected internal void CopyFrom(SpreadsheetWorksheetBehaviorOptions value) {
			this.insert = value.Insert;
			this.delete = value.Delete;
			this.rename = value.Rename;
			this.hide = value.Hide;
			this.unhide = value.Unhide;
			this.move = value.Move;
			this.copy = value.Copy;
			this.TabColor = value.TabColor;
		}
	}
	#endregion
	#region SpreadsheetRowBehaviorOptions
	[ComVisible(true)]
	public class SpreadsheetRowBehaviorOptions : SpreadsheetNotificationOptions {
		#region Fields
		DocumentCapability insert;
		DocumentCapability delete;
		DocumentCapability resize;
		DocumentCapability autoFit;
		DocumentCapability hide;
		DocumentCapability unhide;
		#endregion
		#region Properties
		#region Insert
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetRowBehaviorOptionsInsert"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Insert {
			get { return insert; }
			set {
				if (this.Insert == value)
					return;
				DocumentCapability oldValue = this.Insert;
				this.insert = value;
				OnChanged("Insert", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool InsertAllowed { get { return IsAllowed(Insert); } }
		#endregion
		#region Delete
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetRowBehaviorOptionsDelete"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Delete {
			get { return delete; }
			set {
				if (this.Delete == value)
					return;
				DocumentCapability oldValue = this.Delete;
				this.delete = value;
				OnChanged("Delete", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DeleteAllowed { get { return IsAllowed(Delete); } }
		#endregion
		#region Resize
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetRowBehaviorOptionsResize"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Resize {
			get { return resize; }
			set {
				if (this.Resize == value)
					return;
				DocumentCapability oldValue = this.Resize;
				this.resize = value;
				OnChanged("Resize", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ResizeAllowed { get { return IsAllowed(Resize); } }
		#endregion
		#region AutoFit
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetRowBehaviorOptionsAutoFit"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability AutoFit {
			get { return autoFit; }
			set {
				if (this.AutoFit == value)
					return;
				DocumentCapability oldValue = this.AutoFit;
				this.autoFit = value;
				OnChanged("AutoFit", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AutoFitAllowed { get { return IsAllowed(AutoFit); } }
		#endregion
		#region Hide
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetRowBehaviorOptionsHide"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Hide {
			get { return hide; }
			set {
				if (this.Hide == value)
					return;
				DocumentCapability oldValue = this.Hide;
				this.hide = value;
				OnChanged("Hide", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HideAllowed { get { return IsAllowed(Hide); } }
		#endregion
		#region Unhide
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetRowBehaviorOptionsUnhide")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Unhide {
			get { return unhide; }
			set {
				if (this.Unhide == value)
					return;
				DocumentCapability oldValue = this.Unhide;
				this.unhide = value;
				OnChanged("Unhide", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UnhideAllowed { get { return IsAllowed(Unhide); } }
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.Insert = DocumentCapability.Default;
			this.Delete = DocumentCapability.Default;
			this.Resize = DocumentCapability.Default;
			this.AutoFit = DocumentCapability.Default;
			this.Hide = DocumentCapability.Default;
			this.Unhide = DocumentCapability.Default;
		}
		protected internal void CopyFrom(SpreadsheetRowBehaviorOptions value) {
			this.insert = value.Insert;
			this.delete = value.Delete;
			this.resize = value.Resize;
			this.autoFit = value.AutoFit;
			this.hide = value.Hide;
			this.unhide = value.Unhide;
		}
	}
	#endregion
	#region SpreadsheetColumnBehaviorOptions
	[ComVisible(true)]
	public class SpreadsheetColumnBehaviorOptions : SpreadsheetNotificationOptions {
		#region Fields
		DocumentCapability insert;
		DocumentCapability delete;
		DocumentCapability resize;
		DocumentCapability autoFit;
		DocumentCapability hide;
		DocumentCapability unhide;
		#endregion
		#region Properties
		#region Insert
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetColumnBehaviorOptionsInsert"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Insert {
			get { return insert; }
			set {
				if (this.Insert == value)
					return;
				DocumentCapability oldValue = this.Insert;
				this.insert = value;
				OnChanged("Insert", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool InsertAllowed { get { return IsAllowed(Insert); } }
		#endregion
		#region Delete
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetColumnBehaviorOptionsDelete"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Delete {
			get { return delete; }
			set {
				if (this.Delete == value)
					return;
				DocumentCapability oldValue = this.Delete;
				this.delete = value;
				OnChanged("Delete", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DeleteAllowed { get { return IsAllowed(Delete); } }
		#endregion
		#region Resize
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetColumnBehaviorOptionsResize"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Resize {
			get { return resize; }
			set {
				if (this.Resize == value)
					return;
				DocumentCapability oldValue = this.Resize;
				this.resize = value;
				OnChanged("Resize", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ResizeAllowed { get { return IsAllowed(Resize); } }
		#endregion
		#region AutoFit
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetColumnBehaviorOptionsAutoFit"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability AutoFit {
			get { return autoFit; }
			set {
				if (this.AutoFit == value)
					return;
				DocumentCapability oldValue = this.AutoFit;
				this.autoFit = value;
				OnChanged("AutoFit", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AutoFitAllowed { get { return IsAllowed(AutoFit); } }
		#endregion
		#region Hide
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetColumnBehaviorOptionsHide"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Hide {
			get { return hide; }
			set {
				if (this.Hide == value)
					return;
				DocumentCapability oldValue = this.Hide;
				this.hide = value;
				OnChanged("Hide", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HideAllowed { get { return IsAllowed(Hide); } }
		#endregion
		#region Unhide
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetColumnBehaviorOptionsUnhide")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Unhide {
			get { return unhide; }
			set {
				if (this.Unhide == value)
					return;
				DocumentCapability oldValue = this.Unhide;
				this.unhide = value;
				OnChanged("Unhide", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UnhideAllowed { get { return IsAllowed(Unhide); } }
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.Insert = DocumentCapability.Default;
			this.Delete = DocumentCapability.Default;
			this.Resize = DocumentCapability.Default;
			this.AutoFit = DocumentCapability.Default;
			this.Hide = DocumentCapability.Default;
			this.Unhide = DocumentCapability.Default;
		}
		protected internal void CopyFrom(SpreadsheetColumnBehaviorOptions value) {
			this.insert = value.Insert;
			this.delete = value.Delete;
			this.resize = value.Resize;
			this.autoFit = value.AutoFit;
			this.hide = value.Hide;
			this.unhide = value.Unhide;
		}
	}
	#endregion
	#region SpreadsheetCellEditorBehaviorOptions
	[ComVisible(true)]
	public class SpreadsheetCellEditorBehaviorOptions : SpreadsheetNotificationOptions {
		#region Fields
		CellEditorCommitMode commitMode;
		#endregion
		#region Properties
		#region Insert
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCellEditorBehaviorOptionsCommitMode"),
#endif
DefaultValue(CellEditorCommitMode.Auto), NotifyParentProperty(true)]
		public CellEditorCommitMode CommitMode {
			get { return commitMode; }
			set {
				if (this.CommitMode == value)
					return;
				CellEditorCommitMode oldValue = this.CommitMode;
				this.commitMode = value;
				OnChanged("CommitMode", oldValue, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.CommitMode = CellEditorCommitMode.Auto;
		}
		protected internal void CopyFrom(SpreadsheetCellEditorBehaviorOptions value) {
			this.commitMode = value.CommitMode;
		}
	}
	#endregion
	#region SpreadsheetBaseValueSource
	[ComVisible(true)]
	public enum SpreadsheetBaseValueSource {
		Auto = 0,
		Document,
		Control
	}
	#endregion
	#region SpreadsheetFillHandleBehaviorOptions
	[ComVisible(true)]
	public class SpreadsheetFillHandleBehaviorOptions : SpreadsheetNotificationOptions {
		#region Fields
		bool enabled;
		#endregion
		#region Properties
		#region Enabled
		[
DefaultValue(true), NotifyParentProperty(true)]
		public bool Enabled {
			get { return enabled; }
			set {
				if (this.Enabled == value)
					return;
				this.enabled = value;
				OnChanged("Enabled", !value, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.Enabled = true;
		}
		protected internal void CopyFrom(SpreadsheetFillHandleBehaviorOptions value) {
			this.enabled = value.Enabled;
		}
	}
	#endregion
	#region SpreadsheetDrawingBehaviorOptions
	[ComVisible(true)]
	public class SpreadsheetDrawingBehaviorOptions : SpreadsheetNotificationOptions {
		#region Fields
		DocumentCapability allowMove;
		DocumentCapability allowResize;
		DocumentCapability allowChangeZOrder;
		DocumentCapability allowRotate;
		#endregion
		#region Properties
		#region Move
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetDrawingBehaviorOptionsMove")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Move {
			get { return allowMove; }
			set {
				if (this.Move == value)
					return;
				DocumentCapability oldValue = this.Move;
				this.allowMove = value;
				OnChanged("Move", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool MoveAllowed { get { return IsAllowed(Move); } }
		#endregion
		#region Resize
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetDrawingBehaviorOptionsResize")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Resize {
			get { return allowResize; }
			set {
				if (this.Resize == value)
					return;
				DocumentCapability oldValue = this.Resize;
				this.allowResize = value;
				OnChanged("Resize", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ResizeAllowed { get { return IsAllowed(Resize); } }
		#endregion
		#region ChangeZOrder
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetDrawingBehaviorOptionsChangeZOrder")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability ChangeZOrder {
			get { return allowChangeZOrder; }
			set {
				if (this.ChangeZOrder == value)
					return;
				DocumentCapability oldValue = this.ChangeZOrder;
				this.allowChangeZOrder = value;
				OnChanged("ChangeZOrder", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ChangeZOrderAllowed { get { return IsAllowed(ChangeZOrder); } }
		#endregion
		#region Rotate
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetDrawingBehaviorOptionsRotate")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Rotate {
			get { return allowRotate; }
			set {
				if (this.Rotate == value)
					return;
				DocumentCapability oldValue = this.Rotate;
				this.allowRotate = value;
				OnChanged("Rotate", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RotateAllowed { get { return IsAllowed(Rotate); } }
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.Move = DocumentCapability.Default;
			this.Resize = DocumentCapability.Default;
			this.ChangeZOrder = DocumentCapability.Default;
			this.Rotate = DocumentCapability.Default;
		}
		protected internal void CopyFrom(SpreadsheetDrawingBehaviorOptions value) {
			this.allowMove = value.Move;
			this.allowResize = value.Resize;
			this.allowChangeZOrder = value.ChangeZOrder;
			this.allowRotate = value.Rotate;
		}
	}
	#endregion
	#region SpreadsheetCommentBehaviorOptions
	[ComVisible(true)]
	public class SpreadsheetCommentBehaviorOptions : SpreadsheetNotificationOptions {
		#region Fields
		DocumentCapability insert;
		DocumentCapability edit;
		DocumentCapability delete;
		DocumentCapability showHide;
		DocumentCapability move;
		DocumentCapability resize;
		#endregion
		#region Properties
		#region Insert
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCommentBehaviorOptionsInsert")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Insert {
			get { return insert; }
			set {
				if (insert == value)
					return;
				DocumentCapability oldValue = insert;
				insert = value;
				OnChanged("Insert", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool InsertAllowed { get { return IsAllowed(Insert); } }
		#endregion
		#region Edit
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCommentBehaviorOptionsEdit")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Edit {
			get { return edit; }
			set {
				if (edit == value)
					return;
				DocumentCapability oldValue = edit;
				edit = value;
				OnChanged("Edit", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool EditAllowed { get { return IsAllowed(Edit); } }
		#endregion
		#region Delete
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCommentBehaviorOptionsDelete")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Delete {
			get { return delete; }
			set {
				if (delete == value)
					return;
				DocumentCapability oldValue = delete;
				delete = value;
				OnChanged("Delete", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DeleteAllowed { get { return IsAllowed(Delete); } }
		#endregion
		#region ShowHide
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCommentBehaviorOptionsShowHide")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability ShowHide {
			get { return showHide; }
			set {
				if (showHide == value)
					return;
				DocumentCapability oldValue = showHide;
				showHide = value;
				OnChanged("ShowHide", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowHideAllowed { get { return IsAllowed(ShowHide); } }
		#endregion
		#region Move
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCommentBehaviorOptionsMove")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Move {
			get { return move; }
			set {
				if (move == value)
					return;
				DocumentCapability oldValue = move;
				move = value;
				OnChanged("Move", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool MoveAllowed { get { return IsAllowed(Move); } }
		#endregion
		#region Resize
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCommentBehaviorOptionsResize")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Resize {
			get { return resize; }
			set {
				if (resize == value)
					return;
				DocumentCapability oldValue = resize;
				resize = value;
				OnChanged("Resize", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ResizeAllowed { get { return IsAllowed(Resize); } }
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.Insert = DocumentCapability.Default;
			this.Edit = DocumentCapability.Default;
			this.Delete = DocumentCapability.Default;
			this.ShowHide = DocumentCapability.Default;
			this.Move = DocumentCapability.Default;
			this.Resize = DocumentCapability.Default;
		}
		protected internal void CopyFrom(SpreadsheetCommentBehaviorOptions value) {
			this.insert = value.Insert;
			this.edit = value.Edit;
			this.delete = value.Delete;
			this.showHide = value.ShowHide;
			this.move = value.Move;
			this.resize = value.Resize;
		}
	}
	#endregion
	#region SpreadsheetGroupBehaviorOptions
	[ComVisible(true)]
	public class SpreadsheetGroupBehaviorOptions : SpreadsheetNotificationOptions {
		#region Fields
		DocumentCapability group;
		DocumentCapability ungroup;
		DocumentCapability collapse;
		DocumentCapability expand;
		DocumentCapability changeSettings;
		DocumentCapability collapseExpandOnProtectedSheet;
		#endregion
		#region Properties
		#region Group
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Group {
			get { return group; }
			set {
				if (group == value)
					return;
				DocumentCapability oldValue = group;
				group = value;
				OnChanged("Group", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool GroupAllowed { get { return IsAllowed(Group); } }
		#endregion
		#region Ungroup
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Ungroup {
			get { return ungroup; }
			set {
				if (ungroup == value)
					return;
				DocumentCapability oldValue = ungroup;
				ungroup = value;
				OnChanged("Ungroup", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UngroupAllowed { get { return IsAllowed(Ungroup); } }
		#endregion
		#region Collapse
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Collapse {
			get { return collapse; }
			set {
				if (collapse == value)
					return;
				DocumentCapability oldValue = collapse;
				collapse = value;
				OnChanged("Collapse", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CollapseAllowed { get { return IsAllowed(Collapse); } }
		#endregion
		#region Expand
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Expand {
			get { return expand; }
			set {
				if (expand == value)
					return;
				DocumentCapability oldValue = expand;
				expand = value;
				OnChanged("Expand", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ExpandAllowed { get { return IsAllowed(Expand); } }
		#endregion
		#region ChangeSettings
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability ChangeSettings {
			get { return changeSettings; }
			set {
				if (changeSettings == value)
					return;
				DocumentCapability oldValue = changeSettings;
				changeSettings = value;
				OnChanged("ChangeSettings", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ChangeSettingsAllowed { get { return IsAllowed(ChangeSettings); } }
		#endregion
		#region CollapseExpandOnProtectedSheet
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability CollapseExpandOnProtectedSheet {
			get { return collapseExpandOnProtectedSheet; }
			set {
				if (collapseExpandOnProtectedSheet == value)
					return;
				DocumentCapability oldValue = collapseExpandOnProtectedSheet;
				collapseExpandOnProtectedSheet = value;
				OnChanged("CollapseExpandOnProtectedSheet", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CollapseExpandOnProtectedSheetAllowed { get { return CollapseExpandOnProtectedSheet == DocumentCapability.Enabled; } }
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.Group = DocumentCapability.Default;
			this.Ungroup = DocumentCapability.Default;
			this.Collapse = DocumentCapability.Default;
			this.Expand = DocumentCapability.Default;
			this.ChangeSettings = DocumentCapability.Default;
			this.CollapseExpandOnProtectedSheet = DocumentCapability.Default;
		}
		protected internal void CopyFrom(SpreadsheetGroupBehaviorOptions value) {
			this.group = value.Group;
			this.ungroup = value.Ungroup;
			this.collapse = value.Collapse;
			this.expand = value.Expand;
			this.changeSettings = value.ChangeSettings;
			this.collapseExpandOnProtectedSheet = value.CollapseExpandOnProtectedSheet;
		}
	}
	#endregion
	#region SpreadsheetProtectionBehaviorOptions
	[ComVisible(true)]
	public class SpreadsheetProtectionBehaviorOptions : SpreadsheetNotificationOptions {
		#region Fields
		DocumentCapability protectSheet;
		DocumentCapability protectWorkbook;
		DocumentCapability unprotectSheet;
		DocumentCapability unprotectWorkbook;
		DocumentCapability allowUsersToEditRange;
		#endregion
		#region Properties
		#region ProtectSheet
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetProtectionBehaviorOptionsProtectSheet")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability ProtectSheet {
			get { return protectSheet; }
			set {
				if (protectSheet == value)
					return;
				DocumentCapability oldValue = protectSheet;
				protectSheet = value;
				OnChanged("ProtectSheet", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ProtectSheetAllowed { get { return IsAllowed(ProtectSheet); } }
		#endregion
		#region ProtectWorkbook
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetProtectionBehaviorOptionsProtectWorkbook")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability ProtectWorkbook {
			get { return protectWorkbook; }
			set {
				if (protectWorkbook == value)
					return;
				DocumentCapability oldValue = protectWorkbook;
				protectWorkbook = value;
				OnChanged("ProtectWorkbook", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ProtectWorkbookAllowed { get { return IsAllowed(ProtectWorkbook); } }
		#endregion
		#region UnprotectSheet
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetProtectionBehaviorOptionsUnprotectSheet")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability UnprotectSheet {
			get { return unprotectSheet; }
			set {
				if (unprotectSheet == value)
					return;
				DocumentCapability oldValue = unprotectSheet;
				unprotectSheet = value;
				OnChanged("UnprotectSheet", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UnprotectSheetAllowed { get { return IsAllowed(UnprotectSheet); } }
		#endregion
		#region UnprotectWorkbook
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetProtectionBehaviorOptionsUnprotectWorkbook")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability UnprotectWorkbook {
			get { return unprotectWorkbook; }
			set {
				if (unprotectWorkbook == value)
					return;
				DocumentCapability oldValue = unprotectWorkbook;
				unprotectWorkbook = value;
				OnChanged("UnprotectWorkbook", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UnprotectWorkbookAllowed { get { return IsAllowed(UnprotectWorkbook); } }
		#endregion
		#region AllowUsersToEditRange
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetProtectionBehaviorOptionsAllowUsersToEditRange")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability AllowUsersToEditRange {
			get { return allowUsersToEditRange; }
			set {
				if (allowUsersToEditRange == value)
					return;
				DocumentCapability oldValue = allowUsersToEditRange;
				allowUsersToEditRange = value;
				OnChanged("AllowUsersToEditRange", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowUsersToEditRangeAllowed { get { return IsAllowed(AllowUsersToEditRange); } }
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.ProtectSheet = DocumentCapability.Default;
			this.ProtectWorkbook = DocumentCapability.Default;
			this.UnprotectSheet = DocumentCapability.Default;
			this.UnprotectWorkbook = DocumentCapability.Default;
			this.AllowUsersToEditRange = DocumentCapability.Default;
		}
		protected internal void CopyFrom(SpreadsheetProtectionBehaviorOptions value) {
			this.protectSheet = value.ProtectSheet;
			this.protectWorkbook = value.ProtectWorkbook;
			this.unprotectSheet = value.UnprotectSheet;
			this.unprotectWorkbook = value.UnprotectWorkbook;
			this.allowUsersToEditRange = value.AllowUsersToEditRange;
		}
	}
	#endregion
}
