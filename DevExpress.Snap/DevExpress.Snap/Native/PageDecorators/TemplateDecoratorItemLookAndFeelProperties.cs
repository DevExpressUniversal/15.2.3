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
namespace DevExpress.Snap.Native.PageDecorators {
	public static class TemplateDecoratorItemLookAndFeelProperties {
		static readonly Color selectedItemColor = Color.FromArgb(74, 203, 252);
		static readonly TemplateDecoratorItemBorder innermostItem = new TemplateDecoratorItemBorder(new SolidBrush(selectedItemColor), 1);
		static readonly TemplateDecoratorItemBorder outerItemFirstLevel = new TemplateDecoratorItemBorder(new SolidBrush(selectedItemColor), 10);
		static readonly TemplateDecoratorItemBorder outerItemSecondLevel = new TemplateDecoratorItemBorder(new SolidBrush(Color.FromArgb(191, 191, 191)), 5);
		static readonly TemplateDecoratorItemBorder outerItemThirdLevel = new TemplateDecoratorItemBorder(new SolidBrush(Color.FromArgb(217, 217, 217)), 5);
		static readonly TemplateDecoratorItemBorder[] outerItems = new TemplateDecoratorItemBorder[] { outerItemFirstLevel, outerItemSecondLevel, outerItemThirdLevel };
		static readonly TemplateDecoratorItemBorder itemHotTrack = new TemplateDecoratorItemBorder(new SolidBrush(selectedItemColor), 1);
		static readonly Brush firstLevelItemBackgroundBrush = new SolidBrush(Color.FromArgb(56, 74, 203, 252));
		static readonly Brush secondLevelItemBackgroundBrush = new SolidBrush(Color.FromArgb(130, Color.White));
		static readonly Font templateTypeFont = new Font("Arial", 8);
		static readonly Size captionTextPadding = new Size(10, 2);
		public static TemplateDecoratorItemBorder ItemHotTrack { get { return itemHotTrack; } }
		public static TemplateDecoratorItemBorder InnermostItem { get { return innermostItem; } }
		public static TemplateDecoratorItemBorder OuterItemFirstLevel { get { return outerItemFirstLevel; } }
		public static TemplateDecoratorItemBorder OuterItemSecondLevel { get { return outerItemSecondLevel; } }
		public static TemplateDecoratorItemBorder OuterItemThirdLevel { get { return outerItemThirdLevel; } }
		public static TemplateDecoratorItemBorder[] OuterItems { get { return outerItems; } }
		public static Brush FirstLevelItemBackgroundBrush { get { return firstLevelItemBackgroundBrush; } }
		public static Brush SecondLevelItemBackgroundBrush { get { return secondLevelItemBackgroundBrush; } }
		public static Font TemplateTypeFont { get { return templateTypeFont; } }
		public static Size CaptionTextPadding { get { return captionTextPadding; } }			
	}
	public class TemplateDecoratorItemBorder {
		public TemplateDecoratorItemBorder(Brush borderBrush, int borderWidth) {
			BorderBrush = borderBrush;
			BorderWidth = borderWidth;
		}
		public Brush BorderBrush { get; private set; }
		public int BorderWidth { get; private set; }
	}
}
