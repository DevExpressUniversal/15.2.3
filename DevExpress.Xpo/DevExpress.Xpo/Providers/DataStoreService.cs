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
#else 
using System.Windows.Threading;
#endif
using DevExpress.Xpo.Helpers;
using System.Threading;
using System.Windows;
using DevExpress.Xpo.Exceptions;
namespace DevExpress.Xpo.DB {
	[ServiceContract, XmlSerializerFormat]
	public interface IDataStoreService {
		[OperationContract(Action = "http://tempuri.org/IDataStoreService/ModifyData")]
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
		OperationResult<ModificationResult> ModifyData(ModificationStatement[] dmlStatements);
		[OperationContract(Action = "http://tempuri.org/IDataStoreService/SelectData")]
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
		OperationResult<SelectedData> SelectData(SelectStatement[] selects);
		[OperationContract(Action = "http://tempuri.org/IDataStoreService/UpdateSchema")]
		OperationResult<UpdateSchemaResult> UpdateSchema(bool dontCreateIfFirstTableNotExist, DBTable[] tables);
		[OperationContract(Action = "http://tempuri.org/IDataStoreService/GetAutoCreateOption")]
		OperationResult<AutoCreateOption> GetAutoCreateOption();
		[ServiceKnownType(typeof(CommandChannelHelper.SprocQuery))]
		[ServiceKnownType(typeof(CommandChannelHelper.SqlQuery))]
		[OperationContract(Action = "http://tempuri.org/IDataStoreService/Do")]
		OperationResult<object> Do(string command, object args);
	}
#if !SL
	[ServiceContract, XmlSerializerFormat]
	public interface IDataStoreWarpService : IDataStoreService {
		[OperationContract(Action = "http://tempuri.org/IDataStoreService/WarpSelectData")]
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
		OperationResult<byte[]> WarpSelectData(SelectStatement[] selects);
	}
#endif
#if SL
	[ServiceContract, XmlSerializerFormat]
	public interface IDataStoreClientAsync {
		[XmlSerializerFormat, OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IDataStoreService/ModifyData", ReplyAction = "*")]
		IAsyncResult BeginModifyData(ModificationStatement[] dmlStatements, AsyncCallback callback, object asyncState);
		OperationResult<ModificationResult> EndModifyData(IAsyncResult result);
		[XmlSerializerFormat,OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IDataStoreService/SelectData", ReplyAction = "*")]
		IAsyncResult BeginSelectData(SelectStatement[] selects, AsyncCallback callback, object asyncState);
		OperationResult<SelectedData> EndSelectData(IAsyncResult result);
		[XmlSerializerFormat, OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IDataStoreService/UpdateSchema", ReplyAction = "*")]
		IAsyncResult BeginUpdateSchema(bool dontCreateIfFirstTableNotExist, DBTable[] tables, AsyncCallback callback, object asyncState);
		OperationResult<UpdateSchemaResult> EndUpdateSchema(IAsyncResult result);
		[XmlSerializerFormat, OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IDataStoreService/GetAutoCreateOption", ReplyAction = "*")]
		IAsyncResult BeginGetAutoCreateOption(AsyncCallback callback, object asyncState);
		OperationResult<AutoCreateOption> EndGetAutoCreateOption(IAsyncResult result);
		[ServiceKnownType(typeof(CommandChannelHelper.SprocQuery))]
		[ServiceKnownType(typeof(CommandChannelHelper.SqlQuery))]
		[XmlSerializerFormat, OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IDataStoreService/Do", ReplyAction = "*")]
		IAsyncResult BeginDo(string command, object args, AsyncCallback callback, object asyncState);
		OperationResult<object> EndDo(IAsyncResult result);
	}
#endif
	public class ServiceBase {
		public static event ServiceExceptionHandler GlobalServiceExceptionThrown;
		public event ServiceExceptionHandler ServiceExceptionThrown;
		protected virtual void OnServiceExceptionThrown(Exception ex) {
			ServiceExceptionEventArgs args = new ServiceExceptionEventArgs(ex);
			ServiceExceptionHandler handler = ServiceExceptionThrown;
			if(handler != null) {
				handler(this, args);
			}
			handler = GlobalServiceExceptionThrown;
			if(handler != null) {
				handler(this, args);
			}
		}
		protected OperationResult<T> Execute<T>(OperationResultPredicate<T> predicate) {
			try {
				return new OperationResult<T>(predicate());
			} catch(NotSupportedException ex) {
				OnServiceExceptionThrown(ex);
				return new OperationResult<T>(ServiceException.NotSupported, ex.Message);
			} catch(SchemaCorrectionNeededException ex) {
				OnServiceExceptionThrown(ex);
				return new OperationResult<T>(ServiceException.Schema, ex.Message);
			} catch(LockingException ex) {
				OnServiceExceptionThrown(ex);
				return new OperationResult<T>(ServiceException.Locking, string.Empty);
			} catch(ObjectLayerSecurityException ex) {
				OnServiceExceptionThrown(ex);
				return new OperationResult<T>(ServiceException.ObjectLayerSecurity, OperationResult.SerializeSecurityException(ex));
			} catch(Exception ex) {
				OnServiceExceptionThrown(ex);
				return new OperationResult<T>(ServiceException.Unknown, ex.Message);
			}
		}
	}
#if !SL
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
#endif
	public class DataStoreService : ServiceBase, 
#if SL
		IDataStoreService {
#else
		IDataStoreWarpService {
#endif
		protected readonly IDataStore provider;
		protected readonly ICommandChannel commandChannel;
		public DataStoreService(IDataStore provider) {
			this.provider = provider;
			this.commandChannel = provider as ICommandChannel;
		}
		public virtual OperationResult<ModificationResult> ModifyData(ModificationStatement[] dmlStatements) {
			return Execute<ModificationResult>(delegate() { return provider.ModifyData(dmlStatements); });
		}
		public virtual OperationResult<SelectedData> SelectData(SelectStatement[] selects) {
			return Execute<SelectedData>(delegate() { return provider.SelectData(selects); });
		}
#if !SL
		public virtual OperationResult<byte[]> WarpSelectData(SelectStatement[] selects) {
			return Execute<byte[]>(delegate() {
				SelectedData selectedData = provider.SelectData(selects);
				return WcfUsedAsDumbPipeHelper.Warp(selectedData == null ? null : selectedData.ResultSet);
			});
		}
#endif
		public virtual OperationResult<UpdateSchemaResult> UpdateSchema(bool dontCreateIfFirstTableNotExist, DBTable[] tables) {
			return Execute<UpdateSchemaResult>(delegate() { return provider.UpdateSchema(dontCreateIfFirstTableNotExist, tables); });
		}
		public virtual OperationResult<AutoCreateOption> GetAutoCreateOption() {
			return Execute<AutoCreateOption>(delegate() { return provider.AutoCreateOption; });
		}
		public virtual OperationResult<object> Do(string command, object args) {
			if(commandChannel == null) {
				if(provider == null) {
					return new OperationResult<object>(ServiceException.NotSupported, string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
				} else {
					return new OperationResult<object>(ServiceException.NotSupported, string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, provider.GetType().FullName));
				}
			}
			return Execute<object>(delegate() { return commandChannel.Do(command, args); });
		}
	}
	public class DataStoreClient : 
#if SL
	   DataStoreClientBase<IDataStoreClientAsync>
#else
	   DataStoreClientBase<IDataStoreWarpService>
#endif
	{
		public DataStoreClient(string confName) : base(confName) { }
		public DataStoreClient(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress) { }
	}
	public class DataStoreClientBase<TContractType> : ClientBase<TContractType>, IDataStore, ICommandChannel
#if SL
		where TContractType: class, IDataStoreClientAsync
#else
		where TContractType: class, IDataStoreWarpService
#endif
	{
		public DataStoreClientBase(string confName) : base(confName) { }
		public DataStoreClientBase(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress) { }
		public event ClientChannelCreatedHandler ClientChannelCreated;
		public static event ClientChannelCreatedHandler GlobalDataClientChannelCreated;
		protected virtual void OnClientChannelCreated(object channel) {
			if(ClientChannelCreated != null) {
				ClientChannelCreated(this, new ClientChannelCreatedEventArgs(channel));
			}
			if(GlobalDataClientChannelCreated != null) {
				GlobalDataClientChannelCreated(this, new ClientChannelCreatedEventArgs(channel));
			}
		}
#if SL
		protected override TContractType CreateChannel() {
			object channel = new DataStoreClientChannel(this);
			OnClientChannelCreated(channel);
			return (TContractType)channel;
		}
		protected new TContractType Channel {
			get {
				return (TContractType)base.Channel;
			}
		}
		ModificationResult IDataStore.ModifyData(ModificationStatement[] dmlStatements) {
			return OperationResult.ExecuteClient<ModificationResult>(() => {
				IAsyncResult res = Channel.BeginModifyData(dmlStatements, null, null);
				return Channel.EndModifyData(res);
			}).HandleError();
		}
		SelectedData IDataStore.SelectData(SelectStatement[] selects) {
			return OperationResult.ExecuteClient<SelectedData>(() => {
				IAsyncResult res = Channel.BeginSelectData(selects, null, null);
				return Channel.EndSelectData(res);
			}).HandleError();
		}
		UpdateSchemaResult IDataStore.UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
			return OperationResult.ExecuteClient<UpdateSchemaResult>(() => {
				IAsyncResult res = Channel.BeginUpdateSchema(dontCreateIfFirstTableNotExist, tables, null, null);
				return Channel.EndUpdateSchema(res);
			}).HandleError();
		}
		AutoCreateOption IDataStore.AutoCreateOption {
			get {
				return OperationResult.ExecuteClient<AutoCreateOption>(() => {
					IAsyncResult res = Channel.BeginGetAutoCreateOption(null, null);
					return Channel.EndGetAutoCreateOption(res);
				}).HandleError();
			}
		}
		object ICommandChannel.Do(string command, object args) {
			return OperationResult.ExecuteClient<object>(() => {
				IAsyncResult res = Channel.BeginDo(command, args, null, null);
				return Channel.EndDo(res);
			}).HandleError();
		}
		class DataStoreClientChannel : DataStoreClientChannelBase<TContractType> {
			public DataStoreClientChannel(ClientBase<TContractType> client)
				: base(client) {
			}
		}
		protected class DataStoreClientChannelBase<TAsyncContractType> : ChannelBase<TAsyncContractType>, IDataStoreClientAsync 
			where TAsyncContractType : class, IDataStoreClientAsync
		{
			public DataStoreClientChannelBase(ClientBase<TAsyncContractType> client)
				: base(client) {
			}
			public IAsyncResult BeginModifyData(ModificationStatement[] statements, AsyncCallback callback, object asyncState) {
				return BeginInvoke("ModifyData", new object[] { statements }, callback, asyncState);
			}
			public IAsyncResult BeginSelectData(SelectStatement[] selects, AsyncCallback callback, object asyncState) {
				return BeginInvoke("SelectData", new object[] { selects }, callback, asyncState);
			}
			public IAsyncResult BeginUpdateSchema(bool dontCreateIfFirstTableNotExist, DBTable[] tables, AsyncCallback callback, object asyncState) {
				return BeginInvoke("UpdateSchema", new object[] { dontCreateIfFirstTableNotExist, tables }, callback, asyncState);
			}
			public IAsyncResult BeginGetAutoCreateOption(AsyncCallback callback, object asyncState) {
				return BeginInvoke("GetAutoCreateOption", emptyArray, callback, asyncState);
			}
			public IAsyncResult BeginDo(string command, object args, AsyncCallback callback, object asyncState) {
				return BeginInvoke("Do", new object[] { command, args, }, callback, asyncState);
			}
			static object[] emptyArray = new object[0];
			public OperationResult<ModificationResult> EndModifyData(IAsyncResult result) {
				return (OperationResult<ModificationResult>)EndInvoke("ModifyData", emptyArray, result);
			}
			public OperationResult<SelectedData> EndSelectData(IAsyncResult result) {
				return (OperationResult<SelectedData>)EndInvoke("SelectData", emptyArray, result);
			}
			public OperationResult<UpdateSchemaResult> EndUpdateSchema(IAsyncResult result) {
				return (OperationResult<UpdateSchemaResult>)EndInvoke("UpdateSchema", emptyArray, result);
			}
			public OperationResult<AutoCreateOption> EndGetAutoCreateOption(IAsyncResult result) {
				return (OperationResult<AutoCreateOption>)EndInvoke("GetAutoCreateOption", emptyArray, result);
			}
			public OperationResult<object> EndDo(IAsyncResult result) {
				return (OperationResult<object>)EndInvoke("Do", emptyArray, result);
			}
		}
#else
		bool selectDataRawFound;
		bool selectDataRawNotFound;
		TContractType channel;
		protected new TContractType Channel {
			get {
				TContractType currentChannel = channel;
				if(currentChannel == null) {
					currentChannel = CreateChannel();
					channel = currentChannel;
					OnClientChannelCreated(currentChannel);
				}
				return currentChannel;
			}
		}
		ModificationResult IDataStore.ModifyData(ModificationStatement[] dmlStatements) {
			return ExecuteClient<ModificationResult>(delegate() { return Channel.ModifyData(dmlStatements); }).HandleError();
		}
		SelectedData IDataStore.SelectData(SelectStatement[] selects) {
			if(!selectDataRawNotFound) {
				try {
					SelectedData selectedData = new SelectedData(WcfUsedAsDumbPipeHelper.Unwarp(ExecuteClient<byte[]>(delegate() {
						return ((IDataStoreWarpService)Channel).WarpSelectData(selects);
					}).HandleError()));
					selectDataRawFound = true;
					return selectedData;
				} catch(Exception) {
					if(selectDataRawFound) throw;
					selectDataRawNotFound = true;
				}
			}
			return ExecuteClient<SelectedData>(delegate() { return Channel.SelectData(selects); }).HandleError();
		}
		UpdateSchemaResult IDataStore.UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
			return ExecuteClient<UpdateSchemaResult>(delegate() { return Channel.UpdateSchema(dontCreateIfFirstTableNotExist, tables); }).HandleError();
		}
		AutoCreateOption IDataStore.AutoCreateOption {
			get { return ExecuteClient<AutoCreateOption>(delegate() { return Channel.GetAutoCreateOption(); }).HandleError(); }
		}
		object ICommandChannel.Do(string command, object args) {
			return ExecuteClient<object>(delegate() { return Channel.Do(command, args); }).HandleError();
		}
		public OperationResult<R> ExecuteClient<R>(OperationResultChannelPredicate<R> predicate) {
			return OperationResult.ExecuteClient<R, TContractType>(predicate, ref channel);
		}
#endif
	}
	public class OperationResult {
		const string SerializeSeparator = "#|O_o|#";
		public ServiceException ErrorType;
		public string Error;
		public OperationResult() { }
		public OperationResult(ServiceException errorType, string error) {
			ErrorType = errorType;
			Error = error;
		}
		public static int ExecuteClientMaxRetryCount = 3;
		public static string SerializeSecurityException(ObjectLayerSecurityException sx) {
			return string.Join(SerializeSeparator, new string[] { sx.TypeName, sx.PropertyName, sx.IsDeletion ? "True" : "False" });
		}
		public static ObjectLayerSecurityException DeserializeSecurityException(string data) {
			if(string.IsNullOrEmpty(data)) return null;
			string[] splittedData = data.Split(new string[] { SerializeSeparator }, StringSplitOptions.None);
			if(splittedData.Length != 3) return null;
			if(string.IsNullOrEmpty(splittedData[0])) return new ObjectLayerSecurityException();
			if(!string.IsNullOrEmpty(splittedData[1])) return new ObjectLayerSecurityException(splittedData[0], splittedData[1]);
			return new ObjectLayerSecurityException(splittedData[0], splittedData[2] == "True");
		}
#if SL
		public static void CheckCallingThread() {
			if (Deployment.Current.Dispatcher.CheckAccess()) throw new InvalidOperationException("Calls from the UI thread are prohibited.");
		}
		public static OperationResult<T> ExecuteClient<T>(OperationResultChannelPredicate<T> predicate) {
			CheckCallingThread();
			int count = ExecuteClientMaxRetryCount;
			do {
				try {
					return predicate();
				} catch (Exception) {
					if (count-- > 0) {
						continue;
					}
					throw;
				}
			} while (true);
		}
#else
		public static OperationResult<T> ExecuteClient<T, N>(OperationResultChannelPredicate<T> predicate, ref N channel) {
			int count = ExecuteClientMaxRetryCount;
			for(; ; ) {
				try {
					return predicate();
				} catch(Exception) {
					channel = default(N);
					if(count-- > 0) {
						continue;
					}
					throw;
				}
			}
		}
#endif
	}
	public class OperationResult<T> : OperationResult {
		public T Result;
		public OperationResult() : base() { }
		public OperationResult(ServiceException errorType, string error)
			: base(errorType, error) {
		}
		public OperationResult(T result)
			: this(ServiceException.None, null) {
			Result = result;
		}
		public T HandleError() {
			switch(ErrorType) {
				case ServiceException.Unknown:
					throw new Exception(Error);
				case ServiceException.Schema:
					throw new SchemaCorrectionNeededException(Error);
				case ServiceException.NotSupported:
					throw new NotSupportedException(Error);
				case ServiceException.ObjectLayerSecurity: {
						ObjectLayerSecurityException ex = DeserializeSecurityException(Error);
						if(ex == null) ex = new ObjectLayerSecurityException();
						throw ex;
					}
				case ServiceException.Locking:
					throw new LockingException();
			}
			return Result;
		}
	}
	public class ClientChannelCreatedEventArgs : EventArgs {
		object channel;
		public object Channel { get { return channel; } }
		public ClientChannelCreatedEventArgs(object channel) {
			this.channel = channel;
		}
	}
	public delegate void ClientChannelCreatedHandler(object sender, ClientChannelCreatedEventArgs e);
	public delegate T OperationResultPredicate<T>();
	public delegate OperationResult<T> OperationResultChannelPredicate<T>();
	public enum ServiceException { None, Unknown, Schema, NotSupported, Locking, ObjectLayerSecurity }
	public delegate void ServiceExceptionHandler(object sender, ServiceExceptionEventArgs e);
	public class ServiceExceptionEventArgs : EventArgs {
		Exception exception;
		public Exception Exception { get { return exception; } }
		public ServiceExceptionEventArgs(Exception exception) {
			this.exception = exception;
		}
	}
}
#endif
