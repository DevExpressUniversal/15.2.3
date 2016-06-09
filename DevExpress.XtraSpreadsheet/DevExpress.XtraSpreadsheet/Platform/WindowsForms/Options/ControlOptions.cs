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
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet {
	#region SpreadsheetControlOptions
	public class SpreadsheetControlOptions : DocumentOptions {
		#region Fields
		SpreadsheetVerticalScrollbarOptions verticalScrollbar = new SpreadsheetVerticalScrollbarOptions();
		SpreadsheetHorizontalScrollbarOptions horizontalScrollbar = new SpreadsheetHorizontalScrollbarOptions();
		SpreadsheetTabSelectorOptions tabSelector = new SpreadsheetTabSelectorOptions();
		SpreadsheetDataSourceWizardOptions dataSourceWizard = new SpreadsheetDataSourceWizardOptions();
		#endregion
		public SpreadsheetControlOptions(InnerSpreadsheetDocumentServer documentServer)
			: base(documentServer) {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlOptionsVerticalScrollbar"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetVerticalScrollbarOptions VerticalScrollbar { get { return verticalScrollbar; } }
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlOptionsHorizontalScrollbar"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetHorizontalScrollbarOptions HorizontalScrollbar { get { return horizontalScrollbar; } }
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlOptionsTabSelector"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetTabSelectorOptions TabSelector { get { return tabSelector; } }
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlOptionsBehavior"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetBehaviorOptions Behavior { get { return base.InnerBehavior; } }
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlOptionsView"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetViewOptions View { get { return base.InnerView; } }
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlOptionsPrint"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetPrintOptions Print { get { return base.InnerPrint; } }
		[
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetPivotTableFieldListOptions PivotTableFieldList { get { return base.InnerPivotTableFieldList; } }
		[
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetDataSourceWizardOptions DataSourceWizard { get { return dataSourceWizard; } }
		#endregion
		protected override void SubscribeInnerOptionsEvents() {
			base.SubscribeInnerOptionsEvents();
			VerticalScrollbar.Changed += OnInnerOptionsChanged;
			HorizontalScrollbar.Changed += OnInnerOptionsChanged;
			TabSelector.Changed += OnInnerOptionsChanged;
			DataSourceWizard.Changed += OnInnerOptionsChanged;
		}
		protected override void UnsubscribeInnerOptionsEvents() {
			base.UnsubscribeInnerOptionsEvents();
			VerticalScrollbar.Changed -= OnInnerOptionsChanged;
			HorizontalScrollbar.Changed -= OnInnerOptionsChanged;
			TabSelector.Changed -= OnInnerOptionsChanged;
			DataSourceWizard.Changed -= OnInnerOptionsChanged;
		}
		protected override void ResetCore() {
			base.ResetCore();
			VerticalScrollbar.Reset();
			HorizontalScrollbar.Reset();
			TabSelector.Reset();
			DataSourceWizard.Reset();
		}
	}
	#endregion
}
