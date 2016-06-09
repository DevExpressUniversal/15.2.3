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

using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Diagram.Core;
namespace DevExpress.Xpf.Diagram {
	public abstract class Ruler : AdornerLayer, ILayer {
		readonly Orientation orientation;
		protected Ruler(DiagramControl diagram, Orientation orientation)
			: base(diagram) {
			this.orientation = orientation;
		}
		protected override Rect GetBounds(Adorner adorner) {
			var bounds = base.GetBounds(adorner);
			var rotatedOrientation = orientation.Rotate();
			bounds = rotatedOrientation.SetSize(bounds, rotatedOrientation.GetSize(RenderSize));
			bounds = rotatedOrientation.SetLocation(bounds, 0);
			return bounds;
		}
	}
	public class VerticalRuler : Ruler {
		public VerticalRuler(DiagramControl diagram)
			: base(diagram, Orientation.Vertical) {
		}
	}
	public class HorizontalRuler : Ruler {
		public HorizontalRuler(DiagramControl diagram)
			: base(diagram, Orientation.Horizontal) {
		}
	}
}
