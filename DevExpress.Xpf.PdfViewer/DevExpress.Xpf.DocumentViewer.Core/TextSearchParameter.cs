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

using DevExpress.Mvvm;
namespace DevExpress.Xpf.DocumentViewer {
	public enum TextSearchDirection {
		Forward = 0,
		Backward = 1
	}
	public class TextSearchParameter : BindableBase {
		string text;
		bool wholeWord;
		bool isCaseSensitive;
		TextSearchDirection searchSearchDirection;
		int currentPage;
		public string Text {
			get { return text; }
			set { SetProperty(ref text, value, () => Text); }
		}
		public bool WholeWord {
			get { return wholeWord; }
			set { SetProperty(ref wholeWord, value, () => WholeWord); }
		}
		public bool IsCaseSensitive {
			get { return isCaseSensitive; }
			set { SetProperty(ref isCaseSensitive, value, () => IsCaseSensitive); }
		}
		public TextSearchDirection SearchDirection {
			get { return searchSearchDirection; }
			set { SetProperty(ref searchSearchDirection, value, () => SearchDirection); }
		}
		public int CurrentPage {
			get { return currentPage; }
			set { SetProperty(ref currentPage, value, () => CurrentPage); }
		}
		public TextSearchParameter() {
			CurrentPage = 1;
			SearchDirection = TextSearchDirection.Forward;
		}
	}
}
