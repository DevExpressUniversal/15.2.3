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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Commands;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Wizard;
namespace DevExpress.XtraCharts.Commands {
	public class SaveAsTemplateCommand : ChartCommand {
		public override string ImageName { get { return "SaveAsTemplate"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdSaveAsTemplateDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdSaveAsTemplateMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.SaveAsTemplate; } }
		public SaveAsTemplateCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "XML Files|*.xml|All files|*.*";
			saveFileDialog.Title = "Save Chart As Template";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "") {
				FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
				Chart.SaveLayout(file);
				file.Close();
			}
		}
	}
	public class LoadTemplateCommand : ChartCommand {
		public override string ImageName { get { return "LoadTemplate"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdLoadTemplateDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdLoadTemplateMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.LoadTemplate; } }
		public LoadTemplateCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "XML Files|*.xml|All files|*.*";
			openFileDialog.Title = "Load Chart From Template";
			if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName != "") {
				try {
					using (FileStream file = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
						Chart.LoadLayout(file);
				}
				catch (Exception ex) {
					MessageBox.Show(ex.Message, ChartLocalizer.GetString(ChartStringId.IOCaption), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);					
				}
			}
		}
	}
}
