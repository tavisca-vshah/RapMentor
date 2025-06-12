using System;
using System.Security.Cryptography;
using System.Text;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    public class DynamoDbSettings
    {
        public string TableName { get; set; }

        public string Region { get; set; }

        public string Signature => GetSignature();

        private string GetSignature()
        {
            string signature = string.Empty;
            var data = Encoding.UTF8.GetBytes($"{TableName}-{Region}");
            using (var sha = SHA512.Create())
            {
                signature = Convert.ToBase64String(sha.ComputeHash(data));
            }
            return signature;
        }
    }
}
