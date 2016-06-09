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
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Native {
	public class InternalMapDataMemberContainer {
		public string Id { get; private set; }
		public string DataMember { get; private set; }
		public Type DataType { get; private set; }
		public InternalMapDataMemberContainer(string id, string dataMember, Type dataType) {
			this.Id = id;
			this.DataMember = dataMember;
			this.DataType = dataType;
		}
	}
	public class InternalMapDataMembersContainer {
		const string Prefix = "DataId";
		readonly PrefixNameGenerator nameGenerator = new PrefixNameGenerator(Prefix);
		public InternalMapDataMemberContainer Latitude { get; private set; }
		public InternalMapDataMemberContainer Longitude { get; private set; }
		public InternalMapDataMemberContainer PieArgument { get; private set; }
		public InternalMapDataMemberContainer PointsCount { get; private set; }
		public List<InternalMapDataMemberContainer> Dimensions { get; private set; }
		public List<InternalMapDataMemberContainer> Measures { get; private set; }
		public InternalMapDataMembersContainer() {
			Dimensions = new List<InternalMapDataMemberContainer>();
			Measures = new List<InternalMapDataMemberContainer>();
		}
		public void AddLatitude(string id) {
			Latitude = new InternalMapDataMemberContainer(id, GenerateName(), typeof(double));
		}
		public void AddLongitude(string id) {
			Longitude = new InternalMapDataMemberContainer(id, GenerateName(), typeof(double));
		}
		public void AddPieArgument(string id, Type dataType) {
			PieArgument = new InternalMapDataMemberContainer(id, GenerateName(), MakeNullable(dataType));
		}
		public void AddDimension(string id, Type dataType) {
			Dimensions.Add(new InternalMapDataMemberContainer(id, GenerateName(), MakeNullable(dataType)));
		}
		public void AddMeasure(string id) {
			Measures.Add(new InternalMapDataMemberContainer(id, GenerateName(), typeof(object)));
		}
		public void AddPointsCount(string id) {
			PointsCount = new InternalMapDataMemberContainer(id, GenerateName(), typeof(int));
		}
		string GenerateName() {
			return nameGenerator.GenerateName();
		}
		Type MakeNullable(Type dataType) {
			if(dataType.IsClass() || Nullable.GetUnderlyingType(dataType) != null || dataType.IsInterface())
				return dataType;
			return typeof(Nullable<>).MakeGenericType(dataType);
		}
	}
}
