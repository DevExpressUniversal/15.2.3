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
using System.CodeDom.Compiler;
using System.Collections;
namespace DevExpress.XtraRichEdit.API.Word {
	#region Bookmark
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Bookmark : IWordObject {
		string Name { get; }
		Range Range { get; }
		bool Empty { get; }
		int Start { get; set; }
		int End { get; set; }
		bool Column { get; }
		WdStoryType StoryType { get; }
		void Select();
		void Delete();
		Bookmark Copy(string Name);
	}
	#endregion
	#region Bookmarks
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Bookmarks : IWordObject, IEnumerable {
		int Count { get; }
		WdBookmarkSortBy DefaultSorting { get; set; }
		bool ShowHidden { get; set; }
		Bookmark this[object Index] { get; } 
		Bookmark Add(string Name, ref object Range);
		bool Exists(string Name);
	}
	#endregion
	#region WdBookmarkSortBy
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdBookmarkSortBy {
		wdSortByName,
		wdSortByLocation
	}
	#endregion
}
