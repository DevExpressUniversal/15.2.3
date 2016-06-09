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
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public class EmptyDocumentInfo : IDocumentInfo {
		Document documentCore;
		WidgetView ownerCore;
		public EmptyDocumentInfo(WidgetView owner, Document document) {
			ownerCore = owner;
			documentCore = document;
		}
		public int Length {
			get { return Document.Parent.IsHorizontal ? Document.Width : Document.Height; }
			set {
				if(Document.Parent.IsHorizontal)
					Document.Width = value;
				else
					Document.Height = value;
			}
		}
		public int RowIndex { get; set; }
		public int ColumnIndex { get; set; }
		public int RowSpan { get; set; }
		public int ColumnSpan { get; set; }
		public Size MinSize { get { return Size.Empty; } }
		public int Height {
			get { return Document.Height; }
			set { Document.Height = value; }
		}
		public WidgetView Owner {
			get { return ownerCore; }
		}
		public void Calc(Rectangle bounds) {
			Bounds = bounds;
		}
		public Rectangle Bounds { get; set; }
		public Document Document {
			get { return documentCore; }
		}
		public int CaptionHeight { get { return 0; } }
	}
}
