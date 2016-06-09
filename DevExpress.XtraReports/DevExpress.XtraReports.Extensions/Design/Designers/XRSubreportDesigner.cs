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

using System.ComponentModel.Design;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraReports.UI;
using System;
using System.IO;
using DevExpress.XtraReports.Design.MouseTargets;
using DevExpress.XtraReports.Localization;
using System.Drawing.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.Design {
	[MouseTarget(typeof(SubreportMouseTarget))]
	public class XRSubreportDesigner : SubreportDesignerBase {
		DesignerVerbCollection verbs = new DesignerVerbCollection();
		public override DesignerVerbCollection Verbs {
			get {
				return verbs;
			}
		}
		public XRSubreportDesigner() {
			verbs.Add(new DesignerVerb(ReportStringId.SubreportDesigner_EditParameterBindings.GetString(), OnEditParamterBindings));
		}
		void OnEditParamterBindings(object sender, EventArgs e) {
			try {
				XRSmartTagService smartTagService = GetService(typeof(XRSmartTagService)) as XRSmartTagService;
				smartTagService.HidePopup();
				EditorContextHelper.EditValue(this, Component, "ParameterBindings");
			} catch { }
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRSubReportDesignerActionList(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
		}
	}
}
