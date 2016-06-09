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
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Presenters {
	public class SetReportServerEndpointPresenter<TView> : RemoteDocumentSourcePagePresenterBase<IPageView> {
		readonly IAppConfigHelper configHelper;
		protected new ISetReportServerEndpointView View { get { return (ISetReportServerEndpointView)base.View; } }
		public override bool FinishEnabled {
			get { return !string.IsNullOrWhiteSpace(View.Endpoint); }
		}
		public override bool MoveNextEnabled {
			get { return false; }
		}
		public SetReportServerEndpointPresenter(ISetReportServerEndpointView view, IAppConfigHelper configHelper)
			: base(view) {
			this.configHelper = configHelper;
		}
		public override Type GetNextPageType() {
			return null;
		}
		public override void Begin() {
			View.FillEndpoints(configHelper.GetEndpointNames(Model.DocumentSourceType));
			View.GenerateEndpointsChanged += View_GenerateEndpointsChanged;
			View.EndpointChanged += View_EndpointChanged;
			View.Endpoint = string.IsNullOrEmpty(Model.Endpoint)
				? configHelper.GetUniqueEndpointName(Model.DocumentSourceType.ToString())
				: Model.Endpoint;
			View.GenerateEndpoints = Model.GenerateEndpoints;
		}
		public override void Commit() {
			View.EndpointChanged -= View_EndpointChanged;
			Model.GenerateEndpoints = View.GenerateEndpoints;
			Model.Endpoint = View.GenerateEndpoints ? View.Endpoint : string.Empty;
		}
		void View_EndpointChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		void View_GenerateEndpointsChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
	}
}
