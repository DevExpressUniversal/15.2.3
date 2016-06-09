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
namespace DevExpress.Web.Mvc {
	using System;
	using System.Drawing.Printing;
	using DevExpress.Web;
	using DevExpress.Web.Internal;
	public class MVCxCardViewSearchPanelSettings : ASPxCardViewSearchPanelSettings {
		public MVCxCardViewSearchPanelSettings()
			: base(null) {
		}
		protected internal MVCxCardViewSearchPanelSettings(MVCxCardView gridView)
			: base(gridView) {
		}
		public string CustomEditorName { get { return base.CustomEditorID; } set { base.CustomEditorID = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string CustomEditorID { get { return base.CustomEditorID; } set { base.CustomEditorID = value; } }
	}
	public class MVCxCardViewEditingSettings : ASPxCardViewEditingSettings {
		public MVCxCardViewEditingSettings()
			: base(null) {
			ShowModelErrorsForEditors = true;
		}
		public object AddNewCardRouteValues { get; set; }
		public object UpdateCardRouteValues { get; set; }
		public object DeleteCardRouteValues { get; set; }
		public object BatchUpdateRouteValues { get; set; }
		public bool ShowModelErrorsForEditors { get; set; }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			MVCxCardViewEditingSettings src = source as MVCxCardViewEditingSettings;
			if(src != null) {
				AddNewCardRouteValues = src.AddNewCardRouteValues;
				UpdateCardRouteValues = src.UpdateCardRouteValues;
				DeleteCardRouteValues = src.DeleteCardRouteValues;
				ShowModelErrorsForEditors = src.ShowModelErrorsForEditors;
				BatchUpdateRouteValues = src.BatchUpdateRouteValues;
			}
		}
	}
	public class MVCxCardViewPagerSettings : ASPxCardViewPagerSettings {
		public MVCxCardViewPagerSettings(ASPxCardView owner)
			: base(owner) {
		}
		internal MVCxCardViewPagerSettings(CardViewSettings cardViewSettings)
			: this((ASPxCardView)null) {
			CardViewSettings = cardViewSettings;
		}
		protected internal CardViewSettings CardViewSettings { get; set; }
		protected override PageSizeItemSettings CreatePageSizeItemSettings(IPropertiesOwner owner) {
			return new MVCxCardViewPageSizeItemSettings(owner);
		}
	}
	public class MVCxCardViewPageSizeItemSettings : CardViewPageSizeItemSettings {
		public MVCxCardViewPageSizeItemSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		protected internal new MVCxCardViewPagerSettings PagerSettings {
			get { return Owner as MVCxCardViewPagerSettings; }
		}
		protected override bool IsFlowLayout {
			get { return PagerSettings.CardViewSettings.Settings.LayoutMode == Layout.Flow; }
		}
	}
	public class MVCxCardViewFormatConditionCollection : CardViewFormatConditionCollection {
		public CardViewFormatConditionHighlight AddHighlight() {
			return AddCore<CardViewFormatConditionHighlight>(null);
		}
		public CardViewFormatConditionHighlight AddHighlight(Action<CardViewFormatConditionHighlight> method) {
			return AddCore<CardViewFormatConditionHighlight>(method);
		}
		public CardViewFormatConditionHighlight AddHighlight(string fieldName, string expression, GridConditionHighlightFormat format) {
			var condition = new CardViewFormatConditionHighlight {
				FieldName = fieldName,
				Expression = expression,
				Format = format
			};
			Add(condition);
			return condition;
		}
		public CardViewFormatConditionTopBottom AddTopBottom() {
			return AddCore<CardViewFormatConditionTopBottom>(null);
		}
		public CardViewFormatConditionTopBottom AddTopBottom(Action<CardViewFormatConditionTopBottom> method) {
			return AddCore<CardViewFormatConditionTopBottom>(method);
		}
		public CardViewFormatConditionTopBottom AddTopBottom(string fieldName, GridTopBottomRule rule, GridConditionHighlightFormat format) {
			var condition = new CardViewFormatConditionTopBottom {
				FieldName = fieldName,
				Rule = rule,
				Format = format
			};
			Add(condition);
			return condition;
		}
		public CardViewFormatConditionColorScale AddColorScale() {
			return AddCore<CardViewFormatConditionColorScale>(null);
		}
		public CardViewFormatConditionColorScale AddColorScale(Action<CardViewFormatConditionColorScale> method) {
			return AddCore<CardViewFormatConditionColorScale>(method);
		}
		public CardViewFormatConditionColorScale AddColorScale(string fieldName, GridConditionColorScaleFormat format) {
			var condition = new CardViewFormatConditionColorScale {
				FieldName = fieldName,
				Format = format
			};
			Add(condition);
			return condition;
		}
		public CardViewFormatConditionIconSet AddIconSet() {
			return AddCore<CardViewFormatConditionIconSet>(null);
		}
		public CardViewFormatConditionIconSet AddIconSet(Action<CardViewFormatConditionIconSet> method) {
			return AddCore<CardViewFormatConditionIconSet>(method);
		}
		public CardViewFormatConditionIconSet AddIconSet(string fieldName, GridConditionIconSetFormat format) {
			var condition = new CardViewFormatConditionIconSet {
				FieldName = fieldName,
				Format = format
			};
			Add(condition);
			return condition;
		}
		T AddCore<T>(Action<T> method) where T : GridFormatConditionBase {
			var condition = Activator.CreateInstance<T>();
			if(method != null)
				method(condition);
			Add(condition);
			return condition;
		}
	}
	public class MVCxCardViewExportSettings {
		static readonly object renderBrick = new object();
		public MVCxCardViewExportSettings() {
			LeftMargin = ASPxCardViewExporter.DefaultMargin;
			TopMargin = ASPxCardViewExporter.DefaultMargin;
			RightMargin = ASPxCardViewExporter.DefaultMargin;
			BottomMargin = ASPxCardViewExporter.DefaultMargin;
			FileName = string.Empty;
			CardWidth = ASPxCardViewExporter.DefaultCardWidth;
			ExportSelectedCardsOnly = false;
			Styles = new CardViewExportStyles(null);
			PaperKind = PaperKind.Letter;
			PaperName = string.Empty;
		}
		public ASPxCardViewExportRenderingEventHandler RenderBrick { get; set; }
		public EventHandler BeforeExport { get; set; }
		public int LeftMargin { get; set; }
		public int TopMargin { get; set; }
		public int RightMargin { get; set; }
		public int BottomMargin { get; set; }
		public string FileName { get; set; }
		public bool ExportSelectedCardsOnly { get; set; }
		public int CardWidth { get; set; }
		public bool PrintSelectCheckBox { get; set; }
		public bool Landscape { get; set; }
		public CardViewExportStyles Styles { get; private set; }
		public PaperKind PaperKind { get; set; }
		public string PaperName { get; set; }
	}
}
