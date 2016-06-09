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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class Clusters : List<Cluster> {
		protected const double factor = 50;
		readonly double topHoleLimit;
		double maxSize = double.NaN;
		double minDistance = double.NaN;
		public double MaxSize { get { return maxSize; } }
		public double MinDistance { get { return minDistance; } }
		public Clusters(List<double> initialValues) : this(initialValues, int.MaxValue) { }
		public Clusters(List<double> initialValues, int clusterMaxCount) {
			if(initialValues.Count == 0 || clusterMaxCount < 2)
				return;
			initialValues.Sort();
			Cluster initialCluster = new Cluster(initialValues);
			topHoleLimit = initialCluster.MinHole * factor;
			Add(initialCluster);
			for(; ; ) {
				int index = GetIndexOfClusterWithMaxHole();
				if(index == -1)
					break;
				Cluster cluster = this[index];
				if(cluster.MaxHole < topHoleLimit)
					break;
				Cluster cluster1, cluster2;
				cluster.Split(out cluster1, out cluster2);
				if(cluster1 != null && cluster2 != null) {
					RemoveAt(index);
					Insert(index, cluster2);
					Insert(index, cluster1);
					if(Count == clusterMaxCount)
						break;
				}
			}
			CalculateMaxSize();
			CalculateMinDistance();
		}
		void CalculateMaxSize() {
			foreach(Cluster cluster in this)
				maxSize = double.IsNaN(maxSize) ? cluster.Size : Math.Max(maxSize, cluster.Size);
		}
		void CalculateMinDistance() {
			for(int i = 0; i < Count - 1; i++) {
				double distance = this[i + 1].FirstValue - this[i].LastValue;
				minDistance = double.IsNaN(minDistance) ? distance : Math.Min(minDistance, distance);
			}
		}
		int GetIndexOfClusterWithMaxHole() {
			int resultIndex = -1;
			double maxHole = double.NaN;
			for(int i = 0; i < Count; i++) {
				Cluster cluster = this[i];
				if(!double.IsNaN(cluster.MaxHole)) {
					if(double.IsNaN(maxHole) || cluster.MaxHole > maxHole) {
						maxHole = cluster.MaxHole;
						resultIndex = i;
					}
				}
			}
			return resultIndex;
		}
	}
	public class Cluster {
		readonly List<double> values = new List<double>();
		double minHole = double.NaN;
		double maxHole = double.NaN;
		int maxHoleIndex = -1;
		public List<double> Values { get { return values; } }
		public double FirstValue { get { return values.Count > 0 ? values[0] : double.NaN; } }
		public double LastValue { get { return values.Count > 0 ? values[values.Count - 1]: double.NaN; } }
		public double MinHole { get { return minHole; } }
		public double MaxHole { get { return maxHole; } }
		public int MaxHoleIndex { get { return maxHoleIndex; } }
		public double Size { get { return LastValue - FirstValue; } }
		public Cluster() {
		}
		public Cluster(IEnumerable<double> sortedValues) {
			foreach(double value in sortedValues)
				AddValue(value);
		}		
		void AddValue(double value) {
			if(values.Count > 0) {
				double hole = value - LastValue;
				if(hole > 0) {
					minHole = double.IsNaN(minHole) ? hole : Math.Min(minHole, hole);
					if(double.IsNaN(maxHole) || hole > maxHole) {
						maxHole = hole;
						maxHoleIndex = values.Count - 1;
					}
				}
			}
			values.Add(value);
		}
		public void Split(out Cluster cluster1, out Cluster cluster2) {
			if(values.Count > 1 && maxHoleIndex != -1) {
				cluster1 = new Cluster();
				cluster2 = new Cluster();
				for(int i = 0; i <= maxHoleIndex; i++)
					cluster1.AddValue(values[i]);
				for(int i = maxHoleIndex + 1; i < values.Count; i++)
					cluster2.AddValue(values[i]);
			}
			else {
				cluster1 = null;
				cluster2 = null;
			}
		}
	}
}
