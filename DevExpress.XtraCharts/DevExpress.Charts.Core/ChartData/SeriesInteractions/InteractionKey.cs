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
namespace DevExpress.Charts.Native {
	public struct InteractionKey {
		public static bool operator ==(InteractionKey a, InteractionKey b) {
			return a.Equals(b);
		}
		public static bool operator !=(InteractionKey a, InteractionKey b) {
			return !a.Equals(b);
		}
		readonly Type seriesViewType;
		readonly IPane pane;
		readonly IAxisData axisX;
		readonly IAxisData axisY;
		readonly object userKey;
		public IAxisData AxisX { get { return axisX; } }
		public IAxisData AxisY { get { return axisY; } }
		public IPane Pane { get { return pane; } }
		public Type SeriesViewType { get { return seriesViewType; } }
		public object UserKey { get { return userKey; } }
		public InteractionKey(ISeriesView view, bool isSideBySideInteraction) {
			seriesViewType = view.GetType();
			axisX = null;
			axisY = null;
			pane = null;
			userKey = null;
			IXYSeriesView xyView = view as IXYSeriesView;
			if (xyView != null) {
				pane = xyView.Pane;
				axisX = xyView.AxisXData;
				if (!isSideBySideInteraction) {
					ISideBySideStackedBarSeriesView sideBySideStackedBarSeriesView = view as ISideBySideStackedBarSeriesView;
					userKey = sideBySideStackedBarSeriesView != null ? sideBySideStackedBarSeriesView.StackedGroup : null;
					axisY = xyView.AxisYData;
				}
			}
		}
		public override bool Equals(Object obj) {
			if (obj is InteractionKey)
				return Equals((InteractionKey)obj);
			return false;
		}
		public bool Equals(InteractionKey key) {
			return (Type.Equals(seriesViewType, key.seriesViewType)) && (pane == key.pane) && (axisX == key.axisX) && (axisY == key.axisY) && (object.Equals(userKey, key.userKey));
		}
		public override int GetHashCode() {
			if (pane == null)
				return seriesViewType.GetHashCode();
			int userKeyHashCode = userKey != null ? userKey.GetHashCode() : 0;
			int axisYHashCode = axisY != null ? axisY.GetHashCode() : 0;
			return pane.GetHashCode() ^ seriesViewType.GetHashCode() ^ axisX.GetHashCode() ^ axisYHashCode ^ userKeyHashCode;
		}
	}
}
