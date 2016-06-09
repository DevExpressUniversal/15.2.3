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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Web.Internal {
	public class CloudControlSampleDataItem {
		private string fText;
		private double fValue;
		public string Text {
			get { return fText; }
		}
		public double Value {
			get { return fValue; }
		}
		public CloudControlSampleDataItem(string text, double value) {
			fText = text;
			fValue = value;
		}
	}
	public class CloudControlSampleData : IEnumerable {
		private readonly string[] SampleItemText = new string[] {
			"Chemicals", "Real-Estate", "Printing", "Food", "Materials", "Textiles", 
			"Cases", "Entertainment", "Fashion", "Packaging", "Minerals", "Office",
			"Gifts", "Paper", "Bags", "Furniture", "Agriculture", 
			"Metals", "Lighting", "Environment", "Industrial", "Construction", 
			"Electronics", "Computers", "Auto", "Publishing", "Health", "Telecommunications", 
			"Art", "Energy", "Security", "Business", "Sports"
		};
		private List<CloudControlSampleDataItem> fData = null;
		public CloudControlSampleData() {
			fData = CreateData();
		}
		public IEnumerator GetEnumerator() {
			return fData.GetEnumerator();
		}
		protected internal List<CloudControlSampleDataItem> CreateData() {
			List<CloudControlSampleDataItem> list = new List<CloudControlSampleDataItem>(SampleItemText.Length);
			for(int i = 0; i < SampleItemText.Length; ++i) {
				list.Add(new CloudControlSampleDataItem(SampleItemText[i], 1000 * Math.Pow(3, (double)i / 15)));
			}
			return list;
		}
	}
}
