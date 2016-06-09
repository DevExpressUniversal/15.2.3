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
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.Viewer {
	public interface IDrawPropertiesContainer {
		IEnumerable<DrawProperties> GetDrawProperties { get; }
	}
	public class DrawProperties : IDrawPropertiesContainer {
		DrawProperties additionalProperties;
		public Rectangle Bounds { get; private set; }
		public DrawProperties(Rectangle bounds) {
			Bounds = bounds;
		}
		public DrawProperties(Rectangle bounds, DrawProperties additionalProperties) : this(bounds) {
			this.additionalProperties = additionalProperties;
		}
		IEnumerable<DrawProperties> IDrawPropertiesContainer.GetDrawProperties {
			get {
				if(additionalProperties != null)
					yield return additionalProperties;
				yield return this;
			}
		}
	}
	public class TextDrawProperties : DrawProperties {
		public string Text { get; private set; }
		public Font Font { get; private set; }
		public Color Color { get; private set; }
		public StringFormat Format { get; private set; }
		public TextDrawProperties(string text, Rectangle bounds, Font font, Color color, StringFormat format)
			: this(text, bounds, font, color, format, null) {
		}
		public TextDrawProperties(string text, Rectangle bounds, Font font, Color color, StringFormat format, TextDrawProperties additionalProperties)
			: base(bounds, additionalProperties) {
			Text = text;
			Font = font;
			Color = color;
			Format = format;
		}
	}
	public class ImageDrawProperties : DrawProperties {
		public Image Image { get; private set; }
		public ImageDrawProperties(Image image, Rectangle bounds)
			: base(bounds) {
			Image = image;
		}
	}
	public class CardDrawProperties : IDrawPropertiesContainer {
		DrawProperties titleProperties;
		DrawProperties subTitleProperties;
		DrawProperties subValue1Properties;
		DrawProperties subValue2Properties;
		DrawProperties mainValueProperties;
		DrawProperties imageDrawProperties;
		public DrawProperties TitleProperties { get { return titleProperties; } set { titleProperties = value; } }
		public DrawProperties SubTitleProperties { get { return subTitleProperties; } set { subTitleProperties = value; } }
		public DrawProperties SubValue1Properties { get { return subValue1Properties; } set { subValue1Properties = value; } }
		public DrawProperties SubValue2Properties { get { return subValue2Properties; } set { subValue2Properties = value; } }
		public DrawProperties MainValueProperties { get { return mainValueProperties; } set { mainValueProperties = value; } }
		public DrawProperties ImageDrawProperties { get { return imageDrawProperties; } set { imageDrawProperties = value; } }
		IEnumerable<DrawProperties> IDrawPropertiesContainer.GetDrawProperties {
			get {
				if(imageDrawProperties != null)
					foreach(DrawProperties drawProperties in ((IDrawPropertiesContainer)imageDrawProperties).GetDrawProperties)
						yield return drawProperties;
				if(mainValueProperties != null)
					foreach(DrawProperties drawProperties in ((IDrawPropertiesContainer)mainValueProperties).GetDrawProperties)
						yield return drawProperties;
				if(subValue2Properties != null)
					foreach(DrawProperties drawProperties in ((IDrawPropertiesContainer)subValue2Properties).GetDrawProperties)
						yield return drawProperties;
				if(subValue1Properties != null)
					foreach(DrawProperties drawProperties in ((IDrawPropertiesContainer)subValue1Properties).GetDrawProperties)
						yield return drawProperties;
				if(subTitleProperties != null)
					foreach(DrawProperties drawProperties in ((IDrawPropertiesContainer)subTitleProperties).GetDrawProperties)
						yield return drawProperties;
				if(titleProperties != null)
					foreach(DrawProperties drawProperties in ((IDrawPropertiesContainer)titleProperties).GetDrawProperties)
						yield return drawProperties;
			}
		}
	}
}
