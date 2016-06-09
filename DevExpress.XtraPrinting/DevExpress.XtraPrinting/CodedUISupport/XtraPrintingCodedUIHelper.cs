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

 using System.Windows.Forms;
using System;
using DevExpress.Utils.CodedUISupport;
using DevExpress.XtraPrinting.Control;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.WinControls;
using System.Collections.Generic;
using System.Drawing.Printing;
using DevExpress.DocumentView;
using DevExpress.DocumentView.Controls;
using System.Reflection;
using DevExpress.XtraPrinting.Preview.Native;
using System.Collections;
namespace DevExpress.XtraPrinting.CodedUISupport {
	public class XtraPrintingCodedUIHelper : IXtraPrintingCodedUIHelper {
		RemoteObject remoteObject;
		public XtraPrintingCodedUIHelper(RemoteObject remoteObject) {
			this.remoteObject = remoteObject;
		}
		public PrintControlElements GetPrintingElementFromPoint(IntPtr windowHandle, int pointX, int pointY, out string path, out string sideMargin) {
			PrintControl printControl = GetPrintControlFromWindowHandle(windowHandle);
			if(printControl != null)
				return GetPrintingElementFromPoint(printControl, pointX, pointY, out path, out sideMargin);
			path = null;
			sideMargin = null;
			return PrintControlElements.Unknown;
		}
		protected PrintControlElements GetPrintingElementFromPoint(PrintControl printControl, int pointX, int pointY, out string path, out string sideMargin) {
			path = String.Empty;
			sideMargin = String.Empty;
			Page page = null;
			RectangleF brickBounds = RectangleF.Empty;
			Brick brick = ExtendedFindBrickBy(printControl, pointX, pointY, out page);
			if(brick != null) {
				int[] brickPath = printControl.GetBrickPath(brick, page);
				if(brickPath != null) {
					for(int i = 0; i < brickPath.Length - 1; i++) {
						path += brickPath[i].ToString() + ",";
					}
					path += brickPath[brickPath.Length - 1].ToString();
					return PrintControlElements.Brick;
				}
			}
			PointF pointMar = new Point(pointX, pointY);
			PageMargin margin = printControl.Margins.GetPointMargin(pointMar);
			if(margin != null) {
				MarginSide marginSide = printControl.Margins.GetPointSide(pointMar);
				sideMargin = TypeDescriptor.GetConverter(typeof(MarginSide)).ConvertToString(null, CultureInfo.InvariantCulture, marginSide);
				return PrintControlElements.Margin;
			}
			return PrintControlElements.Unknown;
		}
		public string GetPrintingElementRectangleOrMakeElementVisible(IntPtr windowHandle, PrintControlElements elementType, string path, string sideMargin) {
			PrintControl printControl = GetPrintControlFromWindowHandle(windowHandle);
			if(printControl != null) {
				Rectangle rectangle = GetPrintingElementRectangleOrMakeElementVisible(printControl, elementType, path, sideMargin);
				if(rectangle != Rectangle.Empty)
					return CodedUIUtils.ConvertToString(rectangle);
			}
			return null;
		}
		const int margingWidth = 4;
		const int searchMargin = 2;
		protected BrickBase GetBrickByIndices(Page page, int[] path) {
			if(page == null)
				return null;
			BrickBase brickBase = page;
			for(int i = 0; i < path.Length; i++) {
				int index = path[i];
				if(index >= brickBase.InnerBrickList.Count)
					break;
				brickBase = (BrickBase)brickBase.InnerBrickList[index];
			}
			return (BrickBase)brickBase;
		}
		protected Rectangle GetPrintingElementRectangleOrMakeElementVisible(PrintControl printControl, PrintControlElements elementType, string path, string sideMargin) {
			switch(elementType) {
				case PrintControlElements.Brick:
					if(path != null) {
						return GetBrickBounds(printControl, ConvertStringPathToIntArray(path));
					}
					return Rectangle.Empty;
				case PrintControlElements.Margin:
					if(sideMargin != null) {
						PointF startMargin = new PointF();
						PointF endMargin = new PointF();
						foreach(PageMargin mar in printControl.Margins) {
							if(sideMargin == mar.Side.ToString()) {
								mar.GetMarginLine(out startMargin, out endMargin);
								break;
							}
						}
						RectangleF marginRect;
						if(startMargin.Y == endMargin.Y) marginRect = new RectangleF(startMargin.X, startMargin.Y, endMargin.X - startMargin.X, margingWidth);
						else marginRect = new RectangleF(startMargin.X, startMargin.Y, margingWidth, endMargin.Y - startMargin.Y);
						return Rectangle.Round(marginRect);
					}
					return Rectangle.Empty;
				default: return Rectangle.Empty;
			}
		}
		Rectangle GetBrickBounds(PrintControl printControl, int[] path) {
			Page page = printControl.PrintingSystem.Pages[path[0]];
			if(page == null) return Rectangle.Empty;
			int[] brickIndices = new int[path.Length - 1];
			Array.Copy(path, 1, brickIndices, 0, brickIndices.Length);
			Brick brick = page.GetBrickByIndices(brickIndices) as Brick;
			if(brick == null) return Rectangle.Empty;
			Rectangle rect = printControl.GetBrickScreenBounds(brick, page);
			Point p = printControl.ViewControl.PointToClient(rect.Location);
			Rectangle resultRectangle = new Rectangle(p, rect.Size);
			return Rectangle.Round(resultRectangle);
		}
		public string GetBrickType(IntPtr windowHandle, string brickPathString) {
			BrickBase brick = GetBrickFromPrintControlWindowHandle(windowHandle, brickPathString);
			if(brick == null || (brick as Brick) == null) return null;
			return (brick as Brick).BrickType;
		}
		public string GetBrickText(IntPtr windowHandle, string brickPathString) {
			BrickBase brick = GetBrickFromPrintControlWindowHandle(windowHandle, brickPathString);
			if(brick == null) return null;
			if(brick is VisualBrick) return (brick as VisualBrick).Text;
			return null;
		}
		public string GetBrickCheckedState(IntPtr windowHandle, string brickPathString) {
			BrickBase brick = GetBrickFromPrintControlWindowHandle(windowHandle, brickPathString);
			if(brick == null) return null;
			if(brick is CheckBoxBrick) return (brick as CheckBoxBrick).Checked.ToString();
			return null;
		}
		public string GetMargin(IntPtr windowHandle, string marginSide) {
			PrintControl printControl = GetPrintControlFromWindowHandle(windowHandle);
			if(printControl == null) return null;
			switch(marginSide) {
				case "LeftMargin":
					return printControl.PrintingSystem.PageMargins.Left.ToString();
				case "RightMargin":
					return printControl.PrintingSystem.PageMargins.Right.ToString();
				case "TopMargin":
					return printControl.PrintingSystem.PageMargins.Top.ToString();
				case "BottomMargin":
					return printControl.PrintingSystem.PageMargins.Bottom.ToString();
			}
			return null;
		}
		public void SetMargin(IntPtr windowHandle, string marginSide, string marginValueAsString) {
			PrintControl printControl = GetPrintControlFromWindowHandle(windowHandle);
			int value = Int32.Parse(marginValueAsString);
			if(printControl != null) {
				switch(marginSide) {
					case "LeftMargin":
						printControl.BeginInvoke(new MethodInvoker(delegate() {
							printControl.PrintingSystem.PageSettings.LeftMargin = value;
						}));
						break;
					case "RightMargin":
						printControl.BeginInvoke(new MethodInvoker(delegate() {
							printControl.PrintingSystem.PageSettings.RightMargin = value;
						}));
						break;
					case "TopMargin":
						printControl.BeginInvoke(new MethodInvoker(delegate() {
							printControl.PrintingSystem.PageSettings.TopMargin = value;
						}));
						break;
					case "BottomMargin":
						printControl.BeginInvoke(new MethodInvoker(delegate() {
							printControl.PrintingSystem.PageSettings.BottomMargin = value;
						}));
						break;
				}
			}
		}
		public BrickBase GetBrickByPath(PrintControl printControl, string brickPathString) {
			if(brickPathString == null) return null;
			Page page = GetPageByBrickPath(printControl, brickPathString);
			int[] brickPath = ConvertStringPathToIntArray(brickPathString);
			if(page == null) return null;
			int[] brickIndices = new int[brickPath.Length - 1];
			Array.Copy(brickPath, 1, brickIndices, 0, brickIndices.Length);
			return GetBrickByIndices(page, brickIndices);
		}
		public Page GetPageByBrickPath(PrintControl printControl, string brickPathString) {
			if(brickPathString == null) return null;
			int[] brickPath = ConvertStringPathToIntArray(brickPathString);
			Page page = printControl.PrintingSystem.Pages[brickPath[0]];
			return page;
		}
		protected string ConvertIntArrayPathToStrign(int[] path) {
			string pathAsString = String.Empty;
			if(path.Length != 0) {
				for(int j = 0; j < path.Length - 1; j++)
					pathAsString += path[j].ToString() + ",";
				pathAsString += path[path.Length - 1].ToString();
			}
			return pathAsString;
		}
		protected int[] ConvertStringPathToIntArray(string path) {
			List<int> listBrickPath = new List<int>();
			string temp = String.Empty;
			for(int i = 0; i < path.Length; i++) {
				if(path[i] != ',') temp += path[i];
				else {
					listBrickPath.Add(Int32.Parse(temp));
					temp = String.Empty;
				}
			}
			listBrickPath.Add(Int32.Parse(temp));
			return listBrickPath.ToArray();
		}
		protected Brick ExtendedFindBrickBy(PrintControl printControl, int pointX, int pointY, out Page page) {
			Brick brick;
			page = null;
			RectangleF brickBounds = Rectangle.Empty;
			for(int i = -searchMargin; i < searchMargin; i++) {
				for(int j = -searchMargin; j < searchMargin; j++) {
					Point screenPoint = printControl.ViewControl.PointToScreen(new Point(pointX + i, pointY + j));
					brick = printControl.FindBrickBy(screenPoint, ref page, ref brickBounds);
					if(brick != null) return brick;
				}
			}
			return null;
		}
		protected PrintControl GetPrintControlFromWindowHandle(IntPtr windowHandle) {
			return (System.Windows.Forms.Control.FromHandle(windowHandle) as ViewControl).Parent as PrintControl;
		}
		protected BrickBase GetBrickFromPrintControlWindowHandle(IntPtr windowHandle, string brickPathString) {
			PrintControl printControl = GetPrintControlFromWindowHandle(windowHandle);
			if(printControl == null) return null;
			BrickBase brick = GetBrickByPath(printControl, brickPathString);
			return brick;
		}
		public String[] GetBricksOnPage(IntPtr windowHandle, int pageIndex) {
			List<String> listBrickPath = new List<String>();
			PrintControl printControl = GetPrintControlFromWindowHandle(windowHandle);
			String path = String.Empty;
			if(printControl != null) {
				Page page = printControl.PrintingSystem.Pages[pageIndex];
				if(page == null)
					return listBrickPath.ToArray();
				List<BrickBase> bricks = page.InnerBricks;
				int[] brickPath;
				for(int i = 0; i < bricks.Count; i++) {
					if(bricks[i] is CompositeBrick) {
						path = "0," + i.ToString();
						listBrickPath.Add(path);
					}
					else {
						brickPath = null;
						path = String.Empty;
						brickPath = printControl.GetBrickPath(bricks[i] as Brick, page);
						if(brickPath != null)
							listBrickPath.Add(ConvertIntArrayPathToStrign(brickPath));
					}
				}
			}
			return listBrickPath.ToArray();
		}
		public String[] GetInnerBricks(IntPtr windowHandle, string brickPath) {
			List<String> listBrickPath = new List<String>();
			PrintControl printControl = GetPrintControlFromWindowHandle(windowHandle);
			if(printControl != null) {
				Page page = GetPageByBrickPath(printControl, brickPath);
				if(page == null) return listBrickPath.ToArray();
				int[] brickPathArray = ConvertStringPathToIntArray(brickPath);
				int[] brickIndices = new int[brickPath.Length - 1];
				String path = String.Empty;
				BrickBase topBrick = GetBrickByPath(printControl, brickPath);
				if(topBrick == null)
					return listBrickPath.ToArray();
				IList innerBricks = topBrick.InnerBrickList;
				int[] innerBrickPath;
				for(int i = 0; i < innerBricks.Count; i++) {
					if(innerBricks[i] is CompositeBrick) {
						path = "0," + i.ToString();
						listBrickPath.Add(path);
					}
					else {
						innerBrickPath = null;
						path = String.Empty;
						innerBrickPath = printControl.GetBrickPath(innerBricks[i] as Brick, page);
						if(innerBrickPath != null)
							listBrickPath.Add(ConvertIntArrayPathToStrign(innerBrickPath));
					}
				}
			}
			return listBrickPath.ToArray();
		}
		public bool GetBrickSelectedState(IntPtr windowHandle, string brickPath) {
			int[] currentBrickPath;
			string currentBrickPathAsString = String.Empty;
			PrintControl printControl = GetPrintControlFromWindowHandle(windowHandle);
			if(printControl != null) {
				FieldInfo selectionServiceField = typeof(PrintControl).GetField("selectionService", BindingFlags.NonPublic | BindingFlags.Instance);
				if(selectionServiceField != null) {
					SelectionService selection = selectionServiceField.GetValue(printControl) as SelectionService;
					if(selection != null) {
						FieldInfo selectedBricksField = selection.GetType().GetField("selectedBricks", BindingFlags.NonPublic | BindingFlags.Instance);
						if(selectedBricksField != null) {
							List<Tuple<Brick, RectangleF>> selectedBricks = selectedBricksField.GetValue(selection) as List<Tuple<Brick, RectangleF>>;
							Page page = GetPageByBrickPath(printControl, brickPath);
							foreach(var item in selectedBricks) {
								currentBrickPath = printControl.GetBrickPath(item.Item1, page);
								currentBrickPathAsString = String.Empty;
								if(currentBrickPath != null && brickPath == ConvertIntArrayPathToStrign(currentBrickPath))
									return true;
							}
						}
					}
				}
			}
			return false;
		}
	}
}
