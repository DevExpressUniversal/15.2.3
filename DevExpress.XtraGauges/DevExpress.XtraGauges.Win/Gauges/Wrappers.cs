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
using DevExpress.XtraGauges.Win.Base;
using System.ComponentModel;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Model;
using System.Drawing;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraGauges.Win.Base {
	public class ImageIndicatorComponentWrapper : BasePropertyGridObjectWrapper {
		protected ImageIndicatorComponent Component {
			get { return WrappedObject as ImageIndicatorComponent; }
		}
		[Category("Appearance")]
		public BaseShapeAppearance AppearanceBackground {
			get { return Component.AppearanceBackground; }
		}
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		[Category("Geometry")]
		public PointF2D Position { get { return Component.Position; } set { Component.Position = value; } }
		[Category("Image"), DefaultValue(ImageLayoutMode.Default)]
		public ImageLayoutMode ImageLayoutMode { get { return Component.ImageLayoutMode; } set { Component.ImageLayoutMode = value; } }
		[Category("Image")]
		public Color Color { get { return Component.Color; } set { Component.Color = value; } }
		[Category("Image"), DefaultValue(false)]
		public bool AllowImageSkinning { get { return Component.AllowImageSkinning; } set { Component.AllowImageSkinning = value; } }
		[Category("Geometry")]
		public SizeF Size { get { return Component.Size; } set { Component.Size = value; } }
		internal bool ShouldSerializeSize() {
			return (Component.Image != null) ? Size != Component.Image.Size : Size != new SizeF(32, 32);
		}
		internal void ResetSize() {
			if(Component.Image != null)
				Size = Component.Image.Size;
			else
				Size = new SizeF(32, 32);
		}
		internal bool ShouldSerializeColor() {
			return Color != Color.Empty;
		}
		internal void ResetColor() {
			Color = Color.Empty;
		}
		internal bool ShouldSerializePosition() {
			return Position != new PointF2D(125, 125);
		}
		internal void ResetPosition() {
			Position = new PointF2D(125, 125);
		}
		internal bool ShouldSerializeAllowImageSkinning() {
			return AllowImageSkinning != false;
		}
		internal void ResetAllowImageSkinning() {
			AllowImageSkinning = false;
		}
	}
	public class LabelComponentWrapper : BasePropertyGridObjectWrapper {
		protected LabelComponent Component {
			get { return WrappedObject as LabelComponent; }
		}
		[Category("Appearance")]
		public BaseShapeAppearance AppearanceBackground {
			get { return Component.AppearanceBackground; }
		}
		bool ShouldSerializeAppearanceText() { return Component.AppearanceText.ShouldSerialize(); }
		void ResetAppearanceText() { Component.AppearanceText.Reset(); }
		[Category("Appearance")]
		public BaseTextAppearance AppearanceText {
			get { return Component.AppearanceText; }
		}
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		[Category("Appearance"), DefaultValue(true)]
		public bool UseColorScheme { get { return Component.UseColorScheme; } set { Component.UseColorScheme = value; } }
		[Category("Geometry")]
		public PointF2D Position { get { return Component.Position; } set { Component.Position = value; } }
		[Category("Geometry")]
		public SizeF Size { get { return Component.Size; } set { Component.Size = value; } }
		[Category("Text"),DefaultValue(LabelOrientation.Default)]
		public LabelOrientation TextOrientation { get { return Component.TextOrientation; } set { Component.TextOrientation = value; } }
		[Category("Text"),DefaultValue("Text")]
		public string Text { get { return Component.Text; } set { Component.Text = value; } }
		[Category("Text"),DefaultValue("{0}")]
		public string FormatString { get { return Component.FormatString; } set { Component.FormatString = value; } }
		[Category("Text"), DefaultValue(false)]
		public bool AllowHTMLString { get { return Component.AllowHTMLString; } set { Component.AllowHTMLString = value; } }
		bool ShouldSerializeAppearanceBackground() { return Component.AppearanceBackground.ShouldSerialize(); }
		void ResetAppearanceBackground() { Component.AppearanceBackground.Reset(); }
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
		internal bool ShouldSerializeSize() {
			return Size != new SizeF(125, 25);
		}
		internal void ResetSize() {
			Size = new SizeF(125, 25);
		}
		internal bool ShouldSerializePosition() {
			return Position != new PointF2D(125, 125);
		}
		internal void ResetPosition() {
			Position = new PointF2D(125, 125);
		}
	}
}
