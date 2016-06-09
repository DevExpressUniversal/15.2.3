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
	public class MinutesScaleMinuteCaptionItemsCalculator : MinutesScaleItemsCalculatorBase {
		#region Fields
		bool alwaysShowTimeDesignator;
		bool useTimeDesignator = true;
		#endregion
		public MinutesScaleMinuteCaptionItemsCalculator(TimeRulerViewInfo ruler, DateTime[] actualTimes)
			: base(ruler, actualTimes) {
			this.alwaysShowTimeDesignator = ruler.Ruler.AlwaysShowTimeDesignator;
		}
		#region Properties
		protected internal override bool UseTimeDesignator { get { return useTimeDesignator; } }
		protected internal override AppearanceObject Appearance { get { return Ruler.BackgroundAppearance; } }
		protected internal bool AlwaysShowTimeDesignator { get { return alwaysShowTimeDesignator; } }
		#endregion
		protected internal override string ChooseFormat(DateTime time) {
			return ScaleFormatHelper.ChooseMinutesFormat(time, UseTimeDesignator, FormatInfo);
		}
		protected internal override ViewInfoItem CreateItem(Rectangle rowBounds, int rowIndex) {
			ViewInfoTextItem item = CreateTimeCaptionItem(ActualTimes[rowIndex]);
			item.Bounds = new Rectangle(Bounds.X, rowBounds.Y - 1, Bounds.Width, rowBounds.Height);
			useTimeDesignator = alwaysShowTimeDesignator;
			return item;
		}
	}
}
