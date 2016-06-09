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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.XtraSpreadsheet.Localization;
#if !SL
using System.Windows.Forms;
#else
using DevExpress.Data;
#endif
#if SL || WPF
namespace DevExpress.Xpf.Spreadsheet {
#else
namespace DevExpress.XtraSpreadsheet {
#endif
	#region SpreadsheetControl
	public partial class SpreadsheetControl : ISpreadsheetDocumentServer, ISpreadsheetControl, IInnerSpreadsheetControlOwner, INotifyPropertyChanged, ICommandAwareControl<SpreadsheetCommandId> {
		#region Fields
		InnerSpreadsheetControl innerControl;
		WorksheetDisplayArea worksheetDisplayArea;
		#endregion
		#region Properties
		protected internal BackgroundThreadUIUpdater BackgroundThreadUIUpdater { get { return InnerControl != null ? InnerControl.BackgroundThreadUIUpdater : null; } }
		protected internal InnerSpreadsheetDocumentServer InnerDocumentServer { get { return InnerControl; } }
		InnerSpreadsheetDocumentServer ISpreadsheetControl.InnerDocumentServer { get { return this.InnerDocumentServer; } }
		protected internal InnerSpreadsheetControl InnerControl { get { return innerControl; } }
		InnerSpreadsheetControl ISpreadsheetControl.InnerControl { get { return this.InnerControl; } }
		DocumentOptions ISpreadsheetComponent.Options { get { return InnerControl.Options; } }
		CommandBasedKeyboardHandler<SpreadsheetCommandId> ICommandAwareControl<SpreadsheetCommandId>.KeyboardHandler { get { return InnerControl != null ? InnerControl.KeyboardHandler as CommandBasedKeyboardHandler<SpreadsheetCommandId> : null; } }
		#region Document
		[Browsable(false)]
		public DevExpress.Spreadsheet.IWorkbook Document { get { return InnerControl != null ? InnerControl.Document : null; } }
		[Browsable(false)]
		public DevExpress.Spreadsheet.Worksheet ActiveWorksheet { get { return InnerControl != null ? InnerControl.ActiveApiWorksheet : null; } }
		[Browsable(false)]
		public DevExpress.Spreadsheet.Cell ActiveCell { get { return InnerControl != null ? InnerControl.ActiveApiCell : null; } }
		[Browsable(false)]
		public DevExpress.Spreadsheet.Range VisibleRange { get { return InnerControl != null ? InnerControl.VisibleApiRange : null; } }
		[Browsable(false)]
		public DevExpress.Spreadsheet.Range VisibleUnfrozenRange { get { return InnerControl != null ? InnerControl.VisibleUnfrozenApiRange : null; } }
		#endregion
		DocumentUnit ISpreadsheetControl.UIUnit { get { return InnerControl != null ? InnerControl.UIUnit : DocumentUnit.Inch; } set { } }
		#region LayoutUnit
		protected internal virtual bool ShouldSerializeLayoutUnit() {
			return LayoutUnit != DefaultLayoutUnit;
		}
		protected internal virtual void ResetLayoutUnit() {
			LayoutUnit = DefaultLayoutUnit;
		}
		DocumentLayoutUnit DefaultLayoutUnit {
			get {
				return DocumentLayoutUnit.Pixel;
			}
		}
		#endregion
		protected internal DocumentModel DocumentModel { get { return InnerControl != null ? InnerControl.DocumentModel : null; } }
		[Browsable(false)]
		public float DpiX { get { return DocumentModel.DpiX; } }
		[Browsable(false)]
		public float DpiY { get { return DocumentModel.DpiY; } }
		protected internal SpreadsheetControlDeferredChanges ControlDeferredChanges { get { return InnerControl != null ? InnerControl.ControlDeferredChanges : null; } }
		#region WorksheetDisplayArea
		[Browsable(false)]
		public WorksheetDisplayArea WorksheetDisplayArea {
			get {
				if (worksheetDisplayArea == null || worksheetDisplayArea.Workbook != DocumentModel)
					worksheetDisplayArea = new WorksheetDisplayArea(DocumentModel);
				return worksheetDisplayArea;
			}
		}
		#endregion
		#endregion
		protected internal virtual void BeginInitialize() {
			InnerControl.BeginInitialize(false);
		}
		protected internal virtual void EndInitializeCommon() {
			SubscribeInnerEventsPlatformSpecific();
			SubscribeInnerControlEvents();
			InnerControl.EndInitialize();
			AddServices();
		}
		#region IDisposable implementation
		protected internal virtual void DisposeCommon() {
			if (innerControl != null) {
				UnsubscribeInnerEventsPlatformSpecific();
				UnsubscribeInnerControlEvents();
				innerControl.Dispose();
				innerControl = null;
			}
		}
		#endregion
		protected internal virtual InnerSpreadsheetControl CreateInnerControl() {
			return new InnerSpreadsheetControl(this);
		}
		protected internal virtual void SubscribeInnerControlEvents() {
			InnerControl.ContentChanged += OnInnerControlContentChanged;
			InnerControl.ModifiedChanged += OnModifiedChanged;
			InnerControl.ReadOnlyChanged += OnReadOnlyChanged;
			InnerControl.EmptyDocumentCreated += OnEmptyDocumentCreated;
			InnerControl.DocumentLoaded += OnDocumentLoaded;
		}
		protected internal virtual void UnsubscribeInnerControlEvents() {
			InnerControl.ContentChanged -= OnInnerControlContentChanged;
			InnerControl.ModifiedChanged -= OnModifiedChanged;
			InnerControl.ReadOnlyChanged -= OnReadOnlyChanged;
			InnerControl.EmptyDocumentCreated -= OnEmptyDocumentCreated;
			InnerControl.DocumentLoaded -= OnDocumentLoaded;
		}
		protected internal virtual void AddServices() {
			AddServicesPlatformSpecific();
		}
		void OnEnabledChanged() {
			InnerControl.OnIsEnabledChanged();
		}
		void ISpreadsheetControl.UpdateUIFromBackgroundThread(Action method) {
			this.UpdateUIFromBackgroundThread(method);
		}
		protected internal virtual void UpdateUIFromBackgroundThread(Action method) {
			BackgroundThreadUIUpdater.UpdateUI(method);
		}
		protected internal virtual void PerformDeferredUIUpdates(DeferredBackgroundThreadUIUpdater deferredUpdater) {
			List<Action> deferredUpdates = deferredUpdater.Updates;
			int count = deferredUpdates.Count;
			for (int i = 0; i < count; i++)
				BackgroundThreadUIUpdater.UpdateUI(deferredUpdates[i]);
		}
		void OnInnerControlContentChanged(object sender, EventArgs e) {
			DocumentContentChangedEventArgs args = e as DocumentContentChangedEventArgs;
			bool suppressBindingNotifications = (args == null ? false : args.SuppressBindingNotifications);
			OnInnerControlContentChangedPlatformSpecific(suppressBindingNotifications);
			RaiseContentChanged();
		}
		protected internal virtual void OnModifiedChanged(object sender, EventArgs e) {
			Modified = InnerControl.Modified;
			RaisePropertyChanged("Modified");
			RaiseModifiedChanged();
		}
		protected internal virtual void OnReadOnlyChanged(object sender, EventArgs e) {
			ReadOnly = InnerControl.IsReadOnly;
			RaisePropertyChanged("ReadOnly");
			RaiseReadOnlyChanged();
			OnReadOnlyChangedPlatformSpecific();
		}
		protected internal virtual void OnEmptyDocumentCreated(object sender, EventArgs e) {
			OnEmptyDocumentCreatedPlatformSpecific();
			RaiseEmptyDocumentCreated();
		}
		protected internal virtual void OnDocumentLoaded(object sender, EventArgs e) {
			OnDocumentLoadedPlatformSpecific();
			ShowCircularRereferenceWarning();
			RaiseDocumentLoaded();
		}
		void ShowCircularRereferenceWarning() {
			if (DocumentModel.HasCircularReferences)
				(this as ISpreadsheetControl).ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_CircularReference));
		}
		void IInnerSpreadsheetControlOwner.ApplyChangesCorePlatformSpecific(DocumentModelChangeActions changeActions) {
			ApplyChangesCorePlatformSpecific(changeActions);
		}
		public void CreateNewDocument() {
			if (InnerControl != null)
				InnerControl.CreateNewDocument();
		}
		public virtual void LoadDocument() {
			if (InnerControl != null)
				InnerControl.LoadDocument();
		}
		public virtual void LoadDocument(IWin32Window parent) {
			if (InnerControl != null)
				InnerControl.LoadDocument(parent);
		}
		public bool LoadDocument(string fileName) {
			if (InnerControl != null)
				return InnerControl.LoadDocument(fileName);
			return false;
		}
		public bool LoadDocument(string fileName, DevExpress.Spreadsheet.DocumentFormat format) {
			if (InnerControl != null)
				return InnerControl.LoadDocument(fileName, format);
			return false;
		}
		public bool LoadDocument(Stream stream, DevExpress.Spreadsheet.DocumentFormat format) {
			if (InnerControl != null)
				return InnerControl.LoadDocument(stream, format);
			return false;
		}
		public bool LoadDocument(byte[] buffer, DevExpress.Spreadsheet.DocumentFormat format) {
			if (InnerControl != null)
				return InnerControl.LoadDocument(buffer, format);
			return false;
		}
		public virtual void SaveDocumentAs() {
			if (InnerControl != null)
				InnerControl.SaveDocumentAs();
		}
		public virtual void SaveDocumentAs(IWin32Window parent) {
			if (InnerControl != null)
				InnerControl.SaveDocumentAs(parent);
		}
		public virtual bool SaveDocument() {
			if (InnerControl != null)
				return InnerControl.SaveDocument();
			else
				return false;
		}
		public virtual bool SaveDocument(IWin32Window parent) {
			if (InnerControl != null)
				return InnerControl.SaveDocument(parent);
			else
				return false;
		}
		public void SaveDocument(string fileName) {
			if (InnerControl != null)
				InnerControl.SaveDocument(fileName);
		}
		public void SaveDocument(string fileName, DevExpress.Spreadsheet.DocumentFormat format) {
			if (InnerControl != null)
				InnerControl.SaveDocument(fileName, format);
		}
		public void SaveDocument(Stream stream, DevExpress.Spreadsheet.DocumentFormat format) {
			if (InnerControl != null)
				InnerControl.SaveDocument(stream, format);
		}
		public byte[] SaveDocument(DevExpress.Spreadsheet.DocumentFormat format) {
			if (InnerControl != null)
				return InnerControl.SaveDocument(format);
			else
				return null;
		}
		public void ExportToHtml(Stream stream, int sheetIndex) {
			if (InnerControl != null)
				InnerControl.ExportToHtml(stream, sheetIndex);
		}
		public void ExportToHtml(Stream stream, DevExpress.Spreadsheet.Worksheet sheet) {
			if (InnerControl != null)
				InnerControl.ExportToHtml(stream, sheet);
		}
		public void ExportToHtml(Stream stream, DevExpress.Spreadsheet.Range range) {
			if (InnerControl != null)
				InnerControl.ExportToHtml(stream, range);
		}
		public void ExportToHtml(Stream stream, HtmlDocumentExporterOptions options) {
			if (InnerControl != null)
				InnerControl.ExportToHtml(stream, options);
		}
		public void ExportToHtml(string fileName, int sheetIndex) {
			if (InnerControl != null)
				InnerControl.ExportToHtml(fileName, sheetIndex);
		}
		public void ExportToHtml(string fileName, DevExpress.Spreadsheet.Worksheet sheet) {
			if (InnerControl != null)
				InnerControl.ExportToHtml(fileName, sheet);
		}
		public void ExportToHtml(string fileName, DevExpress.Spreadsheet.Range range) {
			if (InnerControl != null)
				InnerControl.ExportToHtml(fileName, range);
		}
		public void ExportToHtml(string fileName, HtmlDocumentExporterOptions options) {
			if (InnerControl != null)
				InnerControl.ExportToHtml(fileName, options);
		}
		public DevExpress.Spreadsheet.Cell GetCellFromPoint(PointF clientPoint) {
			if (InnerControl == null)
				return null;
			return InnerControl.GetCellFromPoint(GetPhysicalPoint(Point.Round(clientPoint)));
		}
		public void AssignShortcutKeyToCommand(Keys key, Keys modifier, SpreadsheetCommandId commandId) {
			if (InnerControl != null)
				InnerControl.AssignShortcutKeyToCommand(key, modifier, commandId);
		}
		public void RemoveShortcutKey(Keys key, Keys modifier) {
			if (InnerControl != null)
				InnerControl.RemoveShortcutKey(key, modifier);
		}
		public void ResetLayout() {
			if (InnerControl != null)
				InnerControl.ResetDocumentLayout();
		}
		public IList<DevExpress.Spreadsheet.Range> GetSelectedRanges() {
			if (InnerControl != null)
				return InnerControl.GetSelectedApiRanges();
			else
				return new List<DevExpress.Spreadsheet.Range>();
		}
		public bool SetSelectedRanges(IList<DevExpress.Spreadsheet.Range> ranges) {
			return SetSelectedRanges(ranges, true);
		}
		public bool SetSelectedRanges(IList<DevExpress.Spreadsheet.Range> ranges, bool expandToMergedCellsSize) {
			if (InnerControl != null)
				return InnerControl.SetSelectedApiRanges(ranges, expandToMergedCellsSize);
			else
				return false;
		}
		public IList<DevExpress.Spreadsheet.Shape> GetSelectedShapes() {
			if (InnerControl != null)
				return InnerControl.GetSelectedApiShapes();
			else
				return new List<DevExpress.Spreadsheet.Shape>();
		}
		public bool SetSelectedShapes(IList<DevExpress.Spreadsheet.Shape> Shapes) {
			if (InnerControl != null)
				return InnerControl.SetSelectedApiShapes(Shapes);
			else
				return false;
		}
		public void CircleInvalidData() {
			CircleInvalidDataCommand command = new CircleInvalidDataCommand(this);
			command.Execute();
		}
		public void ClearValidationCircles() {
			ClearValidationCirclesCommand command = new ClearValidationCirclesCommand(this);
			command.Execute();
		}
	}
	#endregion
}
