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

using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Actions;
using System.Windows;
namespace DevExpress.Xpf.Docking.Platform {
	public class LayoutViewActionListener : ActionServiceListener {
		public LayoutView View {
			get { return ServiceProvider as LayoutView; }
		}
		public override void OnHideSelection() {
			if(View.IsAdornerHelperInitialized)
				View.AdornerHelper.HideSelection();
		}
		public override void OnShowSelection() {
			if(View.IsAdornerHelperInitialized)
				View.AdornerHelper.ShowSelection();
		}
	}
	public class AutoHideViewActionListener : LayoutViewActionListener {
		protected AutoHideTray Tray {
			get { return ((AutoHideView)View).Tray; }
		}
		public override void OnHide(bool immediately) {
			if(immediately)
				Tray.DoClosePanel();
			else {
				Tray.DoCollapseIfPossible();
			}
		}
	}
	public class LayoutViewContextActionServiceListener : LayoutViewUIInteractionListener, IContextActionServiceListener {
		public virtual bool OnContextAction(Layout.Core.ILayoutElement element, Layout.Core.ContextAction action) {
			switch(action) {
				case Layout.Core.ContextAction.Float:
					return FloatElementOnDoubleClick(element);
			}
			return false;
		}
		protected override object KeyOverride {
			get { return typeof(IContextActionServiceListener); }
		}
		protected override FloatingHelper CreateFloatingHelper() {
			return new ContextActionFloatingHelper(View);
		}
		class ContextActionFloatingHelper : FloatingHelper {
			public ContextActionFloatingHelper(LayoutView view)
				: base(view) {
			}
			internal override Rect Check(Rect screenRect, Point startPoint) {
				return new Rect(new Point(), base.Check(screenRect, startPoint).Size);
			}
		}
	}
}
