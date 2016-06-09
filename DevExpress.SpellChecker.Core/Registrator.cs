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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraSpellChecker.Parser;
using System.Reflection;
namespace DevExpress.XtraSpellChecker.Native {
	public class UndoControllerRepository {
		Dictionary<Type, Type> registeredTypes = new Dictionary<Type, Type>();
		static UndoControllerRepository fDefault;
		static UndoControllerRepository() {
			SetDefaultRepository(new UndoControllerRepository());
		}
		public static UndoControllerRepository Default { get { return fDefault; } }
		public virtual void Register(Type controlType, Type undoControllerType) {
			if (!IsControllerRegistered(controlType))
				this.registeredTypes.Add(controlType, undoControllerType);
		}
		public virtual void Unregister(Type controlType) {
			if (IsControllerRegistered(controlType))
				this.registeredTypes.Remove(controlType);
		}
		protected virtual IUndoController CreateUndoController(object editControl) {
			if (editControl != null) {
				Type controlType = editControl.GetType();
				if (!IsControllerRegistered(controlType) && !TryRegisterRichEditUndoController(controlType))
					return null;
				Type undoControllerType = registeredTypes[controlType];
				return CreateUndoController(editControl, undoControllerType);
			}
			return null;
		}
		bool TryRegisterRichEditUndoController(Type controlType) {
			if (controlType.FullName == RichEditControlHelper.XtraRichEditTypeName || controlType.FullName == RichEditControlHelper.XpfRichEditTypeName || controlType.FullName == RichEditControlHelper.SnapTypeName) {
				Register(controlType, RichEditControlHelper.GetUndoControllerType());
				return true;
			}
			return false;
		}
		protected virtual bool IsControllerRegistered(Type controlType) {
			return this.registeredTypes.ContainsKey(controlType);
		}
		public static IUndoController GetUndoController(object editControl) {
			return Default.CreateUndoController(editControl);
		}
		static IUndoController CreateUndoController(object editControl, Type undoControllerType) {
			return undoControllerType.GetConstructor(new Type[] { editControl.GetType() }).Invoke(new object[] { editControl }) as IUndoController;
		}
		public static bool IsClassRegistered(Type type) {
			return Default.IsControllerRegistered(type);
		}
		internal static void SetDefaultRepository(UndoControllerRepository repository) {
			fDefault = repository;
		}
	}
	internal static class RichEditControlHelper {
		public const string XtraRichEditAssemblyName = AssemblyInfo.SRAssemblyRichEdit + ", Version=" + AssemblyInfo.Version;
		public const string XpfRichEditAssemblyName = AssemblyInfo.SRAssemblyXpfRichEdit + ", Version=" + AssemblyInfo.Version;
		public const string SnapAssemblyName = AssemblyInfo.SRAssemblySnap + ", Version=" + AssemblyInfo.Version;
		public const string RichEditCoreAssemblyName = "DevExpress.RichEdit" + AssemblyInfo.VSuffix + ".Core" + ", Version=" + AssemblyInfo.Version;
		public const string XtraRichEditTypeName = "DevExpress.XtraRichEdit.RichEditControl";
		public const string XpfRichEditTypeName = "DevExpress.Xpf.RichEdit.RichEditControl";
		public const string SnapTypeName = "DevExpress.Snap.SnapControl";
		public static Type GetXtraRichEditControlType() {
			return GetType(XtraRichEditAssemblyName, XtraRichEditTypeName);
		}
		public static Type GetDXRichEditControlType() {
			return GetType(XpfRichEditAssemblyName, XpfRichEditTypeName);
		}
		public static Type GetSnapControlType() {
			return GetType(SnapAssemblyName, SnapTypeName);
		}
		public static Type GetRichEditControllerType() {
			return GetType(RichEditCoreAssemblyName, "DevExpress.XtraRichEdit.SpellChecker.RichEditSpellCheckController");
		}
		public static Type GetUndoControllerType() {
			return GetType(RichEditCoreAssemblyName, "DevExpress.XtraRichEdit.SpellChecker.RichEditUndoController");
		}
		static Type GetType(string assemblyName, string typeName) {
			try {
				Assembly assembly = DevExpress.Data.Utils.Helpers.LoadWithPartialName(assemblyName);
				if (assembly != null)
					return assembly.GetType(typeName);
				return null;
			}
			catch {
				return null;
			}
		}
	}
}
