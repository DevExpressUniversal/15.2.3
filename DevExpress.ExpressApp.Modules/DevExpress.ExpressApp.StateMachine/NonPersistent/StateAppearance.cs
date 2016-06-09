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
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Data.Filtering;
using System.ComponentModel;
namespace DevExpress.ExpressApp.StateMachine.NonPersistent {
	public class StateAppearance : IAppearanceRuleProperties {
		private IState state;
		public StateAppearance(IState state) {
			this.state = state;
			if(state is IStateAppearancesProvider) {
				((IStateAppearancesProvider)state).Appearances.Add(this);
			}
			AppearanceItemType = "ViewItem";
			Context = "Any";
		}
		public string TargetItems { get; set; }
		public string AppearanceItemType { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string Criteria {
			get {
				if(state != null && state.StateMachine != null && state.Marker != null) {
					return new BinaryOperator(state.StateMachine.StatePropertyName, state.Marker).ToString();
				} else {
					return "0=1";
				}
			}
			set { throw new NotImplementedException(); }
		}
		public string Context { get; set; }
		public Type DeclaringType {
			get { return state.StateMachine.TargetObjectType; }
		}
		public int Priority { get; set;}
		public System.Drawing.FontStyle? FontStyle { get; set;}
		public System.Drawing.Color? FontColor { get; set;}
		public System.Drawing.Color? BackColor { get; set; }
		public DevExpress.ExpressApp.Editors.ViewItemVisibility? Visibility { get; set;}
		public bool? Enabled { get; set; }
		public string Method {
			get { return ""; }
			set { throw new NotImplementedException(); }
		}
	}
}
