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
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.WindowsUI.Base;
namespace DevExpress.Xpf.WindowsUI.Navigation {
	public class NavigationContentProvider : INavigationContentProvider {
		static object LoadContent(string source) {
			if(source == null) return null;
			Type type = TypeProvider.Current.GetTypeByName(source);
			return type != null ? LoadByType(type) : null;
		}
		static object LoadContent(object source) {
			object content = null;
			if(source is Uri)
				return XamlLoaderHelper.LoadContentFromUri((Uri)source);
			if(source is Type)
				return LoadByType((Type)source);
			if(content == null) content = LoadContent(source as string);
			if(content == null) content = source;
			return content;
		}
		static object LoadByType(Type type) {
			object content = Activator.CreateInstance(type);
			return content;
		}
		#region INavigationContentProvider Members
		public object Load(object source) {
			return LoadContent(source);
		}
		public IAsyncResult BeginLoad(object source, AsyncCallback userCallback, object asyncState) {
			NavigationContentProviderAsyncResult result = new NavigationContentProviderAsyncResult(source, asyncState);
			if(source == null) {
				result.Exception = new ArgumentNullException("source");
			}
			Journal.NavigationOperation op = asyncState as Journal.NavigationOperation;
#if !SILVERLIGHT
				Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => LoadAsync(userCallback, result, op)));
#else
				Deployment.Current.Dispatcher.BeginInvoke(new Action(() => LoadAsync(userCallback, result, op)));
#endif
				return result;
		}
		void LoadAsync(AsyncCallback loadedAction, NavigationContentProviderAsyncResult result, Journal.NavigationOperation op) {
			if (result.Exception != null) {
				result.IsCompleted = true;
			}
			try {
				result.Content = LoadContent(result.Source);
			}
			catch(Exception ex) {
				result.Exception = ex;
			}
			finally {
				result.IsCompleted = true;
				if(loadedAction != null) {
					loadedAction(result);
				}
			}
		}
		public void CancelLoad(IAsyncResult asyncResult) {
		}
		public LoadResult EndLoad(IAsyncResult asyncResult) {
			AssertionException.IsNotNull(asyncResult);
			NavigationContentProviderAsyncResult result = asyncResult as NavigationContentProviderAsyncResult;
			if(result == null) {
				throw new InvalidOperationException();
			}
			if(result.Exception != null) {
				throw result.Exception;
			}
			return new LoadResult(result.Content);
		}
		public bool CanLoad(object source) {
			return true;
		}
		#endregion
		class NavigationContentProviderAsyncResult : IAsyncResult {
			internal NavigationContentProviderAsyncResult(object source, object asyncState) {
				AsyncState = asyncState;
				this.Source = source;
			}
			internal object Source { get; set; }
			internal Exception Exception { get; set; }
			internal object Content { get; set; }
			#region IAsyncResult Members
			public object AsyncState { get; private set; }
			public WaitHandle AsyncWaitHandle { get { return null; } }
			public bool CompletedSynchronously { get { return false; } }
			public bool IsCompleted { get; internal set; }
			#endregion
		}
	}
	public class LoadResult {
		public LoadResult(object loadedContent) {
			this.LoadedContent = loadedContent;
		}
		public object LoadedContent { get; private set; }
	}
}
