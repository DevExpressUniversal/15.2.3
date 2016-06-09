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

using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Web.Internal {
	public class TitleIndexSampleDataItem {
		private string fText;
		private string fNavigateUrl;
		public string Text {
			get { return fText; }
		}
		public string NavigateUrl {
			get { return fNavigateUrl; }
		}
		public TitleIndexSampleDataItem(string text, string navigateUrl) {
			fText = text;
			fNavigateUrl = navigateUrl;
		}
	}
	public class TitleIndexSampleData : IEnumerable {
		private const string SampleNavigateUrl = "javascript:void({0})";
		private readonly string[] SampleItemText = new string[] {
			"Chemicals", "Real-Estate", "Printing", "Food", "Materials", "Textiles", 
			"Cases", "Entertainment", "Fashion", "Packaging", "Minerals", "Office",
			"Lighting", "Business", "Bags", "Telecommunications", "Agriculture", 
			"Computers", "Energy", "Environment", "Industrial", "Construction", 
			"Electronics", "Security", "Auto", "Publishing", "Health", "Furniture", 
			"Luggage", "Art", "Gifts", "Metals", "Paper", "Sports"
		};
		private List<TitleIndexSampleDataItem> fData = null;
		public TitleIndexSampleData() {
			fData = CreateData();
		}
		public IEnumerator GetEnumerator() {
			return fData.GetEnumerator();
		}
		protected internal List<TitleIndexSampleDataItem> CreateData() {
			List<TitleIndexSampleDataItem> list = new List<TitleIndexSampleDataItem>(SampleItemText.Length);
			for(int i = 0; i < SampleItemText.Length; ++i)
				list.Add(new TitleIndexSampleDataItem(SampleItemText[i], string.Format(SampleNavigateUrl, i)));
			return list;
		}
	}
}
