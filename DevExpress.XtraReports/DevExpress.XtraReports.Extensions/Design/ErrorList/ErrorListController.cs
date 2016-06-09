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
using System.CodeDom.Compiler;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.Tools;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Design.ErrorList {
	class ErrorListController : TreeListController {
		ErrorListUserControl control;
		public ErrorListController(IServiceProvider serviceProvider)
			: base(serviceProvider) {
		}
		public override void CaptureTreeList(System.Windows.Forms.Control control) {
			this.control = control as ErrorListUserControl;
			if(this.control != null)
				CaptureTreeListCore(this.control, null);
		}
		public override void UpdateTreeList() {
#if DEBUGTEST
			if(TestEnvironment.IsTestRunning())
				return;
#endif
			base.UpdateTreeList();
			if(control == null) return;
			control.ErrorMessage = string.Empty;
			try {
				CompilerErrorCollection errors = new DesignerHostExtensions(designerHost).ValidateReportScripts();
				ScriptControl scriptControl = (ScriptControl)this.GetService(typeof(ScriptControl));
				if(errors != null && errors.Count > 0 && scriptControl != null) {
					control.ShowErrors(errors, scriptControl.LinesCount);
					scriptControl.ShowErrors(errors);
				} else {
					control.ClearErrors();
				}
			} catch(Exception ex) {
				if(ex is DevExpress.XtraReports.Serialization.XRSerializationException)
					control.ErrorMessage = ex.Message;
			}
		}
		protected override void SubscribeEvents() {
		}
		protected override void UnsubscribeEvents() {
		}
	}
}
