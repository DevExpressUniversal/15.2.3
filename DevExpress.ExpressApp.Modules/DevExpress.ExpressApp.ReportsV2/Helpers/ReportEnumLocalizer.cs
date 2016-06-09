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
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class ReportEnumLocalizer {
		private XtraReport report;
		private Dictionary<Type, EnumDescriptor> enumDescriptors = new Dictionary<Type, EnumDescriptor>();
		private static bool runTimeEnumDescriptorRegistration = true;
		public void Attach(XtraReport report) {
			this.report = report;
			AssignBeforePrintToChilds(report);
		}
		public XtraReport Report {
			get { return report; }
		}
		protected virtual object GetLocalizedValueIfEnum(XRControl control, object controlUnderlyingValue) {
			Guard.ArgumentNotNull(control, "control");
			if(controlUnderlyingValue != null && controlUnderlyingValue.GetType().IsEnum) {
				return GetEnumDescriptor(controlUnderlyingValue.GetType()).GetCaption(controlUnderlyingValue);
			}
			return controlUnderlyingValue;
		}
		private void control_EvaluateBinding(object sender, BindingEventArgs e) {
			object controlUnderlyingValue = e.Value;
			e.Value = GetLocalizedValueIfEnum((XRControl)sender, controlUnderlyingValue);
			if(controlUnderlyingValue != null && !controlUnderlyingValue.GetType().IsEnum) {
				((XRControl)sender).EvaluateBinding -= new BindingEventHandler(control_EvaluateBinding);
			}
		}
		private EnumDescriptor GetEnumDescriptor(Type type) {
			EnumDescriptor enumDescriptor;
			if(!enumDescriptors.TryGetValue(type, out enumDescriptor) && runTimeEnumDescriptorRegistration) {
				enumDescriptor = new EnumDescriptor(type);
				enumDescriptors[type] = enumDescriptor;
			}
			return enumDescriptor;
		}
		private void SetupEnumDescriptor(XRControl control) {
			foreach(XRBinding binding in control.DataBindings) {
				DataSourceBase dataSource = binding.DataSource as DataSourceBase;
				IServiceProvider serviceProvider = binding.DataSource as IServiceProvider;
				ITypesInfo typesInfo = serviceProvider != null ? serviceProvider.GetService(typeof(ITypesInfo)) as ITypesInfo : null;
				if(dataSource != null && typesInfo != null) {
					ITypeInfo typeInfo = typesInfo.FindTypeInfo(dataSource.ObjectTypeName);
					IMemberInfo member = typeInfo != null ? typeInfo.FindMember(binding.DataMember) : null;
					if(member != null) {
						Type memberType = member.MemberTypeInfo.IsNullable ? Nullable.GetUnderlyingType(member.MemberType) : member.MemberType;
						if(memberType.IsEnum && !enumDescriptors.ContainsKey(memberType)) {
							enumDescriptors.Add(memberType, new EnumDescriptor(memberType));
						}
					}
				}
			}
		}
		private void AssignBeforePrintToChilds(XRControl parent) {
			foreach(XRControl control in parent.Controls) {
				if(control is XRLabel || control is XRTableCell) {
					SetupEnumDescriptor(control);
					control.EvaluateBinding += new BindingEventHandler(control_EvaluateBinding);
				}
				else {
					if(control is XRSubreport) {
						if(((XRSubreport)control).ReportSource == null && !string.IsNullOrEmpty(((XRSubreport)control).ReportSourceUrl)) {
							((XRSubreport)control).BeforePrint += new System.Drawing.Printing.PrintEventHandler(ReportEnumLocalizer_BeforePrint);
						}
						else {
							if(((XRSubreport)control).ReportSource != null) {
								AssignBeforePrintToChilds(((XRSubreport)control).ReportSource);
							}
						}
					}
					else {
						AssignBeforePrintToChilds(control);
						if(control is Band) {
							foreach(Band subBand in ((Band)control).SubBands) {
								AssignBeforePrintToChilds(subBand);
							}
						}
					}
				}
			}
		}
		private void ReportEnumLocalizer_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e) {
			((XRSubreport)sender).BeforePrint -= new System.Drawing.Printing.PrintEventHandler(ReportEnumLocalizer_BeforePrint);
			if(((XRSubreport)sender).ReportSource != null) {
				AssignBeforePrintToChilds(((XRSubreport)sender).ReportSource);
			}
		}
	}
}
