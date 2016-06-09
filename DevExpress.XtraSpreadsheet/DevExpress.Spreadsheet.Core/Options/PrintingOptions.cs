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
using System.Runtime.InteropServices;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Spreadsheet;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet {
	public enum SpreadsheetPrintRenderMode {
		Gdi,
		GdiPlus,
	}
	#region SpreadsheetPrintOptions
	[ComVisible(true)]
	public class SpreadsheetPrintOptions : SpreadsheetNotificationOptions {
		#region Fields
		const bool defaultRibbonPreview = true;
		const SpreadsheetPrintRenderMode defaultRenderMode = SpreadsheetPrintRenderMode.Gdi;
		const WorksheetVisibilityType defaultPrintSheetVisibility = WorksheetVisibilityType.Visible;
		const bool defaultShowMarginsWarning = true;
		bool ribbonPreview;
		SpreadsheetPrintRenderMode renderMode;
		WorksheetVisibilityType printSheetVisibility;
		bool showMarginsWarning;
		#endregion
		#region Properties
		#region RibbonPreview
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetPrintOptionsRibbonPreview"),
#endif
		NotifyParentProperty(true), DefaultValue(defaultRibbonPreview), XtraSerializableProperty()]
		public bool RibbonPreview
		{
			get { return ribbonPreview; }
			set
			{
				if (RibbonPreview == value)
					return;
				ribbonPreview = value;
				OnChanged("RibbonPreview", !value, value);
			}
		}
		#endregion
		#region RenderMode
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetPrintOptionsRenderMode"),
#endif
		NotifyParentProperty(true), DefaultValue(defaultRenderMode), XtraSerializableProperty()]
		public SpreadsheetPrintRenderMode RenderMode
		{
			get { return renderMode; }
			set
			{
				if (RenderMode == value)
					return;
				SpreadsheetPrintRenderMode oldValue = RenderMode;
				renderMode = value;
				OnChanged("PrintMode", oldValue, value);
			}
		}
		#endregion
		#region PrintSheetVisibility
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetPrintOptionsPrintSheetVisibility"),
#endif
		NotifyParentProperty(true), DefaultValue(defaultPrintSheetVisibility), XtraSerializableProperty()]
		public WorksheetVisibilityType PrintSheetVisibility
		{
			get { return printSheetVisibility; }
			set
			{
				if (PrintSheetVisibility == value)
					return;
				WorksheetVisibilityType oldValue = PrintSheetVisibility;
				printSheetVisibility = value;
				OnChanged("PrintSheetVisibility", oldValue, value);
			}
		}
		#endregion
		#region ShowMarginsWarning
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetPrintOptionsShowMarginsWarning"),
#endif
		NotifyParentProperty(true), DefaultValue(defaultShowMarginsWarning), XtraSerializableProperty()]
		public bool ShowMarginsWarning
		{
			get { return showMarginsWarning; }
			set
			{
				if (ShowMarginsWarning == value)
					return;
				showMarginsWarning = value;
				OnChanged("ShowMarginsWarning", !value, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			RibbonPreview = defaultRibbonPreview;
			RenderMode = defaultRenderMode;
			PrintSheetVisibility = defaultPrintSheetVisibility;
			ShowMarginsWarning = defaultShowMarginsWarning;
		}
	}
	#endregion
}
