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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Actions {
	public class ParametrizedActionExecuteEventArgs : SimpleActionExecuteEventArgs {
		private object parameterCurrentValue;
		public ParametrizedActionExecuteEventArgs(ActionBase action, ISelectionContext context, object parameterCurrentValue)
			: base(action, context) {
			this.parameterCurrentValue = parameterCurrentValue;
		}
		public object ParameterCurrentValue {
			get { return parameterCurrentValue; }
		}
	}
	public delegate void ParametrizedActionExecuteEventHandler(object sender, ParametrizedActionExecuteEventArgs e);
	[DXToolboxItem(true)]
	[ToolboxBitmap(typeof(XafApplication), "Resources.Actions.Action_ParametrizedAction.bmp")]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafActions)]
	public class ParametrizedAction : ActionBase {
		private Type valueType = typeof(string);
		private object currentValue;
		protected virtual void OnValueChanged() {
			if(ValueChanged != null && (LockCount == 0))
				ValueChanged(this, EventArgs.Empty);
		}
		protected override void OnDeactivated() {
			currentValue = null;
			base.OnDeactivated();
		}
		protected internal override void RaiseExecute(ActionBaseEventArgs eventArgs) {
			Execute(this, (ParametrizedActionExecuteEventArgs)eventArgs);
		}
		protected override void LogActionInfo() {
			base.LogActionInfo();
			Tracing.Tracer.LogValue("CurrentValue", currentValue);
		}
		protected override void ReleaseLockedEvents() {
			base.ReleaseLockedEvents();
			OnValueChanged();
		}
		public ParametrizedAction() : this(null) { }
		public ParametrizedAction(IContainer container) : base(container) { }
		public ParametrizedAction(Controller owner, string id, PredefinedCategory category, Type valueType)
			: this(owner, id, category.ToString(), valueType) { }
		public ParametrizedAction(Controller owner, string id, string category, Type valueType)
			: base(owner, id, category) {
			this.valueType = valueType;
		}
		public void DoExecute(object parameterNewValue) {
			Value = parameterNewValue;
			ExecuteCore(Execute, new ParametrizedActionExecuteEventArgs(this, SelectionContext, currentValue));
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ParametrizedActionValueType"),
#endif
 Category("Behavior")]
		[DefaultValue(typeof(string)), TypeConverter(typeof(DevExpress.ExpressApp.Core.Design.ActionParameterValueTypeConverter))]
		public Type ValueType {
			get { return valueType; }
			set { valueType = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Value {
			get { return currentValue; }
			set {
				if(currentValue != value) {
					currentValue = value;
					OnValueChanged();
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ParametrizedActionExecute"),
#endif
 Category("Action")]
		public event ParametrizedActionExecuteEventHandler Execute;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ParametrizedActionShortCaption"),
#endif
 DefaultValue(""), Category("Appearance")]
		public string ShortCaption {
			get { return Model.ShortCaption; }
			set {
				if(ShortCaption != value) {
					Model.ShortCaption = value;
					RaiseChanged(ActionChangedType.Caption);
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ParametrizedActionNullValuePrompt"),
#endif
 DefaultValue(""), Category("Appearance")]
		public string NullValuePrompt {
			get { return Model.NullValuePrompt; }
			set {
				if(NullValuePrompt != value) {
					Model.NullValuePrompt = value;
					RaiseChanged(ActionChangedType.Caption);
				}
			}
		}
		[Browsable(false)]
		public event EventHandler ValueChanged;
	}
}
