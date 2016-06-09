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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
namespace DevExpress.Mvvm.Native {
	[SecuritySafeCritical]
	public class PropertyValidator {
		public static PropertyValidator FromAttributes(IEnumerable attributes, string propertyName) {
			try {
				var displayName = DataAnnotationsAttributeHelper.GetDisplayName(attributes.OfType<Attribute>()) ?? propertyName;
				var validationAttributes = attributes != null ? attributes.OfType<ValidationAttribute>().ToArray() : new ValidationAttribute[0];
				var dxValidationAttributes = attributes != null ? attributes.OfType<DXValidationAttribute>().ToArray() : new DXValidationAttribute[0];
				return validationAttributes.Any() || dxValidationAttributes.Any() ? new PropertyValidator(validationAttributes, dxValidationAttributes, propertyName, displayName) : null;
			} catch(TypeAccessException) {
				return null;
			}
		}
		readonly IEnumerable<ValidationAttribute> attributes;
		readonly IEnumerable<DXValidationAttribute> dxAttributes;
		readonly string propertyName;
		readonly string displayName;
		PropertyValidator(IEnumerable<ValidationAttribute> attributes, IEnumerable<DXValidationAttribute> dxAttributes, string propertyName, string displayName) {
			this.attributes = attributes;
			this.dxAttributes = dxAttributes;
			this.propertyName = propertyName;
			this.displayName = displayName;
		}
		public string GetErrorText(object value, object instance) {
			var sb = new StringBuilder();
			foreach(string error in GetErrors(value, instance)) {
				if(sb.Length > 0)
					sb.Append(' ');
				sb.Append(error);
			}
			return sb.ToString();
		}
		public IEnumerable<string> GetErrors(object value, object instance) {
			return attributes.Select(x => {
				ValidationResult vr = x.GetValidationResult(value, CreateValidationContext(instance));
				return vr != null ? vr.ErrorMessage : null;
			}).Concat(dxAttributes.Select(x => x.GetValidationResult(value, displayName ?? propertyName, instance)))
			.Where(x => !string.IsNullOrEmpty(x));
		}
		ValidationContext CreateValidationContext(object instance) {
			return new ValidationContext(instance, null, null) {
				MemberName = propertyName,
				DisplayName = displayName,
			};
		}
	}
}
