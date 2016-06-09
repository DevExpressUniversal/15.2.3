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

using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.StateMachine.Xpo {
	[DefaultProperty("Caption")]
	[System.ComponentModel.DisplayName("Transition")]
	[ImageName("BO_Transition")]
	public class XpoTransition : StateMachineObjectBase, ITransition, ITransitionUISettings {
		public XpoTransition(Session session) : base(session) { }
		public string Caption {
			get { 
				string caption = GetPropertyValue<string>("Caption");
				if(string.IsNullOrEmpty(caption) && TargetState != null) {
					caption = TargetState.Caption;
				}
				return caption;
			}
			set { SetPropertyValue<string>("Caption", value); }
		}
		[Association("State-Transitions")]
		[RuleRequiredField("XpoTransition.SourceState", DefaultContexts.Save)]
		public XpoState SourceState {
			get { return GetPropertyValue<XpoState>("SourceState"); }
			set { SetPropertyValue<XpoState>("SourceState", value);}
		}
		[RuleRequiredField("XpoTransition.TargetState", DefaultContexts.Save)]
		[DataSourceProperty("SourceState.StateMachine.States")]
		[ImmediatePostData]
		public XpoState TargetState {
			get { return GetPropertyValue<XpoState>("TargetState"); }
			set {
				SetPropertyValue<XpoState>("TargetState", value);
				OnChanged("Caption");
			}
		}
		public int Index {
			get { return GetPropertyValue<int>("Index"); }
			set { SetPropertyValue<int>("Index", value); }
		}
		public bool SaveAndCloseView {
			get { return GetPropertyValue<bool>("SaveAndCloseView"); }
			set { SetPropertyValue<bool>("SaveAndCloseView", value); }
		}
		#region ITransition Members
		IState ITransition.TargetState {
			get { return TargetState; }
		}
		#endregion
	}
}
