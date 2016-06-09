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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Web.Native.ParametersPanel.MultiValue {
	class SimpleTokenConverter : ITokenConverter {
		readonly Type type;
		readonly bool isTypeValueOrString;
		readonly TypeConverter converter;
		public SimpleTokenConverter(Type type) {
			Guard.ArgumentNotNull(type, "type");
			this.type = type;
			isTypeValueOrString = type.IsValueType || type == typeof(string);
			var converter = TypeDescriptor.GetConverter(type);
			if(converter.CanConvertTo(typeof(string))) {
				this.converter = converter;
			}
		}
		public KeyValuePair<object, string> GetEntity(object value) {
			if(value == null) {
				return new KeyValuePair<object, string>(null, "");
			}
			if(value is IConvertible && isTypeValueOrString) {
				value = Convert.ChangeType(value, type);
			}
			var description = converter != null ? converter.ConvertToString(value) : value.ToString();
			return new KeyValuePair<object, string>(value, description);
		}
	}
}
