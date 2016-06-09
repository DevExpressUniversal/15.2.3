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
using DevExpress.Utils.Controls;
using DevExpress.Office;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet {
	#region WorkbookCapabilitiesOptions
	[ComVisible(true)]
	public class WorkbookCapabilitiesOptions : SpreadsheetNotificationOptions, ISupportsCopyFrom<WorkbookCapabilitiesOptions> {
		#region Fields
		DocumentCapability undo;
		DocumentCapability pictures;
		DocumentCapability charts;
		DocumentCapability sparklines;
		DocumentCapability shapes;
		#endregion
		public WorkbookCapabilitiesOptions() {
		}
		protected internal virtual void OnInnerOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged(e);
		}
		#region Properties
		#region Undo
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookCapabilitiesOptionsUndo"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Undo
		{
			get { return undo; }
			set
			{
				if (this.undo == value)
					return;
				DocumentCapability oldValue = this.undo;
				this.undo = value;
				OnChanged("Undo", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UndoAllowed { get { return IsAllowed(Undo); } }
		#endregion
		#region Pictures
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookCapabilitiesOptionsPictures"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Pictures
		{
			get { return pictures; }
			set
			{
				if (this.Pictures == value)
					return;
				DocumentCapability oldValue = this.Pictures;
				this.pictures = value;
				OnChanged("Pictures", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool PicturesAllowed { get { return IsAllowed(Pictures); } }
		#endregion
		#region Charts
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookCapabilitiesOptionsCharts"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Charts
		{
			get { return charts; }
			set
			{
				if (this.Charts == value)
					return;
				DocumentCapability oldValue = this.Charts;
				this.charts = value;
				OnChanged("Charts", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ChartsAllowed { get { return IsAllowed(Charts); } }
		#endregion
		#region Sparklines
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookCapabilitiesOptionsSparklines"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Sparklines {
			get { return sparklines; }
			set {
				if (this.Sparklines == value)
					return;
				DocumentCapability oldValue = this.Sparklines;
				this.sparklines = value;
				OnChanged("Sparklines", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SparklinesAllowed { get { return IsAllowed(Sparklines); } }
		#endregion
		#region Shapes
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookCapabilitiesOptionsShapes"),
#endif
		DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability Shapes {
			get { return shapes; }
			set {
				if(this.Shapes == value)
					return;
				DocumentCapability oldValue = this.Shapes;
				this.shapes = value;
				OnChanged("Shapes", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShapesAllowed { get { return IsAllowed(Shapes); } }
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.Undo = DocumentCapability.Default;
			this.Pictures = DocumentCapability.Default;
			this.Charts = DocumentCapability.Default;
			this.Sparklines = DocumentCapability.Default;
			this.Shapes = DocumentCapability.Default;
		}
		public void CopyFrom(WorkbookCapabilitiesOptions source) {
			this.undo = source.Undo;
			this.pictures = source.Pictures;
			this.charts = source.Charts;
			this.sparklines = source.Sparklines;
			this.shapes = source.Shapes;
		}
	}
	#endregion
}
