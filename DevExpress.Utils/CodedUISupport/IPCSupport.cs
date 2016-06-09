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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Security.Principal;
namespace DevExpress.Utils.CodedUISupport {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class NamedPipeClient {
		public const string PipeName = "DevExpressCodedUISupportPipe";
		public const string ClassUri = "RemoteObject";
		public const int LeaseTimeInMinutes = 120;
		public const int DefaultLeaseTimeInMinutes = 5;
		List<int> registeredServerIds;
		static NamedPipeClient defaultInstance;
		internal static NamedPipeClient Instance {
			get {
				if(defaultInstance == null)
					defaultInstance = new NamedPipeClient();
				return defaultInstance;
			}
		}
		NamedPipeClient() {
			registeredServerIds = new List<int>();
		}
		public void Connect(int serverId) {
			if(!registeredServerIds.Contains(serverId)) {
				registeredServerIds.Add(serverId);
				RegisterChannel(serverId);
				RemoteObject remoteObject = GetRemoteObject(serverId);
				if(remoteObject != null)
					remoteObject.AddHelper(new ClientSideHelper(remoteObject), Process.GetCurrentProcess().Id);
			}
		}
		[System.Security.SecuritySafeCritical]
		void RegisterChannel(int serverId) {
			if(System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseTime.TotalMinutes == NamedPipeClient.DefaultLeaseTimeInMinutes)
				try {
					System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseTime = TimeSpan.FromMinutes(NamedPipeClient.LeaseTimeInMinutes);
				} catch(System.Runtime.Remoting.RemotingException) {
				}
			BinaryClientFormatterSinkProvider clientSinkProvider = new BinaryClientFormatterSinkProvider();
			BinaryServerFormatterSinkProvider binaryServerFormatterSinkProvider = new BinaryServerFormatterSinkProvider();
			binaryServerFormatterSinkProvider.TypeFilterLevel = TypeFilterLevel.Full;
			Hashtable hashtable = new Hashtable();
			hashtable["name"] = string.Format(CultureInfo.InvariantCulture, "client{0}", new object[]
	{
		PipeName + serverId
	});
			hashtable["portName"] = hashtable["name"];
			hashtable["authorizedGroup"] = WindowsIdentity.GetCurrent().Name;
			IChannel chnl = new IpcChannel(hashtable, clientSinkProvider, binaryServerFormatterSinkProvider);
			ChannelServices.RegisterChannel(chnl, false);
		}
		[System.Security.SecuritySafeCritical]
		RemoteObject GetRemoteObject(int serverId) {
			string url = string.Format(CultureInfo.InvariantCulture, @"ipc://{0}/{1}", new object[]
	{
		PipeName + serverId, ClassUri + serverId
	});
			return (RemoteObject)Activator.GetObject(typeof(RemoteObject), url);
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ClientSideHelpersManager {
		Dictionary<int, ClientSideHelper> helpers;
		Func<IntPtr, bool> reconnectFunc;
		internal ClientSideHelpersManager(Func<IntPtr, bool> reconnectFunc) {
			helpers = new Dictionary<int, ClientSideHelper>();
			this.reconnectFunc = reconnectFunc;
		}
		public void Add(ClientSideHelper helper, int processId) {
			if(!helpers.ContainsKey(processId))
				helpers.Add(processId, helper);
			else
				helpers[processId] = helper;
		}
		public ClientSideHelper Get(IntPtr windowHandle) {
			int processId = GetProcessId(windowHandle);
			if(helpers.ContainsKey(processId)) {
				if(!IsHelperConnected(windowHandle, helpers[processId]))
					reconnectFunc(windowHandle);
				return helpers[processId];
			}
			return null;
		}
		public bool IsConnected(IntPtr windowHandle) {
			int processId = GetProcessId(windowHandle);
			if(helpers.ContainsKey(processId))
				return IsHelperConnected(windowHandle, helpers[processId]);
			return false;
		}
		bool IsHelperConnected(IntPtr windowHandle, ClientSideHelper helper) {
			try {
				return helper != null && helper.CheckConnection(windowHandle);
			} catch(System.Runtime.Remoting.RemotingException) {
				return false;
			}
		}
		[System.Security.SecuritySafeCritical]
		public int GetProcessId(IntPtr windowHandle) {
			int pid;
			GetWindowThreadProcessId(windowHandle, out pid);
			return pid;
		}
		[DllImport("user32.dll")]
		internal static extern uint GetWindowThreadProcessId(IntPtr windowHandle, out int dwProcessId);
	}
}
