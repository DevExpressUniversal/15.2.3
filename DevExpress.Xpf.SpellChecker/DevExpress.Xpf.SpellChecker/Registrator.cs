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
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
using System.Reflection;
namespace DevExpress.XtraSpellChecker.Native {
	public class RegistrationManagerBase {
		Dictionary<Type, Type> registeredTypes = new Dictionary<Type, Type>();
		public RegistrationManagerBase() {
			RegisterDefaults(registeredTypes);
		}
		protected virtual void RegisterDefaults(Dictionary<Type, Type> collection) { }
		protected Dictionary<Type, Type> RegisteredTypes { get { return registeredTypes; } }
		public virtual void RegisterClass(Type controlType, Type customControlHelperType) {
			if(IsClassRegistered(controlType))
				return;
			RegisteredTypes[controlType] = customControlHelperType;
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
		protected override void RegisterDefaults(Dictionary<Type, Type> collection) {
			collection.Clear();
			collection.Add(typeof(System.Windows.Controls.TextBox), typeof(SimpleTextBoxTextController));
			collection.Add(typeof(DevExpress.Xpf.Editors.TextEdit), typeof(SimpleTextEditTextController));
			collection.Add(typeof(DevExpress.Xpf.Editors.MemoEdit), typeof(SimpleMemoEditTextController));
			collection.Add(typeof(System.Windows.Controls.RichTextBox), typeof(RichTextBoxController));
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
			RegisteredTypes[editControl.GetType()] = textControlControllerType;
		}
		public virtual void RegisterTextControlController(Type editControlType, Type textControlControllerType) {
			RegisteredTypes[editControlType] = textControlControllerType;
		}
	}
}
