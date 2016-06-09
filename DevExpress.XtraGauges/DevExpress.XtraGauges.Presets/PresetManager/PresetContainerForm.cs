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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Presets;
namespace DevExpress.XtraGauges.Presets.PresetManager {
	public enum Actions {
		UpdatePreset, 
		CreateImage
	};
	public partial class PresetContainerForm : Form {
		Dictionary<string, string> StyleNames;
		protected SimpleButton saveButton;
		public PresetContainerForm() {
			StyleNames = new Dictionary<string, string>();
			StyleNames.Add("Style1", "White");
			StyleNames.Add("Style2", "Dark Night");
			StyleNames.Add("Style3", "Deep Fire");
			StyleNames.Add("Style4", "Ice-Cold Zone");
			StyleNames.Add("Style5", "Gothic Mat");
			StyleNames.Add("Style6", "Shining Dark");
			StyleNames.Add("Style7", "Africa Sunset");
			StyleNames.Add("Style8", "Mechanical");
			StyleNames.Add("Style9", "Silver Blur");
			StyleNames.Add("Style10", "Pure Dark");
			StyleNames.Add("Style11", "Clean White");
			StyleNames.Add("Style12", "Sport Car");
			StyleNames.Add("Style13", "Military");
			StyleNames.Add("Style14", "Retro");
			StyleNames.Add("Style15", "Disco");
			StyleNames.Add("Style16", "Clever");
			StyleNames.Add("Style17", "Cosmic");
			StyleNames.Add("Style18", "Smart");
			StyleNames.Add("Style19", "Progressive");
			StyleNames.Add("Style20", "Eco");
			StyleNames.Add("Style21", "Magic Light");
			StyleNames.Add("Style22", "iStyle");
			StyleNames.Add("Style23", "Future");
			StyleNames.Add("Style24", "Yellow Submarine");
			StyleNames.Add("Style25", "Classic");
			StyleNames.Add("Style26", "Red");
			StyleNames.Add("Style27", "Flat Light");
			StyleNames.Add("Style28", "Flat Dark");
			StyleNames.Add("Style29", "Ignis");
			StyleNames.Add("Style30", "Haze");
			StyleNames.Add("Style1.Left", "White Left");
			StyleNames.Add("Style2.Left", "Dark Night Left");
			StyleNames.Add("Style3.Left", "Deep Fire Left");
			StyleNames.Add("Style4.Left", "Ice-Cold Zone Left");
			StyleNames.Add("Style5.Left", "Gothic Mat Left");
			StyleNames.Add("Style6.Left", "Shining Dark Left");
			StyleNames.Add("Style7.Left", "Africa Sunset Left");
			StyleNames.Add("Style8.Left", "Mechanical Left");
			StyleNames.Add("Style9.Left", "Silver Blur Left");
			StyleNames.Add("Style10.Left", "Pure Dark Left");
			StyleNames.Add("Style11.Left", "Clean White Left");
			StyleNames.Add("Style12.Left", "Sport Car Left");
			StyleNames.Add("Style13.Left", "Military Left");
			StyleNames.Add("Style14.Left", "Retro Left");
			StyleNames.Add("Style15.Left", "Disco Left");
			StyleNames.Add("Style16.Left", "Clever Left");
			StyleNames.Add("Style17.Left", "Cosmic Left");
			StyleNames.Add("Style18.Left", "Smart Left");
			StyleNames.Add("Style19.Left", "Progressive Left");
			StyleNames.Add("Style20.Left", "Eco Left");
			StyleNames.Add("Style21.Left", "Magic Light Left");
			StyleNames.Add("Style22.Left", "iStyle Left");
			StyleNames.Add("Style23.Left", "Future Left");
			StyleNames.Add("Style24.Left", "Yellow Submarine Left");
			StyleNames.Add("Style25.Left", "Classic Left");
			StyleNames.Add("Style26.Left", "Red Left");
			StyleNames.Add("Style27.Left", "Flat Light");
			StyleNames.Add("Style28.Left", "Flat Dark");
			StyleNames.Add("Style1.Right", "White Right");
			StyleNames.Add("Style2.Right", "Dark Night Right");
			StyleNames.Add("Style3.Right", "Deep Fire Right");
			StyleNames.Add("Style4.Right", "Ice-Cold Zone Right");
			StyleNames.Add("Style5.Right", "Gothic Mat Right");
			StyleNames.Add("Style6.Right", "Shining Dark Right");
			StyleNames.Add("Style7.Right", "Africa Sunset Right");
			StyleNames.Add("Style8.Right", "Mechanical Right");
			StyleNames.Add("Style9.Right", "Silver Blur Right");
			StyleNames.Add("Style10.Right", "Pure Dark Right");
			StyleNames.Add("Style11.Right", "Clean White Right");
			StyleNames.Add("Style12.Right", "Sport Car Right");
			StyleNames.Add("Style13.Right", "Military Right");
			StyleNames.Add("Style14.Right", "Retro Right");
			StyleNames.Add("Style15.Right", "Disco Right");
			StyleNames.Add("Style16.Right", "Clever Right");
			StyleNames.Add("Style17.Right", "Cosmic Right");
			StyleNames.Add("Style18.Right", "Smart Right");
			StyleNames.Add("Style19.Right", "Progressive Right");
			StyleNames.Add("Style20.Right", "Eco Right");
			StyleNames.Add("Style21.Right", "Magic Light Right");
			StyleNames.Add("Style22.Right", "iStyle Right");
			StyleNames.Add("Style23.Right", "Future Right");
			StyleNames.Add("Style24.Right", "Yellow Submarine Right");
			StyleNames.Add("Style25.Right", "Classic Right");
			StyleNames.Add("Style26.Right", "Red Right");
			StyleNames.Add("Style27.Right", "Flat Light");
			StyleNames.Add("Style28.Right", "Flat Dark");
			saveButton = new SimpleButton();
			saveButton.Text = "UpdatePresets";
			saveButton.Parent = this;
			saveButton.Size = new Size(200, 25);
			saveButton.Dock = DockStyle.Bottom;
			saveButton.Click += new EventHandler(OnSave);
			InitializeComponent();
		}
		void OnSave(object sender, EventArgs e) {
			UpdatePresets(Actions.UpdatePreset);
		}
		public virtual void UpdateImages() { }
		public virtual void UpdatePresets(Actions action) {
			foreach(Control control in Controls) {
				IGaugeContainer gc = control as IGaugeContainer;
				if(gc == null) continue;
				PresetCategory category = (gc.Gauges.Count == 1) ? GetGaugePesetCategory(gc.Gauges[0]) : PresetCategory.CircularFull;
				string gaugeTypeName = gc.Name.Replace("_", ".");
				switch(action) {
					case Actions.UpdatePreset:
						SavePreset(gc, category, gaugeTypeName);
						break;
					case Actions.CreateImage:
						CreateImage(gc, category, gaugeTypeName);
						break;
				}
			}
		}
		PresetCategory GetGaugePesetCategory(IGauge gauge) {
			PresetCategory result = PresetCategory.CircularFull;
			String category = gauge.Category;
			if(gauge.Name.StartsWith("__COMPLEX__")) {
				gauge.Name = gauge.Name.Replace("__COMPLEX__", "");
				return PresetCategory.Complex;
			}
			switch(gauge.GetType().Name) {
				case "DigitalGauge": result = PresetCategory.Digital; break;
				case "CircularGauge":
					if(category.Contains("Full")) result = PresetCategory.CircularFull;
					if(category.Contains("Half")) result = PresetCategory.CircularHalf;
					if(category.Contains("Quarter")) result = PresetCategory.CircularQuarter;
					if(category.Contains("ThreeFourth")) result = PresetCategory.CircularThreeFourth;
					if(category.Contains("Wide")) result = PresetCategory.CircularWide;
					break;
				case "LinearGauge":
					if(category.Contains("Vertical")) result = PresetCategory.LinearVertical;
					else result = PresetCategory.LinearHorizontal;
					break;
				case "StateIndicatorGauge":
					result = PresetCategory.StateIndicator;
					break;
				default: throw new NotImplementedException("TODO");
			}
			return result;
		}
		protected virtual void CreateImage(IGaugeContainer gc, PresetCategory category, string name) {
			Image bmp = gc.GetImage(250, 250);
			bmp.Save(category.ToString() + name + ".jpeg");
		}
		protected virtual void SavePreset(IGaugeContainer gc, PresetCategory category, string name) {
			BaseGaugePreset cPreset = BaseGaugePreset.FromGaugeContainer(gc);
			cPreset.Category = category;
			cPreset.Name = GetStyleName(name);
			using(MemoryStream ms = new MemoryStream()) {
				new BinaryXtraSerializer().SerializeObject(gc, ms, "IGaugeContainer");
				cPreset.LayoutInfo = ms.ToArray();
			}
			using(FileStream fs = new FileStream(PresetEditControl.GetFileName(cPreset, name), FileMode.CreateNew)) {
				cPreset.SaveToStream(fs);
			}
		}
		string GetStyleName(string name) {
			string result;
			return StyleNames.TryGetValue(name, out result) ? result : name;
		}
	}
}
