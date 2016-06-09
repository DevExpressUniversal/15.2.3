module __aspxRichEdit {
    // this funcs works only with corrected intervals
    export class FieldContextMenuHelper {
        public static getHyperlinkResultText(subDocument: SubDocument, field: Field): string {
            var result: string = "";
            var iterator: ModelIterator = new ModelIterator(subDocument, true);
            iterator.setPosition(field.getResultStartPosition());

            var currFieldIndex: number = field.index;
            var fields: Field[] = subDocument.fields;
            do {
                if (iterator.getCurrectPosition() >= field.getResultEndPosition())
                    break;

                switch (iterator.run.type) {
                    case TextRunType.FieldCodeStartRun:
                        currFieldIndex++;
                        iterator.setPosition(fields[currFieldIndex].getResultStartPosition());
                        continue;
                    case TextRunType.FieldResultEndRun:
                        iterator.setPosition(fields[currFieldIndex].getFieldEndPosition());
                        continue;
                    case TextRunType.TextRun:
                        result += iterator.chunk.getRunText(iterator.run);
                }
            } while(iterator.moveToNextRun())
            return result;
        }

        public static showUpdateAndToogleCodeItems(fields: Field[], intervals: FixedInterval[]): boolean {
            if (fields.length == 0)
                return false;
            for (var intervalIndex: number = 0, interval: FixedInterval; interval = intervals[intervalIndex]; intervalIndex++) {
                var intervalEnd: number = interval.end();
                var fieldIndex: number = Math.max(0, Field.normedBinaryIndexOf(fields, interval.start + 1));
                var field: Field = fields[fieldIndex].getAbsolutelyTopLevelField(fields);
                var topLevelField: Field = field;

                for (fieldIndex = field.index; field = fields[fieldIndex]; fieldIndex++) {
                    if (field.showCode ? field.getSeparatorPosition() < interval.start : field.getResultEndPosition() < interval.start)
                        continue;
                    if (field.showCode ? field.getFieldStartPosition() >= intervalEnd : field.getResultStartPosition() > intervalEnd)
                        break;
                    return true;
                }

                if (topLevelField.getFieldStartPosition() == interval.start)
                    return true;
            }
            return false;
        }

        public static showCreateHyperlinkItem(fields: Field[], interval: FixedInterval): boolean {
            if (fields.length == 0)
                return true;

            var intervalEnd: number = interval.end();
            var fieldIndex: number = Math.max(0, Field.normedBinaryIndexOf(fields, interval.start + 1));
            var field: Field = fields[fieldIndex].getAbsolutelyTopLevelField(fields);

            for (fieldIndex = field.index; field = fields[fieldIndex]; fieldIndex++) {
                if (field.getFieldStartPosition() >= intervalEnd)
                    break;

                if (field.getFieldEndPosition() <= interval.start)
                    continue;

                return false;
            }
            return true;
        }

        // null in case no show context menu items. HyperlinkField in other case
        public static showHyperlinkItems(fields: Field[], interval: FixedInterval): Field {
            if (fields.length == 0)
                return null;

            var intervalEnd: number = interval.end();
            var fieldIndex: number = Math.max(0, Field.normedBinaryIndexOf(fields, interval.start + 1)); // max for case field [2, 5, 9] and selection [1, 8]
            var field: Field = fields[fieldIndex];

            if (interval.length == 0) {
                do {
                    if (field.getAllFieldIntervalWithoutBorders().contains(interval.start))
                        return field.isHyperlinkField() ? field : null;
                    field = field.parent;
                } while (field);
                return null;
            }

            if (FixedInterval.getIntersection(field.getAllFieldIntervalWithoutBorders(), interval))
                return FieldContextMenuHelper.getFinalResult(fields, interval, field)

            var parent: Field = field.parent;
            if (parent) {
                if (FixedInterval.getIntersection(parent.getAllFieldIntervalWithoutBorders(), interval))
                    return FieldContextMenuHelper.getFinalResult(fields, interval, parent);
                else {
                    field = FieldContextMenuHelper.getNextTopLevelField(fields, field.index);
                    if (!field)
                        return null;

                    return FixedInterval.getIntersection(field.getAllFieldIntervalWithoutBorders(), interval) ? FieldContextMenuHelper.getFinalResult(fields, interval, field) : null;
                }
            }

            if (interval.start <= field.getFieldStartPosition())
                return null;

            field = FieldContextMenuHelper.getNextTopLevelField(fields, field.index);
            if (!field)
                return null;

            return FixedInterval.getIntersection(field.getAllFieldIntervalWithoutBorders(), interval) ? FieldContextMenuHelper.getFinalResult(fields, interval, field) : null;
        }

        private static getNextTopLevelField(fields: Field[], fieldIndex: number): Field {
            var field: Field;
            for (fieldIndex++; field = fields[fieldIndex]; fieldIndex++)
                if (!field.parent)
                    break;
            return field;
        }

        private static getFinalResult(fields: Field[], interval: FixedInterval, field: Field): Field {
            if (!field)
                return null;

            if (!field.isHyperlinkField())
                return null;

            var nextTopLevelField: Field = FieldContextMenuHelper.getNextTopLevelField(fields, field.index);
            if (nextTopLevelField && nextTopLevelField.getCodeStartPosition() <= interval.end())
                return null;

            return field;
        }
    }
}