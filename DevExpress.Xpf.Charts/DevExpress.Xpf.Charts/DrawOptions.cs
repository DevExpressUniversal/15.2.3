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
using System.ComponentModel;
using System.Windows.Media;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class DrawOptions : ICloneable {
		public static bool operator ==(DrawOptions a, DrawOptions b) {
			if (((object)a == null) && ((object)b == null))
				return true;
			if (((object)a == null) || ((object)b == null))
				return false;
			return a.Equals(b);
		}
		public static bool operator !=(DrawOptions a, DrawOptions b) {
			if (((object)a == null) && ((object)b == null))
				return false;
			if (((object)a == null) || ((object)b == null))
				return true;
			return !a.Equals(b);
		}
		Color color;
		readonly Series series;
		internal Color ActualColor {
			get {
				return ((IInteractiveElement)series).IsSelected ? VisualSelectionHelper.GetSelectedPointColor(Color) : Color;
			}
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("DrawOptionsColor"),
#endif
		Category(Categories.Presentation)
		]
		public Color Color {
			get { return color; }
			set { color = value; }
		}
		DrawOptions() { }
		internal DrawOptions(Series series) {
			Color = series.BaseColor;
			this.series = series;
		}
		#region ICloneable implementation
		public object Clone() {
			DrawOptions drawOptions = new DrawOptions(); 
			drawOptions.DeepClone(this);
			return drawOptions;
		}
		#endregion
		void DeepClone(object obj) {
			DrawOptions drawOptions = obj as DrawOptions;
			if (drawOptions != null)
				Color = drawOptions.Color;
		}
		public override bool Equals(object obj) {
			if (obj != null) {
				DrawOptions drawOptions = obj as DrawOptions;
				if (drawOptions != null)
					return Color.Equals(drawOptions.Color);
			}
			return false;
		}
		public override int GetHashCode() {
			return Color.GetHashCode();
		}
	}
}
