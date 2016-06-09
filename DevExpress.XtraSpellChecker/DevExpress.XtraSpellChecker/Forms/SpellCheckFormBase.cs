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
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Strategies;
namespace DevExpress.XtraSpellChecker.Forms {
	public class BaseSpellCheckForm : DevExpress.XtraEditors.XtraForm {
		private System.ComponentModel.Container components = null;
		SpellChecker spellChecker = null;
		public BaseSpellCheckForm() {
			InitializeComponent();
		}
		public BaseSpellCheckForm(SpellChecker spellChecker) {
			this.spellChecker = spellChecker;
			InitializeComponent();
		}
		public virtual void Init() { }
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(!DesignMode && SpellChecker != null)
				SetFormSize();
		}
		protected virtual void OnStrategyChanged() { }
		public SpellChecker SpellChecker {
			get {
				return spellChecker;
			}
		}
		public SpellingFormsManager FormsManager {
			get {
				if(SpellChecker != null)
					return SpellChecker.FormsManager;
				return null;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseSpellCheckForm));
			this.SuspendLayout();
			resources.ApplyResources(this, "$this");
			this.Name = "BaseSpellCheckForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.VisibleChanged += new System.EventHandler(this.BaseSpellCheckForm_VisibleChanged);
			this.ResumeLayout(false);
		}
		#endregion
		protected virtual void AssignSize(FormSerializationInfo info) {
			this.Left = info.Left;
			this.Top = info.Top;
			this.Width = info.Width;
			this.Height = info.Height;
		}
		protected virtual void SetFormSize() {
			if(FormsManager != null) {
				FormSerializationInfo info = FormsManager.GetFormSerializationInfo(this);
				if(info != null)
					AssignSize(info);
			}
		}
		protected virtual bool CanSaveFormInformation() {
			return !DesignMode && FormsManager != null;
		}
		protected virtual bool ShouldSaveFormInformation() {
			return false;
		}
		private void BaseSpellCheckForm_VisibleChanged(object sender, EventArgs e) {
			if(!Visible && ShouldSaveFormInformation() && CanSaveFormInformation())
				FormsManager.AddForm(this);
		}
	}
}
