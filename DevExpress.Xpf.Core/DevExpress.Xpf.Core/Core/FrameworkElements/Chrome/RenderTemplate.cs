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
using System.Linq;
using System.Windows.Markup;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.Native {
	[ContentProperty("RenderTree")]
	public class RenderTemplate {
		public static readonly RenderTemplate Default = new RenderTemplate() {
			RenderTree = new RenderTextBlock() { Name = "contentPresenter" },
			Triggers = new RenderTriggersCollection() {
				new RenderBinding() { Property = "Content", TargetProperty = "Text", TargetName = "contentPresenter"}
			}
		};
		public FrameworkRenderElement RenderTree { get; set; }
		public RenderTriggersCollection Triggers { get; set; }
		public RenderTemplate() {
			Triggers = new RenderTriggersCollection();
		}
		public FrameworkRenderElementContext CreateContext(INamescope namescope, IElementHost elementHost, object dataContext) {
			var context = RenderTree.CreateContext(namescope, elementHost);
			context.DataContext = dataContext;
			return context;
		}
		public void InitializeTemplate(FrameworkRenderElementContext context, INamescope namescope, IElementHost elementHost, IPropertyChangedListener listener) {
			var triggers = Triggers;
			List<RenderTriggerContextBase> contexts = new List<RenderTriggerContextBase>(triggers.Count);
			foreach (RenderTriggerBase trigger in triggers) {
				var triggerContext = trigger.CreateContext(namescope, elementHost);
				triggerContext.Attach(namescope, elementHost, listener);
				contexts.Add(triggerContext);
			}
			namescope.Triggers = contexts;
		}
		public void ReleaseTemplate(FrameworkRenderElementContext context, INamescope namescope, IElementHost element) {
			var triggers = namescope.Triggers;
			if (triggers == null)
				return;
			foreach (var trigger in triggers) {
				trigger.Detach();
			}
			namescope.Triggers = null;
		}
	}
}
