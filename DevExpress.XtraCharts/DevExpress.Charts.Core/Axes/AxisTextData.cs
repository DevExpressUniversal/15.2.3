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
namespace DevExpress.Charts.Native {
	public interface IAxisLabel {
		bool Staggered { get; }
		int Angle { get; }
		IAxisLabelResolveOverlappingOptions ResolveOverlappingOptions { get; }
		IAxisLabelFormatterCore Formatter { get; set; }
		string TextPattern { get; }
	}
	public class AxisTextItem : IComparable {
		readonly ICustomAxisLabel customAxisLabel;
		readonly double value;
		readonly object content;
		readonly int gridIndex;
		readonly bool visible;
		readonly bool isCustomLabel;
		public ICustomAxisLabel CustomAxisLabel { get { return customAxisLabel; } }
		public double Value { get { return value; } }
		public object Content { get { return content; } }
		public bool Visible { get { return visible; } }
		public int GridIndex { get { return gridIndex; } }
		public bool IsCustomLabel { get { return isCustomLabel; } }
		public AxisTextItem(ICustomAxisLabel customAxisLabel, int gridIndex, double value, object content, bool visible, bool isCustomLabel) {
			this.customAxisLabel = customAxisLabel;
			this.value = value;
			this.content = content;
			this.visible = visible;
			this.gridIndex = gridIndex;
			this.isCustomLabel = isCustomLabel;
		}
		public int CompareTo(object obj) {
			AxisTextItem textItem = (AxisTextItem)obj;
			return SortingUtils.CompareDoubles(value, textItem.value);
		}
	}
}
