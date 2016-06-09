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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using System.IO;
namespace DevExpress.XtraBars.WinRTLiveTiles {
	public partial class TileTemplateHelperForm : XtraForm {
		string wideDefinition = null;
		string squareDefinition = null;
		string updateDefinition = null;
		const string strNone = "none";
		const string strText = "text";
		const string strImage = "image";
		string componentName = "componentName";
		const string createPrefix = "Create";
		const string updateDefinitionEnd = ".UpdateTile(wideTile, squareTile);";
		const string updateDefinitionEndNull = ".UpdateTile(wideTile, null);";
		const string wideDefinitionStart = "WideTile wideTile = WideTile.Create";
		const string squareDefinitionStart = "SquareTile squareTile = SquareTile.Create";
		const string tileDefinitionMask = "{0}{1}({2});";
		MethodInfo wideMethod;
		MethodInfo squareMethod;
		Assembly designerAssembly;
		public TileTemplateHelperForm(string componentName, Assembly designerAssemblyInput) {
			this.componentName = componentName;
			designerAssembly = designerAssemblyInput;
			Visible = false;
			ShowInTaskbar = false;
			MinimizeBox = false;
			SizeGripStyle = SizeGripStyle.Hide;
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			InitializeComponent();
			MaximumSize = Size;
			MinimumSize = Size;
			StartPosition = FormStartPosition.CenterScreen;
			comboBoxEdit1.Properties.TextEditStyle = XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			comboBoxEdit2.Properties.TextEditStyle = XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			FillTemplates();
			comboBoxEdit1.SelectedValueChanged += comboBoxEdit1_SelectedValueChanged;
			comboBoxEdit2.SelectedValueChanged += comboBoxEdit2_SelectedValueChanged;
			comboBoxEdit1.SelectedIndex = 0;
			comboBoxEdit2.SelectedIndex = 0;
			pictureBoxWide.SizeMode = pictureBoxSquare.SizeMode = PictureBoxSizeMode.StretchImage;
			TileItem wide = new TileItem();
			wide.Elements.Clear();
			wide.ItemSize = TileItemSize.Wide;
			TileItem square = new TileItem();
			square.Elements.Clear();
			tileGroup1.Items.Add(wide);
			tileGroup2.Items.Add(square);
			wideHelper = new TileControlTemplateHelper(wide);
			squareHelper = new TileControlTemplateHelper(square);
		}
		TileControlTemplateHelper wideHelper;
		TileControlTemplateHelper squareHelper;
		const string pieceofpath = @"D:\temp\1\";
		void FillTemplates() {
			comboBoxEdit1.Properties.Items.AddRange(Enum.GetValues(typeof(WideTileTypes)));
			comboBoxEdit2.Properties.Items.Add(strNone);
			comboBoxEdit2.Properties.Items.AddRange(Enum.GetValues(typeof(SquareTileTypes)));
		}
		void comboBoxEdit1_SelectedValueChanged(object sender, EventArgs e) {
			GenerateWide();
			if(wideHelper == null) return;
			string filename = ((ComboBoxEdit)sender).Text + ".xml";
			string file = Path.Combine(pieceofpath, filename);
			wideHelper.SetTemplate(wideHelper.LoadTemplate(file));
		}
		void comboBoxEdit2_SelectedValueChanged(object sender, EventArgs e) {
			GenerateSquare();
		}
		const string pathToPicturesStart = "DevExpress.XtraBars.Design.WinRTPresenter.Images.";
		private void UpdatePicture(PictureBox pictureBox, ComboBoxEdit comboBox) {
			if(designerAssembly != null) {
				pictureBox.Image = ResourceImageHelper.CreateImageFromResources(pathToPicturesStart + comboBox.Text + ".png", designerAssembly);
				pictureBox.Visible = comboBox.Text == strNone ? false : true;
				pictureBox.Height = comboBox.Text.Contains("Peek") ? 240 : 120;
			}
		}
		void GenerateUpdate() {
			if(comboBoxEdit2.Text == strNone) updateDefinition = componentName + updateDefinitionEndNull;
			else updateDefinition = componentName + updateDefinitionEnd;
		}
		void GenerateWide() {
			wideMethod = typeof(WideTile).GetMethod(createPrefix + comboBoxEdit1.Text);
			wideDefinition = String.Format(tileDefinitionMask, wideDefinitionStart, comboBoxEdit1.Text, GetParameters(wideMethod , true));
			UpdateMemo();
		}
		void GenerateSquare() {
			if(comboBoxEdit2.Text != strNone) {
				squareMethod = typeof(SquareTile).GetMethod(createPrefix + comboBoxEdit2.Text);
				squareDefinition = String.Format(tileDefinitionMask, squareDefinitionStart, comboBoxEdit2.Text, GetParameters(squareMethod, false));
			}
			else {
				squareDefinition = null;
				currentSquareImgCount = 0;
				currentSquareStringsCount = 0;
			} 
			UpdateMemo();
		}
		void UpdateMemo() {
			GenerateUpdate();
			memoEdit1.Text = "";
			FillStringDeclaration(Math.Max(currentWideStringsCount, currentSquareStringsCount));
			FillImgDeclaration(Math.Max(currentWideImgCount, currentSquareImgCount));
			AppendList(memoEdit1, stringDeclarations);
			AppendList(memoEdit1, imgDeclarations);
			memoEdit1.Text += Environment.NewLine + wideDefinition;
			if(!String.IsNullOrEmpty(squareDefinition)) memoEdit1.Text += Environment.NewLine + squareDefinition;
			memoEdit1.Text += Environment.NewLine + updateDefinition;
			stringDeclarations.Clear();
			imgDeclarations.Clear();
		}
		string GetParameters(MethodInfo methodInfo, bool isWide) {
			ParameterInfo[] parametersArray = methodInfo.GetParameters();
			string parametersString = String.Empty;
			string prefix = "";
			int textCounter = 1;
			int imgCounter = 1;
			if(isWide) { currentWideStringsCount = 0; currentWideImgCount = 0; }
			else { currentSquareStringsCount = 0; currentSquareImgCount = 0; }
			foreach(ParameterInfo parameter in parametersArray) {
				if(parameter.ParameterType == typeof(string)) {
					parametersString += String.Format("{0}{1}{2}", prefix, strText, textCounter.ToString());
					textCounter++;
					if(isWide) currentWideStringsCount++; 
					else currentSquareStringsCount++;
				}
				else {
					parametersString += String.Format("{0}{1}{2}", prefix, strImage, imgCounter.ToString());
					imgCounter++;
					if(isWide) currentWideImgCount++;
					else currentSquareImgCount++;
				}
				prefix = ", ";
			}
			return parametersString;
		}
		int currentWideStringsCount = 0;
		int currentSquareStringsCount = 0;
		int currentWideImgCount = 0;
		int currentSquareImgCount = 0;
		List<string> stringDeclarations = new List<string>();
		List<string> imgDeclarations = new List<string>();
		const string strDeclarationMask = "string text{0} = \"My text{0}\"; ";
		void FillStringDeclaration(int size) {
			if(stringDeclarations == null) stringDeclarations = new List<string>();
			for(int i = 1; i < size+1; i++) {
				stringDeclarations.Add(String.Format(strDeclarationMask, i));
			}
		}
		const string imgDeclarationMask = "Image image{0} = Image.FromFile(@\"{1}\"); ";
		void FillImgDeclaration(int size) {
			if(imgDeclarations == null) imgDeclarations = new List<string>();
			for(int i = 1; i < size+1; i++) {
				imgDeclarations.Add(String.Format(imgDeclarationMask, i, GetTestpath())); 
			}
		}
		void AppendList(MemoEdit memo, List<string> list) {
			if(list == null || list.Count == 0 || memo == null) return;
			string prefix;
			if(memo.Text == "") prefix = "";
			else prefix = Environment.NewLine;
			foreach(string declarationString in list) {
				memo.Text += prefix + declarationString;
				prefix = Environment.NewLine;
			}
		}
		static string GetTestpath() {
			return @"D:\temp\temp.jpg";
		}
		static string GetProgramFilesx86() {
			if(IntPtr.Size == 8 || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")))) {
				return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
			}
			return Environment.GetEnvironmentVariable("ProgramFiles");
		}
		private void simpleButton1_Click(object sender, EventArgs e) {
			if(!String.IsNullOrEmpty(memoEdit1.Text))
				Clipboard.SetText(memoEdit1.Text);
			this.Close();
		}
		private void TileTemplateHelperForm_Load(object sender, EventArgs e) {
		}
	}
}
