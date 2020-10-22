using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Infrastructures
{
  public class Md5PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
  {
    public string HashPassword(TUser user, string password)
    {
      var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
      MD5Digest digest = new MD5Digest();
      digest.BlockUpdate(passwordBytes, 0, passwordBytes.Length);
      byte[] md5Bytes = new byte[digest.GetDigestSize()];
      digest.DoFinal(md5Bytes, 0);
      string hex = BitConverter.ToString(md5Bytes).ToLower().Replace("-", "");
      return hex;
    }

    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
      if (hashedPassword.Equals(HashPassword(null, providedPassword)))
      {
        return PasswordVerificationResult.Success;
      }
      else
      {
        return PasswordVerificationResult.Failed;
      }
    }
  }
}
