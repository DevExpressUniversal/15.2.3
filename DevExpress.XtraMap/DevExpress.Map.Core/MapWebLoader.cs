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
using System.IO;
#if DXPORTABLE
	using System.Net.Http;
	namespace DevExpress.Map.Native {
		public class MapWebLoaderThreadArgs {
			public Uri Address { get; set; }
			public object UserToken { get; set; }
		}
		public class MapWebLoader : IDisposable {
			HttpClient httpCLient;
			public event EventHandler<MapWebLoaderEventArgs> LoadComlete;
			public MapWebLoader() {
				httpCLient = new HttpClient();
		}
		async void LoadMethod(object args) {
			MapWebLoaderThreadArgs mapWebLoader = (MapWebLoaderThreadArgs)args;
			MapWebLoaderEventArgs eventArgs = new MapWebLoaderEventArgs();
			eventArgs.UserState = mapWebLoader.UserToken;
			try {
				Stream stream = await httpCLient.GetStreamAsync(mapWebLoader.Address);
				eventArgs.Stream = stream;
			} catch (Exception e) {
				eventArgs.Error = e;
			}
			if (LoadComlete != null)
				LoadComlete(this, eventArgs);
		}
		public void ReadAsync(Uri uri, object userToken = null) {
			LoadMethod(new MapWebLoaderThreadArgs() { Address = uri, UserToken = userToken });
		}
		public void ReadAsync(Uri uri) {
			ReadAsync(uri, null);
		}
		public void Dispose() {
			httpCLient.Dispose();
		}
	}
}
#else
	using System.Net;
	namespace DevExpress.Map.Native {
		public class MapWebLoader : IDisposable {
			WebClient client;
			public event EventHandler<MapWebLoaderEventArgs> LoadComlete;
			public MapWebLoader() {
				client = new WebClient();
			}
			void OnOpenReadCompleted(object sender, OpenReadCompletedEventArgs e) {
				MapWebLoaderEventArgs eventArgs = new MapWebLoaderEventArgs();
				eventArgs.Error = e.Error;
				eventArgs.Cancelled = e.Cancelled;
				eventArgs.UserState = e.UserState;
				eventArgs.Stream = e.Result;
				if (LoadComlete != null)
					LoadComlete(sender, eventArgs);
			}
			public void ReadAsync(Uri uri, object userToken) {
				client.OpenReadCompleted += OnOpenReadCompleted;
				client.OpenReadAsync(uri, userToken);
			}
			public void ReadAsync(Uri uri) {
				this.ReadAsync(uri, null);
			}
			public void Dispose() {
				client.Dispose();
			}
		}
	}
#endif
namespace DevExpress.Map.Native {
	public class MapWebLoaderEventArgs : EventArgs {
		public Stream Stream { get; set; }
		public Exception Error { get; set; }
		public bool Cancelled { get; set; }
		public object UserState { get; set; }
	}
}
