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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
namespace DevExpress.XtraGrid.Blending {
	[Designer("DevExpress.XtraGrid.Design.XtraGridBlendingDesigner, " + AssemblyInfo.SRAssemblyGridDesign, typeof(IDesigner)),
	 Description("Manipulates transparency options for a GridControl's appearance settings."),
	 ToolboxTabName(AssemblyInfo.DXTabComponents),
	 ToolboxItem(false)
	]
	public class XtraGridBlending : Component {
		#region Private fields
		private GridControl gridControl;
		private AlphaStyles alphaStyles;
		private bool rtStart;
		private bool enabled;
		#endregion
		#region Static arrays
		public static string[] defaultStyleNames = 
			new string[] {"BandPanelBackground", "BandPanel", "HeaderPanelBackground", "HeaderPanel",
							"GroupPanel", "FooterPanel", "Row", "RowSeparator", 
							"GroupRow", "EvenRow", "OddRow", "Preview", 
							"FocusedRow", "FocusedCell", "FilterPanel", 
							"GroupFooter", "Empty", "SelectedRow", "HideSelectionRow", "TopNewRow", 
							"CardCaption", "EmptySpace", "FieldCaption", "FieldValue", "FocusedCardCaption", 
							"FilterPanel", "HideSelectionCardCaption", "SelectedCardCaption", "Card"
							};
		internal static int[] defaultAlpha = 
			new int[] {255, 230, 255, 240,  
						220, 255, 90, 50, 
						150, 50, 90, 120, 
						255, 255, 255, 
						255, 0, 255, 255, 120, 
						255, 0, 200, 90, 255, 
						255, 255, 230, 255
						};
		#endregion
		#region Ctor
		public XtraGridBlending() {
			gridControl = null;
			rtStart = true;
			enabled = true;
			alphaStyles = new AlphaStyles(this);
			alphaStyles.NeedSerializeStyle += new CheckSerializeEventHandler(OnCheckIsNeedSerialize);
			FillAlphaStyles(AlphaStyles, defaultStyleNames, defaultAlpha);
		}
		#endregion
		#region Properties
		[Editor("DevExpress.XtraGrid.Blending.UIAlphaStyleEditor, " + AssemblyInfo.SRAssemblyGrid, typeof(System.Drawing.Design.UITypeEditor)),
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
					if(!DesignMode && gridControl != null) {
						rtStart = false;
						if(Enabled) 
							RefreshStyles();
						else
							SetOpacity();
					}
				}
			}
		}		
		[DefaultValue(null)]
		public GridControl GridControl {
			get { return gridControl; }
			set { 
				if(gridControl != value) {
					if(gridControl != null) GridControl.OldAlphaBlending = null;
					gridControl = value;
					if(gridControl != null) GridControl.OldAlphaBlending = this;
				}
			}
		}
		#endregion
		#region Functions
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
		private void GridControlPaint(object sender, EventArgs e) {
			if(!DesignMode && gridControl != null && rtStart && Enabled) {
				RefreshStyles();
				rtStart = false;
			}
		}
		public void ShowDialog() {
			AlphaStyleEditor editor = new AlphaStyleEditor(AlphaStyles, null);
			editor.ShowDialog();
		}
		public void SetOpacity() {
			RefreshStyles(255);
		}
		public void RefreshStyles() { RefreshStyles(-1); } 
		private void RefreshStyles(int alpha) {
			if(gridControl != null) {
				gridControl.BeginUpdate();
				try {
					foreach(BaseView view in gridControl.ViewCollection) {
						if(view.ViewInfo != null) view.ViewInfo.SetPaintAppearanceDirty();
						view.LayoutChanged();
					}
				} finally {
					gridControl.EndUpdate();
				}
			}
		}
		#endregion
	}
	[System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraGrid.Serializers.GridAlphaStylesCodeDomSerializer, " + AssemblyInfo.SRAssemblyGridDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
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
