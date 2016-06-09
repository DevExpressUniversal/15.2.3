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

using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Xpf.Core.Native {
	public class ScaleQualifier : IUriQualifier {
		const string nameValue = "scale";
		public event EventHandler ActiveValueChanged { add { } remove { } }
		public string Name { get { return nameValue; } }
		public int GetAltitude(DependencyObject context, string value, IEnumerable<string> values, out int maxAltitude) {
			IEnumerable<int> integerValues = values.Select(int.Parse).OrderBy(x => x);
			maxAltitude = integerValues.Count();
			int closestValue = GetClosestValue(integerValues, int.Parse(value));
			int currentValue = GetClosestValue(integerValues, (int)ScreenHelper.ScaleX * 100);
			var closestIndex = GetIndex(integerValues, closestValue);
			var currentIndex = GetIndex(integerValues, currentValue);
			if (closestIndex == -1 || currentIndex == -1)
				return -1;
			var result = currentIndex - closestIndex;
			if (result < 0)
				return -1;
			return result + 1;
		}
		static int GetIndex(IEnumerable<int> integerValues, int checkValue) {
			int i = 0;
			foreach(var element in integerValues) {
				if (checkValue == element)
					return i;
				i++;
			}
			return -1;
		}
		static int GetClosestValue(IEnumerable<int> integerValues, int checkValue) {
			return integerValues.Select(x => new Tuple<int, int>(x, Math.Abs(checkValue - x))).OrderBy(x => x.Item2).FirstOrDefault().Return(x => x.Item1, () => -1);
		}
		public bool IsValidValue(string value) {
			int result;
			return int.TryParse(value, out result);
		}
	}
}
