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

namespace DevExpress.XtraPrinting.XamlExport {
	public static class XamlTag {
		public static readonly string Canvas = "Canvas";
		public static readonly string CanvasClip = "Canvas.Clip";
		public static readonly string RectangleGeometry = "RectangleGeometry";
		public static readonly string TextBlock = "TextBlock";
		public static readonly string Grid = "Grid";
		public static readonly string Border = "Border";
		public static readonly string Line = "Line";
		public static readonly string CheckBox = "CheckBox";
		public static readonly string CanvasResources = "Canvas.Resources";
		public static readonly string Style = "Style";
		public static readonly string Setter = "Setter";
		public static readonly string Image = "Image";
		public static readonly string String = "String";
		public static readonly string Base64StringImageConverter = "Base64StringImageConverter";
		public static readonly string ResourceDictionary = "ResourceDictionary";
		public static readonly string ResourceDictionaryMergedDictionaries = "ResourceDictionary.MergedDictionaries";
		public static readonly string ContentPresenter = "ContentPresenter";
		public static readonly string ImageEffect = "Image.Effect";
		public static readonly string TileEffect = "TileEffect";
		public static readonly string TextBlockRenderTransform = "TextBlock.RenderTransform";
		public static readonly string TransformGroup = "TransformGroup";
		public static readonly string RotateTransform = "RotateTransform";
		public static readonly string ResolveImageConverter = "ResolveImageConverter"; 
#if !SL
		public static readonly string RepositoryImageConverter = "RepositoryImageConverter";
		public static readonly string TextBlockLayoutTransform = "TextBlock.LayoutTransform";
#endif
	}
}
