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

using System;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.DashboardCommon.Native.Performance {
	public class ApproximationController {
		readonly List<SupportPoint> supportPoints = new List<SupportPoint>();
		int maxParameter = -1;
		int minParameter = -1;
		public IList<SupportPoint> SupportPoints { get { return supportPoints; } }
		public int MaxParameter { get { return maxParameter; } }
		public int MinParameter { get { return minParameter; } }
		public string SaveToCsv() {
			StringBuilder stb = new StringBuilder();
			for(int i = 0; i < supportPoints.Count; i++) {
				SupportPoint point = supportPoints[i];
				stb.AppendFormat("{0}, {1},", point.Parameter, point.Result);
				if(i != supportPoints.Count - 1)
					stb.Append('\n');
			}
			return stb.ToString();
		}
		public void RestoreFromCsv(string csvString) {
			supportPoints.Clear();
			string[] rows = csvString.Split('\n');
			foreach(string row in rows) {
				string[] values = row.Split(',');
				int parameter = Int32.Parse(values[0]);
				int result = Int32.Parse(values[1]);
				supportPoints.Add(new SupportPoint(parameter, result));
			}
			OnSupportPointsChanged();
		}
		public int GetApproximation(int parameter) {
			SupportPoint leftPoint = null;
			SupportPoint rightPoint = null;
			for(int i = 0; i < supportPoints.Count; i++) {
				if(supportPoints[i].Parameter <= parameter)
					leftPoint = supportPoints[i];
				if(i + 1 < supportPoints.Count) {
					if(supportPoints[i + 1].Parameter > parameter) {
						rightPoint = supportPoints[i + 1];
						break;
					}
				}
			}
			if(rightPoint == null)
				return leftPoint.Result;
			if(leftPoint == null)
				return supportPoints[0].Result;
			double pl = leftPoint.Parameter;
			double rl = leftPoint.Result;
			double pr = rightPoint.Parameter;
			double rr = rightPoint.Result;
			double b = (rr - rl * pr / pl) / (1 - pr / pl);
			double k = (rl - b) / pl;
			return Convert.ToInt32(k * parameter + b);
		}
		public void AddSupportPoint(SupportPoint point) {
			supportPoints.Add(point);
			OnSupportPointsChanged();
		}
		void OnSupportPointsChanged() {
			supportPoints.Sort((point1, point2) => {
				return point1.Parameter - point2.Parameter;
			});
			minParameter = supportPoints[0].Parameter;
			maxParameter = supportPoints[supportPoints.Count - 1].Parameter;
		}
	}
	public class SupportPoint {
		public SupportPoint(int parameter, int result) {
			Parameter = parameter;
			Result = result;
		}
		public int Parameter { get; private set; }
		public int Result { get; private set; }
	}
}
