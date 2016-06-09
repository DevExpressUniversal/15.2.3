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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardWin.Native {
	public abstract class Painter : StyleObjectPainter {
		readonly CardStyleProperties styleProperties = new CardStyleProperties();
		public ObjectPainter ObjectPainter { get { return this; } }
		public CardStyleProperties StyleProperties {
			get { return styleProperties; }
		}
		protected Painter() {
		}
	}
	public class ItemSkinPainter : Painter {
		const int HeightCoef = 100;
		readonly SkinElement skinElement;
		protected SkinElement SkinElement { get { return skinElement; } }
		public ItemSkinPainter(UserLookAndFeel lookAndFeel, string elementName) {
			Skin skin = CardSkinHelper.GetSkin(lookAndFeel);
			if(skin != null) {
				skinElement = skin[elementName];
				InitializeStyleProperties(skin);
			}
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return skinElement == null ? client : SkinElementPainter.Default.CalcBoundsByClientRectangle(new SkinElementInfo(skinElement, client));
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return skinElement == null ? e.Bounds : SkinElementPainter.Default.GetObjectClientRectangle(new SkinElementInfo(skinElement, e.Bounds));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SkinElementInfo elementInfo = new SkinElementInfo(skinElement, e.Bounds);
			PrepareElementInfo(elementInfo, e);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, elementInfo);
		}
		protected virtual void PrepareElementInfo(SkinElementInfo elementInfo, ObjectInfoArgs e) {
		}
		void InitializeStyleProperties(Skin skin) {
			DeltaColorsGetter deltaColorsGetter = new DeltaColorsGetter(skin);
			StyleProperties.Neutral = deltaColorsGetter.Neutral;
			StyleProperties.Bad = deltaColorsGetter.Bad;
			StyleProperties.Good = deltaColorsGetter.Good;
			StyleProperties.Warning = deltaColorsGetter.Warning;
			StyleProperties.ActualValueColor = deltaColorsGetter.ActualValueColor;
			StyleProperties.SubTextColor = skin.CommonSkin.Colors["DisabledText"];
			StyleProperties.Margin = new Size(GetIntegerProperty("MarginX"), GetIntegerProperty("MarginY"));
			StyleProperties.Proportions = new Size(GetIntegerProperty("ProportionsX"), GetIntegerProperty("ProportionsY"));
			StyleProperties.MainTitleHeight = (float)GetIntegerProperty("HeightMainTitle") / HeightCoef;
			StyleProperties.SubTitleHeight = (float)GetIntegerProperty("HeightSubTitle") / HeightCoef;
			StyleProperties.SubValue1Height = (float)GetIntegerProperty("HeightSubValue1") / HeightCoef;
			StyleProperties.SubValue2Height = (float)GetIntegerProperty("HeightSubValue2") / HeightCoef;
			StyleProperties.MainValueHeight = (float)GetIntegerProperty("HeightMainValue") / HeightCoef;
		}
		protected int GetIntegerProperty(string propertyName) {
			return skinElement == null ? 0 : (int)skinElement.Properties.GetInteger(propertyName);
		}
	}
	public class ItemStylePainter : Painter {
		readonly int leftMargin;
		readonly int topMargin;
		readonly int horizontalMargins;
		readonly int verticalMargins;
		public ItemStylePainter(Padding contentMargins) {
			leftMargin = contentMargins.Left;
			horizontalMargins = leftMargin + contentMargins.Right;
			topMargin = contentMargins.Top;
			verticalMargins = topMargin + contentMargins.Bottom;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return new Rectangle(client.X - leftMargin, client.Y - topMargin, client.Width + horizontalMargins, client.Height + verticalMargins);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle bounds = e.Bounds;
			return new Rectangle(bounds.X + leftMargin, bounds.Y + topMargin, bounds.Width - horizontalMargins, bounds.Height - verticalMargins);
		}
	}
}
