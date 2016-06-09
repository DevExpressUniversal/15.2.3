﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class AllDayAreaCell : SchedulerViewCellBase {
		bool drawLeftSeparator;
		Rectangle leftSeparatorBounds;
		SkinElementInfo cachedSkinElementInfo;
		public AllDayAreaCell() {
		}
		#region Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool DrawLeftSeparator { get { return drawLeftSeparator; } set { drawLeftSeparator = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Rectangle LeftSeparatorBounds { get { return leftSeparatorBounds; } set { leftSeparatorBounds = value; } }
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.AllDayArea; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SkinElementInfo CachedSkinElementInfo { get { return cachedSkinElementInfo; } set { cachedSkinElementInfo = value; } }
		#endregion
		protected internal override bool RaiseCustomDrawEvent(GraphicsCache cache, ISupportCustomDraw customDrawProvider, DefaultDrawDelegate defaultDrawDelegate) {
			this.Cache = cache;
			try {
				CustomDrawObjectEventArgs args = new CustomDrawObjectEventArgs(this, this.Bounds, defaultDrawDelegate);
				customDrawProvider.RaiseCustomDrawDayViewAllDayArea(args);
				return args.Handled;
			} finally {
				this.Cache = null;
			}
		}
	}
}
