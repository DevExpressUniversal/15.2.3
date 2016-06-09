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
using System.Net;
using System.Text;
using DevExpress.PivotGrid.OLAP;
using DevExpress.PivotGrid.Xmla.Helpers;
using DevExpress.XtraPivotGrid;
using System.Xml;
namespace DevExpress.PivotGrid.Xmla {
	class XmlaCommandExecutor : LoaderBase<XmlaExecuteResult> {
		public static XmlaCommandExecutor Create(XmlaCommand command) {
			return new XmlaCommandExecutor(command);
		}
		readonly XmlaCommand command;
		public XmlaCommandExecutor(XmlaCommand command)
			: base(command.Connection) {
			this.command = command;
		}
		#region RowSet
		public IOLAPRowSet ExecuteRowSet() {
			return (IOLAPRowSet)base.GetActionResult(ExecuteRowSetAsync);
		}
		void ExecuteRowSetAsync(IOLAPUniqueEntity entity) {
			ExecuteAsync(Connection, command, command.CommandText, CatalogName, CubeName, OnExecuteRowSetCompleted);
		}
		void OnExecuteRowSetCompleted(object sender, UploadRequestCompletedEventArgs<XmlaExecuteResult> e) {
			XmlaCommand command = e.UserState as XmlaCommand;
			if(CheckMethodResult(command, e))
				return;
			IOLAPRowSet rowSet = new XMLReaderRowSet(e.MethodResult.XmlReader);
			base.FinalizeAction(rowSet);
		}
		#endregion
		#region CellSet
		public IOLAPCellSet ExecuteCellSet() {
			return (IOLAPCellSet)base.GetActionResult(ExecuteCellSetAsync);
		}
		void ExecuteCellSetAsync(IOLAPUniqueEntity entity) {
			ExecuteAsync(Connection, command, command.CommandText, CatalogName, CubeName, OnExecuteCellSetCompleted);
		}
		void OnExecuteCellSetCompleted(object sender, UploadRequestCompletedEventArgs<XmlaExecuteResult> e) {
			XmlaCommand command = e.UserState as XmlaCommand;
			if(CheckMethodResult(command, e))
				return;
			IOLAPCellSet cellSet = new XMLReaderCellSet(e.MethodResult.XmlReader);
			base.FinalizeAction(cellSet);
		}
		#endregion
		#region Common
		void ExecuteAsync(XmlaConnection connection, object userState, string mdx, string catalogName, string cubeName,
				UploadRequestCompletedEventHandler<XmlaExecuteResult> completedProc) {
			XmlaExecuteMethod method = new XmlaExecuteMethod(mdx);
			method.AddProperty(OlapProperty.Catalog, catalogName);
			method.AddProperty(OlapProperty.Cube, cubeName);
			method.AddProperty(OlapProperty.Timeout, Convert.ToString(command.CommandTimeout));
			method.AddProperty(OlapProperty.Format, command.ResponseFormat.ToString());
			method.AddProperty(OlapProperty.AxisFormat, "TupleFormat");
			method.AddProperty(OlapProperty.Content, "SchemaData");
			base.AddCommonProperties(connection, method);
			base.UploadMethodRequest(method, userState, completedProc);
		}
		#endregion
	}
	class XmlaSchemaLoader : LoaderBase<XmlaDiscoverResult> {
		public static XmlaSchemaLoader Create(XmlaConnection connection) {
			return new XmlaSchemaLoader(connection);
		}
		Dictionary<string, Dictionary<string, OLAPDataType>> levelProperties;
		XmlaSchemaLoader(XmlaConnection connection)
			: base(connection) {
			this.levelProperties = new Dictionary<string, Dictionary<string, OLAPDataType>>();
		}
		public void BeginSession() {
			base.DoAction(BeginSessionAsync);
		}
		public void EndSession() {
			base.DoAction(EndSessionAsync);
		}
		public void GetCatalogsAsync(AsyncCompletedHandler completed) {
			GetSchemaAsync(Connection, completed, OlapSchema.Catalogs, null,
				delegate(object sender, UploadRequestCompletedEventArgs<XmlaDiscoverResult> e) {
					OnGetNamesCompleted(sender, e, OlapProperty.CatalogName);
				});
		}
		public void GetCubesAsync(AsyncCompletedHandler completed) {
			GetSchemaAsync(Connection, completed, OlapSchema.Cubes, CatalogName,
				(sender, e) => {
					OnGetNamesCompleted(sender, e, OlapProperty.CubeName);
				});
		}
		public IOLAPRowSet ExecuteSchemaRowSet(string schema, Dictionary<string, object> restrictions) {
			return new XMLReaderRowSet(((XmlaDiscoverResult)base.GetActionResult((aa) =>
				GetSchemaAsync(Connection, null, schema, CatalogName, restrictions,
				(sender, e) => {
					base.FinalizeAction(e.MethodResult);
				})
			)).XmlReader);
		}
		void OnGetNamesCompleted(object sender, UploadRequestCompletedEventArgs<XmlaDiscoverResult> e, string name) {
			XmlaConnection connection = Connection;
			AsyncCompletedHandler completed = e.UserState as AsyncCompletedHandler;
			IList<string> namesList = null;
			Exception exception = null;
			try {
				if(CheckMethodResult(connection, e))
					return;
				namesList = XParser.GetNames(e.MethodResult.Value, connection, name);
			} catch(Exception ex) {
				exception = ex;
			} finally {
				completed(XmlaMetaGetter.CreateAsyncOperationResult(namesList, exception));
			}
		}
		#region Begin/EndSession
		void BeginSessionAsync(IOLAPUniqueEntity entity) {
			GetSchemaAsync(Connection, Connection, OlapSchema.DiscoverProperties, CatalogName, OnBeginSessionCompleted);
		}
		void OnBeginSessionCompleted(object sender, UploadRequestCompletedEventArgs<XmlaDiscoverResult> e) {
			XmlaConnection connection = e.UserState as XmlaConnection;
			if(CheckMethodResult(connection, e))
				return;
			XParser.FillConnectionProps(e.MethodResult.Value, connection);
		}
		void EndSessionAsync(IOLAPUniqueEntity entity) {
			GetSchemaAsync(Connection, Connection, OlapSchema.DiscoverProperties, CatalogName, null);
		}
		#endregion
		#region Common
		void GetSchemaAsync(XmlaConnection connection, object userState, string schema, string catalogName, Dictionary<string, object> restrictions, UploadRequestCompletedEventHandler<XmlaDiscoverResult> completedProc) {
			XmlaDiscoverMethod method = new XmlaDiscoverMethod(schema);
			if(!string.IsNullOrEmpty(catalogName))
				method.AddProperty(OlapProperty.Catalog, catalogName);
			base.AddCommonProperties(connection, method);
			if(schema != OlapSchema.DiscoverProperties)
				method.Restrictions.Add(OlapProperty.CatalogName, catalogName);
			foreach(KeyValuePair<string, object> pair in restrictions)
				if(!object.ReferenceEquals(null, pair.Value) && !method.Restrictions.ContainsKey(pair.Key))
					method.Restrictions.Add(pair.Key, pair.Value);
			base.UploadMethodRequest(method, userState, completedProc);
		}
		void GetSchemaAsync(XmlaConnection connection, object userState, string schema, string catalogName, UploadRequestCompletedEventHandler<XmlaDiscoverResult> completedProc) {
			GetSchemaAsync(connection, userState, schema, catalogName, new Dictionary<string, object>(), completedProc);
		}
		void GetSchemaAsync(XmlaConnection connection, object userState, string schema, string catalogName, string cubeName, UploadRequestCompletedEventHandler<XmlaDiscoverResult> completedProc) {
			GetSchemaAsync(connection, userState, schema, catalogName, new Dictionary<string, object>() {
																					{ OlapProperty.CubeName, cubeName }
																										}, completedProc);
		}
		void GetSchemaAsync(XmlaConnection connection, object userState, string schema, string catalogName, string cubeName, string memberUniqueName, UploadRequestCompletedEventHandler<XmlaDiscoverResult> completedProc) {
			GetSchemaAsync(connection, userState, schema, catalogName, new Dictionary<string, object>() {
																					{ OlapProperty.CubeName, cubeName },
																					{ OlapProperty.MemberUniqueName, memberUniqueName }
																										}, completedProc);
		}
		void GetSchemaAsync(XmlaConnection connection, object userState, string schema, string catalogName, string cubeName, int memberType, UploadRequestCompletedEventHandler<XmlaDiscoverResult> completedProc) {
			GetSchemaAsync(connection, userState, schema, catalogName, new Dictionary<string, object>() {
																					{ OlapProperty.CubeName, cubeName },
																					{ OlapProperty.MEMBERTYPE, memberType }
																										}, completedProc);
		}
		void GetPropertyAsync(XmlaConnection connection, object userState, string schema, string propertyName, UploadRequestCompletedEventHandler<XmlaDiscoverResult> completedProc) {
			XmlaDiscoverMethod method = new XmlaDiscoverMethod(schema);
			method.Restrictions.Add(OlapProperty.PropertyName, propertyName);
			base.UploadMethodRequest(method, userState, completedProc);
		}
		#endregion
	}
	abstract class LoaderBase<TResult> where TResult : IXmlaMethodResult, new() {
		readonly XmlaConnection connection;
		IEventWaiter eventWaiter;
		NetworkCredential credentials;
		protected DevExpress.XtraPivotGrid.Data.XmlaErrorResponseException ex;
		object actionResult;
		protected LoaderBase(XmlaConnection connection) {
			this.connection = connection;
			this.eventWaiter = null;
			if(string.IsNullOrEmpty(connection.UserId)) {
				this.credentials = null;
			} else {
				this.credentials = new NetworkCredential(connection.UserId, connection.Password);
			}
		}
		protected string CatalogName {
			get { return connection.CatalogName; }
		}
		protected string CubeName {
			get { return connection.CubeName; }
		}
		protected XmlaConnection Connection {
			get { return connection; }
		}
		protected object GetActionResult(ProcessEntity process) {
			this.actionResult = null;
			DoAction(process);
			return this.actionResult;
		}
		protected void FinalizeAction(object actionResult) {
			this.actionResult = actionResult;
		}
		protected void DoAction(ProcessEntity process) {
			DoAction(process, null);
		}
		protected void DoAction(ProcessEntity process, IOLAPUniqueEntity entity) {
			using(this.eventWaiter = EventWaiterCreator.Create(IsAsync)) {
				this.eventWaiter.InvokeAndWait(process, entity);
			}
			ThrowInCallingThread();
		}
		void ThrowInCallingThread() {
			if(ex != null)
				throw ex;
		}
		bool IsAsync {
			get { return connection.IsAsync; }
		}
		protected void AddCommonProperties(XmlaConnection connection, XmlaMethodBase<TResult> method) {
			method.AddProperty(OlapProperty.LocaleIdentifier, Convert.ToString(connection.LocaleIdentifier));
			if(!string.IsNullOrEmpty(connection.Roles))
				method.AddProperty(OlapProperty.Roles, connection.Roles);
			if(!string.IsNullOrEmpty(connection.CustomData))
				method.AddProperty(OlapProperty.CustomData, connection.CustomData);
		}
		protected void UploadMethodRequest(IXmlaMethod<TResult> method, object userState, UploadRequestCompletedEventHandler<TResult> completedProc) {
			XmlaMethodRequestUploader<TResult> requestUploader = new XmlaMethodRequestUploader<TResult>(connection.ServerName, Encoding.UTF8, this.credentials, this.eventWaiter);
			XmlaHeader header = new XmlaHeader(connection.SessionId);
			if(completedProc == null) {
				header.HeaderType = XmlaSoapHeaderType.EndSession;
			} else {
				requestUploader.UploadRequestCompleted += completedProc;
			}
			XmlaSoapMessage<TResult> soapMessage = new XmlaSoapMessage<TResult>(header, method);
			bool uploaded = requestUploader.UploadRequest(soapMessage, userState, IsAsync);
			if(!uploaded) {
				this.eventWaiter.Finish(true);
			}
		}
		protected bool CheckMethodResult(IOLAPEntity olapEntity, UploadRequestCompletedEventArgs<TResult> args) {
			bool hasError = (olapEntity == null || args.Errors != null || args.Error != null || args.MethodResult.ResultData == null);
			if(!hasError && !string.IsNullOrEmpty(args.MethodResult.SessionId)) {
				Connection.SessionId = args.MethodResult.SessionId;
			}
			if(hasError)
				ex = new DevExpress.XtraPivotGrid.Data.XmlaErrorResponseException(args.Errors, args.Error);
			return hasError;
		}
	}
}
