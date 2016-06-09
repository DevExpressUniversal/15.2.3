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

using DevExpress.DashboardCommon.ViewerData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.ViewerData {
	public class AxisPointTuple {
		IList<AxisPoint> axisPoints;
		List<string> availableAxisNames;
		internal IList<AxisPoint> AxisPoints { get { return axisPoints; } }
		public bool IsEmpty { get { return axisPoints.Count == 0; } }
		public List<string> AvailableAxisNames { get { return availableAxisNames; } }
		internal AxisPointTuple(AxisPoint axisPoint)
			: this(new List<AxisPoint>() { axisPoint }) {
		}
		internal AxisPointTuple(IList<AxisPoint> axisPoints) {
			List<string> axisNameList = new List<string>();
			if(axisPoints != null && axisPoints.Count > 0) {
				foreach(AxisPoint axisPoint in axisPoints) {
					if(axisNameList.Contains(axisPoint.AxisName))
						throw new Exception("An AxisPointTuple object cannon contain axis points with the same axis names.");
					else
						axisNameList.Add(axisPoint.AxisName);
				}
			}
			this.availableAxisNames = axisNameList;
			this.axisPoints = axisPoints;
		}
		public AxisPoint GetAxisPoint() {
			switch (availableAxisNames.Count) {
				case 0:
					return null;
				case 1:
					return GetAxisPoint(availableAxisNames[0]);
				default:
					int mostPriorityIndex = DashboardDataAxisNames.MostPriority(availableAxisNames);
					if(mostPriorityIndex >= 0)
						return GetAxisPoint(availableAxisNames[mostPriorityIndex]);
					return null;
			}
		}
		public AxisPoint GetAxisPoint(string axisName) {
			return axisPoints.FirstOrDefault(point => String.Equals(point.AxisName, axisName));
		}
		public override bool Equals(object obj) {
			AxisPointTuple axisPointTuple = obj as AxisPointTuple;
			if(axisPointTuple != null && axisPointTuple.AxisPoints != null && axisPoints != null && axisPointTuple.AxisPoints.Count == axisPoints.Count) {
				foreach(AxisPoint axisPoint in axisPoints) {
					AxisPoint newAxisPoint = axisPointTuple.GetAxisPoint(axisPoint.AxisName);
					if(newAxisPoint != null) {
						if(!axisPoint.Equals(newAxisPoint))
							return false;
					}
				}
				return true;
			}
			return false;
		}
		public override int GetHashCode() {
			int hash = 0;
			foreach(AxisPoint axisPoint in axisPoints)
				hash ^= axisPoint.GetHashCode();
			return hash;
		}
	}
}
