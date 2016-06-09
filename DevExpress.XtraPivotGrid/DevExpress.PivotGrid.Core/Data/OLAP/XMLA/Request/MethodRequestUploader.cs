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
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
#if SL
using System.ServiceModel;
#endif
using System.Text;
using System.Threading;
using System.Xml.Linq;
using DevExpress.XtraPivotGrid;
using DevExpress.PivotGrid.Xmla.Helpers;
using System.Xml;
using DevExpress.PivotGrid.Xmla;
namespace DevExpress.PivotGrid.Xmla {
	class XmlaMethodRequestUploader<TResult> where TResult : IXmlaMethodResult {
		UploadRequestCompletedEventHandler<TResult> uploadRequestCompleted;
		XmlaWebClient webClient;
		IEventWaiter eventWaiter;
		public XmlaMethodRequestUploader(string address, Encoding encoding, NetworkCredential credentials, IEventWaiter eventWaiter) {
			this.webClient = new XmlaWebClient(address, encoding, credentials);
			this.webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(this.UploadAsyncCompleted);
			this.eventWaiter = eventWaiter;
		}
		protected XmlaWebClient WebClient { get { return this.webClient; } }
		public event UploadRequestCompletedEventHandler<TResult> UploadRequestCompleted {
			add { uploadRequestCompleted += value; }
			remove { uploadRequestCompleted -= value; }
		}
		public bool UploadRequest(XmlaSoapMessage<TResult> soapMessage, object data, bool isAsync) {
			UserStateData userStateData = new UserStateData(data, soapMessage.CreateMethodResult());
			string msg = soapMessage.Generate();
			if(isAsync) {
				StartUploadAsync(msg, userStateData);
			} else {
				UploadSync(msg, userStateData);
			}
			return true;
		}
		void UploadSync(string xmlaSoapMessage, UserStateData userStateData) {
			this.OnRequestUploadStarting();
			string result = WebClient.UploadString(xmlaSoapMessage);
			UploadRequestCompletedEventArgs<TResult> requestArgs = GetUploadRequestCompletedArgs(result, false, userStateData);
			this.OnRequestUploadCompleted(requestArgs);
		}
		void StartUploadAsync(string xmlaSoapMessage, object userToken) {
			this.OnRequestUploadStarting();
			WebClient.UploadStringAsync(xmlaSoapMessage, userToken);
		}
		protected void UploadAsyncCompleted(object sender, UploadStringCompletedEventArgs args) {
			UserStateData userStateData = args.UserState as UserStateData;
			UploadRequestCompletedEventArgs<TResult> requestArgs = (args.Error == null) ? GetUploadRequestCompletedArgs(args.Result, args.Cancelled, userStateData) :
				new UploadRequestCompletedEventArgs<TResult>(null, args.Error, args.Cancelled, userStateData.Data);
			this.OnRequestUploadCompleted(requestArgs);
		}
		protected void OnRequestUploadStarting() {
			if(this.eventWaiter != null)
				this.eventWaiter.Push();
		}
		protected void OnRequestUploadCompleted(UploadRequestCompletedEventArgs<TResult> args) {
			if(this.uploadRequestCompleted != null)
				this.uploadRequestCompleted(this, args);
			if(this.eventWaiter != null)
				this.eventWaiter.Pop();
		}
		UploadRequestCompletedEventArgs<TResult> GetUploadRequestCompletedArgs(string result, bool cancelled, UserStateData userStateData) {
			XmlaErrors errors = userStateData.EnsureMethodResult(result);
			return new UploadRequestCompletedEventArgs<TResult>(userStateData.MethodResult, errors, null, cancelled, userStateData.Data);
		}
		class UserStateData {
			object data;
			TResult methodResult;
			internal UserStateData(object data, TResult methodResult) {
				this.data = data;
				this.methodResult = methodResult;
			}
			internal object Data { get { return this.data; } }
			internal TResult MethodResult { get { return this.methodResult; } }
			internal XmlaErrors EnsureMethodResult(string resultString) {
				return methodResult.EnsureMethodResult(resultString, data as IResponseFormatProvider);
			}
		}
	}
	class UploadRequestCompletedEventArgs<TResult> : AsyncCompletedEventArgs where TResult : IXmlaMethodResult {
		TResult methodResult;
		XmlaErrors errors;
		public UploadRequestCompletedEventArgs(XmlaErrors errors, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState) {
			this.errors = errors;
		}
		public UploadRequestCompletedEventArgs(TResult result, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState) {
			this.methodResult = result;
		}
		public UploadRequestCompletedEventArgs(TResult result, XmlaErrors errors, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState) {
			this.methodResult = result;
			this.errors = errors;
		}
		public XmlaErrors Errors { get { return this.errors; } }
		public TResult MethodResult {
			get {
				base.RaiseExceptionIfNecessary();
				return this.methodResult;
			}
		}
	}
	delegate void UploadRequestCompletedEventHandler<TResult>(object sender, UploadRequestCompletedEventArgs<TResult> e) where TResult : IXmlaMethodResult;
	class XmlaWebClient {
		const string ContentType = "text/xml";
		Uri uriAddress;
		WebClient client;
		internal XmlaWebClient(string address, Encoding encoding, NetworkCredential credentials)
			: base() {
			this.client = new WebClient();
			try {
				this.uriAddress = new Uri(address);
			} catch(System.UriFormatException e) {
				throw new DevExpress.XtraPivotGrid.Data.XmlaErrorResponseException(null, e);
			}
			this.Initialize(encoding, credentials);
		}
		void Initialize(Encoding encoding, NetworkCredential credentials) {
			this.client.Encoding = encoding;
			this.client.Headers["Content-Type"] = ContentType;
			if(credentials == null) {
				this.client.Credentials = null;
				this.client.UseDefaultCredentials = true;
			} else {
				this.client.UseDefaultCredentials = false;
				this.client.Credentials = credentials;
			}
		}
		public event UploadStringCompletedEventHandler UploadStringCompleted {
			add { this.client.UploadStringCompleted += value; }
			remove { this.client.UploadStringCompleted -= value; }
		}
		public string UploadString(string data) {
			return this.client.UploadString(uriAddress, data);
		}
		public void UploadStringAsync(string data, object userToken) {
			this.client.UploadStringAsync(this.uriAddress, null, data, userToken);
		}
	}
	class EventWaiterCreator {
		internal static IEventWaiter Create(bool isAsync) {
			if(isAsync)
				return new EventWaiter();
			return new DummyEventWaiter();
		}
	}
	interface IEventWaiter: IDisposable {
		void InvokeAndWait(ProcessEntity process, IOLAPUniqueEntity entity);
		void Push();
		void Pop();
		void Finish(bool force);
	}
	delegate void ProcessEntity(IOLAPUniqueEntity entity);
	class EventWaiter : IEventWaiter {
		ManualResetEvent manualResetEvent;
		int completedCallCount;
		public EventWaiter() {
			this.completedCallCount = 0;
			this.manualResetEvent = new ManualResetEvent(true);
		}
		public void InvokeAndWait(ProcessEntity process, IOLAPUniqueEntity entity) {
			this.Start();
			process.Invoke(entity);
			this.Wait();
		}
		void Start() {
			this.manualResetEvent.Reset();
		}
		public void Finish(bool force) {
			if(force || completedCallCount == 0) {
				manualResetEvent.Set();
			}
		}
		void Wait() {
			WaitHandle.WaitAny(new WaitHandle[] { manualResetEvent });
		}
		public void Push() {
			Interlocked.Increment(ref completedCallCount);
		}
		public void Pop() {
			Interlocked.Decrement(ref completedCallCount);
			if(completedCallCount < 0)
				throw new ArgumentException("EventWaiter: some operation has not been completed (completedCallCount < 0)");
			Finish(false);
		}
		#region IDisposable Members
		public void Dispose() {
			this.completedCallCount = 0;
			((IDisposable)this.manualResetEvent).Dispose();
			this.manualResetEvent = null;
		}
		#endregion
	}
	class DummyEventWaiter : IEventWaiter {
		public DummyEventWaiter() { }
		public void InvokeAndWait(ProcessEntity process, IOLAPUniqueEntity entity) {
			process.Invoke(entity);
		}
		public void Push() { }
		public void Pop() { }
		public void Finish(bool force) { }
		#region IDisposable Members
		public void Dispose() { }
		#endregion
	}
}
