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

using DevExpress.Office.NumberConverters;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class ChangeListLevelPropertiesCommand : WebRichEditPropertyStateBasedCommand<ListLevelCommandState, JSONListLevelProperty> {
		public ChangeListLevelPropertiesCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeListLevelProperties; } }
		protected override bool IsEnabled() { return true; }
		static Dictionary<JSONListLevelProperty, JSONModelModifier<ListLevelCommandState>> modifiers = new Dictionary<JSONListLevelProperty, JSONModelModifier<ListLevelCommandState>>() {
			{JSONListLevelProperty.Alignment, model => new ListLevelPropertiesAlignmentModifier(model)},
			{JSONListLevelProperty.ConvertPreviousLevelNumberingToDecimal, model => new ListLevelPropertiesConvertPreviousLevelNumberingToDecimalModifier(model)},
			{JSONListLevelProperty.DisplayFormatString, model => new ListLevelPropertiesDisplayFormatStringModifier(model)},
			{JSONListLevelProperty.Format, model => new ListLevelPropertiesFormatModifier(model)},
			{JSONListLevelProperty.Legacy, model => new ListLevelPropertiesLegacyModifier(model)},
			{JSONListLevelProperty.LegacyIndent, model => new ListLevelPropertiesLegacyIndentModifier(model)},
			{JSONListLevelProperty.LegacySpace, model => new ListLevelPropertiesLegacySpaceModifier(model)},
			{JSONListLevelProperty.OriginalLeftIndent, model => new ListLevelPropertiesOriginalLeftIndentModifier(model)},
			{JSONListLevelProperty.RelativeRestartLevel, model => new ListLevelPropertiesRelativeRestartLevelModifier(model)},
			{JSONListLevelProperty.Separator, model => new ListLevelPropertiesSeparatorModifier(model)},
			{JSONListLevelProperty.Start, model => new ListLevelPropertiesStartModifier(model)},
			{JSONListLevelProperty.SuppressBulletResize, model => new ListLevelPropertiesSuppressBulletResizeModifier(model)},
			{JSONListLevelProperty.SuppressRestart, model => new ListLevelPropertiesSuppressRestartModifier(model)},
			{JSONListLevelProperty.TemplateCode, model => new ListLevelPropertiesTemplateCodeModifier(model)},
		};
		protected override IModelModifier<ListLevelCommandState> CreateModifier(JSONListLevelProperty property) {
			JSONModelModifier<ListLevelCommandState> creator;
			if(!modifiers.TryGetValue(property, out creator))
				throw new ArgumentException();
			return creator(DocumentModel);
		}
		class ListLevelPropertiesAlignmentModifier : ListLevelPropertiesModifier<ListNumberAlignment> {
			public ListLevelPropertiesAlignmentModifier(DocumentModel documentModel) :base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, ListNumberAlignment value) {
				properties.Alignment = value;
			}
		}
		class ListLevelPropertiesConvertPreviousLevelNumberingToDecimalModifier : ListLevelPropertiesModifier<bool> {
			public ListLevelPropertiesConvertPreviousLevelNumberingToDecimalModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, bool value) {
				properties.ConvertPreviousLevelNumberingToDecimal = value;
			}
		}
		class ListLevelPropertiesDisplayFormatStringModifier : ListLevelPropertiesModifier<string> {
			public ListLevelPropertiesDisplayFormatStringModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, string value) {
				properties.DisplayFormatString = value;
			}
		}
		class ListLevelPropertiesFormatModifier : ListLevelPropertiesModifier<NumberingFormat> {
			public ListLevelPropertiesFormatModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, NumberingFormat value) {
				properties.Format = value;
			}
		}
		class ListLevelPropertiesLegacyModifier : ListLevelPropertiesModifier<bool> {
			public ListLevelPropertiesLegacyModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, bool value) {
				properties.Legacy = value;
			}
		}
		class ListLevelPropertiesLegacyIndentModifier : ListLevelPropertiesModifier<int> {
			public ListLevelPropertiesLegacyIndentModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, int value) {
				properties.LegacyIndent = value;
			}
		}
		class ListLevelPropertiesLegacySpaceModifier : ListLevelPropertiesModifier<int> {
			public ListLevelPropertiesLegacySpaceModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, int value) {
				properties.LegacySpace = value;
			}
		}
		class ListLevelPropertiesOriginalLeftIndentModifier : ListLevelPropertiesModifier<int> {
			public ListLevelPropertiesOriginalLeftIndentModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, int value) {
				properties.OriginalLeftIndent = value;
			}
		}
		class ListLevelPropertiesRelativeRestartLevelModifier : ListLevelPropertiesModifier<int> {
			public ListLevelPropertiesRelativeRestartLevelModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, int value) {
				properties.RelativeRestartLevel = value;
			}
		}
		class ListLevelPropertiesSeparatorModifier : ListLevelPropertiesModifier<char> {
			public ListLevelPropertiesSeparatorModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, char value) {
				properties.Separator = value;
			}
			protected override char GetNewValue(object value) {
				return Convert.ToChar(value);
			}
		}
		class ListLevelPropertiesStartModifier : ListLevelPropertiesModifier<int> {
			public ListLevelPropertiesStartModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, int value) {
				properties.Start = value;
			}
		}
		class ListLevelPropertiesSuppressBulletResizeModifier : ListLevelPropertiesModifier<bool> {
			public ListLevelPropertiesSuppressBulletResizeModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, bool value) {
				properties.SuppressBulletResize = value;
			}
		}
		class ListLevelPropertiesSuppressRestartModifier : ListLevelPropertiesModifier<bool> {
			public ListLevelPropertiesSuppressRestartModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, bool value) {
				properties.SuppressRestart = value;
			}
		}
		class ListLevelPropertiesTemplateCodeModifier : ListLevelPropertiesModifier<int> {
			public ListLevelPropertiesTemplateCodeModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevelProperties(ListLevelProperties properties, int value) {
				properties.TemplateCode = value;
			}
		}
		abstract class ListLevelPropertiesModifier<T> : ListLevelModelModifier<T> {
			protected ListLevelPropertiesModifier(DocumentModel documentModel) : base(documentModel) { }
			protected abstract void ModifyListLevelProperties(ListLevelProperties properties, T value);
			protected override void ModifyCore(bool isAbstract, int listIndex, int listLevelIndex, T value) {
				IListLevel listLevel;
				if(isAbstract)
					listLevel = DocumentModel.AbstractNumberingLists[new AbstractNumberingListIndex(listIndex)].Levels[listLevelIndex];
				else
					listLevel = DocumentModel.NumberingLists[new NumberingListIndex(listIndex)].Levels[listLevelIndex];
				ModifyListLevelProperties(listLevel.ListLevelProperties, value);
			}
		}
	}
}
