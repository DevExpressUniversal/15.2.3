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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.Utils;
namespace DevExpress.XtraTreeList.Blending {
	[Designer("DevExpress.XtraTreeList.Design.XtraTreeListBlendingDesigner, " + AssemblyInfo.SRAssemblyTreeListDesign, typeof(IDesigner)),
	 ToolboxTabName(AssemblyInfo.DXTabComponents),
	 Description("Manipulates transparency options for a TreeList control's appearance settings."),
	 ToolboxItem(false)
	]
	public class XtraTreeListBlending : Component {
		#region Private fields
		private TreeList tlControl;
		private AlphaStyles alphaStyles;
		private bool rtStart;
		private bool enabled;
		#endregion
		#region Static arrays
		public static string[] defaultStyleNames = 
			new string[] {"HeaderPanel", "FooterPanel", "Row", 
							 "EvenRow", "OddRow", "Preview", 
							 "FocusedRow", "FocusedCell", "GroupButton", 
							 "GroupFooter", "Empty", "SelectedRow", "HideSelectionRow"
						 };
		internal static int[] defaultAlpha = 
			new int[] {230, 255, 90,
						  50, 90, 120, 
						  255, 255, 255,
						  255, 0, 230, 255
					  };
		#endregion
		#region Ctor
		public XtraTreeListBlending() {
			tlControl = null;
			rtStart = true;
			enabled = true;
			alphaStyles = new AlphaStyles(this);
			alphaStyles.NeedSerializeStyle += new CheckSerializeEventHandler(OnCheckIsNeedSerialize);
			FillAlphaStyles(AlphaStyles, defaultStyleNames, defaultAlpha);
		}
		#endregion
		#region Properties
		[Editor("DevExpress.XtraTreeList.Blending.UIAlphaStyleEditor, " + AssemblyInfo.SRAssemblyTreeList, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AlphaStyles AlphaStyles {
			get { return alphaStyles; }
		}
		[DefaultValue(true)]
		public bool Enabled {
			get { return enabled; }
			set { 
				if(enabled != value) {
					enabled = value;
					if(!DesignMode && tlControl != null) {
						rtStart = false;
						RefreshStyles();
					}
				}
			}
		}		
		[DefaultValue(null)]
		public TreeList TreeListControl {
			get { return tlControl; }
			set { 
				if(TreeListControl == value) return;
				if(TreeListControl != null) {
					TreeListControl.HandleCreated -= new EventHandler(TreeListControlCreated);
					TreeListControl.Blending = null;
				}
				tlControl = value;
				if(TreeListControl != null) {
					TreeListControl.HandleCreated += new EventHandler(TreeListControlCreated);
					TreeListControl.Blending = this;
				}
			}
		}
		#endregion
		private void TreeListControlCreated(object sender, EventArgs e) {
			if(!DesignMode && tlControl != null && rtStart && Enabled) {
				RefreshStyles();
				rtStart = false;
			}
		}
		private int IndexByName(string[] names, string s) {
			for(int i = 0; i < names.Length; i++)
				if(names[i] == s) return i;
			return -1;
		}
		void OnCheckIsNeedSerialize(object sender, CheckSerializeEventArgs e) {
			int index = IndexByName(defaultStyleNames, (string)e.Entry.Key);
			if(index != -1)
				e.NeedSerialize =  defaultAlpha[index] != (int)e.Entry.Value;
		}
		internal static void FillAlphaStyles(AlphaStyles styles, string[] defaultStyleNames, int[] defaultAlpha) {
			styles.Clear();
			for(int n = 0; n < defaultStyleNames.Length; n++) {
				styles.AddReplace((string)(defaultStyleNames[n]), 
					(int)(defaultAlpha[n]));
			}
		}
		public void ShowDialog() {
			AlphaStyleEditor editor = new AlphaStyleEditor(AlphaStyles, null);
			editor.ShowDialog();
		}
		public void RefreshStyles() {
			if(tlControl != null) 
				RefreshPaintAppearance(tlControl);
		}
		internal void RefreshPaintAppearance(TreeList tl) {
			tl.Blending = this;
			tl.ViewInfo.SetAppearanceDirty();
			tl.LayoutChanged();
		}
	}
	[System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraTreeList.Design.Serializers.TreeListAlphaStylesCodeDomSerializer, " + AssemblyInfo.SRAssemblyTreeListDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class AlphaStyles : Hashtable {
		public event CheckSerializeEventHandler NeedSerializeStyle;
		public override object Clone() {
			AlphaStyles ss = new AlphaStyles(parent);
			foreach(DictionaryEntry entry in this) {
				ss.Add(entry.Key, entry.Value);
			}
			return ss;
		}
		public void AddReplace(object key, object value) {
			if(Contains(key)) Remove(key);
			base.Add(key, value);
		}
		public bool IsNeedSerializeStyle(DictionaryEntry entry) {
			if(NeedSerializeStyle != null) {
				CheckSerializeEventArgs e = new CheckSerializeEventArgs(entry);
				NeedSerializeStyle(this, e);
				return e.NeedSerialize;
			}
			return true;
		}
		internal object parent;
		public AlphaStyles(object parent) {
			this.parent = parent;
		}
	}
	public delegate void CheckSerializeEventHandler(object sender, CheckSerializeEventArgs e);
	public class CheckSerializeEventArgs : EventArgs {
		bool needSerialzie;
		DictionaryEntry entry;
		public CheckSerializeEventArgs(DictionaryEntry entry) {
			this.needSerialzie = true;
			this.entry = entry;
		}
		public DictionaryEntry Entry { get { return entry; } }
		public bool NeedSerialize {
			get { return needSerialzie; }
			set { needSerialzie = value; }
		}
	}
}
