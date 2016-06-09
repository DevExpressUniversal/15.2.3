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
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Services {
	public interface IRTDServiceFactory {
		IRTDService CreateRTDService();
	}
	public interface IRTDService {
		bool Connect(string progId, string serverName);
		void Stop();
		bool Heartbeat();
		event EventHandler DataChanged;
		bool ConnectData(int topicId, object[] parameters);
		Dictionary<int, VariantValue> GetData();
		void DisconnectData(int topicId);
	}
}
#if !SL && !DXPORTABLE
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	using System.Threading;
	using DevExpress.Office.Services.Implementation;
	using Microsoft.Office.Interop.Excel;
#region RTDServiceFactory
	public class RTDServiceFactory : IRTDServiceFactory {
		public RTDServiceFactory() {
		}
		public IRTDService CreateRTDService() {
			return new RTDService();
		}
	}
#endregion
#region RTDService
	class RTDService : IRTDService, IDisposable, IRTDUpdateEvent {
#region Fields
		readonly Dictionary<int, object[]> registeredTopics;
		object serverObject;
		IRtdServer server;
		bool started;
#endregion
		public RTDService() {
			this.registeredTopics = new Dictionary<int, object[]>();
		}
		bool IsServerStarted { get { return server != null && started; } }
#region IRTDService implementation
		EventHandler onDataChanged;
		public event EventHandler DataChanged { add { onDataChanged += value; } remove { onDataChanged -= value; } }
		protected virtual void RaiseDataChanged() {
			if (onDataChanged != null)
				onDataChanged(this, EventArgs.Empty);
		}
		public bool Connect(string progId, string serverName) {
			if (!ConnectToServer(progId, serverName))
				return false;
			return StartServer();
		}
		public void Stop() {
			StopServer();
		}
		public bool Heartbeat() {
			if (IsServerStarted) {
				return server.Heartbeat() > 0;
			}
			else
				return false;
		}
		public bool ConnectData(int topicId, object[] parameters) {
			if (!IsServerStarted)
				return false;
			try {
				bool getNewValues = false;
				server.ConnectData(topicId, parameters, ref getNewValues);
				registeredTopics[topicId] = parameters;
				return true;
			}
			catch {
				return false;
			}
		}
		public Dictionary<int, VariantValue> GetData() {
			Dictionary<int, VariantValue> result = new Dictionary<int, VariantValue>();
			if (!IsServerStarted)
				return result;
			try {
				int topicCount = 0;
				Array data = server.RefreshData(ref topicCount);
				int count = topicCount;
				for (int i = 0; i < count; i++) {
					int topicId = (int)data.GetValue(0, i);
					VariantValue value = CreateVariantValue(data.GetValue(1, i));
					result[topicId] = value;
				}
			}
			catch {
			}
			return result;
		}
		public void DisconnectData(int topicId) {
			if (!IsServerStarted)
				return;
			try {
				server.DisconnectData(topicId);
			}
			catch {
			}
			if (registeredTopics.ContainsKey(topicId))
				registeredTopics.Remove(topicId);
		}
#endregion
#region CreateVariantValue
		static VariantValue CreateVariantValue(object value) {
			if (value == null)
				return VariantValue.Empty;
			if (Convert.IsDBNull(value))
				return VariantValue.Empty;
			Type type = value.GetType();
			if (type == typeof(string))
				return (string)value;
			if (type == typeof(bool))
				return (bool)value;
			if (type == typeof(double))
				return Convert.ToDouble(value);
			if (type == typeof(int))
				return Convert.ToDouble(value);
			if (type == typeof(long))
				return Convert.ToDouble(value);
			if (type == typeof(decimal))
				return Convert.ToDouble(value);
			if (type == typeof(float))
				return Convert.ToDouble(value);
			if (type == typeof(short))
				return Convert.ToDouble(value);
			if (type == typeof(byte))
				return Convert.ToDouble(value);
			if (type == typeof(ushort))
				return Convert.ToDouble(value);
			if (type == typeof(uint))
				return Convert.ToDouble(value);
			return VariantValue.Empty;
		}
#endregion
		void DisconnectAllTopics() {
			if (!IsServerStarted)
				return;
			foreach (int topicId in registeredTopics.Keys) {
				try {
					server.DisconnectData(topicId);
				}
				catch {
				}
			}
			registeredTopics.Clear();
		}
		bool ConnectToServer(string progId, string serverName) {
			try {
				return ConnectToServerCore(progId, serverName);
			}
			catch {
				return false;
			}
		}
		[System.Security.SecuritySafeCritical]
		bool ConnectToServerCore(string progId, string serverName) {
			Type type;
			if (String.IsNullOrEmpty(serverName))
				type = Type.GetTypeFromProgID(progId);
			else
				type = Type.GetTypeFromProgID(progId, serverName, false);
			if (type == null)
				return false;
			this.serverObject = Activator.CreateInstance(type);
			if (serverObject == null)
				return false;
			this.server = serverObject as IRtdServer;
			return server != null;
		}
		bool StartServer() {
			try {
				started = server.ServerStart(this) > 0;
				return started;
			}
			catch {
				return false;
			}
		}
		void StopServer() {
			try {
				if (IsServerStarted) {
					started = false;
					DisconnectAllTopics();
					server.ServerTerminate();
				}
				server = null;
				serverObject = null;
			}
			catch {
			}
		}
#region IDisposable implementation
		~RTDService() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				StopServer();
			}
		}
#endregion
#region IRTDUpdateEvent Members
		public int HeartbeatInterval { get; set; }
		public void UpdateNotify() {
			RaiseDataChanged();
		}
		public void Disconnect() {
			StopServer();
		}
#endregion
	}
#endregion
}
namespace Microsoft.Office.Interop.Excel {
	using System.Runtime.InteropServices;
#region IRtdServer
	[ComImport, TypeIdentifier, Guid("EC0E6191-DB51-11D3-8F3E-00C04F3651B8")]
	public interface IRtdServer {
		[DispId(10)]
		int ServerStart(IRTDUpdateEvent callback);
		[DispId(11)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		object ConnectData(int topicId, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref Array strings, ref bool newValues);
		[return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
		[DispId(12)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		Array RefreshData(ref int topicCount);
		[DispId(13)]
		void DisconnectData(int topicId);
		[DispId(14)]
		int Heartbeat();
		[DispId(15)]
		void ServerTerminate();
	}
#endregion
#region IRTDUpdateEvent
	[ComImport, TypeIdentifier, Guid("A43788C1-D91B-11D3-8F39-00C04F3651B8")]
	public interface IRTDUpdateEvent {
		[DispId(10)]
		void UpdateNotify();
		[DispId(11)]
		int HeartbeatInterval { get; set; }
		[DispId(12)]
		void Disconnect();
	}
#endregion
}
#endif
