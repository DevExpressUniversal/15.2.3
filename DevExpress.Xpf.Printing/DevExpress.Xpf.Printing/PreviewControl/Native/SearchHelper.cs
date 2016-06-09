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
using System.Linq;
using System.Text;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native.Navigation;
namespace DevExpress.Xpf.Printing.PreviewControl.Native {
	public class SearchHelper {
		readonly SearchHelperBase searchHelper = new SearchHelperBase();
		string actualText;
		bool actualCaseSensitive;
		bool actualWholeWord;
		SearchDirection actualDirection;
		string ActualText {
			get {
				return actualText;
			}
			set {
				if(actualText == value)
					return;
				actualText = value;
				searchHelper.ResetSearchResults();
			}
		}
		bool ActualCaseSensitive {
			get {
				return actualCaseSensitive;
			}
			set {
				if(actualCaseSensitive == value)
					return;
				actualCaseSensitive = value;
				searchHelper.ResetSearchResults();
			}
		}
		bool ActualWholeWord {
			get {
				return actualWholeWord;
			}
			set {
				if(actualWholeWord == value)
					return;
				actualWholeWord = value;
				searchHelper.ResetSearchResults();
			}
		}
		SearchDirection ActualDirection {
			get {
				return actualDirection;
			}
			set {
				if(actualDirection == value)
					return;
				actualDirection = value;
			}
		}
		public BrickPagePair FindNext(PrintingSystemBase ps, TextSearchParameter searchParameter) {
			AssignParameters(searchParameter);
			return searchHelper.CircleFindNext(ps, ActualText, ActualDirection, ActualWholeWord, ActualCaseSensitive);
		}
		void AssignParameters(TextSearchParameter searchParameter) {
			ActualText = searchParameter.Text;
			ActualCaseSensitive = searchParameter.IsCaseSensitive;
			ActualWholeWord = searchParameter.WholeWord;
			ActualDirection = searchParameter.SearchDirection == TextSearchDirection.Forward ? SearchDirection.Down : SearchDirection.Up;
		}
	}
}
