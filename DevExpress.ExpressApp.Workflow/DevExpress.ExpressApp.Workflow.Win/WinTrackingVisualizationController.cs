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
using DevExpress.ExpressApp.Workflow.Visualization;
using DevExpress.ExpressApp.Editors;
using System.Windows.Forms;
namespace DevExpress.ExpressApp.Workflow.Win {
	public class WinTrackingVisualizationController : TrackingVisualizationController {
		protected override void UpdateWorkflowTrackingRecordVisualization(string selectedActivityId) {
			base.UpdateWorkflowTrackingRecordVisualization(selectedActivityId);
			ListPropertyEditor trackingRecords = View.FindItem("TrackingRecords") as ListPropertyEditor;
			if(trackingRecords != null && trackingRecords.ListView != null && trackingRecords.ListView.IsControlCreated) {
				((Control)trackingRecords.ListView.Editor.Control).LostFocus +=new EventHandler(WinTrackingVisualizationController_LostFocus);
			}
		}
		private void WinTrackingVisualizationController_LostFocus(object sender, EventArgs e) {
			Control control = sender as Control;
			control.LostFocus -=new EventHandler(WinTrackingVisualizationController_LostFocus);
			control.Focus();
		}
	}
}
