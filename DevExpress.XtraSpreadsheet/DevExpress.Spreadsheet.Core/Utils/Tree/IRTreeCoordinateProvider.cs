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
namespace DevExpress.XtraSpreadsheet.Utils.Trees {
	public interface IRTreeCoordinateProvider<T> {
		T MinValue { get; }
		T MaxValue { get; }
		T Multiply(T value, float multiplier);
		T Sqrt(T value);
	}
	public class RTreeFloatCoordinateProvider : IRTreeCoordinateProvider<float> {
		public float MinValue { get { return float.MinValue; } }
		public float MaxValue { get { return float.MaxValue; } }
		public float Multiply(float value, float multiplier) {
			return value * multiplier;
		}
		public float Sqrt(float value) {
			return (float)Math.Sqrt(value);
		}
	}
	public class RTreeIntCoordinateProvider : IRTreeCoordinateProvider<int> {
		public int MinValue { get { return int.MinValue; } }
		public int MaxValue { get { return int.MaxValue; } }
		public int Multiply(int value, float multiplier) {
			return (int)(value * multiplier);
		}
		public int Sqrt(int value) {
			return (int)Math.Sqrt(value);
		}
	}
}
