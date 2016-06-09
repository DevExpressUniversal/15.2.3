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

using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class ASPxCardViewSummaryItem : ASPxSummaryItemBase {
		public ASPxCardViewSummaryItem() : base() { }
		public ASPxCardViewSummaryItem(string fieldName, SummaryItemType summaryType) : base(fieldName, summaryType) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSummaryItemSummaryType"),
#endif
 DefaultValue(SummaryItemType.None), NotifyParentProperty(true)]
		public new SummaryItemType SummaryType { get { return base.SummaryType; } set { base.SummaryType = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSummaryItemFieldName"),
#endif
 DefaultValue(""), Localizable(false), TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), NotifyParentProperty(true)]
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSummaryItemDisplayFormat"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string DisplayFormat { get { return base.DisplayFormat; } set { base.DisplayFormat = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSummaryItemValueDisplayFormat"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ValueDisplayFormat { get { return base.ValueDisplayFormat; } set { base.ValueDisplayFormat = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSummaryItemVisible"),
#endif
 DefaultValue(true), Localizable(false), NotifyParentProperty(true)]
		public new bool Visible { get { return base.Visible; } set { base.Visible = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSummaryItemTag"),
#endif
 DefaultValue(""), Localizable(false), NotifyParentProperty(true)]
		public new string Tag { get { return base.Tag; } set { base.Tag = value; } }
		protected override void OnSummaryChangedCore() {
			var summaryCollection = Collection as ASPxCardViewSummaryItemCollection;
			if(summaryCollection != null)
				summaryCollection.OnSummaryChanged(this);
		}
		public string GetTotalSummaryDisplayText(CardViewColumn column, object value) {
			return base.GetSummaryDisplayText(column, value);
		}
	}
	public class ASPxCardViewSummaryItemCollection : ASPxGridSummaryItemCollectionBase<ASPxCardViewSummaryItem> {
		public ASPxCardViewSummaryItemCollection(IWebControlObject webControlObject)
			: base(webControlObject) {
		}
		[Browsable(false)]
		public new ASPxCardViewSummaryItem this[string fieldName] { get { return base[fieldName]; } }
		[Browsable(false)]
		public new ASPxCardViewSummaryItem this[string fieldName, SummaryItemType summaryType] { get { return base[fieldName, summaryType]; } }
		public new ASPxCardViewSummaryItem Add(SummaryItemType summaryType, string fieldName) {
			return base.Add(summaryType, fieldName);
		}
	}
}
