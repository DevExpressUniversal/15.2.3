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
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.NativeBricks;
namespace DevExpress.XtraPrinting.Native {
	public interface IBrickFactory {
		Brick CreateBrick();
	}
	public static class BrickFactory {
		public static event BrickResolveEventHandler BrickResolve;
		static Brick RaiseBrickResolve(string brickType) {
			if(BrickResolve != null) {
				BrickResolveEventArgs args = new BrickResolveEventArgs(brickType);
				BrickResolve(null, args);
				return args.Brick;
			} return null;
		}
		static readonly Dictionary<string, IBrickFactory> factories = new Dictionary<string, IBrickFactory>();
		static readonly IBrickFactory DefaultFactory = new DefaultBrickFactory<VisualBrick>();
		static BrickFactory() {
			RegisterFactory(BrickTypes.Visual, DefaultFactory);
			RegisterFactory(BrickTypes.Text, new DefaultBrickFactory<TextBrick>());
			RegisterFactory(BrickTypes.PageInfoText, new DefaultBrickFactory<PageInfoTextBrick>());
			RegisterFactory(BrickTypes.Panel, new DefaultBrickFactory<PanelBrick>());
			RegisterFactory(BrickTypes.CheckBox, new DefaultBrickFactory<CheckBoxBrick>());
			RegisterFactory(BrickTypes.CheckBoxText, new DefaultBrickFactory<CheckBoxTextBrick>());
			RegisterFactory(BrickTypes.Line, new DefaultBrickFactory<LineBrick>());
			RegisterFactory(BrickTypes.Image, new DefaultBrickFactory<ImageBrick>());
			RegisterFactory(BrickTypes.Empty, new DefaultBrickFactory<EmptyBrick>());
			RegisterFactory(BrickTypes.ContainerBase, new DefaultBrickFactory<BrickContainerBase>());
			RegisterFactory(BrickTypes.Container, new DefaultBrickFactory<BrickContainer>());
			RegisterFactory(BrickTypes.Default, new DefaultBrickFactory<Brick>());
			RegisterFactory(BrickTypes.PageInfo, new DefaultBrickFactory<PageInfoBrick>());
#if !SL
			RegisterFactory(BrickTypes.XECheckBox, new DefaultBrickFactory<XECheckBoxBrick>());
			RegisterFactory(BrickTypes.XECheckBoxText, new DefaultBrickFactory<XECheckBoxTextBrick>());
			RegisterFactory(BrickTypes.XEToggleSwitch, new DefaultBrickFactory<XEToggleSwitchBrick>());
			RegisterFactory(BrickTypes.XEToggleSwitchText, new DefaultBrickFactory<XEToggleSwitchTextBrick>());
			RegisterFactory(BrickTypes.XETextPanel, new DefaultBrickFactory<XETextPanelBrick>());
			RegisterFactory(BrickTypes.Label, new DefaultBrickFactory<LabelBrick>());
			RegisterFactory(BrickTypes.GroupingPageInfoText, new DefaultBrickFactory<GroupingPageInfoTextBrick>());
			RegisterFactory(BrickTypes.Table, new DefaultBrickFactory<TableBrick>());
			RegisterFactory(BrickTypes.Row, new DefaultBrickFactory<RowBrick>());
			RegisterFactory(BrickTypes.ZipCode, new DefaultBrickFactory<ZipCodeBrick>());
			RegisterFactory(BrickTypes.Shape, new DefaultBrickFactory<ShapeBrick>());
			RegisterFactory(BrickTypes.BarCode, new DefaultBrickFactory<BarCodeBrick>());
			RegisterFactory(BrickTypes.ProgressBar, new DefaultBrickFactory<ProgressBarBrick>());
			RegisterFactory(BrickTypes.PageImage, new DefaultBrickFactory<PageImageBrick>());
			RegisterFactory(BrickTypes.PageUser, new DefaultBrickFactory<UserPageBrick>());
			RegisterFactory(BrickTypes.PageTable, new DefaultBrickFactory<PageTableBrick>());
			RegisterFactory(BrickTypes.ToggleSwitch, new DefaultBrickFactory<ToggleSwitchBrick>());
			RegisterFactory(BrickTypes.ToggleSwitchText, new DefaultBrickFactory<ToggleSwitchTextBrick>());
			RegisterFactory(BrickTypes.TableOfContentsLine, new DefaultBrickFactory<TableOfContentsLineBrick>());
#endif
		}
		public static void SetFactory(string brickType, IBrickFactory factory) {
			factories[brickType] = factory;
		}
		public static void RegisterFactory(string brickType, IBrickFactory factory) {
			factories.Add(brickType, factory);
		}
		internal static BrickStyle CreateBrickStyle(XtraItemEventArgs e) {
			BrickStyle brickStyle = new BrickStyle();
			((Document)e.RootObject).StylesSerializationCache.AddDeserializationObject(brickStyle, e);
			return brickStyle;
		}
		internal static Brick CreateBrick(string brickType) {
			IBrickFactory factory;
			if(factories.TryGetValue(brickType, out factory))
				return factory.CreateBrick();
			else {
				Brick brick = RaiseBrickResolve(brickType);
				return brick != null ? brick : DefaultFactory.CreateBrick();
			}
		}
		internal static Brick CreateBrick(XtraItemEventArgs e) {
			string brickType = GetStringProperty(e, "BrickType");
			Brick brick = CreateBrick(brickType);
			((Document)e.RootObject).BricksSerializationCache.AddDeserializationObject(brick, e);
			return brick;
		}
		internal static string GetStringProperty(XtraItemEventArgs e, string propertyName) {
			return e.Item.ChildProperties[propertyName].Value.ToString();
		}
	}
}
