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

using DevExpress.Web.ASPxSpellChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace DevExpress.Web.Mvc {
	public class MVCxSpellCheckerDictionaryCollection: SpellCheckerDictionaryCollection {
		public MVCxSpellCheckerDictionaryCollection()
			: base(null) {
		}
		public ASPxSpellCheckerDictionary Add() {
			return Add(null);
		}
		public ASPxSpellCheckerDictionary Add(Action<ASPxSpellCheckerDictionary> method) {
			return AddDictionary<ASPxSpellCheckerDictionary>(method);
		}
		public ASPxSpellCheckerISpellDictionary AddISpellDictionary() {
			return AddISpellDictionary(null);
		}
		public ASPxSpellCheckerISpellDictionary AddISpellDictionary(Action<ASPxSpellCheckerISpellDictionary> method) {
			return AddDictionary<ASPxSpellCheckerISpellDictionary>(method);
		}
		public ASPxSpellCheckerOpenOfficeDictionary AddOpenOfficeDictionary() {
			return AddOpenOfficeDictionary(null);
		}
		public ASPxSpellCheckerOpenOfficeDictionary AddOpenOfficeDictionary(Action<ASPxSpellCheckerOpenOfficeDictionary> method) {
			return AddDictionary<ASPxSpellCheckerOpenOfficeDictionary>(method);
		}
		public ASPxHunspellDictionary AddHunspellDictionary() {
			return AddHunspellDictionary(null);
		}
		public ASPxHunspellDictionary AddHunspellDictionary(Action<ASPxHunspellDictionary> method) {
			return AddDictionary<ASPxHunspellDictionary>(method);
		}
		public ASPxSpellCheckerCustomDictionary AddCustomDictionary() {
			return AddCustomDictionary(null);
		}
		public ASPxSpellCheckerCustomDictionary AddCustomDictionary(Action<ASPxSpellCheckerCustomDictionary> method) {
			return AddDictionary<ASPxSpellCheckerCustomDictionary>(method);
		}
		protected T AddDictionary<T>(Action<T> method) where T: WebDictionary, new() {
			var dictionary = new T();
			if(method != null)
				method(dictionary);
			Add(dictionary);
			return dictionary;
		}
	}
	public class MVCxSpellCheckerFormsSettings : SpellCheckerFormsSettings {
		public MVCxSpellCheckerFormsSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string SpellCheckFormPath {
			get { return base.SpellCheckFormPath; }
			set { base.SpellCheckFormPath = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string SpellCheckOptionsFormPath {
			get { return base.SpellCheckOptionsFormPath; }
			set { base.SpellCheckOptionsFormPath = value; }
		}
		public string SpellCheckFormAction {
			get { return base.SpellCheckFormPath; }
			set { base.SpellCheckFormPath = value; }
		}
		public string SpellCheckOptionsFormAction {
			get { return base.SpellCheckOptionsFormPath; }
			set { base.SpellCheckOptionsFormPath = value; }
		}
		protected internal string GetFormAction(string formName) {
			return GetFormPath(formName);
		}
	}
}
