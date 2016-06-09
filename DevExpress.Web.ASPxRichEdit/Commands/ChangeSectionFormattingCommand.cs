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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Forms;
using System;
using DevExpress.Office;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class ChangeSectionFormattingCommand : WebRichEditPropertyStateBasedCommand<SectionCommandState, JSONSectionProperty> {
		public ChangeSectionFormattingCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeSectionProperties; } }
		protected override bool IsEnabled() { return true; }
		static Dictionary<JSONSectionProperty, JSONModelModifier<SectionCommandState>> modifiers = CreateModifiers();
		static Dictionary<JSONSectionProperty, JSONModelModifier<SectionCommandState>> CreateModifiers() {
			var result = new Dictionary<JSONSectionProperty, JSONModelModifier<SectionCommandState>>();
			result.Add(JSONSectionProperty.Landscape, (DocumentModel model) => new SectionPropertiesLandscapeModifier(model));
			result.Add(JSONSectionProperty.ColumnCount, (DocumentModel model) => new SectionPropertiesColumnCountModifier(model));
			result.Add(JSONSectionProperty.ColumnsInfo, (DocumentModel model) => new SectionPropertiesColumnsInfoModifier(model));
			result.Add(JSONSectionProperty.EqualWidthColumns, (DocumentModel model) => new SectionPropertiesEqualWidthColumnsModifier(model));
			result.Add(JSONSectionProperty.MarginBottom, (DocumentModel model) => new SectionPropertiesMarginBottomModifier(model));
			result.Add(JSONSectionProperty.MarginLeft, (DocumentModel model) => new SectionPropertiesMarginLeftModifier(model));
			result.Add(JSONSectionProperty.MarginRight, (DocumentModel model) => new SectionPropertiesMarginRightModifier(model));
			result.Add(JSONSectionProperty.MarginTop, (DocumentModel model) => new SectionPropertiesMarginTopModifier(model));
			result.Add(JSONSectionProperty.PageHeight, (DocumentModel model) => new SectionPropertiesPageHeightModifier(model));
			result.Add(JSONSectionProperty.PageWidth, (DocumentModel model) => new SectionPropertiesPageWidthModifier(model));
			result.Add(JSONSectionProperty.Space, (DocumentModel model) => new SectionPropertiesSpaceModifier(model));
			result.Add(JSONSectionProperty.StartType, (DocumentModel model) => new SectionPropertiesStartTypeModifier(model));
			result.Add(JSONSectionProperty.DifferentFirstPage, (DocumentModel model) => new SectionPropertiesDifferentFirstPageModifier(model));
			return result;
		}
		protected override IModelModifier<SectionCommandState> CreateModifier(JSONSectionProperty property) {
			JSONModelModifier<SectionCommandState> creator;
			if(!modifiers.TryGetValue(property, out creator))
				throw new ArgumentException();
			return creator(DocumentModel);
		}
		protected override bool IsValidOperation() {
			return Manager.Client.DocumentCapabilitiesOptions.SectionsAllowed;
		}
	}
	public abstract class SectionPropertiesModifier<T> : SectionModelModifier<T> {
		public SectionPropertiesModifier(DocumentModel model) :base(model) { }
		protected override void ModifyCore(SectionIndex sectionIndex, T value) {
			Section section = DocumentModel.Sections[sectionIndex];
			ModifySectionCore(section, sectionIndex, value);
		}
		public virtual void ModifySectionCore(Section section, SectionIndex index, T newValue) {
			SectionPropertyModifier<T> modifier = CreateModifier(newValue);
			modifier.ModifySection(section, index);
		}
		protected abstract SectionPropertyModifier<T> CreateModifier(T newValue);
		protected DocumentModelUnitConverter UnitConverter { get { return DocumentModel.UnitConverter; } }
	}
	public class SectionPropertiesLandscapeModifier : SectionPropertiesModifier<bool> {
		public SectionPropertiesLandscapeModifier(DocumentModel model) : base(model) { }
		protected override SectionPropertyModifier<bool> CreateModifier(bool newValue) {
			return new SectionPageOrientationLandscapeModifier(newValue);
		}
	}
	public class SectionPropertiesDifferentFirstPageModifier : SectionPropertiesModifier<bool> {
		public SectionPropertiesDifferentFirstPageModifier(DocumentModel model) : base(model) { }
		public override void ModifySectionCore(Section section, SectionIndex index, bool newValue) {
			section.GeneralSettings.BeginUpdate();
			try {
				section.GeneralSettings.DifferentFirstPage = newValue;
			} finally {
				section.GeneralSettings.EndUpdate();
			}
		}
		protected override SectionPropertyModifier<bool> CreateModifier(bool newValue) { return null; }
	}
	public class SectionPropertiesStartTypeModifier : SectionPropertiesModifier<SectionStartType> {
		public SectionPropertiesStartTypeModifier(DocumentModel model) : base(model) { }
		public override void ModifySectionCore(Section section, SectionIndex index, SectionStartType newValue) {
			section.GeneralSettings.BeginUpdate();
			try {
				section.GeneralSettings.StartType = newValue;
			}
			finally {
				section.GeneralSettings.EndUpdate();
			}
		}
		protected override SectionPropertyModifier<SectionStartType> CreateModifier(SectionStartType newValue) { return null; }
	}
	#region Margins Modifiers
	public class SectionPropertiesMarginBottomModifier : SectionPropertiesModifier<int> {
		public SectionPropertiesMarginBottomModifier(DocumentModel model) : base(model) { }
		public override void ModifySectionCore(Section section, SectionIndex index, int newValue) {
			section.Margins.BeginUpdate();
			try {
				section.Margins.Bottom = UnitConverter.TwipsToModelUnits(newValue);
			}
			finally {
				section.Margins.EndUpdate();
			}
		}
		protected override SectionPropertyModifier<int> CreateModifier(int newValue) { return null; }
	}
	public class SectionPropertiesMarginLeftModifier : SectionPropertiesModifier<int> {
		public SectionPropertiesMarginLeftModifier(DocumentModel model) : base(model) { }
		public override void ModifySectionCore(Section section, SectionIndex index, int newValue) {
			section.Margins.BeginUpdate();
			try {
				section.Margins.Left = UnitConverter.TwipsToModelUnits(newValue);
			}
			finally {
				section.Margins.EndUpdate();
			}
		}
		protected override SectionPropertyModifier<int> CreateModifier(int newValue) { return null; }
	}
	public class SectionPropertiesMarginRightModifier : SectionPropertiesModifier<int> {
		public SectionPropertiesMarginRightModifier(DocumentModel model) : base(model) { }
		public override void ModifySectionCore(Section section, SectionIndex index, int newValue) {
			section.Margins.BeginUpdate();
			try {
				section.Margins.Right = UnitConverter.TwipsToModelUnits(newValue);
			}
			finally {
				section.Margins.EndUpdate();
			}
		}
		protected override SectionPropertyModifier<int> CreateModifier(int newValue) { return null; }
	}
	public class SectionPropertiesMarginTopModifier : SectionPropertiesModifier<int> {
		public SectionPropertiesMarginTopModifier(DocumentModel model) : base(model) { }
		public override void ModifySectionCore(Section section, SectionIndex index, int newValue) {
			section.Margins.BeginUpdate();
			try {
				section.Margins.Top = UnitConverter.TwipsToModelUnits(newValue);
			}
			finally {
				section.Margins.EndUpdate();
			}
		}
		protected override SectionPropertyModifier<int> CreateModifier(int newValue) { return null; }
	}
	#endregion
	#region PageSize Modifiers
	public class SectionPropertiesPageHeightModifier : SectionPropertiesModifier<int> {
		public SectionPropertiesPageHeightModifier(DocumentModel model) : base(model) { }
		public override void ModifySectionCore(Section section, SectionIndex index, int newValue) {
			section.Page.BeginUpdate();
			try {
				section.Page.Height = UnitConverter.TwipsToModelUnits(newValue);
			}
			finally {
				section.Page.EndUpdate();
			}
		}
		protected override SectionPropertyModifier<int> CreateModifier(int newValue) { return null; }
	}
	public class SectionPropertiesPageWidthModifier : SectionPropertiesModifier<int> {
		public SectionPropertiesPageWidthModifier(DocumentModel model) : base(model) { }
		public override void ModifySectionCore(Section section, SectionIndex index, int newValue) {
			section.Page.BeginUpdate();
			try {
				section.Page.Width = UnitConverter.TwipsToModelUnits(newValue);
			}
			finally {
				section.Page.EndUpdate();
			}
		}
		protected override SectionPropertyModifier<int> CreateModifier(int newValue) { return null; }
	}
	#endregion
	#region Columns Modifiers
	public class SectionPropertiesColumnCountModifier : SectionPropertiesModifier<int> {
		public SectionPropertiesColumnCountModifier(DocumentModel model) : base(model) { }
		public override void ModifySectionCore(Section section, SectionIndex index, int newValue) {
			section.Columns.BeginUpdate();
			try {
				section.Columns.ColumnCount = newValue;
			}
			finally {
				section.Columns.EndUpdate();
			}
		}
		protected override SectionPropertyModifier<int> CreateModifier(int newValue) { return null; }
	}
	public class SectionPropertiesSpaceModifier : SectionPropertiesModifier<int> {
		public SectionPropertiesSpaceModifier(DocumentModel model) : base(model) { }
		public override void ModifySectionCore(Section section, SectionIndex index, int newValue) {
			section.Columns.BeginUpdate();
			try {
				section.Columns.Space = UnitConverter.TwipsToModelUnits(newValue);
			}
			finally {
				section.Columns.EndUpdate();
			}
		}
		protected override SectionPropertyModifier<int> CreateModifier(int newValue) { return null; }
	}
	public class SectionPropertiesEqualWidthColumnsModifier : SectionPropertiesModifier<bool> {
		public SectionPropertiesEqualWidthColumnsModifier(DocumentModel model) : base(model) { }
		public override void ModifySectionCore(Section section, SectionIndex index, bool newValue) {
			section.Columns.BeginUpdate();
			try {
				section.Columns.EqualWidthColumns = newValue;
			}
			finally {
				section.Columns.EndUpdate();
			}
		}
		protected override SectionPropertyModifier<bool> CreateModifier(bool newValue) { return null; }
	}
	public class SectionPropertiesColumnsInfoModifier : SectionPropertiesModifier<ColumnInfoCollection> {
		public SectionPropertiesColumnsInfoModifier(DocumentModel model) : base(model) { }
		public override void ModifySectionCore(Section section, SectionIndex index, ColumnInfoCollection newValue) {
			section.Columns.BeginUpdate();
			try {
				section.Columns.SetColumns(newValue);
			}
			finally {
				section.Columns.EndUpdate();
			}
		}
		protected override ColumnInfoCollection GetNewValue(object obj) {
			var collection = new ColumnInfoCollection();
			ArrayList clientCollection = (ArrayList)obj;
			foreach(var item in clientCollection) {
				var hashTable = (Hashtable)item;
				var column = new ColumnInfo();
				column.Width = UnitConverter.TwipsToModelUnits((int)hashTable["width"]);
				column.Space = UnitConverter.TwipsToModelUnits((int)hashTable["space"]);
				collection.Add(column);
			}
			return collection;
		}
		protected override SectionPropertyModifier<ColumnInfoCollection> CreateModifier(ColumnInfoCollection newValue) { return null; }
	}
	#endregion
}
