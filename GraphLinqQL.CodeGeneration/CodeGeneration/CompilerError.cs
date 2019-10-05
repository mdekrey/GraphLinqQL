namespace GraphLinqQL.CodeGeneration
{
    public class CompilerError
    {
        public string FileName { get; }
        public int Line { get; }
        public int Column { get; }
        public string ErrorNumber { get; }
        public string ErrorText { get; }

        public bool IsWarning { get; set; }

        public CompilerError(string fileName, int line, int column, string errorNumber, string errorText)
        {
            this.FileName = fileName;
            this.Line = line;
            this.Column = column;
            this.ErrorNumber = errorNumber;
            this.ErrorText = errorText;
        }
    }
}