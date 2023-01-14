namespace Crea
{
    public class Hash
    {
        public static string GenerateHash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new AuctionSiteArgumentNullException("password cannot be null");
            }

            // Uses SHA256 to create the hash
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                //Convert the string to a byte array first, to be processed
                var textBytes = System.Text.Encoding.UTF8.GetBytes(input);
                var hashBytes = sha.ComputeHash(textBytes);

                //Convert back to a string, removing the '-' that BitConverter adds
                var hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }
    }
}
