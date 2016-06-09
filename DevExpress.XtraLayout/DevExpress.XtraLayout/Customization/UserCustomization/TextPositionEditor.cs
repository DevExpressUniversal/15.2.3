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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.XtraLayout.Customization
{
	class TextPositionEditor : PopupContainerEdit, INotifyPropertyChanged{
		TextPositionEditorPopup popupContent;
		PopupContainerControl popupControl;
		public TextPositionEditor(){
				popupControl = new PopupContainerControl();
				popupContent = new TextPositionEditorPopup(this);
				Properties.PopupControl = popupControl;
				popupControl.Controls.Add(popupContent);
		}
		Locations alignCore;
		public Locations Align { get { return alignCore; } 
			set { 
			alignCore = value;
			if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Align"));
			Text = alignCore.ToString();
			Invalidate(); } }
		public event PropertyChangedEventHandler PropertyChanged;
	}
	class TextPositionEditorPopup: Control {
		TextPositionEditor ownerCore;
		private RadioButton LeftButton = new RadioButton();
		private RadioButton TopButton = new RadioButton();
		private RadioButton RightButton = new RadioButton();
		private RadioButton BottomButton = new RadioButton();
		private RadioButton DefaultButton = new RadioButton();
		public TextPositionEditorPopup(TextPositionEditor owner){
			ownerCore = owner;
			ownerCore.PropertyChanged += OnOwnerAlignChanged;
			this.InitComponent();
		}
		protected override void Dispose(bool disposing) {
			if(!disposing) {
				if(ownerCore != null) ownerCore.PropertyChanged -= OnOwnerAlignChanged;
			}
			base.Dispose(disposing);
		}
		void OnOwnerAlignChanged(object sender, PropertyChangedEventArgs e) {
			UpdateCheckedState();
		}
		protected void UpdateCheckedState() { 
			Locations align = ownerCore.Align;
			LeftButton.Checked = align == Locations.Left;
			TopButton.Checked = align == Locations.Top;
			RightButton.Checked = align == Locations.Right;
			BottomButton.Checked = align == Locations.Bottom;
			DefaultButton.Checked = align == Locations.Default;
		}
		private void OptionClick(object sender, EventArgs e) {
			if(sender == LeftButton) ownerCore.Align = Locations.Left;
			if(sender == RightButton) ownerCore.Align = Locations.Right;
			if(sender == TopButton) ownerCore.Align = Locations.Top;
			if(sender == BottomButton) ownerCore.Align = Locations.Bottom;
			if(sender == DefaultButton) ownerCore.Align = Locations.Default;
		}
		private void InitComponent() {
			base.Size = new Size(125, 89);
			this.BackColor = SystemColors.Control;
			this.ForeColor = SystemColors.ControlText;
			this.TopButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
			this.TopButton.Location = new Point(32, 0);
			this.TopButton.Size = new Size(24, 25);
			this.TopButton.TabIndex = 0;
			this.TopButton.Text = "";
			this.TopButton.Appearance = Appearance.Button;
			this.TopButton.Click += new EventHandler(this.OptionClick);
			this.LeftButton.Location = new Point(0, 32);
			this.LeftButton.Size = new Size(24, 25);
			this.LeftButton.TabIndex = 4;
			this.LeftButton.Text = "";
			this.LeftButton.Appearance = Appearance.Button;
			this.LeftButton.Click += new EventHandler(this.OptionClick);
			this.DefaultButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
			this.DefaultButton.Location = new Point(32, 32);
			this.DefaultButton.Size = new Size(24, 25);
			this.DefaultButton.TabIndex = 5;
			this.DefaultButton.Text = "";
			this.DefaultButton.Appearance = Appearance.Button;
			this.DefaultButton.Click += new EventHandler(this.OptionClick);
			this.RightButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
			this.RightButton.Location = new Point(64, 32);
			this.RightButton.Size = new Size(24, 25);
			this.RightButton.TabIndex = 1;
			this.RightButton.Text = "";
			this.RightButton.Appearance = Appearance.Button;
			this.RightButton.Click += new EventHandler(this.OptionClick);
			this.BottomButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
			this.BottomButton.Location = new Point(32, 64);
			this.BottomButton.Size = new Size(24, 25);
			this.BottomButton.TabIndex = 3;
			this.BottomButton.Text = "";
			this.BottomButton.Appearance = Appearance.Button;
			this.BottomButton.Click += new EventHandler(this.OptionClick);
			base.Controls.Clear();
			base.Controls.AddRange(new Control[]
				{
					this.TopButton,
					this.BottomButton,
					this.LeftButton,
					this.RightButton,
					this.DefaultButton,
				});
		}
	}
}
