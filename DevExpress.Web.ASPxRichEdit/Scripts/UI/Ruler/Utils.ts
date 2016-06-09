module __aspxRichEdit {
    export module Ruler {

        // Enums
        export enum DivisionType {
            Number = 0,
            Minor = 1,
            Major = 2
        }
        export enum RulerAction {
            None = 0,
            MarginLeft = 1,
            MarginRight = 2,
            FirstLineIndent = 3,
            RightIdent = 4,
            LeftIdent = 5,
            HangingLeftIdent = 6,

            ColumntMove = 7,
            ColumnSpace = 8,
            ColumnWidth = 9,

            Tab = 10
        }
        export enum TabAction {
            None = 0,
            Insert = 1,
            Delete = 2,
            Move = 3
        }

        export enum SnapTo {
            LeftSide = 0,
            RightSide = 1
        }

        export enum RulerVisibility {
            Auto = 0,
            Visible = 1,
            Hidden = 2,
        }

        // Constants
        export var RICH_EDIT_CLASS_NAME_PREFIX: string = "dxre-";
        export var RULER_CLASS_NAME: string = RICH_EDIT_CLASS_NAME_PREFIX + "ruler";
        export var DIVISION_CONTAINER_CLASS_NAME: string = RULER_CLASS_NAME + "Divisions";
        export var LEFT_IDENT_DRAG_HANDLE_BODY: string = RICH_EDIT_CLASS_NAME_PREFIX + "leftIdentDragHandleBody";

        export var DIVISION_CLASS_NAME: string = "Division";
        export var DIVISION_MINOR_CLASS_NAME: string = RULER_CLASS_NAME + "Minor" + DIVISION_CLASS_NAME;
        export var DIVISION_MAJOR_CLASS_NAME: string = RULER_CLASS_NAME + "Major" + DIVISION_CLASS_NAME;
        export var DIVISION_NUMBER_CLASS_NAME: string = RULER_CLASS_NAME + "Number" + DIVISION_CLASS_NAME;
        export var DIVISION_MARGIN_LEFT_CURSOR_CLASS_NAME: string = RULER_CLASS_NAME + "MarginLeftHandlePanel";
        export var DIVISION_MARGIN_RIGHT_CURSOR_CLASS_NAME: string = RULER_CLASS_NAME + "MarginRightHandlePanel";
        export var DIVISION_MARGIN_LEFT_CLASS_NAME: string = RULER_CLASS_NAME + "MarginLeftPanel";
        export var DIVISION_MARGIN_RIGHT_CLASS_NAME: string = RULER_CLASS_NAME + "MarginRightPanel";

        export var COLUMN_HANDLE_CLASS_NAME: string = RICH_EDIT_CLASS_NAME_PREFIX + "columnHandle";
        export var COLUMN_LEFT_PART_HANDLE_CLASS_NAME: string = RICH_EDIT_CLASS_NAME_PREFIX + "columnHandleLeftPart";
        export var COLUMN_RIGHT_PART_HANDLE_CLASS_NAME: string = RICH_EDIT_CLASS_NAME_PREFIX + "columnHandleRightPart";
        export var TAB_ALIGN_BOX_PART_HANDLE_CLASS_NAME: string = RICH_EDIT_CLASS_NAME_PREFIX + "rulertabAlignBox";

        export var MINOR_TOP_AND_BOTTOM_MARGIN: number = 4;
        export var MAJOR_TOP_AND_BOTTOM_MARGIN: number = 2;
        export var MINIMUN_DISTANCE_BETWEEN_HANDLE: number = 7; //100 twips
        export var MINIMUN_DISTANCE_BETWEEN_COLUMNS: number = 35;
        export var RULLER_NUMBER_CORRECTION: number = 5;

        export class RulerUtils {
            static getTabTitlePropertyName(align: TabAlign) {
                var alignString = "";
                switch (align) {
                    case TabAlign.Left:
                        alignString = "Left";
                        break;
                    case TabAlign.Right:
                        alignString = "Right";
                        break;
                    case TabAlign.Center:
                        alignString = "Center";
                        break;
                    case TabAlign.Decimal:
                        alignString = "Decimal";
                        break;
                }
                return "tab" + alignString;
            }
            static getSpriteClassName(tabAlign: TabAlign, settings: RulerSettings): string {
                switch (tabAlign) {
                    case TabAlign.Left:
                        return settings.styles.tabImages.left.spriteCssClass;
                        break;
                    case TabAlign.Right:
                        return settings.styles.tabImages.right.spriteCssClass;
                        break;
                    case TabAlign.Center:
                        return settings.styles.tabImages.center.spriteCssClass;
                        break;
                    case TabAlign.Decimal:
                        return settings.styles.tabImages.decimal.spriteCssClass;
                        break;
                }
                return "";
            }
        }
    }
}