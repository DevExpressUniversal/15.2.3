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
using DevExpress.Utils.Controls;
namespace DevExpress.XtraScheduler {
	public enum RangeControlDataDisplayType { Auto, Thumbnail, Number };
	public interface IScaleBasedRangeControlClientOptions {
		TimeScaleCollection Scales { get; }
		bool AutoFormatScaleCaptions { get; set; }
		int MinIntervalWidth { get; set; }
		int MaxIntervalWidth { get; set; }
		int MaxSelectedIntervalCount { get; set; }
		RangeControlDataDisplayType DataDisplayType { get; set; }
		int ThumbnailHeight { get; set; }
		DateTime RangeMinimum { get; set; }
		DateTime RangeMaximum { get; set; }
		event BaseOptionChangedEventHandler Changed;
		void BeginUpdate();
		void EndUpdate();
	}
	public interface ISchedulerOptionsRangeControl : IScaleBasedRangeControlClientOptions {
		bool AllowChangeActiveView { get; set; }
		bool AutoAdjustMode { get; set; }
	}
}
