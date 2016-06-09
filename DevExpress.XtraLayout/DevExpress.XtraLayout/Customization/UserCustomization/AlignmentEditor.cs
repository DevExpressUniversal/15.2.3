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
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.XtraLayout.Customization
{
  [Browsable(false), ToolboxItem(false)]
	public class ControlPositionEditor: PopupContainerEdit, INotifyPropertyChanged{
		ControlPositionEditorPopup popupContent;
		PopupContainerControl popupControl;
		public ControlPositionEditor() {
				popupControl = new PopupContainerControl();
				popupContent = new ControlPositionEditorPopup(this);
				Properties.PopupControl = popupControl;
				popupControl.Controls.Add(popupContent);
		}
		ContentAlignment alignCore;
		public ContentAlignment Align {
			get { return alignCore; } 
			set { 
			alignCore = value;
			if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Align"));
			Text = alignCore.ToString();
			Invalidate(); } }
		public event PropertyChangedEventHandler PropertyChanged;
	}
	[Browsable(false), ToolboxItem(false)]
	public class ControlPositionEditorPopup : Control {
		ControlPositionEditor ownerCore;
		private RadioButton topLeft = new RadioButton();
		private RadioButton topCenter = new RadioButton();
		private RadioButton topRight = new RadioButton();
		private RadioButton middleLeft = new RadioButton();
		private RadioButton middleCenter = new RadioButton();
		private RadioButton middleRight = new RadioButton();
		private RadioButton bottomLeft = new RadioButton();
		private RadioButton bottomCenter = new RadioButton();
		private RadioButton bottomRight = new RadioButton();
		public ControlPositionEditorPopup(ControlPositionEditor owner) {
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
			ContentAlignment align = ownerCore.Align;
			topLeft.Checked = align == ContentAlignment.TopLeft;
			topCenter.Checked = align == ContentAlignment.TopCenter;
			topRight.Checked = align == ContentAlignment.TopRight;
			middleLeft.Checked = align == ContentAlignment.MiddleLeft;
			middleCenter.Checked = align == ContentAlignment.MiddleCenter;
			middleRight.Checked = align == ContentAlignment.MiddleRight;
			bottomLeft.Checked = align == ContentAlignment.BottomLeft;
			bottomCenter.Checked = align == ContentAlignment.BottomCenter;
			bottomRight.Checked = align == ContentAlignment.BottomRight;
		}
		private void OptionClick(object sender, EventArgs e) {
			if(sender == topLeft) ownerCore.Align = ContentAlignment.TopLeft;
			if(sender == topCenter) ownerCore.Align = ContentAlignment.TopCenter;
			if(sender == topRight) ownerCore.Align = ContentAlignment.TopRight;
			if(sender == middleLeft) ownerCore.Align = ContentAlignment.MiddleLeft;
			if(sender == middleCenter) ownerCore.Align = ContentAlignment.MiddleCenter;
			if(sender == middleRight) ownerCore.Align = ContentAlignment.MiddleRight;
			if(sender == bottomLeft) ownerCore.Align = ContentAlignment.BottomLeft;
			if(sender == bottomCenter) ownerCore.Align = ContentAlignment.BottomCenter;
			if(sender == bottomRight) ownerCore.Align = ContentAlignment.BottomRight;
		}
		private void InitComponent() {
			base.Size = new Size(125, 89);
			this.BackColor = SystemColors.Control;
			this.ForeColor = SystemColors.ControlText;
			this.topLeft.Size = new Size(24, 25);
			this.topLeft.TabIndex = 8;
			this.topLeft.Text = "";
			this.topLeft.Appearance = Appearance.Button;
			this.topLeft.Click += new EventHandler(this.OptionClick);
			this.topCenter.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.topCenter.Location = new Point(32, 0);
			this.topCenter.Size = new Size(59, 25);
			this.topCenter.TabIndex = 0;
			this.topCenter.Text = "";
			this.topCenter.Appearance = Appearance.Button;
			this.topCenter.Click += new EventHandler(this.OptionClick);
			this.topRight.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.topRight.Location = new Point(99, 0);
			this.topRight.Size = new Size(24, 25);
			this.topRight.TabIndex = 1;
			this.topRight.Text = "";
			this.topRight.Appearance = Appearance.Button;
			this.topRight.Click += new EventHandler(this.OptionClick);
			this.middleLeft.Location = new Point(0, 32);
			this.middleLeft.Size = new Size(24, 25);
			this.middleLeft.TabIndex = 2;
			this.middleLeft.Text = "";
			this.middleLeft.Appearance = Appearance.Button;
			this.middleLeft.Click += new EventHandler(this.OptionClick);
			this.middleCenter.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.middleCenter.Location = new Point(32, 32);
			this.middleCenter.Size = new Size(59, 25);
			this.middleCenter.TabIndex = 3;
			this.middleCenter.Text = "";
			this.middleCenter.Appearance = Appearance.Button;
			this.middleCenter.Click += new EventHandler(this.OptionClick);
			this.middleRight.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.middleRight.Location = new Point(99, 32);
			this.middleRight.Size = new Size(24, 25);
			this.middleRight.TabIndex = 4;
			this.middleRight.Text = "";
			this.middleRight.Appearance = Appearance.Button;
			this.middleRight.Click += new EventHandler(this.OptionClick);
			this.bottomLeft.Location = new Point(0, 64);
			this.bottomLeft.Size = new Size(24, 25);
			this.bottomLeft.TabIndex = 5;
			this.bottomLeft.Text = "";
			this.bottomLeft.Appearance = Appearance.Button;
			this.bottomLeft.Click += new EventHandler(this.OptionClick);
			this.bottomCenter.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.bottomCenter.Location = new Point(32, 64);
			this.bottomCenter.Size = new Size(59, 25);
			this.bottomCenter.TabIndex = 6;
			this.bottomCenter.Text = "";
			this.bottomCenter.Appearance = Appearance.Button;
			this.bottomCenter.Click += new EventHandler(this.OptionClick);
			this.bottomRight.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.bottomRight.Location = new Point(99, 64);
			this.bottomRight.Size = new Size(24, 25);
			this.bottomRight.TabIndex = 7;
			this.bottomRight.Text = "";
			this.bottomRight.Appearance = Appearance.Button;
			this.bottomRight.Click += new EventHandler(this.OptionClick);
			base.Controls.Clear();
			base.Controls.AddRange(new Control[]
				{
					this.bottomRight,
					this.bottomCenter,
					this.bottomLeft,
					this.middleRight,
					this.middleCenter,
					this.middleLeft,
					this.topRight,
					this.topCenter,
					this.topLeft
				});
		}
	}
}
