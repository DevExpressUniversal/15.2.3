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
namespace DevExpress.Utils {
	public static class HashCodeHelper {
		public static int CalcHashCode2(params int[] array) {
			int num = 0x15051505;
			int num2 = num;
			int i = 0;
			for(int j = array.Length * 2; j > 0; j -= 4, i += 2) {
				num = (((num << 5) + num) + (num >> 0x1b)) ^ array[i];
				if(j <= 2) {
					break;
				}
				num2 = (((num2 << 5) + num2) + (num2 >> 0x1b)) ^ array[i + 1];
			}
			return num + (num2 * 0x5d588b65);
		}
		public static int CalcHashCode(params int[] values) {
			int count = values.Length;
			int result = 0;
			for(int i = 0; i < count; i++) {
				result ^= RotateValue(values[i], count);
			}
			return result;
		}
		public static int CalcHashCode(params object[] values) {
			int result = 0;
			foreach(object value in values)
				if(value != null)
					result ^= value.GetHashCode();
			return result;
		}
		public static int RotateValue(int val, int count) {
			int shift = (13 * count) & 0x1F;
			return (val << shift) | (val >> (32 - shift));
		}
	}
}
