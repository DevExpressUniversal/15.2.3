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
using System.Text;
namespace DevExpress.Web {
	public class ListEditItemsRequestedByFilterConditionEventArgs : EventArgs {
		int beginIndex;
		int endIndex;
		string filter;
		public ListEditItemsRequestedByFilterConditionEventArgs(int beginIndex, int endIndex, string filter)
			: base() {
			this.beginIndex = beginIndex;
			this.endIndex = endIndex;
			this.filter = filter;
		}
		public int BeginIndex { get { return beginIndex; } }
		public int EndIndex { get { return endIndex; } }
		public string Filter { get { return filter; } }
	}
	public class ListEditItemRequestedByValueEventArgs : EventArgs {
		object value;
		public ListEditItemRequestedByValueEventArgs(object value){
			this.value = value;
		}
		public object Value { get { return value; } }
	}
	public class TokenBoxCustomTokensAddedEventArgs : EventArgs {
		IEnumerable<string> customTokens;
		public TokenBoxCustomTokensAddedEventArgs(IEnumerable<string> customTokens) {
			this.customTokens = customTokens;
		}
		public IEnumerable<string> CustomTokens { get { return customTokens; } }
	}
	public delegate void ListEditItemsRequestedByFilterConditionEventHandler(object source, ListEditItemsRequestedByFilterConditionEventArgs e);
	public delegate void ListEditItemRequestedByValueEventHandler(object source, ListEditItemRequestedByValueEventArgs e);
	public delegate void TokenBoxCustomTokensAddedEventHandler(object sender, TokenBoxCustomTokensAddedEventArgs e);
}
namespace DevExpress.Web.Internal {
	public class PostBackValueSecuringEventArgs : EventArgs {
		bool isValidValue = true;
		public bool IsValidValue {
			get { return isValidValue; }
			set { isValidValue = value; }
		}
	}
}
