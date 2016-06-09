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

namespace DevExpress.Xpf.Core.Native {
	public abstract class RenderTriggerContextBase {
		protected RenderTriggerContextBase(RenderTriggerBase factory) {
			Factory = factory;
		}
		public RenderTriggerBase Factory { get; private set; }
		public IElementHost ElementHost { get; private set; }
		public INamescope Namescope { get; private set; }
		public IPropertyChangedListener Listener { get; private set; }
		protected bool IsAttached { get; private set; }
		public void Attach(INamescope scope, IElementHost elementHost, IPropertyChangedListener listener) {
			ElementHost = elementHost;
			Namescope = scope;
			Listener = listener;
			AttachOverride(scope, elementHost, listener);
			IsAttached = true;
		}
		public void Detach() {
			if(!IsAttached)
				return;
			IsAttached = false;
			DetachOverride();
			ElementHost = null;
			Namescope = null;
			Listener = null;			
		}
		public virtual void Invalidate() { }
		public void Reset() {
			if(IsAttached)
				ResetOverride();
		}
		protected virtual void ResetOverride() { }
		public abstract bool Matches(FrameworkRenderElementContext context, string propertyName);
		protected virtual void DetachOverride() { }
		protected virtual void AttachOverride(INamescope scope, IElementHost elementHost, IPropertyChangedListener listener) { }
	}
}
