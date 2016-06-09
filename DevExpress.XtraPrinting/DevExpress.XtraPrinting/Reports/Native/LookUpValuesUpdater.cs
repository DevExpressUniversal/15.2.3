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
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.XtraPrinting.Reports.Native {
	abstract class LookUpEditUpdaterBase {
		public static LookUpEditUpdaterBase CreateInstance(BaseEdit editor) {
			if(editor != null) {
				if(editor.Properties is RepositoryItemLookUpEdit)
					return new LookUpEditUpdater(editor, (RepositoryItemLookUpEdit)editor.Properties);
				if(editor.Properties is RepositoryItemCheckedComboBoxEdit)
					return new CheckedComboBoxEditUpdater(editor, (RepositoryItemCheckedComboBoxEdit)editor.Properties);
				System.Diagnostics.Debug.Assert(false, "Missing look-up updater.");
			}
			return null;
		}
		protected BaseEdit editor;
		public LookUpEditUpdaterBase(BaseEdit editor) {
			if(editor == null) throw new ArgumentNullException("editor");
			this.editor = editor;
		}
		public bool TryUpdateEditor(IList<LookUpValue> values) {
			return TryUpdateEditor(() => values, false);
		}
		public bool TryUpdateEditor(Func<IList<LookUpValue>> getValues, bool updateIfNotSetOnly) {
			IList<LookUpValue> oldValues = GetOldValues();
			if(updateIfNotSetOnly && oldValues != null)
				return false;
			IList<LookUpValue> values = getValues();
			if(AreEqual(oldValues, values))
				return false;
			UpdateEditor(values);
			return true;
		}
		protected abstract IList<LookUpValue> GetOldValues();
		protected abstract void UpdateEditor(IList<LookUpValue> values);
		protected static bool AreEqual(IList<LookUpValue> oldValues, IList<LookUpValue> newValues) {
			if(oldValues != null && newValues != null)
				return oldValues.SequenceEqual(newValues);
			return oldValues == newValues;
		}
	}
	class CheckedComboBoxEditUpdater : LookUpEditUpdaterBase {
		RepositoryItemCheckedComboBoxEdit repositoryItem;
		public CheckedComboBoxEditUpdater(BaseEdit editor, RepositoryItemCheckedComboBoxEdit repositoryItem)
			: base(editor) {
				this.repositoryItem = repositoryItem;
		}
		protected override IList<LookUpValue> GetOldValues() {
			return repositoryItem.DataSource as IList<LookUpValue>;
		}
		protected override void UpdateEditor(IList<LookUpValue> values) {
			repositoryItem.DataSource = null;
			repositoryItem.ClearDataAdapter();
			repositoryItem.DataSource = values;
			SetEditorValue(values);
		}
		void SetEditorValue(IList<LookUpValue> lookUps) {
			IEnumerable oldValue = editor.EditValue as IEnumerable;
			if(oldValue != null) {
				ArrayList values = new ArrayList(); 
				foreach(object item in oldValue) {
					if(lookUps.Any<LookUpValue>(lpValue => Equals(lpValue.Value, item)))
						values.Add(item);
				}
				if(values.Count > 0) {
					editor.EditValue = values.ToArray(values[0].GetType());
					return;
				}
			}
			if(lookUps.Count != 0)
				editor.EditValue = lookUps[0].Value;
			else
				editor.EditValue = null;
		}
	}
	class LookUpEditUpdater : LookUpEditUpdaterBase {
		RepositoryItemLookUpEdit repositoryItem;
		public LookUpEditUpdater(BaseEdit editor, RepositoryItemLookUpEdit repositoryItem)
			: base(editor) {
				this.repositoryItem = repositoryItem;
		}
		protected override IList<LookUpValue> GetOldValues() {
			return repositoryItem.DataSource as IList<LookUpValue>;
		}
		protected override void UpdateEditor(IList<LookUpValue> values) {
			repositoryItem.DataSource = values;
			SetEditorValue(values);
		}
		void SetEditorValue(IList<LookUpValue> lookUps) {
			var oldValue = editor.EditValue;
			if(lookUps.Any(x => x.Value == oldValue))
				editor.EditValue = oldValue;
			else if(lookUps.Count != 0)
				editor.EditValue = lookUps[0].Value;
			else
				editor.EditValue = null;
		}
	}
}
