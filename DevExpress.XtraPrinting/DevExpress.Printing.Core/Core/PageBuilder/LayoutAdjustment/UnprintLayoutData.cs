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
using System.Text;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using System.Drawing;
namespace DevExpress.XtraPrinting.Native.LayoutAdjustment {
	public abstract class UnprintLayoutData : ILayoutData {
		protected float initialY;
		protected float fDpi;
		protected abstract float Y { get; }
		VerticalAnchorStyles ILayoutData.AnchorVertical { get { return VerticalAnchorStyles.None; } }
		bool ILayoutData.NeedAdjust { get { return false; } }
		float ILayoutData.Top { get { return Y; } }
		float ILayoutData.Bottom { get { return Y; } }
		public float InitialTop { get { return initialY; } }
		public float InitialBottom { get { return initialY; } }
		RectangleF ILayoutData.InitialRect { get { return RectangleF.FromLTRB(0, InitialTop, 0, InitialBottom); } }
		List<ILayoutData> ILayoutData.ChildrenData { get { return null; } }
		protected UnprintLayoutData(float dpi) {
			fDpi = dpi;
		}
		void ILayoutData.UpdateViewBounds() {
		}
		void ILayoutData.Anchor(float delta, float dpi) { }
		public abstract void SetBoundsY(float y);
	}
}
