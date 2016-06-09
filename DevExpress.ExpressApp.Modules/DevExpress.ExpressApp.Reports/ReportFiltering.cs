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
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Runtime.Serialization;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Reports {
	public class ReportFiltering {
		private string filter = "";
		private string filterForDesignPreview = "";
		private string filterDescription = "";
		private Type parametersObjectType;
		private XafReport report;
		private XafApplication application;
		private Locker initalizationCounter = new Locker();
		public ReportFiltering(XafReport report) {
			this.report = report;
			this.application = null;
		}
		public void BeginInit() {
			initalizationCounter.Lock();
		}
		public void EndInit() {
			initalizationCounter.Unlock();
		}
		public void SetApplication(XafApplication application) {
			this.application = application;
		}
		public bool ShouldSerializeFilter() {
			return !String.IsNullOrEmpty(filter);
		}
		public bool ShouldSerializeFilterForDesignPreview() {
			return filterForDesignPreview != CriteriaParametersProcessor.Process(filter);
		}
		[Browsable(false)]
		public XafReport Report {
			get { return report; }
		}
		[Browsable(false)]
		public XafApplication Application {
			get { return application; }
		}
		[Browsable(true), SettingsBindable(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[Editor("DevExpress.ExpressApp.Reports.Win.FilterEditor, DevExpress.ExpressApp.Reports.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(UITypeEditor))]
		[DXDisplayName(typeof(ReportFiltering), XafReport.DefaultResourceFile, "DevExpress.ExpressApp.Reports.Filter"),
		Localizable(true)]
#if !SL
	[DevExpressExpressAppReportsLocalizedDescription("ReportFilteringFilter")]
#endif
		public string Filter {
			get { return filter; }
			set {
				if(initalizationCounter.Locked) {
					filter = value;
				}
				else {
			   if(report.RaiseFilterChanging(value)) {
						filter = value;
						report.RaiseFilterChanged();
					}
				}
			}
		}
		[Browsable(true), SettingsBindable(true)]
		[Editor("DevExpress.ExpressApp.Reports.Win.FilterEditor, DevExpress.ExpressApp.Reports.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(UITypeEditor))]
		[DXDisplayName(typeof(ReportFiltering), XafReport.DefaultResourceFile, "DevExpress.ExpressApp.Reports.FilterForDesignPreview", "Filter For Design Preview"),
		Localizable(true)]
#if !SL
	[DevExpressExpressAppReportsLocalizedDescription("ReportFilteringFilterForDesignPreview")]
#endif
		public string FilterForDesignPreview {
			get { return filterForDesignPreview; }
			set {
				if(initalizationCounter.Locked) {
					filterForDesignPreview = value;
				}
				else {
					if(report.RaiseFilterForDesignPreviewChanging(value)) {
						filterForDesignPreview = value;
					}
				}
			}
		}
		[Browsable(true), SettingsBindable(true)]
		[Editor(DevExpress.ExpressApp.Utils.Constants.MultilineStringEditorType, typeof(System.Drawing.Design.UITypeEditor))]
		[DXDisplayName(typeof(ReportFiltering), XafReport.DefaultResourceFile, "DevExpress.ExpressApp.Reports.FilterDescription", "Filter Description"),
		Localizable(true)]
#if !SL
	[DevExpressExpressAppReportsLocalizedDescription("ReportFilteringFilterDescription")]
#endif
		public string FilterDescription {
			get { return filterDescription; }
			set { filterDescription = value; }
		}
		[Browsable(true), SettingsBindable(true)]
		[TypeConverter(typeof(ReportParametersObjectTypeConverter))]
		[DXDisplayName(typeof(ReportFiltering), XafReport.DefaultResourceFile, "DevExpress.ExpressApp.Reports.ParametersObjectType", "Parameters Object Type"),
		Localizable(true)]
#if !SL
	[DevExpressExpressAppReportsLocalizedDescription("ReportFilteringParametersObjectType")]
#endif
		public Type ParametersObjectType {
			get { return parametersObjectType; }
			set { parametersObjectType = value; }
		}
	}
	public class CriteriaParametersProcessor : CriteriaProcessorBase {
		protected override void Process(BinaryOperator theOperator) {
			if(!ReferenceEquals(theOperator, null)) {
				OperandValue operand = theOperator.LeftOperand as OperandValue;
				if(!ReferenceEquals(operand, null) && operand.Value == null) {
					theOperator.LeftOperand = CriteriaOperator.Clone(theOperator.RightOperand);
					theOperator.OperatorType = BinaryOperatorType.Equal;
				}
				operand = theOperator.RightOperand as OperandValue;
				if(!ReferenceEquals(operand, null) && operand.Value == null) {
					theOperator.RightOperand = CriteriaOperator.Clone(theOperator.LeftOperand);
					theOperator.OperatorType = BinaryOperatorType.Equal;
				}
			}
		}
		public static String Process(String criteriaString) {
			try {
				CriteriaOperator criteria = CriteriaOperator.Parse(criteriaString);
				if(!ReferenceEquals(criteria, null)) {
					CriteriaParametersProcessor criteriaParametersProcessor = new CriteriaParametersProcessor();
					criteriaParametersProcessor.Process(criteria);
					return criteria.ToString();
				}
				else {
					return "";
				}
			}
			catch {
				return criteriaString;
			}
		}
	}
}
