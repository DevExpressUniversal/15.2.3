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

using System.Runtime.Serialization;
using DevExpress.XtraReports.Parameters;
#if !SL && !WINRT
using System;
using DevExpress.XtraReports.Native;
#endif
namespace DevExpress.DocumentServices.ServiceModel.DataContracts {
	[DataContract]
#if !WINRT
	[KnownType(typeof(System.DBNull))]
#endif
	public class ReportParameter {
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public string Path { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public object Value { get; set; }
		[DataMember]
		public bool Visible { get; set; }
		[DataMember]
		public bool IsFilteredLookUpSettings { get; set; }
		[DataMember]
		public LookUpValueCollection LookUpValues { get; set; }
		[DataMember]
		public bool MultiValue { get; set; }
		public ReportParameter() {
		}
#if !WINRT
		public ReportParameter(ParameterPath parameterPath, LookUpValueCollection lookUpValues) {
			var parameter = parameterPath.Parameter;
			Description = parameter.Description;
			Path = parameterPath.Path;
			Name = parameter.Name;
			Value = GetValue(parameter);
			MultiValue = parameter.MultiValue;
			Visible = parameter.Visible;
			LookUpValues = lookUpValues;
		}
		public ReportParameter(ParameterPath parameterPath, LookUpValueCollection lookUpValues, bool isFiltered)
			: this(parameterPath, lookUpValues) {
			IsFilteredLookUpSettings = isFiltered;
		}
		static object GetValue(Parameter parameter) {
			if(!parameter.MultiValue) {
				return parameter.Value;
			}
			return parameter.Value ?? Array.CreateInstance(parameter.Type, 0);
		}
#endif
	}
}
