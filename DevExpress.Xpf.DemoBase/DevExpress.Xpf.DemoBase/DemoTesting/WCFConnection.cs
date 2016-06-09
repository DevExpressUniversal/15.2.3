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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Diagnostics;
#if (WPF || SL) && !DEMO
namespace DevExpress.Xpf.DemoBase.DemoTesting {
#else
namespace BuildTester.Tester {
#endif
	[ServiceContract]
	public interface IWPFDemoTestService {
		[OperationContract, FaultContract(typeof(Exception))]
		void OnError(string error);
		[OperationContract, FaultContract(typeof(Exception))]
		string GetFixtureClassName();
		[OperationContract, FaultContract(typeof(Exception))]
		void IAmAlive(string currentState);
		[OperationContract, FaultContract(typeof(Exception))]
		void TestComplete();
	}
	[ServiceContract(Name = "IWPFDemoTestService")]
	public interface IAsyncWPFDemoTestService {
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginOnError(string error, System.AsyncCallback callback, object asyncState);
		void EndOnError(System.IAsyncResult result);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetFixtureClassName(System.AsyncCallback callback, object asyncState);
		string EndGetFixtureClassName(System.IAsyncResult result);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginIAmAlive(string currentState, System.AsyncCallback callback, object asyncState);
		void EndIAmAlive(System.IAsyncResult result);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginTestComplete(System.AsyncCallback callback, object asyncState);
		void EndTestComplete(System.IAsyncResult result);
	}
	public static class ServiceHelper {
		public const string SecureServiceUri = "http://localhost/SiteTest/WPFDemoTestService";
		internal const string ServiceUri = "http://localhost:7890/WPFDemoTestService";
		internal static Binding GetBinding() {
			return GetBinding(false);
		}
		internal static Binding GetBinding(bool isSL) {
			return CreateBinding(isSL);
		}
		static Binding CreateBinding(bool isSL) {
			if(!isSL) {
				WSHttpBinding binding = new WSHttpBinding();
				binding.Security.Mode = SecurityMode.None;
				binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
				binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
				binding.ReaderQuotas.MaxDepth = int.MaxValue;
				binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
				binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
				binding.MaxReceivedMessageSize = int.MaxValue;
				binding.OpenTimeout = new TimeSpan(0, 10, 0);
				binding.CloseTimeout = new TimeSpan(0, 10, 0);
				binding.SendTimeout = new TimeSpan(0, 10, 0);
				binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
				return binding;
			} else {
				return CreateBasicBinding();
			}
		}
		static BasicHttpBinding CreateBasicBinding() {
			return new BasicHttpBinding();
		}
	}
}
