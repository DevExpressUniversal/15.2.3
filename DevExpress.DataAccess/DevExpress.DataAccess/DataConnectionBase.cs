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
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DataAccess {
	public abstract class DataConnectionBase : INamedItem {
		static void ProcessException(Exception exception) {
			UnableToOpenDatabaseException unableToOpenException = exception as UnableToOpenDatabaseException;
			Exception innerException = exception;
			string message = exception.Message;
			if(unableToOpenException != null && unableToOpenException.InnerException != null) {
				innerException = unableToOpenException.InnerException;
				message = innerException.Message;
			}
			TargetInvocationException taex = exception as TargetInvocationException;
			if(taex != null && taex.InnerException != null) {
				innerException = taex.InnerException;
				message = taex.Message;
			}
			throw new DatabaseConnectionException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.DatabaseConnectionExceptionStringId), message), innerException);
		}
		string name = "";
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool StoreConnectionNameOnly { get; set; }
		[Browsable(false)]
		public string Name { get { return name; } set { name = value; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		[Browsable(false)]
		public abstract string ConnectionString { get;set; }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public virtual string ConnectionStringSerializable { get { return ConnectionString; } set { ConnectionString = value; } }
		[Browsable(false)]
		public abstract bool IsConnected { get; }
		public bool HasConnectionString { get { return !string.IsNullOrEmpty(ConnectionString); } }
		protected DataConnectionBase(string name) {
			this.name = name;
		}
		protected DataConnectionBase() {
		}
		public void Open() {
			if(!IsConnected)
				CreateDataStoreCore();
		}
		public abstract void Close();
		internal void CreateDataStore(IDataConnectionParametersService dataConnectionParametersProvider, CancellationToken token) {
			CreateDataStore(dataConnectionParametersProvider, EmptyWaitFormProvider.Instance, token);
		}
		internal void CreateDataStore(IDataConnectionParametersService dataConnectionParametersProvider, IWaitFormProvider waitFormProvider, CancellationToken token) {
			if(IsConnected)
				return;
			if(dataConnectionParametersProvider != null) {
				DataConnectionParametersBase dataConnectionParameters = CreateDataConnectionParameters();
				dataConnectionParameters = dataConnectionParametersProvider.RaiseConfigureDataConnection(Name, dataConnectionParameters);
				if(dataConnectionParameters != null) {
					ApplyParameters(dataConnectionParameters);
				}
			}
			do {
				Exception exception = null;
				waitFormProvider.ShowWaitForm();
				try {
					Task.Factory.StartNew(CreateDataStoreCore, token, TaskCreationOptions.None, TaskScheduler.Default).Wait(token);
				}
				catch(Exception ex) {
					AggregateException aex = ex as AggregateException;
					if(aex == null)
						exception = ex;
					else {
						aex = aex.Flatten();
						exception = aex.InnerExceptions.Count == 1 ? aex.InnerException : aex;
					}
				}
				finally {
					waitFormProvider.CloseWaitForm(false);
				}
				if(IsConnected)
					return;
				token.ThrowIfCancellationRequested();
				if(dataConnectionParametersProvider == null) {
					if(exception != null)
						ProcessException(exception);
					return;
				}
				DataConnectionParametersBase dataConnectionParameters = CreateDataConnectionParameters();
				ConnectionErrorEventArgs connectionEventArgs = new ConnectionErrorEventArgs(Name, dataConnectionParameters, exception);
				dataConnectionParameters = dataConnectionParametersProvider.RaiseHandleConnectionError(connectionEventArgs);
				if(!connectionEventArgs.Handled)
					ProcessException(exception);
				if(dataConnectionParameters == null)
					return;
				ApplyParameters(dataConnectionParameters);
			}
			while(true);
		}
		internal void CreateDataStore(IDataConnectionParametersService dataConnectionParametersService) {
			CreateDataStore(dataConnectionParametersService, new CancellationToken(false));
		}   
		internal XElement SaveToXml() {
			XElement element = new XElement(DataConnectionHelper.XmlDataConnection);
			SaveToXml(element);
			return element;
		}
#if DXPORTABLE
		public
#else 
		protected internal 
#endif            
		virtual void SaveToXml(XElement element) {
			element.Add(new XAttribute(DataConnectionHelper.XmlName, Name));
			if(ShouldSerializeConnectionStringSerializable())
				element.Add(new XAttribute(DataConnectionHelper.XmlConnectionString, ConnectionStringSerializable));
		}
#if DXPORTABLE
		public
#else
		protected internal
#endif
		virtual void LoadFromXml(XElement element) {
			name = element.GetAttributeValue(DataConnectionHelper.XmlName);
			if(name == null)
				throw new XmlException();
			ConnectionStringSerializable = XmlHelperBase.GetAttributeValue(element, DataConnectionHelper.XmlConnectionString);
		}
		protected virtual void ApplyParameters(DataConnectionParametersBase dataConnectionParameters) {
			CustomStringConnectionParameters customStringParameters = dataConnectionParameters as CustomStringConnectionParameters;
			if(customStringParameters != null) {
				ConnectionString = customStringParameters.ConnectionString;
			}
		}
		protected virtual void Dispose(bool disposing) {		  
		}
		public abstract DataConnectionParametersBase CreateDataConnectionParameters();
		public virtual string CreateConnectionString() {
			return ConnectionString;
		}
		protected abstract void CreateDataStoreCore();
		bool ShouldSerializeConnectionStringSerializable() {
			return HasConnectionString;
		}
		public virtual DBSchema GetDBSchema() {
			return new DBSchema(new DBTable[0], new DBTable[0]);
		}
		public virtual DBSchema GetDBSchema(string[] tableList) {
			return new DBSchema(new DBTable[0], new DBTable[0]);
		}
	}
}
