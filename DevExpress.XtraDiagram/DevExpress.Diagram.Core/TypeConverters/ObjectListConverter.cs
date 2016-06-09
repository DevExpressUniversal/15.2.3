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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections;
namespace DevExpress.Diagram.Core.TypeConverters {
	public abstract class ObjectListTypeConverterBase : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if((destinationType == typeof(string)) && (value is IList)) {
				IList list = (IList)value;
				return string.Join(GetListSeparator().ToString(), GetStringValues(list));
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		protected IEnumerable<string> GetStringValues(IList list) {
			return list.Cast<object>().Select(item => {
				TypeConverter c = TypeDescriptor.GetConverter(item);
				if(c == null || !c.CanConvertTo(typeof(string))) {
					throw new ArgumentException("list");
				}
				return (string)c.ConvertTo(item, typeof(string));
			});
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string) {
				string listValue = (string)value;
				IList list = CreateList();
				string[] items = listValue.Split(GetListSeparator());
				for(int i = 0; i < items.Length; i++) {
					string item = items[i];
					TypeConverter c = GetItemConverter(item);
					if(c == null || !c.CanConvertFrom(typeof(string))) {
						throw new ArgumentException(item.ToString());
					}
					list.Add(c.ConvertFrom(item));
				}
				return list;
			}
			return base.ConvertFrom(context, culture, value);
		}
		protected virtual TypeConverter GetItemConverter(string item) {
			return TypeDescriptor.GetConverter(typeof(string));
		}
		protected char GetListSeparator() {
			return CultureInfo.InvariantCulture.TextInfo.ListSeparator.First();
		}
		protected abstract IList CreateList();
	}
	public class AdditionalStyleListTypeConverter : ObjectListTypeConverterBase {
		protected override IList CreateList() {
			return new AdditionalStyleList();
		}
	}
}
