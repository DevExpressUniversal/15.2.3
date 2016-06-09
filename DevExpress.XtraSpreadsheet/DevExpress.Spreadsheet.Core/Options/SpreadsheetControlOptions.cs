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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils.Controls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet;
using DevExpress.Compatibility.System.ComponentModel;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	#region CalculationEngine
	public enum CalculationEngineType {
		ChainBased,
		Recursive
	}
	#endregion
	#region DocumentOptions (abstract class)
	[ComVisible(true)]
	public abstract class DocumentOptions : SpreadsheetNotificationOptions {
		readonly InnerSpreadsheetDocumentServer documentServer;
		protected DocumentOptions(InnerSpreadsheetDocumentServer documentServer) {
			this.documentServer = documentServer;
			SubscribeInnerOptionsEvents();
		}
		const CalculationEngineType defaultCalculationEngine = CalculationEngineType.ChainBased;
		#region Properties
		protected internal InnerSpreadsheetDocumentServer DocumentServer { get { return documentServer; } }
		#region Export
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("DocumentOptionsExport"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WorkbookExportOptions Export {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.DocumentExportOptions;
			}
		}
		#endregion
		#region Import
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("DocumentOptionsImport"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WorkbookImportOptions Import {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.DocumentImportOptions;
			}
		}
		#endregion
		#region InnerBehavior
		protected internal SpreadsheetBehaviorOptions InnerBehavior {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.BehaviorOptions;
			}
		}
		#endregion
		#region InnerPivotTableFieldList
		protected internal SpreadsheetPivotTableFieldListOptions InnerPivotTableFieldList {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.PivotTableFieldListOptions;
			}
		}
		#endregion
		#region InnerView
		protected internal SpreadsheetViewOptions InnerView {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.ViewOptions;
			}
		}
		#endregion
		#region InnerPrint
		protected internal SpreadsheetPrintOptions InnerPrint {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.PrintOptions;
			}
		}
		#endregion
		#region Save
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("DocumentOptionsSave"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WorkbookSaveOptions Save {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.DocumentSaveOptions;
			}
		}
		#endregion
		#region DocumentCapabilities
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("DocumentOptionsDocumentCapabilities"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WorkbookCapabilitiesOptions DocumentCapabilities {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.DocumentCapabilities;
			}
		}
		#endregion
		#region Events
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("DocumentOptionsEvents"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WorkbookEventOptions Events {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.EventOptions;
			}
		}
		#endregion
		#region Culture
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("DocumentOptionsCulture"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public CultureInfo Culture {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.Culture;
			}
			set {
				if (DocumentServer == null)
					return;
				DocumentModel documentModel = DocumentServer.DocumentModel;
				if (documentModel.Culture == value)
					return;
				CultureInfo oldValue = documentModel.Culture;
				documentModel.Culture = value;
				OnChanged("Culture", oldValue, value);
			}
		}
		protected internal virtual void ResetCulture() {
			if (Culture != null)
				Culture = CultureInfo.CurrentCulture;
		}
		protected internal virtual bool ShouldSerializeCulture() {
			if (Culture == null)
				return false;
			return !(Culture == CultureInfo.CurrentCulture);
		}
		#endregion
		#region Protection
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("DocumentOptionsProtection"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetProtectionOptions Protection {
			get {
				if (DocumentServer == null)
					return null;
				return DocumentServer.DocumentModel.ProtectionOptions;
			}
		}
		#endregion
		#region Culture
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("DocumentOptionsCalculationEngineType"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public CalculationEngineType CalculationEngineType {
			get {
				if (DocumentServer == null)
					return CalculationEngineType.ChainBased;
				return DocumentServer.DocumentModel.CalculationChain.Enabled ? CalculationEngineType.ChainBased : CalculationEngineType.Recursive;
			}
			set {
				if (DocumentServer == null)
					return;
				DocumentModel documentModel = DocumentServer.DocumentModel;
				bool newValue = value == CalculationEngineType.ChainBased;
				if (documentModel.CalculationChain.Enabled == newValue)
					return;
				CalculationEngineType oldValue = CalculationEngineType;
				documentModel.CalculationChain.Enabled = newValue;
				OnChanged("CalculationEngineType", oldValue, value);
			}
		}
		protected internal virtual void ResetCalculationEngineType() {
			CalculationEngineType = defaultCalculationEngine;
		}
		protected internal virtual bool ShouldSerializeCalculationEngineType() {
			return CalculationEngineType != defaultCalculationEngine;
		}
		#endregion
		#endregion
		protected internal virtual void SubscribeInnerOptionsEvents() {
			if (DocumentCapabilities != null)
				DocumentCapabilities.Changed += OnInnerOptionsChanged;
			if (Events != null)
				Events.Changed += OnInnerOptionsChanged;
			if (InnerView != null)
				InnerView.Changed += OnInnerOptionsChanged;
			if (InnerBehavior != null)
				InnerBehavior.Changed += OnInnerOptionsChanged;
			if (InnerPrint != null)
				InnerPrint.Changed += OnInnerOptionsChanged;
			if (InnerPivotTableFieldList != null)
				InnerPivotTableFieldList.Changed += OnInnerOptionsChanged;
		}
		protected internal virtual void UnsubscribeInnerOptionsEvents() {
			if (DocumentCapabilities != null)
				DocumentCapabilities.Changed -= OnInnerOptionsChanged;
			if (Events != null)
				Events.Changed -= OnInnerOptionsChanged;
			if (InnerView != null)
				InnerView.Changed -= OnInnerOptionsChanged;
			if (InnerBehavior != null)
				InnerBehavior.Changed -= OnInnerOptionsChanged;
			if (InnerPrint != null)
				InnerPrint.Changed -= OnInnerOptionsChanged;
			if (InnerPivotTableFieldList != null)
				InnerPivotTableFieldList.Changed -= OnInnerOptionsChanged;
		}
		protected internal virtual void OnInnerOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged(e);
		}
		protected internal override void ResetCore() {
			if (Save != null)
				Save.Reset();
			if (Import != null)
				Import.Reset();
			if (Export != null)
				Export.Reset();
			if (InnerBehavior != null)
				InnerBehavior.Reset();
			if (InnerPivotTableFieldList != null)
				InnerPivotTableFieldList.Reset();
			if (InnerView != null)
				InnerView.Reset();
			if (InnerPrint != null)
				InnerPrint.Reset();
			if (DocumentCapabilities != null)
				DocumentCapabilities.Reset();
			if (Events != null)
				Events.Reset();
			if (Protection != null)
				Protection.Reset();
			ResetCulture();
			ResetCalculationEngineType();
		}
	}
	#endregion
}
