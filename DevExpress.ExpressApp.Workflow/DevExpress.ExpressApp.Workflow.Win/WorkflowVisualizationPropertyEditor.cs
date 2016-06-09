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
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
using System.Activities.Presentation.Hosting;
namespace DevExpress.ExpressApp.Workflow.Win {
	[PropertyEditor(typeof(ITrackingRecordVisualizationInfo), true)]
	public class WorkflowVisualizationPropertyEditor : WinPropertyEditor, IComplexViewItem {
		private XafApplication application = null;
		public WorkflowVisualizationPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
		protected override void ReadValueCore() {
			base.ReadValueCore();
			if(Control != null && PropertyValue != null) {
				VisualizationControl.Xaml = ((ITrackingRecordVisualizationInfo)PropertyValue).Xaml;
				VisualizationControl.SelectionId = ((ITrackingRecordVisualizationInfo)PropertyValue).SelectionId;
				ReadOnlyState state = VisualizationControl.Designer.Context.Items.GetValue<ReadOnlyState>();
				state.IsReadOnly = true;
			}
		}
		protected override object CreateControlCore() {
			WorkflowVisualizationControl control = new WorkflowVisualizationControl();
			return control;
		}
		public WorkflowVisualizationControl VisualizationControl {
			get { return (WorkflowVisualizationControl)Control; }
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.application = application;
		}
	}
}
