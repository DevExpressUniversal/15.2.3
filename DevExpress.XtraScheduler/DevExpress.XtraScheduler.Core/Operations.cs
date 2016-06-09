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
using System.ComponentModel.Design;
using DevExpress.Services;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Operations;
using DevExpress.Utils;
#if !SL
using System.Windows.Forms;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentKeyEventArgs = System.Windows.Forms.KeyEventArgs;
using PlatformIndependentKeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;
#else
using System.Windows.Media;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentKeyEventArgs = DevExpress.Data.KeyEventArgs;
using PlatformIndependentKeyPressEventArgs = DevExpress.Data.KeyPressEventArgs;
#endif
namespace DevExpress.XtraScheduler.Operations {
	#region IOperation
	public interface IOperation {
		void Start();
		void Finish();
	}
	#endregion
	#region InputOperation (abstract class)
	public abstract class InputOperation : IOperation {
		#region Fields
		IServiceContainer serviceContainer;
		IMouseHandlerService oldMouseHandler;
		IKeyboardHandlerService oldKeyboardHandler;
		#endregion
		protected InputOperation(IServiceContainer serviceContainer) {
			Guard.ArgumentNotNull(serviceContainer, "serviceContainer");
			this.serviceContainer = serviceContainer;
		}
		#region Properties
		public IServiceContainer ServiceContainer { get { return serviceContainer; } }
		internal IMouseHandlerService OldMouseHandler { get { return oldMouseHandler; } }
		internal IKeyboardHandlerService OldKeyboardHandler { get { return oldKeyboardHandler; } }
		#endregion
		#region IOperation Members
		public virtual void Start() {
			SubstituteMouseHandlerService();
			SubstituteKeyboardHandlerService();
		}
		public virtual void Finish() {
			RestoreMouseHandlerService();
			RestoreKeyboardHandlerService();
		}
		public virtual void Execute() {
		}
		#endregion
		#region SubstituteMouseHandlerService
		protected internal virtual void SubstituteMouseHandlerService() {
			this.oldMouseHandler = (IMouseHandlerService)ServiceContainer.GetService(typeof(IMouseHandlerService));
			ServiceContainer.RemoveService(typeof(IMouseHandlerService));
			ServiceContainer.AddService(typeof(IMouseHandlerService), new OperationMouseHandlerService(this, oldMouseHandler));
		}
		#endregion
		#region SubstituteKeyboardHandlerService
		protected internal virtual void SubstituteKeyboardHandlerService() {
			this.oldKeyboardHandler = (IKeyboardHandlerService)ServiceContainer.GetService(typeof(IKeyboardHandlerService));
			ServiceContainer.RemoveService(typeof(IKeyboardHandlerService));
			ServiceContainer.AddService(typeof(IKeyboardHandlerService), new OperationKeyboardHandlerService(this, oldKeyboardHandler));
		}
		#endregion
		#region RestoreMouseHandlerService
		protected internal virtual void RestoreMouseHandlerService() {
			ServiceContainer.RemoveService(typeof(IMouseHandlerService));
			ServiceContainer.AddService(typeof(IMouseHandlerService), this.oldMouseHandler);
		}
		#endregion
		#region RestoreKeyboardHandlerService
		protected internal virtual void RestoreKeyboardHandlerService() {
			ServiceContainer.RemoveService(typeof(IKeyboardHandlerService));
			ServiceContainer.AddService(typeof(IKeyboardHandlerService), this.oldKeyboardHandler);
		}
		#endregion
		#region OnMouseDown
		protected internal virtual void OnMouseDown(PlatformIndependentMouseEventArgs e) {
		}
		#endregion
		#region OnMouseMove
		protected internal virtual void OnMouseMove(PlatformIndependentMouseEventArgs e) {
		}
		#endregion
		#region OnMouseUp
		protected internal virtual void OnMouseUp(PlatformIndependentMouseEventArgs e) {
		}
		#endregion
		#region OnKeyDown
		protected internal virtual void OnKeyDown(PlatformIndependentKeyEventArgs e) {
		}
		#endregion
		#region OnKeyPress
		protected internal virtual void OnKeyPress(PlatformIndependentKeyPressEventArgs e) {
		}
		#endregion
		#region OnKeyUp
		protected internal virtual void OnKeyUp(PlatformIndependentKeyEventArgs e) {
		}
		#endregion
		#region OnMouseWeel
		protected internal virtual void OnMouseWheel(PlatformIndependentMouseEventArgs e) {
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region OperationMouseHandlerService
	public class OperationMouseHandlerService : MouseHandlerServiceWrapper {
		readonly InputOperation operation;
		#region AppointmentToolMouseHandlerService
		public OperationMouseHandlerService(InputOperation operation, IMouseHandlerService service)
			: base(service) {
			Guard.ArgumentNotNull(operation, "operation");
			this.operation = operation;
		}
		#endregion
		internal InputOperation Operation { get { return operation; } }
		#region OnMouseDown
		public override void OnMouseDown(PlatformIndependentMouseEventArgs e) {
			Operation.OnMouseDown(e);
		}
		#endregion
		#region OnMouseMove
		public override void OnMouseMove(PlatformIndependentMouseEventArgs e) {
			Operation.OnMouseMove(e);
		}
		#endregion
		#region OnMouseUp
		public override void OnMouseUp(PlatformIndependentMouseEventArgs e) {
			Operation.OnMouseUp(e);
		}
		#endregion
		#region OnMouseWeel
		public override void OnMouseWheel(PlatformIndependentMouseEventArgs e) {
			Operation.OnMouseWheel(e);
		}
		#endregion
	}
	#endregion
	#region OperationKeyboardHandlerService
	public class OperationKeyboardHandlerService : KeyboardHandlerServiceWrapper {
		readonly InputOperation operation;
		public OperationKeyboardHandlerService(InputOperation operation, IKeyboardHandlerService service)
			: base(service) {
			Guard.ArgumentNotNull(operation, "operation");
			this.operation = operation;
		}
		internal InputOperation Operation { get { return operation; } }
		#region OnKeyDown
		public override void OnKeyDown(PlatformIndependentKeyEventArgs e) {
			Operation.OnKeyDown(e);
		}
		#endregion
		#region OnKeyUp
		public override void OnKeyUp(PlatformIndependentKeyEventArgs e) {
			Operation.OnKeyUp(e);
		}
		#endregion
		#region OnKeyPress
		public override void OnKeyPress(PlatformIndependentKeyPressEventArgs e) {
			Operation.OnKeyPress(e);
		}
		#endregion
	}
	#endregion
}
