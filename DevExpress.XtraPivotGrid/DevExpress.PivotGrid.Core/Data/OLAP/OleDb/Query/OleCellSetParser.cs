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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.OLAP {
	class OleCellSetParser : OLAPCellSetParserBase {
		IDataReader ExecuteReader(IOLAPCommand command) {
			try {
				return ((OleCommand)command).ExecuteReader();
			} catch(OleDbException e) {
				throw new DevExpress.XtraPivotGrid.Data.OleDbErrorResponseException(e);
			}
		}
		interface IOleMemberSource : IOLAPMemberSource {
			bool IsInvalid { get; }
			OLAPMemberProperty GetPropertyValue(OLAPPropertyDescriptor property);
		}
		class OlapMembeEvaluator : IOleMemberSource {
			readonly IDataReader reader;
			readonly int memberNameIndex = -1, memberValueIndex = -1, memberCaptionIndex = -1;
			public OlapMembeEvaluator(string uniqueName, IDataReader reader) {
				this.reader = reader;
				memberNameIndex = reader.GetOrdinal(uniqueName + "." + OLAPDataSourceQueryBase.MemberUniqueNameName);
				memberValueIndex = reader.GetOrdinal(OLAPDataSourceQueryBase.ValueMeasureName);
				memberCaptionIndex = reader.GetOrdinal(OLAPDataSourceQueryBase.CaptionMeasureName);
			}
			public bool IsInvalid {
				get {
					return memberNameIndex < 0 || memberValueIndex < 0;
				}
			}
			public bool Read() {
				return reader.Read();
			}
			string IOLAPMemberSource.UniqueName {
				get { return reader.GetString(memberNameIndex); }
			}
			string IOLAPMemberSource.Caption {
				get { return reader.GetValue(memberCaptionIndex) as string; }
			}
			object IOLAPMemberSource.GetValue(Type dataType) {
				object val = reader.GetValue(memberValueIndex);
				if(val is DBNull)
					val = null;
				return val;
			}
			Dictionary<string, int> indeces = new Dictionary<string, int>();
			public OLAPMemberProperty GetPropertyValue(OLAPPropertyDescriptor property) {
				int index;
				if(!indeces.TryGetValue(property.Name, out index)) {
					try {
						index = reader.GetOrdinal(string.Format(OLAPDataSourceQueryBase.MemberPropertyMeasureFormat, property.Name));
					} catch {
						return null;
					}
					indeces[property.Name] = index;
				}
				if(index == -1)
					return null;
				object val = reader.GetValue(index);
				if(val == null || val is DBNull)
					return null;
				return OLAPMemberProperty.Create(property, val);
			}
		}
		List<OLAPMember> ReadColumnMembersCore(OLAPMetadataColumn meta, OLAPCubeColumn column, IDataReader reader) {
			OlapMembeEvaluator ev;
			try {
				ev = new OlapMembeEvaluator(meta.UniqueName, reader);
			} catch {
				return null;
			}
			if(ev.IsInvalid)
				return null;
			List<OLAPMember> members = new List<OLAPMember>();
			while(ev.Read()) {
				IOleMemberSource source = ev;
				OLAPMember member = meta[source.UniqueName];
				if(member == null)
					member = meta.CreateMemberWithoutProps(source);
				if(column != null) {
					foreach(OLAPPropertyDescriptor property in column.AutoProperties) {
						if(member.autoProperties == null)
							member.autoProperties = new OLAPMemberProperties();
						member.autoProperties[property.Name] = source.GetPropertyValue(property);
					}
				}
				members.Add(member);
			}
			return members;
		}
		OLAPMemberType GetMemberType(object val) {
			string strVal = val as string;
			int intVal = 0;
			if(!string.IsNullOrEmpty(strVal)) {
				if(!Int32.TryParse(strVal, out intVal))
					return OLAPMemberType.Unknown;
			} else {
				if(!(val is int))
					return OLAPMemberType.Unknown;
				else
					intVal = (int)val;
			}
			OLAPMemberType res = (OLAPMemberType)intVal;
			if(!Enum.IsDefined(typeof(OLAPMemberType), res))
				return OLAPMemberType.Unknown;
			return res;
		}
		List<string> ReadColumnMemberNamesCore(OLAPCubeColumn column, IDataReader reader) {
			int memberNameIndex = -1;
			try {
				memberNameIndex = reader.GetOrdinal(column.UniqueName + "." + OLAPDataSourceQueryBase.MemberUniqueNameName);
			} catch(IndexOutOfRangeException) { }
			if(memberNameIndex < 0)
				return null;
			List<string> names = new List<string>();
			while(reader.Read()) {
				string uniqueName = (string)reader.GetValue(memberNameIndex);
				names.Add(uniqueName);
			}
			return names;
		}
		public override CellSet<OLAPCubeColumn> QueryData(IOLAPCommand command, IQueryContext<OLAPCubeColumn> context) {
			using(IDataReader reader = ExecuteReader(command)) {
				if(reader.FieldCount == 0)
					return null;
				return CellSetCreator.CreateCellSet(
						new OLAPCachedCellSet(new ByAreasAxisColumnProvider(context.Areas, (OLAPMetadata)context.Owner.Metadata), new OleCellSet(reader, (IOLAPQueryContext)context)), context, context.Owner);
			}
		}
		public override List<object> QueryVisibleOrAvailableValues(IOLAPCommand command, OLAPCubeColumn olapColumn) {
			using(IDataReader reader = ExecuteReader(command)) {
				List<string> visibleMembers = ReadColumnMemberNamesCore(olapColumn, reader);
				List<object> result = new List<object>(visibleMembers.Count);
				if(olapColumn.OLAPFilterByUniqueName) {
					foreach(string uniqueName in visibleMembers) {
						result.Add(uniqueName);
					}
				} else {
					foreach(string uniqueName in visibleMembers) {
						OLAPMember member = olapColumn[uniqueName];
						if(member != null)
							result.Add(member.Value);
					}
				}
				return result;
			}
		}
		public override IEnumerable<object> QueryValue(IOLAPCommand command) {
			using(IDataReader reader = ExecuteReader(command)) {
				if(reader.Read())
					for(int i = 0; i < reader.FieldCount; i++)
						yield return reader.GetValue(i);
			}
		}
		public override List<OLAPMember> QueryMembers(IOLAPCommand command, OLAPMetadataColumn meta, OLAPCubeColumn childColumn, OLAPChildMember olapMember) {
			using(IDataReader reader = ExecuteReader(command)) {
				List<OLAPMember> childMembers = ReadColumnMembersCore(meta, childColumn, reader);
				if(olapMember != null)
					olapMember.SetChildMembers(childMembers);
				return childMembers;
			}
		}
		public override PivotOLAPKPIValue QueryKPIValue(IOLAPCommand command, PivotOLAPKPIMeasures measures, string kpiName, OLAPMetadata metadata) {
			using(IDataReader reader = ExecuteReader(command)) {
				reader.Read();
				object value = null, goal = null;
				int status = 0, trend = 0;
				double weight = double.NaN;
				for(int i = 0; i < reader.FieldCount; i++) {
					string name = reader.GetName(i);
					object cellValue = reader.GetValue(i);
					InitKpiParameters(measures, name, cellValue,
						ref value, ref goal, ref status, ref trend, ref weight);
				}
				return new PivotOLAPKPIValue(value, goal, status, trend, weight);
			}
		}
		public override IOLAPRowSet QueryDrillDown(IOLAPCommand command) {
			return DataReaderWrapper.Wrap(ExecuteReader(command));
		}
	}
}
