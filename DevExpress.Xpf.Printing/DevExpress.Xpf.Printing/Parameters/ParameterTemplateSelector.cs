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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Printing.Parameters.Models;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing.Parameters {
	public class ParameterTemplateSelector : DataTemplateSelector {
		#region templates
		DataTemplate lookUpEditTemplate = EditorTemplates.LookUpEditTemplate;
		DataTemplate booleanTemplate = EditorTemplates.BooleanTemplate;
		DataTemplate dateTimeTemplate = EditorTemplates.DateTimeTemplate;
		DataTemplate stringTemplate = EditorTemplates.StringTemplate;
		DataTemplate numericTemplate = EditorTemplates.NumericTemplate;
		DataTemplate numericFloatTemplate = EditorTemplates.NumericFloatTemplate;
		DataTemplate guidTemplate = EditorTemplates.GuidTemplate;
		DataTemplate multiValueTemplate = EditorTemplates.MultiValueTemplate;
		DataTemplate multiValueLookUpTemplate = EditorTemplates.MultiValueLookUpTemplate;
		public DataTemplate LookUpEditTemplate {
			get { return lookUpEditTemplate; }
			set { lookUpEditTemplate = value; }
		}
		public DataTemplate BooleanTemplate {
			get { return booleanTemplate; }
			set { booleanTemplate = value; }
		}
		public DataTemplate DateTimeTemplate {
			get { return dateTimeTemplate; }
			set { dateTimeTemplate = value; }
		}
		public DataTemplate StringTemplate {
			get { return stringTemplate; }
			set { stringTemplate = value; }
		}
		public DataTemplate NumericTemplate {
			get { return numericTemplate; }
			set { numericTemplate = value; }
		}
		public DataTemplate NumericFloatTemplate {
			get { return numericFloatTemplate; }
			set { numericFloatTemplate = value; }
		}
		public DataTemplate GuidTempate {
			get { return guidTemplate; }
			set { guidTemplate = value; }
		}
		public DataTemplate MultiValueTemplate {
			get { return multiValueTemplate; }
			set { multiValueTemplate = value; }
		}
		public DataTemplate MultiValueLookUpTemplate {
			get { return multiValueLookUpTemplate; }
			set { multiValueLookUpTemplate = value; }
		}
		#endregion
		public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container) {
			var parameterModel = item as ParameterModel;
			if(parameterModel == null)
				return null;
			if(parameterModel.MultiValue)
				return parameterModel.LookUpValues!=null ? MultiValueLookUpTemplate : MultiValueTemplate;
			else if(parameterModel.LookUpValues != null)
				return LookUpEditTemplate;
			var type = parameterModel.Parameter.Type;
			if(type == typeof(bool))
				return BooleanTemplate;
			if(type == typeof(DateTime))
				return DateTimeTemplate;
			if(PSNativeMethods.IsNumericalType(type)) {
				if(PSNativeMethods.IsFloatType(type))
					return NumericFloatTemplate;
				else
					return NumericTemplate;
			}
			if(type == typeof(Guid))
				return GuidTempate;			
			return StringTemplate;
		}
	}
}
