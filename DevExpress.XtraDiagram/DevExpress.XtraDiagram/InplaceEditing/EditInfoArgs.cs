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
using DevExpress.LookAndFeel;
using DevExpress.Utils;
namespace DevExpress.XtraDiagram.InplaceEditing {
	public class DiagramEditViewInfo {
		readonly Font font;
		readonly Color backColor;
		readonly Color foreColor;
		public DiagramEditViewInfo(Font font, Color backColor, Color foreColor) {
			this.font = font;
			this.backColor = backColor;
			this.foreColor = foreColor;
		}
		public Font Font { get { return font; } }
		public Color BackColor { get { return backColor; } }
		public Color ForeColor { get { return foreColor; } }
	}
	public class DiagramEditorRects {
		Rectangle editorBounds;
		Rectangle textRect;
		Rectangle clipRect;
		public DiagramEditorRects(Rectangle editorBounds, Rectangle textRect, Rectangle clipRect) {
			this.editorBounds = editorBounds;
			this.textRect = textRect;
			this.clipRect = clipRect;
		}
		public Rectangle EditorBounds { get { return editorBounds; } }
		public Rectangle TextRect { get { return textRect; } }
		public Rectangle ClipRect { get { return clipRect; } }
	}
	public class DiagramEditInfoArgs {
		Rectangle adornerBounds;
		DiagramEditorRects editorRects;
		string editValue;
		UserLookAndFeel lookAndFeel;
		bool rightToLeft;
		DiagramEditViewInfo editView;
		public DiagramEditInfoArgs(Rectangle adornerBounds, DiagramEditorRects editorRects, DiagramEditViewInfo editView, string editValue, UserLookAndFeel lookAndFeel, bool rightToLeft) {
			this.rightToLeft = rightToLeft;
			this.lookAndFeel = lookAndFeel;
			this.editorRects = editorRects;
			this.adornerBounds = adornerBounds;
			this.editValue = editValue;
			this.editView = editView;
		}
		internal DiagramEditInfoArgs UpdateRects(Rectangle adornerBounds, DiagramEditorRects editorRects ) {
			this.adornerBounds = adornerBounds;
			this.editorRects = editorRects;
			return this;
		}
		internal void UpdateEditValue(string editValue) {
			this.editValue = editValue;
		}
		internal void UpdateAdornerBounds(Rectangle adornerBounds) {
			this.adornerBounds = adornerBounds;
		}
		public bool RightToLeft { get { return rightToLeft; } }
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		public Rectangle AdornerBounds { get { return adornerBounds; } }
		public DiagramEditorRects EditorRects { get { return editorRects; } }
		public string EditValue { get { return editValue; } }
		public DiagramEditViewInfo EditView { get { return editView; } }
	}
}
