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
using System.Linq;
using System.Text;
namespace DevExpress.Mvvm.Native {
	public static class DesignTimeValuesProvider {
		public static readonly DateTime Today = DateTime.Today;
		public static object[] Types = new object[] { typeof(string), typeof(DateTime), typeof(int), typeof(Decimal), typeof(byte),
												 typeof(Int64), typeof(double), typeof(bool) };
		public static object[] CreateValues() {
			return new object[] { "string", Today, 123, 123, 123, 123, 123, null };
		}
		public static object[][] CreateDistinctValues() {
			return new object[][] { 
					new object[] { "string1", Today, 123, 123, (byte)123, 123, 123, null },
					new object[] { "string2", Today.AddDays(1), 456, 456, (byte)124, 456, 456, true },
					new object[] { "string3", Today.AddDays(2), 789, 789, (byte)125, 789, 789, false },
				};
		}
		static readonly object[][] distinctValues = CreateDistinctValues();
		public static object GetDesignTimeValue(Type propertyType, int index) {
			return GetDesignTimeValue(propertyType, index, null, distinctValues, true);
		}
		public static object GetDesignTimeValue(Type propertyType, object component, object[] values, object[][] distinctValues, bool useDistinctValues) {
			Type type = Nullable.GetUnderlyingType(propertyType);
			if(type == null)
				type = propertyType;
			int index = Array.IndexOf(DesignTimeValuesProvider.Types, type);
			if(index == -1) {
				if(type.IsValueType)
					return Activator.CreateInstance(type);
				else
					return null;
			}
			object value = GetValues((int)component, values, distinctValues, useDistinctValues)[index];
			return value != null ? Convert.ChangeType(value, type, null) : null;
		}
		static object[] GetValues(int index, object[] values, object[][] distinctValues, bool useDistinctValues) {
			return useDistinctValues ? distinctValues[index % distinctValues.Length] : values;
		}
	}
}
