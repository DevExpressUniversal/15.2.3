module __aspxRichEdit {
    export class Errors {
        // old
        static ThrowNotImplementedError() {
            throw new Error("The method is not implemented.");
        }
        //new
        static ArgumentException(argument: string, value: any): string {
            return argument + " is not a valid value for " + (value ? value.toString() : typeof (value));
        }
        static NotImplemented: string = "The method is not implemented.";
        static InternalException: string = "Internal exception.";
        static ValueCannotBeNull: string = "Value cannot be null."
    }
} 