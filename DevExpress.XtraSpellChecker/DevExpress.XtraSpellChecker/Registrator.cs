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
using System.Windows.Forms;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.XtraEditors;
using System.Reflection;
namespace DevExpress.XtraSpellChecker.Native {
	public class RegistrationManagerBase {
		Hashtable registeredTypes = new Hashtable();
		public RegistrationManagerBase() {
			RegisterDefaults(registeredTypes);
		}
		protected virtual void RegisterDefaults(Hashtable collection) { }
		protected Hashtable RegisteredTypes { get { return registeredTypes; } }
		public virtual void RegisterClass(Type controlType, Type customControlHelperType) {
			if(IsClassRegistered(controlType))
				return;
			RegisteredTypes.Add(controlType, customControlHelperType);
		}
		public virtual void UnregisterClass(Type controlType) {
			if(!IsClassRegistered(controlType))
				return;
			RegisteredTypes.Remove(controlType);
		}
		public virtual bool IsClassRegistered(Type controlType) {
			return RegisteredTypes.ContainsKey(controlType);
		}
	}
	public class SpellCheckTextControllersManager : RegistrationManagerBase {
		static SpellCheckTextControllersManager fDefault;
		static SpellCheckTextControllersManager() {
			fDefault = new SpellCheckTextControllersManager();
		}
		public static SpellCheckTextControllersManager Default { get { return fDefault; } }
		public SpellCheckTextControllersManager() : base() { }
		protected override void RegisterDefaults(Hashtable collection) {
			collection.Clear();
			collection.Add(typeof(System.Windows.Forms.TextBox), typeof(SimpleTextBoxTextController));
			collection.Add(typeof(DevExpress.XtraEditors.TextEdit), typeof(SimpleTextEditTextController));
			collection.Add(typeof(DevExpress.XtraEditors.MemoEdit), typeof(SimpleTextEditTextController));
			collection.Add(typeof(RichTextBox), typeof(RichTextBoxTextController));
			collection.Add(typeof(DevExpress.XtraSpellChecker.Controls.CustomSpellCheckMemoEdit), typeof(SimpleTextEditMSWordTextController));
			RegisterXtraRichEdit(collection);
		}
		void RegisterXtraRichEdit(Hashtable collection) {
			Type richEditType = RichEditControlHelper.GetXtraRichEditControlType();
			Type spellCheckControllerType = RichEditControlHelper.GetRichEditControllerType();
			if (richEditType != null && spellCheckControllerType != null)
				collection.Add(richEditType, spellCheckControllerType);
			Type snapType = RichEditControlHelper.GetSnapControlType();
			if (snapType != null && spellCheckControllerType != null)
				collection.Add(snapType, spellCheckControllerType);
		}
		public virtual ISpellCheckTextControlController GetSpellCheckTextControlController(Control editControl) {
			if(editControl == null)
				return null;
			Type editControlType = editControl.GetType();
			if(IsClassRegistered(editControlType)) {
				Type controlHelperType = RegisteredTypes[editControlType] as Type;
					return CreateSpellCheckTextControlController(controlHelperType, editControl);
			}
			return null;
		}
		public virtual ISpellCheckTextControlController GetSpellCheckTextControlController(Control editControl, ISpellCheckTextControlController originalController) {
			if(editControl == null)
				return null;
			Type editControlType = editControl.GetType();
			if(IsClassRegistered(editControlType)) {
				Type controlHelperType = RegisteredTypes[editControlType] as Type;
				return CreateSpellCheckTextControlController(controlHelperType, editControl, originalController);
			}
			return null;
		}
		protected virtual ISpellCheckTextControlController CreateSpellCheckTextControlController(Type type, Control editControl, ISpellCheckTextControlController originalController) {
			return type.GetConstructor(new System.Type[] { editControl.GetType(), typeof(ISpellCheckTextControlController) }).Invoke(new object[] { editControl, originalController }) as ISpellCheckTextControlController;
		}
		protected virtual ISpellCheckTextControlController CreateSpellCheckTextControlController(Type type, Control editControl) {
			return type.GetConstructor(new System.Type[] { editControl.GetType() }).Invoke(new object[] { editControl }) as ISpellCheckTextControlController;
		}
		public virtual void RegisterTextControlController(Control editControl, Type textControlControllerType) {
			RegisteredTypes.Add(editControl.GetType(), textControlControllerType);
		}
		public virtual void RegisterTextControlController(Type editControlType, Type textControlControllerType) {
			RegisteredTypes.Add(editControlType, textControlControllerType);
		}
	}
	public class SpellCheckTextBoxBaseFinderManager : RegistrationManagerBase {
		static SpellCheckTextBoxBaseFinderManager fDefault;
		static SpellCheckTextBoxBaseFinderManager() {
			fDefault = new SpellCheckTextBoxBaseFinderManager();
		}
		public static SpellCheckTextBoxBaseFinderManager Default { get { return fDefault; } }
		public SpellCheckTextBoxBaseFinderManager() : base() { }
		protected override void RegisterDefaults(Hashtable collection) {
			collection.Clear();
			collection.Add(typeof(System.Windows.Forms.TextBox), typeof(TextBoxFinder));
			collection.Add(typeof(DevExpress.XtraEditors.TextEdit), typeof(TextEditTextBoxFinder));
			collection.Add(typeof(DevExpress.XtraEditors.MemoEdit), typeof(MemoEditTextBoxFinder));
			collection.Add(typeof(RichTextBox), typeof(RTFTextBoxFinder));
		}
		public virtual TextBoxBase GetTextBoxInstance(Control editControl) {
			if(editControl == null)
				return null;
			Type type = editControl.GetType();
			if(IsClassRegistered(type)) {
				Type finderType = RegisteredTypes[type] as Type;
				TextBoxBaseFinderBase finder = finderType.GetConstructor(new System.Type[] { }).Invoke(new object[] { }) as TextBoxBaseFinderBase;
				return finder.GetTextBoxInstance(editControl);
			}
			return null;
		}
		public virtual void RegisterTextBoxFinderType(Type editControlType, Type textBoxBaseFinderType) {
			if(typeof(TextBoxBaseFinderBase).IsAssignableFrom(textBoxBaseFinderType)) {
				RegisteredTypes.Add(editControlType, textBoxBaseFinderType);
			}
		}
	}
	public abstract class TextBoxBaseFinderBase {
		public abstract TextBoxBase GetTextBoxInstance(Control editControl);
	}
	public class TextBoxFinder : TextBoxBaseFinderBase {
		public override TextBoxBase GetTextBoxInstance(Control editControl) {
			return editControl as TextBoxBase;
		}
	}
	public class TextEditTextBoxFinder : TextBoxBaseFinderBase {
		public override TextBoxBase GetTextBoxInstance(Control editControl) {
			TextEdit textEdit = editControl as TextEdit;
			if(textEdit != null)
				return GetTextBoxInstanceFromTextEdit(textEdit);
			return null;
		}
		protected virtual TextBoxBase GetTextBoxInstanceFromTextEdit(TextEdit textEdit) {
			int count = textEdit.Controls.Count;
			for (int i = 0; i < count; i++) {
				TextBoxBase textBox = textEdit.Controls[i] as TextBoxBase;
				if (textBox != null)
					return textBox;
			}
			return null;
		}
	}
	public class MemoEditTextBoxFinder : TextBoxBaseFinderBase {
		protected virtual TextBoxBase GetTextBoxInstanceFromMemoEdit(TextEdit textEdit) {
			Control.ControlCollection controls = textEdit.Controls;
			int count = controls.Count;
			for (int i = 0; i < count; i++) {
				TextBoxBase textBoxBase = controls[i] as TextBoxBase;
				if (textBoxBase != null)
					return textBoxBase;
			}
			return null;
		}
		public override TextBoxBase GetTextBoxInstance(Control editControl) {
			MemoEdit memoEdit = editControl as MemoEdit;
			if (memoEdit != null)
				return GetTextBoxInstanceFromMemoEdit(memoEdit);
			return null;
		}
	}
	public class RTFTextBoxFinder : TextBoxBaseFinderBase {
		public override TextBoxBase GetTextBoxInstance(Control editControl) {
			return editControl as RichTextBox;
		}
	}
}
