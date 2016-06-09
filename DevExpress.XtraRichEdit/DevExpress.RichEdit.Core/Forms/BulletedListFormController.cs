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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Forms {
	#region BulletedListFormController
	public class BulletedListFormController : FormController {
		#region Fields
		ListLevelCollection<ListLevel> sourceLevels;
		ListLevelCollection<ListLevel> editedLevels;
		int editedLevelIndex;
		#endregion
		public BulletedListFormController(ListLevelCollection<ListLevel> sourceLevels) {
			Guard.ArgumentNotNull(sourceLevels, "sourceLevels");
			this.sourceLevels = sourceLevels;
			InitializeController();
		}
		#region Properties
		public ListLevelCollection<ListLevel> EditedLevels { get { return editedLevels; } }
		public IListLevel EditedLevel { get { return editedLevels[editedLevelIndex]; } }
		public int EditedLevelIndex { get { return editedLevelIndex; } set { editedLevelIndex = value; } }
		public string DisplayFormat { get { return EditedLevel.ListLevelProperties.DisplayFormatString; } set { EditedLevel.ListLevelProperties.DisplayFormatString = value; } }
		public int FirstLineIndent { get { return EditedLevel.FirstLineIndent; } set { EditedLevel.FirstLineIndent = value; } }
		public int LeftIndent { get { return EditedLevel.LeftIndent; } set { EditedLevel.LeftIndent = value; } }
		public CharacterProperties CharacterProperties { get { return EditedLevel.CharacterProperties; } }
		public ParagraphFirstLineIndent FirstLineIndentType { get { return EditedLevel.FirstLineIndentType; } set { EditedLevel.FirstLineIndentType = value; } }
		#endregion
		protected internal virtual void InitializeController() {
			this.editedLevels = CreateEditedLevels();
		}
		protected virtual ListLevelCollection<ListLevel> CreateEditedLevels() {
			ListLevelCollection<ListLevel> result = new ListLevelCollection<ListLevel>();
			int count = sourceLevels.Count;
			for (int i = 0; i < count; i++) {
				ListLevel level = new ListLevel(sourceLevels[i].DocumentModel);
				level.CopyFrom(sourceLevels[i]);
				result.Add(level);
			}
			return result;
		}
		public override void ApplyChanges() {
			int count = editedLevels.Count;
			for (int i = 0; i < count; i++) {
				((ListLevel)sourceLevels[i]).CopyFrom(editedLevels[i]);
			}
		}
	}
	#endregion
}
