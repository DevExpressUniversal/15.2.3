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
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Utils.Drawing;
using DevExpress.Skins.XtraForm;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Text;
using DevExpress.Utils.Win;
using System.Runtime.InteropServices;
using DevExpress.Utils.Drawing.Animation;
using System.Drawing.Imaging;
namespace DevExpress.XtraBars.Alerter {
	class PostponedArgs {
		Form owner;
		Point location;
		public PostponedArgs(Form owner, Point location) {
			this.owner = owner;
			this.location = location;
		}
		public Form Owner { get { return owner; } }
		public Point Location { get { return location; } }
	}
	public class AlertForm : AlertFormCore {
		PostponedArgs postponedArgs = null;
		public AlertForm(Point location, IAlertControl control, AlertInfo info) : base(location, control, info) { }
		protected override void CreatePopupMenuPutton(AlertButtonCollection collection) {
			AlertControl control = AlertControl as AlertControl;
			if(control != null && control.PopupMenu != null)
				collection.Add(new AlertPopupMenuButton(this, collection, control.PopupMenu));
		}
		internal void SetPosponed(Form owner, Point location) {
			postponedArgs = new PostponedArgs(owner, location);
		}
		internal PostponedArgs PostponedArgs { get { return postponedArgs; } }
		protected override void WndProc(ref Message msg) {
			base.WndProc(ref msg);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
	}
}
