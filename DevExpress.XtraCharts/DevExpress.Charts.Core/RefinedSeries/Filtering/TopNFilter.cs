﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
namespace DevExpress.Charts.Native {
	public class TopNFilterBehavior : ValueThresholdFilterBehavior {
		int CalculateMinPointIndex(IList<RefinedPoint> points) {
			int result = 0;
			for (int i = 1; i < points.Count; i++) {
				if (GetValue(points[result]) >= GetValue(points[i]))
					result = i;
			}
			return result;
		}
		protected override IList<RefinedPoint> FilterPoints(IList<RefinedPoint> initialPoints, double valueThreshold) {
			int topNCount = (int)valueThreshold;
			IList<RefinedPoint> result = new List<RefinedPoint>();
			int minPointIndex = -1;
			for (int i = 0; i < initialPoints.Count; i++) {
				if (result.Count < topNCount) {
					result.Add(initialPoints[i]);
					minPointIndex = -1;
				} else {
					if (minPointIndex < 0)
						minPointIndex = CalculateMinPointIndex(result);
					if (GetValue(result[minPointIndex]) < GetValue(initialPoints[i])) {
						AddToOthersPoints(result[minPointIndex]);
						result.RemoveAt(minPointIndex);
						result.Add(initialPoints[i]);
						minPointIndex = -1;
					}
					else
						AddToOthersPoints(initialPoints[i]);
				}					
			}
			return result;
		} 
	}
}
