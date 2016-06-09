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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using DevExpress.Data.Browsing;
using System.Windows.Data;
using DevExpress.Xpf.Data;
using DevExpress.Utils;
namespace DevExpress.Xpf.Data {
	public class RowTypeDescriptorListIndex : RowTypeDescriptorBase {
		int listIndex;
		public RowTypeDescriptorListIndex(WeakReference ownerReference, int listIndex)
			: base(ownerReference) {
			this.listIndex = listIndex;
		}
		GridDataProvider GridOwner { get { return (GridDataProvider)Owner; } }
		protected internal override object GetValue(DataColumnInfo info) {
			return GridOwner.GetRowValueByListIndex(listIndex, info);
		}
		protected internal override void SetValue(DataColumnInfo info, object value) {
			throw new NotImplementedException();
		}
		internal override object GetValue(string fieldName) {
			return GridOwner.GetRowValueByListIndex(listIndex, fieldName); ;
		}
	}
}
namespace DevExpress.Xpf.Grid {
	public class RowPropertyValueConverter : IValueConverter {
		public IValueConverter InnerConverter { get; set; }
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			object convertedValue = ConvertCore(value, parameter);
			if(InnerConverter != null)
				convertedValue = InnerConverter.Convert(convertedValue, targetType, parameter, culture);
			return convertedValue;
		}
		object ConvertCore(object value, object parameter) {
			RowTypeDescriptorBase rowTypeDescriptor = value as RowTypeDescriptorBase;
			if(rowTypeDescriptor == null)
				return null;
			return rowTypeDescriptor.GetValue(parameter as string);
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
