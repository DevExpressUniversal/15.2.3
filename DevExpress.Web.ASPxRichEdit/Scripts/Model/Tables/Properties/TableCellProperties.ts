 module __aspxRichEdit {
     export class TableCellProperties implements IEquatable<TableCellProperties>, ICloneable<TableCellProperties>, IMaskedProperties<TableCellPropertiesMask> {
         mask: TableCellPropertiesMask = TableCellPropertiesMask.UseNone;

         cellMargins: TableCellMargins = TableCellMargins.create(TableWidthUnit.createDefault(), TableWidthUnit.createDefault(), TableWidthUnit.createDefault(), TableWidthUnit.createDefault());
         borders: TableCellBorders = TableCellBorders.create(new BorderInfo(), new BorderInfo(), new BorderInfo(), new BorderInfo(), new BorderInfo(), new BorderInfo());

         hideCellMark: boolean = false; // no one don't understand what this do. Think MS Word ignore this
         noWrap: boolean = false; // If the table algorithm specified as AutoFit, it is the content of the current cell line, which is set WrapText fit without transfer
                                  // One line corresponding cells not reduce the width of the cell
         fitText: boolean = false; // Put the text on the same line cell, by manipulating the size of its text
         textDirection: TextDirection = TextDirection.LeftToRightTopToBottom; // not realised
         verticalAlignment: TableCellVerticalAlignment = TableCellVerticalAlignment.Top;
         backgroundColor: number = ColorHelper.NO_COLOR;
         foreColor: number = ColorHelper.NO_COLOR; // ??? dont used
         shading: ShadingPattern = ShadingPattern.Clear;

         public equals(obj: TableCellProperties): boolean {
             if (!obj)
                 return false;
             return this.mask == obj.mask &&
                 this.cellMargins.equals(obj.cellMargins) &&
                 this.borders.equals(obj.borders) &&
                 this.hideCellMark == obj.hideCellMark &&
                 this.noWrap == obj.noWrap &&
                 this.fitText == obj.fitText &&
                 this.textDirection == obj.textDirection &&
                 this.verticalAlignment == obj.verticalAlignment &&
                 this.backgroundColor == obj.backgroundColor &&
                 this.foreColor == obj.foreColor &&
                 this.shading == obj.shading;
         }

         public clone(): TableCellProperties {
             var result = new TableCellProperties();
             result.copyFrom(this);
             return result;
         }

         public copyFrom(obj: TableCellProperties) {
             this.mask = obj.mask;
             this.cellMargins = obj.cellMargins.clone();
             this.borders = obj.borders.clone();
             this.hideCellMark = obj.hideCellMark;
             this.noWrap = obj.noWrap;
             this.fitText = obj.fitText;
             this.textDirection = obj.textDirection;
             this.verticalAlignment = obj.verticalAlignment;
             this.backgroundColor = obj.backgroundColor;
             this.foreColor = obj.foreColor;
             this.shading = obj.shading;
         }

         setUseValue(mask: TableCellPropertiesMask, value: boolean) {
             if(value)
                 this.mask |= mask;
             else
                 this.mask &= ~mask;
         }
         getUseValue(mask: TableCellPropertiesMask): boolean {
             return (this.mask & mask) != 0;
         }
     }

     export enum TableCellPropertiesMask {
         UseNone = 0x00000000,
         //UsePreferredWidth = 0x00000001,
         UseHideCellMark = 0x00000002,
         UseNoWrap = 0x00000004,
         UseFitText = 0x00000008,
         UseLeftMargin = 0x00000010,
         UseRightMargin = 0x00000020,
         UseTopMargin = 0x00000040,
         UseBottomMargin = 0x00000080,
         UseTextDirection = 0x00000100,
         UseVerticalAlignment = 0x00000200,
         // missed 0x00000400. scary touch this
         UseCellConditionalFormatting = 0x00000800,
         UseLeftBorder = 0x00001000,
         UseRightBorder = 0x00002000,
         UseTopBorder = 0x00004000,
         UseBottomBorder = 0x00008000,
         //UseInsideHorizontalBorder = 0x00010000, // this type of border only for tables
         //UseInsideVerticalBorder = 0x000020000,
         UseTopLeftDiagonalBorder = 0x00040000, 
         UseTopRightDiagonalBorder = 0x00080000,
         UseBackgroundColor = 0x00100000,
         UseForegroundColor = 0x00200000,
         UseShading = 0x00400000,
         UseAll = 0x7FFFFFFF
     }
 }