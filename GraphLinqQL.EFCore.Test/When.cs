namespace GraphLinqQL
{
    public class When
    {

#nullable disable warnings
        public bool Parse { get; set; }
        public bool CodeGeneration { get; set; }
        public bool Execute { get; set; }

        public float LanguageVersion { get; set; } = 8;
        public string Namespace { get; set; } = "Testing";
#nullable restore
    }
}
