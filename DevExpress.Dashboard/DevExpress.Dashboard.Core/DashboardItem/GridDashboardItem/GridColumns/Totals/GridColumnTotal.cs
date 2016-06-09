#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public enum GridColumnTotalType { Auto, Count, Min, Max, Avg, Sum }
	public class GridColumnTotal {
		const string xmlTotal = "Total";
		const string xmlTotalType = "Type";
		const GridColumnTotalType DefaultTotalType = GridColumnTotalType.Count;
		GridColumnTotalType totalType = DefaultTotalType;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridColumnTotalTotalType"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultTotalType)
		]
		public GridColumnTotalType TotalType {
			get { return totalType; }
			set {
				if(totalType != value) {
					totalType = value;
					OnChanged();
				}
			}
		}
		internal event EventHandler Changed;
		public GridColumnTotal() {
		}
		public GridColumnTotal(GridColumnTotalType totalType) {
			this.totalType = totalType;
		}
		void OnChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		internal XElement SaveToXml() {
			XElement element = new XElement(xmlTotal);
			if(totalType != DefaultTotalType)
				element.Add(new XAttribute(xmlTotalType, totalType));
			return element;
		}
		internal void LoadFromXml(XElement element) {
			string totalTypeString = XmlHelper.GetAttributeValue(element, xmlTotalType);
			if(!string.IsNullOrEmpty(totalTypeString))
				totalType = XmlHelper.FromString<GridColumnTotalType>(totalTypeString);
		}
	}
}
