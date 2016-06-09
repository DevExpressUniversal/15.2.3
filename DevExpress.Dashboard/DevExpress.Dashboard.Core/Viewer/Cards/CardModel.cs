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
using System.Collections;
using System.Drawing;
using DevExpress.DashboardCommon.Data;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Viewer {
	public class CardModel : ISizable, IValuesProvider {
		readonly CardViewModel viewModel;
		readonly CardData data;
		readonly CardViewInfo viewInfo = new CardViewInfo();
		const int cardLabelCount = 5;
		Rectangle baseBounds = new Rectangle();
		Rectangle bounds = new Rectangle();
		Rectangle cardPanelBounds = Rectangle.Empty;
		Rectangle cardSparklineBounds = Rectangle.Empty;
		Point offset = new Point();
		string mainValue;
		string subValue1;
		string subValue2;
		string title;
		string subTitle;
		string measureId;
		public int Top {
			get { return baseBounds.Top - offset.Y; }
			set { baseBounds.Y = value; }
		}
		public int Left {
			get { return baseBounds.Left - offset.X; }
			set { baseBounds.X = value; }
		}
		public int Width {
			get { return baseBounds.Width; }
			set { baseBounds.Width = value; }
		}
		public int Height {
			get { return baseBounds.Height; }
			set { baseBounds.Height = value; }
		}
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle CardPanelBounds {
			get {
				Rectangle allBounds = Bounds;
				if(cardPanelBounds == Rectangle.Empty)
					cardPanelBounds = new Rectangle(allBounds.X, allBounds.Y, allBounds.Width, Convert.ToInt32(allBounds.Height * 4 / 5));
				return cardPanelBounds;
			} 
		}
		public Rectangle CardSparklineBounds { 
			get {
				Rectangle allBounds = Bounds;
				Rectangle cardPanelBounds = CardPanelBounds;
				if(cardSparklineBounds == Rectangle.Empty)
					cardSparklineBounds = new Rectangle(allBounds.X, cardPanelBounds.Bottom, allBounds.Width, allBounds.Height - cardPanelBounds.Height);
				return cardSparklineBounds;
			} 
		}
		public string MainValue { get { return mainValue; } }
		public string SubValue1 { get { return subValue1; } }
		public string SubValue2 { get { return subValue2; } }
		public string Title { get { return title; } }
		public string SubTitle { get { return subTitle; } }
		public CardData Data { get { return data; } }
		public CardViewModel ViewModel { get { return viewModel; } }
		public CardViewInfo ViewInfo { get { return viewInfo; } }
		IList IValuesProvider.SelectionValues { get { return data.SelectionValues; } }
		string IValuesProvider.MeasureID { get { return measureId; } }
		public CardModel(CardViewModel viewModel, CardData data) {
			this.viewModel = viewModel;
			this.data = data;
			this.mainValue = data.ValueText;
			this.subValue1 = data.SubValue1Text;
			this.subValue2 = data.SubValue2Text;
			this.title = data.Title;
			this.subTitle = String.Empty;
			if(data.SelectionValues != null) {
				ElementTitleFormatter formatter = new ElementTitleFormatter(data.SelectionCaptions);
				title = formatter.MainTitle;
				subTitle = formatter.SubTitle;
			}
			this.measureId = viewModel.ID;
		}
		public bool CheckBounds(Rectangle clientBounds) {
			return clientBounds.Contains(bounds) || clientBounds.IntersectsWith(bounds);
		}
		public void SetOffset(Point offset) {
			this.offset = offset;
			this.bounds = new Rectangle(Left, Top, Width, Height);
			cardPanelBounds = Rectangle.Empty;
			cardSparklineBounds = Rectangle.Empty;
		}
	}
	public class CardViewInfo {
		public Rectangle TitleBounds { get; set; }
		public Rectangle SubTitleBounds { get; set; }
		public Rectangle SubValue1Bounds { get; set; }
		public Rectangle SubValue2Bounds { get; set; }
		public Rectangle MainValueBounds { get; set; }
		public Rectangle SparklineBounds { get; set; }
		public bool ShowTitleTooltip { get; set; }
		public bool ShowSubTitleTooltip { get; set; }
		public bool ShowSubValue1Tooltip { get; set; }
		public bool ShowSubValue2Tooltip { get; set; }
		public bool ShowMainValueTooltip { get; set; }
		public bool ShowSparklineTooltip { get { return true; } }
	}
}
