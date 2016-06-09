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
using DevExpress.ExpressApp.ConditionalAppearance;
using System.ComponentModel;
namespace DevExpress.ExpressApp.StateMachine {
	public interface IStateMachine {
		string Name { get; }
		bool Active { get; }
		Type TargetObjectType { get; }
		string StatePropertyName { get; }
		IList<IState> States { get; }
		IState FindCurrentState(object targetObject);
		void ExecuteTransition(object targetObject, IState targetState);
	}
	public interface IState {
		string Caption { get; }
		object Marker { get; }
		string TargetObjectCriteria { get; }
		IList<ITransition> Transitions { get; }
		IStateMachine StateMachine { get; }
	}
	public interface ITransition {
		string Caption { get; }
		IState TargetState { get; }
	}
	public interface ITransitionUISettings {
		int Index { get; }
		bool SaveAndCloseView { get; }
	}
	public interface IStateMachineUISettings {
		bool ExpandActionsInDetailView { get; }
	}
	public interface IStateAppearancesProvider {
		IList<IAppearanceRuleProperties> Appearances { get; }
	}
}
