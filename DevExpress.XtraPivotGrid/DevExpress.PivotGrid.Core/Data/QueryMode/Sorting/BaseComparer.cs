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

using DevExpress.Compatibility.System.ComponentModel;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
namespace DevExpress.PivotGrid.QueryMode.Sorting {
	class BaseComparer {
		protected static int CompareCatched(object value1, object value2) {
			if(object.ReferenceEquals(value1, null))
				if(object.ReferenceEquals(value2, null))
					return 0;
				else
					return 1;
			else
				if(object.ReferenceEquals(value2, null))
					return -1;
			Type type1 = value1.GetType();
			Type type2 = value2.GetType();
			if(type1 != type2) {
				try {
					return Comparer.Default.Compare(value1, ChangeType(value2, type1, type2));
				} catch {
					return Comparer.Default.Compare(value2, ChangeType(value1, type2, type1));
				}
			} else
				return Comparer.Default.Compare(value1, value2);
		}
	   internal static object ChangeType(object value, Type toType, Type fromType) {
			TypeConverter converter = TypeDescriptor.GetConverter(toType);
			try {
				if(converter.CanConvertFrom(fromType))
					return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
			} catch { }
			converter = TypeDescriptor.GetConverter(fromType);
			try {
				if(converter.CanConvertTo(toType))
					return converter.ConvertTo(null, CultureInfo.InvariantCulture, value, toType);
			} catch { }
			return Convert.ChangeType(value, toType, CultureInfo.InvariantCulture);
		}
	}
}
