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

using DevExpress.DashboardCommon.DataProcessing;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.Native {
	public class ClusterizedMapDataEngine {
		readonly List<Dimension> dimensions = new List<Dimension>();
		readonly List<Measure> measures = new List<Measure>();
		readonly Dimension latitude;
		readonly Dimension longitude;
		readonly Dimension pieArgument;
		readonly Measure count;
		ClusterizedMapGeoPointData sourceData;
		HierarchicalDataParams hData;
		List<Dimension> sliceDimensions;
		Dictionary<DataItem, string> listDataMembers = new Dictionary<DataItem, string>();
		IEnumerable<Dimension> CoordinateDimensions {
			get {
				yield return latitude;
				yield return longitude;
			}
		}
		IEnumerable<Dimension> MainDimensions {
			get {
				if(pieArgument != null)
					return CoordinateDimensions.Concat(new Dimension[] { pieArgument });
				else
					return CoordinateDimensions;
			}
		}
		public ClusterizedMapDataEngine(ClusterizedMapGeoPointData data, InternalMapDataMembersContainer clientDataDataMembers) {
			latitude = new Dimension(clientDataDataMembers.Latitude.DataMember);
			listDataMembers.Add(latitude, clientDataDataMembers.Latitude.Id);
			longitude = new Dimension(clientDataDataMembers.Longitude.DataMember);
			listDataMembers.Add(longitude, clientDataDataMembers.Longitude.Id);
			if(clientDataDataMembers.PieArgument != null) {
				pieArgument = new Dimension(clientDataDataMembers.PieArgument.DataMember);
				listDataMembers.Add(pieArgument, clientDataDataMembers.PieArgument.Id);
			}
			foreach(InternalMapDataMemberContainer pair in clientDataDataMembers.Dimensions) {
				Dimension dim = new Dimension(pair.DataMember);
				dimensions.Add(dim);
				listDataMembers.Add(dim, pair.Id);
			}
			foreach(InternalMapDataMemberContainer pair in clientDataDataMembers.Measures) {
				Measure mea = new Measure(pair.DataMember);
				measures.Add(mea);
				listDataMembers.Add(mea, pair.Id);
			}
			count = new Measure(clientDataDataMembers.PointsCount.DataMember) { SummaryType = SummaryType.Sum };
			listDataMembers.Add(count, clientDataDataMembers.PointsCount.Id);
			sourceData = data;
		}
		public HierarchicalDataParams GetHierarchicalDataParams() {
			return hData;
		}
		public void Calculate() {
			sliceDimensions = MainDimensions.Concat(dimensions).ToList();
#if !DXPORTABLE
			DashboardObjectDataSource ds = new DashboardObjectDataSource(sourceData);
			IExternalSchemaConsumer cons = ds;
			cons.SetSchema (null,sliceDimensions.Select(d => d.DataMember).Concat(MainDimensions.Select(d => d.DataMember)).Append(count.DataMember).Concat(measures.Select(m => m.DataMember)).NotNull().Distinct().ToArray());
#else
			DashboardSqlDataSource ds = new DashboardSqlDataSource();
#endif
			DataSourceInfo dataSourceInfo = new DataSourceInfo(ds, null);
			DataSourceModel model = new DataSourceModel(dataSourceInfo, sourceData, null);
			IDataSession session = DataSessionFactory.Default.RequestSession(model);
			Dictionary<DataItem, string> ids = new Dictionary<DataItem, string>();
			ItemModelBuilder builder = new ItemModelBuilder(dataSourceInfo, (a) => {
				ids[a] = listDataMembers[a];
				return listDataMembers[a];
			}, new EmptyParametersProvider());
			SliceDataQueryBuilder queryBuilder = SliceDataQueryBuilder.CreateEmpty(builder, new Dimension[0], null);
			queryBuilder.AddSlice(sliceDimensions, measures);
			queryBuilder.AddSlice(CoordinateDimensions, new[] { count });
			queryBuilder.AddSlice(MainDimensions, measures); 
			queryBuilder.SetAxes(sliceDimensions, new Dimension[0]);
			hData = session.GetData(queryBuilder.FinalQuery()).HierarchicalData;
			DataStorage storage = hData.Storage;
		}
	}
}
