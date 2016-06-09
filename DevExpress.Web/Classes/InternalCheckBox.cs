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
using System.Text;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.ComponentModel;
#if ASP
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
#else
using DevExpress.vNext.Internal;
#endif
namespace DevExpress.Web.Internal.InternalCheckBox {
	public interface IInternalCheckBoxOwner {
		string AccessKey { get; }
		bool Enabled { get; }
		bool ClientEnabled { get; }
		CheckState CheckState { get; }
		string GetCheckBoxInputID();
		bool IsInputElementRequired { get; }
		AppearanceStyleBase InternalCheckBoxStyle { get; }
		InternalCheckBoxImageProperties GetCurrentCheckableImage();
		Dictionary<string, string> AccessibilityCheckBoxAttributes { get; }
	}
	public class InternalCheckboxControl : ASPxInternalWebControl {
		public const string
			InternalCheckBoxInputIDSuffix = "I",
			CheckedStateKey = "C",
			UncheckedStateKey = "U",
			IndeterminateStateKey = "I",
			FocusedCheckBoxClassName = "dxICBFocused",
			FocusedRadioButtonClassName = "dxeIRBFocused",
			CheckBoxClassName = "dxICheckBox",
			RadioButtonClassName = "dxeIRadioButton",
			CheckBoxCheckedImageName = "edtCheckBoxChecked",
			CheckBoxUncheckedImageName = "edtCheckBoxUnchecked",
			CheckBoxGrayedImageName = "edtCheckBoxGrayed",
			DesignModeSpriteImagePath = ASPxWebControl.WebImagesResourcePath + ImagesBase.SpriteImageName + ".png",
			WebSpriteCssPath = "DevExpress.Web.Css.Sprite.css",
			WebSpriteControlName = "Web",
			EditorsSpriteControlName = "Editors";
		private WebControl mainElement = null;
		private KeyboardSupportInputHelper keyboardInput = null;
		private IInternalCheckBoxOwner owner = null;
		public IInternalCheckBoxOwner Owner {
			get { return owner; }
		}
		public WebControl MainElement {
			get { return mainElement; }
		}
		public WebControl Input {
			get { return keyboardInput.Input; }
		}
		public KeyboardSupportInputHelper KeyboardInput {
			get { return keyboardInput; }
		}
		public InternalCheckboxControl(IInternalCheckBoxOwner owner)
			: base() {
			this.owner = owner;
		}
		public static string GetCheckStateKey(CheckState state) {
			switch(state) {
				case CheckState.Checked: return CheckedStateKey;
				case CheckState.Unchecked: return UncheckedStateKey;
				default: return IndeterminateStateKey;
			}
		}
		public static string SerializeFocusedStyle(AppearanceStyleBase style, IUrlResolutionService owner) {
			return string.Format("[{0},{1}]", HtmlConvertor.ToScript(style.CssClass ?? ""), HtmlConvertor.ToScript(style.GetStyleAttributes(owner).Value ?? ""));
		}
		public static CheckState GetCheckStateByKey(string key) {
			if(string.Equals(key, CheckedStateKey, StringComparison.OrdinalIgnoreCase)) return CheckState.Checked;
			if(string.Equals(key, UncheckedStateKey, StringComparison.OrdinalIgnoreCase)) return CheckState.Unchecked;
			return CheckState.Indeterminate;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.mainElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			if(Owner.IsInputElementRequired && !DesignMode) {
				this.keyboardInput = CreateKeyboardSupportInput();
				this.mainElement.Controls.Add(this.keyboardInput);
			}
			Controls.Add(this.mainElement);
		}
		protected KeyboardSupportInputHelper CreateKeyboardSupportInput() {
			string keyboardInputID = Owner.GetCheckBoxInputID();
			return new KeyboardSupportInputHelper(keyboardInputID ?? InternalCheckBoxInputIDSuffix, true, Owner.AccessibilityCheckBoxAttributes != null);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(this.keyboardInput != null) {
				RenderUtils.SetStringAttribute(this.keyboardInput.Input, "value", GetCheckStateKey(Owner.CheckState));
				this.keyboardInput.Input.AccessKey = Owner.AccessKey;
				this.keyboardInput.Input.Enabled = Owner.Enabled;
			}
			if(this.mainElement != null) {
				InternalCheckBoxImageProperties imageProperties = Owner.GetCurrentCheckableImage();
				ResolveImageURL(imageProperties);
				imageProperties.AssignToControl(this.mainElement, DesignMode, !Enabled || !Owner.Enabled || !Owner.ClientEnabled);
				Owner.InternalCheckBoxStyle.AssignToControl(this.mainElement);
				Owner.InternalCheckBoxStyle.Margins.AssignToControl(this.mainElement);
				RenderUtils.AppendDefaultDXClassName(this.mainElement, "dxichSys");
				if(Owner.AccessibilityCheckBoxAttributes != null) {
					RenderUtils.SetStringAttribute(this.mainElement, "tabindex", "0");
					foreach(var attribute in Owner.AccessibilityCheckBoxAttributes)
						RenderUtils.SetStringAttribute(this.mainElement, attribute.Key, attribute.Value);
				}
			}
			if(this.keyboardInput != null)
				this.keyboardInput.TabIndex = TabIndex;
		}
		protected void ResolveImageURL(InternalCheckBoxImageProperties properties) {
			if(Page != null) {
				properties.Url = Page.ResolveClientUrl(properties.Url);
				properties.UrlDisabled = Page.ResolveClientUrl(properties.UrlDisabled);
			}
		}
		protected override void ClearControlFields() {
			this.keyboardInput = null;
			this.mainElement = null;
		}
	}
	public static class ImagePropertiesSerializer {
		public delegate object GetImagePropertyDelegate(InternalCheckBoxImageProperties image);
		public delegate string ResolveUrlDelegate(string url);
		private static void CheckImageProperty(List<InternalCheckBoxImageProperties> images, Dictionary<int, object> properties, GetImagePropertyDelegate getPropertyFunc, int propertyNumber) {
			List<object> propertyValues = new List<object>();
			for(int i = 0; i < images.Count; i++)
				propertyValues.Add(getPropertyFunc(images[i]));
			if(propertyValues.Exists(delegate(object str) { return str != null; }))
				properties[propertyNumber] = propertyValues;
		}
		private static bool IsSpriteUsed(InternalCheckBoxImageProperties pr) {
			return string.IsNullOrEmpty(pr.Url);
		}
		public static string GetImageProperties(List<InternalCheckBoxImageProperties> images, WebControl control) {
			ResolveUrlDelegate resolveUrl = delegate(string url) { 
				if(control.Page != null)
					url = control.Page.ResolveUrl(url);
				return ResourceManager.GetResourceUrl(control.Page, url);
			};
			return GetImagePropertiesInternal(images, resolveUrl);
		}
		public static string GetImagePropertiesInternal(List<InternalCheckBoxImageProperties> images, ResolveUrlDelegate resolveUrl) {
			Dictionary<int, object> properties = new Dictionary<int, object>();
			GetImagePropertyDelegate[] getPropertyMethods = new GetImagePropertyDelegate[] {
					delegate(InternalCheckBoxImageProperties pr) { 
						return string.IsNullOrEmpty(pr.ToolTip) ? null : pr.ToolTip; 
					},
					delegate(InternalCheckBoxImageProperties pr) { 
						return pr.Width.IsEmpty ? null : pr.Width.Value as object; 
					},
					delegate(InternalCheckBoxImageProperties pr) { 
						return pr.Height.IsEmpty ? null : pr.Height.Value as object; 
					},
					delegate(InternalCheckBoxImageProperties pr) { 
						return string.IsNullOrEmpty(pr.Url) ? null : resolveUrl(pr.Url); 
					},
					delegate(InternalCheckBoxImageProperties pr) {
						if(!string.IsNullOrEmpty(pr.IconID))
							return IconsHelper.GetIconCssClass(pr.IconID, false);
						return string.IsNullOrEmpty(pr.SpriteProperties.CssClass) ||!IsSpriteUsed(pr) ? null : pr.SpriteProperties.CssClass; 
					},
					delegate(InternalCheckBoxImageProperties pr) { 
						return pr.SpriteProperties.Left.IsEmpty || !IsSpriteUsed(pr) ? null : pr.SpriteProperties.Left.Value as object; 
					},
					delegate(InternalCheckBoxImageProperties pr) { 
						return pr.SpriteProperties.Top.IsEmpty || !IsSpriteUsed(pr) ? null : pr.SpriteProperties.Top.Value as object; 
					},
					delegate(InternalCheckBoxImageProperties pr) { 
						return string.IsNullOrEmpty(pr.UrlDisabled) ? null : resolveUrl(pr.UrlDisabled); 
					},
					delegate(InternalCheckBoxImageProperties pr) { 
						if(!string.IsNullOrEmpty(pr.IconID))
							return IconsHelper.GetIconCssClass(pr.IconID, true);
						return string.IsNullOrEmpty(pr.SpriteProperties.DisabledCssClass) || !IsSpriteUsed(pr) ? null : pr.SpriteProperties.DisabledCssClass; 
					},
					delegate(InternalCheckBoxImageProperties pr) { 
						return pr.SpriteProperties.DisabledTop.IsEmpty || !IsSpriteUsed(pr) ? null : pr.SpriteProperties.DisabledTop.Value as object; 
					},
					delegate(InternalCheckBoxImageProperties pr) { 
						return pr.SpriteProperties.DisabledLeft.IsEmpty || !IsSpriteUsed(pr) ? null : pr.SpriteProperties.DisabledLeft.Value as object; 
					},
				};
			for(int i = 0; i < getPropertyMethods.Length; i++)
				CheckImageProperty(images, properties, getPropertyMethods[i], i);
			string result = HtmlConvertor.ToJSON(properties, true);
			return result.Substring(1, result.Length - 2);
		}
	}
}
