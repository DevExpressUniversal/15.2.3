#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfBookmark : IPdfBookmarkParent {
		readonly PdfAction action;
		PdfBookmarkList children;
		string title;
		bool isItalic;
		bool isBold;
		bool isInitiallyClosed;
		PdfDestinationObject destinationObject;
		IPdfBookmarkParent parent;
		PdfRGBColor textColor;
		internal IPdfBookmarkParent Parent {
			get { return parent; }
			set { parent = value; }
		}
		internal PdfDestinationObject DestinationObject { get { return destinationObject; } }
		public string Title {
			get { return title; }
			set {
				if (value != title) {
					title = value;
					Invalidate();
				}
			}
		}
		public PdfDestination Destination {
			get { return destinationObject == null ? null : destinationObject.GetDestination(((IPdfBookmarkParent)this).DocumentCatalog, true); }
			set {
				if (value != Destination) {
					PdfDocumentCatalog catalog = ((IPdfBookmarkParent)this).DocumentCatalog;
					if (catalog != null && value != null && catalog != value.Page.DocumentCatalog)
						throw new ArgumentException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectDestinationPage));
					destinationObject = new PdfDestinationObject(value);
					Invalidate();
				}
			}
		}
		public PdfAction Action { get { return action; } }
		public bool IsItalic {
			get { return isItalic; }
			set {
				if (value != isItalic) {
					isItalic = value;
					Invalidate();
				}
			}
		}
		public bool IsBold {
			get { return isBold; }
			set {
				if (value != isBold) {
					isBold = value;
					Invalidate();
				}
			}
		}
		public bool IsInitiallyClosed {
			get { return isInitiallyClosed; }
			set {
				if (value != isInitiallyClosed) {
					isInitiallyClosed = value;
					Invalidate();
				}
			}
		}
		public IList<PdfBookmark> Children {
			get { return children; }
			set {
				if (value == null)
					throw new ArgumentNullException("Children", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectBookmarkListValue));
				children = new PdfBookmarkList(this, value);
				Invalidate();
			}
		}
		public PdfRGBColor TextColor {
			get { return textColor; }
			set {
				textColor = value;
				Invalidate();
			}
		}
		PdfDocumentCatalog IPdfBookmarkParent.DocumentCatalog {
			get { return parent == null ? null : parent.DocumentCatalog; }
		}
		public PdfBookmark() {
			children = new PdfBookmarkList(this);
		}
		internal PdfBookmark(IPdfBookmarkParent parent, PdfOutline outline) {
			this.parent = parent;
			title = outline.Title;
			destinationObject = outline.DestinationObject;
			isItalic = outline.IsItalic;
			isBold = outline.IsBold;
			isInitiallyClosed = outline.Closed;
			children = new PdfBookmarkList(this, outline);
			textColor = PdfRGBColor.FromColor(outline.Color);
			action = outline.Action;
		}
		internal PdfBookmark(PdfBookmark bookmark, PdfDestinationObject destinationObject, PdfAction action)
			: this() {
			title = bookmark.Title;
			isItalic = bookmark.IsItalic;
			isBold = bookmark.IsBold;
			isInitiallyClosed = bookmark.IsInitiallyClosed;
			textColor = bookmark.TextColor;
			this.destinationObject = destinationObject;
			this.action = action;
		}
		void Invalidate() {
			((IPdfBookmarkParent)this).Invalidate();
		}
		void IPdfBookmarkParent.Invalidate() {
			if (parent != null)
				parent.Invalidate();
		}
	}
}
