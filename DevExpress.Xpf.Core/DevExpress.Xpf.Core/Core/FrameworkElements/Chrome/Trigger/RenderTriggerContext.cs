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

using System.Collections.Generic;
namespace DevExpress.Xpf.Core.Native {
	public abstract class SettableRenderTriggerContextBase : RenderTriggerContextBase {
		new SettableRenderTriggerBase Factory { get { return base.Factory as SettableRenderTriggerBase; } }
		protected SettableRenderTriggerContextBase(SettableRenderTriggerBase factory)
			: base(factory) {
		}
		public abstract bool IsValid();
		public override void Invalidate() {
			if(IsValid()) {
				foreach(var setter in Factory.Setters) {
					setter.SetValue(Namescope, ElementHost);
				}
			} else {
				foreach(var setter in Factory.Setters) {
					setter.ResetValue(Namescope, ElementHost);
				}
			}
		}
		public override bool Matches(FrameworkRenderElementContext context, string propertyName) {
			foreach(var setter in Factory.Setters) {
				if(setter.Matches(context, propertyName))
					return true;
			}
			return false;
		}
	}
	public class RenderTriggerContext : SettableRenderTriggerContextBase {
		readonly RenderConditionContext conditionContext;
		public new RenderTrigger Factory { get { return base.Factory as RenderTrigger; } }
		public RenderTriggerContext(RenderTrigger factory)
			: base(factory) {
				conditionContext = factory.Condition.CreateContext() as RenderConditionContext;
		}
		protected override void AttachOverride(INamescope scope, IElementHost elementHost, IPropertyChangedListener listener) {
			base.AttachOverride(scope, elementHost, listener);
			conditionContext.Attach(scope, elementHost, listener, this);
			if(IsValid())
				Invalidate();
		}
		protected override void DetachOverride() {
			conditionContext.Detach();
			base.DetachOverride();
		}
		protected override void ResetOverride() {
			base.ResetOverride();
			conditionContext.Reset();
		}
		public override bool IsValid() {
			return conditionContext.IsValid;
		}		
	}
	public class MultiRenderTriggerContext : SettableRenderTriggerContextBase {
		readonly RenderConditionGroupContext conditionGroupContext;
		new MultiRenderTrigger Factory { get { return base.Factory as MultiRenderTrigger; } }
		public MultiRenderTriggerContext(MultiRenderTrigger factory)
			: base(factory) {
				conditionGroupContext = (RenderConditionGroupContext)Factory.ConditionGroup.CreateContext();			
		}
		protected override void AttachOverride(INamescope scope, IElementHost elementHost, IPropertyChangedListener listener) {
			base.AttachOverride(scope, elementHost, listener);
			conditionGroupContext.Attach(scope, elementHost, listener, this);
			if(IsValid())
				Invalidate();
		}
		protected override void DetachOverride() {
			conditionGroupContext.Detach();
			base.DetachOverride();
		}
		protected override void ResetOverride() {
			base.ResetOverride();
			conditionGroupContext.Reset();
		}
		public override bool IsValid() {
			return conditionGroupContext.IsValid;
		}
	}	
}
