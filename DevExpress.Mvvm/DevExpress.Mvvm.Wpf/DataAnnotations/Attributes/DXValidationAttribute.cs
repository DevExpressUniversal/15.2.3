#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
namespace DevExpress.Mvvm.Native {
	public abstract class DXValidationAttribute : Attribute {
		readonly Func<string> errorMessageAccessor;
		protected DXValidationAttribute() { throw new NotSupportedException(); }
		protected DXValidationAttribute(Func<string> errorMessageAccessor, Func<string> defaultErrorMessageAccessor) {
			this.errorMessageAccessor = errorMessageAccessor ?? defaultErrorMessageAccessor;
		}
		protected virtual string FormatErrorMessage(string name) {
			return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
		}
		public string GetValidationResult(object value, string memberName, object instance = null) {
			if(memberName == null) {
				throw new ArgumentNullException("memberName");
			}
			if(!IsValid(value) || !IsInstanceValid(value, instance)) {
				return FormatErrorMessage(memberName);
			}
			return null;
		}
		protected abstract bool IsValid(object value);
		protected virtual bool IsInstanceValid(object value, object instance) { return true; }
		protected string ErrorMessageString { get { return errorMessageAccessor(); } }
	}
}
