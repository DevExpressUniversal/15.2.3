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
using System.Linq;
namespace DevExpress.Charts.Model {
	public enum LegendPosition {
		Bottom,
		Left,
		Right,
		Top,
		TopRight
	}
	public enum LegendOrientation { 
		Vertical, 
		Horizontal 
	}
	public class Legend : ModelElement {
		LegendItemCollection items;
		LegendPosition legendPosition;
		LegendOrientation orientation;
		Border border;
		bool overlay;
		DefaultBoolean enableAntialiasing = DefaultBoolean.Default;
		public LegendItemCollection Items { get { return items; } }
		public LegendPosition LegendPosition {
			get { return legendPosition; }
			set {
				if(legendPosition == value)
					return;
				legendPosition = value;
				NotifyParent(this, "LegendPosition", legendPosition);
			}
		}
		public LegendOrientation Orientation {
			get { return orientation; }
			set {
				if (orientation != value) {
					orientation = value;
					NotifyParent(this, "Orientation", orientation);
				}
			}
		}
		public Border Border {
			get { return border; }
			set {
				if (border != value) {
					border = value;
					NotifyParent(this, "Border", border);
				}
			}
		}
		public bool Overlay {
			get { return overlay; }
			set {
				if(overlay != value) {
					overlay = value;
					NotifyParent(this, "Overlay", overlay);
				}
			}
		}
		public DefaultBoolean EnableAntialiasing {
			get { return enableAntialiasing; }
			set {
				if (enableAntialiasing != value) {
					enableAntialiasing = value;
					NotifyParent(this, "EnableAntialiasing", value);
				}
			}
		}
		public Legend() {
			this.items = new LegendItemCollection(this);
			LegendPosition = Model.LegendPosition.Right;
		}
	}
	public class LegendItemCollection : ModelElementCollection<LegendItem> { 
		public LegendItemCollection(Legend parent) : base(parent) {
		}
	}
	public class LegendItem : ModelElement {
		public string Text { get; set; }
		public LegendItem() : base(null) { 
		}
	}
}
