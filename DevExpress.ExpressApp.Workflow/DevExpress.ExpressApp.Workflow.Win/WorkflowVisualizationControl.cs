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
using System.Linq;
using System.Text;
using System.Windows.Forms.Integration;
using System.Activities.Presentation;
using System.Activities.Debugger;
using System.Activities.Presentation.Services;
using System.Activities;
using DevExpress.Utils;
using System.Activities.XamlIntegration;
using System.IO;
using System.Activities.Presentation.Debug;
namespace DevExpress.ExpressApp.Workflow.Win {
	[System.ComponentModel.ToolboxItem(false)]
	public class WorkflowVisualizationControl : WorkflowDesignerControlBase {
		private WorkflowVisualizer control;
		private IWorkflowVizualizationHelper vizualizationHelper = null;
		private string selectionId;
		private void UpdateSelectionHighlight() {
			if(vizualizationHelper != null) {
				vizualizationHelper.UpdateCurrentLocation(selectionId);
			}
		}
		protected virtual IWorkflowVizualizationHelper CreateVizualizationHelper() {
			return new WorkflowDebuggerHelper(Designer);
		}
		protected override void LoadActivity(string xaml) {
			base.LoadActivity(xaml);
			control.DesignerBorder.Child = Designer.View;
			if(vizualizationHelper == null) {
				vizualizationHelper = CreateVizualizationHelper();
				Guard.ArgumentNotNull(vizualizationHelper, "vizualizationHelper");
			}
			else {
				vizualizationHelper.Initialize(Designer);
			}
		}
		protected override void AfterLoadActivity() {
			if(HighlightLocationEnabled) {
				vizualizationHelper.UpdateSourceLocationMapping();
				UpdateSelectionHighlight();
			}
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(vizualizationHelper != null) {
					vizualizationHelper.Dispose();
					vizualizationHelper = null;
				}
				if(control != null) {
					control.DesignerBorder.Child = null;
					control = null;
				}
			}
		}
		public WorkflowVisualizationControl() {
			control = new WorkflowVisualizer();
			Child = control;
			HighlightLocationEnabled = true;
		}
		public bool HighlightLocationEnabled { get; set; }
		public string SelectionId {
			get { return selectionId; }
			set {
				if(selectionId != value) {
					selectionId = value;
					if(HighlightLocationEnabled) {
						UpdateSelectionHighlight();
					}
				}
			}
		}
	}
}
