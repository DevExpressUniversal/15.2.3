#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Globalization;
namespace DevExpress.DashboardWin.Native {
	public abstract class ListPropertyTypeConverter<TValue> : PropertyProviderTypeConverter<TValue> where TValue : class {
		internal static SelectedContextService GetSelectedContextService(ITypeDescriptorContext context) {
			return context.GetService(typeof(SelectedContextService)) as SelectedContextService;
		}
		protected abstract bool IsSupportNullValue { get; }
		protected abstract string NullValueCaption { get; }
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return false;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)) {
				if(value == null) { 
					List<ListItem> list = GetItems(context);
					return list.Count > 0 ? list[0].Caption : string.Empty;
				} else {
					return GetValueCaption(context, value as TValue);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			IList list = GetItems(context);
			return new StandardValuesCollection(list);
		}
		protected abstract ICollection<TValue> GetValues(ITypeDescriptorContext context);
		protected abstract string GetValueCaption(ITypeDescriptorContext context, TValue value);
		List<ListItem> GetItems(ITypeDescriptorContext context) {
			List<ListItem> list = new List<ListItem>();
			ICollection<TValue> values = GetValues(context);
			if(IsSupportNullValue) {
				list.Add(new ListItem(null, NullValueCaption));
			}
			if (values != null) {
				foreach (TValue value in values) {
					string caption = GetValueCaption(context, value);
					list.Add(new ListItem(value, caption));
				}
			}
			return list;
		}
	}
}
