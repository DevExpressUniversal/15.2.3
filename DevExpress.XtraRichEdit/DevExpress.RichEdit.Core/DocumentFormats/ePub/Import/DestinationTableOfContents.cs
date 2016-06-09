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
using System.Xml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Utils;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.EPub {
	#region TableOfContentDestination
	public class TableOfContentDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("navMap", OnNavigationMap);
			return result;
		}
		public TableOfContentDestination(EPubImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnNavigationMap(EPubImporter importer, XmlReader reader) {
			return new NavigationMapDestination(importer);
		}
	}
	#endregion
	#region NavigationMapDestination
	public class NavigationMapDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("navPoint", OnNavigationPoint);
			return result;
		}
		public NavigationMapDestination(EPubImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnNavigationPoint(EPubImporter importer, XmlReader reader) {
			return new NavigationPointDestination(importer);
		}
	}
	#endregion
	#region NavigationPointDestination
	public class NavigationPointDestination : ElementDestination {
		string text;
		string content;
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("navLabel", OnNavigationLabel);
			result.Add("content", OnNavigationPointContent);
			result.Add("navPoint", OnNavigationPoint);
			return result;
		}
		public NavigationPointDestination(EPubImporter importer)
			: base(importer) {
		}
		#region Properties
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public string Text { get { return text; } set { text = value; } }
		public string Content { get { return content; } set { content = value; } }
		#endregion
		static NavigationPointDestination GetThis(EPubImporter importer) {
			return (NavigationPointDestination)importer.PeekDestination();
		}
		static Destination OnNavigationLabel(EPubImporter importer, XmlReader reader) {
			return new NavigationLabelDestination(importer, GetThis(importer));
		}
		static Destination OnNavigationPointContent(EPubImporter importer, XmlReader reader) {
			return new NavigationPointContentDestination(importer, GetThis(importer));
		}
		static Destination OnNavigationPoint(EPubImporter importer, XmlReader reader) {
			return new NavigationPointDestination(importer);
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (!String.IsNullOrEmpty(Content) && !String.IsNullOrEmpty(Text)) {
				Importer.BookmarkedHtmlFiles.Add(Content, Text);
				Importer.BookmarkedHtmlFilePaths.Add(Importer.DocumentRootFolder + "/" + Content, Text);
			}
		}
	}
	#endregion
	#region NavigationLabelDestination
	public class NavigationLabelDestination : ElementDestination {
		#region Fields
		readonly NavigationPointDestination navigationPointDestination;
		#endregion
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("text", OnNavigationPointText);
			return result;
		}
		public NavigationLabelDestination(EPubImporter importer, NavigationPointDestination navigationPointDestination)
			: base(importer) {
			Guard.ArgumentNotNull(navigationPointDestination, "navigationPointDestination");
			this.navigationPointDestination = navigationPointDestination;
		}
		#region Properties
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static NavigationLabelDestination GetThis(EPubImporter importer) {
			return (NavigationLabelDestination)importer.PeekDestination();
		}
		static Destination OnNavigationPointText(EPubImporter importer, XmlReader reader) {
			return new NavigationPointTextDestination(importer, GetThis(importer).navigationPointDestination);
		}
	}
	#endregion
	#region NavigationPointContentDestination
	public class NavigationPointContentDestination : LeafElementDestination {
		readonly NavigationPointDestination navigationPointDestination;
		public NavigationPointContentDestination(EPubImporter importer, NavigationPointDestination navigationPointDestination)
			: base(importer) {
			Guard.ArgumentNotNull(navigationPointDestination, "navigationPointDestination");
			this.navigationPointDestination = navigationPointDestination;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			navigationPointDestination.Content = reader.GetAttribute("src");
		}
	}
	#endregion
	#region NavigationPointTextDestination
	public class NavigationPointTextDestination : LeafElementDestination {
		readonly NavigationPointDestination navigationPointDestination;
		public NavigationPointTextDestination(EPubImporter importer, NavigationPointDestination navigationPointDestination)
			: base(importer) {
			Guard.ArgumentNotNull(navigationPointDestination, "navigationPointDestination");
			this.navigationPointDestination = navigationPointDestination;
		}
		public override bool ProcessText(XmlReader reader) {
			navigationPointDestination.Text = reader.Value.Trim();
			return true;
		}
	}
	#endregion
}
