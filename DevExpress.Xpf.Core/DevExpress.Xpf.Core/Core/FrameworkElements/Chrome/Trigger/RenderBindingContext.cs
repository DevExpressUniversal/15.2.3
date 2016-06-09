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
using System.Globalization;
using System.Reflection;
namespace DevExpress.Xpf.Core.Native {
	public class RenderBindingContext : RenderTriggerContextBase {
		new RenderBinding Factory { get { return (RenderBinding)base.Factory; } }
		readonly RenderPropertyChangedListenerContext conditionContext;
		public RenderBindingContext(RenderBinding factory)
			: base(factory) {
			conditionContext = (RenderPropertyChangedListenerContext)factory.Condition.CreateContext();
		}
		protected override void AttachOverride(INamescope scope, IElementHost elementHost, IPropertyChangedListener listener) {
			base.AttachOverride(scope, elementHost, listener);
			conditionContext.Attach(scope, elementHost, listener, this);
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
		public override void Invalidate() {
			base.Invalidate();
			var value = conditionContext.GetValue();
			var context = Namescope.GetElement(Factory.TargetName);
			if (context == null) {
				if (!IsAttached)
					return;
				throw new ArgumentException(String.Format("Cannot find element with name '{0}'", Factory.TargetName));
			}				
			if (Factory.Converter != null)
				value = Factory.Converter.Convert(value, RenderTriggerHelper.GetPropertyType(context, Factory.TargetProperty), Factory.ConverterParamenter, CultureInfo.CurrentCulture);
			context.SetValue(Factory.TargetProperty ?? Factory.Property, value);
		}
		public override bool Matches(FrameworkRenderElementContext context, string propertyName) {
			return string.Equals(Factory.TargetProperty ?? Factory.Property, propertyName);
		}
	}
}
