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

using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class CustomShowPreviewEventArgs : HandledEventArgs {
		private string reportContainerHandle;
		private ReportParametersObjectBase parametersObject;
		private CriteriaOperator criteria;
		private bool canApplyCriteria;
		private SortProperty[] sortProperty;
		private bool canApplySortProperty;
		private ShowViewParameters showViewParameters;
		public CustomShowPreviewEventArgs(string reportContainerHandle, ReportParametersObjectBase parametersObject, CriteriaOperator criteria, bool canApplyCriteria, SortProperty[] sortProperty, bool canApplySortProperty, ShowViewParameters showViewParameters) {
			this.reportContainerHandle = reportContainerHandle;
			this.parametersObject = parametersObject;
			this.criteria = criteria;
			this.canApplyCriteria = canApplyCriteria;
			this.sortProperty = sortProperty;
			this.canApplySortProperty = canApplySortProperty;
			this.showViewParameters = showViewParameters;
		}
		public string ReportContainerHandle {
			get { return reportContainerHandle; }
		}
		public ReportParametersObjectBase ParametersObject {
			get { return parametersObject; }
		}
		public CriteriaOperator Criteria {
			get { return criteria; }
		}
		public bool CanApplyCriteria {
			get { return canApplyCriteria; }
		}
		public SortProperty[] SortProperty {
			get { return sortProperty; }
		}
		public bool CanApplySortProperty {
			get { return canApplySortProperty; }
		}
		public ShowViewParameters ShowViewParameters {
			get { return showViewParameters; }
		}
	}
}
