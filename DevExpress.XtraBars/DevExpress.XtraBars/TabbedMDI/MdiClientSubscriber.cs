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
using System.Windows.Forms;
namespace DevExpress.Utils.Mdi {
	public interface IMdiClientListener {
		void OnControlAdded(Control control);
		void OnControlRemoved(Control control);
		void OnVisibleChanged(bool visible);
		void OnDisposed();
	}
	public interface IMdiClientSubscriber : IDisposable {
		void Ensure(MdiClient client);
	}
	public class MdiClientSubscriber : IMdiClientSubscriber {
		public IMdiClientListener Listener;
		protected MdiClient MdiClient;
		public MdiClientSubscriber(MdiClient client, IMdiClientListener listener) {
			Listener = listener;
			MdiClient = client;
			if(MdiClient != null)
				Subscribe();
		}
		public void Dispose() {
			if(MdiClient != null)
				UnSubscribe();
			if(MdiClient.MdiChildren != null) {
				if(!XtraBars.Docking2010.Views.DocumentsHostContext.IsClosing(MdiClient.Parent)) {
					foreach(var child in MdiClient.MdiChildren)
						child.Dispose();
				}
			}
			MdiClient = null;
			Listener = null;
			GC.SuppressFinalize(this);
		}
		public void Ensure(MdiClient client) {
			if(MdiClient != null)
				UnSubscribe();
			if(MdiClient != client) {
				if(MdiClient.MdiChildren != null) {
					foreach(var child in MdiClient.MdiChildren)
						child.Dispose();
				}
				MdiClient = client;
			}
			if(MdiClient != null)
				Subscribe();
		}
		protected virtual void Subscribe() {
			MdiClient.VisibleChanged += MdiClient_VisibleChanged;
			MdiClient.ControlAdded += MdiClient_ControlAdded;
			MdiClient.ControlRemoved += MdiClient_ControlRemoved;
			MdiClient.Disposed += MdiClient_Disposed;
		}
		protected virtual void UnSubscribe() {
			MdiClient.VisibleChanged -= MdiClient_VisibleChanged;
			MdiClient.ControlAdded -= MdiClient_ControlAdded;
			MdiClient.ControlRemoved -= MdiClient_ControlRemoved;
			MdiClient.Disposed -= MdiClient_Disposed;
		}
		void MdiClient_Disposed(object sender, EventArgs e) {
			Listener.OnDisposed();
		}
		void MdiClient_ControlAdded(object sender, ControlEventArgs e) {
			Listener.OnControlAdded(e.Control);
		}
		void MdiClient_ControlRemoved(object sender, ControlEventArgs e) {
			Listener.OnControlRemoved(e.Control);
		}
		bool visible;
		void MdiClient_VisibleChanged(object sender, EventArgs e) {
			bool mdiClientVisible = MdiClient.Visible;
			if(visible != mdiClientVisible)
				Listener.OnVisibleChanged(mdiClientVisible);
			visible = mdiClientVisible;
		}
	}
}
