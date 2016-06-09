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
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpellChecker  {
	public class SpellCheckerUserControl : UserControl, IDialogFormElementRequiresLoad {
		private ASPxSpellChecker spellChecker = null;
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
		protected ASPxSpellChecker SpellChecker {
			get {
				if(spellChecker == null)
					spellChecker = FindParentSpellChecker();
				return spellChecker;
			}
		}
		protected override void OnInit(EventArgs e) {
			ClientIDHelper.UpdateClientIDMode(this);
			base.OnInit(e);
		}
		protected override void OnLoad(System.EventArgs e) {
			base.OnLoad(e);			
			PrepareChildControls();
		}
		protected virtual ASPxButton[] GetChildDxButtons() {
			return new ASPxButton[] { };
		}
		protected virtual ASPxEditBase[] GetChildDxEdits() {
			return new ASPxEditBase[] { };
		}
		private ASPxSpellChecker FindParentSpellChecker() {
			Control control = Parent;
			while(control != null) {
				ASPxSpellChecker spellChecker = control as ASPxSpellChecker;
				if (spellChecker != null)
					return spellChecker;
				control = control.Parent;
			}
			return null;
		}
		protected virtual void PrepareChildControls() {
			ASPxEditBase[] dxEdits = GetChildDxEdits();
			foreach(ASPxEditBase edit in dxEdits) {
				edit.ParentStyles = SpellChecker.GetEditorsStyles();
				edit.ParentImages = SpellChecker.GetEditorsImages();
			}
			ASPxButton[] dxButtons = GetChildDxButtons();
			foreach(ASPxButton btn in dxButtons)
				btn.ParentStyles = SpellChecker.GetButtonStyles();
		}
		void IDialogFormElementRequiresLoad.ForceInit() {
			FrameworkInitialize();
		}
		void IDialogFormElementRequiresLoad.ForceLoad() {
			OnLoad(EventArgs.Empty);
		}
	}
}
