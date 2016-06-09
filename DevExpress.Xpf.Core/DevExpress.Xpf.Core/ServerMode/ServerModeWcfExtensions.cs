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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
#if !SL
using DeflateStream = System.IO.Compression.DeflateStream;
using CompressionMode = System.IO.Compression.CompressionMode;
#else
using DeflateStream = DevExpress.Utils.Zip.DeflateStream;
using CompressionMode = DevExpress.Utils.Zip.CompressionMode;
#endif
namespace DevExpress.Xpf.Core.ServerMode {
	[Serializable]
	public class ServerModeOrderDescriptorSerializationWrapper {
		public CriteriaOperator SortExpression;
		[XmlAttribute]
		public bool IsDesc;
		public ServerModeOrderDescriptorSerializationWrapper() { }
		public ServerModeOrderDescriptorSerializationWrapper(ServerModeOrderDescriptor orderDescriptor) {
			this.SortExpression = orderDescriptor.SortExpression;
			this.IsDesc = orderDescriptor.IsDesc;
		}
		public ServerModeOrderDescriptor ToServerModeOrderDescriptor() {
			return new ServerModeOrderDescriptor(SortExpression, IsDesc);
		}
	}
	[Serializable]
	public class ServerModeSummaryDescriptorSerializationWrapper {
		public CriteriaOperator SummaryExpression;
		[XmlAttribute]
		public Aggregate SummaryType;
		public ServerModeSummaryDescriptorSerializationWrapper() { }
		public ServerModeSummaryDescriptorSerializationWrapper(ServerModeSummaryDescriptor summaryDescriptor) {
			this.SummaryExpression = summaryDescriptor.SummaryExpression;
			this.SummaryType = summaryDescriptor.SummaryType;
		}
		public ServerModeSummaryDescriptor ToServerModeSummaryDescriptor() {
			return new ServerModeSummaryDescriptor(SummaryExpression, SummaryType);
		}
	}
	[Serializable]
	public class ServerModeGroupInfoDataSerializationWrapper {
		public object GroupValue;
		[XmlAttribute]
		public int ChildDataRowCount;
#if !SL
		public ArrayList Summary = new ArrayList();
#else
		public DevExpress.Xpf.Collections.ArrayList Summary = new DevExpress.Xpf.Collections.ArrayList();
#endif
		public ServerModeGroupInfoDataSerializationWrapper() { }
		public ServerModeGroupInfoDataSerializationWrapper(ServerModeGroupInfoData groupInfoData) {
			this.GroupValue = groupInfoData.GroupValue;
			this.ChildDataRowCount = groupInfoData.ChildDataRowCount;
			this.Summary.AddRange(groupInfoData.Summary);
		}
		public ServerModeGroupInfoData ToServerModeGroupInfoData() {
			return new ServerModeGroupInfoData(GroupValue, ChildDataRowCount, Summary);
		}
	}
	public enum ExtendedOperationType { None, FetchKeys, GetCount, GetUniqueValues, PrepareChildren, PrepareTopGroupInfo }
	public static class ExtendedDataHelper {
		public const string ExtendedQuerySuffix = "ExtendedData";
		public const string ExtendedParameterName = "extendedDataInfo";
		public static string Serialize<T>(object obj) {
			if(ReferenceEquals(obj, null)) return String.Empty;
			using(StringWriter writer = new StringWriter()) {
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				serializer.Serialize(writer, obj);
				byte[] encodedXml = Encoding.UTF8.GetBytes(writer.ToString());
				using(MemoryStream inStream = new MemoryStream(encodedXml)) {
					using(MemoryStream outStream = new MemoryStream(encodedXml.Length * 10)) {
						using(DeflateStream zipStream = new DeflateStream(outStream, CompressionMode.Compress, true)) {
							inStream.CopyTo(zipStream);
						}
						return Convert.ToBase64String(outStream.ToArray()).Replace('+', '-').Replace('/', '_');
					}
				}
			}
		}
		public static T Deserialize<T>(string sourceXml) {
			if(String.IsNullOrEmpty(sourceXml)) return default(T);
			byte[] decodedXml = Convert.FromBase64String(sourceXml.Replace('-', '+').Replace('_', '/'));
			using(MemoryStream inStream = new MemoryStream(decodedXml)) {
				using(MemoryStream outStream = new MemoryStream(decodedXml.Length * 10)) {
					using(DeflateStream zipStream = new DeflateStream(inStream, CompressionMode.Decompress, true)) {
						zipStream.CopyTo(outStream);
					}
					byte[] decompressedXml = outStream.ToArray();
					using(StringReader reader = new StringReader(Encoding.UTF8.GetString(decompressedXml, 0, decompressedXml.Length))) {
						XmlSerializer serializer = new XmlSerializer(typeof(T));
						return (T)serializer.Deserialize(reader);
					}
				}
			}
		}
		internal static ServerModeOrderDescriptorSerializationWrapper[] WrapOrder(ServerModeOrderDescriptor[] orderDescriptors) {
			ServerModeOrderDescriptorSerializationWrapper[] result = new ServerModeOrderDescriptorSerializationWrapper[orderDescriptors.Length];
			for(int i = 0; i < orderDescriptors.Length; i++) {
				result[i] = new ServerModeOrderDescriptorSerializationWrapper(orderDescriptors[i]);
			}
			return result;
		}
		internal static ServerModeOrderDescriptor[] UnwrapOrder(ServerModeOrderDescriptorSerializationWrapper[] wrappers) {
			ServerModeOrderDescriptor[] result = new ServerModeOrderDescriptor[wrappers.Length];
			for(int i = 0; i < wrappers.Length; i++) {
				result[i] = wrappers[i].ToServerModeOrderDescriptor();
			}
			return result;
		}
		internal static ServerModeSummaryDescriptorSerializationWrapper[] WrapSummaries(ServerModeSummaryDescriptor[] summaryDescriptors) {
			ServerModeSummaryDescriptorSerializationWrapper[] result = new ServerModeSummaryDescriptorSerializationWrapper[summaryDescriptors.Length];
			for(int i = 0; i < summaryDescriptors.Length; i++) {
				result[i] = new ServerModeSummaryDescriptorSerializationWrapper(summaryDescriptors[i]);
			}
			return result;
		}
		internal static ServerModeSummaryDescriptor[] UnwrapSummaries(ServerModeSummaryDescriptorSerializationWrapper[] wrappers) {
			ServerModeSummaryDescriptor[] result = new ServerModeSummaryDescriptor[wrappers.Length];
			for(int i = 0; i < wrappers.Length; i++) {
				result[i] = wrappers[i].ToServerModeSummaryDescriptor();
			}
			return result;
		}
		internal static ServerModeGroupInfoDataSerializationWrapper[] WrapGroupInfoData(ServerModeGroupInfoData[] groupInfoData) {
			ServerModeGroupInfoDataSerializationWrapper[] result = new ServerModeGroupInfoDataSerializationWrapper[groupInfoData.Length];
			for(int i = 0; i < groupInfoData.Length; i++) {
				result[i] = new ServerModeGroupInfoDataSerializationWrapper(groupInfoData[i]);
			}
			return result;
		}
		internal static ServerModeGroupInfoData[] UnwrapGroupInfoData(ServerModeGroupInfoDataSerializationWrapper[] wrappers) {
			ServerModeGroupInfoData[] result = new ServerModeGroupInfoData[wrappers.Length];
			for(int i = 0; i < wrappers.Length; i++) {
				result[i] = wrappers[i].ToServerModeGroupInfoData();
			}
			return result;
		}
		public static string GetExtendedData(IQueryable queryable, string extendedDataInfo) {
			return GetExtendedData(queryable, new DevExpress.Data.Linq.CriteriaToExpressionConverter(), extendedDataInfo);
		}
		public static string GetExtendedData(IQueryable queryable, DevExpress.Data.Linq.ICriteriaToExpressionConverter converter, string extendedDataInfo) {
			ExtendedDataParametersContainer parametersContainer = Deserialize<ExtendedDataParametersContainer>(extendedDataInfo);
			ExtendedDataResultContainer resultContainer = null;			 
			switch(parametersContainer.OperationType) {
				case ExtendedOperationType.FetchKeys:
					resultContainer = new ExtendedDataResultContainer(ExtendedOperationType.FetchKeys,
						ServerModeCoreExtender.FetchKeys(queryable, converter, parametersContainer.KeysCriteria,
						parametersContainer.Where, UnwrapOrder(parametersContainer.Order), parametersContainer.Skip, parametersContainer.Take));
					break;
				case ExtendedOperationType.GetCount:
					resultContainer = new ExtendedDataResultContainer(
						ServerModeCoreExtender.GetCount(queryable, converter, parametersContainer.Where));
					break;
				case ExtendedOperationType.GetUniqueValues:
					resultContainer = new ExtendedDataResultContainer(ExtendedOperationType.GetUniqueValues,
						ServerModeCoreExtender.GetUniqueValues(queryable, converter, parametersContainer.Expression,
						parametersContainer.MaxCount, parametersContainer.Where));
					break;
				case ExtendedOperationType.PrepareChildren:
					resultContainer = new ExtendedDataResultContainer(
						ServerModeCoreExtender.PrepareChildren(queryable, converter, parametersContainer.GroupWhere,
						parametersContainer.GroupByDescriptor.ToServerModeOrderDescriptor(), UnwrapSummaries(parametersContainer.Summaries)));
					break;
				case ExtendedOperationType.PrepareTopGroupInfo:
					resultContainer = new ExtendedDataResultContainer(
						ServerModeCoreExtender.PrepareTopGroupInfo(queryable, converter, parametersContainer.Where, UnwrapSummaries(parametersContainer.Summaries)));
					break;
				default:
					throw new NotImplementedException(parametersContainer.OperationType.ToString() + " is not implemented");
			}
			return Serialize<ExtendedDataResultContainer>(resultContainer);
		}
	}
	[Serializable]
	public class ExtendedDataParametersContainer {
		[XmlAttribute]
		public ExtendedOperationType OperationType = ExtendedOperationType.None;
		public CriteriaOperator[] KeysCriteria;
		public CriteriaOperator Expression;
		public CriteriaOperator Where;
		public CriteriaOperator GroupWhere;
		public ServerModeOrderDescriptorSerializationWrapper GroupByDescriptor;
		public ServerModeOrderDescriptorSerializationWrapper[] Order;
		public ServerModeSummaryDescriptorSerializationWrapper[] Summaries;
		[XmlAttribute]
		public int Skip;
		[XmlAttribute]
		public int Take;
		[XmlAttribute]
		public int MaxCount;
		public ExtendedDataParametersContainer() { }
		public ExtendedDataParametersContainer(CriteriaOperator[] keysCriteria, CriteriaOperator where, ServerModeOrderDescriptor[] order, int skip, int take) {
			this.OperationType = ExtendedOperationType.FetchKeys;
			this.KeysCriteria = keysCriteria;
			this.Where = where;
			this.Order = ExtendedDataHelper.WrapOrder(order);
			this.Skip = skip;
			this.Take = take;
		}
		public ExtendedDataParametersContainer(CriteriaOperator where) {
			this.OperationType = ExtendedOperationType.GetCount;
			this.Where = where;
		}
		public ExtendedDataParametersContainer(CriteriaOperator expression, int maxCount, CriteriaOperator where) {
			this.OperationType = ExtendedOperationType.GetUniqueValues;
			this.Expression = expression;
			this.MaxCount = maxCount;
			this.Where = where;
		}
		public ExtendedDataParametersContainer(CriteriaOperator groupWhere, ServerModeOrderDescriptor groupByDescriptor, ServerModeSummaryDescriptor[] summaries) {
			this.OperationType = ExtendedOperationType.PrepareChildren;
			this.GroupWhere = groupWhere;
			this.GroupByDescriptor = new ServerModeOrderDescriptorSerializationWrapper(groupByDescriptor);
			this.Summaries = ExtendedDataHelper.WrapSummaries(summaries);
		}
		public ExtendedDataParametersContainer(CriteriaOperator where, ServerModeSummaryDescriptor[] summaries) {
			this.OperationType = ExtendedOperationType.PrepareTopGroupInfo;
			this.Where = where;
			this.Summaries = ExtendedDataHelper.WrapSummaries(summaries);
		}
	}
	[Serializable]
	public class ExtendedDataResultContainer {
		[XmlAttribute]
		public ExtendedOperationType OperationType = ExtendedOperationType.None;
		public object[] KeysOrUniqueValues;
		[XmlAttribute]
		public int Count;
		public ServerModeGroupInfoDataSerializationWrapper[] Children;
		public ServerModeGroupInfoDataSerializationWrapper TopGroupInfo;
		public ExtendedDataResultContainer() { }
		public ExtendedDataResultContainer(ExtendedOperationType operationType, object[] keysOrUniqueValues) {
			this.OperationType = operationType;
			this.KeysOrUniqueValues = keysOrUniqueValues;
		}
		public ExtendedDataResultContainer(int count) {
			this.OperationType = ExtendedOperationType.GetCount;
			this.Count = count;
		}
		public ExtendedDataResultContainer(ServerModeGroupInfoData[] children) {
			this.OperationType = ExtendedOperationType.PrepareChildren;
			this.Children = ExtendedDataHelper.WrapGroupInfoData(children);
		}
		public ExtendedDataResultContainer(ServerModeGroupInfoData topGroupInfo) {
			this.TopGroupInfo = new ServerModeGroupInfoDataSerializationWrapper(topGroupInfo);
		}
		public object[] GetKeys() {
			return KeysOrUniqueValues;
		}
		public int GetCount() {
			return Count;
		}
		public object[] GetUniqueValues() {
			return KeysOrUniqueValues;
		}
		public ServerModeGroupInfoData[] GetChildren() {
			return ExtendedDataHelper.UnwrapGroupInfoData(Children);
		}
		public ServerModeGroupInfoData GetTopGroupInfo() {
			return TopGroupInfo.ToServerModeGroupInfoData();
		}
	}
}
