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

using System.Collections;
using System.Drawing;
using DevExpress.DashboardCommon.Native;
using DevExpress.XtraCharts;
namespace DevExpress.DashboardCommon.Viewer {
	public class PieSeries : Series, ISizable, IValuesProvider {
  		public const string ArgumentDataMemberName = "Argument";
		readonly IList selectionValues;
		int top;
		int left;
		int width;
		int height;
		Point offset;
		string measureId;
		IList IValuesProvider.SelectionValues { get { return selectionValues; } }
		string IValuesProvider.MeasureID { get { return measureId; } }
		public int Top {
			get { return top - offset.Y; }
			set { top = value; }
		}
		public int Left {
			get { return left - offset.X; }
			set { left = value; }
		}
		public int Width {
			get { return width; }
			set { width = value; }
		}
		public int Height {
			get { return height; }
			set { height = value; }
		}
		public Rectangle Bounds {
			get { return new Rectangle(Left, Top, Width, Height); }
		}
		public PieSeries(ViewType pieViewType, IList selectionValues, string measureId) {
			this.selectionValues = selectionValues;
			this.measureId = measureId;
			ChangeView(pieViewType);
			ArgumentDataMember = ArgumentDataMemberName;
			ValueDataMembers[0] = measureId;
			PieSeriesLabel pieSeriesLabel = (PieSeriesLabel)Label;
			pieSeriesLabel.Position = PieSeriesLabelPosition.TwoColumns;
			pieSeriesLabel.ResolveOverlappingMode = ResolveOverlappingMode.Default;
			PieSeriesView pieSeriesView = (PieSeriesView)View;
			pieSeriesView.MinAllowedSizePercentage = 70;
		}
		public bool IsVisible(Size bounds) {
			return Left + width > 0 && Top + height > 0 && Top < bounds.Height && Left < bounds.Width;
		}
		public void SetOffset(Point offset) {
			this.offset = offset;
		}
	}
}
