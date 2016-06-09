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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Presets;
namespace DevExpress.XtraGauges.Presets.PresetManager {
	[System.ComponentModel.ToolboxItem(false)]
	public class PresetsTreeView : DXTreeView {
		public PresetsTreeView() {
			this.BorderStyle = BorderStyle.None;
			this.ImageList = new ImageList();
			ImageList.ImageSize = new Size(15, 15);
			for(int i = 0; i < Presets.Resources.UIHelper.PresetCategoryImages.Images.Count; i++) {
				ImageList.Images.Add(Presets.Resources.UIHelper.PresetCategoryImages.Images[i]);
			}
			this.ShowLines = false;
			this.ShowRootLines = false;
			this.ItemHeight = 20;
		}
		public virtual void FillCategories() {
			Font font = new Font("Tahoma", 8.25f);
			ICollection categories = Enum.GetNames(typeof(PresetCategory));
			foreach(string name in categories) {
				TreeNode node = Nodes.Add(name);
				node.ImageIndex = node.SelectedImageIndex = (int)Enum.Parse(typeof(PresetCategory), name);
				node.NodeFont = font;
			}
		}
	}
	public class BasePresetNode : TreeNode {
		IBaseGaugePreset presetCore;
		public BasePresetNode(IBaseGaugePreset preset) {
			this.presetCore = preset;
			this.Text = preset.Name;
			this.ImageIndex = preset.IconIndex;
			this.SelectedImageIndex = preset.IconIndex;
		}
		public IBaseGaugePreset Preset {
			get { return presetCore; }
		}
	}
	public class CurrentPresetNode : BasePresetNode {
		public CurrentPresetNode(IGaugeContainer gaugeContainer)
			: base(BaseGaugePreset.FromGaugeContainer(gaugeContainer)) {
		}
	}
}
