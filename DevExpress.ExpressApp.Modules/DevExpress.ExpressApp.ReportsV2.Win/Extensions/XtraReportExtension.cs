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
using DevExpress.Data;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2.Win {
	public class XtraReportExtension : XtraReportExtensionBase {
		public XtraReportExtension() {
		}
		protected override RepositoryItem CreateRepositoryItem(DevExpress.Data.DataColumnInfo dataColumnInfo, Type dataType, XtraReport report) {
			RepositoryItem result;
			if(OnCreateCustomReportRepositoryItem(report, dataType, dataColumnInfo, null, out result)) {
				return result;
			}
			else {
				return base.CreateRepositoryItem(dataColumnInfo, dataType, report);
			}
		}
		protected override RepositoryItem CreateRepositoryItem(DevExpress.XtraReports.Parameters.Parameter parameter, Type dataType, XtraReport report) {
			RepositoryItem result;
			if(OnCreateCustomReportRepositoryItem(report, parameter.Type, null, parameter, out result)) {
				return result;
			}
			else {
				return base.CreateRepositoryItem(parameter, dataType, report);
			}
		}
		private bool OnCreateCustomReportRepositoryItem(XtraReport report, Type dataType, DataColumnInfo dataColumnInfo, Parameter parameter, out RepositoryItem repositoryItem) {
			repositoryItem = null;
			CreateCustomReportDesignRepositoryItemEventArgs args = new CreateCustomReportDesignRepositoryItemEventArgs(Application, report, dataType, dataColumnInfo, parameter);
			if(CreateCustomReportRepositoryItem != null) {
				CreateCustomReportRepositoryItem(this, args);
				if(args.Handled) {
					repositoryItem = args.RepositoryItem;
					return true;
				}
			}
			return false;
		}
		public event EventHandler<CreateCustomReportDesignRepositoryItemEventArgs> CreateCustomReportRepositoryItem;
	}
}
