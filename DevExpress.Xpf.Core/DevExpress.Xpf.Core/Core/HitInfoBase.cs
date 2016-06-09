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

using System.Windows;
using DevExpress.Xpf.Core.Native;
using System;
using DevExpress.Xpf.Core;
using System.Windows.Controls;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Helpers;
using ApplicationException = System.Exception;
#endif
namespace DevExpress.Xpf.Core {
	public abstract class HitTestAcceptorBase<T> where T : HitTestVisitorBase {
		public abstract void Accept(FrameworkElement element, T visitor);
	}
	[Serializable]
	public class HitTestingIsInProgressException : ApplicationException {
	}
	public abstract class HitTestVisitorBase {
		DependencyObject hitElement;
		internal Locker HitTestingInProgressLocker { get; private set; }
		internal bool CanContinue { get; set; }
		protected DependencyObject HitElement { get { return hitElement; } }
		protected HitTestVisitorBase() {
			CanContinue = true;
			HitTestingInProgressLocker = new Locker();
		}
		protected void StopHitTesting() {
			CanContinue = false;
		}
		internal void SetHitElement(DependencyObject hitElement) {
			this.hitElement = hitElement;
		}
	}
	public abstract class HitInfoBase<T> where T : HitTestVisitorBase {
		readonly DependencyObject d;
		readonly DependencyObject root;
		protected HitInfoBase(DependencyObject d, DependencyObject root) {
			this.d = d;
			this.root = root;
		}
		public void Accept(T visitor) {
			if(visitor.HitTestingInProgressLocker.IsLocked)
				throw new HitTestingIsInProgressException();
			visitor.HitTestingInProgressLocker.DoLockedAction(delegate {
				DependencyObject treeElement = d;
				HitTestAcceptorBase<T> acceptor = null;
				visitor.CanContinue = true;
				while(treeElement != null && treeElement != root) {
					acceptor = GetAcceptor(treeElement);
					if(acceptor != null) {
						visitor.SetHitElement(treeElement);
						acceptor.Accept(treeElement as FrameworkElement, visitor);
						if(!visitor.CanContinue)
							break;
					}
					treeElement = LayoutHelper.GetParent(treeElement);
				}
			});
		}
		protected abstract HitTestAcceptorBase<T> GetAcceptor(DependencyObject treeElement);
	}
}
