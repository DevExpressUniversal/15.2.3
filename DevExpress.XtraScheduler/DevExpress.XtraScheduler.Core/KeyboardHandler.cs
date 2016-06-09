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
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Native;
#if !SL
using System.Windows.Forms;
#else
using DevExpress.Data;
#endif
namespace DevExpress.XtraScheduler.Native {
	#region SchedulerKeyHashProvider
	public class SchedulerKeyHashProvider : IKeyHashProvider {
		#region Fields
		SchedulerViewType viewType;
		SchedulerGroupType groupType;
		#endregion
		public SchedulerKeyHashProvider(SchedulerViewType viewType, SchedulerGroupType groupType) {
			this.viewType = viewType;
			this.groupType = groupType;
		}
		#region Properties
		public SchedulerViewType ViewType { get { return viewType; } set { viewType = value; } }
		public SchedulerGroupType GroupType { get { return groupType; } set { groupType = value; } }
		#endregion
		#region IKeyHashProvider Members
		public Int64 CreateHash(Int64 keyData) {
			Int64 result = keyData; 
			result |= (((Int64)groupType) << (32 + 20));
			result |= (((Int64)viewType) << (32 + 22));
			return result;
		}
		#endregion
	}
	#endregion
	#region NormalKeyboardHandlerBase (abstract class)
	public abstract class NormalKeyboardHandlerBase : CommandBasedKeyboardHandler<SchedulerCommandId> {
		#region Properties
		public InnerSchedulerViewBase InnerView { get { return (InnerSchedulerViewBase)Context; } set { Context = value; } }
		protected internal abstract InnerSchedulerControl InnerControl { get; }
		protected internal abstract ISchedulerCommandTarget ISchedulerCommandTarget { get; }
		#endregion
		public override bool HandleKeyPress(char character, Keys modifier) {
			if (base.HandleKeyPress(character, modifier))
				return true;
			if ((modifier & Keys.Alt) == 0 && (modifier & Keys.Control) == 0) {
				Command command = new NewAppointmentViaInplaceEditorCommand(InnerControl, new String(character, 1));
				command.CommandSourceType = CommandSourceType.Keyboard;
				command.Execute();
				return true;
			}
			else
				return false;
		}
		protected internal virtual void RegisterKeyHandlerForAllGroupTypes(SchedulerViewType viewType, Keys key, Keys modifier, SchedulerCommandId commandId) {
			Int64 keyData = KeysToInt64KeyData(key | modifier);
			RegisterKeyHandlerForAllGroupTypesCore(viewType, keyData, commandId);
		}
		protected internal virtual void RegisterKeyHandlerForAllGroupTypesCore(SchedulerViewType viewType, Int64 keyData, SchedulerCommandId commandId) {
			RegisterKeyHandlerCore(new SchedulerKeyHashProvider(viewType, SchedulerGroupType.None), keyData, commandId);
			RegisterKeyHandlerCore(new SchedulerKeyHashProvider(viewType, SchedulerGroupType.Date), keyData, commandId);
			RegisterKeyHandlerCore(new SchedulerKeyHashProvider(viewType, SchedulerGroupType.Resource), keyData, commandId);
		}
		protected internal virtual void RegisterKeyHandlerForAllViewsAndGroupTypes(SchedulerViewRepositoryBase views, Keys key, Keys modifier, SchedulerCommandId commandId) {
			Int64 keyData = KeysToInt64KeyData(key | modifier);
			RegisterKeyHandlerForAllViewsAndGroupTypesCore(views, keyData, commandId);
		}
		protected internal virtual void RegisterKeyHandlerForAllViewsAndGroupTypes(SchedulerViewRepositoryBase views, char key, Keys modifier, SchedulerCommandId commandId) {
			Int64 keyData = CharToInt64KeyData(key, modifier);
			RegisterKeyHandlerForAllViewsAndGroupTypesCore(views, keyData, commandId);
		}
		protected internal virtual void RegisterKeyHandlerForAllViewsAndGroupTypesCore(SchedulerViewRepositoryBase views, Int64 keyData, SchedulerCommandId commandId) {
			int count = views.Count;
			for (int i = 0; i < count; i++) {
				InnerSchedulerViewBase view = views.GetInnerView(i);
				RegisterKeyHandlerForAllGroupTypesCore(view.Type, keyData, commandId);
			}
		}
		protected override void ValidateHandlerId(SchedulerCommandId commandId) {
			if (commandId == SchedulerCommandId.None)
				Exceptions.ThrowArgumentException("commandId", commandId);
		}
		public override Command CreateHandlerCommand(SchedulerCommandId commandId) {
			return InnerControl.Owner.CreateCommand(commandId);
		}
		protected override IKeyHashProvider CreateKeyHashProviderFromContext() {
			SchedulerGroupType groupType = !InnerView.CanShowResources() ? SchedulerGroupType.None : InnerView.GroupType;
			return new SchedulerKeyHashProvider(InnerView.Type, groupType);
		}
	}
	#endregion
	#region EscapeKeyboardHandler
	public class EscapeKeyboardHandler : KeyboardHandler {
		readonly MouseHandlerState mouseHandlerState;
		public EscapeKeyboardHandler(MouseHandlerState mouseHandlerState) {
			this.mouseHandlerState = mouseHandlerState;
		}
		protected MouseHandlerState MouseHandlerState { get { return mouseHandlerState; } }
		public override bool HandleKey(Keys keyData) {
			if (keyData == Keys.Escape) {
				mouseHandlerState.OnCancelState();
				return true;
			}
			else
				return false;
		}
	}
	#endregion
	#region KeyStateKeyboardHandler
	public class KeyStateKeyboardHandler : EscapeKeyboardHandler {
		KeyState previousKeyState;
		public KeyStateKeyboardHandler(MouseHandlerState mouseHandlerState)
			: base(mouseHandlerState) {
			previousKeyState = KeyState;
		}
		public override bool HandleKey(Keys keyData) {
			if (CheckKeyStateChanged())
				return true;
			return base.HandleKey(keyData);
		}
		public override bool HandleKeyUp(Keys keys) {
			if (CheckKeyStateChanged())
				return true;
			return base.HandleKeyUp(keys);
		}
		private bool CheckKeyStateChanged() {
			KeyState keyState = KeyState;
			if (keyState != previousKeyState) {
				previousKeyState = keyState;
				MouseHandlerState.OnKeyStateChanged(keyState);
				previousKeyState = KeyState;
				return true;
			}
			return false;
		}
	}
	#endregion
}
