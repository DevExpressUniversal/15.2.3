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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
namespace DevExpress.Xpf.Core.Native {
	public abstract class RenderTriggerBase {
		public IElementHost ElementHost { get; private set; }
		public INamescope Namescope { get; private set; }
		public abstract RenderTriggerContextBase CreateContext(INamescope namescope, IElementHost elementHost);		
	}
	public class RenderTriggersCollection : ObservableCollection<RenderTriggerBase> { }
	[ContentProperty("Setters")]
	public abstract class SettableRenderTriggerBase : RenderTriggerBase {
		public RenderSetterCollection Setters { get; private set; }
		protected SettableRenderTriggerBase() {
			Setters = new RenderSetterCollection();
		}
	}
	public class RenderTrigger : SettableRenderTriggerBase {
		public string Property { get { return Condition.Property; } set { Condition.Property = value; } }
		public DependencyProperty DependencyProperty { get { return Condition.DependencyProperty; } set { Condition.DependencyProperty = value; } }
		public object Value { get { return Condition.Value; } set { Condition.Value = value; } }
		public string TargetName { get { return Condition.TargetName; } set { Condition.TargetName = value; } }
		public RenderValueSource ValueSource { get { return Condition.ValueSource; } set { Condition.ValueSource = value; } }
		public string SourceName { get { return Condition.SourceName; } set { Condition.SourceName = value; } }
		public RenderCondition Condition { get; private set; }		
		public RenderTrigger() {
			Condition = new RenderCondition() { };
		}
		public override RenderTriggerContextBase CreateContext(INamescope namescope, IElementHost elementHost) {
			return new RenderTriggerContext(this);
		}		
	}
	public class MultiRenderTrigger : SettableRenderTriggerBase {
		readonly RenderConditionGroup conditionGroup;
		public RenderConditionCollection Conditions { get { return conditionGroup.Conditions; } }
		protected internal RenderConditionGroup ConditionGroup { get { return conditionGroup; } }
		public MultiRenderTrigger() {
			conditionGroup = new RenderConditionGroup();
		}
		public override RenderTriggerContextBase CreateContext(INamescope namescope, IElementHost elementHost) {			
			return new MultiRenderTriggerContext(this);
		}		
	}
}
