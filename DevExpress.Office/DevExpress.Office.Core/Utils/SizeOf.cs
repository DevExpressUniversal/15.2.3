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
using System.Reflection;
namespace DevExpress.Office {
	#region SizeOfInfo
	public class SizeOfInfo {
		readonly string displayName;
		readonly int sizeOf;
		readonly int count;
		public SizeOfInfo(string displayName, int sizeOf, int count) {
			this.displayName = displayName;
			this.sizeOf = sizeOf;
			this.count = count;
		}
		public string DisplayName { get { return displayName; } }
		public int SizeOf { get { return sizeOf; } }
		public int Count { get { return count; } }
	}
	#endregion
	#region ObjectSizeHelper
	public static class ObjectSizeHelper {
		public static SizeOfInfo CalculateTotalSizeOfInfo(List<SizeOfInfo> list, string displayName) {
			int result = 0;
			int count = list.Count;
			for (int i = 0; i < count; i++)
				result += list[i].SizeOf;
			return new SizeOfInfo(displayName, result, -1);
		}
		public static List<SizeOfInfo> CalculateSizeOfInfo(object obj) {
			List<SizeOfInfo> result = new List<SizeOfInfo>();
			FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			foreach (FieldInfo field in fields) {
				object fieldValue = field.GetValue(obj);
				ISupportsSizeOf sizeofObj = fieldValue as ISupportsSizeOf;
				if (sizeofObj != null) {
					result.Add(new SizeOfInfo(field.Name, sizeofObj.SizeOf(), CalculateCount(fieldValue)));
				}
				else
					result.Add(new SizeOfInfo(field.Name, CalculateApproxObjectSize32(fieldValue), CalculateCount(fieldValue)));
			}
			return result;
		}
		public static int CalculateApproxObjectSize32(object obj) {
			return CalculateApproxObjectSize32(obj, true);
		}
		public static int CalculateApproxObjectSize32(object obj, bool ignoreStringSize) {
			if (obj is ValueType)
				return CalculateApproxObjectSize32Core(obj, ignoreStringSize);
			else
				return CalculateApproxReferenceObjectSize32Core(obj, ignoreStringSize);
		}
		static int CalculateCount(object obj) {
			PropertyInfo pi = obj.GetType().GetProperty("Count");
			if (pi != null)
				return (int)pi.GetValue(obj, null);
			ICollection collection = obj as ICollection;
			if (collection != null)
				return collection.Count;
			else
				return -1;
		}
		static int CalculateApproxObjectSize32Core(object obj, bool ignoreStringSize) {
			if (Object.ReferenceEquals(obj, null))
				return sizeof(Int32);
			if (obj is ValueType) {
				if (obj is Enum)
					return CalculateApproxValueObjectSize32(Enum.GetUnderlyingType(obj.GetType()));
				else
					return CalculateApproxValueObjectSize32(obj.GetType());
			}
			else
				return CalculateApproxReferenceObjectSize32Core(obj, ignoreStringSize);
		}
		static int CalculateApproxReferenceObjectSize32Core(object obj, bool ignoreStringSize) {
			if (obj is IDocumentModel)
				return 0;
			string str = obj as string;
			if (str != null) {
				if (ignoreStringSize)
					return sizeof(Int32);
				else
					return sizeof(Int32) + str.Length * sizeof(char);
			}
			else {
				int result = 0;
				FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				foreach (FieldInfo field in fields) {
					object fieldValue = field.GetValue(obj);
					ISupportsSizeOf sizeofObj = fieldValue as ISupportsSizeOf;
					if (sizeofObj != null)
						return sizeofObj.SizeOf();
					else
						result += CalculateApproxObjectSize32Core(fieldValue, ignoreStringSize);
				}
				return result;
			}
		}
		static int CalculateApproxValueObjectSize32(Type type) {
			int result;
			if (!primitiveTypesSizeOfTable.TryGetValue(type, out result))
#if DXRESTRICTED
			{
				const int undefinedTypeSize = 4; 
				return undefinedTypeSize;
			}
#else
				return System.Runtime.InteropServices.Marshal.SizeOf(type);
#endif
			else
				return result;
		}
		static Dictionary<Type, int> primitiveTypesSizeOfTable = CreatePrimitiveTypesSizeofTable();
		static Dictionary<Type, int> CreatePrimitiveTypesSizeofTable() {
			Dictionary<Type, int> result = new Dictionary<Type, int>();
			result.Add(typeof(bool), sizeof(bool));
			result.Add(typeof(byte), sizeof(byte));
			result.Add(typeof(sbyte), sizeof(sbyte));
			result.Add(typeof(char), sizeof(char));
			result.Add(typeof(Int16), sizeof(Int16));
			result.Add(typeof(UInt16), sizeof(UInt16));
			result.Add(typeof(Int32), sizeof(Int32));
			result.Add(typeof(UInt32), sizeof(Int32));
			result.Add(typeof(Int64), sizeof(Int64));
			result.Add(typeof(UInt64), sizeof(Int64));
			result.Add(typeof(Decimal), sizeof(Decimal));
			result.Add(typeof(Single), sizeof(Single));
			result.Add(typeof(Double), sizeof(Double));
			return result;
		}
	}
#endregion
}
