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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class TimeRulerTimeCaptionItemsBaseCalculator : TimeRulerRowBasedItemsCalculator {
		Rectangle bounds;
		protected TimeRulerTimeCaptionItemsBaseCalculator(TimeRulerViewInfo ruler, DateTime[] actualTimes)
			: base(ruler, actualTimes) {
			this.bounds = ruler.ClientBounds;
		}
		protected internal Rectangle Bounds { get { return bounds; } }
		protected internal virtual TimeFormatInfo FormatInfo { get { return Ruler.FormatInfo; } }
		protected internal abstract bool UseTimeDesignator { get; }
		protected internal virtual ViewInfoTextItem CreateTimeCaptionItem(DateTime actualDate) {
			ViewInfoTextItem item = new ViewInfoTextItem();
			AppearanceHelper.Combine(item.Appearance, new AppearanceObject[] { this.Appearance });
			item.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
			item.Text = actualDate.ToString(ChooseFormat(actualDate));
			return item;
		}
		protected internal abstract string ChooseFormat(DateTime actualDate);
	}
}
