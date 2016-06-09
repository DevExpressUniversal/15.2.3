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
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class NewReportWizardController : ViewController {
		private NewObjectViewController newObjectController;
		private void newObjectController_ObjectCreating(object sender, ObjectCreatingEventArgs e) {
			if(!typeof(IReportDataV2).IsAssignableFrom(e.ObjectType)) {
				return;
			}
			e.Cancel = true;
			ShowNewReportWizard(e.ObjectType);
			View.ObjectSpace.Refresh();
		}
		protected virtual void ShowNewReportWizard(Type reportDataType) {
			Frame.GetController<ReportServiceController>().ShowWizard(reportDataType);
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(ReportsModuleV2.ActivateReportController(this)) {
				newObjectController = Frame.GetController<NewObjectViewController>();
				newObjectController.ObjectCreating += new EventHandler<ObjectCreatingEventArgs>(newObjectController_ObjectCreating);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(newObjectController != null) {
				newObjectController.ObjectCreating -= new EventHandler<ObjectCreatingEventArgs>(newObjectController_ObjectCreating);
				newObjectController = null;
			}
		}
	}
}
