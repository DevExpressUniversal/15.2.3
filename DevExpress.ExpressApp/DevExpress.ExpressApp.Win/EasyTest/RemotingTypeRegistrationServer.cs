#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Security;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Win.EasyTest {
	public class XafEasyTestReceiver : MarshalByRefObject {
		public void ReceiveData(string data) {
			DevExpress.Persistent.Base.Tracing.Tracer.LogText("XafEasyTestReceiver get data.");
			if(!string.IsNullOrEmpty(data)) {
				string[] splitedData = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				if(splitedData.Length == 2) {
					string name = Path.GetFileName(splitedData[0]);
					string dir = Path.GetDirectoryName(splitedData[0]);
					Assembly assembly = ReflectionHelper.GetAssembly(name, dir);
					Type type = assembly.GetType(splitedData[1]);
					RemotingConfiguration.RegisterWellKnownServiceType(type, type.Name, WellKnownObjectMode.Singleton);
					EasyTestRemotingRegistration.IsRegistered = true;
					DevExpress.Persistent.Base.Tracing.Tracer.LogText("EasyTestRemotingRegistration is OK.");
				}
			}
			else {
				throw new Exception("Received data is null");
			}
		}
		[SecurityCritical]
		public override object InitializeLifetimeService() {
			return null;
		}
	}
	public class EasyTestRemotingRegistration {
		public static bool IsRegistered { get; set; }
		public static void Register() {
			try {
				DevExpress.Persistent.Base.Tracing.Tracer.LogText(">>EasyTestRemotingRegistration Register");
				int port = 0;
				string communicationPort = ConfigurationManager.AppSettings["EasyTestCommunicationPort"];
				if(string.IsNullOrEmpty(communicationPort) || !int.TryParse(communicationPort, out port)) {
					port = 4100;
				}
				CreateServerChanel(port);
				RemotingConfiguration.RegisterWellKnownServiceType(typeof(XafEasyTestReceiver), typeof(XafEasyTestReceiver).Name, WellKnownObjectMode.Singleton);
				DevExpress.Persistent.Base.Tracing.Tracer.LogText("<<EasyTestRemotingRegistration Register");
			}
			catch(Exception e) {
				DevExpress.Persistent.Base.Tracing.Tracer.LogText(DevExpress.Persistent.Base.Tracing.Tracer.FormatExceptionReport(e));
			}
		}
		[SecuritySafeCritical]
		private static void CreateServerChanel(int port) {
			DevExpress.Persistent.Base.Tracing.Tracer.LogText("CreateServerChanel port: " + port.ToString());
			BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
			provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
			Hashtable props = new Hashtable();
			props.Add("name", "XafEasyTestServerReceiver");
			props.Add("port", port);
			ChannelServices.RegisterChannel(new System.Runtime.Remoting.Channels.Tcp.TcpServerChannel(props, provider, null), false);
		}
	}
}
