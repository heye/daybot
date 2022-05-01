using System;
using System.Collections.Generic;
using System.Text;

namespace Daybot
{
  class Utils
  {
    public static long Now()
    {
      return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
    }
  }
}
