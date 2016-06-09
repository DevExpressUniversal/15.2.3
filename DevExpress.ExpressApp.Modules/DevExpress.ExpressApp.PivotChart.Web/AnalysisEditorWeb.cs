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
using System.ComponentModel;
using System.Web.UI;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.XtraPrinting;
namespace DevExpress.ExpressApp.PivotChart.Web {
	public class AnalysisEditorWeb : AnalysisEditorBase, IComplexViewItem, IExportableAnalysisEditor, ITestableEx, ITestableContainer {
		private IPrintable printable;
		private bool isChartVisible = false;
		private IObjectSpace objectSpace;
		private void UpdateChartVisible() {
			if(Control != null) {
				isChartVisible = Control.IsChartVisible;
			}
		}
		private IPrintable GetPrintable() {
			IPrintable printable = null;
			if(Control != null && IsDataSourceReady) {
				printable = !isChartVisible ? (IPrintable)Control.PivotGridExporter : (IPrintable)Control.Chart;
			}
			return printable;
		}
		private void analysisControl_IsChartVisibleChanged(object sender, HandledEventArgs e) {
			UpdateChartVisible();
			Printable = GetPrintable();
			if(Control.IsChartVisible && Printable != null) {
				((AnalysisControlWeb)Control).UpdateChartDataSource();
			}
		}
		protected override void OnIsDataSourceReadyChanged() {
			base.OnIsDataSourceReadyChanged();
			UpdateChartVisible();
			Printable = GetPrintable();
		}
		private void OnPrintableChanged() {
			if(PrintableChanged != null) {
				PrintableChanged(this, new PrintableChangedEventArgs(printable));
			}
		}
		private void analysisControl_Load(object sender, EventArgs e) {
			ReadValue();
			Control.DataBind();
			Printable = GetPrintable();
		}
		protected override void UpdatePivotGridSettings() {
			base.UpdatePivotGridSettings();
			Control.ForceReadOnly();
		}
		protected override IAnalysisControl CreateAnalysisControl() {
			AnalysisControlWeb analysisControl = new AnalysisControlWeb();
			analysisControl.SetObjectSpace(objectSpace);
			analysisControl.Load += new EventHandler(analysisControl_Load);
			analysisControl.Unload += new EventHandler(Control_Unload);
			analysisControl.IsChartVisibleChanged += new EventHandler<HandledEventArgs>(analysisControl_IsChartVisibleChanged);
			return analysisControl;
		}
		private void Control_Unload(object sender, EventArgs e) {
			OnControlInitialized(sender as Control);
		}
		private void AnalysisEditorWeb_ControlCreated(object sender, EventArgs e) {
			OnTestableControlsCreated();
		}
		protected override void Dispose(bool disposing) {
			AnalysisControlWeb control = Control as AnalysisControlWeb;
			if(control != null) {
				control.Load -= new EventHandler(analysisControl_Load);
				control.Unload -= new EventHandler(Control_Unload);
			}
			base.Dispose(disposing);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		protected override IPivotGridSettingsStore CreatePivotGridSettingsStore() {
			return new ASPxPivotGridSettingsStore(Control.PivotGrid);
		}
		protected void OnTestableControlsCreated() {
			if(TestableControlsCreated != null) {
				TestableControlsCreated(this, EventArgs.Empty);
			}
		}
		public new AnalysisControlWeb Control {
			get { return (AnalysisControlWeb)base.Control; }
		}
		public AnalysisEditorWeb(Type objectType, IModelMemberViewItem info)
			: base(objectType, info) {
			this.ControlCreated += new EventHandler<EventArgs>(AnalysisEditorWeb_ControlCreated);
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.objectSpace = objectSpace;
		}
		#region IExportable Members
		public IList<DevExpress.XtraPrinting.PrintingSystemCommand> ExportTypes {
			get {
				IList<PrintingSystemCommand> exportTypes = new List<PrintingSystemCommand>();
				exportTypes.Add(PrintingSystemCommand.ExportMht);
				exportTypes.Add(PrintingSystemCommand.ExportXls);
				exportTypes.Add(PrintingSystemCommand.ExportPdf);
				if(!isChartVisible) {
					exportTypes.Add(PrintingSystemCommand.ExportRtf);
					exportTypes.Add(PrintingSystemCommand.ExportTxt);
					exportTypes.Add(PrintingSystemCommand.ExportHtm);
				}				 
				exportTypes.Add(PrintingSystemCommand.ExportGraphic);
				return exportTypes;
			}
		}
		public IPrintable Printable {
			get { return printable; }
			set {
				if(printable != value) {
					printable = value;
					OnPrintableChanged();
				}
			}
		}
		public event EventHandler<PrintableChangedEventArgs> PrintableChanged;
		#endregion
		#region ITestable Members
		public string TestCaption {
			get { return Model.Caption; }
		}
		public string ClientId {
			get { return Control.PageControl.ClientID; }
		}
		public IJScriptTestControl TestControl {
			get { return null; }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public virtual TestControlType TestControlType {
			get {
				return TestControlType.Field;
			}
		}
		#endregion
		#region ITestableContainer Members
		public ITestable[] GetTestableControls() {
			return new ITestable[] { Control };
		}
		public event EventHandler TestableControlsCreated;
		#endregion
		#region ITestableEx Members
		public Type RegisterControlType {
			get { return Control.PageControl.GetType(); }
		}
		#endregion
	}
}
