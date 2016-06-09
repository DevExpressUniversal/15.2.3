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
using DevExpress.Data.WizardFramework;
using DevExpress.Data.XtraReports.ServiceModel;
using DevExpress.Utils;
namespace DevExpress.Data.XtraReports.Wizard.Presenters {
	public abstract class ReportWizardServiceClientPage<TView, TModel> : WizardPageBase<TView, TModel> where TModel : ReportModel {
		Guid pageSessionId;
		protected IReportWizardServiceClient Client { get; private set; }
		protected Guid PageSessionId { get { return pageSessionId; } }
		protected ReportWizardServiceClientPage(TView view, IReportWizardServiceClient client) : base(view) {
			Guard.ArgumentNotNull(client, "client");
			Client = client;
		}
		protected bool HandleError(AsyncCompletedEventArgs args, string operationContext) {
			if(args.Error != null || args.Cancelled) {
				string message = string.Format("{0} {1}", operationContext, args.Error != null ? args.Error.Message : "Operation has been cancelled");
				RaiseError(message);
				return true;
			}
			return false;
		}
		public override void Begin() {
			pageSessionId = Guid.NewGuid();
		}
		public override void Commit() {
			pageSessionId = Guid.Empty;
		}
	}
}
