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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet {
	#region SpreadsheetViewOptions
	[ComVisible(true)]
	public class SpreadsheetViewOptions : SpreadsheetNotificationOptions {
		#region Fields
		bool showRowHeaders;
		bool showColumnHeaders;
		bool showPrintArea;
		int columnHeaderHeight;
		int rowHeaderWidth;
		SpreadsheetChartsViewOptions charts = new SpreadsheetChartsViewOptions();
		SpreadsheetPicturesViewOptions pictures = new SpreadsheetPicturesViewOptions();
		#endregion
		public SpreadsheetViewOptions()
			: base() {
			this.charts.Changed += OnChartsChanged;
			this.pictures.Changed += OnPicturesChanged;
		}
		#region Properties
		#region ShowRowHeaders
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetViewOptionsShowRowHeaders"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowRowHeaders {
			get { return showRowHeaders; }
			set {
				if (showRowHeaders == value)
					return;
				this.showRowHeaders = value;
				OnChanged("ShowRowHeaders", !value, value);
			}
		}
		#endregion
		#region ShowColumnHeaders
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetViewOptionsShowColumnHeaders"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowColumnHeaders {
			get { return showColumnHeaders; }
			set {
				if (showColumnHeaders == value)
					return;
				this.showColumnHeaders = value;
				OnChanged("ShowColumnHeaders", !value, value);
			}
		}
		#endregion
		#region ShowPrintArea
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetViewOptionsShowPrintArea"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowPrintArea {
			get { return showPrintArea; }
			set {
				if (showPrintArea == value)
					return;
				this.showPrintArea = value;
				OnChanged("ShowPrintArea", !value, value);
			}
		}
		#endregion
		#region ColumnHeaderHeight
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetViewOptionsColumnHeaderHeight"),
#endif
 DefaultValue(0), NotifyParentProperty(true)]
		public int ColumnHeaderHeight {
			get { return columnHeaderHeight; }
			set {
				if (ColumnHeaderHeight == value)
					return;
				int oldValue = this.ColumnHeaderHeight;
				this.columnHeaderHeight = value;
				OnChanged("ColumnHeaderHeight", oldValue, value);
			}
		}
		#endregion
		#region RowHeaderWidth
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetViewOptionsRowHeaderWidth"),
#endif
 DefaultValue(0), NotifyParentProperty(true)]
		public int RowHeaderWidth {
			get { return rowHeaderWidth; }
			set {
				if (RowHeaderWidth == value)
					return;
				int oldValue = this.RowHeaderWidth;
				this.rowHeaderWidth = value;
				OnChanged("RowHeaderWidth", oldValue, value);
			}
		}
		#endregion
		#region Charts
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetViewOptionsCharts"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetChartsViewOptions Charts { get { return charts; } }
		#endregion
		#region Pictures
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetViewOptionsPictures"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetPicturesViewOptions Pictures { get { return pictures; } }
		#endregion
		#endregion
		void OnChartsChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged("Charts." + e.Name, e.OldValue, e.NewValue);
		}
		void OnPicturesChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged("Pictures." + e.Name, e.OldValue, e.NewValue);
		}
		protected internal override void ResetCore() {
			this.ShowRowHeaders = true;
			this.ShowColumnHeaders = true;
			this.ShowPrintArea = true;
			this.ColumnHeaderHeight = 0;
			this.RowHeaderWidth = 0;
			this.charts.Reset();
			this.pictures.Reset();
		}
		protected internal void CopyFrom(SpreadsheetViewOptions value) {
			this.showRowHeaders = value.ShowRowHeaders;
			this.showColumnHeaders = value.ShowColumnHeaders;
			this.showPrintArea = value.ShowPrintArea;
			this.columnHeaderHeight = value.ColumnHeaderHeight;
			this.rowHeaderWidth = value.RowHeaderWidth;
			this.charts.CopyFrom(value.Charts);
			this.pictures.CopyFrom(value.Pictures);
		}
	}
	#endregion
	#region SpreadsheetChartsViewOptions
	[ComVisible(true)]
	public class SpreadsheetChartsViewOptions : SpreadsheetNotificationOptions {
		#region Fields
		DocumentCapability antialiasing;
		DocumentCapability textAntialiasing;
		#endregion
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetChartsViewOptionsAntialiasing")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Antialiasing {
			get { return antialiasing; }
			set {
				if (this.Antialiasing == value)
					return;
				DocumentCapability oldValue = this.Antialiasing;
				this.antialiasing = value;
				OnChanged("Antialiasing", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AntialiasingAllowed { get { return Antialiasing == DocumentCapability.Enabled; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetChartsViewOptionsTextAntialiasing")]
#endif
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability TextAntialiasing {
			get { return textAntialiasing; }
			set {
				if (this.TextAntialiasing == value)
					return;
				DocumentCapability oldValue = this.TextAntialiasing;
				this.textAntialiasing = value;
				OnChanged("TextAntialiasing", oldValue, value);
			}
		}
		#endregion
		protected internal override void ResetCore() {
			this.Antialiasing = DocumentCapability.Default;
			this.TextAntialiasing = DocumentCapability.Default;
		}
		protected internal void CopyFrom(SpreadsheetChartsViewOptions value) {
			this.antialiasing = value.Antialiasing;
			this.textAntialiasing = value.TextAntialiasing;
		}
	}
	#endregion
	#region SpreadsheetPicturesViewOptions
	[ComVisible(true)]
	public class SpreadsheetPicturesViewOptions : SpreadsheetNotificationOptions {
		#region Fields
		DocumentCapability highQualityScaling;
		#endregion
		#region Properties
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability HighQualityScaling {
			get { return highQualityScaling; }
			set {
				if (this.HighQualityScaling == value)
					return;
				DocumentCapability oldValue = this.HighQualityScaling;
				this.highQualityScaling = value;
				OnChanged("HighQualityScaling", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HighQualityScalingAllowed { get { return HighQualityScaling == DocumentCapability.Enabled || HighQualityScaling == DocumentCapability.Default; } }
		#endregion
		protected internal override void ResetCore() {
			this.HighQualityScaling = DocumentCapability.Default;
		}
		protected internal void CopyFrom(SpreadsheetPicturesViewOptions value) {
			this.highQualityScaling = value.HighQualityScaling;
		}
	}
	#endregion
}
