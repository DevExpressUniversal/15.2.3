#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.XtraPrinting;
namespace DevExpress.ExpressApp.PivotChart.Win {
	public class AnalysisEditorWin : AnalysisEditorBase, IComplexViewItem, IExportableAnalysisEditor, IAnalysisEditorWin {
		private IPrintable printable;
		private IObjectSpace objectSpace;
		private IPrintable GetPrintable() {
			IPrintable printable = null;
			if(IsDataSourceReady && Control != null) {
				printable = !Control.IsChartVisible ? (IPrintable)Control.PivotGrid : (IPrintable)Control.ChartControl;
			}
			return printable;
		}
		private void OnPrintableChanged() {
			if(PrintableChanged != null) {
				PrintableChanged(this, new PrintableChangedEventArgs(printable));
			}
		}
		private void TabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e) {
			Printable = GetPrintable();
		}
		private void AnalysisEditorWin_IsDataSourceReadyChanged(object sender, EventArgs e) {
			Printable = GetPrintable();
		}
		private void analysisControl_HandleCreated(object sender, EventArgs e) {
			ReadValue();
		}
		protected override void UpdatePivotGridSettings() {
			base.UpdatePivotGridSettings();
			Control.PivotGrid.OptionsChartDataSource.SelectionOnly = false;
		}
		protected override void OnAnalysisInfoChanged() {
			base.OnAnalysisInfoChanged();
			objectSpace.SetModified(CurrentObject);
		}
		protected override IPivotGridSettingsStore CreatePivotGridSettingsStore() {
			return new PivotGridControlSettingsStore(Control.PivotGrid);
		}
		protected override IAnalysisControl CreateAnalysisControl() {
			AnalysisControlWin analysisControl = new AnalysisControlWin();
			analysisControl.SetObjectSpace(objectSpace);
			analysisControl.HandleCreated += new EventHandler(analysisControl_HandleCreated);
			analysisControl.TabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(TabControl_SelectedPageChanged);
			return analysisControl;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				IsDataSourceReadyChanged -= new EventHandler<EventArgs>(AnalysisEditorWin_IsDataSourceReadyChanged);
				if(Control != null) {
					Control.TabControl.SelectedPageChanged -= new DevExpress.XtraTab.TabPageChangedEventHandler(TabControl_SelectedPageChanged);
				}
			}
			base.Dispose(disposing);
		}
		public new AnalysisControlWin Control {
			get { return (AnalysisControlWin)base.Control; }
		}
		public AnalysisEditorWin(Type objectType, IModelMemberViewItem model) : base(objectType, model) {
			IsDataSourceReadyChanged += new EventHandler<EventArgs>(AnalysisEditorWin_IsDataSourceReadyChanged);
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.objectSpace = objectSpace;
		}
		#region IExportableEditor Members
		public IList<DevExpress.XtraPrinting.PrintingSystemCommand> ExportTypes {
			get {
				IList<PrintingSystemCommand> exportTypes = new List<PrintingSystemCommand>();
				exportTypes.Add(PrintingSystemCommand.ExportMht);
				exportTypes.Add(PrintingSystemCommand.ExportXls);
				exportTypes.Add(PrintingSystemCommand.ExportXlsx);
				exportTypes.Add(PrintingSystemCommand.ExportPdf);
				exportTypes.Add(PrintingSystemCommand.ExportHtm);
				exportTypes.Add(PrintingSystemCommand.ExportGraphic);
				if(!Control.IsChartVisible) {
					exportTypes.Add(PrintingSystemCommand.ExportRtf);
					exportTypes.Add(PrintingSystemCommand.ExportTxt);
				}
				return exportTypes;
			}
		}
		public IPrintable Printable {
			get {
				return printable;
			}
			set {
				if(printable != value) {
					printable = value;
					OnPrintableChanged();
				}
			}
		}
		public event EventHandler<PrintableChangedEventArgs> PrintableChanged;
		#endregion
	}
}
