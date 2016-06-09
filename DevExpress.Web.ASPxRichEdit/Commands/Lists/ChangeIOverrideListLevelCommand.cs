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

using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class ChangeIOverrideListLevelCommand : WebRichEditPropertyStateBasedCommand<ListLevelCommandState, JSONIOverrideListLevelProperty> {
		public ChangeIOverrideListLevelCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeIOverrideListLevel; } }
		protected override bool IsEnabled() { return true; }
		static Dictionary<JSONIOverrideListLevelProperty, JSONModelModifier<ListLevelCommandState>> modifiers = new Dictionary<JSONIOverrideListLevelProperty, JSONModelModifier<ListLevelCommandState>>() {
			{JSONIOverrideListLevelProperty.NewStart, model => new ListLevelIOverrideNewStartModifier(model)},
			{JSONIOverrideListLevelProperty.OverrideStart, model => new ListLevelIOverrideStartModifier(model)},
		};
		protected override IModelModifier<ListLevelCommandState> CreateModifier(JSONIOverrideListLevelProperty property) {
			JSONModelModifier<ListLevelCommandState> creator;
			if(!modifiers.TryGetValue(property, out creator))
				throw new ArgumentException();
			return creator(DocumentModel);
		}
		abstract class ListLevelIOverrideModifier<T> : ListLevelModelModifier<T> {
			protected ListLevelIOverrideModifier(DocumentModel documentModel) : base(documentModel) { }
			protected abstract void ModifyListLevel(IOverrideListLevel listLevel, T value);
			protected override void ModifyCore(bool isAbstract, int listIndex, int listLevelIndex, T value) {
				IOverrideListLevel listLevel = DocumentModel.NumberingLists[new NumberingListIndex(listIndex)].Levels[listLevelIndex];
				ModifyListLevel(listLevel, value);
			}
		}
		class ListLevelIOverrideNewStartModifier : ListLevelIOverrideModifier<int> {
			public ListLevelIOverrideNewStartModifier(DocumentModel documentModel) :base(documentModel) { }
			protected override void ModifyListLevel(IOverrideListLevel listLevel, int value) {
				listLevel.NewStart = value;
			}
		}
		class ListLevelIOverrideStartModifier : ListLevelIOverrideModifier<bool> {
			public ListLevelIOverrideStartModifier(DocumentModel documentModel) : base(documentModel) { }
			protected override void ModifyListLevel(IOverrideListLevel listLevel, bool value) {
				listLevel.SetOverrideStart(value);
			}
		}
	}
	public enum JSONIOverrideListLevelProperty {
		NewStart = 0,
		OverrideStart = 1
	}
}
