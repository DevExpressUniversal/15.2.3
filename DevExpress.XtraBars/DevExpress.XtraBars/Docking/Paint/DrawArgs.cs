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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking.Helpers;
namespace DevExpress.XtraBars.Docking.Paint {
	public class DrawArgs {
		GraphicsCache cache;
		Rectangle bounds;
		AppearanceObject appearance;
		public DrawArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance) {
			this.cache = cache;
			this.bounds = bounds;
			this.appearance = appearance;
		}
		public void DrawBackground() {
			Appearance.DrawBackground(Graphics, Cache, Bounds);
		}
		public void DrawString(string text) {
			DrawString(text, Bounds);
		}
		public void DrawString(string text, Rectangle textBounds) {
			if(textBounds.Width > 0)
				Appearance.DrawString(Cache, text, textBounds, Appearance.GetStringFormat(TextOptions.DefaultOptionsNoWrapEx));
		}
		public Graphics Graphics { get { return cache.Graphics; } }
		public Rectangle Bounds { get { return bounds; } }
		public AppearanceObject Appearance { get { return appearance; } }
		public GraphicsCache Cache { get { return cache; } }
	}
	public class DrawApplicationCaptionArgs : DrawArgs {
		Rectangle textBounds;
		Rectangle imageBounds;
		string caption;
		bool activeCaption;
		Image imageCore;
		object imagesCore;
		int imageIndexCore;
		public DrawApplicationCaptionArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, Rectangle textBounds, Rectangle imageBounds, string caption, bool activeCaption, Image image, object images, int imageIndex) :
			base(cache, bounds, appearance) {
			this.textBounds = textBounds;
			this.imageBounds = imageBounds;
			this.activeCaption = activeCaption;
			this.caption = caption;
			this.imageCore = image;
			this.imageIndexCore = imageIndex;
			this.imagesCore = images;
		}
		public Image Image { get { return imageCore; } }
		public bool ActiveCaption { get { return activeCaption; } }
		public Rectangle TextBounds { get { return textBounds; } }
		public Rectangle ImageBounds { get { return imageBounds; } }
		public string Caption { get { return caption; } }
		public object Images { get { return imagesCore; } }
		public int ImageIndex { get { return imageIndexCore; } }
		public bool AllowGlyphSkinning { get; set; }
	}
	public class DrawWindowCaptionArgs : DrawApplicationCaptionArgs {
		bool isVertical;
		public DrawWindowCaptionArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, string caption, Rectangle textBounds, Rectangle imageBounds, bool activeCaption, Image image, object images, int imageIndex) :
			base(cache, bounds, appearance, textBounds, imageBounds, caption, activeCaption, image, images, imageIndex) {
			this.isVertical = false;
		}
		public bool IsVertical { get { return isVertical; } set { isVertical = value; } }
	}
	public class DrawBorderArgs : DrawArgs {
		bool isActive;
		bool isFloat;
		Color borderPen, captionColor;
		BorderObjectInfoArgs borderArgs;
		public DrawBorderArgs(GraphicsCache cache, Rectangle bounds, Color borderPen, Color captionColor, bool isActive, bool isFloat) :
			base(cache, bounds, null) {
			this.isActive = isActive;
			this.isFloat = isFloat;
			this.borderPen = borderPen;
			this.captionColor = captionColor;
			this.borderArgs = new BorderObjectInfoArgs(Cache, Bounds, null);
		}
		public DevExpress.Utils.Paint.XPaint Paint { get { return Cache.Paint; } }
		public BorderObjectInfoArgs BorderArgs { get { return borderArgs; } }
		public Pen BorderPen { get { return Cache.GetPen(borderPen); } }
		public Pen CaptionPen { get { return Cache.GetPen(CaptionColor); } }
		public Color CaptionColor { get { return captionColor; } }
		public bool IsActive { get { return isActive; } }
		public bool IsFloat { get { return isFloat; } }
	}
	public class DrawTabPanelArgs : BaseDrawTabArgs {
		DockLayout tabContainer;
		bool drawTabButtons;
		public DrawTabPanelArgs(GraphicsCache cache, DockLayout tabContainer, bool drawTabButtons, ObjectState state) :
			base(cache, tabContainer.TabPanelBounds, tabContainer.TabsAppearance, state) {
			this.tabContainer = tabContainer;
			this.drawTabButtons = drawTabButtons;
		}
		protected ObjectState GetState(int index) {
			if(index >= tabContainer.TabButtons.Count) return ObjectState.Normal;
			return tabContainer.GetObjectStateByCaptionButton((DockPanelCaptionButton)tabContainer.TabButtons[index]);
		}
		public DrawTabArgs GetTabArgs(int index) {
			if(TabContainer.Count <= index || !TabContainer[index].IsValid || index < TabContainer.FirstVisibleTabIndex) return null;
			Rectangle tabBounds = TabContainer.TabsBounds[index];
			if(Bounds.IntersectsWith(tabBounds) && DockLayoutUtils.CanDraw(null, tabBounds))
				return new DrawTabArgs(Cache, tabBounds, Bounds, index == TabContainer.ActiveChildIndex ? TabContainer.ActiveTabAppearance : Appearance, TabContainer, index, GetState(index));
			return null;
		}
		int GetScrollButtonsClipZoneWidth(int indent) {
			if(!TabContainer.IsTabsButtonsVisible) return 0;
			DirectionRectangle dBounds = new DirectionRectangle(Bounds, !IsVertical);
			DirectionRectangle dButton = new DirectionRectangle(PrevTabButtonArgs.Bounds, !IsVertical);
			return dBounds.Right - dButton.Left + indent;
		}
		public Rectangle GetClipBounds(TabPanelPainter tabPainter) {
			Rectangle result = Bounds;
			if(DrawTabButtons)
				result = LayoutRectangle.RemoveSize(result, GetScrollButtonsClipZoneWidth(tabPainter.TabHorzTextIndent),
					IsVertical ? DockingStyle.Bottom : DockingStyle.Right);
			if(result.IntersectsWith(TabContainer.CaptionBounds)) {
				Rectangle cutBounds = Rectangle.Intersect(result, TabContainer.CaptionBounds);
				result = LayoutRectangle.RemoveSize(result, cutBounds.Height, DockingStyle.Top);
			}
			return result;
		}
		protected DockLayout TabContainer { get { return tabContainer; } }
		public DrawTabPanelButtonArgs DrawTabPanelButtonArgs {
			get {
				int index = TabContainer.ActiveChildIndex;
				Rectangle tabBounds = (index >= 0 && index < TabContainer.TabsBounds.Length) ?
					TabContainer.TabsBounds[index] : Rectangle.Empty;
				return new DrawTabPanelButtonArgs(Cache, Bounds, Appearance, TabContainer.ActiveTabAppearance, TabContainer.TabsAppearance, tabBounds, Position);
			}
		}
		public DrawTabButtonArgs PrevTabButtonArgs { get { return new DrawTabButtonArgs(Cache, Appearance, TabContainer.PrevTabButton); } }
		public DrawTabButtonArgs NextTabButtonArgs { get { return new DrawTabButtonArgs(Cache, Appearance, TabContainer.NextTabButton); } }
		public bool DrawTabButtons { get { return drawTabButtons; } }
		public override TabsPosition Position { get { return TabContainer.TabsPosition; } }
		public int TabsCount { get { return TabContainer.TabsBounds.Length; } }
		public int ActiveTabIndex { get { return TabContainer.ActiveChildIndex; } }
	}
	public class DrawTabButtonArgs : DrawArgs {
		DockPanelCaptionButton tabButton;
		public DrawTabButtonArgs(GraphicsCache cache, AppearanceObject appearance, DockPanelCaptionButton tabButton)
			: base(cache, tabButton.Bounds, appearance) {
			this.tabButton = tabButton;
		}
		public DockPanelCaptionButton TabButton { get { return tabButton; } }
	}
	public class DrawTabPanelButtonArgs : BaseDrawTabArgs {
		Rectangle activeTabBounds;
		TabsPosition position;
		AppearanceObject activeTabAppearance;
		AppearanceObject tabsAppearance;
		public DrawTabPanelButtonArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, AppearanceObject activeTabAppearance, AppearanceObject tabsAppearance, Rectangle activeTabBounds, TabsPosition position) :
			base(cache, bounds, appearance, ObjectState.Normal) {
			this.activeTabBounds = activeTabBounds;
			this.activeTabAppearance = activeTabAppearance;
			this.tabsAppearance = tabsAppearance;
			this.position = position;
		}
		public Rectangle ActiveTabBounds { get { return activeTabBounds; } }
		public Color ForePanelRectColor { get { return (DockLayoutUtils.IsHead(Position) && !ActiveTabAppearance.BackColor2.IsEmpty ? ActiveTabAppearance.BackColor2 : ActiveTabAppearance.BackColor); } }
		protected AppearanceObject ActiveTabAppearance { get { return activeTabAppearance; } }
		public AppearanceObject TabsAppearance { get { return tabsAppearance; } }
		public override TabsPosition Position { get { return position; } }
	}
	public abstract class BaseDrawTabArgs : DrawArgs {
		ObjectState state;
		public BaseDrawTabArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, ObjectState objState
		)
			:
			base(cache, bounds, appearance) {
			state = objState;
		}
		public virtual DrawTabTextArgs CreateDrawTabTextArgs(string tabText, Rectangle bounds, bool active) {
			return new DrawTabTextArgs(Cache, bounds, GetTabTextAppearance(active), tabText, IsVertical, IsActive);
		}
		protected virtual AppearanceObject GetTabTextAppearance(bool active) { return Appearance; }
		public abstract TabsPosition Position { get; }
		public Pen BorderPen { get { return Appearance.GetBorderPen(Cache); } }
		public bool IsVertical { get { return DockLayoutUtils.IsVerticalPosition(Position); } }
		public virtual bool IsActive { get { return false; } }
		public virtual ObjectState State { get { return state; } }
	}
	public class DrawTabRotateArgs : BaseDrawTabArgs {
		int elementId;
		BaseDrawTabArgs nativeArgs;
		Size bmSize;
		public DrawTabRotateArgs(BaseDrawTabArgs nativeArgs, Rectangle bounds, Size bmSize,
			int elementId, ObjectState state)
			: base(nativeArgs.Cache, bounds, nativeArgs.Appearance, state) {
			this.elementId = elementId;
			this.nativeArgs = nativeArgs;
			this.bmSize = bmSize;
		}
		public override TabsPosition Position { get { return NativeArgs.Position; } }
		public int ElementId { get { return elementId; } }
		public Size BitmapSize { get { return bmSize; } }
		public BaseDrawTabArgs NativeArgs { get { return nativeArgs; } }
	}
	public class DrawTabArgs : BaseDrawTabArgs {
		int tabIndex;
		DockLayout tabContainer;
		Rectangle tabPanelRect;
		public DrawTabArgs(GraphicsCache cache, Rectangle bounds, Rectangle tabPanelRect, AppearanceObject appearance, DockLayout tabContainer, int tabIndex, ObjectState state)
			: base(cache, bounds, appearance, state) {
			this.tabContainer = tabContainer;
			this.tabPanelRect = tabPanelRect;
			this.tabIndex = tabIndex;
		}
		public DrawTabArgs(DrawTabArgs args, Rectangle bounds, ObjectState state)
			: base(args.Cache, bounds, args.Appearance, state) {
			this.tabContainer = args.tabContainer;
			this.tabPanelRect = args.TabPanelBounds;
			this.tabIndex = args.TabIndex;
		}
		public DockLayout TabContainer { get { return tabContainer; } }
		public int TabIndex { get { return tabIndex; } }
		public DockLayout TabLayout { get { return tabContainer[TabIndex]; } }
		public int ActiveChildIndex { get { return tabContainer.ActiveChildIndex; } }
		public override bool IsActive { get { return (ActiveChildIndex == TabIndex); } }
		public Rectangle TabPanelBounds { get { return tabPanelRect; } }
		public override TabsPosition Position { get { return tabContainer.TabsPosition; } }
	}
	public class DrawTabTextArgs : DrawArgs {
		string tabText;
		bool isVertical;
		bool isActive;
		public DrawTabTextArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, string tabText, bool isVertical) :
			this(cache, bounds, appearance, tabText, isVertical, false) {
		}
		public DrawTabTextArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, string tabText, bool isVertical, bool isActive) :
			base(cache, bounds, appearance) {
			this.tabText = tabText;
			this.isVertical = isVertical;
			this.isActive = isActive;
		}
		public void DrawVerticalString() {
			const int angle = 90;
			Cache.DrawVString(TabText, Appearance.GetFont(), Appearance.GetForeBrush(Cache), Bounds, Appearance.GetStringFormat(TextOptions.DefaultOptionsNoWrapEx), angle);
		}
		public void DrawString() {
			DrawString(TabText, Bounds);
		}
		public string TabText { get { return tabText; } }
		public bool IsVertical { get { return isVertical; } }
		public bool IsActive { get { return isActive; } }
	}
}
