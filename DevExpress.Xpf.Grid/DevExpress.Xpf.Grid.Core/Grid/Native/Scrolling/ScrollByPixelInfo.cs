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
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Grid.Native {
	public class ScrollByPixelInfo : ScrollInfoBase {
		public ScrollByPixelInfo(IScrollInfoOwner scrollOwner, SizeHelperBase sizeHelper)
			: base(scrollOwner, sizeHelper) {
		}
		protected override double ValidateOffsetCore(double value) {
			if(value <= 0)
				return 0;
			return Math.Min(value, Math.Max(0, Extent - Viewport));
		}
		protected override void NeedMeasure() {
			base.NeedMeasure();
			ScrollOwner.InvalidateHorizontalScrolling();
		}
		protected override void OnScrollInfoChanged() {
			if(Offset + Viewport > Extent)
				fOffset = Math.Max(0, Extent - Viewport);
			ScrollOwner.OnSecondaryScrollInfoChanged();
		}
		protected override bool OnBeforeChangeOffset() {
			return ScrollOwner.OnBeforeChangePixelScrollOffset();
		}
		public override void LineDown() {
			base.LinesDown(ScrollOwner.ScrollStep);
		}
		public override void LineUp() {
			base.LinesUp(ScrollOwner.ScrollStep);
		}
	}
}
