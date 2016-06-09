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
using System.Globalization;
namespace DevExpress.Charts.Native {
	public interface IAxisValueContainer {
		IAxisData Axis { get; }
		bool IsEnabled { get; }
		CultureInfo Culture { get; }
		object GetAxisValue();
		void SetAxisValue(object axisValue);
		double GetValue();
		void SetValue(double value);
	}
	public static class AxisValueContainerHelper {
		static bool IsCompatibleWith(IAxisValueContainer container, IScaleMap map) {
			return map.IsCompatible(container.GetAxisValue());
		}
		public static void UpdateAxisValueContainer(IAxisValueContainer container, IScaleMap map) {
			if (container.GetAxisValue() == null) {
				double value = container.GetValue();
				if (!Double.IsNaN(value))
					container.SetAxisValue(map.InternalToNative(value));
			}
			else if (IsCompatibleWith(container, map))
				container.SetValue(map.NativeToInternal(container.GetAxisValue()));
		}
		public static void UpdateAxisValue(IAxisValueContainer container, AxisScaleTypeMap map) {
			object axisValue = map.ConvertValue(container.GetAxisValue(), container.Culture);
			if (axisValue != null) {
				container.SetAxisValue(axisValue);
				if (axisValue is double)
					container.SetValue((double)axisValue);
			}
		}
		public static void UpdateAxisValue(IAxisValueContainer container1, IAxisValueContainer container2, AxisScaleTypeMap map) {
			object axisValue1 = map.ConvertValue(container1.GetAxisValue(), container1.Culture);
			object axisValue2 = map.ConvertValue(container2.GetAxisValue(), container2.Culture);
			if (axisValue1 == null || axisValue2 == null || axisValue1.GetType() == axisValue2.GetType()) {
				if (axisValue1 != null) {
					container1.SetAxisValue(axisValue1);
					if (axisValue1 is double)
						container1.SetValue((double)axisValue1);
				}
				if (axisValue2 != null) {
					container2.SetAxisValue(axisValue2);
					if (axisValue2 is double)
						container2.SetValue((double)axisValue2);
				}
			}
		}
	}
}
