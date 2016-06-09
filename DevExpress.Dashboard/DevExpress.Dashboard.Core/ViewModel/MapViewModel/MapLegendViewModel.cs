#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
namespace DevExpress.DashboardCommon.ViewModel {
	public class MapLegendViewModelBase {
		public bool Visible { get; set; }
		public MapLegendPosition Position { get; set; }
		public MapLegendViewModelBase() {
		}
		public MapLegendViewModelBase(MapLegendBase legend) {
			Visible = legend.Visible;
			Position = legend.Position;
		}
		public override bool Equals(object obj) {
			MapLegendViewModelBase model = obj as MapLegendViewModelBase;
			if(model == null)
				return false;
			return Visible == model.Visible && Position == model.Position;
		}
		public override int GetHashCode() {
			return Visible.GetHashCode() ^ Position.GetHashCode();
		}
	}
	public class MapLegendViewModel : MapLegendViewModelBase {
		public MapLegendOrientation Orientation { get; set; }
		public MapLegendViewModel() : base() {
		}
		public MapLegendViewModel(MapLegend legend)
			: base(legend) {
			Orientation = legend.Orientation;
		}
		public override bool Equals(object obj) {
			MapLegendViewModel model = obj as MapLegendViewModel;
			if(model == null)
				return false;
			return base.Equals(model) && Orientation == model.Orientation;
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ Orientation.GetHashCode();
		}
	}
	public class WeightedLegendViewModel : MapLegendViewModelBase {
		public WeightedLegendType Type { get; set; }
		public WeightedLegendViewModel()
			: base() {
		}
		public WeightedLegendViewModel(WeightedLegend legend)
			: base(legend) {
			Type = legend.Type;
		}
		public override bool Equals(object obj) {
			WeightedLegendViewModel model = obj as WeightedLegendViewModel;
			if(model == null)
				return false;
			return base.Equals(model) && Type == model.Type;
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ Type.GetHashCode();
		}
	}
}
