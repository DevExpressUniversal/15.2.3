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

using DevExpress.XtraCharts.Native;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					  "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class LegendItem : ILegendItemData {
		bool markerVisible = true;
		bool textVisible = true;
		string text;
		Color textColor = Color.Black;
		Color markerColor;
		Image markerImage = null;
		Size markerSize = new Size(16, 16);
		Legend legend;
		Legend Legend { get { return legend; } }
		public LegendItem(string text, Color markerColor, Color textColor, Size markerSize, bool markerVisible, bool textVisible) {
			this.textColor = textColor;
			this.text = text;
			this.markerColor = markerColor;
			this.markerSize = markerSize;
			this.markerVisible = markerVisible;
			this.textVisible = textVisible;
		}
		internal void Init(Legend legend) {
			this.legend = legend;
		}
		#region ILegendItemData
		bool ILegendItemData.DisposeFont { get { return true; } }
		bool ILegendItemData.DisposeMarkerImage { get { return true; } }
		bool ILegendItemData.MarkerVisible { get { return markerVisible; } }
		bool ILegendItemData.TextVisible { get { return textVisible; } }
		string ILegendItemData.Text { get { return text; } }
		Color ILegendItemData.TextColor { get { return textColor; } }
		Image ILegendItemData.MarkerImage { get { return markerImage; } }
		ChartImageSizeMode ILegendItemData.MarkerImageSizeMode { get { return ChartImageSizeMode.Tile; } }
		Size ILegendItemData.MarkerSize { get { return markerSize; } }
		Font ILegendItemData.Font { get { return Legend.Font; } }
		object ILegendItemData.RepresentedObject { get { return null; } }
		void ILegendItemData.RenderMarker(IRenderer renderer, Rectangle bounds) { renderer.FillRectangle(bounds, markerColor); }
		#endregion
		public override bool Equals(object obj) {
			LegendItem item = obj as LegendItem;
			if (item == null)
				return false;
			return string.Equals(this.text, item.text) && this.markerColor.Equals(item.markerColor);
		}
		public override int GetHashCode() {
			return this.text.GetHashCode() ^ this.markerColor.GetHashCode();
		}
	}
}
