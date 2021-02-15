namespace DevOidc.Core.Models.Dtos
{
    public class KeyDto
    {
        public string? KeyType { get; set; }

        public string? Algorithm { get; set; }

        public string? Use { get; set; }

        public string? Id { get; set; }

        public string? Modulus { get; set; }

        public string? Exponent { get; set; }
    }
}
