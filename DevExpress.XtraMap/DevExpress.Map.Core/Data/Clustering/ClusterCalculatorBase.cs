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

using DevExpress.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Map.Native {
	public struct ClusterCalculatorResult {
		public IList<IClusterItem> Items { get; set; }
		public int MaxItemsCountInGroup { get; set; }
		public int MinItemsCountInGroup { get; set; }
	}
	public interface IClusterCalculator {
		ClusterCalculatorResult CalculateGroups(IEnumerable<IClusterable> sourceItems, double step);
	}
	public class ClusterCore : IClusterItem, IClusterable {
		IList<IClusterable> clusteredItems = new List<IClusterable>();
		public IList<IClusterable> ClusteredItems { get { return clusteredItems; } set { clusteredItems = value; } }
		public CoordPoint Location { get; set; }
		public virtual IMapUnit UnitLocation { get; set; }
		public object Owner { get; set; }
		string IClusterItem.Text { get; set; }
		void IClusterItem.ApplySize(double size) {
		}
		IMapUnit IClusterItemCore.GetUnitLocation() {
			return UnitLocation;
		}
		public virtual IClusterItem CreateInstance() {
			return null;
		}
	}
	public abstract class ClusterCalculatorBase : IClusterCalculator {
		IClusterItemFactoryCore clusterItemFactory;
		protected ClusterCalculatorBase(IClusterItemFactoryCore clusterItemFactory) {
			this.clusterItemFactory = clusterItemFactory;
		}
		protected double GetSqrDist(IMapUnit p1, IMapUnit p2) {
			double dx = p1.X - p2.X;
			double dy = p1.Y - p2.Y;
			return dx * dx + dy * dy;
		}
		protected double GetDist(IMapUnit p1, IMapUnit p2) {
			return Math.Sqrt(GetSqrDist(p1, p2));
		}
		protected virtual ClusterCalculatorResult CreateResults(List<ClusterCore> clusters, int maxItemsCount, int minItemsCount) {
			IList<IClusterItem> results = new List<IClusterItem>();
			foreach(ClusterCore clusterCoreItem in clusters) {
				IClusterItem clusterItem = clusterItemFactory.CreateClusterItem(clusterCoreItem.ClusteredItems);
				clusterItem.Owner = clusterCoreItem.Owner;
				clusterItem.Location = clusterCoreItem.Location;
				if(clusterCoreItem.ClusteredItems.Count > 1) {
					clusterItem.Text = clusterCoreItem.ClusteredItems.Count.ToString();
					clusterItem.ApplySize(maxItemsCount);
					clusterItem.ClusteredItems = clusterCoreItem.ClusteredItems;
					results.Add(clusterItem);
				}
				else {
					IClusterItem clusterCore = clusterCoreItem.ClusteredItems[0] as IClusterItem;
					if(clusterCore != null)
						results.Add(clusterCore);
				}
			}
			return new ClusterCalculatorResult() { Items = results, MaxItemsCountInGroup = maxItemsCount, MinItemsCountInGroup = minItemsCount };
		}
		public abstract ClusterCalculatorResult CalculateGroups(IEnumerable<IClusterable> sourceItems, double step); 
	}
	public class MarkerClusterCalculator : ClusterCalculatorBase {
		protected internal class MarkerClusterBox {
			double HalfSize { get; set; }
			public ClusterCore Cluster { get; private set; }
			public MarkerClusterBox(ClusterCore cluster, double halfSize) {
				this.Cluster = cluster;
				this.HalfSize = halfSize;
			}
			public bool IsPointInBox(IMapUnit point) {
				return point.X >= Cluster.UnitLocation.X - this.HalfSize &&
					   point.X <= Cluster.UnitLocation.X + this.HalfSize &&
					   point.Y >= Cluster.UnitLocation.Y - this.HalfSize &&
					   point.Y <= Cluster.UnitLocation.Y + this.HalfSize;
			}
		}
		public MarkerClusterCalculator(IClusterItemFactoryCore clusterItemFactory) : base(clusterItemFactory) { }
		public override ClusterCalculatorResult CalculateGroups(IEnumerable<IClusterable> sourceItems, double step) {
			List<MarkerClusterBox> clusters = CreateClusters(sourceItems, step);
			List<ClusterCore> result = new List<ClusterCore>();
			int maxItemsCount = int.MinValue;
			int minItemsCount = int.MaxValue;
			foreach(MarkerClusterBox cluster in clusters) {
				result.Add(cluster.Cluster);
				if(cluster.Cluster.ClusteredItems.Count > maxItemsCount)
					maxItemsCount = cluster.Cluster.ClusteredItems.Count;
				if(cluster.Cluster.ClusteredItems.Count < minItemsCount)
					minItemsCount = cluster.Cluster.ClusteredItems.Count;
			}
			return CreateResults(result, maxItemsCount, minItemsCount);
		}
		protected internal MarkerClusterBox GetClosestBoxCenter(List<MarkerClusterBox> clusters, IClusterable point) {
			double minDist = double.MaxValue;
			MarkerClusterBox closest = null;
			IMapUnit pointUnit = point.GetUnitLocation();
			foreach (MarkerClusterBox clusterBox in clusters) {
				double dist = GetSqrDist(pointUnit, clusterBox.Cluster.UnitLocation);
				if (dist < minDist) {
					minDist = dist;
					closest = clusterBox;
				}
			}
			return closest;
		}
		List<MarkerClusterBox> CreateClusters(IEnumerable<IClusterable> sourceItems, double step) {
			double halfSize = step;
			List<MarkerClusterBox> clusters = new List<MarkerClusterBox>();
			foreach (IClusterable point in sourceItems) {
				IMapUnit pointUnitLocation = point.GetUnitLocation();
				List<MarkerClusterBox> suitableClusters = new List<MarkerClusterBox>();
				foreach(MarkerClusterBox cluster in clusters)
					if(cluster.IsPointInBox(pointUnitLocation))
						suitableClusters.Add(cluster);
				if(suitableClusters.Count > 0) {
					MarkerClusterBox clusterBox = GetClosestBoxCenter(suitableClusters, point);
					clusterBox.Cluster.ClusteredItems.Add(point);
				}
				else {
					ClusterCore clusterItem = new ClusterCore() { Location = point.Location, UnitLocation = pointUnitLocation, Owner=point.Owner };
					clusterItem.ClusteredItems.Add(point);
					clusters.Add(new MarkerClusterBox(clusterItem, halfSize));
				}
			}
			return clusters;
		}
	}
	public class DistanceBasedClusterCalculator : ClusterCalculatorBase {
		CoordObjectFactory PointFactory { get; set; }
		public DistanceBasedClusterCalculator(IClusterItemFactoryCore clusterItemFactory, CoordObjectFactory pointFactory)
			: base(clusterItemFactory) {
			Guard.ArgumentNotNull(pointFactory, "PointFactory");
			PointFactory = pointFactory;
		}
		public override ClusterCalculatorResult CalculateGroups(IEnumerable<IClusterable> sourceItems, double step) {
			List<ClusterCore> clusters = new List<ClusterCore>();
			CreateClusters(sourceItems, step, clusters);
			RelocateClusteredItems(sourceItems, clusters);
			int maxItemsCount = int.MinValue;
			int minItemsCount = int.MaxValue;
			foreach(ClusterCore cluster in clusters) {
				cluster.Location = GetAverageCoord(cluster.ClusteredItems);
				if(cluster.ClusteredItems.Count > 1)
					cluster.Location = GetAverageCoord(cluster.ClusteredItems);
				if(cluster.ClusteredItems.Count > maxItemsCount)
					maxItemsCount = cluster.ClusteredItems.Count;
				if(cluster.ClusteredItems.Count < minItemsCount)
					minItemsCount = cluster.ClusteredItems.Count;
			}
			return CreateResults(clusters, maxItemsCount, minItemsCount);
		}
		protected internal CoordPoint GetAverageCoord(IList<IClusterable> cluster) {
			double x = 0, y = 0;
			foreach(IClusterable point in cluster) {
				x += point.Location.GetX();
				y += point.Location.GetY();
			}
			x /= cluster.Count;
			y /= cluster.Count;
			return PointFactory.CreatePoint(x, y);
		}
		void CreateClusters(IEnumerable<IClusterable> sourceItems, double step, List<ClusterCore> clusters) {
			foreach(IClusterable point in sourceItems) {
				IMapUnit pointUnitLocation = point.GetUnitLocation();
				bool itemIsFree = true;
					foreach(ClusterCore group in clusters)
							if(GetDist(pointUnitLocation, group.UnitLocation) <= step) {
									group.ClusteredItems.Add(point);
									itemIsFree = false;
									break;
							}
					if(!itemIsFree)
						continue;
					clusters.Add(new ClusterCore() { Location = point.Location, UnitLocation = pointUnitLocation, Owner=point.Owner });
					clusters.Last().ClusteredItems.Add(point);
			}
		}
		void RelocateClusteredItems(IEnumerable<IClusterable> sourceItems, List<ClusterCore> clusters) {
			foreach(ClusterCore cluster in clusters)
				cluster.ClusteredItems.Clear();
			foreach(IClusterable point in sourceItems) {
				IMapUnit pointUnitLocation = point.GetUnitLocation();
				double sqrDist = double.MaxValue;
				int closestClusterIndex = -1;
				for(int i = 0; i < clusters.Count; i++) {
					double curSqrDist = GetSqrDist(pointUnitLocation, clusters[i].UnitLocation);
					if(curSqrDist <= sqrDist) {
						sqrDist = curSqrDist;
						closestClusterIndex = i;
					}
				}
				clusters[closestClusterIndex].ClusteredItems.Add(point);
			}
		}
	}
}
