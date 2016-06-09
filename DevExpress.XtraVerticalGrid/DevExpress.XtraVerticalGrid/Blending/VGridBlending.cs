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
using DevExpress.XtraVerticalGrid;
using DevExpress.Utils;
using DevExpress.XtraVerticalGrid.ViewInfo;
namespace DevExpress.XtraVerticalGrid.Blending {
	[
	Designer("DevExpress.XtraVerticalGrid.Design.XtraVerticalGridBlendingDesigner, " + AssemblyInfo.SRAssemblyVertGridDesign, typeof(IDesigner)),
	ToolboxBitmap(typeof(DevExpress.XtraVerticalGrid.VGridControlBase), DevExpress.Utils.ControlConstants.BitmapPath + "XtraVertGridBlending.bmp"),
	Description("Manipulates transparency options for a VGridControl and PropertyGridControl control's appearance settings."),
	ToolboxItem(false), ToolboxTabName(AssemblyInfo.DXTabComponents)
	]
	public class XtraVertGridBlending : Component {
		#region Private fields
		private VGridControlBase vgControl;
		private AlphaStyles alphaStyles;
		private bool rtStart;
		private bool enabled;
		#endregion
		#region Static arrays
		public static AlphaStyle[] defaultAlphaStyles = new AlphaStyle[] {
			new AlphaStyle(GridStyles.ReadOnlyRow, 210),
			new AlphaStyle(GridStyles.ModifiedRow, 210),
			new AlphaStyle(GridStyles.DisabledRow, 200),
			new AlphaStyle(GridStyles.FocusedRow, 200),
			new AlphaStyle(GridStyles.HideSelectionRow, 250),
			new AlphaStyle(GridStyles.PressedRow, 250),
			new AlphaStyle(GridStyles.Category, 210),
			new AlphaStyle(GridStyles.Empty, 50),
			new AlphaStyle(GridStyles.FocusedCell, 110),
			new AlphaStyle(GridStyles.FocusedRecord, 90),
			new AlphaStyle(GridStyles.RecordValue, 90),
			new AlphaStyle(GridStyles.DisabledRecordValue, 90),
			new AlphaStyle(GridStyles.ReadOnlyRecordValue, 90),
			new AlphaStyle(GridStyles.ModifiedRecordValue, 90),
			new AlphaStyle(GridStyles.RowHeaderPanel, 190)
		};
		#endregion
		#region Init
		public XtraVertGridBlending() {
			vgControl = null;
			rtStart = true;
			enabled = true;
			alphaStyles = new AlphaStyles(this);
			alphaStyles.NeedSerializeStyle += new CheckSerializeEventHandler(OnCheckIsNeedSerialize);
			FillAlphaStyles(AlphaStyles, defaultAlphaStyles);
		}
		#endregion
		#region Properties
		[Editor(typeof(UIAlphaStyleEditor), typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AlphaStyles AlphaStyles {
			get { return alphaStyles; }
		}
		[DefaultValue(null)]
		public VGridControlBase VertGridControl {
			get { return vgControl; }
			set { 
				if(VertGridControl == value) return;
				if(VertGridControl != null) {
					VertGridControl.HandleCreated -= new EventHandler(VertGridControlLoad);
					VertGridControl.Blending = null;
				}
				vgControl = value;
				if(VertGridControl != null) {
					VertGridControl.HandleCreated += new EventHandler(VertGridControlLoad);
					VertGridControl.Blending = this;
				}
			}
		}
		[DefaultValue(true)]
		public bool Enabled {
			get { return enabled; }
			set { 
				if(enabled != value) {
					enabled = value;
					if(!DesignMode && vgControl != null) {
						rtStart = false;
						RefreshStyles();
					}
				}
			}
		}
		#endregion
		#region Function
		private int IndexByName(AlphaStyle[] styles, string s) {
			for(int i = 0; i < styles.Length; i++)
				if(styles[i].StyleName == s) return i;
			return -1;
		}
		void OnCheckIsNeedSerialize(object sender, CheckSerializeEventArgs e) {
			int index = IndexByName(defaultAlphaStyles, (string)e.Entry.Key);
			if(index != -1)
				e.NeedSerialize = defaultAlphaStyles[index].Alpha != (int)e.Entry.Value;
		}
		internal static void FillAlphaStyles(AlphaStyles styles, AlphaStyle[] alphaStyles) {
			styles.Clear();
			for(int n = 0; n < alphaStyles.Length; n++) {
				styles.AddReplace(alphaStyles[n].StyleName, alphaStyles[n].Alpha);
			}
		}
		private void VertGridControlLoad(object sender, EventArgs e) {
			if(!DesignMode && vgControl != null && rtStart && Enabled) {
				RefreshStyles();
				rtStart = false;
			}
		}
		public void ShowDialog() {
			AlphaStyleEditor editor = new AlphaStyleEditor(AlphaStyles, null);
			editor.ShowDialog();
		}
		public void RefreshStyles() { RefreshStyles(-1); } 
		private void RefreshStyles(int alpha) {
			if(vgControl == null) return;
			RefreshPaintAppearance(vgControl);
		}
		internal void RefreshPaintAppearance(VGridControlBase grid) {
			grid.Blending = this;
			grid.ViewInfo.SetAppearanceDirty();
			grid.LayoutChanged();
		}
		#endregion
	}
	[System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraVerticalGrid.Design.Serializers.VertGridAlphaStylesCodeDomSerializer, " + AssemblyInfo.SRAssemblyVertGridDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
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
	public class AlphaStyle {
		string styleName;
		int alpha;
		public AlphaStyle(string styleName, int alpha) {
			this.styleName = styleName;
			this.alpha = alpha;
		}
		public string StyleName { get { return styleName; } }
		internal int Alpha { get { return alpha; } }
		public override string ToString() {
			return base.ToString();
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
