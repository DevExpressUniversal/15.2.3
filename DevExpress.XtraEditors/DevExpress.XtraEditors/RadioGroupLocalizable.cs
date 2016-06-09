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
namespace DevExpress.XtraEditors.Internal {
	using DevExpress.XtraEditors.Controls;
	using System.ComponentModel.Design.Serialization;
	using System.ComponentModel.Design;
	using System.CodeDom;
	using System.ComponentModel;
	using System.Windows.Forms;
	[ToolboxItem(false), DesignerSerializer("DevExpress.XtraEditors.Design.RadioGroupSerializer, " + AssemblyInfo.SRAssemblyEditorsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class RadioGroupLocalizable : DevExpress.XtraEditors.RadioGroup {
		public RadioGroupLocalizable() : base () {
		}
		protected internal override void OnLoaded() {
			base.OnLoaded();
			if(DesignMode) return;
			Control control = GetResourceSource();
			if(control != null) SyncDescriptions(control.GetType());
		}
		Control GetResourceSource() {
			Control control = Parent;
			while(control != null) {
				if(control is Form || control is UserControl)
					return control;
				control = control.Parent;
			}
			return null;
		}
		void SyncDescriptions(Type resourceSource) {
			if(resourceSource == null || DesignMode) return;
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(resourceSource);
			if(resources == null)return;
			for(int i = 0; i < Properties.Items.Count; i++) { 
				try {
					Properties.Items[i].Description = resources.GetString(GetItemName(i));
				} catch {}
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string GetItemName(int index) {
			return String.Format("{0}.Item{1}.Description", Name, index);
		}
	}
}
