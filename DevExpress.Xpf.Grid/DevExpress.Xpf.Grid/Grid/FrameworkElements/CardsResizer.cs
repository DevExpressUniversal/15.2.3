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
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils.Native;
using DevExpress.Xpf.Core.Native;
using Thumb = DevExpress.Xpf.Core.DXThumb;
using DevExpress.Xpf.Utils;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using DevExpress.Xpf.Grid.Hierarchy;
namespace DevExpress.Xpf.Grid {
	public class CardSeparatorItemsControl : CachedItemsControl {
		protected override FrameworkElement CreateChild(object item) {
			FrameworkElement element = base.CreateChild(item);
			element.SetBinding(FrameworkElement.MarginProperty, new Binding("Margin"));
			return element;
		}
	}
	public class CardsResizer : Thumb, IResizeHelperOwner {
		CardView View { get { return (CardView)GridControl.FindCurrentView(this); } }
		public int RowIndex { get { return Separator.RowIndex; } }
		SeparatorInfo Separator { get { return (SeparatorInfo)DataContext; } }
		public CardsResizer() {
			this.SetDefaultStyleKey(typeof(CardsResizer));
			ResizeHelper resizeHelper = new ResizeHelper(this);
			resizeHelper.Init(this);
		}
#if !SILVERLIGHT
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
			HitTestResult result = base.HitTestCore(hitTestParameters);
			return result == null && IsHitTestVisible ? new PointHitTestResult(this, hitTestParameters.HitPoint) : result;
		}
#else
		protected HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
			return IsHitTestVisible ? new PointHitTestResult(this, hitTestParameters.HitPoint) : null;
		}
#endif
		#region IResizeHelperOwner Members
		double IResizeHelperOwner.ActualSize { 
			get { return View != null ? View.FixedSize : 0d; } 
			set { if(View != null) View.FixedSize = value; }
		}
		SizeHelperBase IResizeHelperOwner.SizeHelper { get { return SizeHelperBase.GetDefineSizeHelper(Separator.Orientation); } }
		void IResizeHelperOwner.ChangeSize(double delta) {
			((IResizeHelperOwner)this).ActualSize += delta / RowIndex;
		}
		void IResizeHelperOwner.OnDoubleClick() { }
		void IResizeHelperOwner.SetIsResizing(bool isResizing) { }
		#endregion
	}
}
