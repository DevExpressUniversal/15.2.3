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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using System.IO;
namespace DevExpress.Utils.FormShadow {
	public class ShadowWindowInfo : ObjectInfoArgs {
		public ShadowWindowTypes WindowType { get; set; }
		public bool Active { get; set; }
	}
	public class BeakFormInfo : ObjectInfoArgs {
		public BeakFormAlignment BeakAlignment { get; set; }
		public bool IsBorder { get; set; }
	}
	public abstract class SkinShadowWindowPainter : StyleObjectPainter {
		ISkinProvider providerCore;
		public SkinShadowWindowPainter(ISkinProvider provider) {
			this.providerCore = provider;
			isActive = false;
			IsGlow = false;
			ActiveGlowColor = InactiveGlowColor = Color.Empty;
		}
		public abstract SkinElement GetSkinElementByWindowType(ShadowWindowTypes windowType);
		public abstract int GetOffsetByWindowType(ShadowWindowTypes windowType);
		public ISkinProvider Provider { get { return providerCore; } }
		protected virtual SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			ShadowWindowInfo hdwi = e as ShadowWindowInfo;
			SkinElementInfo info = new SkinElementInfo(GetSkinElementByWindowType(hdwi.WindowType), e.Bounds);
			info.State = e.State;
			info.Cache = e.Cache;
			info.ImageIndex = hdwi.Active ? 0 : 1;
			this.isActive = hdwi.Active;
			if(IsGlow) {
				info.ImageIndex = 0;
				info.Attributes = GetImageAttributes();
			}
			return info;
		}
		bool isActive;
		public bool IsGlow { get; set; }
		public Color ActiveGlowColor { get; set; }
		public Color InactiveGlowColor { get; set; }
		Color CurrentColor { get { return isActive ? ActiveGlowColor : InactiveGlowColor; } }
		protected virtual System.Drawing.Imaging.ImageAttributes GetImageAttributes() {
			var targetColor = CurrentColor;
			float fr = (float)targetColor.R / 255;
			float fg = (float)targetColor.G / 255;
			float fb = (float)targetColor.B / 255;
			float[][] ptsArray = {
				new float[] {1, 0, 0, 0, 0},
				new float[] {0, 1, 0, 0, 0},
				new float[] {0, 0, 1, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {fr, fg, fb, 0, 1},
			};
			System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix(ptsArray);
			System.Drawing.Imaging.ImageAttributes imgAttribs = new System.Drawing.Imaging.ImageAttributes();
			imgAttribs.SetColorMatrix(colorMatrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Default);
			return imgAttribs;
		}
		Dictionary<ShadowWindowTypes, SkinElement> defaultGlowSkinElement = new Dictionary<ShadowWindowTypes, SkinElement>();
		protected SkinElement GetDefaultGlowSkinElement(ShadowWindowTypes windowType, string elementName) {
			SkinElement skinElement = null;
			if(defaultGlowSkinElement.TryGetValue(windowType, out skinElement))
				return skinElement;
			var skinBuilderElementInfo = new SkinBuilderElementInfo();
			using(var stream = GetImageStreamByWindowType(windowType, elementName)) {
				if(stream == null) return null;
				var image = Image.FromStream(stream);
				skinBuilderElementInfo.Image = new SkinImage(image);
			}
			skinBuilderElementInfo.Image.ImageCount = 2;
			switch(windowType) {
				case ShadowWindowTypes.Left:
				case ShadowWindowTypes.Right:
					skinBuilderElementInfo.Image.SizingMargins.Top = 10;
					skinBuilderElementInfo.Image.SizingMargins.Bottom = 10;
					skinBuilderElementInfo.Image.Layout = SkinImageLayout.Horizontal;
					break;
				case ShadowWindowTypes.Top:
				case ShadowWindowTypes.Bottom:
					skinBuilderElementInfo.Image.Layout = SkinImageLayout.Vertical;
					break;
			}
			skinElement = new SkinElement(null, elementName, skinBuilderElementInfo);
			defaultGlowSkinElement.Add(windowType, skinElement);
			return skinElement;
		}
		Stream GetImageStreamByWindowType(ShadowWindowTypes windowType, string elementName) {
			string glowResourcesPath = "DevExpress.Utils.FormShadow.Resources";
			var imageName = elementName;
			if(string.IsNullOrEmpty(imageName)) return null;
			return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("{0}.{1}.png", glowResourcesPath, imageName));
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return SkinElementPainter.Default.GetObjectClientRectangle(UpdateInfo(e));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SkinElementPainter.Default.DrawObject(UpdateInfo(e));
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return SkinElementPainter.Default.CalcBoundsByClientRectangle(UpdateInfo(e), client);
		}
		public void ReleaseAdditionalGlowResources() {
			if(defaultGlowSkinElement == null) return;
			foreach(var item in defaultGlowSkinElement) {
				((SkinElement)(item.Value)).Image.Image.Dispose();
				((SkinElement)(item.Value)).Image.Image = null;
			}
			defaultGlowSkinElement.Clear();
			defaultGlowSkinElement = null;
		}
	}
	public class XtraFormSkinShadowPainter : SkinShadowWindowPainter {
		public XtraFormSkinShadowPainter(ISkinProvider provider) : base(provider) { }
		public override SkinElement GetSkinElementByWindowType(ShadowWindowTypes windowType) {
			Skin frmSkin = FormSkins.GetSkin(Provider);
			SkinElement res = GetSkinElement(frmSkin, windowType);
			if(res == null) {
				frmSkin = FormSkins.GetSkin(null);
				res = GetSkinElement(frmSkin, windowType);
			}
			return res;
		}
		SkinElement GetSkinElement(Skin skin, ShadowWindowTypes windowType) {
			var skinElementName = GetSkinElementNameByWindowType(windowType);
			if(string.IsNullOrEmpty(skinElementName)) return null;
			var res = skin[skinElementName];
			if(res == null && IsGlow)
				res = GetDefaultGlowSkinElement(windowType, GetSkinElementNameByWindowType(windowType));
			return res;
		}
		string GetSkinElementNameByWindowType(ShadowWindowTypes windowType) {
			switch(windowType) {
				case ShadowWindowTypes.Left: return IsGlow ? FormSkins.SkinFormDecoratorGlowFrameLeft : FormSkins.SkinFormDecoratorFrameLeft;
				case ShadowWindowTypes.Top: return IsGlow ? FormSkins.SkinFormDecoratorGlowFrameTop : FormSkins.SkinFormDecoratorFrameTop;
				case ShadowWindowTypes.Right: return IsGlow ? FormSkins.SkinFormDecoratorGlowFrameRight : FormSkins.SkinFormDecoratorFrameRight;
				case ShadowWindowTypes.Bottom: return IsGlow ? FormSkins.SkinFormDecoratorGlowFrameBottom : FormSkins.SkinFormDecoratorFrameBottom;
			}
			return string.Empty;
		}
		public override int GetOffsetByWindowType(ShadowWindowTypes windowType) {
			if(IsGlow) return 1;
			Skin frmSkin = FormSkins.GetSkin(Provider);
			return frmSkin.Properties.GetInteger(FormSkins.OptDecoratorOffset);
		}
	}
	public class ToolWindowSkinShadowPainter : SkinShadowWindowPainter {
		public ToolWindowSkinShadowPainter(ISkinProvider provider) : base(provider) { }
		public override SkinElement GetSkinElementByWindowType(ShadowWindowTypes windowType) {
			Skin frmSkin = FormSkins.GetSkin(Provider);
			switch(windowType) {
				case ShadowWindowTypes.Left: return frmSkin[FormSkins.SkinToolbarDecoratorFrameLeft];
				case ShadowWindowTypes.Top: return frmSkin[FormSkins.SkinToolbarDecoratorFrameTop];
				case ShadowWindowTypes.Right: return frmSkin[FormSkins.SkinToolbarDecoratorFrameRight];
				case ShadowWindowTypes.Bottom: return frmSkin[FormSkins.SkinToolbarDecoratorFrameBottom];
			}
			return null;
		}
		public override int GetOffsetByWindowType(ShadowWindowTypes windowType) {
			Skin frmSkin = FormSkins.GetSkin(Provider);
			return frmSkin.Properties.GetInteger(FormSkins.OptToolbarShadowOffset);
		}
	}
	public class RibbonFormSkinShadowPainter : SkinShadowWindowPainter {
		public RibbonFormSkinShadowPainter(ISkinProvider provider) : base(provider) { }
		public override SkinElement GetSkinElementByWindowType(ShadowWindowTypes windowType) {
			Skin frmSkin = RibbonSkins.GetSkin(Provider);
			SkinElement res = GetSkinElement(frmSkin, windowType, true);
			hasSkinImages = true;
			if(res == null) {
				frmSkin = FormSkins.GetSkin(null);
				res = GetSkinElement(frmSkin, windowType, false);
				hasSkinImages = false;
			}
			return res;
		}
		SkinElement GetSkinElement(Skin skin, ShadowWindowTypes windowType, bool ribbon) {
			var skinElementName = GetSkinElementNameByWindowType(windowType, ribbon);
			if(string.IsNullOrEmpty(skinElementName)) return null;
			var res = skin[skinElementName];
			if(res == null && IsGlow)
				res = GetDefaultGlowSkinElement(windowType, GetSkinElementNameByWindowType(windowType, true));
			return res;
		}
		struct SkinElementNames {
			public SkinElementNames(string xtraForm, string glowXtraForm, string ribbon, string glowRibbon) {
				Ribbon = ribbon;
				GlowRibbon = glowRibbon;
				XtraForm = xtraForm;
				GlowXtraForm = glowXtraForm;
			}
			public string Ribbon;
			public string GlowRibbon;
			public string XtraForm;
			public string GlowXtraForm;
			public string GetElementName(bool isRibbon, bool isGlow) {
				if(isRibbon)
					return isGlow ? GlowRibbon : Ribbon;
				return isGlow ? GlowXtraForm : XtraForm;
			}
		}
		Dictionary<ShadowWindowTypes, SkinElementNames> windowTypeToElementName = new Dictionary<ShadowWindowTypes, SkinElementNames>();
		Dictionary<ShadowWindowTypes, SkinElementNames> WindowTypeToElementName {
			get {
				if(windowTypeToElementName.Count == 0) {
					windowTypeToElementName.Add(ShadowWindowTypes.Left, new SkinElementNames(FormSkins.SkinFormDecoratorFrameLeft, FormSkins.SkinFormDecoratorGlowFrameLeft, RibbonSkins.SkinFormDecoratorFrameLeft , RibbonSkins.SkinFormDecoratorGlowFrameLeft));
					windowTypeToElementName.Add(ShadowWindowTypes.Top, new SkinElementNames(FormSkins.SkinFormDecoratorFrameTop, FormSkins.SkinFormDecoratorGlowFrameTop, RibbonSkins.SkinFormDecoratorFrameTop, RibbonSkins.SkinFormDecoratorGlowFrameTop));
					windowTypeToElementName.Add(ShadowWindowTypes.Right, new SkinElementNames(FormSkins.SkinFormDecoratorFrameRight, FormSkins.SkinFormDecoratorGlowFrameRight, RibbonSkins.SkinFormDecoratorFrameRight, RibbonSkins.SkinFormDecoratorGlowFrameRight));
					windowTypeToElementName.Add(ShadowWindowTypes.Bottom, new SkinElementNames(FormSkins.SkinFormDecoratorFrameBottom, FormSkins.SkinFormDecoratorGlowFrameBottom, RibbonSkins.SkinFormDecoratorFrameBottom, RibbonSkins.SkinFormDecoratorGlowFrameBottom));
					windowTypeToElementName.Add(ShadowWindowTypes.Composite, new SkinElementNames());
				}
				return windowTypeToElementName;
			}
		}
		string GetSkinElementNameByWindowType(ShadowWindowTypes windowType, bool ribbon) {
			SkinElementNames skinElementNames;
			if(!WindowTypeToElementName.TryGetValue(windowType, out skinElementNames))
				return string.Empty;
			return skinElementNames.GetElementName(ribbon, IsGlow);
		}
		bool hasSkinImages = true;
		public override int GetOffsetByWindowType(ShadowWindowTypes windowType) {
			if(IsGlow) return 2;
			Skin frmSkin = null;
			if(hasSkinImages) {
				frmSkin = RibbonSkins.GetSkin(Provider);
				return frmSkin.Properties.GetInteger(RibbonSkins.OptDecoratorOffset);
			}
			frmSkin = FormSkins.GetSkin(null);
			return frmSkin.Properties.GetInteger(FormSkins.OptDecoratorOffset);
		}
	}
	public class BeakFormSkinShadowPainter : SkinShadowWindowPainter {
		public BeakFormSkinShadowPainter(ISkinProvider provider) : base(provider) { }
		public override SkinElement GetSkinElementByWindowType(ShadowWindowTypes windowType) {
			Skin frmSkin = FormSkins.GetSkin(Provider);
			switch(windowType) {
				case ShadowWindowTypes.Left: return frmSkin[FormSkins.SkinBeakFormShadowFrameLeft];
				case ShadowWindowTypes.Top: return frmSkin[FormSkins.SkinBeakFormShadowFrameTop];
				case ShadowWindowTypes.Right: return frmSkin[FormSkins.SkinBeakFormShadowFrameRight];
				case ShadowWindowTypes.Bottom: return frmSkin[FormSkins.SkinBeakFormShadowFrameBottom];
			}
			return null;
		}
		public SkinElement GetSkinElementByBeakAlignment(BeakFormAlignment beakAlignment) {
			Skin frmSkin = FormSkins.GetSkin(Provider);
			switch(beakAlignment) {
				case BeakFormAlignment.Left: return frmSkin[FormSkins.SkinBeakFormRightArrow];
				case BeakFormAlignment.Top: return frmSkin[FormSkins.SkinBeakFormDownArrow];
				case BeakFormAlignment.Right: return frmSkin[FormSkins.SkinBeakFormLeftArrow];
				case BeakFormAlignment.Bottom: return frmSkin[FormSkins.SkinBeakFormUpArrow];
			}
			return null;
		}
		public SkinElement GetBorderSkinElement() {
			Skin frmSkin = FormSkins.GetSkin(Provider);
			return frmSkin[FormSkins.SkinBeakFormBorder];
		}
		public override int GetOffsetByWindowType(ShadowWindowTypes windowType) {
			Skin frmSkin = FormSkins.GetSkin(Provider);
			return frmSkin.Properties.GetInteger(FormSkins.OptBeakFormShadowOffset);
		}
		public int GetBorderThickness() {
			var border = GetBorderSkinElement();
			return border.Properties.GetInteger(FormSkins.OptThickness);
		}
		protected override SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			SkinElement element = null;
			if(e is BeakFormInfo) {
				if((e as BeakFormInfo).IsBorder) element = GetBorderSkinElement();
				else element = GetSkinElementByBeakAlignment((e as BeakFormInfo).BeakAlignment);
			}
			else if(e is ShadowWindowInfo) element = GetSkinElementByWindowType((e as ShadowWindowInfo).WindowType);
			if(element == null) return null;
			SkinElementInfo info = new SkinElementInfo(element, e.Bounds);
			info.State = e.State;
			info.Cache = e.Cache;
			if(e is ShadowWindowInfo)
				info.ImageIndex = (e as ShadowWindowInfo).Active ? 0 : 1;
			return info;
		}
	}
}
