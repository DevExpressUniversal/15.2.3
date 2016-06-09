#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using DevExpress.XtraReports.Parameters;
namespace DevExpress.XtraReports.Web.Native.ParametersPanel {
	public abstract class ParametersEditorCreatorBase<T>
		where T : class {
		public T CreateEditorByParameter(Parameter parameter) {
			if(!parameter.Visible) {
				return null;
			}
			if(parameter.MultiValue) {
				return CreateMultiValueEditor(parameter);
			}
			if(parameter.LookUpSettings != null) {
				return CreateLookUpEditor(parameter);
			}
			var parameterType = parameter.Type;
			if(parameterType == typeof(bool)) {
				return CreateBoolComboBox();
			} if(parameterType == typeof(Guid)) {
				return CreateGuidTextBox();
			} else if(parameterType == typeof(DateTime)) {
				return CreateDateTimeEdit();
			} else if(WebReportParameterHelper.NumericTypes.Contains(parameterType)) {
				return CreateNumericEditor(parameterType);
			} else if(parameterType.IsEnum) {
				return CreateEnumComboBox(parameterType);
			}
			return CreateTextBox();
		}
		protected abstract T CreateMultiValueEditor(Parameter parameter);
		protected abstract T CreateLookUpEditor(Parameter parameter);
		protected abstract T CreateBoolComboBox();
		protected abstract T CreateGuidTextBox();
		protected abstract T CreateDateTimeEdit();
		protected abstract T CreateNumericEditor(Type parameterType);
		protected abstract T CreateEnumComboBox(Type parameterType);
		protected abstract T CreateTextBox();
	}
}
