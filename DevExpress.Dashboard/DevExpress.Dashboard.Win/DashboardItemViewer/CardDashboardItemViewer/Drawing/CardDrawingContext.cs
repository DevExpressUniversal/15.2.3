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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardWin.Native {
	static class CardSkinHelper {
		public static readonly string ItemName = DashboardSkins.SkinCard;
		public static Skin GetSkin(UserLookAndFeel lookAndFeel) {
			return DashboardSkins.GetSkin(lookAndFeel);
		}
	}
	public class CardDrawingContext : IDisposable {
		readonly CardAppearances appearances;
		Painter itemPainter;
		public CardAppearances Appearances { get { return appearances; } }
		public Painter ItemPainter { get { return itemPainter; } }
		public CardDrawingContext(UserLookAndFeel lookAndFeel) {
			appearances = new CardAppearances(lookAndFeel);
			Update(lookAndFeel);
		}
		public void Dispose() {
			appearances.Dispose();
			GC.SuppressFinalize(this);
		}
		public void Update(UserLookAndFeel lookAndFeel) {
			appearances.Update(lookAndFeel);
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				itemPainter = new ItemSkinPainter(lookAndFeel, CardSkinHelper.ItemName);
			else
				itemPainter = new ItemStylePainter(new Padding(16, 6, 16, 6));
		}
		public void DrawObject(GraphicsCache cache, Rectangle bounds) {
			ObjectPainter.DrawObject(cache, ItemPainter.ObjectPainter,
				new StyleObjectInfoArgs(cache, bounds, Appearances.ItemAppearance));
		}
		public Rectangle GetObjectClientRectangle(GraphicsCache cache, Rectangle bounds) {
			return ObjectPainter.GetObjectClientRectangle(cache.Graphics, ItemPainter.ObjectPainter,
				new StyleObjectInfoArgs(cache, bounds, Appearances.ItemAppearance));
		}
	}
	public class CardAppearances : BaseAppearanceCollection {
		static void ApplySkinToAppearance(UserLookAndFeel lookAndFeel, string skinElementName, AppearanceDefault appearance) {
			Skin skin = CardSkinHelper.GetSkin(lookAndFeel);
			if(skin != null) {
				SkinElement skinElement = skin[skinElementName];
				if(skinElement != null)
					skinElement.Apply(appearance);
			}
		}
		AppearanceObject itemAppearance;
		public AppearanceObject ItemAppearance { get { return itemAppearance; } }
		public CardAppearances(UserLookAndFeel lookAndFeel) {
			Update(lookAndFeel);
		}
		protected override void CreateAppearances() {
			base.CreateAppearances();
			itemAppearance = CreateAppearance(CardSkinHelper.ItemName);
		}
		public override void Dispose() {
			if(itemAppearance != null)
				itemAppearance.Dispose();
			base.Dispose();
		}
		public void Update(UserLookAndFeel lookAndFeel) {
			AppearanceDefault itemAppearance = new AppearanceDefault(SystemColors.ControlText,
				SystemColors.ControlLight, SystemColors.ControlDarkDark, SystemColors.ControlLightLight);
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				ApplySkinToAppearance(lookAndFeel, CardSkinHelper.ItemName, itemAppearance);
			GetAppearance(CardSkinHelper.ItemName).Assign(itemAppearance);
		}
	}
}
