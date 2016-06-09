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
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.General;
namespace DevExpress.ExpressApp.Reports {
	public abstract class ReportServiceController : Controller {
		protected abstract void ShowPreviewCore(IReportData reportData, CriteriaOperator criteria);
		protected virtual IObjectSpace CreateObjectSpace(Type type) {
			return Application.CreateObjectSpace(type);
		}
		public void ShowPreview(IReportData reportData, bool reloadReportDataInNewObjectSpace) {
			ShowPreview(reportData, null, reloadReportDataInNewObjectSpace);
		}
		public void ShowPreview(IReportData reportData, CriteriaOperator criteria, bool reloadReportDataInNewObjectSpace) {
			Guard.ArgumentNotNull(reportData, "reportData");
			IObjectSpace reportDataObjectSpace = null;
			IReportData workingReportData = reportData;
			try {
				if(reloadReportDataInNewObjectSpace) {
					reportDataObjectSpace = CreateObjectSpace(reportData.GetType());
					workingReportData = reportDataObjectSpace.GetObject<IReportData>(reportData);
				}
				ShowPreviewCore(workingReportData, criteria);
			}
			finally {
				if(reportDataObjectSpace != null) {
					reportDataObjectSpace.Dispose();
				}
			}
		}
		public void ShowPreview(Type reportDataType, object reportDataKey) {
			using(IObjectSpace reportDataObjectSpace = CreateObjectSpace(reportDataType)) {
				IReportData reportData = (IReportData)reportDataObjectSpace.GetObjectByKey(reportDataType, reportDataKey);
				ShowPreview(reportData, null, false);
			}
		}
		public void ShowPreview(IReportData reportData) {
			ShowPreview(reportData, null, true);
		}
		public void ShowPreview(IReportData reportData, CriteriaOperator criteria) {
			ShowPreview(reportData, criteria, true);
		}
	}
}
