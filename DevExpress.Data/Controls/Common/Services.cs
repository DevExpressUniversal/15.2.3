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
using System.ComponentModel.Design;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Utils.Commands;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Windows.Forms;
#if SILVERLIGHT
using DevExpress.Data;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
#elif DXPORTABLE
using PlatformIndependentMouseEventArgs = DevExpress.Compatibility.System.Windows.Forms.MouseEventArgs;
#else
using System.Windows.Forms;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using System.ComponentModel;
#endif
namespace DevExpress.Services {
	public interface IKeyboardHandlerService {
		void OnKeyDown(KeyEventArgs e);
		void OnKeyUp(KeyEventArgs e);
		void OnKeyPress(KeyPressEventArgs e);
	}
	public interface IMouseHandlerService {
		void OnMouseMove(MouseEventArgs e);
		void OnMouseDown(MouseEventArgs e);
		void OnMouseUp(MouseEventArgs e);
		void OnMouseWheel(MouseEventArgs e);
	}
	public class KeyboardHandlerServiceWrapper : IKeyboardHandlerService {
		IKeyboardHandlerService service;
		public KeyboardHandlerServiceWrapper(IKeyboardHandlerService service) {
			Guard.ArgumentNotNull(service, "service");
			this.service = service;
		}
		public IKeyboardHandlerService Service { get { return service; } }
		#region IKeyboardHandlerService Members
		public virtual void OnKeyDown(KeyEventArgs e) {
			Service.OnKeyDown(e);
		}
		public virtual void OnKeyUp(KeyEventArgs e) {
			Service.OnKeyUp(e);
		}
		public virtual void OnKeyPress(KeyPressEventArgs e) {
			Service.OnKeyPress(e);
		}
		#endregion
	}
	public class MouseHandlerServiceWrapper : IMouseHandlerService {
		IMouseHandlerService service;
		public MouseHandlerServiceWrapper(IMouseHandlerService service) {
			Guard.ArgumentNotNull(service, "service");
			this.service = service;
		}
		public IMouseHandlerService Service { get { return service; } }
		#region IMouseHandlerService Members
		public virtual void OnMouseMove(MouseEventArgs e) {
			Service.OnMouseMove(e);
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			Service.OnMouseDown(e);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			Service.OnMouseUp(e);
		}
		public virtual void OnMouseWheel(MouseEventArgs e) {
			Service.OnMouseWheel(e);
		}
		#endregion
	}
}
namespace DevExpress.Services.Implementation {
	public abstract class KeyboardHandlerService : IKeyboardHandlerService {
		readonly KeyboardHandler handler;
		protected KeyboardHandlerService(KeyboardHandler handler) {
			Guard.ArgumentNotNull(handler, "handler");
			this.handler = handler;
		}
		public abstract object CreateContext();
		protected virtual KeyboardHandler Handler { get { return handler; } }
		#region IKeyboardHandlerService Members
		public virtual void OnKeyDown(KeyEventArgs e) {
			Handler.Context = CreateContext();
#if !SILVERLIGHT
			e.Handled = Handler.HandleKey(e.KeyData);
#if DEBUG
			System.Diagnostics.Debug.WriteLine(String.Format("{0}", e.KeyData));
#endif
#else
			e.Handled = Handler.HandleKey(e.Keys);
#endif
		}
		public virtual void OnKeyUp(KeyEventArgs e) {
			Handler.Context = CreateContext();
#if !SILVERLIGHT
			e.Handled = Handler.HandleKeyUp(e.KeyData);
#else
			e.Handled = Handler.HandleKeyUp(e.Keys);
#endif
		}
		public virtual void OnKeyPress(KeyPressEventArgs e) {
			if(Handler.IsValidChar(e.KeyChar)) {
#if DEBUG
				System.Diagnostics.Debug.WriteLine(String.Format("{0}", e.KeyChar));
#endif
				Handler.Context = CreateContext();
#if SILVERLIGHT
				Handler.HandleKeyPress(e.KeyChar, new Keys(e.Modifier));
#elif DXPORTABLE
				Handler.HandleKeyPress(e.KeyChar, e.ModifierKeys);
#else
				Handler.HandleKeyPress(e.KeyChar, System.Windows.Forms.Control.ModifierKeys);
#endif
			}
		}
		#endregion
	}
	#region MouseHandlerService
	public class MouseHandlerService : IMouseHandlerService {
		MouseHandler handler;
		public MouseHandlerService(MouseHandler handler) {
			Guard.ArgumentNotNull(handler, "handler");
			this.handler = handler;
		}
		public virtual MouseHandler Handler { get { return handler; } }
		#region IMouseHandlerService Members
		public virtual void OnMouseMove(PlatformIndependentMouseEventArgs e) {
			Handler.OnMouseMove(e);
		}
		public virtual void OnMouseDown(PlatformIndependentMouseEventArgs e) {
			Handler.OnMouseDown(e);
		}
		public virtual void OnMouseUp(PlatformIndependentMouseEventArgs e) {
			Handler.OnMouseUp(e);
		}
		public virtual void OnMouseWheel(PlatformIndependentMouseEventArgs e) {
			Handler.OnMouseWheel(e);
		}
		#endregion
	}
	#endregion
	#region CommandExecutionListenerService
	public class CommandExecutionListenerService : ICommandExecutionListenerService, IBatchUpdateable, IBatchUpdateHandler {
		BatchUpdateHelper batchUpdateHelper;
		public CommandExecutionListenerService() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
		}
		#region IBatchUpdateable Members
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return BatchUpdateHelper.IsUpdateLocked; } }
		BatchUpdateHelper BatchUpdateHelper { get { return batchUpdateHelper; } }
		public void BeginUpdate() {
			BatchUpdateHelper.BeginUpdate();
		}
		public void CancelUpdate() {
			BatchUpdateHelper.EndUpdate();
		}
		public void EndUpdate() {
			BatchUpdateHelper.EndUpdate();
		}
		#endregion
		#region IBatchUpdateHandler Members
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastEndUpdateCore();
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdateCore();
		}
		#endregion
		#region ICommandExecutionListenerService Members
		public virtual void BeginCommandExecution(Command command, ICommandUIState state) {
			BeginUpdate();
		}
		public virtual void EndCommandExecution(Command command, ICommandUIState state) {
			EndUpdate();
		}
		#endregion
		protected internal virtual void OnLastEndUpdateCore() {
		}
	}
	#endregion
}
