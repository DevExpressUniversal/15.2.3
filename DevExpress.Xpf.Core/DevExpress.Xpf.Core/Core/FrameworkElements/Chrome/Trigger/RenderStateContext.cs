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

using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
namespace DevExpress.Xpf.Core.Native {
	public class RenderStateGroupContext : RenderTriggerContextBase {
		List<RenderStateTriggerContext> stateContexts;
		new RenderStateGroup Factory { get { return base.Factory as RenderStateGroup; } }
		public RenderStateTriggerContext ActiveState { get; private set; }
		public RenderStateGroupContext(RenderStateGroup factory) : base(factory) {
			stateContexts = new List<RenderStateTriggerContext>(Factory.States.Count);			
		}
		protected virtual void CreateStateContexts(INamescope scope, IElementHost elementHost) {
			if(stateContexts.Count != 0)
				return;
			foreach(var state in Factory.States) {
				var context = (RenderStateTriggerContext)state.CreateContext(scope, elementHost);
				context.GroupContext = this;
				stateContexts.Add(context);
			}
		}
		protected override void AttachOverride(INamescope scope, IElementHost elementHost, IPropertyChangedListener listener) {
			base.AttachOverride(scope, elementHost, listener);
			CreateStateContexts(scope, elementHost);
			foreach(var context in stateContexts)
				context.Attach(scope, elementHost, listener);
		}
		protected override void DetachOverride() {
			stateContexts.ForEach(x => x.Detach());			
			base.DetachOverride();
		}
		public override bool Matches(FrameworkRenderElementContext context, string propertyName) {
			if(ActiveState == null)
				return false;
			return ActiveState.Matches(context, propertyName);
		}
		public void GoToState(string stateName) {
			var newState = stateContexts.FirstOrDefault(x => Equals(x.Name, stateName));
			if(newState != null) {
				var oldState = ActiveState;
				ActiveState = newState;
				oldState.Do(x => x.Invalidate());
				ActiveState.Invalidate();
			}
		}
	}	
	public class RenderStateTriggerContext : SettableRenderTriggerContextBase {
		public RenderStateGroupContext GroupContext { get; set; }
		new RenderStateTrigger Factory { get { return base.Factory as RenderStateTrigger; } }
		public string Name { get { return Factory.Name; } }		
		public RenderStateTriggerContext(RenderStateTrigger factory) : base(factory) { }
		public override bool IsValid() { return GroupContext.ActiveState == this; }		
	}
}
