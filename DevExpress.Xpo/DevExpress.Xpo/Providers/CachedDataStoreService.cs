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

#if !CF
using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using System.Configuration;
using System.Xml.Serialization;
using DevExpress.Xpo.DB.Exceptions;
using System.ServiceModel.Channels;
#if !SL
using System.ServiceModel.Activation;
#endif
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
using System.Threading;
namespace DevExpress.Xpo.DB {
	[ServiceContract, XmlSerializerFormat]
	public interface ICachedDataStoreService: IDataStoreService {
		[OperationContract(Action = "http://tempuri.org/ICachedDataStoreService/ModifyDataCached")]
		[ServiceKnownType(typeof(DeleteStatement))]
		[ServiceKnownType(typeof(InsertStatement))]
		[ServiceKnownType(typeof(UpdateStatement))]
		[ServiceKnownType(typeof(AggregateOperand))]
		[ServiceKnownType(typeof(BetweenOperator))]
		[ServiceKnownType(typeof(BinaryOperator))]
		[ServiceKnownType(typeof(ContainsOperator))]
		[ServiceKnownType(typeof(FunctionOperator))]
		[ServiceKnownType(typeof(GroupOperator))]
		[ServiceKnownType(typeof(InOperator))]
		[ServiceKnownType(typeof(NotOperator))]
		[ServiceKnownType(typeof(NullOperator))]
		[ServiceKnownType(typeof(OperandProperty))]
		[ServiceKnownType(typeof(OperandValue))]
		[ServiceKnownType(typeof(ParameterValue))]
		[ServiceKnownType(typeof(QueryOperand))]
		[ServiceKnownType(typeof(UnaryOperator))]
		[ServiceKnownType(typeof(JoinOperand))]
		[ServiceKnownType(typeof(OperandParameter))]
		[ServiceKnownType(typeof(QuerySubQueryContainer))]
		OperationResult<DataCacheModificationResult> ModifyDataCached(DataCacheCookie cookie, ModificationStatement[] dmlStatements);
		[OperationContract(Action = "http://tempuri.org/ICachedDataStoreService/NotifyDirtyTables")]
		OperationResult<DataCacheResult> NotifyDirtyTables(DataCacheCookie cookie, params string[] dirtyTablesNames);
		[OperationContract(Action = "http://tempuri.org/ICachedDataStoreService/ProcessCookie")]
		OperationResult<DataCacheResult> ProcessCookie(DataCacheCookie cookie);
		[OperationContract(Action = "http://tempuri.org/ICachedDataStoreService/SelectDataCached")]
		[ServiceKnownType(typeof(AggregateOperand))]
		[ServiceKnownType(typeof(BetweenOperator))]
		[ServiceKnownType(typeof(BinaryOperator))]
		[ServiceKnownType(typeof(ContainsOperator))]
		[ServiceKnownType(typeof(FunctionOperator))]
		[ServiceKnownType(typeof(GroupOperator))]
		[ServiceKnownType(typeof(InOperator))]
		[ServiceKnownType(typeof(NotOperator))]
		[ServiceKnownType(typeof(NullOperator))]
		[ServiceKnownType(typeof(OperandProperty))]
		[ServiceKnownType(typeof(OperandValue))]
		[ServiceKnownType(typeof(ParameterValue))]
		[ServiceKnownType(typeof(QueryOperand))]
		[ServiceKnownType(typeof(UnaryOperator))]
		[ServiceKnownType(typeof(JoinOperand))]
		[ServiceKnownType(typeof(OperandParameter))]
		[ServiceKnownType(typeof(QuerySubQueryContainer))]
		OperationResult<DataCacheSelectDataResult> SelectDataCached(DataCacheCookie cookie, SelectStatement[] selects);
		[OperationContract(Action = "http://tempuri.org/ICachedDataStoreService/UpdateSchemaCached")]
		OperationResult<DataCacheUpdateSchemaResult> UpdateSchemaCached(DataCacheCookie cookie, DBTable[] tables, bool dontCreateIfFirstTableNotExist);
	}
#if !SL
	[ServiceContract, XmlSerializerFormat]
	public interface ICachedDataStoreWarpService : ICachedDataStoreService, IDataStoreWarpService {
		[OperationContract(Action = "http://tempuri.org/ICachedDataStoreService/WarpSelectDataCached")]
		[ServiceKnownType(typeof(AggregateOperand))]
		[ServiceKnownType(typeof(BetweenOperator))]
		[ServiceKnownType(typeof(BinaryOperator))]
		[ServiceKnownType(typeof(ContainsOperator))]
		[ServiceKnownType(typeof(FunctionOperator))]
		[ServiceKnownType(typeof(GroupOperator))]
		[ServiceKnownType(typeof(InOperator))]
		[ServiceKnownType(typeof(NotOperator))]
		[ServiceKnownType(typeof(NullOperator))]
		[ServiceKnownType(typeof(OperandProperty))]
		[ServiceKnownType(typeof(OperandValue))]
		[ServiceKnownType(typeof(ParameterValue))]
		[ServiceKnownType(typeof(QueryOperand))]
		[ServiceKnownType(typeof(UnaryOperator))]
		[ServiceKnownType(typeof(JoinOperand))]
		[ServiceKnownType(typeof(OperandParameter))]
		[ServiceKnownType(typeof(QuerySubQueryContainer))]
		OperationResult<DataCacheWarpSelectDataResult> WarpSelectDataCached(DataCacheCookie cookie, SelectStatement[] selects);
	}
	public class DataCacheWarpSelectDataResult : DataCacheResult {
		public byte[] SelectResult;
		public DataCacheCookie SelectingCookie;
		public DataCacheWarpSelectDataResult() : base() { }
		public DataCacheWarpSelectDataResult(DataCacheSelectDataResult selectedData)
			: this() {
			SelectResult = WcfUsedAsDumbPipeHelper.Warp(selectedData.SelectedData == null ? null : selectedData.SelectedData.ResultSet);
			SelectingCookie = selectedData.SelectingCookie;
			CacheConfig = selectedData.CacheConfig;
			Cookie = selectedData.Cookie;
			UpdatedTableAges = selectedData.UpdatedTableAges;
		}
		public DataCacheSelectDataResult GetResult() {
			DataCacheSelectDataResult result = new DataCacheSelectDataResult();
			result.SelectedData = new SelectedData(WcfUsedAsDumbPipeHelper.Unwarp(SelectResult));
			result.SelectingCookie = SelectingCookie;
			result.CacheConfig = CacheConfig;
			result.Cookie = Cookie;
			result.UpdatedTableAges = UpdatedTableAges;
			return result;
		}
	}
#endif
#if SL
	[ServiceContract, XmlSerializerFormat]
	public interface ICachedDataStoreClientAsync : IDataStoreClientAsync {
		[XmlSerializerFormat, OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ICachedDataStoreService/ModifyDataCached", ReplyAction = "*")]
		IAsyncResult BeginModifyDataCached(DataCacheCookie cookie, ModificationStatement[] dmlStatements, AsyncCallback callback, object asyncState);
		OperationResult<DataCacheModificationResult> EndModifyDataCached(IAsyncResult result);
		[XmlSerializerFormat, OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ICachedDataStoreService/NotifyDirtyTables", ReplyAction = "*")]
		IAsyncResult BeginNotifyDirtyTables(DataCacheCookie cookie, string[] dirtyTablesNames, AsyncCallback callback, object asyncState);
		OperationResult<DataCacheResult> EndNotifyDirtyTables(IAsyncResult result);
		[XmlSerializerFormat, OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ICachedDataStoreService/ProcessCookie", ReplyAction = "*")]
		IAsyncResult BeginProcessCookie(DataCacheCookie cookie, AsyncCallback callback, object asyncState);
		OperationResult<DataCacheResult> EndProcessCookie(IAsyncResult result);
		[XmlSerializerFormat, OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ICachedDataStoreService/SelectDataCached", ReplyAction = "*")]
		IAsyncResult BeginSelectDataCached(DataCacheCookie cookie, SelectStatement[] selects, AsyncCallback callback, object asyncState);
		OperationResult<DataCacheSelectDataResult> EndSelectDataCached(IAsyncResult result);
		[XmlSerializerFormat, OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ICachedDataStoreService/UpdateSchemaCached", ReplyAction = "*")]
		IAsyncResult BeginUpdateSchemaCached(DataCacheCookie cookie, DBTable[] tables, bool dontCreateIfFirstTableNotExist, AsyncCallback callback, object asyncState);
		OperationResult<DataCacheUpdateSchemaResult> EndUpdateSchemaCached(IAsyncResult result);
	}
#endif
#if !SL
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
#endif
	public class CachedDataStoreService : DataStoreService, 
#if SL
		ICachedDataStoreService {
#else
		ICachedDataStoreWarpService {
#endif
		ICachedDataStore cachedProvider;
		public CachedDataStoreService(ICachedDataStore provider)
			: base(provider) {
				cachedProvider = provider;
		}
		public virtual OperationResult<DataCacheModificationResult> ModifyDataCached(DataCacheCookie cookie, ModificationStatement[] dmlStatements) {
			return Execute<DataCacheModificationResult>(delegate() { return cachedProvider.ModifyData(cookie, dmlStatements); });
		}
		public virtual OperationResult<DataCacheResult> NotifyDirtyTables(DataCacheCookie cookie, params string[] dirtyTablesNames) {
			return Execute<DataCacheResult>(delegate() { return cachedProvider.NotifyDirtyTables(cookie, dirtyTablesNames); });
		}
		public virtual OperationResult<DataCacheResult> ProcessCookie(DataCacheCookie cookie) {
			return Execute<DataCacheResult>(delegate() { return cachedProvider.ProcessCookie(cookie); });
		}
		public virtual OperationResult<DataCacheSelectDataResult> SelectDataCached(DataCacheCookie cookie, SelectStatement[] selects) {
			return Execute<DataCacheSelectDataResult>(delegate() { return cachedProvider.SelectData(cookie, selects); });
		}
		public virtual OperationResult<DataCacheUpdateSchemaResult> UpdateSchemaCached(DataCacheCookie cookie, DBTable[] tables, bool dontCreateIfFirstTableNotExist) {
			return Execute<DataCacheUpdateSchemaResult>(delegate() { return cachedProvider.UpdateSchema(cookie, tables, dontCreateIfFirstTableNotExist); });
		}
#if !SL
		public virtual OperationResult<DataCacheWarpSelectDataResult> WarpSelectDataCached(DataCacheCookie cookie, SelectStatement[] selects) {
			return Execute<DataCacheWarpSelectDataResult>(delegate() {  return new DataCacheWarpSelectDataResult(cachedProvider.SelectData(cookie, selects)); });
		}
#endif
	}
	public class CachedDataStoreClient :
#if SL
		CachedDataStoreClientBase<ICachedDataStoreClientAsync> 
#else
		CachedDataStoreClientBase<ICachedDataStoreWarpService>
#endif
	{
		public CachedDataStoreClient(string confName) : base(confName) { }
		public CachedDataStoreClient(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress) { }
	}
	public class CachedDataStoreClientBase<TContractType> : DataStoreClientBase<TContractType>, ICachedDataStore, ICommandChannel
#if SL
		where TContractType: class, ICachedDataStoreClientAsync
#else
		where TContractType: class, ICachedDataStoreWarpService
#endif
   {
		public CachedDataStoreClientBase(string confName) : base(confName) { }
		public CachedDataStoreClientBase(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress) { }
#if SL
		protected override TContractType CreateChannel() {
			return (TContractType)(object)new CachedDataStoreClientChannel(this);
		}
		DataCacheModificationResult ICacheToCacheCommunicationCore.ModifyData(DataCacheCookie cookie, ModificationStatement[] dmlStatements) {
			return OperationResult.ExecuteClient<DataCacheModificationResult>(() => {
				IAsyncResult res = Channel.BeginModifyDataCached(cookie, dmlStatements, null, null);
				return Channel.EndModifyDataCached(res);
			}).HandleError();
		}
		DataCacheResult ICacheToCacheCommunicationCore.NotifyDirtyTables(DataCacheCookie cookie, params string[] dirtyTablesNames) {
			return OperationResult.ExecuteClient<DataCacheResult>(() => {
				IAsyncResult res = Channel.BeginNotifyDirtyTables(cookie, dirtyTablesNames, null, null);
				return Channel.EndNotifyDirtyTables(res);
			}).HandleError();
		}
		DataCacheResult ICacheToCacheCommunicationCore.ProcessCookie(DataCacheCookie cookie) {
			return OperationResult.ExecuteClient<DataCacheResult>(() => {
				IAsyncResult res = Channel.BeginProcessCookie(cookie, null, null);
				return Channel.EndProcessCookie(res);
			}).HandleError();
		}
		DataCacheSelectDataResult ICacheToCacheCommunicationCore.SelectData(DataCacheCookie cookie, SelectStatement[] selects) {
			return OperationResult.ExecuteClient<DataCacheSelectDataResult>(() => {
				IAsyncResult res = Channel.BeginSelectDataCached(cookie, selects, null, null);
				return Channel.EndSelectDataCached(res);
			}).HandleError();
		}
		DataCacheUpdateSchemaResult ICacheToCacheCommunicationCore.UpdateSchema(DataCacheCookie cookie, DBTable[] tables, bool dontCreateIfFirstTableNotExist) {
			return OperationResult.ExecuteClient<DataCacheUpdateSchemaResult>(() => {
				IAsyncResult res = Channel.BeginUpdateSchemaCached(cookie, tables, dontCreateIfFirstTableNotExist, null, null);
				return Channel.EndUpdateSchemaCached(res);
			}).HandleError();
		}
		object ICommandChannel.Do(string command, object args) {
			return OperationResult.ExecuteClient<object>(() => {
				IAsyncResult res = Channel.BeginDo(command, args, null, null);
				return Channel.EndDo(res);
			}).HandleError();
		}
		class CachedDataStoreClientChannel : CachedDataStoreClientChannelBase<TContractType>, ICachedDataStoreClientAsync {
			public CachedDataStoreClientChannel(ClientBase<TContractType> client)
				: base(client) {
			}
		}
		protected class CachedDataStoreClientChannelBase<TAsyncContractType> : DataStoreClientChannelBase<TAsyncContractType>, ICachedDataStoreClientAsync
			where TAsyncContractType : class, ICachedDataStoreClientAsync
		{
			public CachedDataStoreClientChannelBase(ClientBase<TAsyncContractType> client)
				: base(client) {
			}
			public IAsyncResult BeginModifyDataCached(DataCacheCookie cookie, ModificationStatement[] dmlStatements, AsyncCallback callback, object asyncState) {
				return BeginInvoke("ModifyDataCached", new object[] { cookie, dmlStatements }, callback, asyncState);
			}
			public IAsyncResult BeginNotifyDirtyTables(DataCacheCookie cookie, string[] dirtyTablesNames, AsyncCallback callback, object asyncState) {
				return BeginInvoke("NotifyDirtyTables", new object[] { cookie, dirtyTablesNames }, callback, asyncState);
			}
			public IAsyncResult BeginProcessCookie(DataCacheCookie cookie, AsyncCallback callback, object asyncState) {
				return BeginInvoke("ProcessCookie", new object[] { cookie }, callback, asyncState);
			}
			public IAsyncResult BeginSelectDataCached(DataCacheCookie cookie, SelectStatement[] selects, AsyncCallback callback, object asyncState) {
				return BeginInvoke("SelectDataCached", new object[] { cookie, selects }, callback, asyncState);
			}
			public IAsyncResult BeginUpdateSchemaCached(DataCacheCookie cookie, DBTable[] tables, bool dontCreateIfFirstTableNotExist, AsyncCallback callback, object asyncState) {
				return BeginInvoke("UpdateSchemaCached", new object[] { cookie, tables, dontCreateIfFirstTableNotExist }, callback, asyncState);
			}
			static object[] emptyArray = new object[0];
			public OperationResult<DataCacheModificationResult> EndModifyDataCached(IAsyncResult result) {
				return (OperationResult<DataCacheModificationResult>)EndInvoke("ModifyDataCached", emptyArray, result);
			}
			public OperationResult<DataCacheResult> EndNotifyDirtyTables(IAsyncResult result) {
				return (OperationResult<DataCacheResult>)EndInvoke("ModifyDataCached", emptyArray, result);
			}
			public OperationResult<DataCacheResult> EndProcessCookie(IAsyncResult result) {
				return (OperationResult<DataCacheResult>)EndInvoke("ProcessCookie", emptyArray, result);
			}
			public OperationResult<DataCacheSelectDataResult> EndSelectDataCached(IAsyncResult result) {
				return (OperationResult<DataCacheSelectDataResult>)EndInvoke("SelectDataCached", emptyArray, result);
			}
			public OperationResult<DataCacheUpdateSchemaResult> EndUpdateSchemaCached(IAsyncResult result) {
				return (OperationResult<DataCacheUpdateSchemaResult>)EndInvoke("UpdateSchemaCached", emptyArray, result);
			}
		}
#else
		bool selectDataRawFound;
		bool selectDataRawNotFound;
		DataCacheModificationResult ICacheToCacheCommunicationCore.ModifyData(DataCacheCookie cookie, ModificationStatement[] dmlStatements) {
			return ExecuteClient<DataCacheModificationResult>(delegate() { return Channel.ModifyDataCached(cookie, dmlStatements); }).HandleError();
		}
		DataCacheResult ICacheToCacheCommunicationCore.NotifyDirtyTables(DataCacheCookie cookie, params string[] dirtyTablesNames) {
			return ExecuteClient<DataCacheResult>(delegate() { return Channel.NotifyDirtyTables(cookie, dirtyTablesNames); }).HandleError();
		}
		DataCacheResult ICacheToCacheCommunicationCore.ProcessCookie(DataCacheCookie cookie) {
			return ExecuteClient<DataCacheResult>(delegate() { return Channel.ProcessCookie(cookie); }).HandleError();
		}
		DataCacheSelectDataResult ICacheToCacheCommunicationCore.SelectData(DataCacheCookie cookie, SelectStatement[] selects) {
			if(!selectDataRawNotFound) {
				try {
					DataCacheSelectDataResult selecteData = ExecuteClient<DataCacheWarpSelectDataResult>(delegate() { return ((ICachedDataStoreWarpService)Channel).WarpSelectDataCached(cookie, selects); }).HandleError().GetResult();
					selectDataRawFound = true;
					return selecteData;
				} catch(Exception) {
					if(selectDataRawFound) throw;
					selectDataRawNotFound = true;
				}
			}
			return ExecuteClient<DataCacheSelectDataResult>(delegate() { return Channel.SelectDataCached(cookie, selects); }).HandleError();
		}
		DataCacheUpdateSchemaResult ICacheToCacheCommunicationCore.UpdateSchema(DataCacheCookie cookie, DBTable[] tables, bool dontCreateIfFirstTableNotExist) {
			return ExecuteClient<DataCacheUpdateSchemaResult>(delegate() { return Channel.UpdateSchemaCached(cookie, tables, dontCreateIfFirstTableNotExist); }).HandleError();
		}
#endif
	}
}
#if !SL
namespace DevExpress.Xpo.Helpers {
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Runtime.Serialization.Formatters.Binary;
	public static class WcfUsedAsDumbPipeHelper {
		const string UnpackedSignature = "Xpo.SSRs.0.Un:";
		const string GzPackedSignature = "Xpo.SSRs.0.Gz:";
		public static byte[] Warp(SelectStatementResult[] resultSet) {
			if(GzPackedSignature.Length != UnpackedSignature.Length)
				throw new ArgumentException("GzPackedSignature.Length != UnpackedSignature.Length");
			byte[] unpackedBytesWithASig;
			using(MemoryStream ms = new MemoryStream()) {
				foreach(char ch in UnpackedSignature)
					ms.WriteByte((byte)ch);
				SelectResultsToStream(ms, resultSet);
				unpackedBytesWithASig = ms.ToArray();
			}
			if(CanTryPackWhole(unpackedBytesWithASig)) {
				using(MemoryStream packed = new MemoryStream()) {
					foreach(char ch in GzPackedSignature)
						packed.WriteByte((byte)ch);
					using(GZipStream ps = new GZipStream(packed, CompressionMode.Compress, true)) {
						ps.Write(unpackedBytesWithASig, UnpackedSignature.Length, unpackedBytesWithASig.Length - UnpackedSignature.Length);
						ps.Flush();
					}
					if(packed.Length < unpackedBytesWithASig.Length * 0.8)
						return packed.ToArray();
				}
			}
			return unpackedBytesWithASig;
		}
		public static SelectStatementResult[] Unwarp(byte[] warped) {
			if(GzPackedSignature.Length != UnpackedSignature.Length)
				throw new ArgumentException("GzPackedSignature.Length != UnpackedSignature.Length");
			using(MemoryStream ms = new MemoryStream(warped)) {
				System.Text.StringBuilder sigBuilder = new System.Text.StringBuilder();
				for(int i = 0; i < UnpackedSignature.Length; ++i) {
					sigBuilder.Append((char)ms.ReadByte());
				}
				string signature = sigBuilder.ToString();
				switch(signature) {
					case UnpackedSignature:
						return SelectResultsFromStream(ms);
					case GzPackedSignature:
						using(GZipStream ps = new GZipStream(ms, CompressionMode.Decompress, true)) {
							return SelectResultsFromStream(ps);
						}
					default:
						throw new InvalidOperationException("Unexpected Signature: " + signature);
				}
			}
		}
		static bool CanTryPackWhole(byte[] load) {
			if(load == null)
				throw new ArgumentNullException("load");
			if(load.Length < 4096)
				return false;
			if(load.Length < 1024 * 1024)
				return true;
			using(MemoryStream sample = new MemoryStream()) {
				using(GZipStream ps = new GZipStream(sample, CompressionMode.Compress, true)) {
					ps.Write(load, load.Length / 2 - 64 * 1024, 128 * 1024);
					ps.Flush();
				}
				const int Threshold = 128 * 1024 * 70 / 100;
				return sample.Length < Threshold;
			}
		}
		static void SelectResultsToStream(Stream stream, SelectStatementResult[] resultSet) {
			BinaryWriter wr = new BinaryWriter(stream);
			wr.Write((Int32)resultSet.Length);
			foreach(SelectStatementResult result in resultSet) {
				wr.Write((Int32)result.Rows.Length);
				foreach(SelectStatementResultRow row in result.Rows) {
					wr.Write((Int32)row.Values.Length);
					foreach(object value in row.Values) {
						WriteObject(wr, value);
					}
				}
			}
		}
		static SelectStatementResult[] SelectResultsFromStream(Stream stream) {
			BinaryReader rd = new BinaryReader(stream);
			int resultsCount = rd.ReadInt32();
			SelectStatementResult[] resultSet = new SelectStatementResult[resultsCount];
			for(int i = 0; i < resultsCount; ++i) {
				int rowsCount = rd.ReadInt32();
				SelectStatementResultRow[] rows = new SelectStatementResultRow[rowsCount];
				for(int r = 0; r < rowsCount; ++r) {
					int valuesCount = rd.ReadInt32();
					object[] values = new object[valuesCount];
					for(int v = 0; v < valuesCount; ++v) {
						values[v] = ReadObject(rd);
					}
					rows[r] = new SelectStatementResultRow(values);
				}
				resultSet[i] = new SelectStatementResult(rows);
			}
			return resultSet;
		}
		static void WriteObject(BinaryWriter wr, object value) {
			if(value == null) {
				wr.Write((byte)TypeCode.Empty);
			} else {
				Type t = value.GetType();
				sbyte code = (sbyte)Type.GetTypeCode(t);
				switch(code) {
					case (sbyte)TypeCode.Boolean:
						wr.Write(code);
						wr.Write((Boolean)value);
						break;
					case (sbyte)TypeCode.Char:
						wr.Write(code);
						wr.Write((Char)value);
						break;
					case (sbyte)TypeCode.SByte:
						wr.Write(code);
						wr.Write((SByte)value);
						break;
					case (sbyte)TypeCode.Byte:
						wr.Write(code);
						wr.Write((Byte)value);
						break;
					case (sbyte)TypeCode.Int16:
						wr.Write(code);
						wr.Write((Int16)value);
						break;
					case (sbyte)TypeCode.UInt16:
						wr.Write(code);
						wr.Write((UInt16)value);
						break;
					case (sbyte)TypeCode.Int32:
						wr.Write(code);
						wr.Write((Int32)value);
						break;
					case (sbyte)TypeCode.UInt32:
						wr.Write(code);
						wr.Write((UInt32)value);
						break;
					case (sbyte)TypeCode.Int64:
						wr.Write(code);
						wr.Write((Int64)value);
						break;
					case (sbyte)TypeCode.UInt64:
						wr.Write(code);
						wr.Write((UInt64)value);
						break;
					case (sbyte)TypeCode.Single:
						wr.Write(code);
						wr.Write((Single)value);
						break;
					case (sbyte)TypeCode.Double:
						wr.Write(code);
						wr.Write((Double)value);
						break;
					case (sbyte)TypeCode.Decimal:
						wr.Write(code);
						wr.Write((Decimal)value);
						break;
					case (sbyte)TypeCode.DateTime:
						wr.Write(code);
						wr.Write((Int64)((DateTime)value).ToBinary());
						break;
					case (sbyte)TypeCode.String:
						wr.Write(code);
						wr.Write((String)value);
						break;
					default:
						if(t == typeof(byte[])) {
							wr.Write((sbyte)-1);
							byte[] array = (byte[])value;
							wr.Write((Int32)array.Length);
							wr.Write(array);
						} else if(t == typeof(Guid)) {
							wr.Write((sbyte)-2);
							Guid guid = (Guid)value;
							wr.Write(guid.ToByteArray(), 0, 16);
						} else if(t == typeof(TimeSpan)) {
							wr.Write((sbyte)-3);
							TimeSpan ts = (TimeSpan)value;
							wr.Write((Int64)ts.Ticks);
						} else {
							wr.Write((sbyte)TypeCode.Object);
							new BinaryFormatter().Serialize(wr.BaseStream, value);
						}
						break;
				}
			}
		}
		static object ReadObject(BinaryReader rd) {
			SByte code = rd.ReadSByte();
			switch(code) {
				case (sbyte)TypeCode.Empty:
					return null;
				case (sbyte)TypeCode.Boolean:
					return rd.ReadBoolean();
				case (sbyte)TypeCode.Char:
					return rd.ReadChar();
				case (sbyte)TypeCode.SByte:
					return rd.ReadSByte();
				case (sbyte)TypeCode.Byte:
					return rd.ReadByte();
				case (sbyte)TypeCode.Int16:
					return rd.ReadInt16();
				case (sbyte)TypeCode.UInt16:
					return rd.ReadUInt16();
				case (sbyte)TypeCode.Int32:
					return rd.ReadInt32();
				case (sbyte)TypeCode.UInt32:
					return rd.ReadUInt32();
				case (sbyte)TypeCode.Int64:
					return rd.ReadInt64();
				case (sbyte)TypeCode.UInt64:
					return rd.ReadUInt64();
				case (sbyte)TypeCode.Single:
					return rd.ReadSingle();
				case (sbyte)TypeCode.Double:
					return rd.ReadDouble();
				case (sbyte)TypeCode.Decimal:
					return rd.ReadDecimal();
				case (sbyte)TypeCode.DateTime:
					return DateTime.FromBinary(rd.ReadInt64());
				case (sbyte)TypeCode.String:
					return rd.ReadString();
				case (sbyte)TypeCode.Object:
					return new BinaryFormatter().Deserialize(rd.BaseStream);
				case (sbyte)-1:
					return rd.ReadBytes(rd.ReadInt32());
				case (sbyte)-2:
					return new Guid(rd.ReadBytes(16));
				case (sbyte)-3:
					return TimeSpan.FromTicks(rd.ReadInt64());
				default:
					throw new InvalidOperationException("Unknown type code: " + code.ToString());
			}
		}
	}
}
#endif
#endif
