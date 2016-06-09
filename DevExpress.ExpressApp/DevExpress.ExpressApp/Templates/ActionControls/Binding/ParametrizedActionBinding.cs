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

using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Templates.ActionControls.Binding {
	public class ParametrizedActionBinding : ActionBinding {
		public static void Register() {
			ActionBindingFactory.Instance.Register("ParametrizedActionBinding", CanCreate, Create);
		}
		private static bool CanCreate(ActionBase action, IActionControl actionControl) {
			return action is ParametrizedAction && actionControl is IParametrizedActionControl;
		}
		private static ActionBinding Create(ActionBase action, IActionControl actionControl) {
			return new ParametrizedActionBinding((ParametrizedAction)action, (IParametrizedActionControl)actionControl);
		}
		private void ActionControl_Execute(object sender, ParametrizedActionControlExecuteEventArgs e) {
			Action.DoExecute(e.Parameter);
		}
		private void Action_ValueChanged(object sender, System.EventArgs e) {
			UpdateParameterValue();
		}
		protected override void OnActionChanged(ActionChangedType changesType) {
			base.OnActionChanged(changesType);
			if(changesType == ActionChangedType.Caption) {
				UpdateControlNullValuePrompt();
				UpdateExecuteButtonCaption();
			}
			if(changesType == ActionChangedType.Image) {
				UpdateExecuteButtonImage();
			}
		}
		protected virtual void UpdateControlNullValuePrompt() {
			ActionControl.SetNullValuePrompt(Action.NullValuePrompt);
		}
		protected virtual void UpdateExecuteButtonCaption() {
			ActionControl.SetExecuteButtonCaption(Action.ShortCaption);
		}
		protected virtual void UpdateExecuteButtonImage() {
			ActionControl.SetExecuteButtonImage(Action.ImageName);
		}
		protected virtual void UpdateParameterType() {
			ActionControl.SetParameterType(Action.ValueType);
		}
		protected virtual void UpdateParameterValue() {
			ActionControl.SetParameterValue(Action.Value);
		}
		protected override void UpdateControlImage() {
		}
		public ParametrizedActionBinding(ParametrizedAction action, IParametrizedActionControl actionControl)
			: base(action, actionControl) {
			Action.ValueChanged += Action_ValueChanged;
			ActionControl.Execute += ActionControl_Execute;
			UpdateParameterType();
			UpdateControlNullValuePrompt();
			UpdateExecuteButtonCaption();
			UpdateExecuteButtonImage();
			UpdateParameterValue();
		}
		public override void Dispose() {
			Action.ValueChanged -= Action_ValueChanged;
			ActionControl.Execute -= ActionControl_Execute;
			base.Dispose();
		}
		public new ParametrizedAction Action {
			get { return (ParametrizedAction)base.Action; }
		}
		public new IParametrizedActionControl ActionControl {
			get { return (IParametrizedActionControl)base.ActionControl; }
		}
	}
}
